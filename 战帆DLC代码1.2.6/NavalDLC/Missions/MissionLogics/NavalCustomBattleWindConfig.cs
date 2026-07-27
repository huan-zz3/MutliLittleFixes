using System;

namespace NavalDLC.Missions.MissionLogics
{
	// Token: 0x020000CC RID: 204
	public class NavalCustomBattleWindConfig
	{
		// Token: 0x04000951 RID: 2385
		public static NavalCustomBattleWindConfig.Direction WindDirection;

		// Token: 0x0200024E RID: 590
		public enum Direction
		{
			// Token: 0x04001052 RID: 4178
			TowardsDefender,
			// Token: 0x04001053 RID: 4179
			TowardsAttacker,
			// Token: 0x04001054 RID: 4180
			Side,
			// Token: 0x04001055 RID: 4181
			Random
		}
	}
}
