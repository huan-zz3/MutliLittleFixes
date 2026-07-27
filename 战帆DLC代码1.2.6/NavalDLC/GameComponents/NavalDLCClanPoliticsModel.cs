using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.Naval;

namespace NavalDLC.GameComponents
{
	// Token: 0x02000112 RID: 274
	public class NavalDLCClanPoliticsModel : ClanPoliticsModel
	{
		// Token: 0x060013B9 RID: 5049 RVA: 0x0008E4C8 File Offset: 0x0008C6C8
		public override ExplainedNumber CalculateInfluenceChange(Clan clan, bool includeDescriptions = false)
		{
			ExplainedNumber explainedNumber = base.BaseModel.CalculateInfluenceChange(clan, includeDescriptions);
			if (clan.Kingdom != null && !clan.IsUnderMercenaryService && clan.Kingdom.HasPolicy(NavalPolicies.NavalConjoiningStatute))
			{
				List<Ship> list = clan.AliveLords.Where<Hero>((Hero x) => x.PartyBelongedTo != null).SelectMany<Hero, Ship>((Hero x) => x.PartyBelongedTo.Ships).ToList<Ship>();
				if (list.Any<Ship>((Ship x) => x.ShipHull.Type == 2))
				{
					explainedNumber.Add(1f, NavalPolicies.NavalConjoiningStatute.Name, null);
				}
				else if (list.All<Ship>((Ship x) => x.ShipHull.Type == 0))
				{
					explainedNumber.Add(-1f, NavalPolicies.NavalConjoiningStatute.Name, null);
				}
			}
			return explainedNumber;
		}

		// Token: 0x060013BA RID: 5050 RVA: 0x0008E5E1 File Offset: 0x0008C7E1
		public override float CalculateSupportForPolicyInClan(Clan clan, PolicyObject policy)
		{
			return base.BaseModel.CalculateSupportForPolicyInClan(clan, policy);
		}

		// Token: 0x060013BB RID: 5051 RVA: 0x0008E5F0 File Offset: 0x0008C7F0
		public override float CalculateRelationshipChangeWithSponsor(Clan clan, Clan sponsorClan)
		{
			return base.BaseModel.CalculateRelationshipChangeWithSponsor(clan, sponsorClan);
		}

		// Token: 0x060013BC RID: 5052 RVA: 0x0008E5FF File Offset: 0x0008C7FF
		public override int GetInfluenceRequiredToOverrideKingdomDecision(DecisionOutcome popularOption, DecisionOutcome overridingOption, KingdomDecision decision)
		{
			return base.BaseModel.GetInfluenceRequiredToOverrideKingdomDecision(popularOption, overridingOption, decision);
		}

		// Token: 0x060013BD RID: 5053 RVA: 0x0008E60F File Offset: 0x0008C80F
		public override bool CanHeroBeGovernor(Hero hero)
		{
			return base.BaseModel.CanHeroBeGovernor(hero);
		}
	}
}
