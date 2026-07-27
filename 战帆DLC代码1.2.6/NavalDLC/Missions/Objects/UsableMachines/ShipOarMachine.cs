using System;
using System.Collections.Generic;
using NavalDLC.Missions.AI.UsableMachineAIs;
using NavalDLC.Missions.ShipActuators;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.Objects.UsableMachines
{
	// Token: 0x020000B8 RID: 184
	public class ShipOarMachine : UsableMachine, IShipOarScriptComponent
	{
		// Token: 0x17000295 RID: 661
		// (get) Token: 0x06000E0F RID: 3599 RVA: 0x0006DDD1 File Offset: 0x0006BFD1
		// (set) Token: 0x06000E10 RID: 3600 RVA: 0x0006DDD9 File Offset: 0x0006BFD9
		public ResetAnimationOnStopUsageComponent ResetAnimationOnStopUsageComponent { get; private set; }

		// Token: 0x17000296 RID: 662
		// (get) Token: 0x06000E11 RID: 3601 RVA: 0x0006DDE2 File Offset: 0x0006BFE2
		public override bool IsFocusable
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06000E12 RID: 3602 RVA: 0x0006DDE8 File Offset: 0x0006BFE8
		protected override void OnInit()
		{
			base.OnInit();
			WeakGameEntity weakGameEntity;
			WeakGameEntity weakGameEntity2;
			ShipOarDeck.LoadOarScriptEntity(base.GameEntity, out weakGameEntity, ref this._oarExtractedEntitialFrame, ref this._oarRetractedEntitialFrame, out weakGameEntity2);
			this._oarEntity = (weakGameEntity.IsValid ? GameEntity.CreateFromWeakEntity(weakGameEntity) : null);
			this._handTargetLocalFrame = (weakGameEntity2.IsValid ? weakGameEntity2.GetLocalFrame() : MatrixFrame.Identity);
			this._rowIdleActionIndex = ActionIndexCache.Create(this._rowIdleAction);
			this._rowLoopActionIndex = ActionIndexCache.Create(this._rowLoopAction);
			this._rowLoopBackwardActionIndex = ActionIndexCache.Create(this._rowLoopBackwardAction);
			this._rowDeathActionIndex = ActionIndexCache.Create(this._rowDeathAction);
			this._rowSitDownActionIndex = ActionIndexCache.Create(this._rowSitDownAction);
			this._rowStandUpActionIndex = ActionIndexCache.Create(this._rowStandUpAction);
			base.SetScriptComponentToTick(this.GetTickRequirement());
			base.GameEntity.SetHasCustomBoundingBoxValidationSystem(true);
			this._oarMachineBaseBoundingBox = base.GameEntity.ComputeBoundingBoxFromLongestHalfDimension(2f);
			base.DestructionComponent.OnDestroyed += new DestructableComponent.OnHitTakenAndDestroyedDelegate(this.OnOarDestroyed);
			this.ResetAnimationOnStopUsageComponent = new ResetAnimationOnStopUsageComponent(ActionIndexCache.act_none, true);
			this.EnemyRangeToStopUsing = 5f;
			base.PilotStandingPoint.SetIsDisabledForPlayersSynched(true);
		}

		// Token: 0x06000E13 RID: 3603 RVA: 0x0006DF24 File Offset: 0x0006C124
		public void InitializeOar(MissionOar oar)
		{
			this._oar = oar;
		}

		// Token: 0x06000E14 RID: 3604 RVA: 0x0006DF2D File Offset: 0x0006C12D
		public override void OnDeploymentFinished()
		{
			this.EnsureStandingPointComponents();
		}

		// Token: 0x06000E15 RID: 3605 RVA: 0x0006DF38 File Offset: 0x0006C138
		private void EnsureStandingPointComponents()
		{
			if (base.PilotStandingPoint.GetComponent<ResetAnimationOnStopUsageComponent>() == null)
			{
				base.PilotStandingPoint.AddComponent(this.ResetAnimationOnStopUsageComponent);
				base.PilotStandingPoint.AddComponent(new ClearHandInverseKinematicsOnStopUsageComponent());
				base.PilotStandingPoint.AddComponent(new OverrideStrikeAndDeathActionDuringUsageComponent(ref ActionIndexCache.act_row_strike, ref this._rowDeathActionIndex));
			}
		}

		// Token: 0x06000E16 RID: 3606 RVA: 0x0006DF8E File Offset: 0x0006C18E
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return base.GetTickRequirement() | 8;
		}

		// Token: 0x06000E17 RID: 3607 RVA: 0x0006DF98 File Offset: 0x0006C198
		public void ArrangeOarBoundingBox()
		{
			base.GameEntity.SetManualLocalBoundingBox(ref this._oarMachineBaseBoundingBox);
			base.GameEntity.Parent.SetBoundingboxDirty();
		}

		// Token: 0x06000E18 RID: 3608 RVA: 0x0006DFD0 File Offset: 0x0006C1D0
		protected override void OnBoundingBoxValidate()
		{
			BoundingBox boundingBox = base.GameEntity.ComputeBoundingBoxIncludeChildren();
			boundingBox.RelaxWithBoundingBox(this._oarMachineBaseBoundingBox);
			boundingBox.RecomputeRadius();
			base.GameEntity.RelaxLocalBoundingBox(ref boundingBox);
		}

		// Token: 0x06000E19 RID: 3609 RVA: 0x0006E010 File Offset: 0x0006C210
		public bool CheckOarMachineFlags(bool editMode)
		{
			foreach (WeakGameEntity weakGameEntity in base.GameEntity.GetChildren())
			{
				if (!Extensions.HasAnyFlag<EntityFlags>(weakGameEntity.EntityFlags, 131072) && !Extensions.HasAnyFlag<EntityFlags>(weakGameEntity.EntityFlags, 4096))
				{
					string text = string.Concat(new string[]
					{
						"Root Entity: ",
						base.GameEntity.Root.Name,
						" ",
						base.GameEntity.Name,
						"'s child ",
						weakGameEntity.Name,
						" must have Does not Affect Parent's Local Bounding Box flag."
					});
					if (editMode)
					{
						MBEditor.AddEntityWarning(weakGameEntity, text);
					}
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000E1A RID: 3610 RVA: 0x0006E100 File Offset: 0x0006C300
		public void SetSlowDownPhaseForDuration(float slowDownMultiplier, float slowDownDuration)
		{
			this._oar.SetSlowDownPhaseForDuration(slowDownMultiplier, slowDownDuration);
		}

		// Token: 0x06000E1B RID: 3611 RVA: 0x0006E110 File Offset: 0x0006C310
		public void RegisterRampEntityDisablingOar(GameEntity rampEntity)
		{
			if (this._disablingAttachmentRampEntities.Count == 0)
			{
				if (base.PilotStandingPoint.HasUser)
				{
					base.PilotStandingPoint.UserAgent.StopUsingGameObject(true, 1);
				}
				else if (base.PilotStandingPoint.HasAIMovingTo)
				{
					base.PilotStandingPoint.MovingAgent.StopUsingGameObject(true, 1);
				}
				base.PilotStandingPoint.SetIsDeactivatedSynched(true);
			}
			if (!this._disablingAttachmentRampEntities.Contains(rampEntity))
			{
				this._disablingAttachmentRampEntities.Add(rampEntity);
			}
		}

		// Token: 0x06000E1C RID: 3612 RVA: 0x0006E190 File Offset: 0x0006C390
		public void DeregisterRampEntityDisablingOar(GameEntity rampEntity)
		{
			if (this._disablingAttachmentRampEntities.Remove(rampEntity) && this._disablingAttachmentRampEntities.Count == 0)
			{
				base.PilotStandingPoint.SetIsDeactivatedSynched(false);
			}
		}

		// Token: 0x06000E1D RID: 3613 RVA: 0x0006E1B9 File Offset: 0x0006C3B9
		public override bool ShouldAutoLeaveDetachmentWhenDisabled(BattleSideEnum sideEnum)
		{
			return false;
		}

		// Token: 0x06000E1E RID: 3614 RVA: 0x0006E1BC File Offset: 0x0006C3BC
		public override void OnPilotAssignedDuringSpawn()
		{
			this.EnsureStandingPointComponents();
			this._lastPilotAgent = base.PilotAgent;
			this._isPilotSitting = true;
			base.PilotAgent.SetActionChannel(0, ref this._rowIdleActionIndex, false, 0L, 0f, 1f, 0f, 0f, 1f, false, -0.2f, 0, true);
			Vec3 animationDisplacementAtProgress = MBAnimation.GetAnimationDisplacementAtProgress(MBActionSet.GetAnimationIndexOfAction(base.PilotAgent.ActionSet, ref this._rowSitDownActionIndex), 1f);
			MatrixFrame globalFrame = base.PilotStandingPoint.GameEntity.GetGlobalFrame();
			globalFrame.rotation.Orthonormalize();
			Vec3 vec = globalFrame.TransformToParent(ref animationDisplacementAtProgress);
			Vec2 vec2 = globalFrame.rotation.f.AsVec2;
			Vec2 vec3 = vec2.Normalized();
			base.PilotAgent.TeleportToPosition(vec);
			base.PilotAgent.DisableScriptedMovement();
			base.PilotAgent.SetMovementDirection(ref vec3);
			Agent pilotAgent = base.PilotAgent;
			vec2 = vec.AsVec2;
			Vec3 vec4 = vec3.ToVec3(0f);
			pilotAgent.SetTargetPositionAndDirection(ref vec2, ref vec4);
			this._oar.SetOarForceMultiplierFromUserAgent(MissionGameModels.Current.MissionShipParametersModel.CalculateOarForceMultiplier(base.PilotAgent, 1f));
			this._oar.OnPilotAssignedDuringSpawn();
		}

		// Token: 0x06000E1F RID: 3615 RVA: 0x0006E2FA File Offset: 0x0006C4FA
		public void StartDelayedPilotRemoval(Agent.StopUsingGameObjectFlags flags)
		{
			if (this._pilotRemovalTime.Item1 <= 0f)
			{
				this._pilotRemovalTime = new ValueTuple<float, Agent.StopUsingGameObjectFlags>(Mission.Current.CurrentTime + MBRandom.RandomFloat * 2f, flags);
			}
		}

		// Token: 0x06000E20 RID: 3616 RVA: 0x0006E330 File Offset: 0x0006C530
		protected override void OnTickParallel2(float dt)
		{
			MatrixFrame matrixFrame;
			if (this._lastPilotAgent != base.PilotAgent)
			{
				UsableMissionObject pilotStandingPoint = base.PilotStandingPoint;
				matrixFrame = MatrixFrame.Identity;
				pilotStandingPoint.SetCustomLocalFrame(ref matrixFrame);
				base.PilotStandingPoint.LockUserFrames = true;
				this._isPilotSitting = false;
				if (base.PilotAgent != null)
				{
					WorldFrame userFrameForAgent = base.PilotStandingPoint.GetUserFrameForAgent(base.PilotAgent);
					Agent pilotAgent = base.PilotAgent;
					Vec2 vec = userFrameForAgent.Origin.AsVec2;
					pilotAgent.SetTargetPositionAndDirection(ref vec, ref userFrameForAgent.Rotation.f);
					base.PilotAgent.SetScriptedFlags(base.PilotAgent.GetScriptedFlags() | 2);
					this._oar.SetOarForceMultiplierFromUserAgent(MissionGameModels.Current.MissionShipParametersModel.CalculateOarForceMultiplier(base.PilotAgent, 1f));
				}
			}
			this._lastPilotAgent = base.PilotAgent;
			bool flag = base.PilotAgent != null;
			bool flag2 = false;
			this._oar.SetUsed(flag, flag ? base.PilotAgent.Index : (-1));
			MissionOar oar = this._oar;
			matrixFrame = base.GameEntity.GetLocalFrame();
			MatrixFrame localFrame = this._oarEntity.GetLocalFrame();
			MatrixFrame matrixFrame2 = oar.ComputeOarEntityFrame(dt, in matrixFrame, in localFrame, in this._oarExtractedEntitialFrame, in this._oarRetractedEntitialFrame, this._lastIdleTime, false);
			this._oarEntity.SetLocalFrame(ref matrixFrame2, false);
			if (flag)
			{
				if (this._pilotRemovalTime.Item1 > 0f && this._pilotRemovalTime.Item1 < Mission.Current.CurrentTime)
				{
					base.PilotAgent.StopUsingGameObjectMT(true, this._pilotRemovalTime.Item2);
					this._pilotRemovalTime = new ValueTuple<float, Agent.StopUsingGameObjectFlags>(0f, 0);
				}
				else if (!this._isPilotSitting)
				{
					if (base.PilotAgent.GetCurrentAction(0) != this._rowStandUpActionIndex)
					{
						if (base.PilotAgent.MovementLockedState != null)
						{
							MatrixFrame globalFrame = base.PilotStandingPoint.GameEntity.GetGlobalFrame();
							Agent pilotAgent2 = base.PilotAgent;
							Vec2 asVec = globalFrame.origin.AsVec2;
							Vec3 vec2 = this._oar.OwnerShip.Physics.LinearVelocity;
							Vec2 vec = asVec - vec2.AsVec2 * dt;
							pilotAgent2.SetTargetPositionAndDirection(ref vec, ref globalFrame.rotation.f);
							base.PilotStandingPoint.LockUserFrames = true;
							vec = globalFrame.rotation.f.AsVec2;
							if (Vec2.DotProduct(vec.Normalized(), base.PilotAgent.GetMovementDirection()) > 0.99f)
							{
								vec = base.PilotAgent.GetTargetPosition();
								vec2 = base.PilotAgent.Position;
								if (vec.DistanceSquared(vec2.AsVec2) < 0.01f)
								{
									base.PilotAgent.ClearTargetFrame();
									base.PilotAgent.SetActionChannel(0, ref this._rowSitDownActionIndex, false, 0L, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
									base.PilotStandingPoint.LockUserFrames = false;
								}
							}
						}
						else if (base.PilotAgent.GetCurrentAction(0) == this._rowSitDownActionIndex)
						{
							MatrixFrame globalFrame2 = base.PilotStandingPoint.GameEntity.GetGlobalFrame();
							Agent pilotAgent3 = base.PilotAgent;
							Vec2 vec = globalFrame2.origin.AsVec2;
							pilotAgent3.SetTargetPositionAndDirection(ref vec, ref globalFrame2.rotation.f);
							base.PilotAgent.ClearTargetFrame();
							base.PilotStandingPoint.LockUserFrames = false;
							if (base.PilotAgent.GetCurrentActionProgress(0) > 0.99f)
							{
								this._isPilotSitting = true;
								base.PilotAgent.SetActionChannel(0, ref ActionIndexCache.act_usage_row_idle_no_hold, false, 0L, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
							}
							else if (base.PilotAgent.GetCurrentActionProgress(0) > 0.25f)
							{
								flag2 = true;
							}
						}
						else
						{
							base.PilotAgent.StopUsingGameObjectMT(true, 1);
						}
					}
				}
				else
				{
					int animationIndexOfAction = MBActionSet.GetAnimationIndexOfAction(base.PilotAgent.ActionSet, ref this._rowSitDownActionIndex);
					UsableMissionObject pilotStandingPoint2 = base.PilotStandingPoint;
					Mat3 identity = Mat3.Identity;
					Vec3 vec2 = MBAnimation.GetAnimationDisplacementAtProgress(animationIndexOfAction, 1f);
					matrixFrame = new MatrixFrame(ref identity, ref vec2);
					pilotStandingPoint2.SetCustomLocalFrame(ref matrixFrame);
					base.PilotStandingPoint.LockUserFrames = true;
					if (this._oar.IsExtracted)
					{
						bool flag3 = this._oar.NeededRevolutionRate < 0f;
						float num = ((this._oar.VisualPhase + 1.5707964f) / 6.2831855f + 1f) % 1f;
						if (flag3)
						{
							num = 1f - num;
						}
						bool flag4 = this._oar.IsInRowingMotion();
						ActionIndexCache actionIndexCache;
						float num2;
						if (flag4)
						{
							actionIndexCache = (flag3 ? this._rowLoopBackwardActionIndex : this._rowLoopActionIndex);
							num2 = 0f;
						}
						else
						{
							actionIndexCache = this._rowIdleActionIndex;
							num2 = MBRandom.RandomFloatWithSeed((uint)(base.PilotAgent.Index * Environment.TickCount), (uint)(this._oar.OwnerShip.Index * 100));
						}
						if (base.PilotAgent.SetActionChannel(0, ref actionIndexCache, false, 0L, 0f, 1f, -0.2f, 0.4f, num2, false, -0.2f, 0, true) && flag4)
						{
							base.PilotAgent.SetCurrentActionProgress(0, num);
						}
						bool isInBeingStruckAction = base.PilotAgent.IsInBeingStruckAction;
						if (!isInBeingStruckAction && base.PilotAgent.SetActionChannel(1, ref actionIndexCache, false, 0L, 0f, 1f, -0.2f, 0.4f, num2, false, -0.2f, 0, true) && flag4)
						{
							base.PilotAgent.SetCurrentActionProgress(1, num);
						}
						ActionIndexCache currentAction = base.PilotAgent.GetCurrentAction(0);
						ActionIndexCache currentAction2 = base.PilotAgent.GetCurrentAction(1);
						if (isInBeingStruckAction || (base.PilotAgent.ActionSet.AreActionsAlternatives(ref currentAction, ref actionIndexCache) && base.PilotAgent.ActionSet.AreActionsAlternatives(ref currentAction2, ref actionIndexCache)))
						{
							MBActionSet actionSet = base.PilotAgent.ActionSet;
							ActionIndexCache actionIndexCache2 = ((currentAction2 != ActionIndexCache.act_none) ? currentAction2 : currentAction);
							int animationIndexOfAction2 = MBActionSet.GetAnimationIndexOfAction(actionSet, ref actionIndexCache2);
							MatrixFrame frame = base.PilotAgent.Frame;
							matrixFrame = base.PilotAgent.GetBoneEntitialFrameAtAnimationProgress(base.PilotAgent.Monster.MainHandBoneIndex, animationIndexOfAction2, num);
							MatrixFrame matrixFrame3 = frame.TransformToParent(ref matrixFrame);
							matrixFrame = base.PilotAgent.GetBoneEntitialFrameAtAnimationProgress(base.PilotAgent.Monster.OffHandBoneIndex, animationIndexOfAction2, num);
							MatrixFrame matrixFrame4 = frame.TransformToParent(ref matrixFrame);
							matrixFrame = this._oarEntity.GetGlobalFrame();
							Vec3 vec3 = matrixFrame.rotation.f.NormalizedCopy();
							float num3 = Vec3.DotProduct(vec3, matrixFrame3.origin - matrixFrame4.origin);
							matrixFrame = this._oarEntity.GetGlobalFrame();
							MatrixFrame matrixFrame5 = matrixFrame.TransformToParent(ref this._handTargetLocalFrame);
							matrixFrame3.origin = matrixFrame5.origin + 0.5f * num3 * vec3;
							matrixFrame4.origin = matrixFrame5.origin - 0.5f * num3 * vec3;
							base.PilotAgent.SetHandInverseKinematicsFrame(ref matrixFrame4, ref matrixFrame3);
						}
						else
						{
							base.PilotAgent.ClearHandInverseKinematics();
							base.PilotAgent.StopUsingGameObjectMT(true, 1);
						}
					}
					else
					{
						base.PilotAgent.SetActionChannel(0, ref ActionIndexCache.act_usage_row_idle_no_hold, false, 0L, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
						if (!base.PilotAgent.IsInBeingStruckAction)
						{
							base.PilotAgent.SetActionChannel(1, ref ActionIndexCache.act_usage_row_idle_no_hold, false, 0L, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
						}
						base.PilotAgent.ClearHandInverseKinematics();
					}
				}
			}
			else
			{
				UsableMissionObject pilotStandingPoint3 = base.PilotStandingPoint;
				matrixFrame = MatrixFrame.Identity;
				pilotStandingPoint3.SetCustomLocalFrame(ref matrixFrame);
				base.PilotStandingPoint.LockUserFrames = true;
				this._isPilotSitting = false;
				this._pilotRemovalTime = new ValueTuple<float, Agent.StopUsingGameObjectFlags>(0f, 0);
			}
			this.ResetAnimationOnStopUsageComponent.UpdateSuccessfulResetAction((base.PilotAgent != null && (this._isPilotSitting || flag2) && base.PilotAgent.Mission.Mode != 6) ? this._rowStandUpActionIndex : ActionIndexCache.act_none);
			if (!flag || !this._oar.IsExtracted)
			{
				this._lastIdleTime = Mission.Current.CurrentTime;
			}
		}

		// Token: 0x06000E21 RID: 3617 RVA: 0x0006EB90 File Offset: 0x0006CD90
		private void OnOarDestroyed(DestructableComponent target, Agent attackerAgent, in MissionWeapon weapon, ScriptComponentBehavior attackerScriptComponentBehavior, int inflictedDamage)
		{
			this._oar.SetUsed(false, -1);
			target.OnDestroyed -= new DestructableComponent.OnHitTakenAndDestroyedDelegate(this.OnOarDestroyed);
		}

		// Token: 0x06000E22 RID: 3618 RVA: 0x0006EBB1 File Offset: 0x0006CDB1
		protected override float GetDetachmentWeightAux(BattleSideEnum side)
		{
			return float.MinValue;
		}

		// Token: 0x06000E23 RID: 3619 RVA: 0x0006EBB8 File Offset: 0x0006CDB8
		public override TextObject GetActionTextForStandingPoint(UsableMissionObject usableGameObject)
		{
			TextObject textObject = new TextObject("{=fEQAPJ2e}{KEY} Use", null);
			textObject.SetTextVariable("KEY", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13), 1f));
			return textObject;
		}

		// Token: 0x06000E24 RID: 3620 RVA: 0x0006EBE7 File Offset: 0x0006CDE7
		public override TextObject GetDescriptionText(WeakGameEntity gameEntity)
		{
			return new TextObject("{=4b2SXZG8}Oar", null);
		}

		// Token: 0x06000E25 RID: 3621 RVA: 0x0006EBF4 File Offset: 0x0006CDF4
		public override UsableMachineAIBase CreateAIBehaviorObject()
		{
			return new ShipOarMachineAI(this);
		}

		// Token: 0x040008C2 RID: 2242
		private GameEntity _oarEntity;

		// Token: 0x040008C3 RID: 2243
		private MatrixFrame _handTargetLocalFrame;

		// Token: 0x040008C4 RID: 2244
		private MatrixFrame _oarExtractedEntitialFrame;

		// Token: 0x040008C5 RID: 2245
		private MatrixFrame _oarRetractedEntitialFrame;

		// Token: 0x040008C6 RID: 2246
		private MissionOar _oar;

		// Token: 0x040008C7 RID: 2247
		private float _lastIdleTime;

		// Token: 0x040008C8 RID: 2248
		private ActionIndexCache _rowIdleActionIndex;

		// Token: 0x040008C9 RID: 2249
		private ActionIndexCache _rowLoopActionIndex;

		// Token: 0x040008CA RID: 2250
		private ActionIndexCache _rowLoopBackwardActionIndex;

		// Token: 0x040008CB RID: 2251
		private ActionIndexCache _rowDeathActionIndex;

		// Token: 0x040008CC RID: 2252
		private ActionIndexCache _rowSitDownActionIndex;

		// Token: 0x040008CD RID: 2253
		private ActionIndexCache _rowStandUpActionIndex;

		// Token: 0x040008CE RID: 2254
		private bool _isPilotSitting;

		// Token: 0x040008D0 RID: 2256
		private Agent _lastPilotAgent;

		// Token: 0x040008D1 RID: 2257
		private ValueTuple<float, Agent.StopUsingGameObjectFlags> _pilotRemovalTime;

		// Token: 0x040008D2 RID: 2258
		private readonly List<GameEntity> _disablingAttachmentRampEntities = new List<GameEntity>();

		// Token: 0x040008D3 RID: 2259
		private BoundingBox _oarMachineBaseBoundingBox;

		// Token: 0x040008D4 RID: 2260
		[EditableScriptComponentVariable(true, "")]
		private string _rowIdleAction = "act_usage_row_idle_right";

		// Token: 0x040008D5 RID: 2261
		[EditableScriptComponentVariable(true, "")]
		private string _rowLoopAction = "act_usage_row_loop_right";

		// Token: 0x040008D6 RID: 2262
		[EditableScriptComponentVariable(true, "")]
		private string _rowLoopBackwardAction = "act_usage_row_loop_right_backward";

		// Token: 0x040008D7 RID: 2263
		[EditableScriptComponentVariable(true, "")]
		private string _rowDeathAction = "act_row_death_right";

		// Token: 0x040008D8 RID: 2264
		[EditableScriptComponentVariable(true, "")]
		private string _rowSitDownAction = "act_row_sit_down_right";

		// Token: 0x040008D9 RID: 2265
		[EditableScriptComponentVariable(true, "")]
		private string _rowStandUpAction = "act_row_stand_up_right";
	}
}
