using System;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace NavalDLC.ComponentInterfaces
{
	// Token: 0x02000156 RID: 342
	public abstract class ShipDistributionModel : MBGameModel<ShipDistributionModel>
	{
		// Token: 0x06001654 RID: 5716
		public abstract float GetScoreForPartyShipComposition(MobileParty party, MBReadOnlyList<Ship> shipsToConsider);

		// Token: 0x06001655 RID: 5717
		public abstract bool CanPartyTakeShip(PartyBase party, Ship ship);

		// Token: 0x06001656 RID: 5718
		public abstract bool CanSendShipToParty(Ship ship, MobileParty mobileParty);
	}
}
