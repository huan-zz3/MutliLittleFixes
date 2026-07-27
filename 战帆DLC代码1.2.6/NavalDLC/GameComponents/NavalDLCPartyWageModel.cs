using System;
using Helpers;
using NavalDLC.CharacterDevelopment;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Localization;

namespace NavalDLC.GameComponents
{
	// Token: 0x0200012A RID: 298
	public class NavalDLCPartyWageModel : PartyWageModel
	{
		// Token: 0x1700037C RID: 892
		// (get) Token: 0x060014B4 RID: 5300 RVA: 0x000928D9 File Offset: 0x00090AD9
		public override int MaxWagePaymentLimit
		{
			get
			{
				return base.BaseModel.MaxWagePaymentLimit;
			}
		}

		// Token: 0x060014B5 RID: 5301 RVA: 0x000928E6 File Offset: 0x00090AE6
		public override int GetCharacterWage(CharacterObject character)
		{
			return base.BaseModel.GetCharacterWage(character);
		}

		// Token: 0x060014B6 RID: 5302 RVA: 0x000928F4 File Offset: 0x00090AF4
		public override ExplainedNumber GetTotalWage(MobileParty mobileParty, TroopRoster troopRoster, bool includeDescriptions = false)
		{
			ExplainedNumber totalWage = base.BaseModel.GetTotalWage(mobileParty, troopRoster, includeDescriptions);
			bool flag = !mobileParty.HasPerk(DefaultPerks.Steward.AidCorps, false);
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < troopRoster.Count; i++)
			{
				TroopRosterElement elementCopyAtIndex = troopRoster.GetElementCopyAtIndex(i);
				CharacterObject character = elementCopyAtIndex.Character;
				int num3 = (flag ? elementCopyAtIndex.Number : (elementCopyAtIndex.Number - elementCopyAtIndex.WoundedNumber));
				if (!character.IsHero)
				{
					int num4 = character.TroopWage * num3;
					if (!character.IsMariner)
					{
						num += num4;
					}
					if (character.IsMounted)
					{
						num2 += num4;
					}
				}
			}
			if (mobileParty.IsCurrentlyAtSea)
			{
				totalWage.AddFactor(-0.2f, this._partyWageReductionAtSea);
				if (mobileParty.IsCaravan)
				{
					totalWage.AddFactor(-0.8f, this._convoyPartyWageCutText);
				}
				if (mobileParty.HasPerk(NavalPerks.Boatswain.Optimization, false))
				{
					float num5 = (float)num / totalWage.BaseNumber;
					if (num5 > 0f)
					{
						float num6 = NavalPerks.Boatswain.Optimization.PrimaryBonus * num5;
						totalWage.AddFactor(num6, NavalPerks.Boatswain.Optimization.Name);
					}
				}
				if (mobileParty.HasPerk(NavalPerks.Boatswain.NavalHorde, false))
				{
					float num7 = (float)num2 / totalWage.BaseNumber;
					if (num7 > 0f)
					{
						float num8 = NavalPerks.Boatswain.NavalHorde.PrimaryBonus * num7;
						totalWage.AddFactor(num8, NavalPerks.Boatswain.NavalHorde.Name);
					}
				}
			}
			return totalWage;
		}

		// Token: 0x060014B7 RID: 5303 RVA: 0x00092A60 File Offset: 0x00090C60
		public override ExplainedNumber GetTroopRecruitmentCost(CharacterObject troop, Hero buyerHero, bool withoutItemCost = false)
		{
			ExplainedNumber troopRecruitmentCost = base.BaseModel.GetTroopRecruitmentCost(troop, buyerHero, withoutItemCost);
			if (buyerHero != null)
			{
				PerkHelper.AddPerkBonusForCharacter(NavalPerks.Boatswain.PopularCaptain, buyerHero.CharacterObject, true, ref troopRecruitmentCost, false);
			}
			return troopRecruitmentCost;
		}

		// Token: 0x04000AFC RID: 2812
		private const float PartyWageReductionAtSea = 0.2f;

		// Token: 0x04000AFD RID: 2813
		private const float ConvoyPartyWageCut = -0.8f;

		// Token: 0x04000AFE RID: 2814
		private readonly TextObject _convoyPartyWageCutText = new TextObject("{=lDxu6pez}Convoy Wage Multiplier", null);

		// Token: 0x04000AFF RID: 2815
		private readonly TextObject _partyWageReductionAtSea = new TextObject("{=sWhNhHkV}Wage Reduction At Sea", null);
	}
}
