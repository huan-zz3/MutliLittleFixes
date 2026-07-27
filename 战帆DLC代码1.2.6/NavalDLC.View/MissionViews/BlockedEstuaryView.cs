using System;
using NavalDLC.Missions.ShipInput;
using NavalDLC.Storyline.MissionControllers;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace NavalDLC.View.MissionViews
{
	// Token: 0x02000018 RID: 24
	public class BlockedEstuaryView : MissionView
	{
		// Token: 0x06000096 RID: 150 RVA: 0x00005C48 File Offset: 0x00003E48
		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			if (!this._isInitialized)
			{
				this.InitializeView();
			}
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00005C60 File Offset: 0x00003E60
		public override void OnMissionTick(float dt)
		{
			if (this._isInitialized && !Game.Current.GameStateManager.ActiveStateDisabledByUser && this._camera != null)
			{
				this.UpdateCamera(dt);
				if (!this._cameraTargetFrame.IsIdentity && !this._cameraTargetFrame.IsZero)
				{
					Camera camera = this._camera;
					MatrixFrame frame = this._camera.Frame;
					camera.Frame = MatrixFrame.Lerp(ref frame, ref this._cameraTargetFrame, dt * this._transitionSpeed);
				}
			}
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00005CE4 File Offset: 0x00003EE4
		public void FadeToBlack(float fadeOutTime, float blackTime, float fadeInTime)
		{
			ScreenFadeController.BeginFadeOutAndIn(fadeOutTime, blackTime, fadeInTime);
			base.MissionScreen.CameraBearing = Agent.Main.LookDirection.RotationZ;
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00005D18 File Offset: 0x00003F18
		private void UpdateCamera(float dt)
		{
			if (this._controller.CollisionImminent)
			{
				if (this._switchTimer <= 2f)
				{
					this._switchTimer += dt;
				}
				else
				{
					this._useShipCamera = false;
				}
			}
			if (this._useShipCamera)
			{
				this._transitionSpeed = 4f;
				this.SetCameraFrame(this._shipCameraFrame.GlobalPosition, -this._shipCameraFrame.GetGlobalFrame().rotation.u * 2f);
				return;
			}
			if (this._controller.CollisionImminent)
			{
				this._transitionSpeed = 0.3f;
				this.SetCameraFrame(this._cameraFrame.GlobalPosition, -this._cameraFrame.GetGlobalFrame().rotation.u);
			}
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00005DE4 File Offset: 0x00003FE4
		private void SetupCamera()
		{
			this._camera = Camera.CreateCamera();
			Vec3 zero = Vec3.Zero;
			this._cameraFrame.GetCameraParamsFromCameraScript(this._camera, ref zero);
			base.MissionScreen.CustomCamera = this._camera;
			this._camera.Frame = base.MissionScreen.CombatCamera.Frame;
			this._switchTimer = 0f;
			this._useShipCamera = true;
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00005E54 File Offset: 0x00004054
		private void SetCameraFrame(Vec3 position, Vec3 direction)
		{
			MatrixFrame frame = this._camera.Frame;
			frame.origin = position;
			frame.rotation.s = Vec3.Side;
			frame.rotation.f = Vec3.Up;
			frame.rotation.u = -direction;
			frame.rotation.Orthonormalize();
			this._cameraTargetFrame = frame;
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00005EBC File Offset: 0x000040BC
		private void InitializeView()
		{
			this._controller = base.Mission.GetMissionBehavior<BlockedEstuaryMissionController>();
			this._mainAgentController = base.Mission.GetMissionBehavior<MissionMainAgentController>();
			BlockedEstuaryMissionController controller = this._controller;
			controller.OnCheckPointReachedEvent = (Action)Delegate.Combine(controller.OnCheckPointReachedEvent, new Action(this.OnCheckPointReached));
			BlockedEstuaryMissionController controller2 = this._controller;
			controller2.OnLastExitZoneReachedEvent = (Action)Delegate.Combine(controller2.OnLastExitZoneReachedEvent, new Action(this.LastExitZoneReached));
			BlockedEstuaryMissionController controller3 = this._controller;
			controller3.OnPhaseEnd = (Action)Delegate.Combine(controller3.OnPhaseEnd, new Action(this.OnPhaseEnd));
			this._cameraFrame = this.GetCameraEntity();
			this._shipCameraFrame = this.GetShipCameraEntity();
			this._isInitialized = true;
			MissionShipControlView missionBehavior = base.Mission.GetMissionBehavior<MissionShipControlView>();
			if (missionBehavior != null && missionBehavior.IsReady())
			{
				if (this._controller.CurrentPhase != BlockedEstuaryMissionController.BattlePhase.Phase3)
				{
					missionBehavior.SetSailInput(SailInput.Full);
				}
				missionBehavior.SetActiveCameraMode(MissionShipControlView.CameraModes.Back);
			}
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00005FB2 File Offset: 0x000041B2
		private void OnPhaseEnd()
		{
			if (this._camera != null)
			{
				this.ReleaseCamera();
			}
			this.FadeToBlack(0.1f, 0.5f, 0.5f);
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00005FDD File Offset: 0x000041DD
		private void LastExitZoneReached()
		{
			this._mainAgentController.Disable();
			this.SetupCamera();
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00005FF0 File Offset: 0x000041F0
		private void OnPlayerDismounted()
		{
			this.FadeToBlack(0.1f, 0.5f, 0.5f);
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00006007 File Offset: 0x00004207
		private void OnCheckPointReached()
		{
			if (Agent.Main.HasMount)
			{
				this._mainAgentController.Disable();
			}
			this._checkPointReached = true;
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00006027 File Offset: 0x00004227
		public override void OnAgentDismount(Agent agent)
		{
			if (agent.IsMainAgent && this._checkPointReached)
			{
				this._mainAgentController.Enable();
				this.OnPlayerDismounted();
			}
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x0000604C File Offset: 0x0000424C
		public override void OnMissionScreenFinalize()
		{
			BlockedEstuaryMissionController controller = this._controller;
			controller.OnCheckPointReachedEvent = (Action)Delegate.Remove(controller.OnCheckPointReachedEvent, new Action(this.OnCheckPointReached));
			BlockedEstuaryMissionController controller2 = this._controller;
			controller2.OnLastExitZoneReachedEvent = (Action)Delegate.Remove(controller2.OnLastExitZoneReachedEvent, new Action(this.LastExitZoneReached));
			BlockedEstuaryMissionController controller3 = this._controller;
			controller3.OnPhaseEnd = (Action)Delegate.Remove(controller3.OnPhaseEnd, new Action(this.OnPhaseEnd));
			base.OnMissionScreenFinalize();
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x000060D4 File Offset: 0x000042D4
		private void ReleaseCamera()
		{
			this._mainAgentController.Enable();
			base.MissionScreen.UpdateFreeCamera(base.MissionScreen.CustomCamera.Frame);
			base.MissionScreen.CustomCamera = null;
			this._camera.ReleaseCamera();
			this._camera = null;
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00006128 File Offset: 0x00004328
		private GameEntity GetCameraEntity()
		{
			GameEntity gameEntity = base.Mission.Scene.FindEntityWithTag("sp_camera");
			if (gameEntity != null)
			{
				return gameEntity;
			}
			Debug.FailedAssert("Cant find CameraEntity", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.View\\MissionViews\\BlockedEstuaryView.cs", "GetCameraEntity", 217);
			return null;
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00006170 File Offset: 0x00004370
		private GameEntity GetShipCameraEntity()
		{
			GameEntity gameEntity = base.Mission.Scene.FindEntityWithTag("sp_camera_ship");
			if (gameEntity != null)
			{
				return gameEntity;
			}
			Debug.FailedAssert("Cant find ShipCameraEntity", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.View\\MissionViews\\BlockedEstuaryView.cs", "GetShipCameraEntity", 229);
			return null;
		}

		// Token: 0x0400002A RID: 42
		private const string CameraSpawnId = "sp_camera";

		// Token: 0x0400002B RID: 43
		private const string CameraShipSpawnId = "sp_camera_ship";

		// Token: 0x0400002C RID: 44
		private BlockedEstuaryMissionController _controller;

		// Token: 0x0400002D RID: 45
		private Camera _camera;

		// Token: 0x0400002E RID: 46
		private bool _isInitialized;

		// Token: 0x0400002F RID: 47
		private GameEntity _cameraFrame;

		// Token: 0x04000030 RID: 48
		private GameEntity _shipCameraFrame;

		// Token: 0x04000031 RID: 49
		private MissionMainAgentController _mainAgentController;

		// Token: 0x04000032 RID: 50
		private bool _checkPointReached;

		// Token: 0x04000033 RID: 51
		private MatrixFrame _cameraTargetFrame;

		// Token: 0x04000034 RID: 52
		private bool _useShipCamera;

		// Token: 0x04000035 RID: 53
		private float _switchTimer;

		// Token: 0x04000036 RID: 54
		private float _transitionSpeed = 2f;
	}
}
