using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;

namespace ExampleMod.Patches
{
    /// <summary>
    /// 禁止家族部队被别的军团征召：
    /// 在 AI 领主创建军团筛选候选部队时，从候选列表中剔除玩家家族的非主角部队。
    ///
    /// 不带 [HarmonyPatch] 属性，由 SubModule 根据 MCM 开关手动安装。
    /// 确保设置关闭时完全不 patch 原版方法，避免与其他 mod 冲突。
    /// </summary>
    internal static class PreventClanPartyRecruitmentPatch
    {
        internal static void Postfix(
            MobileParty mobileParty,
            ref MBList<MobileParty> possibleArmyMembers,
            ref bool __result)
        {
            if (!__result || possibleArmyMembers == null || possibleArmyMembers.Count == 0)
                return;

            for (int i = possibleArmyMembers.Count - 1; i >= 0; i--)
            {
                if (possibleArmyMembers[i].LeaderHero?.Clan == Clan.PlayerClan
                    && possibleArmyMembers[i] != MobileParty.MainParty)
                {
                    possibleArmyMembers.RemoveAt(i);
                }
            }

            if (possibleArmyMembers.Count == 0)
                __result = false;
        }
    }
}
