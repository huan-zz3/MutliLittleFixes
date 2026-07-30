using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace ExampleMod.Behaviors
{
    /// <summary>
    /// 在 NPC 领主被释放后的数天内，逐步向其交付部队。
    /// 通过 DailyTickHeroEvent 每日交付，受配置的等级比例和队伍规模上限约束。
    /// </summary>
    public class LordTroopRestorationBehavior : CampaignBehaviorBase
    {
        private Dictionary<Hero, PendingRestoration> _pendingRestorations = new Dictionary<Hero, PendingRestoration>();

        public override void RegisterEvents()
        {
            CampaignEvents.HeroPrisonerReleased.AddNonSerializedListener(this, OnHeroPrisonerReleased);
            CampaignEvents.DailyTickHeroEvent.AddNonSerializedListener(this, OnDailyTickHero);
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("_pendingRestorations", ref _pendingRestorations);
        }

        // ── 事件：领主被释放 ─────────────────────────────────────────────

        private void OnHeroPrisonerReleased(
            Hero prisoner, PartyBase party, IFaction capturerFaction,
            EndCaptivityDetail detail, bool showNotification)
        {
            // 无论开关状态，始终记录释放事件（开关只控制是否发兵）
            if (prisoner == null || prisoner == Hero.MainHero)
            {
                LogDebug($"[补兵] 跳过释放: prisoner=null={prisoner == null}, MainHero={prisoner == Hero.MainHero}");
                return;
            }

            if (prisoner.Clan == null)
            {
                LogDebug($"[补兵] 跳过释放: {prisoner.Name} 无家族");
                return;
            }

            int partySizeLimit = GetPartySizeLimit(prisoner);
            int totalTroops = (int)(partySizeLimit * (Settings.Instance?.RestorationPartySizeRatio ?? 0.6f));

            if (totalTroops <= 0)
            {
                LogDebug($"[补兵] 跳过释放: {prisoner.Name} totalTroops={totalTroops} (上限={partySizeLimit}, 比例={Settings.Instance?.RestorationPartySizeRatio ?? 0.6f})");
                return;
            }

            float t12 = Settings.Instance?.RestorationTier12Ratio ?? 0.50f;
            float t34 = Settings.Instance?.RestorationTier34Ratio ?? 0.35f;
            float t56 = Settings.Instance?.RestorationTier56Ratio ?? 0.15f;
            // 强制归一化：即使三个比例之和≠1，也保证加起来正好等于1
            float totalRatio = t12 + t34 + t56;
            if (totalRatio > 0f) { t12 /= totalRatio; t34 /= totalRatio; t56 /= totalRatio; }

            string cultureId = DetermineCultureId(prisoner);
            int days = Math.Max(1, Settings.Instance?.RestorationDays ?? 7);
            int goldTotal = totalTroops * (int)(Settings.Instance?.RestorationGoldPerTroop ?? 0f);

            var pending = new PendingRestoration
            {
                DaysRemaining = days,
                TotalTroopsToDeliver = totalTroops,
                TroopsPerDay = totalTroops / days,
                GoldToDeliver = goldTotal,
                Tier12Ratio = t12,
                Tier34Ratio = t34,
                Tier56Ratio = t56,
                PartySizeLimitAtRelease = partySizeLimit,
                TroopCultureId = cultureId,
            };

            _pendingRestorations[prisoner] = pending;

            LogDebug($"[补兵] {GetHeroFactionPrefix(prisoner)}{prisoner.Name?.ToString()} 释放: 总量={totalTroops} 兵, {days}天, 金币={goldTotal}");
        }

        // ── 事件：英雄每日触发 ─────────────────────────────────────────────

        private void OnDailyTickHero(Hero hero)
        {
            if (!_pendingRestorations.TryGetValue(hero, out PendingRestoration pending))
                return;

            if (pending.DaysRemaining <= 0 || pending.TotalTroopsToDeliver <= 0)
            {
                LogDebug($"[补兵] {GetHeroFactionPrefix(hero)}{hero.Name} 恢复完成，移除");
                _pendingRestorations.Remove(hero);
                return;
            }

            // ── DaysWithoutParty 追踪（不受开关控制）────────────────────
            MobileParty party = hero.PartyBelongedTo;
            if (party == null)
            {
                pending.DaysWithoutParty++;
                int abandonDays = Settings.Instance?.RestorationAbandonDays ?? 15;
                if (abandonDays > 0 && pending.DaysWithoutParty >= abandonDays)
                {
                    LogDebug($"[补兵] {GetHeroFactionPrefix(hero)}{hero.Name} 已超过{abandonDays}天无队伍，放弃补兵");
                    _pendingRestorations.Remove(hero);
                    return;
                }
                LogDebug($"[补兵] {GetHeroFactionPrefix(hero)}{hero.Name} 今日无队伍，跳过(无队伍第{pending.DaysWithoutParty}天/剩余{pending.DaysRemaining}天/待交付{pending.TotalTroopsToDeliver}兵)");
                return;
            }

            // ── 开关控制是否发兵（不扣天数，记录保留以衔接）────────────
            if (Settings.Instance?.RestorationEnabled != true)
            {
                LogDebug($"[补兵] {GetHeroFactionPrefix(hero)}{hero.Name} 功能已禁用，保留记录待启用");
                return;
            }

            // ── 交付部队 ──────────────────────────────────────────────
            int troopsToday = (pending.DaysRemaining == 1)
                ? pending.TotalTroopsToDeliver
                : pending.TroopsPerDay;

            List<CharacterObject> chosen = new List<CharacterObject>();
            if (troopsToday > 0 && pending.TotalTroopsToDeliver > 0)
            {
                chosen = GetTroopsForToday(pending, troopsToday);
                foreach (CharacterObject troop in chosen)
                    party.MemberRoster.AddToCounts(troop, 1);

                pending.TotalTroopsToDeliver -= troopsToday;
            }

            // ── 交付金币（按比例每日交付）──────────────────────────────
            if (pending.GoldToDeliver > 0)
            {
                int goldToday = pending.GoldToDeliver / pending.DaysRemaining;
                if (goldToday > 0)
                {
                    party.PartyTradeGold += goldToday;
                    pending.GoldToDeliver -= goldToday;
                }
            }

            pending.DaysRemaining--;

            // 从部队 roster 中直接读取实际总兵数（而非简单相加）
            int totalTroopsNow = party.MemberRoster.TotalManCount;
            int lowCount = chosen.Count(t => t.Tier >= 1 && t.Tier <= 2);
            int midCount = chosen.Count(t => t.Tier >= 3 && t.Tier <= 4);
            int highCount = chosen.Count(t => t.Tier >= 5 && t.Tier <= 6);
            string troopSummary = chosen.Count > 0
                ? $"【低级兵{lowCount}个，中级兵{midCount}个，高级兵{highCount}个】"
                : "(无)";
            LogDebug($"[补兵] {GetHeroFactionPrefix(hero)}{hero.Name} 交付: +{troopsToday}兵 {troopSummary}, 现有总计{totalTroopsNow}兵, 剩余{pending.DaysRemaining}天/待{pending.TotalTroopsToDeliver}兵");

            if (pending.DaysRemaining <= 0 || pending.TotalTroopsToDeliver <= 0)
            {
                LogDebug($"[补兵] {GetHeroFactionPrefix(hero)}{hero.Name} 恢复全部完成");
                _pendingRestorations.Remove(hero);
            }
        }

        // ── 查询接口 ─────────────────────────────────────────────────────
        //
        // 返回给定王国当前正在等待补兵的领主总数（包含等候和正在补兵）。
        // 兼容旧存档和已有调用。

        public int GetPendingRestorationCount(Kingdom kingdom)
        {
            if (kingdom == null)
                return 0;

            return _pendingRestorations.Count(kvp =>
                kvp.Key.Clan?.Kingdom == kingdom);
        }

        /// <summary>
        /// 等候补兵人数：进入补兵队列但尚未实际开始接收部队的领主数量。
        /// 原因包括：无队伍（party==null）、功能被禁用但记录保留。
        /// </summary>
        public int GetWaitingRestorationCount(Kingdom kingdom)
        {
            if (kingdom == null) return 0;
            bool restorationEnabled = Settings.Instance?.RestorationEnabled ?? true;

            return _pendingRestorations.Count(kvp =>
                kvp.Key.Clan?.Kingdom == kingdom
                && (kvp.Key.PartyBelongedTo == null || !restorationEnabled));
        }

        /// <summary>
        /// 正在补兵人数：有队伍且功能开启，正在每日接收部队的领主数量。
        /// </summary>
        public int GetActiveRestorationCount(Kingdom kingdom)
        {
            if (kingdom == null) return 0;
            bool restorationEnabled = Settings.Instance?.RestorationEnabled ?? true;

            return _pendingRestorations.Count(kvp =>
                kvp.Key.Clan?.Kingdom == kingdom
                && kvp.Key.PartyBelongedTo != null
                && restorationEnabled
                && kvp.Value.DaysRemaining > 0
                && kvp.Value.TotalTroopsToDeliver > 0);
        }

        // ── 辅助方法 ─────────────────────────────────────────────────────

        private static string GetHeroFactionPrefix(Hero hero)
        {
            string kingdomName = hero.Clan?.Kingdom?.Name?.ToString();
            return string.IsNullOrEmpty(kingdomName) ? "" : $"{kingdomName}-";
        }

        private static int GetPartySizeLimit(Hero hero)
        {
            try
            {
                return Campaign.Current?.Models?.PartySizeLimitModel
                    ?.GetAssumedPartySizeForLordParty(hero, hero.MapFaction, hero.Clan) ?? 20;
            }
            catch
            {
                return 20;
            }
        }

        private static string DetermineCultureId(Hero hero)
        {
            CultureObject culture = hero.Clan?.Culture
                ?? hero.Clan?.Kingdom?.Culture
                ?? hero.Clan?.BasicTroop?.Culture;

            return culture?.StringId ?? string.Empty;
        }

        private static List<CharacterObject> GetTroopsForToday(PendingRestoration pending, int count)
        {
            var culture = MBObjectManager.Instance.GetObject<CultureObject>(pending.TroopCultureId);
            if (culture == null)
                return new List<CharacterObject>();

            var tier1to2 = GetAvailableTroops(culture, 1, 2);
            var tier3to4 = GetAvailableTroops(culture, 3, 4);
            var tier5to6 = GetAvailableTroops(culture, 5, 6);

            int count12 = (int)(count * pending.Tier12Ratio);
            int count34 = (int)(count * pending.Tier34Ratio);
            int count56 = count - count12 - count34;

            var result = new List<CharacterObject>();
            result.AddRange(PickRandom(tier1to2, count12));
            result.AddRange(PickRandom(tier3to4, count34));
            result.AddRange(PickRandom(tier5to6, count56));
            return result;
        }

        private static List<CharacterObject> GetAvailableTroops(
            CultureObject culture, int minTier, int maxTier)
        {
            var result = new List<CharacterObject>();
            var visited = new HashSet<CharacterObject>();
            var queue = new Queue<CharacterObject>();

            if (culture.BasicTroop != null)
                queue.Enqueue(culture.BasicTroop);
            if (culture.EliteBasicTroop != null)
                queue.Enqueue(culture.EliteBasicTroop);

            while (queue.Count > 0)
            {
                var troop = queue.Dequeue();
                if (!visited.Add(troop))
                    continue;

                if (troop.Tier >= minTier && troop.Tier <= maxTier)
                    result.Add(troop);

                if (troop.UpgradeTargets != null)
                {
                    foreach (var upgrade in troop.UpgradeTargets)
                    {
                        if (!visited.Contains(upgrade))
                            queue.Enqueue(upgrade);
                    }
                }
            }

            return result;
        }

        private static List<T> PickRandom<T>(List<T> source, int count)
        {
            if (source.Count == 0 || count <= 0)
                return new List<T>();

            var result = new List<T>();
            var rng = new Random();
            for (int i = 0; i < count; i++)
                result.Add(source[rng.Next(source.Count)]);

            return result;
        }

        // ── 调试日志（屏幕左下角）────────────────────────────────────────

        private static void LogDebug(string message)
        {
            if (Settings.Instance?.EnableRestorationDebugLog != true) return;
            InformationManager.DisplayMessage(
                new InformationMessage(message, Color.FromUint(0x00FFFFu)));
        }
    }
}
