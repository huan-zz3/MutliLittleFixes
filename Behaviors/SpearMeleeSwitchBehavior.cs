using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace MutliLittleFixes
{
    /// <summary>
    /// 长矛兵近身换刀：
    /// 携带长杆 + 单手近战武器的 AI 步兵，在近身肉搏（敌人进入换刀距离）时
    /// 自动从长杆切换到单手武器，防止长杆刺击在贴身距离判定落空（卡刀）；
    /// 敌人拉开距离后自动换回长杆，保持对骑兵冲锋的刺击/架矛能力。
    ///
    /// 行为（由 MCM 开关实时控制，敌我双方 AI 对称生效，玩家本人不受影响）：
    ///   - 当前持长杆 且 最近敌人距离 &lt; 换刀距离 → 切换到单手武器槽
    ///   - 当前持单手 且 最近敌人距离 &gt; 回切距离（或无敌人）→ 切回长杆槽
    ///   - 切换带冷却（单兵防抖），避免在阈值边界来回抽刀
    ///
    /// 架矛（被动攻击）本身由原生引擎在条件满足时自动触发，本行为不干预，
    /// 只负责"何时该用长杆、何时该用单刀"的武器选择。
    ///
    /// 由 SubModule.OnMissionBehaviorInitialize 注册为 MissionLogic（不写 Harmony 属性）。
    /// </summary>
    public class SpearMeleeSwitchBehavior : MissionLogic
    {
        private const float CheckInterval = 0.5f;    // 扫描间隔（秒）
        private const float EnemyScanRadius = 8.0f;  // 敌人搜索基础半径（米），覆盖回切距离上限
        private const float SwitchCooldown = 1.0f;   // 单个士兵两次切换的最小间隔（秒）

        private float _checkTimer;

        // 已被本功能切换到单手武器的士兵 → 最近一次切换时间（冷却 + MCM 关闭时恢复用）
        private readonly Dictionary<Agent, float> _switchedToMeleeAgents = new Dictionary<Agent, float>();
        // 敌人搜索缓冲（复用，避免每 tick 分配）
        private readonly MBList<Agent> _nearbyEnemies = new MBList<Agent>();

        public override void OnMissionTick(float dt)
        {
            if (Mission == null || Mission.Mode == MissionMode.Deployment)
                return;

            // 海战禁用（战帆 DLC 海战/沿海掠夺海战）— 统一原则：士兵 AI 行为调整在海战不干预
            if (NavalBattleDetector.IsNavalBattle(Mission))
                return;

            // MCM 总开关 — 关闭时把所有被本功能切到单刀的士兵恢复长杆并停止干预
            if (Settings.Instance?.SpearMeleeSwitchEnabled != true)
            {
                RestoreAllAgentsToPolearm();
                return;
            }

            _checkTimer += dt;
            if (_checkTimer < CheckInterval)
                return;
            _checkTimer = 0f;

            TickSwitchLogic();
        }

        // ── 核心切换逻辑 ───────────────────────────────────────────────

        private void TickSwitchLogic()
        {
            float switchToMeleeDist = Settings.Instance?.SpearMeleeSwitchDistance ?? 2.0f;
            float switchBackDist = Settings.Instance?.SpearMeleeSwitchBackDistance ?? 4.0f;
            if (switchBackDist < switchToMeleeDist)
                switchBackDist = switchToMeleeDist;

            float switchToMeleeDistSq = switchToMeleeDist * switchToMeleeDist;
            float switchBackDistSq = switchBackDist * switchBackDist;

            foreach (Agent agent in Mission.Agents)
            {
                if (!IsEligibleAgent(agent))
                    continue;

                EquipmentIndex polearmSlot = FindWeaponSlot(agent, IsPolearm);
                EquipmentIndex oneHandedSlot = FindWeaponSlot(agent, IsOneHandedMelee);
                if (polearmSlot == EquipmentIndex.None || oneHandedSlot == EquipmentIndex.None)
                    continue;

                EquipmentIndex wieldedSlot = agent.GetPrimaryWieldedItemIndex();
                if (wieldedSlot == EquipmentIndex.None)
                    continue;

                // 找最近敌人距离（平方距离比较，免开方）
                float nearestEnemyDistSq = GetNearestEnemyDistanceSq(agent, switchBackDist);

                bool wieldingPolearm = wieldedSlot == polearmSlot || IsPolearmSlot(agent, wieldedSlot);
                bool wieldingOneHanded = IsOneHandedMeleeSlot(agent, wieldedSlot);

                if (wieldingPolearm && nearestEnemyDistSq < switchToMeleeDistSq)
                {
                    SwitchToSlot(agent, oneHandedSlot);
                }
                else if (wieldingOneHanded && nearestEnemyDistSq > switchBackDistSq)
                {
                    SwitchToSlot(agent, polearmSlot);
                }
            }
        }

        // ── 敌人距离 ───────────────────────────────────────────────────

        /// <summary>
        /// 返回以 agent 为中心、扫描半径内最近敌人的距离平方；无敌人时返回正无穷。
        /// 复用 Mission.GetNearbyEnemyAgents（引擎按队伍空间查询，一次调用拿全列表）。
        /// </summary>
        private float GetNearestEnemyDistanceSq(Agent agent, float scanRadius)
        {
            if (agent.Team == null)
                return float.MaxValue;

            float radius = MathF.Max(EnemyScanRadius, scanRadius);
            Mission.GetNearbyEnemyAgents(agent.Position.AsVec2, radius, agent.Team, _nearbyEnemies);

            float nearestSq = float.MaxValue;
            foreach (Agent enemy in _nearbyEnemies)
            {
                if (enemy == null || !enemy.IsActive())
                    continue;
                float distSq = agent.Position.DistanceSquared(enemy.Position);
                if (distSq < nearestSq)
                    nearestSq = distSq;
            }
            return nearestSq;
        }

        // ── 切换动作 ───────────────────────────────────────────────────

        /// <summary>
        /// 把士兵主手强制切换到指定槽位（近战武器切换，Instant 无装填状态机问题；
        /// 与盾牌插地中禁止对弩兵使用 Instant 的告诫无关——弩兵不满足本行为的装备条件）。
        /// </summary>
        private void SwitchToSlot(Agent agent, EquipmentIndex targetSlot)
        {
            // 防抖：切换冷却期内不重复切换
            if (_switchedToMeleeAgents.TryGetValue(agent, out float lastSwitchTime)
                && Mission.CurrentTime - lastSwitchTime < SwitchCooldown)
            {
                return;
            }

            if (agent.GetPrimaryWieldedItemIndex() == targetSlot)
                return;

            agent.TryToWieldWeaponInSlot(targetSlot, Agent.WeaponWieldActionType.Instant, false);
            _switchedToMeleeAgents[agent] = Mission.CurrentTime;
        }

        /// <summary>
        /// MCM 开关关闭时：把所有仍存活且已被切到单刀的士兵恢复长杆，并清空追踪状态。
        /// </summary>
        private void RestoreAllAgentsToPolearm()
        {
            if (_switchedToMeleeAgents.Count == 0)
                return;

            foreach (Agent agent in _switchedToMeleeAgents.Keys)
            {
                if (agent == null || !agent.IsActive())
                    continue;

                EquipmentIndex polearmSlot = FindWeaponSlot(agent, IsPolearm);
                if (polearmSlot == EquipmentIndex.None)
                    continue;

                if (agent.GetPrimaryWieldedItemIndex() != polearmSlot)
                    agent.TryToWieldWeaponInSlot(polearmSlot, Agent.WeaponWieldActionType.Instant, false);
            }
            _switchedToMeleeAgents.Clear();
        }

        // ── 筛选与武器分类 ─────────────────────────────────────────────

        private static bool IsEligibleAgent(Agent agent)
            => agent != null && agent.IsActive() && agent.IsHuman
               && agent.IsAIControlled && !agent.IsMainAgent
               && !agent.HasMount && agent.Team != null;

        /// <summary>
        /// 在武器槽位（Weapon0-3 + 额外槽）中找满足条件的第一个槽位。
        /// </summary>
        private static EquipmentIndex FindWeaponSlot(Agent agent, System.Func<MissionWeapon, bool> predicate)
        {
            for (EquipmentIndex i = EquipmentIndex.WeaponItemBeginSlot; i < EquipmentIndex.NumAllWeaponSlots; i++)
            {
                MissionWeapon weapon = agent.Equipment[i];
                if (weapon.IsEmpty || weapon.Item == null)
                    continue;
                if (predicate(weapon))
                    return i;
            }
            return EquipmentIndex.None;
        }

        /// <summary>长杆：物品类型为 Polearm（含长矛/长柄武器；架矛/骑枪同属此类）。</summary>
        private static bool IsPolearm(MissionWeapon weapon)
            => weapon.Item != null && weapon.Item.ItemType == ItemObject.ItemTypeEnum.Polearm;

        /// <summary>单手近战：物品类型为 OneHandedWeapon（单手剑/斧/锤/匕首等，可配盾）。</summary>
        private static bool IsOneHandedMelee(MissionWeapon weapon)
            => weapon.Item != null && weapon.Item.ItemType == ItemObject.ItemTypeEnum.OneHandedWeapon;

        private static bool IsPolearmSlot(Agent agent, EquipmentIndex slot)
        {
            if (slot < EquipmentIndex.WeaponItemBeginSlot || slot >= EquipmentIndex.NumAllWeaponSlots)
                return false;
            return IsPolearm(agent.Equipment[slot]);
        }

        private static bool IsOneHandedMeleeSlot(Agent agent, EquipmentIndex slot)
        {
            if (slot < EquipmentIndex.WeaponItemBeginSlot || slot >= EquipmentIndex.NumAllWeaponSlots)
                return false;
            return IsOneHandedMelee(agent.Equipment[slot]);
        }

        // ── 清理 ───────────────────────────────────────────────────────

        public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
        {
            if (affectedAgent != null)
                _switchedToMeleeAgents.Remove(affectedAgent);
        }

        public override void OnRemoveBehavior()
        {
            _switchedToMeleeAgents.Clear();
            _nearbyEnemies.Clear();
        }
    }
}
