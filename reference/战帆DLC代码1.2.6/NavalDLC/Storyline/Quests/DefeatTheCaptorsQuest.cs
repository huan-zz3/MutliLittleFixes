using System;
using Helpers;
using NavalDLC.Missions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace NavalDLC.Storyline.Quests
{
	// Token: 0x02000034 RID: 52
	public class DefeatTheCaptorsQuest : NavalStorylineQuestBase
	{
		// Token: 0x0600032E RID: 814 RVA: 0x00017BA4 File Offset: 0x00015DA4
		public DefeatTheCaptorsQuest(string questId)
			: base(questId, Hero.MainHero, CampaignTime.Never, 0)
		{
			this.SetDialogs();
			base.AddLog(this._descriptionLogText, false);
		}

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x0600032F RID: 815 RVA: 0x00017BD3 File Offset: 0x00015DD3
		public override TextObject Title
		{
			get
			{
				return new TextObject("{=pyPqiRwR}Break Free of Captivity", null);
			}
		}

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x06000330 RID: 816 RVA: 0x00017BE0 File Offset: 0x00015DE0
		private TextObject _descriptionLogText
		{
			get
			{
				return new TextObject("{=l315rexF}Defeat your captors, then free Gunnar and the others.", null);
			}
		}

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x06000331 RID: 817 RVA: 0x00017BED File Offset: 0x00015DED
		public override NavalStorylineData.NavalStorylineStage Stage
		{
			get
			{
				return NavalStorylineData.NavalStorylineStage.Act1;
			}
		}

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x06000332 RID: 818 RVA: 0x00017BF0 File Offset: 0x00015DF0
		public override bool WillProgressStoryline
		{
			get
			{
				return this._willProgressStoryline;
			}
		}

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x06000333 RID: 819 RVA: 0x00017BF8 File Offset: 0x00015DF8
		protected override string MainPartyTemplateStringId
		{
			get
			{
				return "storyline_act1_captivity_template";
			}
		}

		// Token: 0x06000334 RID: 820 RVA: 0x00017BFF File Offset: 0x00015DFF
		protected override void SetDialogs()
		{
			this.AddAllyDialog();
			this.AddPlayerUnconsciousAllyDialog();
		}

		// Token: 0x06000335 RID: 821 RVA: 0x00017C0D File Offset: 0x00015E0D
		protected override void InitializeQuestOnGameLoadInternal()
		{
			base.InitializeQuestOnGameLoadInternal();
			this.SetDialogs();
		}

		// Token: 0x06000336 RID: 822 RVA: 0x00017C1C File Offset: 0x00015E1C
		protected override void OnStartQuestInternal()
		{
			base.OnStartQuestInternal();
			this._willProgressStoryline = false;
			TextObject textObject = new TextObject("{=ATA1PShK}Purig's Party", null);
			Clan randomElementInefficiently = Extensions.GetRandomElementInefficiently<Clan>(Clan.BanditFactions);
			MobileParty mobileParty = CustomPartyComponent.CreateCustomPartyWithTroopRoster(NavalStorylineData.HomeSettlement.GatePosition, 4f, NavalStorylineData.HomeSettlement, textObject, randomElementInefficiently, TroopRoster.CreateDummyTroopRoster(), TroopRoster.CreateDummyTroopRoster(), null, "", "", 0f, false);
			Ship ship = new Ship(MBObjectManager.Instance.GetObject<ShipHull>("nord_medium_ship"));
			ChangeShipOwnerAction.ApplyByMobilePartyCreation(mobileParty.Party, ship);
			CampaignVec2 campaignVec;
			campaignVec..ctor(new Vec2(188f, 600f), false);
			mobileParty.SetSailAtPosition(campaignVec);
			MobileParty.MainParty.SetSailAtPosition(campaignVec);
			PlayerEncounter.RestartPlayerEncounter(mobileParty.Party, PartyBase.MainParty, false, false);
			PlayerEncounter.StartBattle();
			GameMenu.ActivateGameMenu("defeat_the_captors_after_fight");
			this.StartMission();
		}

		// Token: 0x06000337 RID: 823 RVA: 0x00017CF4 File Offset: 0x00015EF4
		public void StartMission()
		{
			CharacterObject @object = MBObjectManager.Instance.GetObject<CharacterObject>("sea_hound_captivity");
			CharacterObject object2 = MBObjectManager.Instance.GetObject<CharacterObject>("captivity_troops");
			NavalMissions.OpenNavalStorylineCaptivityMission(NavalStorylineData.GetNavalMissionInitializerTemplate("naval_storyline_act_1_phase_03"), NavalStorylineData.Gunnar.CharacterObject, @object, object2);
		}

		// Token: 0x06000338 RID: 824 RVA: 0x00017D3D File Offset: 0x00015F3D
		protected override void HourlyTick()
		{
		}

		// Token: 0x06000339 RID: 825 RVA: 0x00017D3F File Offset: 0x00015F3F
		protected override void RegisterEventsInternal()
		{
		}

		// Token: 0x0600033A RID: 826 RVA: 0x00017D44 File Offset: 0x00015F44
		private void AddAllyDialog()
		{
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1200).NpcLine("{=qtQXIguv}Well done, {PLAYER.NAME}! That's twice now you've gotten me out of a bad spot.", null, null, null, null).Condition(delegate
			{
				StringHelpers.SetCharacterProperties("PLAYER", Hero.MainHero.CharacterObject, null, false);
				Mission mission = Mission.Current;
				NavalStorylineCaptivityMissionController navalStorylineCaptivityMissionController = ((mission != null) ? mission.GetMissionBehavior<NavalStorylineCaptivityMissionController>() : null);
				return Hero.OneToOneConversationHero == NavalStorylineData.Gunnar && NavalStorylineData.Gunnar.HasMet && navalStorylineCaptivityMissionController != null && !navalStorylineCaptivityMissionController.WasPlayerKnockedOut;
			})
				.NpcLine("{=utFgkzhx}Well… Normally I'd say we put as much distance between us and Purig as quickly as we can, but those merchants are still out there floundering in the waves. We can't leave them there. I can get the sail up. Take the steering oar. Let's see if we can  get them out of the water.", null, null, null, null)
				.Consequence(delegate
				{
					Campaign.Current.ConversationManager.ConversationEndOneShot += this.OnDialogueEnded;
				})
				.CloseDialog(), this);
		}

		// Token: 0x0600033B RID: 827 RVA: 0x00017DC8 File Offset: 0x00015FC8
		private void AddPlayerUnconsciousAllyDialog()
		{
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1200).NpcLine("{=nQJohWdO}Are you all right, {PLAYER.NAME}? Don't worry, the rest of us managed to break free and took care of those bastards.", null, null, null, null).Condition(delegate
			{
				StringHelpers.SetCharacterProperties("PLAYER", Hero.MainHero.CharacterObject, null, false);
				Mission mission = Mission.Current;
				NavalStorylineCaptivityMissionController navalStorylineCaptivityMissionController = ((mission != null) ? mission.GetMissionBehavior<NavalStorylineCaptivityMissionController>() : null);
				return Hero.OneToOneConversationHero == NavalStorylineData.Gunnar && NavalStorylineData.Gunnar.HasMet && navalStorylineCaptivityMissionController != null && navalStorylineCaptivityMissionController.WasPlayerKnockedOut;
			})
				.NpcLine("{=evfMsY6h}Well… Normally I'd say we put as much distance between us and Purig as quickly as we can, but those merchants are still out there floundering in the waves. We can't leave them there. I can get the sail up. Take the steering oar. Let's see if we can't get them out of the water.", null, null, null, null)
				.Consequence(delegate
				{
					Campaign.Current.ConversationManager.ConversationEndOneShot += this.OnDialogueEnded;
				})
				.CloseDialog(), this);
		}

		// Token: 0x0600033C RID: 828 RVA: 0x00017E4C File Offset: 0x0001604C
		private void OnDialogueEnded()
		{
			Mission.Current.GetMissionBehavior<NavalStorylineCaptivityMissionController>().OnShipCaptured();
			base.CompleteQuestWithSuccess();
			MobileParty.MainParty.MemberRoster.Clear();
			CharacterObject @object = MBObjectManager.Instance.GetObject<CharacterObject>("captivity_troops");
			MobileParty.MainParty.AddElementToMemberRoster(@object, 7, false);
			MobileParty.MainParty.AddElementToMemberRoster(Hero.MainHero.CharacterObject, 1, true);
			MobileParty.MainParty.AddElementToMemberRoster(NavalStorylineData.Gunnar.CharacterObject, 1, false);
			MobileParty.MainParty.PartyComponent.ChangePartyLeader(Hero.MainHero);
			MobileParty.MainParty.IgnoreForHours(16f);
			new SaveTheCrewmenQuest("naval_storyline_save_the_crewmen_quest", NavalStorylineData.Gunnar).StartQuest();
		}

		// Token: 0x04000206 RID: 518
		private const string EnemyCharacterStringId = "sea_hound_captivity";

		// Token: 0x04000207 RID: 519
		private const string CrewCharacterStringId = "captivity_troops";

		// Token: 0x04000208 RID: 520
		private const float EncounterPositionX = 188f;

		// Token: 0x04000209 RID: 521
		private const float EncounterPositionY = 600f;

		// Token: 0x0400020A RID: 522
		private bool _willProgressStoryline = true;
	}
}
