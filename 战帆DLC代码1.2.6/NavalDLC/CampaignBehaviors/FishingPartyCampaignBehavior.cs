using System;
using System.Collections.Generic;
using Helpers;
using NavalDLC.CharacterDevelopment;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace NavalDLC.CampaignBehaviors
{
	// Token: 0x02000160 RID: 352
	public class FishingPartyCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x060016E6 RID: 5862 RVA: 0x0009BFBF File Offset: 0x0009A1BF
		private bool CanHaveFishingParties(Village village)
		{
			return village.VillageType == DefaultVillageTypes.Fisherman && village.Settlement.Culture.FishingPartyTemplate != null && village.Settlement.HasPort;
		}

		// Token: 0x060016E7 RID: 5863 RVA: 0x0009BFF0 File Offset: 0x0009A1F0
		private bool CanCreateFishingParties(Village village)
		{
			bool flag = false;
			if (village.VillageState == null && village.Settlement.Party.MapEvent == null && village.Hearth > (float)Campaign.Current.Models.PartySizeLimitModel.MinimumNumberOfVillagersAtVillagerParty && this.CanHaveFishingParties(village) && this.GetIdealFishingPartyCount(village) > village.FishingParties().Count)
			{
				int num = 0;
				for (int i = 0; i < village.Owner.ItemRoster.Count; i++)
				{
					num += village.Owner.ItemRoster[i].Amount;
				}
				flag = num < village.GetWarehouseCapacity();
			}
			return flag;
		}

		// Token: 0x060016E8 RID: 5864 RVA: 0x0009C09C File Offset: 0x0009A29C
		private float EndingRoamingChance(FishingPartyComponent fishingParty)
		{
			if (fishingParty.MobileParty.TotalWeightCarried >= (float)fishingParty.MobileParty.InventoryCapacity)
			{
				return 1f;
			}
			float num = 9f;
			float elapsedHoursUntilNow = fishingParty.RoamingStartTime.ElapsedHoursUntilNow;
			if (elapsedHoursUntilNow < 1f * (float)CampaignTime.HoursInDay - num * 0.5f)
			{
				return 0f;
			}
			if (elapsedHoursUntilNow > 3f * (float)CampaignTime.HoursInDay)
			{
				return 1f;
			}
			int num2 = MathF.Round((3f * (float)CampaignTime.HoursInDay - elapsedHoursUntilNow) / num);
			return 1f / (float)(num2 + 1);
		}

		// Token: 0x060016E9 RID: 5865 RVA: 0x0009C130 File Offset: 0x0009A330
		private float EndingFishingChance(FishingPartyComponent fishingParty)
		{
			if (fishingParty.MobileParty.TotalWeightCarried >= (float)fishingParty.MobileParty.InventoryCapacity)
			{
				return 1f;
			}
			float elapsedHoursUntilNow = fishingParty.FishingWaitStartTime.ElapsedHoursUntilNow;
			if (elapsedHoursUntilNow < 7.5f)
			{
				return 0f;
			}
			if (elapsedHoursUntilNow > 10f)
			{
				return 1f;
			}
			int num = MathF.Round(10f - elapsedHoursUntilNow);
			return 1f / (float)(num + 1);
		}

		// Token: 0x060016EA RID: 5866 RVA: 0x0009C19E File Offset: 0x0009A39E
		private int GetIdealFishingPartySize(Village village)
		{
			return Campaign.Current.Models.PartySizeLimitModel.GetIdealVillagerPartySize(village);
		}

		// Token: 0x060016EB RID: 5867 RVA: 0x0009C1B8 File Offset: 0x0009A3B8
		private int GetIdealFishingPartyCount(Village village)
		{
			int num = Math.Min(2, village.GetHearthLevel() + 1);
			Hero governor = village.Bound.Town.Governor;
			if (governor != null && governor.GetPerkValue(NavalPerks.Shipmaster.TheCorsairsEdge))
			{
				num += MathF.Round(NavalPerks.Shipmaster.TheCorsairsEdge.SecondaryBonus);
			}
			return num;
		}

		// Token: 0x060016EC RID: 5868 RVA: 0x0009C20C File Offset: 0x0009A40C
		public override void RegisterEvents()
		{
			CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnNewGameCreated));
			CampaignEvents.HourlyTickPartyEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.OnHourlyTickParty));
			CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener(this, new Action<Settlement>(this.OnDailySettlementTick));
			CampaignEvents.OnGameEarlyLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnGameEarlyLoaded));
		}

		// Token: 0x060016ED RID: 5869 RVA: 0x0009C278 File Offset: 0x0009A478
		private void InitializeCachedData()
		{
			this.Fish = MBObjectManager.Instance.GetObject<ItemObject>("fish");
			MBList<int> mblist = Extensions.ToMBList<int>(Campaign.Current.Models.PartyNavigationModel.GetInvalidTerrainTypesForNavigationType(2));
			mblist.Add(11);
			mblist.Add(22);
			mblist.Add(25);
			this._invalidFishingTerrainTypes = mblist.ToArray();
		}

		// Token: 0x060016EE RID: 5870 RVA: 0x0009C2DC File Offset: 0x0009A4DC
		private void OnGameEarlyLoaded(CampaignGameStarter starter)
		{
			this.InitializeCachedData();
			if (MBSaveLoad.IsUpdatingGameVersion && MBSaveLoad.LastLoadedGameVersion.IsOlderThan(ApplicationVersion.FromString("v1.3.9.103870", 0)))
			{
				foreach (MobileParty mobileParty in MobileParty.AllVillagerParties)
				{
					if (mobileParty.IsFishingParty())
					{
						int itemNumber = mobileParty.ItemRoster.GetItemNumber(this.Fish);
						int num = (int)((float)mobileParty.InventoryCapacity / (this.Fish.Weight + 5f));
						if (itemNumber > num)
						{
							mobileParty.ItemRoster.AddToCounts(this.Fish, num - itemNumber);
						}
					}
				}
			}
		}

		// Token: 0x060016EF RID: 5871 RVA: 0x0009C3A4 File Offset: 0x0009A5A4
		private void OnDailySettlementTick(Settlement settlement)
		{
			Village village = settlement.Village;
			if (village != null && MBRandom.RandomFloat > 0.5f && this.CanCreateFishingParties(village))
			{
				MobileParty mobileParty = FishingPartyComponent.CreateFishingParty(settlement.OwnerClan.StringId + "_1", village);
				village.Hearth = MathF.Max(0f, village.Hearth - (float)((mobileParty.MemberRoster.TotalManCount + 1) / 2));
				this.StartRoaming(mobileParty.PartyComponent as FishingPartyComponent);
			}
		}

		// Token: 0x060016F0 RID: 5872 RVA: 0x0009C424 File Offset: 0x0009A624
		private void OnNewGameCreated(CampaignGameStarter starter)
		{
			this.InitializeCachedData();
			foreach (Village village in Village.All)
			{
				if (this.CanHaveFishingParties(village))
				{
					this.OnDailySettlementTick(village.Settlement);
				}
			}
		}

		// Token: 0x060016F1 RID: 5873 RVA: 0x0009C48C File Offset: 0x0009A68C
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x060016F2 RID: 5874 RVA: 0x0009C490 File Offset: 0x0009A690
		private void TryReinforceParty(FishingPartyComponent fishingParty)
		{
			if (fishingParty.HomeSettlement.Party.MapEvent == null && fishingParty.Village.VillageState == null)
			{
				int num = this.GetIdealFishingPartySize(fishingParty.Village) - fishingParty.MobileParty.MemberRoster.TotalManCount;
				if (num > (int)fishingParty.Village.Hearth)
				{
					num = (int)fishingParty.Village.Hearth;
				}
				if (num > 0)
				{
					if (num > (int)fishingParty.HomeSettlement.Village.Hearth)
					{
						num = (int)fishingParty.HomeSettlement.Village.Hearth;
					}
					fishingParty.HomeSettlement.Village.Hearth -= (float)((num + 1) / 2);
					CharacterObject character = Extensions.GetRandomElement<PartyTemplateStack>(fishingParty.HomeSettlement.Culture.FishingPartyTemplate.Stacks).Character;
					fishingParty.MobileParty.MemberRoster.AddToCounts(character, num, false, 0, 0, true, -1);
				}
				foreach (Ship ship in fishingParty.MobileParty.Ships)
				{
					if (ship.HitPoints < ship.MaxHitPoints)
					{
						RepairShipAction.ApplyForFree(ship);
					}
				}
			}
		}

		// Token: 0x060016F3 RID: 5875 RVA: 0x0009C5D4 File Offset: 0x0009A7D4
		private void CatchFish(FishingPartyComponent fishingParty)
		{
			MobileParty mobileParty = fishingParty.MobileParty;
			float num = 1f * MBRandom.RandomFloat;
			Hero governor = fishingParty.Village.Bound.Town.Governor;
			if (governor != null && governor.GetPerkValue(NavalPerks.Shipmaster.MasterAngler))
			{
				num += num * NavalPerks.Shipmaster.MasterAngler.SecondaryBonus;
			}
			int num2 = MBRandom.RoundRandomized(num);
			int num3 = (int)(((float)mobileParty.InventoryCapacity - mobileParty.TotalWeightCarried) / (this.Fish.Weight + 0.1f));
			if (num3 < num2)
			{
				num2 = num3;
			}
			if (num2 > 0)
			{
				mobileParty.ItemRoster.AddToCounts(this.Fish, num2);
			}
		}

		// Token: 0x060016F4 RID: 5876 RVA: 0x0009C674 File Offset: 0x0009A874
		private void OnHourlyTickParty(MobileParty party)
		{
			FishingPartyComponent fishingPartyComponent;
			if (party.MapEvent == null && (fishingPartyComponent = party.PartyComponent as FishingPartyComponent) != null)
			{
				if (party.ShortTermBehavior != party.DefaultBehavior)
				{
					fishingPartyComponent.IsFishing = false;
					if (party.IsFleeing() && party.ShortTermTargetParty != null && party.ShortTermTargetParty.Position.DistanceSquared(party.TargetPosition) < 16f)
					{
						this.StartRoaming(fishingPartyComponent);
						return;
					}
				}
				else if (party.DefaultBehavior == null)
				{
					if (!fishingPartyComponent.IsRoaming)
					{
						this.StartDropOff(fishingPartyComponent);
						return;
					}
					if (fishingPartyComponent.IsFishing)
					{
						this.CatchFish(fishingPartyComponent);
						if (MBRandom.RandomFloat < this.EndingFishingChance(fishingPartyComponent))
						{
							if (MBRandom.RandomFloat < this.EndingRoamingChance(fishingPartyComponent))
							{
								this.StartDropOff(fishingPartyComponent);
								return;
							}
							this.GoToNewFishingPoint(fishingPartyComponent);
							return;
						}
					}
					else
					{
						if (MBRandom.RandomFloat < this.EndingRoamingChance(fishingPartyComponent) && fishingPartyComponent.MobileParty.ItemRoster.Count > 5)
						{
							this.StartDropOff(fishingPartyComponent);
							return;
						}
						this.GoToNewFishingPoint(fishingPartyComponent);
						return;
					}
				}
				else if (party.DefaultBehavior == 9 && (party.Position - party.TargetPosition).LengthSquared < 0.01f)
				{
					party.SetMoveModeHold();
					if ((party.Position - fishingPartyComponent.Village.Settlement.PortPosition).LengthSquared < 0.01f)
					{
						this.OnDropOff(fishingPartyComponent);
						if (fishingPartyComponent.Village.FishingParties().Count > this.GetIdealFishingPartyCount(fishingPartyComponent.Village) && !party.IsVisible)
						{
							DestroyPartyAction.Apply(null, party);
							return;
						}
						this.StartRoaming(fishingPartyComponent);
						return;
					}
					else
					{
						if (!fishingPartyComponent.IsRoaming)
						{
							Debug.FailedAssert("fishing ship not roaming nor dropping off", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\CampaignBehaviors\\FishingPartyCampaignBehavior.cs", "OnHourlyTickParty", 440);
							this.StartDropOff(fishingPartyComponent);
							return;
						}
						fishingPartyComponent.IsFishing = true;
					}
				}
			}
		}

		// Token: 0x060016F5 RID: 5877 RVA: 0x0009C848 File Offset: 0x0009AA48
		private void OnDropOff(FishingPartyComponent fishingParty)
		{
			this.TryReinforceParty(fishingParty);
			fishingParty.Village.Settlement.ItemRoster.Add(fishingParty.MobileParty.ItemRoster);
			fishingParty.MobileParty.ItemRoster.Clear();
			Town town = fishingParty.Village.Bound.Town;
			Hero governor = town.Governor;
			if (governor != null && governor.GetPerkValue(NavalPerks.Shipmaster.TheHelmsmansShield))
			{
				town.Prosperity += NavalPerks.Shipmaster.TheHelmsmansShield.SecondaryBonus;
			}
			Hero governor2 = town.Governor;
			if (governor2 != null && governor2.GetPerkValue(NavalPerks.Shipmaster.RavenEye))
			{
				town.Loyalty += NavalPerks.Shipmaster.RavenEye.SecondaryBonus;
			}
		}

		// Token: 0x060016F6 RID: 5878 RVA: 0x0009C8FD File Offset: 0x0009AAFD
		private void StartRoaming(FishingPartyComponent fishingParty)
		{
			fishingParty.IsRoaming = true;
			fishingParty.IsFishing = false;
			this.GoToNewFishingPoint(fishingParty);
		}

		// Token: 0x060016F7 RID: 5879 RVA: 0x0009C914 File Offset: 0x0009AB14
		private void StartDropOff(FishingPartyComponent fishingParty)
		{
			fishingParty.IsFishing = false;
			fishingParty.IsRoaming = false;
			CampaignVec2 portPosition = fishingParty.Village.Settlement.PortPosition;
			fishingParty.MobileParty.SetMoveGoToPoint(portPosition, 2);
		}

		// Token: 0x060016F8 RID: 5880 RVA: 0x0009C950 File Offset: 0x0009AB50
		private void GoToNewFishingPoint(FishingPartyComponent fishingParty)
		{
			fishingParty.IsFishing = false;
			CampaignVec2 campaignVec = fishingParty.Village.Settlement.PortPosition;
			int num = 20;
			do
			{
				campaignVec = NavigationHelper.FindReachablePointAroundPosition(fishingParty.Village.Settlement.PortPosition, this._invalidFishingTerrainTypes, 36f, 12f, true);
				num--;
			}
			while (num > 0 && campaignVec.Distance(fishingParty.MobileParty.Position) < 12f);
			fishingParty.MobileParty.SetMoveGoToPoint(campaignVec, 2);
		}

		// Token: 0x060016F9 RID: 5881 RVA: 0x0009C9CD File Offset: 0x0009ABCD
		[CommandLineFunctionality.CommandLineArgumentFunction("show_drop_off", "campaign")]
		public static string show_drop_off(List<string> strings)
		{
			return Extensions.MinBy<Village, float>(Village.All, (Village v) => v.Settlement.Position.DistanceSquared(MobileParty.MainParty.Position)).Name.ToString();
		}

		// Token: 0x04000BC0 RID: 3008
		private ItemObject Fish;

		// Token: 0x04000BC1 RID: 3009
		private int[] _invalidFishingTerrainTypes;

		// Token: 0x04000BC2 RID: 3010
		private const float FishingZoneThreatClosenessDistanceSquared = 16f;

		// Token: 0x04000BC3 RID: 3011
		private const float MinDistanceForInteractionSquared = 0.01f;

		// Token: 0x04000BC4 RID: 3012
		private const int MinFishCountToDropOff = 5;

		// Token: 0x04000BC5 RID: 3013
		private const float MinFishingTimeInHours = 8f;

		// Token: 0x04000BC6 RID: 3014
		private const float MaxFishingTimeInHours = 10f;

		// Token: 0x04000BC7 RID: 3015
		private const float MinRoamingTimeInDays = 1f;

		// Token: 0x04000BC8 RID: 3016
		private const float MaxRoamingTimeInDays = 3f;

		// Token: 0x04000BC9 RID: 3017
		private const float MaxFishToCatchPerHour = 1f;

		// Token: 0x04000BCA RID: 3018
		private const float MaxDistanceBetweenPointsInFishingSpots = 36f;

		// Token: 0x04000BCB RID: 3019
		private const float MinDistanceBetweenPointsInFishingSpots = 12f;
	}
}
