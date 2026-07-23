using System;
using TaleWorlds.Library;

namespace BattlefieldUI.ViewModels
{
	// Token: 0x02000008 RID: 8
	public sealed class BattlefieldUIVM : ViewModel
	{
		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000054 RID: 84 RVA: 0x000036F0 File Offset: 0x000018F0
		[DataSourceProperty]
		public MBBindingList<BattlefieldHealthBarItemVM> Items
		{
			get
			{
				return this._items;
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000055 RID: 85 RVA: 0x000036F8 File Offset: 0x000018F8
		[DataSourceProperty]
		public MBBindingList<BattlefieldDamageNumberItemVM> DamageNumbers
		{
			get
			{
				return this._damageNumbers;
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000056 RID: 86 RVA: 0x00003700 File Offset: 0x00001900
		// (set) Token: 0x06000057 RID: 87 RVA: 0x00003708 File Offset: 0x00001908
		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
				}
			}
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00003726 File Offset: 0x00001926
		public BattlefieldHealthBarItemVM GetOrCreateItem(int index)
		{
			while (this._items.Count <= index)
			{
				this._items.Add(new BattlefieldHealthBarItemVM());
			}
			return this._items[index];
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00003754 File Offset: 0x00001954
		public BattlefieldDamageNumberItemVM GetOrCreateDamageNumberItem(int index)
		{
			while (this._damageNumbers.Count <= index)
			{
				this._damageNumbers.Add(new BattlefieldDamageNumberItemVM());
			}
			return this._damageNumbers[index];
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00003784 File Offset: 0x00001984
		public void HideFrom(int index)
		{
			for (int i = index; i < this._items.Count; i++)
			{
				this._items[i].Hide();
			}
		}

		// Token: 0x0600005B RID: 91 RVA: 0x000037B8 File Offset: 0x000019B8
		public void HideAll()
		{
			this.IsEnabled = false;
			this.HideFrom(0);
			this.HideAllDamageNumbers();
		}

		// Token: 0x0600005C RID: 92 RVA: 0x000037D0 File Offset: 0x000019D0
		public void HideAllDamageNumbers()
		{
			foreach (BattlefieldDamageNumberItemVM battlefieldDamageNumberItemVM in this._damageNumbers)
			{
				battlefieldDamageNumberItemVM.Hide();
			}
		}

		// Token: 0x0600005D RID: 93 RVA: 0x0000381C File Offset: 0x00001A1C
		public override void OnFinalize()
		{
			foreach (BattlefieldHealthBarItemVM battlefieldHealthBarItemVM in this._items)
			{
				battlefieldHealthBarItemVM.OnFinalize();
			}
			foreach (BattlefieldDamageNumberItemVM battlefieldDamageNumberItemVM in this._damageNumbers)
			{
				battlefieldDamageNumberItemVM.OnFinalize();
			}
			base.OnFinalize();
		}

		// Token: 0x04000027 RID: 39
		private readonly MBBindingList<BattlefieldHealthBarItemVM> _items = new MBBindingList<BattlefieldHealthBarItemVM>();

		// Token: 0x04000028 RID: 40
		private readonly MBBindingList<BattlefieldDamageNumberItemVM> _damageNumbers = new MBBindingList<BattlefieldDamageNumberItemVM>();

		// Token: 0x04000029 RID: 41
		private bool _isEnabled;
	}
}
