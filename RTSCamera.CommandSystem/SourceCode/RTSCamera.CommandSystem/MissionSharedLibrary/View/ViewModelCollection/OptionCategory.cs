using System;
using System.Collections.Generic;
using MissionLibrary.View;
using MissionSharedLibrary.View.ViewModelCollection.Basic;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace MissionSharedLibrary.View.ViewModelCollection
{
	// Token: 0x0200001F RID: 31
	public class OptionCategory : ViewModel, IOptionCategory, IViewModelProvider<ViewModel>
	{
		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000118 RID: 280 RVA: 0x0000575C File Offset: 0x0000395C
		public string Id { get; }

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x06000119 RID: 281 RVA: 0x00005764 File Offset: 0x00003964
		public TextViewModel Title { get; }

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x0600011A RID: 282 RVA: 0x0000576C File Offset: 0x0000396C
		// (set) Token: 0x0600011B RID: 283 RVA: 0x00005774 File Offset: 0x00003974
		[DataSourceProperty]
		public bool IsTargetVisible
		{
			get
			{
				return this._isTargetVisible;
			}
			set
			{
				if (this._isTargetVisible == value)
				{
					return;
				}
				this._isTargetVisible = value;
				base.OnPropertyChanged("IsTargetVisible");
				this._onVisibleChanged(this.IsTargetVisible);
			}
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x0600011C RID: 284 RVA: 0x000057A3 File Offset: 0x000039A3
		// (set) Token: 0x0600011D RID: 285 RVA: 0x000057AB File Offset: 0x000039AB
		[DataSourceProperty]
		public MBBindingList<ViewModel> OptionViewModels
		{
			get
			{
				return this._optionViewModels;
			}
			set
			{
				if (this._optionViewModels == value)
				{
					return;
				}
				this._optionViewModels = value;
				base.OnPropertyChanged("OptionViewModels");
			}
		}

		// Token: 0x0600011E RID: 286 RVA: 0x000057CC File Offset: 0x000039CC
		public OptionCategory(string id, TextObject title, Func<bool> isVisible, Action<bool> onVisibleChanged)
		{
			this.Id = id;
			this.Title = new TextViewModel(title, true);
			this.OptionViewModels = new MBBindingList<ViewModel>();
			this._onVisibleChanged = onVisibleChanged;
			this._isVisible = isVisible;
		}

		// Token: 0x0600011F RID: 287 RVA: 0x0000581F File Offset: 0x00003A1F
		public void AddOption(IOption option)
		{
			this._options.Add(option);
			this.OptionViewModels.Add(option.GetViewModel());
		}

		// Token: 0x06000120 RID: 288 RVA: 0x00005840 File Offset: 0x00003A40
		public override void RefreshValues()
		{
			base.RefreshValues();
			bool flag = this._isVisible();
			this.IsTargetVisible = flag;
			foreach (ViewModel viewModel in this.OptionViewModels)
			{
				viewModel.RefreshValues();
			}
		}

		// Token: 0x06000121 RID: 289 RVA: 0x000058A4 File Offset: 0x00003AA4
		public ViewModel GetViewModel()
		{
			return this;
		}

		// Token: 0x04000068 RID: 104
		private readonly List<IOption> _options = new List<IOption>();

		// Token: 0x04000069 RID: 105
		private MBBindingList<ViewModel> _optionViewModels;

		// Token: 0x0400006A RID: 106
		private bool _isTargetVisible = true;

		// Token: 0x0400006B RID: 107
		private Func<bool> _isVisible;

		// Token: 0x0400006C RID: 108
		private Action<bool> _onVisibleChanged;
	}
}
