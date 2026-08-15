using System;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;

namespace AutoResolveRebalanced
{
	// Token: 0x0200000D RID: 13
	public class CustomSettings : AttributeGlobalSettings<CustomSettings>, ICustomSettingsProvider
	{
		// Token: 0x17000028 RID: 40
		// (get) Token: 0x06000063 RID: 99 RVA: 0x00002DD2 File Offset: 0x00000FD2
		public override string Id
		{
			get
			{
				return "AutoResolveRebalanced";
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x06000064 RID: 100 RVA: 0x00002DD9 File Offset: 0x00000FD9
		public override string DisplayName
		{
			get
			{
				return "Auto Resolve Rebalanced";
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x06000065 RID: 101 RVA: 0x00002DE0 File Offset: 0x00000FE0
		public override string FolderName
		{
			get
			{
				return "AutoResolveRebalanced";
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000066 RID: 102 RVA: 0x00002DE7 File Offset: 0x00000FE7
		public override string FormatType
		{
			get
			{
				return "json2";
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000067 RID: 103 RVA: 0x00002DEE File Offset: 0x00000FEE
		// (set) Token: 0x06000068 RID: 104 RVA: 0x00002DF6 File Offset: 0x00000FF6
		[SettingPropertyFloatingInteger("{=AWqIUpd3S}Battle Speed", 0.1f, 30f, "0.0", Order = 2, RequireRestart = false, HintText = "{=AcvJM7L2E}Increase this if you feel AIvsAI battle is taking too long. If increased too much, HP may not be simulated properly and troops will die more randomly.")]
		[SettingPropertyGroup("{=Azt5UxgZ0}Main Functions", GroupOrder = 1)]
		public float damageModifier { get; set; } = DefVal.damageModifier;

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000069 RID: 105 RVA: 0x00002DFF File Offset: 0x00000FFF
		// (set) Token: 0x0600006A RID: 106 RVA: 0x00002E07 File Offset: 0x00001007
		[SettingPropertyBool("{=AJn978XeY}Armor Reduce Damage", Order = 3, RequireRestart = false, HintText = "{=AsQMXi3i9}Enable troops damage to be reduced based on armor they are equipped.")]
		[SettingPropertyGroup("{=Azt5UxgZ0}Main Functions")]
		public bool armorEnabled { get; set; } = DefVal.armorEnabled;

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x0600006B RID: 107 RVA: 0x00002E10 File Offset: 0x00001010
		// (set) Token: 0x0600006C RID: 108 RVA: 0x00002E18 File Offset: 0x00001018
		[SettingPropertyBool("{=A4pArFFd3}Weapon Type Bonus to Damage", Order = 4, RequireRestart = false, HintText = "{=AP428vsus}Enable troops damage to be increased based on weapon type they are equipped.")]
		[SettingPropertyGroup("{=Azt5UxgZ0}Main Functions")]
		public bool weaponEnabled { get; set; } = DefVal.weaponEnabled;

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x0600006D RID: 109 RVA: 0x00002E21 File Offset: 0x00001021
		// (set) Token: 0x0600006E RID: 110 RVA: 0x00002E29 File Offset: 0x00001029
		[SettingPropertyBool("{=AyNzxwwuF}Apply Mod to AI battles", Order = 1, RequireRestart = false, HintText = "{=Av411FSiO}Make mod's features apply to AIvsAI battles. Disable this if you have troubles with AIvsAI battles.")]
		[SettingPropertyGroup("{=Azt5UxgZ0}Main Functions")]
		public bool aiEnabled { get; set; } = DefVal.aiEnabled;

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x0600006F RID: 111 RVA: 0x00002E32 File Offset: 0x00001032
		// (set) Token: 0x06000070 RID: 112 RVA: 0x00002E3A File Offset: 0x0000103A
		[SettingPropertyFloatingInteger("{=AyaxvxQnm}Armor Reduce Percentage to DMG", 0.01f, 1f, "0 %", Order = 4, RequireRestart = false, HintText = "{=As8dxw5cW}Armor percentage subtracted from damage")]
		[SettingPropertyGroup("{=AdZW2ReSX}Armor Bonus Settings", GroupOrder = 2)]
		public float defModifierPct { get; set; } = DefVal.defModifierPct;

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000071 RID: 113 RVA: 0x00002E43 File Offset: 0x00001043
		// (set) Token: 0x06000072 RID: 114 RVA: 0x00002E4B File Offset: 0x0000104B
		[SettingPropertyBool("{=AWxluQzpF}Shield Reduce Damage", Order = 6, RequireRestart = false, HintText = "{=Ap89MtOjq}Enable troops armor which reduce damage to be increased when equipped with shield.")]
		[SettingPropertyGroup("{=AdZW2ReSX}Armor Bonus Settings")]
		public bool shieldEnabled { get; set; } = DefVal.shieldEnabled;

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x06000073 RID: 115 RVA: 0x00002E54 File Offset: 0x00001054
		// (set) Token: 0x06000074 RID: 116 RVA: 0x00002E5C File Offset: 0x0000105C
		[SettingPropertyFloatingInteger("{=AtJckbS0C}Shield Bonus to Armor", 0.01f, 1f, "0 %", Order = 7, RequireRestart = false, HintText = "{=AtchMifgr}Amount of bonus armor receives when equipped with a shield.")]
		[SettingPropertyGroup("{=AdZW2ReSX}Armor Bonus Settings")]
		public float shieldMultiplierPct { get; set; } = DefVal.shieldMultiplierPct;

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x06000075 RID: 117 RVA: 0x00002E65 File Offset: 0x00001065
		// (set) Token: 0x06000076 RID: 118 RVA: 0x00002E6D File Offset: 0x0000106D
		[SettingPropertyFloatingInteger("{=ADOICHLqJ}TwoHanded Bonus to Unmounted", 0.01f, 1f, "0 %", Order = 7, RequireRestart = false, HintText = "{=Au6Y4XFPn}Amount of bonus damage when TwoHanded troop attacks Unmounted troop.")]
		[SettingPropertyGroup("{=ALgNmM7LG}Weapon Bonus Settings", GroupOrder = 3)]
		public float twoHandedBonusPct { get; set; } = DefVal.twoHandedBonusPct;

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x06000077 RID: 119 RVA: 0x00002E76 File Offset: 0x00001076
		// (set) Token: 0x06000078 RID: 120 RVA: 0x00002E7E File Offset: 0x0000107E
		[SettingPropertyFloatingInteger("{=A66DkBe5G}Ranged Bonus to Unshielded", 0.01f, 1f, "0 %", Order = 7, RequireRestart = false, HintText = "{=ALr4cNdpc}Amount of bonus damage when Ranged troop attacks troop without shield.")]
		[SettingPropertyGroup("{=ALgNmM7LG}Weapon Bonus Settings")]
		public float rangedBonusPct { get; set; } = DefVal.rangedBonusPct;

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x06000079 RID: 121 RVA: 0x00002E87 File Offset: 0x00001087
		// (set) Token: 0x0600007A RID: 122 RVA: 0x00002E8F File Offset: 0x0000108F
		[SettingPropertyFloatingInteger("{=Ax1tgHtCF}Polearm Bonus to Mounted", 0.01f, 1f, "0 %", Order = 7, RequireRestart = false, HintText = "{=AjoVAOHzl}Amount of bonus damage when Polearm troop attacks Mounted troop.")]
		[SettingPropertyGroup("{=ALgNmM7LG}Weapon Bonus Settings")]
		public float polearmBonusPct { get; set; } = DefVal.polearmBonusPct;

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x0600007B RID: 123 RVA: 0x00002E98 File Offset: 0x00001098
		// (set) Token: 0x0600007C RID: 124 RVA: 0x00002EA0 File Offset: 0x000010A0
		[SettingPropertyBool("{=AqmwiRsGq}Show logs", Order = 3, RequireRestart = false)]
		[SettingPropertyGroup("{=A9G576Zxn}Debug Settings", GroupOrder = 99)]
		public bool showLog { get; set; } = DefVal.showLog;

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x0600007D RID: 125 RVA: 0x00002EA9 File Offset: 0x000010A9
		// (set) Token: 0x0600007E RID: 126 RVA: 0x00002EB1 File Offset: 0x000010B1
		[SettingPropertyBool("{=AyDRVw7tQ}Show Errors", Order = 1, RequireRestart = false)]
		[SettingPropertyGroup("{=A9G576Zxn}Debug Settings", GroupOrder = 99)]
		public bool showError { get; set; } = DefVal.showError;

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x0600007F RID: 127 RVA: 0x00002EBA File Offset: 0x000010BA
		// (set) Token: 0x06000080 RID: 128 RVA: 0x00002EC2 File Offset: 0x000010C2
		[SettingPropertyBool("{=A41u5EM2B}Show Warnings", Order = 2, RequireRestart = false)]
		[SettingPropertyGroup("{=A9G576Zxn}Debug Settings", GroupOrder = 99)]
		public bool showWarn { get; set; } = DefVal.showWarn;
	}
}
