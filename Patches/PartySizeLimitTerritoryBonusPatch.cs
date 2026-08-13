using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using MutliLittleFixes.Behaviors;

namespace MutliLittleFixes.Patches
{
    internal static class PartySizeLimitTerritoryBonusPatch
    {
        private static readonly TextObject _bonusText = new TextObject("{=mlf_explained_territory}Territory Loss Compensation", null);

        internal static void Postfix(PartyBase party, bool includeDescriptions, ref ExplainedNumber __result)
        {
            // 只应用于移动队伍 — 不包括驻军或定居点
            if (!party.IsMobile)
                return;

            MobileParty mobileParty = party.MobileParty;
            if (mobileParty == null)
                return;

            // 排除非领主队伍：驻军、巡逻队、商队、村民队伍
            if (mobileParty.IsGarrison || mobileParty.IsPatrolParty ||
                mobileParty.IsCaravan || mobileParty.IsVillager)
                return;

            // 获取队伍领袖所属的王国
            Hero leader = party.LeaderHero;
            if (leader == null)
                return;

            Kingdom kingdom = leader.MapFaction as Kingdom;
            if (kingdom == null)
                return;

            // 可选：跳过雇佣兵（仅有活跃雇佣合同的家族）
            if (Settings.Instance?.TerritoryBonusVassalsOnly == true)
            {
                Clan clan = leader.Clan;
                if (clan != null && clan.IsUnderMercenaryService)
                    return;
            }

            // 从行为中获取领土加成
            KingdomTerritoryBonusBehavior behavior =
                Campaign.Current?.GetCampaignBehavior<KingdomTerritoryBonusBehavior>();
            if (behavior == null)
                return;

            float bonus = behavior.GetTerritoryBonus(kingdom);

            // 如果加成为正数则应用
            if (bonus > 0f)
            {
                __result.Add(bonus, _bonusText);
            }
        }
    }
}
