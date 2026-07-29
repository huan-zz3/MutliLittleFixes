using System;
using MissionLibrary.Usage;
using MissionSharedLibrary.View.ViewModelCollection.Usage;
using TaleWorlds.Library;

namespace MissionSharedLibrary.Usage
{
	// Token: 0x0200000F RID: 15
	public class UsageCategory : AUsageCategory
	{
		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000096 RID: 150 RVA: 0x0000458D File Offset: 0x0000278D
		public override string ItemId { get; }

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000097 RID: 151 RVA: 0x00004595 File Offset: 0x00002795
		public override ViewModel ViewModel
		{
			get
			{
				return this._viewModel;
			}
		}

		// Token: 0x06000098 RID: 152 RVA: 0x0000459D File Offset: 0x0000279D
		public UsageCategory(string itemId, UsageCategoryData usageCategoryData)
		{
			this.ItemId = itemId;
			this._viewModel = new UsageCategoryViewModel(usageCategoryData);
		}

		// Token: 0x06000099 RID: 153 RVA: 0x000045B8 File Offset: 0x000027B8
		public override void UpdateSelection(bool isSelected)
		{
			this._viewModel.UpdateSelection(isSelected);
		}

		// Token: 0x04000034 RID: 52
		private readonly UsageCategoryViewModel _viewModel;
	}
}
