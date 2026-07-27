using System;
using Helpers;
using NavalDLC.CharacterDevelopment;
using NavalDLC.Storyline;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace NavalDLC.GameComponents
{
	// Token: 0x02000108 RID: 264
	public class NavalDLCArmyManagementCalculationModel : ArmyManagementCalculationModel
	{
		// Token: 0x17000340 RID: 832
		// (get) Token: 0x06001335 RID: 4917 RVA: 0x0008C222 File Offset: 0x0008A422
		public override float AIMobilePartySizeRatioToCallToArmy
		{
			get
			{
				return base.BaseModel.AIMobilePartySizeRatioToCallToArmy;
			}
		}

		// Token: 0x17000341 RID: 833
		// (get) Token: 0x06001336 RID: 4918 RVA: 0x0008C22F File Offset: 0x0008A42F
		public override float PlayerMobilePartySizeRatioToCallToArmy
		{
			get
			{
				return base.BaseModel.PlayerMobilePartySizeRatioToCallToArmy;
			}
		}

		// Token: 0x17000342 RID: 834
		// (get) Token: 0x06001337 RID: 4919 RVA: 0x0008C23C File Offset: 0x0008A43C
		public override float MinimumNeededFoodInDaysToCallToArmy
		{
			get
			{
				return base.BaseModel.MinimumNeededFoodInDaysToCallToArmy;
			}
		}

		// Token: 0x17000343 RID: 835
		// (get) Token: 0x06001338 RID: 4920 RVA: 0x0008C249 File Offset: 0x0008A449
		public override float MaximumDistanceToCallToArmy
		{
			get
			{
				return base.BaseModel.MaximumDistanceToCallToArmy;
			}
		}

		// Token: 0x17000344 RID: 836
		// (get) Token: 0x06001339 RID: 4921 RVA: 0x0008C256 File Offset: 0x0008A456
		public override int InfluenceValuePerGold
		{
			get
			{
				return base.BaseModel.InfluenceValuePerGold;
			}
		}

		// Token: 0x17000345 RID: 837
		// (get) Token: 0x0600133A RID: 4922 RVA: 0x0008C263 File Offset: 0x0008A463
		public override int AverageCallToArmyCost
		{
			get
			{
				return base.BaseModel.AverageCallToArmyCost;
			}
		}

		// Token: 0x17000346 RID: 838
		// (get) Token: 0x0600133B RID: 4923 RVA: 0x0008C270 File Offset: 0x0008A470
		public override int CohesionThresholdForDispersion
		{
			get
			{
				return base.BaseModel.CohesionThresholdForDispersion;
			}
		}

		// Token: 0x17000347 RID: 839
		// (get) Token: 0x0600133C RID: 4924 RVA: 0x0008C27D File Offset: 0x0008A47D
		public override float MaximumWaitTime
		{
			get
			{
				return base.BaseModel.MaximumWaitTime;
			}
		}

		// Token: 0x0600133D RID: 4925 RVA: 0x0008C28C File Offset: 0x0008A48C
		public override ExplainedNumber CalculateDailyCohesionChange(Army army, bool includeDescriptions = false)
		{
			ExplainedNumber explainedNumber = base.BaseModel.CalculateDailyCohesionChange(army, includeDescriptions);
			if (army.LeaderParty != null && !army.LeaderParty.IsCurrentlyAtSea && PartyBaseHelper.HasFeat(army.LeaderParty.Party, NavalCulturalFeats.NordArmyCohesionFeat))
			{
				explainedNumber.AddFactor(NavalCulturalFeats.NordArmyCohesionFeat.EffectBonus, GameTexts.FindText("str_culture", null));
			}
			return explainedNumber;
		}

		// Token: 0x0600133E RID: 4926 RVA: 0x0008C2F0 File Offset: 0x0008A4F0
		public override int CalculateNewCohesion(Army army, PartyBase newParty, int calculatedCohesion, int sign)
		{
			return base.BaseModel.CalculateNewCohesion(army, newParty, calculatedCohesion, sign);
		}

		// Token: 0x0600133F RID: 4927 RVA: 0x0008C302 File Offset: 0x0008A502
		public override int CalculatePartyInfluenceCost(MobileParty armyLeaderParty, MobileParty party)
		{
			return base.BaseModel.CalculatePartyInfluenceCost(armyLeaderParty, party);
		}

		// Token: 0x06001340 RID: 4928 RVA: 0x0008C311 File Offset: 0x0008A511
		public override bool CanLordCreateArmy(MobileParty leaderParty, out MBList<MobileParty> possibleArmyMembers)
		{
			return base.BaseModel.CanLordCreateArmy(leaderParty, ref possibleArmyMembers);
		}

		// Token: 0x06001341 RID: 4929 RVA: 0x0008C320 File Offset: 0x0008A520
		public override int CalculateTotalInfluenceCost(Army army, float percentage)
		{
			return base.BaseModel.CalculateTotalInfluenceCost(army, percentage);
		}

		// Token: 0x06001342 RID: 4930 RVA: 0x0008C330 File Offset: 0x0008A530
		public override bool CanPlayerCreateArmy(out TextObject disabledReason)
		{
			if (!NavalStorylineData.IsNavalStoryLineActive())
			{
				MenuContext currentMenuContext = Campaign.Current.CurrentMenuContext;
				string text;
				if (currentMenuContext == null)
				{
					text = null;
				}
				else
				{
					GameMenu gameMenu = currentMenuContext.GameMenu;
					text = ((gameMenu != null) ? gameMenu.StringId : null);
				}
				if (!(text == "naval_storyline_outside_town"))
				{
					return base.BaseModel.CanPlayerCreateArmy(ref disabledReason);
				}
			}
			disabledReason = new TextObject("{=lwbwTg5b}You can't perform this action during this time.", null);
			return false;
		}

		// Token: 0x06001343 RID: 4931 RVA: 0x0008C38D File Offset: 0x0008A58D
		public override bool CheckPartyEligibility(MobileParty party, out TextObject explanation)
		{
			return base.BaseModel.CheckPartyEligibility(party, ref explanation);
		}

		// Token: 0x06001344 RID: 4932 RVA: 0x0008C39C File Offset: 0x0008A59C
		public override float DailyBeingAtArmyInfluenceAward(MobileParty armyMemberParty)
		{
			return base.BaseModel.DailyBeingAtArmyInfluenceAward(armyMemberParty);
		}

		// Token: 0x06001345 RID: 4933 RVA: 0x0008C3AA File Offset: 0x0008A5AA
		public override int GetCohesionBoostInfluenceCost(Army army, int percentageToBoost = 100)
		{
			return base.BaseModel.GetCohesionBoostInfluenceCost(army, percentageToBoost);
		}

		// Token: 0x06001346 RID: 4934 RVA: 0x0008C3B9 File Offset: 0x0008A5B9
		public override int GetPartyRelation(Hero hero)
		{
			return base.BaseModel.GetPartyRelation(hero);
		}

		// Token: 0x06001347 RID: 4935 RVA: 0x0008C3C7 File Offset: 0x0008A5C7
		public override float GetPartySizeScore(MobileParty party)
		{
			return base.BaseModel.GetPartySizeScore(party);
		}
	}
}
