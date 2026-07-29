using System;
using System.Collections.Generic;
using System.Linq;
using MissionLibrary.View;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace MissionSharedLibrary.View.ViewModelCollection.Options.Selection
{
	// Token: 0x02000029 RID: 41
	public class SelectionOptionViewModel : OptionViewModel, IOption, IViewModelProvider<ViewModel>
	{
		// Token: 0x17000047 RID: 71
		// (get) Token: 0x0600016A RID: 362 RVA: 0x00005F73 File Offset: 0x00004173
		// (set) Token: 0x0600016B RID: 363 RVA: 0x00005F7B File Offset: 0x0000417B
		[DataSourceProperty]
		public SelectorVM<SelectorItemVM> Selector
		{
			get
			{
				return this._selector;
			}
			set
			{
				if (value == this._selector)
				{
					return;
				}
				this._selector = value;
				base.OnPropertyChangedWithValue<SelectorVM<SelectorItemVM>>(value, "Selector");
			}
		}

		// Token: 0x0600016C RID: 364 RVA: 0x00005F9C File Offset: 0x0000419C
		public SelectionOptionViewModel(TextObject name, TextObject description, SelectionOptionData selectionOptionData, bool commitOnlyWhenChange, bool includeOrdinal = false)
			: base(name, description, 3, true)
		{
			this._selectionOptionData = selectionOptionData;
			this._commitOnlyWhenChange = commitOnlyWhenChange;
			this._includeOrdinal = includeOrdinal;
			this.Selector = new SelectorVM<SelectorItemVM>(0, null);
			this.UpdateData(true);
			this.Selector.SelectedIndex = this._selectionOptionData.GetDefaultValue();
		}

		// Token: 0x0600016D RID: 365 RVA: 0x00005FF4 File Offset: 0x000041F4
		public ViewModel GetViewModel()
		{
			return this;
		}

		// Token: 0x0600016E RID: 366 RVA: 0x00005FF7 File Offset: 0x000041F7
		public void Commit()
		{
			this._selectionOptionData.Commit();
		}

		// Token: 0x0600016F RID: 367 RVA: 0x00006004 File Offset: 0x00004204
		public void Cancel()
		{
			this.Selector.SelectedIndex = this._selectionOptionData.GetDefaultValue();
			this.UpdateValue(this._selector);
		}

		// Token: 0x06000170 RID: 368 RVA: 0x00006028 File Offset: 0x00004228
		public override void UpdateData(bool initialUpdate)
		{
			base.UpdateData(initialUpdate);
			IEnumerable<SelectionItem> selectableOptionNames = this._selectionOptionData.GetSelectableOptionNames();
			this.Selector.SetOnChangeAction(null);
			this.Selector.SelectedIndex = -1;
			this._selectionOptionData.SetValue(this._selectionOptionData.GetDefaultValue());
			SelectionItem[] array = (selectableOptionNames as SelectionItem[]) ?? selectableOptionNames.ToArray<SelectionItem>();
			if (array.Any<SelectionItem>())
			{
				if (array.All<SelectionItem>((SelectionItem n) => n.IsLocalizationId))
				{
					List<TextObject> list = new List<TextObject>();
					foreach (ValueTuple<SelectionItem, int> valueTuple in array.Select<SelectionItem, ValueTuple<SelectionItem, int>>((SelectionItem item, int i) => new ValueTuple<SelectionItem, int>(item, i)))
					{
						SelectionItem item3 = valueTuple.Item1;
						int item2 = valueTuple.Item2;
						TextObject textObject = GameTexts.FindText(item3.Data, item3.Variation);
						list.Add(this._includeOrdinal ? new TextObject(string.Format("{0}: ", item2 + 1) + "{Text}", null).SetTextVariable("Text", textObject) : textObject);
					}
					this.Selector.Refresh(list, this._selectionOptionData.GetValue(), new Action<SelectorVM<SelectorItemVM>>(this.UpdateValue));
					return;
				}
			}
			List<string> list2 = new List<string>();
			foreach (SelectionItem selectionItem in array)
			{
				if (selectionItem.IsLocalizationId)
				{
					TextObject textObject2 = GameTexts.FindText(selectionItem.Data, null);
					list2.Add(textObject2.ToString());
				}
				else
				{
					list2.Add(selectionItem.Data);
				}
			}
			this.Selector.Refresh(list2, this._selectionOptionData.GetValue(), new Action<SelectorVM<SelectorItemVM>>(this.UpdateValue));
		}

		// Token: 0x06000171 RID: 369 RVA: 0x00006224 File Offset: 0x00004424
		public override void RefreshValues()
		{
			base.RefreshValues();
			SelectorVM<SelectorItemVM> selector = this.Selector;
			if (selector == null)
			{
				return;
			}
			selector.RefreshValues();
		}

		// Token: 0x06000172 RID: 370 RVA: 0x0000623C File Offset: 0x0000443C
		private void UpdateValue(SelectorVM<SelectorItemVM> selector)
		{
			if (selector.SelectedIndex < 0)
			{
				return;
			}
			this._selectionOptionData.SetValue(selector.SelectedIndex);
			if (!this._commitOnlyWhenChange || this._selectionOptionData.GetValue() != this._selectionOptionData.GetDefaultValue())
			{
				this._selectionOptionData.Commit();
			}
		}

		// Token: 0x04000091 RID: 145
		private readonly SelectionOptionData _selectionOptionData;

		// Token: 0x04000092 RID: 146
		private readonly bool _commitOnlyWhenChange;

		// Token: 0x04000093 RID: 147
		private SelectorVM<SelectorItemVM> _selector;

		// Token: 0x04000094 RID: 148
		private bool _includeOrdinal;
	}
}
