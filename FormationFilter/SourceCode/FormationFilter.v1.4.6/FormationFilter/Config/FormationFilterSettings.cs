using System;
using System.Runtime.CompilerServices;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using TaleWorlds.Localization;

namespace FormationFilter.Config
{
	// Token: 0x02000020 RID: 32
	[NullableContext(1)]
	[Nullable(new byte[] { 0, 1 })]
	public class FormationFilterSettings : AttributeGlobalSettings<FormationFilterSettings>
	{
		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06000128 RID: 296 RVA: 0x00008FFC File Offset: 0x000071FC
		public override string Id
		{
			get
			{
				return "FormationFilter";
			}
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x06000129 RID: 297 RVA: 0x00009003 File Offset: 0x00007203
		public override string DisplayName
		{
			get
			{
				return new TextObject("{=FormationFilter_formation_filter}Formation Filter", null).ToString();
			}
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x0600012A RID: 298 RVA: 0x00009015 File Offset: 0x00007215
		public override string FolderName
		{
			get
			{
				return "FormationFilter";
			}
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x0600012B RID: 299 RVA: 0x0000901C File Offset: 0x0000721C
		public override string FormatType
		{
			get
			{
				return "json";
			}
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x0600012C RID: 300 RVA: 0x00009023 File Offset: 0x00007223
		// (set) Token: 0x0600012D RID: 301 RVA: 0x0000902B File Offset: 0x0000722B
		[SettingPropertyBool("{=FormationFilter_assign_reinforcement_according_to_formation_filter}Assign reinforcement according to formation filter", Order = 0, RequireRestart = false)]
		public bool AssignReinforcementAccordingToFormationFilter
		{
			get
			{
				return this._assignReinforcementAccordingToFormationFilter;
			}
			set
			{
				this._assignReinforcementAccordingToFormationFilter = value;
				this.OnPropertyChanged("AssignReinforcementAccordingToFormationFilter");
			}
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x0600012E RID: 302 RVA: 0x0000903F File Offset: 0x0000723F
		// (set) Token: 0x0600012F RID: 303 RVA: 0x00009047 File Offset: 0x00007247
		[SettingPropertyBool("{=FormationFilter_treat_swing_polearm_as_two_handed_weapon}Treat swing polearm as two handed weapon", Order = 2, RequireRestart = false)]
		public bool TreatSwingPolearmAsTwoHandedWeapon
		{
			get
			{
				return this._treatSwingPolearmAsTwoHandedWeapon;
			}
			set
			{
				if (this._treatSwingPolearmAsTwoHandedWeapon == value)
				{
					return;
				}
				this._treatSwingPolearmAsTwoHandedWeapon = value;
				this.OnPropertyChanged("TreatSwingPolearmAsTwoHandedWeapon");
			}
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x06000130 RID: 304 RVA: 0x00009065 File Offset: 0x00007265
		// (set) Token: 0x06000131 RID: 305 RVA: 0x0000906D File Offset: 0x0000726D
		[SettingPropertyBool("{=FormationFilter_treat_throwing_spear_as_polearm}Treat throwing spear as polearm", Order = 3, RequireRestart = false)]
		public bool TreatThrowingSpearAsPolearm
		{
			get
			{
				return this._treatThrowingSpearAsPolearm;
			}
			set
			{
				if (this._treatThrowingSpearAsPolearm == value)
				{
					return;
				}
				this._treatThrowingSpearAsPolearm = value;
				this.OnPropertyChanged("TreatThrowingSpearAsPolearm");
			}
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x06000132 RID: 306 RVA: 0x0000908B File Offset: 0x0000728B
		// (set) Token: 0x06000133 RID: 307 RVA: 0x00009093 File Offset: 0x00007293
		[SettingPropertyBool("{=FormationFilter_treat_throwing_spear_as_throwing_weapon}Treat throwing spear as throwing weapon", Order = 4, RequireRestart = false)]
		public bool TreatThrowingSpearAsThrowingWeapon
		{
			get
			{
				return this._treatThrowingSpearAsThrowingWeapon;
			}
			set
			{
				if (this._treatThrowingSpearAsThrowingWeapon == value)
				{
					return;
				}
				this._treatThrowingSpearAsThrowingWeapon = value;
				this.OnPropertyChanged("TreatThrowingSpearAsThrowingWeapon");
			}
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x06000134 RID: 308 RVA: 0x000090B1 File Offset: 0x000072B1
		// (set) Token: 0x06000135 RID: 309 RVA: 0x000090B9 File Offset: 0x000072B9
		[SettingPropertyInteger("{=FormationFilter_high_tier_threshold}The lowest tier to be considered as high tier", 0, 7, "0", Order = 5)]
		public int HighTierThreshold
		{
			get
			{
				return this._highTierThreshold;
			}
			set
			{
				if (this._highTierThreshold == value)
				{
					return;
				}
				this._highTierThreshold = value;
				this.OnPropertyChanged("HighTierThreshold");
			}
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x06000136 RID: 310 RVA: 0x000090D7 File Offset: 0x000072D7
		// (set) Token: 0x06000137 RID: 311 RVA: 0x000090DF File Offset: 0x000072DF
		[SettingPropertyInteger("{=FormationFilter_low_tier_threshold}The hightest tier to be considered as low tier", 0, 7, "0", Order = 6)]
		public int LowTierThreshold
		{
			get
			{
				return this._lowTierThreshold;
			}
			set
			{
				if (this._lowTierThreshold == value)
				{
					return;
				}
				this._lowTierThreshold = value;
				this.OnPropertyChanged("LowTierThreshold");
			}
		}

		// Token: 0x04000089 RID: 137
		private bool _assignReinforcementAccordingToFormationFilter;

		// Token: 0x0400008A RID: 138
		private bool _treatSlingAsRangeWeapon;

		// Token: 0x0400008B RID: 139
		private bool _treatSwingPolearmAsTwoHandedWeapon;

		// Token: 0x0400008C RID: 140
		private bool _treatThrowingSpearAsPolearm = true;

		// Token: 0x0400008D RID: 141
		private bool _treatThrowingSpearAsThrowingWeapon = true;

		// Token: 0x0400008E RID: 142
		private int _highTierThreshold = 5;

		// Token: 0x0400008F RID: 143
		private int _lowTierThreshold = 2;
	}
}
