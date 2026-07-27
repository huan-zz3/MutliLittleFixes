using System;
using Helpers;
using NavalDLC.CharacterDevelopment;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using NavalDLC.Missions.Objects.UsableMachines;
using NavalDLC.Storyline;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace NavalDLC.GameComponents
{
	// Token: 0x02000103 RID: 259
	public class NavalAgentApplyDamageModel : AgentApplyDamageModel
	{
		// Token: 0x060012D0 RID: 4816 RVA: 0x00089FDD File Offset: 0x000881DD
		private NavalShipsLogic GetNavalShipsLogic()
		{
			return Mission.Current.GetMissionBehavior<NavalShipsLogic>();
		}

		// Token: 0x060012D1 RID: 4817 RVA: 0x00089FE9 File Offset: 0x000881E9
		public override bool IsDamageIgnored(in AttackInformation attackInformation, in AttackCollisionData collisionData)
		{
			return base.BaseModel.IsDamageIgnored(ref attackInformation, ref collisionData);
		}

		// Token: 0x060012D2 RID: 4818 RVA: 0x00089FF8 File Offset: 0x000881F8
		public override float ApplyDamageAmplifications(in AttackInformation attackInformation, in AttackCollisionData collisionData, float baseDamage)
		{
			float num = base.BaseModel.ApplyDamageAmplifications(ref attackInformation, ref collisionData, baseDamage);
			bool isNavalBattle = Mission.Current.IsNavalBattle;
			Agent agent = (attackInformation.IsAttackerAgentMount ? attackInformation.AttackerAgent.RiderAgent : attackInformation.AttackerAgent);
			CharacterObject characterObject = (attackInformation.IsAttackerAgentMount ? attackInformation.AttackerRiderAgentCharacter : attackInformation.AttackerAgentCharacter) as CharacterObject;
			CharacterObject characterObject2 = attackInformation.AttackerCaptainCharacter as CharacterObject;
			Agent agent2 = (attackInformation.IsVictimAgentMount ? attackInformation.AttackerAgent.RiderAgent : attackInformation.VictimAgent);
			bool isVictimAgentMount = attackInformation.IsVictimAgentMount;
			CharacterObject characterObject3 = attackInformation.VictimCaptainCharacter as CharacterObject;
			AttackCollisionData attackCollisionData = collisionData;
			bool flag;
			if (!attackCollisionData.AttackBlockedWithShield)
			{
				attackCollisionData = collisionData;
				flag = attackCollisionData.CollidedWithShieldOnBack;
			}
			else
			{
				flag = true;
			}
			bool flag2 = flag;
			ExplainedNumber explainedNumber;
			explainedNumber..ctor(num, false, null);
			MissionWeapon attackerWeapon = attackInformation.AttackerWeapon;
			WeaponComponentData currentUsageItem = attackerWeapon.CurrentUsageItem;
			if (characterObject != null)
			{
				if (currentUsageItem != null)
				{
					if (currentUsageItem.IsMeleeWeapon)
					{
						if (Mission.Current.IsNavalBattle)
						{
							if (currentUsageItem.RelevantSkill == DefaultSkills.OneHanded)
							{
								PerkHelper.AddPerkBonusForCharacter(NavalPerks.Shipmaster.TheCorsairsEdge, characterObject, true, ref explainedNumber, false);
							}
							if (currentUsageItem.WeaponClass == 4 || currentUsageItem.WeaponClass == 5)
							{
								PerkHelper.AddPerkBonusForCharacter(NavalPerks.Mariner.AxeOfTheNorthwind, characterObject, true, ref explainedNumber, false);
							}
							if (currentUsageItem.WeaponClass == 2 || currentUsageItem.WeaponClass == 3)
							{
								PerkHelper.AddPerkBonusForCharacter(NavalPerks.Mariner.SunnyDisposition, characterObject, true, ref explainedNumber, false);
							}
							if (currentUsageItem.WeaponClass == 5 || currentUsageItem.WeaponClass == 8 || currentUsageItem.WeaponClass == 10 || currentUsageItem.WeaponClass == 3)
							{
								PerkHelper.AddPerkBonusFromCaptain(NavalPerks.Mariner.MightyBlows, characterObject2, ref explainedNumber);
							}
							if (currentUsageItem.IsMeleeWeapon)
							{
								PerkHelper.AddPerkBonusForCharacter(NavalPerks.Mariner.WarriorsMight, characterObject, true, ref explainedNumber, false);
							}
						}
					}
					else if (currentUsageItem.IsConsumable)
					{
						if (currentUsageItem.RelevantSkill == DefaultSkills.Bow)
						{
							attackCollisionData = collisionData;
							if (attackCollisionData.CollisionBoneIndex != -1)
							{
								if (isNavalBattle)
								{
									PerkHelper.AddPerkBonusFromCaptain(NavalPerks.Mariner.TheSkysFury, characterObject2, ref explainedNumber);
									goto IL_0251;
								}
								goto IL_0251;
							}
						}
						if (currentUsageItem.RelevantSkill == DefaultSkills.Crossbow)
						{
							attackCollisionData = collisionData;
							if (attackCollisionData.CollisionBoneIndex != -1)
							{
								if (isNavalBattle)
								{
									PerkHelper.AddPerkBonusFromCaptain(NavalPerks.Mariner.TheSkysFury, characterObject2, ref explainedNumber);
									goto IL_0251;
								}
								goto IL_0251;
							}
						}
						if (currentUsageItem.RelevantSkill == DefaultSkills.Throwing && isNavalBattle)
						{
							PerkHelper.AddPerkBonusFromCaptain(NavalPerks.Mariner.CrewOfSpears, characterObject2, ref explainedNumber);
							PerkHelper.AddPerkBonusFromCaptain(NavalPerks.Mariner.WarriorsMight, characterObject2, ref explainedNumber);
						}
						IL_0251:
						if (isNavalBattle && (currentUsageItem.RelevantSkill == DefaultSkills.Bow || currentUsageItem.RelevantSkill == DefaultSkills.Crossbow || currentUsageItem.RelevantSkill == DefaultSkills.Throwing))
						{
							if (flag2)
							{
								PerkHelper.AddPerkBonusFromCaptain(NavalPerks.Boatswain.AccuracyTraining, characterObject2, ref explainedNumber);
							}
							if (!this.IsAgentCrewBoarded(agent2))
							{
								PerkHelper.AddPerkBonusFromCaptain(NavalPerks.Shipmaster.SeaborneFortress, characterObject3, ref explainedNumber);
							}
							PerkHelper.AddPerkBonusForCharacter(NavalPerks.Mariner.TheSkysFury, characterObject, true, ref explainedNumber, false);
						}
					}
				}
				if ((currentUsageItem == null || currentUsageItem.IsMeleeWeapon) && Mission.Current.IsNavalBattle)
				{
					if (this.IsAgentOnEnemyShip(agent))
					{
						agent.Name == "Itsul Ironeye";
						PerkHelper.AddPerkBonusForCharacter(NavalPerks.Mariner.BoardingMaster, characterObject, true, ref explainedNumber, false);
						PerkHelper.AddPerkBonusFromCaptain(NavalPerks.Mariner.BoardingMaster, characterObject2, ref explainedNumber);
					}
					else if (this.IsAgentOnOwnShip(agent))
					{
						PerkHelper.AddPerkBonusForCharacter(NavalPerks.Mariner.HomeTurfAdvantage, characterObject, true, ref explainedNumber, false);
						PerkHelper.AddPerkBonusFromCaptain(NavalPerks.Mariner.HomeTurfAdvantage, characterObject2, ref explainedNumber);
					}
				}
				attackCollisionData = collisionData;
				if (attackCollisionData.IsAlternativeAttack)
				{
					PerkHelper.AddPerkBonusForCharacter(NavalPerks.Mariner.BruteForce, characterObject, true, ref explainedNumber, false);
				}
				if (flag2 && isNavalBattle)
				{
					PerkHelper.AddPerkBonusFromCaptain(NavalPerks.Mariner.Forceful, characterObject2, ref explainedNumber);
				}
			}
			return explainedNumber.ResultNumber;
		}

		// Token: 0x060012D3 RID: 4819 RVA: 0x0008A370 File Offset: 0x00088570
		public override float ApplyDamageScaling(in AttackInformation attackInformation, in AttackCollisionData collisionData, float baseDamage)
		{
			return base.BaseModel.ApplyDamageScaling(ref attackInformation, ref collisionData, baseDamage);
		}

		// Token: 0x060012D4 RID: 4820 RVA: 0x0008A380 File Offset: 0x00088580
		public override float ApplyDamageReductions(in AttackInformation attackInformation, in AttackCollisionData collisionData, float baseDamage)
		{
			float num = base.BaseModel.ApplyDamageReductions(ref attackInformation, ref collisionData, baseDamage);
			bool isNavalBattle = Mission.Current.IsNavalBattle;
			bool isAttackerAgentMount = attackInformation.IsAttackerAgentMount;
			Agent agent = (attackInformation.IsVictimAgentMount ? attackInformation.VictimAgent.RiderAgent : attackInformation.VictimAgent);
			Agent agent2 = (attackInformation.IsAttackerAgentMount ? attackInformation.AttackerAgent.RiderAgent : attackInformation.AttackerAgent);
			CharacterObject characterObject = (attackInformation.IsVictimAgentMount ? attackInformation.VictimRiderAgentCharacter : attackInformation.VictimAgentCharacter) as CharacterObject;
			CharacterObject characterObject2 = attackInformation.VictimCaptainCharacter as CharacterObject;
			ExplainedNumber explainedNumber;
			explainedNumber..ctor(num, false, null);
			MissionWeapon attackerWeapon = attackInformation.AttackerWeapon;
			WeaponComponentData currentUsageItem = attackerWeapon.CurrentUsageItem;
			if (characterObject != null && currentUsageItem != null)
			{
				if (currentUsageItem.IsConsumable)
				{
					if (isNavalBattle)
					{
						if (agent.CurrentlyUsedGameObject != null && agent.CurrentlyUsedGameObject.GetComponent<UserDamageCalculateComponent>() != null)
						{
							UserDamageCalculateComponent component = agent.CurrentlyUsedGameObject.GetComponent<UserDamageCalculateComponent>();
							component.ApplyPerkBonusForCharacter(NavalPerks.Shipmaster.TheHelmsmansShield, true, characterObject, ref explainedNumber);
							if (agent == Agent.Main)
							{
								explainedNumber.AddFactor(component.DamageReductionFactor, null);
								if (currentUsageItem.WeaponClass == 26 && NavalStorylineData.GetNavalStorylineSetPieceBattleMissionType() == NavalStorylineData.NavalStorylineSetPieceBattleMissionTypes.Act3Quest4)
								{
									explainedNumber.AddFactor(-0.9f, null);
								}
							}
						}
						if (agent2 != null && agent2.IsAIControlled && (currentUsageItem.WeaponClass == 13 || currentUsageItem.WeaponClass == 12))
						{
							explainedNumber.AddFactor(-0.15f, null);
						}
					}
				}
				else if (currentUsageItem.IsMeleeWeapon)
				{
					if (Mission.Current.IsNavalBattle && this.IsAgentOnEnemyShip(agent))
					{
						PerkHelper.AddPerkBonusFromCaptain(NavalPerks.Mariner.TerrorOfTheSeas, characterObject2, ref explainedNumber);
					}
					else if (Mission.Current.IsNavalBattle && this.IsAgentOnOwnShip(agent) && characterObject2 != null && characterObject2.GetPerkValue(NavalPerks.Mariner.RallyingCry))
					{
						explainedNumber.AddFactor(NavalPerks.Mariner.RallyingCry.SecondaryBonus, null);
					}
				}
			}
			return explainedNumber.ResultNumber;
		}

		// Token: 0x060012D5 RID: 4821 RVA: 0x0008A559 File Offset: 0x00088759
		public override float ApplyGeneralDamageModifiers(in AttackInformation attackInformation, in AttackCollisionData collisionData, float baseDamage)
		{
			return base.BaseModel.ApplyGeneralDamageModifiers(ref attackInformation, ref collisionData, baseDamage);
		}

		// Token: 0x060012D6 RID: 4822 RVA: 0x0008A569 File Offset: 0x00088769
		public override bool DecideCrushedThrough(Agent attackerAgent, Agent defenderAgent, float totalAttackEnergy, Agent.UsageDirection attackDirection, StrikeType strikeType, WeaponComponentData defendItem, bool isPassiveUsage)
		{
			return base.BaseModel.DecideCrushedThrough(attackerAgent, defenderAgent, totalAttackEnergy, attackDirection, strikeType, defendItem, isPassiveUsage);
		}

		// Token: 0x060012D7 RID: 4823 RVA: 0x0008A584 File Offset: 0x00088784
		public override void DecideMissileWeaponFlags(Agent attackerAgent, in MissionWeapon missileWeapon, ref WeaponFlags missileWeaponFlags)
		{
			base.BaseModel.DecideMissileWeaponFlags(attackerAgent, ref missileWeapon, ref missileWeaponFlags);
			CharacterObject characterObject = ((attackerAgent != null) ? attackerAgent.Character : null) as CharacterObject;
			if (characterObject != null)
			{
				MissionWeapon missionWeapon = missileWeapon;
				if (missionWeapon.CurrentUsageItem.WeaponClass == 23 && Mission.Current.IsNavalBattle && characterObject.GetPerkValue(NavalPerks.Mariner.CrewOfSpears))
				{
					missileWeaponFlags |= 131072L;
				}
			}
		}

		// Token: 0x060012D8 RID: 4824 RVA: 0x0008A5EF File Offset: 0x000887EF
		public override bool CanWeaponIgnoreFriendlyFireChecks(WeaponComponentData weapon)
		{
			return base.BaseModel.CanWeaponIgnoreFriendlyFireChecks(weapon);
		}

		// Token: 0x060012D9 RID: 4825 RVA: 0x0008A5FD File Offset: 0x000887FD
		public override bool CanWeaponDealSneakAttack(in AttackInformation attackInformation, WeaponComponentData weapon)
		{
			return base.BaseModel.CanWeaponDealSneakAttack(ref attackInformation, weapon);
		}

		// Token: 0x060012DA RID: 4826 RVA: 0x0008A60C File Offset: 0x0008880C
		public override bool CanWeaponDismount(Agent attackerAgent, WeaponComponentData attackerWeapon, in Blow blow, in AttackCollisionData collisionData)
		{
			return base.BaseModel.CanWeaponDismount(attackerAgent, attackerWeapon, ref blow, ref collisionData);
		}

		// Token: 0x060012DB RID: 4827 RVA: 0x0008A61E File Offset: 0x0008881E
		public override void CalculateDefendedBlowStunMultipliers(Agent attackerAgent, Agent defenderAgent, CombatCollisionResult collisionResult, WeaponComponentData attackerWeapon, WeaponComponentData defenderWeapon, ref float attackerStunPeriod, ref float defenderStunPeriod)
		{
			base.BaseModel.CalculateDefendedBlowStunMultipliers(attackerAgent, defenderAgent, collisionResult, attackerWeapon, defenderWeapon, ref attackerStunPeriod, ref defenderStunPeriod);
		}

		// Token: 0x060012DC RID: 4828 RVA: 0x0008A636 File Offset: 0x00088836
		public override bool CanWeaponKnockback(Agent attackerAgent, WeaponComponentData attackerWeapon, in Blow blow, in AttackCollisionData collisionData)
		{
			return base.BaseModel.CanWeaponKnockback(attackerAgent, attackerWeapon, ref blow, ref collisionData);
		}

		// Token: 0x060012DD RID: 4829 RVA: 0x0008A648 File Offset: 0x00088848
		public override bool CanWeaponKnockDown(Agent attackerAgent, Agent victimAgent, WeaponComponentData attackerWeapon, in Blow blow, in AttackCollisionData collisionData)
		{
			return base.BaseModel.CanWeaponKnockDown(attackerAgent, victimAgent, attackerWeapon, ref blow, ref collisionData);
		}

		// Token: 0x060012DE RID: 4830 RVA: 0x0008A65C File Offset: 0x0008885C
		public override float GetDismountPenetration(Agent attackerAgent, WeaponComponentData attackerWeapon, in Blow blow, in AttackCollisionData collisionData)
		{
			return base.BaseModel.GetDismountPenetration(attackerAgent, attackerWeapon, ref blow, ref collisionData);
		}

		// Token: 0x060012DF RID: 4831 RVA: 0x0008A66E File Offset: 0x0008886E
		public override float GetKnockBackPenetration(Agent attackerAgent, WeaponComponentData attackerWeapon, in Blow blow, in AttackCollisionData collisionData)
		{
			return base.BaseModel.GetKnockBackPenetration(attackerAgent, attackerWeapon, ref blow, ref collisionData);
		}

		// Token: 0x060012E0 RID: 4832 RVA: 0x0008A680 File Offset: 0x00088880
		public override float GetKnockDownPenetration(Agent attackerAgent, WeaponComponentData attackerWeapon, in Blow blow, in AttackCollisionData collisionData)
		{
			return base.BaseModel.GetKnockDownPenetration(attackerAgent, attackerWeapon, ref blow, ref collisionData);
		}

		// Token: 0x060012E1 RID: 4833 RVA: 0x0008A692 File Offset: 0x00088892
		public override float GetHorseChargePenetration()
		{
			return base.BaseModel.GetHorseChargePenetration();
		}

		// Token: 0x060012E2 RID: 4834 RVA: 0x0008A69F File Offset: 0x0008889F
		public override float CalculateStaggerThresholdDamage(Agent defenderAgent, in Blow blow)
		{
			return base.BaseModel.CalculateStaggerThresholdDamage(defenderAgent, ref blow);
		}

		// Token: 0x060012E3 RID: 4835 RVA: 0x0008A6AE File Offset: 0x000888AE
		public override float CalculateAlternativeAttackDamage(in AttackInformation attackInformation, in AttackCollisionData collisionData, WeaponComponentData weapon)
		{
			return base.BaseModel.CalculateAlternativeAttackDamage(ref attackInformation, ref collisionData, weapon);
		}

		// Token: 0x060012E4 RID: 4836 RVA: 0x0008A6BE File Offset: 0x000888BE
		public override float CalculatePassiveAttackDamage(BasicCharacterObject attackerCharacter, in AttackCollisionData collisionData, float baseDamage)
		{
			return base.BaseModel.CalculatePassiveAttackDamage(attackerCharacter, ref collisionData, baseDamage);
		}

		// Token: 0x060012E5 RID: 4837 RVA: 0x0008A6CE File Offset: 0x000888CE
		public override MeleeCollisionReaction DecidePassiveAttackCollisionReaction(Agent attacker, Agent defender, bool isFatalHit)
		{
			return base.BaseModel.DecidePassiveAttackCollisionReaction(attacker, defender, isFatalHit);
		}

		// Token: 0x060012E6 RID: 4838 RVA: 0x0008A6DE File Offset: 0x000888DE
		public override float CalculateShieldDamage(in AttackInformation attackInformation, float baseDamage)
		{
			return base.BaseModel.CalculateShieldDamage(ref attackInformation, baseDamage);
		}

		// Token: 0x060012E7 RID: 4839 RVA: 0x0008A6F0 File Offset: 0x000888F0
		public override float CalculateSailFireDamage(Agent agent, IShipOrigin shipOrigin, float baseDamage, bool damageFromShipMachine)
		{
			float num = base.BaseModel.CalculateSailFireDamage(agent, shipOrigin, baseDamage, damageFromShipMachine);
			ExplainedNumber explainedNumber;
			explainedNumber..ctor(num, false, null);
			Formation formation = agent.Formation;
			object obj;
			if (formation == null)
			{
				obj = null;
			}
			else
			{
				Agent captain = formation.Captain;
				obj = ((captain != null) ? captain.Character : null);
			}
			CharacterObject characterObject = obj as CharacterObject;
			if (characterObject != null)
			{
				PerkHelper.AddPerkBonusFromCaptain(NavalPerks.Mariner.EnemyOfTheWood, characterObject, ref explainedNumber);
				if (!damageFromShipMachine)
				{
					PerkHelper.AddPerkBonusFromCaptain(NavalPerks.Boatswain.SpecialArrows, characterObject, ref explainedNumber);
				}
			}
			Figurehead figurehead = (shipOrigin as Ship).Figurehead;
			if (figurehead != null && figurehead == DefaultFigureheads.SeaSerpent)
			{
				explainedNumber.AddFactor(-figurehead.EffectAmount, null);
			}
			return explainedNumber.ResultNumber;
		}

		// Token: 0x060012E8 RID: 4840 RVA: 0x0008A78C File Offset: 0x0008898C
		public override float CalculateHullFireDamage(float baseFireDamage, IShipOrigin shipOrigin)
		{
			base.BaseModel.CalculateHullFireDamage(baseFireDamage, shipOrigin);
			ExplainedNumber explainedNumber;
			explainedNumber..ctor(baseFireDamage, false, null);
			Figurehead figurehead = (shipOrigin as Ship).Figurehead;
			if (figurehead != null && figurehead == DefaultFigureheads.SeaSerpent)
			{
				explainedNumber.AddFactor(-figurehead.EffectAmount, null);
			}
			return explainedNumber.ResultNumber;
		}

		// Token: 0x060012E9 RID: 4841 RVA: 0x0008A7DE File Offset: 0x000889DE
		public override float GetDamageMultiplierForBodyPart(BoneBodyPartType bodyPart, DamageTypes type, bool isHuman, bool isMissile)
		{
			return base.BaseModel.GetDamageMultiplierForBodyPart(bodyPart, type, isHuman, isMissile);
		}

		// Token: 0x060012EA RID: 4842 RVA: 0x0008A7F0 File Offset: 0x000889F0
		public override bool DecideAgentShrugOffBlow(Agent victimAgent, in AttackCollisionData collisionData, in Blow blow)
		{
			return base.BaseModel.DecideAgentShrugOffBlow(victimAgent, ref collisionData, ref blow);
		}

		// Token: 0x060012EB RID: 4843 RVA: 0x0008A800 File Offset: 0x00088A00
		public override bool DecideAgentDismountedByBlow(Agent attackerAgent, Agent victimAgent, in AttackCollisionData collisionData, WeaponComponentData attackerWeapon, in Blow blow)
		{
			return base.BaseModel.DecideAgentDismountedByBlow(attackerAgent, victimAgent, ref collisionData, attackerWeapon, ref blow);
		}

		// Token: 0x060012EC RID: 4844 RVA: 0x0008A814 File Offset: 0x00088A14
		public override bool DecideAgentKnockedBackByBlow(Agent attackerAgent, Agent victimAgent, in AttackCollisionData collisionData, WeaponComponentData attackerWeapon, in Blow blow)
		{
			return base.BaseModel.DecideAgentKnockedBackByBlow(attackerAgent, victimAgent, ref collisionData, attackerWeapon, ref blow);
		}

		// Token: 0x060012ED RID: 4845 RVA: 0x0008A828 File Offset: 0x00088A28
		public override bool DecideAgentKnockedDownByBlow(Agent attackerAgent, Agent victimAgent, in AttackCollisionData collisionData, WeaponComponentData attackerWeapon, in Blow blow)
		{
			return base.BaseModel.DecideAgentKnockedDownByBlow(attackerAgent, victimAgent, ref collisionData, attackerWeapon, ref blow);
		}

		// Token: 0x060012EE RID: 4846 RVA: 0x0008A83C File Offset: 0x00088A3C
		public override bool DecideMountRearedByBlow(Agent attackerAgent, Agent victimAgent, in AttackCollisionData collisionData, WeaponComponentData attackerWeapon, in Blow blow)
		{
			return base.BaseModel.DecideMountRearedByBlow(attackerAgent, victimAgent, ref collisionData, attackerWeapon, ref blow);
		}

		// Token: 0x060012EF RID: 4847 RVA: 0x0008A850 File Offset: 0x00088A50
		public override void DecideWeaponCollisionReaction(in Blow registeredBlow, in AttackCollisionData collisionData, Agent attacker, Agent defender, in MissionWeapon attackerWeapon, bool isFatalHit, bool isShruggedOff, float momentumRemaining, out MeleeCollisionReaction colReaction)
		{
			base.BaseModel.DecideWeaponCollisionReaction(ref registeredBlow, ref collisionData, attacker, defender, ref attackerWeapon, isFatalHit, isShruggedOff, momentumRemaining, ref colReaction);
		}

		// Token: 0x060012F0 RID: 4848 RVA: 0x0008A878 File Offset: 0x00088A78
		public override bool ShouldMissilePassThroughAfterShieldBreak(Agent attackerAgent, WeaponComponentData attackerWeapon)
		{
			bool flag = base.BaseModel.ShouldMissilePassThroughAfterShieldBreak(attackerAgent, attackerWeapon);
			CharacterObject characterObject = (CharacterObject)attackerAgent.Character;
			return (characterObject != null && Mission.Current.IsNavalBattle && attackerWeapon != null && attackerWeapon.WeaponClass == 21 && characterObject.GetPerkValue(NavalPerks.Mariner.CrewOfSpears)) || flag;
		}

		// Token: 0x060012F1 RID: 4849 RVA: 0x0008A8CC File Offset: 0x00088ACC
		public override float CalculateRemainingMomentum(float originalMomentum, in Blow b, in AttackCollisionData collisionData, Agent attacker, Agent victim, in MissionWeapon attackerWeapon, bool isCrushThrough)
		{
			float num = base.BaseModel.CalculateRemainingMomentum(originalMomentum, ref b, ref collisionData, attacker, victim, ref attackerWeapon, isCrushThrough);
			CharacterObject characterObject = (CharacterObject)attacker.Character;
			AttackCollisionData attackCollisionData = collisionData;
			if (attackCollisionData.IsColliderAgent)
			{
				attackCollisionData = collisionData;
				if (!attackCollisionData.IsHorseCharge && (attacker == null || !attacker.IsDoingPassiveAttack) && !MissionCombatMechanicsHelper.HitWithAnotherBone(ref collisionData, attacker, ref attackerWeapon))
				{
					MissionWeapon missionWeapon = attackerWeapon;
					if (!missionWeapon.IsEmpty && b.StrikeType != 1)
					{
						missionWeapon = attackerWeapon;
						if (!missionWeapon.IsEmpty)
						{
							missionWeapon = attackerWeapon;
							if (missionWeapon.CurrentUsageItem.RelevantSkill == DefaultSkills.TwoHanded)
							{
								ExplainedNumber explainedNumber;
								explainedNumber..ctor(0f, false, null);
								explainedNumber.LimitMin(0f);
								if ((float)b.InflictedDamage > 0f)
								{
									explainedNumber.Add(b.AbsorbedByArmor / (float)b.InflictedDamage, null, null);
									if (characterObject != null)
									{
										PerkHelper.AddPerkBonusForCharacter(NavalPerks.Mariner.MightyBlows, characterObject, true, ref explainedNumber, false);
									}
								}
								num = originalMomentum - explainedNumber.ResultNumber;
								num *= 0.5f;
								if (num < 0.25f)
								{
									num = 0f;
								}
							}
						}
					}
				}
			}
			return num;
		}

		// Token: 0x060012F2 RID: 4850 RVA: 0x0008AA04 File Offset: 0x00088C04
		private bool IsAgentOnEnemyShip(Agent agent)
		{
			foreach (MissionShip missionShip in this.GetNavalShipsLogic().AllShips)
			{
				if (missionShip.GameEntity != null && missionShip.Team != null && missionShip.GetIsAgentOnShip(agent, false) && agent.Team.IsEnemyOf(missionShip.Team))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060012F3 RID: 4851 RVA: 0x0008AA90 File Offset: 0x00088C90
		private bool IsAgentOnOwnShip(Agent agent)
		{
			foreach (MissionShip missionShip in this.GetNavalShipsLogic().AllShips)
			{
				if (missionShip.GameEntity != null && missionShip.Team != null && missionShip.GetIsAgentOnShip(agent, false) && agent.Team.IsFriendOf(missionShip.Team))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060012F4 RID: 4852 RVA: 0x0008AB1C File Offset: 0x00088D1C
		private bool IsAgentCrewBoarded(Agent agent)
		{
			NavalShipsLogic navalShipsLogic = this.GetNavalShipsLogic();
			bool flag = false;
			foreach (MissionShip missionShip in navalShipsLogic.AllShips)
			{
				if (missionShip.GameEntity != null && missionShip.GetIsConnectedToEnemy())
				{
					flag = true;
				}
			}
			return flag;
		}

		// Token: 0x04000ABB RID: 2747
		private const float SallyOutSiegeEngineDamageMultiplier = 4.5f;
	}
}
