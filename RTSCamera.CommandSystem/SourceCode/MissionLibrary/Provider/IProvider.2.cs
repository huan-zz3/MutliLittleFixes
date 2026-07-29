using System;

namespace MissionLibrary.Provider
{
	// Token: 0x02000015 RID: 21
	public interface IProvider<out T> : IProvider where T : ATag<T>
	{
		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000053 RID: 83
		T Value { get; }
	}
}
