using System;
using NavalDLC.CustomBattle.CustomBattle.SelectionItem;
using SandBox.ViewModelCollection.Input;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace NavalDLC.CustomBattle.CustomBattle
{
	// Token: 0x02000016 RID: 22
	public class NavalCustomBattleShipSelectionItemVM : ViewModel
	{
		// Token: 0x06000106 RID: 262 RVA: 0x000068C4 File Offset: 0x00004AC4
		public NavalCustomBattleShipSelectionItemVM(bool isPlayerSide, NavalCustomBattleShipSelectionPopUpVM shipSelectionPopUp, Action onShipSelectedOrUpgraded, Action<NavalCustomBattleShipItemVM> onShipFocused)
		{
			this._isPlayerSide = isPlayerSide;
			this._shipSelectionPopUp = shipSelectionPopUp;
			this._onShipSelectedOrUpgraded = onShipSelectedOrUpgraded;
			this._onShipFocused = onShipFocused;
			this.ClearShipHint = new HintViewModel(new TextObject("{=On45SbIp}Clear ship", null), null);
			this.NotRelevantHint = new HintViewModel(new TextObject("{=VpQVzOiV}Limited in selected game mode", null), null);
			this.UpdateIsSelectedItemEligible();
			this.IsRelevant = true;
		}

		// Token: 0x06000107 RID: 263 RVA: 0x0000692F File Offset: 0x00004B2F
		public override void RefreshValues()
		{
			base.RefreshValues();
			NavalCustomBattleShipItemVM selectedItem = this.SelectedItem;
			if (selectedItem != null)
			{
				selectedItem.RefreshValues();
			}
			InputKeyItemVM cycleTierInputKey = this.CycleTierInputKey;
			if (cycleTierInputKey == null)
			{
				return;
			}
			cycleTierInputKey.RefreshValues();
		}

		// Token: 0x06000108 RID: 264 RVA: 0x00006958 File Offset: 0x00004B58
		public void SetHull(ShipHull shipHull)
		{
			if (shipHull == null)
			{
				this.SelectedItem = null;
			}
			else
			{
				NavalCustomBattleShipItemVM selectedItem = this.SelectedItem;
				if (shipHull != ((selectedItem != null) ? selectedItem.ShipHull : null))
				{
					this.SelectedItem = new NavalCustomBattleShipItemVM(shipHull, this._isPlayerSide, this._onShipSelectedOrUpgraded);
				}
			}
			Action onShipSelectedOrUpgraded = this._onShipSelectedOrUpgraded;
			if (onShipSelectedOrUpgraded != null)
			{
				onShipSelectedOrUpgraded();
			}
			this.UpdateIsSelectedItemEligible();
		}

		// Token: 0x06000109 RID: 265 RVA: 0x000069B5 File Offset: 0x00004BB5
		public void ExecuteClearShip()
		{
			this.OnConfirm(null);
		}

		// Token: 0x0600010A RID: 266 RVA: 0x000069C0 File Offset: 0x00004BC0
		public void ExecuteOpenPopUp()
		{
			bool flag = !this.HasSelectedItem || this.CanBecomeEmpty;
			NavalCustomBattleShipSelectionPopUpVM shipSelectionPopUp = this._shipSelectionPopUp;
			string text = new TextObject("{=QVlyuUu6}Select Ship", null).ToString();
			NavalCustomBattleShipItemVM selectedItem = this.SelectedItem;
			shipSelectionPopUp.OpenPopUp(text, (selectedItem != null) ? selectedItem.ShipHull : null, flag, new Func<ShipHull, bool>(this.GetIsHullDisabled), new Action<ShipHull>(this.OnConfirm));
		}

		// Token: 0x0600010B RID: 267 RVA: 0x00006A25 File Offset: 0x00004C25
		private bool GetIsHullDisabled(ShipHull shipHull)
		{
			return this.IsRaid && !NavalCustomBattleHelper.CanShipHullBeUsedInRaid(shipHull);
		}

		// Token: 0x0600010C RID: 268 RVA: 0x00006A3A File Offset: 0x00004C3A
		public void ExecuteHoverBegin()
		{
			this.IsHovered = true;
			Action<NavalCustomBattleShipItemVM> onShipFocused = this._onShipFocused;
			if (onShipFocused == null)
			{
				return;
			}
			onShipFocused(this.SelectedItem);
		}

		// Token: 0x0600010D RID: 269 RVA: 0x00006A59 File Offset: 0x00004C59
		public void ExecuteHoverEnd()
		{
			this.IsHovered = false;
			Action<NavalCustomBattleShipItemVM> onShipFocused = this._onShipFocused;
			if (onShipFocused == null)
			{
				return;
			}
			onShipFocused(null);
		}

		// Token: 0x0600010E RID: 270 RVA: 0x00006A73 File Offset: 0x00004C73
		private void OnConfirm(ShipHull selectedHull)
		{
			this.SetHull(selectedHull);
		}

		// Token: 0x0600010F RID: 271 RVA: 0x00006A7C File Offset: 0x00004C7C
		public override void OnFinalize()
		{
			base.OnFinalize();
			InputKeyItemVM cycleTierInputKey = this.CycleTierInputKey;
			if (cycleTierInputKey == null)
			{
				return;
			}
			cycleTierInputKey.OnFinalize();
		}

		// Token: 0x06000110 RID: 272 RVA: 0x00006A94 File Offset: 0x00004C94
		private void UpdateIsSelectedItemEligible()
		{
			if (!this.HasSelectedItem)
			{
				this.IsSelectedItemEligible = true;
				return;
			}
			this.IsSelectedItemEligible = !this.GetIsHullDisabled(this.SelectedItem.ShipHull);
			this.SelectedItem.DisabledHint = (this.IsSelectedItemEligible ? null : new HintViewModel(new TextObject("{=d3WMrFKo}Not usable in selected game mode", null), null));
		}

		// Token: 0x06000111 RID: 273 RVA: 0x00006AF2 File Offset: 0x00004CF2
		public void SetCycleTierInputKey(HotKey hotkey)
		{
			this.CycleTierInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x06000112 RID: 274 RVA: 0x00006B01 File Offset: 0x00004D01
		// (set) Token: 0x06000113 RID: 275 RVA: 0x00006B09 File Offset: 0x00004D09
		public InputKeyItemVM CycleTierInputKey
		{
			get
			{
				return this._cycleTierInputKey;
			}
			set
			{
				if (value != this._cycleTierInputKey)
				{
					this._cycleTierInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "CycleTierInputKey");
				}
			}
		}

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x06000114 RID: 276 RVA: 0x00006B27 File Offset: 0x00004D27
		// (set) Token: 0x06000115 RID: 277 RVA: 0x00006B2F File Offset: 0x00004D2F
		[DataSourceProperty]
		public bool IsRelevant
		{
			get
			{
				return this._isRelevant;
			}
			set
			{
				if (value != this._isRelevant)
				{
					this._isRelevant = value;
					base.OnPropertyChangedWithValue(value, "IsRelevant");
				}
			}
		}

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x06000116 RID: 278 RVA: 0x00006B4D File Offset: 0x00004D4D
		// (set) Token: 0x06000117 RID: 279 RVA: 0x00006B55 File Offset: 0x00004D55
		[DataSourceProperty]
		public bool IsHovered
		{
			get
			{
				return this._isHovered;
			}
			set
			{
				if (value != this._isHovered)
				{
					this._isHovered = value;
					base.OnPropertyChangedWithValue(value, "IsHovered");
				}
			}
		}

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x06000118 RID: 280 RVA: 0x00006B73 File Offset: 0x00004D73
		// (set) Token: 0x06000119 RID: 281 RVA: 0x00006B7B File Offset: 0x00004D7B
		[DataSourceProperty]
		public bool IsSelectedItemEligible
		{
			get
			{
				return this._isSelectedItemEligible;
			}
			set
			{
				if (value != this._isSelectedItemEligible)
				{
					this._isSelectedItemEligible = value;
					base.OnPropertyChangedWithValue(value, "IsSelectedItemEligible");
				}
			}
		}

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x0600011A RID: 282 RVA: 0x00006B99 File Offset: 0x00004D99
		// (set) Token: 0x0600011B RID: 283 RVA: 0x00006BA1 File Offset: 0x00004DA1
		[DataSourceProperty]
		public bool HasSelectedItem
		{
			get
			{
				return this._hasSelectedItem;
			}
			set
			{
				if (value != this._hasSelectedItem)
				{
					this._hasSelectedItem = value;
					base.OnPropertyChangedWithValue(value, "HasSelectedItem");
				}
			}
		}

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x0600011C RID: 284 RVA: 0x00006BBF File Offset: 0x00004DBF
		// (set) Token: 0x0600011D RID: 285 RVA: 0x00006BC7 File Offset: 0x00004DC7
		[DataSourceProperty]
		public bool CanBecomeEmpty
		{
			get
			{
				return this._canBecomeEmpty;
			}
			set
			{
				if (value != this._canBecomeEmpty)
				{
					this._canBecomeEmpty = value;
					base.OnPropertyChangedWithValue(value, "CanBecomeEmpty");
				}
			}
		}

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x0600011E RID: 286 RVA: 0x00006BE5 File Offset: 0x00004DE5
		// (set) Token: 0x0600011F RID: 287 RVA: 0x00006BED File Offset: 0x00004DED
		[DataSourceProperty]
		public bool IsRaid
		{
			get
			{
				return this._isRaid;
			}
			set
			{
				if (value != this._isRaid)
				{
					this._isRaid = value;
					base.OnPropertyChangedWithValue(value, "IsRaid");
					this.UpdateIsSelectedItemEligible();
				}
			}
		}

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x06000120 RID: 288 RVA: 0x00006C11 File Offset: 0x00004E11
		// (set) Token: 0x06000121 RID: 289 RVA: 0x00006C19 File Offset: 0x00004E19
		[DataSourceProperty]
		public NavalCustomBattleShipItemVM SelectedItem
		{
			get
			{
				return this._selectedItem;
			}
			set
			{
				if (value != this._selectedItem)
				{
					this._selectedItem = value;
					base.OnPropertyChangedWithValue<NavalCustomBattleShipItemVM>(value, "SelectedItem");
					this.HasSelectedItem = this._selectedItem != null;
				}
			}
		}

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x06000122 RID: 290 RVA: 0x00006C46 File Offset: 0x00004E46
		// (set) Token: 0x06000123 RID: 291 RVA: 0x00006C4E File Offset: 0x00004E4E
		[DataSourceProperty]
		public HintViewModel ClearShipHint
		{
			get
			{
				return this._clearShipHint;
			}
			set
			{
				if (value != this._clearShipHint)
				{
					this._clearShipHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ClearShipHint");
				}
			}
		}

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x06000124 RID: 292 RVA: 0x00006C6C File Offset: 0x00004E6C
		// (set) Token: 0x06000125 RID: 293 RVA: 0x00006C74 File Offset: 0x00004E74
		[DataSourceProperty]
		public HintViewModel NotRelevantHint
		{
			get
			{
				return this._notRelevantHint;
			}
			set
			{
				if (value != this._notRelevantHint)
				{
					this._notRelevantHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "NotRelevantHint");
				}
			}
		}

		// Token: 0x0400008D RID: 141
		private readonly bool _isPlayerSide;

		// Token: 0x0400008E RID: 142
		private readonly NavalCustomBattleShipSelectionPopUpVM _shipSelectionPopUp;

		// Token: 0x0400008F RID: 143
		private readonly Action _onShipSelectedOrUpgraded;

		// Token: 0x04000090 RID: 144
		private readonly Action<NavalCustomBattleShipItemVM> _onShipFocused;

		// Token: 0x04000091 RID: 145
		private InputKeyItemVM _cycleTierInputKey;

		// Token: 0x04000092 RID: 146
		private bool _isRelevant;

		// Token: 0x04000093 RID: 147
		private bool _isHovered;

		// Token: 0x04000094 RID: 148
		private bool _isSelectedItemEligible;

		// Token: 0x04000095 RID: 149
		private bool _hasSelectedItem;

		// Token: 0x04000096 RID: 150
		private bool _canBecomeEmpty;

		// Token: 0x04000097 RID: 151
		private bool _isRaid;

		// Token: 0x04000098 RID: 152
		private NavalCustomBattleShipItemVM _selectedItem;

		// Token: 0x04000099 RID: 153
		private HintViewModel _clearShipHint;

		// Token: 0x0400009A RID: 154
		private HintViewModel _notRelevantHint;
	}
}
