using System;
using System.Linq;
using Helpers;
using NavalDLC.Storyline.MissionControllers;
using SandBox.Conversation.MissionLogics;
using StoryMode.StoryModeObjects;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace NavalDLC.Storyline.Quests
{
	// Token: 0x02000041 RID: 65
	public class SpeakToGunnarAndSisterQuest : QuestBase
	{
		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x060004BE RID: 1214 RVA: 0x0001F809 File Offset: 0x0001DA09
		private TextObject _startLog
		{
			get
			{
				TextObject textObject = new TextObject("{=vhqRTs5p}Look for {GUNNAR.NAME} and your sister in Ostican harbor.", null);
				TextObjectExtensions.SetCharacterProperties(textObject, "GUNNAR", NavalStorylineData.Gunnar.CharacterObject, false);
				return textObject;
			}
		}

		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x060004BF RID: 1215 RVA: 0x0001F82C File Offset: 0x0001DA2C
		public override TextObject Title
		{
			get
			{
				return new TextObject("{=9VzikXB0}Speak to Gunnar and Your Sister", null);
			}
		}

		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x060004C0 RID: 1216 RVA: 0x0001F839 File Offset: 0x0001DA39
		public override bool IsRemainingTimeHidden
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x060004C1 RID: 1217 RVA: 0x0001F83C File Offset: 0x0001DA3C
		public override string SpecialQuestType
		{
			get
			{
				return "NavalStoryline";
			}
		}

		// Token: 0x060004C2 RID: 1218 RVA: 0x0001F843 File Offset: 0x0001DA43
		public SpeakToGunnarAndSisterQuest(Quest5SetPieceBattleMissionController.BossFightOutComeEnum bossFightOutcome)
			: base("naval_storyline_act3_quest5_end", NavalStorylineData.Gunnar, CampaignTime.Never, 0)
		{
			this._bossFightOutcome = bossFightOutcome;
		}

		// Token: 0x060004C3 RID: 1219 RVA: 0x0001F862 File Offset: 0x0001DA62
		protected override void OnStartQuest()
		{
			this.InitializeDialogues();
			base.AddLog(this._startLog, false);
			StoryModeHeroes.LittleSister.HitPoints = StoryModeHeroes.LittleSister.MaxHitPoints;
		}

		// Token: 0x060004C4 RID: 1220 RVA: 0x0001F88C File Offset: 0x0001DA8C
		protected override void SetDialogs()
		{
		}

		// Token: 0x060004C5 RID: 1221 RVA: 0x0001F88E File Offset: 0x0001DA8E
		protected override void InitializeQuestOnGameLoad()
		{
			if (this._bossFightOutcome == Quest5SetPieceBattleMissionController.BossFightOutComeEnum.None || this._bossFightOutcome == Quest5SetPieceBattleMissionController.BossFightOutComeEnum.PlayerDefeatedWaitingForConversation)
			{
				this._bossFightOutcome = Quest5SetPieceBattleMissionController.BossFightOutComeEnum.PlayerRefusedTheDuel;
			}
			this.InitializeDialogues();
		}

		// Token: 0x060004C6 RID: 1222 RVA: 0x0001F8AE File Offset: 0x0001DAAE
		protected override void OnCompleteWithSuccess()
		{
			this.MakeGunnarNotable();
			NavalDLCHelpers.AddSisterToClan();
		}

		// Token: 0x060004C7 RID: 1223 RVA: 0x0001F8BC File Offset: 0x0001DABC
		private void InitializeDialogues()
		{
			StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, null, false);
			this.DecideGunnarDialogue();
			DialogFlow dialogFlow = DialogFlow.CreateDialogFlow("start", 1500).NpcLine("{=!}{GUNNAR_FINAL_DIALOG_LINE_1}", null, null, null, null).Condition(() => Campaign.Current.QuestManager.IsThereActiveQuestWithType(typeof(SpeakToGunnarAndSisterQuest)) && Hero.OneToOneConversationHero == NavalStorylineData.Gunnar && Settlement.CurrentSettlement == NavalStorylineData.HomeSettlement)
				.Consequence(delegate
				{
					MissionConversationLogic missionBehavior = Mission.Current.GetMissionBehavior<MissionConversationLogic>();
					if (missionBehavior == null)
					{
						return;
					}
					missionBehavior.DisableStartConversation(true);
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
				.CloseDialog();
			DialogFlow dialogFlow2 = DialogFlow.CreateDialogFlow("start", 1200).NpcLine("{=L3NhSRHr}{PLAYER.NAME}... It's good to be free, and back on land. Things have changed so much though. Men follow you, and jump to their feet to obey your orders, and speak of your deeds...", null, null, null, null).Condition(delegate
			{
				bool flag = Hero.OneToOneConversationHero == StoryModeHeroes.LittleSister && Campaign.Current.QuestManager.IsThereActiveQuestWithType(typeof(SpeakToGunnarAndSisterQuest));
				if (flag)
				{
					StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, null, false);
					StringHelpers.SetCharacterProperties("BROTHER", StoryModeHeroes.ElderBrother.CharacterObject, null, false);
					StringHelpers.SetCharacterProperties("SISTER", StoryModeHeroes.LittleSister.CharacterObject, null, false);
					MBTextManager.SetTextVariable("CLAN_NAME", Clan.PlayerClan.Name, false);
				}
				return flag;
			})
				.NpcLine("{=bqNHSlsb}One moment I am a slave and the next I seem to be some sort of noble lady... I need some time to rest. I will seek out our brother {BROTHER.NAME}.", null, null, null, null)
				.BeginPlayerOptions(null, false)
				.PlayerOption("{=VNEiqDzI}Of course, {SISTER.NAME}. Join {BROTHER.NAME}, and take all the time you need.", null, null, null)
				.GotoDialogState("sister_end_conversation_token")
				.PlayerOption("{=cESGiaPI}Things have indeed changed. Rest now, but remember that you are of the {CLAN_NAME}, and you must learn to command respect.", null, null, null)
				.GotoDialogState("sister_end_conversation_token")
				.EndPlayerOptions()
				.NpcLine("{=WFFv3fyb}Thank you again, {PLAYER.NAME}. I will pray nightly to Heaven for your safety.", null, null, "sister_end_conversation_token", null)
				.Consequence(new ConversationSentence.OnConsequenceDelegate(this.SisterFinalConversationConsequence))
				.CloseDialog();
			Campaign.Current.ConversationManager.AddDialogFlow(dialogFlow, null);
			Campaign.Current.ConversationManager.AddDialogFlow(dialogFlow2, null);
		}

		// Token: 0x060004C8 RID: 1224 RVA: 0x0001FAE8 File Offset: 0x0001DCE8
		private void DecideGunnarDialogue()
		{
			TextObject textObject;
			TextObject textObject2;
			if (this._bossFightOutcome == Quest5SetPieceBattleMissionController.BossFightOutComeEnum.PlayerRefusedTheDuel)
			{
				textObject = new TextObject("{=JoBwweim}Well, {PLAYER.NAME}... Your sister is free, thank the gods. You gave Purig the death he deserved. None will mourn him.", null);
				textObject2 = new TextObject("{=bTCuEZW9}As for the Sea Hounds, I hear, they've mostly scattered. It's time for me to return to my home in Beinland. I've settled what I wish to settle, and all this rowing and ramming and climbing and jostling and fighting is hard on my old bones.", null);
			}
			else if (this._bossFightOutcome == Quest5SetPieceBattleMissionController.BossFightOutComeEnum.PlayerAcceptedAndWonTheDuel)
			{
				textObject = new TextObject("{=AmwwLMvJ}Well, {PLAYER.NAME}... Your sister is free, thank the gods. You gave Purig a far more honorable death than he deserved. Men will speak well of you.", null);
				textObject2 = new TextObject("{=bTCuEZW9}As for the Sea Hounds, I hear, they've mostly scattered. It's time for me to return to my home in Beinland. I've settled what I wish to settle, and all this rowing and ramming and climbing and jostling and fighting is hard on my old bones.", null);
			}
			else if (this._bossFightOutcome == Quest5SetPieceBattleMissionController.BossFightOutComeEnum.PlayerAcceptedTheDuelLostItAndLetPurigGo)
			{
				textObject = new TextObject("{=4rXR7jR9}Well, {PLAYER.NAME}... Your sister is free, thank the gods. Purig may have gotten away, but I doubt the Sea Hounds will be troubling us much more.", null);
				textObject2 = new TextObject("{=GqHo4JE2}It was an honorable thing, to duel him, and I am glad you kept your word to him, though he did not deserve it. For my part, though, I owe him nothing. I will continue to hunt him, and as it is much easier for him to evade a large group than a single hunter, I will do so alone.", null);
			}
			else
			{
				textObject = new TextObject("{=qGZZRhKj}Well, {PLAYER.NAME}... Your sister is free, thank the gods.  Purig is dead, and none will mourn him. I might that wish his death could have come some other way, but I will not dwell on it.", null);
				textObject2 = new TextObject("{=aJ8bK4oo}The Sea Hounds, I hear, they've mostly scattered. It's time for me to return to my home in Beinland. I've settled what I wish to settle, and all this rowing and ramming and climbing and jostling and fighting is hard on my old bones.", null);
			}
			TextObject textObject3;
			if (this._bossFightOutcome == Quest5SetPieceBattleMissionController.BossFightOutComeEnum.PlayerAcceptedTheDuelLostItAndLetPurigGo)
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

		// Token: 0x060004C9 RID: 1225 RVA: 0x0001FBC0 File Offset: 0x0001DDC0
		private void MakeGunnarNotable()
		{
			Village village = Village.All.FirstOrDefault<Village>((Village x) => x.Settlement.StringId == "village_N1_2");
			if (village != null)
			{
				TeleportHeroAction.ApplyImmediateTeleportToSettlement(NavalStorylineData.Gunnar, village.Settlement);
			}
		}

		// Token: 0x060004CA RID: 1226 RVA: 0x0001FC0C File Offset: 0x0001DE0C
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

		// Token: 0x060004CB RID: 1227 RVA: 0x0001FD28 File Offset: 0x0001DF28
		private void SisterFinalConversationConsequence()
		{
			base.CompleteQuestWithSuccess();
			MissionConversationLogic missionBehavior = Mission.Current.GetMissionBehavior<MissionConversationLogic>();
			if (missionBehavior != null)
			{
				missionBehavior.DisableStartConversation(false);
			}
			Campaign.Current.ConversationManager.ConversationEndOneShot += delegate
			{
				CampaignMission.Current.EndMission();
			};
		}

		// Token: 0x0400027D RID: 637
		private const string GunnarsLongshipStringId = "northern_medium_ship";

		// Token: 0x0400027E RID: 638
		private const string Tier3NordInfantryStringId = "nord_spear_warrior";

		// Token: 0x0400027F RID: 639
		private const string Tier4NordInfantryStringId = "nord_vargr";

		// Token: 0x04000280 RID: 640
		private const int Tier3NordInfantryCount = 10;

		// Token: 0x04000281 RID: 641
		private const int Tier4NordInfantryCount = 10;

		// Token: 0x04000282 RID: 642
		[SaveableField(1)]
		private Quest5SetPieceBattleMissionController.BossFightOutComeEnum _bossFightOutcome;
	}
}
