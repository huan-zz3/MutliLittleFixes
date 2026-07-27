using System;
using Helpers;
using NavalDLC.CharacterDevelopment;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;

namespace NavalDLC.GameComponents
{
	// Token: 0x0200010D RID: 269
	public class NavalDLCCampaignShipDamageModel : CampaignShipDamageModel
	{
		// Token: 0x06001385 RID: 4997 RVA: 0x0008D560 File Offset: 0x0008B760
		public override int GetHourlyShipDamage(MobileParty owner, Ship ship)
		{
			int num = 0;
			if (owner.CurrentSettlement == null && owner.MapEvent == null && Campaign.Current.MapSceneWrapper.GetFaceTerrainType(owner.CurrentNavigationFace) == 19)
			{
				num = (int)this.CalculateOpenSeaAttritionDamageForShip(ship);
			}
			return num;
		}

		// Token: 0x06001386 RID: 4998 RVA: 0x0008D5A4 File Offset: 0x0008B7A4
		public override float GetEstimatedSafeSailDuration(MobileParty mobileParty)
		{
			float num = 0f;
			foreach (Ship ship in mobileParty.Ships)
			{
				float num2 = this.CalculateOpenSeaAttritionDamageForShip(ship) * 0.27f;
				float num3 = ship.HitPoints / num2;
				num += num3;
			}
			return num / (float)mobileParty.Ships.Count;
		}

		// Token: 0x06001387 RID: 4999 RVA: 0x0008D624 File Offset: 0x0008B824
		public override float GetShipDamage(Ship ship, Ship rammingShip, float rawDamage)
		{
			ExplainedNumber explainedNumber;
			explainedNumber..ctor(rawDamage, false, null);
			PartyBase owner = ship.Owner;
			if (owner != null && owner.IsMobile)
			{
				SkillHelper.AddSkillBonusForParty(NavalSkillEffects.ShipDamageReduction, ship.Owner.MobileParty, ref explainedNumber);
			}
			if (rammingShip != null && rammingShip.Figurehead != null && rammingShip.Figurehead == DefaultFigureheads.Ram)
			{
				explainedNumber.AddFactor(rammingShip.Figurehead.EffectAmount, null);
			}
			return Math.Max(0f, explainedNumber.ResultNumber);
		}

		// Token: 0x06001388 RID: 5000 RVA: 0x0008D6A4 File Offset: 0x0008B8A4
		private float CalculateOpenSeaAttritionDamageForShip(Ship ship)
		{
			int seaWorthiness = ship.SeaWorthiness;
			return MBMath.ClampFloat(Campaign.Current.Models.CampaignShipParametersModel.GetShipSizeWeatherFactor(ship.ShipHull) * (1f - (float)seaWorthiness / 400f) * ((100f - (float)seaWorthiness) / 100f), 1f, 10000f);
		}

		// Token: 0x04000AC1 RID: 2753
		private const float MaximumDamageToShip = 10000f;

		// Token: 0x04000AC2 RID: 2754
		private const float MinimumDamageToShip = 1f;

		// Token: 0x04000AC3 RID: 2755
		private const float AverageBeingOnOpenSeaRatio = 0.27f;
	}
}
