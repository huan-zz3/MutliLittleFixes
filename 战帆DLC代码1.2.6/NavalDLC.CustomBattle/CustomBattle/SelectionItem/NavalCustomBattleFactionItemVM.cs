using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace NavalDLC.CustomBattle.CustomBattle.SelectionItem
{
	// Token: 0x0200001D RID: 29
	public class NavalCustomBattleFactionItemVM : ViewModel
	{
		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x060001D4 RID: 468 RVA: 0x00008F07 File Offset: 0x00007107
		// (set) Token: 0x060001D5 RID: 469 RVA: 0x00008F0F File Offset: 0x0000710F
		public BasicCultureObject Faction { get; private set; }

		// Token: 0x060001D6 RID: 470 RVA: 0x00008F18 File Offset: 0x00007118
		public NavalCustomBattleFactionItemVM(BasicCultureObject faction, Action<NavalCustomBattleFactionItemVM> onSelected)
		{
			this.Faction = faction;
			this._onSelected = onSelected;
			this.CultureCode = faction.StringId.ToLower();
			this.Hint = new HintViewModel(faction.Name, null);
		}

		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x060001D7 RID: 471 RVA: 0x00008F51 File Offset: 0x00007151
		// (set) Token: 0x060001D8 RID: 472 RVA: 0x00008F59 File Offset: 0x00007159
		[DataSourceProperty]
		public HintViewModel Hint
		{
			get
			{
				return this._hint;
			}
			set
			{
				if (value != this._hint)
				{
					this._hint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "Hint");
				}
			}
		}

		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x060001D9 RID: 473 RVA: 0x00008F77 File Offset: 0x00007177
		// (set) Token: 0x060001DA RID: 474 RVA: 0x00008F7F File Offset: 0x0000717F
		[DataSourceProperty]
		public string CultureCode
		{
			get
			{
				return this._cultureCode;
			}
			set
			{
				if (value != this._cultureCode)
				{
					this._cultureCode = value;
					base.OnPropertyChangedWithValue<string>(value, "CultureCode");
				}
			}
		}

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x060001DB RID: 475 RVA: 0x00008FA2 File Offset: 0x000071A2
		// (set) Token: 0x060001DC RID: 476 RVA: 0x00008FAA File Offset: 0x000071AA
		[DataSourceProperty]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
					if (value)
					{
						this._onSelected(this);
					}
				}
			}
		}

		// Token: 0x040000E2 RID: 226
		private Action<NavalCustomBattleFactionItemVM> _onSelected;

		// Token: 0x040000E3 RID: 227
		private HintViewModel _hint;

		// Token: 0x040000E4 RID: 228
		private string _cultureCode;

		// Token: 0x040000E5 RID: 229
		private bool _isSelected;
	}
}
