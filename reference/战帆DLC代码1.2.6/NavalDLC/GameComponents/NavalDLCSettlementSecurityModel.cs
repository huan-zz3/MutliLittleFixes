using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.LinQuick;

namespace NavalDLC.GameComponents
{
	// Token: 0x02000131 RID: 305
	public class NavalDLCSettlementSecurityModel : SettlementSecurityModel
	{
		// Token: 0x1700037E RID: 894
		// (get) Token: 0x060014DB RID: 5339 RVA: 0x000930BD File Offset: 0x000912BD
		public override int MaximumSecurityInSettlement
		{
			get
			{
				return base.BaseModel.MaximumSecurityInSettlement;
			}
		}

		// Token: 0x1700037F RID: 895
		// (get) Token: 0x060014DC RID: 5340 RVA: 0x000930CA File Offset: 0x000912CA
		public override int SecurityDriftMedium
		{
			get
			{
				return base.BaseModel.SecurityDriftMedium;
			}
		}

		// Token: 0x17000380 RID: 896
		// (get) Token: 0x060014DD RID: 5341 RVA: 0x000930D7 File Offset: 0x000912D7
		public override float MapEventSecurityEffectRadius
		{
			get
			{
				return base.BaseModel.MapEventSecurityEffectRadius;
			}
		}

		// Token: 0x17000381 RID: 897
		// (get) Token: 0x060014DE RID: 5342 RVA: 0x000930E4 File Offset: 0x000912E4
		public override float HideoutClearedSecurityEffectRadius
		{
			get
			{
				return base.BaseModel.HideoutClearedSecurityEffectRadius;
			}
		}

		// Token: 0x17000382 RID: 898
		// (get) Token: 0x060014DF RID: 5343 RVA: 0x000930F1 File Offset: 0x000912F1
		public override int HideoutClearedSecurityGain
		{
			get
			{
				return base.BaseModel.HideoutClearedSecurityGain;
			}
		}

		// Token: 0x17000383 RID: 899
		// (get) Token: 0x060014E0 RID: 5344 RVA: 0x000930FE File Offset: 0x000912FE
		public override int ThresholdForTaxCorruption
		{
			get
			{
				return base.BaseModel.ThresholdForTaxCorruption;
			}
		}

		// Token: 0x17000384 RID: 900
		// (get) Token: 0x060014E1 RID: 5345 RVA: 0x0009310B File Offset: 0x0009130B
		public override int ThresholdForHigherTaxCorruption
		{
			get
			{
				return base.BaseModel.ThresholdForHigherTaxCorruption;
			}
		}

		// Token: 0x17000385 RID: 901
		// (get) Token: 0x060014E2 RID: 5346 RVA: 0x00093118 File Offset: 0x00091318
		public override int ThresholdForTaxBoost
		{
			get
			{
				return base.BaseModel.ThresholdForTaxBoost;
			}
		}

		// Token: 0x17000386 RID: 902
		// (get) Token: 0x060014E3 RID: 5347 RVA: 0x00093125 File Offset: 0x00091325
		public override int SettlementTaxBoostPercentage
		{
			get
			{
				return base.BaseModel.SettlementTaxBoostPercentage;
			}
		}

		// Token: 0x17000387 RID: 903
		// (get) Token: 0x060014E4 RID: 5348 RVA: 0x00093132 File Offset: 0x00091332
		public override int SettlementTaxPenaltyPercentage
		{
			get
			{
				return base.BaseModel.SettlementTaxPenaltyPercentage;
			}
		}

		// Token: 0x17000388 RID: 904
		// (get) Token: 0x060014E5 RID: 5349 RVA: 0x0009313F File Offset: 0x0009133F
		public override int ThresholdForNotableRelationBonus
		{
			get
			{
				return base.BaseModel.ThresholdForNotableRelationBonus;
			}
		}

		// Token: 0x17000389 RID: 905
		// (get) Token: 0x060014E6 RID: 5350 RVA: 0x0009314C File Offset: 0x0009134C
		public override int ThresholdForNotableRelationPenalty
		{
			get
			{
				return base.BaseModel.ThresholdForNotableRelationPenalty;
			}
		}

		// Token: 0x1700038A RID: 906
		// (get) Token: 0x060014E7 RID: 5351 RVA: 0x00093159 File Offset: 0x00091359
		public override int DailyNotableRelationBonus
		{
			get
			{
				return base.BaseModel.DailyNotableRelationBonus;
			}
		}

		// Token: 0x1700038B RID: 907
		// (get) Token: 0x060014E8 RID: 5352 RVA: 0x00093166 File Offset: 0x00091366
		public override int DailyNotableRelationPenalty
		{
			get
			{
				return base.BaseModel.DailyNotableRelationPenalty;
			}
		}

		// Token: 0x1700038C RID: 908
		// (get) Token: 0x060014E9 RID: 5353 RVA: 0x00093173 File Offset: 0x00091373
		public override int DailyNotablePowerBonus
		{
			get
			{
				return base.BaseModel.DailyNotablePowerBonus;
			}
		}

		// Token: 0x1700038D RID: 909
		// (get) Token: 0x060014EA RID: 5354 RVA: 0x00093180 File Offset: 0x00091380
		public override int DailyNotablePowerPenalty
		{
			get
			{
				return base.BaseModel.DailyNotablePowerPenalty;
			}
		}

		// Token: 0x060014EB RID: 5355 RVA: 0x00093190 File Offset: 0x00091390
		public override ExplainedNumber CalculateSecurityChange(Town town, bool includeDescriptions = false)
		{
			ExplainedNumber explainedNumber = base.BaseModel.CalculateSecurityChange(town, includeDescriptions);
			Clan ownerClan = town.OwnerClan;
			Kingdom kingdom = ((ownerClan != null) ? ownerClan.Kingdom : null);
			if (kingdom != null && kingdom.HasPolicy(NavalPolicies.RaidersSpoils))
			{
				explainedNumber.Add((float)(-(float)LinQuick.CountQ<MobileParty>(town.Settlement.Parties, (MobileParty x) => x.IsLordParty)), NavalPolicies.RaidersSpoils.Name, null);
			}
			return explainedNumber;
		}

		// Token: 0x060014EC RID: 5356 RVA: 0x00093212 File Offset: 0x00091412
		public override float GetNearbyBanditPartyDefeatedSecurityEffect(Town town, float sumOfAttackedPartyStrengths)
		{
			return base.BaseModel.GetNearbyBanditPartyDefeatedSecurityEffect(town, sumOfAttackedPartyStrengths);
		}

		// Token: 0x060014ED RID: 5357 RVA: 0x00093221 File Offset: 0x00091421
		public override float GetLootedNearbyPartySecurityEffect(Town town, float sumOfAttackedPartyStrengths)
		{
			return base.BaseModel.GetLootedNearbyPartySecurityEffect(town, sumOfAttackedPartyStrengths);
		}

		// Token: 0x060014EE RID: 5358 RVA: 0x00093230 File Offset: 0x00091430
		public override void CalculateGoldGainDueToHighSecurity(Town town, ref ExplainedNumber explainedNumber)
		{
			base.BaseModel.CalculateGoldGainDueToHighSecurity(town, ref explainedNumber);
		}

		// Token: 0x060014EF RID: 5359 RVA: 0x0009323F File Offset: 0x0009143F
		public override void CalculateGoldCutDueToLowSecurity(Town town, ref ExplainedNumber explainedNumber)
		{
			base.BaseModel.CalculateGoldCutDueToLowSecurity(town, ref explainedNumber);
		}
	}
}
