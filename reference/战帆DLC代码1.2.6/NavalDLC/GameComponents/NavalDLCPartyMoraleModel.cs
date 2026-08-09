using System;
using Helpers;
using NavalDLC.CharacterDevelopment;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;

namespace NavalDLC.GameComponents
{
	// Token: 0x02000124 RID: 292
	public class NavalDLCPartyMoraleModel : PartyMoraleModel
	{
		// Token: 0x17000378 RID: 888
		// (get) Token: 0x06001481 RID: 5249 RVA: 0x00091B17 File Offset: 0x0008FD17
		public override float HighMoraleValue
		{
			get
			{
				return base.BaseModel.HighMoraleValue;
			}
		}

		// Token: 0x06001482 RID: 5250 RVA: 0x00091B24 File Offset: 0x0008FD24
		public override int GetDailyStarvationMoralePenalty(PartyBase party)
		{
			return base.BaseModel.GetDailyStarvationMoralePenalty(party);
		}

		// Token: 0x06001483 RID: 5251 RVA: 0x00091B32 File Offset: 0x0008FD32
		public override int GetDailyNoWageMoralePenalty(MobileParty party)
		{
			return base.BaseModel.GetDailyNoWageMoralePenalty(party);
		}

		// Token: 0x06001484 RID: 5252 RVA: 0x00091B40 File Offset: 0x0008FD40
		public override float GetStandardBaseMorale(PartyBase party)
		{
			return base.BaseModel.GetStandardBaseMorale(party);
		}

		// Token: 0x06001485 RID: 5253 RVA: 0x00091B4E File Offset: 0x0008FD4E
		public override float GetVictoryMoraleChange(PartyBase party)
		{
			return base.BaseModel.GetVictoryMoraleChange(party);
		}

		// Token: 0x06001486 RID: 5254 RVA: 0x00091B5C File Offset: 0x0008FD5C
		public override float GetDefeatMoraleChange(PartyBase party)
		{
			return base.BaseModel.GetDefeatMoraleChange(party);
		}

		// Token: 0x06001487 RID: 5255 RVA: 0x00091B6C File Offset: 0x0008FD6C
		public override ExplainedNumber GetEffectivePartyMorale(MobileParty party, bool includeDescription = false)
		{
			ExplainedNumber effectivePartyMorale = base.BaseModel.GetEffectivePartyMorale(party, includeDescription);
			if (party.Anchor != null && party.CurrentSettlement != null && party.CurrentSettlement.HasPort && party.Anchor.IsAtSettlement(party.CurrentSettlement))
			{
				PerkHelper.AddPerkBonusForParty(NavalPerks.Boatswain.EfficientCaptain, party, false, ref effectivePartyMorale, false);
			}
			return effectivePartyMorale;
		}
	}
}
