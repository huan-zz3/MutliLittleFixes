using System;
using NavalDLC.Storyline;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace NavalDLC.View.MissionViews
{
	// Token: 0x02000025 RID: 37
	public class NavalStorylineAlleyFightCinematicView : MissionView
	{
		// Token: 0x060000E8 RID: 232 RVA: 0x000079E0 File Offset: 0x00005BE0
		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			if (!this._isInitialized)
			{
				this.InitializeView();
				return;
			}
			if (!Game.Current.GameStateManager.ActiveStateDisabledByUser)
			{
				this.UpdateCamera(dt);
			}
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x00007A10 File Offset: 0x00005C10
		public override bool IsPhotoModeAllowed()
		{
			return !this._isCinematicPartActive;
		}

		// Token: 0x060000EA RID: 234 RVA: 0x00007A1C File Offset: 0x00005C1C
		private void GetCameraFrame(Vec3 position, Vec3 direction, out MatrixFrame cameraFrame)
		{
			cameraFrame.origin = position;
			cameraFrame.rotation.s = Vec3.Side;
			cameraFrame.rotation.f = Vec3.Up;
			cameraFrame.rotation.u = -direction;
			cameraFrame.rotation.Orthonormalize();
		}

		// Token: 0x060000EB RID: 235 RVA: 0x00007A6C File Offset: 0x00005C6C
		private void SetupCamera()
		{
			this._camera = Camera.CreateCamera();
			Camera combatCamera = base.MissionScreen.CombatCamera;
			if (combatCamera != null)
			{
				this._camera.FillParametersFrom(combatCamera);
			}
			else
			{
				Debug.FailedAssert("Combat camera is null.", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.View\\MissionViews\\NavalStorylineAlleyFightCinematicView.cs", "SetupCamera", 62);
			}
			Vec3 vec;
			Vec3 vec2;
			this._cinematicLogicController.GetCameraFrame(out vec, out vec2);
			this.GetCameraFrame(vec, vec2, out this._cameraFrame);
			this._camera.Frame = this._cameraFrame;
			base.MissionScreen.CustomCamera = this._camera;
		}

		// Token: 0x060000EC RID: 236 RVA: 0x00007AFC File Offset: 0x00005CFC
		private void UpdateCamera(float dt)
		{
			if (this._camera != null)
			{
				Vec3 vec;
				Vec3 vec2;
				this._cinematicLogicController.GetCameraFrame(out vec, out vec2);
				this.GetCameraFrame(vec, vec2, out this._cameraFrame);
				this._camera.Frame = this._cameraFrame;
			}
		}

		// Token: 0x060000ED RID: 237 RVA: 0x00007B45 File Offset: 0x00005D45
		private void ReleaseCamera()
		{
			base.MissionScreen.CustomCamera = null;
			this._camera.ReleaseCamera();
		}

		// Token: 0x060000EE RID: 238 RVA: 0x00007B60 File Offset: 0x00005D60
		private void OnCinematicStateChanged(NavalStorylineAlleyFightCinematicController.NavalAlleyFightCinematicState state)
		{
			if (this._isInitialized)
			{
				float fadeDuration = this._cinematicLogicController.GetFadeDuration();
				float fadeDuration2 = this._cinematicLogicController.GetFadeDuration();
				if (state == NavalStorylineAlleyFightCinematicController.NavalAlleyFightCinematicState.InitialFadeOut)
				{
					base.Mission.GetMissionBehavior<MissionMainAgentController>().Disable();
					this._isCinematicPartActive = true;
					ScreenFadeController.BeginFadeOutAndIn(fadeDuration, fadeDuration2, fadeDuration);
					return;
				}
				if (state == NavalStorylineAlleyFightCinematicController.NavalAlleyFightCinematicState.InitialFadeIn)
				{
					this.SetupCamera();
					return;
				}
				if (state == NavalStorylineAlleyFightCinematicController.NavalAlleyFightCinematicState.Completed)
				{
					base.Mission.GetMissionBehavior<MissionMainAgentController>().Enable();
					this._isCinematicPartActive = false;
					this.ReleaseCamera();
				}
			}
		}

		// Token: 0x060000EF RID: 239 RVA: 0x00007BDD File Offset: 0x00005DDD
		private void OnFightEnded(float fadeInDuration, float blackDuration, float fadeOutDuration)
		{
			ScreenFadeController.BeginFadeOutAndIn(fadeInDuration, blackDuration, fadeOutDuration);
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x00007BE7 File Offset: 0x00005DE7
		private void OnConversationSetup(Vec3 direction)
		{
			base.MissionScreen.CameraBearing = direction.RotationZ;
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x00007BFC File Offset: 0x00005DFC
		private void InitializeView()
		{
			this._cinematicLogicController = base.Mission.GetMissionBehavior<NavalStorylineAlleyFightCinematicController>();
			this._isInitialized = this._cinematicLogicController != null;
			if (this._cinematicLogicController != null)
			{
				this._cinematicLogicController.OnCinematicStateChanged += this.OnCinematicStateChanged;
				this._cinematicLogicController.OnFightEndedEvent += this.OnFightEnded;
				this._cinematicLogicController.OnConversationSetupEvent += this.OnConversationSetup;
			}
			MissionAgentLabelView missionBehavior = base.Mission.GetMissionBehavior<MissionAgentLabelView>();
			if (missionBehavior != null && missionBehavior.IsReady())
			{
				missionBehavior.SuspendView();
			}
		}

		// Token: 0x04000059 RID: 89
		private bool _isInitialized;

		// Token: 0x0400005A RID: 90
		private bool _isCinematicPartActive;

		// Token: 0x0400005B RID: 91
		private NavalStorylineAlleyFightCinematicController _cinematicLogicController;

		// Token: 0x0400005C RID: 92
		private Camera _camera;

		// Token: 0x0400005D RID: 93
		private MatrixFrame _cameraFrame = MatrixFrame.Identity;
	}
}
