using System;
using NavalDLC.Settlements.Building;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;

namespace NavalDLC.GameComponents
{
	// Token: 0x0200010C RID: 268
	public class NavalDLCBuildingModel : BuildingModel
	{
		// Token: 0x06001383 RID: 4995 RVA: 0x0008D528 File Offset: 0x0008B728
		public override bool CanAddBuildingTypeToTown(BuildingType buildingType, Town town)
		{
			if (buildingType == NavalBuildingTypes.SettlementShipyard)
			{
				return town.IsTown && town.Settlement.HasPort;
			}
			return base.BaseModel.CanAddBuildingTypeToTown(buildingType, town);
		}
	}
}
