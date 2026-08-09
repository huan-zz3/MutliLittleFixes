using System;
using Helpers;
using NavalDLC.Storyline.CampaignBehaviors;
using NavalDLC.Storyline.Quests;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace NavalDLC.Storyline
{
	// Token: 0x02000031 RID: 49
	public class NavalStorylineData
	{
		// Token: 0x17000063 RID: 99
		// (get) Token: 0x060002C5 RID: 709 RVA: 0x00015987 File Offset: 0x00013B87
		public static Hero Gunnar
		{
			get
			{
				return NavalDLCManager.Instance.NavalStorylineData._gunnar;
			}
		}

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x060002C6 RID: 710 RVA: 0x00015998 File Offset: 0x00013B98
		public static Hero Bjolgur
		{
			get
			{
				return NavalDLCManager.Instance.NavalStorylineData._bjolgur;
			}
		}

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x060002C7 RID: 711 RVA: 0x000159A9 File Offset: 0x00013BA9
		public static Hero Purig
		{
			get
			{
				return NavalDLCManager.Instance.NavalStorylineData._purig;
			}
		}

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x060002C8 RID: 712 RVA: 0x000159BA File Offset: 0x00013BBA
		public static Hero Lahar
		{
			get
			{
				return NavalDLCManager.Instance.NavalStorylineData._lahar;
			}
		}

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x060002C9 RID: 713 RVA: 0x000159CB File Offset: 0x00013BCB
		public static Hero EmiraAlFahda
		{
			get
			{
				return NavalDLCManager.Instance.NavalStorylineData._emiraAlFahda;
			}
		}

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x060002CA RID: 714 RVA: 0x000159DC File Offset: 0x00013BDC
		public static Hero Prusas
		{
			get
			{
				return NavalDLCManager.Instance.NavalStorylineData._prusas;
			}
		}

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x060002CB RID: 715 RVA: 0x000159ED File Offset: 0x00013BED
		public static Settlement HomeSettlement
		{
			get
			{
				return NavalDLCManager.Instance.NavalStorylineData._homeSettlement;
			}
		}

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x060002CC RID: 716 RVA: 0x000159FE File Offset: 0x00013BFE
		public static Settlement Act3Quest1TargetSettlement
		{
			get
			{
				return NavalDLCManager.Instance.NavalStorylineData._act3Quest1TargetSettlement;
			}
		}

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x060002CD RID: 717 RVA: 0x00015A0F File Offset: 0x00013C0F
		public static Settlement Act3Quest2TargetSettlement
		{
			get
			{
				return NavalDLCManager.Instance.NavalStorylineData._act3Quest2TargetSettlement;
			}
		}

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x060002CE RID: 718 RVA: 0x00015A20 File Offset: 0x00013C20
		public static Settlement Act3Quest3TargetSettlement
		{
			get
			{
				return NavalDLCManager.Instance.NavalStorylineData._act3Quest3TargetSettlement;
			}
		}

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x060002CF RID: 719 RVA: 0x00015A31 File Offset: 0x00013C31
		public static Settlement Act3Quest4TargetSettlement
		{
			get
			{
				return NavalDLCManager.Instance.NavalStorylineData._act3Quest4TargetSettlement;
			}
		}

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x060002D0 RID: 720 RVA: 0x00015A42 File Offset: 0x00013C42
		public static Banner CorsairBanner
		{
			get
			{
				return NavalDLCManager.Instance.NavalStorylineData._corsairBanner;
			}
		}

		// Token: 0x060002D1 RID: 721 RVA: 0x00015A54 File Offset: 0x00013C54
		public static void OnCheckpointReached(NavalStorylineData.NavalStorylineCheckpoint checkpoint)
		{
			NavalStorylineCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<NavalStorylineCampaignBehavior>();
			if (campaignBehavior != null)
			{
				campaignBehavior.OnCheckpointReached(checkpoint);
			}
		}

		// Token: 0x060002D2 RID: 722 RVA: 0x00015A78 File Offset: 0x00013C78
		public void Initialize()
		{
			if (!NavalStorylineData.IsNavalStorylineCanceled())
			{
				this.CacheNavalStorylineSettlements();
				this.CreateStorylineHero("naval_storyline_gangradir", out this._gunnar);
				this.CreateStorylineHero("naval_storyline_bjolgur", out this._bjolgur);
				this.CreateStorylineHero("naval_storyline_northerner", out this._purig);
				this.CreateStorylineHero("naval_storyline_lahar", out this._lahar);
				this.CreateStorylineHero("naval_storyline_emira_al_fahda", out this._emiraAlFahda);
				this.CreateStorylineHero("naval_storyline_crusas", out this._prusas);
				this._corsairBanner = new Banner("11.97.166.1528.1528.764.764.1.0.0.500.35.171.555.555.764.764.0.0.0.167.35.171.350.350.764.764.0.0.0");
			}
		}

		// Token: 0x060002D3 RID: 723 RVA: 0x00015B08 File Offset: 0x00013D08
		private void CacheNavalStorylineSettlements()
		{
			this._homeSettlement = Settlement.Find("town_V8");
			this._act3Quest1TargetSettlement = Settlement.Find("town_N1");
			this._act3Quest2TargetSettlement = Settlement.Find("town_A1");
			this._act3Quest3TargetSettlement = Settlement.Find("town_S3");
			this._act3Quest4TargetSettlement = Settlement.Find("town_V7");
		}

		// Token: 0x060002D4 RID: 724 RVA: 0x00015B68 File Offset: 0x00013D68
		private void CreateStorylineHero(string stringId, out Hero hero)
		{
			hero = Campaign.Current.CampaignObjectManager.Find<Hero>(stringId);
			if (hero == null)
			{
				CharacterObject @object = MBObjectManager.Instance.GetObject<CharacterObject>(stringId);
				HeroCreator.CreateBasicHero(stringId, @object, ref hero, true);
				hero.SetName(@object.Name, @object.Name);
				CampaignTime randomBirthDayForAge = HeroHelper.GetRandomBirthDayForAge(@object.Age);
				hero.SetBirthDay(randomBirthDayForAge);
				hero.SetNewOccupation(31);
				if (@object.Culture.StringId == "aserai")
				{
					hero.BornSettlement = NavalStorylineData.Act3Quest2TargetSettlement;
				}
				else
				{
					hero.BornSettlement = NavalStorylineData.HomeSettlement;
				}
				NavalStorylineData.SetHeroText(hero);
			}
			hero.CharacterObject.SetTransferableInPartyScreen(false);
		}

		// Token: 0x060002D5 RID: 725 RVA: 0x00015C18 File Offset: 0x00013E18
		public static void SetHeroText(Hero hero)
		{
			if (hero == NavalStorylineData.Gunnar)
			{
				TextObject textObject = new TextObject("{=eW6Pbefi}Gunnar of {LAGSHOFN.NAME} is a Nord warrior from the island of Beinland. He won a reputation for courage fighting in a rebellion against {VOLBJORN.NAME}, first king of the Nordvyg, but after the rebels' defeat he made his peace with the victors. He has recently embarked on a campaign against the Sea Hounds, a pirate confederation that has terrorized the northern seas of Calradia.", null);
				StringHelpers.SetCharacterProperties("VOLBJORN", CharacterObject.Find("dead_lord_7_1"), textObject, false);
				StringHelpers.SetSettlementProperties("LAGSHOFN", Settlement.Find("village_N1_2"), textObject, false);
				hero.EncyclopediaText = textObject;
				return;
			}
			if (hero == NavalStorylineData.Purig)
			{
				TextObject textObject2 = new TextObject("{=bbBAWYbu}Purig is a Nord warrior who fought in the rebellion against {VOLBJORN.NAME}, first ruler of the Nordvyg. Following the king's victory he joined with other defeated rebels to form the Sea Hounds, a pirate confederation that is terrorizing the northern seas of Calradia.", null);
				StringHelpers.SetCharacterProperties("VOLBJORN", CharacterObject.Find("dead_lord_7_1"), textObject2, false);
				hero.EncyclopediaText = textObject2;
				return;
			}
			if (hero == NavalStorylineData.Lahar)
			{
				TextObject textObject3 = new TextObject("{=7y7cF9dC}Lahar is a sea captain and former corsair, now currently in the employ of the merchants of Quyaz.", null);
				hero.EncyclopediaText = textObject3;
				return;
			}
			if (hero == NavalStorylineData.EmiraAlFahda)
			{
				TextObject textObject4 = new TextObject("{=MADDAmO5}The Emira al-Fahda is a noblewoman from the city of Quyaz. She fell out with her uncles over an inheritance dispute and turned pirate, allying with the Sea Hounds and ravaging Quyazi shipping.", null);
				hero.EncyclopediaText = textObject4;
				return;
			}
			if (hero == NavalStorylineData.Prusas)
			{
				TextObject textObject5 = new TextObject("{=A0Zr68nk}Salautas Crusas is an imperial merchant who owns sulfur mines in the Gulf of Charas. He uses slaves, which he purchases from the Sea Hounds.", null);
				hero.EncyclopediaText = textObject5;
				return;
			}
			if (hero == NavalStorylineData.Bjolgur)
			{
				TextObject textObject6 = new TextObject("{=8qAnEXE5}Bjolgur of Agilting is a Nord warrior who fought alongside Gunnar in the rebellion against King Volbjorn. After the rebels' defeat he joined up with the Skolderbroda, a mercenary brotherhood.", null);
				hero.EncyclopediaText = textObject6;
			}
		}

		// Token: 0x060002D6 RID: 726 RVA: 0x00015D14 File Offset: 0x00013F14
		public static bool IsNavalStorylineHero(Hero hero)
		{
			return hero == NavalStorylineData.Gunnar || hero == NavalStorylineData.Purig || hero == NavalStorylineData.Bjolgur || hero == NavalStorylineData.Lahar || hero == NavalStorylineData.EmiraAlFahda || hero == NavalStorylineData.Prusas;
		}

		// Token: 0x060002D7 RID: 727 RVA: 0x00015D48 File Offset: 0x00013F48
		public static void StartNavalStoryline()
		{
			new InquireAtOstican().StartQuest();
		}

		// Token: 0x060002D8 RID: 728 RVA: 0x00015D54 File Offset: 0x00013F54
		public static bool IsStorylineActivationPossible()
		{
			return !Campaign.Current.IsMainHeroDisguised && MobileParty.MainParty.Army == null && !NavalStorylineData.IsWaitingForSistersReturn();
		}

		// Token: 0x060002D9 RID: 729 RVA: 0x00015D78 File Offset: 0x00013F78
		public static void ActivateNavalStoryline()
		{
			NavalStorylineCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<NavalStorylineCampaignBehavior>();
			if (campaignBehavior != null && !campaignBehavior.IsNavalStorylineActive())
			{
				campaignBehavior.ChangeNavalStorylineActivity(true);
			}
		}

		// Token: 0x060002DA RID: 730 RVA: 0x00015DA4 File Offset: 0x00013FA4
		public static void DeactivateNavalStoryline()
		{
			NavalStorylineCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<NavalStorylineCampaignBehavior>();
			if (campaignBehavior != null && campaignBehavior.IsNavalStorylineActive())
			{
				campaignBehavior.ChangeNavalStorylineActivity(false);
			}
		}

		// Token: 0x060002DB RID: 731 RVA: 0x00015DD0 File Offset: 0x00013FD0
		public static bool IsMainPartyAllowed()
		{
			SettlementAccessModel.AccessDetails accessDetails;
			Campaign.Current.Models.SettlementAccessModel.CanMainHeroEnterSettlement(Settlement.CurrentSettlement, ref accessDetails);
			return accessDetails.AccessLevel == 2 && !Clan.PlayerClan.MapFaction.IsAtWarWith(Settlement.CurrentSettlement.MapFaction) && (Settlement.CurrentSettlement.SiegeEvent == null || (!Settlement.CurrentSettlement.SiegeEvent.IsBlockadeActive && MobileParty.MainParty.HasNavalNavigationCapability));
		}

		// Token: 0x060002DC RID: 732 RVA: 0x00015E4C File Offset: 0x0001404C
		public static bool IsTutorialSkipped()
		{
			NavalStorylineCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<NavalStorylineCampaignBehavior>();
			return campaignBehavior != null && campaignBehavior.IsTutorialSkipped();
		}

		// Token: 0x060002DD RID: 733 RVA: 0x00015E70 File Offset: 0x00014070
		public static bool IsNavalStoryLineActive()
		{
			NavalStorylineCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<NavalStorylineCampaignBehavior>();
			return campaignBehavior != null && campaignBehavior.IsNavalStorylineActive();
		}

		// Token: 0x060002DE RID: 734 RVA: 0x00015E94 File Offset: 0x00014094
		public static bool HasCompletedLast(NavalStorylineData.NavalStorylineStage stage)
		{
			NavalStorylineCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<NavalStorylineCampaignBehavior>();
			return campaignBehavior != null && stage == campaignBehavior.GetNavalStorylineStage();
		}

		// Token: 0x060002DF RID: 735 RVA: 0x00015EBC File Offset: 0x000140BC
		public static NavalStorylineData.NavalStorylineStage GetStorylineStage()
		{
			NavalStorylineCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<NavalStorylineCampaignBehavior>();
			if (campaignBehavior != null)
			{
				return campaignBehavior.GetNavalStorylineStage();
			}
			return NavalStorylineData.NavalStorylineStage.None;
		}

		// Token: 0x060002E0 RID: 736 RVA: 0x00015EDF File Offset: 0x000140DF
		public static NavalStorylineData.NavalStorylineSetPieceBattleMissionTypes GetNavalStorylineSetPieceBattleMissionType()
		{
			NavalStorylineCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<NavalStorylineCampaignBehavior>();
			if (campaignBehavior == null)
			{
				return NavalStorylineData.NavalStorylineSetPieceBattleMissionTypes.None;
			}
			return campaignBehavior.GetNavalStorylineSetPieceBattleMissionType();
		}

		// Token: 0x060002E1 RID: 737 RVA: 0x00015EF8 File Offset: 0x000140F8
		public static bool IsWaitingForSistersReturn()
		{
			NavalStorylineCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<NavalStorylineCampaignBehavior>();
			return campaignBehavior != null && campaignBehavior.IsWaitingForSistersReturn();
		}

		// Token: 0x060002E2 RID: 738 RVA: 0x00015F1B File Offset: 0x0001411B
		public static void SetNavalStorylineSetPieceBattleMissionType(NavalStorylineData.NavalStorylineSetPieceBattleMissionTypes missionType)
		{
			NavalStorylineCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<NavalStorylineCampaignBehavior>();
			if (campaignBehavior == null)
			{
				return;
			}
			campaignBehavior.SetNavalStorylineSetPieceBattleMissionType(missionType);
		}

		// Token: 0x060002E3 RID: 739 RVA: 0x00015F34 File Offset: 0x00014134
		public static bool IsNavalStorylineCanceled()
		{
			NavalStorylineCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<NavalStorylineCampaignBehavior>();
			return campaignBehavior == null || campaignBehavior.GetIsNavalStorylineCanceled();
		}

		// Token: 0x060002E4 RID: 740 RVA: 0x00015F58 File Offset: 0x00014158
		public static void OnStorylineProgress(NavalStorylineQuestBase navalQuest)
		{
			NavalStorylineData.ActivateNavalStoryline();
			NavalStorylineData.FadeToBlack();
			if (navalQuest.Template != null)
			{
				MobileParty.MainParty.InitializeMobilePartyAtPosition(navalQuest.Template, MobileParty.MainParty.Position);
				foreach (Ship ship in MobileParty.MainParty.Ships)
				{
					ship.IsTradeable = false;
					ship.IsUsedByQuest = true;
				}
			}
			NavalStorylineData.AddGunnarToMainParty();
			NavalStorylineData.GiveProvisionsToPlayer();
			MobileParty.MainParty.SetSailAtPosition(Settlement.CurrentSettlement.PortPosition);
			PlayerEncounter.Finish(true);
		}

		// Token: 0x060002E5 RID: 741 RVA: 0x00016004 File Offset: 0x00014204
		private static void GiveProvisionsToPlayer()
		{
			NavalStorylineCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<NavalStorylineCampaignBehavior>();
			if (campaignBehavior == null)
			{
				return;
			}
			campaignBehavior.GiveProvisionsToPlayer();
		}

		// Token: 0x060002E6 RID: 742 RVA: 0x0001601A File Offset: 0x0001421A
		public static void AddGunnarToMainParty()
		{
			NavalStorylineData.Gunnar.Heal(NavalStorylineData.Gunnar.MaxHitPoints, false);
			MobileParty.MainParty.MemberRoster.AddToCounts(NavalStorylineData.Gunnar.CharacterObject, 1, false, 0, 0, true, -1);
		}

		// Token: 0x060002E7 RID: 743 RVA: 0x00016054 File Offset: 0x00014254
		public static void TeleportMainPartyBackToBase()
		{
			NavalStorylineData.FadeToBlack();
			if (MobileParty.MainParty.CurrentSettlement != NavalStorylineData.HomeSettlement)
			{
				MobileParty.MainParty.Position = (MobileParty.MainParty.HasNavalNavigationCapability ? NavalStorylineData.HomeSettlement.PortPosition : NavalStorylineData.HomeSettlement.GatePosition);
				MobileParty.MainParty.IsCurrentlyAtSea = MobileParty.MainParty.HasNavalNavigationCapability;
				if (PlayerEncounter.Current != null)
				{
					PlayerEncounter.Finish(true);
				}
				if (MobileParty.MainParty.IsInRaftState)
				{
					RaftStateChangeAction.DeactivateRaftStateForParty(MobileParty.MainParty);
				}
				if (Hero.MainHero.IsPrisoner)
				{
					EndCaptivityAction.ApplyByReleasedAfterBattle(Hero.MainHero);
				}
				if (MobileParty.MainParty.Anchor.IsValid)
				{
					MobileParty.MainParty.Anchor.SetPosition(new CampaignVec2(Vec2.Invalid, false));
				}
				EncounterManager.StartSettlementEncounter(MobileParty.MainParty, NavalStorylineData.HomeSettlement);
			}
			MobileParty.MainParty.SetMoveModeHold();
			MapState mapState;
			if ((mapState = GameStateManager.Current.ActiveState as MapState) != null)
			{
				mapState.Handler.TeleportCameraToMainParty();
			}
			NavalStorylineData.UpdateVisibilityAndInspectedAroundHomeSettlement();
		}

		// Token: 0x060002E8 RID: 744 RVA: 0x0001615A File Offset: 0x0001435A
		public static void TeleportMainHeroAndGunnarBackToBase()
		{
			NavalStorylineData.TeleportMainPartyBackToBase();
			NavalStorylineData.Gunnar.Heal(NavalStorylineData.Gunnar.MaxHitPoints, false);
			EnterSettlementAction.ApplyForCharacterOnly(NavalStorylineData.Gunnar, NavalStorylineData.HomeSettlement);
			GameMenu.ActivateGameMenu("naval_storyline_outside_town");
		}

		// Token: 0x060002E9 RID: 745 RVA: 0x00016190 File Offset: 0x00014390
		private static void UpdateVisibilityAndInspectedAroundHomeSettlement()
		{
			float seeingRange = MobileParty.MainParty.SeeingRange;
			CampaignVec2 position = NavalStorylineData.HomeSettlement.Position;
			LocatableSearchData<MobileParty> locatableSearchData = MobileParty.StartFindingLocatablesAroundPosition(position.ToVec2(), Campaign.Current.Models.MapVisibilityModel.MaximumSeeingRange() + 5f);
			NavalStorylineData.HomeSettlement.Party.UpdateVisibilityAndInspected(position, seeingRange);
			for (MobileParty mobileParty = MobileParty.FindNextLocatable(ref locatableSearchData); mobileParty != null; mobileParty = MobileParty.FindNextLocatable(ref locatableSearchData))
			{
				if (!mobileParty.IsMilitia && !mobileParty.IsGarrison)
				{
					mobileParty.Party.UpdateVisibilityAndInspected(position, seeingRange);
				}
			}
		}

		// Token: 0x060002EA RID: 746 RVA: 0x0001621D File Offset: 0x0001441D
		private static void FadeToBlack()
		{
			if (Game.Current.GameStateManager.ActiveState is MapState)
			{
				ScreenFadeController.BeginFadeOutAndIn(0.1f, 0.5f, 0.35f);
			}
		}

		// Token: 0x060002EB RID: 747 RVA: 0x0001624C File Offset: 0x0001444C
		public static MissionInitializerRecord GetNavalMissionInitializerTemplate(string sceneName)
		{
			MissionInitializerRecord missionInitializerRecord;
			missionInitializerRecord..ctor(sceneName);
			missionInitializerRecord.DamageToFriendsMultiplier = Campaign.Current.Models.DifficultyModel.GetPlayerTroopsReceivedDamageMultiplier();
			missionInitializerRecord.DamageFromPlayerToFriendsMultiplier = Campaign.Current.Models.DifficultyModel.GetPlayerTroopsReceivedDamageMultiplier();
			missionInitializerRecord.DecalAtlasGroup = 2;
			return missionInitializerRecord;
		}

		// Token: 0x060002EC RID: 748 RVA: 0x000162A0 File Offset: 0x000144A0
		public static void OnPlayerPostponedQuestStart()
		{
			if (!NavalStorylineData.IsMainPartyAllowed())
			{
				Mission mission = Mission.Current;
				if (mission == null)
				{
					return;
				}
				mission.EndMission();
			}
		}

		// Token: 0x040001D2 RID: 466
		public const string NavalStorylineSpecialQuestType = "NavalStoryline";

		// Token: 0x040001D3 RID: 467
		private const string GunnarStringId = "naval_storyline_gangradir";

		// Token: 0x040001D4 RID: 468
		private const string BjolgurStringId = "naval_storyline_bjolgur";

		// Token: 0x040001D5 RID: 469
		private const string PurigStringId = "naval_storyline_northerner";

		// Token: 0x040001D6 RID: 470
		private const string EmiraAlFahdaStringId = "naval_storyline_emira_al_fahda";

		// Token: 0x040001D7 RID: 471
		private const string LaharStringId = "naval_storyline_lahar";

		// Token: 0x040001D8 RID: 472
		private const string PrusasStringId = "naval_storyline_crusas";

		// Token: 0x040001D9 RID: 473
		public const string NavalStoryLineOutOfTownMenuId = "naval_storyline_outside_town";

		// Token: 0x040001DA RID: 474
		public const string NavalStoryLineEncounterBlockingMenuId = "naval_storyline_encounter_blocking";

		// Token: 0x040001DB RID: 475
		public const string NavalStoryLineVirtualPortMenuId = "naval_storyline_virtualport";

		// Token: 0x040001DC RID: 476
		public const string NavalStoryLineEncounterMeetingMenuId = "naval_storyline_encounter_meeting";

		// Token: 0x040001DD RID: 477
		public const string NavalStoryLineEncounterMenuId = "naval_storyline_encounter";

		// Token: 0x040001DE RID: 478
		public const string NavalStoryLineJoinEncounterMenuId = "naval_storyline_join_encounter";

		// Token: 0x040001DF RID: 479
		private const string HomeSettlementStringId = "town_V8";

		// Token: 0x040001E0 RID: 480
		private const string Act3Quest1TargetSettlementStringId = "town_N1";

		// Token: 0x040001E1 RID: 481
		private const string Act3Quest2TargetSettlementStringId = "town_A1";

		// Token: 0x040001E2 RID: 482
		private const string Act3Quest3TargetSettlementStringId = "town_S3";

		// Token: 0x040001E3 RID: 483
		private const string Act3Quest4TargetSettlementStringId = "town_V7";

		// Token: 0x040001E4 RID: 484
		public const string GunnarsVillageStringId = "village_N1_2";

		// Token: 0x040001E5 RID: 485
		public const string InquireAtOsticanCharacterSpawnPointTag = "sp_storyline_npc";

		// Token: 0x040001E6 RID: 486
		private Hero _gunnar;

		// Token: 0x040001E7 RID: 487
		private Hero _bjolgur;

		// Token: 0x040001E8 RID: 488
		private Hero _purig;

		// Token: 0x040001E9 RID: 489
		private Hero _lahar;

		// Token: 0x040001EA RID: 490
		private Hero _emiraAlFahda;

		// Token: 0x040001EB RID: 491
		private Hero _prusas;

		// Token: 0x040001EC RID: 492
		private Banner _corsairBanner;

		// Token: 0x040001ED RID: 493
		private Settlement _homeSettlement;

		// Token: 0x040001EE RID: 494
		private Settlement _act3Quest1TargetSettlement;

		// Token: 0x040001EF RID: 495
		private Settlement _act3Quest2TargetSettlement;

		// Token: 0x040001F0 RID: 496
		private Settlement _act3Quest3TargetSettlement;

		// Token: 0x040001F1 RID: 497
		private Settlement _act3Quest4TargetSettlement;

		// Token: 0x020001AA RID: 426
		public enum NavalStorylineStage
		{
			// Token: 0x04000CC4 RID: 3268
			None = -1,
			// Token: 0x04000CC5 RID: 3269
			Act1,
			// Token: 0x04000CC6 RID: 3270
			Act2,
			// Token: 0x04000CC7 RID: 3271
			Act3Quest1,
			// Token: 0x04000CC8 RID: 3272
			Act3Quest2,
			// Token: 0x04000CC9 RID: 3273
			Act3SpeakToSailors,
			// Token: 0x04000CCA RID: 3274
			Act3Quest4,
			// Token: 0x04000CCB RID: 3275
			Act3Quest5,
			// Token: 0x04000CCC RID: 3276
			Act3SpeakToGunnarAndSister
		}

		// Token: 0x020001AB RID: 427
		public enum NavalStorylineCheckpoint
		{
			// Token: 0x04000CCE RID: 3278
			None,
			// Token: 0x04000CCF RID: 3279
			Act1PortMenu,
			// Token: 0x04000CD0 RID: 3280
			Act1PortFightSucceeded,
			// Token: 0x04000CD1 RID: 3281
			Act1CaptivitySucceeded,
			// Token: 0x04000CD2 RID: 3282
			Act2EncounterMenu,
			// Token: 0x04000CD3 RID: 3283
			Act2Finalized,
			// Token: 0x04000CD4 RID: 3284
			Act3Quest1SetPieceEncounterMenu,
			// Token: 0x04000CD5 RID: 3285
			Act3Quest1SetPieceSucceeded,
			// Token: 0x04000CD6 RID: 3286
			Act3Quest2EncounterMenu,
			// Token: 0x04000CD7 RID: 3287
			Act3Quest2Succeeded,
			// Token: 0x04000CD8 RID: 3288
			Act3Quest3EncounterMenu,
			// Token: 0x04000CD9 RID: 3289
			Act3Quest3Succeeded,
			// Token: 0x04000CDA RID: 3290
			Act3Quest3InterceptedMenu,
			// Token: 0x04000CDB RID: 3291
			Act3Quest4EncounterMenu,
			// Token: 0x04000CDC RID: 3292
			Act3Quest4Succeeded,
			// Token: 0x04000CDD RID: 3293
			Act3Quest5EncounterMenu,
			// Token: 0x04000CDE RID: 3294
			Act3Quest5MissionMenu,
			// Token: 0x04000CDF RID: 3295
			Act3Quest5Succeeded
		}

		// Token: 0x020001AC RID: 428
		public enum StorylineCancelDetail
		{
			// Token: 0x04000CE1 RID: 3297
			ByDialogue,
			// Token: 0x04000CE2 RID: 3298
			ByRansom
		}

		// Token: 0x020001AD RID: 429
		public enum NavalStorylineSetPieceBattleMissionTypes
		{
			// Token: 0x04000CE4 RID: 3300
			None = -1,
			// Token: 0x04000CE5 RID: 3301
			Act1,
			// Token: 0x04000CE6 RID: 3302
			Act2,
			// Token: 0x04000CE7 RID: 3303
			Act3Quest1,
			// Token: 0x04000CE8 RID: 3304
			Act3Quest2,
			// Token: 0x04000CE9 RID: 3305
			Act3Quest3,
			// Token: 0x04000CEA RID: 3306
			Act3Quest4,
			// Token: 0x04000CEB RID: 3307
			Act3Quest5
		}
	}
}
