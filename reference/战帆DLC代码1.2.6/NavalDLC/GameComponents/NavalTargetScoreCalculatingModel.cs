using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;

namespace NavalDLC.GameComponents
{
	// Token: 0x02000144 RID: 324
	public class NavalTargetScoreCalculatingModel : TargetScoreCalculatingModel
	{
		// Token: 0x17000399 RID: 921
		// (get) Token: 0x06001577 RID: 5495 RVA: 0x00096620 File Offset: 0x00094820
		public override float TravelingToAssignmentFactor
		{
			get
			{
				return base.BaseModel.TravelingToAssignmentFactor;
			}
		}

		// Token: 0x1700039A RID: 922
		// (get) Token: 0x06001578 RID: 5496 RVA: 0x0009662D File Offset: 0x0009482D
		public override float BesiegingFactor
		{
			get
			{
				return base.BaseModel.BesiegingFactor;
			}
		}

		// Token: 0x1700039B RID: 923
		// (get) Token: 0x06001579 RID: 5497 RVA: 0x0009663A File Offset: 0x0009483A
		public override float AssaultingTownFactor
		{
			get
			{
				return base.BaseModel.AssaultingTownFactor;
			}
		}

		// Token: 0x1700039C RID: 924
		// (get) Token: 0x0600157A RID: 5498 RVA: 0x00096647 File Offset: 0x00094847
		public override float RaidingFactor
		{
			get
			{
				return base.BaseModel.RaidingFactor;
			}
		}

		// Token: 0x1700039D RID: 925
		// (get) Token: 0x0600157B RID: 5499 RVA: 0x00096654 File Offset: 0x00094854
		public override float DefendingFactor
		{
			get
			{
				return base.BaseModel.DefendingFactor;
			}
		}

		// Token: 0x0600157C RID: 5500 RVA: 0x00096664 File Offset: 0x00094864
		public override float GetDefensivePatrollingFactor(bool isNavalPatrolling)
		{
			float num = base.BaseModel.GetDefensivePatrollingFactor(isNavalPatrolling);
			if (isNavalPatrolling)
			{
				num *= 0.66f;
			}
			return num;
		}

		// Token: 0x0600157D RID: 5501 RVA: 0x0009668A File Offset: 0x0009488A
		public override float GetOffensivePatrollingFactor(bool isNavalPatrolling)
		{
			return Campaign.Current.Models.TargetScoreCalculatingModel.GetDefensivePatrollingFactor(isNavalPatrolling) * 2f;
		}

		// Token: 0x0600157E RID: 5502 RVA: 0x000966A7 File Offset: 0x000948A7
		public override float GetTargetScoreForFaction(Settlement targetSettlement, Army.ArmyTypes missionType, MobileParty mobileParty, float ourStrength)
		{
			return base.BaseModel.GetTargetScoreForFaction(targetSettlement, missionType, mobileParty, ourStrength);
		}

		// Token: 0x0600157F RID: 5503 RVA: 0x000966BC File Offset: 0x000948BC
		public override float CalculateDefensivePatrollingScoreForSettlement(Settlement settlement, bool isTargetingPort, MobileParty mobileParty)
		{
			if (!isTargetingPort)
			{
				return base.BaseModel.CalculateDefensivePatrollingScoreForSettlement(settlement, false, mobileParty);
			}
			if (!mobileParty.HasNavalNavigationCapability || !settlement.HasPort || settlement.MapFaction != mobileParty.MapFaction)
			{
				return 0f;
			}
			float num = ((mobileParty.Food / -mobileParty.FoodChange > 5f) ? 1f : 0.2f);
			Clan ownerClan = settlement.OwnerClan;
			Hero leaderHero = mobileParty.LeaderHero;
			float num2 = ((ownerClan == ((leaderHero != null) ? leaderHero.Clan : null)) ? 1f : 0.5f);
			bool flag = mobileParty.DefaultBehavior == 13 && !mobileParty.TargetPosition.IsOnLand && mobileParty.TargetSettlement != null && !mobileParty.TargetSettlement.MapFaction.IsAtWarWith(mobileParty.MapFaction);
			bool flag2 = mobileParty.DefaultBehavior == 13 && mobileParty.TargetPosition.IsOnLand;
			float num3 = (flag ? 1.35f : 1f);
			float num4 = (3f + settlement.NearbyNavalThreatIntensity - settlement.NearbyNavalAllyIntensity * 1.5f) * (flag ? 1.5f : 1f);
			float num5 = LinQuick.SumQ<Ship>(mobileParty.Ships, (Ship x) => x.HitPoints / x.MaxHitPoints) / (float)mobileParty.Ships.Count;
			float num6 = (flag2 ? 0.5f : 1f);
			return num3 * num2 * num4 * num5 * num6 * num * Campaign.Current.Models.TargetScoreCalculatingModel.GetDefensivePatrollingFactor(true);
		}

		// Token: 0x06001580 RID: 5504 RVA: 0x00096847 File Offset: 0x00094A47
		public override float CurrentObjectiveValue(MobileParty mobileParty)
		{
			return base.BaseModel.CurrentObjectiveValue(mobileParty);
		}

		// Token: 0x06001581 RID: 5505 RVA: 0x00096858 File Offset: 0x00094A58
		public override float CalculateOffensivePatrollingScoreForSettlement(Settlement settlement, bool isTargetingPort, MobileParty mobileParty)
		{
			float num = ((mobileParty.Food / -mobileParty.FoodChange > 6f) ? 1f : 0.2f);
			bool flag = mobileParty.DefaultBehavior == 13 && !mobileParty.TargetPosition.IsOnLand && mobileParty.TargetSettlement != null && mobileParty.TargetSettlement == settlement && mobileParty.TargetSettlement.MapFaction.IsAtWarWith(mobileParty.MapFaction);
			bool flag2 = mobileParty.DefaultBehavior == 13 && mobileParty.TargetPosition.IsOnLand;
			float num2 = (flag ? 1.2f : 1f);
			float num3 = LinQuick.SumQ<Ship>(mobileParty.Ships, (Ship x) => x.HitPoints / x.MaxHitPoints) / (float)mobileParty.Ships.Count;
			float num4 = (flag2 ? 0.5f : 1f);
			float num5 = (settlement.IsVillage ? 1.2f : 1f);
			int num6 = 0;
			foreach (WarPartyComponent warPartyComponent in mobileParty.MapFaction.WarPartyComponents)
			{
				if (warPartyComponent.MobileParty != mobileParty && warPartyComponent.MobileParty.DefaultBehavior == 13 && warPartyComponent.MobileParty.TargetSettlement == settlement && warPartyComponent.MobileParty.IsTargetingPort)
				{
					num6++;
				}
			}
			float num7 = MathF.Pow(0.5f, (float)num6);
			return num2 * num3 * num * num4 * num7 * num5 * Campaign.Current.Models.TargetScoreCalculatingModel.GetOffensivePatrollingFactor(true);
		}
	}
}
