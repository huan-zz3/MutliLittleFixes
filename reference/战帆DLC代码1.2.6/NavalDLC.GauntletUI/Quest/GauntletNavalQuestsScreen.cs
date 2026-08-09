using System;
using SandBox.GauntletUI;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.ViewModelCollection.Quests;
using TaleWorlds.MountAndBlade.View.Screens;

namespace NavalDLC.GauntletUI.Quest
{
	// Token: 0x02000012 RID: 18
	[GameStateScreen(typeof(QuestsState))]
	public class GauntletNavalQuestsScreen : GauntletQuestsScreen
	{
		// Token: 0x0600006C RID: 108 RVA: 0x00005553 File Offset: 0x00003753
		public GauntletNavalQuestsScreen(QuestsState questsState)
			: base(questsState)
		{
		}

		// Token: 0x0600006D RID: 109 RVA: 0x0000555C File Offset: 0x0000375C
		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			if (this._dataSource != null)
			{
				for (int i = 0; i < this._dataSource.ActiveQuestsList.Count; i++)
				{
					QuestItemVM questItemVM = this._dataSource.ActiveQuestsList[i];
					if (questItemVM.Quest != null)
					{
						questItemVM.IsNavalQuest = questItemVM.Quest.SpecialQuestType == "NavalStoryline";
					}
				}
			}
		}
	}
}
