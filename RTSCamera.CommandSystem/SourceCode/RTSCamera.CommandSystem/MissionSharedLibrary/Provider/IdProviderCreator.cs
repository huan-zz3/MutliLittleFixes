using System;
using MissionLibrary.Provider;

namespace MissionSharedLibrary.Provider
{
	// Token: 0x02000048 RID: 72
	public class IdProviderCreator
	{
		// Token: 0x06000266 RID: 614 RVA: 0x00008AD7 File Offset: 0x00006CD7
		public static ConcreteIdProvider<T> Create<T>(Func<ATag<T>> creator, string id) where T : ATag<T>
		{
			return new ConcreteIdProvider<T>(creator, id);
		}
	}
}
