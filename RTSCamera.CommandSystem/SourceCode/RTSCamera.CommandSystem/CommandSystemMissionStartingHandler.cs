using System;
using System.Collections.Generic;
using MissionLibrary.Controller;
using MissionSharedLibrary.Controller;
using RTSCamera.CommandSystem.Logic;
using RTSCamera.CommandSystem.View;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace RTSCamera.CommandSystem
{
	// Token: 0x0200004C RID: 76
	public class CommandSystemMissionStartingHandler : AMissionStartingHandler
	{
		// Token: 0x06000275 RID: 629 RVA: 0x00008C9C File Offset: 0x00006E9C
		public override void OnCreated(MissionView entranceView)
		{
			foreach (MissionBehavior missionBehavior in new List<MissionBehavior>
			{
				new CommandSystemLogic(),
				new CommandQueuePreview()
			})
			{
				MissionStartingManager.AddMissionBehavior(entranceView, missionBehavior);
			}
		}

		// Token: 0x06000276 RID: 630 RVA: 0x00008D04 File Offset: 0x00006F04
		public override void OnPreMissionTick(MissionView entranceView, float dt)
		{
		}
	}
}
