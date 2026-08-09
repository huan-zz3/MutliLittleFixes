using System;
using System.Collections.Generic;
using NavalDLC.CampaignBehaviors;
using NavalDLC.Map;
using NavalDLC.Storyline;
using NavalDLC.Storyline.CampaignBehaviors;
using NavalDLC.Storyline.MissionControllers;
using NavalDLC.Storyline.Quests;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.Core;
using TaleWorlds.SaveSystem;

namespace NavalDLC
{
	// Token: 0x02000026 RID: 38
	public class SaveableNavalDLCTypeDefiner : SaveableTypeDefiner
	{
		// Token: 0x06000196 RID: 406 RVA: 0x0000A36D File Offset: 0x0000856D
		public SaveableNavalDLCTypeDefiner()
			: base(520000)
		{
		}

		// Token: 0x06000197 RID: 407 RVA: 0x0000A37C File Offset: 0x0000857C
		protected override void DefineClassTypes()
		{
			base.AddClassDefinition(typeof(DefeatTheCaptorsQuest), 2, null);
			base.AddClassDefinition(typeof(SpeakToTheSailorsQuest), 9, null);
			base.AddClassDefinition(typeof(SailToTheGulfOfCharasQuest), 10, null);
			base.AddClassDefinition(typeof(HuntDownTheEmiraAlFahdaAndTheCorsairsQuest), 11, null);
			base.AddClassDefinition(typeof(ReturnToBaseQuest), 12, null);
			base.AddClassDefinition(typeof(SetSailAndEscortTheFortuneSeekersQuest), 13, null);
			base.AddClassDefinition(typeof(SetSailAndMeetTheFortuneSeekersInTargetSettlementQuest), 14, null);
			base.AddClassDefinition(typeof(GoToSkatriaIslandsQuest), 15, null);
			base.AddClassDefinition(typeof(CaptureTheImperialMerchantPrusas), 16, null);
			base.AddClassDefinition(typeof(InquireAtOstican), 17, null);
			base.AddClassDefinition(typeof(DefeatThePiratesQuest), 18, null);
			base.AddClassDefinition(typeof(FreeTheSeaHoundsCaptivesQuest), 19, null);
			base.AddClassDefinition(typeof(NavalOrderOfBattleCampaignBehavior.NavalOrderOfBattleFormationData), 20, null);
			base.AddClassDefinition(typeof(FishingPartyComponent), 21, null);
			base.AddClassDefinition(typeof(StormManager), 22, null);
			base.AddClassDefinition(typeof(Storm), 23, null);
			base.AddClassDefinition(typeof(SpeakToGunnarAndSisterQuest), 24, null);
			base.AddClassDefinition(typeof(ScourgeoftheSeasQuest), 25, null);
		}

		// Token: 0x06000198 RID: 408 RVA: 0x0000A4E0 File Offset: 0x000086E0
		protected override void DefineContainerDefinitions()
		{
			base.ConstructContainerDefinition(typeof(List<NavalOrderOfBattleCampaignBehavior.NavalOrderOfBattleFormationData>));
			base.ConstructContainerDefinition(typeof(List<Storm>));
			base.ConstructContainerDefinition(typeof(Storm.PreviousData[]));
			base.ConstructContainerDefinition(typeof(Dictionary<Ship, List<ShipUpgradePiece>>));
		}

		// Token: 0x06000199 RID: 409 RVA: 0x0000A52D File Offset: 0x0000872D
		protected override void DefineStructTypes()
		{
			base.AddStructDefinition(typeof(Storm.PreviousData), 1, null);
		}

		// Token: 0x0600019A RID: 410 RVA: 0x0000A544 File Offset: 0x00008744
		protected override void DefineEnumTypes()
		{
			base.AddEnumDefinition(typeof(NavalStorylineData.NavalStorylineStage), 1000, null);
			base.AddEnumDefinition(typeof(Storm.StormTypes), 1001, null);
			base.AddEnumDefinition(typeof(FreeTheSeaHoundsCaptivesQuest.FreeTheSeaHoundsCaptivesQuestState), 1002, null);
			base.AddEnumDefinition(typeof(Quest5SetPieceBattleMissionController.BossFightOutComeEnum), 1003, null);
			base.AddEnumDefinition(typeof(Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState), 1004, null);
			base.AddEnumDefinition(typeof(NavalStorylineThirdActFifthQuestBehaviour.NavalStorylineFinalQuestState), 1005, null);
			base.AddEnumDefinition(typeof(NavalStorylineData.NavalStorylineCheckpoint), 1006, null);
		}
	}
}
