using System;
using System.Linq;
using NavalDLC.Map;
using NavalDLC.Storyline.Quests;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace NavalDLC.Storyline.CampaignBehaviors
{
	// Token: 0x0200007B RID: 123
	public class NavalStorylineTravelCommentaryCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x17000168 RID: 360
		// (get) Token: 0x060008CD RID: 2253 RVA: 0x0003DD68 File Offset: 0x0003BF68
		private float ImportantLocationLargeRadius
		{
			get
			{
				return 15f;
			}
		}

		// Token: 0x17000169 RID: 361
		// (get) Token: 0x060008CE RID: 2254 RVA: 0x0003DD6F File Offset: 0x0003BF6F
		private float CommentarySettlementArrivalRadius
		{
			get
			{
				return 20f;
			}
		}

		// Token: 0x1700016A RID: 362
		// (get) Token: 0x060008CF RID: 2255 RVA: 0x0003DD76 File Offset: 0x0003BF76
		private float SkatriaRadius
		{
			get
			{
				return 25f;
			}
		}

		// Token: 0x1700016B RID: 363
		// (get) Token: 0x060008D0 RID: 2256 RVA: 0x0003DD7D File Offset: 0x0003BF7D
		private float ByalicRadius
		{
			get
			{
				return 50f;
			}
		}

		// Token: 0x1700016C RID: 364
		// (get) Token: 0x060008D1 RID: 2257 RVA: 0x0003DD84 File Offset: 0x0003BF84
		private float GarantorRadius
		{
			get
			{
				return 10f;
			}
		}

		// Token: 0x1700016D RID: 365
		// (get) Token: 0x060008D2 RID: 2258 RVA: 0x0003DD8B File Offset: 0x0003BF8B
		private NavalStorylineCampaignBehavior StorylineBehavior
		{
			get
			{
				if (this._storylineBehavior == null)
				{
					this._storylineBehavior = Campaign.Current.GetCampaignBehavior<NavalStorylineCampaignBehavior>();
				}
				return this._storylineBehavior;
			}
		}

		// Token: 0x1700016E RID: 366
		// (get) Token: 0x060008D3 RID: 2259 RVA: 0x0003DDAB File Offset: 0x0003BFAB
		private bool IsStorylineActive
		{
			get
			{
				NavalStorylineCampaignBehavior storylineBehavior = this.StorylineBehavior;
				return storylineBehavior != null && storylineBehavior.IsNavalStorylineActive();
			}
		}

		// Token: 0x1700016F RID: 367
		// (get) Token: 0x060008D4 RID: 2260 RVA: 0x0003DDBE File Offset: 0x0003BFBE
		private NavalStorylineData.NavalStorylineStage CurrentStage
		{
			get
			{
				NavalStorylineCampaignBehavior storylineBehavior = this.StorylineBehavior;
				if (storylineBehavior == null)
				{
					return NavalStorylineData.NavalStorylineStage.None;
				}
				return storylineBehavior.GetNavalStorylineStage();
			}
		}

		// Token: 0x060008D5 RID: 2261 RVA: 0x0003DDD4 File Offset: 0x0003BFD4
		public override void RegisterEvents()
		{
			CampaignEvents.QuarterHourlyTickEvent.AddNonSerializedListener(this, new Action(this.QuarterlyHourlyTick));
			CampaignEvents.OnQuestStartedEvent.AddNonSerializedListener(this, new Action<QuestBase>(this.OnQuestStarted));
			CampaignEvents.OnQuestCompletedEvent.AddNonSerializedListener(this, new Action<QuestBase, QuestBase.QuestCompleteDetails>(this.OnQuestCompleted));
			CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener(this, new Action(this.OnGameLoadFinished));
		}

		// Token: 0x060008D6 RID: 2262 RVA: 0x0003DE40 File Offset: 0x0003C040
		private void OnQuestStarted(QuestBase quest)
		{
			NavalStorylineQuestBase navalStorylineQuestBase;
			if ((navalStorylineQuestBase = quest as NavalStorylineQuestBase) != null)
			{
				this.CanShowLowFoodCommentary = true;
				this.CanShowLowTroopsAndShipsCommentary = true;
				this.CanShowLowShipHpCommentary = true;
				this.CanShowStormCommentary = true;
				this._activeQuest = navalStorylineQuestBase;
				return;
			}
			if (quest is ReturnToBaseQuest)
			{
				this.CanShowStormCommentary = true;
				if (this.CurrentStage < NavalStorylineData.NavalStorylineStage.Act3Quest4)
				{
					if (this.CurrentStage == NavalStorylineData.NavalStorylineStage.None)
					{
						this.AddNotification(new TextObject("{=2epUWf2j}Let's row into harbor and tie her up.", null), 3);
						return;
					}
					if (this.CurrentStage == NavalStorylineData.NavalStorylineStage.Act1)
					{
						this.AddNotification(new TextObject("{=cXk7SjQD}Right. Let's get her docked.", null), 3);
						return;
					}
					if (this.CurrentStage == NavalStorylineData.NavalStorylineStage.Act2)
					{
						this.AddNotification(new TextObject("{=shqI8YFE}Time to go ashore.", null), 3);
						return;
					}
					if (this.CurrentStage == NavalStorylineData.NavalStorylineStage.Act3Quest1 || this.CurrentStage == NavalStorylineData.NavalStorylineStage.Act3SpeakToSailors || this.CurrentStage == NavalStorylineData.NavalStorylineStage.Act3Quest2)
					{
						this.CanShowNearOsticanCommentary = true;
						if (MBRandom.RandomFloat < 0.5f)
						{
							this.AddNotification(new TextObject("{=MbyjzdVW}Let's hope for a fair wind back to Ostican.", null), 3);
							return;
						}
						this.AddNotification(new TextObject("{=AS1hHmHa}And now back to Ostican…", null), 3);
					}
				}
			}
		}

		// Token: 0x060008D7 RID: 2263 RVA: 0x0003DF40 File Offset: 0x0003C140
		private void OnQuestCompleted(QuestBase quest, QuestBase.QuestCompleteDetails details)
		{
			if (quest is NavalStorylineQuestBase)
			{
				this.CanShowLowFoodCommentary = false;
				this.CanShowLowTroopsAndShipsCommentary = false;
				this.CanShowLowShipHpCommentary = false;
				this.CanShowStormCommentary = false;
				if (this._activeQuest != null && !this._activeQuest.IsOngoing)
				{
					this._activeQuest = null;
				}
				if (quest is DefeatThePiratesQuest)
				{
					this.DidShowAct2Commentary = false;
					return;
				}
				if (quest is SetSailAndMeetTheFortuneSeekersInTargetSettlementQuest)
				{
					this.DidShowAct3Q1Commentary1 = false;
					return;
				}
				if (quest is SetSailAndEscortTheFortuneSeekersQuest)
				{
					this.DidShowAct3Q1Commentary2 = false;
					return;
				}
				if (quest is HuntDownTheEmiraAlFahdaAndTheCorsairsQuest)
				{
					this.DidShowAct3Q2Commentary = false;
					return;
				}
				if (quest is SpeakToTheSailorsQuest)
				{
					this.DidShowAct3Q3Commentary = false;
					return;
				}
				if (quest is CaptureTheImperialMerchantPrusas)
				{
					this.DidShowAct3Q4Commentary = false;
					return;
				}
				if (quest is FreeTheSeaHoundsCaptivesQuest)
				{
					this.DidShowAct3Q5Commentary = false;
					return;
				}
			}
			else if (quest is ReturnToBaseQuest)
			{
				this.CanShowNearOsticanCommentary = false;
			}
		}

		// Token: 0x060008D8 RID: 2264 RVA: 0x0003E010 File Offset: 0x0003C210
		private void OnGameLoadFinished()
		{
			foreach (QuestBase questBase in Campaign.Current.QuestManager.Quests)
			{
				if (questBase.IsOngoing)
				{
					this.OnQuestStarted(questBase);
				}
			}
		}

		// Token: 0x060008D9 RID: 2265 RVA: 0x0003E074 File Offset: 0x0003C274
		private void QuarterlyHourlyTick()
		{
			if (this.CanComment())
			{
				bool flag = false;
				DefeatThePiratesQuest defeatThePiratesQuest;
				SetSailAndEscortTheFortuneSeekersQuest setSailAndEscortTheFortuneSeekersQuest;
				HuntDownTheEmiraAlFahdaAndTheCorsairsQuest huntDownTheEmiraAlFahdaAndTheCorsairsQuest;
				CaptureTheImperialMerchantPrusas captureTheImperialMerchantPrusas;
				if (!this.DidShowAct2Commentary && this._activeQuest != null && this._activeQuest.Stage == NavalStorylineData.NavalStorylineStage.Act2 && (defeatThePiratesQuest = this._activeQuest as DefeatThePiratesQuest) != null && defeatThePiratesQuest.IsPiratePartyVisible())
				{
					this.AddNotification(new TextObject("{=PHPX0Hhe}That ship out there, raising sail to intercept us. That's got to be one of them.", null), 4);
					this.DidShowAct2Commentary = true;
				}
				else if (!this.DidShowAct3Q1Commentary1 && this._activeQuest != null && this._activeQuest.Stage == NavalStorylineData.NavalStorylineStage.Act3Quest1 && this._activeQuest is SetSailAndMeetTheFortuneSeekersInTargetSettlementQuest && this.IsNearSettlement(NavalStorylineData.Act3Quest1TargetSettlement))
				{
					this.AddNotification(new TextObject("{=Td3JWbx9}We are nearing Hvalvik. Keep an eye out for sails. A pity I can't set my feet on the soil of dear old Beinland this time, but I don't want to miss that merchantman.", null), 4);
					this.DidShowAct3Q1Commentary1 = true;
				}
				else if (!this.DidShowAct3Q1Commentary2 && this._activeQuest != null && this._activeQuest.Stage == NavalStorylineData.NavalStorylineStage.Act3Quest1 && (setSailAndEscortTheFortuneSeekersQuest = this._activeQuest as SetSailAndEscortTheFortuneSeekersQuest) != null && setSailAndEscortTheFortuneSeekersQuest.AreEnemiesNearby())
				{
					this.AddNotification(new TextObject("{=5RgeG4bw}There's more of those devils! They seem to really want this prize.", null), 4);
					this.DidShowAct3Q1Commentary2 = true;
				}
				else if (!this.DidShowAct3Q2Commentary && this._activeQuest != null && this._activeQuest.Stage == NavalStorylineData.NavalStorylineStage.Act3Quest2 && (huntDownTheEmiraAlFahdaAndTheCorsairsQuest = this._activeQuest as HuntDownTheEmiraAlFahdaAndTheCorsairsQuest) != null && huntDownTheEmiraAlFahdaAndTheCorsairsQuest.IsFahdaVisible())
				{
					this.AddNotification(new TextObject("{=GgYW2rJX}Hard to make things out in this rough sea, but she's got to be around here somewhere.", null), 4);
					this.DidShowAct3Q2Commentary = true;
				}
				else if (!this.DidShowAct3Q3Commentary && this._activeQuest != null && this._activeQuest.Stage == NavalStorylineData.NavalStorylineStage.Act3SpeakToSailors && this.IsNearSettlement(NavalStorylineData.Act3Quest3TargetSettlement))
				{
					this.AddNotification(new TextObject("{=3bmPcXJA}We should be docking at Omor shortly. Not sure if we'll have time, but there's a place by the harbor that makes a fine herring pie…", null), 4);
					this.DidShowAct3Q3Commentary = true;
				}
				else if (!this.DidShowAct3Q4Commentary && this._activeQuest != null && this._activeQuest.Stage == NavalStorylineData.NavalStorylineStage.Act3Quest4 && (captureTheImperialMerchantPrusas = this._activeQuest as CaptureTheImperialMerchantPrusas) != null && captureTheImperialMerchantPrusas.IsCrusasVisible())
				{
					this.AddNotification(new TextObject("{=9C9hnWKO}The lookout has spotted sails. That may be Crusas's fleet.", null), 4);
					this.DidShowAct3Q4Commentary = true;
				}
				else if (!this.DidShowAct3Q5Commentary && this._activeQuest != null && this._activeQuest.Stage == NavalStorylineData.NavalStorylineStage.Act3Quest5 && NavalStorylineTravelCommentaryCampaignBehavior.IsNearLocation(this.AngrafjordLocation, this.ImportantLocationLargeRadius))
				{
					this.AddNotification(new TextObject("{=7rCbyb0F}This part of Beinland, here in the far north, is called the Iskap. I can see why Purig made his base here. There's little reason for most ships to come this way, unless they're chasing whale or walrus. Even if you're an old Beinlander like me, the wind that whips past the promontory will put a chill in your bones.", null), 4);
					this.DidShowAct3Q5Commentary = true;
				}
				else if (this._finisterCooldown.IsPast && NavalStorylineTravelCommentaryCampaignBehavior.IsNearLocation(this.FinisterreLocation, this.ImportantLocationLargeRadius) && (this._activeQuest == null || this._activeQuest.Stage != NavalStorylineData.NavalStorylineStage.Act3Quest1))
				{
					this.AddNotification(new TextObject("{=jzIhvbYe}That cliff jutting out into the sea over there, that's the Revelkap. The Battanians will tell you it's the end of the earth, and the spirits of the dead take flight here for the west. Sometimes you can hear a howling that will set your hair on edge, though I'd say it's just the wind passing through the sea-caves at its foot.", null), 3);
					this._finisterCooldown = CampaignTime.Never;
				}
				else if (this._byalicCooldown.IsPast && NavalStorylineTravelCommentaryCampaignBehavior.IsNearLocation(this.ByalicLocation, this.ByalicRadius))
				{
					this.AddNotification(new TextObject("{=gS2lr1Zq}We're well into the Byalic now. I know these waters well. When I was a boy, my father would bring me along on his trading voyages to the Sturgian lands. They'd stuff me between the bales of ivory and wrap me in a cloak, and I'd sit there looking out at the waves or playing with tafl-men. I don't think we have business here now, though. We've come too far east. Perhaps we should turn around?", null), 3);
					this._byalicCooldown = CampaignTime.Never;
				}
				else if (this._ransLaundryCooldown.IsPast && NavalStorylineTravelCommentaryCampaignBehavior.IsNearLocation(this.RansLaundryLocation, this.ImportantLocationLargeRadius))
				{
					this.AddNotification(new TextObject("{=m3bg8ahN}That passage between the isles off the coast and the mainland - we call that Ran's Laundry. The currents come through here fast and create whirlpools. The old lady of the sea washing out her nets, d'you think? Getting rid of fish-chewed bits of sailor in preparation for the next catch.", null), 3);
					this._ransLaundryCooldown = CampaignTime.Never;
				}
				else if (this._trandEstuaryCooldown.IsPast && NavalStorylineTravelCommentaryCampaignBehavior.IsNearLocation(this.TrandEstuaryLocation, this.ImportantLocationLargeRadius))
				{
					this.AddNotification(new TextObject("{=1oD3fc8a}We're coming up the mouth of the Trand. You can see the water change color - that rich soil washing miles out to sea. Our forefathers knew it as the gateway to the lands round Pravend, the finest raiding a warrior could want. Then the Vlandians came and decided not to bother with cramped, cold ships - they'd sit in a castle and raid the lands around them. Except they call it \"ruling,\" not raiding.", null), 3);
					this._trandEstuaryCooldown = CampaignTime.Never;
				}
				else if (this._charasCooldown.IsPast && NavalStorylineTravelCommentaryCampaignBehavior.IsNearLocation(this.GulfOfCharasLocation, this.SkatriaRadius) && (this._activeQuest == null || this._activeQuest.Stage != NavalStorylineData.NavalStorylineStage.Act3Quest2) && this.CurrentStage != NavalStorylineData.NavalStorylineStage.Act3Quest2)
				{
					this.AddNotification(new TextObject("{=7XyEmKQO}We're in the Gulf of Charas now. The fishermen here used to have this way of catching tuna - herd them into a maze of nets, into a smaller and smaller area, until they could just spear them from their boats and drag them out. Worked like dogs for a few days then ate like kings for months. Can't do that these days, though. Too many pirates about. Fishermen must be like fish, always moving to survive.", null), 3);
					this._charasCooldown = CampaignTime.Never;
				}
				else if (this._garantorCastleCooldown.IsPast && NavalStorylineTravelCommentaryCampaignBehavior.IsNearLocation(this.GarantorCastleLocation, this.GarantorRadius))
				{
					this.AddNotification(new TextObject("{=L7A9aFlB}We're coming up here on the Gates of Garantor, the passage to the Perassic Sea. Treacherous waters, these, all the more so if you're at the helm of an overladen drakkar and you can't slow down because there's three vengeful sambuks on your tail! Ah, I could tell you some stories. I need to ask, though - are you going to the Perassic? Seems a bit out of our way.", null), 3);
					this._garantorCastleCooldown = CampaignTime.Never;
				}
				else if (this._mazoporCooldown.IsPast && NavalStorylineTravelCommentaryCampaignBehavior.IsNearLocation(this.MarzoporLocation, this.ImportantLocationLargeRadius))
				{
					this.AddNotification(new TextObject("{=8FeBLAYc}This here is the estuary of Mazopor, taking us from the Byalic to Lake Laconis. We'd sail down here in peacetime if we wished to offer our services to the Sturgian boyars or the imperial governor at Diathma. In wartime, well... When you've got a lofty dromon bearing down on you, it's nice to have a bit more searoom to evade her than an estuary can give you.", null), 3);
					this._mazoporCooldown = CampaignTime.Never;
				}
				else if (this._galendCooldown.IsPast && NavalStorylineTravelCommentaryCampaignBehavior.IsNearLocation(this.GalendLocation, this.ImportantLocationLargeRadius))
				{
					this.AddNotification(new TextObject("{=IWNneauS}That's Galend over there, and beyond that the Biscan coast. I respect the Biscaners. Their sailors are as brave, and as hungry for wealth, as any Nord. They'll go far into the western ocean in pursuit of whales. They can be very cagey about their voyages, though, when you press them for details. Sometimes I wonder if they've found something out there.", null), 3);
					this._galendCooldown = CampaignTime.Never;
				}
				else if (this._rookCooldown.IsPast && MobileParty.MainParty.Position.X < this.RookBirdLocation.X && MobileParty.MainParty.Position.Y < this.RookBirdLocation.Y)
				{
					this.AddNotification(new TextObject("{=bvgLDvvS}Are you trying to take us to the Isle of Bounty, then, in the far south seas? Where you can walk the beach and gather emeralds like they were shells? Tempting, but also know that, in those waters, a great rook bird the size of a mountain might swoop down and pluck our ship right out of the waves. Let's think about turning around.", null), 3);
					this._rookCooldown = CampaignTime.Never;
				}
				else if (this.CanShowNearOsticanCommentary && this.IsNearSettlement(NavalStorylineData.HomeSettlement))
				{
					this.AddNotification(new TextObject("{=PmFbxbKk}Ostican in sight. Make ready to bring her in.", null), 2);
					this.CanShowNearOsticanCommentary = false;
				}
				else if (this._nextGoToPortEventTime.IsPast)
				{
					if (this.CanShowLowFoodCommentary && PartyBase.MainParty.IsStarving)
					{
						this.AddNotification(new TextObject("{=zmHH8l1p}Our food is running low. We should resupply in a nearby port.", null), 2);
						this.CanShowLowFoodCommentary = false;
					}
					else if (this.CanShowLowTroopsAndShipsCommentary && this.IsLowOnTroopsOrShips())
					{
						this.AddNotification(new TextObject("{=TujGZTu3}We have taken too many losses. Let's stop in a nearby port and gather reinforcements.", null), 2);
						this.CanShowLowTroopsAndShipsCommentary = false;
					}
					else
					{
						if (this.CanShowLowShipHpCommentary)
						{
							if (MobileParty.MainParty.Ships.Average<Ship>((Ship ship) => ship.HitPoints / ship.MaxHitPoints) < 0.6f)
							{
								this.AddNotification(new TextObject("{=Q7vglid0}Our ships are damaged. Let's stop in a nearby port to repair.", null), 2);
								goto IL_0643;
							}
						}
						if (this.CanShowStormCommentary && this.IsMainPartyInStorm() && (this._activeQuest == null || this._activeQuest.Stage != NavalStorylineData.NavalStorylineStage.Act3Quest2))
						{
							flag = true;
						}
					}
				}
				else if (this.CanShowStormCommentary && this.IsMainPartyInStorm() && (this._activeQuest == null || this._activeQuest.Stage != NavalStorylineData.NavalStorylineStage.Act3Quest2))
				{
					flag = true;
				}
				IL_0643:
				if (flag)
				{
					this.AddNotification(new TextObject("{=JdKcm9LY}Steer clear of these storms, if you can. The winds and waves can damage our vessels.", null), 2);
					this.CanShowStormCommentary = false;
				}
			}
		}

		// Token: 0x060008DA RID: 2266 RVA: 0x0003E6E0 File Offset: 0x0003C8E0
		private bool IsLowOnTroopsOrShips()
		{
			if (MobileParty.MainParty.PartySizeRatio < 0.5f)
			{
				return true;
			}
			NavalStorylinePartyData navalStorylinePartyData;
			if (MobileParty.MainParty.IsNavalStorylineQuestParty(out navalStorylinePartyData) && navalStorylinePartyData != null && navalStorylinePartyData.IsQuestParty)
			{
				return navalStorylinePartyData.Template.ShipHulls.Sum<ShipTemplateStack>((ShipTemplateStack x) => x.MaxValue) > MobileParty.MainParty.Ships.Count;
			}
			return false;
		}

		// Token: 0x060008DB RID: 2267 RVA: 0x0003E75C File Offset: 0x0003C95C
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<CampaignTime>("_galendCooldown", ref this._galendCooldown);
			dataStore.SyncData<CampaignTime>("_mazoporCooldown", ref this._mazoporCooldown);
			dataStore.SyncData<CampaignTime>("_garantorCastleCooldown", ref this._garantorCastleCooldown);
			dataStore.SyncData<CampaignTime>("_charasCooldown", ref this._charasCooldown);
			dataStore.SyncData<CampaignTime>("_trandEstuaryCooldown", ref this._trandEstuaryCooldown);
			dataStore.SyncData<CampaignTime>("_ransLaundryCooldown", ref this._ransLaundryCooldown);
			dataStore.SyncData<CampaignTime>("_finisterCooldown", ref this._finisterCooldown);
			dataStore.SyncData<CampaignTime>("_byalicCooldown", ref this._byalicCooldown);
			dataStore.SyncData<CampaignTime>("_rookCooldown", ref this._rookCooldown);
		}

		// Token: 0x060008DC RID: 2268 RVA: 0x0003E80C File Offset: 0x0003CA0C
		private bool CanComment()
		{
			return MobileParty.MainParty.IsActive && MobileParty.MainParty.IsCurrentlyAtSea && this.IsStorylineActive && !MobileParty.MainParty.IsInRaftState && MobileParty.MainParty.MapEvent == null && MobileParty.MainParty.SiegeEvent == null;
		}

		// Token: 0x060008DD RID: 2269 RVA: 0x0003E863 File Offset: 0x0003CA63
		private void AddNotification(TextObject text, MBInformationManager.NotificationPriority priority)
		{
			CampaignInformationManager.AddDialogLine(text, NavalStorylineData.Gunnar.CharacterObject, null, 0, priority);
			this._nextGoToPortEventTime = CampaignTime.HoursFromNow(6f);
		}

		// Token: 0x060008DE RID: 2270 RVA: 0x0003E88C File Offset: 0x0003CA8C
		private static bool IsNearLocation(CampaignVec2 location, float radius)
		{
			return MobileParty.MainParty.Position.DistanceSquared(location) <= radius * radius;
		}

		// Token: 0x060008DF RID: 2271 RVA: 0x0003E8B4 File Offset: 0x0003CAB4
		private bool IsNearSettlement(Settlement settlement)
		{
			float num = MobileParty.MainParty.Position.DistanceSquared(settlement.PortPosition);
			return (num <= this.CommentarySettlementArrivalRadius * this.CommentarySettlementArrivalRadius * 4f && MobileParty.MainParty.TargetSettlement == settlement) || num <= this.CommentarySettlementArrivalRadius * this.CommentarySettlementArrivalRadius;
		}

		// Token: 0x060008E0 RID: 2272 RVA: 0x0003E914 File Offset: 0x0003CB14
		private bool IsMainPartyInStorm()
		{
			foreach (Storm storm in NavalDLCManager.Instance.StormManager.SpawnedStorms)
			{
				if (storm.CurrentPosition.Distance(MobileParty.MainParty.Position.ToVec2()) < storm.EffectRadius * 0.9f)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x04000517 RID: 1303
		private CampaignVec2 RansLaundryLocation = new CampaignVec2(new Vec2(321.2f, 697.4f), false);

		// Token: 0x04000518 RID: 1304
		private CampaignVec2 FinisterreLocation = new CampaignVec2(new Vec2(202.24f, 621.91f), false);

		// Token: 0x04000519 RID: 1305
		private CampaignVec2 RookBirdLocation = new CampaignVec2(new Vec2(205f, 230f), false);

		// Token: 0x0400051A RID: 1306
		private CampaignVec2 TrandEstuaryLocation = new CampaignVec2(new Vec2(196.84f, 528.48f), false);

		// Token: 0x0400051B RID: 1307
		private CampaignVec2 GulfOfCharasLocation = new CampaignVec2(new Vec2(217f, 372f), false);

		// Token: 0x0400051C RID: 1308
		private CampaignVec2 GarantorCastleLocation = new CampaignVec2(new Vec2(359.33f, 304.45f), false);

		// Token: 0x0400051D RID: 1309
		private CampaignVec2 MarzoporLocation = new CampaignVec2(new Vec2(543.53f, 641.23f), false);

		// Token: 0x0400051E RID: 1310
		private CampaignVec2 GalendLocation = new CampaignVec2(new Vec2(195f, 437f), false);

		// Token: 0x0400051F RID: 1311
		private CampaignVec2 AngrafjordLocation = new CampaignVec2(new Vec2(260f, 770f), false);

		// Token: 0x04000520 RID: 1312
		private CampaignVec2 ByalicLocation = new CampaignVec2(new Vec2(555f, 724.72f), false);

		// Token: 0x04000521 RID: 1313
		private NavalStorylineCampaignBehavior _storylineBehavior;

		// Token: 0x04000522 RID: 1314
		private NavalStorylineQuestBase _activeQuest;

		// Token: 0x04000523 RID: 1315
		private CampaignTime _finisterCooldown = CampaignTime.Zero;

		// Token: 0x04000524 RID: 1316
		private CampaignTime _byalicCooldown = CampaignTime.Zero;

		// Token: 0x04000525 RID: 1317
		private CampaignTime _rookCooldown = CampaignTime.Zero;

		// Token: 0x04000526 RID: 1318
		private CampaignTime _ransLaundryCooldown = CampaignTime.Zero;

		// Token: 0x04000527 RID: 1319
		private CampaignTime _trandEstuaryCooldown = CampaignTime.Zero;

		// Token: 0x04000528 RID: 1320
		private CampaignTime _charasCooldown = CampaignTime.Zero;

		// Token: 0x04000529 RID: 1321
		private CampaignTime _garantorCastleCooldown = CampaignTime.Zero;

		// Token: 0x0400052A RID: 1322
		private CampaignTime _mazoporCooldown = CampaignTime.Zero;

		// Token: 0x0400052B RID: 1323
		private CampaignTime _galendCooldown = CampaignTime.Zero;

		// Token: 0x0400052C RID: 1324
		private bool DidShowAct2Commentary;

		// Token: 0x0400052D RID: 1325
		private bool DidShowAct3Q1Commentary1;

		// Token: 0x0400052E RID: 1326
		private bool DidShowAct3Q1Commentary2;

		// Token: 0x0400052F RID: 1327
		private bool DidShowAct3Q2Commentary;

		// Token: 0x04000530 RID: 1328
		private bool DidShowAct3Q3Commentary;

		// Token: 0x04000531 RID: 1329
		private bool DidShowAct3Q4Commentary;

		// Token: 0x04000532 RID: 1330
		private bool DidShowAct3Q5Commentary;

		// Token: 0x04000533 RID: 1331
		private CampaignTime _nextGoToPortEventTime = CampaignTime.Zero;

		// Token: 0x04000534 RID: 1332
		private bool CanShowLowFoodCommentary;

		// Token: 0x04000535 RID: 1333
		private bool CanShowLowShipHpCommentary;

		// Token: 0x04000536 RID: 1334
		private bool CanShowLowTroopsAndShipsCommentary;

		// Token: 0x04000537 RID: 1335
		private bool CanShowStormCommentary;

		// Token: 0x04000538 RID: 1336
		private bool CanShowNearOsticanCommentary;
	}
}
