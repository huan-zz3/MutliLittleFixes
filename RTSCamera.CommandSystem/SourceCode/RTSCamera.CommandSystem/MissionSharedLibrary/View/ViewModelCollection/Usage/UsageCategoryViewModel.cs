using System;
using MissionLibrary.src.View;
using MissionSharedLibrary.Usage;
using MissionSharedLibrary.View.ViewModelCollection.Basic;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace MissionSharedLibrary.View.ViewModelCollection.Usage
{
	// Token: 0x0200002E RID: 46
	public class UsageCategoryViewModel : AUsageCategoryViewModel
	{
		// Token: 0x06000192 RID: 402 RVA: 0x000065B0 File Offset: 0x000047B0
		public UsageCategoryViewModel(UsageCategoryData usageCategoryData)
		{
			this.Name = new TextViewModel(usageCategoryData.Name, true);
			foreach (TextObject textObject in usageCategoryData.UsageList)
			{
				this.UsageList.Add(new TextViewModel(textObject, true));
			}
		}

		// Token: 0x06000193 RID: 403 RVA: 0x00006634 File Offset: 0x00004834
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name.RefreshValues();
			foreach (TextViewModel textViewModel in this.UsageList)
			{
				textViewModel.RefreshValues();
			}
		}

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x06000194 RID: 404 RVA: 0x00006690 File Offset: 0x00004890
		[DataSourceProperty]
		public TextViewModel Name { get; }

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x06000195 RID: 405 RVA: 0x00006698 File Offset: 0x00004898
		[DataSourceProperty]
		public MBBindingList<TextViewModel> UsageList { get; } = new MBBindingList<TextViewModel>();

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x06000196 RID: 406 RVA: 0x000066A0 File Offset: 0x000048A0
		// (set) Token: 0x06000197 RID: 407 RVA: 0x000066A8 File Offset: 0x000048A8
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

		// Token: 0x06000198 RID: 408 RVA: 0x000066CC File Offset: 0x000048CC
		public override void UpdateSelection(bool isSelected)
		{
			this.IsSelected = isSelected;
		}

		// Token: 0x040000A8 RID: 168
		private bool _isSelected;
	}
}
