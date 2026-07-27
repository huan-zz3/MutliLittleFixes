using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Helpers;
using NavalDLC.Storyline.Quests;
using SandBox;
using SandBox.GameComponents;
using SandBox.Missions.AgentBehaviors;
using SandBox.Missions.MissionLogics;
using StoryMode;
using StoryMode.StoryModeObjects;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace NavalDLC.Storyline.CampaignBehaviors
{
	// Token: 0x02000072 RID: 114
	public class NavalStorylineCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x060007C1 RID: 1985 RVA: 0x00037918 File Offset: 0x00035B18
		private void OnNewGameCreated(CampaignGameStarter starter)
		{
			if (NavalStorylineData.Gunnar.IsDisabled || NavalStorylineData.Gunnar.IsNotSpawned)
			{
				NavalStorylineData.Gunnar.ChangeState(1);
			}
			if (NavalStorylineData.Gunnar.PartyBelongedTo == null && NavalStorylineData.Gunnar.StayingInSettlement == null)
			{
				EnterSettlementAction.ApplyForCharacterOnly(NavalStorylineData.Gunnar, NavalStorylineData.HomeSettlement);
			}
		}

		// Token: 0x060007C2 RID: 1986 RVA: 0x00037970 File Offset: 0x00035B70
		public override void RegisterEvents()
		{
			if (!this._isNavalStorylineCanceled)
			{
				CampaignEvents.TickEvent.AddNonSerializedListener(this, new Action<float>(this.Tick));
				CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
				CampaignEvents.OnHeirSelectionOverEvent.AddNonSerializedListener(this, new Action<Hero>(this.OnHeirSelectionOver));
				CampaignEvents.CanHeroDieEvent.AddNonSerializedListener(this, new ReferenceAction<Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.CanHeroDie));
				CampaignEvents.CanHeroBecomePrisonerEvent.AddNonSerializedListener(this, new ReferenceAction<Hero, bool>(this.CanHeroBecomePrisoner));
				CampaignEvents.CanHaveCampaignIssuesEvent.AddNonSerializedListener(this, new ReferenceAction<Hero, bool>(this.CanHaveCampaignIssues));
				CampaignEvents.OnMobilePartyNavigationStateChangedEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.OnMobilePartyNavigationStateChanged));
				CampaignEvents.OnSettlementLeftEvent.AddNonSerializedListener(this, new Action<MobileParty, Settlement>(this.OnSettlementLeft));
				CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.HourlyTick));
				CampaignEvents.OnQuestCompletedEvent.AddNonSerializedListener(this, new Action<QuestBase, QuestBase.QuestCompleteDetails>(this.OnQuestCompleted));
				CampaignEvents.OnNewGameCreatedPartialFollowUpEndEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnNewGameCreated));
				CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
				NavalDLCEvents.OnNavalStorylineTutorialSkippedEvent.AddNonSerializedListener(this, new Action(this.OnNavalStorylineSkipped));
				NavalDLCEvents.OnNavalStorylineCanceledEvent.AddNonSerializedListener(this, new Action<NavalStorylineData.StorylineCancelDetail>(this.OnNavalStorylineCanceled));
				CampaignEvents.AfterMissionStarted.AddNonSerializedListener(this, new Action<IMission>(this.AfterMissionStarted));
				CampaignEvents.MissionTickEvent.AddNonSerializedListener(this, new Action<float>(this.MissionTickEvent));
				CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener(this, new Action(this.OnGameLoadFinished));
				CampaignEvents.OnMissionEndedEvent.AddNonSerializedListener(this, new Action<IMission>(this.OnMissionEnded));
				CampaignEvents.HeroComesOfAgeEvent.AddNonSerializedListener(this, new Action<Hero>(this.OnHeroComesOfAge));
				CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.OnHeroKilled));
			}
		}

		// Token: 0x060007C3 RID: 1987 RVA: 0x00037B54 File Offset: 0x00035D54
		private void OnMissionEnded(IMission mission)
		{
			if (this._isNavalStorylineActive && this._activeMissionType != NavalStorylineData.NavalStorylineSetPieceBattleMissionTypes.None)
			{
				this._activeMissionType = NavalStorylineData.NavalStorylineSetPieceBattleMissionTypes.None;
			}
		}

		// Token: 0x060007C4 RID: 1988 RVA: 0x00037B70 File Offset: 0x00035D70
		private void HourlyTick()
		{
			if (MobileParty.MainParty.MapEvent == null && MobileParty.MainParty.SiegeEvent == null && Hero.MainHero.IsActive && !MobileParty.MainParty.IsInRaftState && this.IsWaitingForSistersReturn() && this._sisterReturnTime.IsPast)
			{
				this.ShowSisterPopUp();
				this._sisterReturnTime = CampaignTime.Zero;
			}
		}

		// Token: 0x060007C5 RID: 1989 RVA: 0x00037BD4 File Offset: 0x00035DD4
		private void ShowSisterPopUp()
		{
			object obj = new TextObject("{=FdXpi6Ql}Word from Gunnar", null);
			TextObject textObject = new TextObject("{=9AjMDDOJ}You receive a message from Gunnar urging you to hurry back to Ostican. He has found and ransomed your sister.", null);
			TextObject textObject2 = new TextObject("{=DM6luo3c}Continue", null);
			InformationManager.ShowInquiry(new InquiryData(obj.ToString(), textObject.ToString(), true, false, textObject2.ToString(), string.Empty, new Action(this.OnSisterRansomed), null, "", 0f, null, null, null), true, false);
		}

		// Token: 0x060007C6 RID: 1990 RVA: 0x00037C42 File Offset: 0x00035E42
		private void OnSisterRansomed()
		{
			NavalStorylineData.Gunnar.ChangeState(1);
			TeleportHeroAction.ApplyImmediateTeleportToSettlement(NavalStorylineData.Gunnar, NavalStorylineData.HomeSettlement);
			this._lastCompletedStorylineStage = NavalStorylineData.NavalStorylineStage.Act3SpeakToGunnarAndSister;
			NavalDLCEvents.Instance.OnSisterRansomed();
		}

		// Token: 0x060007C7 RID: 1991 RVA: 0x00037C6F File Offset: 0x00035E6F
		private bool IsSister(IAgent agent)
		{
			return agent.Character == StoryModeHeroes.LittleSister.CharacterObject;
		}

		// Token: 0x060007C8 RID: 1992 RVA: 0x00037C83 File Offset: 0x00035E83
		private bool IsGunnar(IAgent agent)
		{
			return agent.Character == NavalStorylineData.Gunnar.CharacterObject;
		}

		// Token: 0x060007C9 RID: 1993 RVA: 0x00037C97 File Offset: 0x00035E97
		private bool IsPlayer(IAgent agent)
		{
			return agent.Character == Hero.MainHero.CharacterObject;
		}

		// Token: 0x060007CA RID: 1994 RVA: 0x00037CAB File Offset: 0x00035EAB
		private void OnHeroComesOfAge(Hero hero)
		{
			if (hero == StoryModeHeroes.LittleSister && NavalStorylineData.HasCompletedLast(NavalStorylineData.NavalStorylineStage.Act3Quest5))
			{
				StoryModeHelpers.SetPlayerSiblingsSkillsIfNeeded(hero);
			}
		}

		// Token: 0x060007CB RID: 1995 RVA: 0x00037CC4 File Offset: 0x00035EC4
		private void OnGameLoadFinished()
		{
			if (StoryModeHeroes.LittleSister.IsAlive && MBSaveLoad.IsUpdatingGameVersion && MBSaveLoad.LastLoadedGameVersion < ApplicationVersion.FromString("v1.3.15.109185", 0))
			{
				AgingCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<AgingCampaignBehavior>();
				FieldInfo field = typeof(AgingCampaignBehavior).GetField("_heroesYoungerThanHeroComesOfAge", BindingFlags.Instance | BindingFlags.NonPublic);
				Dictionary<Hero, int> dictionary = ((campaignBehavior != null) ? ((Dictionary<Hero, int>)field.GetValue(campaignBehavior)) : null);
				bool flag = NavalStorylineData.HasCompletedLast(NavalStorylineData.NavalStorylineStage.Act3Quest5);
				if (StoryModeHeroes.LittleSister.Age < (float)Campaign.Current.Models.AgeModel.HeroComesOfAge)
				{
					if (!StoryModeHeroes.LittleSister.IsDisabled && !StoryModeHeroes.LittleSister.IsNotSpawned)
					{
						if (flag)
						{
							StoryModeHeroes.LittleSister.ChangeState(0);
						}
						else
						{
							DisableHeroAction.Apply(StoryModeHeroes.LittleSister);
						}
					}
					if (!StoryModeHeroes.LittleSister.IsDisabled && dictionary != null && !dictionary.ContainsKey(StoryModeHeroes.LittleSister))
					{
						dictionary.Add(StoryModeHeroes.LittleSister, (int)StoryModeHeroes.LittleSister.Age);
						field.SetValue(campaignBehavior, dictionary);
					}
				}
				else if (flag)
				{
					if (dictionary != null && dictionary.ContainsKey(StoryModeHeroes.LittleSister))
					{
						dictionary.Remove(StoryModeHeroes.LittleSister);
					}
					this.CheckPlayerSiblingsEducationStages(StoryModeHeroes.LittleSister);
					this.CheckStoryModeHeroStateAndUpdateIfNeeded(StoryModeHeroes.LittleSister);
					StoryModeHelpers.SetPlayerSiblingsSkillsIfNeeded(StoryModeHeroes.LittleSister);
				}
				else if (!StoryModeHeroes.LittleSister.IsDisabled)
				{
					DisableHeroAction.Apply(StoryModeHeroes.LittleSister);
					if (StoryModeHeroes.LittleSister.GovernorOf != null)
					{
						ChangeGovernorAction.RemoveGovernorOf(StoryModeHeroes.LittleSister);
					}
				}
				this.CheckAndUpdateGovernorStatusOfStoryModeHero(StoryModeHeroes.LittleSister);
			}
			if (MBSaveLoad.IsUpdatingGameVersion && MBSaveLoad.LastLoadedGameVersion.IsOlderThan(ApplicationVersion.FromString("v1.3.15", 0)))
			{
				if (NavalStorylineData.GetStorylineStage() >= NavalStorylineData.NavalStorylineStage.Act3Quest5 && !NavalStorylineData.Gunnar.IsDead)
				{
					Settlement currentSettlement = NavalStorylineData.Gunnar.CurrentSettlement;
					Village village = Village.All.FirstOrDefault<Village>((Village x) => x.Settlement.StringId == "village_N1_2");
					if (village != null && currentSettlement != village.Settlement)
					{
						TeleportHeroAction.ApplyImmediateTeleportToSettlement(NavalStorylineData.Gunnar, village.Settlement);
					}
				}
				NavalStorylineData.SetHeroText(NavalStorylineData.Gunnar);
				NavalStorylineData.SetHeroText(NavalStorylineData.Purig);
				NavalStorylineData.SetHeroText(NavalStorylineData.Lahar);
				NavalStorylineData.SetHeroText(NavalStorylineData.EmiraAlFahda);
				NavalStorylineData.SetHeroText(NavalStorylineData.Prusas);
				NavalStorylineData.SetHeroText(NavalStorylineData.Bjolgur);
				if (NavalStorylineData.Purig.IsDead)
				{
					this.SetOnPurigKilledTexts();
				}
			}
		}

		// Token: 0x060007CC RID: 1996 RVA: 0x00037F38 File Offset: 0x00036138
		private void CheckPlayerSiblingsEducationStages(Hero hero)
		{
			EducationCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<EducationCampaignBehavior>();
			if (campaignBehavior != null)
			{
				Type typeFromHandle = typeof(EducationCampaignBehavior);
				if (((Dictionary<Hero, short>)typeFromHandle.GetField("_previousEducations", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(campaignBehavior)).ContainsKey(hero) || !this.IsHeroAttributesInitialized(hero))
				{
					typeFromHandle.GetMethod("OnHeroComesOfAge", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(campaignBehavior, new object[] { hero });
				}
			}
		}

		// Token: 0x060007CD RID: 1997 RVA: 0x00037FA8 File Offset: 0x000361A8
		private void CheckStoryModeHeroStateAndUpdateIfNeeded(Hero hero)
		{
			if (hero.IsNotSpawned || hero.IsDisabled)
			{
				Settlement settlementToSpawnForPlayerRelative = this.GetSettlementToSpawnForPlayerRelative(hero);
				if (hero.BornSettlement == null)
				{
					hero.BornSettlement = settlementToSpawnForPlayerRelative;
				}
				TeleportHeroAction.ApplyImmediateTeleportToSettlement(hero, settlementToSpawnForPlayerRelative);
				if (!hero.IsActive)
				{
					hero.ChangeState(1);
				}
			}
			if (hero.Clan == null)
			{
				hero.Clan = Clan.PlayerClan;
				if (!hero.IsFugitive)
				{
					MakeHeroFugitiveAction.Apply(hero, false);
				}
			}
		}

		// Token: 0x060007CE RID: 1998 RVA: 0x00038014 File Offset: 0x00036214
		private void CheckAndUpdateGovernorStatusOfStoryModeHero(Hero hero)
		{
			if (hero.GovernorOf != null && hero.CurrentSettlement != hero.GovernorOf.Settlement)
			{
				Debug.FailedAssert("Last governor check might be unnecessary, check this case", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\Storyline\\CampaignBehaviors\\NavalStorylineCampaignBehavior.cs", "CheckAndUpdateGovernorStatusOfStoryModeHero", 342);
				ChangeGovernorAction.RemoveGovernorOf(StoryModeHeroes.LittleSister);
			}
		}

		// Token: 0x060007CF RID: 1999 RVA: 0x00038054 File Offset: 0x00036254
		private bool IsHeroAttributesInitialized(Hero hero)
		{
			return hero.CharacterAttributes.GetPropertyValue(DefaultCharacterAttributes.Vigor) != 0 || hero.CharacterAttributes.GetPropertyValue(DefaultCharacterAttributes.Control) != 0 || hero.CharacterAttributes.GetPropertyValue(DefaultCharacterAttributes.Endurance) != 0 || hero.CharacterAttributes.GetPropertyValue(DefaultCharacterAttributes.Cunning) != 0 || hero.CharacterAttributes.GetPropertyValue(DefaultCharacterAttributes.Social) != 0 || hero.CharacterAttributes.GetPropertyValue(DefaultCharacterAttributes.Intelligence) != 0;
		}

		// Token: 0x060007D0 RID: 2000 RVA: 0x000380D0 File Offset: 0x000362D0
		private Settlement GetSettlementToSpawnForPlayerRelative(Hero hero)
		{
			if (hero.GovernorOf != null)
			{
				return hero.GovernorOf.Settlement;
			}
			if (!hero.HomeSettlement.OwnerClan.IsAtWarWith(Clan.PlayerClan.MapFaction))
			{
				return hero.HomeSettlement;
			}
			if (!Extensions.IsEmpty<Settlement>(Clan.PlayerClan.MapFaction.Settlements))
			{
				return Extensions.GetRandomElement<Settlement>(Clan.PlayerClan.MapFaction.Settlements);
			}
			foreach (Settlement settlement in Settlement.All)
			{
				if (!settlement.MapFaction.IsAtWarWith(Clan.PlayerClan.MapFaction))
				{
					return settlement;
				}
			}
			return Extensions.GetRandomElement<Village>(Village.All).Settlement;
		}

		// Token: 0x060007D1 RID: 2001 RVA: 0x000381AC File Offset: 0x000363AC
		private void MissionTickEvent(float dt)
		{
			if (this._removeCrimeHandler)
			{
				this.RemoveCrimeHandler(Mission.Current);
				this._removeCrimeHandler = false;
			}
		}

		// Token: 0x060007D2 RID: 2002 RVA: 0x000381C8 File Offset: 0x000363C8
		private void AfterMissionStarted(IMission mission)
		{
			if (this._isNavalStorylineActive && LocationComplex.Current != null && Settlement.CurrentSettlement != null && Settlement.CurrentSettlement.IsTown && Campaign.Current.Models.CrimeModel.IsPlayerCrimeRatingSevere(Settlement.CurrentSettlement.MapFaction))
			{
				this._removeCrimeHandler = true;
			}
		}

		// Token: 0x060007D3 RID: 2003 RVA: 0x0003821E File Offset: 0x0003641E
		private void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification = true)
		{
			if (victim == NavalStorylineData.Purig)
			{
				this.SetOnPurigKilledTexts();
			}
		}

		// Token: 0x060007D4 RID: 2004 RVA: 0x00038230 File Offset: 0x00036430
		private void SetOnPurigKilledTexts()
		{
			TextObject textObject = new TextObject("{=QPn1OTcd}Purig was a Nord warrior who fought in the rebellion against Volbjorn, first ruler of the Nordvyg. Following the king's victory he joined with other defeated rebels to form the Sea Hounds, a pirate confederation that terrorized the northern seas of Calradia, but he was defeated and slain by {PLAYER.NAME}.", null);
			StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, textObject, false);
			NavalStorylineData.Purig.EncyclopediaText = textObject;
			TextObject textObject2 = new TextObject("{=8NxBfsY1}Gunnar of Lagshofn is a Nord warrior from the island of Beinland. He won a reputation for courage fighting in a rebellion against Volbjorn, first king of the Nordvyg, but after the rebels' defeat he made his peace with the victors. He then joined with {PLAYER.NAME} to vanquish the Sea Hounds, a pirate confederation led by Purig that had terrorized the northern seas of Calradia.", null);
			StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, textObject2, false);
			NavalStorylineData.Gunnar.EncyclopediaText = textObject2;
		}

		// Token: 0x060007D5 RID: 2005 RVA: 0x0003828F File Offset: 0x0003648F
		private void OnNavalStorylineCanceled(NavalStorylineData.StorylineCancelDetail detail)
		{
			this._isNavalStorylineCanceled = true;
			CampaignEventDispatcher.Instance.RemoveListeners(this);
		}

		// Token: 0x060007D6 RID: 2006 RVA: 0x000382A4 File Offset: 0x000364A4
		private void OnSettlementEntered(MobileParty party, Settlement settlement, Hero hero)
		{
			if (party == MobileParty.MainParty && NavalStorylineData.Gunnar.StayingInSettlement == settlement && settlement.StringId.Equals("village_N1_2"))
			{
				Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(NavalStorylineData.Gunnar.CharacterObject.Race, "_settlement");
				ValueTuple<string, Monster> valueTuple = new ValueTuple<string, Monster>(ActionSetCode.GenerateActionSetNameWithSuffix(monsterWithSuffix, NavalStorylineData.Gunnar.CharacterObject.IsFemale, "_lord"), monsterWithSuffix);
				IFaction mapFaction = NavalStorylineData.Gunnar.MapFaction;
				uint num = ((mapFaction != null) ? mapFaction.Color : 4291609515U);
				IFaction mapFaction2 = NavalStorylineData.Gunnar.MapFaction;
				uint num2 = ((mapFaction2 != null) ? mapFaction2.Color : 4291609515U);
				AgentData agentData = new AgentData(new SimpleAgentOrigin(NavalStorylineData.Gunnar.CharacterObject, -1, null, default(UniqueTroopDescriptor))).Monster(valueTuple.Item2).NoHorses(true).ClothingColor1(num)
					.ClothingColor2(num2);
				LocationComplex.Current.GetLocationWithId("village_center").AddCharacter(new LocationCharacter(agentData, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddFixedCharacterBehaviors), "sp_notable", true, 1, valueTuple.Item1, true, false, null, false, false, true, null, false));
			}
		}

		// Token: 0x060007D7 RID: 2007 RVA: 0x000383D8 File Offset: 0x000365D8
		private void OnQuestCompleted(QuestBase quest, QuestBase.QuestCompleteDetails details)
		{
			NavalStorylineQuestBase navalStorylineQuestBase;
			if ((navalStorylineQuestBase = quest as NavalStorylineQuestBase) != null && navalStorylineQuestBase.WillProgressStoryline)
			{
				if (details == 2)
				{
					this.ChangeNavalStorylineActivity(false);
					return;
				}
				if (navalStorylineQuestBase.Stage < NavalStorylineData.NavalStorylineStage.Act3Quest5)
				{
					new ReturnToBaseQuest("naval_storyline_return_to_base", NavalStorylineData.Gunnar).StartQuest();
				}
				this._lastCompletedStorylineStage = navalStorylineQuestBase.Stage;
			}
		}

		// Token: 0x060007D8 RID: 2008 RVA: 0x0003842C File Offset: 0x0003662C
		private void CanHeroBecomePrisoner(Hero hero, ref bool result)
		{
			if (this._isNavalStorylineActive && NavalStorylineData.IsNavalStorylineHero(hero))
			{
				result = false;
			}
		}

		// Token: 0x060007D9 RID: 2009 RVA: 0x00038441 File Offset: 0x00036641
		private void OnSettlementLeft(MobileParty party, Settlement settlement)
		{
			if (party == MobileParty.MainParty && this._isNavalStorylineActive)
			{
				Campaign.Current.SaveHandler.ForceAutoSave();
				if (!MobileParty.MainParty.IsCurrentlyAtSea)
				{
					this.ChangeNavalStorylineActivity(false);
				}
			}
		}

		// Token: 0x060007DA RID: 2010 RVA: 0x00038475 File Offset: 0x00036675
		private void OnMobilePartyNavigationStateChanged(MobileParty mobileParty)
		{
			if (this._isNavalStorylineActive && mobileParty.IsMainParty && !mobileParty.IsCurrentlyAtSea && PlayerEncounter.EncounterSettlement == null)
			{
				this.ChangeNavalStorylineActivity(false);
			}
		}

		// Token: 0x060007DB RID: 2011 RVA: 0x0003849D File Offset: 0x0003669D
		private void OnHeirSelectionOver(Hero hero)
		{
			if (this._isNavalStorylineActive)
			{
				this.ChangeNavalStorylineActivity(false);
			}
		}

		// Token: 0x060007DC RID: 2012 RVA: 0x000384AE File Offset: 0x000366AE
		private void OnSessionLaunched(CampaignGameStarter starter)
		{
			this.AddGameMenus(starter);
			this.AddDialogues();
		}

		// Token: 0x060007DD RID: 2013 RVA: 0x000384BD File Offset: 0x000366BD
		private void AddDialogues()
		{
			this.AddGunnarSeaDefaultConversations();
			this.AddGunnarTownDefaultConversations();
			this.AddGunnarRansomConversations();
			this.AddGunnarSisterRansomConversations();
			this.AddGunnarStorylineActivationNotPossibleConversation();
			this.AddBjolgurDefaultConversations();
			this.AddLaharDefaultConversations();
		}

		// Token: 0x060007DE RID: 2014 RVA: 0x000384EC File Offset: 0x000366EC
		private void AddGunnarSeaDefaultConversations()
		{
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 200).NpcLine("{=0zTShzbi}Keep an eye on the horizon, and look for sails.", null, null, null, null).Condition(() => Hero.OneToOneConversationHero == NavalStorylineData.Gunnar && Settlement.CurrentSettlement == null && MobileParty.MainParty.IsCurrentlyAtSea && Hero.OneToOneConversationHero.PartyBelongedTo == MobileParty.MainParty)
				.CloseDialog(), null);
		}

		// Token: 0x060007DF RID: 2015 RVA: 0x00038550 File Offset: 0x00036750
		private void AddGunnarTownDefaultConversations()
		{
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 200).NpcLine("{=Si6F4bdz}I'm waiting for more news. Soon, I may have more to tell you.", null, null, null, null).Condition(() => Hero.OneToOneConversationHero == NavalStorylineData.Gunnar && Settlement.CurrentSettlement != null)
				.CloseDialog(), null);
		}

		// Token: 0x060007E0 RID: 2016 RVA: 0x000385B4 File Offset: 0x000367B4
		private void AddGunnarStorylineActivationNotPossibleConversation()
		{
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 30000).NpcLine("{=njVdva7h}This isn't the right time to pursue our war against the Sea Hounds, but believe me, I am not about to abandon it.", null, null, null, null).Condition(() => Hero.OneToOneConversationHero == NavalStorylineData.Gunnar && !NavalStorylineData.IsStorylineActivationPossible())
				.PlayerLine("{=KrsZJv1e}I shall return, hopefully under better circumstances.", null, null, null)
				.CloseDialog(), null);
		}

		// Token: 0x060007E1 RID: 2017 RVA: 0x00038624 File Offset: 0x00036824
		private void AddGunnarRansomConversations()
		{
			string text;
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("gunnar_ransom_sister", 1200).NpcLine("{=F94IaWhk}Ah... So be it. I understand why you must put your sister's safety above other considerations. I know people who can pass a message to the Sea Hounds, and I can make inquiries about a ransom, if you like.", null, null, null, null).ClickableCondition(new ConversationSentence.OnClickableConditionDelegate(this.CanRansomSister))
				.GenerateToken(ref text)
				.BeginPlayerOptions(null, false)
				.PlayerOption("{=JIoiP1Is}Do that.", null, null, null)
				.GotoDialogState(text)
				.PlayerOption("{=NvCbw6VY}No. I will not pay money to pirates.", null, null, null)
				.NpcLine("{=BpQZOVIp}I am glad that you see things that way.", null, null, null, null)
				.CloseDialog()
				.EndPlayerOptions()
				.NpcLine("{=R9byg5mp}Very well. But I should warn you... By now, I am sure, the Sea Hounds know your name. You are building a bit of a reputation. I doubt that they’d give up your sister as cheaply as they would some common captive. If you left me {GOLD}{GOLD_ICON} denars, I'm sure that would suffice, but that kind of money may be hard to come by.", null, null, text, null)
				.Condition(delegate
				{
					MBTextManager.SetTextVariable("GOLD", 10000);
					MBTextManager.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"6\">", false);
					return true;
				})
				.BeginPlayerOptions(null, false)
				.PlayerOption("{=rFaOeL2M}Here - this is {GOLD}{GOLD_ICON} denars. Take it, and make your inquiries.", null, null, null)
				.ClickableCondition(new ConversationSentence.OnClickableConditionDelegate(this.DoesPlayerHaveEnoughGoldToRansomSister))
				.NpcLine("{=nSWqf79K}Right... I will send word when I know more.", null, null, null, null)
				.Consequence(new ConversationSentence.OnConsequenceDelegate(this.RequestRansomSister))
				.CloseDialog()
				.PlayerOption("{=UgWdVbxn}Somehow, I will raise the money.", null, null, null)
				.NpcLine("{=S5cioFLJ}Very well. If you are able to raise the money, and still wish to proceed with the ransom, then let me know. I owe you my life, and I am always ready to help you in whatever course you choose.", null, null, null, null)
				.CloseDialog()
				.PlayerOption("{=OONSEXb2}I will never pay that kind of money to brigands.", null, null, null)
				.NpcLine("{=BpQZOVIp}I am glad that you see things that way.", null, null, null, null)
				.CloseDialog()
				.EndPlayerOptions(), null);
		}

		// Token: 0x060007E2 RID: 2018 RVA: 0x00038778 File Offset: 0x00036978
		private void AddLaharDefaultConversations()
		{
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 200).NpcLine("{=TAiPuK1n}When you're at sea, you long to be on shore. When you're on shore, shuffling about and waiting for things to be made ready, you'd give anything to be back at sea, running fast before the wind. That's always how it is.", null, null, null, null).Condition(() => Hero.OneToOneConversationHero == NavalStorylineData.Lahar)
				.CloseDialog(), null);
		}

		// Token: 0x060007E3 RID: 2019 RVA: 0x000387DC File Offset: 0x000369DC
		private void AddGunnarSisterRansomConversations()
		{
			string text;
			string text2;
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 1200).NpcLine("{=Gkmt02Zo}{PLAYER.NAME}... I have someone who is eager to see you again!", null, null, null, null).Condition(delegate
			{
				bool flag = Hero.OneToOneConversationHero == NavalStorylineData.Gunnar && Settlement.CurrentSettlement == NavalStorylineData.HomeSettlement && this._lastCompletedStorylineStage == NavalStorylineData.NavalStorylineStage.Act3SpeakToGunnarAndSister;
				if (flag)
				{
					StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, null, false);
				}
				return flag;
			})
				.Consequence(delegate
				{
					this.SpawnSister(this.GetSisterTeleportPosition());
				})
				.NpcLine("{=MmGO1qT4}{?PLAYER.GENDER}Sister{?}Brother{\\?}! Is that you? Heaven's mercy! I had all but given up hope, and then they told me that you had arranged for my ransom. Thank you, from the bottom of my heart, thank you!", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsSister), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsPlayer), null, null)
				.GenerateToken(ref text)
				.GenerateToken(ref text2)
				.BeginPlayerOptions(null, false)
				.PlayerOption("{=ZYtba6KE}My sister... Of course I could not leave you in the hands of those cruel men.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsSister), null, null)
				.GotoDialogState(text)
				.PlayerOption("{=5Drb4hBh}A small price to pay for your safety. Well, maybe not that small...", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsSister), null, null)
				.GotoDialogState(text)
				.EndPlayerOptions()
				.NpcLine("{=QunHWmAo}That terrible night... Father, mother... Those vile slavers, dragging me from port to port. I won’t speak of it now. But Gunnar says that you have risen in the world, that our fortunes have changed. Know that I am ready to do my part, for our family and our future...", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsSister), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsPlayer), text, null)
				.NpcLine("{=LZSRoGTN}{PLAYER.NAME}... I am glad to have helped you reunite your family, and I hope it repays part of my debt to you. But now I must take my leave. I have unfinished business with the Sea Hounds, and with Purig in particular. I do not think we shall meet again.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsGunnar), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsPlayer), null, null)
				.BeginPlayerOptions(null, false)
				.PlayerOption("{=I2Ab1kzZ}Good hunting, Gunnar. Give that bastard Purig one from me.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsGunnar), null, null)
				.GotoDialogState(text2)
				.PlayerOption("{=agCqAQuA}It seems a bit of a doomed errand, but good luck anyway.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsGunnar), null, null)
				.GotoDialogState(text2)
				.EndPlayerOptions()
				.NpcLine("{=2g2FhKb5}Farewell.", null, null, text2, null)
				.Consequence(new ConversationSentence.OnConsequenceDelegate(this.EndStorylineByRansom))
				.CloseDialog(), null);
		}

		// Token: 0x060007E4 RID: 2020 RVA: 0x00038968 File Offset: 0x00036B68
		private void SpawnSister(Vec3 spawnPosition)
		{
			Agent agent = Mission.Current.Agents.FirstOrDefault<Agent>((Agent x) => this.IsSister(x));
			if (agent == null)
			{
				AgentBuildData agentBuildData = new AgentBuildData(StoryModeHeroes.LittleSister.CharacterObject);
				agentBuildData.TroopOrigin(new SimpleAgentOrigin(agentBuildData.AgentCharacter, -1, null, default(UniqueTroopDescriptor)));
				agentBuildData.InitialPosition(ref spawnPosition);
				AgentBuildData agentBuildData2 = agentBuildData;
				Vec2 vec = Agent.Main.LookDirection.AsVec2;
				vec = -vec.Normalized();
				agentBuildData2.InitialDirection(ref vec);
				agentBuildData.NoHorses(true);
				agentBuildData.CivilianEquipment(true);
				agent = Mission.Current.SpawnAgent(agentBuildData, false);
			}
			ConversationManager conversationManager = Campaign.Current.ConversationManager;
			MBList<IAgent> mblist = new MBList<IAgent>();
			mblist.Add(agent);
			conversationManager.AddConversationAgents(mblist, true);
			this.RemoveWalkingBehavior(StoryModeHeroes.LittleSister.CharacterObject);
		}

		// Token: 0x060007E5 RID: 2021 RVA: 0x00038A40 File Offset: 0x00036C40
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

		// Token: 0x060007E6 RID: 2022 RVA: 0x00038A9C File Offset: 0x00036C9C
		private Vec3 GetSisterTeleportPosition()
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

		// Token: 0x060007E7 RID: 2023 RVA: 0x00038B50 File Offset: 0x00036D50
		private void EndStorylineByRansom()
		{
			NavalDLCEvents.Instance.OnNavalStorylineCanceled(NavalStorylineData.StorylineCancelDetail.ByRansom);
			Campaign.Current.ConversationManager.ConversationEndOneShot += delegate
			{
				Mission.Current.EndMission();
			};
			LocationComplex locationComplex = LocationComplex.Current;
			if (locationComplex != null)
			{
				locationComplex.RemoveCharacterIfExists(NavalStorylineData.Gunnar);
			}
			DisableHeroAction.Apply(NavalStorylineData.Gunnar);
			NavalDLCHelpers.AddSisterToClan();
		}

		// Token: 0x060007E8 RID: 2024 RVA: 0x00038BBC File Offset: 0x00036DBC
		private bool DoesPlayerHaveEnoughGoldToRansomSister(out TextObject tooltip)
		{
			bool flag = Hero.MainHero.Gold >= 10000;
			if (!flag)
			{
				tooltip = new TextObject("{=d0kbtGYn}You don't have enough gold.", null);
			}
			else
			{
				tooltip = TextObject.GetEmpty();
			}
			MBTextManager.SetTextVariable("GOLD", 10000);
			MBTextManager.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"6\">", false);
			return flag;
		}

		// Token: 0x060007E9 RID: 2025 RVA: 0x00038C18 File Offset: 0x00036E18
		private void RequestRansomSister()
		{
			if (!Campaign.Current.QuestManager.IsThereActiveQuestWithType(typeof(ScourgeoftheSeasQuest)))
			{
				new ScourgeoftheSeasQuest().StartQuest();
			}
			GiveGoldAction.ApplyBetweenCharacters(Hero.MainHero, null, 10000, false);
			this._sisterReturnTime = CampaignTime.WeeksFromNow(3f);
			LocationComplex locationComplex = LocationComplex.Current;
			if (locationComplex != null)
			{
				locationComplex.RemoveCharacterIfExists(NavalStorylineData.Gunnar);
			}
			DisableHeroAction.Apply(NavalStorylineData.Gunnar);
			Campaign.Current.ConversationManager.ConversationEndOneShot += delegate
			{
				Mission.Current.EndMission();
			};
			NavalDLCEvents.Instance.OnSisterRansomRequested();
		}

		// Token: 0x060007EA RID: 2026 RVA: 0x00038CC2 File Offset: 0x00036EC2
		private bool CanRansomSister(out TextObject tooltip)
		{
			tooltip = TextObject.GetEmpty();
			return Hero.OneToOneConversationHero == NavalStorylineData.Gunnar && Settlement.CurrentSettlement == NavalStorylineData.HomeSettlement && !NavalStorylineData.IsNavalStorylineCanceled() && !NavalStorylineData.IsNavalStoryLineActive();
		}

		// Token: 0x060007EB RID: 2027 RVA: 0x00038CF4 File Offset: 0x00036EF4
		private void AddBjolgurDefaultConversations()
		{
			Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 200).NpcLine("{=fzhTwmvM}A battle at sea is a fine thing. Cowards have nowhere to run, and the fish do your cleaning-up for you.", null, null, null, null).Condition(() => Hero.OneToOneConversationHero == NavalStorylineData.Bjolgur)
				.CloseDialog(), null);
		}

		// Token: 0x060007EC RID: 2028 RVA: 0x00038D58 File Offset: 0x00036F58
		private void home_settlement_encounter_init(MenuCallbackArgs args)
		{
			TextObject textObject = new TextObject("{=lqy3wHWi}You have returned to Ostican harbor. Gunnar takes his leave to see if any new information about the Sea Hounds has arrived, or any new allies to join you in your fight. He tells you to look for him in the harbor when you are ready to proceed.", null);
			if (this._isFirstReturnToOstican)
			{
				textObject = new TextObject("{=7UmbvMKi}You return to Ostican harbor, and tie your ship up at the pier. Besides the Vlandian traders and fishing vessels lies a small Nordic longship. Gunnar tells you that some of his comrades have responded to his call to hunt the Sea Hounds. He tells you he needs to dictate a letter to some others, and asks you to meet him later in the port.", null);
			}
			MBTextManager.SetTextVariable("MENU_TEXT", textObject, false);
		}

		// Token: 0x060007ED RID: 2029 RVA: 0x00038D94 File Offset: 0x00036F94
		private void leave_on_consequence(MenuCallbackArgs args)
		{
			if (this._isFirstReturnToOstican)
			{
				this._isFirstReturnToOstican = false;
			}
			Settlement settlement = Settlement.CurrentSettlement ?? PlayerEncounter.EncounterSettlement;
			bool flag;
			bool flag2;
			GameMenu.SwitchToMenu((MobileParty.MainParty.HasNavalNavigationCapability && MobileParty.MainParty.Anchor.IsAtSettlement(settlement)) ? "naval_town_outside" : Campaign.Current.Models.EncounterGameMenuModel.GetEncounterMenu(PartyBase.MainParty, settlement.Party, ref flag, ref flag2));
		}

		// Token: 0x060007EE RID: 2030 RVA: 0x00038E0F File Offset: 0x0003700F
		private bool leave_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 16;
			return true;
		}

		// Token: 0x060007EF RID: 2031 RVA: 0x00038E1C File Offset: 0x0003701C
		private void AddGameMenus(CampaignGameStarter campaignGameStarter)
		{
			campaignGameStarter.AddGameMenu("naval_storyline_encounter_blocking", "{=LptlZGpR}The seas are rough, and it is difficult to bring your ship within hailing distance. Gunnar urges you not to waste time here, as you are in some haste.", new OnInitDelegate(this.virtual_encounter_init), 0, 0, null);
			campaignGameStarter.AddGameMenuOption("naval_storyline_encounter_blocking", "continue", "{=3sRdGQou}Leave", new GameMenuOption.OnConditionDelegate(this.leave_on_condition), new GameMenuOption.OnConsequenceDelegate(this.virtual_encounter_end_consequence), true, -1, false, null);
			campaignGameStarter.AddGameMenu("naval_storyline_outside_town", "{MENU_TEXT}", new OnInitDelegate(this.home_settlement_encounter_init), 0, 0, null);
			campaignGameStarter.AddGameMenuOption("naval_storyline_outside_town", "talk_to_gunnar", "{=fJP8DJcB}Talk to Gunnar in port", new GameMenuOption.OnConditionDelegate(this.talk_to_gunnar_on_condition), new GameMenuOption.OnConsequenceDelegate(this.talk_to_gunnar_on_consequence), false, -1, false, null);
			campaignGameStarter.AddGameMenuOption("naval_storyline_outside_town", "continue", "{=8nP7PcCQ}Continue the story later", new GameMenuOption.OnConditionDelegate(this.leave_on_condition), new GameMenuOption.OnConsequenceDelegate(this.leave_on_consequence), true, -1, false, null);
			campaignGameStarter.AddGameMenu("naval_storyline_encounter_meeting", "{=!}.", new OnInitDelegate(this.game_menu_naval_storyline_encounter_meeting_on_init), 0, 0, null);
			campaignGameStarter.AddGameMenu("naval_storyline_encounter", "{=!}{ENCOUNTER_TEXT}", new OnInitDelegate(this.game_menu_naval_storyline_encounter_on_init), 4, 0, null);
			campaignGameStarter.AddGameMenuOption("naval_storyline_encounter", "attack", "{=zxMOqlhs}Attack", new GameMenuOption.OnConditionDelegate(this.game_menu_naval_storyline_encounter_attack_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_naval_storyline_encounter_attack_on_consequence), false, -1, false, null);
			campaignGameStarter.AddGameMenuOption("naval_storyline_encounter", "leave", "{=2YYRyrOO}Leave...", new GameMenuOption.OnConditionDelegate(this.game_menu_naval_storyline_encounter_leave_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_naval_storyline_encounter_leave_on_consequence), true, -1, false, null);
			campaignGameStarter.AddGameMenu("naval_storyline_join_encounter", "{=jKWJpIES}{JOIN_ENCOUNTER_TEXT}. You decide to...", new OnInitDelegate(this.game_menu_join_naval_storyline_encounter_on_init), 0, 0, null);
			campaignGameStarter.AddGameMenuOption("naval_storyline_join_encounter", "join_encounter_help_attackers", "{=h3yEHb4U}Help {ATTACKER}.", new GameMenuOption.OnConditionDelegate(this.game_menu_join_naval_storyline_encounter_help_attackers_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_join_naval_storyline_encounter_help_attackers_on_consequence), false, -1, false, null);
			campaignGameStarter.AddGameMenuOption("naval_storyline_join_encounter", "join_encounter_help_defenders", "{=FwIgakj8}Help {DEFENDER}.", new GameMenuOption.OnConditionDelegate(this.game_menu_join_naval_storyline_encounter_help_defenders_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_join_naval_storyline_encounter_help_defenders_on_consequence), false, -1, false, null);
			campaignGameStarter.AddGameMenuOption("naval_storyline_join_encounter", "join_encounter_leave", "{=!}{LEAVE_TEXT}", new GameMenuOption.OnConditionDelegate(this.game_menu_join_naval_storyline_encounter_leave_no_army_on_condition), new GameMenuOption.OnConsequenceDelegate(this.game_menu_join_naval_storyline_encounter_leave_on_condition), true, -1, false, null);
			campaignGameStarter.AddGameMenuOption("town_outside", "contact_gunnar", "{=KStpUvo2}Hail Gunnar's contact for entry", new GameMenuOption.OnConditionDelegate(this.talk_to_gunnar_town_outside_on_condition), new GameMenuOption.OnConsequenceDelegate(this.contact_gunnar_on_consequence), false, -1, false, null);
			campaignGameStarter.AddGameMenuOption("naval_town_outside", "contact_gunnar", "{=KStpUvo2}Hail Gunnar's contact for entry", new GameMenuOption.OnConditionDelegate(this.talk_to_gunnar_town_outside_on_condition), new GameMenuOption.OnConsequenceDelegate(this.contact_gunnar_on_consequence), false, -1, false, null);
			campaignGameStarter.AddGameMenu("talk_to_gunnar_restricted", "{=sNybnI5O}Gunnar's contact snuck you into the town and lead you to him.", null, 0, 0, null);
			campaignGameStarter.AddGameMenuOption("talk_to_gunnar_restricted", "talk_to_gunnar_restricted_continue", "{=DM6luo3c}Continue", new GameMenuOption.OnConditionDelegate(this.talk_to_gunnar_restricted_continue), new GameMenuOption.OnConsequenceDelegate(this.talk_to_gunnar_on_consequence), false, -1, false, null);
			campaignGameStarter.AddGameMenuOption("talk_to_gunnar_restricted", "leave", "{=3sRdGQou}Leave", null, new GameMenuOption.OnConsequenceDelegate(this.contact_gunnar_leave_on_consequence), false, -1, false, null);
		}

		// Token: 0x060007F0 RID: 2032 RVA: 0x0003911C File Offset: 0x0003731C
		private bool talk_to_gunnar_town_outside_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 17;
			NavalStorylineData.NavalStorylineStage storylineStage = NavalStorylineData.GetStorylineStage();
			return storylineStage >= NavalStorylineData.NavalStorylineStage.Act1 && storylineStage != NavalStorylineData.NavalStorylineStage.Act3Quest5 && !NavalStorylineData.IsMainPartyAllowed() && !Settlement.CurrentSettlement.IsUnderSiege;
		}

		// Token: 0x060007F1 RID: 2033 RVA: 0x00039155 File Offset: 0x00037355
		private bool talk_to_gunnar_restricted_continue(MenuCallbackArgs args)
		{
			args.optionLeaveType = 22;
			return true;
		}

		// Token: 0x060007F2 RID: 2034 RVA: 0x00039160 File Offset: 0x00037360
		private void contact_gunnar_on_consequence(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("talk_to_gunnar_restricted");
		}

		// Token: 0x060007F3 RID: 2035 RVA: 0x0003916C File Offset: 0x0003736C
		private void contact_gunnar_leave_on_consequence(MenuCallbackArgs args)
		{
			Settlement settlement = Settlement.CurrentSettlement ?? PlayerEncounter.EncounterSettlement;
			bool flag;
			bool flag2;
			GameMenu.SwitchToMenu((MobileParty.MainParty.HasNavalNavigationCapability && MobileParty.MainParty.Anchor.IsAtSettlement(settlement)) ? "naval_town_outside" : Campaign.Current.Models.EncounterGameMenuModel.GetEncounterMenu(PartyBase.MainParty, settlement.Party, ref flag, ref flag2));
		}

		// Token: 0x060007F4 RID: 2036 RVA: 0x000391D8 File Offset: 0x000373D8
		private void game_menu_naval_storyline_encounter_meeting_on_init(MenuCallbackArgs args)
		{
			if (PlayerEncounter.Current == null || ((PlayerEncounter.Battle == null || PlayerEncounter.Battle.AttackerSide.LeaderParty == PartyBase.MainParty || PlayerEncounter.Battle.DefenderSide.LeaderParty == PartyBase.MainParty) && !PlayerEncounter.MeetingDone))
			{
				PlayerEncounter.DoMeeting();
				return;
			}
			if (PlayerEncounter.LeaveEncounter)
			{
				PlayerEncounter.Finish(true);
				return;
			}
			if (PlayerEncounter.Battle == null)
			{
				PlayerEncounter.StartBattle();
			}
			if (PlayerEncounter.BattleChallenge)
			{
				GameMenu.SwitchToMenu("duel_starter_menu");
				return;
			}
			MBTextManager.SetTextVariable("ENCOUNTER_TEXT", GameTexts.FindText("str_you_have_encountered_PARTY", null), false);
			GameMenu.SwitchToMenu("naval_storyline_encounter");
		}

		// Token: 0x060007F5 RID: 2037 RVA: 0x0003927C File Offset: 0x0003747C
		private void game_menu_naval_storyline_encounter_on_init(MenuCallbackArgs args)
		{
			args.MenuContext.SetPanelSound("event:/ui/panels/battle/slide_in");
			if (PlayerEncounter.Battle == null)
			{
				if (MobileParty.MainParty.MapEvent != null)
				{
					PlayerEncounter.Init();
				}
				else
				{
					PlayerEncounter.StartBattle();
				}
			}
			PlayerEncounter.Update();
			if (PlayerEncounter.Current == null)
			{
				Campaign.Current.SaveHandler.SignalAutoSave();
			}
		}

		// Token: 0x060007F6 RID: 2038 RVA: 0x000392D4 File Offset: 0x000374D4
		private bool game_menu_naval_storyline_encounter_attack_on_condition(MenuCallbackArgs args)
		{
			MenuCallbackArgs menuCallbackArgs = new MenuCallbackArgs(args.MapState, TextObject.GetEmpty());
			CampaignBattleResult campaignBattleResult = PlayerEncounter.CampaignBattleResult;
			if (campaignBattleResult != null && !campaignBattleResult.PlayerVictory && Hero.MainHero.IsWounded && !PlayerEncounter.PlayerSurrender)
			{
				PlayerEncounter.PlayerSurrender = true;
				PlayerEncounter.Update();
				return false;
			}
			if (MenuHelper.EncounterOrderAttackCondition(menuCallbackArgs) && Hero.MainHero.HitPoints < Hero.MainHero.WoundedHealthLimit + 1)
			{
				Hero.MainHero.HitPoints = Hero.MainHero.WoundedHealthLimit + 1;
			}
			MenuHelper.CheckEnemyAttackableHonorably(args);
			return MenuHelper.EncounterAttackCondition(args);
		}

		// Token: 0x060007F7 RID: 2039 RVA: 0x0003936A File Offset: 0x0003756A
		private void game_menu_naval_storyline_encounter_attack_on_consequence(MenuCallbackArgs args)
		{
			MenuHelper.EncounterAttackConsequence(args);
		}

		// Token: 0x060007F8 RID: 2040 RVA: 0x00039372 File Offset: 0x00037572
		private bool game_menu_naval_storyline_encounter_leave_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 16;
			return true;
		}

		// Token: 0x060007F9 RID: 2041 RVA: 0x0003937D File Offset: 0x0003757D
		private void game_menu_naval_storyline_encounter_leave_on_consequence(MenuCallbackArgs args)
		{
			MenuHelper.EncounterLeaveConsequence();
		}

		// Token: 0x060007FA RID: 2042 RVA: 0x00039384 File Offset: 0x00037584
		private void game_menu_join_naval_storyline_encounter_on_init(MenuCallbackArgs args)
		{
			MapEvent encounteredBattle = PlayerEncounter.EncounteredBattle;
			PartyBase leaderParty = encounteredBattle.GetLeaderParty(1);
			PartyBase leaderParty2 = encounteredBattle.GetLeaderParty(0);
			if (leaderParty.IsMobile && leaderParty.MobileParty.Army != null)
			{
				MBTextManager.SetTextVariable("ATTACKER", leaderParty.MobileParty.ArmyName, false);
			}
			else
			{
				MBTextManager.SetTextVariable("ATTACKER", leaderParty.Name, false);
			}
			if (leaderParty2.IsMobile && leaderParty2.MobileParty.Army != null)
			{
				MBTextManager.SetTextVariable("DEFENDER", leaderParty2.MobileParty.ArmyName, false);
			}
			else
			{
				MBTextManager.SetTextVariable("DEFENDER", leaderParty2.Name, false);
			}
			MBTextManager.SetTextVariable("JOIN_ENCOUNTER_TEXT", GameTexts.FindText("str_come_across_battle", null), false);
		}

		// Token: 0x060007FB RID: 2043 RVA: 0x00039437 File Offset: 0x00037637
		private void game_menu_join_naval_storyline_encounter_leave_on_condition(MenuCallbackArgs args)
		{
			PlayerEncounter.Finish(true);
		}

		// Token: 0x060007FC RID: 2044 RVA: 0x0003943F File Offset: 0x0003763F
		private bool game_menu_join_naval_storyline_encounter_help_attackers_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 23;
			return PlayerEncounter.EncounteredBattle.CanPartyJoinBattle(PartyBase.MainParty, 1);
		}

		// Token: 0x060007FD RID: 2045 RVA: 0x00039459 File Offset: 0x00037659
		private void game_menu_join_naval_storyline_encounter_help_attackers_on_consequence(MenuCallbackArgs args)
		{
			PlayerEncounter.JoinBattle(1);
			GameMenu.SwitchToMenu("naval_storyline_encounter");
		}

		// Token: 0x060007FE RID: 2046 RVA: 0x0003946B File Offset: 0x0003766B
		private bool game_menu_join_naval_storyline_encounter_help_defenders_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 23;
			return PlayerEncounter.EncounteredBattle.CanPartyJoinBattle(PartyBase.MainParty, 0);
		}

		// Token: 0x060007FF RID: 2047 RVA: 0x00039485 File Offset: 0x00037685
		private void game_menu_join_naval_storyline_encounter_help_defenders_on_consequence(MenuCallbackArgs args)
		{
			PlayerEncounter.JoinBattle(0);
			GameMenu.ActivateGameMenu("naval_storyline_encounter");
		}

		// Token: 0x06000800 RID: 2048 RVA: 0x00039497 File Offset: 0x00037697
		private bool game_menu_join_naval_storyline_encounter_leave_no_army_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 16;
			MBTextManager.SetTextVariable("LEAVE_TEXT", "{=ebUwP3Q3}Don't get involved.", false);
			return true;
		}

		// Token: 0x06000801 RID: 2049 RVA: 0x000394B2 File Offset: 0x000376B2
		[GameMenuInitializationHandler("naval_storyline_encounter")]
		private static void game_menu_naval_storyline_encounter_on_init_background(MenuCallbackArgs args)
		{
			args.MenuContext.SetBackgroundMeshName("encounter_naval");
		}

		// Token: 0x06000802 RID: 2050 RVA: 0x000394C4 File Offset: 0x000376C4
		[GameMenuInitializationHandler("naval_storyline_encounter_meeting")]
		private static void game_menu_naval_storyline_encounter_meeting_on_init_background(MenuCallbackArgs args)
		{
			args.MenuContext.SetBackgroundMeshName("encounter_naval");
		}

		// Token: 0x06000803 RID: 2051 RVA: 0x000394D8 File Offset: 0x000376D8
		[GameMenuInitializationHandler("naval_storyline_join_encounter")]
		private static void game_menu_naval_storyline_join_encounter_on_init_background(MenuCallbackArgs args)
		{
			string encounterCultureBackgroundMesh = MenuHelper.GetEncounterCultureBackgroundMesh(PlayerEncounter.EncounteredParty.MapFaction.Culture);
			args.MenuContext.SetBackgroundMeshName(encounterCultureBackgroundMesh);
		}

		// Token: 0x06000804 RID: 2052 RVA: 0x00039508 File Offset: 0x00037708
		private void talk_to_gunnar_on_consequence(MenuCallbackArgs args)
		{
			this.leave_on_consequence(args);
			Mission mission;
			if (LocationComplex.Current != null && PlayerEncounter.LocationEncounter != null)
			{
				mission = (Mission)PlayerEncounter.LocationEncounter.CreateAndOpenMissionController(LocationComplex.Current.GetLocationWithId("port"), null, NavalStorylineData.Gunnar.CharacterObject, null);
			}
			else
			{
				Location locationWithId = NavalStorylineData.HomeSettlement.LocationComplex.GetLocationWithId("port");
				mission = (Mission)CampaignMission.OpenConversationMission(new ConversationCharacterData(CharacterObject.PlayerCharacter, PartyBase.MainParty, true, true, false, false, false, true), new ConversationCharacterData(NavalStorylineData.Gunnar.CharacterObject, PartyBase.MainParty, true, true, false, false, false, true), locationWithId.GetSceneName(NavalStorylineData.HomeSettlement.Town.GetWallLevel()), "", false);
			}
			this.RemoveCrimeHandler(mission);
		}

		// Token: 0x06000805 RID: 2053 RVA: 0x000395CA File Offset: 0x000377CA
		[GameMenuInitializationHandler("naval_storyline_encounter_blocking")]
		private static void naval_storyline_encounter_meeting_blocking_on_init_background(MenuCallbackArgs args)
		{
			args.MenuContext.SetBackgroundMeshName(SettlementHelper.FindNearestHideoutToMobileParty(MobileParty.MainParty, 3, null).WaitMeshName);
		}

		// Token: 0x06000806 RID: 2054 RVA: 0x000395E8 File Offset: 0x000377E8
		private void RemoveCrimeHandler(Mission mission)
		{
			MissionCrimeHandler missionBehavior = mission.GetMissionBehavior<MissionCrimeHandler>();
			if (missionBehavior != null)
			{
				mission.RemoveMissionBehavior(missionBehavior);
			}
		}

		// Token: 0x06000807 RID: 2055 RVA: 0x00039606 File Offset: 0x00037806
		[GameMenuInitializationHandler("naval_storyline_outside_town")]
		private static void naval_storyline_outside_town_on_init_background(MenuCallbackArgs args)
		{
			args.MenuContext.SetBackgroundMeshName(NavalStorylineData.HomeSettlement.SettlementComponent.WaitMeshName);
		}

		// Token: 0x06000808 RID: 2056 RVA: 0x00039622 File Offset: 0x00037822
		private bool talk_to_gunnar_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 1;
			return true;
		}

		// Token: 0x06000809 RID: 2057 RVA: 0x0003962C File Offset: 0x0003782C
		private void virtual_encounter_end_consequence(MenuCallbackArgs args)
		{
			PlayerEncounter.Finish(true);
		}

		// Token: 0x0600080A RID: 2058 RVA: 0x00039634 File Offset: 0x00037834
		private void virtual_encounter_init(MenuCallbackArgs args)
		{
		}

		// Token: 0x0600080B RID: 2059 RVA: 0x00039636 File Offset: 0x00037836
		private void CanHaveCampaignIssues(Hero hero, ref bool result)
		{
			if (NavalStorylineData.IsNavalStorylineHero(hero))
			{
				result = false;
			}
		}

		// Token: 0x0600080C RID: 2060 RVA: 0x00039643 File Offset: 0x00037843
		private void OnNavalStorylineSkipped()
		{
			this._lastCompletedStorylineStage = NavalStorylineData.NavalStorylineStage.Act2;
			this._isTutorialSkipped = true;
		}

		// Token: 0x0600080D RID: 2061 RVA: 0x00039654 File Offset: 0x00037854
		private void Tick(float dt)
		{
			if (!this._inquiryFired && !MobileParty.MainParty.IsInRaftState && this._isNavalStorylineActive && MobileParty.MainParty.IsCurrentlyAtSea && MobileParty.MainParty.IsTransitionInProgress)
			{
				InformationManager.ShowInquiry(new InquiryData(new TextObject("{=461jcc87}Leaving Story Mode", null).ToString(), new TextObject("{=dV92VE8i}When you leave story mode, you will be returned to Ostican. You can speak to Gunnar in port to try again later. Do you wish to continue?", null).ToString(), true, true, GameTexts.FindText("str_ok", null).ToString(), GameTexts.FindText("str_cancel", null).ToString(), new Action(this.OnAcceptDeactivatingNavalStoryline), new Action(this.OnRejectDeactivatingNavalStoryline), "", 0f, null, null, null), true, false);
			}
		}

		// Token: 0x0600080E RID: 2062 RVA: 0x00039711 File Offset: 0x00037911
		private void OnAcceptDeactivatingNavalStoryline()
		{
			this._inquiryFired = true;
		}

		// Token: 0x0600080F RID: 2063 RVA: 0x0003971A File Offset: 0x0003791A
		private void OnRejectDeactivatingNavalStoryline()
		{
			MobileParty.MainParty.SetMoveModeHold();
			MobileParty.MainParty.CancelNavigationTransition();
		}

		// Token: 0x06000810 RID: 2064 RVA: 0x00039730 File Offset: 0x00037930
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<bool>("_isActive", ref this._isNavalStorylineActive);
			dataStore.SyncData<TroopRoster>("_troops", ref this._troops);
			dataStore.SyncData<List<Ship>>("_ships", ref this._ships);
			dataStore.SyncData<TroopRoster>("_prisoners", ref this._prisoners);
			dataStore.SyncData<bool>("_inquiryFired", ref this._inquiryFired);
			dataStore.SyncData<AnchorPoint>("_cachedAnchor", ref this._cachedAnchor);
			dataStore.SyncData<NavalStorylineData.NavalStorylineStage>("_storylineStage", ref this._lastCompletedStorylineStage);
			dataStore.SyncData<bool>("_isNavalStorylineCanceled", ref this._isNavalStorylineCanceled);
			dataStore.SyncData<bool>("_isFirstReturnToOstican", ref this._isFirstReturnToOstican);
			dataStore.SyncData<bool>("_isTutorialSkipped", ref this._isTutorialSkipped);
			dataStore.SyncData<int>("_foodStage", ref this._foodStage);
			dataStore.SyncData<CampaignTime>("_sisterReturnTime", ref this._sisterReturnTime);
			dataStore.SyncData<NavalStorylineData.NavalStorylineCheckpoint>("_lastSavedCheckpoint", ref this._lastSavedCheckpoint);
		}

		// Token: 0x06000811 RID: 2065 RVA: 0x00039827 File Offset: 0x00037A27
		public bool IsNavalStorylineActive()
		{
			return this._isNavalStorylineActive;
		}

		// Token: 0x06000812 RID: 2066 RVA: 0x0003982F File Offset: 0x00037A2F
		private void CanHeroDie(Hero hero, KillCharacterAction.KillCharacterActionDetail causeOfDeath, ref bool result)
		{
			if (!this._isNavalStorylineCanceled && NavalStorylineData.IsNavalStorylineHero(hero) && !NavalStorylineData.HasCompletedLast(NavalStorylineData.NavalStorylineStage.Act3Quest5))
			{
				result = false;
			}
		}

		// Token: 0x06000813 RID: 2067 RVA: 0x0003984C File Offset: 0x00037A4C
		public NavalStorylineData.NavalStorylineSetPieceBattleMissionTypes GetNavalStorylineSetPieceBattleMissionType()
		{
			return this._activeMissionType;
		}

		// Token: 0x06000814 RID: 2068 RVA: 0x00039854 File Offset: 0x00037A54
		public void SetNavalStorylineSetPieceBattleMissionType(NavalStorylineData.NavalStorylineSetPieceBattleMissionTypes missionType)
		{
			this._activeMissionType = missionType;
		}

		// Token: 0x06000815 RID: 2069 RVA: 0x0003985D File Offset: 0x00037A5D
		public NavalStorylineData.NavalStorylineStage GetNavalStorylineStage()
		{
			return this._lastCompletedStorylineStage;
		}

		// Token: 0x06000816 RID: 2070 RVA: 0x00039865 File Offset: 0x00037A65
		public bool GetIsNavalStorylineCanceled()
		{
			return this._isNavalStorylineCanceled;
		}

		// Token: 0x06000817 RID: 2071 RVA: 0x0003986D File Offset: 0x00037A6D
		public bool IsTutorialSkipped()
		{
			return this._isTutorialSkipped;
		}

		// Token: 0x06000818 RID: 2072 RVA: 0x00039875 File Offset: 0x00037A75
		public void OnCheckpointReached(NavalStorylineData.NavalStorylineCheckpoint checkpoint)
		{
			if (checkpoint != this._lastSavedCheckpoint)
			{
				this._lastSavedCheckpoint = checkpoint;
				Campaign.Current.SaveHandler.ForceAutoSave();
			}
		}

		// Token: 0x06000819 RID: 2073 RVA: 0x00039896 File Offset: 0x00037A96
		public void ChangeNavalStorylineActivity(bool activity)
		{
			if (this._isNavalStorylineActive != activity)
			{
				this._isNavalStorylineActive = activity;
				this.OnActivityChanged(this._isNavalStorylineActive);
			}
		}

		// Token: 0x0600081A RID: 2074 RVA: 0x000398B6 File Offset: 0x00037AB6
		private void OnActivityChanged(bool newState)
		{
			this._inquiryFired = false;
			if (newState)
			{
				this.CacheTroopsAndShips();
			}
			else
			{
				this.ClearRosters();
				this.GetTroopsAndShipsFromCache();
				NavalStorylineData.TeleportMainHeroAndGunnarBackToBase();
			}
			MobileParty.MainParty.MemberRoster.UpdateVersion();
			NavalDLCEvents.Instance.OnNavalStorylineActivityChanged(newState);
		}

		// Token: 0x0600081B RID: 2075 RVA: 0x000398F5 File Offset: 0x00037AF5
		public bool IsWaitingForSistersReturn()
		{
			return this._sisterReturnTime != CampaignTime.Zero;
		}

		// Token: 0x0600081C RID: 2076 RVA: 0x00039908 File Offset: 0x00037B08
		public void GiveProvisionsToPlayer()
		{
			int num = (int)(this._lastCompletedStorylineStage + 1);
			if (this._foodStage < num)
			{
				this.GiveProvisionsToPlayerInternal();
				this._foodStage = (int)(this._lastCompletedStorylineStage + 1);
			}
		}

		// Token: 0x0600081D RID: 2077 RVA: 0x0003993C File Offset: 0x00037B3C
		private void GiveProvisionsToPlayerInternal()
		{
			float num = ((this._lastCompletedStorylineStage == NavalStorylineData.NavalStorylineStage.Act3Quest2) ? 7f : 5.5f);
			float num2 = num * MathF.Abs(MobileParty.MainParty.FoodChange);
			if (num2 > 0f)
			{
				ItemRosterElement itemRosterElement;
				itemRosterElement..ctor(DefaultItems.Grain, (int)(num2 / 2f), null);
				MobileParty.MainParty.ItemRoster.Add(itemRosterElement);
				ItemObject @object = MBObjectManager.Instance.GetObject<ItemObject>("fish");
				if (@object != null)
				{
					ItemRosterElement itemRosterElement2;
					itemRosterElement2..ctor(@object, (int)(num2 / 2f), null);
					MobileParty.MainParty.ItemRoster.Add(itemRosterElement2);
				}
			}
			int num3 = (int)((float)MobileParty.MainParty.TotalWage * num);
			num3 = (int)(Math.Round((double)((float)num3 / 100f)) * 100.0);
			GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, num3, false);
			InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=wJefidrb}Gunnar has secured some provisions for the journey.", null).ToString(), new Color(0f, 1f, 0f, 1f)));
		}

		// Token: 0x0600081E RID: 2078 RVA: 0x00039A40 File Offset: 0x00037C40
		private void GetTroopsAndShipsFromCache()
		{
			MBList<TroopRosterElement> troopRoster = this._troops.GetTroopRoster();
			for (int i = troopRoster.Count - 1; i >= 0; i--)
			{
				TroopRosterElement troopRosterElement = troopRoster[i];
				if (troopRosterElement.Character.IsHero)
				{
					troopRosterElement.Character.HeroObject.ChangeState(1);
				}
				MobileParty.MainParty.MemberRoster.AddToCounts(troopRosterElement.Character, troopRosterElement.Number, false, troopRosterElement.WoundedNumber, troopRosterElement.Xp, true, -1);
			}
			MBList<TroopRosterElement> troopRoster2 = this._prisoners.GetTroopRoster();
			for (int j = troopRoster2.Count - 1; j >= 0; j--)
			{
				TroopRosterElement troopRosterElement2 = troopRoster2[j];
				if (troopRosterElement2.Character.IsHero)
				{
					troopRosterElement2.Character.HeroObject.ChangeState(3);
				}
				MobileParty.MainParty.PrisonRoster.AddToCounts(troopRosterElement2.Character, troopRosterElement2.Number, false, troopRosterElement2.WoundedNumber, 0, true, -1);
			}
			this._troops.Clear();
			this._prisoners.Clear();
			for (int k = this._ships.Count - 1; k >= 0; k--)
			{
				Ship ship = this._ships[k];
				ChangeShipOwnerAction.ApplyByTransferring(PartyBase.MainParty, ship);
			}
			if (this._cachedAnchor != null)
			{
				MobileParty.MainParty.SetAnchor(this._cachedAnchor);
				this._cachedAnchor = null;
			}
			else
			{
				MobileParty.MainParty.Anchor.ResetPosition();
			}
			this._ships.Clear();
		}

		// Token: 0x0600081F RID: 2079 RVA: 0x00039BC0 File Offset: 0x00037DC0
		private void ClearRosters()
		{
			MBList<TroopRosterElement> troopRoster = MobileParty.MainParty.MemberRoster.GetTroopRoster();
			for (int i = troopRoster.Count - 1; i >= 0; i--)
			{
				TroopRosterElement troopRosterElement = troopRoster[i];
				if (troopRosterElement.Character != CharacterObject.PlayerCharacter)
				{
					MobileParty.MainParty.MemberRoster.AddToCounts(troopRosterElement.Character, -troopRosterElement.Number, false, -troopRosterElement.WoundedNumber, 0, true, -1);
				}
				if (troopRosterElement.Character.IsHero)
				{
					foreach (IMissionPlayerFollowerHandler missionPlayerFollowerHandler in Campaign.Current.CampaignBehaviorManager.GetBehaviors<IMissionPlayerFollowerHandler>())
					{
						missionPlayerFollowerHandler.RemoveFollowingHero(troopRosterElement.Character.HeroObject);
					}
				}
			}
			MBList<TroopRosterElement> troopRoster2 = MobileParty.MainParty.PrisonRoster.GetTroopRoster();
			for (int j = troopRoster2.Count - 1; j >= 0; j--)
			{
				TroopRosterElement troopRosterElement2 = troopRoster2[j];
				MobileParty.MainParty.PrisonRoster.AddToCounts(troopRosterElement2.Character, -troopRosterElement2.Number, false, -troopRosterElement2.WoundedNumber, 0, true, -1);
			}
			for (int k = PartyBase.MainParty.Ships.Count - 1; k >= 0; k--)
			{
				DestroyShipAction.Apply(PartyBase.MainParty.Ships[k]);
			}
		}

		// Token: 0x06000820 RID: 2080 RVA: 0x00039D2C File Offset: 0x00037F2C
		private void CacheTroopsAndShips()
		{
			MBList<TroopRosterElement> troopRoster = MobileParty.MainParty.MemberRoster.GetTroopRoster();
			for (int i = troopRoster.Count - 1; i >= 0; i--)
			{
				TroopRosterElement troopRosterElement = troopRoster[i];
				if (troopRosterElement.Character != CharacterObject.PlayerCharacter)
				{
					this._troops.Add(troopRosterElement);
					if (troopRosterElement.Character.IsHero)
					{
						troopRosterElement.Character.HeroObject.ChangeState(6);
					}
					MobileParty.MainParty.MemberRoster.AddToCountsAtIndex(i, -troopRosterElement.Number, -troopRosterElement.WoundedNumber, 0, true);
				}
			}
			MBList<TroopRosterElement> troopRoster2 = MobileParty.MainParty.PrisonRoster.GetTroopRoster();
			for (int j = troopRoster2.Count - 1; j >= 0; j--)
			{
				TroopRosterElement troopRosterElement2 = troopRoster2[j];
				this._prisoners.Add(troopRosterElement2);
				if (troopRosterElement2.Character.IsHero)
				{
					troopRosterElement2.Character.HeroObject.ChangeState(6);
				}
				MobileParty.MainParty.PrisonRoster.AddToCountsAtIndex(j, -troopRosterElement2.Number, -troopRosterElement2.WoundedNumber, 0, true);
			}
			this._cachedAnchor = (MobileParty.MainParty.Anchor.IsValid ? new AnchorPoint(MobileParty.MainParty.Anchor) : null);
			for (int k = MobileParty.MainParty.Ships.Count - 1; k >= 0; k--)
			{
				Ship ship = MobileParty.MainParty.Ships[k];
				ship.Owner = null;
				this._ships.Add(ship);
			}
			MobileParty.MainParty.Anchor.ResetPosition();
		}

		// Token: 0x040004EA RID: 1258
		private const int RansomGoldCost = 10000;

		// Token: 0x040004EB RID: 1259
		private bool _isNavalStorylineActive;

		// Token: 0x040004EC RID: 1260
		private NavalStorylineData.NavalStorylineSetPieceBattleMissionTypes _activeMissionType = NavalStorylineData.NavalStorylineSetPieceBattleMissionTypes.None;

		// Token: 0x040004ED RID: 1261
		private bool _isNavalStorylineCanceled;

		// Token: 0x040004EE RID: 1262
		private TroopRoster _troops = TroopRoster.CreateDummyTroopRoster();

		// Token: 0x040004EF RID: 1263
		private TroopRoster _prisoners = TroopRoster.CreateDummyTroopRoster();

		// Token: 0x040004F0 RID: 1264
		private List<Ship> _ships = new List<Ship>();

		// Token: 0x040004F1 RID: 1265
		private bool _inquiryFired;

		// Token: 0x040004F2 RID: 1266
		private AnchorPoint _cachedAnchor;

		// Token: 0x040004F3 RID: 1267
		private NavalStorylineData.NavalStorylineStage _lastCompletedStorylineStage = NavalStorylineData.NavalStorylineStage.None;

		// Token: 0x040004F4 RID: 1268
		private bool _isFirstReturnToOstican = true;

		// Token: 0x040004F5 RID: 1269
		private bool _isTutorialSkipped;

		// Token: 0x040004F6 RID: 1270
		private CampaignTime _sisterReturnTime = CampaignTime.Zero;

		// Token: 0x040004F7 RID: 1271
		private bool _removeCrimeHandler;

		// Token: 0x040004F8 RID: 1272
		private int _foodStage = 1;

		// Token: 0x040004F9 RID: 1273
		private NavalStorylineData.NavalStorylineCheckpoint _lastSavedCheckpoint;
	}
}
