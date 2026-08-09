using System;
using NavalDLC.CharacterDevelopment;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.ComponentInterfaces
{
	// Token: 0x02000158 RID: 344
	public class NavalCustomBattleAgentStatCalculateModel : AgentStatCalculateModel
	{
		// Token: 0x06001669 RID: 5737 RVA: 0x000997A1 File Offset: 0x000979A1
		public override float GetDifficultyModifier()
		{
			return base.BaseModel.GetDifficultyModifier();
		}

		// Token: 0x0600166A RID: 5738 RVA: 0x000997AE File Offset: 0x000979AE
		public override bool CanAgentRideMount(Agent agent, Agent targetMount)
		{
			return base.BaseModel.CanAgentRideMount(agent, targetMount);
		}

		// Token: 0x0600166B RID: 5739 RVA: 0x000997C0 File Offset: 0x000979C0
		public override void InitializeAgentStatsAfterDeploymentFinished(Agent agent)
		{
			base.BaseModel.InitializeAgentStatsAfterDeploymentFinished(agent);
			NavalShipsLogic missionBehavior = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			AgentDrivenProperties agentDrivenProperties = agent.AgentDrivenProperties;
			if (missionBehavior != null)
			{
				foreach (MissionShip missionShip in missionBehavior.AllShips)
				{
					if (missionShip.GetIsAgentOnShip(agent, true))
					{
						agentDrivenProperties.MeleeWeaponDamageMultiplierBonus += missionShip.ShipOrigin.CrewMeleeDamageFactor;
						break;
					}
				}
			}
		}

		// Token: 0x0600166C RID: 5740 RVA: 0x00099854 File Offset: 0x00097A54
		public override void InitializeMissionEquipmentAfterDeploymentFinished(Agent agent)
		{
			base.BaseModel.InitializeMissionEquipmentAfterDeploymentFinished(agent);
			NavalShipsLogic missionBehavior = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			if (missionBehavior != null && !Mission.Current.IsNavalRaidBattle)
			{
				foreach (MissionShip missionShip in missionBehavior.AllShips)
				{
					if (missionShip.GetIsAgentOnShip(agent, true))
					{
						bool flag = MathF.Abs(missionShip.ShipOrigin.CrewShieldHitPointsFactor) > 1E-05f;
						bool flag2 = missionShip.ShipOrigin.AdditionalArcherQuivers != 0;
						bool flag3 = missionShip.ShipOrigin.AdditionalThrowingWeaponStack != 0;
						if (flag || flag2 || flag3)
						{
							for (EquipmentIndex equipmentIndex = 0; equipmentIndex < 4; equipmentIndex++)
							{
								if (!agent.Equipment[equipmentIndex].IsEmpty)
								{
									WeaponComponentData weaponComponentDataForUsage = agent.Equipment[equipmentIndex].GetWeaponComponentDataForUsage(0);
									if (weaponComponentDataForUsage.IsShield)
									{
										if (flag)
										{
											agent.Equipment.SetHitPointsOfSlot(equipmentIndex, (short)((float)agent.Equipment[equipmentIndex].ModifiedMaxHitPoints * (1f + missionShip.ShipOrigin.CrewShieldHitPointsFactor)), true);
											flag = false;
										}
									}
									else if (weaponComponentDataForUsage.IsConsumable)
									{
										if (weaponComponentDataForUsage.IsRangedWeapon)
										{
											if (flag3)
											{
												agent.Equipment.SetAmountOfSlot(equipmentIndex, (short)((int)agent.Equipment[equipmentIndex].ModifiedMaxAmount * (1 + missionShip.ShipOrigin.AdditionalThrowingWeaponStack)), true);
												agent.SetWeaponAmountInSlot(equipmentIndex, agent.Equipment[equipmentIndex].Amount, true);
												flag3 = false;
											}
										}
										else if (weaponComponentDataForUsage.IsAmmo && flag2)
										{
											agent.Equipment.SetAmountOfSlot(equipmentIndex, (short)((int)agent.Equipment[equipmentIndex].ModifiedMaxAmount * (1 + missionShip.ShipOrigin.AdditionalArcherQuivers)), true);
											agent.SetWeaponAmountInSlot(equipmentIndex, agent.Equipment[equipmentIndex].Amount, true);
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

		// Token: 0x0600166D RID: 5741 RVA: 0x00099AA0 File Offset: 0x00097CA0
		public override void InitializeAgentStats(Agent agent, Equipment spawnEquipment, AgentDrivenProperties agentDrivenProperties, AgentBuildData agentBuildData)
		{
			base.BaseModel.InitializeAgentStats(agent, spawnEquipment, agentDrivenProperties, agentBuildData);
		}

		// Token: 0x0600166E RID: 5742 RVA: 0x00099AB2 File Offset: 0x00097CB2
		public override void InitializeMissionEquipment(Agent agent)
		{
			base.BaseModel.InitializeMissionEquipment(agent);
		}

		// Token: 0x0600166F RID: 5743 RVA: 0x00099AC0 File Offset: 0x00097CC0
		public override void UpdateAgentStats(Agent agent, AgentDrivenProperties agentDrivenProperties)
		{
			base.BaseModel.UpdateAgentStats(agent, agentDrivenProperties);
			if (Mission.Current.IsNavalBattle && agent.IsHuman)
			{
				this.UpdateNavalHumanStats(agent, agentDrivenProperties);
			}
		}

		// Token: 0x06001670 RID: 5744 RVA: 0x00099AEC File Offset: 0x00097CEC
		private void UpdateNavalHumanStats(Agent agent, AgentDrivenProperties agentDrivenProperties)
		{
			bool flag = this.GetEffectiveSkill(agent, NavalSkills.Mariner) >= 40;
			MissionEquipment equipment = agent.Equipment;
			EquipmentIndex primaryWieldedItemIndex = agent.GetPrimaryWieldedItemIndex();
			WeaponComponentData weaponComponentData = ((primaryWieldedItemIndex != -1) ? equipment[primaryWieldedItemIndex].CurrentUsageItem : null);
			if (weaponComponentData != null && weaponComponentData.IsRangedWeapon && !flag)
			{
				float num = 1.3f;
				agentDrivenProperties.WeaponMaxMovementAccuracyPenalty *= num;
				agentDrivenProperties.WeaponMaxUnsteadyAccuracyPenalty *= num;
				agentDrivenProperties.AiShooterErrorWoRangeUpdate += 0.2f;
				agentDrivenProperties.WeaponInaccuracy *= 1.3f;
				agentDrivenProperties.WeaponRotationalAccuracyPenaltyInRadians *= 1.3f;
				agentDrivenProperties.WeaponExternalAccelerationAccuracyPenalty += 0.03f;
			}
			if (!flag)
			{
				agentDrivenProperties.MaxSpeedMultiplier *= 0.8f;
				agentDrivenProperties.DamageMultiplierBonus -= 0.2f;
			}
		}

		// Token: 0x06001671 RID: 5745 RVA: 0x00099BD5 File Offset: 0x00097DD5
		public override int GetEffectiveSkill(Agent agent, SkillObject skill)
		{
			return base.BaseModel.GetEffectiveSkill(agent, skill);
		}

		// Token: 0x06001672 RID: 5746 RVA: 0x00099BE4 File Offset: 0x00097DE4
		public override float GetWeaponDamageMultiplier(Agent agent, WeaponComponentData weapon)
		{
			return base.BaseModel.GetWeaponDamageMultiplier(agent, weapon);
		}

		// Token: 0x06001673 RID: 5747 RVA: 0x00099BF3 File Offset: 0x00097DF3
		public override float GetEquipmentStealthBonus(Agent agent)
		{
			return base.BaseModel.GetEquipmentStealthBonus(agent);
		}

		// Token: 0x06001674 RID: 5748 RVA: 0x00099C01 File Offset: 0x00097E01
		public override float GetSneakAttackMultiplier(Agent agent, WeaponComponentData weapon)
		{
			return base.BaseModel.GetSneakAttackMultiplier(agent, weapon);
		}

		// Token: 0x06001675 RID: 5749 RVA: 0x00099C10 File Offset: 0x00097E10
		public override float GetKnockBackResistance(Agent agent)
		{
			return base.BaseModel.GetKnockBackResistance(agent);
		}

		// Token: 0x06001676 RID: 5750 RVA: 0x00099C1E File Offset: 0x00097E1E
		public override float GetKnockDownResistance(Agent agent, StrikeType strikeType = -1)
		{
			return base.BaseModel.GetKnockDownResistance(agent, strikeType);
		}

		// Token: 0x06001677 RID: 5751 RVA: 0x00099C2D File Offset: 0x00097E2D
		public override float GetDismountResistance(Agent agent)
		{
			return base.BaseModel.GetDismountResistance(agent);
		}

		// Token: 0x06001678 RID: 5752 RVA: 0x00099C3B File Offset: 0x00097E3B
		public override float GetWeaponInaccuracy(Agent agent, WeaponComponentData weapon, int weaponSkill)
		{
			return base.BaseModel.GetWeaponInaccuracy(agent, weapon, weaponSkill);
		}

		// Token: 0x06001679 RID: 5753 RVA: 0x00099C4B File Offset: 0x00097E4B
		public override float GetInteractionDistance(Agent agent)
		{
			return base.BaseModel.GetInteractionDistance(agent);
		}

		// Token: 0x0600167A RID: 5754 RVA: 0x00099C59 File Offset: 0x00097E59
		public override float GetMaxCameraZoom(Agent agent)
		{
			return base.BaseModel.GetMaxCameraZoom(agent);
		}

		// Token: 0x0600167B RID: 5755 RVA: 0x00099C67 File Offset: 0x00097E67
		public override string GetMissionDebugInfoForAgent(Agent agent)
		{
			return base.BaseModel.GetMissionDebugInfoForAgent(agent);
		}

		// Token: 0x0600167C RID: 5756 RVA: 0x00099C75 File Offset: 0x00097E75
		public override float GetEffectiveMaxHealth(Agent agent)
		{
			return base.BaseModel.GetEffectiveMaxHealth(agent);
		}

		// Token: 0x0600167D RID: 5757 RVA: 0x00099C83 File Offset: 0x00097E83
		public override float GetEnvironmentSpeedFactor(Agent agent)
		{
			return base.BaseModel.GetEnvironmentSpeedFactor(agent);
		}

		// Token: 0x0600167E RID: 5758 RVA: 0x00099C94 File Offset: 0x00097E94
		public override float GetBreatheHoldMaxDuration(Agent agent, float baseBreatheHoldMaxDuration)
		{
			if (agent.IsHuman)
			{
				float num = base.BaseModel.GetBreatheHoldMaxDuration(agent, baseBreatheHoldMaxDuration);
				if (this.GetEffectiveSkill(agent, NavalSkills.Mariner) >= 40)
				{
					num += 20f;
				}
				return num;
			}
			return 1E+09f;
		}

		// Token: 0x04000B70 RID: 2928
		private const int MinMarinerSkillToConsiderAgentAsMariner = 40;
	}
}
