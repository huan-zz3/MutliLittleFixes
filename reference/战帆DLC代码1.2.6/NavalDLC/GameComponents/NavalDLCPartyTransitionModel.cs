using System;
using NavalDLC.CharacterDevelopment;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;

namespace NavalDLC.GameComponents
{
	// Token: 0x02000128 RID: 296
	public class NavalDLCPartyTransitionModel : PartyTransitionModel
	{
		// Token: 0x060014A6 RID: 5286 RVA: 0x00092608 File Offset: 0x00090808
		public override CampaignTime GetTransitionTimeForEmbarking(MobileParty mobileParty)
		{
			if (!mobileParty.Anchor.IsValid)
			{
				return CampaignTime.Hours(48f);
			}
			float num;
			if (mobileParty.CurrentSettlement == null)
			{
				MapDistanceModel mapDistanceModel = Campaign.Current.Models.MapDistanceModel;
				CampaignVec2 interactionPosition = mobileParty.Anchor.GetInteractionPosition(mobileParty);
				float num2;
				num = mapDistanceModel.GetDistance(mobileParty, ref interactionPosition, 1, ref num2);
			}
			else
			{
				MapDistanceModel mapDistanceModel2 = Campaign.Current.Models.MapDistanceModel;
				Settlement currentSettlement = mobileParty.CurrentSettlement;
				CampaignVec2 position = mobileParty.Anchor.Position;
				num = mapDistanceModel2.GetDistance(currentSettlement, ref position, true, 2);
			}
			float num3 = num;
			if (num3 < 10f)
			{
				return CampaignTime.Zero;
			}
			return CampaignTime.Hours(this.GetAnchorReachDurationInHours(num3, 0f));
		}

		// Token: 0x060014A7 RID: 5287 RVA: 0x000926A8 File Offset: 0x000908A8
		public override CampaignTime GetTransitionTimeDisembarking(MobileParty mobileParty)
		{
			CampaignTime campaignTime = CampaignTime.Zero;
			if (!mobileParty.IsInRaftState)
			{
				campaignTime = CampaignTime.Hours(2f);
				if (mobileParty.HasPerk(NavalPerks.Shipmaster.Unflinching, false))
				{
					float num = NavalPerks.Shipmaster.Unflinching.PrimaryBonus * 100f;
					float num2 = -(num * 100f) / (100f + num);
					campaignTime = CampaignTime.Hours((float)campaignTime.ToHours * num2);
				}
			}
			return campaignTime;
		}

		// Token: 0x060014A8 RID: 5288 RVA: 0x00092710 File Offset: 0x00090910
		public override CampaignTime GetFleetTravelTimeToSettlement(MobileParty mobileParty, Settlement targetSettlement)
		{
			AnchorPoint anchor = mobileParty.Anchor;
			CampaignVec2 campaignVec = anchor.Position;
			if (campaignVec.IsValid() || anchor.IsMovingToPoint)
			{
				float num = (anchor.IsMovingToPoint ? ((float)(anchor.ArrivalTime - CampaignTime.Now).ToHours) : 0f);
				MapDistanceModel mapDistanceModel = Campaign.Current.Models.MapDistanceModel;
				campaignVec = anchor.Position;
				campaignVec = (campaignVec.IsValid() ? anchor.Position : anchor.TargetPosition);
				float distance = mapDistanceModel.GetDistance(targetSettlement, ref campaignVec, true, 2);
				return CampaignTime.Hours(this.GetAnchorReachDurationInHours(distance, num));
			}
			CampaignTime campaignTime = CampaignTime.Hours(48f);
			if (mobileParty.HasPerk(NavalPerks.Shipmaster.ShoreMaster, false))
			{
				campaignTime = CampaignTime.Hours((float)campaignTime.ToHours * NavalPerks.Shipmaster.ShoreMaster.PrimaryBonus * -1f);
			}
			return campaignTime;
		}

		// Token: 0x060014A9 RID: 5289 RVA: 0x000927E8 File Offset: 0x000909E8
		private float GetAnchorReachDurationInHours(float distance, float currentTravelTime = 0f)
		{
			distance = MathF.Pow(distance, 0.95f);
			return MBMath.ClampFloat(distance / 35f + currentTravelTime, 3f, 48f);
		}

		// Token: 0x04000AF7 RID: 2807
		private const float MinHoursToMoveAnchor = 3f;

		// Token: 0x04000AF8 RID: 2808
		private const float MaxHoursToMoveAnchor = 48f;

		// Token: 0x04000AF9 RID: 2809
		private const float AnchorMoveSpeedPerHour = 35f;

		// Token: 0x04000AFA RID: 2810
		private const float DisembarkHours = 2f;

		// Token: 0x04000AFB RID: 2811
		private const float InstantEmbarkDistanceThresholdForAI = 10f;
	}
}
