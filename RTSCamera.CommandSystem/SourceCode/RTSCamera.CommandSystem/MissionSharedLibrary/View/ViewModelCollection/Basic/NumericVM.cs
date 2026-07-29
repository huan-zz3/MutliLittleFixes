using System;
using TaleWorlds.Library;

namespace MissionSharedLibrary.View.ViewModelCollection.Basic
{
	// Token: 0x0200002B RID: 43
	public class NumericVM : ViewModel
	{
		// Token: 0x06000176 RID: 374 RVA: 0x000062C4 File Offset: 0x000044C4
		public NumericVM(string name, float initialValue, float min, float max, bool isDiscrete, Action<float> updateAction, int roundScale = 100, bool isVisible = true)
		{
			this.Name = name;
			this._initialValue = initialValue;
			this._min = min;
			this._max = max;
			this._optionValue = initialValue;
			this._isDiscrete = isDiscrete;
			this._updateAction = updateAction;
			this._roundScale = roundScale;
			this._isVisible = isVisible;
		}

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x06000177 RID: 375 RVA: 0x0000631B File Offset: 0x0000451B
		public string Name { get; }

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x06000178 RID: 376 RVA: 0x00006323 File Offset: 0x00004523
		// (set) Token: 0x06000179 RID: 377 RVA: 0x0000632B File Offset: 0x0000452B
		[DataSourceProperty]
		public float Min
		{
			get
			{
				return this._min;
			}
			set
			{
				if (Math.Abs(value - this._min) < 0.01f)
				{
					return;
				}
				this._min = value;
				base.OnPropertyChanged("Min");
			}
		}

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x0600017A RID: 378 RVA: 0x00006354 File Offset: 0x00004554
		// (set) Token: 0x0600017B RID: 379 RVA: 0x0000635C File Offset: 0x0000455C
		[DataSourceProperty]
		public float Max
		{
			get
			{
				return this._max;
			}
			set
			{
				if (Math.Abs(value - this._max) < 0.01f)
				{
					return;
				}
				this._max = value;
				base.OnPropertyChanged("Max");
			}
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x0600017C RID: 380 RVA: 0x00006385 File Offset: 0x00004585
		// (set) Token: 0x0600017D RID: 381 RVA: 0x00006390 File Offset: 0x00004590
		[DataSourceProperty]
		public float OptionValue
		{
			get
			{
				return this._optionValue;
			}
			set
			{
				if (MathF.Abs((double)value - (double)this._optionValue) < 0.009999999776482582)
				{
					return;
				}
				this._optionValue = (float)MathF.Round(value * (float)this._roundScale) / (float)this._roundScale;
				base.OnPropertyChanged("OptionValue");
				base.OnPropertyChanged("OptionValueAsString");
				this._updateAction(this.OptionValue);
			}
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x0600017E RID: 382 RVA: 0x000063FC File Offset: 0x000045FC
		// (set) Token: 0x0600017F RID: 383 RVA: 0x00006404 File Offset: 0x00004604
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
				base.OnPropertyChanged("IsDiscrete");
			}
		}

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x06000180 RID: 384 RVA: 0x00006424 File Offset: 0x00004624
		[DataSourceProperty]
		public string OptionValueAsString
		{
			get
			{
				if (this.IsDiscrete)
				{
					return ((int)this._optionValue).ToString();
				}
				return this._optionValue.ToString("F");
			}
		}

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x06000181 RID: 385 RVA: 0x00006459 File Offset: 0x00004659
		// (set) Token: 0x06000182 RID: 386 RVA: 0x00006461 File Offset: 0x00004661
		[DataSourceProperty]
		public bool IsVisible
		{
			get
			{
				return this._isVisible;
			}
			set
			{
				if (value == this._isVisible)
				{
					return;
				}
				this._isVisible = value;
				base.OnPropertyChanged("IsVisible");
			}
		}

		// Token: 0x04000096 RID: 150
		private readonly float _initialValue;

		// Token: 0x04000097 RID: 151
		private float _min;

		// Token: 0x04000098 RID: 152
		private float _max;

		// Token: 0x04000099 RID: 153
		private float _optionValue;

		// Token: 0x0400009A RID: 154
		private bool _isDiscrete;

		// Token: 0x0400009B RID: 155
		private readonly Action<float> _updateAction;

		// Token: 0x0400009C RID: 156
		private readonly int _roundScale;

		// Token: 0x0400009D RID: 157
		private bool _isVisible;
	}
}
