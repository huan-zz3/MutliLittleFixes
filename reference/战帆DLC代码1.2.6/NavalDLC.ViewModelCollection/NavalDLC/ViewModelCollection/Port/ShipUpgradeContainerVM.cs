using System;
using System.Collections.Generic;
using NavalDLC.ViewModelCollection.Port.PortScreenHandlers;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace NavalDLC.ViewModelCollection.Port
{
	// Token: 0x02000012 RID: 18
	public class ShipUpgradeContainerVM : ViewModel
	{
		// Token: 0x06000158 RID: 344 RVA: 0x00009914 File Offset: 0x00007B14
		public ShipUpgradeContainerVM(ShipItemVM ship)
		{
			this.Ship = ship;
			this.UpgradeSlots = new MBBindingList<ShipUpgradeSlotBaseVM>();
			foreach (KeyValuePair<string, ShipSlot> keyValuePair in this.Ship.Ship.ShipHull.AvailableSlots)
			{
				this.UpgradeSlots.Add(new ShipUpgradeSlotVM(this.Ship.Ship, keyValuePair.Value.GetSlotTypeName(), keyValuePair.Key, keyValuePair.Value.TypeId, new Action<ShipUpgradeSlotBaseVM>(this.OnSlotSelectedAux)));
			}
			if (this.Ship.Ship.CanEquipFigurehead)
			{
				this.UpgradeSlots.Add(new ShipFigureheadSlotVM(this.Ship.Ship, new TextObject("{=YLbBHN0Z}Figurehead", null), "figurehead", "figurehead", new Action<ShipUpgradeSlotBaseVM>(this.OnSlotSelectedAux)));
			}
		}

		// Token: 0x06000159 RID: 345 RVA: 0x00009A1C File Offset: 0x00007C1C
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.UpgradeSlots.ApplyActionOnAllItems(delegate(ShipUpgradeSlotBaseVM us)
			{
				us.RefreshValues();
			});
		}

		// Token: 0x0600015A RID: 346 RVA: 0x00009A4E File Offset: 0x00007C4E
		public override void OnFinalize()
		{
			base.OnFinalize();
			this.UpgradeSlots.ApplyActionOnAllItems(delegate(ShipUpgradeSlotBaseVM us)
			{
				us.OnFinalize();
			});
		}

		// Token: 0x0600015B RID: 347 RVA: 0x00009A80 File Offset: 0x00007C80
		public void ResetUpgradePieces()
		{
			this.UpgradeSlots.ApplyActionOnAllItems(delegate(ShipUpgradeSlotBaseVM s)
			{
				s.ResetPieces();
			});
		}

		// Token: 0x0600015C RID: 348 RVA: 0x00009AAC File Offset: 0x00007CAC
		public void UpdateEnabledStatus(in PortActionInfo actionInfo)
		{
			this.CanTradeUpgrades = actionInfo.IsEnabled;
			for (int i = 0; i < this.UpgradeSlots.Count; i++)
			{
				this.UpgradeSlots[i].UpdateEnabledStatus(in actionInfo);
			}
		}

		// Token: 0x0600015D RID: 349 RVA: 0x00009AF0 File Offset: 0x00007CF0
		private void OnSlotSelectedAux(ShipUpgradeSlotBaseVM slot)
		{
			if (this.SelectedSlot != null && this.SelectedSlot == slot)
			{
				this.SelectedSlot = null;
				ShipUpgradeContainerVM.ShipSlotSelectedDelegate onSlotSelected = ShipUpgradeContainerVM.OnSlotSelected;
				if (onSlotSelected == null)
				{
					return;
				}
				onSlotSelected(this.SelectedSlot);
				return;
			}
			else
			{
				if (slot != null && slot.AvailablePieces.Count == 0 && !slot.HasSelectedPiece)
				{
					return;
				}
				this.SelectedSlot = slot;
				ShipUpgradeContainerVM.ShipSlotSelectedDelegate onSlotSelected2 = ShipUpgradeContainerVM.OnSlotSelected;
				if (onSlotSelected2 == null)
				{
					return;
				}
				onSlotSelected2(this.SelectedSlot);
				return;
			}
		}

		// Token: 0x0600015E RID: 350 RVA: 0x00009B60 File Offset: 0x00007D60
		public void ExecuteClearSelection()
		{
			ShipUpgradeSlotBaseVM selectedSlot = this.SelectedSlot;
			if (selectedSlot == null)
			{
				return;
			}
			selectedSlot.ExecuteDeselect();
		}

		// Token: 0x0600015F RID: 351 RVA: 0x00009B74 File Offset: 0x00007D74
		public void Update()
		{
			for (int i = 0; i < this.UpgradeSlots.Count; i++)
			{
				this.UpgradeSlots[i].Update();
			}
		}

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x06000160 RID: 352 RVA: 0x00009BA8 File Offset: 0x00007DA8
		// (set) Token: 0x06000161 RID: 353 RVA: 0x00009BB0 File Offset: 0x00007DB0
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

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x06000162 RID: 354 RVA: 0x00009BCE File Offset: 0x00007DCE
		// (set) Token: 0x06000163 RID: 355 RVA: 0x00009BD6 File Offset: 0x00007DD6
		[DataSourceProperty]
		public bool HasSelectedSlot
		{
			get
			{
				return this._hasSelectedSlot;
			}
			set
			{
				if (value != this._hasSelectedSlot)
				{
					this._hasSelectedSlot = value;
					base.OnPropertyChangedWithValue(value, "HasSelectedSlot");
				}
			}
		}

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x06000164 RID: 356 RVA: 0x00009BF4 File Offset: 0x00007DF4
		// (set) Token: 0x06000165 RID: 357 RVA: 0x00009BFC File Offset: 0x00007DFC
		[DataSourceProperty]
		public ShipItemVM Ship
		{
			get
			{
				return this._ship;
			}
			set
			{
				if (value != this._ship)
				{
					this._ship = value;
					base.OnPropertyChangedWithValue<ShipItemVM>(value, "Ship");
				}
			}
		}

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x06000166 RID: 358 RVA: 0x00009C1A File Offset: 0x00007E1A
		// (set) Token: 0x06000167 RID: 359 RVA: 0x00009C24 File Offset: 0x00007E24
		[DataSourceProperty]
		public ShipUpgradeSlotBaseVM SelectedSlot
		{
			get
			{
				return this._selectedSlot;
			}
			set
			{
				if (value != this._selectedSlot)
				{
					if (this._selectedSlot != null)
					{
						this._selectedSlot.IsSelected = false;
					}
					this._selectedSlot = value;
					base.OnPropertyChangedWithValue<ShipUpgradeSlotBaseVM>(value, "SelectedSlot");
					if (this._selectedSlot != null)
					{
						this._selectedSlot.IsSelected = true;
					}
					this.HasSelectedSlot = this._selectedSlot != null;
				}
			}
		}

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x06000168 RID: 360 RVA: 0x00009C84 File Offset: 0x00007E84
		// (set) Token: 0x06000169 RID: 361 RVA: 0x00009C8C File Offset: 0x00007E8C
		[DataSourceProperty]
		public MBBindingList<ShipUpgradeSlotBaseVM> UpgradeSlots
		{
			get
			{
				return this._upgradeSlots;
			}
			set
			{
				if (value != this._upgradeSlots)
				{
					this._upgradeSlots = value;
					base.OnPropertyChangedWithValue<MBBindingList<ShipUpgradeSlotBaseVM>>(value, "UpgradeSlots");
				}
			}
		}

		// Token: 0x04000080 RID: 128
		public static ShipUpgradeContainerVM.ShipSlotSelectedDelegate OnSlotSelected;

		// Token: 0x04000081 RID: 129
		private bool _canTradeUpgrades;

		// Token: 0x04000082 RID: 130
		private bool _hasSelectedSlot;

		// Token: 0x04000083 RID: 131
		private ShipItemVM _ship;

		// Token: 0x04000084 RID: 132
		private ShipUpgradeSlotBaseVM _selectedSlot;

		// Token: 0x04000085 RID: 133
		private MBBindingList<ShipUpgradeSlotBaseVM> _upgradeSlots;

		// Token: 0x0200004E RID: 78
		// (Invoke) Token: 0x06000496 RID: 1174
		public delegate void ShipSlotSelectedDelegate(ShipUpgradeSlotBaseVM slot);
	}
}
