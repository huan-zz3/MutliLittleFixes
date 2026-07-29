using System;
using System.Collections.Generic;
using System.Linq;
using MissionLibrary.HotKey;
using MissionLibrary.Provider;
using MissionSharedLibrary.Utilities;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace MissionSharedLibrary.View.ViewModelCollection.HotKey
{
	// Token: 0x02000034 RID: 52
	public class MissionLibraryGameKeyOptionCategoryVM : ViewModel
	{
		// Token: 0x060001E7 RID: 487 RVA: 0x00007480 File Offset: 0x00005680
		public MissionLibraryGameKeyOptionCategoryVM(AGameKeyCategoryManager gameKeyCategoryManager, Action<IHotKeySetter> onKeyBindRequest)
		{
			this._gameKeyCategoryManager = gameKeyCategoryManager;
			this._onKeyBindRequest = onKeyBindRequest;
			this._categories = this._gameKeyCategoryManager.Items.ToDictionary<KeyValuePair<string, IProvider<AGameKeyCategory>>, string, AGameKeyCategory>((KeyValuePair<string, IProvider<AGameKeyCategory>> pair) => pair.Key, (KeyValuePair<string, IProvider<AGameKeyCategory>> pair) => pair.Value.Value);
			this.Groups = new MBBindingList<AHotKeyConfigVM>();
			foreach (KeyValuePair<string, AGameKeyCategory> keyValuePair in this._categories)
			{
				this.Groups.Add(keyValuePair.Value.CreateViewModel(onKeyBindRequest));
			}
			this.RefreshValues();
		}

		// Token: 0x060001E8 RID: 488 RVA: 0x00007568 File Offset: 0x00005768
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = new TextObject("{=Met1U45t}Mouse and Keyboard", null).ToString();
			this.Groups.ApplyActionOnAllItems(delegate(AHotKeyConfigVM x)
			{
				x.RefreshValues();
			});
			this.ResetText = new TextObject("{=RVIKFCno}Reset to Defaults", null).ToString();
		}

		// Token: 0x060001E9 RID: 489 RVA: 0x000075D4 File Offset: 0x000057D4
		public void Update()
		{
			foreach (AHotKeyConfigVM ahotKeyConfigVM in this.Groups)
			{
				ahotKeyConfigVM.Update();
			}
		}

		// Token: 0x060001EA RID: 490 RVA: 0x00007620 File Offset: 0x00005820
		public void OnReset()
		{
			try
			{
				foreach (AHotKeyConfigVM ahotKeyConfigVM in this.Groups)
				{
					ahotKeyConfigVM.ExecuteCommand("OnReset", new object[0]);
				}
				this._keysToChangeOnDone.Clear();
			}
			catch (Exception ex)
			{
				Utility.DisplayMessageForced(ex.ToString());
			}
		}

		// Token: 0x060001EB RID: 491 RVA: 0x0000769C File Offset: 0x0000589C
		public void OnDone()
		{
			foreach (AHotKeyConfigVM ahotKeyConfigVM in this.Groups)
			{
				ahotKeyConfigVM.OnDone();
			}
			foreach (KeyValuePair<GameKey, InputKey> keyValuePair in this._keysToChangeOnDone)
			{
				this.FindValidInputKey(keyValuePair.Key).ChangeKey(keyValuePair.Value);
			}
			this._gameKeyCategoryManager.Save();
		}

		// Token: 0x060001EC RID: 492 RVA: 0x00007748 File Offset: 0x00005948
		private Key FindValidInputKey(GameKey gameKey)
		{
			return gameKey.KeyboardKey;
		}

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x060001ED RID: 493 RVA: 0x00007750 File Offset: 0x00005950
		// (set) Token: 0x060001EE RID: 494 RVA: 0x00007758 File Offset: 0x00005958
		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value == this._name)
				{
					return;
				}
				this._name = value;
				base.OnPropertyChangedWithValue<string>(value, "Name");
			}
		}

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x060001EF RID: 495 RVA: 0x0000777C File Offset: 0x0000597C
		// (set) Token: 0x060001F0 RID: 496 RVA: 0x00007784 File Offset: 0x00005984
		[DataSourceProperty]
		public string ResetText
		{
			get
			{
				return this._resetText;
			}
			set
			{
				if (this._resetText == value)
				{
					return;
				}
				this._resetText = value;
				base.OnPropertyChangedWithValue<string>(value, "ResetText");
			}
		}

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x060001F1 RID: 497 RVA: 0x000077A8 File Offset: 0x000059A8
		// (set) Token: 0x060001F2 RID: 498 RVA: 0x000077B0 File Offset: 0x000059B0
		[DataSourceProperty]
		public MBBindingList<AHotKeyConfigVM> Groups
		{
			get
			{
				return this._groups;
			}
			set
			{
				if (value == this._groups)
				{
					return;
				}
				this._groups = value;
				base.OnPropertyChangedWithValue<MBBindingList<AHotKeyConfigVM>>(value, "Groups");
			}
		}

		// Token: 0x060001F3 RID: 499 RVA: 0x000077D0 File Offset: 0x000059D0
		public void ExecuteResetToDefault()
		{
			InformationManager.ShowInquiry(new InquiryData(new TextObject("{=4gCU2ykB}Reset all keys to default", null).ToString(), new TextObject("{=YjbNtFcw}This will reset ALL keys to their default states. You won't be able to undo this action. {newline} {newline}Are you sure?", null).ToString(), true, true, new TextObject("{=aeouhelq}Yes", null).ToString(), new TextObject("{=8OkPHu4f}No", null).ToString(), new Action(this.OnReset), null, "", 0f, null, null, null), false, false);
		}

		// Token: 0x040000C8 RID: 200
		private readonly AGameKeyCategoryManager _gameKeyCategoryManager;

		// Token: 0x040000C9 RID: 201
		private readonly Action<MissionLibraryGameKeyOptionVM> _onKeyBindRequest;

		// Token: 0x040000CA RID: 202
		private readonly Dictionary<GameKey, InputKey> _keysToChangeOnDone = new Dictionary<GameKey, InputKey>();

		// Token: 0x040000CB RID: 203
		private string _name;

		// Token: 0x040000CC RID: 204
		private string _resetText;

		// Token: 0x040000CD RID: 205
		private MBBindingList<AHotKeyConfigVM> _groups;

		// Token: 0x040000CE RID: 206
		private readonly Dictionary<string, AGameKeyCategory> _categories;
	}
}
