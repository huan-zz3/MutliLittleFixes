using System;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using NavalDLC.Missions.ShipInput;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.ShipControl
{
	// Token: 0x0200008B RID: 139
	public class AIShipController : ShipController
	{
		// Token: 0x17000198 RID: 408
		// (get) Token: 0x060009CA RID: 2506 RVA: 0x0004535B File Offset: 0x0004355B
		// (set) Token: 0x060009CB RID: 2507 RVA: 0x00045363 File Offset: 0x00043563
		public MissionShip TargetShip { get; private set; }

		// Token: 0x17000199 RID: 409
		// (get) Token: 0x060009CC RID: 2508 RVA: 0x0004536C File Offset: 0x0004356C
		internal MBReadOnlyList<MissionShip> ShipCollisionIgnoreList
		{
			get
			{
				return this._shipCollisionIgnoreList;
			}
		}

		// Token: 0x1700019A RID: 410
		// (get) Token: 0x060009CD RID: 2509 RVA: 0x00045374 File Offset: 0x00043574
		public bool CanAvoidCollisions
		{
			get
			{
				return this._ownerShip.HasDWAAgent && this.CollisionChecksActive && (this.AvoidShipCollisions || this.AvoidObstacleCollisions);
			}
		}

		// Token: 0x1700019B RID: 411
		// (get) Token: 0x060009CE RID: 2510 RVA: 0x0004539D File Offset: 0x0004359D
		internal bool CollisionChecksActive
		{
			get
			{
				return this._collisionChecksActive;
			}
		}

		// Token: 0x1700019C RID: 412
		// (get) Token: 0x060009CF RID: 2511 RVA: 0x000453A5 File Offset: 0x000435A5
		internal bool AvoidShipCollisions
		{
			get
			{
				return this._avoidShipCollisions;
			}
		}

		// Token: 0x1700019D RID: 413
		// (get) Token: 0x060009D0 RID: 2512 RVA: 0x000453AD File Offset: 0x000435AD
		internal bool AvoidObstacleCollisions
		{
			get
			{
				return this._avoidObstacleCollisions;
			}
		}

		// Token: 0x1700019E RID: 414
		// (get) Token: 0x060009D1 RID: 2513 RVA: 0x000453B5 File Offset: 0x000435B5
		// (set) Token: 0x060009D2 RID: 2514 RVA: 0x000453BD File Offset: 0x000435BD
		internal float DesiredLinearAcceleration { get; private set; }

		// Token: 0x1700019F RID: 415
		// (get) Token: 0x060009D3 RID: 2515 RVA: 0x000453C6 File Offset: 0x000435C6
		// (set) Token: 0x060009D4 RID: 2516 RVA: 0x000453CE File Offset: 0x000435CE
		internal float DesiredAngularAcceleration { get; private set; }

		// Token: 0x170001A0 RID: 416
		// (get) Token: 0x060009D5 RID: 2517 RVA: 0x000453D7 File Offset: 0x000435D7
		public bool HasTargetState
		{
			get
			{
				return this._targetMode > AIShipController.TargetMode.None;
			}
		}

		// Token: 0x170001A1 RID: 417
		// (get) Token: 0x060009D6 RID: 2518 RVA: 0x000453E2 File Offset: 0x000435E2
		public bool HasTarget
		{
			get
			{
				return this._targetMode > AIShipController.TargetMode.None;
			}
		}

		// Token: 0x170001A2 RID: 418
		// (get) Token: 0x060009D7 RID: 2519 RVA: 0x000453ED File Offset: 0x000435ED
		public bool HasNavigationPath
		{
			get
			{
				if (this._navigationPath == null && this._navalShipsLogic.SeaPathfindingEnabled)
				{
					this._navigationPath = new NavigationPath();
					this._lastNavPathPointIndex = -1;
				}
				return this._navigationPath != null;
			}
		}

		// Token: 0x060009D8 RID: 2520 RVA: 0x00045420 File Offset: 0x00043620
		public AIShipController(MissionShip ownerShip)
			: base(ownerShip)
		{
			this._controllerType = ShipControllerType.AI;
			this._navalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			this.ClearTarget();
		}

		// Token: 0x060009D9 RID: 2521 RVA: 0x00045478 File Offset: 0x00043678
		public override ShipInputRecord Update(float dt)
		{
			ShipInputRecord shipInputRecord = ShipInputRecord.None();
			if (this.UpdateTargetState())
			{
				float num;
				float desiredLinearAcceleration;
				bool flag = this.HasArrivedAtTarget(out num, out desiredLinearAcceleration);
				if (this._stopOnArrival && flag)
				{
					this.ClearTarget();
					shipInputRecord = ShipInputRecord.Stop();
				}
				else if (!flag)
				{
					ShipInputRecord shipInputRecord2 = shipInputRecord;
					MatrixFrame matrixFrame = this._ownerShip.GlobalFrame;
					Vec2 vec = matrixFrame.rotation.f.AsVec2;
					Vec2 vec2 = vec.Normalized();
					matrixFrame = this._ownerShip.GameEntity.GetGlobalFrame();
					Vec3 linearVelocity = this._ownerShip.Physics.LinearVelocity;
					Vec3 vec3 = matrixFrame.rotation.TransformToLocal(ref linearVelocity);
					vec = this._ownerShip.Scene.GetGlobalWindVelocity();
					float desiredAngularAcceleration = this.DesiredAngularAcceleration;
					desiredLinearAcceleration = this.DesiredLinearAcceleration;
					AIShipController.DecideControl(in shipInputRecord2, in vec2, in vec, desiredAngularAcceleration, in desiredLinearAcceleration, this._ownerShip.MissionShipObject.MaxLinearAccel, this._ownerShip.MissionShipObject.MaxAngularAccel, out shipInputRecord, vec3, this._ownerShip.ShipOrder.EnforceSailUsage);
				}
			}
			this._inputRecord = shipInputRecord;
			return this._inputRecord;
		}

		// Token: 0x060009DA RID: 2522 RVA: 0x00045594 File Offset: 0x00043794
		public void SetTargetPosition(in Vec2 targetPosition, bool stopOnArrival = false)
		{
			this._targetMode = AIShipController.TargetMode.Position;
			this.SetTargetShipAux(null, false);
			this._targetOffset = NavalVec.Zero;
			this._stopOnArrival = stopOnArrival;
			Vec2 vec = this._ownerShip.GlobalFrame.rotation.f.AsVec2;
			vec = vec.Normalized();
			NavalState navalState = new NavalState(in targetPosition, in vec, 0f);
			if (this.HasNavigationPath)
			{
				NavalState navalState2 = this._ownerShip.GetNavalState();
				this.ReComputeNavigationPath(in navalState2, in navalState, false);
			}
			this._targetState = navalState;
		}

		// Token: 0x060009DB RID: 2523 RVA: 0x0004561C File Offset: 0x0004381C
		public void SetTargetState(in Vec2 targetPosition, in Vec2 targetDirection, bool stopOnArrival = false)
		{
			this._targetMode = AIShipController.TargetMode.State;
			this.SetTargetShipAux(null, false);
			this._targetOffset = NavalVec.Zero;
			this._stopOnArrival = stopOnArrival;
			NavalState navalState = new NavalState(in targetPosition, in targetDirection, 0f);
			if (this.HasNavigationPath)
			{
				NavalState navalState2 = this._ownerShip.GetNavalState();
				this.ReComputeNavigationPath(in navalState2, in navalState, false);
			}
			this._targetState = navalState;
		}

		// Token: 0x060009DC RID: 2524 RVA: 0x00045680 File Offset: 0x00043880
		public void SetTargetState(in NavalState targetState, bool stopOnArrival = false)
		{
			this._targetMode = AIShipController.TargetMode.State;
			this.SetTargetShipAux(null, false);
			this._targetOffset = NavalVec.Zero;
			this._stopOnArrival = stopOnArrival;
			if (this.HasNavigationPath)
			{
				NavalState navalState = this._ownerShip.GetNavalState();
				this.ReComputeNavigationPath(in navalState, in targetState, false);
			}
			this._targetState = targetState;
		}

		// Token: 0x060009DD RID: 2525 RVA: 0x000456D8 File Offset: 0x000438D8
		public void SetTargetShip(in MissionShip targetShip, bool stopOnArrival = false, bool ignoreTargetShipCollision = false)
		{
			this._targetMode = AIShipController.TargetMode.Ship;
			this.SetTargetShipAux(targetShip, ignoreTargetShipCollision);
			this._targetOffset = NavalVec.Zero;
			this._stopOnArrival = stopOnArrival;
			NavalState navalState = this.TargetShip.GetNavalState();
			if (this.HasNavigationPath)
			{
				NavalState navalState2 = this._ownerShip.GetNavalState();
				this.ReComputeNavigationPath(in navalState2, in navalState, false);
			}
			this._targetState = navalState;
		}

		// Token: 0x060009DE RID: 2526 RVA: 0x0004573C File Offset: 0x0004393C
		public void SetTargetShipWithOffset(in MissionShip targetShip, in NavalVec localOffset, bool stopOnArrival = false, bool ignoreTargetShipCollision = false)
		{
			this._targetMode = AIShipController.TargetMode.ShipOffset;
			this.SetTargetShipAux(targetShip, ignoreTargetShipCollision);
			this._targetOffset = localOffset;
			this._stopOnArrival = stopOnArrival;
			NavalState navalState = this.TargetShip.GetNavalState(in localOffset);
			if (this.HasNavigationPath)
			{
				NavalState navalState2 = this._ownerShip.GetNavalState();
				this.ReComputeNavigationPath(in navalState2, in navalState, false);
			}
			this._targetState = navalState;
		}

		// Token: 0x060009DF RID: 2527 RVA: 0x000457A0 File Offset: 0x000439A0
		internal void AddShipToCollisionIgnoreListOnAccountOfRamming(MissionShip ship)
		{
			this.AddShipToCollisionIgnoreList(ship);
		}

		// Token: 0x060009E0 RID: 2528 RVA: 0x000457A9 File Offset: 0x000439A9
		internal void AddShipToCollisionIgnoreList(MissionShip ship)
		{
			if (!this._shipCollisionIgnoreList.Contains(ship))
			{
				this._shipCollisionIgnoreList.Add(ship);
			}
		}

		// Token: 0x060009E1 RID: 2529 RVA: 0x000457C5 File Offset: 0x000439C5
		internal void SetAvoidShipCollisions(bool value = true)
		{
			this._avoidShipCollisions = value;
		}

		// Token: 0x060009E2 RID: 2530 RVA: 0x000457CE File Offset: 0x000439CE
		internal void RemoveShipFromCollisionIgnoreListOnAccountOfRamming(MissionShip ship)
		{
			this.RemoveShipFromCollisionIgnoreList(ship);
		}

		// Token: 0x060009E3 RID: 2531 RVA: 0x000457D7 File Offset: 0x000439D7
		internal void RemoveShipFromCollisionIgnoreList(MissionShip ship)
		{
			this._shipCollisionIgnoreList.Remove(ship);
		}

		// Token: 0x060009E4 RID: 2532 RVA: 0x000457E6 File Offset: 0x000439E6
		internal void SetAvoidObstacleCollisions(bool value = true)
		{
			this._avoidObstacleCollisions = value;
		}

		// Token: 0x060009E5 RID: 2533 RVA: 0x000457EF File Offset: 0x000439EF
		internal void SetCollisionChecksActive(bool value = true)
		{
			this._collisionChecksActive = value;
		}

		// Token: 0x060009E6 RID: 2534 RVA: 0x000457F8 File Offset: 0x000439F8
		internal void ClearShipCollisionIgnoreList()
		{
			this._shipCollisionIgnoreList.Clear();
		}

		// Token: 0x060009E7 RID: 2535 RVA: 0x00045805 File Offset: 0x00043A05
		internal bool CheckShipInCollisionIgnoreList(MissionShip ship)
		{
			return this._shipCollisionIgnoreList.Contains(ship);
		}

		// Token: 0x060009E8 RID: 2536 RVA: 0x00045814 File Offset: 0x00043A14
		public bool GetRawTargetState(out Vec2 targetPosition, out Vec2 targetDirection, out float targetSpeed)
		{
			if (this._targetMode != AIShipController.TargetMode.None)
			{
				targetPosition = this._targetState.Position;
				targetDirection = this._targetState.Direction;
				targetSpeed = this._targetState.Speed;
				return true;
			}
			targetPosition = Vec2.Invalid;
			targetDirection = Vec2.Invalid;
			targetSpeed = 0f;
			return false;
		}

		// Token: 0x060009E9 RID: 2537 RVA: 0x00045878 File Offset: 0x00043A78
		public bool GetNextTarget(out Vec2 targetPosition, out Vec2 targetDirection, out float targetSpeed)
		{
			if (this._targetMode == AIShipController.TargetMode.None)
			{
				targetPosition = Vec2.Invalid;
				targetDirection = Vec2.Invalid;
				targetSpeed = 0f;
				return false;
			}
			if (this.HasNavigationPath && this._navigationPath.Size > 0)
			{
				NavalState nextTargetStateOverPath = this.GetNextTargetStateOverPath();
				targetPosition = nextTargetStateOverPath.Position;
				targetDirection = nextTargetStateOverPath.Direction;
				targetSpeed = this._targetState.Speed;
				return true;
			}
			return this.GetRawTargetState(out targetPosition, out targetDirection, out targetSpeed);
		}

		// Token: 0x060009EA RID: 2538 RVA: 0x000458FC File Offset: 0x00043AFC
		public bool HasArrivedAtTarget(out float postionErrorSquared, out float rotationError)
		{
			float num = this._ownerShip.Physics.PhysicsBoundingBoxSizeWithoutChildren.y / 20f;
			float num2 = 0.2617994f;
			NavalState navalState = this._ownerShip.GetNavalState();
			NavalVec navalVec = (in this._targetState) - (in navalState);
			postionErrorSquared = navalVec.DeltaPosition.LengthSquared;
			rotationError = MathF.Abs(navalVec.DeltaOrientation);
			return postionErrorSquared < num * num && rotationError < num2;
		}

		// Token: 0x060009EB RID: 2539 RVA: 0x00045974 File Offset: 0x00043B74
		internal void UpdateTrajectory(float desiredLinearAcceleration, float desiredAngularAcceleration)
		{
			this.DesiredLinearAcceleration = desiredLinearAcceleration;
			this.DesiredAngularAcceleration = desiredAngularAcceleration;
		}

		// Token: 0x060009EC RID: 2540 RVA: 0x00045984 File Offset: 0x00043B84
		public void ClearTarget()
		{
			if (this._ignoreTargetShipCollision)
			{
				this.RemoveShipFromCollisionIgnoreList(this.TargetShip);
				this._ignoreTargetShipCollision = false;
			}
			this.TargetShip = null;
			this._targetMode = AIShipController.TargetMode.None;
			this._targetState = NavalState.Zero;
			if (this.HasNavigationPath)
			{
				this.ClearNavigationPathAux();
			}
			this._targetOffset = NavalVec.Zero;
			this._stopOnArrival = false;
		}

		// Token: 0x060009ED RID: 2541 RVA: 0x000459E8 File Offset: 0x00043BE8
		public bool UpdateTargetState()
		{
			if (this._targetMode != AIShipController.TargetMode.None)
			{
				NavalState navalState = this._ownerShip.GetNavalState();
				if (this._targetMode == AIShipController.TargetMode.Position)
				{
					Vec2 vec = this._ownerShip.GlobalFrame.rotation.f.AsVec2;
					vec = vec.Normalized();
					this._targetState.SetTargetDirection(in vec);
					if (this.HasNavigationPath)
					{
						if (this._navigationPath.Size > 0)
						{
							this.UpdateNavigationPath(in navalState);
						}
						else
						{
							this.ReComputeNavigationPath(in navalState, in this._targetState, false);
						}
					}
					return true;
				}
				if (this._targetMode == AIShipController.TargetMode.State)
				{
					if (this.HasNavigationPath)
					{
						if (this._navigationPath.Size > 0)
						{
							this.UpdateNavigationPath(in navalState);
						}
						else
						{
							this.ReComputeNavigationPath(in navalState, in this._targetState, false);
						}
					}
					return true;
				}
				if (this._targetMode == AIShipController.TargetMode.Ship || this._targetMode == AIShipController.TargetMode.ShipOffset)
				{
					NavalState navalState2;
					if (this._targetMode == AIShipController.TargetMode.Ship)
					{
						navalState2 = this.TargetShip.GetNavalState();
					}
					else
					{
						navalState2 = this.TargetShip.GetNavalState(in this._targetOffset);
					}
					if (this.HasNavigationPath)
					{
						this.ReComputeNavigationPath(in navalState, in navalState2, false);
					}
					this._targetState = navalState2;
					return true;
				}
			}
			return false;
		}

		// Token: 0x060009EE RID: 2542 RVA: 0x00045B0C File Offset: 0x00043D0C
		public float GetTargetStateZ()
		{
			float num = 0f;
			if (this._targetMode != AIShipController.TargetMode.None)
			{
				if (this._targetMode == AIShipController.TargetMode.Ship)
				{
					num = this.TargetShip.GlobalFrame.origin.z;
				}
				else if (this._targetMode == AIShipController.TargetMode.ShipOffset)
				{
					Vec3 origin = this.TargetShip.GlobalFrame.origin;
					float waterLevelAtPosition = this._ownerShip.Scene.GetWaterLevelAtPosition(origin.AsVec2, true, false);
					float num2 = MathF.Max(0f, origin.z - waterLevelAtPosition);
					num = this._ownerShip.Scene.GetWaterLevelAtPosition(this._targetState.Position, true, false) + num2;
				}
				else
				{
					num = this._ownerShip.Scene.GetWaterLevelAtPosition(this._targetState.Position, true, false);
				}
			}
			return num;
		}

		// Token: 0x060009EF RID: 2543 RVA: 0x00045BD8 File Offset: 0x00043DD8
		private ShipInputRecord StabilizeInput(ShipInputRecord inputRecord)
		{
			int num = 5;
			RowerLateralInput rowerLateralInput = this._inputRecord.RowerLateral;
			RowerLongitudinalInput rowerLongitudinalInput = this._inputRecord.RowerLongitudinal;
			RowerLongitudinalInput rowerLongitudinalDoubleTap = this._inputRecord.RowerLongitudinalDoubleTap;
			float num2 = this._inputRecord.RudderLateral;
			SailInput sailInput = this._inputRecord.Sail;
			if (inputRecord.RowerLateral != rowerLateralInput)
			{
				this._rowerLateralDebounceCounter += 1U;
				if ((ulong)this._rowerLateralDebounceCounter >= (ulong)((long)num))
				{
					rowerLateralInput = inputRecord.RowerLateral;
					this._rowerLateralDebounceCounter = 0U;
				}
			}
			else
			{
				this._rowerLateralDebounceCounter = 0U;
			}
			if (inputRecord.RowerLongitudinal != rowerLongitudinalInput)
			{
				this._rowerLongitudinalDebounceCounter += 1U;
				if ((ulong)this._rowerLongitudinalDebounceCounter >= (ulong)((long)num))
				{
					rowerLongitudinalInput = inputRecord.RowerLongitudinal;
					this._rowerLongitudinalDebounceCounter = 0U;
				}
			}
			else
			{
				this._rowerLongitudinalDebounceCounter = 0U;
			}
			if (inputRecord.RudderLateral != num2)
			{
				this._rudderLateralDebounceCounter += 1U;
				if ((ulong)this._rudderLateralDebounceCounter >= (ulong)((long)num))
				{
					num2 = inputRecord.RudderLateral;
					this._rudderLateralDebounceCounter = 0U;
				}
			}
			else
			{
				this._rudderLateralDebounceCounter = 0U;
			}
			if (inputRecord.Sail != sailInput)
			{
				this._sailDebounceCounter += 1U;
				if ((ulong)this._sailDebounceCounter >= (ulong)((long)num))
				{
					sailInput = inputRecord.Sail;
					this._sailDebounceCounter = 0U;
				}
			}
			else
			{
				this._sailDebounceCounter = 0U;
			}
			return new ShipInputRecord(rowerLateralInput, rowerLongitudinalInput, rowerLongitudinalDoubleTap, num2, sailInput);
		}

		// Token: 0x060009F0 RID: 2544 RVA: 0x00045D21 File Offset: 0x00043F21
		private void SetTargetShipAux(MissionShip targetShip, bool ignoreCollision = false)
		{
			if (this._ignoreTargetShipCollision != ignoreCollision || this.TargetShip != targetShip)
			{
				if (this._ignoreTargetShipCollision)
				{
					this.RemoveShipFromCollisionIgnoreList(this.TargetShip);
				}
				if (ignoreCollision)
				{
					this.AddShipToCollisionIgnoreList(targetShip);
				}
				this._ignoreTargetShipCollision = ignoreCollision;
				this.TargetShip = targetShip;
			}
		}

		// Token: 0x060009F1 RID: 2545 RVA: 0x00045D64 File Offset: 0x00043F64
		private static void DecideControl(in ShipInputRecord oldInputRecord, in Vec2 shipForward2D, in Vec2 globalWindVelocity, float desiredAngularAcceleration, in float desiredLinearAcceleration, float maxLinearAcceleration, float maxAngularAcceleration, out ShipInputRecord inputRecord, Vec3 shipLocalVelocity, int enforceSailUsage)
		{
			inputRecord = ShipInputRecord.None();
			float num = MathF.Abs(desiredAngularAcceleration);
			float num2 = MathF.Abs(desiredLinearAcceleration);
			bool flag = num > 0.3f || (num > 0f && num2 <= 0.001f);
			if (num > 0.01f && flag)
			{
				if (desiredAngularAcceleration > 0f)
				{
					inputRecord.SetRowerLateral(RowerLateralInput.Left);
				}
				else if (desiredAngularAcceleration < 0f)
				{
					inputRecord.SetRowerLateral(RowerLateralInput.Right);
				}
			}
			if (flag)
			{
				if (shipLocalVelocity.y > 1f)
				{
					inputRecord.SetRowerLongitudinal(RowerLongitudinalInput.Backward);
				}
				else if (shipLocalVelocity.y < -1f)
				{
					inputRecord.SetRowerLongitudinal(RowerLongitudinalInput.Forward);
				}
			}
			else if (num2 >= 0.01f)
			{
				if (desiredLinearAcceleration >= 0f)
				{
					inputRecord.SetRowerLongitudinal(RowerLongitudinalInput.Forward);
				}
				else
				{
					inputRecord.SetRowerLongitudinal(RowerLongitudinalInput.Backward);
				}
			}
			float num3 = 0f;
			if (flag)
			{
				num3 = inputRecord.RowerLateral.ToRudderInput();
			}
			else if (desiredAngularAcceleration > 0f)
			{
				num3 = -1f;
			}
			else if (desiredAngularAcceleration < 0f)
			{
				num3 = 1f;
			}
			inputRecord.SetRudderLateral(num3);
			Vec2 vec = globalWindVelocity;
			float num4 = Vec2.DotProduct(vec.Normalized(), shipForward2D) * desiredLinearAcceleration;
			float num5 = 0.2f * maxLinearAcceleration;
			float num6 = 0.6f * maxLinearAcceleration;
			if (enforceSailUsage > 0)
			{
				inputRecord.SetSail(SailInput.Full);
				return;
			}
			if (enforceSailUsage < 0)
			{
				inputRecord.SetSail(SailInput.Raised);
				return;
			}
			if (flag)
			{
				inputRecord.SetSail(SailInput.Raised);
				return;
			}
			if (num4 > num6)
			{
				inputRecord.SetSail(SailInput.Full);
				return;
			}
			if (num4 < num5)
			{
				inputRecord.SetSail(SailInput.Raised);
				return;
			}
			ShipInputRecord shipInputRecord = oldInputRecord;
			inputRecord.SetSail(shipInputRecord.Sail);
		}

		// Token: 0x060009F2 RID: 2546 RVA: 0x00045F04 File Offset: 0x00044104
		private void ReComputeNavigationPath(in NavalState currentState, in NavalState newTargetState, bool forceRecompute = false)
		{
			NavalState navalState;
			if (!forceRecompute && !this.ShouldRecomputePath(in currentState, in newTargetState))
			{
				Vec2 vec;
				if (this._navigationPath.Size > 0)
				{
					NavigationPath navigationPath = this._navigationPath;
					int num = this._navigationPath.Size - 1;
					navalState = newTargetState;
					vec = navalState.Position;
					navigationPath.OverridePathPointAtIndex(num, ref vec);
				}
				navalState = newTargetState;
				vec = navalState.Position - this._lastNavPathTargetPosition;
				float length = vec.Length;
				if (length >= 0.0001f)
				{
					this._navPathTargetDriftAccumulator += length;
				}
				navalState = newTargetState;
				this._lastNavPathTargetPosition = navalState.Position;
				this.UpdateNavigationPath(in currentState);
				return;
			}
			navalState = currentState;
			Vec3 vec2;
			vec2..ctor(navalState.Position, 0f, -1f);
			navalState = newTargetState;
			Vec3 vec3;
			vec3..ctor(navalState.Position, 0f, -1f);
			Mission.Current.Scene.SetAbilityOfFacesWithId(1, true);
			UIntPtr nearestNavigationMeshForPosition = Mission.Current.Scene.GetNearestNavigationMeshForPosition(ref vec2, 1000000f, true);
			UIntPtr nearestNavigationMeshForPosition2 = Mission.Current.Scene.GetNearestNavigationMeshForPosition(ref vec3, 1000000f, true);
			if (!(nearestNavigationMeshForPosition != UIntPtr.Zero) || !(nearestNavigationMeshForPosition2 != UIntPtr.Zero))
			{
				this.ClearNavigationPathAux();
				return;
			}
			float num2 = MathF.Lerp(this._ownerShip.Physics.PhysicsBoundingBoxSizeWithoutChildren.x, this._ownerShip.Physics.PhysicsBoundingBoxSizeWithoutChildren.y, 0.75f, 1E-05f);
			bool flag = nearestNavigationMeshForPosition == this._lastNavPathStartFace && nearestNavigationMeshForPosition2 == this._lastNavPathTargetFace;
			bool flag2 = false;
			bool flag3 = this._navigationPath.Size > 0;
			if (!flag || !flag3)
			{
				this._navigationPath.Size = 0;
				flag3 = Mission.Current.Scene.GetPathBetweenAIFaces(nearestNavigationMeshForPosition, nearestNavigationMeshForPosition2, vec2.AsVec2, vec3.AsVec2, num2, this._navigationPath, null);
				flag2 = true;
			}
			else if (flag3)
			{
				NavigationPath navigationPath2 = this._navigationPath;
				int num3 = this._navigationPath.Size - 1;
				navalState = newTargetState;
				Vec2 vec = navalState.Position;
				navigationPath2.OverridePathPointAtIndex(num3, ref vec);
			}
			Mission.Current.Scene.SetAbilityOfFacesWithId(1, false);
			if (flag3)
			{
				if (flag2)
				{
					this._lastNavPathPointIndex = 0;
					this._lastNavPathHardRecomputeTime = Mission.Current.CurrentTime;
				}
				this._lastNavPathStartFace = nearestNavigationMeshForPosition;
				this._lastNavPathTargetFace = nearestNavigationMeshForPosition2;
				navalState = newTargetState;
				this._lastNavPathTargetPosition = navalState.Position;
				this._navPathTargetDriftAccumulator = 0f;
				this.UpdateNavigationPath(in currentState);
				return;
			}
			this.ClearNavigationPathAux();
		}

		// Token: 0x060009F3 RID: 2547 RVA: 0x000461A8 File Offset: 0x000443A8
		private bool ShouldRecomputePath(in NavalState currentState, in NavalState newTargetState)
		{
			if (this._navigationPath.Size == 0)
			{
				return true;
			}
			float num = Mission.Current.CurrentTime - this._lastNavPathHardRecomputeTime;
			Vec2 lastNavPathTargetPosition = this._lastNavPathTargetPosition;
			NavalState navalState = newTargetState;
			Vec2 vec = lastNavPathTargetPosition - navalState.Position;
			if (vec.LengthSquared >= 16f)
			{
				return true;
			}
			if (num >= 0.5f)
			{
				if (this._navPathTargetDriftAccumulator >= 4f)
				{
					return true;
				}
				navalState = currentState;
				vec = navalState.Position;
				navalState = newTargetState;
				Vec2 position = navalState.Position;
				if (this.NavPathStartOrGoalFaceChanged(in vec, in position))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060009F4 RID: 2548 RVA: 0x00046244 File Offset: 0x00044444
		private bool NavPathStartOrGoalFaceChanged(in Vec2 currentPos, in Vec2 newTargetPos)
		{
			bool flag = false;
			Mission.Current.Scene.SetAbilityOfFacesWithId(1, true);
			Vec3 vec;
			vec..ctor(currentPos, 0f, -1f);
			Vec3 vec2;
			vec2..ctor(newTargetPos, 0f, -1f);
			UIntPtr nearestNavigationMeshForPosition = Mission.Current.Scene.GetNearestNavigationMeshForPosition(ref vec, 1000000f, true);
			UIntPtr nearestNavigationMeshForPosition2 = Mission.Current.Scene.GetNearestNavigationMeshForPosition(ref vec2, 1000000f, true);
			if (nearestNavigationMeshForPosition == UIntPtr.Zero || nearestNavigationMeshForPosition2 == UIntPtr.Zero)
			{
				flag = true;
			}
			else if (nearestNavigationMeshForPosition != this._lastNavPathStartFace || nearestNavigationMeshForPosition2 != this._lastNavPathTargetFace)
			{
				flag = true;
			}
			Mission.Current.Scene.SetAbilityOfFacesWithId(1, false);
			return flag;
		}

		// Token: 0x060009F5 RID: 2549 RVA: 0x00046318 File Offset: 0x00044518
		private void UpdateNavigationPath(in NavalState currentState)
		{
			Vec2[] pathPoints = this._navigationPath.PathPoints;
			int num = this._navigationPath.Size - 1;
			NavalState navalState = currentState;
			Vec2 position = navalState.Position;
			while (this._lastNavPathPointIndex < num)
			{
				int lastNavPathPointIndex = this._lastNavPathPointIndex;
				int num2 = lastNavPathPointIndex + 1;
				Vec2 vec = pathPoints[lastNavPathPointIndex];
				Vec2 vec2 = pathPoints[num2];
				Vec2 vec3 = position - vec;
				if (vec3.LengthSquared <= 900f)
				{
					this._lastNavPathPointIndex++;
				}
				else
				{
					Vec2 vec4 = vec2 - vec;
					if (vec3.DotProduct(vec4) <= 0f)
					{
						break;
					}
					this._lastNavPathPointIndex++;
				}
			}
		}

		// Token: 0x060009F6 RID: 2550 RVA: 0x000463CC File Offset: 0x000445CC
		private NavalState GetNextTargetStateOverPath()
		{
			if (this._lastNavPathPointIndex < this._navigationPath.Size - 1)
			{
				Vec2 vec = this._navigationPath[this._lastNavPathPointIndex];
				return new NavalState(in vec, (this._navigationPath[this._lastNavPathPointIndex + 1] - this._navigationPath[this._lastNavPathPointIndex]).RotationInRadians, 0f);
			}
			return this._targetState;
		}

		// Token: 0x060009F7 RID: 2551 RVA: 0x00046444 File Offset: 0x00044644
		private void ClearNavigationPathAux()
		{
			this._navigationPath.Size = 0;
			this._lastNavPathPointIndex = -1;
			this._lastNavPathStartFace = UIntPtr.Zero;
			this._lastNavPathTargetFace = UIntPtr.Zero;
			this._lastNavPathTargetPosition = Vec2.Zero;
			this._navPathTargetDriftAccumulator = 0f;
		}

		// Token: 0x040005AC RID: 1452
		public const float ProportionalControllerSamplingPeriod = 0.033333335f;

		// Token: 0x040005AD RID: 1453
		private const float LateralInputAccelerationThreshold = 0.01f;

		// Token: 0x040005AE RID: 1454
		private const float LongitudinalInputAccelerationThreshold = 0.01f;

		// Token: 0x040005AF RID: 1455
		private const float RaisedSailInputThresholdMultiplier = 0.2f;

		// Token: 0x040005B0 RID: 1456
		private const float FullSailInputThresholdMultiplier = 0.6f;

		// Token: 0x040005B1 RID: 1457
		private AIShipController.TargetMode _targetMode;

		// Token: 0x040005B2 RID: 1458
		private NavalState _targetState;

		// Token: 0x040005B3 RID: 1459
		private NavalVec _targetOffset;

		// Token: 0x040005B4 RID: 1460
		private bool _stopOnArrival;

		// Token: 0x040005B5 RID: 1461
		private bool _ignoreTargetShipCollision;

		// Token: 0x040005B7 RID: 1463
		private uint _rowerLateralDebounceCounter;

		// Token: 0x040005B8 RID: 1464
		private uint _rowerLongitudinalDebounceCounter;

		// Token: 0x040005B9 RID: 1465
		private uint _rudderLateralDebounceCounter;

		// Token: 0x040005BA RID: 1466
		private uint _sailDebounceCounter;

		// Token: 0x040005BB RID: 1467
		private ShipInputRecord _inputRecord;

		// Token: 0x040005BC RID: 1468
		private NavalShipsLogic _navalShipsLogic;

		// Token: 0x040005BD RID: 1469
		private NavigationPath _navigationPath;

		// Token: 0x040005BE RID: 1470
		private int _lastNavPathPointIndex = -1;

		// Token: 0x040005BF RID: 1471
		private UIntPtr _lastNavPathStartFace;

		// Token: 0x040005C0 RID: 1472
		private UIntPtr _lastNavPathTargetFace;

		// Token: 0x040005C1 RID: 1473
		private Vec2 _lastNavPathTargetPosition;

		// Token: 0x040005C2 RID: 1474
		private float _navPathTargetDriftAccumulator;

		// Token: 0x040005C3 RID: 1475
		private float _lastNavPathHardRecomputeTime;

		// Token: 0x040005C6 RID: 1478
		private bool _collisionChecksActive = true;

		// Token: 0x040005C7 RID: 1479
		private bool _avoidShipCollisions = true;

		// Token: 0x040005C8 RID: 1480
		private bool _avoidObstacleCollisions = true;

		// Token: 0x040005C9 RID: 1481
		private MBList<MissionShip> _shipCollisionIgnoreList = new MBList<MissionShip>();

		// Token: 0x02000209 RID: 521
		public enum TargetMode
		{
			// Token: 0x04000E9B RID: 3739
			None,
			// Token: 0x04000E9C RID: 3740
			Position,
			// Token: 0x04000E9D RID: 3741
			State,
			// Token: 0x04000E9E RID: 3742
			Ship,
			// Token: 0x04000E9F RID: 3743
			ShipOffset
		}
	}
}
