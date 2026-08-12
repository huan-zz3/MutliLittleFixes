using TaleWorlds.CampaignSystem;

namespace MutliLittleFixes.Patches
{
    /// <summary>
    /// 流亡家族永不灭亡：
    /// 阻断 FactionDiscontinuationCampaignBehavior 对无国无地家族（灭国后流浪的家族）
    /// 的 28 天生存倒计时灭亡机制，使其永久存续。
    ///
    /// 实现：Prefix 拦截 DailyTickClan。该方法是原版唯一触发 DiscontinueClan（灭族）的入口，
    /// 开关开启时直接跳过原方法体，倒计时到期也不会执行 DestroyClanAction；
    /// 开关关闭时 return true 放行，恢复原版 28 天倒计时灭亡行为。
    ///
    /// 注意：只阻断"流亡家族"的倒计时灭亡，不影响王国被灭（DiscontinueKingdom）流程本身、
    /// 叛乱失败灭族、领主死亡灭族等其他原版机制。
    ///
    /// 不带 [HarmonyPatch] 属性，由 HarmonyPatchRegistry 显式注册。
    /// 运行时开关检查：关闭时 Prefix 直接 return true 放行，零开销。
    /// </summary>
    internal static class WanderingClanSurvivalPatch
    {
        internal static bool Prefix(Clan clan)
        {
            // MCM 运行时开关 — 关闭时放行原方法，恢复原版倒计时灭亡
            if (Settings.Instance?.WanderingClanSurvivalEnabled != true)
                return true;

            // 开启时跳过 DailyTickClan：家族永久存续，不再触发灭族
            return false;
        }
    }
}
