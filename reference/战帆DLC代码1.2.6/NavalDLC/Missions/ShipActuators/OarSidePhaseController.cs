using System;
using NavalDLC.Missions.NavalPhysics;
using NavalDLC.Missions.Objects;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.ShipActuators
{
	// Token: 0x02000094 RID: 148
	public class OarSidePhaseController
	{
		// Token: 0x170001CD RID: 461
		// (get) Token: 0x06000A87 RID: 2695 RVA: 0x00049A28 File Offset: 0x00047C28
		// (set) Token: 0x06000A88 RID: 2696 RVA: 0x00049A30 File Offset: 0x00047C30
		public float Phase { get; private set; }

		// Token: 0x170001CE RID: 462
		// (get) Token: 0x06000A89 RID: 2697 RVA: 0x00049A39 File Offset: 0x00047C39
		// (set) Token: 0x06000A8A RID: 2698 RVA: 0x00049A41 File Offset: 0x00047C41
		public float CycleArcSizeMult { get; private set; }

		// Token: 0x170001CF RID: 463
		// (get) Token: 0x06000A8B RID: 2699 RVA: 0x00049A4A File Offset: 0x00047C4A
		// (set) Token: 0x06000A8C RID: 2700 RVA: 0x00049A52 File Offset: 0x00047C52
		public float LastSlowDownFactor { get; private set; }

		// Token: 0x170001D0 RID: 464
		// (get) Token: 0x06000A8D RID: 2701 RVA: 0x00049A5B File Offset: 0x00047C5B
		// (set) Token: 0x06000A8E RID: 2702 RVA: 0x00049A63 File Offset: 0x00047C63
		public float VisualPhase { get; private set; }

		// Token: 0x170001D1 RID: 465
		// (get) Token: 0x06000A8F RID: 2703 RVA: 0x00049A6C File Offset: 0x00047C6C
		// (set) Token: 0x06000A90 RID: 2704 RVA: 0x00049A74 File Offset: 0x00047C74
		public float PhaseRate { get; private set; }

		// Token: 0x170001D2 RID: 466
		// (get) Token: 0x06000A91 RID: 2705 RVA: 0x00049A7D File Offset: 0x00047C7D
		// (set) Token: 0x06000A92 RID: 2706 RVA: 0x00049A85 File Offset: 0x00047C85
		public float NeededRevolutionRate { get; private set; }

		// Token: 0x170001D3 RID: 467
		// (get) Token: 0x06000A93 RID: 2707 RVA: 0x00049A8E File Offset: 0x00047C8E
		// (set) Token: 0x06000A94 RID: 2708 RVA: 0x00049A96 File Offset: 0x00047C96
		public float VisualVerticalBaseAngleOffsetFromShipRoll { get; private set; }

		// Token: 0x170001D4 RID: 468
		// (get) Token: 0x06000A95 RID: 2709 RVA: 0x00049A9F File Offset: 0x00047C9F
		public OarSidePhaseController.OarSide Side { get; }

		// Token: 0x06000A96 RID: 2710 RVA: 0x00049AA8 File Offset: 0x00047CA8
		public OarSidePhaseController(MissionShip ownerShip, OarSidePhaseController.OarSide side)
		{
			this.Phase = 3.1415927f;
			this._lastPhase = 3.1415927f;
			this.VisualPhase = 3.1415927f;
			this.PhaseRate = 0f;
			this.NeededRevolutionRate = 0f;
			this._ownerShip = ownerShip;
			this.Side = side;
		}

		// Token: 0x06000A97 RID: 2711 RVA: 0x00049B00 File Offset: 0x00047D00
		public void SetAverageOarDeckParameters(OarDeckParameters averageDeckParameters)
		{
			this._averageDeckParameters = averageDeckParameters;
		}

		// Token: 0x06000A98 RID: 2712 RVA: 0x00049B0C File Offset: 0x00047D0C
		public ValueTuple<float, float> ComputeForceAndSlowDownFactor(float rowerNeededPhaseRate, float shipForwardSpeed, float syncPhase, float targetPhaseRate, float oarsmenForceMultiplier, float oarFrictionMultiplier, float maxTipSpeed)
		{
			float num = 0f;
			float num2 = 1f;
			if (rowerNeededPhaseRate != 0f)
			{
				Vec3 vec = MissionOar.ComputeBladeContactVelocityAux(this._averageDeckParameters, syncPhase, targetPhaseRate, 1f);
				if (vec.y <= 0f)
				{
					if (vec.y < -maxTipSpeed)
					{
						num2 = MathF.Abs(maxTipSpeed / vec.y);
						vec.y = -maxTipSpeed;
					}
					float num3 = vec.y * (float)MathF.Sign(rowerNeededPhaseRate);
					float num4 = num3 + shipForwardSpeed;
					if (num4 * rowerNeededPhaseRate <= 0f)
					{
						float num5 = MathF.Abs(MathF.Cos(syncPhase));
						float num6 = 1000f * oarsmenForceMultiplier;
						float num7 = 1.2f * oarFrictionMultiplier * 0.5f * NavalPhysics.GetWaterDensity() * (0.45f * num5);
						num = num7 * num4 * num4 * (float)MathF.Sign(rowerNeededPhaseRate);
						if (MathF.Abs(num) > num6)
						{
							float num8 = MathF.Sqrt(num6 / num7);
							float num9 = (float)MathF.Sign(num4) * num8 - shipForwardSpeed;
							if (num9 * num3 < 0f || MathF.Abs(num9) < 0.8f)
							{
								num9 = (float)MathF.Sign(num9) * 0.8f;
							}
							num2 *= MathF.Abs(num9 / num3);
							if (num2 > 1f)
							{
								num2 = 1f;
							}
							num = (float)MathF.Sign(num) * num6;
						}
					}
				}
			}
			this.LastSlowDownFactor = num2;
			return new ValueTuple<float, float>(num, num2);
		}

		// Token: 0x06000A99 RID: 2713 RVA: 0x00049C62 File Offset: 0x00047E62
		public void SetPhaseData(float phase, float phaseRate, float cycleArcSizeMult, float neededRevolutionRate)
		{
			this.PhaseRate = phaseRate;
			this._lastPhase = this.Phase;
			this.Phase = phase;
			this.CycleArcSizeMult = cycleArcSizeMult;
			this.NeededRevolutionRate = neededRevolutionRate;
		}

		// Token: 0x06000A9A RID: 2714 RVA: 0x00049C90 File Offset: 0x00047E90
		public void OnParallelTick(float dt)
		{
			float num;
			float num2;
			Mission.Current.Scene.GetInterpolationFactorForBodyWorldTransformSmoothing(ref num, ref num2);
			float num3 = MathF.Abs(this._lastPhase - this.Phase);
			float num4 = MathF.Abs(this._lastPhase - 6.2831855f - this.Phase);
			float num5 = MathF.Abs(this._lastPhase + 6.2831855f - this.Phase);
			if (num3 < num4)
			{
				if (num5 < num3)
				{
					this.VisualPhase = MathF.Lerp(this._lastPhase + 6.2831855f, this.Phase, num, 1E-05f);
				}
				else
				{
					this.VisualPhase = MathF.Lerp(this._lastPhase, this.Phase, num, 1E-05f);
				}
			}
			else if (num5 < num3)
			{
				this.VisualPhase = MathF.Lerp(this._lastPhase + 6.2831855f, this.Phase, num, 1E-05f);
			}
			else
			{
				this.VisualPhase = MathF.Lerp(this._lastPhase - 6.2831855f, this.Phase, num, 1E-05f);
			}
			this.VisualPhase = MBMath.WrapAngleSafe(this.VisualPhase);
			float num6 = 0f;
			if (this.PhaseRate != 0f)
			{
				num6 = -this._ownerShip.GameEntity.GetLocalFrame().rotation.GetEulerAngles().y;
				if (this.Side == OarSidePhaseController.OarSide.Left)
				{
					num6 = -num6;
				}
				if (num6 < 0f)
				{
					num6 = 0f;
				}
			}
			this.VisualVerticalBaseAngleOffsetFromShipRoll = MBMath.Lerp(this.VisualVerticalBaseAngleOffsetFromShipRoll, num6, dt * 3f, 1E-05f);
		}

		// Token: 0x06000A9B RID: 2715 RVA: 0x00049E18 File Offset: 0x00048018
		public void Stop()
		{
			this.PhaseRate = 0f;
			this.NeededRevolutionRate = 0f;
		}

		// Token: 0x06000A9C RID: 2716 RVA: 0x00049E30 File Offset: 0x00048030
		public bool IsInRowingMotion()
		{
			return this.PhaseRate != 0f || (!MBMath.ApproximatelyEqualsTo(this.Phase, 3.1415927f, 1E-05f) && !MBMath.ApproximatelyEqualsTo(MBMath.WrapAngleSafe(this.Phase), 3.1415927f, 1E-05f)) || (!MBMath.ApproximatelyEqualsTo(this.VisualPhase, 3.1415927f, 1E-05f) && !MBMath.ApproximatelyEqualsTo(MBMath.WrapAngleSafe(this.VisualPhase), 3.1415927f, 1E-05f));
		}

		// Token: 0x06000A9D RID: 2717 RVA: 0x00049EB7 File Offset: 0x000480B7
		public float GetLastSubmergedHeightFactorForActuators()
		{
			return MathF.Clamp(this._ownerShip.Physics.LastSubmergedHeightFactorForActuators, 0f, 1f);
		}

		// Token: 0x04000621 RID: 1569
		public const float RaisedPhase = 3.1415927f;

		// Token: 0x04000622 RID: 1570
		public const float LoweredPhase = 0f;

		// Token: 0x04000623 RID: 1571
		private OarDeckParameters _averageDeckParameters;

		// Token: 0x04000625 RID: 1573
		private float _lastPhase;

		// Token: 0x0400062D RID: 1581
		private readonly MissionShip _ownerShip;

		// Token: 0x0200020E RID: 526
		public enum OarSide
		{
			// Token: 0x04000EB7 RID: 3767
			Left,
			// Token: 0x04000EB8 RID: 3768
			Right
		}
	}
}
