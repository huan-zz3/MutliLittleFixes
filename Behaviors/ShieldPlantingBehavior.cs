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
    /// 任何携带盾牌且装备远程武器（弓/弩，不含标枪）的步兵（不含骑射手）都可把盾牌插在地上作为障碍物。
    ///
    /// 玩家方：按 F11 插盾、按 J 收盾。选中了阵型（有听令阵型）时只作用于选中阵型中符合条件的
    /// 士兵；未选中阵型时本方全部符合条件的士兵。
    ///
    /// 自动插盾/收盾（命令驱动，仅玩家方士兵）：当编队处于"就位"（StandYourGround）或移动到位
    /// （Move 命令且已抵达目标点）等非移动战斗命令时，士兵静止一段时间后自动插盾；当编队收到
    /// 冲锋/开战/后退/撤退/跟随/攻击实体等移动战斗命令时立即收盾。位置驱动的收盾（离开插盾点
    /// 超过 2 米）【仅】在玩家下发 Move 命令（前往新位置）或阵型变换命令（Arrangement*/Form*，
    /// 士兵需要重新走位）后才允许——士兵被敌方冲击/碰撞被迫位移时不会收盾。
    /// 自动操作不影响手动 F11/J，手动操作后 3 秒内自动逻辑不干预该士兵。
    /// 静止判定使用 Agent.MovementVelocity 移动速度（同 AutoCrouchMissionLogic 的做法）：
    /// AI 士兵原地作战时位置会有微小波动（转身/姿态调整/开火步幅），速度才准确反映是否真正在移动。
    ///
    /// 插盾后士兵卸下盾牌（地上生成盾牌实体作为掩体），可继续用远程武器射击。
    /// 士兵阵亡/逃跑或战斗结束时自动清理地上的盾牌实体（实体保留作遗留障碍物）。
    ///
    /// MCM 实时开关（游戏中修改立即生效）：
    /// - ShieldPlantingEnabled：总开关，关闭时收回所有已插盾牌并停止干预
    /// - ShieldPlantingAutoDeployEnabled：命令驱动的自动插盾/收盾
    /// </summary>
    public class ShieldPlantingBehavior : MissionLogic
    {
        private const float PLACE_DISTANCE = 0.65f;
        private const float KEY_COOLDOWN = 0.5f;

        // 自动插盾/收盾参数
        private const float AUTO_SCAN_INTERVAL = 2f;    // 自动逻辑扫描间隔（秒）
        private const float AUTO_STATIONARY_TIME = 1f;    // 静止多少秒后自动插盾
        private const float AUTO_STATIONARY_SPEED_SQ = 0.01f; // 移动速度平方低于该值（<0.1 m/s）视为静止（同 AutoCrouchMissionLogic）
        private const float AUTO_UNDEPLOY_DISTANCE = 0.5f;  // 离开插盾点超过该距离（米）自动收盾
        private const float AUTO_MANUAL_COOLDOWN = 3f;    // 手动 F11/J 后该秒数内自动逻辑不干预
        private const float AUTO_SUMMARY_INTERVAL = 5f;   // 调试日志扫描摘要间隔（秒）

        private static readonly InputKey DEPLOY_KEY = InputKey.F11;
        private static readonly InputKey UNDEPLOY_KEY = InputKey.J;

        // 已插盾 agent → 地上的盾牌实体
        private readonly Dictionary<Agent, GameEntity> _deployedAgents = new Dictionary<Agent, GameEntity>();
        // 已插盾 agent → 盾牌原装备槽位（收盾时归还）
        private readonly Dictionary<Agent, EquipmentIndex> _shieldSlots = new Dictionary<Agent, EquipmentIndex>();
        // 已插盾 agent → 卸盾前保存的原盾牌 MissionWeapon（含物品修饰符与当前耐久，收盾时原样归还）
        private readonly Dictionary<Agent, MissionWeapon> _savedShieldWeapons = new Dictionary<Agent, MissionWeapon>();
        // 未插盾 agent → 静止计时（秒）；已插盾 agent → 记录插盾点用于离开距离判定
        private readonly Dictionary<Agent, float> _stationaryTime = new Dictionary<Agent, float>();
        // 已插盾 agent → 插盾点位置（离开超过 AUTO_UNDEPLOY_DISTANCE 米则自动收盾）
        private readonly Dictionary<Agent, Vec3> _plantPoints = new Dictionary<Agent, Vec3>();
        // 被玩家 Move 命令要求前往新位置的士兵（仅这些士兵允许"离开插盾点后收盾"；
        // 士兵被敌方冲击/碰撞被迫位移时【不】收盾）
        private readonly HashSet<Agent> _moveOrderedAgents = new HashSet<Agent>();
        // 玩家命令事件订阅标志（懒订阅 PlayerOrderController.OnOrderIssued）
        private bool _orderEventsSubscribed;
        // agent → 最近一次手动 F11/J 操作的时间（自动逻辑防抖用）
        private readonly Dictionary<Agent, float> _lastManualActionTime = new Dictionary<Agent, float>();
        // agent → 上次扫描时的编队命令类型（命令变化调试日志用）
        private readonly Dictionary<Agent, OrderType> _lastOrderTypes = new Dictionary<Agent, OrderType>();

        private float _cooldown;
        private float _autoTick;
        private float _summaryTick = AUTO_SUMMARY_INTERVAL; // 首次扫描立即输出调试摘要
        // 缓冲池：每扫描周期已自动插盾的人数（不超过 MCM 上限 ShieldPlantingMaxAutoDeployPerScan，
        // 防止大量盾远程兵同一时刻放盾造成卡顿；手动 F11/J 不受限）
        private int _autoDeployCountThisScan;
        // 超限提示去重（每扫描周期只提示一次）
        private bool _autoDeployLimitLogged;
        // 缓冲池：每扫描周期已自动收盾的人数（与插盾【独立计数】，各自最多
        // ShieldPlantingMaxAutoDeployPerScan 次，防止同一时刻大量收盾造成卡顿；手动 F11/J 不受限）
        private int _autoUndeployCountThisScan;
        // 收盾超限提示去重（每扫描周期只提示一次）
        private bool _autoUndeployLimitLogged;
        private bool _manualCooldownLogged;   // 防抖提示去重（每次冷却期只提示一次）
        private bool _disabledLogged;         // 开关关闭提示去重

        public override void OnMissionTick(float dt)
        {
            if (Mission == null || Mission.Mode == MissionMode.Deployment)
                return;

            // 懒订阅玩家命令事件：Move 命令 → 记录需要前往新位置的士兵（位置驱动收盾的允许集合）
            if (!_orderEventsSubscribed && Mission.PlayerTeam?.PlayerOrderController != null)
            {
                Mission.PlayerTeam.PlayerOrderController.OnOrderIssued += OnPlayerOrderIssued;
                _orderEventsSubscribed = true;
            }

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

            // 命令驱动的自动插盾/收盾（节流扫描）
            _autoTick -= dt;
            if (_autoTick <= 0f)
            {
                _autoTick = AUTO_SCAN_INTERVAL;
                TickAutoPlanting(dt);
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
            {
                Deploy(agent);
                _lastManualActionTime[agent] = Mission.CurrentTime;
            }

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
            {
                Undeploy(agent);
                _lastManualActionTime[agent] = Mission.CurrentTime;
            }

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
            // 记录插盾点：自动收盾用（士兵离开插盾点 AUTO_UNDEPLOY_DISTANCE 米则收盾）
            _plantPoints[agent] = agent.Position;
            _stationaryTime[agent] = 0f;
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

            // 清除自动逻辑追踪状态
            _plantPoints.Remove(agent);
            _stationaryTime.Remove(agent);
            _moveOrderedAgents.Remove(agent);
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
        /// 插盾条件：携带盾牌 + 装备远程武器（弓/弩，不含标枪）+ 未骑乘（不含骑射手）。
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
                    || type == ItemObject.ItemTypeEnum.Crossbow)
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
                    _plantPoints.Remove(kvp.Key!);
                    _stationaryTime.Remove(kvp.Key!);
                    _lastManualActionTime.Remove(kvp.Key!);
                    _lastOrderTypes.Remove(kvp.Key!);
                    _moveOrderedAgents.Remove(kvp.Key!);
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

        // ── 命令驱动的自动插盾/收盾 ──────────────────────────────────────

        /// <summary>
        /// 玩家命令事件回调（PlayerOrderController.OnOrderIssued）：
        /// - Move / 阵型变换（Arrangement*/Form*）命令：记录被命令重新走位的士兵
        ///   （仅这些士兵允许"离开插盾点后自动收盾"，避免被动位移误收盾）；
        /// - 其它任何命令（就位/冲锋/开战等）：使旧的移动目标作废。
        /// </summary>
        private void OnPlayerOrderIssued(OrderType orderType, MBReadOnlyList<Formation> appliedFormations,
            OrderController orderController, object[] delegateParams)
        {
            switch (orderType)
            {
                // 玩家下达新目标点（Move）或变换阵型（Arrangement*/Form*）：
                // 士兵需要重新走位，允许离开插盾点后收盾
                case OrderType.Move:
                case OrderType.MoveToLineSegment:
                case OrderType.MoveToLineSegmentWithHorizontalLayout:
                case OrderType.ArrangementLine:
                case OrderType.ArrangementCloseOrder:
                case OrderType.ArrangementLoose:
                case OrderType.ArrangementCircular:
                case OrderType.ArrangementSchiltron:
                case OrderType.ArrangementVee:
                case OrderType.ArrangementColumn:
                case OrderType.ArrangementScatter:
                case OrderType.FormCustom:
                case OrderType.FormDeep:
                case OrderType.FormWide:
                case OrderType.FormWider:
                    _moveOrderedAgents.Clear();
                    if (appliedFormations == null) return;
                    foreach (Formation f in appliedFormations)
                    {
                        if (f == null || f.CountOfUnits <= 0 || f.Team != Mission.PlayerTeam) continue;
                        f.ApplyActionOnEachUnit(a =>
                        {
                            if (a is Agent agent && agent.Team == Mission.PlayerTeam)
                                _moveOrderedAgents.Add(agent);
                        });
                    }
                    break;
                default:
                    // 其它命令：旧的移动目标作废（位置驱动收盾不再允许）
                    _moveOrderedAgents.Clear();
                    break;
            }
        }

        /// <summary>
        /// 自动插盾/收盾主逻辑（仅玩家方士兵，AUTO_SCAN_INTERVAL 节流调用）：
        /// - 编队处于移动战斗命令（冲锋/开战/后退/撤退/跟随/攻击实体）时，已插盾士兵立即收盾；
        /// - 编队处于"就位"（StandYourGround）或移动到位（Move 命令）等非移动战斗命令时，
        ///   士兵位移小于阈值持续 AUTO_STATIONARY_TIME 秒 → 自动插盾；
        ///   已插盾士兵离开插盾点超过 AUTO_UNDEPLOY_DISTANCE 米 → 自动收盾
        ///   （对应"就位后玩家给予新坐标点需要前往"的场景，士兵移动后自动收起盾牌）。
        /// 手动 F11/J 操作后 AUTO_MANUAL_COOLDOWN 秒内不自动干预，尊重玩家手动意图。
        /// </summary>
        private void TickAutoPlanting(float dt)
        {
            bool debug = Settings.Instance?.ShieldPlantingDebugLog == true;
            bool autoEnabled = Settings.Instance?.ShieldPlantingAutoDeployEnabled == true;

            // 调试摘要计时
            _summaryTick -= dt;
            bool summaryDue = _summaryTick <= 0f;
            if (summaryDue) _summaryTick = AUTO_SUMMARY_INTERVAL;

            if (!autoEnabled)
            {
                // 自动插盾关闭时：清理未插盾士兵的静止计时（已插盾的保持不动，交回手动控制）
                foreach (Agent a in _stationaryTime.Keys.ToList())
                {
                    if (!_deployedAgents.ContainsKey(a))
                        _stationaryTime.Remove(a);
                }
                if (debug && summaryDue && !_disabledLogged)
                {
                    LogDebug("[插盾自动] 自动插盾开关=关，仅手动 F11/J 生效（MCM: Shield Planting & Formation → Auto Plant / Pick Up on Orders）");
                    _disabledLogged = true;
                }
                return;
            }
            _disabledLogged = false;

            Team? playerTeam = Mission.PlayerTeam;
            if (playerTeam == null) return;

            // 缓冲池：每扫描周期最多自动插盾/收盾人数（MCM 实时读取；插盾与收盾各自独立计数，
            // 每周期各最多 maxAutoDeployPerScan 次，手动 F11/J 不受限）
            int maxAutoDeployPerScan = Settings.Instance?.ShieldPlantingMaxAutoDeployPerScan ?? 5;
            _autoDeployCountThisScan = 0;
            _autoDeployLimitLogged = false;
            _autoUndeployCountThisScan = 0;
            _autoUndeployLimitLogged = false;

            int candidateCount = 0, movingOrderCount = 0, holdOrderCount = 0;
            bool anyManualCooldown = false;

            foreach (Agent agent in Mission.Agents)
            {
                if (!agent.IsActive() || agent.Team != playerTeam || agent.IsMainAgent)
                    continue;
                if (!IsPlantableAgent(agent) && !_deployedAgents.ContainsKey(agent))
                    continue;

                candidateCount++;

                // 读取编队当前命令（无编队视为 None）
                Formation? formation = agent.Formation;
                OrderType orderType = formation == null
                    ? OrderType.None
                    : formation.GetReadonlyMovementOrderReference().OrderType;

                // 命令变化调试日志
                if (debug && _lastOrderTypes.TryGetValue(agent, out OrderType prevType) && prevType != orderType)
                    LogDebug($"[插盾自动] {agent.Name} 命令变化: {prevType} → {orderType}");
                _lastOrderTypes[agent] = orderType;

                // 手动操作防抖：尊重玩家最近的手动意图
                if (_lastManualActionTime.TryGetValue(agent, out float manualTime)
                    && Mission.CurrentTime - manualTime < AUTO_MANUAL_COOLDOWN)
                {
                    anyManualCooldown = true;
                    if (debug && !_manualCooldownLogged)
                    {
                        LogDebug($"[插盾自动] {agent.Name} 手动操作后 {AUTO_MANUAL_COOLDOWN:F0}s 防抖中，自动逻辑跳过");
                        _manualCooldownLogged = true;
                    }
                    continue;
                }

                if (IsMovingCombatOrder(orderType))
                {
                    // 冲锋/开战/后退/撤退/跟随/攻击实体 → 移动战斗命令，立即收盾
                    movingOrderCount++;
                    if (_deployedAgents.ContainsKey(agent))
                    {
                        // 缓冲池节流：收盾与插盾独立计数，每扫描周期最多自动收盾 maxAutoDeployPerScan 人。
                        // 超限士兵保持已插盾状态，下一周期命令仍是移动战斗命令时再次尝试，逐周期消化。
                        if (_autoUndeployCountThisScan >= maxAutoDeployPerScan)
                        {
                            if (debug && !_autoUndeployLimitLogged)
                            {
                                LogDebug($"[插盾自动] 本扫描周期已达收盾上限 {maxAutoDeployPerScan} 人，其余士兵顺延到下一周期");
                                _autoUndeployLimitLogged = true;
                            }
                        }
                        else
                        {
                            Undeploy(agent);
                            _autoUndeployCountThisScan++;
                            if (debug)
                                LogDebug($"[插盾自动] {agent.Name} 自动收盾（移动战斗命令 {orderType}）");
                        }
                    }
                    _stationaryTime[agent] = 0f;
                    continue;
                }
                holdOrderCount++;

                // 就位/移动到位命令：按移动速度判定静止（同 AutoCrouchMissionLogic 的做法，
                // 用 MovementVelocity 速度而非位置位移——AI 士兵原地作战时位置会有微小波动
                // （转身/姿态调整/开火步幅），速度才准确反映是否真正在移动）
                Vec3 pos = agent.Position;
                if (_deployedAgents.ContainsKey(agent))
                {
                    // 已插盾：仅当玩家下发过 Move 命令（需要前往新位置）且士兵离开插盾点超过
                    // AUTO_UNDEPLOY_DISTANCE 米时才收盾。士兵被敌方冲击/碰撞被迫位移（无玩家
                    // 移动命令）时【不】收盾——盾留在原地，士兵回阵位即可继续使用。
                    if (_moveOrderedAgents.Contains(agent)
                        && _plantPoints.TryGetValue(agent, out Vec3 plantPoint)
                        && pos.Distance(plantPoint) > AUTO_UNDEPLOY_DISTANCE)
                    {
                        // 缓冲池节流：收盾与插盾独立计数（每周期各 maxAutoDeployPerScan 次）。
                        // 超限时保持已插盾状态，下一周期士兵离插盾点更远，再次满足条件即收盾。
                        if (_autoUndeployCountThisScan >= maxAutoDeployPerScan)
                        {
                            if (debug && !_autoUndeployLimitLogged)
                            {
                                LogDebug($"[插盾自动] 本扫描周期已达收盾上限 {maxAutoDeployPerScan} 人，其余士兵顺延到下一周期");
                                _autoUndeployLimitLogged = true;
                            }
                        }
                        else
                        {
                            if (debug)
                                LogDebug($"[插盾自动] {agent.Name} 自动收盾（玩家 Move 命令，离开插盾点 {pos.Distance(plantPoint):F1}m）");
                            Undeploy(agent);
                            _autoUndeployCountThisScan++;
                        }
                    }
                }
                else
                {
                    // 未插盾前的资格检查（同 AutoCrouch）：玩家控制的角色不自动插盾、交互中（如攻城器械）不插盾
                    if (!agent.IsAIControlled || agent.InteractingWithAnyGameObject())
                        continue;

                    // 静止判定：移动速度低于阈值持续累积 AUTO_STATIONARY_TIME 秒 → 插盾
                    // 注意：本方法每 AUTO_SCAN_INTERVAL 秒才调用一次，传入的 dt 是帧时间（≈0.016s），
                    // 静止计时必须按扫描间隔累积（近似 0.5s/次），否则 2s 静止要累积 30 秒才够。
                    if (agent.MovementVelocity.LengthSquared <= AUTO_STATIONARY_SPEED_SQ)
                    {
                        _stationaryTime[agent] = _stationaryTime.TryGetValue(agent, out float t) ? t + AUTO_SCAN_INTERVAL : AUTO_SCAN_INTERVAL;
                        if (_stationaryTime[agent] >= AUTO_STATIONARY_TIME)
                        {
                            // 缓冲池节流：每扫描周期最多自动插盾 maxAutoDeployPerScan 人。
                            // 超限士兵的静止计时保持已达阈值状态（不清零），顺延到下一扫描周期插盾，
                            // 逐周期消化积压，避免同一时刻大量士兵放盾造成卡顿。
                            if (_autoDeployCountThisScan >= maxAutoDeployPerScan)
                            {
                                if (debug && !_autoDeployLimitLogged)
                                {
                                    LogDebug($"[插盾自动] 本扫描周期已达插盾上限 {maxAutoDeployPerScan} 人，其余士兵顺延到下一周期");
                                    _autoDeployLimitLogged = true;
                                }
                                continue;
                            }
                            if (debug)
                                LogDebug($"[插盾自动] {agent.Name} 自动插盾（速度静止 {AUTO_STATIONARY_TIME:F0}s，命令 {orderType}）");
                            _stationaryTime[agent] = 0f;
                            Deploy(agent);
                            _autoDeployCountThisScan++;
                        }
                    }
                    else
                    {
                        if (debug && _stationaryTime.TryGetValue(agent, out float st) && st > 0f)
                            LogDebug($"[插盾自动] {agent.Name} 移动中（速度 {agent.MovementVelocity.Length:F2} m/s），静止计时清零，暂不插盾");
                        _stationaryTime[agent] = 0f;
                    }
                }
            }

            // 防抖提示去重重置：所有 agent 都离开冷却期后允许下一次提示
            if (!anyManualCooldown) _manualCooldownLogged = false;

            // 扫描摘要（每 AUTO_SUMMARY_INTERVAL 秒一次）
            if (debug && summaryDue)
            {
                LogDebug($"[插盾自动] 扫描: 候选={candidateCount} 已插盾={_deployedAgents.Count} | 移动命令={movingOrderCount} 就位/到位={holdOrderCount} | 手动防抖中={anyManualCooldown}");
            }
        }

        /// <summary>
        /// 编队是否处于移动战斗命令（此时应收盾）：
        /// 冲锋/冲锋目标/开战/前进十步/后退/后退十步/撤退/跟随/跟随实体/攻击实体。
        /// "就位"（StandYourGround）与移动到目标点（Move）不在其列，交给位移检测决定。
        /// </summary>
        private static bool IsMovingCombatOrder(OrderType orderType)
        {
            switch (orderType)
            {
                case OrderType.Charge:
                case OrderType.ChargeWithTarget:
                case OrderType.Advance:
                case OrderType.AdvanceTenPaces:
                case OrderType.FallBack:
                case OrderType.FallBackTenPaces:
                case OrderType.Retreat:
                case OrderType.FollowMe:
                case OrderType.FollowEntity:
                case OrderType.AttackEntity:
                    return true;
                default:
                    // StandYourGround / Move / None 等
                    return false;
            }
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
                _plantPoints.Remove(affectedAgent);
                _stationaryTime.Remove(affectedAgent);
                _lastManualActionTime.Remove(affectedAgent);
                _lastOrderTypes.Remove(affectedAgent);
                _moveOrderedAgents.Remove(affectedAgent);
            }
        }

        public override void OnRemoveBehavior()
        {
            // 退订玩家命令事件
            if (_orderEventsSubscribed && Mission.PlayerTeam?.PlayerOrderController != null)
            {
                Mission.PlayerTeam.PlayerOrderController.OnOrderIssued -= OnPlayerOrderIssued;
                _orderEventsSubscribed = false;
            }

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
            _plantPoints.Clear();
            _stationaryTime.Clear();
            _lastManualActionTime.Clear();
            _lastOrderTypes.Clear();
            _moveOrderedAgents.Clear();
        }

        // ── 调试日志 ──

        /// <summary>
        /// 该士兵当前是否已插盾（供 ShieldBearerFormationBehavior 查询：
        /// 已插盾的士兵在站位重排中被跳过，防止"重排把盾弩换位 → 士兵离开插盾点自动收盾 →
        /// 重新有盾又被排回前排"的循环导致站位重排无法收敛）。
        /// </summary>
        public bool IsDeployed(Agent agent) => _deployedAgents.ContainsKey(agent);

        private static void LogDebug(string message)
        {
            if (Settings.Instance?.ShieldPlantingDebugLog != true)
                return;

            InformationManager.DisplayMessage(
                new InformationMessage(message, Color.FromUint(0x00FF00u)));
        }
    }
}
