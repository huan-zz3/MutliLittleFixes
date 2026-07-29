using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace FormationFilter.View.ViewModels
{
	// Token: 0x02000010 RID: 16
	[NullableContext(1)]
	[Nullable(0)]
	public class FormationFilterResultVM : ViewModel
	{
		// Token: 0x06000090 RID: 144 RVA: 0x00004C38 File Offset: 0x00002E38
		public void SetResult(List<Agent> agents)
		{
			this._count = agents.Count;
			if (this._count == 0)
			{
				this.SuccessStringVisible = true;
				this.FailStringVisible = false;
				return;
			}
			this.firstName = agents[0].NameTextObject;
			this.FailString = GameTexts.FindText("str_formation_filter_result_fail", null).SetTextVariable("number", this._count).SetTextVariable("name", this.firstName.ToString())
				.ToString();
			this.SuccessStringVisible = false;
			this.FailStringVisible = true;
		}

		// Token: 0x06000091 RID: 145 RVA: 0x00004CC4 File Offset: 0x00002EC4
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.SuccessString = GameTexts.FindText("str_formation_filter_result_success", null).ToString();
			this.FailString = GameTexts.FindText("str_formation_filter_result_fail", null).SetTextVariable("number", this._count).ToString();
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x06000092 RID: 146 RVA: 0x00004D13 File Offset: 0x00002F13
		// (set) Token: 0x06000093 RID: 147 RVA: 0x00004D1B File Offset: 0x00002F1B
		[DataSourceProperty]
		public string SuccessString
		{
			get
			{
				return this._successString;
			}
			set
			{
				if (this._successString == value)
				{
					return;
				}
				this._successString = value;
				base.OnPropertyChangedWithValue<string>(this._successString, "SuccessString");
			}
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x06000094 RID: 148 RVA: 0x00004D44 File Offset: 0x00002F44
		// (set) Token: 0x06000095 RID: 149 RVA: 0x00004D4C File Offset: 0x00002F4C
		[DataSourceProperty]
		public bool SuccessStringVisible
		{
			get
			{
				return this._successStringVisible;
			}
			set
			{
				if (this._successStringVisible == value)
				{
					return;
				}
				this._successStringVisible = value;
				base.OnPropertyChangedWithValue(this._successStringVisible, "SuccessStringVisible");
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x06000096 RID: 150 RVA: 0x00004D70 File Offset: 0x00002F70
		// (set) Token: 0x06000097 RID: 151 RVA: 0x00004D78 File Offset: 0x00002F78
		[DataSourceProperty]
		public string FailString
		{
			get
			{
				return this._failString;
			}
			set
			{
				if (this._failString == value)
				{
					return;
				}
				this._failString = value;
				base.OnPropertyChangedWithValue<string>(this._failString, "FailString");
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x06000098 RID: 152 RVA: 0x00004DA1 File Offset: 0x00002FA1
		// (set) Token: 0x06000099 RID: 153 RVA: 0x00004DA9 File Offset: 0x00002FA9
		[DataSourceProperty]
		public bool FailStringVisible
		{
			get
			{
				return this._failStringVisible;
			}
			set
			{
				if (this._failStringVisible == value)
				{
					return;
				}
				this._failStringVisible = value;
				base.OnPropertyChangedWithValue(this._failStringVisible, "FailStringVisible");
			}
		}

		// Token: 0x0400004C RID: 76
		private string _successString = GameTexts.FindText("str_formation_filter_result_success", null).ToString();

		// Token: 0x0400004D RID: 77
		private string _failString = GameTexts.FindText("str_formation_filter_result_fail", null).ToString();

		// Token: 0x0400004E RID: 78
		private bool _successStringVisible;

		// Token: 0x0400004F RID: 79
		private bool _failStringVisible;

		// Token: 0x04000050 RID: 80
		private int _count;

		// Token: 0x04000051 RID: 81
		private TextObject firstName = TextObject.GetEmpty();
	}
}
