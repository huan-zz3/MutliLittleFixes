using System;
using MissionLibrary.View;
using MissionSharedLibrary.View.HotKey;
using MissionSharedLibrary.View.ViewModelCollection.Basic;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace MissionSharedLibrary.View
{
	// Token: 0x02000018 RID: 24
	public class OptionVM : MissionMenuVMBase
	{
		// Token: 0x060000C8 RID: 200 RVA: 0x00004C04 File Offset: 0x00002E04
		public OptionVM(AMenuClassCollection menuClassCollection, Action closeMenu)
			: base(closeMenu)
		{
			this._menuClassCollection = menuClassCollection;
			this.OptionClassCollection = menuClassCollection.GetViewModel();
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x00004C97 File Offset: 0x00002E97
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.ConfigKeyTitle.RefreshValues();
			this.ConfigKeyHint.RefreshValues();
			this.ShowUsageTitle.RefreshValues();
			this.ShowUsageHint.RefreshValues();
			this.OptionClassCollection.RefreshValues();
		}

		// Token: 0x060000CA RID: 202 RVA: 0x00004CD6 File Offset: 0x00002ED6
		public override void OnFinalize()
		{
			base.OnFinalize();
			this._menuClassCollection.Clear();
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x060000CB RID: 203 RVA: 0x00004CE9 File Offset: 0x00002EE9
		// (set) Token: 0x060000CC RID: 204 RVA: 0x00004CF1 File Offset: 0x00002EF1
		public TextViewModel ConfigKeyTitle { get; set; } = new TextViewModel(GameTexts.FindText("str_mission_library_gamekey_config", null), true);

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x060000CD RID: 205 RVA: 0x00004CFA File Offset: 0x00002EFA
		// (set) Token: 0x060000CE RID: 206 RVA: 0x00004D02 File Offset: 0x00002F02
		public HintViewModel ConfigKeyHint { get; set; } = new HintViewModel(GameTexts.FindText("str_mission_library_config_key_hint", null), null);

		// Token: 0x060000CF RID: 207 RVA: 0x00004D0B File Offset: 0x00002F0B
		public void ConfigKey()
		{
			InformationManager.HideInquiry();
			GameKeyConfigView gameKeyConfigView = this._gameKeyConfigView;
			if (gameKeyConfigView == null)
			{
				return;
			}
			gameKeyConfigView.Activate();
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x060000D0 RID: 208 RVA: 0x00004D22 File Offset: 0x00002F22
		// (set) Token: 0x060000D1 RID: 209 RVA: 0x00004D2A File Offset: 0x00002F2A
		public TextViewModel ShowUsageTitle { get; set; } = new TextViewModel(GameTexts.FindText("str_mission_library_show_usage", null), true);

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x060000D2 RID: 210 RVA: 0x00004D33 File Offset: 0x00002F33
		// (set) Token: 0x060000D3 RID: 211 RVA: 0x00004D3B File Offset: 0x00002F3B
		public HintViewModel ShowUsageHint { get; set; } = new HintViewModel(GameTexts.FindText("str_mission_library_show_usage_hint", null), null);

		// Token: 0x060000D4 RID: 212 RVA: 0x00004D44 File Offset: 0x00002F44
		public void ShowUsageView()
		{
			Mission mission = Mission.Current;
			if (mission == null)
			{
				return;
			}
			UsageView missionBehavior = mission.GetMissionBehavior<UsageView>();
			if (missionBehavior == null)
			{
				return;
			}
			missionBehavior.ActivateMenu();
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x060000D5 RID: 213 RVA: 0x00004D5F File Offset: 0x00002F5F
		public ViewModel OptionClassCollection { get; }

		// Token: 0x04000046 RID: 70
		private readonly AMenuClassCollection _menuClassCollection;

		// Token: 0x04000049 RID: 73
		private readonly GameKeyConfigView _gameKeyConfigView = Mission.Current.GetMissionBehavior<GameKeyConfigView>();
	}
}
