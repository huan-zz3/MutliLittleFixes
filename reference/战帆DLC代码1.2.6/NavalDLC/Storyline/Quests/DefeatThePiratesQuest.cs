using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using NavalDLC.Missions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.SaveSystem;

namespace NavalDLC.Storyline.Quests
{
	// Token: 0x02000035 RID: 53
	public class DefeatThePiratesQuest : NavalStorylineQuestBase
	{
		// Token: 0x17000081 RID: 129
		// (get) Token: 0x0600033F RID: 831 RVA: 0x00017F3A File Offset: 0x0001613A
		public override NavalStorylineData.NavalStorylineStage Stage
		{
			get
			{
				return NavalStorylineData.NavalStorylineStage.Act2;
			}
		}

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x06000340 RID: 832 RVA: 0x00017F3D File Offset: 0x0001613D
		public override bool WillProgressStoryline
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x06000341 RID: 833 RVA: 0x00017F40 File Offset: 0x00016140
		protected override string MainPartyTemplateStringId
		{
			get
			{
				return "storyline_act_2_main_party_template";
			}
		}

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x06000342 RID: 834 RVA: 0x00017F47 File Offset: 0x00016147
		public int PirateTroopCount
		{
			get
			{
				return this._pirateTemplate.Stacks.Sum<PartyTemplateStack>((PartyTemplateStack t) => t.MaxValue);
			}
		}

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x06000343 RID: 835 RVA: 0x00017F78 File Offset: 0x00016178
		public override TextObject Title
		{
			get
			{
				return new TextObject("{=wKBtraSp}Defeat the Sea Hounds", null);
			}
		}

		// Token: 0x06000344 RID: 836 RVA: 0x00017F85 File Offset: 0x00016185
		public DefeatThePiratesQuest(string questId, Hero questGiver)
			: base(questId, questGiver, CampaignTime.Never, 0)
		{
			this._pirateTemplate = Campaign.Current.ObjectManager.GetObject<PartyTemplateObject>("storyline_act_2_sea_hounds_template");
			base.AddLog(this._descriptionLogText, false);
		}

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x06000345 RID: 837 RVA: 0x00017FBD File Offset: 0x000161BD
		private TextObject _descriptionLogText
		{
			get
			{
				return new TextObject("{=VWK3jIqG}Defeat the two Sea Hound vessels that are lying in wait outside of Ostican.", null);
			}
		}

		// Token: 0x06000346 RID: 838 RVA: 0x00017FCC File Offset: 0x000161CC
		protected override void SetDialogs()
		{
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1200).NpcLine("{=NW5vE1xa}That's one Sea Hound defeated, but the other can't be too far away. We've captured a second ship, though. It's a snekkja - it should be quick and nimble. How about you cross over and take the helm? I'll keep command of our old knarr.", null, null, null, null).Condition(delegate
			{
				Mission mission = Mission.Current;
				PirateBattleMissionController pirateBattleMissionController = ((mission != null) ? mission.GetMissionBehavior<PirateBattleMissionController>() : null);
				return Hero.OneToOneConversationHero == NavalStorylineData.Gunnar && pirateBattleMissionController != null && pirateBattleMissionController.IsFirstShipCleared;
			})
				.BeginPlayerOptions(null, false)
				.PlayerOption("{=alDwmQtB}I'll go do that.", null, null, null)
				.Consequence(delegate
				{
					PirateBattleMissionController missionBehavior = Mission.Current.GetMissionBehavior<PirateBattleMissionController>();
					Campaign.Current.ConversationManager.ConversationEndOneShot += missionBehavior.OnPlayerSelectedSecondShipToCommand;
				})
				.NpcLine("{=qauwgx3r}Splendid. Let's go chase down that second Sea Hound.", null, null, null, null)
				.CloseDialog()
				.PlayerOption("{=cnjTiMmv}Very good. I'll keep command of our old knarr. You captain this agile snekkja.", null, null, null)
				.Consequence(delegate
				{
					PirateBattleMissionController missionBehavior2 = Mission.Current.GetMissionBehavior<PirateBattleMissionController>();
					Campaign.Current.ConversationManager.ConversationEndOneShot += missionBehavior2.OnPlayerSelectedFirstShipToCommand;
				})
				.NpcLine("{=qauwgx3r}Splendid. Let's go chase down that second Sea Hound.", null, null, null, null)
				.CloseDialog()
				.EndPlayerOptions()
				.CloseDialog(), this);
			string text = "";
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1200).NpcLine("{=dF7jeK5a}I'm new at this, my {?PLAYER.GENDER}lady{?}lord{\\?}! I'm just a farmer who fell on hard times. I signed on with this ship in Varcheg a month ago. They told me we'd be trading grain and ivory across the Byalic. I didn't know we'd be attacking honest folk like yourselves![ib:weary]", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsPirate), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), null, null).Condition(delegate
			{
				StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, null, false);
				CharacterObject oneToOneConversationCharacter = CharacterObject.OneToOneConversationCharacter;
				MobileParty pirateParty = this._pirateParty;
				return oneToOneConversationCharacter == ConversationHelper.GetConversationCharacterPartyLeader((pirateParty != null) ? pirateParty.Party : null);
			})
				.Consequence(delegate
				{
					AgentBuildData agentBuildData = new AgentBuildData(NavalStorylineData.Gunnar.CharacterObject);
					agentBuildData.TroopOrigin(new SimpleAgentOrigin(agentBuildData.AgentCharacter, -1, null, default(UniqueTroopDescriptor)));
					Vec3 globalPosition = Mission.Current.Scene.FindEntityWithName("free_infantry_spawn_point_0").GlobalPosition;
					agentBuildData.InitialPosition(ref globalPosition);
					AgentBuildData agentBuildData2 = agentBuildData;
					Vec2 vec = Agent.Main.LookDirection.AsVec2;
					vec = vec.Normalized();
					agentBuildData2.InitialDirection(ref vec);
					if (Mission.Current != null)
					{
						Agent agent = Mission.Current.SpawnAgent(agentBuildData, false);
						Campaign.Current.ConversationManager.AddConversationAgents(new List<IAgent> { agent }, true);
					}
				})
				.NpcLine("{=GsPj9ptT}Listen - these Sea Hounds are trolls and demons, not men! I want no part of this any more! Spare me, and I promise I'll go back to my old life.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsPirate), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), null, null)
				.BeginPlayerOptions(null, false)
				.PlayerOption("{=LBoq4sXI}Tell me the truth, and I'll let you live.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsPirate), null, text)
				.PlayerOption("{=wTEbf3gc}I am looking for my sister. Let me know how to find her, and we will spare your life.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsPirate), null, text)
				.EndPlayerOptions()
				.GenerateToken(ref text)
				.NpcLine("{=Q3bpobtL}We purchased some slaves from some bandits in Ostican. We were planning on selling them onward to another buyer further south along the coast. Perhaps your sister was one of them? Will you spare me?", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsPirate), null, null, null)
				.NpcLine("{=b1saAIdA}Are you really a farmer, now? Callouses such as those on your hands are made by oars, not ploughs. And I see a scar on your sword-arm that doesn't look like it came from the kick of a mule. Indeed, I might even recall your name. Hralgar Eel-Nose, is it not?", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsGunnar), null, null, null)
				.NpcLine("{=tiHQafDb}[if:convo_predatory][ib:warrior]Gunnar of Langshofn… Three of your old shipmates have we visited while reeving. One died well. The others… It's said that your people are mean and stingy hosts, but those two gave us some fine entertainment.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsPirate), null, null, null)
				.NpcLine("{=yhEKOBfT}As for you, friend of Gunnar... I told you where to seek your sister. Best rescue her quick, or she may take a liking to one of our brave lads and give you a litter of Sea Puppies. So there you have it… I fulfilled my end of the bargain. Put me ashore.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsPirate), null, null, null)
				.BeginPlayerOptions(null, false)
				.PlayerOption("{=00iNZpwG}You lied. The bargain is void. Gunnar, do what you will with him.", null, null, null)
				.Consequence(delegate
				{
					Campaign.Current.ConversationManager.ConversationEndOneShot += this.OnOption1Chosen;
				})
				.CloseDialog()
				.PlayerOption("{=RSBjrwHG}We will spare your life, but the sea may have other plans for you. Over the side you go.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsPirate), null, null)
				.Consequence(delegate
				{
					Campaign.Current.ConversationManager.ConversationEndOneShot += this.OnOption2Chosen;
				})
				.CloseDialog()
				.PlayerOption("{=KfQHGUID}I keep my bargains, however loathsome they may be. We shall put you ashore.", null, null, null)
				.Consequence(delegate
				{
					Campaign.Current.ConversationManager.ConversationEndOneShot += this.OnOption3Chosen;
				})
				.CloseDialog()
				.EndPlayerOptions()
				.CloseDialog(), this);
		}

		// Token: 0x06000347 RID: 839 RVA: 0x00018294 File Offset: 0x00016494
		private void AddGameMenus()
		{
			base.AddGameMenu("quest3_retry_menu", new TextObject("{=etH1IHNZ}You manage to put some distance between you and your enemies, and you have a moment to consider how to proceed.", null), new OnInitDelegate(this.retry_menu_on_init), 0, 0);
			base.AddGameMenuOption("quest3_retry_menu", "try_again_option", new TextObject("{=YHMDy3lQ}Try again", null), new GameMenuOption.OnConditionDelegate(this.retry_menu_try_again_on_condition), new GameMenuOption.OnConsequenceDelegate(this.retry_menu_try_again_on_consequence), false, -1);
			base.AddGameMenuOption("quest3_retry_menu", "leave_option", new TextObject("{=3sRdGQou}Leave", null), new GameMenuOption.OnConditionDelegate(this.leave_on_condition), new GameMenuOption.OnConsequenceDelegate(this.leave_on_consequence), true, -1);
			base.AddGameMenu("quest3_encounter_menu", new TextObject("{=Mv2qMTmx}As you sail out of Ostican harbor you spot a single ship, anchored just offshore. As soon as it sights you it runs out its oars and steers to intercept your course. It is not waiting for its partner, and is probably not expecting you to put up much of a fight.", null), new OnInitDelegate(this.encounter_menu_on_init), 0, 0);
			base.AddGameMenuOption("quest3_encounter_menu", "fight_option", new TextObject("{=Ky03jg94}Fight", null), new GameMenuOption.OnConditionDelegate(this.encounter_menu_attack_on_condition), new GameMenuOption.OnConsequenceDelegate(this.encounter_menu_attack_on_consequence), false, -1);
		}

		// Token: 0x06000348 RID: 840 RVA: 0x00018388 File Offset: 0x00016588
		private bool retry_menu_try_again_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 12;
			return this._battleFinished && !this._battleWon;
		}

		// Token: 0x06000349 RID: 841 RVA: 0x000183A5 File Offset: 0x000165A5
		private void retry_menu_try_again_on_consequence(MenuCallbackArgs args)
		{
			this.OnRetry();
		}

		// Token: 0x0600034A RID: 842 RVA: 0x000183AD File Offset: 0x000165AD
		private bool leave_on_condition(MenuCallbackArgs args)
		{
			args.Tooltip = new TextObject("{=wmTjX28f}This will exit story mode and return you to the Sandbox. You can continue the storyline later by talking to Gunnar in the port again.", null);
			args.optionLeaveType = 16;
			return true;
		}

		// Token: 0x0600034B RID: 843 RVA: 0x000183C9 File Offset: 0x000165C9
		private void leave_on_consequence(MenuCallbackArgs args)
		{
			base.CompleteQuestWithCancel(null);
			NavalStorylineData.DeactivateNavalStoryline();
		}

		// Token: 0x0600034C RID: 844 RVA: 0x000183D7 File Offset: 0x000165D7
		private void retry_menu_on_init(MenuCallbackArgs args)
		{
			args.MenuContext.SetBackgroundMeshName("encounter_naval");
			if (this._battleFinished && this._battleWon)
			{
				this.OnPlayerWon();
			}
		}

		// Token: 0x0600034D RID: 845 RVA: 0x000183FF File Offset: 0x000165FF
		private void encounter_menu_on_init(MenuCallbackArgs args)
		{
			args.MenuContext.SetBackgroundMeshName("encounter_naval");
			NavalStorylineData.OnCheckpointReached(NavalStorylineData.NavalStorylineCheckpoint.Act2EncounterMenu);
			MobileParty.MainParty.SetMoveModeHold();
			MobileParty pirateParty = this._pirateParty;
			if (pirateParty == null)
			{
				return;
			}
			pirateParty.SetMoveModeHold();
		}

		// Token: 0x0600034E RID: 846 RVA: 0x00018431 File Offset: 0x00016631
		private bool encounter_menu_attack_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 12;
			return true;
		}

		// Token: 0x0600034F RID: 847 RVA: 0x0001843C File Offset: 0x0001663C
		private void encounter_menu_attack_on_consequence(MenuCallbackArgs args)
		{
			this.StartBattle();
		}

		// Token: 0x06000350 RID: 848 RVA: 0x00018444 File Offset: 0x00016644
		private bool IsGunnar(IAgent agent)
		{
			return agent.Character == NavalStorylineData.Gunnar.CharacterObject;
		}

		// Token: 0x06000351 RID: 849 RVA: 0x00018458 File Offset: 0x00016658
		private bool IsPirate(IAgent agent)
		{
			return agent.Character.StringId == "sea_hounds";
		}

		// Token: 0x06000352 RID: 850 RVA: 0x0001846F File Offset: 0x0001666F
		private bool IsMainHero(IAgent agent)
		{
			return agent.Character == CharacterObject.PlayerCharacter;
		}

		// Token: 0x06000353 RID: 851 RVA: 0x0001847E File Offset: 0x0001667E
		private void OnOption1Chosen()
		{
			GainRenownAction.Apply(Hero.MainHero, 10f, false);
			TraitLevelingHelper.OnIssueSolvedThroughQuest(Hero.MainHero, DefaultTraits.Honor, -5);
			base.CompleteQuestWithSuccess();
		}

		// Token: 0x06000354 RID: 852 RVA: 0x000184A7 File Offset: 0x000166A7
		private void OnOption2Chosen()
		{
			GainRenownAction.Apply(Hero.MainHero, 5f, false);
			base.CompleteQuestWithSuccess();
		}

		// Token: 0x06000355 RID: 853 RVA: 0x000184BF File Offset: 0x000166BF
		private void OnOption3Chosen()
		{
			TraitLevelingHelper.OnIssueSolvedThroughQuest(Hero.MainHero, DefaultTraits.Honor, 20);
			base.CompleteQuestWithSuccess();
		}

		// Token: 0x06000356 RID: 854 RVA: 0x000184D8 File Offset: 0x000166D8
		protected override void InitializeQuestOnGameLoadInternal()
		{
			this._pirateTemplate = Campaign.Current.ObjectManager.GetObject<PartyTemplateObject>("storyline_act_2_sea_hounds_template");
			this.AddGameMenus();
			this.SetDialogs();
			if (MobileParty.MainParty.IsActive)
			{
				NavalDLCHelpers.SetCustomSailPatternOfPartyShips(MobileParty.MainParty, "generated_square__h4_09");
			}
			if (this._pirateParty != null && this._pirateParty.IsActive)
			{
				NavalDLCHelpers.SetCustomSailPatternOfPartyShips(this._pirateParty, "generated_square_l1_h4_10");
			}
		}

		// Token: 0x06000357 RID: 855 RVA: 0x0001854C File Offset: 0x0001674C
		protected override void OnStartQuestInternal()
		{
			this.SetDialogs();
			this.AddGameMenus();
			this.SpawnPirates(NavalStorylineData.HomeSettlement);
			MobileParty.MainParty.IgnoreByOtherPartiesTill(base.QuestDueTime);
			NavalDLCHelpers.SetCustomSailPatternOfPartyShips(MobileParty.MainParty, "generated_square__h4_09");
			NavalDLCHelpers.AddUpgradePiecesToPartyShips(MobileParty.MainParty, DefeatThePiratesQuest.PlayerShipUpgradePieces, null);
		}

		// Token: 0x06000358 RID: 856 RVA: 0x000185A0 File Offset: 0x000167A0
		protected override void HourlyTick()
		{
			if (this._pirateParty != null && MobileParty.MainParty.Position.DistanceSquared(this._pirateParty.Position) <= Campaign.Current.Models.EncounterModel.GetEncounterJoiningRadius * Campaign.Current.Models.EncounterModel.GetEncounterJoiningRadius * 1.5f)
			{
				GameMenu.ActivateGameMenu("quest3_encounter_menu");
			}
		}

		// Token: 0x06000359 RID: 857 RVA: 0x00018610 File Offset: 0x00016810
		private void StartBattle()
		{
			foreach (Ship ship in this._pirateParty.Ships)
			{
				ship.IsInvulnerable = false;
			}
			PlayerEncounter.RestartPlayerEncounter(PartyBase.MainParty, this._pirateParty.Party, false, false);
			PlayerEncounter.StartBattle();
			GameMenu.ActivateGameMenu("quest3_retry_menu");
			this.OpenPirateBattleMission();
		}

		// Token: 0x0600035A RID: 858 RVA: 0x00018694 File Offset: 0x00016894
		private void OpenPirateBattleMission()
		{
			MissionInitializerRecord navalMissionInitializerTemplate = NavalStorylineData.GetNavalMissionInitializerTemplate("naval_storyline_act_2_tutorial");
			navalMissionInitializerTemplate.PlayingInCampaignMode = true;
			navalMissionInitializerTemplate.AtmosphereOnCampaign = Campaign.Current.Models.MapWeatherModel.GetAtmosphereModel(MobileParty.MainParty.Position);
			NavalMissions.OpenNavalStorylinePirateBattleMission(navalMissionInitializerTemplate, this._pirateParty, this.PirateTroopCount);
		}

		// Token: 0x0600035B RID: 859 RVA: 0x000186EC File Offset: 0x000168EC
		protected override void RegisterEventsInternal()
		{
			CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
			CampaignEvents.OnSettlementLeftEvent.AddNonSerializedListener(this, new Action<MobileParty, Settlement>(this.OnSettlementLeft));
			CampaignEvents.OnMissionEndedEvent.AddNonSerializedListener(this, new Action<IMission>(this.OnMissionEnded));
		}

		// Token: 0x0600035C RID: 860 RVA: 0x00018740 File Offset: 0x00016940
		private void OnSettlementLeft(MobileParty party, Settlement settlement)
		{
			if (party == MobileParty.MainParty && this._pirateParty != null)
			{
				this._pirateParty.Ai.SetDoNotMakeNewDecisions(false);
				this._pirateParty.SetMoveEngageParty(MobileParty.MainParty, 2);
				this._pirateParty.Ai.SetDoNotMakeNewDecisions(true);
			}
		}

		// Token: 0x0600035D RID: 861 RVA: 0x00018790 File Offset: 0x00016990
		private void OnMissionEnded(IMission mission)
		{
			if (PlayerEncounter.Current != null)
			{
				PartyBase encounteredParty = PlayerEncounter.EncounteredParty;
				MobileParty pirateParty = this._pirateParty;
				if (encounteredParty == ((pirateParty != null) ? pirateParty.Party : null))
				{
					this._battleFinished = true;
					this._battleWon = false;
					if (PlayerEncounter.Battle != null && PlayerEncounter.BattleState == 1)
					{
						this._battleWon = true;
					}
					Hero.MainHero.Heal(Hero.MainHero.MaxHitPoints, false);
				}
			}
		}

		// Token: 0x0600035E RID: 862 RVA: 0x000187F8 File Offset: 0x000169F8
		private void OnSettlementEntered(MobileParty party, Settlement settlement, Hero hero)
		{
			if (party == MobileParty.MainParty && this._pirateParty != null)
			{
				this._pirateParty.Ai.SetDoNotMakeNewDecisions(false);
				this._pirateParty.SetMovePatrolAroundPoint(settlement.PortPosition, 2);
				this._pirateParty.Ai.SetDoNotMakeNewDecisions(true);
			}
		}

		// Token: 0x0600035F RID: 863 RVA: 0x0001884C File Offset: 0x00016A4C
		protected override void OnFinalizeInternal()
		{
			if (PlayerEncounter.Battle != null && PlayerEncounter.Battle.InvolvedParties.Contains(this._pirateParty.Party))
			{
				PlayerEncounter.Finish(true);
			}
			if (this._pirateParty != null)
			{
				if (this._pirateParty.IsActive)
				{
					this._pirateParty.Ai.DisableAi();
					DestroyPartyAction.Apply(null, this._pirateParty);
				}
				this._pirateParty = null;
			}
			MobileParty.MainParty.IgnoreByOtherPartiesTill(CampaignTime.Now);
		}

		// Token: 0x06000360 RID: 864 RVA: 0x000188C9 File Offset: 0x00016AC9
		protected override void OnCompleteWithSuccessInternal()
		{
			NavalStorylineData.OnCheckpointReached(NavalStorylineData.NavalStorylineCheckpoint.Act2Finalized);
		}

		// Token: 0x06000361 RID: 865 RVA: 0x000188D1 File Offset: 0x00016AD1
		private void OnPlayerWon()
		{
			this.StartConversationWithPirate();
		}

		// Token: 0x06000362 RID: 866 RVA: 0x000188DC File Offset: 0x00016ADC
		private void StartConversationWithPirate()
		{
			CharacterObject @object = Campaign.Current.ObjectManager.GetObject<CharacterObject>("sea_hounds");
			this._pirateParty.Party.AddElementToMemberRoster(@object, 1, false);
			CharacterObject conversationCharacterPartyLeader = ConversationHelper.GetConversationCharacterPartyLeader(this._pirateParty.Party);
			ConversationCharacterData conversationCharacterData = new ConversationCharacterData(CharacterObject.PlayerCharacter, PartyBase.MainParty, true, false, false, false, false, true);
			ConversationCharacterData conversationCharacterData2;
			conversationCharacterData2..ctor(conversationCharacterPartyLeader, this._pirateParty.Party, true, false, false, false, false, true);
			CampaignMission.OpenConversationMission(conversationCharacterData, conversationCharacterData2, "conversation_scene_sea_multi_agent", "", true);
		}

		// Token: 0x06000363 RID: 867 RVA: 0x00018963 File Offset: 0x00016B63
		private void OnRetry()
		{
			this.RefreshPiratePartyForces();
			this._battleFinished = false;
			this._battleWon = false;
			this.OpenPirateBattleMission();
		}

		// Token: 0x06000364 RID: 868 RVA: 0x00018980 File Offset: 0x00016B80
		private void SpawnPirates(Settlement settlement)
		{
			Clan clan = Clan.All.FirstOrDefault<Clan>((Clan t) => t.StringId == "northern_pirates");
			CampaignVec2 campaignVec = NavigationHelper.FindReachablePointAroundPosition(settlement.PortPosition, 2, 20f, 10f, false);
			TextObject textObject = new TextObject("{=SKC3FeGR}Sea Hounds", null);
			this._pirateParty = CustomPartyComponent.CreateCustomPartyWithPartyTemplate(campaignVec, 0.5f, SettlementHelper.FindRandomHideout((Settlement t) => t.IsHideout), textObject, clan, this._pirateTemplate, NavalStorylineData.Purig, "", "", 0f, false);
			this._pirateParty.Party.SetCustomName(textObject);
			this._pirateParty.InitializeMobilePartyAtPosition(campaignVec);
			this._pirateParty.SetLandNavigationAccess(false);
			this._pirateParty.Party.SetVisualAsDirty();
			this._pirateParty.ActualClan = clan;
			this._pirateParty.SetPartyUsedByQuest(true);
			this._pirateParty.Party.SetCustomBanner(NavalStorylineData.CorsairBanner);
			(this._pirateParty.PartyComponent as CustomPartyComponent).SetBaseSpeed(2.5f);
			this._pirateParty.IgnoreByOtherPartiesTill(CampaignTime.Never);
			this._pirateParty.SetMoveEngageParty(MobileParty.MainParty, 2);
			this._pirateParty.Ai.SetDoNotMakeNewDecisions(true);
			NavalDLCHelpers.AddUpgradePiecesToPartyShips(this._pirateParty, DefeatThePiratesQuest.PirateShipUpgradePieces, null);
			NavalDLCHelpers.SetCustomSailPatternOfPartyShips(this._pirateParty, "generated_square_l1_h4_10");
		}

		// Token: 0x06000365 RID: 869 RVA: 0x00018B00 File Offset: 0x00016D00
		private void RefreshPiratePartyForces()
		{
			this._pirateParty.MemberRoster.Clear();
			CharacterObject @object = Campaign.Current.ObjectManager.GetObject<CharacterObject>("sea_hounds");
			this._pirateParty.AddElementToMemberRoster(@object, this.PirateTroopCount * 2, false);
			foreach (Ship ship in this._pirateParty.Ships.ToList<Ship>())
			{
				ship.Owner = null;
			}
			foreach (ShipTemplateStack shipTemplateStack in this._pirateTemplate.ShipHulls)
			{
				Ship ship2 = new Ship(shipTemplateStack.ShipHull);
				ship2.Owner = this._pirateParty.Party;
				ship2.IsInvulnerable = true;
			}
		}

		// Token: 0x06000366 RID: 870 RVA: 0x00018BF8 File Offset: 0x00016DF8
		public bool IsPiratePartyVisible()
		{
			return this._pirateParty != null && this._pirateParty.IsActive && this._pirateParty.IsVisible;
		}

		// Token: 0x0400020B RID: 523
		private const string EncounterMenuId = "quest3_encounter_menu";

		// Token: 0x0400020C RID: 524
		private const string RetryMenuId = "quest3_retry_menu";

		// Token: 0x0400020D RID: 525
		private const string PiratePartyTemplateStringId = "storyline_act_2_sea_hounds_template";

		// Token: 0x0400020E RID: 526
		private const string PirateConversationCharacterId = "sea_hounds";

		// Token: 0x0400020F RID: 527
		public const string PlayerPartySailPatternId = "generated_square__h4_09";

		// Token: 0x04000210 RID: 528
		public const string PiratePartySailPatternId = "generated_square_l1_h4_10";

		// Token: 0x04000211 RID: 529
		private static readonly Dictionary<string, string> PlayerShipUpgradePieces = new Dictionary<string, string> { { "sail", "sails_lvl2" } };

		// Token: 0x04000212 RID: 530
		private static readonly Dictionary<string, string> PirateShipUpgradePieces = new Dictionary<string, string> { { "sail", "sails_lvl2" } };

		// Token: 0x04000213 RID: 531
		[SaveableField(1)]
		private MobileParty _pirateParty;

		// Token: 0x04000214 RID: 532
		[SaveableField(2)]
		private bool _battleWon;

		// Token: 0x04000215 RID: 533
		[SaveableField(3)]
		private bool _battleFinished;

		// Token: 0x04000216 RID: 534
		private PartyTemplateObject _pirateTemplate;
	}
}
