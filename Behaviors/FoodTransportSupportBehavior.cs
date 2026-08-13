using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace MutliLittleFixes.Behaviors
{
    /// <summary>
    /// 粮草运输支援行为。
    /// 玩家家族的富余城镇每 3 游戏小时检查一次,向缺粮的玩家家族城镇/城堡派出由驻军转化的运输队。
    /// 粮草直接加减 FoodStocks(不走市场消费转化);运输队携带抽象粮(支援)与实物粮(自身消耗,互不互通)。
    /// 运输队保持原版 AI(每小时思考开启,遭遇强敌会像商队一样逃跑);目标被围时原地等待(不禁用 AI)。
    /// </summary>
    public class FoodTransportSupportBehavior : CampaignBehaviorBase
    {
        private const int CheckIntervalHours = 3;

        private int _hourCounter;

        public override void RegisterEvents()
        {
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, OnHourlyTick);
            CampaignEvents.SettlementEntered.AddNonSerializedListener(this, OnSettlementEntered);
            CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(this, OnMobilePartyDestroyed);
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("_hourCounter", ref _hourCounter);
        }

        // ── 每 3 小时调度 ──────────────────────────────────────────────

        private void OnHourlyTick()
        {
            _hourCounter++;
            if (_hourCounter % CheckIntervalHours != 0)
            {
                return;
            }

            LogSupportSituationOverview();
            MaintainActiveTransports();
            DispatchNewTransports();
        }

        // ── 调试:每 3 小时输出全局面况快照(玩家家族城况 + 在途运输队)──

        private void LogSupportSituationOverview()
        {
            if (Settings.Instance?.EnableSupportDebugLog != true)
            {
                return;
            }

            Settings settings = Settings.Instance!;
            LogDebug($"[运粮] ── 3小时巡检 开关={settings.TransportSupportEnabled} 缺粮阈值={settings.TargetFoodThreshold} 源城阈值={settings.SourceFoodThreshold} 驻军阈值={settings.SourceGarrisonThreshold} 队规模={settings.TransportPartySize} ──");

            // 玩家家族每座城镇/城堡当前情况
            foreach (Settlement settlement in Settlement.All)
            {
                if (!settlement.IsTown && !settlement.IsCastle)
                {
                    continue;
                }
                if (settlement.OwnerClan != Clan.PlayerClan)
                {
                    continue;
                }
                Town town = settlement.Town;
                if (town == null)
                {
                    continue;
                }
                int garrison = town.GarrisonParty?.Party.NumberOfRegularMembers ?? 0;
                LogDebug($"[运粮] 城况 {settlement.Name}: 粮 {town.FoodStocks:F0}/{town.FoodStocksUpperLimit():F0} 驻军 {garrison} {(settlement.IsUnderSiege ? "被围" : "未围")}");
            }

            // 在途运输队情况
            bool hasTransports = false;
            foreach (MobileParty party in MobileParty.All)
            {
                if (party?.PartyComponent is not FoodTransportPartyComponent transport)
                {
                    continue;
                }
                if (transport.Phase == FoodTransportPartyComponent.TransportPhase.Done)
                {
                    continue;
                }
                hasTransports = true;
                string phaseText = transport.Phase switch
                {
                    FoodTransportPartyComponent.TransportPhase.TravelingToTarget => "前往",
                    FoodTransportPartyComponent.TransportPhase.Returning => "返程",
                    _ => "未知",
                };
                LogDebug($"[运粮] 在途 {GetPartyName(party)}: {phaseText} {transport.SourceSettlement?.Name}→{transport.TargetSettlement?.Name} 携粮 {transport.FoodCarried} 兵力 {party.Party.NumberOfAllMembers} 行为 {party.DefaultBehavior} 位置 {party.GetPosition2D}");
            }
            if (!hasTransports)
            {
                LogDebug("[运粮] 在途运输队: 无");
            }
        }

        // ── 在途运输队维护(围城等待/目标易主返程退款/源城丢失解散)──────

        private void MaintainActiveTransports()
        {
            foreach (MobileParty party in MobileParty.All)
            {
                if (party?.PartyComponent is not FoodTransportPartyComponent transport)
                {
                    continue;
                }
                if (transport.Phase == FoodTransportPartyComponent.TransportPhase.Done)
                {
                    continue;
                }

                Settlement source = transport.SourceSettlement;
                Settlement target = transport.TargetSettlement;

                // 源城已丢失(易主/不存在)→ 队伍解散,士兵损失,抽象粮不退回(已随源城失去)
                if (source == null || source.OwnerClan != Clan.PlayerClan)
                {
                    LogDebug($"[运粮] {GetPartyName(party)} 源城已失,运输队解散");
                    transport.Phase = FoodTransportPartyComponent.TransportPhase.Done;
                    DestroyPartyAction.Apply(null, party);
                    continue;
                }

                switch (transport.Phase)
                {
                    case FoodTransportPartyComponent.TransportPhase.TravelingToTarget:
                        // 目标被攻占(易主)→ 返程,抽象粮全额退回源城
                        if (target == null || target.OwnerClan != Clan.PlayerClan)
                        {
                            RefundAbstractFood(source, transport);
                            transport.Phase = FoodTransportPartyComponent.TransportPhase.Returning;
                            SetPartyAiAction.GetActionForVisitingSettlement(party, source, MobileParty.NavigationType.Default, false, false);
                            LogDebug($"[运粮] {GetPartyName(party)} 目标已失,返程退回 {transport.FoodCarried} 粮");
                            continue;
                        }
                        // 目标被围 → 原地等待(AI 保持开启,仍会像商队一样躲避敌军);
                        // 围城解除后重新下发目标继续赶路
                        if (target.IsUnderSiege)
                        {
                            party.SetMoveModeHold();
                        }
                        else if (party.DefaultBehavior != AiBehavior.GoToSettlement)
                        {
                            SetPartyAiAction.GetActionForVisitingSettlement(party, target, MobileParty.NavigationType.Default, false, false);
                        }
                        break;

                    case FoodTransportPartyComponent.TransportPhase.Returning:
                        // 返程回收由 SettlementEntered(回到源城)事件处理
                        break;
                }
            }
        }

        // ── 调度新运输队 ───────────────────────────────────────────────

        private void DispatchNewTransports()
        {
            Settings settings = Settings.Instance;
            if (settings?.TransportSupportEnabled != true)
            {
                return;
            }

            int maxSupporters = Math.Max(1, settings.MaxSupportingTownsPerTarget);

            // 统计在途运输队:各源城外派数 / 各目标被支援数 / 在途(源,目标)对
            Dictionary<Settlement, int> outgoingPerSource = new Dictionary<Settlement, int>();
            Dictionary<Settlement, int> incomingPerTarget = new Dictionary<Settlement, int>();
            HashSet<KeyValuePair<Settlement, Settlement>> activePairs = new HashSet<KeyValuePair<Settlement, Settlement>>();
            foreach (MobileParty party in MobileParty.All)
            {
                if (party?.PartyComponent is not FoodTransportPartyComponent transport)
                {
                    continue;
                }
                if (transport.Phase == FoodTransportPartyComponent.TransportPhase.Done)
                {
                    continue;
                }
                if (transport.SourceSettlement != null && transport.TargetSettlement != null)
                {
                    outgoingPerSource[transport.SourceSettlement] = CountOf(outgoingPerSource, transport.SourceSettlement) + 1;
                    incomingPerTarget[transport.TargetSettlement] = CountOf(incomingPerTarget, transport.TargetSettlement) + 1;
                    activePairs.Add(new KeyValuePair<Settlement, Settlement>(transport.SourceSettlement, transport.TargetSettlement));
                }
            }

            // 目标候选:玩家家族城镇/城堡,未被围城,粮草低于阈值,按缺口从大到小(粮草最少优先)
            List<Settlement> targets = new List<Settlement>();
            foreach (Settlement settlement in Settlement.All)
            {
                if (!settlement.IsTown && !settlement.IsCastle)
                {
                    continue;
                }
                if (settlement.OwnerClan != Clan.PlayerClan)
                {
                    continue;
                }
                if (settlement.IsUnderSiege)
                {
                    continue;
                }
                Town town = settlement.Town;
                if (town == null || town.FoodStocks >= settings.TargetFoodThreshold)
                {
                    continue;
                }
                if (CountOf(incomingPerTarget, settlement) >= maxSupporters)
                {
                    continue;
                }
                targets.Add(settlement);
            }
            targets.Sort((a, b) => a.Town.FoodStocks.CompareTo(b.Town.FoodStocks));

            foreach (Settlement target in targets)
            {
                int incoming = CountOf(incomingPerTarget, target);
                while (incoming < maxSupporters)
                {
                    Settlement? source = PickSourceTown(target, outgoingPerSource, activePairs);
                    if (source == null)
                    {
                        break;
                    }
                    if (TryDispatchTransport(source, target, settings))
                    {
                        incoming++;
                        outgoingPerSource[source] = CountOf(outgoingPerSource, source) + 1;
                        incomingPerTarget[target] = incoming;
                        activePairs.Add(new KeyValuePair<Settlement, Settlement>(source, target));
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>从玩家家族城镇中选距目标最近的可用来源(未被围、粮草/驻军达标、未超外派上限、未在支援同一目标)。</summary>
        private Settlement? PickSourceTown(Settlement target, Dictionary<Settlement, int> outgoingPerSource, HashSet<KeyValuePair<Settlement, Settlement>> activePairs)
        {
            Settings settings = Settings.Instance;
            float sourceFoodThreshold = settings?.SourceFoodThreshold ?? 200f;
            int sourceGarrisonThreshold = settings?.SourceGarrisonThreshold ?? 150;
            int maxOutgoing = Math.Max(1, settings?.MaxOutgoingTransportsPerTown ?? 2);

            Settlement? best = null;
            float bestDistance = float.MaxValue;
            foreach (Settlement settlement in Settlement.All)
            {
                if (!settlement.IsTown)
                {
                    continue; // 城堡不发起支援
                }
                if (settlement.OwnerClan != Clan.PlayerClan)
                {
                    continue;
                }
                if (settlement.IsUnderSiege)
                {
                    continue;
                }
                Town town = settlement.Town;
                if (town == null || town.FoodStocks < sourceFoodThreshold)
                {
                    continue;
                }
                MobileParty garrison = town.GarrisonParty;
                if (garrison == null || garrison.Party.NumberOfRegularMembers < sourceGarrisonThreshold)
                {
                    continue;
                }
                if (CountOf(outgoingPerSource, settlement) >= maxOutgoing)
                {
                    continue;
                }
                if (activePairs.Contains(new KeyValuePair<Settlement, Settlement>(settlement, target)))
                {
                    continue; // 同(源,目标)对已在途,不重复发起
                }
                float distance = settlement.Position.Distance(target.Position);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    best = settlement;
                }
            }
            return best;
        }

        private bool TryDispatchTransport(Settlement source, Settlement target, Settings settings)
        {
            Town sourceTown = source.Town;
            Town targetTown = target.Town;
            if (sourceTown == null || targetTown == null)
            {
                return false;
            }

            int partySize = Math.Max(1, settings.TransportPartySize);
            int foodPerTroop = Math.Max(0, settings.FoodPerTroop);
            int physicalPerTroop = Math.Max(0, settings.PhysicalFoodPerTroop);

            // 抽象粮:不超过源城"超出支援阈值"的余量,也不扣到 0 以下
            int maxFood = partySize * foodPerTroop;
            int spareFood = (int)sourceTown.FoodStocks - Math.Max(0, settings.SourceFoodThreshold);
            // int foodCarried = Math.Min(maxFood, Math.Max(0, spareFood));
            int foodCarried = maxFood; // 直接按最大值运送,不考虑源城余粮,避免频繁触发支援
            if (foodCarried <= 0)
            {
                return false;
            }

            // 抽兵:高低级各半,半区内随机;不足时从另一半补齐
            List<CharacterObject> chosen = PickTransportTroops(sourceTown, partySize);
            if (chosen.Count < partySize)
            {
                return false;
            }

            TroopRoster garrisonRoster = sourceTown.GarrisonParty.MemberRoster;
            foreach (CharacterObject character in chosen)
            {
                garrisonRoster.AddToCounts(character, -1);
            }

            TroopRoster transportRoster = TroopRoster.CreateDummyTroopRoster();
            foreach (CharacterObject character in chosen)
            {
                transportRoster.AddToCounts(character, 1);
            }

            FoodTransportPartyComponent component = new FoodTransportPartyComponent(source, target, foodCarried, transportRoster);
            MobileParty party = MobileParty.CreateParty("food_transport_party", component);

            // 实物粮:凭空生成入队背包(不从源城市场取),仅供队伍途中自用
            int physicalFood = partySize * physicalPerTroop;
            if (physicalFood > 0)
            {
                party.ItemRoster.AddToCounts(DefaultItems.Grain, physicalFood);
            }

            // 先入源城,再出发前往目标
            EnterSettlementAction.ApplyForParty(party, source);
            SetPartyAiAction.GetActionForVisitingSettlement(party, target, MobileParty.NavigationType.Default, false, false);

            // 扣源城抽象粮(保证不低于 0)
            sourceTown.FoodStocks = Math.Max(0f, sourceTown.FoodStocks - foodCarried);

            NotifyPlayer(
                new TextObject("{=mlf_food_dispatched}{SOURCE_NAME} has dispatched a {PARTY_SIZE}-man food transport to support {TARGET_NAME} ({FOOD_AMOUNT} food)")
                .SetTextVariable("SOURCE_NAME", source.Name)
                .SetTextVariable("PARTY_SIZE", partySize)
                .SetTextVariable("TARGET_NAME", target.Name)
                .SetTextVariable("FOOD_AMOUNT", foodCarried)
                .ToString());
            return true;
        }

        /// <summary>从驻军中按"高级/低级各半、半区内随机"抽取指定人数,返回实际抽到的士兵(可能不足)。</summary>
        private static List<CharacterObject> PickTransportTroops(Town sourceTown, int partySize)
        {
            List<CharacterObject> low = new List<CharacterObject>();
            List<CharacterObject> high = new List<CharacterObject>();
            TroopRoster? garrisonRoster = sourceTown.GarrisonParty?.MemberRoster;
            if (garrisonRoster != null)
            {
                for (int i = 0; i < garrisonRoster.Count; i++)
                {
                    TroopRosterElement element = garrisonRoster.GetElementCopyAtIndex(i);
                    if (element.Number <= 0 || element.Character == null || element.Character.IsHero)
                    {
                        continue;
                    }
                    if (element.Character.Tier <= 3)
                    {
                        low.Add(element.Character);
                    }
                    else
                    {
                        high.Add(element.Character);
                    }
                }
            }

            List<CharacterObject> chosen = new List<CharacterObject>();
            int wantLow = partySize / 2;
            int wantHigh = partySize - wantLow;
            int takenLow = PickRandomInto(chosen, low, wantLow);
            int takenHigh = PickRandomInto(chosen, high, wantHigh);
            if (takenLow < wantLow)
            {
                PickRandomInto(chosen, high, wantLow - takenLow);
            }
            if (takenHigh < wantHigh)
            {
                PickRandomInto(chosen, low, wantHigh - takenHigh);
            }
            return chosen;
        }

        private static int PickRandomInto(List<CharacterObject> target, List<CharacterObject> source, int count)
        {
            if (source.Count == 0 || count <= 0)
            {
                return 0;
            }
            int taken = 0;
            for (int i = 0; i < count; i++)
            {
                target.Add(source[MBRandom.RandomInt(source.Count)]);
                taken++;
            }
            return taken;
        }

        // ── 交付 / 回收 ────────────────────────────────────────────────

        private void OnSettlementEntered(MobileParty party, Settlement settlement, Hero hero)
        {
            if (party?.PartyComponent is not FoodTransportPartyComponent transport)
            {
                return;
            }
            if (transport.Phase == FoodTransportPartyComponent.TransportPhase.Done)
            {
                return;
            }

            if (transport.Phase == FoodTransportPartyComponent.TransportPhase.TravelingToTarget && settlement == transport.TargetSettlement)
            {
                DeliverFood(party, transport, settlement);
            }
            else if (transport.Phase == FoodTransportPartyComponent.TransportPhase.Returning && settlement == transport.SourceSettlement)
            {
                ReturnAndDisband(party, transport, settlement);
            }
        }

        private void DeliverFood(MobileParty party, FoodTransportPartyComponent transport, Settlement target)
        {
            Town town = target.Town;
            if (town == null)
            {
                return;
            }
            // 正常流程被围城时无法进城;若事件仍触发(罕见),转为等待
            if (target.IsUnderSiege)
            {
                party.SetMoveModeHold();
                return;
            }

            // 抽象粮直接加入目标城 FoodStocks,超出上限部分销毁浪费
            float upper = town.FoodStocksUpperLimit();
            town.FoodStocks += transport.FoodCarried;
            if (town.FoodStocks > upper)
            {
                town.FoodStocks = upper;
            }

            // 销毁剩余实物粮(不进市场)
            DestroyPhysicalFood(party);

            transport.Phase = FoodTransportPartyComponent.TransportPhase.Returning;
            SetPartyAiAction.GetActionForVisitingSettlement(party, transport.SourceSettlement, MobileParty.NavigationType.Default, false, false);

            LogDebug($"[运粮] {GetPartyName(party)} 抵达 {target.Name},交付 {transport.FoodCarried} 粮,返程");
            NotifyPlayer(
                new TextObject("{=mlf_food_delivered}{TARGET_NAME} received {FOOD_AMOUNT} food support")
                .SetTextVariable("TARGET_NAME", target.Name)
                .SetTextVariable("FOOD_AMOUNT", transport.FoodCarried)
                .ToString());
        }

        private void ReturnAndDisband(MobileParty party, FoodTransportPartyComponent transport, Settlement source)
        {
            // 剩余士兵归还驻军
            Town town = source.Town;
            if (town != null && town.GarrisonParty != null)
            {
                TroopRoster memberRoster = party.MemberRoster;
                for (int i = 0; i < memberRoster.Count; i++)
                {
                    TroopRosterElement element = memberRoster.GetElementCopyAtIndex(i);
                    if (element.Number > 0 && element.Character != null && !element.Character.IsHero)
                    {
                        town.GarrisonParty.MemberRoster.AddToCounts(element.Character, element.Number);
                    }
                }
            }

            // 销毁剩余实物粮
            DestroyPhysicalFood(party);

            transport.Phase = FoodTransportPartyComponent.TransportPhase.Done;
            DestroyPartyAction.Apply(null, party);

            LogDebug($"[运粮] {source.Name} 运输队返程回收,士兵归队");
        }

        private void OnMobilePartyDestroyed(MobileParty party, PartyBase destroyerParty)
        {
            if (party?.PartyComponent is not FoodTransportPartyComponent transport)
            {
                return;
            }
            if (transport.Phase == FoodTransportPartyComponent.TransportPhase.Done)
            {
                return; // 主动解散(回收/源城丢失),非战斗摧毁
            }
            // 被摧毁:抽象粮不退回(随队损失),士兵损失
            LogDebug($"[运粮] {GetPartyName(party)} 运输队被摧毁,{transport.FoodCarried} 粮草随队损失");
            NotifyPlayer(
                new TextObject("{=mlf_food_destroyed}{PARTY_NAME} was destroyed on the way. Food was lost.")
                .SetTextVariable("PARTY_NAME", GetPartyName(party))
                .ToString());
        }

        // ── 辅助方法 ───────────────────────────────────────────────────

        private static void RefundAbstractFood(Settlement source, FoodTransportPartyComponent transport)
        {
            Town town = source.Town;
            if (town == null)
            {
                return;
            }
            town.FoodStocks += transport.FoodCarried;
            float upper = town.FoodStocksUpperLimit();
            if (town.FoodStocks > upper)
            {
                town.FoodStocks = upper;
            }
        }

        private static void DestroyPhysicalFood(MobileParty party)
        {
            ItemRoster roster = party.ItemRoster;
            for (int i = roster.Count - 1; i >= 0; i--)
            {
                ItemRosterElement element = roster.GetElementCopyAtIndex(i);
                if (element.Amount > 0 && element.EquipmentElement.Item != null && element.EquipmentElement.Item.IsFood)
                {
                    roster.AddToCounts(element.EquipmentElement, -element.Amount);
                }
            }
        }

        private static int CountOf(Dictionary<Settlement, int> counts, Settlement key)
        {
            return counts.TryGetValue(key, out int value) ? value : 0;
        }

        private static string GetPartyName(MobileParty party)
        {
            return party.Name?.ToString() ?? "运输队";
        }

        private static void LogDebug(string message)
        {
            if (Settings.Instance?.EnableSupportDebugLog != true)
            {
                return;
            }
            InformationManager.DisplayMessage(new InformationMessage(message, Color.FromUint(0x00FFFFu)));
        }

        private static void NotifyPlayer(string message)
        {
            InformationManager.DisplayMessage(new InformationMessage(message, Color.FromUint(0xFFD700u)));
        }
    }
}
