using System;
using System.Collections.Generic;
using System.Linq;
using NavalDLC.Missions;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using NavalDLC.Missions.Objects.UsableMachines;
using NavalDLC.Missions.ShipActuators;
using NavalDLC.Missions.ShipControl;
using NavalDLC.Missions.ShipInput;
using NavalDLC.Storyline.Objectives.Captivity;
using SandBox;
using SandBox.Missions.AgentBehaviors;
using SandBox.Missions.MissionLogics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.Extensions;
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
	// Token: 0x0200002F RID: 47
	public class NavalStorylineCaptivityMissionController : MissionLogic
	{
		// Token: 0x1700005D RID: 93
		// (get) Token: 0x06000259 RID: 601 RVA: 0x00010FF9 File Offset: 0x0000F1F9
		// (set) Token: 0x0600025A RID: 602 RVA: 0x00011001 File Offset: 0x0000F201
		public MissionShip MissionShip { get; private set; }

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x0600025B RID: 603 RVA: 0x0001100A File Offset: 0x0000F20A
		// (set) Token: 0x0600025C RID: 604 RVA: 0x00011012 File Offset: 0x0000F212
		public bool IsPlayerFree { get; private set; }

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x0600025D RID: 605 RVA: 0x0001101B File Offset: 0x0000F21B
		public bool HasTalkedToGunnar
		{
			get
			{
				return this._hasTalkedToGunnar;
			}
		}

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x0600025E RID: 606 RVA: 0x00011023 File Offset: 0x0000F223
		// (set) Token: 0x0600025F RID: 607 RVA: 0x0001102B File Offset: 0x0000F22B
		public bool WasPlayerKnockedOut { get; private set; }

		// Token: 0x06000260 RID: 608 RVA: 0x00011034 File Offset: 0x0000F234
		public NavalStorylineCaptivityMissionController(CharacterObject allyCharacter, BasicCharacterObject enemyCharacter, CharacterObject crewCharacter)
		{
			this._allyCharacterObject = allyCharacter;
			this._enemyCharacterObject = enemyCharacter;
			this._crewCharacterObject = crewCharacter;
		}

		// Token: 0x06000261 RID: 609 RVA: 0x000110B4 File Offset: 0x0000F2B4
		public override void OnBehaviorInitialize()
		{
			if (!SailWindProfile.IsSailWindProfileInitialized)
			{
				SailWindProfile.InitializeProfile();
			}
		}

		// Token: 0x06000262 RID: 610 RVA: 0x000110C2 File Offset: 0x0000F2C2
		public bool IsInitialized()
		{
			return this._missionInitializationPeriod > 1;
		}

		// Token: 0x06000263 RID: 611 RVA: 0x000110D0 File Offset: 0x0000F2D0
		public override void OnMissionTick(float dt)
		{
			if (this._missionInitializationPeriod == 0)
			{
				if (!SailWindProfile.IsSailWindProfileInitialized)
				{
					SailWindProfile.InitializeProfile();
				}
				this._missionInitializationPeriod++;
				this._missionObjectiveLogic = base.Mission.GetMissionBehavior<MissionObjectiveLogic>();
				this.UpdateEntityReferences();
				base.Mission.PlayerTeam.DisableDetachmentTicking();
				base.Mission.Scene.SetWaterStrength(0f);
				this.MissionShip = this.CreateShip();
				this.UpdateEntityReferences();
				this.CategorizeOars();
				this._entities.FirstOrDefault<GameEntity>((GameEntity t) => t.HasTag("spawn_highlight_1")).SetVisibilityExcludeParents(false);
				this._entities.FirstOrDefault<GameEntity>((GameEntity t) => t.HasTag("spawn_highlight_2")).SetVisibilityExcludeParents(false);
				this.MissionShip.SetController(ShipControllerType.AI, false);
				this.MissionShip.SetCustomSailSetting(true, SailInput.Raised);
				MatrixFrame globalFrame = this.MissionShip.GlobalFrame;
				ShipOrder shipOrder = this.MissionShip.ShipOrder;
				Vec2 vec = globalFrame.origin.AsVec2 + globalFrame.rotation.f.AsVec2 * 50f;
				shipOrder.SetShipMovementOrder(in vec);
				GameEntity gameEntity = this._entities.FirstOrDefault<GameEntity>((GameEntity t) => t.HasTag("sp_wind"));
				if (gameEntity != null)
				{
					Vec3 f = gameEntity.GetGlobalFrame().rotation.f;
					Vec2 vec2 = f.AsVec2.Normalized() * 1.1f;
					Mission.Current.Scene.SetGlobalWindStrengthVector(ref vec2);
				}
				base.Mission.Scene.SetWaterStrength(1f);
				Mission.Current.OnDeploymentFinished();
				MBMusicManager.Current.StartThemeWithConstantIntensity(8, false);
				MBMusicManager.Current.ChangeCurrentThemeIntensity(-1f);
				MBMusicManager.Current.ChangeCurrentThemeIntensity(0.5f);
			}
			else if (this._missionInitializationPeriod == 1)
			{
				this._missionInitializationPeriod++;
				this.SpawnPlayerAgent();
				this.SpawnAllyAgent();
				this.SpawnEnemyAgents();
				this.SpawnCrewAgents();
				this.SpawnWeapon();
				this.InitializeUsableMachines();
				this.SetOarForceMultipliers(0.01f);
				this.MissionShip.Formation.SetFacingOrder(FacingOrder.FacingOrderLookAtDirection((-this.MissionShip.GameEntity.GetGlobalFrame().rotation.f).AsVec2));
				TextObject textObject = new TextObject("{=lRLE9fpA}{PLAYER.NAME}! Your chain is loose. It's now or never! Get up and strike them down!", null);
				TextObjectExtensions.SetCharacterProperties(textObject, "PLAYER", Hero.MainHero.CharacterObject, false);
				CampaignInformationManager.AddDialogLine(textObject, this._allyCharacterObject, this._allyCharacterObject.FirstCivilianEquipment, 1000, 2);
				CaptivityEscapeCaptivityObjective captivityEscapeCaptivityObjective = new CaptivityEscapeCaptivityObjective(Mission.Current, this);
				this._missionObjectiveLogic.StartObjective(captivityEscapeCaptivityObjective);
			}
			this.CheckEnemyAlarmedState();
			this.CheckIfCrewmenAreNearby();
			if (this._isPlayerTinkeringWithTheBindsMachine)
			{
				this.CheckIfPlayerIsReleasedFromOar();
			}
			if (this._hasSavedOarsmen && !this._hasTalkedToGunnar)
			{
				this._speechDelayTimer += dt;
				if (!this._isConversationSetupInProgress && this._speechDelayTimer > 0.75f)
				{
					this.SetupPostFightConversation();
				}
				if (this._speechDelayTimer > 1.75f)
				{
					this.StartPostFightConversation();
					this.ReenableAllOars();
				}
			}
			if (this._allScatteredCrewMembersAreSaved && !this._hasTalkedToGunnarOutro)
			{
				this._outroSpeechDelayTimer += dt;
				if (!this._isConversationSetupInProgress && this._outroSpeechDelayTimer > 0.75f)
				{
					this.SetupSavedCrewConversation();
				}
				if (this._outroSpeechDelayTimer > 1.75f)
				{
					this.StartSavedCrewConversation();
				}
			}
			if (this.HasTalkedToGunnar)
			{
				if (this.MissionShip.ShipOrder.OarsmenLevel > 0)
				{
					foreach (Agent agent in this._crewAgents)
					{
						if (agent.IsActive())
						{
							this.MakeAgentUseAssignedOarMachine(agent);
						}
					}
					foreach (Agent agent2 in this._savedScatteredAgents)
					{
						if (agent2.IsActive() && agent2 != this._crewConversationAgent)
						{
							this.MakeAgentUseAssignedOarMachine(agent2);
						}
					}
					if (!this._allScatteredCrewMembersAreSaved && !Campaign.Current.ConversationManager.IsAgentInConversation(this._gunnarAgent))
					{
						this.MakeAgentUseAssignedOarMachine(this._gunnarAgent);
					}
				}
				int oarsmenLevel = this.MissionShip.ShipOrder.OarsmenLevel;
				if (this._previousOarsmenLevel != oarsmenLevel)
				{
					this.OnOarsmenLevelChanged(oarsmenLevel);
					this._previousOarsmenLevel = oarsmenLevel;
				}
			}
		}

		// Token: 0x06000264 RID: 612 RVA: 0x0001159C File Offset: 0x0000F79C
		private void MakeAgentUseAssignedOarMachine(Agent agent)
		{
			if (!agent.IsDetachedFromFormation)
			{
				ShipOarMachine oarMachineToUse;
				this._oarAssignments.TryGetValue(agent, out oarMachineToUse);
				if (oarMachineToUse == null)
				{
					oarMachineToUse = this.GetOarMachineToUse();
					if (oarMachineToUse != null)
					{
						this._oarAssignments.Add(agent, oarMachineToUse);
					}
				}
				if (!oarMachineToUse.IsDisabledForBattleSideAI(agent.Team.Side))
				{
					oarMachineToUse.AddAgentAtSlotIndex(agent, 0);
				}
			}
		}

		// Token: 0x06000265 RID: 613 RVA: 0x000115F8 File Offset: 0x0000F7F8
		private void CheckIfCrewmenAreNearby()
		{
			if (this._hasSavedOarsmen && !this._allScatteredCrewMembersAreSaved && this._scatteredCrew.Count > 0)
			{
				Vec3 origin = this.MissionShip.GlobalFrame.origin;
				for (int i = this._scatteredCrew.Count - 1; i >= 0; i--)
				{
					ValueTuple<Agent, bool> valueTuple = this._scatteredCrew[i];
					Agent item = valueTuple.Item1;
					if (this.MissionShip.GetIsAgentOnShip(item, false) && item.CurrentlyUsedGameObject == null)
					{
						this._scatteredCrew.RemoveAt(i);
						this._savedScatteredAgents.Add(item);
						if (this._savedScatteredAgents.Count == 2)
						{
							this.OnFirstHighlightClearedEvent();
							this.OnFirstHighlightCleared();
						}
						if (this._savedScatteredAgents.Count == this._saveTargetAgentCount)
						{
							this.OnAllCrewSaved();
						}
					}
					else if (!valueTuple.Item2 && origin.DistanceSquared(valueTuple.Item1.Position) <= 900f)
					{
						this._scatteredCrew[i] = new ValueTuple<Agent, bool>(valueTuple.Item1, true);
						valueTuple.Item1.ClearTargetFrame();
						valueTuple.Item1.Formation = base.Mission.PlayerTeam.GetFormation(0);
						this.MissionShip.SetShipClimbingOrderStandAloneTickingActive(true);
						NavalAgentsLogic missionBehavior = Mission.Current.GetMissionBehavior<NavalAgentsLogic>();
						missionBehavior.AddAgentToShip(valueTuple.Item1, this.MissionShip);
						missionBehavior.TransferAgentToShip(valueTuple.Item1, this.MissionShip);
						this.OnPlayerReachedFirstZone();
						if (item.Position.DistanceSquared(this._spawnZone1.GlobalPosition) > item.Position.DistanceSquared(this._spawnZone2.GlobalPosition))
						{
							if (this._spawnZone2HelpSoundEvent != null)
							{
								this._spawnZone2HelpSoundEvent.Stop();
								this._spawnZone2HelpSoundEvent = null;
							}
						}
						else if (this._spawnZone1HelpSoundEvent != null)
						{
							this._spawnZone1HelpSoundEvent.Stop();
							this._spawnZone1HelpSoundEvent = null;
						}
					}
				}
				if (!this._allScatteredCrewMembersAreSaved)
				{
					if (this._spawnZone1.GlobalPosition.DistanceSquared(origin) <= 900f)
					{
						this._entities.First<GameEntity>((GameEntity t) => t.HasTag("spawn_highlight_1")).SetVisibilityExcludeParents(false);
					}
					if (this._spawnZone2.GlobalPosition.DistanceSquared(origin) <= 900f)
					{
						this._entities.First<GameEntity>((GameEntity t) => t.HasTag("spawn_highlight_2")).SetVisibilityExcludeParents(false);
					}
				}
			}
		}

		// Token: 0x06000266 RID: 614 RVA: 0x00011899 File Offset: 0x0000FA99
		private void UpdateEntityReferences()
		{
			this._entities.Clear();
			base.Mission.Scene.GetEntities(ref this._entities);
		}

		// Token: 0x06000267 RID: 615 RVA: 0x000118BC File Offset: 0x0000FABC
		private void CheckEnemyAlarmedState()
		{
			foreach (Agent agent in base.Mission.PlayerEnemyTeam.ActiveAgents)
			{
				if (!agent.IsAlarmed())
				{
					foreach (Agent agent2 in base.Mission.PlayerTeam.ActiveAgents)
					{
						bool flag = agent.GetComponent<CampaignAgentComponent>().AgentNavigator.CanSeeAgent(agent2);
						float num = agent.Position.DistanceSquared(agent2.Position);
						if (num <= 5f || (num <= 10f && flag))
						{
							this.OnAgentEntersFight(agent, agent2);
						}
					}
				}
			}
		}

		// Token: 0x06000268 RID: 616 RVA: 0x000119B4 File Offset: 0x0000FBB4
		public override InquiryData OnEndMissionRequest(out bool canLeave)
		{
			canLeave = this._isFinalized;
			return null;
		}

		// Token: 0x06000269 RID: 617 RVA: 0x000119C0 File Offset: 0x0000FBC0
		private MissionShip CreateShip()
		{
			NavalShipsLogic missionBehavior = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			GameEntity gameEntity = this._entities.FirstOrDefault<GameEntity>((GameEntity t) => t.HasTag("spawnpoint_ship"));
			MatrixFrame globalFrame = gameEntity.GetGlobalFrame();
			float waterLevelAtPosition = Mission.Current.Scene.GetWaterLevelAtPosition(gameEntity.GlobalPosition.AsVec2, true, false);
			globalFrame.origin = new Vec3(gameEntity.GlobalPosition.x, gameEntity.GlobalPosition.y, waterLevelAtPosition, -1f);
			Team team = Mission.GetTeam(0);
			Formation formation = team.GetFormation(0);
			Ship ship = PartyBase.MainParty.Ships.FirstOrDefault<Ship>((Ship s) => s.ShipHull.StringId == "ship_knarr_storyline") ?? PartyBase.MainParty.Ships.First<Ship>();
			MissionShip missionShip = missionBehavior.SpawnShip(ship, in globalFrame, team, formation, false, 8, true);
			missionShip.ShipOrder.SetAIControllableWithoutTroops(true);
			missionShip.ShipOrder.SetOrderOarsmenLevel(2);
			missionShip.SetShipOrderActive(false);
			return missionShip;
		}

		// Token: 0x0600026A RID: 618 RVA: 0x00011AD4 File Offset: 0x0000FCD4
		private void SpawnPlayerAgent()
		{
			ShipOarMachine firstScriptOfType = this._entities.FirstOrDefault<GameEntity>((GameEntity t) => t.HasTag("target_player")).Parent.GetFirstScriptOfType<ShipOarMachine>();
			firstScriptOfType.PilotStandingPoint.AddComponent(new ResetAnimationOnStopUsageComponent(ActionIndexCache.act_none, true));
			this._oarUsedByPlayer = firstScriptOfType;
			WeakGameEntity gameEntity = this._oarUsedByPlayer.PilotStandingPoint.GameEntity;
			Formation formation = base.Mission.PlayerTeam.GetFormation(0);
			MBEquipmentRoster @object = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>("item_set_player_captivity");
			AgentBuildData agentBuildData = new AgentBuildData(Hero.MainHero.CharacterObject).TroopOrigin(new PartyAgentOrigin(PartyBase.MainParty, Hero.MainHero.CharacterObject, -1, default(UniqueTroopDescriptor), false, false)).Team(base.Mission.PlayerTeam);
			Vec3 globalPosition = gameEntity.GlobalPosition;
			AgentBuildData agentBuildData2 = agentBuildData.InitialPosition(ref globalPosition);
			Vec2 asVec = gameEntity.GetGlobalFrame().rotation.f.AsVec2;
			AgentBuildData agentBuildData3 = agentBuildData2.InitialDirection(ref asVec).NoHorses(true).NoWeapons(true)
				.Formation(formation)
				.Equipment(@object.DefaultEquipment);
			Agent agent = Mission.Current.SpawnAgent(agentBuildData3, false);
			agent.Controller = 2;
			agent.UseGameObject(this._oarUsedByPlayer.PilotStandingPoint, -1);
			this._oarUsedByPlayer.OnPilotAssignedDuringSpawn();
		}

		// Token: 0x0600026B RID: 619 RVA: 0x00011C34 File Offset: 0x0000FE34
		private void SpawnAllyAgent()
		{
			GameEntity gameEntity = this._entities.FirstOrDefault<GameEntity>((GameEntity t) => t.HasTag("target_ally"));
			this._oarUsedByAlly = gameEntity.Parent.GetFirstScriptOfType<ShipOarMachine>();
			AgentBindsMachine firstScriptOfType = this._entities.FirstOrDefault<GameEntity>((GameEntity t) => t.HasTag("agentbind_ally")).GetFirstScriptOfType<AgentBindsMachine>();
			firstScriptOfType.SetOarMachine(this._oarUsedByAlly);
			this._agentBindMachines.Add(firstScriptOfType);
			WeakGameEntity gameEntity2 = this._oarUsedByAlly.PilotStandingPoint.GameEntity;
			MBEquipmentRoster @object = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>("item_set_gangradir_captivity");
			Formation formation = base.Mission.PlayerTeam.GetFormation(0);
			AgentBuildData agentBuildData = new AgentBuildData(this._allyCharacterObject).TroopOrigin(new PartyAgentOrigin(PartyBase.MainParty, this._allyCharacterObject, -1, default(UniqueTroopDescriptor), false, false)).Team(base.Mission.PlayerTeam);
			Vec3 globalPosition = gameEntity2.GlobalPosition;
			AgentBuildData agentBuildData2 = agentBuildData.InitialPosition(ref globalPosition);
			Vec2 asVec = gameEntity2.GetGlobalFrame().rotation.f.AsVec2;
			AgentBuildData agentBuildData3 = agentBuildData2.InitialDirection(ref asVec).NoHorses(true).NoWeapons(false)
				.Equipment(@object.DefaultEquipment)
				.Formation(formation);
			Agent agent = Mission.Current.SpawnAgent(agentBuildData3, false);
			this._gunnarAgent = agent;
			this.OnAgentAssignedToOarOnSpawn(agent, this._oarUsedByAlly);
		}

		// Token: 0x0600026C RID: 620 RVA: 0x00011DB4 File Offset: 0x0000FFB4
		private Agent SpawnAllyCrewAgent(Vec3 globalPosition, Vec2 globalDirection)
		{
			AgentBuildData agentBuildData = new AgentBuildData(this._crewCharacterObject).TroopOrigin(new SimpleAgentOrigin(this._crewCharacterObject, -1, null, default(UniqueTroopDescriptor))).Team(base.Mission.PlayerTeam).InitialPosition(ref globalPosition)
				.InitialDirection(ref globalDirection)
				.NoHorses(true)
				.NoWeapons(false);
			Agent agent = Mission.Current.SpawnAgent(agentBuildData, false);
			agent.GetComponent<AgentNavalComponent>().SetCanDrown(false);
			agent.SetTargetPosition(agent.Position.AsVec2);
			VisualTrackerMissionBehavior missionBehavior = Mission.Current.GetMissionBehavior<VisualTrackerMissionBehavior>();
			if (missionBehavior != null)
			{
				missionBehavior.RegisterLocalOnlyObject(agent);
			}
			return agent;
		}

		// Token: 0x0600026D RID: 621 RVA: 0x00011E58 File Offset: 0x00010058
		private void SpawnEnemyAgents()
		{
			foreach (GameEntity gameEntity in this._entities.Where<GameEntity>((GameEntity t) => t.HasTag("spawnpoint_guard")).ToList<GameEntity>())
			{
				AgentBuildData agentBuildData = new AgentBuildData(this._enemyCharacterObject).TroopOrigin(new SimpleAgentOrigin(this._enemyCharacterObject, -1, null, default(UniqueTroopDescriptor))).Team(base.Mission.PlayerEnemyTeam);
				Vec3 globalPosition = gameEntity.GlobalPosition;
				AgentBuildData agentBuildData2 = agentBuildData.InitialPosition(ref globalPosition);
				Vec2 vec = gameEntity.GetGlobalFrame().rotation.f.AsVec2;
				vec = vec.Normalized();
				AgentBuildData agentBuildData3 = agentBuildData2.InitialDirection(ref vec).NoHorses(true).NoWeapons(false);
				Agent agent = Mission.Current.SpawnAgent(agentBuildData3, false);
				CampaignAgentComponent component = agent.GetComponent<CampaignAgentComponent>();
				if (component.AgentNavigator == null)
				{
					component.CreateAgentNavigator();
				}
				string text = "act_drunk_trio_right";
				if (gameEntity.HasTag("guard_1"))
				{
					text = "act_drunk_trio_middle";
				}
				else if (gameEntity.HasTag("guard_2"))
				{
					text = "act_drunk_trio_left";
				}
				else if (gameEntity.HasTag("guard_3"))
				{
					text = "act_drunk_trio_right";
				}
				MBActionSet actionSet = MBGlobals.GetActionSet("as_human_hideout_bandit");
				AnimationSystemData animationSystemData = MonsterExtensions.FillAnimationSystemData(agentBuildData3.AgentMonster, actionSet, NavalStorylineData.Gunnar.CharacterObject.GetStepSize(), false);
				agent.SetActionSet(ref animationSystemData);
				int num = 0;
				ActionIndexCache actionIndexCache = ActionIndexCache.Create(text);
				agent.SetActionChannel(num, ref actionIndexCache, false, 0L, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
			}
		}

		// Token: 0x0600026E RID: 622 RVA: 0x00012030 File Offset: 0x00010230
		private void SpawnCrewAgents()
		{
			GameEntity gameEntity = this._entities.First<GameEntity>((GameEntity t) => t.HasTag("spawnpoint_neutral_npc_1"));
			ShipOarMachine firstScriptOfType = gameEntity.Parent.GetFirstScriptOfType<ShipOarMachine>();
			AgentBindsMachine firstScriptOfType2 = this._entities.First<GameEntity>((GameEntity t) => t.HasTag("agentbind_neutral_1")).GetFirstScriptOfType<AgentBindsMachine>();
			firstScriptOfType2.SetOarMachine(firstScriptOfType);
			this._agentBindMachines.Add(firstScriptOfType2);
			GameEntity gameEntity2 = this._entities.First<GameEntity>((GameEntity t) => t.HasTag("spawnpoint_neutral_npc_2"));
			ShipOarMachine firstScriptOfType3 = gameEntity2.Parent.GetFirstScriptOfType<ShipOarMachine>();
			AgentBindsMachine firstScriptOfType4 = this._entities.First<GameEntity>((GameEntity t) => t.HasTag("agentbind_neutral_2")).GetFirstScriptOfType<AgentBindsMachine>();
			firstScriptOfType4.SetOarMachine(firstScriptOfType3);
			this._agentBindMachines.Add(firstScriptOfType4);
			GameEntity gameEntity3 = this._entities.First<GameEntity>((GameEntity t) => t.HasTag("spawnpoint_neutral_npc_3"));
			ShipOarMachine firstScriptOfType5 = gameEntity3.Parent.GetFirstScriptOfType<ShipOarMachine>();
			AgentBindsMachine firstScriptOfType6 = this._entities.First<GameEntity>((GameEntity t) => t.HasTag("agentbind_neutral_3")).GetFirstScriptOfType<AgentBindsMachine>();
			firstScriptOfType6.SetOarMachine(firstScriptOfType5);
			this._agentBindMachines.Add(firstScriptOfType6);
			foreach (GameEntity gameEntity4 in new GameEntity[] { gameEntity, gameEntity2, gameEntity3 })
			{
				PartyBase.MainParty.AddMember(this._crewCharacterObject, 1, 0);
				Formation formation = base.Mission.PlayerTeam.GetFormation(0);
				AgentBuildData agentBuildData = new AgentBuildData(this._crewCharacterObject).TroopOrigin(new PartyAgentOrigin(PartyBase.MainParty, this._crewCharacterObject, -1, default(UniqueTroopDescriptor), false, false)).Team(base.Mission.PlayerTeam);
				Vec3 globalPosition = gameEntity4.GlobalPosition;
				AgentBuildData agentBuildData2 = agentBuildData.InitialPosition(ref globalPosition);
				Vec2 vec = gameEntity4.GetGlobalFrame().rotation.f.AsVec2;
				vec = vec.Normalized();
				AgentBuildData agentBuildData3 = agentBuildData2.InitialDirection(ref vec).NoHorses(true).NoWeapons(false)
					.Formation(formation);
				Agent agent = Mission.Current.SpawnAgent(agentBuildData3, false);
				this._crewAgents.Add(agent);
				ShipOarMachine firstScriptOfType7 = gameEntity4.Parent.GetFirstScriptOfType<ShipOarMachine>();
				this.OnAgentAssignedToOarOnSpawn(agent, firstScriptOfType7);
				this._spawnedOarsmenCount++;
			}
		}

		// Token: 0x0600026F RID: 623 RVA: 0x000122E2 File Offset: 0x000104E2
		private void OnAgentAssignedToOarOnSpawn(Agent agent, ShipOarMachine oarMachine)
		{
			Formation formation = agent.Formation;
			if (formation != null)
			{
				formation.DetachUnit(agent, false);
			}
			agent.Detachment = oarMachine;
			agent.UseGameObject(oarMachine.PilotStandingPoint, -1);
			this._oarAssignments.Add(agent, oarMachine);
			oarMachine.OnPilotAssignedDuringSpawn();
		}

		// Token: 0x06000270 RID: 624 RVA: 0x00012320 File Offset: 0x00010520
		private void SpawnWeapon()
		{
			GameEntity gameEntity = this._entities.FirstOrDefault<GameEntity>((GameEntity t) => t.HasTag("pickup_weapon"));
			ItemObject @object = MBObjectManager.Instance.GetObject<ItemObject>("shackle");
			MissionWeapon missionWeapon;
			missionWeapon..ctor(@object, null, null);
			this._weaponEntity = Mission.Current.SpawnWeaponWithNewEntity(ref missionWeapon, 8, gameEntity.GetGlobalFrame()).GetFirstScriptOfType<SpawnedItemEntity>();
		}

		// Token: 0x06000271 RID: 625 RVA: 0x00012390 File Offset: 0x00010590
		public override void OnObjectUsed(Agent userAgent, UsableMissionObject usedObject)
		{
			if (userAgent.IsPlayerControlled)
			{
				this.OnMarkedObjectStatusChangedEvent();
			}
		}

		// Token: 0x06000272 RID: 626 RVA: 0x000123A8 File Offset: 0x000105A8
		public override void OnObjectStoppedBeingUsed(Agent userAgent, UsableMissionObject usedObject)
		{
			if (this._isFinalized)
			{
				return;
			}
			if (userAgent.IsPlayerControlled && usedObject == this._oarUsedByPlayer.PilotStandingPoint)
			{
				this.OnPlayerStartedEscape();
			}
			else if (userAgent == this._gunnarAgent || userAgent.Character == this._crewCharacterObject)
			{
				if (!this.HasTalkedToGunnar)
				{
					this._savedOarsmenCount++;
					AgentBindsMachine agentBindsMachine = this._agentBindMachines.FirstOrDefault<AgentBindsMachine>((AgentBindsMachine t) => t.ShipOarMachine.PilotStandingPoint == usedObject);
					if (agentBindsMachine != null)
					{
						agentBindsMachine.PilotStandingPoint.IsDisabledForPlayers = true;
					}
					if (!this._hasSavedOarsmen && this._savedOarsmenCount >= this._spawnedOarsmenCount + 1)
					{
						this._hasSavedOarsmen = true;
						this.OnStartFadeOutEvent(0.75f, 1f, 0.75f);
					}
				}
				UsableMissionObject usedObject2 = usedObject;
				if (usedObject2 != null && usedObject2.GameEntity.Parent.HasScriptOfType<ShipOarMachine>())
				{
					Vec3 origin = usedObject.GameEntity.GetGlobalFrame().origin;
					WorldPosition worldPosition;
					worldPosition..ctor(base.Mission.Scene, origin);
					userAgent.SetScriptedPosition(ref worldPosition, true, 0);
				}
				else
				{
					WorldPosition worldPosition2 = userAgent.GetWorldPosition();
					userAgent.SetScriptedPosition(ref worldPosition2, true, 0);
				}
			}
			this.OnMarkedObjectStatusChangedEvent();
		}

		// Token: 0x06000273 RID: 627 RVA: 0x000124FC File Offset: 0x000106FC
		private void HandleChainVisualsAfterDialogue()
		{
			foreach (AgentBindsMachine agentBindsMachine in this._agentBindMachines)
			{
				agentBindsMachine.Deactivate();
				agentBindsMachine.GameEntity.SetVisibilityExcludeParents(false);
			}
			GameEntity gameEntity = this._entities.FirstOrDefault<GameEntity>((GameEntity t) => t.HasTag("agentbind_ally_broken"));
			if (gameEntity != null)
			{
				gameEntity.SetVisibilityExcludeParents(true);
			}
			GameEntity gameEntity2 = this._entities.FirstOrDefault<GameEntity>((GameEntity t) => t.HasTag("agentbind_neutral_1_broken"));
			if (gameEntity2 != null)
			{
				gameEntity2.SetVisibilityExcludeParents(true);
			}
			GameEntity gameEntity3 = this._entities.FirstOrDefault<GameEntity>((GameEntity t) => t.HasTag("agentbind_neutral_2_broken"));
			if (gameEntity3 != null)
			{
				gameEntity3.SetVisibilityExcludeParents(true);
			}
			GameEntity gameEntity4 = this._entities.FirstOrDefault<GameEntity>((GameEntity t) => t.HasTag("agentbind_neutral_3_broken"));
			if (gameEntity4 == null)
			{
				return;
			}
			gameEntity4.SetVisibilityExcludeParents(true);
		}

		// Token: 0x06000274 RID: 628 RVA: 0x00012634 File Offset: 0x00010834
		private void OnPlayerStartedEscape()
		{
			this.OnPlayerStartedEscapeEvent();
			this._tinkeringAction = ActionIndexCache.Create("act_cutscene_break_chains_1");
			Agent.Main.SetActionChannel(0, ref this._tinkeringAction, false, 0L, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
			this._isPlayerTinkeringWithTheBindsMachine = true;
		}

		// Token: 0x06000275 RID: 629 RVA: 0x0001269C File Offset: 0x0001089C
		private void CheckIfPlayerIsReleasedFromOar()
		{
			if (Agent.Main.GetCurrentAction(0) == this._tinkeringAction && Agent.Main.GetCurrentActionProgress(0) > 0.95f)
			{
				Agent.Main.ClearHandInverseKinematics();
				CampaignInformationManager.AddDialogLine(new TextObject("{=g1PnXEDa}{PLAYER.NAME}! It's now or never! Go, cut those bastards down!", null), this._allyCharacterObject, this._allyCharacterObject.FirstCivilianEquipment, 1000, 2);
				bool flag;
				Agent.Main.OnItemPickup(this._weaponEntity, 0, ref flag);
				this._isPlayerTinkeringWithTheBindsMachine = false;
				this.IsPlayerFree = true;
				GameEntity gameEntity = this._entities.FirstOrDefault<GameEntity>((GameEntity t) => t.HasTag("player_shackle"));
				if (gameEntity != null)
				{
					gameEntity.SetVisibilityExcludeParents(false);
				}
				this._oarUsedByPlayer.PilotStandingPoint.IsDisabledForPlayers = true;
				CaptivityDefeatCaptorsObjective captivityDefeatCaptorsObjective = new CaptivityDefeatCaptorsObjective(Mission.Current, this);
				this._missionObjectiveLogic.StartObjective(captivityDefeatCaptorsObjective);
				this.MissionShip.ShipOrder.SetShipStopOrder();
			}
		}

		// Token: 0x06000276 RID: 630 RVA: 0x000127A0 File Offset: 0x000109A0
		private void TriggerEnemies()
		{
			foreach (Agent agent in base.Mission.PlayerEnemyTeam.ActiveAgents)
			{
				if (agent.IsAIControlled && !agent.IsUsingGameObject && !agent.IsAlarmed())
				{
					this.OnAgentEntersFight(agent, null);
				}
			}
		}

		// Token: 0x06000277 RID: 631 RVA: 0x00012818 File Offset: 0x00010A18
		private void OnAgentEntersFight(Agent agent, Agent targetAgent = null)
		{
			AgentFlag agentFlags = agent.GetAgentFlags();
			agent.SetAgentFlags(agentFlags | 65536);
			agent.SetActionChannel(0, ref ActionIndexCache.act_none, true, 0L, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
			CampaignAgentComponent component = agent.GetComponent<CampaignAgentComponent>();
			AgentNavigator agentNavigator = component.AgentNavigator ?? component.CreateAgentNavigator();
			AlarmedBehaviorGroup alarmedBehaviorGroup = agentNavigator.GetBehaviorGroup<AlarmedBehaviorGroup>();
			if (alarmedBehaviorGroup == null)
			{
				alarmedBehaviorGroup = agentNavigator.AddBehaviorGroup<AlarmedBehaviorGroup>();
				alarmedBehaviorGroup.AddBehavior<FightBehavior>();
			}
			alarmedBehaviorGroup.SetScriptedBehavior<FightBehavior>();
			agent.SetAutomaticTargetSelection(false);
			if (targetAgent == null)
			{
				targetAgent = Agent.Main;
			}
			if (targetAgent != null)
			{
				agent.SetTargetAgent(targetAgent);
				AlarmedBehaviorGroup.AlarmAgent(agent);
			}
		}

		// Token: 0x06000278 RID: 632 RVA: 0x000128C4 File Offset: 0x00010AC4
		public override void OnEarlyAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			if (this._scatteredCrew != null && this._scatteredCrew.Any<ValueTuple<Agent, bool>>((ValueTuple<Agent, bool> x) => x.Item1 == affectedAgent))
			{
				Debug.FailedAssert("Should crew to save agent be removed", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\Storyline\\MissionControllers\\NavalStorylineCaptivityMissionController.cs", "OnEarlyAgentRemoved", 796);
			}
			if (affectedAgent.Team == base.Mission.PlayerEnemyTeam)
			{
				this.TriggerEnemies();
				if (Extensions.IsEmpty<Agent>(base.Mission.PlayerEnemyTeam.ActiveAgents))
				{
					CampaignInformationManager.AddDialogLine(new TextObject("{=bu8MRgpS}Well done! Now, help us get these chains off.", null), this._allyCharacterObject, this._allyCharacterObject.FirstCivilianEquipment, 1000, 2);
					foreach (AgentBindsMachine agentBindsMachine in this._agentBindMachines)
					{
						agentBindsMachine.PilotStandingPoint.IsDisabledForPlayers = false;
					}
					this.OnMarkedObjectStatusChangedEvent();
					CaptivityFreePrisonersObjective captivityFreePrisonersObjective = new CaptivityFreePrisonersObjective(Mission.Current, this);
					this._missionObjectiveLogic.StartObjective(captivityFreePrisonersObjective);
					foreach (Agent agent in this._crewAgents)
					{
						SkinVoiceManager.CombatVoiceNetworkPredictionType combatVoiceNetworkPredictionType = 2;
						agent.MakeVoice(SkinVoiceManager.VoiceType.Victory, combatVoiceNetworkPredictionType);
					}
				}
			}
		}

		// Token: 0x06000279 RID: 633 RVA: 0x00012A30 File Offset: 0x00010C30
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			if (affectedAgent == Agent.Main)
			{
				this.FinalizeMission();
			}
		}

		// Token: 0x0600027A RID: 634 RVA: 0x00012A40 File Offset: 0x00010C40
		private void SpawnScatteredCrew()
		{
			this._saveTargetAgentCount = 0;
			this._spawnZone1 = this._entities.FirstOrDefault<GameEntity>((GameEntity t) => t.HasTag("crewmen_spawn_zone_alt_1"));
			this._spawnZone2 = this._entities.FirstOrDefault<GameEntity>((GameEntity t) => t.HasTag("crewmen_spawn_zone_alt_2"));
			this._entities.FirstOrDefault<GameEntity>((GameEntity t) => t.HasTag("spawn_highlight_1")).SetVisibilityExcludeParents(true);
			this._entities.FirstOrDefault<GameEntity>((GameEntity t) => t.HasTag("spawn_highlight_2")).SetVisibilityExcludeParents(true);
			Vec3 globalPosition = this._spawnZone1.GlobalPosition;
			MatrixFrame matrixFrame = this._spawnZone1.GetGlobalFrame();
			this.SpawnCrewAroundPosition(globalPosition, matrixFrame.rotation.f.AsVec2.Normalized());
			Vec3 globalPosition2 = this._spawnZone2.GlobalPosition;
			matrixFrame = this._spawnZone2.GetGlobalFrame();
			this.SpawnCrewAroundPosition(globalPosition2, matrixFrame.rotation.f.AsVec2.Normalized());
			int eventGlobalIndex = SoundManager.GetEventGlobalIndex("event:/mission/ambient/special/storyline/drowning_save_us");
			this._spawnZone1HelpSoundEvent = SoundEvent.CreateEvent(eventGlobalIndex, Mission.Current.Scene);
			this._spawnZone1HelpSoundEvent.SetPosition(this._spawnZone1.GlobalPosition);
			this._spawnZone1HelpSoundEvent.Play();
			this._spawnZone2HelpSoundEvent = SoundEvent.CreateEvent(eventGlobalIndex, Mission.Current.Scene);
			this._spawnZone2HelpSoundEvent.SetPosition(this._spawnZone2.GlobalPosition);
			this._spawnZone2HelpSoundEvent.Play();
			CaptivitySaveTheCrewmenObjective captivitySaveTheCrewmenObjective = new CaptivitySaveTheCrewmenObjective(Mission.Current, this);
			this._missionObjectiveLogic.StartObjective(captivitySaveTheCrewmenObjective);
		}

		// Token: 0x0600027B RID: 635 RVA: 0x00012C14 File Offset: 0x00010E14
		private void SpawnCrewAroundPosition(Vec3 spawnGlobalPosition, Vec2 spawnGlobalDirection)
		{
			spawnGlobalPosition.z = base.Mission.Scene.GetWaterLevelAtPosition(spawnGlobalPosition.AsVec2, false, false) - 3f;
			for (int i = 0; i < 2; i++)
			{
				Agent agent = this.SpawnAllyCrewAgent(spawnGlobalPosition + new Vec3(MBRandom.RandomFloatRanged(1f, 4f), MBRandom.RandomFloatRanged(1f, 4f), 0f, -1f), spawnGlobalDirection);
				this._scatteredCrew.Add(new ValueTuple<Agent, bool>(agent, false));
				this._saveTargetAgentCount++;
			}
		}

		// Token: 0x0600027C RID: 636 RVA: 0x00012CB0 File Offset: 0x00010EB0
		private void SetupPostFightConversation()
		{
			this._isConversationSetupInProgress = true;
			this.MissionShip.SetAnchor(true, true, 1f);
			this.MissionShip.ShipOrder.SetOrderOarsmenLevel(0);
			GameEntity gameEntity = this._entities.First<GameEntity>((GameEntity t) => t.HasTag("conversation_player"));
			GameEntity gameEntity2 = this._entities.First<GameEntity>((GameEntity t) => t.HasTag("conversation_ally"));
			if (Agent.Main == null || !Agent.Main.IsActive())
			{
				this.RespawnMainAgent(gameEntity);
			}
			Agent.Main.AgentVisuals.SetVisible(false);
			for (int i = base.Mission.PlayerEnemyTeam.ActiveAgents.Count - 1; i >= 0; i--)
			{
				base.Mission.PlayerEnemyTeam.ActiveAgents[i].FadeOut(true, true);
			}
			foreach (Agent agent in base.Mission.PlayerTeam.ActiveAgents)
			{
				if (!agent.IsPlayerControlled && agent != this._gunnarAgent)
				{
					agent.AgentVisuals.SetVisible(false);
				}
			}
			if (this._gunnarAgent.IsUsingGameObject)
			{
				this._gunnarAgent.StopUsingGameObject(true, 1);
				this._gunnarAgent.SetActionChannel(0, ref ActionIndexCache.act_none, true, 0L, 0f, 1f, 0f, 0.4f, 0f, false, -0.2f, 0, true);
				this._gunnarAgent.SetActionChannel(1, ref ActionIndexCache.act_none, true, 0L, 0f, 1f, 0f, 0.4f, 0f, false, -0.2f, 0, true);
			}
			this._gunnarAgent.TeleportToPosition(gameEntity2.GlobalPosition);
			Agent.Main.AgentVisuals.SetVisible(true);
			Agent.Main.TeleportToPosition(gameEntity.GlobalPosition);
			Vec3 vec = gameEntity2.GlobalPosition - Agent.Main.Position;
			this.OnConversationSetupEvent(vec);
			WorldPosition worldPosition;
			worldPosition..ctor(Mission.Current.Scene, gameEntity2.GlobalPosition);
			Agent gunnarAgent = this._gunnarAgent;
			Vec2 vec2 = vec.AsVec2;
			gunnarAgent.SetScriptedPositionAndDirection(ref worldPosition, -vec2.RotationInRadians, false, 0);
			Agent gunnarAgent2 = this._gunnarAgent;
			vec2 = vec.AsVec2;
			vec2 = -vec2.Normalized();
			gunnarAgent2.SetMovementDirection(ref vec2);
			this._gunnarAgent.Controller = 0;
		}

		// Token: 0x0600027D RID: 637 RVA: 0x00012F58 File Offset: 0x00011158
		private void StartPostFightConversation()
		{
			this._hasTalkedToGunnar = true;
			this._isConversationSetupInProgress = false;
			Campaign.Current.ConversationManager.SetupAndStartMissionConversation(this._gunnarAgent, base.Mission.MainAgent, true);
			foreach (AgentBindsMachine agentBindsMachine in this._agentBindMachines)
			{
				agentBindsMachine.Deactivate();
				agentBindsMachine.GameEntity.SetVisibilityExcludeParents(false);
			}
			this.SetOarForceMultipliers(0.95f);
			foreach (GameEntity gameEntity in this._entities.Where<GameEntity>((GameEntity t) => t.HasScriptOfType<ShipControllerMachine>()))
			{
				ShipControllerMachine firstScriptOfType = gameEntity.GetFirstScriptOfType<ShipControllerMachine>();
				if (firstScriptOfType != null)
				{
					firstScriptOfType.PilotStandingPoint.IsDisabledForPlayers = false;
				}
			}
			this.OnMarkedObjectStatusChangedEvent();
			Mission.Current.SetMissionMode(1, true);
		}

		// Token: 0x0600027E RID: 638 RVA: 0x00013078 File Offset: 0x00011278
		private void SetOarForceMultipliers(float forceMultiplier)
		{
			this.MissionShip.SetOarAppliedForceMultiplierForStoryMission(forceMultiplier);
		}

		// Token: 0x0600027F RID: 639 RVA: 0x00013088 File Offset: 0x00011288
		private void CategorizeOars()
		{
			foreach (GameEntity gameEntity in this._entities.FirstOrDefault<GameEntity>((GameEntity t) => t.HasTag("left_oar")).GetChildren())
			{
				IEnumerable<ShipOarMachine> scriptComponents = gameEntity.GetScriptComponents<ShipOarMachine>();
				if (!Extensions.IsEmpty<ShipOarMachine>(scriptComponents))
				{
					ShipOarMachine shipOarMachine = scriptComponents.FirstOrDefault<ShipOarMachine>();
					this._leftOars.Add(shipOarMachine);
				}
			}
			foreach (GameEntity gameEntity2 in this._entities.FirstOrDefault<GameEntity>((GameEntity t) => t.HasTag("right_oar")).GetChildren())
			{
				IEnumerable<ShipOarMachine> scriptComponents2 = gameEntity2.GetScriptComponents<ShipOarMachine>();
				if (!Extensions.IsEmpty<ShipOarMachine>(scriptComponents2))
				{
					ShipOarMachine shipOarMachine2 = scriptComponents2.FirstOrDefault<ShipOarMachine>();
					this._rightOars.Add(shipOarMachine2);
				}
			}
		}

		// Token: 0x06000280 RID: 640 RVA: 0x00013198 File Offset: 0x00011398
		private ShipOarMachine GetOarMachineToUse()
		{
			IEnumerable<ShipOarMachine> enumerable = this._leftOars.Where<ShipOarMachine>((ShipOarMachine t) => !t.PilotStandingPoint.HasUser && !t.PilotStandingPoint.HasAIMovingTo && this._oarAssignments.All<KeyValuePair<Agent, ShipOarMachine>>((KeyValuePair<Agent, ShipOarMachine> k) => k.Value != t));
			IEnumerable<ShipOarMachine> enumerable2 = this._rightOars.Where<ShipOarMachine>((ShipOarMachine t) => !t.PilotStandingPoint.HasUser && !t.PilotStandingPoint.HasAIMovingTo && this._oarAssignments.All<KeyValuePair<Agent, ShipOarMachine>>((KeyValuePair<Agent, ShipOarMachine> k) => k.Value != t));
			if (enumerable.Count<ShipOarMachine>() <= enumerable2.Count<ShipOarMachine>())
			{
				return enumerable2.FirstOrDefault<ShipOarMachine>();
			}
			return enumerable.FirstOrDefault<ShipOarMachine>();
		}

		// Token: 0x06000281 RID: 641 RVA: 0x000131F0 File Offset: 0x000113F0
		private void RespawnMainAgent(GameEntity respawnPositionEntity)
		{
			this.WasPlayerKnockedOut = true;
			AgentBuildData agentBuildData = new AgentBuildData(Hero.MainHero.CharacterObject).TroopOrigin(new PartyAgentOrigin(PartyBase.MainParty, Hero.MainHero.CharacterObject, -1, default(UniqueTroopDescriptor), false, false)).Team(base.Mission.PlayerTeam);
			Vec3 globalPosition = respawnPositionEntity.GlobalPosition;
			AgentBuildData agentBuildData2 = agentBuildData.InitialPosition(ref globalPosition);
			Vec2 vec = respawnPositionEntity.GetGlobalFrame().rotation.f.AsVec2;
			vec = vec.Normalized();
			AgentBuildData agentBuildData3 = agentBuildData2.InitialDirection(ref vec).NoHorses(true).NoWeapons(false);
			Mission.Current.SpawnAgent(agentBuildData3, false).Controller = 2;
		}

		// Token: 0x06000282 RID: 642 RVA: 0x000132A0 File Offset: 0x000114A0
		private void ReenableAllOars()
		{
			IEnumerable<GameEntity> enumerable = this._entities.Where<GameEntity>((GameEntity t) => t.HasScriptOfType<UsableMachine>());
			Formation formation = base.Mission.PlayerTeam.GetFormation(0);
			foreach (GameEntity gameEntity in enumerable)
			{
				UsableMachine firstScriptOfType = gameEntity.GetFirstScriptOfType<UsableMachine>();
				if (firstScriptOfType is ShipOarMachine && !formation.Detachments.Contains(firstScriptOfType))
				{
					ModuleExtensions.StartUsingMachine(formation, firstScriptOfType, false);
				}
			}
		}

		// Token: 0x06000283 RID: 643 RVA: 0x00013340 File Offset: 0x00011540
		private void InitializeUsableMachines()
		{
			foreach (GameEntity gameEntity in this._entities.Where<GameEntity>((GameEntity t) => t.HasScriptOfType<UsableMachine>()))
			{
				UsableMachine firstScriptOfType = gameEntity.GetFirstScriptOfType<UsableMachine>();
				if (firstScriptOfType is ShipOarMachine)
				{
					firstScriptOfType.SetEnemyRangeToStopUsing(-1f);
					if (firstScriptOfType != this._oarUsedByPlayer)
					{
						firstScriptOfType.PilotStandingPoint.IsDisabledForPlayers = true;
					}
				}
				if (firstScriptOfType is ShipControllerMachine)
				{
					firstScriptOfType.PilotStandingPoint.IsDisabledForPlayers = true;
				}
			}
			foreach (AgentBindsMachine agentBindsMachine in this._agentBindMachines)
			{
				agentBindsMachine.PilotStandingPoint.IsDisabledForPlayers = true;
			}
		}

		// Token: 0x06000284 RID: 644 RVA: 0x00013430 File Offset: 0x00011630
		public override void OnAgentAlarmedStateChanged(Agent agent, Agent.AIStateFlag flag)
		{
			if (agent.Character == this._enemyCharacterObject && (agent.IsUsingGameObject || AgentComponentExtensions.AIInterestedInAnyGameObject(agent)))
			{
				agent.StopUsingGameObject(true, 1);
			}
		}

		// Token: 0x06000285 RID: 645 RVA: 0x00013458 File Offset: 0x00011658
		public void FinalizeMission()
		{
			this._isFinalized = true;
			MBMusicManager.Current.ForceStopThemeWithFadeOut();
			base.Mission.EndMission();
		}

		// Token: 0x06000286 RID: 646 RVA: 0x00013476 File Offset: 0x00011676
		public override bool MissionEnded(ref MissionResult missionResult)
		{
			if (this._isFinalized)
			{
				missionResult = MissionResult.CreateSuccessful(base.Mission, false);
				return true;
			}
			return false;
		}

		// Token: 0x06000287 RID: 647 RVA: 0x00013494 File Offset: 0x00011694
		public void OnShipCaptured()
		{
			foreach (Agent agent in base.Mission.PlayerTeam.ActiveAgents)
			{
				if (!agent.IsPlayerControlled || agent != this._gunnarAgent)
				{
					agent.AgentVisuals.SetVisible(true);
				}
			}
			this._gunnarAgent.Controller = 1;
			this._gunnarAgent.ClearTargetFrame();
			base.Mission.SetMissionMode(2, true);
			this.SpawnScatteredCrew();
			this.MissionShip.SetController(ShipControllerType.None, true);
			this.MissionShip.ShipOrder.SetShipStopOrder();
			MissionShip missionShip = this.MissionShip;
			ShipInputRecord shipInputRecord = ShipInputRecord.None();
			missionShip.SetInputRecord(in shipInputRecord);
			this.MissionShip.SetCustomSailSetting(false, SailInput.Raised);
			this.MissionShip.SetShipOrderActive(false);
			Formation formation = Mission.GetTeam(0).GetFormation(0);
			this.MissionShip.ShipOrder.SetFormation(formation);
			if (this._oarUsedByAlly.PilotStandingPoint.UserAgent != null && this._oarUsedByAlly.PilotStandingPoint.UserAgent != this._gunnarAgent)
			{
				this._oarUsedByAlly.PilotStandingPoint.UserAgent.StopUsingGameObject(true, 1);
			}
			this.HandleChainVisualsAfterDialogue();
			this.MissionShip.SetAnchor(false, false, 1f);
		}

		// Token: 0x06000288 RID: 648 RVA: 0x000135F4 File Offset: 0x000117F4
		public bool IsSavedCrew(IAgent agent)
		{
			return this._savedScatteredAgents.Contains(agent);
		}

		// Token: 0x06000289 RID: 649 RVA: 0x00013604 File Offset: 0x00011804
		private void OnAllCrewSaved()
		{
			this._allScatteredCrewMembersAreSaved = true;
			this.OnStartFadeOutEvent(0.75f, 1f, 0.75f);
			this._crewConversationAgent = this._savedScatteredAgents[this._savedScatteredAgents.Count - 1];
			if (this.MissionShip.IsPlayerControlled)
			{
				Agent.Main.HandleStopUsingAction();
			}
		}

		// Token: 0x0600028A RID: 650 RVA: 0x00013668 File Offset: 0x00011868
		private void SetupSavedCrewConversation()
		{
			this._isConversationSetupInProgress = true;
			GameEntity gameEntity = this._entities.First<GameEntity>((GameEntity t) => t.HasTag("conversation_player"));
			GameEntity gameEntity2 = this._entities.First<GameEntity>((GameEntity t) => t.HasTag("conversation_ally"));
			GameEntity gameEntity3 = this._entities.First<GameEntity>((GameEntity t) => t.HasTag("conversation_crew"));
			Agent.Main.AgentVisuals.SetVisible(true);
			foreach (Agent agent in base.Mission.PlayerTeam.ActiveAgents)
			{
				if (!agent.IsPlayerControlled && agent != this._gunnarAgent && agent != this._crewConversationAgent)
				{
					agent.AgentVisuals.SetVisible(false);
				}
			}
			if (this._gunnarAgent.IsUsingGameObject)
			{
				this._gunnarAgent.StopUsingGameObject(true, 1);
				this._gunnarAgent.SetActionChannel(0, ref ActionIndexCache.act_none, true, 0L, 0f, 1f, 0f, 0.4f, 0f, false, -0.2f, 0, true);
				this._gunnarAgent.SetActionChannel(1, ref ActionIndexCache.act_none, true, 0L, 0f, 1f, 0f, 0.4f, 0f, false, -0.2f, 0, true);
			}
			if (this._crewConversationAgent.IsUsingGameObject)
			{
				this._crewConversationAgent.StopUsingGameObject(true, 1);
				this._crewConversationAgent.SetActionChannel(0, ref ActionIndexCache.act_none, true, 0L, 0f, 1f, 0f, 0.4f, 0f, false, -0.2f, 0, true);
				this._crewConversationAgent.SetActionChannel(1, ref ActionIndexCache.act_none, true, 0L, 0f, 1f, 0f, 0.4f, 0f, false, -0.2f, 0, true);
			}
			if (Agent.Main.IsUsingGameObject)
			{
				Agent.Main.StopUsingGameObject(true, 1);
			}
			if (!this.MissionShip.HasController)
			{
				MissionShip missionShip = this.MissionShip;
				ShipInputRecord shipInputRecord = ShipInputRecord.None();
				missionShip.SetInputRecord(in shipInputRecord);
			}
			else if (this.MissionShip.IsAIControlled)
			{
				this.MissionShip.ShipOrder.SetShipStopOrder();
			}
			if (!this.MissionShip.Physics.IsAnchored)
			{
				this.MissionShip.Physics.SetAnchor(true, false, 1f);
			}
			this._gunnarAgent.ClearTargetFrame();
			this._gunnarAgent.TeleportToPosition(gameEntity2.GlobalPosition);
			Agent.Main.TeleportToPosition(gameEntity.GlobalPosition);
			this._crewConversationAgent.TeleportToPosition(gameEntity3.GlobalPosition);
			this._crewConversationAgent.ClearTargetFrame();
			WorldPosition worldPosition;
			worldPosition..ctor(base.Mission.Scene, gameEntity3.GlobalPosition);
			this._crewConversationAgent.SetScriptedPosition(ref worldPosition, true, 16);
			Vec3 vec = this._crewConversationAgent.Position - Agent.Main.Position;
			this.OnConversationSetupEvent(vec);
			Agent crewConversationAgent = this._crewConversationAgent;
			Vec2 vec2 = vec.AsVec2;
			vec2 = -vec2.Normalized();
			crewConversationAgent.SetMovementDirection(ref vec2);
			this._crewConversationAgent.Controller = 0;
			WorldPosition worldPosition2;
			worldPosition2..ctor(Mission.Current.Scene, gameEntity2.GlobalPosition);
			Agent gunnarAgent = this._gunnarAgent;
			vec2 = vec.AsVec2;
			gunnarAgent.SetScriptedPositionAndDirection(ref worldPosition2, -vec2.RotationInRadians, false, 0);
			Vec3 vec3 = Agent.Main.Position - gameEntity2.GlobalPosition;
			Agent gunnarAgent2 = this._gunnarAgent;
			vec2 = vec3.AsVec2;
			vec2 = vec2.Normalized();
			gunnarAgent2.SetMovementDirection(ref vec2);
			this._gunnarAgent.Controller = 0;
			this.MissionShip.ShipOrder.SetShipStopOrder();
		}

		// Token: 0x0600028B RID: 651 RVA: 0x00013A64 File Offset: 0x00011C64
		private void OnPlayerReachedFirstZone()
		{
			CampaignInformationManager.AddDialogLine(new TextObject("{=wYMz91k4}Right - now let’s slow down so that they can climb aboard.", null), this._allyCharacterObject, this._allyCharacterObject.FirstCivilianEquipment, 1000, 2);
		}

		// Token: 0x0600028C RID: 652 RVA: 0x00013A8E File Offset: 0x00011C8E
		private void OnFirstHighlightCleared()
		{
			CampaignInformationManager.AddDialogLine(new TextObject("{=HuChgeJp}There’s two more of them over there. Let’s go fish them out.", null), this._allyCharacterObject, this._allyCharacterObject.FirstCivilianEquipment, 1000, 2);
		}

		// Token: 0x0600028D RID: 653 RVA: 0x00013AB8 File Offset: 0x00011CB8
		private void StartSavedCrewConversation()
		{
			this._hasTalkedToGunnarOutro = true;
			this._isConversationSetupInProgress = false;
			Mission.Current.SetMissionMode(1, true);
			Campaign.Current.ConversationManager.SetupAndStartMissionConversation(this._crewConversationAgent, base.Mission.MainAgent, true);
			Campaign.Current.ConversationManager.AddConversationAgents(new List<IAgent> { this._gunnarAgent }, true);
		}

		// Token: 0x0600028E RID: 654 RVA: 0x00013B21 File Offset: 0x00011D21
		public override void OnTutorialCompleted(string completedTutorialIdentifier)
		{
			if (completedTutorialIdentifier == "ShipCameraTutorial")
			{
				this.OnCameraTutorialFinished();
			}
		}

		// Token: 0x0600028F RID: 655 RVA: 0x00013B36 File Offset: 0x00011D36
		private void OnCameraTutorialFinished()
		{
			CampaignInformationManager.AddDialogLine(new TextObject("{=o8Jj8RJ1}Can you see those poor lads thrashing in the water over there?", null), this._allyCharacterObject, this._allyCharacterObject.FirstCivilianEquipment, 1000, 2);
		}

		// Token: 0x06000290 RID: 656 RVA: 0x00013B60 File Offset: 0x00011D60
		public ShipControllerMachine GetMarkedShipControllerMachine()
		{
			if (this.HasTalkedToGunnar)
			{
				Agent userAgent = this.MissionShip.ShipControllerMachine.PilotStandingPoint.UserAgent;
				if (userAgent == null || !userAgent.IsPlayerControlled)
				{
					return this.MissionShip.ShipControllerMachine;
				}
			}
			return null;
		}

		// Token: 0x06000291 RID: 657 RVA: 0x00013B9D File Offset: 0x00011D9D
		public List<AgentBindsMachine> GetMarkedAgentBinds()
		{
			return this._agentBindMachines.Where<AgentBindsMachine>((AgentBindsMachine t) => !t.PilotStandingPoint.IsDisabledForPlayers).ToList<AgentBindsMachine>();
		}

		// Token: 0x06000292 RID: 658 RVA: 0x00013BCE File Offset: 0x00011DCE
		public List<Agent> GetScatteredCrewmen()
		{
			return this._scatteredCrew.Select<ValueTuple<Agent, bool>, Agent>((ValueTuple<Agent, bool> t) => t.Item1).ToList<Agent>();
		}

		// Token: 0x06000293 RID: 659 RVA: 0x00013BFF File Offset: 0x00011DFF
		public List<Agent> GetCaptorAgents()
		{
			return Mission.Current.PlayerEnemyTeam.ActiveAgents.ToList<Agent>();
		}

		// Token: 0x06000294 RID: 660 RVA: 0x00013C15 File Offset: 0x00011E15
		public bool IsFirstHighlightCleared()
		{
			return this._savedScatteredAgents.Count<Agent>((Agent t) => t.IsOnLand()) == 2;
		}

		// Token: 0x06000295 RID: 661 RVA: 0x00013C44 File Offset: 0x00011E44
		public bool IsReadyToCloseSails()
		{
			return this.IsFirstHighlightCleared() && this._scatteredCrew.Count > 0 && (this._scatteredCrew.FirstOrDefault<ValueTuple<Agent, bool>>().Item1.Position - this.MissionShip.GlobalFrame.origin).LengthSquared <= 8100f;
		}

		// Token: 0x06000296 RID: 662 RVA: 0x00013CA5 File Offset: 0x00011EA5
		public float GetStoppedShipSpeedThreshold()
		{
			return 2f;
		}

		// Token: 0x06000297 RID: 663 RVA: 0x00013CAC File Offset: 0x00011EAC
		public bool IsPlayerInShipControls()
		{
			return this.MissionShip != null && Agent.Main != null && this.MissionShip.ShipControllerMachine.PilotStandingPoint.UserAgent == Agent.Main;
		}

		// Token: 0x04000166 RID: 358
		private const int ScatteredCrewCountPerArea = 2;

		// Token: 0x04000167 RID: 359
		private const string PlayerEquipmentId = "item_set_player_captivity";

		// Token: 0x04000168 RID: 360
		private const string GunnarEquipmentId = "item_set_gangradir_captivity";

		// Token: 0x04000169 RID: 361
		private const float InitialOarForceMultiplier = 0.01f;

		// Token: 0x0400016A RID: 362
		private const float FinalOarForceMultiplier = 0.95f;

		// Token: 0x0400016B RID: 363
		private const float CloseSailsDistanceToFinalHighlight = 90f;

		// Token: 0x0400016C RID: 364
		private const float WindStrength = 1.1f;

		// Token: 0x0400016D RID: 365
		private const float FadeInDuration = 0.75f;

		// Token: 0x0400016E RID: 366
		private const float BlackDuration = 1f;

		// Token: 0x0400016F RID: 367
		private const float FadeOutDuration = 0.75f;

		// Token: 0x04000170 RID: 368
		private int _missionInitializationPeriod;

		// Token: 0x04000171 RID: 369
		private MissionObjectiveLogic _missionObjectiveLogic;

		// Token: 0x04000172 RID: 370
		private Agent _gunnarAgent;

		// Token: 0x04000173 RID: 371
		private readonly List<Agent> _crewAgents = new List<Agent>();

		// Token: 0x04000174 RID: 372
		private readonly CharacterObject _allyCharacterObject;

		// Token: 0x04000175 RID: 373
		private readonly BasicCharacterObject _enemyCharacterObject;

		// Token: 0x04000176 RID: 374
		private readonly CharacterObject _crewCharacterObject;

		// Token: 0x04000177 RID: 375
		private ShipOarMachine _oarUsedByPlayer;

		// Token: 0x04000178 RID: 376
		private ShipOarMachine _oarUsedByAlly;

		// Token: 0x04000179 RID: 377
		private List<GameEntity> _entities = new List<GameEntity>();

		// Token: 0x0400017A RID: 378
		private readonly List<ValueTuple<Agent, bool>> _scatteredCrew = new List<ValueTuple<Agent, bool>>();

		// Token: 0x0400017B RID: 379
		private readonly List<Agent> _savedScatteredAgents = new List<Agent>();

		// Token: 0x0400017C RID: 380
		private bool _allScatteredCrewMembersAreSaved;

		// Token: 0x0400017D RID: 381
		private bool _hasTalkedToGunnarOutro;

		// Token: 0x0400017E RID: 382
		private float _outroSpeechDelayTimer;

		// Token: 0x0400017F RID: 383
		private SpawnedItemEntity _weaponEntity;

		// Token: 0x04000181 RID: 385
		private GameEntity _spawnZone1;

		// Token: 0x04000182 RID: 386
		private GameEntity _spawnZone2;

		// Token: 0x04000183 RID: 387
		private bool _isFinalized;

		// Token: 0x04000184 RID: 388
		private bool _hasSavedOarsmen;

		// Token: 0x04000185 RID: 389
		private SoundEvent _spawnZone1HelpSoundEvent;

		// Token: 0x04000186 RID: 390
		private SoundEvent _spawnZone2HelpSoundEvent;

		// Token: 0x04000187 RID: 391
		private int _savedOarsmenCount;

		// Token: 0x04000188 RID: 392
		private bool _hasTalkedToGunnar;

		// Token: 0x04000189 RID: 393
		private bool _isConversationSetupInProgress;

		// Token: 0x0400018A RID: 394
		private int _spawnedOarsmenCount;

		// Token: 0x0400018B RID: 395
		private float _speechDelayTimer;

		// Token: 0x0400018C RID: 396
		private int _saveTargetAgentCount;

		// Token: 0x0400018D RID: 397
		private ActionIndexCache _tinkeringAction;

		// Token: 0x0400018E RID: 398
		private bool _isPlayerTinkeringWithTheBindsMachine;

		// Token: 0x04000190 RID: 400
		private int _previousOarsmenLevel;

		// Token: 0x04000191 RID: 401
		private List<AgentBindsMachine> _agentBindMachines = new List<AgentBindsMachine>();

		// Token: 0x04000192 RID: 402
		private List<ShipOarMachine> _leftOars = new List<ShipOarMachine>();

		// Token: 0x04000193 RID: 403
		private List<ShipOarMachine> _rightOars = new List<ShipOarMachine>();

		// Token: 0x04000194 RID: 404
		private Dictionary<Agent, ShipOarMachine> _oarAssignments = new Dictionary<Agent, ShipOarMachine>();

		// Token: 0x04000195 RID: 405
		private Agent _crewConversationAgent;

		// Token: 0x04000196 RID: 406
		public Action OnMarkedObjectStatusChangedEvent;

		// Token: 0x04000197 RID: 407
		public Action OnPlayerStartedEscapeEvent;

		// Token: 0x04000198 RID: 408
		public Action<Vec3> OnConversationSetupEvent;

		// Token: 0x04000199 RID: 409
		public Action<int> OnOarsmenLevelChanged;

		// Token: 0x0400019A RID: 410
		public Action<float, float, float> OnStartFadeOutEvent;

		// Token: 0x0400019B RID: 411
		public Action OnFirstHighlightClearedEvent;
	}
}
