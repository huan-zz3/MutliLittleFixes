using System;
using System.Runtime.CompilerServices;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using TaleWorlds.Localization;

namespace ProjectileTrajectorySystem
{
	// Token: 0x02000009 RID: 9
	[NullableContext(1)]
	[Nullable(new byte[] { 0, 1 })]
	public class PTSettings : AttributeGlobalSettings<PTSettings>
	{
		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000049 RID: 73 RVA: 0x00005D2D File Offset: 0x00003F2D
		public override string Id
		{
			get
			{
				return "CatapultGuide_Config_v1";
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600004A RID: 74 RVA: 0x00005D34 File Offset: 0x00003F34
		public override string DisplayName
		{
			get
			{
				return new TextObject("{=pt_mod_name}Catapult Guide", null).ToString();
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600004B RID: 75 RVA: 0x00005D46 File Offset: 0x00003F46
		public override string FolderName
		{
			get
			{
				return "CatapultGuide";
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600004C RID: 76 RVA: 0x00005D4D File Offset: 0x00003F4D
		public override string FormatType
		{
			get
			{
				return "json";
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600004D RID: 77 RVA: 0x00005D54 File Offset: 0x00003F54
		// (set) Token: 0x0600004E RID: 78 RVA: 0x00005D5C File Offset: 0x00003F5C
		[SettingPropertyBool("{=pt_handheld}Show handheld weapon trajectory", Order = 0, RequireRestart = false, HintText = "{=pt_handheld_hint}Enable or disable trajectory preview for bows, crossbows and thrown weapons.")]
		[SettingPropertyGroup("{=pt_group_handheld}Handheld weapons")]
		public bool EnableHandheld { get; set; } = true;

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600004F RID: 79 RVA: 0x00005D65 File Offset: 0x00003F65
		// (set) Token: 0x06000050 RID: 80 RVA: 0x00005D6D File Offset: 0x00003F6D
		[SettingPropertyBool("{=pt_ballista}Show ballista trajectory", Order = 1, RequireRestart = false, HintText = "{=pt_ballista_hint}Enable or disable trajectory preview for ballista and scorpion.")]
		[SettingPropertyGroup("{=pt_group_siege}Siege engines")]
		public bool EnableBallista { get; set; } = true;

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000051 RID: 81 RVA: 0x00005D76 File Offset: 0x00003F76
		// (set) Token: 0x06000052 RID: 82 RVA: 0x00005D7E File Offset: 0x00003F7E
		[SettingPropertyBool("{=pt_mangonel}Show mangonel trajectory", Order = 2, RequireRestart = false, HintText = "{=pt_mangonel_hint}Enable or disable trajectory preview for mangonel and trebuchet.")]
		[SettingPropertyGroup("{=pt_group_siege}Siege engines")]
		public bool EnableMangonel { get; set; } = true;

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000053 RID: 83 RVA: 0x00005D87 File Offset: 0x00003F87
		// (set) Token: 0x06000054 RID: 84 RVA: 0x00005D8F File Offset: 0x00003F8F
		[SettingPropertyBool("{=pt_naval}Enable naval auto-aim", Order = 4, RequireRestart = false, HintText = "{=pt_naval_hint}Allow automatic target snapping with middle mouse button when using ballista in naval battles.")]
		[SettingPropertyGroup("{=pt_group_naval}Naval assistance")]
		public bool EnableNavalAutoAim { get; set; } = true;
	}
}
