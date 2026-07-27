using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace NavalDLC.Storyline.Quests
{
	// Token: 0x02000037 RID: 55
	public class GoToSkatriaIslandsQuest : NavalStorylineQuestBase
	{
		// Token: 0x1700008F RID: 143
		// (get) Token: 0x060003A3 RID: 931 RVA: 0x0001A81E File Offset: 0x00018A1E
		public override TextObject Title
		{
			get
			{
				return new TextObject("{=HEpykTDR}Go to the Skatria Islands", null);
			}
		}

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x060003A4 RID: 932 RVA: 0x0001A82B File Offset: 0x00018A2B
		private TextObject QuestSuccessLogText
		{
			get
			{
				return new TextObject("{=U6O5y26b}You found the Skatria Islands.", null);
			}
		}

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x060003A5 RID: 933 RVA: 0x0001A838 File Offset: 0x00018A38
		public override NavalStorylineData.NavalStorylineStage Stage
		{
			get
			{
				return NavalStorylineData.NavalStorylineStage.Act3Quest4;
			}
		}

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x060003A6 RID: 934 RVA: 0x0001A83B File Offset: 0x00018A3B
		public override bool WillProgressStoryline
		{
			get
			{
				return this._willProgressStoryline;
			}
		}

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x060003A7 RID: 935 RVA: 0x0001A843 File Offset: 0x00018A43
		protected override string MainPartyTemplateStringId
		{
			get
			{
				return "storyline_act3_quest_4_main_party_template";
			}
		}

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x060003A8 RID: 936 RVA: 0x0001A84A File Offset: 0x00018A4A
		private TextObject QuestStartLogText
		{
			get
			{
				TextObject textObject = new TextObject("{=5ygak6Ob}Sail to the Skatria Islands off {SETTLEMENT_NAME}", null);
				textObject.SetTextVariable("SETTLEMENT_NAME", NavalStorylineData.Act3Quest4TargetSettlement.Name);
				return textObject;
			}
		}

		// Token: 0x060003A9 RID: 937 RVA: 0x0001A870 File Offset: 0x00018A70
		public GoToSkatriaIslandsQuest(string questId, Hero questGiver, CampaignVec2 corsairSpawnPosition)
			: base(questId, questGiver, CampaignTime.Never, 0)
		{
			this._corsairSpawnPosition = corsairSpawnPosition;
			this._willProgressStoryline = true;
			this._skatriaIslandMarker = Campaign.Current.MapMarkerManager.CreateMapMarker(NavalStorylineData.CorsairBanner, new TextObject("{=9EIh8xRM}Skatria Islands", null), this._corsairSpawnPosition.AsVec3(), true, base.StringId);
		}

		// Token: 0x060003AA RID: 938 RVA: 0x0001A8D0 File Offset: 0x00018AD0
		protected override void RegisterEventsInternal()
		{
			CampaignEvents.TickEvent.AddNonSerializedListener(this, new Action<float>(this.OnTick));
		}

		// Token: 0x060003AB RID: 939 RVA: 0x0001A8E9 File Offset: 0x00018AE9
		protected override void SetDialogs()
		{
		}

		// Token: 0x060003AC RID: 940 RVA: 0x0001A8EB File Offset: 0x00018AEB
		protected override void InitializeQuestOnGameLoadInternal()
		{
		}

		// Token: 0x060003AD RID: 941 RVA: 0x0001A8ED File Offset: 0x00018AED
		protected override void OnStartQuestInternal()
		{
			this.InitializeQuestParty();
			base.AddLog(this.QuestStartLogText, false);
			this._skatriaIslandMarker.IsVisibleOnMap = true;
		}

		// Token: 0x060003AE RID: 942 RVA: 0x0001A90F File Offset: 0x00018B0F
		protected override void OnFinalizeInternal()
		{
			base.OnFinalizeInternal();
			this._skatriaIslandMarker.IsVisibleOnMap = false;
		}

		// Token: 0x060003AF RID: 943 RVA: 0x0001A924 File Offset: 0x00018B24
		private void InitializeQuestParty()
		{
			NavalStorylineData.Bjolgur.ChangeState(1);
			AddHeroToPartyAction.Apply(NavalStorylineData.Bjolgur, MobileParty.MainParty, true);
			foreach (Ship ship in MobileParty.MainParty.Ships)
			{
				foreach (KeyValuePair<string, string> keyValuePair in GoToSkatriaIslandsQuest.PlayerShipUpgradePieces)
				{
					if (ship.HasSlot(keyValuePair.Key))
					{
						ship.EquipUpgradePiece(keyValuePair.Key, MBObjectManager.Instance.GetObject<ShipUpgradePiece>(keyValuePair.Value));
					}
				}
				ship.ChangeFigurehead(DefaultFigureheads.Raven);
			}
			Ship ship2 = MobileParty.MainParty.Ships.FirstOrDefault<Ship>();
			if (ship2 != null)
			{
				ship2.ChangeFigurehead(DefaultFigureheads.Dragon);
			}
		}

		// Token: 0x060003B0 RID: 944 RVA: 0x0001AA20 File Offset: 0x00018C20
		private void OnTick(float deltaTime)
		{
			if (MobileParty.MainParty.SeeingRange + 5f > this._corsairSpawnPosition.Distance(MobileParty.MainParty.Position) && !Campaign.Current.QuestManager.IsThereActiveQuestWithType(typeof(CaptureTheImperialMerchantPrusas)))
			{
				base.AddLog(this.QuestSuccessLogText, false);
				this._willProgressStoryline = false;
				this._skatriaIslandMarker.IsVisibleOnMap = false;
				base.CompleteQuestWithSuccess();
				new CaptureTheImperialMerchantPrusas("naval_storyline_act3_quest4_2", NavalStorylineData.Gunnar, this._corsairSpawnPosition).StartQuest();
			}
		}

		// Token: 0x0400022E RID: 558
		private static readonly Dictionary<string, string> PlayerShipUpgradePieces = new Dictionary<string, string>
		{
			{ "sail", "sails_lvl2" },
			{ "side", "side_northern_shields_lvl2" }
		};

		// Token: 0x0400022F RID: 559
		[SaveableField(1)]
		private CampaignVec2 _corsairSpawnPosition;

		// Token: 0x04000230 RID: 560
		[SaveableField(2)]
		private readonly MapMarker _skatriaIslandMarker;

		// Token: 0x04000231 RID: 561
		[SaveableField(3)]
		private bool _willProgressStoryline;
	}
}
