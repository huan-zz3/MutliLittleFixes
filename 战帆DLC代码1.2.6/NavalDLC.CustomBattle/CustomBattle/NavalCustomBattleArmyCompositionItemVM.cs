using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace NavalDLC.CustomBattle.CustomBattle
{
	// Token: 0x0200000B RID: 11
	public class NavalCustomBattleArmyCompositionItemVM : ViewModel
	{
		// Token: 0x06000076 RID: 118 RVA: 0x0000420C File Offset: 0x0000240C
		public NavalCustomBattleArmyCompositionItemVM(NavalCustomBattleArmyCompositionItemVM.CompositionType type, List<BasicCharacterObject> allCharacterObjects, MBReadOnlyList<SkillObject> allSkills, Action<int, int> onCompositionValueChanged, NavalCustomBattleTroopTypeSelectionPopUpVM troopTypeSelectionPopUp, int[] compositionValues)
		{
			this._allCharacterObjects = allCharacterObjects;
			this._allSkills = allSkills;
			this._onCompositionValueChanged = onCompositionValueChanged;
			this._troopTypeSelectionPopUp = troopTypeSelectionPopUp;
			this._type = type;
			this._compositionValues = compositionValues;
			this.TroopTypes = new MBBindingList<NavalCustomBattleTroopTypeVM>();
			this.InvalidHint = new HintViewModel(new TextObject("{=iSQTtNUD}This faction doesn't have this troop type.", null), null);
			this.AddTroopTypeHint = new HintViewModel(new TextObject("{=eMbuGGus}Select troops to spawn in formation.", null), null);
			this.UpdatePercentageText(this._compositionValues[(int)this._type]);
		}

		// Token: 0x06000077 RID: 119 RVA: 0x00004298 File Offset: 0x00002498
		public override void RefreshValues()
		{
			base.RefreshValues();
		}

		// Token: 0x06000078 RID: 120 RVA: 0x000042A0 File Offset: 0x000024A0
		public void SetCurrentSelectedCulture(BasicCultureObject culture)
		{
			this.IsLocked = false;
			this._culture = culture;
			this.PopulateTroopTypes();
		}

		// Token: 0x06000079 RID: 121 RVA: 0x000042B8 File Offset: 0x000024B8
		public void ExecuteRandomize(int compositionValue)
		{
			this.IsValid = true;
			this.IsLocked = false;
			this.CompositionValue = compositionValue;
			this.IsValid = this.TroopTypes.Count > 0;
			this.TroopTypes.ApplyActionOnAllItems(delegate(NavalCustomBattleTroopTypeVM x)
			{
				x.ExecuteRandomize();
			});
			if (!this.TroopTypes.Any<NavalCustomBattleTroopTypeVM>((NavalCustomBattleTroopTypeVM x) => x.IsSelected) && this.IsValid)
			{
				this.TroopTypes[0].IsSelected = true;
			}
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00004360 File Offset: 0x00002560
		public void ExecuteAddTroopTypes()
		{
			string text = GameTexts.FindText("str_custom_battle_choose_troop", this._type.ToString()).ToString();
			NavalCustomBattleTroopTypeSelectionPopUpVM troopTypeSelectionPopUp = this._troopTypeSelectionPopUp;
			if (troopTypeSelectionPopUp == null)
			{
				return;
			}
			troopTypeSelectionPopUp.OpenPopUp(text, this.TroopTypes);
		}

		// Token: 0x0600007B RID: 123 RVA: 0x000043A5 File Offset: 0x000025A5
		public void RefreshCompositionValue()
		{
			base.OnPropertyChanged("CompositionValue");
			this.UpdatePercentageText(this._compositionValues[(int)this._type]);
		}

		// Token: 0x0600007C RID: 124 RVA: 0x000043C8 File Offset: 0x000025C8
		private void UpdatePercentageText(int percentage)
		{
			int num = (int)MathF.Clamp((float)percentage, 0f, 100f);
			this.CompositionValuePercentageText = GameTexts.FindText("str_NUMBER_percent", null).SetTextVariable("NUMBER", num).ToString();
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00004409 File Offset: 0x00002609
		private void OnValidityChanged(bool value)
		{
			this.IsLocked = false;
			if (!value)
			{
				this.CompositionValue = 0;
			}
			this.IsLocked = !value;
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00004428 File Offset: 0x00002628
		private void PopulateTroopTypes()
		{
			this.TroopTypes.Clear();
			MBReadOnlyList<BasicCharacterObject> defaultCharacters = this.GetDefaultCharacters();
			foreach (BasicCharacterObject basicCharacterObject in this._allCharacterObjects)
			{
				if (this.IsValidUnitItem(basicCharacterObject))
				{
					this.TroopTypes.Add(new NavalCustomBattleTroopTypeVM(basicCharacterObject, new Action<NavalCustomBattleTroopTypeVM>(this._troopTypeSelectionPopUp.OnItemSelectionToggled), NavalCustomBattleArmyCompositionItemVM.GetTroopTypeIconData(basicCharacterObject, this._type, this.IsLand, false), this._allSkills, defaultCharacters.Contains(basicCharacterObject)));
				}
			}
			this.IsValid = this.TroopTypes.Count > 0;
			if (this.IsValid)
			{
				if (!this.TroopTypes.Any<NavalCustomBattleTroopTypeVM>((NavalCustomBattleTroopTypeVM x) => x.IsDefault))
				{
					this.TroopTypes[0].IsDefault = true;
				}
			}
			this.TroopTypes.ApplyActionOnAllItems(delegate(NavalCustomBattleTroopTypeVM x)
			{
				x.IsSelected = x.IsDefault;
			});
		}

		// Token: 0x0600007F RID: 127 RVA: 0x00004558 File Offset: 0x00002758
		private bool IsValidUnitItem(BasicCharacterObject o)
		{
			if (o == null || this._culture != o.Culture)
			{
				return false;
			}
			if (this.IsLand)
			{
				switch (this._type)
				{
				case NavalCustomBattleArmyCompositionItemVM.CompositionType.MeleeInfantry:
					return o.DefaultFormationClass == null || o.DefaultFormationClass == 5;
				case NavalCustomBattleArmyCompositionItemVM.CompositionType.RangedInfantry:
					return o.DefaultFormationClass == 1;
				case NavalCustomBattleArmyCompositionItemVM.CompositionType.MeleeCavalry:
					return o.DefaultFormationClass == 2 || o.DefaultFormationClass == 7 || o.DefaultFormationClass == 6;
				case NavalCustomBattleArmyCompositionItemVM.CompositionType.RangedCavalry:
					return o.DefaultFormationClass == 3;
				default:
					return false;
				}
			}
			else
			{
				NavalCustomBattleArmyCompositionItemVM.CompositionType type = this._type;
				if (type != NavalCustomBattleArmyCompositionItemVM.CompositionType.MeleeInfantry)
				{
					return type == NavalCustomBattleArmyCompositionItemVM.CompositionType.RangedInfantry && (o.DefaultFormationClass == 1 || o.DefaultFormationClass == 3);
				}
				return o.DefaultFormationClass == null || o.DefaultFormationClass == 5 || o.DefaultFormationClass == 2 || o.DefaultFormationClass == 7 || o.DefaultFormationClass == 6;
			}
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00004644 File Offset: 0x00002844
		private MBReadOnlyList<BasicCharacterObject> GetDefaultCharacters()
		{
			MBList<BasicCharacterObject> mblist = new MBList<BasicCharacterObject>();
			FormationClass formationClass = 10;
			switch (this._type)
			{
			case NavalCustomBattleArmyCompositionItemVM.CompositionType.MeleeInfantry:
				formationClass = 0;
				break;
			case NavalCustomBattleArmyCompositionItemVM.CompositionType.RangedInfantry:
				formationClass = 1;
				break;
			case NavalCustomBattleArmyCompositionItemVM.CompositionType.MeleeCavalry:
				formationClass = 2;
				break;
			case NavalCustomBattleArmyCompositionItemVM.CompositionType.RangedCavalry:
				formationClass = 3;
				break;
			}
			mblist.Add(NavalCustomBattleHelper.GetDefaultTroopOfFormationForFaction(this._culture, formationClass));
			return mblist;
		}

		// Token: 0x06000081 RID: 129 RVA: 0x0000469C File Offset: 0x0000289C
		public static StringItemWithHintVM GetTroopTypeIconData(BasicCharacterObject basicCharacterObject, NavalCustomBattleArmyCompositionItemVM.CompositionType type, bool isLand, bool isBig = false)
		{
			bool flag = false;
			if (basicCharacterObject != null && !isLand)
			{
				flag = basicCharacterObject.StringId.Contains("marine") || basicCharacterObject.Culture.StringId.Contains("nord");
			}
			TextObject textObject = new TextObject("{=!}{TYPENAME}{MARINER}{BIG}", null);
			TextObject textObject2;
			switch (type)
			{
			case NavalCustomBattleArmyCompositionItemVM.CompositionType.MeleeInfantry:
			{
				textObject.SetTextVariable("TYPENAME", "infantry");
				string text = (flag ? "Infantry_Mariner" : "Infantry");
				textObject2 = GameTexts.FindText("str_troop_type_name", text);
				break;
			}
			case NavalCustomBattleArmyCompositionItemVM.CompositionType.RangedInfantry:
			{
				textObject.SetTextVariable("TYPENAME", "bow");
				string text = (flag ? "Ranged_Mariner" : "Ranged");
				textObject2 = GameTexts.FindText("str_troop_type_name", text);
				break;
			}
			case NavalCustomBattleArmyCompositionItemVM.CompositionType.MeleeCavalry:
				textObject.SetTextVariable("TYPENAME", "cavalry");
				textObject2 = GameTexts.FindText("str_troop_type_name", "Cavalry");
				break;
			case NavalCustomBattleArmyCompositionItemVM.CompositionType.RangedCavalry:
				textObject.SetTextVariable("TYPENAME", "horse_archer");
				textObject2 = GameTexts.FindText("str_troop_type_name", "HorseArcher");
				break;
			default:
				return new StringItemWithHintVM("", null);
			}
			textObject.SetTextVariable("MARINER", flag ? "_mariner" : "");
			textObject.SetTextVariable("BIG", isBig ? "_big" : "");
			return new StringItemWithHintVM("General\\TroopTypeIcons\\icon_troop_type_" + textObject.ToString(), new TextObject("{=!}" + textObject2.ToString(), null));
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x06000082 RID: 130 RVA: 0x00004817 File Offset: 0x00002A17
		// (set) Token: 0x06000083 RID: 131 RVA: 0x0000481F File Offset: 0x00002A1F
		[DataSourceProperty]
		public MBBindingList<NavalCustomBattleTroopTypeVM> TroopTypes
		{
			get
			{
				return this._troopTypes;
			}
			set
			{
				if (value != this._troopTypes)
				{
					this._troopTypes = value;
					base.OnPropertyChangedWithValue<MBBindingList<NavalCustomBattleTroopTypeVM>>(value, "TroopTypes");
				}
			}
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x06000084 RID: 132 RVA: 0x0000483D File Offset: 0x00002A3D
		// (set) Token: 0x06000085 RID: 133 RVA: 0x00004845 File Offset: 0x00002A45
		[DataSourceProperty]
		public HintViewModel InvalidHint
		{
			get
			{
				return this._invalidHint;
			}
			set
			{
				if (value != this._invalidHint)
				{
					this._invalidHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "InvalidHint");
				}
			}
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x06000086 RID: 134 RVA: 0x00004863 File Offset: 0x00002A63
		// (set) Token: 0x06000087 RID: 135 RVA: 0x0000486B File Offset: 0x00002A6B
		[DataSourceProperty]
		public HintViewModel AddTroopTypeHint
		{
			get
			{
				return this._addTroopTypeHint;
			}
			set
			{
				if (value != this._addTroopTypeHint)
				{
					this._addTroopTypeHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "AddTroopTypeHint");
				}
			}
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x06000088 RID: 136 RVA: 0x00004889 File Offset: 0x00002A89
		// (set) Token: 0x06000089 RID: 137 RVA: 0x00004891 File Offset: 0x00002A91
		[DataSourceProperty]
		public bool IsLocked
		{
			get
			{
				return this._isLocked;
			}
			set
			{
				if (value != this._isLocked)
				{
					this._isLocked = value;
					base.OnPropertyChangedWithValue(value, "IsLocked");
				}
			}
		}

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x0600008A RID: 138 RVA: 0x000048AF File Offset: 0x00002AAF
		// (set) Token: 0x0600008B RID: 139 RVA: 0x000048B7 File Offset: 0x00002AB7
		[DataSourceProperty]
		public bool IsValid
		{
			get
			{
				return this._isValid;
			}
			set
			{
				if (value != this._isValid)
				{
					this._isValid = value;
					base.OnPropertyChangedWithValue(value, "IsValid");
				}
				this.OnValidityChanged(value);
			}
		}

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x0600008C RID: 140 RVA: 0x000048DC File Offset: 0x00002ADC
		// (set) Token: 0x0600008D RID: 141 RVA: 0x000048EB File Offset: 0x00002AEB
		[DataSourceProperty]
		public int CompositionValue
		{
			get
			{
				return this._compositionValues[(int)this._type];
			}
			set
			{
				if (value != this._compositionValues[(int)this._type])
				{
					this._onCompositionValueChanged(value, (int)this._type);
				}
			}
		}

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x0600008E RID: 142 RVA: 0x0000490F File Offset: 0x00002B0F
		// (set) Token: 0x0600008F RID: 143 RVA: 0x00004917 File Offset: 0x00002B17
		[DataSourceProperty]
		public string CompositionValuePercentageText
		{
			get
			{
				return this._compositionValuePercentageText;
			}
			set
			{
				if (value != this._compositionValuePercentageText)
				{
					this._compositionValuePercentageText = value;
					base.OnPropertyChangedWithValue<string>(value, "CompositionValuePercentageText");
				}
			}
		}

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x06000090 RID: 144 RVA: 0x0000493A File Offset: 0x00002B3A
		// (set) Token: 0x06000091 RID: 145 RVA: 0x00004942 File Offset: 0x00002B42
		[DataSourceProperty]
		public bool IsLand
		{
			get
			{
				return this._isLand;
			}
			set
			{
				if (value != this._isLand)
				{
					this._isLand = value;
					base.OnPropertyChangedWithValue(value, "IsLand");
					this.PopulateTroopTypes();
				}
			}
		}

		// Token: 0x04000024 RID: 36
		private readonly MBReadOnlyList<SkillObject> _allSkills;

		// Token: 0x04000025 RID: 37
		private readonly List<BasicCharacterObject> _allCharacterObjects;

		// Token: 0x04000026 RID: 38
		private readonly Action<int, int> _onCompositionValueChanged;

		// Token: 0x04000027 RID: 39
		private readonly NavalCustomBattleTroopTypeSelectionPopUpVM _troopTypeSelectionPopUp;

		// Token: 0x04000028 RID: 40
		private BasicCultureObject _culture;

		// Token: 0x04000029 RID: 41
		private readonly NavalCustomBattleArmyCompositionItemVM.CompositionType _type;

		// Token: 0x0400002A RID: 42
		private readonly int[] _compositionValues;

		// Token: 0x0400002B RID: 43
		private MBBindingList<NavalCustomBattleTroopTypeVM> _troopTypes;

		// Token: 0x0400002C RID: 44
		private HintViewModel _invalidHint;

		// Token: 0x0400002D RID: 45
		private HintViewModel _addTroopTypeHint;

		// Token: 0x0400002E RID: 46
		private bool _isLocked;

		// Token: 0x0400002F RID: 47
		private bool _isValid;

		// Token: 0x04000030 RID: 48
		private string _compositionValuePercentageText;

		// Token: 0x04000031 RID: 49
		private bool _isLand;

		// Token: 0x0200002B RID: 43
		public enum CompositionType
		{
			// Token: 0x04000127 RID: 295
			MeleeInfantry,
			// Token: 0x04000128 RID: 296
			RangedInfantry,
			// Token: 0x04000129 RID: 297
			MeleeCavalry,
			// Token: 0x0400012A RID: 298
			RangedCavalry
		}
	}
}
