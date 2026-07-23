using System;
using TaleWorlds.Library;

namespace BattlefieldUI.ViewModels
{
	// Token: 0x02000006 RID: 6
	public sealed class BattlefieldDamageNumberItemVM : ViewModel
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000022 RID: 34 RVA: 0x000031EB File Offset: 0x000013EB
		// (set) Token: 0x06000023 RID: 35 RVA: 0x000031F3 File Offset: 0x000013F3
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

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000024 RID: 36 RVA: 0x0000321C File Offset: 0x0000141C
		// (set) Token: 0x06000025 RID: 37 RVA: 0x00003224 File Offset: 0x00001424
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

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000026 RID: 38 RVA: 0x0000324D File Offset: 0x0000144D
		// (set) Token: 0x06000027 RID: 39 RVA: 0x00003255 File Offset: 0x00001455
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

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000028 RID: 40 RVA: 0x0000327E File Offset: 0x0000147E
		// (set) Token: 0x06000029 RID: 41 RVA: 0x00003286 File Offset: 0x00001486
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

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600002A RID: 42 RVA: 0x000032A4 File Offset: 0x000014A4
		// (set) Token: 0x0600002B RID: 43 RVA: 0x000032AC File Offset: 0x000014AC
		[DataSourceProperty]
		public string Text
		{
			get
			{
				return this._text;
			}
			set
			{
				value = value ?? string.Empty;
				if (value != this._text)
				{
					this._text = value;
					base.OnPropertyChangedWithValue<string>(value, "Text");
				}
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600002C RID: 44 RVA: 0x000032DB File Offset: 0x000014DB
		// (set) Token: 0x0600002D RID: 45 RVA: 0x000032E3 File Offset: 0x000014E3
		[DataSourceProperty]
		public string Color
		{
			get
			{
				return this._color;
			}
			set
			{
				if (value != this._color)
				{
					this._color = value;
					base.OnPropertyChangedWithValue<string>(value, "Color");
				}
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600002E RID: 46 RVA: 0x00003306 File Offset: 0x00001506
		// (set) Token: 0x0600002F RID: 47 RVA: 0x0000330E File Offset: 0x0000150E
		[DataSourceProperty]
		public int FontSize
		{
			get
			{
				return this._fontSize;
			}
			set
			{
				if (value != this._fontSize)
				{
					this._fontSize = value;
					base.OnPropertyChangedWithValue(value, "FontSize");
				}
			}
		}

		// Token: 0x06000030 RID: 48 RVA: 0x0000332C File Offset: 0x0000152C
		public void Hide()
		{
			this.IsVisible = false;
			this.Alpha = 0f;
			this.ScreenPositionX = -10000f;
			this.ScreenPositionY = -10000f;
		}

		// Token: 0x04000010 RID: 16
		private float _screenPositionX = -10000f;

		// Token: 0x04000011 RID: 17
		private float _screenPositionY = -10000f;

		// Token: 0x04000012 RID: 18
		private float _alpha;

		// Token: 0x04000013 RID: 19
		private bool _isVisible;

		// Token: 0x04000014 RID: 20
		private string _text = string.Empty;

		// Token: 0x04000015 RID: 21
		private string _color = "#FFFFFFFF";

		// Token: 0x04000016 RID: 22
		private int _fontSize = 22;
	}
}
