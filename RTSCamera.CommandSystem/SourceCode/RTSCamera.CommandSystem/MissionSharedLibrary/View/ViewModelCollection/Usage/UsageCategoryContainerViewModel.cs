using System;
using MissionLibrary.src.View;
using MissionLibrary.Usage;
using TaleWorlds.Library;

namespace MissionSharedLibrary.View.ViewModelCollection.Usage
{
	// Token: 0x0200002D RID: 45
	public class UsageCategoryContainerViewModel : AUsageCategoryViewModel
	{
		// Token: 0x0600018B RID: 395 RVA: 0x0000651C File Offset: 0x0000471C
		public UsageCategoryContainerViewModel(AUsageCategory usageCategory, Action<UsageCategoryContainerViewModel> onSelect)
		{
			this._usageCategory = usageCategory;
			this.UsageCategoryViewModel = usageCategory.ViewModel;
			this._onSelect = onSelect;
		}

		// Token: 0x0600018C RID: 396 RVA: 0x0000653E File Offset: 0x0000473E
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.UsageCategoryViewModel.RefreshValues();
		}

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x0600018D RID: 397 RVA: 0x00006551 File Offset: 0x00004751
		public ViewModel UsageCategoryViewModel { get; }

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x0600018E RID: 398 RVA: 0x00006559 File Offset: 0x00004759
		// (set) Token: 0x0600018F RID: 399 RVA: 0x00006561 File Offset: 0x00004761
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
				base.OnPropertyChangedWithValue<object>(value, "IsSelected");
			}
		}

		// Token: 0x06000190 RID: 400 RVA: 0x00006585 File Offset: 0x00004785
		public override void UpdateSelection(bool isSelected)
		{
			this.IsSelected = isSelected;
			this._usageCategory.UpdateSelection(isSelected);
		}

		// Token: 0x06000191 RID: 401 RVA: 0x0000659A File Offset: 0x0000479A
		public void ExecuteSelection()
		{
			Action<UsageCategoryContainerViewModel> onSelect = this._onSelect;
			if (onSelect == null)
			{
				return;
			}
			onSelect(this);
		}

		// Token: 0x040000A2 RID: 162
		private readonly AUsageCategory _usageCategory;

		// Token: 0x040000A3 RID: 163
		private readonly Action<UsageCategoryContainerViewModel> _onSelect;

		// Token: 0x040000A4 RID: 164
		private bool _isSelected;
	}
}
