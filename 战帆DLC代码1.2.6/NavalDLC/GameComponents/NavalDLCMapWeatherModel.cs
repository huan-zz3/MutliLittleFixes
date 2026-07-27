using System;
using System.Collections.Generic;
using NavalDLC.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace NavalDLC.GameComponents
{
	// Token: 0x0200011E RID: 286
	public class NavalDLCMapWeatherModel : MapWeatherModel
	{
		// Token: 0x17000369 RID: 873
		// (get) Token: 0x06001441 RID: 5185 RVA: 0x00090D1B File Offset: 0x0008EF1B
		public override CampaignTime WeatherUpdateFrequency
		{
			get
			{
				return base.BaseModel.WeatherUpdateFrequency;
			}
		}

		// Token: 0x1700036A RID: 874
		// (get) Token: 0x06001442 RID: 5186 RVA: 0x00090D28 File Offset: 0x0008EF28
		public override CampaignTime WeatherUpdatePeriod
		{
			get
			{
				return base.BaseModel.WeatherUpdatePeriod;
			}
		}

		// Token: 0x06001443 RID: 5187 RVA: 0x00090D38 File Offset: 0x0008EF38
		public override AtmosphereInfo GetAtmosphereModel(CampaignVec2 position)
		{
			AtmosphereInfo atmosphereModel = base.BaseModel.GetAtmosphereModel(position);
			if (!position.IsOnLand)
			{
				atmosphereModel.NauticalInfo.UseSceneWindDirection = 0;
				atmosphereModel.NauticalInfo.CanUseLowAltitudeAtmosphere = 1;
				atmosphereModel.NauticalInfo.IsInsideStorm = (this.IsPositionInsideStormForMission(position) ? 1 : 0);
				float num;
				if (atmosphereModel.NauticalInfo.IsInsideStorm == 1)
				{
					num = (atmosphereModel.NauticalInfo.WindVector.Length - 0.1f) * 0.3333333f / 0.9f + 0.6666667f;
				}
				else
				{
					num = atmosphereModel.NauticalInfo.WindVector.Length * 0.13333336f / 0.39333335f + 0.4f;
				}
				atmosphereModel.NauticalInfo.WindVector = atmosphereModel.NauticalInfo.WindVector.Normalized() * num;
			}
			else
			{
				atmosphereModel.NauticalInfo.WindVector = Campaign.Current.Models.MapWeatherModel.GetWindForPosition(position).Normalized() * 0.26f;
			}
			return atmosphereModel;
		}

		// Token: 0x06001444 RID: 5188 RVA: 0x00090E4D File Offset: 0x0008F04D
		public override AtmosphereState GetInterpolatedAtmosphereState(CampaignTime timeOfYear, Vec3 pos)
		{
			return base.BaseModel.GetInterpolatedAtmosphereState(timeOfYear, pos);
		}

		// Token: 0x06001445 RID: 5189 RVA: 0x00090E5C File Offset: 0x0008F05C
		public override void GetSeasonTimeFactorOfCampaignTime(CampaignTime ct, out float timeFactorForSnow, out float timeFactorForRain, bool snapCampaignTimeToWeatherPeriod = true)
		{
			base.BaseModel.GetSeasonTimeFactorOfCampaignTime(ct, ref timeFactorForSnow, ref timeFactorForRain, snapCampaignTimeToWeatherPeriod);
		}

		// Token: 0x06001446 RID: 5190 RVA: 0x00090E70 File Offset: 0x0008F070
		public override void GetSnowAndRainDataForPosition(Vec2 position, CampaignTime ct, out float snowValue, out float rainValue)
		{
			base.BaseModel.GetSnowAndRainDataForPosition(position, ct, ref snowValue, ref rainValue);
			foreach (Storm storm in NavalDLCManager.Instance.StormManager.SpawnedStorms)
			{
				if (storm.CurrentPosition.DistanceSquared(position) < storm.EffectRadius * storm.EffectRadius)
				{
					rainValue = 1f;
					break;
				}
			}
		}

		// Token: 0x06001447 RID: 5191 RVA: 0x00090F00 File Offset: 0x0008F100
		public override MapWeatherModel.WeatherEventEffectOnTerrain GetWeatherEffectOnTerrainForPosition(Vec2 pos)
		{
			MapWeatherModel.WeatherEventEffectOnTerrain weatherEventEffectOnTerrain = base.BaseModel.GetWeatherEffectOnTerrainForPosition(pos);
			using (List<Storm>.Enumerator enumerator = NavalDLCManager.Instance.StormManager.SpawnedStorms.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.HasWetWeatherEffectAtPosition(pos))
					{
						weatherEventEffectOnTerrain = 1;
						break;
					}
				}
			}
			return weatherEventEffectOnTerrain;
		}

		// Token: 0x06001448 RID: 5192 RVA: 0x00090F70 File Offset: 0x0008F170
		public override void InitializeCaches()
		{
			base.BaseModel.InitializeCaches();
		}

		// Token: 0x06001449 RID: 5193 RVA: 0x00090F80 File Offset: 0x0008F180
		public override MapWeatherModel.WeatherEvent GetWeatherEventInPosition(Vec2 pos)
		{
			MapWeatherModel.WeatherEvent weatherEventInPosition = base.BaseModel.GetWeatherEventInPosition(pos);
			if (weatherEventInPosition == null)
			{
				Storm storm = null;
				foreach (Storm storm2 in NavalDLCManager.Instance.StormManager.SpawnedStorms)
				{
					if (storm2.IsActive && storm2.CurrentPosition.Distance(pos) < storm2.EffectRadius * 1.25f)
					{
						storm = storm2;
						break;
					}
				}
				if (storm != null)
				{
					return 5;
				}
			}
			return weatherEventInPosition;
		}

		// Token: 0x0600144A RID: 5194 RVA: 0x00091018 File Offset: 0x0008F218
		public override Vec2 GetWindForPosition(CampaignVec2 position)
		{
			Vec2 windAtPosition = NavalDLCManager.Instance.NavalMapSceneWrapper.GetWindAtPosition(position.ToVec2());
			float num = MathF.Clamp(windAtPosition.Length, 0.06666667f, 0.46f);
			windAtPosition.Normalize();
			float normalizedWindStrengthOfStormForPosition = NavalDLCManager.Instance.GameModels.MapStormModel.GetNormalizedWindStrengthOfStormForPosition(position.ToVec2());
			float num2 = 0f;
			if (normalizedWindStrengthOfStormForPosition > 0f)
			{
				num2 = MBMath.Map(normalizedWindStrengthOfStormForPosition, 0f, 1f, 0.1f, 0.6f);
			}
			return windAtPosition * MathF.Clamp(num + num2, 1E-05f, 1f);
		}

		// Token: 0x0600144B RID: 5195 RVA: 0x000910B8 File Offset: 0x0008F2B8
		public override MapWeatherModel.WeatherEvent UpdateWeatherForPosition(CampaignVec2 position, CampaignTime ct)
		{
			if (!position.IsOnLand)
			{
				return 0;
			}
			return base.BaseModel.UpdateWeatherForPosition(position, ct);
		}

		// Token: 0x0600144C RID: 5196 RVA: 0x000910D4 File Offset: 0x0008F2D4
		private bool IsPositionInsideStormForMission(CampaignVec2 position)
		{
			Storm storm = null;
			foreach (Storm storm2 in NavalDLCManager.Instance.StormManager.SpawnedStorms)
			{
				if (storm2.IsActive && storm2.CurrentPosition.DistanceSquared(position.ToVec2()) < storm2.EffectRadius * storm2.EffectRadius)
				{
					storm = storm2;
					break;
				}
			}
			if (storm != null)
			{
				float num = storm.CurrentPosition.DistanceSquared(position.ToVec2());
				float num2 = storm.EffectRadius * storm.EffectRadius;
				float num3 = num / num2;
				float num4 = 0.64000005f;
				return num3 <= num4;
			}
			return false;
		}

		// Token: 0x04000AD3 RID: 2771
		private const float MaximumWindSpeed = 30f;

		// Token: 0x04000AD4 RID: 2772
		private const float MinWindWithStormOnCampaignMap = 0.1f;

		// Token: 0x04000AD5 RID: 2773
		private const float MaxWindWithStormOnCampaignMap = 1f;

		// Token: 0x04000AD6 RID: 2774
		private const float MinWindWithoutStormOnCampaignMap = 0.06666667f;

		// Token: 0x04000AD7 RID: 2775
		private const float MaxWindWithoutStormOnCampaignMap = 0.46f;

		// Token: 0x04000AD8 RID: 2776
		private const float MinWindSpeedRatioWithStormOnMission = 0.6666667f;

		// Token: 0x04000AD9 RID: 2777
		private const float MaxWindSpeedRatioWithStormOnMission = 1f;

		// Token: 0x04000ADA RID: 2778
		private const float MinWindSpeedRatioWithoutStormOnMission = 0.4f;

		// Token: 0x04000ADB RID: 2779
		private const float MaxWindSpeedRatioWithoutStormOnMission = 0.53333336f;
	}
}
