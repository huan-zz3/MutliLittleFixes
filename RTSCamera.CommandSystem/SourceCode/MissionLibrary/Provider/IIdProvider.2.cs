using System;

namespace MissionLibrary.Provider
{
	// Token: 0x02000013 RID: 19
	public interface IIdProvider<out T> : IIdProvider where T : ATag<T>
	{
		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600004E RID: 78
		T Value { get; }
	}
}
