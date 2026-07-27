using System;
using NavalDLC.Storyline.MissionControllers;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace NavalDLC.View.MissionViews.Storyline
{
	// Token: 0x0200002A RID: 42
	public class Quest5SetPieceBattleBossFightCameraView : MissionView
	{
		// Token: 0x06000114 RID: 276 RVA: 0x00008095 File Offset: 0x00006295
		public override void AfterStart()
		{
			base.AfterStart();
			this._quest5SetPieceBattleMissionController = Mission.Current.GetMissionBehavior<Quest5SetPieceBattleMissionController>();
		}

		// Token: 0x06000115 RID: 277 RVA: 0x000080B0 File Offset: 0x000062B0
		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			if (this._quest5SetPieceBattleMissionController.State < Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.BossFightConversationInProgress)
			{
				return;
			}
			if (this._quest5SetPieceBattleMissionController.BossFightConversationCameraGameEntity != null)
			{
				if (this._bossFightCamera == null)
				{
					Vec3 invalid = Vec3.Invalid;
					this._bossFightCamera = Camera.CreateCamera();
					this._quest5SetPieceBattleMissionController.BossFightConversationCameraGameEntity.GetCameraParamsFromCameraScript(this._bossFightCamera, ref invalid);
					this._bossFightCamera.SetFovVertical(this._bossFightCamera.GetFovVertical(), Screen.AspectRatio, this._bossFightCamera.Near, this._bossFightCamera.Far);
				}
				else
				{
					this._bossFightCamera.Frame = this._quest5SetPieceBattleMissionController.BossFightConversationCameraGameEntity.GetGlobalFrame();
				}
				base.MissionScreen.CustomCamera = this._bossFightCamera;
				return;
			}
			if (base.MissionScreen.CustomCamera != null)
			{
				base.MissionScreen.CustomCamera = null;
			}
		}

		// Token: 0x04000066 RID: 102
		private Quest5SetPieceBattleMissionController _quest5SetPieceBattleMissionController;

		// Token: 0x04000067 RID: 103
		private Camera _bossFightCamera;
	}
}
