using System;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace NavalDLC.ViewModelCollection.Port
{
	// Token: 0x02000013 RID: 19
	public class ShipUpgradePieceBaseVM : ViewModel
	{
		// Token: 0x14000007 RID: 7
		// (add) Token: 0x0600016A RID: 362 RVA: 0x00009CAC File Offset: 0x00007EAC
		// (remove) Token: 0x0600016B RID: 363 RVA: 0x00009CE0 File Offset: 0x00007EE0
		public static event Action<ShipUpgradePieceBaseVM> OnInspected;

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x0600016C RID: 364 RVA: 0x00009D13 File Offset: 0x00007F13
		// (set) Token: 0x0600016D RID: 365 RVA: 0x00009D1C File Offset: 0x00007F1C
		public ShipUpgradePieceBaseVM.ShipUpgradePieceTier UpgradePieceTier
		{
			get
			{
				return this._upgradePieceTier;
			}
			set
			{
				if (this._upgradePieceTier != value)
				{
					this._upgradePieceTier = value;
					this.IsBronzeTier = this._upgradePieceTier == ShipUpgradePieceBaseVM.ShipUpgradePieceTier.Bronze;
					this.IsSilverTier = this._upgradePieceTier == ShipUpgradePieceBaseVM.ShipUpgradePieceTier.Silver;
					this.IsGoldTier = this._upgradePieceTier == ShipUpgradePieceBaseVM.ShipUpgradePieceTier.Gold;
					this.IsDiamondTier = this._upgradePieceTier == ShipUpgradePieceBaseVM.ShipUpgradePieceTier.Diamond;
				}
			}
		}

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x0600016E RID: 366 RVA: 0x00009D75 File Offset: 0x00007F75
		// (set) Token: 0x0600016F RID: 367 RVA: 0x00009D7D File Offset: 0x00007F7D
		public bool IsInspectedFromSlot { get; private set; }

		// Token: 0x06000170 RID: 368 RVA: 0x00009D86 File Offset: 0x00007F86
		public ShipUpgradePieceBaseVM(Action<ShipUpgradePieceBaseVM> onSelected)
		{
			this._onSelected = onSelected;
			this.Properties = new MBBindingList<StringPairItemVM>();
		}

		// Token: 0x06000171 RID: 369 RVA: 0x00009DAE File Offset: 0x00007FAE
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.UpdateProperties();
		}

		// Token: 0x06000172 RID: 370 RVA: 0x00009DBC File Offset: 0x00007FBC
		protected virtual PropertyBasedTooltipVM GetProperties()
		{
			return null;
		}

		// Token: 0x06000173 RID: 371 RVA: 0x00009DC0 File Offset: 0x00007FC0
		private void UpdateProperties()
		{
			this.Properties.Clear();
			PropertyBasedTooltipVM properties = this.GetProperties();
			if (properties != null)
			{
				for (int i = 0; i < properties.TooltipPropertyList.Count; i++)
				{
					TooltipProperty tooltipProperty = properties.TooltipPropertyList[i];
					if (tooltipProperty.PropertyModifier != 4096)
					{
						this.Properties.Add(new StringPairItemVM(tooltipProperty.DefinitionLabel, tooltipProperty.ValueLabel, null));
					}
				}
			}
		}

		// Token: 0x06000174 RID: 372 RVA: 0x00009E2F File Offset: 0x0000802F
		public void ExecuteSelect()
		{
			Action<ShipUpgradePieceBaseVM> onSelected = this._onSelected;
			if (onSelected == null)
			{
				return;
			}
			onSelected(this);
		}

		// Token: 0x06000175 RID: 373 RVA: 0x00009E42 File Offset: 0x00008042
		public void ExecuteInspectBegin()
		{
			this.InspectPiece(false, null);
		}

		// Token: 0x06000176 RID: 374 RVA: 0x00009E4C File Offset: 0x0000804C
		public virtual void InspectPiece(bool isInspectedFromSlot = false, TextObject slotHintText = null)
		{
			if (this.IsInspectedFromSlot != isInspectedFromSlot || this._slotHintText != slotHintText)
			{
				this.IsInspectedFromSlot = isInspectedFromSlot;
				this._slotHintText = slotHintText;
				this.UpdateProperties();
			}
			Action<ShipUpgradePieceBaseVM> onInspected = ShipUpgradePieceBaseVM.OnInspected;
			if (onInspected == null)
			{
				return;
			}
			onInspected(this);
		}

		// Token: 0x06000177 RID: 375 RVA: 0x00009E89 File Offset: 0x00008089
		public void ExecuteInspectEnd()
		{
			Action<ShipUpgradePieceBaseVM> onInspected = ShipUpgradePieceBaseVM.OnInspected;
			if (onInspected == null)
			{
				return;
			}
			onInspected(null);
		}

		// Token: 0x06000178 RID: 376 RVA: 0x00009E9B File Offset: 0x0000809B
		public virtual void Update()
		{
		}

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x06000179 RID: 377 RVA: 0x00009E9D File Offset: 0x0000809D
		// (set) Token: 0x0600017A RID: 378 RVA: 0x00009EA5 File Offset: 0x000080A5
		[DataSourceProperty]
		public string Identifier
		{
			get
			{
				return this._identifier;
			}
			set
			{
				if (value != this._identifier)
				{
					this._identifier = value;
					base.OnPropertyChangedWithValue<string>(value, "Identifier");
				}
			}
		}

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x0600017B RID: 379 RVA: 0x00009EC8 File Offset: 0x000080C8
		// (set) Token: 0x0600017C RID: 380 RVA: 0x00009ED0 File Offset: 0x000080D0
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

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x0600017D RID: 381 RVA: 0x00009EF3 File Offset: 0x000080F3
		// (set) Token: 0x0600017E RID: 382 RVA: 0x00009EFB File Offset: 0x000080FB
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

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x0600017F RID: 383 RVA: 0x00009F19 File Offset: 0x00008119
		// (set) Token: 0x06000180 RID: 384 RVA: 0x00009F21 File Offset: 0x00008121
		[DataSourceProperty]
		public bool IsDisabled
		{
			get
			{
				return this._isDisabled;
			}
			set
			{
				if (value != this._isDisabled)
				{
					this._isDisabled = value;
					base.OnPropertyChangedWithValue(value, "IsDisabled");
				}
			}
		}

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x06000181 RID: 385 RVA: 0x00009F3F File Offset: 0x0000813F
		// (set) Token: 0x06000182 RID: 386 RVA: 0x00009F47 File Offset: 0x00008147
		[DataSourceProperty]
		public bool IsInspected
		{
			get
			{
				return this._isInspected;
			}
			set
			{
				if (value != this._isInspected)
				{
					this._isInspected = value;
					base.OnPropertyChangedWithValue(value, "IsInspected");
				}
			}
		}

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x06000183 RID: 387 RVA: 0x00009F65 File Offset: 0x00008165
		// (set) Token: 0x06000184 RID: 388 RVA: 0x00009F6D File Offset: 0x0000816D
		[DataSourceProperty]
		public bool IsDiamondTier
		{
			get
			{
				return this._isDiamondTier;
			}
			set
			{
				if (value != this._isDiamondTier)
				{
					this._isDiamondTier = value;
					base.OnPropertyChangedWithValue(value, "IsDiamondTier");
				}
			}
		}

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x06000185 RID: 389 RVA: 0x00009F8B File Offset: 0x0000818B
		// (set) Token: 0x06000186 RID: 390 RVA: 0x00009F93 File Offset: 0x00008193
		[DataSourceProperty]
		public bool IsBronzeTier
		{
			get
			{
				return this._isBronzeTier;
			}
			set
			{
				if (value != this._isBronzeTier)
				{
					this._isBronzeTier = value;
					base.OnPropertyChangedWithValue(value, "IsBronzeTier");
				}
			}
		}

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x06000187 RID: 391 RVA: 0x00009FB1 File Offset: 0x000081B1
		// (set) Token: 0x06000188 RID: 392 RVA: 0x00009FB9 File Offset: 0x000081B9
		[DataSourceProperty]
		public bool IsSilverTier
		{
			get
			{
				return this._isSilverTier;
			}
			set
			{
				if (value != this._isSilverTier)
				{
					this._isSilverTier = value;
					base.OnPropertyChangedWithValue(value, "IsSilverTier");
				}
			}
		}

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x06000189 RID: 393 RVA: 0x00009FD7 File Offset: 0x000081D7
		// (set) Token: 0x0600018A RID: 394 RVA: 0x00009FDF File Offset: 0x000081DF
		[DataSourceProperty]
		public bool IsGoldTier
		{
			get
			{
				return this._isGoldTier;
			}
			set
			{
				if (value != this._isGoldTier)
				{
					this._isGoldTier = value;
					base.OnPropertyChangedWithValue(value, "IsGoldTier");
				}
			}
		}

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x0600018B RID: 395 RVA: 0x00009FFD File Offset: 0x000081FD
		// (set) Token: 0x0600018C RID: 396 RVA: 0x0000A005 File Offset: 0x00008205
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

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x0600018D RID: 397 RVA: 0x0000A023 File Offset: 0x00008223
		// (set) Token: 0x0600018E RID: 398 RVA: 0x0000A02B File Offset: 0x0000822B
		[DataSourceProperty]
		public bool IsHiddenFromPlayer
		{
			get
			{
				return this._isHiddenFromPlayer;
			}
			set
			{
				if (value != this._isHiddenFromPlayer)
				{
					this._isHiddenFromPlayer = value;
					base.OnPropertyChangedWithValue(value, "IsHiddenFromPlayer");
				}
			}
		}

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x0600018F RID: 399 RVA: 0x0000A049 File Offset: 0x00008249
		// (set) Token: 0x06000190 RID: 400 RVA: 0x0000A051 File Offset: 0x00008251
		[DataSourceProperty]
		public MBBindingList<StringPairItemVM> Properties
		{
			get
			{
				return this._properties;
			}
			set
			{
				if (value != this._properties)
				{
					this._properties = value;
					base.OnPropertyChangedWithValue<MBBindingList<StringPairItemVM>>(value, "Properties");
				}
			}
		}

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x06000191 RID: 401 RVA: 0x0000A06F File Offset: 0x0000826F
		// (set) Token: 0x06000192 RID: 402 RVA: 0x0000A077 File Offset: 0x00008277
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

		// Token: 0x04000087 RID: 135
		public Action<ShipUpgradePieceBaseVM> _onSelected;

		// Token: 0x04000088 RID: 136
		private ShipUpgradePieceBaseVM.ShipUpgradePieceTier _upgradePieceTier = ShipUpgradePieceBaseVM.ShipUpgradePieceTier.Bronze;

		// Token: 0x0400008A RID: 138
		protected TextObject _slotHintText;

		// Token: 0x0400008B RID: 139
		private string _identifier;

		// Token: 0x0400008C RID: 140
		private string _name;

		// Token: 0x0400008D RID: 141
		private bool _isSelected;

		// Token: 0x0400008E RID: 142
		private bool _isDisabled;

		// Token: 0x0400008F RID: 143
		private bool _isInspected;

		// Token: 0x04000090 RID: 144
		private bool _isBronzeTier = true;

		// Token: 0x04000091 RID: 145
		private bool _isSilverTier;

		// Token: 0x04000092 RID: 146
		private bool _isGoldTier;

		// Token: 0x04000093 RID: 147
		private bool _isDiamondTier;

		// Token: 0x04000094 RID: 148
		private bool _isUnexamined;

		// Token: 0x04000095 RID: 149
		private bool _isHiddenFromPlayer;

		// Token: 0x04000096 RID: 150
		private int _price;

		// Token: 0x04000097 RID: 151
		private MBBindingList<StringPairItemVM> _properties;

		// Token: 0x02000050 RID: 80
		public enum ShipUpgradePieceTier
		{
			// Token: 0x040001D5 RID: 469
			Bronze = 1,
			// Token: 0x040001D6 RID: 470
			Silver,
			// Token: 0x040001D7 RID: 471
			Gold,
			// Token: 0x040001D8 RID: 472
			Diamond
		}
	}
}
