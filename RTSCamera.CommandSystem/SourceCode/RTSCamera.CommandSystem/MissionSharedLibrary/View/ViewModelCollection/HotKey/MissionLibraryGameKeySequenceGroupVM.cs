using System;
using System.Collections.Generic;
using MissionLibrary.HotKey;
using MissionSharedLibrary.Config.HotKey;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace MissionSharedLibrary.View.ViewModelCollection.HotKey
{
	// Token: 0x02000032 RID: 50
	public class MissionLibraryGameKeySequenceGroupVM : AHotKeyConfigVM
	{
		// Token: 0x060001C3 RID: 451 RVA: 0x00006DB4 File Offset: 0x00004FB4
		public MissionLibraryGameKeySequenceGroupVM(string categoryId, IEnumerable<GameKeySequence> keys, Action<MissionLibraryGameKeyOptionVM> onKeyBindRequest)
		{
			this._categoryId = categoryId;
			this._gameKeySequenceOptions = new MBBindingList<MissionLibraryGameKeySequenceOptionVM>();
			foreach (GameKeySequence gameKeySequence in keys)
			{
				this._gameKeySequenceOptions.Add(new MissionLibraryGameKeySequenceOptionVM(gameKeySequence, onKeyBindRequest));
			}
			this.RefreshValues();
		}

		// Token: 0x060001C4 RID: 452 RVA: 0x00006E28 File Offset: 0x00005028
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Description = Module.CurrentModule.GlobalTextManager.FindText("str_key_category_name", this._categoryId).ToString();
			this.GameKeySequenceOptions.ApplyActionOnAllItems(delegate(MissionLibraryGameKeySequenceOptionVM x)
			{
				x.RefreshValues();
			});
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x00006E8A File Offset: 0x0000508A
		private Key FindValidInputKey(GameKey gameKey)
		{
			return gameKey.KeyboardKey;
		}

		// Token: 0x060001C6 RID: 454 RVA: 0x00006E94 File Offset: 0x00005094
		public override void Update()
		{
			foreach (MissionLibraryGameKeySequenceOptionVM missionLibraryGameKeySequenceOptionVM in this.GameKeySequenceOptions)
			{
				missionLibraryGameKeySequenceOptionVM.Update();
			}
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x00006EE0 File Offset: 0x000050E0
		public override void OnReset()
		{
			foreach (MissionLibraryGameKeySequenceOptionVM missionLibraryGameKeySequenceOptionVM in this.GameKeySequenceOptions)
			{
				missionLibraryGameKeySequenceOptionVM.OnReset();
			}
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x00006F2C File Offset: 0x0000512C
		public override void OnDone()
		{
			foreach (MissionLibraryGameKeySequenceOptionVM missionLibraryGameKeySequenceOptionVM in this.GameKeySequenceOptions)
			{
				missionLibraryGameKeySequenceOptionVM.OnDone();
			}
		}

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x060001C9 RID: 457 RVA: 0x00006F78 File Offset: 0x00005178
		// (set) Token: 0x060001CA RID: 458 RVA: 0x00006F80 File Offset: 0x00005180
		[DataSourceProperty]
		public MBBindingList<MissionLibraryGameKeySequenceOptionVM> GameKeySequenceOptions
		{
			get
			{
				return this._gameKeySequenceOptions;
			}
			set
			{
				if (value == this._gameKeySequenceOptions)
				{
					return;
				}
				this._gameKeySequenceOptions = value;
				base.OnPropertyChangedWithValue<MBBindingList<MissionLibraryGameKeySequenceOptionVM>>(value, "GameKeySequenceOptions");
			}
		}

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x060001CB RID: 459 RVA: 0x00006F9F File Offset: 0x0000519F
		// (set) Token: 0x060001CC RID: 460 RVA: 0x00006FA7 File Offset: 0x000051A7
		[DataSourceProperty]
		public string Description
		{
			get
			{
				return this._description;
			}
			set
			{
				if (value == this._description)
				{
					return;
				}
				this._description = value;
				base.OnPropertyChangedWithValue<string>(value, "Description");
			}
		}

		// Token: 0x040000B9 RID: 185
		private readonly string _categoryId;

		// Token: 0x040000BA RID: 186
		private string _description;

		// Token: 0x040000BB RID: 187
		private MBBindingList<MissionLibraryGameKeySequenceOptionVM> _gameKeySequenceOptions;
	}
}
