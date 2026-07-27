using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace NavalDLC.GameComponents
{
	// Token: 0x0200012E RID: 302
	public class NavalDLCSettlementGarrisonModel : SettlementGarrisonModel
	{
		// Token: 0x060014CD RID: 5325 RVA: 0x00092E94 File Offset: 0x00091094
		public override int GetMaximumDailyAutoRecruitmentCount(Town town)
		{
			return base.BaseModel.GetMaximumDailyAutoRecruitmentCount(town);
		}

		// Token: 0x060014CE RID: 5326 RVA: 0x00092EA2 File Offset: 0x000910A2
		public override ExplainedNumber CalculateBaseGarrisonChange(Settlement settlement, bool includeDescriptions = false)
		{
			return base.BaseModel.CalculateBaseGarrisonChange(settlement, includeDescriptions);
		}

		// Token: 0x060014CF RID: 5327 RVA: 0x00092EB1 File Offset: 0x000910B1
		public override int FindNumberOfTroopsToTakeFromGarrison(MobileParty mobileParty, Settlement settlement, float idealGarrisonStrengthPerWalledCenter = 0f)
		{
			return base.BaseModel.FindNumberOfTroopsToTakeFromGarrison(mobileParty, settlement, idealGarrisonStrengthPerWalledCenter);
		}

		// Token: 0x060014D0 RID: 5328 RVA: 0x00092EC1 File Offset: 0x000910C1
		public override int FindNumberOfTroopsToLeaveToGarrison(MobileParty mobileParty, Settlement settlement)
		{
			return base.BaseModel.FindNumberOfTroopsToLeaveToGarrison(mobileParty, settlement);
		}

		// Token: 0x060014D1 RID: 5329 RVA: 0x00092ED0 File Offset: 0x000910D0
		public override float GetMaximumDailyRepairAmount(Settlement settlement)
		{
			return base.BaseModel.GetMaximumDailyRepairAmount(settlement);
		}
	}
}
