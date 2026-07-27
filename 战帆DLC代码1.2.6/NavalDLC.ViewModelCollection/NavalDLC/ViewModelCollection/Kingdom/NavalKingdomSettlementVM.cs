using System;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Settlements;

namespace NavalDLC.ViewModelCollection.Kingdom
{
	// Token: 0x02000032 RID: 50
	public class NavalKingdomSettlementVM : KingdomSettlementVM
	{
		// Token: 0x060003F8 RID: 1016 RVA: 0x000132D6 File Offset: 0x000114D6
		public NavalKingdomSettlementVM(Action<KingdomDecision> forceDecision, Action<Settlement> onGrantFief)
			: base(forceDecision, onGrantFief)
		{
		}

		// Token: 0x060003F9 RID: 1017 RVA: 0x000132E0 File Offset: 0x000114E0
		protected override KingdomSettlementItemVM CreateSettlementItemVM(Settlement settlement, Action<KingdomSettlementItemVM> onSelect)
		{
			return new NavalKingdomSettlementItemVM(settlement, onSelect);
		}
	}
}
