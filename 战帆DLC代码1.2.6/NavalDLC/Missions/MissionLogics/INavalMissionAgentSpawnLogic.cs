using System;

namespace NavalDLC.Missions.MissionLogics
{
	// Token: 0x020000C9 RID: 201
	internal interface INavalMissionAgentSpawnLogic
	{
		// Token: 0x170002BC RID: 700
		// (get) Token: 0x06000F01 RID: 3841
		int DeployablePlayerShipCount { get; }

		// Token: 0x170002BD RID: 701
		// (get) Token: 0x06000F02 RID: 3842
		bool ReassignCaptainsOfRemovedShips { get; }

		// Token: 0x06000F03 RID: 3843
		void SetReassignCaptainsOfRemovedShips(bool value);

		// Token: 0x06000F04 RID: 3844
		void OnPlayerShipsUpdated();
	}
}
