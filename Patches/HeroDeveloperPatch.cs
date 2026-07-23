using HarmonyLib;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace ExampleMod.Patches
{
    /// <summary>
    /// 经验倍率：在 GainRawXp 中倍增主角的 TotalXp（角色升级经验），不影响技能经验
    /// </summary>
    [HarmonyPatch(typeof(HeroDeveloper), "GainRawXp")]
    internal static class HeroDeveloperPatch
    {
        internal static void Prefix(HeroDeveloper __instance, ref float rawXp)
        {
            // 仅对主角生效
            if (__instance.Hero != null && __instance.Hero == TaleWorlds.CampaignSystem.Hero.MainHero)
            {
                float expMultiplier = Settings.Instance?.ExperienceMultiplier ?? 1.0f;
                if (expMultiplier != 1.0f)
                {
                    rawXp *= expMultiplier;
                }
            }
        }
    }
}
