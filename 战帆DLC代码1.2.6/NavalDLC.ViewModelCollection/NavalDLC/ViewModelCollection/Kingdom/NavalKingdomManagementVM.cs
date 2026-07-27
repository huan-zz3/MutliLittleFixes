using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Settlements;

namespace NavalDLC.ViewModelCollection.Kingdom
{
	// Token: 0x02000031 RID: 49
	public class NavalKingdomManagementVM : KingdomManagementVM
	{
		// Token: 0x060003F6 RID: 1014 RVA: 0x000132C2 File Offset: 0x000114C2
		public NavalKingdomManagementVM(Action onClose, Action onManageArmy, Action<Army> onShowArmyOnMap)
			: base(onClose, onManageArmy, onShowArmyOnMap)
		{
		}

		// Token: 0x060003F7 RID: 1015 RVA: 0x000132CD File Offset: 0x000114CD
		protected override KingdomSettlementVM CreateSettlementVM(Action<KingdomDecision> forceDecision, Action<Settlement> onGrantFief)
		{
			return new NavalKingdomSettlementVM(forceDecision, onGrantFief);
		}
	}
}
