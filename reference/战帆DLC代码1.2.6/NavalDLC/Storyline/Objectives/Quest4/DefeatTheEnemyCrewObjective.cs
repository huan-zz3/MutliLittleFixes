using System;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.Objectives;

namespace NavalDLC.Storyline.Objectives.Quest4
{
	// Token: 0x02000055 RID: 85
	public class DefeatTheEnemyCrewObjective : MissionObjective
	{
		// Token: 0x0600058C RID: 1420 RVA: 0x00022494 File Offset: 0x00020694
		public DefeatTheEnemyCrewObjective(Mission mission)
			: base(mission)
		{
		}

		// Token: 0x1700010D RID: 269
		// (get) Token: 0x0600058D RID: 1421 RVA: 0x0002249D File Offset: 0x0002069D
		public override string UniqueId
		{
			get
			{
				return "naval_storyline_quest_4_defeat_the_enemy_crew_objective";
			}
		}

		// Token: 0x1700010E RID: 270
		// (get) Token: 0x0600058E RID: 1422 RVA: 0x000224A4 File Offset: 0x000206A4
		public override TextObject Name
		{
			get
			{
				return new TextObject("{=7OeuYDQS}Defeat the Enemy Crew", null);
			}
		}

		// Token: 0x1700010F RID: 271
		// (get) Token: 0x0600058F RID: 1423 RVA: 0x000224B1 File Offset: 0x000206B1
		public override TextObject Description
		{
			get
			{
				return new TextObject("{=aImP2qRA}Defeat Crusas’ men in the battle aboard the floating fortress", null);
			}
		}

		// Token: 0x06000590 RID: 1424 RVA: 0x000224BE File Offset: 0x000206BE
		protected override bool IsActivationRequirementsMet()
		{
			return true;
		}

		// Token: 0x06000591 RID: 1425 RVA: 0x000224C1 File Offset: 0x000206C1
		protected override bool IsCompletionRequirementsMet()
		{
			return base.Mission.PlayerEnemyTeam.ActiveAgents.Count == 0;
		}
	}
}
