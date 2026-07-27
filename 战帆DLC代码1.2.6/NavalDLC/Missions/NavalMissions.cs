using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using NavalDLC.Missions.Deployment;
using NavalDLC.Missions.Handlers;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Storyline;
using NavalDLC.Storyline.MissionControllers;
using SandBox;
using SandBox.Conversation.MissionLogics;
using SandBox.Missions;
using SandBox.Missions.MissionLogics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.CampaignSystem.TroopSuppliers;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.MissionLogics;
using TaleWorlds.MountAndBlade.Source.Missions;
using TaleWorlds.MountAndBlade.Source.Missions.Handlers;
using TaleWorlds.MountAndBlade.Source.Missions.Handlers.Logic;

namespace NavalDLC.Missions
{
	// Token: 0x02000084 RID: 132
	[MissionManager]
	public static class NavalMissions
	{
		// Token: 0x0600099C RID: 2460 RVA: 0x0004472C File Offset: 0x0004292C
		[MissionMethod]
		public static Mission OpenNavalBattleMission(MissionInitializerRecord rec)
		{
			MobileParty mainParty = MobileParty.MainParty;
			MapEvent mapEvent = mainParty.MapEvent;
			bool isPlayerSergeant = mapEvent.IsPlayerSergeant();
			bool isPlayerInArmy = mainParty.Army != null;
			bool isPlayerAttacker = !Extensions.IsEmpty<MapEventParty>(mapEvent.AttackerSide.Parties.Where<MapEventParty>((MapEventParty p) => p.Party == mainParty.Party));
			rec.AtmosphereOnCampaign.NauticalInfo.UsesNavalSimulatedWater = 1;
			Mission mission2 = NavalMissionState.OpenNew("NavalBattle", rec, delegate(Mission mission)
			{
				IMissionTroopSupplier[] array = new IMissionTroopSupplier[]
				{
					new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, 0, null, null),
					new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, 1, null, null)
				};
				BattleSideEnum playerSide = mapEvent.PlayerSide;
				BattleSideEnum otherSide = mapEvent.GetOtherSide(playerSide);
				MBReadOnlyList<MapEventParty> parties = mapEvent.GetMapEventSide(playerSide).Parties;
				MapEventParty mapEventParty;
				MBList<MapEventParty> mblist;
				MBList<MapEventParty> mblist2;
				NavalDLCManager.Instance.GameModels.ShipDeploymentModel.GetMapEventPartiesOfPlayerTeams(parties, isPlayerSergeant, out mapEventParty, out mblist, out mblist2);
				NavalShipDeploymentLimit navalShipDeploymentLimit;
				NavalShipDeploymentLimit navalShipDeploymentLimit2;
				NavalDLCManager.Instance.GameModels.ShipDeploymentModel.GetShipDeploymentLimitsOfPlayerTeams(mblist, mblist2, out navalShipDeploymentLimit, out navalShipDeploymentLimit2);
				MBList<IShipOrigin> mblist3 = new MBList<IShipOrigin>();
				Ship suitablePlayerShip = NavalDLCManager.Instance.GameModels.ShipDeploymentModel.GetSuitablePlayerShip(mapEventParty, mblist);
				mblist3.Add(suitablePlayerShip);
				NavalDLCManager.Instance.GameModels.ShipDeploymentModel.FillShipsOfTeamParties(mblist, navalShipDeploymentLimit, mblist3);
				List<string> list;
				NavalDLCManager.Instance.GameModels.ShipDeploymentModel.GetOrderedCaptainsForPlayerTeamShips(mblist, mblist3, out list);
				MBList<IShipOrigin> mblist4 = new MBList<IShipOrigin>();
				if (!Extensions.IsEmpty<MapEventParty>(mblist2))
				{
					NavalDLCManager.Instance.GameModels.ShipDeploymentModel.FillShipsOfTeamParties(mblist2, navalShipDeploymentLimit2, mblist4);
				}
				MBList<MapEventParty> mblist5 = Extensions.ToMBList<MapEventParty>(mapEvent.GetMapEventSide(otherSide).Parties);
				NavalShipDeploymentLimit teamShipDeploymentLimit = NavalDLCManager.Instance.GameModels.ShipDeploymentModel.GetTeamShipDeploymentLimit(mblist5);
				MBList<IShipOrigin> mblist6 = new MBList<IShipOrigin>();
				NavalDLCManager.Instance.GameModels.ShipDeploymentModel.FillShipsOfTeamParties(mblist5, teamShipDeploymentLimit, mblist6);
				int num = MathF.Min(mblist3.Count, navalShipDeploymentLimit.NetDeploymentLimit);
				int maximumDeployableTroopCountForTeam = NavalDLCManager.Instance.GameModels.ShipDeploymentModel.GetMaximumDeployableTroopCountForTeam(mblist3, true);
				int maximumDeployableTroopCountForTeam2 = NavalDLCManager.Instance.GameModels.ShipDeploymentModel.GetMaximumDeployableTroopCountForTeam(mblist4, false);
				int maximumDeployableTroopCountForTeam3 = NavalDLCManager.Instance.GameModels.ShipDeploymentModel.GetMaximumDeployableTroopCountForTeam(mblist6, false);
				int[] array2 = new int[] { maximumDeployableTroopCountForTeam, maximumDeployableTroopCountForTeam2, maximumDeployableTroopCountForTeam3 };
				return new MissionBehavior[]
				{
					new NavalShipsLogic(),
					new NavalFloatsamLogic(),
					new NavalAgentsLogic(),
					new DefaultNavalMissionLogic(mblist3, mblist4, mblist6, navalShipDeploymentLimit, navalShipDeploymentLimit2, teamShipDeploymentLimit),
					new NavalTrajectoryPlanningLogic(),
					new DefaultNavalMissionAgentSpawnLogic(array, playerSide, num, array2),
					new NavalMissionDeploymentPlanningLogic(mission),
					new BattlePowerCalculationLogic(),
					new NavalBattleAgentLogic(),
					new WaveParametersComputerLogic(),
					new MissionOptionsComponent(),
					new CampaignMissionComponent(),
					new NavalAgentMoraleInteractionLogic(),
					new NavalBattleEndLogic(),
					new NavalMissionCombatantsLogic(MobileParty.MainParty.MapEvent.InvolvedParties, PartyBase.MainParty, MobileParty.MainParty.MapEvent.GetLeaderParty(0), MobileParty.MainParty.MapEvent.GetLeaderParty(1), 4, isPlayerSergeant),
					new BattleObserverMissionLogic(),
					new AgentHumanAILogic(),
					new AgentVictoryLogic(),
					new ShipCollisionOutcomeLogic(mission),
					new ShipRetreatLogic(),
					new NavalBoundaryForceFieldLogic(),
					new BattleMissionAgentInteractionLogic(),
					new NavalAssignPlayerRoleInTeamMissionController(!isPlayerSergeant, isPlayerSergeant, isPlayerInArmy, list),
					new EquipmentControllerLeaveLogic(),
					new MissionHardBorderPlacer(),
					new MissionBoundaryPlacer(),
					new MissionBoundaryCrossingHandler(30f),
					new HighlightsController(),
					new BattleHighlightsController(),
					new NavalDeploymentMissionController(isPlayerAttacker),
					new NavalDeploymentHandler(isPlayerAttacker)
				};
			}, true, true);
			mission2.SetPlayerCanTakeControlOfAnotherAgentWhenDead();
			return mission2;
		}

		// Token: 0x0600099D RID: 2461 RVA: 0x000447E0 File Offset: 0x000429E0
		[MissionMethod]
		public static Mission OpenNavalRaidMission(TroopRoster navalRaidTroops, BattleSideEnum navalSide, List<Ship> allShips)
		{
			Settlement mapEventSettlement = PlayerEncounter.Battle.MapEventSettlement;
			string scene = mapEventSettlement.LocationComplex.GetScene("village_center", 1);
			MissionInitializerRecord missionInitializerRecord;
			missionInitializerRecord..ctor(scene);
			missionInitializerRecord.TerrainType = 11;
			missionInitializerRecord.DamageToFriendsMultiplier = Campaign.Current.Models.DifficultyModel.GetPlayerTroopsReceivedDamageMultiplier();
			missionInitializerRecord.DamageFromPlayerToFriendsMultiplier = Campaign.Current.Models.DifficultyModel.GetPlayerTroopsReceivedDamageMultiplier();
			missionInitializerRecord.NeedsRandomTerrain = false;
			missionInitializerRecord.PlayingInCampaignMode = true;
			missionInitializerRecord.AtmosphereOnCampaign = Campaign.Current.Models.MapWeatherModel.GetAtmosphereModel(mapEventSettlement.Position);
			missionInitializerRecord.SceneHasMapPatch = false;
			missionInitializerRecord.DecalAtlasGroup = 2;
			MissionInitializerRecord missionInitializerRecord2 = missionInitializerRecord;
			missionInitializerRecord2.AtmosphereOnCampaign.NauticalInfo.UsesNavalSimulatedWater = 1;
			missionInitializerRecord2.SceneLevels = "naval_raid";
			MBList<IShipOrigin> navalSideShips = new MBList<IShipOrigin>();
			foreach (Ship ship in allShips)
			{
				navalSideShips.Add(ship);
			}
			Mission mission2 = NavalMissionState.OpenNew("NavalRaid", missionInitializerRecord2, delegate(Mission mission)
			{
				MapEvent mapEvent = MobileParty.MainParty.MapEvent;
				BattleSideEnum otherSide = mapEvent.GetOtherSide(navalSide);
				IMissionTroopSupplier[] array = new IMissionTroopSupplier[2];
				array[otherSide] = new PartyGroupTroopSupplier(mapEvent, otherSide, null, null);
				array[navalSide] = new PartyGroupTroopSupplier(mapEvent, navalSide, navalRaidTroops.ToFlattenedRoster(), null);
				List<string> list;
				NavalDLCManager.Instance.GameModels.ShipDeploymentModel.GetOrderedCaptainsForPlayerTeamShips(mapEvent.PartiesOnSide(navalSide), navalSideShips, out list);
				int totalManCount = navalRaidTroops.TotalManCount;
				int totalHealthyTroopCountOfSide = mapEvent.GetMapEventSide(otherSide).GetTotalHealthyTroopCountOfSide();
				bool flag = navalSide == 1;
				bool flag2 = mapEvent.PlayerSide == 1;
				int num;
				int num2;
				NavalRaidMissionAgentSpawnLogic.ComputeInitialTroopCounts(flag ? totalManCount : totalHealthyTroopCountOfSide, flag ? totalHealthyTroopCountOfSide : totalManCount, out num, out num2);
				return new MissionBehavior[]
				{
					new NavalShipsLogic(),
					new NavalFloatsamLogic(),
					new NavalAgentsLogic(),
					new NavalRaidMissionController(),
					new NavalRaidMissionAgentSpawnLogic(array, mapEvent.PlayerSide, navalSideShips, new NavalShipDeploymentLimit(navalSideShips.Count), num, num2),
					new NavalTrajectoryPlanningLogic(),
					new NavalRaidMissionDeploymentPlanningLogic(),
					new BattlePowerCalculationLogic(),
					new NavalBattleAgentLogic(),
					new WaveParametersComputerLogic(),
					new MissionOptionsComponent(),
					new CampaignMissionComponent(),
					new NavalAgentMoraleInteractionLogic(),
					new BattleEndLogic(),
					new NavalMissionCombatantsLogic(mapEvent.InvolvedParties, PartyBase.MainParty, flag ? mapEvent.GetLeaderParty(otherSide) : mapEvent.GetLeaderParty(navalSide), flag ? mapEvent.GetLeaderParty(navalSide) : mapEvent.GetLeaderParty(otherSide), 5, mapEvent.IsPlayerSergeant()),
					new BattleObserverMissionLogic(),
					new AgentHumanAILogic(),
					new AgentVictoryLogic(),
					new ShipCollisionOutcomeLogic(mission),
					new NavalBoundaryForceFieldLogic(),
					new BattleMissionAgentInteractionLogic(),
					new NavalAssignPlayerRoleInTeamMissionController(!mapEvent.IsPlayerSergeant(), mapEvent.IsPlayerSergeant(), MobileParty.MainParty.Army != null, list),
					new EquipmentControllerLeaveLogic(),
					new MissionHardBorderPlacer(),
					new MissionBoundaryPlacer(),
					new MissionBoundaryCrossingHandler(30f),
					new HighlightsController(),
					new BattleHighlightsController(),
					new NavalRaidDeploymentMissionController(flag2),
					new NavalRaidDeploymentHandler(flag2)
				};
			}, true, true);
			mission2.SetPlayerCanTakeControlOfAnotherAgentWhenDead();
			return mission2;
		}

		// Token: 0x0600099E RID: 2462 RVA: 0x0004493C File Offset: 0x00042B3C
		[MissionMethod]
		public static Mission OpenNavalSetPieceBattleMission(MissionInitializerRecord rec, MBList<IShipOrigin> playerShips, MBList<IShipOrigin> playerAllyShips, MBList<IShipOrigin> enemyShips)
		{
			bool isPlayerSergeant = MobileParty.MainParty.MapEvent.IsPlayerSergeant();
			bool isPlayerInArmy = MobileParty.MainParty.Army != null;
			List<string> heroesOnPlayerSideByPriority = HeroHelper.OrderHeroesOnPlayerSideByPriority(false, false);
			bool isPlayerAttacker = !Extensions.IsEmpty<MapEventParty>(MobileParty.MainParty.MapEvent.AttackerSide.Parties.Where<MapEventParty>((MapEventParty p) => p.Party == MobileParty.MainParty.Party));
			rec.AtmosphereOnCampaign.NauticalInfo.UsesNavalSimulatedWater = 1;
			return NavalMissionState.OpenNew("NavalBattle", rec, delegate(Mission mission)
			{
				IMissionTroopSupplier[] array = new IMissionTroopSupplier[]
				{
					new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, 0, null, null),
					new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, 1, null, null)
				};
				BattleSideEnum playerSide = MobileParty.MainParty.MapEvent.PlayerSide;
				NavalShipDeploymentLimit navalShipDeploymentLimit = NavalShipDeploymentLimit.Max();
				NavalShipDeploymentLimit navalShipDeploymentLimit2 = NavalShipDeploymentLimit.Max();
				NavalShipDeploymentLimit navalShipDeploymentLimit3 = NavalShipDeploymentLimit.Max();
				int num = MathF.Min(playerShips.Count, NavalShipDeploymentLimit.Max().NetDeploymentLimit);
				int maximumDeployableTroopCountForTeam = NavalDLCManager.Instance.GameModels.ShipDeploymentModel.GetMaximumDeployableTroopCountForTeam(playerShips, true);
				int maximumDeployableTroopCountForTeam2 = NavalDLCManager.Instance.GameModels.ShipDeploymentModel.GetMaximumDeployableTroopCountForTeam(playerAllyShips, false);
				int maximumDeployableTroopCountForTeam3 = NavalDLCManager.Instance.GameModels.ShipDeploymentModel.GetMaximumDeployableTroopCountForTeam(enemyShips, false);
				int[] array2 = new int[] { maximumDeployableTroopCountForTeam, maximumDeployableTroopCountForTeam2, maximumDeployableTroopCountForTeam3 };
				return new MissionBehavior[]
				{
					new NavalShipsLogic(),
					new NavalFloatsamLogic(),
					new NavalAgentsLogic(),
					new DefaultNavalMissionLogic(playerShips, playerAllyShips, enemyShips, navalShipDeploymentLimit, navalShipDeploymentLimit2, navalShipDeploymentLimit3),
					new NavalTrajectoryPlanningLogic(),
					new DefaultNavalMissionAgentSpawnLogic(array, playerSide, num, array2),
					new NavalMissionDeploymentPlanningLogic(mission),
					new BattlePowerCalculationLogic(),
					new NavalBattleAgentLogic(),
					new WaveParametersComputerLogic(),
					new MissionOptionsComponent(),
					new CampaignMissionComponent(),
					new NavalBattleEndLogic(),
					new NavalMissionCombatantsLogic(MobileParty.MainParty.MapEvent.InvolvedParties, PartyBase.MainParty, MobileParty.MainParty.MapEvent.GetLeaderParty(0), MobileParty.MainParty.MapEvent.GetLeaderParty(1), 4, isPlayerSergeant),
					new BattleObserverMissionLogic(),
					new AgentHumanAILogic(),
					new AgentVictoryLogic(),
					new ShipCollisionOutcomeLogic(mission),
					new BattleMissionAgentInteractionLogic(),
					new NavalAssignPlayerRoleInTeamMissionController(!isPlayerSergeant, isPlayerSergeant, isPlayerInArmy, heroesOnPlayerSideByPriority),
					new EquipmentControllerLeaveLogic(),
					new MissionHardBorderPlacer(),
					new MissionBoundaryPlacer(),
					new MissionBoundaryCrossingHandler(30f),
					new HighlightsController(),
					new BattleHighlightsController(),
					new NavalDeploymentMissionController(isPlayerAttacker),
					new NavalDeploymentHandler(isPlayerAttacker)
				};
			}, true, true);
		}

		// Token: 0x0600099F RID: 2463 RVA: 0x00044A0C File Offset: 0x00042C0C
		[MissionMethod]
		public static Mission OpenBlockedEstuaryMission(MissionInitializerRecord rec, MobileParty enemyParty, bool startFromCheckPoint)
		{
			NavalStorylineData.SetNavalStorylineSetPieceBattleMissionType(NavalStorylineData.NavalStorylineSetPieceBattleMissionTypes.Act3Quest4);
			bool isPlayerSergeant = MobileParty.MainParty.MapEvent.IsPlayerSergeant();
			bool isPlayerInArmy = MobileParty.MainParty.Army != null;
			List<string> heroesOnPlayerSideByPriority = HeroHelper.OrderHeroesOnPlayerSideByPriority(false, false);
			Extensions.IsEmpty<MapEventParty>(MobileParty.MainParty.MapEvent.AttackerSide.Parties.Where<MapEventParty>((MapEventParty p) => p.Party == MobileParty.MainParty.Party));
			rec.AtmosphereOnCampaign.NauticalInfo.UsesNavalSimulatedWater = 1;
			return NavalMissionState.OpenNew("BlockedEstuary", rec, delegate(Mission mission)
			{
				IMissionTroopSupplier[] array = new IMissionTroopSupplier[]
				{
					new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, 0, null, null),
					new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, 1, null, null)
				};
				BattleSideEnum playerSide = MobileParty.MainParty.MapEvent.PlayerSide;
				return new MissionBehavior[]
				{
					new NavalShipsLogic(),
					new NavalFloatsamLogic(),
					new NavalAgentsLogic(),
					new NavalTrajectoryPlanningLogic(),
					new DefaultNavalMissionAgentSpawnLogic(array, playerSide, 0, null),
					new BattlePowerCalculationLogic(),
					new NavalBattleAgentLogic(),
					new MissionOptionsComponent(),
					new CampaignMissionComponent(),
					new BlockedEstuaryMissionController(enemyParty, startFromCheckPoint),
					new BlockedEstuaryBattleEndLogic(),
					new NavalMissionCombatantsLogic(MobileParty.MainParty.MapEvent.InvolvedParties, PartyBase.MainParty, MobileParty.MainParty.MapEvent.GetLeaderParty(0), MobileParty.MainParty.MapEvent.GetLeaderParty(1), 4, isPlayerSergeant),
					new BattleObserverMissionLogic(),
					new AgentHumanAILogic(),
					new AgentVictoryLogic(),
					new ShipCollisionOutcomeLogic(mission),
					new MissionObjectiveLogic(),
					new BattleMissionAgentInteractionLogic(),
					new NavalAssignPlayerRoleInTeamMissionController(!isPlayerSergeant, isPlayerSergeant, isPlayerInArmy, heroesOnPlayerSideByPriority),
					new EquipmentControllerLeaveLogic(),
					new MissionHardBorderPlacer(),
					new MissionBoundaryPlacer(),
					new MissionBoundaryCrossingHandler(30f),
					new HighlightsController(),
					new BattleHighlightsController()
				};
			}, true, true);
		}

		// Token: 0x060009A0 RID: 2464 RVA: 0x00044AD4 File Offset: 0x00042CD4
		[MissionMethod]
		public static Mission OpenNavalStorylineCaptivityMission(MissionInitializerRecord rec, CharacterObject allyCharacter, CharacterObject enemyCharacter, CharacterObject crewCharacter)
		{
			NavalStorylineData.SetNavalStorylineSetPieceBattleMissionType(NavalStorylineData.NavalStorylineSetPieceBattleMissionTypes.Act1);
			bool isPlayerSergeant = MobileParty.MainParty.MapEvent.IsPlayerSergeant();
			Army army = MobileParty.MainParty.Army;
			HeroHelper.OrderHeroesOnPlayerSideByPriority(false, false);
			rec.AtmosphereOnCampaign.NauticalInfo.UsesNavalSimulatedWater = 1;
			return NavalMissionState.OpenNew("NavalCaptivityBattle", rec, delegate(Mission mission)
			{
				IMissionTroopSupplier[] array = new IMissionTroopSupplier[]
				{
					new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, 0, null, null),
					new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, 1, null, null)
				};
				BattleSideEnum playerSide = MobileParty.MainParty.MapEvent.PlayerSide;
				BattleSideEnum otherSide = MobileParty.MainParty.MapEvent.GetOtherSide(playerSide);
				List<IShipOrigin> list = new MBList<IShipOrigin>();
				MBList<IShipOrigin> mblist = new MBList<IShipOrigin>();
				MBList<IShipOrigin> mblist2 = new MBList<IShipOrigin>();
				list.AddRange(MobileParty.MainParty.Ships);
				foreach (MapEventParty mapEventParty in MobileParty.MainParty.MapEvent.GetMapEventSide(playerSide).Parties)
				{
					if (mapEventParty.IsNpcParty)
					{
						mblist.AddRange(mapEventParty.Party.Ships);
					}
				}
				foreach (MapEventParty mapEventParty2 in MobileParty.MainParty.MapEvent.GetMapEventSide(otherSide).Parties)
				{
					mblist2.AddRange(mapEventParty2.Party.Ships);
				}
				return new MissionBehavior[]
				{
					new NavalShipsLogic(),
					new NavalFloatsamLogic(),
					new NavalAgentsLogic(),
					new NavalStorylineCaptivityMissionController(allyCharacter, enemyCharacter, crewCharacter),
					new MissionHintLogic(),
					new NavalTrajectoryPlanningLogic(),
					new NavalBattleAgentLogic(),
					new VisualTrackerMissionBehavior(),
					new MissionFightHandler(),
					new WaveParametersComputerLogic(),
					new MissionObjectiveLogic(),
					new MissionOptionsComponent(),
					new CampaignMissionComponent(),
					new MissionCombatantsLogic(MobileParty.MainParty.MapEvent.InvolvedParties, PartyBase.MainParty, MobileParty.MainParty.MapEvent.GetLeaderParty(0), MobileParty.MainParty.MapEvent.GetLeaderParty(1), 4, isPlayerSergeant),
					new AgentHumanAILogic(),
					new BattleMissionAgentInteractionLogic(),
					new EquipmentControllerLeaveLogic(),
					new MissionHardBorderPlacer(),
					new MissionBoundaryPlacer(),
					new HighlightsController(),
					new BattleHighlightsController()
				};
			}, true, true);
		}

		// Token: 0x060009A1 RID: 2465 RVA: 0x00044B58 File Offset: 0x00042D58
		[MissionMethod]
		public static Mission OpenNavalStorylinePirateBattleMission(MissionInitializerRecord rec, MobileParty pirateParty, int pirateTroopCount)
		{
			NavalStorylineData.SetNavalStorylineSetPieceBattleMissionType(NavalStorylineData.NavalStorylineSetPieceBattleMissionTypes.Act2);
			bool isPlayerSergeant = MobileParty.MainParty.MapEvent.IsPlayerSergeant();
			rec.AtmosphereOnCampaign.NauticalInfo.UsesNavalSimulatedWater = 1;
			return NavalMissionState.OpenNew("NavalStorylinePirateBattle", rec, delegate(Mission mission)
			{
				IMissionTroopSupplier[] array = new IMissionTroopSupplier[]
				{
					new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, 0, null, null),
					new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, 1, null, null)
				};
				BattleSideEnum playerSide = MobileParty.MainParty.MapEvent.PlayerSide;
				BattleSideEnum otherSide = MobileParty.MainParty.MapEvent.GetOtherSide(playerSide);
				List<IShipOrigin> list = new MBList<IShipOrigin>();
				MBList<IShipOrigin> mblist = new MBList<IShipOrigin>();
				MBList<IShipOrigin> mblist2 = new MBList<IShipOrigin>();
				list.AddRange(MobileParty.MainParty.Ships);
				foreach (MapEventParty mapEventParty in MobileParty.MainParty.MapEvent.GetMapEventSide(playerSide).Parties)
				{
					if (mapEventParty.IsNpcParty)
					{
						mblist.AddRange(mapEventParty.Party.Ships);
					}
				}
				foreach (MapEventParty mapEventParty2 in MobileParty.MainParty.MapEvent.GetMapEventSide(otherSide).Parties)
				{
					mblist2.AddRange(mapEventParty2.Party.Ships);
				}
				return new MissionBehavior[]
				{
					new NavalShipsLogic(),
					new NavalFloatsamLogic(),
					new NavalAgentsLogic(),
					new NavalTrajectoryPlanningLogic(),
					new PirateBattleMissionController(pirateParty, pirateTroopCount),
					new NavalBattleAgentLogic(),
					new MissionFightHandler(),
					new WaveParametersComputerLogic(),
					new DefaultNavalMissionAgentSpawnLogic(array, playerSide, 0, null),
					new BattlePowerCalculationLogic(),
					new MissionOptionsComponent(),
					new CampaignMissionComponent(),
					new BattleObserverMissionLogic(),
					new NavalMissionCombatantsLogic(MobileParty.MainParty.MapEvent.InvolvedParties, PartyBase.MainParty, MobileParty.MainParty.MapEvent.GetLeaderParty(0), MobileParty.MainParty.MapEvent.GetLeaderParty(1), 4, isPlayerSergeant),
					new AgentHumanAILogic(),
					new BattleMissionAgentInteractionLogic(),
					new EquipmentControllerLeaveLogic(),
					new NavalAgentMoraleInteractionLogic(),
					new ShipCollisionOutcomeLogic(mission),
					new MissionObjectiveLogic(),
					new MissionHardBorderPlacer(),
					new MissionBoundaryPlacer(),
					new MissionBoundaryCrossingHandler(30f),
					new HighlightsController(),
					new BattleHighlightsController()
				};
			}, true, true);
		}

		// Token: 0x060009A2 RID: 2466 RVA: 0x00044BC0 File Offset: 0x00042DC0
		[MissionMethod]
		public static Mission OpenNavalStorylineQuest5SetPieceBattleMission(MissionInitializerRecord rec, MobileParty enemyParty, Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState lastHitCheckpoint = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase1Part1)
		{
			NavalStorylineData.SetNavalStorylineSetPieceBattleMissionType(NavalStorylineData.NavalStorylineSetPieceBattleMissionTypes.Act3Quest5);
			bool isPlayerSergeant = MobileParty.MainParty.MapEvent.IsPlayerSergeant();
			rec.AtmosphereOnCampaign.NauticalInfo.UsesNavalSimulatedWater = 1;
			return NavalMissionState.OpenNew("NavalStorylineQuest5SetPieceBattleMission", rec, delegate(Mission mission)
			{
				IMissionTroopSupplier[] array = new IMissionTroopSupplier[]
				{
					new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, 0, null, null),
					new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, 1, null, null)
				};
				BattleSideEnum playerSide = MobileParty.MainParty.MapEvent.PlayerSide;
				BattleSideEnum otherSide = MobileParty.MainParty.MapEvent.GetOtherSide(playerSide);
				List<IShipOrigin> list = new MBList<IShipOrigin>();
				MBList<IShipOrigin> mblist = new MBList<IShipOrigin>();
				MBList<IShipOrigin> mblist2 = new MBList<IShipOrigin>();
				list.AddRange(MobileParty.MainParty.Ships);
				foreach (MapEventParty mapEventParty in MobileParty.MainParty.MapEvent.GetMapEventSide(playerSide).Parties)
				{
					if (mapEventParty.IsNpcParty)
					{
						mblist.AddRange(mapEventParty.Party.Ships);
					}
				}
				foreach (MapEventParty mapEventParty2 in MobileParty.MainParty.MapEvent.GetMapEventSide(otherSide).Parties)
				{
					mblist2.AddRange(mapEventParty2.Party.Ships);
				}
				List<MissionBehavior> list2 = new List<MissionBehavior>();
				list2.Add(new NavalShipsLogic());
				list2.Add(new NavalFloatsamLogic());
				list2.Add(new NavalAgentsLogic());
				list2.Add(new MissionObjectiveLogic());
				list2.Add(new NavalTrajectoryPlanningLogic());
				list2.Add(new Quest5NavalMissionDeploymentPlanningLogic(mission));
				list2.Add(new Quest5SetPieceBattleMissionController(lastHitCheckpoint, enemyParty));
				list2.Add(new NavalBattleAgentLogic());
				list2.Add(new MissionFightHandler());
				list2.Add(new CosmeticShipSpawnMissionLogic());
				list2.Add(new LightScriptedFiresMissionController());
				list2.Add(new BattlePowerCalculationLogic());
				list2.Add(new MissionOptionsComponent());
				list2.Add(new CampaignMissionComponent());
				list2.Add(new Quest5BattleObserverMissionLogic());
				list2.Add(new MissionCombatantsLogic(MobileParty.MainParty.MapEvent.InvolvedParties, PartyBase.MainParty, MobileParty.MainParty.MapEvent.GetLeaderParty(0), MobileParty.MainParty.MapEvent.GetLeaderParty(1), 4, isPlayerSergeant));
				list2.Add(new AgentHumanAILogic());
				list2.Add(new EquipmentControllerLeaveLogic());
				list2.Add(new MissionConversationLogic());
				list2.Add(new MissionHardBorderPlacer());
				list2.Add(new MissionBoundaryPlacer());
				list2.Add(new MissionBoundaryCrossingHandler(30f));
				list2.Add(new HighlightsController());
				list2.Add(new BattleHighlightsController());
				list2.Add(new StealthPatrolPointMissionLogic());
				if (lastHitCheckpoint != Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase1Part1)
				{
					Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState lastHitCheckpoint2 = lastHitCheckpoint;
				}
				return list2;
			}, true, true);
		}

		// Token: 0x060009A3 RID: 2467 RVA: 0x00044C28 File Offset: 0x00042E28
		[MissionMethod]
		public static Mission OpenNavalFinalConversationMission()
		{
			int wallLevel = Settlement.CurrentSettlement.Town.GetWallLevel();
			string civilianUpgradeLevelTag = Campaign.Current.Models.LocationModel.GetCivilianUpgradeLevelTag(wallLevel);
			Location location = Settlement.CurrentSettlement.LocationComplex.GetLocationWithId("port");
			List<Ship> townLordShips = new List<Ship>();
			List<Ship> mainPartyShips = MobileParty.MainParty.Ships.ToList<Ship>();
			foreach (MobileParty mobileParty in Settlement.CurrentSettlement.Parties)
			{
				townLordShips.AddRange(mobileParty.Ships);
			}
			return MissionState.OpenNew("NavalFinalConversationMission", SandBoxMissions.CreateSandBoxMissionInitializerRecord(location.GetSceneName(wallLevel), civilianUpgradeLevelTag, true, 3), (Mission mission) => new MissionBehavior[]
			{
				new MissionOptionsComponent(),
				new CampaignMissionComponent(),
				new MissionBasicTeamLogic(),
				new BasicLeaveMissionLogic(),
				new LeaveMissionLogic("settlement_player_unconscious"),
				new SandBoxMissionHandler(),
				new MissionAgentLookHandler(),
				new MissionConversationLogic(),
				new MissionAgentHandler(),
				new MissionLocationLogic(location, null),
				new HeroSkillHandler(),
				new MissionFightHandler(),
				new BattleAgentLogic(),
				new MountAgentLogic(),
				new AgentHumanAILogic(),
				new MissionCrimeHandler(),
				new MissionFacialAnimationHandler(),
				new LocationItemSpawnHandler(),
				new IndoorMissionController(),
				new VisualTrackerMissionBehavior(),
				new EquipmentControllerLeaveLogic(),
				new BattleSurgeonLogic(),
				new CivilianPortShipSpawnMissionLogic(mainPartyShips, townLordShips)
			}, true, true);
		}

		// Token: 0x060009A4 RID: 2468 RVA: 0x00044D20 File Offset: 0x00042F20
		[MissionMethod]
		public static Mission OpenNavalStorylineWoundedBeastBattleMission(MissionInitializerRecord rec)
		{
			NavalStorylineData.SetNavalStorylineSetPieceBattleMissionType(NavalStorylineData.NavalStorylineSetPieceBattleMissionTypes.Act3Quest2);
			bool isPlayerSergeant = true;
			HeroHelper.OrderHeroesOnPlayerSideByPriority(false, false);
			IMissionTroopSupplier[] suppliers = new IMissionTroopSupplier[2];
			suppliers[0] = new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, 0, null, null);
			suppliers[1] = new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, 1, null, null);
			rec.AtmosphereOnCampaign.NauticalInfo.UsesNavalSimulatedWater = 1;
			BattleSideEnum playerSide = MobileParty.MainParty.MapEvent.PlayerSide;
			return NavalMissionState.OpenNew("NavalStorylineWoundedBeastBattle", rec, (Mission mission) => new MissionBehavior[]
			{
				new NavalShipsLogic(),
				new NavalFloatsamLogic(),
				new NavalAgentsLogic(),
				new MissionObjectiveLogic(),
				new WoundedBeastMissionController(),
				new BattleAgentLogic(),
				new MissionFightHandler(),
				new WaveParametersComputerLogic(),
				new DefaultNavalMissionAgentSpawnLogic(suppliers, playerSide, 0, null),
				new NavalTrajectoryPlanningLogic(),
				new BattlePowerCalculationLogic(),
				new MissionOptionsComponent(),
				new CampaignMissionComponent(),
				new BattleObserverMissionLogic(),
				new NavalMissionCombatantsLogic(MobileParty.MainParty.MapEvent.InvolvedParties, PartyBase.MainParty, MobileParty.MainParty.MapEvent.GetLeaderParty(0), MobileParty.MainParty.MapEvent.GetLeaderParty(1), 4, isPlayerSergeant),
				new AgentHumanAILogic(),
				new BattleMissionAgentInteractionLogic(),
				new EquipmentControllerLeaveLogic(),
				new NavalAgentMoraleInteractionLogic(),
				new ShipCollisionOutcomeLogic(mission),
				new AgentVictoryLogic(),
				new NavalBattleEndLogic(),
				new MissionHardBorderPlacer(),
				new MissionBoundaryPlacer(),
				new MissionBoundaryCrossingHandler(30f),
				new HighlightsController(),
				new BattleHighlightsController()
			}, true, true);
		}

		// Token: 0x060009A5 RID: 2469 RVA: 0x00044DC0 File Offset: 0x00042FC0
		[MissionMethod]
		public static Mission OpenHelpingAnAllySetPieceBattleMission(MissionInitializerRecord rec, MobileParty merchantParty, MobileParty seaHoundsParty)
		{
			NavalStorylineData.SetNavalStorylineSetPieceBattleMissionType(NavalStorylineData.NavalStorylineSetPieceBattleMissionTypes.Act3Quest1);
			bool isPlayerSergeant = MobileParty.MainParty.MapEvent.IsPlayerSergeant();
			rec.AtmosphereOnCampaign.NauticalInfo.UsesNavalSimulatedWater = 1;
			return NavalMissionState.OpenNew("HelpAnAllySetPieceBattle", rec, delegate(Mission mission)
			{
				IMissionTroopSupplier[] array = new IMissionTroopSupplier[]
				{
					new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, 0, null, null),
					new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, 1, null, null)
				};
				return new MissionBehavior[]
				{
					new NavalShipsLogic(),
					new NavalFloatsamLogic(),
					new NavalAgentsLogic(),
					new MissionObjectiveLogic(),
					new NavalTrajectoryPlanningLogic(),
					new HelpingAnAllySetPieceBattleMissionController(merchantParty, seaHoundsParty),
					new NavalBattleAgentLogic(),
					new BattlePowerCalculationLogic(),
					new MissionFightHandler(),
					new MissionOptionsComponent(),
					new CampaignMissionComponent(),
					new BattleObserverMissionLogic(),
					new NavalMissionCombatantsLogic(MobileParty.MainParty.MapEvent.InvolvedParties, PartyBase.MainParty, MobileParty.MainParty.MapEvent.GetLeaderParty(0), MobileParty.MainParty.MapEvent.GetLeaderParty(1), 4, isPlayerSergeant),
					new AgentHumanAILogic(),
					new BattleMissionAgentInteractionLogic(),
					new EquipmentControllerLeaveLogic(),
					new MissionHardBorderPlacer(),
					new MissionBoundaryPlacer(),
					new MissionBoundaryCrossingHandler(30f),
					new HighlightsController(),
					new BattleHighlightsController()
				};
			}, true, true);
		}

		// Token: 0x060009A6 RID: 2470 RVA: 0x00044E28 File Offset: 0x00043028
		[MissionMethod]
		public static Mission OpenFloatingFortressSetPieceBattleMission(MissionInitializerRecord rec, bool startFromCheckpoint)
		{
			NavalStorylineData.SetNavalStorylineSetPieceBattleMissionType(NavalStorylineData.NavalStorylineSetPieceBattleMissionTypes.Act3Quest4);
			bool isPlayerSergeant = MobileParty.MainParty.MapEvent.IsPlayerSergeant();
			rec.AtmosphereOnCampaign.NauticalInfo.UsesNavalSimulatedWater = 1;
			return NavalMissionState.OpenNew("FloatingFortressSetPieceBattleMission", rec, delegate(Mission mission)
			{
				IMissionTroopSupplier[] array = new IMissionTroopSupplier[]
				{
					new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, 0, null, null),
					new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, 1, null, null)
				};
				BattleSideEnum playerSide = MobileParty.MainParty.MapEvent.PlayerSide;
				return new MissionBehavior[]
				{
					new NavalShipsLogic(),
					new NavalFloatsamLogic(),
					new NavalAgentsLogic(),
					new NavalTrajectoryPlanningLogic(),
					new BattlePowerCalculationLogic(),
					new NavalBattleAgentLogic(),
					new MissionOptionsComponent(),
					new CampaignMissionComponent(),
					new BattleObserverMissionLogic(),
					new NavalMissionCombatantsLogic(MobileParty.MainParty.MapEvent.InvolvedParties, PartyBase.MainParty, MobileParty.MainParty.MapEvent.GetLeaderParty(0), MobileParty.MainParty.MapEvent.GetLeaderParty(1), 4, isPlayerSergeant),
					new FloatingFortressSetPieceBattleMissionController(startFromCheckpoint),
					new AgentHumanAILogic(),
					new BattleMissionAgentInteractionLogic(),
					new EquipmentControllerLeaveLogic(),
					new DefaultNavalMissionAgentSpawnLogic(array, playerSide, 0, null),
					new MissionHintLogic(),
					new MissionObjectiveLogic(),
					new AgentVictoryLogic(),
					new NavalBattleEndLogic(),
					new MissionHardBorderPlacer(),
					new MissionBoundaryPlacer(),
					new MissionBoundaryCrossingHandler(30f),
					new HighlightsController(),
					new BattleHighlightsController()
				};
			}, true, true);
		}

		// Token: 0x060009A7 RID: 2471 RVA: 0x00044E88 File Offset: 0x00043088
		[MissionMethod]
		public static Mission OpenNavalStorylineAlleyFightMission(MissionInitializerRecord rec)
		{
			return MissionState.OpenNew("NavalStorylineAlleyFight", rec, (Mission mission) => new List<MissionBehavior>
			{
				new NavalStorylineAlleyFightMissionController(),
				new NavalStorylineAlleyFightCinematicController(),
				new MissionHintLogic(),
				new MissionOptionsComponent(),
				new AgentHumanAILogic(),
				new BattlePowerCalculationLogic(),
				new CampaignMissionComponent(),
				new BattleObserverMissionLogic(),
				new AgentVictoryLogic(),
				new MissionHardBorderPlacer(),
				new MissionAgentHandler(),
				new MissionFightHandler(),
				new MissionBoundaryPlacer(),
				new MissionBoundaryCrossingHandler(10f),
				new HighlightsController(),
				new BattleHighlightsController(),
				new EquipmentControllerLeaveLogic()
			}.ToArray(), true, true);
		}
	}
}
