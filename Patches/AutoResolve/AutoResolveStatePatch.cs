using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.Core;

namespace MutliLittleFixes.Patches
{
    /// <summary>
    /// 坐镇指挥模拟重平衡 —— 累计 HP 状态的登记钩子（AllocateTroops Postfix）。
    ///
    /// 每次分配模拟部队后，为当前 MapEventSide 登记/更新 AutoResolveSimulationData
    /// （每个非英雄士兵的累计 HP 字典）。已有数据时先清空再按最新 troopsList 重建
    /// （重建时若士兵数增加会按平均 HP 比例压缩存量，见 AutoResolveSimulationData.UpdateDict）。
    ///
    /// 对应旧版 AutoResolveRebalanced 的 Patch_AllocateTroops。
    /// </summary>
    internal static class AutoResolveAllocateTroopsPatch
    {
        internal static void Postfix(ref List<UniqueTroopDescriptor> troopsList, MapEventSide __instance)
        {
            // MCM 运行时开关 — 关闭时不干预
            if (Settings.Instance?.AutoResolveEnabled != true)
                return;

            try
            {
                if (Settings.Instance.AutoResolveAiEnabled || __instance.MapEvent.IsPlayerSimulation)
                {
                    AutoResolveSimulationData data;
                    if (AutoResolveSimulationDataDict.GetData(__instance, out data))
                    {
                        data.Clear(clearAvg: false);
                        data.UpdateDict(__instance, troopsList);
                    }
                    else
                    {
                        data = new AutoResolveSimulationData();
                        data.UpdateDict(__instance, troopsList);
                        AutoResolveSimulationDataDict.AddData(__instance, data);
                    }
                }
            }
            catch (Exception ex)
            {
                AutoResolveLog.PrintError("[坐镇重平衡] AllocateTroops 补丁异常: " + ex);
            }
        }
    }

    /// <summary>
    /// 坐镇指挥模拟重平衡 —— 累计 HP 状态的跨回合续算钩子（EndSimulation Prefix）。
    ///
    /// 在原版 EndSimulation 清空模拟列表之前，把剩余士兵数与平均 HP 存回 SimulateData，
    /// 供下一回合 AllocateTroops → UpdateDict 按比例续算存量 HP（含援军入场加权）。
    /// 随后放行原方法（返回 true）。
    ///
    /// 对应旧版 AutoResolveRebalanced 的 Patch_EndSimulation。
    /// </summary>
    internal static class AutoResolveEndSimulationPatch
    {
        internal static bool Prefix(MapEventSide __instance, ref List<UniqueTroopDescriptor> ____simulationTroopList)
        {
            // MCM 运行时开关 — 关闭时不干预（放行原方法）
            if (Settings.Instance?.AutoResolveEnabled != true)
                return true;

            try
            {
                if (Settings.Instance.AutoResolveAiEnabled || __instance.MapEvent.IsPlayerSimulation)
                {
                    AutoResolveSimulationData data;
                    if (AutoResolveSimulationDataDict.GetData(__instance, out data))
                    {
                        data.StoreTroopNumber(____simulationTroopList.Count);
                        data.StoreHitPointAverage();
                        data.Clear(clearAvg: false);
                    }
                    else if (__instance.MapEvent.BattleState == BattleState.None)
                    {
                        AutoResolveLog.PrintWarn("[坐镇重平衡] EndSimulation 未找到累计 HP 数据");
                    }
                }
            }
            catch (Exception ex)
            {
                AutoResolveLog.PrintError("[坐镇重平衡] EndSimulation 补丁异常: " + ex);
            }
            return true;
        }
    }
}
