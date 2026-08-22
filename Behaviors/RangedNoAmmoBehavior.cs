using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.GauntletUI;

namespace MutliLittleFixes
{
    /// <summary>
    /// 远程部队弹药耗尽自动移入目标编队（默认第9队）。
    /// 
    /// 功能:
    ///   1. 每 ~2s 检测 Ranged 阵型（排除 HorseArcher）中弹药耗尽的 AI 士兵，
    ///      将其从原阵型转移到目标编队（默认第9队 = FormationClass 索引 8）。
    ///   2. 目标编队可通过 MCM 配置（1~9，默认 9）；战斗中修改立即生效。
    ///   3. 一经移交不再归还——士兵永久留在目标编队，即使恢复弹药也不移回原阵型。
    ///   4. 按数字键 9 可选中目标编队（仅当目标=第9队时生效；目标为标准编队时
    ///      玩家用 1~8 数字键即可选中，按键 9 不干预）。
    ///   5. 第9队（待命池）初始化时设为玩家控制 + Stop 指令 + Loose 阵型；
    ///      标准编队（1~8）保留玩家/原版 AI 的现有指令与站位，不干预。
    ///   6. 阵型转移使用与原版手动移送（战前布阵拖拽）一致的链路：
    ///      OnMassUnitTransferStart/End 批量包装 + Team.TriggerOnFormationsChanged 事件通知。
    ///   7. 玩家本人跳过检测。
    ///   8. 阵型队长（Formation.Captain，为全队提供加成）即使弹药耗尽也保留在原阵型，不参与移交，
    ///      避免队长被移走后原版清空 Captain 导致全队加成丢失。
    /// </summary>
    public class RangedNoAmmoBehavior : MissionLogic
    {
        private const float CheckInterval = 2.0f;
        /// <summary>第9队（待命池）= 将军位 General 槽，FormationClass 索引 8；第 N 队 = FormationClass(N-1)。</summary>
        private const FormationClass StandbyFormationClass = (FormationClass)8;

        private float _checkTimer;
        private bool _initialized;
        private bool _startupLogged;
        /// <summary>当前生效的目标编队（每次 tick 从 MCM 实时解析）。</summary>
        private FormationClass _targetFormationClass = StandbyFormationClass;
        private bool _d9WasDown;

        /// <summary>
        /// 从 MCM 实时解析目标编队：设置值 1~9 → FormationClass(N-1)，第9队 = 将军位 General 槽（默认）。
        /// </summary>
        private static FormationClass GetTargetFormationClass()
        {
            int target = Settings.Instance?.RangedNoAmmoTargetFormation ?? 9;
            target = MBMath.ClampInt(target, 1, 9);
            return (FormationClass)(target - 1);
        }

        public override void OnMissionTick(float dt)
        {
            if (Mission == null || Mission.Mode == MissionMode.Deployment)
                return;

            // 海战禁用（战帆 DLC 海战/沿海掠夺海战）— 士兵绑定船编队，目标编队无船，
            // 且 NavalTeamAgents 会在夺船/转移时把士兵强制拉回船编队，移交会被还原
            if (NavalBattleDetector.IsNavalBattle(Mission))
                return;

            // MCM 实时开关 — 关闭时不干预（一经移交的士兵保持在目标编队，不归还）
            if (Settings.Instance?.RangedNoAmmoEnabled != true)
                return;

            // MCM 实时目标编队 — 变更时重新初始化（新目标为待命池时需要接管控制权与待命指令）
            FormationClass currentTarget = GetTargetFormationClass();
            if (currentTarget != _targetFormationClass)
            {
                _initialized = false;
                _startupLogged = false;
                _targetFormationClass = currentTarget;
            }

            // 启动诊断（仅首次/目标变更后）
            if (!_startupLogged)
            {
                _startupLogged = true;
                InformationManager.DisplayMessage(new InformationMessage(
                    new TextObject("{=mlf_d9_loaded}Formation {FORMATION}: RangedNoAmmoBehavior loaded", null)
                    .SetTextVariable("FORMATION", (int)_targetFormationClass + 1)
                    .ToString()));
            }

            // 首次运行时初始化目标编队
            if (!_initialized)
            {
                InitializeTargetFormation(_targetFormationClass);
                _initialized = true;
            }

            // 按键输入逐帧处理，不被计时器阻挡
            ProcessInput();

            _checkTimer += dt;
            if (_checkTimer < CheckInterval)
                return;
            _checkTimer = 0f;

            ProcessNoAmmoDetection();
        }

        // ── 初始化 ────────────────────────────────────────────────────────

        /// <summary>
        /// 初始化目标编队：仅第9队（待命池）设为玩家控制、屏蔽 BehaviorGeneral、默认 Stop + Loose；
        /// 标准编队（1~8）保留玩家/原版 AI 的现有指令与站位，不干预。
        /// </summary>
        private void InitializeTargetFormation(FormationClass targetClass)
        {
            // 标准编队由玩家/原版 AI 正常指挥，无需初始化
            if ((int)targetClass != (int)StandbyFormationClass)
                return;

            foreach (Team team in Mission.Teams)
            {
                if (team != Mission.PlayerTeam && team != Mission.PlayerAllyTeam)
                    continue;

                Formation targetFormation = team.GetFormation(targetClass);
                if (targetFormation == null)
                    continue;

                // 设置 PlayerOwner 为玩家，使得 OrderController 可以选中该阵型
                // 同时 SetControlledByAI(false) 由 setter 自动调用
                targetFormation.PlayerOwner = Agent.Main;
                targetFormation.SetMovementOrder(MovementOrder.MovementOrderStop);
                targetFormation.SetArrangementOrder(ArrangementOrder.ArrangementOrderLoose);
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
                    // 跳过空阵型和目标编队自身
                    if (formation.CountOfUnits == 0)
                        continue;
                    if ((int)formation.FormationIndex == (int)_targetFormationClass)
                        continue;

                    ScanFormationForNoAmmo(formation);
                }
            }
        }

        /// <summary>
        /// 扫描单个阵型中弹药耗尽的 Agent，批量移入目标编队（一经移交不再归还）。
        /// 先收集需要移动的 Agent 列表，遍历结束后再统一执行移动，
        /// 避免在 ApplyActionOnEachUnit 遍历期间修改阵型内 Agent.Formation 导致集合变更异常。
        /// </summary>
        private void ScanFormationForNoAmmo(Formation formation)
        {
            Formation targetFormation = formation.Team.GetFormation(_targetFormationClass);
            if (targetFormation == null || formation == targetFormation)
                return;

            var agentsToMove = new List<Agent>();

            formation.ApplyActionOnEachUnit(agent =>
            {
                if (agent == Mission.MainAgent)
                    return;

                // 队长保留在原阵型——队长为全队提供加成（Formation.Captain），
                // 即使弹药耗尽也不移交，避免原版清空 Captain 导致加成丢失
                if (agent == formation.Captain)
                    return;

                if (IsOutOfAmmo(agent))
                {
                    agentsToMove.Add(agent);
                }
            });

            if (agentsToMove.Count == 0)
                return;

            // 与原版手动移送（战前布阵拖拽）一致的批量转移：Start/End 包装 + 事件通知
            ExecuteFormationTransfer(formation, targetFormation, agentsToMove);

            // 确保待命池（第9队）持续为玩家控制——某些系统可能重置其控制权，
            // 失控会让待命士兵自动参战；标准编队保持原 AI/玩家控制状态，不干预
            if ((int)_targetFormationClass == (int)StandbyFormationClass && targetFormation.IsAIControlled)
            {
                targetFormation.SetControlledByAI(false);
            }

            int squadIndex = (int)formation.FormationIndex + 1;
            InformationManager.DisplayMessage(new InformationMessage(
                new TextObject("{=mlf_d9_moved}Formation {FORMATION}: Detected {MOVED} ranged soldiers out of ammo in squad {SQUAD_INDEX}, moved to Formation {FORMATION}", null)
                .SetTextVariable("FORMATION", (int)_targetFormationClass + 1)
                .SetTextVariable("MOVED", agentsToMove.Count)
                .SetTextVariable("SQUAD_INDEX", squadIndex)
                .ToString()));
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

        // ── 阵型转移（与原版手动移送一致）──────────────────────────────────

        /// <summary>
        /// 按原版手动移送（战前布阵拖拽）的链路执行批量阵型转移：
        /// OnMassUnitTransferStart/End 批量包装（PostponeCostlyOperations 抑制单兵重算，
        /// 结束后 ReapplyFormOrder + QuerySystem.Expire）+ Team.TriggerOnFormationsChanged
        /// 事件通知——原版 TransferUnitsAux 移送后的必要步骤，通知 DetachmentManager
        /// 清除 detachment 评分、MissionAgentLabelView 刷新名牌等。
        /// try/finally 保证 End 一定执行，避免异常导致 PostponeCostlyOperations 残留。
        /// </summary>
        private static void ExecuteFormationTransfer(Formation source, Formation target, List<Agent> agents)
        {
            if (agents.Count == 0)
                return;

            source.OnMassUnitTransferStart();
            target.OnMassUnitTransferStart();
            try
            {
                foreach (Agent agent in agents)
                {
                    // 使用原版 Agent.Formation 设置器进行阵型转移，
                    // 自动处理 native 引擎站位更新、SetPositioning、ForceUpdateCachedAndFormationValues 等
                    agent.Formation = target;
                }
            }
            finally
            {
                source.OnMassUnitTransferEnd();
                target.OnMassUnitTransferEnd();
            }

            source.Team.TriggerOnFormationsChanged(source);
            source.Team.TriggerOnFormationsChanged(target);
        }

        // ── 按键: 9 → 选中第9队 ──────────────────────────────────────────

        private void ProcessInput()
        {
            // 目标为标准编队（1~8）时按键 9 不干预——玩家可直接用对应数字键选中该编队
            if ((int)_targetFormationClass != (int)StandbyFormationClass)
                return;

            // 手动边缘检测：仅在按下瞬间触发
            bool d9IsDown = Input.IsKeyDown(InputKey.D9);
            bool risingEdge = d9IsDown && !_d9WasDown;
            _d9WasDown = d9IsDown;
            if (!risingEdge)
                return;

            Team? playerTeam = Mission.PlayerTeam;
            int formationNumber = (int)_targetFormationClass + 1;

            if (playerTeam == null)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    new TextObject("{=mlf_d9_noteam}Formation {FORMATION}: playerTeam == null", null)
                    .SetTextVariable("FORMATION", formationNumber)
                    .ToString()));
                return;
            }

            Formation formation = playerTeam.GetFormation(_targetFormationClass);
            if (formation == null)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    new TextObject("{=mlf_d9_noformation}Formation {FORMATION}: GetFormation({INDEX}) == null", null)
                    .SetTextVariable("FORMATION", formationNumber)
                    .SetTextVariable("INDEX", (int)_targetFormationClass)
                    .ToString()));
                return;
            }
            if (formation.CountOfUnits <= 0)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    new TextObject("{=mlf_d9_empty}Formation {FORMATION} is empty", null)
                    .SetTextVariable("FORMATION", formationNumber)
                    .ToString()));
                return;
            }

            // 每按9重新确保 PlayerOwner（某些系统可能会重置它）
            formation.PlayerOwner = Agent.Main;

            OrderController controller = playerTeam.PlayerOrderController;
            if (controller == null)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    new TextObject("{=mlf_d9_noordercontroller}Formation {FORMATION}: PlayerOrderController == null", null)
                    .SetTextVariable("FORMATION", formationNumber)
                    .ToString()));
                return;
            }
            if (!controller.IsFormationSelectable(formation))
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    new TextObject("{=mlf_d9_notselectable}Formation {FORMATION}: IsFormationSelectable returned false (CountOfUnits={COUNT_OF_UNITS})", null)
                    .SetTextVariable("FORMATION", formationNumber)
                    .SetTextVariable("COUNT_OF_UNITS", formation.CountOfUnits)
                    .ToString()));
                return;
            }

            controller.ClearSelectedFormations();
            controller.SelectFormation(formation);

            // 直接打开阵型选择 UI（绕开 TroopList 查找，因为第9队不在 FormationsIncludingEmpty 中）
            OpenToggleOrderDirectly();

            InformationManager.DisplayMessage(new InformationMessage(
                new TextObject("{=mlf_d9_selected}Formation {FORMATION} selected ({COUNT_OF_UNITS} units), orders can be issued", null)
                .SetTextVariable("FORMATION", formationNumber)
                .SetTextVariable("COUNT_OF_UNITS", formation.CountOfUnits)
                .ToString()));
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
