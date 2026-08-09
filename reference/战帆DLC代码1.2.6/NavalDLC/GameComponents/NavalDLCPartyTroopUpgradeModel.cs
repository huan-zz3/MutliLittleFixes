using System;
using Helpers;
using NavalDLC.CharacterDevelopment;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;

namespace NavalDLC.GameComponents
{
	// Token: 0x02000129 RID: 297
	internal class NavalDLCPartyTroopUpgradeModel : PartyTroopUpgradeModel
	{
		// Token: 0x060014AB RID: 5291 RVA: 0x00092817 File Offset: 0x00090A17
		public override bool CanPartyUpgradeTroopToTarget(PartyBase party, CharacterObject character, CharacterObject target)
		{
			return base.BaseModel.CanPartyUpgradeTroopToTarget(party, character, target);
		}

		// Token: 0x060014AC RID: 5292 RVA: 0x00092827 File Offset: 0x00090A27
		public override bool IsTroopUpgradeable(PartyBase party, CharacterObject character)
		{
			return base.BaseModel.IsTroopUpgradeable(party, character);
		}

		// Token: 0x060014AD RID: 5293 RVA: 0x00092836 File Offset: 0x00090A36
		public override bool DoesPartyHaveRequiredItemsForUpgrade(PartyBase party, CharacterObject upgradeTarget)
		{
			return base.BaseModel.DoesPartyHaveRequiredItemsForUpgrade(party, upgradeTarget);
		}

		// Token: 0x060014AE RID: 5294 RVA: 0x00092845 File Offset: 0x00090A45
		public override bool DoesPartyHaveRequiredPerksForUpgrade(PartyBase party, CharacterObject character, CharacterObject upgradeTarget, out PerkObject requiredPerk)
		{
			return base.BaseModel.DoesPartyHaveRequiredPerksForUpgrade(party, character, upgradeTarget, ref requiredPerk);
		}

		// Token: 0x060014AF RID: 5295 RVA: 0x00092858 File Offset: 0x00090A58
		public override ExplainedNumber GetGoldCostForUpgrade(PartyBase party, CharacterObject characterObject, CharacterObject upgradeTarget)
		{
			ExplainedNumber goldCostForUpgrade = base.BaseModel.GetGoldCostForUpgrade(party, characterObject, upgradeTarget);
			if (party.IsMobile && characterObject.IsMariner && !characterObject.IsHero)
			{
				PerkHelper.AddPerkBonusForParty(NavalPerks.Boatswain.EfficientCaptain, party.MobileParty, true, ref goldCostForUpgrade, false);
			}
			return goldCostForUpgrade;
		}

		// Token: 0x060014B0 RID: 5296 RVA: 0x000928A1 File Offset: 0x00090AA1
		public override int GetXpCostForUpgrade(PartyBase party, CharacterObject characterObject, CharacterObject upgradeTarget)
		{
			return base.BaseModel.GetXpCostForUpgrade(party, characterObject, upgradeTarget);
		}

		// Token: 0x060014B1 RID: 5297 RVA: 0x000928B1 File Offset: 0x00090AB1
		public override int GetSkillXpFromUpgradingTroops(PartyBase party, CharacterObject troop, int numberOfTroops)
		{
			return base.BaseModel.GetSkillXpFromUpgradingTroops(party, troop, numberOfTroops);
		}

		// Token: 0x060014B2 RID: 5298 RVA: 0x000928C1 File Offset: 0x00090AC1
		public override float GetUpgradeChanceForTroopUpgrade(PartyBase party, CharacterObject troop, int upgradeTargetIndex)
		{
			return base.BaseModel.GetUpgradeChanceForTroopUpgrade(party, troop, upgradeTargetIndex);
		}
	}
}
