using System;
using Helpers;
using NavalDLC.CharacterDevelopment;
using NavalDLC.Missions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace NavalDLC.GameComponents
{
	// Token: 0x02000140 RID: 320
	public class NavalMissionSiegeEngineCalculationModel : MissionSiegeEngineCalculationModel
	{
		// Token: 0x0600155C RID: 5468 RVA: 0x00095DC4 File Offset: 0x00093FC4
		public override float CalculateReloadSpeed(Agent userAgent, float baseSpeed)
		{
			float num = base.BaseModel.CalculateReloadSpeed(userAgent, baseSpeed);
			ExplainedNumber explainedNumber;
			explainedNumber..ctor(num, false, null);
			if (Mission.Current.IsNavalBattle)
			{
				object obj;
				if (userAgent == null)
				{
					obj = null;
				}
				else
				{
					Formation formation = userAgent.Formation;
					if (formation == null)
					{
						obj = null;
					}
					else
					{
						Agent captain = formation.Captain;
						obj = ((captain != null) ? captain.Character : null);
					}
				}
				CharacterObject characterObject = (CharacterObject)obj;
				if (((userAgent != null) ? userAgent.Character : null) == characterObject)
				{
					characterObject = null;
				}
				if (characterObject != null)
				{
					PerkHelper.AddPerkBonusFromCaptain(NavalPerks.Boatswain.StreamlinedOperations, characterObject, ref explainedNumber);
				}
				AgentNavalComponent agentNavalComponent = ((userAgent != null) ? userAgent.GetComponent<AgentNavalComponent>() : null);
				if (agentNavalComponent != null && agentNavalComponent.SteppedShip != null)
				{
					Figurehead figurehead = (agentNavalComponent.SteppedShip.ShipOrigin as Ship).Figurehead;
					if (figurehead != null && figurehead == DefaultFigureheads.Viper)
					{
						explainedNumber.AddFactor(figurehead.EffectAmount, null);
					}
				}
			}
			return explainedNumber.ResultNumber;
		}

		// Token: 0x0600155D RID: 5469 RVA: 0x00095E98 File Offset: 0x00094098
		public override int CalculateShipSiegeWeaponAmmoCount(IShipOrigin shipOrigin, Agent captain, RangedSiegeWeapon weapon)
		{
			ExplainedNumber explainedNumber;
			explainedNumber..ctor((float)weapon.AmmoCount, false, null);
			CharacterObject characterObject = ((captain != null) ? captain.Character : null) as CharacterObject;
			if (characterObject != null && weapon is Ballista)
			{
				PerkHelper.AddPerkBonusFromCaptain(NavalPerks.Boatswain.SmoothOperator, characterObject, ref explainedNumber);
			}
			return MathF.Ceiling(explainedNumber.ResultNumber);
		}

		// Token: 0x0600155E RID: 5470 RVA: 0x00095EEC File Offset: 0x000940EC
		public override int CalculateDamage(Agent attackerAgent, float baseDamage)
		{
			int num = base.BaseModel.CalculateDamage(attackerAgent, baseDamage);
			Formation formation = attackerAgent.Formation;
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
			ExplainedNumber explainedNumber;
			explainedNumber..ctor((float)num, false, null);
			if (characterObject != null)
			{
				if (((attackerAgent != null) ? attackerAgent.Character : null) == characterObject)
				{
					characterObject = null;
				}
				if (characterObject != null && characterObject.GetPerkValue(NavalPerks.Boatswain.ShipwrightsInsight))
				{
					explainedNumber.AddFactor(NavalPerks.Boatswain.ShipwrightsInsight.PrimaryBonus, null);
				}
			}
			return MBMath.ClampInt(MathF.Ceiling(explainedNumber.ResultNumber), 0, 2000);
		}
	}
}
