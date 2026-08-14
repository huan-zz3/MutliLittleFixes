using System;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using MutliLittleFixes.Behaviors;

namespace MutliLittleFixes.Patches
{
    /// <summary>
    /// 运粮队大地图全局可见补丁组（参照 AnimusForge 信使队 CourierDeliveryBehavior 的成熟机制）。
    ///
    /// 可见性状态本身由 Behaviors/FoodTransportSupportBehavior 维护（每 tick 保活）：
    ///   - IsVisible = true（写入 setter，触发 OnVisibilityChanged → 名牌创建；配合 SetVisualAsDirty 触发地图图标淡入）
    ///   - IsInspected = true（悬停侦查状态）
    ///   - VisualTrackerManager.RegisterObject（原版任务追踪同款，注册进大地图追踪系统）
    ///   - SetPartyUsedByQuest(true)（任务标记：MapTrackerProvider.CanAddMobileParty 的资格判定
    ///     要求 LeaderHero==null 且被任务使用且已注册追踪，标记后自动触发 MobilePartyQuestStatusChanged 事件
    ///     → MapTrackerProvider.OnPartyQuestStatusChanged → AddIfEligible 加入追踪列表）
    ///
    /// 本类负责 UI 层的两个兜底补丁（均带 MCM 实时开关）：
    ///   1) PartyNameplateVM.RefreshBinding postfix — 强制运粮队名牌 IsVisibleOnMap/IsArmy/ShouldShowFullName/PartyBanner，
    ///      突破原版"名牌随相机缩放/侦查状态隐藏"的显示规则，按军队样式带旗帜显示全名；
    ///   2) MapTrackerProvider.CanAddMobileParty postfix — 已注册追踪且活跃的运粮队强制通过资格检查，
    ///      防止被原版资格判定踢出地图追踪列表。
    /// </summary>
    internal static class TransportPartyMapVisibilityPatch
    {
        /// <summary>UI 层兜底补丁是否已注册（延迟注册，避免在 SubModule 加载早期 patch UI 程序集方法触发程序集初始化挂起）。</summary>
        private static bool _uiPatchesApplied;

        /// <summary>UI 层兜底补丁是否注册失败（失败说明目标类型在当前游戏版本不存在，重试无意义）。</summary>
        private static bool _uiPatchesFailed;

        /// <summary>
        /// 延迟注册 UI 层兜底补丁（PartyNameplateVM.RefreshBinding / MapTrackerProvider.CanAddMobileParty）。
        /// 由 FoodTransportSupportBehavior.OnCampaignTick 在加载完成后首次调用；
        /// 不随 OnSubModuleLoad 注册——目标类型位于 SandBox.ViewModelCollection.dll（UI 程序集），
        /// 在加载存档/启动早期 patch 其方法可能触发程序集静态初始化在临界期执行而挂起（参照 AnimusForge 信使队
        /// 延迟重试模式）。返回是否已注册成功。
        /// </summary>
        internal static bool EnsureUiPatchesRegistered()
        {
            if (_uiPatchesApplied || _uiPatchesFailed)
            {
                return _uiPatchesApplied;
            }
            try
            {
                Harmony harmony = new Harmony("MutliLittleFixes");

                Type? nameplateType = AccessTools.TypeByName("SandBox.ViewModelCollection.Nameplate.PartyNameplateVM");
                MethodInfo? refreshBinding = nameplateType == null
                    ? null
                    : AccessTools.Method(nameplateType, "RefreshBinding");
                if (refreshBinding != null)
                {
                    harmony.Patch(refreshBinding,
                        postfix: new HarmonyMethod(typeof(TransportPartyMapVisibilityPatch), nameof(PartyNameplateRefreshBindingPostfix)));
                }

                Type? providerType = AccessTools.TypeByName("SandBox.ViewModelCollection.Map.Tracker.MapTrackerProvider");
                MethodInfo? canAddMobileParty = providerType == null
                    ? null
                    : AccessTools.Method(providerType, "CanAddMobileParty", new[] { typeof(MobileParty) });
                if (canAddMobileParty != null)
                {
                    harmony.Patch(canAddMobileParty,
                        postfix: new HarmonyMethod(typeof(TransportPartyMapVisibilityPatch), nameof(MapTrackerProviderCanAddMobilePartyPostfix)));
                }

                _uiPatchesApplied = refreshBinding != null || canAddMobileParty != null;
                _uiPatchesFailed = !_uiPatchesApplied;
            }
            catch
            {
                _uiPatchesFailed = true;
            }
            return _uiPatchesApplied;
        }

        /// <summary>
        /// PartyBase.UpdateVisibilityAndInspected prefix：运粮队跳过原版可见性计算。
        /// 原版每帧只对玩家视野半径(约 65 单位)内的队伍调用此方法重算可见性,未侦查则写 IsVisible=false,
        /// 导致运粮队在玩家附近被持续回滚(每帧隐藏→保活写回→图标每帧重建/闪烁)。
        /// 对运粮队直接跳过原方法体,保持保活写入的可见状态(IsVisible/IsInspected 不被覆盖)。
        /// </summary>
        internal static bool UpdateVisibilityAndInspectedPrefix(PartyBase __instance)
        {
            // MCM 运行时开关 — 关闭时恢复原版可见性计算
            if (Settings.Instance?.TransportMapVisibilityEnabled != true)
            {
                return true;
            }
            if (__instance?.MobileParty?.PartyComponent is FoodTransportPartyComponent)
            {
                return false; // 运粮队:跳过原版距离/侦查计算
            }
            return true;
        }

        /// <summary>PartyNameplateVM.RefreshBinding postfix：运粮队名牌强制显示。</summary>
        internal static void PartyNameplateRefreshBindingPostfix(object __instance)
        {
            // MCM 运行时开关 — 关闭时恢复原版显示规则
            if (Settings.Instance?.TransportMapVisibilityEnabled != true)
            {
                return;
            }
            try
            {
                if (__instance == null)
                {
                    return;
                }
                Type type = __instance.GetType();
                PropertyInfo partyProperty = type.GetProperty("Party", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                MobileParty? party = partyProperty?.GetValue(__instance, null) as MobileParty;
                if (party?.PartyComponent is not FoodTransportPartyComponent)
                {
                    return;
                }
                type.GetProperty("IsArmy", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.SetValue(__instance, true, null);
                type.GetProperty("ShouldShowFullName", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.SetValue(__instance, true, null);
                type.BaseType?.GetProperty("IsVisibleOnMap", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.SetValue(__instance, true, null);
                type.GetProperty("PartyBanner", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.SetValue(__instance, CreateBannerImageIdentifier(party), null);
            }
            catch
            {
            }
        }

        /// <summary>MapTrackerProvider.CanAddMobileParty postfix：运粮队追踪列表资格兜底。</summary>
        internal static void MapTrackerProviderCanAddMobilePartyPostfix(MobileParty party, ref bool __result)
        {
            // MCM 运行时开关 — 关闭时恢复原版资格判定
            if (Settings.Instance?.TransportMapVisibilityEnabled != true)
            {
                return;
            }
            if (party?.PartyComponent is not FoodTransportPartyComponent)
            {
                return;
            }
            try
            {
                VisualTrackerManager? tracker = Campaign.Current?.VisualTrackerManager;
                bool tracked = tracker != null && tracker.CheckTracked(party);
                if (tracked && party.IsActive)
                {
                    __result = true;
                }
            }
            catch
            {
            }
        }

        /// <summary>为运粮队构造旗帜图标（优先队伍旗帜，回退玩家家族旗帜）。</summary>
        private static object? CreateBannerImageIdentifier(MobileParty party)
        {
            try
            {
                Banner? banner = party?.Banner ?? Clan.PlayerClan?.Banner ?? Hero.MainHero?.Clan?.Banner;
                if (banner == null)
                {
                    return null;
                }
                Type? bannerVmType = AccessTools.TypeByName("TaleWorlds.Core.ViewModelCollection.ImageIdentifiers.BannerImageIdentifierVM");
                ConstructorInfo? ctor = bannerVmType?.GetConstructor(new[] { typeof(Banner), typeof(bool) });
                return ctor?.Invoke(new object[] { banner, true });
            }
            catch
            {
                return null;
            }
        }
    }
}
