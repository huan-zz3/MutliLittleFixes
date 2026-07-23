using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;

namespace ExampleMod.Patches
{
    /// <summary>
    /// 禁止家族部队捐兵：
    /// 阻断玩家家族的非主角部队进入要塞管理流程，从源头防止捐兵。
    ///
    /// 覆盖场景：己方新征服要塞（原代码唯一会触发玩家家族部队捐兵的场景）
    ///
    /// 不带 [HarmonyPatch] 属性，由 SubModule 根据 MCM 开关手动安装。
    /// 确保设置关闭时完全不 patch 原版方法，避免与其他 mod 冲突。
    /// </summary>
    internal static class PreventClanPartyDonateTroopPatch
    {
        internal static bool Prefix(MobileParty mobileParty)
        {
            if (mobileParty.LeaderHero?.Clan == Clan.PlayerClan && !mobileParty.IsMainParty)
                return false;

            return true;
        }
    }
}
