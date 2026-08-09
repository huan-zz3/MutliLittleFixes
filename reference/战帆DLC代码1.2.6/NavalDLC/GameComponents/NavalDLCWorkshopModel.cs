using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace NavalDLC.GameComponents
{
	// Token: 0x0200013D RID: 317
	public class NavalDLCWorkshopModel : WorkshopModel
	{
		// Token: 0x17000392 RID: 914
		// (get) Token: 0x06001542 RID: 5442 RVA: 0x000958BE File Offset: 0x00093ABE
		public override int DaysForPlayerSaveWorkshopFromBankruptcy
		{
			get
			{
				return base.BaseModel.DaysForPlayerSaveWorkshopFromBankruptcy;
			}
		}

		// Token: 0x17000393 RID: 915
		// (get) Token: 0x06001543 RID: 5443 RVA: 0x000958CB File Offset: 0x00093ACB
		public override int CapitalLowLimit
		{
			get
			{
				return base.BaseModel.CapitalLowLimit;
			}
		}

		// Token: 0x17000394 RID: 916
		// (get) Token: 0x06001544 RID: 5444 RVA: 0x000958D8 File Offset: 0x00093AD8
		public override int InitialCapital
		{
			get
			{
				return base.BaseModel.InitialCapital;
			}
		}

		// Token: 0x17000395 RID: 917
		// (get) Token: 0x06001545 RID: 5445 RVA: 0x000958E5 File Offset: 0x00093AE5
		public override int DailyExpense
		{
			get
			{
				return base.BaseModel.DailyExpense;
			}
		}

		// Token: 0x17000396 RID: 918
		// (get) Token: 0x06001546 RID: 5446 RVA: 0x000958F2 File Offset: 0x00093AF2
		public override int WarehouseCapacity
		{
			get
			{
				return base.BaseModel.WarehouseCapacity;
			}
		}

		// Token: 0x17000397 RID: 919
		// (get) Token: 0x06001547 RID: 5447 RVA: 0x000958FF File Offset: 0x00093AFF
		public override int DefaultWorkshopCountInSettlement
		{
			get
			{
				return base.BaseModel.DefaultWorkshopCountInSettlement;
			}
		}

		// Token: 0x17000398 RID: 920
		// (get) Token: 0x06001548 RID: 5448 RVA: 0x0009590C File Offset: 0x00093B0C
		public override int MaximumWorkshopsPlayerCanHave
		{
			get
			{
				return base.BaseModel.MaximumWorkshopsPlayerCanHave;
			}
		}

		// Token: 0x06001549 RID: 5449 RVA: 0x00095919 File Offset: 0x00093B19
		public override int GetMaxWorkshopCountForClanTier(int tier)
		{
			return base.BaseModel.GetMaxWorkshopCountForClanTier(tier);
		}

		// Token: 0x0600154A RID: 5450 RVA: 0x00095927 File Offset: 0x00093B27
		public override int GetCostForPlayer(Workshop workshop)
		{
			return base.BaseModel.GetCostForPlayer(workshop);
		}

		// Token: 0x0600154B RID: 5451 RVA: 0x00095935 File Offset: 0x00093B35
		public override int GetCostForNotable(Workshop workshop)
		{
			return base.BaseModel.GetCostForNotable(workshop);
		}

		// Token: 0x0600154C RID: 5452 RVA: 0x00095943 File Offset: 0x00093B43
		public override Hero GetNotableOwnerForWorkshop(Workshop workshop)
		{
			return base.BaseModel.GetNotableOwnerForWorkshop(workshop);
		}

		// Token: 0x0600154D RID: 5453 RVA: 0x00095954 File Offset: 0x00093B54
		public override ExplainedNumber GetEffectiveConversionSpeedOfProduction(Workshop workshop, float speed, bool includeDescriptions)
		{
			ExplainedNumber effectiveConversionSpeedOfProduction = base.BaseModel.GetEffectiveConversionSpeedOfProduction(workshop, speed, includeDescriptions);
			Clan clan = workshop.Owner.Clan;
			Kingdom kingdom = ((clan != null) ? clan.Kingdom : null);
			if (kingdom != null)
			{
				if (kingdom.HasPolicy(NavalPolicies.RoyalNavyPrerogative) && (workshop.WorkshopType.StringId == "smithy" || workshop.WorkshopType.StringId == "wood_WorkshopType"))
				{
					effectiveConversionSpeedOfProduction.AddFactor(-0.05f, NavalPolicies.RoyalNavyPrerogative.Name);
				}
				if (kingdom.HasPolicy(NavalPolicies.MaritimeWealEdict) && workshop.Settlement.HasPort)
				{
					effectiveConversionSpeedOfProduction.AddFactor(0.25f, NavalPolicies.MaritimeWealEdict.Name);
				}
			}
			return effectiveConversionSpeedOfProduction;
		}

		// Token: 0x0600154E RID: 5454 RVA: 0x00095A10 File Offset: 0x00093C10
		public override int GetConvertProductionCost(WorkshopType workshopType)
		{
			return base.BaseModel.GetConvertProductionCost(workshopType);
		}

		// Token: 0x0600154F RID: 5455 RVA: 0x00095A1E File Offset: 0x00093C1E
		public override bool CanPlayerSellWorkshop(Workshop workshop, out TextObject explanation)
		{
			return base.BaseModel.CanPlayerSellWorkshop(workshop, ref explanation);
		}

		// Token: 0x06001550 RID: 5456 RVA: 0x00095A2D File Offset: 0x00093C2D
		public override float GetTradeXpPerWarehouseProduction(EquipmentElement production)
		{
			return base.BaseModel.GetTradeXpPerWarehouseProduction(production);
		}
	}
}
