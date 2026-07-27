using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using NavalDLC.ComponentInterfaces;
using NavalDLC.Missions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace NavalDLC.CampaignBehaviors
{
	// Token: 0x02000168 RID: 360
	public class NavalForceStartNavalMissionCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x060017AE RID: 6062 RVA: 0x000A163A File Offset: 0x0009F83A
		public override void RegisterEvents()
		{
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
			CampaignEvents.TickEvent.AddNonSerializedListener(this, new Action<float>(this.OnTick));
		}

		// Token: 0x060017AF RID: 6063 RVA: 0x000A166A File Offset: 0x0009F86A
		private void OnSessionLaunched(CampaignGameStarter starter)
		{
			this.AddGameMenus(starter);
		}

		// Token: 0x060017B0 RID: 6064 RVA: 0x000A1673 File Offset: 0x0009F873
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x060017B1 RID: 6065 RVA: 0x000A1678 File Offset: 0x0009F878
		private void AddGameMenus(CampaignGameStarter starter)
		{
			starter.AddGameMenuOption("encounter", "attack_naval", "{=!}Start Naval Mission (Cheat)", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = 1;
				return true;
			}, new GameMenuOption.OnConsequenceDelegate(this.StartNavalBattle), false, 2, false, null);
		}

		// Token: 0x060017B2 RID: 6066 RVA: 0x000A16C9 File Offset: 0x0009F8C9
		private void OnTick(float dt)
		{
			if (NavalForceStartNavalMissionCampaignBehavior._forceStartNavalMission && GameStateManager.Current.ActiveState is MapState)
			{
				this.StartNavalMissionFromCheats();
				NavalForceStartNavalMissionCampaignBehavior._forceStartNavalMission = false;
			}
		}

		// Token: 0x060017B3 RID: 6067 RVA: 0x000A16F0 File Offset: 0x0009F8F0
		private void HealPartiesInPlayerEncounterCheat()
		{
			foreach (MapEventParty mapEventParty in MapEvent.PlayerMapEvent.PartiesOnSide(PlayerEncounter.Current.PlayerSide))
			{
				PartyBase party = mapEventParty.Party;
				for (int i = 0; i < party.MemberRoster.Count; i++)
				{
					TroopRosterElement elementCopyAtIndex = party.MemberRoster.GetElementCopyAtIndex(i);
					if (elementCopyAtIndex.Character.IsHero)
					{
						elementCopyAtIndex.Character.HeroObject.Heal(elementCopyAtIndex.Character.HeroObject.MaxHitPoints, false);
					}
					else
					{
						party.AddToMemberRosterElementAtIndex(i, 0, -party.MemberRoster.GetElementWoundedNumber(i));
					}
				}
			}
			foreach (MapEventParty mapEventParty2 in MapEvent.PlayerMapEvent.PartiesOnSide(PlayerEncounter.Current.OpponentSide))
			{
				PartyBase party2 = mapEventParty2.Party;
				for (int j = 0; j < party2.MemberRoster.Count; j++)
				{
					TroopRosterElement elementCopyAtIndex2 = party2.MemberRoster.GetElementCopyAtIndex(j);
					if (elementCopyAtIndex2.Character.IsHero)
					{
						elementCopyAtIndex2.Character.HeroObject.Heal(elementCopyAtIndex2.Character.HeroObject.MaxHitPoints, false);
					}
					else
					{
						party2.AddToMemberRosterElementAtIndex(j, 0, -party2.MemberRoster.GetElementWoundedNumber(j));
					}
				}
			}
		}

		// Token: 0x060017B4 RID: 6068 RVA: 0x000A1884 File Offset: 0x0009FA84
		private void StartNavalMissionFromCheats()
		{
			this.StartNavalMissionWithHandlingCheat();
		}

		// Token: 0x060017B5 RID: 6069 RVA: 0x000A188C File Offset: 0x0009FA8C
		private void StartNavalMissionWithHandlingCheat()
		{
			PartyBase mainParty = PartyBase.MainParty;
			if (PlayerEncounter.Current == null)
			{
				this.SetupTeamForEncounterCheat(0, mainParty);
				if (this._enemyParty == null)
				{
					IEnumerable<MobileParty> enumerable = MobileParty.AllLordParties.Where<MobileParty>((MobileParty x) => x.IsActive);
					this._enemyParty = Extensions.GetRandomElementInefficiently<MobileParty>(enumerable).Party;
					this._enemyParty.MemberRoster.Clear();
				}
				this.SetupTeamForEncounterCheat(2, this._enemyParty);
				if (this._enemyParty.Position.IsOnLand)
				{
					CampaignVec2 campaignVec = NavigationHelper.FindPointAroundPosition(Extensions.GetRandomElementInefficiently<Settlement>(Campaign.Current.Settlements.Where<Settlement>((Settlement x) => x.HasPort)).PortPosition, 2, 10f, 1f, true, false);
					this._enemyParty.MobileParty.Position = campaignVec;
				}
				PlayerEncounter.RestartPlayerEncounter(this._enemyParty, mainParty, true, false);
			}
			else if (this._enemyParty == null)
			{
				this._enemyParty = MapEvent.PlayerMapEvent.PartiesOnSide(PlayerEncounter.Current.OpponentSide)[0].Party;
			}
			if (mainParty.Ships.Count == 0)
			{
				NavalForceStartNavalMissionCampaignBehavior.AddShipsToTeamPartyForEncounterCheat(0, mainParty);
			}
			if (this._enemyParty.Ships.Count == 0)
			{
				NavalForceStartNavalMissionCampaignBehavior.AddShipsToTeamPartyForEncounterCheat(2, this._enemyParty);
			}
			if (mainParty.MemberRoster.TotalManCount == 1)
			{
				NavalForceStartNavalMissionCampaignBehavior.AddTroopsToTeamPartyForEncounterCheat(0, mainParty);
			}
			if (this._enemyParty.MemberRoster.TotalManCount == 0)
			{
				NavalForceStartNavalMissionCampaignBehavior.AddTroopsToTeamPartyForEncounterCheat(2, this._enemyParty);
			}
			if (this._enemyParty.Position.IsOnLand != mainParty.Position.IsOnLand)
			{
				if (this._enemyParty.Position.IsOnLand)
				{
					this._enemyParty.MobileParty.Position = mainParty.Position;
				}
				else
				{
					mainParty.MobileParty.Position = this._enemyParty.Position;
				}
			}
			if (!this._enemyParty.MapFaction.IsAtWarWith(Clan.PlayerClan.MapFaction))
			{
				DeclareWarAction.ApplyByDefault(this._enemyParty.MapFaction, Clan.PlayerClan.MapFaction);
			}
			if (PlayerEncounter.Battle == null)
			{
				PlayerEncounter.StartBattle();
			}
			this.HealPartiesInPlayerEncounterCheat();
			string text = ((!string.IsNullOrEmpty(NavalForceStartNavalMissionCampaignBehavior._sceneName)) ? NavalForceStartNavalMissionCampaignBehavior._sceneName : "battle_terrain_opensea_northern");
			MissionInitializerRecord missionInitializerRecord;
			missionInitializerRecord..ctor(text);
			TerrainType faceTerrainType = Campaign.Current.MapSceneWrapper.GetFaceTerrainType(MobileParty.MainParty.CurrentNavigationFace);
			missionInitializerRecord.TerrainType = faceTerrainType;
			missionInitializerRecord.DamageToFriendsMultiplier = Campaign.Current.Models.DifficultyModel.GetPlayerTroopsReceivedDamageMultiplier();
			missionInitializerRecord.DamageFromPlayerToFriendsMultiplier = Campaign.Current.Models.DifficultyModel.GetPlayerTroopsReceivedDamageMultiplier();
			missionInitializerRecord.NeedsRandomTerrain = false;
			missionInitializerRecord.PlayingInCampaignMode = true;
			missionInitializerRecord.RandomTerrainSeed = MBRandom.RandomInt(10000);
			missionInitializerRecord.AtmosphereOnCampaign = Campaign.Current.Models.MapWeatherModel.GetAtmosphereModel(MobileParty.MainParty.Position);
			missionInitializerRecord.SceneHasMapPatch = false;
			missionInitializerRecord.DecalAtlasGroup = 2;
			NavalMissions.OpenNavalBattleMission(missionInitializerRecord);
		}

		// Token: 0x060017B6 RID: 6070 RVA: 0x000A1BA4 File Offset: 0x0009FDA4
		private void SetupTeamForEncounterCheat(TeamSideEnum teamSide, PartyBase teamParty)
		{
			foreach (TroopRosterElement troopRosterElement in teamParty.MemberRoster.GetTroopRoster())
			{
				if (troopRosterElement.Character != CharacterObject.PlayerCharacter)
				{
					teamParty.MemberRoster.RemoveTroop(troopRosterElement.Character, troopRosterElement.Number, default(UniqueTroopDescriptor), 0);
				}
			}
			foreach (Ship ship in teamParty.Ships.ToList<Ship>())
			{
				DestroyShipAction.Apply(ship);
			}
			NavalForceStartNavalMissionCampaignBehavior.AddShipsToTeamPartyForEncounterCheat(teamSide, teamParty);
			NavalForceStartNavalMissionCampaignBehavior.AddTroopsToTeamPartyForEncounterCheat(teamSide, teamParty);
		}

		// Token: 0x060017B7 RID: 6071 RVA: 0x000A1C78 File Offset: 0x0009FE78
		private static void AddTroopsToTeamPartyForEncounterCheat(TeamSideEnum teamSide, PartyBase teamParty)
		{
			int num = 0;
			int num2 = 0;
			if (NavalForceStartNavalMissionCampaignBehavior._maximizeTroopCounts)
			{
				NavalForceStartNavalMissionCampaignBehavior.GetMaximumTroopCountForShipList(teamParty.Ships, out num, out num2);
			}
			else if (teamSide == null)
			{
				num = NavalForceStartNavalMissionCampaignBehavior._playerMeleeTroopCount;
				num2 = NavalForceStartNavalMissionCampaignBehavior._playerRangedTroopCount;
			}
			else if (teamSide == 2)
			{
				num = NavalForceStartNavalMissionCampaignBehavior._enemyMeleeTroopCount;
				num2 = NavalForceStartNavalMissionCampaignBehavior._enemyRangedTroopCount;
			}
			else
			{
				Debug.FailedAssert("This team side is not currently supported", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\CampaignBehaviors\\NavalForceStartNavalMissionCampaignBehavior.cs", "AddTroopsToTeamPartyForEncounterCheat", 287);
			}
			teamParty.MemberRoster.AddToCounts(MBObjectManager.Instance.GetObject<CharacterObject>("imperial_recruit"), num, false, 0, 0, true, -1);
			teamParty.MemberRoster.AddToCounts(MBObjectManager.Instance.GetObject<CharacterObject>("imperial_archer"), num2, false, 0, 0, true, -1);
		}

		// Token: 0x060017B8 RID: 6072 RVA: 0x000A1D20 File Offset: 0x0009FF20
		private static MBList<Ship> AddShipsToTeamPartyForEncounterCheat(TeamSideEnum teamSide, PartyBase teamParty)
		{
			MBList<Ship> defaultShipSet = NavalForceStartNavalMissionCampaignBehavior.GetDefaultShipSet(teamSide);
			foreach (Ship ship in defaultShipSet)
			{
				ChangeShipOwnerAction.ApplyByLooting(teamParty, ship);
			}
			return defaultShipSet;
		}

		// Token: 0x060017B9 RID: 6073 RVA: 0x000A1D78 File Offset: 0x0009FF78
		private static void GetMaximumTroopCountForShipList(MBReadOnlyList<Ship> shipList, out int maxMeleeTroopCount, out int maxRangedTroopCount)
		{
			int num = shipList.Sum<Ship>((Ship ship) => ship.TotalCrewCapacity);
			maxRangedTroopCount = num / 2;
			maxMeleeTroopCount = num - NavalForceStartNavalMissionCampaignBehavior._playerRangedTroopCount;
		}

		// Token: 0x060017BA RID: 6074 RVA: 0x000A1DB9 File Offset: 0x0009FFB9
		private void StartNavalBattle(MenuCallbackArgs args)
		{
			this.StartNavalMissionWithHandlingCheat();
		}

		// Token: 0x060017BB RID: 6075 RVA: 0x000A1DC4 File Offset: 0x0009FFC4
		private static Ship CreateShip(string shipHullId)
		{
			ShipHull @object = MBObjectManager.Instance.GetObject<ShipHull>(shipHullId);
			if (@object != null)
			{
				return new Ship(@object);
			}
			return null;
		}

		// Token: 0x060017BC RID: 6076 RVA: 0x000A1DE8 File Offset: 0x0009FFE8
		private static MBList<Ship> GetDefaultShipSet(TeamSideEnum teamSide)
		{
			MBList<Ship> mblist = new MBList<Ship>();
			foreach (string text in NavalForceStartNavalMissionCampaignBehavior._shipHullIds[teamSide])
			{
				Ship ship = NavalForceStartNavalMissionCampaignBehavior.CreateShip(text);
				mblist.Add(ship);
			}
			return mblist;
		}

		// Token: 0x060017BD RID: 6077 RVA: 0x000A1E48 File Offset: 0x000A0048
		private static string GetMissionSettings()
		{
			string text = "Scene Name: " + NavalForceStartNavalMissionCampaignBehavior._sceneName + "\nTroop Counts Maximized: " + NavalForceStartNavalMissionCampaignBehavior._maximizeTroopCounts.ToString();
			if (!NavalForceStartNavalMissionCampaignBehavior._maximizeTroopCounts)
			{
				text = string.Concat(new object[]
				{
					text,
					"\nPlayer Melee Troop Count: ",
					NavalForceStartNavalMissionCampaignBehavior._playerMeleeTroopCount,
					"\nPlayer Ranged Troop Count: ",
					NavalForceStartNavalMissionCampaignBehavior._playerRangedTroopCount,
					"\nEnemy Melee Troop Count: ",
					NavalForceStartNavalMissionCampaignBehavior._enemyMeleeTroopCount,
					"\nEnemy Ranged Troop Count: ",
					NavalForceStartNavalMissionCampaignBehavior._enemyRangedTroopCount
				});
			}
			for (int i = 0; i < NavalForceStartNavalMissionCampaignBehavior._shipHullIds.Length; i++)
			{
				MBList<string> mblist = NavalForceStartNavalMissionCampaignBehavior._shipHullIds[i];
				if (!Extensions.IsEmpty<string>(mblist))
				{
					TeamSideEnum teamSideEnum = i;
					text = text + "\n" + teamSideEnum.ToString() + " Mission Ships:";
					int num = mblist.Count - 1;
					for (int j = 0; j < num; j++)
					{
						text = text + mblist[j] + ", ";
					}
					text += mblist[num];
				}
			}
			return text;
		}

		// Token: 0x060017BE RID: 6078 RVA: 0x000A1F61 File Offset: 0x000A0161
		private static void ResetMissionSettings()
		{
			NavalForceStartNavalMissionCampaignBehavior._sceneName = "battle_terrain_opensea_northern";
			NavalForceStartNavalMissionCampaignBehavior._maximizeTroopCounts = false;
			NavalForceStartNavalMissionCampaignBehavior._playerMeleeTroopCount = 30;
			NavalForceStartNavalMissionCampaignBehavior._playerRangedTroopCount = 30;
			NavalForceStartNavalMissionCampaignBehavior._enemyMeleeTroopCount = 30;
			NavalForceStartNavalMissionCampaignBehavior._enemyRangedTroopCount = 30;
			NavalForceStartNavalMissionCampaignBehavior.ResetShipHullsToDefault();
		}

		// Token: 0x060017BF RID: 6079 RVA: 0x000A1F94 File Offset: 0x000A0194
		private static void ResetShipHullsToDefault()
		{
			NavalForceStartNavalMissionCampaignBehavior._shipHullIds[0].Clear();
			NavalForceStartNavalMissionCampaignBehavior._shipHullIds[0].AddRange(NavalForceStartNavalMissionCampaignBehavior._defaultShipHullIds);
			NavalForceStartNavalMissionCampaignBehavior._shipHullIds[1].Clear();
			NavalForceStartNavalMissionCampaignBehavior._shipHullIds[2].Clear();
			NavalForceStartNavalMissionCampaignBehavior._shipHullIds[2].AddRange(NavalForceStartNavalMissionCampaignBehavior._defaultShipHullIds);
		}

		// Token: 0x060017C0 RID: 6080 RVA: 0x000A1FE7 File Offset: 0x000A01E7
		[CommandLineFunctionality.CommandLineArgumentFunction("get_mission_settings", "naval")]
		public static string GetMissionSettings(List<string> strings)
		{
			return NavalForceStartNavalMissionCampaignBehavior.GetMissionSettings();
		}

		// Token: 0x060017C1 RID: 6081 RVA: 0x000A1FEE File Offset: 0x000A01EE
		[CommandLineFunctionality.CommandLineArgumentFunction("reset_mission_settings", "naval")]
		public static string ResetMissionSettings(List<string> strings)
		{
			NavalForceStartNavalMissionCampaignBehavior.ResetMissionSettings();
			return "Mission settings reset successfully.\n" + NavalForceStartNavalMissionCampaignBehavior.GetMissionSettings();
		}

		// Token: 0x060017C2 RID: 6082 RVA: 0x000A2004 File Offset: 0x000A0204
		[CommandLineFunctionality.CommandLineArgumentFunction("set_mission_scene", "naval")]
		public static string SetMissionScene(List<string> strings)
		{
			if (strings.Count == 1)
			{
				NavalForceStartNavalMissionCampaignBehavior._sceneName = strings[0];
				return "Mission scene is set to " + NavalForceStartNavalMissionCampaignBehavior._sceneName;
			}
			return "usage: naval.set_mission_scene [SceneName]";
		}

		// Token: 0x060017C3 RID: 6083 RVA: 0x000A2030 File Offset: 0x000A0230
		[CommandLineFunctionality.CommandLineArgumentFunction("set_mission_ships", "naval")]
		public static string SetMissionShips(List<string> strings)
		{
			bool flag = false;
			string text = "";
			TeamSideEnum teamSideEnum = -1;
			if (strings.Count == 0)
			{
				text += "Invalid number of arguments provided\n";
				flag = true;
			}
			if (strings.Count == 1)
			{
				string text2 = strings[0];
				if (text2.ToLower() == "help")
				{
					flag = true;
				}
				else if (text2.ToLower() == "default")
				{
					teamSideEnum = 3;
					NavalForceStartNavalMissionCampaignBehavior.ResetShipHullsToDefault();
				}
				else
				{
					text += "Unable to parse single parameter argument.\nFor single parameter calls, the parameter must either be \"default\" or \"help\"\n";
					flag = true;
				}
			}
			else
			{
				string text3 = strings[0].ToLower();
				if (text3 == "player" || text3 == "playerTeam")
				{
					teamSideEnum = 0;
				}
				else if (text3 == "playerAlly" || text3 == "playerAllyTeam")
				{
					teamSideEnum = 1;
				}
				else if (text3 == "enemy" || text3 == "enemyTeam")
				{
					teamSideEnum = 2;
				}
				if (TeamSideEnumExtensions.IsValid(teamSideEnum))
				{
					int num = teamSideEnum;
					MBList<string> mblist = NavalForceStartNavalMissionCampaignBehavior._shipHullIds[num];
					if (strings.Count == 2 && strings[1].ToLower() == "default")
					{
						mblist.Clear();
						mblist.AddRange(NavalForceStartNavalMissionCampaignBehavior._defaultShipHullIds);
					}
					else
					{
						mblist.Clear();
						int num2 = strings.Count - 1;
						if (num2 > 8)
						{
							text += "At most 8 ships hull ids can be passed as parameter\n";
							num2 = 8;
						}
						for (int i = 0; i < num2; i++)
						{
							string text4 = strings[i + 1];
							MBObjectManager instance = MBObjectManager.Instance;
							if (instance != null)
							{
								if (instance.GetObject<ShipHull>(text4) != null)
								{
									mblist.Add(text4);
								}
								else
								{
									text = text + "Passed ship hull id: " + text4 + " does not refer to a valid ship hull. Omitting this\n";
								}
							}
							else
							{
								mblist.Add(text4);
							}
						}
						if (Extensions.IsEmpty<string>(mblist))
						{
							text += "None of the passed ship hull ids refer to a valid ship hull\n";
							text = text + "Reverting to default ship hulls for " + teamSideEnum.ToString().ToLower() + "\n";
							if (teamSideEnum != 1)
							{
								mblist.AddRange(NavalForceStartNavalMissionCampaignBehavior._defaultShipHullIds);
							}
						}
					}
				}
				else
				{
					text += "Unable to parse team side argument\nIt must refer to a valid team side like \"player\",\"playerAlly\" or \"enemy\"\n";
					flag = true;
				}
			}
			if (flag)
			{
				text += "Mission will be loaded with the specified ship hulls for the given team\n\nUsage: naval.set_mission_ships [TeamSide] [ShipHullId0] [ShipHullId1] ...\n\n- TeamSide: is the side of the team for which starting ships will be changed.\n  Can be \"player\", \"playerAlly\" or \"enemy\"\n- ShipHullId(s): are the hull id(s) of the ships to be spawned for the given side.\n  These must exist in ShipHulls.xml file.\n\nRemarks: Passing \"default\" as the first parameter will reset ships to default for all teams\n          Passing \"default\" as the second parameter after the TeamSide parameter will set ships to default\n         for only the given team";
			}
			else if (teamSideEnum == 3)
			{
				text += "Player and enemy teams will start with their default ships:\n";
				int num3 = NavalForceStartNavalMissionCampaignBehavior._defaultShipHullIds.Count - 1;
				for (int j = 0; j < num3; j++)
				{
					text = text + NavalForceStartNavalMissionCampaignBehavior._defaultShipHullIds[j] + ", ";
				}
				text = text + NavalForceStartNavalMissionCampaignBehavior._defaultShipHullIds[num3] + "\n";
			}
			else if (TeamSideEnumExtensions.IsValid(teamSideEnum))
			{
				int num4 = teamSideEnum;
				text = text + teamSideEnum.ToString() + " will use the following ships:\n";
				MBList<string> mblist2 = NavalForceStartNavalMissionCampaignBehavior._shipHullIds[num4];
				int num5 = mblist2.Count - 1;
				for (int k = 0; k < num5; k++)
				{
					text = text + mblist2[k] + ", ";
				}
				text = text + mblist2[num5] + "\n";
			}
			return text;
		}

		// Token: 0x060017C4 RID: 6084 RVA: 0x000A2338 File Offset: 0x000A0538
		[CommandLineFunctionality.CommandLineArgumentFunction("set_maximize_troop_counts", "naval")]
		public static string SetMaximizeTroopCounts(List<string> strings)
		{
			bool flag = false;
			string text = "";
			if (strings.Count == 1)
			{
				if (strings[0].ToLower() == "help")
				{
					flag = true;
				}
				else if (strings[0] == "1" || strings[0] == "0")
				{
					NavalForceStartNavalMissionCampaignBehavior._maximizeTroopCounts = strings[0] == "1";
				}
				else
				{
					text = "Unable to parse parameter.\n";
					flag = true;
				}
			}
			else
			{
				NavalForceStartNavalMissionCampaignBehavior._maximizeTroopCounts = !NavalForceStartNavalMissionCampaignBehavior._maximizeTroopCounts;
			}
			if (flag)
			{
				text += "\nIf set, mission will start with all ships having maximum number of troops\nusage: naval.set_maximize_troop_counts [value]\n- value: If passed 1 setting is enabled, if passed 0 it is disabled. Omitting the parameter toggles the setting";
			}
			else if (NavalForceStartNavalMissionCampaignBehavior._maximizeTroopCounts)
			{
				text += "Troops counts will be maximized in next mission";
			}
			else
			{
				text = string.Concat(new object[]
				{
					text,
					"Troops counts will be specified manually in next mission\n- Player Melee Troop Count:",
					NavalForceStartNavalMissionCampaignBehavior._playerMeleeTroopCount,
					"\n- Player Ranged Troop Count:",
					NavalForceStartNavalMissionCampaignBehavior._playerRangedTroopCount,
					"\n- Enemy Melee Troop Count:",
					NavalForceStartNavalMissionCampaignBehavior._enemyMeleeTroopCount,
					"\n- Enemy Ranged Troop Count:",
					NavalForceStartNavalMissionCampaignBehavior._enemyRangedTroopCount
				});
			}
			return text;
		}

		// Token: 0x060017C5 RID: 6085 RVA: 0x000A2454 File Offset: 0x000A0654
		[CommandLineFunctionality.CommandLineArgumentFunction("set_mission_troop_counts", "naval")]
		public static string SetMissionTroopCounts(List<string> strings)
		{
			string text = "";
			bool flag = false;
			if (strings.Count == 1 && strings[0].ToLower() == "help")
			{
				flag = true;
			}
			else if (strings.Count == 4 && int.TryParse(strings[0] ?? "error", out NavalForceStartNavalMissionCampaignBehavior._playerMeleeTroopCount) && int.TryParse(strings[1] ?? "error", out NavalForceStartNavalMissionCampaignBehavior._playerRangedTroopCount) && int.TryParse(strings[2] ?? "error", out NavalForceStartNavalMissionCampaignBehavior._enemyMeleeTroopCount) && int.TryParse(strings[3] ?? "error", out NavalForceStartNavalMissionCampaignBehavior._enemyRangedTroopCount))
			{
				if (NavalForceStartNavalMissionCampaignBehavior._maximizeTroopCounts)
				{
					NavalForceStartNavalMissionCampaignBehavior._maximizeTroopCounts = false;
					text += "Troop count maximization disabled\n";
				}
				text = string.Concat(new object[]
				{
					text,
					"Mission troop counts are successfully set.\n- Player Melee Troop Count:",
					NavalForceStartNavalMissionCampaignBehavior._playerMeleeTroopCount,
					"\n- Player Ranged Troop Count:",
					NavalForceStartNavalMissionCampaignBehavior._playerRangedTroopCount,
					"\n- Enemy Melee Troop Count:",
					NavalForceStartNavalMissionCampaignBehavior._enemyMeleeTroopCount,
					"\n- Enemy Ranged Troop Count:",
					NavalForceStartNavalMissionCampaignBehavior._enemyRangedTroopCount
				});
			}
			else
			{
				text += "Unable to parse one or more of the parameters.\n";
				flag = true;
			}
			if (flag)
			{
				text += "usage: naval.set_mission_troop_counts [PlayerMeleeTroopCount] [PlayerRangedTroopCount] [EnemyMeleeTroopCount] [EnemyRangedTroopCount]";
			}
			return text;
		}

		// Token: 0x060017C6 RID: 6086 RVA: 0x000A25BC File Offset: 0x000A07BC
		[CommandLineFunctionality.CommandLineArgumentFunction("start_mission", "naval")]
		public static string StartMission(List<string> strings)
		{
			if (!NavalForceStartNavalMissionCampaignBehavior._forceStartNavalMission)
			{
				NavalForceStartNavalMissionCampaignBehavior._forceStartNavalMission = true;
				ShipDeploymentModel.IgnoreDeploymentLimits = true;
				if (GameStateManager.Current.ActiveState is InitialState)
				{
					Module.CurrentModule.ExecuteInitialStateOptionWithId("SandBoxNewGame");
				}
				else
				{
					ModuleInfo moduleInfo = ModuleHelper.GetModuleInfo("NavalDLC");
					if (moduleInfo == null || !moduleInfo.IsActive)
					{
						NavalForceStartNavalMissionCampaignBehavior._forceStartNavalMission = false;
						return "Naval DLC module isn't active!";
					}
					Campaign.Current.TimeControlMode = 2;
				}
			}
			return "Starting mission with current mission settings...\n" + NavalForceStartNavalMissionCampaignBehavior.GetMissionSettings();
		}

		// Token: 0x060017C8 RID: 6088 RVA: 0x000A2648 File Offset: 0x000A0848
		// Note: this type is marked as 'beforefieldinit'.
		static NavalForceStartNavalMissionCampaignBehavior()
		{
			MBList<string> mblist = new MBList<string>();
			mblist.Add("northern_trade_ship");
			mblist.Add("nord_medium_ship");
			mblist.Add("vlandia_heavy_ship");
			NavalForceStartNavalMissionCampaignBehavior._defaultShipHullIds = mblist;
			NavalForceStartNavalMissionCampaignBehavior._shipHullIds = new MBList<string>[]
			{
				new MBList<string>(NavalForceStartNavalMissionCampaignBehavior._defaultShipHullIds),
				new MBList<string>(),
				new MBList<string>(NavalForceStartNavalMissionCampaignBehavior._defaultShipHullIds)
			};
		}

		// Token: 0x04000BDC RID: 3036
		private static bool _forceStartNavalMission = false;

		// Token: 0x04000BDD RID: 3037
		private const string DefaultTestSceneName = "battle_terrain_opensea_northern";

		// Token: 0x04000BDE RID: 3038
		private static string _sceneName = "battle_terrain_opensea_northern";

		// Token: 0x04000BDF RID: 3039
		private static int _enemyMeleeTroopCount = 30;

		// Token: 0x04000BE0 RID: 3040
		private static int _enemyRangedTroopCount = 30;

		// Token: 0x04000BE1 RID: 3041
		private static int _playerMeleeTroopCount = 30;

		// Token: 0x04000BE2 RID: 3042
		private static int _playerRangedTroopCount = 30;

		// Token: 0x04000BE3 RID: 3043
		private static bool _maximizeTroopCounts = true;

		// Token: 0x04000BE4 RID: 3044
		private static MBList<string> _defaultShipHullIds;

		// Token: 0x04000BE5 RID: 3045
		private static MBList<string>[] _shipHullIds;

		// Token: 0x04000BE6 RID: 3046
		private PartyBase _enemyParty;
	}
}
