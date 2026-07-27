using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.MissionLogics
{
	// Token: 0x020000D8 RID: 216
	internal struct NavalTroopAssignment
	{
		// Token: 0x170002FB RID: 763
		// (get) Token: 0x0600110C RID: 4364 RVA: 0x0007F023 File Offset: 0x0007D223
		public bool HasAgent
		{
			get
			{
				return this.Agent != null;
			}
		}

		// Token: 0x170002FC RID: 764
		// (get) Token: 0x0600110D RID: 4365 RVA: 0x0007F02E File Offset: 0x0007D22E
		public bool IsValid
		{
			get
			{
				return this.Origin != null;
			}
		}

		// Token: 0x170002FD RID: 765
		// (get) Token: 0x0600110E RID: 4366 RVA: 0x0007F039 File Offset: 0x0007D239
		public int Priority
		{
			get
			{
				return NavalTroopAssignment.GetPriority(this.Origin, this.Agent);
			}
		}

		// Token: 0x0600110F RID: 4367 RVA: 0x0007F04C File Offset: 0x0007D24C
		private NavalTroopAssignment(IAgentOriginBase origin, Agent agent = null)
		{
			this.Origin = origin;
			this.Agent = agent;
		}

		// Token: 0x06001110 RID: 4368 RVA: 0x0007F05C File Offset: 0x0007D25C
		public bool Equals(in NavalTroopAssignment other)
		{
			return this.Origin == other.Origin && this.Agent == other.Agent;
		}

		// Token: 0x06001111 RID: 4369 RVA: 0x0007F07C File Offset: 0x0007D27C
		public static NavalTroopAssignment Invalid()
		{
			return new NavalTroopAssignment(null, null);
		}

		// Token: 0x06001112 RID: 4370 RVA: 0x0007F085 File Offset: 0x0007D285
		public static NavalTroopAssignment Create(IAgentOriginBase origin, Agent agent = null)
		{
			return new NavalTroopAssignment(origin, agent);
		}

		// Token: 0x06001113 RID: 4371 RVA: 0x0007F090 File Offset: 0x0007D290
		public static int GetPriority(IAgentOriginBase origin, Agent agent = null)
		{
			bool flag = agent != null;
			if ((flag && agent.IsMainAgent) || origin.Troop.IsPlayerCharacter)
			{
				return 4;
			}
			bool isHero = origin.Troop.IsHero;
			if (flag)
			{
				if (!isHero)
				{
					return 2;
				}
				return 3;
			}
			else
			{
				if (!isHero)
				{
					return 0;
				}
				return 1;
			}
		}

		// Token: 0x040009E4 RID: 2532
		public readonly IAgentOriginBase Origin;

		// Token: 0x040009E5 RID: 2533
		public readonly Agent Agent;
	}
}
