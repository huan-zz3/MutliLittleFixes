using System;
using MissionLibrary.Controller;
using MissionSharedLibrary.Controller.MissionBehaviors;
using MissionSharedLibrary.View;
using MissionSharedLibrary.View.HotKey;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace MissionSharedLibrary.Controller
{
	// Token: 0x02000035 RID: 53
	public class DefaultMissionStartingHandler : AMissionStartingHandler
	{
		// Token: 0x060001F4 RID: 500 RVA: 0x00007845 File Offset: 0x00005A45
		public override void OnCreated(MissionView entranceView)
		{
			this.AddMissionLibraryMissionBehaviors(entranceView);
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x0000784E File Offset: 0x00005A4E
		public override void OnPreMissionTick(MissionView entranceView, float dt)
		{
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x00007850 File Offset: 0x00005A50
		private void AddMissionLibraryMissionBehaviors(MissionView entranceView)
		{
			MissionStartingManager.AddMissionBehavior(entranceView, new MissionLibraryLogic());
			MissionStartingManager.AddMissionBehavior(entranceView, new OptionView(24, new Version(1, 4, 0)));
			MissionStartingManager.AddMissionBehavior(entranceView, new GameKeyConfigView());
			MissionStartingManager.AddMissionBehavior(entranceView, new UsageView(26, new Version(1, 2, 0)));
		}
	}
}
