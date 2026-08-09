using System;
using Helpers;
using NavalDLC.CharacterDevelopment;
using NavalDLC.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Localization;

namespace NavalDLC.GameComponents
{
	// Token: 0x0200011D RID: 285
	public class NavalDLCMapVisibilityModel : MapVisibilityModel
	{
		// Token: 0x0600143B RID: 5179 RVA: 0x00090BAF File Offset: 0x0008EDAF
		public override float MaximumSeeingRange()
		{
			return base.BaseModel.MaximumSeeingRange();
		}

		// Token: 0x0600143C RID: 5180 RVA: 0x00090BBC File Offset: 0x0008EDBC
		public override float GetPartySeeingRangeBase(MobileParty party)
		{
			float num = base.BaseModel.GetPartySeeingRangeBase(party);
			if (party.IsCurrentlyAtSea)
			{
				if (party.IsInRaftState)
				{
					num *= 0.5f;
				}
				if (Campaign.Current.IsNight && party.HasPerk(NavalPerks.Shipmaster.NightRaider, false))
				{
					num += 3f;
				}
			}
			return num;
		}

		// Token: 0x0600143D RID: 5181 RVA: 0x00090C14 File Offset: 0x0008EE14
		public override ExplainedNumber GetPartySpottingRange(MobileParty party, bool includeDescriptions = false)
		{
			ExplainedNumber partySpottingRange = base.BaseModel.GetPartySpottingRange(party, includeDescriptions);
			if (party.IsCurrentlyAtSea)
			{
				PerkHelper.AddPerkBonusForParty(NavalPerks.Shipmaster.RavenEye, party, true, ref partySpottingRange, false);
				partySpottingRange.AddFactor(0.3f, new TextObject("{=B0aCb3Je}At Sea", null));
				foreach (Storm storm in NavalDLCManager.Instance.StormManager.SpawnedStorms)
				{
					if (storm.IsActive && storm.CurrentPosition.DistanceSquared(party.Position.ToVec2()) < storm.EffectRadius * storm.EffectRadius)
					{
						partySpottingRange.AddFactor(-0.4f, new TextObject("{=M6V6eCTg}Storm", null));
						break;
					}
				}
			}
			return partySpottingRange;
		}

		// Token: 0x0600143E RID: 5182 RVA: 0x00090CF8 File Offset: 0x0008EEF8
		public override float GetPartySpottingRatioForMainPartySeeingRange(MobileParty party)
		{
			return base.BaseModel.GetPartySpottingRatioForMainPartySeeingRange(party);
		}

		// Token: 0x0600143F RID: 5183 RVA: 0x00090D06 File Offset: 0x0008EF06
		public override float GetHideoutSpottingDistance()
		{
			return base.BaseModel.GetHideoutSpottingDistance();
		}

		// Token: 0x04000AD1 RID: 2769
		private const float SeaSpottingRangeBonus = 0.3f;

		// Token: 0x04000AD2 RID: 2770
		private const float StormSpottingRangePenalty = -0.4f;
	}
}
