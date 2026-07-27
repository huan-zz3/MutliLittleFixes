using System;
using System.Linq;
using Helpers;
using NavalDLC.Storyline;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace NavalDLC.GameComponents
{
	// Token: 0x0200013E RID: 318
	public class NavalEncounterMenuModel : EncounterGameMenuModel
	{
		// Token: 0x06001552 RID: 5458 RVA: 0x00095A44 File Offset: 0x00093C44
		public override string GetEncounterMenu(PartyBase attackerParty, PartyBase defenderParty, out bool startBattle, out bool joinBattle)
		{
			PartyBase encounteredPartyBase = MapEventHelper.GetEncounteredPartyBase(attackerParty, defenderParty);
			if (NavalStorylineData.IsNavalStoryLineActive() && encounteredPartyBase.IsMobile && encounteredPartyBase.MobileParty.StringId == "free_the_sea_hounds_captives_initial_quest_party")
			{
				startBattle = false;
				joinBattle = false;
				return "act_3_quest_5_encounter_menu";
			}
			if (NavalStorylineData.IsNavalStoryLineActive() && defenderParty.IsSettlement && defenderParty.Settlement.IsTown && defenderParty.Settlement.HasPort)
			{
				startBattle = false;
				joinBattle = false;
				return "naval_storyline_virtualport";
			}
			if (NavalStorylineData.IsNavalStoryLineActive() && defenderParty.IsSettlement && defenderParty.Settlement.IsVillage && defenderParty.Settlement.HasPort)
			{
				startBattle = false;
				joinBattle = false;
				return "naval_storyline_encounter_blocking";
			}
			if (NavalStorylineData.IsNavalStoryLineActive() && Settlement.CurrentSettlement == null)
			{
				bool flag = attackerParty.IsMobile && attackerParty.MobileParty.IsBandit;
				bool flag2 = defenderParty.IsMobile && defenderParty.MobileParty.IsBandit;
				if (!flag && !flag2 && (!defenderParty.IsMobile || attackerParty != PartyBase.MainParty || !defenderParty.IsNavalStorylineQuestParty()) && (!attackerParty.IsMobile || defenderParty != PartyBase.MainParty || !attackerParty.IsNavalStorylineQuestParty()))
				{
					startBattle = false;
					joinBattle = false;
					return "naval_storyline_encounter_blocking";
				}
			}
			string encounterMenu = base.BaseModel.GetEncounterMenu(attackerParty, defenderParty, ref startBattle, ref joinBattle);
			PartyBase partyBase = ((attackerParty == PartyBase.MainParty) ? defenderParty : attackerParty);
			if (NavalStorylineData.IsNavalStoryLineActive() && partyBase.IsNavalStorylineQuestParty())
			{
				if (encounterMenu == "encounter_meeting")
				{
					return "naval_storyline_encounter_meeting";
				}
				if (encounterMenu == "encounter")
				{
					return "naval_storyline_encounter";
				}
				if (encounterMenu == "join_encounter")
				{
					return "naval_storyline_join_encounter";
				}
			}
			return encounterMenu;
		}

		// Token: 0x06001553 RID: 5459 RVA: 0x00095BDC File Offset: 0x00093DDC
		public override string GetGenericStateMenu()
		{
			string genericStateMenu = base.BaseModel.GetGenericStateMenu();
			if (NavalStorylineData.IsNavalStoryLineActive() && genericStateMenu == "encounter")
			{
				MapEvent mapEvent = MobileParty.MainParty.MapEvent;
				if (mapEvent.PartiesOnSide(mapEvent.GetOtherSide(mapEvent.PlayerSide)).Any<MapEventParty>((MapEventParty x) => x.Party.IsNavalStorylineQuestParty()))
				{
					return "naval_storyline_encounter";
				}
			}
			return genericStateMenu;
		}

		// Token: 0x06001554 RID: 5460 RVA: 0x00095C51 File Offset: 0x00093E51
		public override string GetNewPartyJoinMenu(MobileParty newParty)
		{
			return base.BaseModel.GetNewPartyJoinMenu(newParty);
		}

		// Token: 0x06001555 RID: 5461 RVA: 0x00095C5F File Offset: 0x00093E5F
		public override string GetRaidCompleteMenu()
		{
			return base.BaseModel.GetRaidCompleteMenu();
		}

		// Token: 0x06001556 RID: 5462 RVA: 0x00095C6C File Offset: 0x00093E6C
		public override bool IsPlunderMenu(string menuId)
		{
			return base.BaseModel.IsPlunderMenu(menuId);
		}
	}
}
