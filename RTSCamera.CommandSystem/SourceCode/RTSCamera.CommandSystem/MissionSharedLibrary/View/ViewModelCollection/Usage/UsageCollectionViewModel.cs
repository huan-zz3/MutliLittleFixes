using System;
using System.Linq;
using MissionLibrary.Provider;
using MissionLibrary.Usage;
using MissionSharedLibrary.View.ViewModelCollection.Basic;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace MissionSharedLibrary.View.ViewModelCollection.Usage
{
	// Token: 0x0200002F RID: 47
	public class UsageCollectionViewModel : ViewModel
	{
		// Token: 0x06000199 RID: 409 RVA: 0x000066D8 File Offset: 0x000048D8
		public UsageCollectionViewModel(TextObject title, AUsageCategoryManager usageCategoryManager, Action onClose)
		{
			this.Title = new TextViewModel(title, true);
			this._usageCategoryManager = usageCategoryManager;
			this._onClose = onClose;
			foreach (AUsageCategory ausageCategory in this._usageCategoryManager.Items.Values.Select<IProvider<AUsageCategory>, AUsageCategory>((IProvider<AUsageCategory> v) => v.Value))
			{
				this.UsageCategoryContainerViewModels.Add(new UsageCategoryContainerViewModel(ausageCategory, new Action<UsageCategoryContainerViewModel>(this.OnUsageCategorySelected)));
			}
			this.OnUsageCategorySelected(this.UsageCategoryContainerViewModels.FirstOrDefault<UsageCategoryContainerViewModel>());
		}

		// Token: 0x0600019A RID: 410 RVA: 0x000067A8 File Offset: 0x000049A8
		public void OnUsageCategorySelected(UsageCategoryContainerViewModel usageCategoryViewModel)
		{
			if (this.CurrentUsageCategoryContainerViewModel == usageCategoryViewModel)
			{
				return;
			}
			UsageCategoryContainerViewModel currentUsageCategoryContainerViewModel = this.CurrentUsageCategoryContainerViewModel;
			if (currentUsageCategoryContainerViewModel != null)
			{
				currentUsageCategoryContainerViewModel.UpdateSelection(false);
			}
			this.CurrentUsageCategoryContainerViewModel = usageCategoryViewModel;
			UsageCategoryContainerViewModel currentUsageCategoryContainerViewModel2 = this.CurrentUsageCategoryContainerViewModel;
			if (currentUsageCategoryContainerViewModel2 == null)
			{
				return;
			}
			currentUsageCategoryContainerViewModel2.UpdateSelection(true);
		}

		// Token: 0x0600019B RID: 411 RVA: 0x000067E0 File Offset: 0x000049E0
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Title.RefreshValues();
			foreach (UsageCategoryContainerViewModel usageCategoryContainerViewModel in this.UsageCategoryContainerViewModels)
			{
				usageCategoryContainerViewModel.RefreshValues();
			}
			UsageCategoryContainerViewModel currentUsageCategoryContainerViewModel = this.CurrentUsageCategoryContainerViewModel;
			if (currentUsageCategoryContainerViewModel == null)
			{
				return;
			}
			currentUsageCategoryContainerViewModel.RefreshValues();
		}

		// Token: 0x0600019C RID: 412 RVA: 0x0000684C File Offset: 0x00004A4C
		public void OnNext()
		{
			int num = Extensions.FindIndex<UsageCategoryContainerViewModel>(this.UsageCategoryContainerViewModels, (UsageCategoryContainerViewModel viewModel) => viewModel == this.CurrentUsageCategoryContainerViewModel);
			if (num == -1)
			{
				this.OnUsageCategorySelected(this.UsageCategoryContainerViewModels.FirstOrDefault<UsageCategoryContainerViewModel>());
				return;
			}
			if (num != this.UsageCategoryContainerViewModels.Count - 1)
			{
				this.OnUsageCategorySelected(this.UsageCategoryContainerViewModels[num + 1]);
				return;
			}
			Action onClose = this._onClose;
			if (onClose == null)
			{
				return;
			}
			onClose();
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x0600019D RID: 413 RVA: 0x000068BC File Offset: 0x00004ABC
		[DataSourceProperty]
		public TextViewModel Title { get; }

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x0600019E RID: 414 RVA: 0x000068C4 File Offset: 0x00004AC4
		[DataSourceProperty]
		public MBBindingList<UsageCategoryContainerViewModel> UsageCategoryContainerViewModels { get; } = new MBBindingList<UsageCategoryContainerViewModel>();

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x0600019F RID: 415 RVA: 0x000068CC File Offset: 0x00004ACC
		// (set) Token: 0x060001A0 RID: 416 RVA: 0x000068D4 File Offset: 0x00004AD4
		[DataSourceProperty]
		public UsageCategoryContainerViewModel CurrentUsageCategoryContainerViewModel
		{
			get
			{
				return this._currentUsageCategoryContainerViewModel;
			}
			set
			{
				if (this._currentUsageCategoryContainerViewModel == value)
				{
					return;
				}
				this._currentUsageCategoryContainerViewModel = value;
				base.OnPropertyChanged("CurrentUsageCategoryContainerViewModel");
			}
		}

		// Token: 0x040000A9 RID: 169
		private UsageCategoryContainerViewModel _currentUsageCategoryContainerViewModel;

		// Token: 0x040000AA RID: 170
		private AUsageCategoryManager _usageCategoryManager;

		// Token: 0x040000AB RID: 171
		private readonly Action _onClose;
	}
}
