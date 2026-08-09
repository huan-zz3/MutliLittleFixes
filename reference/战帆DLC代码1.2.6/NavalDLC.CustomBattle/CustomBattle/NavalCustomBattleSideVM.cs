using System;
using System.Collections.Generic;
using System.Linq;
using NavalDLC.CustomBattle.CustomBattle.SelectionItem;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.CustomBattle.CustomBattle
{
	// Token: 0x02000018 RID: 24
	public class NavalCustomBattleSideVM : ViewModel
	{
		// Token: 0x17000073 RID: 115
		// (get) Token: 0x06000138 RID: 312 RVA: 0x00006EE9 File Offset: 0x000050E9
		// (set) Token: 0x06000139 RID: 313 RVA: 0x00006EF1 File Offset: 0x000050F1
		public BasicCharacterObject SelectedCharacter { get; private set; }

		// Token: 0x0600013A RID: 314 RVA: 0x00006EFC File Offset: 0x000050FC
		public NavalCustomBattleSideVM(TextObject sideName, bool isPlayerSide, NavalCustomBattleTroopTypeSelectionPopUpVM troopTypeSelectionPopUp, NavalCustomBattleShipSelectionPopUpVM shipSelectionPopUp, Action<NavalCustomBattleShipItemVM> onShipFocused, Action onShipSelected, Action onCharacterSelected)
		{
			this._sideName = sideName;
			this._isPlayerSide = isPlayerSide;
			this._onCharacterSelected = onCharacterSelected;
			this._onShipSelected = onShipSelected;
			this.CompositionGroup = new NavalCustomBattleArmyCompositionGroupVM(troopTypeSelectionPopUp);
			this.FactionSelectionGroup = new NavalCustomBattleFactionSelectionVM(new Action<BasicCultureObject>(this.OnCultureSelection));
			this.CharacterSelectionGroup = new SelectorVM<NavalCustomBattleCharacterItemVM>(0, new Action<SelectorVM<NavalCustomBattleCharacterItemVM>>(this.OnCharacterSelection));
			this.ShipSelectionGroup = new NavalCustomBattleShipSelectionGroupVM(this._isPlayerSide, shipSelectionPopUp, new Action(this.OnShipSelectedOrUpgraded), onShipFocused);
			this.ArmorsList = new MBBindingList<CharacterEquipmentItemVM>();
			this.WeaponsList = new MBBindingList<CharacterEquipmentItemVM>();
			this.UpdateTroopCountLimits();
			this.RefreshValues();
		}

		// Token: 0x0600013B RID: 315 RVA: 0x00006FAC File Offset: 0x000051AC
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Name = this._sideName.ToString();
			this.FactionText = GameTexts.FindText("str_faction", null).ToString();
			if (this._isPlayerSide)
			{
				this.TitleText = new TextObject("{=bLXleed8}Player Character", null).ToString();
			}
			else
			{
				this.TitleText = new TextObject("{=QAYngoNQ}Enemy Character", null).ToString();
			}
			this.CharacterSelectionGroup.ItemList.Clear();
			foreach (BasicCharacterObject basicCharacterObject in NavalCustomBattleData.Characters)
			{
				this.CharacterSelectionGroup.AddItem(new NavalCustomBattleCharacterItemVM(basicCharacterObject));
			}
			this.CharacterSelectionGroup.SelectedIndex = (this._isPlayerSide ? 0 : 1);
			this.UpdateCharacterVisual();
			Action onCharacterSelected = this._onCharacterSelected;
			if (onCharacterSelected != null)
			{
				onCharacterSelected();
			}
			this.CompositionGroup.RefreshValues();
			this.CharacterSelectionGroup.RefreshValues();
			this.FactionSelectionGroup.RefreshValues();
			this.ShipSelectionGroup.RefreshValues();
		}

		// Token: 0x0600013C RID: 316 RVA: 0x000070D0 File Offset: 0x000052D0
		private void OnShipSelectedOrUpgraded()
		{
			Action onShipSelected = this._onShipSelected;
			if (onShipSelected != null)
			{
				onShipSelected();
			}
			this.UpdateTroopCountLimits();
		}

		// Token: 0x0600013D RID: 317 RVA: 0x000070E9 File Offset: 0x000052E9
		public void OnGameTypeChange(string gameTypeStringId)
		{
			this.IsRaid = gameTypeStringId == "NavalRaid";
		}

		// Token: 0x0600013E RID: 318 RVA: 0x000070FC File Offset: 0x000052FC
		private void UpdateTroopCountLimits()
		{
			if (this.ShipSelectionGroup != null && this.CompositionGroup != null)
			{
				List<IShipOrigin> selectedShips = this.ShipSelectionGroup.GetSelectedShips();
				int num = (this.IsLandSide ? 1 : (selectedShips.Count<IShipOrigin>() * 4));
				int num2;
				if (!this.IsLandSide)
				{
					if (!this.IsRaid)
					{
						num2 = selectedShips.Sum<IShipOrigin>((IShipOrigin x) => x.TotalCrewCapacity);
					}
					else
					{
						num2 = selectedShips.Sum<IShipOrigin>((IShipOrigin x) => x.MainDeckCrewCapacity);
					}
				}
				else
				{
					num2 = BannerlordConfig.MaxBattleSize;
				}
				int num3 = num2;
				int num4;
				if (!this.IsLandSide)
				{
					num4 = selectedShips.Sum<IShipOrigin>((IShipOrigin x) => x.SkeletalCrewCapacity);
				}
				else
				{
					num4 = 1;
				}
				int num5 = num4;
				int num6;
				if (!this.IsLandSide)
				{
					num6 = selectedShips.Sum<IShipOrigin>((IShipOrigin x) => x.MainDeckCrewCapacity);
				}
				else
				{
					num6 = 1;
				}
				int num7 = num6;
				this.CompositionGroup.UpdateTroopCountLimits(num, num3, num5, num7);
			}
		}

		// Token: 0x0600013F RID: 319 RVA: 0x00007218 File Offset: 0x00005418
		private void OnCultureSelection(BasicCultureObject selectedCulture)
		{
			this.CompositionGroup.SetCurrentSelectedCulture(selectedCulture);
			if (this.CurrentSelectedCharacter != null)
			{
				this.CurrentSelectedCharacter.ArmorColor1 = selectedCulture.Color;
				this.CurrentSelectedCharacter.ArmorColor2 = selectedCulture.Color2;
				this.CurrentSelectedCharacter.BannerCodeText = selectedCulture.Banner.BannerCode;
			}
		}

		// Token: 0x06000140 RID: 320 RVA: 0x00007274 File Offset: 0x00005474
		private void OnCharacterSelection(SelectorVM<NavalCustomBattleCharacterItemVM> selector)
		{
			BasicCharacterObject character = selector.SelectedItem.Character;
			this.SelectedCharacter = character;
			this.UpdateCharacterVisual();
			Action onCharacterSelected = this._onCharacterSelected;
			if (onCharacterSelected == null)
			{
				return;
			}
			onCharacterSelected();
		}

		// Token: 0x06000141 RID: 321 RVA: 0x000072AC File Offset: 0x000054AC
		public void UpdateCharacterVisual()
		{
			this.CurrentSelectedCharacter = new CharacterViewModel(1);
			CharacterViewModel currentSelectedCharacter = this.CurrentSelectedCharacter;
			BasicCharacterObject selectedCharacter = this.SelectedCharacter;
			int num = -1;
			NavalCustomBattleFactionSelectionVM factionSelectionGroup = this.FactionSelectionGroup;
			string text;
			if (factionSelectionGroup == null)
			{
				text = null;
			}
			else
			{
				NavalCustomBattleFactionItemVM selectedItem = factionSelectionGroup.SelectedItem;
				text = ((selectedItem != null) ? selectedItem.Faction.Banner.BannerCode : null);
			}
			currentSelectedCharacter.FillFrom(selectedCharacter, num, text);
			this.CurrentSelectedCharacter.SetEquipment(10, EquipmentElement.Invalid);
			NavalCustomBattleFactionSelectionVM factionSelectionGroup2 = this.FactionSelectionGroup;
			if (((factionSelectionGroup2 != null) ? factionSelectionGroup2.SelectedItem : null) != null)
			{
				this.CurrentSelectedCharacter.ArmorColor1 = this.FactionSelectionGroup.SelectedItem.Faction.Color;
				this.CurrentSelectedCharacter.ArmorColor2 = this.FactionSelectionGroup.SelectedItem.Faction.Color2;
			}
			this.ArmorsList.Clear();
			this.ArmorsList.Add(new CharacterEquipmentItemVM(this.SelectedCharacter.Equipment[5].Item));
			this.ArmorsList.Add(new CharacterEquipmentItemVM(this.SelectedCharacter.Equipment[9].Item));
			this.ArmorsList.Add(new CharacterEquipmentItemVM(this.SelectedCharacter.Equipment[6].Item));
			this.ArmorsList.Add(new CharacterEquipmentItemVM(this.SelectedCharacter.Equipment[8].Item));
			this.ArmorsList.Add(new CharacterEquipmentItemVM(this.SelectedCharacter.Equipment[7].Item));
			this.WeaponsList.Clear();
			this.WeaponsList.Add(new CharacterEquipmentItemVM(this.SelectedCharacter.Equipment[0].Item));
			this.WeaponsList.Add(new CharacterEquipmentItemVM(this.SelectedCharacter.Equipment[1].Item));
			this.WeaponsList.Add(new CharacterEquipmentItemVM(this.SelectedCharacter.Equipment[2].Item));
			this.WeaponsList.Add(new CharacterEquipmentItemVM(this.SelectedCharacter.Equipment[3].Item));
			this.WeaponsList.Add(new CharacterEquipmentItemVM(this.SelectedCharacter.Equipment[4].Item));
		}

		// Token: 0x06000142 RID: 322 RVA: 0x00007516 File Offset: 0x00005716
		public void Randomize(int targetDeckSize)
		{
			this.CharacterSelectionGroup.ExecuteRandomize();
			this.FactionSelectionGroup.ExecuteRandomize();
			this.ShipSelectionGroup.ExecuteRandomize(targetDeckSize);
			this.CompositionGroup.ExecuteRandomize(targetDeckSize);
		}

		// Token: 0x06000143 RID: 323 RVA: 0x00007548 File Offset: 0x00005748
		public override void OnFinalize()
		{
			base.OnFinalize();
			SelectorVM<NavalCustomBattleCharacterItemVM> characterSelectionGroup = this.CharacterSelectionGroup;
			if (characterSelectionGroup != null)
			{
				characterSelectionGroup.OnFinalize();
			}
			NavalCustomBattleFactionSelectionVM factionSelectionGroup = this.FactionSelectionGroup;
			if (factionSelectionGroup != null)
			{
				factionSelectionGroup.OnFinalize();
			}
			NavalCustomBattleShipSelectionGroupVM shipSelectionGroup = this.ShipSelectionGroup;
			if (shipSelectionGroup != null)
			{
				shipSelectionGroup.OnFinalize();
			}
			NavalCustomBattleArmyCompositionGroupVM compositionGroup = this.CompositionGroup;
			if (compositionGroup == null)
			{
				return;
			}
			compositionGroup.OnFinalize();
		}

		// Token: 0x06000144 RID: 324 RVA: 0x0000759E File Offset: 0x0000579E
		public void SetCycleTierInputKey(HotKey hotkey)
		{
			this.ShipSelectionGroup.SetCycleTierInputKey(hotkey);
		}

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x06000145 RID: 325 RVA: 0x000075AC File Offset: 0x000057AC
		// (set) Token: 0x06000146 RID: 326 RVA: 0x000075B4 File Offset: 0x000057B4
		[DataSourceProperty]
		public CharacterViewModel CurrentSelectedCharacter
		{
			get
			{
				return this._currentSelectedCharacter;
			}
			set
			{
				if (value != this._currentSelectedCharacter)
				{
					this._currentSelectedCharacter = value;
					base.OnPropertyChangedWithValue<CharacterViewModel>(value, "CurrentSelectedCharacter");
				}
			}
		}

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x06000147 RID: 327 RVA: 0x000075D2 File Offset: 0x000057D2
		// (set) Token: 0x06000148 RID: 328 RVA: 0x000075DA File Offset: 0x000057DA
		[DataSourceProperty]
		public MBBindingList<CharacterEquipmentItemVM> ArmorsList
		{
			get
			{
				return this._armorsList;
			}
			set
			{
				if (value != this._armorsList)
				{
					this._armorsList = value;
					base.OnPropertyChangedWithValue<MBBindingList<CharacterEquipmentItemVM>>(value, "ArmorsList");
				}
			}
		}

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x06000149 RID: 329 RVA: 0x000075F8 File Offset: 0x000057F8
		// (set) Token: 0x0600014A RID: 330 RVA: 0x00007600 File Offset: 0x00005800
		[DataSourceProperty]
		public MBBindingList<CharacterEquipmentItemVM> WeaponsList
		{
			get
			{
				return this._weaponsList;
			}
			set
			{
				if (value != this._weaponsList)
				{
					this._weaponsList = value;
					base.OnPropertyChangedWithValue<MBBindingList<CharacterEquipmentItemVM>>(value, "WeaponsList");
				}
			}
		}

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x0600014B RID: 331 RVA: 0x0000761E File Offset: 0x0000581E
		// (set) Token: 0x0600014C RID: 332 RVA: 0x00007626 File Offset: 0x00005826
		[DataSourceProperty]
		public string FactionText
		{
			get
			{
				return this._factionText;
			}
			set
			{
				if (value != this._factionText)
				{
					this._factionText = value;
					base.OnPropertyChangedWithValue<string>(value, "FactionText");
				}
			}
		}

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x0600014D RID: 333 RVA: 0x00007649 File Offset: 0x00005849
		// (set) Token: 0x0600014E RID: 334 RVA: 0x00007651 File Offset: 0x00005851
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

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x0600014F RID: 335 RVA: 0x00007674 File Offset: 0x00005874
		// (set) Token: 0x06000150 RID: 336 RVA: 0x0000767C File Offset: 0x0000587C
		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x06000151 RID: 337 RVA: 0x0000769F File Offset: 0x0000589F
		// (set) Token: 0x06000152 RID: 338 RVA: 0x000076A7 File Offset: 0x000058A7
		[DataSourceProperty]
		public SelectorVM<NavalCustomBattleCharacterItemVM> CharacterSelectionGroup
		{
			get
			{
				return this._characterSelectionGroup;
			}
			set
			{
				if (value != this._characterSelectionGroup)
				{
					this._characterSelectionGroup = value;
					base.OnPropertyChangedWithValue<SelectorVM<NavalCustomBattleCharacterItemVM>>(value, "CharacterSelectionGroup");
				}
			}
		}

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x06000153 RID: 339 RVA: 0x000076C5 File Offset: 0x000058C5
		// (set) Token: 0x06000154 RID: 340 RVA: 0x000076CD File Offset: 0x000058CD
		[DataSourceProperty]
		public NavalCustomBattleArmyCompositionGroupVM CompositionGroup
		{
			get
			{
				return this._compositionGroup;
			}
			set
			{
				if (value != this._compositionGroup)
				{
					this._compositionGroup = value;
					base.OnPropertyChangedWithValue<NavalCustomBattleArmyCompositionGroupVM>(value, "CompositionGroup");
				}
			}
		}

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x06000155 RID: 341 RVA: 0x000076EB File Offset: 0x000058EB
		// (set) Token: 0x06000156 RID: 342 RVA: 0x000076F3 File Offset: 0x000058F3
		[DataSourceProperty]
		public NavalCustomBattleFactionSelectionVM FactionSelectionGroup
		{
			get
			{
				return this._factionSelectionGroup;
			}
			set
			{
				if (value != this._factionSelectionGroup)
				{
					this._factionSelectionGroup = value;
					base.OnPropertyChangedWithValue<NavalCustomBattleFactionSelectionVM>(value, "FactionSelectionGroup");
				}
			}
		}

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x06000157 RID: 343 RVA: 0x00007711 File Offset: 0x00005911
		// (set) Token: 0x06000158 RID: 344 RVA: 0x00007719 File Offset: 0x00005919
		[DataSourceProperty]
		public NavalCustomBattleShipSelectionGroupVM ShipSelectionGroup
		{
			get
			{
				return this._shipSelectionGroup;
			}
			set
			{
				if (value != this._shipSelectionGroup)
				{
					this._shipSelectionGroup = value;
					base.OnPropertyChangedWithValue<NavalCustomBattleShipSelectionGroupVM>(value, "ShipSelectionGroup");
				}
			}
		}

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x06000159 RID: 345 RVA: 0x00007737 File Offset: 0x00005937
		// (set) Token: 0x0600015A RID: 346 RVA: 0x0000773F File Offset: 0x0000593F
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
					this.ShipSelectionGroup.IsRaid = value;
					this.UpdateTroopCountLimits();
				}
			}
		}

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x0600015B RID: 347 RVA: 0x0000776F File Offset: 0x0000596F
		// (set) Token: 0x0600015C RID: 348 RVA: 0x00007777 File Offset: 0x00005977
		[DataSourceProperty]
		public bool IsLandSide
		{
			get
			{
				return this._isLandSide;
			}
			set
			{
				if (value != this._isLandSide)
				{
					this._isLandSide = value;
					base.OnPropertyChangedWithValue(value, "IsLandSide");
					this.CompositionGroup.IsLand = value;
					this.UpdateTroopCountLimits();
				}
			}
		}

		// Token: 0x040000A3 RID: 163
		private readonly TextObject _sideName;

		// Token: 0x040000A4 RID: 164
		private readonly bool _isPlayerSide;

		// Token: 0x040000A5 RID: 165
		private readonly Action _onCharacterSelected;

		// Token: 0x040000A6 RID: 166
		private readonly Action _onShipSelected;

		// Token: 0x040000A7 RID: 167
		private NavalCustomBattleArmyCompositionGroupVM _compositionGroup;

		// Token: 0x040000A8 RID: 168
		private NavalCustomBattleFactionSelectionVM _factionSelectionGroup;

		// Token: 0x040000A9 RID: 169
		private SelectorVM<NavalCustomBattleCharacterItemVM> _characterSelectionGroup;

		// Token: 0x040000AA RID: 170
		private NavalCustomBattleShipSelectionGroupVM _shipSelectionGroup;

		// Token: 0x040000AB RID: 171
		private CharacterViewModel _currentSelectedCharacter;

		// Token: 0x040000AC RID: 172
		private MBBindingList<CharacterEquipmentItemVM> _armorsList;

		// Token: 0x040000AD RID: 173
		private MBBindingList<CharacterEquipmentItemVM> _weaponsList;

		// Token: 0x040000AE RID: 174
		private string _name;

		// Token: 0x040000AF RID: 175
		private string _factionText;

		// Token: 0x040000B0 RID: 176
		private string _titleText;

		// Token: 0x040000B1 RID: 177
		private bool _isRaid;

		// Token: 0x040000B2 RID: 178
		private bool _isLandSide;
	}
}
