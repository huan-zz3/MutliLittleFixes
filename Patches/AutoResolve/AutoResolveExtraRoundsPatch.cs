using System;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.Core;

namespace MutliLittleFixes.Patches
{
    /// <summary>
    /// 坐镇指挥模拟重平衡 —— 兵力悬殊时追加模拟回合。
    ///
    /// 原版每回合的模拟轮数由 GetSimulationTicksForBattleRound 一次性给定，轮数耗尽战斗就暂停；
    /// 当大兵力一方（兵力比 &gt; 10:1）的小兵力残兵尚未被消灭时，战斗会「打不完」。
    /// 本补丁在轮数耗尽且未分胜负时，给大兵力一方追加 10 轮模拟，直到对方减员或战斗结束。
    ///
    /// 适配说明：旧版目标方法为 MapEvent.SimulateBattleForRounds（参数 simulationRoundsDefender/Attacker），
    /// 1.4.5 中该方法已更名为 MapEvent.SimulateBattleRound（参数 simulationTicksDefender/Attacker），
    /// 行为等价（一轮内按轮数比例随机选边执行命中模拟）。
    ///
    /// 对应旧版 AutoResolveRebalanced 的 Patch_SimulateBattleForRounds。
    /// </summary>
    internal static class AutoResolveExtraRoundsPatch
    {
        // 防递归：Postfix 内再次调用 SimulateBattleRound 会再次进入本 Postfix，置位后直接放行
        private static bool _runningExtraRounds;

        // 防御性上限：单次 Postfix 最多追加 20 组 × 10 轮，防止极端情况下死循环
        private const int MaxExtraRoundGroups = 20;

        internal static void Postfix(MapEvent __instance, int simulationTicksDefender, int simulationTicksAttacker)
        {
            // MCM 运行时开关 — 关闭时不干预
            if (Settings.Instance?.AutoResolveEnabled != true)
                return;

            if (_runningExtraRounds)
                return;

            try
            {
                if ((Settings.Instance.AutoResolveAiEnabled || __instance.IsPlayerSimulation)
                    && __instance.BattleState == BattleState.None
                    && __instance.AttackerSide.NumRemainingSimulationTroops > 0
                    && __instance.DefenderSide.NumRemainingSimulationTroops > 0)
                {
                    int defenderRemaining = __instance.DefenderSide.NumRemainingSimulationTroops;
                    int attackerRemaining = __instance.AttackerSide.NumRemainingSimulationTroops;
                    int guard = 0;

                    // 防守方兵力为攻击方 10 倍以上：给防守方追加轮数，直到攻击方出现减员或战斗结束
                    if ((float)defenderRemaining / (float)attackerRemaining > 10f)
                    {
                        _runningExtraRounds = true;
                        try
                        {
                            while (__instance.BattleState == BattleState.None
                                   && __instance.AttackerSide.NumRemainingSimulationTroops == attackerRemaining
                                   && guard++ < MaxExtraRoundGroups)
                            {
                                __instance.SimulateBattleRound(simulationTicksDefender + 10, simulationTicksAttacker);
                            }
                        }
                        finally
                        {
                            _runningExtraRounds = false;
                        }
                    }
                    // 攻击方兵力为防守方 10 倍以上：对称处理
                    else if ((float)attackerRemaining / (float)defenderRemaining > 10f)
                    {
                        _runningExtraRounds = true;
                        try
                        {
                            while (__instance.BattleState == BattleState.None
                                   && __instance.DefenderSide.NumRemainingSimulationTroops == defenderRemaining
                                   && guard++ < MaxExtraRoundGroups)
                            {
                                __instance.SimulateBattleRound(simulationTicksDefender, simulationTicksAttacker + 10);
                            }
                        }
                        finally
                        {
                            _runningExtraRounds = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AutoResolveLog.PrintError("[坐镇重平衡] 追加回合异常: " + ex);
            }
        }
    }
}
