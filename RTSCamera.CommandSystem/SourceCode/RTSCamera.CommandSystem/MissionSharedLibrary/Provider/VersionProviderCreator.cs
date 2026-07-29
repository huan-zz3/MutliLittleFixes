using System;
using MissionLibrary.Provider;

namespace MissionSharedLibrary.Provider
{
	// Token: 0x0200004A RID: 74
	public class VersionProviderCreator
	{
		// Token: 0x0600026E RID: 622 RVA: 0x00008B87 File Offset: 0x00006D87
		public static ConcreteVersionProvider<T> Create<T>(Func<ATag<T>> creator, Version providerVersion) where T : ATag<T>
		{
			return new ConcreteVersionProvider<T>(creator, providerVersion);
		}
	}
}
