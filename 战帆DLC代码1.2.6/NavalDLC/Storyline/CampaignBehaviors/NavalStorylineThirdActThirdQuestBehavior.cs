using System;
using NavalDLC.Storyline.Quests;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Storyline.CampaignBehaviors
{
	// Token: 0x0200007A RID: 122
	public class NavalStorylineThirdActThirdQuestBehavior : CampaignBehaviorBase
	{
		// Token: 0x060008BC RID: 2236 RVA: 0x0003D93E File Offset: 0x0003BB3E
		public override void RegisterEvents()
		{
			if (!NavalStorylineData.IsNavalStorylineCanceled())
			{
				CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
				NavalDLCEvents.OnNavalStorylineCanceledEvent.AddNonSerializedListener(this, new Action<NavalStorylineData.StorylineCancelDetail>(this.OnNavalStorylineCanceled));
			}
		}

		// Token: 0x060008BD RID: 2237 RVA: 0x0003D975 File Offset: 0x0003BB75
		private void OnNavalStorylineCanceled(NavalStorylineData.StorylineCancelDetail detail)
		{
			CampaignEventDispatcher.Instance.RemoveListeners(this);
		}

		// Token: 0x060008BE RID: 2238 RVA: 0x0003D982 File Offset: 0x0003BB82
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<bool>("_isIntroGiven", ref this._isIntroGiven);
		}

		// Token: 0x060008BF RID: 2239 RVA: 0x0003D996 File Offset: 0x0003BB96
		private void OnSessionLaunched(CampaignGameStarter campaignGameSystemStarter)
		{
			this.AddDialogs();
			this.AddGameMenus(campaignGameSystemStarter);
		}

		// Token: 0x060008C0 RID: 2240 RVA: 0x0003D9A5 File Offset: 0x0003BBA5
		private void AddGameMenus(CampaignGameStarter starter)
		{
			starter.AddGameMenu("naval_storyline_act_3_quest_3_conversation_menu", string.Empty, new OnInitDelegate(this.naval_storyline_act_3_quest_3_conversation_menu_on_init), 0, 0, null);
		}

		// Token: 0x060008C1 RID: 2241 RVA: 0x0003D9C6 File Offset: 0x0003BBC6
		private void naval_storyline_act_3_quest_3_conversation_menu_on_init(MenuCallbackArgs args)
		{
			if (this._isQuestAcceptedThroughMission && Mission.Current == null)
			{
				this.StartQuest();
				this._isQuestAcceptedThroughMission = false;
			}
		}

		// Token: 0x060008C2 RID: 2242 RVA: 0x0003D9E4 File Offset: 0x0003BBE4
		private void AddDialogs()
		{
			this.AddGunnarInitialDialogFlow();
		}

		// Token: 0x060008C3 RID: 2243 RVA: 0x0003D9EC File Offset: 0x0003BBEC
		private void AddGunnarInitialDialogFlow()
		{
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1500).NpcLine("{=0xymiaMQ}{PLAYER.NAME}... So… I have been making inquiries into what Fahda told us, about these Vlandian pirates in Purig's employ and their plan to steal the Sturgian silver. Several large warships have been sighted patrolling near {SETTLEMENT_LINK}. I suspect that these are the Vlandians.", null, null, null, null).Condition(delegate
			{
				MBTextManager.SetTextVariable("SETTLEMENT_LINK", NavalStorylineData.Act3Quest3TargetSettlement.EncyclopediaLinkWithName, false);
				return NavalStorylineData.IsStorylineActivationPossible() && NavalStorylineData.HasCompletedLast(NavalStorylineData.NavalStorylineStage.Act3Quest2) && Settlement.CurrentSettlement == NavalStorylineData.HomeSettlement && Hero.OneToOneConversationHero == NavalStorylineData.Gunnar && !Campaign.Current.QuestManager.IsThereActiveQuestWithType(typeof(SpeakToTheSailorsQuest)) && !this._isIntroGiven;
			})
				.NpcLine("{=Jjm2hpCl}{SETTLEMENT_LINK} is linked to the Byalic Sea by a wide estuary. It would be easy for the pirates to sit there, like spiders in a web, and wait until the Sturgians despair of losing all their commerce and try to run the blockade. Then the Vlandians will snap up the ships and their treasure.", null, null, null, null)
				.Condition(delegate
				{
					MBTextManager.SetTextVariable("SETTLEMENT_LINK", NavalStorylineData.Act3Quest3TargetSettlement.EncyclopediaLinkWithName, false);
					return true;
				})
				.NpcLine("{=jFhkURpP}I'm sure Purig could wreak a great deal of wickedness with this silver in his hands, and I would very much like to foil this plan of his.", null, null, null, null)
				.Consequence(delegate
				{
					this._isIntroGiven = true;
				})
				.BeginPlayerOptions(null, false)
				.PlayerOption("{=el44RZG4}Let us set out, then.", null, null, null)
				.Consequence(delegate
				{
					if (Mission.Current == null)
					{
						Campaign.Current.ConversationManager.ConversationEndOneShot += this.StartQuest;
						return;
					}
					Campaign.Current.ConversationManager.ConversationEndOneShot += this.OnPlayerAcceptsQuestThroughMission;
				})
				.CloseDialog()
				.PlayerOption("{=a0j86F9C}I need a bit more time.", null, null, null)
				.Consequence(delegate
				{
					Campaign.Current.ConversationManager.ConversationEndOneShot += NavalStorylineData.OnPlayerPostponedQuestStart;
				})
				.CloseDialog()
				.PlayerOption("{=aEKNUI45}This war on the Sea Hounds is too risky. There must be another way to get my sister back.", null, null, null)
				.GotoDialogState("gunnar_ransom_sister")
				.EndPlayerOptions(), null);
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1500).NpcLine("{=LnqHcu5S}Are we ready to sail for {SETTLEMENT_LINK}? The tide and winds are right.", null, null, null, null).Condition(delegate
			{
				MBTextManager.SetTextVariable("SETTLEMENT_LINK", NavalStorylineData.Act3Quest3TargetSettlement.EncyclopediaLinkWithName, false);
				return NavalStorylineData.IsStorylineActivationPossible() && NavalStorylineData.HasCompletedLast(NavalStorylineData.NavalStorylineStage.Act3Quest2) && Settlement.CurrentSettlement == NavalStorylineData.HomeSettlement && Hero.OneToOneConversationHero == NavalStorylineData.Gunnar && !Campaign.Current.QuestManager.IsThereActiveQuestWithType(typeof(SpeakToTheSailorsQuest)) && this._isIntroGiven;
			})
				.BeginPlayerOptions(null, false)
				.PlayerOption("{=EjnrlsjX}Get the men to their ships. We sail at once.", null, null, null)
				.Consequence(delegate
				{
					if (Mission.Current == null)
					{
						Campaign.Current.ConversationManager.ConversationEndOneShot += this.StartQuest;
						return;
					}
					Campaign.Current.ConversationManager.ConversationEndOneShot += this.OnPlayerAcceptsQuestThroughMission;
				})
				.CloseDialog()
				.PlayerOption("{=Ebk8s9s1}I am not yet ready.", null, null, null)
				.Consequence(delegate
				{
					Campaign.Current.ConversationManager.ConversationEndOneShot += NavalStorylineData.OnPlayerPostponedQuestStart;
				})
				.CloseDialog()
				.PlayerOption("{=aEKNUI45}This war on the Sea Hounds is too risky. There must be another way to get my sister back.", null, null, null)
				.GotoDialogState("gunnar_ransom_sister")
				.EndPlayerOptions(), null);
		}

		// Token: 0x060008C4 RID: 2244 RVA: 0x0003DBBE File Offset: 0x0003BDBE
		private void OnPlayerAcceptsQuestThroughMission()
		{
			this._isQuestAcceptedThroughMission = true;
			this.OpenQuestMenu();
			Mission.Current.EndMission();
		}

		// Token: 0x060008C5 RID: 2245 RVA: 0x0003DBD7 File Offset: 0x0003BDD7
		private void OpenQuestMenu()
		{
			GameMenu.ActivateGameMenu("naval_storyline_act_3_quest_3_conversation_menu");
		}

		// Token: 0x060008C6 RID: 2246 RVA: 0x0003DBE3 File Offset: 0x0003BDE3
		private void StartQuest()
		{
			new SpeakToTheSailorsQuest("speak_to_the_sailors_quest", NavalStorylineData.Act3Quest3TargetSettlement).StartQuest();
		}

		// Token: 0x04000514 RID: 1300
		private const string _questConversationMenuId = "naval_storyline_act_3_quest_3_conversation_menu";

		// Token: 0x04000515 RID: 1301
		private bool _isQuestAcceptedThroughMission;

		// Token: 0x04000516 RID: 1302
		private bool _isIntroGiven;
	}
}
