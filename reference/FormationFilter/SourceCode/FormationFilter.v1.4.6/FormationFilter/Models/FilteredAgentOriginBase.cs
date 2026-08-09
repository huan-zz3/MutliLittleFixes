using System;
using System.Runtime.CompilerServices;
using TaleWorlds.Core;

namespace FormationFilter.Models
{
	// Token: 0x02000016 RID: 22
	public class FilteredAgentOriginBase : IFilteredAgent
	{
		// Token: 0x060000CA RID: 202 RVA: 0x00005EEE File Offset: 0x000040EE
		[NullableContext(1)]
		public FilteredAgentOriginBase(IAgentOriginBase agentOriginBase)
		{
			this.AgentOriginBase = agentOriginBase;
		}

		// Token: 0x060000CB RID: 203 RVA: 0x00005EFD File Offset: 0x000040FD
		public ulong GetAgentBitMask()
		{
			return TroopFilter.GetIAgentOriginBaseBitMask(this.AgentOriginBase);
		}

		// Token: 0x04000054 RID: 84
		[Nullable(1)]
		public readonly IAgentOriginBase AgentOriginBase;
	}
}
