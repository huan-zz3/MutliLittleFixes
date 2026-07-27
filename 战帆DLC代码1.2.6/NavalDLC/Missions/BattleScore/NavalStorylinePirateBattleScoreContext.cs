using System;
using SandBox.Missions.BattleScore;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.BattleScore
{
	// Token: 0x020000E5 RID: 229
	public class NavalStorylinePirateBattleScoreContext : SandboxMissionBattleScoreContext
	{
		// Token: 0x060011E2 RID: 4578 RVA: 0x0008296C File Offset: 0x00080B6C
		public NavalStorylinePirateBattleScoreContext(Mission mission)
			: base(mission)
		{
		}

		// Token: 0x17000312 RID: 786
		// (get) Token: 0x060011E3 RID: 4579 RVA: 0x00082975 File Offset: 0x00080B75
		public override bool IsPowerComparisonRelevant
		{
			get
			{
				return false;
			}
		}
	}
}
