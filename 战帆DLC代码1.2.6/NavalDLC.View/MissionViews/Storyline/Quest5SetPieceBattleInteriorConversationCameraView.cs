using System;
using NavalDLC.Storyline.MissionControllers;
using SandBox.Conversation.MissionLogics;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace NavalDLC.View.MissionViews.Storyline
{
	// Token: 0x0200002B RID: 43
	public class Quest5SetPieceBattleInteriorConversationCameraView : MissionView
	{
		// Token: 0x06000117 RID: 279 RVA: 0x000081A8 File Offset: 0x000063A8
		public override void AfterStart()
		{
			base.AfterStart();
			this._quest5SetPieceBattleMissionController = Mission.Current.GetMissionBehavior<Quest5SetPieceBattleMissionController>();
		}

		// Token: 0x06000118 RID: 280 RVA: 0x000081C0 File Offset: 0x000063C0
		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			switch (this._state)
			{
			case Quest5SetPieceBattleInteriorConversationCameraView.CameraChangeState.None:
				if (this._quest5SetPieceBattleMissionController.State != Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1ShipInteriorPhase)
				{
					return;
				}
				if (this._quest5SetPieceBattleMissionController.Phase1InteriorCameraSisterEntity != null)
				{
					this._state = Quest5SetPieceBattleInteriorConversationCameraView.CameraChangeState.FadeOutBeforeConversation;
					return;
				}
				break;
			case Quest5SetPieceBattleInteriorConversationCameraView.CameraChangeState.FadeOutBeforeConversation:
				ScreenFadeController.BeginFadeOutAndIn(0.25f, 0.25f, 0.25f);
				this._state = Quest5SetPieceBattleInteriorConversationCameraView.CameraChangeState.ConversationInProgress;
				return;
			case Quest5SetPieceBattleInteriorConversationCameraView.CameraChangeState.ConversationInProgress:
				if (!(this._quest5SetPieceBattleMissionController.Phase1InteriorCameraSisterEntity != null))
				{
					this._state = Quest5SetPieceBattleInteriorConversationCameraView.CameraChangeState.FadeOutAfterConversation;
					return;
				}
				if (ScreenFadeController.IsFadedOut)
				{
					if (this._interiorConversationCamera == null)
					{
						Vec3 invalid = Vec3.Invalid;
						this._interiorConversationCamera = Camera.CreateCamera();
						this._quest5SetPieceBattleMissionController.Phase1InteriorCameraSisterEntity.GetCameraParamsFromCameraScript(this._interiorConversationCamera, ref invalid);
						this._interiorConversationCamera.SetFovVertical(this._interiorConversationCamera.GetFovVertical(), Screen.AspectRatio, this._interiorConversationCamera.Near, this._interiorConversationCamera.Far);
					}
					Agent.Main.AgentVisuals.SetVisible(false);
				}
				else if (ScreenFadeController.IsFadingIn)
				{
					this._fadeInDuration -= dt;
				}
				if (this._fadeInDuration <= 0f && !this._sisterConversationStarted)
				{
					this._sisterConversationStarted = true;
					MissionConversationLogic missionBehavior = base.Mission.GetMissionBehavior<MissionConversationLogic>();
					missionBehavior.DisableStartConversation(false);
					missionBehavior.StartConversation(this._quest5SetPieceBattleMissionController.SisterAgent, false, false);
				}
				if (this._interiorConversationCamera != null)
				{
					this._interiorConversationCamera.Frame = this._quest5SetPieceBattleMissionController.Phase1InteriorCameraSisterEntity.GetGlobalFrame();
					base.MissionScreen.CustomCamera = this._interiorConversationCamera;
					return;
				}
				break;
			case Quest5SetPieceBattleInteriorConversationCameraView.CameraChangeState.FadeOutAfterConversation:
				ScreenFadeController.BeginFadeOutAndIn(0.25f, 0.25f, 0.25f);
				this._state = Quest5SetPieceBattleInteriorConversationCameraView.CameraChangeState.ChangeCameraBack;
				return;
			case Quest5SetPieceBattleInteriorConversationCameraView.CameraChangeState.ChangeCameraBack:
				if (ScreenFadeController.IsFadedOut)
				{
					Agent.Main.AgentVisuals.SetVisible(true);
					base.MissionScreen.CustomCamera = null;
					this._state = Quest5SetPieceBattleInteriorConversationCameraView.CameraChangeState.End;
				}
				break;
			case Quest5SetPieceBattleInteriorConversationCameraView.CameraChangeState.End:
				break;
			default:
				return;
			}
		}

		// Token: 0x04000068 RID: 104
		private Quest5SetPieceBattleMissionController _quest5SetPieceBattleMissionController;

		// Token: 0x04000069 RID: 105
		private float _fadeInDuration = 0.25f;

		// Token: 0x0400006A RID: 106
		private Camera _interiorConversationCamera;

		// Token: 0x0400006B RID: 107
		private Quest5SetPieceBattleInteriorConversationCameraView.CameraChangeState _state;

		// Token: 0x0400006C RID: 108
		private bool _sisterConversationStarted;

		// Token: 0x0200004A RID: 74
		private enum CameraChangeState
		{
			// Token: 0x040000FC RID: 252
			None,
			// Token: 0x040000FD RID: 253
			FadeOutBeforeConversation,
			// Token: 0x040000FE RID: 254
			ConversationInProgress,
			// Token: 0x040000FF RID: 255
			FadeOutAfterConversation,
			// Token: 0x04000100 RID: 256
			ChangeCameraBack,
			// Token: 0x04000101 RID: 257
			End
		}
	}
}
