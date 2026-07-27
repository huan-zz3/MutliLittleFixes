using System;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using NavalDLC.Missions.Objects.UsableMachines;
using NavalDLC.Missions.ShipControl;
using NavalDLC.Missions.ShipInput;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.SiegeWeapon;
using TaleWorlds.MountAndBlade.View.Screens;

namespace NavalDLC.View.MissionViews
{
	// Token: 0x0200001A RID: 26
	public class MissionShipControlView : MissionBattleUIBaseView
	{
		// Token: 0x17000003 RID: 3
		// (get) Token: 0x060000A8 RID: 168 RVA: 0x000061D3 File Offset: 0x000043D3
		// (set) Token: 0x060000A9 RID: 169 RVA: 0x000061DB File Offset: 0x000043DB
		public MissionShipControlView.CameraModes ActiveCameraMode { get; protected set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x060000AA RID: 170 RVA: 0x000061E4 File Offset: 0x000043E4
		// (set) Token: 0x060000AB RID: 171 RVA: 0x000061EC File Offset: 0x000043EC
		public ShipControllerMachine ControllerMachine { get; private set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x060000AC RID: 172 RVA: 0x000061F5 File Offset: 0x000043F5
		protected bool IsAimingWithRangedWeaponAndAllowed
		{
			get
			{
				return this.IsAimingWithRangedWeapon && this.IsAimingWithRangedWeaponAllowed;
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x060000AD RID: 173 RVA: 0x00006207 File Offset: 0x00004407
		protected bool IsAimingWithRangedWeaponAllowed
		{
			get
			{
				return !base.Mission.IsOrderMenuOpen && !this._wasOrderMenuOpenLastFrame && this.RangedSiegeWeapon != null && !this.RangedSiegeWeapon.IsDisabled && !this.RangedSiegeWeapon.IsDestroyed;
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x060000AE RID: 174 RVA: 0x00006243 File Offset: 0x00004443
		protected bool IsDisplayingADialog
		{
			get
			{
				MissionScreen missionScreen = base.MissionScreen;
				if (missionScreen == null || !missionScreen.GetDisplayDialog())
				{
					MissionScreen missionScreen2 = base.MissionScreen;
					if (missionScreen2 == null || !missionScreen2.IsRadialMenuActive)
					{
						Mission mission = base.Mission;
						return mission != null && mission.IsOrderMenuOpen;
					}
				}
				return true;
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x060000AF RID: 175 RVA: 0x00006280 File Offset: 0x00004480
		// (set) Token: 0x060000B0 RID: 176 RVA: 0x00006288 File Offset: 0x00004488
		private protected RangedSiegeWeapon RangedSiegeWeapon { protected get; private set; }

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x060000B1 RID: 177 RVA: 0x00006291 File Offset: 0x00004491
		// (set) Token: 0x060000B2 RID: 178 RVA: 0x00006299 File Offset: 0x00004499
		private protected RangedSiegeWeapon DirectlyControlledRangedSiegeWeapon { protected get; private set; }

		// Token: 0x060000B3 RID: 179 RVA: 0x000062A2 File Offset: 0x000044A2
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this.NavalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x000062BA File Offset: 0x000044BA
		public override void OnPreMissionTick(float dt)
		{
			base.OnPreMissionTick(dt);
			this.HandleShipControls(dt);
			this.HandleShipCamera(dt);
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x000062D4 File Offset: 0x000044D4
		public override void OnObjectUsed(Agent userAgent, UsableMissionObject usedObject)
		{
			StandingPoint standingPoint;
			if (userAgent.IsMainAgent && (standingPoint = usedObject as StandingPoint) != null)
			{
				UsableMachine usableMachineFromPoint = MissionShipControlView.GetUsableMachineFromPoint(standingPoint);
				ShipControllerMachine shipControllerMachine;
				RangedSiegeWeapon rangedSiegeWeapon;
				MissionShip firstScriptOfType;
				if ((shipControllerMachine = usableMachineFromPoint as ShipControllerMachine) != null)
				{
					this.ControllerMachine = shipControllerMachine;
					RangedSiegeWeapon firstScriptInFamilyDescending = MBExtensions.GetFirstScriptInFamilyDescending<RangedSiegeWeapon>(shipControllerMachine.GameEntity.Root);
					if (firstScriptInFamilyDescending != null)
					{
						this.RangedSiegeWeapon = firstScriptInFamilyDescending;
						return;
					}
				}
				else if ((rangedSiegeWeapon = usableMachineFromPoint as RangedSiegeWeapon) != null && (firstScriptOfType = rangedSiegeWeapon.GameEntity.Root.GetFirstScriptOfType<MissionShip>()) != null)
				{
					this.DirectlyControlledRangedSiegeWeapon = rangedSiegeWeapon;
					firstScriptOfType.OnSetRangedWeaponControlMode(true);
				}
			}
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x00006364 File Offset: 0x00004564
		public override void OnObjectStoppedBeingUsed(Agent userAgent, UsableMissionObject usedObject)
		{
			StandingPoint standingPoint;
			if (userAgent.IsMainAgent && (standingPoint = usedObject as StandingPoint) != null)
			{
				UsableMachine usableMachineFromPoint = MissionShipControlView.GetUsableMachineFromPoint(standingPoint);
				if (usableMachineFromPoint is ShipControllerMachine)
				{
					RangedSiegeWeapon rangedSiegeWeapon = this.RangedSiegeWeapon;
					if (rangedSiegeWeapon != null)
					{
						rangedSiegeWeapon.SetPlayerForceUse(false);
					}
					this.ControllerMachine = null;
					this.RangedSiegeWeapon = null;
					base.Mission.SetListenerAndAttenuationPosBlendFactor(0f);
					return;
				}
				RangedSiegeWeapon rangedSiegeWeapon2;
				MissionShip firstScriptOfType;
				if ((rangedSiegeWeapon2 = usableMachineFromPoint as RangedSiegeWeapon) != null && (firstScriptOfType = rangedSiegeWeapon2.GameEntity.Root.GetFirstScriptOfType<MissionShip>()) != null)
				{
					this.DirectlyControlledRangedSiegeWeapon = null;
					firstScriptOfType.OnSetRangedWeaponControlMode(false);
				}
			}
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x000063F8 File Offset: 0x000045F8
		private static UsableMachine GetUsableMachineFromPoint(StandingPoint standingPoint)
		{
			WeakGameEntity weakGameEntity = standingPoint.GameEntity;
			while (weakGameEntity.IsValid && !weakGameEntity.HasScriptOfType<UsableMachine>())
			{
				weakGameEntity = weakGameEntity.Parent;
			}
			if (weakGameEntity.IsValid)
			{
				UsableMachine firstScriptOfType = weakGameEntity.GetFirstScriptOfType<UsableMachine>();
				if (firstScriptOfType != null)
				{
					return firstScriptOfType;
				}
			}
			return null;
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x00006440 File Offset: 0x00004640
		private void TickRowerInput(Vec2 inputVec, out RowerLongitudinalInput longitudinalRowerControl, out RowerLongitudinalInput longitudinalControlDoubleTap, out RowerLateralInput lateralRowerControl)
		{
			int num = 0;
			int num2 = 0;
			if (inputVec.LengthSquared > 0f)
			{
				inputVec.Normalize();
				float num3 = MBMath.ToDegrees(inputVec.RotationInRadians);
				bool flag = false;
				if (num3 < 0f)
				{
					flag = true;
					num3 = -num3;
				}
				if (num3 <= 22.5f)
				{
					num = 1;
				}
				else if (num3 <= 67.5f)
				{
					num = 1;
					num2 = 1;
				}
				else if (num3 <= 112.5f)
				{
					num2 = 1;
				}
				else if (num3 < 157.5f)
				{
					num = -1;
					num2 = 1;
				}
				else
				{
					num = -1;
				}
				if (flag)
				{
					num2 = -num2;
				}
			}
			bool flag2 = num == 1 && this._lastAccelerationAxisInput == 1;
			bool flag3 = num == -1 && this._lastAccelerationAxisInput == -1;
			this._lastAccelerationAxisInput = num;
			bool flag4 = false;
			bool flag5 = false;
			longitudinalRowerControl = (RowerLongitudinalInput)num;
			longitudinalControlDoubleTap = RowerLongitudinalInput.None;
			if (num == 1)
			{
				if (flag2 && this._lastForwardKeyPressTime + 0.3f > Time.ApplicationTime)
				{
					longitudinalControlDoubleTap = RowerLongitudinalInput.Forward;
					flag4 = true;
				}
			}
			else if (num == -1 && flag3 && this._lastBackwardKeyPressTime + 0.3f > Time.ApplicationTime)
			{
				longitudinalControlDoubleTap = RowerLongitudinalInput.Backward;
				flag5 = true;
			}
			lateralRowerControl = (RowerLateralInput)num2;
			if (!flag4 && flag2)
			{
				this._lastForwardKeyPressTime = Time.ApplicationTime;
			}
			if (!flag5 && flag3)
			{
				this._lastBackwardKeyPressTime = Time.ApplicationTime;
			}
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x0000656E File Offset: 0x0000476E
		private float TickRudderInput(Vec2 inputVec)
		{
			return MathF.Min(MathF.Abs(inputVec.x) * 1.4f, 1f) * (float)MathF.Sign(inputVec.x);
		}

		// Token: 0x060000BA RID: 186 RVA: 0x00006598 File Offset: 0x00004798
		private void HandleShipControls(float dt)
		{
			this._wasOrderMenuOpenLastFrame = base.Mission.IsOrderMenuOpen;
			NavalShipsLogic navalShipsLogic = this.NavalShipsLogic;
			MissionShip missionShip = ((navalShipsLogic != null) ? navalShipsLogic.PlayerControlledShip : null);
			if (missionShip != null && missionShip.IsPlayerControlled)
			{
				PlayerShipController playerController = missionShip.PlayerController;
				RowerLongitudinalInput rowerLongitudinalInput = RowerLongitudinalInput.None;
				RowerLongitudinalInput rowerLongitudinalInput2 = RowerLongitudinalInput.None;
				RowerLateralInput rowerLateralInput = RowerLateralInput.None;
				float num = 0f;
				if (!base.MissionScreen.IsCheatGhostMode)
				{
					float gameKeyAxis = base.Input.GetGameKeyAxis("MovementAxisY");
					float gameKeyAxis2 = base.Input.GetGameKeyAxis("MovementAxisX");
					Vec2 vec;
					vec..ctor(gameKeyAxis2, gameKeyAxis);
					if (MathF.Abs(vec.x) <= 0.2f)
					{
						vec.x = 0f;
					}
					if (MathF.Abs(vec.y) <= 0.2f)
					{
						vec.y = 0f;
					}
					this.TickRowerInput(vec, out rowerLongitudinalInput, out rowerLongitudinalInput2, out rowerLateralInput);
					num = this.TickRudderInput(vec);
				}
				ShipInputRecord shipInputRecord = new ShipInputRecord(rowerLateralInput, rowerLongitudinalInput, rowerLongitudinalInput2, num, this.SailControl);
				playerController.SetInput(in shipInputRecord);
			}
		}

		// Token: 0x060000BB RID: 187 RVA: 0x0000669B File Offset: 0x0000489B
		public void SetSailInput(SailInput sailInput)
		{
			this.SailControl = sailInput;
		}

		// Token: 0x060000BC RID: 188 RVA: 0x000066A4 File Offset: 0x000048A4
		public void SetActiveCameraMode(MissionShipControlView.CameraModes mode)
		{
			this.ActiveCameraMode = mode;
		}

		// Token: 0x060000BD RID: 189 RVA: 0x000066B0 File Offset: 0x000048B0
		private void HandleShipCamera(float dt)
		{
			if (this.ControllerMachine != null)
			{
				if (this.RangedSiegeWeapon != null)
				{
					if (this.RangedSiegeWeapon.GetComponent<RangedSiegeWeaponView>() == null)
					{
						RangedSiegeWeaponView rangedSiegeWeaponView = new BallistaView();
						rangedSiegeWeaponView.Initialize(this.RangedSiegeWeapon, base.MissionScreen);
						this.RangedSiegeWeapon.AddComponent(rangedSiegeWeaponView);
					}
					this.RangedSiegeWeapon.SetPlayerForceUse(this.IsAimingWithRangedWeaponAndAllowed);
				}
				Agent pilotAgent = this.ControllerMachine.PilotAgent;
				Vec3 vec;
				float num;
				Vec3 vec2;
				switch (this.ActiveCameraMode)
				{
				case MissionShipControlView.CameraModes.Back:
					vec = this.ControllerMachine.BackCameraOffset * 0.5f;
					num = this.ControllerMachine.BackCameraFovMultiplier;
					if (base.Mission.InputManager.IsGameKeyDown(28))
					{
						this._backCameraDistanceMultiplier -= 0.5f * dt;
					}
					if (base.Mission.InputManager.IsGameKeyDown(29))
					{
						this._backCameraDistanceMultiplier += 0.5f * dt;
					}
					this._backCameraDistanceMultiplier = MBMath.ClampFloat(this._backCameraDistanceMultiplier, 0.2f, 3f);
					vec2..ctor(this.ControllerMachine.BackCameraTargetLocalPosition.AsVec2, this.ControllerMachine.BackCameraTargetLocalPosition.z * this._backCameraDistanceMultiplier, -1f);
					base.Mission.SetListenerAndAttenuationPosBlendFactor(0.33f);
					goto IL_01C8;
				case MissionShipControlView.CameraModes.Front:
					vec = this.ControllerMachine.FrontCameraOffset;
					vec2 = this.ControllerMachine.FrontCameraTargetLocalPosition;
					num = this.ControllerMachine.FrontCameraFovMultiplier;
					base.Mission.SetListenerAndAttenuationPosBlendFactor(1f);
					goto IL_01C8;
				}
				vec = this.ControllerMachine.ShoulderCameraOffset;
				vec2 = this.ControllerMachine.ShoulderCameraTargetLocalPosition;
				num = this.ControllerMachine.ShoulderCameraFovMultiplier;
				base.Mission.SetListenerAndAttenuationPosBlendFactor(0f);
				IL_01C8:
				bool flag = (!this._lastCameraOffset.NearlyEquals(ref vec, 0.001f) || MathF.Abs(this._lastCameraFovMultiplier - num) > 0.001f) && !this.IsAimingWithRangedWeaponAndAllowed;
				this._lastCameraOffset = (flag ? MBMath.Lerp(this._lastCameraOffset, vec, dt * 5f, 0.001f) : vec);
				this._lastCameraFovMultiplier = (flag ? MBMath.Lerp(this._lastCameraFovMultiplier, num, dt * 5f, 0.001f) : num);
				WeakGameEntity root = this.ControllerMachine.GameEntity.Root;
				float num2;
				Vec3 vec3;
				if (pilotAgent != null)
				{
					num2 = MBMath.WrapAngle(base.MissionScreen.CameraBearing - pilotAgent.MovementDirectionAsAngle);
					vec3 = pilotAgent.Position;
				}
				else
				{
					num2 = MBMath.WrapAngle(base.MissionScreen.CameraBearing);
					vec3 = root.GlobalPosition;
				}
				Vec3 vec4;
				if (!vec2.IsNonZero)
				{
					vec4 = Vec3.Zero;
				}
				else
				{
					Vec3 vec5 = vec3;
					MatrixFrame matrixFrame = this.ControllerMachine.GameEntity.GetGlobalFrame();
					Vec3 vec6 = vec5 - matrixFrame.TransformToParent(ref vec2);
					Vec3 vec7;
					if (this.ActiveCameraMode != MissionShipControlView.CameraModes.Shoulder)
					{
						if (this.ActiveCameraMode != MissionShipControlView.CameraModes.Front)
						{
							vec7 = Vec3.Zero;
						}
						else
						{
							matrixFrame = this.ControllerMachine.AttachedShip.GameEntity.GetGlobalFrame();
							vec7 = matrixFrame.rotation.f.NormalizedCopy() * MathF.Cos(Math.Min(MathF.Abs(num2) * 2.5f, 1.5707964f)) * 8f;
						}
					}
					else
					{
						matrixFrame = this.ControllerMachine.AttachedShip.GameEntity.GetGlobalFrame();
						vec7 = matrixFrame.rotation.s.NormalizedCopy() * MathF.Sin(num2) * this.ControllerMachine.ShoulderCameraDistance;
					}
					vec4 = vec6 - vec7;
				}
				Vec3 vec8 = vec4;
				Mission.Current.SetCustomCameraFixedDistance((this.ActiveCameraMode == MissionShipControlView.CameraModes.Front) ? this.ControllerMachine.FrontCameraDistance : ((this.ActiveCameraMode == MissionShipControlView.CameraModes.Back) ? (vec.Length * this._backCameraDistanceMultiplier) : float.MinValue));
				Mission.Current.SetCustomCameraTargetLocalOffset(MBMath.Lerp(Mission.Current.CustomCameraTargetLocalOffset, -vec8, dt * 10f, 0.001f));
				if (this.ActiveCameraMode == MissionShipControlView.CameraModes.Shoulder)
				{
					if (!flag)
					{
						Mission.Current.SetIgnoredEntityForCamera(null);
					}
				}
				else if (Mission.Current.IgnoredEntityForCamera != root)
				{
					Mission.Current.SetIgnoredEntityForCamera(GameEntity.CreateFromWeakEntity(root));
				}
				Mission.Current.SetCustomCameraIgnoreCollision(this.ActiveCameraMode == MissionShipControlView.CameraModes.Front);
			}
			else
			{
				this._lastCameraOffset = MBMath.Lerp(this._lastCameraOffset, Vec3.Zero, dt * 5f, 0.001f);
				this._lastCameraFovMultiplier = MBMath.Lerp(this._lastCameraFovMultiplier, 1f, dt * 5f, 0.001f);
				Mission.Current.SetCustomCameraFixedDistance(float.MinValue);
				Mission.Current.SetCustomCameraTargetLocalOffset(MBMath.Lerp(Mission.Current.CustomCameraTargetLocalOffset, Vec3.Zero, dt * 5f, 0.001f));
				if (!this._lastCameraOffset.IsNonZero)
				{
					Mission.Current.SetIgnoredEntityForCamera(null);
				}
				Mission.Current.SetCustomCameraIgnoreCollision(false);
			}
			Mission.Current.SetCustomCameraLocalOffset(this._lastCameraOffset);
			Mission.Current.SetCustomCameraFovMultiplier(this._lastCameraFovMultiplier);
		}

		// Token: 0x060000BE RID: 190 RVA: 0x00006BD8 File Offset: 0x00004DD8
		protected override void OnCreateView()
		{
		}

		// Token: 0x060000BF RID: 191 RVA: 0x00006BDA File Offset: 0x00004DDA
		protected override void OnDestroyView()
		{
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x00006BDC File Offset: 0x00004DDC
		protected override void OnSuspendView()
		{
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x00006BDE File Offset: 0x00004DDE
		protected override void OnResumeView()
		{
		}

		// Token: 0x04000038 RID: 56
		protected SailInput SailControl;

		// Token: 0x04000039 RID: 57
		protected NavalShipsLogic NavalShipsLogic;

		// Token: 0x0400003A RID: 58
		private Vec3 _lastCameraOffset;

		// Token: 0x0400003B RID: 59
		private float _lastCameraFovMultiplier = 1f;

		// Token: 0x0400003D RID: 61
		private bool _wasOrderMenuOpenLastFrame;

		// Token: 0x0400003E RID: 62
		protected bool IsAimingWithRangedWeapon;

		// Token: 0x0400003F RID: 63
		private float _backCameraDistanceMultiplier = 1f;

		// Token: 0x04000040 RID: 64
		private float _lastForwardKeyPressTime;

		// Token: 0x04000041 RID: 65
		private float _lastBackwardKeyPressTime;

		// Token: 0x04000042 RID: 66
		private int _lastAccelerationAxisInput;

		// Token: 0x02000046 RID: 70
		public enum CameraModes
		{
			// Token: 0x040000F0 RID: 240
			Back,
			// Token: 0x040000F1 RID: 241
			Shoulder,
			// Token: 0x040000F2 RID: 242
			Front,
			// Token: 0x040000F3 RID: 243
			NumPositions
		}
	}
}
