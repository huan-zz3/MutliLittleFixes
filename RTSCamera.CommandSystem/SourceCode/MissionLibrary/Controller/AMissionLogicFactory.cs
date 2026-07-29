using System;
using System.Collections.Generic;
using MissionLibrary.Provider;
using TaleWorlds.MountAndBlade;

namespace MissionLibrary.Controller
{
	// Token: 0x02000023 RID: 35
	public abstract class AMissionLogicFactory : ATag<AMissionLogicFactory>
	{
		// Token: 0x06000090 RID: 144
		public abstract List<MissionLogic> CreateMissionLogics(Mission mission);
	}
}
