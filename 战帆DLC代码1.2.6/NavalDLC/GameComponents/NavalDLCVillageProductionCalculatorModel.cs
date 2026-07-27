using System;
using Helpers;
using NavalDLC.CharacterDevelopment;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace NavalDLC.GameComponents
{
	// Token: 0x0200013B RID: 315
	public class NavalDLCVillageProductionCalculatorModel : VillageProductionCalculatorModel
	{
		// Token: 0x0600153B RID: 5435 RVA: 0x00095768 File Offset: 0x00093968
		public override float CalculateProductionSpeedOfItemCategory(ItemCategory item)
		{
			return base.BaseModel.CalculateProductionSpeedOfItemCategory(item);
		}

		// Token: 0x0600153C RID: 5436 RVA: 0x00095778 File Offset: 0x00093978
		public override ExplainedNumber CalculateDailyProductionAmount(Village village, ItemObject item)
		{
			ExplainedNumber explainedNumber = base.BaseModel.CalculateDailyProductionAmount(village, item);
			if (village.TradeBound != null)
			{
				if (item.ItemCategory == NavalItemCategories.WalrusTusk || item.ItemCategory == NavalItemCategories.WhaleOil)
				{
					PerkHelper.AddPerkBonusForTown(NavalPerks.Boatswain.PortAuthority, village.TradeBound.Town, ref explainedNumber);
				}
				if (item.ItemCategory == DefaultItemCategories.Fish)
				{
					PerkHelper.AddPerkBonusForTown(NavalPerks.Boatswain.BlessingsOfTheSea, village.TradeBound.Town, ref explainedNumber);
				}
			}
			Clan ownerClan = village.Bound.OwnerClan;
			Kingdom kingdom = ((ownerClan != null) ? ownerClan.Kingdom : null);
			if (kingdom != null)
			{
				if (kingdom.HasPolicy(NavalPolicies.MaritimeWealEdict))
				{
					explainedNumber.AddFactor(0.25f, NavalPolicies.MaritimeWealEdict.Name);
				}
				if (kingdom.HasPolicy(NavalPolicies.BolsterTheFyrd))
				{
					explainedNumber.AddFactor(-0.05f, NavalPolicies.BolsterTheFyrd.Name);
				}
			}
			return explainedNumber;
		}

		// Token: 0x0600153D RID: 5437 RVA: 0x00095852 File Offset: 0x00093A52
		public override float CalculateDailyFoodProductionAmount(Village village)
		{
			return base.BaseModel.CalculateDailyFoodProductionAmount(village);
		}
	}
}
