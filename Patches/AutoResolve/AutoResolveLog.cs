using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace MutliLittleFixes.Patches
{
    /// <summary>
    /// 坐镇指挥模拟重平衡 —— 调试日志辅助（受 MCM 调试开关 EnableAutoResolveDebugLog 控制）。
    /// 调试日志保持中文，不本地化（AGENTS.md §3.1）。
    /// </summary>
    internal static class AutoResolveLog
    {
        public static void PrintDebug(string message)
        {
            if (Settings.Instance?.EnableAutoResolveDebugLog == true)
            {
                InformationManager.DisplayMessage(new InformationMessage(message));
            }
        }

        public static void PrintWarn(string message)
        {
            if (Settings.Instance?.EnableAutoResolveDebugLog == true)
            {
                InformationManager.DisplayMessage(new InformationMessage(message));
                Debug.PrintError(message, "MutliLittleFixes.AutoResolve");
            }
        }

        public static void PrintError(string message)
        {
            // 错误日志始终写入日志文件（不受调试开关控制，便于排查），屏幕提示仅在调试开关开启时显示
            Debug.PrintError(message, "MutliLittleFixes.AutoResolve");
            if (Settings.Instance?.EnableAutoResolveDebugLog == true)
            {
                InformationManager.DisplayMessage(new InformationMessage(message));
            }
        }
    }
}
