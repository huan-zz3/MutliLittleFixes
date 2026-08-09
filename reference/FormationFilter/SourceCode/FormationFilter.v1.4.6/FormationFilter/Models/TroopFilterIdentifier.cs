using System;
using System.Runtime.CompilerServices;
using TaleWorlds.MountAndBlade;

namespace FormationFilter.Models
{
	// Token: 0x02000017 RID: 23
	[NullableContext(1)]
	[Nullable(0)]
	public class TroopFilterIdentifier
	{
		// Token: 0x060000CC RID: 204 RVA: 0x00005F0A File Offset: 0x0000410A
		public TroopFilterIdentifier(Formation formation, int index)
		{
			this.Formation = formation;
			this.Index = index;
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000CD RID: 205 RVA: 0x00005F20 File Offset: 0x00004120
		public Formation Formation { get; }

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000CE RID: 206 RVA: 0x00005F28 File Offset: 0x00004128
		public int Index { get; }

		// Token: 0x060000CF RID: 207 RVA: 0x00005F30 File Offset: 0x00004130
		public override bool Equals(object obj)
		{
			TroopFilterIdentifier troopFilterIdentifier = obj as TroopFilterIdentifier;
			return troopFilterIdentifier != null && this.Formation == troopFilterIdentifier.Formation && this.Index == troopFilterIdentifier.Index;
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x00005F67 File Offset: 0x00004167
		public override int GetHashCode()
		{
			return HashCode.Combine<Formation, int>(this.Formation, this.Index);
		}
	}
}
