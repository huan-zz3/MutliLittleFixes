using System;
using NavalDLC.CampaignBehaviors;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;

namespace NavalDLC.ViewModelCollection.ClanManagement
{
	// Token: 0x0200003B RID: 59
	public class NavalClanSettlementItemVM : ClanSettlementItemVM
	{
		// Token: 0x0600044F RID: 1103 RVA: 0x000140D6 File Offset: 0x000122D6
		public NavalClanSettlementItemVM(Settlement settlement, Action<ClanSettlementItemVM> onSelection, Action onShowSendMembers, ITeleportationCampaignBehavior teleportationBehavior)
			: base(settlement, onSelection, onShowSendMembers, teleportationBehavior)
		{
		}

		// Token: 0x06000450 RID: 1104 RVA: 0x000140E3 File Offset: 0x000122E3
		protected override ClanSettlementItemVM CreateSettlementItem(Settlement settlement, Action<ClanSettlementItemVM> onSelection, Action onShowSendMembers, ITeleportationCampaignBehavior teleportationBehavior)
		{
			return new NavalClanSettlementItemVM(settlement, onSelection, onShowSendMembers, teleportationBehavior);
		}

		// Token: 0x06000451 RID: 1105 RVA: 0x000140F0 File Offset: 0x000122F0
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
