using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace ExampleMod
{
    /// <summary>
    /// 调试工具：按 "," 键将玩家部队下 5% 的远程士兵弹药强制归零，
    /// 用于测试 RangedNoAmmoBehavior 的移入第9队逻辑。
    /// 受 MCM "调试 → 远程弹药归零调试" 开关控制，可在游戏中实时启用/禁用。
    /// </summary>
    public class RangedNoAmmoDebugBehavior : MissionLogic
    {
        private bool _startupLogged;
        private bool _commaWasDown;

        public override void OnMissionTick(float dt)
        {
            if (Mission == null)
                return;

            // MCM 开关实时控制：关闭时跳过全部逻辑
            if (Settings.Instance == null || !Settings.Instance.RangedNoAmmoDebugEnabled)
            {
                _startupLogged = false;
                _commaWasDown = false;
                return;
            }

            // 启动诊断（仅首次）
            if (!_startupLogged)
            {
                _startupLogged = true;
                InformationManager.DisplayMessage(new InformationMessage(
                    "[RangedNoAmmoDebug] 已加载，按 , 测试弹药归零"));
            }

            // 手动边缘检测：按下一次只触发一次
            bool commaIsDown = Input.IsKeyDown(InputKey.Comma);
            if (!commaIsDown || _commaWasDown)
            {
                _commaWasDown = commaIsDown;
                return;
            }
            _commaWasDown = true;

            Team? playerTeam = Mission.PlayerTeam;
            if (playerTeam == null)
                return;

            // 收集玩家阵营中所有装备弓/弩的 AI 士兵
            var candidates = new List<Agent>();
            foreach (Agent agent in playerTeam.ActiveAgents)
            {
                if (!agent.IsActive() || !agent.IsHuman || !agent.IsAIControlled)
                    continue;
                if (agent == Mission.MainAgent)
                    continue;

                if (HasBowOrCrossbow(agent))
                {
                    candidates.Add(agent);
                }
            }

            if (candidates.Count == 0)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    "[RangedNoAmmoDebug] 未找到装备弓/弩的 AI 士兵"));
                return;
            }

            // 随机选取 5%（至少 1 人）
            int count = TaleWorlds.Library.MathF.Max(1, candidates.Count * 5 / 100);
            var selected = new HashSet<Agent>();

            while (selected.Count < count && selected.Count < candidates.Count)
            {
                int idx = MBRandom.RandomInt(candidates.Count);
                selected.Add(candidates[idx]);
            }

            // 弹药归零
            int zeroed = 0;
            foreach (Agent agent in selected)
            {
                ZeroOutAmmo(agent);
                zeroed++;
            }

            InformationManager.DisplayMessage(new InformationMessage(
                $"[RangedNoAmmoDebug] 已将 {zeroed}/{candidates.Count} 名远程士兵弹药归零"));
        }

        private static bool HasBowOrCrossbow(Agent agent)
        {
            for (EquipmentIndex idx = EquipmentIndex.WeaponItemBeginSlot;
                 idx < EquipmentIndex.NumAllWeaponSlots; idx++)
            {
                MissionWeapon weapon = agent.Equipment[idx];
                if (weapon.IsEmpty)
                    continue;

                WeaponComponentData? usage = weapon.CurrentUsageItem;
                if (usage == null)
                    continue;

                if (usage.WeaponClass == WeaponClass.Bow ||
                    usage.WeaponClass == WeaponClass.Crossbow)
                {
                    return true;
                }
            }
            return false;
        }

        private static void ZeroOutAmmo(Agent agent)
        {
            for (EquipmentIndex idx = EquipmentIndex.WeaponItemBeginSlot;
                 idx < EquipmentIndex.NumAllWeaponSlots; idx++)
            {
                MissionWeapon weapon = agent.Equipment[idx];
                if (weapon.IsEmpty)
                    continue;

                // IsAnyAmmo() 对箭矢/弩矢为 true
                // 使用 agent.SetWeaponAmountInSlot 走原生引擎（与 AmmoSupplyLogic 一致）
                if (weapon.IsAnyAmmo() && weapon.Amount > 0)
                {
                    agent.SetWeaponAmountInSlot(idx, 0, enforcePrimaryItem: false);
                }
            }
        }
    }
}
