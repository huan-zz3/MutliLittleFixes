using System;
using NavalDLC.ComponentInterfaces;
using TaleWorlds.CampaignSystem;

namespace NavalDLC.GameComponents
{
	// Token: 0x02000113 RID: 275
	public class NavalDLCClanShipOwnershipModel : ClanShipOwnershipModel
	{
		// Token: 0x060013BF RID: 5055 RVA: 0x0008E625 File Offset: 0x0008C825
		public override int GetIdealShipNumberForClan(Clan clan)
		{
			return Campaign.Current.Models.ClanTierModel.GetPartyLimitForTier(clan, clan.Tier) * 3;
		}
	}
}
