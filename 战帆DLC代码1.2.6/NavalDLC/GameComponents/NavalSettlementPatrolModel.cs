using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace NavalDLC.GameComponents
{
	// Token: 0x02000142 RID: 322
	public class NavalSettlementPatrolModel : SettlementPatrolModel
	{
		// Token: 0x06001568 RID: 5480 RVA: 0x000962B0 File Offset: 0x000944B0
		public override bool CanSettlementHavePatrolParties(Settlement settlement, bool naval)
		{
			if (naval)
			{
				return settlement.OwnerClan != null && !settlement.OwnerClan.IsRebelClan && settlement.IsTown && settlement.HasPort && settlement.OwnerClan.Kingdom != null && this.HasCoastalEdict(settlement.OwnerClan.Kingdom);
			}
			return base.BaseModel.CanSettlementHavePatrolParties(settlement, naval);
		}

		// Token: 0x06001569 RID: 5481 RVA: 0x00096313 File Offset: 0x00094513
		public override PartyTemplateObject GetPartyTemplateForPatrolParty(Settlement settlement, bool naval)
		{
			if (naval)
			{
				return settlement.OwnerClan.Culture.SettlementPatrolPartyTemplateNaval;
			}
			return base.BaseModel.GetPartyTemplateForPatrolParty(settlement, naval);
		}

		// Token: 0x0600156A RID: 5482 RVA: 0x00096338 File Offset: 0x00094538
		public override CampaignTime GetPatrolPartySpawnDuration(Settlement settlement, bool naval)
		{
			if (naval)
			{
				return CampaignTime.Days(RandomOwnerExtensions.RandomFloatWithSeed(settlement, (uint)CampaignTime.Now.ElapsedMillisecondsUntilNow, 5f, 7f));
			}
			return base.BaseModel.GetPatrolPartySpawnDuration(settlement, naval);
		}

		// Token: 0x0600156B RID: 5483 RVA: 0x00096379 File Offset: 0x00094579
		private bool HasCoastalEdict(Kingdom kingdom)
		{
			return kingdom.HasPolicy(NavalPolicies.CoastalGuardEdict);
		}
	}
}
