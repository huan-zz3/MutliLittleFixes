using System;
using TaleWorlds.Library;

namespace NavalDLC.DWA
{
	// Token: 0x02000152 RID: 338
	public interface IDWAObstacleVertex
	{
		// Token: 0x170003DC RID: 988
		// (get) Token: 0x06001642 RID: 5698
		int Id { get; }

		// Token: 0x170003DD RID: 989
		// (get) Token: 0x06001643 RID: 5699
		Vec2 Point { get; }

		// Token: 0x170003DE RID: 990
		// (get) Token: 0x06001644 RID: 5700
		float PointZ { get; }
	}
}
