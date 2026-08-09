using System;
using NavalDLC.CustomBattle.CustomBattle.SelectionItem;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace NavalDLC.CustomBattle.CustomBattle
{
	// Token: 0x02000011 RID: 17
	public class NavalCustomBattleGameTypeSelectionGroupVM : ViewModel
	{
		// Token: 0x1700004A RID: 74
		// (get) Token: 0x060000A7 RID: 167 RVA: 0x00004BF1 File Offset: 0x00002DF1
		// (set) Token: 0x060000A8 RID: 168 RVA: 0x00004BF9 File Offset: 0x00002DF9
		public NavalCustomBattlePlayerSide SelectedPlayerSide { get; private set; }

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x060000A9 RID: 169 RVA: 0x00004C02 File Offset: 0x00002E02
		// (set) Token: 0x060000AA RID: 170 RVA: 0x00004C0A File Offset: 0x00002E0A
		public string SelectedGameTypeStringId { get; private set; }

		// Token: 0x060000AB RID: 171 RVA: 0x00004C14 File Offset: 0x00002E14
		public NavalCustomBattleGameTypeSelectionGroupVM(Action<string> onGameTypeChange, Action onPlayerSideChange)
		{
			this._onGameTypeChange = onGameTypeChange;
			this._onPlayerSideChange = onPlayerSideChange;
			this.PlayerSideSelection = new SelectorVM<NavalCustomBattlePlayerSideItemVM>(0, new Action<SelectorVM<NavalCustomBattlePlayerSideItemVM>>(this.OnPlayerSideSelection));
			this.GameTypeSelection = new SelectorVM<NavalGameTypeItemVM>(0, new Action<SelectorVM<NavalGameTypeItemVM>>(this.OnGameTypeSelection));
			this.RefreshValues();
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00004C6C File Offset: 0x00002E6C
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.GameTypeText = new TextObject("{=JPimShCw}Game Type", null).ToString();
			this.PlayerSideText = new TextObject("{=P3rMg4uZ}Player Side", null).ToString();
			this.GameTypeSelection.ItemList.Clear();
			this.PlayerSideSelection.ItemList.Clear();
			foreach (Tuple<string, string> tuple in NavalCustomBattleData.GameTypes)
			{
				this.GameTypeSelection.AddItem(new NavalGameTypeItemVM(tuple.Item1, tuple.Item2));
			}
			foreach (Tuple<string, NavalCustomBattlePlayerSide> tuple2 in NavalCustomBattleData.PlayerSides)
			{
				this.PlayerSideSelection.AddItem(new NavalCustomBattlePlayerSideItemVM(tuple2.Item1, tuple2.Item2));
			}
			this.GameTypeSelection.SelectedIndex = 0;
			this.PlayerSideSelection.SelectedIndex = 0;
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00004D88 File Offset: 0x00002F88
		public void RandomizeAll()
		{
			this.GameTypeSelection.ExecuteRandomize();
			this.PlayerSideSelection.ExecuteRandomize();
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00004DA0 File Offset: 0x00002FA0
		private void OnGameTypeSelection(SelectorVM<NavalGameTypeItemVM> selector)
		{
			this.SelectedGameTypeStringId = selector.SelectedItem.GameTypeStringId;
			this._onGameTypeChange(this.SelectedGameTypeStringId);
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00004DC4 File Offset: 0x00002FC4
		private void OnPlayerSideSelection(SelectorVM<NavalCustomBattlePlayerSideItemVM> selector)
		{
			this.SelectedPlayerSide = selector.SelectedItem.PlayerSide;
			Action onPlayerSideChange = this._onPlayerSideChange;
			if (onPlayerSideChange == null)
			{
				return;
			}
			onPlayerSideChange();
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x060000B0 RID: 176 RVA: 0x00004DE7 File Offset: 0x00002FE7
		// (set) Token: 0x060000B1 RID: 177 RVA: 0x00004DEF File Offset: 0x00002FEF
		[DataSourceProperty]
		public SelectorVM<NavalGameTypeItemVM> GameTypeSelection
		{
			get
			{
				return this._gameTypeSelection;
			}
			set
			{
				if (value != this._gameTypeSelection)
				{
					this._gameTypeSelection = value;
					base.OnPropertyChangedWithValue<SelectorVM<NavalGameTypeItemVM>>(value, "GameTypeSelection");
				}
			}
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x060000B2 RID: 178 RVA: 0x00004E0D File Offset: 0x0000300D
		// (set) Token: 0x060000B3 RID: 179 RVA: 0x00004E15 File Offset: 0x00003015
		[DataSourceProperty]
		public SelectorVM<NavalCustomBattlePlayerSideItemVM> PlayerSideSelection
		{
			get
			{
				return this._playerSideSelection;
			}
			set
			{
				if (value != this._playerSideSelection)
				{
					this._playerSideSelection = value;
					base.OnPropertyChangedWithValue<SelectorVM<NavalCustomBattlePlayerSideItemVM>>(value, "PlayerSideSelection");
				}
			}
		}

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x060000B4 RID: 180 RVA: 0x00004E33 File Offset: 0x00003033
		// (set) Token: 0x060000B5 RID: 181 RVA: 0x00004E3B File Offset: 0x0000303B
		[DataSourceProperty]
		public string GameTypeText
		{
			get
			{
				return this._gameTypeText;
			}
			set
			{
				if (value != this._gameTypeText)
				{
					this._gameTypeText = value;
					base.OnPropertyChangedWithValue<string>(value, "GameTypeText");
				}
			}
		}

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x060000B6 RID: 182 RVA: 0x00004E5E File Offset: 0x0000305E
		// (set) Token: 0x060000B7 RID: 183 RVA: 0x00004E66 File Offset: 0x00003066
		[DataSourceProperty]
		public string PlayerSideText
		{
			get
			{
				return this._playerSideText;
			}
			set
			{
				if (value != this._playerSideText)
				{
					this._playerSideText = value;
					base.OnPropertyChangedWithValue<string>(value, "PlayerSideText");
				}
			}
		}

		// Token: 0x04000052 RID: 82
		private readonly Action<string> _onGameTypeChange;

		// Token: 0x04000053 RID: 83
		private readonly Action _onPlayerSideChange;

		// Token: 0x04000054 RID: 84
		private SelectorVM<NavalGameTypeItemVM> _gameTypeSelection;

		// Token: 0x04000055 RID: 85
		private SelectorVM<NavalCustomBattlePlayerSideItemVM> _playerSideSelection;

		// Token: 0x04000056 RID: 86
		private string _gameTypeText;

		// Token: 0x04000057 RID: 87
		private string _playerSideText;
	}
}
