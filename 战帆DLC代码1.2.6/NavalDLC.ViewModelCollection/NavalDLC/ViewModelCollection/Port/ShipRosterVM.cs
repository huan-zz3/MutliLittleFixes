using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.ImageIdentifiers;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace NavalDLC.ViewModelCollection.Port
{
	// Token: 0x0200000F RID: 15
	public class ShipRosterVM : ViewModel
	{
		// Token: 0x17000049 RID: 73
		// (get) Token: 0x06000113 RID: 275 RVA: 0x00008640 File Offset: 0x00006840
		// (set) Token: 0x06000114 RID: 276 RVA: 0x00008648 File Offset: 0x00006848
		public PartyBase Owner { get; private set; }

		// Token: 0x06000115 RID: 277 RVA: 0x00008651 File Offset: 0x00006851
		public ShipRosterVM(Action onSelected)
		{
			this._onSelected = onSelected;
			this.Ships = new MBBindingList<ShipItemVM>();
			this.Tooltip = new HintViewModel();
			this.RefreshValues();
		}

		// Token: 0x06000116 RID: 278 RVA: 0x0000867C File Offset: 0x0000687C
		public override void RefreshValues()
		{
			base.RefreshValues();
			TextObject rosterName = this._rosterName;
			this.Name = ((rosterName != null) ? rosterName.ToString() : null);
			this.HasNoShipsText = new TextObject("{=vfXHD89T}No ships available", null).ToString();
			this.ShipCountText = new TextObject("{=nx9Pk1ca}{AMOUNT} {?AMOUNT==1}ship{?}ships{\\?}", null).SetTextVariable("AMOUNT", this.Ships.Count).ToString();
			if (this.HasOwnerCharacter)
			{
				float num = (this.Owner.IsMobile ? Campaign.Current.Models.InventoryCapacityModel.CalculateTotalWeightCarried(this.Owner.MobileParty, true, false).ResultNumber : 0f);
				float num2;
				if (!this.Owner.IsMobile || !this.HasAnyShips)
				{
					num2 = this._ships.Sum<ShipItemVM>((ShipItemVM x) => x.Ship.InventoryCapacity);
				}
				else
				{
					num2 = Campaign.Current.Models.InventoryCapacityModel.CalculateInventoryCapacity(this.Owner.MobileParty, true, false, 0, 0, 0, false).ResultNumber;
				}
				float num3 = num2;
				this.WeightText = GameTexts.FindText("str_LEFT_over_RIGHT_no_space", null).SetTextVariable("LEFT", (int)num).SetTextVariable("RIGHT", (int)num3)
					.ToString();
				this.IsWeightDangerous = num > num3;
				int numberOfAllMembers = this.Owner.NumberOfAllMembers;
				int num4 = this._ships.Sum<ShipItemVM>((ShipItemVM x) => x.Ship.TotalCrewCapacity);
				this.TroopCountText = GameTexts.FindText("str_LEFT_over_RIGHT_no_space", null).SetTextVariable("LEFT", numberOfAllMembers).SetTextVariable("RIGHT", num4)
					.ToString();
				this.IsTroopCountDangerous = numberOfAllMembers > num4;
			}
			else
			{
				this.WeightText = string.Empty;
				this.TroopCountText = string.Empty;
				this.IsWeightDangerous = false;
				this.IsTroopCountDangerous = false;
			}
			if (!this.HasAnyShips)
			{
				this.Tooltip.HintText = new TextObject("{=vfXHD89T}No ships available", null);
			}
			else if (this.IsTroopCountDangerous)
			{
				this.Tooltip.HintText = new TextObject("{=LPUWr7J1}Over the troop limit, sailing speed will be negatively affected!", null);
			}
			else if (this.IsWeightDangerous)
			{
				this.Tooltip.HintText = new TextObject("{=qSRbt9qc}Over the carrying limit, sailing speed will be negatively affected!", null);
			}
			else
			{
				this.Tooltip.HintText = null;
			}
			this.Ships.ApplyActionOnAllItems(delegate(ShipItemVM s)
			{
				s.RefreshValues();
			});
		}

		// Token: 0x06000117 RID: 279 RVA: 0x00008903 File Offset: 0x00006B03
		public void SetRosterName(TextObject rosterName)
		{
			this._rosterName = rosterName;
			this.RefreshValues();
		}

		// Token: 0x06000118 RID: 280 RVA: 0x00008914 File Offset: 0x00006B14
		public void SetRosterOwner(PartyBase owner)
		{
			this.Owner = owner;
			this.HasOwnerCharacter = this.Owner != null && this.Owner.LeaderHero != null;
			PartyBase owner2 = this.Owner;
			this.IsTownShipyard = owner2 != null && owner2.IsSettlement && this.Owner.Settlement.HasPort;
			int num;
			if (!this.IsTownShipyard)
			{
				num = 0;
			}
			else
			{
				Town town = this.Owner.Settlement.Town;
				int? num2;
				if (town == null)
				{
					num2 = null;
				}
				else
				{
					Building shipyard = town.GetShipyard();
					num2 = ((shipyard != null) ? new int?(shipyard.CurrentLevel) : null);
				}
				num = num2 ?? 0;
			}
			this.TownShipyardLevel = num;
			CharacterImageIdentifierVM ownerCharacterVisual = this.OwnerCharacterVisual;
			if (ownerCharacterVisual != null)
			{
				ownerCharacterVisual.OnFinalize();
			}
			if (this.HasOwnerCharacter)
			{
				this.OwnerCharacterVisual = new CharacterImageIdentifierVM(CharacterCode.CreateFrom(this.Owner.LeaderHero.CharacterObject));
			}
			else
			{
				this.OwnerCharacterVisual = null;
			}
			this.RefreshValues();
		}

		// Token: 0x06000119 RID: 281 RVA: 0x00008A20 File Offset: 0x00006C20
		public void RefreshShips(MBReadOnlyList<ShipItemVM> removedShips, MBReadOnlyList<ShipItemVM> addedShips, MBReadOnlyList<Ship> orderedShipsList)
		{
			for (int i = 0; i < removedShips.Count; i++)
			{
				this.Ships.Remove(removedShips[i]);
			}
			for (int j = 0; j < addedShips.Count; j++)
			{
				this.Ships.Add(addedShips[j]);
			}
			this.Ships.Sort(new ShipRosterVM.PortShipVMComparer(orderedShipsList));
			this.HasAnyShips = this.Ships.Count > 0;
			this.HasMultipleShips = this.Ships.Count > 1;
			this.RefreshValues();
		}

		// Token: 0x0600011A RID: 282 RVA: 0x00008AB3 File Offset: 0x00006CB3
		public void ExecuteSelectRoster()
		{
			Action onSelected = this._onSelected;
			if (onSelected == null)
			{
				return;
			}
			onSelected();
		}

		// Token: 0x0600011B RID: 283 RVA: 0x00008AC8 File Offset: 0x00006CC8
		public override void OnFinalize()
		{
			base.OnFinalize();
			foreach (ShipItemVM shipItemVM in this.Ships)
			{
				shipItemVM.OnFinalize();
			}
			this.Ships.Clear();
			CharacterImageIdentifierVM ownerCharacterVisual = this.OwnerCharacterVisual;
			if (ownerCharacterVisual != null)
			{
				ownerCharacterVisual.OnFinalize();
			}
			this.OwnerCharacterVisual = null;
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x0600011C RID: 284 RVA: 0x00008B3C File Offset: 0x00006D3C
		// (set) Token: 0x0600011D RID: 285 RVA: 0x00008B44 File Offset: 0x00006D44
		[DataSourceProperty]
		public bool HasAnyShips
		{
			get
			{
				return this._hasAnyShips;
			}
			set
			{
				if (value != this._hasAnyShips)
				{
					this._hasAnyShips = value;
					base.OnPropertyChangedWithValue(value, "HasAnyShips");
				}
			}
		}

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x0600011E RID: 286 RVA: 0x00008B62 File Offset: 0x00006D62
		// (set) Token: 0x0600011F RID: 287 RVA: 0x00008B6A File Offset: 0x00006D6A
		[DataSourceProperty]
		public bool HasMultipleShips
		{
			get
			{
				return this._hasMultipleShips;
			}
			set
			{
				if (value != this._hasMultipleShips)
				{
					this._hasMultipleShips = value;
					base.OnPropertyChangedWithValue(value, "HasMultipleShips");
				}
			}
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x06000120 RID: 288 RVA: 0x00008B88 File Offset: 0x00006D88
		// (set) Token: 0x06000121 RID: 289 RVA: 0x00008B90 File Offset: 0x00006D90
		[DataSourceProperty]
		public bool HasOwnerCharacter
		{
			get
			{
				return this._hasOwnerCharacter;
			}
			set
			{
				if (value != this._hasOwnerCharacter)
				{
					this._hasOwnerCharacter = value;
					base.OnPropertyChangedWithValue(value, "HasOwnerCharacter");
				}
			}
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x06000122 RID: 290 RVA: 0x00008BAE File Offset: 0x00006DAE
		// (set) Token: 0x06000123 RID: 291 RVA: 0x00008BB6 File Offset: 0x00006DB6
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

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x06000124 RID: 292 RVA: 0x00008BD4 File Offset: 0x00006DD4
		// (set) Token: 0x06000125 RID: 293 RVA: 0x00008BDC File Offset: 0x00006DDC
		[DataSourceProperty]
		public bool IsTownShipyard
		{
			get
			{
				return this._isTownShipyard;
			}
			set
			{
				if (value != this._isTownShipyard)
				{
					this._isTownShipyard = value;
					base.OnPropertyChangedWithValue(value, "IsTownShipyard");
				}
			}
		}

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x06000126 RID: 294 RVA: 0x00008BFA File Offset: 0x00006DFA
		// (set) Token: 0x06000127 RID: 295 RVA: 0x00008C02 File Offset: 0x00006E02
		[DataSourceProperty]
		public int TownShipyardLevel
		{
			get
			{
				return this._townShipyardLevel;
			}
			set
			{
				if (value != this._townShipyardLevel)
				{
					this._townShipyardLevel = value;
					base.OnPropertyChangedWithValue(value, "TownShipyardLevel");
				}
			}
		}

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x06000128 RID: 296 RVA: 0x00008C20 File Offset: 0x00006E20
		// (set) Token: 0x06000129 RID: 297 RVA: 0x00008C28 File Offset: 0x00006E28
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

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x0600012A RID: 298 RVA: 0x00008C4B File Offset: 0x00006E4B
		// (set) Token: 0x0600012B RID: 299 RVA: 0x00008C53 File Offset: 0x00006E53
		[DataSourceProperty]
		public string HasNoShipsText
		{
			get
			{
				return this._hasNoShipsText;
			}
			set
			{
				if (value != this._hasNoShipsText)
				{
					this._hasNoShipsText = value;
					base.OnPropertyChangedWithValue<string>(value, "HasNoShipsText");
				}
			}
		}

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x0600012C RID: 300 RVA: 0x00008C76 File Offset: 0x00006E76
		// (set) Token: 0x0600012D RID: 301 RVA: 0x00008C7E File Offset: 0x00006E7E
		[DataSourceProperty]
		public string ShipCountText
		{
			get
			{
				return this._shipCountText;
			}
			set
			{
				if (value != this._shipCountText)
				{
					this._shipCountText = value;
					base.OnPropertyChangedWithValue<string>(value, "ShipCountText");
				}
			}
		}

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x0600012E RID: 302 RVA: 0x00008CA1 File Offset: 0x00006EA1
		// (set) Token: 0x0600012F RID: 303 RVA: 0x00008CA9 File Offset: 0x00006EA9
		[DataSourceProperty]
		public string WeightText
		{
			get
			{
				return this._weightText;
			}
			set
			{
				if (value != this._weightText)
				{
					this._weightText = value;
					base.OnPropertyChangedWithValue<string>(value, "WeightText");
				}
			}
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x06000130 RID: 304 RVA: 0x00008CCC File Offset: 0x00006ECC
		// (set) Token: 0x06000131 RID: 305 RVA: 0x00008CD4 File Offset: 0x00006ED4
		[DataSourceProperty]
		public string TroopCountText
		{
			get
			{
				return this._troopCountText;
			}
			set
			{
				if (value != this._troopCountText)
				{
					this._troopCountText = value;
					base.OnPropertyChangedWithValue<string>(value, "TroopCountText");
				}
			}
		}

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x06000132 RID: 306 RVA: 0x00008CF7 File Offset: 0x00006EF7
		// (set) Token: 0x06000133 RID: 307 RVA: 0x00008CFF File Offset: 0x00006EFF
		[DataSourceProperty]
		public bool IsWeightDangerous
		{
			get
			{
				return this._isWeightDangerous;
			}
			set
			{
				if (value != this._isWeightDangerous)
				{
					this._isWeightDangerous = value;
					base.OnPropertyChangedWithValue(value, "IsWeightDangerous");
				}
			}
		}

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x06000134 RID: 308 RVA: 0x00008D1D File Offset: 0x00006F1D
		// (set) Token: 0x06000135 RID: 309 RVA: 0x00008D25 File Offset: 0x00006F25
		[DataSourceProperty]
		public bool IsTroopCountDangerous
		{
			get
			{
				return this._isTroopCountDangerous;
			}
			set
			{
				if (value != this._isTroopCountDangerous)
				{
					this._isTroopCountDangerous = value;
					base.OnPropertyChangedWithValue(value, "IsTroopCountDangerous");
				}
			}
		}

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x06000136 RID: 310 RVA: 0x00008D43 File Offset: 0x00006F43
		// (set) Token: 0x06000137 RID: 311 RVA: 0x00008D4B File Offset: 0x00006F4B
		[DataSourceProperty]
		public MBBindingList<ShipItemVM> Ships
		{
			get
			{
				return this._ships;
			}
			set
			{
				if (value != this._ships)
				{
					this._ships = value;
					base.OnPropertyChangedWithValue<MBBindingList<ShipItemVM>>(value, "Ships");
				}
			}
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x06000138 RID: 312 RVA: 0x00008D69 File Offset: 0x00006F69
		// (set) Token: 0x06000139 RID: 313 RVA: 0x00008D71 File Offset: 0x00006F71
		[DataSourceProperty]
		public CharacterImageIdentifierVM OwnerCharacterVisual
		{
			get
			{
				return this._ownerVisual;
			}
			set
			{
				if (value != this._ownerVisual)
				{
					this._ownerVisual = value;
					base.OnPropertyChangedWithValue<CharacterImageIdentifierVM>(value, "OwnerCharacterVisual");
				}
			}
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x0600013A RID: 314 RVA: 0x00008D8F File Offset: 0x00006F8F
		// (set) Token: 0x0600013B RID: 315 RVA: 0x00008D97 File Offset: 0x00006F97
		[DataSourceProperty]
		public HintViewModel Tooltip
		{
			get
			{
				return this._tooltip;
			}
			set
			{
				if (value != this._tooltip)
				{
					this._tooltip = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "Tooltip");
				}
			}
		}

		// Token: 0x04000065 RID: 101
		private TextObject _rosterName;

		// Token: 0x04000066 RID: 102
		private readonly Action _onSelected;

		// Token: 0x04000067 RID: 103
		private bool _hasAnyShips;

		// Token: 0x04000068 RID: 104
		private bool _hasMultipleShips;

		// Token: 0x04000069 RID: 105
		private bool _hasOwnerCharacter;

		// Token: 0x0400006A RID: 106
		private bool _isSelected;

		// Token: 0x0400006B RID: 107
		private bool _isTownShipyard;

		// Token: 0x0400006C RID: 108
		private int _townShipyardLevel;

		// Token: 0x0400006D RID: 109
		private string _name;

		// Token: 0x0400006E RID: 110
		private string _hasNoShipsText;

		// Token: 0x0400006F RID: 111
		private string _shipCountText;

		// Token: 0x04000070 RID: 112
		private string _weightText;

		// Token: 0x04000071 RID: 113
		private string _troopCountText;

		// Token: 0x04000072 RID: 114
		private bool _isWeightDangerous;

		// Token: 0x04000073 RID: 115
		private bool _isTroopCountDangerous;

		// Token: 0x04000074 RID: 116
		private MBBindingList<ShipItemVM> _ships;

		// Token: 0x04000075 RID: 117
		private CharacterImageIdentifierVM _ownerVisual;

		// Token: 0x04000076 RID: 118
		private HintViewModel _tooltip;

		// Token: 0x0200004B RID: 75
		private class PortShipVMComparer : IComparer<ShipItemVM>
		{
			// Token: 0x06000489 RID: 1161 RVA: 0x00014ACF File Offset: 0x00012CCF
			public PortShipVMComparer(MBReadOnlyList<Ship> orderedShipsList)
			{
				this._orderedShipsList = orderedShipsList;
			}

			// Token: 0x0600048A RID: 1162 RVA: 0x00014AE0 File Offset: 0x00012CE0
			public int Compare(ShipItemVM x, ShipItemVM y)
			{
				int num = this._orderedShipsList.IndexOf(x.Ship);
				int num2 = this._orderedShipsList.IndexOf(y.Ship);
				return num.CompareTo(num2);
			}

			// Token: 0x040001C7 RID: 455
			private readonly MBReadOnlyList<Ship> _orderedShipsList;
		}
	}
}
