using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;

namespace AutoResolveRebalanced
{
	// Token: 0x02000005 RID: 5
	[HarmonyPatch(typeof(MapEvent), "GetSimulatedDamage")]
	internal class Patch_GetSimulatedDamage
	{
		// Token: 0x06000008 RID: 8 RVA: 0x00002418 File Offset: 0x00000618
		public static void Postfix(ref int __result, MapEvent __instance, CharacterObject strikerTroop, CharacterObject strikedTroop)
		{
			Settings settings = new Settings();
			try
			{
				if (settings.aiEnabled || __instance.IsPlayerSimulation)
				{
					float num = 0f;
					float num2 = 1f;
					if (settings.armorEnabled)
					{
						num = SimulateModel.GetArmorInRandomPart(strikedTroop);
					}
					if (settings.weaponEnabled)
					{
						num2 = SimulateModel.GetWeaponBonus(strikerTroop, strikedTroop, __instance);
					}
					float num3 = (float)__result + (float)__result * (num2 - 1f) - num * settings.defModifierPct;
					if (num3 < 1f)
					{
						num3 = 1f;
					}
					__result = (int)num3;
				}
			}
			catch (Exception ex)
			{
				Debugger.Message(ex.ToString(), Debugger.Type.Exception, null, false);
			}
		}
	}
}
