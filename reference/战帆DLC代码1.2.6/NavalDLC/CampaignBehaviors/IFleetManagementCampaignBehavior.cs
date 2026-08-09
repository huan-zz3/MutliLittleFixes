using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Naval;

namespace NavalDLC.CampaignBehaviors
{
	// Token: 0x02000161 RID: 353
	public interface IFleetManagementCampaignBehavior
	{
		// Token: 0x060016FB RID: 5883
		void SendShipToClan(Ship ship, Clan clan);
	}
}
