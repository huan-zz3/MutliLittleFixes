using System;

namespace NavalDLC.Missions.Objects
{
	// Token: 0x020000A5 RID: 165
	public class ShipFireBallistaSpawner : ShipBallistaSpawner
	{
		// Token: 0x06000CD9 RID: 3289 RVA: 0x0006251E File Offset: 0x0006071E
		protected override void OnPreInit()
		{
			this._spawnerMissionHelper = new ShipSpawnerEntityMissionHelper(this, true);
		}
	}
}
