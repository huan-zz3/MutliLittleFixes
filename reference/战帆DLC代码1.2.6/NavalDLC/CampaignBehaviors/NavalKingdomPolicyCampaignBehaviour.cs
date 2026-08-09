using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.BarterSystem.Barterables;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace NavalDLC.CampaignBehaviors
{
	// Token: 0x0200016A RID: 362
	public class NavalKingdomPolicyCampaignBehaviour : CampaignBehaviorBase
	{
		// Token: 0x060017D2 RID: 6098 RVA: 0x000A2920 File Offset: 0x000A0B20
		public override void RegisterEvents()
		{
			CampaignEvents.OnShipOwnerChangedEvent.AddNonSerializedListener(this, new Action<Ship, PartyBase, ChangeShipOwnerAction.ShipOwnerChangeDetail>(this.OnShipOwnerChanged));
			CampaignEvents.RaidCompletedEvent.AddNonSerializedListener(this, new Action<BattleSideEnum, RaidEventComponent>(this.OnRaidCompleted));
			CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.OnDailyTick));
			CampaignEvents.HourlyTickPartyEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.OnHourlyTickParty));
			CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(this, new Action<MobileParty, PartyBase>(this.OnMobilePartyDestroyed));
			CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, new Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.OnSettlementOwnerChanged));
			CampaignEvents.OnBarterAcceptedEvent.AddNonSerializedListener(this, new Action<Hero, Hero, List<Barterable>>(this.OnBarterAccepted));
		}

		// Token: 0x060017D3 RID: 6099 RVA: 0x000A29D0 File Offset: 0x000A0BD0
		private void OnBarterAccepted(Hero offererHero, Hero otherHero, List<Barterable> barters)
		{
			Clan clan = offererHero.Clan;
			Kingdom kingdom = ((clan != null) ? clan.Kingdom : null);
			if (kingdom != null && kingdom.RulingClan != clan && kingdom.HasPolicy(NavalPolicies.RoyalRansomClaim))
			{
				IEnumerable<SetPrisonerFreeBarterable> enumerable = barters.Where<Barterable>((Barterable x) => x is SetPrisonerFreeBarterable).Cast<SetPrisonerFreeBarterable>();
				float num = 0f;
				foreach (SetPrisonerFreeBarterable setPrisonerFreeBarterable in enumerable)
				{
					num = (float)setPrisonerFreeBarterable.GetUnitValueForFaction(otherHero.MapFaction);
				}
				int num2 = MathF.Round(num * 0.15f);
				GiveGoldAction.ApplyBetweenCharacters(offererHero, kingdom.Leader, num2, false);
			}
		}

		// Token: 0x060017D4 RID: 6100 RVA: 0x000A2AA0 File Offset: 0x000A0CA0
		private void OnDailyTick()
		{
			foreach (Kingdom kingdom in Kingdom.All)
			{
				if (kingdom.HasPolicy(NavalPolicies.KingsPardonForPirates))
				{
					foreach (Settlement settlement in kingdom.Settlements)
					{
						if (MBRandom.RandomFloat <= 0.05f && settlement.IsTown && settlement.HasPort)
						{
							MobileParty availableNearbyPirateParty = this.GetAvailableNearbyPirateParty(settlement);
							if (availableNearbyPirateParty != null)
							{
								availableNearbyPirateParty.SetMoveGoToPoint(settlement.PortPosition, 2);
								availableNearbyPirateParty.Ai.SetDoNotMakeNewDecisions(true);
								this._settlementToSurrenderByParty.Add(availableNearbyPirateParty, settlement);
							}
						}
					}
				}
			}
		}

		// Token: 0x060017D5 RID: 6101 RVA: 0x000A2B90 File Offset: 0x000A0D90
		private void OnHourlyTickParty(MobileParty party)
		{
			Settlement settlement;
			float num;
			if (this._settlementToSurrenderByParty.TryGetValue(party, out settlement) && Campaign.Current.Models.MapDistanceModel.GetDistance(party, settlement, true, 2, ref num) <= 5f)
			{
				MobileParty garrisonParty = settlement.Town.GarrisonParty;
				if (garrisonParty != null)
				{
					foreach (TroopRosterElement troopRosterElement in party.MemberRoster.GetTroopRoster())
					{
						int num2 = Math.Min(garrisonParty.GetAvailableWageBudget() / Campaign.Current.Models.PartyWageModel.GetCharacterWage(troopRosterElement.Character), MathF.Round((float)troopRosterElement.Number * 0.25f));
						if (num2 > 0)
						{
							garrisonParty.MemberRoster.AddToCounts(troopRosterElement.Character, num2, false, 0, 0, true, -1);
						}
					}
				}
				settlement.Town.Security -= 5f;
				for (int i = party.Ships.Count - 1; i >= 0; i--)
				{
					ChangeShipOwnerAction.ApplyByTransferring(settlement.Party, party.Ships[i]);
				}
				if (party.MapEvent != null)
				{
					party.MapEventSide = null;
				}
				DestroyPartyAction.Apply(null, party);
			}
		}

		// Token: 0x060017D6 RID: 6102 RVA: 0x000A2CE8 File Offset: 0x000A0EE8
		private void OnMobilePartyDestroyed(MobileParty mobileParty, PartyBase destroyerParty)
		{
			this._settlementToSurrenderByParty.Remove(mobileParty);
		}

		// Token: 0x060017D7 RID: 6103 RVA: 0x000A2CF8 File Offset: 0x000A0EF8
		private void OnSettlementOwnerChanged(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturerHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
		{
			KeyValuePair<MobileParty, Settlement> keyValuePair = this._settlementToSurrenderByParty.FirstOrDefault<KeyValuePair<MobileParty, Settlement>>((KeyValuePair<MobileParty, Settlement> x) => x.Value == settlement);
			Clan clan = newOwner.Clan;
			Kingdom kingdom = ((clan != null) ? clan.Kingdom : null);
			MobileParty key = keyValuePair.Key;
			if (key != null && (kingdom == null || !kingdom.HasPolicy(NavalPolicies.KingsPardonForPirates)))
			{
				if (key.MapEvent != null)
				{
					key.MapEventSide = null;
				}
				DestroyPartyAction.Apply(null, key);
			}
		}

		// Token: 0x060017D8 RID: 6104 RVA: 0x000A2D70 File Offset: 0x000A0F70
		private void OnRaidCompleted(BattleSideEnum winnerSide, RaidEventComponent raidEvent)
		{
			if (winnerSide == 1)
			{
				Hero owner = raidEvent.AttackerSide.LeaderParty.Owner;
				Clan clan = ((owner != null) ? owner.Clan : null);
				Kingdom kingdom = ((clan != null) ? clan.Kingdom : null);
				if (kingdom != null && kingdom.HasPolicy(NavalPolicies.RaidersSpoils))
				{
					foreach (MapEventParty mapEventParty in raidEvent.AttackerSide.Parties)
					{
						GainKingdomInfluenceAction.ApplyForDefault(mapEventParty.Party.LeaderHero, 5f);
					}
				}
			}
		}

		// Token: 0x060017D9 RID: 6105 RVA: 0x000A2E14 File Offset: 0x000A1014
		private void OnShipOwnerChanged(Ship ship, PartyBase oldOwner, ChangeShipOwnerAction.ShipOwnerChangeDetail details)
		{
			if (details == null && oldOwner.IsSettlement)
			{
				Clan ownerClan = oldOwner.Settlement.OwnerClan;
				Kingdom kingdom = ((ownerClan != null) ? ownerClan.Kingdom : null);
				if (kingdom != null && kingdom.RulingClan != ownerClan && kingdom.HasPolicy(NavalPolicies.KingsTitheOnKeels))
				{
					int num = MathF.Round(Campaign.Current.Models.ShipCostModel.GetShipTradeValue(ship, oldOwner, ship.Owner) * 0.15f);
					GiveGoldAction.ApplyForPartyToCharacter(oldOwner, kingdom.Leader, num, false);
				}
			}
		}

		// Token: 0x060017DA RID: 6106 RVA: 0x000A2E94 File Offset: 0x000A1094
		private MobileParty GetAvailableNearbyPirateParty(Settlement settlement)
		{
			LocatableSearchData<MobileParty> locatableSearchData = MobileParty.StartFindingLocatablesAroundPosition(settlement.PortPosition.ToVec2(), 50f);
			for (MobileParty mobileParty = MobileParty.FindNextLocatable(ref locatableSearchData); mobileParty != null; mobileParty = MobileParty.FindNextLocatable(ref locatableSearchData))
			{
				if (mobileParty.IsBandit && mobileParty.MapEvent == null && mobileParty.HasNavalNavigationCapability && !this._settlementToSurrenderByParty.ContainsKey(mobileParty))
				{
					return mobileParty;
				}
			}
			return null;
		}

		// Token: 0x060017DB RID: 6107 RVA: 0x000A2EF8 File Offset: 0x000A10F8
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<Dictionary<MobileParty, Settlement>>("_settlementsToSurrenderByParties", ref this._settlementToSurrenderByParty);
		}

		// Token: 0x04000BE8 RID: 3048
		private const float KingsPardonForPiratesSearchRadius = 50f;

		// Token: 0x04000BE9 RID: 3049
		private const float KingsPardonForPiratesArriveDistance = 5f;

		// Token: 0x04000BEA RID: 3050
		private const float KingsPardonDailyCheckChance = 0.05f;

		// Token: 0x04000BEB RID: 3051
		private const float KingsPardonRecruitPercentage = 0.25f;

		// Token: 0x04000BEC RID: 3052
		private Dictionary<MobileParty, Settlement> _settlementToSurrenderByParty = new Dictionary<MobileParty, Settlement>();
	}
}
