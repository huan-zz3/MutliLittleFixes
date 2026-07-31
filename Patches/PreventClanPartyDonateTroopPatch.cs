using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace ExampleMod.Patches
{
    /// <summary>
    /// 禁止家族部队捐兵：
    /// 阻断玩家家族的非主角部队在进入要塞时向要塞捐兵。
    ///
    /// 覆盖场景：己方新征服要塞（原代码唯一会触发玩家家族部队捐兵的场景）
    ///
    /// 注意：只 Patch ManageGarrisonForParty，不堵 OnSettlementEntered。
    /// OnSettlementEntered 还包含军团主管理驻军等关键逻辑，堵了会导致状态不同步崩溃。
    ///
    /// 不带 [HarmonyPatch] 属性，由 HarmonyPatchRegistry 显式注册。
    /// 运行时开关检查：关闭时 Prefix 直接 return true 放行，零开销。
    /// </summary>
    internal static class PreventClanPartyDonateTroopPatch
    {
        internal static bool Prefix(MobileParty mobileParty, Settlement settlement)
        {
            // 运行时检查 MCM 开关，关闭时放行
            if (Settings.Instance?.PreventClanPartyDonateTroops == false)
                return true;

            // 仅阻止玩家家族非主角部队的驻军管理（含捐兵）
            if (mobileParty.LeaderHero?.Clan == Clan.PlayerClan && !mobileParty.IsMainParty)
                return false;

            return true;
        }
    }
}
