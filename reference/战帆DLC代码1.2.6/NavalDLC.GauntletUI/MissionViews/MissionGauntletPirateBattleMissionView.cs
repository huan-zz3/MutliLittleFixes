using System;
using NavalDLC.View.MissionViews;
using TaleWorlds.MountAndBlade.View;

namespace NavalDLC.GauntletUI.MissionViews
{
	// Token: 0x0200001C RID: 28
	[OverrideView(typeof(NavalStorylinePirateBattleMissionView))]
	internal class MissionGauntletPirateBattleMissionView : NavalStorylinePirateBattleMissionView
	{
		// Token: 0x060000BB RID: 187 RVA: 0x00007928 File Offset: 0x00005B28
		protected override void OnShipsInitializedInternal()
		{
			MissionGauntletShipControlView missionBehavior = base.Mission.GetMissionBehavior<MissionGauntletShipControlView>();
			if (missionBehavior != null && missionBehavior.IsReady())
			{
				missionBehavior.SetActiveCameraMode(MissionShipControlView.CameraModes.Back);
			}
		}
	}
}
