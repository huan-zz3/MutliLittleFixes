using System;

namespace AutoResolveRebalanced
{
	// Token: 0x0200000A RID: 10
	public interface ICustomSettingsProvider
	{
		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600002C RID: 44
		// (set) Token: 0x0600002D RID: 45
		float damageModifier { get; set; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600002E RID: 46
		// (set) Token: 0x0600002F RID: 47
		bool armorEnabled { get; set; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000030 RID: 48
		// (set) Token: 0x06000031 RID: 49
		bool weaponEnabled { get; set; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000032 RID: 50
		// (set) Token: 0x06000033 RID: 51
		bool aiEnabled { get; set; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000034 RID: 52
		// (set) Token: 0x06000035 RID: 53
		float defModifierPct { get; set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000036 RID: 54
		// (set) Token: 0x06000037 RID: 55
		bool shieldEnabled { get; set; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000038 RID: 56
		// (set) Token: 0x06000039 RID: 57
		float shieldMultiplierPct { get; set; }

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600003A RID: 58
		// (set) Token: 0x0600003B RID: 59
		float twoHandedBonusPct { get; set; }

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600003C RID: 60
		// (set) Token: 0x0600003D RID: 61
		float rangedBonusPct { get; set; }

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600003E RID: 62
		// (set) Token: 0x0600003F RID: 63
		float polearmBonusPct { get; set; }

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000040 RID: 64
		// (set) Token: 0x06000041 RID: 65
		bool showLog { get; set; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000042 RID: 66
		// (set) Token: 0x06000043 RID: 67
		bool showError { get; set; }

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000044 RID: 68
		// (set) Token: 0x06000045 RID: 69
		bool showWarn { get; set; }
	}
}
