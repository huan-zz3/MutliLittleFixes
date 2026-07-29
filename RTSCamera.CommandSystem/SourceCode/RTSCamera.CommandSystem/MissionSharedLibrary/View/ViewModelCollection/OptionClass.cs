using System;
using MissionLibrary.View;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace MissionSharedLibrary.View.ViewModelCollection
{
	// Token: 0x02000020 RID: 32
	public class OptionClass : AOptionClass
	{
		// Token: 0x06000122 RID: 290 RVA: 0x000058A7 File Offset: 0x00003AA7
		public OptionClass(string id, TextObject name, AMenuClassCollection menuClassCollection)
		{
			this.ItemId = id;
			this._viewModel = new OptionClassViewModel(id, name, new Action(this.OnSelect));
			this._menuClassCollection = menuClassCollection;
		}

		// Token: 0x06000123 RID: 291 RVA: 0x000058D6 File Offset: 0x00003AD6
		public void AddOptionCategory(int column, IOptionCategory optionCategory)
		{
			this._viewModel.AddOptionCategory(column, optionCategory);
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x06000124 RID: 292 RVA: 0x000058E5 File Offset: 0x00003AE5
		public override string ItemId { get; }

		// Token: 0x06000125 RID: 293 RVA: 0x000058ED File Offset: 0x00003AED
		public override ViewModel GetViewModel()
		{
			return this._viewModel;
		}

		// Token: 0x06000126 RID: 294 RVA: 0x000058F5 File Offset: 0x00003AF5
		public override void UpdateSelection(bool isSelected)
		{
			this._viewModel.IsSelected = isSelected;
			if (isSelected)
			{
				this._viewModel.RefreshValues();
			}
		}

		// Token: 0x06000127 RID: 295 RVA: 0x00005911 File Offset: 0x00003B11
		private void OnSelect()
		{
			this._menuClassCollection.OnOptionClassSelected(this);
		}

		// Token: 0x0400006F RID: 111
		private readonly OptionClassViewModel _viewModel;

		// Token: 0x04000070 RID: 112
		private readonly AMenuClassCollection _menuClassCollection;
	}
}
