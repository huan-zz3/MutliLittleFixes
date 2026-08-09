using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade;
using TaleWorlds.SaveSystem;

namespace NavalDLC.Storyline.Quests
{
	// Token: 0x02000039 RID: 57
	public class InquireAtOstican : QuestBase
	{
		// Token: 0x1700009E RID: 158
		// (get) Token: 0x060003EC RID: 1004 RVA: 0x0001C641 File Offset: 0x0001A841
		public override TextObject Title
		{
			get
			{
				TextObject textObject = new TextObject("{=GOYpy4gI}Inquire at {SETTLEMENT}", null);
				textObject.SetTextVariable("SETTLEMENT", NavalStorylineData.HomeSettlement.Name);
				return textObject;
			}
		}

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x060003ED RID: 1005 RVA: 0x0001C664 File Offset: 0x0001A864
		public override bool IsRemainingTimeHidden
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x060003EE RID: 1006 RVA: 0x0001C667 File Offset: 0x0001A867
		public override string SpecialQuestType
		{
			get
			{
				return "NavalStoryline";
			}
		}

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x060003EF RID: 1007 RVA: 0x0001C66E File Offset: 0x0001A86E
		private TextObject _questStartLog
		{
			get
			{
				TextObject textObject = new TextObject("{=JFNtXUF2}You have heard that bandits might be selling captives to pirates on the Vlandian coast, and the port of {SETTLEMENT} might be a good place to start.", null);
				textObject.SetTextVariable("SETTLEMENT", NavalStorylineData.HomeSettlement.Name);
				return textObject;
			}
		}

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x060003F0 RID: 1008 RVA: 0x0001C694 File Offset: 0x0001A894
		private TextObject _isGunnarSavedLog
		{
			get
			{
				TextObject textObject = new TextObject("{=Rynxrlis}You met {GUNNAR.LINK} after helping him fight off some attackers. He suggested you come on a voyage north with him. Go to the tavern at {SETTLEMENT} and talk to his comrade {NORTHERNER.LINK}.", null);
				TextObjectExtensions.SetCharacterProperties(textObject, "GUNNAR", NavalStorylineData.Gunnar.CharacterObject, false);
				TextObjectExtensions.SetCharacterProperties(textObject, "NORTHERNER", NavalStorylineData.Purig.CharacterObject, false);
				textObject.SetTextVariable("SETTLEMENT", NavalStorylineData.HomeSettlement.EncyclopediaLinkWithName);
				return textObject;
			}
		}

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x060003F1 RID: 1009 RVA: 0x0001C6EE File Offset: 0x0001A8EE
		private TextObject _tutorialSkippedLog
		{
			get
			{
				TextObject textObject = new TextObject("{=3mvfEsqk}You declined to join {GUNNAR.LINK} on his voyage, but may be able to find him later at {SETTLEMENT}.", null);
				TextObjectExtensions.SetCharacterProperties(textObject, "GUNNAR", NavalStorylineData.Gunnar.CharacterObject, false);
				textObject.SetTextVariable("SETTLEMENT", NavalStorylineData.HomeSettlement.EncyclopediaLinkWithName);
				return textObject;
			}
		}

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x060003F2 RID: 1010 RVA: 0x0001C727 File Offset: 0x0001A927
		private TextObject _cancelQuestLog
		{
			get
			{
				TextObject textObject = new TextObject("{=nHc1jonU}You decided to stop searching for your sister.", null);
				TextObjectExtensions.SetCharacterProperties(textObject, "NORTHERNER", NavalStorylineData.Purig.CharacterObject, false);
				textObject.SetTextVariable("SETTLEMENT", NavalStorylineData.HomeSettlement.EncyclopediaLinkWithName);
				return textObject;
			}
		}

		// Token: 0x060003F3 RID: 1011 RVA: 0x0001C760 File Offset: 0x0001A960
		public InquireAtOstican()
			: base("inquire_at_ostican", null, CampaignTime.Never, 0)
		{
		}

		// Token: 0x060003F4 RID: 1012 RVA: 0x0001C774 File Offset: 0x0001A974
		protected override void OnStartQuest()
		{
			base.OnStartQuest();
			base.AddLog(this._questStartLog, false);
			base.AddTrackedObject(NavalStorylineData.HomeSettlement);
		}

		// Token: 0x060003F5 RID: 1013 RVA: 0x0001C795 File Offset: 0x0001A995
		protected override void SetDialogs()
		{
			this.AddNorthernerDialog();
		}

		// Token: 0x060003F6 RID: 1014 RVA: 0x0001C79D File Offset: 0x0001A99D
		protected override void InitializeQuestOnGameLoad()
		{
			if (this._isGunnarSaved)
			{
				this.SetDialogs();
			}
		}

		// Token: 0x060003F7 RID: 1015 RVA: 0x0001C7B0 File Offset: 0x0001A9B0
		protected override void RegisterEvents()
		{
			NavalDLCEvents.OnGunnarSavedEvent.AddNonSerializedListener(this, new Action(this.OnGunnarSaved));
			NavalDLCEvents.OnNavalStorylineCanceledEvent.AddNonSerializedListener(this, new Action<NavalStorylineData.StorylineCancelDetail>(this.OnNavalStorylineCanceled));
			CampaignEvents.LocationCharactersAreReadyToSpawnEvent.AddNonSerializedListener(this, new Action<Dictionary<string, int>>(this.LocationCharactersAreReadyToSpawn));
			CampaignEvents.GameMenuOpened.AddNonSerializedListener(this, new Action<MenuCallbackArgs>(this.OnGameMenuOpened));
			NavalDLCEvents.OnNavalStorylineTutorialSkippedEvent.AddNonSerializedListener(this, new Action(this.OnNavalTutorialSkipped));
		}

		// Token: 0x060003F8 RID: 1016 RVA: 0x0001C830 File Offset: 0x0001AA30
		private void OnGunnarSaved()
		{
			this._isGunnarSaved = true;
			this.SetDialogs();
			base.AddLog(this._isGunnarSavedLog, false);
			base.AddTrackedObject(NavalStorylineData.Purig);
			NavalStorylineData.Gunnar.SetPersonalRelation(Hero.MainHero, 15);
		}

		// Token: 0x060003F9 RID: 1017 RVA: 0x0001C869 File Offset: 0x0001AA69
		private void OnNavalTutorialSkipped()
		{
			base.AddLog(this._tutorialSkippedLog, false);
			base.CompleteQuestWithSuccess();
			NavalStorylineData.Gunnar.SetPersonalRelation(Hero.MainHero, 10);
		}

		// Token: 0x060003FA RID: 1018 RVA: 0x0001C890 File Offset: 0x0001AA90
		private void OnNavalStorylineCanceled(NavalStorylineData.StorylineCancelDetail detail)
		{
			if (NavalStorylineData.Gunnar.IsActive)
			{
				DisableHeroAction.Apply(NavalStorylineData.Gunnar);
				LocationComplex locationComplex = Settlement.CurrentSettlement.LocationComplex;
				Location location = ((locationComplex != null) ? locationComplex.GetLocationOfCharacter(NavalStorylineData.Gunnar) : null);
				if (location != null && location.GetLocationCharacter(NavalStorylineData.Gunnar) != null)
				{
					Settlement.CurrentSettlement.LocationComplex.RemoveCharacterIfExists(NavalStorylineData.Gunnar);
					LocationEncounter locationEncounter = PlayerEncounter.LocationEncounter;
					if (locationEncounter != null)
					{
						locationEncounter.RemoveAccompanyingCharacter(NavalStorylineData.Gunnar);
					}
				}
			}
			base.CompleteQuestWithFail(this._cancelQuestLog);
		}

		// Token: 0x060003FB RID: 1019 RVA: 0x0001C914 File Offset: 0x0001AB14
		protected override void HourlyTick()
		{
		}

		// Token: 0x060003FC RID: 1020 RVA: 0x0001C918 File Offset: 0x0001AB18
		public override GameMenuOption.IssueQuestFlags IsLocationTrackedByQuest(Location location)
		{
			if (Settlement.CurrentSettlement == NavalStorylineData.HomeSettlement)
			{
				if (this._isGunnarSaved)
				{
					if (location.StringId == "tavern" && !location.ContainsCharacter(NavalStorylineData.Purig))
					{
						return 16;
					}
				}
				else if (location.StringId == "port")
				{
					return 4;
				}
			}
			return 0;
		}

		// Token: 0x060003FD RID: 1021 RVA: 0x0001C970 File Offset: 0x0001AB70
		private void LocationCharactersAreReadyToSpawn(Dictionary<string, int> unusedUsablePointCount)
		{
			Settlement settlement = PlayerEncounter.LocationEncounter.Settlement;
			if (NavalStorylineData.HomeSettlement == settlement && settlement.IsTown && CampaignMission.Current != null)
			{
				Location location = CampaignMission.Current.Location;
				if (location != null && location.StringId == "tavern" && !NavalStorylineData.Purig.IsDead && this._isGunnarSaved)
				{
					location.AddLocationCharacters(new CreateLocationCharacterDelegate(this.CreateNortherner), settlement.Culture, 0, 1);
				}
			}
		}

		// Token: 0x060003FE RID: 1022 RVA: 0x0001C9F0 File Offset: 0x0001ABF0
		private void OnGameMenuOpened(MenuCallbackArgs args)
		{
			if (this._playCutscene && GameStateManager.Current.ActiveState is MapState)
			{
				this._playCutscene = false;
				VideoPlaybackState videoPlaybackState = Game.Current.GameStateManager.CreateState<VideoPlaybackState>();
				string text = ModuleHelper.GetModuleFullPath("NavalDLC") + "Videos/Storyline/";
				string text2 = text + "naval_storyline_intro";
				float num = 24f;
				string text3 = text + "naval_storyline_intro_cinematic.ivf";
				string text4 = text + "naval_storyline_intro_cinematic.ogg";
				videoPlaybackState.SetStartingParameters(text3, text4, text2, num, true);
				videoPlaybackState.SetOnVideoFinisedDelegate(new Action(this.OnCinematicCompleted));
				Game.Current.GameStateManager.PushState(videoPlaybackState, 0);
			}
		}

		// Token: 0x060003FF RID: 1023 RVA: 0x0001CAA0 File Offset: 0x0001ACA0
		private LocationCharacter CreateNortherner(CultureObject culture, LocationCharacter.CharacterRelations relation)
		{
			CharacterObject characterObject = NavalStorylineData.Purig.CharacterObject;
			Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(characterObject.Race, "_settlement");
			AgentData agentData = new AgentData(new SimpleAgentOrigin(characterObject, -1, null, default(UniqueTroopDescriptor))).Monster(monsterWithSuffix);
			return new LocationCharacter(agentData, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddCompanionBehaviors), "sp_storyline_npc", true, relation, ActionSetCode.GenerateActionSetNameWithSuffix(agentData.AgentMonster, agentData.AgentIsFemale, "_villager"), true, false, null, false, false, true, null, false);
		}

		// Token: 0x06000400 RID: 1024 RVA: 0x0001CB28 File Offset: 0x0001AD28
		private void AddNorthernerDialog()
		{
			DialogFlow dialogFlow = DialogFlow.CreateDialogFlow("start", 1200);
			dialogFlow.AddDialogLine("northerner_meet_dialog_start_before_met", "start", "northerner_meet_dialog_player_options", "{=ay0tHozl}Aye? So who're you, then?", new ConversationSentence.OnConditionDelegate(this.northerner_meet_dialog_start_before_met_on_condition), null, this, 1200, null, null, null);
			dialogFlow.AddPlayerLine("northerner_meet_dialog_player_options_1", "northerner_meet_dialog_player_options", "northerner_meet_dialog_continue", "{=HXnni7no}I am {PLAYER.NAME}. Gunnar sent me. We were in a fight.", null, null, this, 100, null, null, null, null);
			dialogFlow.AddPlayerLine("northerner_meet_dialog_player_options_2", "northerner_meet_dialog_player_options", "northerner_meet_dialog_continue", "{=O4kwRlyY}I helped out Gunnar in a fight. He said he planned to sail with you.", null, null, this, 100, null, null, null, null);
			dialogFlow.AddDialogLine("northerner_meet_dialog_continue_1_line", "northerner_meet_dialog_continue", "northerner_meet_dialogue_continue_2", "{=4K9ycbC8}A fight, you say… I take it that Gunnar and you won?", null, null, this, 100, null, null, null);
			dialogFlow.AddPlayerLine("northerner_meet_dialog_continue_2_line", "northerner_meet_dialogue_continue_2", "northerner_meet_dialog_aftermath", "{=uyWWPIxA}Yes, we defeated three Sea Hounds. Now I wish to sail with you.", null, null, this, 100, null, null, null, null);
			dialogFlow.AddPlayerLine("northerner_meet_dialog_continue_3_line", "northerner_meet_dialogue_continue_2", "northerner_meet_dialog_aftermath", "{=Ic4e9HVF}We won, and now I wish to join you against our common enemy.", null, null, this, 100, null, null, null, null);
			dialogFlow.AddDialogLine("northerner_meet_dialog_aftermath_line_1", "northerner_meet_dialog_aftermath", "northerner_meet_dialog_aftermath_2", "{=Ni7ienXY}Well... Good for you two! Gunnar is a tough old goat and rather hard to kill. I shall have to ask him all about it when I get the chance. So... Yes, I agreed to help him in his little feud with the Sea Hounds, for old time's sake. I've got my ship and men ready to sail.", null, null, this, 100, null, null, null);
			dialogFlow.AddDialogLine("northerner_meet_dialog_aftermath_line_2", "northerner_meet_dialog_aftermath_2", "northerner_quest_options", "{=0JNfhDrT}If you're indeed of a mind to go with us, I'm happy to take you. But I've got room for only you. So if you've got any traveling companions, you'll need to leave them in this port. I'm sure you'll be back soon to rejoin them, safe and sound.", null, null, this, 100, null, null, null);
			DialogFlow dialogFlow2 = dialogFlow;
			string text = "northerner_quest_options_1_line";
			string text2 = "northerner_quest_options";
			string text3 = "northerner_quest_options_1";
			string text4 = "{=S1ES8FFM}I'd feel better if my men could come along as well...";
			ConversationSentence.OnConditionDelegate onConditionDelegate = null;
			ConversationSentence.OnConsequenceDelegate onConsequenceDelegate = new ConversationSentence.OnConsequenceDelegate(this.northerner_quest_options_1_consequence);
			ConversationSentence.OnClickableConditionDelegate onClickableConditionDelegate = new ConversationSentence.OnClickableConditionDelegate(this.northerner_quest_options_1_clickable_condition);
			dialogFlow2.AddPlayerLine(text, text2, text3, text4, onConditionDelegate, onConsequenceDelegate, this, 100, onClickableConditionDelegate, null, null, null);
			dialogFlow.AddDialogLine("northerner_quest_options_1_line_continue", "northerner_quest_options_1", "northerner_quest_options", "{=MjIfvPk9}The northern seas aren't for everyone! Even if you had your own ship, it would just slow us down. Don't worry, me and my boys know those waters like the back of our hands. We won't let you slip overboard.", null, null, this, 100, null, null, null);
			DialogFlow dialogFlow3 = dialogFlow;
			string text5 = "northerner_quest_options_2_line";
			string text6 = "northerner_quest_options";
			string text7 = "northerner_quest_options_2_answer_1";
			string text8 = "{=R6CH1xOc}Did you also fight in this rebellion with Gunnar?";
			ConversationSentence.OnConditionDelegate onConditionDelegate2 = null;
			ConversationSentence.OnConsequenceDelegate onConsequenceDelegate2 = new ConversationSentence.OnConsequenceDelegate(this.northerner_quest_options_2_answer_1_consequence);
			onClickableConditionDelegate = new ConversationSentence.OnClickableConditionDelegate(this.northerner_quest_options_2_answer_1_clickable_condition);
			dialogFlow3.AddPlayerLine(text5, text6, text7, text8, onConditionDelegate2, onConsequenceDelegate2, this, 100, onClickableConditionDelegate, null, null, null);
			dialogFlow.AddDialogLine("northerner_quest_options_2_continue_1", "northerner_quest_options_2_answer_1", "northerner_quest_options_2_answer_2", "{=sfuFR9fr}I did, I did. We started out as young men with nothing but our swords, our sweet mistress the sea whispering promises of wealth and glory in our ears... We served no kings and had no lords. Those were fine times!", null, null, this, 100, null, null, null);
			dialogFlow.AddDialogLine("northerner_quest_options_2_continue_2", "northerner_quest_options_2_answer_2", "northerner_quest_options_2_answer_3", "{=pGeYLxkL}Then old Volbjorn brought down the full weight of the north on our brotherhood. Against those odds we could not fight. But some of our old comrades weren't quite ready to abandon that life, and they  turned pirate and became the Sea Hounds...", null, null, this, 100, null, null, null);
			dialogFlow.AddDialogLine("northerner_quest_options_2_continue_3", "northerner_quest_options_2_answer_3", "northerner_quest_options", "{=06hn50KS}Now Gunnar says they are even worse than the king and the jarls we fought, preying upon the farmers and fishermen of the coast. There's no honor in attacking the weak, he told me so many times. And he's right, of course - it's just that it's so much easier to take their wealth!", null, null, this, 100, null, null, null);
			dialogFlow.AddPlayerLine("northerner_quest_options_3_line", "northerner_quest_options", "northerner_quest_options_3_answer", "{=roU1EPwp}Very well. I'll make ready to sail.", null, null, this, 100, new ConversationSentence.OnClickableConditionDelegate(this.CanSetSailWithNortherner), null, null, null);
			dialogFlow.AddDialogLine("northerner_quest_options_3_continue", "northerner_quest_options_3_answer", "close_window", "{=5LbipyXT}Come down to the ship with me, then! Wind and tide are with us, and I won't tarry long.", null, new ConversationSentence.OnConsequenceDelegate(this.northerner_quest_options_3_continue_on_consequence), this, 100, null, null, null);
			dialogFlow.AddPlayerLine("northerner_quest_options_4_line", "northerner_quest_options", "northerner_quest_options_4_answer", "{=18bzzaFH}I'm not ready to sail just yet.", null, null, this, 100, null, null, null, null);
			dialogFlow.AddDialogLine("northerner_quest_options_4_continue", "northerner_quest_options_4_answer", "close_window", "{=s9Rz14CU}Are you sure you're cut out for a life at sea? Make haste when wind and tide are with you, friend! Anyway, come back when you're ready.", null, null, this, 100, null, null, null);
			dialogFlow.AddDialogLine("northerner_meet_dialog_start_after_met", "start", "northerner_returned_options", "{=b9hRGOhC}All is good? Packed your bag, kissed your mother and your sweetheart good-bye? Of course my lads and I won't mind if you want to tarry here a little longer. Oh no. There's no hurry at all.", new ConversationSentence.OnConditionDelegate(this.northerner_meet_dialog_start_after_met_on_condition), null, this, 1200, null, null, null);
			dialogFlow.AddPlayerLine("northerner_returned_options_1", "northerner_returned_options", "northerner_quest_options_3_answer", "{=nLM7Lu2m}All is good. I am ready to sail.", null, null, this, 100, new ConversationSentence.OnClickableConditionDelegate(this.CanSetSailWithNortherner), null, null, null);
			dialogFlow.AddPlayerLine("northerner_returned_options_2", "northerner_returned_options", "northerner_quest_options_4_answer", "{=18bzzaFH}I'm not ready to sail just yet.", null, null, this, 100, null, null, null, null);
			Campaign.Current.ConversationManager.AddDialogFlow(dialogFlow, null);
		}

		// Token: 0x06000401 RID: 1025 RVA: 0x0001CEAC File Offset: 0x0001B0AC
		private bool CanSetSailWithNortherner(out TextObject reasonText)
		{
			reasonText = null;
			bool flag = NavalStorylineData.IsStorylineActivationPossible();
			if (!flag)
			{
				if (MobileParty.MainParty.Army != null)
				{
					reasonText = new TextObject("{=q9fzW0W3}You cannot do this while you are in an army.", null);
					return flag;
				}
				if (Campaign.Current.IsMainHeroDisguised)
				{
					reasonText = new TextObject("{=V9Ub68T7}You cannot do this while disguised.", null);
					return flag;
				}
				reasonText = new TextObject("{=H6F5BxgB}This isn't the right time.", null);
			}
			return flag;
		}

		// Token: 0x06000402 RID: 1026 RVA: 0x0001CF05 File Offset: 0x0001B105
		private bool northerner_meet_dialog_came_back_on_condition()
		{
			return Hero.OneToOneConversationHero == NavalStorylineData.Purig && Hero.OneToOneConversationHero.HasMet;
		}

		// Token: 0x06000403 RID: 1027 RVA: 0x0001CF1F File Offset: 0x0001B11F
		private bool northerner_meet_dialog_start_before_met_on_condition()
		{
			StringHelpers.SetCharacterProperties("PLAYER", Hero.MainHero.CharacterObject, null, false);
			return Hero.OneToOneConversationHero == NavalStorylineData.Purig && !Hero.OneToOneConversationHero.HasMet && NavalStorylineData.HasCompletedLast(NavalStorylineData.NavalStorylineStage.None);
		}

		// Token: 0x06000404 RID: 1028 RVA: 0x0001CF58 File Offset: 0x0001B158
		private bool northerner_meet_dialog_start_after_met_on_condition()
		{
			StringHelpers.SetCharacterProperties("PLAYER", Hero.MainHero.CharacterObject, null, false);
			return Hero.OneToOneConversationHero == NavalStorylineData.Purig && Hero.OneToOneConversationHero.HasMet && NavalStorylineData.HasCompletedLast(NavalStorylineData.NavalStorylineStage.None);
		}

		// Token: 0x06000405 RID: 1029 RVA: 0x0001CF91 File Offset: 0x0001B191
		private void northerner_quest_options_1_consequence()
		{
			this._northernerQuestOptions1Selected = true;
		}

		// Token: 0x06000406 RID: 1030 RVA: 0x0001CF9A File Offset: 0x0001B19A
		private bool northerner_quest_options_1_clickable_condition(out TextObject explanation)
		{
			explanation = TextObject.GetEmpty();
			return !this._northernerQuestOptions1Selected;
		}

		// Token: 0x06000407 RID: 1031 RVA: 0x0001CFAC File Offset: 0x0001B1AC
		private void northerner_quest_options_2_answer_1_consequence()
		{
			this._northernerQuestOptions2Answer1Selected = true;
		}

		// Token: 0x06000408 RID: 1032 RVA: 0x0001CFB5 File Offset: 0x0001B1B5
		private bool northerner_quest_options_2_answer_1_clickable_condition(out TextObject explanation)
		{
			explanation = TextObject.GetEmpty();
			return !this._northernerQuestOptions2Answer1Selected;
		}

		// Token: 0x06000409 RID: 1033 RVA: 0x0001CFC7 File Offset: 0x0001B1C7
		private void northerner_quest_options_3_continue_on_consequence()
		{
			Campaign.Current.ConversationManager.ConversationEndOneShot += this.OnDialogueEnded;
		}

		// Token: 0x0600040A RID: 1034 RVA: 0x0001CFE4 File Offset: 0x0001B1E4
		private void OnDialogueEnded()
		{
			this._playCutscene = true;
			Mission mission = Mission.Current;
			if (mission == null)
			{
				return;
			}
			mission.EndMission();
		}

		// Token: 0x0600040B RID: 1035 RVA: 0x0001CFFC File Offset: 0x0001B1FC
		private void OnCinematicCompleted()
		{
			GameStateManager.Current.PopState(0);
			Settlement.CurrentSettlement.LocationComplex.RemoveCharacterIfExists(NavalStorylineData.Purig);
			base.CompleteQuestWithSuccess();
			new DefeatTheCaptorsQuest("naval_storyline_defeat_the_captors_quest").StartQuest();
		}

		// Token: 0x0400024F RID: 591
		[SaveableField(1)]
		private bool _isGunnarSaved;

		// Token: 0x04000250 RID: 592
		private bool _playCutscene;

		// Token: 0x04000251 RID: 593
		private bool _northernerQuestOptions2Answer1Selected;

		// Token: 0x04000252 RID: 594
		private bool _northernerQuestOptions1Selected;
	}
}
