using System;
using Helpers;
using NavalDLC.CharacterDevelopment;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace NavalDLC.GameComponents
{
	// Token: 0x02000143 RID: 323
	public class NavalStrikeMagnitudeModel : StrikeMagnitudeCalculationModel
	{
		// Token: 0x0600156D RID: 5485 RVA: 0x0009638E File Offset: 0x0009458E
		public override float CalculateHorseArcheryFactor(BasicCharacterObject characterObject)
		{
			return base.BaseModel.CalculateHorseArcheryFactor(characterObject);
		}

		// Token: 0x0600156E RID: 5486 RVA: 0x0009639C File Offset: 0x0009459C
		public override float CalculateStrikeMagnitudeForMissile(in AttackInformation attackInformation, in AttackCollisionData collisionData, in MissionWeapon weapon, float missileSpeed)
		{
			return base.BaseModel.CalculateStrikeMagnitudeForMissile(ref attackInformation, ref collisionData, ref weapon, missileSpeed);
		}

		// Token: 0x0600156F RID: 5487 RVA: 0x000963AE File Offset: 0x000945AE
		public override float CalculateStrikeMagnitudeForSwing(in AttackInformation attackInformation, in AttackCollisionData collisionData, in MissionWeapon weapon, float swingSpeed, float impactPointAsPercent, float extraLinearSpeed)
		{
			return base.BaseModel.CalculateStrikeMagnitudeForSwing(ref attackInformation, ref collisionData, ref weapon, swingSpeed, impactPointAsPercent, extraLinearSpeed);
		}

		// Token: 0x06001570 RID: 5488 RVA: 0x000963C4 File Offset: 0x000945C4
		public override float CalculateStrikeMagnitudeForUnarmedAttack(in AttackInformation attackInformation, in AttackCollisionData collisionData, float progressEffect, float momentumRemaining)
		{
			return base.BaseModel.CalculateStrikeMagnitudeForUnarmedAttack(ref attackInformation, ref collisionData, progressEffect, momentumRemaining);
		}

		// Token: 0x06001571 RID: 5489 RVA: 0x000963D6 File Offset: 0x000945D6
		public override float CalculateStrikeMagnitudeForThrust(in AttackInformation attackInformation, in AttackCollisionData collisionData, in MissionWeapon weapon, float thrustWeaponSpeed, float extraLinearSpeed, bool isThrown = false)
		{
			return base.BaseModel.CalculateStrikeMagnitudeForThrust(ref attackInformation, ref collisionData, ref weapon, thrustWeaponSpeed, extraLinearSpeed, isThrown);
		}

		// Token: 0x06001572 RID: 5490 RVA: 0x000963EC File Offset: 0x000945EC
		public override float CalculateBaseBlowMagnitudeForPassiveUsage(in AttackInformation attackInformation, in AttackCollisionData collisionData, float extraLinearSpeed)
		{
			return base.BaseModel.CalculateBaseBlowMagnitudeForPassiveUsage(ref attackInformation, ref collisionData, extraLinearSpeed);
		}

		// Token: 0x06001573 RID: 5491 RVA: 0x000963FC File Offset: 0x000945FC
		public override float ComputeRawDamage(DamageTypes damageType, float magnitude, float armorEffectiveness, float absorbedDamageRatio)
		{
			return base.BaseModel.ComputeRawDamage(damageType, magnitude, armorEffectiveness, absorbedDamageRatio);
		}

		// Token: 0x06001574 RID: 5492 RVA: 0x0009640E File Offset: 0x0009460E
		public override float GetBluntDamageFactorByDamageType(DamageTypes damageType)
		{
			return base.BaseModel.GetBluntDamageFactorByDamageType(damageType);
		}

		// Token: 0x06001575 RID: 5493 RVA: 0x0009641C File Offset: 0x0009461C
		public override float CalculateAdjustedArmorForBlow(in AttackInformation attackInformation, in AttackCollisionData collisionData, float baseArmor, BasicCharacterObject attackerCharacter, BasicCharacterObject attackerCaptainCharacter, BasicCharacterObject victimCharacter, BasicCharacterObject victimCaptainCharacter, WeaponComponentData weaponComponent)
		{
			bool flag = false;
			float num = base.BaseModel.CalculateAdjustedArmorForBlow(ref attackInformation, ref collisionData, baseArmor, attackerCharacter, attackerCaptainCharacter, victimCharacter, victimCaptainCharacter, weaponComponent);
			CharacterObject characterObject = attackerCharacter as CharacterObject;
			CharacterObject characterObject2 = attackerCaptainCharacter as CharacterObject;
			if (attackerCharacter == characterObject2)
			{
				characterObject2 = null;
			}
			if (num > 0f && characterObject != null)
			{
				if (weaponComponent != null)
				{
					if (weaponComponent.RelevantSkill == DefaultSkills.Crossbow && baseArmor < DefaultPerks.Crossbow.Piercer.PrimaryBonus && characterObject.GetPerkValue(DefaultPerks.Crossbow.Piercer))
					{
						flag = true;
					}
					else if (weaponComponent.WeaponClass == 14)
					{
						AttackCollisionData attackCollisionData = collisionData;
						if (attackCollisionData.VictimHitBodyPart == null && characterObject.GetPerkValue(DefaultPerks.Throwing.SlingingCompetitions))
						{
							flag = true;
						}
					}
				}
				if (flag)
				{
					num = 0f;
				}
				else
				{
					ExplainedNumber explainedNumber;
					explainedNumber..ctor(baseArmor, false, null);
					PerkHelper.AddPerkBonusForCharacter(DefaultPerks.TwoHanded.Vandal, characterObject, true, ref explainedNumber, false);
					if (weaponComponent != null)
					{
						if (weaponComponent.RelevantSkill == DefaultSkills.OneHanded)
						{
							PerkHelper.AddPerkBonusForCharacter(DefaultPerks.OneHanded.ChinkInTheArmor, characterObject, true, ref explainedNumber, false);
						}
						else if (weaponComponent.RelevantSkill == DefaultSkills.Bow)
						{
							PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Bow.Bodkin, characterObject, true, ref explainedNumber, false);
							if (characterObject2 != null)
							{
								PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Bow.Bodkin, characterObject2, ref explainedNumber);
							}
						}
						else if (weaponComponent.RelevantSkill == DefaultSkills.Crossbow)
						{
							PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Crossbow.Puncture, characterObject, true, ref explainedNumber, false);
							if (characterObject2 != null)
							{
								PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Crossbow.Puncture, characterObject2, ref explainedNumber);
							}
						}
						else if (weaponComponent.RelevantSkill == DefaultSkills.Throwing)
						{
							PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Throwing.WeakSpot, characterObject, true, ref explainedNumber, false);
							if (characterObject2 != null)
							{
								PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Throwing.WeakSpot, characterObject2, ref explainedNumber);
							}
						}
						if (weaponComponent.IsMeleeWeapon)
						{
							PerkHelper.AddPerkBonusForCharacter(NavalPerks.Mariner.ShatteringBlow, characterObject, true, ref explainedNumber, false);
							if (characterObject2 != null)
							{
								PerkHelper.AddPerkBonusFromCaptain(NavalPerks.Mariner.ShatteringBlow, characterObject2, ref explainedNumber);
							}
						}
						else if (weaponComponent.IsConsumable && weaponComponent.RelevantSkill != null)
						{
							PerkHelper.AddPerkBonusForCharacter(NavalPerks.Mariner.ShatteringVolley, characterObject, true, ref explainedNumber, false);
							if (characterObject2 != null)
							{
								PerkHelper.AddPerkBonusFromCaptain(NavalPerks.Mariner.ShatteringVolley, characterObject2, ref explainedNumber);
							}
						}
					}
					float num2 = explainedNumber.ResultNumber - baseArmor;
					num = MathF.Max(0f, baseArmor - num2);
				}
			}
			return num;
		}
	}
}
