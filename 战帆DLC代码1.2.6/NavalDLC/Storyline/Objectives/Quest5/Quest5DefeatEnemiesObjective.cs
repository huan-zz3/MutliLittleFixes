using System;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.Objectives;

namespace NavalDLC.Storyline.Objectives.Quest5
{
	// Token: 0x0200004A RID: 74
	public class Quest5DefeatEnemiesObjective : MissionObjective
	{
		// Token: 0x170000EC RID: 236
		// (get) Token: 0x06000549 RID: 1353 RVA: 0x00021FF5 File Offset: 0x000201F5
		public override string UniqueId
		{
			get
			{
				return "quest_5_defeat_enemies_objective";
			}
		}

		// Token: 0x170000ED RID: 237
		// (get) Token: 0x0600054A RID: 1354 RVA: 0x00021FFC File Offset: 0x000201FC
		public override TextObject Name
		{
			get
			{
				return new TextObject("{=camyYPvf}Defeat the Sea Hound fleet", null);
			}
		}

		// Token: 0x170000EE RID: 238
		// (get) Token: 0x0600054B RID: 1355 RVA: 0x00022009 File Offset: 0x00020209
		public override TextObject Description
		{
			get
			{
				return new TextObject("{=fgshYPOw}Lead your fleet into battle and defeat your foes.", null);
			}
		}

		// Token: 0x0600054C RID: 1356 RVA: 0x00022016 File Offset: 0x00020216
		public Quest5DefeatEnemiesObjective(Mission mission, int phase3TotalEnemyCount)
			: base(mission)
		{
			this._phase3TotalEnemyCount = phase3TotalEnemyCount;
		}

		// Token: 0x0600054D RID: 1357 RVA: 0x00022026 File Offset: 0x00020226
		protected override bool IsActivationRequirementsMet()
		{
			return true;
		}

		// Token: 0x0600054E RID: 1358 RVA: 0x00022029 File Offset: 0x00020229
		protected override bool IsCompletionRequirementsMet()
		{
			return (float)Mission.Current.PlayerEnemyTeam.ActiveAgents.Count <= (float)this._phase3TotalEnemyCount * 0.01f;
		}

		// Token: 0x040002AB RID: 683
		private readonly int _phase3TotalEnemyCount;
	}
}
