using System;
using System.Linq;
using Helpers;
using NavalDLC.CharacterDevelopment;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace NavalDLC.CampaignBehaviors
{
	// Token: 0x02000178 RID: 376
	public class ShipTradeCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x060018AA RID: 6314 RVA: 0x000AADCC File Offset: 0x000A8FCC
		public override void RegisterEvents()
		{
			CampaignEvents.OnNewGameCreatedPartialFollowUpEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter, int>(this.OnNewGameCreatedPartialFollowUp));
			CampaignEvents.DailyTickClanEvent.AddNonSerializedListener(this, new Action<Clan>(this.DailyTickClan));
			CampaignEvents.OnShipOwnerChangedEvent.AddNonSerializedListener(this, new Action<Ship, PartyBase, ChangeShipOwnerAction.ShipOwnerChangeDetail>(this.OnShipOwnerChanged));
			CampaignEvents.OnShipRepairedEvent.AddNonSerializedListener(this, new Action<Ship, Settlement>(this.OnShipRepaired));
			CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
			CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener(this, new Action(this.OnGameLoadFinished));
			CampaignEvents.TickEvent.AddNonSerializedListener(this, new Action<float>(this.Tick));
		}

		// Token: 0x060018AB RID: 6315 RVA: 0x000AAE7C File Offset: 0x000A907C
		private void OnGameLoadFinished()
		{
			if (MBSaveLoad.IsUpdatingGameVersion && MBSaveLoad.LastLoadedGameVersion.IsOlderThan(ApplicationVersion.FromString("v1.3.9.103828", 0)))
			{
				foreach (MobileParty mobileParty in MobileParty.AllLordParties)
				{
					if (mobileParty != MobileParty.MainParty && mobileParty.MapEvent == null && mobileParty.SiegeEvent == null && mobileParty.ActualClan != null && mobileParty.LeaderHero != null && mobileParty.Ships.Count > 0 && mobileParty.IsActive && !mobileParty.IsCurrentlyUsedByAQuest && mobileParty.LeaderHero.IsActive)
					{
						int num = 0;
						Ship ship;
						while (this.TryGetShipToSell(mobileParty, out ship))
						{
							num += (int)Campaign.Current.Models.ShipCostModel.GetShipTradeValue(ship, mobileParty.Party, null);
							DestroyShipAction.Apply(ship);
						}
						if (num > 0)
						{
							GiveGoldAction.ApplyBetweenCharacters(null, mobileParty.ActualClan.Leader, num, false);
						}
					}
				}
				foreach (MobileParty mobileParty2 in MobileParty.AllBanditParties)
				{
					if (mobileParty2.MapEvent == null && mobileParty2.ActualClan != null && mobileParty2.Ships.Count > 0 && mobileParty2.IsActive && !mobileParty2.IsCurrentlyUsedByAQuest && mobileParty2.Ships.Count > Campaign.Current.Models.PartyShipLimitModel.GetIdealShipNumber(mobileParty2))
					{
						for (int i = mobileParty2.Ships.Count - 1; i > Campaign.Current.Models.PartyShipLimitModel.GetIdealShipNumber(mobileParty2) - 1; i--)
						{
							DestroyShipAction.Apply(mobileParty2.Ships[i]);
						}
					}
				}
			}
		}

		// Token: 0x060018AC RID: 6316 RVA: 0x000AB088 File Offset: 0x000A9288
		private void OnNewGameCreatedPartialFollowUp(CampaignGameStarter starter, int index)
		{
			foreach (Clan clan in Clan.All)
			{
				this.DailyTickClan(clan);
			}
		}

		// Token: 0x060018AD RID: 6317 RVA: 0x000AB0DC File Offset: 0x000A92DC
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x060018AE RID: 6318 RVA: 0x000AB0E0 File Offset: 0x000A92E0
		private void DailyTickClan(Clan clan)
		{
			if (!clan.IsBanditFaction && !clan.IsEliminated && clan != Clan.PlayerClan)
			{
				this.ConsiderPurchasingShip(clan);
				this.ConsiderSwappingClanLeaderShips(clan);
				this.ConsiderSwappingShipsBetweenClanParties(clan);
				if (this.GetTotalNumberOfWarShipsInClan(clan) > NavalDLCManager.Instance.GameModels.ClanShipOwnershipModel.GetIdealShipNumberForClan(clan))
				{
					this.ConsiderSellingShips(clan);
				}
			}
		}

		// Token: 0x060018AF RID: 6319 RVA: 0x000AB140 File Offset: 0x000A9340
		private void ConsiderPurchasingShip(Clan clan)
		{
			if (MBRandom.RandomFloat < this.GetClanShipPurchaseChance(clan))
			{
				MobileParty partyToGiveShipTo = this.GetPartyToGiveShipTo(clan);
				if (partyToGiveShipTo != null)
				{
					Town townToBuyShipFrom = this.GetTownToBuyShipFrom(clan);
					if (townToBuyShipFrom != null)
					{
						this.TryPurchasingShipFromTown(partyToGiveShipTo, townToBuyShipFrom);
					}
				}
			}
		}

		// Token: 0x060018B0 RID: 6320 RVA: 0x000AB179 File Offset: 0x000A9379
		private float GetClanShipPurchaseChance(Clan clan)
		{
			return 0.5f;
		}

		// Token: 0x060018B1 RID: 6321 RVA: 0x000AB180 File Offset: 0x000A9380
		private void TryPurchasingShipFromTown(MobileParty mobileParty, Town town)
		{
			Ship ship = null;
			MBList<Ship> mblist = Extensions.ToMBList<Ship>(mobileParty.Ships);
			float num = NavalDLCManager.Instance.GameModels.ShipDistributionModel.GetScoreForPartyShipComposition(mobileParty, mblist);
			foreach (Ship ship2 in town.AvailableShips)
			{
				if (NavalDLCManager.Instance.GameModels.ShipDistributionModel.CanPartyTakeShip(mobileParty.Party, ship2) && Campaign.Current.Models.ShipCostModel.GetShipTradeValue(ship2, town.Settlement.Party, mobileParty.Party) < (float)mobileParty.ActualClan.Gold * 0.2f)
				{
					mblist.Add(ship2);
					float scoreForPartyShipComposition = NavalDLCManager.Instance.GameModels.ShipDistributionModel.GetScoreForPartyShipComposition(mobileParty, mblist);
					mblist.Remove(ship2);
					if (scoreForPartyShipComposition > num)
					{
						num = scoreForPartyShipComposition;
						ship = ship2;
					}
				}
			}
			Ship ship3 = null;
			foreach (Ship ship4 in town.AvailableShips)
			{
				if (NavalDLCManager.Instance.GameModels.ShipDistributionModel.CanPartyTakeShip(mobileParty.Party, ship4))
				{
					for (int i = 0; i < mblist.Count; i++)
					{
						if (Campaign.Current.Models.ShipCostModel.GetShipTradeValue(ship4, town.Settlement.Party, mobileParty.Party) < (float)mobileParty.ActualClan.Gold * 0.2f)
						{
							Ship ship5 = mblist[i];
							mblist[i] = ship4;
							float scoreForPartyShipComposition2 = NavalDLCManager.Instance.GameModels.ShipDistributionModel.GetScoreForPartyShipComposition(mobileParty, mblist);
							if (scoreForPartyShipComposition2 > num)
							{
								num = scoreForPartyShipComposition2;
								ship = ship4;
								ship3 = ship5;
							}
							mblist[i] = ship5;
						}
					}
				}
			}
			if (ship != null)
			{
				if (ship3 != null)
				{
					ChangeShipOwnerAction.ApplyByTrade(town.Settlement.Party, ship3);
				}
				ChangeShipOwnerAction.ApplyByTrade(mobileParty.Party, ship);
			}
		}

		// Token: 0x060018B2 RID: 6322 RVA: 0x000AB3AC File Offset: 0x000A95AC
		private MobileParty GetPartyToGiveShipTo(Clan clan)
		{
			MobileParty mobileParty = null;
			float num = float.MaxValue;
			foreach (WarPartyComponent warPartyComponent in clan.WarPartyComponents)
			{
				if (this.CanPartyTradeShip(warPartyComponent.MobileParty))
				{
					float scoreForPartyShipComposition = NavalDLCManager.Instance.GameModels.ShipDistributionModel.GetScoreForPartyShipComposition(warPartyComponent.MobileParty, warPartyComponent.MobileParty.Ships);
					if (scoreForPartyShipComposition < num)
					{
						num = scoreForPartyShipComposition;
						mobileParty = warPartyComponent.MobileParty;
					}
				}
			}
			return mobileParty;
		}

		// Token: 0x060018B3 RID: 6323 RVA: 0x000AB448 File Offset: 0x000A9648
		private Town GetTownToBuyShipFrom(Clan clan)
		{
			Town town = null;
			if (clan.MapFaction.Fiefs.Count > 0)
			{
				town = Extensions.GetRandomElementWithPredicate<Town>(clan.MapFaction.Fiefs, (Town x) => this.CanClanBuyShipFromTown(clan, x));
			}
			if (town == null && MBRandom.RandomFloat < 0.2f)
			{
				town = Extensions.GetRandomElementWithPredicate<Town>(Town.AllTowns, (Town x) => this.CanClanBuyShipFromTown(clan, x) && !x.MapFaction.IsAtWarWith(clan));
			}
			return town;
		}

		// Token: 0x060018B4 RID: 6324 RVA: 0x000AB4CC File Offset: 0x000A96CC
		private bool CanClanBuyShipFromTown(Clan clan, Town town)
		{
			return !town.IsUnderSiege && town.AvailableShips.Count > 0;
		}

		// Token: 0x060018B5 RID: 6325 RVA: 0x000AB4E8 File Offset: 0x000A96E8
		private void ConsiderSwappingClanLeaderShips(Clan clan)
		{
			if (MBRandom.RandomFloat < 0.75f && clan.WarPartyComponents.Count > 2 && this.CanPartyTradeShip(clan.Leader.PartyBelongedTo))
			{
				MobileParty mobileParty = Extensions.GetRandomElementWithPredicate<WarPartyComponent>(clan.WarPartyComponents, (WarPartyComponent x) => x.MobileParty != clan.Leader.PartyBelongedTo).MobileParty;
				if (mobileParty != null && this.CanPartyTradeShip(mobileParty))
				{
					MBList<Ship> mblist = Extensions.ToMBList<Ship>(clan.Leader.PartyBelongedTo.Ships);
					float num = NavalDLCManager.Instance.GameModels.ShipDistributionModel.GetScoreForPartyShipComposition(clan.Leader.PartyBelongedTo, mblist);
					Tuple<Ship, Ship> tuple = new Tuple<Ship, Ship>(null, null);
					for (int i = mblist.Count - 1; i >= 0; i--)
					{
						Ship ship = mblist[i];
						if (ship.IsTradeable && NavalDLCManager.Instance.GameModels.ShipDistributionModel.CanPartyTakeShip(mobileParty.Party, ship))
						{
							MBList<Ship> mblist2 = Extensions.ToMBList<Ship>(mobileParty.Ships);
							if (mblist2.Any<Ship>())
							{
								mblist.RemoveAt(i);
								for (int j = 0; j < mblist2.Count; j++)
								{
									Ship ship2 = mblist2[j];
									if (ship2.IsTradeable && NavalDLCManager.Instance.GameModels.ShipDistributionModel.CanPartyTakeShip(clan.Leader.PartyBelongedTo.Party, ship2))
									{
										mblist.Add(ship2);
										float scoreForPartyShipComposition = NavalDLCManager.Instance.GameModels.ShipDistributionModel.GetScoreForPartyShipComposition(clan.Leader.PartyBelongedTo, mblist);
										if (scoreForPartyShipComposition > num)
										{
											num = scoreForPartyShipComposition;
											tuple = new Tuple<Ship, Ship>(ship, ship2);
										}
										mblist.Remove(ship2);
									}
								}
								mblist.Add(ship);
							}
						}
					}
					if (tuple.Item1 != null)
					{
						ChangeShipOwnerAction.ApplyByTransferring(tuple.Item2.Owner, tuple.Item1);
						ChangeShipOwnerAction.ApplyByTransferring(clan.Leader.PartyBelongedTo.Party, tuple.Item2);
					}
				}
			}
		}

		// Token: 0x060018B6 RID: 6326 RVA: 0x000AB728 File Offset: 0x000A9928
		private void ConsiderSwappingShipsBetweenClanParties(Clan clan)
		{
			if (MBRandom.RandomFloat < 0.75f && clan.WarPartyComponents.Count > 2)
			{
				ShipTradeCampaignBehavior.<>c__DisplayClass17_0 CS$<>8__locals1 = new ShipTradeCampaignBehavior.<>c__DisplayClass17_0();
				CS$<>8__locals1.<>4__this = this;
				ShipTradeCampaignBehavior.<>c__DisplayClass17_0 CS$<>8__locals2 = CS$<>8__locals1;
				WarPartyComponent randomElementWithPredicate = Extensions.GetRandomElementWithPredicate<WarPartyComponent>(clan.WarPartyComponents, (WarPartyComponent x) => this.CanPartyTradeShip(x.MobileParty));
				CS$<>8__locals2.party1 = ((randomElementWithPredicate != null) ? randomElementWithPredicate.MobileParty : null);
				WarPartyComponent randomElementWithPredicate2 = Extensions.GetRandomElementWithPredicate<WarPartyComponent>(clan.WarPartyComponents, (WarPartyComponent x) => x.MobileParty != CS$<>8__locals1.party1 && CS$<>8__locals1.<>4__this.CanPartyTradeShip(x.MobileParty));
				MobileParty mobileParty = ((randomElementWithPredicate2 != null) ? randomElementWithPredicate2.MobileParty : null);
				if (CS$<>8__locals1.party1 != null && mobileParty != null && !CS$<>8__locals1.party1.IsDisbanding && !mobileParty.IsDisbanding)
				{
					MBList<Ship> mblist = Extensions.ToMBList<Ship>(CS$<>8__locals1.party1.Ships);
					MBList<Ship> mblist2 = Extensions.ToMBList<Ship>(mobileParty.Ships);
					float scoreForPartyShipComposition = NavalDLCManager.Instance.GameModels.ShipDistributionModel.GetScoreForPartyShipComposition(CS$<>8__locals1.party1, mblist);
					float scoreForPartyShipComposition2 = NavalDLCManager.Instance.GameModels.ShipDistributionModel.GetScoreForPartyShipComposition(mobileParty, mblist2);
					float num = scoreForPartyShipComposition + scoreForPartyShipComposition2;
					Tuple<Ship, Ship> tuple = new Tuple<Ship, Ship>(null, null);
					for (int i = mblist.Count - 1; i >= 0; i--)
					{
						Ship ship = mblist[i];
						if (ship.IsTradeable && NavalDLCManager.Instance.GameModels.ShipDistributionModel.CanPartyTakeShip(mobileParty.Party, ship))
						{
							mblist.RemoveAt(i);
							float num2 = NavalDLCManager.Instance.GameModels.ShipDistributionModel.GetScoreForPartyShipComposition(CS$<>8__locals1.party1, mblist);
							mblist2.Add(ship);
							float num3 = NavalDLCManager.Instance.GameModels.ShipDistributionModel.GetScoreForPartyShipComposition(mobileParty, mblist2);
							mblist2.Remove(ship);
							if (num2 + num3 > num && CS$<>8__locals1.party1.Ships.Count > 1 && (clan.Leader.PartyBelongedTo != CS$<>8__locals1.party1 || num2 > scoreForPartyShipComposition) && (clan.Leader.PartyBelongedTo != mobileParty || num3 > scoreForPartyShipComposition2))
							{
								num = num2 + num3;
								tuple = new Tuple<Ship, Ship>(ship, null);
							}
							for (int j = mblist2.Count - 1; j >= 0; j--)
							{
								Ship ship2 = mblist2[j];
								if (ship2.IsTradeable && NavalDLCManager.Instance.GameModels.ShipDistributionModel.CanPartyTakeShip(CS$<>8__locals1.party1.Party, ship2))
								{
									mblist.Add(ship2);
									mblist2.Add(ship);
									mblist2.RemoveAt(j);
									num2 = NavalDLCManager.Instance.GameModels.ShipDistributionModel.GetScoreForPartyShipComposition(CS$<>8__locals1.party1, mblist);
									num3 = NavalDLCManager.Instance.GameModels.ShipDistributionModel.GetScoreForPartyShipComposition(mobileParty, mblist2);
									if (num2 + num3 > num && (clan.Leader.PartyBelongedTo != CS$<>8__locals1.party1 || num2 > scoreForPartyShipComposition) && (clan.Leader.PartyBelongedTo != mobileParty || num3 > scoreForPartyShipComposition2))
									{
										num = num2 + num3;
										tuple = new Tuple<Ship, Ship>(ship, ship2);
									}
									mblist2.Remove(ship);
									mblist2.Add(ship2);
								}
							}
							mblist.Add(ship);
						}
					}
					if (tuple.Item1 != null)
					{
						if (tuple.Item2 != null)
						{
							ChangeShipOwnerAction.ApplyByTransferring(CS$<>8__locals1.party1.Party, tuple.Item2);
						}
						ChangeShipOwnerAction.ApplyByTransferring(mobileParty.Party, tuple.Item1);
					}
				}
			}
		}

		// Token: 0x060018B7 RID: 6327 RVA: 0x000ABA7C File Offset: 0x000A9C7C
		private void ConsiderSellingShips(Clan clan)
		{
			if (MBRandom.RandomFloat < 0.1f && clan.WarPartyComponents.Any<WarPartyComponent>())
			{
				MobileParty mobileParty = Extensions.GetRandomElement<WarPartyComponent>(clan.WarPartyComponents).MobileParty;
				Ship ship;
				if (!mobileParty.IsDisbanding && this.CanPartyTradeShip(mobileParty) && this.TryGetShipToSell(mobileParty, out ship))
				{
					Town townToSellShip = this.GetTownToSellShip(clan);
					if (townToSellShip != null)
					{
						ChangeShipOwnerAction.ApplyByTrade(townToSellShip.Settlement.Party, ship);
					}
				}
			}
		}

		// Token: 0x060018B8 RID: 6328 RVA: 0x000ABAEC File Offset: 0x000A9CEC
		private bool TryGetShipToSell(MobileParty mobileParty, out Ship shipToSell)
		{
			shipToSell = null;
			MBList<Ship> mblist = Extensions.ToMBList<Ship>(mobileParty.Ships);
			float num = NavalDLCManager.Instance.GameModels.ShipDistributionModel.GetScoreForPartyShipComposition(mobileParty, mblist);
			for (int i = mblist.Count - 1; i >= 0; i--)
			{
				Ship ship = mblist[i];
				if (ship.IsTradeable)
				{
					mblist.RemoveAt(i);
					float scoreForPartyShipComposition = NavalDLCManager.Instance.GameModels.ShipDistributionModel.GetScoreForPartyShipComposition(mobileParty, mblist);
					if (scoreForPartyShipComposition > num)
					{
						num = scoreForPartyShipComposition;
						shipToSell = ship;
					}
					mblist.Add(ship);
				}
			}
			return shipToSell != null;
		}

		// Token: 0x060018B9 RID: 6329 RVA: 0x000ABB78 File Offset: 0x000A9D78
		private Town GetTownToSellShip(Clan clan)
		{
			return Extensions.GetRandomElementWithPredicate<Town>(clan.MapFaction.Fiefs, (Town x) => x.IsTown && x.GetShipyard() != null && x.GetShipyard().CurrentLevel > 0);
		}

		// Token: 0x060018BA RID: 6330 RVA: 0x000ABBAC File Offset: 0x000A9DAC
		private int GetTotalNumberOfWarShipsInClan(Clan clan)
		{
			int num = 0;
			for (int i = 0; i < clan.WarPartyComponents.Count; i++)
			{
				num += clan.WarPartyComponents[i].MobileParty.Ships.Count;
			}
			return num;
		}

		// Token: 0x060018BB RID: 6331 RVA: 0x000ABBF0 File Offset: 0x000A9DF0
		private bool CanPartyTradeShip(MobileParty party)
		{
			return party != null && party.MapEvent == null && party.SiegeEvent == null && !party.IsCurrentlyAtSea && party.LeaderHero != null && party.IsActive;
		}

		// Token: 0x060018BC RID: 6332 RVA: 0x000ABC20 File Offset: 0x000A9E20
		private void OnShipOwnerChanged(Ship ship, PartyBase oldOwner, ChangeShipOwnerAction.ShipOwnerChangeDetail details)
		{
			if (details == null)
			{
				Hero hero = null;
				if (oldOwner.IsSettlement)
				{
					hero = oldOwner.Settlement.Town.Governor;
				}
				else if (ship.Owner.IsSettlement)
				{
					hero = ship.Owner.Settlement.Town.Governor;
				}
				if (hero != null && (hero != Hero.MainHero || ship.Owner.LeaderHero != Hero.MainHero))
				{
					ExplainedNumber explainedNumber;
					explainedNumber..ctor(0f, false, null);
					PerkHelper.AddPerkBonusForTown(NavalPerks.Boatswain.MerchantPrince, hero.CurrentSettlement.Town, ref explainedNumber);
					GiveGoldAction.ApplyBetweenCharacters(null, hero, explainedNumber.RoundedResultNumber, false);
				}
			}
		}

		// Token: 0x060018BD RID: 6333 RVA: 0x000ABCC4 File Offset: 0x000A9EC4
		private void OnShipRepaired(Ship ship, Settlement repairPort)
		{
			if (repairPort != null && repairPort.IsTown)
			{
				Hero governor = repairPort.Town.Governor;
				if (governor != null && (governor != Hero.MainHero || ship.Owner.LeaderHero != Hero.MainHero))
				{
					ExplainedNumber explainedNumber;
					explainedNumber..ctor(0f, false, null);
					PerkHelper.AddPerkBonusForTown(NavalPerks.Boatswain.MasterShipwright, repairPort.Town, ref explainedNumber);
					GiveGoldAction.ApplyBetweenCharacters(null, governor, explainedNumber.RoundedResultNumber, false);
				}
			}
		}

		// Token: 0x060018BE RID: 6334 RVA: 0x000ABD34 File Offset: 0x000A9F34
		public void OnSettlementEntered(MobileParty mobileParty, Settlement settlement, Hero hero)
		{
			if (mobileParty != null && mobileParty.IsCaravan && mobileParty.HasNavalNavigationCapability && settlement.IsTown && settlement.Town.Governor != null)
			{
				if (settlement.Town.Governor.GetPerkValue(NavalPerks.Boatswain.Salvage))
				{
					settlement.Town.TradeTaxAccumulated += MathF.Round(NavalPerks.Boatswain.Salvage.SecondaryBonus);
				}
				if (settlement.Town.Governor.GetPerkValue(NavalPerks.Boatswain.ShipwrightsHand))
				{
					CharacterObject basicTroop = settlement.MapFaction.BasicTroop;
					int characterWage = Campaign.Current.Models.PartyWageModel.GetCharacterWage(basicTroop);
					if (settlement.GarrisonWagePaymentLimit > characterWage + 5)
					{
						MobileParty mobileParty2 = settlement.Town.GarrisonParty;
						if (mobileParty2 == null)
						{
							settlement.AddGarrisonParty();
							mobileParty2 = settlement.Town.GarrisonParty;
						}
						int num = Math.Min(mobileParty2.GetAvailableWageBudget() / characterWage, MathF.Round(NavalPerks.Boatswain.ShipwrightsHand.SecondaryBonus));
						if (num > 0)
						{
							mobileParty2.MemberRoster.AddToCounts(basicTroop, num, false, 0, 0, true, -1);
						}
					}
				}
			}
		}

		// Token: 0x060018BF RID: 6335 RVA: 0x000ABE4C File Offset: 0x000AA04C
		private void Tick(float dt)
		{
			if (ShipTradeCampaignBehavior.DebugNavalLordParties || ShipTradeCampaignBehavior.DebugLordParties)
			{
				foreach (MobileParty mobileParty in MobileParty.AllLordParties)
				{
					if ((ShipTradeCampaignBehavior.DebugLordParties || (mobileParty.IsCurrentlyAtSea && ShipTradeCampaignBehavior.DebugNavalLordParties)) && (mobileParty.Army == null || mobileParty.Army.LeaderParty == mobileParty) && mobileParty.CurrentSettlement == null && !mobileParty.IsInRaftState && mobileParty != MobileParty.MainParty)
					{
						(mobileParty.Position.AsVec3() + Vec3.Up * 3.75f).x -= 1f;
						if (mobileParty.Army != null)
						{
							string.Format("Army Ship Count: {0}", mobileParty.Ships.Count + mobileParty.AttachedParties.Sum<MobileParty>((MobileParty x) => x.Ships.Count));
						}
						else
						{
							string.Format("Ship Count: {0}", mobileParty.Ships.Count);
						}
					}
				}
				int num = 0;
				foreach (Kingdom kingdom in Kingdom.All)
				{
					kingdom.WarPartyComponents.Count<WarPartyComponent>((WarPartyComponent x) => x.MobileParty.IsCurrentlyAtSea && !x.MobileParty.IsInRaftState);
					kingdom.WarPartyComponents.Count<WarPartyComponent>((WarPartyComponent x) => !x.MobileParty.IsCurrentlyAtSea);
					num++;
				}
			}
		}

		// Token: 0x04000C0A RID: 3082
		private const float ShipSellingChance = 0.1f;

		// Token: 0x04000C0B RID: 3083
		private const float ShipTransferringChance = 0.75f;

		// Token: 0x04000C0C RID: 3084
		private const float ClanGoldRatioToBuyShip = 0.2f;

		// Token: 0x04000C0D RID: 3085
		public static bool DebugNavalLordParties;

		// Token: 0x04000C0E RID: 3086
		public static bool DebugLordParties;
	}
}
