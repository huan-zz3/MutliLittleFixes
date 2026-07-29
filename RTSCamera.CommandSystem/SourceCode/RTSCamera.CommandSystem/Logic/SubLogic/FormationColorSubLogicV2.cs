using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using MissionLibrary.Event;
using MissionSharedLibrary.Config;
using MissionSharedLibrary.Utilities;
using RTSCamera.CommandSystem.Config;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.GauntletUI.Mission.Singleplayer;
using TaleWorlds.MountAndBlade.View;

namespace RTSCamera.CommandSystem.Logic.SubLogic
{
	// Token: 0x02000086 RID: 134
	public class FormationColorSubLogicV2
	{
		// Token: 0x170000BD RID: 189
		// (get) Token: 0x06000524 RID: 1316 RVA: 0x0001EAA6 File Offset: 0x0001CCA6
		private OrderController PlayerOrderController
		{
			get
			{
				Team playerTeam = Mission.Current.PlayerTeam;
				if (playerTeam == null)
				{
					return null;
				}
				return playerTeam.PlayerOrderController;
			}
		}

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x06000525 RID: 1317 RVA: 0x0001EAC0 File Offset: 0x0001CCC0
		private bool ShouldHighlightWhenShowingIndicator
		{
			get
			{
				return this._isShowIndicatorDown && ((!this._isFreeCamera && this.HighlightEnabledInCharacterMode() && this._config.HighlightTroopsWhenShowingIndicators == ShowMode.Always) || (this._isFreeCamera && this.HighlightEnabledInRtsMode() && this._config.HighlightTroopsWhenShowingIndicators >= ShowMode.FreeCameraOnly));
			}
		}

		// Token: 0x170000BF RID: 191
		// (get) Token: 0x06000526 RID: 1318 RVA: 0x0001EB24 File Offset: 0x0001CD24
		private bool ShouldHighlightWhenShowingOrder
		{
			get
			{
				return this._isOrderShown && ((!this._isFreeCamera && this.HighlightEnabledInCharacterMode()) || (this._isFreeCamera && this.HighlightEnabledInRtsMode()));
			}
		}

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x06000527 RID: 1319 RVA: 0x0001EB5C File Offset: 0x0001CD5C
		private bool ShouldMouseOverFormationWithShowingOrder
		{
			get
			{
				return this.ShouldHighlightWhenShowingOrder && this.MouseOverEnabled();
			}
		}

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x06000528 RID: 1320 RVA: 0x0001EB73 File Offset: 0x0001CD73
		public Func<bool> HighlightEnabledInCharacterMode { get; }

		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x06000529 RID: 1321 RVA: 0x0001EB7B File Offset: 0x0001CD7B
		public Func<bool> HighlightEnabledInRtsMode { get; }

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x0600052A RID: 1322 RVA: 0x0001EB83 File Offset: 0x0001CD83
		public Func<bool> MouseOverEnabled { get; }

		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x0600052B RID: 1323 RVA: 0x0001EB8B File Offset: 0x0001CD8B
		public Func<bool> ShouldHighlightAgentWithoutFormation { get; }

		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x0600052C RID: 1324 RVA: 0x0001EB93 File Offset: 0x0001CD93
		public Action<Agent, int, uint?, bool, bool> SetAgentColor { get; }

		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x0600052D RID: 1325 RVA: 0x0001EB9B File Offset: 0x0001CD9B
		public Action<Agent, bool> ClearAgentHighlight { get; }

		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x0600052E RID: 1326 RVA: 0x0001EBA3 File Offset: 0x0001CDA3
		public Action<Agent> UpdateAgentColor { get; }

		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x0600052F RID: 1327 RVA: 0x0001EBAB File Offset: 0x0001CDAB
		public Action<Formation> ClearTargetOrSelectedFormationColor { get; }

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x06000530 RID: 1328 RVA: 0x0001EBB3 File Offset: 0x0001CDB3
		public Action<Formation> UpdateFormationColor { get; }

		// Token: 0x06000531 RID: 1329 RVA: 0x0001EBBC File Offset: 0x0001CDBC
		public FormationColorSubLogicV2(Func<bool> highlightEnabledInCharacterMode, Func<bool> highlightEnabledInRtsMode, Func<bool> mouseOverEnabled, Func<bool> shouldHighlightAgentWithoutFormation, Action<Agent, int, uint?, bool, bool> setAgentColor, Action<Agent, bool> clearAgentHighlight, Action<Agent> updateAgentColor, Action<Formation> clearTargetOrSelectedFormationColor, Action<Formation> updateFormationColor)
		{
			this.HighlightEnabledInCharacterMode = highlightEnabledInCharacterMode;
			this.HighlightEnabledInRtsMode = highlightEnabledInRtsMode;
			this.MouseOverEnabled = mouseOverEnabled;
			this.ShouldHighlightAgentWithoutFormation = shouldHighlightAgentWithoutFormation;
			this.SetAgentColor = setAgentColor;
			this.ClearAgentHighlight = clearAgentHighlight;
			this.UpdateAgentColor = updateAgentColor;
			this.ClearTargetOrSelectedFormationColor = clearTargetOrSelectedFormationColor;
			this.UpdateFormationColor = updateFormationColor;
		}

		// Token: 0x06000532 RID: 1330 RVA: 0x0001EC88 File Offset: 0x0001CE88
		public void OnBehaviourInitialize()
		{
			Mission.Current.Teams.OnPlayerTeamChanged += this.Mission_OnPlayerTeamChanged;
			Game.Current.EventManager.RegisterEvent<MissionPlayerToggledOrderViewEvent>(new Action<MissionPlayerToggledOrderViewEvent>(this.OnToggleOrderViewEvent));
			this._orderUiHandler = Mission.Current.GetMissionBehavior<MissionGauntletSingleplayerOrderUIHandler>();
			MissionEvent.ToggleFreeCamera += this.OnToggleFreeCamera;
		}

		// Token: 0x06000533 RID: 1331 RVA: 0x0001ECEC File Offset: 0x0001CEEC
		public void OnRemoveBehaviour()
		{
			this._actionQueue.Clear();
			this._agentsRemovedFromFormations.Clear();
			this._agentsWithEmptyFormations.Clear();
			Game.Current.EventManager.UnregisterEvent<MissionPlayerToggledOrderViewEvent>(new Action<MissionPlayerToggledOrderViewEvent>(this.OnToggleOrderViewEvent));
			Mission.Current.Teams.OnPlayerTeamChanged -= this.Mission_OnPlayerTeamChanged;
			MissionEvent.ToggleFreeCamera -= this.OnToggleFreeCamera;
		}

		// Token: 0x06000534 RID: 1332 RVA: 0x0001ED61 File Offset: 0x0001CF61
		public void OnShowIndicatorKeyDownUpdate(bool isShowIndicatorDown)
		{
			this._isShowIndicatorDown = isShowIndicatorDown;
			this.UpdateAllFormationColorTypes();
		}

		// Token: 0x06000535 RID: 1333 RVA: 0x0001ED70 File Offset: 0x0001CF70
		private void UpdateAllFormationColorTypes()
		{
			foreach (Team team in Mission.Current.Teams)
			{
				foreach (Formation formation in team.FormationsIncludingSpecialAndEmpty)
				{
					this.GetFormationColorStatus(formation).UpdateFormationColorType(this);
				}
			}
			if (this.ShouldHighlightAgentWithoutFormation())
			{
				this._colorStatusOfNoFormationAgents.UpdateFormationColorType(this);
			}
		}

		// Token: 0x06000536 RID: 1334 RVA: 0x0001EE20 File Offset: 0x0001D020
		private FormationColorSubLogicV2.FormationColorStatus GetFormationColorStatus(Formation formation)
		{
			FormationColorSubLogicV2.FormationColorStatus formationColorStatus;
			if (!this._formationColorStatusDictionary.TryGetValue(formation, out formationColorStatus))
			{
				formationColorStatus = new FormationColorSubLogicV2.FormationColorStatus();
				this._formationColorStatusDictionary.Add(formation, formationColorStatus);
			}
			return formationColorStatus;
		}

		// Token: 0x06000537 RID: 1335 RVA: 0x0001EE54 File Offset: 0x0001D054
		private void OnToggleFreeCamera(bool freeCamera)
		{
			this._isFreeCamera = freeCamera;
			foreach (Team team in Mission.Current.Teams)
			{
				foreach (Formation formation in team.FormationsIncludingSpecialAndEmpty)
				{
					this.GetFormationColorStatus(formation).UpdateFormationColorType(this);
				}
			}
			if (this.ShouldHighlightAgentWithoutFormation())
			{
				this._colorStatusOfNoFormationAgents.UpdateFormationColorType(this);
			}
		}

		// Token: 0x06000538 RID: 1336 RVA: 0x0001EF0C File Offset: 0x0001D10C
		public void OnPreDisplayMissionTick(float dt)
		{
			try
			{
				while (this._agentsRemovedFromFormations.Count > 0)
				{
					Agent agent5 = this._agentsRemovedFromFormations.Pop();
					if (agent5.Formation == null || !this.IsFormationDirty(agent5.Formation))
					{
						this.ClearAgentHighlight(agent5, true);
					}
				}
				while (this._agentsNewlyAddedToFormations.Count > 0)
				{
					Agent agent2 = this._agentsNewlyAddedToFormations.Pop();
					this.SetAgentColorAccordingToFormation(agent2, true);
				}
				foreach (KeyValuePair<Formation, FormationColorSubLogicV2.FormationColorStatus> keyValuePair in this._formationColorStatusDictionary.Where<KeyValuePair<Formation, FormationColorSubLogicV2.FormationColorStatus>>((KeyValuePair<Formation, FormationColorSubLogicV2.FormationColorStatus> pair) => pair.Value.IsDirty))
				{
					keyValuePair.Value.IsDirty = false;
					uint? color = keyValuePair.Value.GetFormationColorResultInt(this, FormationColorSubLogicV2.GetRoleType(keyValuePair.Key));
					keyValuePair.Key.ApplyActionOnEachUnit(delegate(Agent agent)
					{
						this.SetAgentColor(agent, 2, color, true, true);
					}, null);
				}
				if (this.ShouldHighlightAgentWithoutFormation())
				{
					if (!this._colorStatusOfNoFormationAgents.IsDirty)
					{
						goto IL_0249;
					}
					this._colorStatusOfNoFormationAgents.IsDirty = false;
					using (List<Agent>.Enumerator enumerator2 = this._agentsWithEmptyFormations.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							Agent agent3 = enumerator2.Current;
							uint? formationColorResultInt = this._colorStatusOfNoFormationAgents.GetFormationColorResultInt(this, FormationColorSubLogicV2.GetRoleType(agent3));
							if (!agent3.IsRunningAway)
							{
								CommonAIComponent commonAIComponent = agent3.CommonAIComponent;
								if (commonAIComponent != null)
								{
									bool isRetreating = commonAIComponent.IsRetreating;
								}
							}
							if (formationColorResultInt != null && agent3.IsRunningAway)
							{
								formationColorResultInt = new uint?(Vec3.Lerp(Color.FromUint(formationColorResultInt.Value).ToVec3(), Color.White.ToVec3(), 0.5f).ToARGB);
							}
							this.SetAgentColor(agent3, 2, formationColorResultInt, true, true);
						}
						goto IL_0249;
					}
				}
				foreach (Agent agent4 in this._agentsWithEmptyFormations)
				{
					this.ClearAgentHighlight(agent4, true);
				}
				this._agentsWithEmptyFormations.Clear();
				IL_0249:;
			}
			catch (Exception ex)
			{
				Utility.DisplayMessageForced(ex.ToString());
			}
		}

		// Token: 0x06000539 RID: 1337 RVA: 0x0001F1D4 File Offset: 0x0001D3D4
		private bool IsFormationDirty(Formation formation)
		{
			FormationColorSubLogicV2.FormationColorStatus formationColorStatus;
			return this._formationColorStatusDictionary.TryGetValue(formation, out formationColorStatus) && formationColorStatus.IsDirty;
		}

		// Token: 0x0600053A RID: 1338 RVA: 0x0001F1F9 File Offset: 0x0001D3F9
		public void AfterAddTeam(Team team)
		{
			team.OnOrderIssued += new OnOrderIssuedDelegate(this.OnOrderIssued);
			team.PlayerOrderController.OnSelectedFormationsChanged += this.OrderController_OnSelectedFormationsChanged;
		}

		// Token: 0x0600053B RID: 1339 RVA: 0x0001F224 File Offset: 0x0001D424
		public void OnUnitAdded(Formation formation, Agent agent)
		{
			if (this._agentsRemovedFromFormations.Count > 0 && this._agentsRemovedFromFormations.Peek() == agent)
			{
				this._agentsRemovedFromFormations.Pop();
			}
			this._agentsNewlyAddedToFormations.Push(agent);
			if (this.ShouldHighlightAgentWithoutFormation())
			{
				this._agentsWithEmptyFormations.Remove(agent);
			}
		}

		// Token: 0x0600053C RID: 1340 RVA: 0x0001F280 File Offset: 0x0001D480
		public void OnUnitRemoved(Formation formation, Agent agent)
		{
			if (agent.State != 1 || Mission.Current.IsMissionEnding || !Mission.Current.IsDeploymentFinished)
			{
				return;
			}
			if (this._agentsNewlyAddedToFormations.Count > 0 && this._agentsNewlyAddedToFormations.Peek() == agent)
			{
				this._agentsNewlyAddedToFormations.Pop();
			}
			this._agentsRemovedFromFormations.Push(agent);
			if (this.ShouldHighlightAgentWithoutFormation())
			{
				this._agentsWithEmptyFormations.Add(agent);
				this._colorStatusOfNoFormationAgents.IsDirty = true;
			}
		}

		// Token: 0x0600053D RID: 1341 RVA: 0x0001F308 File Offset: 0x0001D508
		public void OnAgentBuild(Agent agent, Banner banner)
		{
			if (!agent.IsHuman)
			{
				return;
			}
			if (agent.Formation == null)
			{
				if (this.ShouldHighlightAgentWithoutFormation())
				{
					this._agentsWithEmptyFormations.Add(agent);
					this._colorStatusOfNoFormationAgents.IsDirty = true;
				}
				this.ClearAgentHighlight(agent, true);
				return;
			}
			FormationColorSubLogicV2.FormationColorStatus formationColorStatus;
			if (this._formationColorStatusDictionary.TryGetValue(agent.Formation, out formationColorStatus))
			{
				if (formationColorStatus.IsDirty)
				{
					return;
				}
				uint? formationColorResultInt = formationColorStatus.GetFormationColorResultInt(this, FormationColorSubLogicV2.GetRoleType(agent.Formation));
				if (formationColorResultInt != null)
				{
					this.SetAgentColor(agent, 2, formationColorResultInt, true, true);
				}
			}
		}

		// Token: 0x0600053E RID: 1342 RVA: 0x0001F3A3 File Offset: 0x0001D5A3
		public void OnAgentFleeing(Agent affectedAgent)
		{
			if (!affectedAgent.IsHuman)
			{
				return;
			}
			if (affectedAgent.Formation == null)
			{
				this._colorStatusOfNoFormationAgents.IsDirty = true;
			}
		}

		// Token: 0x0600053F RID: 1343 RVA: 0x0001F3C2 File Offset: 0x0001D5C2
		public void OnAgentRemoved(Agent affectedAgent)
		{
			if (!affectedAgent.IsHuman)
			{
				return;
			}
			if (affectedAgent.Formation != null)
			{
				return;
			}
			if (this.ShouldHighlightAgentWithoutFormation())
			{
				this._agentsWithEmptyFormations.Remove(affectedAgent);
			}
		}

		// Token: 0x06000540 RID: 1344 RVA: 0x0001F3F0 File Offset: 0x0001D5F0
		private void SetAgentColorAccordingToFormation(Agent agent, bool updateInstantly)
		{
			if (agent.Formation == null)
			{
				this.ClearAgentHighlight(agent, updateInstantly);
				return;
			}
			FormationColorSubLogicV2.FormationColorStatus formationColorStatus;
			if (this._formationColorStatusDictionary.TryGetValue(agent.Formation, out formationColorStatus))
			{
				if (formationColorStatus.IsDirty)
				{
					return;
				}
				uint? formationColorResultInt = formationColorStatus.GetFormationColorResultInt(this, FormationColorSubLogicV2.GetRoleType(agent.Formation));
				this.SetAgentColor(agent, 2, formationColorResultInt, true, updateInstantly);
			}
		}

		// Token: 0x06000541 RID: 1345 RVA: 0x0001F454 File Offset: 0x0001D654
		public void MouseOver(Formation formation)
		{
			if (formation == this._mouseOverFormation)
			{
				return;
			}
			if (this._mouseOverFormation != null)
			{
				this.GetFormationColorStatus(this._mouseOverFormation).MouseOver(this, false);
			}
			this._mouseOverFormation = formation;
			if (formation != null)
			{
				this.GetFormationColorStatus(formation).MouseOver(this, true);
			}
		}

		// Token: 0x06000542 RID: 1346 RVA: 0x0001F493 File Offset: 0x0001D693
		public void OnMouseOverEnabledChanged(bool enable)
		{
			this.UpdateAllFormationColorTypes();
		}

		// Token: 0x06000543 RID: 1347 RVA: 0x0001F49B File Offset: 0x0001D69B
		private void OnToggleOrderViewEvent(MissionPlayerToggledOrderViewEvent e)
		{
			this._isOrderShown = e.IsOrderEnabled;
			this.UpdateAllFormationColorTypes();
		}

		// Token: 0x06000544 RID: 1348 RVA: 0x0001F4AF File Offset: 0x0001D6AF
		public void OnMovementOrderChanged(Formation formation)
		{
			this.RefreshTargetedFormations();
		}

		// Token: 0x06000545 RID: 1349 RVA: 0x0001F4B8 File Offset: 0x0001D6B8
		private void RefreshTargetedFormations()
		{
			OrderController playerOrderController = this.PlayerOrderController;
			List<Formation> list;
			if (playerOrderController == null)
			{
				list = null;
			}
			else
			{
				list = (from formation in playerOrderController.SelectedFormations
					select formation.TargetFormation into formation
					where formation != null
					select formation).ToList<Formation>();
			}
			List<Formation> list2 = list ?? new List<Formation>();
			if (Utility.IsTeamValid(Mission.Current.PlayerEnemyTeam))
			{
				list2.AddRange(from formation in Mission.Current.PlayerEnemyTeam.FormationsIncludingSpecialAndEmpty
					select formation.TargetFormation into formation
					where formation != null
					select formation);
			}
			foreach (Team team in Mission.Current.Teams)
			{
				foreach (Formation formation2 in team.FormationsIncludingSpecialAndEmpty)
				{
					this.GetFormationColorStatus(formation2).Target(this, list2.Contains(formation2));
				}
			}
		}

		// Token: 0x06000546 RID: 1350 RVA: 0x0001F62C File Offset: 0x0001D82C
		public void OnMovementOrderChanged(IEnumerable<Formation> appliedFormations)
		{
			this.RefreshTargetedFormations();
		}

		// Token: 0x06000547 RID: 1351 RVA: 0x0001F634 File Offset: 0x0001D834
		private void OnOrderIssued(OrderType orderType, MBReadOnlyList<Formation> appliedFormations, OrderController orderController, params object[] delegateParams)
		{
			if (Extensions.FindIndex<OrderType>(FormationColorSubLogicV2.movementOrderTypes, (OrderType o) => o == orderType) == -1)
			{
				return;
			}
			this.OnMovementOrderChanged(appliedFormations);
		}

		// Token: 0x06000548 RID: 1352 RVA: 0x0001F66F File Offset: 0x0001D86F
		private void OrderController_OnSelectedFormationsChanged()
		{
			this.OnSelectedFormationsChanged();
		}

		// Token: 0x06000549 RID: 1353 RVA: 0x0001F678 File Offset: 0x0001D878
		private void OnSelectedFormationsChanged()
		{
			OrderController playerOrderController = this.PlayerOrderController;
			List<Formation> list = ((playerOrderController != null) ? playerOrderController.SelectedFormations : null) ?? new List<Formation>();
			foreach (Team team in Mission.Current.Teams)
			{
				foreach (Formation formation in team.FormationsIncludingSpecialAndEmpty)
				{
					this.GetFormationColorStatus(formation).Select(this, list.Contains(formation));
				}
			}
			this.RefreshTargetedFormations();
		}

		// Token: 0x0600054A RID: 1354 RVA: 0x0001F738 File Offset: 0x0001D938
		private void Mission_OnPlayerTeamChanged(Team arg1, Team arg2)
		{
			this.OnSelectedFormationsChanged();
		}

		// Token: 0x0600054B RID: 1355 RVA: 0x0001F740 File Offset: 0x0001D940
		private static FormationColorSubLogicV2.FormationRoleType GetRoleType(Formation formation)
		{
			Team team = formation.Team;
			if (team == null)
			{
				return FormationColorSubLogicV2.FormationRoleType.Neutral;
			}
			if (team.IsPlayerTeam)
			{
				return FormationColorSubLogicV2.FormationRoleType.PlayerTeam;
			}
			if (team.IsPlayerAlly)
			{
				return FormationColorSubLogicV2.FormationRoleType.PlayerAllyTeam;
			}
			if (Utility.IsEnemy(formation))
			{
				return FormationColorSubLogicV2.FormationRoleType.EnemyTeam;
			}
			return FormationColorSubLogicV2.FormationRoleType.Neutral;
		}

		// Token: 0x0600054C RID: 1356 RVA: 0x0001F778 File Offset: 0x0001D978
		private static FormationColorSubLogicV2.FormationRoleType GetRoleType(Agent agent)
		{
			Team team = agent.Team;
			if (team == null)
			{
				if (Mission.Current.PlayerTeam == null || !Mission.Current.PlayerTeam.IsValid || Mission.Current.PlayerTeam.ActiveAgents.Count == 0)
				{
					return FormationColorSubLogicV2.FormationRoleType.Neutral;
				}
				if (agent.IsEnemyOf(Mission.Current.PlayerTeam.ActiveAgents[0]))
				{
					return FormationColorSubLogicV2.FormationRoleType.EnemyTeam;
				}
				if (agent.IsFriendOf(Mission.Current.PlayerTeam.ActiveAgents[0]))
				{
					return FormationColorSubLogicV2.FormationRoleType.PlayerAllyTeam;
				}
				return FormationColorSubLogicV2.FormationRoleType.Neutral;
			}
			else
			{
				if (team.IsPlayerTeam)
				{
					return FormationColorSubLogicV2.FormationRoleType.PlayerTeam;
				}
				if (team.IsPlayerAlly)
				{
					return FormationColorSubLogicV2.FormationRoleType.PlayerAllyTeam;
				}
				if (Utility.IsEnemy(agent))
				{
					return FormationColorSubLogicV2.FormationRoleType.EnemyTeam;
				}
				return FormationColorSubLogicV2.FormationRoleType.Neutral;
			}
		}

		// Token: 0x0600054D RID: 1357 RVA: 0x0001F823 File Offset: 0x0001DA23
		// Note: this type is marked as 'beforefieldinit'.
		static FormationColorSubLogicV2()
		{
			OrderType[] array = new OrderType[14];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.315D0F925F0BBB38D45296C5BD863215F8793235649558DFA237457920F344AE).FieldHandle);
			FormationColorSubLogicV2.movementOrderTypes = array;
		}

		// Token: 0x04000223 RID: 547
		private static readonly OrderType[] movementOrderTypes;

		// Token: 0x04000224 RID: 548
		public uint _invisibleGroundMarkerColor = new Color(0f, 0f, 0f, 0f).ToUnsignedInteger();

		// Token: 0x04000225 RID: 549
		private readonly Stack<Agent> _agentsNewlyAddedToFormations = new Stack<Agent>();

		// Token: 0x04000226 RID: 550
		private readonly Stack<Agent> _agentsRemovedFromFormations = new Stack<Agent>();

		// Token: 0x04000227 RID: 551
		private readonly List<Agent> _agentsWithEmptyFormations = new List<Agent>();

		// Token: 0x04000228 RID: 552
		private readonly Dictionary<Formation, FormationColorSubLogicV2.FormationColorStatus> _formationColorStatusDictionary = new Dictionary<Formation, FormationColorSubLogicV2.FormationColorStatus>();

		// Token: 0x04000229 RID: 553
		private readonly FormationColorSubLogicV2.FormationColorStatus _colorStatusOfNoFormationAgents = new FormationColorSubLogicV2.FormationColorStatus();

		// Token: 0x0400022A RID: 554
		private Formation _mouseOverFormation;

		// Token: 0x0400022B RID: 555
		private MissionGauntletSingleplayerOrderUIHandler _orderUiHandler;

		// Token: 0x0400022C RID: 556
		private readonly CommandSystemConfig _config = MissionConfigBase<CommandSystemConfig>.Get();

		// Token: 0x0400022D RID: 557
		private bool _isShowIndicatorDown;

		// Token: 0x0400022E RID: 558
		private bool _isOrderShown;

		// Token: 0x0400022F RID: 559
		private bool _isFreeCamera;

		// Token: 0x04000239 RID: 569
		private readonly Queue<Action> _actionQueue = new Queue<Action>();

		// Token: 0x020000DB RID: 219
		public enum FormationRoleType
		{
			// Token: 0x0400037F RID: 895
			PlayerTeam,
			// Token: 0x04000380 RID: 896
			PlayerAllyTeam,
			// Token: 0x04000381 RID: 897
			EnemyTeam,
			// Token: 0x04000382 RID: 898
			Neutral
		}

		// Token: 0x020000DC RID: 220
		public enum FormationColorType
		{
			// Token: 0x04000384 RID: 900
			Normal,
			// Token: 0x04000385 RID: 901
			Highlight,
			// Token: 0x04000386 RID: 902
			Targeted,
			// Token: 0x04000387 RID: 903
			MouseOver,
			// Token: 0x04000388 RID: 904
			MouseOverHighlight,
			// Token: 0x04000389 RID: 905
			MouseOverTargeted
		}

		// Token: 0x020000DD RID: 221
		public enum FormationColorWithTeam
		{
			// Token: 0x0400038B RID: 907
			Normal,
			// Token: 0x0400038C RID: 908
			PlayerTeamHighlight,
			// Token: 0x0400038D RID: 909
			PlayerTeamTargeted,
			// Token: 0x0400038E RID: 910
			PlayerTeamMouseOver,
			// Token: 0x0400038F RID: 911
			PlayerTeamMouseOverHighlight,
			// Token: 0x04000390 RID: 912
			PlayerTeamMouseOverTargeted,
			// Token: 0x04000391 RID: 913
			AllyTeamHighlight,
			// Token: 0x04000392 RID: 914
			AllyTeamTargeted,
			// Token: 0x04000393 RID: 915
			AllyTeamMouseOver,
			// Token: 0x04000394 RID: 916
			AllyTeamMouseOverHighlight,
			// Token: 0x04000395 RID: 917
			AllyTeamMouseOverTargeted,
			// Token: 0x04000396 RID: 918
			EnemyTeamHighlight,
			// Token: 0x04000397 RID: 919
			EnemyTeamTargeted,
			// Token: 0x04000398 RID: 920
			EnemyTeamMouseOver,
			// Token: 0x04000399 RID: 921
			EnemyTeamMouseOverHighlight,
			// Token: 0x0400039A RID: 922
			EnemyTeamMouseOverTargeted,
			// Token: 0x0400039B RID: 923
			NeutralHighlight,
			// Token: 0x0400039C RID: 924
			NeutralTargeted,
			// Token: 0x0400039D RID: 925
			NeutralMouseOver,
			// Token: 0x0400039E RID: 926
			NeutralMouseOverHighlight,
			// Token: 0x0400039F RID: 927
			NeutralMouseOverTargeted
		}

		// Token: 0x020000DE RID: 222
		public class FormationColorStatus
		{
			// Token: 0x170000D4 RID: 212
			// (get) Token: 0x0600069A RID: 1690 RVA: 0x00021FEE File Offset: 0x000201EE
			// (set) Token: 0x0600069B RID: 1691 RVA: 0x00021FF6 File Offset: 0x000201F6
			public bool IsSelected { get; set; }

			// Token: 0x0600069C RID: 1692 RVA: 0x00022000 File Offset: 0x00020200
			public void UpdateFormationColorType(FormationColorSubLogicV2 logic)
			{
				FormationColorSubLogicV2.FormationColorType formationColorType = this.GetFormationColorType(logic);
				this.IsDirty |= this._formationColorType != formationColorType;
				this._formationColorType = formationColorType;
			}

			// Token: 0x0600069D RID: 1693 RVA: 0x00022035 File Offset: 0x00020235
			public void Select(FormationColorSubLogicV2 logic, bool selected)
			{
				this.IsSelected = selected;
				this.UpdateFormationColorType(logic);
			}

			// Token: 0x0600069E RID: 1694 RVA: 0x00022045 File Offset: 0x00020245
			public void Target(FormationColorSubLogicV2 logic, bool targeted)
			{
				this.IsTargeted = targeted;
				this.UpdateFormationColorType(logic);
			}

			// Token: 0x0600069F RID: 1695 RVA: 0x00022055 File Offset: 0x00020255
			public void MouseOver(FormationColorSubLogicV2 logic, bool mouseOver)
			{
				this.IsMouseOver = mouseOver;
				this.UpdateFormationColorType(logic);
			}

			// Token: 0x060006A0 RID: 1696 RVA: 0x00022068 File Offset: 0x00020268
			public FormationColorSubLogicV2.FormationColorType GetFormationColorType(FormationColorSubLogicV2 logic)
			{
				bool flag = this.IsMouseOver && logic.ShouldMouseOverFormationWithShowingOrder;
				bool flag2 = (this.IsSelected && logic.ShouldHighlightWhenShowingOrder) || logic.ShouldHighlightWhenShowingIndicator;
				bool flag3 = this.IsTargeted && logic.ShouldHighlightWhenShowingOrder;
				if (flag2)
				{
					if (flag)
					{
						return FormationColorSubLogicV2.FormationColorType.MouseOverHighlight;
					}
					return FormationColorSubLogicV2.FormationColorType.Highlight;
				}
				else if (flag3)
				{
					if (flag)
					{
						return FormationColorSubLogicV2.FormationColorType.MouseOverTargeted;
					}
					return FormationColorSubLogicV2.FormationColorType.Targeted;
				}
				else
				{
					if (flag)
					{
						return FormationColorSubLogicV2.FormationColorType.MouseOver;
					}
					return FormationColorSubLogicV2.FormationColorType.Normal;
				}
			}

			// Token: 0x060006A1 RID: 1697 RVA: 0x000220CC File Offset: 0x000202CC
			public FormationColorSubLogicV2.FormationColorWithTeam GetFormationColorResult(FormationColorSubLogicV2 logic, FormationColorSubLogicV2.FormationRoleType roleType)
			{
				FormationColorSubLogicV2.FormationColorType formationColorType = this.GetFormationColorType(logic);
				if (roleType == FormationColorSubLogicV2.FormationRoleType.PlayerTeam)
				{
					if (formationColorType == FormationColorSubLogicV2.FormationColorType.Normal)
					{
						return FormationColorSubLogicV2.FormationColorWithTeam.Normal;
					}
					if (formationColorType == FormationColorSubLogicV2.FormationColorType.MouseOver)
					{
						return FormationColorSubLogicV2.FormationColorWithTeam.PlayerTeamMouseOver;
					}
					if (formationColorType == FormationColorSubLogicV2.FormationColorType.Highlight)
					{
						return FormationColorSubLogicV2.FormationColorWithTeam.PlayerTeamHighlight;
					}
					if (formationColorType == FormationColorSubLogicV2.FormationColorType.MouseOverHighlight)
					{
						return FormationColorSubLogicV2.FormationColorWithTeam.PlayerTeamMouseOverHighlight;
					}
					if (formationColorType == FormationColorSubLogicV2.FormationColorType.Targeted)
					{
						return FormationColorSubLogicV2.FormationColorWithTeam.PlayerTeamTargeted;
					}
					if (formationColorType == FormationColorSubLogicV2.FormationColorType.MouseOverTargeted)
					{
						return FormationColorSubLogicV2.FormationColorWithTeam.PlayerTeamMouseOverTargeted;
					}
				}
				else if (roleType == FormationColorSubLogicV2.FormationRoleType.PlayerAllyTeam)
				{
					if (formationColorType == FormationColorSubLogicV2.FormationColorType.Normal)
					{
						return FormationColorSubLogicV2.FormationColorWithTeam.Normal;
					}
					if (formationColorType == FormationColorSubLogicV2.FormationColorType.MouseOver)
					{
						return FormationColorSubLogicV2.FormationColorWithTeam.AllyTeamMouseOver;
					}
					if (formationColorType == FormationColorSubLogicV2.FormationColorType.Highlight)
					{
						return FormationColorSubLogicV2.FormationColorWithTeam.AllyTeamHighlight;
					}
					if (formationColorType == FormationColorSubLogicV2.FormationColorType.MouseOverHighlight)
					{
						return FormationColorSubLogicV2.FormationColorWithTeam.AllyTeamMouseOverHighlight;
					}
					if (formationColorType == FormationColorSubLogicV2.FormationColorType.Targeted)
					{
						return FormationColorSubLogicV2.FormationColorWithTeam.AllyTeamTargeted;
					}
					if (formationColorType == FormationColorSubLogicV2.FormationColorType.MouseOverTargeted)
					{
						return FormationColorSubLogicV2.FormationColorWithTeam.AllyTeamMouseOverTargeted;
					}
				}
				else if (roleType == FormationColorSubLogicV2.FormationRoleType.EnemyTeam)
				{
					if (formationColorType == FormationColorSubLogicV2.FormationColorType.Normal)
					{
						return FormationColorSubLogicV2.FormationColorWithTeam.Normal;
					}
					if (formationColorType == FormationColorSubLogicV2.FormationColorType.MouseOver)
					{
						return FormationColorSubLogicV2.FormationColorWithTeam.EnemyTeamMouseOver;
					}
					if (formationColorType == FormationColorSubLogicV2.FormationColorType.Highlight)
					{
						return FormationColorSubLogicV2.FormationColorWithTeam.EnemyTeamHighlight;
					}
					if (formationColorType == FormationColorSubLogicV2.FormationColorType.MouseOverHighlight)
					{
						return FormationColorSubLogicV2.FormationColorWithTeam.EnemyTeamMouseOverHighlight;
					}
					if (formationColorType == FormationColorSubLogicV2.FormationColorType.Targeted)
					{
						return FormationColorSubLogicV2.FormationColorWithTeam.EnemyTeamTargeted;
					}
					if (formationColorType == FormationColorSubLogicV2.FormationColorType.MouseOverTargeted)
					{
						return FormationColorSubLogicV2.FormationColorWithTeam.EnemyTeamMouseOverTargeted;
					}
				}
				else if (roleType == FormationColorSubLogicV2.FormationRoleType.Neutral)
				{
					if (formationColorType == FormationColorSubLogicV2.FormationColorType.Normal)
					{
						return FormationColorSubLogicV2.FormationColorWithTeam.Normal;
					}
					if (formationColorType == FormationColorSubLogicV2.FormationColorType.MouseOver)
					{
						return FormationColorSubLogicV2.FormationColorWithTeam.NeutralMouseOver;
					}
					if (formationColorType == FormationColorSubLogicV2.FormationColorType.Highlight)
					{
						return FormationColorSubLogicV2.FormationColorWithTeam.NeutralHighlight;
					}
					if (formationColorType == FormationColorSubLogicV2.FormationColorType.MouseOverHighlight)
					{
						return FormationColorSubLogicV2.FormationColorWithTeam.NeutralMouseOverHighlight;
					}
					if (formationColorType == FormationColorSubLogicV2.FormationColorType.Targeted)
					{
						return FormationColorSubLogicV2.FormationColorWithTeam.NeutralTargeted;
					}
					if (formationColorType == FormationColorSubLogicV2.FormationColorType.MouseOverTargeted)
					{
						return FormationColorSubLogicV2.FormationColorWithTeam.NeutralMouseOverTargeted;
					}
				}
				return FormationColorSubLogicV2.FormationColorWithTeam.Normal;
			}

			// Token: 0x060006A2 RID: 1698 RVA: 0x0002218C File Offset: 0x0002038C
			public uint? GetFormationColorResultInt(FormationColorSubLogicV2 logic, FormationColorSubLogicV2.FormationRoleType roleType)
			{
				switch (this.GetFormationColorResult(logic, roleType))
				{
				case FormationColorSubLogicV2.FormationColorWithTeam.Normal:
					return null;
				case FormationColorSubLogicV2.FormationColorWithTeam.PlayerTeamHighlight:
					return new uint?(new Color(0f, 0.6f, 1f, 1f).ToUnsignedInteger());
				case FormationColorSubLogicV2.FormationColorWithTeam.PlayerTeamTargeted:
					return new uint?(new Color(0.1f, 0.1f, 0.9f, 1f).ToUnsignedInteger());
				case FormationColorSubLogicV2.FormationColorWithTeam.PlayerTeamMouseOver:
				case FormationColorSubLogicV2.FormationColorWithTeam.PlayerTeamMouseOverTargeted:
					return new uint?(new Color(0.65f, 0.9f, 1f, 1f).ToUnsignedInteger());
				case FormationColorSubLogicV2.FormationColorWithTeam.PlayerTeamMouseOverHighlight:
					return new uint?(new Color(0.1f, 0.82f, 0.86f, 1f).ToUnsignedInteger());
				case FormationColorSubLogicV2.FormationColorWithTeam.AllyTeamHighlight:
					return new uint?(new Color(0.1f, 0.62f, 0.25f, 1f).ToUnsignedInteger());
				case FormationColorSubLogicV2.FormationColorWithTeam.AllyTeamTargeted:
					return new uint?(new Color(0.5f, 0.1f, 0.8f, 1f).ToUnsignedInteger());
				case FormationColorSubLogicV2.FormationColorWithTeam.AllyTeamMouseOver:
				case FormationColorSubLogicV2.FormationColorWithTeam.AllyTeamMouseOverTargeted:
					return new uint?(new Color(0.8f, 0.85f, 0.5f, 1f).ToUnsignedInteger());
				case FormationColorSubLogicV2.FormationColorWithTeam.AllyTeamMouseOverHighlight:
					return new uint?(new Color(0.5f, 1f, 0.6f, 1f).ToUnsignedInteger());
				case FormationColorSubLogicV2.FormationColorWithTeam.EnemyTeamHighlight:
				case FormationColorSubLogicV2.FormationColorWithTeam.EnemyTeamTargeted:
					return new uint?(new Color(0.62f, 0.09f, 0.05f, 1f).ToUnsignedInteger());
				case FormationColorSubLogicV2.FormationColorWithTeam.EnemyTeamMouseOver:
					return new uint?(new Color(0.8f, 0.54f, 0.45f, 1f).ToUnsignedInteger());
				case FormationColorSubLogicV2.FormationColorWithTeam.EnemyTeamMouseOverHighlight:
				case FormationColorSubLogicV2.FormationColorWithTeam.EnemyTeamMouseOverTargeted:
					return new uint?(new Color(0.89f, 0.4f, 0.1f, 1f).ToUnsignedInteger());
				case FormationColorSubLogicV2.FormationColorWithTeam.NeutralHighlight:
					return new uint?(new Color(0.5f, 0.5f, 0.5f, 1f).ToUnsignedInteger());
				case FormationColorSubLogicV2.FormationColorWithTeam.NeutralTargeted:
					return new uint?(new Color(0.3f, 0.3f, 0.9f, 1f).ToUnsignedInteger());
				case FormationColorSubLogicV2.FormationColorWithTeam.NeutralMouseOver:
				case FormationColorSubLogicV2.FormationColorWithTeam.NeutralMouseOverTargeted:
					return new uint?(new Color(0.9f, 0.9f, 0.9f, 1f).ToUnsignedInteger());
				case FormationColorSubLogicV2.FormationColorWithTeam.NeutralMouseOverHighlight:
					return new uint?(new Color(0.7f, 0.7f, 0.7f, 1f).ToUnsignedInteger());
				default:
					return null;
				}
			}

			// Token: 0x040003A1 RID: 929
			public bool IsTargeted;

			// Token: 0x040003A2 RID: 930
			public bool IsMouseOver;

			// Token: 0x040003A3 RID: 931
			public bool IsDirty;

			// Token: 0x040003A4 RID: 932
			private FormationColorSubLogicV2.FormationColorType _formationColorType;
		}
	}
}
