using System;
using System.Collections.Generic;
using MissionLibrary.Provider;

namespace MissionLibrary.Repository
{
	// Token: 0x02000010 RID: 16
	public abstract class ARepository<TSelf, TItem> : ATag<TSelf> where TSelf : ARepository<TSelf, TItem> where TItem : AItem<TItem>
	{
		// Token: 0x06000043 RID: 67 RVA: 0x00002405 File Offset: 0x00000605
		public static TSelf Get()
		{
			return Global.GetInstance<TSelf>("");
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000044 RID: 68
		public abstract Dictionary<string, IProvider<TItem>> Items { get; }

		// Token: 0x06000045 RID: 69
		public abstract void RegisterItem(IProvider<TItem> category, bool addOnlyWhenMissing = true);

		// Token: 0x06000046 RID: 70
		public abstract TItem GetItem(string categoryId);

		// Token: 0x06000047 RID: 71
		public abstract T GetItem<T>(string categoryId) where T : TItem;
	}
}
