using System;
using Helpers;
using NavalDLC.CharacterDevelopment;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace NavalDLC.GameComponents
{
	// Token: 0x0200013F RID: 319
	public class NavalMissionShipParametersModel : MissionShipParametersModel
	{
		// Token: 0x06001558 RID: 5464 RVA: 0x00095C84 File Offset: 0x00093E84
		public override int CalculateMainDeckCrewSize(IShipOrigin shipOrigin, Agent formationUnit)
		{
			ExplainedNumber explainedNumber;
			explainedNumber..ctor((float)shipOrigin.MainDeckCrewCapacity, false, null);
			object obj;
			if (formationUnit == null)
			{
				obj = null;
			}
			else
			{
				IAgentOriginBase origin = formationUnit.Origin;
				obj = ((origin != null) ? origin.BattleCombatant : null);
			}
			PartyBase partyBase = (PartyBase)obj;
			MobileParty mobileParty = ((partyBase != null && partyBase.IsMobile) ? partyBase.MobileParty : null);
			if (mobileParty != null)
			{
				PerkHelper.AddPerkBonusForParty(NavalPerks.Boatswain.PopularCaptain, mobileParty, false, ref explainedNumber, false);
			}
			return MathF.Min(MathF.Ceiling(explainedNumber.ResultNumber), shipOrigin.TotalCrewCapacity);
		}

		// Token: 0x06001559 RID: 5465 RVA: 0x00095D00 File Offset: 0x00093F00
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

		// Token: 0x0600155A RID: 5466 RVA: 0x00095D58 File Offset: 0x00093F58
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
