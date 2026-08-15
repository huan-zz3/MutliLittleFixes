using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.MapEvents;

namespace MutliLittleFixes.Patches
{
    /// <summary>
    /// 坐镇指挥模拟重平衡 —— 攻击频次比上限。
    ///
    /// 原版每轮 tick 数 = min(对方×2, 己方^0.6)，兵力越悬殊攻击频次比越大（10:1 → 约 4:1）。
    /// 本补丁把两侧 tick 比 clamp 到上限：默认上限兵力比 2:1（对应频次比 2^0.6 ≈ 1.52），
    /// 可通过 MCM 调整上限兵力比。战斗后期弱势方减员更快时，频次差距不再无限拉大。
    ///
    /// 注意：tick 数向下取整，clamp 后可能让大侧总轮数略有减少，但保持每轮内部分子（伤害/护甲）不变。
    /// </summary>
    internal static class AutoResolveAttackRatioCapPatch
    {
        internal static void Postfix(MapEvent mapEvent, ref (int defenderRounds, int attackerRounds) __result)
        {
            // MCM 运行时开关 — 关闭时不干预
            if (Settings.Instance?.AutoResolveEnabled != true)
                return;

            try
            {
                if (Settings.Instance.AutoResolveAiEnabled || mapEvent.IsPlayerSimulation)
                {
                    float maxForceRatio = Settings.Instance.AutoResolveAttackRatioCap;
                    if (maxForceRatio <= 1f)
                    {
                        return; // 不设上限
                    }
                    // 频次比上限 = 上限兵力比 ^ 0.6（与原版 tick 公式同幂）
                    float maxTickRatio = (float)Math.Pow(maxForceRatio, 0.6);

                    int defenderTicks = __result.defenderRounds;
                    int attackerTicks = __result.attackerRounds;
                    int bigger = Math.Max(defenderTicks, attackerTicks);
                    int smaller = Math.Min(defenderTicks, attackerTicks);
                    if (smaller <= 0 || bigger <= smaller * maxTickRatio)
                    {
                        return; // 未超上限
                    }

                    int capped = (int)Math.Floor(smaller * maxTickRatio);
                    if (capped < smaller)
                    {
                        capped = smaller; // 保底：大侧至少与小侧相等
                    }

                    if (defenderTicks > attackerTicks)
                    {
                        __result.defenderRounds = capped;
                    }
                    else
                    {
                        __result.attackerRounds = capped;
                    }
                }
            }
            catch (Exception ex)
            {
                AutoResolveLog.PrintError("[坐镇重平衡] 攻击频次上限补丁异常: " + ex);
            }
        }
    }
}
