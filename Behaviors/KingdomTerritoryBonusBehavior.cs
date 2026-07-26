using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace ExampleMod.Behaviors
{
    /// <summary>
    /// 追踪每个王国的领土丢失/征服情况，并累积队伍规模加成。
    /// 丢失定居点增加加成（带有衰减效果）；征服定居点减少加成。
    /// 由 Harmony 补丁用于调整领主队伍规模上限。
    /// </summary>
    public class KingdomTerritoryBonusBehavior : CampaignBehaviorBase
    {
        private Dictionary<Kingdom, KingdomTerritoryData> _kingdomAccumulators =
            new Dictionary<Kingdom, KingdomTerritoryData>();

        public override void RegisterEvents()
        {
            CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, OnSettlementOwnerChanged);
            CampaignEvents.KingdomCreatedEvent.AddNonSerializedListener(this, OnKingdomCreated);
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("_kingdomAccumulators", ref _kingdomAccumulators);
        }

        /// <summary>
        /// 返回王国当前累积的领土加成值。
        /// 直接返回缓存值（O(1)），不在领土事件发生时永不复算。
        /// 如果王国没有数据或该功能已禁用，则返回 0。
        ///
        /// 旧存档兼容：自动检测旧版计数器并一次性迁移到 Events。
        /// </summary>
        public float GetTerritoryBonus(Kingdom kingdom)
        {
            if (Settings.Instance?.TerritoryBonusEnabled != true) return 0f;
            if (kingdom == null) return 0f;
            if (!_kingdomAccumulators.TryGetValue(kingdom, out var data))
                return 0f;

            TryMigrateFromOldFormat(data, kingdom);
            return Math.Max(0f, data.AccumulatedBonus);
        }

        /// <summary>
        /// 遍历全部事件，先计算净丢失数（丢失-征服，钳制到0），
        /// 再对净丢失序列按时间顺序应用衰减，算出最终加成。
        /// 征服事件不直接扣减加成，只减少净丢失计数。
        /// 只在领土事件发生时调用，其他时候不重算。
        /// </summary>
        private float RecalculateFromEvents(KingdomTerritoryData data)
        {
            if (data.Events == null || data.Events.Count == 0)
                return 0f;

            float townValue = Settings.Instance?.TerritoryBonusTownValue ?? 5f;
            float castleValue = Settings.Instance?.TerritoryBonusCastleValue ?? 3f;
            float diminishRate = Settings.Instance?.TerritoryBonusDiminishRate ?? 0.85f;
            float maxCap = Settings.Instance?.TerritoryBonusMaxCap ?? 200f;

            // 第一步：计算净丢失数（征服会抵消之前的丢失）
            int netTowns = 0, netCastles = 0;
            for (int i = 0; i < data.Events.Count; i++)
            {
                if (data.Events[i].IsLoss)
                {
                    if (data.Events[i].IsTown) netTowns++;
                    else netCastles++;
                }
                else // 征服
                {
                    if (data.Events[i].IsTown) netTowns = Math.Max(0, netTowns - 1);
                    else netCastles = Math.Max(0, netCastles - 1);
                }
            }

            // 第二步：城镇和城堡各自独立衰减计算，最后相加
            float total = 0f;
            for (int i = 0; i < netTowns; i++)
                total += townValue * (float)Math.Pow(diminishRate, i);
            for (int i = 0; i < netCastles; i++)
                total += castleValue * (float)Math.Pow(diminishRate, i);

            return Math.Min(total, maxCap);
        }

        /// <summary>
        /// 将旧存档的累计计数器（TownsLost/CastlesLost - TownsConquered/CastlesConquered）
        /// 转换为新系统的 Events，并重算缓存。
        /// 只在首次检测到旧存档数据时执行一次。
        /// 转换策略：将净丢失全部生成为丢失事件。
        /// </summary>
        private void TryMigrateFromOldFormat(KingdomTerritoryData data, Kingdom kingdom)
        {
            // 旧存档兼容：反序列化时字段初始化器不会执行，Events 可能为 null
            if (data.Events == null)
                data.Events = new List<TerritoryEvent>();

#pragma warning disable 612,618
            if (data.Events.Count > 0)
                return;
            if (data.TownsLost == 0 && data.CastlesLost == 0)
                return;

            int netTowns = Math.Max(0, data.TownsLost - data.TownsConquered);
            int netCastles = Math.Max(0, data.CastlesLost - data.CastlesConquered);
#pragma warning restore 612,618

            if (netTowns == 0 && netCastles == 0)
                return;

            for (int i = 0; i < netTowns; i++)
                data.Events.Add(new TerritoryEvent { IsTown = true, IsLoss = true });
            for (int i = 0; i < netCastles; i++)
                data.Events.Add(new TerritoryEvent { IsTown = false, IsLoss = true });

            data.AccumulatedBonus = RecalculateFromEvents(data);
            LogDebug($"[领土补偿] 旧存档迁移: {kingdom.Name} 转换 {netTowns}城+{netCastles}堡 → Events({data.Events.Count}条), 缓存加成={data.AccumulatedBonus:F2}");
        }

        private void OnKingdomCreated(Kingdom kingdom)
        {
            if (kingdom == null) return;
            if (!_kingdomAccumulators.ContainsKey(kingdom))
            {
                _kingdomAccumulators[kingdom] = new KingdomTerritoryData
                {
                    AccumulatedBonus = 0f,
                    Events = new List<TerritoryEvent>()
                };
            }
        }

        private void OnSettlementOwnerChanged(
            Settlement settlement,
            bool openToClaim,
            Hero newOwner,
            Hero oldOwner,
            Hero capturerHero,
            ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
        {
            // 无论开关状态，始终记录领土变化事件（开关只控制 GetTerritoryBonus 的返回值）

            // 只追踪要塞（城镇和城堡），不追踪村庄
            if (!settlement.IsTown && !settlement.IsCastle) return;

            Kingdom oldKingdom = oldOwner?.Clan?.Kingdom;
            Kingdom newKingdom = newOwner?.Clan?.Kingdom;

            // 同王国内部交易 = 不变
            if (oldKingdom == newKingdom && oldKingdom != null)
            {
                LogDebug($"[领土补偿] 内部转让，跳过: {settlement.Name?.ToString()} → 仍在 {oldKingdom.Name?.ToString()}");
                return;
            }

            LogDebug($"[领土补偿] 事件触发: {settlement.Name?.ToString()} ({(settlement.IsTown ? "城镇" : "城堡")}) {oldKingdom?.Name?.ToString() ?? "无王国"}→{newKingdom?.Name?.ToString() ?? "无王国"}");

            bool isTown = settlement.IsTown;

            // ── 旧王国丢失定居点 → 追加一条丢失事件 ────────────────
            if (oldKingdom != null)
            {
                EnsureKingdomData(oldKingdom);
                var data = _kingdomAccumulators[oldKingdom];
                TryMigrateFromOldFormat(data, oldKingdom);

                float previousBonus = data.AccumulatedBonus;
                data.Events.Add(new TerritoryEvent { IsTown = isTown, IsLoss = true });
                data.AccumulatedBonus = RecalculateFromEvents(data);

                LogDebug($"[领土补偿] {oldKingdom.Name} 丢失 {(isTown?"城镇":"城堡")} {settlement.Name}: 加成 {previousBonus:F2} → {data.AccumulatedBonus:F2}");
            }

            // ── 新王国征服定居点 → 追加一条征服事件 ──────────────
            if (newKingdom != null)
            {
                EnsureKingdomData(newKingdom);
                var data = _kingdomAccumulators[newKingdom];
                TryMigrateFromOldFormat(data, newKingdom);

                float previousBonus = data.AccumulatedBonus;
                data.Events.Add(new TerritoryEvent { IsTown = isTown, IsLoss = false });
                data.AccumulatedBonus = RecalculateFromEvents(data);

                LogDebug($"[领土补偿] {newKingdom.Name} 征服 {(isTown?"城镇":"城堡")} {settlement.Name}: 加成 {previousBonus:F2} → {data.AccumulatedBonus:F2}");
            }
        }

        private void EnsureKingdomData(Kingdom kingdom)
        {
            if (!_kingdomAccumulators.ContainsKey(kingdom))
            {
                _kingdomAccumulators[kingdom] = new KingdomTerritoryData
                {
                    AccumulatedBonus = 0f,
                    Events = new List<TerritoryEvent>()
                };
                LogDebug($"[领土补偿] 初始化数据: {kingdom.Name?.ToString() ?? kingdom.StringId}");
            }
        }

        // ── 调试日志（屏幕左下角）────────────────────────────────────────

        private static void LogDebug(string message)
        {
            if (Settings.Instance?.EnableDebugLogging != true) return;
            InformationManager.DisplayMessage(
                new InformationMessage(message, Color.FromUint(0x00FFFFu)));
        }
    }
}
