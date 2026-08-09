using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Core.ViewModelCollection.ImageIdentifiers;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace NavalDLC.CustomBattle.CustomBattle
{
	// Token: 0x0200001A RID: 26
	public class NavalCustomBattleTroopTypeVM : ViewModel
	{
		// Token: 0x1700008A RID: 138
		// (get) Token: 0x0600017E RID: 382 RVA: 0x00007C59 File Offset: 0x00005E59
		// (set) Token: 0x0600017F RID: 383 RVA: 0x00007C61 File Offset: 0x00005E61
		public BasicCharacterObject Character { get; private set; }

		// Token: 0x06000180 RID: 384 RVA: 0x00007C6C File Offset: 0x00005E6C
		public NavalCustomBattleTroopTypeVM(BasicCharacterObject character, Action<NavalCustomBattleTroopTypeVM> onSelectionToggled, StringItemWithHintVM typeIconData, MBReadOnlyList<SkillObject> allSkills, bool isDefault)
		{
			this.Character = character;
			this.IsDefault = isDefault;
			this._onSelectionToggled = onSelectionToggled;
			this._allSkills = allSkills;
			if (character != null)
			{
				this.Visual = new CharacterImageIdentifierVM(CharacterCode.CreateFrom(character));
				this.NameHint = new HintViewModel(character.Name, null);
				this.TroopSkillsHint = new BasicTooltipViewModel(() => this.GetTroopSkillsTooltip(this.Character));
				this.TierIconData = NavalCustomBattleTroopTypeVM.GetCharacterTierData(this.Character, false);
				this.TypeIconData = typeIconData;
			}
			else
			{
				Debug.FailedAssert("Character shouldn't be null", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.CustomBattle\\CustomBattle\\NavalCustomBattleTroopTypeVM.cs", ".ctor", 38);
			}
			this.RefreshValues();
		}

		// Token: 0x06000181 RID: 385 RVA: 0x00007D11 File Offset: 0x00005F11
		public override void RefreshValues()
		{
			base.RefreshValues();
			BasicCharacterObject character = this.Character;
			this.Name = ((character != null) ? character.Name.ToString() : null);
		}

		// Token: 0x06000182 RID: 386 RVA: 0x00007D36 File Offset: 0x00005F36
		public void ExecuteToggleSelection()
		{
			Action<NavalCustomBattleTroopTypeVM> onSelectionToggled = this._onSelectionToggled;
			if (onSelectionToggled == null)
			{
				return;
			}
			onSelectionToggled(this);
		}

		// Token: 0x06000183 RID: 387 RVA: 0x00007D49 File Offset: 0x00005F49
		public void ExecuteRandomize()
		{
			this.IsSelected = MBRandom.RandomInt(2) == 1;
		}

		// Token: 0x06000184 RID: 388 RVA: 0x00007D5C File Offset: 0x00005F5C
		private List<TooltipProperty> GetTroopSkillsTooltip(BasicCharacterObject character)
		{
			List<TooltipProperty> list = new List<TooltipProperty>();
			list.Add(new TooltipProperty("", character.Name.ToString(), 1, false, 4096));
			list.Add(new TooltipProperty(GameTexts.FindText("str_skills", null).ToString(), " ", 0, false, 0));
			list.Add(new TooltipProperty("", "", 0, false, 512));
			foreach (SkillObject skillObject in this._allSkills)
			{
				int skillValue = character.GetSkillValue(skillObject);
				if (skillValue > 0)
				{
					list.Add(new TooltipProperty(skillObject.Name.ToString(), skillValue.ToString(), 0, false, 0));
				}
			}
			return list;
		}

		// Token: 0x06000185 RID: 389 RVA: 0x00007E3C File Offset: 0x0000603C
		public static StringItemWithHintVM GetCharacterTierData(BasicCharacterObject character, bool isBig = false)
		{
			int characterTier = NavalCustomBattleTroopTypeVM.GetCharacterTier(character);
			if (characterTier <= 0 || characterTier > 7)
			{
				return new StringItemWithHintVM("", null);
			}
			string text = (isBig ? (characterTier.ToString() + "_big") : characterTier.ToString());
			string text2 = "General\\TroopTierIcons\\icon_tier_" + text;
			GameTexts.SetVariable("TIER_LEVEL", characterTier);
			TextObject textObject = new TextObject("{=!}" + GameTexts.FindText("str_party_troop_tier", null).ToString(), null);
			return new StringItemWithHintVM(text2, textObject);
		}

		// Token: 0x06000186 RID: 390 RVA: 0x00007EBF File Offset: 0x000060BF
		public static int GetCharacterTier(BasicCharacterObject character)
		{
			if (character.IsHero)
			{
				return 0;
			}
			return MathF.Min(MathF.Max(MathF.Ceiling(((float)character.Level - 5f) / 5f), 0), 7);
		}

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x06000187 RID: 391 RVA: 0x00007EEF File Offset: 0x000060EF
		// (set) Token: 0x06000188 RID: 392 RVA: 0x00007EF7 File Offset: 0x000060F7
		[DataSourceProperty]
		public CharacterImageIdentifierVM Visual
		{
			get
			{
				return this._visual;
			}
			set
			{
				if (value != this._visual)
				{
					this._visual = value;
					base.OnPropertyChangedWithValue<CharacterImageIdentifierVM>(value, "Visual");
				}
			}
		}

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x06000189 RID: 393 RVA: 0x00007F15 File Offset: 0x00006115
		// (set) Token: 0x0600018A RID: 394 RVA: 0x00007F1D File Offset: 0x0000611D
		[DataSourceProperty]
		public BasicTooltipViewModel TroopSkillsHint
		{
			get
			{
				return this._troopSkillsHint;
			}
			set
			{
				if (value != this._troopSkillsHint)
				{
					this._troopSkillsHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "TroopSkillsHint");
				}
			}
		}

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x0600018B RID: 395 RVA: 0x00007F3B File Offset: 0x0000613B
		// (set) Token: 0x0600018C RID: 396 RVA: 0x00007F43 File Offset: 0x00006143
		[DataSourceProperty]
		public HintViewModel NameHint
		{
			get
			{
				return this._nameHint;
			}
			set
			{
				if (value != this._nameHint)
				{
					this._nameHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "NameHint");
				}
			}
		}

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x0600018D RID: 397 RVA: 0x00007F61 File Offset: 0x00006161
		// (set) Token: 0x0600018E RID: 398 RVA: 0x00007F69 File Offset: 0x00006169
		[DataSourceProperty]
		public StringItemWithHintVM TierIconData
		{
			get
			{
				return this._tierIconData;
			}
			set
			{
				if (value != this._tierIconData)
				{
					this._tierIconData = value;
					base.OnPropertyChangedWithValue<StringItemWithHintVM>(value, "TierIconData");
				}
			}
		}

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x0600018F RID: 399 RVA: 0x00007F87 File Offset: 0x00006187
		// (set) Token: 0x06000190 RID: 400 RVA: 0x00007F8F File Offset: 0x0000618F
		[DataSourceProperty]
		public StringItemWithHintVM TypeIconData
		{
			get
			{
				return this._typeIconData;
			}
			set
			{
				if (value != this._typeIconData)
				{
					this._typeIconData = value;
					base.OnPropertyChangedWithValue<StringItemWithHintVM>(value, "TypeIconData");
				}
			}
		}

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x06000191 RID: 401 RVA: 0x00007FAD File Offset: 0x000061AD
		// (set) Token: 0x06000192 RID: 402 RVA: 0x00007FB5 File Offset: 0x000061B5
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

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x06000193 RID: 403 RVA: 0x00007FD8 File Offset: 0x000061D8
		// (set) Token: 0x06000194 RID: 404 RVA: 0x00007FE0 File Offset: 0x000061E0
		[DataSourceProperty]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
				}
			}
		}

		// Token: 0x040000C1 RID: 193
		public bool IsDefault;

		// Token: 0x040000C2 RID: 194
		private readonly Action<NavalCustomBattleTroopTypeVM> _onSelectionToggled;

		// Token: 0x040000C3 RID: 195
		private readonly MBReadOnlyList<SkillObject> _allSkills;

		// Token: 0x040000C4 RID: 196
		private CharacterImageIdentifierVM _visual;

		// Token: 0x040000C5 RID: 197
		private BasicTooltipViewModel _troopSkillsHint;

		// Token: 0x040000C6 RID: 198
		private HintViewModel _nameHint;

		// Token: 0x040000C7 RID: 199
		private StringItemWithHintVM _tierIconData;

		// Token: 0x040000C8 RID: 200
		private StringItemWithHintVM _typeIconData;

		// Token: 0x040000C9 RID: 201
		private string _name;

		// Token: 0x040000CA RID: 202
		private bool _isSelected;
	}
}
