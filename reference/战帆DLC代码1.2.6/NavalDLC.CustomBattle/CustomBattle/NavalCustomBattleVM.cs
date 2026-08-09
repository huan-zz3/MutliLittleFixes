using System;
using System.Collections.Generic;
using System.Linq;
using NavalDLC.CustomBattle.CustomBattle.SelectionItem;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.CustomBattle;
using TaleWorlds.MountAndBlade.ViewModelCollection.Input;

namespace NavalDLC.CustomBattle.CustomBattle
{
	// Token: 0x0200001B RID: 27
	public class NavalCustomBattleVM : ViewModel
	{
		// Token: 0x06000196 RID: 406 RVA: 0x0000800C File Offset: 0x0000620C
		public NavalCustomBattleVM()
		{
			this.TroopTypeSelectionPopUp = new NavalCustomBattleTroopTypeSelectionPopUpVM();
			this.ShipSelectionPopUp = new NavalCustomBattleShipSelectionPopUpVM();
			this.PlayerSide = new NavalCustomBattleSideVM(new TextObject("{=BC7n6qxk}PLAYER", null), true, this.TroopTypeSelectionPopUp, this.ShipSelectionPopUp, new Action<NavalCustomBattleShipItemVM>(this.OnShipFocused), new Action(this.UpdateCanConfirm), new Action(this.OnSelectedCharactersChanged));
			this.EnemySide = new NavalCustomBattleSideVM(new TextObject("{=35IHscBa}ENEMY", null), false, this.TroopTypeSelectionPopUp, this.ShipSelectionPopUp, new Action<NavalCustomBattleShipItemVM>(this.OnShipFocused), new Action(this.UpdateCanConfirm), new Action(this.OnSelectedCharactersChanged));
			this.OnSelectedCharactersChanged();
			this.MapSelectionGroup = new NavalCustomBattleMapSelectionGroupVM();
			this.GameTypeSelectionGroup = new NavalCustomBattleGameTypeSelectionGroupVM(new Action<string>(this.OnGameTypeChange), new Action(this.UpdateIsLandSide));
			this.CanSwitchMode = CustomBattleFactory.GetProviderCount() > 1;
			if (this.CanSwitchMode)
			{
				this._nextCustomBattleProvider = CustomBattleFactory.CollectNextProvider(typeof(NavalCustomBattleProvider));
				this.SwitchHint = new HintViewModel(new TextObject("{=Jfe53wbr}Switch to {PROVIDER_NAME}", null).SetTextVariable("PROVIDER_NAME", this._nextCustomBattleProvider.GetName()), null);
			}
			this.ConfirmHint = new HintViewModel();
			this.UpdateCanConfirm();
			this.RefreshValues();
		}

		// Token: 0x06000197 RID: 407 RVA: 0x00008165 File Offset: 0x00006365
		private static NavalCustomBattleCompositionData GetBattleCompositionDataFromCompositionGroup(NavalCustomBattleArmyCompositionGroupVM compositionGroup)
		{
			return new NavalCustomBattleCompositionData((float)compositionGroup.RangedInfantryComposition.CompositionValue / 100f, (float)compositionGroup.MeleeCavalryComposition.CompositionValue / 100f, (float)compositionGroup.RangedCavalryComposition.CompositionValue / 100f);
		}

		// Token: 0x06000198 RID: 408 RVA: 0x000081A4 File Offset: 0x000063A4
		private static List<BasicCharacterObject>[] GetTroopSelections(NavalCustomBattleArmyCompositionGroupVM armyComposition)
		{
			List<BasicCharacterObject>[] array = new List<BasicCharacterObject>[4];
			array[0] = (from x in armyComposition.MeleeInfantryComposition.TroopTypes
				where x.IsSelected
				select x.Character).ToList<BasicCharacterObject>();
			array[1] = (from x in armyComposition.RangedInfantryComposition.TroopTypes
				where x.IsSelected
				select x.Character).ToList<BasicCharacterObject>();
			array[2] = (from x in armyComposition.MeleeCavalryComposition.TroopTypes
				where x.IsSelected
				select x.Character).ToList<BasicCharacterObject>();
			array[3] = (from x in armyComposition.RangedCavalryComposition.TroopTypes
				where x.IsSelected
				select x.Character).ToList<BasicCharacterObject>();
			return array;
		}

		// Token: 0x06000199 RID: 409 RVA: 0x00008323 File Offset: 0x00006523
		public void SetActiveState(bool isActive)
		{
			if (isActive)
			{
				this.EnemySide.UpdateCharacterVisual();
				this.PlayerSide.UpdateCharacterVisual();
				return;
			}
			this.EnemySide.CurrentSelectedCharacter = null;
			this.PlayerSide.CurrentSelectedCharacter = null;
		}

		// Token: 0x0600019A RID: 410 RVA: 0x00008358 File Offset: 0x00006558
		private void OnSelectedCharactersChanged()
		{
			NavalCustomBattleSideVM playerSide = this.PlayerSide;
			if (((playerSide != null) ? playerSide.CharacterSelectionGroup : null) != null)
			{
				NavalCustomBattleSideVM enemySide = this.EnemySide;
				if (((enemySide != null) ? enemySide.CharacterSelectionGroup : null) != null)
				{
					NavalCustomBattleCharacterItemVM selectedItem = this.PlayerSide.CharacterSelectionGroup.SelectedItem;
					BasicCharacterObject basicCharacterObject = ((selectedItem != null) ? selectedItem.Character : null);
					NavalCustomBattleCharacterItemVM selectedItem2 = this.EnemySide.CharacterSelectionGroup.SelectedItem;
					BasicCharacterObject basicCharacterObject2 = ((selectedItem2 != null) ? selectedItem2.Character : null);
					foreach (NavalCustomBattleCharacterItemVM navalCustomBattleCharacterItemVM in this.PlayerSide.CharacterSelectionGroup.ItemList)
					{
						navalCustomBattleCharacterItemVM.CanBeSelected = navalCustomBattleCharacterItemVM.Character != basicCharacterObject2;
					}
					foreach (NavalCustomBattleCharacterItemVM navalCustomBattleCharacterItemVM2 in this.EnemySide.CharacterSelectionGroup.ItemList)
					{
						navalCustomBattleCharacterItemVM2.CanBeSelected = navalCustomBattleCharacterItemVM2.Character != basicCharacterObject;
					}
				}
			}
		}

		// Token: 0x0600019B RID: 411 RVA: 0x00008470 File Offset: 0x00006670
		private void OnGameTypeChange(string gameTypeStringId)
		{
			this.MapSelectionGroup.OnGameTypeChange(gameTypeStringId);
			this.UpdateIsLandSide();
			NavalCustomBattleSideVM playerSide = this.PlayerSide;
			if (playerSide != null)
			{
				playerSide.OnGameTypeChange(gameTypeStringId);
			}
			NavalCustomBattleSideVM enemySide = this.EnemySide;
			if (enemySide != null)
			{
				enemySide.OnGameTypeChange(gameTypeStringId);
			}
			this.UpdateCanConfirm();
		}

		// Token: 0x0600019C RID: 412 RVA: 0x000084B0 File Offset: 0x000066B0
		private void UpdateIsLandSide()
		{
			if (this.PlayerSide == null || this.EnemySide == null || this.GameTypeSelectionGroup == null)
			{
				return;
			}
			if (this.GameTypeSelectionGroup.SelectedGameTypeStringId == "NavalRaid")
			{
				this.PlayerSide.IsLandSide = this.GameTypeSelectionGroup.SelectedPlayerSide == NavalCustomBattlePlayerSide.Defender;
				this.EnemySide.IsLandSide = this.GameTypeSelectionGroup.SelectedPlayerSide == NavalCustomBattlePlayerSide.Attacker;
				return;
			}
			this.PlayerSide.IsLandSide = false;
			this.EnemySide.IsLandSide = false;
		}

		// Token: 0x0600019D RID: 413 RVA: 0x00008538 File Offset: 0x00006738
		private void UpdateCanConfirm()
		{
			if (this.PlayerSide == null || this.EnemySide == null || this.GameTypeSelectionGroup == null)
			{
				return;
			}
			List<string> list = new List<string>();
			if (this.GameTypeSelectionGroup.SelectedGameTypeStringId == "NavalRaid")
			{
				bool flag;
				if (!this.PlayerSide.IsLandSide)
				{
					if (!this.PlayerSide.ShipSelectionGroup.ShipSelectionItems.All<NavalCustomBattleShipSelectionItemVM>((NavalCustomBattleShipSelectionItemVM x) => !x.IsRelevant || !x.HasSelectedItem || x.IsSelectedItemEligible))
					{
						flag = false;
						goto IL_00C4;
					}
				}
				if (!this.EnemySide.IsLandSide)
				{
					flag = this.EnemySide.ShipSelectionGroup.ShipSelectionItems.All<NavalCustomBattleShipSelectionItemVM>((NavalCustomBattleShipSelectionItemVM x) => !x.IsRelevant || !x.HasSelectedItem || x.IsSelectedItemEligible);
				}
				else
				{
					flag = true;
				}
				IL_00C4:
				this.CanConfirm = flag;
				if (!this.CanConfirm)
				{
					if (!this.PlayerSide.IsLandSide)
					{
						list.AddRange(from x in this.PlayerSide.ShipSelectionGroup.ShipSelectionItems
							where x.IsRelevant && x.HasSelectedItem && !x.IsSelectedItemEligible
							select x.SelectedItem.Name);
					}
					if (!this.EnemySide.IsLandSide)
					{
						list.AddRange(from x in this.EnemySide.ShipSelectionGroup.ShipSelectionItems
							where x.IsRelevant && x.HasSelectedItem && !x.IsSelectedItemEligible
							select x.SelectedItem.Name);
					}
					list = list.Distinct<string>().ToList<string>();
				}
			}
			else
			{
				this.CanConfirm = true;
			}
			this.ConfirmHint.HintText = (this.CanConfirm ? null : new TextObject("{=MC7KdXJm}Following ship types are not eligible for the selected game mode: {INELIGIBLE_SHIPS}", null).SetTextVariable("INELIGIBLE_SHIPS", string.Join(", ", list)));
		}

		// Token: 0x0600019E RID: 414 RVA: 0x0000873C File Offset: 0x0000693C
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.RandomizeButtonText = GameTexts.FindText("str_randomize", null).ToString();
			this.StartButtonText = GameTexts.FindText("str_start", null).ToString();
			this.BackButtonText = GameTexts.FindText("str_back", null).ToString();
			this.SwitchButtonText = GameTexts.FindText("str_switch", null).ToString();
			this.TitleText = GameTexts.FindText("str_naval_custom_battle", null).ToString();
			this.EnemySide.RefreshValues();
			this.PlayerSide.RefreshValues();
			this.MapSelectionGroup.RefreshValues();
			this.GameTypeSelectionGroup.RefreshValues();
			NavalCustomBattleTroopTypeSelectionPopUpVM troopTypeSelectionPopUp = this.TroopTypeSelectionPopUp;
			if (troopTypeSelectionPopUp != null)
			{
				troopTypeSelectionPopUp.RefreshValues();
			}
			NavalCustomBattleShipSelectionPopUpVM shipSelectionPopUp = this.ShipSelectionPopUp;
			if (shipSelectionPopUp == null)
			{
				return;
			}
			shipSelectionPopUp.RefreshValues();
		}

		// Token: 0x0600019F RID: 415 RVA: 0x0000880A File Offset: 0x00006A0A
		public void ExecuteBack()
		{
			Game.Current.GameStateManager.PopState(0);
		}

		// Token: 0x060001A0 RID: 416 RVA: 0x0000881C File Offset: 0x00006A1C
		private NavalCustomBattleData PrepareBattleData()
		{
			BasicCharacterObject selectedCharacter = this.PlayerSide.SelectedCharacter;
			BasicCharacterObject selectedCharacter2 = this.EnemySide.SelectedCharacter;
			int armySize = this.PlayerSide.CompositionGroup.ArmySize;
			int armySize2 = this.EnemySide.CompositionGroup.ArmySize;
			bool flag = this.GameTypeSelectionGroup.SelectedPlayerSide == NavalCustomBattlePlayerSide.Attacker;
			BasicCultureObject faction = this.PlayerSide.FactionSelectionGroup.SelectedItem.Faction;
			BasicCultureObject faction2 = this.EnemySide.FactionSelectionGroup.SelectedItem.Faction;
			List<IShipOrigin>[] customBattleShipLists = NavalCustomBattleHelper.GetCustomBattleShipLists(this.PlayerSide.IsLandSide ? new List<IShipOrigin>() : this.PlayerSide.ShipSelectionGroup.GetSelectedShips(), this.EnemySide.IsLandSide ? new List<IShipOrigin>() : this.EnemySide.ShipSelectionGroup.GetSelectedShips());
			int num = (this.PlayerSide.IsLandSide ? 1 : customBattleShipLists[0].Count);
			int num2 = (this.EnemySide.IsLandSide ? 1 : customBattleShipLists[1].Count);
			int[] troopCounts = NavalCustomBattleHelper.GetTroopCounts(armySize, num, NavalCustomBattleVM.GetBattleCompositionDataFromCompositionGroup(this.PlayerSide.CompositionGroup));
			int[] troopCounts2 = NavalCustomBattleHelper.GetTroopCounts(armySize2, num2, NavalCustomBattleVM.GetBattleCompositionDataFromCompositionGroup(this.EnemySide.CompositionGroup));
			List<BasicCharacterObject>[] troopSelections = NavalCustomBattleVM.GetTroopSelections(this.PlayerSide.CompositionGroup);
			List<BasicCharacterObject>[] troopSelections2 = NavalCustomBattleVM.GetTroopSelections(this.EnemySide.CompositionGroup);
			List<BasicCharacterObject> list = new List<BasicCharacterObject>();
			foreach (BasicCharacterObject basicCharacterObject in NavalCustomBattleData.Characters)
			{
				if (basicCharacterObject != selectedCharacter && basicCharacterObject != selectedCharacter2)
				{
					list.Add(basicCharacterObject);
				}
			}
			CustomBattleCombatant[] customBattleParties = NavalCustomBattleHelper.GetCustomBattleParties(selectedCharacter, selectedCharacter2, list, faction, troopCounts, troopSelections, num, faction2, troopCounts2, troopSelections2, num2, flag);
			BasicCharacterObject basicCharacterObject2 = selectedCharacter;
			CustomBattleCombatant customBattleCombatant = customBattleParties[0];
			List<IShipOrigin> list2 = customBattleShipLists[0];
			CustomBattleCombatant customBattleCombatant2 = customBattleParties[1];
			List<IShipOrigin> list3 = customBattleShipLists[1];
			string selectedGameTypeStringId = this.GameTypeSelectionGroup.SelectedGameTypeStringId;
			NavalCustomBattleMapItemVM selectedMap = this.MapSelectionGroup.SelectedMap;
			return NavalCustomBattleHelper.PrepareBattleData(basicCharacterObject2, customBattleCombatant, list2, customBattleCombatant2, list3, selectedGameTypeStringId, (selectedMap != null) ? selectedMap.MapId : null, this.MapSelectionGroup.SelectedSeasonId, (float)this.MapSelectionGroup.SelectedTimeOfDay, this.MapSelectionGroup.SelectedWindStrength, this.MapSelectionGroup.SelectedWindDirection, this.MapSelectionGroup.SelectedMap.Terrain, this.MapSelectionGroup.SelectedMap.ForcedSceneLevel);
		}

		// Token: 0x060001A1 RID: 417 RVA: 0x00008A74 File Offset: 0x00006C74
		public void ExecuteStart()
		{
			if (this.CanConfirm)
			{
				NavalCustomBattleHelper.StartGame(this.PrepareBattleData());
			}
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x00008A8C File Offset: 0x00006C8C
		public void ExecuteRandomize()
		{
			int num = MBRandom.RandomInt(40, 500);
			this.MapSelectionGroup.RandomizeAll();
			this.GameTypeSelectionGroup.RandomizeAll();
			this.PlayerSide.Randomize(num);
			this.EnemySide.Randomize(num);
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x00008AD4 File Offset: 0x00006CD4
		public override void OnFinalize()
		{
			base.OnFinalize();
			this.StartInputKey.OnFinalize();
			this.CancelInputKey.OnFinalize();
			this.ResetInputKey.OnFinalize();
			this.RandomizeInputKey.OnFinalize();
			NavalCustomBattleTroopTypeSelectionPopUpVM troopTypeSelectionPopUp = this.TroopTypeSelectionPopUp;
			if (troopTypeSelectionPopUp != null)
			{
				troopTypeSelectionPopUp.OnFinalize();
			}
			NavalCustomBattleShipSelectionPopUpVM shipSelectionPopUp = this.ShipSelectionPopUp;
			if (shipSelectionPopUp == null)
			{
				return;
			}
			shipSelectionPopUp.OnFinalize();
		}

		// Token: 0x060001A4 RID: 420 RVA: 0x00008B34 File Offset: 0x00006D34
		public void ExecuteSwitchToNextCustomBattle()
		{
			if (this.CanSwitchMode)
			{
				this.ExecuteBack();
				GameStateManager.Current = Module.CurrentModule.GlobalGameStateManager;
				this._nextCustomBattleProvider.StartCustomBattle();
			}
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x00008B5E File Offset: 0x00006D5E
		private void OnShipFocused(NavalCustomBattleShipItemVM focusedItem)
		{
			this.FocusedShipItem = focusedItem;
		}

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x060001A6 RID: 422 RVA: 0x00008B67 File Offset: 0x00006D67
		// (set) Token: 0x060001A7 RID: 423 RVA: 0x00008B6F File Offset: 0x00006D6F
		[DataSourceProperty]
		public NavalCustomBattleTroopTypeSelectionPopUpVM TroopTypeSelectionPopUp
		{
			get
			{
				return this._troopTypeSelectionPopUp;
			}
			set
			{
				if (value != this._troopTypeSelectionPopUp)
				{
					this._troopTypeSelectionPopUp = value;
					base.OnPropertyChangedWithValue<NavalCustomBattleTroopTypeSelectionPopUpVM>(value, "TroopTypeSelectionPopUp");
				}
			}
		}

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x060001A8 RID: 424 RVA: 0x00008B8D File Offset: 0x00006D8D
		// (set) Token: 0x060001A9 RID: 425 RVA: 0x00008B95 File Offset: 0x00006D95
		[DataSourceProperty]
		public NavalCustomBattleShipSelectionPopUpVM ShipSelectionPopUp
		{
			get
			{
				return this._shipSelectionPopUp;
			}
			set
			{
				if (value != this._shipSelectionPopUp)
				{
					this._shipSelectionPopUp = value;
					base.OnPropertyChangedWithValue<NavalCustomBattleShipSelectionPopUpVM>(value, "ShipSelectionPopUp");
				}
			}
		}

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x060001AA RID: 426 RVA: 0x00008BB3 File Offset: 0x00006DB3
		// (set) Token: 0x060001AB RID: 427 RVA: 0x00008BBB File Offset: 0x00006DBB
		[DataSourceProperty]
		public string RandomizeButtonText
		{
			get
			{
				return this._randomizeButtonText;
			}
			set
			{
				if (value != this._randomizeButtonText)
				{
					this._randomizeButtonText = value;
					base.OnPropertyChangedWithValue<string>(value, "RandomizeButtonText");
				}
			}
		}

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x060001AC RID: 428 RVA: 0x00008BDE File Offset: 0x00006DDE
		// (set) Token: 0x060001AD RID: 429 RVA: 0x00008BE6 File Offset: 0x00006DE6
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

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x060001AE RID: 430 RVA: 0x00008C09 File Offset: 0x00006E09
		// (set) Token: 0x060001AF RID: 431 RVA: 0x00008C11 File Offset: 0x00006E11
		[DataSourceProperty]
		public string BackButtonText
		{
			get
			{
				return this._backButtonText;
			}
			set
			{
				if (value != this._backButtonText)
				{
					this._backButtonText = value;
					base.OnPropertyChangedWithValue<string>(value, "BackButtonText");
				}
			}
		}

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x060001B0 RID: 432 RVA: 0x00008C34 File Offset: 0x00006E34
		// (set) Token: 0x060001B1 RID: 433 RVA: 0x00008C3C File Offset: 0x00006E3C
		[DataSourceProperty]
		public string StartButtonText
		{
			get
			{
				return this._startButtonText;
			}
			set
			{
				if (value != this._startButtonText)
				{
					this._startButtonText = value;
					base.OnPropertyChangedWithValue<string>(value, "StartButtonText");
				}
			}
		}

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x060001B2 RID: 434 RVA: 0x00008C5F File Offset: 0x00006E5F
		// (set) Token: 0x060001B3 RID: 435 RVA: 0x00008C67 File Offset: 0x00006E67
		[DataSourceProperty]
		public string SwitchButtonText
		{
			get
			{
				return this._switchButtonText;
			}
			set
			{
				if (value != this._switchButtonText)
				{
					this._switchButtonText = value;
					base.OnPropertyChangedWithValue<string>(value, "SwitchButtonText");
				}
			}
		}

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x060001B4 RID: 436 RVA: 0x00008C8A File Offset: 0x00006E8A
		// (set) Token: 0x060001B5 RID: 437 RVA: 0x00008C92 File Offset: 0x00006E92
		[DataSourceProperty]
		public NavalCustomBattleSideVM EnemySide
		{
			get
			{
				return this._enemySide;
			}
			set
			{
				if (value != this._enemySide)
				{
					this._enemySide = value;
					base.OnPropertyChangedWithValue<NavalCustomBattleSideVM>(value, "EnemySide");
				}
			}
		}

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x060001B6 RID: 438 RVA: 0x00008CB0 File Offset: 0x00006EB0
		// (set) Token: 0x060001B7 RID: 439 RVA: 0x00008CB8 File Offset: 0x00006EB8
		[DataSourceProperty]
		public NavalCustomBattleSideVM PlayerSide
		{
			get
			{
				return this._playerSide;
			}
			set
			{
				if (value != this._playerSide)
				{
					this._playerSide = value;
					base.OnPropertyChangedWithValue<NavalCustomBattleSideVM>(value, "PlayerSide");
				}
			}
		}

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x060001B8 RID: 440 RVA: 0x00008CD6 File Offset: 0x00006ED6
		// (set) Token: 0x060001B9 RID: 441 RVA: 0x00008CDE File Offset: 0x00006EDE
		[DataSourceProperty]
		public NavalCustomBattleMapSelectionGroupVM MapSelectionGroup
		{
			get
			{
				return this._mapSelectionGroup;
			}
			set
			{
				if (value != this._mapSelectionGroup)
				{
					this._mapSelectionGroup = value;
					base.OnPropertyChangedWithValue<NavalCustomBattleMapSelectionGroupVM>(value, "MapSelectionGroup");
				}
			}
		}

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x060001BA RID: 442 RVA: 0x00008CFC File Offset: 0x00006EFC
		// (set) Token: 0x060001BB RID: 443 RVA: 0x00008D04 File Offset: 0x00006F04
		[DataSourceProperty]
		public NavalCustomBattleGameTypeSelectionGroupVM GameTypeSelectionGroup
		{
			get
			{
				return this._gameTypeSelectionGroup;
			}
			set
			{
				if (value != this._gameTypeSelectionGroup)
				{
					this._gameTypeSelectionGroup = value;
					base.OnPropertyChangedWithValue<NavalCustomBattleGameTypeSelectionGroupVM>(value, "GameTypeSelectionGroup");
				}
			}
		}

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x060001BC RID: 444 RVA: 0x00008D22 File Offset: 0x00006F22
		// (set) Token: 0x060001BD RID: 445 RVA: 0x00008D2A File Offset: 0x00006F2A
		[DataSourceProperty]
		public bool CanConfirm
		{
			get
			{
				return this._canConfirm;
			}
			set
			{
				if (value != this._canConfirm)
				{
					this._canConfirm = value;
					base.OnPropertyChangedWithValue(value, "CanConfirm");
				}
			}
		}

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x060001BE RID: 446 RVA: 0x00008D48 File Offset: 0x00006F48
		// (set) Token: 0x060001BF RID: 447 RVA: 0x00008D50 File Offset: 0x00006F50
		[DataSourceProperty]
		public HintViewModel ConfirmHint
		{
			get
			{
				return this._confirmHint;
			}
			set
			{
				if (value != this._confirmHint)
				{
					this._confirmHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ConfirmHint");
				}
			}
		}

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x060001C0 RID: 448 RVA: 0x00008D6E File Offset: 0x00006F6E
		// (set) Token: 0x060001C1 RID: 449 RVA: 0x00008D76 File Offset: 0x00006F76
		[DataSourceProperty]
		public bool CanSwitchMode
		{
			get
			{
				return this._canSwitchMode;
			}
			set
			{
				if (value != this._canSwitchMode)
				{
					this._canSwitchMode = value;
					base.OnPropertyChangedWithValue(value, "CanSwitchMode");
				}
			}
		}

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x060001C2 RID: 450 RVA: 0x00008D94 File Offset: 0x00006F94
		// (set) Token: 0x060001C3 RID: 451 RVA: 0x00008D9C File Offset: 0x00006F9C
		[DataSourceProperty]
		public HintViewModel SwitchHint
		{
			get
			{
				return this._switchHint;
			}
			set
			{
				if (value != this._switchHint)
				{
					this._switchHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "SwitchHint");
				}
			}
		}

		// Token: 0x060001C4 RID: 452 RVA: 0x00008DBA File Offset: 0x00006FBA
		public void SetStartInputKey(HotKey hotkey)
		{
			this.StartInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x00008DC9 File Offset: 0x00006FC9
		public void SetCancelInputKey(HotKey hotkey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
			NavalCustomBattleTroopTypeSelectionPopUpVM troopTypeSelectionPopUp = this.TroopTypeSelectionPopUp;
			if (troopTypeSelectionPopUp != null)
			{
				troopTypeSelectionPopUp.SetCancelInputKey(hotkey);
			}
			NavalCustomBattleShipSelectionPopUpVM shipSelectionPopUp = this.ShipSelectionPopUp;
			if (shipSelectionPopUp == null)
			{
				return;
			}
			shipSelectionPopUp.SetCloseInputKey(hotkey);
		}

		// Token: 0x060001C6 RID: 454 RVA: 0x00008DFB File Offset: 0x00006FFB
		public void SetResetInputKey(HotKey hotkey)
		{
			this.ResetInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
			NavalCustomBattleTroopTypeSelectionPopUpVM troopTypeSelectionPopUp = this.TroopTypeSelectionPopUp;
			if (troopTypeSelectionPopUp == null)
			{
				return;
			}
			troopTypeSelectionPopUp.SetResetInputKey(hotkey);
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x00008E1B File Offset: 0x0000701B
		public void SetRandomizeInputKey(HotKey hotkey)
		{
			this.RandomizeInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x00008E2A File Offset: 0x0000702A
		public void SetCycleTierInputKey(HotKey hotkey)
		{
			this.PlayerSide.SetCycleTierInputKey(hotkey);
			this.EnemySide.SetCycleTierInputKey(hotkey);
		}

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x060001C9 RID: 457 RVA: 0x00008E44 File Offset: 0x00007044
		// (set) Token: 0x060001CA RID: 458 RVA: 0x00008E4C File Offset: 0x0000704C
		public InputKeyItemVM StartInputKey
		{
			get
			{
				return this._startInputKey;
			}
			set
			{
				if (value != this._startInputKey)
				{
					this._startInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "StartInputKey");
				}
			}
		}

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x060001CB RID: 459 RVA: 0x00008E6A File Offset: 0x0000706A
		// (set) Token: 0x060001CC RID: 460 RVA: 0x00008E72 File Offset: 0x00007072
		public InputKeyItemVM CancelInputKey
		{
			get
			{
				return this._cancelInputKey;
			}
			set
			{
				if (value != this._cancelInputKey)
				{
					this._cancelInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "CancelInputKey");
				}
			}
		}

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x060001CD RID: 461 RVA: 0x00008E90 File Offset: 0x00007090
		// (set) Token: 0x060001CE RID: 462 RVA: 0x00008E98 File Offset: 0x00007098
		public InputKeyItemVM ResetInputKey
		{
			get
			{
				return this._resetInputKey;
			}
			set
			{
				if (value != this._resetInputKey)
				{
					this._resetInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "ResetInputKey");
				}
			}
		}

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x060001CF RID: 463 RVA: 0x00008EB6 File Offset: 0x000070B6
		// (set) Token: 0x060001D0 RID: 464 RVA: 0x00008EBE File Offset: 0x000070BE
		public InputKeyItemVM RandomizeInputKey
		{
			get
			{
				return this._randomizeInputKey;
			}
			set
			{
				if (value != this._randomizeInputKey)
				{
					this._randomizeInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "RandomizeInputKey");
				}
			}
		}

		// Token: 0x040000CB RID: 203
		public NavalCustomBattleShipItemVM FocusedShipItem;

		// Token: 0x040000CC RID: 204
		private readonly ICustomBattleProvider _nextCustomBattleProvider;

		// Token: 0x040000CD RID: 205
		private NavalCustomBattleTroopTypeSelectionPopUpVM _troopTypeSelectionPopUp;

		// Token: 0x040000CE RID: 206
		private NavalCustomBattleShipSelectionPopUpVM _shipSelectionPopUp;

		// Token: 0x040000CF RID: 207
		private NavalCustomBattleSideVM _enemySide;

		// Token: 0x040000D0 RID: 208
		private NavalCustomBattleSideVM _playerSide;

		// Token: 0x040000D1 RID: 209
		private NavalCustomBattleMapSelectionGroupVM _mapSelectionGroup;

		// Token: 0x040000D2 RID: 210
		private NavalCustomBattleGameTypeSelectionGroupVM _gameTypeSelectionGroup;

		// Token: 0x040000D3 RID: 211
		private string _randomizeButtonText;

		// Token: 0x040000D4 RID: 212
		private string _backButtonText;

		// Token: 0x040000D5 RID: 213
		private string _startButtonText;

		// Token: 0x040000D6 RID: 214
		private string _switchButtonText;

		// Token: 0x040000D7 RID: 215
		private string _titleText;

		// Token: 0x040000D8 RID: 216
		private bool _canConfirm;

		// Token: 0x040000D9 RID: 217
		private HintViewModel _confirmHint;

		// Token: 0x040000DA RID: 218
		private bool _canSwitchMode;

		// Token: 0x040000DB RID: 219
		private HintViewModel _switchHint;

		// Token: 0x040000DC RID: 220
		private InputKeyItemVM _startInputKey;

		// Token: 0x040000DD RID: 221
		private InputKeyItemVM _cancelInputKey;

		// Token: 0x040000DE RID: 222
		private InputKeyItemVM _resetInputKey;

		// Token: 0x040000DF RID: 223
		private InputKeyItemVM _randomizeInputKey;
	}
}
