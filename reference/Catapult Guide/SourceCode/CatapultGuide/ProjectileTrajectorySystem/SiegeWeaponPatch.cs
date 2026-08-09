using System;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.MountAndBlade;

namespace ProjectileTrajectorySystem
{
	// Token: 0x02000005 RID: 5
	[HarmonyPatch]
	public static class SiegeWeaponPatch
	{
		// Token: 0x06000005 RID: 5 RVA: 0x0000208C File Offset: 0x0000028C
		[NullableContext(1)]
		[HarmonyPatch(typeof(RangedSiegeWeapon), "Shoot")]
		[HarmonyPrefix]
		public static void Prefix_Shoot(RangedSiegeWeapon __instance)
		{
			bool flag = __instance.PilotAgent != null && __instance.PilotAgent.IsMainAgent;
			if (flag)
			{
				SiegeWeaponPatch.IsFiringNow = true;
			}
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000020BC File Offset: 0x000002BC
		[HarmonyPatch(typeof(RangedSiegeWeapon), "Shoot")]
		[HarmonyPostfix]
		public static void Postfix_Shoot()
		{
			SiegeWeaponPatch.IsFiringNow = false;
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000020C8 File Offset: 0x000002C8
		[HarmonyPatch(typeof(RangedSiegeWeapon), "MaximumBallisticError", 1)]
		[HarmonyPrefix]
		public static bool Prefix_GetError(ref float __result)
		{
			bool isFiringNow = SiegeWeaponPatch.IsFiringNow;
			bool flag;
			if (isFiringNow)
			{
				__result = 0.001f;
				flag = false;
			}
			else
			{
				flag = true;
			}
			return flag;
		}

		// Token: 0x04000003 RID: 3
		public static bool IsFiringNow;
	}
}
