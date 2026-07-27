using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace NavalDLC.Storyline.Quests
{
	// Token: 0x0200003C RID: 60
	public class SailToTheGulfOfCharasQuest : NavalStorylineQuestBase
	{
		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x06000439 RID: 1081 RVA: 0x0001D439 File Offset: 0x0001B639
		public override bool WillProgressStoryline
		{
			get
			{
				return this._willProgressStoryline;
			}
		}

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x0600043A RID: 1082 RVA: 0x0001D441 File Offset: 0x0001B641
		public override TextObject Title
		{
			get
			{
				return new TextObject("{=LMRgfeFC}Sail to the Gulf of Charas", null);
			}
		}

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x0600043B RID: 1083 RVA: 0x0001D44E File Offset: 0x0001B64E
		private TextObject QuestStartLogText
		{
			get
			{
				TextObject textObject = new TextObject("{=7i9UFPLB}Find {HERO.NAME} in her hunting grounds in the Gulf of Charas", null);
				TextObjectExtensions.SetCharacterProperties(textObject, "HERO", NavalStorylineData.EmiraAlFahda.CharacterObject, false);
				return textObject;
			}
		}

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x0600043C RID: 1084 RVA: 0x0001D471 File Offset: 0x0001B671
		private TextObject QuestSuccessLogText
		{
			get
			{
				return new TextObject("{=lY5770ox}You found the corsairs.", null);
			}
		}

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x0600043D RID: 1085 RVA: 0x0001D47E File Offset: 0x0001B67E
		public override NavalStorylineData.NavalStorylineStage Stage
		{
			get
			{
				return NavalStorylineData.NavalStorylineStage.Act3Quest2;
			}
		}

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x0600043E RID: 1086 RVA: 0x0001D481 File Offset: 0x0001B681
		protected override string MainPartyTemplateStringId
		{
			get
			{
				return "storyline_act3_quest_2_main_party_template";
			}
		}

		// Token: 0x0600043F RID: 1087 RVA: 0x0001D488 File Offset: 0x0001B688
		public SailToTheGulfOfCharasQuest(string questId, Hero questGiver, CampaignVec2 corsairSpawnPosition)
			: base(questId, questGiver, CampaignTime.Never, 0)
		{
			this._corsairSpawnPosition = corsairSpawnPosition;
			this._willProgressStoryline = true;
			this._corsairHuntingGroundMarker = Campaign.Current.MapMarkerManager.CreateMapMarker(NavalStorylineData.CorsairBanner, new TextObject("{=QLrwlirp}Corsair Hunting Grounds", null), this._corsairSpawnPosition.AsVec3(), true, base.StringId);
		}

		// Token: 0x06000440 RID: 1088 RVA: 0x0001D4E8 File Offset: 0x0001B6E8
		protected override void SetDialogs()
		{
		}

		// Token: 0x06000441 RID: 1089 RVA: 0x0001D4EA File Offset: 0x0001B6EA
		protected override void OnStartQuestInternal()
		{
			this.InitializeQuestParty();
			base.AddLog(this.QuestStartLogText, false);
			base.AddTrackedObject(this._corsairHuntingGroundMarker);
		}

		// Token: 0x06000442 RID: 1090 RVA: 0x0001D50C File Offset: 0x0001B70C
		protected override void HourlyTick()
		{
			if (MobileParty.MainParty.SeeingRange > this._corsairSpawnPosition.Distance(MobileParty.MainParty.Position))
			{
				base.AddLog(this.QuestSuccessLogText, false);
				this._corsairHuntingGroundMarker.IsVisibleOnMap = false;
				Campaign.Current.TimeControlMode = 0;
				new HuntDownTheEmiraAlFahdaAndTheCorsairsQuest("naval_storyline_act3_quest2_2", NavalStorylineData.Gunnar, this._corsairSpawnPosition).StartQuest();
				TextObject textObject = new TextObject("{=tBigbw3U}You have reached the Gulf of Charas. Winds whip across the waves, carrying dust from the deserts, and visibility comes and goes. Lahar's ship keeps station several bowshots off of your port side, and together you comb the seas for the corsairs.", null);
				InformationManager.ShowInquiry(new InquiryData("", textObject.ToString(), true, false, GameTexts.FindText("str_continue", null).ToString(), GameTexts.FindText("str_no", null).ToString(), null, null, "", 0f, null, null, null), false, false);
				this._willProgressStoryline = false;
				base.CompleteQuestWithSuccess();
			}
		}

		// Token: 0x06000443 RID: 1091 RVA: 0x0001D5E0 File Offset: 0x0001B7E0
		protected override void IsNavalQuestPartyInternal(PartyBase party, NavalStorylinePartyData data)
		{
			if (party == PartyBase.MainParty)
			{
				data.PartySize++;
			}
		}

		// Token: 0x06000444 RID: 1092 RVA: 0x0001D5F8 File Offset: 0x0001B7F8
		protected override void RegisterEventsInternal()
		{
		}

		// Token: 0x06000445 RID: 1093 RVA: 0x0001D5FA File Offset: 0x0001B7FA
		protected override void OnFinalizeInternal()
		{
		}

		// Token: 0x06000446 RID: 1094 RVA: 0x0001D5FC File Offset: 0x0001B7FC
		protected override void OnCanceledInternal()
		{
			EnterSettlementAction.ApplyForCharacterOnly(NavalStorylineData.Lahar, NavalStorylineData.HomeSettlement);
			NavalStorylineData.Lahar.Heal(NavalStorylineData.Lahar.MaxHitPoints, false);
		}

		// Token: 0x06000447 RID: 1095 RVA: 0x0001D624 File Offset: 0x0001B824
		private void InitializeQuestParty()
		{
			NavalStorylineData.Lahar.ChangeState(1);
			NavalStorylineData.Lahar.Heal(NavalStorylineData.Lahar.MaxHitPoints, false);
			AddHeroToPartyAction.Apply(NavalStorylineData.Lahar, MobileParty.MainParty, true);
			foreach (Ship ship in MobileParty.MainParty.Ships)
			{
				if (ship.ShipHull.StringId == "ship_liburna_q2_storyline")
				{
					ship.ChangeFigurehead(DefaultFigureheads.Hawk);
					this.AddShipUpgradePieces(ship, SailToTheGulfOfCharasQuest.LaharShipUpgradePieces);
				}
				else if (ship.ShipHull.StringId == "northern_medium_ship")
				{
					ship.ChangeFigurehead(DefaultFigureheads.Dragon);
					this.AddShipUpgradePieces(ship, SailToTheGulfOfCharasQuest.GunnarShipUpgradePieces);
				}
			}
		}

		// Token: 0x06000448 RID: 1096 RVA: 0x0001D704 File Offset: 0x0001B904
		private void AddShipUpgradePieces(Ship ship, Dictionary<string, string> upgradePieces)
		{
			using (Dictionary<string, string>.Enumerator enumerator = upgradePieces.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, string> kv = enumerator.Current;
					ShipUpgradePiece @object = MBObjectManager.Instance.GetObject<ShipUpgradePiece>(kv.Value);
					if (ship.ShipHull.AvailableSlots.Any<KeyValuePair<string, ShipSlot>>((KeyValuePair<string, ShipSlot> slot) => slot.Key == kv.Key))
					{
						ship.EquipUpgradePiece(kv.Key, @object);
					}
				}
			}
		}

		// Token: 0x04000255 RID: 597
		private const string LaharShipHullId = "ship_liburna_q2_storyline";

		// Token: 0x04000256 RID: 598
		private static readonly Dictionary<string, string> LaharShipUpgradePieces = new Dictionary<string, string>
		{
			{ "side", "side_southern_shields_lvl3" },
			{ "sail", "sails_lvl2" },
			{ "bow", "bow_northern_reinforced_ram_lvl3" }
		};

		// Token: 0x04000257 RID: 599
		private const string GunnarShipHullId = "northern_medium_ship";

		// Token: 0x04000258 RID: 600
		private static readonly Dictionary<string, string> GunnarShipUpgradePieces = new Dictionary<string, string>
		{
			{ "side", "side_southern_shields_lvl2" },
			{ "sail", "sails_lvl2" }
		};

		// Token: 0x04000259 RID: 601
		[SaveableField(1)]
		private readonly CampaignVec2 _corsairSpawnPosition;

		// Token: 0x0400025A RID: 602
		[SaveableField(2)]
		private readonly MapMarker _corsairHuntingGroundMarker;

		// Token: 0x0400025B RID: 603
		[SaveableField(3)]
		private bool _willProgressStoryline;
	}
}
