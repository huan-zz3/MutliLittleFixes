using TaleWorlds.MountAndBlade;

namespace MutliLittleFixes.Patches
{
    /// <summary>
    ///     当步兵第一排蹲下时（盾阵/线阵/散阵），修改盾牌方向从 DefendDown 为 DefendUp。
    ///
    ///     原版 GetShieldDirectionOfUnit 在 ShieldWall/Circle/Square 阵型中为第一排返回
    ///     DefendDown（盾牌向下遮挡），但蹲下的士兵应当举盾向上遮挡头部/上身。
    /// </summary>
    internal static class ShieldDirectionForCrouchPatch
    {
        internal static void AdjustForCrouch(
            Agent unit,
            ref Agent.UsageDirection __result)
        {
            // MCM 运行时开关 — 关闭时不干预
            if (Settings.Instance?.CrouchShieldDirectionEnabled != true)
                return;

            if (__result == Agent.UsageDirection.DefendDown && unit?.CrouchMode == true)
            {
                __result = Agent.UsageDirection.DefendUp;
            }
        }
    }
}
