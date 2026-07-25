using System;
using System.IO;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement;
using TaleWorlds.Library;
using ExampleMod.UI;

namespace ExampleMod.Patches
{
    /// <summary>
    /// 标签页切换协调补丁合集。
    ///
    /// 一组一：5 个原生标签方法 → Postfix 清除本 Mod 标签
    /// 组二：ViewModel.OnPropertyChanged 拦截 → 跨 Mod 双向互斥
    ///
    /// 原生游戏用 SetSelectedCategory 实现标签互斥（一个选中，其他全灭）。
    /// 自定义注入标签各自用独立的 IsXxxSelected 标志，彼此不知对方存在。
    /// 本补丁让这个生态恢复到互斥状态。
    /// </summary>
    [HarmonyPatch]
    internal static class BonusTabCoordinationPatch
    {
        // ════════════════════════════════════════════════════════
        // 组一：5 个原生标签被选中 → 清除本 Mod 标签
        // ════════════════════════════════════════════════════════

        [HarmonyPostfix]
        [HarmonyPatch(typeof(KingdomManagementVM), "ExecuteShowClan")]
        private static void ClearOnClan(KingdomManagementVM __instance)
            => ClearSelf(__instance);

        [HarmonyPostfix]
        [HarmonyPatch(typeof(KingdomManagementVM), "ExecuteShowFiefs")]
        private static void ClearOnFiefs(KingdomManagementVM __instance)
            => ClearSelf(__instance);

        [HarmonyPostfix]
        [HarmonyPatch(typeof(KingdomManagementVM), "ExecuteShowPolicies")]
        private static void ClearOnPolicies(KingdomManagementVM __instance)
            => ClearSelf(__instance);

        [HarmonyPostfix]
        [HarmonyPatch(typeof(KingdomManagementVM), "ExecuteShowArmy")]
        private static void ClearOnArmy(KingdomManagementVM __instance)
            => ClearSelf(__instance);

        [HarmonyPostfix]
        [HarmonyPatch(typeof(KingdomManagementVM), "ExecuteShowDiplomacy")]
        private static void ClearOnDiplomacy(KingdomManagementVM __instance)
            => ClearSelf(__instance);

        // ════════════════════════════════════════════════════════
        // 组二：跨 Mod 标签互斥（OnPropertyChanged 拦截）
        // ════════════════════════════════════════════════════════

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ViewModel), "OnPropertyChanged", new[] { typeof(string) })]
        private static void OnViewModelPropertyChanged(ViewModel __instance, string propertyName)
        {
            if (!(__instance is KingdomManagementVM vm))
                return;

            if (propertyName == "IsAgendaSelected")
            {
                Log("[Cross] 议程标签 IsAgendaSelected 变化 → 清除本标签");
                ClearSelf(vm);
            }
            else if (propertyName == "IsBonusTabSelected")
            {
                Log($"[Cross] 本标签 IsBonusTabSelected 变化 → 尝试清除议程标签");
                TryClearOther(vm, "IsAgendaSelected", false);
            }
        }

        // ════════════════════════════════════════════════════════
        // 辅助方法
        // ════════════════════════════════════════════════════════

        private static void ClearSelf(KingdomManagementVM vm)
            => BonusTabVMMixin.TryClear(vm);

        private static void TryClearOther(KingdomManagementVM vm, string propName, bool value)
        {
            try
            {
                var prop = vm.GetType().GetProperty(propName,
                    BindingFlags.Public | BindingFlags.Instance);
                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(vm, value);
                    Log($"  → 成功设置 {propName} = {value}");
                }
                else
                {
                    Log($"  → 属性 {propName} 不存在或不可写，跳过");
                }
            }
            catch (Exception ex)
            {
                Log($"  → 设置 {propName} 异常: {ex.Message}");
            }
        }

        // ── 调试日志 ─────────────────────────────────────────────

        private static readonly string _logPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            "BonusTab_debug.log");

        private static void Log(string message)
        {
            try
            {
                File.AppendAllText(_logPath,
                    $"[{DateTime.Now:HH:mm:ss.fff}] {message}{Environment.NewLine}");
            }
            catch { }
        }
    }
}
