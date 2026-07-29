using System;

namespace MissionLibrary.Provider
{
	// Token: 0x02000014 RID: 20
	public interface IProvider
	{
		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600004F RID: 79
		string Id { get; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000050 RID: 80
		Version ProviderVersion { get; }

		// Token: 0x06000051 RID: 81
		void ForceCreate();

		// Token: 0x06000052 RID: 82
		void Clear();
	}
}
