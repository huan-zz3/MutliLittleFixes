using System;
using System.Collections.Generic;
using NavalDLC.Storyline;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;

namespace NavalDLC.GameComponents
{
	// Token: 0x02000125 RID: 293
	public class NavalDLCPartySizeLimitModel : PartySizeLimitModel
	{
		// Token: 0x17000379 RID: 889
		// (get) Token: 0x06001489 RID: 5257 RVA: 0x00091BCF File Offset: 0x0008FDCF
		public override int MinimumNumberOfVillagersAtVillagerParty
		{
			get
			{
				return base.BaseModel.MinimumNumberOfVillagersAtVillagerParty;
			}
		}

		// Token: 0x0600148A RID: 5258 RVA: 0x00091BDC File Offset: 0x0008FDDC
		public override ExplainedNumber CalculateGarrisonPartySizeLimit(Settlement settlement, bool includeDescriptions = false)
		{
			return base.BaseModel.CalculateGarrisonPartySizeLimit(settlement, includeDescriptions);
		}

		// Token: 0x0600148B RID: 5259 RVA: 0x00091BEB File Offset: 0x0008FDEB
		public override TroopRoster FindAppropriateInitialRosterForMobileParty(MobileParty party, PartyTemplateObject partyTemplate)
		{
			return base.BaseModel.FindAppropriateInitialRosterForMobileParty(party, partyTemplate);
		}

		// Token: 0x0600148C RID: 5260 RVA: 0x00091BFA File Offset: 0x0008FDFA
		public override List<Ship> FindAppropriateInitialShipsForMobileParty(MobileParty party, PartyTemplateObject partyTemplate)
		{
			return base.BaseModel.FindAppropriateInitialShipsForMobileParty(party, partyTemplate);
		}

		// Token: 0x0600148D RID: 5261 RVA: 0x00091C09 File Offset: 0x0008FE09
		public override int GetAssumedPartySizeForLordParty(Hero leaderHero, IFaction partyMapFaction, Clan actualClan)
		{
			return base.BaseModel.GetAssumedPartySizeForLordParty(leaderHero, partyMapFaction, actualClan);
		}

		// Token: 0x0600148E RID: 5262 RVA: 0x00091C19 File Offset: 0x0008FE19
		public override int GetClanTierPartySizeEffectForHero(Hero hero)
		{
			return base.BaseModel.GetClanTierPartySizeEffectForHero(hero);
		}

		// Token: 0x0600148F RID: 5263 RVA: 0x00091C27 File Offset: 0x0008FE27
		public override int GetIdealVillagerPartySize(Village village)
		{
			return base.BaseModel.GetIdealVillagerPartySize(village);
		}

		// Token: 0x06001490 RID: 5264 RVA: 0x00091C35 File Offset: 0x0008FE35
		public override int GetNextClanTierPartySizeEffectChangeForHero(Hero hero)
		{
			return base.BaseModel.GetNextClanTierPartySizeEffectChangeForHero(hero);
		}

		// Token: 0x06001491 RID: 5265 RVA: 0x00091C44 File Offset: 0x0008FE44
		public override ExplainedNumber GetPartyMemberSizeLimit(PartyBase party, bool includeDescriptions = false)
		{
			NavalStorylinePartyData navalStorylinePartyData;
			if (party.IsNavalStorylineQuestParty(out navalStorylinePartyData) && navalStorylinePartyData.IsQuestParty)
			{
				return new ExplainedNumber((float)navalStorylinePartyData.PartySize, false, null);
			}
			if (party.IsMobile && party.MobileParty.ActualClan != null && party.MobileParty.ActualClan.IsBanditFaction && !party.MobileParty.IsCurrentlyUsedByAQuest && party.MobileParty.HasNavalNavigationCapability)
			{
				return new ExplainedNumber((float)party.MobileParty.ActualClan.DefaultPartyTemplate.GetUpperTroopLimit(), false, null);
			}
			if (party.IsMobile && party.MobileParty.IsPatrolParty && party.MobileParty.PatrolPartyComponent.IsNaval)
			{
				return this.CalculatePatrolPartySizeLimit(party.MobileParty, includeDescriptions);
			}
			return base.BaseModel.GetPartyMemberSizeLimit(party, includeDescriptions);
		}

		// Token: 0x06001492 RID: 5266 RVA: 0x00091D13 File Offset: 0x0008FF13
		private ExplainedNumber CalculatePatrolPartySizeLimit(MobileParty mobileParty, bool includeDescriptions)
		{
			return new ExplainedNumber((float)mobileParty.HomeSettlement.Culture.SettlementPatrolPartyTemplateNaval.GetUpperTroopLimit(), includeDescriptions, null);
		}

		// Token: 0x06001493 RID: 5267 RVA: 0x00091D32 File Offset: 0x0008FF32
		public override ExplainedNumber GetPartyPrisonerSizeLimit(PartyBase party, bool includeDescriptions = false)
		{
			return base.BaseModel.GetPartyPrisonerSizeLimit(party, includeDescriptions);
		}
	}
}
