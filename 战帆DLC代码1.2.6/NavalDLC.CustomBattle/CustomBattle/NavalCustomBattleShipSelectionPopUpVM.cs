using System;
using NavalDLC.CustomBattle.CustomBattle.SelectionItem;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace NavalDLC.CustomBattle.CustomBattle
{
	// Token: 0x02000017 RID: 23
	public class NavalCustomBattleShipSelectionPopUpVM : ViewModel
	{
		// Token: 0x06000126 RID: 294 RVA: 0x00006C94 File Offset: 0x00004E94
		public NavalCustomBattleShipSelectionPopUpVM()
		{
			MBBindingList<NavalCustomBattleShipHullItemVM> mbbindingList = new MBBindingList<NavalCustomBattleShipHullItemVM>();
			mbbindingList.Add(new NavalCustomBattleShipHullItemVM(new TextObject("{=koX9okuG}None", null), new TextObject("{=fNyb979i}Must have at least one ship", null), new Action<NavalCustomBattleShipHullItemVM>(this.OnShipHullSelected)));
			this.Items = mbbindingList;
			foreach (ShipHull shipHull in NavalCustomBattleData.ShipHulls)
			{
				this.Items.Add(new NavalCustomBattleShipHullItemVM(shipHull, new TextObject("{=d3WMrFKo}Not usable in selected game mode", null), new Action<NavalCustomBattleShipHullItemVM>(this.OnShipHullSelected)));
			}
		}

		// Token: 0x06000127 RID: 295 RVA: 0x00006D40 File Offset: 0x00004F40
		public override void OnFinalize()
		{
			base.OnFinalize();
			this.CloseInputKey.OnFinalize();
		}

		// Token: 0x06000128 RID: 296 RVA: 0x00006D54 File Offset: 0x00004F54
		public void OpenPopUp(string title, ShipHull selectedItem, bool canSelectEmpty, Func<ShipHull, bool> getIsHullDisabled, Action<ShipHull> onConfirm)
		{
			this.Title = title;
			this.IsOpen = true;
			this._onConfirm = onConfirm;
			this.Items.ApplyActionOnAllItems(delegate(NavalCustomBattleShipHullItemVM item)
			{
				item.IsSelected = item.ShipHull == selectedItem;
				bool flag;
				if (item.ShipHull != null || canSelectEmpty)
				{
					if (item.ShipHull != null)
					{
						Func<ShipHull, bool> getIsHullDisabled2 = getIsHullDisabled;
						flag = getIsHullDisabled2 != null && getIsHullDisabled2(item.ShipHull);
					}
					else
					{
						flag = false;
					}
				}
				else
				{
					flag = true;
				}
				item.IsDisabled = flag;
			});
		}

		// Token: 0x06000129 RID: 297 RVA: 0x00006DAA File Offset: 0x00004FAA
		public void ExecuteClose()
		{
			this.IsOpen = false;
			this._onConfirm = null;
		}

		// Token: 0x0600012A RID: 298 RVA: 0x00006DBA File Offset: 0x00004FBA
		private void OnShipHullSelected(NavalCustomBattleShipHullItemVM item)
		{
			Action<ShipHull> onConfirm = this._onConfirm;
			if (onConfirm != null)
			{
				onConfirm((item != null) ? item.ShipHull : null);
			}
			this.IsOpen = false;
			this._onConfirm = null;
		}

		// Token: 0x0600012B RID: 299 RVA: 0x00006DE7 File Offset: 0x00004FE7
		public void SetCloseInputKey(HotKey hotkey)
		{
			this.CloseInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x0600012C RID: 300 RVA: 0x00006DF6 File Offset: 0x00004FF6
		// (set) Token: 0x0600012D RID: 301 RVA: 0x00006DFE File Offset: 0x00004FFE
		[DataSourceProperty]
		public InputKeyItemVM CloseInputKey
		{
			get
			{
				return this._closeInputKey;
			}
			set
			{
				if (value != this._closeInputKey)
				{
					this._closeInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "CloseInputKey");
				}
			}
		}

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x0600012E RID: 302 RVA: 0x00006E1C File Offset: 0x0000501C
		// (set) Token: 0x0600012F RID: 303 RVA: 0x00006E24 File Offset: 0x00005024
		[DataSourceProperty]
		public MBBindingList<NavalCustomBattleShipHullItemVM> Items
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
					base.OnPropertyChangedWithValue<MBBindingList<NavalCustomBattleShipHullItemVM>>(value, "Items");
				}
			}
		}

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x06000130 RID: 304 RVA: 0x00006E42 File Offset: 0x00005042
		// (set) Token: 0x06000131 RID: 305 RVA: 0x00006E4A File Offset: 0x0000504A
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

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x06000132 RID: 306 RVA: 0x00006E6D File Offset: 0x0000506D
		// (set) Token: 0x06000133 RID: 307 RVA: 0x00006E75 File Offset: 0x00005075
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

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x06000134 RID: 308 RVA: 0x00006E98 File Offset: 0x00005098
		// (set) Token: 0x06000135 RID: 309 RVA: 0x00006EA0 File Offset: 0x000050A0
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

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x06000136 RID: 310 RVA: 0x00006EC3 File Offset: 0x000050C3
		// (set) Token: 0x06000137 RID: 311 RVA: 0x00006ECB File Offset: 0x000050CB
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

		// Token: 0x0400009B RID: 155
		private Action<ShipHull> _onConfirm;

		// Token: 0x0400009C RID: 156
		private InputKeyItemVM _closeInputKey;

		// Token: 0x0400009D RID: 157
		private MBBindingList<NavalCustomBattleShipHullItemVM> _items;

		// Token: 0x0400009E RID: 158
		private string _title;

		// Token: 0x0400009F RID: 159
		private string _doneLbl;

		// Token: 0x040000A0 RID: 160
		private string _cancelLbl;

		// Token: 0x040000A1 RID: 161
		private bool _isOpen;
	}
}
