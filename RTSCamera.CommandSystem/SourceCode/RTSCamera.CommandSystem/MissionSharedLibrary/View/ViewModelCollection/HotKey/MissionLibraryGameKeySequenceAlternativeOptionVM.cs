using System;
using System.Linq;
using MissionLibrary.HotKey;
using MissionSharedLibrary.Config.HotKey;
using MissionSharedLibrary.View.ViewModelCollection.Basic;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace MissionSharedLibrary.View.ViewModelCollection.HotKey
{
	// Token: 0x02000030 RID: 48
	public class MissionLibraryGameKeySequenceAlternativeOptionVM : AHotKeyConfigVM
	{
		// Token: 0x1700005B RID: 91
		// (get) Token: 0x060001A2 RID: 418 RVA: 0x000068FD File Offset: 0x00004AFD
		// (set) Token: 0x060001A3 RID: 419 RVA: 0x00006905 File Offset: 0x00004B05
		public GameKeySequenceAlternative GameKeySequenceAlternative { get; private set; }

		// Token: 0x060001A4 RID: 420 RVA: 0x00006910 File Offset: 0x00004B10
		public MissionLibraryGameKeySequenceAlternativeOptionVM(GameKeySequenceAlternative gameKeySequenceAlternative, Action<MissionLibraryGameKeyOptionVM> onKeybindRequest)
		{
			this._onKeybindRequest = onKeybindRequest;
			this.GameKeySequenceAlternative = gameKeySequenceAlternative;
			this.UpdateOptions();
			this.RefreshValues();
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x0000696C File Offset: 0x00004B6C
		public override void Update()
		{
			foreach (MissionLibraryGameKeyOptionVM missionLibraryGameKeyOptionVM in this._options.Where<MissionLibraryGameKeyOptionVM>((MissionLibraryGameKeyOptionVM option) => option.CurrentKey.InputKey == -1).ToList<MissionLibraryGameKeyOptionVM>())
			{
				this.Options.Remove(missionLibraryGameKeyOptionVM);
			}
			this.UpdateButtons();
		}

		// Token: 0x060001A6 RID: 422 RVA: 0x000069F4 File Offset: 0x00004BF4
		public override void OnDone()
		{
			foreach (MissionLibraryGameKeyOptionVM missionLibraryGameKeyOptionVM in this._options)
			{
				missionLibraryGameKeyOptionVM.OnDone();
			}
			this.GameKeySequenceAlternative.SetGameKeys(this.Options.Select<MissionLibraryGameKeyOptionVM, InputKey>((MissionLibraryGameKeyOptionVM vm) => vm.Key.InputKey).ToList<InputKey>());
		}

		// Token: 0x060001A7 RID: 423 RVA: 0x00006A78 File Offset: 0x00004C78
		public override void OnReset()
		{
		}

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x060001A8 RID: 424 RVA: 0x00006A7A File Offset: 0x00004C7A
		// (set) Token: 0x060001A9 RID: 425 RVA: 0x00006A82 File Offset: 0x00004C82
		[DataSourceProperty]
		public bool PushEnabled
		{
			get
			{
				return this._pushEnabled;
			}
			set
			{
				this._pushEnabled = value;
				base.OnPropertyChanged("PushEnabled");
			}
		}

		// Token: 0x060001AA RID: 426 RVA: 0x00006A98 File Offset: 0x00004C98
		public void PushGameKey()
		{
			MissionLibraryGameKeyOptionVM missionLibraryGameKeyOptionVM = new MissionLibraryGameKeyOptionVM(new Key(-1), this._onKeybindRequest);
			this.Options.Add(missionLibraryGameKeyOptionVM);
			this.UpdateButtons();
			Action<MissionLibraryGameKeyOptionVM> onKeybindRequest = this._onKeybindRequest;
			if (onKeybindRequest == null)
			{
				return;
			}
			onKeybindRequest(missionLibraryGameKeyOptionVM);
		}

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x060001AB RID: 427 RVA: 0x00006ADA File Offset: 0x00004CDA
		// (set) Token: 0x060001AC RID: 428 RVA: 0x00006AE2 File Offset: 0x00004CE2
		[DataSourceProperty]
		public bool PopEnabled
		{
			get
			{
				return this._popEnabled;
			}
			set
			{
				this._popEnabled = value;
				base.OnPropertyChanged("PopEnabled");
			}
		}

		// Token: 0x060001AD RID: 429 RVA: 0x00006AF6 File Offset: 0x00004CF6
		public void PopGameKey()
		{
			this.Options.RemoveAt(this.Options.Count - 1);
			this.UpdateButtons();
		}

		// Token: 0x060001AE RID: 430 RVA: 0x00006B16 File Offset: 0x00004D16
		public bool IsChanged()
		{
			return this._options.Any<MissionLibraryGameKeyOptionVM>((MissionLibraryGameKeyOptionVM option) => option.IsChanged());
		}

		// Token: 0x060001AF RID: 431 RVA: 0x00006B42 File Offset: 0x00004D42
		private void UpdateButtons()
		{
			this.PopEnabled = this.Options.Count > 1;
			this.PushEnabled = this.Options.Count < 4;
		}

		// Token: 0x060001B0 RID: 432 RVA: 0x00006B6C File Offset: 0x00004D6C
		private void UpdateOptions()
		{
			this.Options = new MBBindingList<MissionLibraryGameKeyOptionVM>();
			foreach (Key key in this.GameKeySequenceAlternative.Keys)
			{
				this.Options.Add(new MissionLibraryGameKeyOptionVM(key, this._onKeybindRequest));
			}
			this.UpdateButtons();
		}

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x060001B1 RID: 433 RVA: 0x00006BE8 File Offset: 0x00004DE8
		// (set) Token: 0x060001B2 RID: 434 RVA: 0x00006BF0 File Offset: 0x00004DF0
		[DataSourceProperty]
		public MBBindingList<MissionLibraryGameKeyOptionVM> Options
		{
			get
			{
				return this._options;
			}
			set
			{
				if (this._options == value)
				{
					return;
				}
				this._options = value;
				base.OnPropertyChanged("Options");
			}
		}

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x060001B3 RID: 435 RVA: 0x00006C0E File Offset: 0x00004E0E
		public TextViewModel AddKey { get; } = new TextViewModel(GameTexts.FindText("str_mission_library_hotkey_add_key", null), true);

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x060001B4 RID: 436 RVA: 0x00006C16 File Offset: 0x00004E16
		public TextViewModel RemoveKey { get; } = new TextViewModel(GameTexts.FindText("str_mission_library_hotkey_remove_key", null), true);

		// Token: 0x040000AE RID: 174
		private readonly Action<MissionLibraryGameKeyOptionVM> _onKeybindRequest;

		// Token: 0x040000AF RID: 175
		private MBBindingList<MissionLibraryGameKeyOptionVM> _options;

		// Token: 0x040000B0 RID: 176
		private bool _pushEnabled;

		// Token: 0x040000B1 RID: 177
		private bool _popEnabled;
	}
}
