using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using NavalDLC.Missions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
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
	// Token: 0x0200003F RID: 63
	public class SetSailAndEscortTheFortuneSeekersQuest : NavalStorylineQuestBase
	{
		// Token: 0x170000BF RID: 191
		// (get) Token: 0x06000473 RID: 1139 RVA: 0x0001DE5D File Offset: 0x0001C05D
		public override bool WillProgressStoryline
		{
			get
			{
				return this._willProgressStoryline;
			}
		}

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x06000474 RID: 1140 RVA: 0x0001DE65 File Offset: 0x0001C065
		public override NavalStorylineData.NavalStorylineStage Stage
		{
			get
			{
				return NavalStorylineData.NavalStorylineStage.Act3Quest1;
			}
		}

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x06000475 RID: 1141 RVA: 0x0001DE68 File Offset: 0x0001C068
		public bool HasMetMerchants
		{
			get
			{
				return this._hasMetMerchantParty;
			}
		}

		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x06000476 RID: 1142 RVA: 0x0001DE70 File Offset: 0x0001C070
		public bool HasSavedMerchants
		{
			get
			{
				return this._isMerchantPartySaved;
			}
		}

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x06000477 RID: 1143 RVA: 0x0001DE78 File Offset: 0x0001C078
		public bool IsConversationHeroTheMerchant
		{
			get
			{
				return CharacterObject.OneToOneConversationCharacter == this._merchantCharacter;
			}
		}

		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x06000478 RID: 1144 RVA: 0x0001DE87 File Offset: 0x0001C087
		private TextObject QuestSecondPhaseStartLog
		{
			get
			{
				TextObject textObject = new TextObject("{=ycq46riU}Escort the Vlandian merchants the rest of the way to {SETTLEMENT_LINK}.", null);
				textObject.SetTextVariable("SETTLEMENT_LINK", NavalStorylineData.HomeSettlement.EncyclopediaLinkWithName);
				return textObject;
			}
		}

		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x06000479 RID: 1145 RVA: 0x0001DEAA File Offset: 0x0001C0AA
		protected override string MainPartyTemplateStringId
		{
			get
			{
				return "storyline_act3_quest_1_main_party_template";
			}
		}

		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x0600047A RID: 1146 RVA: 0x0001DEB1 File Offset: 0x0001C0B1
		private TextObject MerchantPartyArrivedToHomeSettlementNotification
		{
			get
			{
				TextObject textObject = new TextObject("{=7ZFbP4TO}You have successfully escorted the Vlandian merchants to {SETTLEMENT_LINK}.", null);
				textObject.SetTextVariable("SETTLEMENT_LINK", NavalStorylineData.HomeSettlement.EncyclopediaLinkWithName);
				return textObject;
			}
		}

		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x0600047B RID: 1147 RVA: 0x0001DED4 File Offset: 0x0001C0D4
		private TextObject FailLogText
		{
			get
			{
				return new TextObject("{=F0bGPXyz}You failed to defend the Vlandian merchants.", null);
			}
		}

		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x0600047C RID: 1148 RVA: 0x0001DEE1 File Offset: 0x0001C0E1
		public override TextObject Title
		{
			get
			{
				return new TextObject("{=ntIGLPdc}Escort the Vlandian Merchants", null);
			}
		}

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x0600047D RID: 1149 RVA: 0x0001DEEE File Offset: 0x0001C0EE
		private TextObject _descriptionLogText
		{
			get
			{
				return new TextObject("{=ik68yVRc}Guard a Vlandian merchant ship sailing home from Beinland.", null);
			}
		}

		// Token: 0x170000CA RID: 202
		// (get) Token: 0x0600047E RID: 1150 RVA: 0x0001DEFB File Offset: 0x0001C0FB
		private TextObject _allyDefeatedText
		{
			get
			{
				return new TextObject("{=9sfcVI0Q}Your allies were defeated. You will have to try again.", null);
			}
		}

		// Token: 0x0600047F RID: 1151 RVA: 0x0001DF08 File Offset: 0x0001C108
		public SetSailAndEscortTheFortuneSeekersQuest(string questId, Hero questGiver, Settlement targetSettlement)
			: base(questId, questGiver, CampaignTime.Never, 0)
		{
			this._willProgressStoryline = false;
			this._targetSettlement = targetSettlement;
			this.SetMerchantCharacterReference();
			base.AddLog(this._descriptionLogText, false);
		}

		// Token: 0x06000480 RID: 1152 RVA: 0x0001DF3A File Offset: 0x0001C13A
		protected override void SetDialogs()
		{
			this.AddMerchantDialogue();
		}

		// Token: 0x06000481 RID: 1153 RVA: 0x0001DF44 File Offset: 0x0001C144
		protected override void InitializeQuestOnGameLoadInternal()
		{
			this.SetMerchantCharacterReference();
			this.AddGameMenus();
			this.SetDialogs();
			this.SetBanditSpawnPositions();
			if (this._merchantParty != null && this._merchantParty.IsActive)
			{
				NavalDLCHelpers.SetCustomSailPatternOfPartyShips(this._merchantParty, "generated_square_l1_h4_04");
			}
			MobileParty activeBanditParty = this.GetActiveBanditParty();
			if (activeBanditParty != null && activeBanditParty.IsActive)
			{
				NavalDLCHelpers.SetCustomSailPatternOfPartyShips(activeBanditParty, "generated_square_l1_h4_10");
			}
			if (MobileParty.MainParty.IsActive)
			{
				NavalDLCHelpers.SetCustomSailPatternOfPartyShips(MobileParty.MainParty, "generated_square__h4_09");
			}
		}

		// Token: 0x06000482 RID: 1154 RVA: 0x0001DFC6 File Offset: 0x0001C1C6
		private void SetMerchantCharacterReference()
		{
			this._merchantCharacter = MBObjectManager.Instance.GetObject<CharacterObject>("vlandian_fortune_seekers");
		}

		// Token: 0x06000483 RID: 1155 RVA: 0x0001DFE0 File Offset: 0x0001C1E0
		protected override void OnStartQuestInternal()
		{
			this.AddGameMenus();
			this.SetDialogs();
			this.SpawnMerchantParty();
			this.SetBanditSpawnPositions();
			CampaignVec2 banditSpawnPosition = this.GetBanditSpawnPosition(0);
			this._initialBanditParty = this.SpawnBanditParty("set_sail_and_escort_generic_party_1", Campaign.Current.ObjectManager.GetObject<PartyTemplateObject>("storyline_act3_quest_1_generic_party_template"), false, banditSpawnPosition);
			this._willProgressStoryline = true;
		}

		// Token: 0x06000484 RID: 1156 RVA: 0x0001E03C File Offset: 0x0001C23C
		private void SetBanditSpawnPositions()
		{
			this._banditSpawnPositions = new List<Vec2>
			{
				new Vec2(200f, 655f),
				new Vec2(202f, 615f),
				new Vec2(210f, 595f)
			};
		}

		// Token: 0x06000485 RID: 1157 RVA: 0x0001E094 File Offset: 0x0001C294
		private CampaignVec2 GetBanditSpawnPosition(int index)
		{
			Vec2 vec = this._banditSpawnPositions[index];
			return NavigationHelper.FindReachablePointAroundPosition(new CampaignVec2(vec, false), 2, 5f, 0f, false);
		}

		// Token: 0x06000486 RID: 1158 RVA: 0x0001E0C8 File Offset: 0x0001C2C8
		protected override void IsNavalQuestPartyInternal(PartyBase party, NavalStorylinePartyData data)
		{
			MobileParty initialBanditParty = this._initialBanditParty;
			if (((initialBanditParty != null) ? initialBanditParty.Party : null) != party)
			{
				MobileParty secondBanditParty = this._secondBanditParty;
				if (((secondBanditParty != null) ? secondBanditParty.Party : null) != party)
				{
					MobileParty merchantParty = this._merchantParty;
					if (((merchantParty != null) ? merchantParty.Party : null) == party)
					{
						PartyTemplateObject @object = Campaign.Current.ObjectManager.GetObject<PartyTemplateObject>("storyline_act3_quest_1_caravan_party_template");
						data.PartySize = (int)NavalDLCHelpers.GetMaxPartySizeLimitFromTemplate(@object).ResultNumber;
						data.IsQuestParty = true;
						return;
					}
					MobileParty specialBanditParty = this._specialBanditParty;
					if (((specialBanditParty != null) ? specialBanditParty.Party : null) == party)
					{
						PartyTemplateObject object2 = Campaign.Current.ObjectManager.GetObject<PartyTemplateObject>("storyline_act3_quest_1_special_party_template");
						data.PartySize = (int)NavalDLCHelpers.GetMaxPartySizeLimitFromTemplate(object2).ResultNumber;
						data.IsQuestParty = true;
					}
					return;
				}
			}
			PartyTemplateObject object3 = Campaign.Current.ObjectManager.GetObject<PartyTemplateObject>("storyline_act3_quest_1_generic_party_template");
			data.PartySize = (int)NavalDLCHelpers.GetMaxPartySizeLimitFromTemplate(object3).ResultNumber;
			data.IsQuestParty = true;
		}

		// Token: 0x06000487 RID: 1159 RVA: 0x0001E1C0 File Offset: 0x0001C3C0
		private void AddMerchantDialogue()
		{
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 100).NpcLine("{=6QkMVCgz}Ahoy! It's good to have you with us. We've seen sails, and I reckon that there are still pirates about.", null, null, null, null).Condition(() => this._hasMetMerchantParty && !this._isMerchantPartySaved && CharacterObject.OneToOneConversationCharacter == this._merchantCharacter)
				.CloseDialog(), this);
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 100).NpcLine("{=acz9UxsD}Thank the Heavens. And thank you. Those Sea Hound vessels would have torn us to pieces. You came just in time.", null, null, null, null).Condition(() => this._isMerchantPartySaved && !this._isAfterFightDialogDone && CharacterObject.OneToOneConversationCharacter == this._merchantCharacter)
				.NpcLine("{=CowdyMzB}We would still wish to show you our gratitude. I took a collection among the men whose lives you saved today. We wish to offer you a barrel of oil and a bundle of ivory. These are the rewards of our labor over the past months, but they would mean nothing to us if our ship were seized by pirates.", null, null, null, null)
				.Consequence(delegate
				{
					base.AddLog(this.QuestSecondPhaseStartLog, false);
					this._isAfterFightDialogDone = true;
				})
				.BeginPlayerOptions(null, false)
				.PlayerOption("{=e69pk8m2}I accept your gift. Let us return to Ostican.", null, null, null)
				.Consequence(new ConversationSentence.OnConsequenceDelegate(this.AcceptGifts))
				.CloseDialog()
				.PlayerOption("{=sacjGtbK}You risked much for those goods. Keep them.", null, null, null)
				.Consequence(new ConversationSentence.OnConsequenceDelegate(this.RejectGifts))
				.CloseDialog()
				.EndPlayerOptions()
				.CloseDialog(), this);
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 100).NpcLine("{=acz9UxsD}Thank the Heavens. And thank you. Those Sea Hound vessels would have torn us to pieces. You came just in time.", null, null, null, null).Condition(() => this._isMerchantPartySaved && this._isAfterFightDialogDone && CharacterObject.OneToOneConversationCharacter == this._merchantCharacter)
				.CloseDialog(), this);
		}

		// Token: 0x06000488 RID: 1160 RVA: 0x0001E2FE File Offset: 0x0001C4FE
		public void OnMerchantsMet()
		{
			this._hasMetMerchantParty = true;
			this.DirectMerchantPartyToBase();
		}

		// Token: 0x06000489 RID: 1161 RVA: 0x0001E310 File Offset: 0x0001C510
		private void AcceptGifts()
		{
			ItemRosterElement itemRosterElement;
			itemRosterElement..ctor(Extensions.GetRandomElementWithPredicate<ItemObject>(Items.All, (ItemObject x) => x.IsTradeGood && x.ItemCategory == DefaultItemCategories.Oil), 1, null);
			PartyBase.MainParty.ItemRoster.AddToCounts(itemRosterElement.EquipmentElement, itemRosterElement.Amount);
			ItemRosterElement itemRosterElement2;
			itemRosterElement2..ctor(Extensions.GetRandomElementWithPredicate<ItemObject>(Items.All, (ItemObject x) => x.IsTradeGood && x.ItemCategory == NavalItemCategories.WalrusTusk), 1, null);
			PartyBase.MainParty.ItemRoster.AddToCounts(itemRosterElement2.EquipmentElement, itemRosterElement2.Amount);
		}

		// Token: 0x0600048A RID: 1162 RVA: 0x0001E3BD File Offset: 0x0001C5BD
		private void RejectGifts()
		{
			TraitLevelingHelper.OnIssueSolvedThroughQuest(Hero.MainHero, new Tuple<TraitObject, int>[]
			{
				new Tuple<TraitObject, int>(DefaultTraits.Generosity, 50)
			});
		}

		// Token: 0x0600048B RID: 1163 RVA: 0x0001E3E0 File Offset: 0x0001C5E0
		protected override void HourlyTick()
		{
			if (this._merchantParty != null && this._merchantParty.IsActive && base.IsOngoing)
			{
				if (this._merchantParty.MapEvent == null)
				{
					float getEncounterJoiningRadius = Campaign.Current.Models.EncounterModel.GetEncounterJoiningRadius;
					if (!this._hasMetMerchantParty && this._merchantParty.Position.DistanceSquared(MobileParty.MainParty.Position) <= getEncounterJoiningRadius * getEncounterJoiningRadius)
					{
						EncounterManager.StartPartyEncounter(MobileParty.MainParty.Party, this._merchantParty.Party);
					}
					if (!this._isMerchantPartySaved && this.GetActiveBanditParty() != null && this._merchantParty.Position.DistanceSquared(this.GetActiveBanditParty().Position) <= getEncounterJoiningRadius * getEncounterJoiningRadius)
					{
						MBInformationManager.AddQuickInformation(new TextObject("{=cjkHktxl}The merchant party is under attack.", null), 0, null, null, "event:/ui/notification/quest_update");
						EncounterManager.StartPartyEncounter(this.GetActiveBanditParty().Party, this._merchantParty.Party);
						return;
					}
					if (this._merchantParty.Position.DistanceSquared(NavalStorylineData.HomeSettlement.PortPosition) <= 100f)
					{
						MBInformationManager.AddQuickInformation(this.MerchantPartyArrivedToHomeSettlementNotification, 0, null, null, "");
						base.CompleteQuestWithSuccess();
						return;
					}
					SetSailAndEscortTheFortuneSeekersQuest.UtilizePartyEscortBehavior(this._merchantParty, MobileParty.MainParty, ref this._isMerchantPartyWaitingForEscort, 7f, 11f, new MobilePartyHelper.ResumePartyEscortBehaviorDelegate(this.DirectMerchantPartyToBase), false);
					MobileParty activeBanditParty = this.GetActiveBanditParty();
					if (activeBanditParty != null && PlayerCaptivity.CaptorParty != activeBanditParty.Party)
					{
						if (!base.IsTracked(activeBanditParty) && activeBanditParty.Position.Distance(MobileParty.MainParty.Position) < MobileParty.MainParty.SeeingRange)
						{
							base.AddTrackedObject(activeBanditParty);
						}
						SetPartyAiAction.GetActionForEngagingParty(activeBanditParty, this._merchantParty, 2, false);
						activeBanditParty.Ai.SetDoNotMakeNewDecisions(true);
					}
					this.AdjustMerchantPartySpeed();
					return;
				}
				else if (this._merchantParty.MapEvent.IsInvulnerable && this._merchantParty.MapEvent.BattleStartTime.ElapsedHoursUntilNow > 8f)
				{
					this._merchantParty.MapEvent.IsInvulnerable = false;
				}
			}
		}

		// Token: 0x0600048C RID: 1164 RVA: 0x0001E5FC File Offset: 0x0001C7FC
		private MobileParty GetActiveBanditParty()
		{
			MobileParty mobileParty;
			if ((mobileParty = this._initialBanditParty) == null)
			{
				mobileParty = this._secondBanditParty ?? this._specialBanditParty;
			}
			return mobileParty;
		}

		// Token: 0x0600048D RID: 1165 RVA: 0x0001E618 File Offset: 0x0001C818
		private void DirectMerchantPartyToBase()
		{
			SetPartyAiAction.GetActionForVisitingSettlement(this._merchantParty, NavalStorylineData.HomeSettlement, 2, false, true);
		}

		// Token: 0x0600048E RID: 1166 RVA: 0x0001E630 File Offset: 0x0001C830
		protected override void RegisterEventsInternal()
		{
			CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.MapEventEnded));
			CampaignEvents.MapEventStarted.AddNonSerializedListener(this, new Action<MapEvent, PartyBase, PartyBase>(this.MapEventStarted));
			CampaignEvents.GameMenuOpened.AddNonSerializedListener(this, new Action<MenuCallbackArgs>(this.OnGameMenuOpened));
			CampaignEvents.OnMissionEndedEvent.AddNonSerializedListener(this, new Action<IMission>(this.OnMissionEnded));
		}

		// Token: 0x0600048F RID: 1167 RVA: 0x0001E69C File Offset: 0x0001C89C
		private void OnGameMenuOpened(MenuCallbackArgs args)
		{
			if (NavalStorylineData.IsNavalStoryLineActive() && PlayerEncounter.Current != null && PlayerEncounter.EncounteredParty != null && PlayerEncounter.EncounteredParty.IsNavalStorylineQuestParty())
			{
				MenuContext menuContext = args.MenuContext;
				object obj;
				if (menuContext == null)
				{
					obj = null;
				}
				else
				{
					GameMenu gameMenu = menuContext.GameMenu;
					obj = ((gameMenu != null) ? gameMenu.StringId : null);
				}
				object obj2 = obj;
				if (obj2 == "naval_storyline_encounter_meeting")
				{
					if (PlayerEncounter.EncounteredParty == this._merchantParty.Party)
					{
						if (PlayerEncounter.MeetingDone)
						{
							PlayerEncounter.LeaveEncounter = true;
						}
					}
					else
					{
						PlayerEncounter.SetMeetingDone();
					}
				}
				if (obj2 == "naval_storyline_encounter" && this.GetActiveBanditParty() != null)
				{
					MobileParty initialBanditParty = this._initialBanditParty;
					if (((initialBanditParty != null) ? initialBanditParty.Party : null) != PlayerEncounter.EncounteredParty)
					{
						MobileParty secondBanditParty = this._secondBanditParty;
						if (((secondBanditParty != null) ? secondBanditParty.Party : null) != PlayerEncounter.EncounteredParty)
						{
							goto IL_0131;
						}
					}
					if (PlayerEncounter.EncounteredBattle == null || !PlayerEncounter.EncounteredBattle.HasWinner)
					{
						MapEvent encounteredBattle = PlayerEncounter.EncounteredBattle;
						if (encounteredBattle == null || !encounteredBattle.InvolvedParties.Contains(this._merchantParty.Party))
						{
							MBTextManager.SetTextVariable("ENCOUNTER_TEXT", new TextObject("{=Iu7TkxZo}“A ship! A ship!” calls out one of your lookouts. You can see it too - a square sail, outlined against the steel-gray northern sky. One the Sea Hounds has spotted you, and thinks to make you its prey.", null), false);
						}
						else
						{
							MBTextManager.SetTextVariable("ENCOUNTER_TEXT", new TextObject("{=XfqPvVDc}“A ship! A ship!” calls out one of your lookouts. You can see it too - a square sail, outlined against the steel-gray northern sky. One of the Sea Hounds stalking the merchant seems to be closing in on its prey.", null), false);
						}
					}
				}
			}
			IL_0131:
			MenuContext menuContext2 = args.MenuContext;
			string text;
			if (menuContext2 == null)
			{
				text = null;
			}
			else
			{
				GameMenu gameMenu2 = menuContext2.GameMenu;
				text = ((gameMenu2 != null) ? gameMenu2.StringId : null);
			}
			if (text == "naval_storyline_encounter" && this.GetActiveBanditParty() != null && PlayerEncounter.Current != null)
			{
				IEnumerable<PartyBase> involvedParties = PlayerEncounter.EncounteredBattle.InvolvedParties;
				MobileParty specialBanditParty = this._specialBanditParty;
				if (involvedParties.Contains((specialBanditParty != null) ? specialBanditParty.Party : null) || PlayerEncounter.EncounteredMobileParty == this._specialBanditParty)
				{
					GameMenu.SwitchToMenu("naval_storyline_act3_quest1_setpiece_menu");
				}
			}
		}

		// Token: 0x06000490 RID: 1168 RVA: 0x0001E850 File Offset: 0x0001CA50
		private void OnMissionEnded(IMission mission)
		{
			if (Mission.Current.IsNavalBattle && PlayerEncounter.Current != null && PlayerEncounter.EncounteredParty != null)
			{
				MobileParty specialBanditParty = this._specialBanditParty;
				if (((specialBanditParty != null) ? specialBanditParty.Party : null) == PlayerEncounter.EncounteredParty && PlayerEncounter.Battle != null && PlayerEncounter.Battle.BattleState == 1)
				{
					this._specialBattleWon = true;
					this._isMerchantPartySaved = true;
				}
			}
		}

		// Token: 0x06000491 RID: 1169 RVA: 0x0001E8B4 File Offset: 0x0001CAB4
		private void SpawnMerchantParty()
		{
			Clan clan = new Clan();
			clan.StringId = Campaign.Current.CampaignObjectManager.FindNextUniqueStringId<Clan>("naval_storyline_vlandian_merchant_clan");
			clan.ChangeClanName(new TextObject("{=FjwRsf1C}Vlandia", null), new TextObject("{=FjwRsf1C}Vlandia", null));
			clan.Culture = MBObjectManager.Instance.GetObject<CultureObject>("vlandia");
			clan.Banner = Banner.CreateRandomClanBanner(-1);
			clan.Color = 4287441178U;
			clan.Color2 = 4294426438U;
			clan.Banner.ChangePrimaryColor(4287441178U);
			clan.Banner.ChangeBackgroundColor(4287441178U, 4287441178U);
			clan.Banner.ChangeIconColors(4294426438U);
			TextObject textObject = new TextObject("{=FyfpoKvX}Vlandian Merchants", null);
			CampaignVec2 portPosition = this._targetSettlement.PortPosition;
			PartyTemplateObject @object = Campaign.Current.ObjectManager.GetObject<PartyTemplateObject>("storyline_act3_quest_1_caravan_party_template");
			this._merchantParty = CustomPartyComponent.CreateCustomPartyWithPartyTemplate(portPosition, 0.1f, NavalStorylineData.HomeSettlement, textObject, clan, @object, null, "camel", "camel_saddle_b", MobileParty.MainParty.Speed * 1.5f, false);
			NavalDLCHelpers.AddUpgradePiecesToPartyShips(this._merchantParty, SetSailAndEscortTheFortuneSeekersQuest.MerchantShipUpgradePieces, null);
			NavalDLCHelpers.SetCustomSailPatternOfPartyShips(this._merchantParty, "generated_square_l1_h4_04");
			foreach (Ship ship in this._merchantParty.Ships)
			{
				ship.IsInvulnerable = true;
			}
			this._merchantParty.MemberRoster.AddToCounts(this._merchantCharacter, 1, false, 0, 0, true, -1);
			this._merchantParty.ItemRoster.AddToCounts(DefaultItems.Grain, 40);
			this._merchantParty.IgnoreByOtherPartiesTill(base.QuestDueTime);
			SetPartyAiAction.GetActionForEngagingParty(this._merchantParty, MobileParty.MainParty, 2, false);
			this._merchantParty.Ai.SetDoNotMakeNewDecisions(true);
			this._merchantParty.SetPartyUsedByQuest(true);
			base.AddTrackedObject(this._merchantParty);
		}

		// Token: 0x06000492 RID: 1170 RVA: 0x0001EAB8 File Offset: 0x0001CCB8
		private void AdjustMerchantPartySpeed()
		{
			if (!this._hasMetMerchantParty)
			{
				return;
			}
			MobileParty activeBanditParty = this.GetActiveBanditParty();
			MobileParty mobileParty = MobileParty.MainParty;
			if (!mobileParty.IsActive || activeBanditParty == null || !activeBanditParty.IsActive)
			{
				return;
			}
			float num = Campaign.Current.Models.EncounterModel.GetEncounterJoiningRadius * 2.5f;
			if (activeBanditParty.Position.DistanceSquared(this._merchantParty.Position) <= num * num)
			{
				mobileParty = activeBanditParty;
			}
			float num2 = this.GetReferencePartySpeed(mobileParty);
			float num3 = this._merchantParty.Speed;
			CustomPartyComponent customPartyComponent = this._merchantParty.PartyComponent as CustomPartyComponent;
			while (num2 < num3 || this.ShouldMerchantPartyCatchUpWithParty(mobileParty, num2, num3))
			{
				num2 = this.GetReferencePartySpeed(mobileParty);
				if (num3 > num2 || MBMath.ApproximatelyEqualsTo(num2, num3, 1E-05f))
				{
					customPartyComponent.SetBaseSpeed(customPartyComponent.BaseSpeed - 0.05f);
				}
				else if (this.ShouldMerchantPartyCatchUpWithParty(mobileParty, num2, num3))
				{
					customPartyComponent.SetBaseSpeed(customPartyComponent.BaseSpeed + 0.05f);
				}
				num3 = this._merchantParty.Speed;
			}
		}

		// Token: 0x06000493 RID: 1171 RVA: 0x0001EBC4 File Offset: 0x0001CDC4
		private bool ShouldMerchantPartyCatchUpWithParty(MobileParty referenceParty, float cachedReferencePartySpeed, float cachedMerchantPartySpeed)
		{
			return referenceParty.IsMainParty && cachedMerchantPartySpeed <= 5.5f && MathF.Abs(cachedMerchantPartySpeed - cachedReferencePartySpeed) > 0.7f;
		}

		// Token: 0x06000494 RID: 1172 RVA: 0x0001EBE8 File Offset: 0x0001CDE8
		private float GetReferencePartySpeed(MobileParty referenceParty)
		{
			float num = 1f;
			if (referenceParty.IsActive)
			{
				num = referenceParty.Speed;
				if (referenceParty == this.GetActiveBanditParty())
				{
					num -= 0.5f;
				}
			}
			return num;
		}

		// Token: 0x06000495 RID: 1173 RVA: 0x0001EC1C File Offset: 0x0001CE1C
		private MobileParty SpawnBanditParty(string stringId, PartyTemplateObject partyTemplate, bool isSpecialParty, CampaignVec2 banditPartyPosition)
		{
			Hideout hideout = SettlementHelper.FindNearestHideoutToMobileParty(MobileParty.MainParty, 3, (Settlement x) => x.IsActive);
			Clan clan = Clan.All.FirstOrDefault<Clan>((Clan x) => x.StringId == "northern_pirates");
			MobileParty mobileParty = BanditPartyComponent.CreateBanditParty(stringId, clan, hideout.Settlement.Hideout, false, partyTemplate, banditPartyPosition);
			mobileParty.Party.SetCustomName(new TextObject("{=SKC3FeGR}Sea Hounds", null));
			mobileParty.SetPartyUsedByQuest(true);
			mobileParty.SetLandNavigationAccess(false);
			foreach (Ship ship in mobileParty.Ships)
			{
				ship.IsInvulnerable = true;
				if (isSpecialParty)
				{
					ship.IsTradeable = false;
					ship.IsUsedByQuest = true;
				}
			}
			NavalDLCHelpers.AddUpgradePiecesToPartyShips(mobileParty, isSpecialParty ? SetSailAndEscortTheFortuneSeekersQuest.SpecialBanditShipUpgradePieces : SetSailAndEscortTheFortuneSeekersQuest.RegularBanditShipUpgradePieces, null);
			NavalDLCHelpers.SetCustomSailPatternOfPartyShips(mobileParty, "generated_square_l1_h4_10");
			mobileParty.IgnoreByOtherPartiesTill(base.QuestDueTime);
			mobileParty.Ai.SetDoNotMakeNewDecisions(true);
			mobileParty.Party.SetCustomBanner(NavalStorylineData.CorsairBanner);
			return mobileParty;
		}

		// Token: 0x06000496 RID: 1174 RVA: 0x0001ED60 File Offset: 0x0001CF60
		private void MapEventStarted(MapEvent mapEvent, PartyBase attackerParty, PartyBase defenderParty)
		{
			if (attackerParty.IsNavalStorylineQuestParty())
			{
				foreach (Ship ship in attackerParty.Ships)
				{
					ship.IsInvulnerable = false;
				}
			}
			if (defenderParty.IsNavalStorylineQuestParty())
			{
				foreach (Ship ship2 in defenderParty.Ships)
				{
					ship2.IsInvulnerable = false;
				}
			}
			if (defenderParty.MobileParty == this._merchantParty && attackerParty.MobileParty == this.GetActiveBanditParty())
			{
				mapEvent.IsInvulnerable = true;
			}
		}

		// Token: 0x06000497 RID: 1175 RVA: 0x0001EE24 File Offset: 0x0001D024
		private void MapEventEnded(MapEvent mapEvent)
		{
			if (!this._isMerchantPartySaved && mapEvent.WinningSide != -1 && mapEvent.DefeatedSide != -1)
			{
				MapEventSide mapEventSide = mapEvent.GetMapEventSide(mapEvent.WinningSide);
				MapEventSide mapEventSide2 = mapEvent.GetMapEventSide(mapEvent.DefeatedSide);
				MobileParty banditParty = this.GetActiveBanditParty();
				if (mapEventSide2.Parties.Any<MapEventParty>((MapEventParty t) => t.Party == this._merchantParty.Party) && !mapEventSide2.IsMainPartyAmongParties())
				{
					this.OnMerchantPartyDestroyed();
				}
				else if (mapEventSide2.Parties.Any<MapEventParty>(delegate(MapEventParty t)
				{
					PartyBase party = t.Party;
					MobileParty banditParty2 = banditParty;
					return party == ((banditParty2 != null) ? banditParty2.Party : null);
				}))
				{
					if (mapEventSide.IsMainPartyAmongParties())
					{
						if (this._merchantParty.IsActive)
						{
							this.OnBanditPartyDestroyed();
							if (this._merchantParty.MemberRoster.TotalHealthyCount == 0 && mapEvent.InvolvedParties.Contains(this._merchantParty.Party))
							{
								this._merchantParty.MemberRoster.Clear();
								this._merchantParty.MemberRoster.AddToCounts(this._merchantCharacter, 11, false, 0, 0, true, -1);
							}
						}
						else
						{
							this.OnMerchantPartyDestroyed();
						}
					}
					else
					{
						this.OnMerchantSurvivedWithoutHelp();
					}
				}
				if (banditParty != null && banditParty.IsActive && mapEvent.InvolvedParties.Contains(banditParty.Party) && (banditParty.NavigationCapability & 2) == 2)
				{
					banditParty.SetMovePatrolAroundSettlement(NavalStorylineData.HomeSettlement, 2, true);
				}
			}
			if (this._merchantParty != null && this._merchantParty.IsActive && mapEvent.InvolvedParties.Contains(this._merchantParty.Party) && !this._isMerchantPartySaved && this._merchantParty.MemberRoster.TotalHealthyCount > 0)
			{
				this.DirectMerchantPartyToBase();
			}
		}

		// Token: 0x06000498 RID: 1176 RVA: 0x0001EFE8 File Offset: 0x0001D1E8
		private void OnBanditPartyDestroyed()
		{
			if (this.GetActiveBanditParty() == this._initialBanditParty || this.GetActiveBanditParty() == this._secondBanditParty)
			{
				CampaignVec2 banditSpawnPosition = this.GetBanditSpawnPosition(2);
				this._specialBanditParty = this.SpawnBanditParty("set_sail_and_escort_special_party", Campaign.Current.ObjectManager.GetObject<PartyTemplateObject>("storyline_act3_quest_1_special_party_template"), true, banditSpawnPosition);
				this._specialBanditParty.IsInfoHidden = true;
				this._initialBanditParty = null;
				this._secondBanditParty = null;
			}
		}

		// Token: 0x06000499 RID: 1177 RVA: 0x0001F05C File Offset: 0x0001D25C
		private void OpenConversationWithMerchants()
		{
			ConversationCharacterData conversationCharacterData = new ConversationCharacterData(CharacterObject.PlayerCharacter, PartyBase.MainParty, true, false, false, false, false, false);
			ConversationCharacterData conversationCharacterData2;
			conversationCharacterData2..ctor(this._merchantCharacter, this._merchantParty.Party, true, false, false, false, false, false);
			CampaignMission.OpenConversationMission(conversationCharacterData, conversationCharacterData2, "", "", false);
		}

		// Token: 0x0600049A RID: 1178 RVA: 0x0001F0AE File Offset: 0x0001D2AE
		private void OnMerchantPartyDestroyed()
		{
			this.ShowAllyDefeatedPopUp();
		}

		// Token: 0x0600049B RID: 1179 RVA: 0x0001F0B6 File Offset: 0x0001D2B6
		private void OnMerchantSurvivedWithoutHelp()
		{
			this.CancelQuest(null);
		}

		// Token: 0x0600049C RID: 1180 RVA: 0x0001F0BF File Offset: 0x0001D2BF
		private void CancelQuest(TextObject logText = null)
		{
			base.CompleteQuestWithCancel(logText);
			NavalStorylineData.DeactivateNavalStoryline();
		}

		// Token: 0x0600049D RID: 1181 RVA: 0x0001F0D0 File Offset: 0x0001D2D0
		protected override void OnFinalizeInternal()
		{
			MobileParty activeBanditParty = this.GetActiveBanditParty();
			if (activeBanditParty != null && activeBanditParty.IsActive)
			{
				DestroyPartyAction.Apply(null, activeBanditParty);
			}
			if (this._merchantParty.IsActive)
			{
				if (this._merchantParty.MapEventSide != null)
				{
					this._merchantParty.MapEventSide = null;
				}
				DestroyPartyAction.ApplyForDisbanding(this._merchantParty, NavalStorylineData.HomeSettlement);
			}
			MobileParty merchantParty = this._merchantParty;
			if (((merchantParty != null) ? merchantParty.ActualClan : null) != null)
			{
				DestroyClanAction.Apply(this._merchantParty.ActualClan);
			}
		}

		// Token: 0x0600049E RID: 1182 RVA: 0x0001F150 File Offset: 0x0001D350
		private void ShowAllyDefeatedPopUp()
		{
			object obj = new TextObject("{=cH3Kpkwg}Ally Defeated", null);
			TextObject textObject = new TextObject("{=DM6luo3c}Continue", null);
			InformationManager.ShowInquiry(new InquiryData(obj.ToString(), this._allyDefeatedText.ToString(), true, false, textObject.ToString(), null, new Action(this.OnAllyDefeatedPopUpClosed), null, "", 0f, null, null, null), true, false);
		}

		// Token: 0x0600049F RID: 1183 RVA: 0x0001F1B3 File Offset: 0x0001D3B3
		private void OnAllyDefeatedPopUpClosed()
		{
			this.CancelQuest(this._allyDefeatedText);
		}

		// Token: 0x060004A0 RID: 1184 RVA: 0x0001F1C4 File Offset: 0x0001D3C4
		public static void UtilizePartyEscortBehavior(MobileParty escortedParty, MobileParty escortParty, ref bool isWaitingForEscortParty, float innerRadius, float outerRadius, MobilePartyHelper.ResumePartyEscortBehaviorDelegate onPartyEscortBehaviorResumed, bool showDebugSpheres = false)
		{
			if (!isWaitingForEscortParty)
			{
				if (escortParty.Position.DistanceSquared(escortedParty.Position) >= outerRadius * outerRadius)
				{
					escortedParty.SetMoveGoToPoint(escortedParty.Position, 3);
					escortedParty.Ai.CheckPartyNeedsUpdate();
					isWaitingForEscortParty = true;
					return;
				}
			}
			else if (escortParty.Position.DistanceSquared(escortedParty.Position) <= innerRadius * innerRadius)
			{
				onPartyEscortBehaviorResumed.Invoke();
				escortedParty.Ai.CheckPartyNeedsUpdate();
				isWaitingForEscortParty = false;
			}
		}

		// Token: 0x060004A1 RID: 1185 RVA: 0x0001F23C File Offset: 0x0001D43C
		private void AddGameMenus()
		{
			base.AddGameMenu("naval_storyline_act3_quest1_setpiece_menu", new TextObject("{=tcfyZUb8}A brief squall cuts visibility to a few bowshots, and when it clears, you see that two Sea Hound vessels have snuck up upon the merchant’s ship and are in hot pursuit. They are much faster, so unless you can close and defeat them or draw them off, it is likely that your ally will be taken.", null), new OnInitDelegate(this.naval_storyline_act_3_quest_1_setpiece_menu_on_init), 4, 0);
			base.AddGameMenuOption("naval_storyline_act3_quest1_setpiece_menu", "naval_storyline_act3_quest1_setpiece_attack", new TextObject("{=DM6luo3c}Continue", null), new GameMenuOption.OnConditionDelegate(this.naval_storyline_act3_quest1_setpiece_attack_condition), new GameMenuOption.OnConsequenceDelegate(this.naval_storyline_act3_quest1_setpiece_attack_consequence), false, -1);
			base.AddGameMenu("set_piece_retry_menu", new TextObject("{=etH1IHNZ}You manage to put some distance between you and your enemies, and you have a moment to consider how to proceed.", null), new OnInitDelegate(this.set_piece_retry_menu_on_init), 0, 0);
			base.AddGameMenuOption("set_piece_retry_menu", "try_again_option", new TextObject("{=YHMDy3lQ}Try again", null), new GameMenuOption.OnConditionDelegate(this.set_piece_retry_menu_try_again_on_condition), new GameMenuOption.OnConsequenceDelegate(this.encounter_menu_try_again_on_consequence), false, -1);
			base.AddGameMenuOption("set_piece_retry_menu", "leave_option", new TextObject("{=3sRdGQou}Leave", null), new GameMenuOption.OnConditionDelegate(this.leave_on_condition), new GameMenuOption.OnConsequenceDelegate(this.leave_on_consequence), true, -1);
		}

		// Token: 0x060004A2 RID: 1186 RVA: 0x0001F330 File Offset: 0x0001D530
		private void naval_storyline_act_3_quest_1_setpiece_menu_on_init(MenuCallbackArgs args)
		{
			args.MenuContext.SetBackgroundMeshName("encounter_naval");
			NavalStorylineData.OnCheckpointReached(NavalStorylineData.NavalStorylineCheckpoint.Act3Quest1SetPieceEncounterMenu);
		}

		// Token: 0x060004A3 RID: 1187 RVA: 0x0001F348 File Offset: 0x0001D548
		private bool naval_storyline_act3_quest1_setpiece_attack_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 17;
			return true;
		}

		// Token: 0x060004A4 RID: 1188 RVA: 0x0001F353 File Offset: 0x0001D553
		private void naval_storyline_act3_quest1_setpiece_attack_consequence(MenuCallbackArgs args)
		{
			this.StartBattle();
		}

		// Token: 0x060004A5 RID: 1189 RVA: 0x0001F35C File Offset: 0x0001D55C
		private void set_piece_retry_menu_on_init(MenuCallbackArgs args)
		{
			args.MenuContext.SetBackgroundMeshName("encounter_naval");
			if (this._specialBattleWon)
			{
				DestroyPartyAction.Apply(null, this._specialBanditParty);
				this._merchantParty.Ai.SetDoNotMakeNewDecisions(true);
				this.DirectMerchantPartyToBase();
				PlayerEncounter.Finish(true);
				this.OpenConversationWithMerchants();
				this._specialBanditParty = null;
				NavalStorylineData.OnCheckpointReached(NavalStorylineData.NavalStorylineCheckpoint.Act3Quest1SetPieceSucceeded);
			}
		}

		// Token: 0x060004A6 RID: 1190 RVA: 0x0001F3BD File Offset: 0x0001D5BD
		private bool set_piece_retry_menu_try_again_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 1;
			return true;
		}

		// Token: 0x060004A7 RID: 1191 RVA: 0x0001F3C7 File Offset: 0x0001D5C7
		private void encounter_menu_try_again_on_consequence(MenuCallbackArgs args)
		{
			this.StartBattle();
		}

		// Token: 0x060004A8 RID: 1192 RVA: 0x0001F3CF File Offset: 0x0001D5CF
		private bool leave_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 16;
			return true;
		}

		// Token: 0x060004A9 RID: 1193 RVA: 0x0001F3DA File Offset: 0x0001D5DA
		private void leave_on_consequence(MenuCallbackArgs args)
		{
			this.CancelQuest(this.FailLogText);
		}

		// Token: 0x060004AA RID: 1194 RVA: 0x0001F3E8 File Offset: 0x0001D5E8
		private void StartBattle()
		{
			this._specialBattleWon = false;
			if (Hero.MainHero.IsWounded)
			{
				Hero.MainHero.Heal(Hero.MainHero.WoundedHealthLimit - Hero.MainHero.HitPoints + 1, false);
			}
			PlayerEncounter.Finish(true);
			PlayerEncounter.Start();
			PlayerEncounter.Current.SetupFields(this._specialBanditParty.Party, PartyBase.MainParty);
			PlayerEncounter.StartBattle();
			this._merchantParty.MapEventSide = PlayerEncounter.Battle.GetMapEventSide(PlayerEncounter.Battle.PlayerSide);
			MissionInitializerRecord navalMissionInitializerTemplate = NavalStorylineData.GetNavalMissionInitializerTemplate("naval_storyline_act_3_quest_1");
			TerrainType faceTerrainType = Campaign.Current.MapSceneWrapper.GetFaceTerrainType(MobileParty.MainParty.CurrentNavigationFace);
			navalMissionInitializerTemplate.TerrainType = faceTerrainType;
			navalMissionInitializerTemplate.NeedsRandomTerrain = false;
			navalMissionInitializerTemplate.PlayingInCampaignMode = true;
			navalMissionInitializerTemplate.RandomTerrainSeed = MBRandom.RandomInt(10000);
			navalMissionInitializerTemplate.AtmosphereOnCampaign = Campaign.Current.Models.MapWeatherModel.GetAtmosphereModel(MobileParty.MainParty.Position);
			navalMissionInitializerTemplate.SceneHasMapPatch = false;
			navalMissionInitializerTemplate.AtmosphereOnCampaign.NauticalInfo.UsesNavalSimulatedWater = 1;
			PartyTemplateObject @object = Campaign.Current.ObjectManager.GetObject<PartyTemplateObject>("storyline_act3_quest_1_caravan_party_template");
			new MBList<Ship>(NavalDLCHelpers.GetSetPieceBattleShips(base.Template, PartyBase.MainParty));
			new MBList<Ship>(NavalDLCHelpers.GetSetPieceBattleShips(@object, this._merchantParty.Party));
			new MBList<Ship>(this._specialBanditParty.Ships);
			NavalMissions.OpenHelpingAnAllySetPieceBattleMission(navalMissionInitializerTemplate, this._merchantParty, this._specialBanditParty);
			GameMenu.ActivateGameMenu("set_piece_retry_menu");
		}

		// Token: 0x060004AB RID: 1195 RVA: 0x0001F56F File Offset: 0x0001D76F
		public bool AreEnemiesNearby()
		{
			return this._specialBanditParty != null && this._specialBanditParty.IsActive && this._specialBanditParty.IsVisible;
		}

		// Token: 0x0400025F RID: 607
		private const string MerchantCharacterStringId = "vlandian_fortune_seekers";

		// Token: 0x04000260 RID: 608
		private const string Act3Quest1CaravanPartyTemplateStringId = "storyline_act3_quest_1_caravan_party_template";

		// Token: 0x04000261 RID: 609
		private const string Act3Quest1GenericPartyTemplateStringId = "storyline_act3_quest_1_generic_party_template";

		// Token: 0x04000262 RID: 610
		private const string Act3Quest1SpecialPartyTemplateStringId = "storyline_act3_quest_1_special_party_template";

		// Token: 0x04000263 RID: 611
		private const int TargetSettlementArrivalRadius = 10;

		// Token: 0x04000264 RID: 612
		private const float MapEventInvulnerabilityDurationInHours = 8f;

		// Token: 0x04000265 RID: 613
		public const string PlayerPartySailPatternId = "generated_square__h4_09";

		// Token: 0x04000266 RID: 614
		public const string MerchantPartySailPatternId = "generated_square_l1_h4_04";

		// Token: 0x04000267 RID: 615
		public const string SeaHoundsPartySailPatternId = "generated_square_l1_h4_10";

		// Token: 0x04000268 RID: 616
		private static readonly Dictionary<string, string> MerchantShipUpgradePieces = new Dictionary<string, string> { { "sail", "sails_lvl2" } };

		// Token: 0x04000269 RID: 617
		private static readonly Dictionary<string, string> RegularBanditShipUpgradePieces = new Dictionary<string, string>
		{
			{ "sail", "sails_lvl2" },
			{ "side", "side_northern_shields_lvl1" }
		};

		// Token: 0x0400026A RID: 618
		private static readonly Dictionary<string, string> SpecialBanditShipUpgradePieces = new Dictionary<string, string>
		{
			{ "sail", "sails_lvl2" },
			{ "side", "side_northern_shields_lvl1" }
		};

		// Token: 0x0400026B RID: 619
		private CharacterObject _merchantCharacter;

		// Token: 0x0400026C RID: 620
		[SaveableField(1)]
		private bool _isMerchantPartyWaitingForEscort;

		// Token: 0x0400026D RID: 621
		[SaveableField(2)]
		private bool _isMerchantPartySaved;

		// Token: 0x0400026E RID: 622
		[SaveableField(3)]
		private bool _isAfterFightDialogDone;

		// Token: 0x0400026F RID: 623
		[SaveableField(4)]
		private bool _specialBattleWon;

		// Token: 0x04000270 RID: 624
		[SaveableField(5)]
		private MobileParty _merchantParty;

		// Token: 0x04000271 RID: 625
		[SaveableField(6)]
		private MobileParty _initialBanditParty;

		// Token: 0x04000272 RID: 626
		[SaveableField(7)]
		private MobileParty _secondBanditParty;

		// Token: 0x04000273 RID: 627
		[SaveableField(8)]
		private MobileParty _specialBanditParty;

		// Token: 0x04000274 RID: 628
		[SaveableField(9)]
		private Settlement _targetSettlement;

		// Token: 0x04000275 RID: 629
		[SaveableField(10)]
		private bool _willProgressStoryline;

		// Token: 0x04000276 RID: 630
		[SaveableField(11)]
		private bool _hasMetMerchantParty;

		// Token: 0x04000277 RID: 631
		private List<Vec2> _banditSpawnPositions;
	}
}
