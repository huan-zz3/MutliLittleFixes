using System;
using MissionSharedLibrary.Config;
using RTSCamera.CommandSystem.Config;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace RTSCamera.CommandSystem.Logic.SubLogic
{
	// Token: 0x02000085 RID: 133
	public class AgentAIInputHandler
	{
		// Token: 0x170000BA RID: 186
		// (get) Token: 0x06000503 RID: 1283 RVA: 0x0001D8EA File Offset: 0x0001BAEA
		// (set) Token: 0x06000504 RID: 1284 RVA: 0x0001D8F2 File Offset: 0x0001BAF2
		public VolleyMode VolleyMode { get; private set; }

		// Token: 0x170000BB RID: 187
		// (get) Token: 0x06000505 RID: 1285 RVA: 0x0001D8FB File Offset: 0x0001BAFB
		// (set) Token: 0x06000506 RID: 1286 RVA: 0x0001D903 File Offset: 0x0001BB03
		public bool IsVolleySuspended { get; private set; }

		// Token: 0x170000BC RID: 188
		// (get) Token: 0x06000507 RID: 1287 RVA: 0x0001D90C File Offset: 0x0001BB0C
		public bool AllowPreAiming
		{
			get
			{
				return MissionConfigBase<CommandSystemConfig>.Get().VolleyPreAimingMode == VolleyPreAimingMode.BothAutoAndManualVolley || this.VolleyMode == VolleyMode.Auto;
			}
		}

		// Token: 0x06000508 RID: 1288 RVA: 0x0001D926 File Offset: 0x0001BB26
		public bool IsPreAimingEnabled(Agent agent)
		{
			return this.AllowPreAiming && (!this._formationPosition.IsValid || this._distanceToFormationPosition < 7f);
		}

		// Token: 0x06000509 RID: 1289 RVA: 0x0001D950 File Offset: 0x0001BB50
		public void SetVolleyMode(Agent agent, VolleyMode volleyMode)
		{
			if (this.VolleyMode == volleyMode)
			{
				return;
			}
			this.VolleyMode = volleyMode;
			if (this.VolleyMode == VolleyMode.Disabled)
			{
				if (agent.GetFiringOrder() == 1 && this.IsVolleyStatusDrawing(agent))
				{
					this._cancelAttackOnVolleyDisabled = true;
				}
				this.IsVolleySuspended = false;
				this._volleyStatus = AgentAIInputHandler.VolleyStatus.WaitingForOrder;
				this.OnVolleyDisabled(agent);
				return;
			}
			if (this.VolleyMode == VolleyMode.Manual)
			{
				this.TransitToState(AgentAIInputHandler.VolleyStatus.CancelAttackBeforeWaitingForOrder, agent);
				return;
			}
			if (this.VolleyMode == VolleyMode.Auto)
			{
				this.ShootUnderVolley(agent);
			}
		}

		// Token: 0x0600050A RID: 1290 RVA: 0x0001D9C8 File Offset: 0x0001BBC8
		private void OnVolleyWait(Agent agent)
		{
			if (!agent.HasMount)
			{
				this.SetCanAttack(agent, false);
			}
			this.SetWaitingBehavior(agent);
		}

		// Token: 0x0600050B RID: 1291 RVA: 0x0001D9E1 File Offset: 0x0001BBE1
		private void OnVolleyDisabled(Agent agent)
		{
			this.SetCanAttack(agent, true);
			this.SetNoVolleyBehavior(agent);
		}

		// Token: 0x0600050C RID: 1292 RVA: 0x0001D9F2 File Offset: 0x0001BBF2
		private void OnShootingEnabled(Agent agent)
		{
			this.SetCanAttack(agent, true);
			this.SetShootingBehavior(agent);
		}

		// Token: 0x0600050D RID: 1293 RVA: 0x0001DA04 File Offset: 0x0001BC04
		public bool ShootUnderVolley(Agent agent)
		{
			if (this.VolleyMode != VolleyMode.Disabled && !this.IsVolleySuspended)
			{
				this._shouldResetAutoVolleyTimer = true;
				switch (this._volleyStatus)
				{
				case AgentAIInputHandler.VolleyStatus.CancelAttackBeforeWaitingForOrder:
				case AgentAIInputHandler.VolleyStatus.Reloading:
				case AgentAIInputHandler.VolleyStatus.WaitingForOrder:
				case AgentAIInputHandler.VolleyStatus.StandAfterShooted:
				case AgentAIInputHandler.VolleyStatus.StandAfterCancelShooting:
					this.TransitToState(AgentAIInputHandler.VolleyStatus.PrepareForShooting, agent);
					return true;
				case AgentAIInputHandler.VolleyStatus.TryAimingWhileWaitingForOrder:
					this._waitingForLookingForTargetTimer.Reset(Mission.Current.CurrentTime, 2f);
					this.TransitToState(AgentAIInputHandler.VolleyStatus.WaitingForLookingForTarget, agent);
					return true;
				case AgentAIInputHandler.VolleyStatus.AimWhileWaitingForOrder:
				case AgentAIInputHandler.VolleyStatus.AimingDoneWhileWaitingForOrder:
					this.TransitToState(AgentAIInputHandler.VolleyStatus.DrawingTheBowUnderShootingOrder, agent);
					return true;
				case AgentAIInputHandler.VolleyStatus.WaitingForLookingForTarget:
					this._waitingForLookingForTargetTimer.Reset(Mission.Current.CurrentTime, 1f);
					return true;
				case AgentAIInputHandler.VolleyStatus.DrawingTheBowUnderShootingOrder:
					this._drawingTheBowUnderShootingOrderTimer.Reset(Mission.Current.CurrentTime, 4f);
					return true;
				}
				return false;
			}
			return false;
		}

		// Token: 0x0600050E RID: 1294 RVA: 0x0001DAE4 File Offset: 0x0001BCE4
		public void OnFormationSet(Agent agent)
		{
			VolleyMode volleyMode = VolleyMode.Disabled;
			if (agent.Formation != null)
			{
				volleyMode = CommandQueueLogic.GetFormationVolleyMode(agent.Formation);
			}
			if (volleyMode != this.VolleyMode)
			{
				this.SetVolleyMode(agent, volleyMode);
			}
		}

		// Token: 0x0600050F RID: 1295 RVA: 0x0001DB18 File Offset: 0x0001BD18
		public void OnHit(Agent affectedAgent, Agent affectorAgent, int damage, in MissionWeapon affectorWeapon, in Blow b, in AttackCollisionData collisionData)
		{
			if (this.VolleyMode != VolleyMode.Disabled && !this.IsVolleySuspended)
			{
				AgentAIInputHandler.VolleyStatus volleyStatus = this._volleyStatus;
				if (volleyStatus - AgentAIInputHandler.VolleyStatus.AimWhileWaitingForOrder <= 1)
				{
					this.TransitToState(AgentAIInputHandler.VolleyStatus.TryAimingWhileWaitingForOrder, affectedAgent);
					return;
				}
				if (volleyStatus != AgentAIInputHandler.VolleyStatus.DrawingTheBowUnderShootingOrder)
				{
					return;
				}
				this._allowMovingTimer.Reset(Mission.Current.CurrentTime, 0.6f);
				this._waitingForLookingForTargetTimer.Reset(Mission.Current.CurrentTime, 3f);
				this.TransitToState(AgentAIInputHandler.VolleyStatus.WaitingForLookingForTarget, affectedAgent);
			}
		}

		// Token: 0x06000510 RID: 1296 RVA: 0x0001DB90 File Offset: 0x0001BD90
		public void OnControllerChanged(Agent agent, AgentControllerType oldController)
		{
			VolleyMode volleyMode = VolleyMode.Disabled;
			if (agent.Controller == 1 && agent.Formation != null)
			{
				volleyMode = CommandQueueLogic.GetFormationVolleyMode(agent.Formation);
			}
			if (volleyMode != this.VolleyMode)
			{
				this.SetVolleyMode(agent, volleyMode);
			}
		}

		// Token: 0x06000511 RID: 1297 RVA: 0x0001DBD0 File Offset: 0x0001BDD0
		public bool IsCandidateForNextFireInAutoVolley(Agent agent)
		{
			return agent.IsAIControlled && !this.IsVolleySuspended && ((this._volleyStatus == AgentAIInputHandler.VolleyStatus.TryAimingWhileWaitingForOrder && !this._tryAimingTimeoutInAutoVolley) || (this._volleyStatus == AgentAIInputHandler.VolleyStatus.AimWhileWaitingForOrder && !this._aimTimeout) || this._volleyStatus == AgentAIInputHandler.VolleyStatus.AimingDoneWhileWaitingForOrder);
		}

		// Token: 0x06000512 RID: 1298 RVA: 0x0001DC1C File Offset: 0x0001BE1C
		public bool IsReadyForNextFire(Agent agent)
		{
			switch (this._volleyStatus)
			{
			case AgentAIInputHandler.VolleyStatus.CancelAttackBeforeWaitingForOrder:
				return false;
			case AgentAIInputHandler.VolleyStatus.Reloading:
				return false;
			case AgentAIInputHandler.VolleyStatus.WaitingForOrder:
				return false;
			case AgentAIInputHandler.VolleyStatus.TryAimingWhileWaitingForOrder:
				return false;
			case AgentAIInputHandler.VolleyStatus.AimWhileWaitingForOrder:
				return false;
			case AgentAIInputHandler.VolleyStatus.AimingDoneWhileWaitingForOrder:
				return true;
			case AgentAIInputHandler.VolleyStatus.PrepareForShooting:
				return false;
			case AgentAIInputHandler.VolleyStatus.ForceDrawing:
				return false;
			case AgentAIInputHandler.VolleyStatus.WaitingForLookingForTarget:
				return false;
			case AgentAIInputHandler.VolleyStatus.DrawingTheBowUnderShootingOrder:
				return false;
			case AgentAIInputHandler.VolleyStatus.StandAfterShooted:
				return false;
			case AgentAIInputHandler.VolleyStatus.StandAfterCancelShooting:
				return false;
			default:
				return false;
			}
		}

		// Token: 0x06000513 RID: 1299 RVA: 0x0001DC81 File Offset: 0x0001BE81
		private void SetCanAttack(Agent agent, bool canAttack)
		{
			if (canAttack)
			{
				agent.SetAgentFlags(agent.GetAgentFlags() | 8);
				return;
			}
			agent.SetAgentFlags(agent.GetAgentFlags() & -9);
		}

		// Token: 0x06000514 RID: 1300 RVA: 0x0001DCA4 File Offset: 0x0001BEA4
		private void SetNoVolleyBehavior(Agent agent)
		{
			if (!agent.IsAIControlled)
			{
				return;
			}
			if (agent.Formation != null)
			{
				AgentComponentExtensions.RefreshBehaviorValues(agent, agent.Formation.GetReadonlyMovementOrderReference().OrderEnum, agent.Formation.ArrangementOrder.OrderEnum);
				return;
			}
			AgentComponentExtensions.SetBehaviorValueSet(agent, 0);
		}

		// Token: 0x06000515 RID: 1301 RVA: 0x0001DCF0 File Offset: 0x0001BEF0
		private void SetWaitingBehavior(Agent agent)
		{
			if (!agent.IsAIControlled)
			{
				return;
			}
			if (agent.Formation != null)
			{
				MovementOrder.MovementOrderEnum orderEnum = agent.Formation.GetReadonlyMovementOrderReference().OrderEnum;
				AgentComponentExtensions.RefreshBehaviorValues(agent, orderEnum, agent.Formation.ArrangementOrder.OrderEnum);
				if (orderEnum == 2 || orderEnum == 3)
				{
					return;
				}
			}
			else
			{
				AgentComponentExtensions.SetBehaviorValueSet(agent, 0);
			}
			if (this.VolleyMode == VolleyMode.Auto)
			{
				return;
			}
			AgentComponentExtensions.SetAIBehaviorValues(agent, 2, 0f, 7f, 0f, 20f, 0f);
			AgentComponentExtensions.SetAIBehaviorValues(agent, 4, 0f, 15f, 0f, 30f, 0f);
		}

		// Token: 0x06000516 RID: 1302 RVA: 0x0001DD90 File Offset: 0x0001BF90
		private void SetShootingBehavior(Agent agent)
		{
			if (!agent.IsAIControlled)
			{
				return;
			}
			if (agent.Formation != null)
			{
				MovementOrder.MovementOrderEnum orderEnum = agent.Formation.GetReadonlyMovementOrderReference().OrderEnum;
				AgentComponentExtensions.RefreshBehaviorValues(agent, orderEnum, agent.Formation.ArrangementOrder.OrderEnum);
				if (orderEnum == 2 || orderEnum == 3)
				{
					return;
				}
			}
			else
			{
				AgentComponentExtensions.SetBehaviorValueSet(agent, 0);
			}
			if (this.VolleyMode == VolleyMode.Auto)
			{
				return;
			}
			AgentComponentExtensions.SetAIBehaviorValues(agent, 0, 3f, 7f, 1f, 20f, 0.5f);
			AgentComponentExtensions.SetAIBehaviorValues(agent, 1, 0f, 7f, 0f, 20f, 0f);
			AgentComponentExtensions.SetAIBehaviorValues(agent, 2, 1f, 7f, 1f, 20f, 1f);
			AgentComponentExtensions.SetAIBehaviorValues(agent, 3, 0f, 25f, 0f, 30f, 0f);
			AgentComponentExtensions.SetAIBehaviorValues(agent, 4, 0.7f, 15f, 0.7f, 30f, 0.7f);
			AgentComponentExtensions.SetAIBehaviorValues(agent, 5, 0f, 12f, 0f, 30f, 0f);
			AgentComponentExtensions.SetAIBehaviorValues(agent, 6, 1f, 12f, 1f, 30f, 1f);
		}

		// Token: 0x06000517 RID: 1303 RVA: 0x0001DED0 File Offset: 0x0001C0D0
		private void TransitToState(AgentAIInputHandler.VolleyStatus newStatus, Agent agent)
		{
			switch (newStatus)
			{
			case AgentAIInputHandler.VolleyStatus.CancelAttackBeforeWaitingForOrder:
				this._cancelAttackeBeforeWaitingForOrder.Reset(Mission.Current.CurrentTime, MBRandom.RandomFloat * 0.6f);
				break;
			case AgentAIInputHandler.VolleyStatus.WaitingForOrder:
				this.OnVolleyWait(agent);
				break;
			case AgentAIInputHandler.VolleyStatus.AimWhileWaitingForOrder:
				this._minAimingError = float.MaxValue;
				this._aimWhileWaitingForOrderTimer.Reset(Mission.Current.CurrentTime, MissionConfigBase<CommandSystemConfig>.Get().MaxAimingTime);
				this._aimTimeout = false;
				break;
			case AgentAIInputHandler.VolleyStatus.AimingDoneWhileWaitingForOrder:
				this._minAimingError = agent.CurrentAimingError;
				break;
			case AgentAIInputHandler.VolleyStatus.PrepareForShooting:
				if (this.VolleyMode == VolleyMode.Auto)
				{
					this._prepareForShootingTimer.Reset(Mission.Current.CurrentTime, 0f);
				}
				else
				{
					this._prepareForShootingTimer.Reset(Mission.Current.CurrentTime, MBRandom.RandomFloat * 0.6f);
				}
				break;
			case AgentAIInputHandler.VolleyStatus.DrawingTheBowUnderShootingOrder:
				this._drawingTheBowUnderShootingOrderTimer.Reset(Mission.Current.CurrentTime, agent.HasMount ? 15f : 7.5f);
				break;
			case AgentAIInputHandler.VolleyStatus.StandAfterShooted:
				this._standAfterShootedTimer.Reset(Mission.Current.CurrentTime, 2f);
				this.SetWaitingBehavior(agent);
				break;
			case AgentAIInputHandler.VolleyStatus.StandAfterCancelShooting:
				this._standAfterCancelShootingTimer.Reset(Mission.Current.CurrentTime, 0f);
				this.SetWaitingBehavior(agent);
				break;
			}
			this._volleyStatus = newStatus;
		}

		// Token: 0x06000518 RID: 1304 RVA: 0x0001E050 File Offset: 0x0001C250
		public void OnAIInputSet(Agent agent, ref Agent.EventControlFlag eventFlag, ref Agent.MovementControlFlag movementFlag, ref Vec2 inputVector)
		{
			if (this.VolleyMode != VolleyMode.Disabled && agent.IsAIControlled)
			{
				this.UpdateAITarget(agent);
				bool flag = !this.AllowPreAiming;
				MissionWeapon wieldedWeapon = agent.WieldedWeapon;
				if ((agent.Formation != null && ((agent.Formation.GetMovementState() == 0 && flag) || agent.Formation.FiringOrder == FiringOrder.FiringOrderHoldYourFire)) || (!wieldedWeapon.IsEmpty && !wieldedWeapon.CurrentUsageItem.IsRangedWeapon) || (!agent.HasAnyRangedWeaponCached && !wieldedWeapon.IsEmpty && wieldedWeapon.CurrentUsageItem.IsRangedWeapon && wieldedWeapon.IsReloading) || agent.IsDetachedFromFormation || agent.IsUsingGameObject || AgentComponentExtensions.AIMoveToGameObjectIsEnabled(agent) || (this._isTargetAgentNearby && flag))
				{
					if (!this.IsVolleySuspended)
					{
						this.IsVolleySuspended = true;
						this.OnVolleyDisabled(agent);
						if (this._volleyStatus != AgentAIInputHandler.VolleyStatus.PrepareForShooting)
						{
							this._volleyStatus = AgentAIInputHandler.VolleyStatus.WaitingForOrder;
						}
						return;
					}
				}
				else if (this.IsVolleySuspended)
				{
					this.IsVolleySuspended = false;
					this.OnVolleyWait(agent);
				}
				if (!wieldedWeapon.IsEmpty && wieldedWeapon.CurrentUsageItem.IsRangedWeapon && !this.IsVolleySuspended)
				{
					this.UpdateAIDestination(agent, inputVector);
					float currentTime = Mission.Current.CurrentTime;
					agent.GetLastTargetVisibilityState();
					switch (this._volleyStatus)
					{
					case AgentAIInputHandler.VolleyStatus.CancelAttackBeforeWaitingForOrder:
						if (AgentAIInputHandler.IsAttacking(movementFlag) && !agent.WieldedWeapon.IsReloading)
						{
							AgentAIInputHandler.SetCancelAttack(ref movementFlag);
						}
						if (this._cancelAttackeBeforeWaitingForOrder.Check(currentTime))
						{
							this.TransitToState(AgentAIInputHandler.VolleyStatus.WaitingForOrder, agent);
							return;
						}
						break;
					case AgentAIInputHandler.VolleyStatus.Reloading:
						if (!agent.WieldedWeapon.IsReloading && !agent.WieldedWeapon.IsEmpty)
						{
							this.TransitToState(AgentAIInputHandler.VolleyStatus.WaitingForOrder, agent);
							return;
						}
						break;
					case AgentAIInputHandler.VolleyStatus.WaitingForOrder:
						if (agent.WieldedWeapon.IsReloading)
						{
							this.OnShootingEnabled(agent);
							this.TransitToState(AgentAIInputHandler.VolleyStatus.Reloading, agent);
							return;
						}
						if (this.IsPreAimingEnabled(agent))
						{
							this.OnShootingEnabled(agent);
							if (AgentAIInputHandler.IsAttacking(movementFlag))
							{
								this.TransitToState(AgentAIInputHandler.VolleyStatus.AimWhileWaitingForOrder, agent);
								return;
							}
							this._tryAimingTimeoutInAutoVolley = false;
							if (this._shouldResetAutoVolleyTimer)
							{
								this._autoVolleyAimingTimer.Reset(currentTime, MissionConfigBase<CommandSystemConfig>.Get().MaxAimingTime);
								this._shouldResetAutoVolleyTimer = false;
							}
							this.TransitToState(AgentAIInputHandler.VolleyStatus.TryAimingWhileWaitingForOrder, agent);
							return;
						}
						else if (AgentAIInputHandler.IsAttacking(movementFlag))
						{
							this._shouldResetAutoVolleyTimer = true;
							AgentAIInputHandler.SetCancelAttack(ref movementFlag);
							return;
						}
						break;
					case AgentAIInputHandler.VolleyStatus.TryAimingWhileWaitingForOrder:
						if (!this.IsPreAimingEnabled(agent))
						{
							if (AgentAIInputHandler.IsAttacking(movementFlag))
							{
								AgentAIInputHandler.SetCancelAttack(ref movementFlag);
							}
							this.OnVolleyWait(agent);
							this._shouldResetAutoVolleyTimer = true;
							this.TransitToState(AgentAIInputHandler.VolleyStatus.WaitingForOrder, agent);
							return;
						}
						if (AgentAIInputHandler.IsAttacking(movementFlag))
						{
							this.TransitToState(AgentAIInputHandler.VolleyStatus.AimWhileWaitingForOrder, agent);
							return;
						}
						if (!this._tryAimingTimeoutInAutoVolley && this._autoVolleyAimingTimer.Check(currentTime))
						{
							this._tryAimingTimeoutInAutoVolley = true;
							return;
						}
						break;
					case AgentAIInputHandler.VolleyStatus.AimWhileWaitingForOrder:
						if (!this.IsPreAimingEnabled(agent))
						{
							AgentAIInputHandler.SetCancelAttack(ref movementFlag);
							this._shouldResetAutoVolleyTimer = true;
							this.TransitToState(AgentAIInputHandler.VolleyStatus.WaitingForOrder, agent);
							return;
						}
						if (AgentAIInputHandler.IsCancelAttack(movementFlag))
						{
							this.TransitToState(AgentAIInputHandler.VolleyStatus.TryAimingWhileWaitingForOrder, agent);
							return;
						}
						if (!AgentAIInputHandler.IsAttacking(movementFlag))
						{
							AgentAIInputHandler.SetAttack(ref movementFlag, true);
							float currentAimingError = agent.CurrentAimingError;
							this._minAimingError = currentAimingError;
							this.TransitToState(AgentAIInputHandler.VolleyStatus.AimingDoneWhileWaitingForOrder, agent);
							return;
						}
						if (!this._aimTimeout && this._autoVolleyAimingTimer.Check(currentTime))
						{
							this._aimTimeout = true;
							return;
						}
						break;
					case AgentAIInputHandler.VolleyStatus.AimingDoneWhileWaitingForOrder:
					{
						if (!this.IsPreAimingEnabled(agent))
						{
							AgentAIInputHandler.SetCancelAttack(ref movementFlag);
							this._shouldResetAutoVolleyTimer = true;
							this.TransitToState(AgentAIInputHandler.VolleyStatus.WaitingForOrder, agent);
							return;
						}
						if (!AgentAIInputHandler.IsAttacking(movementFlag))
						{
							AgentAIInputHandler.SetAttack(ref movementFlag, true);
							return;
						}
						bool flag2 = AgentAIInputHandler.IsCancelAttack(movementFlag);
						if (!flag2)
						{
							float currentAimingError2 = agent.CurrentAimingError;
							float currentAimingTurbulance = agent.CurrentAimingTurbulance;
							this._minAimingError = MathF.Min(this._minAimingError, currentAimingError2);
							if (this._minAimingError < currentAimingError2 && !agent.HasMount && this._isAIAtMoveDestination && inputVector == Vec2.Zero)
							{
								AgentAIInputHandler.SetCancelAttack(ref movementFlag);
								flag2 = true;
							}
						}
						if (flag2)
						{
							this.TransitToState(AgentAIInputHandler.VolleyStatus.TryAimingWhileWaitingForOrder, agent);
							return;
						}
						break;
					}
					case AgentAIInputHandler.VolleyStatus.PrepareForShooting:
						if (this._prepareForShootingTimer.Check(currentTime))
						{
							this.OnShootingEnabled(agent);
							if (AgentAIInputHandler.ForceShootingEnabled)
							{
								this._volleyStatus = AgentAIInputHandler.VolleyStatus.ForceDrawing;
								this._forceDrawingTimer.Reset(currentTime, 0f);
								return;
							}
							this._allowMovingTimer.Reset(currentTime, 0.6f);
							this._waitingForLookingForTargetTimer.Reset(currentTime, 3f);
							this.TransitToState(AgentAIInputHandler.VolleyStatus.WaitingForLookingForTarget, agent);
							return;
						}
						break;
					case AgentAIInputHandler.VolleyStatus.ForceDrawing:
						this.SetStand(agent, ref inputVector);
						if (AgentAIInputHandler.IsAttacking(movementFlag))
						{
							this.TransitToState(AgentAIInputHandler.VolleyStatus.DrawingTheBowUnderShootingOrder, agent);
							this._drawingTheBowUnderShootingOrderTimer.Reset(currentTime, 3.6f);
							return;
						}
						if (this._forceDrawingTimer.Check(currentTime))
						{
							this._allowMovingTimer.Reset(currentTime, 0.6f);
							this._waitingForLookingForTargetTimer.Reset(currentTime, 1f);
							this.TransitToState(AgentAIInputHandler.VolleyStatus.WaitingForLookingForTarget, agent);
							return;
						}
						AgentAIInputHandler.SetAttack(ref movementFlag, true);
						return;
					case AgentAIInputHandler.VolleyStatus.WaitingForLookingForTarget:
						if (!agent.WieldedWeapon.IsReloading)
						{
							if (this._allowMovingTimer.Check(currentTime))
							{
								this.SetStand(agent, ref inputVector);
							}
							if (AgentAIInputHandler.IsAttacking(movementFlag))
							{
								this.TransitToState(AgentAIInputHandler.VolleyStatus.DrawingTheBowUnderShootingOrder, agent);
								return;
							}
							if (this._waitingForLookingForTargetTimer.Check(currentTime))
							{
								if (AgentAIInputHandler.ForceShootingEnabled && !agent.WieldedWeapon.IsReloading)
								{
									AgentAIInputHandler.SetCancelAttack(ref movementFlag);
								}
								this.TransitToState(AgentAIInputHandler.VolleyStatus.StandAfterCancelShooting, agent);
								return;
							}
							if (AgentAIInputHandler.ForceShootingEnabled)
							{
								AgentAIInputHandler.SetAttack(ref movementFlag, true);
								return;
							}
						}
						break;
					case AgentAIInputHandler.VolleyStatus.DrawingTheBowUnderShootingOrder:
						this.SetStand(agent, ref inputVector);
						if (AgentAIInputHandler.IsCancelAttack(movementFlag))
						{
							this._allowMovingTimer.Reset(currentTime, 0.6f);
							this._waitingForLookingForTargetTimer.Reset(currentTime, 2f);
							this.TransitToState(AgentAIInputHandler.VolleyStatus.WaitingForLookingForTarget, agent);
							return;
						}
						if (!AgentAIInputHandler.IsAttacking(movementFlag))
						{
							this.TransitToState(AgentAIInputHandler.VolleyStatus.StandAfterShooted, agent);
							return;
						}
						if (agent.WieldedWeapon.IsReloading)
						{
							AgentAIInputHandler.SetCancelAttack(ref movementFlag);
							this.SetWaitingBehavior(agent);
							this.TransitToState(AgentAIInputHandler.VolleyStatus.Reloading, agent);
							return;
						}
						if (this._drawingTheBowUnderShootingOrderTimer.Check(currentTime))
						{
							AgentAIInputHandler.SetCancelAttack(ref movementFlag);
							this.TransitToState(AgentAIInputHandler.VolleyStatus.StandAfterCancelShooting, agent);
							return;
						}
						break;
					case AgentAIInputHandler.VolleyStatus.StandAfterShooted:
						this.SetStand(agent, ref inputVector);
						AgentAIInputHandler.SetAttack(ref movementFlag, false);
						if (agent.WieldedWeapon.IsReloading || agent.WieldedWeapon.IsEmpty)
						{
							this.SetWaitingBehavior(agent);
							this.TransitToState(AgentAIInputHandler.VolleyStatus.Reloading, agent);
							return;
						}
						if (this._standAfterShootedTimer.Check(currentTime) || (!this._isTargetAgentOutOfRange && !AgentAIInputHandler.IsHoldingThrownWeapon(agent)))
						{
							this.TransitToState(AgentAIInputHandler.VolleyStatus.WaitingForOrder, agent);
							return;
						}
						break;
					case AgentAIInputHandler.VolleyStatus.StandAfterCancelShooting:
						this.SetStand(agent, ref inputVector);
						AgentAIInputHandler.SetAttack(ref movementFlag, false);
						if (this._standAfterCancelShootingTimer.Check(currentTime) || !this._isTargetAgentOutOfRange)
						{
							this.TransitToState(AgentAIInputHandler.VolleyStatus.WaitingForOrder, agent);
							return;
						}
						break;
					default:
						return;
					}
				}
			}
			else if (this._cancelAttackOnVolleyDisabled)
			{
				this._cancelAttackOnVolleyDisabled = false;
				AgentAIInputHandler.SetCancelAttack(ref movementFlag);
			}
		}

		// Token: 0x06000519 RID: 1305 RVA: 0x0001E704 File Offset: 0x0001C904
		private static bool IsHoldingThrownWeapon(Agent agent)
		{
			return !agent.WieldedWeapon.IsEmpty && !agent.WieldedWeapon.IsReloading && agent.WieldedWeapon.CurrentUsageItem.IsConsumable;
		}

		// Token: 0x0600051A RID: 1306 RVA: 0x0001E746 File Offset: 0x0001C946
		private static bool IsAttacking(Agent.MovementControlFlag movementFlag)
		{
			return (movementFlag & 960) > 0;
		}

		// Token: 0x0600051B RID: 1307 RVA: 0x0001E752 File Offset: 0x0001C952
		private static void SetAttack(ref Agent.MovementControlFlag movementFlag, bool attack)
		{
			if (attack)
			{
				movementFlag = (int)(movementFlag | 512U);
				return;
			}
			movementFlag = (int)(movementFlag & 4294966335U);
		}

		// Token: 0x0600051C RID: 1308 RVA: 0x0001E76C File Offset: 0x0001C96C
		private static bool IsCancelAttack(Agent.MovementControlFlag movementFlag)
		{
			return (movementFlag & 31744) > 0;
		}

		// Token: 0x0600051D RID: 1309 RVA: 0x0001E778 File Offset: 0x0001C978
		private static void SetCancelAttack(ref Agent.MovementControlFlag movementFlag)
		{
			movementFlag = (int)(movementFlag & 4294966335U);
			movementFlag = (int)(movementFlag | 8192U);
		}

		// Token: 0x0600051E RID: 1310 RVA: 0x0001E78E File Offset: 0x0001C98E
		private void SetStand(Agent agent, ref Vec2 inputVector)
		{
			if (agent.HasMount || this.VolleyMode == VolleyMode.Auto)
			{
				return;
			}
			if (this._isTargetAgentOutOfRange && !this._isAIAtMoveDestination && !this._isMovingToDestination)
			{
				inputVector = Vec2.Zero;
			}
		}

		// Token: 0x0600051F RID: 1311 RVA: 0x0001E7C8 File Offset: 0x0001C9C8
		private void UpdateAIDestination(Agent agent, Vec2 inputVector)
		{
			this._aiMoveDestination = agent.GetAIMoveDestination();
			this._isAIAtMoveDestination = this.IsAIAtMoveDestination(agent);
			if (agent.Formation == null)
			{
				this._isMovingToDestination = true;
				return;
			}
			this._formationPosition = agent.Formation.GetOrderPositionOfUnit(agent);
			if (!this._formationPosition.IsValid)
			{
				this._isMovingToDestination = true;
				return;
			}
			this._agentFrame = agent.Frame;
			Vec3 vec = this._formationPosition.GetGroundVec3() - agent.Position;
			this._distanceToFormationPosition = vec.Normalize();
			if ((double)inputVector.LengthSquared < 0.1 || this._isAIAtMoveDestination)
			{
				this._isMovingToDestination = false;
				return;
			}
			Vec3 vec2 = inputVector.ToVec3(0f);
			float num = Vec3.DotProduct(this._agentFrame.rotation.TransformToParent(ref vec2).NormalizedCopy(), vec);
			this._isMovingToDestination = (double)num > 0.2;
		}

		// Token: 0x06000520 RID: 1312 RVA: 0x0001E8BC File Offset: 0x0001CABC
		private void UpdateAITarget(Agent agent)
		{
			Agent agent2 = agent.GetTargetAgent() ?? agent.ImmediateEnemy;
			if (agent2 == null)
			{
				this._isTargetAgentOutOfRange = true;
				this._isTargetAgentNearby = false;
				return;
			}
			float num = agent2.Position.DistanceSquared(agent.Position);
			float missileRangeWithHeightDifferenceAux = agent.GetMissileRangeWithHeightDifferenceAux(agent2.Position.z);
			this._isTargetAgentOutOfRange = num > missileRangeWithHeightDifferenceAux * missileRangeWithHeightDifferenceAux;
			this._isTargetAgentNearby = num < 25f;
		}

		// Token: 0x06000521 RID: 1313 RVA: 0x0001E930 File Offset: 0x0001CB30
		private bool IsAIAtMoveDestination(Agent agent)
		{
			float aimoveStartTolerance = agent.GetAIMoveStartTolerance();
			return (double)this._aiMoveDestination.AsVec2.DistanceSquared(agent.Position.AsVec2) <= (double)aimoveStartTolerance * (double)aimoveStartTolerance;
		}

		// Token: 0x06000522 RID: 1314 RVA: 0x0001E970 File Offset: 0x0001CB70
		private bool IsVolleyStatusDrawing(Agent agent)
		{
			switch (this._volleyStatus)
			{
			case AgentAIInputHandler.VolleyStatus.AimWhileWaitingForOrder:
			case AgentAIInputHandler.VolleyStatus.AimingDoneWhileWaitingForOrder:
			case AgentAIInputHandler.VolleyStatus.ForceDrawing:
			case AgentAIInputHandler.VolleyStatus.DrawingTheBowUnderShootingOrder:
				return true;
			}
			return false;
		}

		// Token: 0x04000209 RID: 521
		private AgentAIInputHandler.VolleyStatus _volleyStatus;

		// Token: 0x0400020A RID: 522
		private bool _cancelAttackOnVolleyDisabled;

		// Token: 0x0400020C RID: 524
		private MatrixFrame _agentFrame;

		// Token: 0x0400020D RID: 525
		private WorldPosition _aiMoveDestination;

		// Token: 0x0400020E RID: 526
		private WorldPosition _formationPosition;

		// Token: 0x0400020F RID: 527
		private float _distanceToFormationPosition;

		// Token: 0x04000210 RID: 528
		private bool _isMovingToDestination;

		// Token: 0x04000211 RID: 529
		private bool _isAIAtMoveDestination;

		// Token: 0x04000212 RID: 530
		private bool _isTargetAgentOutOfRange;

		// Token: 0x04000213 RID: 531
		private bool _isTargetAgentNearby;

		// Token: 0x04000214 RID: 532
		private float _minAimingError = float.MaxValue;

		// Token: 0x04000215 RID: 533
		private bool _tryAimingTimeoutInAutoVolley;

		// Token: 0x04000216 RID: 534
		private bool _aimTimeout;

		// Token: 0x04000217 RID: 535
		private bool _shouldResetAutoVolleyTimer;

		// Token: 0x04000218 RID: 536
		private Timer _cancelAttackeBeforeWaitingForOrder = new Timer(-1f, -1f, false);

		// Token: 0x04000219 RID: 537
		private Timer _autoVolleyAimingTimer = new Timer(-1f, -1f, false);

		// Token: 0x0400021A RID: 538
		private Timer _aimWhileWaitingForOrderTimer = new Timer(-1f, -1f, false);

		// Token: 0x0400021B RID: 539
		private Timer _prepareForShootingTimer = new Timer(-1f, -1f, false);

		// Token: 0x0400021C RID: 540
		private Timer _forceDrawingTimer = new Timer(-1f, -1f, false);

		// Token: 0x0400021D RID: 541
		private Timer _waitingForLookingForTargetTimer = new Timer(-1f, -1f, false);

		// Token: 0x0400021E RID: 542
		private Timer _allowMovingTimer = new Timer(-1f, -1f, false);

		// Token: 0x0400021F RID: 543
		private Timer _drawingTheBowUnderShootingOrderTimer = new Timer(-1f, -1f, false);

		// Token: 0x04000220 RID: 544
		private Timer _standAfterShootedTimer = new Timer(-1f, -1f, false);

		// Token: 0x04000221 RID: 545
		private Timer _standAfterCancelShootingTimer = new Timer(-1f, -1f, false);

		// Token: 0x04000222 RID: 546
		public static bool ForceShootingEnabled;

		// Token: 0x020000DA RID: 218
		private enum VolleyStatus
		{
			// Token: 0x04000372 RID: 882
			CancelAttackBeforeWaitingForOrder,
			// Token: 0x04000373 RID: 883
			Reloading,
			// Token: 0x04000374 RID: 884
			WaitingForOrder,
			// Token: 0x04000375 RID: 885
			TryAimingWhileWaitingForOrder,
			// Token: 0x04000376 RID: 886
			AimWhileWaitingForOrder,
			// Token: 0x04000377 RID: 887
			AimingDoneWhileWaitingForOrder,
			// Token: 0x04000378 RID: 888
			PrepareForShooting,
			// Token: 0x04000379 RID: 889
			ForceDrawing,
			// Token: 0x0400037A RID: 890
			WaitingForLookingForTarget,
			// Token: 0x0400037B RID: 891
			DrawingTheBowUnderShootingOrder,
			// Token: 0x0400037C RID: 892
			StandAfterShooted,
			// Token: 0x0400037D RID: 893
			StandAfterCancelShooting
		}
	}
}
