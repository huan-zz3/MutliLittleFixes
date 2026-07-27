using System;
using Helpers;
using NavalDLC.CharacterDevelopment;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;

namespace NavalDLC.GameComponents
{
	// Token: 0x02000123 RID: 291
	public class NavalDLCPartyImpairmentModel : PartyImpairmentModel
	{
		// Token: 0x0600147C RID: 5244 RVA: 0x00091AB4 File Offset: 0x0008FCB4
		public override ExplainedNumber GetDisorganizedStateDuration(MobileParty party)
		{
			ExplainedNumber disorganizedStateDuration = base.BaseModel.GetDisorganizedStateDuration(party);
			if (party.IsCurrentlyAtSea)
			{
				PerkHelper.AddPerkBonusForParty(NavalPerks.Shipmaster.Windborne, party, false, ref disorganizedStateDuration, false);
			}
			return disorganizedStateDuration;
		}

		// Token: 0x0600147D RID: 5245 RVA: 0x00091AE6 File Offset: 0x0008FCE6
		public override float GetVulnerabilityStateDuration(PartyBase party)
		{
			return base.BaseModel.GetVulnerabilityStateDuration(party);
		}

		// Token: 0x0600147E RID: 5246 RVA: 0x00091AF4 File Offset: 0x0008FCF4
		public override float GetSiegeExpectedVulnerabilityTime()
		{
			return base.BaseModel.GetSiegeExpectedVulnerabilityTime();
		}

		// Token: 0x0600147F RID: 5247 RVA: 0x00091B01 File Offset: 0x0008FD01
		public override bool CanGetDisorganized(PartyBase partyBase)
		{
			return base.BaseModel.CanGetDisorganized(partyBase);
		}
	}
}
