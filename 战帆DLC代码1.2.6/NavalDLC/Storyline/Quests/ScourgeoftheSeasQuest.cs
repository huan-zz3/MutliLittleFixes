using System;
using NavalDLC.Storyline.CampaignBehaviors;
using NavalDLC.Storyline.MissionControllers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace NavalDLC.Storyline.Quests
{
	// Token: 0x0200003E RID: 62
	public class ScourgeoftheSeasQuest : QuestBase
	{
		// Token: 0x06000460 RID: 1120 RVA: 0x0001DAA5 File Offset: 0x0001BCA5
		public ScourgeoftheSeasQuest()
			: base("scourge_of_the_seas", NavalStorylineData.Gunnar, CampaignTime.Never, 0)
		{
		}

		// Token: 0x170000BC RID: 188
		// (get) Token: 0x06000461 RID: 1121 RVA: 0x0001DABD File Offset: 0x0001BCBD
		public override TextObject Title
		{
			get
			{
				return new TextObject("{=1EJ1kav2}Scourge of the Seas", null);
			}
		}

		// Token: 0x170000BD RID: 189
		// (get) Token: 0x06000462 RID: 1122 RVA: 0x0001DACA File Offset: 0x0001BCCA
		public override bool IsRemainingTimeHidden
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x06000463 RID: 1123 RVA: 0x0001DACD File Offset: 0x0001BCCD
		public override string SpecialQuestType
		{
			get
			{
				return "NavalStoryline";
			}
		}

		// Token: 0x06000464 RID: 1124 RVA: 0x0001DAD4 File Offset: 0x0001BCD4
		protected override void OnStartQuest()
		{
			if (NavalStorylineData.IsTutorialSkipped())
			{
				this.UpdateProgress(new TextObject("{=PMfKcz6o}You met a Nord warrior named Gunnar, helping him defeat an ambush in a back alley of Ostican. He suggested that you join forces with him to battle the Sea Hounds, a pirate confederacy, and in doing so that you might learn something about your sister. You declined, and he told you that you might find him in Ostican if you ever changed your mind.", null));
			}
		}

		// Token: 0x06000465 RID: 1125 RVA: 0x0001DAF0 File Offset: 0x0001BCF0
		protected override void RegisterEvents()
		{
			CampaignEvents.OnQuestCompletedEvent.AddNonSerializedListener(this, new Action<QuestBase, QuestBase.QuestCompleteDetails>(this.OnQuestCompleted));
			NavalDLCEvents.OnNavalStorylineCanceledEvent.AddNonSerializedListener(this, new Action<NavalStorylineData.StorylineCancelDetail>(this.OnStorylineCanceled));
			NavalDLCEvents.OnSisterRansomRequestedEvent.AddNonSerializedListener(this, new Action(this.OnSisterRansomRequested));
			NavalDLCEvents.OnSisterRansomedEvent.AddNonSerializedListener(this, new Action(this.OnSisterRansomed));
		}

		// Token: 0x06000466 RID: 1126 RVA: 0x0001DB59 File Offset: 0x0001BD59
		private void OnStorylineCanceled(NavalStorylineData.StorylineCancelDetail detail)
		{
			if (detail == NavalStorylineData.StorylineCancelDetail.ByRansom)
			{
				base.AddLog(new TextObject("{=lR4a9LMR}You paid the Sea Hounds a ransom for your sister, and were reunited with her. You took your leave of Gunnar. He will continue his war against them, but you are no longer part of it.", null), false);
				base.CompleteQuestWithSuccess();
			}
		}

		// Token: 0x06000467 RID: 1127 RVA: 0x0001DB78 File Offset: 0x0001BD78
		private void OnSisterRansomed()
		{
			base.AddLog(new TextObject("{=kOZcZcaR}Gunnar has informed you that he has ransomed your sister, and is waiting for you at Ostican.", null), false);
		}

		// Token: 0x06000468 RID: 1128 RVA: 0x0001DB8D File Offset: 0x0001BD8D
		private void OnSisterRansomRequested()
		{
			this.RemoveGunnarLog();
			base.AddLog(new TextObject("{=UwEzZB5K}You have asked Gunnar to contact the Sea Hounds with an offer to ransom your sister. He will send you a message when he has done so.", null), false);
		}

		// Token: 0x06000469 RID: 1129 RVA: 0x0001DBA8 File Offset: 0x0001BDA8
		private void OnQuestCompleted(QuestBase quest, QuestBase.QuestCompleteDetails details)
		{
			if (details == 1)
			{
				if (quest is ReturnToBaseQuest)
				{
					switch (NavalStorylineData.GetStorylineStage())
					{
					case NavalStorylineData.NavalStorylineStage.Act1:
						this.UpdateProgress(new TextObject("{=7HVntRW9}You met a Nord warrior named Gunnar, helping him defeat an ambush in a back alley of Ostican. You decided to join forces with him to battle the Sea Hounds, a pirate confederacy, and hunt for your sister. But no sooner did you set out with him then the two of you were betrayed by his old comrade Purig, who had joined the Sea Hounds. His men were lax, however, and you were able to break out of captivity and make off with one of his ships.", null));
						return;
					case NavalStorylineData.NavalStorylineStage.Act2:
						this.UpdateProgress(new TextObject("{=vcOPDZ83}Together with Gunnar's kin, you sailed forth in his longship and defeated two Sea Hound vessels lying in wait off of Ostican. You took a prisoner who revealed to you that the Sea Hounds were involved in trading slaves, giving you your first clue in the hunt for your sister.", null));
						return;
					case NavalStorylineData.NavalStorylineStage.Act3Quest1:
						this.UpdateQuest1Progress();
						return;
					case NavalStorylineData.NavalStorylineStage.Act3Quest2:
						this.UpdateProgress(new TextObject("{=UrAFO5ve}Gunnar introduced you to an Aserai sea-captain named Lahar. He was pursuing one of the Sea Hounds' allies, the Emira al-Fahda, and together you set out in search of your fleet. You brought Fahda to battle in the stormy seas off Charas and took her prisoner. She told of Purig's plans to become leader of the Sea Hounds and in exchange for her life revealed to you his next target, a Sturgian silver-ship.", null));
						return;
					case NavalStorylineData.NavalStorylineStage.Act3SpeakToSailors:
						this.UpdateProgress(new TextObject("{=5O51VPFJ}Acting on what you learned from Fahda, you and Gunnar sailed to the Sturgian port of Omor. There, you met another of Gunnar's old comrades, Bjolgur, a member of the Skolderbroda mercenary brotherhood. Bjolgur hoped to run the silver-ship past the Sea Hounds and enlisted you in his dangerous but effective plan to crash a fireship into their blockading fleet. In exchange, Bjolgur gave you information about a key Sea Hound ally, the imperial merchant Crusas, who he suspected would know more about your sister.", null));
						return;
					case NavalStorylineData.NavalStorylineStage.Act3Quest4:
						this.UpdateProgress(new TextObject("{=Mpf7S1ED}You and Gunnar sailed to the barren Skatria islands, where you found Crusas' fleet. He had lashed his ships together into a floating fortress, but you led your fleet in to storm it and took him prisoner. Crusas revealed to you that Purig was holding your sister as a hostage, and could likely be found at his base in a northern fjord. You made plans to attack Purig, using Crusas to allay his lookouts' suspicions as you sailed in close to rescue your sister.", null));
						return;
					case NavalStorylineData.NavalStorylineStage.Act3Quest5:
						this.UpdateFinalQuestProgress();
						return;
					default:
						Debug.FailedAssert("None state is wrong.", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\Storyline\\Quests\\ScourgeoftheSeasQuest.cs", "OnQuestCompleted", 92);
						return;
					}
				}
				else
				{
					if (quest is SpeakToGunnarAndSisterQuest)
					{
						this.UpdateOutro();
						return;
					}
					FreeTheSeaHoundsCaptivesQuest freeTheSeaHoundsCaptivesQuest;
					if ((freeTheSeaHoundsCaptivesQuest = quest as FreeTheSeaHoundsCaptivesQuest) != null)
					{
						this.UpdateAlternativeFinalQuestProgress(freeTheSeaHoundsCaptivesQuest.BossFightOutCome);
						this.UpdateOutro();
					}
				}
			}
		}

		// Token: 0x0600046A RID: 1130 RVA: 0x0001DC9B File Offset: 0x0001BE9B
		private void UpdateQuest1Progress()
		{
			if (NavalStorylineData.IsTutorialSkipped())
			{
				this.UpdateProgress(new TextObject("{=pwVjDfo1}You spoke again to Gunnar in Ostican. He told you that he had been betrayed by an old comrade, Purig, but escaped and learned more about the Sea Hounds' activities. You sailed with Gunnar and his kin to escort a Vlandian merchantman on its homeward voyage from Beinland, hoping that its rich cargo would entice the Sea Hounds into battle. You were not disappointed. Together with the merchants you defeated the attackers, dealing the Sea Hounds a heavy blow.", null));
				return;
			}
			this.UpdateProgress(new TextObject("{=HnNsNEtE}You and Gunnar offered to escort a Vlandian merchantman on its homeward voyage from Beinland, hoping that its rich cargo would entice the Sea Hounds into battle. You were not disappointed. Together with the merchants you defeated the attackers, dealing the Sea Hounds a heavy blow.", null));
		}

		// Token: 0x0600046B RID: 1131 RVA: 0x0001DCC7 File Offset: 0x0001BEC7
		private void UpdateOutro()
		{
			this.RemoveGunnarLog();
			base.AddLog(new TextObject("{=H5FJWA7W}You bid farewell to Gunnar. He returned to his home in Lagshofn, in Beinland, where hopes that some day you might visit him. Your sister, recovered from her ordeal, stands ready to join you and the rest of your clan as you continue to forge your destiny in Calradia.", null), false);
			base.CompleteQuestWithSuccess();
		}

		// Token: 0x0600046C RID: 1132 RVA: 0x0001DCE8 File Offset: 0x0001BEE8
		private void UpdateAlternativeFinalQuestProgress(Quest5SetPieceBattleMissionController.BossFightOutComeEnum outcome)
		{
			Campaign.Current.GetCampaignBehavior<NavalStorylineThirdActFifthQuestBehaviour>();
			TextObject textObject = TextObject.GetEmpty();
			switch (outcome)
			{
			case Quest5SetPieceBattleMissionController.BossFightOutComeEnum.PlayerRefusedTheDuel:
			case Quest5SetPieceBattleMissionController.BossFightOutComeEnum.PlayerAcceptedTheDuelLostItAndHadPurigKilledAnyway:
				textObject = new TextObject("{=Y8lIANDP}You arrived at Angranfjord and put your plan into action. While Crusas bantered with Purig's men, you and Gunnar swum to his prisoner ship and overcame the guards, sailing your sister to safety. Together with Lahar and Bjolgur you then engaged the Sea Hound fleet in a battle in the fjord. In the end, you fought Purig himself on his flagship and had your men cut him down. You ended the Sea Hounds' reign of terror, and set your sister free.", null);
				break;
			case Quest5SetPieceBattleMissionController.BossFightOutComeEnum.PlayerAcceptedAndWonTheDuel:
				textObject = new TextObject("{=an8yoGf1}You arrived at Angranfjord and put your plan into action. While Crusas bantered with Purig's men, you and Gunnar swum to his prisoner ship and overcame the guards, sailing your sister to safety. Together with Lahar and Bjolgur you then engaged the Sea Hound fleet in a battle in the fjord. In the end, you fought Purig himself on his flagship and defeated him in a duel. You ended the Sea Hounds' reign of terror, and set your sister free.", null);
				break;
			case Quest5SetPieceBattleMissionController.BossFightOutComeEnum.PlayerDefeatedWaitingForConversation:
				Debug.FailedAssert("Invalid case", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\Storyline\\Quests\\ScourgeoftheSeasQuest.cs", "UpdateAlternativeFinalQuestProgress", 146);
				break;
			case Quest5SetPieceBattleMissionController.BossFightOutComeEnum.PlayerAcceptedTheDuelLostItAndLetPurigGo:
				textObject = new TextObject("{=Y0H43ait}You arrived at Angranfjord and put your plan into action. While Crusas bantered with Purig's men, you and Gunnar swum to his prisoner ship and overcame the guards, sailing your sister to safety. Together with Lahar and Bjolgur you then engaged the Sea Hound fleet in a battle in the fjord. In the end, you fought Purig himself on his flagship but spared his life after he bested you in one-to-one combat. You ended the Sea Hounds' reign of terror, and set your sister free.", null);
				break;
			}
			this.RemoveGunnarLog();
			base.AddLog(textObject, false);
		}

		// Token: 0x0600046D RID: 1133 RVA: 0x0001DD78 File Offset: 0x0001BF78
		private void UpdateFinalQuestProgress()
		{
			NavalStorylineThirdActFifthQuestBehaviour campaignBehavior = Campaign.Current.GetCampaignBehavior<NavalStorylineThirdActFifthQuestBehaviour>();
			TextObject textObject = TextObject.GetEmpty();
			switch (campaignBehavior.GetBossFightOutcome())
			{
			case Quest5SetPieceBattleMissionController.BossFightOutComeEnum.PlayerRefusedTheDuel:
			case Quest5SetPieceBattleMissionController.BossFightOutComeEnum.PlayerAcceptedTheDuelLostItAndHadPurigKilledAnyway:
				textObject = new TextObject("{=LxfXj7qE}You arrived at Angranfjord and put your plan into action. While Crusas bantered with Purig's men, you and Gunnar swum to his prisoner ship and overcame the guards, sailing your sister to safety. Together with Lahar and Bjolgur you then engaged the Sea Hound fleet in a battle in the fjord. In the end, you fought Purig himself on his flagship and had your men cut him down. You ended the Sea Hounds' reign of terror, and set your sister free. Gunnar awaits you in Ostican to say his farewells.", null);
				break;
			case Quest5SetPieceBattleMissionController.BossFightOutComeEnum.PlayerAcceptedAndWonTheDuel:
				textObject = new TextObject("{=8aiE63ie}You arrived at Angranfjord and put your plan into action. While Crusas bantered with Purig's men, you and Gunnar swum to his prisoner ship and overcame the guards, sailing your sister to safety. Together with Lahar and Bjolgur you then engaged the Sea Hound fleet in a battle in the fjord. In the end, you fought Purig himself on his flagship and defeated him in a duel. You ended the Sea Hounds' reign of terror, and set your sister free. Gunnar awaits you in Ostican to say his farewells.", null);
				break;
			case Quest5SetPieceBattleMissionController.BossFightOutComeEnum.PlayerDefeatedWaitingForConversation:
				Debug.FailedAssert("Invalid case", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\Storyline\\Quests\\ScourgeoftheSeasQuest.cs", "UpdateFinalQuestProgress", 175);
				break;
			case Quest5SetPieceBattleMissionController.BossFightOutComeEnum.PlayerAcceptedTheDuelLostItAndLetPurigGo:
				textObject = new TextObject("{=sUvAbm1a}You arrived at Angranfjord and put your plan into action. While Crusas bantered with Purig's men, you and Gunnar swum to his prisoner ship and overcame the guards, sailing your sister to safety. Together with Lahar and Bjolgur you then engaged the Sea Hound fleet in a battle in the fjord. In the end, you fought Purig himself on his flagship but spared his life after he bested you in one-to-one combat. You ended the Sea Hounds' reign of terror, and set your sister free. Gunnar awaits you in Ostican to say his farewells.", null);
				break;
			}
			this.RemoveGunnarLog();
			base.AddLog(textObject, false);
		}

		// Token: 0x0600046E RID: 1134 RVA: 0x0001DE0B File Offset: 0x0001C00B
		protected override void InitializeQuestOnGameLoad()
		{
		}

		// Token: 0x0600046F RID: 1135 RVA: 0x0001DE0D File Offset: 0x0001C00D
		protected override void SetDialogs()
		{
		}

		// Token: 0x06000470 RID: 1136 RVA: 0x0001DE0F File Offset: 0x0001C00F
		private void UpdateProgress(TextObject log)
		{
			base.AddLog(log, false);
			this.UpdateGunnarLog();
		}

		// Token: 0x06000471 RID: 1137 RVA: 0x0001DE20 File Offset: 0x0001C020
		private void RemoveGunnarLog()
		{
			if (this._gunnarJournal != null)
			{
				base.RemoveLog(this._gunnarJournal);
			}
			this._gunnarJournal = null;
		}

		// Token: 0x06000472 RID: 1138 RVA: 0x0001DE3D File Offset: 0x0001C03D
		private void UpdateGunnarLog()
		{
			this.RemoveGunnarLog();
			this._gunnarJournal = base.AddLog(new TextObject("{=vT1aPyAo}Gunnar awaits you in Ostican when you are ready to embark again.", null), false);
		}

		// Token: 0x0400025E RID: 606
		[SaveableField(0)]
		private JournalLog _gunnarJournal;
	}
}
