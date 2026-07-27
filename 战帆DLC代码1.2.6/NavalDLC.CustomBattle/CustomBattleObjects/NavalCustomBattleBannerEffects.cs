using System;
using TaleWorlds.Core;

namespace NavalDLC.CustomBattle.CustomBattleObjects
{
	// Token: 0x02000027 RID: 39
	public class NavalCustomBattleBannerEffects
	{
		// Token: 0x170000BF RID: 191
		// (get) Token: 0x0600021B RID: 539 RVA: 0x0000983D File Offset: 0x00007A3D
		private static NavalCustomBattleBannerEffects Instance
		{
			get
			{
				return NavalCustomGame.Current.NavalCustomBattleBannerEffects;
			}
		}

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x0600021C RID: 540 RVA: 0x00009849 File Offset: 0x00007A49
		public static BannerEffect IncreasedMeleeDamage
		{
			get
			{
				return NavalCustomBattleBannerEffects.Instance._increasedMeleeDamage;
			}
		}

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x0600021D RID: 541 RVA: 0x00009855 File Offset: 0x00007A55
		public static BannerEffect IncreasedMeleeDamageAgainstMountedTroops
		{
			get
			{
				return NavalCustomBattleBannerEffects.Instance._increasedMeleeDamageAgainstMountedTroops;
			}
		}

		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x0600021E RID: 542 RVA: 0x00009861 File Offset: 0x00007A61
		public static BannerEffect IncreasedRangedDamage
		{
			get
			{
				return NavalCustomBattleBannerEffects.Instance._increasedRangedDamage;
			}
		}

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x0600021F RID: 543 RVA: 0x0000986D File Offset: 0x00007A6D
		public static BannerEffect IncreasedChargeDamage
		{
			get
			{
				return NavalCustomBattleBannerEffects.Instance._increasedChargeDamage;
			}
		}

		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x06000220 RID: 544 RVA: 0x00009879 File Offset: 0x00007A79
		public static BannerEffect DecreasedRangedWeaponAccuracy
		{
			get
			{
				return NavalCustomBattleBannerEffects.Instance._decreasedRangedAccuracyPenalty;
			}
		}

		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x06000221 RID: 545 RVA: 0x00009885 File Offset: 0x00007A85
		public static BannerEffect DecreasedMoraleShock
		{
			get
			{
				return NavalCustomBattleBannerEffects.Instance._decreasedMoraleShock;
			}
		}

		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x06000222 RID: 546 RVA: 0x00009891 File Offset: 0x00007A91
		public static BannerEffect DecreasedMeleeAttackDamage
		{
			get
			{
				return NavalCustomBattleBannerEffects.Instance._decreasedMeleeAttackDamage;
			}
		}

		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x06000223 RID: 547 RVA: 0x0000989D File Offset: 0x00007A9D
		public static BannerEffect DecreasedRangedAttackDamage
		{
			get
			{
				return NavalCustomBattleBannerEffects.Instance._decreasedRangedAttackDamage;
			}
		}

		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x06000224 RID: 548 RVA: 0x000098A9 File Offset: 0x00007AA9
		public static BannerEffect DecreasedShieldDamage
		{
			get
			{
				return NavalCustomBattleBannerEffects.Instance._decreasedShieldDamage;
			}
		}

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x06000225 RID: 549 RVA: 0x000098B5 File Offset: 0x00007AB5
		public static BannerEffect IncreasedTroopMovementSpeed
		{
			get
			{
				return NavalCustomBattleBannerEffects.Instance._increasedTroopMovementSpeed;
			}
		}

		// Token: 0x170000CA RID: 202
		// (get) Token: 0x06000226 RID: 550 RVA: 0x000098C1 File Offset: 0x00007AC1
		public static BannerEffect IncreasedMountMovementSpeed
		{
			get
			{
				return NavalCustomBattleBannerEffects.Instance._increasedMountMovementSpeed;
			}
		}

		// Token: 0x06000227 RID: 551 RVA: 0x000098CD File Offset: 0x00007ACD
		public NavalCustomBattleBannerEffects()
		{
			this.RegisterAll();
		}

		// Token: 0x06000228 RID: 552 RVA: 0x000098DC File Offset: 0x00007ADC
		private void RegisterAll()
		{
			this._increasedMeleeDamage = this.Create("IncreasedMeleeDamage");
			this._increasedMeleeDamageAgainstMountedTroops = this.Create("IncreasedMeleeDamageAgainstMountedTroops");
			this._increasedRangedDamage = this.Create("IncreasedRangedDamage");
			this._increasedChargeDamage = this.Create("IncreasedChargeDamage");
			this._decreasedRangedAccuracyPenalty = this.Create("DecreasedRangedAccuracyPenalty");
			this._decreasedMoraleShock = this.Create("DecreasedMoraleShock");
			this._decreasedMeleeAttackDamage = this.Create("DecreasedMeleeAttackDamage");
			this._decreasedRangedAttackDamage = this.Create("DecreasedRangedAttackDamage");
			this._decreasedShieldDamage = this.Create("DecreasedShieldDamage");
			this._increasedTroopMovementSpeed = this.Create("IncreasedTroopMovementSpeed");
			this._increasedMountMovementSpeed = this.Create("IncreasedMountMovementSpeed");
			this.InitializeAll();
		}

		// Token: 0x06000229 RID: 553 RVA: 0x000099AA File Offset: 0x00007BAA
		private BannerEffect Create(string stringId)
		{
			return Game.Current.ObjectManager.RegisterPresumedObject<BannerEffect>(new BannerEffect(stringId));
		}

		// Token: 0x0600022A RID: 554 RVA: 0x000099C4 File Offset: 0x00007BC4
		private void InitializeAll()
		{
			this._increasedMeleeDamage.Initialize("{=unaWKloT}Increased Melee Damage", "{=8ZNOgT8Z}{BONUS_AMOUNT}% melee damage to troops in your formation.", 0.05f, 0.1f, 0.15f, 1);
			this._increasedMeleeDamageAgainstMountedTroops.Initialize("{=2bHoiaoe}Increased Damage Against Mounted Troops", "{=9RZLSV3E}{BONUS_AMOUNT}% damage by melee troops in your formation against cavalry.", 0.1f, 0.2f, 0.3f, 1);
			this._increasedRangedDamage.Initialize("{=Ch5NpCd0}Increased Ranged Damage", "{=labbKop6}{BONUS_AMOUNT}% ranged damage to troops in your formation.", 0.04f, 0.06f, 0.08f, 1);
			this._decreasedRangedAccuracyPenalty.Initialize("{=MkBPRCuF}Decreased Ranged Accuracy Penalty", "{=Gu0Wxxul}{BONUS_AMOUNT}% accuracy penalty for ranged troops in your formation.", -0.04f, -0.06f, -0.08f, 1);
			this._increasedChargeDamage.Initialize("{=O2oBC9sH}Increased Charge Damage", "{=Z2xgnrDa}{BONUS_AMOUNT}% charge damage to mounted troops in your formation.", 0.1f, 0.2f, 0.3f, 1);
			this._decreasedMoraleShock.Initialize("{=nOMT0Cw6}Decreased Morale Shock", "{=W0agPHes}{BONUS_AMOUNT}% morale penalty from casualties to troops in your formation.", -0.1f, -0.2f, -0.3f, 1);
			this._decreasedMeleeAttackDamage.Initialize("{=a3Vc59WV}Decreased Taken Melee Attack Damage", "{=ORFrCYSn}{BONUS_AMOUNT}% damage by melee attacks to troops in your formation.", -0.05f, -0.1f, -0.15f, 1);
			this._decreasedRangedAttackDamage.Initialize("{=p0JFbL7G}Decreased Taken Ranged Attack Damage", "{=W0agPHes}{BONUS_AMOUNT}% morale penalty from casualties to troops in your formation.", -0.05f, -0.1f, -0.15f, 1);
			this._decreasedShieldDamage.Initialize("{=T79exjaP}Decreased Taken Shield Damage", "{=klGEDUmw}{BONUS_AMOUNT}% damage to shields of troops in your formation.", -0.15f, -0.25f, -0.3f, 1);
			this._increasedTroopMovementSpeed.Initialize("{=PbJAOKKZ}Increased Troop Movement Speed", "{=nqWulUTP}{BONUS_AMOUNT}% movement speed to infantry in your formation.", 0.15f, 0.25f, 0.3f, 1);
			this._increasedMountMovementSpeed.Initialize("{=nMfxbc0Y}Increased Mount Movement Speed", "{=g0l7W5xQ}{BONUS_AMOUNT}% movement speed to mounts in your formation.", 0.05f, 0.08f, 0.1f, 1);
		}

		// Token: 0x04000100 RID: 256
		private BannerEffect _increasedMeleeDamage;

		// Token: 0x04000101 RID: 257
		private BannerEffect _increasedMeleeDamageAgainstMountedTroops;

		// Token: 0x04000102 RID: 258
		private BannerEffect _increasedRangedDamage;

		// Token: 0x04000103 RID: 259
		private BannerEffect _increasedChargeDamage;

		// Token: 0x04000104 RID: 260
		private BannerEffect _decreasedRangedAccuracyPenalty;

		// Token: 0x04000105 RID: 261
		private BannerEffect _decreasedMoraleShock;

		// Token: 0x04000106 RID: 262
		private BannerEffect _decreasedMeleeAttackDamage;

		// Token: 0x04000107 RID: 263
		private BannerEffect _decreasedRangedAttackDamage;

		// Token: 0x04000108 RID: 264
		private BannerEffect _decreasedShieldDamage;

		// Token: 0x04000109 RID: 265
		private BannerEffect _increasedTroopMovementSpeed;

		// Token: 0x0400010A RID: 266
		private BannerEffect _increasedMountMovementSpeed;
	}
}
