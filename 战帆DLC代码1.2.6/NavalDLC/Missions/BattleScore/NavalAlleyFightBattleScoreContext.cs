using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.BattleScore;

namespace NavalDLC.Missions.BattleScore
{
	// Token: 0x020000E2 RID: 226
	public class NavalAlleyFightBattleScoreContext : BattleScoreContext
	{
		// Token: 0x060011DC RID: 4572 RVA: 0x00082949 File Offset: 0x00080B49
		public NavalAlleyFightBattleScoreContext(Mission mission)
		{
		}

		// Token: 0x17000311 RID: 785
		// (get) Token: 0x060011DD RID: 4573 RVA: 0x00082951 File Offset: 0x00080B51
		public override bool IsPowerComparisonRelevant
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060011DE RID: 4574 RVA: 0x00082954 File Offset: 0x00080B54
		public override Banner GetAttackerBanner()
		{
			return null;
		}

		// Token: 0x060011DF RID: 4575 RVA: 0x00082957 File Offset: 0x00080B57
		public override Banner GetDefenderBanner()
		{
			return null;
		}
	}
}
