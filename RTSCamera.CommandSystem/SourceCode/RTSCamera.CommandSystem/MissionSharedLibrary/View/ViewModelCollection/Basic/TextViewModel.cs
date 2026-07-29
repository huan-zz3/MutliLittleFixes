using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace MissionSharedLibrary.View.ViewModelCollection.Basic
{
	// Token: 0x0200002C RID: 44
	public class TextViewModel : ViewModel
	{
		// Token: 0x17000050 RID: 80
		// (get) Token: 0x06000183 RID: 387 RVA: 0x0000647F File Offset: 0x0000467F
		// (set) Token: 0x06000184 RID: 388 RVA: 0x00006487 File Offset: 0x00004687
		public TextObject TextObject
		{
			get
			{
				return this._textObject;
			}
			set
			{
				this._textObject = value;
				this.Text = this._textObject.ToString();
			}
		}

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x06000185 RID: 389 RVA: 0x000064A1 File Offset: 0x000046A1
		// (set) Token: 0x06000186 RID: 390 RVA: 0x000064A9 File Offset: 0x000046A9
		[DataSourceProperty]
		public string Text
		{
			get
			{
				return this._text;
			}
			set
			{
				if (this._text == value)
				{
					return;
				}
				this._text = value;
				base.OnPropertyChanged("Text");
			}
		}

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x06000187 RID: 391 RVA: 0x000064CC File Offset: 0x000046CC
		// (set) Token: 0x06000188 RID: 392 RVA: 0x000064D4 File Offset: 0x000046D4
		[DataSourceProperty]
		public bool IsVisible
		{
			get
			{
				return this._isVisible;
			}
			set
			{
				if (this._isVisible == value)
				{
					return;
				}
				this._isVisible = value;
				base.OnPropertyChanged("IsVisible");
			}
		}

		// Token: 0x06000189 RID: 393 RVA: 0x000064F2 File Offset: 0x000046F2
		public TextViewModel(TextObject text, bool isVisible = true)
		{
			this.TextObject = text;
			this.IsVisible = isVisible;
		}

		// Token: 0x0600018A RID: 394 RVA: 0x00006508 File Offset: 0x00004708
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TextObject = this.TextObject;
		}

		// Token: 0x0400009F RID: 159
		private TextObject _textObject;

		// Token: 0x040000A0 RID: 160
		private string _text;

		// Token: 0x040000A1 RID: 161
		private bool _isVisible;
	}
}
