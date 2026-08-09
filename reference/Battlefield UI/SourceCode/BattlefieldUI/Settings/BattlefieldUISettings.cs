using System;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using MCM.Common;

namespace BattlefieldUI.Settings
{
	// Token: 0x0200000B RID: 11
	public sealed class BattlefieldUISettings : AttributeGlobalSettings<BattlefieldUISettings>
	{
		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000069 RID: 105 RVA: 0x00003BE8 File Offset: 0x00001DE8
		public static BattlefieldUISettings Current
		{
			get
			{
				BattlefieldUISettings battlefieldUISettings = GlobalSettings<BattlefieldUISettings>.Instance ?? BattlefieldUISettings.Defaults;
				if (string.Equals(battlefieldUISettings.FriendlyDamageColor, "#FF6B6BFF", StringComparison.OrdinalIgnoreCase))
				{
					battlefieldUISettings.FriendlyDamageColor = "#FF4D4DFF";
				}
				return battlefieldUISettings;
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600006A RID: 106 RVA: 0x00003C23 File Offset: 0x00001E23
		public override string Id
		{
			get
			{
				return "BattlefieldUI_v1";
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600006B RID: 107 RVA: 0x00003C2A File Offset: 0x00001E2A
		public override string DisplayName
		{
			get
			{
				return "战场UI";
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600006C RID: 108 RVA: 0x00003C31 File Offset: 0x00001E31
		public override string FolderName
		{
			get
			{
				return "BattlefieldUI";
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x0600006D RID: 109 RVA: 0x00003C38 File Offset: 0x00001E38
		public override string FormatType
		{
			get
			{
				return "json2";
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x0600006E RID: 110 RVA: 0x00003C3F File Offset: 0x00001E3F
		// (set) Token: 0x0600006F RID: 111 RVA: 0x00003C47 File Offset: 0x00001E47
		[SettingPropertyBool("显示主角血条", RequireRestart = false)]
		[SettingPropertyGroup("显示规则")]
		public bool ShowMainAgent { get; set; } = true;

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000070 RID: 112 RVA: 0x00003C50 File Offset: 0x00001E50
		// (set) Token: 0x06000071 RID: 113 RVA: 0x00003C58 File Offset: 0x00001E58
		[SettingPropertyBool("显示我方单位血条", RequireRestart = false)]
		[SettingPropertyGroup("显示规则")]
		public bool ShowFriendlyAgents { get; set; } = true;

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000072 RID: 114 RVA: 0x00003C61 File Offset: 0x00001E61
		// (set) Token: 0x06000073 RID: 115 RVA: 0x00003C69 File Offset: 0x00001E69
		[SettingPropertyBool("显示敌方单位血条", RequireRestart = false)]
		[SettingPropertyGroup("显示规则")]
		public bool ShowEnemyAgents { get; set; } = true;

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000074 RID: 116 RVA: 0x00003C72 File Offset: 0x00001E72
		// (set) Token: 0x06000075 RID: 117 RVA: 0x00003C7A File Offset: 0x00001E7A
		[SettingPropertyBool("显示英雄 NPC 姓名", RequireRestart = false)]
		[SettingPropertyGroup("显示规则")]
		public bool ShowHeroNames { get; set; } = true;

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x06000076 RID: 118 RVA: 0x00003C83 File Offset: 0x00001E83
		// (set) Token: 0x06000077 RID: 119 RVA: 0x00003C8B File Offset: 0x00001E8B
		[SettingPropertyDropdown("血条显示时机", RequireRestart = false)]
		[SettingPropertyGroup("显示规则")]
		public Dropdown<string> DisplayMode { get; set; } = new Dropdown<string>(new string[] { "常驻显示", "仅受伤时显示" }, 1);

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x06000078 RID: 120 RVA: 0x00003C94 File Offset: 0x00001E94
		// (set) Token: 0x06000079 RID: 121 RVA: 0x00003C9C File Offset: 0x00001E9C
		[SettingPropertyText("我方血条颜色", -1, true, "", RequireRestart = false)]
		[SettingPropertyGroup("外观")]
		public string FriendlyColor { get; set; } = "#49B86EFF";

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x0600007A RID: 122 RVA: 0x00003CA5 File Offset: 0x00001EA5
		// (set) Token: 0x0600007B RID: 123 RVA: 0x00003CAD File Offset: 0x00001EAD
		[SettingPropertyText("敌方血条颜色", -1, true, "", RequireRestart = false)]
		[SettingPropertyGroup("外观")]
		public string EnemyColor { get; set; } = "#D9534FFF";

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x0600007C RID: 124 RVA: 0x00003CB6 File Offset: 0x00001EB6
		// (set) Token: 0x0600007D RID: 125 RVA: 0x00003CBE File Offset: 0x00001EBE
		[SettingPropertyText("血条背景颜色", -1, true, "", RequireRestart = false)]
		[SettingPropertyGroup("外观")]
		public string BackgroundColor { get; set; } = "#181818CC";

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x0600007E RID: 126 RVA: 0x00003CC7 File Offset: 0x00001EC7
		// (set) Token: 0x0600007F RID: 127 RVA: 0x00003CCF File Offset: 0x00001ECF
		[SettingPropertyInteger("整体透明度", 10, 100, "0%", RequireRestart = false)]
		[SettingPropertyGroup("外观")]
		public int OpacityPercent { get; set; } = 85;

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x06000080 RID: 128 RVA: 0x00003CD8 File Offset: 0x00001ED8
		// (set) Token: 0x06000081 RID: 129 RVA: 0x00003CE0 File Offset: 0x00001EE0
		[SettingPropertyInteger("血条基础长度", 24, 100, "0 像素", RequireRestart = false)]
		[SettingPropertyGroup("外观")]
		public int HealthBarWidth { get; set; } = 45;

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x06000082 RID: 130 RVA: 0x00003CE9 File Offset: 0x00001EE9
		// (set) Token: 0x06000083 RID: 131 RVA: 0x00003CF1 File Offset: 0x00001EF1
		[SettingPropertyInteger("血条基础厚度", 4, 16, "0 像素", RequireRestart = false)]
		[SettingPropertyGroup("外观")]
		public int HealthBarHeight { get; set; } = 8;

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000084 RID: 132 RVA: 0x00003CFA File Offset: 0x00001EFA
		// (set) Token: 0x06000085 RID: 133 RVA: 0x00003D02 File Offset: 0x00001F02
		[SettingPropertyInteger("血条横向位置", -100, 100, "0 像素", RequireRestart = false)]
		[SettingPropertyGroup("外观")]
		public int HealthBarOffsetX { get; set; }

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000086 RID: 134 RVA: 0x00003D0B File Offset: 0x00001F0B
		// (set) Token: 0x06000087 RID: 135 RVA: 0x00003D13 File Offset: 0x00001F13
		[SettingPropertyInteger("血条纵向位置", -60, 60, "0 像素", RequireRestart = false)]
		[SettingPropertyGroup("外观")]
		public int HealthBarOffsetY { get; set; }

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000088 RID: 136 RVA: 0x00003D1C File Offset: 0x00001F1C
		// (set) Token: 0x06000089 RID: 137 RVA: 0x00003D24 File Offset: 0x00001F24
		[SettingPropertyDropdown("血条圆角尺寸", RequireRestart = false)]
		[SettingPropertyGroup("外观")]
		public Dropdown<string> HealthBarCornerStyle { get; set; } = new Dropdown<string>(new string[] { "直角", "小圆角", "大圆角" }, 2);

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x0600008A RID: 138 RVA: 0x00003D2D File Offset: 0x00001F2D
		// (set) Token: 0x0600008B RID: 139 RVA: 0x00003D35 File Offset: 0x00001F35
		[SettingPropertyFloatingInteger("标记头顶高度", 0.1f, 1.5f, "0.0 米", RequireRestart = false)]
		[SettingPropertyGroup("外观")]
		public float HeightOffset { get; set; } = 0.45f;

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x0600008C RID: 140 RVA: 0x00003D3E File Offset: 0x00001F3E
		// (set) Token: 0x0600008D RID: 141 RVA: 0x00003D46 File Offset: 0x00001F46
		[SettingPropertyInteger("最大显示距离", 10, 300, "0 米", RequireRestart = false)]
		[SettingPropertyGroup("性能")]
		public int MaximumDistance { get; set; } = 80;

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x0600008E RID: 142 RVA: 0x00003D4F File Offset: 0x00001F4F
		// (set) Token: 0x0600008F RID: 143 RVA: 0x00003D57 File Offset: 0x00001F57
		[SettingPropertyInteger("距离淡出起点", 5, 250, "0 米", RequireRestart = false)]
		[SettingPropertyGroup("性能")]
		public int FadeStartDistance { get; set; } = 50;

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000090 RID: 144 RVA: 0x00003D60 File Offset: 0x00001F60
		// (set) Token: 0x06000091 RID: 145 RVA: 0x00003D68 File Offset: 0x00001F68
		[SettingPropertyInteger("最大同时显示数量", 25, 500, "0", RequireRestart = false)]
		[SettingPropertyGroup("性能")]
		public int MaximumVisibleBars { get; set; } = 200;

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x06000092 RID: 146 RVA: 0x00003D71 File Offset: 0x00001F71
		// (set) Token: 0x06000093 RID: 147 RVA: 0x00003D79 File Offset: 0x00001F79
		[SettingPropertyFloatingInteger("目标刷新间隔", 0.05f, 0.5f, "0.00 秒", RequireRestart = false)]
		[SettingPropertyGroup("性能")]
		public float RefreshInterval { get; set; } = 0.1f;

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x06000094 RID: 148 RVA: 0x00003D82 File Offset: 0x00001F82
		// (set) Token: 0x06000095 RID: 149 RVA: 0x00003D8A File Offset: 0x00001F8A
		[SettingPropertyBool("启用伤害飘字", RequireRestart = false)]
		[SettingPropertyGroup("伤害飘字/显示规则")]
		public bool ShowDamageNumbers { get; set; } = true;

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x06000096 RID: 150 RVA: 0x00003D93 File Offset: 0x00001F93
		// (set) Token: 0x06000097 RID: 151 RVA: 0x00003D9B File Offset: 0x00001F9B
		[SettingPropertyBool("显示主角受到的伤害", RequireRestart = false)]
		[SettingPropertyGroup("伤害飘字/显示规则")]
		public bool ShowMainAgentDamageNumbers { get; set; } = true;

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x06000098 RID: 152 RVA: 0x00003DA4 File Offset: 0x00001FA4
		// (set) Token: 0x06000099 RID: 153 RVA: 0x00003DAC File Offset: 0x00001FAC
		[SettingPropertyBool("显示我方单位受到的伤害", RequireRestart = false)]
		[SettingPropertyGroup("伤害飘字/显示规则")]
		public bool ShowFriendlyDamageNumbers { get; set; } = true;

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x0600009A RID: 154 RVA: 0x00003DB5 File Offset: 0x00001FB5
		// (set) Token: 0x0600009B RID: 155 RVA: 0x00003DBD File Offset: 0x00001FBD
		[SettingPropertyBool("显示敌方单位受到的伤害", RequireRestart = false)]
		[SettingPropertyGroup("伤害飘字/显示规则")]
		public bool ShowEnemyDamageNumbers { get; set; } = true;

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x0600009C RID: 156 RVA: 0x00003DC6 File Offset: 0x00001FC6
		// (set) Token: 0x0600009D RID: 157 RVA: 0x00003DCE File Offset: 0x00001FCE
		[SettingPropertyText("我方受伤飘字颜色（敌方命中）", -1, true, "", RequireRestart = false)]
		[SettingPropertyGroup("伤害飘字/外观")]
		public string FriendlyDamageColor { get; set; } = "#FF4D4DFF";

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x0600009E RID: 158 RVA: 0x00003DD7 File Offset: 0x00001FD7
		// (set) Token: 0x0600009F RID: 159 RVA: 0x00003DDF File Offset: 0x00001FDF
		[SettingPropertyText("敌方受伤飘字颜色", -1, true, "", RequireRestart = false)]
		[SettingPropertyGroup("伤害飘字/外观")]
		public string EnemyDamageColor { get; set; } = "#FFD166FF";

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x060000A0 RID: 160 RVA: 0x00003DE8 File Offset: 0x00001FE8
		// (set) Token: 0x060000A1 RID: 161 RVA: 0x00003DF0 File Offset: 0x00001FF0
		[SettingPropertyInteger("飘字字号", 12, 40, "0", RequireRestart = false)]
		[SettingPropertyGroup("伤害飘字/外观")]
		public int DamageNumberFontSize { get; set; } = 22;

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x060000A2 RID: 162 RVA: 0x00003DF9 File Offset: 0x00001FF9
		// (set) Token: 0x060000A3 RID: 163 RVA: 0x00003E01 File Offset: 0x00002001
		[SettingPropertyInteger("上浮距离", 10, 120, "0 像素", RequireRestart = false)]
		[SettingPropertyGroup("伤害飘字/外观")]
		public int DamageNumberRiseDistance { get; set; } = 45;

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060000A4 RID: 164 RVA: 0x00003E0A File Offset: 0x0000200A
		// (set) Token: 0x060000A5 RID: 165 RVA: 0x00003E12 File Offset: 0x00002012
		[SettingPropertyFloatingInteger("显示时间", 0.3f, 2f, "0.00 秒", RequireRestart = false)]
		[SettingPropertyGroup("伤害飘字/性能")]
		public float DamageNumberLifetime { get; set; } = 0.85f;

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060000A6 RID: 166 RVA: 0x00003E1B File Offset: 0x0000201B
		// (set) Token: 0x060000A7 RID: 167 RVA: 0x00003E23 File Offset: 0x00002023
		[SettingPropertyFloatingInteger("多段伤害合并窗口", 0f, 0.3f, "0.00 秒", RequireRestart = false)]
		[SettingPropertyGroup("伤害飘字/性能")]
		public float DamageNumberMergeWindow { get; set; } = 0.08f;

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060000A8 RID: 168 RVA: 0x00003E2C File Offset: 0x0000202C
		// (set) Token: 0x060000A9 RID: 169 RVA: 0x00003E34 File Offset: 0x00002034
		[SettingPropertyInteger("飘字最大显示距离", 10, 200, "0 米", RequireRestart = false)]
		[SettingPropertyGroup("伤害飘字/性能")]
		public int DamageNumberMaximumDistance { get; set; } = 80;

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060000AA RID: 170 RVA: 0x00003E3D File Offset: 0x0000203D
		// (set) Token: 0x060000AB RID: 171 RVA: 0x00003E45 File Offset: 0x00002045
		[SettingPropertyInteger("最大同时飘字数量", 20, 300, "0", RequireRestart = false)]
		[SettingPropertyGroup("伤害飘字/性能")]
		public int MaximumActiveDamageNumbers { get; set; } = 120;

		// Token: 0x0400002F RID: 47
		private static readonly BattlefieldUISettings Defaults = new BattlefieldUISettings();
	}
}
