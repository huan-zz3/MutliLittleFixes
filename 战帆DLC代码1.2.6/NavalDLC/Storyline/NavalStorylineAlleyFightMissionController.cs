using System;
using System.Collections.Generic;
using System.Linq;
using SandBox;
using SandBox.Missions.AgentBehaviors;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Storyline
{
	// Token: 0x0200002E RID: 46
	public class NavalStorylineAlleyFightMissionController : MissionLogic
	{
		// Token: 0x06000244 RID: 580 RVA: 0x000102F8 File Offset: 0x0000E4F8
		public override void EarlyStart()
		{
			base.EarlyStart();
			base.Mission.Teams.Add(0, Clan.PlayerClan.Color, Clan.PlayerClan.Color2, Clan.PlayerClan.Banner, true, false, true);
			base.Mission.Teams.Add(1, NavalStorylineData.CorsairBanner.GetPrimaryColor(), NavalStorylineData.CorsairBanner.GetSecondaryColor(), NavalStorylineData.CorsairBanner, true, false, true);
			base.Mission.PlayerTeam = base.Mission.DefenderTeam;
		}

		// Token: 0x06000245 RID: 581 RVA: 0x00010384 File Offset: 0x0000E584
		public override void OnMissionTick(float dt)
		{
			if (!this._isMissionInitialized)
			{
				this._isMissionInitialized = true;
				this.UpdateEntityReferences();
				Team team = Mission.GetTeam(0);
				Formation formation = team.GetFormation(0);
				Mission.GetTeam(2);
				this.SpawnPlayer();
				GameEntity gameEntity = this._entities.FirstOrDefault<GameEntity>((GameEntity t) => t.HasTag("sp_gangradir"));
				this.SpawnGunnar(gameEntity);
				this.SpawnEnemyTroop("sp_thug_1", "act_argue_trio_right");
				this.SpawnEnemyTroop("sp_thug_2", "act_argue_trio_middle_2");
				this.SpawnEnemyTroop("sp_thug_3", "act_argue_trio_left");
				team.SetPlayerRole(true, true);
				formation.PlayerOwner = Agent.Main;
				Mission.Current.OnDeploymentFinished();
			}
			if (this._willGunnarBecomeVulnerable)
			{
				this._gunnarInvulnerabilityTimer += dt;
				if (this._gunnarInvulnerabilityTimer >= this._gunnarInvulnerabilityDurationAfterCinematic)
				{
					this._gunnarAgent.ToggleInvulnerable();
					this._willGunnarBecomeVulnerable = false;
				}
			}
			if (this._shoulStartOutroConversation)
			{
				this._speechDelayTimer += dt;
				if (this._speechDelayTimer >= 1.5f)
				{
					this._shoulStartOutroConversation = false;
					this.TriggerCombatEnd();
				}
			}
			if (this._shouldShowEndNotification)
			{
				GameTexts.SetVariable("leave_key", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("Generic", 4), 1f));
				this.ShowNotification(GameTexts.FindText("str_battle_won_press_tab_to_view_results", null), null);
				this._shouldShowEndNotification = false;
			}
			if (this._shouldShowBanterNotifications)
			{
				this._banterNotificationTimer += dt;
				if (this._banterNotificationTimer > 12f)
				{
					this._banterNotificationTimer = 0f;
					TextObject textObject = this._banterLines[this._banterLineIndex];
					this.ShowNotification(textObject, Extensions.GetRandomElement<Agent>(base.Mission.Teams.PlayerEnemy.ActiveAgents).Character);
					this._banterLineIndex = (this._banterLineIndex + 1) % this._banterLines.Count;
				}
			}
			if (this._isEnemyAttackToPlayerQueued)
			{
				this._enemyAttackToPlayerTimer += dt;
				if (this._enemyAttackToPlayerTimer >= this._enemyAttackToPlayerDuration)
				{
					this._isEnemyAttackToPlayerQueued = false;
					Agent randomElement = Extensions.GetRandomElement<Agent>(base.Mission.PlayerEnemyTeam.ActiveAgents);
					if (randomElement != null)
					{
						this._directedEnemyAgent = randomElement;
					}
				}
			}
			if (this._directedEnemyAgent != null && this._directedEnemyAgent.IsActive())
			{
				if (this._directedEnemyAgent.Position.DistanceSquared(Agent.Main.Position) <= 1f)
				{
					this._directedEnemyAgent.ClearTargetFrame();
					this._directedEnemyAgent = null;
					return;
				}
				WorldPosition worldPosition;
				worldPosition..ctor(base.Mission.Scene, Agent.Main.Position);
				this._directedEnemyAgent.SetScriptedPosition(ref worldPosition, false, 8);
			}
		}

		// Token: 0x06000246 RID: 582 RVA: 0x0001062B File Offset: 0x0000E82B
		private void UpdateEntityReferences()
		{
			base.Mission.Scene.GetEntities(ref this._entities);
		}

		// Token: 0x06000247 RID: 583 RVA: 0x00010643 File Offset: 0x0000E843
		public void OnCinematicStarted()
		{
			this._shouldShowBanterNotifications = false;
			Mission.Current.SetMissionMode(9, true);
		}

		// Token: 0x06000248 RID: 584 RVA: 0x0001065C File Offset: 0x0000E85C
		public void StartFight()
		{
			Mission.Current.SetMissionMode(2, true);
			this._willGunnarBecomeVulnerable = true;
			foreach (Agent agent in base.Mission.Teams.PlayerEnemy.ActiveAgents)
			{
				agent.ToggleInvulnerable();
			}
			this.OnTeamAgentsShouldAttack(base.Mission.Teams.Player);
			this.OnTeamAgentsShouldAttack(base.Mission.Teams.PlayerEnemy);
			base.Mission.PlayerTeam.MasterOrderController.SelectAllFormations(false);
			base.Mission.PlayerTeam.MasterOrderController.SetOrder(4);
			this._isEnemyAttackToPlayerQueued = true;
			this.ShowNotification(new TextObject("{=6zHJnnil}Hey you! Stranger! Would you like to help an old man drive off a few stray dogs here?", null), NavalStorylineData.Gunnar.CharacterObject);
		}

		// Token: 0x06000249 RID: 585 RVA: 0x00010748 File Offset: 0x0000E948
		private void SpawnPlayer()
		{
			GameEntity gameEntity = this._entities.FirstOrDefault<GameEntity>((GameEntity t) => t.HasTag("sp_player"));
			Formation formation = base.Mission.PlayerTeam.GetFormation(0);
			AgentBuildData agentBuildData = new AgentBuildData(Hero.MainHero.CharacterObject).TroopOrigin(new SimpleAgentOrigin(Hero.MainHero.CharacterObject, -1, null, default(UniqueTroopDescriptor))).Team(base.Mission.PlayerTeam);
			Vec3 globalPosition = gameEntity.GlobalPosition;
			AgentBuildData agentBuildData2 = agentBuildData.InitialPosition(ref globalPosition);
			Vec2 asVec = gameEntity.GetGlobalFrame().rotation.f.AsVec2;
			AgentBuildData agentBuildData3 = agentBuildData2.InitialDirection(ref asVec).NoHorses(true).NoWeapons(false)
				.Formation(formation);
			Mission.Current.SpawnAgent(agentBuildData3, false).Controller = 2;
		}

		// Token: 0x0600024A RID: 586 RVA: 0x00010828 File Offset: 0x0000EA28
		private void SpawnGunnar(GameEntity spawnPoint)
		{
			MBEquipmentRoster @object = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>("item_set_gangradir_alleyfight");
			AgentBuildData agentBuildData = new AgentBuildData(NavalStorylineData.Gunnar.CharacterObject).TroopOrigin(new PartyAgentOrigin(PartyBase.MainParty, NavalStorylineData.Gunnar.CharacterObject, -1, default(UniqueTroopDescriptor), false, false)).Team(base.Mission.PlayerTeam);
			Vec3 globalPosition = spawnPoint.GlobalPosition;
			AgentBuildData agentBuildData2 = agentBuildData.InitialPosition(ref globalPosition);
			Vec2 asVec = spawnPoint.GetGlobalFrame().rotation.f.AsVec2;
			AgentBuildData agentBuildData3 = agentBuildData2.InitialDirection(ref asVec).NoHorses(true).NoWeapons(false)
				.Equipment(@object.DefaultEquipment);
			this._gunnarAgent = Mission.Current.SpawnAgent(agentBuildData3, false);
			MBActionSet actionSet = MBGlobals.GetActionSet("as_human_hideout_bandit");
			AnimationSystemData animationSystemData = MonsterExtensions.FillAnimationSystemData(agentBuildData3.AgentMonster, actionSet, NavalStorylineData.Gunnar.CharacterObject.GetStepSize(), false);
			this._gunnarAgent.SetActionSet(ref animationSystemData);
			this._gunnarAgent.SetActionChannel(0, ref ActionIndexCache.act_argue_trio_middle, false, 0L, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
			UsableMachine firstScriptOfType = spawnPoint.GetFirstScriptOfType<UsableMachine>();
			if (firstScriptOfType != null)
			{
				StandingPoint standingPoint = firstScriptOfType.StandingPoints.FirstOrDefault<StandingPoint>();
				this._gunnarAgent.UseGameObject(standingPoint, -1);
			}
			this._gunnarAgent.ToggleInvulnerable();
		}

		// Token: 0x0600024B RID: 587 RVA: 0x00010988 File Offset: 0x0000EB88
		private void SpawnEnemyTroop(string spawnPointId, string animationId)
		{
			CharacterObject @object = Campaign.Current.ObjectManager.GetObject<CharacterObject>("naval_storyline_alley_fight_enemy");
			GameEntity gameEntity = this._entities.FirstOrDefault<GameEntity>((GameEntity t) => t.HasTag(spawnPointId));
			Vec3 globalPosition = gameEntity.GlobalPosition;
			Vec2 asVec = gameEntity.GetGlobalFrame().rotation.f.AsVec2;
			AgentBuildData agentBuildData = new AgentBuildData(@object).TroopOrigin(new SimpleAgentOrigin(@object, -1, null, default(UniqueTroopDescriptor))).Team(base.Mission.PlayerEnemyTeam).InitialPosition(ref globalPosition)
				.InitialDirection(ref asVec)
				.NoHorses(true)
				.NoWeapons(false)
				.Banner(NavalStorylineData.CorsairBanner);
			Agent agent = Mission.Current.SpawnAgent(agentBuildData, false);
			MBActionSet actionSet = MBGlobals.GetActionSet("as_human_hideout_bandit");
			AnimationSystemData animationSystemData = MonsterExtensions.FillAnimationSystemData(agentBuildData.AgentMonster, actionSet, @object.GetStepSize(), false);
			agent.SetActionSet(ref animationSystemData);
			ActionIndexCache actionIndexCache = ActionIndexCache.Create(animationId);
			agent.SetActionChannel(0, ref actionIndexCache, false, 0L, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
			StandingPoint standingPoint = gameEntity.GetFirstScriptOfType<UsableMachine>().StandingPoints.FirstOrDefault<StandingPoint>();
			agent.UseGameObject(standingPoint, -1);
			for (int i = 0; i < 50; i++)
			{
				agent.TickActionChannels(0.1f);
			}
			agent.ToggleInvulnerable();
		}

		// Token: 0x0600024C RID: 588 RVA: 0x00010AF4 File Offset: 0x0000ECF4
		private void OnTeamAgentsShouldAttack(Team team)
		{
			foreach (Agent agent in team.ActiveAgents)
			{
				AgentFlag agentFlags = agent.GetAgentFlags();
				agent.SetAgentFlags(agentFlags | 65536);
				CampaignAgentComponent component = agent.GetComponent<CampaignAgentComponent>();
				AgentNavigator agentNavigator = component.AgentNavigator;
				if (agentNavigator == null)
				{
					agentNavigator = component.CreateAgentNavigator();
					agentNavigator.AddBehaviorGroup<AlarmedBehaviorGroup>().AddBehavior<FightBehavior>();
				}
				agentNavigator.GetBehaviorGroup<AlarmedBehaviorGroup>().SetScriptedBehavior<FightBehavior>();
				agent.SetAlarmState(3);
				if (agent.IsUsingGameObject)
				{
					agent.StopUsingGameObject(true, 1);
				}
			}
		}

		// Token: 0x0600024D RID: 589 RVA: 0x00010BA0 File Offset: 0x0000EDA0
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			if (Extensions.IsEmpty<Agent>(base.Mission.PlayerEnemyTeam.ActiveAgents))
			{
				this.OnEnemyTeamDefeated();
				return;
			}
			if (Extensions.IsEmpty<Agent>(base.Mission.PlayerTeam.ActiveAgents) || affectedAgent.IsMainAgent)
			{
				this.OnPlayerTeamDefeated();
				if (affectedAgent.IsMainAgent)
				{
					base.Mission.EndMission();
				}
			}
		}

		// Token: 0x0600024E RID: 590 RVA: 0x00010C03 File Offset: 0x0000EE03
		private void OnEnemyTeamDefeated()
		{
			this._shoulStartOutroConversation = true;
		}

		// Token: 0x0600024F RID: 591 RVA: 0x00010C0C File Offset: 0x0000EE0C
		private void TriggerCombatEnd()
		{
			this._shouldShowBanterNotifications = false;
			Agent gunnarAgent = this._gunnarAgent;
			CampaignAgentComponent campaignAgentComponent = ((gunnarAgent != null) ? gunnarAgent.GetComponent<CampaignAgentComponent>() : null);
			AgentNavigator agentNavigator = ((campaignAgentComponent != null) ? campaignAgentComponent.AgentNavigator : null);
			if (agentNavigator != null)
			{
				AlarmedBehaviorGroup behaviorGroup = agentNavigator.GetBehaviorGroup<AlarmedBehaviorGroup>();
				if (behaviorGroup != null)
				{
					behaviorGroup.IsActive = false;
				}
			}
			base.Mission.GetMissionBehavior<NavalStorylineAlleyFightCinematicController>().OnFightEnded();
		}

		// Token: 0x06000250 RID: 592 RVA: 0x00010C64 File Offset: 0x0000EE64
		public void SetupConversation()
		{
			if (Agent.Main == null || !Agent.Main.IsActive())
			{
				this.SpawnPlayer();
			}
			GameEntity gameEntity = this._entities.FirstOrDefault<GameEntity>((GameEntity t) => t.HasTag("conversation_ally"));
			GameEntity gameEntity2 = this._entities.FirstOrDefault<GameEntity>((GameEntity t) => t.HasTag("conversation_player"));
			if (this._gunnarAgent == null || !this._gunnarAgent.IsActive())
			{
				this.SpawnGunnar(gameEntity);
			}
			if (gameEntity != null && gameEntity2 != null)
			{
				this._gunnarAgent.TeleportToPosition(gameEntity.GlobalPosition);
				this._gunnarAgent.SetTargetPosition(gameEntity.GlobalPosition.AsVec2);
				Agent.Main.TeleportToPosition(gameEntity2.GlobalPosition);
				Agent.Main.TryToSheathWeaponInHand(1, 1);
				Agent.Main.TryToSheathWeaponInHand(0, 1);
				Agent.Main.SetActionChannel(0, ref ActionIndexCache.act_none, true, 0L, 0f, 1f, 0f, 0.4f, 0f, false, -0.2f, 0, true);
				Agent.Main.SetActionChannel(1, ref ActionIndexCache.act_none, true, 0L, 0f, 1f, 0f, 0.4f, 0f, false, -0.2f, 0, true);
				this._gunnarAgent.TryToSheathWeaponInHand(1, 1);
				this._gunnarAgent.TryToSheathWeaponInHand(0, 1);
				this._gunnarAgent.SetActionChannel(0, ref ActionIndexCache.act_none, true, 0L, 0f, 1f, 0f, 0.4f, 0f, false, -0.2f, 0, true);
				this._gunnarAgent.SetActionChannel(1, ref ActionIndexCache.act_none, true, 0L, 0f, 1f, 0f, 0.4f, 0f, false, -0.2f, 0, true);
				Vec3 vec = Agent.Main.Position - this._gunnarAgent.Position;
				base.Mission.GetMissionBehavior<NavalStorylineAlleyFightCinematicController>().OnConversationSetup(-vec);
				Agent gunnarAgent = this._gunnarAgent;
				Vec2 vec2 = vec.AsVec2;
				vec2 = vec2.Normalized();
				gunnarAgent.SetMovementDirection(ref vec2);
				this._gunnarAgent.Controller = 0;
			}
		}

		// Token: 0x06000251 RID: 593 RVA: 0x00010EB2 File Offset: 0x0000F0B2
		public void StartPostFightConversation()
		{
			Campaign.Current.ConversationManager.SetupAndStartMissionConversation(this._gunnarAgent, base.Mission.MainAgent, true);
			Mission.Current.SetMissionMode(1, true);
		}

		// Token: 0x06000252 RID: 594 RVA: 0x00010EE1 File Offset: 0x0000F0E1
		private void ShowNotification(TextObject text, BasicCharacterObject speaker)
		{
			MBInformationManager.AddQuickInformation(text, 0, speaker, NavalStorylineData.Gunnar.CharacterObject.FirstCivilianEquipment, "");
		}

		// Token: 0x06000253 RID: 595 RVA: 0x00010EFF File Offset: 0x0000F0FF
		private void OnPlayerTeamDefeated()
		{
			this._isMissionFailed = true;
			this._shouldShowEndNotification = true;
		}

		// Token: 0x06000254 RID: 596 RVA: 0x00010F0F File Offset: 0x0000F10F
		public CharacterObject GetEnemyCharacterObject()
		{
			return Campaign.Current.ObjectManager.GetObject<CharacterObject>("naval_storyline_alley_fight_enemy");
		}

		// Token: 0x06000255 RID: 597 RVA: 0x00010F25 File Offset: 0x0000F125
		public void OnConversationEnded()
		{
			Mission.Current.EndMission();
		}

		// Token: 0x06000256 RID: 598 RVA: 0x00010F34 File Offset: 0x0000F134
		public override bool MissionEnded(ref MissionResult missionResult)
		{
			bool flag = false;
			if (this._isMissionFailed)
			{
				missionResult = MissionResult.CreateDefeated(base.Mission);
				flag = true;
			}
			return flag;
		}

		// Token: 0x06000257 RID: 599 RVA: 0x00010F5B File Offset: 0x0000F15B
		protected override void OnEndMission()
		{
			CampaignInformationManager.ClearAllDialogNotifications(true);
		}

		// Token: 0x04000150 RID: 336
		private const string EnemyTroopStringId = "naval_storyline_alley_fight_enemy";

		// Token: 0x04000151 RID: 337
		private const float SpeechDelayAfterCombatDuration = 1.5f;

		// Token: 0x04000152 RID: 338
		private const float BanterNotificationRepeatDuration = 12f;

		// Token: 0x04000153 RID: 339
		private const string GunnarEquipmentId = "item_set_gangradir_alleyfight";

		// Token: 0x04000154 RID: 340
		private bool _isMissionInitialized;

		// Token: 0x04000155 RID: 341
		private bool _isMissionFailed;

		// Token: 0x04000156 RID: 342
		private List<GameEntity> _entities = new List<GameEntity>();

		// Token: 0x04000157 RID: 343
		private Agent _gunnarAgent;

		// Token: 0x04000158 RID: 344
		private bool _willGunnarBecomeVulnerable;

		// Token: 0x04000159 RID: 345
		private float _gunnarInvulnerabilityTimer;

		// Token: 0x0400015A RID: 346
		private float _gunnarInvulnerabilityDurationAfterCinematic = 10f;

		// Token: 0x0400015B RID: 347
		private bool _shouldShowEndNotification;

		// Token: 0x0400015C RID: 348
		private bool _shouldShowBanterNotifications = true;

		// Token: 0x0400015D RID: 349
		private float _banterNotificationTimer = 12f;

		// Token: 0x0400015E RID: 350
		private int _banterLineIndex;

		// Token: 0x0400015F RID: 351
		private List<TextObject> _banterLines = new List<TextObject>
		{
			new TextObject("{=kDQXVwSJ}Hey old man! We want a word.", null),
			new TextObject("{=J3eXaYJs}Don't worry - we just want to talk to you.", null),
			new TextObject("{=q7cvwXab}We're not going to hurt you.", null),
			new TextObject("{=aneZwbHJ}Easy there, grandpa. Hand off your sword hilt.", null)
		};

		// Token: 0x04000160 RID: 352
		private bool _shoulStartOutroConversation;

		// Token: 0x04000161 RID: 353
		private float _speechDelayTimer;

		// Token: 0x04000162 RID: 354
		private bool _isEnemyAttackToPlayerQueued;

		// Token: 0x04000163 RID: 355
		private float _enemyAttackToPlayerTimer;

		// Token: 0x04000164 RID: 356
		private float _enemyAttackToPlayerDuration = 3f;

		// Token: 0x04000165 RID: 357
		private Agent _directedEnemyAgent;
	}
}
