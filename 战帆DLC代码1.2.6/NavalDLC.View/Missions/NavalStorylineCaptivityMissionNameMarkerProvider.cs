using System;
using System.Collections.Generic;
using NavalDLC.Missions.Objects.UsableMachines;
using NavalDLC.Storyline;
using NavalDLC.View.MissionViews.Storyline;
using NavalDLC.ViewModelCollection.Missions.NameMarkers;
using SandBox.ViewModelCollection.Missions.NameMarker;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.View.Missions
{
	// Token: 0x02000030 RID: 48
	public class NavalStorylineCaptivityMissionNameMarkerProvider : MissionNameMarkerProvider
	{
		// Token: 0x06000136 RID: 310 RVA: 0x0000916C File Offset: 0x0000736C
		protected override void OnInitialize(Mission mission)
		{
			base.OnInitialize(mission);
			this._captivityMissionController = mission.GetMissionBehavior<NavalStorylineCaptivityMissionController>();
			this._captivityMissionView = mission.GetMissionBehavior<NavalCaptivityBattleMissionView>();
		}

		// Token: 0x06000137 RID: 311 RVA: 0x00009190 File Offset: 0x00007390
		protected override void OnTick(float dt)
		{
			base.OnTick(dt);
			if (!this._hasSetTargets && this._captivityMissionController.IsInitialized())
			{
				this._hasSetTargets = true;
				base.SetMarkersDirty();
			}
			if (this._captivityMissionView.AreMarkersDirty)
			{
				base.SetMarkersDirty();
				this._captivityMissionView.OnDirtyMarkersHandled();
			}
		}

		// Token: 0x06000138 RID: 312 RVA: 0x000091E4 File Offset: 0x000073E4
		public override void CreateMarkers(List<MissionNameMarkerTargetBaseVM> markers)
		{
			if (this._hasSetTargets)
			{
				ShipControllerMachine markedShipControllerMachine = this._captivityMissionController.GetMarkedShipControllerMachine();
				if (markedShipControllerMachine != null)
				{
					markers.Add(new NavalMissionShipControlPointMarkerTargetVM(markedShipControllerMachine)
					{
						IsPersistent = true
					});
				}
			}
		}

		// Token: 0x04000080 RID: 128
		private NavalStorylineCaptivityMissionController _captivityMissionController;

		// Token: 0x04000081 RID: 129
		private NavalCaptivityBattleMissionView _captivityMissionView;

		// Token: 0x04000082 RID: 130
		private bool _hasSetTargets;
	}
}
