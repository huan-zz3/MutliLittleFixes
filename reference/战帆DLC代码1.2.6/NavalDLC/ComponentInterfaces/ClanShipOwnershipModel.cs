using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace NavalDLC.ComponentInterfaces
{
	// Token: 0x02000153 RID: 339
	public abstract class ClanShipOwnershipModel : MBGameModel<ClanShipOwnershipModel>
	{
		// Token: 0x06001645 RID: 5701
		public abstract int GetIdealShipNumberForClan(Clan clan);
	}
}
