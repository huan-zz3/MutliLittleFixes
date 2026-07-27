using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace NavalDLC.Missions
{
	// Token: 0x02000083 RID: 131
	public class NavalMissionManager : CampaignMission.ICampaignMissionManager
	{
		// Token: 0x06000982 RID: 2434 RVA: 0x00044566 File Offset: 0x00042766
		public NavalMissionManager(CampaignMission.ICampaignMissionManager baseMissionManager)
		{
			this._baseMissionManager = baseMissionManager;
		}

		// Token: 0x06000983 RID: 2435 RVA: 0x00044575 File Offset: 0x00042775
		public IMission OpenNavalRaidMission(TroopRoster navalRaidTroops, BattleSideEnum navalSide, List<Ship> allShips)
		{
			return NavalMissions.OpenNavalRaidMission(navalRaidTroops, navalSide, allShips);
		}

		// Token: 0x06000984 RID: 2436 RVA: 0x0004457F File Offset: 0x0004277F
		public IMission OpenNavalBattleMission(MissionInitializerRecord rec)
		{
			return NavalMissions.OpenNavalBattleMission(rec);
		}

		// Token: 0x06000985 RID: 2437 RVA: 0x00044587 File Offset: 0x00042787
		public IMission OpenNavalSetPieceBattleMission(MissionInitializerRecord rec, MBList<IShipOrigin> playerShips, MBList<IShipOrigin> playerAllyShips, MBList<IShipOrigin> enemyShips)
		{
			return NavalMissions.OpenNavalSetPieceBattleMission(rec, playerShips, playerAllyShips, enemyShips);
		}

		// Token: 0x06000986 RID: 2438 RVA: 0x00044593 File Offset: 0x00042793
		public IMission OpenAlleyFightMission(string scene, int upgradeLevel, Location location, TroopRoster playerSideTroops, TroopRoster rivalSideTroops)
		{
			return this._baseMissionManager.OpenAlleyFightMission(scene, upgradeLevel, location, playerSideTroops, rivalSideTroops);
		}

		// Token: 0x06000987 RID: 2439 RVA: 0x000445A7 File Offset: 0x000427A7
		public IMission OpenArenaDuelMission(string scene, Location location, CharacterObject duelCharacter, bool requireCivilianEquipment, bool spawnBOthSidesWithHorse, Action<CharacterObject> onDuelEndAction, float customAgentHealth)
		{
			return this._baseMissionManager.OpenArenaDuelMission(scene, location, duelCharacter, requireCivilianEquipment, spawnBOthSidesWithHorse, onDuelEndAction, customAgentHealth);
		}

		// Token: 0x06000988 RID: 2440 RVA: 0x000445BF File Offset: 0x000427BF
		public IMission OpenArenaStartMission(string scene, Location location, CharacterObject talkToChar)
		{
			return this._baseMissionManager.OpenArenaStartMission(scene, location, talkToChar);
		}

		// Token: 0x06000989 RID: 2441 RVA: 0x000445CF File Offset: 0x000427CF
		public IMission OpenBattleMission(MissionInitializerRecord rec)
		{
			return this._baseMissionManager.OpenBattleMission(rec);
		}

		// Token: 0x0600098A RID: 2442 RVA: 0x000445DD File Offset: 0x000427DD
		public IMission OpenBattleMission(string scene, bool usesTownDecalAtlas, string sceneLevels)
		{
			return this._baseMissionManager.OpenBattleMission(scene, usesTownDecalAtlas, sceneLevels);
		}

		// Token: 0x0600098B RID: 2443 RVA: 0x000445ED File Offset: 0x000427ED
		public IMission OpenBattleMissionWhileEnteringSettlement(string scene, int upgradeLevel, int numberOfMaxTroopToBeSpawnedForPlayer, int numberOfMaxTroopToBeSpawnedForOpponent)
		{
			return this._baseMissionManager.OpenBattleMissionWhileEnteringSettlement(scene, upgradeLevel, numberOfMaxTroopToBeSpawnedForPlayer, numberOfMaxTroopToBeSpawnedForOpponent);
		}

		// Token: 0x0600098C RID: 2444 RVA: 0x000445FF File Offset: 0x000427FF
		public IMission OpenCaravanBattleMission(MissionInitializerRecord rec, bool isCaravan)
		{
			return this._baseMissionManager.OpenCaravanBattleMission(rec, isCaravan);
		}

		// Token: 0x0600098D RID: 2445 RVA: 0x0004460E File Offset: 0x0004280E
		public IMission OpenCastleCourtyardMission(string scene, int castleUpgradeLevel, Location location, CharacterObject talkToChar)
		{
			return this._baseMissionManager.OpenCastleCourtyardMission(scene, castleUpgradeLevel, location, talkToChar);
		}

		// Token: 0x0600098E RID: 2446 RVA: 0x00044620 File Offset: 0x00042820
		public IMission OpenCombatMissionWithDialogue(string scene, CharacterObject characterToTalkTo, int upgradeLevel)
		{
			return this._baseMissionManager.OpenCombatMissionWithDialogue(scene, characterToTalkTo, upgradeLevel);
		}

		// Token: 0x0600098F RID: 2447 RVA: 0x00044630 File Offset: 0x00042830
		public IMission OpenConversationMission(ConversationCharacterData playerCharacterData, ConversationCharacterData conversationPartnerData, string specialScene = "", string sceneLevels = "", bool isMultiAgentConversation = false)
		{
			return this._baseMissionManager.OpenConversationMission(playerCharacterData, conversationPartnerData, specialScene, sceneLevels, isMultiAgentConversation);
		}

		// Token: 0x06000990 RID: 2448 RVA: 0x00044644 File Offset: 0x00042844
		public IMission OpenHideoutBattleMission(string scene, FlattenedTroopRoster playerTroops, bool isTutorial)
		{
			return this._baseMissionManager.OpenHideoutBattleMission(scene, playerTroops, isTutorial);
		}

		// Token: 0x06000991 RID: 2449 RVA: 0x00044654 File Offset: 0x00042854
		public IMission OpenIndoorMission(string scene, int upgradeLevel, Location location, CharacterObject talkToChar)
		{
			return this._baseMissionManager.OpenIndoorMission(scene, upgradeLevel, location, talkToChar);
		}

		// Token: 0x06000992 RID: 2450 RVA: 0x00044666 File Offset: 0x00042866
		public IMission OpenMeetingMission(string scene, CharacterObject character)
		{
			return this._baseMissionManager.OpenMeetingMission(scene, character);
		}

		// Token: 0x06000993 RID: 2451 RVA: 0x00044675 File Offset: 0x00042875
		public IMission OpenPrisonBreakMission(string scene, Location location, CharacterObject prisonerCharacter)
		{
			return this._baseMissionManager.OpenPrisonBreakMission(scene, location, prisonerCharacter);
		}

		// Token: 0x06000994 RID: 2452 RVA: 0x00044685 File Offset: 0x00042885
		public IMission OpenRetirementMission(string scene, Location location, CharacterObject talkToChar = null, string sceneLevels = null, string unconsciousMenuId = "")
		{
			return this._baseMissionManager.OpenRetirementMission(scene, location, talkToChar, sceneLevels, "");
		}

		// Token: 0x06000995 RID: 2453 RVA: 0x0004469C File Offset: 0x0004289C
		public IMission OpenSiegeLordsHallFightMission(string scene, FlattenedTroopRoster attackerPriorityList)
		{
			return this._baseMissionManager.OpenSiegeLordsHallFightMission(scene, attackerPriorityList);
		}

		// Token: 0x06000996 RID: 2454 RVA: 0x000446AB File Offset: 0x000428AB
		public IMission OpenSiegeMissionNoDeployment(string scene, bool isSallyOut = false, bool isReliefForceAttack = false)
		{
			return this._baseMissionManager.OpenSiegeMissionNoDeployment(scene, isSallyOut, isReliefForceAttack);
		}

		// Token: 0x06000997 RID: 2455 RVA: 0x000446BC File Offset: 0x000428BC
		public IMission OpenSiegeMissionWithDeployment(string scene, float[] wallHitPointsPercentages, bool hasAnySiegeTower, List<MissionSiegeWeapon> siegeWeaponsOfAttackers, List<MissionSiegeWeapon> siegeWeaponsOfDefenders, bool isPlayerAttacker, int upgradeLevel = 0, bool isSallyOut = false, bool isReliefForceAttack = false)
		{
			return this._baseMissionManager.OpenSiegeMissionWithDeployment(scene, wallHitPointsPercentages, hasAnySiegeTower, siegeWeaponsOfAttackers, siegeWeaponsOfDefenders, isPlayerAttacker, upgradeLevel, isSallyOut, isReliefForceAttack);
		}

		// Token: 0x06000998 RID: 2456 RVA: 0x000446E3 File Offset: 0x000428E3
		public IMission OpenTownCenterMission(string scene, int townUpgradeLevel, Location location, CharacterObject talkToChar, string playerSpawnTag)
		{
			return this._baseMissionManager.OpenTownCenterMission(scene, townUpgradeLevel, location, talkToChar, playerSpawnTag);
		}

		// Token: 0x06000999 RID: 2457 RVA: 0x000446F7 File Offset: 0x000428F7
		public IMission OpenVillageMission(string scene, Location location, CharacterObject talkToChar)
		{
			return this._baseMissionManager.OpenVillageMission(scene, location, talkToChar);
		}

		// Token: 0x0600099A RID: 2458 RVA: 0x00044707 File Offset: 0x00042907
		public IMission OpenHideoutAmbushMission(string sceneName, FlattenedTroopRoster playerTroops, Location location)
		{
			return this._baseMissionManager.OpenHideoutAmbushMission(sceneName, playerTroops, location);
		}

		// Token: 0x0600099B RID: 2459 RVA: 0x00044717 File Offset: 0x00042917
		public IMission OpenDisguiseMission(string scene, bool willSetUpContact, string sceneLevels, Location fromLocation)
		{
			return this._baseMissionManager.OpenDisguiseMission(scene, willSetUpContact, sceneLevels, fromLocation);
		}

		// Token: 0x04000592 RID: 1426
		private readonly CampaignMission.ICampaignMissionManager _baseMissionManager;
	}
}
