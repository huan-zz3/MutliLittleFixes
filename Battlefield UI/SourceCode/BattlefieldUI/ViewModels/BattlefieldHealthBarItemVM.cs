using System;
using TaleWorlds.Library;

namespace BattlefieldUI.ViewModels
{
	// Token: 0x02000007 RID: 7
	public sealed class BattlefieldHealthBarItemVM : ViewModel
	{
		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000032 RID: 50 RVA: 0x00003392 File Offset: 0x00001592
		// (set) Token: 0x06000033 RID: 51 RVA: 0x0000339A File Offset: 0x0000159A
		[DataSourceProperty]
		public float ScreenPositionX
		{
			get
			{
				return this._screenPositionX;
			}
			set
			{
				if (Math.Abs(value - this._screenPositionX) > 0.0001f)
				{
					this._screenPositionX = value;
					base.OnPropertyChangedWithValue(value, "ScreenPositionX");
				}
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000034 RID: 52 RVA: 0x000033C3 File Offset: 0x000015C3
		// (set) Token: 0x06000035 RID: 53 RVA: 0x000033CB File Offset: 0x000015CB
		[DataSourceProperty]
		public float ScreenPositionY
		{
			get
			{
				return this._screenPositionY;
			}
			set
			{
				if (Math.Abs(value - this._screenPositionY) > 0.0001f)
				{
					this._screenPositionY = value;
					base.OnPropertyChangedWithValue(value, "ScreenPositionY");
				}
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000036 RID: 54 RVA: 0x000033F4 File Offset: 0x000015F4
		// (set) Token: 0x06000037 RID: 55 RVA: 0x000033FC File Offset: 0x000015FC
		[DataSourceProperty]
		public float HealthRatio
		{
			get
			{
				return this._healthRatio;
			}
			set
			{
				if (Math.Abs(value - this._healthRatio) > 0.0001f)
				{
					this._healthRatio = value;
					base.OnPropertyChangedWithValue(value, "HealthRatio");
				}
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000038 RID: 56 RVA: 0x00003425 File Offset: 0x00001625
		// (set) Token: 0x06000039 RID: 57 RVA: 0x0000342D File Offset: 0x0000162D
		[DataSourceProperty]
		public float Alpha
		{
			get
			{
				return this._alpha;
			}
			set
			{
				if (Math.Abs(value - this._alpha) > 0.0001f)
				{
					this._alpha = value;
					base.OnPropertyChangedWithValue(value, "Alpha");
				}
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600003A RID: 58 RVA: 0x00003456 File Offset: 0x00001656
		// (set) Token: 0x0600003B RID: 59 RVA: 0x0000345E File Offset: 0x0000165E
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

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600003C RID: 60 RVA: 0x0000347C File Offset: 0x0000167C
		// (set) Token: 0x0600003D RID: 61 RVA: 0x00003484 File Offset: 0x00001684
		[DataSourceProperty]
		public bool ShowSquareBar
		{
			get
			{
				return this._showSquareBar;
			}
			set
			{
				if (value != this._showSquareBar)
				{
					this._showSquareBar = value;
					base.OnPropertyChangedWithValue(value, "ShowSquareBar");
				}
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600003E RID: 62 RVA: 0x000034A2 File Offset: 0x000016A2
		// (set) Token: 0x0600003F RID: 63 RVA: 0x000034AA File Offset: 0x000016AA
		[DataSourceProperty]
		public bool ShowSmallRoundedBar
		{
			get
			{
				return this._showSmallRoundedBar;
			}
			set
			{
				if (value != this._showSmallRoundedBar)
				{
					this._showSmallRoundedBar = value;
					base.OnPropertyChangedWithValue(value, "ShowSmallRoundedBar");
				}
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000040 RID: 64 RVA: 0x000034C8 File Offset: 0x000016C8
		// (set) Token: 0x06000041 RID: 65 RVA: 0x000034D0 File Offset: 0x000016D0
		[DataSourceProperty]
		public bool ShowLargeRoundedBar
		{
			get
			{
				return this._showLargeRoundedBar;
			}
			set
			{
				if (value != this._showLargeRoundedBar)
				{
					this._showLargeRoundedBar = value;
					base.OnPropertyChangedWithValue(value, "ShowLargeRoundedBar");
				}
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000042 RID: 66 RVA: 0x000034EE File Offset: 0x000016EE
		// (set) Token: 0x06000043 RID: 67 RVA: 0x000034F6 File Offset: 0x000016F6
		[DataSourceProperty]
		public bool ShowName
		{
			get
			{
				return this._showName;
			}
			set
			{
				if (value != this._showName)
				{
					this._showName = value;
					base.OnPropertyChangedWithValue(value, "ShowName");
				}
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000044 RID: 68 RVA: 0x00003514 File Offset: 0x00001714
		// (set) Token: 0x06000045 RID: 69 RVA: 0x0000351C File Offset: 0x0000171C
		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				value = value ?? string.Empty;
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000046 RID: 70 RVA: 0x0000354B File Offset: 0x0000174B
		// (set) Token: 0x06000047 RID: 71 RVA: 0x00003553 File Offset: 0x00001753
		[DataSourceProperty]
		public string BarColor
		{
			get
			{
				return this._barColor;
			}
			set
			{
				if (value != this._barColor)
				{
					this._barColor = value;
					base.OnPropertyChangedWithValue<string>(value, "BarColor");
				}
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000048 RID: 72 RVA: 0x00003576 File Offset: 0x00001776
		// (set) Token: 0x06000049 RID: 73 RVA: 0x0000357E File Offset: 0x0000177E
		[DataSourceProperty]
		public string BackgroundColor
		{
			get
			{
				return this._backgroundColor;
			}
			set
			{
				if (value != this._backgroundColor)
				{
					this._backgroundColor = value;
					base.OnPropertyChangedWithValue<string>(value, "BackgroundColor");
				}
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600004A RID: 74 RVA: 0x000035A1 File Offset: 0x000017A1
		// (set) Token: 0x0600004B RID: 75 RVA: 0x000035A9 File Offset: 0x000017A9
		[DataSourceProperty]
		public float BarWidth
		{
			get
			{
				return this._barWidth;
			}
			set
			{
				if (Math.Abs(value - this._barWidth) > 0.0001f)
				{
					this._barWidth = value;
					base.OnPropertyChangedWithValue(value, "BarWidth");
				}
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600004C RID: 76 RVA: 0x000035D2 File Offset: 0x000017D2
		// (set) Token: 0x0600004D RID: 77 RVA: 0x000035DA File Offset: 0x000017DA
		[DataSourceProperty]
		public float BarHeight
		{
			get
			{
				return this._barHeight;
			}
			set
			{
				if (Math.Abs(value - this._barHeight) > 0.0001f)
				{
					this._barHeight = value;
					base.OnPropertyChangedWithValue(value, "BarHeight");
				}
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600004E RID: 78 RVA: 0x00003603 File Offset: 0x00001803
		// (set) Token: 0x0600004F RID: 79 RVA: 0x0000360B File Offset: 0x0000180B
		[DataSourceProperty]
		public float BarPositionX
		{
			get
			{
				return this._barPositionX;
			}
			set
			{
				if (Math.Abs(value - this._barPositionX) > 0.0001f)
				{
					this._barPositionX = value;
					base.OnPropertyChangedWithValue(value, "BarPositionX");
				}
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000050 RID: 80 RVA: 0x00003634 File Offset: 0x00001834
		// (set) Token: 0x06000051 RID: 81 RVA: 0x0000363C File Offset: 0x0000183C
		[DataSourceProperty]
		public float BarPositionY
		{
			get
			{
				return this._barPositionY;
			}
			set
			{
				if (Math.Abs(value - this._barPositionY) > 0.0001f)
				{
					this._barPositionY = value;
					base.OnPropertyChangedWithValue(value, "BarPositionY");
				}
			}
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00003665 File Offset: 0x00001865
		public void Hide()
		{
			this.IsVisible = false;
			this.Alpha = 0f;
			this.ScreenPositionX = -10000f;
			this.ScreenPositionY = -10000f;
		}

		// Token: 0x04000017 RID: 23
		private float _screenPositionX = -10000f;

		// Token: 0x04000018 RID: 24
		private float _screenPositionY = -10000f;

		// Token: 0x04000019 RID: 25
		private float _healthRatio;

		// Token: 0x0400001A RID: 26
		private float _alpha;

		// Token: 0x0400001B RID: 27
		private bool _isVisible;

		// Token: 0x0400001C RID: 28
		private bool _showSquareBar;

		// Token: 0x0400001D RID: 29
		private bool _showSmallRoundedBar;

		// Token: 0x0400001E RID: 30
		private bool _showLargeRoundedBar;

		// Token: 0x0400001F RID: 31
		private bool _showName;

		// Token: 0x04000020 RID: 32
		private string _name = string.Empty;

		// Token: 0x04000021 RID: 33
		private string _barColor = "#FFFFFFFF";

		// Token: 0x04000022 RID: 34
		private string _backgroundColor = "#181818CC";

		// Token: 0x04000023 RID: 35
		private float _barWidth = 45f;

		// Token: 0x04000024 RID: 36
		private float _barHeight = 8f;

		// Token: 0x04000025 RID: 37
		private float _barPositionX;

		// Token: 0x04000026 RID: 38
		private float _barPositionY;
	}
}
