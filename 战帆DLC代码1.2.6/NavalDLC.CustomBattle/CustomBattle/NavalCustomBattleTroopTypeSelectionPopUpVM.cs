using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace NavalDLC.CustomBattle.CustomBattle
{
	// Token: 0x02000019 RID: 25
	public class NavalCustomBattleTroopTypeSelectionPopUpVM : ViewModel
	{
		// Token: 0x0600015E RID: 350 RVA: 0x000077B0 File Offset: 0x000059B0
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.DoneLbl = GameTexts.FindText("str_done", null).ToString();
			this.CancelLbl = GameTexts.FindText("str_cancel", null).ToString();
			this.SelectAllLbl = GameTexts.FindText("str_custom_battle_select_all", null).ToString();
			this.BackToDefaultLbl = GameTexts.FindText("str_custom_battle_back_to_default", null).ToString();
		}

		// Token: 0x0600015F RID: 351 RVA: 0x0000781B File Offset: 0x00005A1B
		public override void OnFinalize()
		{
			base.OnFinalize();
			this.DoneInputKey.OnFinalize();
			this.CancelInputKey.OnFinalize();
			this.ResetInputKey.OnFinalize();
		}

		// Token: 0x06000160 RID: 352 RVA: 0x00007844 File Offset: 0x00005A44
		public void OpenPopUp(string title, MBBindingList<NavalCustomBattleTroopTypeVM> troops)
		{
			this._itemSelectionsBackUp = new List<bool>();
			foreach (NavalCustomBattleTroopTypeVM navalCustomBattleTroopTypeVM in troops)
			{
				this._itemSelectionsBackUp.Add(navalCustomBattleTroopTypeVM.IsSelected);
			}
			this._selectedItemCount = troops.Count<NavalCustomBattleTroopTypeVM>((NavalCustomBattleTroopTypeVM x) => x.IsSelected);
			this.Title = title;
			this.Items = troops;
			this.IsOpen = true;
		}

		// Token: 0x06000161 RID: 353 RVA: 0x000078E4 File Offset: 0x00005AE4
		public void OnItemSelectionToggled(NavalCustomBattleTroopTypeVM item)
		{
			if (this._selectedItemCount > 1 || !item.IsSelected)
			{
				item.IsSelected = !item.IsSelected;
				this._selectedItemCount += (item.IsSelected ? 1 : (-1));
			}
		}

		// Token: 0x06000162 RID: 354 RVA: 0x00007930 File Offset: 0x00005B30
		public void ExecuteSelectAll()
		{
			this.Items.ApplyActionOnAllItems(delegate(NavalCustomBattleTroopTypeVM x)
			{
				x.IsSelected = true;
			});
			this._selectedItemCount = this.Items.Count;
		}

		// Token: 0x06000163 RID: 355 RVA: 0x00007970 File Offset: 0x00005B70
		public void ExecuteBackToDefault()
		{
			this.Items.ApplyActionOnAllItems(delegate(NavalCustomBattleTroopTypeVM x)
			{
				x.IsSelected = x.IsDefault;
			});
			this._selectedItemCount = this.Items.Count<NavalCustomBattleTroopTypeVM>((NavalCustomBattleTroopTypeVM x) => x.IsSelected);
		}

		// Token: 0x06000164 RID: 356 RVA: 0x000079D7 File Offset: 0x00005BD7
		public void ExecuteCancel()
		{
			this.ExecuteReset();
			Action onPopUpClosed = this.OnPopUpClosed;
			if (onPopUpClosed != null)
			{
				onPopUpClosed();
			}
			this.IsOpen = false;
		}

		// Token: 0x06000165 RID: 357 RVA: 0x000079F7 File Offset: 0x00005BF7
		public void ExecuteDone()
		{
			this.IsOpen = false;
		}

		// Token: 0x06000166 RID: 358 RVA: 0x00007A00 File Offset: 0x00005C00
		public void ExecuteReset()
		{
			int count = this._itemSelectionsBackUp.Count;
			if (count != this.Items.Count)
			{
				Debug.FailedAssert("Backup troop count does not match with the actual troop count.", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.CustomBattle\\CustomBattle\\NavalCustomBattleTroopTypeSelectionPopUpVM.cs", "ExecuteReset", 99);
				return;
			}
			for (int i = 0; i < count; i++)
			{
				this.Items[i].IsSelected = this._itemSelectionsBackUp[i];
			}
			this._selectedItemCount = this.Items.Count<NavalCustomBattleTroopTypeVM>((NavalCustomBattleTroopTypeVM x) => x.IsSelected);
		}

		// Token: 0x06000167 RID: 359 RVA: 0x00007A97 File Offset: 0x00005C97
		public void SetCancelInputKey(HotKey hotkey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		// Token: 0x06000168 RID: 360 RVA: 0x00007AA6 File Offset: 0x00005CA6
		public void SetDoneInputKey(HotKey hotkey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		// Token: 0x06000169 RID: 361 RVA: 0x00007AB5 File Offset: 0x00005CB5
		public void SetResetInputKey(HotKey hotkey)
		{
			this.ResetInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x0600016A RID: 362 RVA: 0x00007AC4 File Offset: 0x00005CC4
		// (set) Token: 0x0600016B RID: 363 RVA: 0x00007ACC File Offset: 0x00005CCC
		[DataSourceProperty]
		public InputKeyItemVM DoneInputKey
		{
			get
			{
				return this._doneInputKey;
			}
			set
			{
				if (value != this._doneInputKey)
				{
					this._doneInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "DoneInputKey");
				}
			}
		}

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x0600016C RID: 364 RVA: 0x00007AEA File Offset: 0x00005CEA
		// (set) Token: 0x0600016D RID: 365 RVA: 0x00007AF2 File Offset: 0x00005CF2
		[DataSourceProperty]
		public InputKeyItemVM CancelInputKey
		{
			get
			{
				return this._cancelInputKey;
			}
			set
			{
				if (value != this._cancelInputKey)
				{
					this._cancelInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "CancelInputKey");
				}
			}
		}

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x0600016E RID: 366 RVA: 0x00007B10 File Offset: 0x00005D10
		// (set) Token: 0x0600016F RID: 367 RVA: 0x00007B18 File Offset: 0x00005D18
		[DataSourceProperty]
		public InputKeyItemVM ResetInputKey
		{
			get
			{
				return this._resetInputKey;
			}
			set
			{
				if (value != this._resetInputKey)
				{
					this._resetInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "ResetInputKey");
				}
			}
		}

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x06000170 RID: 368 RVA: 0x00007B36 File Offset: 0x00005D36
		// (set) Token: 0x06000171 RID: 369 RVA: 0x00007B3E File Offset: 0x00005D3E
		[DataSourceProperty]
		public MBBindingList<NavalCustomBattleTroopTypeVM> Items
		{
			get
			{
				return this._items;
			}
			set
			{
				if (value != this._items)
				{
					this._items = value;
					base.OnPropertyChangedWithValue<MBBindingList<NavalCustomBattleTroopTypeVM>>(value, "Items");
				}
			}
		}

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x06000172 RID: 370 RVA: 0x00007B5C File Offset: 0x00005D5C
		// (set) Token: 0x06000173 RID: 371 RVA: 0x00007B64 File Offset: 0x00005D64
		[DataSourceProperty]
		public string Title
		{
			get
			{
				return this._title;
			}
			set
			{
				if (value != this._title)
				{
					this._title = value;
					base.OnPropertyChangedWithValue<string>(value, "Title");
				}
			}
		}

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x06000174 RID: 372 RVA: 0x00007B87 File Offset: 0x00005D87
		// (set) Token: 0x06000175 RID: 373 RVA: 0x00007B8F File Offset: 0x00005D8F
		[DataSourceProperty]
		public string DoneLbl
		{
			get
			{
				return this._doneLbl;
			}
			set
			{
				if (value != this._doneLbl)
				{
					this._doneLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "DoneLbl");
				}
			}
		}

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x06000176 RID: 374 RVA: 0x00007BB2 File Offset: 0x00005DB2
		// (set) Token: 0x06000177 RID: 375 RVA: 0x00007BBA File Offset: 0x00005DBA
		[DataSourceProperty]
		public string CancelLbl
		{
			get
			{
				return this._cancelLbl;
			}
			set
			{
				if (value != this._cancelLbl)
				{
					this._cancelLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "CancelLbl");
				}
			}
		}

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x06000178 RID: 376 RVA: 0x00007BDD File Offset: 0x00005DDD
		// (set) Token: 0x06000179 RID: 377 RVA: 0x00007BE5 File Offset: 0x00005DE5
		[DataSourceProperty]
		public string SelectAllLbl
		{
			get
			{
				return this._selectAllLbl;
			}
			set
			{
				if (value != this._selectAllLbl)
				{
					this._selectAllLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "SelectAllLbl");
				}
			}
		}

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x0600017A RID: 378 RVA: 0x00007C08 File Offset: 0x00005E08
		// (set) Token: 0x0600017B RID: 379 RVA: 0x00007C10 File Offset: 0x00005E10
		[DataSourceProperty]
		public string BackToDefaultLbl
		{
			get
			{
				return this._backToDefaultLbl;
			}
			set
			{
				if (value != this._backToDefaultLbl)
				{
					this._backToDefaultLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "BackToDefaultLbl");
				}
			}
		}

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x0600017C RID: 380 RVA: 0x00007C33 File Offset: 0x00005E33
		// (set) Token: 0x0600017D RID: 381 RVA: 0x00007C3B File Offset: 0x00005E3B
		[DataSourceProperty]
		public bool IsOpen
		{
			get
			{
				return this._isOpen;
			}
			set
			{
				if (value != this._isOpen)
				{
					this._isOpen = value;
					base.OnPropertyChangedWithValue(value, "IsOpen");
				}
			}
		}

		// Token: 0x040000B3 RID: 179
		public Action OnPopUpClosed;

		// Token: 0x040000B4 RID: 180
		private List<bool> _itemSelectionsBackUp;

		// Token: 0x040000B5 RID: 181
		private int _selectedItemCount;

		// Token: 0x040000B6 RID: 182
		private InputKeyItemVM _doneInputKey;

		// Token: 0x040000B7 RID: 183
		private InputKeyItemVM _cancelInputKey;

		// Token: 0x040000B8 RID: 184
		private InputKeyItemVM _resetInputKey;

		// Token: 0x040000B9 RID: 185
		private MBBindingList<NavalCustomBattleTroopTypeVM> _items;

		// Token: 0x040000BA RID: 186
		private string _title;

		// Token: 0x040000BB RID: 187
		private string _doneLbl;

		// Token: 0x040000BC RID: 188
		private string _cancelLbl;

		// Token: 0x040000BD RID: 189
		private string _selectAllLbl;

		// Token: 0x040000BE RID: 190
		private string _backToDefaultLbl;

		// Token: 0x040000BF RID: 191
		private bool _isOpen;
	}
}
