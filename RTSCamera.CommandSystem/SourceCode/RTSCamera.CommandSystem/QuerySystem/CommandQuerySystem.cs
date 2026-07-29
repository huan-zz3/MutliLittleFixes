using System;
using System.Collections.Generic;
using TaleWorlds.MountAndBlade;

namespace RTSCamera.CommandSystem.QuerySystem
{
	// Token: 0x02000057 RID: 87
	public class CommandQuerySystem
	{
		// Token: 0x06000311 RID: 785 RVA: 0x0000D74A File Offset: 0x0000B94A
		public static void OnBehaviorInitialize()
		{
			CommandQuerySystem.FormationQuerySystem = new Dictionary<Formation, CommandFormationQuerySystem>();
		}

		// Token: 0x06000312 RID: 786 RVA: 0x0000D756 File Offset: 0x0000B956
		public static void OnRemoveBehavior()
		{
			CommandQuerySystem.FormationQuerySystem = null;
		}

		// Token: 0x06000313 RID: 787 RVA: 0x0000D760 File Offset: 0x0000B960
		public static CommandFormationQuerySystem GetQueryForFormation(Formation formation)
		{
			CommandFormationQuerySystem commandFormationQuerySystem;
			if (!CommandQuerySystem.FormationQuerySystem.TryGetValue(formation, out commandFormationQuerySystem))
			{
				commandFormationQuerySystem = (CommandQuerySystem.FormationQuerySystem[formation] = new CommandFormationQuerySystem(formation));
			}
			return commandFormationQuerySystem;
		}

		// Token: 0x0400013E RID: 318
		public static Dictionary<Formation, CommandFormationQuerySystem> FormationQuerySystem = new Dictionary<Formation, CommandFormationQuerySystem>();
	}
}
