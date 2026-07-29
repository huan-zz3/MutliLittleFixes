using System;
using MissionLibrary.HotKey;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace MissionSharedLibrary.View.ViewModelCollection.HotKey
{
	// Token: 0x02000031 RID: 49
	public class MissionLibraryGameKeyOptionVM : ViewModel, IHotKeySetter
	{
		// Token: 0x17000061 RID: 97
		// (get) Token: 0x060001B5 RID: 437 RVA: 0x00006C1E File Offset: 0x00004E1E
		// (set) Token: 0x060001B6 RID: 438 RVA: 0x00006C26 File Offset: 0x00004E26
		public Key CurrentKey { get; private set; }

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x060001B7 RID: 439 RVA: 0x00006C2F File Offset: 0x00004E2F
		// (set) Token: 0x060001B8 RID: 440 RVA: 0x00006C37 File Offset: 0x00004E37
		public Key Key { get; private set; }

		// Token: 0x060001B9 RID: 441 RVA: 0x00006C40 File Offset: 0x00004E40
		public MissionLibraryGameKeyOptionVM(Key key, Action<MissionLibraryGameKeyOptionVM> onKeybindRequest)
		{
			this._onKeybindRequest = onKeybindRequest;
			this.Key = key;
			this.CurrentKey = new Key(this.Key.InputKey);
			this.RefreshValues();
		}

		// Token: 0x060001BA RID: 442 RVA: 0x00006C72 File Offset: 0x00004E72
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.OptionValueText = Module.CurrentModule.GlobalTextManager.FindText("str_game_key_text", this.CurrentKey.ToString().ToLower()).ToString();
		}

		// Token: 0x060001BB RID: 443 RVA: 0x00006CA9 File Offset: 0x00004EA9
		private void ExecuteKeybindRequest()
		{
			this._onKeybindRequest(this);
		}

		// Token: 0x060001BC RID: 444 RVA: 0x00006CB7 File Offset: 0x00004EB7
		public void Set(InputKey newKey)
		{
			this.OnKeySet(newKey);
		}

		// Token: 0x060001BD RID: 445 RVA: 0x00006CC0 File Offset: 0x00004EC0
		private void OnKeySet(InputKey key)
		{
			this.CurrentKey.ChangeKey(key);
			this.OptionValueText = Module.CurrentModule.GlobalTextManager.FindText("str_game_key_text", this.CurrentKey.ToString().ToLower()).ToString();
		}

		// Token: 0x060001BE RID: 446 RVA: 0x00006D00 File Offset: 0x00004F00
		public void Update()
		{
			this.CurrentKey = new Key(this.Key.InputKey);
			this.OptionValueText = Module.CurrentModule.GlobalTextManager.FindText("str_game_key_text", this.CurrentKey.ToString().ToLower()).ToString();
		}

		// Token: 0x060001BF RID: 447 RVA: 0x00006D52 File Offset: 0x00004F52
		public void OnDone()
		{
			this.Key.ChangeKey(this.CurrentKey.InputKey);
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x00006D6A File Offset: 0x00004F6A
		internal bool IsChanged()
		{
			return this.CurrentKey.InputKey != this.Key.InputKey;
		}

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x060001C1 RID: 449 RVA: 0x00006D87 File Offset: 0x00004F87
		// (set) Token: 0x060001C2 RID: 450 RVA: 0x00006D8F File Offset: 0x00004F8F
		[DataSourceProperty]
		public string OptionValueText
		{
			get
			{
				return this._optionValueText;
			}
			set
			{
				if (this._optionValueText == value)
				{
					return;
				}
				this._optionValueText = value;
				base.OnPropertyChangedWithValue<string>(value, "OptionValueText");
			}
		}

		// Token: 0x040000B5 RID: 181
		private readonly Action<MissionLibraryGameKeyOptionVM> _onKeybindRequest;

		// Token: 0x040000B6 RID: 182
		private string _optionValueText;
	}
}
