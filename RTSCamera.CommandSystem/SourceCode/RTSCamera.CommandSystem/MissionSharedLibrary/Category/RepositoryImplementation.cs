using System;
using System.Collections.Generic;
using MissionLibrary.Provider;
using MissionLibrary.Repository;

namespace MissionSharedLibrary.Category
{
	// Token: 0x0200004B RID: 75
	public class RepositoryImplementation<TItem> : ARepository<RepositoryImplementation<TItem>, TItem> where TItem : AItem<TItem>
	{
		// Token: 0x1700008A RID: 138
		// (get) Token: 0x06000270 RID: 624 RVA: 0x00008B98 File Offset: 0x00006D98
		public override Dictionary<string, IProvider<TItem>> Items { get; } = new Dictionary<string, IProvider<TItem>>();

		// Token: 0x06000271 RID: 625 RVA: 0x00008BA0 File Offset: 0x00006DA0
		public override void RegisterItem(IProvider<TItem> provider, bool addOnlyWhenMissing = true)
		{
			IProvider<TItem> provider2;
			if (!this.Items.TryGetValue(provider.Id, out provider2))
			{
				this.Items.Add(provider.Id, provider);
				return;
			}
			if ((provider2.ProviderVersion == provider.ProviderVersion && addOnlyWhenMissing) || provider2.ProviderVersion > provider.ProviderVersion)
			{
				return;
			}
			this.Items[provider.Id] = provider;
		}

		// Token: 0x06000272 RID: 626 RVA: 0x00008C10 File Offset: 0x00006E10
		public override TItem GetItem(string categoryId)
		{
			IProvider<TItem> provider;
			if (this.Items.TryGetValue(categoryId, out provider))
			{
				return provider.Value;
			}
			return default(TItem);
		}

		// Token: 0x06000273 RID: 627 RVA: 0x00008C40 File Offset: 0x00006E40
		public override T GetItem<T>(string categoryId)
		{
			IProvider<TItem> provider;
			if (this.Items.TryGetValue(categoryId, out provider))
			{
				T t = provider.Value as T;
				if (t != null)
				{
					return t;
				}
			}
			return default(T);
		}
	}
}
