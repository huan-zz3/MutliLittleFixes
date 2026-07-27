using System;
using System.Collections.Generic;
using System.Linq;
using NavalDLC.Missions;
using NavalDLC.Missions.AI.Tactics;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using NavalDLC.Missions.ShipActuators;
using NavalDLC.Missions.ShipControl;
using NavalDLC.Missions.ShipInput;
using NavalDLC.Storyline.Objectives.Captivity;
using SandBox;
using SandBox.Missions.AgentBehaviors;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.MissionLogics;
using TaleWorlds.ObjectSystem;

namespace NavalDLC.Storyline
{
	// Token: 0x0200002C RID: 44
	public class HelpingAnAllySetPieceBattleMissionController : MissionLogic, IMissionAgentSpawnLogic, IMissionBehavior
	{
		// Token: 0x1700005B RID: 91
		// (get) Token: 0x06000206 RID: 518 RVA: 0x0000DFF4 File Offset: 0x0000C1F4
		public BattleSideEnum PlayerSide
		{
			get
			{
				return -1;
			}
		}

		// Token: 0x06000207 RID: 519 RVA: 0x0000DFF8 File Offset: 0x0000C1F8
		public HelpingAnAllySetPieceBattleMissionController(MobileParty merchantParty, MobileParty seaHoundsParty)
		{
			this._merchantParty = merchantParty;
			this._seaHoundsParty = seaHoundsParty;
		}

		// Token: 0x06000208 RID: 520 RVA: 0x0000E048 File Offset: 0x0000C248
		public override void OnMissionTick(float dt)
		{
			if (!this._isMissionInitialized)
			{
				this._isMissionInitialized = true;
				this.UpdateEntityReferences();
				this._agentsLogic = Mission.Current.GetMissionBehavior<NavalAgentsLogic>();
				NavalShipsLogic missionBehavior = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
				this._agentsLogic.SetDeploymentMode(true);
				missionBehavior.SetDeploymentMode(true);
				missionBehavior.SetTeamShipDeploymentLimit(0, NavalShipDeploymentLimit.Max());
				missionBehavior.SetTeamShipDeploymentLimit(1, NavalShipDeploymentLimit.Max());
				missionBehavior.SetTeamShipDeploymentLimit(2, NavalShipDeploymentLimit.Max());
				Team team = Mission.GetTeam(0);
				Formation formation = team.GetFormation(0);
				team.SetPlayerRole(true, true);
				Formation formation2 = Mission.GetTeam(1).GetFormation(0);
				Team team2 = Mission.GetTeam(2);
				Formation formation3 = team2.GetFormation(0);
				Formation formation4 = team2.GetFormation(1);
				this._playerShip = this.CreateShip("longship_storyline_q1", "player_ship_sp", formation, PartyBase.MainParty, "generated_square__h4_09", DefaultFigureheads.Dragon, HelpingAnAllySetPieceBattleMissionController.PlayerShipUpgradePieces);
				if (missionBehavior != null)
				{
					missionBehavior.TeleportShip(this._playerShip, this._playerShip.GlobalFrame, false, false, true);
				}
				Scene scene = Mission.Current.Scene;
				MatrixFrame matrixFrame = this._playerShip.GlobalFrame;
				Vec2 vec = matrixFrame.rotation.f.AsVec2;
				scene.SetGlobalWindVelocity(ref vec);
				this._allyShip = this.CreateShip("ship_trade_cog_q1", "ally_ship_sp", formation2, this._merchantParty.Party, "generated_square_l1_h4_04", null, HelpingAnAllySetPieceBattleMissionController.AllyShipUpgradePieces);
				if (missionBehavior != null)
				{
					missionBehavior.TeleportShip(this._allyShip, this._allyShip.GlobalFrame, false, false, true);
				}
				this._pursuerShip1 = this.CreateShip("northern_medium_ship", "sea_hound_ship_1_sp", formation3, this._seaHoundsParty.Party, "generated_square_l1_h4_10", DefaultFigureheads.Viper, HelpingAnAllySetPieceBattleMissionController.Enemy1ShipUpgradePieces);
				this._pursuerShip2 = this.CreateShip("ship_lightlongship_q1", "sea_hound_ship_2_sp", formation4, this._seaHoundsParty.Party, "generated_square_l1_h4_10", DefaultFigureheads.Ram, HelpingAnAllySetPieceBattleMissionController.Enemy2ShipUpgradePieces);
				if (missionBehavior != null)
				{
					missionBehavior.TeleportShip(this._pursuerShip1, this._pursuerShip1.GlobalFrame, false, false, true);
				}
				if (missionBehavior != null)
				{
					missionBehavior.TeleportShip(this._pursuerShip2, this._pursuerShip2.GlobalFrame, false, false, true);
				}
				base.Mission.DefenderTeam.TeamAI.ClearTacticOptions();
				base.Mission.DefenderTeam.AddTacticOption(new TacticNavalLineDefense(base.Mission.DefenderTeam));
				base.Mission.AttackerTeam.TeamAI.ClearTacticOptions();
				base.Mission.AttackerTeam.AddTacticOption(new TacticNavalBalancedOffense(base.Mission.AttackerTeam));
				this._playerShip.SetController(ShipControllerType.Player, true);
				this._playerShip.SetAnchor(false, false, 1f);
				if (missionBehavior != null)
				{
					missionBehavior.SetCanHaveConnectionCooldown(false);
				}
				this._pursuerShip1.SetController(ShipControllerType.AI, true);
				this._pursuerShip2.SetController(ShipControllerType.AI, true);
				this._pursuerShip1.ShipOrder.SetShipEngageOrder(this._allyShip);
				this._pursuerShip2.ShipOrder.SetShipEngageOrder(this._allyShip);
				this._pursuerShip1.SetShipOrderActive(true);
				this._pursuerShip2.SetShipOrderActive(true);
				this._pursuerShip1.SetCanBeTakenOver(false);
				this._pursuerShip2.SetCanBeTakenOver(false);
				this._allyShip.SetShipOrderActive(true);
				this.UpdateEntityReferences();
				this.SpawnPlayer();
				formation.PlayerOwner = Agent.Main;
				this.SpawnPlayerShipAgents();
				this.SpawnAllyShipAgents(this._allyShip);
				this.SpawnEnemyAgents(this._pursuerShip1, "sea_hounds_pups", 28, "sea_hounds", 2);
				this.SpawnEnemyAgents(this._pursuerShip2, "sea_hounds_pups", 16, "sea_hounds", 2);
				team.SetPlayerRole(true, true);
				foreach (Team team3 in Mission.Current.Teams)
				{
					team3.MasterOrderController.SelectAllFormations(false);
					team3.MasterOrderController.SetOrder(34);
					team3.MasterOrderController.ClearSelectedFormations();
				}
				int j;
				int i;
				for (i = 1; i <= 6; i = j + 1)
				{
					GameEntity gameEntity = this._entities.FirstOrDefault<GameEntity>((GameEntity t) => t.HasTag("volume_box_" + i));
					this._waypoints.Add(gameEntity);
					j = i;
				}
				this._agentsLogic.AssignCaptainToShipForDeploymentMode(Agent.Main, this._playerShip, null);
				this._agentsLogic.AssignAndTeleportCrewToShipMachines(0);
				this._agentsLogic.AssignAndTeleportCrewToShipMachines(1);
				this._agentsLogic.AssignAndTeleportCrewToShipMachines(2);
				Mission.Current.OnDeploymentFinished();
				this._agentsLogic.SetDeploymentMode(false);
				missionBehavior.SetDeploymentMode(false);
				Scene scene2 = Mission.Current.Scene;
				matrixFrame = this._entities.First<GameEntity>((GameEntity t) => t.HasTag("sp_wind")).GetGlobalFrame();
				vec = matrixFrame.rotation.f.AsVec2;
				vec = vec.Normalized() * 2f;
				scene2.SetGlobalWindStrengthVector(ref vec);
				CampaignInformationManager.AddDialogLine(new TextObject("{=FkFpeYSI}Look - there's two of them giving chase. We'll have to take one down quickly, and hope the Vlandians can hold the other off until we reach them.", null), NavalStorylineData.Gunnar.CharacterObject, null, 0, 2);
				HelpingAnAllyMissionObjective helpingAnAllyMissionObjective = new HelpingAnAllyMissionObjective(Mission.Current);
				this._missionObjectiveLogic.StartObjective(helpingAnAllyMissionObjective);
				this._playerShip.SetCustomSailSetting(false, SailInput.Raised);
				this.OnShipsInitializedEvent();
			}
			this.HandleShipOrders();
			this._drownCheckTimer += dt;
			if (this._drownCheckTimer >= this._drownCheckDuration)
			{
				this._drownCheckTimer = 0f;
				this.CheckDrowningAgents(this._pursuerShip1);
				this.CheckDrowningAgents(this._pursuerShip2);
			}
			if (this._isVictoryQueued)
			{
				this._victoryPopUpTimer += dt;
				if (this._victoryPopUpTimer >= this._victoryPopUpDelay)
				{
					this._isVictoryQueued = false;
					this.OpenVictoryPopUp();
				}
			}
			if (this._isDefeatQueued)
			{
				this._defeatTimer += dt;
				if (!this._isFadeOutTriggered && this._defeatTimer >= 2f)
				{
					this._isFadeOutTriggered = true;
					this.StartDefeatFadeOut();
				}
				if (this._defeatTimer >= 5f)
				{
					this._isDefeatQueued = false;
					this.OnMissionFailed();
				}
			}
			if (!this._playerShip.GetIsConnected())
			{
				this._notificationTimer += dt;
				if (this._notificationTimer > 10f)
				{
					this._notificationTimer = 0f;
					if (this.HasSailThrust())
					{
						if (this._playerShip.SailTargetSetting < 1f)
						{
							CampaignInformationManager.AddDialogLine(new TextObject("{=cGay4oWJ}The wind is with us. Should we unfurl the sail?", null), NavalStorylineData.Gunnar.CharacterObject, null, 0, 2);
							return;
						}
					}
					else if (this._playerShip.SailTargetSetting > 0f)
					{
						CampaignInformationManager.AddDialogLine(new TextObject("{=IpjMuSVa}The wind is blowing against us. Best furl the sail.", null), NavalStorylineData.Gunnar.CharacterObject, null, 0, 2);
						return;
					}
				}
			}
			else
			{
				this._notificationTimer = 0f;
			}
		}

		// Token: 0x06000209 RID: 521 RVA: 0x0000E714 File Offset: 0x0000C914
		public override void OnBehaviorInitialize()
		{
			if (!SailWindProfile.IsSailWindProfileInitialized)
			{
				SailWindProfile.InitializeProfile();
			}
			this._missionObjectiveLogic = base.Mission.GetMissionBehavior<MissionObjectiveLogic>();
		}

		// Token: 0x0600020A RID: 522 RVA: 0x0000E733 File Offset: 0x0000C933
		private void UpdateEntityReferences()
		{
			base.Mission.Scene.GetEntities(ref this._entities);
		}

		// Token: 0x0600020B RID: 523 RVA: 0x0000E74C File Offset: 0x0000C94C
		private MissionShip CreateShip(string shipHullId, string spawnPointId, Formation formation, PartyBase owner, string materialName, Figurehead figurehead = null, Dictionary<string, string> upgradePieces = null)
		{
			Ship ship = new Ship(Campaign.Current.ObjectManager.GetObject<ShipHull>(shipHullId));
			if (figurehead != null)
			{
				ship.ChangeFigurehead(figurehead);
			}
			if (upgradePieces != null)
			{
				foreach (KeyValuePair<string, string> keyValuePair in upgradePieces)
				{
					if (ship.HasSlot(keyValuePair.Key))
					{
						ship.EquipUpgradePiece(keyValuePair.Key, MBObjectManager.Instance.GetObject<ShipUpgradePiece>(keyValuePair.Value));
					}
				}
			}
			MissionShip missionShip = this.CreateMissionShip(ship, spawnPointId, formation);
			if (owner.MobileParty.IsBandit)
			{
				this.ChangeShipColors(missionShip, NavalStorylineData.CorsairBanner.GetPrimaryColor(), NavalStorylineData.CorsairBanner.GetSecondaryColor(), materialName);
			}
			else
			{
				this.ChangeShipColors(missionShip, owner.MapFaction.Color, owner.MapFaction.Color2, materialName);
			}
			return missionShip;
		}

		// Token: 0x0600020C RID: 524 RVA: 0x0000E840 File Offset: 0x0000CA40
		private void ChangeShipColors(MissionShip missionShip, uint color1, uint color2, string materialName)
		{
			foreach (GameEntity gameEntity in missionShip.SailMeshEntities)
			{
				this.SetSailColors(gameEntity, color1, color2, materialName);
			}
		}

		// Token: 0x0600020D RID: 525 RVA: 0x0000E898 File Offset: 0x0000CA98
		private void SetSailColors(GameEntity sailEntity, uint sailColor1, uint sailColor2, string materialName)
		{
			if (sailEntity.Skeleton != null)
			{
				foreach (Mesh mesh in sailEntity.Skeleton.GetAllMeshes())
				{
					if (mesh.HasTag("faction_color"))
					{
						Material fromResource = Material.GetFromResource(materialName);
						if (fromResource != null)
						{
							mesh.SetMaterial(fromResource);
						}
						mesh.Color = sailColor1;
						mesh.Color2 = sailColor2;
					}
				}
			}
			foreach (Mesh mesh2 in sailEntity.WeakEntity.GetAllMeshesWithTag("faction_color"))
			{
				mesh2.Color = sailColor1;
				mesh2.Color2 = sailColor2;
			}
		}

		// Token: 0x0600020E RID: 526 RVA: 0x0000E974 File Offset: 0x0000CB74
		private void OnShipsEngaged(MissionShip ship1, MissionShip ship2)
		{
			int activeAgentCountOfShip = this._agentsLogic.GetActiveAgentCountOfShip(ship1);
			int activeAgentCountOfShip2 = this._agentsLogic.GetActiveAgentCountOfShip(ship2);
			if (activeAgentCountOfShip > 0 && activeAgentCountOfShip2 > 0)
			{
				ship1.ShipOrder.SetShipEngageOrder(ship2);
				ship2.ShipOrder.SetShipEngageOrder(ship1);
				this.AddFightBehaviors(ship1);
				this.AddFightBehaviors(ship2);
			}
		}

		// Token: 0x0600020F RID: 527 RVA: 0x0000E9C8 File Offset: 0x0000CBC8
		private void HandleShipOrders()
		{
			if (this.AreShipsWithinDistance(this._pursuerShip1, this._playerShip, 30f))
			{
				this.OnShipsEngaged(this._pursuerShip1, this._playerShip);
				this._isPursuer1ShipEngaged = true;
			}
			else if (this._isPursuer1ShipEngaged)
			{
				this.CalmAgentsOfShip(this._playerShip);
				this.CalmAgentsOfShip(this._pursuerShip1);
				this._isPursuer1ShipEngaged = false;
			}
			if (this.AreShipsWithinDistance(this._pursuerShip2, this._playerShip, 30f))
			{
				this.OnShipsEngaged(this._pursuerShip2, this._playerShip);
				this._isPursuer2ShipEngaged = true;
			}
			else if (this._isPursuer2ShipEngaged)
			{
				this.CalmAgentsOfShip(this._playerShip);
				this.CalmAgentsOfShip(this._pursuerShip2);
				this._isPursuer2ShipEngaged = false;
			}
			if (this.AreShipsWithinDistance(this._pursuerShip1, this._allyShip, 30f))
			{
				this.OnShipsEngaged(this._pursuerShip1, this._allyShip);
				this.OnMerchantsAboutToBeBoarded();
			}
			if (this.AreShipsWithinDistance(this._pursuerShip2, this._allyShip, 30f))
			{
				this.OnShipsEngaged(this._pursuerShip2, this._allyShip);
				this.OnMerchantsAboutToBeBoarded();
			}
			if (this.AreShipsWithinDistance(this._pursuerShip1, this._playerShip, 10f))
			{
				this._pursuerShip1.ShipOrder.SetShipEngageOrder(this._playerShip);
				this._pursuerShip1.ShipOrder.SetBoardingTargetShip(this._playerShip);
			}
			else if (!this._pursuerShip1.GetIsConnected())
			{
				this._pursuerShip1.ShipOrder.SetShipEngageOrder(this._allyShip);
				this._pursuerShip1.ShipOrder.SetBoardingTargetShip(this._allyShip);
			}
			if (this.AreShipsWithinDistance(this._pursuerShip2, this._playerShip, 10f))
			{
				this._pursuerShip2.ShipOrder.SetShipEngageOrder(this._playerShip);
				this._pursuerShip2.ShipOrder.SetBoardingTargetShip(this._playerShip);
			}
			else if (!this._pursuerShip2.GetIsConnected())
			{
				this._pursuerShip2.ShipOrder.SetShipEngageOrder(this._allyShip);
				this._pursuerShip2.ShipOrder.SetBoardingTargetShip(this._allyShip);
			}
			GameEntity gameEntity = this._waypoints[this._currentWaypointIndex];
			if ((gameEntity.GlobalPosition - this._allyShip.GlobalFrame.origin).LengthSquared <= 10000f)
			{
				this._currentWaypointIndex = (this._currentWaypointIndex + 1) % 6;
			}
			ShipOrder shipOrder = this._allyShip.ShipOrder;
			Vec2 asVec = gameEntity.GlobalPosition.AsVec2;
			shipOrder.SetShipMovementOrder(in asVec);
			if (!this._isAllyBoardedNotificationGiven && (this._allyShip.GetIsThereActiveBridgeTo(this._pursuerShip1) || this._allyShip.GetIsThereActiveBridgeTo(this._pursuerShip2)))
			{
				this._isAllyBoardedNotificationGiven = true;
				CampaignInformationManager.AddDialogLine(new TextObject("{=J83UkY9F}They’re boarding the Vlandians!", null), NavalStorylineData.Gunnar.CharacterObject, null, 0, 2);
			}
			if (!this._hasPlayerEngagedEnemyNotificationGiven && (this._playerShip.GetIsThereActiveBridgeTo(this._pursuerShip1) || this._playerShip.GetIsThereActiveBridgeTo(this._pursuerShip2)))
			{
				this._hasPlayerEngagedEnemyNotificationGiven = true;
				CampaignInformationManager.AddDialogLine(new TextObject("{=LABFnNwV}The grapples have caught. Cut them down!", null), NavalStorylineData.Gunnar.CharacterObject, null, 0, 2);
			}
		}

		// Token: 0x06000210 RID: 528 RVA: 0x0000ED00 File Offset: 0x0000CF00
		private void OnMerchantsAboutToBeBoarded()
		{
			if (!this._isAllyAboutToBeBoardedNotificationGiven)
			{
				this._isAllyAboutToBeBoardedNotificationGiven = true;
				CampaignInformationManager.AddDialogLine(new TextObject("{=Iy0a0ucw}I think the Vlandians are about to be overtaken and boarded.", null), NavalStorylineData.Gunnar.CharacterObject, null, 0, 2);
			}
		}

		// Token: 0x06000211 RID: 529 RVA: 0x0000ED30 File Offset: 0x0000CF30
		private MissionShip CreateMissionShip(Ship ship, string spawnPointId, Formation formation)
		{
			NavalShipsLogic missionBehavior = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			GameEntity gameEntity = this._entities.FirstOrDefault<GameEntity>((GameEntity t) => t.HasTag(spawnPointId));
			MatrixFrame globalFrame = gameEntity.GetGlobalFrame();
			float waterLevelAtPosition = Mission.Current.Scene.GetWaterLevelAtPosition(gameEntity.GlobalPosition.AsVec2, true, false);
			globalFrame.origin = new Vec3(gameEntity.GlobalPosition.x, gameEntity.GlobalPosition.y, waterLevelAtPosition, -1f);
			return missionBehavior.SpawnShip(ship, in globalFrame, formation.Team, formation, false, 8, true);
		}

		// Token: 0x06000212 RID: 530 RVA: 0x0000EDD0 File Offset: 0x0000CFD0
		private void SpawnPlayer()
		{
			WeakGameEntity weakGameEntity = this._playerShip.GameEntity.CollectChildrenEntitiesWithTag("sp_troop_captain").FirstOrDefault<WeakGameEntity>();
			Formation formation = base.Mission.PlayerTeam.GetFormation(0);
			AgentBuildData agentBuildData = new AgentBuildData(Hero.MainHero.CharacterObject).TroopOrigin(new SimpleAgentOrigin(Hero.MainHero.CharacterObject, -1, null, default(UniqueTroopDescriptor))).Team(base.Mission.PlayerTeam);
			Vec3 globalPosition = weakGameEntity.GlobalPosition;
			AgentBuildData agentBuildData2 = agentBuildData.InitialPosition(ref globalPosition);
			Vec2 asVec = weakGameEntity.GetGlobalFrame().rotation.f.AsVec2;
			AgentBuildData agentBuildData3 = agentBuildData2.InitialDirection(ref asVec).NoHorses(true).NoWeapons(false)
				.Formation(formation);
			Mission.Current.SpawnAgent(agentBuildData3, false).Controller = 2;
			this._agentsLogic.AddAgentToShip(Agent.Main, this._playerShip);
		}

		// Token: 0x06000213 RID: 531 RVA: 0x0000EEBC File Offset: 0x0000D0BC
		private void SpawnPlayerShipAgents()
		{
			NavalAgentsLogic missionBehavior = base.Mission.GetMissionBehavior<NavalAgentsLogic>();
			int num = 33;
			missionBehavior.SetDesiredTroopCountOfShip(this._playerShip, num + 1);
			CharacterObject @object = Campaign.Current.ObjectManager.GetObject<CharacterObject>("gangradirs_kin_melee");
			CharacterObject object2 = Campaign.Current.ObjectManager.GetObject<CharacterObject>("gangradirs_kin_ranged");
			int deckFrameCount = this._playerShip.DeckFrameCount;
			int num2 = 0;
			while (num2 < deckFrameCount && num2 < num)
			{
				CharacterObject characterObject = @object;
				if (num2 >= 32)
				{
					characterObject = object2;
				}
				MatrixFrame nextOuterInnerSpawnGlobalFrame = this._playerShip.GetNextOuterInnerSpawnGlobalFrame();
				AgentBuildData agentBuildData = new AgentBuildData(characterObject).TroopOrigin(new PartyAgentOrigin(PartyBase.MainParty, characterObject, -1, default(UniqueTroopDescriptor), false, true)).Team(base.Mission.PlayerTeam).InitialPosition(ref nextOuterInnerSpawnGlobalFrame.origin);
				Vec2 vec = nextOuterInnerSpawnGlobalFrame.rotation.f.AsVec2;
				vec = vec.Normalized();
				AgentBuildData agentBuildData2 = agentBuildData.InitialDirection(ref vec).NoHorses(true).NoWeapons(false);
				Agent agent = Mission.Current.SpawnAgent(agentBuildData2, false);
				missionBehavior.AddAgentToShip(agent, this._playerShip);
				num2++;
			}
		}

		// Token: 0x06000214 RID: 532 RVA: 0x0000EFE8 File Offset: 0x0000D1E8
		private void SpawnEnemyAgents(MissionShip ship, string troopType1, int troopType1Count, string troopType2, int troopType2Count)
		{
			int num = troopType1Count + troopType2Count;
			NavalAgentsLogic missionBehavior = base.Mission.GetMissionBehavior<NavalAgentsLogic>();
			missionBehavior.SetDesiredTroopCountOfShip(ship, num);
			CharacterObject @object = Campaign.Current.ObjectManager.GetObject<CharacterObject>(troopType1);
			CharacterObject object2 = Campaign.Current.ObjectManager.GetObject<CharacterObject>(troopType2);
			int deckFrameCount = ship.DeckFrameCount;
			for (int i = 0; i < deckFrameCount; i++)
			{
				CharacterObject characterObject = @object;
				if (i >= num)
				{
					break;
				}
				if (i >= troopType1Count)
				{
					characterObject = object2;
				}
				MatrixFrame nextOuterInnerSpawnGlobalFrame = ship.GetNextOuterInnerSpawnGlobalFrame();
				AgentBuildData agentBuildData = new AgentBuildData(characterObject).TroopOrigin(new PartyAgentOrigin(this._seaHoundsParty.Party, characterObject, -1, default(UniqueTroopDescriptor), false, true)).Team(base.Mission.PlayerEnemyTeam).InitialPosition(ref nextOuterInnerSpawnGlobalFrame.origin);
				Vec2 vec = nextOuterInnerSpawnGlobalFrame.rotation.f.AsVec2;
				vec = vec.Normalized();
				AgentBuildData agentBuildData2 = agentBuildData.InitialDirection(ref vec).NoHorses(true).NoWeapons(false);
				Agent agent = Mission.Current.SpawnAgent(agentBuildData2, false);
				missionBehavior.AddAgentToShip(agent, ship);
			}
		}

		// Token: 0x06000215 RID: 533 RVA: 0x0000F0FC File Offset: 0x0000D2FC
		private void SpawnAllyShipAgents(MissionShip ship)
		{
			NavalAgentsLogic missionBehavior = base.Mission.GetMissionBehavior<NavalAgentsLogic>();
			missionBehavior.SetDesiredTroopCountOfShip(ship, 12);
			CharacterObject @object = Campaign.Current.ObjectManager.GetObject<CharacterObject>("vlandian_fortune_seekers");
			int deckFrameCount = ship.DeckFrameCount;
			int num = 0;
			while (num < deckFrameCount && num < 12)
			{
				MatrixFrame nextOuterInnerSpawnGlobalFrame = ship.GetNextOuterInnerSpawnGlobalFrame();
				AgentBuildData agentBuildData = new AgentBuildData(@object).TroopOrigin(new PartyAgentOrigin(this._merchantParty.Party, @object, -1, default(UniqueTroopDescriptor), false, true)).Team(base.Mission.PlayerAllyTeam).InitialPosition(ref nextOuterInnerSpawnGlobalFrame.origin);
				Vec2 vec = nextOuterInnerSpawnGlobalFrame.rotation.f.AsVec2;
				vec = vec.Normalized();
				AgentBuildData agentBuildData2 = agentBuildData.InitialDirection(ref vec).NoHorses(true).NoWeapons(false);
				Agent agent = Mission.Current.SpawnAgent(agentBuildData2, false);
				missionBehavior.AddAgentToShip(agent, ship);
				ship.Formation.PlayerOwner = Agent.Main;
				num++;
			}
		}

		// Token: 0x06000216 RID: 534 RVA: 0x0000F1FC File Offset: 0x0000D3FC
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			if (this._isMissionFailed || this._isMissionSuccessful || Mission.Current.CurrentState != 2)
			{
				return;
			}
			if (this._isPursuer1ShipEngaged && this._agentsLogic.GetActiveAgentCountOfShip(this._pursuerShip1) == 0)
			{
				this.CalmAgentsOfShip(this._playerShip);
				this._isPursuer1ShipEngaged = false;
				if (!this._hasPlayerClearedFirstEnemyNotificationGiven)
				{
					this._hasPlayerClearedFirstEnemyNotificationGiven = true;
					CampaignInformationManager.AddDialogLine(new TextObject("{=Xjm7x5vu}Hah! That's the end of them! Now, about the other one…", null), NavalStorylineData.Gunnar.CharacterObject, null, 0, 2);
				}
			}
			if (this._isPursuer2ShipEngaged && this._agentsLogic.GetActiveAgentCountOfShip(this._pursuerShip2) == 0)
			{
				this.CalmAgentsOfShip(this._playerShip);
				this._isPursuer2ShipEngaged = false;
				if (!this._hasPlayerClearedSecondEnemyNotificationGiven)
				{
					this._hasPlayerClearedSecondEnemyNotificationGiven = true;
					CampaignInformationManager.AddDialogLine(new TextObject("{=2lX2bIwy}That's the last of them!", null), NavalStorylineData.Gunnar.CharacterObject, null, 0, 2);
				}
			}
			MBReadOnlyList<Agent> activeAgents = base.Mission.PlayerAllyTeam.ActiveAgents;
			if (activeAgents != null && !this._isDefeatQueued && !this._isVictoryQueued)
			{
				if ((float)activeAgents.Count <= 3.6000001f || Extensions.IsEmpty<Agent>(base.Mission.PlayerTeam.ActiveAgents))
				{
					this.StartDefeatSequence();
				}
				else if (activeAgents.Count == 6)
				{
					CampaignInformationManager.AddDialogLine(new TextObject("{=zdQoMBZd}Most of the Vlandians are down! We haven't much time!", null), NavalStorylineData.Gunnar.CharacterObject, null, 0, 2);
				}
			}
			if (!this._isMissionSuccessful)
			{
				MBReadOnlyList<Agent> activeAgentsOfShip = this._agentsLogic.GetActiveAgentsOfShip(this._pursuerShip1);
				MBReadOnlyList<Agent> activeAgentsOfShip2 = this._agentsLogic.GetActiveAgentsOfShip(this._pursuerShip2);
				if (activeAgentsOfShip != null && activeAgentsOfShip2 != null)
				{
					IEnumerable<Agent> enumerable = activeAgentsOfShip.Where<Agent>((Agent t) => t.Team == base.Mission.PlayerEnemyTeam);
					IEnumerable<Agent> enumerable2 = activeAgentsOfShip2.Where<Agent>((Agent t) => t.Team == base.Mission.PlayerEnemyTeam);
					if (Extensions.IsEmpty<Agent>(enumerable) && Extensions.IsEmpty<Agent>(enumerable2))
					{
						this.OnAllPursuingShipsDefeated();
					}
				}
			}
		}

		// Token: 0x06000217 RID: 535 RVA: 0x0000F3C4 File Offset: 0x0000D5C4
		private void CheckDrowningAgents(MissionShip ship)
		{
			foreach (Agent agent in this._agentsLogic.GetActiveAgentsOfShip(ship).ToList<Agent>())
			{
				if (!agent.IsMainAgent && agent.Team != base.Mission.PlayerTeam && agent.CurrentMortalityState == null && agent.IsInWater())
				{
					AgentNavalComponent component = agent.GetComponent<AgentNavalComponent>();
					if (component != null)
					{
						component.DrownAgent();
					}
				}
			}
		}

		// Token: 0x06000218 RID: 536 RVA: 0x0000F458 File Offset: 0x0000D658
		private void CalmAgentsOfShip(MissionShip targetShip)
		{
			foreach (Agent agent in this._agentsLogic.GetActiveAgentsOfShip(targetShip))
			{
				agent.SetAlarmState(0);
				AgentNavigator agentNavigator = agent.GetComponent<CampaignAgentComponent>().AgentNavigator;
				if (agentNavigator != null)
				{
					agentNavigator.RemoveBehaviorGroup<AlarmedBehaviorGroup>();
				}
			}
		}

		// Token: 0x06000219 RID: 537 RVA: 0x0000F4C8 File Offset: 0x0000D6C8
		private bool AreShipsWithinDistance(MissionShip ship1, MissionShip ship2, float distance)
		{
			return (ship1.GlobalFrame.origin - ship2.GlobalFrame.origin).LengthSquared <= distance * distance;
		}

		// Token: 0x0600021A RID: 538 RVA: 0x0000F500 File Offset: 0x0000D700
		private void OnAllPursuingShipsDefeated()
		{
			this._playerShip.ShipOrder.SetShipStopOrder();
			this._allyShip.ShipOrder.SetShipStopOrder();
			this._isVictoryQueued = true;
		}

		// Token: 0x0600021B RID: 539 RVA: 0x0000F52C File Offset: 0x0000D72C
		private void OpenVictoryPopUp()
		{
			object obj = new TextObject("{=R4Gqskgq}Victory", null);
			TextObject textObject = new TextObject("{=p0HTLZzH}After the last Sea Hound is defeated, the merchants approach you...", null);
			TextObject textObject2 = new TextObject("{=DM6luo3c}Continue", null);
			InformationManager.ShowInquiry(new InquiryData(obj.ToString(), textObject.ToString(), true, false, textObject2.ToString(), null, new Action(this.OnVictoryPopUpClosed), null, "", 0f, null, null, null), true, false);
		}

		// Token: 0x0600021C RID: 540 RVA: 0x0000F596 File Offset: 0x0000D796
		private void OnVictoryPopUpClosed()
		{
			this._isMissionSuccessful = true;
			PlayerEncounter.Battle.SetOverrideWinner(PlayerEncounter.Battle.PlayerSide);
			base.Mission.EndMission();
		}

		// Token: 0x0600021D RID: 541 RVA: 0x0000F5BE File Offset: 0x0000D7BE
		private void StartDefeatSequence()
		{
			this._isDefeatQueued = true;
			MBInformationManager.AddQuickInformation(new TextObject("{=fhEaEedK}Vlandian merchants have been destroyed.", null), 0, null, null, "");
		}

		// Token: 0x0600021E RID: 542 RVA: 0x0000F5DF File Offset: 0x0000D7DF
		private void StartDefeatFadeOut()
		{
			this.OnDefeatedEvent(1f);
		}

		// Token: 0x0600021F RID: 543 RVA: 0x0000F5F1 File Offset: 0x0000D7F1
		private void OnMissionFailed()
		{
			this._isMissionFailed = true;
			PlayerEncounter.Battle.SetOverrideWinner(PlayerEncounter.Battle.GetOtherSide(PlayerEncounter.Battle.PlayerSide));
			ScreenFadeController.BeginFadeIn(0.5f);
			base.Mission.EndMission();
		}

		// Token: 0x06000220 RID: 544 RVA: 0x0000F630 File Offset: 0x0000D830
		private void AddFightBehaviors(MissionShip ship)
		{
			foreach (Agent agent in this._agentsLogic.GetActiveAgentsOfShip(ship))
			{
				AgentFlag agentFlags = agent.GetAgentFlags();
				agent.SetAgentFlags(agentFlags | 65536);
				CampaignAgentComponent component = agent.GetComponent<CampaignAgentComponent>();
				AgentNavigator agentNavigator = component.AgentNavigator;
				if (agentNavigator == null)
				{
					agentNavigator = component.CreateAgentNavigator();
				}
				AlarmedBehaviorGroup alarmedBehaviorGroup = agentNavigator.GetBehaviorGroup<AlarmedBehaviorGroup>();
				if (alarmedBehaviorGroup == null)
				{
					alarmedBehaviorGroup = agentNavigator.AddBehaviorGroup<AlarmedBehaviorGroup>();
					alarmedBehaviorGroup.AddBehavior<FightBehavior>();
				}
				alarmedBehaviorGroup.SetScriptedBehavior<FightBehavior>();
				agent.SetAlarmState(3);
			}
		}

		// Token: 0x06000221 RID: 545 RVA: 0x0000F6D8 File Offset: 0x0000D8D8
		private bool HasSailThrust()
		{
			Vec2 globalWindVelocity = base.Mission.Scene.GetGlobalWindVelocity();
			MatrixFrame matrixFrame = ref this._playerShip.GameEntity.GetGlobalFrame();
			Vec3 vec = globalWindVelocity.ToVec3(0f);
			Vec2 vec2 = matrixFrame.rotation.TransformToLocal(ref vec).AsVec2.Normalized();
			List<MissionSail> sails = this._playerShip.Sails;
			float num = 0f;
			foreach (MissionSail missionSail in sails)
			{
				float num2 = -missionSail.SailObject.RightRotationLimit;
				float leftRotationLimit = missionSail.SailObject.LeftRotationLimit;
				float num3 = (leftRotationLimit - num2) * 0.01f;
				for (float num4 = num2; num4 <= leftRotationLimit; num4 += num3)
				{
					Vec2 forward = Vec2.Forward;
					forward.RotateCCW(num4);
					num += SailWindProfile.Instance.ComputeSailThrustValue(missionSail.SailObject.Type, forward, Vec2.Forward, vec2);
				}
			}
			return num > 0.1f;
		}

		// Token: 0x06000222 RID: 546 RVA: 0x0000F7FC File Offset: 0x0000D9FC
		public override bool MissionEnded(ref MissionResult missionResult)
		{
			bool flag = false;
			if (this._isMissionSuccessful)
			{
				missionResult = MissionResult.CreateSuccessful(base.Mission, true);
				flag = true;
			}
			else if (this._isMissionFailed)
			{
				missionResult = MissionResult.CreateDefeated(base.Mission);
				flag = true;
			}
			return flag;
		}

		// Token: 0x06000223 RID: 547 RVA: 0x0000F83D File Offset: 0x0000DA3D
		public void StartSpawner(BattleSideEnum side)
		{
		}

		// Token: 0x06000224 RID: 548 RVA: 0x0000F83F File Offset: 0x0000DA3F
		public void StopSpawner(BattleSideEnum side)
		{
		}

		// Token: 0x06000225 RID: 549 RVA: 0x0000F841 File Offset: 0x0000DA41
		public bool IsSideSpawnEnabled(BattleSideEnum side)
		{
			return false;
		}

		// Token: 0x06000226 RID: 550 RVA: 0x0000F844 File Offset: 0x0000DA44
		public float GetReinforcementInterval(BattleSideEnum side = -1)
		{
			return 0f;
		}

		// Token: 0x06000227 RID: 551 RVA: 0x0000F84B File Offset: 0x0000DA4B
		public bool IsSideDepleted(BattleSideEnum side)
		{
			return false;
		}

		// Token: 0x06000228 RID: 552 RVA: 0x0000F850 File Offset: 0x0000DA50
		public IEnumerable<IAgentOriginBase> GetAllTroopsForSide(BattleSideEnum side)
		{
			if (side == 1)
			{
				return Mission.Current.PlayerEnemyTeam.ActiveAgents.Select<Agent, IAgentOriginBase>((Agent t) => t.Origin);
			}
			if (side == null)
			{
				List<IAgentOriginBase> list = new List<IAgentOriginBase>();
				list.AddRange(Mission.Current.PlayerTeam.ActiveAgents.Select<Agent, IAgentOriginBase>((Agent t) => t.Origin));
				list.AddRange(Mission.Current.PlayerAllyTeam.ActiveAgents.Select<Agent, IAgentOriginBase>((Agent t) => t.Origin));
				return list;
			}
			return null;
		}

		// Token: 0x06000229 RID: 553 RVA: 0x0000F911 File Offset: 0x0000DB11
		public int GetNumberOfPlayerControllableTroops()
		{
			return 1;
		}

		// Token: 0x0600022A RID: 554 RVA: 0x0000F914 File Offset: 0x0000DB14
		public bool GetSpawnHorses(BattleSideEnum side)
		{
			return false;
		}

		// Token: 0x040000F9 RID: 249
		private const string PlayerShipId = "longship_storyline_q1";

		// Token: 0x040000FA RID: 250
		private const string AllyShipId = "ship_trade_cog_q1";

		// Token: 0x040000FB RID: 251
		private const string EnemyShip1Id = "northern_medium_ship";

		// Token: 0x040000FC RID: 252
		private const string EnemyShip2Id = "ship_lightlongship_q1";

		// Token: 0x040000FD RID: 253
		private const string AllyShipTroopType = "vlandian_fortune_seekers";

		// Token: 0x040000FE RID: 254
		private const int AllyShipTroopCount = 12;

		// Token: 0x040000FF RID: 255
		private const int PlayerShipTroopType1Count = 32;

		// Token: 0x04000100 RID: 256
		private const int PlayerShipTroopType2Count = 1;

		// Token: 0x04000101 RID: 257
		private const int EnemyShip1TroopType1Count = 28;

		// Token: 0x04000102 RID: 258
		private const string EnemyShip1TroopType2 = "sea_hounds";

		// Token: 0x04000103 RID: 259
		private const int EnemyShip1TroopType2Count = 2;

		// Token: 0x04000104 RID: 260
		private const int EnemyShip2TroopType1Count = 16;

		// Token: 0x04000105 RID: 261
		private const int EnemyShip2TroopType2Count = 2;

		// Token: 0x04000106 RID: 262
		private const string PlayerShipTroopType1 = "gangradirs_kin_melee";

		// Token: 0x04000107 RID: 263
		private const string PlayerShipTroopType2 = "gangradirs_kin_ranged";

		// Token: 0x04000108 RID: 264
		private const string EnemyShip1TroopType1 = "sea_hounds_pups";

		// Token: 0x04000109 RID: 265
		private MissionShip _playerShip;

		// Token: 0x0400010A RID: 266
		private const string EnemyShip2TroopType1 = "sea_hounds_pups";

		// Token: 0x0400010B RID: 267
		private MissionShip _allyShip;

		// Token: 0x0400010C RID: 268
		private const string EnemyShip2TroopType2 = "sea_hounds";

		// Token: 0x0400010D RID: 269
		private MissionShip _pursuerShip1;

		// Token: 0x0400010E RID: 270
		private const float WindStrength = 2f;

		// Token: 0x0400010F RID: 271
		private const int WayPointCount = 6;

		// Token: 0x04000110 RID: 272
		private const float AiPlayerEngagementDistance = 10f;

		// Token: 0x04000111 RID: 273
		private MissionObjectiveLogic _missionObjectiveLogic;

		// Token: 0x04000112 RID: 274
		private NavalAgentsLogic _agentsLogic;

		// Token: 0x04000113 RID: 275
		private const float ShipAgentsAlarmDistance = 30f;

		// Token: 0x04000114 RID: 276
		private const float DefeatFadeOutDelayDuration = 2f;

		// Token: 0x04000115 RID: 277
		private const float DefeatFadeOutDuration = 1f;

		// Token: 0x04000116 RID: 278
		private const float DefeatBlackScreenDuration = 2f;

		// Token: 0x04000117 RID: 279
		private static readonly Dictionary<string, string> PlayerShipUpgradePieces = new Dictionary<string, string>
		{
			{ "oars", "oars_wide_lvl3" },
			{ "sail", "sails_lvl2" },
			{ "side", "side_northern_shields_lvl2" }
		};

		// Token: 0x04000118 RID: 280
		private static readonly Dictionary<string, string> AllyShipUpgradePieces = new Dictionary<string, string>
		{
			{ "oars", "oars_wide_lvl3" },
			{ "sail", "sails_lvl2" }
		};

		// Token: 0x04000119 RID: 281
		private static readonly Dictionary<string, string> Enemy1ShipUpgradePieces = new Dictionary<string, string>
		{
			{ "sail", "sails_lvl2" },
			{ "side", "side_northern_shields_lvl1" }
		};

		// Token: 0x0400011A RID: 282
		private static readonly Dictionary<string, string> Enemy2ShipUpgradePieces = new Dictionary<string, string>
		{
			{ "sail", "sails_lvl2" },
			{ "side", "side_northern_shields_lvl1" }
		};

		// Token: 0x0400011B RID: 283
		private List<GameEntity> _entities = new List<GameEntity>();

		// Token: 0x0400011C RID: 284
		private MobileParty _merchantParty;

		// Token: 0x0400011D RID: 285
		private MobileParty _seaHoundsParty;

		// Token: 0x0400011E RID: 286
		private MissionShip _pursuerShip2;

		// Token: 0x0400011F RID: 287
		private List<GameEntity> _waypoints = new List<GameEntity>();

		// Token: 0x04000120 RID: 288
		private bool _isAllyBoardedNotificationGiven;

		// Token: 0x04000121 RID: 289
		private int _currentWaypointIndex;

		// Token: 0x04000122 RID: 290
		private bool _isMissionInitialized;

		// Token: 0x04000123 RID: 291
		private bool _isMissionSuccessful;

		// Token: 0x04000124 RID: 292
		private bool _isAllyAboutToBeBoardedNotificationGiven;

		// Token: 0x04000125 RID: 293
		private bool _hasPlayerEngagedEnemyNotificationGiven;

		// Token: 0x04000126 RID: 294
		private bool _hasPlayerClearedFirstEnemyNotificationGiven;

		// Token: 0x04000127 RID: 295
		private bool _hasPlayerClearedSecondEnemyNotificationGiven;

		// Token: 0x04000128 RID: 296
		private bool _isPursuer1ShipEngaged;

		// Token: 0x04000129 RID: 297
		private bool _isMissionFailed;

		// Token: 0x0400012A RID: 298
		private bool _isPursuer2ShipEngaged;

		// Token: 0x0400012B RID: 299
		private float _drownCheckTimer;

		// Token: 0x0400012C RID: 300
		private float _drownCheckDuration = 3f;

		// Token: 0x0400012D RID: 301
		private bool _isVictoryQueued;

		// Token: 0x0400012E RID: 302
		private float _victoryPopUpTimer;

		// Token: 0x0400012F RID: 303
		private float _victoryPopUpDelay = 3f;

		// Token: 0x04000130 RID: 304
		private bool _isDefeatQueued;

		// Token: 0x04000131 RID: 305
		private bool _isFadeOutTriggered;

		// Token: 0x04000132 RID: 306
		private float _defeatTimer;

		// Token: 0x04000133 RID: 307
		private float _notificationTimer;

		// Token: 0x04000134 RID: 308
		public Action OnShipsInitializedEvent;

		// Token: 0x04000135 RID: 309
		public Action<float> OnDefeatedEvent;
	}
}
