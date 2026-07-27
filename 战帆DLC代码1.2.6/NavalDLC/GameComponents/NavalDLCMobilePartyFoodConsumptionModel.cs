using System;
using Helpers;
using NavalDLC.CharacterDevelopment;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Localization;

namespace NavalDLC.GameComponents
{
	// Token: 0x02000121 RID: 289
	public class NavalDLCMobilePartyFoodConsumptionModel : MobilePartyFoodConsumptionModel
	{
		// Token: 0x17000377 RID: 887
		// (get) Token: 0x0600146E RID: 5230 RVA: 0x0009192C File Offset: 0x0008FB2C
		public override int NumberOfMenOnMapToEatOneFood
		{
			get
			{
				return base.BaseModel.NumberOfMenOnMapToEatOneFood;
			}
		}

		// Token: 0x0600146F RID: 5231 RVA: 0x00091939 File Offset: 0x0008FB39
		public override ExplainedNumber CalculateDailyBaseFoodConsumptionf(MobileParty party, bool includeDescription = false)
		{
			return base.BaseModel.CalculateDailyBaseFoodConsumptionf(party, includeDescription);
		}

		// Token: 0x06001470 RID: 5232 RVA: 0x00091948 File Offset: 0x0008FB48
		public override ExplainedNumber CalculateDailyFoodConsumptionf(MobileParty party, ExplainedNumber baseConsumption)
		{
			ExplainedNumber explainedNumber = base.BaseModel.CalculateDailyFoodConsumptionf(party, baseConsumption);
			if (party.IsCurrentlyAtSea)
			{
				explainedNumber.AddFactor(-0.2f, this._partyFoodConsumptionReductionAtSea);
				PerkHelper.AddPerkBonusForParty(NavalPerks.Boatswain.SmoothOperator, party, false, ref explainedNumber, false);
			}
			return explainedNumber;
		}

		// Token: 0x06001471 RID: 5233 RVA: 0x0009198D File Offset: 0x0008FB8D
		public override bool DoesPartyConsumeFood(MobileParty mobileParty)
		{
			return base.BaseModel.DoesPartyConsumeFood(mobileParty);
		}

		// Token: 0x04000AE4 RID: 2788
		private const float PartyFoodConsumptionReductionAtSea = 0.2f;

		// Token: 0x04000AE5 RID: 2789
		private readonly TextObject _partyFoodConsumptionReductionAtSea = new TextObject("{=Z1af4yEX}Food Consumption Reduction At Sea", null);
	}
}
