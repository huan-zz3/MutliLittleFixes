using System;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.MountAndBlade;

namespace MutliLittleFixes.Patches
{
    /// <summary>
    ///     修复原版 <see cref="LineFormation.SwitchFrontUnitTypesToFrontRows" /> 的收敛缺陷，
    ///     保证持盾兵（或架矛兵）在有可换对象时必然被排到最前排。
    ///
    ///     原版缺陷（两者叠加导致「首排非盾兵 + 第二排持盾兵」并存）：
    ///     1. 第 1 轮「同列冒泡」是单趟扫描：深排交换把持盾者送进第 2 排后，不再复查
    ///        第 1/2 排之间已被检查过的边界，同列「盾在非盾正后方」会残留；
    ///     2. 第 2 轮「跨列补位」从最后一排向前扫，一旦遇到「前一排全盾」的持盾者就
    ///        return 中止整个函数，跳过最前排（rank 1）的补位机会——而恰恰是 rank 1
    ///        的跨列补位才能换掉首排（rank 0）的非盾兵。
    ///
    ///     修复方式：Prefix 整体替换该方法（返回 false 跳过原方法）——
    ///     第 1 轮改为反复冒泡直至每一列都稳定（无任何「持盾者在非持盾者正后方」）；
    ///     第 2 轮保持从后向前（保证跨列晋升可逐排级联）但卡住时 continue 而非 return。
    /// </summary>
    internal static class FormationFrontRankShieldSortPatch
    {
        private static readonly FieldInfo FrontUnitDelegateField =
            AccessTools.Field(typeof(LineFormation), "_isFrontUnitDelegate");

        private static readonly PropertyInfo FileCountProperty =
            AccessTools.Property(typeof(LineFormation), "FileCount");

        private static readonly PropertyInfo IntervalProperty =
            AccessTools.Property(typeof(LineFormation), "Interval");

        internal static bool Prefix(LineFormation __instance)
        {
            try
            {
                return PrefixCore(__instance);
            }
            catch (Exception)
            {
                // 任何意外异常都降级为放行原版方法（返回 true），
                // 保证本补丁永远不会成为游戏崩溃/卡死的源头。
                return true;
            }
        }

        private static bool PrefixCore(LineFormation __instance)
        {
            // MCM 运行时开关 — 关闭时放行原方法
            if (Settings.Instance?.FormationFrontRankSortEnabled != true)
                return true;

            // 反射成员缺失（游戏版本差异）时放行原方法，安全降级
            if (FrontUnitDelegateField == null || FileCountProperty == null || IntervalProperty == null)
                return true;

            // 复刻原版 Interval <= 0 早退语义（单位间隔未就绪时不整理站位）
            if ((float)IntervalProperty.GetValue(__instance) <= 0f)
                return false;

            var frontDelegate = FrontUnitDelegateField.GetValue(__instance) as Func<Agent, bool>;
            if (frontDelegate == null)
                return true;

            int fileCount = (int)FileCountProperty.GetValue(__instance);
            int rankCount = __instance.RankCount;

            // 第 1 轮：同列冒泡，反复扫描直至该列不再有「持盾者在非持盾者正后方」。
            // 修复原版单趟扫描不复查已处理排间边界的缺陷。
            //
            // 收敛性：每次交换都把持盾者上移一排、非持盾者下移一排，严格减少列内逆序数，
            // 稳定比较器下至多 rankCount-1 趟即稳定。maxSweeps 趟数上限用于防御比较器
            // 中途抖动（如士兵整理途中换装导致持盾判定变化）的极端情形，保证循环有界。
            int maxSweeps = Math.Max(1, rankCount);
            bool swapped;
            int sweep = 0;
            do
            {
                swapped = false;
                for (int rank = 1; rank < rankCount; rank++)
                {
                    for (int file = 0; file < fileCount; file++)
                    {
                        Agent rear = __instance.GetUnit(file, rank) as Agent;
                        Agent front = __instance.GetUnit(file, rank - 1) as Agent;
                        if (rear != null && front != null && frontDelegate(rear) && !frontDelegate(front))
                        {
                            __instance.SwitchUnitLocations(rear, front);
                            swapped = true;
                        }
                    }
                }
            }
            while (swapped && ++sweep < maxSweeps);

            // 第 2 轮：跨列补位，从后排向前（保证晋升可逐排级联）。
            // 修复原版「前一排全盾时 return 中止整个函数」的缺陷：改为跳过继续，
            // 使 rank 1 → rank 0 的补位机会必然被处理。
            for (int rank = rankCount - 1; rank > 0; rank--)
            {
                for (int file = 0; file < fileCount; file++)
                {
                    if (!(__instance.GetUnit(file, rank) is Agent shield) || !frontDelegate(shield))
                        continue;

                    int frontRank = rank - 1;
                    for (int frontFile = 0; frontFile < fileCount; frontFile++)
                    {
                        if (__instance.GetUnit(frontFile, frontRank) is Agent nonShield && !frontDelegate(nonShield))
                        {
                            __instance.SwitchUnitLocations(shield, nonShield);
                            break;
                        }
                    }
                }
            }

            return false; // 已用修正逻辑完整替代原方法
        }
    }
}
