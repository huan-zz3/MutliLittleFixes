using System;
using Helpers;
using NavalDLC.CharacterDevelopment;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace NavalDLC.GameComponents
{
	// Token: 0x0200011B RID: 283
	public class NavalDLCInventoryCapacityModel : InventoryCapacityModel
	{
		// Token: 0x06001422 RID: 5154 RVA: 0x0008FE74 File Offset: 0x0008E074
		public override ExplainedNumber CalculateInventoryCapacity(MobileParty mobileParty, bool isCurrentlyAtSea, bool includeDescriptions = false, int additionalManOnFoot = 0, int additionalSpareMounts = 0, int additionalPackAnimals = 0, bool includeFollowers = false)
		{
			ExplainedNumber explainedNumber = base.BaseModel.CalculateInventoryCapacity(mobileParty, isCurrentlyAtSea, includeDescriptions, additionalManOnFoot, additionalSpareMounts, additionalPackAnimals, includeFollowers);
			if (isCurrentlyAtSea)
			{
				float num = 0f;
				foreach (Ship ship in mobileParty.Ships)
				{
					num += ship.InventoryCapacity;
				}
				foreach (MobileParty mobileParty2 in mobileParty.AttachedParties)
				{
					foreach (Ship ship2 in mobileParty2.Ships)
					{
						num += ship2.InventoryCapacity;
					}
				}
				explainedNumber.Add(num, NavalDLCInventoryCapacityModel._textBaseNavalCapacity, null);
			}
			return explainedNumber;
		}

		// Token: 0x06001423 RID: 5155 RVA: 0x0008FF7C File Offset: 0x0008E17C
		public override int GetItemAverageWeight()
		{
			return base.BaseModel.GetItemAverageWeight();
		}

		// Token: 0x06001424 RID: 5156 RVA: 0x0008FF8C File Offset: 0x0008E18C
		public override ExplainedNumber CalculateTotalWeightCarried(MobileParty mobileParty, bool isCurrentlyAtSea, bool includeDescriptions = false)
		{
			ExplainedNumber explainedNumber = base.BaseModel.CalculateTotalWeightCarried(mobileParty, isCurrentlyAtSea, includeDescriptions);
			if (isCurrentlyAtSea)
			{
				foreach (TroopRosterElement troopRosterElement in mobileParty.MemberRoster.GetTroopRoster())
				{
					float num = 0f;
					if (!troopRosterElement.Character.IsHero && !troopRosterElement.Character.Equipment.Horse.IsEmpty)
					{
						num += 50f * (float)troopRosterElement.Number;
					}
					explainedNumber.Add(num, NavalDLCInventoryCapacityModel._textTroopMounts, null);
				}
			}
			return explainedNumber;
		}

		// Token: 0x06001425 RID: 5157 RVA: 0x00090040 File Offset: 0x0008E240
		public override float GetItemEffectiveWeight(EquipmentElement equipmentElement, MobileParty mobileParty, bool isCurrentlyAtSea, out TextObject description)
		{
			if (isCurrentlyAtSea)
			{
				ItemObject item = equipmentElement.Item;
				ExplainedNumber explainedNumber;
				if (item.HasHorseComponent)
				{
					if (item.HorseComponent.IsMount)
					{
						explainedNumber..ctor(50f, false, null);
						description = NavalDLCInventoryCapacityModel._textMountsAndPackAnimals;
						PerkHelper.AddPerkBonusForParty(NavalPerks.Boatswain.NavalHorde, mobileParty, false, ref explainedNumber, false);
					}
					else if (item.HorseComponent.IsPackAnimal)
					{
						explainedNumber..ctor(30f, false, null);
						description = NavalDLCInventoryCapacityModel._textMountsAndPackAnimals;
						PerkHelper.AddPerkBonusForParty(NavalPerks.Boatswain.Optimization, mobileParty, false, ref explainedNumber, false);
					}
					else if (item.HorseComponent.IsLiveStock)
					{
						explainedNumber..ctor(20f, false, null);
						description = NavalDLCInventoryCapacityModel._textLiveStocksAnimals;
						PerkHelper.AddPerkBonusForParty(NavalPerks.Boatswain.Optimization, mobileParty, false, ref explainedNumber, false);
					}
					else
					{
						explainedNumber..ctor(equipmentElement.GetEquipmentElementWeight(), false, null);
						description = NavalDLCInventoryCapacityModel._textItems;
					}
				}
				else
				{
					explainedNumber..ctor(equipmentElement.GetEquipmentElementWeight(), false, null);
					description = NavalDLCInventoryCapacityModel._textItems;
				}
				if (item.IsTradeGood)
				{
					PerkHelper.AddPerkBonusForParty(NavalPerks.Boatswain.GildedPurse, mobileParty, false, ref explainedNumber, false);
				}
				return explainedNumber.ResultNumber;
			}
			return base.BaseModel.GetItemEffectiveWeight(equipmentElement, mobileParty, isCurrentlyAtSea, ref description);
		}

		// Token: 0x04000AC8 RID: 2760
		private static readonly TextObject _textTroopMounts = new TextObject("{=GIlU4NXm}Troops' Mounts", null);

		// Token: 0x04000AC9 RID: 2761
		private static readonly TextObject _textMountsAndPackAnimals = new TextObject("{=Sb1MKbvP}Mounts and Pack Animals", null);

		// Token: 0x04000ACA RID: 2762
		private static readonly TextObject _textLiveStocksAnimals = new TextObject("{=KxUgSAKi}Live Stock Animals", null);

		// Token: 0x04000ACB RID: 2763
		private static readonly TextObject _textItems = new TextObject("{=U7er3V9s}Items", null);

		// Token: 0x04000ACC RID: 2764
		private static readonly TextObject _textBaseNavalCapacity = new TextObject("{=7Q8ufo5X}Ships", null);

		// Token: 0x04000ACD RID: 2765
		private const float CustomMountWeight = 50f;

		// Token: 0x04000ACE RID: 2766
		private const float CustomPackAnimalWeight = 30f;

		// Token: 0x04000ACF RID: 2767
		private const float CustomLiveStockWeight = 20f;
	}
}
