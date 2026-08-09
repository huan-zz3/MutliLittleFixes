using System;
using System.Linq;
using NavalDLC.ViewModelCollection.Port.PortScreenHandlers;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace NavalDLC.ViewModelCollection.Port
{
	// Token: 0x0200000E RID: 14
	public class ShipItemVM : ViewModel
	{
		// Token: 0x14000004 RID: 4
		// (add) Token: 0x060000D5 RID: 213 RVA: 0x00007DCC File Offset: 0x00005FCC
		// (remove) Token: 0x060000D6 RID: 214 RVA: 0x00007E00 File Offset: 0x00006000
		public static event Action<ShipItemVM> OnSelected;

		// Token: 0x14000005 RID: 5
		// (add) Token: 0x060000D7 RID: 215 RVA: 0x00007E34 File Offset: 0x00006034
		// (remove) Token: 0x060000D8 RID: 216 RVA: 0x00007E68 File Offset: 0x00006068
		public static event Action<ShipItemVM, string> OnRenamed;

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x060000D9 RID: 217 RVA: 0x00007E9C File Offset: 0x0000609C
		// (remove) Token: 0x060000DA RID: 218 RVA: 0x00007ED0 File Offset: 0x000060D0
		public static event Action<ShipItemVM> OnNameReset;

		// Token: 0x060000DB RID: 219 RVA: 0x00007F04 File Offset: 0x00006104
		public ShipItemVM(Ship ship)
		{
			this.Ship = ship;
			this.PrefabId = NavalUIHelper.GetPrefabIdOfShipHull(this.Ship.ShipHull);
			this.Stats = new ShipStatsVM(this.Ship);
			this.Upgrades = new ShipUpgradeContainerVM(this);
			this.RefreshValues();
		}

		// Token: 0x060000DC RID: 220 RVA: 0x00007F58 File Offset: 0x00006158
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = (this.IsRenamed ? this._changedName : this.Ship.Name.ToString());
			this.HullName = this.Ship.ShipHull.Name.ToString();
			this.Upgrades.RefreshValues();
			this.Stats.RefreshValues();
			this.RefreshHpStrings();
		}

		// Token: 0x060000DD RID: 221 RVA: 0x00007FC8 File Offset: 0x000061C8
		public override void OnFinalize()
		{
			base.OnFinalize();
			this.Upgrades.OnFinalize();
			this.Stats.OnFinalize();
		}

		// Token: 0x060000DE RID: 222 RVA: 0x00007FE8 File Offset: 0x000061E8
		public void RefreshProperties(PortScreenHandler handler)
		{
			this.IsBought = handler.ShipsToBuy.Any<PortScreenHandler.ShipTradeInfo>((PortScreenHandler.ShipTradeInfo x) => x.Ship == this.Ship);
			this.IsSold = handler.ShipsToSell.Any<PortScreenHandler.ShipTradeInfo>((PortScreenHandler.ShipTradeInfo x) => x.Ship == this.Ship);
			this.IsRepaired = handler.ShipsToRepair.Contains(this.Ship);
			this.IsRenamed = handler.ShipsToRename.Any<PortScreenHandler.ShipRenameInfo>((PortScreenHandler.ShipRenameInfo s) => s.Ship == this.Ship);
			this.InitialHp = this.Ship.HitPoints;
			this.MaxHp = this.Ship.MaxHitPoints;
			this.CurrentHp = (this.IsRepaired ? this.Ship.MaxHitPoints : this.Ship.HitPoints);
			this.IsHealthRelevant = this.InitialHp < this.MaxHp;
			bool flag;
			if (!this.IsBought && !this.IsSold && !this.IsRepaired && !this.IsRenamed)
			{
				flag = this.Upgrades.UpgradeSlots.Any<ShipUpgradeSlotBaseVM>((ShipUpgradeSlotBaseVM s) => s.IsChanged);
			}
			else
			{
				flag = true;
			}
			this.HasChanges = flag;
			if (handler.LeftShips.Contains(this.Ship))
			{
				PortActionInfo canBuyShip = handler.GetCanBuyShip(this.Ship);
				this.Price = ((canBuyShip.IsRelevant && canBuyShip.IsEnabled) ? canBuyShip.GoldCost : 0);
			}
			else
			{
				PortActionInfo canSellShip = handler.GetCanSellShip(this.Ship);
				this.Price = ((canSellShip.IsRelevant && canSellShip.IsEnabled) ? canSellShip.GoldCost : 0);
			}
			this.RefreshValues();
		}

		// Token: 0x060000DF RID: 223 RVA: 0x0000818C File Offset: 0x0000638C
		public void ExecuteChangeShipName()
		{
			InformationManager.ShowTextInquiry(new TextInquiryData(new TextObject("{=rO84r0W1}Change Ship Name", null).ToString(), string.Empty, true, true, GameTexts.FindText("str_done", null).ToString(), GameTexts.FindText("str_cancel", null).ToString(), new Action<string>(this.OnChangeShipNameDone), null, false, new Func<string, Tuple<bool, string>>(NavalUIHelper.IsStringApplicableForShipName), "", ""), false, false);
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x00008200 File Offset: 0x00006400
		public void ExecuteSelect()
		{
			Action<ShipItemVM> onSelected = ShipItemVM.OnSelected;
			if (onSelected == null)
			{
				return;
			}
			onSelected(this);
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x00008212 File Offset: 0x00006412
		public void ExecuteResetShipName()
		{
			if (this.IsRenamed)
			{
				this._changedName = string.Empty;
				Action<ShipItemVM> onNameReset = ShipItemVM.OnNameReset;
				if (onNameReset == null)
				{
					return;
				}
				onNameReset(this);
			}
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x00008237 File Offset: 0x00006437
		private void OnChangeShipNameDone(string newName)
		{
			if (newName != this.Name)
			{
				this._changedName = newName;
				Action<ShipItemVM, string> onRenamed = ShipItemVM.OnRenamed;
				if (onRenamed == null)
				{
					return;
				}
				onRenamed(this, newName);
			}
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x00008260 File Offset: 0x00006460
		private void RefreshHpStrings()
		{
			this.CurrentHpText = ((int)this.CurrentHp).ToString();
			this.MaxHpText = ((int)this.MaxHp).ToString();
			this.SeparatorText = " / ";
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x060000E4 RID: 228 RVA: 0x000082A2 File Offset: 0x000064A2
		// (set) Token: 0x060000E5 RID: 229 RVA: 0x000082AA File Offset: 0x000064AA
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

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x060000E6 RID: 230 RVA: 0x000082C8 File Offset: 0x000064C8
		// (set) Token: 0x060000E7 RID: 231 RVA: 0x000082D0 File Offset: 0x000064D0
		[DataSourceProperty]
		public bool IsRepaired
		{
			get
			{
				return this._isRepaired;
			}
			set
			{
				if (value != this._isRepaired)
				{
					this._isRepaired = value;
					base.OnPropertyChangedWithValue(value, "IsRepaired");
				}
			}
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x060000E8 RID: 232 RVA: 0x000082EE File Offset: 0x000064EE
		// (set) Token: 0x060000E9 RID: 233 RVA: 0x000082F6 File Offset: 0x000064F6
		[DataSourceProperty]
		public bool IsRenamed
		{
			get
			{
				return this._isRenamed;
			}
			set
			{
				if (value != this._isRenamed)
				{
					this._isRenamed = value;
					base.OnPropertyChangedWithValue(value, "IsRenamed");
				}
			}
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x060000EA RID: 234 RVA: 0x00008314 File Offset: 0x00006514
		// (set) Token: 0x060000EB RID: 235 RVA: 0x0000831C File Offset: 0x0000651C
		[DataSourceProperty]
		public bool PlayerCanChangeShipName
		{
			get
			{
				return this._playerCanChangeShipName;
			}
			set
			{
				if (value != this._playerCanChangeShipName)
				{
					this._playerCanChangeShipName = value;
					base.OnPropertyChangedWithValue(value, "PlayerCanChangeShipName");
				}
			}
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x060000EC RID: 236 RVA: 0x0000833A File Offset: 0x0000653A
		// (set) Token: 0x060000ED RID: 237 RVA: 0x00008342 File Offset: 0x00006542
		[DataSourceProperty]
		public bool IsSold
		{
			get
			{
				return this._isSold;
			}
			set
			{
				if (value != this._isSold)
				{
					this._isSold = value;
					base.OnPropertyChangedWithValue(value, "IsSold");
				}
			}
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x060000EE RID: 238 RVA: 0x00008360 File Offset: 0x00006560
		// (set) Token: 0x060000EF RID: 239 RVA: 0x00008368 File Offset: 0x00006568
		[DataSourceProperty]
		public float InitialHp
		{
			get
			{
				return this._initialHp;
			}
			set
			{
				if (value != this._initialHp)
				{
					this._initialHp = value;
					base.OnPropertyChangedWithValue(value, "InitialHp");
				}
			}
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x060000F0 RID: 240 RVA: 0x00008386 File Offset: 0x00006586
		// (set) Token: 0x060000F1 RID: 241 RVA: 0x0000838E File Offset: 0x0000658E
		[DataSourceProperty]
		public bool HasChanges
		{
			get
			{
				return this._hasChanges;
			}
			set
			{
				if (value != this._hasChanges)
				{
					this._hasChanges = value;
					base.OnPropertyChangedWithValue(value, "HasChanges");
				}
			}
		}

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x060000F2 RID: 242 RVA: 0x000083AC File Offset: 0x000065AC
		// (set) Token: 0x060000F3 RID: 243 RVA: 0x000083B4 File Offset: 0x000065B4
		[DataSourceProperty]
		public bool IsBought
		{
			get
			{
				return this._isBought;
			}
			set
			{
				if (value != this._isBought)
				{
					this._isBought = value;
					base.OnPropertyChangedWithValue(value, "IsBought");
				}
			}
		}

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060000F4 RID: 244 RVA: 0x000083D2 File Offset: 0x000065D2
		// (set) Token: 0x060000F5 RID: 245 RVA: 0x000083DA File Offset: 0x000065DA
		[DataSourceProperty]
		public float CurrentHp
		{
			get
			{
				return this._currentHp;
			}
			set
			{
				if (value != this._currentHp)
				{
					this._currentHp = value;
					base.OnPropertyChangedWithValue(value, "CurrentHp");
					this.RefreshHpStrings();
				}
			}
		}

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060000F6 RID: 246 RVA: 0x000083FE File Offset: 0x000065FE
		// (set) Token: 0x060000F7 RID: 247 RVA: 0x00008406 File Offset: 0x00006606
		[DataSourceProperty]
		public float MaxHp
		{
			get
			{
				return this._maxHp;
			}
			set
			{
				if (value != this._maxHp)
				{
					this._maxHp = value;
					base.OnPropertyChangedWithValue(value, "MaxHp");
					this.RefreshHpStrings();
				}
			}
		}

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060000F8 RID: 248 RVA: 0x0000842A File Offset: 0x0000662A
		// (set) Token: 0x060000F9 RID: 249 RVA: 0x00008432 File Offset: 0x00006632
		[DataSourceProperty]
		public string CurrentHpText
		{
			get
			{
				return this._currentHpText;
			}
			set
			{
				if (value != this._currentHpText)
				{
					this._currentHpText = value;
					base.OnPropertyChangedWithValue<string>(value, "CurrentHpText");
				}
			}
		}

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060000FA RID: 250 RVA: 0x00008455 File Offset: 0x00006655
		// (set) Token: 0x060000FB RID: 251 RVA: 0x0000845D File Offset: 0x0000665D
		[DataSourceProperty]
		public string MaxHpText
		{
			get
			{
				return this._maxHpText;
			}
			set
			{
				if (value != this._maxHpText)
				{
					this._maxHpText = value;
					base.OnPropertyChangedWithValue<string>(value, "MaxHpText");
				}
			}
		}

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060000FC RID: 252 RVA: 0x00008480 File Offset: 0x00006680
		// (set) Token: 0x060000FD RID: 253 RVA: 0x00008488 File Offset: 0x00006688
		[DataSourceProperty]
		public string SeparatorText
		{
			get
			{
				return this._separatorText;
			}
			set
			{
				if (value != this._separatorText)
				{
					this._separatorText = value;
					base.OnPropertyChangedWithValue<string>(value, "SeparatorText");
				}
			}
		}

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060000FE RID: 254 RVA: 0x000084AB File Offset: 0x000066AB
		// (set) Token: 0x060000FF RID: 255 RVA: 0x000084B3 File Offset: 0x000066B3
		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x06000100 RID: 256 RVA: 0x000084D6 File Offset: 0x000066D6
		// (set) Token: 0x06000101 RID: 257 RVA: 0x000084DE File Offset: 0x000066DE
		[DataSourceProperty]
		public string HullName
		{
			get
			{
				return this._hullName;
			}
			set
			{
				if (value != this._hullName)
				{
					this._hullName = value;
					base.OnPropertyChangedWithValue<string>(value, "HullName");
				}
			}
		}

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x06000102 RID: 258 RVA: 0x00008501 File Offset: 0x00006701
		// (set) Token: 0x06000103 RID: 259 RVA: 0x00008509 File Offset: 0x00006709
		[DataSourceProperty]
		public string PrefabId
		{
			get
			{
				return this._prefabId;
			}
			set
			{
				if (value != this._prefabId)
				{
					this._prefabId = value;
					base.OnPropertyChangedWithValue<string>(value, "PrefabId");
				}
			}
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x06000104 RID: 260 RVA: 0x0000852C File Offset: 0x0000672C
		// (set) Token: 0x06000105 RID: 261 RVA: 0x00008534 File Offset: 0x00006734
		[DataSourceProperty]
		public bool IsNight
		{
			get
			{
				return this._isNight;
			}
			set
			{
				if (value != this._isNight)
				{
					this._isNight = value;
					base.OnPropertyChangedWithValue(value, "IsNight");
				}
			}
		}

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x06000106 RID: 262 RVA: 0x00008552 File Offset: 0x00006752
		// (set) Token: 0x06000107 RID: 263 RVA: 0x0000855A File Offset: 0x0000675A
		[DataSourceProperty]
		public int Price
		{
			get
			{
				return this._price;
			}
			set
			{
				if (value != this._price)
				{
					this._price = value;
					base.OnPropertyChangedWithValue(value, "Price");
				}
			}
		}

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x06000108 RID: 264 RVA: 0x00008578 File Offset: 0x00006778
		// (set) Token: 0x06000109 RID: 265 RVA: 0x00008580 File Offset: 0x00006780
		[DataSourceProperty]
		public bool IsHealthRelevant
		{
			get
			{
				return this._isHealthRelevant;
			}
			set
			{
				if (value != this._isHealthRelevant)
				{
					this._isHealthRelevant = value;
					base.OnPropertyChangedWithValue(value, "IsHealthRelevant");
				}
			}
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x0600010A RID: 266 RVA: 0x0000859E File Offset: 0x0000679E
		// (set) Token: 0x0600010B RID: 267 RVA: 0x000085A6 File Offset: 0x000067A6
		[DataSourceProperty]
		public HintViewModel ChangeShipNameHint
		{
			get
			{
				return this._changeShipNameHint;
			}
			set
			{
				if (value != this._changeShipNameHint)
				{
					this._changeShipNameHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ChangeShipNameHint");
				}
			}
		}

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x0600010C RID: 268 RVA: 0x000085C4 File Offset: 0x000067C4
		// (set) Token: 0x0600010D RID: 269 RVA: 0x000085CC File Offset: 0x000067CC
		[DataSourceProperty]
		public ShipStatsVM Stats
		{
			get
			{
				return this._stats;
			}
			set
			{
				if (value != this._stats)
				{
					this._stats = value;
					base.OnPropertyChangedWithValue<ShipStatsVM>(value, "Stats");
				}
			}
		}

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x0600010E RID: 270 RVA: 0x000085EA File Offset: 0x000067EA
		// (set) Token: 0x0600010F RID: 271 RVA: 0x000085F2 File Offset: 0x000067F2
		[DataSourceProperty]
		public ShipUpgradeContainerVM Upgrades
		{
			get
			{
				return this._upgrades;
			}
			set
			{
				if (value != this._upgrades)
				{
					this._upgrades = value;
					base.OnPropertyChangedWithValue<ShipUpgradeContainerVM>(value, "Upgrades");
				}
			}
		}

		// Token: 0x04000049 RID: 73
		public readonly Ship Ship;

		// Token: 0x0400004D RID: 77
		private string _changedName;

		// Token: 0x0400004E RID: 78
		private bool _isSelected;

		// Token: 0x0400004F RID: 79
		private bool _playerCanChangeShipName;

		// Token: 0x04000050 RID: 80
		private bool _isRepaired;

		// Token: 0x04000051 RID: 81
		private float _initialHp;

		// Token: 0x04000052 RID: 82
		private float _currentHp;

		// Token: 0x04000053 RID: 83
		private bool _hasChanges;

		// Token: 0x04000054 RID: 84
		private bool _isRenamed;

		// Token: 0x04000055 RID: 85
		private float _maxHp;

		// Token: 0x04000056 RID: 86
		private bool _isSold;

		// Token: 0x04000057 RID: 87
		private string _currentHpText;

		// Token: 0x04000058 RID: 88
		private bool _isBought;

		// Token: 0x04000059 RID: 89
		private string _maxHpText;

		// Token: 0x0400005A RID: 90
		private string _separatorText;

		// Token: 0x0400005B RID: 91
		private string _name;

		// Token: 0x0400005C RID: 92
		private string _hullName;

		// Token: 0x0400005D RID: 93
		private string _prefabId;

		// Token: 0x0400005E RID: 94
		private bool _isNight;

		// Token: 0x0400005F RID: 95
		private int _price;

		// Token: 0x04000060 RID: 96
		public bool _isHealthRelevant;

		// Token: 0x04000061 RID: 97
		private HintViewModel _changeShipNameHint;

		// Token: 0x04000062 RID: 98
		private ShipStatsVM _stats;

		// Token: 0x04000063 RID: 99
		private ShipUpgradeContainerVM _upgrades;
	}
}
