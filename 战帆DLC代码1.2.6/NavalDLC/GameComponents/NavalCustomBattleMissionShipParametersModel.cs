using System;
using Helpers;
using NavalDLC.CharacterDevelopment;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace NavalDLC.GameComponents
{
	// Token: 0x02000107 RID: 263
	public class NavalCustomBattleMissionShipParametersModel : MissionShipParametersModel
	{
		// Token: 0x06001331 RID: 4913 RVA: 0x0008C12C File Offset: 0x0008A32C
		public override int CalculateMainDeckCrewSize(IShipOrigin shipOrigin, Agent formationUnit)
		{
			ExplainedNumber explainedNumber;
			explainedNumber..ctor((float)shipOrigin.MainDeckCrewCapacity, false, null);
			return MathF.Min(MathF.Ceiling(explainedNumber.ResultNumber), shipOrigin.TotalCrewCapacity);
		}

		// Token: 0x06001332 RID: 4914 RVA: 0x0008C160 File Offset: 0x0008A360
		public override float CalculateWindBonus(IShipOrigin shipOrigin, Agent captain, float baseSailForceMagnitude)
		{
			ExplainedNumber explainedNumber;
			explainedNumber..ctor(baseSailForceMagnitude, false, null);
			if (captain != null)
			{
				CharacterObject characterObject = captain.Character as CharacterObject;
				if (characterObject != null)
				{
					int skillValue = characterObject.GetSkillValue(NavalSkills.Shipmaster);
					SkillHelper.AddSkillBonusForSkillLevel(NavalSkillEffects.WindBonus, ref explainedNumber, skillValue);
					PerkHelper.AddPerkBonusFromCaptain(NavalPerks.Shipmaster.Windborne, characterObject, ref explainedNumber);
				}
			}
			return explainedNumber.ResultNumber;
		}

		// Token: 0x06001333 RID: 4915 RVA: 0x0008C1B8 File Offset: 0x0008A3B8
		public override float CalculateOarForceMultiplier(Agent pilotAgent, float baseOarForceMultiplier)
		{
			ExplainedNumber explainedNumber;
			explainedNumber..ctor(baseOarForceMultiplier, false, null);
			explainedNumber.LimitMin(0f);
			Agent agent;
			if (pilotAgent == null)
			{
				agent = null;
			}
			else
			{
				Formation formation = pilotAgent.Formation;
				agent = ((formation != null) ? formation.Captain : null);
			}
			Agent agent2 = agent;
			if (agent2 != null)
			{
				CharacterObject characterObject = agent2.Character as CharacterObject;
				if (characterObject != null)
				{
					PerkHelper.AddPerkBonusFromCaptain(NavalPerks.Shipmaster.ChainToOars, characterObject, ref explainedNumber);
				}
			}
			return explainedNumber.ResultNumber;
		}
	}
}
