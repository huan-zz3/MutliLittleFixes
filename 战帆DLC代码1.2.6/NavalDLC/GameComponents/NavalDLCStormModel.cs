using System;
using NavalDLC.ComponentInterfaces;
using NavalDLC.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;

namespace NavalDLC.GameComponents
{
	// Token: 0x02000138 RID: 312
	public class NavalDLCStormModel : MapStormModel
	{
		// Token: 0x1700038E RID: 910
		// (get) Token: 0x06001513 RID: 5395 RVA: 0x00094B3C File Offset: 0x00092D3C
		public override float MinimumWeatherStrengthInsideStorm
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x1700038F RID: 911
		// (get) Token: 0x06001514 RID: 5396 RVA: 0x00094B43 File Offset: 0x00092D43
		public override int MaximumNumberOfStorms
		{
			get
			{
				return 6;
			}
		}

		// Token: 0x06001515 RID: 5397 RVA: 0x00094B48 File Offset: 0x00092D48
		public override float GetPositionDamageForStorm(Storm storm, Vec2 shipPosition, Ship ship)
		{
			float maximumWeatherStrengthAtEye = NavalDLCManager.Instance.GameModels.MapStormModel.GetMaximumWeatherStrengthAtEye(storm);
			float num = storm.CurrentPosition.Distance(shipPosition);
			float num2 = this.MinimumWeatherStrengthInsideStorm;
			if (num < storm.EyeRadius)
			{
				num2 = maximumWeatherStrengthAtEye;
			}
			else
			{
				num2 = ((num + storm.EyeRadius < storm.EffectRadius) ? MBMath.Map(num, 0f, storm.EffectRadius, maximumWeatherStrengthAtEye, this.MinimumWeatherStrengthInsideStorm) : 0f);
			}
			IMapScene mapSceneWrapper = Campaign.Current.MapSceneWrapper;
			CampaignVec2 campaignVec = new CampaignVec2(shipPosition, false);
			PathFaceRecord faceIndex = mapSceneWrapper.GetFaceIndex(ref campaignVec);
			Campaign.Current.MapSceneWrapper.GetFaceTerrainType(faceIndex);
			float num3 = 0f;
			int seaWorthiness = ship.SeaWorthiness;
			if ((float)seaWorthiness / 2f <= num2 * 10f)
			{
				num3 = Campaign.Current.Models.CampaignShipParametersModel.GetShipSizeWeatherFactor(ship.ShipHull) * (num2 - (float)seaWorthiness / 400f) * ((100f - (float)seaWorthiness) / 100f);
				float num4 = 10000f;
				float num5 = 1f;
				num3 = MBMath.ClampFloat(num3, num5, num4);
			}
			return num3;
		}

		// Token: 0x06001516 RID: 5398 RVA: 0x00094C64 File Offset: 0x00092E64
		public override float GetHourlyIntensityChangeForStorm(Storm storm)
		{
			CampaignVec2 campaignVec;
			campaignVec..ctor(storm.CurrentPosition, false);
			if (!Campaign.Current.MapSceneWrapper.GetFaceIndex(ref campaignVec).IsValid())
			{
				return -0.05f;
			}
			if (NavalDLCManager.Instance.NavalMapSceneWrapper.GetWindAtPosition(campaignVec.ToVec2()).Length >= 0.15f)
			{
				return 0.01f;
			}
			return -0.01f;
		}

		// Token: 0x06001517 RID: 5399 RVA: 0x00094CD0 File Offset: 0x00092ED0
		public override float GetMaximumWeatherStrengthAtEye(Storm storm)
		{
			switch (storm.StormType)
			{
			case Storm.StormTypes.Storm:
				return 3f;
			case Storm.StormTypes.ThunderStorm:
				return 6f;
			case Storm.StormTypes.Hurricane:
				return 10f;
			default:
				return 0f;
			}
		}

		// Token: 0x06001518 RID: 5400 RVA: 0x00094D0F File Offset: 0x00092F0F
		public override void GetStormLifeSpan(out CampaignTime minimumDuration, out CampaignTime maximumDuration)
		{
			minimumDuration = CampaignTime.Days(10f);
			maximumDuration = minimumDuration + CampaignTime.Days(11f);
		}

		// Token: 0x06001519 RID: 5401 RVA: 0x00094D3C File Offset: 0x00092F3C
		public override float GetHourlyStormSpawnChanceForPosition(Vec2 position)
		{
			return 0.0025f;
		}

		// Token: 0x0600151A RID: 5402 RVA: 0x00094D44 File Offset: 0x00092F44
		public override Storm.StormTypes GetSpawnedStormTypeForPosition(Vec2 position)
		{
			float num;
			float num2;
			Campaign.Current.Models.MapWeatherModel.GetSnowAndRainDataForPosition(position, CampaignTime.Now, ref num, ref num2);
			if (num2 <= 0.2f)
			{
				return Storm.StormTypes.Storm;
			}
			if (num2 <= 0.6f)
			{
				return Storm.StormTypes.ThunderStorm;
			}
			return Storm.StormTypes.Hurricane;
		}

		// Token: 0x0600151B RID: 5403 RVA: 0x00094D84 File Offset: 0x00092F84
		public override bool CanPartyGetDamagedByStorm(MobileParty mobileParty)
		{
			return mobileParty.CurrentSettlement == null && mobileParty.IsCurrentlyAtSea && mobileParty.MapEvent == null;
		}

		// Token: 0x0600151C RID: 5404 RVA: 0x00094DA4 File Offset: 0x00092FA4
		public override float GetEffectRadiusOfStorm(Storm storm)
		{
			float num = 0f;
			switch (storm.StormType)
			{
			case Storm.StormTypes.Storm:
				num = 20f;
				break;
			case Storm.StormTypes.ThunderStorm:
				num = 30f;
				break;
			case Storm.StormTypes.Hurricane:
				num = 40f;
				break;
			}
			float num2 = MBMath.Map(storm.Intensity, 0f, 1f, -0.2f, 0.2f);
			return num + num * num2;
		}

		// Token: 0x0600151D RID: 5405 RVA: 0x00094E10 File Offset: 0x00093010
		public override float GetEyeRadiusOfStorm(Storm storm)
		{
			switch (storm.StormType)
			{
			case Storm.StormTypes.Storm:
				return 0f;
			case Storm.StormTypes.ThunderStorm:
				return 0f;
			case Storm.StormTypes.Hurricane:
				return storm.EffectRadius * 0.2f;
			default:
				return 0f;
			}
		}

		// Token: 0x0600151E RID: 5406 RVA: 0x00094E58 File Offset: 0x00093058
		public override float GetSpeedOfStorm(Storm storm)
		{
			switch (storm.StormType)
			{
			case Storm.StormTypes.Storm:
				return 3f;
			case Storm.StormTypes.ThunderStorm:
				return 2f;
			case Storm.StormTypes.Hurricane:
				return 1f;
			default:
				return 0f;
			}
		}

		// Token: 0x0600151F RID: 5407 RVA: 0x00094E97 File Offset: 0x00093097
		public override CampaignTime GetDevelopingStateDurationOfStorm(Storm storm)
		{
			return CampaignTime.Hours(8f);
		}

		// Token: 0x06001520 RID: 5408 RVA: 0x00094EA3 File Offset: 0x000930A3
		public override CampaignTime GetFinalizingStateDurationOfStorm(Storm storm)
		{
			return CampaignTime.Hours(8f);
		}

		// Token: 0x06001521 RID: 5409 RVA: 0x00094EAF File Offset: 0x000930AF
		public override float GetStormSpawnDistanceSquaredThresholdWithOtherStorms()
		{
			return 40000f;
		}

		// Token: 0x06001522 RID: 5410 RVA: 0x00094EB8 File Offset: 0x000930B8
		public override float GetNormalizedWindStrengthOfStormForPosition(Vec2 position)
		{
			Storm storm = null;
			foreach (Storm storm2 in NavalDLCManager.Instance.StormManager.SpawnedStorms)
			{
				if (storm2.IsActive && storm2.CurrentPosition.Distance(position) < storm2.EffectRadius)
				{
					storm = storm2;
					break;
				}
			}
			if (storm != null)
			{
				float num = storm.EffectRadius - storm.CurrentPosition.Distance(position);
				float effectRadius = storm.EffectRadius;
				return num / effectRadius * storm.Intensity;
			}
			return 0f;
		}
	}
}
