using System;
using System.Collections.Generic;
using System.Linq;
using MissionLibrary.HotKey;
using MissionSharedLibrary.Config.HotKey;
using MissionSharedLibrary.View.ViewModelCollection.Basic;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace MissionSharedLibrary.View.ViewModelCollection.HotKey
{
	// Token: 0x02000033 RID: 51
	public class MissionLibraryGameKeySequenceOptionVM : AHotKeyConfigVM
	{
		// Token: 0x17000066 RID: 102
		// (get) Token: 0x060001CD RID: 461 RVA: 0x00006FCB File Offset: 0x000051CB
		// (set) Token: 0x060001CE RID: 462 RVA: 0x00006FD3 File Offset: 0x000051D3
		public GameKeySequence GameKeySequence { get; private set; }

		// Token: 0x060001CF RID: 463 RVA: 0x00006FDC File Offset: 0x000051DC
		public MissionLibraryGameKeySequenceOptionVM(GameKeySequence gameKeySequence, Action<MissionLibraryGameKeyOptionVM> onKeybindRequest)
		{
			this._onKeybindRequest = onKeybindRequest;
			this.GameKeySequence = gameKeySequence;
			this._groupId = this.GameKeySequence.CategoryId;
			this._id = this.GameKeySequence.StringId;
			this.UpdateAlternatives();
			this.RefreshValues();
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x00007064 File Offset: 0x00005264
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = Module.CurrentModule.GlobalTextManager.FindText("str_key_name", this._groupId + "_" + this._id).ToString();
			this.Description = Module.CurrentModule.GlobalTextManager.FindText("str_key_description", this._groupId + "_" + this._id).ToString();
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x000070E4 File Offset: 0x000052E4
		public override void Update()
		{
			foreach (MissionLibraryGameKeySequenceAlternativeOptionVM missionLibraryGameKeySequenceAlternativeOptionVM in this._alternatives)
			{
				missionLibraryGameKeySequenceAlternativeOptionVM.Update();
			}
			foreach (MissionLibraryGameKeySequenceAlternativeOptionVM missionLibraryGameKeySequenceAlternativeOptionVM2 in this._alternatives.Where<MissionLibraryGameKeySequenceAlternativeOptionVM>((MissionLibraryGameKeySequenceAlternativeOptionVM alternative) => alternative.Options.Count == 0).ToList<MissionLibraryGameKeySequenceAlternativeOptionVM>())
			{
				this.Alternatives.Remove(missionLibraryGameKeySequenceAlternativeOptionVM2);
			}
			this.UpdateButtons();
		}

		// Token: 0x060001D2 RID: 466 RVA: 0x000071A8 File Offset: 0x000053A8
		public override void OnDone()
		{
			foreach (MissionLibraryGameKeySequenceAlternativeOptionVM missionLibraryGameKeySequenceAlternativeOptionVM in this._alternatives)
			{
				missionLibraryGameKeySequenceAlternativeOptionVM.OnDone();
			}
			this.GameKeySequence.SetGameKeys(this.Alternatives.Select<MissionLibraryGameKeySequenceAlternativeOptionVM, GameKeySequenceAlternative>((MissionLibraryGameKeySequenceAlternativeOptionVM vm) => vm.GameKeySequenceAlternative).ToList<GameKeySequenceAlternative>());
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x0000722C File Offset: 0x0000542C
		public override void OnReset()
		{
			this.GameKeySequence.ResetToDefault();
			this.UpdateAlternatives();
		}

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x060001D4 RID: 468 RVA: 0x0000723F File Offset: 0x0000543F
		// (set) Token: 0x060001D5 RID: 469 RVA: 0x00007247 File Offset: 0x00005447
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

		// Token: 0x060001D6 RID: 470 RVA: 0x0000725C File Offset: 0x0000545C
		public void PushAlternative()
		{
			MissionLibraryGameKeySequenceAlternativeOptionVM missionLibraryGameKeySequenceAlternativeOptionVM = new MissionLibraryGameKeySequenceAlternativeOptionVM(new GameKeySequenceAlternative(new List<InputKey> { -1 }), this._onKeybindRequest);
			this.Alternatives.Add(missionLibraryGameKeySequenceAlternativeOptionVM);
			this.UpdateButtons();
			if (missionLibraryGameKeySequenceAlternativeOptionVM.Options.Count < 1)
			{
				return;
			}
			Action<MissionLibraryGameKeyOptionVM> onKeybindRequest = this._onKeybindRequest;
			if (onKeybindRequest == null)
			{
				return;
			}
			onKeybindRequest(missionLibraryGameKeySequenceAlternativeOptionVM.Options.First<MissionLibraryGameKeyOptionVM>());
		}

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x060001D7 RID: 471 RVA: 0x000072C2 File Offset: 0x000054C2
		// (set) Token: 0x060001D8 RID: 472 RVA: 0x000072CA File Offset: 0x000054CA
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

		// Token: 0x060001D9 RID: 473 RVA: 0x000072DE File Offset: 0x000054DE
		public void PopAlternative()
		{
			this.Alternatives.RemoveAt(this.Alternatives.Count - 1);
			this.UpdateButtons();
		}

		// Token: 0x060001DA RID: 474 RVA: 0x000072FE File Offset: 0x000054FE
		public bool IsChanged()
		{
			return this._alternatives.Any<MissionLibraryGameKeySequenceAlternativeOptionVM>((MissionLibraryGameKeySequenceAlternativeOptionVM option) => option.IsChanged());
		}

		// Token: 0x060001DB RID: 475 RVA: 0x0000732A File Offset: 0x0000552A
		private void UpdateButtons()
		{
			this.PopEnabled = this.Alternatives.Count > ((this.GameKeySequence.Mandatory > false) ? 1 : 0);
			this.PushEnabled = this.Alternatives.Count < 4;
		}

		// Token: 0x060001DC RID: 476 RVA: 0x00007364 File Offset: 0x00005564
		private void UpdateAlternatives()
		{
			this.Alternatives = new MBBindingList<MissionLibraryGameKeySequenceAlternativeOptionVM>();
			foreach (GameKeySequenceAlternative gameKeySequenceAlternative in this.GameKeySequence.KeyAlternatives)
			{
				this.Alternatives.Add(new MissionLibraryGameKeySequenceAlternativeOptionVM(gameKeySequenceAlternative, this._onKeybindRequest));
			}
			this.UpdateButtons();
		}

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x060001DD RID: 477 RVA: 0x000073E0 File Offset: 0x000055E0
		// (set) Token: 0x060001DE RID: 478 RVA: 0x000073E8 File Offset: 0x000055E8
		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (this._name == value)
				{
					return;
				}
				this._name = value;
				base.OnPropertyChangedWithValue<string>(value, "Name");
			}
		}

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x060001DF RID: 479 RVA: 0x0000740C File Offset: 0x0000560C
		// (set) Token: 0x060001E0 RID: 480 RVA: 0x00007414 File Offset: 0x00005614
		[DataSourceProperty]
		public string Description
		{
			get
			{
				return this._description;
			}
			set
			{
				if (this._description == value)
				{
					return;
				}
				this._description = value;
				base.OnPropertyChangedWithValue<string>(value, "Description");
			}
		}

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x060001E1 RID: 481 RVA: 0x00007438 File Offset: 0x00005638
		// (set) Token: 0x060001E2 RID: 482 RVA: 0x00007440 File Offset: 0x00005640
		[DataSourceProperty]
		public MBBindingList<MissionLibraryGameKeySequenceAlternativeOptionVM> Alternatives
		{
			get
			{
				return this._alternatives;
			}
			set
			{
				if (this._alternatives == value)
				{
					return;
				}
				this._alternatives = value;
				base.OnPropertyChanged("Alternatives");
			}
		}

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x060001E3 RID: 483 RVA: 0x0000745E File Offset: 0x0000565E
		// (set) Token: 0x060001E4 RID: 484 RVA: 0x00007466 File Offset: 0x00005666
		[DataSourceProperty]
		public string GameKeySequenceOptionVMVersion { get; private set; } = "v2";

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x060001E5 RID: 485 RVA: 0x0000746F File Offset: 0x0000566F
		public TextViewModel AddShortcut { get; } = new TextViewModel(GameTexts.FindText("str_mission_library_hotkey_add_shortcut", null), true);

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x060001E6 RID: 486 RVA: 0x00007477 File Offset: 0x00005677
		public TextViewModel RemoveShortcut { get; } = new TextViewModel(GameTexts.FindText("str_mission_library_hotkey_remove_shortcut", null), true);

		// Token: 0x040000BC RID: 188
		private readonly Action<MissionLibraryGameKeyOptionVM> _onKeybindRequest;

		// Token: 0x040000BD RID: 189
		private readonly string _groupId;

		// Token: 0x040000BE RID: 190
		private readonly string _id;

		// Token: 0x040000BF RID: 191
		private string _name;

		// Token: 0x040000C0 RID: 192
		private string _description;

		// Token: 0x040000C1 RID: 193
		private MBBindingList<MissionLibraryGameKeySequenceAlternativeOptionVM> _alternatives;

		// Token: 0x040000C2 RID: 194
		private bool _pushEnabled;

		// Token: 0x040000C3 RID: 195
		private bool _popEnabled;
	}
}
