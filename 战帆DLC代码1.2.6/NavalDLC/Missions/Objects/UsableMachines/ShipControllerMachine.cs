using System;
using System.Collections.Generic;
using NavalDLC.CharacterDevelopment;
using NavalDLC.Missions.AI.UsableMachineAIs;
using NavalDLC.Missions.MissionLogics;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace NavalDLC.Missions.Objects.UsableMachines
{
	// Token: 0x020000B5 RID: 181
	public class ShipControllerMachine : UsableMachine
	{
		// Token: 0x1700027F RID: 639
		// (get) Token: 0x06000DDD RID: 3549 RVA: 0x0006CB9E File Offset: 0x0006AD9E
		// (set) Token: 0x06000DDE RID: 3550 RVA: 0x0006CBA6 File Offset: 0x0006ADA6
		public GameEntity ControllerEntity { get; private set; }

		// Token: 0x17000280 RID: 640
		// (get) Token: 0x06000DDF RID: 3551 RVA: 0x0006CBAF File Offset: 0x0006ADAF
		// (set) Token: 0x06000DE0 RID: 3552 RVA: 0x0006CBB7 File Offset: 0x0006ADB7
		public MissionShip AttachedShip { get; private set; }

		// Token: 0x17000281 RID: 641
		// (get) Token: 0x06000DE1 RID: 3553 RVA: 0x0006CBC0 File Offset: 0x0006ADC0
		// (set) Token: 0x06000DE2 RID: 3554 RVA: 0x0006CBC8 File Offset: 0x0006ADC8
		public GameEntity HandTargetEntity { get; private set; }

		// Token: 0x17000282 RID: 642
		// (get) Token: 0x06000DE3 RID: 3555 RVA: 0x0006CBD1 File Offset: 0x0006ADD1
		public Vec3 BackCameraOffset
		{
			get
			{
				return this._cameraOffset;
			}
		}

		// Token: 0x17000283 RID: 643
		// (get) Token: 0x06000DE4 RID: 3556 RVA: 0x0006CBD9 File Offset: 0x0006ADD9
		public float CaptureTimer
		{
			get
			{
				return this._captureTimer;
			}
		}

		// Token: 0x17000284 RID: 644
		// (get) Token: 0x06000DE5 RID: 3557 RVA: 0x0006CBE1 File Offset: 0x0006ADE1
		public Vec3 ShoulderCameraOffset
		{
			get
			{
				return this._shoulderCameraOffset;
			}
		}

		// Token: 0x17000285 RID: 645
		// (get) Token: 0x06000DE6 RID: 3558 RVA: 0x0006CBE9 File Offset: 0x0006ADE9
		public Vec3 FrontCameraOffset
		{
			get
			{
				return this._frontCameraOffset;
			}
		}

		// Token: 0x17000286 RID: 646
		// (get) Token: 0x06000DE7 RID: 3559 RVA: 0x0006CBF1 File Offset: 0x0006ADF1
		public float ShoulderCameraDistance
		{
			get
			{
				return this._shoulderCameraDistance;
			}
		}

		// Token: 0x17000287 RID: 647
		// (get) Token: 0x06000DE8 RID: 3560 RVA: 0x0006CBF9 File Offset: 0x0006ADF9
		public float FrontCameraDistance
		{
			get
			{
				return this._frontCameraDistance;
			}
		}

		// Token: 0x17000288 RID: 648
		// (get) Token: 0x06000DE9 RID: 3561 RVA: 0x0006CC01 File Offset: 0x0006AE01
		public float BackCameraFovMultiplier
		{
			get
			{
				return this._cameraFovMultiplier;
			}
		}

		// Token: 0x17000289 RID: 649
		// (get) Token: 0x06000DEA RID: 3562 RVA: 0x0006CC09 File Offset: 0x0006AE09
		public float ShoulderCameraFovMultiplier
		{
			get
			{
				return this._shoulderCameraFovMultiplier;
			}
		}

		// Token: 0x1700028A RID: 650
		// (get) Token: 0x06000DEB RID: 3563 RVA: 0x0006CC11 File Offset: 0x0006AE11
		public float FrontCameraFovMultiplier
		{
			get
			{
				return this._frontCameraFovMultiplier;
			}
		}

		// Token: 0x1700028B RID: 651
		// (get) Token: 0x06000DEC RID: 3564 RVA: 0x0006CC19 File Offset: 0x0006AE19
		public Vec3 BackCameraTargetLocalPosition
		{
			get
			{
				GameEntity cameraTargetEntity = this._cameraTargetEntity;
				if (cameraTargetEntity == null)
				{
					return Vec3.Zero;
				}
				return cameraTargetEntity.GetFrame().origin;
			}
		}

		// Token: 0x1700028C RID: 652
		// (get) Token: 0x06000DED RID: 3565 RVA: 0x0006CC35 File Offset: 0x0006AE35
		public Vec3 ShoulderCameraTargetLocalPosition
		{
			get
			{
				GameEntity shoulderCameraTargetEntity = this._shoulderCameraTargetEntity;
				if (shoulderCameraTargetEntity == null)
				{
					return Vec3.Zero;
				}
				return shoulderCameraTargetEntity.GetFrame().origin;
			}
		}

		// Token: 0x1700028D RID: 653
		// (get) Token: 0x06000DEE RID: 3566 RVA: 0x0006CC51 File Offset: 0x0006AE51
		public Vec3 FrontCameraTargetLocalPosition
		{
			get
			{
				GameEntity frontCameraTargetEntity = this._frontCameraTargetEntity;
				if (frontCameraTargetEntity == null)
				{
					return Vec3.Zero;
				}
				return frontCameraTargetEntity.GetFrame().origin;
			}
		}

		// Token: 0x06000DEF RID: 3567 RVA: 0x0006CC6D File Offset: 0x0006AE6D
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return 2;
		}

		// Token: 0x06000DF0 RID: 3568 RVA: 0x0006CC70 File Offset: 0x0006AE70
		protected override void OnEditorTick(float dt)
		{
			if (!base.GameEntity.IsGhostObject())
			{
				this.UpdateVisualizer();
			}
		}

		// Token: 0x06000DF1 RID: 3569 RVA: 0x0006CC94 File Offset: 0x0006AE94
		protected override void OnInit()
		{
			base.OnInit();
			this.AttachedShip = base.GameEntity.GetFirstScriptOfTypeInFamily<MissionShip>();
			foreach (WeakGameEntity weakGameEntity in base.GameEntity.GetChildren())
			{
				if (weakGameEntity.Name == "controller")
				{
					this.ControllerEntity = GameEntity.CreateFromWeakEntity(weakGameEntity);
					this._rudderRotationEntity = this.ControllerEntity;
					this._rudderRotationEntityInitialLocalFrame = this._rudderRotationEntity.GetFrame();
					using (IEnumerator<WeakGameEntity> enumerator2 = weakGameEntity.GetChildren().GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							WeakGameEntity weakGameEntity2 = enumerator2.Current;
							if (weakGameEntity2.Name == "hand_position")
							{
								this.HandTargetEntity = GameEntity.CreateFromWeakEntity(weakGameEntity2);
							}
						}
						continue;
					}
				}
				if (weakGameEntity.Name == "hand_position")
				{
					this.HandTargetEntity = GameEntity.CreateFromWeakEntity(weakGameEntity);
				}
				else if (weakGameEntity.Name == "camera_target")
				{
					this._cameraTargetEntity = GameEntity.CreateFromWeakEntity(weakGameEntity);
				}
				else if (weakGameEntity.Name == "shoulder_camera_target")
				{
					this._shoulderCameraTargetEntity = GameEntity.CreateFromWeakEntity(weakGameEntity);
				}
				else if (weakGameEntity.Name == "front_camera_target")
				{
					this._frontCameraTargetEntity = GameEntity.CreateFromWeakEntity(weakGameEntity);
				}
			}
			if (this._rudderRotationEntity == null)
			{
				List<WeakGameEntity> list = new List<WeakGameEntity>();
				base.GameEntity.Root.GetChildrenWithTagRecursive(list, "rudder_rotation_entity");
				foreach (WeakGameEntity weakGameEntity3 in list)
				{
					this._rudderRotationEntity = GameEntity.CreateFromWeakEntity(weakGameEntity3);
					this._rudderRotationEntityInitialLocalFrame = this._rudderRotationEntity.GetFrame();
				}
			}
			this._shipControlActionPushLeftIndex = ActionIndexCache.Create(this._shipControlActionTurnLeft);
			this._shipControlActionPullRightIndex = ActionIndexCache.Create(this._shipControlActionTurnRight);
			this._shipControlActionRelaxedIndex = ActionIndexCache.Create(this._shipControlActionRelaxed);
			this._shipCaptureActionIndex = ActionIndexCache.Create(this._shipCaptureAction);
			base.SetScriptComponentToTick(this.GetTickRequirement());
			this.EnemyRangeToStopUsing = 5f;
		}

		// Token: 0x06000DF2 RID: 3570 RVA: 0x0006CF24 File Offset: 0x0006B124
		public bool CheckControllerMachineFlags(bool editMode)
		{
			List<WeakGameEntity> list = new List<WeakGameEntity>();
			base.GameEntity.GetChildrenRecursive(ref list);
			bool flag = false;
			list.Add(base.GameEntity);
			foreach (WeakGameEntity weakGameEntity in list)
			{
				if (!Extensions.HasAnyFlag<EntityFlags>(weakGameEntity.EntityFlags, 131072) && !Extensions.HasAnyFlag<EntityFlags>(weakGameEntity.EntityFlags, 4096))
				{
					flag = true;
				}
			}
			if (flag)
			{
				string text = string.Format("In Root Entity {0}, {1}'s every descendant including itself must have Does not Affect Parent's Local Bounding Box flag.", base.GameEntity.Root.Name, base.GameEntity.Name);
				if (editMode)
				{
					MBEditor.AddEntityWarning(base.GameEntity, text);
				}
			}
			return flag;
		}

		// Token: 0x06000DF3 RID: 3571 RVA: 0x0006CFFC File Offset: 0x0006B1FC
		public override void OnDeploymentFinished()
		{
			this.EnsureStandingPointComponents();
			if (this.AttachedShip.BattleSide != Mission.Current.PlayerTeam.Side)
			{
				base.PilotStandingPoint.SetUsableByAIOnly();
			}
			this._navalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			this._navalAgentsLogic = Mission.Current.GetMissionBehavior<NavalAgentsLogic>();
		}

		// Token: 0x06000DF4 RID: 3572 RVA: 0x0006D058 File Offset: 0x0006B258
		private void EnsureStandingPointComponents()
		{
			if (base.PilotStandingPoint.GetComponent<ResetAnimationOnStopUsageComponent>() == null)
			{
				base.PilotStandingPoint.AddComponent(new ResetAnimationOnStopUsageComponent(ActionIndexCache.act_none, false));
				base.PilotStandingPoint.AddComponent(new ClearHandInverseKinematicsOnStopUsageComponent());
				base.PilotStandingPoint.AddComponent((NavalDLCManager.Instance.NavalPerks != null) ? new UserDamageCalculateComponent(NavalPerks.Shipmaster.TheHelmsmansShield, true, -0.6f) : new UserDamageCalculateComponent(null, false, -0.6f));
			}
		}

		// Token: 0x06000DF5 RID: 3573 RVA: 0x0006D0D0 File Offset: 0x0006B2D0
		public override void OnPilotAssignedDuringSpawn()
		{
			this.EnsureStandingPointComponents();
			bool flag = MBAnimation.GetAnimationBlendsWithActionIndex(MBActionSet.GetAnimationIndexOfAction(base.PilotAgent.ActionSet, ref this._shipControlActionRelaxedIndex)).Index >= 0;
			base.PilotAgent.SetActionChannel(1, ref this._shipControlActionRelaxedIndex, false, 71L, flag ? 0.5f : 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
			MatrixFrame globalFrame = base.PilotStandingPoint.GameEntity.GetGlobalFrame();
			base.PilotAgent.TeleportToPosition(globalFrame.origin);
			base.PilotAgent.DisableScriptedMovement();
			Agent pilotAgent = base.PilotAgent;
			Vec2 vec = globalFrame.rotation.f.AsVec2;
			vec = vec.Normalized();
			pilotAgent.SetMovementDirection(ref vec);
		}

		// Token: 0x06000DF6 RID: 3574 RVA: 0x0006D1A8 File Offset: 0x0006B3A8
		protected override void OnTick(float dt)
		{
			base.OnTick(dt);
			if (this._rudderRotationEntity != null)
			{
				MatrixFrame rudderRotationEntityInitialLocalFrame = this._rudderRotationEntityInitialLocalFrame;
				rudderRotationEntityInitialLocalFrame.rotation.RotateAboutUp(this.AttachedShip.VisualRudderRotation);
				this._rudderRotationEntity.SetLocalFrame(ref rudderRotationEntityInitialLocalFrame, false);
			}
			if (this._navalShipsLogic != null)
			{
				Agent main = Agent.Main;
				bool flag;
				if (main == null)
				{
					flag = null != null;
				}
				else
				{
					Formation formation = main.Formation;
					flag = ((formation != null) ? formation.Team : null) != null;
				}
				if (flag && this.AttachedShip.BattleSide != Agent.Main.Formation.Team.Side)
				{
					base.PilotStandingPoint.IsDisabledForPlayers = !this.AttachedShip.CanBeTakenOver || !this.IsAttachedShipVacant() || !MissionShip.AreShipsConnected(this._navalShipsLogic.GetShipAssignment(Agent.Main.Formation.Team.TeamSide, Agent.Main.Formation.FormationIndex).MissionShip, this.AttachedShip);
				}
			}
			if (base.PilotAgent == null)
			{
				this._captureTimer = -1f;
			}
			if (base.PilotAgent != null)
			{
				if (base.PilotAgent.IsMainAgent && this.IsAttachedShipVacant() && base.PilotAgent.Formation != null)
				{
					MissionShip missionShip = this._navalShipsLogic.GetShipAssignment(base.PilotAgent.Formation.Team.TeamSide, base.PilotAgent.Formation.FormationIndex).MissionShip;
					if (!MissionShip.AreShipsConnected(missionShip, this.AttachedShip))
					{
						this._captureTimer = -1f;
						base.PilotAgent.StopUsingGameObject(true, 1);
						return;
					}
					if (base.PilotAgent.SetActionChannel(0, ref this._shipCaptureActionIndex, false, 0L, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true))
					{
						if (this._captureTimer <= 0f)
						{
							this._captureTimer = 3f;
							return;
						}
						this._captureTimer -= dt;
						if (this._captureTimer <= 0f)
						{
							Agent pilotAgent = base.PilotAgent;
							base.PilotAgent.StopUsingGameObject(true, 1);
							this.OnShipCapturedByAgent(pilotAgent);
							missionShip.InvalidateActiveFormationTroopOnShipCache();
							this.AttachedShip.InvalidateActiveFormationTroopOnShipCache();
							return;
						}
					}
				}
				else
				{
					float num = this.AttachedShip.VisualRudderRotationPercentage * (float)MathF.Sign(base.GameEntity.GetGlobalScale().x);
					num = MBMath.Map(num, -1f, 1f, 0.95f, 0.05f);
					ActionIndexCache actionIndexCache;
					if (this.AttachedShip.VisualRudderPullDirection == 0f)
					{
						actionIndexCache = this._shipControlActionRelaxedIndex;
					}
					else if (this.AttachedShip.VisualRudderPullDirection > 0f)
					{
						actionIndexCache = this._shipControlActionPullRightIndex;
					}
					else
					{
						actionIndexCache = this._shipControlActionPushLeftIndex;
					}
					int animationIndexOfAction = MBActionSet.GetAnimationIndexOfAction(base.PilotAgent.ActionSet, ref actionIndexCache);
					bool flag2 = MBAnimation.GetAnimationBlendsWithActionIndex(animationIndexOfAction) != ActionIndexCache.act_none;
					AnimFlags animFlags = 17592202822215L;
					if (base.PilotAgent.SetActionChannel(1, ref actionIndexCache, false, animFlags, flag2 ? num : 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true))
					{
						if (this.HandTargetEntity != null)
						{
							Vec3 origin = this.HandTargetEntity.GetGlobalFrame().origin;
							float currentActionProgress = base.PilotAgent.GetCurrentActionProgress(1);
							MatrixFrame frame = base.PilotAgent.Frame;
							MBAgentVisuals agentVisuals = base.PilotAgent.AgentVisuals;
							MatrixFrame boneEntitialFrame = agentVisuals.GetBoneEntitialFrame(base.PilotAgent.Monster.MainHandBoneIndex, false);
							MatrixFrame boneEntitialFrame2 = agentVisuals.GetBoneEntitialFrame(base.PilotAgent.Monster.OffHandBoneIndex, false);
							MatrixFrame boneEntitialFrameAtAnimationProgress = base.PilotAgent.GetBoneEntitialFrameAtAnimationProgress(base.PilotAgent.Monster.MainHandBoneIndex, animationIndexOfAction, currentActionProgress);
							MatrixFrame boneEntitialFrameAtAnimationProgress2 = base.PilotAgent.GetBoneEntitialFrameAtAnimationProgress(base.PilotAgent.Monster.OffHandBoneIndex, animationIndexOfAction, currentActionProgress);
							Vec3 vec = frame.TransformToParent(ref boneEntitialFrameAtAnimationProgress.origin);
							Vec3 vec2 = frame.TransformToParent(ref boneEntitialFrameAtAnimationProgress2.origin);
							float num2 = MathF.Clamp(dt * 15f, 0f, 1f);
							MatrixFrame matrixFrame;
							matrixFrame.origin = boneEntitialFrameAtAnimationProgress.origin;
							matrixFrame.rotation = Mat3.SlerpFPSIndependent(ref boneEntitialFrame.rotation, ref boneEntitialFrameAtAnimationProgress.rotation, num2);
							MatrixFrame matrixFrame2;
							matrixFrame2.origin = boneEntitialFrameAtAnimationProgress2.origin;
							matrixFrame2.rotation = Mat3.SlerpFPSIndependent(ref boneEntitialFrame2.rotation, ref boneEntitialFrameAtAnimationProgress2.rotation, num2);
							MatrixFrame matrixFrame3 = frame.TransformToParent(ref matrixFrame);
							MatrixFrame matrixFrame4 = frame.TransformToParent(ref matrixFrame2);
							if (this._isLeftHandOnly)
							{
								matrixFrame4.origin = origin;
								Agent pilotAgent2 = base.PilotAgent;
								MatrixFrame matrixFrame5 = MatrixFrame.Identity;
								pilotAgent2.SetHandInverseKinematicsFrame(ref matrixFrame4, ref matrixFrame5);
								return;
							}
							if (this._isRightHandOnly)
							{
								matrixFrame3.origin = origin;
								Agent pilotAgent3 = base.PilotAgent;
								MatrixFrame matrixFrame5 = MatrixFrame.Identity;
								pilotAgent3.SetHandInverseKinematicsFrame(ref matrixFrame5, ref matrixFrame3);
								return;
							}
							Vec3 vec3;
							if (!(this.ControllerEntity != null))
							{
								MatrixFrame matrixFrame5 = base.PilotStandingPoint.GameEntity.GetGlobalFrame();
								vec3 = matrixFrame5.rotation.s.NormalizedCopy();
							}
							else
							{
								MatrixFrame matrixFrame5 = this.ControllerEntity.GetGlobalFrame();
								vec3 = matrixFrame5.rotation.s.NormalizedCopy();
							}
							Vec3 vec4 = vec3;
							float num3 = Vec3.DotProduct(vec4, vec - vec2);
							matrixFrame3.origin = origin + 0.5f * num3 * vec4;
							matrixFrame4.origin = origin - 0.5f * num3 * vec4;
							base.PilotAgent.SetHandInverseKinematicsFrame(ref matrixFrame4, ref matrixFrame3);
							return;
						}
					}
					else
					{
						if (base.PilotAgent.IsInBeingStruckAction)
						{
							base.PilotAgent.ClearHandInverseKinematics();
							return;
						}
						base.PilotAgent.StopUsingGameObject(true, 1);
					}
				}
			}
		}

		// Token: 0x06000DF7 RID: 3575 RVA: 0x0006D76C File Offset: 0x0006B96C
		private void OnShipCapturedByAgent(Agent captorAgent)
		{
			NavalShipsLogic navalShipsLogic = this._navalShipsLogic;
			if (navalShipsLogic == null)
			{
				return;
			}
			navalShipsLogic.OnShipCaptured(this.AttachedShip, captorAgent.Formation);
		}

		// Token: 0x06000DF8 RID: 3576 RVA: 0x0006D78A File Offset: 0x0006B98A
		public override TextObject GetActionTextForStandingPoint(UsableMissionObject usableGameObject)
		{
			TextObject textObject = new TextObject("{=!}{KEY}", null);
			textObject.SetTextVariable("KEY", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13), 1f));
			return textObject;
		}

		// Token: 0x06000DF9 RID: 3577 RVA: 0x0006D7B9 File Offset: 0x0006B9B9
		protected override float GetDetachmentWeightAux(BattleSideEnum side)
		{
			return float.MinValue;
		}

		// Token: 0x06000DFA RID: 3578 RVA: 0x0006D7C0 File Offset: 0x0006B9C0
		public override TextObject GetDescriptionText(WeakGameEntity gameEntity)
		{
			if (this.AttachedShip.BattleSide == Mission.Current.PlayerTeam.Side)
			{
				return new TextObject("{=OGY9BKOM}Control the Ship", null);
			}
			if (!this.AttachedShip.CanBeTakenOver)
			{
				return null;
			}
			if (!this.IsAttachedShipVacant())
			{
				return new TextObject("{=UrBktTYi}Clear the crew to capture the ship", null);
			}
			MissionShip missionShip = null;
			if (this._navalShipsLogic != null)
			{
				Formation formation = Agent.Main.Formation;
				if (((formation != null) ? formation.Team : null) != null)
				{
					ShipAssignment shipAssignment = this._navalShipsLogic.GetShipAssignment(Agent.Main.Formation.Team.TeamSide, Agent.Main.Formation.FormationIndex);
					missionShip = ((shipAssignment != null) ? shipAssignment.MissionShip : null);
				}
			}
			if (missionShip != null && MissionShip.AreShipsConnected(missionShip, this.AttachedShip))
			{
				return new TextObject("{=fOX1aVDv}Capture the ship", null);
			}
			if (!(this._overridenDescriptionForActiveEnemyShipControllerMachine != null))
			{
				return new TextObject("{=lS53LgyN}You need to be boarded to capture the ship", null);
			}
			return this._overridenDescriptionForActiveEnemyShipControllerMachine;
		}

		// Token: 0x06000DFB RID: 3579 RVA: 0x0006D8B8 File Offset: 0x0006BAB8
		public override UsableMachineAIBase CreateAIBehaviorObject()
		{
			return new ShipControllerMachineAI(this);
		}

		// Token: 0x06000DFC RID: 3580 RVA: 0x0006D8C0 File Offset: 0x0006BAC0
		private void UpdateVisualizer()
		{
			WeakGameEntity weakGameEntity = base.GameEntity.GetFirstChildEntityWithTag("visualizer");
			StandingPoint firstScriptOfTypeRecursive = base.GameEntity.GetFirstScriptOfTypeRecursive<StandingPoint>();
			bool flag = false;
			if (this._shipControlActionRelaxedIndex == ActionIndexCache.act_none || this._shipControlActionRelaxedIndex.GetName() != this._shipControlActionRelaxed)
			{
				this._shipControlActionRelaxedIndex = ActionIndexCache.Create(this._shipControlActionRelaxed);
				if (this._shipControlActionRelaxedIndex != ActionIndexCache.act_none)
				{
					flag = MBAnimation.GetAnimationBlendsWithActionIndex(MBActionSet.GetAnimationIndexOfAction(MBActionSet.GetActionSetWithIndex(0), ref this._shipControlActionRelaxedIndex)) != ActionIndexCache.act_none;
				}
			}
			if (this._shipControlActionRelaxedIndex != ActionIndexCache.act_none && firstScriptOfTypeRecursive != null)
			{
				WeakGameEntity gameEntity = firstScriptOfTypeRecursive.GameEntity;
				if (!weakGameEntity.IsValid)
				{
					GameEntity gameEntity2 = GameEntity.CreateEmpty(base.GameEntity.Scene, false, true, true);
					weakGameEntity = gameEntity2.WeakEntity;
					weakGameEntity.SetEntityFlags(weakGameEntity.EntityFlags | 131072);
					weakGameEntity.SetName("visualizer");
					weakGameEntity.AddTag("visualizer");
					MBActionSet actionSetWithIndex = MBActionSet.GetActionSetWithIndex(0);
					GameEntityExtensions.CreateAgentSkeleton(weakGameEntity, "human_skeleton", true, actionSetWithIndex, "human", MBObjectManager.Instance.GetObject<Monster>("human"));
					MBSkeletonExtensions.SetAgentActionChannel(weakGameEntity.Skeleton, 0, ref this._shipControlActionRelaxedIndex, 0f, 0f, true, flag ? 0.5f : 0f);
					weakGameEntity.AddMultiMeshToSkeleton(MetaMesh.GetCopy("roman_cloth_tunic_a", true, false));
					weakGameEntity.AddMultiMeshToSkeleton(MetaMesh.GetCopy("casual_02_boots", true, false));
					weakGameEntity.AddMultiMeshToSkeleton(MetaMesh.GetCopy("hands_male_a", true, false));
					weakGameEntity.AddMultiMeshToSkeleton(MetaMesh.GetCopy("head_male_a", true, false));
					base.GameEntity.AddChild(gameEntity2.WeakEntity, false);
				}
			}
			if (weakGameEntity.IsValid)
			{
				MatrixFrame globalFrame = firstScriptOfTypeRecursive.GameEntity.GetGlobalFrame();
				weakGameEntity.SetGlobalFrame(ref globalFrame, true);
				if (MBSkeletonExtensions.GetActionAtChannel(weakGameEntity.Skeleton, 0) != this._shipControlActionRelaxedIndex)
				{
					MBSkeletonExtensions.SetAgentActionChannel(weakGameEntity.Skeleton, 0, ref this._shipControlActionRelaxedIndex, 0f, 0f, true, flag ? 0.5f : 0f);
				}
			}
		}

		// Token: 0x06000DFD RID: 3581 RVA: 0x0006DAFD File Offset: 0x0006BCFD
		public override bool ShouldAutoLeaveDetachmentWhenDisabled(BattleSideEnum sideEnum)
		{
			return false;
		}

		// Token: 0x06000DFE RID: 3582 RVA: 0x0006DB00 File Offset: 0x0006BD00
		public bool IsAttachedShipVacant()
		{
			if (this.AttachedShip.Formation == null)
			{
				return true;
			}
			if (!this.AttachedShip.AnyActiveFormationTroopOnShip)
			{
				NavalAgentsLogic navalAgentsLogic = this._navalAgentsLogic;
				return navalAgentsLogic != null && navalAgentsLogic.GetReservedTroopsCountOfShip(this.AttachedShip) <= 0;
			}
			return false;
		}

		// Token: 0x06000DFF RID: 3583 RVA: 0x0006DB3D File Offset: 0x0006BD3D
		public override void OnMissionEnded()
		{
		}

		// Token: 0x06000E00 RID: 3584 RVA: 0x0006DB3F File Offset: 0x0006BD3F
		public void SetOverridenDescriptionForActiveEnemyShipControllerMachine(TextObject description)
		{
			this._overridenDescriptionForActiveEnemyShipControllerMachine = description;
		}

		// Token: 0x0400089A RID: 2202
		public const float CaptureTime = 3f;

		// Token: 0x0400089B RID: 2203
		private const string ControllerEntityName = "controller";

		// Token: 0x0400089C RID: 2204
		private const string HandTargetEntityName = "hand_position";

		// Token: 0x0400089D RID: 2205
		private const string CameraTargetEntityName = "camera_target";

		// Token: 0x0400089E RID: 2206
		private const string ShoulderCameraTargetEntityName = "shoulder_camera_target";

		// Token: 0x0400089F RID: 2207
		private const string FrontCameraTargetEntityName = "front_camera_target";

		// Token: 0x040008A0 RID: 2208
		private const string RudderRotationEntityTag = "rudder_rotation_entity";

		// Token: 0x040008A3 RID: 2211
		private GameEntity _cameraTargetEntity;

		// Token: 0x040008A5 RID: 2213
		public GameEntity _rudderRotationEntity;

		// Token: 0x040008A6 RID: 2214
		private MatrixFrame _rudderRotationEntityInitialLocalFrame;

		// Token: 0x040008A7 RID: 2215
		private GameEntity _shoulderCameraTargetEntity;

		// Token: 0x040008A8 RID: 2216
		private GameEntity _frontCameraTargetEntity;

		// Token: 0x040008A9 RID: 2217
		private ActionIndexCache _shipControlActionPushLeftIndex = ActionIndexCache.act_none;

		// Token: 0x040008AA RID: 2218
		private ActionIndexCache _shipControlActionPullRightIndex = ActionIndexCache.act_none;

		// Token: 0x040008AB RID: 2219
		private ActionIndexCache _shipControlActionRelaxedIndex = ActionIndexCache.act_none;

		// Token: 0x040008AC RID: 2220
		private ActionIndexCache _shipCaptureActionIndex = ActionIndexCache.act_none;

		// Token: 0x040008AD RID: 2221
		private TextObject _overridenDescriptionForActiveEnemyShipControllerMachine;

		// Token: 0x040008AE RID: 2222
		private NavalShipsLogic _navalShipsLogic;

		// Token: 0x040008AF RID: 2223
		private NavalAgentsLogic _navalAgentsLogic;

		// Token: 0x040008B0 RID: 2224
		[EditableScriptComponentVariable(true, "")]
		private Vec3 _cameraOffset = new Vec3(0f, -20f, 5f, -1f);

		// Token: 0x040008B1 RID: 2225
		[EditableScriptComponentVariable(true, "")]
		private string _shipCaptureAction = "act_ship_capture";

		// Token: 0x040008B2 RID: 2226
		[EditableScriptComponentVariable(true, "")]
		private string _shipControlActionTurnLeft = "act_rudder_backward_push_idle";

		// Token: 0x040008B3 RID: 2227
		[EditableScriptComponentVariable(true, "")]
		private string _shipControlActionTurnRight = "act_rudder_backward_pull_idle";

		// Token: 0x040008B4 RID: 2228
		[EditableScriptComponentVariable(true, "")]
		private string _shipControlActionRelaxed = "act_rudder_backward_stand_idle";

		// Token: 0x040008B5 RID: 2229
		[EditableScriptComponentVariable(true, "")]
		private bool _isRightHandOnly;

		// Token: 0x040008B6 RID: 2230
		[EditableScriptComponentVariable(true, "")]
		private Vec3 _shoulderCameraOffset = new Vec3(0f, 0f, 0f, -1f);

		// Token: 0x040008B7 RID: 2231
		[EditableScriptComponentVariable(true, "")]
		private bool _isLeftHandOnly;

		// Token: 0x040008B8 RID: 2232
		[EditableScriptComponentVariable(true, "")]
		private Vec3 _frontCameraOffset = new Vec3(0f, -10f, 2f, -1f);

		// Token: 0x040008B9 RID: 2233
		[EditableScriptComponentVariable(true, "")]
		private float _shoulderCameraDistance = 2f;

		// Token: 0x040008BA RID: 2234
		[EditableScriptComponentVariable(true, "")]
		private float _frontCameraDistance = 10f;

		// Token: 0x040008BB RID: 2235
		[EditableScriptComponentVariable(true, "")]
		private float _cameraFovMultiplier = 1f;

		// Token: 0x040008BC RID: 2236
		[EditableScriptComponentVariable(true, "")]
		private float _shoulderCameraFovMultiplier = 1f;

		// Token: 0x040008BD RID: 2237
		[EditableScriptComponentVariable(true, "")]
		private float _frontCameraFovMultiplier = 1f;

		// Token: 0x040008BE RID: 2238
		private float _captureTimer = -1f;
	}
}
