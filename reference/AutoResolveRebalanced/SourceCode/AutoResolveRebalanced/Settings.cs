using System;
using MCM.Abstractions.Base.Global;

namespace AutoResolveRebalanced
{
	// Token: 0x02000009 RID: 9
	public class Settings
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000011 RID: 17 RVA: 0x00002A52 File Offset: 0x00000C52
		// (set) Token: 0x06000012 RID: 18 RVA: 0x00002A5F File Offset: 0x00000C5F
		public float damageModifier
		{
			get
			{
				return this._provider.damageModifier;
			}
			set
			{
				this._provider.damageModifier = value;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000013 RID: 19 RVA: 0x00002A6D File Offset: 0x00000C6D
		// (set) Token: 0x06000014 RID: 20 RVA: 0x00002A7A File Offset: 0x00000C7A
		public bool armorEnabled
		{
			get
			{
				return this._provider.armorEnabled;
			}
			set
			{
				this._provider.armorEnabled = value;
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000015 RID: 21 RVA: 0x00002A88 File Offset: 0x00000C88
		// (set) Token: 0x06000016 RID: 22 RVA: 0x00002A95 File Offset: 0x00000C95
		public bool weaponEnabled
		{
			get
			{
				return this._provider.weaponEnabled;
			}
			set
			{
				this._provider.weaponEnabled = value;
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000017 RID: 23 RVA: 0x00002AA3 File Offset: 0x00000CA3
		// (set) Token: 0x06000018 RID: 24 RVA: 0x00002AB0 File Offset: 0x00000CB0
		public bool aiEnabled
		{
			get
			{
				return this._provider.aiEnabled;
			}
			set
			{
				this._provider.aiEnabled = value;
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000019 RID: 25 RVA: 0x00002ABE File Offset: 0x00000CBE
		// (set) Token: 0x0600001A RID: 26 RVA: 0x00002ACB File Offset: 0x00000CCB
		public float defModifierPct
		{
			get
			{
				return this._provider.defModifierPct;
			}
			set
			{
				this._provider.defModifierPct = value;
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600001B RID: 27 RVA: 0x00002AD9 File Offset: 0x00000CD9
		// (set) Token: 0x0600001C RID: 28 RVA: 0x00002AE6 File Offset: 0x00000CE6
		public bool shieldEnabled
		{
			get
			{
				return this._provider.shieldEnabled;
			}
			set
			{
				this._provider.shieldEnabled = value;
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600001D RID: 29 RVA: 0x00002AF4 File Offset: 0x00000CF4
		// (set) Token: 0x0600001E RID: 30 RVA: 0x00002B01 File Offset: 0x00000D01
		public float twoHandedBonusPct
		{
			get
			{
				return this._provider.twoHandedBonusPct;
			}
			set
			{
				this._provider.twoHandedBonusPct = value;
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600001F RID: 31 RVA: 0x00002B0F File Offset: 0x00000D0F
		// (set) Token: 0x06000020 RID: 32 RVA: 0x00002B1C File Offset: 0x00000D1C
		public float rangedBonusPct
		{
			get
			{
				return this._provider.rangedBonusPct;
			}
			set
			{
				this._provider.rangedBonusPct = value;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000021 RID: 33 RVA: 0x00002B2A File Offset: 0x00000D2A
		// (set) Token: 0x06000022 RID: 34 RVA: 0x00002B37 File Offset: 0x00000D37
		public float polearmBonusPct
		{
			get
			{
				return this._provider.polearmBonusPct;
			}
			set
			{
				this._provider.polearmBonusPct = value;
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000023 RID: 35 RVA: 0x00002B45 File Offset: 0x00000D45
		// (set) Token: 0x06000024 RID: 36 RVA: 0x00002B52 File Offset: 0x00000D52
		public float shieldMultiplierPct
		{
			get
			{
				return this._provider.shieldMultiplierPct;
			}
			set
			{
				this._provider.shieldMultiplierPct = value;
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000025 RID: 37 RVA: 0x00002B60 File Offset: 0x00000D60
		// (set) Token: 0x06000026 RID: 38 RVA: 0x00002B6D File Offset: 0x00000D6D
		public bool showLog
		{
			get
			{
				return this._provider.showLog;
			}
			set
			{
				this._provider.showLog = value;
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000027 RID: 39 RVA: 0x00002B7B File Offset: 0x00000D7B
		// (set) Token: 0x06000028 RID: 40 RVA: 0x00002B88 File Offset: 0x00000D88
		public bool showError
		{
			get
			{
				return this._provider.showError;
			}
			set
			{
				this._provider.showError = value;
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000029 RID: 41 RVA: 0x00002B96 File Offset: 0x00000D96
		// (set) Token: 0x0600002A RID: 42 RVA: 0x00002BA3 File Offset: 0x00000DA3
		public bool showWarn
		{
			get
			{
				return this._provider.showWarn;
			}
			set
			{
				this._provider.showWarn = value;
			}
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002BB1 File Offset: 0x00000DB1
		public Settings()
		{
			if (GlobalSettings<CustomSettings>.Instance != null)
			{
				this._provider = GlobalSettings<CustomSettings>.Instance;
				return;
			}
			this._provider = new HardcodedCustomSettings();
		}

		// Token: 0x04000006 RID: 6
		private ICustomSettingsProvider _provider;
	}
}
