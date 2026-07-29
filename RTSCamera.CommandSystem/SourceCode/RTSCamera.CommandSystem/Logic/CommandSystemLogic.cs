using System;
using System.Collections.Generic;
using MissionSharedLibrary.Config;
using RTSCamera.CommandSystem.AgentComponents;
using RTSCamera.CommandSystem.Config;
using RTSCamera.CommandSystem.Config.HotKey;
using RTSCamera.CommandSystem.Logic.SubLogic;
using RTSCamera.CommandSystem.Patch;
using RTSCamera.CommandSystem.QuerySystem;
using RTSCamera.CommandSystem.Utilities;
using RTSCameraAgentComponent;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade;

namespace RTSCamera.CommandSystem.Logic
{
	// Token: 0x02000082 RID: 130
	public class CommandSystemLogic : MissionLogic, IMissionListener
	{
		// Token: 0x060004CC RID: 1228 RVA: 0x0001C814 File Offset: 0x0001AA14
		public CommandSystemLogic()
		{
			this.OutlineColorSubLogic = new FormationColorSubLogicV2(() => this._config.TroopHighlightStyleInCharacterMode == TroopHighlightStyle.Outline, () => this._config.TroopHighlightStyleInRTSMode == TroopHighlightStyle.Outline, () => this._config.IsMouseOverEnabled(), () => this._config.HighlightTroopsWithoutFormation, delegate(Agent agent, int level, uint? color, bool alwaysVisible, bool updateInstantly)
			{
				RTSCameraComponent component = agent.GetComponent<RTSCameraComponent>();
				if (component == null)
				{
					return;
				}
				component.SetContourColor(level, color, alwaysVisible, updateInstantly);
			}, delegate(Agent agent, bool updateInstantly)
			{
				RTSCameraComponent component2 = agent.GetComponent<RTSCameraComponent>();
				if (component2 == null)
				{
					return;
				}
				component2.ClearFormationColor(updateInstantly);
			}, delegate(Agent agent)
			{
				RTSCameraComponent component3 = agent.GetComponent<RTSCameraComponent>();
				if (component3 == null)
				{
					return;
				}
				component3.UpdateContour();
			}, delegate(Formation formation)
			{
				formation.ApplyActionOnEachUnit(delegate(Agent agent)
				{
					RTSCameraComponent component4 = agent.GetComponent<RTSCameraComponent>();
					if (component4 == null)
					{
						return;
					}
					component4.ClearTargetOrSelectedFormationColor();
				}, null);
			}, delegate(Formation formation)
			{
				formation.ApplyActionOnEachUnit(delegate(Agent a)
				{
					RTSCameraComponent component5 = a.GetComponent<RTSCameraComponent>();
					if (component5 == null)
					{
						return;
					}
					component5.UpdateContour();
				}, null);
			});
			this.GroundMarkerColorSubLogic = new FormationColorSubLogicV2(() => this._config.TroopHighlightStyleInCharacterMode == TroopHighlightStyle.GroundMarker, () => this._config.TroopHighlightStyleInRTSMode == TroopHighlightStyle.GroundMarker, () => this._config.IsMouseOverEnabled(), () => this._config.HighlightTroopsWithoutFormation, delegate(Agent agent, int level, uint? color, bool alwaysVisible, bool updateInstantly)
			{
				CommandSystemAgentComponent component6 = agent.GetComponent<CommandSystemAgentComponent>();
				if (component6 == null)
				{
					return;
				}
				component6.SetColor(level, color, alwaysVisible, updateInstantly);
			}, delegate(Agent agent, bool updateInstantly)
			{
				CommandSystemAgentComponent component7 = agent.GetComponent<CommandSystemAgentComponent>();
				if (component7 == null)
				{
					return;
				}
				component7.ClearFormationColor(updateInstantly);
			}, delegate(Agent agent)
			{
				CommandSystemAgentComponent component8 = agent.GetComponent<CommandSystemAgentComponent>();
				if (component8 == null)
				{
					return;
				}
				component8.TryUpdateColor();
			}, delegate(Formation formation)
			{
				formation.ApplyActionOnEachUnit(delegate(Agent agent)
				{
					CommandSystemAgentComponent component9 = agent.GetComponent<CommandSystemAgentComponent>();
					if (component9 == null)
					{
						return;
					}
					component9.ClearTargetOrSelectedFormationColor();
				}, null);
			}, delegate(Formation formation)
			{
				formation.ApplyActionOnEachUnit(delegate(Agent a)
				{
					CommandSystemAgentComponent component10 = a.GetComponent<CommandSystemAgentComponent>();
					if (component10 == null)
					{
						return;
					}
					component10.TryUpdateColor();
				}, null);
			});
		}

		// Token: 0x060004CD RID: 1229 RVA: 0x0001C9DE File Offset: 0x0001ABDE
		public void OnMovementOrderChanged(Formation formation)
		{
			this.OutlineColorSubLogic.OnMovementOrderChanged(formation);
			this.GroundMarkerColorSubLogic.OnMovementOrderChanged(formation);
		}

		// Token: 0x060004CE RID: 1230 RVA: 0x0001C9F8 File Offset: 0x0001ABF8
		public void OnMovementOrderChanged(IEnumerable<Formation> appliedFormations)
		{
			this.OutlineColorSubLogic.OnMovementOrderChanged(appliedFormations);
			this.GroundMarkerColorSubLogic.OnMovementOrderChanged(appliedFormations);
		}

		// Token: 0x060004CF RID: 1231 RVA: 0x0001CA12 File Offset: 0x0001AC12
		public override void OnAfterMissionCreated()
		{
			base.OnAfterMissionCreated();
			Patch_OrderController.OnAfterMissionCreated();
		}

		// Token: 0x060004D0 RID: 1232 RVA: 0x0001CA20 File Offset: 0x0001AC20
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			Patch_MovementOrder.Patch();
			this.OutlineColorSubLogic.OnBehaviourInitialize();
			this.GroundMarkerColorSubLogic.OnBehaviourInitialize();
			Patch_OrderTroopPlacer.OnBehaviorInitialize();
			CommandQueueLogic.OnBehaviorInitialize();
			CommandQuerySystem.OnBehaviorInitialize();
			Patch_GauntletOrderUIHandler.OnBehaviorInitialize();
			CommandSystemConfig commandSystemConfig = MissionConfigBase<CommandSystemConfig>.Get();
			if (!commandSystemConfig.HasHintDisplayed)
			{
				commandSystemConfig.HasHintDisplayed = true;
				commandSystemConfig.Serialize();
				Utility.PrintOrderHint();
			}
		}

		// Token: 0x060004D1 RID: 1233 RVA: 0x0001CA84 File Offset: 0x0001AC84
		public override void OnRemoveBehavior()
		{
			foreach (Team team in Mission.Current.Teams)
			{
				if (team.FormationsIncludingSpecialAndEmpty != null)
				{
					foreach (Formation formation in team.FormationsIncludingSpecialAndEmpty)
					{
						formation.OnUnitAdded -= this.OnUnitAdded;
						formation.OnUnitRemoved -= this.OnUnitRemoved;
					}
				}
			}
			this.OutlineColorSubLogic.OnRemoveBehaviour();
			this.GroundMarkerColorSubLogic.OnRemoveBehaviour();
			Patch_OrderTroopPlacer.OnRemoveBehavior();
			Patch_OrderController.OnRemoveBehavior();
			CommandQueueLogic.OnRemoveBehavior();
			CommandQuerySystem.OnRemoveBehavior();
			Patch_GauntletOrderUIHandler.OnRemoveBehavior();
			CommandSystemAgentComponent.ClearMaterial();
		}

		// Token: 0x060004D2 RID: 1234 RVA: 0x0001CB70 File Offset: 0x0001AD70
		public override void AfterStart()
		{
			base.AfterStart();
			base.Mission.AddListener(this);
			CommandQueueLogic.AfterStart();
		}

		// Token: 0x060004D3 RID: 1235 RVA: 0x0001CB89 File Offset: 0x0001AD89
		public override void OnAddTeam(Team team)
		{
			base.OnAddTeam(team);
			Patch_OrderController.OnAddTeam(team);
		}

		// Token: 0x060004D4 RID: 1236 RVA: 0x0001CB98 File Offset: 0x0001AD98
		public override void OnPreDisplayMissionTick(float dt)
		{
			base.OnPreDisplayMissionTick(dt);
			this.OutlineColorSubLogic.OnPreDisplayMissionTick(dt);
			this.GroundMarkerColorSubLogic.OnPreDisplayMissionTick(dt);
			GameKeyContext category = HotKeyManager.GetCategory("CombatHotKeyCategory");
			if (category != null)
			{
				category.GetGameKey(5);
			}
			if (base.Mission.InputManager.IsGameKeyDown(5))
			{
				if (!this._isShowIndicatorsDown)
				{
					this._isShowIndicatorsDown = true;
					this.OutlineColorSubLogic.OnShowIndicatorKeyDownUpdate(this._isShowIndicatorsDown);
					this.GroundMarkerColorSubLogic.OnShowIndicatorKeyDownUpdate(this._isShowIndicatorsDown);
				}
			}
			else if (this._isShowIndicatorsDown)
			{
				this._isShowIndicatorsDown = false;
				this.OutlineColorSubLogic.OnShowIndicatorKeyDownUpdate(this._isShowIndicatorsDown);
				this.GroundMarkerColorSubLogic.OnShowIndicatorKeyDownUpdate(this._isShowIndicatorsDown);
			}
			if (CommandSystemGameKeyCategory.GetKey(GameKeyEnum.AutoVolley).IsKeyPressed(null))
			{
				Utility.ExecuteAutoVolley();
				return;
			}
			if (CommandSystemGameKeyCategory.GetKey(GameKeyEnum.ManualVolley).IsKeyPressed(null))
			{
				Utility.ExecuteManualVolley();
				return;
			}
			if (CommandSystemGameKeyCategory.GetKey(GameKeyEnum.VolleyFire).IsKeyPressed(null))
			{
				Utility.ExecuteVolleyFire();
			}
		}

		// Token: 0x060004D5 RID: 1237 RVA: 0x0001CC90 File Offset: 0x0001AE90
		public override void OnDeploymentFinished()
		{
			base.OnDeploymentFinished();
			if (base.Mission.PlayerTeam != null && MissionConfigBase<CommandSystemConfig>.Get().FacingEnemyByDefault)
			{
				foreach (Formation formation in base.Mission.PlayerTeam.FormationsIncludingEmpty)
				{
					if (base.Mission.PlayerTeam.PlayerOrderController.IsFormationSelectable(formation) && !formation.IsAIControlled && formation.PlayerOwner != null && formation.PlayerOwner == base.Mission.MainAgent)
					{
						formation.SetFacingOrder(FacingOrder.FacingOrderLookAtEnemy);
					}
				}
			}
		}

		// Token: 0x060004D6 RID: 1238 RVA: 0x0001CD50 File Offset: 0x0001AF50
		public override void AfterAddTeam(Team team)
		{
			this.OutlineColorSubLogic.AfterAddTeam(team);
			this.GroundMarkerColorSubLogic.AfterAddTeam(team);
			if (team.FormationsIncludingSpecialAndEmpty == null)
			{
				return;
			}
			foreach (Formation formation in team.FormationsIncludingSpecialAndEmpty)
			{
				formation.OnUnitAdded += this.OnUnitAdded;
				formation.OnUnitRemoved += this.OnUnitRemoved;
			}
		}

		// Token: 0x060004D7 RID: 1239 RVA: 0x0001CDE0 File Offset: 0x0001AFE0
		private void OnUnitAdded(Formation formation, Agent agent)
		{
			this.OutlineColorSubLogic.OnUnitAdded(formation, agent);
			this.GroundMarkerColorSubLogic.OnUnitAdded(formation, agent);
		}

		// Token: 0x060004D8 RID: 1240 RVA: 0x0001CDFC File Offset: 0x0001AFFC
		private void OnUnitRemoved(Formation formation, Agent agent)
		{
			this.OutlineColorSubLogic.OnUnitRemoved(formation, agent);
			this.GroundMarkerColorSubLogic.OnUnitRemoved(formation, agent);
			if (formation.CountOfUnits == 0)
			{
				CommandQueueLogic.OnFormationUnitsCleared(formation);
			}
		}

		// Token: 0x060004D9 RID: 1241 RVA: 0x0001CE26 File Offset: 0x0001B026
		public override void OnAgentCreated(Agent agent)
		{
			base.OnAgentCreated(agent);
			agent.AddComponent(new CommandSystemAgentComponent(agent));
		}

		// Token: 0x060004DA RID: 1242 RVA: 0x0001CE3B File Offset: 0x0001B03B
		public override void OnAgentBuild(Agent agent, Banner banner)
		{
			this.OutlineColorSubLogic.OnAgentBuild(agent, banner);
			this.GroundMarkerColorSubLogic.OnAgentBuild(agent, banner);
		}

		// Token: 0x060004DB RID: 1243 RVA: 0x0001CE57 File Offset: 0x0001B057
		public override void OnAgentFleeing(Agent affectedAgent)
		{
			base.OnAgentFleeing(affectedAgent);
			this.OutlineColorSubLogic.OnAgentFleeing(affectedAgent);
			this.GroundMarkerColorSubLogic.OnAgentFleeing(affectedAgent);
		}

		// Token: 0x060004DC RID: 1244 RVA: 0x0001CE78 File Offset: 0x0001B078
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			base.OnAgentRemoved(affectedAgent, affectorAgent, agentState, blow);
			this.OutlineColorSubLogic.OnAgentRemoved(affectedAgent);
			this.GroundMarkerColorSubLogic.OnAgentRemoved(affectedAgent);
		}

		// Token: 0x060004DD RID: 1245 RVA: 0x0001CE9D File Offset: 0x0001B09D
		public void OnEquipItemsFromSpawnEquipmentBegin(Agent agent, Agent.CreationType creationType)
		{
		}

		// Token: 0x060004DE RID: 1246 RVA: 0x0001CE9F File Offset: 0x0001B09F
		public void OnEquipItemsFromSpawnEquipment(Agent agent, Agent.CreationType creationType)
		{
			CommandSystemAgentComponent component = agent.GetComponent<CommandSystemAgentComponent>();
			if (component == null)
			{
				return;
			}
			component.Refresh();
		}

		// Token: 0x060004DF RID: 1247 RVA: 0x0001CEB1 File Offset: 0x0001B0B1
		void IMissionListener.OnEndMission()
		{
			base.Mission.RemoveListener(this);
		}

		// Token: 0x060004E0 RID: 1248 RVA: 0x0001CEBF File Offset: 0x0001B0BF
		public void OnConversationCharacterChanged()
		{
		}

		// Token: 0x060004E1 RID: 1249 RVA: 0x0001CEC1 File Offset: 0x0001B0C1
		public void OnResetMission()
		{
		}

		// Token: 0x060004E2 RID: 1250 RVA: 0x0001CEC3 File Offset: 0x0001B0C3
		public void OnDeploymentPlanMade(Team team, bool isFirstPlan)
		{
		}

		// Token: 0x060004E3 RID: 1251 RVA: 0x0001CEC5 File Offset: 0x0001B0C5
		protected override void OnAgentControllerChanged(Agent agent, AgentControllerType oldController)
		{
			base.OnAgentControllerChanged(agent, oldController);
			CommandSystemAgentComponent component = agent.GetComponent<CommandSystemAgentComponent>();
			if (component == null)
			{
				return;
			}
			component.OnControllerChanged(oldController);
		}

		// Token: 0x040001F2 RID: 498
		private CommandSystemConfig _config = MissionConfigBase<CommandSystemConfig>.Get();

		// Token: 0x040001F3 RID: 499
		public readonly FormationColorSubLogicV2 OutlineColorSubLogic;

		// Token: 0x040001F4 RID: 500
		public readonly FormationColorSubLogicV2 GroundMarkerColorSubLogic;

		// Token: 0x040001F5 RID: 501
		private bool _isShowIndicatorsDown;
	}
}
