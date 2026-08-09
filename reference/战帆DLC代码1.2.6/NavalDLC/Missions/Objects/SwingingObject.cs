using System;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.Objects
{
	// Token: 0x020000AC RID: 172
	public class SwingingObject : MissionObject
	{
		// Token: 0x06000D2C RID: 3372 RVA: 0x00068294 File Offset: 0x00066494
		internal SwingingObject()
		{
		}

		// Token: 0x06000D2D RID: 3373 RVA: 0x00068315 File Offset: 0x00066515
		public void DummyFunc()
		{
			Debug.Print(this._resetSimulation.ToString(), 0, 12, 17592186044416UL);
		}

		// Token: 0x06000D2E RID: 3374 RVA: 0x00068334 File Offset: 0x00066534
		private void InitAux()
		{
			this._swingingEntity = base.GameEntity.GetFirstChildEntityWithTag("swinging_entity");
			MatrixFrame globalFrame = base.GameEntity.Root.GetGlobalFrame();
			MatrixFrame globalFrame2 = base.GameEntity.GetGlobalFrame();
			this._frameWrtDynamicRoot = globalFrame.TransformToLocalNonOrthogonal(ref globalFrame2);
			this._ownerSceneCached = base.GameEntity.Scene;
			Vec3 origin = MBExtensions.GetFirstChildEntityWithName(base.GameEntity, "collision_sphere").GetLocalFrame().origin;
			origin.x = 0f;
			origin.y = -origin.y;
			origin.Normalize();
			this._minLimitXRotation = -Vec3.DotProduct(origin, Vec3.Forward);
			if (this._minLimitXRotation < -1.0471976f)
			{
				this._minLimitXRotation = -1.0471976f;
			}
		}

		// Token: 0x06000D2F RID: 3375 RVA: 0x0006840E File Offset: 0x0006660E
		protected override void OnInit()
		{
			this.InitAux();
		}

		// Token: 0x06000D30 RID: 3376 RVA: 0x00068416 File Offset: 0x00066616
		protected override void OnParallelFixedTick(float fixedDt)
		{
			if (fixedDt > 0f)
			{
				this.HandleSwingMotion(fixedDt);
			}
		}

		// Token: 0x06000D31 RID: 3377 RVA: 0x00068428 File Offset: 0x00066628
		protected override void OnEditorVariableChanged(string variableName)
		{
			if (variableName == "Reset Simulation")
			{
				this._prevSwing = Vec2.Zero;
				this._currSwing = Vec2.Zero;
				this._swingVelocity = Vec2.Zero;
				return;
			}
			if (variableName == "Test Collision")
			{
				this.InitAux();
				this._prevSwing.x = this._minLimitXRotation;
				this._currSwing.x = this._minLimitXRotation;
			}
		}

		// Token: 0x06000D32 RID: 3378 RVA: 0x00068499 File Offset: 0x00066699
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return 36;
		}

		// Token: 0x06000D33 RID: 3379 RVA: 0x000684A0 File Offset: 0x000666A0
		protected override void OnTickParallel(float dt)
		{
			if (this._ownerSceneCached.GetEnginePhysicsEnabled())
			{
				float num;
				float num2;
				this._ownerSceneCached.GetInterpolationFactorForBodyWorldTransformSmoothing(ref num, ref num2);
				MatrixFrame identity = MatrixFrame.Identity;
				identity.rotation.RotateAboutForward(MathF.Lerp(this._prevSwing.y, this._currSwing.y, num, 1E-05f));
				identity.rotation.RotateAboutSide(MathF.Lerp(this._prevSwing.x, this._currSwing.x, num, 1E-05f));
				this._swingingEntity.SetFrame(ref identity, false);
			}
		}

		// Token: 0x06000D34 RID: 3380 RVA: 0x00068538 File Offset: 0x00066738
		private void HandleSwingMotion(float fixedDt)
		{
			Vec3 vec = Vec3.Zero;
			MatrixFrame matrixFrame;
			MatrixFrame matrixFrame2;
			if (GameEntityPhysicsExtensions.HasPhysicsBody(base.GameEntity.Root))
			{
				matrixFrame = base.GameEntity.Root.GetBodyWorldTransform();
				matrixFrame2 = matrixFrame.TransformToParent(ref this._frameWrtDynamicRoot);
				vec = GameEntityPhysicsExtensions.GetLinearVelocityAtGlobalPointForEntityWithDynamicBody(base.GameEntity.Root, matrixFrame2.origin);
			}
			else
			{
				matrixFrame = base.GameEntity.Root.GetFrame();
				matrixFrame2 = matrixFrame.TransformToParent(ref this._frameWrtDynamicRoot);
			}
			Vec3 vec2 = (vec - this._parentPrevVelocity) / fixedDt;
			this._parentPrevVelocity = vec;
			Vec3 vec3 = MBGlobals.GravitationalAcceleration - vec2 + this._accumulatedAcceleration;
			this._accumulatedAcceleration = Vec3.Zero;
			matrixFrame = this._swingingEntity.GetFrame();
			MatrixFrame matrixFrame3 = matrixFrame2.TransformToParent(ref matrixFrame);
			Vec3 origin = matrixFrame3.origin;
			Vec3 vec4 = origin + matrixFrame3.rotation.u * this._centerOfMassHeight;
			Vec3 vec5 = matrixFrame3.TransformToLocalNonOrthogonal(ref origin);
			Vec3 vec6 = matrixFrame3.TransformToLocalNonOrthogonal(ref vec4);
			Vec3 vec7 = matrixFrame3.rotation.TransformToLocal(ref vec3);
			Vec3 vec8 = vec6 - vec5;
			Vec3 vec9 = vec7 * this._mass;
			Vec3 vec10 = Vec3.CrossProduct(vec8, vec9);
			float num = MathF.Max(this._momentOfInertia * this._mass, 0.001f);
			Vec3 side = Vec3.Side;
			float num2 = Vec3.DotProduct(vec10, side) / num;
			this._swingVelocity.x = this._swingVelocity.x + num2 * fixedDt;
			Vec3 forward = Vec3.Forward;
			float num3 = Vec3.DotProduct(vec10, forward) / num;
			this._swingVelocity.y = this._swingVelocity.y + num3 * fixedDt;
			if (MathF.Abs(this._swingVelocity.x) > 5f)
			{
				this._swingVelocity.x = 5f * (float)MathF.Sign(this._swingVelocity.x);
			}
			if (MathF.Abs(this._swingVelocity.y) > 5f)
			{
				this._swingVelocity.y = 5f * (float)MathF.Sign(this._swingVelocity.y);
			}
			this._prevSwing = this._currSwing;
			this._currSwing += this._swingVelocity * fixedDt;
			if (this._currSwing.x > 1.0471976f && this._swingVelocity.x > 0f)
			{
				this._swingVelocity.x = this._swingVelocity.x * -0.1f;
			}
			if (this._currSwing.x < this._minLimitXRotation)
			{
				this._currSwing.x = this._minLimitXRotation;
				if (this._swingVelocity.x < 0f)
				{
					this._swingVelocity.x = this._swingVelocity.x * -0.1f;
				}
			}
			if (this._currSwing.y > 1.0471976f)
			{
				this._currSwing.y = 1.0471976f;
				if (this._swingVelocity.y > 0f)
				{
					this._swingVelocity.y = this._swingVelocity.y * -0.1f;
				}
			}
			if (this._currSwing.y < -1.0471976f)
			{
				this._currSwing.y = -1.0471976f;
				if (this._swingVelocity.y < 0f)
				{
					this._swingVelocity.y = this._swingVelocity.y * -0.1f;
				}
			}
			Vec2 swingVelocity = this._swingVelocity;
			float num4 = swingVelocity.Normalize();
			float num5 = (this._damping * 0.2f * num4 + this._damping * 0.03f) / this._mass;
			if (num5 > num4)
			{
				this._swingVelocity = Vec2.Zero;
				return;
			}
			this._swingVelocity -= swingVelocity * num5;
		}

		// Token: 0x06000D35 RID: 3381 RVA: 0x00068904 File Offset: 0x00066B04
		protected override bool OnHit(Agent attackerAgent, int damage, Vec3 impactPosition, Vec3 impactDirection, in MissionWeapon weapon, int affectorWeaponSlotOrMissileIndex, ScriptComponentBehavior attackerScriptComponentBehavior, out bool reportDamage, out float finalDamage, out float fireDamage, out float modifiedFireDamage)
		{
			MissionWeapon missionWeapon = weapon;
			float num;
			if (missionWeapon.Item == null)
			{
				num = 1f;
			}
			else
			{
				missionWeapon = weapon;
				num = missionWeapon.GetWeight();
			}
			float num2 = num;
			num2 = MathF.Clamp(num2, 0.5f, 2f);
			Vec3 vec = impactDirection * num2 * 300f;
			this._accumulatedAcceleration += vec / this._mass;
			reportDamage = false;
			finalDamage = 0f;
			fireDamage = -1f;
			modifiedFireDamage = -1f;
			return true;
		}

		// Token: 0x04000823 RID: 2083
		[EditableScriptComponentVariable(true, "Damping")]
		public float _damping = 5f;

		// Token: 0x04000824 RID: 2084
		[EditableScriptComponentVariable(true, "Center of Mass Height")]
		public float _centerOfMassHeight = -0.8f;

		// Token: 0x04000825 RID: 2085
		[EditableScriptComponentVariable(true, "Mass")]
		public float _mass = 1f;

		// Token: 0x04000826 RID: 2086
		[EditableScriptComponentVariable(true, "Moment Of Inertia")]
		public float _momentOfInertia = 0.5f;

		// Token: 0x04000827 RID: 2087
		[EditableScriptComponentVariable(true, "Reset Simulation")]
		public SimpleButton _resetSimulation = new SimpleButton();

		// Token: 0x04000828 RID: 2088
		[EditableScriptComponentVariable(true, "Test Collision")]
		public SimpleButton _testCollision = new SimpleButton();

		// Token: 0x04000829 RID: 2089
		private Vec2 _currSwing;

		// Token: 0x0400082A RID: 2090
		private Vec2 _prevSwing;

		// Token: 0x0400082B RID: 2091
		private Vec2 _swingVelocity;

		// Token: 0x0400082C RID: 2092
		private float _minLimitXRotation;

		// Token: 0x0400082D RID: 2093
		private Vec3 _accumulatedAcceleration = Vec3.Zero;

		// Token: 0x0400082E RID: 2094
		private WeakGameEntity _swingingEntity = WeakGameEntity.Invalid;

		// Token: 0x0400082F RID: 2095
		private Vec3 _parentPrevVelocity = Vec3.Zero;

		// Token: 0x04000830 RID: 2096
		private MatrixFrame _frameWrtDynamicRoot = MatrixFrame.Identity;

		// Token: 0x04000831 RID: 2097
		private Scene _ownerSceneCached;
	}
}
