using System;
using System.Collections.Generic;
using System.Linq;
using MissionLibrary.Provider;
using MissionLibrary.View;
using MissionSharedLibrary.Utilities;
using TaleWorlds.Library;

namespace MissionSharedLibrary.View.ViewModelCollection
{
	// Token: 0x0200001E RID: 30
	public class MenuClassCollectionViewModel : ViewModel
	{
		// Token: 0x1700002E RID: 46
		// (get) Token: 0x0600010E RID: 270 RVA: 0x00005561 File Offset: 0x00003761
		// (set) Token: 0x0600010F RID: 271 RVA: 0x00005569 File Offset: 0x00003769
		public AOptionClass CurrentSelectedOptionClass
		{
			get
			{
				return this._currentSelectedOptionClass;
			}
			private set
			{
				if (this._currentSelectedOptionClass == value)
				{
					return;
				}
				this._currentSelectedOptionClass = value;
				AOptionClass currentSelectedOptionClass = this._currentSelectedOptionClass;
				this.CurrentOptionClassViewModel = ((currentSelectedOptionClass != null) ? currentSelectedOptionClass.GetViewModel() : null);
			}
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06000110 RID: 272 RVA: 0x00005594 File Offset: 0x00003794
		// (set) Token: 0x06000111 RID: 273 RVA: 0x0000559C File Offset: 0x0000379C
		[DataSourceProperty]
		public MBBindingList<ViewModel> OptionClassViewModels
		{
			get
			{
				return this._optionClassViewModels;
			}
			set
			{
				if (this._optionClassViewModels == value)
				{
					return;
				}
				this._optionClassViewModels = value;
				base.OnPropertyChanged("OptionClassViewModels");
			}
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x06000112 RID: 274 RVA: 0x000055BA File Offset: 0x000037BA
		// (set) Token: 0x06000113 RID: 275 RVA: 0x000055C2 File Offset: 0x000037C2
		[DataSourceProperty]
		public ViewModel CurrentOptionClassViewModel
		{
			get
			{
				return this._currentOptionClassViewModel;
			}
			set
			{
				if (this._currentOptionClassViewModel == value)
				{
					return;
				}
				this._currentOptionClassViewModel = value;
				base.OnPropertyChanged("CurrentOptionClassViewModel");
			}
		}

		// Token: 0x06000114 RID: 276 RVA: 0x000055E0 File Offset: 0x000037E0
		public void OnOptionClassSelected(AOptionClass optionClass)
		{
			if (this.CurrentSelectedOptionClass == optionClass)
			{
				return;
			}
			AOptionClass currentSelectedOptionClass = this.CurrentSelectedOptionClass;
			if (currentSelectedOptionClass != null)
			{
				currentSelectedOptionClass.UpdateSelection(false);
			}
			this.CurrentSelectedOptionClass = optionClass;
			AOptionClass currentSelectedOptionClass2 = this.CurrentSelectedOptionClass;
			if (currentSelectedOptionClass2 == null)
			{
				return;
			}
			currentSelectedOptionClass2.UpdateSelection(true);
		}

		// Token: 0x06000115 RID: 277 RVA: 0x00005618 File Offset: 0x00003818
		public MenuClassCollectionViewModel(List<IProvider<AOptionClass>> optionClasses, string selectedOptionClassId)
		{
			MBBindingList<ViewModel> mbbindingList = new MBBindingList<ViewModel>();
			foreach (IProvider<AOptionClass> provider in optionClasses)
			{
				try
				{
					mbbindingList.Add(provider.Value.GetViewModel());
				}
				catch (Exception ex)
				{
					Utility.DisplayMessageForced(ex.ToString());
					Console.WriteLine(ex);
				}
			}
			this.OptionClassViewModels = mbbindingList;
			try
			{
				IProvider<AOptionClass> provider2 = optionClasses.FirstOrDefault<IProvider<AOptionClass>>((IProvider<AOptionClass> optionClass) => optionClass.Value.ItemId == selectedOptionClassId) ?? optionClasses.FirstOrDefault<IProvider<AOptionClass>>();
				this.OnOptionClassSelected((provider2 != null) ? provider2.Value : null);
			}
			catch (Exception ex2)
			{
				Utility.DisplayMessageForced(ex2.ToString());
				Console.WriteLine(ex2);
			}
		}

		// Token: 0x06000116 RID: 278 RVA: 0x00005700 File Offset: 0x00003900
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Refresh();
		}

		// Token: 0x06000117 RID: 279 RVA: 0x00005710 File Offset: 0x00003910
		private void Refresh()
		{
			foreach (ViewModel viewModel in this.OptionClassViewModels)
			{
				viewModel.RefreshValues();
			}
		}

		// Token: 0x04000065 RID: 101
		private MBBindingList<ViewModel> _optionClassViewModels;

		// Token: 0x04000066 RID: 102
		private AOptionClass _currentSelectedOptionClass;

		// Token: 0x04000067 RID: 103
		private ViewModel _currentOptionClassViewModel;
	}
}
