using System;

namespace MissionLibrary.Provider
{
	// Token: 0x02000012 RID: 18
	public interface IIdProvider
	{
		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600004B RID: 75
		string Id { get; }

		// Token: 0x0600004C RID: 76
		void ForceCreate();

		// Token: 0x0600004D RID: 77
		void Clear();
	}
}
