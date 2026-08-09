using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using NavalDLC.Missions;
using NavalDLC.SceneInformationPopupTypes;
using NavalDLC.Storyline.MissionControllers;
using StoryMode.StoryModeObjects;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace NavalDLC.Storyline.Quests
{
	// Token: 0x02000036 RID: 54
	public class FreeTheSeaHoundsCaptivesQuest : NavalStorylineQuestBase
	{
		// Token: 0x17000087 RID: 135
		// (get) Token: 0x0600036C RID: 876 RVA: 0x00018CDB File Offset: 0x00016EDB
		public override TextObject Title
		{
			get
			{
				return new TextObject("{=JYCrUhnu}Free the Sea Hounds' captives", null);
			}
		}

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x0600036D RID: 877 RVA: 0x00018CE8 File Offset: 0x00016EE8
		public override NavalStorylineData.NavalStorylineStage Stage
		{
			get
			{
				return NavalStorylineData.NavalStorylineStage.Act3Quest5;
			}
		}

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x0600036E RID: 878 RVA: 0x00018CEB File Offset: 0x00016EEB
		public override bool WillProgressStoryline
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x0600036F RID: 879 RVA: 0x00018CEE File Offset: 0x00016EEE
		protected override string MainPartyTemplateStringId
		{
			get
			{
				return "storyline_act3_quest_5_main_party_template";
			}
		}

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x06000370 RID: 880 RVA: 0x00018CF5 File Offset: 0x00016EF5
		private CampaignVec2 _seaHoundsSpawnPosition
		{
			get
			{
				return new CampaignVec2(new Vec2(260f, 815f), false);
			}
		}

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x06000371 RID: 881 RVA: 0x00018D0C File Offset: 0x00016F0C
		private TextObject _allyDefeatedText
		{
			get
			{
				return new TextObject("{=9sfcVI0Q}Your allies were defeated. You will have to try again.", null);
			}
		}

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x06000372 RID: 882 RVA: 0x00018D19 File Offset: 0x00016F19
		private TextObject _findSeaHoundsQuestLog
		{
			get
			{
				return new TextObject("{=mp0EKEI9}Go to Angranfjord and locate the Sea Hounds.", null);
			}
		}

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x06000373 RID: 883 RVA: 0x00018D26 File Offset: 0x00016F26
		private TextObject _arrivedAngranfjordQuestLog
		{
			get
			{
				return new TextObject("{=7Gl82o4g}You have arrived at Angranfjord, Purig's lair.", null);
			}
		}

		// Token: 0x06000374 RID: 884 RVA: 0x00018D34 File Offset: 0x00016F34
		public FreeTheSeaHoundsCaptivesQuest(string questId, float strengthModifier)
			: base(questId, NavalStorylineData.Gunnar, CampaignTime.Never, 0)
		{
			this._strengthModifier = strengthModifier;
			this._skatriaIslandsMarker = Campaign.Current.MapMarkerManager.CreateMapMarker(NavalStorylineData.CorsairBanner, new TextObject("{=GSksjBCZ}Angranfjord", null), this._seaHoundsSpawnPosition.AsVec3(), true, base.StringId);
			this._currentState = FreeTheSeaHoundsCaptivesQuest.FreeTheSeaHoundsCaptivesQuestState.GoToSeaHoundPartyPosition;
			this.SetDialogs();
			this.AddGameMenus();
		}

		// Token: 0x06000375 RID: 885 RVA: 0x00018E70 File Offset: 0x00017070
		protected override void PreAfterLoad()
		{
			if (!NavalStorylineData.Purig.IsDead)
			{
				if (NavalStorylineData.HasCompletedLast(NavalStorylineData.NavalStorylineStage.Act3Quest5) || NavalStorylineData.HasCompletedLast(NavalStorylineData.NavalStorylineStage.Act3SpeakToGunnarAndSister))
				{
					KillCharacterAction.ApplyByRemove(NavalStorylineData.Purig, false, true);
					return;
				}
				if (NavalStorylineData.HasCompletedLast(NavalStorylineData.NavalStorylineStage.Act3Quest4) && NavalStorylineData.Purig.VolunteerTypes == null)
				{
					MobileParty partyBelongedTo = NavalStorylineData.Purig.PartyBelongedTo;
					bool flag;
					if (partyBelongedTo == null)
					{
						flag = false;
					}
					else
					{
						MapEvent mapEvent = partyBelongedTo.MapEvent;
						bool? flag2 = ((mapEvent != null) ? new bool?(mapEvent.IsPlayerMapEvent) : null);
						bool flag3 = true;
						flag = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
					}
					if (flag)
					{
						NavalStorylineData.Purig.PartyBelongedTo.MapEvent.FinalizeEvent();
					}
					KillCharacterAction.ApplyByRemove(NavalStorylineData.Purig, false, true);
					this._lastHitCheckpoint = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.End;
					this._currentState = FreeTheSeaHoundsCaptivesQuest.FreeTheSeaHoundsCaptivesQuestState.DefeatedPurig;
				}
			}
		}

		// Token: 0x06000376 RID: 886 RVA: 0x00018F35 File Offset: 0x00017135
		protected override void InitializeQuestOnGameLoadInternal()
		{
			base.InitializeQuestOnGameLoadInternal();
			this.SetDialogs();
			this.AddGameMenus();
			if (this._lastHitCheckpoint == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.End)
			{
				if (this.BossFightOutCome == Quest5SetPieceBattleMissionController.BossFightOutComeEnum.None)
				{
					this.BossFightOutCome = Quest5SetPieceBattleMissionController.BossFightOutComeEnum.PlayerRefusedTheDuel;
				}
				this.ShowNavalSaveSisterSceneNotification();
			}
		}

		// Token: 0x06000377 RID: 887 RVA: 0x00018F68 File Offset: 0x00017168
		protected override void RegisterEventsInternal()
		{
			CampaignEvents.OnMissionEndedEvent.AddNonSerializedListener(this, new Action<IMission>(this.OnMissionEnded));
			CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.OnHourlyTick));
			CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.OnMapEventEnded));
			CampaignEvents.CanHeroBecomePrisonerEvent.AddNonSerializedListener(this, new ReferenceAction<Hero, bool>(this.CanHeroBecomePrisoner));
			CampaignEvents.PartyVisibilityChangedEvent.AddNonSerializedListener(this, new Action<PartyBase>(this.OnPartyVisibilityChanged));
		}

		// Token: 0x06000378 RID: 888 RVA: 0x00018FE8 File Offset: 0x000171E8
		protected override void SetDialogs()
		{
			DialogFlow dialogFlow = DialogFlow.CreateDialogFlow("start", 1200).NpcLine(new TextObject("{=qn00ppJR}There they are. With your sister as their hostage, a straight-out attack is out of the question. Throughout this voyage, I have been thinking on what we might do to ensure her safety, and I recommend that we try an old corsair's trick.", null), new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsGunnar|44_11), new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsPlayer|44_9), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.GunnarInitialMeetingDialogCondition))
				.NpcLine(new TextObject("{=axgouPEG}Do you see that big cluster of ships back there? That's got to be where they're holding the prisoners. That smaller vessel out front, though - that's got to be a picket, and it will stop us before we get too close. Let's approach it, pretending to be a buyer, while Bjolgur and Lahar stay out of sight. Crusas can banter with them a bit as a distraction. One of our men shall stand at his side with a dagger, lest he betray us.", null), new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsGunnar|44_11), new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsPlayer|44_9), null, null)
				.NpcLine(new TextObject("{=HzlWiTns}You and I, meanwhile, shall dive off the side of our ship, swim round to the stern of the prisoner ship, and climb up the side. Then together we can try to find your sister on board. Once we succeed, well, we'll just have to figure it out from there.", null), new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsGunnar|44_11), new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsPlayer|44_9), null, null)
				.PlayerLine(new TextObject("{=kJaiDDRi}Let's proceed, then.", null), new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsGunnar|44_11), null, null)
				.Consequence(new ConversationSentence.OnConsequenceDelegate(this.GunnarInitialMeetingDialogConsequence))
				.CloseDialog();
			DialogFlow dialogFlow2 = DialogFlow.CreateDialogFlow("start", 1200).NpcLine("{=Q5B3Uvoa}Who's there? What's going on??[if:convo_dismayed]", new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsSister|44_10), new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsPlayer|44_9), null, null).Condition(delegate
			{
				if (Mission.Current == null)
				{
					return false;
				}
				Quest5SetPieceBattleMissionController missionBehavior = Mission.Current.GetMissionBehavior<Quest5SetPieceBattleMissionController>();
				return missionBehavior != null && Hero.OneToOneConversationHero == StoryModeHeroes.LittleSister && missionBehavior.State == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1ShipInteriorPhase;
			})
				.PlayerLine("{=0lTm2sy1}{SISTER.NAME}... Is that you? It's me!", new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsSister|44_10), null, null)
				.Condition(delegate
				{
					StringHelpers.SetCharacterProperties("SISTER", StoryModeHeroes.LittleSister.CharacterObject, null, false);
					return true;
				})
				.NpcLine("{=IC9Fvl54}{?PLAYER.GENDER}Sister{?}Brother{\\?}! Heaven's mercy! What are you doing here?[rf:convo_relaxed_happy]", new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsSister|44_10), new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsPlayer|44_9), null, null)
				.BeginPlayerOptions(null, false)
				.PlayerOption("{=HKx2nxGt}It is. We're here to rescue you! Just... Keep your voice low.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsSister|44_10), null, null)
				.GotoDialogState("sister_answer_1")
				.PlayerOption("{=gvOJ43Na}{SISTER.NAME}, I just need you to be patient and strong a little longer.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsSister|44_10), null, null)
				.GotoDialogState("sister_answer_1")
				.EndPlayerOptions()
				.NpcLine("{=OLTofDbM}I'll be silent. What's going on?[ib:wounded]", new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsSister|44_10), new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsPlayer|44_9), "sister_answer_1", null)
				.BeginPlayerOptions(null, false)
				.PlayerOption("{=jrloQtMP}I'm going to take this ship, and get you to safety.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsSister|44_10), null, null)
				.GotoDialogState("sister_answer_2")
				.PlayerOption("{=aLaA3jZ2}I'm going to free you, and kill every last one of those slavers!", new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsSister|44_10), null, null)
				.GotoDialogState("sister_answer_2")
				.EndPlayerOptions()
				.NpcLine("{=w83SHIYa}Can you get me out of here?", new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsSister|44_10), new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsPlayer|44_9), "sister_answer_2", null)
				.BeginPlayerOptions(null, false)
				.PlayerOption("{=21BSwRCQ}Those timbers on your cell look thick. I don't have time now to chop through them.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsSister|44_10), null, null)
				.GotoDialogState("sister_answer_3")
				.PlayerOption("{=kfHpv0Jg}I'll finish off the slavers and sail this ship out of here, then we can break you out.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsSister|44_10), null, null)
				.GotoDialogState("sister_answer_3")
				.EndPlayerOptions()
				.NpcLine("{=jjjS4TLY}I understand. Heaven protect you, {?PLAYER.GENDER}Sister{?}Brother{\\?}![rf:convo_grave]", new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsSister|44_10), new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsPlayer|44_9), "sister_answer_3", null)
				.Consequence(delegate
				{
					Campaign.Current.ConversationManager.ConversationEndOneShot += delegate
					{
						Mission.Current.GetMissionBehavior<Quest5SetPieceBattleMissionController>().SetTalkedWithSister();
					};
				})
				.CloseDialog();
			DialogFlow dialogFlow3 = DialogFlow.CreateDialogFlow("start", 5200).NpcLine("{=Ja5bHsro}You... You and {QUEST_5_COMPANION.NAME} have been slaughtering my allies all up and down this coast, and now it comes to this.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsPurig|44_12), new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsPlayer|44_9), null, null).Condition(delegate
			{
				if (Mission.Current == null)
				{
					return false;
				}
				StringHelpers.SetCharacterProperties("QUEST_5_COMPANION", NavalStorylineData.Gunnar.CharacterObject, null, false);
				Quest5SetPieceBattleMissionController missionBehavior2 = Mission.Current.GetMissionBehavior<Quest5SetPieceBattleMissionController>();
				return missionBehavior2 != null && Hero.OneToOneConversationHero == NavalStorylineData.Purig && missionBehavior2.State == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.BossFightConversationInProgress;
			})
				.NpcLine("{=naMWdTPV}I was going to forge the Sea Hounds into a weapon of vengeance against the house of Volbjorn.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsPurig|44_12), new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsPlayer|44_9), null, null)
				.NpcLine("{=MR1tc1Ao}I would have drowned them in their own blood. But to the free warriors of the north, to the men who stood against the tyrant - I would have showered them with gold. I would have given them the fame that they deserved. We would have ruled the northern seas.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsPurig|44_12), new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsPlayer|44_9), null, null)
				.NpcLine("{=7rCvGfgb}But that is all for nothing. Instead, the kings of Nordvyg, the men that {QUEST_5_COMPANION.NAME} and I fought, will have the last laugh. So, do you like what you've wrought?", new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsPurig|44_12), new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsPlayer|44_9), null, null)
				.BeginPlayerOptions(null, false)
				.PlayerOption("{=fiSglIaN}You'd have been twice the tyrant that Volbjorn was.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsPurig|44_12), null, null)
				.GotoDialogState("purig_answer")
				.PlayerOption("{=7pWJKkQx}I don't care about your old wars. You put my sister in a cage.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsPurig|44_12), null, null)
				.GotoDialogState("purig_answer")
				.PlayerOption("{=Mkxm5l1N}You are outnumbered. Stop bandying words.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsPurig|44_12), null, null)
				.GotoDialogState("purig_answer")
				.EndPlayerOptions()
				.NpcLine("{=U9CfaZTF}Not much honor in having your men just cut me down, is there? Fight me one-to-one. If I win, I go free, and we need never see each other again. If you win, people will remember you as the one who slew the terror of the north.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsPurig|44_12), new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsPlayer|44_9), "purig_answer", null)
				.BeginPlayerOptions(null, false)
				.PlayerOption("{=16CMD4HL}I am willing to duel.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsPurig|44_12), null, null)
				.Consequence(delegate
				{
					Mission.Current.GetMissionBehavior<Quest5SetPieceBattleMissionController>().StartBossFight(true);
					this._currentState = FreeTheSeaHoundsCaptivesQuest.FreeTheSeaHoundsCaptivesQuestState.TalkedWithPurigBeforeBossFight;
				})
				.CloseDialog()
				.PlayerOption("{=pspOcQY7}You dare talk to me of honor? Kill him, lads!", new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsPurig|44_12), null, null)
				.Consequence(delegate
				{
					Mission.Current.GetMissionBehavior<Quest5SetPieceBattleMissionController>().StartBossFight(false);
					this._currentState = FreeTheSeaHoundsCaptivesQuest.FreeTheSeaHoundsCaptivesQuestState.TalkedWithPurigBeforeBossFight;
				})
				.CloseDialog()
				.EndPlayerOptions()
				.CloseDialog();
			DialogFlow dialogFlow4 = DialogFlow.CreateDialogFlow("start", 1200).NpcLine("{=bMaepOl8}Had enough, have you? Well, are you going to honor your word and put us ashore?", new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsPurig|44_12), new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsPlayer|44_9), null, null).Condition(() => Hero.OneToOneConversationHero == NavalStorylineData.Purig && this.BossFightOutCome == Quest5SetPieceBattleMissionController.BossFightOutComeEnum.PlayerDefeatedWaitingForConversation)
				.BeginPlayerOptions(null, false)
				.PlayerOption("{=da9N56ba}You won fairly, Purig. You and your men shall be put ashore.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsPurig|44_12), null, null)
				.NpcLine("{=mnBuBKhI}Good. Perhaps {QUEST_5_COMPANION.NAME} and I will find each other some day and settle things our own way, but you will never see me again.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsPurig|44_12), new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsPlayer|44_9), null, null)
				.Consequence(delegate
				{
					this._isPurigKilledViaConversation = false;
					this.BossFightOutCome = Quest5SetPieceBattleMissionController.BossFightOutComeEnum.PlayerAcceptedTheDuelLostItAndLetPurigGo;
					StringHelpers.SetCharacterProperties("QUEST_5_COMPANION", NavalStorylineData.Gunnar.CharacterObject, null, false);
					Campaign.Current.ConversationManager.ConversationEndOneShot += this.BossFightAftermathConversationWithPurigConsequence;
				})
				.CloseDialog()
				.PlayerOption("{=fsumvsjK}I'll repay your treachery in your own coin. Finish him, lads!", new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsPurig|44_12), null, null)
				.Consequence(delegate
				{
					this._isPurigKilledViaConversation = true;
					this.BossFightOutCome = Quest5SetPieceBattleMissionController.BossFightOutComeEnum.PlayerAcceptedTheDuelLostItAndHadPurigKilledAnyway;
					Campaign.Current.ConversationManager.ConversationEndOneShot += this.BossFightAftermathConversationWithPurigConsequence;
				})
				.CloseDialog()
				.EndPlayerOptions()
				.CloseDialog();
			TextObject textObject = new TextObject("{=FW5OE4fE}{PLAYER.NAME}... {?PLAYER.GENDER}Sister{?}Brother{\\?}... Heaven's mercy, I had given up hope. I thought I'd die in that dark place, in the power of those cruel men.", null);
			TextObjectExtensions.SetCharacterProperties(textObject, "PLAYER", CharacterObject.PlayerCharacter, false);
			TextObject textObject2 = new TextObject("{=6Bx9b4JH}Heaven bless you, {?PLAYER.GENDER}sister{?}brother{\\?}! I am ready to do my part, for our family and our future! But I can see your men calling you. Get us to safety, and we will speak again.", null);
			TextObjectExtensions.SetCharacterProperties(textObject2, "PLAYER", CharacterObject.PlayerCharacter, false);
			TextObjectExtensions.SetCharacterProperties(new TextObject("{=V52pdTgC}{PLAYER.NAME}... I hate to interrupt, but we need to move fast. We've got men badly hurt, and our water stocks are low. My lads won't be leaving any loot behind, though, not after they bled for it. We shall see you in Ostican!", null), "PLAYER", CharacterObject.PlayerCharacter, false);
			string text;
			string text2;
			DialogFlow dialogFlow5 = DialogFlow.CreateDialogFlow("start", 1200).GenerateToken(ref text).GenerateToken(ref text2)
				.NpcLine(textObject, new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsSister|44_10), new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsPlayer|44_9), null, null)
				.Condition(() => this._currentState == FreeTheSeaHoundsCaptivesQuest.FreeTheSeaHoundsCaptivesQuestState.DefeatedPurig)
				.Consequence(delegate
				{
					this.SpawnBjolgur();
					this._currentState = FreeTheSeaHoundsCaptivesQuest.FreeTheSeaHoundsCaptivesQuestState.HeadBackToOstican;
				})
				.BeginPlayerOptions(null, false);
			string text3 = "{=iP0fWuZA}My sister... What you must have gone through...";
			string text4 = text;
			DialogFlow dialogFlow6 = dialogFlow5.PlayerOption(text3, new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsSister|44_10), null, text4);
			string text5 = "{=0vwGcEoV}You're safe now. Rest. We can speak later.";
			string text6 = text;
			DialogFlow dialogFlow7 = dialogFlow6.PlayerOption(text5, new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsSister|44_10), null, text6).EndPlayerOptions().NpcLine("{=CZ6yprOg}That awful night... I awoke to cries and screaming and smoke. Father and mother... I won't speak of it. Some of those villains grabbed me and threw me over a horse. In the camp I saw our little brother, and my heart sank, but I did not see you, and that gave me hope.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsSister|44_10), new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsPlayer|44_9), text, null)
				.NpcLine("{=O5xn66z4}They separated us and took the younger stronger ones to be marched to the coast. They mocked us, telling us that we would be worked until our deaths on some hot island mine or on a frozen shoreline. I told them that you would come after me with an army of warriors and see them all hanged. I did not believe it, though... I just could not bear to have no answer to their taunts.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsSister|44_10), new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsPlayer|44_9), null, null)
				.NpcLine("{=ugyC5nt9}We arrived in Ostican. We were smuggled in by night, as the slave trade was banned by the Vlandian king, though many there clearly profited from it. Eventually Purig came to buy us. He questioned all of us closely, about our families. At first I thought he was trying to find out whether he could get a ransom for us, but no, he was trying to find someone related to you! He feared you, and was keeping me to protect himself from you! That made me proud, despite my misery.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsSister|44_10), new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsPlayer|44_9), null, null)
				.NpcLine("{=rTlhgDi8}They threw me in that cell, where you found me, and we sailed from port to port. Sometimes I could press my ear to the door and I could hear Purig discussing his plans to topple the Nord king and build a pirate empire. And I heard your name again and again, as their schemes were foiled and the noose around his neck grew tighter. And then, just a short while ago, I heard your voice at the door of my cell, and I knew Heaven had answered my prayers!", new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsSister|44_10), new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsPlayer|44_9), null, null)
				.BeginPlayerOptions(null, false);
			string text7 = "{=JUwcYtEY}I would never have given up trying to rescue you, or our little brother or any of us!";
			string text8 = text2;
			DialogFlow dialogFlow8 = dialogFlow7.PlayerOption(text7, new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsSister|44_10), null, text8);
			string text9 = "{=5J3vrPII}Our fortunes have changed. This morning you were a captive, but now you are a lady of rank.";
			string text10 = text2;
			DialogFlow dialogFlow9 = dialogFlow8.PlayerOption(text9, new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsSister|44_10), null, text10).EndPlayerOptions().NpcLine(textObject2, new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsSister|44_10), new ConversationSentence.OnMultipleConversationConsequenceDelegate(FreeTheSeaHoundsCaptivesQuest.<>c.<>9.<SetDialogs>g__IsPlayer|44_9), null, null)
				.Consequence(new ConversationSentence.OnConsequenceDelegate(base.CompleteQuestWithSuccess))
				.CloseDialog();
			Campaign.Current.ConversationManager.AddDialogFlow(dialogFlow, null);
			Campaign.Current.ConversationManager.AddDialogFlow(dialogFlow2, null);
			Campaign.Current.ConversationManager.AddDialogFlow(dialogFlow3, null);
			Campaign.Current.ConversationManager.AddDialogFlow(dialogFlow4, null);
			Campaign.Current.ConversationManager.AddDialogFlow(dialogFlow9, null);
		}

		// Token: 0x06000379 RID: 889 RVA: 0x0001990C File Offset: 0x00017B0C
		private void SpawnBjolgur()
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
			Agent agent = Mission.Current.SpawnAgent(agentBuildData, false);
			Campaign.Current.ConversationManager.AddConversationAgents(new Agent[] { agent }, true);
		}

		// Token: 0x0600037A RID: 890 RVA: 0x000199C0 File Offset: 0x00017BC0
		private void BossFightAftermathConversationWithPurigConsequence()
		{
			this._currentState = FreeTheSeaHoundsCaptivesQuest.FreeTheSeaHoundsCaptivesQuestState.DefeatedPurig;
			TextObject textObject;
			if (this._isPurigKilledViaConversation)
			{
				textObject = new TextObject("{=T76bsVKF}Your men make quick work of Purig and his crew, assured that few will blame them for giving the Sea Hounds a taste of their own villainy. Meanwhile, you return to the roundship, which your men have already begun to search for loot and captives to free. As hopeful cries well up from the hold, they pry open the hatches, and look below.", null);
				TraitLevelingHelper.OnIssueSolvedThroughQuest(Hero.MainHero, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, 50)
				});
			}
			else
			{
				textObject = new TextObject("{=bWFRemi6}Purig and his men jump into the waters of the bay and wade to shore. They disappear into the forested cliffs by the fjord. Meanwhile, you return to the Sea Hounds' roundship, which your men have already begun to search for loot and captives to free. As hopeful cries well up from the hold, they pry open the hatches, and look below.", null);
				TraitLevelingHelper.OnIssueSolvedThroughQuest(Hero.MainHero, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, -50)
				});
				Clan.PlayerClan.AddRenown(50f, true);
			}
			InformationManager.ShowInquiry(new InquiryData(new TextObject("{=fNLTX4VS}Sister Saved", null).ToString(), textObject.ToString(), true, false, GameTexts.FindText("str_ok", null).ToString(), string.Empty, new Action(this.DuelLostPopUpConsequence), null, "", 0f, null, null, null), false, false);
		}

		// Token: 0x0600037B RID: 891 RVA: 0x00019A98 File Offset: 0x00017C98
		private bool GunnarInitialMeetingDialogCondition()
		{
			return this._currentState == FreeTheSeaHoundsCaptivesQuest.FreeTheSeaHoundsCaptivesQuestState.EncounteredWithSeaHoundsParty && Hero.OneToOneConversationHero == NavalStorylineData.Gunnar && NavalStorylineData.IsStorylineActivationPossible() && NavalStorylineData.HasCompletedLast(NavalStorylineData.NavalStorylineStage.Act3Quest4) && Campaign.Current.QuestManager.IsThereActiveQuestWithType(typeof(FreeTheSeaHoundsCaptivesQuest));
		}

		// Token: 0x0600037C RID: 892 RVA: 0x00019AE4 File Offset: 0x00017CE4
		private void GunnarInitialMeetingDialogConsequence()
		{
			this._currentState = FreeTheSeaHoundsCaptivesQuest.FreeTheSeaHoundsCaptivesQuestState.TalkedWithGunnarBeforeFight;
		}

		// Token: 0x0600037D RID: 893 RVA: 0x00019AED File Offset: 0x00017CED
		private void DuelLostPopUpConsequence()
		{
			this.ShowNavalSaveSisterSceneNotification();
		}

		// Token: 0x0600037E RID: 894 RVA: 0x00019AF8 File Offset: 0x00017CF8
		private void AddGameMenus()
		{
			base.AddGameMenu("act_3_quest_5_encounter_menu", new TextObject("{=oPap9pvt}You have arrived at your destination, Angranfjord. The entrance to the inlet between forested crags is hard to spot from the open sea, but Crusas points it out to you. You row forward in Crusas' ship while Bjolgur and Lahar hold back, keeping watch for the Shield Brother reinforcements. Soon you see a cluster of vessels, sitting at anchor. This must be Purig's fleet.", null), new OnInitDelegate(this.game_menu_encounter_on_init), 0, 0);
			base.AddGameMenuOption("act_3_quest_5_encounter_menu", "continue", new TextObject("{=DM6luo3c}Continue", null), new GameMenuOption.OnConditionDelegate(this.encounter_menu_continue_on_condition), new GameMenuOption.OnConsequenceDelegate(this.encounter_menu_continue_on_consequence), false, -1);
			base.AddGameMenu("act_3_quest_5_mission_menu", new TextObject("{=etH1IHNZ}You manage to put some distance between you and your enemies, and you have a moment to consider how to proceed.", null), new OnInitDelegate(this.mission_menu_on_init), 0, 0);
			base.AddGameMenuOption("act_3_quest_5_mission_menu", "checkpoint", new TextObject("{=mBAxWNpo}Try again from last checkpoint", null), new GameMenuOption.OnConditionDelegate(this.encounter_menu_checkpoint_on_condition), new GameMenuOption.OnConsequenceDelegate(this.encounter_menu_checkpoint_on_consequence), false, -1);
			base.AddGameMenuOption("act_3_quest_5_mission_menu", "start_over", new TextObject("{=lvbqEglM}Start over", null), new GameMenuOption.OnConditionDelegate(this.encounter_menu_start_over_on_condition), new GameMenuOption.OnConsequenceDelegate(this.encounter_menu_start_over_on_consequence), false, -1);
			base.AddGameMenuOption("act_3_quest_5_mission_menu", "leave", new TextObject("{=3sRdGQou}Leave", null), new GameMenuOption.OnConditionDelegate(this.encounter_menu_leave_on_condition), new GameMenuOption.OnConsequenceDelegate(this.encounter_menu_leave_on_consequence), true, -1);
		}

		// Token: 0x0600037F RID: 895 RVA: 0x00019C24 File Offset: 0x00017E24
		private void HandleMenuInitState()
		{
			if (this._currentState == FreeTheSeaHoundsCaptivesQuest.FreeTheSeaHoundsCaptivesQuestState.TalkedWithGunnarBeforeFight)
			{
				if (PlayerEncounter.Battle == null)
				{
					PlayerEncounter.StartBattle();
				}
				this.InitializeSetPieceBattleMission(Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase1Part1);
				return;
			}
			if (this._currentState == FreeTheSeaHoundsCaptivesQuest.FreeTheSeaHoundsCaptivesQuestState.DefeatedPurig)
			{
				PlayerEncounter.LeaveEncounter = true;
				GameMenu.ExitToLast();
				if (this.BossFightOutCome != Quest5SetPieceBattleMissionController.BossFightOutComeEnum.PlayerAcceptedTheDuelLostItAndHadPurigKilledAnyway && this.BossFightOutCome != Quest5SetPieceBattleMissionController.BossFightOutComeEnum.PlayerAcceptedTheDuelLostItAndLetPurigGo)
				{
					this.ShowNavalSaveSisterSceneNotification();
					return;
				}
			}
			else if (this._currentState == FreeTheSeaHoundsCaptivesQuest.FreeTheSeaHoundsCaptivesQuestState.PlayerLostBossFight && this.BossFightOutCome == Quest5SetPieceBattleMissionController.BossFightOutComeEnum.PlayerDefeatedWaitingForConversation)
			{
				CampaignMission.OpenConversationMission(new ConversationCharacterData(CharacterObject.PlayerCharacter, PartyBase.MainParty, false, false, false, false, false, false), new ConversationCharacterData(NavalStorylineData.Purig.CharacterObject, this._seaHoundsParty.Party, false, false, false, false, false, false), "", "", false);
			}
		}

		// Token: 0x06000380 RID: 896 RVA: 0x00019CD4 File Offset: 0x00017ED4
		private void mission_menu_on_init(MenuCallbackArgs args)
		{
			args.MenuContext.SetBackgroundMeshName(SettlementHelper.FindNearestHideoutToMobileParty(MobileParty.MainParty, 3, null).WaitMeshName);
			this.HandleMenuInitState();
			NavalStorylineData.OnCheckpointReached(NavalStorylineData.NavalStorylineCheckpoint.Act3Quest5MissionMenu);
		}

		// Token: 0x06000381 RID: 897 RVA: 0x00019CFF File Offset: 0x00017EFF
		private void game_menu_encounter_on_init(MenuCallbackArgs args)
		{
			if (this._lastHitCheckpoint == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.None || this._lastHitCheckpoint == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase1Part1)
			{
				args.MenuContext.SetBackgroundMeshName("encounter_naval");
				this.HandleMenuInitState();
			}
			else
			{
				GameMenu.SwitchToMenu("act_3_quest_5_mission_menu");
			}
			NavalStorylineData.OnCheckpointReached(NavalStorylineData.NavalStorylineCheckpoint.Act3Quest5EncounterMenu);
		}

		// Token: 0x06000382 RID: 898 RVA: 0x00019D3B File Offset: 0x00017F3B
		[GameMenuInitializationHandler("act_3_quest_5_encounter_menu")]
		private static void quest_game_menus_on_init_background(MenuCallbackArgs args)
		{
			args.MenuContext.SetBackgroundMeshName(SettlementHelper.FindNearestHideoutToMobileParty(MobileParty.MainParty, 3, null).WaitMeshName);
		}

		// Token: 0x06000383 RID: 899 RVA: 0x00019D59 File Offset: 0x00017F59
		private bool encounter_menu_continue_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 17;
			return true;
		}

		// Token: 0x06000384 RID: 900 RVA: 0x00019D64 File Offset: 0x00017F64
		private void encounter_menu_continue_on_consequence(MenuCallbackArgs args)
		{
			this._currentState = FreeTheSeaHoundsCaptivesQuest.FreeTheSeaHoundsCaptivesQuestState.EncounteredWithSeaHoundsParty;
			ConversationCharacterData conversationCharacterData = new ConversationCharacterData(CharacterObject.PlayerCharacter, PartyBase.MainParty, false, false, false, false, false, false);
			ConversationCharacterData conversationCharacterData2;
			conversationCharacterData2..ctor(NavalStorylineData.Gunnar.CharacterObject, PartyBase.MainParty, false, false, false, false, false, false);
			CampaignMission.OpenConversationMission(conversationCharacterData, conversationCharacterData2, "", "", false);
			GameMenu.ActivateGameMenu("act_3_quest_5_mission_menu");
		}

		// Token: 0x06000385 RID: 901 RVA: 0x00019DC5 File Offset: 0x00017FC5
		private bool encounter_menu_checkpoint_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 1;
			return this.CanStartFromCheckPoint();
		}

		// Token: 0x06000386 RID: 902 RVA: 0x00019DD4 File Offset: 0x00017FD4
		private void encounter_menu_checkpoint_on_consequence(MenuCallbackArgs args)
		{
			this.InitializeSetPieceBattleMission(this._lastHitCheckpoint);
		}

		// Token: 0x06000387 RID: 903 RVA: 0x00019DE2 File Offset: 0x00017FE2
		private bool encounter_menu_start_over_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 1;
			return !this.CanStartFromCheckPoint();
		}

		// Token: 0x06000388 RID: 904 RVA: 0x00019DF4 File Offset: 0x00017FF4
		private void encounter_menu_start_over_on_consequence(MenuCallbackArgs args)
		{
			this.InitializeSetPieceBattleMission(Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase1Part1);
		}

		// Token: 0x06000389 RID: 905 RVA: 0x00019DFD File Offset: 0x00017FFD
		private bool encounter_menu_leave_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 16;
			return true;
		}

		// Token: 0x0600038A RID: 906 RVA: 0x00019E08 File Offset: 0x00018008
		private void encounter_menu_leave_on_consequence(MenuCallbackArgs args)
		{
			if (MobileParty.MainParty.MapEvent != null)
			{
				MenuHelper.EncounterLeaveConsequence();
			}
			NavalStorylineData.DeactivateNavalStoryline();
			GameMenu.ExitToLast();
		}

		// Token: 0x0600038B RID: 907 RVA: 0x00019E28 File Offset: 0x00018028
		private void InitializeSetPieceBattleMission(Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState checkpoint = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase1Part1)
		{
			if (NavalStorylineData.Purig.PartyBelongedTo != this._seaHoundsParty && !NavalStorylineData.Purig.IsDead)
			{
				if (NavalStorylineData.Purig.HeroState != 1)
				{
					NavalStorylineData.Purig.ChangeState(1);
				}
				this._seaHoundsParty.Party.MemberRoster.AddToCounts(NavalStorylineData.Purig.CharacterObject, 1, false, 0, 0, true, -1);
			}
			NavalMissions.OpenNavalStorylineQuest5SetPieceBattleMission(NavalStorylineData.GetNavalMissionInitializerTemplate("naval_storyline_act_3_quest_5"), this._seaHoundsParty, checkpoint);
		}

		// Token: 0x0600038C RID: 908 RVA: 0x00019EA8 File Offset: 0x000180A8
		protected override void OnStartQuestInternal()
		{
			base.AddLog(this._findSeaHoundsQuestLog, false);
			this.CreateSeaHoundParty();
			base.AddTrackedObject(this._skatriaIslandsMarker);
			foreach (Ship ship in MobileParty.MainParty.Ships)
			{
				if (ship.ShipHull.StringId == "nord_medium_ship")
				{
					ship.ChangeFigurehead(DefaultFigureheads.Raven);
					using (List<KeyValuePair<string, string>>.Enumerator enumerator2 = this._nordMediumShipyShipUpgradePieceList.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							KeyValuePair<string, string> keyValuePair = enumerator2.Current;
							if (!string.IsNullOrEmpty(keyValuePair.Value))
							{
								ShipUpgradePiece @object = MBObjectManager.Instance.GetObject<ShipUpgradePiece>(keyValuePair.Value);
								ship.EquipUpgradePiece(keyValuePair.Key, @object);
							}
						}
						continue;
					}
				}
				if (ship.ShipHull.StringId == "aserai_heavy_ship")
				{
					foreach (KeyValuePair<string, string> keyValuePair2 in this._aseraiHeavyShipUpgradePieceList)
					{
						if (!string.IsNullOrEmpty(keyValuePair2.Value))
						{
							ShipUpgradePiece object2 = MBObjectManager.Instance.GetObject<ShipUpgradePiece>(keyValuePair2.Value);
							ship.EquipUpgradePiece(keyValuePair2.Key, object2);
						}
					}
				}
			}
		}

		// Token: 0x0600038D RID: 909 RVA: 0x0001A058 File Offset: 0x00018258
		private void OnPartyVisibilityChanged(PartyBase party)
		{
			if (this._currentState == FreeTheSeaHoundsCaptivesQuest.FreeTheSeaHoundsCaptivesQuestState.GoToSeaHoundPartyPosition && party == this._seaHoundsParty.Party && this._seaHoundsParty.IsVisible)
			{
				base.AddLog(this._arrivedAngranfjordQuestLog, false);
			}
		}

		// Token: 0x0600038E RID: 910 RVA: 0x0001A08C File Offset: 0x0001828C
		private void CanHeroBecomePrisoner(Hero hero, ref bool result)
		{
			if (hero == Hero.MainHero)
			{
				result = false;
			}
		}

		// Token: 0x0600038F RID: 911 RVA: 0x0001A099 File Offset: 0x00018299
		private void OnMapEventEnded(MapEvent mapEvent)
		{
			if (MobileParty.MainParty.MapEvent == mapEvent && mapEvent.HasWinner)
			{
				BattleSideEnum winningSide = mapEvent.WinningSide;
				BattleSideEnum playerSide = mapEvent.PlayerSide;
			}
		}

		// Token: 0x06000390 RID: 912 RVA: 0x0001A0C0 File Offset: 0x000182C0
		private void OnHourlyTick()
		{
			if (this._skatriaIslandsMarker.Position.Distance(MobileParty.MainParty.Position.AsVec3()) > 15f)
			{
				this._skatriaIslandsMarker.IsVisibleOnMap = true;
				return;
			}
			this._skatriaIslandsMarker.IsVisibleOnMap = false;
		}

		// Token: 0x06000391 RID: 913 RVA: 0x0001A114 File Offset: 0x00018314
		private void OnMissionEnded(IMission mission)
		{
			Quest5SetPieceBattleMissionController missionBehavior = ((Mission)mission).GetMissionBehavior<Quest5SetPieceBattleMissionController>();
			if (missionBehavior != null)
			{
				this.BossFightOutCome = missionBehavior.BossFightOutCome;
				this._lastHitCheckpoint = missionBehavior.LastHitCheckpoint;
				this._shouldMissionContinueFromCheckpoint = missionBehavior.ShouldMissionContinueFromCheckpoint;
			}
			if (this._lastHitCheckpoint != Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.None && this._lastHitCheckpoint < Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.End)
			{
				this._currentState = FreeTheSeaHoundsCaptivesQuest.FreeTheSeaHoundsCaptivesQuestState.RestartMission;
			}
			if (this._currentState > FreeTheSeaHoundsCaptivesQuest.FreeTheSeaHoundsCaptivesQuestState.TalkedWithGunnarBeforeFight)
			{
				if (this._currentState == FreeTheSeaHoundsCaptivesQuest.FreeTheSeaHoundsCaptivesQuestState.TalkedWithPurigBeforeBossFight && this.BossFightOutCome == Quest5SetPieceBattleMissionController.BossFightOutComeEnum.PlayerDefeatedWaitingForConversation)
				{
					this._currentState = FreeTheSeaHoundsCaptivesQuest.FreeTheSeaHoundsCaptivesQuestState.PlayerLostBossFight;
					return;
				}
				if (PlayerEncounter.EncounteredMobileParty == this._seaHoundsParty && MapEvent.PlayerMapEvent != null && MapEvent.PlayerMapEvent.HasWinner && MapEvent.PlayerMapEvent.WinningSide == mission.PlayerTeam.Side)
				{
					this._currentState = FreeTheSeaHoundsCaptivesQuest.FreeTheSeaHoundsCaptivesQuestState.DefeatedPurig;
				}
			}
		}

		// Token: 0x06000392 RID: 914 RVA: 0x0001A1D3 File Offset: 0x000183D3
		protected override void OnFinalizeInternal()
		{
			this.DestroySeaHoundParty();
			if (NavalStorylineData.Purig.IsAlive)
			{
				KillCharacterAction.ApplyByRemove(NavalStorylineData.Purig, false, true);
			}
		}

		// Token: 0x06000393 RID: 915 RVA: 0x0001A1F3 File Offset: 0x000183F3
		protected override void OnCompleteWithSuccessInternal()
		{
			NavalStorylineData.OnCheckpointReached(NavalStorylineData.NavalStorylineCheckpoint.Act3Quest5Succeeded);
		}

		// Token: 0x06000394 RID: 916 RVA: 0x0001A1FC File Offset: 0x000183FC
		private void CreateSeaHoundParty()
		{
			Hideout hideout = SettlementHelper.FindNearestHideoutToMobileParty(MobileParty.MainParty, 3, (Settlement x) => x.IsActive);
			Clan clan = Clan.All.FirstOrDefault<Clan>((Clan x) => x.StringId == "northern_pirates");
			PartyTemplateObject partyTemplateObject = Campaign.Current.ObjectManager.GetObject<PartyTemplateObject>("storyline_act3_quest_5_sea_hounds_template") ?? clan.DefaultPartyTemplate;
			this._seaHoundsParty = BanditPartyComponent.CreateBanditParty("free_the_sea_hounds_captives_initial_quest_party", clan, hideout.Settlement.Hideout, false, partyTemplateObject, this._seaHoundsSpawnPosition);
			this._seaHoundsParty.Party.SetCustomName(new TextObject("{=SKC3FeGR}Sea Hounds", null));
			this._seaHoundsParty.SetPartyUsedByQuest(true);
			this._seaHoundsParty.IsInfoHidden = true;
			this._seaHoundsParty.IgnoreByOtherPartiesTill(CampaignTime.Years(999f));
			this._seaHoundsParty.SetLandNavigationAccess(false);
			this._seaHoundsParty.Ai.SetDoNotMakeNewDecisions(true);
			this._seaHoundsParty.Party.SetCustomBanner(NavalStorylineData.CorsairBanner);
			MobileParty.UpdateLocator(this._seaHoundsParty);
			this._seaHoundsParty.MemberRoster.Clear();
			FreeTheSeaHoundsCaptivesQuest.FillParty(this._seaHoundsParty, partyTemplateObject, MathF.Round(67f * this._strengthModifier));
			base.AddTrackedObject(this._seaHoundsParty);
			foreach (Ship ship in this._seaHoundsParty.Ships)
			{
				ship.ChangeFigurehead(DefaultFigureheads.Dragon);
				foreach (KeyValuePair<string, string> keyValuePair in this._seaHoundPartyShipUpgradePieceList)
				{
					if (!string.IsNullOrEmpty(keyValuePair.Value))
					{
						ShipUpgradePiece @object = MBObjectManager.Instance.GetObject<ShipUpgradePiece>(keyValuePair.Value);
						ship.EquipUpgradePiece(keyValuePair.Key, @object);
					}
				}
			}
		}

		// Token: 0x06000395 RID: 917 RVA: 0x0001A420 File Offset: 0x00018620
		private void DestroySeaHoundParty()
		{
			if (this._seaHoundsParty != null && this._seaHoundsParty.IsActive)
			{
				DestroyPartyAction.Apply(null, this._seaHoundsParty);
			}
		}

		// Token: 0x06000396 RID: 918 RVA: 0x0001A444 File Offset: 0x00018644
		private static void FillParty(MobileParty mobileParty, PartyTemplateObject partyTemplate, int desiredMenCount)
		{
			int num = partyTemplate.Stacks.Sum<PartyTemplateStack>((PartyTemplateStack s) => s.MinValue);
			int num2 = partyTemplate.Stacks.Sum<PartyTemplateStack>((PartyTemplateStack s) => s.MaxValue);
			float num3;
			if (desiredMenCount < num)
			{
				num3 = (float)desiredMenCount / (float)num - 1f;
			}
			else if (num <= desiredMenCount && desiredMenCount <= num2)
			{
				num3 = (float)(desiredMenCount - num) / (float)(num2 - num);
			}
			else
			{
				num3 = (float)desiredMenCount / (float)num2;
			}
			for (int i = 0; i < partyTemplate.Stacks.Count; i++)
			{
				PartyTemplateStack partyTemplateStack = partyTemplate.Stacks[i];
				int minValue = partyTemplateStack.MinValue;
				int maxValue = partyTemplateStack.MaxValue;
				int num4;
				if (-1f <= num3 && num3 < 0f)
				{
					num4 = MBRandom.RoundRandomized((float)minValue + (float)minValue * num3);
				}
				else if (0f <= num3 && num3 <= 1f)
				{
					num4 = MBRandom.RoundRandomized((float)minValue + (float)(maxValue - minValue) * num3);
				}
				else
				{
					num4 = MBRandom.RoundRandomized((float)maxValue * num3);
				}
				if (num4 > 0)
				{
					mobileParty.MemberRoster.AddToCounts(partyTemplateStack.Character, num4, false, 0, 0, true, -1);
				}
			}
			while (mobileParty.MemberRoster.TotalManCount > desiredMenCount)
			{
				int num5 = MBRandom.RoundRandomized(MBRandom.RandomFloatRanged((float)(partyTemplate.Stacks.Count - 1)));
				CharacterObject character = partyTemplate.Stacks[num5].Character;
				mobileParty.MemberRoster.AddToCounts(character, -1, false, 0, 0, true, -1);
			}
			while (mobileParty.MemberRoster.TotalManCount < desiredMenCount)
			{
				int num6 = MBRandom.RoundRandomized(MBRandom.RandomFloatRanged((float)(partyTemplate.Stacks.Count - 1)));
				CharacterObject character2 = partyTemplate.Stacks[num6].Character;
				mobileParty.MemberRoster.AddToCounts(character2, 1, false, 0, 0, true, -1);
			}
		}

		// Token: 0x06000397 RID: 919 RVA: 0x0001A62B File Offset: 0x0001882B
		private void ShowNavalSaveSisterSceneNotification()
		{
			if (this._isSisterSavedSceneNotificationTriggered)
			{
				return;
			}
			MBInformationManager.ShowSceneNotification(new NavalSaveSisterSceneNotificationItem(Hero.MainHero, StoryModeHeroes.LittleSister, new Action(this.OnNavalSaveSisterSceneNotificationClosed)));
			this._isSisterSavedSceneNotificationTriggered = true;
		}

		// Token: 0x06000398 RID: 920 RVA: 0x0001A660 File Offset: 0x00018860
		private void OnNavalSaveSisterSceneNotificationClosed()
		{
			ConversationCharacterData conversationCharacterData = new ConversationCharacterData(CharacterObject.PlayerCharacter, PartyBase.MainParty, true, true, false, false, false, true);
			ConversationCharacterData conversationCharacterData2;
			conversationCharacterData2..ctor(StoryModeHeroes.LittleSister.CharacterObject, PartyBase.MainParty, true, true, false, true, false, true);
			CampaignMission.OpenConversationMission(conversationCharacterData, conversationCharacterData2, "conversation_scene_sea_multi_agent", "", true);
		}

		// Token: 0x06000399 RID: 921 RVA: 0x0001A6B0 File Offset: 0x000188B0
		private void ShowAllyDefeatedPopUp()
		{
			object obj = new TextObject("{=cH3Kpkwg}Ally Defeated", null);
			TextObject textObject = new TextObject("{=DM6luo3c}Continue", null);
			InformationManager.ShowInquiry(new InquiryData(obj.ToString(), this._allyDefeatedText.ToString(), true, false, textObject.ToString(), null, new Action(this.OnAllyDefeatedPopUpClosed), null, "", 0f, null, null, null), true, false);
		}

		// Token: 0x0600039A RID: 922 RVA: 0x0001A713 File Offset: 0x00018913
		private void OnAllyDefeatedPopUpClosed()
		{
			base.CompleteQuestWithCancel(this._allyDefeatedText);
			NavalStorylineData.DeactivateNavalStoryline();
		}

		// Token: 0x0600039B RID: 923 RVA: 0x0001A726 File Offset: 0x00018926
		private bool CanStartFromCheckPoint()
		{
			return this._lastHitCheckpoint != Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.None && this._lastHitCheckpoint != Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase1Part1;
		}

		// Token: 0x04000217 RID: 535
		private const int PlayerLostDuelAndLetPurigGoHonorBonus = 50;

		// Token: 0x04000218 RID: 536
		private const int PlayerLostDuelAndKilledPurigHonorPenalty = -50;

		// Token: 0x04000219 RID: 537
		private const int PlayerLostDuelAndKilledPurigRenownBonus = 50;

		// Token: 0x0400021A RID: 538
		private const string SeaHoundSetPieceBattlePartyTemplateString = "storyline_act3_quest_5_sea_hounds_set_piece_battle_template";

		// Token: 0x0400021B RID: 539
		private const string SeaHoundPartyTemplateStringId = "storyline_act3_quest_5_sea_hounds_template";

		// Token: 0x0400021C RID: 540
		private const string EncounterMenuId = "act_3_quest_5_encounter_menu";

		// Token: 0x0400021D RID: 541
		private const string MissionMenuId = "act_3_quest_5_mission_menu";

		// Token: 0x0400021E RID: 542
		private const string SetPieceBattleSceneName = "naval_storyline_act_3_quest_5";

		// Token: 0x0400021F RID: 543
		private const int SeaHoundPartySize = 67;

		// Token: 0x04000220 RID: 544
		private const string NordMediumShipStringId = "nord_medium_ship";

		// Token: 0x04000221 RID: 545
		private const string AseraiHeavyShipStringId = "aserai_heavy_ship";

		// Token: 0x04000222 RID: 546
		[SaveableField(1)]
		private MobileParty _seaHoundsParty;

		// Token: 0x04000223 RID: 547
		private bool _shouldMissionContinueFromCheckpoint;

		// Token: 0x04000224 RID: 548
		[SaveableField(0)]
		private FreeTheSeaHoundsCaptivesQuest.FreeTheSeaHoundsCaptivesQuestState _currentState;

		// Token: 0x04000225 RID: 549
		[SaveableField(7)]
		private float _strengthModifier;

		// Token: 0x04000226 RID: 550
		private bool _isPurigKilledViaConversation;

		// Token: 0x04000227 RID: 551
		private bool _isSisterSavedSceneNotificationTriggered;

		// Token: 0x04000228 RID: 552
		[SaveableField(12)]
		private readonly MapMarker _skatriaIslandsMarker;

		// Token: 0x04000229 RID: 553
		[SaveableField(13)]
		private Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState _lastHitCheckpoint;

		// Token: 0x0400022A RID: 554
		[SaveableField(14)]
		public Quest5SetPieceBattleMissionController.BossFightOutComeEnum BossFightOutCome;

		// Token: 0x0400022B RID: 555
		private readonly List<KeyValuePair<string, string>> _seaHoundPartyShipUpgradePieceList = new List<KeyValuePair<string, string>>
		{
			new KeyValuePair<string, string>("sail", "sails_lvl2"),
			new KeyValuePair<string, string>("side", "side_northern_shields_lvl2")
		};

		// Token: 0x0400022C RID: 556
		private readonly List<KeyValuePair<string, string>> _nordMediumShipyShipUpgradePieceList = new List<KeyValuePair<string, string>>
		{
			new KeyValuePair<string, string>("sail", "sails_lvl2"),
			new KeyValuePair<string, string>("side", "side_northern_shields_lvl2")
		};

		// Token: 0x0400022D RID: 557
		private readonly List<KeyValuePair<string, string>> _aseraiHeavyShipUpgradePieceList = new List<KeyValuePair<string, string>>
		{
			new KeyValuePair<string, string>("fore", "fore_ballista"),
			new KeyValuePair<string, string>("aft", "aft_battlement_lvl3_wbarracks"),
			new KeyValuePair<string, string>("deck", "deck_arrow_and_javelin_crates_lvl2"),
			new KeyValuePair<string, string>("sail", "sails_lvl2")
		};

		// Token: 0x020001B2 RID: 434
		public enum FreeTheSeaHoundsCaptivesQuestState
		{
			// Token: 0x04000CFD RID: 3325
			None,
			// Token: 0x04000CFE RID: 3326
			RestartMission,
			// Token: 0x04000CFF RID: 3327
			GoToSeaHoundPartyPosition,
			// Token: 0x04000D00 RID: 3328
			EncounteredWithSeaHoundsParty,
			// Token: 0x04000D01 RID: 3329
			TalkedWithGunnarBeforeFight,
			// Token: 0x04000D02 RID: 3330
			TalkedWithPurigBeforeBossFight,
			// Token: 0x04000D03 RID: 3331
			PlayerLostBossFight,
			// Token: 0x04000D04 RID: 3332
			DefeatedPurig,
			// Token: 0x04000D05 RID: 3333
			HeadBackToOstican
		}
	}
}
