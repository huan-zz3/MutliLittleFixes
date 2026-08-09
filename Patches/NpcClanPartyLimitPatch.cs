using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;

namespace MutliLittleFixes.Patches
{
    /// <summary>
    /// NPC 家族部队数量加成：
    /// 在 DefaultClanTierModel.GetPartyLimitForTier 返回的部队数量上限基础上，
    /// 为所有非玩家家族额外增加 Settings.NpcClanPartyLimitBonus 支（默认 +2）。
    /// 该方法是部队上限的"数量核心"——同时影响每日外派判定（HeroSpawnCampaignBehavior.ConsiderSpawningLordParties）
    /// 与 Clan.WarPartyLimit，因此只在此处加成即可全局生效。
    ///
    /// 不带 [HarmonyPatch] 属性，由 HarmonyPatchRegistry 显式注册。
    /// 内部有运行时 MCM 开关检查（null-safe），关闭或加成量为 0 时不干预。
    /// </summary>
    internal static class NpcClanPartyLimitPatch
    {
        internal static void Postfix(Clan clan, ref int __result)
        {
            // MCM 运行时开关 — 关闭时不干预
            if (Settings.Instance?.NpcClanPartyLimitBonusEnabled != true)
                return;

            int bonus = Settings.Instance.NpcClanPartyLimitBonus;
            if (bonus <= 0)
                return;

            // 仅影响 NPC 家族（玩家家族不自动派队，不受此加成）
            if (clan == null || clan == Clan.PlayerClan)
                return;

            __result += bonus;
        }
    }
}
