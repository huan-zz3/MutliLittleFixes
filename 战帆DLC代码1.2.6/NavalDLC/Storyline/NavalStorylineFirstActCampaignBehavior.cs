using System;
using NavalDLC.Missions;
using NavalDLC.Storyline.Quests;
using StoryMode;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace NavalDLC.Storyline
{
	// Token: 0x0200002A RID: 42
	public class NavalStorylineFirstActCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x060001B7 RID: 439 RVA: 0x0000A8CC File Offset: 0x00008ACC
		public override void RegisterEvents()
		{
			if (!NavalStorylineData.IsNavalStorylineCanceled())
			{
				CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnNewGameCreated));
				CampaignEvents.GameMenuOpened.AddNonSerializedListener(this, new Action<MenuCallbackArgs>(this.OnGameMenuOpened));
				CampaignEvents.OnAfterSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnAfterSessionLaunched));
				CampaignEvents.OnMissionEndedEvent.AddNonSerializedListener(this, new Action<IMission>(this.OnMissionEnded));
				NavalDLCEvents.OnNavalStorylineCanceledEvent.AddNonSerializedListener(this, new Action<NavalStorylineData.StorylineCancelDetail>(this.OnNavalStorylineCanceled));
			}
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x0000A953 File Offset: 0x00008B53
		private void OnNavalStorylineCanceled(NavalStorylineData.StorylineCancelDetail detail)
		{
			CampaignEventDispatcher.Instance.RemoveListeners(this);
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x0000A960 File Offset: 0x00008B60
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<NavalStorylineFirstActCampaignBehavior.PortFightState>("_portFightState", ref this._portFightState);
		}

		// Token: 0x060001BA RID: 442 RVA: 0x0000A974 File Offset: 0x00008B74
		private void OnNewGameCreated(CampaignGameStarter campaignGameStarter)
		{
			if (StoryModeManager.Current == null)
			{
				this._portFightState = NavalStorylineFirstActCampaignBehavior.PortFightState.ReadyToBeFinalized;
			}
		}

		// Token: 0x060001BB RID: 443 RVA: 0x0000A984 File Offset: 0x00008B84
		private void OnGameMenuOpened(MenuCallbackArgs args)
		{
			if (this._portFightState != NavalStorylineFirstActCampaignBehavior.PortFightState.ReadyToBeFinalized && args.MenuContext.GameMenu.StringId == "port_menu" && Settlement.CurrentSettlement == NavalStorylineData.HomeSettlement && Campaign.Current.QuestManager.IsThereActiveQuestWithType(typeof(InquireAtOstican)))
			{
				if (this._portFightState == NavalStorylineFirstActCampaignBehavior.PortFightState.FightMissionWon)
				{
					GameMenu.ActivateGameMenu("naval_storyline_after_port_fight");
					return;
				}
				GameMenu.ActivateGameMenu("naval_storyline_port_fight");
			}
		}

		// Token: 0x060001BC RID: 444 RVA: 0x0000A9FB File Offset: 0x00008BFB
		private void OnAfterSessionLaunched(CampaignGameStarter campaignGameStarter)
		{
			this.AddGameMenus(campaignGameStarter);
			this.AddPortFightOnSuccessDialogFlow(campaignGameStarter);
		}

		// Token: 0x060001BD RID: 445 RVA: 0x0000AA0B File Offset: 0x00008C0B
		private void OnMissionEnded(IMission mission)
		{
			if (this._portFightState == NavalStorylineFirstActCampaignBehavior.PortFightState.FightMissionStarted)
			{
				MissionResult missionResult = (mission as Mission).MissionResult;
				if (missionResult != null && missionResult.PlayerVictory)
				{
					this._portFightState = NavalStorylineFirstActCampaignBehavior.PortFightState.FightMissionWon;
					return;
				}
				this._portFightState = NavalStorylineFirstActCampaignBehavior.PortFightState.FightShouldContinue;
			}
		}

		// Token: 0x060001BE RID: 446 RVA: 0x0000AA40 File Offset: 0x00008C40
		private void AddGameMenus(CampaignGameStarter campaignGameStarter)
		{
			campaignGameStarter.AddGameMenu("naval_storyline_port_fight", "{=GhTjvwpl}You're strolling through {SETTLEMENT}{.o} streets when you hear raised voices coming from a side alley. You turn to look, and see three rough-looking men accosting an older man in a cloak. His gaze shifts quickly from one to the other and his body is tensed, as though he is going to spring into action. You sense a fight is about to start.", new OnInitDelegate(this.port_fight_on_init), 0, 0, null);
			campaignGameStarter.AddGameMenuOption("naval_storyline_port_fight", "continue", "{=DM6luo3c}Continue", new GameMenuOption.OnConditionDelegate(this.port_fight_condition), new GameMenuOption.OnConsequenceDelegate(this.port_fight_consequence), false, -1, false, null);
			campaignGameStarter.AddGameMenu("naval_storyline_after_port_fight", "{=!}{AFTER_PORT_FIGHT_MENU_TEXT}", new OnInitDelegate(this.after_port_fight_on_init), 0, 0, null);
			campaignGameStarter.AddGameMenuOption("naval_storyline_after_port_fight", "continue_to_dialog", "{=DM6luo3c}Continue", new GameMenuOption.OnConditionDelegate(this.naval_storyline_after_port_fight_continue_to_dialog_on_condition), new GameMenuOption.OnConsequenceDelegate(this.naval_storyline_after_port_fight_continue_to_dialog_on_consequence), false, -1, false, null);
			campaignGameStarter.AddGameMenuOption("naval_storyline_after_port_fight", "return_to_fight", "{=inC6Ia5s}Return to the fight", new GameMenuOption.OnConditionDelegate(this.naval_storyline_after_port_fight_return_to_fight_on_condition), new GameMenuOption.OnConsequenceDelegate(this.naval_storyline_after_port_fight_return_to_fight_on_consequence), false, -1, false, null);
			campaignGameStarter.AddGameMenuOption("naval_storyline_after_port_fight", "escape", "{=qqjRkMy9}Make good your escape", new GameMenuOption.OnConditionDelegate(this.naval_storyline_after_port_fight_escape_on_condition), new GameMenuOption.OnConsequenceDelegate(this.naval_storyline_after_port_fight_escape_on_consequence), true, -1, false, null);
		}

		// Token: 0x060001BF RID: 447 RVA: 0x0000AB50 File Offset: 0x00008D50
		[GameMenuInitializationHandler("naval_storyline_port_fight")]
		[GameMenuInitializationHandler("naval_storyline_after_port_fight")]
		public static void port_menu_on_init(MenuCallbackArgs args)
		{
			string text = Settlement.CurrentSettlement.Culture.StringId + "_port";
			args.MenuContext.SetBackgroundMeshName(text);
			args.MenuContext.SetAmbientSound("event:/map/ambient/node/settlements/2d/port");
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x0000AB93 File Offset: 0x00008D93
		private void port_fight_on_init(MenuCallbackArgs args)
		{
			NavalStorylineData.OnCheckpointReached(NavalStorylineData.NavalStorylineCheckpoint.Act1PortMenu);
			MBTextManager.SetTextVariable("SETTLEMENT", NavalStorylineData.HomeSettlement.EncyclopediaLinkWithName, false);
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x0000ABB0 File Offset: 0x00008DB0
		private bool port_fight_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 1;
			return true;
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x0000ABBC File Offset: 0x00008DBC
		private void port_fight_consequence(MenuCallbackArgs args)
		{
			TroopRoster troopRoster = TroopRoster.CreateDummyTroopRoster();
			troopRoster.AddToCounts(CharacterObject.PlayerCharacter, 1, true, 0, 0, true, -1);
			troopRoster.AddToCounts(NavalStorylineData.Gunnar.CharacterObject, 1, false, 0, 0, true, -1);
			TroopRoster troopRoster2 = TroopRoster.CreateDummyTroopRoster();
			CharacterObject @object = MBObjectManager.Instance.GetObject<CharacterObject>("gangster_3");
			troopRoster2.AddToCounts(@object, 3, false, 0, 0, true, -1);
			int wallLevel = Settlement.CurrentSettlement.Town.GetWallLevel();
			Settlement.CurrentSettlement.LocationComplex.GetScene("center", wallLevel);
			LocationComplex.Current.GetLocationWithId("center");
			GameMenu.ActivateGameMenu("naval_storyline_after_port_fight");
			this._portFightState = NavalStorylineFirstActCampaignBehavior.PortFightState.FightMissionStarted;
			MissionInitializerRecord navalMissionInitializerTemplate = NavalStorylineData.GetNavalMissionInitializerTemplate("storyline_shipyard_alley");
			TerrainType faceTerrainType = Campaign.Current.MapSceneWrapper.GetFaceTerrainType(MobileParty.MainParty.CurrentNavigationFace);
			navalMissionInitializerTemplate.TerrainType = faceTerrainType;
			navalMissionInitializerTemplate.NeedsRandomTerrain = false;
			navalMissionInitializerTemplate.PlayingInCampaignMode = true;
			navalMissionInitializerTemplate.RandomTerrainSeed = MBRandom.RandomInt(10000);
			navalMissionInitializerTemplate.AtmosphereOnCampaign = Campaign.Current.Models.MapWeatherModel.GetAtmosphereModel(MobileParty.MainParty.Position);
			navalMissionInitializerTemplate.SceneHasMapPatch = false;
			navalMissionInitializerTemplate.AtmosphereOnCampaign.NauticalInfo.UsesNavalSimulatedWater = 1;
			NavalMissions.OpenNavalStorylineAlleyFightMission(navalMissionInitializerTemplate);
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x0000ACF4 File Offset: 0x00008EF4
		private void after_port_fight_on_init(MenuCallbackArgs args)
		{
			if (this._portFightState != NavalStorylineFirstActCampaignBehavior.PortFightState.None)
			{
				if (this._portFightState == NavalStorylineFirstActCampaignBehavior.PortFightState.FightMissionWon)
				{
					if (NavalStorylineData.Gunnar.IsWounded)
					{
						MBTextManager.SetTextVariable("AFTER_PORT_FIGHT_MENU_TEXT", new TextObject("{=3V80vvSz}You make quick work of the alley thugs, and help their victim to his feet. He seems dazed, but grateful.", null), false);
						return;
					}
					if (Hero.MainHero.IsWounded)
					{
						MBTextManager.SetTextVariable("AFTER_PORT_FIGHT_MENU_TEXT", new TextObject("{=5NoZgdqr}The alley thugs are too many for you, and knock you to the ground. Before they can finish you off, however, you hear a rush of feet and cries of alarm. The town watch must have heard the commotion, and your assailants make a quick retreat. The watch helps you to your feet and tells you to be more careful. The thugs' victim, dazed but apparently unhurt, introduces himself.", null), false);
						return;
					}
					this.OnFightMissionFinalized();
					return;
				}
				else
				{
					if (this._portFightState == NavalStorylineFirstActCampaignBehavior.PortFightState.FightShouldContinue)
					{
						MBTextManager.SetTextVariable("AFTER_PORT_FIGHT_MENU_TEXT", new TextObject("{=7C4JYwZp}You back out of the alley. You could easily escape, but you sense that the thugs will kill the old man.", null), false);
						return;
					}
					this.OnFightMissionFinalized();
				}
			}
		}

		// Token: 0x060001C4 RID: 452 RVA: 0x0000AD85 File Offset: 0x00008F85
		private void OnFightMissionFinalized()
		{
			this._portFightState = NavalStorylineFirstActCampaignBehavior.PortFightState.ReadyToBeFinalized;
			NavalStorylineData.OnCheckpointReached(NavalStorylineData.NavalStorylineCheckpoint.Act1PortFightSucceeded);
			GameMenu.SwitchToMenu("town");
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x0000AD9E File Offset: 0x00008F9E
		private void OpenConversationWithGunnar()
		{
			this.SpawnPortQuestGiver();
			PlayerEncounter.LocationEncounter.CreateAndOpenMissionController(LocationComplex.Current.GetLocationOfCharacter(NavalStorylineData.Gunnar), null, NavalStorylineData.Gunnar.CharacterObject, null);
		}

		// Token: 0x060001C6 RID: 454 RVA: 0x0000ADCC File Offset: 0x00008FCC
		private bool naval_storyline_after_port_fight_continue_to_dialog_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 12;
			return this._portFightState == NavalStorylineFirstActCampaignBehavior.PortFightState.FightMissionWon;
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x0000ADDF File Offset: 0x00008FDF
		private void naval_storyline_after_port_fight_continue_to_dialog_on_consequence(MenuCallbackArgs args)
		{
			this.OpenConversationWithGunnar();
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x0000ADE7 File Offset: 0x00008FE7
		private bool naval_storyline_after_port_fight_return_to_fight_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 12;
			return this._portFightState == NavalStorylineFirstActCampaignBehavior.PortFightState.FightShouldContinue;
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x0000ADFA File Offset: 0x00008FFA
		private void naval_storyline_after_port_fight_return_to_fight_on_consequence(MenuCallbackArgs args)
		{
			this.port_fight_consequence(args);
		}

		// Token: 0x060001CA RID: 458 RVA: 0x0000AE03 File Offset: 0x00009003
		private bool naval_storyline_after_port_fight_escape_on_condition(MenuCallbackArgs args)
		{
			args.Tooltip = new TextObject("{=SpZEO1Rx}This option will abandon the storyline.", null);
			args.optionLeaveType = 16;
			return this._portFightState == NavalStorylineFirstActCampaignBehavior.PortFightState.FightShouldContinue;
		}

		// Token: 0x060001CB RID: 459 RVA: 0x0000AE27 File Offset: 0x00009027
		private void naval_storyline_after_port_fight_escape_on_consequence(MenuCallbackArgs args)
		{
			this._portFightState = NavalStorylineFirstActCampaignBehavior.PortFightState.ReadyToBeFinalized;
			NavalDLCEvents.Instance.OnNavalStorylineCanceled(NavalStorylineData.StorylineCancelDetail.ByDialogue);
			GameMenu.SwitchToMenu("town_outside");
		}

		// Token: 0x060001CC RID: 460 RVA: 0x0000AE48 File Offset: 0x00009048
		private void AddPortFightOnSuccessDialogFlow(CampaignGameStarter campaignGameStarter)
		{
			campaignGameStarter.AddDialogLine("initial_port_fight_success_dialog_start", "start", "gunnar_introduction_1", "{=!}{START_LINE}", new ConversationSentence.OnConditionDelegate(this.initial_port_fight_success_dialog_start_on_condition), null, 50000, null);
			campaignGameStarter.AddDialogLine("gunnar_introduction_1_line", "gunnar_introduction_1", "gunnar_introduction_2", "{=rbpBs3bZ}I am Gunnar of Lagshofn, from the Nordvyg lands.", null, null, 100, null);
			campaignGameStarter.AddDialogLine("gunnar_introduction_2_line", "gunnar_introduction_2", "gunnar_introduction_3", "{=8kUr3LUi}I've come to this port seeking warriors and a ship. These men we fought were allies of a pirate gang who call themselves the Sea Hounds. They have been raiding and slaving along the Nordvyg’s shores, and I intend to go to war with them.", null, null, 100, null);
			campaignGameStarter.AddDialogLine("gunnar_introduction_3_line", "gunnar_introduction_3", "initial_port_fight_success_dialog_player_options", "{=enXch5l7}The Sea Hounds and I have history, and nowadays they hate my guts as fiercely as I hate theirs. Somebody must have sent word of my whereabouts to their local friends as these lowlifes had a mind to do me in. Again, you have my thanks for evening the odds.", null, null, 100, null);
			campaignGameStarter.AddPlayerLine("initial_port_fight_success_dialog_player_options_1", "initial_port_fight_success_dialog_player_options", "initial_port_fight_success_dialog_player_options_1_answer", "{=Z39CjlP7}Did you say slave raids? My brother and sister were taken in one.", new ConversationSentence.OnConditionDelegate(this.initial_port_fight_success_dialog_player_options_1_condition), new ConversationSentence.OnConsequenceDelegate(this.initial_port_fight_success_dialog_player_options_1_on_consequence), 100, new ConversationSentence.OnClickableConditionDelegate(this.initial_port_fight_success_dialog_player_options_1_clickable_condition), null);
			campaignGameStarter.AddPlayerLine("initial_port_fight_success_dialog_player_options_2", "initial_port_fight_success_dialog_player_options", "initial_port_fight_success_dialog_player_options_2_answer", "{=tIxXxFQU}Who are these Sea Hounds?", new ConversationSentence.OnConditionDelegate(this.initial_port_fight_success_dialog_player_options_2_condition), new ConversationSentence.OnConsequenceDelegate(this.initial_port_fight_success_dialog_player_options_2_on_consequence), 100, new ConversationSentence.OnClickableConditionDelegate(this.initial_port_fight_success_dialog_player_options_2_clickable_condition), null);
			campaignGameStarter.AddPlayerLine("initial_port_fight_success_dialog_player_options_3", "initial_port_fight_success_dialog_player_options", "initial_port_fight_success_dialog_player_options_3_answer", "{=XP7g0Kiq}Why do you risk so much to hunt them?", new ConversationSentence.OnConditionDelegate(this.initial_port_fight_success_dialog_player_options_3_condition), new ConversationSentence.OnConsequenceDelegate(this.initial_port_fight_success_dialog_player_options_3_on_consequence), 100, new ConversationSentence.OnClickableConditionDelegate(this.initial_port_fight_success_dialog_player_options_3_clickable_condition), null);
			campaignGameStarter.AddPlayerLine("initial_port_fight_success_dialog_player_options_4", "initial_port_fight_success_dialog_player_options", "initial_port_fight_success_dialog_player_options_4_answer", "{=ac5oq0pt}What are you doing now?", new ConversationSentence.OnConditionDelegate(this.initial_port_fight_success_dialog_continue_condition), null, 100, null, null);
			campaignGameStarter.AddDialogLine("initial_port_fight_success_dialog_player_options_1_answer_line", "initial_port_fight_success_dialog_player_options_1_answer", "initial_port_fight_success_dialog_player_options", "{=zTr3dBd7}I know what it's like to lose family to slavers. If you're still searching, look to the Sea Hounds. They've got their hands in most of the slaving that happens along these coasts.", null, null, 100, null);
			campaignGameStarter.AddDialogLine("initial_port_fight_success_dialog_player_options_2_answer_line", "initial_port_fight_success_dialog_player_options_2_answer", "initial_port_fight_success_dialog_player_options", "{=Vs5cNhfI}It’s hard to believe now, but they were once my brothers-in-arms. Years ago we fought side-by-side in the last great rebellion in the north. Most of the clans and many freemen like myself refused to bow to Volbjorn the usurper, as he was then called. But Volbjorn knew how to speak to men’s desires. He won over the bigger clans with promises of land and silver, and when summer came and he brought a fleet to give us battle, he had with him so many long ships that their sails covered the horizon. We still fought them of course, but their numbers were too many to beat.", null, null, 100, null);
			campaignGameStarter.AddDialogLine("initial_port_fight_success_dialog_player_options_3_answer_line", "initial_port_fight_success_dialog_player_options_3_answer", "initial_port_fight_success_dialog_player_options", "{=lIpAlkH2}They dishonor what we fought for. I'm no stranger to battle - I'll kill when I must. But they murder for pleasure, thinking the All-Father rewards bloodthirst. He wants warriors, not hounds.", null, null, 100, null);
			campaignGameStarter.AddDialogLine("initial_port_fight_success_dialog_player_options_4_answer_line", "initial_port_fight_success_dialog_player_options_4_answer", "next_move_explanation_1", "{=RQ0qIqGH}I mean to gather up with some of my kin and friends to go against the Sea Hounds. Just a few days ago, I ran into an old comrade of mine here in Ostican. He is called Purig and he happens to own a fast ship. He promised to help me capture a Sea Hound ship and put together a crew.", null, null, 100, null);
			campaignGameStarter.AddDialogLine("next_move_explanation_1_line", "next_move_explanation_1", "next_move_explanation_player_options", "{=okfrRTb4}So, I'm going to make you a proposal. Perhaps you'd like to come with us? I can't guarantee we'll find your kin, but I can promise a good fight and, if we win, a bit of fine loot. And, well, if you'd ever had an interest in learning how to handle a ship, you won't find any better school than these northern seas.", null, null, 100, null);
			campaignGameStarter.AddPlayerLine("next_move_explanation_player_option_1_line", "next_move_explanation_player_options", "player_joins_gunnar_answer", "{=9buEaTHt}I will join you, and we can hunt together.", null, null, 100, null, null);
			campaignGameStarter.AddPlayerLine("next_move_explanation_player_option_2_line", "next_move_explanation_player_options", "player_waits_answer", "{=qFFYyNeR}Let me think this over.", null, null, 100, null, null);
			campaignGameStarter.AddPlayerLine("next_move_explanation_player_option_3_line", "next_move_explanation_player_options", "player_skips_tutorial", "{=JAuDUFkG}I have other obligations, and I already know how to handle a ship. (Skip tutorial)", null, null, 100, null, null);
			campaignGameStarter.AddDialogLine("player_joins_gunnar_answer_line", "player_joins_gunnar_answer", "close_window", "{=nu5vuTvX}You can find Purig in the tavern and introduce yourself. I should go get myself cleaned up and get ready to travel.", null, delegate
			{
				Campaign.Current.ConversationManager.ConversationEndOneShot += this.OnQuestGiverSaved;
			}, 100, null);
			campaignGameStarter.AddDialogLine("player_waits_answer_line", "player_waits_answer", "close_window", "{=nyQhfz0B}The decision is of course yours. I expect you can find Purig in the tavern for the next few days, if you change your mind.", null, delegate
			{
				Campaign.Current.ConversationManager.ConversationEndOneShot += this.OnQuestGiverSaved;
			}, 100, null);
			campaignGameStarter.AddDialogLine("player_skips_tutorial_line", "player_skips_tutorial", "skip_naval_tutorial_confirmation", "{=2biaAIpM}Very well. I hope you find your kin some day. Listen, whatever I manage to do near Hvalvik, I will return here and try to find other warriors to help me. If you ever reconsider, look for me here in Ostican.", null, null, 100, null);
			campaignGameStarter.AddPlayerLine("skip_tutorial_confirmation_option_1_line", "skip_naval_tutorial_confirmation", "player_joins_gunnar_answer", "{=58CsRmug}Wait, I changed my mind.", null, null, 100, null, null);
			campaignGameStarter.AddPlayerLine("skip_tutorial_confirmation_option_2_line", "skip_naval_tutorial_confirmation", "close_window", "{=1zleX968}Farewell to you too, and good luck.", null, delegate
			{
				Campaign.Current.ConversationManager.ConversationEndOneShot += this.OnNavalTutorialSkipped;
			}, 100, null, null);
		}

		// Token: 0x060001CD RID: 461 RVA: 0x0000B19C File Offset: 0x0000939C
		private bool initial_port_fight_success_dialog_start_on_condition()
		{
			bool flag = Hero.OneToOneConversationHero == NavalStorylineData.Gunnar && !NavalStorylineData.Gunnar.HasMet;
			if (flag)
			{
				TextObject textObject;
				if (!Hero.MainHero.IsWounded)
				{
					textObject = new TextObject("{=CvcV0DWt}By my blood… Damn, that hurts. I think I'm all right, though. Thank you.", null);
				}
				else
				{
					textObject = new TextObject("{=h46iGLj0}Are you all right? One on three aren't the worst odds I've faced, but even so, that could have gone either way. I owe you my thanks.", null);
				}
				TextObjectExtensions.SetCharacterProperties(textObject, "QUEST_GIVER", NavalStorylineData.Gunnar.CharacterObject, false);
				TextObjectExtensions.SetCharacterProperties(textObject, "PLAYER", Hero.MainHero.CharacterObject, false);
				MBTextManager.SetTextVariable("START_LINE", textObject, false);
			}
			return flag;
		}

		// Token: 0x060001CE RID: 462 RVA: 0x0000B226 File Offset: 0x00009426
		private bool initial_port_fight_success_dialog_player_options_1_condition()
		{
			return true;
		}

		// Token: 0x060001CF RID: 463 RVA: 0x0000B229 File Offset: 0x00009429
		private bool initial_port_fight_success_dialog_player_options_1_clickable_condition(out TextObject explanation)
		{
			explanation = TextObject.GetEmpty();
			return !this._initialPortFightSuccessDialogPlayerOption1Selected;
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x0000B23B File Offset: 0x0000943B
		private void initial_port_fight_success_dialog_player_options_1_on_consequence()
		{
			this._initialPortFightSuccessDialogPlayerOption1Selected = true;
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x0000B244 File Offset: 0x00009444
		private bool initial_port_fight_success_dialog_player_options_2_condition()
		{
			return true;
		}

		// Token: 0x060001D2 RID: 466 RVA: 0x0000B247 File Offset: 0x00009447
		private bool initial_port_fight_success_dialog_player_options_2_clickable_condition(out TextObject explanation)
		{
			explanation = TextObject.GetEmpty();
			return !this._initialPortFightSuccessDialogPlayerOption2Selected;
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x0000B259 File Offset: 0x00009459
		private void initial_port_fight_success_dialog_player_options_2_on_consequence()
		{
			this._initialPortFightSuccessDialogPlayerOption2Selected = true;
		}

		// Token: 0x060001D4 RID: 468 RVA: 0x0000B262 File Offset: 0x00009462
		private bool initial_port_fight_success_dialog_player_options_3_condition()
		{
			return this._initialPortFightSuccessDialogPlayerOption2Selected;
		}

		// Token: 0x060001D5 RID: 469 RVA: 0x0000B26A File Offset: 0x0000946A
		private bool initial_port_fight_success_dialog_player_options_3_clickable_condition(out TextObject explanation)
		{
			explanation = TextObject.GetEmpty();
			return !this._initialPortFightSuccessDialogPlayerOption4Selected;
		}

		// Token: 0x060001D6 RID: 470 RVA: 0x0000B27C File Offset: 0x0000947C
		private void initial_port_fight_success_dialog_player_options_3_on_consequence()
		{
			this._initialPortFightSuccessDialogPlayerOption4Selected = true;
		}

		// Token: 0x060001D7 RID: 471 RVA: 0x0000B285 File Offset: 0x00009485
		private bool initial_port_fight_success_dialog_continue_condition()
		{
			return this._initialPortFightSuccessDialogPlayerOption1Selected && this._initialPortFightSuccessDialogPlayerOption2Selected && this._initialPortFightSuccessDialogPlayerOption4Selected;
		}

		// Token: 0x060001D8 RID: 472 RVA: 0x0000B2A0 File Offset: 0x000094A0
		private void SpawnPortQuestGiver()
		{
			Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(NavalStorylineData.Gunnar.CharacterObject.Race, "_settlement");
			LocationCharacter locationCharacter = new LocationCharacter(new AgentData(new SimpleAgentOrigin(NavalStorylineData.Gunnar.CharacterObject, -1, null, default(UniqueTroopDescriptor))).Monster(monsterWithSuffix), new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddWandererBehaviors), "npc_common", true, 0, null, true, false, null, false, false, true, null, false);
			LocationComplex.Current.GetLocationWithId("center").AddCharacter(locationCharacter);
		}

		// Token: 0x060001D9 RID: 473 RVA: 0x0000B32D File Offset: 0x0000952D
		private void OnQuestGiverSaved()
		{
			Mission.Current.GetMissionBehavior<NavalStorylineAlleyFightMissionController>().OnConversationEnded();
			LocationComplex.Current.RemoveCharacterIfExists(NavalStorylineData.Gunnar);
			NavalDLCEvents.Instance.OnGunnarSaved();
			NavalStorylineData.Gunnar.SetHasMet();
			this.OnFightMissionFinalized();
		}

		// Token: 0x060001DA RID: 474 RVA: 0x0000B368 File Offset: 0x00009568
		private void OnNavalTutorialSkipped()
		{
			Mission mission = Mission.Current;
			if (mission != null)
			{
				NavalStorylineAlleyFightMissionController missionBehavior = mission.GetMissionBehavior<NavalStorylineAlleyFightMissionController>();
				if (missionBehavior != null)
				{
					missionBehavior.OnConversationEnded();
				}
			}
			NavalStorylineData.Gunnar.SetHasMet();
			this.OnFightMissionFinalized();
			NavalDLCEvents.Instance.OnNavalStorylineTutorialSkipped();
			Settlement currentSettlement = Settlement.CurrentSettlement;
			if (currentSettlement != null && currentSettlement == NavalStorylineData.HomeSettlement && currentSettlement.HasPort && currentSettlement.LocationComplex.GetLocationOfCharacter(NavalStorylineData.Gunnar) == null)
			{
				Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(NavalStorylineData.Gunnar.CharacterObject.Race, "_settlement");
				LocationCharacter locationCharacter = new LocationCharacter(new AgentData(new SimpleAgentOrigin(NavalStorylineData.Gunnar.CharacterObject, -1, null, default(UniqueTroopDescriptor))).Monster(monsterWithSuffix), new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddWandererBehaviors), "npc_common", true, 0, null, true, false, null, false, false, true, null, false);
				LocationComplex.Current.GetLocationWithId("port").AddCharacter(locationCharacter);
			}
		}

		// Token: 0x040000B8 RID: 184
		private const string PortFightEnemyTroopStringId = "gangster_3";

		// Token: 0x040000B9 RID: 185
		private NavalStorylineFirstActCampaignBehavior.PortFightState _portFightState;

		// Token: 0x040000BA RID: 186
		private bool _initialPortFightSuccessDialogPlayerOption1Selected;

		// Token: 0x040000BB RID: 187
		private bool _initialPortFightSuccessDialogPlayerOption2Selected;

		// Token: 0x040000BC RID: 188
		private bool _initialPortFightSuccessDialogPlayerOption4Selected;

		// Token: 0x0200018B RID: 395
		private enum PortFightState
		{
			// Token: 0x04000C46 RID: 3142
			None,
			// Token: 0x04000C47 RID: 3143
			FightMissionStarted,
			// Token: 0x04000C48 RID: 3144
			FightMissionWon,
			// Token: 0x04000C49 RID: 3145
			FightShouldContinue,
			// Token: 0x04000C4A RID: 3146
			ReadyToBeFinalized
		}

		// Token: 0x0200018C RID: 396
		public class NavalStorylineFirstActCampaignBehaviorTypeDefiner : SaveableTypeDefiner
		{
			// Token: 0x06001913 RID: 6419 RVA: 0x000AD196 File Offset: 0x000AB396
			public NavalStorylineFirstActCampaignBehaviorTypeDefiner()
				: base(370000)
			{
			}

			// Token: 0x06001914 RID: 6420 RVA: 0x000AD1A3 File Offset: 0x000AB3A3
			protected override void DefineEnumTypes()
			{
				base.AddEnumDefinition(typeof(NavalStorylineFirstActCampaignBehavior.PortFightState), 1, null);
			}
		}
	}
}
