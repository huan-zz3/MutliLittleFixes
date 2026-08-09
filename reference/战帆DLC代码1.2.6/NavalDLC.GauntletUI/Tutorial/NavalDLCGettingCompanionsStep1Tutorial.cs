using System;
using NavalDLC.Storyline;
using NavalDLC.Storyline.Quests;
using SandBox.GauntletUI.Tutorial;
using StoryMode.GauntletUI.Tutorial;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;

namespace NavalDLC.GauntletUI.Tutorial
{
	// Token: 0x02000006 RID: 6
	[Tutorial("GettingCompanionsStep1")]
	public class NavalDLCGettingCompanionsStep1Tutorial : GettingCompanionsStep1Tutorial
	{
		// Token: 0x0600000A RID: 10 RVA: 0x0000211C File Offset: 0x0000031C
		public override bool IsConditionsMetForActivation()
		{
			Settlement currentSettlement = Settlement.CurrentSettlement;
			return (currentSettlement == null || currentSettlement != NavalStorylineData.HomeSettlement || !Campaign.Current.QuestManager.IsThereActiveQuestWithType(typeof(InquireAtOstican))) && base.IsConditionsMetForActivation();
		}
	}
}
