using System;
using System.Linq;
using Helpers;
using NavalDLC.Missions;
using NavalDLC.Storyline.MissionControllers;
using SandBox;
using SandBox.Missions.AgentBehaviors;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace NavalDLC.Storyline.Quests
{
	// Token: 0x02000042 RID: 66
	public class SpeakToTheSailorsQuest : NavalStorylineQuestBase
	{
		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x060004CC RID: 1228 RVA: 0x0001FD7F File Offset: 0x0001DF7F
		public override TextObject Title
		{
			get
			{
				TextObject textObject = new TextObject("{=ebFg8V9z}Speak to the Sailors in {SETTLEMENT_NAME}", null);
				textObject.SetTextVariable("SETTLEMENT_NAME", this._settlement.Name);
				return textObject;
			}
		}

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x060004CD RID: 1229 RVA: 0x0001FDA3 File Offset: 0x0001DFA3
		protected override string MainPartyTemplateStringId
		{
			get
			{
				return "storyline_act3_quest_3_main_party_template";
			}
		}

		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x060004CE RID: 1230 RVA: 0x0001FDAA File Offset: 0x0001DFAA
		public override NavalStorylineData.NavalStorylineStage Stage
		{
			get
			{
				return NavalStorylineData.NavalStorylineStage.Act3SpeakToSailors;
			}
		}

		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x060004CF RID: 1231 RVA: 0x0001FDAD File Offset: 0x0001DFAD
		public override bool WillProgressStoryline
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060004D0 RID: 1232 RVA: 0x0001FDB0 File Offset: 0x0001DFB0
		public SpeakToTheSailorsQuest(string questId, Settlement targetSettlement)
			: base(questId, NavalStorylineData.Gunnar, CampaignTime.Never, 0)
		{
			this._settlement = targetSettlement;
		}

		// Token: 0x060004D1 RID: 1233 RVA: 0x0001FDCB File Offset: 0x0001DFCB
		protected override void InitializeQuestOnGameLoadInternal()
		{
			this.InitializeTemplates();
			this.SetDialogs();
			this.AddGameMenus();
		}

		// Token: 0x060004D2 RID: 1234 RVA: 0x0001FDDF File Offset: 0x0001DFDF
		protected override void SetDialogs()
		{
			this.AddTalkToGangradirDialogue();
			this.AddBjolgurDialogs();
			this.AddBjolgurSecondConversationDialogs();
			this.AddGunnarHorsebackDialogs();
			this.AddBjolgurDialogsEndBattle();
		}

		// Token: 0x060004D3 RID: 1235 RVA: 0x0001FE00 File Offset: 0x0001E000
		protected override void OnStartQuestInternal()
		{
			this.InitializeTemplates();
			this.SetDialogs();
			this.AddGameMenus();
			TextObject textObject = new TextObject("{=ZDDXZcMW}Gunnar has learned that the Sea Hounds will be targeting a ship that sails from the estuary near {SETTLEMENT_LINK}, bringing Sturgian silver to the Skolderbroda.", null);
			textObject.SetTextVariable("SETTLEMENT_LINK", this._settlement.EncyclopediaLinkWithName);
			NavalStorylineData.Bjolgur.ChangeState(1);
			TeleportHeroAction.ApplyImmediateTeleportToSettlement(NavalStorylineData.Bjolgur, this._settlement);
			base.AddLog(textObject, false);
			base.AddTrackedObject(this._settlement);
			base.AddTrackedObject(NavalStorylineData.Bjolgur);
		}

		// Token: 0x060004D4 RID: 1236 RVA: 0x0001FE7D File Offset: 0x0001E07D
		private void InitializeTemplates()
		{
			this._houndsTemplate = Campaign.Current.ObjectManager.GetObject<PartyTemplateObject>("storyline_act3_quest_3_sea_hounds_template");
			this._merchantsTemplate = Campaign.Current.ObjectManager.GetObject<PartyTemplateObject>("storyline_act3_quest_3_merchants_template");
		}

		// Token: 0x060004D5 RID: 1237 RVA: 0x0001FEB4 File Offset: 0x0001E0B4
		protected override void RegisterEventsInternal()
		{
			CampaignEvents.OnSettlementLeftEvent.AddNonSerializedListener(this, new Action<MobileParty, Settlement>(this.OnSettlementLeft));
			CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
			CampaignEvents.OnMissionEndedEvent.AddNonSerializedListener(this, new Action<IMission>(this.OnMissionEnded));
			CampaignEvents.GameMenuOpened.AddNonSerializedListener(this, new Action<MenuCallbackArgs>(this.OnGameMenuOpened));
		}

		// Token: 0x060004D6 RID: 1238 RVA: 0x0001FF1D File Offset: 0x0001E11D
		private void OnSettlementEntered(MobileParty party, Settlement settlement, Hero hero)
		{
			if (party == MobileParty.MainParty && settlement == NavalStorylineData.Act3Quest3TargetSettlement && !this.HadEncounterWithBjolgur())
			{
				this.StartConversationOnSettlementEntered();
			}
		}

		// Token: 0x060004D7 RID: 1239 RVA: 0x0001FF40 File Offset: 0x0001E140
		private void OnGameMenuOpened(MenuCallbackArgs args)
		{
			if (args.MenuContext.GameMenu.StringId == "naval_storyline_virtualport" && base.IsOngoing && Settlement.CurrentSettlement == this._settlement)
			{
				if (!this.HasTalkedToSailors())
				{
					TextObject textObject = new TextObject("{=4PUz4yQv}You have arrived in {SETTLEMENT_LINK}. As you sail up the estuary into the harbor, you spot several large ships at anchor in a cove. They look like Vlandian craft, probably the pirates that Fahda told you about. They do not try to give chase, however, possibly because they saw you too late to raise sail, or perhaps because they are lying in wait for more lucrative prey.", null);
					textObject.SetTextVariable("SETTLEMENT_LINK", this._settlement.EncyclopediaLinkWithName);
					MBTextManager.SetTextVariable("VIRTUAL_PORT_TEXT", textObject, false);
					return;
				}
				MobileParty.MainParty.SetSailAtPosition(Settlement.CurrentSettlement.PortPosition);
				PlayerEncounter.Finish(true);
			}
		}

		// Token: 0x060004D8 RID: 1240 RVA: 0x0001FFD0 File Offset: 0x0001E1D0
		private void OnMissionEnded(IMission mission)
		{
			if (PlayerEncounter.Current != null)
			{
				PartyBase encounteredParty = PlayerEncounter.EncounteredParty;
				MobileParty houndsParty = this._houndsParty;
				if (encounteredParty == ((houndsParty != null) ? houndsParty.Party : null))
				{
					if (PlayerEncounter.CampaignBattleResult != null && PlayerEncounter.CampaignBattleResult.BattleResolved)
					{
						if (!PlayerEncounter.CampaignBattleResult.PlayerDefeat && PlayerEncounter.CampaignBattleResult.PlayerVictory)
						{
							base.AddLog(new TextObject("{=bWqvK0iY}You were able to run the Sea Hound blockade.", null), false);
							this.AddState(SpeakToTheSailorsQuest.QuestState.BattleWon);
							return;
						}
					}
					else if (PlayerEncounter.WinningSide != -1)
					{
						Debug.FailedAssert("unhandled case", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\Storyline\\Quests\\SpeakToTheSailorsQuest.cs", "OnMissionEnded", 213);
					}
				}
			}
		}

		// Token: 0x060004D9 RID: 1241 RVA: 0x00020067 File Offset: 0x0001E267
		private void OnSettlementLeft(MobileParty party, Settlement settlement)
		{
			if (party.IsMainParty && this.HasTalkedToSailors() && NavalStorylineData.IsNavalStoryLineActive() && !this.HasBattleStarted() && MobileParty.MainParty.IsCurrentlyAtSea)
			{
				GameMenu.ActivateGameMenu("hounds_3_intercepted");
			}
		}

		// Token: 0x060004DA RID: 1242 RVA: 0x000200A0 File Offset: 0x0001E2A0
		private void AddGameMenus()
		{
			base.AddGameMenu("hounds_3_intercepted", new TextObject("{=lbLABNVY}You row out of {SETTLEMENT_LINK} harbor, with the Sturgian merchantmen following close behind you, and make your way toward the sea. But as you reach the estuary mouth, you see several ominous squat shapes blocking your passage to the open sea. Clearly it is the Sea Hounds, and you will either have to defeat them or hold them off long enough for your allies to make good their escape.", null), new OnInitDelegate(this.intercepted_menu_on_init), 0, 0);
			base.AddGameMenuOption("hounds_3_intercepted", "continue", new TextObject("{=1r0tDsrR}Attack!", null), new GameMenuOption.OnConditionDelegate(this.intercepted_menu_on_condition), new GameMenuOption.OnConsequenceDelegate(this.intercepted_menu_on_consequence), false, -1);
			base.AddGameMenu("quest3_encounter_invisible_menu", new TextObject("{=!}{RETRY_DESC}", null), new OnInitDelegate(this.quest3_encounter_invisible_menu_on_init), 0, 0);
			base.AddGameMenuOption("quest3_encounter_invisible_menu", "retry", new TextObject("{=YHMDy3lQ}Try again", null), new GameMenuOption.OnConditionDelegate(this.on_retry_condition), new GameMenuOption.OnConsequenceDelegate(this.on_retry_consequence), false, -1);
			base.AddGameMenuOption("quest3_encounter_invisible_menu", "retry_checkpoint", new TextObject("{=rHlzkNFL}Try again from checkpoint", null), new GameMenuOption.OnConditionDelegate(this.on_retry_from_checkpoint_condition), new GameMenuOption.OnConsequenceDelegate(this.on_retry_from_checkpoint_consequence), false, -1);
			base.AddGameMenuOption("quest3_encounter_invisible_menu", "leave", new TextObject("{=3sRdGQou}Leave", null), new GameMenuOption.OnConditionDelegate(this.on_leave_condition), new GameMenuOption.OnConsequenceDelegate(this.on_leave_consequence), true, -1);
		}

		// Token: 0x060004DB RID: 1243 RVA: 0x000201C9 File Offset: 0x0001E3C9
		private void StartConversationOnSettlementEntered()
		{
			PlayerEncounter.LocationEncounter.CreateAndOpenMissionController(LocationComplex.Current.GetLocationWithId("port"), null, NavalStorylineData.Bjolgur.CharacterObject, null);
		}

		// Token: 0x060004DC RID: 1244 RVA: 0x000201F1 File Offset: 0x0001E3F1
		private void on_leave_consequence(MenuCallbackArgs args)
		{
			base.CompleteQuestWithCancel(null);
		}

		// Token: 0x060004DD RID: 1245 RVA: 0x000201FA File Offset: 0x0001E3FA
		private bool on_leave_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 16;
			return this.HasBattleStarted();
		}

		// Token: 0x060004DE RID: 1246 RVA: 0x0002020A File Offset: 0x0001E40A
		private bool on_retry_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 1;
			return this.HasBattleStarted() && !this.CheckPointReached();
		}

		// Token: 0x060004DF RID: 1247 RVA: 0x00020226 File Offset: 0x0001E426
		private bool on_retry_from_checkpoint_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 1;
			return this.HasBattleStarted() && this.CheckPointReached();
		}

		// Token: 0x060004E0 RID: 1248 RVA: 0x0002023F File Offset: 0x0001E43F
		private void on_retry_consequence(MenuCallbackArgs args)
		{
			this.StartBattle(false);
		}

		// Token: 0x060004E1 RID: 1249 RVA: 0x00020248 File Offset: 0x0001E448
		private void on_retry_from_checkpoint_consequence(MenuCallbackArgs args)
		{
			this.StartBattle(true);
		}

		// Token: 0x060004E2 RID: 1250 RVA: 0x00020254 File Offset: 0x0001E454
		private void quest3_encounter_invisible_menu_on_init(MenuCallbackArgs args)
		{
			MBTextManager.SetTextVariable("RETRY_DESC", new TextObject("{=etH1IHNZ}You manage to put some distance between you and your enemies, and you have a moment to consider how to proceed.", null), false);
			this.DestroyParty(ref this._merchantParty);
			if (!this.HasBattleWon())
			{
				this.RefreshParty(this._houndsParty, this._houndsTemplate);
				this.RefreshParty(MobileParty.MainParty, base.Template);
				this.AddBurningTradeShipsToParties();
			}
			if (base.IsOngoing)
			{
				if (NavalStorylineData.IsNavalStoryLineActive() && this.HasBattleWon())
				{
					this.TalkToBjolgur();
				}
				else if (!this.HasBattleStarted())
				{
					this.StartBattle(false);
				}
			}
			else
			{
				GameMenu.ExitToLast();
			}
			NavalStorylineData.OnCheckpointReached(NavalStorylineData.NavalStorylineCheckpoint.Act3Quest3EncounterMenu);
		}

		// Token: 0x060004E3 RID: 1251 RVA: 0x000202F0 File Offset: 0x0001E4F0
		private void RefreshParty(MobileParty mobileParty, PartyTemplateObject pt)
		{
			MBList<TroopRosterElement> troopRoster = mobileParty.MemberRoster.GetTroopRoster();
			for (int i = 0; i < troopRoster.Count; i++)
			{
				if (troopRoster[i].Character.IsHero)
				{
					troopRoster[i].Character.HeroObject.Heal(troopRoster[i].Character.HeroObject.MaxHitPoints, false);
				}
				else
				{
					mobileParty.MemberRoster.RemoveTroop(troopRoster[i].Character, troopRoster[i].Number, default(UniqueTroopDescriptor), 0);
				}
			}
			TroopRoster troopRoster2 = Campaign.Current.Models.PartySizeLimitModel.FindAppropriateInitialRosterForMobileParty(mobileParty, pt);
			mobileParty.MemberRoster.Add(troopRoster2);
			this.HealShips(mobileParty);
		}

		// Token: 0x060004E4 RID: 1252 RVA: 0x000203B8 File Offset: 0x0001E5B8
		private void HealShips(MobileParty mobileParty)
		{
			foreach (Ship ship in mobileParty.Ships)
			{
				ship.HitPoints = ship.MaxHitPoints;
			}
		}

		// Token: 0x060004E5 RID: 1253 RVA: 0x00020410 File Offset: 0x0001E610
		private void intercepted_menu_on_init(MenuCallbackArgs args)
		{
			MBTextManager.SetTextVariable("SETTLEMENT_LINK", this._settlement.EncyclopediaLinkWithName, false);
			if (this._houndsParty == null)
			{
				this.CreateHoundsParty();
			}
		}

		// Token: 0x060004E6 RID: 1254 RVA: 0x00020438 File Offset: 0x0001E638
		[GameMenuInitializationHandler("hounds_3_intercepted")]
		private static void intercepted_menu_background_on_init(MenuCallbackArgs args)
		{
			Settlement settlement = Settlement.CurrentSettlement ?? MobileParty.MainParty.LastVisitedSettlement;
			args.MenuContext.SetBackgroundMeshName(settlement.Culture.StringId + "_port");
		}

		// Token: 0x060004E7 RID: 1255 RVA: 0x00020479 File Offset: 0x0001E679
		[GameMenuInitializationHandler("quest3_encounter_invisible_menu")]
		private static void encounter_menu_background_on_init(MenuCallbackArgs args)
		{
			args.MenuContext.SetBackgroundMeshName("encounter_naval");
		}

		// Token: 0x060004E8 RID: 1256 RVA: 0x0002048B File Offset: 0x0001E68B
		private void intercepted_menu_on_consequence(MenuCallbackArgs args)
		{
			GameMenu.ActivateGameMenu("quest3_encounter_invisible_menu");
		}

		// Token: 0x060004E9 RID: 1257 RVA: 0x00020498 File Offset: 0x0001E698
		private void AddBurningTradeShipsToParties()
		{
			ShipHull tradeCogHull = MBObjectManager.Instance.GetObject<ShipHull>("burning_cog_ship");
			ShipHull normalCogHull = MBObjectManager.Instance.GetObject<ShipHull>("ship_trade_cog_q3");
			ShipHull fishingShipHull = MBObjectManager.Instance.GetObject<ShipHull>("burning_fishing_ship");
			if (!MobileParty.MainParty.Ships.Any<Ship>((Ship x) => x.ShipHull == normalCogHull))
			{
				Ship ship = new Ship(normalCogHull);
				ChangeShipOwnerAction.ApplyByLooting(PartyBase.MainParty, ship);
			}
			if (!MobileParty.MainParty.Ships.Any<Ship>((Ship x) => x.ShipHull == fishingShipHull))
			{
				Ship ship2 = new Ship(fishingShipHull);
				ChangeShipOwnerAction.ApplyByLooting(PartyBase.MainParty, ship2);
			}
			if (!this._houndsParty.Ships.Any<Ship>((Ship x) => x.ShipHull == tradeCogHull))
			{
				Ship ship3 = new Ship(tradeCogHull);
				ship3.EquipUpgradePiece("fore", MBObjectManager.Instance.GetObject<ShipUpgradePiece>("fore_heavy_ballista_pot"));
				ChangeShipOwnerAction.ApplyByLooting(this._houndsParty.Party, ship3);
			}
		}

		// Token: 0x060004EA RID: 1258 RVA: 0x000205A7 File Offset: 0x0001E7A7
		private bool intercepted_menu_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 17;
			return true;
		}

		// Token: 0x060004EB RID: 1259 RVA: 0x000205B2 File Offset: 0x0001E7B2
		protected override void HourlyTick()
		{
		}

		// Token: 0x060004EC RID: 1260 RVA: 0x000205B4 File Offset: 0x0001E7B4
		protected override void OnFinalizeInternal()
		{
			if (base.IsTracked(this._settlement))
			{
				base.RemoveTrackedObject(this._settlement);
			}
			bool flag;
			if (PlayerEncounter.EncounteredParty != null)
			{
				PartyBase encounteredParty = PlayerEncounter.EncounteredParty;
				MobileParty houndsParty = this._houndsParty;
				flag = encounteredParty == ((houndsParty != null) ? houndsParty.Party : null);
			}
			else
			{
				flag = false;
			}
			this.DestroyParty(ref this._houndsParty);
			this.DestroyParty(ref this._merchantParty);
			if (NavalStorylineData.Bjolgur.IsActive)
			{
				this.RemoveHero(NavalStorylineData.Bjolgur);
			}
			if (flag)
			{
				PlayerEncounter.Finish(true);
			}
			for (int i = MobileParty.MainParty.Ships.Count - 1; i >= 0; i--)
			{
				if (MobileParty.MainParty.Ships[i].ShipHull.StringId == "burning_fishing_ship")
				{
					DestroyShipAction.Apply(MobileParty.MainParty.Ships[i]);
				}
			}
		}

		// Token: 0x060004ED RID: 1261 RVA: 0x0002068C File Offset: 0x0001E88C
		protected override void OnCompleteWithSuccessInternal()
		{
			NavalStorylineData.OnCheckpointReached(NavalStorylineData.NavalStorylineCheckpoint.Act3Quest3Succeeded);
		}

		// Token: 0x060004EE RID: 1262 RVA: 0x00020698 File Offset: 0x0001E898
		protected override void IsNavalQuestPartyInternal(PartyBase party, NavalStorylinePartyData data)
		{
			MobileParty houndsParty = this._houndsParty;
			if (party == ((houndsParty != null) ? houndsParty.Party : null))
			{
				data.PartySize = (int)NavalDLCHelpers.GetMaxPartySizeLimitFromTemplate(this._houndsTemplate).ResultNumber;
				data.Template = this._houndsTemplate;
				data.IsQuestParty = true;
				return;
			}
			MobileParty merchantParty = this._merchantParty;
			if (party == ((merchantParty != null) ? merchantParty.Party : null))
			{
				data.PartySize = (int)NavalDLCHelpers.GetMaxPartySizeLimitFromTemplate(this._merchantsTemplate).ResultNumber;
				data.Template = this._merchantsTemplate;
				data.IsQuestParty = true;
			}
		}

		// Token: 0x060004EF RID: 1263 RVA: 0x0002072C File Offset: 0x0001E92C
		private void AddTalkToGangradirDialogue()
		{
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 250).NpcLine("{=O0qBJmSS}Talk with Bjolgur when you're ready to depart.", null, null, null, null).Condition(() => base.IsOngoing && this.HadEncounterWithBjolgur() && Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero == NavalStorylineData.Gunnar && !this.HasTalkedToSailors())
				.CloseDialog(), this);
		}

		// Token: 0x060004F0 RID: 1264 RVA: 0x0002077C File Offset: 0x0001E97C
		private void AddBjolgurSecondConversationDialogs()
		{
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1300).NpcLine("{=GkaEhSwJ}{PLAYER.NAME}...", null, null, null, null).Condition(() => base.IsOngoing && this.HadEncounterWithBjolgur() && Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero == NavalStorylineData.Bjolgur && !this.HasTalkedToSailors())
				.NpcLine("{=zNaWTBin}Are you ready to take command of the fireship and break the blockade?", null, null, null, null)
				.BeginPlayerOptions(null, false)
				.PlayerOption("{=anANUCFV}I am as ready as I will ever be, I suppose.", null, null, null)
				.Consequence(new ConversationSentence.OnConsequenceDelegate(this.OnTalkedToSailors))
				.CloseDialog()
				.PlayerOption("{=6c2bHHHj}No, not yet.", null, null, null)
				.CloseDialog(), this);
		}

		// Token: 0x060004F1 RID: 1265 RVA: 0x00020814 File Offset: 0x0001EA14
		private void AddBjolgurDialogs()
		{
			string text;
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1300).NpcLine("{=J6QLFwbb}Welcome to {SETTLEMENT_LINK}, friend. Is that grizzled fellow with you, coming up now, is that my old comrade Gunnar of Lagshofn? A bit greyer than I remember from the days when we stood together in the shield wall facing Volbjorn's host, but, well, aren't we all…", null, null, null, null).Condition(delegate
			{
				MBTextManager.SetTextVariable("SETTLEMENT_LINK", NavalStorylineData.Act3Quest3TargetSettlement.EncyclopediaLinkWithName, false);
				bool flag = base.IsOngoing && !this.HadEncounterWithBjolgur() && Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero == NavalStorylineData.Bjolgur && !this.HasTalkedToSailors();
				if (flag)
				{
					Agent agent = Mission.Current.Agents.FirstOrDefault<Agent>((Agent x) => x.Character == NavalStorylineData.Gunnar.CharacterObject);
					if (!Campaign.Current.ConversationManager.ConversationAgents.Contains(agent))
					{
						this.AddGunnarToConversation(true);
					}
					agent.TeleportToPosition(this.GetGunnarTeleportPosition());
				}
				return flag;
			})
				.Consequence(delegate
				{
					this.AddState(SpeakToTheSailorsQuest.QuestState.HadEncounterWithBjolgor);
				})
				.NpcLine("{=KYqqVZh1}We received his letter a while back, about your run-in with Purig. Hah! That worm must have cursed like an old woman when he learned that his captives stole his ship. You two are making quite a name for yourselves.", null, null, null, null)
				.NpcLine("{=4bsY9noo}Bjolgur of Gauksdal! Well met! Are the Skolderbroda working for the merchants of {SETTLEMENT_LINK} now?", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsGunnar), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsBjolgur), null, null)
				.Condition(delegate
				{
					MBTextManager.SetTextVariable("SETTLEMENT_LINK", NavalStorylineData.Act3Quest3TargetSettlement.EncyclopediaLinkWithName, false);
					return true;
				})
				.NpcLine("{=lTjvOdoX}Not yet. As you know, our brotherhood does not fight before it's paid.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsBjolgur), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsGunnar), null, null)
				.NpcLine("{=iSKIBXnj}See, the {SETTLEMENT_LINK} merchants promised us a hoard of silver to protect their ships from the Sea Hounds, but it never arrived. I was sent down to learn what was going on, and I find the silver just sitting here, loaded onto a ship in the harbor, and the Sturgians are burning through it paying their men double wages not to run off. Some Vlandian pirates were sighted in the estuary, and the Sturgians refuse to venture out.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsBjolgur), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainAgent), null, null)
				.Condition(delegate
				{
					MBTextManager.SetTextVariable("SETTLEMENT_LINK", NavalStorylineData.Act3Quest3TargetSettlement.EncyclopediaLinkWithName, false);
					return true;
				})
				.GenerateToken(ref text)
				.BeginPlayerOptions(null, false)
				.PlayerOption("{=325GxBag}With so much wealth at stake, the Sturgians are right to be cautious.", null, null, null)
				.GotoDialogState(text)
				.PlayerOption("{=2YEmSZq1}Pirates are scum. Let's just sail out and crush them.", null, null, null)
				.GotoDialogState(text)
				.EndPlayerOptions()
				.NpcLine("{=kbug6MQB}Much as I would like to simply sail forth and bathe my sword in Sea Hound blood, my brotherhood has commanded me to do my best to ensure that the silver gets through safely.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsBjolgur), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainAgent), text, null)
				.NpcLine("{=rlpVWadN}Listen. I've been watching these Vlandian blockaders, and mulling over a plan. Their flagship has a lofty deck and it would be hard to board, but it doesn't seem very maneuverable. I think we can hit them with a trick that can be deadly in estuaries.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsBjolgur), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainAgent), null, null)
				.NpcLine("{=K3B52zD6}We will be upstream of them. I'll have the merchants here donate some leaky old vessel that they are about to scrap. We load it up with oil and pitch. Then we steer it towards the pirates, throw a torch in the hull, and jump.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsBjolgur), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainAgent), null, null)
				.NpcLine("{=8PmocyQy}Good, very good. With luck, the current shall carry it right into them, and they shall all merrily blaze up like a bonfire at a midwinter feast. The silver ship will make for the open sea, while the rest of us can have it out with any surviving Sea Hounds.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsGunnar), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsBjolgur), null, null)
				.NpcLine("{=867iaibq}Listen, though… We need someone to steer the fireship. I'd do it myself, but my order wants me to stay close to the silver. I'd found a few volunteers who've offered to do it, but they keep sobering up.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsBjolgur), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainAgent), null, null)
				.BeginPlayerOptions(null, false)
				.PlayerOption("{=ybDSa8Xr}I'll steer the fireship. Let us sail forth.", null, null, null)
				.Consequence(new ConversationSentence.OnConsequenceDelegate(this.OnTalkedToSailors))
				.CloseDialog()
				.PlayerOption("{=brMsnacx}I need a little while here in port first.", null, null, null)
				.CloseDialog()
				.EndPlayerOptions()
				.CloseDialog(), this);
		}

		// Token: 0x060004F2 RID: 1266 RVA: 0x00020A68 File Offset: 0x0001EC68
		private void AddGunnarHorsebackDialogs()
		{
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1300).NpcLine("{=GkaEhSwJ}{PLAYER.NAME}...", null, null, null, null).Condition(new ConversationSentence.OnConditionDelegate(this.gunnar_horseback_dialog_on_condition))
				.NpcLine("{=ypTUg9xC}There may be some Hound patrols about. Keep a wary eye.", null, null, null, null)
				.Consequence(delegate
				{
					Campaign.Current.ConversationManager.ConversationEndOneShot += this.gunnar_horseback_dialog_on_consequence;
				})
				.CloseDialog(), this);
		}

		// Token: 0x060004F3 RID: 1267 RVA: 0x00020AD8 File Offset: 0x0001ECD8
		private bool gunnar_horseback_dialog_on_condition()
		{
			StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, null, false);
			if (base.IsOngoing && Mission.Current != null && Hero.OneToOneConversationHero == NavalStorylineData.Gunnar)
			{
				BlockedEstuaryMissionController missionBehavior = Mission.Current.GetMissionBehavior<BlockedEstuaryMissionController>();
				if (missionBehavior != null && missionBehavior.CurrentPhase == BlockedEstuaryMissionController.BattlePhase.Phase2)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060004F4 RID: 1268 RVA: 0x00020B2C File Offset: 0x0001ED2C
		private void gunnar_horseback_dialog_on_consequence()
		{
			Mission.Current.GetMissionBehavior<BlockedEstuaryMissionController>().OnTalkedToGunnarPhase2();
		}

		// Token: 0x060004F5 RID: 1269 RVA: 0x00020B40 File Offset: 0x0001ED40
		private void AddBjolgurDialogsEndBattle()
		{
			string text;
			string text2;
			string text3;
			string text4;
			string text5;
			string text6;
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1300).NpcLine("{=8OtmPWCK}So! {PLAYER.NAME}... You did well with that fireship! The silver is on its way to my order, and that bastard Purig will no doubt be much discomfitted. You helped me out there, so let me see if I can now help you.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsBjolgur), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainAgent), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.MultiAgentConversationCondition))
				.NpcLine("{=5GMbKn4x}Just before I set sail for {SETTLEMENT_LINK}, my brothers and I had a visitor, a merchant named Salautas Crusas who said he was acting as an “ambassador” for Purig. He wanted us to break our contract with Balgard and ally with the Sea Hounds instead. He offered a great deal of money, too, and more - we could share in Purig's grand plan of conquest.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsBjolgur), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainAgent), null, null)
				.Condition(delegate
				{
					MBTextManager.SetTextVariable("SETTLEMENT_LINK", NavalStorylineData.Act3Quest3TargetSettlement.EncyclopediaLinkWithName, false);
					return true;
				})
				.GenerateToken(ref text)
				.GenerateToken(ref text2)
				.GenerateToken(ref text3)
				.GenerateToken(ref text4)
				.GenerateToken(ref text5)
				.GenerateToken(ref text6)
				.BeginPlayerOptions(null, false)
				.PlayerOption("{=0EVkbp01}What grand plans?", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsBjolgur), null, null)
				.GotoDialogState(text)
				.PlayerOption("{=jce9rAAu}I'm not interested in Purig's lies, just how to find him.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsBjolgur), null, null)
				.GotoDialogState(text2)
				.EndPlayerOptions()
				.NpcLine("{=n4bIAwNN}Well, first we would join the Sea Hounds in ravaging the coasts of Sturgia and Vlandia, so that no ship would dare sail on the Byalic Sea without paying us our due. Then Purig would raise an army out of the king's old enemies and take the Nordvyg, and crown himself in Thronderlag, and shower upon us lands, and titles, and anything else we might want.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsBjolgur), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainAgent), text, null)
				.NpcLine("{=2oEhDTjU}Well, some of the brothers listened to him, men who had fought against Volbjorn to whom a fine meal of wealth seasoned with revenge sounded rather tasty. But the rest of us… We'd heard such promises before, and we had no wish to serve any king. Better to fight for gold… and if you want the gold to flow, you honor your contracts, even if some fancy Calradian merchant comes along offering you the riches of the seven seas.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsBjolgur), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainAgent), null, text3)
				.NpcLine("{=3mxtyo2y}Here's the detail that would interest you…. In addition to all the other delights that Crusas dangled before us, he also offered to build us ships. Purig was going to construct them in some northern anchorage called Angranfjord, where he had brought a large number of captives to work in a shipyard.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsBjolgur), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainAgent), text2, text3)
				.NpcLine("{=GlV3EsEv}This must be the slave colony that Fahda mentioned. Pirates value safe havens to build new ships. With an anchorage like that, Purig can have the Sea Hounds out of his hands.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsGunnar), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainAgent), text3, text4)
				.NpcLine("{=v2664Qeo}...", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsGunnar), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainAgent), text4, null)
				.BeginPlayerOptions(null, false)
				.PlayerOption("{=WtODG7Mc}Bjolgur... you've known this for some time, you say?", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsBjolgur), null, null)
				.GotoDialogState(text5)
				.PlayerOption("{=X14bPFvN}Why didn't you tell us this before the battle?", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsBjolgur), null, null)
				.GotoDialogState(text6)
				.EndPlayerOptions()
				.NpcLine("{=7UNOf0DZ}Come now, I couldn't have you dash off to hunt Crusas before the silver got past the Sea Hounds. My brothers named me their emissary, you see, and we diplomats need to be crafty.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsBjolgur), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainAgent), text5, null)
				.PlayerLine("{=l8Rbjazw}It sounds as though, if we find Crusas, we can find Purig.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsBjolgur), null, null)
				.NpcLine("{=vhr55efV}So… I need to get this silver safely to harbor, but after that, I shall request permission from my order to fit out a ship and sail to Ostican to join your hunt. I'm not saying I owe you anything, mind you - but those bastards did try to take our money, and all Crusas' talk about gold and riches made me think that I wouldn't mind taking one of his ships and having a rummage through his holds.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsBjolgur), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainAgent), null, null)
				.PlayerLine("{=JEpBDamz}We are grateful for your help. We shall meet you back in Ostican.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsBjolgur), null, null)
				.NpcLine("{=Sl45Pmxg}I shall see you shortly in Ostican, then.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsBjolgur), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainAgent), null, null)
				.Consequence(delegate
				{
					Campaign.Current.ConversationManager.ConversationEndOneShot += this.FinishQuest;
				})
				.CloseDialog()
				.NpcLine("{=7UNOf0DZ}Come now, I couldn't have you dash off to hunt Crusas before the silver got past the Sea Hounds. My brothers named me their emissary, you see, and we diplomats need to be crafty.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsBjolgur), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainAgent), text6, null)
				.PlayerLine("{=U9e7WbOS}I piloted a fireship. I think you owe us more than just information.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsBjolgur), null, null)
				.NpcLine("{=vhr55efV}So… I need to get this silver safely to harbor, but after that, I shall request permission from my order to fit out a ship and sail to Ostican to join your hunt. I'm not saying I owe you anything, mind you - but those bastards did try to take our money, and all Crusas' talk about gold and riches made me think that I wouldn't mind taking one of his ships and having a rummage through his holds.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsBjolgur), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainAgent), null, null)
				.PlayerLine("{=8zxLaxKn}You'll get your share of Crusas' ill-gained wealth, never fear.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsBjolgur), null, null)
				.NpcLine("{=Sl45Pmxg}I shall see you shortly in Ostican, then.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsBjolgur), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainAgent), null, null)
				.Consequence(delegate
				{
					Campaign.Current.ConversationManager.ConversationEndOneShot += this.FinishQuest;
				})
				.CloseDialog()
				.CloseDialog(), this);
		}

		// Token: 0x060004F6 RID: 1270 RVA: 0x00020EC4 File Offset: 0x0001F0C4
		private void TalkToBjolgur()
		{
			Campaign.Current.CampaignMissionManager.OpenConversationMission(new ConversationCharacterData(CharacterObject.PlayerCharacter, PartyBase.MainParty, true, true, false, false, false, true), new ConversationCharacterData(NavalStorylineData.Bjolgur.CharacterObject, PartyBase.MainParty, true, true, false, false, false, true), "conversation_scene_sea_multi_agent", "", true);
		}

		// Token: 0x060004F7 RID: 1271 RVA: 0x00020F1C File Offset: 0x0001F11C
		private bool MultiAgentConversationCondition()
		{
			if (base.IsOngoing && Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero == NavalStorylineData.Bjolgur && this.HasBattleWon() && this.HasTalkedToSailors())
			{
				this.AddGunnarToConversation(false);
				StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, null, false);
				return true;
			}
			return false;
		}

		// Token: 0x060004F8 RID: 1272 RVA: 0x00020F70 File Offset: 0x0001F170
		private Vec3 GetGunnarTeleportPosition()
		{
			Vec3 vec = Mission.Current.GetRandomPositionAroundPoint(Agent.Main.Position + Agent.Main.LookRotation.s * 3f, 1f, 1.5f, false);
			int num = 20;
			while (Mission.Current.Scene.GetNavigationMeshForPosition(ref vec) == UIntPtr.Zero && num > 0)
			{
				vec = Mission.Current.GetRandomPositionAroundPoint(Agent.Main.Position + Agent.Main.LookRotation.s * 3f, 1f, 1.5f, false);
				num--;
			}
			return vec;
		}

		// Token: 0x060004F9 RID: 1273 RVA: 0x00021024 File Offset: 0x0001F224
		private void AddGunnarToConversation(bool isAgentSpawned)
		{
			Agent agent;
			if (!isAgentSpawned)
			{
				AgentBuildData agentBuildData = new AgentBuildData(NavalStorylineData.Gunnar.CharacterObject);
				agentBuildData.TroopOrigin(new SimpleAgentOrigin(agentBuildData.AgentCharacter, -1, null, default(UniqueTroopDescriptor)));
				Vec3 globalPosition = Mission.Current.Scene.FindEntityWithName("free_infantry_spawn_point_0").GlobalPosition;
				agentBuildData.InitialPosition(ref globalPosition);
				AgentBuildData agentBuildData2 = agentBuildData;
				Vec2 vec = Agent.Main.LookDirection.AsVec2;
				vec = vec.Normalized();
				agentBuildData2.InitialDirection(ref vec);
				agentBuildData.NoHorses(true);
				agent = Mission.Current.SpawnAgent(agentBuildData, false);
			}
			else
			{
				agent = Mission.Current.Agents.FirstOrDefault<Agent>((Agent x) => this.IsGunnar(x));
				this.RemoveWalkingBehavior(NavalStorylineData.Gunnar.CharacterObject);
				this.RemoveWalkingBehavior(NavalStorylineData.Bjolgur.CharacterObject);
			}
			ConversationManager conversationManager = Campaign.Current.ConversationManager;
			MBList<IAgent> mblist = new MBList<IAgent>();
			mblist.Add(agent);
			conversationManager.AddConversationAgents(mblist, true);
		}

		// Token: 0x060004FA RID: 1274 RVA: 0x00021120 File Offset: 0x0001F320
		private void RemoveWalkingBehavior(CharacterObject character)
		{
			Agent agent = Mission.Current.Agents.FirstOrDefault<Agent>((Agent x) => x.Character == character);
			CampaignAgentComponent component = agent.GetComponent<CampaignAgentComponent>();
			agent.ClearTargetFrame();
			AgentNavigator agentNavigator = component.AgentNavigator;
			if (agentNavigator == null)
			{
				return;
			}
			DailyBehaviorGroup behaviorGroup = agentNavigator.GetBehaviorGroup<DailyBehaviorGroup>();
			if (behaviorGroup == null)
			{
				return;
			}
			behaviorGroup.RemoveBehavior<WalkingBehavior>();
		}

		// Token: 0x060004FB RID: 1275 RVA: 0x0002117B File Offset: 0x0001F37B
		private void FinishQuest()
		{
			base.CompleteQuestWithSuccess();
		}

		// Token: 0x060004FC RID: 1276 RVA: 0x00021183 File Offset: 0x0001F383
		private bool IsGunnar(IAgent agent)
		{
			return agent.Character == NavalStorylineData.Gunnar.CharacterObject;
		}

		// Token: 0x060004FD RID: 1277 RVA: 0x00021197 File Offset: 0x0001F397
		private bool IsBjolgur(IAgent agent)
		{
			return agent.Character == NavalStorylineData.Bjolgur.CharacterObject;
		}

		// Token: 0x060004FE RID: 1278 RVA: 0x000211AB File Offset: 0x0001F3AB
		private bool IsMainAgent(IAgent agent)
		{
			return agent == Agent.Main;
		}

		// Token: 0x060004FF RID: 1279 RVA: 0x000211B5 File Offset: 0x0001F3B5
		private void RemoveHero(Hero hero)
		{
			hero.ChangeState(6);
			LocationComplex locationComplex = LocationComplex.Current;
			if (locationComplex != null)
			{
				locationComplex.RemoveCharacterIfExists(hero);
			}
			LeaveSettlementAction.ApplyForCharacterOnly(hero);
		}

		// Token: 0x06000500 RID: 1280 RVA: 0x000211D8 File Offset: 0x0001F3D8
		private void OnTalkedToSailors()
		{
			this.AddState(SpeakToTheSailorsQuest.QuestState.TalkedToSailors);
			TextObject textObject = new TextObject("{=FOQ5YOWH}You talked to {HERO.NAME}, and agreed to pilot a fireship and help the Sturgians run the Sea Hound blockade.", null);
			TextObjectExtensions.SetCharacterProperties(textObject, "HERO", NavalStorylineData.Bjolgur.CharacterObject, false);
			base.AddLog(textObject, false);
			Campaign.Current.ConversationManager.ConversationEndOneShot += delegate
			{
				this.RemoveHero(NavalStorylineData.Bjolgur);
				Mission.Current.EndMission();
			};
		}

		// Token: 0x06000501 RID: 1281 RVA: 0x00021234 File Offset: 0x0001F434
		private void CreateHoundsParty()
		{
			CampaignVec2 campaignVec = NavigationHelper.FindPointAroundPosition(MobileParty.MainParty.Position, 2, 3f, 1f, true, false);
			TextObject textObject = new TextObject("{=27QTvW27}Vlandian Pirates", null);
			this._houndsParty = CustomPartyComponent.CreateCustomPartyWithPartyTemplate(campaignVec, 1f, this._settlement, textObject, Clan.FindFirst((Clan x) => x.StringId == "northern_pirates"), this._houndsTemplate, null, "", "", 0f, false);
			this._houndsParty.SetPartyUsedByQuest(true);
			this._houndsParty.IsInfoHidden = true;
			this._houndsParty.IgnoreByOtherPartiesTill(CampaignTime.Never);
			this._houndsParty.Party.SetCustomBanner(NavalStorylineData.CorsairBanner);
			ShipUpgradePiece @object = MBObjectManager.Instance.GetObject<ShipUpgradePiece>("fore_heavy_ballista_pot");
			ShipUpgradePiece object2 = MBObjectManager.Instance.GetObject<ShipUpgradePiece>("sails_lvl2");
			ShipUpgradePiece object3 = MBObjectManager.Instance.GetObject<ShipUpgradePiece>("fore_heavy_ballista_pot");
			foreach (Ship ship in this._houndsParty.Ships)
			{
				if (ship.HasSlot("fore"))
				{
					if (ship.ShipHull.StringId == "burning_cog_ship")
					{
						ship.EquipUpgradePiece("fore", object3);
					}
					else
					{
						ship.EquipUpgradePiece("fore", @object);
					}
				}
				if (ship.HasSlot("sail") && ship.ShipHull.StringId != "burning_cog_ship")
				{
					ship.EquipUpgradePiece("sail", object2);
				}
			}
		}

		// Token: 0x06000502 RID: 1282 RVA: 0x000213EC File Offset: 0x0001F5EC
		private void CreateMerchantsParty()
		{
			CampaignVec2 campaignVec = NavigationHelper.FindPointAroundPosition(MobileParty.MainParty.Position, 2, 3f, 1f, true, false);
			TextObject textObject = new TextObject("{=CElcGl2R}Sturgian Merchants", null);
			this._merchantParty = CustomPartyComponent.CreateCustomPartyWithPartyTemplate(campaignVec, 3f, this._settlement, textObject, this._settlement.OwnerClan, this._merchantsTemplate, null, "", "", 0f, false);
			this._merchantParty.SetPartyUsedByQuest(true);
			this._merchantParty.IgnoreByOtherPartiesTill(CampaignTime.Never);
		}

		// Token: 0x06000503 RID: 1283 RVA: 0x00021478 File Offset: 0x0001F678
		private void StartBattle(bool fromCheckPoint)
		{
			this.AddState(SpeakToTheSailorsQuest.QuestState.BattleStarted);
			if (PartyBase.MainParty.MapEventSide == null)
			{
				PlayerEncounter.Start();
				PlayerEncounter.Current.SetupFields(this._houndsParty.Party, PartyBase.MainParty);
				PlayerEncounter.StartBattle();
			}
			this.CreateMerchantsParty();
			this._merchantParty.MapEventSide = PartyBase.MainParty.MapEventSide;
			NavalMissions.OpenBlockedEstuaryMission(this.GetMissionInitializerRecord(), this._houndsParty, fromCheckPoint);
		}

		// Token: 0x06000504 RID: 1284 RVA: 0x000214EB File Offset: 0x0001F6EB
		public void OnCheckPointReached()
		{
			this.AddState(SpeakToTheSailorsQuest.QuestState.CheckpointReached);
		}

		// Token: 0x06000505 RID: 1285 RVA: 0x000214F4 File Offset: 0x0001F6F4
		private MissionInitializerRecord GetMissionInitializerRecord()
		{
			MissionInitializerRecord navalMissionInitializerTemplate = NavalStorylineData.GetNavalMissionInitializerTemplate("naval_storyline_act_3_quest_3");
			TerrainType faceTerrainType = Campaign.Current.MapSceneWrapper.GetFaceTerrainType(MobileParty.MainParty.CurrentNavigationFace);
			navalMissionInitializerTemplate.TerrainType = faceTerrainType;
			navalMissionInitializerTemplate.NeedsRandomTerrain = false;
			navalMissionInitializerTemplate.PlayingInCampaignMode = true;
			navalMissionInitializerTemplate.RandomTerrainSeed = MBRandom.RandomInt(10000);
			navalMissionInitializerTemplate.AtmosphereOnCampaign = Campaign.Current.Models.MapWeatherModel.GetAtmosphereModel(MobileParty.MainParty.Position);
			navalMissionInitializerTemplate.SceneHasMapPatch = false;
			navalMissionInitializerTemplate.AtmosphereOnCampaign.NauticalInfo.UsesNavalSimulatedWater = 1;
			return navalMissionInitializerTemplate;
		}

		// Token: 0x06000506 RID: 1286 RVA: 0x0002158F File Offset: 0x0001F78F
		private void DestroyParty(ref MobileParty mobileParty)
		{
			if (mobileParty != null && mobileParty.IsActive)
			{
				if (mobileParty.MapEventSide != null)
				{
					mobileParty.MapEventSide = null;
				}
				DestroyPartyAction.Apply(null, mobileParty);
				mobileParty = null;
			}
		}

		// Token: 0x06000507 RID: 1287 RVA: 0x000215BA File Offset: 0x0001F7BA
		private bool HasTalkedToSailors()
		{
			return (this._state & SpeakToTheSailorsQuest.QuestState.TalkedToSailors) == SpeakToTheSailorsQuest.QuestState.TalkedToSailors;
		}

		// Token: 0x06000508 RID: 1288 RVA: 0x000215C7 File Offset: 0x0001F7C7
		private bool HasBattleStarted()
		{
			return (this._state & SpeakToTheSailorsQuest.QuestState.BattleStarted) == SpeakToTheSailorsQuest.QuestState.BattleStarted;
		}

		// Token: 0x06000509 RID: 1289 RVA: 0x000215D4 File Offset: 0x0001F7D4
		private bool HasBattleWon()
		{
			return (this._state & SpeakToTheSailorsQuest.QuestState.BattleWon) == SpeakToTheSailorsQuest.QuestState.BattleWon;
		}

		// Token: 0x0600050A RID: 1290 RVA: 0x000215E1 File Offset: 0x0001F7E1
		private bool CheckPointReached()
		{
			return (this._state & SpeakToTheSailorsQuest.QuestState.CheckpointReached) == SpeakToTheSailorsQuest.QuestState.CheckpointReached;
		}

		// Token: 0x0600050B RID: 1291 RVA: 0x000215EE File Offset: 0x0001F7EE
		private bool HadEncounterWithBjolgur()
		{
			return (this._state & SpeakToTheSailorsQuest.QuestState.HadEncounterWithBjolgor) == SpeakToTheSailorsQuest.QuestState.HadEncounterWithBjolgor;
		}

		// Token: 0x0600050C RID: 1292 RVA: 0x000215FD File Offset: 0x0001F7FD
		private void AddState(SpeakToTheSailorsQuest.QuestState state)
		{
			this._state |= state;
		}

		// Token: 0x04000283 RID: 643
		private const string SeaHoundsTemplateStringId = "storyline_act3_quest_3_sea_hounds_template";

		// Token: 0x04000284 RID: 644
		private const string MerchantsTemplateStringId = "storyline_act3_quest_3_merchants_template";

		// Token: 0x04000285 RID: 645
		private const string InterceptedMenuId = "hounds_3_intercepted";

		// Token: 0x04000286 RID: 646
		private const string EncounterMenuId = "quest3_encounter_invisible_menu";

		// Token: 0x04000287 RID: 647
		private const string BattleScene = "naval_storyline_act_3_quest_3";

		// Token: 0x04000288 RID: 648
		private const string ShipBallistaSlotId = "fore";

		// Token: 0x04000289 RID: 649
		private const string ShipSailSlotId = "sail";

		// Token: 0x0400028A RID: 650
		private const string BurningShipBallistaId = "fore_heavy_ballista_pot";

		// Token: 0x0400028B RID: 651
		private const string ExplosiveShipBallistaId = "fore_heavy_ballista_pot";

		// Token: 0x0400028C RID: 652
		private const string GalleySailId = "sails_lvl2";

		// Token: 0x0400028D RID: 653
		public const string FishingShipId = "burning_fishing_ship";

		// Token: 0x0400028E RID: 654
		public const string BurningTradeCogId = "burning_cog_ship";

		// Token: 0x0400028F RID: 655
		public const string TradeCogId = "ship_trade_cog_q3";

		// Token: 0x04000290 RID: 656
		private PartyTemplateObject _houndsTemplate;

		// Token: 0x04000291 RID: 657
		private PartyTemplateObject _merchantsTemplate;

		// Token: 0x04000292 RID: 658
		[SaveableField(0)]
		private Settlement _settlement;

		// Token: 0x04000293 RID: 659
		[SaveableField(1)]
		private MobileParty _houndsParty;

		// Token: 0x04000294 RID: 660
		private MobileParty _merchantParty;

		// Token: 0x04000295 RID: 661
		[SaveableField(2)]
		private SpeakToTheSailorsQuest.QuestState _state;

		// Token: 0x020001BC RID: 444
		public class SpeakToTheSailorsQuestTypeDefiner : SaveableTypeDefiner
		{
			// Token: 0x060019DA RID: 6618 RVA: 0x000AE340 File Offset: 0x000AC540
			public SpeakToTheSailorsQuestTypeDefiner()
				: base(312250)
			{
			}

			// Token: 0x060019DB RID: 6619 RVA: 0x000AE34D File Offset: 0x000AC54D
			protected override void DefineClassTypes()
			{
			}

			// Token: 0x060019DC RID: 6620 RVA: 0x000AE34F File Offset: 0x000AC54F
			protected override void DefineEnumTypes()
			{
				base.AddEnumDefinition(typeof(SpeakToTheSailorsQuest.QuestState), 100, null);
			}
		}

		// Token: 0x020001BD RID: 445
		[Flags]
		private enum QuestState
		{
			// Token: 0x04000D24 RID: 3364
			None = 0,
			// Token: 0x04000D25 RID: 3365
			TalkedToSailors = 1,
			// Token: 0x04000D26 RID: 3366
			BattleStarted = 2,
			// Token: 0x04000D27 RID: 3367
			BattleWon = 4,
			// Token: 0x04000D28 RID: 3368
			CheckpointReached = 8,
			// Token: 0x04000D29 RID: 3369
			HadEncounterWithBjolgor = 16
		}
	}
}
