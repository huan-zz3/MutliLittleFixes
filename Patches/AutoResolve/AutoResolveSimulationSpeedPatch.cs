using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;

namespace MutliLittleFixes.Patches
{
    /// <summary>
    /// 坐镇指挥模拟重平衡 —— AI 对 AI 战斗加速（缩短模拟结算间隔）。
    ///
    /// 原版大地图上 AI 部队之间的战斗每 30 分钟（攻城突击 60 分钟）游戏时间推进一轮
    /// SimulateBattleRound（间隔由 DefaultCombatSimulationModel.GetSimulationTickInterval 决定），
    /// 旁观 AI 打架的等待时间较长。旧方案用「伤害倍率」放大单次命中伤害来加快战斗，
    /// 会扭曲伤亡数字与胜负比例；本方案改为把结算间隔缩短 N 倍：
    ///   - 每轮内部的命中伤害、护甲、武器加成公式完全不动；
    ///   - 同样的游戏时间内推进更多轮次，战斗更快分胜负；
    ///   - 玩家坐镇（SimulatePlayerEncounterBattle）由坐镇 UI 驱动、不读该间隔，零影响；
    ///   - 仅作用于玩家未参与（IsPlayerMapEvent == false）的纯 AI 对 AI 战斗。
    /// </summary>
    internal static class AutoResolveSimulationSpeedPatch
    {
        internal static void Postfix(MapEvent mapEvent, ref CampaignTime __result)
        {
            // MCM 运行时开关 — 关闭时不干预
            if (Settings.Instance?.AutoResolveEnabled != true)
                return;
            // AI 对 AI 未启用重平衡时也不加速
            if (Settings.Instance.AutoResolveAiEnabled != true)
                return;
            // 玩家参与的战斗（含玩家坐镇）不加速
            if (mapEvent.IsPlayerMapEvent)
                return;

            try
            {
                float speed = Settings.Instance.AutoResolveAiSimulationSpeed;
                if (speed <= 1f)
                    return;
                double minutes = __result.ToMinutes;
                __result = CampaignTime.Minutes((long)Math.Max(1L, minutes / speed));
            }
            catch (Exception ex)
            {
                AutoResolveLog.PrintError("[坐镇重平衡] 模拟速度补丁异常: " + ex);
            }
        }
    }
}
