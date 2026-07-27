using System;
using System.Collections.Generic;
using System.Diagnostics;
using NavalDLC.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace NavalDLC.CampaignBehaviors
{
	// Token: 0x0200017A RID: 378
	public class StormCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x060018CC RID: 6348 RVA: 0x000AC495 File Offset: 0x000AA695
		public override void RegisterEvents()
		{
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunchedEvent));
			CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.HourlyTick));
		}

		// Token: 0x060018CD RID: 6349 RVA: 0x000AC4C8 File Offset: 0x000AA6C8
		private void HourlyTick()
		{
			foreach (Storm storm in NavalDLCManager.Instance.StormManager.SpawnedStorms)
			{
				if (storm.IsActive)
				{
					if (MBRandom.RandomFloat < 0.5f)
					{
						storm.ChangeMoveDirection();
					}
					float hourlyIntensityChangeForStorm = NavalDLCManager.Instance.GameModels.MapStormModel.GetHourlyIntensityChangeForStorm(storm);
					storm.Intensity += hourlyIntensityChangeForStorm;
					this.DamageNearbyParties(storm);
				}
			}
			this.TrySpawningNewStorm();
		}

		// Token: 0x060018CE RID: 6350 RVA: 0x000AC568 File Offset: 0x000AA768
		private void DamageNearbyParties(Storm spawnedStorm)
		{
			LocatableSearchData<MobileParty> locatableSearchData = MobileParty.StartFindingLocatablesAroundPosition(spawnedStorm.CurrentPosition, spawnedStorm.EffectRadius);
			for (MobileParty mobileParty = MobileParty.FindNextLocatable(ref locatableSearchData); mobileParty != null; mobileParty = MobileParty.FindNextLocatable(ref locatableSearchData))
			{
				if (mobileParty.AttachedTo == null)
				{
					this.TryDamagingParty(mobileParty, spawnedStorm);
				}
			}
		}

		// Token: 0x060018CF RID: 6351 RVA: 0x000AC5AC File Offset: 0x000AA7AC
		private void TryDamagingParty(MobileParty mobileParty, Storm affectingStorm)
		{
			if (NavalDLCManager.Instance.GameModels.MapStormModel.CanPartyGetDamagedByStorm(mobileParty))
			{
				for (int i = mobileParty.Ships.Count - 1; i >= 0; i--)
				{
					Ship ship = mobileParty.Ships[i];
					float positionDamageForStorm = NavalDLCManager.Instance.GameModels.MapStormModel.GetPositionDamageForStorm(affectingStorm, mobileParty.Position.ToVec2(), ship);
					float num;
					ship.OnShipDamaged(positionDamageForStorm, null, ref num);
					bool debugVisualsEnabled = NavalDLCManager.Instance.StormManager.DebugVisualsEnabled;
				}
			}
			foreach (MobileParty mobileParty2 in mobileParty.AttachedParties)
			{
				this.TryDamagingParty(mobileParty2, affectingStorm);
			}
		}

		// Token: 0x060018D0 RID: 6352 RVA: 0x000AC680 File Offset: 0x000AA880
		private void TrySpawningNewStorm()
		{
			foreach (int num in this._weatherNodePositionsShuffledIndices)
			{
				Vec2 vec = this._allOpenSeaWeatherNodePositions[num];
				int count = NavalDLCManager.Instance.StormManager.SpawnedStorms.Count;
				if (NavalDLCManager.Instance.GameModels.MapStormModel.GetHourlyStormSpawnChanceForPosition(vec) > MBRandom.RandomFloat && count < NavalDLCManager.Instance.GameModels.MapStormModel.MaximumNumberOfStorms)
				{
					bool flag = false;
					using (List<Storm>.Enumerator enumerator = NavalDLCManager.Instance.StormManager.SpawnedStorms.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (enumerator.Current.CurrentPosition.DistanceSquared(vec) < this._spawnDistanceSquaredThreshold)
							{
								flag = true;
								break;
							}
						}
					}
					if (!flag)
					{
						NavalDLCManager.Instance.StormManager.CreateStormAtPosition(vec);
					}
				}
			}
		}

		// Token: 0x060018D1 RID: 6353 RVA: 0x000AC784 File Offset: 0x000AA984
		private void CreateAndShuffleWeatherNodeDataIndicesDeterministic()
		{
			int count = this._allOpenSeaWeatherNodePositions.Count;
			this._weatherNodePositionsShuffledIndices = new int[count];
			for (int i = 0; i < count; i++)
			{
				this._weatherNodePositionsShuffledIndices[i] = i;
			}
			MBFastRandom mbfastRandom = new MBFastRandom((uint)Extensions.GetDeterministicHashCode(Campaign.Current.UniqueGameId));
			for (int j = 0; j < 20; j++)
			{
				for (int k = 0; k < count; k++)
				{
					int num = mbfastRandom.Next(count);
					int num2 = this._weatherNodePositionsShuffledIndices[k];
					this._weatherNodePositionsShuffledIndices[k] = this._weatherNodePositionsShuffledIndices[num];
					this._weatherNodePositionsShuffledIndices[num] = num2;
				}
			}
		}

		// Token: 0x060018D2 RID: 6354 RVA: 0x000AC821 File Offset: 0x000AAA21
		private void OnSessionLaunchedEvent(CampaignGameStarter obj)
		{
			this._spawnDistanceSquaredThreshold = NavalDLCManager.Instance.GameModels.MapStormModel.GetStormSpawnDistanceSquaredThresholdWithOtherStorms();
			this.InitializeStormNodes();
		}

		// Token: 0x060018D3 RID: 6355 RVA: 0x000AC844 File Offset: 0x000AAA44
		private void InitializeStormNodes()
		{
			this._allOpenSeaWeatherNodePositions = new List<Vec2>();
			Vec2 terrainSize = Campaign.Current.MapSceneWrapper.GetTerrainSize();
			int defaultWeatherNodeDimension = Campaign.Current.DefaultWeatherNodeDimension;
			int num = defaultWeatherNodeDimension;
			int num2 = defaultWeatherNodeDimension;
			for (int i = 0; i < num2; i++)
			{
				for (int j = 0; j < num; j++)
				{
					float num3 = (float)i / (float)defaultWeatherNodeDimension * terrainSize.X;
					float num4 = (float)j / (float)defaultWeatherNodeDimension * terrainSize.Y;
					Vec2 vec;
					vec..ctor(num3, num4);
					IMapScene mapSceneWrapper = Campaign.Current.MapSceneWrapper;
					CampaignVec2 campaignVec = new CampaignVec2(vec, false);
					if (mapSceneWrapper.GetFaceIndex(ref campaignVec).IsValid())
					{
						IMapScene mapSceneWrapper2 = Campaign.Current.MapSceneWrapper;
						campaignVec = new CampaignVec2(vec, false);
						if (mapSceneWrapper2.GetTerrainTypeAtPosition(ref campaignVec) == 19)
						{
							this._allOpenSeaWeatherNodePositions.Add(vec);
						}
					}
				}
			}
			this.CreateAndShuffleWeatherNodeDataIndicesDeterministic();
		}

		// Token: 0x060018D4 RID: 6356 RVA: 0x000AC92A File Offset: 0x000AAB2A
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x060018D5 RID: 6357 RVA: 0x000AC92C File Offset: 0x000AAB2C
		[Conditional("DEBUG")]
		private void MainPartyStormDamageDebugVisualTick(Storm storm, MobileParty nearbyParty, Ship ship, float damage)
		{
			if (nearbyParty.IsMainParty)
			{
				MobileParty mainParty = MobileParty.MainParty;
				float maximumWeatherStrengthAtEye = NavalDLCManager.Instance.GameModels.MapStormModel.GetMaximumWeatherStrengthAtEye(storm);
				float num = storm.CurrentPosition.Distance(mainParty.Position.ToVec2());
				float minimumWeatherStrengthInsideStorm = NavalDLCManager.Instance.GameModels.MapStormModel.MinimumWeatherStrengthInsideStorm;
				if (num >= storm.EyeRadius && num + storm.EyeRadius < storm.EffectRadius)
				{
					MBMath.Map(num, 0f, storm.EffectRadius, maximumWeatherStrengthAtEye, NavalDLCManager.Instance.GameModels.MapStormModel.MinimumWeatherStrengthInsideStorm);
				}
				Campaign.Current.Models.CampaignShipParametersModel.GetShipSizeWeatherFactor(ship.ShipHull);
			}
		}

		// Token: 0x04000C10 RID: 3088
		private List<Vec2> _allOpenSeaWeatherNodePositions;

		// Token: 0x04000C11 RID: 3089
		private float _spawnDistanceSquaredThreshold;

		// Token: 0x04000C12 RID: 3090
		private int[] _weatherNodePositionsShuffledIndices;
	}
}
