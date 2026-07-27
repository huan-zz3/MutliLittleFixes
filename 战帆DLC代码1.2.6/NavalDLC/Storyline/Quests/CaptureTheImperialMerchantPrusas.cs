using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using NavalDLC.Missions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.SaveSystem;

namespace NavalDLC.Storyline.Quests
{
	// Token: 0x02000033 RID: 51
	public class CaptureTheImperialMerchantPrusas : NavalStorylineQuestBase
	{
		// Token: 0x17000072 RID: 114
		// (get) Token: 0x060002F5 RID: 757 RVA: 0x000162FB File Offset: 0x000144FB
		public int SelectedOption
		{
			get
			{
				return this._selectedOption;
			}
		}

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x060002F6 RID: 758 RVA: 0x00016303 File Offset: 0x00014503
		public override bool WillProgressStoryline
		{
			get
			{
				return this._willProgressStoryline;
			}
		}

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x060002F7 RID: 759 RVA: 0x0001630B File Offset: 0x0001450B
		public override TextObject Title
		{
			get
			{
				return new TextObject("{=2eXHN7v8}Capture the Merchant Crusas", null);
			}
		}

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x060002F8 RID: 760 RVA: 0x00016318 File Offset: 0x00014518
		private TextObject DescriptionLogText
		{
			get
			{
				return new TextObject("{=uGTU4k9w}Defeat Crusas' fleet and take him prisoner.", null);
			}
		}

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x060002F9 RID: 761 RVA: 0x00016325 File Offset: 0x00014525
		private TextObject MainCorsairShipSpawnedLogText
		{
			get
			{
				TextObject textObject = new TextObject("{=6HCOzjBt}The way is now clear to attack {HERO.NAME}'s fleet. Destroy it!", null);
				TextObjectExtensions.SetCharacterProperties(textObject, "HERO", NavalStorylineData.Prusas.CharacterObject, false);
				return textObject;
			}
		}

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x060002FA RID: 762 RVA: 0x00016348 File Offset: 0x00014548
		private TextObject PlayerStartsQuestLogText
		{
			get
			{
				TextObject textObject = new TextObject("{=vgnaNH9O}You've learned that Purig's ally, the merchant {HERO.NAME}, is anchored in the Skatria islands. You should sail there and defeat him, along with any other Sea Hounds you find there.", null);
				TextObjectExtensions.SetCharacterProperties(textObject, "HERO", NavalStorylineData.Prusas.CharacterObject, false);
				TextObjectExtensions.SetCharacterProperties(textObject, "ISSUE_GIVER", base.QuestGiver.CharacterObject, false);
				return textObject;
			}
		}

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x060002FB RID: 763 RVA: 0x00016382 File Offset: 0x00014582
		private TextObject QuestSucceededWithHonorableOptionLogText
		{
			get
			{
				TextObject textObject = new TextObject("{=GFERb4SK}You promised {HERO.NAME} his life if he helped you capture Purig's prisoner ship.  (+{HONOR_BONUS_AMOUNT} honor bonus)", null);
				textObject.SetTextVariable("HONOR_BONUS_AMOUNT", 50);
				return textObject;
			}
		}

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x060002FC RID: 764 RVA: 0x0001639D File Offset: 0x0001459D
		private TextObject QuestSucceededWithCalculatingOptionLogText
		{
			get
			{
				TextObject textObject = new TextObject("{=4wJCXVb4}You forced {HERO.NAME} to help you capture Purig's prisoner ship, promising him nothing. (+{CALCULATING_BONUS_AMOUNT} calculating bonus)", null);
				textObject.SetTextVariable("CALCULATING_BONUS_AMOUNT", 50);
				return textObject;
			}
		}

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x060002FD RID: 765 RVA: 0x000163B8 File Offset: 0x000145B8
		public override NavalStorylineData.NavalStorylineStage Stage
		{
			get
			{
				return NavalStorylineData.NavalStorylineStage.Act3Quest4;
			}
		}

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x060002FE RID: 766 RVA: 0x000163BB File Offset: 0x000145BB
		protected override string MainPartyTemplateStringId
		{
			get
			{
				return "storyline_act3_quest_4_main_party_template";
			}
		}

		// Token: 0x060002FF RID: 767 RVA: 0x000163C4 File Offset: 0x000145C4
		public CaptureTheImperialMerchantPrusas(string questId, Hero questGiver, CampaignVec2 corsairSpawnPosition)
			: base(questId, questGiver, CampaignTime.Never, 0)
		{
			this._willProgressStoryline = false;
			this._numberOfDefeatedCorsairParties = 0;
			this._corsairParties = new List<MobileParty>();
			this._bossCorsairParty = null;
			this._corsairSpawnPosition = corsairSpawnPosition;
			base.AddLog(this.DescriptionLogText, false);
		}

		// Token: 0x06000300 RID: 768 RVA: 0x00016414 File Offset: 0x00014614
		protected override void OnFinalizeInternal()
		{
			this._playerStartsQuestLog = null;
			this.DestroyCorsairParties();
		}

		// Token: 0x06000301 RID: 769 RVA: 0x00016423 File Offset: 0x00014623
		protected override void InitializeQuestOnGameLoadInternal()
		{
			this.SetDialogs();
			this.AddGameMenus();
		}

		// Token: 0x06000302 RID: 770 RVA: 0x00016431 File Offset: 0x00014631
		protected override void SetDialogs()
		{
			this.AddDialogsForFinalFight();
		}

		// Token: 0x06000303 RID: 771 RVA: 0x0001643C File Offset: 0x0001463C
		protected override void OnStartQuestInternal()
		{
			this.SetDialogs();
			this.AddGameMenus();
			this._numberOfDefeatedCorsairParties = 2;
			this.SpawnMainCorsairParty();
			this._willProgressStoryline = true;
			MBInformationManager.AddQuickInformation(new TextObject("{=vbrXtMyM}Feel that hot fetid air? It means we’re in the Skatrias, now. The foe is near…", null), 200, NavalStorylineData.Gunnar.CharacterObject, null, "");
		}

		// Token: 0x06000304 RID: 772 RVA: 0x00016490 File Offset: 0x00014690
		protected override void HourlyTick()
		{
			foreach (MobileParty mobileParty in this._corsairParties)
			{
				if (mobileParty.IsActive && !mobileParty.IsMoving && !mobileParty.Ai.IsDisabled)
				{
					CampaignVec2 campaignVec = NavigationHelper.FindReachablePointAroundPosition(this._corsairSpawnPosition, 2, 20f, 5f, false);
					mobileParty.SetMoveGoToPoint(campaignVec, 2);
				}
			}
		}

		// Token: 0x06000305 RID: 773 RVA: 0x0001651C File Offset: 0x0001471C
		protected override void IsNavalQuestPartyInternal(PartyBase party, NavalStorylinePartyData data)
		{
			if (this._corsairParties.Any<MobileParty>((MobileParty c) => c.Party == party))
			{
				PartyTemplateObject @object = Campaign.Current.ObjectManager.GetObject<PartyTemplateObject>("storyline_act3_quest_4_corsair_generic_template");
				data.PartySize = (int)NavalDLCHelpers.GetMaxPartySizeLimitFromTemplate(@object).ResultNumber;
				data.IsQuestParty = true;
				return;
			}
			if (this._bossCorsairParty != null && this._bossCorsairParty.Party == party)
			{
				PartyTemplateObject object2 = Campaign.Current.ObjectManager.GetObject<PartyTemplateObject>("storyline_act3_quest_4_boss_corsair_template");
				data.PartySize = (int)NavalDLCHelpers.GetMaxPartySizeLimitFromTemplate(object2).ResultNumber;
				data.IsQuestParty = true;
			}
		}

		// Token: 0x06000306 RID: 774 RVA: 0x000165D0 File Offset: 0x000147D0
		protected override void OnCompleteWithSuccessInternal()
		{
			MobileParty.MainParty.MemberRoster.RemoveTroop(NavalStorylineData.Bjolgur.CharacterObject, 1, default(UniqueTroopDescriptor), 0);
			NavalStorylineData.Bjolgur.ChangeState(6);
			NavalStorylineData.OnCheckpointReached(NavalStorylineData.NavalStorylineCheckpoint.Act3Quest4Succeeded);
		}

		// Token: 0x06000307 RID: 775 RVA: 0x00016614 File Offset: 0x00014814
		protected override void OnFailedInternal()
		{
			MobileParty.MainParty.MemberRoster.RemoveTroop(NavalStorylineData.Bjolgur.CharacterObject, 1, default(UniqueTroopDescriptor), 0);
			NavalStorylineData.Bjolgur.ChangeState(6);
		}

		// Token: 0x06000308 RID: 776 RVA: 0x00016650 File Offset: 0x00014850
		public void OnCheckPointReached()
		{
			this._checkpointReached = true;
		}

		// Token: 0x06000309 RID: 777 RVA: 0x0001665C File Offset: 0x0001485C
		protected override void RegisterEventsInternal()
		{
			CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(this, new Action<MobileParty, PartyBase>(this.OnMobilePartyDestroyed));
			CampaignEvents.MapEventStarted.AddNonSerializedListener(this, new Action<MapEvent, PartyBase, PartyBase>(this.OnMapEventStarted));
			CampaignEvents.OnMissionEndedEvent.AddNonSerializedListener(this, new Action<IMission>(this.OnMissionEnded));
			CampaignEvents.GameMenuOpened.AddNonSerializedListener(this, new Action<MenuCallbackArgs>(this.OnGameMenuOpened));
			CampaignEvents.OnShipOwnerChangedEvent.AddNonSerializedListener(this, new Action<Ship, PartyBase, ChangeShipOwnerAction.ShipOwnerChangeDetail>(this.OnShipOwnerChanged));
			CampaignEvents.BeforeGameMenuOpenedEvent.AddNonSerializedListener(this, new Action<MenuCallbackArgs>(this.OnBeforeGameMenuOpened));
			CampaignEvents.ConversationEnded.AddNonSerializedListener(this, new Action<IEnumerable<CharacterObject>>(this.OnConversationEnded));
		}

		// Token: 0x0600030A RID: 778 RVA: 0x0001670C File Offset: 0x0001490C
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

		// Token: 0x0600030B RID: 779 RVA: 0x000167B0 File Offset: 0x000149B0
		private void OnShipOwnerChanged(Ship ship, PartyBase partyBase, ChangeShipOwnerAction.ShipOwnerChangeDetail shipOwnerChangeDetail)
		{
			if (partyBase == PartyBase.MainParty && ship.IsInvulnerable)
			{
				ship.IsInvulnerable = false;
			}
		}

		// Token: 0x0600030C RID: 780 RVA: 0x000167CC File Offset: 0x000149CC
		private void OnConversationEnded(IEnumerable<CharacterObject> conversationCharacters)
		{
			if (NavalStorylineData.IsNavalStoryLineActive() && this._battleWon && conversationCharacters.Contains(NavalStorylineData.Prusas.CharacterObject))
			{
				int selectedOption = this._selectedOption;
				if (selectedOption == 1)
				{
					this.OnPlayerSelectsOption1();
					return;
				}
				if (selectedOption == 2)
				{
					this.OnPlayerSelectsOption2();
					return;
				}
				Debug.FailedAssert("Quest selected option is wrong!", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\Storyline\\Quests\\CaptureTheImperialMerchantPrusas.cs", "OnConversationEnded", 255);
			}
		}

		// Token: 0x0600030D RID: 781 RVA: 0x00016834 File Offset: 0x00014A34
		private void OnBeforeGameMenuOpened(MenuCallbackArgs args)
		{
			if (NavalStorylineData.IsNavalStoryLineActive() && PlayerEncounter.EncounteredParty != null && PlayerEncounter.EncounteredParty.IsMobile && PlayerEncounter.EncounteredParty.IsNavalStorylineQuestParty())
			{
				if (!this._corsairParties.Contains(PlayerEncounter.EncounteredParty.MobileParty))
				{
					PartyBase encounteredParty = PlayerEncounter.EncounteredParty;
					MobileParty bossCorsairParty = this._bossCorsairParty;
					if (encounteredParty != ((bossCorsairParty != null) ? bossCorsairParty.Party : null))
					{
						return;
					}
				}
				string stringId = args.MenuContext.GameMenu.StringId;
				if (stringId == "naval_storyline_encounter_meeting")
				{
					PlayerEncounter.SetMeetingDone();
					return;
				}
				if (stringId == "naval_storyline_encounter")
				{
					TextObject textObject = new TextObject("{=7b05ZaVm}You are in the Skatrias. The jagged silhouettes of small rocky islands, streaked with gull dung, stretch southwest to the horizon.{NEW_LINE}{NEW_LINE}Through the hazy air you make out the outline of a sail. It’s still quite distant, but closing fast. They are clearly Sea Hounds, ready to pounce on anyone who ventures into their hunting grounds in the Skatrias.", null).SetTextVariable("NEW_LINE", "\n");
					MBTextManager.SetTextVariable("ENCOUNTER_TEXT", textObject, false);
				}
			}
		}

		// Token: 0x0600030E RID: 782 RVA: 0x000168FC File Offset: 0x00014AFC
		private void OnGameMenuOpened(MenuCallbackArgs args)
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
			if (text == "naval_storyline_encounter" && PlayerEncounter.EncounteredParty != null && NavalStorylineData.IsNavalStoryLineActive())
			{
				MobileParty bossCorsairParty = this._bossCorsairParty;
				if (((bossCorsairParty != null) ? bossCorsairParty.Party : null) == PlayerEncounter.EncounteredParty)
				{
					NavalStorylineData.OnCheckpointReached(NavalStorylineData.NavalStorylineCheckpoint.Act3Quest4EncounterMenu);
					GameMenu.ActivateGameMenu("naval_storyline_act_3_quest_4_encounter_menu");
				}
			}
		}

		// Token: 0x0600030F RID: 783 RVA: 0x0001696C File Offset: 0x00014B6C
		private void OnMissionEnded(IMission mission)
		{
			if (PlayerEncounter.Current != null)
			{
				PartyBase encounteredParty = PlayerEncounter.EncounteredParty;
				MobileParty bossCorsairParty = this._bossCorsairParty;
				if (encounteredParty == ((bossCorsairParty != null) ? bossCorsairParty.Party : null))
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
							this._battleWon = true;
							return;
						}
					}
					else
					{
						if (PlayerEncounter.WinningSide == -1)
						{
							this._battleWon = false;
							return;
						}
						Debug.FailedAssert("unhandled case", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\Storyline\\Quests\\CaptureTheImperialMerchantPrusas.cs", "OnMissionEnded", 319);
					}
				}
			}
		}

		// Token: 0x06000310 RID: 784 RVA: 0x00016A00 File Offset: 0x00014C00
		private void OnMobilePartyDestroyed(MobileParty party, PartyBase partyBase)
		{
			if (NavalStorylineData.IsNavalStoryLineActive() && this._playerStartsQuestLog != null && this._corsairParties.Contains(party))
			{
				this._numberOfDefeatedCorsairParties++;
				this._corsairParties.Remove(party);
				base.UpdateQuestTaskStage(this._playerStartsQuestLog, this._numberOfDefeatedCorsairParties);
				if (2 == this._numberOfDefeatedCorsairParties)
				{
					this.SpawnMainCorsairParty();
					base.AddLog(this.MainCorsairShipSpawnedLogText, false);
					this._bossCorsairParty.SetMoveGoToPoint(MobileParty.MainParty.Position, 2);
					return;
				}
				MBInformationManager.AddQuickInformation(new TextObject("{=Kal82TKK}There may be more Sea Hounds patrolling these islands. Let's keep searching.", null), 0, NavalStorylineData.Gunnar.CharacterObject, null, "");
			}
		}

		// Token: 0x06000311 RID: 785 RVA: 0x00016AB8 File Offset: 0x00014CB8
		private void AddDialogsForFinalFight()
		{
			TextObject textObject = new TextObject("{=A1e4qar9}Did you see that big fiery ball? Not very accurate, I’ll warrant, but if one of our ships gets hit by one of those… Those who don’t jump into the sea in time will die a nasty death.", null);
			TextObject textObject2 = new TextObject("{=sawnbWQP}I’ve heard Crusas does this… He doesn’t try to maneuver or run, but lashes his ships together, building himself a floating fortress. He mounts mangonels on them, and peppers any attackers with flaming pitch. Not a bad tactic, if you’ve got the time to prepare and you just want to be left alone. Most attackers will keep their distance and look for easier prey.", null);
			TextObject textObject3 = new TextObject("{=Rc2iUkN2}No fortress is invulnerable.", null);
			TextObject textObject4 = new TextObject("{=ZYheTO7N}How do we counter this?", null);
			TextObject textObject5 = new TextObject("{=G5gTXNKi}If all our ships row in together, we’d be presenting enough targets that we’re bound to get hit. So let’s not do that. Here’s another idea…", null);
			TextObject textObject6 = new TextObject("{=0AWyunPW}Our captured ship, the Golden Wasp, is fast and maneuverable and has that ballista. If we make it as light as possible by removing all cargo and move in our strongest rowers to man the oars, we can dart within range while avoiding that flaming pitch. Then we can use the ballista to take out the mangonels one by one, and when they’re all down, the rest of us will storm in and clear their decks.", null);
			TextObject textObject7 = new TextObject("{=b8XvnNSs}Sounds like good fun. I’ll do it.", null);
			TextObject textObject8 = new TextObject("{=PUxIpByI}I’m not sure about this. Maybe you can command the Golden Wasp.", null);
			TextObject textObject9 = new TextObject("{=kW7yU5CE}I saw you handle that fireship at Omor, and I think you’re the one to take the helm. I’ll come with you though, to keep my men rowing briskly.", null);
			string text;
			string text2;
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1200).GenerateToken(ref text).GenerateToken(ref text2)
				.NpcLine(textObject, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsBjolgur), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), null, null)
				.Condition(() => Campaign.Current.QuestManager.IsThereActiveQuestWithType(typeof(CaptureTheImperialMerchantPrusas)) && Hero.OneToOneConversationHero == NavalStorylineData.Bjolgur && !this._hasRanMissionBefore)
				.NpcLine(textObject2, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsBjolgur), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), null, null)
				.BeginPlayerOptions(null, false)
				.PlayerOption(textObject3, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsBjolgur), null, null)
				.GotoDialogState(text)
				.PlayerOption(textObject4, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsBjolgur), null, null)
				.GotoDialogState(text)
				.EndPlayerOptions()
				.NpcLine(textObject5, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsBjolgur), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), text, null)
				.NpcLine(textObject6, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsBjolgur), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), null, null)
				.BeginPlayerOptions(null, false)
				.PlayerOption(textObject7, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsBjolgur), null, null)
				.GotoDialogState(text2)
				.PlayerOption(textObject8, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsBjolgur), null, null)
				.GotoDialogState(text2)
				.EndPlayerOptions()
				.NpcLine(textObject9, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsBjolgur), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), text2, null)
				.CloseDialog()
				.Consequence(delegate
				{
					(Campaign.Current.QuestManager.Quests.FirstOrDefault<QuestBase>((QuestBase x) => x is CaptureTheImperialMerchantPrusas) as CaptureTheImperialMerchantPrusas)._shouldRunMission = true;
				}), this);
			TextObject textObject10 = new TextObject("{=DaYQ2dm8}That was a good fight! You did a fine job taking out those mangonels.", null);
			TextObject textObject11 = new TextObject("{=0x6OBWqY}Now then… I wish to present you with my old acquaintance, Salautas Crusas, who gave himself up when the last of his men fell to our swords. He seems very sure of himself for a man in his circumstances, and will no doubt try to bluster his way out of trouble.", null);
			TextObject textObject12 = new TextObject("{=1L0smluY}Crusas! Step forward.", null);
			TextObject textObject13 = new TextObject("{=1aHDn1cc}I am Salautas Crusas. I sail under the protection of the Sea Hounds. If you kill me, it will not go well for you.", null);
			TextObject textObject14 = new TextObject("{=Y2hbEtJN}Your threats mean nothing to me. Tell me about your deals with Purig.", null);
			TextObject textObject15 = new TextObject("{=zIBIcnNa}You’re a slaver, the scum of the seas. Talk fast if you value your life.", null);
			TextObject textObject16 = new TextObject("{=edUrD21k}Yes, I buy slaves. They work my sulfur mines. Sulfur is valuable, and if I did not mine it another would. Anyway, these islands are part of no kingdom and I am violating no law. Since when does a pirate like yourself care about such things?", null);
			TextObject textObject17 = new TextObject("{=H5iCH92M}I am no pirate, but a liberator. I intend to free your captives.", null);
			TextObject textObject18 = new TextObject("{=kEBVuiUY}I have reason to believe that one of you slaving bastards has my sister.", null);
			TextObject textObject19 = new TextObject("{=hp67Xmzj}So then… I believe I have heard of you. {PLAYER.NAME}? Purig spoke of you. From what I know, I think I can be of use to you. Do we have a bargain? I tell you what I know, and you give my freedom.", null);
			TextObjectExtensions.SetCharacterProperties(textObject19, "PLAYER", Hero.MainHero.CharacterObject, false);
			TextObject textObject20 = new TextObject("{=v3NQFt1b}We might, if you speak truthfully.", null);
			TextObject textObject21 = new TextObject("{=J0j7IGno}You are in no place to speak of bargains.", null);
			TextObject textObject22 = new TextObject("{=l8XH22F7}So then. When I last spoke to Purig, I saw your sister among his captives, and tried to buy her. ‘Not that one,’ he said. ‘That’s my insurance against a pair of avenging furies.’ I think he grudgingly admired how persistently you pursued him.", null);
			TextObject textObject23 = new TextObject("{=JYSOmwV8}He told me the whole story. Apparently, you had taken passage with him on some voyage to the north, hoping to find and free your sister from pirates. Then, you stole his ship - or so he said. Realizing that you were a dangerous enemy, he made inquiries among his Sea Hound allies to find her. Now he keeps her as a hostage on a ship in his fleet.", null);
			TextObject textObject24 = new TextObject("{=blWf6oTJ}So.. I can tell you how to find Purig, which means you’ve found your sister as well. But if you harm me, it’s likely you’ll never have such a chance again. So I repeat - do we have a bargain? ", null);
			TextObject textObject25 = new TextObject("{=jdBPxHZQ}And I repeat: speak the full truth, and we might.", null);
			TextObject textObject26 = new TextObject("{=MebLhJmj}You try my patience. Speak if you value your life.", null);
			TextObject textObject27 = new TextObject("{=udKuGe2a}Indeed… So then, Purig has run a bit short of money, and has arranged to sell off some of his captives in Angranfjord, his hideaway in the north. He will be anchored there for the next several weeks, doing business with his favored buyers. You may be able to get close to him without him suspecting that anything is amiss. He will not sell your sister, though, as I explained.", null);
			TextObject textObject28 = new TextObject("{=Yj4RhLbo}Were you to be one of these buyers?", null);
			TextObject textObject29 = new TextObject("{=kh5HVkT1}Among others, yes.", null);
			TextObject textObject30 = new TextObject("{=G7ekdQvI}Good. Then we will take your ship. It has fine lines and expensive fittings, and I have no doubt that Purig, who has an eye for costly things, would recognize it instantly", null);
			TextObject textObject31 = new TextObject("{=zDG4dNbj}{PLAYER.NAME}... If Purig is holding your sister as a hostage, then capturing his roundship will be a very delicate affair. If he sees Crusas’ ship and believes that we are Crusas, we may be able to allay his suspicions while we sneak aboard and turn things to our advantage.", null);
			TextObjectExtensions.SetCharacterProperties(textObject31, "PLAYER", Hero.MainHero.CharacterObject, false);
			TextObject textObject32 = new TextObject("{=0L1ZKRk4}We shall need to think on this, but it might even be good to keep Crusas with us, to converse with Purig or his crew.", null);
			TextObject textObject33 = new TextObject("{=QmIfTGw4}Good news, Crusas! You are indeed worth more to us alive than dead, for now.", null);
			TextObject textObject34 = new TextObject("{=SsAit4jx}For now, you say. What, might I ask, is to be my fate?", null);
			TextObject textObject35 = new TextObject("{=ijvIIOfv}If you don’t play us false, we’ll have mercy on you. (+{HONOR_BONUS_AMOUNT} Honor Bonus)", null);
			textObject35.SetTextVariable("HONOR_BONUS_AMOUNT", 50);
			TextObject textObject36 = new TextObject("{=zkYn0OKb}I will make you no promises. (+{CALCULATING_BONUS_AMOUNT} Calculating Bonus)", null);
			textObject36.SetTextVariable("CALCULATING_BONUS_AMOUNT", 50);
			TextObject textObject37 = new TextObject("{=uUrrMnad}Well, that’s decided then. We should return to Ostican to refit and gather our allies, then prepare to sail for Angranfjord.", null);
			string text3;
			string text4;
			string text5;
			string text6;
			string text7;
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1200).GenerateToken(ref text3).GenerateToken(ref text4)
				.GenerateToken(ref text5)
				.GenerateToken(ref text6)
				.GenerateToken(ref text7)
				.NpcLine(textObject10, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsBjolgur), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), null, null)
				.Condition(new ConversationSentence.OnConditionDelegate(this.MultiAgentConversationCondition))
				.NpcLine(textObject11, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsBjolgur), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), null, null)
				.NpcLine(textObject12, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsBjolgur), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsCrusas), null, null)
				.NpcLine(textObject13, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsCrusas), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), null, null)
				.BeginPlayerOptions(null, false)
				.PlayerOption(textObject14, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsCrusas), null, null)
				.GotoDialogState(text3)
				.PlayerOption(textObject15, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsCrusas), null, null)
				.GotoDialogState(text3)
				.EndPlayerOptions()
				.NpcLine(textObject16, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsCrusas), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), text3, null)
				.BeginPlayerOptions(null, false)
				.PlayerOption(textObject17, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsCrusas), null, null)
				.GotoDialogState(text4)
				.PlayerOption(textObject18, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsCrusas), null, null)
				.GotoDialogState(text4)
				.EndPlayerOptions()
				.NpcLine(textObject19, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsCrusas), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), text4, null)
				.BeginPlayerOptions(null, false)
				.PlayerOption(textObject20, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsCrusas), null, null)
				.GotoDialogState(text5)
				.PlayerOption(textObject21, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsCrusas), null, null)
				.GotoDialogState(text5)
				.EndPlayerOptions()
				.NpcLine(textObject22, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsCrusas), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), text5, null)
				.NpcLine(textObject23, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsCrusas), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), null, null)
				.NpcLine(textObject24, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsCrusas), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), null, null)
				.BeginPlayerOptions(null, false)
				.PlayerOption(textObject25, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsCrusas), null, null)
				.GotoDialogState(text6)
				.PlayerOption(textObject26, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsCrusas), null, null)
				.GotoDialogState(text6)
				.EndPlayerOptions()
				.NpcLine(textObject27, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsCrusas), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), text6, null)
				.NpcLine(textObject28, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsGunnar), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsCrusas), null, null)
				.NpcLine(textObject29, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsCrusas), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsGunnar), null, null)
				.NpcLine(textObject30, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsGunnar), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsCrusas), null, null)
				.NpcLine(textObject31, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsGunnar), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), null, null)
				.NpcLine(textObject32, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsGunnar), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), null, null)
				.NpcLine(textObject33, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsGunnar), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsCrusas), null, null)
				.NpcLine(textObject34, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsCrusas), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsGunnar), null, null)
				.BeginPlayerOptions(null, false)
				.PlayerOption(textObject35, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsCrusas), null, null)
				.Consequence(delegate
				{
					this._selectedOption = 1;
				})
				.GotoDialogState(text7)
				.PlayerOption(textObject36, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsCrusas), null, null)
				.Consequence(delegate
				{
					this._selectedOption = 2;
				})
				.GotoDialogState(text7)
				.EndPlayerOptions()
				.NpcLine(textObject37, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsBjolgur), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), text7, null)
				.CloseDialog(), this);
		}

		// Token: 0x06000312 RID: 786 RVA: 0x000172A5 File Offset: 0x000154A5
		private void OnPlayerSelectsOption1()
		{
			TraitLevelingHelper.OnIssueSolvedThroughQuest(base.QuestGiver, new Tuple<TraitObject, int>[]
			{
				new Tuple<TraitObject, int>(DefaultTraits.Honor, 50)
			});
			base.AddLog(this.QuestSucceededWithHonorableOptionLogText, false);
			base.CompleteQuestWithSuccess();
		}

		// Token: 0x06000313 RID: 787 RVA: 0x000172DB File Offset: 0x000154DB
		private void OnPlayerSelectsOption2()
		{
			TraitLevelingHelper.OnIssueSolvedThroughQuest(base.QuestGiver, new Tuple<TraitObject, int>[]
			{
				new Tuple<TraitObject, int>(DefaultTraits.Calculating, 50)
			});
			base.AddLog(this.QuestSucceededWithCalculatingOptionLogText, false);
			base.CompleteQuestWithSuccess();
		}

		// Token: 0x06000314 RID: 788 RVA: 0x00017311 File Offset: 0x00015511
		private bool IsMainHero(IAgent agent)
		{
			return agent.Character == CharacterObject.PlayerCharacter;
		}

		// Token: 0x06000315 RID: 789 RVA: 0x00017320 File Offset: 0x00015520
		private bool IsCrusas(IAgent agent)
		{
			return agent.Character == NavalStorylineData.Prusas.CharacterObject;
		}

		// Token: 0x06000316 RID: 790 RVA: 0x00017334 File Offset: 0x00015534
		private bool IsBjolgur(IAgent agent)
		{
			return agent.Character == NavalStorylineData.Bjolgur.CharacterObject;
		}

		// Token: 0x06000317 RID: 791 RVA: 0x00017348 File Offset: 0x00015548
		private bool IsGunnar(IAgent agent)
		{
			return agent.Character == NavalStorylineData.Gunnar.CharacterObject;
		}

		// Token: 0x06000318 RID: 792 RVA: 0x0001735C File Offset: 0x0001555C
		private bool MultiAgentConversationCondition()
		{
			if (Hero.OneToOneConversationHero == NavalStorylineData.Prusas && MobileParty.MainParty.IsCurrentlyAtSea && Mission.Current != null)
			{
				Agent agent = this.SpawnBjolgur();
				Agent agent2 = this.SpawnGunnar();
				Campaign.Current.ConversationManager.AddConversationAgents(new List<Agent> { agent, agent2 }, true);
				return true;
			}
			return false;
		}

		// Token: 0x06000319 RID: 793 RVA: 0x000173BC File Offset: 0x000155BC
		private Agent SpawnBjolgur()
		{
			AgentBuildData agentBuildData = new AgentBuildData(NavalStorylineData.Bjolgur.CharacterObject);
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

		// Token: 0x0600031A RID: 794 RVA: 0x00017454 File Offset: 0x00015654
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

		// Token: 0x0600031B RID: 795 RVA: 0x000174EC File Offset: 0x000156EC
		private void StartBattle(bool startFromCheckpoint)
		{
			this._battleWon = false;
			this._hasRanMissionBefore = true;
			if (Hero.MainHero.IsWounded)
			{
				Hero.MainHero.Heal(Hero.MainHero.WoundedHealthLimit - Hero.MainHero.HitPoints + 1, false);
			}
			PlayerEncounter.Finish(true);
			PlayerEncounter.Start();
			PlayerEncounter.Current.SetupFields(PartyBase.MainParty, this._bossCorsairParty.Party);
			PlayerEncounter.StartBattle();
			MissionInitializerRecord navalMissionInitializerTemplate = NavalStorylineData.GetNavalMissionInitializerTemplate("naval_storyline_act_3_quest_4");
			TerrainType faceTerrainType = Campaign.Current.MapSceneWrapper.GetFaceTerrainType(MobileParty.MainParty.CurrentNavigationFace);
			navalMissionInitializerTemplate.TerrainType = faceTerrainType;
			navalMissionInitializerTemplate.NeedsRandomTerrain = false;
			navalMissionInitializerTemplate.PlayingInCampaignMode = true;
			navalMissionInitializerTemplate.RandomTerrainSeed = MBRandom.RandomInt(10000);
			navalMissionInitializerTemplate.AtmosphereOnCampaign = Campaign.Current.Models.MapWeatherModel.GetAtmosphereModel(MobileParty.MainParty.Position);
			navalMissionInitializerTemplate.SceneHasMapPatch = false;
			NavalMissions.OpenFloatingFortressSetPieceBattleMission(navalMissionInitializerTemplate, startFromCheckpoint);
		}

		// Token: 0x0600031C RID: 796 RVA: 0x000175E4 File Offset: 0x000157E4
		private void SpawnMainCorsairParty()
		{
			NavalStorylineData.Prusas.ChangeState(1);
			PartyTemplateObject @object = Campaign.Current.ObjectManager.GetObject<PartyTemplateObject>("storyline_act3_quest_4_boss_corsair_template");
			this._bossCorsairParty = BanditPartyComponent.CreateLooterParty("naval_corsair_boss", Clan.BanditFactions.FirstOrDefault<Clan>((Clan x) => x.StringId == "southern_pirates"), NavalStorylineData.Act3Quest2TargetSettlement, false, @object, this._corsairSpawnPosition);
			MobilePartyHelper.FillPartyManuallyAfterCreation(this._bossCorsairParty, @object, @object.GetUpperTroopLimit());
			foreach (ShipTemplateStack shipTemplateStack in @object.ShipHulls)
			{
				for (int i = 0; i < shipTemplateStack.MaxValue; i++)
				{
					new Ship(shipTemplateStack.ShipHull).Owner = this._bossCorsairParty.Party;
				}
			}
			TextObject textObject = GameTexts.FindText("str_lord_party_name", null);
			TextObjectExtensions.SetCharacterProperties(textObject, "TROOP", NavalStorylineData.Prusas.CharacterObject, false);
			this._bossCorsairParty.Party.SetCustomName(textObject);
			this._bossCorsairParty.Party.SetCustomBanner(NavalStorylineData.CorsairBanner);
			this._bossCorsairParty.IsInfoHidden = true;
			this.SetupCorsairParty(this._bossCorsairParty);
		}

		// Token: 0x0600031D RID: 797 RVA: 0x00017738 File Offset: 0x00015938
		private void SetupCorsairParty(MobileParty corsairParty)
		{
			corsairParty.SetPartyUsedByQuest(true);
			base.AddTrackedObject(corsairParty);
			corsairParty.IsCurrentlyAtSea = true;
			corsairParty.IsVisible = MobileParty.MainParty.Position.Distance(corsairParty.Position) <= MobileParty.MainParty.SeeingRange;
			foreach (Ship ship in corsairParty.Ships)
			{
				ship.IsInvulnerable = true;
			}
			corsairParty.Ai.SetDoNotMakeNewDecisions(true);
			corsairParty.Ai.DisableForHours(3);
			corsairParty.IgnoreByOtherPartiesTill(CampaignTime.Never);
			corsairParty.Party.SetVisualAsDirty();
		}

		// Token: 0x0600031E RID: 798 RVA: 0x000177FC File Offset: 0x000159FC
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

		// Token: 0x0600031F RID: 799 RVA: 0x00017880 File Offset: 0x00015A80
		private void AddGameMenus()
		{
			base.AddGameMenu("naval_storyline_act_3_quest_4_encounter_menu", new TextObject("{=KBe6oPWy}You see the silhouette of a larger ship on the horizon, but its details are hard to make out. At first, you attribute this to the shimmering heat coming off of the sea, but as you close you can see that it is not one ship but several lashed together.\n\nSuddenly a flaming ball arcs out of the cluster of ships, tracing a line of smoke in the sky, before impacting a few arrow-shots from your prow and scattering fire across the water.", null), new OnInitDelegate(this.naval_storyline_act_3_quest_4_encounter_menu_on_init), 0, 0);
			base.AddGameMenuOption("naval_storyline_act_3_quest_4_encounter_menu", "naval_storyline_act_3_quest_4_encounter_menu_continue_option", new TextObject("{=DM6luo3c}Continue", null), new GameMenuOption.OnConditionDelegate(this.naval_storyline_act_3_quest_4_encounter_menu_continue_option_on_condition), new GameMenuOption.OnConsequenceDelegate(this.naval_storyline_act_3_quest_4_encounter_menu_continue_option_on_consequence), false, -1);
			base.AddGameMenu("naval_storyline_act_3_quest_4_encounter_retry", new TextObject("{=etH1IHNZ}You manage to put some distance between you and your enemies, and you have a moment to consider how to proceed.", null), null, 0, 0);
			base.AddGameMenuOption("naval_storyline_act_3_quest_4_encounter_retry", "naval_storyline_act_3_quest_4_encounter_retry_continue", new TextObject("{=YHMDy3lQ}Try again", null), new GameMenuOption.OnConditionDelegate(this.game_menu_encounter_retry_attack_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_encounter_retry_attack_on_consequence), false, -1);
			base.AddGameMenuOption("naval_storyline_act_3_quest_4_encounter_retry", "naval_storyline_act_3_quest_4_encounter_retry_continue_from_checkpoint", new TextObject("{=rHlzkNFL}Try again from checkpoint", null), new GameMenuOption.OnConditionDelegate(this.game_menu_encounter_retry_continue_from_checkpoint_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_encounter_retry_continue_from_checkpoint_on_consequence), false, -1);
			base.AddGameMenuOption("naval_storyline_act_3_quest_4_encounter_retry", "naval_storyline_act_3_quest_4_encounter_retry_leave", new TextObject("{=3sRdGQou}Leave", null), new GameMenuOption.OnConditionDelegate(this.game_menu_encounter_retry_leave_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_encounter_retry_leave_on_consequence), true, -1);
		}

		// Token: 0x06000320 RID: 800 RVA: 0x0001799E File Offset: 0x00015B9E
		private bool game_menu_encounter_retry_attack_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 1;
			return true;
		}

		// Token: 0x06000321 RID: 801 RVA: 0x000179A8 File Offset: 0x00015BA8
		private void game_menu_encounter_retry_attack_on_consequence(MenuCallbackArgs args)
		{
			CharacterObject.PlayerCharacter.HeroObject.Heal(CharacterObject.PlayerCharacter.HeroObject.MaxHitPoints, false);
			this.StartBattle(false);
		}

		// Token: 0x06000322 RID: 802 RVA: 0x000179D0 File Offset: 0x00015BD0
		private bool game_menu_encounter_retry_continue_from_checkpoint_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 1;
			return this._checkpointReached;
		}

		// Token: 0x06000323 RID: 803 RVA: 0x000179DF File Offset: 0x00015BDF
		private void game_menu_encounter_retry_continue_from_checkpoint_on_consequence(MenuCallbackArgs args)
		{
			CharacterObject.PlayerCharacter.HeroObject.Heal(CharacterObject.PlayerCharacter.HeroObject.MaxHitPoints, false);
			this.StartBattle(true);
		}

		// Token: 0x06000324 RID: 804 RVA: 0x00017A07 File Offset: 0x00015C07
		private bool game_menu_encounter_retry_leave_on_condition(MenuCallbackArgs args)
		{
			args.Tooltip = new TextObject("{=wmTjX28f}This will exit story mode and return you to the Sandbox. You can continue the storyline later by talking to Gunnar in the port again.", null);
			args.optionLeaveType = 16;
			return true;
		}

		// Token: 0x06000325 RID: 805 RVA: 0x00017A23 File Offset: 0x00015C23
		private void game_menu_encounter_retry_leave_on_consequence(MenuCallbackArgs args)
		{
			base.CompleteQuestWithCancel(null);
			NavalStorylineData.DeactivateNavalStoryline();
		}

		// Token: 0x06000326 RID: 806 RVA: 0x00017A34 File Offset: 0x00015C34
		private void naval_storyline_act_3_quest_4_encounter_menu_on_init(MenuCallbackArgs args)
		{
			if (this._shouldRunMission)
			{
				this._shouldRunMission = false;
				this.StartBattle(false);
				return;
			}
			if (this._battleWon)
			{
				PlayerEncounter.Finish(true);
				NavalStorylineData.Prusas.SetHasMet();
				ConversationCharacterData conversationCharacterData = new ConversationCharacterData(CharacterObject.PlayerCharacter, PartyBase.MainParty, true, false, false, false, false, true);
				ConversationCharacterData conversationCharacterData2;
				conversationCharacterData2..ctor(NavalStorylineData.Prusas.CharacterObject, null, true, true, true, false, false, true);
				CampaignMission.OpenConversationMission(conversationCharacterData, conversationCharacterData2, "conversation_scene_sea_multi_agent", "", true);
				return;
			}
			if (this._hasRanMissionBefore)
			{
				GameMenu.SwitchToMenu("naval_storyline_act_3_quest_4_encounter_retry");
			}
		}

		// Token: 0x06000327 RID: 807 RVA: 0x00017AC2 File Offset: 0x00015CC2
		private bool naval_storyline_act_3_quest_4_encounter_menu_continue_option_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 1;
			return true;
		}

		// Token: 0x06000328 RID: 808 RVA: 0x00017ACC File Offset: 0x00015CCC
		private void naval_storyline_act_3_quest_4_encounter_menu_continue_option_on_consequence(MenuCallbackArgs args)
		{
			ConversationCharacterData conversationCharacterData = new ConversationCharacterData(CharacterObject.PlayerCharacter, PartyBase.MainParty, true, false, false, false, false, false);
			ConversationCharacterData conversationCharacterData2;
			conversationCharacterData2..ctor(NavalStorylineData.Bjolgur.CharacterObject, PartyBase.MainParty, true, false, false, false, false, false);
			CampaignMission.OpenConversationMission(conversationCharacterData, conversationCharacterData2, "", "", false);
		}

		// Token: 0x06000329 RID: 809 RVA: 0x00017B1C File Offset: 0x00015D1C
		[GameMenuInitializationHandler("naval_storyline_act_3_quest_4_encounter_menu")]
		[GameMenuInitializationHandler("naval_storyline_act_3_quest_4_encounter_retry")]
		private static void quest_game_menus_on_init_background(MenuCallbackArgs args)
		{
			args.MenuContext.SetBackgroundMeshName(SettlementHelper.FindNearestHideoutToMobileParty(MobileParty.MainParty, 3, null).WaitMeshName);
		}

		// Token: 0x0600032A RID: 810 RVA: 0x00017B3A File Offset: 0x00015D3A
		public bool IsCrusasVisible()
		{
			return this._bossCorsairParty != null && this._bossCorsairParty.IsActive && this._bossCorsairParty.IsVisible;
		}

		// Token: 0x040001F5 RID: 501
		private const int NumberOfCorsairParties = 2;

		// Token: 0x040001F6 RID: 502
		private const int CalculatingBonusAmount = 50;

		// Token: 0x040001F7 RID: 503
		private const int HonorBonusAmount = 50;

		// Token: 0x040001F8 RID: 504
		private const int CorsairShipAiDisableTimeAsHours = 3;

		// Token: 0x040001F9 RID: 505
		[SaveableField(1)]
		private List<MobileParty> _corsairParties;

		// Token: 0x040001FA RID: 506
		[SaveableField(2)]
		private JournalLog _playerStartsQuestLog;

		// Token: 0x040001FB RID: 507
		[SaveableField(3)]
		private CampaignVec2 _corsairSpawnPosition;

		// Token: 0x040001FC RID: 508
		[SaveableField(4)]
		private int _numberOfDefeatedCorsairParties;

		// Token: 0x040001FD RID: 509
		[SaveableField(5)]
		private MobileParty _bossCorsairParty;

		// Token: 0x040001FE RID: 510
		[SaveableField(6)]
		private bool _battleWon;

		// Token: 0x040001FF RID: 511
		[SaveableField(7)]
		private bool _willProgressStoryline;

		// Token: 0x04000200 RID: 512
		[SaveableField(8)]
		private int _selectedOption;

		// Token: 0x04000201 RID: 513
		[SaveableField(9)]
		private bool _checkpointReached;

		// Token: 0x04000202 RID: 514
		[SaveableField(10)]
		private bool _hasRanMissionBefore;

		// Token: 0x04000203 RID: 515
		private bool _shouldRunMission;

		// Token: 0x04000204 RID: 516
		private const string Act3Quest4CorsairPartyTemplateStringId = "storyline_act3_quest_4_corsair_generic_template";

		// Token: 0x04000205 RID: 517
		private const string Act3Quest4BossCorsairPartyTemplateStringId = "storyline_act3_quest_4_boss_corsair_template";
	}
}
