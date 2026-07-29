using System;
using MissionLibrary.Controller;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace RTSCameraAgentComponent
{
	// Token: 0x02000003 RID: 3
	public class MissionStartingHandler : AMissionStartingHandler
	{
		// Token: 0x06000003 RID: 3 RVA: 0x0000206D File Offset: 0x0000026D
		public override void OnCreated(MissionView entranceView)
		{
			MissionStartingHandler.AddMissionBehavior(entranceView, new ComponentAdder());
		}

		// Token: 0x06000004 RID: 4 RVA: 0x0000207A File Offset: 0x0000027A
		public override void OnPreMissionTick(MissionView entranceView, float dt)
		{
		}

		// Token: 0x06000005 RID: 5 RVA: 0x0000207C File Offset: 0x0000027C
		public static void AddMissionBehavior(MissionView entranceView, MissionBehavior behaviour)
		{
			behaviour.OnAfterMissionCreated();
			entranceView.Mission.AddMissionBehavior(behaviour);
		}
	}
}
