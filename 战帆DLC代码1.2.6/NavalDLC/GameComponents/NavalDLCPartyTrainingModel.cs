using System;
using Helpers;
using NavalDLC.CharacterDevelopment;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;

namespace NavalDLC.GameComponents
{
	// Token: 0x02000127 RID: 295
	public class NavalDLCPartyTrainingModel : PartyTrainingModel
	{
		// Token: 0x060014A1 RID: 5281 RVA: 0x00092569 File Offset: 0x00090769
		public override int GenerateSharedXp(CharacterObject troop, int xp, MobileParty mobileParty)
		{
			return base.BaseModel.GenerateSharedXp(troop, xp, mobileParty);
		}

		// Token: 0x060014A2 RID: 5282 RVA: 0x0009257C File Offset: 0x0009077C
		public override ExplainedNumber CalculateXpGainFromBattles(FlattenedTroopRosterElement troopRosterElement, PartyBase party)
		{
			ExplainedNumber explainedNumber = base.BaseModel.CalculateXpGainFromBattles(troopRosterElement, party);
			CharacterObject troop = troopRosterElement.Troop;
			if (!troop.IsHero)
			{
				if (troop.IsMariner)
				{
					PerkHelper.AddPerkBonusForParty(NavalPerks.Mariner.Arr, party.MobileParty, false, ref explainedNumber, false);
				}
				if (troop.IsRegular)
				{
					PerkHelper.AddPerkBonusForParty(NavalPerks.Mariner.PirateHunter, party.MobileParty, false, ref explainedNumber, false);
				}
			}
			return explainedNumber;
		}

		// Token: 0x060014A3 RID: 5283 RVA: 0x000925E0 File Offset: 0x000907E0
		public override int GetXpReward(CharacterObject character)
		{
			return base.BaseModel.GetXpReward(character);
		}

		// Token: 0x060014A4 RID: 5284 RVA: 0x000925EE File Offset: 0x000907EE
		public override ExplainedNumber GetEffectiveDailyExperience(MobileParty party, TroopRosterElement troop)
		{
			return base.BaseModel.GetEffectiveDailyExperience(party, troop);
		}
	}
}
