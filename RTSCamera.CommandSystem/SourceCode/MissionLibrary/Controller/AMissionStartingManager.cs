using System;
using MissionLibrary.Provider;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace MissionLibrary.Controller
{
	// Token: 0x02000025 RID: 37
	public abstract class AMissionStartingManager : ATag<AMissionStartingManager>
	{
		// Token: 0x06000095 RID: 149 RVA: 0x00002A05 File Offset: 0x00000C05
		public static AMissionStartingManager Get()
		{
			return Global.GetInstance<AMissionStartingManager>("");
		}

		// Token: 0x06000096 RID: 150
		public abstract void OnCreated(MissionView entranceView);

		// Token: 0x06000097 RID: 151
		public abstract void OnPreMissionTick(MissionView entranceView, float dt);

		// Token: 0x06000098 RID: 152
		public abstract void AddHandler(AMissionStartingHandler handler);

		// Token: 0x06000099 RID: 153
		public abstract void AddSingletonHandler(string key, AMissionStartingHandler handler, Version version);
	}
}
