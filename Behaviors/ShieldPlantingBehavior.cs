using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace MutliLittleFixes
{
    /// <summary>
    /// 盾牌插地（移植自参考模组 PaviseShield 并泛化为通用功能）：
    /// 任何携带盾牌且装备远程武器（弓/弩/标枪）的步兵（不含骑射手）都可把盾牌插在地上作为障碍物。
    ///
    /// 玩家方：按 F11 插盾、按 J 收盾。选中了阵型（有听令阵型）时只作用于选中阵型中符合条件的
    /// 士兵；未选中阵型时作用于本方全部符合条件的士兵。
    ///
    /// 插盾后士兵卸下盾牌（地上生成盾牌实体作为掩体），可继续用远程武器射击。
    /// 士兵阵亡/逃跑或战斗结束时自动清理地上的盾牌实体。
    ///
    /// MCM 实时开关（游戏中修改立即生效）：
    /// - ShieldPlantingEnabled：总开关，关闭时收回所有已插盾牌并停止干预
    /// </summary>
    public class ShieldPlantingBehavior : MissionLogic
    {
        private const float PLACE_DISTANCE = 0.65f;
        private const float KEY_COOLDOWN = 0.5f;

        private static readonly InputKey DEPLOY_KEY = InputKey.F11;
        private static readonly InputKey UNDEPLOY_KEY = InputKey.J;

        // 已插盾 agent → 地上的盾牌实体
        private readonly Dictionary<Agent, GameEntity> _deployedAgents = new Dictionary<Agent, GameEntity>();
        // 已插盾 agent → 盾牌原装备槽位（收盾时归还）
        private readonly Dictionary<Agent, EquipmentIndex> _shieldSlots = new Dictionary<Agent, EquipmentIndex>();
        // 已插盾 agent → 卸盾前保存的原盾牌 MissionWeapon（含物品修饰符与当前耐久，收盾时原样归还）
        private readonly Dictionary<Agent, MissionWeapon> _savedShieldWeapons = new Dictionary<Agent, MissionWeapon>();

        private float _cooldown;

        public override void OnMissionTick(float dt)
        {
            if (Mission == null || Mission.Mode == MissionMode.Deployment)
                return;

            // MCM 总开关 — 关闭时收回所有已插盾牌并停止干预
            if (Settings.Instance?.ShieldPlantingEnabled != true)
            {
                UndeployAllAgents();
                return;
            }

            _cooldown -= dt;

            // 玩家快捷键
            if (_cooldown <= 0f && Input.IsKeyPressed(DEPLOY_KEY))
            {
                _cooldown = KEY_COOLDOWN;
                ToggleDeploy();
            }

            if (_cooldown <= 0f && Input.IsKeyPressed(UNDEPLOY_KEY))
            {
                _cooldown = KEY_COOLDOWN;
                ToggleUndeploy();
            }

            // 清理死亡/逃跑的 agent
            CleanupDeadAgents();
        }

        // ── 玩家快捷键 ────────────────────────────────────────────────────

        private void ToggleDeploy()
        {
            Team? team = Mission.PlayerTeam;
            if (team == null) return;

            var toDeploy = GetTargetAgents(team)
                .Where(a => !_deployedAgents.ContainsKey(a) && IsPlantableAgent(a)).ToList();

            if (toDeploy.Count == 0)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    new TextObject("{=mlf_shield_none_found}No shield-bearing ranged soldiers found.", null).ToString(),
                    Color.FromUint(0xFFAAAAAA)));
                return;
            }

            foreach (Agent agent in toDeploy)
                Deploy(agent);

            InformationManager.DisplayMessage(new InformationMessage(
                new TextObject("{=mlf_shield_deployed}{COUNT} soldier(s) planted their shields.", null)
                .SetTextVariable("COUNT", toDeploy.Count).ToString(),
                Color.FromUint(0xFFDDAA00)));
        }

        private void ToggleUndeploy()
        {
            Team? team = Mission.PlayerTeam;
            if (team == null) return;

            var toUndeploy = GetTargetAgents(team)
                .Where(a => _deployedAgents.ContainsKey(a)).ToList();

            if (toUndeploy.Count == 0)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    new TextObject("{=mlf_shield_none_deployed}No planted shields found.", null).ToString(),
                    Color.FromUint(0xFFAAAAAA)));
                return;
            }

            foreach (Agent agent in toUndeploy)
                Undeploy(agent);

            InformationManager.DisplayMessage(new InformationMessage(
                new TextObject("{=mlf_shield_undeployed}{COUNT} soldier(s) picked up their shields.", null)
                .SetTextVariable("COUNT", toUndeploy.Count).ToString(),
                Color.FromUint(0xFFDDAA00)));
        }

        // ── 核心插盾/收盾 ────────────────────────────────────────────────

        private void Deploy(Agent agent)
        {
            EquipmentIndex slot = GetShieldSlot(agent);
            if (slot == EquipmentIndex.None) return;

            // 卸盾前保存原盾牌 MissionWeapon（含物品修饰符与当前耐久），收盾时原样归还
            _savedShieldWeapons[agent] = agent.Equipment[slot];

            try
            {
                agent.RemoveEquippedWeapon(slot);
                // 注意：卸盾后【禁止】调用 agent.TryToWieldWeaponInSlot(Weapon0, Instant, false) 强制握持弩。
                // 对 AI 弩兵强制"瞬间握持"会绕过原生装填/待发状态机：弩被强制入袋但内部装填流程未走完，
                // 导致弩手只有瞄准动画却永远射不出箭（发射被原生判定未就绪，AI 也不会重新触发装填，
                // 最终退化为近战）。卸盾与弩槽位无关，让原生 AI 自行重新握持弩即可正常 装填→瞄准→发射。
            }
            catch (Exception ex)
            {
                Debug.PrintError("盾牌插地: 卸盾失败 " + ex.Message, "MutliLittleFixes.ShieldPlanting");
            }

            GameEntity? entity = SpawnProp(agent);
            if (entity == null)
            {
                Debug.PrintError("盾牌插地: 插地盾实体生成失败", "MutliLittleFixes.ShieldPlanting");
                _savedShieldWeapons.Remove(agent);
                return;
            }

            _deployedAgents[agent] = entity;
            _shieldSlots[agent] = slot;
        }

        private void Undeploy(Agent agent)
        {
            if (!_deployedAgents.TryGetValue(agent, out GameEntity? entity)) return;

            try
            {
                entity?.Remove(0);
            }
            catch (Exception ex)
            {
                Debug.PrintError("盾牌插地: 移除插地盾实体失败 " + ex.Message, "MutliLittleFixes.ShieldPlanting");
            }
            _deployedAgents.Remove(agent);

            if (_shieldSlots.TryGetValue(agent, out EquipmentIndex slot))
            {
                _shieldSlots.Remove(agent);
                try
                {
                    // 归还卸盾前保存的原盾牌（保留修饰符与耐久）；正常路径必有保存值
                    if (_savedShieldWeapons.TryGetValue(agent, out MissionWeapon weapon) && !weapon.IsEmpty)
                    {
                        agent.EquipWeaponWithNewEntity(slot, ref weapon);
                    }
                    else
                    {
                        Debug.PrintError("盾牌插地: 未找到保存的盾牌，无法归还给 " + agent.Name, "MutliLittleFixes.ShieldPlanting");
                    }
                }
                catch (Exception ex)
                {
                    Debug.PrintError("盾牌插地: 归还盾牌失败 " + ex.Message, "MutliLittleFixes.ShieldPlanting");
                }
                _savedShieldWeapons.Remove(agent);
            }
        }

        /// <summary>
        /// 在 agent 前方 PLACE_DISTANCE 处的地面上生成插地盾实体。
        /// 实体用士兵盾牌自身的模型（ItemObject.MultiMeshName）与物理体（ItemObject.BodyName）动态生成，
        /// 任意盾型（筝形盾/圆盾/塔盾/步兵盾等）都使用匹配的模型，不再硬编码单一盾型。
        /// </summary>
        private GameEntity? SpawnProp(Agent agent)
        {
            try
            {
                // 用卸盾前保存的原盾牌生成实体（保留修饰符与当前耐久的 MissionWeapon）
                if (!_savedShieldWeapons.TryGetValue(agent, out MissionWeapon weapon) || weapon.IsEmpty)
                    return null;

                Vec3 pos = agent.Position;
                Vec2 dir = agent.GetMovementDirection();

                float len = dir.Length;
                if (len > 0.001f) dir /= len;
                else dir = new Vec2(1f, 0f);

                Vec2 pos2D = new Vec2(pos.x + dir.x * PLACE_DISTANCE, pos.y + dir.y * PLACE_DISTANCE);
                float groundZ = Mission.Scene.GetTerrainHeight(pos2D);
                Vec3 spawnPos = new Vec3(pos2D.x, pos2D.y, groundZ + 0.9f);

                Mat3 rotation = Mat3.CreateMat3WithForward(new Vec3(dir.x, dir.y, 0f));
                rotation.RotateAboutSide((float)(-Math.PI / 2.0));

                MatrixFrame frame = new MatrixFrame(rotation, spawnPos);

                // 复用原生掉落物实体创建机制：从 MissionWeapon 生成带匹配网格（MultiMeshName）的场景实体
                GameEntity entity = GameEntityExtensions.Instantiate(Mission.Scene, weapon, false, true);
                // 移除掉落物脚本（SpawnedItemEntity/UsableMissionObject），防止玩家/敌人把插地盾拾取走
                SpawnedItemEntity? itemScript = entity.GetFirstScriptOfType<SpawnedItemEntity>();
                if (itemScript != null)
                    entity.RemoveScriptComponent(itemScript.ScriptComponent.Pointer, 10);

                entity.SetGlobalFrame(in frame);

                // 加静态物理钉住（复用武器数据：物理体/质量/材质），实体不会被碰撞推飞
                WeaponData weaponData = weapon.GetWeaponData(true);
                GameEntityPhysicsExtensions.AddPhysics(
                    entity,
                    weaponData.BaseWeight,
                    weaponData.CenterOfMassShift,
                    weaponData.Shape,
                    Vec3.Zero,
                    Vec3.Zero,
                    PhysicsMaterial.GetFromIndex(weaponData.PhysicsMaterialIndex),
                    true,  // isStatic：钉在地上
                    -1);
                weaponData.DeinitializeManagedPointers();

                entity.SetPhysicsState(true, false);
                return entity;
            }
            catch (Exception ex)
            {
                Debug.PrintError("盾牌插地: 生成插地盾异常 " + ex.Message, "MutliLittleFixes.ShieldPlanting");
                return null;
            }
        }

        // ── 辅助方法 ───────────────────────────────────────────────────────

        /// <summary>
        /// 获取快捷键作用的目标 agent 列表：
        /// 有选中（听令）阵型时只作用于这些阵型中符合条件的士兵/已插盾士兵，
        /// 否则作用于本方全部符合条件的士兵/已插盾士兵。
        /// </summary>
        private List<Agent> GetTargetAgents(Team team)
        {
            var selected = team.FormationsIncludingEmpty
                .Where(f => f.CountOfUnits > 0 && team.PlayerOrderController.IsFormationListening(f))
                .ToList();

            if (selected.Count > 0)
            {
                var result = new List<Agent>();
                foreach (Formation f in selected)
                {
                    f.ApplyActionOnEachUnit(u =>
                    {
                        if (u is Agent a && (IsPlantableAgent(a) || _deployedAgents.ContainsKey(a)))
                            result.Add(a);
                    });
                }
                return result;
            }

            return Mission.Agents
                .Where(a => a.IsActive() && a.Team == team && (IsPlantableAgent(a) || _deployedAgents.ContainsKey(a)))
                .ToList();
        }

        /// <summary>
        /// 插盾条件：携带盾牌 + 装备远程武器（弓/弩/标枪）+ 未骑乘（不含骑射手）。
        /// </summary>
        private static bool IsPlantableAgent(Agent agent)
            => agent != null && agent.Character != null
               && GetShieldSlot(agent) != EquipmentIndex.None
               && HasRangedWeapon(agent)
               && !agent.HasMount;

        private static EquipmentIndex GetShieldSlot(Agent agent)
        {
            if (agent?.Character == null) return EquipmentIndex.None;
            for (EquipmentIndex i = EquipmentIndex.WeaponItemBeginSlot; i < EquipmentIndex.NumAllWeaponSlots; i++)
            {
                MissionWeapon el = agent.Equipment[i];
                if (!el.IsEmpty && el.Item?.ItemType == ItemObject.ItemTypeEnum.Shield)
                    return i;
            }
            return EquipmentIndex.None;
        }

        private static bool HasRangedWeapon(Agent agent)
        {
            if (agent?.Character == null) return false;
            for (EquipmentIndex i = EquipmentIndex.WeaponItemBeginSlot; i < EquipmentIndex.NumAllWeaponSlots; i++)
            {
                MissionWeapon el = agent.Equipment[i];
                if (el.IsEmpty || el.Item == null) continue;
                ItemObject.ItemTypeEnum type = el.Item.ItemType;
                if (type == ItemObject.ItemTypeEnum.Bow
                    || type == ItemObject.ItemTypeEnum.Crossbow
                    || type == ItemObject.ItemTypeEnum.Thrown)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 清理死亡/逃跑 agent 对应的插地盾记录。
        /// 插地盾实体【保留】在场景中作为遗留障碍物，仅移除字典追踪；
        /// 实体随 Mission 结束（场景销毁）自动消失。
        /// </summary>
        private void CleanupDeadAgents()
        {
            foreach (var kvp in _deployedAgents.ToList())
            {
                if (kvp.Key == null || !kvp.Key.IsActive())
                {
                    _deployedAgents.Remove(kvp.Key!);
                    _shieldSlots.Remove(kvp.Key!);
                    _savedShieldWeapons.Remove(kvp.Key!);
                }
            }
        }

        /// <summary>
        /// 收回所有已插盾牌并清空状态（MCM 总开关关闭时调用）。
        /// </summary>
        private void UndeployAllAgents()
        {
            if (_deployedAgents.Count == 0) return;
            foreach (Agent agent in _deployedAgents.Keys.ToList())
                Undeploy(agent);
        }

        public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
        {
            // 士兵死亡/离场时插地盾【保留】在场景中作遗留障碍物，仅清除字典追踪；
            // 实体随 Mission 结束（场景销毁）自动消失。
            if (_deployedAgents.ContainsKey(affectedAgent))
            {
                _deployedAgents.Remove(affectedAgent);
                _shieldSlots.Remove(affectedAgent);
                _savedShieldWeapons.Remove(affectedAgent);
            }
        }

        public override void OnRemoveBehavior()
        {
            foreach (GameEntity? e in _deployedAgents.Values)
            {
                try
                {
                    e?.Remove(0);
                }
                catch (Exception ex)
                {
                    Debug.PrintError("盾牌插地: OnRemoveBehavior 移除插地盾失败 " + ex.Message, "MutliLittleFixes.ShieldPlanting");
                }
            }
            _deployedAgents.Clear();
            _shieldSlots.Clear();
            _savedShieldWeapons.Clear();
        }
    }
}
