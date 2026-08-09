using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using NavalDLC.CampaignBehaviors;
using NavalDLC.CharacterDevelopment;
using NavalDLC.Storyline;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;

namespace NavalDLC.GameComponents
{
	// Token: 0x0200010A RID: 266
	public class NavalDLCBattleRewardModel : BattleRewardModel
	{
		// Token: 0x06001358 RID: 4952 RVA: 0x0008C666 File Offset: 0x0008A866
		public override int CalculateGoldLossAfterDefeat(Hero partyLeaderHero)
		{
			return base.BaseModel.CalculateGoldLossAfterDefeat(partyLeaderHero);
		}

		// Token: 0x06001359 RID: 4953 RVA: 0x0008C674 File Offset: 0x0008A874
		public override ExplainedNumber CalculateInfluenceGain(PartyBase winnerParty, float influenceValueOfBattleForWinnerSide, float contributionShareOfWinnerParty, float influenceMultiplierForWinnerSide, bool includeDescriptions)
		{
			return base.BaseModel.CalculateInfluenceGain(winnerParty, influenceValueOfBattleForWinnerSide, contributionShareOfWinnerParty, influenceMultiplierForWinnerSide, includeDescriptions);
		}

		// Token: 0x0600135A RID: 4954 RVA: 0x0008C688 File Offset: 0x0008A888
		public override float CalculateMoraleChangeOnRoundVictory(PartyBase party, MapEventSide partySide, BattleSideEnum roundWinner)
		{
			return base.BaseModel.CalculateMoraleChangeOnRoundVictory(party, partySide, roundWinner);
		}

		// Token: 0x0600135B RID: 4955 RVA: 0x0008C698 File Offset: 0x0008A898
		public override ExplainedNumber CalculateMoraleGainVictory(PartyBase winnerParty, float renownValueOfBattleForWinnerSide, float contributionShareOfWinnerParty, bool includeDescriptions)
		{
			return base.BaseModel.CalculateMoraleGainVictory(winnerParty, renownValueOfBattleForWinnerSide, contributionShareOfWinnerParty, includeDescriptions);
		}

		// Token: 0x0600135C RID: 4956 RVA: 0x0008C6AA File Offset: 0x0008A8AA
		public override int CalculatePlunderedGoldAmountFromDefeatedParty(PartyBase defeatedParty)
		{
			return base.BaseModel.CalculatePlunderedGoldAmountFromDefeatedParty(defeatedParty);
		}

		// Token: 0x0600135D RID: 4957 RVA: 0x0008C6B8 File Offset: 0x0008A8B8
		public override ExplainedNumber CalculateRenownGain(PartyBase winnerParty, float renownValueOfBattleForWinnerSide, float contributionShareOfWinnerParty, float renownMultiplierForWinnerSide, bool includeDescriptions)
		{
			return base.BaseModel.CalculateRenownGain(winnerParty, renownValueOfBattleForWinnerSide, contributionShareOfWinnerParty, renownMultiplierForWinnerSide, includeDescriptions);
		}

		// Token: 0x0600135E RID: 4958 RVA: 0x0008C6CC File Offset: 0x0008A8CC
		public override float GetAITradePenalty()
		{
			return base.BaseModel.GetAITradePenalty();
		}

		// Token: 0x0600135F RID: 4959 RVA: 0x0008C6D9 File Offset: 0x0008A8D9
		public override float GetBannerLootChanceFromDefeatedHero(Hero defeatedHero)
		{
			return base.BaseModel.GetBannerLootChanceFromDefeatedHero(defeatedHero);
		}

		// Token: 0x06001360 RID: 4960 RVA: 0x0008C6E7 File Offset: 0x0008A8E7
		public override ItemObject GetBannerRewardForWinningMapEvent(MapEvent mapEvent)
		{
			return base.BaseModel.GetBannerRewardForWinningMapEvent(mapEvent);
		}

		// Token: 0x06001361 RID: 4961 RVA: 0x0008C6F5 File Offset: 0x0008A8F5
		public override float GetExpectedLootedItemValueFromCasualty(Hero winnerPartyLeaderHero, CharacterObject casualtyCharacter)
		{
			return base.BaseModel.GetExpectedLootedItemValueFromCasualty(winnerPartyLeaderHero, casualtyCharacter);
		}

		// Token: 0x06001362 RID: 4962 RVA: 0x0008C704 File Offset: 0x0008A904
		public override MBReadOnlyList<KeyValuePair<MapEventParty, float>> GetLootCasualtyChances(MBReadOnlyList<MapEventParty> winnerParties, PartyBase defeatedParty)
		{
			return base.BaseModel.GetLootCasualtyChances(winnerParties, defeatedParty);
		}

		// Token: 0x06001363 RID: 4963 RVA: 0x0008C713 File Offset: 0x0008A913
		public override EquipmentElement GetLootedItemFromTroop(CharacterObject character, float targetValue)
		{
			return base.BaseModel.GetLootedItemFromTroop(character, targetValue);
		}

		// Token: 0x06001364 RID: 4964 RVA: 0x0008C722 File Offset: 0x0008A922
		public override MBReadOnlyList<KeyValuePair<MapEventParty, float>> GetLootGoldChances(MBReadOnlyList<MapEventParty> winnerParties)
		{
			return base.BaseModel.GetLootGoldChances(winnerParties);
		}

		// Token: 0x06001365 RID: 4965 RVA: 0x0008C730 File Offset: 0x0008A930
		public override MBList<KeyValuePair<MapEventParty, float>> GetLootItemChancesForWinnerParties(MBReadOnlyList<MapEventParty> winnerParties, PartyBase defeatedParty)
		{
			MBList<KeyValuePair<MapEventParty, float>> lootItemChancesForWinnerParties = base.BaseModel.GetLootItemChancesForWinnerParties(winnerParties, defeatedParty);
			if (defeatedParty.IsMobile && (defeatedParty.MobileParty.IsCaravan || defeatedParty.MobileParty.IsVillager))
			{
				for (int i = 0; i < lootItemChancesForWinnerParties.Count; i++)
				{
					PartyBase party = lootItemChancesForWinnerParties[i].Key.Party;
					ExplainedNumber explainedNumber;
					explainedNumber..ctor(lootItemChancesForWinnerParties[i].Value, false, null);
					if (PartyBaseHelper.HasFeat(party, NavalCulturalFeats.NordHostileActionBonusFeat))
					{
						explainedNumber.AddFactor(NavalCulturalFeats.NordHostileActionBonusFeat.EffectBonus, null);
					}
					if (defeatedParty.MobileParty.IsCaravan && defeatedParty.MobileParty.CaravanPartyComponent.CanHaveNavalNavigationCapability)
					{
						PerkHelper.AddPerkBonusForParty(NavalPerks.Mariner.PiratesProwess, party.MobileParty, false, ref explainedNumber, false);
					}
					lootItemChancesForWinnerParties[i] = new KeyValuePair<MapEventParty, float>(lootItemChancesForWinnerParties[i].Key, explainedNumber.ResultNumber);
				}
			}
			return lootItemChancesForWinnerParties;
		}

		// Token: 0x06001366 RID: 4966 RVA: 0x0008C834 File Offset: 0x0008AA34
		public override void GetCaptureMemberChancesForWinnerParties(MapEvent endedMapEvent, MBReadOnlyList<MapEventParty> winnerParties, out MBList<KeyValuePair<MapEventParty, float>> woundedMemberChances, out MBList<KeyValuePair<MapEventParty, float>> healthyMemberChances)
		{
			woundedMemberChances = new MBList<KeyValuePair<MapEventParty, float>>();
			healthyMemberChances = new MBList<KeyValuePair<MapEventParty, float>>();
			base.BaseModel.GetCaptureMemberChancesForWinnerParties(endedMapEvent, winnerParties, ref woundedMemberChances, ref healthyMemberChances);
			float num = 0f;
			for (int i = 0; i < woundedMemberChances.Count; i++)
			{
				KeyValuePair<MapEventParty, float> keyValuePair = woundedMemberChances[i];
				MapEventParty key = keyValuePair.Key;
				ExplainedNumber explainedNumber;
				explainedNumber..ctor(keyValuePair.Value, false, null);
				if (key.Party.IsMobile)
				{
					PerkHelper.AddPerkBonusForParty(NavalPerks.Shipmaster.RiverRaider, key.Party.MobileParty, false, ref explainedNumber, false);
				}
				woundedMemberChances[i] = new KeyValuePair<MapEventParty, float>(key, explainedNumber.ResultNumber);
				num += woundedMemberChances[i].Value;
			}
			if (num > 0f)
			{
				for (int j = 0; j < woundedMemberChances.Count; j++)
				{
					woundedMemberChances[j] = new KeyValuePair<MapEventParty, float>(woundedMemberChances[j].Key, woundedMemberChances[j].Value / num);
				}
			}
			num = 0f;
			for (int k = 0; k < healthyMemberChances.Count; k++)
			{
				KeyValuePair<MapEventParty, float> keyValuePair2 = healthyMemberChances[k];
				MapEventParty key2 = keyValuePair2.Key;
				ExplainedNumber explainedNumber2;
				explainedNumber2..ctor(keyValuePair2.Value, false, null);
				if (key2.Party.IsMobile)
				{
					PerkHelper.AddPerkBonusForParty(NavalPerks.Shipmaster.RiverRaider, key2.Party.MobileParty, false, ref explainedNumber2, false);
				}
				healthyMemberChances[k] = new KeyValuePair<MapEventParty, float>(key2, explainedNumber2.ResultNumber);
				num += woundedMemberChances[k].Value;
			}
			if (num > 0f)
			{
				for (int l = 0; l < healthyMemberChances.Count; l++)
				{
					healthyMemberChances[l] = new KeyValuePair<MapEventParty, float>(healthyMemberChances[l].Key, healthyMemberChances[l].Value / num);
				}
			}
		}

		// Token: 0x06001367 RID: 4967 RVA: 0x0008CA2D File Offset: 0x0008AC2D
		public override MBReadOnlyList<KeyValuePair<MapEventParty, float>> GetLootPrisonerChances(MBReadOnlyList<MapEventParty> winnerParties, TroopRosterElement prisonerElement)
		{
			return base.BaseModel.GetLootPrisonerChances(winnerParties, prisonerElement);
		}

		// Token: 0x06001368 RID: 4968 RVA: 0x0008CA3C File Offset: 0x0008AC3C
		public override float CalculateShipDamageAfterDefeat(Ship ship)
		{
			return ship.MaxHitPoints * MBRandom.RandomFloatRanged(0.2f, 0.5f);
		}

		// Token: 0x06001369 RID: 4969 RVA: 0x0008CA54 File Offset: 0x0008AC54
		public override MBReadOnlyList<KeyValuePair<Ship, MapEventParty>> DistributeDefeatedPartyShipsAmongWinners(MapEvent mapEvent, MBReadOnlyList<Ship> shipsToLoot, MBReadOnlyList<MapEventParty> winnerParties)
		{
			if (mapEvent.IsPlayerMapEvent && NavalStorylineData.IsNavalStoryLineActive())
			{
				return new MBReadOnlyList<KeyValuePair<Ship, MapEventParty>>();
			}
			Dictionary<Ship, MapEventParty> dictionary = new Dictionary<Ship, MapEventParty>();
			MBList<Ship> mblist = new MBList<Ship>();
			foreach (Ship ship in shipsToLoot)
			{
				dictionary.Add(ship, null);
				if (MBRandom.RandomFloat < 0.5f)
				{
					if (ship.CanEquipFigurehead)
					{
						ship.ChangeFigurehead(null);
					}
					mblist.Add(ship);
				}
			}
			IEnumerable<MapEventParty> enumerable = LinQuick.WhereQ<MapEventParty>(winnerParties, (MapEventParty x) => x.Party.IsMobile && x.Party.MobileParty.PartyComponent.CanHaveNavalNavigationCapability && !x.Party.MobileParty.IsPatrolParty);
			if (LinQuick.AnyQ<MapEventParty>(enumerable))
			{
				float winnerPartiesTotalScoreForLootingShips = LinQuick.SumQ<MapEventParty>(enumerable, (MapEventParty x) => this.PartyLootShipScore(x));
				List<MapEventParty> list = LinQuick.OrderByQ<MapEventParty, float>(enumerable, (MapEventParty x) => (float)x.Party.Ships.Count + (1f - this.PartyLootShipScore(x) / winnerPartiesTotalScoreForLootingShips)).ToList<MapEventParty>();
				List<MapEventParty> list2 = new List<MapEventParty>();
				if (mblist.Count < list.Count)
				{
					list2 = list.GetRange(mblist.Count, list.Count - mblist.Count).ToList<MapEventParty>();
					list.RemoveRange(mblist.Count, list.Count - mblist.Count);
				}
				list = list.OrderByDescending<MapEventParty, float>((MapEventParty x) => this.PartyLootShipScore(x)).ToList<MapEventParty>();
				if (LinQuick.AnyQ<MapEventParty>(list2))
				{
					list.AddRange(list2.OrderByDescending<MapEventParty, float>((MapEventParty x) => this.PartyLootShipScore(x)).ToList<MapEventParty>());
				}
				bool flag = true;
				while (flag && mblist.Count > 0)
				{
					flag = false;
					foreach (MapEventParty mapEventParty in list)
					{
						MBList<Ship> mblist2 = Extensions.ToMBList<Ship>(mapEventParty.Ships);
						foreach (KeyValuePair<Ship, MapEventParty> keyValuePair in dictionary)
						{
							if (keyValuePair.Value == mapEventParty)
							{
								mblist2.Add(keyValuePair.Key);
							}
						}
						Ship shipToLootForWinnerParty = this.GetShipToLootForWinnerParty(mapEventParty, mblist2, mblist);
						if (shipToLootForWinnerParty != null)
						{
							flag = true;
							dictionary[shipToLootForWinnerParty] = mapEventParty;
							mblist.Remove(shipToLootForWinnerParty);
						}
						if (mblist.Count == 0)
						{
							break;
						}
					}
					if (mblist.Count > 0)
					{
						list = list.OrderByDescending<MapEventParty, float>((MapEventParty x) => this.PartyLootShipScore(x)).ToList<MapEventParty>();
					}
				}
			}
			if (mblist.Count > 0)
			{
				if (LinQuick.AnyQ<MapEventParty>(winnerParties, (MapEventParty x) => x.Party == PartyBase.MainParty))
				{
					Extensions.Shuffle<Ship>(mblist);
					int num = LinQuick.CountQ<KeyValuePair<Ship, MapEventParty>>(dictionary, delegate(KeyValuePair<Ship, MapEventParty> x)
					{
						MapEventParty value = x.Value;
						return ((value != null) ? value.Party : null) == PartyBase.MainParty;
					});
					if (mblist.Count + num > 25)
					{
						mblist = Extensions.ToMBList<Ship>(mblist.Take<Ship>(25 - num));
					}
					MapEventParty mapEventParty2 = winnerParties.Find((MapEventParty x) => x.Party == PartyBase.MainParty);
					int num2 = 0;
					foreach (MapEventParty mapEventParty3 in winnerParties)
					{
						int contributionToBattle = mapEventParty3.ContributionToBattle;
						num2 += contributionToBattle;
					}
					foreach (Ship ship2 in mblist)
					{
						if (MBRandom.RandomInt(num2) >= mapEventParty2.ContributionToBattle)
						{
							break;
						}
						dictionary[ship2] = mapEventParty2;
					}
				}
			}
			return Extensions.ToMBList<KeyValuePair<Ship, MapEventParty>>(dictionary);
		}

		// Token: 0x0600136A RID: 4970 RVA: 0x0008CE48 File Offset: 0x0008B048
		private float PartyLootShipScore(MapEventParty party)
		{
			ExplainedNumber explainedNumber;
			explainedNumber..ctor((float)party.ContributionToBattle, false, null);
			explainedNumber.Add((float)party.Party.MemberRoster.TotalManCount, null, null);
			if (party.Party.LeaderHero != null)
			{
				Hero leaderHero = party.Party.LeaderHero;
				if (leaderHero.IsKingdomLeader)
				{
					explainedNumber.Add(50000f, null, null);
				}
				else if (leaderHero.IsClanLeader)
				{
					explainedNumber.Add(20000f, null, null);
				}
				if (leaderHero.Clan != null)
				{
					float num = MBMath.Map((float)leaderHero.Clan.Tier, (float)Campaign.Current.Models.ClanTierModel.MinClanTier, (float)Campaign.Current.Models.ClanTierModel.MaxClanTier, 5000f, 10000f);
					explainedNumber.Add(num, null, null);
				}
			}
			MobileParty mobileParty = party.Party.MobileParty;
			if (((mobileParty != null) ? mobileParty.ActualClan : null) != null)
			{
				float num2 = MBMath.Map((float)party.Party.MobileParty.ActualClan.Tier, (float)Campaign.Current.Models.ClanTierModel.MinClanTier, (float)Campaign.Current.Models.ClanTierModel.MaxClanTier, 5000f, 10000f);
				explainedNumber.Add(num2, null, null);
			}
			if (party.Party.IsMobile)
			{
				PerkHelper.AddPerkBonusForParty(NavalPerks.Boatswain.GildedPurse, party.Party.MobileParty, true, ref explainedNumber, false);
			}
			return (float)explainedNumber.RoundedResultNumber;
		}

		// Token: 0x0600136B RID: 4971 RVA: 0x0008CFC4 File Offset: 0x0008B1C4
		private Ship GetShipToLootForWinnerParty(MapEventParty winnerParty, MBList<Ship> partyShipsToConsider, MBList<Ship> lootableShips)
		{
			float num = NavalDLCManager.Instance.GameModels.ShipDistributionModel.GetScoreForPartyShipComposition(winnerParty.Party.MobileParty, partyShipsToConsider);
			Ship ship = null;
			foreach (Ship ship2 in lootableShips)
			{
				if (NavalDLCManager.Instance.GameModels.ShipDistributionModel.CanPartyTakeShip(winnerParty.Party, ship2))
				{
					partyShipsToConsider.Add(ship2);
					float scoreForPartyShipComposition = NavalDLCManager.Instance.GameModels.ShipDistributionModel.GetScoreForPartyShipComposition(winnerParty.Party.MobileParty, partyShipsToConsider);
					partyShipsToConsider.Remove(ship2);
					if (scoreForPartyShipComposition > num)
					{
						num = scoreForPartyShipComposition;
						ship = ship2;
					}
				}
			}
			return ship;
		}

		// Token: 0x0600136C RID: 4972 RVA: 0x0008D088 File Offset: 0x0008B288
		public override float GetMainPartyMemberScatterChance()
		{
			return base.BaseModel.GetMainPartyMemberScatterChance();
		}

		// Token: 0x0600136D RID: 4973 RVA: 0x0008D095 File Offset: 0x0008B295
		public override int GetPlayerGainedRelationAmount(MapEvent mapEvent, Hero hero)
		{
			return base.BaseModel.GetPlayerGainedRelationAmount(mapEvent, hero);
		}

		// Token: 0x0600136E RID: 4974 RVA: 0x0008D0A4 File Offset: 0x0008B2A4
		public override float GetShipSiegeEngineHitMoraleEffect(Ship ship, SiegeEngineType siegeEngineType)
		{
			return 0f;
		}

		// Token: 0x0600136F RID: 4975 RVA: 0x0008D0AC File Offset: 0x0008B2AC
		public override float GetSunkenShipMoraleEffect(PartyBase shipOwner, Ship ship)
		{
			float num = -2f;
			switch (ship.ShipHull.Type)
			{
			case 0:
				num = -1f;
				break;
			case 1:
				num = -2f;
				break;
			case 2:
				num = -3f;
				break;
			default:
				Debug.FailedAssert("Ship type not handled", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\GameComponents\\NavalDLCBattleRewardModel.cs", "GetSunkenShipMoraleEffect", 437);
				break;
			}
			return num;
		}

		// Token: 0x06001370 RID: 4976 RVA: 0x0008D114 File Offset: 0x0008B314
		public override MBReadOnlyList<MapEventParty> GetWinnerPartiesThatCanPlunderGoldFromShips(MBReadOnlyList<MapEventParty> winnerParties)
		{
			MBList<MapEventParty> mblist = new MBList<MapEventParty>();
			foreach (MapEventParty mapEventParty in winnerParties)
			{
				if (mapEventParty.Party != PartyBase.MainParty && mapEventParty.ContributionToBattle > 0 && mapEventParty.Party.IsMobile && !mapEventParty.Party.MobileParty.IsBandit && !mapEventParty.Party.MobileParty.IsCaravan)
				{
					mblist.Add(mapEventParty);
				}
			}
			return mblist;
		}

		// Token: 0x06001371 RID: 4977 RVA: 0x0008D1B0 File Offset: 0x0008B3B0
		public override Figurehead GetFigureheadLoot(MBReadOnlyList<MapEventParty> defeatedParties, PartyBase defeatedSideLeaderParty)
		{
			Figurehead figurehead = null;
			if (this.CanUnlockFigurehead())
			{
				IEnumerable<Hero> enumerable = LinQuick.SelectQ<MapEventParty, Hero>(LinQuick.WhereQ<MapEventParty>(defeatedParties, (MapEventParty x) => x.Party.LeaderHero != null), (MapEventParty x) => x.Party.LeaderHero);
				float figureheadDropChanceForHeroes = this.GetFigureheadDropChanceForHeroes(enumerable);
				if (MBRandom.RandomFloat <= figureheadDropChanceForHeroes)
				{
					List<Figurehead> unlockedFigureheadsByMainHero = Campaign.Current.UnlockedFigureheadsByMainHero;
					List<ValueTuple<Figurehead, float>> list = new List<ValueTuple<Figurehead, float>>();
					foreach (MapEventParty mapEventParty in defeatedParties)
					{
						foreach (Ship ship in mapEventParty.Ships)
						{
							if (ship.Figurehead != null && !unlockedFigureheadsByMainHero.Contains(ship.Figurehead))
							{
								if (mapEventParty.Party == defeatedSideLeaderParty)
								{
									MobileParty mobileParty = defeatedSideLeaderParty.MobileParty;
									MobileParty mobileParty2;
									if (mobileParty == null)
									{
										mobileParty2 = null;
									}
									else
									{
										Army army = mobileParty.Army;
										mobileParty2 = ((army != null) ? army.LeaderParty : null);
									}
									if (mobileParty2 == defeatedSideLeaderParty.MobileParty)
									{
										list.Add(new ValueTuple<Figurehead, float>(ship.Figurehead, 0.2f));
										continue;
									}
								}
								list.Add(new ValueTuple<Figurehead, float>(ship.Figurehead, 0.1f));
							}
						}
					}
					return MBRandom.ChooseWeighted<Figurehead>(list);
				}
			}
			return figurehead;
		}

		// Token: 0x06001372 RID: 4978 RVA: 0x0008D344 File Offset: 0x0008B544
		private bool CanUnlockFigurehead()
		{
			return Campaign.Current.GetCampaignBehavior<NavalDLCFigureheadCampaignBehavior>().LastFigureheadLootTime.ElapsedDaysUntilNow >= 8f;
		}

		// Token: 0x06001373 RID: 4979 RVA: 0x0008D374 File Offset: 0x0008B574
		private float GetFigureheadDropChanceForHeroes(IEnumerable<Hero> heroes)
		{
			float num = 0f;
			foreach (Hero hero in heroes)
			{
				IFaction mapFaction = hero.MapFaction;
				if (mapFaction != null && mapFaction.IsKingdomFaction && hero.MapFaction.Leader == hero)
				{
					num = 0.6f;
					break;
				}
				Clan clan = hero.Clan;
				if (((clan != null) ? clan.Leader : null) == hero && num < 0.5f)
				{
					num = 0.5f;
				}
				else if (hero.Clan != null && num < 0.4f)
				{
					num = 0.4f;
				}
			}
			return num;
		}

		// Token: 0x06001374 RID: 4980 RVA: 0x0008D424 File Offset: 0x0008B624
		public override bool CanTroopBeTakenPrisoner(CharacterObject troop)
		{
			return base.BaseModel.CanTroopBeTakenPrisoner(troop);
		}
	}
}
