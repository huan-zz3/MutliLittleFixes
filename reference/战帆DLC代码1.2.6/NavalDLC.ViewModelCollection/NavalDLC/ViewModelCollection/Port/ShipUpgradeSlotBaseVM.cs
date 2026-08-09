using System;
using System.Collections.Generic;
using NavalDLC.ViewModelCollection.Port.PortScreenHandlers;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace NavalDLC.ViewModelCollection.Port
{
	// Token: 0x02000015 RID: 21
	public class ShipUpgradeSlotBaseVM : ViewModel
	{
		// Token: 0x14000009 RID: 9
		// (add) Token: 0x06000198 RID: 408 RVA: 0x0000A328 File Offset: 0x00008528
		// (remove) Token: 0x06000199 RID: 409 RVA: 0x0000A35C File Offset: 0x0000855C
		public static event ShipUpgradeSlotBaseVM.ShipPieceSelectedDelegate OnShipPieceSelected;

		// Token: 0x0600019A RID: 410 RVA: 0x0000A390 File Offset: 0x00008590
		public ShipUpgradeSlotBaseVM(Ship ship, TextObject slotName, string shipSlotTag, string slotTypeId, Action<ShipUpgradeSlotBaseVM> onSelected)
		{
			this._onSelected = onSelected;
			this.Ship = ship;
			this.ShipSlotTag = shipSlotTag;
			this.SlotTypeId = slotTypeId;
			this._nameText = slotName;
			this.AvailablePieces = new MBBindingList<ShipUpgradePieceBaseVM>();
			this.SlotHint = new HintViewModel();
			this.ClearSlotHint = new HintViewModel(new TextObject("{=pJgyBSz7}Clear Slot", null), null);
			this.IsChanged = false;
			this.RefreshValues();
		}

		// Token: 0x0600019B RID: 411 RVA: 0x0000A40C File Offset: 0x0000860C
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.AvailablePieces.ApplyActionOnAllItems(delegate(ShipUpgradePieceBaseVM p)
			{
				p.RefreshValues();
			});
			this.SlotName = this._nameText.ToString();
		}

		// Token: 0x0600019C RID: 412 RVA: 0x0000A45C File Offset: 0x0000865C
		public override void OnFinalize()
		{
			base.OnFinalize();
			this.AvailablePieces.ApplyActionOnAllItems(delegate(ShipUpgradePieceBaseVM p)
			{
				p.OnFinalize();
			});
			if (this.SelectedPiece != null && !this.AvailablePieces.Contains(this.SelectedPiece))
			{
				this.SelectedPiece.OnFinalize();
			}
		}

		// Token: 0x0600019D RID: 413 RVA: 0x0000A4C0 File Offset: 0x000086C0
		public void Update()
		{
			bool flag = false;
			for (int i = 0; i < this.AvailablePieces.Count; i++)
			{
				this.AvailablePieces[i].Update();
				if (this.AvailablePieces[i].IsUnexamined)
				{
					flag = true;
				}
			}
			ShipUpgradePieceBaseVM selectedPiece = this.SelectedPiece;
			if (selectedPiece != null)
			{
				selectedPiece.Update();
			}
			ShipUpgradePieceBaseVM selectedPiece2 = this.SelectedPiece;
			if (selectedPiece2 != null && selectedPiece2.IsUnexamined)
			{
				flag = true;
			}
			this.IsUnexamined = flag;
		}

		// Token: 0x0600019E RID: 414 RVA: 0x0000A539 File Offset: 0x00008739
		public virtual void ResetPieces()
		{
		}

		// Token: 0x0600019F RID: 415 RVA: 0x0000A53C File Offset: 0x0000873C
		public void UpdateEnabledStatus(in PortActionInfo actionInfo)
		{
			this.CanTradeUpgrades = actionInfo.IsEnabled;
			this._actionInfoTooltip = actionInfo.Tooltip;
			this.SlotHint.HintText = ((this.SelectedPiece == null) ? this._actionInfoTooltip : null);
			if (this.CanTradeUpgrades && this.AvailablePieces.Count == 0 && this.SelectedPiece == null)
			{
				this.CanTradeUpgrades = false;
				this.SlotHint.HintText = new TextObject("{=s96ObCLT}There are no available upgrades for this slot", null);
			}
			if (!this.CanTradeUpgrades && this.IsSelected)
			{
				this.ExecuteDeselect();
			}
			if (TextObject.IsNullOrEmpty(this.SlotHint.HintText) && this.SelectedPiece == null)
			{
				this.SlotHint.HintText = new TextObject("{=!}" + this.SlotName, null);
			}
		}

		// Token: 0x060001A0 RID: 416 RVA: 0x0000A608 File Offset: 0x00008808
		protected virtual void OnPieceSelected(ShipUpgradePieceBaseVM piece)
		{
			this.SelectedPiece = piece;
			ShipUpgradeSlotBaseVM.ShipPieceSelectedDelegate onShipPieceSelected = ShipUpgradeSlotBaseVM.OnShipPieceSelected;
			if (onShipPieceSelected == null)
			{
				return;
			}
			onShipPieceSelected(this.Ship, this.ShipSlotTag, this.SlotTypeId, this.SelectedPiece);
		}

		// Token: 0x060001A1 RID: 417 RVA: 0x0000A638 File Offset: 0x00008838
		protected virtual bool GetIsChanged()
		{
			return false;
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x0000A63B File Offset: 0x0000883B
		public void ExecuteClearSlot()
		{
			this.ExecuteInspectEnd();
			this.OnPieceSelected(null);
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x0000A64A File Offset: 0x0000884A
		public void ExecuteSelect()
		{
			Action<ShipUpgradeSlotBaseVM> onSelected = this._onSelected;
			if (onSelected == null)
			{
				return;
			}
			onSelected(this);
		}

		// Token: 0x060001A4 RID: 420 RVA: 0x0000A65D File Offset: 0x0000885D
		public void ExecuteDeselect()
		{
			Action<ShipUpgradeSlotBaseVM> onSelected = this._onSelected;
			if (onSelected == null)
			{
				return;
			}
			onSelected(null);
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x0000A670 File Offset: 0x00008870
		public void ExecuteInspectBegin()
		{
			ShipUpgradePieceBaseVM selectedPiece = this.SelectedPiece;
			if (selectedPiece == null)
			{
				return;
			}
			selectedPiece.InspectPiece(true, this._actionInfoTooltip);
		}

		// Token: 0x060001A6 RID: 422 RVA: 0x0000A689 File Offset: 0x00008889
		public void ExecuteInspectEnd()
		{
			ShipUpgradePieceBaseVM selectedPiece = this.SelectedPiece;
			if (selectedPiece == null)
			{
				return;
			}
			selectedPiece.ExecuteInspectEnd();
		}

		// Token: 0x060001A7 RID: 423 RVA: 0x0000A69C File Offset: 0x0000889C
		protected void UpdateAnyBetterPiecesAvailable()
		{
			int num = (int)((this.SelectedPiece != null) ? this.SelectedPiece.UpgradePieceTier : ((ShipUpgradePieceBaseVM.ShipUpgradePieceTier)(-1)));
			int num2 = -1;
			for (int i = 0; i < this.AvailablePieces.Count; i++)
			{
				ShipUpgradePieceBaseVM shipUpgradePieceBaseVM = this.AvailablePieces[i];
				if (!shipUpgradePieceBaseVM.IsDisabled)
				{
					num2 = MathF.Max((int)shipUpgradePieceBaseVM.UpgradePieceTier, num2);
				}
			}
			this.AnyBetterPiecesAvailable = num2 > num;
		}

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x060001A8 RID: 424 RVA: 0x0000A704 File Offset: 0x00008904
		// (set) Token: 0x060001A9 RID: 425 RVA: 0x0000A70C File Offset: 0x0000890C
		[DataSourceProperty]
		public bool CanTradeUpgrades
		{
			get
			{
				return this._canTradeUpgrades;
			}
			set
			{
				if (value != this._canTradeUpgrades)
				{
					this._canTradeUpgrades = value;
					base.OnPropertyChangedWithValue(value, "CanTradeUpgrades");
				}
			}
		}

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x060001AA RID: 426 RVA: 0x0000A72A File Offset: 0x0000892A
		// (set) Token: 0x060001AB RID: 427 RVA: 0x0000A732 File Offset: 0x00008932
		[DataSourceProperty]
		public bool IsChanged
		{
			get
			{
				return this._isChanged;
			}
			set
			{
				if (value != this._isChanged)
				{
					this._isChanged = value;
					base.OnPropertyChangedWithValue(value, "IsChanged");
				}
			}
		}

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x060001AC RID: 428 RVA: 0x0000A750 File Offset: 0x00008950
		// (set) Token: 0x060001AD RID: 429 RVA: 0x0000A758 File Offset: 0x00008958
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
				}
			}
		}

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x060001AE RID: 430 RVA: 0x0000A776 File Offset: 0x00008976
		// (set) Token: 0x060001AF RID: 431 RVA: 0x0000A77E File Offset: 0x0000897E
		[DataSourceProperty]
		public bool HasSelectedPiece
		{
			get
			{
				return this._hasSelectedPiece;
			}
			set
			{
				if (value != this._hasSelectedPiece)
				{
					this._hasSelectedPiece = value;
					base.OnPropertyChangedWithValue(value, "HasSelectedPiece");
					this.IsEmpty = !this._hasSelectedPiece;
				}
			}
		}

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x060001B0 RID: 432 RVA: 0x0000A7AB File Offset: 0x000089AB
		// (set) Token: 0x060001B1 RID: 433 RVA: 0x0000A7B3 File Offset: 0x000089B3
		[DataSourceProperty]
		public bool IsEmpty
		{
			get
			{
				return this._isEmpty;
			}
			set
			{
				if (value != this._isEmpty)
				{
					this._isEmpty = value;
					base.OnPropertyChangedWithValue(value, "IsEmpty");
					this.HasSelectedPiece = !this._isEmpty;
				}
			}
		}

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x060001B2 RID: 434 RVA: 0x0000A7E0 File Offset: 0x000089E0
		// (set) Token: 0x060001B3 RID: 435 RVA: 0x0000A7E8 File Offset: 0x000089E8
		[DataSourceProperty]
		public bool IsUnexamined
		{
			get
			{
				return this._isUnexamined;
			}
			set
			{
				if (value != this._isUnexamined)
				{
					this._isUnexamined = value;
					base.OnPropertyChangedWithValue(value, "IsUnexamined");
				}
			}
		}

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x060001B4 RID: 436 RVA: 0x0000A806 File Offset: 0x00008A06
		// (set) Token: 0x060001B5 RID: 437 RVA: 0x0000A80E File Offset: 0x00008A0E
		[DataSourceProperty]
		public string SlotName
		{
			get
			{
				return this._slotName;
			}
			set
			{
				if (value != this._slotName)
				{
					this._slotName = value;
					base.OnPropertyChangedWithValue<string>(value, "SlotName");
				}
			}
		}

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x060001B6 RID: 438 RVA: 0x0000A831 File Offset: 0x00008A31
		// (set) Token: 0x060001B7 RID: 439 RVA: 0x0000A839 File Offset: 0x00008A39
		[DataSourceProperty]
		public string SlotTypeId
		{
			get
			{
				return this._slotTypeId;
			}
			set
			{
				if (value != this._slotTypeId)
				{
					this._slotTypeId = value;
					base.OnPropertyChangedWithValue<string>(value, "SlotTypeId");
				}
			}
		}

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x060001B8 RID: 440 RVA: 0x0000A85C File Offset: 0x00008A5C
		// (set) Token: 0x060001B9 RID: 441 RVA: 0x0000A864 File Offset: 0x00008A64
		[DataSourceProperty]
		public HintViewModel ClearSlotHint
		{
			get
			{
				return this._clearSlotHint;
			}
			set
			{
				if (value != this._clearSlotHint)
				{
					this._clearSlotHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ClearSlotHint");
				}
			}
		}

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x060001BA RID: 442 RVA: 0x0000A882 File Offset: 0x00008A82
		// (set) Token: 0x060001BB RID: 443 RVA: 0x0000A88A File Offset: 0x00008A8A
		[DataSourceProperty]
		public HintViewModel SlotHint
		{
			get
			{
				return this._slotHint;
			}
			set
			{
				if (value != this._slotHint)
				{
					this._slotHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "SlotHint");
				}
			}
		}

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x060001BC RID: 444 RVA: 0x0000A8A8 File Offset: 0x00008AA8
		// (set) Token: 0x060001BD RID: 445 RVA: 0x0000A8B0 File Offset: 0x00008AB0
		[DataSourceProperty]
		public ShipUpgradePieceBaseVM SelectedPiece
		{
			get
			{
				return this._selectedPiece;
			}
			set
			{
				if (value != this._selectedPiece)
				{
					if (this._selectedPiece != null)
					{
						this._selectedPiece.IsSelected = false;
					}
					this._selectedPiece = value;
					base.OnPropertyChangedWithValue<ShipUpgradePieceBaseVM>(value, "SelectedPiece");
					if (this._selectedPiece != null)
					{
						this._selectedPiece.IsSelected = true;
					}
					this.HasSelectedPiece = this._selectedPiece != null;
					this.IsChanged = this.GetIsChanged();
					this.UpdateAnyBetterPiecesAvailable();
				}
			}
		}

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x060001BE RID: 446 RVA: 0x0000A922 File Offset: 0x00008B22
		// (set) Token: 0x060001BF RID: 447 RVA: 0x0000A92A File Offset: 0x00008B2A
		[DataSourceProperty]
		public MBBindingList<ShipUpgradePieceBaseVM> AvailablePieces
		{
			get
			{
				return this._availablePieces;
			}
			set
			{
				if (value != this._availablePieces)
				{
					this._availablePieces = value;
					base.OnPropertyChangedWithValue<MBBindingList<ShipUpgradePieceBaseVM>>(value, "AvailablePieces");
				}
			}
		}

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x060001C0 RID: 448 RVA: 0x0000A948 File Offset: 0x00008B48
		// (set) Token: 0x060001C1 RID: 449 RVA: 0x0000A950 File Offset: 0x00008B50
		[DataSourceProperty]
		public bool AnyBetterPiecesAvailable
		{
			get
			{
				return this._anyBetterPiecesAvailable;
			}
			set
			{
				if (value != this._anyBetterPiecesAvailable)
				{
					this._anyBetterPiecesAvailable = value;
					base.OnPropertyChangedWithValue(value, "AnyBetterPiecesAvailable");
				}
			}
		}

		// Token: 0x0400009C RID: 156
		public readonly Ship Ship;

		// Token: 0x0400009D RID: 157
		public readonly string ShipSlotTag;

		// Token: 0x0400009E RID: 158
		private readonly TextObject _nameText;

		// Token: 0x0400009F RID: 159
		private TextObject _actionInfoTooltip;

		// Token: 0x040000A0 RID: 160
		private readonly Action<ShipUpgradeSlotBaseVM> _onSelected;

		// Token: 0x040000A1 RID: 161
		private bool _canTradeUpgrades;

		// Token: 0x040000A2 RID: 162
		private bool _isChanged;

		// Token: 0x040000A3 RID: 163
		private bool _isSelected;

		// Token: 0x040000A4 RID: 164
		private bool _hasSelectedPiece;

		// Token: 0x040000A5 RID: 165
		private bool _isEmpty = true;

		// Token: 0x040000A6 RID: 166
		private bool _isUnexamined;

		// Token: 0x040000A7 RID: 167
		private HintViewModel _clearSlotHint;

		// Token: 0x040000A8 RID: 168
		private string _slotName;

		// Token: 0x040000A9 RID: 169
		private string _slotTypeId;

		// Token: 0x040000AA RID: 170
		private HintViewModel _slotHint;

		// Token: 0x040000AB RID: 171
		private ShipUpgradePieceBaseVM _selectedPiece;

		// Token: 0x040000AC RID: 172
		private MBBindingList<ShipUpgradePieceBaseVM> _availablePieces;

		// Token: 0x040000AD RID: 173
		private bool _anyBetterPiecesAvailable;

		// Token: 0x02000051 RID: 81
		// (Invoke) Token: 0x0600049F RID: 1183
		public delegate void ShipPieceSelectedDelegate(Ship ship, string shipSlotTag, string slotTypeId, ShipUpgradePieceBaseVM pieceVM);

		// Token: 0x02000052 RID: 82
		protected class UpgradePieceComparer : IComparer<ShipUpgradePieceBaseVM>
		{
			// Token: 0x060004A2 RID: 1186 RVA: 0x00014BB0 File Offset: 0x00012DB0
			public int Compare(ShipUpgradePieceBaseVM x, ShipUpgradePieceBaseVM y)
			{
				int num = x.UpgradePieceTier.CompareTo(y.UpgradePieceTier);
				if (num != 0)
				{
					return num;
				}
				return this.ResolveEquality(x, y);
			}

			// Token: 0x060004A3 RID: 1187 RVA: 0x00014BEA File Offset: 0x00012DEA
			private int ResolveEquality(ShipUpgradePieceBaseVM x, ShipUpgradePieceBaseVM y)
			{
				return x.Name.CompareTo(y.Name);
			}
		}
	}
}
