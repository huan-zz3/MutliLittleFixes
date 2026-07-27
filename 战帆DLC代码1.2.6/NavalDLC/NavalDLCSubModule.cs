using System;
using NavalDLC.CampaignBehaviors;
using NavalDLC.ComponentInterfaces;
using NavalDLC.GameComponents;
using NavalDLC.Missions;
using NavalDLC.Storyline;
using NavalDLC.Storyline.CampaignBehaviors;
using SandBox.GameComponents;
using StoryMode;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ComponentInterfaces;
using TaleWorlds.ObjectSystem;

namespace NavalDLC
{
	// Token: 0x02000021 RID: 33
	public class NavalDLCSubModule : MBSubModuleBase
	{
		// Token: 0x0600016F RID: 367 RVA: 0x000096A8 File Offset: 0x000078A8
		protected override void OnSubModuleLoad()
		{
			TauntUsageManager.Initialize();
		}

		// Token: 0x06000170 RID: 368 RVA: 0x000096B0 File Offset: 0x000078B0
		protected override void RegisterSubModuleTypes()
		{
		}

		// Token: 0x06000171 RID: 369 RVA: 0x000096B4 File Offset: 0x000078B4
		protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
		{
			game.AddGameHandler<NavalDLCManager>();
			NavalDLCManager.Instance = Game.Current.GetGameHandler<NavalDLCManager>();
			NavalDLCManager.Instance.OnGameStart(game, gameStarterObject);
			string applicationVersionBuildNumber = NavalVersion.GetApplicationVersionBuildNumber();
			Utilities.SetWatchdogValue("crash_tags.txt", "ModuleVersion", "NavalDLC", applicationVersionBuildNumber);
		}

		// Token: 0x06000172 RID: 370 RVA: 0x000096FE File Offset: 0x000078FE
		public override void OnGameEnd(Game game)
		{
			NavalDLCManager.Instance.OnGameEnd(game);
		}

		// Token: 0x06000173 RID: 371 RVA: 0x0000970B File Offset: 0x0000790B
		public override void InitializeSubModuleGameObjects(Game game)
		{
			NavalDLCManager.Instance.InitializeNavalGameObjects(game);
		}

		// Token: 0x06000174 RID: 372 RVA: 0x00009718 File Offset: 0x00007918
		public override void RegisterSubModuleObjects(bool isSavedCampaign)
		{
			MBObjectManagerExtensions.LoadXML(MBObjectManager.Instance, "ShipUpgradePieces", false);
			MBObjectManagerExtensions.LoadXML(MBObjectManager.Instance, "ShipSlots", false);
			MBObjectManagerExtensions.LoadXML(MBObjectManager.Instance, "ShipHulls", false);
			MBObjectManagerExtensions.LoadXML(MBObjectManager.Instance, "ShipPhysicsReferences", false);
			MBObjectManagerExtensions.LoadXML(MBObjectManager.Instance, "MissionShips", false);
		}

		// Token: 0x06000175 RID: 373 RVA: 0x00009778 File Offset: 0x00007978
		protected override void InitializeGameStarter(Game game, IGameStarter gameStarterObject)
		{
			if (game.GameType is Campaign)
			{
				CampaignGameStarter campaignGameStarter = gameStarterObject as CampaignGameStarter;
				this.AddBehaviors(campaignGameStarter, game);
				this.AddModels(campaignGameStarter);
				return;
			}
			if (game.GameType is EditorGame)
			{
				gameStarterObject.AddModel<ShipPhysicsParametersModel>(new NavalDLCShipPhysicsParametersModel());
			}
		}

		// Token: 0x06000176 RID: 374 RVA: 0x000097C4 File Offset: 0x000079C4
		public override void OnAfterGameInitializationFinished(Game game, object starterObject)
		{
			Campaign campaign = game.GameType as Campaign;
			if (campaign != null)
			{
				campaign.CampaignMissionManager = new NavalMissionManager(campaign.CampaignMissionManager);
			}
		}

		// Token: 0x06000177 RID: 375 RVA: 0x000097F1 File Offset: 0x000079F1
		public override void OnGameInitializationFinished(Game game)
		{
			if (game.GameType is Campaign && game.GameType is CampaignStoryMode && StoryModeManager.Current != null)
			{
				NavalDLCManager.Instance.NavalStorylineData.Initialize();
			}
		}

		// Token: 0x06000178 RID: 376 RVA: 0x00009824 File Offset: 0x00007A24
		private void AddBehaviors(CampaignGameStarter gameStarter, Game game)
		{
			gameStarter.AddBehavior(new NavalTransitionCampaignBehavior());
			gameStarter.AddBehavior(new NavalCharacterCreationCampaignBehavior());
			gameStarter.AddBehavior(new SeaDamageCampaignBehavior());
			gameStarter.AddBehavior(new ShipProductionCampaignBehavior());
			gameStarter.AddBehavior(new ShipTradeCampaignBehavior());
			gameStarter.AddBehavior(new ShipRepairCampaignBehavior());
			gameStarter.AddBehavior(new RaftStateCampaignBehavior());
			gameStarter.AddBehavior(new ShipUpgradeCampaignBehavior());
			gameStarter.AddBehavior(new PortCharactersCampaignBehavior());
			gameStarter.AddBehavior(new ClanFleetManagementCampaignBehavior());
			gameStarter.AddBehavior(new NavalPatrolPartiesCampaignBehavior());
			gameStarter.AddBehavior(new NavalVeteransWisdomCampaignBehaviour());
			gameStarter.AddBehavior(new NavalFishingCampaignBehaviour());
			gameStarter.AddBehavior(new NavalNimbleSurgeCampaignBehaviour());
			gameStarter.AddBehavior(new NavalStormriderCampaignBehaviour());
			gameStarter.AddBehavior(new NavalOrderOfBattleCampaignBehavior());
			gameStarter.AddBehavior(new NavalDLCTutorialBoxCampaignBehavior());
			gameStarter.AddBehavior(new PiratesCampaignBehavior());
			gameStarter.AddBehavior(new NavalKingdomPolicyCampaignBehaviour());
			gameStarter.AddBehavior(new FishingPartyCampaignBehavior());
			gameStarter.AddBehavior(new StormCampaignBehavior());
			gameStarter.AddBehavior(new NavalDLCFigureheadCampaignBehavior());
			gameStarter.AddBehavior(new NavalShipDistributionCampaignBehavior());
			gameStarter.AddBehavior(new ShipNameCampaignBehavior());
			gameStarter.AddBehavior(new NavalInitializationCampaignBehavior());
			gameStarter.AddBehavior(new NavalCompanionRolesCampaignBehavior());
			if (game.GameType is CampaignStoryMode && StoryModeManager.Current != null)
			{
				gameStarter.AddBehavior(new NavalStorylineCampaignBehavior());
				gameStarter.AddBehavior(new NavalStorylineFirstActCampaignBehavior());
				gameStarter.AddBehavior(new NavalStorylineSecondActCampaignBehavior());
				gameStarter.AddBehavior(new NavalStorylineThirdActSecondQuestBehavior());
				gameStarter.AddBehavior(new NavalStorylineThirdActThirdQuestBehavior());
				gameStarter.AddBehavior(new NavalStorylineTravelCommentaryCampaignBehavior());
				gameStarter.AddBehavior(new NavalStorylinePlayerTownVisitCampaignBehavior());
				gameStarter.AddBehavior(new NavalStorylineHeroAgentSpawnBehavior());
				gameStarter.AddBehavior(new NavalStorylineThirdActFirstQuestBehavior());
				gameStarter.AddBehavior(new NavalStorylineThirdActFourthQuestBehavior());
				gameStarter.AddBehavior(new DefeatTheCaptorsQuestBehavior());
				gameStarter.AddBehavior(new NavalStorylineThirdActFifthQuestBehaviour());
			}
		}

		// Token: 0x06000179 RID: 377 RVA: 0x000099F0 File Offset: 0x00007BF0
		private void AddModels(CampaignGameStarter campaignGameStarter)
		{
			campaignGameStarter.AddModel<PartyNavigationModel>(new NavalPartyNavigationModel(campaignGameStarter.GetModel<PartyNavigationModel>()));
			campaignGameStarter.AddModel<BanditDensityModel>(new NavalDLCBanditDensityModel());
			campaignGameStarter.AddModel<CampaignShipDamageModel>(new NavalDLCCampaignShipDamageModel());
			campaignGameStarter.AddModel<CampaignShipParametersModel>(new NavalDLCCampaignShipParametersModel());
			campaignGameStarter.AddModel<ShipDeploymentModel>(new NavalDLCShipDeploymentModel());
			campaignGameStarter.AddModel<ArmyManagementCalculationModel>(new NavalDLCArmyManagementCalculationModel());
			campaignGameStarter.AddModel<PartySpeedModel>(new NavalDLCPartySpeedCalculationModel());
			campaignGameStarter.AddModel<RaidModel>(new NavalDLCRaidModel());
			campaignGameStarter.AddModel<BuildingModel>(new NavalDLCBuildingModel());
			campaignGameStarter.AddModel<BattleRewardModel>(new NavalDLCBattleRewardModel());
			campaignGameStarter.AddModel<MilitaryPowerModel>(new NavalDLCMilitaryPowerModel());
			campaignGameStarter.AddModel<ShipCostModel>(new NavalDLCShipCostModel());
			campaignGameStarter.AddModel<CombatSimulationModel>(new NavalDLCCombatSimulationModel());
			campaignGameStarter.AddModel<IncidentModel>(new NavalDLCIncidentModel());
			campaignGameStarter.AddModel<EncounterGameMenuModel>(new NavalEncounterMenuModel());
			campaignGameStarter.AddModel<CaravanModel>(new NavalDLCCaravanModel());
			campaignGameStarter.AddModel<PartyShipLimitModel>(new NavalDLCShipLimitModel());
			campaignGameStarter.AddModel<PartySizeLimitModel>(new NavalDLCPartySizeLimitModel());
			campaignGameStarter.AddModel<MobilePartyAIModel>(new NavalDLCMobilePartyAIModel());
			campaignGameStarter.AddModel<EncounterModel>(new NavalDLCEncounterModel());
			campaignGameStarter.AddModel<VoiceOverModel>(new NavalDLCVoiceOverModel());
			campaignGameStarter.AddModel<HeroAgentLocationModel>(new NavalDLCHeroAgentLocationModel());
			campaignGameStarter.AddModel<TournamentModel>(new NavalDLCTournamentModel());
			campaignGameStarter.AddModel<SettlementAccessModel>(new NavalDLCSettlementAccessModel());
			campaignGameStarter.AddModel<FleetManagementModel>(new NavalDLCFleetManagementModel());
			campaignGameStarter.AddModel<TroopSacrificeModel>(new NavalDLCTroopSacrificeModel());
			campaignGameStarter.AddModel<CombatXpModel>(new NavalDLCCombatXpModel());
			campaignGameStarter.AddModel<InventoryCapacityModel>(new NavalDLCInventoryCapacityModel());
			campaignGameStarter.AddModel<MobilePartyFoodConsumptionModel>(new NavalDLCMobilePartyFoodConsumptionModel());
			campaignGameStarter.AddModel<PartyHealingModel>(new NavalDLCPartyHealingModel());
			campaignGameStarter.AddModel<PartyMoraleModel>(new NavalDLCPartyMoraleModel());
			campaignGameStarter.AddModel<PartyTrainingModel>(new NavalDLCPartyTrainingModel());
			campaignGameStarter.AddModel<PartyTroopUpgradeModel>(new NavalDLCPartyTroopUpgradeModel());
			campaignGameStarter.AddModel<PartyWageModel>(new NavalDLCPartyWageModel());
			campaignGameStarter.AddModel<PrisonerRecruitmentCalculationModel>(new NavalDLCPrisonerRecruitmentCalculationModel());
			campaignGameStarter.AddModel<SettlementGarrisonModel>(new NavalDLCSettlementGarrisonModel());
			campaignGameStarter.AddModel<SettlementMilitiaModel>(new NavalDLCSettlementMilitiaModel());
			campaignGameStarter.AddModel<VillageProductionCalculatorModel>(new NavalDLCVillageProductionCalculatorModel());
			campaignGameStarter.AddModel<TroopSacrificeModel>(new NavalDLCTroopSacrificeModel());
			campaignGameStarter.AddModel<MapDistanceModel>(new NavalDLCMapDistanceModel());
			campaignGameStarter.AddModel<MapVisibilityModel>(new NavalDLCMapVisibilityModel());
			campaignGameStarter.AddModel<PartyImpairmentModel>(new NavalDLCPartyImpairmentModel());
			campaignGameStarter.AddModel<PartyTransitionModel>(new NavalDLCPartyTransitionModel());
			campaignGameStarter.AddModel<SettlementProsperityModel>(new NavalDLCSettlementProsperityModel());
			campaignGameStarter.AddModel<WorkshopModel>(new NavalDLCWorkshopModel());
			campaignGameStarter.AddModel<BuildingConstructionModel>(new NavalDLCBuildingConstructionModel());
			campaignGameStarter.AddModel<SettlementSecurityModel>(new NavalDLCSettlementSecurityModel());
			campaignGameStarter.AddModel<ClanFinanceModel>(new NavalDLCClanFinanceModel());
			campaignGameStarter.AddModel<ClanPoliticsModel>(new NavalDLCClanPoliticsModel());
			campaignGameStarter.AddModel<ShipStatModel>(new NavalDLCShipStatModel());
			campaignGameStarter.AddModel<MapStormModel>(new NavalDLCStormModel());
			campaignGameStarter.AddModel<ShipPhysicsParametersModel>(new NavalDLCShipPhysicsParametersModel());
			campaignGameStarter.AddModel<ClanShipOwnershipModel>(new NavalDLCClanShipOwnershipModel());
			campaignGameStarter.AddModel<SettlementPatrolModel>(new NavalSettlementPatrolModel());
			campaignGameStarter.AddModel<CharacterDevelopmentModel>(new NavalCharacterDevelopmentModel());
			campaignGameStarter.AddModel<TradeAgreementModel>(new NavalTradeAgreementModel());
			campaignGameStarter.AddModel<AgentStatCalculateModel>(new NavalAgentStatCalculateModel());
			campaignGameStarter.AddModel<AgentApplyDamageModel>(new NavalAgentApplyDamageModel());
			campaignGameStarter.AddModel<StrikeMagnitudeCalculationModel>(new NavalStrikeMagnitudeModel());
			campaignGameStarter.AddModel<BattleMoraleModel>(new NavalBattleMoraleModel());
			campaignGameStarter.AddModel<MissionShipParametersModel>(new NavalMissionShipParametersModel());
			campaignGameStarter.AddModel<MissionSiegeEngineCalculationModel>(new NavalMissionSiegeEngineCalculationModel());
			campaignGameStarter.AddModel<BattleInitializationModel>(new NavalBattleInitializationModel());
			campaignGameStarter.AddModel<ShipDistributionModel>(new NavalDLCShipDistributionModel());
			campaignGameStarter.AddModel<ClanMemberPartyRoleModel>(new NavalDLCClanMemberPartyRoleModel());
			campaignGameStarter.AddModel<TargetScoreCalculatingModel>(new NavalTargetScoreCalculatingModel());
			if (Game.Current.GameType is Campaign)
			{
				campaignGameStarter.AddModel<MapWeatherModel>(new NavalDLCMapWeatherModel());
			}
		}

		// Token: 0x040000A1 RID: 161
		public const string ShipPhysicsReferencesXMLPath = "ShipPhysicsReferences";

		// Token: 0x040000A2 RID: 162
		public const string MissionShipsXMLPath = "MissionShips";

		// Token: 0x040000A3 RID: 163
		public const string ModuleName = "NavalDLC";

		// Token: 0x040000A4 RID: 164
		public const string FigureheadSlotTag = "figurehead";
	}
}
