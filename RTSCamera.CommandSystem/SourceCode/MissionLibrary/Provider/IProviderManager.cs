using System;

namespace MissionLibrary.Provider
{
	// Token: 0x02000016 RID: 22
	public interface IProviderManager
	{
		// Token: 0x06000054 RID: 84
		void RegisterInstance<T>(IVersionProvider<T> newProvider, string key = "") where T : ATag<T>;
	}
}
