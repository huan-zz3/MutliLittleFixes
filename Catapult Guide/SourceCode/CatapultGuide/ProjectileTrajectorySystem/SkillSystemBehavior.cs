using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.ScreenSystem;

namespace ProjectileTrajectorySystem
{
	// Token: 0x02000007 RID: 7
	[NullableContext(1)]
	[Nullable(0)]
	public class SkillSystemBehavior : MissionLogic
	{
		// Token: 0x06000009 RID: 9 RVA: 0x0000213C File Offset: 0x0000033C
		private static void InitDebugRender()
		{
			bool flag = SkillSystemBehavior._renderLineMethod != null;
			if (!flag)
			{
				try
				{
					Type type = Type.GetType("TaleWorlds.Engine.EngineApplicationInterface, TaleWorlds.Engine");
					bool flag2 = type == null;
					if (!flag2)
					{
						FieldInfo field = type.GetField("IDebug", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
						object obj = ((field != null) ? field.GetValue(null) : null);
						SkillSystemBehavior._debugInterface = obj;
						bool flag3 = obj == null;
						if (!flag3)
						{
							SkillSystemBehavior._renderLineMethod = obj.GetType().GetMethod("RenderDebugLine", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[]
							{
								typeof(Vec3),
								typeof(Vec3),
								typeof(uint),
								typeof(bool),
								typeof(float)
							}, null);
							SkillSystemBehavior._renderSphereMethod = obj.GetType().GetMethod("RenderDebugSphere", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[]
							{
								typeof(Vec3),
								typeof(float),
								typeof(uint),
								typeof(bool),
								typeof(float)
							}, null);
						}
					}
				}
				catch
				{
				}
			}
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002290 File Offset: 0x00000490
		private static void RenderLine(Vec3 start, Vec3 end, uint color)
		{
			bool flag = SkillSystemBehavior._renderLineMethod == null;
			if (flag)
			{
				SkillSystemBehavior.InitDebugRender();
			}
			try
			{
				MethodInfo renderLineMethod = SkillSystemBehavior._renderLineMethod;
				if (renderLineMethod != null)
				{
					renderLineMethod.Invoke(SkillSystemBehavior._debugInterface, new object[]
					{
						start,
						end - start,
						color,
						false,
						0f
					});
				}
			}
			catch
			{
			}
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002320 File Offset: 0x00000520
		private static void RenderSphere(Vec3 pos, float radius, uint color)
		{
			bool flag = SkillSystemBehavior._renderSphereMethod == null;
			if (flag)
			{
				SkillSystemBehavior.InitDebugRender();
			}
			try
			{
				MethodInfo renderSphereMethod = SkillSystemBehavior._renderSphereMethod;
				if (renderSphereMethod != null)
				{
					renderSphereMethod.Invoke(SkillSystemBehavior._debugInterface, new object[] { pos, radius, color, false, 0f });
				}
			}
			catch
			{
			}
		}

		// Token: 0x0600000C RID: 12 RVA: 0x000023AC File Offset: 0x000005AC
		public override void OnAfterMissionCreated()
		{
			base.OnAfterMissionCreated();
			this.Cleanup();
		}

		// Token: 0x0600000D RID: 13 RVA: 0x000023BD File Offset: 0x000005BD
		protected override void OnEndMission()
		{
			base.OnEndMission();
			this.ResetCustomCamera();
			this.Cleanup();
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000023D8 File Offset: 0x000005D8
		private void Cleanup()
		{
			SkillSystemBehavior.WoW_AgentMissileSpeedData.Clear();
			this._camYawOffset = 0f;
			this._camPitchOffset = 0f;
			this._captainControlWeapon = null;
			this._isCaptainModeActive = false;
			this._isRtsModeEnabled = false;
			this._aimYaw = 0f;
			this._aimPitch = 0f;
			this._shipRoot = default(WeakGameEntity);
			this._lmbWasDown = false;
			this._captainCamSmoothInited = false;
			this._captainCamSmoothPos = Vec3.Zero;
			this._captainCamSmoothForward = Vec3.Forward;
			this._shipVelInited = false;
			this._shipVel = Vec3.Zero;
			this._fireDirSmoothInited = false;
			this._fireDirSmooth = Vec3.Forward;
			this._shotFreezeTimer = 0f;
			this._freezeMuzzle = Vec3.Invalid;
			this._freezeFireDir = Vec3.Forward;
			this._muzzleSmoothInited = false;
			this._muzzleSmoothPos = Vec3.Zero;
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000024BC File Offset: 0x000006BC
		private void ResetCustomCamera()
		{
			MissionScreen missionScreen = ScreenManager.TopScreen as MissionScreen;
			bool flag = missionScreen != null && this._customCamera != null && missionScreen.CustomCamera == this._customCamera;
			if (flag)
			{
				missionScreen.CustomCamera = null;
			}
			this._customCamera = null;
			this._camYawOffset = 0f;
			this._camPitchOffset = 0f;
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002524 File Offset: 0x00000724
		private void ExitCaptainMode(string msg = null)
		{
			bool flag = !string.IsNullOrEmpty(msg);
			if (flag)
			{
				InformationManager.DisplayMessage(new InformationMessage(msg, Colors.Yellow));
			}
			this._isCaptainModeActive = false;
			this._captainControlWeapon = null;
			this._shipRoot = default(WeakGameEntity);
			this._captainCamSmoothInited = false;
			this._shipVelInited = false;
			this._fireDirSmoothInited = false;
			this._shotFreezeTimer = 0f;
			this._freezeMuzzle = Vec3.Invalid;
			this._freezeFireDir = Vec3.Forward;
			this._muzzleSmoothInited = false;
			this._muzzleSmoothPos = Vec3.Zero;
			bool isRtsModeEnabled = this._isRtsModeEnabled;
			if (isRtsModeEnabled)
			{
				this.DisableRtsMode();
			}
			else
			{
				this.ResetCustomCamera();
			}
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000025CC File Offset: 0x000007CC
		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			Agent main = Agent.Main;
			bool flag = main == null || !main.IsActive();
			if (flag)
			{
				bool isRtsModeEnabled = this._isRtsModeEnabled;
				if (isRtsModeEnabled)
				{
					this.DisableRtsMode();
				}
			}
			else
			{
				bool flag2 = main.CurrentlyUsedGameObject != null;
				if (flag2)
				{
					bool flag3 = Input.IsKeyPressed(225);
					if (flag3)
					{
						bool isCaptainModeActive = this._isCaptainModeActive;
						if (isCaptainModeActive)
						{
							this.ExitCaptainMode("舰长模式: 关闭");
						}
						else
						{
							WeakGameEntity gameEntity = main.CurrentlyUsedGameObject.GameEntity;
							bool flag4 = SkillSystemBehavior.IsOperatingSiegeWeapon(gameEntity);
							if (flag4)
							{
								InformationManager.DisplayMessage(new InformationMessage("舰长模式仅用于海战操控船只时远程控制弩炮。", Colors.Gray));
							}
							else
							{
								WeakGameEntity weakGameEntity = gameEntity;
								while (weakGameEntity.Parent.IsValid)
								{
									weakGameEntity = weakGameEntity.Parent;
								}
								bool isValid = weakGameEntity.IsValid;
								if (isValid)
								{
									RangedSiegeWeapon rangedSiegeWeapon = this.FindFirstRangedSiegeWeapon(weakGameEntity);
									bool flag5 = rangedSiegeWeapon != null && !SkillSystemBehavior.IsLobber(rangedSiegeWeapon);
									if (flag5)
									{
										this._isCaptainModeActive = true;
										this._shipRoot = weakGameEntity;
										this._captainControlWeapon = rangedSiegeWeapon;
										InformationManager.DisplayMessage(new InformationMessage("舰长模式: 开启", Colors.Green));
										this._shipVelInited = false;
										this._lmbWasDown = false;
										this._captainCamSmoothInited = false;
										this._aimYaw = 0f;
										this._aimPitch = 0f;
										this._fireDirSmoothInited = false;
										this._shotFreezeTimer = 0f;
										this._freezeMuzzle = Vec3.Invalid;
										this._freezeFireDir = Vec3.Forward;
										this._muzzleSmoothInited = false;
										this._muzzleSmoothPos = Vec3.Zero;
									}
									else
									{
										InformationManager.DisplayMessage(new InformationMessage("未找到可接管的海战弩炮（投石车不支持舰长模式）。", Colors.Gray));
									}
								}
								else
								{
									InformationManager.DisplayMessage(new InformationMessage("船体无效", Colors.Gray));
								}
							}
						}
					}
				}
				else
				{
					bool isCaptainModeActive2 = this._isCaptainModeActive;
					if (isCaptainModeActive2)
					{
						this.ExitCaptainMode(null);
					}
				}
				bool flag6 = this._isCaptainModeActive && this._captainControlWeapon != null;
				if (flag6)
				{
					bool flag7 = Input.IsKeyPressed(33);
					if (flag7)
					{
						this.ExitCaptainMode("舰长模式: 已退出 (按下F)");
					}
					else
					{
						bool flag8 = Input.IsKeyPressed(226);
						if (flag8)
						{
							this.ToggleRtsMode();
							bool isRtsModeEnabled2 = this._isRtsModeEnabled;
							if (isRtsModeEnabled2)
							{
								this.InitCaptainAimFromCurrentWeapon(main, this._captainControlWeapon);
							}
						}
						MatrixFrame matrixFrame;
						if (!this._shipRoot.IsValid)
						{
							Vec3 position = main.Position;
							Vec3 vec = main.Position + main.LookDirection;
							matrixFrame = MatrixFrame.CreateLookAt(ref position, ref vec, ref Vec3.Up);
						}
						else
						{
							matrixFrame = this._shipRoot.GetGlobalFrame();
						}
						MatrixFrame matrixFrame2 = matrixFrame;
						Mat3 mat = SkillSystemBehavior.CreateFlatYawBasis(matrixFrame2.rotation.f);
						bool flag9 = !this._isRtsModeEnabled;
						if (flag9)
						{
							this.ComputeYawPitchFromLook(mat, main.LookDirection, out this._aimYaw, out this._aimPitch);
						}
						else
						{
							float mouseMoveX = Input.GetMouseMoveX();
							float mouseMoveY = Input.GetMouseMoveY();
							this._aimYaw += mouseMoveX * 0.001f;
							this._aimPitch -= mouseMoveY * 0.001f;
							this._aimYaw = MBMath.ClampFloat(this._aimYaw, -1.5707964f, 1.5707964f);
							this._aimPitch = MBMath.ClampFloat(this._aimPitch, -0.35f, 0.85f);
						}
						Mat3 mat2 = mat;
						mat2.RotateAboutUp(this._aimYaw);
						mat2.RotateAboutSide(this._aimPitch);
						Vec3 f = mat2.f;
						bool flag10 = f.LengthSquared > 1E-06f;
						if (flag10)
						{
							f.Normalize();
						}
						this.OverrideWeaponRotation(this._captainControlWeapon, f);
						this.UpdateShipVelocity(dt);
						Vec3 vec2 = this.SelectBestFireDirection(this._captainControlWeapon, f);
						bool isRtsModeEnabled3 = this._isRtsModeEnabled;
						Vec3 vec3;
						if (isRtsModeEnabled3)
						{
							vec3 = this.SmoothFireDirection(vec2, dt);
						}
						else
						{
							vec3 = vec2;
							this._fireDirSmoothInited = true;
							this._fireDirSmooth = vec3;
						}
						bool flag11 = SkillSystemBehavior.IsLobber(this._captainControlWeapon);
						float num = SkillSystemBehavior.GetShootingSpeed(this._captainControlWeapon);
						ItemObject ammoItem = SkillSystemBehavior.GetAmmoItem(this._captainControlWeapon);
						float ammoMissileSpeed = SkillSystemBehavior.GetAmmoMissileSpeed(ammoItem);
						bool flag12 = ammoMissileSpeed > 1f && ammoMissileSpeed < 500f;
						if (flag12)
						{
							num = ammoMissileSpeed;
						}
						float dynamicFriction = SkillSystemBehavior.GetDynamicFriction(this._captainControlWeapon, ammoItem);
						Vec3 vec4 = ProjectileTrajectorySystem.GetRealMuzzlePosition(this._captainControlWeapon);
						bool flag13 = vec4 == Vec3.Invalid;
						if (flag13)
						{
							vec4 = this._captainControlWeapon.GameEntity.GetGlobalFrame().origin;
						}
						Vec3 vec5 = (this._isRtsModeEnabled ? this.SmoothMuzzlePosition(vec4, dt) : vec4);
						bool flag14 = !this._isRtsModeEnabled;
						if (flag14)
						{
							this._muzzleSmoothInited = true;
							this._muzzleSmoothPos = vec5;
						}
						Vec3 vec6 = vec5 + vec3 * 0.25f;
						Vec3 vec7 = vec3 * num + this._shipVel;
						float num2 = (flag11 ? 0.1f : 0.02f);
						bool flag15 = Input.IsKeyDown(224);
						bool flag16 = flag15 && !this._lmbWasDown;
						bool flag17 = flag16;
						if (flag17)
						{
							this._shotFreezeTimer = 0.28f;
							this._freezeMuzzle = vec5;
							this._freezeFireDir = vec3;
							this._freezeStart = vec6;
							this._freezeVelocity = vec7;
							this._freezeFriction = dynamicFriction;
							this._freezeUseQuadratic = flag11;
							this._freezeDrawPath = this._isRtsModeEnabled;
							this._freezeIgnoreTime = num2;
						}
						this._lmbWasDown = flag15;
						bool flag18 = flag16;
						if (flag18)
						{
							this.ForceFireWeapon(this._captainControlWeapon);
						}
						bool flag19 = this._shotFreezeTimer > 0f;
						if (flag19)
						{
							this._shotFreezeTimer -= dt;
							bool flag20 = this._shotFreezeTimer < 0f;
							if (flag20)
							{
								this._shotFreezeTimer = 0f;
							}
							this.DrawCaptainPrediction(this._freezeMuzzle, this._freezeFireDir, this._freezeStart, this._freezeVelocity, this._freezeFriction, this._freezeIgnoreTime, this._freezeDrawPath, this._freezeUseQuadratic);
						}
						else
						{
							this.DrawCaptainPrediction(vec5, vec3, vec6, vec7, dynamicFriction, num2, this._isRtsModeEnabled, flag11);
						}
					}
				}
				else
				{
					UsableMissionObject currentlyUsedGameObject = main.CurrentlyUsedGameObject;
					WeakGameEntity weakGameEntity2 = main.GetSteppedEntity();
					bool flag21 = currentlyUsedGameObject != null && currentlyUsedGameObject.GameEntity.IsValid;
					if (flag21)
					{
						weakGameEntity2 = currentlyUsedGameObject.GameEntity;
					}
					RangedSiegeWeapon rangedSiegeWeapon2 = null;
					WeakGameEntity weakGameEntity3 = weakGameEntity2;
					while (weakGameEntity3.IsValid)
					{
						RangedSiegeWeapon firstScriptOfType = weakGameEntity3.GetFirstScriptOfType<RangedSiegeWeapon>();
						bool flag22 = firstScriptOfType != null;
						if (flag22)
						{
							rangedSiegeWeapon2 = firstScriptOfType;
							break;
						}
						bool flag23 = !weakGameEntity3.Parent.IsValid;
						if (flag23)
						{
							break;
						}
						weakGameEntity3 = weakGameEntity3.Parent;
					}
					bool flag24 = this._currentSiegeWeapon != rangedSiegeWeapon2;
					if (flag24)
					{
						bool isRtsModeEnabled4 = this._isRtsModeEnabled;
						if (isRtsModeEnabled4)
						{
							this.DisableRtsMode();
						}
						this._currentSiegeWeapon = rangedSiegeWeapon2;
					}
					bool flag25 = this._currentSiegeWeapon != null;
					if (flag25)
					{
						bool flag26 = Input.IsKeyPressed(226);
						if (flag26)
						{
							bool flag27 = SkillSystemBehavior.IsLobber(this._currentSiegeWeapon);
							if (flag27)
							{
								this.ToggleRtsMode();
							}
						}
						ProjectileTrajectorySystem.UpdateTrajectory(main, this._currentSiegeWeapon);
					}
					else
					{
						bool isRtsModeEnabled5 = this._isRtsModeEnabled;
						if (isRtsModeEnabled5)
						{
							this.DisableRtsMode();
						}
						ProjectileTrajectorySystem.UpdateTrajectoryRangeWeapon(main);
					}
					this._cleanupTimer += dt;
					bool flag28 = this._cleanupTimer >= 5f;
					if (flag28)
					{
						this.CleanDeadAgentEntries();
						this._cleanupTimer = 0f;
					}
				}
			}
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002D5C File Offset: 0x00000F5C
		private void DrawCaptainPrediction(Vec3 muzzle, Vec3 fireDir, Vec3 simStart, Vec3 v0, float friction, float ignoreTime, bool drawPath, bool useQuadratic)
		{
			Vec3 vec;
			float num;
			bool flag = this.TryGetImmediateBlockHit(muzzle, fireDir, 3f, out vec, out num) && num <= 2f;
			if (flag)
			{
				SkillSystemBehavior.RenderSphere(vec, 0.4f, 4294901760U);
				if (drawPath)
				{
					SkillSystemBehavior.RenderLine(muzzle, vec, uint.MaxValue);
				}
			}
			else
			{
				if (drawPath)
				{
					SkillSystemBehavior.RenderLine(muzzle, simStart, uint.MaxValue);
				}
				Vec3 vec2;
				this.SimulateTrajectoryCaptain(simStart, v0, friction, ignoreTime, drawPath, useQuadratic, out vec2);
				bool flag2 = vec2 != Vec3.Invalid;
				if (flag2)
				{
					SkillSystemBehavior.RenderSphere(vec2, 0.4f, 4294901760U);
				}
			}
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002DFC File Offset: 0x00000FFC
		public override void OnPreDisplayMissionTick(float dt)
		{
			base.OnPreDisplayMissionTick(dt);
			MissionScreen missionScreen = ScreenManager.TopScreen as MissionScreen;
			bool flag = missionScreen == null;
			if (!flag)
			{
				RangedSiegeWeapon rangedSiegeWeapon = ((this._captainControlWeapon != null) ? this._captainControlWeapon : this._currentSiegeWeapon);
				bool flag2 = this._isRtsModeEnabled && rangedSiegeWeapon != null;
				if (flag2)
				{
					bool flag3 = this._customCamera == null;
					if (flag3)
					{
						this._customCamera = Camera.CreateCamera();
						float num = 1.3089969f;
						this._customCamera.SetFovVertical(num, Screen.AspectRatio, 0.1f, 2000f);
					}
					bool flag4 = this._isCaptainModeActive && rangedSiegeWeapon == this._captainControlWeapon;
					if (flag4)
					{
						this._customCamera.Frame = this.CalculateStableCaptainRtsCameraFrame(dt);
					}
					else
					{
						this._camYawOffset -= Input.GetMouseMoveX() * 0.003f;
						this._camPitchOffset -= Input.GetMouseMoveY() * 0.003f;
						bool flag5 = this._camPitchOffset > 1f;
						if (flag5)
						{
							this._camPitchOffset = 1f;
						}
						bool flag6 = this._camPitchOffset < -1.5f;
						if (flag6)
						{
							this._camPitchOffset = -1.5f;
						}
						this._customCamera.Frame = this.CalculateRtsCameraFrame(rangedSiegeWeapon);
					}
					missionScreen.CustomCamera = this._customCamera;
				}
				else
				{
					bool flag7 = this._customCamera != null && missionScreen.CustomCamera == this._customCamera;
					if (flag7)
					{
						missionScreen.CustomCamera = null;
					}
				}
			}
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002F94 File Offset: 0x00001194
		private MatrixFrame CalculateStableCaptainRtsCameraFrame(float dt)
		{
			bool isValid = this._shipRoot.IsValid;
			MatrixFrame matrixFrame;
			if (isValid)
			{
				matrixFrame = this._shipRoot.GetGlobalFrame();
			}
			else
			{
				bool flag = this._captainControlWeapon != null;
				if (!flag)
				{
					return MatrixFrame.Identity;
				}
				matrixFrame = this._captainControlWeapon.GameEntity.GetGlobalFrame();
			}
			Vec3 origin = matrixFrame.origin;
			Vec3 vec = matrixFrame.rotation.f;
			vec.z = 0f;
			bool flag2 = vec.LengthSquared < 1E-05f;
			if (flag2)
			{
				vec = Vec3.Forward;
			}
			vec.Normalize();
			float num = 1f - (float)Math.Exp(-10.0 * (double)dt);
			bool flag3 = !this._captainCamSmoothInited;
			if (flag3)
			{
				this._captainCamSmoothInited = true;
				this._captainCamSmoothForward = vec;
				this._captainCamSmoothPos = origin + Vec3.Up * 78f - this._captainCamSmoothForward * 38f;
			}
			else
			{
				this._captainCamSmoothForward += (vec - this._captainCamSmoothForward) * num;
				bool flag4 = this._captainCamSmoothForward.LengthSquared > 1E-05f;
				if (flag4)
				{
					this._captainCamSmoothForward.Normalize();
				}
				Vec3 vec2 = origin + Vec3.Up * 78f - this._captainCamSmoothForward * 38f;
				this._captainCamSmoothPos += (vec2 - this._captainCamSmoothPos) * num;
			}
			float rotationInRadians = this._captainCamSmoothForward.AsVec2.RotationInRadians;
			Mat3 identity = Mat3.Identity;
			identity.RotateAboutUp(rotationInRadians);
			identity.RotateAboutSide(1.57f);
			identity.RotateAboutSide(-0.28f);
			MatrixFrame identity2 = MatrixFrame.Identity;
			identity2.origin = this._captainCamSmoothPos;
			identity2.rotation = identity;
			return identity2;
		}

		// Token: 0x06000015 RID: 21 RVA: 0x000031A0 File Offset: 0x000013A0
		private MatrixFrame CalculateRtsCameraFrame(RangedSiegeWeapon w)
		{
			bool flag = w == null;
			MatrixFrame matrixFrame;
			if (flag)
			{
				matrixFrame = MatrixFrame.Identity;
			}
			else
			{
				MatrixFrame globalFrame = w.GameEntity.GetGlobalFrame();
				Vec3 origin = globalFrame.origin;
				Vec3 f = globalFrame.rotation.f;
				float num = f.AsVec2.RotationInRadians + 3.1415927f;
				Mat3 identity = Mat3.Identity;
				identity.RotateAboutSide(1.5707964f);
				identity.RotateAboutForward(num + this._camYawOffset);
				identity.RotateAboutSide(this._camPitchOffset);
				Vec3 vec = origin - f * 18f;
				vec.z += 32f;
				float groundHeightAtPosition = Mission.Current.Scene.GetGroundHeightAtPosition(vec, 544321929);
				bool flag2 = vec.z < groundHeightAtPosition + 2f;
				if (flag2)
				{
					vec.z = groundHeightAtPosition + 2f;
				}
				matrixFrame = new MatrixFrame(ref identity, ref vec);
			}
			return matrixFrame;
		}

		// Token: 0x06000016 RID: 22 RVA: 0x000032A0 File Offset: 0x000014A0
		private void ToggleRtsMode()
		{
			this._isRtsModeEnabled = !this._isRtsModeEnabled;
			bool isRtsModeEnabled = this._isRtsModeEnabled;
			if (isRtsModeEnabled)
			{
				InformationManager.DisplayMessage(new InformationMessage("RTS视角: 开启", Colors.Magenta));
				this._lmbWasDown = false;
				this._captainCamSmoothInited = false;
			}
			else
			{
				InformationManager.DisplayMessage(new InformationMessage("RTS视角: 关闭", Colors.Gray));
				this.ResetCustomCamera();
				this._lmbWasDown = false;
				this._captainCamSmoothInited = false;
			}
		}

		// Token: 0x06000017 RID: 23 RVA: 0x0000331A File Offset: 0x0000151A
		private void DisableRtsMode()
		{
			this._isRtsModeEnabled = false;
			this.ResetCustomCamera();
			this._lmbWasDown = false;
			this._captainCamSmoothInited = false;
		}

		// Token: 0x06000018 RID: 24 RVA: 0x0000333C File Offset: 0x0000153C
		private void UpdateShipVelocity(float dt)
		{
			bool flag = !this._shipRoot.IsValid || dt <= 1E-05f || dt > 0.2f;
			if (flag)
			{
				this._shipVel = Vec3.Zero;
				this._shipVelInited = false;
			}
			else
			{
				Vec3 origin = this._shipRoot.GetGlobalFrame().origin;
				bool flag2 = !this._shipVelInited;
				if (flag2)
				{
					this._shipVelInited = true;
					this._lastShipPos = origin;
					this._shipVel = Vec3.Zero;
				}
				else
				{
					Vec3 vec = origin - this._lastShipPos;
					vec.z = 0f;
					Vec3 vec2 = vec * (1f / dt);
					bool flag3 = vec2.LengthSquared < 0.0625f;
					if (flag3)
					{
						vec2 = Vec3.Zero;
					}
					this._shipVel += (vec2 - this._shipVel) * 0.15f;
					this._lastShipPos = origin;
				}
			}
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00003434 File Offset: 0x00001634
		private Vec3 SmoothFireDirection(Vec3 raw, float dt)
		{
			bool flag = raw.LengthSquared < 1E-06f;
			Vec3 vec;
			if (flag)
			{
				vec = (this._fireDirSmoothInited ? this._fireDirSmooth : Vec3.Forward);
			}
			else
			{
				bool flag2 = !this._fireDirSmoothInited;
				if (flag2)
				{
					this._fireDirSmoothInited = true;
					this._fireDirSmooth = raw;
					vec = raw;
				}
				else
				{
					float num = 1f - (float)Math.Exp(-18.0 * (double)dt);
					this._fireDirSmooth += (raw - this._fireDirSmooth) * num;
					bool flag3 = this._fireDirSmooth.LengthSquared > 1E-06f;
					if (flag3)
					{
						this._fireDirSmooth.Normalize();
					}
					else
					{
						this._fireDirSmooth = raw;
					}
					vec = this._fireDirSmooth;
				}
			}
			return vec;
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00003504 File Offset: 0x00001704
		private Vec3 SmoothMuzzlePosition(Vec3 raw, float dt)
		{
			bool flag = raw == Vec3.Invalid;
			Vec3 vec;
			if (flag)
			{
				vec = raw;
			}
			else
			{
				bool flag2 = !this._muzzleSmoothInited;
				if (flag2)
				{
					this._muzzleSmoothInited = true;
					this._muzzleSmoothPos = raw;
					vec = raw;
				}
				else
				{
					bool flag3 = (this._muzzleSmoothPos - raw).LengthSquared > 4f;
					if (flag3)
					{
						this._muzzleSmoothPos = raw;
						vec = raw;
					}
					else
					{
						float num = 1f - (float)Math.Exp(-35.0 * (double)dt);
						this._muzzleSmoothPos += (raw - this._muzzleSmoothPos) * num;
						vec = this._muzzleSmoothPos;
					}
				}
			}
			return vec;
		}

		// Token: 0x0600001B RID: 27 RVA: 0x000035BC File Offset: 0x000017BC
		private void ComputeYawPitchFromLook(Mat3 flatBase, Vec3 lookDir, out float yaw, out float pitch)
		{
			Vec3 vec = lookDir;
			bool flag = vec.LengthSquared < 1E-06f;
			if (flag)
			{
				vec = flatBase.f;
			}
			vec.Normalize();
			Vec3 vec2 = vec;
			vec2.z = 0f;
			bool flag2 = vec2.LengthSquared < 1E-06f;
			if (flag2)
			{
				vec2 = flatBase.f;
			}
			vec2.Normalize();
			float num = Vec3.DotProduct(vec2, flatBase.f);
			float num2 = Vec3.DotProduct(vec2, flatBase.s);
			yaw = MathF.Atan2(num2, num);
			float length = vec.AsVec2.Length;
			pitch = ((length > 0.0001f) ? MathF.Atan2(vec.z, length) : 0f);
			yaw = MBMath.ClampFloat(yaw, -1.5707964f, 1.5707964f);
			pitch = MBMath.ClampFloat(pitch, -0.35f, 0.85f);
		}

		// Token: 0x0600001C RID: 28 RVA: 0x000036A0 File Offset: 0x000018A0
		private Vec3 SelectBestFireDirection(RangedSiegeWeapon weapon, Vec3 expectedDir)
		{
			bool flag = weapon == null || !weapon.GameEntity.IsValid;
			Vec3 vec;
			if (flag)
			{
				vec = expectedDir;
			}
			else
			{
				Vec3 vec2 = expectedDir;
				try
				{
					vec2 = -weapon.GameEntity.GetGlobalFrame().rotation.f;
				}
				catch
				{
				}
				bool flag2 = vec2.LengthSquared < 1E-06f;
				if (flag2)
				{
					vec2 = expectedDir;
				}
				bool flag3 = vec2.LengthSquared > 1E-06f;
				if (flag3)
				{
					vec2.Normalize();
				}
				Vec3 vec3 = Vec3.Zero;
				bool flag4 = false;
				try
				{
					bool flag5 = SkillSystemBehavior._weaponShootingDirProp == null;
					if (flag5)
					{
						SkillSystemBehavior._weaponShootingDirProp = weapon.GetType().GetProperty("ShootingDirection", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					}
					bool flag6 = SkillSystemBehavior._weaponShootingDirProp != null && SkillSystemBehavior._weaponShootingDirProp.PropertyType == typeof(Vec3);
					if (flag6)
					{
						vec3 = (Vec3)SkillSystemBehavior._weaponShootingDirProp.GetValue(weapon);
						flag4 = vec3.LengthSquared > 1E-06f;
					}
				}
				catch
				{
					flag4 = false;
				}
				Vec3 vec4 = Vec3.Zero;
				bool flag7 = false;
				try
				{
					bool flag8 = SkillSystemBehavior._weaponProjectileProp == null;
					if (flag8)
					{
						SkillSystemBehavior._weaponProjectileProp = typeof(RangedSiegeWeapon).GetProperty("Projectile", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					}
					PropertyInfo weaponProjectileProp = SkillSystemBehavior._weaponProjectileProp;
					SynchedMissionObject synchedMissionObject = ((weaponProjectileProp != null) ? weaponProjectileProp.GetValue(weapon) : null) as SynchedMissionObject;
					bool flag9 = synchedMissionObject != null && synchedMissionObject.GameEntity != null && synchedMissionObject.GameEntity.IsValid;
					if (flag9)
					{
						MatrixFrame globalFrame = synchedMissionObject.GameEntity.GetGlobalFrame();
						vec4 = ((weapon is Ballista) ? globalFrame.rotation.u : globalFrame.rotation.f);
						flag7 = vec4.LengthSquared > 1E-06f;
					}
				}
				catch
				{
					flag7 = false;
				}
				vec2 = SkillSystemBehavior.AlignToExpected(vec2, expectedDir);
				float num = -1f;
				bool flag10 = flag4;
				if (flag10)
				{
					vec3.Normalize();
					vec3 = SkillSystemBehavior.AlignToExpected(vec3, expectedDir);
					num = Vec3.DotProduct(vec3, vec2);
				}
				float num2 = -1f;
				bool flag11 = flag7;
				if (flag11)
				{
					vec4.Normalize();
					vec4 = SkillSystemBehavior.AlignToExpected(vec4, expectedDir);
					num2 = Vec3.DotProduct(vec4, vec2);
				}
				Vec3 vec5 = vec2;
				bool flag12 = num >= num2 && num > 0.85f;
				if (flag12)
				{
					vec5 = vec3;
				}
				else
				{
					bool flag13 = num2 > 0.85f;
					if (flag13)
					{
						vec5 = vec4;
					}
				}
				bool flag14 = vec5.LengthSquared > 1E-06f;
				if (flag14)
				{
					vec5.Normalize();
				}
				vec = vec5;
			}
			return vec;
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00003964 File Offset: 0x00001B64
		private void SimulateTrajectoryCaptain(Vec3 start, Vec3 velocity, float friction, float ignoreTime, bool drawPath, bool useQuadratic, out Vec3 hitPos)
		{
			hitPos = Vec3.Invalid;
			Vec3 vec = start;
			Vec3 vec2;
			vec2..ctor(0f, 0f, -9.806f, -1f);
			float num = 0.02f;
			for (float num2 = 0f; num2 < 20f; num2 += num)
			{
				Vec3 vec3 = vec + velocity * num;
				float length = velocity.Length;
				bool flag = length > 0.001f;
				if (flag)
				{
					if (useQuadratic)
					{
						Vec3 vec4 = velocity.NormalizedCopy() * (friction * length * length * num);
						velocity -= vec4;
					}
					else
					{
						float num3 = 1f - friction * num;
						bool flag2 = num3 < 0f;
						if (flag2)
						{
							num3 = 0f;
						}
						velocity *= num3;
					}
				}
				velocity += vec2 * num;
				bool flag3 = num2 > ignoreTime;
				if (flag3)
				{
					float num4;
					Vec3 vec5;
					WeakGameEntity weakGameEntity;
					bool flag4 = Mission.Current.Scene.RayCastForClosestEntityOrTerrain(vec, vec3, ref num4, ref vec5, ref weakGameEntity, 0.01f, 79617);
					if (flag4)
					{
						hitPos = vec5;
						break;
					}
				}
				bool flag5 = drawPath && num2 > ignoreTime;
				if (flag5)
				{
					SkillSystemBehavior.RenderLine(vec, vec3, uint.MaxValue);
				}
				vec = vec3;
				bool flag6 = vec.z < -100f;
				if (flag6)
				{
					break;
				}
			}
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00003ACC File Offset: 0x00001CCC
		private bool TryGetImmediateBlockHit(Vec3 muzzle, Vec3 dir, float maxDist, out Vec3 hitPos, out float hitDist)
		{
			hitPos = Vec3.Invalid;
			hitDist = 9999f;
			try
			{
				Vec3 vec = muzzle + dir * maxDist;
				float num;
				Vec3 vec2;
				WeakGameEntity weakGameEntity;
				bool flag = Mission.Current.Scene.RayCastForClosestEntityOrTerrain(muzzle, vec, ref num, ref vec2, ref weakGameEntity, 0.01f, 79617);
				if (flag)
				{
					hitPos = vec2;
					hitDist = num;
					return true;
				}
			}
			catch
			{
			}
			return false;
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00003B54 File Offset: 0x00001D54
		private void ForceFireWeapon(RangedSiegeWeapon weapon)
		{
			bool flag = weapon == null;
			if (!flag)
			{
				try
				{
					PropertyInfo property = weapon.GetType().GetProperty("LoadedProjectileItem", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					bool flag2 = property != null && property.GetValue(weapon) != null;
					if (flag2)
					{
						MethodInfo method = weapon.GetType().GetMethod("Shoot", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
						if (method != null)
						{
							method.Invoke(weapon, null);
						}
					}
				}
				catch
				{
				}
			}
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00003BD8 File Offset: 0x00001DD8
		private void OverrideWeaponRotation(RangedSiegeWeapon weapon, Vec3 direction)
		{
			bool flag = weapon == null;
			if (!flag)
			{
				try
				{
					Vec3 vec = direction;
					bool flag2 = vec.LengthSquared < 1E-06f;
					if (!flag2)
					{
						vec.Normalize();
						vec = -vec;
						MatrixFrame globalFrame = weapon.GameEntity.GetGlobalFrame();
						globalFrame.rotation = Mat3.CreateMat3WithForward(ref vec);
						weapon.GameEntity.SetGlobalFrame(ref globalFrame, true);
					}
				}
				catch
				{
				}
			}
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00003C60 File Offset: 0x00001E60
		private Vec3 GetWeaponRealShootDirection(RangedSiegeWeapon weapon, Vec3 expected)
		{
			bool flag = weapon == null || !weapon.GameEntity.IsValid;
			Vec3 vec;
			if (flag)
			{
				vec = expected;
			}
			else
			{
				try
				{
					bool flag2 = SkillSystemBehavior._weaponProjectileProp == null;
					if (flag2)
					{
						SkillSystemBehavior._weaponProjectileProp = typeof(RangedSiegeWeapon).GetProperty("Projectile", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					}
					PropertyInfo weaponProjectileProp = SkillSystemBehavior._weaponProjectileProp;
					SynchedMissionObject synchedMissionObject = ((weaponProjectileProp != null) ? weaponProjectileProp.GetValue(weapon) : null) as SynchedMissionObject;
					bool flag3 = synchedMissionObject != null && synchedMissionObject.GameEntity != null && synchedMissionObject.GameEntity.IsValid;
					if (flag3)
					{
						MatrixFrame globalFrame = synchedMissionObject.GameEntity.GetGlobalFrame();
						Vec3 vec2 = ((weapon is Ballista) ? globalFrame.rotation.u : globalFrame.rotation.f);
						bool flag4 = vec2.LengthSquared > 1E-06f;
						if (flag4)
						{
							return SkillSystemBehavior.AlignToExpected(vec2, expected);
						}
					}
				}
				catch
				{
				}
				try
				{
					bool flag5 = SkillSystemBehavior._weaponShootingDirProp == null;
					if (flag5)
					{
						SkillSystemBehavior._weaponShootingDirProp = weapon.GetType().GetProperty("ShootingDirection", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					}
					bool flag6 = SkillSystemBehavior._weaponShootingDirProp != null && SkillSystemBehavior._weaponShootingDirProp.PropertyType == typeof(Vec3);
					if (flag6)
					{
						Vec3 vec3 = (Vec3)SkillSystemBehavior._weaponShootingDirProp.GetValue(weapon);
						bool flag7 = vec3.LengthSquared > 1E-06f;
						if (flag7)
						{
							Vec3 vec4 = vec3.NormalizedCopy();
							Vec3 vec5 = ((expected.LengthSquared > 1E-06f) ? expected.NormalizedCopy() : expected);
							bool flag8 = MathF.Abs(vec4.z) >= 0.002f || MathF.Abs(vec5.z) <= 0.02f;
							if (flag8)
							{
								return SkillSystemBehavior.AlignToExpected(vec3, expected);
							}
						}
					}
				}
				catch
				{
				}
				try
				{
					Vec3 vec6 = -weapon.GameEntity.GetGlobalFrame().rotation.f;
					bool flag9 = vec6.LengthSquared > 1E-06f;
					if (flag9)
					{
						return SkillSystemBehavior.AlignToExpected(vec6, expected);
					}
				}
				catch
				{
				}
				vec = expected;
			}
			return vec;
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00003EC4 File Offset: 0x000020C4
		private static Vec3 AlignToExpected(Vec3 candidate, Vec3 expected)
		{
			Vec3 vec = candidate;
			bool flag = vec.LengthSquared > 1E-06f;
			if (flag)
			{
				vec.Normalize();
			}
			Vec3 vec2 = expected;
			bool flag2 = vec2.LengthSquared > 1E-06f;
			if (flag2)
			{
				vec2.Normalize();
			}
			bool flag3 = Vec3.DotProduct(vec, vec2) < 0f;
			if (flag3)
			{
				vec = -vec;
			}
			return vec;
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00003F2C File Offset: 0x0000212C
		private static float GetShootingSpeed(RangedSiegeWeapon w)
		{
			try
			{
				PropertyInfo property = w.GetType().GetProperty("ShootingSpeed", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				bool flag = property != null;
				if (flag)
				{
					object value = property.GetValue(w);
					float num;
					bool flag2;
					if (value is float)
					{
						num = (float)value;
						flag2 = true;
					}
					else
					{
						flag2 = false;
					}
					bool flag3 = flag2;
					if (flag3)
					{
						return num;
					}
				}
			}
			catch
			{
			}
			return 150f;
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00003FA4 File Offset: 0x000021A4
		private static float GetDynamicFriction(RangedSiegeWeapon weapon, ItemObject ammo)
		{
			try
			{
				bool flag = ammo != null && ammo.PrimaryWeapon != null;
				if (flag)
				{
					return SkillSystemBehavior.GetAirFriction(ammo.PrimaryWeapon.WeaponClass, ammo.PrimaryWeapon.WeaponFlags);
				}
			}
			catch
			{
			}
			return 5E-05f;
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00004008 File Offset: 0x00002208
		private static float GetAirFriction(WeaponClass wc, WeaponFlags flags)
		{
			bool flag = SkillSystemBehavior._airFrictionMethod == null;
			if (flag)
			{
				SkillSystemBehavior._airFrictionMethod = typeof(ItemObject).GetMethod("GetAirFrictionConstant", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			}
			bool flag2 = SkillSystemBehavior._airFrictionMethod != null;
			if (flag2)
			{
				try
				{
					object obj = SkillSystemBehavior._airFrictionMethod.Invoke(null, new object[] { wc, flags });
					float num;
					bool flag3;
					if (obj is float)
					{
						num = (float)obj;
						flag3 = true;
					}
					else
					{
						flag3 = false;
					}
					bool flag4 = flag3;
					if (flag4)
					{
						return num;
					}
				}
				catch
				{
				}
			}
			return 5E-05f;
		}

		// Token: 0x06000026 RID: 38 RVA: 0x000040BC File Offset: 0x000022BC
		private static ItemObject GetAmmoItem(RangedSiegeWeapon weapon)
		{
			bool flag = weapon == null;
			ItemObject itemObject;
			if (flag)
			{
				itemObject = null;
			}
			else
			{
				try
				{
					PropertyInfo property = weapon.GetType().GetProperty("LoadedProjectileItem", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					bool flag2 = property != null;
					if (flag2)
					{
						return property.GetValue(weapon) as ItemObject;
					}
					PropertyInfo property2 = weapon.GetType().GetProperty("OriginalAmmoItem", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					bool flag3 = property2 != null;
					if (flag3)
					{
						return property2.GetValue(weapon) as ItemObject;
					}
					FieldInfo field = weapon.GetType().GetField("_originalAmmoItem", BindingFlags.Instance | BindingFlags.NonPublic);
					bool flag4 = field != null;
					if (flag4)
					{
						return field.GetValue(weapon) as ItemObject;
					}
				}
				catch
				{
				}
				itemObject = null;
			}
			return itemObject;
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00004188 File Offset: 0x00002388
		private static float GetAmmoMissileSpeed(ItemObject ammo)
		{
			bool flag = ammo == null;
			float num;
			if (flag)
			{
				num = -1f;
			}
			else
			{
				try
				{
					object primaryWeapon = ammo.PrimaryWeapon;
					bool flag2 = primaryWeapon == null;
					if (flag2)
					{
						return -1f;
					}
					PropertyInfo property = primaryWeapon.GetType().GetProperty("MissileSpeed", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					bool flag3 = property != null && property.PropertyType == typeof(float);
					if (flag3)
					{
						return (float)property.GetValue(primaryWeapon);
					}
				}
				catch
				{
				}
				num = -1f;
			}
			return num;
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00004230 File Offset: 0x00002430
		private void InitCaptainAimFromCurrentWeapon(Agent player, RangedSiegeWeapon weapon)
		{
			try
			{
				MatrixFrame matrixFrame;
				if (!this._shipRoot.IsValid)
				{
					Vec3 position = player.Position;
					Vec3 vec = player.Position + player.LookDirection;
					matrixFrame = MatrixFrame.CreateLookAt(ref position, ref vec, ref Vec3.Up);
				}
				else
				{
					matrixFrame = this._shipRoot.GetGlobalFrame();
				}
				MatrixFrame matrixFrame2 = matrixFrame;
				Mat3 mat = SkillSystemBehavior.CreateFlatYawBasis(matrixFrame2.rotation.f);
				Vec3 vec2 = this.SelectBestFireDirection(weapon, player.LookDirection);
				float rotationInRadians = mat.f.AsVec2.RotationInRadians;
				Vec3 vec3 = vec2;
				vec3.z = 0f;
				bool flag = vec3.LengthSquared < 1E-05f;
				if (flag)
				{
					vec3 = mat.f;
				}
				vec3.Normalize();
				float rotationInRadians2 = vec3.AsVec2.RotationInRadians;
				float num = MBMath.WrapAngle(rotationInRadians2 - rotationInRadians);
				num = MBMath.ClampFloat(num, -1.5707964f, 1.5707964f);
				float length = vec2.AsVec2.Length;
				float num2 = ((length > 0.0001f) ? MathF.Atan2(vec2.z, length) : 0f);
				num2 = MBMath.ClampFloat(num2, -0.35f, 0.85f);
				this._aimYaw = num;
				this._aimPitch = num2;
			}
			catch
			{
				this._aimYaw = 0f;
				this._aimPitch = 0f;
			}
		}

		// Token: 0x06000029 RID: 41 RVA: 0x000043AC File Offset: 0x000025AC
		private static bool IsOperatingSiegeWeapon(WeakGameEntity usedEntity)
		{
			WeakGameEntity weakGameEntity = usedEntity;
			while (weakGameEntity.IsValid)
			{
				try
				{
					bool flag = weakGameEntity.GetFirstScriptOfType<RangedSiegeWeapon>() != null;
					if (flag)
					{
						return true;
					}
				}
				catch
				{
				}
				bool flag2 = !weakGameEntity.Parent.IsValid;
				if (flag2)
				{
					break;
				}
				weakGameEntity = weakGameEntity.Parent;
				continue;
			}
			return false;
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00004420 File Offset: 0x00002620
		private RangedSiegeWeapon FindFirstRangedSiegeWeapon(WeakGameEntity parent)
		{
			bool flag = !parent.IsValid;
			RangedSiegeWeapon rangedSiegeWeapon;
			if (flag)
			{
				rangedSiegeWeapon = null;
			}
			else
			{
				RangedSiegeWeapon firstScriptOfType = parent.GetFirstScriptOfType<RangedSiegeWeapon>();
				bool flag2 = firstScriptOfType != null;
				if (flag2)
				{
					rangedSiegeWeapon = firstScriptOfType;
				}
				else
				{
					foreach (WeakGameEntity weakGameEntity in parent.GetChildren())
					{
						RangedSiegeWeapon rangedSiegeWeapon2 = this.FindFirstRangedSiegeWeapon(weakGameEntity);
						bool flag3 = rangedSiegeWeapon2 != null;
						if (flag3)
						{
							return rangedSiegeWeapon2;
						}
					}
					rangedSiegeWeapon = null;
				}
			}
			return rangedSiegeWeapon;
		}

		// Token: 0x0600002B RID: 43 RVA: 0x000044B8 File Offset: 0x000026B8
		private static bool IsLobber(RangedSiegeWeapon w)
		{
			bool flag = w == null;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				string text = w.GetType().Name.ToLower();
				string text2 = w.GameEntity.Name.ToLower();
				flag2 = text.Contains("mangonel") || text.Contains("trebuchet") || text.Contains("onager") || text2.Contains("mangonel") || text2.Contains("trebuchet") || text2.Contains("onager");
			}
			return flag2;
		}

		// Token: 0x0600002C RID: 44 RVA: 0x0000454C File Offset: 0x0000274C
		private void CleanDeadAgentEntries()
		{
			bool flag = SkillSystemBehavior.WoW_AgentMissileSpeedData.Count == 0;
			if (!flag)
			{
				List<int> list = SkillSystemBehavior.WoW_AgentMissileSpeedData.Keys.ToList<int>();
				foreach (int num in list)
				{
					List<SkillSystemBehavior.AgentMissileSpeedData> list2;
					bool flag2 = SkillSystemBehavior.WoW_AgentMissileSpeedData.TryGetValue(num, out list2);
					if (flag2)
					{
						list2.RemoveAll((SkillSystemBehavior.AgentMissileSpeedData x) => x.Agent == null || !x.Agent.IsActive());
						bool flag3 = list2.Count == 0;
						if (flag3)
						{
							SkillSystemBehavior.WoW_AgentMissileSpeedData.Remove(num);
						}
					}
				}
			}
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00004618 File Offset: 0x00002818
		private static Mat3 CreateFlatYawBasis(Vec3 forward)
		{
			Vec3 vec = forward;
			vec.z = 0f;
			bool flag = vec.LengthSquared < 1E-05f;
			if (flag)
			{
				vec = Vec3.Forward;
			}
			vec.Normalize();
			Vec3 vec2 = Vec3.Up;
			Vec3 vec3 = Vec3.CrossProduct(vec2, vec);
			bool flag2 = vec3.LengthSquared < 1E-05f;
			if (flag2)
			{
				vec3..ctor(1f, 0f, 0f, -1f);
			}
			vec3.Normalize();
			vec2 = Vec3.CrossProduct(vec, vec3);
			bool flag3 = vec2.LengthSquared > 1E-05f;
			if (flag3)
			{
				vec2.Normalize();
			}
			else
			{
				vec2 = Vec3.Up;
			}
			return new Mat3(ref vec3, ref vec, ref vec2);
		}

		// Token: 0x04000004 RID: 4
		private const float CameraBaseHeight = 32f;

		// Token: 0x04000005 RID: 5
		private const float CameraBaseDistance = 18f;

		// Token: 0x04000006 RID: 6
		private const float MouseSensitivity = 0.003f;

		// Token: 0x04000007 RID: 7
		private const float CaptainCamUp = 78f;

		// Token: 0x04000008 RID: 8
		private const float CaptainCamBack = 38f;

		// Token: 0x04000009 RID: 9
		private const float CaptainBaseSide = 1.57f;

		// Token: 0x0400000A RID: 10
		private const float CaptainPitchDown = -0.28f;

		// Token: 0x0400000B RID: 11
		private const float CaptainCamSmoothStrength = 10f;

		// Token: 0x0400000C RID: 12
		private const float CaptainMouseSensitivity = 0.001f;

		// Token: 0x0400000D RID: 13
		private const float AimYawClamp = 1.5707964f;

		// Token: 0x0400000E RID: 14
		private const float AimPitchMin = -0.35f;

		// Token: 0x0400000F RID: 15
		private const float AimPitchMax = 0.85f;

		// Token: 0x04000010 RID: 16
		private const float SimMaxTime = 20f;

		// Token: 0x04000011 RID: 17
		private const float SimStep = 0.02f;

		// Token: 0x04000012 RID: 18
		private const uint ColorFlightLine = 4294967295U;

		// Token: 0x04000013 RID: 19
		private const uint ColorImpact = 4294901760U;

		// Token: 0x04000014 RID: 20
		private const float MuzzleForwardOffset = 0.25f;

		// Token: 0x04000015 RID: 21
		private const float NearBlockCheckDist = 3f;

		// Token: 0x04000016 RID: 22
		private const float NearBlockTriggerDist = 2f;

		// Token: 0x04000017 RID: 23
		private const float FireDirSmoothStrength = 18f;

		// Token: 0x04000018 RID: 24
		private const float ShotFreezeDuration = 0.28f;

		// Token: 0x04000019 RID: 25
		private const float ShipVelDeadzone = 0.25f;

		// Token: 0x0400001A RID: 26
		private static MethodInfo _renderLineMethod;

		// Token: 0x0400001B RID: 27
		private static MethodInfo _renderSphereMethod;

		// Token: 0x0400001C RID: 28
		private static object _debugInterface;

		// Token: 0x0400001D RID: 29
		public static Dictionary<int, List<SkillSystemBehavior.AgentMissileSpeedData>> WoW_AgentMissileSpeedData = new Dictionary<int, List<SkillSystemBehavior.AgentMissileSpeedData>>();

		// Token: 0x0400001E RID: 30
		private float _cleanupTimer = 0f;

		// Token: 0x0400001F RID: 31
		private const float CleanupInterval = 5f;

		// Token: 0x04000020 RID: 32
		private bool _isRtsModeEnabled = false;

		// Token: 0x04000021 RID: 33
		private RangedSiegeWeapon _currentSiegeWeapon = null;

		// Token: 0x04000022 RID: 34
		private Camera _customCamera;

		// Token: 0x04000023 RID: 35
		private float _camYawOffset = 0f;

		// Token: 0x04000024 RID: 36
		private float _camPitchOffset = 0f;

		// Token: 0x04000025 RID: 37
		private bool _isCaptainModeActive = false;

		// Token: 0x04000026 RID: 38
		private RangedSiegeWeapon _captainControlWeapon = null;

		// Token: 0x04000027 RID: 39
		private WeakGameEntity _shipRoot;

		// Token: 0x04000028 RID: 40
		private float _aimYaw = 0f;

		// Token: 0x04000029 RID: 41
		private float _aimPitch = 0f;

		// Token: 0x0400002A RID: 42
		private bool _lmbWasDown = false;

		// Token: 0x0400002B RID: 43
		private bool _captainCamSmoothInited = false;

		// Token: 0x0400002C RID: 44
		private Vec3 _captainCamSmoothPos = Vec3.Zero;

		// Token: 0x0400002D RID: 45
		private Vec3 _captainCamSmoothForward = Vec3.Forward;

		// Token: 0x0400002E RID: 46
		private bool _shipVelInited = false;

		// Token: 0x0400002F RID: 47
		private Vec3 _lastShipPos = Vec3.Zero;

		// Token: 0x04000030 RID: 48
		private Vec3 _shipVel = Vec3.Zero;

		// Token: 0x04000031 RID: 49
		private bool _fireDirSmoothInited = false;

		// Token: 0x04000032 RID: 50
		private Vec3 _fireDirSmooth = Vec3.Forward;

		// Token: 0x04000033 RID: 51
		private float _shotFreezeTimer = 0f;

		// Token: 0x04000034 RID: 52
		private Vec3 _freezeMuzzle = Vec3.Invalid;

		// Token: 0x04000035 RID: 53
		private Vec3 _freezeFireDir = Vec3.Forward;

		// Token: 0x04000036 RID: 54
		private Vec3 _freezeStart = Vec3.Zero;

		// Token: 0x04000037 RID: 55
		private Vec3 _freezeVelocity = Vec3.Zero;

		// Token: 0x04000038 RID: 56
		private float _freezeFriction = 0f;

		// Token: 0x04000039 RID: 57
		private bool _freezeUseQuadratic = false;

		// Token: 0x0400003A RID: 58
		private bool _freezeDrawPath = false;

		// Token: 0x0400003B RID: 59
		private float _freezeIgnoreTime = 0.02f;

		// Token: 0x0400003C RID: 60
		private bool _muzzleSmoothInited = false;

		// Token: 0x0400003D RID: 61
		private Vec3 _muzzleSmoothPos = Vec3.Zero;

		// Token: 0x0400003E RID: 62
		private const float MuzzleSmoothStrength = 35f;

		// Token: 0x0400003F RID: 63
		private const float MuzzleSnapDist = 2f;

		// Token: 0x04000040 RID: 64
		private static MethodInfo _airFrictionMethod;

		// Token: 0x04000041 RID: 65
		private static PropertyInfo _weaponProjectileProp;

		// Token: 0x04000042 RID: 66
		private static PropertyInfo _weaponShootingDirProp;

		// Token: 0x02000011 RID: 17
		[Nullable(0)]
		public class AgentMissileSpeedData
		{
			// Token: 0x1700000E RID: 14
			// (get) Token: 0x0600011D RID: 285 RVA: 0x00009AF4 File Offset: 0x00007CF4
			// (set) Token: 0x0600011E RID: 286 RVA: 0x00009AFC File Offset: 0x00007CFC
			public MissionWeapon Weapon { get; set; }

			// Token: 0x1700000F RID: 15
			// (get) Token: 0x0600011F RID: 287 RVA: 0x00009B05 File Offset: 0x00007D05
			// (set) Token: 0x06000120 RID: 288 RVA: 0x00009B0D File Offset: 0x00007D0D
			public float MissileSpeed { get; set; }

			// Token: 0x06000121 RID: 289 RVA: 0x00009B16 File Offset: 0x00007D16
			public AgentMissileSpeedData(MissionWeapon weapon, float missileSpeen, Agent agent)
			{
				this.Weapon = weapon;
				this.MissileSpeed = missileSpeen;
				this.Agent = agent;
			}

			// Token: 0x0400005E RID: 94
			public Agent Agent;
		}
	}
}
