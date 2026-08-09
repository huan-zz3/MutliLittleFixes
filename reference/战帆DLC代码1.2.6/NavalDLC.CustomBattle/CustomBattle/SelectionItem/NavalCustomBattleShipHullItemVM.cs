using System;
using System.Collections.Generic;
using System.Linq;
using NavalDLC.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace NavalDLC.CustomBattle.CustomBattle.SelectionItem
{
	// Token: 0x02000022 RID: 34
	public class NavalCustomBattleShipHullItemVM : ViewModel
	{
		// Token: 0x060001F2 RID: 498 RVA: 0x00009164 File Offset: 0x00007364
		public NavalCustomBattleShipHullItemVM(ShipHull shipHull, TextObject disabledHintText, Action<NavalCustomBattleShipHullItemVM> onSelected)
		{
			this.ShipHull = shipHull;
			this.PrefabId = NavalUIHelper.GetPrefabIdOfShipHull(this.ShipHull);
			this._nameText = this.ShipHull.Name;
			this.Tooltip = new BasicTooltipViewModel(() => this.GetTooltip());
			this.DisabledHint = new HintViewModel(disabledHintText, null);
			this._onSelected = onSelected;
			this.IsEmpty = false;
			this.RefreshValues();
		}

		// Token: 0x060001F3 RID: 499 RVA: 0x000091D8 File Offset: 0x000073D8
		public NavalCustomBattleShipHullItemVM(TextObject nameText, TextObject disabledHintText, Action<NavalCustomBattleShipHullItemVM> onSelected)
		{
			this._nameText = nameText;
			this._onSelected = onSelected;
			this.DisabledHint = new HintViewModel(disabledHintText, null);
			this.IsEmpty = true;
			this.RefreshValues();
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x00009208 File Offset: 0x00007408
		protected virtual List<TooltipProperty> GetTooltip()
		{
			object[] array = new object[] { this.ShipHull };
			return new PropertyBasedTooltipVM(typeof(ShipHull), array).TooltipPropertyList.ToList<TooltipProperty>();
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x0000923F File Offset: 0x0000743F
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this._nameText.ToString();
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x00009258 File Offset: 0x00007458
		public void ExecuteSelect()
		{
			Action<NavalCustomBattleShipHullItemVM> onSelected = this._onSelected;
			if (onSelected == null)
			{
				return;
			}
			onSelected(this);
		}

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x060001F7 RID: 503 RVA: 0x0000926B File Offset: 0x0000746B
		// (set) Token: 0x060001F8 RID: 504 RVA: 0x00009273 File Offset: 0x00007473
		[DataSourceProperty]
		public BasicTooltipViewModel Tooltip
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
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "Tooltip");
				}
			}
		}

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x060001F9 RID: 505 RVA: 0x00009291 File Offset: 0x00007491
		// (set) Token: 0x060001FA RID: 506 RVA: 0x00009299 File Offset: 0x00007499
		[DataSourceProperty]
		public HintViewModel DisabledHint
		{
			get
			{
				return this._disabledHint;
			}
			set
			{
				if (value != this._disabledHint)
				{
					this._disabledHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "DisabledHint");
				}
			}
		}

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x060001FB RID: 507 RVA: 0x000092B7 File Offset: 0x000074B7
		// (set) Token: 0x060001FC RID: 508 RVA: 0x000092BF File Offset: 0x000074BF
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

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x060001FD RID: 509 RVA: 0x000092E2 File Offset: 0x000074E2
		// (set) Token: 0x060001FE RID: 510 RVA: 0x000092EA File Offset: 0x000074EA
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

		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x060001FF RID: 511 RVA: 0x00009308 File Offset: 0x00007508
		// (set) Token: 0x06000200 RID: 512 RVA: 0x00009310 File Offset: 0x00007510
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

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x06000201 RID: 513 RVA: 0x0000932E File Offset: 0x0000752E
		// (set) Token: 0x06000202 RID: 514 RVA: 0x00009336 File Offset: 0x00007536
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
				}
			}
		}

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x06000203 RID: 515 RVA: 0x00009354 File Offset: 0x00007554
		// (set) Token: 0x06000204 RID: 516 RVA: 0x0000935C File Offset: 0x0000755C
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

		// Token: 0x040000EF RID: 239
		public readonly ShipHull ShipHull;

		// Token: 0x040000F0 RID: 240
		private readonly TextObject _nameText;

		// Token: 0x040000F1 RID: 241
		private readonly Action<NavalCustomBattleShipHullItemVM> _onSelected;

		// Token: 0x040000F2 RID: 242
		private BasicTooltipViewModel _tooltip;

		// Token: 0x040000F3 RID: 243
		private HintViewModel _disabledHint;

		// Token: 0x040000F4 RID: 244
		private string _name;

		// Token: 0x040000F5 RID: 245
		private bool _isSelected;

		// Token: 0x040000F6 RID: 246
		private bool _isDisabled;

		// Token: 0x040000F7 RID: 247
		private bool _isEmpty;

		// Token: 0x040000F8 RID: 248
		private string _prefabId;
	}
}
