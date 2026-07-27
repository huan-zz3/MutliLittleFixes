using System;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement.Categories;

namespace NavalDLC.ViewModelCollection.ClanManagement
{
	// Token: 0x0200003A RID: 58
	public class NavalClanFiefsVM : ClanFiefsVM
	{
		// Token: 0x0600044D RID: 1101 RVA: 0x000140C0 File Offset: 0x000122C0
		public NavalClanFiefsVM(Action onRefresh, Action<ClanCardSelectionInfo> openCardSelectionPopup)
			: base(onRefresh, openCardSelectionPopup)
		{
		}

		// Token: 0x0600044E RID: 1102 RVA: 0x000140CA File Offset: 0x000122CA
		protected override ClanSettlementItemVM CreateSettlementItem(Settlement settlement, Action<ClanSettlementItemVM> onSelection, Action onShowSendMembers, ITeleportationCampaignBehavior teleportationBehavior)
		{
			return new NavalClanSettlementItemVM(settlement, onSelection, onShowSendMembers, teleportationBehavior);
		}
	}
}
