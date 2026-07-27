using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement.Categories;

namespace NavalDLC.ViewModelCollection.ClanManagement
{
	// Token: 0x02000039 RID: 57
	public class NavalClanManagementVM : ClanManagementVM
	{
		// Token: 0x0600044B RID: 1099 RVA: 0x000140AA File Offset: 0x000122AA
		public NavalClanManagementVM(Action onClose, Action<Hero> showHeroOnMap, Action<Hero> openPartyAsManage, Action openBannerEditor)
			: base(onClose, showHeroOnMap, openPartyAsManage, openBannerEditor)
		{
		}

		// Token: 0x0600044C RID: 1100 RVA: 0x000140B7 File Offset: 0x000122B7
		protected override ClanFiefsVM CreateFiefsDataSource(Action onRefresh, Action<ClanCardSelectionInfo> openCardSelectionPopup)
		{
			return new NavalClanFiefsVM(onRefresh, openCardSelectionPopup);
		}
	}
}
