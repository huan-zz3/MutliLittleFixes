using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Settlements;

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
        /// 如果王国没有数据或该功能已禁用，则返回 0。
        /// </summary>
        public float GetTerritoryBonus(Kingdom kingdom)
        {
            if (Settings.Instance?.TerritoryBonusEnabled != true) return 0f;
            if (kingdom == null) return 0f;
            if (_kingdomAccumulators.TryGetValue(kingdom, out var data))
                return Math.Max(0f, data.AccumulatedBonus);
            return 0f;
        }

        private void OnKingdomCreated(Kingdom kingdom)
        {
            if (kingdom == null) return;
            if (!_kingdomAccumulators.ContainsKey(kingdom))
            {
                _kingdomAccumulators[kingdom] = new KingdomTerritoryData
                {
                    AccumulatedBonus = 0f,
                    TownsLost = 0,
                    CastlesLost = 0,
                    TownsConquered = 0,
                    CastlesConquered = 0
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
            if (Settings.Instance?.TerritoryBonusEnabled != true) return;

            // 只追踪要塞（城镇和城堡），不追踪村庄
            if (!settlement.IsTown && !settlement.IsCastle) return;

            Kingdom oldKingdom = oldOwner?.Clan?.Kingdom;
            Kingdom newKingdom = newOwner?.Clan?.Kingdom;

            // 同王国内部交易 = 不变
            if (oldKingdom == newKingdom && oldKingdom != null) return;

            bool isTown = settlement.IsTown;

            // 旧王国丢失定居点 → 加成增加
            if (oldKingdom != null)
            {
                EnsureKingdomData(oldKingdom);
                var data = _kingdomAccumulators[oldKingdom];

                float baseIncrement = isTown
                    ? (Settings.Instance?.TerritoryBonusTownValue ?? 5f)
                    : (Settings.Instance?.TerritoryBonusCastleValue ?? 3f);

                float diminishRate = Settings.Instance?.TerritoryBonusDiminishRate ?? 0.85f;
                int totalLosses = data.TownsLost + data.CastlesLost;
                float diminishing = (float)Math.Pow(diminishRate, totalLosses);
                float effectiveIncrement = baseIncrement * diminishing;

                float maxCap = Settings.Instance?.TerritoryBonusMaxCap ?? 200f;
                data.AccumulatedBonus = Math.Min(maxCap, data.AccumulatedBonus + effectiveIncrement);

                if (isTown) data.TownsLost++;
                else data.CastlesLost++;
            }

            // 新王国征服定居点 → 加成减少
            if (newKingdom != null)
            {
                EnsureKingdomData(newKingdom);
                var data = _kingdomAccumulators[newKingdom];

                float reduction = isTown
                    ? (Settings.Instance?.TerritoryBonusTownReduction ?? 5f)
                    : (Settings.Instance?.TerritoryBonusCastleReduction ?? 3f);

                data.AccumulatedBonus = Math.Max(0f, data.AccumulatedBonus - reduction);

                if (isTown) data.TownsConquered++;
                else data.CastlesConquered++;
            }
        }

        private void EnsureKingdomData(Kingdom kingdom)
        {
            if (!_kingdomAccumulators.ContainsKey(kingdom))
            {
                _kingdomAccumulators[kingdom] = new KingdomTerritoryData
                {
                    AccumulatedBonus = 0f,
                    TownsLost = 0,
                    CastlesLost = 0,
                    TownsConquered = 0,
                    CastlesConquered = 0
                };
            }
        }
    }
}
