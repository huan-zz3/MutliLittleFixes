using TaleWorlds.MountAndBlade;

namespace MutliLittleFixes
{
    public static class SiegeWeaponPatch
    {
        public static void Prefix_Shoot(RangedSiegeWeapon __instance)
        {
            if (__instance.PilotAgent != null && __instance.PilotAgent.IsMainAgent)
            {
                IsFiringNow = true;
            }
        }

        public static void Postfix_Shoot()
        {
            IsFiringNow = false;
        }

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
