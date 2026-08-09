using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;

namespace MutliLittleFixes.Patches
{
    /// <summary>
    /// 玩家打下城池后，分封投票中强制保证玩家出现在候选名单。
    ///
    /// 判定规则（严格）：
    /// - 玩家作为军团领袖带队攻城 ✓
    /// - 玩家单独部队攻城         ✓
    /// - 玩家在他人军团中作成员   ✗
    /// - 同伴带队攻城             ✗
    ///
    /// 策略：
    ///   1) Patch ApplyBySiege 记录精确的玩家身份（运行时字典）
    ///   2) Patch NarrowDownCandidates 在筛完前 3 名后强制追加玩家
    ///   3) 读档后字典为空时，fallback 到 Town.LastCapturedBy
    /// </summary>
    internal static class PlayerCapturedFiefPatch
    {
        /// <summary>
        /// 记录每个打下城池的部队是否由玩家亲自率领。
        /// Key:  被打下的 Settlement
        /// Value: true = 玩家带队打下
        ///
        /// 同步于 ApplyBySiege，不在存档中持久化。
        /// 读档后 fallback 到 Town.LastCapturedBy。
        /// </summary>
        private static readonly Dictionary<Settlement, bool> _playerCapturedSettlements = new();

        // ═══════════════════════════════════════════════════════
        //  Patch 1 — 在攻城完成时记录玩家身份
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// prefix: ChangeOwnerOfSettlementAction.ApplyBySiege
        ///
        /// capturerHero == Hero.MainHero 当且仅当玩家作为军团领袖
        /// 或单独部队的领袖打下此城。同伴带队时 capturerHero 是同伴，
        /// 不满足条件。
        /// </summary>
        internal static void RecordPlayerCaptured(
            Hero newOwner,
            Hero capturerHero,
            Settlement settlement)
        {
            // MCM 运行时开关 — 关闭时不记录
            if (Settings.Instance?.PlayerFiefCandidacyEnabled != true)
                return;

            bool playerWasCapturer = capturerHero == Hero.MainHero;

            if (playerWasCapturer)
            {
                _playerCapturedSettlements[settlement] = true;
            }
            else
            {
                _playerCapturedSettlements.Remove(settlement);
            }
        }

        // ═══════════════════════════════════════════════════════
        //  Patch 2 — 分封投票筛选候选时确保玩家在名单中
        // ═══════════════════════════════════════════════════════

        /// <summary>
        /// postfix: KingdomDecision.NarrowDownCandidates
        ///
        /// 原版 NarrowDownCandidates 按 CalculateMeritOfOutcome 排序后
        /// 只保留前 3 名。此 postfix 检测：
        ///   - 这是否是 SettlementClaimantDecision（分封投票）
        ///   - 城池是否由玩家打下
        /// 如果玩家被筛掉了，强制追加到结果中。
        /// </summary>
        internal static void EnsurePlayerIsCandidate(
            KingdomDecision __instance,
            MBList<DecisionOutcome> __result,
            MBList<DecisionOutcome> initialCandidates,
            int maxCandidateCount)
        {
            // MCM 运行时开关 — 关闭时不干预
            if (Settings.Instance?.PlayerFiefCandidacyEnabled != true)
                return;

            if (__instance is SettlementClaimantDecision scd)
            {
                AddPlayerToCandidatesIfNeeded(scd, __result, initialCandidates);
            }
        }

        /// <summary>
        /// 核心逻辑：判断并追加玩家候选。
        /// </summary>
        private static void AddPlayerToCandidatesIfNeeded(
            SettlementClaimantDecision scd,
            MBList<DecisionOutcome> narrowedCandidates,
            MBList<DecisionOutcome> initialCandidates)
        {
            Settlement settlement = scd.Settlement;

            // 非城镇/城堡的城池无法被分配，不做操作
            if (settlement?.Town == null)
                return;

            // ── 判定玩家是否打下此城 ──────────────────────────
            bool playerCaptured;

            // 优先使用运行时精确记录（攻城当刻捕获的身份）
            if (_playerCapturedSettlements.TryGetValue(settlement, out bool captured))
            {
                playerCaptured = captured;
            }
            else
            {
                // fallback: 读档后字典为空，使用持久化字段
                // 注意：LastCapturedBy 是 Clan 级别，同伴带队也等于 PlayerClan，
                // 但这是读档后我们能拿到的最佳近似
                playerCaptured = settlement.Town.LastCapturedBy == Clan.PlayerClan;
            }

            if (!playerCaptured)
                return;

            Clan playerClan = Clan.PlayerClan;

            // ── 检查玩家是否已在结果列表中 ─────────────────────
            foreach (var outcome in narrowedCandidates)
            {
                if (outcome is SettlementClaimantDecision.ClanAsDecisionOutcome clanOutcome
                    && clanOutcome.Clan == playerClan)
                {
                    return; // 已经在名单中，无需操作
                }
            }

            // ── 从原始候选列表中找回玩家的 outcome 并替换第3名 ──
            // 原版 NarrowDownCandidates 只保留 top 3，直接追加会变成第4名
            // 导致 UI 显示错位。因此先移除末位（最低分），再追加玩家。
            for (int i = narrowedCandidates.Count - 1; i >= 0; i--)
            {
                if (narrowedCandidates[i] is SettlementClaimantDecision.ClanAsDecisionOutcome clanOutcome
                    && clanOutcome.Clan == playerClan)
                {
                    return; // 玩家已在结果中（虽然 count > maxCandidateCount，兜底处理）
                }
            }

            foreach (var outcome in initialCandidates)
            {
                if (outcome is SettlementClaimantDecision.ClanAsDecisionOutcome clanOutcome
                    && clanOutcome.Clan == playerClan)
                {
                    // 移除末位（当前第3名），追加玩家 -> 玩家成为新的第3名
                    narrowedCandidates.RemoveAt(narrowedCandidates.Count - 1);
                    narrowedCandidates.Add(outcome);
                    return;
                }
            }
        }
    }
}
