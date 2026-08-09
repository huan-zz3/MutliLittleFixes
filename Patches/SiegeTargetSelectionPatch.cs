using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;

namespace MutliLittleFixes.Patches
{
    /// <summary>
    /// 玩家作为进攻方时，攻城器械必须优先选择敌方器械为目标。
    /// 只有敌方没有任何存活器械时才允许攻击城墙。
    /// AI vs AI 场景不受影响（原版加权随机逻辑）。
    /// </summary>
    internal static class SiegeTargetSelectionPatch
    {
        internal static bool Prefix(
            BesiegerCamp __instance,
            ISiegeEventSide siegeEventSide,
            SiegeEngineType siegeEngine,
            int siegeEngineSlot,
            out SiegeBombardTargets targetType,
            out int targetIndex)
        {
            targetType = SiegeBombardTargets.None;
            targetIndex = -1;

            // MCM 运行时开关 — 关闭时放行原版逻辑
            if (Settings.Instance?.SiegeTargetSelectionEnabled != true)
                return true;

            // 仅在玩家是围城进攻方时生效，AI 场景走原版逻辑
            if (!__instance.SiegeEvent.IsPlayerSiegeEvent)
            {
                return true;
            }

            // 检查敌方是否有存活且激活的远程器械
            __instance.SiegeEvent.FindAttackableRangedEngineWithHighestPriority(
                siegeEventSide, siegeEngineSlot, out var engineTargetIdx, out _);

            if (engineTargetIdx != -1)
            {
                // 敌方有器械 → 强制选择器械为目标
                targetType = SiegeBombardTargets.RangedEngines;
                targetIndex = engineTargetIdx;
                return false; // 跳过原版加权随机逻辑
            }

            // 敌方无器械 → 回退原版逻辑（自动选择城墙或 Hold）
            return true;
        }
    }
}
