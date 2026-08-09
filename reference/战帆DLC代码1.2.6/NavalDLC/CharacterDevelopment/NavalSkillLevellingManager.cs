using System;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Conversation.Persuasion;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.CharacterDevelopment
{
	// Token: 0x0200015D RID: 349
	public class NavalSkillLevellingManager : ISkillLevelingManager
	{
		// Token: 0x0600169E RID: 5790 RVA: 0x0009B574 File Offset: 0x00099774
		public void OnCombatHit(CharacterObject affectorCharacter, CharacterObject affectedCharacter, CharacterObject captain, Hero commander, float speedBonusFromMovement, float shotDifficulty, WeaponComponentData affectorWeapon, float hitPointRatio, CombatXpModel.MissionTypeEnum missionType, bool isAffectorMounted, bool isTeamKill, bool isAffectorUnderCommand, float damageAmount, bool isFatal, bool isSiegeEngineHit, bool isHorseCharge, bool isSneakAttack)
		{
			if (Mission.Current.IsNavalBattle && affectorCharacter.IsHero && !isTeamKill)
			{
				Hero heroObject = affectorCharacter.HeroObject;
				CombatXpModel combatXpModel = Campaign.Current.Models.CombatXpModel;
				CharacterObject characterObject = heroObject.CharacterObject;
				MobileParty partyBelongedTo = heroObject.PartyBelongedTo;
				float num = (float)combatXpModel.GetXpFromHit(characterObject, captain, affectedCharacter, (partyBelongedTo != null) ? partyBelongedTo.Party : null, (int)damageAmount, isFatal, missionType).RoundedResultNumber;
				heroObject.AddSkillXp(NavalSkills.Mariner, (float)MBRandom.RoundRandomized(num));
			}
			this._defaultSkillLevelingManager.OnCombatHit(affectorCharacter, affectedCharacter, captain, commander, speedBonusFromMovement, shotDifficulty, affectorWeapon, hitPointRatio, missionType, isAffectorMounted, isTeamKill, isAffectorUnderCommand, damageAmount, isFatal, isSiegeEngineHit, isHorseCharge, isSneakAttack);
		}

		// Token: 0x0600169F RID: 5791 RVA: 0x0009B61B File Offset: 0x0009981B
		public void OnSiegeEngineDestroyed(MobileParty party, SiegeEngineType destroyedSiegeEngine)
		{
			this._defaultSkillLevelingManager.OnSiegeEngineDestroyed(party, destroyedSiegeEngine);
		}

		// Token: 0x060016A0 RID: 5792 RVA: 0x0009B62C File Offset: 0x0009982C
		public void OnSimulationCombatKill(CharacterObject affectorCharacter, CharacterObject affectedCharacter, PartyBase affectorParty, PartyBase commanderParty)
		{
			this._defaultSkillLevelingManager.OnSimulationCombatKill(affectorCharacter, affectedCharacter, affectorParty, commanderParty);
			int xpReward = Campaign.Current.Models.PartyTrainingModel.GetXpReward(affectedCharacter);
			if (commanderParty != null && commanderParty.IsMobile && commanderParty.MapEvent.IsNavalMapEvent && commanderParty.LeaderHero != null && commanderParty.LeaderHero != affectedCharacter.HeroObject)
			{
				NavalSkillLevellingManager.OnPartySkillExercised(commanderParty.MobileParty, NavalSkills.Mariner, (float)xpReward * 0.02f, 5);
			}
		}

		// Token: 0x060016A1 RID: 5793 RVA: 0x0009B6AC File Offset: 0x000998AC
		public void OnTradeProfitMade(PartyBase party, int tradeProfit)
		{
			this._defaultSkillLevelingManager.OnTradeProfitMade(party, tradeProfit);
		}

		// Token: 0x060016A2 RID: 5794 RVA: 0x0009B6BB File Offset: 0x000998BB
		public void OnTradeProfitMade(Hero hero, int tradeProfit)
		{
			this._defaultSkillLevelingManager.OnTradeProfitMade(hero, tradeProfit);
		}

		// Token: 0x060016A3 RID: 5795 RVA: 0x0009B6CA File Offset: 0x000998CA
		public void OnSettlementProjectFinished(Settlement settlement)
		{
			this._defaultSkillLevelingManager.OnSettlementProjectFinished(settlement);
		}

		// Token: 0x060016A4 RID: 5796 RVA: 0x0009B6D8 File Offset: 0x000998D8
		public void OnSettlementGoverned(Hero governor, Settlement settlement)
		{
			this._defaultSkillLevelingManager.OnSettlementGoverned(governor, settlement);
		}

		// Token: 0x060016A5 RID: 5797 RVA: 0x0009B6E7 File Offset: 0x000998E7
		public void OnInfluenceSpent(Hero hero, float amountSpent)
		{
			this._defaultSkillLevelingManager.OnInfluenceSpent(hero, amountSpent);
		}

		// Token: 0x060016A6 RID: 5798 RVA: 0x0009B6F6 File Offset: 0x000998F6
		public void OnGainRelation(Hero hero, Hero gainedRelationWith, float relationChange, ChangeRelationAction.ChangeRelationDetail detail = 0)
		{
			this._defaultSkillLevelingManager.OnGainRelation(hero, gainedRelationWith, relationChange, detail);
		}

		// Token: 0x060016A7 RID: 5799 RVA: 0x0009B708 File Offset: 0x00099908
		public void OnTroopRecruited(Hero hero, int amount, int tier)
		{
			this._defaultSkillLevelingManager.OnTroopRecruited(hero, amount, tier);
		}

		// Token: 0x060016A8 RID: 5800 RVA: 0x0009B718 File Offset: 0x00099918
		public void OnBribeGiven(int amount)
		{
			this._defaultSkillLevelingManager.OnBribeGiven(amount);
		}

		// Token: 0x060016A9 RID: 5801 RVA: 0x0009B726 File Offset: 0x00099926
		public void OnWarehouseProduction(EquipmentElement production)
		{
			this._defaultSkillLevelingManager.OnWarehouseProduction(production);
		}

		// Token: 0x060016AA RID: 5802 RVA: 0x0009B734 File Offset: 0x00099934
		public void OnAIPartyLootCasualties(int goldAmount, Hero winnerPartyLeader, PartyBase defeatedParty)
		{
			this._defaultSkillLevelingManager.OnAIPartyLootCasualties(goldAmount, winnerPartyLeader, defeatedParty);
		}

		// Token: 0x060016AB RID: 5803 RVA: 0x0009B744 File Offset: 0x00099944
		public void OnBanditsRecruited(MobileParty mobileParty, CharacterObject bandit, int count)
		{
			this._defaultSkillLevelingManager.OnBanditsRecruited(mobileParty, bandit, count);
		}

		// Token: 0x060016AC RID: 5804 RVA: 0x0009B754 File Offset: 0x00099954
		public void OnMainHeroReleasedFromCaptivity(float captivityTime)
		{
			this._defaultSkillLevelingManager.OnMainHeroReleasedFromCaptivity(captivityTime);
		}

		// Token: 0x060016AD RID: 5805 RVA: 0x0009B762 File Offset: 0x00099962
		public void OnMainHeroTortured()
		{
			this._defaultSkillLevelingManager.OnMainHeroTortured();
		}

		// Token: 0x060016AE RID: 5806 RVA: 0x0009B76F File Offset: 0x0009996F
		public void OnMainHeroDisguised(bool isNotCaught)
		{
			this._defaultSkillLevelingManager.OnMainHeroDisguised(isNotCaught);
		}

		// Token: 0x060016AF RID: 5807 RVA: 0x0009B77D File Offset: 0x0009997D
		public void OnRaid(MobileParty attackerParty, ItemRoster lootedItems)
		{
			this._defaultSkillLevelingManager.OnRaid(attackerParty, lootedItems);
		}

		// Token: 0x060016B0 RID: 5808 RVA: 0x0009B78C File Offset: 0x0009998C
		public void OnLoot(MobileParty attackerParty, MobileParty forcedParty, ItemRoster lootedItems, bool attacked)
		{
			this._defaultSkillLevelingManager.OnLoot(attackerParty, forcedParty, lootedItems, attacked);
		}

		// Token: 0x060016B1 RID: 5809 RVA: 0x0009B79E File Offset: 0x0009999E
		public void OnPrisonerSell(MobileParty mobileParty, in TroopRoster prisonerRoster)
		{
			this._defaultSkillLevelingManager.OnPrisonerSell(mobileParty, ref prisonerRoster);
		}

		// Token: 0x060016B2 RID: 5810 RVA: 0x0009B7AD File Offset: 0x000999AD
		public void OnSurgeryApplied(MobileParty party, bool surgerySuccess, int troopTier)
		{
			this._defaultSkillLevelingManager.OnSurgeryApplied(party, surgerySuccess, troopTier);
		}

		// Token: 0x060016B3 RID: 5811 RVA: 0x0009B7BD File Offset: 0x000999BD
		public void OnTacticsUsed(MobileParty party, float xp)
		{
			this._defaultSkillLevelingManager.OnTacticsUsed(party, xp);
		}

		// Token: 0x060016B4 RID: 5812 RVA: 0x0009B7CC File Offset: 0x000999CC
		public void OnHideoutSpotted(MobileParty party, PartyBase spottedParty)
		{
			this._defaultSkillLevelingManager.OnHideoutSpotted(party, spottedParty);
		}

		// Token: 0x060016B5 RID: 5813 RVA: 0x0009B7DB File Offset: 0x000999DB
		public void OnTrackDetected(Track track)
		{
			this._defaultSkillLevelingManager.OnTrackDetected(track);
		}

		// Token: 0x060016B6 RID: 5814 RVA: 0x0009B7E9 File Offset: 0x000999E9
		public void OnTravelOnFoot(Hero hero, float speed)
		{
			this._defaultSkillLevelingManager.OnTravelOnFoot(hero, speed);
		}

		// Token: 0x060016B7 RID: 5815 RVA: 0x0009B7F8 File Offset: 0x000999F8
		public void OnTravelOnHorse(Hero hero, float speed)
		{
			this._defaultSkillLevelingManager.OnTravelOnHorse(hero, speed);
		}

		// Token: 0x060016B8 RID: 5816 RVA: 0x0009B807 File Offset: 0x00099A07
		public void OnHeroHealedWhileWaiting(Hero hero, int healingAmount)
		{
			this._defaultSkillLevelingManager.OnHeroHealedWhileWaiting(hero, healingAmount);
		}

		// Token: 0x060016B9 RID: 5817 RVA: 0x0009B816 File Offset: 0x00099A16
		public void OnRegularTroopHealedWhileWaiting(MobileParty mobileParty, int healedTroopCount, float averageTier)
		{
			this._defaultSkillLevelingManager.OnRegularTroopHealedWhileWaiting(mobileParty, healedTroopCount, averageTier);
		}

		// Token: 0x060016BA RID: 5818 RVA: 0x0009B826 File Offset: 0x00099A26
		public void OnLeadingArmy(MobileParty mobileParty)
		{
			this._defaultSkillLevelingManager.OnLeadingArmy(mobileParty);
		}

		// Token: 0x060016BB RID: 5819 RVA: 0x0009B834 File Offset: 0x00099A34
		public void OnSieging(MobileParty mobileParty)
		{
			this._defaultSkillLevelingManager.OnSieging(mobileParty);
		}

		// Token: 0x060016BC RID: 5820 RVA: 0x0009B842 File Offset: 0x00099A42
		public void OnSiegeEngineBuilt(MobileParty mobileParty, SiegeEngineType siegeEngine)
		{
			this._defaultSkillLevelingManager.OnSiegeEngineBuilt(mobileParty, siegeEngine);
		}

		// Token: 0x060016BD RID: 5821 RVA: 0x0009B851 File Offset: 0x00099A51
		public void OnUpgradeTroops(PartyBase party, CharacterObject troop, CharacterObject upgrade, int numberOfTroops)
		{
			this._defaultSkillLevelingManager.OnUpgradeTroops(party, troop, upgrade, numberOfTroops);
		}

		// Token: 0x060016BE RID: 5822 RVA: 0x0009B863 File Offset: 0x00099A63
		public void OnPersuasionSucceeded(Hero targetHero, SkillObject skill, PersuasionDifficulty difficulty, int argumentDifficultyBonusCoefficient)
		{
			this._defaultSkillLevelingManager.OnPersuasionSucceeded(targetHero, skill, difficulty, argumentDifficultyBonusCoefficient);
		}

		// Token: 0x060016BF RID: 5823 RVA: 0x0009B875 File Offset: 0x00099A75
		public void OnPrisonBreakEnd(Hero prisonerHero, bool isSucceeded)
		{
			this._defaultSkillLevelingManager.OnPrisonBreakEnd(prisonerHero, isSucceeded);
		}

		// Token: 0x060016C0 RID: 5824 RVA: 0x0009B884 File Offset: 0x00099A84
		public void OnWallBreached(MobileParty party)
		{
			this._defaultSkillLevelingManager.OnWallBreached(party);
		}

		// Token: 0x060016C1 RID: 5825 RVA: 0x0009B892 File Offset: 0x00099A92
		public void OnForceVolunteers(MobileParty attackerParty, PartyBase forcedParty)
		{
			this._defaultSkillLevelingManager.OnForceVolunteers(attackerParty, forcedParty);
		}

		// Token: 0x060016C2 RID: 5826 RVA: 0x0009B8A1 File Offset: 0x00099AA1
		public void OnForceSupplies(MobileParty attackerParty, ItemRoster lootedItems, bool attacked)
		{
			this._defaultSkillLevelingManager.OnForceSupplies(attackerParty, lootedItems, attacked);
		}

		// Token: 0x060016C3 RID: 5827 RVA: 0x0009B8B1 File Offset: 0x00099AB1
		public void OnAIPartiesTravel(Hero hero, bool isCaravanParty, TerrainType currentTerrainType)
		{
			this._defaultSkillLevelingManager.OnAIPartiesTravel(hero, isCaravanParty, currentTerrainType);
		}

		// Token: 0x060016C4 RID: 5828 RVA: 0x0009B8C1 File Offset: 0x00099AC1
		public void OnTraverseTerrain(MobileParty mobileParty, TerrainType currentTerrainType)
		{
			this._defaultSkillLevelingManager.OnTraverseTerrain(mobileParty, currentTerrainType);
		}

		// Token: 0x060016C5 RID: 5829 RVA: 0x0009B8D0 File Offset: 0x00099AD0
		public void OnBattleEnded(PartyBase party, CharacterObject troop, int excessXp)
		{
			this._defaultSkillLevelingManager.OnBattleEnded(party, troop, excessXp);
		}

		// Token: 0x060016C6 RID: 5830 RVA: 0x0009B8E0 File Offset: 0x00099AE0
		public void OnFoodConsumed(MobileParty mobileParty, bool wasStarving)
		{
			this._defaultSkillLevelingManager.OnFoodConsumed(mobileParty, wasStarving);
		}

		// Token: 0x060016C7 RID: 5831 RVA: 0x0009B8EF File Offset: 0x00099AEF
		public void OnAlleyCleared(Alley alley)
		{
			this._defaultSkillLevelingManager.OnAlleyCleared(alley);
		}

		// Token: 0x060016C8 RID: 5832 RVA: 0x0009B8FD File Offset: 0x00099AFD
		public void OnDailyAlleyTick(Alley alley, Hero alleyLeader)
		{
			this._defaultSkillLevelingManager.OnDailyAlleyTick(alley, alleyLeader);
		}

		// Token: 0x060016C9 RID: 5833 RVA: 0x0009B90C File Offset: 0x00099B0C
		public void OnBoardGameWonAgainstLord(Hero lord, BoardGameHelper.AIDifficulty difficulty, bool extraXpGain)
		{
			this._defaultSkillLevelingManager.OnBoardGameWonAgainstLord(lord, difficulty, extraXpGain);
		}

		// Token: 0x060016CA RID: 5834 RVA: 0x0009B91C File Offset: 0x00099B1C
		public void OnShipDamaged(Ship ship, float rawDamage, float finalDamage)
		{
			if (ship.Owner != null && ship.Owner.IsMobile)
			{
				float num = Math.Max(rawDamage - finalDamage, 0f);
				NavalSkillLevellingManager.OnPartySkillExercised(ship.Owner.MobileParty, NavalSkills.Boatswain, num * 0.1f, 14);
			}
			this._defaultSkillLevelingManager.OnShipDamaged(ship, rawDamage, finalDamage);
		}

		// Token: 0x060016CB RID: 5835 RVA: 0x0009B978 File Offset: 0x00099B78
		public void OnShipRepaired(Ship ship, float repairedHitPoints)
		{
			float num = repairedHitPoints * 0.05f;
			if (ship.Owner != null && ship.Owner.IsMobile && num > 0f)
			{
				NavalSkillLevellingManager.OnPartySkillExercised(ship.Owner.MobileParty, NavalSkills.Boatswain, num, 14);
			}
			this._defaultSkillLevelingManager.OnShipRepaired(ship, repairedHitPoints);
		}

		// Token: 0x060016CC RID: 5836 RVA: 0x0009B9CF File Offset: 0x00099BCF
		public void OnHideoutMissionEnd(bool isSucceeded)
		{
			this._defaultSkillLevelingManager.OnHideoutMissionEnd(isSucceeded);
		}

		// Token: 0x060016CD RID: 5837 RVA: 0x0009B9DD File Offset: 0x00099BDD
		public void OnHideoutClearedAsGhost()
		{
			this._defaultSkillLevelingManager.OnHideoutClearedAsGhost();
		}

		// Token: 0x060016CE RID: 5838 RVA: 0x0009B9EA File Offset: 0x00099BEA
		public void OnTravelOnWater(MobileParty party, float speed)
		{
			NavalSkillLevellingManager.OnPartySkillExercised(party, NavalSkills.Shipmaster, (float)MBRandom.RoundRandomized(1.4f * speed), 15);
			this._defaultSkillLevelingManager.OnTravelOnWater(party, speed);
		}

		// Token: 0x060016CF RID: 5839 RVA: 0x0009BA13 File Offset: 0x00099C13
		private static void OnPartySkillExercised(MobileParty party, SkillObject skill, float skillXp, PartyRole partyRole = 5)
		{
			Hero effectiveRoleHolder = party.GetEffectiveRoleHolder(partyRole);
			if (effectiveRoleHolder == null)
			{
				return;
			}
			effectiveRoleHolder.AddSkillXp(skill, skillXp);
		}

		// Token: 0x060016D1 RID: 5841 RVA: 0x0009BA3B File Offset: 0x00099C3B
		void ISkillLevelingManager.OnPrisonerSell(MobileParty mobileParty, in TroopRoster prisonerRoster)
		{
			this.OnPrisonerSell(mobileParty, in prisonerRoster);
		}

		// Token: 0x04000BBA RID: 3002
		private readonly DefaultSkillLevelingManager _defaultSkillLevelingManager = new DefaultSkillLevelingManager();

		// Token: 0x04000BBB RID: 3003
		private const float NavalAutoBattleXpCoefficient = 0.02f;
	}
}
