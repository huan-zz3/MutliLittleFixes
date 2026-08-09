using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;

namespace NavalDLC.Storyline.CampaignBehaviors
{
	// Token: 0x02000073 RID: 115
	public class NavalStorylineHeroAgentSpawnBehavior : CampaignBehaviorBase
	{
		// Token: 0x06000825 RID: 2085 RVA: 0x00039F70 File Offset: 0x00038170
		public override void RegisterEvents()
		{
			if (!NavalStorylineData.IsNavalStorylineCanceled())
			{
				CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
				CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener(this, new Action(this.OnGameLoadFinished));
				CampaignEvents.OnMissionEndedEvent.AddNonSerializedListener(this, new Action<IMission>(this.OnMissionEnded));
			}
		}

		// Token: 0x06000826 RID: 2086 RVA: 0x00039FC9 File Offset: 0x000381C9
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06000827 RID: 2087 RVA: 0x00039FCC File Offset: 0x000381CC
		private void OnMissionEnded(IMission mission)
		{
			if (Settlement.CurrentSettlement != null && !Hero.MainHero.IsPrisoner && LocationComplex.Current != null && PlayerEncounter.LocationEncounter != null && !Settlement.CurrentSettlement.IsUnderSiege && Settlement.CurrentSettlement.IsTown && Settlement.CurrentSettlement.HasPort && NavalStorylineData.IsNavalStoryLineActive())
			{
				this.AddNavalStorylineHeroesInsideMainPartyToPort(Settlement.CurrentSettlement);
			}
		}

		// Token: 0x06000828 RID: 2088 RVA: 0x0003A030 File Offset: 0x00038230
		private void OnGameLoadFinished()
		{
			if (Settlement.CurrentSettlement != null && !Hero.MainHero.IsPrisoner && LocationComplex.Current != null && PlayerEncounter.LocationEncounter != null && !Settlement.CurrentSettlement.IsUnderSiege && Settlement.CurrentSettlement.IsTown && Settlement.CurrentSettlement.HasPort && NavalStorylineData.IsNavalStoryLineActive())
			{
				this.AddNavalStorylineHeroesInsideMainPartyToPort(Settlement.CurrentSettlement);
			}
		}

		// Token: 0x06000829 RID: 2089 RVA: 0x0003A094 File Offset: 0x00038294
		private void AddNavalStorylineHeroesInsideMainPartyToPort(Settlement settlement)
		{
			foreach (TroopRosterElement troopRosterElement in MobileParty.MainParty.MemberRoster.GetTroopRoster())
			{
				CharacterObject character = troopRosterElement.Character;
				if (character.IsHero && NavalStorylineData.IsNavalStorylineHero(character.HeroObject))
				{
					Hero heroObject = character.HeroObject;
					this.AddNavalStorylineHeroToPortAsLocationCharacter(heroObject);
				}
			}
		}

		// Token: 0x0600082A RID: 2090 RVA: 0x0003A114 File Offset: 0x00038314
		private void OnSettlementEntered(MobileParty party, Settlement settlement, Hero hero)
		{
			if (Settlement.CurrentSettlement != null && !Hero.MainHero.IsPrisoner && LocationComplex.Current != null && PlayerEncounter.LocationEncounter != null && !Settlement.CurrentSettlement.IsUnderSiege && Settlement.CurrentSettlement.IsTown && Settlement.CurrentSettlement.HasPort && NavalStorylineData.IsNavalStoryLineActive())
			{
				this.AddNavalStorylineHeroesInsideMainPartyToPort(Settlement.CurrentSettlement);
			}
		}

		// Token: 0x0600082B RID: 2091 RVA: 0x0003A178 File Offset: 0x00038378
		private void AddNavalStorylineHeroToPortAsLocationCharacter(Hero storylineHero)
		{
			Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(storylineHero.CharacterObject.Race, "_settlement");
			IFaction mapFaction = storylineHero.MapFaction;
			uint num = ((mapFaction != null) ? mapFaction.Color : 4291609515U);
			IFaction mapFaction2 = storylineHero.MapFaction;
			uint num2 = ((mapFaction2 != null) ? mapFaction2.Color : 4291609515U);
			AgentData agentData = new AgentData(new SimpleAgentOrigin(storylineHero.CharacterObject, -1, null, default(UniqueTroopDescriptor))).ClothingColor1(num).ClothingColor2(num2).Monster(monsterWithSuffix)
				.NoHorses(true);
			string text = ActionSetCode.GenerateActionSetNameWithSuffix(agentData.AgentMonster, storylineHero.IsFemale, "_lord");
			LocationCharacter locationCharacter = new LocationCharacter(agentData, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddFixedCharacterBehaviors), "sp_notable", true, 0, text, true, false, null, false, false, true, null, false);
			LocationComplex.Current.GetLocationWithId("port").AddCharacter(locationCharacter);
		}
	}
}
