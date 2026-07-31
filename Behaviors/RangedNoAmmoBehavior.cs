using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.GauntletUI;

namespace ExampleMod
{
    /// <summary>
    /// 远程部队弹药耗尽自动移入第9队。
    /// 
    /// 功能:
    ///   1. 每 ~1s 检测 Ranged 阵型（排除 HorseArcher）中弹药耗尽的 AI 士兵，
    ///      将其从原阵型 RemoveUnit 后 AddUnit 到第9队（FormationClass 索引 8）。
    ///   2. 持续检测第9队中已恢复弹药的士兵，自动归还原阵型。
    ///   3. 按数字键 9 可选中第9队，对其下达指令（与 1~8 队相同的全部命令）。
    ///   4. 第9队初始化时屏蔽原版 BehaviorGeneral，设为玩家控制 + Stop 指令。
    ///   5. 玩家本人跳过检测。
    /// </summary>
    public class RangedNoAmmoBehavior : MissionLogic
    {
        private const float CheckInterval = 2.0f;
        private const FormationClass NoAmmoFormationClass = (FormationClass)8;

        private float _checkTimer;
        private bool _initialized;
        private bool _startupLogged;

        /// <summary>
        /// 记录被移入第9队的 Agent → 其原始阵型的映射，用于归队。
        /// </summary>
        private readonly Dictionary<Agent, Formation> _movedAgents = new();

        public override void OnMissionTick(float dt)
        {
            if (Mission == null || Mission.Mode == MissionMode.Deployment)
                return;

            // MCM 实时开关 — 关闭时归还第9队士兵并重置状态，重新启用时重新初始化
            if (Settings.Instance?.RangedNoAmmoEnabled != true)
            {
                if (_initialized)
                {
                    ReturnAllMovedAgents();
                    _initialized = false;
                    _startupLogged = false;
                }
                return;
            }

            // 启动诊断（仅首次）
            if (!_startupLogged)
            {
                _startupLogged = true;
                InformationManager.DisplayMessage(new InformationMessage(
                    "[第9队] RangedNoAmmoBehavior 已加载"));
            }

            // 首次运行时初始化第9队
            if (!_initialized)
            {
                InitializeNoAmmoFormation();
                _initialized = true;
            }

            // 按键输入逐帧处理，不被计时器阻挡
            ProcessInput();

            _checkTimer += dt;
            if (_checkTimer < CheckInterval)
                return;
            _checkTimer = 0f;

            ProcessNoAmmoDetection();
            ProcessReturnDetection();
        }

        /// <summary>
        /// 开关关闭时调用：将所有仍滞留在第9队的士兵归还其原始阵型并清空记录。
        /// </summary>
        private void ReturnAllMovedAgents()
        {
            if (_movedAgents.Count == 0)
                return;

            var toRemove = new List<Agent>(_movedAgents.Count);
            int returned = 0;

            foreach (var kvp in _movedAgents)
            {
                Agent agent = kvp.Key;
                Formation originalFormation = kvp.Value;

                // 已死亡 → 仅清理记录
                if (!agent.IsActive())
                {
                    toRemove.Add(agent);
                    continue;
                }

                if (originalFormation != null)
                {
                    agent.Formation = originalFormation;
                    toRemove.Add(agent);
                    returned++;
                }
            }

            foreach (Agent agent in toRemove)
            {
                _movedAgents.Remove(agent);
            }

            if (returned > 0)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    $"[第9队] 功能已关闭，{returned} 名弓手已归还至原阵型"));
            }
        }

        // ── 初始化 ────────────────────────────────────────────────────────

        /// <summary>
        /// 将第9队设为玩家控制、屏蔽 BehaviorGeneral、默认 Stop。
        /// </summary>
        private void InitializeNoAmmoFormation()
        {
            foreach (Team team in Mission.Teams)
            {
                if (team != Mission.PlayerTeam && team != Mission.PlayerAllyTeam)
                    continue;

                var formation8 = team.GetFormation(NoAmmoFormationClass);
                if (formation8 == null)
                    continue;

                // 设置 PlayerOwner 为玩家，使得 OrderController 可以选中该阵型
                // 同时 SetControlledByAI(false) 由 setter 自动调用
                formation8.PlayerOwner = Agent.Main;
                formation8.SetMovementOrder(MovementOrder.MovementOrderStop);
                formation8.SetArrangementOrder(ArrangementOrder.ArrangementOrderLoose);
            }
        }

        // ── 弹药耗尽检测 ──────────────────────────────────────────────────

        private void ProcessNoAmmoDetection()
        {
            foreach (Team team in Mission.Teams)
            {
                if (team != Mission.PlayerTeam && team != Mission.PlayerAllyTeam)
                    continue;

                foreach (Formation formation in team.FormationsIncludingSpecialAndEmpty)
                {
                    // 跳过空阵型和第9队自身
                    if (formation.CountOfUnits == 0)
                        continue;
                    if ((int)formation.FormationIndex == (int)NoAmmoFormationClass)
                        continue;

                    ScanFormationForNoAmmo(formation);
                }
            }
        }

        /// <summary>
        /// 扫描单个阵型中弹药耗尽的 Agent，移入第9队。
        /// 先收集需要移动的 Agent 列表，遍历结束后再统一执行移动，
        /// 避免在 ApplyActionOnEachUnit 遍历期间修改阵型内 Agent.Formation 导致集合变更异常。
        /// </summary>
        private void ScanFormationForNoAmmo(Formation formation)
        {
            var agentsToMove = new List<Agent>();

            formation.ApplyActionOnEachUnit(agent =>
            {
                if (agent == Mission.MainAgent)
                    return;

                if (_movedAgents.ContainsKey(agent))
                    return;

                if (IsOutOfAmmo(agent))
                {
                    agentsToMove.Add(agent);
                }
            });

            int moved = agentsToMove.Count;
            foreach (Agent agent in agentsToMove)
            {
                MoveToNoAmmoFormation(agent, formation.Team);
            }

            if (moved > 0)
            {
                int squadIndex = (int)formation.FormationIndex + 1;
                InformationManager.DisplayMessage(new InformationMessage(
                    $"[第9队] 从第{squadIndex}小队检测到 {moved} 名远程弹药耗尽，已移入第9队"));
            }
        }

        /// <summary>
        /// 判定士兵是否弹药耗尽。
        /// 条件：
        /// - Ranged 兵种：有弓/弩武器 + 所有弹药槽总量 ≤ 0
        /// - HorseArcher 兵种：下马状态下 + 有弓/弩武器 + 所有弹药槽总量 ≤ 0（马上骑射手不参与移交）
        /// </summary>
        private static bool IsOutOfAmmo(Agent agent)
        {
            if (!agent.IsAIControlled)
                return false;

            FormationClass troopClass = agent.Character?.GetFormationClass() ?? FormationClass.Unset;
            FormationClass defaultClass = troopClass.DefaultClass();

            // Ranged → 直接参与
            if (defaultClass == FormationClass.Ranged)
            {
                // 继续检查武器
            }
            // HorseArcher → 仅下马状态才视为远程，参与移交
            else if (defaultClass == FormationClass.HorseArcher)
            {
                if (agent.HasMount)
                    return false; // 骑在马上不算
            }
            else
            {
                return false; // 其它兵种排除
            }

            bool hasBowOrCrossbow = false;
            int totalAmmo = 0;

            for (EquipmentIndex idx = EquipmentIndex.WeaponItemBeginSlot;
                 idx < EquipmentIndex.NumAllWeaponSlots; idx++)
            {
                MissionWeapon weapon = agent.Equipment[idx];
                if (weapon.IsEmpty)
                    continue;

                WeaponComponentData? usage = weapon.CurrentUsageItem;
                if (usage == null)
                    continue;

                // 检测弓/弩（仅限 bow/crossbow，不含投掷武器）
                if (usage.WeaponClass == WeaponClass.Bow ||
                    usage.WeaponClass == WeaponClass.Crossbow)
                {
                    hasBowOrCrossbow = true;
                }

                // 累积弹药量（IsAnyAmmo 仅对箭矢/弩矢等弹药品为 true）
                if (weapon.IsAnyAmmo())
                {
                    totalAmmo += weapon.Amount;
                }
            }

            return hasBowOrCrossbow && totalAmmo <= 0;
        }

        // ── 移入第9队 ────────────────────────────────────────────────────

        private void MoveToNoAmmoFormation(Agent agent, Team team)
        {
            Formation? currentFormation = agent.Formation;
            Formation noAmmoFormation = team.GetFormation(NoAmmoFormationClass);

            if (currentFormation == null || currentFormation == noAmmoFormation)
                return;

            // 记录原阵型，用于后续归队
            _movedAgents[agent] = currentFormation;

            // 使用原版 Agent.Formation 设置器进行阵型转移，
            // 自动处理 native 引擎站位更新、SetPositioning、ForceUpdateCachedAndFormationValues 等
            agent.Formation = noAmmoFormation;

            // 确保第9队持续为玩家控制
            if (noAmmoFormation.IsAIControlled)
            {
                noAmmoFormation.SetControlledByAI(false);
            }
        }

        // ── 归队检测 ──────────────────────────────────────────────────────

        private void ProcessReturnDetection()
        {
            if (_movedAgents.Count == 0)
                return;

            var toRemove = new List<Agent>(_movedAgents.Count);
            int returned = 0;
            int died = 0;

            foreach (var kvp in _movedAgents)
            {
                Agent agent = kvp.Key;
                Formation originalFormation = kvp.Value;

                // 已死亡 → 清理记录
                if (!agent.IsActive())
                {
                    toRemove.Add(agent);
                    died++;
                    continue;
                }

                // 已恢复弹药 → 归还（使用原版 Agent.Formation 设置器）
                if (!IsOutOfAmmo(agent))
                {
                    if (originalFormation != null)
                    {
                        agent.Formation = originalFormation;
                    }

                    toRemove.Add(agent);
                    returned++;
                }
            }

            foreach (Agent agent in toRemove)
            {
                _movedAgents.Remove(agent);
            }

            if (returned > 0)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    $"[第9队] {returned} 名弓手已恢复弹药，归还至原阵型"));
            }

            if (died > 0)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    $"[第9队] {died} 名弓手已阵亡，从第9队记录中清理"));
            }
        }

        private bool _d9WasDown;

        // ── 按键: 9 → 选中第9队 ──────────────────────────────────────────

        private void ProcessInput()
        {
            // 手动边缘检测：仅在按下瞬间触发
            bool d9IsDown = Input.IsKeyDown(InputKey.D9);
            bool risingEdge = d9IsDown && !_d9WasDown;
            _d9WasDown = d9IsDown;
            if (!risingEdge)
                return;

            Team? playerTeam = Mission.PlayerTeam;

            if (playerTeam == null)
            {
                InformationManager.DisplayMessage(new InformationMessage("[第9队] playerTeam == null"));
                return;
            }

            Formation formation8 = playerTeam.GetFormation(NoAmmoFormationClass);
            if (formation8 == null)
            {
                InformationManager.DisplayMessage(new InformationMessage("[第9队] GetFormation(8) == null"));
                return;
            }
            if (formation8.CountOfUnits <= 0)
            {
                InformationManager.DisplayMessage(new InformationMessage("[第9队] 第9队为空"));
                return;
            }

            // 每按9重新确保 PlayerOwner（某些系统可能会重置它）
            formation8.PlayerOwner = Agent.Main;

            OrderController controller = playerTeam.PlayerOrderController;
            if (controller == null)
            {
                InformationManager.DisplayMessage(new InformationMessage("[第9队] PlayerOrderController == null"));
                return;
            }
            if (!controller.IsFormationSelectable(formation8))
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    $"[第9队] IsFormationSelectable 返回 false（CountOfUnits={formation8.CountOfUnits}）"));
                return;
            }

            controller.ClearSelectedFormations();
            controller.SelectFormation(formation8);

            // 直接打开阵型选择 UI（绕开 TroopList 查找，因为第9队不在 FormationsIncludingEmpty 中）
            OpenToggleOrderDirectly();

            InformationManager.DisplayMessage(new InformationMessage(
                $"[第9队] 已选中（{formation8.CountOfUnits} 人），可下达指令"));
        }

        /// <summary>
        /// 直接打开阵型选择 UI，绕开 TroopList 查找（第9队不在 FormationsIncludingEmpty 中，
        /// 无法通过 SelectFormationAtIndex → OnTroopFormationSelected 路径正常弹出）。
        /// 通过反射调用 GauntletOrderUIHandler._dataSource.OpenToggleOrder()。
        /// </summary>
        private static void OpenToggleOrderDirectly()
        {
            var handler = Mission.Current.GetMissionBehavior<GauntletOrderUIHandler>();
            if (handler == null)
                return;

            var dataSourceField = typeof(GauntletOrderUIHandler).GetField("_dataSource",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var vm = dataSourceField?.GetValue(handler);
            if (vm == null)
                return;

            var openToggleOrder = vm.GetType().GetMethod("OpenToggleOrder",
                new[] { typeof(bool), typeof(bool) });
            openToggleOrder?.Invoke(vm, new object[] { false, true });
        }

    }
}
