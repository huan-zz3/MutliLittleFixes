using System;
using TaleWorlds.Library;

namespace NavalDLC.Missions.MissionLogics
{
	// Token: 0x020000D3 RID: 211
	public struct NavalShipDeploymentLimit
	{
		// Token: 0x170002E6 RID: 742
		// (get) Token: 0x06000FFB RID: 4091 RVA: 0x0007997C File Offset: 0x00077B7C
		public bool IsValid
		{
			get
			{
				return this.NetDeploymentLimit > 0;
			}
		}

		// Token: 0x170002E7 RID: 743
		// (get) Token: 0x06000FFC RID: 4092 RVA: 0x00079987 File Offset: 0x00077B87
		public int NetDeploymentLimit
		{
			get
			{
				return MathF.Min(MathF.Min(this.PartiesLimit, this.SkeletalCrewLimit), this.BattleAllocationLimit);
			}
		}

		// Token: 0x06000FFD RID: 4093 RVA: 0x000799A5 File Offset: 0x00077BA5
		public NavalShipDeploymentLimit(int partiesLimit, int skeletalCrewLimit, int battleAllocationLimit = 8)
		{
			this.PartiesLimit = partiesLimit;
			this.SkeletalCrewLimit = skeletalCrewLimit;
			this.BattleAllocationLimit = battleAllocationLimit;
		}

		// Token: 0x06000FFE RID: 4094 RVA: 0x000799BC File Offset: 0x00077BBC
		public NavalShipDeploymentLimit(int commonLimit)
		{
			this.PartiesLimit = commonLimit;
			this.SkeletalCrewLimit = commonLimit;
			this.BattleAllocationLimit = commonLimit;
		}

		// Token: 0x06000FFF RID: 4095 RVA: 0x000799D3 File Offset: 0x00077BD3
		public static NavalShipDeploymentLimit Invalid()
		{
			return new NavalShipDeploymentLimit(0, 0, 0);
		}

		// Token: 0x06001000 RID: 4096 RVA: 0x000799DD File Offset: 0x00077BDD
		public static NavalShipDeploymentLimit Max()
		{
			return new NavalShipDeploymentLimit(8, 8, 8);
		}

		// Token: 0x04000992 RID: 2450
		public readonly int PartiesLimit;

		// Token: 0x04000993 RID: 2451
		public readonly int SkeletalCrewLimit;

		// Token: 0x04000994 RID: 2452
		public readonly int BattleAllocationLimit;
	}
}
