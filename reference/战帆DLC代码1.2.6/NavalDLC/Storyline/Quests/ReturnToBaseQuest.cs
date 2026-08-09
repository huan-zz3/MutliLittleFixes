using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace NavalDLC.Storyline.Quests
{
	// Token: 0x0200003B RID: 59
	public class ReturnToBaseQuest : QuestBase
	{
		// Token: 0x170000AB RID: 171
		// (get) Token: 0x06000426 RID: 1062 RVA: 0x0001D16D File Offset: 0x0001B36D
		public override string SpecialQuestType
		{
			get
			{
				return "NavalStoryline";
			}
		}

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x06000427 RID: 1063 RVA: 0x0001D174 File Offset: 0x0001B374
		public override TextObject Title
		{
			get
			{
				TextObject textObject = new TextObject("{=B9l3S9qh}Return to {SETTLEMENT_NAME}", null);
				textObject.SetTextVariable("SETTLEMENT_NAME", NavalStorylineData.HomeSettlement.Name);
				return textObject;
			}
		}

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x06000428 RID: 1064 RVA: 0x0001D197 File Offset: 0x0001B397
		public override bool IsRemainingTimeHidden
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170000AE RID: 174
		// (get) Token: 0x06000429 RID: 1065 RVA: 0x0001D19A File Offset: 0x0001B39A
		private TextObject _descriptionLogText
		{
			get
			{
				TextObject textObject = new TextObject("{=vmWnfbJb}Sail back to {SETTLEMENT_LINK} and prepare for your next move.", null);
				textObject.SetTextVariable("SETTLEMENT_LINK", NavalStorylineData.HomeSettlement.EncyclopediaLinkWithName);
				return textObject;
			}
		}

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x0600042A RID: 1066 RVA: 0x0001D1BD File Offset: 0x0001B3BD
		private TextObject _successLogText
		{
			get
			{
				TextObject textObject = new TextObject("{=NJcCXXu9}You have returned to {SETTLEMENT_LINK} and agreed to meet with Gunnar in the port after getting some much-needed rest.", null);
				textObject.SetTextVariable("SETTLEMENT_LINK", NavalStorylineData.HomeSettlement.EncyclopediaLinkWithName);
				return textObject;
			}
		}

		// Token: 0x0600042B RID: 1067 RVA: 0x0001D1E0 File Offset: 0x0001B3E0
		public ReturnToBaseQuest(string questId, Hero questGiver)
			: base(questId, questGiver, CampaignTime.Never, 0)
		{
			base.AddLog(this._descriptionLogText, false);
			base.AddTrackedObject(NavalStorylineData.HomeSettlement);
		}

		// Token: 0x0600042C RID: 1068 RVA: 0x0001D209 File Offset: 0x0001B409
		protected override void SetDialogs()
		{
		}

		// Token: 0x0600042D RID: 1069 RVA: 0x0001D20B File Offset: 0x0001B40B
		protected override void InitializeQuestOnGameLoad()
		{
			this.AddGameMenus();
		}

		// Token: 0x0600042E RID: 1070 RVA: 0x0001D214 File Offset: 0x0001B414
		protected override void OnStartQuest()
		{
			this.AddGameMenus();
			this._popupShown = NavalStorylineData.GetStorylineStage() < NavalStorylineData.NavalStorylineStage.Act2 || this.GetDistanceToOstican() < Campaign.Current.EstimatedAverageLordPartySpeed * 0.8f * (float)CampaignTime.HoursInDay;
			if (!this._popupShown)
			{
				Campaign.Current.GameMenuManager.SetNextMenu("return_to_base_placeholder");
			}
		}

		// Token: 0x0600042F RID: 1071 RVA: 0x0001D274 File Offset: 0x0001B474
		private void ShowReturnPopUp()
		{
			object obj = new TextObject("{=VxcduBO7}Return to Ostican", null);
			TextObject textObject = new TextObject("{=g1ZFrb3E}Do you want to go to Ostican right away?", null);
			TextObject textObject2 = new TextObject("{=7Hj13O18}Yes, take me to Ostican", null);
			TextObject textObject3 = new TextObject("{=l3eSbQJM}No, I will go there myself", null);
			InformationManager.ShowInquiry(new InquiryData(obj.ToString(), textObject.ToString(), true, true, textObject2.ToString(), textObject3.ToString(), new Action(this.FinishQuest), null, "", 0f, null, null, null), true, false);
			if (Campaign.Current.CurrentMenuContext != null)
			{
				GameMenu.ExitToLast();
			}
			this._popupShown = true;
		}

		// Token: 0x06000430 RID: 1072 RVA: 0x0001D307 File Offset: 0x0001B507
		private void AddGameMenus()
		{
			base.AddGameMenu("return_to_base_placeholder", new TextObject("{=!}TEMP", null), new OnInitDelegate(this.return_to_ostican_menu_on_init), 4, 0);
		}

		// Token: 0x06000431 RID: 1073 RVA: 0x0001D32D File Offset: 0x0001B52D
		private void return_to_ostican_menu_on_init(MenuCallbackArgs args)
		{
			this.ShowReturnPopUp();
		}

		// Token: 0x06000432 RID: 1074 RVA: 0x0001D335 File Offset: 0x0001B535
		protected override void HourlyTick()
		{
		}

		// Token: 0x06000433 RID: 1075 RVA: 0x0001D337 File Offset: 0x0001B537
		protected override void RegisterEvents()
		{
			CampaignEvents.GameMenuOpened.AddNonSerializedListener(this, new Action<MenuCallbackArgs>(this.OnGameMenuOpened));
			CampaignEvents.TickEvent.AddNonSerializedListener(this, new Action<float>(this.Tick));
		}

		// Token: 0x06000434 RID: 1076 RVA: 0x0001D367 File Offset: 0x0001B567
		private void Tick(float dt)
		{
			if (!this._popupShown)
			{
				this.ShowReturnPopUp();
			}
		}

		// Token: 0x06000435 RID: 1077 RVA: 0x0001D378 File Offset: 0x0001B578
		private void OnGameMenuOpened(MenuCallbackArgs args)
		{
			if (MobileParty.MainParty.CurrentSettlement == NavalStorylineData.HomeSettlement && base.IsOngoing)
			{
				this.FinishQuest();
				return;
			}
			PlayerEncounter playerEncounter = PlayerEncounter.Current;
			if (((playerEncounter != null) ? playerEncounter.EncounterSettlementAux : null) == NavalStorylineData.HomeSettlement && base.IsOngoing)
			{
				this.FinishQuest();
			}
		}

		// Token: 0x06000436 RID: 1078 RVA: 0x0001D3CB File Offset: 0x0001B5CB
		private void FinishQuest()
		{
			base.CompleteQuestWithSuccess();
			NavalStorylineData.DeactivateNavalStoryline();
		}

		// Token: 0x06000437 RID: 1079 RVA: 0x0001D3D8 File Offset: 0x0001B5D8
		protected override void OnCompleteWithSuccess()
		{
			base.AddLog(this._successLogText, false);
			if (!Campaign.Current.QuestManager.IsThereActiveQuestWithType(typeof(ScourgeoftheSeasQuest)))
			{
				new ScourgeoftheSeasQuest().StartQuest();
			}
		}

		// Token: 0x06000438 RID: 1080 RVA: 0x0001D410 File Offset: 0x0001B610
		private float GetDistanceToOstican()
		{
			return MobileParty.MainParty.Position.Distance(NavalStorylineData.HomeSettlement.PortPosition);
		}

		// Token: 0x04000253 RID: 595
		private const string QuestFinishInvisibleMenuId = "return_to_base_placeholder";

		// Token: 0x04000254 RID: 596
		[SaveableField(0)]
		private bool _popupShown;
	}
}
