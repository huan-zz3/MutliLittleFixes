using System;
using System.Collections.Generic;
using NavalDLC.Missions.MissionLogics;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace NavalDLC.ComponentInterfaces
{
	// Token: 0x02000155 RID: 341
	public abstract class ShipDeploymentModel : MBGameModel<ShipDeploymentModel>
	{
		// Token: 0x0600164A RID: 5706
		public abstract int GetShipDeploymentLimit(MobileParty party);

		// Token: 0x0600164B RID: 5707
		public abstract void GetMapEventPartiesOfPlayerTeams(MBReadOnlyList<MapEventParty> playerSideMapEventParties, bool isPlayerSergeant, out MapEventParty playerMapEventParty, out MBList<MapEventParty> playerTeamMapEventParties, out MBList<MapEventParty> playerAllyTeamMapEventParties);

		// Token: 0x0600164C RID: 5708
		public abstract void GetShipDeploymentLimitsOfPlayerTeams(MBList<MapEventParty> playerTeamMapEventParties, MBList<MapEventParty> playerAllyTeamMapEventParties, out NavalShipDeploymentLimit playerTeamDeploymentLimit, out NavalShipDeploymentLimit playerAllyTeamDeploymentLimit);

		// Token: 0x0600164D RID: 5709
		public abstract NavalShipDeploymentLimit GetTeamShipDeploymentLimit(MBReadOnlyList<MapEventParty> teamMapEventParties);

		// Token: 0x0600164E RID: 5710
		public abstract Ship GetSuitablePlayerShip(MapEventParty playerMapEventParty, MBList<MapEventParty> playerTeamMapEventParties);

		// Token: 0x0600164F RID: 5711
		public abstract void FillShipsOfTeamParties(MBReadOnlyList<MapEventParty> teamMapEventParties, NavalShipDeploymentLimit shipDeploymentLimit, MBList<IShipOrigin> teamShips);

		// Token: 0x06001650 RID: 5712
		public abstract void GetOrderedCaptainsForPlayerTeamShips(MBReadOnlyList<MapEventParty> playerTeamMapEventParties, MBReadOnlyList<IShipOrigin> playerTeamShips, out List<string> playerTeamCaptainsByPriority);

		// Token: 0x06001651 RID: 5713
		public abstract int GetMaximumDeployableTroopCountForTeam(MBList<IShipOrigin> teamShips, bool isPlayerTeam = false);

		// Token: 0x04000B6F RID: 2927
		internal static bool IgnoreDeploymentLimits;
	}
}
