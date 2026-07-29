using System;
using MissionLibrary.Provider;

namespace MissionSharedLibrary.Provider
{
	// Token: 0x02000046 RID: 70
	public class ProviderCreator
	{
		// Token: 0x0600025E RID: 606 RVA: 0x00008A23 File Offset: 0x00006C23
		public static ConcreteProvider<T> Create<T>(Func<ATag<T>> creator, string id, Version providerVersion) where T : ATag<T>
		{
			return new ConcreteProvider<T>(creator, id, providerVersion);
		}
	}
}
