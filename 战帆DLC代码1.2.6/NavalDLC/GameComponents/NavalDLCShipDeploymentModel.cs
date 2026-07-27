using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Helpers;
using NavalDLC.CharacterDevelopment;
using NavalDLC.ComponentInterfaces;
using NavalDLC.Missions.MissionLogics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.GameComponents
{
	// Token: 0x02000133 RID: 307
	public class NavalDLCShipDeploymentModel : ShipDeploymentModel
	{
		// Token: 0x060014F8 RID: 5368 RVA: 0x000936B0 File Offset: 0x000918B0
		public override int GetShipDeploymentLimit(MobileParty party)
		{
			int num = (ShipDeploymentModel.IgnoreDeploymentLimits ? 8 : 3);
			ExplainedNumber explainedNumber;
			explainedNumber..ctor((float)num, false, null);
			PerkHelper.AddPerkBonusForParty(NavalPerks.Boatswain.PortAuthority, party, true, ref explainedNumber, false);
			PerkHelper.AddPerkBonusForParty(NavalPerks.Boatswain.BlessingsOfTheSea, party, true, ref explainedNumber, false);
			PerkHelper.AddPerkBonusForParty(NavalPerks.Boatswain.MerchantFleet, party, true, ref explainedNumber, false);
			PerkHelper.AddPerkBonusForParty(NavalPerks.Shipmaster.Stormrider, party, false, ref explainedNumber, false);
			PerkHelper.AddPerkBonusForParty(NavalPerks.Shipmaster.MasterAndCommander, party, false, ref explainedNumber, false);
			return (int)explainedNumber.ResultNumber;
		}

		// Token: 0x060014F9 RID: 5369 RVA: 0x00093728 File Offset: 0x00091928
		public override void GetMapEventPartiesOfPlayerTeams(MBReadOnlyList<MapEventParty> playerSideMapEventParties, bool isPlayerSergeant, out MapEventParty playerMapEventParty, out MBList<MapEventParty> playerTeamMapEventParties, out MBList<MapEventParty> playerAllyTeamMapEventParties)
		{
			MobileParty mainParty = MobileParty.MainParty;
			playerMapEventParty = playerSideMapEventParties.FirstOrDefault<MapEventParty>((MapEventParty mep) => !mep.IsNpcParty);
			Army army = mainParty.Army;
			playerTeamMapEventParties = new MBList<MapEventParty>();
			playerAllyTeamMapEventParties = new MBList<MapEventParty>();
			bool flag = false;
			IBattleCombatant battleCombatant;
			bool flag2 = MissionCombatantsLogic.SupportsAllyTeamOnPlayerSide(playerSideMapEventParties.Select<MapEventParty, PartyBase>((MapEventParty mapEventParty) => mapEventParty.Party), playerMapEventParty.Party, isPlayerSergeant, flag, ref battleCombatant);
			foreach (MapEventParty mapEventParty2 in playerSideMapEventParties)
			{
				if (PartyBase.IsPartyUnderPlayerCommand(mapEventParty2.Party) || !flag2)
				{
					playerTeamMapEventParties.Add(mapEventParty2);
				}
				else
				{
					playerAllyTeamMapEventParties.Add(mapEventParty2);
				}
			}
		}

		// Token: 0x060014FA RID: 5370 RVA: 0x00093814 File Offset: 0x00091A14
		public override void GetShipDeploymentLimitsOfPlayerTeams(MBList<MapEventParty> playerTeamMapEventParties, MBList<MapEventParty> playerAllyTeamMapEventParties, out NavalShipDeploymentLimit playerTeamDeploymentLimit, out NavalShipDeploymentLimit playerAllyTeamDeploymentLimit)
		{
			if (!Extensions.IsEmpty<MapEventParty>(playerAllyTeamMapEventParties))
			{
				playerTeamDeploymentLimit = this.GetTeamShipDeploymentLimit(playerTeamMapEventParties);
				playerAllyTeamDeploymentLimit = this.GetTeamShipDeploymentLimit(playerAllyTeamMapEventParties);
				int netDeploymentLimit = playerTeamDeploymentLimit.NetDeploymentLimit;
				int netDeploymentLimit2 = playerAllyTeamDeploymentLimit.NetDeploymentLimit;
				int num = netDeploymentLimit + netDeploymentLimit2;
				if (num > 8)
				{
					num = 8;
					float num2 = (float)netDeploymentLimit / (float)(netDeploymentLimit + netDeploymentLimit2);
					int num3 = MathF.Min(MathF.Max(1, MathF.Round(num2 * (float)num)), netDeploymentLimit);
					int num4 = num - num3;
					if (num3 > playerTeamDeploymentLimit.SkeletalCrewLimit)
					{
						int num5 = num3 - playerTeamDeploymentLimit.SkeletalCrewLimit;
						num3 -= num5;
						num4 = MathF.Min(num4 + num5, playerAllyTeamDeploymentLimit.SkeletalCrewLimit);
					}
					if (num4 > playerAllyTeamDeploymentLimit.SkeletalCrewLimit)
					{
						int num6 = num4 - playerAllyTeamDeploymentLimit.SkeletalCrewLimit;
						num4 -= num6;
						num3 = MathF.Min(num3 + num6, playerTeamDeploymentLimit.SkeletalCrewLimit);
					}
					playerTeamDeploymentLimit = new NavalShipDeploymentLimit(playerTeamDeploymentLimit.PartiesLimit, playerTeamDeploymentLimit.SkeletalCrewLimit, num3);
					playerAllyTeamDeploymentLimit = new NavalShipDeploymentLimit(playerAllyTeamDeploymentLimit.PartiesLimit, playerAllyTeamDeploymentLimit.SkeletalCrewLimit, num4);
					return;
				}
			}
			else
			{
				playerTeamDeploymentLimit = this.GetTeamShipDeploymentLimit(playerTeamMapEventParties);
				playerAllyTeamDeploymentLimit = NavalShipDeploymentLimit.Invalid();
			}
		}

		// Token: 0x060014FB RID: 5371 RVA: 0x0009393C File Offset: 0x00091B3C
		public override NavalShipDeploymentLimit GetTeamShipDeploymentLimit(MBReadOnlyList<MapEventParty> teamMapEventParties)
		{
			int num = 0;
			MBList<Ship> mblist = new MBList<Ship>();
			int num2 = 0;
			foreach (MapEventParty mapEventParty in teamMapEventParties)
			{
				MobileParty mobileParty = mapEventParty.Party.MobileParty;
				if (mobileParty != null)
				{
					mblist.AddRange(mobileParty.Ships);
					num += mobileParty.Party.NumberOfHealthyMembers;
					num2 += NavalDLCManager.Instance.GameModels.ShipDeploymentModel.GetShipDeploymentLimit(mobileParty);
				}
			}
			mblist.Sort((Ship s1, Ship s2) => s1.SkeletalCrewCapacity.CompareTo(s2.SkeletalCrewCapacity));
			int num3 = num;
			int num4 = 0;
			foreach (Ship ship in mblist)
			{
				if (num3 < ship.SkeletalCrewCapacity)
				{
					break;
				}
				num3 -= ship.SkeletalCrewCapacity;
				num4++;
			}
			num4 = MathF.Min(MathF.Max(num4, 1), 8);
			num2 = MathF.Min(num2, 8);
			return new NavalShipDeploymentLimit(num2, num4, MathF.Max(num2, num4));
		}

		// Token: 0x060014FC RID: 5372 RVA: 0x00093A78 File Offset: 0x00091C78
		public override Ship GetSuitablePlayerShip(MapEventParty playerMapEventParty, MBList<MapEventParty> playerTeamMapEventParties)
		{
			int playerTeamTroopCount = playerTeamMapEventParties.Sum<MapEventParty>((MapEventParty mep) => mep.Party.NumberOfHealthyMembers);
			Ship ship2;
			if (!Extensions.IsEmpty<Ship>(playerMapEventParty.Ships))
			{
				IEnumerable<Ship> enumerable = playerMapEventParty.Ships.Where<Ship>((Ship s1) => s1.SkeletalCrewCapacity <= playerTeamTroopCount);
				if (!Extensions.IsEmpty<Ship>(enumerable))
				{
					ship2 = Extensions.MaxBy<Ship, float>(enumerable, (Ship ship) => ship.GetCombatFactor());
				}
				else
				{
					ship2 = Extensions.MinBy<Ship, int>(playerMapEventParty.Ships, (Ship ship) => ship.SkeletalCrewCapacity);
				}
			}
			else
			{
				MBList<Ship> mblist = new MBList<Ship>();
				foreach (MapEventParty mapEventParty in playerTeamMapEventParties)
				{
					mblist.AddRange(mapEventParty.Ships);
				}
				IEnumerable<Ship> enumerable2 = mblist.Where<Ship>((Ship s1) => s1.SkeletalCrewCapacity <= playerTeamTroopCount);
				if (!Extensions.IsEmpty<Ship>(enumerable2))
				{
					ship2 = Extensions.MinBy<Ship, float>(enumerable2, (Ship ship) => ship.GetCombatFactor());
				}
				else
				{
					ship2 = Extensions.MinBy<Ship, int>(mblist, (Ship ship) => ship.SkeletalCrewCapacity);
				}
			}
			return ship2;
		}

		// Token: 0x060014FD RID: 5373 RVA: 0x00093BFC File Offset: 0x00091DFC
		public override void FillShipsOfTeamParties(MBReadOnlyList<MapEventParty> teamMapEventParties, NavalShipDeploymentLimit shipDeploymentLimit, MBList<IShipOrigin> teamShips)
		{
			int netDeploymentLimit = shipDeploymentLimit.NetDeploymentLimit;
			IOrderedEnumerable<MapEventParty> orderedEnumerable = teamMapEventParties.OrderByDescending<MapEventParty, float>((MapEventParty teamEventParty) => NavalDLCShipDeploymentModel.GetNavalPartyPriority(teamEventParty.Party));
			int num = orderedEnumerable.Sum<MapEventParty>((MapEventParty party) => party.Party.NumberOfHealthyMembers);
			MBList<ValueTuple<Ship, MapEventParty, bool>> candidateShips = new MBList<ValueTuple<Ship, MapEventParty, bool>>();
			foreach (IShipOrigin shipOrigin in teamShips)
			{
				foreach (MapEventParty mapEventParty in orderedEnumerable)
				{
					if (mapEventParty.Ships.Contains(shipOrigin))
					{
						candidateShips.Add(new ValueTuple<Ship, MapEventParty, bool>((Ship)shipOrigin, mapEventParty, true));
						break;
					}
				}
			}
			teamShips.Clear();
			int num2 = 0;
			Dictionary<MapEventParty, MBQueue<ValueTuple<Ship, bool>>> dictionary = new Dictionary<MapEventParty, MBQueue<ValueTuple<Ship, bool>>>();
			MBList<ValueTuple<Ship, bool>> mblist = new MBList<ValueTuple<Ship, bool>>();
			Predicate<ValueTuple<Ship, bool>> <>9__2;
			foreach (MapEventParty mapEventParty2 in orderedEnumerable)
			{
				foreach (Ship ship in mapEventParty2.Ships)
				{
					mblist.Add(new ValueTuple<Ship, bool>(ship, false));
				}
				if (!Extensions.IsEmpty<ValueTuple<Ship, MapEventParty, bool>>(candidateShips))
				{
					List<ValueTuple<Ship, bool>> list = mblist;
					Predicate<ValueTuple<Ship, bool>> predicate;
					if ((predicate = <>9__2) == null)
					{
						predicate = (<>9__2 = ([TupleElementNames(new string[] { "ship", "isReplaced" })] ValueTuple<Ship, bool> teamShipTuple) => candidateShips.Any<ValueTuple<Ship, MapEventParty, bool>>(([TupleElementNames(new string[] { "ship", "party", "fixedShip" })] ValueTuple<Ship, MapEventParty, bool> candidateShipTuple) => candidateShipTuple.Item1 == teamShipTuple.Item1));
					}
					list.RemoveAll(predicate);
				}
				mblist.Sort(([TupleElementNames(new string[] { "ship", "isReplaced" })] ValueTuple<Ship, bool> firstShipTuple, [TupleElementNames(new string[] { "ship", "isReplaced" })] ValueTuple<Ship, bool> secondShipTuple) => secondShipTuple.Item1.GetCombatFactor().CompareTo(firstShipTuple.Item1.GetCombatFactor()));
				num2 += mblist.Count;
				dictionary[mapEventParty2] = new MBQueue<ValueTuple<Ship, bool>>(mblist);
				mblist.Clear();
			}
			bool flag = true;
			while (flag && candidateShips.Count < netDeploymentLimit)
			{
				flag = false;
				foreach (MapEventParty mapEventParty3 in orderedEnumerable)
				{
					MBQueue<ValueTuple<Ship, bool>> mbqueue = dictionary[mapEventParty3];
					if (!Extensions.IsEmpty<ValueTuple<Ship, bool>>(mbqueue))
					{
						ValueTuple<Ship, bool> valueTuple = mbqueue.Dequeue();
						num2--;
						candidateShips.Add(new ValueTuple<Ship, MapEventParty, bool>(valueTuple.Item1, mapEventParty3, false));
						flag = true;
					}
				}
			}
			if (num2 > 0)
			{
				int num3;
				bool flag2 = NavalDLCShipDeploymentModel.CanShipsBeFilled(num, 0.65f, candidateShips, out num3);
				bool flag3 = true;
				while (flag3 && !flag2)
				{
					flag3 = false;
					for (int i = num3; i >= 0; i--)
					{
						ValueTuple<Ship, MapEventParty, bool> valueTuple2 = candidateShips[i];
						if (!valueTuple2.Item3)
						{
							MapEventParty item = valueTuple2.Item2;
							MBQueue<ValueTuple<Ship, bool>> mbqueue2 = dictionary[item];
							if (!Extensions.IsEmpty<ValueTuple<Ship, bool>>(mbqueue2))
							{
								ValueTuple<Ship, bool> valueTuple3 = mbqueue2.Peek();
								if (!valueTuple3.Item2)
								{
									mbqueue2.Dequeue();
									mbqueue2.Enqueue(new ValueTuple<Ship, bool>(valueTuple2.Item1, true));
									candidateShips[i] = new ValueTuple<Ship, MapEventParty, bool>(valueTuple3.Item1, item, false);
									flag3 = true;
								}
							}
						}
						flag2 = NavalDLCShipDeploymentModel.CanShipsBeFilled(num, 0.65f, candidateShips, out num3);
						if (flag2)
						{
							break;
						}
					}
				}
			}
			if (num2 > 0)
			{
				flag = true;
				while (flag)
				{
					flag = false;
					foreach (MapEventParty mapEventParty4 in orderedEnumerable)
					{
						MBQueue<ValueTuple<Ship, bool>> mbqueue3 = dictionary[mapEventParty4];
						if (!Extensions.IsEmpty<ValueTuple<Ship, bool>>(mbqueue3))
						{
							ValueTuple<Ship, bool> valueTuple4 = mbqueue3.Dequeue();
							num2--;
							candidateShips.Add(new ValueTuple<Ship, MapEventParty, bool>(valueTuple4.Item1, mapEventParty4, false));
							flag = true;
						}
					}
				}
			}
			dictionary.Clear();
			if (candidateShips.Count > netDeploymentLimit)
			{
				bool flag4 = false;
				bool flag5 = true;
				while (!flag4 && flag5)
				{
					flag5 = false;
					flag4 = NavalDLCShipDeploymentModel.IsSkeletalCrewLimitationSatisfied(candidateShips, num, netDeploymentLimit);
					if (!flag4)
					{
						for (int j = netDeploymentLimit - 1; j >= 0; j--)
						{
							ValueTuple<Ship, MapEventParty, bool> valueTuple5 = candidateShips[j];
							if (!valueTuple5.Item3)
							{
								int skeletalCrewCapacity = valueTuple5.Item1.SkeletalCrewCapacity;
								int num4 = -1;
								if (NavalDLCShipDeploymentModel.FindBestSwapShipBelowSkeletalCrewLimit(candidateShips, valueTuple5, netDeploymentLimit, true, out num4))
								{
									ValueTuple<Ship, MapEventParty, bool> valueTuple6 = candidateShips[j];
									candidateShips[j] = candidateShips[num4];
									candidateShips[num4] = valueTuple6;
									flag5 = true;
									break;
								}
								if (NavalDLCShipDeploymentModel.FindBestSwapShipBelowSkeletalCrewLimit(candidateShips, valueTuple5, netDeploymentLimit, false, out num4))
								{
									ValueTuple<Ship, MapEventParty, bool> valueTuple7 = candidateShips[j];
									candidateShips[j] = candidateShips[num4];
									candidateShips[num4] = valueTuple7;
									flag5 = true;
									break;
								}
							}
						}
					}
				}
			}
			if (candidateShips.Count > netDeploymentLimit)
			{
				MBList<ValueTuple<Ship, MapEventParty, bool>> mblist2 = Extensions.ToMBList<ValueTuple<Ship, MapEventParty, bool>>(candidateShips.Skip<ValueTuple<Ship, MapEventParty, bool>>(netDeploymentLimit));
				candidateShips.RemoveRange(netDeploymentLimit, candidateShips.Count - netDeploymentLimit);
				mblist2.Sort(([TupleElementNames(new string[] { "ship", "party", "fixedShip" })] ValueTuple<Ship, MapEventParty, bool> s1, [TupleElementNames(new string[] { "ship", "party", "fixedShip" })] ValueTuple<Ship, MapEventParty, bool> s2) => s2.Item1.TotalCrewCapacity.CompareTo(s1.Item1.TotalCrewCapacity));
				candidateShips.AddRange(mblist2);
			}
			foreach (ValueTuple<Ship, MapEventParty, bool> valueTuple8 in candidateShips)
			{
				teamShips.Add(valueTuple8.Item1);
			}
		}

		// Token: 0x060014FE RID: 5374 RVA: 0x0009420C File Offset: 0x0009240C
		public override void GetOrderedCaptainsForPlayerTeamShips(MBReadOnlyList<MapEventParty> playerTeamMapEventParties, MBReadOnlyList<IShipOrigin> playerTeamShips, out List<string> playerTeamCaptainsByPriority)
		{
			List<string> list = HeroHelper.OrderHeroesOnPlayerSideByPriority(true, true);
			playerTeamCaptainsByPriority = new List<string>(playerTeamShips.Count);
			using (List<IShipOrigin>.Enumerator enumerator = playerTeamShips.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IShipOrigin ship = enumerator.Current;
					MapEventParty shipParty = playerTeamMapEventParties.FirstOrDefault<MapEventParty>((MapEventParty mep) => mep.Ships.Contains(ship));
					string text = list.FirstOrDefault<string>(delegate(string heroId)
					{
						Hero leaderHero = shipParty.Party.LeaderHero;
						return leaderHero != null && leaderHero.StringId.Equals(heroId);
					});
					if (text != null)
					{
						playerTeamCaptainsByPriority.Add(text);
						list.Remove(text);
					}
					else
					{
						playerTeamCaptainsByPriority.Add(string.Empty);
					}
				}
			}
			int num = 0;
			while (num < playerTeamCaptainsByPriority.Count && list.Count > 0)
			{
				if (Extensions.IsEmpty<char>(playerTeamCaptainsByPriority[num]))
				{
					playerTeamCaptainsByPriority[num] = list[0];
					list.RemoveAt(0);
				}
				num++;
			}
			int num2 = -1;
			int num3 = playerTeamCaptainsByPriority.Count - 1;
			while (num3 >= 0 && Extensions.IsEmpty<char>(playerTeamCaptainsByPriority[num3]))
			{
				num2 = num3;
				num3--;
			}
			if (num2 >= 0)
			{
				playerTeamCaptainsByPriority.RemoveRange(num2, playerTeamCaptainsByPriority.Count - num2);
			}
			int num4 = 0;
			for (int i = 0; i < playerTeamCaptainsByPriority.Count; i++)
			{
				if (Extensions.IsEmpty<char>(playerTeamCaptainsByPriority[i]))
				{
					for (int j = playerTeamCaptainsByPriority.Count - 1 - num4; j > i; j--)
					{
						if (!Extensions.IsEmpty<char>(playerTeamCaptainsByPriority[j]))
						{
							playerTeamCaptainsByPriority[i] = playerTeamCaptainsByPriority[j];
							playerTeamCaptainsByPriority[j] = string.Empty;
							num4++;
							break;
						}
					}
				}
			}
			playerTeamCaptainsByPriority.RemoveAll((string entry) => Extensions.IsEmpty<char>(entry));
		}

		// Token: 0x060014FF RID: 5375 RVA: 0x000943F8 File Offset: 0x000925F8
		public override int GetMaximumDeployableTroopCountForTeam(MBList<IShipOrigin> teamShips, bool isPlayerTeam = false)
		{
			int num = 0;
			if (teamShips != null && teamShips.Count > 0)
			{
				int num2 = MathF.Min(8, teamShips.Count);
				if (isPlayerTeam)
				{
					List<IShipOrigin> list = teamShips.OrderByDescending<IShipOrigin, int>((IShipOrigin ship) => ship.TotalCrewCapacity).ToList<IShipOrigin>();
					for (int i = 0; i < num2; i++)
					{
						num += list[i].TotalCrewCapacity;
					}
				}
				else
				{
					for (int j = 0; j < num2; j++)
					{
						num += teamShips[j].TotalCrewCapacity;
					}
				}
			}
			return num;
		}

		// Token: 0x06001500 RID: 5376 RVA: 0x00094490 File Offset: 0x00092690
		private static float GetNavalPartyPriority(PartyBase party)
		{
			float num = 0f;
			IFaction mapFaction = party.MapFaction;
			if (mapFaction != null && mapFaction.IsClan)
			{
				Clan clan = (Clan)mapFaction;
				Hero leaderHero = party.LeaderHero;
				Kingdom kingdom = clan.Kingdom;
				if (leaderHero != null)
				{
					if (kingdom != null && leaderHero == kingdom.Leader)
					{
						num += 100000f;
					}
					if (leaderHero == clan.Leader)
					{
						num += 10000f;
					}
				}
				int maxClanTier = Campaign.Current.Models.ClanTierModel.MaxClanTier;
				int minClanTier = Campaign.Current.Models.ClanTierModel.MinClanTier;
				float num2 = MathF.Clamp((float)(clan.Tier - minClanTier) / (float)maxClanTier, 0f, 1f);
				num += num2 * 1000f;
			}
			return num;
		}

		// Token: 0x06001501 RID: 5377 RVA: 0x00094554 File Offset: 0x00092754
		private static bool CanShipsBeFilled(int troopCount, float fillPercentage, [TupleElementNames(new string[] { "ship", "party", "fixedShip" })] MBReadOnlyList<ValueTuple<Ship, MapEventParty, bool>> ships, out int firstUnfilledIndex)
		{
			int num = troopCount;
			for (int i = ships.Count - 1; i >= 0; i--)
			{
				int num2 = (int)((float)ships[i].Item1.TotalCrewCapacity * fillPercentage);
				if (num < num2)
				{
					firstUnfilledIndex = i;
					return false;
				}
				num -= num2;
			}
			firstUnfilledIndex = -1;
			return true;
		}

		// Token: 0x06001502 RID: 5378 RVA: 0x000945A0 File Offset: 0x000927A0
		private static bool IsSkeletalCrewLimitationSatisfied([TupleElementNames(new string[] { "ship", "party", "fixedShip" })] MBList<ValueTuple<Ship, MapEventParty, bool>> ships, int troopCount, int shipsToProcessCount)
		{
			int num = MathF.Min(shipsToProcessCount, ships.Count);
			int num2 = troopCount;
			for (int i = 0; i < num; i++)
			{
				ValueTuple<Ship, MapEventParty, bool> valueTuple = ships[i];
				if (num2 < valueTuple.Item1.SkeletalCrewCapacity)
				{
					break;
				}
				num2 -= valueTuple.Item1.SkeletalCrewCapacity;
			}
			return num2 >= 0;
		}

		// Token: 0x06001503 RID: 5379 RVA: 0x000945F4 File Offset: 0x000927F4
		private static bool FindBestSwapShipBelowSkeletalCrewLimit([TupleElementNames(new string[] { "ship", "party", "fixedShip" })] MBList<ValueTuple<Ship, MapEventParty, bool>> ships, [TupleElementNames(new string[] { "ship", "party", "fixedShip" })] ValueTuple<Ship, MapEventParty, bool> shipTupleToBeSwapped, int startIndex, bool checkTeamMatch, out int swapIndex)
		{
			swapIndex = -1;
			int num = 0;
			int skeletalCrewCapacity = shipTupleToBeSwapped.Item1.SkeletalCrewCapacity;
			for (int i = startIndex; i < ships.Count; i++)
			{
				ValueTuple<Ship, MapEventParty, bool> valueTuple = ships[i];
				if (!valueTuple.Item3 && (!checkTeamMatch || valueTuple.Item2 == shipTupleToBeSwapped.Item2))
				{
					int skeletalCrewCapacity2 = valueTuple.Item1.SkeletalCrewCapacity;
					if (skeletalCrewCapacity2 < skeletalCrewCapacity && skeletalCrewCapacity2 > num)
					{
						swapIndex = i;
						num = skeletalCrewCapacity2;
					}
				}
			}
			return swapIndex > -1;
		}

		// Token: 0x04000B06 RID: 2822
		private const int BaseShipDeploymentLimit = 3;

		// Token: 0x04000B07 RID: 2823
		private const int MaxShipDeploymentLimit = 8;
	}
}
