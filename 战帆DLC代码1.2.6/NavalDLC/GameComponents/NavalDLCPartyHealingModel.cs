using System;
using Helpers;
using NavalDLC.CharacterDevelopment;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace NavalDLC.GameComponents
{
	// Token: 0x02000122 RID: 290
	public class NavalDLCPartyHealingModel : PartyHealingModel
	{
		// Token: 0x06001473 RID: 5235 RVA: 0x000919B4 File Offset: 0x0008FBB4
		public override float GetSurgeryChance(PartyBase party)
		{
			return base.BaseModel.GetSurgeryChance(party);
		}

		// Token: 0x06001474 RID: 5236 RVA: 0x000919C2 File Offset: 0x0008FBC2
		public override float GetSurvivalChance(PartyBase party, CharacterObject agentCharacter, DamageTypes damageType, bool canDamageKillEvenIfBlunt, PartyBase enemyParty = null)
		{
			return base.BaseModel.GetSurvivalChance(party, agentCharacter, damageType, canDamageKillEvenIfBlunt, enemyParty);
		}

		// Token: 0x06001475 RID: 5237 RVA: 0x000919D6 File Offset: 0x0008FBD6
		public override int GetSkillXpFromHealingTroop(PartyBase party)
		{
			return base.BaseModel.GetSkillXpFromHealingTroop(party);
		}

		// Token: 0x06001476 RID: 5238 RVA: 0x000919E4 File Offset: 0x0008FBE4
		public override ExplainedNumber GetDailyHealingForRegulars(PartyBase partyBase, bool isPrisoner, bool includeDescriptions = false)
		{
			ExplainedNumber dailyHealingForRegulars = base.BaseModel.GetDailyHealingForRegulars(partyBase, isPrisoner, includeDescriptions);
			if (partyBase.IsMobile && partyBase.MobileParty.IsCurrentlyAtSea)
			{
				PerkHelper.AddPerkBonusForParty(NavalPerks.Boatswain.Resilience, partyBase.MobileParty, false, ref dailyHealingForRegulars, false);
			}
			return dailyHealingForRegulars;
		}

		// Token: 0x06001477 RID: 5239 RVA: 0x00091A2A File Offset: 0x0008FC2A
		public override ExplainedNumber GetDailyHealingHpForHeroes(PartyBase partyBase, bool isPrisoners, bool includeDescriptions = false)
		{
			return base.BaseModel.GetDailyHealingHpForHeroes(partyBase, isPrisoners, includeDescriptions);
		}

		// Token: 0x06001478 RID: 5240 RVA: 0x00091A3A File Offset: 0x0008FC3A
		public override int GetHeroesEffectedHealingAmount(Hero hero, float healingRate)
		{
			return base.BaseModel.GetHeroesEffectedHealingAmount(hero, healingRate);
		}

		// Token: 0x06001479 RID: 5241 RVA: 0x00091A49 File Offset: 0x0008FC49
		public override float GetSiegeBombardmentHitSurgeryChance(PartyBase party)
		{
			return base.BaseModel.GetSiegeBombardmentHitSurgeryChance(party);
		}

		// Token: 0x0600147A RID: 5242 RVA: 0x00091A58 File Offset: 0x0008FC58
		public override ExplainedNumber GetBattleEndHealingAmount(PartyBase partyBase, Hero hero)
		{
			ExplainedNumber battleEndHealingAmount = base.BaseModel.GetBattleEndHealingAmount(partyBase, hero);
			if (hero.GetPerkValue(NavalPerks.Boatswain.Resilience))
			{
				battleEndHealingAmount.Add(NavalPerks.Boatswain.Resilience.PrimaryBonus * (float)(hero.MaxHitPoints - hero.HitPoints), NavalPerks.Boatswain.Resilience.Name, null);
			}
			return battleEndHealingAmount;
		}
	}
}
