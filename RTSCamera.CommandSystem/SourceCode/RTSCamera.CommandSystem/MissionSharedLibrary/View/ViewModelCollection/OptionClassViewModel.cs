using System;
using MissionLibrary.View;
using MissionSharedLibrary.View.ViewModelCollection.Basic;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace MissionSharedLibrary.View.ViewModelCollection
{
	// Token: 0x02000021 RID: 33
	public class OptionClassViewModel : ViewModel
	{
		// Token: 0x17000036 RID: 54
		// (get) Token: 0x06000128 RID: 296 RVA: 0x0000591F File Offset: 0x00003B1F
		public string Id { get; }

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x06000129 RID: 297 RVA: 0x00005927 File Offset: 0x00003B27
		public TextViewModel Name { get; }

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x0600012A RID: 298 RVA: 0x0000592F File Offset: 0x00003B2F
		// (set) Token: 0x0600012B RID: 299 RVA: 0x00005937 File Offset: 0x00003B37
		[DataSourceProperty]
		public MBBindingList<OptionColumnViewModel> OptionColumns
		{
			get
			{
				return this._optionColumns;
			}
			set
			{
				if (this._optionColumns == value)
				{
					return;
				}
				this._optionColumns = value;
				base.OnPropertyChanged("OptionColumns");
			}
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x0600012C RID: 300 RVA: 0x00005955 File Offset: 0x00003B55
		// (set) Token: 0x0600012D RID: 301 RVA: 0x0000595D File Offset: 0x00003B5D
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

		// Token: 0x0600012E RID: 302 RVA: 0x00005981 File Offset: 0x00003B81
		public OptionClassViewModel(string id, TextObject name, Action onSelect)
		{
			this.Id = id;
			this.Name = new TextViewModel(name, true);
			this._onSelect = onSelect;
		}

		// Token: 0x0600012F RID: 303 RVA: 0x000059B8 File Offset: 0x00003BB8
		public void AddOptionCategory(int column, IOptionCategory optionCategory)
		{
			column = Math.Min(column, this._maxColumnIndex);
			if (column < 0 || column >= this.OptionColumns.Count)
			{
				column = MBMath.ClampInt(column, 0, this.OptionColumns.Count);
				this.OptionColumns.Insert(column, new OptionColumnViewModel());
			}
			this.OptionColumns[column].AddOptionCategory(optionCategory);
		}

		// Token: 0x06000130 RID: 304 RVA: 0x00005A1C File Offset: 0x00003C1C
		public void ExecuteSelection()
		{
			this._onSelect();
		}

		// Token: 0x06000131 RID: 305 RVA: 0x00005A29 File Offset: 0x00003C29
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Refresh();
		}

		// Token: 0x06000132 RID: 306 RVA: 0x00005A38 File Offset: 0x00003C38
		private void Refresh()
		{
			foreach (OptionColumnViewModel optionColumnViewModel in this._optionColumns)
			{
				optionColumnViewModel.RefreshValues();
			}
		}

		// Token: 0x04000072 RID: 114
		private readonly int _maxColumnIndex = 10;

		// Token: 0x04000073 RID: 115
		private MBBindingList<OptionColumnViewModel> _optionColumns = new MBBindingList<OptionColumnViewModel>();

		// Token: 0x04000074 RID: 116
		private readonly Action _onSelect;

		// Token: 0x04000077 RID: 119
		private bool _isSelected;
	}
}
