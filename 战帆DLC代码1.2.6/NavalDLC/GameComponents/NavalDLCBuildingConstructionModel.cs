using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Settlements;

namespace NavalDLC.GameComponents
{
	// Token: 0x0200010B RID: 267
	public class NavalDLCBuildingConstructionModel : BuildingConstructionModel
	{
		// Token: 0x17000352 RID: 850
		// (get) Token: 0x0600137A RID: 4986 RVA: 0x0008D45E File Offset: 0x0008B65E
		public override int TownBoostCost
		{
			get
			{
				return base.BaseModel.TownBoostCost;
			}
		}

		// Token: 0x17000353 RID: 851
		// (get) Token: 0x0600137B RID: 4987 RVA: 0x0008D46B File Offset: 0x0008B66B
		public override int TownBoostBonus
		{
			get
			{
				return base.BaseModel.TownBoostBonus;
			}
		}

		// Token: 0x17000354 RID: 852
		// (get) Token: 0x0600137C RID: 4988 RVA: 0x0008D478 File Offset: 0x0008B678
		public override int CastleBoostCost
		{
			get
			{
				return base.BaseModel.CastleBoostCost;
			}
		}

		// Token: 0x17000355 RID: 853
		// (get) Token: 0x0600137D RID: 4989 RVA: 0x0008D485 File Offset: 0x0008B685
		public override int CastleBoostBonus
		{
			get
			{
				return base.BaseModel.CastleBoostBonus;
			}
		}

		// Token: 0x0600137E RID: 4990 RVA: 0x0008D494 File Offset: 0x0008B694
		public override ExplainedNumber CalculateDailyConstructionPower(Town town, bool includeDescriptions = false)
		{
			ExplainedNumber explainedNumber = base.BaseModel.CalculateDailyConstructionPower(town, includeDescriptions);
			Clan ownerClan = town.OwnerClan;
			Kingdom kingdom = ((ownerClan != null) ? ownerClan.Kingdom : null);
			if (kingdom != null && kingdom.HasPolicy(NavalPolicies.MaritimeWealEdict) && !town.Settlement.HasPort)
			{
				explainedNumber.AddFactor(0.2f, NavalPolicies.MaritimeWealEdict.Name);
			}
			return explainedNumber;
		}

		// Token: 0x0600137F RID: 4991 RVA: 0x0008D4F6 File Offset: 0x0008B6F6
		public override int CalculateDailyConstructionPowerWithoutBoost(Town town)
		{
			return base.BaseModel.CalculateDailyConstructionPowerWithoutBoost(town);
		}

		// Token: 0x06001380 RID: 4992 RVA: 0x0008D504 File Offset: 0x0008B704
		public override int GetBoostCost(Town town)
		{
			return base.BaseModel.GetBoostCost(town);
		}

		// Token: 0x06001381 RID: 4993 RVA: 0x0008D512 File Offset: 0x0008B712
		public override int GetBoostAmount(Town town)
		{
			return base.BaseModel.GetBoostAmount(town);
		}
	}
}
