using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;

namespace MutliLittleFixes.Patches
{
    /// <summary>
    /// 禁止家族部队被别的军团征召：
    /// 在 AI 领主创建军团筛选候选部队时，从候选列表中剔除玩家家族的非主角部队。
    ///
    /// 不带 [HarmonyPatch] 属性，由 HarmonyPatchRegistry 显式注册。
    /// 内部有运行时 MCM 开关检查（null-safe），关闭时完全放行。
    /// </summary>
    internal static class PreventClanPartyRecruitmentPatch
    {
        internal static void Postfix(
            MobileParty mobileParty,
            ref MBList<MobileParty> possibleArmyMembers,
            ref bool __result)
        {
            // 运行时检查 MCM 开关，关闭时不执行任何过滤。
            // 使用 ?. 防止 Settings 尚未加载时 null 抛 NRE（Harmony 会静默吞掉异常导致过滤失效）
            if (Settings.Instance?.PreventClanPartyRecruitment == false)
                return;

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
