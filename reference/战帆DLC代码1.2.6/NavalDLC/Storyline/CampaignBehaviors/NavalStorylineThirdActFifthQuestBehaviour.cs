using System;
using System.Linq;
using Helpers;
using NavalDLC.Storyline.MissionControllers;
using NavalDLC.Storyline.Quests;
using StoryMode;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace NavalDLC.Storyline.CampaignBehaviors
{
	// Token: 0x02000076 RID: 118
	public class NavalStorylineThirdActFifthQuestBehaviour : CampaignBehaviorBase
	{
		// Token: 0x0600085F RID: 2143 RVA: 0x0003B16C File Offset: 0x0003936C
		public override void RegisterEvents()
		{
			if (!NavalStorylineData.IsNavalStorylineCanceled())
			{
				CampaignEvents.OnAfterSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnAfterSessionLaunched));
				CampaignEvents.OnQuestCompletedEvent.AddNonSerializedListener(this, new Action<QuestBase, QuestBase.QuestCompleteDetails>(this.OnQuestCompleted));
				CampaignEvents.GameMenuOpened.AddNonSerializedListener(this, new Action<MenuCallbackArgs>(this.OnGameMenuOpened));
				CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener(this, new Action(this.OnGameLoadFinished));
			}
		}

		// Token: 0x06000860 RID: 2144 RVA: 0x0003B1DC File Offset: 0x000393DC
		private void OnGameLoadFinished()
		{
			NavalStorylineData.NavalStorylineStage storylineStage = NavalStorylineData.GetStorylineStage();
			if (storylineStage == NavalStorylineData.NavalStorylineStage.Act3Quest4 && Campaign.Current.QuestManager.IsThereActiveQuestWithType(typeof(FreeTheSeaHoundsCaptivesQuest)))
			{
				this._navalStorylineFinalQuestState = NavalStorylineThirdActFifthQuestBehaviour.NavalStorylineFinalQuestState.Quest5IsInProgress;
			}
			else if (storylineStage == NavalStorylineData.NavalStorylineStage.Act3Quest5 && Campaign.Current.QuestManager.IsThereActiveQuestWithType(typeof(ReturnToBaseQuest)))
			{
				this._navalStorylineFinalQuestState = NavalStorylineThirdActFifthQuestBehaviour.NavalStorylineFinalQuestState.SpeakToGunnarAndSister;
			}
			else if (storylineStage >= NavalStorylineData.NavalStorylineStage.Act3Quest5)
			{
				this._navalStorylineFinalQuestState = NavalStorylineThirdActFifthQuestBehaviour.NavalStorylineFinalQuestState.End;
			}
			if (MBSaveLoad.IsUpdatingGameVersion && MBSaveLoad.LastLoadedGameVersion < ApplicationVersion.FromString("v1.3.14", 0) && this._navalStorylineFinalQuestState == NavalStorylineThirdActFifthQuestBehaviour.NavalStorylineFinalQuestState.Quest5IsInProgress && !Campaign.Current.QuestManager.IsThereActiveQuestWithType(typeof(FreeTheSeaHoundsCaptivesQuest)))
			{
				this._navalStorylineFinalQuestState = NavalStorylineThirdActFifthQuestBehaviour.NavalStorylineFinalQuestState.TalkWithGunnarAtPort;
			}
		}

		// Token: 0x06000861 RID: 2145 RVA: 0x0003B294 File Offset: 0x00039494
		private void OnGameMenuOpened(MenuCallbackArgs args)
		{
			if (args.MenuContext.GameMenu.StringId == "naval_storyline_outside_town" && this._navalStorylineFinalQuestState > NavalStorylineThirdActFifthQuestBehaviour.NavalStorylineFinalQuestState.Quest5IsInProgress)
			{
				GameMenu.SwitchToMenu("naval_storyline_finalize_menu");
			}
			if (this._navalStorylineFinalQuestState <= NavalStorylineThirdActFifthQuestBehaviour.NavalStorylineFinalQuestState.Quest5IsInProgress && NavalStorylineData.IsStorylineActivationPossible() && NavalStorylineData.HasCompletedLast(NavalStorylineData.NavalStorylineStage.Act3Quest4) && !Campaign.Current.QuestManager.IsThereActiveQuestWithType(typeof(FreeTheSeaHoundsCaptivesQuest)) && Settlement.CurrentSettlement == NavalStorylineData.HomeSettlement && !Campaign.Current.VisualTrackerManager.CheckTracked(NavalStorylineData.Gunnar))
			{
				Campaign.Current.VisualTrackerManager.RegisterObject(NavalStorylineData.Gunnar);
			}
		}

		// Token: 0x06000862 RID: 2146 RVA: 0x0003B33C File Offset: 0x0003953C
		private void OnQuestCompleted(QuestBase quest, QuestBase.QuestCompleteDetails detail)
		{
			if (detail == 1 && quest is CaptureTheImperialMerchantPrusas)
			{
				this._navalStorylineFinalQuestState = NavalStorylineThirdActFifthQuestBehaviour.NavalStorylineFinalQuestState.TalkWithGunnarAtPort;
				return;
			}
			if (!(quest is FreeTheSeaHoundsCaptivesQuest))
			{
				if (detail == 1 && quest is SpeakToGunnarAndSisterQuest)
				{
					this._navalStorylineFinalQuestState = NavalStorylineThirdActFifthQuestBehaviour.NavalStorylineFinalQuestState.End;
				}
				return;
			}
			if (detail == 1)
			{
				NavalStorylineData.DeactivateNavalStoryline();
				this._navalStorylineFinalQuestState = NavalStorylineThirdActFifthQuestBehaviour.NavalStorylineFinalQuestState.TalkWithGunnarAfterFight;
				this._bossFightOutCome = ((FreeTheSeaHoundsCaptivesQuest)quest).BossFightOutCome;
				return;
			}
			this._navalStorylineFinalQuestState = NavalStorylineThirdActFifthQuestBehaviour.NavalStorylineFinalQuestState.TalkWithGunnarAtPort;
		}

		// Token: 0x06000863 RID: 2147 RVA: 0x0003B3A2 File Offset: 0x000395A2
		private void OnAfterSessionLaunched(CampaignGameStarter campaignGameStarter)
		{
			if (StoryModeManager.Current != null)
			{
				this.AddDialogs();
				this.AddGameMenus(campaignGameStarter);
			}
		}

		// Token: 0x06000864 RID: 2148 RVA: 0x0003B3B8 File Offset: 0x000395B8
		private void AddDialogs()
		{
			DialogFlow dialogFlow = DialogFlow.CreateDialogFlow("start", 1200).NpcLine(new TextObject("{=jWDBinsb}Well... Here we are. Ready to set sail for Angranfjord and settle accounts with our enemies, once and for all. Lahar will sail with us, and Bjolgur, and more of his brothers may join us at our destination. We have Crusas' ship – and Crusas too of course, much as he might not like it – and hopefully the element of surprise. We just need to consider how to turn this best to our advantage.", null), null, null, null, null).Condition(() => this._navalStorylineFinalQuestState == NavalStorylineThirdActFifthQuestBehaviour.NavalStorylineFinalQuestState.TalkWithGunnarAtPort && this.Quest5ConversationStartCondition())
				.BeginPlayerOptions(null, false)
				.PlayerOption(new TextObject("{=el44RZG4}Let us set out, then.", null), null, null, null)
				.Consequence(delegate
				{
					if (Mission.Current == null)
					{
						Campaign.Current.ConversationManager.ConversationEndOneShot += this.ActivateQuest5;
					}
					else
					{
						Campaign.Current.ConversationManager.ConversationEndOneShot += this.OnPlayerAcceptsQuestThroughMission;
					}
					this._navalStorylineFinalQuestState = NavalStorylineThirdActFifthQuestBehaviour.NavalStorylineFinalQuestState.Quest5IsInProgress;
				})
				.CloseDialog()
				.PlayerOption(new TextObject("{=a0j86F9C}I need a bit more time.", null), null, null, null)
				.Consequence(delegate
				{
					this._navalStorylineFinalQuestState = NavalStorylineThirdActFifthQuestBehaviour.NavalStorylineFinalQuestState.GunnarWaitsForAnAnswer;
					Campaign.Current.ConversationManager.ConversationEndOneShot += NavalStorylineData.OnPlayerPostponedQuestStart;
				})
				.CloseDialog()
				.PlayerOption("{=aEKNUI45}This war on the Sea Hounds is too risky. There must be another way to get my sister back.", null, null, null)
				.GotoDialogState("gunnar_ransom_sister")
				.EndPlayerOptions();
			DialogFlow dialogFlow2 = DialogFlow.CreateDialogFlow("start", 1200).NpcLine(new TextObject("{=0Y3S817q}Are you ready to sail to the Angranfjord to carry out our plan? Purig may not be waiting there for much longer.", null), null, null, null, null).Condition(() => this._navalStorylineFinalQuestState == NavalStorylineThirdActFifthQuestBehaviour.NavalStorylineFinalQuestState.GunnarWaitsForAnAnswer && this.Quest5ConversationStartCondition())
				.BeginPlayerOptions(null, false)
				.PlayerOption(new TextObject("{=qcYkbX2a}Let us sail.", null), null, null, null)
				.Consequence(delegate
				{
					if (Mission.Current == null)
					{
						Campaign.Current.ConversationManager.ConversationEndOneShot += this.ActivateQuest5;
					}
					else
					{
						Campaign.Current.ConversationManager.ConversationEndOneShot += this.OnPlayerAcceptsQuestThroughMission;
					}
					this._navalStorylineFinalQuestState = NavalStorylineThirdActFifthQuestBehaviour.NavalStorylineFinalQuestState.Quest5IsInProgress;
				})
				.CloseDialog()
				.PlayerOption(new TextObject("{=4LhjHfSY}I am still not ready.", null), null, null, null)
				.Consequence(delegate
				{
					Campaign.Current.ConversationManager.ConversationEndOneShot += NavalStorylineData.OnPlayerPostponedQuestStart;
				})
				.CloseDialog()
				.PlayerOption("{=aEKNUI45}This war on the Sea Hounds is too risky. There must be another way to get my sister back.", null, null, null)
				.GotoDialogState("gunnar_ransom_sister")
				.EndPlayerOptions();
			TextObject textObject = new TextObject("{=7SzwQ5NK}{PLAYER.NAME}, welcome! I've been entertaining the village with tales of our adventurers. If you're looking for recruits, then I doubt you'll find a more promising batch than the lads of Lagsholfn. You always have a place by my hearth, old friend.", null);
			TextObject textObject2 = new TextObject("{=dV5ai0PF}Well, {PLAYER.NAME}... Alas, you appear to have made some enemies here. I do not know if what they say is true, and at any rate, I will never raise a hand against you. But I do not think it is good for you to stay here just now.", null);
			DialogFlow dialogFlow3 = DialogFlow.CreateDialogFlow("start", 1200).BeginNpcOptions(null, false).NpcOption(textObject, delegate
			{
				if (!this.GunnarNotableConditions())
				{
					return false;
				}
				Settlement currentSettlement = NavalStorylineData.Gunnar.CurrentSettlement;
				if (currentSettlement == null)
				{
					return false;
				}
				Hero owner = currentSettlement.Owner;
				float? num = ((owner != null) ? new float?(owner.GetRelationWithPlayer()) : null);
				float num2 = 0f;
				return (num.GetValueOrDefault() >= num2) & (num != null);
			}, null, null, null, null)
				.GotoDialogState("lord_start")
				.NpcOption(textObject2, delegate
				{
					if (!this.GunnarNotableConditions())
					{
						return false;
					}
					Settlement currentSettlement2 = NavalStorylineData.Gunnar.CurrentSettlement;
					if (currentSettlement2 == null)
					{
						return false;
					}
					Hero owner2 = currentSettlement2.Owner;
					float? num3 = ((owner2 != null) ? new float?(owner2.GetRelationWithPlayer()) : null);
					float num4 = 0f;
					return (num3.GetValueOrDefault() < num4) & (num3 != null);
				}, null, null, null, null)
				.GotoDialogState("lord_start")
				.EndNpcOptions();
			DialogFlow dialogFlow4 = DialogFlow.CreateDialogFlow("start", 1500).NpcLine("{=!}{GUNNAR_FINAL_DIALOG_LINE_1}", null, null, null, null).Condition(delegate
			{
				bool flag = this._navalStorylineFinalQuestState == NavalStorylineThirdActFifthQuestBehaviour.NavalStorylineFinalQuestState.TalkWithGunnarAfterFight && Hero.OneToOneConversationHero == NavalStorylineData.Gunnar;
				if (flag)
				{
					this.DecideGunnarDialogue();
				}
				return flag;
			})
				.NpcLine("{=!}{GUNNAR_FINAL_DIALOG_LINE_2}", null, null, null, null)
				.NpcLine("{=xxxjoDxM}My men, though... I've had a word with them, and some of them have been quite impressed by your leadership. They want to follow you, if you'll have them. And as I mentioned, they prefer to sail on our ship here, the Wave-Steed, so I guess that's yours too, if you'll have it. She'll carry you well, especially in the rough seas of the north.", null, null, null, null)
				.BeginPlayerOptions(null, false)
				.PlayerOption("{=qatVcvrX}I welcome your ship and crew.", null, null, null)
				.Consequence(new ConversationSentence.OnConsequenceDelegate(this.OnPlayerWelcomedGunnarsCrew))
				.GotoDialogState("gunnar_final_dialog_token_1")
				.PlayerOption("{=FaZ1dSuh}I am honored, but I cannot take on your companions.", null, null, null)
				.GotoDialogState("gunnar_final_dialog_token_1")
				.EndPlayerOptions()
				.NpcLine("{=!}{GUNNAR_FINAL_DIALOG_LINE_3}", null, null, "gunnar_final_dialog_token_1", null)
				.BeginPlayerOptions(null, false)
				.PlayerOption("{=uh2W7Jh3}Farewell. Perhaps I will take you up on your reputation.", null, null, null)
				.GotoDialogState("gunnar_final_dialog_token_2")
				.PlayerOption("{=C94hXQp3}Farewell, and good hunting.", null, null, null)
				.GotoDialogState("gunnar_final_dialog_token_2")
				.EndPlayerOptions()
				.NpcLine("{=Vcr7BYxJ}Farewell, {PLAYER.NAME}.", null, null, "gunnar_final_dialog_token_2", null)
				.Consequence(new ConversationSentence.OnConsequenceDelegate(this.GunnarConversationOnConsequence))
				.CloseDialog();
			Campaign.Current.ConversationManager.AddDialogFlow(dialogFlow, null);
			Campaign.Current.ConversationManager.AddDialogFlow(dialogFlow2, null);
			Campaign.Current.ConversationManager.AddDialogFlow(dialogFlow3, null);
			Campaign.Current.ConversationManager.AddDialogFlow(dialogFlow4, null);
		}

		// Token: 0x06000865 RID: 2149 RVA: 0x0003B6F2 File Offset: 0x000398F2
		private void GunnarConversationOnConsequence()
		{
			NavalDLCHelpers.AddSisterToClan();
			this.MakeGunnarNotable();
			this._navalStorylineFinalQuestState = NavalStorylineThirdActFifthQuestBehaviour.NavalStorylineFinalQuestState.End;
		}

		// Token: 0x06000866 RID: 2150 RVA: 0x0003B708 File Offset: 0x00039908
		private void MakeGunnarNotable()
		{
			Village village = Village.All.FirstOrDefault<Village>((Village x) => x.Settlement.StringId == "village_N1_2");
			if (village != null)
			{
				TeleportHeroAction.ApplyImmediateTeleportToSettlement(NavalStorylineData.Gunnar, village.Settlement);
			}
		}

		// Token: 0x06000867 RID: 2151 RVA: 0x0003B752 File Offset: 0x00039952
		private void OnPlayerAcceptsQuestThroughMission()
		{
			this._isQuestAcceptedThroughMission = true;
			this.OpenQuestMenu();
			Mission.Current.EndMission();
		}

		// Token: 0x06000868 RID: 2152 RVA: 0x0003B76B File Offset: 0x0003996B
		private void OpenQuestMenu()
		{
			GameMenu.ActivateGameMenu("naval_storyline_act_3_quest_5_conversation_menu");
		}

		// Token: 0x06000869 RID: 2153 RVA: 0x0003B778 File Offset: 0x00039978
		private void AddGameMenus(CampaignGameStarter starter)
		{
			starter.AddGameMenu("naval_storyline_act_3_quest_5_conversation_menu", string.Empty, new OnInitDelegate(this.naval_storyline_act_3_quest_5_conversation_menu_on_init), 0, 0, null);
			starter.AddGameMenu("naval_storyline_finalize_menu", "{=l1VpTx3x}You have returned to Ostican harbor. Word spreads fast among seafolk, and a trading ship leaving the harbor dips its oars in salute to your victory. As the crews of your ships come ashore, they are clapped on the back by the local fishermen and dock workers and taken to the taverns to drink to the demise of the Sea Hounds.", new OnInitDelegate(this.naval_storyline_finalize_menu_on_init), 0, 0, null);
			starter.AddGameMenuOption("naval_storyline_finalize_menu", "naval_storyline_finalize_menu_continue_option", "{=DM6luo3c}Continue", new GameMenuOption.OnConditionDelegate(this.naval_storyline_finalize_menu_continue_option_on_condition), new GameMenuOption.OnConsequenceDelegate(this.naval_storyline_finalize_menu_continue_option_on_consequence), false, -1, false, null);
		}

		// Token: 0x0600086A RID: 2154 RVA: 0x0003B7F4 File Offset: 0x000399F4
		private void naval_storyline_act_3_quest_5_conversation_menu_on_init(MenuCallbackArgs args)
		{
			if (this._isQuestAcceptedThroughMission && Mission.Current == null)
			{
				this.ActivateQuest5();
				this._isQuestAcceptedThroughMission = false;
			}
		}

		// Token: 0x0600086B RID: 2155 RVA: 0x0003B814 File Offset: 0x00039A14
		private void naval_storyline_finalize_menu_on_init(MenuCallbackArgs args)
		{
			if (this._navalStorylineFinalQuestState == NavalStorylineThirdActFifthQuestBehaviour.NavalStorylineFinalQuestState.TalkWithGunnarAfterFight)
			{
				ConversationCharacterData conversationCharacterData = new ConversationCharacterData(CharacterObject.PlayerCharacter, PartyBase.MainParty, true, true, false, false, false, true);
				ConversationCharacterData conversationCharacterData2;
				conversationCharacterData2..ctor(NavalStorylineData.Gunnar.CharacterObject, PartyBase.MainParty, true, true, false, true, false, true);
				CampaignMission.OpenConversationMission(conversationCharacterData, conversationCharacterData2, "conversation_scene_sea_multi_agent", "", true);
			}
			MapState mapState = Game.Current.GameStateManager.ActiveState as MapState;
			if (mapState != null)
			{
				mapState.Handler.TeleportCameraToMainParty();
			}
			string text = Settlement.CurrentSettlement.Culture.StringId + "_port";
			args.MenuContext.SetBackgroundMeshName(text);
			args.MenuContext.SetAmbientSound("event:/map/ambient/node/settlements/2d/port");
		}

		// Token: 0x0600086C RID: 2156 RVA: 0x0003B8C6 File Offset: 0x00039AC6
		private bool naval_storyline_finalize_menu_continue_option_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 41;
			return true;
		}

		// Token: 0x0600086D RID: 2157 RVA: 0x0003B8D4 File Offset: 0x00039AD4
		private void naval_storyline_finalize_menu_continue_option_on_consequence(MenuCallbackArgs args)
		{
			if (this._navalStorylineFinalQuestState == NavalStorylineThirdActFifthQuestBehaviour.NavalStorylineFinalQuestState.SpeakToGunnarAndSister && !Campaign.Current.QuestManager.IsThereActiveQuestWithType(typeof(SpeakToGunnarAndSisterQuest)))
			{
				new SpeakToGunnarAndSisterQuest(this._bossFightOutCome).StartQuest();
			}
			Settlement settlement = Settlement.CurrentSettlement ?? PlayerEncounter.EncounterSettlement;
			bool flag;
			bool flag2;
			GameMenu.SwitchToMenu(MobileParty.MainParty.HasNavalNavigationCapability ? "naval_town_outside" : Campaign.Current.Models.EncounterGameMenuModel.GetEncounterMenu(PartyBase.MainParty, settlement.Party, ref flag, ref flag2));
		}

		// Token: 0x0600086E RID: 2158 RVA: 0x0003B960 File Offset: 0x00039B60
		private void ActivateQuest5()
		{
			if (!Campaign.Current.QuestManager.IsThereActiveQuestWithType(typeof(FreeTheSeaHoundsCaptivesQuest)))
			{
				Campaign.Current.VisualTrackerManager.RemoveTrackedObject(NavalStorylineData.Gunnar, false);
				new FreeTheSeaHoundsCaptivesQuest("naval_storyline_act3_quest5_1", this._strengthModifier).StartQuest();
				this._navalStorylineFinalQuestState = NavalStorylineThirdActFifthQuestBehaviour.NavalStorylineFinalQuestState.Quest5IsInProgress;
			}
		}

		// Token: 0x0600086F RID: 2159 RVA: 0x0003B9BC File Offset: 0x00039BBC
		private bool Quest5ConversationStartCondition()
		{
			return NavalStorylineData.IsStorylineActivationPossible() && NavalStorylineData.HasCompletedLast(NavalStorylineData.NavalStorylineStage.Act3Quest4) && !Campaign.Current.QuestManager.IsThereActiveQuestWithType(typeof(FreeTheSeaHoundsCaptivesQuest)) && Settlement.CurrentSettlement == NavalStorylineData.HomeSettlement && Hero.OneToOneConversationHero == NavalStorylineData.Gunnar;
		}

		// Token: 0x06000870 RID: 2160 RVA: 0x0003BA0D File Offset: 0x00039C0D
		private bool GunnarNotableConditions()
		{
			StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, null, false);
			return Hero.OneToOneConversationHero == NavalStorylineData.Gunnar && !NavalStorylineData.IsNavalStoryLineActive() && NavalStorylineData.HasCompletedLast(NavalStorylineData.NavalStorylineStage.Act3Quest5);
		}

		// Token: 0x06000871 RID: 2161 RVA: 0x0003BA3C File Offset: 0x00039C3C
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<NavalStorylineThirdActFifthQuestBehaviour.NavalStorylineFinalQuestState>("_navalStorylineFinalQuestState", ref this._navalStorylineFinalQuestState);
			dataStore.SyncData<Quest5SetPieceBattleMissionController.BossFightOutComeEnum>("_bossFightOutCome", ref this._bossFightOutCome);
		}

		// Token: 0x06000872 RID: 2162 RVA: 0x0003BA62 File Offset: 0x00039C62
		public Quest5SetPieceBattleMissionController.BossFightOutComeEnum GetBossFightOutcome()
		{
			return this._bossFightOutCome;
		}

		// Token: 0x06000873 RID: 2163 RVA: 0x0003BA6C File Offset: 0x00039C6C
		private void OnPlayerWelcomedGunnarsCrew()
		{
			Ship ship = new Ship(MBObjectManager.Instance.GetObject<ShipHull>("northern_medium_ship"));
			ship.SetName(new TextObject("{=EUAsSTeT}Wave-Steed", null));
			ChangeShipOwnerAction.ApplyByLooting(PartyBase.MainParty, ship);
			CharacterObject @object = MBObjectManager.Instance.GetObject<CharacterObject>("nord_spear_warrior");
			MobileParty.MainParty.MemberRoster.AddToCounts(@object, 10, false, 0, 0, true, -1);
			CharacterObject object2 = MBObjectManager.Instance.GetObject<CharacterObject>("nord_vargr");
			MobileParty.MainParty.MemberRoster.AddToCounts(object2, 10, false, 0, 0, true, -1);
			if (!MobileParty.MainParty.Anchor.IsValid && Settlement.CurrentSettlement != null && Settlement.CurrentSettlement.HasPort)
			{
				MobileParty.MainParty.Anchor.SetSettlement(Settlement.CurrentSettlement);
			}
			TextObject textObject = new TextObject("{=06sIBlHR}{NUMBER} troops and {SHIP_NAME} were added to your party.", null);
			textObject.SetTextVariable("NUMBER", 20);
			textObject.SetTextVariable("SHIP_NAME", ship.Name);
			InformationManager.DisplayMessage(new InformationMessage(textObject.ToString(), new Color(0f, 1f, 0f, 1f)));
		}

		// Token: 0x06000874 RID: 2164 RVA: 0x0003BB88 File Offset: 0x00039D88
		private void DecideGunnarDialogue()
		{
			TextObject textObject;
			TextObject textObject2;
			if (this._bossFightOutCome == Quest5SetPieceBattleMissionController.BossFightOutComeEnum.PlayerRefusedTheDuel)
			{
				textObject = new TextObject("{=dI8a424b}Well then... Your sister is free, thank the gods. You gave Purig the death he deserved. None will mourn him. And the Sea Hounds... Well, I doubt they'll recover from the thrashing we gave them today. The north will thank you.", null);
				textObject2 = new TextObject("{=UAq8cW8O}Now, I think, I will go ashore, and make my way home. Lagshofn is not far from here. I've settled what I wish to settle, and all this rowing and ramming and climbing and jostling and fighting is hard on my old bones.", null);
			}
			else if (this._bossFightOutCome == Quest5SetPieceBattleMissionController.BossFightOutComeEnum.PlayerAcceptedAndWonTheDuel)
			{
				textObject = new TextObject("{=0TP1KQLE}Well then... Your sister is free, thank the gods. You put an end to the Sea Hounds, and gave Purig a far more honorable death than he deserved. Men will speak well of you.", null);
				textObject2 = new TextObject("{=UAq8cW8O}Now, I think, I will go ashore, and make my way home. Lagshofn is not far from here. I've settled what I wish to settle, and all this rowing and ramming and climbing and jostling and fighting is hard on my old bones.", null);
			}
			else if (this._bossFightOutCome == Quest5SetPieceBattleMissionController.BossFightOutComeEnum.PlayerAcceptedTheDuelLostItAndLetPurigGo)
			{
				textObject = new TextObject("{=XDzsJmMP}Well then...  Your sister is free, thank the gods. Purig may have gotten away, but I doubt the Sea Hounds will be troubling us much more.", null);
				textObject2 = new TextObject("{=dPaN65B1}It was an honorable thing, to duel him, and I am glad you kept your word to him, though he did not deserve it. For my part, though, I owe him nothing. I continue to hunt him, here in Beinland, and as it is much easier for him to evade a large group than a single hunter, I will do so alone.", null);
			}
			else
			{
				textObject = new TextObject("{=8j3z1dBZ}Well then... Your sister is free, thank the gods. Purig is dead, and none will mourn him. I might wish that his death could have come some other way, but I will not dwell on it.", null);
				textObject2 = new TextObject("{=UAq8cW8O}Now, I think, I will go ashore, and make my way home. Lagshofn is not far from here. I've settled what I wish to settle, and all this rowing and ramming and climbing and jostling and fighting is hard on my old bones.", null);
			}
			TextObject textObject3;
			if (this._bossFightOutCome == Quest5SetPieceBattleMissionController.BossFightOutComeEnum.PlayerAcceptedTheDuelLostItAndLetPurigGo)
			{
				textObject3 = new TextObject("{=1PPiv2ns}I suspect Purig will try to travel as far from these parts as possible. Perhaps deep into the south, or to the east... Perhaps I will take years to find him, or perhaps my old age will finally catch up to me on the road or on the seas. I do not know if we will meet again.", null);
			}
			else
			{
				textObject3 = new TextObject("{=IGnbxJHn}You should come see me in my village, Lagshofn, in Beinland. It's not much, not for a {?PLAYER.GENDER}warrior{?}man{\\?} like you, who's no doubt seen all the wonders of the Empire and the lands beyond, but we can pass a summer's night on the beach and drink to our deeds.", null);
			}
			MBTextManager.SetTextVariable("GUNNAR_FINAL_DIALOG_LINE_1", textObject, false);
			MBTextManager.SetTextVariable("GUNNAR_FINAL_DIALOG_LINE_2", textObject2, false);
			MBTextManager.SetTextVariable("GUNNAR_FINAL_DIALOG_LINE_3", textObject3, false);
		}

		// Token: 0x04000500 RID: 1280
		private const string QuestConversationMenuId = "naval_storyline_act_3_quest_5_conversation_menu";

		// Token: 0x04000501 RID: 1281
		private const string GunnarsLongshipStringId = "northern_medium_ship";

		// Token: 0x04000502 RID: 1282
		private const string Tier3NordInfantryStringId = "nord_spear_warrior";

		// Token: 0x04000503 RID: 1283
		private const string Tier4NordInfantryStringId = "nord_vargr";

		// Token: 0x04000504 RID: 1284
		private const int Tier3NordInfantryCount = 10;

		// Token: 0x04000505 RID: 1285
		private const int Tier4NordInfantryCount = 10;

		// Token: 0x04000506 RID: 1286
		private NavalStorylineThirdActFifthQuestBehaviour.NavalStorylineFinalQuestState _navalStorylineFinalQuestState;

		// Token: 0x04000507 RID: 1287
		private Quest5SetPieceBattleMissionController.BossFightOutComeEnum _bossFightOutCome;

		// Token: 0x04000508 RID: 1288
		private bool _isQuestAcceptedThroughMission;

		// Token: 0x04000509 RID: 1289
		private readonly float _strengthModifier = 1f;

		// Token: 0x020001EF RID: 495
		public enum NavalStorylineFinalQuestState
		{
			// Token: 0x04000E24 RID: 3620
			TalkWithGunnarAtPort,
			// Token: 0x04000E25 RID: 3621
			GunnarWaitsForAnAnswer,
			// Token: 0x04000E26 RID: 3622
			Quest5IsInProgress,
			// Token: 0x04000E27 RID: 3623
			TalkWithGunnarAfterFight,
			// Token: 0x04000E28 RID: 3624
			SpeakToGunnarAndSister,
			// Token: 0x04000E29 RID: 3625
			End
		}
	}
}
