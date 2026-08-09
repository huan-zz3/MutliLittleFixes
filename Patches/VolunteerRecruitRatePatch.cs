namespace MutliLittleFixes.Patches
{
    /// <summary>
    /// 城镇/村庄名人志愿者每日补充概率倍率补丁。
    ///
    /// 原版行为：RecruitmentCampaignBehavior.UpdateVolunteersOfNotablesInSettlement 每日
    /// 调用 VolunteerModel.GetDailyVolunteerProductionProbability(hero, index, settlement)
    /// 决定每个槽位是否补充/升级志愿者。
    ///
    /// 本补丁（Postfix）将返回值乘以 MCM 倍率，倍率 1.0 = 原版，实时生效：
    /// - 保留原版全部动态因素（槽位指数衰减 0.7^(index+1)、王国规模系数、政策、骑兵 Perk）
    /// - 城镇/村庄同公式，本补丁对两者一视同仁
    /// - 关闭开关时直接返回（等效原版）
    ///
    /// 由 HarmonyPatchRegistry 显式注册（不使用 [HarmonyPatch] 属性）。
    /// </summary>
    internal static class VolunteerRecruitRatePatch
    {
        public static void Postfix(ref float __result)
        {
            if (Settings.Instance?.VolunteerRecruitRateEnabled != true)
                return;

            __result *= Settings.Instance.VolunteerRecruitRateMultiplier;
        }
    }
}
