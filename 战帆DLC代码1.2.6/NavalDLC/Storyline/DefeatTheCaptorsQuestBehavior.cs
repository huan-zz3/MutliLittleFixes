using System;
using System.Collections.Generic;
using NavalDLC.Storyline.Quests;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;

namespace NavalDLC.Storyline
{
	// Token: 0x02000029 RID: 41
	public class DefeatTheCaptorsQuestBehavior : CampaignBehaviorBase
	{
		// Token: 0x17000057 RID: 87
		// (get) Token: 0x060001AF RID: 431 RVA: 0x0000A760 File Offset: 0x00008960
		private static DefeatTheCaptorsQuest Instance
		{
			get
			{
				DefeatTheCaptorsQuestBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<DefeatTheCaptorsQuestBehavior>();
				if (campaignBehavior._cachedQuest != null && campaignBehavior._cachedQuest.IsOngoing)
				{
					return campaignBehavior._cachedQuest;
				}
				using (List<QuestBase>.Enumerator enumerator = Campaign.Current.QuestManager.Quests.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						DefeatTheCaptorsQuest defeatTheCaptorsQuest;
						if ((defeatTheCaptorsQuest = enumerator.Current as DefeatTheCaptorsQuest) != null)
						{
							campaignBehavior._cachedQuest = defeatTheCaptorsQuest;
							return campaignBehavior._cachedQuest;
						}
					}
				}
				return null;
			}
		}

		// Token: 0x060001B0 RID: 432 RVA: 0x0000A7F8 File Offset: 0x000089F8
		public override void RegisterEvents()
		{
			if (!NavalStorylineData.IsNavalStorylineCanceled())
			{
				CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
			}
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x0000A818 File Offset: 0x00008A18
		private void OnSessionLaunched(CampaignGameStarter gameStarter)
		{
			gameStarter.AddGameMenu("defeat_the_captors_after_fight", "{=GDwBJZQr}For a brief moment, your captors seem to have forgotten about you, offering you a chance to break free from your shackles.", new OnInitDelegate(this.defeat_the_captors_after_fight_on_init), 0, 0, null);
			gameStarter.AddGameMenuOption("defeat_the_captors_after_fight", "defeat_the_captors_after_fight_attack", "{=zxMOqlhs}Attack", new GameMenuOption.OnConditionDelegate(this.defeat_the_captors_fight_on_condition), new GameMenuOption.OnConsequenceDelegate(this.defeat_the_captors_fight_on_consequence), false, -1, false, null);
		}

		// Token: 0x060001B2 RID: 434 RVA: 0x0000A875 File Offset: 0x00008A75
		private void defeat_the_captors_after_fight_on_init(MenuCallbackArgs args)
		{
			args.MenuContext.SetBackgroundMeshName("encounter_naval");
		}

		// Token: 0x060001B3 RID: 435 RVA: 0x0000A887 File Offset: 0x00008A87
		private bool defeat_the_captors_fight_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 17;
			return DefeatTheCaptorsQuestBehavior.Instance != null;
		}

		// Token: 0x060001B4 RID: 436 RVA: 0x0000A899 File Offset: 0x00008A99
		private void defeat_the_captors_fight_on_consequence(MenuCallbackArgs args)
		{
			if (DefeatTheCaptorsQuestBehavior.Instance != null)
			{
				Hero.MainHero.Heal(Hero.MainHero.MaxHitPoints, false);
				DefeatTheCaptorsQuestBehavior.Instance.StartMission();
			}
		}

		// Token: 0x060001B5 RID: 437 RVA: 0x0000A8C1 File Offset: 0x00008AC1
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x040000B7 RID: 183
		private DefeatTheCaptorsQuest _cachedQuest;
	}
}
