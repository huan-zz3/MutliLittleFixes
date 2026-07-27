using System;
using System.Collections.Generic;
using Helpers;
using NavalDLC.Storyline.Quests;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Storyline.CampaignBehaviors
{
	// Token: 0x02000077 RID: 119
	public class NavalStorylineThirdActFirstQuestBehavior : CampaignBehaviorBase
	{
		// Token: 0x17000167 RID: 359
		// (get) Token: 0x0600087E RID: 2174 RVA: 0x0003BE4C File Offset: 0x0003A04C
		private static SetSailAndEscortTheFortuneSeekersQuest Instance
		{
			get
			{
				NavalStorylineThirdActFirstQuestBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<NavalStorylineThirdActFirstQuestBehavior>();
				if (campaignBehavior._cachedQuest != null && campaignBehavior._cachedQuest.IsOngoing)
				{
					return campaignBehavior._cachedQuest;
				}
				using (List<QuestBase>.Enumerator enumerator = Campaign.Current.QuestManager.Quests.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						SetSailAndEscortTheFortuneSeekersQuest setSailAndEscortTheFortuneSeekersQuest;
						if ((setSailAndEscortTheFortuneSeekersQuest = enumerator.Current as SetSailAndEscortTheFortuneSeekersQuest) != null)
						{
							campaignBehavior._cachedQuest = setSailAndEscortTheFortuneSeekersQuest;
							return campaignBehavior._cachedQuest;
						}
					}
				}
				return null;
			}
		}

		// Token: 0x0600087F RID: 2175 RVA: 0x0003BEE4 File Offset: 0x0003A0E4
		public override void RegisterEvents()
		{
			if (!NavalStorylineData.IsNavalStorylineCanceled())
			{
				CampaignEvents.OnAfterSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnAfterSessionLaunched));
				NavalDLCEvents.OnNavalStorylineCanceledEvent.AddNonSerializedListener(this, new Action<NavalStorylineData.StorylineCancelDetail>(this.OnNavalStorylineCanceled));
				CampaignEvents.OnQuestStartedEvent.AddNonSerializedListener(this, new Action<QuestBase>(this.OnQuestStarted));
			}
		}

		// Token: 0x06000880 RID: 2176 RVA: 0x0003BF3D File Offset: 0x0003A13D
		private void OnQuestStarted(QuestBase quest)
		{
			if (quest is SetSailAndEscortTheFortuneSeekersQuest)
			{
				this._merchantsFaction = NavalStorylineData.HomeSettlement.OwnerClan;
			}
		}

		// Token: 0x06000881 RID: 2177 RVA: 0x0003BF57 File Offset: 0x0003A157
		private void OnNavalStorylineCanceled(NavalStorylineData.StorylineCancelDetail detail)
		{
			CampaignEventDispatcher.Instance.RemoveListeners(this);
		}

		// Token: 0x06000882 RID: 2178 RVA: 0x0003BF64 File Offset: 0x0003A164
		private void OnAfterSessionLaunched(CampaignGameStarter campaignGameStarter)
		{
			this.AddGunnarInitialDialogFlow();
			this.AddMerchantsDialogueFlow(campaignGameStarter);
			this.AddGameMenus(campaignGameStarter);
		}

		// Token: 0x06000883 RID: 2179 RVA: 0x0003BF7A File Offset: 0x0003A17A
		private void AddGameMenus(CampaignGameStarter gameStarter)
		{
			gameStarter.AddGameMenu("naval_storyline_act_3_quest_1_conversation_menu", string.Empty, new OnInitDelegate(this.naval_storyline_act_3_quest_1_conversation_menu_on_init), 0, 0, null);
		}

		// Token: 0x06000884 RID: 2180 RVA: 0x0003BF9B File Offset: 0x0003A19B
		private void naval_storyline_act_3_quest_1_conversation_menu_on_init(MenuCallbackArgs args)
		{
			if (this._isQuestAcceptedThroughMission && Mission.Current == null)
			{
				this.OnPlayerAgreedToHelp();
				this._isQuestAcceptedThroughMission = false;
			}
		}

		// Token: 0x06000885 RID: 2181 RVA: 0x0003BFBC File Offset: 0x0003A1BC
		private void AddGunnarInitialDialogFlow()
		{
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1200).NpcLine("{=HTEIIesY}Greetings. Listen… When we sailed with Purig, I was hoping that he would help me fight the Sea Hounds near Hvalvik. His betrayal of course has cost us time, but I think that plan is still a good one.", null, null, null, null).Condition(() => this.IsQuest1ReadyToStart() && !NavalStorylineData.IsTutorialSkipped() && !this._isIntroGiven)
				.NpcLine("{=zYEWPvl2}That captive we took, Hralgar, said that the Sea Hounds expect to find rich pickings near Beinland. I think I know what he is talking about. Every year, a Vlandian merchant ship travels to the far north, bearing hunters and other fortune-seekers. It should be returning south around this time. These men have spent the last months gathering walrus ivory, fur and whale oil, all of which are quite valuable in the southlands.", null, null, null, null)
				.NpcLine("{=Tn5mFdcU}Such a prize would be a great boon to the Sea Hounds. I propose that we deny it to them. We can sail to Hvalvik, meet this merchant, and escort them south, sinking or taking any Sea Hounds we encounter.", null, null, null, null)
				.NpcLine("{=DRRUMKFN}Our longship is ready. If you can join me, then we should set out as soon as you are ready.", null, null, null, null)
				.NpcLine("{=xngacVnQ}One thing - it is hard to revictual at sea, so do make sure we have plenty of supplies with us to go to Hvalvik and back. Twenty loads of grain and meat, or the equivalent, should be sufficient for our voyage.", null, null, null, null)
				.Consequence(delegate
				{
					this._isIntroGiven = true;
				})
				.BeginPlayerOptions(null, false)
				.PlayerOption("{=SdwdyDGN}I am ready to sail.", null, null, null)
				.NpcLine("{=bhUo9L89}Splendid. The tide and winds are with us. Let us go forth!", null, null, null, null)
				.Consequence(delegate
				{
					if (Mission.Current == null)
					{
						Campaign.Current.ConversationManager.ConversationEndOneShot += this.OnPlayerAgreedToHelp;
						return;
					}
					Campaign.Current.ConversationManager.ConversationEndOneShot += this.OnPlayerAcceptsQuestThroughMission;
				})
				.CloseDialog()
				.PlayerOption("{=k07wzat8}I am not ready yet.", null, null, null)
				.Consequence(delegate
				{
					Campaign.Current.ConversationManager.ConversationEndOneShot += NavalStorylineData.OnPlayerPostponedQuestStart;
				})
				.NpcLine("{=mw07yfTt}Very well. We can wait here for a bit longer for you.", null, null, null, null)
				.CloseDialog()
				.PlayerOption("{=aEKNUI45}This war on the Sea Hounds is too risky. There must be another way to get my sister back.", null, null, null)
				.GotoDialogState("gunnar_ransom_sister")
				.EndPlayerOptions(), null);
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1200).NpcLine("{=NSHm5s2u}{PLAYER.NAME}... It's good to see you again! Have you reconsidered joining me in my little feud? I cannot promise you that we will find your sister, but I believe the odds have increased.", null, null, null, null).Condition(delegate
			{
				StringHelpers.SetCharacterProperties("PLAYER", Hero.MainHero.CharacterObject, null, false);
				return this.IsQuest1ReadyToStart() && NavalStorylineData.IsTutorialSkipped() && !this._isIntroGiven;
			})
				.NpcLine("{=XDr67yKI}When last we met, I was intending to sail with my old friend Purig. Well, I always fancied myself a good judge of character, but I suppose fond memories of my warlike youth went to my head like ale. Purig betrayed me. Like so many of my comrades from those days, he turned Sea Hound. I escaped his clutches however, and returned here. I know a great deal more about their operations.", null, null, null, null)
				.NpcLine("{=zYguiNhG}Anyway, I had originally wanted to join up with a merchant returning to Vlandia from Hvalvik, and I think that plan is still a good one. His company has spent the last months hunting and whaling in the far north, and their ship is laden with valuables. I am certain that the Sea Hounds will be unable to resist such a tempting target.", null, null, null, null)
				.Consequence(delegate
				{
					this._isIntroGiven = true;
				})
				.BeginPlayerOptions(null, false)
				.PlayerOption("{=SdwdyDGN}I am ready to sail.", null, null, null)
				.NpcLine("{=bhUo9L89}Splendid. The tide and winds are with us. Let us go forth!", null, null, null, null)
				.Consequence(delegate
				{
					if (Mission.Current == null)
					{
						Campaign.Current.ConversationManager.ConversationEndOneShot += this.OnPlayerAgreedToHelp;
						return;
					}
					Campaign.Current.ConversationManager.ConversationEndOneShot += this.OnPlayerAcceptsQuestThroughMission;
				})
				.CloseDialog()
				.PlayerOption("{=k07wzat8}I am not ready yet.", null, null, null)
				.Consequence(delegate
				{
					Campaign.Current.ConversationManager.ConversationEndOneShot += NavalStorylineData.OnPlayerPostponedQuestStart;
				})
				.NpcLine("{=mw07yfTt}Very well. We can wait here for a bit longer for you.", null, null, null, null)
				.CloseDialog()
				.PlayerOption("{=aEKNUI45}This war on the Sea Hounds is too risky. There must be another way to get my sister back.", null, null, null)
				.GotoDialogState("gunnar_ransom_sister")
				.EndPlayerOptions(), null);
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1500).NpcLine("{=yJIP3tpk}Are we ready to sail to Hvalvik to escort those Vlandian merchants? They will wait as long as they can, but they cannot wait forever.", null, null, null, null).Condition(() => this.IsQuest1ReadyToStart() && this._isIntroGiven)
				.BeginPlayerOptions(null, false)
				.PlayerOption("{=qcYkbX2a}Let us sail.", null, null, null)
				.Consequence(delegate
				{
					if (Mission.Current == null)
					{
						Campaign.Current.ConversationManager.ConversationEndOneShot += this.OnPlayerAgreedToHelp;
						return;
					}
					Campaign.Current.ConversationManager.ConversationEndOneShot += this.OnPlayerAcceptsQuestThroughMission;
				})
				.CloseDialog()
				.PlayerOption("{=yCTF6YvP}I still need more time.", null, null, null)
				.Consequence(delegate
				{
					Campaign.Current.ConversationManager.ConversationEndOneShot += NavalStorylineData.OnPlayerPostponedQuestStart;
				})
				.CloseDialog()
				.PlayerOption("{=aEKNUI45}This war on the Sea Hounds is too risky. There must be another way to get my sister back.", null, null, null)
				.GotoDialogState("gunnar_ransom_sister")
				.EndPlayerOptions(), null);
		}

		// Token: 0x06000886 RID: 2182 RVA: 0x0003C2A8 File Offset: 0x0003A4A8
		private void AddMerchantsDialogueFlow(CampaignGameStarter campaignGameStarter)
		{
			campaignGameStarter.AddDialogLine("merchant_meeting_dialogue", "start", "merchant_meeting_player_options_1", "{=lV2EbD7b}Ahoy! Who are you, and what's your purpose?", new ConversationSentence.OnConditionDelegate(this.merchant_meeting_dialogue_on_condition), null, 50000, null);
			campaignGameStarter.AddPlayerLine("merchant_meeting_dialogue_player_options1_1", "merchant_meeting_player_options_1", "merchant_meeting_npc_answer", "{=zjDk0evO}We're here to escort you, if you'll have us.", null, null, 100, null, null);
			campaignGameStarter.AddPlayerLine("merchant_meeting_dialogue_player_options1_2", "merchant_meeting_player_options_1", "merchant_meeting_npc_answer", "{=1EkgbhaB}We're here making war upon the Sea Hounds, a pirate confederation.", null, null, 100, null, null);
			campaignGameStarter.AddDialogLine("merchant_meeting_npc_answer_line", "merchant_meeting_npc_answer", "merchant_meeting_player_options_2", "{=MlLDWXuR}If that's the case then we're glad to have you around. Back in Hvalvik port, we heard rumors of these pirates, and we were none too pleased that we had to venture out alone like this. Tell me then, are you asking anything for your services?", null, null, 100, null);
			campaignGameStarter.AddPlayerLine("merchant_meeting_dialogue_player_options2_1", "merchant_meeting_player_options_2", "merchant_meeting_npc_answer_2", "{=ZFONiAA3}A small share of your cargo would be customary.", null, null, 100, null, null);
			campaignGameStarter.AddPlayerLine("merchant_meeting_dialogue_player_options2_2", "merchant_meeting_player_options_2", "merchant_meeting_npc_answer_2", "{=ens8bc7I}Merely a chance to fight those slaving bastards.", null, null, 100, null, null);
			campaignGameStarter.AddDialogLine("merchant_meeting_npc_answer_2_line", "merchant_meeting_npc_answer_2", "close_window", "{=tH5wQo81}Very well. Should we arrive safely, we will happily show our gratitude with a contribution to your cause. The wind is brisk and the waves are choppy, so try not to venture too far away… May the Heavens protect us from pirates and the perils of the sea.", null, delegate
			{
				Campaign.Current.ConversationManager.ConversationEndOneShot += this.OnMerchantConversationEnded;
			}, 100, null);
		}

		// Token: 0x06000887 RID: 2183 RVA: 0x0003C3B2 File Offset: 0x0003A5B2
		private bool merchant_meeting_dialogue_on_condition()
		{
			return NavalStorylineThirdActFirstQuestBehavior.Instance != null && !NavalStorylineThirdActFirstQuestBehavior.Instance.HasMetMerchants && !NavalStorylineThirdActFirstQuestBehavior.Instance.HasSavedMerchants && NavalStorylineThirdActFirstQuestBehavior.Instance.IsConversationHeroTheMerchant;
		}

		// Token: 0x06000888 RID: 2184 RVA: 0x0003C3DF File Offset: 0x0003A5DF
		private void OnMerchantConversationEnded()
		{
			NavalStorylineThirdActFirstQuestBehavior.Instance.OnMerchantsMet();
			PlayerEncounter.Finish(true);
		}

		// Token: 0x06000889 RID: 2185 RVA: 0x0003C3F4 File Offset: 0x0003A5F4
		private bool IsQuest1ReadyToStart()
		{
			return NavalStorylineData.IsStorylineActivationPossible() && NavalStorylineData.HasCompletedLast(NavalStorylineData.NavalStorylineStage.Act2) && Settlement.CurrentSettlement == NavalStorylineData.HomeSettlement && Hero.OneToOneConversationHero == NavalStorylineData.Gunnar && !Campaign.Current.QuestManager.IsThereActiveQuestWithType(typeof(SetSailAndMeetTheFortuneSeekersInTargetSettlementQuest)) && !Campaign.Current.QuestManager.IsThereActiveQuestWithType(typeof(SetSailAndEscortTheFortuneSeekersQuest));
		}

		// Token: 0x0600088A RID: 2186 RVA: 0x0003C461 File Offset: 0x0003A661
		private void OnPlayerAcceptsQuestThroughMission()
		{
			this._isQuestAcceptedThroughMission = true;
			GameMenu.ActivateGameMenu("naval_storyline_act_3_quest_1_conversation_menu");
			Mission.Current.EndMission();
		}

		// Token: 0x0600088B RID: 2187 RVA: 0x0003C47E File Offset: 0x0003A67E
		private void OnPlayerAgreedToHelp()
		{
			new SetSailAndMeetTheFortuneSeekersInTargetSettlementQuest("naval_storyline_act3_quest1_1", NavalStorylineData.Gunnar, NavalStorylineData.Act3Quest1TargetSettlement).StartQuest();
		}

		// Token: 0x0600088C RID: 2188 RVA: 0x0003C499 File Offset: 0x0003A699
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<bool>("_isIntroGiven", ref this._isIntroGiven);
			dataStore.SyncData<IFaction>("_merchantsFaction", ref this._merchantsFaction);
		}

		// Token: 0x0400050A RID: 1290
		private const string _questConversationMenuId = "naval_storyline_act_3_quest_1_conversation_menu";

		// Token: 0x0400050B RID: 1291
		private bool _isIntroGiven;

		// Token: 0x0400050C RID: 1292
		private bool _isQuestAcceptedThroughMission;

		// Token: 0x0400050D RID: 1293
		private SetSailAndEscortTheFortuneSeekersQuest _cachedQuest;

		// Token: 0x0400050E RID: 1294
		private IFaction _merchantsFaction;
	}
}
