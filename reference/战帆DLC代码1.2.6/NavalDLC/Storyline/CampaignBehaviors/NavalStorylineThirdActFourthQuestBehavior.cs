using System;
using System.Linq;
using NavalDLC.Storyline.Quests;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Storyline.CampaignBehaviors
{
	// Token: 0x02000078 RID: 120
	public class NavalStorylineThirdActFourthQuestBehavior : CampaignBehaviorBase
	{
		// Token: 0x06000897 RID: 2199 RVA: 0x0003C617 File Offset: 0x0003A817
		public override void RegisterEvents()
		{
			if (!NavalStorylineData.IsNavalStorylineCanceled())
			{
				CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
				NavalDLCEvents.OnNavalStorylineCanceledEvent.AddNonSerializedListener(this, new Action<NavalStorylineData.StorylineCancelDetail>(this.OnNavalStorylineCanceled));
			}
		}

		// Token: 0x06000898 RID: 2200 RVA: 0x0003C64E File Offset: 0x0003A84E
		private void OnNavalStorylineCanceled(NavalStorylineData.StorylineCancelDetail detail)
		{
			CampaignEventDispatcher.Instance.RemoveListeners(this);
		}

		// Token: 0x06000899 RID: 2201 RVA: 0x0003C65B File Offset: 0x0003A85B
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x0600089A RID: 2202 RVA: 0x0003C65D File Offset: 0x0003A85D
		private void OnSessionLaunched(CampaignGameStarter starter)
		{
			this.AddDialogs();
			this.AddGameMenus(starter);
		}

		// Token: 0x0600089B RID: 2203 RVA: 0x0003C66C File Offset: 0x0003A86C
		private void AddGameMenus(CampaignGameStarter starter)
		{
			starter.AddGameMenu("naval_storyline_act_3_quest_4_conversation_menu", string.Empty, new OnInitDelegate(this.naval_storyline_act_3_quest_4_conversation_menu_on_init), 0, 0, null);
		}

		// Token: 0x0600089C RID: 2204 RVA: 0x0003C690 File Offset: 0x0003A890
		private void AddDialogs()
		{
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1200).NpcLine(new TextObject("{=sob0plMW}Good news, {PLAYER.NAME}... Bjolgur’s order has given him permission to sail with us.", null), (IAgent agent) => agent.Character == NavalStorylineData.Gunnar.CharacterObject, (IAgent agent) => agent.Character == CharacterObject.PlayerCharacter, null, null).Condition(new ConversationSentence.OnConditionDelegate(this.GunnarActivateQuestFourDialog1OnCondition))
				.Consequence(new ConversationSentence.OnConsequenceDelegate(this.GunnarActivateQuestFourDialog1OnConsequence))
				.NpcLine(new TextObject("{=eiX98VE9}Greetings, {PLAYER.NAME}... I’ve got my longship, Corpse-Maker, and more of my brothers may yet join us on the journey. We also brought a captured vessel, agile and light, which mounts a ballista. We call it the Golden Wasp. We’ve bought up most of the ale in Ostican for our voyage, as I think we’ll be heading for the sweltering seas of the south.", null), (IAgent agent) => agent.Character == NavalStorylineData.Bjolgur.CharacterObject, (IAgent agent) => agent.Character == CharacterObject.PlayerCharacter, null, null)
				.NpcLine(new TextObject("{=egYc68CI}I’ve been making some inquiries. Crusas is well-known and respected in the Empire and in Vlandia. He mines sulfur from islands in the Gulf of Charas. No doubt he uses some of Purig’s slaves, but I guess the grand lords and ladies don’t know that, or choose not to know.", null), (IAgent agent) => agent.Character == NavalStorylineData.Gunnar.CharacterObject, (IAgent agent) => agent.Character == CharacterObject.PlayerCharacter, null, null)
				.BeginPlayerOptions(null, false)
				.PlayerOption("{=npbsJToM}I hope, then, that he should not be difficult to find.", (IAgent agent) => agent.Character == NavalStorylineData.Gunnar.CharacterObject, null, null)
				.GotoDialogState("q4_next_line")
				.PlayerOption("{=Cywj1xTj}Well respected or not, I’m ready to track him down.", (IAgent agent) => agent.Character == NavalStorylineData.Gunnar.CharacterObject, null, null)
				.GotoDialogState("q4_next_line")
				.EndPlayerOptions()
				.NpcLine(new TextObject("{=sghtD7ov}Not hard to find at all.. On the way here I hailed some fishermen who chase tuna in the Gulf of Charas, and they say he is known to frequent a string of islands known as the Skatrias. They are said to be barren and foul-smelling. I can’t think why a merchant would want to anchor there, were they not the site of these sulfur mines where the captives are sent.{NEW_LINE}{NEW_LINE}So… I say we set out for these islands and hunt for Crusas.", null).SetTextVariable("NEW_LINE", "\n"), (IAgent agent) => agent.Character == NavalStorylineData.Bjolgur.CharacterObject, (IAgent agent) => agent.Character == CharacterObject.PlayerCharacter, "q4_next_line", "q4_next_line_player_choices")
				.BeginPlayerOptions("q4_next_line_player_choices", false)
				.PlayerOption("{=el44RZG4}Let us set out, then.", (IAgent agent) => agent.Character == NavalStorylineData.Bjolgur.CharacterObject, null, null)
				.Consequence(delegate
				{
					Campaign.Current.ConversationManager.ConversationEndOneShot += this.OnPlayerAcceptsQuestThroughMission;
				})
				.CloseDialog()
				.PlayerOption("{=a0j86F9C}I need a bit more time.", (IAgent agent) => agent.Character == NavalStorylineData.Bjolgur.CharacterObject, null, null)
				.Consequence(delegate
				{
					Campaign.Current.ConversationManager.ConversationEndOneShot += NavalStorylineData.OnPlayerPostponedQuestStart;
				})
				.CloseDialog()
				.PlayerOption("{=aEKNUI45}This war on the Sea Hounds is too risky. There must be another way to get my sister back.", null, null, null)
				.GotoDialogState("gunnar_ransom_sister")
				.EndPlayerOptions(), null);
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1200).NpcLine(new TextObject("{=C8aEfvMM}Are we ready to set sail for the Skatrias? I imagine that Crusas will be docked there for some time, but we don’t want to miss this opportunity.", null), (IAgent agent) => agent.Character == NavalStorylineData.Gunnar.CharacterObject, (IAgent agent) => agent.Character == CharacterObject.PlayerCharacter, null, null).Condition(new ConversationSentence.OnConditionDelegate(this.GunnarActivateQuestFourDialog2OnCondition))
				.BeginPlayerOptions(null, false)
				.PlayerOption("{=el44RZG4}Let us set out, then.", (IAgent agent) => agent.Character == NavalStorylineData.Gunnar.CharacterObject, null, null)
				.Consequence(delegate
				{
					if (Mission.Current == null)
					{
						Campaign.Current.ConversationManager.ConversationEndOneShot += this.ActivateQuest4;
						return;
					}
					Campaign.Current.ConversationManager.ConversationEndOneShot += this.OnPlayerAcceptsQuestThroughMission;
				})
				.CloseDialog()
				.PlayerOption("{=a0j86F9C}I need a bit more time.", (IAgent agent) => agent.Character == NavalStorylineData.Gunnar.CharacterObject, null, null)
				.Consequence(delegate
				{
					Campaign.Current.ConversationManager.ConversationEndOneShot += NavalStorylineData.OnPlayerPostponedQuestStart;
				})
				.CloseDialog()
				.PlayerOption("{=aEKNUI45}This war on the Sea Hounds is too risky. There must be another way to get my sister back.", null, null, null)
				.GotoDialogState("gunnar_ransom_sister")
				.EndPlayerOptions(), null);
		}

		// Token: 0x0600089D RID: 2205 RVA: 0x0003CA9F File Offset: 0x0003AC9F
		private void naval_storyline_act_3_quest_4_conversation_menu_on_init(MenuCallbackArgs args)
		{
			if (this._isQuestAcceptedThroughMission && Mission.Current == null)
			{
				this.ActivateQuest4();
				this._isQuestAcceptedThroughMission = false;
			}
		}

		// Token: 0x0600089E RID: 2206 RVA: 0x0003CAC0 File Offset: 0x0003ACC0
		private bool GunnarActivateQuestFourDialog1OnCondition()
		{
			bool flag = !this._initialConversationIsDone && Hero.OneToOneConversationHero == NavalStorylineData.Gunnar && !NavalStorylineData.IsNavalStoryLineActive() && NavalStorylineData.IsStorylineActivationPossible() && NavalStorylineData.HasCompletedLast(NavalStorylineData.NavalStorylineStage.Act3SpeakToSailors);
			if (flag)
			{
				NavalStorylineThirdActFourthQuestBehavior.SpawnBjolgur();
				Agent agent = Mission.Current.Agents.First<Agent>((Agent x) => x.Character == NavalStorylineData.Bjolgur.CharacterObject);
				ConversationManager conversationManager = Campaign.Current.ConversationManager;
				MBList<IAgent> mblist = new MBList<IAgent>();
				mblist.Add(agent);
				conversationManager.AddConversationAgents(mblist, false);
			}
			return flag;
		}

		// Token: 0x0600089F RID: 2207 RVA: 0x0003CB4B File Offset: 0x0003AD4B
		private void GunnarActivateQuestFourDialog1OnConsequence()
		{
			this._initialConversationIsDone = true;
		}

		// Token: 0x060008A0 RID: 2208 RVA: 0x0003CB54 File Offset: 0x0003AD54
		private static void SpawnBjolgur()
		{
			Agent agent = Mission.Current.Agents.First<Agent>((Agent x) => x.Character == NavalStorylineData.Gunnar.CharacterObject);
			AgentBuildData agentBuildData = new AgentBuildData(NavalStorylineData.Bjolgur.CharacterObject);
			agentBuildData.TroopOrigin(new SimpleAgentOrigin(agentBuildData.AgentCharacter, -1, null, default(UniqueTroopDescriptor)));
			Vec3 vec = agent.Position - Agent.Main.Position;
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
				Debug.FailedAssert("Couldn't find a valid position for Bjolgur around Gunnar", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\Storyline\\CampaignBehaviors\\NavalStorylineThirdActFourthQuestBehavior.cs", "SpawnBjolgur", 169);
				vec = Mission.Current.GetRandomPositionAroundPoint(agent.Position, 1f, 3f, true);
			}
			agentBuildData.InitialPosition(ref vec);
			AgentBuildData agentBuildData2 = agentBuildData;
			Vec2 vec2 = Agent.Main.LookDirection.AsVec2;
			vec2 = -vec2.Normalized();
			agentBuildData2.InitialDirection(ref vec2);
			agentBuildData.NoHorses(true);
			agentBuildData.CivilianEquipment(true);
			Mission.Current.SpawnAgent(agentBuildData, false);
		}

		// Token: 0x060008A1 RID: 2209 RVA: 0x0003CD30 File Offset: 0x0003AF30
		private bool GunnarActivateQuestFourDialog2OnCondition()
		{
			return this._initialConversationIsDone && Hero.OneToOneConversationHero == NavalStorylineData.Gunnar && !NavalStorylineData.IsNavalStoryLineActive() && NavalStorylineData.IsStorylineActivationPossible() && NavalStorylineData.HasCompletedLast(NavalStorylineData.NavalStorylineStage.Act3SpeakToSailors);
		}

		// Token: 0x060008A2 RID: 2210 RVA: 0x0003CD5C File Offset: 0x0003AF5C
		private void OnPlayerAcceptsQuestThroughMission()
		{
			this._isQuestAcceptedThroughMission = true;
			this.OpenQuestMenu();
			Mission.Current.EndMission();
		}

		// Token: 0x060008A3 RID: 2211 RVA: 0x0003CD75 File Offset: 0x0003AF75
		private void OpenQuestMenu()
		{
			GameMenu.ActivateGameMenu("naval_storyline_act_3_quest_4_conversation_menu");
		}

		// Token: 0x060008A4 RID: 2212 RVA: 0x0003CD84 File Offset: 0x0003AF84
		private void ActivateQuest4()
		{
			CampaignVec2 campaignVec;
			campaignVec..ctor(new Vec2(285f, 300f), false);
			new GoToSkatriaIslandsQuest("naval_storyline_act_3_quest_4", NavalStorylineData.Gunnar, campaignVec).StartQuest();
		}

		// Token: 0x0400050F RID: 1295
		private const string QuestConversationMenuId = "naval_storyline_act_3_quest_4_conversation_menu";

		// Token: 0x04000510 RID: 1296
		private bool _isQuestAcceptedThroughMission;

		// Token: 0x04000511 RID: 1297
		private bool _initialConversationIsDone;
	}
}
