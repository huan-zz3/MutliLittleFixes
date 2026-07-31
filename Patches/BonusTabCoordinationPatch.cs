using System;
using System.IO;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement;
using ExampleMod.UI;

namespace ExampleMod.Patches
{
    /// <summary>
    /// 标签页切换协调补丁合集。
    ///
    /// 核心策略：AnimusForge 通过 ButtonWidget.HandleClick Postfix + 
    /// HideForeignContentPanels 在 widget 层处理了跨 Mod 自定义标签互斥，
    /// 因此本 Mod 不再需要 ViewModel.OnPropertyChanged 拦截。
    ///
    /// 本补丁只保留原生标签（Clan/Fiefs/Policies/Army/Diplomacy）被选中时
    /// 清除本 Mod 标签的逻辑。
    /// </summary>
    internal static class BonusTabCoordinationPatch
    {
        // ════════════════════════════════════════════════════════
        // 原生标签被选中 → 清除本 Mod 标签
        // ════════════════════════════════════════════════════════

        internal static void ClearOnClan(KingdomManagementVM __instance)
            => ClearSelf(__instance);

        internal static void ClearOnFiefs(KingdomManagementVM __instance)
            => ClearSelf(__instance);

        internal static void ClearOnPolicies(KingdomManagementVM __instance)
            => ClearSelf(__instance);

        internal static void ClearOnArmy(KingdomManagementVM __instance)
            => ClearSelf(__instance);

        internal static void ClearOnDiplomacy(KingdomManagementVM __instance)
            => ClearSelf(__instance);

        // ════════════════════════════════════════════════════════
        // 辅助方法
        // ════════════════════════════════════════════════════════

        private static void ClearSelf(KingdomManagementVM vm)
            => BonusTabVMMixin.TryClear(vm);

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
