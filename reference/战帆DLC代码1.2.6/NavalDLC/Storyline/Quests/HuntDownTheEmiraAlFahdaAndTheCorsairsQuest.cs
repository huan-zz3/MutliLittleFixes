using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using NavalDLC.Missions;
using SandBox;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Objects;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace NavalDLC.Storyline.Quests
{
	// Token: 0x02000038 RID: 56
	public class HuntDownTheEmiraAlFahdaAndTheCorsairsQuest : NavalStorylineQuestBase
	{
		// Token: 0x17000095 RID: 149
		// (get) Token: 0x060003B2 RID: 946 RVA: 0x0001AADC File Offset: 0x00018CDC
		public override bool WillProgressStoryline
		{
			get
			{
				return this._willProgressStoryline;
			}
		}

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x060003B3 RID: 947 RVA: 0x0001AAE4 File Offset: 0x00018CE4
		public override TextObject Title
		{
			get
			{
				TextObject textObject = new TextObject("{=kEyCQWh1}Hunt Down {HERO.NAME}", null);
				TextObjectExtensions.SetCharacterProperties(textObject, "HERO", NavalStorylineData.EmiraAlFahda.CharacterObject, false);
				return textObject;
			}
		}

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x060003B4 RID: 948 RVA: 0x0001AB07 File Offset: 0x00018D07
		private TextObject DescriptionLogText
		{
			get
			{
				TextObject textObject = new TextObject("{=ezctGj6M}Find the corsair {HERO.NAME} and defeat her.", null);
				TextObjectExtensions.SetCharacterProperties(textObject, "HERO", NavalStorylineData.EmiraAlFahda.CharacterObject, false);
				return textObject;
			}
		}

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x060003B5 RID: 949 RVA: 0x0001AB2A File Offset: 0x00018D2A
		private TextObject MainCorsairShipSpawnedLogText
		{
			get
			{
				TextObject textObject = new TextObject("{=BKlHaMZ6}Overtake and defeat {HERO.NAME} and her fleet.", null);
				TextObjectExtensions.SetCharacterProperties(textObject, "HERO", NavalStorylineData.EmiraAlFahda.CharacterObject, false);
				return textObject;
			}
		}

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x060003B6 RID: 950 RVA: 0x0001AB4D File Offset: 0x00018D4D
		private TextObject QuestSucceededWithRansomLogText
		{
			get
			{
				TextObject textObject = new TextObject("{=UvFN0bf1}You decided to accept {HERO.NAME}'s ransom money. ({GOLD_REWARD}{GOLD_ICON}).", null);
				TextObjectExtensions.SetCharacterProperties(textObject, "HERO", NavalStorylineData.EmiraAlFahda.CharacterObject, false);
				textObject.SetTextVariable("GOLD_REWARD", 1000);
				return textObject;
			}
		}

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x060003B7 RID: 951 RVA: 0x0001AB84 File Offset: 0x00018D84
		private TextObject QuestSucceededWithReturnOfEmiraLogText
		{
			get
			{
				TextObject textObject = new TextObject("{=DKA4tOwq}You decided to return {HERO.NAME} to her uncles alive.(+{RELATIONSHIP_REWARD} relationship with all notables in {SETTLEMENT_LINK}).", null);
				TextObjectExtensions.SetCharacterProperties(textObject, "HERO", NavalStorylineData.EmiraAlFahda.CharacterObject, false);
				textObject.SetTextVariable("RELATIONSHIP_REWARD", 10);
				textObject.SetTextVariable("SETTLEMENT_LINK", NavalStorylineData.Act3Quest2TargetSettlement.EncyclopediaLinkWithName);
				return textObject;
			}
		}

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x060003B8 RID: 952 RVA: 0x0001ABD6 File Offset: 0x00018DD6
		private TextObject PlayerStartsQuestLogText
		{
			get
			{
				TextObject textObject = new TextObject("{=pfIWdGnV}The corsairs appear to be scattered. Find them and take them, until you sight {HERO.NAME}.", null);
				TextObjectExtensions.SetCharacterProperties(textObject, "HERO", NavalStorylineData.EmiraAlFahda.CharacterObject, false);
				return textObject;
			}
		}

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x060003B9 RID: 953 RVA: 0x0001ABF9 File Offset: 0x00018DF9
		public override NavalStorylineData.NavalStorylineStage Stage
		{
			get
			{
				return NavalStorylineData.NavalStorylineStage.Act3Quest2;
			}
		}

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x060003BA RID: 954 RVA: 0x0001ABFC File Offset: 0x00018DFC
		protected override string MainPartyTemplateStringId
		{
			get
			{
				return "storyline_act3_quest_2_main_party_template";
			}
		}

		// Token: 0x060003BB RID: 955 RVA: 0x0001AC04 File Offset: 0x00018E04
		public HuntDownTheEmiraAlFahdaAndTheCorsairsQuest(string questId, Hero questGiver, CampaignVec2 corsairSpawnPosition)
			: base(questId, questGiver, CampaignTime.Never, 0)
		{
			this._willProgressStoryline = false;
			this._numberOfDefeatedCorsairParties = 0;
			this._corsairParties = new List<MobileParty>();
			this._bossCorsairParty = null;
			this._corsairSpawnPosition = corsairSpawnPosition;
			this._corsairHuntingGroundMarker = Campaign.Current.MapMarkerManager.CreateMapMarker(NavalStorylineData.CorsairBanner, new TextObject("{=QLrwlirp}Corsair Hunting Grounds", null), this._corsairSpawnPosition.AsVec3(), false, base.StringId);
			base.AddLog(this.DescriptionLogText, false);
		}

		// Token: 0x060003BC RID: 956 RVA: 0x0001AC8C File Offset: 0x00018E8C
		protected override void OnFinalizeInternal()
		{
			this._playerStartsQuestLog = null;
			this.DestroyCorsairParties();
			Scene scene = ((MapScene)Campaign.Current.MapSceneWrapper).Scene;
			List<GameEntity> list = new List<GameEntity>();
			scene.GetAllEntitiesWithScriptComponent<CampaignMapAmbientOccluder>(ref list);
			foreach (GameEntity gameEntity in list)
			{
				gameEntity.GetFirstScriptOfType<CampaignMapAmbientOccluder>().UnregisterQuestStorm(this._stormEntity);
			}
			GameEntity stormEntity = this._stormEntity;
			if (stormEntity == null)
			{
				return;
			}
			stormEntity.Remove(111);
		}

		// Token: 0x060003BD RID: 957 RVA: 0x0001AD24 File Offset: 0x00018F24
		protected override void InitializeQuestOnGameLoadInternal()
		{
			this.SetDialogs();
			this.AddGameMenus();
			if (this._numberOfDefeatedCorsairParties == 2)
			{
				this.SpawnStormEntity();
			}
		}

		// Token: 0x060003BE RID: 958 RVA: 0x0001AD41 File Offset: 0x00018F41
		protected override void SetDialogs()
		{
			this.AddDialogsForFinalFight();
		}

		// Token: 0x060003BF RID: 959 RVA: 0x0001AD49 File Offset: 0x00018F49
		protected override void OnStartQuestInternal()
		{
			this.SetDialogs();
			this.AddGameMenus();
			this._numberOfDefeatedCorsairParties = 2;
			this.SpawnMainCorsairParty();
			this.SpawnStormEntity();
			this._willProgressStoryline = true;
			base.AddTrackedObject(this._corsairHuntingGroundMarker);
		}

		// Token: 0x060003C0 RID: 960 RVA: 0x0001AD80 File Offset: 0x00018F80
		protected override void HourlyTick()
		{
			if (this._corsairHuntingGroundMarker.Position.Distance(MobileParty.MainParty.Position.AsVec3()) > 15f)
			{
				this._corsairHuntingGroundMarker.IsVisibleOnMap = true;
			}
			else
			{
				this._corsairHuntingGroundMarker.IsVisibleOnMap = false;
			}
			foreach (MobileParty mobileParty in this._corsairParties)
			{
				if (MBRandom.RandomFloat < 0.25f && mobileParty.IsActive && !mobileParty.IsMoving && !mobileParty.Ai.IsDisabled)
				{
					CampaignVec2 campaignVec = NavigationHelper.FindReachablePointAroundPosition(this._corsairSpawnPosition, 2, 10f, 3f, false);
					mobileParty.SetMoveGoToPoint(campaignVec, 2);
				}
			}
		}

		// Token: 0x060003C1 RID: 961 RVA: 0x0001AE60 File Offset: 0x00019060
		protected override void IsNavalQuestPartyInternal(PartyBase party, NavalStorylinePartyData data)
		{
			if (this._corsairParties.Any<MobileParty>((MobileParty c) => c.Party == party))
			{
				PartyTemplateObject @object = Campaign.Current.ObjectManager.GetObject<PartyTemplateObject>("storyline_act3_quest_2_corsair_generic_template_" + (party.Id.Contains("0") ? 0 : 1));
				data.PartySize = (int)NavalDLCHelpers.GetMaxPartySizeLimitFromTemplate(@object).ResultNumber;
				data.IsQuestParty = true;
			}
			else if (this._bossCorsairParty != null && this._bossCorsairParty.Party == party)
			{
				PartyTemplateObject object2 = Campaign.Current.ObjectManager.GetObject<PartyTemplateObject>("storyline_act3_quest_2_boss_corsair_template");
				data.PartySize = (int)NavalDLCHelpers.GetMaxPartySizeLimitFromTemplate(object2).ResultNumber + 1;
				data.IsQuestParty = true;
			}
			if (party == PartyBase.MainParty)
			{
				data.PartySize++;
			}
		}

		// Token: 0x060003C2 RID: 962 RVA: 0x0001AF54 File Offset: 0x00019154
		protected override void OnCompleteWithSuccessInternal()
		{
			MobileParty.MainParty.MemberRoster.RemoveTroop(NavalStorylineData.Lahar.CharacterObject, 1, default(UniqueTroopDescriptor), 0);
			NavalStorylineData.Lahar.ChangeState(6);
			NavalStorylineData.OnCheckpointReached(NavalStorylineData.NavalStorylineCheckpoint.Act3Quest2Succeeded);
		}

		// Token: 0x060003C3 RID: 963 RVA: 0x0001AF98 File Offset: 0x00019198
		protected override void OnFailedInternal()
		{
			MobileParty.MainParty.MemberRoster.RemoveTroop(NavalStorylineData.Lahar.CharacterObject, 1, default(UniqueTroopDescriptor), 0);
			NavalStorylineData.Lahar.ChangeState(6);
		}

		// Token: 0x060003C4 RID: 964 RVA: 0x0001AFD4 File Offset: 0x000191D4
		protected override void RegisterEventsInternal()
		{
			CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(this, new Action<MobileParty, PartyBase>(this.OnMobilePartyDestroyed));
			CampaignEvents.MapEventStarted.AddNonSerializedListener(this, new Action<MapEvent, PartyBase, PartyBase>(this.OnMapEventStarted));
			CampaignEvents.OnMissionEndedEvent.AddNonSerializedListener(this, new Action<IMission>(this.OnMissionEnded));
			CampaignEvents.GameMenuOpened.AddNonSerializedListener(this, new Action<MenuCallbackArgs>(this.OnGameMenuOpened));
			CampaignEvents.OnShipOwnerChangedEvent.AddNonSerializedListener(this, new Action<Ship, PartyBase, ChangeShipOwnerAction.ShipOwnerChangeDetail>(this.OnShipOwnerChanged));
			CampaignEvents.BeforeGameMenuOpenedEvent.AddNonSerializedListener(this, new Action<MenuCallbackArgs>(this.OnBeforeGameMenuOpened));
		}

		// Token: 0x060003C5 RID: 965 RVA: 0x0001B06C File Offset: 0x0001926C
		private void OnMapEventStarted(MapEvent mapEvent, PartyBase partyBase1, PartyBase partyBase2)
		{
			if (partyBase1.IsNavalStorylineQuestParty())
			{
				foreach (Ship ship in partyBase1.Ships)
				{
					ship.IsInvulnerable = false;
				}
			}
			if (partyBase2.IsNavalStorylineQuestParty())
			{
				foreach (Ship ship2 in partyBase2.Ships)
				{
					ship2.IsInvulnerable = false;
				}
			}
		}

		// Token: 0x060003C6 RID: 966 RVA: 0x0001B110 File Offset: 0x00019310
		private void OnShipOwnerChanged(Ship ship, PartyBase partyBase, ChangeShipOwnerAction.ShipOwnerChangeDetail detail)
		{
			if (partyBase == PartyBase.MainParty && ship.IsInvulnerable)
			{
				ship.IsInvulnerable = false;
			}
		}

		// Token: 0x060003C7 RID: 967 RVA: 0x0001B12C File Offset: 0x0001932C
		private void AddGameMenus()
		{
			base.AddGameMenu("naval_storyline_act_3_quest_2_encounter_menu", new TextObject("{=YjcPI4pT}An east wind sweeps across the sea, bearing desert dust, and briefly obscures your vision. Soon after it lifts, you hear your lookouts shouting excitedly to you. They have spotted Fahda’s fleet, which appears to have been damaged by the gale. If you attack now, you may be able to sink the flagship before it can escape.", null), new OnInitDelegate(this.naval_storyline_act_3_quest_2_set_piece_encounter_menu_on_init), 4, 0);
			base.AddGameMenuOption("naval_storyline_act_3_quest_2_encounter_menu", "naval_storyline_act_3_quest_2_encounter_menu_continue", new TextObject("{=1r0tDsrR}Attack!", null), new GameMenuOption.OnConditionDelegate(this.naval_storyline_act_3_quest_2_set_piece_encounter_menu_attack_on_condition), new GameMenuOption.OnConsequenceDelegate(this.naval_storyline_act_3_quest_2_set_piece_encounter_menu_attack_on_consequence), false, -1);
			base.AddGameMenu("naval_storyline_act_3_quest_2_retry_menu", new TextObject("{=etH1IHNZ}You manage to put some distance between you and your enemies, and you have a moment to consider how to proceed.", null), new OnInitDelegate(this.naval_storyline_act_3_quest_2_set_piece_retry_menu_on_init), 0, 0);
			base.AddGameMenuOption("naval_storyline_act_3_quest_2_retry_menu", "try_again_option", new TextObject("{=YHMDy3lQ}Try again", null), new GameMenuOption.OnConditionDelegate(this.naval_storyline_act_3_quest_2_set_piece_retry_menu_retry_on_condition), new GameMenuOption.OnConsequenceDelegate(this.naval_storyline_act_3_quest_2_set_piece_retry_menu_retry_on_consequence), false, -1);
			base.AddGameMenuOption("naval_storyline_act_3_quest_2_retry_menu", "leave_option", new TextObject("{=3sRdGQou}Leave", null), new GameMenuOption.OnConditionDelegate(this.naval_storyline_act_3_quest_2_set_piece_retry_menu_leave_on_condition), new GameMenuOption.OnConsequenceDelegate(this.naval_storyline_act_3_quest_2_set_piece_retry_menu_leave_on_consequence), true, -1);
		}

		// Token: 0x060003C8 RID: 968 RVA: 0x0001B220 File Offset: 0x00019420
		private void naval_storyline_act_3_quest_2_set_piece_retry_menu_leave_on_consequence(MenuCallbackArgs args)
		{
			base.CompleteQuestWithCancel(null);
			NavalStorylineData.DeactivateNavalStoryline();
		}

		// Token: 0x060003C9 RID: 969 RVA: 0x0001B22E File Offset: 0x0001942E
		private bool naval_storyline_act_3_quest_2_set_piece_retry_menu_retry_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 1;
			return !this._battleWon && this._battleStarted;
		}

		// Token: 0x060003CA RID: 970 RVA: 0x0001B247 File Offset: 0x00019447
		private bool naval_storyline_act_3_quest_2_set_piece_encounter_menu_attack_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 17;
			return !this._battleStarted && !this._battleWon;
		}

		// Token: 0x060003CB RID: 971 RVA: 0x0001B264 File Offset: 0x00019464
		private void OnBeforeGameMenuOpened(MenuCallbackArgs args)
		{
			MenuContext menuContext = args.MenuContext;
			string text;
			if (menuContext == null)
			{
				text = null;
			}
			else
			{
				GameMenu gameMenu = menuContext.GameMenu;
				text = ((gameMenu != null) ? gameMenu.StringId : null);
			}
			if (text == "naval_storyline_encounter_meeting" && NavalStorylineData.IsNavalStoryLineActive() && PlayerEncounter.EncounteredParty != null && PlayerEncounter.EncounteredParty.IsNavalStorylineQuestParty())
			{
				PlayerEncounter.SetMeetingDone();
			}
		}

		// Token: 0x060003CC RID: 972 RVA: 0x0001B2BA File Offset: 0x000194BA
		private bool naval_storyline_act_3_quest_2_set_piece_retry_menu_leave_on_condition(MenuCallbackArgs args)
		{
			args.Tooltip = new TextObject("{=wmTjX28f}This will exit story mode and return you to the Sandbox. You can continue the storyline later by talking to Gunnar in the port again.", null);
			args.optionLeaveType = 16;
			return !this._battleWon && this._battleStarted;
		}

		// Token: 0x060003CD RID: 973 RVA: 0x0001B2E5 File Offset: 0x000194E5
		private void naval_storyline_act_3_quest_2_set_piece_encounter_menu_on_init(MenuCallbackArgs args)
		{
			args.MenuContext.SetBackgroundMeshName("encounter_naval");
			NavalStorylineData.OnCheckpointReached(NavalStorylineData.NavalStorylineCheckpoint.Act3Quest2EncounterMenu);
		}

		// Token: 0x060003CE RID: 974 RVA: 0x0001B300 File Offset: 0x00019500
		private void naval_storyline_act_3_quest_2_set_piece_retry_menu_on_init(MenuCallbackArgs args)
		{
			args.MenuContext.SetBackgroundMeshName("encounter_naval");
			if (this._battleWon)
			{
				PlayerEncounter.Finish(true);
				this.RefreshShips(MobileParty.MainParty, Campaign.Current.ObjectManager.GetObject<PartyTemplateObject>(this.MainPartyTemplateStringId));
				this.AddShipUpgradesForMainParty();
				NavalStorylineData.EmiraAlFahda.SetHasMet();
				NavalStorylineData.EmiraAlFahda.MakeWounded(null, 0);
				ConversationCharacterData conversationCharacterData = new ConversationCharacterData(CharacterObject.PlayerCharacter, PartyBase.MainParty, true, false, false, false, false, true);
				ConversationCharacterData conversationCharacterData2;
				conversationCharacterData2..ctor(NavalStorylineData.EmiraAlFahda.CharacterObject, null, true, true, true, false, false, true);
				CampaignMission.OpenConversationMission(conversationCharacterData, conversationCharacterData2, "conversation_scene_sea_multi_agent", "", true);
				return;
			}
			this.RefreshParty(this._bossCorsairParty, Campaign.Current.ObjectManager.GetObject<PartyTemplateObject>("storyline_act3_quest_2_boss_corsair_template"));
			this.AddShipUpgradesForMainCorsairParty();
			this.RefreshParty(MobileParty.MainParty, Campaign.Current.ObjectManager.GetObject<PartyTemplateObject>(this.MainPartyTemplateStringId));
			this.AddShipUpgradesForMainParty();
		}

		// Token: 0x060003CF RID: 975 RVA: 0x0001B3F6 File Offset: 0x000195F6
		private void naval_storyline_act_3_quest_2_set_piece_encounter_menu_attack_on_consequence(MenuCallbackArgs args)
		{
			this.StartBattle();
		}

		// Token: 0x060003D0 RID: 976 RVA: 0x0001B3FE File Offset: 0x000195FE
		private void naval_storyline_act_3_quest_2_set_piece_retry_menu_retry_on_consequence(MenuCallbackArgs args)
		{
			this.StartBattle();
		}

		// Token: 0x060003D1 RID: 977 RVA: 0x0001B408 File Offset: 0x00019608
		private void OnGameMenuOpened(MenuCallbackArgs args)
		{
			if (NavalStorylineData.IsNavalStoryLineActive() && PlayerEncounter.EncounteredParty != null && PlayerEncounter.EncounteredParty.IsNavalStorylineQuestParty())
			{
				MenuContext menuContext = args.MenuContext;
				string text;
				if (menuContext == null)
				{
					text = null;
				}
				else
				{
					GameMenu gameMenu = menuContext.GameMenu;
					text = ((gameMenu != null) ? gameMenu.StringId : null);
				}
				string text2 = text;
				MobileParty bossCorsairParty = this._bossCorsairParty;
				if (((bossCorsairParty != null) ? bossCorsairParty.Party : null) == PlayerEncounter.EncounteredParty)
				{
					if (text2 == "naval_storyline_encounter")
					{
						GameMenu.ActivateGameMenu("naval_storyline_act_3_quest_2_encounter_menu");
						return;
					}
				}
				else
				{
					MBTextManager.SetTextVariable("ENCOUNTER_TEXT", new TextObject("{=XVCdua8m}One of your sharper-eyed sailors thinks he sees a ship. You stare at the horizon, and though at first it's hard to make out shapes against the choppy waves of the gulf, you eventually distinguish the unmistakable outline of a lateen sail. It's a corsair, and it's heading directly towards you. ", null), true);
				}
			}
		}

		// Token: 0x060003D2 RID: 978 RVA: 0x0001B498 File Offset: 0x00019698
		private void OnMissionEnded(IMission mission)
		{
			if (Mission.Current.IsNavalBattle && PlayerEncounter.Current != null && PlayerEncounter.EncounteredParty != null)
			{
				MobileParty bossCorsairParty = this._bossCorsairParty;
				if (((bossCorsairParty != null) ? bossCorsairParty.Party : null) == PlayerEncounter.EncounteredParty)
				{
					if (PlayerEncounter.CampaignBattleResult != null && PlayerEncounter.CampaignBattleResult.BattleResolved)
					{
						if (PlayerEncounter.CampaignBattleResult.PlayerDefeat)
						{
							this._battleWon = false;
							return;
						}
						if (PlayerEncounter.CampaignBattleResult.PlayerVictory)
						{
							MobileParty bossCorsairParty2 = this._bossCorsairParty;
							if (((bossCorsairParty2 != null) ? bossCorsairParty2.Party : null) == PlayerEncounter.EncounteredParty)
							{
								this._battleWon = true;
								return;
							}
						}
					}
					else
					{
						if (PlayerEncounter.WinningSide == -1)
						{
							this._battleWon = false;
							return;
						}
						Debug.FailedAssert("unhandled case", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\Storyline\\Quests\\HuntDownTheEmiraAlFahdaAndTheCorsairsQuest.cs", "OnMissionEnded", 475);
					}
				}
			}
		}

		// Token: 0x060003D3 RID: 979 RVA: 0x0001B560 File Offset: 0x00019760
		private void OnMobilePartyDestroyed(MobileParty party, PartyBase partyBase)
		{
			if (NavalStorylineData.IsNavalStoryLineActive() && this._playerStartsQuestLog != null && this._corsairParties.Contains(party) && partyBase == PartyBase.MainParty)
			{
				MBInformationManager.AddQuickInformation(new TextObject("{=MRX4gImP}So far so good, but there are still enemies about.", null), 0, NavalStorylineData.Gunnar.CharacterObject, null, "");
				this._numberOfDefeatedCorsairParties++;
				this._corsairParties.Remove(party);
				base.UpdateQuestTaskStage(this._playerStartsQuestLog, this._numberOfDefeatedCorsairParties);
				if (2 == this._numberOfDefeatedCorsairParties)
				{
					this.SpawnStormEntity();
					this.SpawnMainCorsairParty();
					base.AddLog(this.MainCorsairShipSpawnedLogText, false);
				}
			}
		}

		// Token: 0x060003D4 RID: 980 RVA: 0x0001B60C File Offset: 0x0001980C
		private void AddDialogsForFinalFight()
		{
			string text;
			string text2;
			string text3;
			string text4;
			string text5;
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1200).NpcLine(new TextObject("{=unOIbuqz}What have you done? Do you know who I am? I have allies who'll unthread your entrails from your guts and hang you with them from your own yardarm. I am queen of these waters, you fools, and those who practice piracy here without my permission end up chum to attract the sharks.", null), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsEmiraAlFahda), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsLahar), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.MultiAgentConversationCondition))
				.GenerateToken(ref text)
				.GenerateToken(ref text2)
				.GenerateToken(ref text3)
				.GenerateToken(ref text4)
				.GenerateToken(ref text5)
				.NpcLine(new TextObject("{=xQunuNT9}My lady, we are not pirates. Rather, I am a man who has done many services for families such as your own in Quyaz. At present I am working for your uncles. I do not know what they intend to do with you, although I do not expect that a town that lives on trade will deal leniently with piracy.", null), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsLahar), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsEmiraAlFahda), null, null)
				.NpcLine(new TextObject("{=nyOUdUQI}Before we sail, however, I would like you to have a chat with my friend here.", null), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsLahar), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsEmiraAlFahda), null, null)
				.NpcLine(new TextObject("{=LFLn7SJc}So you are on contract to deliver me alive to Quyaz, are you? I can tell you this, then - my lineage goes back to the founding of that city, and if you spill so much as a drop of my blood, your own shall be drained from your body like that of a horse-fish. As for the Sea Hounds, they are my allies and servants, and I shall not betray them to you.", null), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsEmiraAlFahda), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsLahar), null, null)
				.NpcLine(new TextObject("{=C88poDCA}How much are my uncles paying you, anyway? I have a chest of silver set aside for occasions such as this, and I suspect I could pay you more than they will. They are stingy men.", null), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsEmiraAlFahda), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsLahar), null, null)
				.NpcLine(new TextObject("{=v2664Qeo}...", null), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsEmiraAlFahda), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsLahar), null, null)
				.GotoDialogState(text)
				.BeginPlayerOptions(text, true)
				.PlayerOption(new TextObject("{=q3uOXLEO}I am here too, and I have no contract to deliver you anywhere alive.", null), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsEmiraAlFahda), null, null)
				.GotoDialogState(text2)
				.PlayerOption(new TextObject("{=XfIbjoH8}You tell me all you know about the Sea Hounds and their dealings in slaves.", null), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsEmiraAlFahda), null, null)
				.GotoDialogState(text2)
				.EndPlayerOptions()
				.NpcLine(new TextObject("{=06AGZvSg}Are you threatening me? You won't get a single coin from my uncles if you harm me.", null), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsEmiraAlFahda), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), text2, null)
				.NpcLine(new TextObject("{=T0a3QpjV}Unlike Lahar, here, we have not shed our blood today merely for a part-share of a ransom, or to boost our standing with the merchants of Quyaz. You are an ally of the Sea Hounds, and it serves us well to make an example of you. Your life is forfeit unless you tell us something we can use.", null), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsGunnar), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsEmiraAlFahda), null, null)
				.NpcLine(new TextObject("{=IPq1hnUG}How do I know that telling you about the Sea Hounds will save my life?", null), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsEmiraAlFahda), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), null, text3)
				.BeginPlayerOptions(text3, false)
				.PlayerOption(new TextObject("{=Su0h3ZMC}If you speak truthfully, you will live.", null), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsEmiraAlFahda), null, null)
				.GotoDialogState(text4)
				.PlayerOption(new TextObject("{=9tmYkhb1}You'll just have to try and see.", null), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsEmiraAlFahda), null, null)
				.GotoDialogState(text4)
				.EndPlayerOptions()
				.NpcLine(new TextObject("{=tlXQV9mO}I can tell you this – I don’t have your sister. I used to buy captives from the Sea Hounds. But now they have this new leader named Purig, who keeps them all for his own purposes. Apparently he has some anchorage up in the north, where he intends to use slaves to build larger and stronger ships.", null), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsEmiraAlFahda), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), text4, null)
				.NpcLine(new TextObject("{=w5GbjHDG}Purig, a leader among the Sea Hounds! I'll speak straight here - it gnaws at my gut to hear that he is prospering from his treachery.", null), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsGunnar), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), null, null)
				.NpcLine(new TextObject("{=C2OtgWn0}He acts as though the Sea Hounds have already crowned him their king. He demanded that I hunt for captives here in the south and sell them to him, promising to pay me with a huge store of silver that some new partners of his, a vile-looking gang of Vlandian pirates, hoped to steal from the merchants of Omor.", null), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsEmiraAlFahda), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), null, null)
				.NpcLine(new TextObject("{=Ex5CzHBt}We should get more information on this Omor silver. If we can stop these Vlandians it would deal a great blow to Purig, and we could possibly find out more about this northern anchorage, his captives, and maybe your sister.", null), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsGunnar), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), null, null)
				.NpcLine(new TextObject("{=2bzElv6k}So that information is worth something to you, is it not? If we add in that ransom I mentioned, is it enough to buy my life and my freedom?", null), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsEmiraAlFahda), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), null, null)
				.NpcLine(new TextObject("{=M24S1pEI}You know my preference, {PLAYER.NAME}. If I bring her back to Quyaz, I will ensure that you get some of the credit, but perhaps you prefer good cold silver to goodwill.", null), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsLahar), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), null, text5)
				.BeginPlayerOptions(text5, false)
				.PlayerOption(new TextObject("{=VHbGnf4W}Return her to her uncles alive, as per your original understanding.", null), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsLahar), null, null)
				.NpcLine(new TextObject("{=7f9yXAvI}I accept your decision. Very well then…", null), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsLahar), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), null, null)
				.Consequence(delegate
				{
					this.OnPlayerSelectsOption1();
				})
				.CloseDialog()
				.PlayerOption(new TextObject("{=xpz9JFGK}The lady offers a fair ransom. Let us accept.", null), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsLahar), null, null)
				.NpcLine(new TextObject("{=7f9yXAvI}I accept your decision. Very well then…", null), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsLahar), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), null, null)
				.NpcLine(new TextObject("{=cxG2qhbv}Listen. I have enjoyed our excursion, and hunting pirates is always good business. Though I must depart now to Quyaz, I would like to go hunting with you again. Gunnar tells me that you will be sailing from Ostican once you locate your next quarry. Hopefully I will see you there soon.", null), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsLahar), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), null, null)
				.Consequence(delegate
				{
					this.OnPlayerSelectsOption2();
				})
				.CloseDialog()
				.EndPlayerOptions()
				.CloseDialog(), this);
		}

		// Token: 0x060003D5 RID: 981 RVA: 0x0001BA8C File Offset: 0x00019C8C
		private void OnPlayerSelectsOption1()
		{
			foreach (Hero hero in NavalStorylineData.Act3Quest2TargetSettlement.Notables)
			{
				ChangeRelationAction.ApplyRelationChangeBetweenHeroes(Hero.MainHero, hero, 10, true);
			}
			base.AddLog(this.QuestSucceededWithReturnOfEmiraLogText, false);
			base.CompleteQuestWithSuccess();
		}

		// Token: 0x060003D6 RID: 982 RVA: 0x0001BB00 File Offset: 0x00019D00
		private void OnPlayerSelectsOption2()
		{
			GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, 1000, false);
			base.AddLog(this.QuestSucceededWithRansomLogText, false);
			base.CompleteQuestWithSuccess();
		}

		// Token: 0x060003D7 RID: 983 RVA: 0x0001BB27 File Offset: 0x00019D27
		private bool IsLahar(IAgent agent)
		{
			return agent.Character == NavalStorylineData.Lahar.CharacterObject;
		}

		// Token: 0x060003D8 RID: 984 RVA: 0x0001BB3B File Offset: 0x00019D3B
		private bool IsGunnar(IAgent agent)
		{
			return agent.Character == NavalStorylineData.Gunnar.CharacterObject;
		}

		// Token: 0x060003D9 RID: 985 RVA: 0x0001BB4F File Offset: 0x00019D4F
		private bool IsMainHero(IAgent agent)
		{
			return agent.Character == CharacterObject.PlayerCharacter;
		}

		// Token: 0x060003DA RID: 986 RVA: 0x0001BB5E File Offset: 0x00019D5E
		private bool IsEmiraAlFahda(IAgent agent)
		{
			return agent.Character == NavalStorylineData.EmiraAlFahda.CharacterObject;
		}

		// Token: 0x060003DB RID: 987 RVA: 0x0001BB74 File Offset: 0x00019D74
		private Agent SpawnGunnar()
		{
			AgentBuildData agentBuildData = new AgentBuildData(NavalStorylineData.Gunnar.CharacterObject);
			agentBuildData.TroopOrigin(new SimpleAgentOrigin(agentBuildData.AgentCharacter, -1, null, default(UniqueTroopDescriptor)));
			Vec3 globalPosition = Mission.Current.Scene.FindEntityWithName("free_infantry_spawn_point_1").GlobalPosition;
			agentBuildData.InitialPosition(ref globalPosition);
			AgentBuildData agentBuildData2 = agentBuildData;
			Vec2 vec = Agent.Main.LookDirection.AsVec2;
			vec = vec.Normalized();
			agentBuildData2.InitialDirection(ref vec);
			agentBuildData.NoHorses(true);
			return Mission.Current.SpawnAgent(agentBuildData, false);
		}

		// Token: 0x060003DC RID: 988 RVA: 0x0001BC0C File Offset: 0x00019E0C
		private bool MultiAgentConversationCondition()
		{
			if (Hero.OneToOneConversationHero == NavalStorylineData.EmiraAlFahda && MobileParty.MainParty.IsCurrentlyAtSea && Mission.Current != null)
			{
				Agent agent = this.SpawnLahar();
				Agent agent2 = this.SpawnGunnar();
				Campaign.Current.ConversationManager.AddConversationAgents(new List<Agent> { agent, agent2 }, true);
				return true;
			}
			return false;
		}

		// Token: 0x060003DD RID: 989 RVA: 0x0001BC6C File Offset: 0x00019E6C
		private Agent SpawnLahar()
		{
			AgentBuildData agentBuildData = new AgentBuildData(NavalStorylineData.Lahar.CharacterObject);
			agentBuildData.TroopOrigin(new SimpleAgentOrigin(agentBuildData.AgentCharacter, -1, null, default(UniqueTroopDescriptor)));
			Vec3 globalPosition = Mission.Current.Scene.FindEntityWithName("free_infantry_spawn_point_0").GlobalPosition;
			agentBuildData.InitialPosition(ref globalPosition);
			AgentBuildData agentBuildData2 = agentBuildData;
			Vec2 vec = Agent.Main.LookDirection.AsVec2;
			vec = vec.Normalized();
			agentBuildData2.InitialDirection(ref vec);
			agentBuildData.NoHorses(true);
			return Mission.Current.SpawnAgent(agentBuildData, false);
		}

		// Token: 0x060003DE RID: 990 RVA: 0x0001BD04 File Offset: 0x00019F04
		private void StartBattle()
		{
			this._battleWon = false;
			this._battleStarted = true;
			foreach (TroopRosterElement troopRosterElement in from troop in PartyBase.MainParty.MemberRoster.GetTroopRoster()
				where troop.Character.IsHero && troop.Character.HeroObject.IsWounded
				select troop)
			{
				troopRosterElement.Character.HeroObject.Heal(troopRosterElement.Character.HeroObject.WoundedHealthLimit - troopRosterElement.Character.HeroObject.HitPoints + 1, false);
			}
			PlayerEncounter.Finish(true);
			PlayerEncounter.Start();
			PlayerEncounter.Current.SetupFields(this._bossCorsairParty.Party, PartyBase.MainParty);
			PlayerEncounter.StartBattle();
			MissionInitializerRecord navalMissionInitializerTemplate = NavalStorylineData.GetNavalMissionInitializerTemplate("naval_storyline_act_3_quest_2");
			navalMissionInitializerTemplate.NeedsRandomTerrain = false;
			navalMissionInitializerTemplate.PlayingInCampaignMode = false;
			navalMissionInitializerTemplate.SceneHasMapPatch = false;
			navalMissionInitializerTemplate.AtmosphereOnCampaign.NauticalInfo.UsesNavalSimulatedWater = 1;
			NavalMissions.OpenNavalStorylineWoundedBeastBattleMission(navalMissionInitializerTemplate);
			GameMenu.ActivateGameMenu("naval_storyline_act_3_quest_2_retry_menu");
		}

		// Token: 0x060003DF RID: 991 RVA: 0x0001BE2C File Offset: 0x0001A02C
		private void SpawnMainCorsairParty()
		{
			NavalStorylineData.EmiraAlFahda.ChangeState(1);
			this._bossCorsairParty = CustomPartyComponent.CreateCustomPartyWithPartyTemplate(this._corsairSpawnPosition, 1f, NavalStorylineData.HomeSettlement, new TextObject("{=j7h8QfsE}Fahda's Corsairs", null), Clan.BanditFactions.FirstOrDefault<Clan>((Clan x) => x.StringId == "southern_pirates"), Campaign.Current.ObjectManager.GetObject<PartyTemplateObject>("storyline_act3_quest_2_boss_corsair_template"), NavalStorylineData.EmiraAlFahda, NavalStorylineData.EmiraAlFahda, "", "", 1f, false);
			this.AddShipUpgradesForMainCorsairParty();
			this.SetupCorsairParty(this._bossCorsairParty);
			this._bossCorsairParty.IsInfoHidden = true;
		}

		// Token: 0x060003E0 RID: 992 RVA: 0x0001BEE0 File Offset: 0x0001A0E0
		private void AddShipUpgradesForMainCorsairParty()
		{
			bool flag = false;
			foreach (Ship ship in this._bossCorsairParty.Ships)
			{
				if (ship.ShipHull.StringId == "ship_meditheavy_storyline")
				{
					ship.ChangeFigurehead(DefaultFigureheads.Viper);
					this.AddShipUpgradePieces(ship, HuntDownTheEmiraAlFahdaAndTheCorsairsQuest.FahdaShipUpgradePieces);
				}
				else if (ship.ShipHull.StringId == "ship_liburna_storyline")
				{
					ship.ChangeFigurehead(DefaultFigureheads.Hawk);
					this.AddShipUpgradePieces(ship, HuntDownTheEmiraAlFahdaAndTheCorsairsQuest.MediumReinforcementShipUpgradePieces);
				}
				else if (ship.ShipHull.StringId == "ship_meditlight_storyline")
				{
					if (flag)
					{
						this.AddShipUpgradePieces(ship, HuntDownTheEmiraAlFahdaAndTheCorsairsQuest.SecondLightReinforcementShipUpgradePieces);
					}
					else
					{
						this.AddShipUpgradePieces(ship, HuntDownTheEmiraAlFahdaAndTheCorsairsQuest.FirstLightReinforcementShipUpgradePieces);
						flag = true;
					}
				}
			}
		}

		// Token: 0x060003E1 RID: 993 RVA: 0x0001BFD0 File Offset: 0x0001A1D0
		private void AddShipUpgradesForMainParty()
		{
			foreach (Ship ship in MobileParty.MainParty.Ships)
			{
				if (ship.ShipHull.StringId == "ship_liburna_q2_storyline")
				{
					ship.ChangeFigurehead(DefaultFigureheads.Hawk);
					this.AddShipUpgradePieces(ship, HuntDownTheEmiraAlFahdaAndTheCorsairsQuest.LaharShipUpgradePieces);
				}
				else if (ship.ShipHull.StringId == "northern_medium_ship")
				{
					ship.ChangeFigurehead(DefaultFigureheads.Dragon);
					this.AddShipUpgradePieces(ship, HuntDownTheEmiraAlFahdaAndTheCorsairsQuest.GunnarShipUpgradePieces);
				}
			}
		}

		// Token: 0x060003E2 RID: 994 RVA: 0x0001C080 File Offset: 0x0001A280
		private void SetupCorsairParty(MobileParty corsairParty)
		{
			corsairParty.SetPartyUsedByQuest(true);
			base.AddTrackedObject(corsairParty);
			corsairParty.IsCurrentlyAtSea = true;
			corsairParty.IsVisible = true;
			corsairParty.Party.SetCustomBanner(NavalStorylineData.CorsairBanner);
			foreach (Ship ship in corsairParty.Ships)
			{
				ship.IsInvulnerable = true;
			}
			corsairParty.Ai.SetDoNotMakeNewDecisions(true);
			corsairParty.Ai.DisableForHours(3);
			corsairParty.IgnoreByOtherPartiesTill(CampaignTime.Never);
			corsairParty.Party.SetVisualAsDirty();
		}

		// Token: 0x060003E3 RID: 995 RVA: 0x0001C12C File Offset: 0x0001A32C
		private void DestroyCorsairParties()
		{
			foreach (MobileParty mobileParty in this._corsairParties.ToList<MobileParty>())
			{
				if (mobileParty != null && mobileParty.IsActive)
				{
					DestroyPartyAction.Apply(null, mobileParty);
				}
			}
			if (this._bossCorsairParty != null && this._bossCorsairParty.IsActive)
			{
				DestroyPartyAction.Apply(null, this._bossCorsairParty);
			}
		}

		// Token: 0x060003E4 RID: 996 RVA: 0x0001C1B0 File Offset: 0x0001A3B0
		private void SpawnStormEntity()
		{
			if (this._stormEntity == null)
			{
				MatrixFrame identity = MatrixFrame.Identity;
				Scene scene = ((MapScene)Campaign.Current.MapSceneWrapper).Scene;
				List<GameEntity> list = new List<GameEntity>();
				identity.origin = new Vec3(this._corsairSpawnPosition.X, this._corsairSpawnPosition.Y, 0f, -1f);
				this._stormEntity = GameEntity.Instantiate(scene, "psys_mapicon_darkclouds", identity, true);
				scene.GetAllEntitiesWithScriptComponent<CampaignMapAmbientOccluder>(ref list);
				for (int i = 0; i < list.Count; i++)
				{
					list[i].GetFirstScriptOfType<CampaignMapAmbientOccluder>().RegisterQuestStorm(this._stormEntity);
				}
			}
		}

		// Token: 0x060003E5 RID: 997 RVA: 0x0001C260 File Offset: 0x0001A460
		private void RefreshParty(MobileParty mobileParty, PartyTemplateObject pt)
		{
			MBList<TroopRosterElement> troopRoster = mobileParty.MemberRoster.GetTroopRoster();
			for (int i = 0; i < troopRoster.Count; i++)
			{
				if (troopRoster[i].Character.IsHero)
				{
					troopRoster[i].Character.HeroObject.Heal(troopRoster[i].Character.HeroObject.MaxHitPoints, false);
				}
				else
				{
					mobileParty.MemberRoster.RemoveTroop(troopRoster[i].Character, troopRoster[i].Number, default(UniqueTroopDescriptor), 0);
				}
			}
			TroopRoster troopRoster2 = Campaign.Current.Models.PartySizeLimitModel.FindAppropriateInitialRosterForMobileParty(mobileParty, pt);
			mobileParty.MemberRoster.Add(troopRoster2);
			this.RefreshShips(mobileParty, pt);
		}

		// Token: 0x060003E6 RID: 998 RVA: 0x0001C328 File Offset: 0x0001A528
		private void RefreshShips(MobileParty mobileParty, PartyTemplateObject pt)
		{
			foreach (Ship ship4 in mobileParty.Ships)
			{
				ship4.HitPoints = ship4.MaxHitPoints;
			}
			List<Ship> list = Campaign.Current.Models.PartySizeLimitModel.FindAppropriateInitialShipsForMobileParty(mobileParty, pt);
			if (mobileParty.Ships.Count != list.Count)
			{
				using (List<Ship>.Enumerator enumerator = mobileParty.Ships.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Ship ship = enumerator.Current;
						Ship ship2 = list.FirstOrDefault<Ship>((Ship x) => x.ShipHull == ship.ShipHull);
						if (ship2 != null)
						{
							list.Remove(ship2);
						}
					}
				}
				if (list.Count > 0)
				{
					foreach (Ship ship3 in list)
					{
						ChangeShipOwnerAction.ApplyByMobilePartyCreation(mobileParty.Party, ship3);
						if (mobileParty != MobileParty.MainParty)
						{
							ship3.IsInvulnerable = true;
						}
					}
				}
			}
		}

		// Token: 0x060003E7 RID: 999 RVA: 0x0001C46C File Offset: 0x0001A66C
		private void AddShipUpgradePieces(Ship ship, Dictionary<string, string> upgradePieces)
		{
			using (Dictionary<string, string>.Enumerator enumerator = upgradePieces.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, string> kv = enumerator.Current;
					ShipUpgradePiece @object = MBObjectManager.Instance.GetObject<ShipUpgradePiece>(kv.Value);
					if (ship.ShipHull.AvailableSlots.Any<KeyValuePair<string, ShipSlot>>((KeyValuePair<string, ShipSlot> slot) => slot.Key == kv.Key))
					{
						ship.EquipUpgradePiece(kv.Key, @object);
					}
				}
			}
		}

		// Token: 0x060003E8 RID: 1000 RVA: 0x0001C504 File Offset: 0x0001A704
		public bool IsFahdaVisible()
		{
			return this._bossCorsairParty != null && this._bossCorsairParty.IsActive && this._bossCorsairParty.IsVisible;
		}

		// Token: 0x04000232 RID: 562
		private const int NumberOfCorsairParties = 2;

		// Token: 0x04000233 RID: 563
		private const int GoldReward = 1000;

		// Token: 0x04000234 RID: 564
		private const int RelationshipReward = 10;

		// Token: 0x04000235 RID: 565
		private const int CorsairShipAiDisableTime = 3;

		// Token: 0x04000236 RID: 566
		private const string QuestSetPieceEncounterMenuId = "naval_storyline_act_3_quest_2_encounter_menu";

		// Token: 0x04000237 RID: 567
		private const string QuestSetPieceRetryMenuId = "naval_storyline_act_3_quest_2_retry_menu";

		// Token: 0x04000238 RID: 568
		private const string Act3Quest2CorsairPartyTemplateStringIdBase = "storyline_act3_quest_2_corsair_generic_template_";

		// Token: 0x04000239 RID: 569
		private const string Act3Quest2BossCorsairPartyTemplateStringId = "storyline_act3_quest_2_boss_corsair_template";

		// Token: 0x0400023A RID: 570
		private const string FahdaShipHullId = "ship_meditheavy_storyline";

		// Token: 0x0400023B RID: 571
		private const string MediumReinforcementShipHullId = "ship_liburna_storyline";

		// Token: 0x0400023C RID: 572
		private const string LightReinforcementShipHullId = "ship_meditlight_storyline";

		// Token: 0x0400023D RID: 573
		private static readonly Dictionary<string, string> FahdaShipUpgradePieces = new Dictionary<string, string> { { "side", "side_southern_shields_lvl2" } };

		// Token: 0x0400023E RID: 574
		private static readonly Dictionary<string, string> MediumReinforcementShipUpgradePieces = new Dictionary<string, string>
		{
			{ "side", "side_southern_shields_lvl2" },
			{ "sail", "sails_lvl2" }
		};

		// Token: 0x0400023F RID: 575
		private static readonly Dictionary<string, string> FirstLightReinforcementShipUpgradePieces = new Dictionary<string, string>
		{
			{ "side", "side_southern_shields_lvl2" },
			{ "sail", "sails_lvl2" }
		};

		// Token: 0x04000240 RID: 576
		private static readonly Dictionary<string, string> SecondLightReinforcementShipUpgradePieces = new Dictionary<string, string>
		{
			{ "side", "side_southern_shields_lvl2" },
			{ "sail", "sails_lvl2" }
		};

		// Token: 0x04000241 RID: 577
		private const string LaharShipHullId = "ship_liburna_q2_storyline";

		// Token: 0x04000242 RID: 578
		private static readonly Dictionary<string, string> LaharShipUpgradePieces = new Dictionary<string, string>
		{
			{ "side", "side_southern_shields_lvl3" },
			{ "sail", "sails_lvl2" },
			{ "bow", "bow_northern_reinforced_ram_lvl3" }
		};

		// Token: 0x04000243 RID: 579
		private const string GunnarShipHullId = "northern_medium_ship";

		// Token: 0x04000244 RID: 580
		private static readonly Dictionary<string, string> GunnarShipUpgradePieces = new Dictionary<string, string>
		{
			{ "side", "side_southern_shields_lvl2" },
			{ "sail", "sails_lvl2" }
		};

		// Token: 0x04000245 RID: 581
		private GameEntity _stormEntity;

		// Token: 0x04000246 RID: 582
		[SaveableField(1)]
		private List<MobileParty> _corsairParties;

		// Token: 0x04000247 RID: 583
		[SaveableField(2)]
		private JournalLog _playerStartsQuestLog;

		// Token: 0x04000248 RID: 584
		[SaveableField(3)]
		private CampaignVec2 _corsairSpawnPosition;

		// Token: 0x04000249 RID: 585
		[SaveableField(4)]
		private int _numberOfDefeatedCorsairParties;

		// Token: 0x0400024A RID: 586
		[SaveableField(5)]
		private MobileParty _bossCorsairParty;

		// Token: 0x0400024B RID: 587
		[SaveableField(6)]
		private bool _battleWon;

		// Token: 0x0400024C RID: 588
		[SaveableField(7)]
		private bool _willProgressStoryline;

		// Token: 0x0400024D RID: 589
		[SaveableField(8)]
		private bool _battleStarted;

		// Token: 0x0400024E RID: 590
		[SaveableField(9)]
		private readonly MapMarker _corsairHuntingGroundMarker;
	}
}
