using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;

namespace NavalDLC.GameComponents
{
	// Token: 0x0200011C RID: 284
	public class NavalDLCMapDistanceModel : MapDistanceModel
	{
		// Token: 0x17000366 RID: 870
		// (get) Token: 0x06001428 RID: 5160 RVA: 0x000901CD File Offset: 0x0008E3CD
		public override int RegionSwitchCostFromLandToSea
		{
			get
			{
				return 50;
			}
		}

		// Token: 0x17000367 RID: 871
		// (get) Token: 0x06001429 RID: 5161 RVA: 0x000901D1 File Offset: 0x0008E3D1
		public override int RegionSwitchCostFromSeaToLand
		{
			get
			{
				return 50;
			}
		}

		// Token: 0x17000368 RID: 872
		// (get) Token: 0x0600142A RID: 5162 RVA: 0x000901D5 File Offset: 0x0008E3D5
		public override float MaximumSpawnDistanceForCompanionsAfterDisband
		{
			get
			{
				return 150f;
			}
		}

		// Token: 0x0600142C RID: 5164 RVA: 0x000901EF File Offset: 0x0008E3EF
		public override void RegisterDistanceCache(MobileParty.NavigationType navigationCapability, MapDistanceModel.INavigationCache cacheToRegister)
		{
			this._navigationCaches[navigationCapability] = cacheToRegister;
			cacheToRegister.FinalizeInitialization();
		}

		// Token: 0x0600142D RID: 5165 RVA: 0x00090204 File Offset: 0x0008E404
		public override float GetMaximumDistanceBetweenTwoConnectedSettlements(MobileParty.NavigationType navigationCapabilities)
		{
			MapDistanceModel.INavigationCache navigationCache;
			if (this._navigationCaches.TryGetValue(navigationCapabilities, out navigationCache))
			{
				return navigationCache.MaximumDistanceBetweenTwoConnectedSettlements;
			}
			return 0f;
		}

		// Token: 0x0600142E RID: 5166 RVA: 0x00090230 File Offset: 0x0008E430
		public override float GetLandRatioOfPathBetweenSettlements(Settlement fromSettlement, Settlement toSettlement, bool isFromPort, bool isTargetingPort)
		{
			MapDistanceModel.INavigationCache navigationCache;
			if (this._navigationCaches.TryGetValue(3, out navigationCache))
			{
				float num;
				navigationCache.GetSettlementToSettlementDistanceWithLandRatio(fromSettlement, isFromPort, toSettlement, isTargetingPort, ref num);
				return num;
			}
			return 1f;
		}

		// Token: 0x0600142F RID: 5167 RVA: 0x00090264 File Offset: 0x0008E464
		public override float GetDistance(Settlement fromSettlement, Settlement toSettlement, bool isFromPort = false, bool isTargetingPort = false, MobileParty.NavigationType navigationCapability = 3)
		{
			float num;
			return this.GetDistance(fromSettlement, toSettlement, isFromPort, isTargetingPort, navigationCapability, ref num);
		}

		// Token: 0x06001430 RID: 5168 RVA: 0x00090280 File Offset: 0x0008E480
		public override float GetDistance(Settlement fromSettlement, Settlement toSettlement, bool isFromPort, bool isTargetingPort, MobileParty.NavigationType navigationCapability, out float landRatio)
		{
			float num = float.MaxValue;
			landRatio = -1f;
			if (fromSettlement != null && toSettlement != null)
			{
				if (fromSettlement != toSettlement)
				{
					return this._navigationCaches[navigationCapability].GetSettlementToSettlementDistanceWithLandRatio(fromSettlement, isFromPort, toSettlement, isTargetingPort, ref landRatio);
				}
				num = ((isFromPort == isTargetingPort) ? 0f : ((float)(isFromPort ? this.RegionSwitchCostFromSeaToLand : this.RegionSwitchCostFromLandToSea)));
				landRatio = ((navigationCapability == 3) ? 0.5f : ((navigationCapability == 1) ? 1f : ((navigationCapability == 2) ? 0f : (-1f))));
			}
			return num;
		}

		// Token: 0x06001431 RID: 5169 RVA: 0x00090310 File Offset: 0x0008E510
		public override float GetPortToGateDistanceForSettlement(Settlement settlement)
		{
			float num;
			return this._navigationCaches[3].GetSettlementToSettlementDistanceWithLandRatio(settlement, true, settlement, false, ref num);
		}

		// Token: 0x06001432 RID: 5170 RVA: 0x00090334 File Offset: 0x0008E534
		public override float GetDistance(MobileParty fromMobileParty, Settlement toSettlement, bool isTargetingPort, MobileParty.NavigationType customCapability, out float estimatedLandRatio)
		{
			float num = 100000000f;
			estimatedLandRatio = -1f;
			int faceIndex = fromMobileParty.CurrentNavigationFace.FaceIndex;
			int num2;
			if (!isTargetingPort)
			{
				CampaignVec2 campaignVec = toSettlement.GatePosition;
				num2 = campaignVec.Face.FaceIndex;
			}
			else
			{
				CampaignVec2 campaignVec = toSettlement.PortPosition;
				num2 = campaignVec.Face.FaceIndex;
			}
			if (faceIndex == num2)
			{
				PartyNavigationModel partyNavigationModel = Campaign.Current.Models.PartyNavigationModel;
				IMapScene mapSceneWrapper = Campaign.Current.MapSceneWrapper;
				CampaignVec2 campaignVec = fromMobileParty.Position;
				if (partyNavigationModel.IsTerrainTypeValidForNavigationType(mapSceneWrapper.GetFaceTerrainType(campaignVec.Face), customCapability))
				{
					campaignVec = fromMobileParty.Position;
					num = campaignVec.Distance(isTargetingPort ? toSettlement.PortPosition : toSettlement.GatePosition);
					estimatedLandRatio = ((customCapability == 1) ? 1f : ((customCapability == 2) ? 0f : 0.5f));
				}
			}
			else if (customCapability == 1 && (fromMobileParty.IsCurrentlyAtSea || isTargetingPort))
			{
				num = 100000000f;
			}
			else if (customCapability == 2 && (!fromMobileParty.IsCurrentlyAtSea || !isTargetingPort))
			{
				num = 100000000f;
			}
			else
			{
				ValueTuple<Settlement, bool> closestEntranceToFace = Campaign.Current.Models.MapDistanceModel.GetClosestEntranceToFace(fromMobileParty.CurrentNavigationFace, customCapability);
				Settlement item = closestEntranceToFace.Item1;
				if (item != null)
				{
					bool item2 = closestEntranceToFace.Item2;
					CampaignVec2 campaignVec2 = (item2 ? item.PortPosition : item.GatePosition);
					CampaignVec2 campaignVec3 = (isTargetingPort ? toSettlement.PortPosition : toSettlement.GatePosition);
					CampaignVec2 campaignVec = fromMobileParty.Position;
					num = campaignVec.Distance(campaignVec3) - campaignVec2.Distance(campaignVec3) + Campaign.Current.Models.MapDistanceModel.GetDistance(item, toSettlement, item2, isTargetingPort, customCapability);
					if (item != toSettlement && customCapability == 3)
					{
						bool flag = item.HasPort && fromMobileParty.HasNavalNavigationCapability;
						bool flag2 = toSettlement.HasPort && fromMobileParty.HasNavalNavigationCapability;
						estimatedLandRatio = this.GetLandRatioOfPathBetweenSettlements(item, toSettlement, flag, flag2);
					}
					else
					{
						estimatedLandRatio = ((customCapability == 3) ? 0.5f : ((customCapability == 1) ? 1f : ((customCapability == 2) ? 0f : (-1f))));
					}
					if (customCapability == 3)
					{
						num += Campaign.Current.Models.MapDistanceModel.GetTransitionCostAdjustment(item, item2, toSettlement, isTargetingPort, fromMobileParty.IsCurrentlyAtSea, isTargetingPort);
						if (fromMobileParty.IsCurrentlyAtSea == isTargetingPort)
						{
							float distance = Campaign.Current.Models.MapDistanceModel.GetDistance(fromMobileParty, toSettlement, isTargetingPort, fromMobileParty.IsCurrentlyAtSea ? 2 : 1, ref estimatedLandRatio);
							num = MathF.Min(num, distance);
						}
					}
				}
			}
			return MBMath.ClampFloat(num, 0f, float.MaxValue);
		}

		// Token: 0x06001433 RID: 5171 RVA: 0x000905A8 File Offset: 0x0008E7A8
		public override float GetDistance(MobileParty fromMobileParty, MobileParty toMobileParty, MobileParty.NavigationType customCapability, out float landRatio)
		{
			float num;
			Campaign.Current.Models.MapDistanceModel.GetDistance(fromMobileParty, toMobileParty, customCapability, 100000000f, ref num, ref landRatio);
			return num;
		}

		// Token: 0x06001434 RID: 5172 RVA: 0x000905D8 File Offset: 0x0008E7D8
		public override bool GetDistance(MobileParty fromMobileParty, MobileParty toMobileParty, MobileParty.NavigationType customCapability, float maxDistance, out float distance, out float landRatio)
		{
			landRatio = ((customCapability == 1) ? 1f : ((customCapability == 2) ? 0f : (-0.5f)));
			distance = float.MaxValue;
			if (fromMobileParty.CurrentNavigationFace.FaceIndex == toMobileParty.CurrentNavigationFace.FaceIndex)
			{
				if (Campaign.Current.Models.PartyNavigationModel.IsTerrainTypeValidForNavigationType(Campaign.Current.MapSceneWrapper.GetFaceTerrainType(fromMobileParty.Position.Face), customCapability))
				{
					distance = fromMobileParty.Position.Distance(toMobileParty.Position);
				}
			}
			else if (customCapability == 1 && (fromMobileParty.IsCurrentlyAtSea || toMobileParty.IsCurrentlyAtSea))
			{
				distance = float.MaxValue;
			}
			else if (customCapability == 2 && (!fromMobileParty.IsCurrentlyAtSea || !toMobileParty.IsCurrentlyAtSea))
			{
				distance = float.MaxValue;
			}
			else
			{
				distance = fromMobileParty.Position.Distance(toMobileParty.Position);
			}
			distance = MBMath.ClampFloat(distance, 0f, float.MaxValue);
			return distance <= maxDistance;
		}

		// Token: 0x06001435 RID: 5173 RVA: 0x000906E4 File Offset: 0x0008E8E4
		public override float GetDistance(MobileParty fromMobileParty, in CampaignVec2 toPoint, MobileParty.NavigationType customCapability, out float landRatio)
		{
			float num = float.MaxValue;
			landRatio = -1f;
			CampaignVec2 campaignVec = toPoint;
			PathFaceRecord face = campaignVec.Face;
			if (fromMobileParty.CurrentNavigationFace.FaceIndex == face.FaceIndex)
			{
				if (Campaign.Current.Models.PartyNavigationModel.IsTerrainTypeValidForNavigationType(Campaign.Current.MapSceneWrapper.GetFaceTerrainType(fromMobileParty.Position.Face), customCapability))
				{
					num = fromMobileParty.Position.Distance(toPoint);
					landRatio = ((customCapability == 1) ? 1f : ((customCapability == 2) ? 0f : (-0.5f)));
				}
			}
			else
			{
				MapDistanceModel mapDistanceModel = Campaign.Current.Models.MapDistanceModel;
				ValueTuple<Settlement, bool> closestEntranceToFace = mapDistanceModel.GetClosestEntranceToFace(fromMobileParty.CurrentNavigationFace, customCapability);
				ValueTuple<Settlement, bool> closestEntranceToFace2 = mapDistanceModel.GetClosestEntranceToFace(face, customCapability);
				Settlement item = closestEntranceToFace.Item1;
				Settlement item2 = closestEntranceToFace2.Item1;
				if (item != null && item2 != null)
				{
					bool flag = NavigationHelper.IsPositionValidForNavigationType(toPoint, 2);
					bool item3 = closestEntranceToFace.Item2;
					bool item4 = closestEntranceToFace2.Item2;
					CampaignVec2 campaignVec2 = (item3 ? item.PortPosition : item.GatePosition);
					CampaignVec2 campaignVec3 = (item4 ? item2.PortPosition : item2.GatePosition);
					num = fromMobileParty.Position.Distance(toPoint) - campaignVec2.Distance(campaignVec3) + this.GetDistance(item, item2, item3, item4, customCapability);
					if (customCapability == 3)
					{
						num += mapDistanceModel.GetTransitionCostAdjustment(item, item3, item2, item4, fromMobileParty.IsCurrentlyAtSea, flag);
						if (fromMobileParty.IsCurrentlyAtSea == flag)
						{
							float distance = mapDistanceModel.GetDistance(fromMobileParty, ref toPoint, fromMobileParty.IsCurrentlyAtSea ? 2 : 1, ref landRatio);
							num = MathF.Min(num, distance);
						}
					}
				}
			}
			return MBMath.ClampFloat(num, 0f, float.MaxValue);
		}

		// Token: 0x06001436 RID: 5174 RVA: 0x000908AC File Offset: 0x0008EAAC
		public override float GetDistance(Settlement fromSettlement, in CampaignVec2 toPoint, bool isFromPort, MobileParty.NavigationType customCapability)
		{
			float num = float.MaxValue;
			CampaignVec2 campaignVec = (isFromPort ? fromSettlement.PortPosition : fromSettlement.GatePosition);
			CampaignVec2 campaignVec2 = toPoint;
			PathFaceRecord face = campaignVec2.Face;
			PathFaceRecord face2 = campaignVec.Face;
			if (face2.FaceIndex == face.FaceIndex)
			{
				if (Campaign.Current.Models.PartyNavigationModel.IsTerrainTypeValidForNavigationType(Campaign.Current.MapSceneWrapper.GetFaceTerrainType(face2), customCapability))
				{
					num = campaignVec.Distance(toPoint);
				}
			}
			else
			{
				MapDistanceModel mapDistanceModel = Campaign.Current.Models.MapDistanceModel;
				ValueTuple<Settlement, bool> closestEntranceToFace = mapDistanceModel.GetClosestEntranceToFace(face, customCapability);
				Settlement item = closestEntranceToFace.Item1;
				if (item != null)
				{
					bool flag = NavigationHelper.IsPositionValidForNavigationType(toPoint, 2);
					bool item2 = closestEntranceToFace.Item2;
					CampaignVec2 campaignVec3 = (isFromPort ? fromSettlement.PortPosition : fromSettlement.GatePosition);
					CampaignVec2 campaignVec4 = (item2 ? item.PortPosition : item.GatePosition);
					num = campaignVec3.Distance(toPoint) - campaignVec3.Distance(campaignVec4) + mapDistanceModel.GetDistance(fromSettlement, item, isFromPort, item2, customCapability);
					if (customCapability == 3)
					{
						num += mapDistanceModel.GetTransitionCostAdjustment(fromSettlement, isFromPort, item, item2, isFromPort, flag);
						if (isFromPort == flag)
						{
							float distance = mapDistanceModel.GetDistance(fromSettlement, ref toPoint, isFromPort, isFromPort ? 2 : 1);
							num = MathF.Min(num, distance);
						}
					}
				}
			}
			return MBMath.ClampFloat(num, 0f, 100000000f);
		}

		// Token: 0x06001437 RID: 5175 RVA: 0x00090A14 File Offset: 0x0008EC14
		public override bool PathExistBetweenPoints(in CampaignVec2 fromPoint, in CampaignVec2 toPoint, MobileParty.NavigationType navigationType)
		{
			MapDistanceModel mapDistanceModel = Campaign.Current.Models.MapDistanceModel;
			CampaignVec2 campaignVec = fromPoint;
			ValueTuple<Settlement, bool> closestEntranceToFace = mapDistanceModel.GetClosestEntranceToFace(campaignVec.Face, navigationType);
			MapDistanceModel mapDistanceModel2 = Campaign.Current.Models.MapDistanceModel;
			campaignVec = toPoint;
			ValueTuple<Settlement, bool> closestEntranceToFace2 = mapDistanceModel2.GetClosestEntranceToFace(campaignVec.Face, navigationType);
			return closestEntranceToFace.Item1 != null && closestEntranceToFace2.Item1 != null && Campaign.Current.Models.MapDistanceModel.GetDistance(closestEntranceToFace.Item1, closestEntranceToFace2.Item1, closestEntranceToFace.Item2, closestEntranceToFace2.Item2, navigationType) < Campaign.MapDiagonal * 10f;
		}

		// Token: 0x06001438 RID: 5176 RVA: 0x00090AB8 File Offset: 0x0008ECB8
		public override ValueTuple<Settlement, bool> GetClosestEntranceToFace(PathFaceRecord face, MobileParty.NavigationType navigationCapabilities)
		{
			bool flag;
			return new ValueTuple<Settlement, bool>(this._navigationCaches[navigationCapabilities].GetClosestSettlementToFaceIndex(face.FaceIndex, ref flag), flag);
		}

		// Token: 0x06001439 RID: 5177 RVA: 0x00090AE4 File Offset: 0x0008ECE4
		public override MBReadOnlyList<Settlement> GetNeighborsOfFortification(Town town, MobileParty.NavigationType navigationCapabilities)
		{
			MapDistanceModel.INavigationCache navigationCache;
			if (!this._navigationCaches.TryGetValue(navigationCapabilities, out navigationCache))
			{
				Debug.FailedAssert("cache not found", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\GameComponents\\NavalDLCMapDistanceModel.cs", "GetNeighborsOfFortification", 376);
				return new MBReadOnlyList<Settlement>();
			}
			return navigationCache.GetNeighbors(town.Settlement);
		}

		// Token: 0x0600143A RID: 5178 RVA: 0x00090B2C File Offset: 0x0008ED2C
		public override float GetTransitionCostAdjustment(Settlement settlement1, bool isFromPort, Settlement settlement2, bool isTargetingPort, bool fromIsCurrentlyAtSea, bool toIsCurrentlyAtSea)
		{
			float num = 0f;
			if (isFromPort != fromIsCurrentlyAtSea)
			{
				num -= (float)(isFromPort ? this.RegionSwitchCostFromSeaToLand : this.RegionSwitchCostFromLandToSea);
			}
			if (isTargetingPort != toIsCurrentlyAtSea)
			{
				num -= (float)(isTargetingPort ? this.RegionSwitchCostFromLandToSea : this.RegionSwitchCostFromSeaToLand);
			}
			if (isFromPort == isTargetingPort && Campaign.Current.Models.MapDistanceModel.GetDistance(settlement1, settlement2, isFromPort, isTargetingPort, isFromPort ? 2 : 1) < Campaign.MapDiagonalSquared)
			{
				num *= -1f;
			}
			return num;
		}

		// Token: 0x04000AD0 RID: 2768
		private Dictionary<MobileParty.NavigationType, MapDistanceModel.INavigationCache> _navigationCaches = new Dictionary<MobileParty.NavigationType, MapDistanceModel.INavigationCache>();
	}
}
