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
        /// 三阶段计算：
        ///   ① 同城配对：按 SettlementId 分组，同一城相邻的丢失↔征服配对抵消（栈算法，不修改 Events）。
        ///   ② 过期过滤：未配对事件检查天数阈值：
        ///      - 丢失超过 LossExpireDays 天 → 过期不计
        ///      - 征服超过 ConquestSolidifyDays 天 → 已固化，不计入跨城抵消
        ///      - 旧存档事件(SettlementId=null, EventDay≤1)永不过期
        ///   ③ 跨城抵消 + 衰减：有效未配对丢失数 - 征服数 → 净丢失 → 衰减累加
        /// </summary>
        private float RecalculateFromEvents(KingdomTerritoryData data)
        {
            if (data.Events == null || data.Events.Count == 0)
                return 0f;

            // ── 旧存档兼容：将 EventDay==0 的事件设为 Day 1 ──────────────
            foreach (var e in data.Events)
            {
                if (e.EventDay == 0)
                    e.EventDay = 1;
            }

            float townValue = Settings.Instance?.TerritoryBonusTownValue ?? 5f;
            float castleValue = Settings.Instance?.TerritoryBonusCastleValue ?? 3f;
            float diminishRate = Settings.Instance?.TerritoryBonusDiminishRate ?? 0.85f;
            float maxCap = Settings.Instance?.TerritoryBonusMaxCap ?? 200f;
            int solidifyDays = Settings.Instance?.ConquestSolidifyDays ?? 30;
            int expireDays = Settings.Instance?.LossExpireDays ?? 30;
            int currentDay = GetCurrentCampaignDay();

            // ── 阶段一：按 SettlementId 分组，同城得失配对 ──────────────
            var bySettlement = new Dictionary<string, List<int>>(StringComparer.Ordinal);
            for (int i = 0; i < data.Events.Count; i++)
            {
                string key = data.Events[i].SettlementId ?? "__null__";
                if (!bySettlement.ContainsKey(key))
                    bySettlement[key] = new List<int>();
                bySettlement[key].Add(i);
            }

            var paired = new HashSet<int>();

            foreach (var kvp in bySettlement)
            {
                var indices = kvp.Value;
                var stack = new List<int>(); // 临时栈，只存索引

                foreach (int idx in indices)
                {
                    stack.Add(idx);
                    // 栈顶两个类型相反 → 配对抵消
                    while (stack.Count >= 2)
                    {
                        int top = stack[stack.Count - 1];
                        int second = stack[stack.Count - 2];
                        if (data.Events[top].IsLoss != data.Events[second].IsLoss)
                        {
                            paired.Add(top);
                            paired.Add(second);
                            stack.RemoveAt(stack.Count - 1);
                            stack.RemoveAt(stack.Count - 1);
                        }
                        else break;
                    }
                }
            }

            // ── 阶段二：未配对事件按过期阈值过滤 ──────────────────
            int netTowns = 0, netCastles = 0;

            for (int i = 0; i < data.Events.Count; i++)
            {
                if (paired.Contains(i))
                    continue;

                var e = data.Events[i];
                bool isLegacy = (e.SettlementId == null && e.EventDay <= 1);
                int age = currentDay - e.EventDay;

                if (e.IsLoss)
                {
                    // 旧存档永不过期；非旧存档且超过 LossExpireDays → 过期
                    if (!isLegacy && expireDays > 0 && age >= expireDays)
                        continue;

                    if (e.IsTown) netTowns++;
                    else netCastles++;
                }
                else // 征服
                {
                    // 旧存档永不固化；非旧存档且超过 ConquestSolidifyDays → 已固化，不抵消丢失
                    if (!isLegacy && solidifyDays > 0 && age >= solidifyDays)
                        continue;

                    if (e.IsTown) netTowns = Math.Max(0, netTowns - 1);
                    else netCastles = Math.Max(0, netCastles - 1);
                }
            }

            // ── 阶段三：衰减累加 ──────────────────────────────────
            float total = 0f;
            for (int i = 0; i < netTowns; i++)
                total += townValue * (float)Math.Pow(diminishRate, i);
            for (int i = 0; i < netCastles; i++)
                total += castleValue * (float)Math.Pow(diminishRate, i);

            return Math.Min(total, maxCap);
        }

        /// <summary>
        /// 返回游戏界面显示的战役天数（从战役开始到现在的经过天数）。
        /// 注意：CampaignTime.Now.ToDays 返回的是卡拉丁纪元总天数（包含纪元偏移），
        /// 而游戏 UI 显示的是 CampaignStartTime 到现在的经过天数。
        /// </summary>
        private static int GetCurrentCampaignDay()
        {
            return (int)Campaign.Current.Models.CampaignTimeModel.CampaignStartTime.ElapsedDaysUntilNow;
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
                data.Events.Add(new TerritoryEvent { IsTown = true, IsLoss = true, EventDay = 1, SettlementId = null });
            for (int i = 0; i < netCastles; i++)
                data.Events.Add(new TerritoryEvent { IsTown = false, IsLoss = true, EventDay = 1, SettlementId = null });

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
            int currentDay = GetCurrentCampaignDay();
            string settlementId = settlement.StringId;

            // ── 旧王国丢失定居点 → 追加一条丢失事件 ────────────────
            if (oldKingdom != null)
            {
                EnsureKingdomData(oldKingdom);
                var data = _kingdomAccumulators[oldKingdom];
                TryMigrateFromOldFormat(data, oldKingdom);

                float previousBonus = data.AccumulatedBonus;
                data.Events.Add(new TerritoryEvent { IsTown = isTown, IsLoss = true, EventDay = currentDay, SettlementId = settlementId });
                data.AccumulatedBonus = RecalculateFromEvents(data);

                LogDebug($"[领土补偿] {oldKingdom.Name} 丢失 {(isTown?"城镇":"城堡")} {settlement.Name} (day{currentDay}): 加成 {previousBonus:F2} → {data.AccumulatedBonus:F2}");
            }

            // ── 新王国征服定居点 → 追加一条征服事件 ──────────────
            if (newKingdom != null)
            {
                EnsureKingdomData(newKingdom);
                var data = _kingdomAccumulators[newKingdom];
                TryMigrateFromOldFormat(data, newKingdom);

                float previousBonus = data.AccumulatedBonus;
                data.Events.Add(new TerritoryEvent { IsTown = isTown, IsLoss = false, EventDay = currentDay, SettlementId = settlementId });
                data.AccumulatedBonus = RecalculateFromEvents(data);

                LogDebug($"[领土补偿] {newKingdom.Name} 征服 {(isTown?"城镇":"城堡")} {settlement.Name} (day{currentDay}): 加成 {previousBonus:F2} → {data.AccumulatedBonus:F2}");
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
