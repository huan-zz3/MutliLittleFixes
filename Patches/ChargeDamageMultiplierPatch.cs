using TaleWorlds.Library;

namespace MutliLittleFixes.Patches
{
    /// <summary>
    /// 马匹冲撞伤害倍率补丁。
    ///
    /// 目标方法：MissionCombatMechanicsHelper.ComputeBlowMagnitudeFromHorseCharge
    /// （纯马匹冲撞伤害的唯一计算点——Mission.ChargeDamageCallback → GetAttackCollisionResults
    /// → ComputeBlowMagnitude 在 acd.IsHorseCharge 时转发至此，patch 一处即全覆盖）。
    ///
    /// 行为（由 MCM 开关实时控制，敌我双方对称生效）：
    ///   - 开启时把马匹本体冲撞（非武器攻击，马匹直接撞上敌人）造成的伤害按 MCM 倍率放大；
    ///   - 基础伤害与特殊伤害同步放大，保持原版「伤害 / 击倒判定」之间的比例关系不变。
    ///
    /// 由 HarmonyPatchRegistry 显式注册（不使用 [HarmonyPatch] 属性）。
    /// </summary>
    internal static class ChargeDamageMultiplierPatch
    {
        /// <summary>MCM 实时开关：马匹冲撞伤害倍率。</summary>
        private static bool IsEnabled =>
            Settings.Instance?.ChargeDamageMultiplierEnabled != false;

        /// <summary>MCM 实时倍率：冲撞伤害放大倍率（默认 2.0 = 200% 伤害，1.0 = 原版）。</summary>
        private static float Multiplier =>
            Settings.Instance?.ChargeDamageMultiplier ?? 2.0f;

        internal static void Postfix(ref float baseMagnitude, ref float specialMagnitude)
        {
            // 海战禁用（战帆 DLC 海战/沿海掠夺海战）— 统一原则：士兵行为调整在海战不干预
            if (NavalBattleDetector.IsNavalBattle())
            {
                return;
            }

            // MCM 运行时开关 — 关闭或倍率等于原版（1.0）时不干预
            float multiplier = Multiplier;
            if (!IsEnabled || MathF.Abs(multiplier - 1f) < 0.001f)
            {
                return;
            }

            // 放大马匹冲撞伤害（基础伤害 + 特殊伤害同步放大，保持原版伤害/击倒关系）
            baseMagnitude *= multiplier;
            specialMagnitude *= multiplier;
        }
    }
}