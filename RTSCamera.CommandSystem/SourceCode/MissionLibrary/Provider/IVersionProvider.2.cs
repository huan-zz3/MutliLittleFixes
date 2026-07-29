using System;

namespace MissionLibrary.Provider
{
	// Token: 0x02000018 RID: 24
	public interface IVersionProvider<out T> : IVersionProvider where T : ATag<T>
	{
		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000058 RID: 88
		T Value { get; }
	}
}
