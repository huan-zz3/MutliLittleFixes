using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.Core;

namespace MutliLittleFixes.Patches
{
    /// <summary>
    /// 坐镇指挥模拟重平衡 —— 每侧士兵累计 HP 状态层。
    ///
    /// 原版坐镇指挥的伤亡判定是「每次命中掷一次骰子（RandomInt(最大HP) &lt; 伤害即击倒）」，
    /// 没有累计伤害的概念。本类为每个参与模拟的非英雄士兵维护独立的累计 HP（默认 100），
    /// 伤害累加扣到 ≤0 才进入存活判定（外科手术/医生誓约），实现「士兵更抗打、高伤武器更有价值」。
    ///
    /// 对应旧版 AutoResolveRebalanced 的 SimulateData + SimulateDataDict（合并为单类）。
    /// </summary>
    internal sealed class AutoResolveSimulationData
    {
        private const int DefaultHitPoints = 100;

        private readonly ConcurrentDictionary<UniqueTroopDescriptor, int> _hitPointDict = new ConcurrentDictionary<UniqueTroopDescriptor, int>();

        private float _hitPointAverage = -1f;

        private int _troopNumber = -1;

        /// <summary>为指定一方的模拟部队列表初始化/更新累计 HP 字典。</summary>
        public void UpdateDict(MapEventSide side, List<UniqueTroopDescriptor> troopList)
        {
            float ratio = 1f;
            if (_hitPointAverage > 0f)
            {
                // 计算当前列表剩余非英雄士兵的平均 HP
                float currentAverage = 0f;
                int count = 0;
                foreach (UniqueTroopDescriptor descriptor in troopList)
                {
                    CharacterObject troop = side.GetAllocatedTroop(descriptor);
                    if (troop != null && !troop.IsHero)
                    {
                        currentAverage += troop.HitPoints;
                        count++;
                    }
                }
                if (count > 0)
                {
                    currentAverage = (float)Math.Ceiling(currentAverage / count);
                }
                // 援军入场（士兵数增加）时，把旧的平均 HP 与新兵平均 HP 加权合并
                if (_troopNumber > 0 && troopList.Count > _troopNumber)
                {
                    _hitPointAverage = (currentAverage * (troopList.Count - _troopNumber) + _hitPointAverage * _troopNumber) / troopList.Count;
                }
                // 若当前平均 HP 高于存储值，按比例压缩存量 HP（保持总血量守恒）
                if (currentAverage > _hitPointAverage)
                {
                    ratio = _hitPointAverage / currentAverage;
                }
                _hitPointAverage = -1f;
                _troopNumber = -1;
            }
            foreach (UniqueTroopDescriptor descriptor2 in troopList)
            {
                CharacterObject troop2 = side.GetAllocatedTroop(descriptor2);
                if (troop2 != null && !troop2.IsHero)
                {
                    int hitPoints = (int)(troop2.HitPoints * ratio);
                    _hitPointDict.TryAdd(descriptor2, hitPoints);
                }
            }
        }

        /// <summary>读取士兵累计 HP；字典缺失时回退随机值（与原版一致的行为兜底）。</summary>
        public bool GetHitPoint(UniqueTroopDescriptor descriptor, out int hitPoints)
        {
            bool found = _hitPointDict.TryGetValue(descriptor, out hitPoints);
            if (!found)
            {
                hitPoints = MBRandom.RandomInt(DefaultHitPoints);
            }
            return found;
        }

        /// <summary>回合结束前计算剩余士兵的平均 HP，供下回合续算。</summary>
        public void StoreHitPointAverage()
        {
            int count = 0;
            _hitPointAverage = 0f;
            foreach (int hitPoints in _hitPointDict.Values)
            {
                if (hitPoints > 0)
                {
                    count++;
                    _hitPointAverage += hitPoints;
                }
            }
            if (count > 0)
            {
                _hitPointAverage = (float)Math.Ceiling(_hitPointAverage / count);
            }
        }

        /// <summary>记录回合结束时的模拟士兵总数（供援军加权合并）。</summary>
        public void StoreTroopNumber(int troopNumber)
        {
            _troopNumber = troopNumber;
        }

        public void SetHitPoint(UniqueTroopDescriptor descriptor, int hitPoints)
        {
            _hitPointDict[descriptor] = hitPoints;
        }

        /// <summary>清空字典；clearAvg 为 true 时同时重置跨回合续算状态。</summary>
        public void Clear(bool clearAvg = false)
        {
            _hitPointDict.Clear();
            if (clearAvg)
            {
                _hitPointAverage = -1f;
                _troopNumber = -1;
            }
        }
    }

    /// <summary>MapEventSide → AutoResolveSimulationData 的全局并发字典（按战斗双方各一份）。</summary>
    internal static class AutoResolveSimulationDataDict
    {
        private static readonly ConcurrentDictionary<MapEventSide, AutoResolveSimulationData> _dict = new ConcurrentDictionary<MapEventSide, AutoResolveSimulationData>();

        public static bool AddData(MapEventSide side, AutoResolveSimulationData data)
        {
            return _dict.TryAdd(side, data);
        }

        public static bool GetData(MapEventSide side, out AutoResolveSimulationData data)
        {
            return _dict.TryGetValue(side, out data);
        }

        public static bool RemoveData(MapEventSide side)
        {
            return _dict.TryRemove(side, out _);
        }
    }
}
