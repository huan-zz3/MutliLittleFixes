using System;
using MissionLibrary.HotKey;
using MissionLibrary.Repository;
using MissionSharedLibrary.Provider;

namespace MissionSharedLibrary.Usage
{
	// Token: 0x0200000E RID: 14
	public static class ARespositoryExtension
	{
		// Token: 0x06000094 RID: 148 RVA: 0x00004543 File Offset: 0x00002743
		public static void RegisterItem<TSelf, TItem>(this ARepository<TSelf, TItem> repository, Func<TItem> creator, string id, Version version, bool addOnlyWhenMissing = true) where TSelf : ARepository<TSelf, TItem> where TItem : AItem<TItem>
		{
			repository.RegisterItem(new ConcreteProvider<TItem>(creator, id, version), addOnlyWhenMissing);
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00004558 File Offset: 0x00002758
		public static void RegisterGameKeyCategory(this AGameKeyCategoryManager manager, Func<AGameKeyCategory> creator, string id, Version version, bool addOnlyWhenMissing = true)
		{
			manager.RegisterItem(new ConcreteProvider<AGameKeyCategory>(delegate
			{
				AGameKeyCategory agameKeyCategory = creator();
				agameKeyCategory.Load();
				agameKeyCategory.Save();
				return agameKeyCategory;
			}, id, version), addOnlyWhenMissing);
		}
	}
}
