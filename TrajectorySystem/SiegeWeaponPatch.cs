using HarmonyLib;
using TaleWorlds.MountAndBlade;

namespace ExampleMod
{
    [HarmonyPatch]
    public static class SiegeWeaponPatch
    {
        [HarmonyPatch(typeof(RangedSiegeWeapon), "Shoot")]
        [HarmonyPrefix]
        public static void Prefix_Shoot(RangedSiegeWeapon __instance)
        {
            if (__instance.PilotAgent != null && __instance.PilotAgent.IsMainAgent)
            {
                IsFiringNow = true;
            }
        }

        [HarmonyPatch(typeof(RangedSiegeWeapon), "Shoot")]
        [HarmonyPostfix]
        public static void Postfix_Shoot()
        {
            IsFiringNow = false;
        }

        [HarmonyPatch(typeof(RangedSiegeWeapon), "get_MaximumBallisticError")]
        [HarmonyPrefix]
        public static bool Prefix_GetError(ref float __result)
        {
            if (IsFiringNow)
            {
                __result = 0.001f;
                return false;
            }
            return true;
        }

        public static bool IsFiringNow;
    }
}
