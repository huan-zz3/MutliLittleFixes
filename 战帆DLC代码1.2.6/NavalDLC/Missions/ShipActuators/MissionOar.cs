using System;
using System.Runtime.CompilerServices;
using NavalDLC.Missions.Objects;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.ShipActuators
{
	// Token: 0x02000091 RID: 145
	public class MissionOar
	{
		// Token: 0x170001AF RID: 431
		// (get) Token: 0x06000A1B RID: 2587 RVA: 0x000469FF File Offset: 0x00044BFF
		public MissionShip OwnerShip { get; }

		// Token: 0x170001B0 RID: 432
		// (get) Token: 0x06000A1C RID: 2588 RVA: 0x00046A07 File Offset: 0x00044C07
		// (set) Token: 0x06000A1D RID: 2589 RVA: 0x00046A0F File Offset: 0x00044C0F
		public float VisualPhase { get; private set; }

		// Token: 0x170001B1 RID: 433
		// (get) Token: 0x06000A1E RID: 2590 RVA: 0x00046A18 File Offset: 0x00044C18
		public Vec3 GateOffset
		{
			get
			{
				return this._oarGateOffset;
			}
		}

		// Token: 0x170001B2 RID: 434
		// (get) Token: 0x06000A1F RID: 2591 RVA: 0x00046A20 File Offset: 0x00044C20
		// (set) Token: 0x06000A20 RID: 2592 RVA: 0x00046A28 File Offset: 0x00044C28
		public float Extraction { get; private set; }

		// Token: 0x170001B3 RID: 435
		// (get) Token: 0x06000A21 RID: 2593 RVA: 0x00046A31 File Offset: 0x00044C31
		public bool IsRetracted
		{
			get
			{
				return this.Extraction <= 0f;
			}
		}

		// Token: 0x170001B4 RID: 436
		// (get) Token: 0x06000A22 RID: 2594 RVA: 0x00046A43 File Offset: 0x00044C43
		public bool IsExtracted
		{
			get
			{
				return this.Extraction >= 1f;
			}
		}

		// Token: 0x170001B5 RID: 437
		// (get) Token: 0x06000A23 RID: 2595 RVA: 0x00046A55 File Offset: 0x00044C55
		// (set) Token: 0x06000A24 RID: 2596 RVA: 0x00046A5D File Offset: 0x00044C5D
		public bool IsUsed { get; private set; }

		// Token: 0x170001B6 RID: 438
		// (get) Token: 0x06000A25 RID: 2597 RVA: 0x00046A66 File Offset: 0x00044C66
		// (set) Token: 0x06000A26 RID: 2598 RVA: 0x00046A6E File Offset: 0x00044C6E
		public bool IsRetracting { get; private set; }

		// Token: 0x170001B7 RID: 439
		// (get) Token: 0x06000A27 RID: 2599 RVA: 0x00046A77 File Offset: 0x00044C77
		public Vec3 BladeContact
		{
			get
			{
				if (!this._bladeContact.IsValid)
				{
					this._bladeContact = this.ComputeBladeContactPosition(true);
				}
				return this._bladeContact;
			}
		}

		// Token: 0x170001B8 RID: 440
		// (get) Token: 0x06000A28 RID: 2600 RVA: 0x00046A99 File Offset: 0x00044C99
		// (set) Token: 0x06000A29 RID: 2601 RVA: 0x00046AA1 File Offset: 0x00044CA1
		public OarDeckParameters DeckParameters { get; private set; }

		// Token: 0x170001B9 RID: 441
		// (get) Token: 0x06000A2A RID: 2602 RVA: 0x00046AAA File Offset: 0x00044CAA
		// (set) Token: 0x06000A2B RID: 2603 RVA: 0x00046AB2 File Offset: 0x00044CB2
		public float ForceMultiplierFromUserAgent { get; private set; }

		// Token: 0x170001BA RID: 442
		// (get) Token: 0x06000A2C RID: 2604 RVA: 0x00046ABB File Offset: 0x00044CBB
		public float NeededRevolutionRate
		{
			get
			{
				return this._sidePhaseData.NeededRevolutionRate;
			}
		}

		// Token: 0x06000A2D RID: 2605 RVA: 0x00046AC8 File Offset: 0x00044CC8
		private MissionOar(MissionShip ownerShip, GameEntity entity, OarDeckParameters deckParameters, OarSidePhaseController phaseData)
		{
			this.OwnerShip = ownerShip;
			this.DeckParameters = deckParameters;
			this._ownerSceneCached = this.OwnerShip.Scene;
			MatrixFrame globalFrame = entity.GetGlobalFrame();
			MatrixFrame matrixFrame = this.OwnerShip.GameEntity.GetGlobalFrame().TransformToLocal(ref globalFrame);
			this._oarGateOffset = matrixFrame.origin;
			this._sidePhaseData = phaseData;
			this.VisualPhase = this._sidePhaseData.VisualPhase;
			this.ReRandomizeVisualParameters(-1);
			this._phaseDelayForSlowDown = 0f;
			this.Extraction = 1f;
			this.IsRetracting = false;
			this.IsUsed = true;
			this._slowDownPhaseMultiplier = 1f;
			this._slowDownPhaseDuration = 0f;
			this._timeLeftToCheckForCloseShipsForRetraction = 0f;
			this.ForceMultiplierFromUserAgent = 1f;
			if (!this._ownerSceneCached.IsEditorScene())
			{
				MatrixFrame identity = MatrixFrame.Identity;
				this._oarWaterParticleSmall = ParticleSystem.CreateParticleSystemAttachedToEntity("psys_naval_oar_on_move_small", ownerShip.GameEntity, ref identity);
				this._oarWaterParticleSmall.SetEnable(false);
			}
			for (int i = 0; i < 4; i++)
			{
				this._splashFoamDecals[i] = new MissionOar.OarFoamDecal();
			}
		}

		// Token: 0x06000A2E RID: 2606 RVA: 0x00046C08 File Offset: 0x00044E08
		private void ReRandomizeVisualParameters(int userAgentIndex)
		{
			uint num;
			if (userAgentIndex >= 0)
			{
				num = (uint)userAgentIndex;
			}
			else
			{
				num = (uint)((this._oarGateOffset.x + this._oarGateOffset.y + this._oarGateOffset.z) * 1000f);
			}
			this._oarEffectsRandom = new MBFastRandom(num);
			this._phaseDelayOffset = this._oarEffectsRandom.NextFloatRanged(-10f, 10f) * 0.017453292f;
			this._phaseDelayOffsetTimeScale = this._oarEffectsRandom.NextFloatRanged(0.5f, 1.2f);
			this._visualVerticalBaseAngleOffset = this._oarEffectsRandom.NextFloatRanged(-0.02617994f, 0.02617994f);
			this._visualVerticalAngleMultiplier = this._oarEffectsRandom.NextFloatRanged(1f, 1.1f);
			this._visualLateralAngleMultiplier = this._oarEffectsRandom.NextFloatRanged(0.95f, 1.01f);
			this._visualOarConstantRollAngle = this._oarEffectsRandom.NextFloatRanged(-0.05235988f, 0.05235988f);
			this._visualOarRollAnimationAngleFactor = this._oarEffectsRandom.NextFloatRanged(0.7f, 1f);
			this._visualOarRollAnimationIndex = this._oarEffectsRandom.Next(MissionOar.OarRollAnimManager.RollAnimations.Length);
		}

		// Token: 0x06000A2F RID: 2607 RVA: 0x00046D2F File Offset: 0x00044F2F
		public void SetUsed(bool newIsUsed, int userAgentIndex)
		{
			if (this.IsUsed != newIsUsed)
			{
				this.SetRetractOars(this.IsUsed);
				this.IsUsed = newIsUsed;
				if (this.IsUsed)
				{
					this.ReRandomizeVisualParameters(userAgentIndex);
				}
			}
		}

		// Token: 0x06000A30 RID: 2608 RVA: 0x00046D5C File Offset: 0x00044F5C
		public void SetRetractOars(bool value)
		{
			this.IsRetracting = value;
		}

		// Token: 0x06000A31 RID: 2609 RVA: 0x00046D65 File Offset: 0x00044F65
		public void SetSlowDownPhaseForDuration(float slowDownMultiplier, float slowDownDuration)
		{
			this._slowDownPhaseMultiplier = slowDownMultiplier;
			this._slowDownPhaseDuration = slowDownDuration;
		}

		// Token: 0x06000A32 RID: 2610 RVA: 0x00046D78 File Offset: 0x00044F78
		public void OnParallelTick(float dt)
		{
			float num = this.Extraction;
			if (this.IsRetracting)
			{
				num -= dt * this.DeckParameters.RetractionRate;
				num = MathF.Max(0f, num);
			}
			else
			{
				num += dt * this.DeckParameters.RetractionRate;
				num = MathF.Min(num, 1f);
			}
			this.Extraction = num;
			float num2 = 0f;
			if (!this.IsRetracted)
			{
				float currentTime = Mission.Current.CurrentTime;
				num2 = this._phaseDelayOffset * MathF.Sin(currentTime * this._phaseDelayOffsetTimeScale) * num;
			}
			float num3 = MBMath.WrapAngleSafe(this._sidePhaseData.VisualPhase + num2);
			if (this._slowDownPhaseDuration > 0f || this._phaseDelayForSlowDown != 0f)
			{
				this._slowDownPhaseDuration -= dt;
				if (this._slowDownPhaseDuration > 0f)
				{
					this._phaseDelayForSlowDown -= this._sidePhaseData.PhaseRate * dt * (1f - this._slowDownPhaseMultiplier);
					this._phaseDelayForSlowDown = MBMath.WrapAngleSafe(this._phaseDelayForSlowDown);
				}
				else
				{
					this._slowDownPhaseDuration = 0f;
					float phaseDelayForSlowDown = this._phaseDelayForSlowDown;
					this._phaseDelayForSlowDown += this._sidePhaseData.PhaseRate * dt * (1f - this._slowDownPhaseMultiplier);
					this._phaseDelayForSlowDown = MBMath.WrapAngleSafe(this._phaseDelayForSlowDown);
					if (phaseDelayForSlowDown * this._phaseDelayForSlowDown <= 0f && MathF.Abs(phaseDelayForSlowDown) < 1.5707964f && MathF.Abs(this._phaseDelayForSlowDown) < 1.5707964f)
					{
						this._phaseDelayForSlowDown = 0f;
					}
				}
			}
			num3 += this._phaseDelayForSlowDown;
			this.VisualPhase = MBMath.WrapAngleSafe(num3);
			this.TickFoamDecals(dt);
			MatrixFrame matrixFrame = this.OwnerShip.GlobalFrame;
			Vec3 vec = matrixFrame.TransformToParent(ref this._bladeContact);
			if (this.IsExtracted && MBMath.IsBetweenInclusive(this.VisualPhase, -0.87266463f, 0.87266463f))
			{
				if (this._wakeActive)
				{
					float lastSubmergedHeightFactorForActuators = this._sidePhaseData.GetLastSubmergedHeightFactorForActuators();
					if (this._sidePhaseData.CycleArcSizeMult > 0.5f && lastSubmergedHeightFactorForActuators > 0.01f)
					{
						float num4 = (1f - this._sidePhaseData.LastSlowDownFactor * this._sidePhaseData.LastSlowDownFactor * this._sidePhaseData.LastSlowDownFactor + 0.4f) * 0.25f * dt * lastSubmergedHeightFactorForActuators;
						this._ownerSceneCached.AddWaterWakeWithCapsule(this._lastGlobalBladeContact, 0.90000004f, vec, num4, num4, 0f);
					}
					if (lastSubmergedHeightFactorForActuators > 0.01f && this._sidePhaseData.CycleArcSizeMult > 0.5f && (MBMath.IsBetweenInclusive(this.VisualPhase, -0.87266463f, -0.5235988f) || MBMath.IsBetweenInclusive(this.VisualPhase, 0.17453295f, 0.87266463f)))
					{
						MatrixFrame globalFrame = this.OwnerShip.GameEntity.GetGlobalFrame();
						MatrixFrame identity = MatrixFrame.Identity;
						identity.rotation.s = globalFrame.rotation.s;
						if (this.GateOffset.x < 0f)
						{
							identity.rotation.s = identity.rotation.s * -1f;
						}
						identity.rotation.s.z = 0f;
						identity.rotation.s.Normalize();
						identity.rotation.f = -identity.rotation.s.CrossProductWithUp();
						identity.origin = vec;
						identity.origin.z = this._ownerSceneCached.GetWaterLevelAtPosition(identity.origin.AsVec2, true, false);
						ParticleSystem oarWaterParticleSmall = this._oarWaterParticleSmall;
						matrixFrame = globalFrame.TransformToLocalNonOrthogonal(ref identity);
						oarWaterParticleSmall.SetLocalFrame(ref matrixFrame);
						this._oarWaterParticleSmall.SetEnable(true);
						if (!this._decalSpawned)
						{
							if (this._oarEffectsRandom.NextFloat() > 0.4f)
							{
								this.SpawnNewDecal(in vec);
							}
							this._decalSpawned = true;
						}
					}
					else
					{
						this._oarWaterParticleSmall.SetEnable(false);
					}
				}
				this._wakeActive = true;
			}
			else
			{
				this._oarWaterParticleSmall.SetEnable(false);
				this._wakeActive = false;
				this._decalSpawned = false;
			}
			this._lastGlobalBladeContact = vec;
		}

		// Token: 0x06000A33 RID: 2611 RVA: 0x000471B6 File Offset: 0x000453B6
		private void SpawnNewDecal(in Vec3 spawnPosition)
		{
			this._splashFoamDecals[this._nextDecalIndexToUse].Fill(in spawnPosition, this.OwnerShip);
			this._nextDecalIndexToUse = (this._nextDecalIndexToUse + 1) % 4;
		}

		// Token: 0x06000A34 RID: 2612 RVA: 0x000471E4 File Offset: 0x000453E4
		private void TickFoamDecals(float dt)
		{
			MissionOar.OarFoamDecal[] splashFoamDecals = this._splashFoamDecals;
			for (int i = 0; i < splashFoamDecals.Length; i++)
			{
				splashFoamDecals[i].Tick(dt, this.OwnerShip);
			}
		}

		// Token: 0x06000A35 RID: 2613 RVA: 0x00047218 File Offset: 0x00045418
		public void FixedUpdate(float fixedDt, in MatrixFrame shipGlobalFrame, [TupleElementNames(new string[] { "ship", "shipSide" })] MBList<ValueTuple<MissionShip, OarSidePhaseController.OarSide>> nearbyShips)
		{
			if (!this.IsUsed)
			{
				this.IsRetracting = true;
				this._timeLeftToCheckForCloseShipsForRetraction = 0f;
			}
			else
			{
				this._timeLeftToCheckForCloseShipsForRetraction -= fixedDt;
				if (this._timeLeftToCheckForCloseShipsForRetraction < 0f)
				{
					this._timeLeftToCheckForCloseShipsForRetraction = this._oarEffectsRandom.NextFloatRanged(0.15f, 0.2f);
					this.IsRetracting = false;
					MatrixFrame matrixFrame = shipGlobalFrame;
					Vec3 vec = matrixFrame.TransformToParent(ref this._oarGateOffset);
					foreach (ValueTuple<MissionShip, OarSidePhaseController.OarSide> valueTuple in nearbyShips)
					{
						if (this._sidePhaseData.Side == valueTuple.Item2)
						{
							MissionShip item = valueTuple.Item1;
							Vec3 vec2 = item.GameEntity.GetBodyWorldTransform().TransformToLocal(ref vec);
							Vec3 closestPointToBoundingBox = item.Physics.GetClosestPointToBoundingBox(in vec2);
							float num = this.DeckParameters.OarLength + this.DeckParameters.RetractionOffset;
							if (closestPointToBoundingBox.DistanceSquared(vec2) < num * num)
							{
								this.IsRetracting = true;
								break;
							}
						}
					}
				}
			}
			this._bladeContact = this.ComputeBladeContactPosition(true);
		}

		// Token: 0x06000A36 RID: 2614 RVA: 0x00047358 File Offset: 0x00045558
		public Vec3 ComputeBladeContactPosition(bool ignoreRetraction = true)
		{
			float num = (ignoreRetraction ? 1f : this.Extraction);
			return MissionOar.ComputeBladeContactPositionAux(in this._oarGateOffset, this.DeckParameters, this._sidePhaseData.Phase, num, 0f, 1f, 1f);
		}

		// Token: 0x06000A37 RID: 2615 RVA: 0x000473A4 File Offset: 0x000455A4
		public Vec3 ComputeBladeVisualContactPosition(bool ignoreRetraction = true)
		{
			float num = (ignoreRetraction ? 1f : this.Extraction);
			float num2 = this._sidePhaseData.VisualVerticalBaseAngleOffsetFromShipRoll + this._visualVerticalBaseAngleOffset;
			float num3 = this._sidePhaseData.CycleArcSizeMult * this._visualVerticalAngleMultiplier;
			float visualLateralAngleMultiplier = this._visualLateralAngleMultiplier;
			return MissionOar.ComputeBladeContactPositionAux(in this._oarGateOffset, this.DeckParameters, this.VisualPhase, num, num2, num3, visualLateralAngleMultiplier);
		}

		// Token: 0x06000A38 RID: 2616 RVA: 0x0004740C File Offset: 0x0004560C
		public static Vec3 ComputeBladeContactPositionAux(in Vec3 gateOffset, OarDeckParameters deckParameters, float phase = 0f, float retraction = 1f, float verticalBaseAngleOffset = 0f, float verticalAngleMultiplier = 1f, float lateralAngleMultiplier = 1f)
		{
			int num = MathF.Sign(gateOffset.x);
			Vec3 vec;
			vec..ctor((float)num * deckParameters.OarLength * retraction, 0f, 0f, -1f);
			float num2 = deckParameters.VerticalRotationAngle * verticalAngleMultiplier;
			float num3 = deckParameters.LateralRotationAngle * lateralAngleMultiplier;
			float num4 = (float)num * MissionOar.GetVerticalAngle(phase, deckParameters.VerticalBaseAngle + verticalBaseAngleOffset, num2);
			float num5 = (float)num * MissionOar.GetLateralAngle(phase, deckParameters.LateralBaseAngle, num3);
			float num6;
			float num7;
			MathF.SinCos(num4, ref num6, ref num7);
			vec.z = -vec.x * num6;
			vec.x *= num7;
			MathF.SinCos(num5, ref num6, ref num7);
			vec.y = vec.x * num6;
			vec.x *= num7;
			return gateOffset + vec;
		}

		// Token: 0x06000A39 RID: 2617 RVA: 0x000474DC File Offset: 0x000456DC
		public Vec3 ComputeBladeContactVelocity(bool ignoreRetraction = false)
		{
			float num = (ignoreRetraction ? 1f : this.Extraction);
			return MissionOar.ComputeBladeContactVelocityAux(this.DeckParameters, this._sidePhaseData.Phase, this._sidePhaseData.PhaseRate, num);
		}

		// Token: 0x06000A3A RID: 2618 RVA: 0x0004751C File Offset: 0x0004571C
		public static Vec3 ComputeBladeContactVelocityAux(OarDeckParameters deckParameters, float phase, float phaseRate, float retraction = 1f)
		{
			float verticalAngle = MissionOar.GetVerticalAngle(phase, deckParameters.VerticalBaseAngle, deckParameters.VerticalRotationAngle);
			float lateralAngle = MissionOar.GetLateralAngle(phase, deckParameters.LateralBaseAngle, deckParameters.LateralRotationAngle);
			float num = MathF.Sin(-phase) * deckParameters.VerticalRotationAngle * phaseRate;
			float num2 = -MathF.Cos(-phase) * deckParameters.LateralRotationAngle * phaseRate;
			float num3 = MathF.Sin(verticalAngle);
			float num4 = MathF.Cos(verticalAngle);
			float num5 = MathF.Sin(lateralAngle);
			float num6 = MathF.Cos(lateralAngle);
			float num7 = retraction * deckParameters.OarLength;
			float num8 = -num7 * num3 * num * num6 - num7 * num4 * num5 * num2;
			float num9 = -num7 * num3 * num * num5 + num7 * num4 * num6 * num2;
			float num10 = -num7 * num4 * num;
			return new Vec3(num8, num9, num10, -1f);
		}

		// Token: 0x06000A3B RID: 2619 RVA: 0x000475DD File Offset: 0x000457DD
		public static float GetVerticalAngle(float phase, float verticalBaseAngle, float verticalRotationAngle)
		{
			return verticalBaseAngle + MathF.Cos(-phase) * verticalRotationAngle;
		}

		// Token: 0x06000A3C RID: 2620 RVA: 0x000475EA File Offset: 0x000457EA
		public static float GetLateralAngle(float phase, float lateralBaseAngle, float lateralRotationAngle)
		{
			return lateralBaseAngle + MathF.Sin(-phase) * lateralRotationAngle;
		}

		// Token: 0x06000A3D RID: 2621 RVA: 0x000475F7 File Offset: 0x000457F7
		public static MissionOar CreateShipOar(MissionShip ownerShip, GameEntity entity, OarDeckParameters deckParameters, OarSidePhaseController sidePhase)
		{
			return new MissionOar(ownerShip, entity, deckParameters, sidePhase);
		}

		// Token: 0x06000A3E RID: 2622 RVA: 0x00047604 File Offset: 0x00045804
		public MatrixFrame ComputeOarEntityFrame(float dt, in MatrixFrame oarMachineLocalFrame, in MatrixFrame oarEntityLocalFrame, in MatrixFrame _oarExtractedEntitialFrame, in MatrixFrame _oarRetractedEntitialFrame, float _lastIdleTime, bool forUnmanned)
		{
			Vec3 vec = this.ComputeBladeVisualContactPosition(true);
			MatrixFrame matrixFrame = oarMachineLocalFrame;
			Vec3 vec2 = matrixFrame.TransformToLocal(ref vec);
			Mat3 mat;
			if (this.IsExtracted)
			{
				float currentTime = Mission.Current.CurrentTime;
				MatrixFrame matrixFrame2 = _oarExtractedEntitialFrame;
				matrixFrame2.rotation.f = vec2 - matrixFrame2.origin;
				matrixFrame2.rotation.Orthonormalize();
				float num = this._phaseDelayOffset * MathF.Sin(currentTime * this._phaseDelayOffsetTimeScale);
				float num2 = this.ComputeOarRollAccordingToPhase(MBMath.WrapAngleSafe(num + this.VisualPhase)) * this._visualOarRollAnimationAngleFactor + this._visualOarConstantRollAngle;
				if (this._sidePhaseData.Side == OarSidePhaseController.OarSide.Left)
				{
					num2 *= -1f;
				}
				matrixFrame2.rotation.RotateAboutForward(num2);
				float num3 = currentTime - _lastIdleTime;
				if (num3 < 1.5f)
				{
					Quaternion quaternion = matrixFrame2.rotation.ToQuaternion();
					mat = _oarExtractedEntitialFrame.rotation;
					Quaternion quaternion2 = mat.ToQuaternion();
					matrixFrame2.rotation = Quaternion.Slerp(quaternion2, quaternion, num3 / 1.5f).ToMat3();
					matrixFrame2.rotation.Orthonormalize();
				}
				return matrixFrame2;
			}
			Vec3 vec3;
			if (this.IsRetracted)
			{
				MatrixFrame matrixFrame3 = _oarRetractedEntitialFrame;
				if (forUnmanned)
				{
					Vec2 asVec = matrixFrame3.origin.AsVec2;
					vec3 = _oarExtractedEntitialFrame.origin;
					Vec2 vec4 = asVec - vec3.AsVec2;
					matrixFrame3.origin.z = _oarExtractedEntitialFrame.origin.z + (float)MathF.Sign(Vec2.DotProduct(matrixFrame3.rotation.f.AsVec2, vec4)) * (vec4.Length / matrixFrame3.rotation.f.AsVec2.Length) * matrixFrame3.rotation.f.z;
				}
				else
				{
					float z = _oarExtractedEntitialFrame.origin.z;
					Vec2 vec5 = matrixFrame3.origin.AsVec2;
					vec3 = _oarExtractedEntitialFrame.origin;
					matrixFrame3.origin.z = z + vec5.Distance(vec3.AsVec2) / matrixFrame3.rotation.f.AsVec2.Length * matrixFrame3.rotation.f.z;
				}
				return matrixFrame3;
			}
			mat = oarEntityLocalFrame.rotation;
			Quaternion quaternion3 = mat.ToQuaternion();
			Quaternion quaternion4;
			if (!this.IsRetracting)
			{
				mat = _oarExtractedEntitialFrame.rotation;
				quaternion4 = mat.ToQuaternion();
			}
			else
			{
				mat = _oarRetractedEntitialFrame.rotation;
				quaternion4 = mat.ToQuaternion();
			}
			Quaternion quaternion5 = quaternion4;
			mat = Quaternion.Slerp(quaternion5, quaternion3, MathF.Pow(2f, dt * -3f)).ToMat3();
			vec3 = Vec3.Lerp(_oarRetractedEntitialFrame.origin, _oarExtractedEntitialFrame.origin, this.Extraction);
			MatrixFrame matrixFrame4 = new MatrixFrame(ref mat, ref vec3);
			matrixFrame4.rotation.Orthonormalize();
			if (forUnmanned)
			{
				Vec2 asVec2 = matrixFrame4.origin.AsVec2;
				vec3 = _oarExtractedEntitialFrame.origin;
				Vec2 vec6 = asVec2 - vec3.AsVec2;
				matrixFrame4.origin.z = _oarExtractedEntitialFrame.origin.z + (float)MathF.Sign(Vec2.DotProduct(matrixFrame4.rotation.f.AsVec2, vec6)) * (vec6.Length / matrixFrame4.rotation.f.AsVec2.Length) * matrixFrame4.rotation.f.z;
			}
			else
			{
				float z2 = _oarExtractedEntitialFrame.origin.z;
				Vec2 vec5 = matrixFrame4.origin.AsVec2;
				vec3 = _oarExtractedEntitialFrame.origin;
				matrixFrame4.origin.z = z2 + vec5.Distance(vec3.AsVec2) / matrixFrame4.rotation.f.AsVec2.Length * matrixFrame4.rotation.f.z;
			}
			return matrixFrame4;
		}

		// Token: 0x06000A3F RID: 2623 RVA: 0x000479E0 File Offset: 0x00045BE0
		private float ComputeOarRollAccordingToPhase(float phase)
		{
			MissionOar.OarRollAnimKeyFrame[] array = MissionOar.OarRollAnimManager.RollAnimations[this._visualOarRollAnimationIndex];
			float num = (phase + 3.1415927f) / 6.2831855f;
			if (num >= 1f)
			{
				num -= 1f;
			}
			float num2 = 0f;
			for (int i = 0; i < array.Length - 1; i++)
			{
				MissionOar.OarRollAnimKeyFrame oarRollAnimKeyFrame = array[i];
				MissionOar.OarRollAnimKeyFrame oarRollAnimKeyFrame2 = array[i + 1];
				if (oarRollAnimKeyFrame.KeyProgress <= num && num < oarRollAnimKeyFrame2.KeyProgress)
				{
					float num3 = oarRollAnimKeyFrame2.KeyProgress - oarRollAnimKeyFrame.KeyProgress;
					float num4 = (num - oarRollAnimKeyFrame.KeyProgress) / num3;
					num2 = MathF.Lerp(oarRollAnimKeyFrame.RollAngleInRad, oarRollAnimKeyFrame2.RollAngleInRad, num4, 1E-05f);
					break;
				}
			}
			return num2;
		}

		// Token: 0x06000A40 RID: 2624 RVA: 0x00047A93 File Offset: 0x00045C93
		public void SetOarForceMultiplierFromUserAgent(float forceMultiplierFromUserAgent)
		{
			this.ForceMultiplierFromUserAgent = forceMultiplierFromUserAgent;
		}

		// Token: 0x06000A41 RID: 2625 RVA: 0x00047A9C File Offset: 0x00045C9C
		public void OnPilotAssignedDuringSpawn()
		{
			this.IsRetracting = false;
			this.Extraction = 1f;
		}

		// Token: 0x06000A42 RID: 2626 RVA: 0x00047AB0 File Offset: 0x00045CB0
		public bool IsInRowingMotion()
		{
			return this._sidePhaseData.IsInRowingMotion();
		}

		// Token: 0x040005D7 RID: 1495
		private const int NumberOfFoamDecals = 4;

		// Token: 0x040005DA RID: 1498
		private float _phaseDelayForSlowDown;

		// Token: 0x040005DB RID: 1499
		private float _phaseDelayOffset;

		// Token: 0x040005DC RID: 1500
		private float _phaseDelayOffsetTimeScale;

		// Token: 0x040005DD RID: 1501
		private float _visualVerticalBaseAngleOffset;

		// Token: 0x040005DE RID: 1502
		private float _visualVerticalAngleMultiplier;

		// Token: 0x040005DF RID: 1503
		private float _visualLateralAngleMultiplier;

		// Token: 0x040005E0 RID: 1504
		private float _visualOarConstantRollAngle;

		// Token: 0x040005E1 RID: 1505
		private float _visualOarRollAnimationAngleFactor;

		// Token: 0x040005E2 RID: 1506
		private int _visualOarRollAnimationIndex;

		// Token: 0x040005E3 RID: 1507
		private float _slowDownPhaseMultiplier;

		// Token: 0x040005E4 RID: 1508
		private float _slowDownPhaseDuration;

		// Token: 0x040005E8 RID: 1512
		private readonly MissionOar.OarFoamDecal[] _splashFoamDecals = new MissionOar.OarFoamDecal[4];

		// Token: 0x040005E9 RID: 1513
		private int _nextDecalIndexToUse;

		// Token: 0x040005EA RID: 1514
		private Vec3 _bladeContact = Vec3.Invalid;

		// Token: 0x040005EB RID: 1515
		private readonly Vec3 _oarGateOffset;

		// Token: 0x040005EC RID: 1516
		private OarSidePhaseController _sidePhaseData;

		// Token: 0x040005EE RID: 1518
		private float _timeLeftToCheckForCloseShipsForRetraction;

		// Token: 0x040005F0 RID: 1520
		private Vec3 _lastGlobalBladeContact;

		// Token: 0x040005F1 RID: 1521
		private ParticleSystem _oarWaterParticleSmall;

		// Token: 0x040005F2 RID: 1522
		private bool _wakeActive;

		// Token: 0x040005F3 RID: 1523
		private bool _decalSpawned;

		// Token: 0x040005F4 RID: 1524
		private MBFastRandom _oarEffectsRandom;

		// Token: 0x040005F5 RID: 1525
		private Scene _ownerSceneCached;

		// Token: 0x0200020A RID: 522
		private class OarFoamDecal
		{
			// Token: 0x06001AF8 RID: 6904 RVA: 0x000B12BC File Offset: 0x000AF4BC
			internal OarFoamDecal()
			{
				this._splashFoamDecal = null;
				this._currentFrame = MatrixFrame.Identity;
				this._cumulativeDtTillStart = 0f;
				this._randomScale = 1f;
				this._currentSpeed = Vec3.Zero;
				this._lifeTimeRandomness = 0f;
				this._sideVectorStart = Vec3.Zero;
				this._sideVectorEnd = Vec3.Zero;
			}

			// Token: 0x06001AF9 RID: 6905 RVA: 0x000B1324 File Offset: 0x000AF524
			internal void Tick(float dt, MissionShip ownerShip)
			{
				float num = 5.8f + this._lifeTimeRandomness;
				if (this._splashFoamDecal != null && this._cumulativeDtTillStart < num)
				{
					Vec3 vec = new Vec3(0.65f, 1f, 1f, -1f);
					Vec3 vec2 = vec * 4.5f;
					this._cumulativeDtTillStart += dt;
					float num3;
					if (this._cumulativeDtTillStart > 1.05f)
					{
						float num2 = this._cumulativeDtTillStart - 1.05f;
						num3 = MathF.Clamp(1f - num2 / (num - 1.05f), 0f, 1f);
					}
					else
					{
						num3 = MathF.Clamp(this._cumulativeDtTillStart / 1.05f, 0f, 1f);
					}
					float num4 = MathF.Pow(num3, 4f);
					this._splashFoamDecal.SetAlpha(num4 * 0.17499998f + 0.475f);
					this._currentFrame.origin.z = ownerShip.Scene.GetWaterLevelAtPosition(this._currentFrame.origin.AsVec2, true, false) - 0.15f;
					this._currentFrame.origin = this._currentFrame.origin + this._currentSpeed * dt;
					Vec3 currentSpeed = this._currentSpeed;
					float num5 = currentSpeed.Normalize();
					num5 = MathF.Max(num5 - dt * 0.5f, 0f);
					this._currentSpeed = num5 * currentSpeed;
					float num6 = MathF.Clamp(this._cumulativeDtTillStart / num, 0f, 1f);
					Vec3 vec3 = Vec3.Lerp(vec, vec2, num6) * this._randomScale;
					float num7 = MathF.Clamp(this._cumulativeDtTillStart / num, 0f, 1f);
					Vec3 vec4 = Vec3.Slerp(this._sideVectorStart, this._sideVectorEnd, num7);
					vec4.Normalize();
					this._currentFrame.rotation.s = vec4;
					this._currentFrame.rotation.u = Vec3.Up;
					this._currentFrame.rotation.f = -this._currentFrame.rotation.s.CrossProductWithUp();
					this._currentFrame.rotation.ApplyScaleLocal(ref vec3);
					this._splashFoamDecal.Frame = this._currentFrame;
				}
			}

			// Token: 0x06001AFA RID: 6906 RVA: 0x000B1574 File Offset: 0x000AF774
			internal void Fill(in Vec3 spawnPosition, MissionShip ownerShip)
			{
				if (this._splashFoamDecal == null)
				{
					Decal decal = Decal.CreateDecal(null);
					decal.SetMaterial(Material.GetFromResource("decal_water_foam"));
					ownerShip.Scene.AddDecalInstance(decal, "editor_set", true);
					this._splashFoamDecal = decal;
				}
				MatrixFrame identity = MatrixFrame.Identity;
				identity.origin = spawnPosition;
				identity.rotation.u = Vec3.Up;
				Vec3 linearVelocityAtGlobalPointForEntityWithDynamicBody = GameEntityPhysicsExtensions.GetLinearVelocityAtGlobalPointForEntityWithDynamicBody(ownerShip.GameEntity, spawnPosition);
				Vec3 s = ownerShip.GameEntity.GetGlobalFrame().rotation.s;
				Vec2 asVec = s.AsVec2;
				asVec.Normalize();
				identity.rotation.s = new Vec3(asVec, 0f, -1f);
				identity.rotation.f = -identity.rotation.s.CrossProductWithUp();
				identity.rotation.f.Normalize();
				identity.origin += (-0.5f + MBRandom.RandomFloat) * identity.rotation.f;
				identity.origin += (-0.5f + MBRandom.RandomFloat) * identity.rotation.s;
				this._cumulativeDtTillStart = 0f;
				MathF.Clamp((linearVelocityAtGlobalPointForEntityWithDynamicBody.Length - 4f) / 8f, 0f, 1f);
				float num = 1f;
				this._randomScale = (0.7f + MBRandom.RandomFloat * 0.6f) * num;
				this._splashFoamDecal.Frame = identity;
				this._splashFoamDecal.SetAlpha(0f);
				this._currentFrame = identity;
				int num2 = MBRandom.RandomInt(3);
				float num3 = (float)(num2 % 2) * 0.5f;
				float num4 = (float)(num2 / 2) * 0.5f;
				this._splashFoamDecal.SetVectorArgument(num3, num4, -0.5f, -0.5f);
				float num5 = 0.1f * (-0.5f + MBRandom.RandomFloat) * 0.25f;
				float num6 = 0.2f * (0.9f + MBRandom.RandomFloat * 0.2f);
				this._currentSpeed = linearVelocityAtGlobalPointForEntityWithDynamicBody * num6 + identity.rotation.s * linearVelocityAtGlobalPointForEntityWithDynamicBody.Length * num5;
				this._lifeTimeRandomness = (-0.5f + MBRandom.RandomFloat) * 2f;
				float num7 = 3.1415927f * (2f * MBRandom.RandomFloat - 1f);
				float num8 = -0.34906584f * (0.8f + MBRandom.RandomFloat * 0.4f);
				this._sideVectorStart = new Vec3(asVec, 0f, -1f);
				this._sideVectorStart.RotateAboutZ(num7);
				this._sideVectorEnd = this._sideVectorStart;
				this._sideVectorEnd.RotateAboutZ(num8);
				Vec2 vec;
				vec..ctor(2.5f, 2.5f);
				this._splashFoamDecal.OverrideRoadBoundaryP0(vec);
				Vec2 vec2;
				vec2..ctor(MBRandom.RandomFloat, MBRandom.RandomFloat);
				this._splashFoamDecal.OverrideRoadBoundaryP1(vec2);
			}

			// Token: 0x04000EA0 RID: 3744
			private Decal _splashFoamDecal;

			// Token: 0x04000EA1 RID: 3745
			private MatrixFrame _currentFrame;

			// Token: 0x04000EA2 RID: 3746
			private Vec3 _sideVectorStart;

			// Token: 0x04000EA3 RID: 3747
			private Vec3 _sideVectorEnd;

			// Token: 0x04000EA4 RID: 3748
			private float _cumulativeDtTillStart;

			// Token: 0x04000EA5 RID: 3749
			private float _randomScale;

			// Token: 0x04000EA6 RID: 3750
			private Vec3 _currentSpeed;

			// Token: 0x04000EA7 RID: 3751
			private float _lifeTimeRandomness;
		}

		// Token: 0x0200020B RID: 523
		private struct OarRollAnimKeyFrame
		{
			// Token: 0x06001AFB RID: 6907 RVA: 0x000B18A3 File Offset: 0x000AFAA3
			public OarRollAnimKeyFrame(float keyProgress, float rollAngleInRad)
			{
				this.KeyProgress = keyProgress;
				this.RollAngleInRad = rollAngleInRad;
			}

			// Token: 0x04000EA8 RID: 3752
			public float KeyProgress;

			// Token: 0x04000EA9 RID: 3753
			public float RollAngleInRad;
		}

		// Token: 0x0200020C RID: 524
		private static class OarRollAnimManager
		{
			// Token: 0x04000EAA RID: 3754
			private static readonly MissionOar.OarRollAnimKeyFrame[] rollAnim = new MissionOar.OarRollAnimKeyFrame[]
			{
				new MissionOar.OarRollAnimKeyFrame(0f, -1.2217305f),
				new MissionOar.OarRollAnimKeyFrame(0.15f, 0.31415927f),
				new MissionOar.OarRollAnimKeyFrame(0.25f, 0.34906584f),
				new MissionOar.OarRollAnimKeyFrame(0.5f, 0.34906584f),
				new MissionOar.OarRollAnimKeyFrame(0.7f, -0.6981317f),
				new MissionOar.OarRollAnimKeyFrame(0.73f, -1.2217305f),
				new MissionOar.OarRollAnimKeyFrame(1f, -1.2217305f)
			};

			// Token: 0x04000EAB RID: 3755
			private static readonly MissionOar.OarRollAnimKeyFrame[] rollAnim2 = new MissionOar.OarRollAnimKeyFrame[]
			{
				new MissionOar.OarRollAnimKeyFrame(0f, -1.134464f),
				new MissionOar.OarRollAnimKeyFrame(0.25f, 0.34906584f),
				new MissionOar.OarRollAnimKeyFrame(0.5f, 0.34906584f),
				new MissionOar.OarRollAnimKeyFrame(0.7f, 0.34906584f),
				new MissionOar.OarRollAnimKeyFrame(1f, -1.134464f)
			};

			// Token: 0x04000EAC RID: 3756
			private static readonly MissionOar.OarRollAnimKeyFrame[] rollAnim3 = new MissionOar.OarRollAnimKeyFrame[]
			{
				new MissionOar.OarRollAnimKeyFrame(0f, -1.2217305f),
				new MissionOar.OarRollAnimKeyFrame(0.25f, 0f),
				new MissionOar.OarRollAnimKeyFrame(0.5f, 0f),
				new MissionOar.OarRollAnimKeyFrame(0.75f, -0.6981317f),
				new MissionOar.OarRollAnimKeyFrame(0.88f, -1.0471976f),
				new MissionOar.OarRollAnimKeyFrame(1f, -1.2217305f)
			};

			// Token: 0x04000EAD RID: 3757
			private static readonly MissionOar.OarRollAnimKeyFrame[] rollAnim4 = new MissionOar.OarRollAnimKeyFrame[]
			{
				new MissionOar.OarRollAnimKeyFrame(0f, -1.134464f),
				new MissionOar.OarRollAnimKeyFrame(0.27f, 0.5235988f),
				new MissionOar.OarRollAnimKeyFrame(0.7f, 0.5235988f),
				new MissionOar.OarRollAnimKeyFrame(1f, -1.134464f)
			};

			// Token: 0x04000EAE RID: 3758
			private static readonly MissionOar.OarRollAnimKeyFrame[] rollAnim5 = new MissionOar.OarRollAnimKeyFrame[]
			{
				new MissionOar.OarRollAnimKeyFrame(0f, -0.34906584f),
				new MissionOar.OarRollAnimKeyFrame(0.25f, -0.6981317f),
				new MissionOar.OarRollAnimKeyFrame(0.27f, 0.17453292f),
				new MissionOar.OarRollAnimKeyFrame(0.7f, 0.34906584f),
				new MissionOar.OarRollAnimKeyFrame(0.77f, -0.43633232f),
				new MissionOar.OarRollAnimKeyFrame(1f, -0.34906584f)
			};

			// Token: 0x04000EAF RID: 3759
			private static readonly MissionOar.OarRollAnimKeyFrame[] rollAnim6 = new MissionOar.OarRollAnimKeyFrame[]
			{
				new MissionOar.OarRollAnimKeyFrame(0f, -1.134464f),
				new MissionOar.OarRollAnimKeyFrame(0.15f, 0.34906584f),
				new MissionOar.OarRollAnimKeyFrame(0.5f, 0.34906584f),
				new MissionOar.OarRollAnimKeyFrame(0.55f, 0.34906584f),
				new MissionOar.OarRollAnimKeyFrame(1f, -1.134464f)
			};

			// Token: 0x04000EB0 RID: 3760
			private static readonly MissionOar.OarRollAnimKeyFrame[] rollAnim7 = new MissionOar.OarRollAnimKeyFrame[]
			{
				new MissionOar.OarRollAnimKeyFrame(0f, -1.4835298f),
				new MissionOar.OarRollAnimKeyFrame(0.5f, 0.61086524f),
				new MissionOar.OarRollAnimKeyFrame(1f, -1.4835298f)
			};

			// Token: 0x04000EB1 RID: 3761
			public static readonly MissionOar.OarRollAnimKeyFrame[][] RollAnimations = new MissionOar.OarRollAnimKeyFrame[][]
			{
				MissionOar.OarRollAnimManager.rollAnim,
				MissionOar.OarRollAnimManager.rollAnim2,
				MissionOar.OarRollAnimManager.rollAnim3,
				MissionOar.OarRollAnimManager.rollAnim4,
				MissionOar.OarRollAnimManager.rollAnim5,
				MissionOar.OarRollAnimManager.rollAnim6,
				MissionOar.OarRollAnimManager.rollAnim7
			};
		}
	}
}
