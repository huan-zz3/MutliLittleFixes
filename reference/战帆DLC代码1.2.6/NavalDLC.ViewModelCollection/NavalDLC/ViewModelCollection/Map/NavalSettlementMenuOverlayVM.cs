using System;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.Overlay;
using TaleWorlds.Core.ViewModelCollection.Information;

namespace NavalDLC.ViewModelCollection.Map
{
	// Token: 0x0200002D RID: 45
	[MenuOverlay("SettlementMenuOverlay")]
	public class NavalSettlementMenuOverlayVM : SettlementMenuOverlayVM
	{
		// Token: 0x060003E9 RID: 1001 RVA: 0x00012F0A File Offset: 0x0001110A
		public NavalSettlementMenuOverlayVM(GameMenu.MenuOverlayType type)
			: base(type)
		{
			base.ShipyardHint = new BasicTooltipViewModel(() => NavalUIHelper.GetShipyardTooltip(this._settlement.Town));
			this.RefreshValues();
		}

		// Token: 0x060003EA RID: 1002 RVA: 0x00012F30 File Offset: 0x00011130
		public override void RefreshValues()
		{
			base.RefreshValues();
			Town town = this._settlement.Town;
			Building building = ((town != null) ? town.GetShipyard() : null);
			base.IsShipyardEnabled = building != null;
			base.ShipyardLbl = (base.IsShipyardEnabled ? building.CurrentLevel.ToString() : string.Empty);
		}
	}
}
