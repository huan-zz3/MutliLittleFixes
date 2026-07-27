using System;
using System.Collections.Generic;
using Helpers;
using NavalDLC.CharacterDevelopment;
using NavalDLC.Missions;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.GameComponents
{
	// Token: 0x02000104 RID: 260
	public class NavalAgentStatCalculateModel : AgentStatCalculateModel
	{
		// Token: 0x060012F6 RID: 4854 RVA: 0x0008AB90 File Offset: 0x00088D90
		public override float GetDifficultyModifier()
		{
			return base.BaseModel.GetDifficultyModifier();
		}

		// Token: 0x060012F7 RID: 4855 RVA: 0x0008AB9D File Offset: 0x00088D9D
		public override bool CanAgentRideMount(Agent agent, Agent targetMount)
		{
			return base.BaseModel.CanAgentRideMount(agent, targetMount);
		}

		// Token: 0x060012F8 RID: 4856 RVA: 0x0008ABAC File Offset: 0x00088DAC
		public override void InitializeAgentStatsAfterDeploymentFinished(Agent agent)
		{
			base.BaseModel.InitializeAgentStatsAfterDeploymentFinished(agent);
			NavalShipsLogic missionBehavior = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			AgentDrivenProperties agentDrivenProperties = agent.AgentDrivenProperties;
			if (missionBehavior != null)
			{
				object obj;
				if (agent == null)
				{
					obj = null;
				}
				else
				{
					IAgentOriginBase origin = agent.Origin;
					obj = ((origin != null) ? origin.BattleCombatant : null);
				}
				PartyBase partyBase = obj as PartyBase;
				MobileParty mobileParty = ((partyBase != null && partyBase.IsMobile) ? partyBase.MobileParty : null);
				CharacterObject characterObject;
				if (mobileParty == null)
				{
					characterObject = null;
				}
				else
				{
					Army army = mobileParty.Army;
					if (army == null)
					{
						characterObject = null;
					}
					else
					{
						MobileParty leaderParty = army.LeaderParty;
						if (leaderParty == null)
						{
							characterObject = null;
						}
						else
						{
							Hero leaderHero = leaderParty.LeaderHero;
							characterObject = ((leaderHero != null) ? leaderHero.CharacterObject : null);
						}
					}
				}
				CharacterObject characterObject2 = characterObject;
				Ship ship = ((partyBase != null && partyBase.Ships.Count > 0) ? ((partyBase != null) ? partyBase.FlagShip : null) : null);
				Figurehead figurehead = ((ship != null) ? ship.Figurehead : null);
				bool flag = characterObject2 != null && characterObject2.GetPerkValue(NavalPerks.Shipmaster.Commodore) && ship != null && figurehead != null;
				if (flag)
				{
					this.ApplyFigureheadBonuses(agent, agentDrivenProperties, figurehead);
				}
				foreach (MissionShip missionShip in missionBehavior.AllShips)
				{
					Ship ship2 = missionShip.ShipOrigin as Ship;
					if ((!flag || ship2 != ship) && missionShip.GetIsAgentOnShip(agent, true))
					{
						Figurehead figurehead2 = ((ship2 != null) ? ship2.Figurehead : null);
						if (figurehead2 != null)
						{
							this.ApplyFigureheadBonuses(agent, agentDrivenProperties, figurehead2);
						}
						agentDrivenProperties.MeleeWeaponDamageMultiplierBonus += missionShip.ShipOrigin.CrewMeleeDamageFactor;
						break;
					}
				}
			}
		}

		// Token: 0x060012F9 RID: 4857 RVA: 0x0008AD3C File Offset: 0x00088F3C
		private void ApplyFigureheadBonuses(Agent agent, AgentDrivenProperties agentDrivenProperties, Figurehead figureHead)
		{
			float effectAmount = figureHead.EffectAmount;
			if (figureHead == DefaultFigureheads.Hawk || figureHead == DefaultFigureheads.Boar)
			{
				this._agentFigureHeadSpawnMap.Add(agent, figureHead);
				return;
			}
			if (figureHead == DefaultFigureheads.Raven)
			{
				agentDrivenProperties.ThrowingWeaponDamageMultiplierBonus += effectAmount;
				return;
			}
			if (figureHead == DefaultFigureheads.SaberToothTiger)
			{
				agentDrivenProperties.ArmorPenetrationMultiplierCrossbow += effectAmount;
				agentDrivenProperties.ArmorPenetrationMultiplierBow += effectAmount;
				return;
			}
			if (figureHead == DefaultFigureheads.Oxen)
			{
				agent.HealthLimit += effectAmount;
				agent.Health += effectAmount;
			}
		}

		// Token: 0x060012FA RID: 4858 RVA: 0x0008ADD0 File Offset: 0x00088FD0
		public override void InitializeMissionEquipmentAfterDeploymentFinished(Agent agent)
		{
			base.BaseModel.InitializeMissionEquipmentAfterDeploymentFinished(agent);
			if (Mission.Current.IsNavalBattle && agent.IsHuman)
			{
				CharacterObject characterObject = agent.Character as CharacterObject;
				if (characterObject != null)
				{
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
					CharacterObject characterObject2 = obj as CharacterObject;
					if (characterObject == characterObject2)
					{
						characterObject2 = null;
					}
					MissionEquipment equipment = agent.Equipment;
					for (int i = 0; i < 5; i++)
					{
						EquipmentIndex equipmentIndex = i;
						MissionWeapon missionWeapon = equipment[equipmentIndex];
						if (!missionWeapon.IsEmpty)
						{
							WeaponComponentData currentUsageItem = missionWeapon.CurrentUsageItem;
							if (currentUsageItem != null && currentUsageItem.IsConsumable && currentUsageItem.RelevantSkill != null)
							{
								ExplainedNumber explainedNumber;
								explainedNumber..ctor(0f, false, null);
								if (currentUsageItem.RelevantSkill == DefaultSkills.Throwing && characterObject2 != null && characterObject2.GetPerkValue(NavalPerks.Boatswain.WellStocked))
								{
									explainedNumber.Add(NavalPerks.Boatswain.WellStocked.SecondaryBonus, null, null);
								}
								int num = MathF.Round(explainedNumber.ResultNumber);
								ExplainedNumber explainedNumber2;
								explainedNumber2..ctor((float)((int)missionWeapon.Amount + num), false, null);
								if ((currentUsageItem.RelevantSkill == DefaultSkills.Bow || currentUsageItem.RelevantSkill == DefaultSkills.Crossbow || currentUsageItem.RelevantSkill == DefaultSkills.Throwing) && characterObject2 != null && characterObject2.GetPerkValue(NavalPerks.Boatswain.WellStocked))
								{
									explainedNumber2.AddFactor(NavalPerks.Boatswain.WellStocked.PrimaryBonus, null);
								}
								if (characterObject2 != null && characterObject2.GetPerkValue(NavalPerks.Boatswain.ShipwrightsInsight))
								{
									explainedNumber2.AddFactor(NavalPerks.Boatswain.ShipwrightsInsight.SecondaryBonus, null);
								}
								int num2 = MathF.Round(explainedNumber2.ResultNumber);
								if (num2 != (int)missionWeapon.Amount)
								{
									equipment.SetAmountOfSlot(equipmentIndex, (short)num2, true);
								}
							}
						}
					}
				}
			}
			NavalShipsLogic missionBehavior = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			if (missionBehavior != null)
			{
				foreach (MissionShip missionShip in missionBehavior.AllShips)
				{
					if (missionShip.GetIsAgentOnShip(agent, true) && !Mission.Current.IsNavalRaidBattle)
					{
						bool flag = MathF.Abs(missionShip.ShipOrigin.CrewShieldHitPointsFactor) > 1E-05f;
						bool flag2 = missionShip.ShipOrigin.AdditionalArcherQuivers != 0;
						bool flag3 = missionShip.ShipOrigin.AdditionalThrowingWeaponStack != 0;
						if (flag || flag2 || flag3)
						{
							for (EquipmentIndex equipmentIndex2 = 0; equipmentIndex2 < 4; equipmentIndex2++)
							{
								if (!agent.Equipment[equipmentIndex2].IsEmpty)
								{
									WeaponComponentData weaponComponentDataForUsage = agent.Equipment[equipmentIndex2].GetWeaponComponentDataForUsage(0);
									if (weaponComponentDataForUsage.IsShield)
									{
										if (flag)
										{
											agent.Equipment.SetHitPointsOfSlot(equipmentIndex2, (short)((float)agent.Equipment[equipmentIndex2].ModifiedMaxHitPoints * (1f + missionShip.ShipOrigin.CrewShieldHitPointsFactor)), true);
											flag = false;
										}
									}
									else if (weaponComponentDataForUsage.IsConsumable)
									{
										if (weaponComponentDataForUsage.IsRangedWeapon)
										{
											if (flag3)
											{
												agent.Equipment.SetAmountOfSlot(equipmentIndex2, (short)((int)agent.Equipment[equipmentIndex2].ModifiedMaxAmount * (1 + missionShip.ShipOrigin.AdditionalThrowingWeaponStack)), true);
												agent.SetWeaponAmountInSlot(equipmentIndex2, agent.Equipment[equipmentIndex2].Amount, true);
												flag3 = false;
											}
										}
										else if (weaponComponentDataForUsage.IsAmmo && flag2)
										{
											agent.Equipment.SetAmountOfSlot(equipmentIndex2, (short)((int)agent.Equipment[equipmentIndex2].ModifiedMaxAmount * (1 + missionShip.ShipOrigin.AdditionalArcherQuivers)), true);
											agent.SetWeaponAmountInSlot(equipmentIndex2, agent.Equipment[equipmentIndex2].Amount, true);
											flag2 = false;
										}
									}
								}
							}
							break;
						}
						break;
					}
				}
			}
		}

		// Token: 0x060012FB RID: 4859 RVA: 0x0008B1D0 File Offset: 0x000893D0
		public override void InitializeAgentStats(Agent agent, Equipment spawnEquipment, AgentDrivenProperties agentDrivenProperties, AgentBuildData agentBuildData)
		{
			base.BaseModel.InitializeAgentStats(agent, spawnEquipment, agentDrivenProperties, agentBuildData);
		}

		// Token: 0x060012FC RID: 4860 RVA: 0x0008B1E2 File Offset: 0x000893E2
		public override void InitializeMissionEquipment(Agent agent)
		{
			base.BaseModel.InitializeMissionEquipment(agent);
		}

		// Token: 0x060012FD RID: 4861 RVA: 0x0008B1F0 File Offset: 0x000893F0
		public override void UpdateAgentStats(Agent agent, AgentDrivenProperties agentDrivenProperties)
		{
			base.BaseModel.UpdateAgentStats(agent, agentDrivenProperties);
			if (Mission.Current.IsNavalBattle && agent.IsHuman)
			{
				this.UpdateNavalHumanStats(agent, agentDrivenProperties);
				AgentNavalComponent component = agent.GetComponent<AgentNavalComponent>();
				MissionShip missionShip = ((component != null) ? component.SteppedShip : null);
				Ship ship = ((missionShip != null) ? missionShip.ShipOrigin : null) as Ship;
				Figurehead figurehead = ((ship != null) ? ship.Figurehead : null);
				if (figurehead != null && figurehead == DefaultFigureheads.Siren)
				{
					BattleSideEnum side = agent.Team.Side;
					Team team = missionShip.Team;
					if (side != ((team != null) ? team.Side : (-1)))
					{
						agentDrivenProperties.DamageMultiplierBonus += figurehead.EffectAmount;
					}
				}
				Figurehead figurehead2;
				if (this._agentFigureHeadSpawnMap.TryGetValue(agent, out figurehead2))
				{
					float num = figurehead2.EffectAmount;
					if (figurehead2 == DefaultFigureheads.Hawk)
					{
						agentDrivenProperties.WeaponInaccuracy *= 1f - num;
						return;
					}
					if (figurehead2 == DefaultFigureheads.Boar)
					{
						num += 1f;
						agentDrivenProperties.ArmorHead *= num;
						agentDrivenProperties.ArmorTorso *= num;
						agentDrivenProperties.ArmorArms *= num;
						agentDrivenProperties.ArmorLegs *= num;
					}
				}
			}
		}

		// Token: 0x060012FE RID: 4862 RVA: 0x0008B318 File Offset: 0x00089518
		private void UpdateNavalHumanStats(Agent agent, AgentDrivenProperties agentDrivenProperties)
		{
			ExplainedNumber explainedNumber;
			explainedNumber..ctor(0.3f, false, null);
			ExplainedNumber explainedNumber2;
			explainedNumber2..ctor(0.3f, false, null);
			ExplainedNumber explainedNumber3;
			explainedNumber3..ctor(0.2f, false, null);
			ExplainedNumber explainedNumber4;
			explainedNumber4..ctor(0.3f, false, null);
			ExplainedNumber explainedNumber5;
			explainedNumber5..ctor(0.03f, false, null);
			ExplainedNumber explainedNumber6;
			explainedNumber6..ctor(0.2f, false, null);
			ExplainedNumber explainedNumber7;
			explainedNumber7..ctor(0.2f, false, null);
			explainedNumber.LimitMin(0f);
			explainedNumber2.LimitMin(0f);
			explainedNumber3.LimitMin(0f);
			explainedNumber4.LimitMin(0f);
			explainedNumber5.LimitMin(0f);
			explainedNumber6.LimitMin(0f);
			explainedNumber7.LimitMin(0f);
			CharacterObject characterObject = agent.Character as CharacterObject;
			if (agent.IsHero)
			{
				int effectiveSkill = this.GetEffectiveSkill(agent, NavalSkills.Mariner);
				SkillHelper.AddSkillBonusForSkillLevel(NavalSkillEffects.NavalBattleCombatPenaltyNegation, ref explainedNumber, effectiveSkill);
				SkillHelper.AddSkillBonusForSkillLevel(NavalSkillEffects.NavalBattleCombatPenaltyNegation, ref explainedNumber2, effectiveSkill);
				SkillHelper.AddSkillBonusForSkillLevel(NavalSkillEffects.NavalBattleCombatPenaltyNegation, ref explainedNumber3, effectiveSkill);
				SkillHelper.AddSkillBonusForSkillLevel(NavalSkillEffects.NavalBattleCombatPenaltyNegation, ref explainedNumber4, effectiveSkill);
				SkillHelper.AddSkillBonusForSkillLevel(NavalSkillEffects.NavalBattleCombatPenaltyNegation, ref explainedNumber5, effectiveSkill);
				SkillHelper.AddSkillBonusForSkillLevel(NavalSkillEffects.NavalBattleCombatPenaltyNegation, ref explainedNumber6, effectiveSkill);
				SkillHelper.AddSkillBonusForSkillLevel(NavalSkillEffects.NavalBattleCombatPenaltyNegation, ref explainedNumber7, effectiveSkill);
			}
			else if (characterObject.IsMariner)
			{
				int num = MathF.Round(1f / MathF.Abs(NavalSkillEffects.NavalBattleCombatPenaltyNegation.Bonus));
				SkillHelper.AddSkillBonusForSkillLevel(NavalSkillEffects.NavalBattleCombatPenaltyNegation, ref explainedNumber, num);
				SkillHelper.AddSkillBonusForSkillLevel(NavalSkillEffects.NavalBattleCombatPenaltyNegation, ref explainedNumber2, num);
				SkillHelper.AddSkillBonusForSkillLevel(NavalSkillEffects.NavalBattleCombatPenaltyNegation, ref explainedNumber3, num);
				SkillHelper.AddSkillBonusForSkillLevel(NavalSkillEffects.NavalBattleCombatPenaltyNegation, ref explainedNumber4, num);
				SkillHelper.AddSkillBonusForSkillLevel(NavalSkillEffects.NavalBattleCombatPenaltyNegation, ref explainedNumber5, num);
				SkillHelper.AddSkillBonusForSkillLevel(NavalSkillEffects.NavalBattleCombatPenaltyNegation, ref explainedNumber6, num);
				SkillHelper.AddSkillBonusForSkillLevel(NavalSkillEffects.NavalBattleCombatPenaltyNegation, ref explainedNumber7, num);
			}
			MissionEquipment equipment = agent.Equipment;
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
			CharacterObject characterObject2 = obj as CharacterObject;
			Formation formation2 = agent.Formation;
			if (((formation2 != null) ? formation2.Captain : null) == agent)
			{
				characterObject2 = null;
			}
			PerkHelper.AddPerkBonusForCharacter(NavalPerks.Shipmaster.WindRider, characterObject, true, ref explainedNumber6, false);
			PerkHelper.AddPerkBonusFromCaptain(NavalPerks.Shipmaster.WindRider, characterObject2, ref explainedNumber6);
			PerkHelper.AddPerkBonusForCharacter(NavalPerks.Mariner.RollingThunder, characterObject, true, ref explainedNumber3, false);
			EquipmentIndex primaryWieldedItemIndex = agent.GetPrimaryWieldedItemIndex();
			WeaponComponentData weaponComponentData = ((primaryWieldedItemIndex != -1) ? equipment[primaryWieldedItemIndex].CurrentUsageItem : null);
			if (weaponComponentData != null && weaponComponentData.IsRangedWeapon)
			{
				PerkHelper.AddPerkBonusForCharacter(NavalPerks.Mariner.RollingThunder, characterObject, true, ref explainedNumber2, false);
				float num2 = 1f + explainedNumber2.ResultNumber;
				agentDrivenProperties.WeaponMaxMovementAccuracyPenalty *= num2;
				agentDrivenProperties.WeaponMaxUnsteadyAccuracyPenalty *= num2;
				agentDrivenProperties.AiShooterErrorWoRangeUpdate += explainedNumber3.ResultNumber;
				agentDrivenProperties.WeaponInaccuracy *= 1f + explainedNumber4.ResultNumber;
				agentDrivenProperties.WeaponRotationalAccuracyPenaltyInRadians *= 1f + explainedNumber.ResultNumber;
				agentDrivenProperties.WeaponExternalAccelerationAccuracyPenalty += explainedNumber5.ResultNumber;
			}
			agentDrivenProperties.MaxSpeedMultiplier *= 1f - explainedNumber6.ResultNumber;
			agentDrivenProperties.DamageMultiplierBonus -= explainedNumber7.ResultNumber;
			if (characterObject != null)
			{
				this.SetNavalPerksAndEffectsOnAgent(agent, characterObject, agentDrivenProperties, weaponComponentData);
			}
		}

		// Token: 0x060012FF RID: 4863 RVA: 0x0008B66D File Offset: 0x0008986D
		public override int GetEffectiveSkill(Agent agent, SkillObject skill)
		{
			return base.BaseModel.GetEffectiveSkill(agent, skill);
		}

		// Token: 0x06001300 RID: 4864 RVA: 0x0008B67C File Offset: 0x0008987C
		public override float GetWeaponDamageMultiplier(Agent agent, WeaponComponentData weapon)
		{
			return base.BaseModel.GetWeaponDamageMultiplier(agent, weapon);
		}

		// Token: 0x06001301 RID: 4865 RVA: 0x0008B68B File Offset: 0x0008988B
		public override float GetEquipmentStealthBonus(Agent agent)
		{
			return base.BaseModel.GetEquipmentStealthBonus(agent);
		}

		// Token: 0x06001302 RID: 4866 RVA: 0x0008B699 File Offset: 0x00089899
		public override float GetSneakAttackMultiplier(Agent agent, WeaponComponentData weapon)
		{
			return base.BaseModel.GetSneakAttackMultiplier(agent, weapon);
		}

		// Token: 0x06001303 RID: 4867 RVA: 0x0008B6A8 File Offset: 0x000898A8
		public override float GetKnockBackResistance(Agent agent)
		{
			return base.BaseModel.GetKnockBackResistance(agent);
		}

		// Token: 0x06001304 RID: 4868 RVA: 0x0008B6B6 File Offset: 0x000898B6
		public override float GetKnockDownResistance(Agent agent, StrikeType strikeType = -1)
		{
			return base.BaseModel.GetKnockDownResistance(agent, strikeType);
		}

		// Token: 0x06001305 RID: 4869 RVA: 0x0008B6C5 File Offset: 0x000898C5
		public override float GetDismountResistance(Agent agent)
		{
			return base.BaseModel.GetDismountResistance(agent);
		}

		// Token: 0x06001306 RID: 4870 RVA: 0x0008B6D3 File Offset: 0x000898D3
		public override float GetWeaponInaccuracy(Agent agent, WeaponComponentData weapon, int weaponSkill)
		{
			return base.BaseModel.GetWeaponInaccuracy(agent, weapon, weaponSkill);
		}

		// Token: 0x06001307 RID: 4871 RVA: 0x0008B6E3 File Offset: 0x000898E3
		public override float GetInteractionDistance(Agent agent)
		{
			return base.BaseModel.GetInteractionDistance(agent);
		}

		// Token: 0x06001308 RID: 4872 RVA: 0x0008B6F1 File Offset: 0x000898F1
		public override float GetMaxCameraZoom(Agent agent)
		{
			return base.BaseModel.GetMaxCameraZoom(agent);
		}

		// Token: 0x06001309 RID: 4873 RVA: 0x0008B6FF File Offset: 0x000898FF
		public override string GetMissionDebugInfoForAgent(Agent agent)
		{
			return base.BaseModel.GetMissionDebugInfoForAgent(agent);
		}

		// Token: 0x0600130A RID: 4874 RVA: 0x0008B70D File Offset: 0x0008990D
		public override float GetEffectiveMaxHealth(Agent agent)
		{
			return base.BaseModel.GetEffectiveMaxHealth(agent);
		}

		// Token: 0x0600130B RID: 4875 RVA: 0x0008B71B File Offset: 0x0008991B
		public override float GetEnvironmentSpeedFactor(Agent agent)
		{
			return base.BaseModel.GetEnvironmentSpeedFactor(agent);
		}

		// Token: 0x0600130C RID: 4876 RVA: 0x0008B72C File Offset: 0x0008992C
		public override float GetBreatheHoldMaxDuration(Agent agent, float baseBreatheHoldMaxDuration)
		{
			if (agent.IsHuman)
			{
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
				CharacterObject characterObject2 = agent.Character as CharacterObject;
				float num = base.BaseModel.GetBreatheHoldMaxDuration(agent, baseBreatheHoldMaxDuration);
				if (characterObject2 == characterObject)
				{
					characterObject = null;
				}
				int effectiveSkill = this.GetEffectiveSkill(agent, NavalSkills.Mariner);
				ExplainedNumber explainedNumber;
				explainedNumber..ctor(0f, false, null);
				if (agent.IsHero)
				{
					SkillHelper.AddSkillBonusForSkillLevel(NavalSkillEffects.NavalBattleUnderwaterBreathingDurationBonus, ref explainedNumber, effectiveSkill);
				}
				else if (characterObject2.IsMariner)
				{
					int num2 = MathF.Round(NavalSkillEffects.NavalBattleUnderwaterBreathingDurationBonus.LimitMax / NavalSkillEffects.NavalBattleUnderwaterBreathingDurationBonus.Bonus);
					SkillHelper.AddSkillBonusForSkillLevel(NavalSkillEffects.NavalBattleUnderwaterBreathingDurationBonus, ref explainedNumber, num2);
				}
				num += explainedNumber.ResultNumber;
				if (agent.GetBaseArmorEffectivenessForBodyPart(2) > 10f)
				{
					num -= 10f;
				}
				ExplainedNumber explainedNumber2;
				explainedNumber2..ctor(num, false, null);
				if (Mission.Current.IsNavalBattle && characterObject != null)
				{
					PerkHelper.AddPerkBonusFromCaptain(NavalPerks.Shipmaster.OldSaltsTouch, characterObject, ref explainedNumber2);
				}
				return explainedNumber2.ResultNumber;
			}
			return 1E+09f;
		}

		// Token: 0x0600130D RID: 4877 RVA: 0x0008B840 File Offset: 0x00089A40
		private void SetNavalPerksAndEffectsOnAgent(Agent agent, CharacterObject agentCharacter, AgentDrivenProperties agentDrivenProperties, WeaponComponentData equippedWeaponComponent)
		{
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
			Formation formation2 = agent.Formation;
			if (((formation2 != null) ? formation2.Captain : null) == agent)
			{
				characterObject = null;
			}
			bool flag = equippedWeaponComponent != null && equippedWeaponComponent.IsMeleeWeapon;
			if (equippedWeaponComponent != null && flag)
			{
				ExplainedNumber explainedNumber;
				explainedNumber..ctor(agentDrivenProperties.HandlingMultiplier, false, null);
				PerkHelper.AddPerkBonusForCharacter(NavalPerks.Mariner.PiratesProwess, agentCharacter, true, ref explainedNumber, false);
				agentDrivenProperties.HandlingMultiplier = explainedNumber.ResultNumber;
			}
			float num = 0f;
			float num2 = 0f;
			bool flag2 = false;
			if (characterObject != null)
			{
				if (agentCharacter.Tier <= 3 && characterObject.GetPerkValue(NavalPerks.Boatswain.SpecialArrows))
				{
					num += NavalPerks.Boatswain.SpecialArrows.PrimaryBonus;
					flag2 = true;
				}
				if (flag2)
				{
					float num3 = 1f + num2;
					agentDrivenProperties.ArmorHead = MathF.Max(0f, (agentDrivenProperties.ArmorHead + num) * num3);
					agentDrivenProperties.ArmorTorso = MathF.Max(0f, (agentDrivenProperties.ArmorTorso + num) * num3);
					agentDrivenProperties.ArmorArms = MathF.Max(0f, (agentDrivenProperties.ArmorArms + num) * num3);
					agentDrivenProperties.ArmorLegs = MathF.Max(0f, (agentDrivenProperties.ArmorLegs + num) * num3);
				}
			}
		}

		// Token: 0x04000ABC RID: 2748
		private Dictionary<Agent, Figurehead> _agentFigureHeadSpawnMap = new Dictionary<Agent, Figurehead>();
	}
}
