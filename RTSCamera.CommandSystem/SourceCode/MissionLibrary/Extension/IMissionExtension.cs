using System;
using System.Collections.Generic;
using TaleWorlds.MountAndBlade;

namespace MissionLibrary.Extension
{
	// Token: 0x0200001F RID: 31
	public interface IMissionExtension
	{
		// Token: 0x06000071 RID: 113
		void OpenExtensionMenu(Mission mission);

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000072 RID: 114
		string ExtensionName { get; }

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000073 RID: 115
		string ButtonName { get; }

		// Token: 0x06000074 RID: 116
		List<MissionBehavior> CreateMissionBehaviors(Mission mission);
	}
}
