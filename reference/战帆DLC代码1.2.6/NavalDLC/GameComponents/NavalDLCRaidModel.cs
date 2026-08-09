using System;
using Helpers;
using NavalDLC.CharacterDevelopment;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace NavalDLC.GameComponents
{
	// Token: 0x0200012C RID: 300
	public class NavalDLCRaidModel : RaidModel
	{
		// Token: 0x1700037D RID: 893
		// (get) Token: 0x060014C0 RID: 5312 RVA: 0x00092B5E File Offset: 0x00090D5E
		public override int GoldRewardForEachLostHearth
		{
			get
			{
				return base.BaseModel.GoldRewardForEachLostHearth;
			}
		}

		// Token: 0x060014C1 RID: 5313 RVA: 0x00092B6C File Offset: 0x00090D6C
		public override ExplainedNumber CalculateHitDamage(MapEventSide attackerSide, float settlementHitPoints)
		{
			ExplainedNumber explainedNumber = base.BaseModel.CalculateHitDamage(attackerSide, settlementHitPoints);
			int num = 0;
			foreach (MapEventParty mapEventParty in attackerSide.Parties)
			{
				num += mapEventParty.Party.MemberRoster.TotalManCount;
			}
			if (num > 0)
			{
				foreach (MapEventParty mapEventParty2 in attackerSide.Parties)
				{
					PartyBase party = mapEventParty2.Party;
					int totalManCount = party.MemberRoster.TotalManCount;
					if (totalManCount > 0)
					{
						float num2 = (float)totalManCount / (float)num;
						if (PartyBaseHelper.HasFeat(party, NavalCulturalFeats.NordHostileActionSpeedFeat))
						{
							explainedNumber.AddFactor(NavalCulturalFeats.NordHostileActionSpeedFeat.EffectBonus * num2, null);
						}
						if (party.MobileParty != null && party.MobileParty.IsCurrentlyAtSea)
						{
							ExplainedNumber explainedNumber2;
							explainedNumber2..ctor(0f, false, null);
							PerkHelper.AddPerkBonusForParty(NavalPerks.Mariner.Forceful, party.MobileParty, false, ref explainedNumber2, false);
							if (explainedNumber2.ResultNumber != 0f)
							{
								explainedNumber.AddFactor(explainedNumber2.ResultNumber * num2, null);
							}
						}
					}
				}
			}
			return explainedNumber;
		}

		// Token: 0x060014C2 RID: 5314 RVA: 0x00092CC8 File Offset: 0x00090EC8
		public override ExplainedNumber GetRaidLootMultiplier(PartyBase receivingParty)
		{
			ExplainedNumber raidLootMultiplier = base.BaseModel.GetRaidLootMultiplier(receivingParty);
			if (receivingParty != null && receivingParty.IsMobile && receivingParty.MobileParty.IsCurrentlyAtSea)
			{
				PerkHelper.AddPerkBonusForParty(NavalPerks.Mariner.BruteForce, receivingParty.MobileParty, false, ref raidLootMultiplier, false);
			}
			return raidLootMultiplier;
		}

		// Token: 0x060014C3 RID: 5315 RVA: 0x00092D0F File Offset: 0x00090F0F
		public override MBReadOnlyList<ValueTuple<ItemObject, float>> GetCommonLootItemScores()
		{
			return base.BaseModel.GetCommonLootItemScores();
		}
	}
}
