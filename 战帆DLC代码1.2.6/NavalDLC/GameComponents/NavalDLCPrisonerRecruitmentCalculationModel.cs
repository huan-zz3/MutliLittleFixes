using System;
using Helpers;
using NavalDLC.CharacterDevelopment;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;

namespace NavalDLC.GameComponents
{
	// Token: 0x0200012B RID: 299
	internal class NavalDLCPrisonerRecruitmentCalculationModel : PrisonerRecruitmentCalculationModel
	{
		// Token: 0x060014B9 RID: 5305 RVA: 0x00092ABE File Offset: 0x00090CBE
		public override int GetConformityNeededToRecruitPrisoner(CharacterObject character)
		{
			return base.BaseModel.GetConformityNeededToRecruitPrisoner(character);
		}

		// Token: 0x060014BA RID: 5306 RVA: 0x00092ACC File Offset: 0x00090CCC
		public override ExplainedNumber GetConformityChangePerHour(PartyBase party, CharacterObject troopToBoost)
		{
			ExplainedNumber conformityChangePerHour = base.BaseModel.GetConformityChangePerHour(party, troopToBoost);
			if (party.IsMobile && party.MobileParty.IsCurrentlyAtSea && troopToBoost.IsPirate())
			{
				PerkHelper.AddPerkBonusForParty(NavalPerks.Mariner.RollingThunder, party.MobileParty, false, ref conformityChangePerHour, false);
			}
			return conformityChangePerHour;
		}

		// Token: 0x060014BB RID: 5307 RVA: 0x00092B19 File Offset: 0x00090D19
		public override int GetPrisonerRecruitmentMoraleEffect(PartyBase party, CharacterObject character, int num)
		{
			return base.BaseModel.GetPrisonerRecruitmentMoraleEffect(party, character, num);
		}

		// Token: 0x060014BC RID: 5308 RVA: 0x00092B29 File Offset: 0x00090D29
		public override bool IsPrisonerRecruitable(PartyBase party, CharacterObject character, out int conformityNeeded)
		{
			return base.BaseModel.IsPrisonerRecruitable(party, character, ref conformityNeeded);
		}

		// Token: 0x060014BD RID: 5309 RVA: 0x00092B39 File Offset: 0x00090D39
		public override bool ShouldPartyRecruitPrisoners(PartyBase party)
		{
			return base.BaseModel.ShouldPartyRecruitPrisoners(party);
		}

		// Token: 0x060014BE RID: 5310 RVA: 0x00092B47 File Offset: 0x00090D47
		public override int CalculateRecruitableNumber(PartyBase party, CharacterObject character)
		{
			return base.BaseModel.CalculateRecruitableNumber(party, character);
		}
	}
}
