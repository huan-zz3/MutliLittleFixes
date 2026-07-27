using System;
using System.Collections.Generic;
using NavalDLC.Missions;
using NavalDLC.Missions.Deployment;
using NavalDLC.Missions.Handlers;
using NavalDLC.Missions.MissionLogics;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Source.Missions;
using TaleWorlds.MountAndBlade.Source.Missions.Handlers.Logic;

namespace NavalDLC.CustomBattle
{
	// Token: 0x02000003 RID: 3
	[MissionManager]
	public static class CustomNavalMissions
	{
		// Token: 0x06000029 RID: 41 RVA: 0x00002ADC File Offset: 0x00000CDC
		public static AtmosphereInfo CreateAtmosphereInfoForMission(string seasonId, int timeOfDay, float windStrength, Vec2 windDirection, TerrainType terrain)
		{
			int num;
			new Dictionary<string, int>
			{
				{ "spring", 0 },
				{ "summer", 1 },
				{ "fall", 2 },
				{ "winter", 3 }
			}.TryGetValue(seasonId, out num);
			if (!windDirection.IsNonZero())
			{
				windDirection = Vec2.Side;
			}
			AtmosphereInfo atmosphereInfo = default(AtmosphereInfo);
			TimeInformation timeInformation = default(TimeInformation);
			timeInformation.Season = num;
			timeInformation.TimeOfDay = (float)timeOfDay;
			atmosphereInfo.TimeInfo = timeInformation;
			NauticalInformation nauticalInformation = default(NauticalInformation);
			nauticalInformation.WindVector = windStrength * windDirection.Normalized();
			nauticalInformation.CanUseLowAltitudeAtmosphere = 1;
			nauticalInformation.IsRiverBattle = ((terrain == 11) ? 1 : 0);
			nauticalInformation.UsesNavalSimulatedWater = ((terrain == 11 || terrain == 10 || terrain == 19 || terrain == 18) ? 1 : 0);
			atmosphereInfo.NauticalInfo = nauticalInformation;
			return atmosphereInfo;
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00002BC4 File Offset: 0x00000DC4
		[MissionMethod]
		public static Mission OpenNavalBattleForCustomMission(string scene, BasicCharacterObject playerCharacter, CustomBattleCombatant playerParty, MBList<IShipOrigin> playerTeamShips, CustomBattleCombatant enemyParty, MBList<IShipOrigin> enemyTeamShips, bool isPlayerGeneral, string seasonString, float timeOfDay, float windStrength, NavalCustomBattleWindConfig.Direction windDirection, TerrainType terrain, string forcedSceneLevel)
		{
			BattleSideEnum playerSide = playerParty.Side;
			bool isPlayerAttacker = playerSide == 1;
			IMissionTroopSupplier[] troopSuppliers = new IMissionTroopSupplier[2];
			CustomBattleTroopSupplier customBattleTroopSupplier = new CustomBattleTroopSupplier(playerParty, true, isPlayerGeneral, false, null);
			troopSuppliers[playerParty.Side] = customBattleTroopSupplier;
			CustomBattleTroopSupplier customBattleTroopSupplier2 = new CustomBattleTroopSupplier(enemyParty, false, false, false, null);
			troopSuppliers[enemyParty.Side] = customBattleTroopSupplier2;
			bool isPlayerSergeant = !isPlayerGeneral;
			MissionInitializerRecord missionInitializerRecord;
			missionInitializerRecord..ctor(scene);
			TerrainType terrainType = terrain;
			missionInitializerRecord.TerrainType = terrainType;
			missionInitializerRecord.NeedsRandomTerrain = false;
			missionInitializerRecord.PlayingInCampaignMode = false;
			missionInitializerRecord.AtmosphereOnCampaign = CustomNavalMissions.CreateAtmosphereInfoForMission(seasonString, (int)timeOfDay, windStrength, new Vec2(0f, 1f), terrain);
			missionInitializerRecord.SceneHasMapPatch = false;
			missionInitializerRecord.PlayingInCampaignMode = true;
			missionInitializerRecord.DecalAtlasGroup = 2;
			missionInitializerRecord.AtmosphereOnCampaign.NauticalInfo.UsesNavalSimulatedWater = 1;
			missionInitializerRecord.SceneLevels = forcedSceneLevel;
			int maximumDeployableTroopCountForTeam = NavalDLCManager.Instance.GameModels.ShipDeploymentModel.GetMaximumDeployableTroopCountForTeam(playerTeamShips, true);
			int maximumDeployableTroopCountForTeam2 = NavalDLCManager.Instance.GameModels.ShipDeploymentModel.GetMaximumDeployableTroopCountForTeam(enemyTeamShips, false);
			int[] maxDeployableTroopCountPerTeam = new int[] { maximumDeployableTroopCountForTeam, 0, maximumDeployableTroopCountForTeam2 };
			int deployablePlayerShipCount = MathF.Min(playerTeamShips.Count, NavalShipDeploymentLimit.Max().NetDeploymentLimit);
			Mission mission2 = NavalMissionState.OpenNew("NavalCustomBattle", missionInitializerRecord, (Mission mission) => new MissionBehavior[]
			{
				new NavalShipsLogic(),
				new NavalFloatsamLogic(),
				new NavalAgentsLogic(),
				new DefaultNavalMissionLogic(playerTeamShips, null, enemyTeamShips, NavalShipDeploymentLimit.Max(), NavalShipDeploymentLimit.Invalid(), NavalShipDeploymentLimit.Max()),
				new NavalTrajectoryPlanningLogic(),
				new DefaultNavalMissionAgentSpawnLogic(troopSuppliers, playerSide, deployablePlayerShipCount, maxDeployableTroopCountPerTeam),
				new NavalMissionDeploymentPlanningLogic(mission),
				new BattlePowerCalculationLogic(),
				new CustomBattleAgentLogic(),
				new WaveParametersComputerLogic(),
				new MissionOptionsComponent(),
				new NavalAgentMoraleInteractionLogic(),
				new NavalBattleEndLogic(),
				new NavalBoundaryForceFieldLogic(),
				new NavalMissionCombatantsLogic(new List<CustomBattleCombatant> { playerParty, enemyParty }, playerParty, (!isPlayerAttacker) ? playerParty : enemyParty, isPlayerAttacker ? playerParty : enemyParty, 4, isPlayerSergeant),
				new BattleObserverMissionLogic(),
				new AgentHumanAILogic(),
				new AgentVictoryLogic(),
				new ShipCollisionOutcomeLogic(mission),
				new ShipRetreatLogic(),
				new BattleMissionAgentInteractionLogic(),
				new NavalAssignPlayerRoleInTeamMissionController(!isPlayerSergeant, isPlayerSergeant, false, null),
				new EquipmentControllerLeaveLogic(),
				new MissionHardBorderPlacer(),
				new MissionBoundaryPlacer(),
				new MissionBoundaryCrossingHandler(30f),
				new HighlightsController(),
				new BattleHighlightsController(),
				new NavalDeploymentMissionController(isPlayerAttacker),
				new NavalDeploymentHandler(isPlayerAttacker),
				new NavalCustomBattleWindAndWaveLogic(windDirection, terrainType)
			}, true, true);
			mission2.SetPlayerCanTakeControlOfAnotherAgentWhenDead();
			return mission2;
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002D94 File Offset: 0x00000F94
		[MissionMethod]
		public static Mission OpenNavalRaidBattleForCustomMission(string scene, BasicCharacterObject playerCharacter, CustomBattleCombatant playerParty, CustomBattleCombatant enemyParty, MBList<IShipOrigin> attackerShips, bool isPlayerGeneral, string seasonString, float timeOfDay, float windStrength, NavalCustomBattleWindConfig.Direction windDirection, TerrainType terrain, string forcedSceneLevel)
		{
			BattleSideEnum playerSide = playerParty.Side;
			bool isPlayerAttacker = playerSide == 1;
			IMissionTroopSupplier[] troopSuppliers = new IMissionTroopSupplier[2];
			CustomBattleTroopSupplier customBattleTroopSupplier = new CustomBattleTroopSupplier(playerParty, true, isPlayerGeneral, false, null);
			troopSuppliers[playerParty.Side] = customBattleTroopSupplier;
			CustomBattleTroopSupplier customBattleTroopSupplier2 = new CustomBattleTroopSupplier(enemyParty, false, false, false, null);
			troopSuppliers[enemyParty.Side] = customBattleTroopSupplier2;
			bool isPlayerSergeant = !isPlayerGeneral;
			MissionInitializerRecord missionInitializerRecord;
			missionInitializerRecord..ctor(scene);
			TerrainType terrainType = terrain;
			missionInitializerRecord.TerrainType = terrainType;
			missionInitializerRecord.NeedsRandomTerrain = false;
			missionInitializerRecord.PlayingInCampaignMode = false;
			missionInitializerRecord.AtmosphereOnCampaign = CustomNavalMissions.CreateAtmosphereInfoForMission(seasonString, (int)timeOfDay, windStrength, new Vec2(0f, 1f), terrain);
			missionInitializerRecord.SceneHasMapPatch = false;
			missionInitializerRecord.PlayingInCampaignMode = true;
			missionInitializerRecord.DecalAtlasGroup = 2;
			missionInitializerRecord.SceneLevels = "naval_raid";
			int attackerTeamMaxDeployableTroopCount = NavalDLCManager.Instance.GameModels.ShipDeploymentModel.GetMaximumDeployableTroopCountForTeam(attackerShips, isPlayerAttacker);
			attackerTeamMaxDeployableTroopCount = MathF.Min(troopSuppliers[1].NumTroopsNotSupplied, attackerTeamMaxDeployableTroopCount);
			int num = MathF.Min(attackerShips.Count, NavalShipDeploymentLimit.Max().NetDeploymentLimit);
			NavalShipDeploymentLimit attackerShipsDeploymentLimit = new NavalShipDeploymentLimit(num);
			CustomBattleCombatant customBattleCombatant = (isPlayerAttacker ? enemyParty : playerParty);
			int defenderTeamMaxDeployableTroopCount = customBattleCombatant.NumberOfHealthyMembers;
			Mission mission2 = NavalMissionState.OpenNew("NavalRaidCustomBattle", missionInitializerRecord, (Mission mission) => new MissionBehavior[]
			{
				new NavalShipsLogic(),
				new NavalFloatsamLogic(),
				new NavalAgentsLogic(),
				new NavalRaidMissionController(),
				new NavalRaidMissionAgentSpawnLogic(troopSuppliers, playerSide, attackerShips, attackerShipsDeploymentLimit, attackerTeamMaxDeployableTroopCount, defenderTeamMaxDeployableTroopCount),
				new NavalTrajectoryPlanningLogic(),
				new NavalRaidMissionDeploymentPlanningLogic(),
				new BattlePowerCalculationLogic(),
				new CustomBattleAgentLogic(),
				new WaveParametersComputerLogic(),
				new MissionOptionsComponent(),
				new NavalAgentMoraleInteractionLogic(),
				new BattleEndLogic(),
				new NavalBoundaryForceFieldLogic(),
				new NavalMissionCombatantsLogic(new List<CustomBattleCombatant> { playerParty, enemyParty }, playerParty, (!isPlayerAttacker) ? playerParty : enemyParty, isPlayerAttacker ? playerParty : enemyParty, 5, isPlayerSergeant),
				new BattleObserverMissionLogic(),
				new AgentHumanAILogic(),
				new AgentVictoryLogic(),
				new ShipCollisionOutcomeLogic(mission),
				new BattleMissionAgentInteractionLogic(),
				new NavalAssignPlayerRoleInTeamMissionController(!isPlayerSergeant, isPlayerSergeant, false, null),
				new EquipmentControllerLeaveLogic(),
				new MissionHardBorderPlacer(),
				new MissionBoundaryPlacer(),
				new MissionBoundaryCrossingHandler(30f),
				new HighlightsController(),
				new BattleHighlightsController(),
				new NavalRaidDeploymentMissionController(isPlayerAttacker),
				new NavalRaidDeploymentHandler(isPlayerAttacker),
				new NavalCustomBattleWindAndWaveLogic(windDirection, terrainType)
			}, true, true);
			mission2.SetPlayerCanTakeControlOfAnotherAgentWhenDead();
			return mission2;
		}
	}
}
