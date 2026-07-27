using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Localization;

namespace NavalDLC.GameComponents
{
	// Token: 0x02000145 RID: 325
	public class NavalTradeAgreementModel : TradeAgreementModel
	{
		// Token: 0x06001583 RID: 5507 RVA: 0x00096A10 File Offset: 0x00094C10
		public override bool CanMakeTradeAgreement(Kingdom kingdom, Kingdom other, bool checkOtherSideTradeSupport, out TextObject reason, bool includeReason = false)
		{
			return base.BaseModel.CanMakeTradeAgreement(kingdom, other, checkOtherSideTradeSupport, ref reason, includeReason);
		}

		// Token: 0x06001584 RID: 5508 RVA: 0x00096A24 File Offset: 0x00094C24
		public override int GetInfluenceCostOfProposingTradeAgreement(Clan clan)
		{
			return base.BaseModel.GetInfluenceCostOfProposingTradeAgreement(clan);
		}

		// Token: 0x06001585 RID: 5509 RVA: 0x00096A32 File Offset: 0x00094C32
		public override int GetMaximumTradeAgreementCount(Kingdom kingdom)
		{
			return base.BaseModel.GetMaximumTradeAgreementCount(kingdom);
		}

		// Token: 0x06001586 RID: 5510 RVA: 0x00096A40 File Offset: 0x00094C40
		public override int GetProfitPerCaravanVisit(MobileParty mobileParty)
		{
			if (mobileParty.HasNavalNavigationCapability)
			{
				return 1000;
			}
			return base.BaseModel.GetProfitPerCaravanVisit(mobileParty);
		}

		// Token: 0x06001587 RID: 5511 RVA: 0x00096A5C File Offset: 0x00094C5C
		public override float GetScoreOfStartingTradeAgreement(Kingdom kingdom, Kingdom targetKingdom, Clan clan, out TextObject explanation, bool includeExplanation = false)
		{
			return base.BaseModel.GetScoreOfStartingTradeAgreement(kingdom, targetKingdom, clan, ref explanation, includeExplanation);
		}

		// Token: 0x06001588 RID: 5512 RVA: 0x00096A70 File Offset: 0x00094C70
		public override CampaignTime GetTradeAgreementDurationInYears(Kingdom iniatatingKingdom, Kingdom otherKingdom)
		{
			return base.BaseModel.GetTradeAgreementDurationInYears(iniatatingKingdom, otherKingdom);
		}
	}
}
