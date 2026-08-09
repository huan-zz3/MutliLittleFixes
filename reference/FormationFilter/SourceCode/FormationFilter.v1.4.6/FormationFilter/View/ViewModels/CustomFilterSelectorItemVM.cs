using System;
using System.Runtime.CompilerServices;
using FormationFilter.Models;
using FormationFilter.Utilities;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace FormationFilter.View.ViewModels
{
	// Token: 0x0200000B RID: 11
	[NullableContext(1)]
	[Nullable(0)]
	public class CustomFilterSelectorItemVM : ViewModel
	{
		// Token: 0x0600002E RID: 46 RVA: 0x000026D4 File Offset: 0x000008D4
		public CustomFilterSelectorItemVM(CustomFormationFilterType filterType, Action<CustomFilterSelectorItemVM> onToggled, FilterSelectorMode filterSelectorMode)
		{
			this.FilterType = filterType;
			this.FilterTypeValue = (int)filterType;
			this._onToggled = onToggled;
			this._selectorMode = filterSelectorMode;
			this.RefreshValues();
		}

		// Token: 0x0600002F RID: 47 RVA: 0x000026FE File Offset: 0x000008FE
		public override void RefreshValues()
		{
			this.Hint = new HintViewModel(this.FilterType.GetFilterDescription(), null);
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00002718 File Offset: 0x00000918
		public void ExecuteAction()
		{
			switch (this._selectorMode)
			{
			case FilterSelectorMode.InludeOnly:
				if (this.IsIncluded)
				{
					this.SetFilterValue(FilterValueEnum.Any);
				}
				else
				{
					this.SetFilterValue(FilterValueEnum.Yes);
				}
				break;
			}
			if (this.IsIncluded)
			{
				this.IsIncluded = false;
				this.IsExcluded = true;
			}
			else if (this.IsExcluded)
			{
				this.IsIncluded = false;
				this.IsExcluded = false;
			}
			else
			{
				this.IsIncluded = true;
				this.IsExcluded = false;
			}
			this.UpdateIsSelected();
			Action<CustomFilterSelectorItemVM> onToggled = this._onToggled;
			if (onToggled == null)
			{
				return;
			}
			onToggled(this);
		}

		// Token: 0x06000031 RID: 49 RVA: 0x000027AE File Offset: 0x000009AE
		public void UpdateFilterValueFromTeamFilter(FilterValueEnum filterEnum)
		{
			this.SetFilterValue(filterEnum);
			this.UpdateIsSelected();
		}

		// Token: 0x06000032 RID: 50 RVA: 0x000027C0 File Offset: 0x000009C0
		private void SetFilterValue(FilterValueEnum filterValue)
		{
			switch (filterValue)
			{
			case FilterValueEnum.Invalid:
			case FilterValueEnum.Any:
				this.IsIncluded = false;
				this.IsExcluded = false;
				return;
			case FilterValueEnum.Yes:
				this.IsIncluded = true;
				this.IsExcluded = false;
				return;
			case FilterValueEnum.No:
				this.IsIncluded = false;
				this.IsExcluded = true;
				return;
			default:
				return;
			}
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00002810 File Offset: 0x00000A10
		private void UpdateIsSelected()
		{
			this.IsSelected = this.IsIncluded | this.IsExcluded;
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000034 RID: 52 RVA: 0x00002825 File Offset: 0x00000A25
		// (set) Token: 0x06000035 RID: 53 RVA: 0x0000282D File Offset: 0x00000A2D
		[DataSourceProperty]
		public int FilterTypeValue
		{
			get
			{
				return this._filterType;
			}
			set
			{
				if (value == this._filterType)
				{
					return;
				}
				this._filterType = value;
				base.OnPropertyChangedWithValue(value, "FilterTypeValue");
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000036 RID: 54 RVA: 0x0000284C File Offset: 0x00000A4C
		// (set) Token: 0x06000037 RID: 55 RVA: 0x00002854 File Offset: 0x00000A54
		[DataSourceProperty]
		public bool IsIncluded
		{
			get
			{
				return this._isIncluded;
			}
			set
			{
				if (value == this._isIncluded)
				{
					return;
				}
				this._isIncluded = value;
				this.UpdateIsSelected();
				base.OnPropertyChangedWithValue(value, "IsIncluded");
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000038 RID: 56 RVA: 0x00002879 File Offset: 0x00000A79
		// (set) Token: 0x06000039 RID: 57 RVA: 0x00002881 File Offset: 0x00000A81
		[DataSourceProperty]
		public bool IsExcluded
		{
			get
			{
				return this._isExcluded;
			}
			set
			{
				if (value == this._isExcluded)
				{
					return;
				}
				this._isExcluded = value;
				this.UpdateIsSelected();
				base.OnPropertyChangedWithValue(value, "IsExcluded");
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600003A RID: 58 RVA: 0x000028A6 File Offset: 0x00000AA6
		// (set) Token: 0x0600003B RID: 59 RVA: 0x000028AE File Offset: 0x00000AAE
		[DataSourceProperty]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (value == this._isSelected)
				{
					return;
				}
				this._isSelected = value;
				base.OnPropertyChangedWithValue(value, "IsSelected");
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600003C RID: 60 RVA: 0x000028CD File Offset: 0x00000ACD
		// (set) Token: 0x0600003D RID: 61 RVA: 0x000028D5 File Offset: 0x00000AD5
		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value == this._isEnabled)
				{
					return;
				}
				this._isEnabled = value;
				base.OnPropertyChangedWithValue(value, "IsEnabled");
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600003E RID: 62 RVA: 0x000028F4 File Offset: 0x00000AF4
		// (set) Token: 0x0600003F RID: 63 RVA: 0x000028FC File Offset: 0x00000AFC
		[DataSourceProperty]
		public HintViewModel Hint
		{
			get
			{
				return this._hint;
			}
			set
			{
				if (value == this._hint)
				{
					return;
				}
				this._hint = value;
				base.OnPropertyChangedWithValue<HintViewModel>(value, "Hint");
			}
		}

		// Token: 0x0400002B RID: 43
		public readonly CustomFormationFilterType FilterType;

		// Token: 0x0400002C RID: 44
		private Action<CustomFilterSelectorItemVM> _onToggled;

		// Token: 0x0400002D RID: 45
		private int _filterType;

		// Token: 0x0400002E RID: 46
		private bool _isIncluded;

		// Token: 0x0400002F RID: 47
		private bool _isExcluded;

		// Token: 0x04000030 RID: 48
		private bool _isSelected;

		// Token: 0x04000031 RID: 49
		private bool _isEnabled;

		// Token: 0x04000032 RID: 50
		private HintViewModel _hint;

		// Token: 0x04000033 RID: 51
		private FilterSelectorMode _selectorMode;
	}
}
