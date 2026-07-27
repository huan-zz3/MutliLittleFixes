using System;
using NavalDLC.ViewModelCollection.Port.PortScreenHandlers;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace NavalDLC.ViewModelCollection.Port
{
	// Token: 0x0200000A RID: 10
	public class PortActionVM : ViewModel
	{
		// Token: 0x0600002D RID: 45 RVA: 0x00005535 File Offset: 0x00003735
		public PortActionVM(Action action)
		{
			this._action = action;
			this.Tooltip = new HintViewModel();
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00005550 File Offset: 0x00003750
		public void RefreshWith(PortActionInfo actionInfo)
		{
			this.IsVisible = actionInfo.IsRelevant;
			this.IsEnabled = actionInfo.IsEnabled;
			TextObject actionName = actionInfo.ActionName;
			this.Name = ((actionName != null) ? actionName.ToString() : null);
			this.Tooltip.HintText = actionInfo.Tooltip;
		}

		// Token: 0x0600002F RID: 47 RVA: 0x0000559E File Offset: 0x0000379E
		public void ExecuteAction()
		{
			Action action = this._action;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000030 RID: 48 RVA: 0x000055B0 File Offset: 0x000037B0
		// (set) Token: 0x06000031 RID: 49 RVA: 0x000055B8 File Offset: 0x000037B8
		[DataSourceProperty]
		public bool IsVisible
		{
			get
			{
				return this._isVisible;
			}
			set
			{
				if (value != this._isVisible)
				{
					this._isVisible = value;
					base.OnPropertyChangedWithValue(value, "IsVisible");
				}
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000032 RID: 50 RVA: 0x000055D6 File Offset: 0x000037D6
		// (set) Token: 0x06000033 RID: 51 RVA: 0x000055DE File Offset: 0x000037DE
		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
				}
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000034 RID: 52 RVA: 0x000055FC File Offset: 0x000037FC
		// (set) Token: 0x06000035 RID: 53 RVA: 0x00005604 File Offset: 0x00003804
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

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000036 RID: 54 RVA: 0x00005627 File Offset: 0x00003827
		// (set) Token: 0x06000037 RID: 55 RVA: 0x0000562F File Offset: 0x0000382F
		[DataSourceProperty]
		public string AdditionalInfo
		{
			get
			{
				return this._additionalInfo;
			}
			set
			{
				if (value != this._additionalInfo)
				{
					this._additionalInfo = value;
					base.OnPropertyChangedWithValue<string>(value, "AdditionalInfo");
				}
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000038 RID: 56 RVA: 0x00005652 File Offset: 0x00003852
		// (set) Token: 0x06000039 RID: 57 RVA: 0x0000565A File Offset: 0x0000385A
		[DataSourceProperty]
		public HintViewModel Tooltip
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
					base.OnPropertyChangedWithValue<HintViewModel>(value, "Tooltip");
				}
			}
		}

		// Token: 0x0400000B RID: 11
		private readonly Action _action;

		// Token: 0x0400000C RID: 12
		private bool _isVisible;

		// Token: 0x0400000D RID: 13
		private bool _isEnabled;

		// Token: 0x0400000E RID: 14
		private string _name;

		// Token: 0x0400000F RID: 15
		private string _additionalInfo;

		// Token: 0x04000010 RID: 16
		private HintViewModel _tooltip;
	}
}
