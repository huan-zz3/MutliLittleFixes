using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace NavalDLC.GameComponents
{
	// Token: 0x0200010F RID: 271
	public class NavalDLCCaravanModel : CaravanModel
	{
		// Token: 0x17000356 RID: 854
		// (get) Token: 0x0600139B RID: 5019 RVA: 0x0008DE90 File Offset: 0x0008C090
		public override int MaxNumberOfItemsToBuyFromSingleCategory
		{
			get
			{
				return base.BaseModel.MaxNumberOfItemsToBuyFromSingleCategory;
			}
		}

		// Token: 0x0600139C RID: 5020 RVA: 0x0008DE9D File Offset: 0x0008C09D
		public override bool CanHeroCreateCaravan(Hero hero)
		{
			return base.BaseModel.CanHeroCreateCaravan(hero);
		}

		// Token: 0x0600139D RID: 5021 RVA: 0x0008DEAC File Offset: 0x0008C0AC
		public override int GetCaravanFormingCost(bool eliteCaravan, bool navalCaravan)
		{
			if (!navalCaravan)
			{
				return base.BaseModel.GetCaravanFormingCost(eliteCaravan, navalCaravan);
			}
			int num = (eliteCaravan ? 45000 : 30000);
			if (CharacterObject.PlayerCharacter.Culture.HasFeat(DefaultCulturalFeats.AseraiTraderFeat))
			{
				return MathF.Round((float)num * DefaultCulturalFeats.AseraiTraderFeat.EffectBonus);
			}
			return num;
		}

		// Token: 0x0600139E RID: 5022 RVA: 0x0008DF04 File Offset: 0x0008C104
		public override float GetEliteCaravanSpawnChance(Hero hero)
		{
			return base.BaseModel.GetEliteCaravanSpawnChance(hero);
		}

		// Token: 0x0600139F RID: 5023 RVA: 0x0008DF14 File Offset: 0x0008C114
		public override int GetInitialTradeGold(Hero owner, bool navalCaravan, bool largeCaravan)
		{
			if (navalCaravan)
			{
				int num = 30000;
				int num2 = ((owner == Hero.MainHero) ? 5000 : 0);
				if (largeCaravan)
				{
					num = 40000;
				}
				return num + num2;
			}
			return base.BaseModel.GetInitialTradeGold(owner, navalCaravan, largeCaravan);
		}

		// Token: 0x060013A0 RID: 5024 RVA: 0x0008DF56 File Offset: 0x0008C156
		public override int GetMaxGoldToSpendOnOneItemCategory(MobileParty caravan, ItemCategory itemCategory)
		{
			if (caravan.HasNavalNavigationCapability)
			{
				return 3000;
			}
			return base.BaseModel.GetMaxGoldToSpendOnOneItemCategory(caravan, itemCategory);
		}

		// Token: 0x060013A1 RID: 5025 RVA: 0x0008DF73 File Offset: 0x0008C173
		public override int GetPowerChangeAfterCaravanCreation(Hero hero, MobileParty caravanParty)
		{
			return base.BaseModel.GetPowerChangeAfterCaravanCreation(hero, caravanParty);
		}
	}
}
