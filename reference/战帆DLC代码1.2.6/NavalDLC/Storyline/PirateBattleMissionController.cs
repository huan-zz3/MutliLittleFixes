using System;
using System.Collections.Generic;
using System.Linq;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using NavalDLC.Missions.Objects.UsableMachines;
using NavalDLC.Missions.ShipActuators;
using NavalDLC.Missions.ShipControl;
using NavalDLC.Storyline.Objectives.PirateBattle;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.MissionLogics;
using TaleWorlds.ObjectSystem;

namespace NavalDLC.Storyline
{
	// Token: 0x02000030 RID: 48
	public class PirateBattleMissionController : MissionLogic
	{
		// Token: 0x14000004 RID: 4
		// (add) Token: 0x0600029A RID: 666 RVA: 0x00013D8C File Offset: 0x00011F8C
		// (remove) Token: 0x0600029B RID: 667 RVA: 0x00013DC4 File Offset: 0x00011FC4
		public event Action<float, float> OnBeginScreenFadeEvent;

		// Token: 0x14000005 RID: 5
		// (add) Token: 0x0600029C RID: 668 RVA: 0x00013DFC File Offset: 0x00011FFC
		// (remove) Token: 0x0600029D RID: 669 RVA: 0x00013E34 File Offset: 0x00012034
		public event Action<float> OnCameraBearingNeedsUpdateEvent;

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x0600029E RID: 670 RVA: 0x00013E6C File Offset: 0x0001206C
		// (remove) Token: 0x0600029F RID: 671 RVA: 0x00013EA4 File Offset: 0x000120A4
		public event Action OnShipsInitializedEvent;

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x060002A0 RID: 672 RVA: 0x00013ED9 File Offset: 0x000120D9
		// (set) Token: 0x060002A1 RID: 673 RVA: 0x00013EE1 File Offset: 0x000120E1
		public bool IsFirstShipCleared { get; private set; }

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x060002A2 RID: 674 RVA: 0x00013EEA File Offset: 0x000120EA
		// (set) Token: 0x060002A3 RID: 675 RVA: 0x00013EF2 File Offset: 0x000120F2
		public bool HasSelectedShip { get; private set; }

		// Token: 0x060002A4 RID: 676 RVA: 0x00013EFB File Offset: 0x000120FB
		public PirateBattleMissionController(MobileParty pirateParty, int pirateTroopCount)
		{
			this._pirateParty = pirateParty;
			this._pirateTroopCount = pirateTroopCount;
		}

		// Token: 0x060002A5 RID: 677 RVA: 0x00013F28 File Offset: 0x00012128
		public override void OnMissionTick(float dt)
		{
			if (!this._isMissionInitialized)
			{
				this._isMissionInitialized = true;
				this.UpdateEntityReferences();
				Team team = Mission.GetTeam(0);
				Formation formation = team.GetFormation(0);
				Formation formation2 = Mission.GetTeam(2).GetFormation(0);
				this._playerShip = this.CreateShip("ship_knarr_storyline_2", "spawnpoint_ship_player", formation, PartyBase.MainParty, PirateBattleMissionController.PlayerShipUpgradePieces, "generated_square__h4_09");
				this._secondShip = this.CreateShip("ship_lightlongship_storyline", "spawnpoint_ship_first_enemy", formation2, this._pirateParty.Party, PirateBattleMissionController.SecondShipUpgradePieces, "generated_square_l1_h4_10");
				this._navalShipsLogic.TeleportShip(this._playerShip, this._playerShip.GlobalFrame, false, false, true);
				this._navalShipsLogic.TeleportShip(this._secondShip, this._secondShip.GlobalFrame, false, false, true);
				this.UpdateEntityReferences();
				this.SpawnAllyTroops();
				this.SpawnEnemyAgents(this._secondShip);
				team.SetPlayerRole(true, true);
				this._navalAgentsLogic.AssignCaptainToShipForDeploymentMode(Agent.Main, this._playerShip, null);
				this._playerShip.ShipOrder.SetOrderOarsmenLevel(2);
				this._navalAgentsLogic.AssignAndTeleportCrewToShipMachines(0);
				this._navalAgentsLogic.AssignAndTeleportCrewToShipMachines(2);
				Mission.Current.OnDeploymentFinished();
				this._secondShip.SetAnchor(true, false, 1f);
				this._secondShip.ShipOrder.SetShipStopOrder();
				this._secondShip.SetController(ShipControllerType.None, false);
				this._secondShip.Formation.SetControlledByAI(false, false);
				this._secondShip.SetCanBeTakenOver(false);
				TextObject textObject = new TextObject("{=xz5vyQlF}They must think we're just a fishing vessel. All right now, boys, let's show them that their prey has teeth of its own!", null);
				this.ShowNotification(textObject);
				PirateBattlePhase1Objective pirateBattlePhase1Objective = new PirateBattlePhase1Objective(Mission.Current, this);
				this._missionObjectiveLogic.StartObjective(pirateBattlePhase1Objective);
				Mission.Current.PlayerTeam.PlayerOrderController.OnOrderIssued += new OnOrderIssuedDelegate(this.OnPlayerOrdered);
				this.OnShipsInitializedEvent();
				Vec2 vec = this._entities.FirstOrDefault<GameEntity>((GameEntity t) => t.HasTag("sp_wind")).GetGlobalFrame().rotation.f.NormalizedCopy().AsVec2 * 1.5f;
				Mission.Current.Scene.SetGlobalWindStrengthVector(ref vec);
				MBMusicManager.Current.StartThemeWithConstantIntensity(10242, false);
				MBMusicManager.Current.ChangeCurrentThemeIntensity(0.2f);
			}
			if (this._defeatTimer != null && this._defeatTimer.Check(false))
			{
				this._defeatTimer = null;
				this.OnPlayerTeamDefeated();
			}
			if (this._victoryTimer != null && this._victoryTimer.Check(false))
			{
				this._victoryTimer = null;
				this.OnEnemyTeamDefeated();
			}
			if (this._isInSecondPhase && this.HasSelectedShip)
			{
				if (!this._isGunnarAfterFightFirstNotificationShown)
				{
					this._isGunnarAfterFightFirstNotificationShown = true;
					this._currentNotificationText = new TextObject("{=Ni85tv1G}I think I see them. Untie our ships, and let’s have at it!", null);
				}
				else if (!this._isGunnarAfterFightSecondNotificationShown && !this._playerShip.GetIsThereActiveBridgeTo(this._secondShip))
				{
					this._isGunnarAfterFightSecondNotificationShown = true;
					this._currentNotificationText = new TextObject("{=BfzIsraW}I’ll let you decide how to fight this one. Maneuver a bit, or just go straight at them?", null);
					PirateBattlePhase2Objective pirateBattlePhase2Objective = new PirateBattlePhase2Objective(Mission.Current, this);
					this._missionObjectiveLogic.StartObjective(pirateBattlePhase2Objective);
				}
			}
			this._notificationTimer += dt;
			if (!this._isInSecondPhase)
			{
				if (this._playerShip.GetIsConnectedToEnemy())
				{
					if (!this._hasShownChargeNotification)
					{
						this.ShowChargeNotification();
					}
				}
				else if ((this._playerShip.GameEntity.GlobalPosition - this._secondShip.GameEntity.GlobalPosition).LengthSquared >= 2500f)
				{
					this._hasShownBoardImminentNotification = false;
					if (this._notificationTimer > 27f)
					{
						this._notificationTimer = 0f;
						this._currentNotificationText = new TextObject("{=gMhrY6rz}Get us close so we can board.", null);
					}
				}
				else if (!this._hasShownBoardImminentNotification)
				{
					this._hasShownBoardImminentNotification = true;
					this._currentNotificationText = new TextObject("{=GtSpVtOq}Get ready to board…", null);
					MBMusicManager.Current.ChangeCurrentThemeIntensity(0.5f);
				}
			}
			else
			{
				if (!this._hasShownSecondPhaseChargeNotification && this._isGunnarAfterFightSecondNotificationShown && (this._playerShip.GetIsConnectedToEnemy() || this._secondShip.GetIsConnectedToEnemy()))
				{
					this.ShowSecondPhaseChargeNotification();
				}
				if (this._playerShip.GetIsConnectedToEnemy() && !this._hasIncreasedMusicIntensityForSecondPhase)
				{
					MBMusicManager.Current.ChangeCurrentThemeIntensity(0.5f);
					this._hasIncreasedMusicIntensityForSecondPhase = true;
				}
			}
			if (this._currentNotificationText != null)
			{
				this.ShowNotification(this._currentNotificationText);
			}
			if (this._isDialogueQueued)
			{
				this._dialogueTimer += dt;
				if (!this._isSecondPhaseSetup && this._dialogueTimer > 0.5f)
				{
					this.SetupSecondPhase();
				}
				if (this._dialogueTimer > 1.25f)
				{
					this.StartDialogue();
				}
			}
			if (this._isShipTransferQueued)
			{
				this._afterFightShipChangeTimer += dt;
				if (this._afterFightShipChangeTimer >= 0.5f)
				{
					this._isShipTransferQueued = false;
					this.HandleShipSelection(!this._isSecondShipSelected);
				}
			}
		}

		// Token: 0x060002A6 RID: 678 RVA: 0x00014427 File Offset: 0x00012627
		private void UpdateEntityReferences()
		{
			base.Mission.Scene.GetEntities(ref this._entities);
		}

		// Token: 0x060002A7 RID: 679 RVA: 0x00014440 File Offset: 0x00012640
		public override void OnBehaviorInitialize()
		{
			if (!SailWindProfile.IsSailWindProfileInitialized)
			{
				SailWindProfile.InitializeProfile();
			}
			this._navalAgentsLogic = base.Mission.GetMissionBehavior<NavalAgentsLogic>();
			this._navalShipsLogic = base.Mission.GetMissionBehavior<NavalShipsLogic>();
			this._missionObjectiveLogic = base.Mission.GetMissionBehavior<MissionObjectiveLogic>();
		}

		// Token: 0x060002A8 RID: 680 RVA: 0x0001448C File Offset: 0x0001268C
		private void SpawnAllyTroops()
		{
			CharacterObject @object = Campaign.Current.ObjectManager.GetObject<CharacterObject>("gangradirs_kin_melee");
			CharacterObject object2 = Campaign.Current.ObjectManager.GetObject<CharacterObject>("gangradirs_kin_ranged");
			this._navalAgentsLogic.SetDesiredTroopCountOfShip(this._playerShip, 22);
			this._navalAgentsLogic.AddReservedTroopToShip(new PartyAgentOrigin(PartyBase.MainParty, NavalStorylineData.Gunnar.CharacterObject, -1, default(UniqueTroopDescriptor), false, false), this._playerShip);
			this._navalAgentsLogic.AddReservedTroopToShip(new PartyAgentOrigin(PartyBase.MainParty, CharacterObject.PlayerCharacter, -1, default(UniqueTroopDescriptor), false, false), this._playerShip);
			for (int i = 0; i < 10; i++)
			{
				this._navalAgentsLogic.AddReservedTroopToShip(new PartyAgentOrigin(PartyBase.MainParty, @object, -1, default(UniqueTroopDescriptor), false, true), this._playerShip);
			}
			for (int j = 0; j < 10; j++)
			{
				this._navalAgentsLogic.AddReservedTroopToShip(new PartyAgentOrigin(PartyBase.MainParty, object2, -1, default(UniqueTroopDescriptor), false, true), this._playerShip);
			}
			this._navalAgentsLogic.SpawnNextBatch(0, false, null);
			this._gunnarAgent = base.Mission.Agents.FirstOrDefault<Agent>((Agent x) => x.Character == NavalStorylineData.Gunnar.CharacterObject);
			this._gunnarAgent.ToggleInvulnerable();
			NavalStorylineData.Gunnar.SetHasMet();
			this._playerShip.Formation.PlayerOwner = Agent.Main;
			Mission.Current.PlayerTeam.PlayerOrderController.Owner = Agent.Main;
		}

		// Token: 0x060002A9 RID: 681 RVA: 0x0001462C File Offset: 0x0001282C
		private Agent SpawnHero(CharacterObject character, string spawnPointTag)
		{
			GameEntity gameEntity = this._entities.FirstOrDefault<GameEntity>((GameEntity t) => t.HasTag(spawnPointTag));
			AgentBuildData agentBuildData = new AgentBuildData(character).TroopOrigin(new PartyAgentOrigin(PartyBase.MainParty, character, -1, default(UniqueTroopDescriptor), false, true)).Team(base.Mission.PlayerTeam);
			Vec3 globalPosition = gameEntity.GlobalPosition;
			AgentBuildData agentBuildData2 = agentBuildData.InitialPosition(ref globalPosition);
			Vec2 asVec = gameEntity.GetGlobalFrame().rotation.f.AsVec2;
			AgentBuildData agentBuildData3 = agentBuildData2.InitialDirection(ref asVec).NoHorses(true).NoWeapons(false);
			return Mission.Current.SpawnAgent(agentBuildData3, false);
		}

		// Token: 0x060002AA RID: 682 RVA: 0x000146DC File Offset: 0x000128DC
		private void SpawnEnemyAgents(MissionShip ship)
		{
			CharacterObject @object = Campaign.Current.ObjectManager.GetObject<CharacterObject>("sea_hounds_pups");
			this._navalAgentsLogic.SetDesiredTroopCountOfShip(ship, this._pirateTroopCount);
			for (int i = 0; i < this._pirateTroopCount; i++)
			{
				PartyAgentOrigin partyAgentOrigin = new PartyAgentOrigin(this._pirateParty.Party, @object, -1, default(UniqueTroopDescriptor), false, true);
				partyAgentOrigin.SetBanner(NavalStorylineData.CorsairBanner);
				this._navalAgentsLogic.AddReservedTroopToShip(partyAgentOrigin, ship);
			}
			this._navalAgentsLogic.SpawnNextBatch(2, false, null);
		}

		// Token: 0x060002AB RID: 683 RVA: 0x00014768 File Offset: 0x00012968
		private void SpawnAllyPrisonerAgents(MissionShip ship)
		{
			CharacterObject @object = Campaign.Current.ObjectManager.GetObject<CharacterObject>("gangradirs_kin_melee");
			CharacterObject object2 = Campaign.Current.ObjectManager.GetObject<CharacterObject>("gangradirs_kin_ranged");
			this._navalAgentsLogic.SetDesiredTroopCountOfShip(ship, 16);
			for (int i = 0; i < 7; i++)
			{
				this._navalAgentsLogic.AddReservedTroopToShip(new PartyAgentOrigin(PartyBase.MainParty, @object, -1, default(UniqueTroopDescriptor), false, true), ship);
			}
			for (int j = 0; j < 7; j++)
			{
				this._navalAgentsLogic.AddReservedTroopToShip(new PartyAgentOrigin(PartyBase.MainParty, object2, -1, default(UniqueTroopDescriptor), false, true), ship);
			}
			this._navalAgentsLogic.SpawnNextBatch(0, false, null);
		}

		// Token: 0x060002AC RID: 684 RVA: 0x00014820 File Offset: 0x00012A20
		private MissionShip CreateShip(string shipHullId, string spawnPointId, Formation formation, PartyBase owner, Dictionary<string, string> upgradePieces, string materialName)
		{
			Ship ship = new Ship(Campaign.Current.ObjectManager.GetObject<ShipHull>(shipHullId));
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
			GameEntity gameEntity = this._entities.FirstOrDefault<GameEntity>((GameEntity t) => t.HasTag(spawnPointId));
			MatrixFrame globalFrame = gameEntity.GetGlobalFrame();
			float waterLevelAtPosition = Mission.Current.Scene.GetWaterLevelAtPosition(gameEntity.GlobalPosition.AsVec2, true, false);
			globalFrame.origin = new Vec3(gameEntity.GlobalPosition.x, gameEntity.GlobalPosition.y, waterLevelAtPosition, -1f);
			MissionShip missionShip = this._navalShipsLogic.SpawnShip(ship, in globalFrame, formation.Team, formation, false, 8, true);
			this.ChangeShipColors(missionShip, owner.MapFaction.Color, owner.MapFaction.Color2, materialName);
			return missionShip;
		}

		// Token: 0x060002AD RID: 685 RVA: 0x00014968 File Offset: 0x00012B68
		private void ChangeShipColors(MissionShip missionShip, uint color1, uint color2, string materialName)
		{
			foreach (GameEntity gameEntity in missionShip.SailMeshEntities)
			{
				this.SetSailColors(gameEntity, color1, color2, materialName);
			}
		}

		// Token: 0x060002AE RID: 686 RVA: 0x000149C0 File Offset: 0x00012BC0
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

		// Token: 0x060002AF RID: 687 RVA: 0x00014A9C File Offset: 0x00012C9C
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			if ((this.IsFirstShipCleared ? (this._reinforcementShip != null && this.IsShipEffectivelyDepleted(this._reinforcementShip)) : this.IsShipEffectivelyDepleted(this._secondShip)) && this._defeatTimer == null)
			{
				this._victoryTimer = new MissionTimer(3f);
			}
			if (Extensions.IsEmpty<Agent>(base.Mission.PlayerTeam.ActiveAgents) || affectedAgent.IsMainAgent)
			{
				this._defeatTimer = new MissionTimer(3f);
				this._victoryTimer = null;
			}
		}

		// Token: 0x060002B0 RID: 688 RVA: 0x00014B28 File Offset: 0x00012D28
		private bool IsShipEffectivelyDepleted(MissionShip ship)
		{
			bool flag = true;
			using (List<Agent>.Enumerator enumerator = this._navalAgentsLogic.GetActiveAgentsOfShip(ship).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.IsInWater())
					{
						flag = false;
						break;
					}
				}
			}
			return flag;
		}

		// Token: 0x060002B1 RID: 689 RVA: 0x00014B88 File Offset: 0x00012D88
		private void OnEnemyTeamDefeated()
		{
			if (!this.IsFirstShipCleared)
			{
				this.IsFirstShipCleared = true;
				this.OnFirstEnemyShipCleared();
				return;
			}
			this.OnSecondEnemyShipCleared();
		}

		// Token: 0x060002B2 RID: 690 RVA: 0x00014BA6 File Offset: 0x00012DA6
		private void ShowNotification(TextObject text)
		{
			CampaignInformationManager.AddDialogLine(text, NavalStorylineData.Gunnar.CharacterObject, null, 0, 2);
			this._currentNotificationText = null;
		}

		// Token: 0x060002B3 RID: 691 RVA: 0x00014BC4 File Offset: 0x00012DC4
		private void OnFirstEnemyShipCleared()
		{
			if (Agent.Main.IsUsingGameObject)
			{
				Agent.Main.StopUsingGameObject(true, 1);
			}
			object obj = new TextObject("{=pn7YqjAE}Ship Cleared", null);
			TextObject textObject = new TextObject("{=6UauyvuX}Your men make quick work of the pirates. As the fighting dies down, you find that the Sea Hounds were carrying captives, bound and stashed beneath the rowing benches. You cut their bonds and help them to their feet as your lookouts scan the waters for any sign of the second ship.", null);
			TextObject textObject2 = new TextObject("{=DM6luo3c}Continue", null);
			InquiryData inquiryData = new InquiryData(obj.ToString(), textObject.ToString(), true, false, textObject2.ToString(), null, new Action(this.OnFirstFightPopUpClosed), null, "", 0f, null, null, null);
			MBMusicManager.Current.ChangeCurrentThemeIntensity(-0.5f);
			InformationManager.ShowInquiry(inquiryData, Campaign.Current.GameMode == 1, false);
		}

		// Token: 0x060002B4 RID: 692 RVA: 0x00014C61 File Offset: 0x00012E61
		private void OnFirstFightPopUpClosed()
		{
			this._isDialogueQueued = true;
			Action<float, float> onBeginScreenFadeEvent = this.OnBeginScreenFadeEvent;
			if (onBeginScreenFadeEvent == null)
			{
				return;
			}
			onBeginScreenFadeEvent(0.5f, 0.75f);
		}

		// Token: 0x060002B5 RID: 693 RVA: 0x00014C84 File Offset: 0x00012E84
		private void SetupSecondPhase()
		{
			this._isSecondPhaseSetup = true;
			Formation formation = Mission.GetTeam(2).GetFormation(1);
			this._reinforcementShip = this.CreateShip("ship_lightlongship_storyline", "spawnpoint_ship_reinforcement", formation, this._pirateParty.Party, PirateBattleMissionController.ReinforcementShipUpgradePieces, "generated_square_l1_h4_10");
			this._reinforcementShip.OnDeploymentFinished();
			this.SpawnEnemyAgents(this._reinforcementShip);
			MatrixFrame globalFrame = this._playerShip.GlobalFrame;
			Vec2 asVec = globalFrame.origin.AsVec2;
			Vec2 vec = globalFrame.rotation.f.AsVec2;
			Vec2 vec2 = vec.Normalized();
			this._playerShip.SetAnchor(true, false, 1f);
			this._playerShip.SetAnchorFrame(in asVec, in vec2, 1f);
			if (this._gunnarAgent == null || !this._gunnarAgent.IsActive())
			{
				this._gunnarAgent = this.SpawnHero(NavalStorylineData.Gunnar.CharacterObject, "conversation_ally");
				this._gunnarAgent.ToggleInvulnerable();
			}
			this._gunnarAgent.TryToSheathWeaponInHand(1, 1);
			this._gunnarAgent.TryToSheathWeaponInHand(0, 1);
			Agent.Main.TryToSheathWeaponInHand(1, 1);
			Agent.Main.TryToSheathWeaponInHand(0, 1);
			this._playerShip.ShipOrder.SetOrderOarsmenLevel(2);
			this._navalAgentsLogic.SetDeploymentMode(true);
			this._navalAgentsLogic.AssignAndTeleportCrewToShipMachines(this._playerShip);
			this._navalAgentsLogic.SetDeploymentMode(false);
			if (Agent.Main.IsUsingGameObject)
			{
				Agent.Main.StopUsingGameObject(true, 1);
			}
			if (this._gunnarAgent.IsUsingGameObject)
			{
				this._gunnarAgent.StopUsingGameObject(true, 1);
			}
			this._gunnarAgent.TryAttachToFormation();
			this._gunnarAgent.SetActionChannel(0, ref ActionIndexCache.act_none, true, 0L, 0f, 1f, 0f, 0.4f, 0f, false, -0.2f, 0, true);
			this._gunnarAgent.SetActionChannel(1, ref ActionIndexCache.act_none, true, 0L, 0f, 1f, 0f, 0.4f, 0f, false, -0.2f, 0, true);
			Agent.Main.SetActionChannel(0, ref ActionIndexCache.act_none, true, 0L, 0f, 1f, 0f, 0.4f, 0f, false, -0.2f, 0, true);
			Agent.Main.SetActionChannel(1, ref ActionIndexCache.act_none, true, 0L, 0f, 1f, 0f, 0.4f, 0f, false, -0.2f, 0, true);
			GameEntity gameEntity = this._entities.Last<GameEntity>((GameEntity t) => t.HasTag("conversation_ally"));
			this._gunnarAgent.TeleportToPosition(gameEntity.GlobalPosition);
			GameEntity gameEntity2 = this._entities.Last<GameEntity>((GameEntity t) => t.HasTag("conversation_player"));
			base.Mission.MainAgent.TeleportToPosition(gameEntity2.GlobalPosition);
			Agent.Main.SetLookAgent(this._gunnarAgent);
			Vec3 vec3 = Agent.Main.Position - this._gunnarAgent.Position;
			Agent gunnarAgent = this._gunnarAgent;
			vec = vec3.AsVec2;
			vec = vec.Normalized();
			gunnarAgent.SetMovementDirection(ref vec);
			this._gunnarAgent.SetLookAgent(Agent.Main);
			this._gunnarAgent.Controller = 0;
			this.OnCameraBearingNeedsUpdateEvent((-vec3).RotationZ);
			this._reinforcementShip.SetAnchor(true, false, 1f);
			this._reinforcementShip.ShipOrder.SetShipStopOrder();
			this._reinforcementShip.SetController(ShipControllerType.AI, true);
			this._reinforcementShip.SetCanBeTakenOver(false);
			Agent.Main.Health = Agent.Main.HealthLimit;
			foreach (ShipAttachmentPointMachine shipAttachmentPointMachine in MBExtensions.FindAllWithType<ShipAttachmentPointMachine>(base.Mission.ActiveMissionObjects).ToList<ShipAttachmentPointMachine>())
			{
				shipAttachmentPointMachine.PilotStandingPoint.IsDisabledForPlayers = true;
			}
			foreach (ShipAttachmentMachine shipAttachmentMachine in MBExtensions.FindAllWithType<ShipAttachmentMachine>(base.Mission.ActiveMissionObjects).ToList<ShipAttachmentMachine>())
			{
				shipAttachmentMachine.PilotStandingPoint.IsDisabledForPlayers = true;
			}
		}

		// Token: 0x060002B6 RID: 694 RVA: 0x00015100 File Offset: 0x00013300
		private void StartDialogue()
		{
			this._isDialogueQueued = false;
			Campaign.Current.ConversationManager.SetupAndStartMissionConversation(this._gunnarAgent, base.Mission.MainAgent, true);
			base.Mission.SetMissionMode(1, true);
		}

		// Token: 0x060002B7 RID: 695 RVA: 0x00015137 File Offset: 0x00013337
		public void OnPlayerSelectedFirstShipToCommand()
		{
			this._isSecondShipSelected = false;
			this.OnPlayerSelectedShipToCommand();
		}

		// Token: 0x060002B8 RID: 696 RVA: 0x00015146 File Offset: 0x00013346
		public void OnPlayerSelectedSecondShipToCommand()
		{
			this._isSecondShipSelected = true;
			this.OnPlayerSelectedShipToCommand();
		}

		// Token: 0x060002B9 RID: 697 RVA: 0x00015158 File Offset: 0x00013358
		private void OnPlayerSelectedShipToCommand()
		{
			this._isInSecondPhase = true;
			this._isShipTransferQueued = true;
			PirateBattleCutLooseObjective pirateBattleCutLooseObjective = new PirateBattleCutLooseObjective(Mission.Current, this);
			this._missionObjectiveLogic.StartObjective(pirateBattleCutLooseObjective);
			Action<float, float> onBeginScreenFadeEvent = this.OnBeginScreenFadeEvent;
			if (onBeginScreenFadeEvent == null)
			{
				return;
			}
			onBeginScreenFadeEvent(0.5f, 0.75f);
		}

		// Token: 0x060002BA RID: 698 RVA: 0x000151A8 File Offset: 0x000133A8
		private void HandleShipSelection(bool isFirstShipSelected)
		{
			this.HasSelectedShip = true;
			this._playerShip.SetAnchor(false, false, 1f);
			this._secondShip.SetAnchor(false, false, 1f);
			this._playerShip.SetController(isFirstShipSelected ? ShipControllerType.Player : ShipControllerType.AI, true);
			this._secondShip.SetController(isFirstShipSelected ? ShipControllerType.AI : ShipControllerType.Player, true);
			base.Mission.SetMissionMode(2, true);
			this._playerShip.ShipOrder.SetShipStopOrder();
			this._secondShip.ShipOrder.SetShipStopOrder();
			this._secondShip.BreakAllExistingConnections();
			MatrixFrame bodyWorldTransform = this._playerShip.GameEntity.GetBodyWorldTransform();
			bodyWorldTransform.rotation.u = Vec3.Up;
			bodyWorldTransform.rotation.f = bodyWorldTransform.rotation.s.CrossProductWithUpAsLeftParameter().NormalizedCopy();
			bodyWorldTransform.rotation.s = bodyWorldTransform.rotation.f.CrossProductWithUp();
			bodyWorldTransform.origin += bodyWorldTransform.rotation.s * (this._playerShip.Physics.PhysicsBoundingBoxSizeWithoutChildren.x * 0.5f + this._secondShip.Physics.PhysicsBoundingBoxSizeWithoutChildren.x * 0.5f + 1f);
			this._navalShipsLogic.TeleportShip(this._secondShip, bodyWorldTransform, false, false, true);
			this._secondShip.TryToMaintainConnectionToAnotherShip(this._playerShip, true, false);
			if (isFirstShipSelected)
			{
				this._navalShipsLogic.TransferShipToTeam(this._secondShip, base.Mission.PlayerTeam, null, 8);
			}
			else
			{
				Formation formation = this._playerShip.Formation;
				Formation formation2 = base.Mission.PlayerTeam.GetFormation(1);
				this._navalShipsLogic.TransferShipToFormation(this._playerShip, formation2);
				this._navalShipsLogic.TransferShipToTeam(this._secondShip, base.Mission.PlayerTeam, formation, 8);
			}
			this._playerShip.Formation.PlayerOwner = Agent.Main;
			this._secondShip.Formation.PlayerOwner = Agent.Main;
			MissionShip missionShip = (isFirstShipSelected ? this._secondShip : this._playerShip);
			MissionShip missionShip2;
			bool flag = this._navalAgentsLogic.IsAgentOnAnyShip(this._gunnarAgent, out missionShip2, 0);
			if (flag && missionShip2 != missionShip)
			{
				this._navalAgentsLogic.TransferAgentToShip(this._gunnarAgent, missionShip);
			}
			else if (!flag)
			{
				this._navalAgentsLogic.AddAgentToShip(this._gunnarAgent, missionShip);
			}
			MissionShip missionShip3 = (isFirstShipSelected ? this._playerShip : this._secondShip);
			Team team = Agent.Main.Team;
			foreach (Agent agent in team.ActiveAgents)
			{
				if (agent != this._gunnarAgent)
				{
					MissionShip missionShip4;
					bool flag2 = this._navalAgentsLogic.IsAgentOnAnyShip(agent, out missionShip4, team.TeamSide);
					if (flag2 && missionShip4 != missionShip3)
					{
						this._navalAgentsLogic.TransferAgentToShip(agent, missionShip3);
					}
					else if (!flag2)
					{
						this._navalAgentsLogic.AddAgentToShip(agent, missionShip3);
					}
				}
			}
			this.ReplenishPlayerShipTroops();
			this.SpawnAllyPrisonerAgents(isFirstShipSelected ? this._secondShip : this._playerShip);
			this._navalAgentsLogic.AssignCaptainToShip(Agent.Main, missionShip3, null);
			this._navalAgentsLogic.AssignCaptainToShip(this._gunnarAgent, missionShip, null);
			this._playerShip.Formation.SetControlledByAI(false, false);
			this._secondShip.Formation.SetControlledByAI(false, false);
			this._playerShip.ShipOrder.SetCutLoose(false);
			this._secondShip.ShipOrder.SetCutLoose(false);
			this._playerShip.ShipOrder.SetBoardingTargetShip(null);
			this._secondShip.ShipOrder.SetBoardingTargetShip(null);
			this._playerShip.ShipOrder.MakeEnemyOnShipExpire();
			this._secondShip.ShipOrder.MakeEnemyOnShipExpire();
			this._playerShip.ShipOrder.SetOrderOarsmenLevel(2);
			this._secondShip.ShipOrder.SetOrderOarsmenLevel(2);
			this._gunnarAgent.Controller = 1;
			string keyHyperlinkText = HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("MissionOrderHotkeyCategory", 80), 1f);
			GameTexts.SetVariable("SHIP_COMMANDING_TUTORIAL_GROUP_KEY", keyHyperlinkText);
			this._navalAgentsLogic.SetDeploymentMode(true);
			this._navalShipsLogic.SetDeploymentMode(true);
			this._playerShip.ShipOrder.Tick();
			this._secondShip.ShipOrder.Tick();
			this._navalAgentsLogic.AssignAndTeleportCrewToShipMachines(0);
			this._navalAgentsLogic.SetDeploymentMode(false);
			this._navalShipsLogic.SetDeploymentMode(false);
			this._playerShip.ShipControllerMachine.PilotStandingPoint.IsDisabledForPlayers = false;
			this._secondShip.ShipControllerMachine.PilotStandingPoint.IsDisabledForPlayers = false;
			Vec3 vec = this._reinforcementShip.GameEntity.GlobalPosition - Agent.Main.Position;
			this.OnCameraBearingNeedsUpdateEvent(vec.RotationZ);
		}

		// Token: 0x060002BB RID: 699 RVA: 0x000156B8 File Offset: 0x000138B8
		private void ReplenishPlayerShipTroops()
		{
			int count = Agent.Main.Team.ActiveAgents.Count;
			int num = 14 - count;
			if (num > 0)
			{
				CharacterObject @object = Campaign.Current.ObjectManager.GetObject<CharacterObject>("gangradirs_kin_melee");
				CharacterObject object2 = Campaign.Current.ObjectManager.GetObject<CharacterObject>("gangradirs_kin_ranged");
				int num2 = num / 2;
				int num3 = num / 2;
				num2 += num - (num2 + num3);
				for (int i = 0; i < num2; i++)
				{
					this._navalAgentsLogic.AddReservedTroopToShip(new PartyAgentOrigin(PartyBase.MainParty, @object, -1, default(UniqueTroopDescriptor), false, true), this._playerShip);
				}
				for (int j = 0; j < num3; j++)
				{
					this._navalAgentsLogic.AddReservedTroopToShip(new PartyAgentOrigin(PartyBase.MainParty, object2, -1, default(UniqueTroopDescriptor), false, true), this._playerShip);
				}
				this._navalAgentsLogic.SpawnNextBatch(0, false, null);
			}
		}

		// Token: 0x060002BC RID: 700 RVA: 0x000157AC File Offset: 0x000139AC
		private void OnSecondEnemyShipCleared()
		{
			object obj = new TextObject("{=R4Gqskgq}Victory", null);
			TextObject textObject = new TextObject("{=tEK1RK5N}Once again, you are victorious. Gunnar, meanwhile, inspects the fallen pirates, and soon finds one who is only lightly wounded and able to speak.", null);
			InformationManager.ShowInquiry(new InquiryData(obj.ToString(), textObject.ToString(), true, false, GameTexts.FindText("str_ok", null).ToString(), "", new Action(this.OnVictoryPopUpClosed), null, "", 0f, null, null, null), true, false);
			MBMusicManager.Current.ForceStopThemeWithFadeOut();
		}

		// Token: 0x060002BD RID: 701 RVA: 0x00015822 File Offset: 0x00013A22
		private void OnVictoryPopUpClosed()
		{
			this._isMissionSuccessful = true;
			PlayerEncounter.Battle.SetOverrideWinner(PlayerEncounter.Battle.PlayerSide);
			base.Mission.EndMission();
		}

		// Token: 0x060002BE RID: 702 RVA: 0x0001584A File Offset: 0x00013A4A
		private void OnPlayerTeamDefeated()
		{
			this._isMissionFailed = true;
			PlayerEncounter.Battle.SetOverrideWinner(PlayerEncounter.Battle.GetOtherSide(PlayerEncounter.Battle.PlayerSide));
			base.Mission.EndMission();
		}

		// Token: 0x060002BF RID: 703 RVA: 0x0001587C File Offset: 0x00013A7C
		public bool HaveAllyShipsBeenCutLoose()
		{
			return !this._playerShip.GetIsThereActiveBridgeTo(this._secondShip);
		}

		// Token: 0x060002C0 RID: 704 RVA: 0x00015894 File Offset: 0x00013A94
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

		// Token: 0x060002C1 RID: 705 RVA: 0x000158D5 File Offset: 0x00013AD5
		private void OnPlayerOrdered(OrderType orderType, MBReadOnlyList<Formation> appliedFormations, OrderController orderController, object[] delegateParams)
		{
			if (!this._hasShownChargeNotification && !this._isSecondPhaseSetup && (orderType == 4 || orderType == 5))
			{
				this.ShowChargeNotification();
			}
		}

		// Token: 0x060002C2 RID: 706 RVA: 0x000158F5 File Offset: 0x00013AF5
		private void ShowChargeNotification()
		{
			this._currentNotificationText = new TextObject("{=J0O71ubZ}The lines are holding! At them, lads!", null);
			this._hasShownChargeNotification = true;
		}

		// Token: 0x060002C3 RID: 707 RVA: 0x0001590F File Offset: 0x00013B0F
		private void ShowSecondPhaseChargeNotification()
		{
			this._currentNotificationText = new TextObject("{=8WDTkhc0}Strike hard, boys! Finish them!", null);
			this._hasShownSecondPhaseChargeNotification = true;
		}

		// Token: 0x0400019D RID: 413
		private const int InitialAllyMeleeTroopCount = 10;

		// Token: 0x0400019E RID: 414
		private const int InitialAllyRangedTroopCount = 10;

		// Token: 0x0400019F RID: 415
		private const int SecondPhaseMinTotalAllyTroopCount = 14;

		// Token: 0x040001A0 RID: 416
		private const int SecondPhasePrisonerMeleeTroopCount = 7;

		// Token: 0x040001A1 RID: 417
		private const int SecondPhasePrisonerRangedTroopCount = 7;

		// Token: 0x040001A2 RID: 418
		private const float AfterFightShipChangeDuration = 0.5f;

		// Token: 0x040001A3 RID: 419
		private const float BoardingImminentRadiusSqr = 2500f;

		// Token: 0x040001A4 RID: 420
		private const float NotificationRepeatDuration = 27f;

		// Token: 0x040001A5 RID: 421
		private const string AllyMeleeTroopStringId = "gangradirs_kin_melee";

		// Token: 0x040001A6 RID: 422
		private const string AllyRangedTroopStringId = "gangradirs_kin_ranged";

		// Token: 0x040001A7 RID: 423
		private const string EnemyTroopStringId = "sea_hounds_pups";

		// Token: 0x040001A8 RID: 424
		private const float MissionStateChangeTimer = 3f;

		// Token: 0x040001A9 RID: 425
		private const float WindStrength = 1.5f;

		// Token: 0x040001AA RID: 426
		private const float FadeDuration = 0.5f;

		// Token: 0x040001AB RID: 427
		private const float BlackScreenDuration = 0.75f;

		// Token: 0x040001AC RID: 428
		private static readonly Dictionary<string, string> PlayerShipUpgradePieces = new Dictionary<string, string> { { "sail", "sails_lvl2" } };

		// Token: 0x040001AD RID: 429
		private static readonly Dictionary<string, string> SecondShipUpgradePieces = new Dictionary<string, string> { { "sail", "sails_lvl2" } };

		// Token: 0x040001AE RID: 430
		private static readonly Dictionary<string, string> ReinforcementShipUpgradePieces = new Dictionary<string, string> { { "sail", "sails_lvl2" } };

		// Token: 0x040001AF RID: 431
		private bool _isMissionInitialized;

		// Token: 0x040001B0 RID: 432
		private List<GameEntity> _entities = new List<GameEntity>();

		// Token: 0x040001B1 RID: 433
		private Agent _gunnarAgent;

		// Token: 0x040001B2 RID: 434
		private MissionShip _playerShip;

		// Token: 0x040001B3 RID: 435
		private MissionShip _secondShip;

		// Token: 0x040001B4 RID: 436
		private MissionShip _reinforcementShip;

		// Token: 0x040001B5 RID: 437
		private readonly MobileParty _pirateParty;

		// Token: 0x040001B6 RID: 438
		private MissionTimer _victoryTimer;

		// Token: 0x040001B7 RID: 439
		private MissionTimer _defeatTimer;

		// Token: 0x040001B8 RID: 440
		private float _notificationTimer = 15f;

		// Token: 0x040001B9 RID: 441
		private TextObject _currentNotificationText;

		// Token: 0x040001BA RID: 442
		private bool _isInSecondPhase;

		// Token: 0x040001BB RID: 443
		private bool _isMissionSuccessful;

		// Token: 0x040001BC RID: 444
		private bool _isMissionFailed;

		// Token: 0x040001BD RID: 445
		private bool _hasShownChargeNotification;

		// Token: 0x040001BE RID: 446
		private bool _hasShownSecondPhaseChargeNotification;

		// Token: 0x040001BF RID: 447
		private bool _hasShownBoardImminentNotification;

		// Token: 0x040001C0 RID: 448
		private bool _hasIncreasedMusicIntensityForSecondPhase;

		// Token: 0x040001C1 RID: 449
		private NavalShipsLogic _navalShipsLogic;

		// Token: 0x040001C2 RID: 450
		private NavalAgentsLogic _navalAgentsLogic;

		// Token: 0x040001C3 RID: 451
		private MissionObjectiveLogic _missionObjectiveLogic;

		// Token: 0x040001C4 RID: 452
		private bool _isGunnarAfterFightFirstNotificationShown;

		// Token: 0x040001C5 RID: 453
		private bool _isGunnarAfterFightSecondNotificationShown;

		// Token: 0x040001C6 RID: 454
		private float _afterFightShipChangeTimer;

		// Token: 0x040001C7 RID: 455
		private bool _isShipTransferQueued;

		// Token: 0x040001C8 RID: 456
		private bool _isSecondShipSelected;

		// Token: 0x040001C9 RID: 457
		private readonly int _pirateTroopCount;

		// Token: 0x040001CA RID: 458
		private bool _isDialogueQueued;

		// Token: 0x040001CB RID: 459
		private bool _isSecondPhaseSetup;

		// Token: 0x040001CC RID: 460
		private float _dialogueTimer;
	}
}
