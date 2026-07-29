using System;
using System.Collections.Generic;

namespace MissionSharedLibrary.View.ViewModelCollection.Options.Selection
{
	// Token: 0x02000028 RID: 40
	public class SelectionOptionData
	{
		// Token: 0x06000163 RID: 355 RVA: 0x00005EF7 File Offset: 0x000040F7
		public SelectionOptionData(Action<int> setValue, Func<int> getValue, Func<int> limit, Func<IEnumerable<SelectionItem>> data)
		{
			this._setValue = setValue;
			this._getValue = getValue;
			this._value = getValue();
			this._limit = limit;
			this._data = data;
		}

		// Token: 0x06000164 RID: 356 RVA: 0x00005F28 File Offset: 0x00004128
		public int GetDefaultValue()
		{
			return this._getValue();
		}

		// Token: 0x06000165 RID: 357 RVA: 0x00005F35 File Offset: 0x00004135
		public void Commit()
		{
			this._setValue(this._value);
		}

		// Token: 0x06000166 RID: 358 RVA: 0x00005F48 File Offset: 0x00004148
		public int GetValue()
		{
			return this._value;
		}

		// Token: 0x06000167 RID: 359 RVA: 0x00005F50 File Offset: 0x00004150
		public void SetValue(int value)
		{
			this._value = value;
		}

		// Token: 0x06000168 RID: 360 RVA: 0x00005F59 File Offset: 0x00004159
		public int GetSelectableOptionsLimit()
		{
			return this._limit();
		}

		// Token: 0x06000169 RID: 361 RVA: 0x00005F66 File Offset: 0x00004166
		public IEnumerable<SelectionItem> GetSelectableOptionNames()
		{
			return this._data();
		}

		// Token: 0x0400008C RID: 140
		private readonly Action<int> _setValue;

		// Token: 0x0400008D RID: 141
		private readonly Func<int> _getValue;

		// Token: 0x0400008E RID: 142
		private int _value;

		// Token: 0x0400008F RID: 143
		private readonly Func<int> _limit;

		// Token: 0x04000090 RID: 144
		private readonly Func<IEnumerable<SelectionItem>> _data;
	}
}
