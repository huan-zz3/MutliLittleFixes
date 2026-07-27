using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace NavalDLC.ViewModelCollection.Port
{
	// Token: 0x02000011 RID: 17
	public class ShipStatVM : ViewModel
	{
		// Token: 0x06000149 RID: 329 RVA: 0x00009784 File Offset: 0x00007984
		public ShipStatVM(string statId, TextObject name, string value, string bonusValue = "", bool isBonusBeneficial = true, Func<List<TooltipProperty>> getTooltipProperties = null)
		{
			this._nameTextObj = name;
			this.ValueText = value;
			this.BonusValueText = bonusValue;
			this.IsBonusBeneficial = isBonusBeneficial;
			this.StatId = statId;
			if (getTooltipProperties != null)
			{
				this.Tooltip = new BasicTooltipViewModel(getTooltipProperties);
			}
			else
			{
				this.Tooltip = new BasicTooltipViewModel(() => GameTexts.FindText("str_ship_stat_explanation", this.StatId).ToString());
			}
			this.RefreshValues();
		}

		// Token: 0x0600014A RID: 330 RVA: 0x000097EC File Offset: 0x000079EC
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this._nameTextObj.ToString();
		}

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x0600014B RID: 331 RVA: 0x00009805 File Offset: 0x00007A05
		// (set) Token: 0x0600014C RID: 332 RVA: 0x0000980D File Offset: 0x00007A0D
		[DataSourceProperty]
		public bool IsBonusBeneficial
		{
			get
			{
				return this._isBonusBeneficial;
			}
			set
			{
				if (value != this._isBonusBeneficial)
				{
					this._isBonusBeneficial = value;
					base.OnPropertyChangedWithValue(value, "IsBonusBeneficial");
				}
			}
		}

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x0600014D RID: 333 RVA: 0x0000982B File Offset: 0x00007A2B
		// (set) Token: 0x0600014E RID: 334 RVA: 0x00009833 File Offset: 0x00007A33
		[DataSourceProperty]
		public string StatId
		{
			get
			{
				return this._statId;
			}
			set
			{
				if (value != this._statId)
				{
					this._statId = value;
					base.OnPropertyChangedWithValue<string>(value, "StatId");
				}
			}
		}

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x0600014F RID: 335 RVA: 0x00009856 File Offset: 0x00007A56
		// (set) Token: 0x06000150 RID: 336 RVA: 0x0000985E File Offset: 0x00007A5E
		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x06000151 RID: 337 RVA: 0x00009881 File Offset: 0x00007A81
		// (set) Token: 0x06000152 RID: 338 RVA: 0x00009889 File Offset: 0x00007A89
		[DataSourceProperty]
		public string ValueText
		{
			get
			{
				return this._valueText;
			}
			set
			{
				if (value != this._valueText)
				{
					this._valueText = value;
					base.OnPropertyChangedWithValue<string>(value, "ValueText");
				}
			}
		}

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x06000153 RID: 339 RVA: 0x000098AC File Offset: 0x00007AAC
		// (set) Token: 0x06000154 RID: 340 RVA: 0x000098B4 File Offset: 0x00007AB4
		[DataSourceProperty]
		public string BonusValueText
		{
			get
			{
				return this._bonusValueText;
			}
			set
			{
				if (value != this._bonusValueText)
				{
					this._bonusValueText = value;
					base.OnPropertyChangedWithValue<string>(value, "BonusValueText");
				}
			}
		}

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x06000155 RID: 341 RVA: 0x000098D7 File Offset: 0x00007AD7
		// (set) Token: 0x06000156 RID: 342 RVA: 0x000098DF File Offset: 0x00007ADF
		[DataSourceProperty]
		public BasicTooltipViewModel Tooltip
		{
			get
			{
				return this._tooltip;
			}
			set
			{
				if (value != this._tooltip)
				{
					this._tooltip = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "Tooltip");
				}
			}
		}

		// Token: 0x04000079 RID: 121
		private readonly TextObject _nameTextObj;

		// Token: 0x0400007A RID: 122
		private bool _isBonusBeneficial;

		// Token: 0x0400007B RID: 123
		private string _statId;

		// Token: 0x0400007C RID: 124
		private string _name;

		// Token: 0x0400007D RID: 125
		private string _valueText;

		// Token: 0x0400007E RID: 126
		private string _bonusValueText;

		// Token: 0x0400007F RID: 127
		private BasicTooltipViewModel _tooltip;
	}
}
