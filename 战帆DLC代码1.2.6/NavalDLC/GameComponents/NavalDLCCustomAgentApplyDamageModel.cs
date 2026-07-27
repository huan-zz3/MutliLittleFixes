using System;
using MBHelpers;
using NavalDLC.Missions.Objects.UsableMachines;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace NavalDLC.GameComponents
{
	// Token: 0x02000116 RID: 278
	public class NavalDLCCustomAgentApplyDamageModel : AgentApplyDamageModel
	{
		// Token: 0x060013D6 RID: 5078 RVA: 0x0008EED3 File Offset: 0x0008D0D3
		public override bool IsDamageIgnored(in AttackInformation attackInformation, in AttackCollisionData collisionData)
		{
			return false;
		}

		// Token: 0x060013D7 RID: 5079 RVA: 0x0008EED8 File Offset: 0x0008D0D8
		public override float ApplyDamageAmplifications(in AttackInformation attackInformation, in AttackCollisionData collisionData, float baseDamage)
		{
			bool flag = (attackInformation.IsAttackerAgentMount ? attackInformation.AttackerRiderAgentCharacter : attackInformation.AttackerAgentCharacter) != null;
			Formation attackerFormation = attackInformation.AttackerFormation;
			BannerComponent activeBanner = MissionGameModels.Current.BattleBannerBearersModel.GetActiveBanner(attackerFormation);
			bool isVictimAgentMount = attackInformation.IsVictimAgentMount;
			Formation victimFormation = attackInformation.VictimFormation;
			BannerComponent activeBanner2 = MissionGameModels.Current.BattleBannerBearersModel.GetActiveBanner(victimFormation);
			FactoredNumber factoredNumber;
			factoredNumber..ctor(baseDamage);
			MissionWeapon attackerWeapon = attackInformation.AttackerWeapon;
			WeaponComponentData currentUsageItem = attackerWeapon.CurrentUsageItem;
			if (flag)
			{
				if (currentUsageItem != null)
				{
					if (currentUsageItem.IsMeleeWeapon)
					{
						if (activeBanner != null)
						{
							BannerHelper.AddBannerBonusForBanner(DefaultBannerEffects.IncreasedMeleeDamage, activeBanner, ref factoredNumber);
							if (attackInformation.DoesVictimHaveMountAgent)
							{
								BannerHelper.AddBannerBonusForBanner(DefaultBannerEffects.IncreasedMeleeDamageAgainstMountedTroops, activeBanner, ref factoredNumber);
							}
						}
					}
					else if (currentUsageItem.IsConsumable && activeBanner != null)
					{
						BannerHelper.AddBannerBonusForBanner(DefaultBannerEffects.IncreasedRangedDamage, activeBanner, ref factoredNumber);
					}
				}
				AttackCollisionData attackCollisionData = collisionData;
				if (attackCollisionData.IsHorseCharge)
				{
					if (activeBanner != null)
					{
						BannerHelper.AddBannerBonusForBanner(DefaultBannerEffects.IncreasedChargeDamage, activeBanner, ref factoredNumber);
					}
					if (activeBanner2 != null)
					{
						BannerHelper.AddBannerBonusForBanner(DefaultBannerEffects.DecreasedChargeDamage, activeBanner2, ref factoredNumber);
					}
				}
			}
			return factoredNumber.ResultNumber;
		}

		// Token: 0x060013D8 RID: 5080 RVA: 0x0008EFD4 File Offset: 0x0008D1D4
		public override float ApplyDamageScaling(in AttackInformation attackInformation, in AttackCollisionData collisionData, float baseDamage)
		{
			float num = 1f;
			if (Mission.Current.IsSallyOutBattle)
			{
				DestructableComponent hitObjectDestructibleComponent = attackInformation.HitObjectDestructibleComponent;
				if (hitObjectDestructibleComponent != null && hitObjectDestructibleComponent.GameEntity.GetFirstScriptOfType<SiegeWeapon>() != null)
				{
					num *= 4.5f;
				}
			}
			return baseDamage * num;
		}

		// Token: 0x060013D9 RID: 5081 RVA: 0x0008F01C File Offset: 0x0008D21C
		public override float ApplyDamageReductions(in AttackInformation attackInformation, in AttackCollisionData collisionData, float baseDamage)
		{
			Agent agent = (attackInformation.IsVictimAgentMount ? attackInformation.VictimAgent.RiderAgent : attackInformation.VictimAgent);
			bool flag = (attackInformation.IsVictimAgentMount ? attackInformation.VictimRiderAgentCharacter : attackInformation.VictimAgentCharacter) != null;
			Formation victimFormation = attackInformation.VictimFormation;
			BannerComponent activeBanner = MissionGameModels.Current.BattleBannerBearersModel.GetActiveBanner(victimFormation);
			Agent agent2 = (attackInformation.IsAttackerAgentMount ? attackInformation.AttackerAgent.RiderAgent : attackInformation.AttackerAgent);
			FactoredNumber factoredNumber;
			factoredNumber..ctor(baseDamage);
			MissionWeapon attackerWeapon = attackInformation.AttackerWeapon;
			WeaponComponentData currentUsageItem = attackerWeapon.CurrentUsageItem;
			if (flag && currentUsageItem != null)
			{
				if (currentUsageItem.IsConsumable)
				{
					if (activeBanner != null)
					{
						BannerHelper.AddBannerBonusForBanner(DefaultBannerEffects.DecreasedRangedAttackDamage, activeBanner, ref factoredNumber);
					}
					if (Mission.Current.IsNavalBattle)
					{
						if (agent == Agent.Main && agent.CurrentlyUsedGameObject != null && agent.CurrentlyUsedGameObject.GetComponent<UserDamageCalculateComponent>() != null)
						{
							UserDamageCalculateComponent component = agent.CurrentlyUsedGameObject.GetComponent<UserDamageCalculateComponent>();
							factoredNumber.AddFactor(component.DamageReductionFactor);
						}
						if (agent2 != null && agent2.IsAIControlled && (currentUsageItem.WeaponClass == 13 || currentUsageItem.WeaponClass == 12))
						{
							factoredNumber.AddFactor(-0.2f);
						}
					}
				}
				else if (currentUsageItem.IsMeleeWeapon && activeBanner != null)
				{
					BannerHelper.AddBannerBonusForBanner(DefaultBannerEffects.DecreasedMeleeAttackDamage, activeBanner, ref factoredNumber);
				}
			}
			return factoredNumber.ResultNumber;
		}

		// Token: 0x060013DA RID: 5082 RVA: 0x0008F166 File Offset: 0x0008D366
		public override float ApplyGeneralDamageModifiers(in AttackInformation attackInformation, in AttackCollisionData collisionData, float baseDamage)
		{
			return baseDamage;
		}

		// Token: 0x060013DB RID: 5083 RVA: 0x0008F169 File Offset: 0x0008D369
		public override void DecideMissileWeaponFlags(Agent attackerAgent, in MissionWeapon missileWeapon, ref WeaponFlags missileWeaponFlags)
		{
		}

		// Token: 0x060013DC RID: 5084 RVA: 0x0008F16C File Offset: 0x0008D36C
		public override bool DecideCrushedThrough(Agent attackerAgent, Agent defenderAgent, float totalAttackEnergy, Agent.UsageDirection attackDirection, StrikeType strikeType, WeaponComponentData defendItem, bool isPassiveUsage)
		{
			EquipmentIndex equipmentIndex = attackerAgent.GetOffhandWieldedItemIndex();
			if (equipmentIndex == -1)
			{
				equipmentIndex = attackerAgent.GetPrimaryWieldedItemIndex();
			}
			WeaponComponentData weaponComponentData = ((equipmentIndex != -1) ? attackerAgent.Equipment[equipmentIndex].CurrentUsageItem : null);
			if (weaponComponentData == null || isPassiveUsage || !Extensions.HasAnyFlag<WeaponFlags>(weaponComponentData.WeaponFlags, 134217728L) || strikeType != null || attackDirection != null)
			{
				return false;
			}
			float num = 58f;
			if (defendItem != null && defendItem.IsShield)
			{
				num *= 1.2f;
			}
			return totalAttackEnergy > num;
		}

		// Token: 0x060013DD RID: 5085 RVA: 0x0008F1EC File Offset: 0x0008D3EC
		public override bool CanWeaponDealSneakAttack(in AttackInformation attackInformation, WeaponComponentData weapon)
		{
			if (weapon != null && (weapon.IsMeleeWeapon || weapon.WeaponClass == 22) && attackInformation.IsVictimAgentHuman && !attackInformation.IsVictimPlayer)
			{
				if ((attackInformation.VictimAgentAIStateFlags & 3) == null)
				{
					return true;
				}
				if (!Extensions.HasAllFlags<Agent.AIStateFlag>(attackInformation.VictimAgentAIStateFlags, 3) && !attackInformation.IsAttackerAgentNull && Vec2.DotProduct((attackInformation.AttackerAgentPosition - attackInformation.VictimAgentPosition).AsVec2.Normalized(), attackInformation.VictimAgentMovementDirection) < 0.174f)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060013DE RID: 5086 RVA: 0x0008F278 File Offset: 0x0008D478
		public override bool CanWeaponDismount(Agent attackerAgent, WeaponComponentData attackerWeapon, in Blow blow, in AttackCollisionData collisionData)
		{
			return MBMath.IsBetween(blow.VictimBodyPart, 0, 6) && ((!attackerAgent.HasMount && blow.StrikeType == null && Extensions.HasAnyFlag<WeaponFlags>(blow.WeaponRecord.WeaponFlags, 33554432L)) || (blow.StrikeType == 1 && Extensions.HasAnyFlag<WeaponFlags>(blow.WeaponRecord.WeaponFlags, 16777216L)));
		}

		// Token: 0x060013DF RID: 5087 RVA: 0x0008F2E1 File Offset: 0x0008D4E1
		public override void CalculateDefendedBlowStunMultipliers(Agent attackerAgent, Agent defenderAgent, CombatCollisionResult collisionResult, WeaponComponentData attackerWeapon, WeaponComponentData defenderWeapon, ref float attackerStunPeriod, ref float defenderStunPeriod)
		{
		}

		// Token: 0x060013E0 RID: 5088 RVA: 0x0008F2E4 File Offset: 0x0008D4E4
		public override bool CanWeaponKnockback(Agent attackerAgent, WeaponComponentData attackerWeapon, in Blow blow, in AttackCollisionData collisionData)
		{
			AttackCollisionData attackCollisionData = collisionData;
			return MBMath.IsBetween(attackCollisionData.VictimHitBodyPart, 0, 6) && !Extensions.HasAnyFlag<WeaponFlags>(attackerWeapon.WeaponFlags, 67108864L) && (attackerWeapon.IsConsumable || (blow.BlowFlag & 128) != null || (blow.StrikeType == 1 && Extensions.HasAnyFlag<WeaponFlags>(blow.WeaponRecord.WeaponFlags, 64L)));
		}

		// Token: 0x060013E1 RID: 5089 RVA: 0x0008F354 File Offset: 0x0008D554
		public override bool CanWeaponKnockDown(Agent attackerAgent, Agent victimAgent, WeaponComponentData attackerWeapon, in Blow blow, in AttackCollisionData collisionData)
		{
			if (attackerWeapon.WeaponClass == 20)
			{
				return true;
			}
			AttackCollisionData attackCollisionData = collisionData;
			BoneBodyPartType victimHitBodyPart = attackCollisionData.VictimHitBodyPart;
			bool flag = MBMath.IsBetween(victimHitBodyPart, 0, 6);
			if (!victimAgent.HasMount && victimHitBodyPart == 8)
			{
				flag = true;
			}
			return flag && Extensions.HasAnyFlag<WeaponFlags>(blow.WeaponRecord.WeaponFlags, 67108864L) && ((attackerWeapon.IsPolearm && blow.StrikeType == 1) || (attackerWeapon.IsMeleeWeapon && blow.StrikeType == null && MissionCombatMechanicsHelper.DecideSweetSpotCollision(ref collisionData)));
		}

		// Token: 0x060013E2 RID: 5090 RVA: 0x0008F3E0 File Offset: 0x0008D5E0
		public override float GetDismountPenetration(Agent attackerAgent, WeaponComponentData attackerWeapon, in Blow blow, in AttackCollisionData attackCollisionData)
		{
			float num = 0f;
			if (blow.StrikeType == null && Extensions.HasAnyFlag<WeaponFlags>(blow.WeaponRecord.WeaponFlags, 33554432L))
			{
				num += 0.25f;
			}
			return num;
		}

		// Token: 0x060013E3 RID: 5091 RVA: 0x0008F41C File Offset: 0x0008D61C
		public override float GetKnockBackPenetration(Agent attackerAgent, WeaponComponentData attackerWeapon, in Blow blow, in AttackCollisionData attackCollisionData)
		{
			return 0f;
		}

		// Token: 0x060013E4 RID: 5092 RVA: 0x0008F424 File Offset: 0x0008D624
		public override float GetKnockDownPenetration(Agent attackerAgent, WeaponComponentData attackerWeapon, in Blow blow, in AttackCollisionData attackCollisionData)
		{
			float num = 0f;
			if (attackerWeapon.WeaponClass == 20)
			{
				num += 0.25f;
			}
			else if (attackerWeapon.IsMeleeWeapon)
			{
				AttackCollisionData attackCollisionData2 = attackCollisionData;
				if (attackCollisionData2.VictimHitBodyPart == 8 && blow.StrikeType == null)
				{
					num += 0.1f;
				}
				else
				{
					attackCollisionData2 = attackCollisionData;
					if (attackCollisionData2.VictimHitBodyPart == null)
					{
						num += 0.15f;
					}
				}
			}
			return num;
		}

		// Token: 0x060013E5 RID: 5093 RVA: 0x0008F491 File Offset: 0x0008D691
		public override float GetHorseChargePenetration()
		{
			return 0.4f;
		}

		// Token: 0x060013E6 RID: 5094 RVA: 0x0008F498 File Offset: 0x0008D698
		public override float CalculateStaggerThresholdDamage(Agent defenderAgent, in Blow blow)
		{
			ManagedParametersEnum managedParametersEnum;
			if (blow.DamageType == null)
			{
				managedParametersEnum = 10;
			}
			else if (blow.DamageType == 1)
			{
				managedParametersEnum = 9;
			}
			else
			{
				managedParametersEnum = 11;
			}
			return ManagedParameters.Instance.GetManagedParameter(managedParametersEnum);
		}

		// Token: 0x060013E7 RID: 5095 RVA: 0x0008F4CE File Offset: 0x0008D6CE
		public override float CalculateAlternativeAttackDamage(in AttackInformation attackInformation, in AttackCollisionData collisionData, WeaponComponentData weapon)
		{
			if (weapon == null)
			{
				return 2f;
			}
			if (weapon.WeaponClass == 29)
			{
				return 2f;
			}
			if (weapon.WeaponClass == 28)
			{
				return 1f;
			}
			if (weapon.IsTwoHanded)
			{
				return 2f;
			}
			return 1f;
		}

		// Token: 0x060013E8 RID: 5096 RVA: 0x0008F50C File Offset: 0x0008D70C
		public override float CalculatePassiveAttackDamage(BasicCharacterObject attackerCharacter, in AttackCollisionData collisionData, float baseDamage)
		{
			return baseDamage;
		}

		// Token: 0x060013E9 RID: 5097 RVA: 0x0008F50F File Offset: 0x0008D70F
		public override MeleeCollisionReaction DecidePassiveAttackCollisionReaction(Agent attacker, Agent defender, bool isFatalHit)
		{
			return 3;
		}

		// Token: 0x060013EA RID: 5098 RVA: 0x0008F514 File Offset: 0x0008D714
		public override float CalculateShieldDamage(in AttackInformation attackInformation, float baseDamage)
		{
			baseDamage *= 1.25f;
			FactoredNumber factoredNumber;
			factoredNumber..ctor(baseDamage);
			Formation victimFormation = attackInformation.VictimFormation;
			BannerComponent activeBanner = MissionGameModels.Current.BattleBannerBearersModel.GetActiveBanner(victimFormation);
			if (activeBanner != null)
			{
				BannerHelper.AddBannerBonusForBanner(DefaultBannerEffects.DecreasedShieldDamage, activeBanner, ref factoredNumber);
			}
			return MathF.Max(0f, factoredNumber.ResultNumber);
		}

		// Token: 0x060013EB RID: 5099 RVA: 0x0008F56B File Offset: 0x0008D76B
		public override float CalculateSailFireDamage(Agent attackerAgent, IShipOrigin shipOrigin, float baseDamage, bool damageFromShipMachine)
		{
			return baseDamage;
		}

		// Token: 0x060013EC RID: 5100 RVA: 0x0008F56E File Offset: 0x0008D76E
		public override float CalculateHullFireDamage(float baseFireDamage, IShipOrigin shipOrigin)
		{
			return baseFireDamage;
		}

		// Token: 0x060013ED RID: 5101 RVA: 0x0008F574 File Offset: 0x0008D774
		public override float GetDamageMultiplierForBodyPart(BoneBodyPartType bodyPart, DamageTypes type, bool isHuman, bool isMissile)
		{
			float num = 1f;
			switch (bodyPart)
			{
			case -1:
				num = 1f;
				break;
			case 0:
				switch (type)
				{
				case -1:
					num = 1.5f;
					break;
				case 0:
					num = 1.2f;
					break;
				case 1:
					if (isHuman)
					{
						num = (isMissile ? 2f : 1.25f);
					}
					else
					{
						num = 1.2f;
					}
					break;
				case 2:
					num = 1.2f;
					break;
				}
				break;
			case 1:
				switch (type)
				{
				case -1:
					num = 1.5f;
					break;
				case 0:
					num = 1.2f;
					break;
				case 1:
					if (isHuman)
					{
						num = (isMissile ? 2f : 1.25f);
					}
					else
					{
						num = 1.2f;
					}
					break;
				case 2:
					num = 1.2f;
					break;
				}
				break;
			case 2:
			case 3:
			case 4:
			case 5:
			case 6:
			case 7:
				if (isHuman)
				{
					num = 1f;
				}
				else
				{
					num = 0.8f;
				}
				break;
			case 8:
				num = 0.8f;
				break;
			}
			return num;
		}

		// Token: 0x060013EE RID: 5102 RVA: 0x0008F68E File Offset: 0x0008D88E
		public override bool CanWeaponIgnoreFriendlyFireChecks(WeaponComponentData weapon)
		{
			return weapon != null && weapon.IsConsumable && Extensions.HasAnyFlag<WeaponFlags>(weapon.WeaponFlags, 131072L) && Extensions.HasAnyFlag<WeaponFlags>(weapon.WeaponFlags, 1073741824L);
		}

		// Token: 0x060013EF RID: 5103 RVA: 0x0008F6C4 File Offset: 0x0008D8C4
		public override bool DecideAgentShrugOffBlow(Agent victimAgent, in AttackCollisionData collisionData, in Blow blow)
		{
			return MissionCombatMechanicsHelper.DecideAgentShrugOffBlow(victimAgent, ref collisionData, ref blow);
		}

		// Token: 0x060013F0 RID: 5104 RVA: 0x0008F6CE File Offset: 0x0008D8CE
		public override bool DecideAgentDismountedByBlow(Agent attackerAgent, Agent victimAgent, in AttackCollisionData collisionData, WeaponComponentData attackerWeapon, in Blow blow)
		{
			return MissionCombatMechanicsHelper.DecideAgentDismountedByBlow(attackerAgent, victimAgent, ref collisionData, attackerWeapon, ref blow);
		}

		// Token: 0x060013F1 RID: 5105 RVA: 0x0008F6DC File Offset: 0x0008D8DC
		public override bool DecideAgentKnockedBackByBlow(Agent attackerAgent, Agent victimAgent, in AttackCollisionData collisionData, WeaponComponentData attackerWeapon, in Blow blow)
		{
			return MissionCombatMechanicsHelper.DecideAgentKnockedBackByBlow(attackerAgent, victimAgent, ref collisionData, attackerWeapon, ref blow);
		}

		// Token: 0x060013F2 RID: 5106 RVA: 0x0008F6EA File Offset: 0x0008D8EA
		public override bool DecideAgentKnockedDownByBlow(Agent attackerAgent, Agent victimAgent, in AttackCollisionData collisionData, WeaponComponentData attackerWeapon, in Blow blow)
		{
			return MissionCombatMechanicsHelper.DecideAgentKnockedDownByBlow(attackerAgent, victimAgent, ref collisionData, attackerWeapon, ref blow);
		}

		// Token: 0x060013F3 RID: 5107 RVA: 0x0008F6F8 File Offset: 0x0008D8F8
		public override bool DecideMountRearedByBlow(Agent attackerAgent, Agent victimAgent, in AttackCollisionData collisionData, WeaponComponentData attackerWeapon, in Blow blow)
		{
			return MissionCombatMechanicsHelper.DecideMountRearedByBlow(attackerAgent, victimAgent, ref collisionData, attackerWeapon, ref blow);
		}

		// Token: 0x060013F4 RID: 5108 RVA: 0x0008F708 File Offset: 0x0008D908
		public override void DecideWeaponCollisionReaction(in Blow registeredBlow, in AttackCollisionData collisionData, Agent attacker, Agent defender, in MissionWeapon attackerWeapon, bool isFatalHit, bool isShruggedOff, float momentumRemaining, out MeleeCollisionReaction colReaction)
		{
			MissionCombatMechanicsHelper.DecideWeaponCollisionReaction(ref registeredBlow, ref collisionData, attacker, defender, ref attackerWeapon, isFatalHit, isShruggedOff, momentumRemaining, ref colReaction);
		}

		// Token: 0x060013F5 RID: 5109 RVA: 0x0008F729 File Offset: 0x0008D929
		public override bool ShouldMissilePassThroughAfterShieldBreak(Agent attackerAgent, WeaponComponentData attackerWeapon)
		{
			return false;
		}

		// Token: 0x060013F6 RID: 5110 RVA: 0x0008F72C File Offset: 0x0008D92C
		public override float CalculateRemainingMomentum(float originalMomentum, in Blow b, in AttackCollisionData collisionData, Agent attacker, Agent victim, in MissionWeapon attackerWeapon, bool isCrushThrough)
		{
			return base.CalculateDefaultRemainingMomentum(originalMomentum, ref b, ref collisionData, attacker, victim, ref attackerWeapon, isCrushThrough);
		}

		// Token: 0x060013F7 RID: 5111 RVA: 0x0008F740 File Offset: 0x0008D940
		private UsableMachine GetUsableMachineFromUsableMissionObject(UsableMissionObject usableMissionObject)
		{
			StandingPoint standingPoint;
			if ((standingPoint = usableMissionObject as StandingPoint) != null)
			{
				WeakGameEntity weakGameEntity = standingPoint.GameEntity;
				while (weakGameEntity != null && !weakGameEntity.HasScriptOfType<UsableMachine>())
				{
					weakGameEntity = weakGameEntity.Parent;
				}
				if (weakGameEntity != null)
				{
					UsableMachine firstScriptOfType = weakGameEntity.GetFirstScriptOfType<UsableMachine>();
					if (firstScriptOfType != null)
					{
						return firstScriptOfType;
					}
				}
			}
			return null;
		}

		// Token: 0x04000AC7 RID: 2759
		private const float SallyOutSiegeEngineDamageMultiplier = 4.5f;
	}
}
