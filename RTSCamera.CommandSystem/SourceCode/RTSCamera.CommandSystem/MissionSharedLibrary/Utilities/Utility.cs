using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MissionSharedLibrary.HotKey;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.GauntletUI.Mission.Singleplayer;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order;

namespace MissionSharedLibrary.Utilities
{
	// Token: 0x02000008 RID: 8
	public static class Utility
	{
		// Token: 0x0600003F RID: 63 RVA: 0x00002757 File Offset: 0x00000957
		public static WorldPosition GetOrderPosition(Formation formation)
		{
			FieldInfo field = typeof(Formation).GetField("_orderPosition", BindingFlags.Instance | BindingFlags.NonPublic);
			return (WorldPosition)(((field != null) ? field.GetValue(formation) : null) ?? WorldPosition.Invalid);
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000040 RID: 64 RVA: 0x0000278F File Offset: 0x0000098F
		// (set) Token: 0x06000041 RID: 65 RVA: 0x00002796 File Offset: 0x00000996
		public static bool ShouldDisplayMessage { get; set; } = true;

		// Token: 0x06000042 RID: 66 RVA: 0x000027A0 File Offset: 0x000009A0
		public static void DisplayLocalizedText(string id, string variation = null)
		{
			try
			{
				if (Utility.ShouldDisplayMessage)
				{
					Utility.DisplayMessageImpl(GameTexts.FindText(id, variation).ToString());
				}
			}
			catch
			{
			}
		}

		// Token: 0x06000043 RID: 67 RVA: 0x000027DC File Offset: 0x000009DC
		public static void DisplayLocalizedText(string id, string variation, Color color)
		{
			try
			{
				if (Utility.ShouldDisplayMessage)
				{
					Utility.DisplayMessageImpl(GameTexts.FindText(id, variation).ToString(), color);
				}
			}
			catch
			{
			}
		}

		// Token: 0x06000044 RID: 68 RVA: 0x0000281C File Offset: 0x00000A1C
		public static void DisplayMessage(string msg)
		{
			try
			{
				if (Utility.ShouldDisplayMessage)
				{
					Utility.DisplayMessageImpl(new TextObject(msg, null).ToString());
				}
			}
			catch
			{
			}
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00002858 File Offset: 0x00000A58
		public static void DisplayMessage(string msg, Color color)
		{
			try
			{
				if (Utility.ShouldDisplayMessage)
				{
					Utility.DisplayMessageImpl(new TextObject(msg, null).ToString(), color);
				}
			}
			catch
			{
			}
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00002898 File Offset: 0x00000A98
		private static void DisplayMessageImpl(string str)
		{
			InformationManager.DisplayMessage(new InformationMessage(Utility.ModuleId + ": " + str));
		}

		// Token: 0x06000047 RID: 71 RVA: 0x000028B4 File Offset: 0x00000AB4
		private static void DisplayMessageImpl(string str, Color color)
		{
			InformationManager.DisplayMessage(new InformationMessage(Utility.ModuleId + ": " + str, color));
		}

		// Token: 0x06000048 RID: 72 RVA: 0x000028D4 File Offset: 0x00000AD4
		public static void PrintUsageHint()
		{
			string text = GeneralGameKeyCategory.GetKey(GeneralGameKey.OpenMenu).ToSequenceString();
			Utility.DisplayMessage(Module.CurrentModule.GlobalTextManager.FindText("str_mission_library_open_menu_hint", null).SetTextVariable("KeyName", text).ToString());
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00002917 File Offset: 0x00000B17
		public static void DisplayMessageForced(string text)
		{
			Utility.DisplayMessageImpl(text);
		}

		// Token: 0x0600004A RID: 74 RVA: 0x0000291F File Offset: 0x00000B1F
		public static TextObject TextForKey(InputKey key)
		{
			return Module.CurrentModule.GlobalTextManager.FindText("str_game_key_text", new Key(key).ToString().ToLower());
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00002945 File Offset: 0x00000B45
		public static bool IsAgentDead(Agent agent)
		{
			return agent == null || !agent.IsActive();
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00002955 File Offset: 0x00000B55
		public static bool IsPlayerDead()
		{
			return Utility.IsAgentDead(Mission.Current.MainAgent);
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00002966 File Offset: 0x00000B66
		public static bool IsTeamValid(Team team)
		{
			return team != null && team.IsValid;
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00002974 File Offset: 0x00000B74
		public static void SetPlayerAsCommander(bool forced = false)
		{
			Mission mission = Mission.Current;
			if (!Utility.IsTeamValid((mission != null) ? mission.PlayerTeam : null))
			{
				return;
			}
			mission.PlayerTeam.PlayerOrderController.Owner = mission.MainAgent;
			foreach (Formation formation in mission.PlayerTeam.FormationsIncludingEmpty)
			{
				if ((formation.PlayerOwner != null && formation.PlayerOwner != mission.MainAgent) || forced)
				{
					FieldInfo field = typeof(Formation).GetField("_playerOwner", BindingFlags.Instance | BindingFlags.NonPublic);
					if (field != null)
					{
						field.SetValue(formation, mission.MainAgent);
					}
				}
			}
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00002A40 File Offset: 0x00000C40
		public static void CancelPlayerAsCommander()
		{
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00002A44 File Offset: 0x00000C44
		public static void SetMainAgentFormation(Formation formation)
		{
			Mission mission = Mission.Current;
			Agent agent = ((mission != null) ? mission.MainAgent : null);
			if (agent == null)
			{
				return;
			}
			if (formation == null && agent.Formation != null && Utility.IsTeamValid(agent.Team))
			{
				DetachmentManager detachmentManager = agent.Team.DetachmentManager;
				if (detachmentManager != null)
				{
					detachmentManager.OnAgentRemoved(agent);
				}
			}
			agent.Formation = formation;
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00002AA0 File Offset: 0x00000CA0
		public static void SetPlayerFormationClass(FormationClass formationClass)
		{
			if (Mission.Current.IsNavalBattle || Mission.Current.IsNavalRaidBattle)
			{
				return;
			}
			if (formationClass < 0 || formationClass >= 10)
			{
				return;
			}
			Mission mission = Mission.Current;
			if (mission.MainAgent != null && Utility.IsTeamValid(mission.PlayerTeam))
			{
				Formation formation = mission.MainAgent.Formation;
				if (formation == null || formation.FormationIndex != formationClass)
				{
					Formation formation2 = mission.PlayerTeam.GetFormation(formationClass);
					if (formation2 == null)
					{
						return;
					}
					if (formation2.CountOfUnits == 0)
					{
						if (Mission.Current.PlayerTeam.IsPlayerGeneral && formation2.IsAIControlled && formation2.FormationIndex < 8)
						{
							formation2.SetControlledByAI(false, formation2.IsSplittableByAI);
						}
						if (formation2.FormationIndex == 8)
						{
							Formation formation3 = Mission.Current.PlayerTeam.GetFormation(8);
							if (formation3.AI.GetBehavior<BehaviorGeneral>() != null)
							{
								TacticComponent.SetDefaultBehaviorWeights(formation3);
								formation3.AI.SetBehaviorWeight<BehaviorGeneral>(1f);
								formation3.SetControlledByAI(true, false);
							}
						}
						else if (formation == null || formation.FormationIndex == 8)
						{
							formation2.SetMovementOrder(MovementOrder.MovementOrderMove(mission.MainAgent.GetWorldPosition()));
						}
						else
						{
							Utility.CopyOrdersFrom(formation2, formation);
						}
					}
					Utility.SetMainAgentFormation(formation2);
				}
			}
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00002BD8 File Offset: 0x00000DD8
		private unsafe static void CopyOrdersFrom(Formation formation, Formation target)
		{
			formation.SetMovementOrder(*target.GetReadonlyMovementOrderReference());
			formation.SetFormOrder(target.FormOrder, true);
			int? num = new int?(target.UnitSpacing);
			formation.SetPositioning(null, null, num);
			formation.SetRidingOrder(target.RidingOrder);
			formation.SetFiringOrder(target.FiringOrder);
			formation.SetControlledByAI(target.IsAIControlled || !target.Team.IsPlayerGeneral, false);
			if (target.AI.Side != 3)
			{
				formation.AI.Side = target.AI.Side;
			}
			formation.SetMovementOrder(*target.GetReadonlyMovementOrderReference());
			formation.SetTargetFormation(target.TargetFormation);
			formation.SetFacingOrder(target.FacingOrder);
			formation.SetArrangementOrder(target.ArrangementOrder);
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00002CBC File Offset: 0x00000EBC
		public static bool IsInPlayerParty(Agent agent)
		{
			if (Campaign.Current != null)
			{
				SimpleAgentOrigin simpleAgentOrigin = agent.Origin as SimpleAgentOrigin;
				if (simpleAgentOrigin != null)
				{
					if (simpleAgentOrigin.Party == null)
					{
						return true;
					}
					PartyBase party = simpleAgentOrigin.Party;
					MobileParty mainParty = Campaign.Current.MainParty;
					if (party == ((mainParty != null) ? mainParty.Party : null))
					{
						return true;
					}
				}
				PartyAgentOrigin partyAgentOrigin = agent.Origin as PartyAgentOrigin;
				if (partyAgentOrigin != null)
				{
					PartyBase party2 = partyAgentOrigin.Party;
					MobileParty mainParty2 = Campaign.Current.MainParty;
					if (party2 == ((mainParty2 != null) ? mainParty2.Party : null))
					{
						return true;
					}
				}
				PartyGroupAgentOrigin partyGroupAgentOrigin = agent.Origin as PartyGroupAgentOrigin;
				if (partyGroupAgentOrigin != null)
				{
					PartyBase party3 = partyGroupAgentOrigin.Party;
					MobileParty mainParty3 = Campaign.Current.MainParty;
					if (party3 == ((mainParty3 != null) ? mainParty3.Party : null))
					{
						return true;
					}
				}
				return false;
			}
			return agent.Team == Mission.Current.PlayerTeam;
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00002D78 File Offset: 0x00000F78
		public static bool? IsHigherInMemberRoster(Agent lhs, Agent rhs)
		{
			try
			{
				if (Campaign.Current != null)
				{
					bool flag = Utility.IsInPlayerParty(lhs);
					bool flag2 = Utility.IsInPlayerParty(rhs);
					if (flag && !flag2)
					{
						return new bool?(true);
					}
					if (!flag && flag2)
					{
						return new bool?(false);
					}
					if (!flag)
					{
						return null;
					}
					MobileParty mainParty = Campaign.Current.MainParty;
					bool flag3;
					if (mainParty == null)
					{
						flag3 = null != null;
					}
					else
					{
						PartyBase party = mainParty.Party;
						flag3 = ((party != null) ? party.MemberRoster : null) != null;
					}
					if (!flag3)
					{
						return null;
					}
					int num = Campaign.Current.MainParty.Party.MemberRoster.FindIndexOfTroop(lhs.Character as CharacterObject);
					int num2 = Campaign.Current.MainParty.Party.MemberRoster.FindIndexOfTroop(rhs.Character as CharacterObject);
					if (num == -1 && num2 == -1)
					{
						return null;
					}
					return new bool?(num < num2);
				}
			}
			catch (Exception ex)
			{
				Utility.DisplayMessage(ex.ToString());
			}
			return null;
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00002EA8 File Offset: 0x000010A8
		public static bool IsUsingNonPickableObject(Agent agent)
		{
			return agent.IsUsingGameObject && !(agent.CurrentlyUsedGameObject is SpawnedItemEntity);
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00002EC5 File Offset: 0x000010C5
		public static bool IsPickingUpObject(Agent agent)
		{
			return agent.IsUsingGameObject && agent.CurrentlyUsedGameObject is SpawnedItemEntity;
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00002EE0 File Offset: 0x000010E0
		public static void PlayerControlAgent(Agent agent)
		{
			if (agent == null)
			{
				return;
			}
			Mission mission = Mission.Current;
			if (mission != null && mission.IsFastForward)
			{
				Mission.Current.SetFastForwardingFromUI(false);
			}
			bool flag = AgentComponentExtensions.AIMoveToGameObjectIsEnabled(agent);
			bool flag2 = Utility.IsPickingUpObject(agent);
			if (flag)
			{
				AgentComponentExtensions.AIMoveToGameObjectDisable(agent);
			}
			if (flag || flag2)
			{
				agent.DisableScriptedMovement();
			}
			agent.Controller = 2;
			agent.AIStateFlags = 0;
			Agent mountAgent = agent.MountAgent;
			if (mountAgent != null)
			{
				mountAgent.SetMaximumSpeedLimit(-1f, false);
			}
			agent.SetMaximumSpeedLimit(-1f, false);
			if (agent.WalkMode)
			{
				agent.EventControlFlags |= 4096;
				agent.EventControlFlags &= -2049;
			}
			VictoryComponent component = agent.GetComponent<VictoryComponent>();
			if (component != null)
			{
				agent.RemoveComponent(component);
				agent.SetActionChannel(1, ref ActionIndexCache.act_none, true, 0L, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
				agent.ClearTargetFrame();
			}
			if (agent.Formation != null)
			{
				agent.Formation.OnUnitAddedOrRemoved();
			}
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00002FE8 File Offset: 0x000011E8
		public unsafe static void AIControlMainAgent(bool changeAlarmed, bool alarmed = false)
		{
			Mission mission = Mission.Current;
			if (((mission != null) ? mission.MainAgent : null) == null)
			{
				return;
			}
			try
			{
				MissionMainAgentController missionBehavior = mission.GetMissionBehavior<MissionMainAgentController>();
				if (missionBehavior != null)
				{
					missionBehavior.InteractionComponent.ClearFocus();
				}
				if (mission.MainAgent.Controller != 1)
				{
					if (Utility.IsUsingNonPickableObject(mission.MainAgent))
					{
						mission.MainAgent.HandleStopUsingAction();
					}
					mission.MainAgent.Controller = 1;
					BasicCharacterObject character = mission.MainAgent.Character;
					if (((character != null) ? character.Equipment[10].Item : null) == null)
					{
						BasicCharacterObject character2 = mission.MainAgent.Character;
						if (character2 == null || !character2.IsMounted)
						{
							if (!mission.MainAgent.HasMount)
							{
								mission.MainAgent.SetAgentFlags(mission.MainAgent.GetAgentFlags() & -8193);
								goto IL_00E9;
							}
							goto IL_00E9;
						}
					}
					mission.MainAgent.SetAgentFlags(mission.MainAgent.GetAgentFlags() | 8192);
					IL_00E9:
					try
					{
						CommonAIComponent commonAIComponent = mission.MainAgent.CommonAIComponent;
						if (commonAIComponent != null)
						{
							commonAIComponent.Initialize();
						}
						HumanAIComponent humanAIComponent = mission.MainAgent.HumanAIComponent;
						if (humanAIComponent != null)
						{
							humanAIComponent.Initialize();
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.ToString());
						Utility.DisplayMessage(ex.ToString());
					}
					if (mission.MainAgent.Formation != null)
					{
						mission.MainAgent.SetRidingOrder(mission.MainAgent.Formation.RidingOrder.OrderEnum);
						mission.MainAgent.Formation.OnUnitAddedOrRemoved();
					}
					if (changeAlarmed)
					{
						if (alarmed)
						{
							if ((mission.MainAgent.AIStateFlags & 3) == null)
							{
								Utility.SetMainAgentAlarmed(true);
							}
						}
						else
						{
							Utility.SetMainAgentAlarmed(false);
						}
					}
					if (mission.MainAgent.IsPaused)
					{
						mission.MainAgent.SetIsAIPaused(false);
					}
					mission.MainAgent.DisableScriptedMovement();
					Formation formation = mission.MainAgent.Formation;
					if (formation != null)
					{
						MovementOrder movementOrder = *formation.GetReadonlyMovementOrderReference();
						movementOrder.OnUnitJoinOrLeave(mission.MainAgent.Formation, mission.MainAgent, true);
					}
				}
			}
			catch (Exception ex2)
			{
				Utility.DisplayMessage(ex2.ToString());
			}
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00003220 File Offset: 0x00001420
		public static void SetMainAgentAlarmed(bool alarmed)
		{
			Agent mainAgent = Mission.Current.MainAgent;
			if (mainAgent == null)
			{
				return;
			}
			mainAgent.SetWatchState(alarmed ? 2 : 0);
		}

		// Token: 0x0600005A RID: 90 RVA: 0x0000323D File Offset: 0x0000143D
		public static bool IsEnemy(Agent agent)
		{
			Agent mainAgent = Mission.Current.MainAgent;
			if (mainAgent == null)
			{
				Team playerTeam = Mission.Current.PlayerTeam;
				return playerTeam != null && playerTeam.IsEnemyOf(agent.Team);
			}
			return mainAgent.IsEnemyOf(agent);
		}

		// Token: 0x0600005B RID: 91 RVA: 0x0000326F File Offset: 0x0000146F
		public static bool IsEnemy(Formation formation)
		{
			Team playerTeam = Mission.Current.PlayerTeam;
			return playerTeam != null && playerTeam.IsEnemyOf(formation.Team);
		}

		// Token: 0x0600005C RID: 92 RVA: 0x0000328C File Offset: 0x0000148C
		public static bool BeforeSetMainAgent(Agent agent)
		{
			if (Utility.ShouldSmoothMoveToAgent && Utility.GetMissionScreen().LastFollowedAgent != agent)
			{
				Utility.ShouldSmoothMoveToAgent = false;
				return true;
			}
			return false;
		}

		// Token: 0x0600005D RID: 93 RVA: 0x000032AB File Offset: 0x000014AB
		public static void AfterSetMainAgent(bool shouldSmoothMoveToAgent, MissionScreen missionScreen, bool rotateCamera = true)
		{
			if (shouldSmoothMoveToAgent)
			{
				Utility.ShouldSmoothMoveToAgent = true;
				Utility.SmoothMoveToAgent(missionScreen, false, rotateCamera, missionScreen.LastFollowedAgent != null);
				return;
			}
			Utility.SetIsPlayerAgentAdded(missionScreen, false);
		}

		// Token: 0x0600005E RID: 94 RVA: 0x000032D0 File Offset: 0x000014D0
		public static void SmoothMoveToAgent(MissionScreen missionScreen, bool forceMove = false, bool changeCameraRotation = true, bool wasLockToAgent = false)
		{
			try
			{
				Mission.SpectatorData spectatingData = missionScreen.GetSpectatingData(missionScreen.CombatCamera.Position);
				if (spectatingData.AgentToFollow != null)
				{
					FieldInfo cameraAddSpecialMovement = Utility.CameraAddSpecialMovement;
					if (cameraAddSpecialMovement != null)
					{
						cameraAddSpecialMovement.SetValue(missionScreen, true);
					}
					FieldInfo cameraApplySpecialMovementsInstantly = Utility.CameraApplySpecialMovementsInstantly;
					if (cameraApplySpecialMovementsInstantly != null)
					{
						cameraApplySpecialMovementsInstantly.SetValue(missionScreen, false);
					}
					if (missionScreen.LastFollowedAgent != spectatingData.AgentToFollow || forceMove)
					{
						float num = (changeCameraRotation ? 0f : (wasLockToAgent ? missionScreen.CameraElevation : (missionScreen.CameraElevation - (float)Utility.CameraAddedElevation.GetValue(missionScreen))));
						float num2 = (changeCameraRotation ? spectatingData.AgentToFollow.LookDirectionAsAngle : missionScreen.CameraBearing);
						MatrixFrame cameraFrameWhenLockedToAgent = Utility.GetCameraFrameWhenLockedToAgent(missionScreen, spectatingData.AgentToFollow, spectatingData.CameraType, num, num2);
						Utility.SmoothMoveToPositionAndDirection(missionScreen, cameraFrameWhenLockedToAgent.origin, num, spectatingData.AgentToFollow.LookDirectionAsAngle, changeCameraRotation, changeCameraRotation, wasLockToAgent);
					}
					Utility.SetLastFollowedAgent.Invoke(missionScreen, new object[] { spectatingData.AgentToFollow });
				}
				Utility.SetIsPlayerAgentAdded(missionScreen, false);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		// Token: 0x0600005F RID: 95 RVA: 0x000033F4 File Offset: 0x000015F4
		public static void SmoothMoveToPositionAndDirection(MissionScreen missionScreen, Vec3 position, float elevation, float bearing, bool changeElevation, bool changeBearing, bool wasLockToAgent = false)
		{
			try
			{
				FieldInfo cameraAddSpecialMovement = Utility.CameraAddSpecialMovement;
				if (cameraAddSpecialMovement != null)
				{
					cameraAddSpecialMovement.SetValue(missionScreen, true);
				}
				FieldInfo cameraApplySpecialMovementsInstantly = Utility.CameraApplySpecialMovementsInstantly;
				if (cameraApplySpecialMovementsInstantly != null)
				{
					cameraApplySpecialMovementsInstantly.SetValue(missionScreen, false);
				}
				MatrixFrame frame = missionScreen.CombatCamera.Frame;
				Vec3 vec = frame.rotation.s * (Mission.Current.CustomCameraLocalOffset.x + Mission.Current.CustomCameraLocalOffset2.x) + -frame.rotation.u * (Mission.Current.CustomCameraLocalOffset.y + Mission.Current.CustomCameraLocalOffset2.y) + frame.rotation.f * (Mission.Current.CustomCameraLocalOffset.z + Mission.Current.CustomCameraLocalOffset2.z);
				FieldInfo cameraSpecialCurrentPositionToAdd = Utility.CameraSpecialCurrentPositionToAdd;
				if (cameraSpecialCurrentPositionToAdd != null)
				{
					cameraSpecialCurrentPositionToAdd.SetValue(missionScreen, missionScreen.CombatCamera.Position - position - Mission.Current.CustomCameraTargetLocalOffset - vec);
				}
				float num = (float)Utility.CameraAddedElevation.GetValue(missionScreen);
				if (changeElevation)
				{
					FieldInfo cameraSpecialCurrentAddedElevation = Utility.CameraSpecialCurrentAddedElevation;
					if (cameraSpecialCurrentAddedElevation != null)
					{
						cameraSpecialCurrentAddedElevation.SetValue(missionScreen, missionScreen.CameraElevation - num - elevation);
					}
				}
				MethodInfo setCameraElevation = Utility.SetCameraElevation;
				if (setCameraElevation != null)
				{
					setCameraElevation.Invoke(missionScreen, new object[] { elevation });
				}
				if (changeBearing)
				{
					FieldInfo cameraSpecialCurrentAddedBearing = Utility.CameraSpecialCurrentAddedBearing;
					if (cameraSpecialCurrentAddedBearing != null)
					{
						cameraSpecialCurrentAddedBearing.SetValue(missionScreen, MBMath.WrapAngle(missionScreen.CameraBearing - bearing));
					}
					MethodInfo setCameraBearing = Utility.SetCameraBearing;
					if (setCameraBearing != null)
					{
						setCameraBearing.Invoke(missionScreen, new object[] { bearing });
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		// Token: 0x06000060 RID: 96 RVA: 0x000035D8 File Offset: 0x000017D8
		public static MatrixFrame GetCameraFrameWhenLockedToAgent(MissionScreen missionScreen, Agent agentToFollow, SpectatorCameraTypes cameraType, float virtualCameraElevation, float virtualCameraBearing)
		{
			MatrixFrame identity = MatrixFrame.Identity;
			float agentScale = agentToFollow.AgentScale;
			identity.rotation.RotateAboutSide(1.5707964f);
			identity.rotation.RotateAboutForward(virtualCameraBearing);
			identity.rotation.RotateAboutSide(virtualCameraElevation);
			if (missionScreen.IsPhotoModeEnabled)
			{
				float num = -missionScreen.Mission.Scene.GetPhotoModeRoll();
				identity.rotation.RotateAboutUp(num);
			}
			MatrixFrame matrixFrame = identity;
			if (!missionScreen.IsPhotoModeEnabled)
			{
				identity.rotation.RotateAboutSide((float)Utility.CameraAddedElevation.GetValue(missionScreen));
			}
			bool flag = agentToFollow.AgentVisuals != null && agentToFollow.AgentVisuals.GetSkeleton().GetCurrentRagdollState() > 0;
			Vec3 visualPosition = agentToFollow.VisualPosition;
			Vec3 vec = (flag ? agentToFollow.AgentVisuals.GetFrame().origin : visualPosition);
			if (agentToFollow.HasMount)
			{
				Vec2 vec2 = agentToFollow.MountAgent.GetMovementDirection() * agentToFollow.MountAgent.Monster.RiderBodyCapsuleForwardAdder;
				vec += vec2.ToVec3(0f);
				Monster monster = agentToFollow.MountAgent.Monster;
			}
			vec.z += (float)Utility.CameraTargetAddedHeight.GetValue(missionScreen);
			if (missionScreen.Mission.Mode != 1 && missionScreen.Mission.Mode != 5)
			{
				vec += matrixFrame.rotation.f * agentScale * (0.7f * MathF.Pow(MathF.Cos((float)(1.0 / (((double)missionScreen.CameraResultDistanceToTarget / (double)agentScale - 0.20000000298023224) * 30.0 + 20.0))), 3500f));
			}
			identity.origin = vec + matrixFrame.rotation.u * missionScreen.CameraResultDistanceToTarget;
			return identity;
		}

		// Token: 0x06000061 RID: 97 RVA: 0x000037C0 File Offset: 0x000019C0
		public static Vec3 GetCameraTargetPositionWhenLockedToAgent(MissionScreen missionScreen, Agent agentToFollow)
		{
			bool flag = agentToFollow.AgentVisuals != null && agentToFollow.AgentVisuals.GetSkeleton().GetCurrentRagdollState() > 0;
			Vec3 visualPosition = agentToFollow.VisualPosition;
			Vec3 vec = (flag ? agentToFollow.AgentVisuals.GetFrame().origin : visualPosition);
			if (agentToFollow.MountAgent != null)
			{
				Vec2 vec2 = agentToFollow.MountAgent.GetMovementDirection() * agentToFollow.MountAgent.Monster.RiderBodyCapsuleForwardAdder;
				vec += vec2.ToVec3(0f);
			}
			vec.z += (float)Utility.CameraTargetAddedHeight.GetValue(missionScreen);
			return vec;
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00003866 File Offset: 0x00001A66
		public static void SetIsPlayerAgentAdded(MissionScreen missionScreen, bool value)
		{
			FieldInfo isPlayerAgentAdded = Utility.IsPlayerAgentAdded;
			if (isPlayerAgentAdded != null)
			{
				isPlayerAgentAdded.SetValue(missionScreen, value);
			}
			if (value)
			{
				FieldInfo cameraSpecialCurrentPositionToAdd = Utility.CameraSpecialCurrentPositionToAdd;
				if (cameraSpecialCurrentPositionToAdd == null)
				{
					return;
				}
				cameraSpecialCurrentPositionToAdd.SetValue(missionScreen, Vec3.Zero);
			}
		}

		// Token: 0x06000063 RID: 99 RVA: 0x0000389C File Offset: 0x00001A9C
		public static void SetIsPlayerTroopInFormation(Formation formation, bool hasPlayer)
		{
			try
			{
				if (formation != null)
				{
					MethodInfo setIsPlayerTroopInFormationMethod = Utility.SetIsPlayerTroopInFormationMethod;
					if (setIsPlayerTroopInFormationMethod != null)
					{
						setIsPlayerTroopInFormationMethod.Invoke(formation, new object[] { hasPlayer });
					}
					formation.OnUnitAddedOrRemoved();
				}
			}
			catch (Exception ex)
			{
				Utility.DisplayMessage(ex.ToString());
			}
		}

		// Token: 0x06000064 RID: 100 RVA: 0x000038F4 File Offset: 0x00001AF4
		public static void SetHasPlayerControlledTroop(Formation formation, bool hasPlayer)
		{
			try
			{
				if (formation != null)
				{
					MethodInfo setHasPlayerControlledTroopMethod = Utility.SetHasPlayerControlledTroopMethod;
					if (setHasPlayerControlledTroopMethod != null)
					{
						setHasPlayerControlledTroopMethod.Invoke(formation, new object[] { hasPlayer });
					}
					formation.OnUnitAddedOrRemoved();
				}
			}
			catch (Exception ex)
			{
				Utility.DisplayMessage(ex.ToString());
			}
		}

		// Token: 0x06000065 RID: 101 RVA: 0x0000394C File Offset: 0x00001B4C
		public static void Reset(this GameKey gameKey)
		{
			Key controllerKey = gameKey.ControllerKey;
			if (controllerKey != null)
			{
				Key defaultControllerKey = gameKey.DefaultControllerKey;
				controllerKey.ChangeKey((defaultControllerKey != null) ? defaultControllerKey.InputKey : (-1));
			}
			Key keyboardKey = gameKey.KeyboardKey;
			if (keyboardKey != null)
			{
				Key defaultKeyboardKey = gameKey.DefaultKeyboardKey;
				keyboardKey.ChangeKey((defaultKeyboardKey != null) ? defaultKeyboardKey.InputKey : (-1));
			}
		}

		// Token: 0x06000066 RID: 102 RVA: 0x000039AC File Offset: 0x00001BAC
		public static bool CheckAllFormationArrangementIntegrity()
		{
			if (Mission.Current == null || Mission.Current.PlayerTeam == null)
			{
				return true;
			}
			bool flag = true;
			using (List<Team>.Enumerator enumerator = Mission.Current.Teams.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!Utility.CheckAllFormationInTeamArrangementIntegrity(enumerator.Current))
					{
						flag = false;
					}
				}
			}
			return flag;
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00003A1C File Offset: 0x00001C1C
		public static bool CheckAllFormationInTeamArrangementIntegrity(Team team)
		{
			bool flag = true;
			using (List<Formation>.Enumerator enumerator = team.FormationsIncludingEmpty.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!Utility.CheckFormationArrangementIntegrity(enumerator.Current))
					{
						flag = false;
					}
				}
			}
			return flag;
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00003A74 File Offset: 0x00001C74
		public static bool CheckFormationArrangementIntegrity(Formation formation)
		{
			LineFormation lineFormation = formation.Arrangement as LineFormation;
			if (lineFormation == null)
			{
				return true;
			}
			FieldInfo field = typeof(LineFormation).GetField("_units2D", BindingFlags.Instance | BindingFlags.NonPublic);
			MBList2D<IFormationUnit> mblist2D = (MBList2D<IFormationUnit>)((field != null) ? field.GetValue(lineFormation) : null);
			if (mblist2D == null)
			{
				return true;
			}
			for (int i = 0; i < mblist2D.Count1; i++)
			{
				for (int j = 0; j < mblist2D.Count2; j++)
				{
					IFormationUnit formationUnit = mblist2D[i, j];
					if (formationUnit != null && (formationUnit.FormationFileIndex != i || formationUnit.FormationRankIndex != j))
					{
						string text = "Formation integrity check failed: Agent {0} is in formation {1} has wrong file/rank index";
						object name = ((Agent)formationUnit).Name;
						Formation formation2 = ((Agent)formationUnit).Formation;
						Utility.DisplayMessage(string.Format(text, name, (formation2 != null) ? new FormationClass?(formation2.FormationIndex) : null));
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00003B50 File Offset: 0x00001D50
		public static MissionOrderVM GetMissionOrderVM(Mission mission)
		{
			MissionGauntletSingleplayerOrderUIHandler missionBehavior = mission.GetMissionBehavior<MissionGauntletSingleplayerOrderUIHandler>();
			if (missionBehavior != null)
			{
				FieldInfo field = typeof(MissionGauntletSingleplayerOrderUIHandler).GetField("_dataSource", BindingFlags.Instance | BindingFlags.NonPublic);
				return ((field != null) ? field.GetValue(missionBehavior) : null) as MissionOrderVM;
			}
			return null;
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00003B94 File Offset: 0x00001D94
		public static OrderItemVM FindOrderWithId(MissionOrderVM missionOrderVM, string orderId)
		{
			for (int i = 0; i < missionOrderVM.OrderSets.Count; i++)
			{
				OrderSetVM orderSetVM = missionOrderVM.OrderSets[i];
				for (int j = 0; j < orderSetVM.Orders.Count; j++)
				{
					OrderItemVM orderItemVM = orderSetVM.Orders[j];
					if (orderItemVM.Order.StringId == orderId)
					{
						return orderItemVM;
					}
				}
			}
			return null;
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00003BFD File Offset: 0x00001DFD
		public static MissionScreen GetMissionScreen()
		{
			return MissionState.Current.GetListenerOfType<MissionScreen>();
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00003C0C File Offset: 0x00001E0C
		public static MissionBehavior GetMissionBehaviorOfType(Mission mission, Type type)
		{
			for (int i = 0; i < mission.MissionBehaviors.Count; i++)
			{
				if (type.IsAssignableFrom(mission.MissionBehaviors[i].GetType()))
				{
					return mission.MissionBehaviors[i];
				}
			}
			return null;
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00003C58 File Offset: 0x00001E58
		public static bool IsModuleInstalled(string moduleId)
		{
			bool flag;
			try
			{
				flag = Utilities.GetModulesNames().Select<string, ModuleInfo>(new Func<string, ModuleInfo>(ModuleHelper.GetModuleInfo)).FirstOrDefault<ModuleInfo>((ModuleInfo info) => ((info != null) ? info.Id : null) == moduleId) != null;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				Utility.DisplayMessage(ex.ToString());
				MBDebug.Print(ex.ToString(), 0, 12, 17592186044416UL);
				flag = false;
			}
			return flag;
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00003CDC File Offset: 0x00001EDC
		public static bool IsHideoutBattle()
		{
			MissionState missionState = MissionState.Current;
			return ((missionState != null) ? missionState.MissionName : null) == "HideoutBattle";
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00003CF9 File Offset: 0x00001EF9
		public static bool IsHideoutAmbush()
		{
			MissionState missionState = MissionState.Current;
			return ((missionState != null) ? missionState.MissionName : null) == "HideoutAmbush";
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00003D16 File Offset: 0x00001F16
		public static MissionBehavior GetNavalShipsLogic(Mission mission)
		{
			return Utility.GetMissionBehaviorOfType(mission, AccessTools.TypeByName("NavalDLC.Missions.MissionLogics.NavalShipsLogic"));
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00003D28 File Offset: 0x00001F28
		public static int GetNumTeamShips(MissionBehavior navalShipsLogic, TeamSideEnum teamSide)
		{
			if (Utility._getNumTeamShips == null)
			{
				Utility._getNumTeamShips = AccessTools.Method("NavalDLC.Missions.MissionLogics.NavalShipsLogic:GetNumTeamShips", null, null);
			}
			return (int)Utility._getNumTeamShips.Invoke(navalShipsLogic, new object[] { teamSide });
		}

		// Token: 0x06000072 RID: 114 RVA: 0x00003D64 File Offset: 0x00001F64
		// Note: this type is marked as 'beforefieldinit'.
		static Utility()
		{
			PropertyInfo property = typeof(MissionScreen).GetProperty("LastFollowedAgent", BindingFlags.Instance | BindingFlags.Public);
			Utility.SetLastFollowedAgent = ((property != null) ? property.GetSetMethod(true) : null);
			Utility.CameraSpecialCurrentAddedElevation = typeof(MissionScreen).GetField("_cameraSpecialCurrentAddedElevation", BindingFlags.Instance | BindingFlags.NonPublic);
			Utility.CameraSpecialCurrentAddedBearing = typeof(MissionScreen).GetField("_cameraSpecialCurrentAddedBearing", BindingFlags.Instance | BindingFlags.NonPublic);
			Utility.CameraSpecialCurrentPositionToAdd = typeof(MissionScreen).GetField("_cameraSpecialCurrentPositionToAdd", BindingFlags.Instance | BindingFlags.NonPublic);
			Utility.CameraSpecialCurrentDistanceToAdd = typeof(MissionScreen).GetField("_cameraSpecialCurrentDistanceToAdd", BindingFlags.Instance | BindingFlags.NonPublic);
			PropertyInfo property2 = typeof(MissionScreen).GetProperty("CameraElevation", BindingFlags.Instance | BindingFlags.Public);
			Utility.SetCameraElevation = ((property2 != null) ? property2.GetSetMethod(true) : null);
			PropertyInfo property3 = typeof(MissionScreen).GetProperty("CameraBearing", BindingFlags.Instance | BindingFlags.Public);
			Utility.SetCameraBearing = ((property3 != null) ? property3.GetSetMethod(true) : null);
			Utility.IsPlayerAgentAdded = typeof(MissionScreen).GetField("_isPlayerAgentAdded", BindingFlags.Instance | BindingFlags.NonPublic);
			Utility.ShouldSmoothMoveToAgent = true;
			Utility.HasPlayerControlledTroop = typeof(Formation).GetProperty("HasPlayerControlledTroop", BindingFlags.Instance | BindingFlags.Public);
			Utility.IsPlayerTroopInFormation = typeof(Formation).GetProperty("IsPlayerTroopInFormation", BindingFlags.Instance | BindingFlags.Public);
			PropertyInfo hasPlayerControlledTroop = Utility.HasPlayerControlledTroop;
			Utility.SetHasPlayerControlledTroopMethod = ((hasPlayerControlledTroop != null) ? hasPlayerControlledTroop.GetSetMethod(true) : null);
			PropertyInfo isPlayerTroopInFormation = Utility.IsPlayerTroopInFormation;
			Utility.SetIsPlayerTroopInFormationMethod = ((isPlayerTroopInFormation != null) ? isPlayerTroopInFormation.GetSetMethod(true) : null);
		}

		// Token: 0x04000012 RID: 18
		public static string ModuleId;

		// Token: 0x04000014 RID: 20
		private static readonly FieldInfo CameraAddedElevation = typeof(MissionScreen).GetField("_cameraAddedElevation", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x04000015 RID: 21
		private static readonly FieldInfo CameraTargetAddedHeight = typeof(MissionScreen).GetField("_cameraTargetAddedHeight", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x04000016 RID: 22
		private static readonly FieldInfo CameraAddSpecialMovement = typeof(MissionScreen).GetField("_cameraAddSpecialMovement", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x04000017 RID: 23
		private static readonly FieldInfo CameraApplySpecialMovementsInstantly = typeof(MissionScreen).GetField("_cameraApplySpecialMovementsInstantly", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x04000018 RID: 24
		private static readonly MethodInfo SetLastFollowedAgent;

		// Token: 0x04000019 RID: 25
		private static readonly FieldInfo CameraSpecialCurrentAddedElevation;

		// Token: 0x0400001A RID: 26
		private static readonly FieldInfo CameraSpecialCurrentAddedBearing;

		// Token: 0x0400001B RID: 27
		private static readonly FieldInfo CameraSpecialCurrentPositionToAdd;

		// Token: 0x0400001C RID: 28
		private static readonly FieldInfo CameraSpecialCurrentDistanceToAdd;

		// Token: 0x0400001D RID: 29
		private static readonly MethodInfo SetCameraElevation;

		// Token: 0x0400001E RID: 30
		private static readonly MethodInfo SetCameraBearing;

		// Token: 0x0400001F RID: 31
		private static readonly FieldInfo IsPlayerAgentAdded;

		// Token: 0x04000020 RID: 32
		public static bool ShouldSmoothMoveToAgent;

		// Token: 0x04000021 RID: 33
		private static readonly PropertyInfo HasPlayerControlledTroop;

		// Token: 0x04000022 RID: 34
		private static readonly PropertyInfo IsPlayerTroopInFormation;

		// Token: 0x04000023 RID: 35
		private static readonly MethodInfo SetHasPlayerControlledTroopMethod;

		// Token: 0x04000024 RID: 36
		private static readonly MethodInfo SetIsPlayerTroopInFormationMethod;

		// Token: 0x04000025 RID: 37
		private static MethodInfo _getNumTeamShips;
	}
}
