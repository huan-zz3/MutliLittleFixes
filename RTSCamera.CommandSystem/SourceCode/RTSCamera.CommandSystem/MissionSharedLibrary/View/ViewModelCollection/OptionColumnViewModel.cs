using System;
using System.Collections.Generic;
using MissionLibrary.View;
using TaleWorlds.Library;

namespace MissionSharedLibrary.View.ViewModelCollection
{
	// Token: 0x02000022 RID: 34
	public class OptionColumnViewModel : ViewModel
	{
		// Token: 0x1700003A RID: 58
		// (get) Token: 0x06000133 RID: 307 RVA: 0x00005A84 File Offset: 0x00003C84
		// (set) Token: 0x06000134 RID: 308 RVA: 0x00005A8C File Offset: 0x00003C8C
		[DataSourceProperty]
		public MBBindingList<ViewModel> Categories
		{
			get
			{
				return this._categories;
			}
			set
			{
				if (this._categories == value)
				{
					return;
				}
				this._categories = value;
				base.OnPropertyChanged("Categories");
			}
		}

		// Token: 0x06000135 RID: 309 RVA: 0x00005AAA File Offset: 0x00003CAA
		public OptionColumnViewModel()
		{
			this.Categories = new MBBindingList<ViewModel>();
		}

		// Token: 0x06000136 RID: 310 RVA: 0x00005AC8 File Offset: 0x00003CC8
		public void AddOptionCategory(IOptionCategory optionCategory)
		{
			int num = this._optionCategories.FindIndex((IOptionCategory o) => o.Id == optionCategory.Id);
			if (num < 0)
			{
				this._optionCategories.Add(optionCategory);
				this.Categories.Add(optionCategory.GetViewModel());
				return;
			}
			this._optionCategories[num] = optionCategory;
			this.Categories[num] = optionCategory.GetViewModel();
		}

		// Token: 0x06000137 RID: 311 RVA: 0x00005B4F File Offset: 0x00003D4F
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Refresh();
		}

		// Token: 0x06000138 RID: 312 RVA: 0x00005B60 File Offset: 0x00003D60
		private void Refresh()
		{
			foreach (ViewModel viewModel in this.Categories)
			{
				viewModel.RefreshValues();
			}
		}

		// Token: 0x04000078 RID: 120
		private readonly List<IOptionCategory> _optionCategories = new List<IOptionCategory>();

		// Token: 0x04000079 RID: 121
		private MBBindingList<ViewModel> _categories;
	}
}
