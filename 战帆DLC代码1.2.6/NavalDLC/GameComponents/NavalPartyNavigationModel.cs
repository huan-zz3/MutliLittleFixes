using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace NavalDLC.GameComponents
{
	// Token: 0x02000141 RID: 321
	public class NavalPartyNavigationModel : PartyNavigationModel
	{
		// Token: 0x06001560 RID: 5472 RVA: 0x00095F8B File Offset: 0x0009418B
		public override float GetEmbarkDisembarkThresholdDistance()
		{
			return 0.5f;
		}

		// Token: 0x06001561 RID: 5473 RVA: 0x00095F92 File Offset: 0x00094192
		private static bool IsTerrainTypeValidForNaval(TerrainType t)
		{
			return t == 8 || t == 10 || t == 11 || t == 18 || t == 19 || t == 23 || t == 24 || t == 25;
		}

		// Token: 0x06001562 RID: 5474 RVA: 0x00095FBD File Offset: 0x000941BD
		public NavalPartyNavigationModel(PartyNavigationModel partyNavigationModel)
		{
			this._baseModel = partyNavigationModel;
			this.InitializeInvalidTypesCache();
		}

		// Token: 0x06001563 RID: 5475 RVA: 0x00095FE0 File Offset: 0x000941E0
		private void InitializeInvalidTypesCache()
		{
			this._invalidTypesIntegerCache.Clear();
			List<int> list = new List<int>();
			List<int> list2 = new List<int>();
			List<int> list3 = new List<int>();
			foreach (object obj in Enum.GetValues(typeof(TerrainType)))
			{
				TerrainType terrainType = (TerrainType)obj;
				if (!this.IsTerrainTypeValidForNavigationType(terrainType, 3))
				{
					list.Add(terrainType);
				}
				if (!this.IsTerrainTypeValidForNavigationType(terrainType, 2))
				{
					list3.Add(terrainType);
				}
				if (!this.IsTerrainTypeValidForNavigationType(terrainType, 1))
				{
					list2.Add(terrainType);
				}
			}
			this._invalidTypesIntegerCache.Add(3, list.ToArray());
			this._invalidTypesIntegerCache.Add(1, list2.ToArray());
			this._invalidTypesIntegerCache.Add(2, list3.ToArray());
		}

		// Token: 0x06001564 RID: 5476 RVA: 0x000960CC File Offset: 0x000942CC
		public override bool IsTerrainTypeValidForNavigationType(TerrainType terrainType, MobileParty.NavigationType navigationType)
		{
			if (navigationType == 2)
			{
				return NavalPartyNavigationModel.IsTerrainTypeValidForNaval(terrainType);
			}
			if (navigationType == 3)
			{
				return NavalPartyNavigationModel.IsTerrainTypeValidForNaval(terrainType) || this._baseModel.IsTerrainTypeValidForNavigationType(terrainType, navigationType);
			}
			return this._baseModel.IsTerrainTypeValidForNavigationType(terrainType, navigationType);
		}

		// Token: 0x06001565 RID: 5477 RVA: 0x00096102 File Offset: 0x00094302
		public override int[] GetInvalidTerrainTypesForNavigationType(MobileParty.NavigationType navigationType)
		{
			if (this._invalidTypesIntegerCache.ContainsKey(navigationType))
			{
				return this._invalidTypesIntegerCache[navigationType];
			}
			return new int[0];
		}

		// Token: 0x06001566 RID: 5478 RVA: 0x00096128 File Offset: 0x00094328
		public override bool HasNavalNavigationCapability(MobileParty mobileParty)
		{
			if (mobileParty.Ships.Count > 0)
			{
				return true;
			}
			if (mobileParty.IsMainParty)
			{
				return false;
			}
			if (mobileParty.AttachedTo != null && mobileParty.AttachedTo.HasNavalNavigationCapability)
			{
				return true;
			}
			if (mobileParty.AttachedParties.Count > 0)
			{
				return mobileParty.AttachedParties.Any<MobileParty>((MobileParty x) => x.Ships.Count > 0);
			}
			return false;
		}

		// Token: 0x06001567 RID: 5479 RVA: 0x000961A0 File Offset: 0x000943A0
		public override bool CanPlayerNavigateToPosition(CampaignVec2 vec2, out MobileParty.NavigationType navigationType)
		{
			navigationType = 0;
			if (!vec2.Face.IsValid())
			{
				return false;
			}
			if (!MobileParty.MainParty.IsCurrentlyAtSea && NavigationHelper.IsPositionValidForNavigationType(vec2, 2))
			{
				return false;
			}
			if (MobileParty.MainParty.IsCurrentlyAtSea)
			{
				if (MobileParty.MainParty.HasNavalNavigationCapability && NavigationHelper.IsPositionValidForNavigationType(vec2, 2))
				{
					navigationType = 2;
				}
				else
				{
					navigationType = MobileParty.MainParty.NavigationCapability;
				}
			}
			else
			{
				navigationType = 1;
			}
			int[] invalidTerrainTypesForNavigationType = Campaign.Current.Models.PartyNavigationModel.GetInvalidTerrainTypesForNavigationType(navigationType);
			float num;
			return !invalidTerrainTypesForNavigationType.Contains(vec2.Face.FaceGroupIndex) && ((!vec2.IsOnLand && MobileParty.MainParty.IsCurrentlyAtSea) || Campaign.Current.MapSceneWrapper.GetPathDistanceBetweenAIFaces(MobileParty.MainParty.CurrentNavigationFace, vec2.Face, MobileParty.MainParty.Position.ToVec2(), vec2.ToVec2(), 0.3f, (float)Campaign.PathFindingMaxCostLimit, ref num, invalidTerrainTypesForNavigationType, MobileParty.MainParty.GetRegionSwitchCostFromLandToSea(), MobileParty.MainParty.GetRegionSwitchCostFromSeaToLand()));
		}

		// Token: 0x04000B12 RID: 2834
		private readonly Dictionary<MobileParty.NavigationType, int[]> _invalidTypesIntegerCache = new Dictionary<MobileParty.NavigationType, int[]>();

		// Token: 0x04000B13 RID: 2835
		private readonly PartyNavigationModel _baseModel;
	}
}
