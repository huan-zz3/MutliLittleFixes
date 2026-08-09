using System;
using NavalDLC.Missions.ShipInput;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace NavalDLC.View.MissionViews
{
	// Token: 0x02000027 RID: 39
	public class WoundedBeastView : MissionView
	{
		// Token: 0x060000FA RID: 250 RVA: 0x00007D64 File Offset: 0x00005F64
		public override void OnMissionTick(float dt)
		{
			if (!this._initialized)
			{
				this.Initialize();
				this._initialized = true;
			}
		}

		// Token: 0x060000FB RID: 251 RVA: 0x00007D7C File Offset: 0x00005F7C
		private void Initialize()
		{
			MissionShipControlView missionBehavior = base.Mission.GetMissionBehavior<MissionShipControlView>();
			if (missionBehavior != null && missionBehavior.IsReady())
			{
				missionBehavior.SetSailInput(SailInput.Full);
			}
			if (missionBehavior != null && missionBehavior.IsReady())
			{
				missionBehavior.SetActiveCameraMode(MissionShipControlView.CameraModes.Back);
			}
		}

		// Token: 0x04000060 RID: 96
		private bool _initialized;
	}
}
