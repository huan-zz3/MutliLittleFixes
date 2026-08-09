using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using NavalDLC.GameComponents;
using StoryMode;
using StoryMode.StoryModePhases;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.SaveSystem;

namespace NavalDLC.CampaignBehaviors
{
	// Token: 0x02000171 RID: 369
	public class PiratesCampaignBehavior : CampaignBehaviorBase, IPiratePatrolBehavior
	{
		// Token: 0x06001829 RID: 6185 RVA: 0x000A4B10 File Offset: 0x000A2D10
		public override void RegisterEvents()
		{
			CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnNewGameCreated));
			CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnGameLoaded));
			CampaignEvents.OnNewGameCreatedPartialFollowUpEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter, int>(this.OnNewGameCreatedPartialFollowUp));
			CampaignEvents.DailyTickClanEvent.AddNonSerializedListener(this, new Action<Clan>(this.DailyTickClan));
			CampaignEvents.DailyTickPartyEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.DailyTickParty));
			CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(this, new Action<MobileParty, PartyBase>(this.OnMobilePartyDestroyed));
			CampaignEvents.AiHourlyTickEvent.AddNonSerializedListener(this, new Action<MobileParty, PartyThinkParams>(this.AiHourlyTick));
			CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.MapEventEnded));
			TutorialPhase instance = TutorialPhase.Instance;
			if (instance != null && !instance.IsCompleted)
			{
				StoryModeEvents.OnStoryModeTutorialEndedEvent.AddNonSerializedListener(this, new Action(this.OnTutorialEnded));
			}
		}

		// Token: 0x0600182A RID: 6186 RVA: 0x000A4C04 File Offset: 0x000A2E04
		private void MapEventEnded(MapEvent mapEvent)
		{
			foreach (PartyBase partyBase in mapEvent.InvolvedParties)
			{
				if (partyBase.IsMobile && mapEvent.WinningSide == partyBase.Side && this.IsPirateParty(partyBase.MobileParty) && partyBase.MobileParty.Ships.Count > Campaign.Current.Models.PartyShipLimitModel.GetIdealShipNumber(partyBase.MobileParty))
				{
					this.DiscardShips(partyBase.MobileParty);
				}
			}
		}

		// Token: 0x0600182B RID: 6187 RVA: 0x000A4CA8 File Offset: 0x000A2EA8
		private void DiscardShips(MobileParty pirateParty)
		{
			MBList<Ship> mblist = Extensions.ToMBList<Ship>(pirateParty.Ships.OrderByDescending<Ship, float>((Ship x) => Campaign.Current.Models.PartyShipLimitModel.GetShipPriority(pirateParty, x, false)));
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < mblist.Count; i++)
			{
				Ship ship = mblist[i];
				ShipHull.ShipType type = ship.ShipHull.Type;
				if (num2 < 2 && (type != 1 || num2 == 0 || num < 2) && num < Campaign.Current.Models.PartyShipLimitModel.GetIdealShipNumber(pirateParty))
				{
					num++;
					if (type == 1)
					{
						num2++;
					}
				}
				else
				{
					DestroyShipAction.ApplyByDiscard(ship);
				}
			}
		}

		// Token: 0x0600182C RID: 6188 RVA: 0x000A4D60 File Offset: 0x000A2F60
		private void OnTutorialEnded()
		{
			if (TutorialPhase.Instance.IsSkipped)
			{
				for (int i = 0; i < 3; i++)
				{
					foreach (Clan clan in Clan.BanditFactions)
					{
						this.DailyTickClan(clan);
					}
				}
			}
			StoryModeEvents.OnStoryModeTutorialEndedEvent.ClearListeners(this);
		}

		// Token: 0x0600182D RID: 6189 RVA: 0x000A4DD0 File Offset: 0x000A2FD0
		private void OnMobilePartyDestroyed(MobileParty party, PartyBase destroyerParty)
		{
			PiratesCampaignBehavior.PatrolZone patrolZone;
			if (this._assignedZones.TryGetValue(party, out patrolZone))
			{
				this.UnassignPartyFromZone(patrolZone, party);
			}
		}

		// Token: 0x0600182E RID: 6190 RVA: 0x000A4DF8 File Offset: 0x000A2FF8
		private void AiHourlyTick(MobileParty mobileParty, PartyThinkParams p)
		{
			if (this.IsPirateParty(mobileParty))
			{
				PiratesCampaignBehavior.PatrolZone assignedZone = this.GetAssignedZone(mobileParty);
				if (assignedZone != null)
				{
					MobileParty.NavigationType navigationType = 2;
					AIBehaviorData aibehaviorData;
					aibehaviorData..ctor(assignedZone.Position, 13, navigationType, false, false, false);
					ValueTuple<AIBehaviorData, float> valueTuple = new ValueTuple<AIBehaviorData, float>(aibehaviorData, 5f);
					p.AddBehaviorScore(ref valueTuple);
					return;
				}
				Debug.FailedAssert("This should only be possible for cheats & mods.", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\CampaignBehaviors\\PiratesCampaignBehavior.cs", "AiHourlyTick", 244);
			}
		}

		// Token: 0x0600182F RID: 6191 RVA: 0x000A4E5C File Offset: 0x000A305C
		private void OnNewGameCreated(CampaignGameStarter starter)
		{
			this.AssignPatrolZones();
		}

		// Token: 0x06001830 RID: 6192 RVA: 0x000A4E64 File Offset: 0x000A3064
		private void AssignPatrolZones()
		{
			foreach (Clan clan in Clan.BanditFactions)
			{
				if (this.IsPirateClan(clan))
				{
					this.AssignPatrolZones(clan);
				}
			}
		}

		// Token: 0x06001831 RID: 6193 RVA: 0x000A4EBC File Offset: 0x000A30BC
		private void OnGameLoaded(CampaignGameStarter starter)
		{
			this.AssignPatrolZones();
			this.AdjustAssignedPatrolZones();
		}

		// Token: 0x06001832 RID: 6194 RVA: 0x000A4ECC File Offset: 0x000A30CC
		private void OnNewGameCreatedPartialFollowUp(CampaignGameStarter starter, int index)
		{
			if (index % 33 == 0)
			{
				foreach (Clan clan in Clan.BanditFactions)
				{
					this.DailyTickClan(clan);
				}
			}
		}

		// Token: 0x06001833 RID: 6195 RVA: 0x000A4F20 File Offset: 0x000A3120
		private void DailyTickClan(Clan clan)
		{
			if (this.IsPirateClan(clan))
			{
				this.TrySpawnPirateParties(clan);
			}
		}

		// Token: 0x06001834 RID: 6196 RVA: 0x000A4F32 File Offset: 0x000A3132
		private void DailyTickParty(MobileParty mobileParty)
		{
			if (this.IsPirateParty(mobileParty))
			{
				this.TryRemoveWeakPirate(mobileParty);
			}
		}

		// Token: 0x06001835 RID: 6197 RVA: 0x000A4F44 File Offset: 0x000A3144
		private void TryRemoveWeakPirate(MobileParty pirateParty)
		{
			if (pirateParty.HasNavalNavigationCapability && pirateParty.MapEvent == null)
			{
				bool flag;
				if ((float)pirateParty.MemberRoster.TotalHealthyCount >= (float)pirateParty.ActualClan.DefaultPartyTemplate.GetLowerTroopLimit() * 0.7f)
				{
					flag = (float)pirateParty.Ships.Count < (float)LinQuick.SumQ<ShipTemplateStack>(pirateParty.ActualClan.DefaultPartyTemplate.ShipHulls, (ShipTemplateStack t) => t.MinValue) * 0.7f;
				}
				else
				{
					flag = true;
				}
				float num = MobileParty.MainParty.SeeingRange * 2f;
				if (flag && pirateParty.Position.DistanceSquared(MobileParty.MainParty.Position) >= num * num)
				{
					DestroyPartyAction.ApplyForDisbanding(pirateParty, Settlement.FindFirst((Settlement t) => t.IsHideout));
				}
			}
		}

		// Token: 0x06001836 RID: 6198 RVA: 0x000A5032 File Offset: 0x000A3232
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<Dictionary<MobileParty, PiratesCampaignBehavior.PatrolZone>>("_assignedZones", ref this._assignedZones);
		}

		// Token: 0x06001837 RID: 6199 RVA: 0x000A5048 File Offset: 0x000A3248
		private void AssignPatrolZones(Clan clan)
		{
			this._patrolZones[clan] = new List<PiratesCampaignBehavior.PatrolZone>();
			foreach (ValueTuple<CampaignVec2, float> valueTuple in NavalDLCManager.Instance.NavalMapSceneWrapper.GetSpawnPoints(clan.StringId))
			{
				PiratesCampaignBehavior.PatrolZone patrolZone;
				if (!this.FindIdenticalZone(valueTuple.Item1, valueTuple.Item2, out patrolZone))
				{
					patrolZone = new PiratesCampaignBehavior.PatrolZone(valueTuple.Item1, valueTuple.Item2);
				}
				this._patrolZones[clan].Add(patrolZone);
			}
		}

		// Token: 0x06001838 RID: 6200 RVA: 0x000A50F0 File Offset: 0x000A32F0
		private bool FindIdenticalZone(CampaignVec2 position, float radius, out PiratesCampaignBehavior.PatrolZone zone)
		{
			zone = null;
			foreach (KeyValuePair<MobileParty, PiratesCampaignBehavior.PatrolZone> keyValuePair in this._assignedZones)
			{
				if (MBMath.ApproximatelyEqualsTo(keyValuePair.Value.Position.Distance(position), 0f, 1E-05f) && MBMath.ApproximatelyEqualsTo(keyValuePair.Value.Radius, radius, 1E-05f))
				{
					zone = keyValuePair.Value;
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001839 RID: 6201 RVA: 0x000A5190 File Offset: 0x000A3390
		private PiratesCampaignBehavior.PatrolZone GetClosestPatrolZone(MobileParty mobileParty)
		{
			List<PiratesCampaignBehavior.PatrolZone> list = this._patrolZones[mobileParty.ActualClan];
			PiratesCampaignBehavior.PatrolZone patrolZone = null;
			float num = float.MaxValue;
			foreach (PiratesCampaignBehavior.PatrolZone patrolZone2 in list)
			{
				float num2 = mobileParty.TargetPosition.Distance(patrolZone2.Position);
				if (num2 < num && this.CanSpawnPiratePartyInZone(mobileParty.ActualClan, patrolZone2))
				{
					num = num2;
					patrolZone = patrolZone2;
				}
			}
			return patrolZone;
		}

		// Token: 0x0600183A RID: 6202 RVA: 0x000A5220 File Offset: 0x000A3420
		private void AdjustAssignedPatrolZones()
		{
			foreach (MobileParty mobileParty in MobileParty.AllBanditParties)
			{
				if (this.IsPirateParty(mobileParty) && this.GetAssignedZone(mobileParty) == null)
				{
					PiratesCampaignBehavior.PatrolZone patrolZone = this.GetClosestPatrolZone(mobileParty);
					if (patrolZone == null)
					{
						Debug.FailedAssert("zone != null", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\CampaignBehaviors\\PiratesCampaignBehavior.cs", "AdjustAssignedPatrolZones", 387);
						List<PiratesCampaignBehavior.PatrolZone> patrolZones = this.GetPatrolZones(mobileParty.ActualClan);
						if (patrolZones.Count == 0)
						{
							patrolZone = new PiratesCampaignBehavior.PatrolZone(mobileParty.TargetPosition, 20f);
						}
						else
						{
							patrolZone = Extensions.GetRandomElement<PiratesCampaignBehavior.PatrolZone>(patrolZones);
						}
					}
					this.AssignPartyToZone(patrolZone, mobileParty);
					if (mobileParty.MapEvent == null)
					{
						mobileParty.SetMovePatrolAroundPoint(patrolZone.Position, 2);
					}
				}
			}
			foreach (KeyValuePair<MobileParty, PiratesCampaignBehavior.PatrolZone> keyValuePair in this._assignedZones)
			{
				PiratesCampaignBehavior.PatrolZone value = keyValuePair.Value;
				int density = value.Density;
				value.Density = density + 1;
			}
		}

		// Token: 0x0600183B RID: 6203 RVA: 0x000A5348 File Offset: 0x000A3548
		private bool IsPirateParty(MobileParty mobileParty)
		{
			return mobileParty.ActualClan != null && this.IsPirateClan(mobileParty.ActualClan) && !mobileParty.IsCurrentlyUsedByAQuest && mobileParty.HasNavalNavigationCapability && mobileParty.IsCurrentlyAtSea;
		}

		// Token: 0x0600183C RID: 6204 RVA: 0x000A5378 File Offset: 0x000A3578
		private List<PiratesCampaignBehavior.PatrolZone> GetPatrolZones(Clan clan)
		{
			return this._patrolZones[clan];
		}

		// Token: 0x0600183D RID: 6205 RVA: 0x000A5388 File Offset: 0x000A3588
		private List<PiratesCampaignBehavior.PatrolZone> GetAllPatrolZones()
		{
			List<PiratesCampaignBehavior.PatrolZone> list = new List<PiratesCampaignBehavior.PatrolZone>();
			foreach (KeyValuePair<Clan, List<PiratesCampaignBehavior.PatrolZone>> keyValuePair in this._patrolZones)
			{
				list.AddRange(keyValuePair.Value);
			}
			return list;
		}

		// Token: 0x0600183E RID: 6206 RVA: 0x000A53E8 File Offset: 0x000A35E8
		private PiratesCampaignBehavior.PatrolZone GetAssignedZone(MobileParty party)
		{
			PiratesCampaignBehavior.PatrolZone patrolZone;
			if (this._assignedZones.TryGetValue(party, out patrolZone))
			{
				return patrolZone;
			}
			return null;
		}

		// Token: 0x0600183F RID: 6207 RVA: 0x000A5408 File Offset: 0x000A3608
		private void AssignPartyToZone(PiratesCampaignBehavior.PatrolZone zone, MobileParty mobileParty)
		{
			PiratesCampaignBehavior.PatrolZone patrolZone;
			if (this._assignedZones.TryGetValue(mobileParty, out patrolZone))
			{
				this.UnassignPartyFromZone(patrolZone, mobileParty);
			}
			int density = zone.Density;
			zone.Density = density + 1;
			this._assignedZones[mobileParty] = zone;
		}

		// Token: 0x06001840 RID: 6208 RVA: 0x000A544C File Offset: 0x000A364C
		private void UnassignPartyFromZone(PiratesCampaignBehavior.PatrolZone assignedZone, MobileParty mobileParty)
		{
			int density = assignedZone.Density;
			assignedZone.Density = density - 1;
			this._assignedZones.Remove(mobileParty);
		}

		// Token: 0x06001841 RID: 6209 RVA: 0x000A5478 File Offset: 0x000A3678
		private void TrySpawnPirateParties(Clan clan)
		{
			int num;
			int num2;
			this.GetPirateData(clan, out num, out num2);
			int num3 = MathF.Floor(MathF.Pow((float)(num2 - num), 0.66f));
			if (num3 > 0)
			{
				int num4 = 0;
				List<PiratesCampaignBehavior.PatrolZone> patrolZones = this.GetPatrolZones(clan);
				Extensions.Shuffle<PiratesCampaignBehavior.PatrolZone>(patrolZones);
				int num5 = 0;
				int num6 = 0;
				for (int i = 0; i < num3; i++)
				{
					PiratesCampaignBehavior.PatrolZone randomSuitableZone = this.GetRandomSuitableZone(clan, patrolZones, ref num5, ref num6);
					if (randomSuitableZone == null)
					{
						break;
					}
					this.SpawnPirateParty(clan, randomSuitableZone);
					num4++;
				}
			}
		}

		// Token: 0x06001842 RID: 6210 RVA: 0x000A54F0 File Offset: 0x000A36F0
		private void GetPirateData(Clan clan, out int pirateMemberCount, out int maximumPirateCount)
		{
			pirateMemberCount = clan.WarPartyComponents.Count;
			maximumPirateCount = Campaign.Current.Models.BanditDensityModel.GetMaxSupportedNumberOfLootersForClan(clan);
		}

		// Token: 0x06001843 RID: 6211 RVA: 0x000A5518 File Offset: 0x000A3718
		private void SpawnPirateParty(Clan clan, PiratesCampaignBehavior.PatrolZone patrolZone)
		{
			Settlement settlement = SettlementHelper.FindNearestSettlementToPoint(ref patrolZone.Position, (Settlement x) => x.IsTown && x.HasPort);
			CampaignVec2 spawnPosition = this.GetSpawnPosition(patrolZone);
			MobileParty mobileParty = BanditPartyComponent.CreateLooterParty(clan.StringId + "_1", clan, settlement, false, clan.DefaultPartyTemplate, spawnPosition);
			this.InitializePirateParty(mobileParty, clan);
			this.AssignPartyToZone(patrolZone, mobileParty);
			mobileParty.SetMovePatrolAroundPoint(patrolZone.Position, 2);
		}

		// Token: 0x06001844 RID: 6212 RVA: 0x000A5598 File Offset: 0x000A3798
		private CampaignVec2 GetSpawnPosition(PiratesCampaignBehavior.PatrolZone zone)
		{
			if (Campaign.Current.GameStarted && this.IsPointVisibleToPlayer(zone))
			{
				float num = float.MaxValue;
				PiratesCampaignBehavior.PatrolZone patrolZone = null;
				foreach (PiratesCampaignBehavior.PatrolZone patrolZone2 in this.GetAllPatrolZones())
				{
					float zoneDistance = this.GetZoneDistance(zone, patrolZone2);
					if (!this.IsPointVisibleToPlayer(patrolZone2) && (zoneDistance < num || patrolZone == null))
					{
						patrolZone = patrolZone2;
						num = zoneDistance;
					}
				}
				if (patrolZone != null)
				{
					zone = patrolZone;
				}
			}
			int num2 = 0;
			CampaignVec2 campaignVec = NavigationHelper.FindPointAroundPosition(zone.Position, 2, zone.Radius, 0f, true, false);
			do
			{
				campaignVec = NavigationHelper.FindPointAroundPosition(zone.Position, 2, zone.Radius, 0f, true, false);
				num2++;
			}
			while (num2 < 100 && Campaign.Current.Models.BanditDensityModel.IsPositionInsideNavalSafeZone(campaignVec));
			return campaignVec;
		}

		// Token: 0x06001845 RID: 6213 RVA: 0x000A5684 File Offset: 0x000A3884
		private float GetZoneDistance(PiratesCampaignBehavior.PatrolZone p1, PiratesCampaignBehavior.PatrolZone p2)
		{
			int[] invalidTerrainTypesForNavigationType = Campaign.Current.Models.PartyNavigationModel.GetInvalidTerrainTypesForNavigationType(2);
			float num;
			Campaign.Current.MapSceneWrapper.GetPathDistanceBetweenAIFaces(p1.Position.Face, p2.Position.Face, p1.Position.ToVec2(), p2.Position.ToVec2(), 0.5f, (float)Campaign.PathFindingMaxCostLimit, ref num, invalidTerrainTypesForNavigationType, Campaign.Current.Models.MapDistanceModel.RegionSwitchCostFromLandToSea, Campaign.Current.Models.MapDistanceModel.RegionSwitchCostFromSeaToLand);
			return num;
		}

		// Token: 0x06001846 RID: 6214 RVA: 0x000A5728 File Offset: 0x000A3928
		private PiratesCampaignBehavior.PatrolZone GetRandomSuitableZone(Clan clan, List<PiratesCampaignBehavior.PatrolZone> zones, ref int iter, ref int bestScore)
		{
			int num = iter + zones.Count;
			int num2 = -1;
			PiratesCampaignBehavior.PatrolZone patrolZone = null;
			while (iter < num)
			{
				PiratesCampaignBehavior.PatrolZone patrolZone2 = zones[iter % zones.Count];
				iter++;
				if (this.CanSpawnPiratePartyInZone(clan, patrolZone2))
				{
					if (bestScore == patrolZone2.Density)
					{
						return patrolZone2;
					}
					if (num2 < patrolZone2.Density)
					{
						num2 = patrolZone2.Density;
						patrolZone = patrolZone2;
					}
				}
			}
			iter = 0;
			bestScore = num2;
			return patrolZone;
		}

		// Token: 0x06001847 RID: 6215 RVA: 0x000A5794 File Offset: 0x000A3994
		private bool CanSpawnPiratePartyInZone(Clan clan, PiratesCampaignBehavior.PatrolZone zone)
		{
			List<PiratesCampaignBehavior.PatrolZone> list;
			return this._patrolZones.TryGetValue(clan, out list) && list.Contains(zone);
		}

		// Token: 0x06001848 RID: 6216 RVA: 0x000A57BA File Offset: 0x000A39BA
		private bool IsPirateClan(Clan clan)
		{
			return !clan.Culture.CanHaveSettlement && clan.HasNavalNavigationCapability && clan.IsBanditFaction;
		}

		// Token: 0x06001849 RID: 6217 RVA: 0x000A57D9 File Offset: 0x000A39D9
		private void InitializePirateParty(MobileParty pirateParty, Clan faction)
		{
			pirateParty.Party.SetVisualAsDirty();
			pirateParty.ActualClan = faction;
			pirateParty.Aggressiveness = 1f - 0.2f * MBRandom.RandomFloat;
			PiratesCampaignBehavior.CreatePartyTrade(pirateParty);
			this.GiveFoodToBanditParty(pirateParty);
			pirateParty.SetLandNavigationAccess(false);
		}

		// Token: 0x0600184A RID: 6218 RVA: 0x000A5818 File Offset: 0x000A3A18
		private bool IsPointVisibleToPlayer(PiratesCampaignBehavior.PatrolZone zone)
		{
			return MobileParty.MainParty.Position.DistanceSquared(zone.Position) < (MobileParty.MainParty.SeeingRange + zone.Radius) * (MobileParty.MainParty.SeeingRange + zone.Radius);
		}

		// Token: 0x0600184B RID: 6219 RVA: 0x000A5864 File Offset: 0x000A3A64
		private static void CreatePartyTrade(MobileParty banditParty)
		{
			int num = (int)(10f * (float)banditParty.Party.MemberRoster.TotalManCount * (0.5f + 1f * MBRandom.RandomFloat));
			banditParty.InitializePartyTrade(num);
		}

		// Token: 0x0600184C RID: 6220 RVA: 0x000A58A4 File Offset: 0x000A3AA4
		private void GiveFoodToBanditParty(MobileParty banditParty)
		{
			foreach (ItemObject itemObject in Items.All)
			{
				if (itemObject.IsFood)
				{
					int num = MBRandom.RoundRandomized((float)banditParty.MemberRoster.TotalManCount * (1f / (float)itemObject.Value) * 8f * MBRandom.RandomFloat * MBRandom.RandomFloat * MBRandom.RandomFloat * MBRandom.RandomFloat);
					if (num > 0)
					{
						banditParty.ItemRoster.AddToCounts(itemObject, num);
					}
				}
			}
		}

		// Token: 0x0600184D RID: 6221 RVA: 0x000A5948 File Offset: 0x000A3B48
		public float GetPatrolRadius(MobileParty mobileParty)
		{
			PiratesCampaignBehavior.PatrolZone patrolZone;
			if (this._assignedZones.TryGetValue(mobileParty, out patrolZone))
			{
				return patrolZone.Radius;
			}
			return 25f;
		}

		// Token: 0x04000BF3 RID: 3059
		private const float PirateStartGoldPerBandit = 10f;

		// Token: 0x04000BF4 RID: 3060
		private const float PatrollingScore = 5f;

		// Token: 0x04000BF5 RID: 3061
		private const float DefaultPatrolRadius = 20f;

		// Token: 0x04000BF6 RID: 3062
		private const float WeakPirateRemovalStrengthThreshold = 0.7f;

		// Token: 0x04000BF7 RID: 3063
		private Dictionary<Clan, List<PiratesCampaignBehavior.PatrolZone>> _patrolZones = new Dictionary<Clan, List<PiratesCampaignBehavior.PatrolZone>>();

		// Token: 0x04000BF8 RID: 3064
		private Dictionary<MobileParty, PiratesCampaignBehavior.PatrolZone> _assignedZones = new Dictionary<MobileParty, PiratesCampaignBehavior.PatrolZone>();

		// Token: 0x0200029E RID: 670
		private class PatrolZone
		{
			// Token: 0x17000467 RID: 1127
			// (get) Token: 0x06001CE2 RID: 7394 RVA: 0x000BA053 File Offset: 0x000B8253
			// (set) Token: 0x06001CE3 RID: 7395 RVA: 0x000BA05B File Offset: 0x000B825B
			public int Density { get; set; }

			// Token: 0x06001CE4 RID: 7396 RVA: 0x000BA064 File Offset: 0x000B8264
			public PatrolZone(CampaignVec2 position, float radius)
			{
				this.Position = position;
				this.Radius = radius;
				this.Density = 0;
			}

			// Token: 0x0400113C RID: 4412
			[SaveableField(0)]
			public readonly CampaignVec2 Position;

			// Token: 0x0400113D RID: 4413
			[SaveableField(1)]
			public readonly float Radius;
		}

		// Token: 0x0200029F RID: 671
		private class PiratesCampaignBehaviorSaveDefiner : SaveableTypeDefiner
		{
			// Token: 0x06001CE5 RID: 7397 RVA: 0x000BA081 File Offset: 0x000B8281
			public PiratesCampaignBehaviorSaveDefiner()
				: base(2277221)
			{
			}

			// Token: 0x06001CE6 RID: 7398 RVA: 0x000BA08E File Offset: 0x000B828E
			protected override void DefineClassTypes()
			{
				base.AddClassDefinition(typeof(PiratesCampaignBehavior.PatrolZone), 1, null);
			}

			// Token: 0x06001CE7 RID: 7399 RVA: 0x000BA0A2 File Offset: 0x000B82A2
			protected override void DefineContainerDefinitions()
			{
				base.ConstructContainerDefinition(typeof(Dictionary<MobileParty, PiratesCampaignBehavior.PatrolZone>));
			}
		}
	}
}
