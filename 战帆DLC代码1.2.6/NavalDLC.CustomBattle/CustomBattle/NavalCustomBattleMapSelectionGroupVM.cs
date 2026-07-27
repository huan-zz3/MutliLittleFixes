using System;
using System.Collections.Generic;
using NavalDLC.CustomBattle.CustomBattle.SelectionItem;
using NavalDLC.Missions.MissionLogics;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace NavalDLC.CustomBattle.CustomBattle
{
	// Token: 0x02000013 RID: 19
	public class NavalCustomBattleMapSelectionGroupVM : ViewModel
	{
		// Token: 0x17000050 RID: 80
		// (get) Token: 0x060000C1 RID: 193 RVA: 0x000057F0 File Offset: 0x000039F0
		// (set) Token: 0x060000C2 RID: 194 RVA: 0x000057F8 File Offset: 0x000039F8
		public int SelectedTimeOfDay { get; private set; }

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x060000C3 RID: 195 RVA: 0x00005801 File Offset: 0x00003A01
		// (set) Token: 0x060000C4 RID: 196 RVA: 0x00005809 File Offset: 0x00003A09
		public float SelectedWindStrength { get; private set; }

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x060000C5 RID: 197 RVA: 0x00005812 File Offset: 0x00003A12
		// (set) Token: 0x060000C6 RID: 198 RVA: 0x0000581A File Offset: 0x00003A1A
		public NavalCustomBattleWindConfig.Direction SelectedWindDirection { get; private set; }

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x060000C7 RID: 199 RVA: 0x00005823 File Offset: 0x00003A23
		// (set) Token: 0x060000C8 RID: 200 RVA: 0x0000582B File Offset: 0x00003A2B
		public string SelectedSeasonId { get; private set; }

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x060000C9 RID: 201 RVA: 0x00005834 File Offset: 0x00003A34
		// (set) Token: 0x060000CA RID: 202 RVA: 0x0000583C File Offset: 0x00003A3C
		public NavalCustomBattleMapItemVM SelectedMap { get; private set; }

		// Token: 0x060000CB RID: 203 RVA: 0x00005848 File Offset: 0x00003A48
		public NavalCustomBattleMapSelectionGroupVM()
		{
			this._customNavalBattleMaps = new List<NavalCustomBattleMapItemVM>();
			this._customNavalRaidMaps = new List<NavalCustomBattleMapItemVM>();
			this._availableMaps = this._customNavalBattleMaps;
			this.MapSelection = new SelectorVM<NavalCustomBattleMapItemVM>(0, new Action<SelectorVM<NavalCustomBattleMapItemVM>>(this.OnMapSelection));
			this.SeasonSelection = new SelectorVM<NavalCustomBattleSeasonItemVM>(0, new Action<SelectorVM<NavalCustomBattleSeasonItemVM>>(this.OnSeasonSelection));
			this.TimeOfDaySelection = new SelectorVM<NavalCustomBattleTimeOfDayItemVM>(0, new Action<SelectorVM<NavalCustomBattleTimeOfDayItemVM>>(this.OnTimeOfDaySelection));
			this.WindStrengthSelection = new SelectorVM<NavalCustomBattleWindStrengthItemVM>(0, new Action<SelectorVM<NavalCustomBattleWindStrengthItemVM>>(this.OnWindStrengthSelection));
			this.WindDirectionSelection = new SelectorVM<NavalCustomBattleWindDirectionItemVM>(0, new Action<SelectorVM<NavalCustomBattleWindDirectionItemVM>>(this.OnWindDirectionSelection));
			this.RefreshValues();
		}

		// Token: 0x060000CC RID: 204 RVA: 0x000058FC File Offset: 0x00003AFC
		public override void RefreshValues()
		{
			base.RefreshValues();
			this._customNavalBattleMaps.Clear();
			this._customNavalRaidMaps.Clear();
			foreach (NavalCustomBattleSceneData navalCustomBattleSceneData in NavalCustomGame.Current.CustomNavalBattleScenes)
			{
				NavalCustomBattleMapItemVM navalCustomBattleMapItemVM = new NavalCustomBattleMapItemVM(navalCustomBattleSceneData.Name.ToString(), navalCustomBattleSceneData.SceneID, navalCustomBattleSceneData.Terrain, navalCustomBattleSceneData.ForcedSceneLevel);
				this._customNavalBattleMaps.Add(navalCustomBattleMapItemVM);
			}
			foreach (NavalCustomBattleSceneData navalCustomBattleSceneData2 in NavalCustomGame.Current.CustomNavalRaidScenes)
			{
				NavalCustomBattleMapItemVM navalCustomBattleMapItemVM2 = new NavalCustomBattleMapItemVM(navalCustomBattleSceneData2.Name.ToString(), navalCustomBattleSceneData2.SceneID, navalCustomBattleSceneData2.Terrain, navalCustomBattleSceneData2.ForcedSceneLevel);
				this._customNavalRaidMaps.Add(navalCustomBattleMapItemVM2);
			}
			this.TitleText = new TextObject("{=customgametitle}Map", null).ToString();
			this.MapText = new TextObject("{=customgamemapname}Map", null).ToString();
			this.SeasonText = new TextObject("{=xTzDM5XE}Season", null).ToString();
			this.TimeOfDayText = new TextObject("{=DszSWnc3}Time of Day", null).ToString();
			this.WindStrengthText = new TextObject("{=bbwr1vdO}Wind Strength", null).ToString();
			this.WindDirectionText = new TextObject("{=CFUowjPd}Wind Direction", null).ToString();
			this.MapSelection.ItemList.Clear();
			this.SeasonSelection.ItemList.Clear();
			this.TimeOfDaySelection.ItemList.Clear();
			this.WindStrengthSelection.ItemList.Clear();
			this.WindDirectionSelection.ItemList.Clear();
			foreach (NavalCustomBattleMapItemVM navalCustomBattleMapItemVM3 in this._availableMaps)
			{
				this.MapSelection.AddItem(new NavalCustomBattleMapItemVM(navalCustomBattleMapItemVM3.MapName, navalCustomBattleMapItemVM3.MapId, navalCustomBattleMapItemVM3.Terrain, navalCustomBattleMapItemVM3.ForcedSceneLevel));
			}
			foreach (Tuple<string, string> tuple in NavalCustomBattleData.Seasons)
			{
				this.SeasonSelection.AddItem(new NavalCustomBattleSeasonItemVM(tuple.Item1, tuple.Item2));
			}
			foreach (Tuple<string, NavalCustomBattleTimeOfDay> tuple2 in NavalCustomBattleData.TimesOfDay)
			{
				this.TimeOfDaySelection.AddItem(new NavalCustomBattleTimeOfDayItemVM(tuple2.Item1, (int)tuple2.Item2));
			}
			foreach (Tuple<string, float> tuple3 in NavalCustomBattleData.WindStrengths)
			{
				this.WindStrengthSelection.AddItem(new NavalCustomBattleWindStrengthItemVM(tuple3.Item1, tuple3.Item2));
			}
			foreach (Tuple<string, NavalCustomBattleWindConfig.Direction> tuple4 in NavalCustomBattleData.WindDirections)
			{
				this.WindDirectionSelection.AddItem(new NavalCustomBattleWindDirectionItemVM(tuple4.Item1, tuple4.Item2));
			}
			this.MapSelection.SelectedIndex = 0;
			this.SeasonSelection.SelectedIndex = 0;
			this.TimeOfDaySelection.SelectedIndex = 0;
			this.WindStrengthSelection.SelectedIndex = 0;
			this.WindDirectionSelection.SelectedIndex = 0;
		}

		// Token: 0x060000CD RID: 205 RVA: 0x00005CE0 File Offset: 0x00003EE0
		private void OnMapSelection(SelectorVM<NavalCustomBattleMapItemVM> selector)
		{
			this.SelectedMap = selector.SelectedItem;
		}

		// Token: 0x060000CE RID: 206 RVA: 0x00005CEE File Offset: 0x00003EEE
		private void OnSeasonSelection(SelectorVM<NavalCustomBattleSeasonItemVM> selector)
		{
			this.SelectedSeasonId = selector.SelectedItem.SeasonId;
		}

		// Token: 0x060000CF RID: 207 RVA: 0x00005D01 File Offset: 0x00003F01
		private void OnTimeOfDaySelection(SelectorVM<NavalCustomBattleTimeOfDayItemVM> selector)
		{
			this.SelectedTimeOfDay = selector.SelectedItem.TimeOfDay;
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x00005D14 File Offset: 0x00003F14
		private void OnWindStrengthSelection(SelectorVM<NavalCustomBattleWindStrengthItemVM> selector)
		{
			this.SelectedWindStrength = selector.SelectedItem.WindStrength;
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x00005D27 File Offset: 0x00003F27
		private void OnWindDirectionSelection(SelectorVM<NavalCustomBattleWindDirectionItemVM> selector)
		{
			this.SelectedWindDirection = selector.SelectedItem.WindDirection;
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x00005D3C File Offset: 0x00003F3C
		public void OnGameTypeChange(string gameTypeStringId)
		{
			if (gameTypeStringId == "NavalBattle")
			{
				this._availableMaps = this._customNavalBattleMaps;
			}
			else if (gameTypeStringId == "NavalRaid")
			{
				this._availableMaps = this._customNavalRaidMaps;
			}
			this.MapSelection.ItemList.Clear();
			foreach (NavalCustomBattleMapItemVM navalCustomBattleMapItemVM in this._availableMaps)
			{
				this.MapSelection.AddItem(navalCustomBattleMapItemVM);
			}
			this.MapSelection.SelectedIndex = 0;
			this.IsRaid = gameTypeStringId == "NavalRaid";
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x00005DF8 File Offset: 0x00003FF8
		public void RandomizeAll()
		{
			this.MapSelection.ExecuteRandomize();
			this.SeasonSelection.ExecuteRandomize();
			this.TimeOfDaySelection.ExecuteRandomize();
			this.WindStrengthSelection.ExecuteRandomize();
			this.WindDirectionSelection.ExecuteRandomize();
		}

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x060000D4 RID: 212 RVA: 0x00005E31 File Offset: 0x00004031
		// (set) Token: 0x060000D5 RID: 213 RVA: 0x00005E39 File Offset: 0x00004039
		[DataSourceProperty]
		public SelectorVM<NavalCustomBattleMapItemVM> MapSelection
		{
			get
			{
				return this._mapSelection;
			}
			set
			{
				if (value != this._mapSelection)
				{
					this._mapSelection = value;
					base.OnPropertyChangedWithValue<SelectorVM<NavalCustomBattleMapItemVM>>(value, "MapSelection");
				}
			}
		}

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x060000D6 RID: 214 RVA: 0x00005E57 File Offset: 0x00004057
		// (set) Token: 0x060000D7 RID: 215 RVA: 0x00005E5F File Offset: 0x0000405F
		[DataSourceProperty]
		public SelectorVM<NavalCustomBattleSeasonItemVM> SeasonSelection
		{
			get
			{
				return this._seasonSelection;
			}
			set
			{
				if (value != this._seasonSelection)
				{
					this._seasonSelection = value;
					base.OnPropertyChangedWithValue<SelectorVM<NavalCustomBattleSeasonItemVM>>(value, "SeasonSelection");
				}
			}
		}

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x060000D8 RID: 216 RVA: 0x00005E7D File Offset: 0x0000407D
		// (set) Token: 0x060000D9 RID: 217 RVA: 0x00005E85 File Offset: 0x00004085
		[DataSourceProperty]
		public SelectorVM<NavalCustomBattleTimeOfDayItemVM> TimeOfDaySelection
		{
			get
			{
				return this._timeOfDaySelection;
			}
			set
			{
				if (value != this._timeOfDaySelection)
				{
					this._timeOfDaySelection = value;
					base.OnPropertyChangedWithValue<SelectorVM<NavalCustomBattleTimeOfDayItemVM>>(value, "TimeOfDaySelection");
				}
			}
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x060000DA RID: 218 RVA: 0x00005EA3 File Offset: 0x000040A3
		// (set) Token: 0x060000DB RID: 219 RVA: 0x00005EAB File Offset: 0x000040AB
		[DataSourceProperty]
		public SelectorVM<NavalCustomBattleWindStrengthItemVM> WindStrengthSelection
		{
			get
			{
				return this._windStrengthSelection;
			}
			set
			{
				if (value != this._windStrengthSelection)
				{
					this._windStrengthSelection = value;
					base.OnPropertyChangedWithValue<SelectorVM<NavalCustomBattleWindStrengthItemVM>>(value, "WindStrengthSelection");
				}
			}
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x060000DC RID: 220 RVA: 0x00005EC9 File Offset: 0x000040C9
		// (set) Token: 0x060000DD RID: 221 RVA: 0x00005ED1 File Offset: 0x000040D1
		[DataSourceProperty]
		public SelectorVM<NavalCustomBattleWindDirectionItemVM> WindDirectionSelection
		{
			get
			{
				return this._windDirectionSelection;
			}
			set
			{
				if (value != this._windDirectionSelection)
				{
					this._windDirectionSelection = value;
					base.OnPropertyChangedWithValue<SelectorVM<NavalCustomBattleWindDirectionItemVM>>(value, "WindDirectionSelection");
				}
			}
		}

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x060000DE RID: 222 RVA: 0x00005EEF File Offset: 0x000040EF
		// (set) Token: 0x060000DF RID: 223 RVA: 0x00005EF7 File Offset: 0x000040F7
		[DataSourceProperty]
		public string TitleText
		{
			get
			{
				return this._titleText;
			}
			set
			{
				if (value != this._titleText)
				{
					this._titleText = value;
					base.OnPropertyChangedWithValue<string>(value, "TitleText");
				}
			}
		}

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x060000E0 RID: 224 RVA: 0x00005F1A File Offset: 0x0000411A
		// (set) Token: 0x060000E1 RID: 225 RVA: 0x00005F22 File Offset: 0x00004122
		[DataSourceProperty]
		public string MapText
		{
			get
			{
				return this._mapText;
			}
			set
			{
				if (value != this._mapText)
				{
					this._mapText = value;
					base.OnPropertyChangedWithValue<string>(value, "MapText");
				}
			}
		}

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x060000E2 RID: 226 RVA: 0x00005F45 File Offset: 0x00004145
		// (set) Token: 0x060000E3 RID: 227 RVA: 0x00005F4D File Offset: 0x0000414D
		[DataSourceProperty]
		public string SeasonText
		{
			get
			{
				return this._seasonText;
			}
			set
			{
				if (value != this._seasonText)
				{
					this._seasonText = value;
					base.OnPropertyChangedWithValue<string>(value, "SeasonText");
				}
			}
		}

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x060000E4 RID: 228 RVA: 0x00005F70 File Offset: 0x00004170
		// (set) Token: 0x060000E5 RID: 229 RVA: 0x00005F78 File Offset: 0x00004178
		[DataSourceProperty]
		public string TimeOfDayText
		{
			get
			{
				return this._timeOfDayText;
			}
			set
			{
				if (value != this._timeOfDayText)
				{
					this._timeOfDayText = value;
					base.OnPropertyChangedWithValue<string>(value, "TimeOfDayText");
				}
			}
		}

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x060000E6 RID: 230 RVA: 0x00005F9B File Offset: 0x0000419B
		// (set) Token: 0x060000E7 RID: 231 RVA: 0x00005FA3 File Offset: 0x000041A3
		[DataSourceProperty]
		public string WindStrengthText
		{
			get
			{
				return this._windStrengthText;
			}
			set
			{
				if (value != this._windStrengthText)
				{
					this._windStrengthText = value;
					base.OnPropertyChangedWithValue<string>(value, "WindStrengthText");
				}
			}
		}

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x060000E8 RID: 232 RVA: 0x00005FC6 File Offset: 0x000041C6
		// (set) Token: 0x060000E9 RID: 233 RVA: 0x00005FCE File Offset: 0x000041CE
		[DataSourceProperty]
		public string WindDirectionText
		{
			get
			{
				return this._windDirectionText;
			}
			set
			{
				if (value != this._windDirectionText)
				{
					this._windDirectionText = value;
					base.OnPropertyChangedWithValue<string>(value, "WindDirectionText");
				}
			}
		}

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x060000EA RID: 234 RVA: 0x00005FF1 File Offset: 0x000041F1
		// (set) Token: 0x060000EB RID: 235 RVA: 0x00005FF9 File Offset: 0x000041F9
		[DataSourceProperty]
		public bool IsRaid
		{
			get
			{
				return this._isRaid;
			}
			set
			{
				if (value != this._isRaid)
				{
					this._isRaid = value;
					base.OnPropertyChangedWithValue(value, "IsRaid");
				}
			}
		}

		// Token: 0x04000076 RID: 118
		private List<NavalCustomBattleMapItemVM> _customNavalBattleMaps;

		// Token: 0x04000077 RID: 119
		private List<NavalCustomBattleMapItemVM> _customNavalRaidMaps;

		// Token: 0x04000078 RID: 120
		private List<NavalCustomBattleMapItemVM> _availableMaps;

		// Token: 0x04000079 RID: 121
		private SelectorVM<NavalCustomBattleMapItemVM> _mapSelection;

		// Token: 0x0400007A RID: 122
		private SelectorVM<NavalCustomBattleSeasonItemVM> _seasonSelection;

		// Token: 0x0400007B RID: 123
		private SelectorVM<NavalCustomBattleTimeOfDayItemVM> _timeOfDaySelection;

		// Token: 0x0400007C RID: 124
		private SelectorVM<NavalCustomBattleWindStrengthItemVM> _windStrengthSelection;

		// Token: 0x0400007D RID: 125
		private SelectorVM<NavalCustomBattleWindDirectionItemVM> _windDirectionSelection;

		// Token: 0x0400007E RID: 126
		private string _titleText;

		// Token: 0x0400007F RID: 127
		private string _mapText;

		// Token: 0x04000080 RID: 128
		private string _seasonText;

		// Token: 0x04000081 RID: 129
		private string _timeOfDayText;

		// Token: 0x04000082 RID: 130
		private string _windStrengthText;

		// Token: 0x04000083 RID: 131
		private string _windDirectionText;

		// Token: 0x04000084 RID: 132
		private bool _isRaid;
	}
}
