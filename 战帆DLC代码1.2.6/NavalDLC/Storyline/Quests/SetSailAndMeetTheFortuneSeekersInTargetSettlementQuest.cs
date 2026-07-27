using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace NavalDLC.Storyline.Quests
{
	// Token: 0x02000040 RID: 64
	public class SetSailAndMeetTheFortuneSeekersInTargetSettlementQuest : NavalStorylineQuestBase
	{
		// Token: 0x170000CB RID: 203
		// (get) Token: 0x060004B2 RID: 1202 RVA: 0x0001F69E File Offset: 0x0001D89E
		public override bool WillProgressStoryline
		{
			get
			{
				return this._willProgressStoryline;
			}
		}

		// Token: 0x170000CC RID: 204
		// (get) Token: 0x060004B3 RID: 1203 RVA: 0x0001F6A6 File Offset: 0x0001D8A6
		public override TextObject Title
		{
			get
			{
				TextObject textObject = new TextObject("{=ugNRbWrI}Meet the Vlandian Merchants", null);
				textObject.SetTextVariable("SETTLEMENT_NAME", this._targetSettlement.Name);
				return textObject;
			}
		}

		// Token: 0x170000CD RID: 205
		// (get) Token: 0x060004B4 RID: 1204 RVA: 0x0001F6CA File Offset: 0x0001D8CA
		public override NavalStorylineData.NavalStorylineStage Stage
		{
			get
			{
				return NavalStorylineData.NavalStorylineStage.Act3Quest1;
			}
		}

		// Token: 0x170000CE RID: 206
		// (get) Token: 0x060004B5 RID: 1205 RVA: 0x0001F6CD File Offset: 0x0001D8CD
		protected override string MainPartyTemplateStringId
		{
			get
			{
				return "storyline_act3_quest_1_main_party_template";
			}
		}

		// Token: 0x170000CF RID: 207
		// (get) Token: 0x060004B6 RID: 1206 RVA: 0x0001F6D4 File Offset: 0x0001D8D4
		private TextObject _descriptionLogText
		{
			get
			{
				TextObject textObject = new TextObject("{=eIGO7zhf}Locate the Vlandian merchant ship in the waters off {SETTLEMENT_LINK}.", null);
				textObject.SetTextVariable("SETTLEMENT_LINK", this._targetSettlement.EncyclopediaLinkWithName);
				return textObject;
			}
		}

		// Token: 0x060004B7 RID: 1207 RVA: 0x0001F6F8 File Offset: 0x0001D8F8
		public SetSailAndMeetTheFortuneSeekersInTargetSettlementQuest(string questId, Hero questGiver, Settlement targetSettlement)
			: base(questId, questGiver, CampaignTime.Never, 0)
		{
			this._willProgressStoryline = true;
			this._targetSettlement = targetSettlement;
			base.AddTrackedObject(this._targetSettlement);
			base.AddLog(this._descriptionLogText, false);
		}

		// Token: 0x060004B8 RID: 1208 RVA: 0x0001F730 File Offset: 0x0001D930
		protected override void OnStartQuestInternal()
		{
			NavalDLCHelpers.AddUpgradePiecesToPartyShips(MobileParty.MainParty, SetSailAndMeetTheFortuneSeekersInTargetSettlementQuest.PlayerShipUpgradePieces, DefaultFigureheads.Dragon);
			NavalDLCHelpers.SetCustomSailPatternOfPartyShips(MobileParty.MainParty, "generated_square__h4_09");
		}

		// Token: 0x060004B9 RID: 1209 RVA: 0x0001F755 File Offset: 0x0001D955
		protected override void InitializeQuestOnGameLoadInternal()
		{
			if (MobileParty.MainParty.IsActive)
			{
				NavalDLCHelpers.SetCustomSailPatternOfPartyShips(MobileParty.MainParty, "generated_square__h4_09");
			}
		}

		// Token: 0x060004BA RID: 1210 RVA: 0x0001F772 File Offset: 0x0001D972
		protected override void SetDialogs()
		{
		}

		// Token: 0x060004BB RID: 1211 RVA: 0x0001F774 File Offset: 0x0001D974
		protected override void HourlyTick()
		{
			if (MobileParty.MainParty.Position.Distance(this._targetSettlement.PortPosition) <= MathF.Min(10f, MobileParty.MainParty.SeeingRange))
			{
				this._willProgressStoryline = false;
				new SetSailAndEscortTheFortuneSeekersQuest("naval_storyline_act3_quest1_2", NavalStorylineData.Gunnar, this._targetSettlement).StartQuest();
				base.CompleteQuestWithSuccess();
			}
		}

		// Token: 0x060004BC RID: 1212 RVA: 0x0001F7DB File Offset: 0x0001D9DB
		protected override void RegisterEventsInternal()
		{
		}

		// Token: 0x04000278 RID: 632
		public const float DistanceToSettlementToSpawnMerchantParty = 10f;

		// Token: 0x04000279 RID: 633
		private static readonly Dictionary<string, string> PlayerShipUpgradePieces = new Dictionary<string, string>
		{
			{ "sail", "sails_lvl2" },
			{ "side", "side_northern_shields_lvl2" }
		};

		// Token: 0x0400027A RID: 634
		public const string PlayerPartySailPatternId = "generated_square__h4_09";

		// Token: 0x0400027B RID: 635
		[SaveableField(1)]
		private Settlement _targetSettlement;

		// Token: 0x0400027C RID: 636
		[SaveableField(2)]
		private bool _willProgressStoryline;
	}
}
