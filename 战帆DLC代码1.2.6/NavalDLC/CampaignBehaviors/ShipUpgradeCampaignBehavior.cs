using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace NavalDLC.CampaignBehaviors
{
	// Token: 0x02000179 RID: 377
	public class ShipUpgradeCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x060018C3 RID: 6339 RVA: 0x000AC06C File Offset: 0x000AA26C
		public override void RegisterEvents()
		{
			CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
			CampaignEvents.DailyTickPartyEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.DailyTickPartyEvent));
			CampaignEvents.OnNewGameCreatedPartialFollowUpEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter, int>(this.OnNewGameCreatedPartialFollowUp));
		}

		// Token: 0x060018C4 RID: 6340 RVA: 0x000AC0C0 File Offset: 0x000AA2C0
		private void OnSettlementEntered(MobileParty mobileParty, Settlement settlement, Hero hero)
		{
			if (mobileParty != null && mobileParty.IsCaravan && settlement.HasPort && settlement.IsTown && this.CanPartyUpgradeShips(mobileParty) && settlement.Town.GetShipyard().CurrentLevel > 0)
			{
				List<ShipUpgradePiece> availableShipUpgradePieces = settlement.Town.GetAvailableShipUpgradePieces();
				foreach (Ship ship in mobileParty.Ships)
				{
					if (MBRandom.RandomFloat < 0.4f)
					{
						KeyValuePair<string, ShipSlot> randomSlot = Extensions.GetRandomElementInefficiently<KeyValuePair<string, ShipSlot>>(ship.ShipHull.AvailableSlots);
						ShipUpgradePiece randomElementWithPredicate = Extensions.GetRandomElementWithPredicate<ShipUpgradePiece>(availableShipUpgradePieces, (ShipUpgradePiece x) => x.DoesPieceMatchSlot(randomSlot.Value));
						if (randomElementWithPredicate != null)
						{
							int shipUpgradePieceCost = Campaign.Current.Models.ShipCostModel.GetShipUpgradePieceCost(ship, randomElementWithPredicate, ship.Owner);
							if ((float)mobileParty.PartyTradeGold * 0.2f > (float)shipUpgradePieceCost)
							{
								this.UpgradeShip(ship, randomSlot.Key, randomElementWithPredicate);
								GiveGoldAction.ApplyForPartyToSettlement(mobileParty.Party, settlement, shipUpgradePieceCost, false);
							}
						}
					}
				}
			}
		}

		// Token: 0x060018C5 RID: 6341 RVA: 0x000AC200 File Offset: 0x000AA400
		private float GetChanceToUpgradeShipForLord(Hero hero)
		{
			float num = (float)(hero.Clan.Tier + 1 - Campaign.Current.Models.ClanTierModel.MinClanTier) / (float)(1 + Campaign.Current.Models.ClanTierModel.MaxClanTier - Campaign.Current.Models.ClanTierModel.MinClanTier);
			float num2 = (hero.IsKingdomLeader ? 0.6f : (hero.IsClanLeader ? 0.4f : 0.2f));
			return num * num2;
		}

		// Token: 0x060018C6 RID: 6342 RVA: 0x000AC284 File Offset: 0x000AA484
		private void OnNewGameCreatedPartialFollowUp(CampaignGameStarter starter, int index)
		{
			if (index % 2 == 0)
			{
				foreach (MobileParty mobileParty in MobileParty.All)
				{
					this.DailyTickPartyEvent(mobileParty);
				}
			}
		}

		// Token: 0x060018C7 RID: 6343 RVA: 0x000AC2DC File Offset: 0x000AA4DC
		private void DailyTickPartyEvent(MobileParty party)
		{
			if (party.LeaderHero != null && !party.IsCurrentlyAtSea && this.CanPartyUpgradeShips(party))
			{
				float chanceToUpgradeShipForLord = this.GetChanceToUpgradeShipForLord(party.LeaderHero);
				foreach (Ship ship in party.Ships)
				{
					if (MBRandom.RandomFloat < chanceToUpgradeShipForLord)
					{
						KeyValuePair<string, ShipSlot> randomElementInefficiently = Extensions.GetRandomElementInefficiently<KeyValuePair<string, ShipSlot>>(ship.ShipHull.AvailableSlots);
						ShipUpgradePiece pieceAtSlot = ship.GetPieceAtSlot(randomElementInefficiently.Key);
						int upgradePieceLevelToLook = ((pieceAtSlot == null) ? 1 : (pieceAtSlot.RequiredPortLevel + 1));
						ShipUpgradePiece randomElementWithPredicate = Extensions.GetRandomElementWithPredicate<ShipUpgradePiece>(randomElementInefficiently.Value.MatchingPieces, (ShipUpgradePiece x) => !x.NotMerchandise && x.RequiredPortLevel == upgradePieceLevelToLook);
						if (randomElementWithPredicate != null)
						{
							this.UpgradeShip(ship, randomElementInefficiently.Key, randomElementWithPredicate);
						}
					}
				}
			}
		}

		// Token: 0x060018C8 RID: 6344 RVA: 0x000AC3D8 File Offset: 0x000AA5D8
		private void UpgradeShip(Ship ship, string slotId, ShipUpgradePiece upgradePiece)
		{
			ShipUpgradePiece pieceAtSlot = ship.GetPieceAtSlot(slotId);
			ShipSlot shipSlot = ship.ShipHull.AvailableSlots[slotId];
			if (pieceAtSlot == null || pieceAtSlot.RequiredPortLevel != 3)
			{
				ship.EquipUpgradePiece(slotId, upgradePiece);
			}
			PartyBase owner = ship.Owner;
			if (owner == null)
			{
				return;
			}
			MobileParty mobileParty = owner.MobileParty;
			if (mobileParty == null)
			{
				return;
			}
			mobileParty.SetNavalVisualAsDirty();
		}

		// Token: 0x060018C9 RID: 6345 RVA: 0x000AC430 File Offset: 0x000AA630
		private bool CanPartyUpgradeShips(MobileParty party)
		{
			return party.ActualClan != Clan.PlayerClan && party.Ships.Count > 0 && !party.IsCurrentlyUsedByAQuest && party.IsActive && party.MapEvent == null && party.SiegeEvent == null && !party.IsInRaftState && !party.IsDisbanding;
		}

		// Token: 0x060018CA RID: 6346 RVA: 0x000AC48B File Offset: 0x000AA68B
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x04000C0F RID: 3087
		private const float CaravanShipUpgradeChance = 0.4f;
	}
}
