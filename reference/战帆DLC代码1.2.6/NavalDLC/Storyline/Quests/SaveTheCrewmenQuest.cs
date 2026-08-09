using System;
using SandBox.Conversation;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace NavalDLC.Storyline.Quests
{
	// Token: 0x0200003D RID: 61
	public class SaveTheCrewmenQuest : NavalStorylineQuestBase
	{
		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x0600044A RID: 1098 RVA: 0x0001D80D File Offset: 0x0001BA0D
		public override string SpecialQuestType
		{
			get
			{
				return "NavalStoryline";
			}
		}

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x0600044B RID: 1099 RVA: 0x0001D814 File Offset: 0x0001BA14
		public override NavalStorylineData.NavalStorylineStage Stage
		{
			get
			{
				return NavalStorylineData.NavalStorylineStage.Act1;
			}
		}

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x0600044C RID: 1100 RVA: 0x0001D817 File Offset: 0x0001BA17
		public override bool WillProgressStoryline
		{
			get
			{
				return this._willProgressStoryline;
			}
		}

		// Token: 0x0600044D RID: 1101 RVA: 0x0001D81F File Offset: 0x0001BA1F
		public SaveTheCrewmenQuest(string questId, Hero questGiver)
			: base(questId, questGiver, CampaignTime.Never, 0)
		{
			base.AddLog(this.DescriptionLogText, false);
			this.SetDialogs();
		}

		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x0600044E RID: 1102 RVA: 0x0001D843 File Offset: 0x0001BA43
		protected override string MainPartyTemplateStringId
		{
			get
			{
				return string.Empty;
			}
		}

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x0600044F RID: 1103 RVA: 0x0001D84A File Offset: 0x0001BA4A
		public override TextObject Title
		{
			get
			{
				return new TextObject("{=tvGCC1BF}Save the Crewmen", null);
			}
		}

		// Token: 0x170000BB RID: 187
		// (get) Token: 0x06000450 RID: 1104 RVA: 0x0001D857 File Offset: 0x0001BA57
		private TextObject DescriptionLogText
		{
			get
			{
				return new TextObject("{=PSjYdlCe}Rescue the merchant sailors who jumped overboard to escape the pirates.", null);
			}
		}

		// Token: 0x06000451 RID: 1105 RVA: 0x0001D864 File Offset: 0x0001BA64
		protected override void SetDialogs()
		{
			this.AddPlayerSavedCrewDialog();
		}

		// Token: 0x06000452 RID: 1106 RVA: 0x0001D86C File Offset: 0x0001BA6C
		protected override void InitializeQuestOnGameLoadInternal()
		{
			this.AddGameMenus();
		}

		// Token: 0x06000453 RID: 1107 RVA: 0x0001D874 File Offset: 0x0001BA74
		protected override void OnStartQuestInternal()
		{
			this._willProgressStoryline = true;
			this.AddGameMenus();
		}

		// Token: 0x06000454 RID: 1108 RVA: 0x0001D883 File Offset: 0x0001BA83
		protected override void HourlyTick()
		{
		}

		// Token: 0x06000455 RID: 1109 RVA: 0x0001D885 File Offset: 0x0001BA85
		protected override void RegisterEventsInternal()
		{
		}

		// Token: 0x06000456 RID: 1110 RVA: 0x0001D888 File Offset: 0x0001BA88
		private void AddPlayerSavedCrewDialog()
		{
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1200).NpcLine("{=kPB4vvTD}Thank you. Heaven be praised. We thought we'd escaped the arrows only to be drowned by the waves. Heaven protect us all.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsSavedCrew), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainAgent), null, null).Condition(() => this.IsSavedCrew(ConversationMission.OneToOneConversationAgent))
				.NpcLine("{=GVBtIsvA}Think nothing of it, lads. You'd have done the same for any of us, one sailor for another.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsGunnar), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsSavedCrew), null, null)
				.NpcLine("{=zQRrXKQH}So look, lads… Purig is still around, but I suspect he's overladen and undermanned. I doubt he can find us before nightfall, which is good, because I don't think we can outfight him. By my reckoning, we're still not far from Ostican. So row, my boys, for Ostican and safety!", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsGunnar), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainAgent), null, null)
				.Consequence(delegate
				{
					Campaign.Current.ConversationManager.ConversationEndOneShot += this.OnCrewSaved;
				})
				.CloseDialog(), this);
		}

		// Token: 0x06000457 RID: 1111 RVA: 0x0001D947 File Offset: 0x0001BB47
		private void OnCrewSaved()
		{
			Mission mission = Mission.Current;
			((mission != null) ? mission.GetMissionBehavior<NavalStorylineCaptivityMissionController>() : null).FinalizeMission();
			Campaign.Current.GameMenuManager.SetNextMenu("save_the_crewmen_placeholder_menu");
		}

		// Token: 0x06000458 RID: 1112 RVA: 0x0001D974 File Offset: 0x0001BB74
		private void CompleteQuest()
		{
			PlayerEncounter.Finish(true);
			base.CompleteQuestWithSuccess();
			for (int i = MobileParty.MainParty.Ships.Count - 1; i >= 0; i--)
			{
				MobileParty.MainParty.Ships[i].Owner = null;
			}
			Ship ship = new Ship(MBObjectManager.Instance.GetObject<ShipHull>("northern_trade_ship"));
			ChangeShipOwnerAction.ApplyByTransferring(PartyBase.MainParty, ship);
			MapState mapState;
			if ((mapState = GameStateManager.Current.ActiveState as MapState) != null)
			{
				mapState.Handler.TeleportCameraToMainParty();
			}
			NavalStorylineData.OnCheckpointReached(NavalStorylineData.NavalStorylineCheckpoint.Act1CaptivitySucceeded);
		}

		// Token: 0x06000459 RID: 1113 RVA: 0x0001DA03 File Offset: 0x0001BC03
		private void AddGameMenus()
		{
			base.AddGameMenu("save_the_crewmen_placeholder_menu", new TextObject("{=!}TEMP", null), new OnInitDelegate(this.naval_storyline_act_3_quest_1_setpiece_menu_on_init), 4, 0);
		}

		// Token: 0x0600045A RID: 1114 RVA: 0x0001DA29 File Offset: 0x0001BC29
		private void naval_storyline_act_3_quest_1_setpiece_menu_on_init(MenuCallbackArgs args)
		{
			this.CompleteQuest();
		}

		// Token: 0x0600045B RID: 1115 RVA: 0x0001DA31 File Offset: 0x0001BC31
		private bool IsGunnar(IAgent agent)
		{
			return agent.Character == NavalStorylineData.Gunnar.CharacterObject;
		}

		// Token: 0x0600045C RID: 1116 RVA: 0x0001DA45 File Offset: 0x0001BC45
		private bool IsMainAgent(IAgent agent)
		{
			return agent == Agent.Main;
		}

		// Token: 0x0600045D RID: 1117 RVA: 0x0001DA50 File Offset: 0x0001BC50
		private bool IsSavedCrew(IAgent agent)
		{
			Mission mission = Mission.Current;
			NavalStorylineCaptivityMissionController navalStorylineCaptivityMissionController = ((mission != null) ? mission.GetMissionBehavior<NavalStorylineCaptivityMissionController>() : null);
			return navalStorylineCaptivityMissionController != null && navalStorylineCaptivityMissionController.IsSavedCrew(agent);
		}

		// Token: 0x0400025C RID: 604
		private const string QuestFinishMenuId = "save_the_crewmen_placeholder_menu";

		// Token: 0x0400025D RID: 605
		private bool _willProgressStoryline;
	}
}
