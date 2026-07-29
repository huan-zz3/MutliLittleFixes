using System;

namespace MissionSharedLibrary.Config.HotKey
{
	// Token: 0x02000041 RID: 65
	public interface IGameKeyConfig
	{
		// Token: 0x1700007D RID: 125
		// (get) Token: 0x06000245 RID: 581
		// (set) Token: 0x06000246 RID: 582
		SerializedGameKeyCategory Category { get; set; }

		// Token: 0x06000247 RID: 583
		bool Serialize();

		// Token: 0x06000248 RID: 584
		bool Deserialize();
	}
}
