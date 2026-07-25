using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
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
            if (Settings.Instance?.RestorationEnabled != true)
                return;

            if (prisoner == null || prisoner == Hero.MainHero || prisoner.IsPlayerCompanion)
                return;

            if (prisoner.Clan == null)
                return;

            int partySizeLimit = GetPartySizeLimit(prisoner);
            int totalTroops = (int)(partySizeLimit * (Settings.Instance?.RestorationPartySizeRatio ?? 0.6f));

            if (totalTroops <= 0)
                return;

            float t12 = Settings.Instance?.RestorationTier12Ratio ?? 0.50f;
            float t34 = Settings.Instance?.RestorationTier34Ratio ?? 0.35f;
            float t56 = Settings.Instance?.RestorationTier56Ratio ?? 0.15f;

            string cultureId = DetermineCultureId(prisoner);
            int days = Math.Max(1, (int)(Settings.Instance?.RestorationDays ?? 7f));
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
        }

        // ── 事件：英雄每日触发 ─────────────────────────────────────────────

        private void OnDailyTickHero(Hero hero)
        {
            if (Settings.Instance?.RestorationEnabled != true) return;

            if (!_pendingRestorations.TryGetValue(hero, out PendingRestoration pending))
                return;

            if (pending.DaysRemaining <= 0 || pending.TotalTroopsToDeliver <= 0)
            {
                _pendingRestorations.Remove(hero);
                return;
            }

            MobileParty party = hero.PartyBelongedTo;
            if (party == null)
                return; // 英雄还没有队伍 — 保留待办，明天再试

            // ── 交付部队 ──────────────────────────────────────────────
            int troopsToday = (pending.DaysRemaining == 1)
                ? pending.TotalTroopsToDeliver
                : pending.TroopsPerDay;

            if (troopsToday > 0 && pending.TotalTroopsToDeliver > 0)
            {
                List<CharacterObject> chosen = GetTroopsForToday(pending, troopsToday);
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

            if (pending.DaysRemaining <= 0 || pending.TotalTroopsToDeliver <= 0)
                _pendingRestorations.Remove(hero);
        }

        // ── 查询接口 ─────────────────────────────────────────────────────
        //
        // 返回给定王国当前正在等待补兵的领主数量。
        // 无论是尚未有队伍还是正在交付中的都计入统计。

        public int GetPendingRestorationCount(Kingdom kingdom)
        {
            if (kingdom == null)
                return 0;

            return _pendingRestorations.Count(kvp =>
                kvp.Key.Clan?.Kingdom == kingdom);
        }

        // ── 辅助方法 ─────────────────────────────────────────────────────

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
    }
}
