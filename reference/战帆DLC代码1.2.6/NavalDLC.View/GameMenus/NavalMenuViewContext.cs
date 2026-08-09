using System;
using System.Collections.Generic;
using SandBox.View.Menu;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.ScreenSystem;

namespace NavalDLC.View.GameMenus
{
	// Token: 0x0200003C RID: 60
	public class NavalMenuViewContext : MenuViewContext
	{
		// Token: 0x060001D4 RID: 468 RVA: 0x0000DE42 File Offset: 0x0000C042
		public NavalMenuViewContext(ScreenBase screen, MenuContext menuContext)
			: base(screen, menuContext)
		{
		}

		// Token: 0x060001D5 RID: 469 RVA: 0x0000DE4C File Offset: 0x0000C04C
		protected override MenuView CreateTroopSelectionView(TroopRoster fullRoster, TroopRoster initialSelections, List<Ship> eligibleShips, Func<CharacterObject, bool> canChangeStatusOfTroop, Action<TroopRoster> onDone, int maxSelectableTroopCount, int minSelectableTroopCount, bool isNavalRaid)
		{
			if (isNavalRaid)
			{
				return base.AddMenuView<NavalMenuTroopSelectionView>(new object[] { fullRoster, initialSelections, eligibleShips, canChangeStatusOfTroop, onDone, maxSelectableTroopCount, minSelectableTroopCount });
			}
			return base.AddMenuView<MenuTroopSelectionView>(new object[] { fullRoster, initialSelections, canChangeStatusOfTroop, onDone, maxSelectableTroopCount, minSelectableTroopCount });
		}
	}
}
