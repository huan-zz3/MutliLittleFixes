using System;
using NavalDLC.Storyline;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace NavalDLC.View.MissionViews
{
	// Token: 0x02000026 RID: 38
	public class NavalStorylinePirateBattleMissionView : MissionView
	{
		// Token: 0x060000F3 RID: 243 RVA: 0x00007CA6 File Offset: 0x00005EA6
		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			if (!this._isInitialized)
			{
				this.InitializeView();
			}
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x00007CBD File Offset: 0x00005EBD
		private void OnBeginScreenFade(float fadeDuration, float blackScreenDuration)
		{
			ScreenFadeController.BeginFadeOutAndIn(fadeDuration, blackScreenDuration, fadeDuration);
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x00007CC7 File Offset: 0x00005EC7
		private void OnCameraBearingNeedsUpdate(float direction)
		{
			base.MissionScreen.CameraBearing = direction;
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x00007CD8 File Offset: 0x00005ED8
		private void InitializeView()
		{
			this._controller = base.Mission.GetMissionBehavior<PirateBattleMissionController>();
			this._isInitialized = this._controller != null;
			if (this._controller != null)
			{
				this._controller.OnBeginScreenFadeEvent += this.OnBeginScreenFade;
				this._controller.OnCameraBearingNeedsUpdateEvent += this.OnCameraBearingNeedsUpdate;
				this._controller.OnShipsInitializedEvent += this.OnShipsInitialized;
			}
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x00007D52 File Offset: 0x00005F52
		private void OnShipsInitialized()
		{
			this.OnShipsInitializedInternal();
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x00007D5A File Offset: 0x00005F5A
		protected virtual void OnShipsInitializedInternal()
		{
		}

		// Token: 0x0400005E RID: 94
		private bool _isInitialized;

		// Token: 0x0400005F RID: 95
		private PirateBattleMissionController _controller;
	}
}
