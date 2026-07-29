using System;
using System.Runtime.CompilerServices;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace FormationFilter.View.Widgets
{
	// Token: 0x02000008 RID: 8
	[NullableContext(1)]
	[Nullable(0)]
	internal class FormationFilterFilterVisualBrushWidget : BrushWidget
	{
		// Token: 0x06000012 RID: 18 RVA: 0x00002366 File Offset: 0x00000566
		public FormationFilterFilterVisualBrushWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002370 File Offset: 0x00000570
		private void SetBaseBrush()
		{
			switch (this.FormationFilter)
			{
			case 0:
				base.Brush = this.UnsetBrush;
				break;
			case 1:
				base.Brush = this.OneHandedBrush;
				break;
			case 2:
				base.Brush = this.TwoHandedBrush;
				break;
			case 3:
				base.Brush = this.SpearBrush;
				break;
			case 4:
				base.Brush = this.ThrownBrush;
				break;
			case 5:
				base.Brush = this.ShieldBrush;
				break;
			case 6:
				base.Brush = this.HeavyBrush;
				break;
			case 7:
				base.Brush = this.LowTierBrush;
				break;
			case 8:
				base.Brush = this.HighTierBrush;
				break;
			case 9:
				base.Brush = this.BowBrush;
				break;
			case 10:
				base.Brush = this.CrossbowBrush;
				break;
			case 11:
				base.Brush = this.SlingBrush;
				break;
			default:
				base.Brush = this.UnsetBrush;
				break;
			}
			this._hasBaseBrushSet = true;
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000014 RID: 20 RVA: 0x00002483 File Offset: 0x00000683
		// (set) Token: 0x06000015 RID: 21 RVA: 0x0000248B File Offset: 0x0000068B
		[Editor(false)]
		public int FormationFilter
		{
			get
			{
				return this._formationFilter;
			}
			set
			{
				if (value == this._formationFilter && this._hasBaseBrushSet)
				{
					return;
				}
				this._formationFilter = value;
				base.OnPropertyChanged(value, "FormationFilter");
				this.SetBaseBrush();
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000016 RID: 22 RVA: 0x000024B8 File Offset: 0x000006B8
		// (set) Token: 0x06000017 RID: 23 RVA: 0x000024C0 File Offset: 0x000006C0
		[Editor(false)]
		public Brush UnsetBrush
		{
			get
			{
				return this._unsetBrush;
			}
			set
			{
				if (value == this._unsetBrush)
				{
					return;
				}
				this._unsetBrush = value;
				base.OnPropertyChanged<Brush>(value, "UnsetBrush");
				this.SetBaseBrush();
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000018 RID: 24 RVA: 0x000024E5 File Offset: 0x000006E5
		// (set) Token: 0x06000019 RID: 25 RVA: 0x000024ED File Offset: 0x000006ED
		[Editor(false)]
		public Brush SpearBrush
		{
			get
			{
				return this._spearBrush;
			}
			set
			{
				if (value == this._spearBrush)
				{
					return;
				}
				this._spearBrush = value;
				base.OnPropertyChanged<Brush>(value, "SpearBrush");
				this.SetBaseBrush();
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600001A RID: 26 RVA: 0x00002512 File Offset: 0x00000712
		// (set) Token: 0x0600001B RID: 27 RVA: 0x0000251A File Offset: 0x0000071A
		[Editor(false)]
		public Brush OneHandedBrush
		{
			get
			{
				return this._oneHandedBrush;
			}
			set
			{
				if (value == this._oneHandedBrush)
				{
					return;
				}
				this._oneHandedBrush = value;
				base.OnPropertyChanged<Brush>(value, "OneHandedBrush");
				this.SetBaseBrush();
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600001C RID: 28 RVA: 0x0000253F File Offset: 0x0000073F
		// (set) Token: 0x0600001D RID: 29 RVA: 0x00002547 File Offset: 0x00000747
		[Editor(false)]
		public Brush TwoHandedBrush
		{
			get
			{
				return this._twoHandedBrush;
			}
			set
			{
				if (value == this._twoHandedBrush)
				{
					return;
				}
				this._twoHandedBrush = value;
				base.OnPropertyChanged<Brush>(value, "TwoHandedBrush");
				this.SetBaseBrush();
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600001E RID: 30 RVA: 0x0000256C File Offset: 0x0000076C
		// (set) Token: 0x0600001F RID: 31 RVA: 0x00002574 File Offset: 0x00000774
		[Editor(false)]
		public Brush ShieldBrush
		{
			get
			{
				return this._shieldBrush;
			}
			set
			{
				if (value == this._shieldBrush)
				{
					return;
				}
				this._shieldBrush = value;
				base.OnPropertyChanged<Brush>(value, "ShieldBrush");
				this.SetBaseBrush();
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000020 RID: 32 RVA: 0x00002599 File Offset: 0x00000799
		// (set) Token: 0x06000021 RID: 33 RVA: 0x000025A1 File Offset: 0x000007A1
		[Editor(false)]
		public Brush ThrownBrush
		{
			get
			{
				return this._thrownBrush;
			}
			set
			{
				if (value == this._thrownBrush)
				{
					return;
				}
				this._thrownBrush = value;
				base.OnPropertyChanged<Brush>(value, "ThrownBrush");
				this.SetBaseBrush();
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000022 RID: 34 RVA: 0x000025C6 File Offset: 0x000007C6
		// (set) Token: 0x06000023 RID: 35 RVA: 0x000025CE File Offset: 0x000007CE
		[Editor(false)]
		public Brush HeavyBrush
		{
			get
			{
				return this._heavyBrush;
			}
			set
			{
				if (value == this._heavyBrush)
				{
					return;
				}
				this._heavyBrush = value;
				base.OnPropertyChanged<Brush>(value, "HeavyBrush");
				this.SetBaseBrush();
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000024 RID: 36 RVA: 0x000025F3 File Offset: 0x000007F3
		// (set) Token: 0x06000025 RID: 37 RVA: 0x000025FB File Offset: 0x000007FB
		[Editor(false)]
		public Brush HighTierBrush
		{
			get
			{
				return this._highTierBrush;
			}
			set
			{
				if (value == this._highTierBrush)
				{
					return;
				}
				this._highTierBrush = value;
				base.OnPropertyChanged<Brush>(value, "HighTierBrush");
				this.SetBaseBrush();
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000026 RID: 38 RVA: 0x00002620 File Offset: 0x00000820
		// (set) Token: 0x06000027 RID: 39 RVA: 0x00002628 File Offset: 0x00000828
		[Editor(false)]
		public Brush LowTierBrush
		{
			get
			{
				return this._lowTierBrush;
			}
			set
			{
				if (value == this._lowTierBrush)
				{
					return;
				}
				this._lowTierBrush = value;
				base.OnPropertyChanged<Brush>(value, "LowTierBrush");
				this.SetBaseBrush();
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000028 RID: 40 RVA: 0x0000264D File Offset: 0x0000084D
		// (set) Token: 0x06000029 RID: 41 RVA: 0x00002655 File Offset: 0x00000855
		[Editor(false)]
		public Brush BowBrush
		{
			get
			{
				return this._bowBrush;
			}
			set
			{
				if (value == this._bowBrush)
				{
					return;
				}
				this._bowBrush = value;
				base.OnPropertyChanged<Brush>(value, "BowBrush");
				this.SetBaseBrush();
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600002A RID: 42 RVA: 0x0000267A File Offset: 0x0000087A
		// (set) Token: 0x0600002B RID: 43 RVA: 0x00002682 File Offset: 0x00000882
		[Editor(false)]
		public Brush CrossbowBrush
		{
			get
			{
				return this._crossbowBrush;
			}
			set
			{
				if (value == this._crossbowBrush)
				{
					return;
				}
				this._crossbowBrush = value;
				base.OnPropertyChanged<Brush>(value, "CrossbowBrush");
				this.SetBaseBrush();
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600002C RID: 44 RVA: 0x000026A7 File Offset: 0x000008A7
		// (set) Token: 0x0600002D RID: 45 RVA: 0x000026AF File Offset: 0x000008AF
		[Editor(false)]
		public Brush SlingBrush
		{
			get
			{
				return this._slingBrush;
			}
			set
			{
				if (value == this._slingBrush)
				{
					return;
				}
				this._slingBrush = value;
				base.OnPropertyChanged<Brush>(value, "SlingBrush");
				this.SetBaseBrush();
			}
		}

		// Token: 0x0400000B RID: 11
		private bool _hasBaseBrushSet;

		// Token: 0x0400000C RID: 12
		private int _formationFilter;

		// Token: 0x0400000D RID: 13
		private Brush _unsetBrush;

		// Token: 0x0400000E RID: 14
		private Brush _oneHandedBrush;

		// Token: 0x0400000F RID: 15
		private Brush _twoHandedBrush;

		// Token: 0x04000010 RID: 16
		private Brush _spearBrush;

		// Token: 0x04000011 RID: 17
		private Brush _shieldBrush;

		// Token: 0x04000012 RID: 18
		private Brush _thrownBrush;

		// Token: 0x04000013 RID: 19
		private Brush _heavyBrush;

		// Token: 0x04000014 RID: 20
		private Brush _highTierBrush;

		// Token: 0x04000015 RID: 21
		private Brush _lowTierBrush;

		// Token: 0x04000016 RID: 22
		private Brush _bowBrush;

		// Token: 0x04000017 RID: 23
		private Brush _crossbowBrush;

		// Token: 0x04000018 RID: 24
		private Brush _slingBrush;
	}
}
