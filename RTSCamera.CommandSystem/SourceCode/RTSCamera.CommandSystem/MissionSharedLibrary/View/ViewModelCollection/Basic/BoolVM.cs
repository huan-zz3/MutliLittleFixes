using System;
using TaleWorlds.Library;

namespace MissionSharedLibrary.View.ViewModelCollection.Basic
{
	// Token: 0x0200002A RID: 42
	public class BoolVM : ViewModel
	{
		// Token: 0x17000048 RID: 72
		// (get) Token: 0x06000173 RID: 371 RVA: 0x0000628F File Offset: 0x0000448F
		// (set) Token: 0x06000174 RID: 372 RVA: 0x00006297 File Offset: 0x00004497
		[DataSourceProperty]
		public bool BoolValue
		{
			get
			{
				return this._boolValue;
			}
			set
			{
				if (value == this._boolValue)
				{
					return;
				}
				this._boolValue = value;
				base.OnPropertyChangedWithValue<object>(value, "BoolValue");
			}
		}

		// Token: 0x04000095 RID: 149
		private bool _boolValue;
	}
}
