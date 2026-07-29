using System;

namespace MissionLibrary.Provider
{
	// Token: 0x02000017 RID: 23
	public interface IVersionProvider
	{
		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000055 RID: 85
		Version ProviderVersion { get; }

		// Token: 0x06000056 RID: 86
		void ForceCreate();

		// Token: 0x06000057 RID: 87
		void Clear();
	}
}
