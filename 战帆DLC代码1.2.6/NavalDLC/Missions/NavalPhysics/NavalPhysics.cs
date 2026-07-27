using System;
using System.Linq;
using System.Runtime.InteropServices;
using NavalDLC.Missions.Objects;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.NavalPhysics
{
	// Token: 0x020000C0 RID: 192
	[ScriptComponentParams("ship_visual_only", "")]
	public class NavalPhysics : ScriptComponentBehavior
	{
		// Token: 0x17000299 RID: 665
		// (get) Token: 0x06000E4B RID: 3659 RVA: 0x0006F50E File Offset: 0x0006D70E
		// (set) Token: 0x06000E4C RID: 3660 RVA: 0x0006F516 File Offset: 0x0006D716
		public bool IsInitialized { get; private set; }

		// Token: 0x1700029A RID: 666
		// (get) Token: 0x06000E4D RID: 3661 RVA: 0x0006F51F File Offset: 0x0006D71F
		// (set) Token: 0x06000E4E RID: 3662 RVA: 0x0006F527 File Offset: 0x0006D727
		public Vec3 PhysicsBoundingBoxWithChildrenSize { get; private set; }

		// Token: 0x1700029B RID: 667
		// (get) Token: 0x06000E4F RID: 3663 RVA: 0x0006F530 File Offset: 0x0006D730
		// (set) Token: 0x06000E50 RID: 3664 RVA: 0x0006F538 File Offset: 0x0006D738
		public Vec3 PhysicsBoundingBoxSizeWithoutChildren { get; private set; }

		// Token: 0x1700029C RID: 668
		// (get) Token: 0x06000E51 RID: 3665 RVA: 0x0006F541 File Offset: 0x0006D741
		// (set) Token: 0x06000E52 RID: 3666 RVA: 0x0006F549 File Offset: 0x0006D749
		public BoundingBox PhysicsBoundingBoxWithChildren { get; private set; }

		// Token: 0x1700029D RID: 669
		// (get) Token: 0x06000E53 RID: 3667 RVA: 0x0006F552 File Offset: 0x0006D752
		// (set) Token: 0x06000E54 RID: 3668 RVA: 0x0006F55A File Offset: 0x0006D75A
		public BoundingBox PhysicsBoundingBoxWithoutChildren { get; private set; }

		// Token: 0x1700029E RID: 670
		// (get) Token: 0x06000E55 RID: 3669 RVA: 0x0006F563 File Offset: 0x0006D763
		public float Mass
		{
			get
			{
				return this._cachedMass;
			}
		}

		// Token: 0x1700029F RID: 671
		// (get) Token: 0x06000E56 RID: 3670 RVA: 0x0006F56C File Offset: 0x0006D76C
		public Vec3 LocalCenterOfMass
		{
			get
			{
				return base.GameEntity.CenterOfMass;
			}
		}

		// Token: 0x170002A0 RID: 672
		// (get) Token: 0x06000E57 RID: 3671 RVA: 0x0006F587 File Offset: 0x0006D787
		public Vec3 MassSpaceInertia
		{
			get
			{
				return GameEntityPhysicsExtensions.GetMassSpaceInertia(base.GameEntity);
			}
		}

		// Token: 0x170002A1 RID: 673
		// (get) Token: 0x06000E58 RID: 3672 RVA: 0x0006F594 File Offset: 0x0006D794
		public readonly ref NavalPhysics.NavalPhysicsParameters PhysicsParameters
		{
			get
			{
				return ref this._physicsParameters;
			}
		}

		// Token: 0x170002A2 RID: 674
		// (get) Token: 0x06000E59 RID: 3673 RVA: 0x0006F59C File Offset: 0x0006D79C
		// (set) Token: 0x06000E5A RID: 3674 RVA: 0x0006F5A4 File Offset: 0x0006D7A4
		public NavalPhysics.SinkingState NavalSinkingState { get; private set; }

		// Token: 0x170002A3 RID: 675
		// (get) Token: 0x06000E5B RID: 3675 RVA: 0x0006F5AD File Offset: 0x0006D7AD
		private float StabilitySubmergedVolume
		{
			get
			{
				return this.Mass / (NavalPhysics.GetWaterDensity() * this._physicsParameters.FloatingForceMultiplier);
			}
		}

		// Token: 0x170002A4 RID: 676
		// (get) Token: 0x06000E5C RID: 3676 RVA: 0x0006F5C7 File Offset: 0x0006D7C7
		public float FloatingForceMultiplierWhenDamaged
		{
			get
			{
				return this.StabilitySubmergedVolume / (this._totalFloaterVolumeCached * this._physicsParameters.MaximumSubmergedVolumeRatio);
			}
		}

		// Token: 0x170002A5 RID: 677
		// (get) Token: 0x06000E5D RID: 3677 RVA: 0x0006F5E2 File Offset: 0x0006D7E2
		// (set) Token: 0x06000E5E RID: 3678 RVA: 0x0006F5EA File Offset: 0x0006D7EA
		public float StabilitySubmergedHeightOfShip { get; private set; }

		// Token: 0x170002A6 RID: 678
		// (get) Token: 0x06000E5F RID: 3679 RVA: 0x0006F5F3 File Offset: 0x0006D7F3
		// (set) Token: 0x06000E60 RID: 3680 RVA: 0x0006F5FB File Offset: 0x0006D7FB
		public float LastSubmergedHeightFactorForActuators { get; private set; }

		// Token: 0x170002A7 RID: 679
		// (get) Token: 0x06000E61 RID: 3681 RVA: 0x0006F604 File Offset: 0x0006D804
		// (set) Token: 0x06000E62 RID: 3682 RVA: 0x0006F60C File Offset: 0x0006D80C
		public LinearFrictionTerm LinearDragTerm { get; private set; }

		// Token: 0x170002A8 RID: 680
		// (get) Token: 0x06000E63 RID: 3683 RVA: 0x0006F615 File Offset: 0x0006D815
		// (set) Token: 0x06000E64 RID: 3684 RVA: 0x0006F61D File Offset: 0x0006D81D
		public LinearFrictionTerm LinearDampingTerm { get; private set; }

		// Token: 0x170002A9 RID: 681
		// (get) Token: 0x06000E65 RID: 3685 RVA: 0x0006F626 File Offset: 0x0006D826
		public float MinFloaterEntitialBottomPos
		{
			get
			{
				return this._minFloaterEntitialBottomPos;
			}
		}

		// Token: 0x170002AA RID: 682
		// (get) Token: 0x06000E66 RID: 3686 RVA: 0x0006F62E File Offset: 0x0006D82E
		public float MaxFloaterEntitialTopPos
		{
			get
			{
				return this._maxFloaterEntitialTopPos;
			}
		}

		// Token: 0x170002AB RID: 683
		// (get) Token: 0x06000E67 RID: 3687 RVA: 0x0006F636 File Offset: 0x0006D836
		public float AngularDragYSideComponentTerm
		{
			get
			{
				return this._angularDragYSideComponentTerm;
			}
		}

		// Token: 0x170002AC RID: 684
		// (get) Token: 0x06000E68 RID: 3688 RVA: 0x0006F63E File Offset: 0x0006D83E
		// (set) Token: 0x06000E69 RID: 3689 RVA: 0x0006F646 File Offset: 0x0006D846
		public LinearFrictionTerm ConstantLinearDampingTerm { get; private set; }

		// Token: 0x170002AD RID: 685
		// (get) Token: 0x06000E6A RID: 3690 RVA: 0x0006F64F File Offset: 0x0006D84F
		public float AngularDampingYSideComponentTerm
		{
			get
			{
				return this._angularDampingYSideComponentTerm;
			}
		}

		// Token: 0x170002AE RID: 686
		// (get) Token: 0x06000E6B RID: 3691 RVA: 0x0006F657 File Offset: 0x0006D857
		public Vec3 LinearVelocity
		{
			get
			{
				return GameEntityPhysicsExtensions.GetLinearVelocity(base.GameEntity);
			}
		}

		// Token: 0x170002AF RID: 687
		// (get) Token: 0x06000E6C RID: 3692 RVA: 0x0006F664 File Offset: 0x0006D864
		public Vec3 AngularVelocity
		{
			get
			{
				return GameEntityPhysicsExtensions.GetAngularVelocity(base.GameEntity);
			}
		}

		// Token: 0x170002B0 RID: 688
		// (get) Token: 0x06000E6D RID: 3693 RVA: 0x0006F671 File Offset: 0x0006D871
		// (set) Token: 0x06000E6E RID: 3694 RVA: 0x0006F679 File Offset: 0x0006D879
		public bool IsAnchored { get; private set; }

		// Token: 0x170002B1 RID: 689
		// (get) Token: 0x06000E6F RID: 3695 RVA: 0x0006F682 File Offset: 0x0006D882
		public MatrixFrame AnchorGlobalFrame
		{
			get
			{
				return this._anchorGlobalFrame;
			}
		}

		// Token: 0x06000E71 RID: 3697 RVA: 0x0006F6B4 File Offset: 0x0006D8B4
		protected override void OnEditorInit()
		{
			this._ownScene = base.GameEntity.Scene;
			if (this._ownScene.GetEnginePhysicsEnabled())
			{
				if (!this.IsInitialized && !base.GameEntity.HasScriptOfType<MissionShip>())
				{
					this.OnInit();
					return;
				}
			}
			else
			{
				this.IsInitialized = false;
			}
		}

		// Token: 0x06000E72 RID: 3698 RVA: 0x0006F708 File Offset: 0x0006D908
		protected override void OnInit()
		{
			if (!this.IsInitialized && base.GameEntity.GetFirstScriptOfType<MissionShip>() == null)
			{
				this.StabilitySubmergedHeightOfShip = 0f;
				this._weightedAgentsPosition = Vec3.Zero;
				this._totalMass = 0f;
				this._committedWeightedAgentsPosition = Vec3.Zero;
				this._committedTotalMass = 0f;
				CustomNavalPhysicsParameters customNavalPhysicsParameters = base.GameEntity.GetFirstScriptOfType<CustomNavalPhysicsParameters>() ?? new CustomNavalPhysicsParameters();
				ShipPhysicsReference shipPhysicsReference = (customNavalPhysicsParameters.BehaveLikeShip ? ShipPhysicsReference.Default : ShipPhysicsReference.DefaultDebris);
				NavalPhysics.NavalPhysicsParameters navalPhysicsParameters = new NavalPhysics.NavalPhysicsParameters
				{
					OverrideMass = 0f,
					MassMultiplier = 1f,
					MomentOfInertiaMultiplier = Vec3.One,
					FloatingForceMultiplier = customNavalPhysicsParameters.FloatingForceMultiplier,
					LinearFrictionMultiplier = new LinearFrictionTerm(customNavalPhysicsParameters.LinearFrictionMultiplierRight, customNavalPhysicsParameters.LinearFrictionMultiplierLeft, customNavalPhysicsParameters.LinearFrictionMultiplierForward, customNavalPhysicsParameters.LinearFrictionMultiplierBackward, customNavalPhysicsParameters.LinearFrictionMultiplierUp, customNavalPhysicsParameters.LinearFrictionMultiplierDown),
					AngularFrictionMultiplier = customNavalPhysicsParameters.AngularFrictionMultiplier,
					MaximumSubmergedVolumeRatio = 0.7f,
					ForwardDragMultiplier = 1f,
					TorqueMultiplierOfLateralBuoyantForces = 1f,
					TorqueMultiplierOfVerticalBuoyantForces = Vec3.One,
					UpSideDownFrictionMultiplier = 1f,
					MaxLinearSpeedForLateralDragCenterShift = 1E+09f,
					MaxLateralDragShift = 0f,
					LateralDragShiftCriticalAngle = 1f,
					StepAgentWeightMultiplier = 1f,
					MakeAgentsStepToEntityEvenUnderWater = false
				};
				this.Initialize(navalPhysicsParameters, shipPhysicsReference);
			}
			this._ownScene = base.GameEntity.Scene;
		}

		// Token: 0x06000E73 RID: 3699 RVA: 0x0006F8A0 File Offset: 0x0006DAA0
		protected override void OnPreInit()
		{
			WeakGameEntity firstChildEntityWithTag = base.GameEntity.GetFirstChildEntityWithTag("batched_physics_entity");
			if (firstChildEntityWithTag != WeakGameEntity.Invalid)
			{
				GameEntityPhysicsExtensions.CreateVariableRatePhysics(firstChildEntityWithTag, true);
			}
			foreach (WeakGameEntity weakGameEntity in base.GameEntity.GetChildren())
			{
				if (weakGameEntity != firstChildEntityWithTag)
				{
					GameEntityPhysicsExtensions.CreateVariableRatePhysics(weakGameEntity, true);
				}
			}
		}

		// Token: 0x06000E74 RID: 3700 RVA: 0x0006F928 File Offset: 0x0006DB28
		protected override void OnRemoved(int removeReason)
		{
			if (this._floaterVolumeDataPinnedPointer != UIntPtr.Zero)
			{
				this._floaterVolumeDataPinnedGCHandler.Free();
				this._floaterVolumeDataPinnedPointer = UIntPtr.Zero;
			}
		}

		// Token: 0x06000E75 RID: 3701 RVA: 0x0006F954 File Offset: 0x0006DB54
		public void Initialize(NavalPhysics.NavalPhysicsParameters physicsParameters, ShipPhysicsReference basePhysicsRef)
		{
			this._shipForceRecord = ShipForceRecord.None();
			this._continuousDriftForceData.Initialize();
			base.GameEntity.Scene.SetOnCollisionFilterCallbackActive(true);
			this.UpdateShipPhysics(physicsParameters, basePhysicsRef);
			this.LoadFloaterVolumes();
			this.PreComputeAngularDragTerms(out this.AngularDampingTerm, out this.AngularDragTerm, out this._angularDampingYSideComponentTerm, out this._angularDragYSideComponentTerm);
			if (!physicsParameters.MakeAgentsStepToEntityEvenUnderWater)
			{
				base.GameEntity.AddBodyFlags(-1073741824, true);
			}
			base.GameEntity.Scene.SetFixedTickCallbackActive(true);
			this.IsInitialized = true;
			this.IsAnchored = false;
			this._anchorGlobalFrame = MatrixFrame.Zero;
		}

		// Token: 0x06000E76 RID: 3702 RVA: 0x0006FA00 File Offset: 0x0006DC00
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return 52;
		}

		// Token: 0x06000E77 RID: 3703 RVA: 0x0006FA04 File Offset: 0x0006DC04
		protected override void OnSaveAsPrefab()
		{
		}

		// Token: 0x06000E78 RID: 3704 RVA: 0x0006FA06 File Offset: 0x0006DC06
		public static float GetAirDensity()
		{
			return GameModels.Instance.ShipPhysicsParametersModel.GetAirDensity();
		}

		// Token: 0x06000E79 RID: 3705 RVA: 0x0006FA17 File Offset: 0x0006DC17
		public static float GetWaterDensity()
		{
			return GameModels.Instance.ShipPhysicsParametersModel.GetWaterDensity();
		}

		// Token: 0x06000E7A RID: 3706 RVA: 0x0006FA28 File Offset: 0x0006DC28
		public void CheckPrefab()
		{
			float waterDensity = NavalPhysics.GetWaterDensity();
			float num = base.GameEntity.Mass * 9.806f * 1.01f;
			float num2 = this._totalFloaterVolumeCached * waterDensity * 9.806f * this._physicsParameters.FloatingForceMultiplier;
			if (num2 <= num)
			{
				base.GameEntity.GetFirstScriptOfType<MissionShip>();
			}
			float num3 = num2 * this.FloatingForceMultiplierWhenDamaged;
		}

		// Token: 0x06000E7B RID: 3707 RVA: 0x0006FA8D File Offset: 0x0006DC8D
		public void OnShipObjectUpdated(NavalPhysics.NavalPhysicsParameters physicsParameters, ShipPhysicsReference basePhysicsRef)
		{
			this.UpdateShipPhysics(physicsParameters, basePhysicsRef);
		}

		// Token: 0x06000E7C RID: 3708 RVA: 0x0006FA97 File Offset: 0x0006DC97
		public void SetShipForceRecord(in ShipForceRecord record)
		{
			this._shipForceRecord = record;
		}

		// Token: 0x06000E7D RID: 3709 RVA: 0x0006FAA5 File Offset: 0x0006DCA5
		public void SetContinuousDriftSpeed(float driftSpeed)
		{
			this._continuousDriftForceData.DriftSpeed = driftSpeed;
		}

		// Token: 0x06000E7E RID: 3710 RVA: 0x0006FAB4 File Offset: 0x0006DCB4
		public void SetAnchor(bool isAnchored, bool anchorInPlace = false, float forceMultiplier = 1f)
		{
			this.IsAnchored = isAnchored;
			if (this.IsAnchored)
			{
				if (this._anchorGlobalFrame.IsZero || anchorInPlace)
				{
					MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
					Vec2 asVec = globalFrame.origin.AsVec2;
					Vec2 vec = globalFrame.rotation.f.AsVec2.Normalized();
					this.SetAnchorFrame(in asVec, in vec, forceMultiplier);
					return;
				}
			}
			else
			{
				this._anchorGlobalFrame = MatrixFrame.Zero;
				this._anchorForceMultiplier = 1f;
			}
		}

		// Token: 0x06000E7F RID: 3711 RVA: 0x0006FB38 File Offset: 0x0006DD38
		public void SetAnchorFrame(in Vec2 position, in Vec2 direction, float forceMultiplier = 1f)
		{
			float waterLevelAtPosition = base.GameEntity.Scene.GetWaterLevelAtPosition(position, true, false);
			Vec2 vec = position;
			this._anchorGlobalFrame.origin = vec.ToVec3(waterLevelAtPosition);
			vec = direction;
			this._anchorGlobalFrame.rotation.f = vec.ToVec3(0f);
			this._anchorGlobalFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
			this._anchorForceMultiplier = forceMultiplier;
		}

		// Token: 0x06000E80 RID: 3712 RVA: 0x0006FBB8 File Offset: 0x0006DDB8
		protected override void OnParallelFixedTick(float fixedDt)
		{
			if (!GameEntityPhysicsExtensions.HasDynamicRigidBodyAndActiveSimulation(base.GameEntity))
			{
				return;
			}
			base.GameEntity.GetBodyWorldTransform();
			TWSharedMutexReadLock twsharedMutexReadLock;
			twsharedMutexReadLock..ctor(Scene.PhysicsAndRayCastLock);
			Vec3 linearVelocity;
			Vec3 angularVelocity;
			Vec3 massSpaceInertia;
			try
			{
				linearVelocity = this.LinearVelocity;
				angularVelocity = this.AngularVelocity;
				massSpaceInertia = this.MassSpaceInertia;
			}
			finally
			{
				twsharedMutexReadLock.Dispose();
			}
			if (this.IsAnchored)
			{
				float waterLevelAtPosition = base.GameEntity.Scene.GetWaterLevelAtPosition(this._anchorGlobalFrame.origin.AsVec2, true, false);
				this._anchorGlobalFrame.origin.z = waterLevelAtPosition;
			}
			this.UpdateFloaterVolumeData();
			this.TickFloaterDurabilities(fixedDt);
			this.FillWaterHeightQueryResultsIterative();
			this.ComputeBuoyancyForces(fixedDt, in linearVelocity, in angularVelocity);
			this.ComputeDragForces(fixedDt, in linearVelocity, in angularVelocity, in massSpaceInertia);
			this.ComputeContinuousDriftForce(fixedDt);
		}

		// Token: 0x06000E81 RID: 3713 RVA: 0x0006FC98 File Offset: 0x0006DE98
		protected override void OnFixedTick(float fixedDt)
		{
			if (!GameEntityPhysicsExtensions.HasDynamicRigidBodyAndActiveSimulation(base.GameEntity))
			{
				return;
			}
			this.LastSubmergedHeightFactorForActuators = this._buoyancyComputationResult.SubmergedHeightFactor;
			Vec3 vec = MBGlobals.GravitationalAcceleration * this.Mass;
			this.ApplyForceToDynamicBody(in vec, 0);
			this.ApplyAgentForces();
			this.ApplyBuoyancyForces();
			this.ApplyDragForces();
			this.ApplyActuatorForces();
			this.ApplyAnchorForces();
			this.ApplyContinuousDriftForce();
		}

		// Token: 0x06000E82 RID: 3714 RVA: 0x0006FD04 File Offset: 0x0006DF04
		protected override void OnEditorTick(float dt)
		{
			if (base.GameEntity.Scene.GetEnginePhysicsEnabled())
			{
				if (!this.IsInitialized && !base.GameEntity.HasScriptOfType<MissionShip>())
				{
					this.OnInit();
					return;
				}
			}
			else
			{
				this.IsInitialized = false;
			}
		}

		// Token: 0x06000E83 RID: 3715 RVA: 0x0006FD4C File Offset: 0x0006DF4C
		public void ApplyGlobalForceAtLocalPos(in Vec3 localPos, in Vec3 globalForceVec, GameEntityPhysicsExtensions.ForceMode forceMode = 0)
		{
			GameEntityPhysicsExtensions.ApplyGlobalForceAtLocalPosToDynamicBody(base.GameEntity, localPos, globalForceVec, forceMode);
		}

		// Token: 0x06000E84 RID: 3716 RVA: 0x0006FD66 File Offset: 0x0006DF66
		public void ApplyLocalForceAtLocalPos(in Vec3 localPos, in Vec3 localForceVec, GameEntityPhysicsExtensions.ForceMode forceMode = 0)
		{
			GameEntityPhysicsExtensions.ApplyLocalForceAtLocalPosToDynamicBody(base.GameEntity, localPos, localForceVec, forceMode);
		}

		// Token: 0x06000E85 RID: 3717 RVA: 0x0006FD80 File Offset: 0x0006DF80
		public void ApplyForceToDynamicBody(in Vec3 forceVec, GameEntityPhysicsExtensions.ForceMode forceMode = 0)
		{
			GameEntityPhysicsExtensions.ApplyForceToDynamicBody(base.GameEntity, forceVec, forceMode);
		}

		// Token: 0x06000E86 RID: 3718 RVA: 0x0006FD94 File Offset: 0x0006DF94
		public void ApplyTorque(in Vec3 torqueVec, GameEntityPhysicsExtensions.ForceMode forceMode = 0)
		{
			GameEntityPhysicsExtensions.ApplyTorqueToDynamicBody(base.GameEntity, torqueVec, forceMode);
		}

		// Token: 0x06000E87 RID: 3719 RVA: 0x0006FDA8 File Offset: 0x0006DFA8
		public MatrixFrame GetGlobalMassFrame()
		{
			MatrixFrame bodyWorldTransform = base.GameEntity.GetBodyWorldTransform();
			MatrixFrame matrixFrame;
			matrixFrame.rotation = bodyWorldTransform.rotation;
			Vec3 localCenterOfMass = this.LocalCenterOfMass;
			matrixFrame.origin = bodyWorldTransform.TransformToParent(ref localCenterOfMass);
			return matrixFrame;
		}

		// Token: 0x06000E88 RID: 3720 RVA: 0x0006FDEC File Offset: 0x0006DFEC
		public Vec3 GetClosestPointToBoundingBox(in Vec3 localPoint)
		{
			Vec3 min = this.PhysicsBoundingBoxWithoutChildren.min;
			Vec3 max = this.PhysicsBoundingBoxWithoutChildren.max;
			float num = Math.Max(min.x, Math.Min(max.x, localPoint.x));
			float num2 = Math.Max(min.y, Math.Min(max.y, localPoint.y));
			float num3 = Math.Max(min.z, Math.Min(max.z, localPoint.z));
			return new Vec3(num, num2, num3, -1f);
		}

		// Token: 0x06000E89 RID: 3721 RVA: 0x0006FE73 File Offset: 0x0006E073
		public void SetTargetDurabilityOfPart(int part, float targetDurability)
		{
			this._shipPartsTargetDurabilities[part] = MathF.Max(0.01f, MathF.Min(this._shipPartsTargetDurabilities[part], targetDurability));
		}

		// Token: 0x06000E8A RID: 3722 RVA: 0x0006FE98 File Offset: 0x0006E098
		private void SetTargetDurabilityToAdjacentParts(int part, float targetDurability)
		{
			if (part - 1 >= 0 && part % 2 - (part - 1) % 2 == 1)
			{
				this.SetTargetDurabilityOfPart(part - 1, targetDurability);
			}
			if (part + 1 < 6 && (part + 1) % 2 - part % 2 == 1)
			{
				this.SetTargetDurabilityOfPart(part + 1, targetDurability);
			}
			if (part - 2 >= 0)
			{
				this.SetTargetDurabilityOfPart(part - 2, targetDurability);
			}
			if (part + 2 < 6)
			{
				this.SetTargetDurabilityOfPart(part + 2, targetDurability);
			}
		}

		// Token: 0x06000E8B RID: 3723 RVA: 0x0006FF00 File Offset: 0x0006E100
		private void TickFloaterDurabilities(float fixedDt)
		{
			float floatingForceMultiplierWhenDamaged = this.FloatingForceMultiplierWhenDamaged;
			for (int i = 0; i < 6; i++)
			{
				float num = ((this.NavalSinkingState != NavalPhysics.SinkingState.Floating) ? this._shipPartsTargetDurabilities[i] : MathF.Max(this._shipPartsTargetDurabilities[i], floatingForceMultiplierWhenDamaged));
				if (num < this._shipPartsDurabilities[i])
				{
					this._shipPartsDurabilities[i] = MathF.Max(num, this._shipPartsDurabilities[i] - 0.2f * fixedDt * MathF.Max(0.5f, this._shipPartsDurabilities[i]));
					float num2 = ((this._shipPartsDurabilities[i] <= 0.01f) ? 0.01f : MathF.Min(1f, 1f - (1f - this._shipPartsDurabilities[i]) / 2f));
					this.SetTargetDurabilityToAdjacentParts(i, num2);
				}
			}
		}

		// Token: 0x06000E8C RID: 3724 RVA: 0x0006FFC6 File Offset: 0x0006E1C6
		protected override bool CanPhysicsCollideBetweenTwoEntities(WeakGameEntity myEntity, BodyFlags myEntityBodyFlags, WeakGameEntity otherEntity, BodyFlags otherEntityBodyFlags)
		{
			if (!Extensions.HasAnyFlag<BodyFlags>(myEntityBodyFlags, -2147483648))
			{
				return true;
			}
			if (!Extensions.HasAnyFlag<BodyFlags>(otherEntityBodyFlags, 16))
			{
				return true;
			}
			if (Extensions.HasAnyFlag<BodyFlags>(otherEntityBodyFlags, -2147483648))
			{
				return Extensions.HasAnyFlag<BodyFlags>(otherEntityBodyFlags, 8);
			}
			return !Extensions.HasAnyFlag<BodyFlags>(otherEntityBodyFlags, 8);
		}

		// Token: 0x06000E8D RID: 3725 RVA: 0x00070008 File Offset: 0x0006E208
		private void FillWaterHeightQueryResultsIterative()
		{
			MatrixFrame bodyWorldTransform = base.GameEntity.GetBodyWorldTransform();
			this._ownScene.GetBulkWaterLevelAtVolumes(this._floaterVolumeDataPinnedPointer, this._floaterVolumeData.Length, ref bodyWorldTransform);
		}

		// Token: 0x06000E8E RID: 3726 RVA: 0x00070040 File Offset: 0x0006E240
		private static ValueTuple<float, float> RungeKuttaIntegrationStepForBuoyancyAndGravity(float prevIterationUpSpeed, float prevIterationUpAcceleration, float baseShipUpSpeed, float fixedDt, float baseSubmergedHeight, float volumeHeight, float volumeWidthMultDepth, float waterDensity, float durabilityMultiplier, float curInvVolumeMass)
		{
			float num = prevIterationUpSpeed * fixedDt;
			float num2 = MathF.Clamp(baseSubmergedHeight - num, 0f, volumeHeight) * volumeWidthMultDepth * waterDensity * 9.806f * durabilityMultiplier * curInvVolumeMass + -9.806f;
			float num3 = baseShipUpSpeed + fixedDt * prevIterationUpAcceleration;
			return new ValueTuple<float, float>(num2, num3);
		}

		// Token: 0x06000E8F RID: 3727 RVA: 0x00070088 File Offset: 0x0006E288
		private void ComputeBuoyancyForces(float fixedDt, in Vec3 globalLinearVelocity, in Vec3 globalAngularVelocity)
		{
			MatrixFrame bodyWorldTransform = base.GameEntity.GetBodyWorldTransform();
			MatrixFrame globalMassFrame = this.GetGlobalMassFrame();
			float waterDensity = NavalPhysics.GetWaterDensity();
			float z = globalLinearVelocity.z;
			float num = this.Mass / this._totalFloaterVolumeCached;
			float num2;
			if (this.NavalSinkingState == NavalPhysics.SinkingState.Floating)
			{
				num2 = this._minimumFloaterDurabilityToFloatWhileNotSinking;
			}
			else
			{
				num2 = 0f;
			}
			this._buoyancyComputationResult.Reset();
			int num3 = 0;
			Vec3 localCenterOfMass = this.LocalCenterOfMass;
			float floatingForceMultiplier = this._physicsParameters.FloatingForceMultiplier;
			float num4 = 0f;
			Vec3 vec = Vec3.Zero;
			float num5 = 0f;
			Vec3 vec2 = globalMassFrame.rotation.TransformToLocal(ref globalAngularVelocity);
			Mat3 identity = Mat3.Identity;
			Vec3 vec3 = bodyWorldTransform.rotation.TransformToLocal(ref Vec3.Up);
			float num6 = 0f;
			float num7 = 0f;
			float num8 = (this.LinearDampingTerm.Up / this.LinearDampingTerm.Down + this.LinearDragTerm.Up / this.LinearDragTerm.Down) * 0.5f;
			for (int i = 0; i < this._floaterVolumeData.Length; i++)
			{
				VolumeDataForSubmergeComputation volumeDataForSubmergeComputation = this._floaterVolumeData[i];
				float inOutWaterHeightWrtVolume = volumeDataForSubmergeComputation.InOutWaterHeightWrtVolume;
				if (inOutWaterHeightWrtVolume > 0f)
				{
					float height = volumeDataForSubmergeComputation.Height;
					float width = volumeDataForSubmergeComputation.Width;
					float depth = volumeDataForSubmergeComputation.Depth;
					float num9 = MathF.Clamp(inOutWaterHeightWrtVolume, 0f, height);
					num3++;
					num5 += num9;
					Vec3 vec4 = Vec3.CrossProduct(vec2, volumeDataForSubmergeComputation.DynamicLocalBottomPos - localCenterOfMass);
					Vec3 vec5 = globalMassFrame.rotation.TransformToParent(ref vec4);
					float num10 = vec5.z + z;
					if (inOutWaterHeightWrtVolume >= volumeDataForSubmergeComputation.Height || vec5.z <= 0f)
					{
						num6 += 1f;
						num7 += 1f;
					}
					else
					{
						num6 += num8;
						num7 += num8;
					}
					float num11 = width * depth;
					float num12 = height * num11 * num;
					float num13 = 1f / num12;
					float num14 = this._shipPartsDurabilities[(int)this._floaterVolumesShipPartMap[i]] * floatingForceMultiplier;
					if (num14 < num2)
					{
						num14 = num2;
					}
					Vec3 vec6;
					if (bodyWorldTransform.rotation[volumeDataForSubmergeComputation.DynamicUpAxis].z < 0f)
					{
						vec6 = volumeDataForSubmergeComputation.DynamicLocalBottomPos + identity[volumeDataForSubmergeComputation.DynamicUpAxis] * (height - num9 * 0.5f);
					}
					else
					{
						vec6 = volumeDataForSubmergeComputation.DynamicLocalBottomPos + identity[volumeDataForSubmergeComputation.DynamicUpAxis] * (num9 * 0.5f);
					}
					ValueTuple<float, float> valueTuple = NavalPhysics.RungeKuttaIntegrationStepForBuoyancyAndGravity(0f, 0f, num10, fixedDt, inOutWaterHeightWrtVolume, height, num11, waterDensity, num14, num13);
					float item = valueTuple.Item1;
					ValueTuple<float, float> valueTuple2 = NavalPhysics.RungeKuttaIntegrationStepForBuoyancyAndGravity(valueTuple.Item2, item, num10, fixedDt * 0.5f, inOutWaterHeightWrtVolume, height, num11, waterDensity, num14, num13);
					float item2 = valueTuple2.Item1;
					ValueTuple<float, float> valueTuple3 = NavalPhysics.RungeKuttaIntegrationStepForBuoyancyAndGravity(valueTuple2.Item2, item2, num10, fixedDt * 0.5f, inOutWaterHeightWrtVolume, height, num11, waterDensity, num14, num13);
					float item3 = valueTuple3.Item1;
					float item4 = NavalPhysics.RungeKuttaIntegrationStepForBuoyancyAndGravity(valueTuple3.Item2, item3, num10, fixedDt, inOutWaterHeightWrtVolume, height, num11, waterDensity, num14, num13).Item1;
					float num15 = 0.16666667f * (item + 2f * item2 + 2f * item3 + item4);
					float num16 = num9 * num11;
					float num17 = (num15 + 9.806f) * num12;
					num4 += num16;
					vec += vec6 * num16;
					Vec3 vec12;
					if (inOutWaterHeightWrtVolume < height)
					{
						Vec3 outGlobalWaterSurfaceNormal = this._floaterVolumeData[i].OutGlobalWaterSurfaceNormal;
						float num18 = MathF.Clamp(outGlobalWaterSurfaceNormal.x / outGlobalWaterSurfaceNormal.z * width, -height, height);
						float num19 = MathF.Clamp(outGlobalWaterSurfaceNormal.y / outGlobalWaterSurfaceNormal.z * depth, -height, height);
						Vec2 vec7;
						vec7..ctor(num18, num19);
						Vec2 vec8 = Vec2.Abs(new Vec2(vec7.x * width, vec7.y * depth));
						Vec2 vec9 = vec7 * 0.5f;
						Vec2 vec10 = Vec2.ElementWiseProduct(waterDensity * 9.806f * vec9, vec8) * num14;
						Vec3 vec11 = new Vec3(vec10, 0f, -1f);
						vec12 = bodyWorldTransform.rotation.TransformToLocal(ref vec11);
					}
					else
					{
						vec12 = Vec3.Zero;
					}
					Vec3 vec13 = bodyWorldTransform.rotation.TransformToLocal(ref this._floaterVolumeData[i].OutGlobalWaterSurfaceNormal) * (num17 * 0.1f) + vec3 * num17;
					Vec3 vec14 = vec13 + vec12;
					this._buoyancyComputationResult.NetGlobalBuoyancyForce = this._buoyancyComputationResult.NetGlobalBuoyancyForce + bodyWorldTransform.rotation.TransformToParent(ref vec14);
					Vec3 torqueMultiplierOfVerticalBuoyantForces = this._physicsParameters.TorqueMultiplierOfVerticalBuoyantForces;
					Vec3 vec15 = Vec3.CrossProduct(Vec3.ElementWiseProduct(vec13, torqueMultiplierOfVerticalBuoyantForces) + vec12 * this._physicsParameters.TorqueMultiplierOfLateralBuoyantForces, localCenterOfMass - vec6);
					Vec3 vec16 = bodyWorldTransform.rotation.TransformToParent(ref vec15);
					this._buoyancyComputationResult.NetBuoyancyTorque = this._buoyancyComputationResult.NetBuoyancyTorque + vec16;
				}
			}
			this._buoyancyComputationResult.SubmergedFloaterCountFactor = (float)num3 / (float)this._stabilitySubmergedFloaterCount;
			this._buoyancyComputationResult.PitchSubmergedAreaFactor = num6 / ((float)this._stabilitySubmergedFloaterCount * 0.5f) / (1f + num8);
			this._buoyancyComputationResult.RollSubmergedAreaFactor = num7 / ((float)this._stabilitySubmergedFloaterCount * 0.5f) / (1f + num8);
			if (num3 > 0)
			{
				float num20 = num5 / (float)this._stabilitySubmergedFloaterCount;
				this._buoyancyComputationResult.SubmergedHeightFactor = num20 / this._stabilityAvgSubmergedHeight;
				if (this._buoyancyComputationResult.SubmergedHeightFactor > 2f)
				{
					this._buoyancyComputationResult.SubmergedHeightFactor = 2f;
				}
			}
			else
			{
				this._buoyancyComputationResult.SubmergedHeightFactor = 0f;
			}
			float num21 = NavalPhysics.GetAirDensity() / NavalPhysics.GetWaterDensity();
			if (this._buoyancyComputationResult.SubmergedHeightFactor < num21)
			{
				this._buoyancyComputationResult.SubmergedHeightFactor = num21;
				this._buoyancyComputationResult.SimulatingAirFriction = true;
			}
			if (this._buoyancyComputationResult.SubmergedFloaterCountFactor < num21)
			{
				this._buoyancyComputationResult.SubmergedFloaterCountFactor = num21;
				this._buoyancyComputationResult.SimulatingAirFriction = true;
			}
			if (this._buoyancyComputationResult.PitchSubmergedAreaFactor < num21)
			{
				this._buoyancyComputationResult.PitchSubmergedAreaFactor = num21;
				this._buoyancyComputationResult.SimulatingAirFriction = true;
			}
			if (this._buoyancyComputationResult.RollSubmergedAreaFactor < num21)
			{
				this._buoyancyComputationResult.RollSubmergedAreaFactor = num21;
				this._buoyancyComputationResult.SimulatingAirFriction = true;
			}
			if (this._buoyancyComputationResult.RollSubmergedAreaFactor < 0.25f)
			{
				this._buoyancyComputationResult.RollSubmergedAreaFactor = 0.25f;
			}
			if (num4 > 0f)
			{
				vec /= num4;
				this._buoyancyComputationResult.AvgLocalBuoyancyApplyPosition = vec;
				return;
			}
			this._buoyancyComputationResult.AvgLocalBuoyancyApplyPosition = Vec3.Zero;
		}

		// Token: 0x06000E90 RID: 3728 RVA: 0x00070768 File Offset: 0x0006E968
		private void PreComputeAngularDragTerms(out Vec3 angularDampingTerm, out Vec3 angularDragTerm, out float angularDampingYSideComponentTerm, out float angularDragYSideComponentTerm)
		{
			angularDampingTerm = Vec3.One;
			angularDragTerm = Vec3.One;
			Vec3 localCenterOfMass = this.LocalCenterOfMass;
			double num = (double)(this.PhysicsBoundingBoxWithoutChildren.max.y - this.PhysicsBoundingBoxWithoutChildren.min.y);
			double num2 = 0.001;
			double num3 = 0.001;
			double num4 = (double)localCenterOfMass.y;
			double num5 = (double)this.LinearDragTerm.Up / num;
			double num6 = (double)this.LinearDampingTerm.Up / num;
			double num7 = (double)this.LinearDragTerm.Down / num;
			double num8 = (double)this.LinearDampingTerm.Down / num;
			double num9 = num3 * num5 + num3 * num7;
			double num10 = num3 * num6 + num3 * num8;
			double num11 = 0.0;
			double num12 = 0.0;
			for (double num13 = (double)this.PhysicsBoundingBoxWithoutChildren.min.y; num13 <= (double)this.PhysicsBoundingBoxWithoutChildren.max.y; num13 += num3)
			{
				double num14 = Math.Abs(num13 - num4);
				num11 += num14 * num14 * num14;
				num12 += num14 * num14;
			}
			num11 *= num9;
			num12 *= num10;
			angularDampingTerm.x = (float)num12;
			angularDragTerm.x = (float)num11;
			double num15 = (double)localCenterOfMass.x;
			double num16 = (double)(MathF.Abs(this.PhysicsBoundingBoxWithoutChildren.min.x) + MathF.Abs(this.PhysicsBoundingBoxWithoutChildren.max.x) + MathF.Abs(this.PhysicsBoundingBoxWithoutChildren.min.x) + MathF.Abs(this.PhysicsBoundingBoxWithoutChildren.max.x)) * 0.25;
			double num17 = (double)this.LinearDragTerm.Up / num16;
			double num18 = (double)this.LinearDampingTerm.Up / num16;
			double num19 = (double)this.LinearDragTerm.Down / num16;
			double num20 = (double)this.LinearDampingTerm.Down / num16;
			double num21 = num2 * num17 + num2 * num19;
			double num22 = num2 * num18 + num2 * num20;
			double num23 = 0.0;
			double num24 = 0.0;
			for (double num25 = -num16; num25 <= num16; num25 += num2)
			{
				double num26 = Math.Abs(num25 - num15);
				num23 += num26 * num26 * num26;
				num24 += num26 * num26;
			}
			num23 *= num21;
			num24 *= num22;
			angularDampingTerm.y = (float)num24;
			angularDragTerm.y = (float)num23;
			double num27 = (double)localCenterOfMass.z;
			double num28 = (double)this.StabilitySubmergedHeightOfShip;
			double num29 = num28 - (double)this._minFloaterEntitialBottomPos;
			double num30 = 0.001;
			double num31 = (double)(this.LinearDragTerm.Left + this.LinearDragTerm.Right) * 1.0 / num29;
			double num32 = (double)(this.LinearDampingTerm.Left + this.LinearDampingTerm.Right) * 1.0 / num29;
			double num33 = num30 * num31;
			double num34 = num30 * num32;
			double num35 = 0.0;
			double num36 = 0.0;
			for (double num37 = (double)this._minFloaterEntitialBottomPos; num37 <= num28; num37 += num30)
			{
				double num38 = Math.Abs(num37 - num27);
				num35 += num38 * num38 * num38;
				num36 += num38 * num38;
			}
			num35 *= num33;
			num36 *= num34;
			angularDampingYSideComponentTerm = (float)num36;
			angularDragYSideComponentTerm = (float)num35;
			double num39 = (double)localCenterOfMass.y;
			double num40 = num;
			double num41 = (double)(this.LinearDragTerm.Left + this.LinearDragTerm.Right) * 0.5 / num40;
			double num42 = (double)(this.LinearDampingTerm.Left + this.LinearDampingTerm.Right) * 0.5 / num40;
			double num43 = num3 * num41;
			double num44 = num3 * num42;
			double num45 = 0.0;
			double num46 = 0.0;
			for (double num47 = (double)this.PhysicsBoundingBoxWithoutChildren.min.y; num47 <= (double)this.PhysicsBoundingBoxWithoutChildren.max.y; num47 += num3)
			{
				double num48 = Math.Abs(num47 - num39);
				num45 += num48 * num48 * num48;
				num46 += num48 * num48;
			}
			num45 *= num43;
			num46 *= num44;
			angularDampingTerm.z = (float)num46;
			angularDragTerm.z = (float)num45;
		}

		// Token: 0x06000E91 RID: 3729 RVA: 0x00070BC4 File Offset: 0x0006EDC4
		private void ComputeDragForces(float fixedDt, in Vec3 globalLinearVelocity, in Vec3 globalAngularVelocity, in Vec3 massSpaceLocalInertia)
		{
			this._dragComputationResult.Reset();
			MatrixFrame bodyWorldTransform = base.GameEntity.GetBodyWorldTransform();
			MatrixFrame globalMassFrame = this.GetGlobalMassFrame();
			Vec3 localCenterOfMass = this.LocalCenterOfMass;
			int num = MathF.Ceiling(fixedDt / 0.016666668f);
			NavalPhysics.ComputeAngularDrag(fixedDt, num, in globalAngularVelocity, in globalMassFrame, in massSpaceLocalInertia, in this._physicsParameters, in this._buoyancyComputationResult, in this.AngularDragTerm, in this.AngularDampingTerm, this._angularDragYSideComponentTerm, this._angularDampingYSideComponentTerm, ref this._dragComputationResult);
			this.ComputeDriftFromAngularFriction(fixedDt, in bodyWorldTransform, in globalMassFrame);
			int num2 = num;
			float mass = this.Mass;
			LinearFrictionTerm linearDragTerm = this.LinearDragTerm;
			LinearFrictionTerm linearDampingTerm = this.LinearDampingTerm;
			LinearFrictionTerm constantLinearDampingTerm = this.ConstantLinearDampingTerm;
			float num3;
			NavalPhysics.ComputeLinearDrag(fixedDt, num2, in globalLinearVelocity, in bodyWorldTransform, in mass, in localCenterOfMass, in this._physicsParameters, in this._buoyancyComputationResult, in linearDragTerm, in linearDampingTerm, in constantLinearDampingTerm, this._minFloaterEntitialBottomPos, this._maxFloaterEntitialTopPos, ref this._dragComputationResult, out num3);
		}

		// Token: 0x06000E92 RID: 3730 RVA: 0x00070C9C File Offset: 0x0006EE9C
		private void ComputeDriftFromAngularFriction(float fixedDt, in MatrixFrame entityGlobalFrame, in MatrixFrame centerOfMassGlobalFrame)
		{
			if (!this._buoyancyComputationResult.SimulatingAirFriction && this._buoyancyComputationResult.SubmergedHeightFactor < 2f && this.NavalSinkingState == NavalPhysics.SinkingState.Floating)
			{
				MatrixFrame matrixFrame = entityGlobalFrame;
				Vec3 vec = matrixFrame.TransformToParent(ref this._buoyancyComputationResult.AvgLocalBuoyancyApplyPosition);
				Vec3 vec2 = this._dragComputationResult.AngularDragTorqueGlobal * fixedDt;
				vec2.z = 0f;
				Vec3 vec3;
				Vec3 vec4;
				GameEntityPhysicsExtensions.ComputeVelocityDeltaFromImpulse(base.GameEntity, ref Vec3.Zero, ref vec2, ref vec3, ref vec4);
				Vec3 vec5 = -Vec3.CrossProduct(centerOfMassGlobalFrame.origin - vec, vec4);
				float num = 1f;
				if (this._buoyancyComputationResult.SubmergedHeightFactor > 1f)
				{
					num = 2f / this._buoyancyComputationResult.SubmergedHeightFactor - 1f;
				}
				vec5 *= num;
				if (vec5.LengthSquared > 0.010000001f)
				{
					vec5 = vec5.NormalizedCopy() * 0.1f;
				}
				this._dragComputationResult.DriftForceFromAngularDragGlobal = this.Mass * (-vec5 / fixedDt);
			}
		}

		// Token: 0x06000E93 RID: 3731 RVA: 0x00070DBD File Offset: 0x0006EFBD
		protected override void OnTickParallel(float dt)
		{
			this._committedWeightedAgentsPosition = this._weightedAgentsPosition;
			this._committedTotalMass = this._totalMass;
			this.ClearAgentWeightAndPositionInformation();
		}

		// Token: 0x06000E94 RID: 3732 RVA: 0x00070DE0 File Offset: 0x0006EFE0
		private void ApplyDragForces()
		{
			this.ApplyGlobalForceAtLocalPos(in this._dragComputationResult.CenterOfLateralDragLocal, in this._dragComputationResult.LateralDragForceGlobal, 0);
			this.ApplyGlobalForceAtLocalPos(in this._dragComputationResult.CenterOfLongitudinalDragLocal, in this._dragComputationResult.LongitudinalDragForceGlobal, 0);
			this.ApplyGlobalForceAtLocalPos(in this._dragComputationResult.CenterOfVerticalDragLocal, in this._dragComputationResult.VerticalDragForceGlobal, 0);
			this.ApplyTorque(in this._dragComputationResult.AngularDragTorqueGlobal, 0);
			this.ApplyForceToDynamicBody(in this._dragComputationResult.DriftForceFromAngularDragGlobal, 0);
		}

		// Token: 0x06000E95 RID: 3733 RVA: 0x00070E68 File Offset: 0x0006F068
		private void ApplyAgentForces()
		{
			if (this._committedTotalMass > 0f)
			{
				Vec3 vec = this._committedWeightedAgentsPosition / this._committedTotalMass;
				Vec3 vec2 = base.GameEntity.GetBodyWorldTransform().TransformToLocal(ref vec);
				if (this.PhysicsBoundingBoxWithoutChildren.PointInsideBox(vec2, 0.1f))
				{
					float stepAgentWeightMultiplier = this._physicsParameters.StepAgentWeightMultiplier;
					Vec3 vec3 = this._committedTotalMass * stepAgentWeightMultiplier * MBGlobals.GravitationalAcceleration;
					this.ApplyGlobalForceAtLocalPos(in vec2, in vec3, 0);
				}
			}
		}

		// Token: 0x06000E96 RID: 3734 RVA: 0x00070EF1 File Offset: 0x0006F0F1
		private void ClearAgentWeightAndPositionInformation()
		{
			this._weightedAgentsPosition = Vec3.Zero;
			this._totalMass = 0f;
		}

		// Token: 0x06000E97 RID: 3735 RVA: 0x00070F09 File Offset: 0x0006F109
		private void ApplyBuoyancyForces()
		{
			this.ApplyForceToDynamicBody(in this._buoyancyComputationResult.NetGlobalBuoyancyForce, 0);
			this.ApplyTorque(in this._buoyancyComputationResult.NetBuoyancyTorque, 0);
		}

		// Token: 0x06000E98 RID: 3736 RVA: 0x00070F30 File Offset: 0x0006F130
		public void AddAgentWeightAndPositionInformation(Agent agent)
		{
			float totalMass = agent.GetTotalMass();
			Vec3 position = agent.Position;
			if (this.PhysicsBoundingBoxWithoutChildren.PointInsideBox(base.GameEntity.GetBodyWorldTransform().TransformToLocal(ref position), 0.1f))
			{
				this._weightedAgentsPosition += totalMass * position;
				this._totalMass += totalMass;
			}
		}

		// Token: 0x06000E99 RID: 3737 RVA: 0x00070FA0 File Offset: 0x0006F1A0
		private void ApplyActuatorForces()
		{
			if (this._shipForceRecord.HasLeftOarForces)
			{
				foreach (ShipForce shipForce in this._shipForceRecord.LeftOarForces)
				{
					if (shipForce.IsApplicable)
					{
						this.ApplyGlobalForceAtLocalPos(in shipForce.LocalPosition, in shipForce.Force, 0);
					}
				}
			}
			if (this._shipForceRecord.HasRightOarForces)
			{
				foreach (ShipForce shipForce2 in this._shipForceRecord.RightOarForces)
				{
					if (shipForce2.IsApplicable)
					{
						this.ApplyGlobalForceAtLocalPos(in shipForce2.LocalPosition, in shipForce2.Force, 0);
					}
				}
			}
			if (this._shipForceRecord.HasSailForces)
			{
				foreach (ShipForce shipForce3 in this._shipForceRecord.SailForces)
				{
					if (shipForce3.IsApplicable)
					{
						Vec3 vec;
						Vec3 vec2;
						shipForce3.ComputeRealisticAndGamifiedForceComponents(out vec, out vec2);
						this.ApplyGlobalForceAtLocalPos(in shipForce3.LocalPosition, in vec, 0);
						this.ApplyForceToDynamicBody(in vec2, 0);
					}
				}
			}
			if (this._shipForceRecord.RudderForce.IsApplicable)
			{
				Vec3 vec3;
				Vec3 vec4;
				this._shipForceRecord.RudderForce.ComputeRealisticAndGamifiedForceComponents(out vec3, out vec4);
				this.ApplyGlobalForceAtLocalPos(in this._shipForceRecord.RudderForce.LocalPosition, in vec3, 0);
				Vec3 localPosition = this._shipForceRecord.RudderForce.LocalPosition;
				localPosition.z = this.LocalCenterOfMass.z;
				this.ApplyGlobalForceAtLocalPos(in localPosition, in vec4, 0);
			}
		}

		// Token: 0x06000E9A RID: 3738 RVA: 0x00071178 File Offset: 0x0006F378
		private void ApplyAnchorForces()
		{
			Vec3 vec = Vec3.Zero;
			Vec3 vec2 = Vec3.Zero;
			Vec3 zero = Vec3.Zero;
			if (this.IsAnchored)
			{
				MatrixFrame bodyWorldTransform = base.GameEntity.GetBodyWorldTransform();
				bodyWorldTransform.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
				Vec3 vec3 = this._anchorGlobalFrame.origin - bodyWorldTransform.origin;
				vec = this.Mass * this._anchorForceMultiplier * (1.2f * vec3 - 3.6f * this.LinearVelocity);
				vec.z = 0f;
				float num = vec.Normalize();
				float num2 = 2f * this.Mass * 9.806f;
				vec = MathF.Min(num, num2) * vec;
				float y = this.PhysicsBoundingBoxWithChildrenSize.y;
				float num3 = 0.6f * y;
				if (vec3.LengthSquared <= num3 * num3)
				{
					Vec2 vec4 = bodyWorldTransform.rotation.f.AsVec2.Normalized();
					Vec2 vec5 = this.AnchorGlobalFrame.rotation.f.AsVec2.Normalized();
					float num4 = MathF.Atan2(Vec2.Determinant(ref vec4, ref vec5), Vec2.DotProduct(vec4, vec5));
					float num5 = (1.4f * num4 - 4.2f * this.AngularVelocity.z) * this._anchorForceMultiplier;
					num5 = (float)MathF.Sign(num5) * MathF.Min(0.34906587f, MathF.Abs(num5));
					Vec3 vec6 = num5 * Vec3.Up;
					Vec3 vec7 = bodyWorldTransform.rotation.TransformToLocal(ref vec6);
					Vec3 vec8 = Vec3.ElementWiseProduct(this.MassSpaceInertia, vec7);
					Vec3 vec9 = bodyWorldTransform.rotation.TransformToParent(ref vec8);
					vec2 = this.LocalCenterOfMass;
					this.ApplyGlobalForceAtLocalPos(in vec2, in vec, 0);
					this.ApplyTorque(in vec9, 0);
					return;
				}
				Vec3 vec10 = ((Vec3.DotProduct(vec3.NormalizedCopy(), bodyWorldTransform.rotation.f) >= 0f) ? 1f : (-1f)) * (0.1f * y * Vec3.Forward);
				vec2 = this.LocalCenterOfMass + vec10;
				this.ApplyGlobalForceAtLocalPos(in vec2, in vec, 0);
			}
		}

		// Token: 0x06000E9B RID: 3739 RVA: 0x000713B8 File Offset: 0x0006F5B8
		public Oriented2DArea GetGlobalMaximal2DArea()
		{
			BoundingBox boundingBox = this.PhysicsBoundingBoxWithChildren;
			Vec2 asVec = boundingBox.min.AsVec2;
			boundingBox = this.PhysicsBoundingBoxWithChildren;
			Vec2 asVec2 = boundingBox.max.AsVec2;
			Vec2 vec = (asVec2 + asVec) / 2f;
			MatrixFrame bodyWorldTransform = base.GameEntity.GetBodyWorldTransform();
			Vec2 vec2 = bodyWorldTransform.rotation.f.AsVec2.Normalized();
			Vec2 vec3 = -vec2.LeftVec();
			Vec2 vec4 = bodyWorldTransform.origin.AsVec2 + vec.X * vec3 + vec.Y * vec2;
			Vec2 vec5 = asVec2 - asVec;
			return new Oriented2DArea(ref vec4, ref vec2, ref vec5);
		}

		// Token: 0x06000E9C RID: 3740 RVA: 0x00071480 File Offset: 0x0006F680
		public int GetPartIndexAtPosition(Vec3 position)
		{
			BoundingBox boundingBox = this.PhysicsBoundingBoxWithoutChildren;
			Vec2 asVec = boundingBox.min.AsVec2;
			boundingBox = this.PhysicsBoundingBoxWithoutChildren;
			Vec2 asVec2 = boundingBox.max.AsVec2;
			float num = asVec2.Y - asVec.Y;
			float num2 = asVec2.X - asVec.X;
			float num3 = num / 3f;
			float num4 = num2 / 2f;
			float num5 = position.y + num * 0.5f - (asVec2.y + asVec.y) * 0.5f;
			float num6 = position.x + num2 * 0.5f - (asVec2.x + asVec.x) * 0.5f;
			int num7 = MathF.Floor(num5 / num3);
			int num8 = MathF.Floor(num6 / num4);
			num7 = MBMath.ClampIndex(num7, 0, 3);
			num8 = MBMath.ClampIndex(num8, 0, 2);
			return num7 * 2 + num8;
		}

		// Token: 0x06000E9D RID: 3741 RVA: 0x00071564 File Offset: 0x0006F764
		private void LoadFloaterVolumes()
		{
			this.PhysicsBoundingBoxWithChildren = GameEntityPhysicsExtensions.GetLocalPhysicsBoundingBox(base.GameEntity, true);
			this.PhysicsBoundingBoxWithChildrenSize = this.PhysicsBoundingBoxWithChildren.max - this.PhysicsBoundingBoxWithChildren.min;
			this.PhysicsBoundingBoxWithoutChildren = GameEntityPhysicsExtensions.GetLocalPhysicsBoundingBox(base.GameEntity, false);
			this.PhysicsBoundingBoxSizeWithoutChildren = this.PhysicsBoundingBoxWithoutChildren.max - this.PhysicsBoundingBoxWithoutChildren.min;
			this._totalFloaterVolumeCached = 0f;
			WeakGameEntity weakGameEntity = WeakGameEntity.Invalid;
			foreach (WeakGameEntity weakGameEntity2 in base.GameEntity.GetChildren())
			{
				if (weakGameEntity2.Name == "floater_volume_holder")
				{
					weakGameEntity = weakGameEntity2;
					break;
				}
			}
			if (weakGameEntity == WeakGameEntity.Invalid)
			{
				return;
			}
			int num = weakGameEntity.GetChildren().Count<WeakGameEntity>();
			this._floaterVolumesShipPartMap = new NavalPhysics.ShipPart[num];
			this._floaterVolumeData = new VolumeDataForSubmergeComputation[num];
			this._floaterVolumeDataPinnedGCHandler = GCHandle.Alloc(this._floaterVolumeData, GCHandleType.Pinned);
			this._floaterVolumeDataPinnedPointer = (UIntPtr)((ulong)(long)this._floaterVolumeDataPinnedGCHandler.AddrOfPinnedObject());
			float num2 = float.MaxValue;
			float num3 = float.MinValue;
			for (int i = 0; i < num; i++)
			{
				MatrixFrame localFrame = weakGameEntity.GetChild(i).GetLocalFrame();
				this._floaterVolumeData[i].DynamicUpAxis = 2;
				this._floaterVolumeData[i].DynamicLocalBottomPos = localFrame.origin;
				this._floaterVolumeData[i].LocalFrame = localFrame;
				this._floaterVolumeData[i].LocalScale = localFrame.GetScale();
				this._floaterVolumeData[i].OutGlobalWaterSurfaceNormal = Vec3.Up;
				this._floaterVolumeData[i].InOutWaterHeightWrtVolume = this._floaterVolumeData[i].Height * 0.5f;
				this._floaterVolumesShipPartMap[i] = (NavalPhysics.ShipPart)this.GetPartIndexAtPosition(this._floaterVolumeData[i].DynamicLocalBottomPos);
				this._totalFloaterVolumeCached += this._floaterVolumeData[i].Width * this._floaterVolumeData[i].Depth * this._floaterVolumeData[i].Height;
				num2 = Math.Min(num2, this._floaterVolumeData[i].DynamicLocalBottomPos.z);
				num3 = Math.Max(num3, this._floaterVolumeData[i].DynamicLocalBottomPos.z + this._floaterVolumeData[i].Height);
			}
			float waterDensity = NavalPhysics.GetWaterDensity();
			float num4 = this.Mass * 9.806f;
			float num5 = this._totalFloaterVolumeCached * waterDensity * 9.806f;
			this._minimumFloaterDurabilityToFloatWhileNotSinking = num4 * 1.1f / num5;
			this._shipPartsDurabilities = Enumerable.Repeat<float>(1f, 6).ToArray<float>();
			this._shipPartsTargetDurabilities = Enumerable.Repeat<float>(1f, 6).ToArray<float>();
			this.ComputeAndCacheStabilityAvgSubmergedHeight(num2, num3);
		}

		// Token: 0x06000E9E RID: 3742 RVA: 0x0007189C File Offset: 0x0006FA9C
		private void UpdateFloaterVolumeData()
		{
			Mat3 rotation = base.GameEntity.GetBodyWorldTransform().rotation;
			for (int i = 0; i < this._floaterVolumeData.Length; i++)
			{
				Vec3 localScale = this._floaterVolumeData[i].LocalScale;
				int num = this._floaterVolumeData[i].DynamicUpAxis;
				float num2 = localScale[num] * MathF.Abs(rotation[num].z);
				for (int j = 1; j < 3; j++)
				{
					int num3 = (this._floaterVolumeData[i].DynamicUpAxis + j) % 3;
					float num4 = localScale[num3] * MathF.Abs(rotation[num3].z);
					if (num4 > num2 * 1.1f)
					{
						num2 = num4;
						num = num3;
					}
				}
				if (this._floaterVolumeData[i].DynamicUpAxis != (byte)num)
				{
					float num5 = this._floaterVolumeData[i].InOutWaterHeightWrtVolume / this._floaterVolumeData[i].Height;
					this._floaterVolumeData[i].DynamicUpAxis = (byte)num;
					switch (this._floaterVolumeData[i].DynamicUpAxis)
					{
					case 0:
						this._floaterVolumeData[i].DynamicLocalBottomPos = this._floaterVolumeData[i].LocalFrame.origin + new Vec3(-localScale.x * 0.5f, 0f, localScale.z * 0.5f, -1f);
						break;
					case 1:
						this._floaterVolumeData[i].DynamicLocalBottomPos = this._floaterVolumeData[i].LocalFrame.origin + new Vec3(0f, -localScale.y * 0.5f, localScale.z * 0.5f, -1f);
						break;
					case 2:
						this._floaterVolumeData[i].DynamicLocalBottomPos = this._floaterVolumeData[i].LocalFrame.origin;
						break;
					}
					this._floaterVolumeData[i].InOutWaterHeightWrtVolume = this._floaterVolumeData[i].Height * num5;
				}
			}
		}

		// Token: 0x06000E9F RID: 3743 RVA: 0x00071AEC File Offset: 0x0006FCEC
		private void ComputeAndCacheStabilityAvgSubmergedHeight(float minimumEntitialFloaterZ, float maximumEntitialFloaterZ)
		{
			float waterDensity = NavalPhysics.GetWaterDensity();
			float num = this.Mass * 9.806f;
			float num2 = minimumEntitialFloaterZ + 0.01f;
			float floatingForceMultiplier = this._physicsParameters.FloatingForceMultiplier;
			this._stabilityAvgSubmergedHeight = maximumEntitialFloaterZ - minimumEntitialFloaterZ;
			this._stabilitySubmergedFloaterCount = this._floaterVolumeData.Length;
			this._minFloaterEntitialBottomPos = minimumEntitialFloaterZ;
			this._maxFloaterEntitialTopPos = maximumEntitialFloaterZ;
			while (maximumEntitialFloaterZ > num2)
			{
				float num3 = 0f;
				int num4 = 0;
				float num5 = 0f;
				for (int i = 0; i < this._floaterVolumeData.Length; i++)
				{
					float num6 = num2 - this._floaterVolumeData[i].DynamicLocalBottomPos.z;
					if (num6 > 0f)
					{
						float num7 = Math.Min(num6, this._floaterVolumeData[i].Height);
						float num8 = num7 * this._floaterVolumeData[i].Width * this._floaterVolumeData[i].Depth * waterDensity * 9.806f * floatingForceMultiplier;
						num3 += num7;
						num4++;
						num5 += num8;
					}
				}
				if (num5 >= num)
				{
					this.StabilitySubmergedHeightOfShip = num2;
					this._stabilityAvgSubmergedHeight = num3 / (float)num4;
					this._stabilitySubmergedFloaterCount = num4;
					return;
				}
				num2 += 0.01f;
			}
		}

		// Token: 0x06000EA0 RID: 3744 RVA: 0x00071C30 File Offset: 0x0006FE30
		private void UpdateShipPhysics(NavalPhysics.NavalPhysicsParameters physicsParameters, ShipPhysicsReference basePhysicsRef)
		{
			this._physicsParameters = physicsParameters;
			float overrideMass = this._physicsParameters.OverrideMass;
			float num;
			if (overrideMass > 0f)
			{
				num = overrideMass;
			}
			else
			{
				num = base.GameEntity.Mass;
			}
			num *= this._physicsParameters.MassMultiplier;
			Vec3 centerOfMass = base.GameEntity.CenterOfMass;
			GameEntityPhysicsExtensions.SetMassAndUpdateInertiaAndCenterOfMass(base.GameEntity, num);
			GameEntityPhysicsExtensions.SetCenterOfMass(base.GameEntity, centerOfMass);
			this._cachedMass = base.GameEntity.Mass;
			Vec3 vec = Vec3.ElementWiseProduct(GameEntityPhysicsExtensions.GetMassSpaceInertia(base.GameEntity), this._physicsParameters.MomentOfInertiaMultiplier);
			GameEntityPhysicsExtensions.SetMassSpaceInertia(base.GameEntity, vec);
			this.LinearDragTerm = basePhysicsRef.LinearDragTerm * this._cachedMass;
			this.LinearDampingTerm = basePhysicsRef.LinearDampingTerm * this._cachedMass;
			this.ConstantLinearDampingTerm = basePhysicsRef.ConstantLinearDampingTerm * this._cachedMass;
			GameEntityPhysicsExtensions.SetLinearVelocity(base.GameEntity, Vec3.Zero);
			GameEntityPhysicsExtensions.SetAngularVelocity(base.GameEntity, Vec3.Zero);
			GameEntityPhysicsExtensions.DisableGravity(base.GameEntity);
			PhysicsMaterial physicsMaterial = base.GameEntity.GetPhysicsMaterial();
			GameEntityPhysicsExtensions.SetDamping(base.GameEntity, physicsMaterial.GetLinearDamping(), physicsMaterial.GetAngularDamping());
		}

		// Token: 0x06000EA1 RID: 3745 RVA: 0x00071D7C File Offset: 0x0006FF7C
		private void ComputeContinuousDriftForce(float fixedDt)
		{
			this._continuousDriftForceData.ResultForce = Vec3.Zero;
			if (this._continuousDriftForceData.DriftSpeed > 0f && this.IsInitialized && !this._buoyancyComputationResult.SimulatingAirFriction && this.NavalSinkingState == NavalPhysics.SinkingState.Floating)
			{
				Vec2 vec = base.GameEntity.GetGlobalWindVelocityOfScene().Normalized();
				Vec2 vec2 = vec * this._continuousDriftForceData.DriftSpeed;
				Vec2 vec3 = vec2 - this.LinearVelocity.AsVec2;
				float num = vec.DotProduct(vec3);
				if (num > 0f)
				{
					Vec2 vec4 = vec * num;
					float num2 = MathF.Clamp(this.LastSubmergedHeightFactorForActuators, 0f, 1f);
					float num3 = MathF.Sin(this._continuousDriftForceData.DriftForceTimer * 3.1415927f * 0.1f);
					this._continuousDriftForceData.DriftForceTimer = this._continuousDriftForceData.DriftForceTimer + fixedDt * num2 * this._continuousDriftForceData.DriftRandom.NextFloat();
					num2 *= num3 * 0.4f + 0.8f;
					float num4 = num3;
					Vec2 vec5 = vec4;
					vec5.RotateCCW(num4 * 0.08726646f);
					this._continuousDriftForceData.ResultForce = vec5.ToVec3(0f) * num2 * this.Mass;
				}
			}
		}

		// Token: 0x06000EA2 RID: 3746 RVA: 0x00071ED9 File Offset: 0x000700D9
		private void ApplyContinuousDriftForce()
		{
			if (this._continuousDriftForceData.ResultForce.LengthSquared > 0f)
			{
				this.ApplyForceToDynamicBody(in this._continuousDriftForceData.ResultForce, 0);
			}
		}

		// Token: 0x06000EA3 RID: 3747 RVA: 0x00071F04 File Offset: 0x00070104
		private static float ComputeLateralDragShift(in Vec3 localVelocity, float maxLateralDragShift, float lateralDragShiftCriticalAngle, float maxLateralShiftSpeed)
		{
			Vec3 vec = localVelocity;
			float num = MathF.Acos(MathF.Max(vec.NormalizedCopy().y, 0f));
			float num2 = 2.5f * num / lateralDragShiftCriticalAngle;
			float num3 = 1f - (float)Math.Exp((double)(-(double)(num2 * num2)));
			return MathF.Clamp(localVelocity.y / maxLateralShiftSpeed, 0f, 1f) * num3 * maxLateralDragShift;
		}

		// Token: 0x06000EA4 RID: 3748 RVA: 0x00071F6B File Offset: 0x0007016B
		public void SetSinkingState(NavalPhysics.SinkingState state)
		{
			this.NavalSinkingState = state;
		}

		// Token: 0x06000EA5 RID: 3749 RVA: 0x00071F74 File Offset: 0x00070174
		public void ForceSink()
		{
			if (this._shipPartsTargetDurabilities != null)
			{
				for (int i = 0; i < this._shipPartsTargetDurabilities.Length; i++)
				{
					this._shipPartsTargetDurabilities[i] = 0f;
				}
			}
			this.NavalSinkingState = NavalPhysics.SinkingState.Sinking;
		}

		// Token: 0x06000EA6 RID: 3750 RVA: 0x00071FB0 File Offset: 0x000701B0
		private static Vec3 SubStepIntegrationStepForLinearFriction(Vec3 absLinearVelocityLocal, float subStepFixedDt, float mass, Vec3 submergedLinearDragTerm, Vec3 submergedLinearDampingTerm, Vec3 submergedConstantLinearDampingTerm, Vec3 submergedFactorLinear)
		{
			Vec3 vec = Vec3.ElementWiseProduct(NavalPhysics.ComputeVelocityFactorForClampingDrag(absLinearVelocityLocal), submergedFactorLinear);
			Vec3 vec2 = Vec3.ElementWiseProduct(absLinearVelocityLocal, absLinearVelocityLocal);
			return (Vec3.ElementWiseProduct(submergedLinearDragTerm, vec2) + Vec3.ElementWiseProduct(submergedLinearDampingTerm, absLinearVelocityLocal) + submergedConstantLinearDampingTerm + Vec3.ElementWiseProduct(submergedLinearDragTerm, vec)) / mass * subStepFixedDt;
		}

		// Token: 0x06000EA7 RID: 3751 RVA: 0x00072008 File Offset: 0x00070208
		private static Vec3 SubStepIntegrationStepForAngularFriction(Vec3 absMassLocalAngularVelocity, float subStepFixedDt, Vec3 massLocalInertia, Vec3 angularDragTerm, Vec3 angularDampingTerm, float angularDragYSideComponentTerm, float angularDampingYSideComponentTerm, in NavalPhysics.BuoyancyComputationResult buoyancyComputationResult)
		{
			Vec3 zero = Vec3.Zero;
			zero.x = angularDragTerm.x * absMassLocalAngularVelocity.x * absMassLocalAngularVelocity.x;
			zero.x += angularDampingTerm.x * absMassLocalAngularVelocity.x;
			zero.x *= buoyancyComputationResult.PitchSubmergedAreaFactor;
			float num = angularDragTerm.y * absMassLocalAngularVelocity.y * absMassLocalAngularVelocity.y;
			num += angularDampingTerm.y * absMassLocalAngularVelocity.y;
			num *= buoyancyComputationResult.RollSubmergedAreaFactor;
			zero.y = num;
			float num2 = angularDragYSideComponentTerm * absMassLocalAngularVelocity.y * absMassLocalAngularVelocity.y;
			num2 += angularDampingYSideComponentTerm * absMassLocalAngularVelocity.y;
			num2 *= buoyancyComputationResult.SubmergedHeightFactor;
			zero.y += num2;
			zero.z = angularDragTerm.z * absMassLocalAngularVelocity.z * absMassLocalAngularVelocity.z;
			zero.z += angularDampingTerm.z * absMassLocalAngularVelocity.z;
			zero.z *= buoyancyComputationResult.SubmergedHeightFactor;
			return Vec3.ElementWiseDivision(zero, massLocalInertia) * subStepFixedDt;
		}

		// Token: 0x06000EA8 RID: 3752 RVA: 0x00072120 File Offset: 0x00070320
		public static void ComputeLinearDrag(float fixedDt, int substepCount, in Vec3 globalLinearVelocity, in MatrixFrame globalFrame, in float mass, in Vec3 localCenterOfMass, in NavalPhysics.NavalPhysicsParameters physicsParameters, in NavalPhysics.BuoyancyComputationResult buoyancyComputationResult, in LinearFrictionTerm linearDragTerm, in LinearFrictionTerm linearDampingTerm, in LinearFrictionTerm constantLinearDampingTerm, float minFloaterEntitialBottomPos, float maxFloaterEntitialTopPos, ref NavalPhysics.DragForceComputationResult dragComputationResult, out float lateralDragForwardShift)
		{
			Vec3 vec = globalLinearVelocity;
			Mat3 rotation = globalFrame.rotation;
			Vec3 vec2 = rotation.TransformToLocal(ref vec);
			Vec3 vec3;
			vec3..ctor(buoyancyComputationResult.SubmergedHeightFactor, buoyancyComputationResult.SubmergedHeightFactor, buoyancyComputationResult.SubmergedFloaterCountFactor, -1f);
			LinearFrictionTerm linearFrictionTerm = linearDragTerm;
			LinearFrictionTerm linearFrictionTerm2 = linearFrictionTerm.ElementWiseProduct(physicsParameters.LinearFrictionMultiplier);
			linearFrictionTerm = linearDampingTerm;
			LinearFrictionTerm linearFrictionTerm3 = linearFrictionTerm.ElementWiseProduct(physicsParameters.LinearFrictionMultiplier);
			linearFrictionTerm = constantLinearDampingTerm;
			LinearFrictionTerm linearFrictionTerm4 = linearFrictionTerm.ElementWiseProduct(physicsParameters.LinearFrictionMultiplier);
			Vec3 vec4;
			vec4..ctor((vec2.x >= 0f) ? linearFrictionTerm2.Right : linearFrictionTerm2.Left, (vec2.y >= 0f) ? linearFrictionTerm2.Forward : linearFrictionTerm2.Backward, (vec2.z >= 0f) ? linearFrictionTerm2.Up : linearFrictionTerm2.Down, -1f);
			Vec3 vec5;
			vec5..ctor((vec2.x >= 0f) ? linearFrictionTerm3.Right : linearFrictionTerm3.Left, (vec2.y >= 0f) ? linearFrictionTerm3.Forward : linearFrictionTerm3.Backward, (vec2.z >= 0f) ? linearFrictionTerm3.Up : linearFrictionTerm3.Down, -1f);
			Vec3 vec6 = new Vec3((vec2.x >= 0f) ? linearFrictionTerm4.Right : linearFrictionTerm4.Left, (vec2.y >= 0f) ? linearFrictionTerm4.Forward : linearFrictionTerm4.Backward, (vec2.z >= 0f) ? linearFrictionTerm4.Up : linearFrictionTerm4.Down, -1f);
			Vec3 vec7 = Vec3.ElementWiseProduct(vec4, vec3);
			Vec3 vec8 = Vec3.ElementWiseProduct(vec5, vec3);
			Vec3 vec9 = Vec3.ElementWiseProduct(vec6, vec3);
			Vec3 vec10 = Vec3.Abs(vec2);
			Vec3 vec11 = Vec3.One;
			vec11.y *= physicsParameters.ForwardDragMultiplier;
			vec11 *= NavalPhysics.GetWaterDensity();
			if (globalFrame.rotation.u.z < -0.4f)
			{
				vec11 *= physicsParameters.UpSideDownFrictionMultiplier;
			}
			float num = fixedDt / (float)substepCount;
			Vec3 vec12 = vec10;
			for (int i = 0; i < substepCount; i++)
			{
				Vec3 vec13 = NavalPhysics.SubStepIntegrationStepForLinearFriction(vec12, num, mass, vec7, vec8, vec9, vec3);
				vec13 = Vec3.ElementWiseProduct(vec13, vec11);
				vec12 -= vec13;
				if (vec12.x < 0f)
				{
					vec12.x = 0f;
				}
				if (vec12.y < 0f)
				{
					vec12.y = 0f;
				}
				if (vec12.z < 0f)
				{
					vec12.z = 0f;
				}
			}
			Vec3 vec14 = (vec10 - vec12) * (mass / fixedDt);
			Vec3 vec15 = mass * vec10;
			Vec3 vec16 = 1f / fixedDt * vec15;
			Vec3 vec17 = new Vec3((float)(-(float)MathF.Sign(vec2.x)) * MathF.Min(vec16.x, vec14.x), (float)(-(float)MathF.Sign(vec2.y)) * MathF.Min(vec16.y, vec14.y), (float)(-(float)MathF.Sign(vec2.z)) * MathF.Min(vec16.z, vec14.z), -1f);
			Vec3 vec18 = vec17.x * globalFrame.rotation.s;
			Vec3 vec19 = vec17.y * globalFrame.rotation.f;
			Vec3 vec20 = vec17.z * globalFrame.rotation.u;
			float num2 = physicsParameters.MaxLinearSpeedForLateralDragCenterShift * 0.2f;
			lateralDragForwardShift = NavalPhysics.ComputeLateralDragShift(in vec2, physicsParameters.MaxLateralDragShift, physicsParameters.LateralDragShiftCriticalAngle, num2);
			dragComputationResult.LateralDragForceGlobal = vec18;
			dragComputationResult.LongitudinalDragForceGlobal = vec19;
			dragComputationResult.VerticalDragForceGlobal = vec20;
			if (buoyancyComputationResult.SimulatingAirFriction)
			{
				dragComputationResult.CenterOfLateralDragLocal = localCenterOfMass;
				dragComputationResult.CenterOfLongitudinalDragLocal = localCenterOfMass;
				dragComputationResult.CenterOfVerticalDragLocal = localCenterOfMass;
				return;
			}
			dragComputationResult.CenterOfLateralDragLocal.x = buoyancyComputationResult.AvgLocalBuoyancyApplyPosition.x;
			dragComputationResult.CenterOfLateralDragLocal.y = localCenterOfMass.y - Vec3.Forward.y * lateralDragForwardShift;
			dragComputationResult.CenterOfLateralDragLocal.z = buoyancyComputationResult.AvgLocalBuoyancyApplyPosition.z;
			dragComputationResult.CenterOfLongitudinalDragLocal = localCenterOfMass;
			dragComputationResult.CenterOfVerticalDragLocal.x = buoyancyComputationResult.AvgLocalBuoyancyApplyPosition.x;
			dragComputationResult.CenterOfVerticalDragLocal.y = buoyancyComputationResult.AvgLocalBuoyancyApplyPosition.y;
			dragComputationResult.CenterOfVerticalDragLocal.z = ((globalFrame.rotation.u.z >= 0f) ? minFloaterEntitialBottomPos : maxFloaterEntitialTopPos);
		}

		// Token: 0x06000EA9 RID: 3753 RVA: 0x000725F0 File Offset: 0x000707F0
		public static void ComputeAngularDrag(float fixedDt, int substepCount, in Vec3 globalAngularVelocity, in MatrixFrame centerOfMassGlobalFrame, in Vec3 massSpaceLocalInertia, in NavalPhysics.NavalPhysicsParameters physicsParameters, in NavalPhysics.BuoyancyComputationResult buoyancyComputationResult, in Vec3 angularDragTerm, in Vec3 angularDampingTerm, float angularDragYSideComponentTerm, float angularDampingYSideComponentTerm, ref NavalPhysics.DragForceComputationResult dragComputationResult)
		{
			Vec3 vec = globalAngularVelocity;
			Mat3 mat = centerOfMassGlobalFrame.rotation;
			Vec3 vec2 = mat.TransformToLocal(ref vec);
			Vec3 vec3 = Vec3.Abs(vec2);
			Vec3 vec4 = Vec3.ElementWiseProduct(massSpaceLocalInertia, vec3);
			Vec3 vec5 = 1f / fixedDt * vec4;
			Vec3 vec6 = physicsParameters.AngularFrictionMultiplier * NavalPhysics.GetWaterDensity();
			if (centerOfMassGlobalFrame.rotation.u.z < -0.4f)
			{
				vec6 *= physicsParameters.UpSideDownFrictionMultiplier;
			}
			float num = fixedDt / (float)substepCount;
			Vec3 vec7 = vec3;
			for (int i = 0; i < substepCount; i++)
			{
				Vec3 vec8 = NavalPhysics.SubStepIntegrationStepForAngularFriction(vec7, num, massSpaceLocalInertia, angularDragTerm, angularDampingTerm, angularDragYSideComponentTerm, angularDampingYSideComponentTerm, in buoyancyComputationResult);
				vec8 = Vec3.ElementWiseProduct(vec8, vec6);
				vec7 -= vec8;
				if (vec7.x < 0f)
				{
					vec7.x = 0f;
				}
				if (vec7.y < 0f)
				{
					vec7.y = 0f;
				}
				if (vec7.z < 0f)
				{
					vec7.z = 0f;
				}
			}
			Vec3 vec9 = Vec3.ElementWiseProduct(vec3 - vec7, massSpaceLocalInertia) / fixedDt;
			Vec3 vec10;
			vec10..ctor((float)(-(float)MathF.Sign(vec2.x)) * MathF.Min(vec5.x, vec9.x), (float)(-(float)MathF.Sign(vec2.y)) * MathF.Min(vec5.y, vec9.y), (float)(-(float)MathF.Sign(vec2.z)) * MathF.Min(vec5.z, vec9.z), -1f);
			mat = centerOfMassGlobalFrame.rotation;
			Vec3 vec11 = mat.TransformToParent(ref vec10);
			dragComputationResult.AngularDragTorqueGlobal = vec11;
		}

		// Token: 0x06000EAA RID: 3754 RVA: 0x000727C4 File Offset: 0x000709C4
		private static Vec3 ComputeVelocityFactorForClampingDrag(Vec3 absLinearVelocityLocal)
		{
			Vec3 vec;
			vec..ctor(7f, 20f, 20f, -1f);
			Vec3 zero = Vec3.Zero;
			for (int i = 0; i < 3; i++)
			{
				float num = absLinearVelocityLocal[i] - vec[i];
				if (num > 0f)
				{
					zero[i] = MathF.Pow(num, 4f);
				}
			}
			return zero;
		}

		// Token: 0x040008F3 RID: 2291
		public const byte VerticalPartitionCount = 3;

		// Token: 0x040008F4 RID: 2292
		public const byte HorizontalPartitionCount = 2;

		// Token: 0x040008FA RID: 2298
		private NavalPhysics.NavalPhysicsParameters _physicsParameters;

		// Token: 0x040008FC RID: 2300
		private float _stabilityAvgSubmergedHeight;

		// Token: 0x040008FD RID: 2301
		private int _stabilitySubmergedFloaterCount;

		// Token: 0x040008FE RID: 2302
		private float _minFloaterEntitialBottomPos;

		// Token: 0x040008FF RID: 2303
		private Scene _ownScene;

		// Token: 0x04000900 RID: 2304
		private float _maxFloaterEntitialTopPos;

		// Token: 0x04000901 RID: 2305
		private float _minimumFloaterDurabilityToFloatWhileNotSinking;

		// Token: 0x04000906 RID: 2310
		[EditableScriptComponentVariable(false, "")]
		public Vec3 AngularDragTerm;

		// Token: 0x04000907 RID: 2311
		[EditableScriptComponentVariable(true, "Sink")]
		private SimpleButton _sinkButton = new SimpleButton();

		// Token: 0x04000908 RID: 2312
		private float _angularDragYSideComponentTerm;

		// Token: 0x0400090A RID: 2314
		[EditableScriptComponentVariable(false, "")]
		public Vec3 AngularDampingTerm;

		// Token: 0x0400090B RID: 2315
		private float _angularDampingYSideComponentTerm;

		// Token: 0x0400090D RID: 2317
		private float _cachedMass;

		// Token: 0x0400090E RID: 2318
		private float[] _shipPartsDurabilities;

		// Token: 0x0400090F RID: 2319
		private NavalPhysics.ShipPart[] _floaterVolumesShipPartMap;

		// Token: 0x04000910 RID: 2320
		private float[] _shipPartsTargetDurabilities;

		// Token: 0x04000911 RID: 2321
		private VolumeDataForSubmergeComputation[] _floaterVolumeData;

		// Token: 0x04000912 RID: 2322
		private UIntPtr _floaterVolumeDataPinnedPointer = UIntPtr.Zero;

		// Token: 0x04000913 RID: 2323
		private GCHandle _floaterVolumeDataPinnedGCHandler;

		// Token: 0x04000914 RID: 2324
		private float _totalFloaterVolumeCached;

		// Token: 0x04000915 RID: 2325
		private ShipForceRecord _shipForceRecord;

		// Token: 0x04000916 RID: 2326
		private NavalPhysics.BuoyancyComputationResult _buoyancyComputationResult;

		// Token: 0x04000917 RID: 2327
		private NavalPhysics.DragForceComputationResult _dragComputationResult;

		// Token: 0x04000918 RID: 2328
		private MatrixFrame _anchorGlobalFrame;

		// Token: 0x04000919 RID: 2329
		private float _anchorForceMultiplier = 1f;

		// Token: 0x0400091A RID: 2330
		private Vec3 _weightedAgentsPosition;

		// Token: 0x0400091B RID: 2331
		private float _totalMass;

		// Token: 0x0400091C RID: 2332
		private Vec3 _committedWeightedAgentsPosition;

		// Token: 0x0400091D RID: 2333
		private float _committedTotalMass;

		// Token: 0x0400091E RID: 2334
		private NavalPhysics.WaterDriftForceData _continuousDriftForceData;

		// Token: 0x02000240 RID: 576
		public struct NavalPhysicsParameters
		{
			// Token: 0x0400100E RID: 4110
			public float OverrideMass;

			// Token: 0x0400100F RID: 4111
			public float MassMultiplier;

			// Token: 0x04001010 RID: 4112
			public Vec3 MomentOfInertiaMultiplier;

			// Token: 0x04001011 RID: 4113
			public float FloatingForceMultiplier;

			// Token: 0x04001012 RID: 4114
			public float MaximumSubmergedVolumeRatio;

			// Token: 0x04001013 RID: 4115
			public float ForwardDragMultiplier;

			// Token: 0x04001014 RID: 4116
			public LinearFrictionTerm LinearFrictionMultiplier;

			// Token: 0x04001015 RID: 4117
			public Vec3 AngularFrictionMultiplier;

			// Token: 0x04001016 RID: 4118
			public float TorqueMultiplierOfLateralBuoyantForces;

			// Token: 0x04001017 RID: 4119
			public Vec3 TorqueMultiplierOfVerticalBuoyantForces;

			// Token: 0x04001018 RID: 4120
			public float UpSideDownFrictionMultiplier;

			// Token: 0x04001019 RID: 4121
			public float MaxLinearSpeedForLateralDragCenterShift;

			// Token: 0x0400101A RID: 4122
			public float MaxLateralDragShift;

			// Token: 0x0400101B RID: 4123
			public float LateralDragShiftCriticalAngle;

			// Token: 0x0400101C RID: 4124
			public float StepAgentWeightMultiplier;

			// Token: 0x0400101D RID: 4125
			public bool MakeAgentsStepToEntityEvenUnderWater;
		}

		// Token: 0x02000241 RID: 577
		public struct BuoyancyComputationResult
		{
			// Token: 0x06001B94 RID: 7060 RVA: 0x000B8B18 File Offset: 0x000B6D18
			public void Reset()
			{
				this.PitchSubmergedAreaFactor = 0f;
				this.RollSubmergedAreaFactor = 0f;
				this.SubmergedHeightFactor = 0f;
				this.SubmergedFloaterCountFactor = 1f;
				this.AvgLocalBuoyancyApplyPosition = Vec3.Zero;
				this.NetGlobalBuoyancyForce = Vec3.Zero;
				this.NetBuoyancyTorque = Vec3.Zero;
				this.SimulatingAirFriction = false;
			}

			// Token: 0x0400101E RID: 4126
			public float PitchSubmergedAreaFactor;

			// Token: 0x0400101F RID: 4127
			public float RollSubmergedAreaFactor;

			// Token: 0x04001020 RID: 4128
			public float SubmergedHeightFactor;

			// Token: 0x04001021 RID: 4129
			public float SubmergedFloaterCountFactor;

			// Token: 0x04001022 RID: 4130
			public Vec3 AvgLocalBuoyancyApplyPosition;

			// Token: 0x04001023 RID: 4131
			public Vec3 NetGlobalBuoyancyForce;

			// Token: 0x04001024 RID: 4132
			public Vec3 NetBuoyancyTorque;

			// Token: 0x04001025 RID: 4133
			public bool SimulatingAirFriction;
		}

		// Token: 0x02000242 RID: 578
		public struct DragForceComputationResult
		{
			// Token: 0x06001B95 RID: 7061 RVA: 0x000B8B7C File Offset: 0x000B6D7C
			public void Reset()
			{
				this.CenterOfLateralDragLocal = Vec3.Zero;
				this.LateralDragForceGlobal = Vec3.Zero;
				this.CenterOfVerticalDragLocal = Vec3.Zero;
				this.VerticalDragForceGlobal = Vec3.Zero;
				this.CenterOfLongitudinalDragLocal = Vec3.Zero;
				this.LongitudinalDragForceGlobal = Vec3.Zero;
				this.AngularDragTorqueGlobal = Vec3.Zero;
				this.DriftForceFromAngularDragGlobal = Vec3.Zero;
			}

			// Token: 0x04001026 RID: 4134
			public Vec3 CenterOfLateralDragLocal;

			// Token: 0x04001027 RID: 4135
			public Vec3 LateralDragForceGlobal;

			// Token: 0x04001028 RID: 4136
			public Vec3 CenterOfVerticalDragLocal;

			// Token: 0x04001029 RID: 4137
			public Vec3 VerticalDragForceGlobal;

			// Token: 0x0400102A RID: 4138
			public Vec3 CenterOfLongitudinalDragLocal;

			// Token: 0x0400102B RID: 4139
			public Vec3 LongitudinalDragForceGlobal;

			// Token: 0x0400102C RID: 4140
			public Vec3 AngularDragTorqueGlobal;

			// Token: 0x0400102D RID: 4141
			public Vec3 DriftForceFromAngularDragGlobal;
		}

		// Token: 0x02000243 RID: 579
		public struct WaterDriftForceData
		{
			// Token: 0x06001B96 RID: 7062 RVA: 0x000B8BE4 File Offset: 0x000B6DE4
			public void Initialize()
			{
				this.DriftSpeed = 0f;
				this.DriftForceTimer = 0f;
				this.DriftRandom = new MBFastRandom();
				this.DriftForceTimer = this.DriftRandom.NextFloatRanged(0f, 31.415928f);
				this.ResultForce = Vec3.Zero;
			}

			// Token: 0x0400102E RID: 4142
			public float DriftSpeed;

			// Token: 0x0400102F RID: 4143
			public float DriftForceTimer;

			// Token: 0x04001030 RID: 4144
			public MBFastRandom DriftRandom;

			// Token: 0x04001031 RID: 4145
			public Vec3 ResultForce;
		}

		// Token: 0x02000244 RID: 580
		public enum ShipPart : byte
		{
			// Token: 0x04001033 RID: 4147
			LeftBack,
			// Token: 0x04001034 RID: 4148
			RightBack,
			// Token: 0x04001035 RID: 4149
			LeftMid,
			// Token: 0x04001036 RID: 4150
			RightMid,
			// Token: 0x04001037 RID: 4151
			LeftFront,
			// Token: 0x04001038 RID: 4152
			RightFront,
			// Token: 0x04001039 RID: 4153
			Count
		}

		// Token: 0x02000245 RID: 581
		public enum SinkingState : byte
		{
			// Token: 0x0400103B RID: 4155
			Floating,
			// Token: 0x0400103C RID: 4156
			Sinking,
			// Token: 0x0400103D RID: 4157
			Sunk
		}
	}
}
