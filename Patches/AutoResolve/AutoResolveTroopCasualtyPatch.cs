using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace MutliLittleFixes.Patches
{
    /// <summary>
    /// 坐镇指挥模拟重平衡 —— 按士兵累计 HP 结算伤亡（核心改动，Prefix 整体替换原方法）。
    ///
    /// 原版非英雄伤亡判定：每次命中掷一次骰子 `RandomInt(最大HP) &lt; damage` 即击倒，无累计伤害，
    /// 单发大伤直接带走、低级兵一发毙命。
    /// 本补丁：伤害累加到 SimulateData 的士兵 HP（默认 100）上，扣到 ≤0 才进入存活判定
    /// （外科手术/医生誓约）→ 伤/亡 + 事件通知，实现「士兵更抗打、高伤武器更有价值」。
    /// 战斗节奏由 AutoResolveAiSimulationSpeed（AI 对 AI）或玩家坐镇 UI 驱动控制，不再用伤害倍率放大。
    ///
    /// 英雄分支保持原版流程（放行原方法）。
    /// 对应旧版 AutoResolveRebalanced 的 Patch_ApplySimulationDamageToSelectedTroop。
    /// </summary>
    internal static class AutoResolveTroopCasualtyPatch
    {
        private static readonly PropertyInfo _battleObserverProperty = AccessTools.Property(typeof(MapEventSide), "BattleObserver");
        private static readonly MethodInfo _removeSelectedTroopMethod = AccessTools.Method(typeof(MapEventSide), "RemoveSelectedTroopFromSimulationList");

        /// <summary>Prefix 整体替换。返回 false = 跳过原方法（已在本补丁内完成结算）。</summary>
        internal static bool Prefix(ref int damage, DamageTypes damageType, PartyBase strikerParty,
            MapEventSide __instance, ref bool __result,
            ref CharacterObject ____selectedSimulationTroop,
            ref UniqueTroopDescriptor ____selectedSimulationTroopDescriptor,
            ref List<UniqueTroopDescriptor> ____simulationTroopList,
            ref Dictionary<UniqueTroopDescriptor, MapEventParty> ____allocatedTroops)
        {
            // MCM 运行时开关 — 关闭时放行原方法
            if (Settings.Instance?.AutoResolveEnabled != true)
                return true;

            try
            {
                if (Settings.Instance.AutoResolveAiEnabled || __instance.MapEvent.IsPlayerSimulation)
                {
                    bool handled = false;
                    CharacterObject selectedTroop = ____selectedSimulationTroop;

                    if (selectedTroop != null && selectedTroop.IsHero)
                    {
                        // 英雄走原版受伤/死亡流程（damage 已是 SimulateHit 的武器面板伤害，通过 ref 生效）
                        return true;
                    }

                    AutoResolveSimulationData data;
                    if (!AutoResolveSimulationDataDict.GetData(__instance, out data))
                    {
                        data = new AutoResolveSimulationData();
                        data.UpdateDict(__instance, ____simulationTroopList);
                        AutoResolveSimulationDataDict.AddData(__instance, data);
                        AutoResolveLog.PrintWarn("[坐镇重平衡] ApplyDamage 时未找到累计 HP 数据，已重建");
                    }

                    int hitPoints;
                    if (!data.GetHitPoint(____selectedSimulationTroopDescriptor, out hitPoints))
                    {
                        // 字典缺失（极端情况）：清空重建后重试
                        data.Clear(true);
                        AutoResolveSimulationDataDict.RemoveData(__instance);
                        data = new AutoResolveSimulationData();
                        data.UpdateDict(__instance, ____simulationTroopList);
                        AutoResolveSimulationDataDict.AddData(__instance, data);
                        if (!data.GetHitPoint(____selectedSimulationTroopDescriptor, out hitPoints))
                        {
                            AutoResolveLog.PrintWarn("[坐镇重平衡] ApplyDamage 累计 HP 重建后仍缺失，放行原版");
                            return true;
                        }
                    }

                    int remaining = hitPoints - damage;
                    data.SetHitPoint(____selectedSimulationTroopDescriptor, remaining);
                    AutoResolveLog.PrintDebug($"[坐镇重平衡] {selectedTroop?.ToString()}(T{selectedTroop?.Tier}) HP {hitPoints} -> {remaining} (-{damage})");

                    if (remaining <= 0)
                    {
                        PartyBase party = ____allocatedTroops[____selectedSimulationTroopDescriptor].Party;
                        float survivalChance = Campaign.Current.Models.PartyHealingModel.GetSurvivalChance(
                            party, ____selectedSimulationTroop, damageType, canDamageKillEvenIfBlunt: false, strikerParty);
                        if (MBRandom.RandomFloat < survivalChance)
                        {
                            __instance.OnTroopWounded(____selectedSimulationTroopDescriptor);
                            IBattleObserver battleObserver = (IBattleObserver)_battleObserverProperty.GetValue(__instance);
                            if (battleObserver != null)
                            {
                                battleObserver.TroopNumberChanged(
                                    __instance.MissionSide, __instance.GetAllocatedTroopParty(____selectedSimulationTroopDescriptor),
                                    ____selectedSimulationTroop, -1, 0, 1, 0, 0, 0);
                            }
                            SkillLevelingManager.OnSurgeryApplied(party.MobileParty, true, ____selectedSimulationTroop.Tier);
                            if (strikerParty?.MobileParty != null && strikerParty.MobileParty.HasPerk(DefaultPerks.Medicine.DoctorsOath))
                            {
                                SkillLevelingManager.OnSurgeryApplied(strikerParty.MobileParty, true, ____selectedSimulationTroop.Tier);
                            }
                        }
                        else
                        {
                            __instance.OnTroopKilled(____selectedSimulationTroopDescriptor);
                            IBattleObserver battleObserver2 = (IBattleObserver)_battleObserverProperty.GetValue(__instance);
                            if (battleObserver2 != null)
                            {
                                battleObserver2.TroopNumberChanged(
                                    __instance.MissionSide, __instance.GetAllocatedTroopParty(____selectedSimulationTroopDescriptor),
                                    ____selectedSimulationTroop, -1, 1, 0, 0, 0, 0);
                            }
                            SkillLevelingManager.OnSurgeryApplied(party.MobileParty, false, ____selectedSimulationTroop.Tier);
                            if (strikerParty?.MobileParty != null && strikerParty.MobileParty.HasPerk(DefaultPerks.Medicine.DoctorsOath))
                            {
                                SkillLevelingManager.OnSurgeryApplied(strikerParty.MobileParty, false, ____selectedSimulationTroop.Tier);
                            }
                        }
                        handled = true;
                    }

                    if (handled)
                    {
                        _removeSelectedTroopMethod.Invoke(__instance, null);
                    }
                    __result = handled;
                    return false; // 已处理，跳过原方法
                }
            }
            catch (Exception ex)
            {
                AutoResolveLog.PrintError("[坐镇重平衡] 伤亡结算异常，放行原版: " + ex);
                return true;
            }
            return true;
        }
    }
}
