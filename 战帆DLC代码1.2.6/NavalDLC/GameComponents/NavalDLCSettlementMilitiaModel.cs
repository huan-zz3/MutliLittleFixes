using System;
using Helpers;
using NavalDLC.CharacterDevelopment;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Settlements;

namespace NavalDLC.GameComponents
{
	// Token: 0x0200012F RID: 303
	public class NavalDLCSettlementMilitiaModel : SettlementMilitiaModel
	{
		// Token: 0x060014D3 RID: 5331 RVA: 0x00092EE6 File Offset: 0x000910E6
		public override int MilitiaToSpawnAfterSiege(Town town)
		{
			return base.BaseModel.MilitiaToSpawnAfterSiege(town);
		}

		// Token: 0x060014D4 RID: 5332 RVA: 0x00092EF4 File Offset: 0x000910F4
		public override ExplainedNumber CalculateMilitiaChange(Settlement settlement, bool includeDescriptions = false)
		{
			ExplainedNumber explainedNumber = base.BaseModel.CalculateMilitiaChange(settlement, includeDescriptions);
			if (settlement.IsTown && settlement.HasPort)
			{
				PerkHelper.AddPerkBonusForTown(NavalPerks.Boatswain.AccuracyTraining, settlement.Town, ref explainedNumber);
			}
			else if (settlement.IsVillage)
			{
				Town town = settlement.Village.Bound.Town;
				if (town != null && town.Settlement.HasPort && town.Governor != null && town.Governor.GetPerkValue(NavalPerks.Boatswain.AccuracyTraining))
				{
					explainedNumber.Add(NavalPerks.Boatswain.AccuracyTraining.SecondaryBonus, NavalPerks.Boatswain.AccuracyTraining.SecondaryDescription, null);
				}
			}
			Clan ownerClan = settlement.OwnerClan;
			Kingdom kingdom = ((ownerClan != null) ? ownerClan.Kingdom : null);
			if (kingdom != null && kingdom.HasPolicy(NavalPolicies.BolsterTheFyrd))
			{
				explainedNumber.AddFactor(0.25f, NavalPolicies.BolsterTheFyrd.Name);
			}
			return explainedNumber;
		}

		// Token: 0x060014D5 RID: 5333 RVA: 0x00092FCC File Offset: 0x000911CC
		public override ExplainedNumber CalculateVeteranMilitiaSpawnChance(Settlement settlement)
		{
			ExplainedNumber explainedNumber = base.BaseModel.CalculateVeteranMilitiaSpawnChance(settlement);
			if (settlement.IsTown && settlement.HasPort)
			{
				PerkHelper.AddPerkBonusForTown(NavalPerks.Mariner.NavalFightingTraining, settlement.Town, ref explainedNumber);
			}
			if (settlement.IsVillage && settlement.Village.Bound.HasPort)
			{
				PerkHelper.AddPerkBonusForTown(NavalPerks.Mariner.NavalFightingTraining, settlement.Village.Bound.Town, ref explainedNumber);
			}
			return explainedNumber;
		}

		// Token: 0x060014D6 RID: 5334 RVA: 0x0009303F File Offset: 0x0009123F
		public override void CalculateMilitiaSpawnRate(Settlement settlement, out float meleeTroopRate, out float rangedTroopRate)
		{
			base.BaseModel.CalculateMilitiaSpawnRate(settlement, ref meleeTroopRate, ref rangedTroopRate);
		}
	}
}
