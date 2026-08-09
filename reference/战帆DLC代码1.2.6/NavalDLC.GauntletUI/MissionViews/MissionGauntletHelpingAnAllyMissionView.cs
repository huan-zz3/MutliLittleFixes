using System;
using NavalDLC.View.MissionViews;
using NavalDLC.View.MissionViews.Storyline;
using TaleWorlds.MountAndBlade.View;

namespace NavalDLC.GauntletUI.MissionViews
{
	// Token: 0x02000015 RID: 21
	[OverrideView(typeof(HelpingAnAllyMissionView))]
	public class MissionGauntletHelpingAnAllyMissionView : HelpingAnAllyMissionView
	{
		// Token: 0x0600007C RID: 124 RVA: 0x000061E0 File Offset: 0x000043E0
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
