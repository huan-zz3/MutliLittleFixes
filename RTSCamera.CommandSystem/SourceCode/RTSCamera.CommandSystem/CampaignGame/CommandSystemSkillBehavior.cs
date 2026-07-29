using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace RTSCamera.CommandSystem.CampaignGame
{
	// Token: 0x02000096 RID: 150
	public class CommandSystemSkillBehavior : CampaignBehaviorBase
	{
		// Token: 0x06000564 RID: 1380 RVA: 0x000200BC File Offset: 0x0001E2BC
		public override void RegisterEvents()
		{
			CampaignEvents.HeroGainedSkill.AddNonSerializedListener(this, new Action<Hero, SkillObject, int, bool>(this.OnHeroGainedSKill));
			CampaignEvents.OnMissionStartedEvent.AddNonSerializedListener(this, new Action<IMission>(this.OnMissionStarted));
		}

		// Token: 0x06000565 RID: 1381 RVA: 0x000200EC File Offset: 0x0001E2EC
		private void OnMissionStarted(IMission mission)
		{
			CommandSystemSkillBehavior.Update();
		}

		// Token: 0x06000566 RID: 1382 RVA: 0x000200F3 File Offset: 0x0001E2F3
		private void OnHeroGainedSKill(Hero hero, SkillObject skill, int change, bool shouldShowNotify)
		{
			if (Mission.Current != null && hero == CommandSystemSkillBehavior.GetHeroForTacticLevel())
			{
				CommandSystemSkillBehavior.Update();
			}
		}

		// Token: 0x06000567 RID: 1383 RVA: 0x00020109 File Offset: 0x0001E309
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06000568 RID: 1384 RVA: 0x0002010B File Offset: 0x0001E30B
		public static void Update()
		{
			CommandSystemSkillBehavior.CanIssueChargeToFormationOrder = CommandSystemSkillBehavior.CheckCanIssueChargeToFormationOrder();
		}

		// Token: 0x06000569 RID: 1385 RVA: 0x00020118 File Offset: 0x0001E318
		private static bool CheckCanIssueChargeToFormationOrder()
		{
			if (Campaign.Current == null)
			{
				return true;
			}
			Hero heroForTacticLevel = CommandSystemSkillBehavior.GetHeroForTacticLevel();
			return heroForTacticLevel == null || heroForTacticLevel.GetSkillValue(DefaultSkills.Tactics) >= CommandSystemSkillBehavior.RequiredTacticsLevelToIssueChargeToFormationOrder;
		}

		// Token: 0x0600056A RID: 1386 RVA: 0x0002014E File Offset: 0x0001E34E
		public static Hero GetHeroForTacticLevel()
		{
			Campaign campaign = Campaign.Current;
			object obj;
			if (campaign == null)
			{
				obj = null;
			}
			else
			{
				MobileParty mainParty = campaign.MainParty;
				obj = ((mainParty != null) ? mainParty.GetEffectiveRoleHolder(5) : null);
			}
			object obj2;
			if ((obj2 = obj) == null)
			{
				Game game = Game.Current;
				if (((game != null) ? game.PlayerTroop : null) != null)
				{
					return Hero.MainHero;
				}
				obj2 = null;
			}
			return obj2;
		}

		// Token: 0x0400029D RID: 669
		public static int RequiredTacticsLevelToIssueChargeToFormationOrder = 0;

		// Token: 0x0400029E RID: 670
		public static bool CanIssueChargeToFormationOrder = true;
	}
}
