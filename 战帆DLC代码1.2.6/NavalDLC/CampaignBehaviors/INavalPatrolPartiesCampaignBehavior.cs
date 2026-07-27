using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;

namespace NavalDLC.CampaignBehaviors
{
	// Token: 0x02000162 RID: 354
	public interface INavalPatrolPartiesCampaignBehavior
	{
		// Token: 0x060016FC RID: 5884
		TextObject GetSettlementPatrolStatus(Settlement settlement);

		// Token: 0x060016FD RID: 5885
		MobileParty GetNavalPatrolParty(Settlement settlement);
	}
}
