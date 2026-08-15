using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.Core;

namespace AutoResolveRebalanced
{
	// Token: 0x0200000F RID: 15
	public class SimulateModel
	{
		// Token: 0x0600008A RID: 138 RVA: 0x0000330C File Offset: 0x0000150C
		public static float GetArmorInRandomPart(CharacterObject troop)
		{
			Settings settings = new Settings();
			Equipment equipment = troop.Equipment;
			float num;
			switch (new Random().Next(1, 6))
			{
			case 1:
				num = equipment.GetHeadArmorSum() / 2f;
				break;
			case 2:
				num = equipment.GetArmArmorSum();
				break;
			case 3:
				num = equipment.GetLegArmorSum();
				break;
			default:
				num = equipment.GetHumanBodyArmorSum();
				break;
			}
			if (settings.shieldEnabled)
			{
				for (EquipmentIndex equipmentIndex = 0; equipmentIndex < 5; equipmentIndex++)
				{
					EquipmentElement equipmentElement = equipment[equipmentIndex];
					if (!equipmentElement.IsEmpty && equipmentElement.Item.PrimaryWeapon.IsShield)
					{
						num *= 1f + settings.shieldMultiplierPct;
						break;
					}
				}
			}
			return num;
		}

		// Token: 0x0600008B RID: 139 RVA: 0x000033C8 File Offset: 0x000015C8
		public static float GetWeaponBonus(CharacterObject strikerTroop, CharacterObject struckTroop, MapEvent battle)
		{
			Settings settings = new Settings();
			float num = 1f;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			Equipment equipment = strikerTroop.Equipment;
			for (EquipmentIndex equipmentIndex = 0; equipmentIndex < 5; equipmentIndex++)
			{
				EquipmentElement equipmentElement = equipment[equipmentIndex];
				if (!equipmentElement.IsEmpty)
				{
					if (equipmentElement.Item.RelevantSkill == DefaultSkills.TwoHanded)
					{
						flag = true;
					}
					if (equipmentElement.Item.RelevantSkill == DefaultSkills.Polearm)
					{
						flag3 = true;
					}
					if (equipmentElement.Item.RelevantSkill == DefaultSkills.Bow || equipmentElement.Item.RelevantSkill == DefaultSkills.Crossbow)
					{
						flag2 = true;
						flag = false;
						flag3 = false;
						break;
					}
				}
			}
			if (flag2)
			{
				equipment = struckTroop.Equipment;
				bool flag4 = false;
				for (EquipmentIndex equipmentIndex2 = 0; equipmentIndex2 < 5; equipmentIndex2++)
				{
					EquipmentElement equipmentElement2 = equipment[equipmentIndex2];
					if (!equipmentElement2.IsEmpty && equipmentElement2.Item.PrimaryWeapon.IsShield)
					{
						flag4 = true;
						break;
					}
				}
				if (!flag4)
				{
					num = 1f + settings.rangedBonusPct;
				}
			}
			else if (flag3)
			{
				if (struckTroop.IsMounted && battle.EventType != 5)
				{
					num = 1f + settings.polearmBonusPct;
				}
			}
			else if (flag && (!struckTroop.IsMounted || battle.EventType == 5))
			{
				num = 1f + settings.twoHandedBonusPct;
			}
			return num;
		}
	}
}
