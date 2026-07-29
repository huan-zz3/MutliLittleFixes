using System;
using MissionLibrary.HotKey;
using MissionSharedLibrary.View.ViewModelCollection.HotKey;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace MissionSharedLibrary.View.HotKey
{
	// Token: 0x0200001C RID: 28
	public class GameKeyConfigVM : ViewModel
	{
		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000F7 RID: 247 RVA: 0x00005251 File Offset: 0x00003451
		// (set) Token: 0x060000F8 RID: 248 RVA: 0x00005259 File Offset: 0x00003459
		public MissionLibraryGameKeyOptionCategoryVM GameKeyOptions { get; set; }

		// Token: 0x060000F9 RID: 249 RVA: 0x00005262 File Offset: 0x00003462
		public GameKeyConfigVM(AGameKeyCategoryManager gameKeyCategoryManager, Action<IHotKeySetter> onKeyBindRequest, Action onClose)
		{
			this._onClose = onClose;
			this.GameKeyOptions = new MissionLibraryGameKeyOptionCategoryVM(gameKeyCategoryManager, onKeyBindRequest);
			this.RefreshValues();
		}

		// Token: 0x060000FA RID: 250 RVA: 0x00005284 File Offset: 0x00003484
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.GameKeyOptions.RefreshValues();
			this.CancelLbl = new TextObject("{=3CpNUnVl}Cancel", null).ToString();
			this.DoneLbl = new TextObject("{=WiNRdfsm}Done", null).ToString();
			this.ResetLbl = new TextObject("{=mAxXKaXp}Reset", null).ToString();
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060000FB RID: 251 RVA: 0x000052E4 File Offset: 0x000034E4
		// (set) Token: 0x060000FC RID: 252 RVA: 0x000052EC File Offset: 0x000034EC
		[DataSourceProperty]
		public string CancelLbl
		{
			get
			{
				return this._cancelLbl;
			}
			set
			{
				if (value == this._cancelLbl)
				{
					return;
				}
				this._cancelLbl = value;
				base.OnPropertyChangedWithValue<string>(value, "CancelLbl");
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060000FD RID: 253 RVA: 0x00005310 File Offset: 0x00003510
		// (set) Token: 0x060000FE RID: 254 RVA: 0x00005318 File Offset: 0x00003518
		[DataSourceProperty]
		public string DoneLbl
		{
			get
			{
				return this._doneLbl;
			}
			set
			{
				if (value == this._doneLbl)
				{
					return;
				}
				this._doneLbl = value;
				base.OnPropertyChangedWithValue<string>(value, "DoneLbl");
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060000FF RID: 255 RVA: 0x0000533C File Offset: 0x0000353C
		// (set) Token: 0x06000100 RID: 256 RVA: 0x00005344 File Offset: 0x00003544
		[DataSourceProperty]
		public string ResetLbl
		{
			get
			{
				return this._resetLbl;
			}
			set
			{
				if (value == this._resetLbl)
				{
					return;
				}
				this._resetLbl = value;
				base.OnPropertyChangedWithValue<string>(value, "ResetLbl");
			}
		}

		// Token: 0x06000101 RID: 257 RVA: 0x00005368 File Offset: 0x00003568
		public void Update()
		{
			this.GameKeyOptions.Update();
		}

		// Token: 0x06000102 RID: 258 RVA: 0x00005375 File Offset: 0x00003575
		protected void ExecuteDone()
		{
			this.GameKeyOptions.OnDone();
			Action onClose = this._onClose;
			if (onClose == null)
			{
				return;
			}
			onClose();
		}

		// Token: 0x06000103 RID: 259 RVA: 0x00005392 File Offset: 0x00003592
		public void ExecuteCancel()
		{
			Action onClose = this._onClose;
			if (onClose == null)
			{
				return;
			}
			onClose();
		}

		// Token: 0x06000104 RID: 260 RVA: 0x000053A4 File Offset: 0x000035A4
		protected void ExecuteReset()
		{
			InformationManager.ShowInquiry(new InquiryData("", new TextObject("{=cDzWYQrz}Reset to default settings?", null).ToString(), true, true, new TextObject("{=oHaWR73d}Ok", null).ToString(), new TextObject("{=3CpNUnVl}Cancel", null).ToString(), new Action(this.OnResetToDefaults), null, "", 0f, null, null, null), false, false);
		}

		// Token: 0x06000105 RID: 261 RVA: 0x0000540E File Offset: 0x0000360E
		private void OnResetToDefaults()
		{
			this.GameKeyOptions.OnReset();
		}

		// Token: 0x0400005D RID: 93
		private readonly Action _onClose;

		// Token: 0x0400005E RID: 94
		private string _cancelLbl;

		// Token: 0x0400005F RID: 95
		private string _doneLbl;

		// Token: 0x04000060 RID: 96
		private string _resetLbl;
	}
}
