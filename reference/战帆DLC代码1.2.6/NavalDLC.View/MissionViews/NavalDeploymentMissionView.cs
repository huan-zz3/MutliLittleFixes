using System;
using NavalDLC.View.MissionViews.Order;
using TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer;

namespace NavalDLC.View.MissionViews
{
	// Token: 0x0200001B RID: 27
	public class NavalDeploymentMissionView : DeploymentMissionView
	{
		// Token: 0x060000C3 RID: 195 RVA: 0x00006BFE File Offset: 0x00004DFE
		public override void AfterStart()
		{
			this._orderTroopPlacer = base.Mission.GetMissionBehavior<NavalOrderTroopPlacer>();
			this._deploymentBoundaryMarkerHandler = base.Mission.GetMissionBehavior<NavalMissionDeploymentBoundaryMarker>();
		}
	}
}
