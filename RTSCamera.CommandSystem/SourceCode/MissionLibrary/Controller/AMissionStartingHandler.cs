using System;
using MissionLibrary.Provider;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace MissionLibrary.Controller
{
	// Token: 0x02000024 RID: 36
	public abstract class AMissionStartingHandler : ATag<AMissionStartingHandler>
	{
		// Token: 0x06000092 RID: 146
		public abstract void OnCreated(MissionView entranceView);

		// Token: 0x06000093 RID: 147
		public abstract void OnPreMissionTick(MissionView entranceView, float dt);
	}
}
