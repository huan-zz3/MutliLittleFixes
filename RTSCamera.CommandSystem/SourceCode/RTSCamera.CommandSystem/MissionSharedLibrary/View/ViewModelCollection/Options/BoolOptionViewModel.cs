using System;
using MissionLibrary.View;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace MissionSharedLibrary.View.ViewModelCollection.Options
{
	// Token: 0x02000025 RID: 37
	public class BoolOptionViewModel : OptionViewModel, IOption, IViewModelProvider<ViewModel>
	{
		// Token: 0x17000041 RID: 65
		// (get) Token: 0x0600014E RID: 334 RVA: 0x00005D2B File Offset: 0x00003F2B
		// (set) Token: 0x0600014F RID: 335 RVA: 0x00005D38 File Offset: 0x00003F38
		[DataSourceProperty]
		public bool OptionValueAsBoolean
		{
			get
			{
				return this._getValue();
			}
			set
			{
				if (value == this._getValue())
				{
					return;
				}
				if (this._setValue != null)
				{
					this._setValue(value);
					base.OnPropertyChanged("OptionValueAsBoolean");
				}
			}
		}

		// Token: 0x06000150 RID: 336 RVA: 0x00005D68 File Offset: 0x00003F68
		public BoolOptionViewModel(TextObject name, TextObject description, Func<bool> getValue, Action<bool> setValue)
			: base(name, description, 0, true)
		{
			this._getValue = getValue;
			this._setValue = setValue;
		}

		// Token: 0x06000151 RID: 337 RVA: 0x00005D83 File Offset: 0x00003F83
		public override void RefreshValues()
		{
			base.RefreshValues();
			base.OnPropertyChanged("OptionValueAsBoolean");
		}

		// Token: 0x06000152 RID: 338 RVA: 0x00005D96 File Offset: 0x00003F96
		public ViewModel GetViewModel()
		{
			return this;
		}

		// Token: 0x06000153 RID: 339 RVA: 0x00005D99 File Offset: 0x00003F99
		public void Commit()
		{
		}

		// Token: 0x06000154 RID: 340 RVA: 0x00005D9B File Offset: 0x00003F9B
		public void Cancel()
		{
		}

		// Token: 0x04000081 RID: 129
		private readonly Func<bool> _getValue;

		// Token: 0x04000082 RID: 130
		private readonly Action<bool> _setValue;
	}
}
