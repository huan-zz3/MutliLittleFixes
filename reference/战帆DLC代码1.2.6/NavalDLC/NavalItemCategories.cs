using System;
using TaleWorlds.Core;

namespace NavalDLC
{
	// Token: 0x02000022 RID: 34
	public class NavalItemCategories
	{
		// Token: 0x17000040 RID: 64
		// (get) Token: 0x0600017B RID: 379 RVA: 0x00009CFD File Offset: 0x00007EFD
		private static NavalItemCategories Instance
		{
			get
			{
				return NavalDLCManager.Instance.NavalItemCategories;
			}
		}

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x0600017C RID: 380 RVA: 0x00009D09 File Offset: 0x00007F09
		public static ItemCategory WalrusTusk
		{
			get
			{
				return NavalItemCategories.Instance._itemCategoryWalrusTusk;
			}
		}

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x0600017D RID: 381 RVA: 0x00009D15 File Offset: 0x00007F15
		public static ItemCategory WhaleOil
		{
			get
			{
				return NavalItemCategories.Instance._itemCategoryWhaleOil;
			}
		}

		// Token: 0x0600017E RID: 382 RVA: 0x00009D21 File Offset: 0x00007F21
		public NavalItemCategories()
		{
			this.RegisterAll();
			this.InitializeAll();
		}

		// Token: 0x0600017F RID: 383 RVA: 0x00009D35 File Offset: 0x00007F35
		private static ItemCategory Create(string stringId)
		{
			return Game.Current.ObjectManager.RegisterPresumedObject<ItemCategory>(new ItemCategory(stringId));
		}

		// Token: 0x06000180 RID: 384 RVA: 0x00009D4C File Offset: 0x00007F4C
		private void RegisterAll()
		{
			this._itemCategoryWalrusTusk = NavalItemCategories.Create("walrus_tusk");
			this._itemCategoryWhaleOil = NavalItemCategories.Create("whale_oil");
		}

		// Token: 0x06000181 RID: 385 RVA: 0x00009D70 File Offset: 0x00007F70
		private void InitializeAll()
		{
			this._itemCategoryWalrusTusk.InitializeObject(true, 10, 38, 3, null, 0f, false, true);
			this._itemCategoryWhaleOil.InitializeObject(true, 10, 38, 3, null, 0f, false, true);
		}

		// Token: 0x040000A5 RID: 165
		private ItemCategory _itemCategoryWalrusTusk;

		// Token: 0x040000A6 RID: 166
		private ItemCategory _itemCategoryWhaleOil;
	}
}
