using System;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.Objects
{
	// Token: 0x0200009B RID: 155
	internal interface IShipEventListener
	{
		// Token: 0x06000AE0 RID: 2784
		void OnShipSpawned(MissionShip ship);

		// Token: 0x06000AE1 RID: 2785
		void OnShipRemoved(MissionShip ship);

		// Token: 0x06000AE2 RID: 2786
		void OnShipTransferred(MissionShip ship, Formation oldFormation);
	}
}
