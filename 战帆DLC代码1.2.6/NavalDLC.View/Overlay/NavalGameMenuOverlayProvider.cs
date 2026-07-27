using System;
using NavalDLC.ViewModelCollection.Map;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.Overlay;
using TaleWorlds.Library;

namespace NavalDLC.View.Overlay
{
	// Token: 0x02000017 RID: 23
	public class NavalGameMenuOverlayProvider : IGameMenuOverlayProvider
	{
		// Token: 0x06000094 RID: 148 RVA: 0x00005BE8 File Offset: 0x00003DE8
		public GameMenuOverlay GetOverlay(GameMenu.MenuOverlayType menuOverlayType)
		{
			if (menuOverlayType == 4)
			{
				return new EncounterMenuOverlayVM();
			}
			if (menuOverlayType == 1 || menuOverlayType == 2 || menuOverlayType == 3)
			{
				return new NavalSettlementMenuOverlayVM(menuOverlayType);
			}
			Debug.FailedAssert("Game menu overlay: " + menuOverlayType.ToString() + " could not be found", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.View\\Overlay\\NavalGameMenuOverlayProvider.cs", "GetOverlay", 23);
			return null;
		}
	}
}
