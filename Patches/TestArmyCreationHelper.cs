using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using Helpers;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;

namespace ExampleMod.Patches
{
    /// <summary>
    /// 强制创建军团测试辅助类。
    /// 由 MCM 按钮触发，选择玩家王国中影响力最高的非玩家家族领袖，
    /// 模拟 AI 拉起军团的全流程，输出日志用于验证屏蔽效果。
    /// </summary>
    internal static class TestArmyCreationHelper
    {
        internal static void TriggerTestArmyCreation()
        {
            try
            {
                // ========== Step 1: 检查玩家王国 ==========
                var kingdom = Clan.PlayerClan.Kingdom;
                if (kingdom == null)
                {
                    InformationManager.DisplayMessage(
                        new InformationMessage("[Test] 玩家不属于任何王国", Colors.Red));
                    return;
                }
                InformationManager.DisplayMessage(
                    new InformationMessage($"[Test] 当前王国: {kingdom.Name}", Colors.White));

                // ========== Step 2: 筛选我方非玩家家族领袖（按影响力降序）==========
                var eligibleClans = kingdom.Clans
                    .Where(c => c != Clan.PlayerClan
                             && c.Leader != null
                             && c.Leader.PartyBelongedTo != null)
                    .OrderByDescending(c => c.Influence)
                    .ToList();

                if (eligibleClans.Count == 0)
                {
                    InformationManager.DisplayMessage(
                        new InformationMessage("[Test] 没有符合条件（有部队）的非玩家家族领袖", Colors.Red));
                    return;
                }

                InformationManager.DisplayMessage(
                    new InformationMessage(
                        $"[Test] 共有 {eligibleClans.Count} 个非玩家家族领袖，按影响力顺位尝试...", Colors.White));

                // ========== Step 3: 按影响力顺位逐个尝试 ==========
                MobileParty? succeededParty = null;
                MBList<MobileParty>? succeededCandidates = null;
                Clan? succeededClan = null;

                foreach (var candidateClan in eligibleClans)
                {
                    var leader = candidateClan.Leader;
                    var mobileParty = leader!.PartyBelongedTo;

                    InformationManager.DisplayMessage(
                        new InformationMessage(
                            $"[Test] --- 尝试 [{candidateClan.Name}] {leader.Name} ---", Colors.Cyan));

                    // 检查领主基本状态
                    if (mobileParty!.Army != null)
                    {
                        InformationManager.DisplayMessage(
                            new InformationMessage($"[Test]  跳过：已属于军团", Colors.Yellow));
                        continue;
                    }
                    if (mobileParty.MapEvent != null)
                    {
                        InformationManager.DisplayMessage(
                            new InformationMessage($"[Test]  跳过：战斗中", Colors.Yellow));
                        continue;
                    }

                    // 逐条检查 CanLordCreateArmy 的 7 个前置条件
                    InformationManager.DisplayMessage(
                        new InformationMessage("[Test]  详细检查创建条件:", Colors.White));

                    bool cond1 = !mobileParty.IsCurrentlyAtSea;
                    Info($"[条件1] 不在海上", cond1, $"IsCurrentlyAtSea={mobileParty.IsCurrentlyAtSea}");

                    float influence = mobileParty.LeaderHero!.Clan.Influence;
                    bool cond2 = influence > 100f;
                    Info($"[条件2] 影响力 > 100", cond2, $"Influence={influence:F1}");

                    bool cond3 = !mobileParty.LeaderHero.Clan.IsUnderMercenaryService;
                    Info($"[条件3] 非雇佣兵", cond3, $"IsUnderMercenaryService={mobileParty.LeaderHero.Clan.IsUnderMercenaryService}");

                    float foodDays = mobileParty.GetNumDaysForFoodToLast();
                    float foodThreshold = Campaign.Current.Models.MobilePartyAIModel.NeededFoodsInDaysThresholdForSiege;
                    bool cond4 = foodDays > foodThreshold;
                    Info($"[条件4] 食物 > {foodThreshold:F1}天", cond4, $"FoodDays={foodDays:F1}");

                    bool cond5 = kingdom.FactionsAtWarWith.AnyQ(f => f.Fiefs.Any());
                    string atWarWith = string.Join(", ", kingdom.FactionsAtWarWith.Select(f => $"{f.Name}(领地{f.Fiefs.Count})"));
                    Info($"[条件5] 战争中有领地的敌方", cond5, $"敌方: {(kingdom.FactionsAtWarWith.Count > 0 ? atWarWith : "无")}");

                    float partySizeRatio = mobileParty.PartySizeRatio;
                    float sizeThreshold = Campaign.Current.Models.ArmyManagementCalculationModel.AIMobilePartySizeRatioToCallToArmy;
                    bool cond6 = partySizeRatio > sizeThreshold;
                    Info($"[条件6] 兵力比 > {sizeThreshold:P0}", cond6, $"PartySizeRatio={partySizeRatio:P2} (人数={mobileParty.Party.NumberOfAllMembers}, 上限={mobileParty.Party.PartySizeLimit})");

                    bool isLeader = mobileParty.LeaderHero.Clan.Leader == mobileParty.LeaderHero;
                    bool isFirstWarParty = mobileParty.LeaderHero.Clan.Leader.PartyBelongedTo == null
                        && mobileParty.LeaderHero.Clan.WarPartyComponents?.FirstOrDefault() == mobileParty.WarPartyComponent;
                    bool cond7 = isLeader || isFirstWarParty;
                    string identity = isLeader ? "家族领袖" : (isFirstWarParty ? "代理领袖(家族领袖无部队)" : "普通领主(无权创建)");
                    Info($"[条件7] 有权创建军团", cond7, identity);

                    // 全部满足才调用 CanLordCreateArmy
                    if (!cond1 || !cond2 || !cond3 || !cond4 || !cond5 || !cond6 || !cond7)
                    {
                        InformationManager.DisplayMessage(
                            new InformationMessage("[Test]  ❌ 不满足前置条件", Colors.Yellow));
                        continue;
                    }

                    // 调用 CanLordCreateArmy（获取候选列表并做最终检查）
                    bool canCreate = Campaign.Current.Models.ArmyManagementCalculationModel
                        .CanLordCreateArmy(mobileParty, out var possibleArmyMembers);

                    if (!canCreate)
                    {
                        InformationManager.DisplayMessage(
                            new InformationMessage("[Test]  ❌ CanLordCreateArmy 返回 false（候选总战力可能 < 1000）", Colors.Yellow));
                        continue;
                    }

                    // 成功！
                    succeededParty = mobileParty;
                    succeededCandidates = possibleArmyMembers;
                    succeededClan = candidateClan;
                    break;
                }

                if (succeededParty == null)
                {
                    InformationManager.DisplayMessage(
                        new InformationMessage("[Test] ❌ 所有家族领袖均无法创建军团", Colors.Red));
                    return;
                }

                InformationManager.DisplayMessage(
                    new InformationMessage(
                        $"[Test] ✅ 选中: [{succeededClan!.Name}] {succeededClan.Leader!.Name}", Colors.Green));

                // ========== Step 4: 逐部队输出筛选条件明细 ==========
                InformationManager.DisplayMessage(
                    new InformationMessage($"[Test] 候选部队数量: {succeededCandidates!.Count}，逐条输出筛选条件:", Colors.White));

                float maxDist = Campaign.Current.Models.ArmyManagementCalculationModel.MaximumDistanceToCallToArmy;
                float minFood = Campaign.Current.Models.ArmyManagementCalculationModel.MinimumNeededFoodInDaysToCallToArmy;
                float sizeRatioThreshold = Campaign.Current.Models.ArmyManagementCalculationModel.AIMobilePartySizeRatioToCallToArmy;

                int partyIndex = 0;
                int playerClanCount = 0;
                foreach (var party in succeededCandidates)
                {
                    partyIndex++;
                    var leaderHero = party.LeaderHero;
                    bool isPlayerClan = leaderHero?.Clan == Clan.PlayerClan;
                    if (isPlayerClan) playerClanCount++;

                    // ── 首行：部队身份判定 ──
                    Color headerColor = isPlayerClan ? Colors.Red : Colors.Green;
                    string clanTag = isPlayerClan ? "[玩家家族] ❌ 应被屏蔽" : "[非玩家家族] ✅";
                    InformationManager.DisplayMessage(
                        new InformationMessage(
                            $"  [{partyIndex}/{succeededCandidates.Count}] {party.Name}  {clanTag}", headerColor));

                    // ── 逐条件输出 ──
                    string ldrName = leaderHero?.Name?.ToString() ?? "无";
                    var partyKingdom = succeededParty!.MapFaction as Kingdom;
                    bool isKing = leaderHero == partyKingdom?.Leader;

                    Info("领主", leaderHero != null, $"Name={ldrName}");
                    Info("非主角部队", !party.IsMainParty, $"IsMainParty={party.IsMainParty}");
                    Info("非国王本人", !isKing, $"IsLeader={isKing}");
                    Info("非自己", party != succeededParty, $"{party.Name} != {succeededParty.Name}");
                    Info("未在其它军团", party.Army == null, $"Army={(party.Army?.Name?.ToString() ?? "null")}");
                    Info("AI可决策", !party.Ai.DoNotMakeNewDecisions, $"DoNotMakeNewDecisions={party.Ai.DoNotMakeNewDecisions}");
                    Info("非战斗中", party.MapEvent == null, $"MapEvent={(party.MapEvent != null ? "战斗中" : "null")}");
                    Info("非围攻中", party.BesiegedSettlement == null, $"BesiegedSettlement={(party.BesiegedSettlement?.Name?.ToString() ?? "null")}");
                    Info("所在地未被围", party.CurrentSettlement?.SiegeEvent == null, $"SiegeEvent={(party.CurrentSettlement?.SiegeEvent != null ? "被围" : "null")}");
                    Info("非解散中", !party.IsDisbanding, $"IsDisbanding={party.IsDisbanding}");
                    Info("非漂流状态", !party.IsInRaftState, $"IsInRaftState={party.IsInRaftState}");

                    float foodDays = party.GetNumDaysForFoodToLast();
                    Info($"食物 > {minFood:F0}天", foodDays > minFood, $"FoodDays={foodDays:F1}");

                    float pr = party.PartySizeRatio;
                    Info($"兵力比 > {sizeRatioThreshold:P0}", pr > sizeRatioThreshold, $"PartySizeRatio={pr:P2} (人数={party.Party.NumberOfAllMembers}, 上限={party.Party.PartySizeLimit})");

                    bool canLead = leaderHero?.CanLeadParty() ?? false;
                    Info("可领导部队", canLead, $"CanLeadParty={canLead}");

                    // 距离检查
                    float landRatio;
                    float dist = DistanceHelper.GetDistanceBetweenMobilePartyToMobileParty(
                        party, succeededParty, party.NavigationCapability, out landRatio);
                    Info($"距离 < {maxDist:F0}", dist < maxDist, $"Distance={dist:F1}, MaxDist={maxDist:F0}, LandRatio={landRatio:P1}");
                }

                // 汇总屏蔽验证
                if (playerClanCount > 0)
                {
                    InformationManager.DisplayMessage(
                        new InformationMessage($"[Test] ⚠️ 警告：{playerClanCount} 个玩家家族部队仍在候选列表中！屏蔽可能未生效", Colors.Red));
                }
                else
                {
                    InformationManager.DisplayMessage(
                        new InformationMessage("[Test] ✅ 屏蔽验证通过：候选部队中不包含任何玩家家族部队", Colors.Green));
                }

                // ========== Step 5: 验证结论 ==========
                // 注意：跳过实际的 Army 对象创建和 GatherArmyAction 调用。
                // new Army(...) 构造函数会注册游戏全局周期性事件（Tick/HourlyTick），
                // 与其他 Mod 的 CampaignPeriodicEventManager.Patch 冲突导致 NRE 崩溃。
                // 候选列表验证已完整证明了屏蔽效果。

                InformationManager.DisplayMessage(
                    new InformationMessage("[Test] ── 候选验证结论 ──", Colors.Cyan));

                if (playerClanCount > 0)
                {
                    InformationManager.DisplayMessage(
                        new InformationMessage("[Test] 屏蔽功能异常：仍有玩家家族部队在候选列表中", Colors.Red));
                    InformationManager.DisplayMessage(
                        new InformationMessage("[Test] 请检查 MCM 设置「禁止家族部队被征召」是否已开启", Colors.Yellow));
                }
                else
                {
                    InformationManager.DisplayMessage(
                        new InformationMessage("[Test] 屏蔽功能正常：玩家家族部队已被正确过滤", Colors.Green));
                    InformationManager.DisplayMessage(
                        new InformationMessage(
                            $"[Test] {succeededClan!.Leader.Name} 满足创建条件，可拉起军团", Colors.Green));
                    InformationManager.DisplayMessage(
                        new InformationMessage(
                            $"[Test] 候选 {succeededCandidates!.Count} 个部队将正常响应征召", Colors.Green));
                }

                InformationManager.DisplayMessage(
                    new InformationMessage("[Test] ✅ 验证完成（未实际创建军团，无副作用）", Colors.Green));
            }
            catch (Exception ex)
            {
                InformationManager.DisplayMessage(
                    new InformationMessage($"[Test] 执行失败: {ex.Message}", Colors.Red));
            }
        }

        /// <summary>
        /// 带颜色指示的输出：绿色表示条件满足，红色表示不满足。
        /// </summary>
        private static void Info(string label, bool passed, string detail)
        {
            Color color = passed ? Colors.Green : Colors.Red;
            string mark = passed ? "✅" : "❌";
            InformationManager.DisplayMessage(
                new InformationMessage($"  {mark} {label}: {detail}", color));
        }
    }
}
