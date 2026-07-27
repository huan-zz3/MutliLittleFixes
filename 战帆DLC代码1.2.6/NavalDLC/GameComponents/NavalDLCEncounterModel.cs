using System;
using System.Collections.Generic;
using Helpers;
using NavalDLC.CharacterDevelopment;
using NavalDLC.Storyline;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace NavalDLC.GameComponents
{
	// Token: 0x02000117 RID: 279
	public class NavalDLCEncounterModel : EncounterModel
	{
		// Token: 0x1700035A RID: 858
		// (get) Token: 0x060013F9 RID: 5113 RVA: 0x0008F799 File Offset: 0x0008D999
		public override float NeededMaximumLandDistanceForEncounteringMobileParty
		{
			get
			{
				return base.BaseModel.NeededMaximumLandDistanceForEncounteringMobileParty;
			}
		}

		// Token: 0x1700035B RID: 859
		// (get) Token: 0x060013FA RID: 5114 RVA: 0x0008F7A6 File Offset: 0x0008D9A6
		public override float NeededMaximumNavalDistanceForEncounteringMobileParty
		{
			get
			{
				return 1.5f;
			}
		}

		// Token: 0x1700035C RID: 860
		// (get) Token: 0x060013FB RID: 5115 RVA: 0x0008F7AD File Offset: 0x0008D9AD
		public override float MaximumAllowedLandDistanceForEncounteringMobilePartyInArmy
		{
			get
			{
				return base.BaseModel.MaximumAllowedLandDistanceForEncounteringMobilePartyInArmy;
			}
		}

		// Token: 0x1700035D RID: 861
		// (get) Token: 0x060013FC RID: 5116 RVA: 0x0008F7BA File Offset: 0x0008D9BA
		public override float MaximumAllowedNavalDistanceForEncounteringMobilePartyInArmy
		{
			get
			{
				return 2.5f;
			}
		}

		// Token: 0x1700035E RID: 862
		// (get) Token: 0x060013FD RID: 5117 RVA: 0x0008F7C1 File Offset: 0x0008D9C1
		public override float NeededMaximumDistanceForEncounteringTown
		{
			get
			{
				return base.BaseModel.NeededMaximumDistanceForEncounteringTown;
			}
		}

		// Token: 0x1700035F RID: 863
		// (get) Token: 0x060013FE RID: 5118 RVA: 0x0008F7CE File Offset: 0x0008D9CE
		public override float NeededMaximumDistanceForEncounteringBlockade
		{
			get
			{
				return base.BaseModel.NeededMaximumDistanceForEncounteringBlockade;
			}
		}

		// Token: 0x17000360 RID: 864
		// (get) Token: 0x060013FF RID: 5119 RVA: 0x0008F7DB File Offset: 0x0008D9DB
		public override float NeededMaximumDistanceForEncounteringVillage
		{
			get
			{
				return base.BaseModel.NeededMaximumDistanceForEncounteringVillage;
			}
		}

		// Token: 0x17000361 RID: 865
		// (get) Token: 0x06001400 RID: 5120 RVA: 0x0008F7E8 File Offset: 0x0008D9E8
		public override float GetEncounterJoiningRadius
		{
			get
			{
				return base.BaseModel.GetEncounterJoiningRadius;
			}
		}

		// Token: 0x17000362 RID: 866
		// (get) Token: 0x06001401 RID: 5121 RVA: 0x0008F7F5 File Offset: 0x0008D9F5
		public override float GetSettlementBeingNearFieldBattleRadius
		{
			get
			{
				return base.BaseModel.GetSettlementBeingNearFieldBattleRadius;
			}
		}

		// Token: 0x17000363 RID: 867
		// (get) Token: 0x06001402 RID: 5122 RVA: 0x0008F802 File Offset: 0x0008DA02
		public override float PlayerParleyDistance
		{
			get
			{
				return base.BaseModel.PlayerParleyDistance;
			}
		}

		// Token: 0x17000364 RID: 868
		// (get) Token: 0x06001403 RID: 5123 RVA: 0x0008F80F File Offset: 0x0008DA0F
		public override int MinimumNumberOfMenForAttackingVillageViaScene
		{
			get
			{
				return 15;
			}
		}

		// Token: 0x06001404 RID: 5124 RVA: 0x0008F814 File Offset: 0x0008DA14
		public override bool CanMainHeroDoParleyWithParty(PartyBase partyBase, out TextObject explanation)
		{
			bool flag = base.BaseModel.CanMainHeroDoParleyWithParty(partyBase, ref explanation);
			if (flag)
			{
				if (MobileParty.MainParty.IsCurrentlyAtSea)
				{
					explanation = new TextObject("{=eWxpOYAe}You can't start parley while at sea.", null);
					flag = false;
				}
				else if (MobileParty.MainParty.IsTransitionInProgress)
				{
					explanation = new TextObject("{=boWTBYUF}You can't start parley while embarking.", null);
					flag = false;
				}
			}
			return flag;
		}

		// Token: 0x06001405 RID: 5125 RVA: 0x0008F86B File Offset: 0x0008DA6B
		public override MapEventComponent CreateMapEventComponentForEncounter(PartyBase attackerParty, PartyBase defenderParty, MapEvent.BattleTypes battleType)
		{
			return base.BaseModel.CreateMapEventComponentForEncounter(attackerParty, defenderParty, battleType);
		}

		// Token: 0x06001406 RID: 5126 RVA: 0x0008F87C File Offset: 0x0008DA7C
		public override void FindNonAttachedNpcPartiesWhoWillJoinPlayerEncounter(List<MobileParty> partiesToJoinPlayerSide, List<MobileParty> partiesToJoinEnemySide)
		{
			base.BaseModel.FindNonAttachedNpcPartiesWhoWillJoinPlayerEncounter(partiesToJoinPlayerSide, partiesToJoinEnemySide);
			if (NavalStorylineData.IsNavalStoryLineActive())
			{
				for (int i = partiesToJoinPlayerSide.Count - 1; i >= 0; i--)
				{
					if (!partiesToJoinPlayerSide[i].IsNavalStorylineQuestParty())
					{
						partiesToJoinPlayerSide.RemoveAt(i);
					}
				}
				for (int j = partiesToJoinEnemySide.Count - 1; j >= 0; j--)
				{
					if (!partiesToJoinEnemySide[j].IsNavalStorylineQuestParty())
					{
						partiesToJoinEnemySide.RemoveAt(j);
					}
				}
			}
		}

		// Token: 0x06001407 RID: 5127 RVA: 0x0008F8F0 File Offset: 0x0008DAF0
		public override bool CanPlayerForceBanditsToJoin(out TextObject explanation)
		{
			if (MobileParty.MainParty.IsCurrentlyAtSea)
			{
				bool perkValue = Hero.MainHero.GetPerkValue(NavalPerks.Mariner.Arr);
				explanation = (perkValue ? null : new TextObject("{=MaetSSa1}You need '{PERK}' perk to make this party join you.", null).SetTextVariable("PERK", NavalPerks.Mariner.Arr.Name));
				return perkValue;
			}
			return base.BaseModel.CanPlayerForceBanditsToJoin(ref explanation);
		}

		// Token: 0x06001408 RID: 5128 RVA: 0x0008F94E File Offset: 0x0008DB4E
		public override float GetMapEventSideRunAwayChance(MapEventSide mapEventSide)
		{
			return base.BaseModel.GetMapEventSideRunAwayChance(mapEventSide);
		}

		// Token: 0x06001409 RID: 5129 RVA: 0x0008F95C File Offset: 0x0008DB5C
		public override ExplainedNumber GetBribeChance(MobileParty defenderParty, MobileParty attackerParty)
		{
			ExplainedNumber bribeChance = base.BaseModel.GetBribeChance(defenderParty, attackerParty);
			if (defenderParty.IsBandit && defenderParty.HasNavalNavigationCapability)
			{
				PerkHelper.AddPerkBonusForCharacter(NavalPerks.Mariner.Arr, attackerParty.LeaderHero.CharacterObject, true, ref bribeChance, false);
			}
			return bribeChance;
		}

		// Token: 0x0600140A RID: 5130 RVA: 0x0008F9A1 File Offset: 0x0008DBA1
		public override int GetCharacterSergeantScore(Hero hero)
		{
			return base.BaseModel.GetCharacterSergeantScore(hero);
		}

		// Token: 0x0600140B RID: 5131 RVA: 0x0008F9AF File Offset: 0x0008DBAF
		public override IEnumerable<PartyBase> GetDefenderPartiesOfSettlement(Settlement settlement, MapEvent.BattleTypes mapEventType)
		{
			return base.BaseModel.GetDefenderPartiesOfSettlement(settlement, mapEventType);
		}

		// Token: 0x0600140C RID: 5132 RVA: 0x0008F9BE File Offset: 0x0008DBBE
		public override Hero GetLeaderOfMapEvent(MapEvent mapEvent, BattleSideEnum side)
		{
			return base.BaseModel.GetLeaderOfMapEvent(mapEvent, side);
		}

		// Token: 0x0600140D RID: 5133 RVA: 0x0008F9CD File Offset: 0x0008DBCD
		public override Hero GetLeaderOfSiegeEvent(SiegeEvent siegeEvent, BattleSideEnum side)
		{
			return base.BaseModel.GetLeaderOfSiegeEvent(siegeEvent, side);
		}

		// Token: 0x0600140E RID: 5134 RVA: 0x0008F9DC File Offset: 0x0008DBDC
		public override PartyBase GetNextDefenderPartyOfSettlement(Settlement settlement, ref int partyIndex, MapEvent.BattleTypes mapEventType)
		{
			return base.BaseModel.GetNextDefenderPartyOfSettlement(settlement, ref partyIndex, mapEventType);
		}

		// Token: 0x0600140F RID: 5135 RVA: 0x0008F9EC File Offset: 0x0008DBEC
		public override float GetSurrenderChance(MobileParty defenderParty, MobileParty attackerParty)
		{
			return base.BaseModel.GetSurrenderChance(defenderParty, attackerParty);
		}

		// Token: 0x06001410 RID: 5136 RVA: 0x0008F9FB File Offset: 0x0008DBFB
		public override bool IsEncounterExemptFromHostileActions(PartyBase side1, PartyBase side2)
		{
			return base.BaseModel.IsEncounterExemptFromHostileActions(side1, side2);
		}

		// Token: 0x06001411 RID: 5137 RVA: 0x0008FA0C File Offset: 0x0008DC0C
		public override bool IsPartyUnderPlayerCommand(PartyBase party)
		{
			return (!party.IsMobile || party.MobileParty.IsMainParty || !party.MobileParty.IsCurrentlyUsedByAQuest || !NavalStorylineData.IsNavalStoryLineActive() || NavalStorylineData.GetStorylineStage() != NavalStorylineData.NavalStorylineStage.Act2) && base.BaseModel.IsPartyUnderPlayerCommand(party);
		}

		// Token: 0x06001412 RID: 5138 RVA: 0x0008FA58 File Offset: 0x0008DC58
		public override MBReadOnlyList<MobileParty> GetPartiesToTeleportOnMapEventFinalize(MapEvent mapEvent)
		{
			List<MobileParty> partiesToTeleportOnMapEventFinalize = base.BaseModel.GetPartiesToTeleportOnMapEventFinalize(mapEvent);
			MBList<MobileParty> mblist = new MBList<MobileParty>();
			foreach (MobileParty mobileParty in partiesToTeleportOnMapEventFinalize)
			{
				if (!mobileParty.IsCurrentlyAtSea || mobileParty.HasNavalNavigationCapability)
				{
					mblist.Add(mobileParty);
				}
			}
			return mblist;
		}
	}
}
