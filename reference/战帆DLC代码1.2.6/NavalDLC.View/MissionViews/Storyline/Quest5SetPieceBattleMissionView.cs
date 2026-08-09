using System;
using NavalDLC.Storyline.MissionControllers;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace NavalDLC.View.MissionViews.Storyline
{
	// Token: 0x0200002C RID: 44
	public class Quest5SetPieceBattleMissionView : MissionView
	{
		// Token: 0x0600011A RID: 282 RVA: 0x000083CC File Offset: 0x000065CC
		public Quest5SetPieceBattleMissionView()
		{
			this._state = Quest5SetPieceBattleMissionView.Quest5SetPieceBattleMissionViewState.None;
			this._approachPlayerShipLocationCheckState = Quest5SetPieceBattleMissionView.ApproachPlayerShipLocationCheckState.None;
			this._allowedSwimRadiusCheckState = Quest5SetPieceBattleMissionView.AllowedSwimRadiusCheckState.None;
			this._escapeShipStuckCheckState = Quest5SetPieceBattleMissionView.EscapeShipStuckCheckState.None;
			this._purigCutsceneCameraChangeState = Quest5SetPieceBattleMissionView.PurigCutsceneCameraChangeState.None;
			this._purigCutsceneCameraChangeTimer = null;
		}

		// Token: 0x0600011B RID: 283 RVA: 0x0000841A File Offset: 0x0000661A
		public virtual void PassMissionStateOnTick(Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState currentState)
		{
		}

		// Token: 0x0600011C RID: 284 RVA: 0x0000841C File Offset: 0x0000661C
		protected virtual void SetPlayerMovementEnabled(bool isPlayerMovementEnabled)
		{
		}

		// Token: 0x0600011D RID: 285 RVA: 0x0000841E File Offset: 0x0000661E
		public override void AfterStart()
		{
			base.AfterStart();
			this._quest5SetPieceBattleMissionController = base.Mission.GetMissionBehavior<Quest5SetPieceBattleMissionController>();
			this.LastHitCheckpoint = this._quest5SetPieceBattleMissionController.LastHitCheckpoint;
		}

		// Token: 0x0600011E RID: 286 RVA: 0x00008448 File Offset: 0x00006648
		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			this.PassMissionStateOnTick(this._quest5SetPieceBattleMissionController.State);
			this.HandleAllowedSwimRadiusCheck();
			this.HandleApproachPlayerShipLocationCheck();
			this.HandleEscapeShipStuckCheck();
			this.HandlePurigCutsceneCameraChange();
			if (!this._isPlayerShipRotationCorrectedAtTheStartOfTheMission && this._quest5SetPieceBattleMissionController.State == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1GoToEnemyShip)
			{
				this.ChangeMainAgentRotation(this._quest5SetPieceBattleMissionController.CalculateMissionStartDirection());
				this._isPlayerShipRotationCorrectedAtTheStartOfTheMission = true;
			}
			if (this._state == Quest5SetPieceBattleMissionView.Quest5SetPieceBattleMissionViewState.None && (this._quest5SetPieceBattleMissionController.State == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1GoToShipInteriorFadeOut || this._quest5SetPieceBattleMissionController.State == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1GoBackToShipFadeOut || this._quest5SetPieceBattleMissionController.State == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1ToPhase2FadeOut || this._quest5SetPieceBattleMissionController.State == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase2ToPhase3FadeOut || this._quest5SetPieceBattleMissionController.State == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase3ToPhase4FadeOut || this._quest5SetPieceBattleMissionController.State == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase4ToBossFightFadeOut))
			{
				this._state = Quest5SetPieceBattleMissionView.Quest5SetPieceBattleMissionViewState.FadeOut;
				this.SetPlayerMovementEnabled(false);
			}
			Quest5SetPieceBattleMissionController quest5SetPieceBattleMissionController = this._quest5SetPieceBattleMissionController;
			if (quest5SetPieceBattleMissionController != null && quest5SetPieceBattleMissionController.State == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.End)
			{
				if (this._missionEndTimer == null)
				{
					this._missionEndTimer = new MissionTimer(1.75f);
				}
				else
				{
					this._missionEndTimer.Check(false);
				}
			}
			switch (this._state)
			{
			case Quest5SetPieceBattleMissionView.Quest5SetPieceBattleMissionViewState.None:
				break;
			case Quest5SetPieceBattleMissionView.Quest5SetPieceBattleMissionViewState.FadeOut:
				ScreenFadeController.BeginFadeOutAndIn(1f, 1f, 1f);
				this._state = Quest5SetPieceBattleMissionView.Quest5SetPieceBattleMissionViewState.Initialize;
				return;
			case Quest5SetPieceBattleMissionView.Quest5SetPieceBattleMissionViewState.Initialize:
				if (ScreenFadeController.IsFadedOut)
				{
					if (this._quest5SetPieceBattleMissionController.State == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1GoToShipInteriorFadeOut)
					{
						this._quest5SetPieceBattleMissionController.TriggerPhase1InitializeShipInteriorPhase();
						return;
					}
					if (this._quest5SetPieceBattleMissionController.State == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1GoBackToShipFadeOut)
					{
						this._quest5SetPieceBattleMissionController.TriggerPhase1InitializeGoBackToShipPhase();
						return;
					}
					if (this._quest5SetPieceBattleMissionController.State == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1ToPhase2FadeOut)
					{
						this._quest5SetPieceBattleMissionController.TriggerInitializePhase2();
						return;
					}
					if (this._quest5SetPieceBattleMissionController.State == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase2ToPhase3FadeOut)
					{
						this._quest5SetPieceBattleMissionController.TriggerInitializePhase3();
						return;
					}
					if (this._quest5SetPieceBattleMissionController.State == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase3ToPhase4FadeOut)
					{
						this._quest5SetPieceBattleMissionController.TriggerInitializePhase4();
						return;
					}
					if (this._quest5SetPieceBattleMissionController.State == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase4ToBossFightFadeOut)
					{
						this._quest5SetPieceBattleMissionController.TriggerInitializeBossFight();
						return;
					}
					if (this._quest5SetPieceBattleMissionController.State == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1GoToShipInteriorFadeIn || this._quest5SetPieceBattleMissionController.State == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1GoBackToShipFadeIn || this._quest5SetPieceBattleMissionController.State == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1ToPhase2FadeIn || this._quest5SetPieceBattleMissionController.State == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase2ToPhase3FadeIn || this._quest5SetPieceBattleMissionController.State == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase3ToPhase4FadeIn || this._quest5SetPieceBattleMissionController.State == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase4ToBossFightFadeIn)
					{
						if (this._quest5SetPieceBattleMissionController.State == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1GoToShipInteriorFadeIn)
						{
							Vec3 vec;
							this._quest5SetPieceBattleMissionController.GetIntendedMainAgentDirectionForPhase1InteriorTeleport(out vec);
							this.ChangeMainAgentRotation(vec);
						}
						else if (this._quest5SetPieceBattleMissionController.State == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1GoBackToShipFadeIn)
						{
							Vec3 vec2;
							this._quest5SetPieceBattleMissionController.GetIntendedMainAgentDirectionForPhase1EscapeShipTeleport(out vec2);
							this.ChangeMainAgentRotation(vec2);
						}
						else if (this._quest5SetPieceBattleMissionController.State == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase3ToPhase4FadeIn)
						{
							this._purigCutsceneCameraChangeState = Quest5SetPieceBattleMissionView.PurigCutsceneCameraChangeState.WaitingForCountDown;
						}
						this._state = Quest5SetPieceBattleMissionView.Quest5SetPieceBattleMissionViewState.FadeIn;
						return;
					}
				}
				break;
			case Quest5SetPieceBattleMissionView.Quest5SetPieceBattleMissionViewState.FadeIn:
				if (!this._isMainAgentRotatedBeforeBossFight && this._quest5SetPieceBattleMissionController.State == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase4ToBossFightFadeIn)
				{
					this._isMainAgentRotatedBeforeBossFight = true;
					Vec3 vec3;
					this._quest5SetPieceBattleMissionController.GetIntendedMainAgentDirectionForBossFight(out vec3);
					this.ChangeMainAgentRotation(vec3);
				}
				if (!ScreenFadeController.IsFadeActive)
				{
					if (this._quest5SetPieceBattleMissionController.State == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1GoToShipInteriorFadeIn)
					{
						this._quest5SetPieceBattleMissionController.CompletePhase1GoToShipInteriorTransition();
					}
					else if (this._quest5SetPieceBattleMissionController.State == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1GoBackToShipFadeIn)
					{
						this._quest5SetPieceBattleMissionController.CompletePhase1InitializeGoBackToShipTransition();
					}
					else if (this._quest5SetPieceBattleMissionController.State == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1ToPhase2FadeIn)
					{
						this._quest5SetPieceBattleMissionController.CompletePhase1ToPhase2Transition();
					}
					else if (this._quest5SetPieceBattleMissionController.State == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase2ToPhase3FadeIn)
					{
						this._quest5SetPieceBattleMissionController.CompletePhase2ToPhase3Transition();
					}
					else if (this._quest5SetPieceBattleMissionController.State == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase3ToPhase4FadeIn)
					{
						this._quest5SetPieceBattleMissionController.CompletePhase3ToPhase4Transition();
					}
					else if (this._quest5SetPieceBattleMissionController.State == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase4ToBossFightFadeIn)
					{
						this._quest5SetPieceBattleMissionController.CompletePhase4ToBossFightTransition();
					}
					this.SetPlayerMovementEnabled(true);
					this._state = Quest5SetPieceBattleMissionView.Quest5SetPieceBattleMissionViewState.End;
					return;
				}
				break;
			case Quest5SetPieceBattleMissionView.Quest5SetPieceBattleMissionViewState.End:
				this._state = Quest5SetPieceBattleMissionView.Quest5SetPieceBattleMissionViewState.None;
				break;
			default:
				return;
			}
		}

		// Token: 0x0600011F RID: 287 RVA: 0x0000880C File Offset: 0x00006A0C
		private void HandleAllowedSwimRadiusCheck()
		{
			if (this._allowedSwimRadiusCheckState != Quest5SetPieceBattleMissionView.AllowedSwimRadiusCheckState.End)
			{
				switch (this._allowedSwimRadiusCheckState)
				{
				case Quest5SetPieceBattleMissionView.AllowedSwimRadiusCheckState.None:
				{
					Quest5SetPieceBattleMissionController quest5SetPieceBattleMissionController = this._quest5SetPieceBattleMissionController;
					if (quest5SetPieceBattleMissionController != null && quest5SetPieceBattleMissionController.State == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1GoToEnemyShip)
					{
						this._allowedSwimRadiusCheckState = Quest5SetPieceBattleMissionView.AllowedSwimRadiusCheckState.CheckDistance;
						return;
					}
					break;
				}
				case Quest5SetPieceBattleMissionView.AllowedSwimRadiusCheckState.CheckDistance:
					if (this._quest5SetPieceBattleMissionController.State >= Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase2Part1)
					{
						this._allowedSwimRadiusCheckState = Quest5SetPieceBattleMissionView.AllowedSwimRadiusCheckState.End;
						return;
					}
					if (this._quest5SetPieceBattleMissionController.ShouldTeleportPlayerBetweenTargetPositionAndHidingSpot())
					{
						this._allowedSwimRadiusCheckState = Quest5SetPieceBattleMissionView.AllowedSwimRadiusCheckState.FadeOut;
						return;
					}
					break;
				case Quest5SetPieceBattleMissionView.AllowedSwimRadiusCheckState.FadeOut:
					ScreenFadeController.BeginFadeOutAndIn(0.25f, 0.25f, 0.25f);
					this._allowedSwimRadiusCheckState = Quest5SetPieceBattleMissionView.AllowedSwimRadiusCheckState.TeleportPlayer;
					this.SetPlayerMovementEnabled(false);
					return;
				case Quest5SetPieceBattleMissionView.AllowedSwimRadiusCheckState.TeleportPlayer:
					if (ScreenFadeController.IsFadedOut)
					{
						Vec3 vec;
						this._quest5SetPieceBattleMissionController.TeleportPlayerBetweenTargetPositionAndHidingSpot(out vec);
						this.ChangeMainAgentRotation(vec);
						MBInformationManager.AddQuickInformation(this._restrictionNotificationText, 0, null, null, "");
						this._allowedSwimRadiusCheckState = Quest5SetPieceBattleMissionView.AllowedSwimRadiusCheckState.CheckDistance;
						this.SetPlayerMovementEnabled(true);
					}
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x06000120 RID: 288 RVA: 0x000088F0 File Offset: 0x00006AF0
		private void HandleApproachPlayerShipLocationCheck()
		{
			if (this._approachPlayerShipLocationCheckState != Quest5SetPieceBattleMissionView.ApproachPlayerShipLocationCheckState.End)
			{
				switch (this._approachPlayerShipLocationCheckState)
				{
				case Quest5SetPieceBattleMissionView.ApproachPlayerShipLocationCheckState.None:
				{
					Quest5SetPieceBattleMissionController quest5SetPieceBattleMissionController = this._quest5SetPieceBattleMissionController;
					if (quest5SetPieceBattleMissionController != null && quest5SetPieceBattleMissionController.State == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1GoToEnemyShip)
					{
						this._approachPlayerShipLocationCheckState = Quest5SetPieceBattleMissionView.ApproachPlayerShipLocationCheckState.CheckDistance;
						return;
					}
					break;
				}
				case Quest5SetPieceBattleMissionView.ApproachPlayerShipLocationCheckState.CheckDistance:
					if (this._quest5SetPieceBattleMissionController.State != Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1GoToEnemyShip)
					{
						this._approachPlayerShipLocationCheckState = Quest5SetPieceBattleMissionView.ApproachPlayerShipLocationCheckState.End;
						return;
					}
					if (this._quest5SetPieceBattleMissionController.ShouldTeleportPlayerShipToStartingPosition())
					{
						this._approachPlayerShipLocationCheckState = Quest5SetPieceBattleMissionView.ApproachPlayerShipLocationCheckState.FadeOut;
						return;
					}
					break;
				case Quest5SetPieceBattleMissionView.ApproachPlayerShipLocationCheckState.FadeOut:
					ScreenFadeController.BeginFadeOutAndIn(0.25f, 0.25f, 0.25f);
					this._approachPlayerShipLocationCheckState = Quest5SetPieceBattleMissionView.ApproachPlayerShipLocationCheckState.TeleportPlayerShip;
					this.SetPlayerMovementEnabled(false);
					return;
				case Quest5SetPieceBattleMissionView.ApproachPlayerShipLocationCheckState.TeleportPlayerShip:
					if (ScreenFadeController.IsFadedOut)
					{
						Vec3 vec;
						this._quest5SetPieceBattleMissionController.TeleportPlayerShipToStartingPosition(out vec);
						this.ChangeMainAgentRotation(vec);
						MBInformationManager.AddQuickInformation(this._restrictionNotificationText, 0, null, null, "");
						this._approachPlayerShipLocationCheckState = Quest5SetPieceBattleMissionView.ApproachPlayerShipLocationCheckState.CheckDistance;
						this.SetPlayerMovementEnabled(true);
					}
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x06000121 RID: 289 RVA: 0x000089D4 File Offset: 0x00006BD4
		private void HandlePurigCutsceneCameraChange()
		{
			switch (this._purigCutsceneCameraChangeState)
			{
			case Quest5SetPieceBattleMissionView.PurigCutsceneCameraChangeState.WaitingForCountDown:
			{
				if (this._purigCutsceneCameraChangeTimer == null)
				{
					this.InitializePurigShipCutsceneCamera();
					this._quest5SetPieceBattleMissionController.OnPurigCutsceneStarted();
					return;
				}
				if (this._purigCutsceneCameraChangeTimer != null && this._purigCutsceneCameraChangeTimer.Check(false))
				{
					this._purigCutsceneCameraChangeTimer = null;
					this._purigCutsceneCameraChangeState = Quest5SetPieceBattleMissionView.PurigCutsceneCameraChangeState.FadeOut;
					this.SetPlayerMovementEnabled(false);
				}
				MatrixFrame matrixFrame;
				this.GetCameraFrame(out matrixFrame);
				this.PurigShipCutsceneCamera.Frame = matrixFrame;
				base.MissionScreen.CustomCamera = this.PurigShipCutsceneCamera;
				return;
			}
			case Quest5SetPieceBattleMissionView.PurigCutsceneCameraChangeState.FadeOut:
			{
				ScreenFadeController.BeginFadeOutAndIn(0.5f, 0.5f, 0.5f);
				this._purigCutsceneCameraChangeState = Quest5SetPieceBattleMissionView.PurigCutsceneCameraChangeState.ChangeBackToDefaultCamera;
				MatrixFrame matrixFrame2;
				this.GetCameraFrame(out matrixFrame2);
				this.PurigShipCutsceneCamera.Frame = matrixFrame2;
				base.MissionScreen.CustomCamera = this.PurigShipCutsceneCamera;
				return;
			}
			case Quest5SetPieceBattleMissionView.PurigCutsceneCameraChangeState.ChangeBackToDefaultCamera:
			{
				if (ScreenFadeController.IsFadedOut)
				{
					base.MissionScreen.CustomCamera = null;
					this._purigCutsceneCameraChangeState = Quest5SetPieceBattleMissionView.PurigCutsceneCameraChangeState.End;
					this._quest5SetPieceBattleMissionController.OnPurigShipCutsceneEnded();
					this.SetPlayerMovementEnabled(true);
					return;
				}
				MatrixFrame matrixFrame3;
				this.GetCameraFrame(out matrixFrame3);
				this.PurigShipCutsceneCamera.Frame = matrixFrame3;
				base.MissionScreen.CustomCamera = this.PurigShipCutsceneCamera;
				return;
			}
			default:
				return;
			}
		}

		// Token: 0x06000122 RID: 290 RVA: 0x00008AFC File Offset: 0x00006CFC
		private void InitializePurigShipCutsceneCamera()
		{
			GameEntity gameEntity = Mission.Current.Scene.FindEntityWithTag("purig_ship_cutscene_cam_tag");
			if (gameEntity != null && this._quest5SetPieceBattleMissionController.Phase4PurigShip != null)
			{
				Vec3 invalid = Vec3.Invalid;
				this.PurigShipCutsceneCamera = Camera.CreateCamera();
				gameEntity.GetCameraParamsFromCameraScript(this.PurigShipCutsceneCamera, ref invalid);
				this.PurigShipCutsceneCamera.SetFovVertical(this.PurigShipCutsceneCamera.GetFovVertical(), Screen.AspectRatio, this.PurigShipCutsceneCamera.Near, this.PurigShipCutsceneCamera.Far);
				MatrixFrame matrixFrame;
				this.GetCameraFrame(out matrixFrame);
				this.PurigShipCutsceneCamera.Frame = matrixFrame;
				base.MissionScreen.CustomCamera = this.PurigShipCutsceneCamera;
				this._purigCutsceneCameraChangeTimer = new MissionTimer(6f);
			}
		}

		// Token: 0x06000123 RID: 291 RVA: 0x00008BBF File Offset: 0x00006DBF
		private void GetCameraFrame(out MatrixFrame cameraFrame)
		{
			cameraFrame = this.PurigShipCutsceneCamera.Frame;
		}

		// Token: 0x06000124 RID: 292 RVA: 0x00008BD4 File Offset: 0x00006DD4
		private void ChangeMainAgentRotation(Vec3 mainAgentDirection)
		{
			Agent main = Agent.Main;
			Vec2 vec = mainAgentDirection.AsVec2;
			vec = vec.Normalized();
			main.SetMovementDirection(ref vec);
			base.MissionScreen.CameraBearing = mainAgentDirection.RotationZ;
		}

		// Token: 0x06000125 RID: 293 RVA: 0x00008C10 File Offset: 0x00006E10
		private void HandleEscapeShipStuckCheck()
		{
			if (this._escapeShipStuckCheckState != Quest5SetPieceBattleMissionView.EscapeShipStuckCheckState.End)
			{
				switch (this._escapeShipStuckCheckState)
				{
				case Quest5SetPieceBattleMissionView.EscapeShipStuckCheckState.None:
				{
					Quest5SetPieceBattleMissionController quest5SetPieceBattleMissionController = this._quest5SetPieceBattleMissionController;
					if (quest5SetPieceBattleMissionController != null && quest5SetPieceBattleMissionController.State == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase2InProgress)
					{
						this._escapeShipStuckCheckState = Quest5SetPieceBattleMissionView.EscapeShipStuckCheckState.CheckForStuck;
						return;
					}
					break;
				}
				case Quest5SetPieceBattleMissionView.EscapeShipStuckCheckState.CheckForStuck:
					if (this._quest5SetPieceBattleMissionController.State != Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase2InProgress)
					{
						this._escapeShipStuckCheckState = Quest5SetPieceBattleMissionView.EscapeShipStuckCheckState.End;
						return;
					}
					if (this._quest5SetPieceBattleMissionController.IsEscapeShipStuck)
					{
						this._escapeShipStuckCheckState = Quest5SetPieceBattleMissionView.EscapeShipStuckCheckState.FadeOut;
						this.SetPlayerMovementEnabled(false);
						return;
					}
					break;
				case Quest5SetPieceBattleMissionView.EscapeShipStuckCheckState.FadeOut:
					ScreenFadeController.BeginFadeOutAndIn(0.25f, 0.25f, 0.25f);
					this._escapeShipStuckCheckState = Quest5SetPieceBattleMissionView.EscapeShipStuckCheckState.TeleportEscapeShip;
					return;
				case Quest5SetPieceBattleMissionView.EscapeShipStuckCheckState.TeleportEscapeShip:
					if (ScreenFadeController.IsFadedOut)
					{
						this._quest5SetPieceBattleMissionController.HandleEscapeShipStuck();
						this.SetPlayerMovementEnabled(true);
						this._escapeShipStuckCheckState = Quest5SetPieceBattleMissionView.EscapeShipStuckCheckState.CheckForStuck;
					}
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x0400006D RID: 109
		private const string PurigShipCutsceneCamTag = "purig_ship_cutscene_cam_tag";

		// Token: 0x0400006E RID: 110
		private TextObject _restrictionNotificationText = new TextObject("{=GHuQ4xKj}The realm's borders hold firm. You are returned.", null);

		// Token: 0x0400006F RID: 111
		private Quest5SetPieceBattleMissionView.Quest5SetPieceBattleMissionViewState _state;

		// Token: 0x04000070 RID: 112
		private Quest5SetPieceBattleMissionController _quest5SetPieceBattleMissionController;

		// Token: 0x04000071 RID: 113
		private Quest5SetPieceBattleMissionView.ApproachPlayerShipLocationCheckState _approachPlayerShipLocationCheckState;

		// Token: 0x04000072 RID: 114
		private Quest5SetPieceBattleMissionView.AllowedSwimRadiusCheckState _allowedSwimRadiusCheckState;

		// Token: 0x04000073 RID: 115
		private Quest5SetPieceBattleMissionView.EscapeShipStuckCheckState _escapeShipStuckCheckState;

		// Token: 0x04000074 RID: 116
		private Quest5SetPieceBattleMissionView.PurigCutsceneCameraChangeState _purigCutsceneCameraChangeState;

		// Token: 0x04000075 RID: 117
		private MissionTimer _purigCutsceneCameraChangeTimer;

		// Token: 0x04000076 RID: 118
		private MissionTimer _missionEndTimer;

		// Token: 0x04000077 RID: 119
		private bool _isPlayerShipRotationCorrectedAtTheStartOfTheMission;

		// Token: 0x04000078 RID: 120
		private bool _isMainAgentRotatedBeforeBossFight;

		// Token: 0x04000079 RID: 121
		public Camera PurigShipCutsceneCamera;

		// Token: 0x0400007A RID: 122
		public Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState LastHitCheckpoint;

		// Token: 0x0200004B RID: 75
		public enum Quest5SetPieceBattleMissionViewState
		{
			// Token: 0x04000103 RID: 259
			None,
			// Token: 0x04000104 RID: 260
			FadeOut,
			// Token: 0x04000105 RID: 261
			Initialize,
			// Token: 0x04000106 RID: 262
			FadeIn,
			// Token: 0x04000107 RID: 263
			End
		}

		// Token: 0x0200004C RID: 76
		private enum ApproachPlayerShipLocationCheckState
		{
			// Token: 0x04000109 RID: 265
			None,
			// Token: 0x0400010A RID: 266
			CheckDistance,
			// Token: 0x0400010B RID: 267
			FadeOut,
			// Token: 0x0400010C RID: 268
			TeleportPlayerShip,
			// Token: 0x0400010D RID: 269
			End
		}

		// Token: 0x0200004D RID: 77
		private enum AllowedSwimRadiusCheckState
		{
			// Token: 0x0400010F RID: 271
			None,
			// Token: 0x04000110 RID: 272
			CheckDistance,
			// Token: 0x04000111 RID: 273
			FadeOut,
			// Token: 0x04000112 RID: 274
			TeleportPlayer,
			// Token: 0x04000113 RID: 275
			End
		}

		// Token: 0x0200004E RID: 78
		private enum EscapeShipStuckCheckState
		{
			// Token: 0x04000115 RID: 277
			None,
			// Token: 0x04000116 RID: 278
			CheckForStuck,
			// Token: 0x04000117 RID: 279
			FadeOut,
			// Token: 0x04000118 RID: 280
			TeleportEscapeShip,
			// Token: 0x04000119 RID: 281
			End
		}

		// Token: 0x0200004F RID: 79
		private enum PurigCutsceneCameraChangeState
		{
			// Token: 0x0400011B RID: 283
			None,
			// Token: 0x0400011C RID: 284
			WaitingForCountDown,
			// Token: 0x0400011D RID: 285
			FadeOut,
			// Token: 0x0400011E RID: 286
			ChangeBackToDefaultCamera,
			// Token: 0x0400011F RID: 287
			End
		}
	}
}
