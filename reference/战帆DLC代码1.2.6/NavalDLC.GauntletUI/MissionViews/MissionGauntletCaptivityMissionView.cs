using System;
using NavalDLC.View.MissionViews;
using NavalDLC.View.MissionViews.Storyline;
using TaleWorlds.MountAndBlade.View;

namespace NavalDLC.GauntletUI.MissionViews
{
	// Token: 0x02000013 RID: 19
	[OverrideView(typeof(NavalCaptivityBattleMissionView))]
	public class MissionGauntletCaptivityMissionView : NavalCaptivityBattleMissionView
	{
		// Token: 0x0600006E RID: 110 RVA: 0x000055C8 File Offset: 0x000037C8
		protected override void OnFirstHighlightClearedInternal()
		{
			MissionGauntletShipControlView missionBehavior = base.Mission.GetMissionBehavior<MissionGauntletShipControlView>();
			if (missionBehavior != null && missionBehavior.IsReady())
			{
				missionBehavior.ResumeFeature(MissionGauntletShipControlView.ShipControlFeatureFlags.ToggleSails);
			}
		}

		// Token: 0x0600006F RID: 111 RVA: 0x000055F4 File Offset: 0x000037F4
		protected override void OnPlayerStartedEscapeInternal()
		{
			MissionGauntletShipControlView missionBehavior = base.Mission.GetMissionBehavior<MissionGauntletShipControlView>();
			if (missionBehavior != null && missionBehavior.IsReady())
			{
				missionBehavior.SuspendFeature(MissionGauntletShipControlView.ShipControlFeatureFlags.ToggleSails);
				missionBehavior.SuspendFeature(MissionGauntletShipControlView.ShipControlFeatureFlags.ChangeCamera);
				missionBehavior.SetActiveCameraMode(MissionShipControlView.CameraModes.Shoulder);
			}
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00005632 File Offset: 0x00003832
		protected override void OnOarsmenLevelChangedInternal(int level)
		{
			if (!this._hasHandledOarsmenLevel && level == 2)
			{
				this._hasHandledOarsmenLevel = true;
				base.Mission.GetMissionBehavior<MissionGauntletShipControlView>().ResumeFeature(MissionGauntletShipControlView.ShipControlFeatureFlags.ChangeCamera);
			}
		}

		// Token: 0x0400002A RID: 42
		private bool _hasHandledOarsmenLevel;
	}
}
