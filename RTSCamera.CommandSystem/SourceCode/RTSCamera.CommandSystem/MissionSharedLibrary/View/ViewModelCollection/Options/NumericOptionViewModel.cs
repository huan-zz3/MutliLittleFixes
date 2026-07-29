using System;
using MissionLibrary.View;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace MissionSharedLibrary.View.ViewModelCollection.Options
{
	// Token: 0x02000024 RID: 36
	public class NumericOptionViewModel : OptionViewModel, IOption, IViewModelProvider<ViewModel>
	{
		// Token: 0x1700003B RID: 59
		// (get) Token: 0x0600013E RID: 318 RVA: 0x00005BDE File Offset: 0x00003DDE
		// (set) Token: 0x0600013F RID: 319 RVA: 0x00005BE6 File Offset: 0x00003DE6
		[DataSourceProperty]
		public float Min
		{
			get
			{
				return this._min;
			}
			set
			{
				this._min = value;
				base.OnPropertyChangedWithValue(value, "Min");
			}
		}

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x06000140 RID: 320 RVA: 0x00005BFB File Offset: 0x00003DFB
		// (set) Token: 0x06000141 RID: 321 RVA: 0x00005C03 File Offset: 0x00003E03
		[DataSourceProperty]
		public float Max
		{
			get
			{
				return this._max;
			}
			set
			{
				this._max = value;
				base.OnPropertyChangedWithValue(value, "Max");
			}
		}

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x06000142 RID: 322 RVA: 0x00005C18 File Offset: 0x00003E18
		// (set) Token: 0x06000143 RID: 323 RVA: 0x00005C25 File Offset: 0x00003E25
		[DataSourceProperty]
		public float OptionValue
		{
			get
			{
				return this._getValue();
			}
			set
			{
				this._setValue(value);
				base.OnPropertyChangedWithValue(value, "OptionValue");
				base.OnPropertyChanged("OptionValueAsString");
			}
		}

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x06000144 RID: 324 RVA: 0x00005C4A File Offset: 0x00003E4A
		// (set) Token: 0x06000145 RID: 325 RVA: 0x00005C52 File Offset: 0x00003E52
		[DataSourceProperty]
		public bool IsDiscrete
		{
			get
			{
				return this._isDiscrete;
			}
			set
			{
				if (value == this._isDiscrete)
				{
					return;
				}
				this._isDiscrete = value;
				base.OnPropertyChangedWithValue(value, "IsDiscrete");
			}
		}

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x06000146 RID: 326 RVA: 0x00005C71 File Offset: 0x00003E71
		// (set) Token: 0x06000147 RID: 327 RVA: 0x00005C79 File Offset: 0x00003E79
		[DataSourceProperty]
		public bool UpdateContinuously
		{
			get
			{
				return this._updateContinuously;
			}
			set
			{
				if (value == this._updateContinuously)
				{
					return;
				}
				this._updateContinuously = value;
				base.OnPropertyChangedWithValue(value, "UpdateContinuously");
			}
		}

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x06000148 RID: 328 RVA: 0x00005C98 File Offset: 0x00003E98
		[DataSourceProperty]
		public string OptionValueAsString
		{
			get
			{
				if (this.IsDiscrete)
				{
					return ((int)this.OptionValue).ToString();
				}
				return this.OptionValue.ToString("F");
			}
		}

		// Token: 0x06000149 RID: 329 RVA: 0x00005CD0 File Offset: 0x00003ED0
		public NumericOptionViewModel(TextObject name, TextObject description, Func<float> getValue, Action<float> setValue, float min, float max, bool isDiscrete, bool updateContinuously)
			: base(name, description, 1, true)
		{
			this._getValue = getValue;
			this._setValue = setValue;
			this.Min = min;
			this.Max = max;
			this.IsDiscrete = isDiscrete;
			this.UpdateContinuously = updateContinuously;
		}

		// Token: 0x0600014A RID: 330 RVA: 0x00005D0B File Offset: 0x00003F0B
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.OptionValue = this._getValue();
		}

		// Token: 0x0600014B RID: 331 RVA: 0x00005D24 File Offset: 0x00003F24
		public ViewModel GetViewModel()
		{
			return this;
		}

		// Token: 0x0600014C RID: 332 RVA: 0x00005D27 File Offset: 0x00003F27
		public void Commit()
		{
		}

		// Token: 0x0600014D RID: 333 RVA: 0x00005D29 File Offset: 0x00003F29
		public void Cancel()
		{
		}

		// Token: 0x0400007B RID: 123
		private readonly Func<float> _getValue;

		// Token: 0x0400007C RID: 124
		private readonly Action<float> _setValue;

		// Token: 0x0400007D RID: 125
		private float _min;

		// Token: 0x0400007E RID: 126
		private float _max;

		// Token: 0x0400007F RID: 127
		private bool _isDiscrete;

		// Token: 0x04000080 RID: 128
		private bool _updateContinuously;
	}
}
