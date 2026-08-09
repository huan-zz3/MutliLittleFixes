using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NavalDLC.DWA;
using NavalDLC.Missions.NavalPhysics;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.Objects
{
	// Token: 0x020000A4 RID: 164
	public class ShipDWAAgentDelegate : IDWAAgentDelegate
	{
		// Token: 0x1700024F RID: 591
		// (get) Token: 0x06000CB7 RID: 3255 RVA: 0x00061A18 File Offset: 0x0005FC18
		// (set) Token: 0x06000CB8 RID: 3256 RVA: 0x00061A20 File Offset: 0x0005FC20
		public int Id { get; private set; }

		// Token: 0x17000250 RID: 592
		// (get) Token: 0x06000CB9 RID: 3257 RVA: 0x00061A29 File Offset: 0x0005FC29
		// (set) Token: 0x06000CBA RID: 3258 RVA: 0x00061A31 File Offset: 0x0005FC31
		public MissionShip OwnerShip { get; private set; }

		// Token: 0x17000251 RID: 593
		// (get) Token: 0x06000CBB RID: 3259 RVA: 0x00061A3A File Offset: 0x0005FC3A
		// (set) Token: 0x06000CBC RID: 3260 RVA: 0x00061A42 File Offset: 0x0005FC42
		public float ShapeOffsetY { get; private set; }

		// Token: 0x17000252 RID: 594
		// (get) Token: 0x06000CBD RID: 3261 RVA: 0x00061A4B File Offset: 0x0005FC4B
		// (set) Token: 0x06000CBE RID: 3262 RVA: 0x00061A53 File Offset: 0x0005FC53
		public float ShapeComOffsetY { get; private set; }

		// Token: 0x17000253 RID: 595
		// (get) Token: 0x06000CBF RID: 3263 RVA: 0x00061A5C File Offset: 0x0005FC5C
		public readonly ref DWAAgentState State
		{
			get
			{
				return ref this._state;
			}
		}

		// Token: 0x17000254 RID: 596
		// (get) Token: 0x06000CC0 RID: 3264 RVA: 0x00061A64 File Offset: 0x0005FC64
		public float NeighborDistance
		{
			get
			{
				return this._detectionRadius;
			}
		}

		// Token: 0x17000255 RID: 597
		// (get) Token: 0x06000CC1 RID: 3265 RVA: 0x00061A6C File Offset: 0x0005FC6C
		public float MaxLinearSpeed
		{
			get
			{
				return this.OwnerShip.MissionShipObject.MaxLinearSpeed;
			}
		}

		// Token: 0x17000256 RID: 598
		// (get) Token: 0x06000CC2 RID: 3266 RVA: 0x00061A7E File Offset: 0x0005FC7E
		public float MaxLinearAcceleration
		{
			get
			{
				return this.OwnerShip.MissionShipObject.MaxLinearAccel;
			}
		}

		// Token: 0x17000257 RID: 599
		// (get) Token: 0x06000CC3 RID: 3267 RVA: 0x00061A90 File Offset: 0x0005FC90
		public float MaxAngularSpeed
		{
			get
			{
				return this.OwnerShip.MissionShipObject.MaxAngularSpeed;
			}
		}

		// Token: 0x17000258 RID: 600
		// (get) Token: 0x06000CC4 RID: 3268 RVA: 0x00061AA2 File Offset: 0x0005FCA2
		public float MaxAngularAcceleration
		{
			get
			{
				return this.OwnerShip.MissionShipObject.MaxAngularAccel;
			}
		}

		// Token: 0x17000259 RID: 601
		// (get) Token: 0x06000CC5 RID: 3269 RVA: 0x00061AB4 File Offset: 0x0005FCB4
		bool IDWAAgentDelegate.AvoidAgentCollisions
		{
			get
			{
				return this.OwnerShip.IsAIControlled && this.OwnerShip.AIController.AvoidShipCollisions;
			}
		}

		// Token: 0x1700025A RID: 602
		// (get) Token: 0x06000CC6 RID: 3270 RVA: 0x00061AD5 File Offset: 0x0005FCD5
		bool IDWAAgentDelegate.AvoidObstacleCollisions
		{
			get
			{
				return this.OwnerShip.IsAIControlled && this.OwnerShip.AIController.AvoidObstacleCollisions;
			}
		}

		// Token: 0x06000CC7 RID: 3271 RVA: 0x00061AF8 File Offset: 0x0005FCF8
		public ShipDWAAgentDelegate(MissionShip ownerShip, in DWASimulatorParameters parameters)
		{
			this.Id = -1;
			this.OwnerShip = ownerShip;
			BoundingBox physicsBoundingBoxWithoutChildren = ownerShip.Physics.PhysicsBoundingBoxWithoutChildren;
			Vec3 physicsBoundingBoxSizeWithoutChildren = ownerShip.Physics.PhysicsBoundingBoxSizeWithoutChildren;
			Vec3 vec = (physicsBoundingBoxWithoutChildren.min + physicsBoundingBoxWithoutChildren.max) * 0.5f;
			this.ShapeOffsetY = vec.y;
			Vec3 localCenterOfMass = ownerShip.Physics.LocalCenterOfMass;
			this.ShapeComOffsetY = this.ShapeOffsetY - localCenterOfMass.y;
			this._state.ShapeHalfSize = new Vec2(physicsBoundingBoxSizeWithoutChildren.x / 2f, physicsBoundingBoxSizeWithoutChildren.y / 2f);
			this._state.ShapeOffset = new Vec2(0f, this.ShapeComOffsetY);
			DWASimulatorParameters dwasimulatorParameters = parameters;
			this.SetTimeHorizon(dwasimulatorParameters.TimeHorizon);
		}

		// Token: 0x06000CC8 RID: 3272 RVA: 0x00061BD0 File Offset: 0x0005FDD0
		void IDWAAgentDelegate.Initialize(int id)
		{
			this.Id = id;
			this.CacheDynamicParameters();
		}

		// Token: 0x06000CC9 RID: 3273 RVA: 0x00061BE0 File Offset: 0x0005FDE0
		void IDWAAgentDelegate.SetParameters(in DWASimulatorParameters parameters)
		{
			DWASimulatorParameters dwasimulatorParameters = parameters;
			this.SetTimeHorizon(dwasimulatorParameters.TimeHorizon);
		}

		// Token: 0x06000CCA RID: 3274 RVA: 0x00061C01 File Offset: 0x0005FE01
		float IDWAAgentDelegate.GetSafetyFactor()
		{
			return 1f;
		}

		// Token: 0x06000CCB RID: 3275 RVA: 0x00061C08 File Offset: 0x0005FE08
		bool IDWAAgentDelegate.CanPlanTrajectory()
		{
			return this.OwnerShip.IsAIControlled;
		}

		// Token: 0x06000CCC RID: 3276 RVA: 0x00061C1C File Offset: 0x0005FE1C
		bool IDWAAgentDelegate.HasArrivedAtTarget()
		{
			float num;
			float num2;
			return this._hasTarget && this.OwnerShip.AIController.HasArrivedAtTarget(out num, out num2);
		}

		// Token: 0x06000CCD RID: 3277 RVA: 0x00061C48 File Offset: 0x0005FE48
		bool IDWAAgentDelegate.IsAgentEligibleNeighbor(int targetAgentId, IDWAAgentDelegate targetAgentDelegate)
		{
			using (List<MissionShip>.Enumerator enumerator = this.OwnerShip.AIController.ShipCollisionIgnoreList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.DWAAgentId == targetAgentId)
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06000CCE RID: 3278 RVA: 0x00061CAC File Offset: 0x0005FEAC
		bool IDWAAgentDelegate.IsObstacleSegmentEligibleNeighbor(IDWAObstacleVertex obstacle1, IDWAObstacleVertex obstacle2)
		{
			return true;
		}

		// Token: 0x06000CCF RID: 3279 RVA: 0x00061CB0 File Offset: 0x0005FEB0
		void IDWAAgentDelegate.OnStateUpdate()
		{
			this.CacheDynamicParameters();
			this._hasTarget = false;
			if (this.OwnerShip.IsAIControlled)
			{
				Vec2 vec;
				Vec2 vec2;
				float num;
				this._hasTarget = this.OwnerShip.AIController.GetNextTarget(out vec, out vec2, out num);
				if (this._hasTarget)
				{
					this.CacheShipTrajectoryData(in vec, in vec2, num);
				}
			}
		}

		// Token: 0x06000CD0 RID: 3280 RVA: 0x00061D06 File Offset: 0x0005FF06
		void IDWAAgentDelegate.UpdateSelectedAction(float dV, float dOmega)
		{
			if (this.OwnerShip.IsAIControlled)
			{
				this.OwnerShip.AIController.UpdateTrajectory(dV, dOmega);
			}
			this._selectedAction = new ValueTuple<float, float>(dV, dOmega);
		}

		// Token: 0x06000CD1 RID: 3281 RVA: 0x00061D34 File Offset: 0x0005FF34
		float IDWAAgentDelegate.GetGoalDirection(out Vec2 goalDir)
		{
			goalDir = this._shipToTargetDir;
			return this._shipToTargetDistance;
		}

		// Token: 0x06000CD2 RID: 3282 RVA: 0x00061D48 File Offset: 0x0005FF48
		[return: TupleElementNames(new string[] { "dV", "dOmega" })]
		ValueTuple<float, float> IDWAAgentDelegate.GetSelectedAction()
		{
			return this._selectedAction;
		}

		// Token: 0x06000CD3 RID: 3283 RVA: 0x00061D50 File Offset: 0x0005FF50
		float IDWAAgentDelegate.ComputeGoalCost(int sampleIndex, in DWAAgentState sampleState, [TupleElementNames(new string[] { "distance", "amount" })] ValueTuple<float, float> targetOcclusion)
		{
			if (!this._hasTarget)
			{
				return 0f;
			}
			float num = 8f;
			float num2 = 16f;
			float num3 = 16f;
			float num4 = 1f;
			float num5 = 0.5f;
			float num6 = 0.1f;
			DWAAgentState dwaagentState = sampleState;
			Vec2 shapeCenter = dwaagentState.ShapeCenter;
			Vec2 direction = sampleState.Direction;
			Vec2 linearVelocity = sampleState.LinearVelocity;
			Vec2 vec2;
			Vec2 vec = (vec2 = this._targetPos - shapeCenter);
			float num7 = vec2.Normalize();
			if (num7 <= 1E-06f)
			{
				vec2 = this._targetHeadingDir;
			}
			float num8 = MathF.Abs(Vec2.DotProduct(vec, this._shipToTargetNormalDir));
			float x = sampleState.ShapeHalfSize.x;
			float y = sampleState.ShapeHalfSize.y;
			float num9 = 2f * y;
			float num10 = MBMath.SmoothStep(0.15f, 0.85f, targetOcclusion.Item2);
			float num11 = DWAHelpers.GateNear(targetOcclusion.Item1, num9, num9);
			float num12 = MathF.Clamp(num10 * num11, 0f, 1f);
			float num13 = 1f - num12;
			float num14 = this._timeHorizon * this.MaxLinearSpeed;
			float num15 = this._shipToTargetDistance - num7;
			float num16 = 0f;
			if (num15 >= 0f)
			{
				float num17 = MathF.Min(this._shipToTargetDistance, num14);
				num16 = MathF.Clamp(num15 / MathF.Max(num17, 0.001f), 0f, 1f);
			}
			float num18 = num * (1f - num16);
			float num19 = 0f;
			if (num15 < 0f)
			{
				num19 = -num15 / num14;
				num19 = MathF.Min(num19, 1f);
			}
			float num20 = num2 * num19;
			float num21 = MathF.Clamp(Vec2.DotProduct(direction, this._targetHeadingDir), -1f, 1f);
			float num22 = MathF.Clamp(Vec2.DotProduct(direction, vec2), -1f, 1f);
			float num23 = 0.5f * (1f - num21);
			float num24 = 0.5f * (1f - num22);
			float num25 = DWAHelpers.GateNear(num8, 0.5f * x, 0.5f * x);
			float num26 = num25 * num23 + (1f - num25) * num24;
			float num27 = 0.2f + 0.8f * DWAHelpers.GateNear(num7, 2.5f * num9, x);
			float num28 = num3 * (num13 * num13) * num27 * num26;
			float num29 = MathF.Clamp(Vec2.DotProduct(linearVelocity, direction) / this.MaxLinearSpeed, -1f, 1f);
			float num30 = DWAHelpers.GateFar(num7, 2f * num9, num9);
			float num31 = num4 * num13 * num30 * MathF.Max(0f, -num29);
			float num32 = Vec2.DotProduct(linearVelocity, this._targetHeadingDir);
			float num33 = MathF.Abs(this._targetSpeed - num32) / this.MaxLinearSpeed;
			float num34 = DWAHelpers.GateNear(num7, 3f * num9, num9);
			float num35 = num5 * num13 * num34 * MathF.Clamp(num33, 0f, 1f);
			Vec2 vec3 = this.OwnerShip.Scene.GetGlobalWindVelocity();
			if (vec3.Normalize() <= 1E-06f)
			{
				vec3 = this._targetHeadingDir;
			}
			float num36 = MathF.Clamp(Vec2.DotProduct(direction, vec3), -1f, 1f);
			float num37 = 0.5f * (1f - num36);
			float num38 = DWAHelpers.GateFar(num7, 2f * num9, num9);
			float num39 = num6 * num13 * num38 * num37;
			return num18 + num20 + num28 + num31 + num35 + num39;
		}

		// Token: 0x06000CD4 RID: 3284 RVA: 0x000620B4 File Offset: 0x000602B4
		void IDWAAgentDelegate.ComputeExternalAccelerationsOnState(float dt, in DWAAgentState state, out Vec2 extLinearAcc, out float extAngularAcc)
		{
			extLinearAcc = Vec2.Zero;
			extAngularAcc = 0f;
			MatrixFrame matrixFrame = default(MatrixFrame);
			DWAAgentState dwaagentState = state;
			matrixFrame.origin = dwaagentState.Position3D;
			Vec2 vec = state.Direction;
			matrixFrame.rotation.f = vec.ToVec3(0f);
			matrixFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
			NavalPhysics physics = this.OwnerShip.Physics;
			NavalPhysics.BuoyancyComputationResult buoyancyComputationResult = default(NavalPhysics.BuoyancyComputationResult);
			buoyancyComputationResult.NetGlobalBuoyancyForce = physics.Mass * -MBGlobals.GravitationalAcceleration;
			buoyancyComputationResult.SimulatingAirFriction = true;
			buoyancyComputationResult.SubmergedHeightFactor = 1f;
			buoyancyComputationResult.SubmergedFloaterCountFactor = 1f;
			buoyancyComputationResult.PitchSubmergedAreaFactor = 1f;
			buoyancyComputationResult.RollSubmergedAreaFactor = 1f;
			NavalPhysics.DragForceComputationResult dragForceComputationResult = default(NavalPhysics.DragForceComputationResult);
			MatrixFrame matrixFrame2;
			matrixFrame2.rotation = matrixFrame.rotation;
			Vec3 vec2 = physics.LocalCenterOfMass;
			matrixFrame2.origin = matrixFrame.TransformToParent(ref vec2);
			int num = 1;
			vec2 = state.AngularVelocity * Vec3.Up;
			Vec3 vec3 = physics.MassSpaceInertia;
			NavalPhysics.ComputeAngularDrag(dt, num, in vec2, in matrixFrame2, in vec3, physics.PhysicsParameters, in buoyancyComputationResult, in physics.AngularDragTerm, in physics.AngularDampingTerm, physics.AngularDragYSideComponentTerm, physics.AngularDampingYSideComponentTerm, ref dragForceComputationResult);
			int num2 = 1;
			vec = state.LinearVelocity;
			vec2 = vec.ToVec3(0f);
			float mass = physics.Mass;
			vec3 = physics.LocalCenterOfMass;
			readonly ref NavalPhysics.NavalPhysicsParameters physicsParameters = ref physics.PhysicsParameters;
			LinearFrictionTerm linearDragTerm = physics.LinearDragTerm;
			LinearFrictionTerm linearDampingTerm = physics.LinearDampingTerm;
			LinearFrictionTerm constantLinearDampingTerm = physics.ConstantLinearDampingTerm;
			float num3;
			NavalPhysics.ComputeLinearDrag(dt, num2, in vec2, in matrixFrame, in mass, in vec3, in physicsParameters, in buoyancyComputationResult, in linearDragTerm, in linearDampingTerm, in constantLinearDampingTerm, physics.MinFloaterEntitialBottomPos, physics.MaxFloaterEntitialTopPos, ref dragForceComputationResult, out num3);
			extLinearAcc += (dragForceComputationResult.LateralDragForceGlobal.AsVec2 + dragForceComputationResult.LongitudinalDragForceGlobal.AsVec2) / physics.Mass;
			extAngularAcc += dragForceComputationResult.AngularDragTorqueGlobal.z / physics.MassSpaceInertia.z;
		}

		// Token: 0x06000CD5 RID: 3285 RVA: 0x000622C0 File Offset: 0x000604C0
		private void CacheDynamicParameters()
		{
			MatrixFrame globalFrame = this.OwnerShip.GlobalFrame;
			this._state.Position = globalFrame.origin.AsVec2;
			this._state.PositionZ = globalFrame.origin.z;
			this._state.Direction = globalFrame.rotation.f.AsVec2.Normalized();
			this._state.LinearVelocity = this.OwnerShip.Physics.LinearVelocity.AsVec2;
			this._state.AngularVelocity = this.OwnerShip.Physics.AngularVelocity.z;
		}

		// Token: 0x06000CD6 RID: 3286 RVA: 0x0006236D File Offset: 0x0006056D
		private static float ComputeDetectionRadius(float halfLength, float timeHorizon, float maxLinearSpeed)
		{
			return 4f * halfLength + timeHorizon * maxLinearSpeed;
		}

		// Token: 0x06000CD7 RID: 3287 RVA: 0x0006237C File Offset: 0x0006057C
		private void CacheShipTrajectoryData(in Vec2 targetPos, in Vec2 targetDir, float targetSpeed)
		{
			this._targetPos = targetPos;
			this._targetSpeed = targetSpeed;
			Vec2 vec = this.OwnerShip.GlobalFrame.rotation.f.AsVec2.Normalized();
			this._shipToTargetDir = this._targetPos - this.State.Position;
			this._shipToTargetDistance = this._shipToTargetDir.Normalize();
			if (this._shipToTargetDistance <= 1E-06f)
			{
				this._shipToTargetDir = vec;
				this._shipToTargetDistance = 0f;
			}
			this._targetHeadingDir = targetDir;
			if (this._targetHeadingDir.Normalize() <= 1E-06f)
			{
				this._targetHeadingDir = vec;
			}
			this._dotShipFwdToTargetHeading = Vec2.DotProduct(vec, this._targetHeadingDir);
			Vec2 vec2 = this.State.Position - this._targetPos;
			Vec2 vec3 = Vec2.DotProduct(vec2, this._targetHeadingDir) * this._targetHeadingDir;
			Vec2 vec4 = vec2 - vec3;
			if (vec3.LengthSquared >= 1E-05f)
			{
				this._shipToTargetTangentDir = -vec3;
				this._shipToTargetTangentDir.Normalize();
			}
			else
			{
				this._shipToTargetTangentDir = -this._targetHeadingDir;
			}
			if (vec4.LengthSquared >= 1E-05f)
			{
				this._shipToTargetNormalDir = -vec4;
				return;
			}
			this._shipToTargetNormalDir = (-this._targetHeadingDir).LeftVec();
		}

		// Token: 0x06000CD8 RID: 3288 RVA: 0x000624E9 File Offset: 0x000606E9
		private void SetTimeHorizon(float timeHorizon)
		{
			this._timeHorizon = timeHorizon;
			this._detectionRadius = ShipDWAAgentDelegate.ComputeDetectionRadius(this._state.ShapeHalfSize.y, timeHorizon, this.OwnerShip.MissionShipObject.MaxLinearSpeed);
		}

		// Token: 0x040007A1 RID: 1953
		private DWAAgentState _state;

		// Token: 0x040007A2 RID: 1954
		private float _detectionRadius;

		// Token: 0x040007A3 RID: 1955
		private bool _hasTarget;

		// Token: 0x040007A4 RID: 1956
		private Vec2 _targetPos;

		// Token: 0x040007A5 RID: 1957
		private Vec2 _targetHeadingDir;

		// Token: 0x040007A6 RID: 1958
		private Vec2 _shipToTargetDir;

		// Token: 0x040007A7 RID: 1959
		private Vec2 _shipToTargetNormalDir;

		// Token: 0x040007A8 RID: 1960
		private Vec2 _shipToTargetTangentDir;

		// Token: 0x040007A9 RID: 1961
		private float _dotShipFwdToTargetHeading;

		// Token: 0x040007AA RID: 1962
		private float _targetSpeed;

		// Token: 0x040007AB RID: 1963
		private float _shipToTargetDistance;

		// Token: 0x040007AC RID: 1964
		private float _timeHorizon;

		// Token: 0x040007AD RID: 1965
		[TupleElementNames(new string[] { "dV", "dOmega" })]
		private ValueTuple<float, float> _selectedAction;
	}
}
