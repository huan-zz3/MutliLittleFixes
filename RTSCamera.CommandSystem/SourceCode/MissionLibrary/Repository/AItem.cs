using System;
using MissionLibrary.Provider;

namespace MissionLibrary.Repository
{
	// Token: 0x0200000F RID: 15
	public abstract class AItem<T> : ATag<T> where T : AItem<T>
	{
		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000041 RID: 65
		public abstract string ItemId { get; }
	}
}
