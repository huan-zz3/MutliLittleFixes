using System;
using NavalDLC.CampaignBehaviors;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;

namespace NavalDLC.ViewModelCollection.Kingdom
{
	// Token: 0x02000033 RID: 51
	public class NavalKingdomSettlementItemVM : KingdomSettlementItemVM
	{
		// Token: 0x060003FA RID: 1018 RVA: 0x000132E9 File Offset: 0x000114E9
		public NavalKingdomSettlementItemVM(Settlement settlement, Action<KingdomSettlementItemVM> onSelect)
			: base(settlement, onSelect)
		{
		}

		// Token: 0x060003FB RID: 1019 RVA: 0x000132F4 File Offset: 0x000114F4
		protected override void UpdateProperties()
		{
			base.UpdateProperties();
			Town town = this.Settlement.Town;
			Building building = ((town != null) ? town.GetShipyard() : null);
			if (building != null)
			{
				BasicTooltipViewModel basicTooltipViewModel = new BasicTooltipViewModel(() => NavalUIHelper.GetShipyardTooltip(this.Settlement.Town));
				int currentLevel = building.CurrentLevel;
				base.ItemProperties.Insert(1, new SelectableFiefItemPropertyVM(GameTexts.FindText("str_shipyard", null).ToString(), currentLevel.ToString(), 0, 8, basicTooltipViewModel, false));
			}
			if (this.Settlement.IsTown && this.Settlement.HasPort)
			{
				BasicTooltipViewModel basicTooltipViewModel2 = new BasicTooltipViewModel(() => NavalUIHelper.GetTownCoastalPatrolTooltip(this.Settlement.Town));
				base.ItemProperties.Add(new SelectableFiefItemPropertyVM(GameTexts.FindText("str_coastal_patrol", null).ToString(), Campaign.Current.GetCampaignBehavior<INavalPatrolPartiesCampaignBehavior>().GetSettlementPatrolStatus(this.Settlement).ToString(), 0, 10, basicTooltipViewModel2, false));
			}
		}
	}
}
