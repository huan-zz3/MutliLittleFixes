using System;
using System.Collections.Generic;
using NavalDLC.Missions.NavalPhysics;
using NavalDLC.Missions.Objects;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.ShipActuators
{
	// Token: 0x02000092 RID: 146
	public class MissionSail : MissionObject
	{
		// Token: 0x170001BB RID: 443
		// (get) Token: 0x06000A43 RID: 2627 RVA: 0x00047ABD File Offset: 0x00045CBD
		public override TextObject HitObjectName
		{
			get
			{
				return new TextObject("{=92jVTPDA}Ship Sails", null);
			}
		}

		// Token: 0x170001BC RID: 444
		// (get) Token: 0x06000A44 RID: 2628 RVA: 0x00047ACA File Offset: 0x00045CCA
		public ShipSail SailObject
		{
			get
			{
				return this._sailObject;
			}
		}

		// Token: 0x170001BD RID: 445
		// (get) Token: 0x06000A45 RID: 2629 RVA: 0x00047AD2 File Offset: 0x00045CD2
		public ShipForce Force
		{
			get
			{
				return this._force;
			}
		}

		// Token: 0x170001BE RID: 446
		// (get) Token: 0x06000A46 RID: 2630 RVA: 0x00047ADA File Offset: 0x00045CDA
		public float LocalSailRotation
		{
			get
			{
				return this._localSailRotation;
			}
		}

		// Token: 0x170001BF RID: 447
		// (get) Token: 0x06000A47 RID: 2631 RVA: 0x00047AE2 File Offset: 0x00045CE2
		// (set) Token: 0x06000A48 RID: 2632 RVA: 0x00047AEA File Offset: 0x00045CEA
		public float Setting { get; private set; }

		// Token: 0x170001C0 RID: 448
		// (get) Token: 0x06000A49 RID: 2633 RVA: 0x00047AF3 File Offset: 0x00045CF3
		// (set) Token: 0x06000A4A RID: 2634 RVA: 0x00047AFB File Offset: 0x00045CFB
		public float TargetSailSetting { get; private set; }

		// Token: 0x170001C1 RID: 449
		// (get) Token: 0x06000A4B RID: 2635 RVA: 0x00047B04 File Offset: 0x00045D04
		public Vec3 CenterOfSailForceShipLocal
		{
			get
			{
				return this._centerOfSailForceShipLocal;
			}
		}

		// Token: 0x170001C2 RID: 450
		// (get) Token: 0x06000A4C RID: 2636 RVA: 0x00047B0C File Offset: 0x00045D0C
		public float FoldDuration
		{
			get
			{
				return this._sailVisual.TotalFoldDuration;
			}
		}

		// Token: 0x170001C3 RID: 451
		// (get) Token: 0x06000A4D RID: 2637 RVA: 0x00047B19 File Offset: 0x00045D19
		public float UnfoldDuration
		{
			get
			{
				return this._sailVisual.TotalUnfoldDuration;
			}
		}

		// Token: 0x170001C4 RID: 452
		// (get) Token: 0x06000A4E RID: 2638 RVA: 0x00047B26 File Offset: 0x00045D26
		// (set) Token: 0x06000A4F RID: 2639 RVA: 0x00047B2E File Offset: 0x00045D2E
		public GameEntity SailEntity { get; private set; }

		// Token: 0x170001C5 RID: 453
		// (get) Token: 0x06000A50 RID: 2640 RVA: 0x00047B37 File Offset: 0x00045D37
		public float Area
		{
			get
			{
				return this._width * ((this._sailObject.Type == 1) ? (this._height * 0.5f) : this._height);
			}
		}

		// Token: 0x06000A52 RID: 2642 RVA: 0x00047B6C File Offset: 0x00045D6C
		internal void InitWithVariables(ShipSail sailObject, MissionShip ownerShip, SailVisual sailVisual)
		{
			this._sailObject = sailObject;
			this._ownerShip = ownerShip;
			this._sailVisual = sailVisual;
			this.SailEntity = GameEntity.CreateFromWeakEntity(sailVisual.GameEntity);
			this.InitializeCenterOfSailForceLocal();
			this.Setting = 0f;
			this._sailRotationStateTimer = 7f;
			this._fullSailWeight = 0f;
			this._localSailRotation = (-this._sailObject.RightRotationLimit + this._sailObject.LeftRotationLimit) * 0.5f;
			this._localSailRotation = MathF.Clamp(this._localSailRotation, -this._sailObject.RightRotationLimit, this._sailObject.LeftRotationLimit);
			this._targetSailRotation = this._localSailRotation;
			this._currentSailRotationSpeed = 0f;
			this.TargetSailSetting = 1f;
			this._currentSailTurningState = MissionSail.SailTurningState.Stationary;
			this._gustMode = false;
			this.SetVisualSailEnabled(false);
			this.InitializeSailSounds();
			this.InitSailRotationAccordingToWindDirection();
		}

		// Token: 0x06000A53 RID: 2643 RVA: 0x00047C58 File Offset: 0x00045E58
		private void InitSailRotationAccordingToWindDirection()
		{
			Vec2 globalWindVelocity = this._ownerShip.Scene.GetGlobalWindVelocity();
			if (globalWindVelocity.LengthSquared > 1f)
			{
				MatrixFrame matrixFrame = ref this._ownerShip.GameEntity.GetGlobalFrame();
				Vec3 vec = globalWindVelocity.ToVec3(0f);
				Vec2 vec2 = matrixFrame.rotation.TransformToLocal(ref vec).AsVec2.Normalized();
				this.FixedTickTargetSailRotation(vec2, true);
				this._localSailRotation = this._targetSailRotation;
			}
		}

		// Token: 0x06000A54 RID: 2644 RVA: 0x00047CDC File Offset: 0x00045EDC
		public bool CheckSailFlags(bool editMode)
		{
			List<GameEntity> list = new List<GameEntity>();
			this.SailEntity.GetChildrenRecursive(ref list);
			bool flag = false;
			list.Add(this.SailEntity);
			foreach (GameEntity gameEntity in list)
			{
				if (!Extensions.HasAnyFlag<EntityFlags>(gameEntity.EntityFlags, 131072) && !Extensions.HasAnyFlag<EntityFlags>(gameEntity.EntityFlags, 4096))
				{
					flag = true;
				}
			}
			if (flag)
			{
				string text = string.Concat(new string[]
				{
					"In Root Entity ",
					this.SailEntity.Root.Name,
					", ",
					this.SailEntity.Name,
					"'s every descendant including itself must have Does not Affect Parent's Local Bounding Box flag."
				});
				if (editMode)
				{
					MBEditor.AddEntityWarning(this.SailEntity.WeakEntity, text);
				}
			}
			return flag;
		}

		// Token: 0x06000A55 RID: 2645 RVA: 0x00047DCC File Offset: 0x00045FCC
		public void UpdateForcedWindOfSailsAndTopBanner(float dt)
		{
			Vec3 linearVelocity = this._ownerShip.Physics.LinearVelocity;
			Vec3 angularVelocity = this._ownerShip.Physics.AngularVelocity;
			MatrixFrame bodyWorldTransform = this._ownerShip.GameEntity.GetBodyWorldTransform();
			Vec3 localCenterOfMass = this._ownerShip.Physics.LocalCenterOfMass;
			Vec3 vec = bodyWorldTransform.TransformToParent(ref localCenterOfMass);
			Vec3 vec2 = this._ownerShip.Scene.GetGlobalWindVelocity().ToVec3(0f);
			Vec3 vec3 = this.ComputeCenterOfSailForceGlobal() - vec;
			Vec3 vec4 = Vec3.CrossProduct(angularVelocity, vec3) + linearVelocity;
			Vec3 vec5 = vec2 - vec4;
			Vec3 vec6 = this._sailVisual.SailTopBannerEntity.GetGlobalFrame().origin - vec;
			Vec3 vec7 = Vec3.CrossProduct(angularVelocity, vec6) + linearVelocity;
			Vec3 vec8 = vec2 - vec7;
			Vec3 vec9;
			if (this._force.IsApplicable)
			{
				vec9 = this._force.Force / this._force.GamifiedForceMultiplier;
			}
			else
			{
				vec9 = Vec3.Zero;
			}
			this._sailVisual.UpdateForcedWindOfSailsAndTopBanner(dt, vec8, in vec5, in vec9);
		}

		// Token: 0x06000A56 RID: 2646 RVA: 0x00047EEF File Offset: 0x000460EF
		private void SetTargetSailSetting(in ShipActuatorRecord actuatorInput)
		{
			if (this._sailObject.Type == null)
			{
				this.TargetSailSetting = actuatorInput.SquareSailSetting;
				return;
			}
			if (this._sailObject.Type == 1)
			{
				this.TargetSailSetting = actuatorInput.LateenSailSetting;
			}
		}

		// Token: 0x06000A57 RID: 2647 RVA: 0x00047F28 File Offset: 0x00046128
		private void FixedUpdateSailForce(Vec3 windVelocityGlobal, Vec3 sailLinearVelocityGlobal, Vec3 sailLinearVelocityFromAngularGlobal)
		{
			MatrixFrame bodyWorldTransform = this._ownerShip.GameEntity.GetBodyWorldTransform();
			Vec3 vec = this.Compute3DSailDirection();
			Vec3 vec2 = Vec3.Zero;
			if (bodyWorldTransform.rotation.u.z > 0f)
			{
				Vec3 vec3 = windVelocityGlobal * bodyWorldTransform.rotation.u.z * bodyWorldTransform.rotation.u.z - sailLinearVelocityGlobal;
				float num = 16f;
				if (vec3.LengthSquared > num * num)
				{
					vec3 = vec3.NormalizedCopy() * num;
				}
				Vec2 asVec = bodyWorldTransform.rotation.TransformToLocal(ref vec3).AsVec2;
				float num2 = asVec.Normalize();
				Vec2 vec4 = bodyWorldTransform.rotation.TransformToLocal(ref vec).AsVec2.Normalized();
				float num3 = MathF.Abs(bodyWorldTransform.rotation.u.z);
				float num4 = this.Setting * this.Area * num3;
				float num5 = num2 * num3;
				Vec3 vec5 = MissionSail.ComputeSailForce(in vec4, in asVec, num5, in bodyWorldTransform, num4, this._sailObject.Type);
				if (this._gustMode)
				{
					vec5 *= 0.5f;
				}
				vec2 += vec5;
			}
			Vec3 vec6 = -sailLinearVelocityFromAngularGlobal;
			Vec2 asVec2 = bodyWorldTransform.rotation.TransformToLocal(ref vec6).AsVec2;
			float num6 = asVec2.Normalize();
			Vec2 vec7 = bodyWorldTransform.rotation.TransformToLocal(ref vec).AsVec2.Normalized();
			float num7 = this.Setting * this.Area;
			float num8 = num6;
			Vec3 vec8 = MissionSail.ComputeSailForce(in vec7, in asVec2, num8, in bodyWorldTransform, num7, this._sailObject.Type);
			if (bodyWorldTransform.rotation.u.z <= 0f)
			{
				vec8 *= 2f;
			}
			vec2 += vec8;
			float num9 = (1f + this._ownerShip.ShipOrigin.SailForceFactor) * this._sailObject.ForceMultiplier;
			vec2 *= num9;
			float num10 = vec2.Normalize();
			float num11 = MissionGameModels.Current.MissionShipParametersModel.CalculateWindBonus(this._ownerShip.ShipOrigin, this._ownerShip.Captain, num10);
			float num12 = ((num10 > 0f) ? (num11 / num10) : 1f);
			num9 *= num12;
			vec2 *= num11;
			this._force = new ShipForce(in this._centerOfSailForceShipLocal, in vec2, ShipForce.SourceType.Sail, num9);
		}

		// Token: 0x06000A58 RID: 2648 RVA: 0x000481B0 File Offset: 0x000463B0
		public void FixedUpdate(float fixedDt, in ShipActuatorRecord actuatorInput, in Vec3 shipLinearVelocityGlobal, in Vec3 shipAngularVelocityGlobal)
		{
			if (this._ownerShip.ShipSailState != MissionShip.SailState.Intact)
			{
				this._force = new ShipForce(in this._centerOfSailForceShipLocal, in Vec3.Zero, ShipForce.SourceType.Sail, 1f);
				return;
			}
			MatrixFrame bodyWorldTransform = this._ownerShip.GameEntity.GetBodyWorldTransform();
			Vec3 localCenterOfMass = this._ownerShip.Physics.LocalCenterOfMass;
			Vec3 vec = bodyWorldTransform.TransformToParent(ref localCenterOfMass);
			Vec3 vec2 = this.ComputeCenterOfSailForceGlobal() - vec;
			Vec3 vec3 = Vec3.CrossProduct(shipAngularVelocityGlobal, vec2);
			Vec3 vec4 = shipLinearVelocityGlobal;
			Vec3 vec5 = this._ownerShip.GameEntity.GetGlobalWindVelocityOfScene().ToVec3(0f);
			Vec3 vec6 = vec5 - vec4;
			this.SetTargetSailSetting(in actuatorInput);
			float localSailRotation = this._localSailRotation;
			this.FixedUpdateSailRotation(fixedDt, in actuatorInput, in vec6);
			if (this.TargetSailSetting == 1f)
			{
				Vec3 force = this._force.Force;
				this.FixedUpdateSailForce(vec5, vec4, vec3);
				if (this._ownerShip.ShouldUpdateSoundPos && this._blowSoundEventCooldown <= 0.01f && this._force.Force.LengthSquared / force.LengthSquared > 1.21f)
				{
					this._shouldMakeBlowingSound = true;
					this._blowSoundEventCooldown += 10f;
				}
				this.CalculateSailSoundEventRotationParamAndShouldUpdateSoundPos(fixedDt, MathF.Abs(this._localSailRotation - localSailRotation));
				this._blowSoundEventCooldown -= fixedDt;
				this._blowSoundEventCooldown = ((this._blowSoundEventCooldown < 0f) ? 0f : this._blowSoundEventCooldown);
				return;
			}
			this._force = new ShipForce(in this._centerOfSailForceShipLocal, in Vec3.Zero, ShipForce.SourceType.Sail, 1f);
		}

		// Token: 0x06000A59 RID: 2649 RVA: 0x00048360 File Offset: 0x00046560
		private void UpdateSailRotationVisual(float dt)
		{
			float num = this._targetSailRotation - this._localSailRotation;
			float num2 = Math.Abs(num);
			float num3 = dt * this._currentSailRotationSpeed;
			num3 = Math.Min(num2, num3);
			float num4 = this._localSailRotation + (float)Math.Sign(num) * num3;
			this._localSailRotation = MathF.Clamp(num4, -this._sailObject.RightRotationLimit, this._sailObject.LeftRotationLimit);
		}

		// Token: 0x06000A5A RID: 2650 RVA: 0x000483C8 File Offset: 0x000465C8
		private void UpdateSailSetting(float dt)
		{
			float targetSailSetting = this.TargetSailSetting;
			float num = ((targetSailSetting - this.Setting >= 0f) ? this.UnfoldDuration : this.FoldDuration);
			float num2 = 1f / num;
			this.Setting = ShipActuators.ComputeActuatorParameter(this.Setting, targetSailSetting, dt, num2 * (1f + this._ownerShip.ShipOrigin.FurlUnfurlSpeedFactor));
			this.Setting = MathF.Clamp(this.Setting, 0f, 1f);
		}

		// Token: 0x06000A5B RID: 2651 RVA: 0x00048448 File Offset: 0x00046648
		private void UpdateSailVisuals(float dt)
		{
			MatrixFrame localFrame = this._sailVisual.SailYawRotationEntity.GetLocalFrame();
			localFrame.rotation = Mat3.Identity;
			localFrame.rotation.RotateAboutUp(this._localSailRotation);
			this._sailVisual.SailYawRotationEntity.SetLocalFrame(ref localFrame, false);
			this.SetVisualSailEnabled(this.TargetSailSetting > 0.5f);
			this.UpdateForcedWindOfSailsAndTopBanner(dt);
		}

		// Token: 0x06000A5C RID: 2652 RVA: 0x000484B4 File Offset: 0x000466B4
		private void UpdateSoundPos()
		{
			if (this._ownerShip.ShouldUpdateSoundPos && this._sailContinuousSoundEvent == null)
			{
				this._sailContinuousSoundEvent = SoundEvent.CreateEvent(MissionSail._sailContinuousSoundEventId, this._ownerShip.GameEntity.Scene);
				this._sailRotationSoundEvent = SoundEvent.CreateEvent(MissionSail._sailRotationSoundEventId, this._ownerShip.GameEntity.Scene);
				Vec3 vec = this.ComputeCenterOfSailForceGlobal();
				this._sailContinuousSoundEvent.SetPosition(vec);
				this._sailRotationSoundEvent.SetPosition(vec);
				this._sailRotationSoundEvent.SetParameter("SailRotation", this._sailSoundEventRotationParam);
				this._sailRotationSoundEvent.Play();
				this._sailContinuousSoundEvent.Play();
				return;
			}
			if (this._ownerShip.ShouldUpdateSoundPos)
			{
				Vec3 vec2 = this.ComputeCenterOfSailForceGlobal();
				this._sailContinuousSoundEvent.SetPosition(vec2);
				this._sailRotationSoundEvent.SetPosition(vec2);
				this._sailRotationSoundEvent.SetParameter("SailRotation", this._sailSoundEventRotationParam);
				if (this._shouldMakeBlowingSound)
				{
					SoundManager.StartOneShotEvent("event:/mission/movement/vessel/sail/sail_blow", ref this.SailEntity.GetGlobalFrame().origin);
					this._shouldMakeBlowingSound = false;
					return;
				}
			}
			else if (this._sailContinuousSoundEvent != null)
			{
				this._sailRotationSoundEvent.Stop();
				this._sailContinuousSoundEvent.Stop();
				this._sailRotationSoundEvent = null;
				this._sailContinuousSoundEvent = null;
			}
		}

		// Token: 0x06000A5D RID: 2653 RVA: 0x0004860C File Offset: 0x0004680C
		public void Update(float dt)
		{
			this.UpdateSailRotationVisual(dt);
			this.UpdateSailSetting(dt);
			this.UpdateSailVisuals(dt);
			this.UpdateSoundPos();
		}

		// Token: 0x06000A5E RID: 2654 RVA: 0x0004862C File Offset: 0x0004682C
		public static Vec3 ComputeSailForce(in Vec2 sailDirection2DShip, in Vec2 relWindDirection2DShip, float relWindSpeed2DShip, in MatrixFrame shipFrame, float effectiveSailArea, SailType sailType)
		{
			Vec2 sailForceCoefficients = SailWindProfile.Instance.GetSailForceCoefficients(sailType, sailDirection2DShip, relWindDirection2DShip);
			float num = relWindSpeed2DShip * relWindSpeed2DShip;
			float airDensity = GameModels.Instance.ShipPhysicsParametersModel.GetAirDensity();
			Vec2 vec = 0.5f * airDensity * num * sailForceCoefficients * effectiveSailArea;
			Mat3 rotation = shipFrame.rotation;
			Vec3 vec2 = vec.ToVec3(0f);
			return rotation.TransformToParent(ref vec2);
		}

		// Token: 0x06000A5F RID: 2655 RVA: 0x0004869C File Offset: 0x0004689C
		public float ComputeMaximumForceMagnitudeSailCanApply()
		{
			Vec2 maximumSailForceCoefficients = SailWindProfile.Instance.GetMaximumSailForceCoefficients(this._sailObject.Type);
			float maximumWindSpeed = Scene.MaximumWindSpeed;
			float area = this.Area;
			return 0.5f * GameModels.Instance.ShipPhysicsParametersModel.GetAirDensity() * maximumWindSpeed * maximumWindSpeed * (this._sailObject.ForceMultiplier * maximumSailForceCoefficients.Length) * area * (1f + this._ownerShip.ShipOrigin.SailForceFactor);
		}

		// Token: 0x06000A60 RID: 2656 RVA: 0x00048714 File Offset: 0x00046914
		private Vec3 ComputeWindVectorForSailVisuals(in Vec3 sailForceGlobal)
		{
			Vec3 vec = sailForceGlobal;
			Vec3 vec2 = vec.NormalizedCopy();
			vec = sailForceGlobal;
			float num = MathF.Sqrt(vec.Length * 2f / (GameModels.Instance.ShipPhysicsParametersModel.GetAirDensity() * this._sailObject.ForceMultiplier * this.Area));
			return vec2 * num;
		}

		// Token: 0x06000A61 RID: 2657 RVA: 0x00048772 File Offset: 0x00046972
		private void SetVisualSailEnabled(bool visualSailEnabled)
		{
			this._sailVisual.SailEnabled = visualSailEnabled;
		}

		// Token: 0x06000A62 RID: 2658 RVA: 0x00048780 File Offset: 0x00046980
		private void FixedTickFullSailInputWeight(float fixedDt, in ShipActuatorRecord actuatorInput)
		{
			float num = actuatorInput.RowerThrust;
			if (this.TargetSailSetting <= 0f || (!this._gustMode && this.TargetSailSetting < 1f))
			{
				num = 0f;
			}
			if (num > 0f)
			{
				float rowerThrustDoubleTap = actuatorInput.RowerThrustDoubleTap;
				if (this._fullSailWeight >= 0f)
				{
					this._fullSailWeight += fixedDt * 0.4f;
					if (rowerThrustDoubleTap > 0f && this._fullSailWeight < 0.5f)
					{
						this._fullSailWeight = 0.5f;
					}
				}
				else
				{
					this._fullSailWeight += fixedDt * 2f;
					this._fullSailMode = false;
				}
				if (this._fullSailWeight >= 1f)
				{
					this._fullSailMode = true;
					this._fullSailWeight = 1f;
					return;
				}
			}
			else if (num < 0f)
			{
				if (this._fullSailWeight <= 0f)
				{
					this._fullSailWeight -= fixedDt * 0.4f;
				}
				else
				{
					this._fullSailWeight -= fixedDt * 2f;
					this._fullSailMode = false;
				}
				if (this._fullSailWeight <= -1f)
				{
					this._fullSailMode = true;
					this._fullSailWeight = -1f;
					return;
				}
			}
			else
			{
				float num2 = fixedDt * 2f;
				if (MathF.Abs(this._fullSailWeight) <= num2)
				{
					this._fullSailMode = false;
					this._fullSailWeight = 0f;
					return;
				}
				this._fullSailWeight -= (float)MathF.Sign(this._fullSailWeight) * num2;
			}
		}

		// Token: 0x06000A63 RID: 2659 RVA: 0x000488F9 File Offset: 0x00046AF9
		public bool GetVisualSailEnabled()
		{
			return this._sailVisual.SailEnabled;
		}

		// Token: 0x06000A64 RID: 2660 RVA: 0x00048908 File Offset: 0x00046B08
		private void FixedTickTargetSailRotation(Vec2 relWindDirection2DShip, bool forceFindTheBestAngle)
		{
			float num = ((this._currentSailTurningState != MissionSail.SailTurningState.Stationary) ? this._targetSailRotation : this._localSailRotation);
			Vec2 forward = Vec2.Forward;
			forward.RotateCCW(num);
			float num2 = SailWindProfile.Instance.ComputeSailThrustValue(this._sailObject.Type, forward, Vec2.Forward, relWindDirection2DShip);
			float num3 = num;
			float num4 = 1f;
			if (!forceFindTheBestAngle && !this._gustMode && this._currentSailTurningState == MissionSail.SailTurningState.Stationary)
			{
				if (this._fullSailMode && this._fullSailWeight > 0f)
				{
					num4 = 1.1f;
				}
				else
				{
					num4 = 1.3f;
				}
			}
			float num5 = num2 * num4;
			float num6 = -this._sailObject.RightRotationLimit;
			float num7 = this._sailObject.LeftRotationLimit;
			if (this._currentSailTurningState == MissionSail.SailTurningState.TurningLeft)
			{
				num6 = this._localSailRotation;
			}
			else if (this._currentSailTurningState == MissionSail.SailTurningState.TurningRight)
			{
				num7 = this._localSailRotation;
			}
			float num8 = (num7 - num6) * 0.01f;
			if (num7 - num6 > 0.10471976f)
			{
				for (float num9 = num6; num9 <= num7; num9 += num8)
				{
					Vec2 forward2 = Vec2.Forward;
					forward2.RotateCCW(num9);
					float num10 = SailWindProfile.Instance.ComputeSailThrustValue(this._sailObject.Type, forward2, Vec2.Forward, relWindDirection2DShip);
					float num11 = num10;
					if (num11 > num5)
					{
						num5 = num11;
						num2 = num10;
						num3 = num9;
					}
				}
				if (forceFindTheBestAngle)
				{
					if (num2 > 0f)
					{
						this._targetSailRotation = num3;
					}
				}
				else if (!this._gustMode || num2 > 0f)
				{
					this._targetSailRotation = num3;
				}
			}
			this._targetSailRotation = MathF.Clamp(this._targetSailRotation, -this._sailObject.RightRotationLimit, this._sailObject.LeftRotationLimit);
		}

		// Token: 0x06000A65 RID: 2661 RVA: 0x00048AA4 File Offset: 0x00046CA4
		private void FixedTickSailGustMode(float thrustDirection, float curSailThrustValue, float maxThrustValue)
		{
			if (thrustDirection >= 0f)
			{
				if (this._fullSailMode && this._fullSailWeight > 0f)
				{
					this._gustMode = curSailThrustValue < 0f;
					return;
				}
				if (this._gustMode && (curSailThrustValue > 0f || maxThrustValue > 0f || curSailThrustValue * this._fullSailWeight > 0f))
				{
					this._gustMode = false;
					return;
				}
			}
			else
			{
				this._gustMode = true;
			}
		}

		// Token: 0x06000A66 RID: 2662 RVA: 0x00048B14 File Offset: 0x00046D14
		private Vec3 Compute3DSailDirection()
		{
			MatrixFrame bodyWorldTransform = this._ownerShip.GameEntity.GetBodyWorldTransform();
			Vec3 f = bodyWorldTransform.rotation.f;
			Vec3 u = bodyWorldTransform.rotation.u;
			Vec3 vec = f.RotateAboutAnArbitraryVector(u, this._localSailRotation);
			vec.Normalize();
			return vec;
		}

		// Token: 0x06000A67 RID: 2663 RVA: 0x00048B64 File Offset: 0x00046D64
		private void InitializeCenterOfSailForceLocal()
		{
			MatrixFrame bodyWorldTransform = this._ownerShip.GameEntity.GetBodyWorldTransform();
			this._sailVisual.GetDimensions(in bodyWorldTransform, this._sailObject.Type == 1, out this._width, out this._height, out this._centerOfSailForceShipLocal);
		}

		// Token: 0x06000A68 RID: 2664 RVA: 0x00048BB4 File Offset: 0x00046DB4
		private void FixedUpdateSailRotation(float fixedDt, in ShipActuatorRecord actuatorInput, in Vec3 relWindVelocityGlobal)
		{
			Vec2 vec = this._ownerShip.GameEntity.GetBodyWorldTransform().rotation.TransformToLocal(ref relWindVelocityGlobal).AsVec2.Normalized();
			float rowerThrust = actuatorInput.RowerThrust;
			if (this.TargetSailSetting <= 0f)
			{
				this._sailRotationStateTimer = float.MaxValue;
			}
			this.FixedTickFullSailInputWeight(fixedDt, in actuatorInput);
			bool flag = this.TargetSailSetting == 1f && this.Setting == 0f;
			if (flag)
			{
				this._sailRotationStateTimer = 0f;
			}
			if (this._fullSailMode && this._fullSailWeight > 0f && this._sailRotationStateTimer > 2f && this._currentSailTurningState == MissionSail.SailTurningState.Stationary)
			{
				this._sailRotationStateTimer = 2f;
			}
			this._sailRotationStateTimer -= fixedDt;
			Vec2 forward = Vec2.Forward;
			forward.RotateCCW(this._localSailRotation);
			float num = SailWindProfile.Instance.ComputeSailThrustValue(this._sailObject.Type, forward, Vec2.Forward, vec);
			bool flag2 = (this._currentSailTurningState != MissionSail.SailTurningState.Stationary || this._sailRotationStateTimer <= 0f) && this.TargetSailSetting >= 1f;
			float num2 = num;
			if (flag2)
			{
				this.FixedTickTargetSailRotation(vec, flag);
				Vec2 forward2 = Vec2.Forward;
				forward2.RotateCCW(this._targetSailRotation);
				float num3 = SailWindProfile.Instance.ComputeSailThrustValue(this._sailObject.Type, forward2, Vec2.Forward, vec);
				num2 = MathF.Max(num2, num3);
				if (this._currentSailTurningState == MissionSail.SailTurningState.Stationary && !MBMath.ApproximatelyEqualsTo(this._targetSailRotation, this._localSailRotation, 0.10471976f))
				{
					this._sailRotationStateTimer = 30f;
					this._currentSailTurningState = ((this._targetSailRotation < this._localSailRotation) ? MissionSail.SailTurningState.TurningRight : MissionSail.SailTurningState.TurningLeft);
				}
			}
			this.FixedTickSailGustMode(rowerThrust, num, num2);
			if (this._currentSailTurningState != MissionSail.SailTurningState.Stationary)
			{
				float num4 = this._sailObject.RotationRate * (1f + this._ownerShip.ShipOrigin.SailRotationSpeedFactor);
				float num5 = this._targetSailRotation - this._localSailRotation;
				float num6 = Math.Abs(num5);
				float num7 = num6 / num4;
				if (this.TargetSailSetting < 1f && num7 > 1f)
				{
					num6 = num4;
					num5 = (float)MathF.Sign(num5) * num6;
					this._targetSailRotation = this._localSailRotation + num5;
					num7 = 1f;
				}
				float num8;
				if (num7 > 1f)
				{
					num8 = num4;
				}
				else
				{
					num8 = num6 / 1f;
				}
				this._currentSailRotationSpeed = MathF.Lerp(this._currentSailRotationSpeed, num8, fixedDt * 2f, 1E-05f);
				if (MBMath.ApproximatelyEqualsTo(this._currentSailRotationSpeed, 0f, 0.005f) && MBMath.ApproximatelyEqualsTo(num6, 0f, 0.005f))
				{
					this._sailRotationStateTimer = ((this._fullSailMode && this._fullSailWeight > 0f) ? 2f : 2f);
					this._currentSailTurningState = MissionSail.SailTurningState.Stationary;
					this._currentSailRotationSpeed = 0f;
				}
			}
		}

		// Token: 0x06000A69 RID: 2665 RVA: 0x00048EAC File Offset: 0x000470AC
		private Vec3 ComputeCenterOfSailForceGlobal()
		{
			return this._ownerShip.GameEntity.GetBodyWorldTransform().TransformToParent(ref this._centerOfSailForceShipLocal);
		}

		// Token: 0x06000A6A RID: 2666 RVA: 0x00048EDA File Offset: 0x000470DA
		public void ForceFold()
		{
			this._sailVisual.InstantCloseSails();
		}

		// Token: 0x06000A6B RID: 2667 RVA: 0x00048EE8 File Offset: 0x000470E8
		private void CalculateSailSoundEventRotationParamAndShouldUpdateSoundPos(float dt, float rotationDiff)
		{
			if (this._ownerShip.ShouldUpdateSoundPos)
			{
				float num = dt * this._sailObject.RotationRate;
				this._sailSoundEventRotationParam = ((num > 0f) ? MathF.Clamp(rotationDiff / num, 0f, 1f) : 0f);
			}
		}

		// Token: 0x06000A6C RID: 2668 RVA: 0x00048F37 File Offset: 0x00047137
		private void InitializeSailSounds()
		{
			this.CalculateSailSoundEventRotationParamAndShouldUpdateSoundPos(0f, 0f);
			this.UpdateSoundPos();
			this._blowSoundEventCooldown = 0f;
		}

		// Token: 0x06000A6D RID: 2669 RVA: 0x00048F5C File Offset: 0x0004715C
		private BoundingBox GetPhysicsBoundingBox()
		{
			BoundingBox boundingBox = default(BoundingBox);
			boundingBox.BeginRelaxation();
			MatrixFrame globalFrame = this._sailVisual.SailSkeletonEntity.GetGlobalFrame();
			if (this._sailObject.Type == null)
			{
				Vec3 vec;
				vec..ctor(-0.5f, 0f, -0.5f, -1f);
				for (int i = 0; i < 9; i++)
				{
					for (int j = 0; j < 9; j++)
					{
						Vec3 globalSailPoint = this.GetGlobalSailPoint(vec + 0.125f * new Vec3((float)j, 0f, (float)i, -1f), in globalFrame);
						boundingBox.RelaxMinMaxWithPoint(ref globalSailPoint);
					}
				}
			}
			else
			{
				Vec3 vec2;
				vec2..ctor(-0.5f, 0f, 0f, -1f);
				for (int k = 0; k < 5; k++)
				{
					int num = 9 - k * 2;
					for (int l = 0; l < num; l++)
					{
						Vec3 globalSailPoint2 = this.GetGlobalSailPoint(vec2 + 0.125f * new Vec3((float)(l + k), 0f, (float)(-(float)k), -1f), in globalFrame);
						boundingBox.RelaxMinMaxWithPoint(ref globalSailPoint2);
					}
				}
			}
			return boundingBox;
		}

		// Token: 0x06000A6E RID: 2670 RVA: 0x0004908E File Offset: 0x0004728E
		public bool IsBurningFinished()
		{
			return this._sailVisual.IsBurningFinished();
		}

		// Token: 0x06000A6F RID: 2671 RVA: 0x0004909B File Offset: 0x0004729B
		public bool IsBurning()
		{
			return this._sailVisual.IsBurning();
		}

		// Token: 0x06000A70 RID: 2672 RVA: 0x000490A8 File Offset: 0x000472A8
		public void StartFire()
		{
			this._sailVisual.StartFire();
		}

		// Token: 0x06000A71 RID: 2673 RVA: 0x000490B8 File Offset: 0x000472B8
		public bool IntersectLineSegmentWithSail(in Vec3 lineSegmentStart, in Vec3 lineSegmentEnd)
		{
			BoundingBox physicsBoundingBox = this.GetPhysicsBoundingBox();
			if (MBMath.IntersectLineSegmentWithBoundingBox(ref lineSegmentStart, ref lineSegmentEnd, ref physicsBoundingBox.min, ref physicsBoundingBox.max))
			{
				MatrixFrame globalFrame = this._sailVisual.SailSkeletonEntity.GetGlobalFrame();
				if (this._sailObject.Type == null)
				{
					Vec3 vec;
					vec..ctor(-0.5f, 0f, -0.5f, -1f);
					for (int i = 0; i < 8; i++)
					{
						for (int j = 0; j < 8; j++)
						{
							Vec3 globalSailPoint = this.GetGlobalSailPoint(vec + 0.125f * new Vec3((float)j, 0f, (float)i, -1f), in globalFrame);
							Vec3 globalSailPoint2 = this.GetGlobalSailPoint(vec + 0.125f * new Vec3((float)(j + 1), 0f, (float)i, -1f), in globalFrame);
							Vec3 globalSailPoint3 = this.GetGlobalSailPoint(vec + 0.125f * new Vec3((float)(j + 1), 0f, (float)(i + 1), -1f), in globalFrame);
							if (MBMath.IntersectLineSegmentWithTriangle(ref lineSegmentStart, ref lineSegmentEnd, ref globalSailPoint, ref globalSailPoint3, ref globalSailPoint2))
							{
								return true;
							}
							Vec3 globalSailPoint4 = this.GetGlobalSailPoint(vec + 0.125f * new Vec3((float)j, 0f, (float)(i + 1), -1f), in globalFrame);
							if (MBMath.IntersectLineSegmentWithTriangle(ref lineSegmentStart, ref lineSegmentEnd, ref globalSailPoint, ref globalSailPoint4, ref globalSailPoint3))
							{
								return true;
							}
						}
					}
				}
				else
				{
					Vec3 vec2;
					vec2..ctor(-0.5f, 0f, 0f, -1f);
					for (int k = 0; k < 4; k++)
					{
						int num = 9 - k * 2 - 1;
						for (int l = 0; l < num; l++)
						{
							if (l == num - 1)
							{
								Vec3 globalSailPoint5 = this.GetGlobalSailPoint(vec2 + 0.125f * new Vec3((float)(l + k), 0f, (float)(-(float)k), -1f), in globalFrame);
								Vec3 globalSailPoint6 = this.GetGlobalSailPoint(vec2 + 0.125f * new Vec3((float)(l + k + 1), 0f, (float)(-(float)k), -1f), in globalFrame);
								Vec3 globalSailPoint7 = this.GetGlobalSailPoint(vec2 + 0.125f * new Vec3((float)(l + k), 0f, (float)(-(float)k - 1), -1f), in globalFrame);
								if (MBMath.IntersectLineSegmentWithTriangle(ref lineSegmentStart, ref lineSegmentEnd, ref globalSailPoint5, ref globalSailPoint7, ref globalSailPoint6))
								{
									return true;
								}
							}
							else
							{
								Vec3 globalSailPoint8 = this.GetGlobalSailPoint(vec2 + 0.125f * new Vec3((float)(l + k), 0f, (float)(-(float)k), -1f), in globalFrame);
								Vec3 globalSailPoint9 = this.GetGlobalSailPoint(vec2 + 0.125f * new Vec3((float)(l + k + 1), 0f, (float)(-(float)k), -1f), in globalFrame);
								Vec3 globalSailPoint10 = this.GetGlobalSailPoint(vec2 + 0.125f * new Vec3((float)(l + k + 1), 0f, (float)(-(float)k - 1), -1f), in globalFrame);
								if (MBMath.IntersectLineSegmentWithTriangle(ref lineSegmentStart, ref lineSegmentEnd, ref globalSailPoint8, ref globalSailPoint10, ref globalSailPoint9))
								{
									return true;
								}
								if (l > 0)
								{
									Vec3 globalSailPoint11 = this.GetGlobalSailPoint(vec2 + 0.125f * new Vec3((float)(l + k), 0f, (float)(-(float)k - 1), -1f), in globalFrame);
									if (MBMath.IntersectLineSegmentWithTriangle(ref lineSegmentStart, ref lineSegmentEnd, ref globalSailPoint8, ref globalSailPoint11, ref globalSailPoint10))
									{
										return true;
									}
								}
							}
						}
					}
				}
			}
			return false;
		}

		// Token: 0x06000A72 RID: 2674 RVA: 0x00049444 File Offset: 0x00047644
		private Vec3 GetGlobalSailPoint(Vec3 point, in MatrixFrame sailGlobalFrame)
		{
			MatrixFrame matrixFrame = sailGlobalFrame;
			Vec3 scale = matrixFrame.GetScale();
			float num = this._width / scale.x;
			float num2 = this._height / scale.z;
			point.x *= num;
			point.z *= num2;
			float num3 = MathF.Min((this._sailObject.Type == null) ? (0.5f * num2 - point.z) : (-point.z), (this._sailObject.Type == null) ? point.Distance(new Vec3(((point.x > 0f) ? 0.5f : (-0.5f)) * num, 0f, -0.5f * num2, -1f)) : point.Distance(new Vec3(0f, 0f, -0.5f * num2, -1f)));
			float num4 = ((this._sailObject.Type == null) ? (0.5f * num2 + ((point.z > 0f) ? (-point.z) : point.z)) : (-point.z));
			num4 = Math.Min(num4, (this._sailObject.Type == null) ? (0.5f * num + ((point.x > 0f) ? (-point.x) : point.x)) : (-point.z));
			float num5 = MathF.Sqrt(num3 * (num4 + 0.4f) / (Math.Min(num2, num) * 0.5f + 0.4f));
			point.z += (1f - this.Setting) * ((this._sailObject.Type == null) ? (0.25f * num2 - point.z) : (-point.z));
			Vec2 asVec = this._force.Force.AsVec2;
			float num6 = asVec.Normalize();
			Vec2 vec = asVec * (MathF.Sqrt(num6) / 100f);
			matrixFrame = sailGlobalFrame;
			Vec3 vec2 = matrixFrame.TransformToParent(ref point);
			if (this._sailObject.Type == null)
			{
				Vec3 vec3 = sailGlobalFrame.rotation.s;
				Vec2 vec4 = vec3.AsVec2.Normalized();
				vec3 = sailGlobalFrame.rotation.f;
				Vec2 vec5 = vec3.AsVec2.Normalized();
				float num7 = Math.Max(0f, Vec2.DotProduct(vec5, vec));
				vec2 += new Vec3(vec4 * (Vec2.DotProduct(vec4, vec) * 0.65f * num5), 0f, -1f);
				vec2 += new Vec3(vec5 * (num7 * 0.9f * num5), 0f, -1f);
				vec2 += new Vec3(0f, 0f, (0.5f - point.z / num2) * 0.35f * num7 * 0.9f * num5, -1f);
				vec2 += (0.5f - point.z / num2) * sailGlobalFrame.rotation.f * 0.6f;
			}
			else
			{
				Vec3 vec3 = sailGlobalFrame.rotation.s;
				Vec2 vec6 = -vec3.AsVec2.Normalized();
				vec3 = sailGlobalFrame.rotation.f;
				Vec2 vec7 = -vec3.AsVec2.Normalized();
				float num8 = Math.Max(0f, Vec2.DotProduct(vec7, vec));
				vec2 += new Vec3(vec6 * (Vec2.DotProduct(vec6, vec) * 0.1f * num5), 0f, -1f);
				vec2 += new Vec3(vec7 * (num8 * 0.7f * num5), 0f, -1f);
				vec2 += new Vec3(0f, 0f, (0.5f - point.z / num2) * 0.1f * num8 * 0.7f * num5, -1f);
				vec2 += sailGlobalFrame.rotation.f * 0.25f;
			}
			return vec2;
		}

		// Token: 0x06000A73 RID: 2675 RVA: 0x0004988C File Offset: 0x00047A8C
		public void OnSailHit(Agent attackerAgent, float rawDamage, float inflictedDamage)
		{
			bool flag = false;
			bool isHuman = attackerAgent.IsHuman;
			bool isMine = attackerAgent.IsMine;
			bool flag2 = attackerAgent.RiderAgent != null;
			Agent riderAgent = attackerAgent.RiderAgent;
			CombatLogData combatLogData;
			combatLogData..ctor(flag, isHuman, isMine, flag2, riderAgent != null && riderAgent.IsMine, attackerAgent.IsMount, false, false, false, false, false, false, this, false, false, false, 0f);
			combatLogData.InflictedFireDamage = (int)rawDamage;
			combatLogData.ModifiedFireDamage = MathF.Round(inflictedDamage - rawDamage);
			Mission.Current.AddCombatLogSafe(attackerAgent, null, combatLogData);
		}

		// Token: 0x06000A74 RID: 2676 RVA: 0x00049902 File Offset: 0x00047B02
		public void StartShipCaptureAnimation(Texture newTexture)
		{
			this._sailVisual.StartFlagCaptureAnimation(newTexture);
		}

		// Token: 0x040005F6 RID: 1526
		public const float OptimalDirectionSearchInterval = 1f;

		// Token: 0x040005F7 RID: 1527
		private const int PhysicsPointCountPerAxis = 9;

		// Token: 0x040005F8 RID: 1528
		private const float BlowSoundEventCooldown = 10f;

		// Token: 0x040005F9 RID: 1529
		private static readonly int _sailContinuousSoundEventId = SoundEvent.GetEventIdFromString("event:/mission/movement/vessel/sail/sail_movement");

		// Token: 0x040005FA RID: 1530
		private static readonly int _sailRotationSoundEventId = SoundEvent.GetEventIdFromString("event:/mission/movement/vessel/sail/sail_rotation");

		// Token: 0x040005FB RID: 1531
		private const float MinSearchSpaceForTargetSailRotationInRadians = 0.10471976f;

		// Token: 0x040005FF RID: 1535
		private ShipSail _sailObject;

		// Token: 0x04000600 RID: 1536
		private MissionShip _ownerShip;

		// Token: 0x04000601 RID: 1537
		private SailVisual _sailVisual;

		// Token: 0x04000602 RID: 1538
		private float _localSailRotation;

		// Token: 0x04000603 RID: 1539
		private float _currentSailRotationSpeed;

		// Token: 0x04000604 RID: 1540
		private Vec3 _centerOfSailForceShipLocal;

		// Token: 0x04000605 RID: 1541
		private float _width;

		// Token: 0x04000606 RID: 1542
		private float _height;

		// Token: 0x04000607 RID: 1543
		private float _sailRotationStateTimer;

		// Token: 0x04000608 RID: 1544
		private float _fullSailWeight;

		// Token: 0x04000609 RID: 1545
		private bool _fullSailMode;

		// Token: 0x0400060A RID: 1546
		private ShipForce _force;

		// Token: 0x0400060B RID: 1547
		private bool _gustMode;

		// Token: 0x0400060C RID: 1548
		private MissionSail.SailTurningState _currentSailTurningState;

		// Token: 0x0400060D RID: 1549
		private float _targetSailRotation;

		// Token: 0x0400060E RID: 1550
		private SoundEvent _sailContinuousSoundEvent;

		// Token: 0x0400060F RID: 1551
		private SoundEvent _sailRotationSoundEvent;

		// Token: 0x04000610 RID: 1552
		private float _blowSoundEventCooldown;

		// Token: 0x04000611 RID: 1553
		private float _sailSoundEventRotationParam;

		// Token: 0x04000612 RID: 1554
		private bool _shouldMakeBlowingSound;

		// Token: 0x0200020D RID: 525
		public enum SailTurningState : sbyte
		{
			// Token: 0x04000EB3 RID: 3763
			Stationary,
			// Token: 0x04000EB4 RID: 3764
			TurningLeft,
			// Token: 0x04000EB5 RID: 3765
			TurningRight
		}
	}
}
