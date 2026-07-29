using System;
using System.Runtime.CompilerServices;
using TaleWorlds.MountAndBlade;

namespace FormationFilter.Models
{
	// Token: 0x02000015 RID: 21
	public class FilteredAgent : IFilteredAgent
	{
		// Token: 0x060000C8 RID: 200 RVA: 0x00005ED2 File Offset: 0x000040D2
		[NullableContext(1)]
		public FilteredAgent(Agent agent)
		{
			this.Agent = agent;
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x00005EE1 File Offset: 0x000040E1
		public ulong GetAgentBitMask()
		{
			return TroopFilter.GetAgentBitMask(this.Agent);
		}

		// Token: 0x04000053 RID: 83
		[Nullable(1)]
		public readonly Agent Agent;
	}
}
