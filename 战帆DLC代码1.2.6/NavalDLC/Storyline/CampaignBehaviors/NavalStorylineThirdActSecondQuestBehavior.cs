using System;
using System.Collections.Generic;
using Helpers;
using NavalDLC.Storyline.Quests;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Storyline.CampaignBehaviors
{
	// Token: 0x02000079 RID: 121
	public class NavalStorylineThirdActSecondQuestBehavior : CampaignBehaviorBase
	{
		// Token: 0x060008A8 RID: 2216 RVA: 0x0003CE22 File Offset: 0x0003B022
		public override void RegisterEvents()
		{
			if (!NavalStorylineData.IsNavalStorylineCanceled())
			{
				CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
				NavalDLCEvents.OnNavalStorylineCanceledEvent.AddNonSerializedListener(this, new Action<NavalStorylineData.StorylineCancelDetail>(this.OnNavalStorylineCanceled));
			}
		}

		// Token: 0x060008A9 RID: 2217 RVA: 0x0003CE59 File Offset: 0x0003B059
		private void OnNavalStorylineCanceled(NavalStorylineData.StorylineCancelDetail detail)
		{
			CampaignEventDispatcher.Instance.RemoveListeners(this);
		}

		// Token: 0x060008AA RID: 2218 RVA: 0x0003CE66 File Offset: 0x0003B066
		private void OnSessionLaunched(CampaignGameStarter starter)
		{
			this.AddGameMenus(starter);
			this.AddDialogs(starter);
		}

		// Token: 0x060008AB RID: 2219 RVA: 0x0003CE76 File Offset: 0x0003B076
		private void AddGameMenus(CampaignGameStarter starter)
		{
			starter.AddGameMenu("naval_storyline_act_3_quest_2_conversation_menu", string.Empty, new OnInitDelegate(this.naval_storyline_act_3_quest_2_conversation_menu_on_init), 0, 0, null);
		}

		// Token: 0x060008AC RID: 2220 RVA: 0x0003CE97 File Offset: 0x0003B097
		private void naval_storyline_act_3_quest_2_conversation_menu_on_init(MenuCallbackArgs args)
		{
			if (this._isQuestAcceptedThroughMission && Mission.Current == null)
			{
				this.StartQuest();
				this._isQuestAcceptedThroughMission = false;
			}
		}

		// Token: 0x060008AD RID: 2221 RVA: 0x0003CEB8 File Offset: 0x0003B0B8
		private void AddDialogs(CampaignGameStarter starter)
		{
			TextObject textObject = new TextObject("{=TlgUi5Sh}{PLAYER.NAME}... Word spreads fast among sailors. We seem to have made a bit of a name for ourselves with that victory off of Hvalvik. I have someone for you to meet.", null);
			TextObjectExtensions.SetCharacterProperties(textObject, "PLAYER", CharacterObject.PlayerCharacter, false);
			TextObject textObject2 = new TextObject("{=AGY68GQE}So… You are the captain who thrashed those so-called Sea Hounds up north. I have a proposal that I hope would be of interest.", null);
			TextObject textObject3 = new TextObject("{=pUZTxrEy}I am Lahar, of Quyaz, on the Jade Sea. I am here because one of the great families of our city has been having some troubles. The head of one branch, the lady Fahda, has quarreled over her inheritance with her uncles. The elders of the town backed the uncles, so she took to the sea with her retainers and vowed to ravage their shipping.", null);
			textObject3.SetTextVariable("SETTLEMENT_LINK", NavalStorylineData.Act3Quest2TargetSettlement.EncyclopediaLinkWithName);
			TextObject textObject4 = new TextObject("{=MM0mXw6o}How formidable a foe is this Fahda?", null);
			TextObject textObject5 = new TextObject("{=x3EgmkF8}The lady is good at her craft. Fahda has been sailing since she was a child. She always wears a sailor’s cap, and underneath she is as bald as an egg. She persuaded her late father to take her to sea, so the story goes, by cutting off all of her long shining hair lest it catch in the rigging. She has taken several Quyazi ships, and I would be reluctant to fight her alone.", null);
			textObject5.SetTextVariable("SETTLEMENT_LINK", NavalStorylineData.Act3Quest2TargetSettlement.EncyclopediaLinkWithName);
			TextObject textObject6 = new TextObject("{=s7CSGwZ5}What does this have to do with our quarrel with the Sea Hounds?", null);
			TextObject textObject7 = new TextObject("{=JBOE2x1a}The lady Fahda has reportedly joined up with these Sea Hounds, as pirates often band together. She has been prowling about the Gulf of Charas, taking Quyazi vessels. You wish to continue hunting Sea Hounds, do you not? Those would be good waters in which to hunt, and if you are going there, I would like to come with you and lend my assistance.", null);
			TextObject textObject8 = new TextObject("{=pUZPt8Po}Fahda also traffics in captives with the Sea Hounds. She may have bought or held your sister at some point, or if not, at least she may be able to tell us more about the Sea Hounds' trade in slaves.", null);
			TextObject textObject9 = new TextObject("{=TUmPKK8P}Lahar - what will we gain by helping you catch her?", null);
			TextObject textObject10 = new TextObject("{=fbKlKR0v}If you wish to weaken these Sea Hounds, you may want to strike at their allies first. And of course the elders of Quyaz will be most happy to pay a handsome reward, of which you and Gunnar would receive your fair share.", null);
			TextObject textObject11 = new TextObject("{=jo3s90PF}What will you bring on our hunt?", null);
			TextObject textObject12 = new TextObject("{=w9ar5Ldc}I have my loyal crew and a swift liburna, outfitted with a ram, which I think you might put to good purpose. It would be especially useful if we encounter any slow but powerful ships that would be costly to take by boarding.", null);
			TextObject textObject13 = new TextObject("{=jSaUTBbW}I am ready to set out.", null);
			TextObject textObject14 = new TextObject("{=ZUAvYPpg}That sounds promising, but I am not yet ready to depart.", null);
			TextObject textObject15 = new TextObject("{=8T2uf1ay}Can I tell Lahar that we are ready to sail? The tide and winds are with us, and it would be a pity if someone else were to hunt down Fahda and claim the bounty.", null);
			TextObject textObject16 = new TextObject("{=hcm7PZLK}Order the men to their ships. We sail at once.", null);
			TextObject textObject17 = new TextObject("{=vxLowgvR}I am not quite ready. Let us pray that the good winds last a little longer.", null);
			TextObject textObject18 = new TextObject("{=OSZozYIR}Talk with Gunnar when you're ready to depart.", null);
			string text;
			string text2;
			string text3;
			string text4;
			string text5;
			string text6;
			string text7;
			DialogFlow dialogFlow = DialogFlow.CreateDialogFlow("start", 1200).GenerateToken(ref text).GenerateToken(ref text2)
				.GenerateToken(ref text3)
				.GenerateToken(ref text4)
				.GenerateToken(ref text5)
				.GenerateToken(ref text6)
				.GenerateToken(ref text7)
				.NpcLine(textObject, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsGunnar), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), null, null)
				.Condition(new ConversationSentence.OnConditionDelegate(this.MultiAgentConversationCondition))
				.NpcLine(textObject2, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsLahar), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), null, null)
				.NpcLine(textObject3, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsLahar), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), null, null)
				.GotoDialogState(text)
				.BeginPlayerOptions(text, false)
				.PlayerOption(textObject4, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsLahar), null, null)
				.NpcLine(textObject5, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsLahar), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), null, null)
				.GotoDialogState(text3)
				.PlayerOption(textObject6, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsLahar), null, null)
				.NpcLine(textObject7, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsLahar), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), null, null)
				.GotoDialogState(text2)
				.EndPlayerOptions()
				.BeginPlayerOptions(text2, false)
				.PlayerOption(textObject4, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsLahar), null, null)
				.NpcLine(textObject5, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsLahar), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), null, null)
				.NpcLine(textObject8, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsGunnar), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), null, null)
				.GotoDialogState(text4)
				.EndPlayerOptions()
				.BeginPlayerOptions(text3, false)
				.PlayerOption(textObject6, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsLahar), null, null)
				.NpcLine(textObject7, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsLahar), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), null, null)
				.NpcLine(textObject8, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsGunnar), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), null, null)
				.GotoDialogState(text4)
				.EndPlayerOptions()
				.BeginPlayerOptions(text4, false)
				.PlayerOption(textObject9, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsLahar), null, null)
				.NpcLine(textObject10, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsLahar), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), null, null)
				.GotoDialogState(text6)
				.PlayerOption(textObject11, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsLahar), null, null)
				.NpcLine(textObject12, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsLahar), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), null, null)
				.GotoDialogState(text5)
				.EndPlayerOptions()
				.BeginPlayerOptions(text5, false)
				.PlayerOption(textObject9, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsLahar), null, null)
				.NpcLine(textObject10, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsLahar), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), null, null)
				.GotoDialogState(text7)
				.EndPlayerOptions()
				.BeginPlayerOptions(text6, false)
				.PlayerOption(textObject11, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsLahar), null, null)
				.NpcLine(textObject12, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsLahar), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), null, null)
				.GotoDialogState(text7)
				.EndPlayerOptions()
				.BeginPlayerOptions(text7, false)
				.PlayerOption(textObject13, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsGunnar), null, null)
				.Consequence(delegate
				{
					Campaign.Current.ConversationManager.ConversationEndOneShot += this.OnPlayerAcceptsQuestThroughMission;
				})
				.CloseDialog()
				.PlayerOption(textObject14, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsGunnar), null, null)
				.Consequence(delegate
				{
					Campaign.Current.ConversationManager.ConversationEndOneShot += NavalStorylineData.OnPlayerPostponedQuestStart;
				})
				.CloseDialog()
				.PlayerOption("{=aEKNUI45}This war on the Sea Hounds is too risky. There must be another way to get my sister back.", null, null, null)
				.GotoDialogState("gunnar_ransom_sister")
				.EndPlayerOptions();
			DialogFlow dialogFlow2 = DialogFlow.CreateDialogFlow("start", 1200).NpcLine(textObject15, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsGunnar), null, null, null).Condition(() => NavalStorylineData.Lahar.HasMet && this.IsAct3Quest2ReadyToStart(NavalStorylineData.Gunnar))
				.BeginPlayerOptions(null, false)
				.PlayerOption(textObject16, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsGunnar), null, null)
				.Consequence(delegate
				{
					Campaign.Current.ConversationManager.ConversationEndOneShot += this.OnPlayerAcceptsQuestThroughMission;
				})
				.CloseDialog()
				.PlayerOption(textObject17, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsGunnar), null, null)
				.Consequence(delegate
				{
					Campaign.Current.ConversationManager.ConversationEndOneShot += NavalStorylineData.OnPlayerPostponedQuestStart;
				})
				.CloseDialog()
				.PlayerOption("{=aEKNUI45}This war on the Sea Hounds is too risky. There must be another way to get my sister back.", null, null, null)
				.GotoDialogState("gunnar_ransom_sister")
				.EndPlayerOptions();
			DialogFlow dialogFlow3 = DialogFlow.CreateDialogFlow("start", 1200).NpcLine(textObject18, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsLahar), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), null, null).Condition(() => NavalStorylineData.Lahar.HasMet && this.IsAct3Quest2ReadyToStart(NavalStorylineData.Lahar))
				.CloseDialog();
			Campaign.Current.ConversationManager.AddDialogFlow(dialogFlow, null);
			Campaign.Current.ConversationManager.AddDialogFlow(dialogFlow2, null);
			Campaign.Current.ConversationManager.AddDialogFlow(dialogFlow3, null);
		}

		// Token: 0x060008AE RID: 2222 RVA: 0x0003D4E8 File Offset: 0x0003B6E8
		private bool MultiAgentConversationCondition()
		{
			StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, null, false);
			if (this.IsAct3Quest2ReadyToStart(NavalStorylineData.Gunnar) && Mission.Current != null && !NavalStorylineData.Lahar.HasMet)
			{
				NavalStorylineData.Lahar.SetHasMet();
				Agent agent = null;
				foreach (Agent agent2 in Mission.Current.GetNearbyAgents(Agent.Main.Position.AsVec2, 100f, new MBList<Agent>()))
				{
					if (agent2.Character == NavalStorylineData.Gunnar.CharacterObject)
					{
						agent = agent2;
						break;
					}
				}
				if (agent != null)
				{
					Agent agent3 = this.SpawnLahar(agent);
					agent3.SetLookAgent(Agent.Main);
					Campaign.Current.ConversationManager.AddConversationAgents(new List<Agent> { agent3 }, true);
				}
				return true;
			}
			return false;
		}

		// Token: 0x060008AF RID: 2223 RVA: 0x0003D5EC File Offset: 0x0003B7EC
		private bool IsAct3Quest2ReadyToStart(Hero conversationHero)
		{
			return NavalStorylineData.IsStorylineActivationPossible() && NavalStorylineData.HasCompletedLast(NavalStorylineData.NavalStorylineStage.Act3Quest1) && Hero.OneToOneConversationHero == conversationHero && Settlement.CurrentSettlement == NavalStorylineData.HomeSettlement && !Campaign.Current.QuestManager.IsThereActiveQuestWithType(typeof(SailToTheGulfOfCharasQuest)) && !Campaign.Current.QuestManager.IsThereActiveQuestWithType(typeof(HuntDownTheEmiraAlFahdaAndTheCorsairsQuest));
		}

		// Token: 0x060008B0 RID: 2224 RVA: 0x0003D655 File Offset: 0x0003B855
		private bool IsGunnar(IAgent agent)
		{
			return agent.Character == NavalStorylineData.Gunnar.CharacterObject;
		}

		// Token: 0x060008B1 RID: 2225 RVA: 0x0003D669 File Offset: 0x0003B869
		private bool IsLahar(IAgent agent)
		{
			return agent.Character == NavalStorylineData.Lahar.CharacterObject;
		}

		// Token: 0x060008B2 RID: 2226 RVA: 0x0003D67D File Offset: 0x0003B87D
		private bool IsMainHero(IAgent agent)
		{
			return agent.Character == CharacterObject.PlayerCharacter;
		}

		// Token: 0x060008B3 RID: 2227 RVA: 0x0003D68C File Offset: 0x0003B88C
		private Agent SpawnLahar(Agent gunnar)
		{
			AgentBuildData agentBuildData = new AgentBuildData(NavalStorylineData.Lahar.CharacterObject);
			agentBuildData.TroopOrigin(new SimpleAgentOrigin(agentBuildData.AgentCharacter, -1, null, default(UniqueTroopDescriptor)));
			Vec3 vec = gunnar.Position - Agent.Main.Position;
			vec.RotateAboutZ(0.34906584f);
			vec += Agent.Main.Position;
			int num = 250;
			for (;;)
			{
				Mission mission = Mission.Current;
				UIntPtr? uintPtr;
				if (mission == null)
				{
					uintPtr = null;
				}
				else
				{
					Scene scene = mission.Scene;
					uintPtr = ((scene != null) ? new UIntPtr?(scene.GetNavigationMeshForPosition(ref vec)) : null);
				}
				UIntPtr? uintPtr2 = uintPtr;
				UIntPtr zero = UIntPtr.Zero;
				if (uintPtr2 == null || (uintPtr2 != null && !(uintPtr2.GetValueOrDefault() == zero)) || num == 0)
				{
					break;
				}
				if (MBRandom.RandomFloat > 0.5f)
				{
					vec.RotateAboutZ(0.017453292f * (float)MBRandom.RandomInt(20, 45));
				}
				else
				{
					vec.RotateAboutZ(0.017453292f * (float)MBRandom.RandomInt(-45, -20));
				}
				num--;
			}
			if (num == 0)
			{
				Debug.FailedAssert("Couldn't find a valid position for Lahar around Gunnar", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\Storyline\\CampaignBehaviors\\NavalStorylineThirdActSecondQuestBehavior.cs", "SpawnLahar", 284);
				vec = Mission.Current.GetRandomPositionAroundPoint(gunnar.Position, 1f, 3f, true);
			}
			agentBuildData.InitialPosition(ref vec);
			AgentBuildData agentBuildData2 = agentBuildData;
			Vec2 vec2 = Agent.Main.LookDirection.AsVec2;
			vec2 = -vec2.Normalized();
			agentBuildData2.InitialDirection(ref vec2);
			agentBuildData.NoHorses(true);
			agentBuildData.CivilianEquipment(true);
			return Mission.Current.SpawnAgent(agentBuildData, false);
		}

		// Token: 0x060008B4 RID: 2228 RVA: 0x0003D837 File Offset: 0x0003BA37
		private void OnPlayerAcceptsQuestThroughMission()
		{
			this._isQuestAcceptedThroughMission = true;
			GameMenu.ActivateGameMenu("naval_storyline_act_3_quest_2_conversation_menu");
			Mission.Current.EndMission();
		}

		// Token: 0x060008B5 RID: 2229 RVA: 0x0003D854 File Offset: 0x0003BA54
		private void StartQuest()
		{
			if (!Campaign.Current.QuestManager.IsThereActiveQuestWithType(typeof(SailToTheGulfOfCharasQuest)))
			{
				CampaignVec2 campaignVec;
				campaignVec..ctor(new Vec2(194.4578f, 359.8387f), false);
				if (!NavigationHelper.IsPositionValidForNavigationType(campaignVec, 2))
				{
					campaignVec = NavigationHelper.FindReachablePointAroundPosition(campaignVec, 2, 10f, 0f, false);
				}
				new SailToTheGulfOfCharasQuest("naval_storyline_act3_quest2_1", NavalStorylineData.Gunnar, campaignVec).StartQuest();
			}
		}

		// Token: 0x060008B6 RID: 2230 RVA: 0x0003D8C4 File Offset: 0x0003BAC4
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x04000512 RID: 1298
		private const string _questConversationMenuId = "naval_storyline_act_3_quest_2_conversation_menu";

		// Token: 0x04000513 RID: 1299
		private bool _isQuestAcceptedThroughMission;
	}
}
