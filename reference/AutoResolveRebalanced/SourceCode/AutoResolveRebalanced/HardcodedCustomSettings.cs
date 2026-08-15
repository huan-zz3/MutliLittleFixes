using System;

namespace AutoResolveRebalanced
{
	// Token: 0x0200000C RID: 12
	public class HardcodedCustomSettings : ICustomSettingsProvider
	{
		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000048 RID: 72 RVA: 0x00002C53 File Offset: 0x00000E53
		// (set) Token: 0x06000049 RID: 73 RVA: 0x00002C5B File Offset: 0x00000E5B
		public float damageModifier { get; set; } = DefVal.damageModifier;

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600004A RID: 74 RVA: 0x00002C64 File Offset: 0x00000E64
		// (set) Token: 0x0600004B RID: 75 RVA: 0x00002C6C File Offset: 0x00000E6C
		public bool armorEnabled { get; set; } = DefVal.armorEnabled;

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600004C RID: 76 RVA: 0x00002C75 File Offset: 0x00000E75
		// (set) Token: 0x0600004D RID: 77 RVA: 0x00002C7D File Offset: 0x00000E7D
		public bool weaponEnabled { get; set; } = DefVal.weaponEnabled;

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600004E RID: 78 RVA: 0x00002C86 File Offset: 0x00000E86
		// (set) Token: 0x0600004F RID: 79 RVA: 0x00002C8E File Offset: 0x00000E8E
		public bool aiEnabled { get; set; } = DefVal.aiEnabled;

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000050 RID: 80 RVA: 0x00002C97 File Offset: 0x00000E97
		// (set) Token: 0x06000051 RID: 81 RVA: 0x00002C9F File Offset: 0x00000E9F
		public float defModifierPct { get; set; } = DefVal.defModifierPct;

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000052 RID: 82 RVA: 0x00002CA8 File Offset: 0x00000EA8
		// (set) Token: 0x06000053 RID: 83 RVA: 0x00002CB0 File Offset: 0x00000EB0
		public bool shieldEnabled { get; set; } = DefVal.shieldEnabled;

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000054 RID: 84 RVA: 0x00002CB9 File Offset: 0x00000EB9
		// (set) Token: 0x06000055 RID: 85 RVA: 0x00002CC1 File Offset: 0x00000EC1
		public float shieldMultiplierPct { get; set; } = DefVal.shieldMultiplierPct;

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000056 RID: 86 RVA: 0x00002CCA File Offset: 0x00000ECA
		// (set) Token: 0x06000057 RID: 87 RVA: 0x00002CD2 File Offset: 0x00000ED2
		public float twoHandedBonusPct { get; set; } = DefVal.twoHandedBonusPct;

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000058 RID: 88 RVA: 0x00002CDB File Offset: 0x00000EDB
		// (set) Token: 0x06000059 RID: 89 RVA: 0x00002CE3 File Offset: 0x00000EE3
		public float rangedBonusPct { get; set; } = DefVal.rangedBonusPct;

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x0600005A RID: 90 RVA: 0x00002CEC File Offset: 0x00000EEC
		// (set) Token: 0x0600005B RID: 91 RVA: 0x00002CF4 File Offset: 0x00000EF4
		public float polearmBonusPct { get; set; } = DefVal.polearmBonusPct;

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x0600005C RID: 92 RVA: 0x00002CFD File Offset: 0x00000EFD
		// (set) Token: 0x0600005D RID: 93 RVA: 0x00002D05 File Offset: 0x00000F05
		public bool showLog { get; set; } = DefVal.showLog;

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x0600005E RID: 94 RVA: 0x00002D0E File Offset: 0x00000F0E
		// (set) Token: 0x0600005F RID: 95 RVA: 0x00002D16 File Offset: 0x00000F16
		public bool showError { get; set; } = DefVal.showError;

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x06000060 RID: 96 RVA: 0x00002D1F File Offset: 0x00000F1F
		// (set) Token: 0x06000061 RID: 97 RVA: 0x00002D27 File Offset: 0x00000F27
		public bool showWarn { get; set; } = DefVal.showWarn;
	}
}
