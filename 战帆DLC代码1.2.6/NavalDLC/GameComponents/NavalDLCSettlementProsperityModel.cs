using System;
using Helpers;
using NavalDLC.CharacterDevelopment;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Settlements;

namespace NavalDLC.GameComponents
{
	// Token: 0x02000130 RID: 304
	public class NavalDLCSettlementProsperityModel : SettlementProsperityModel
	{
		// Token: 0x060014D8 RID: 5336 RVA: 0x00093057 File Offset: 0x00091257
		public override ExplainedNumber CalculateProsperityChange(Town fortification, bool includeDescriptions = false)
		{
			return base.BaseModel.CalculateProsperityChange(fortification, includeDescriptions);
		}

		// Token: 0x060014D9 RID: 5337 RVA: 0x00093068 File Offset: 0x00091268
		public override ExplainedNumber CalculateHearthChange(Village village, bool includeDescriptions = false)
		{
			ExplainedNumber explainedNumber = base.BaseModel.CalculateHearthChange(village, includeDescriptions);
			if (village.Bound.HasPort && village.Bound.IsFortification)
			{
				PerkHelper.AddPerkBonusForTown(NavalPerks.Shipmaster.FairWinds, village.Bound.Town, ref explainedNumber);
			}
			return explainedNumber;
		}
	}
}
