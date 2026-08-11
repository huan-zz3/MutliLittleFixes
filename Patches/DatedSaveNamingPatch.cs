using System;
using System.Globalization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;

namespace MutliLittleFixes.Patches
{
    /// <summary>
    /// 日期时间并行存档命名补丁。
    ///
    /// 目标方法：MBSaveLoad.QuickSaveCurrentGame / MBSaveLoad.AutoSaveCurrentGame
    /// （TaleWorlds.Core，F5 / ESC 菜单「Save」「Save And Exit」/ 定时自动存档的最终汇聚点，
    ///  一处补丁即覆盖所有快速存档与自动存档入口；另存为与铁人模式不经过这两个方法，保持原版）。
    ///
    /// 行为（由 MCM 开关实时控制，不重启生效）：
    ///   - 开关开启时，快速存档与自动存档改用「存档时日期时间」命名：
    ///       快速存档 → save_qu_yyyy-MM-dd_HH-mm-ss.fff
    ///       自动存档 → save_au_yyyy-MM-dd_HH-mm-ss.fff
    ///     每个战役（UniqueGameId）各自独立轮转一个池（容量 DatedSavePoolSize，默认 10）：
    ///     当前战役每次保存成功后若该战役的日期档超容量，按 CreationTime 淘汰该战役最旧的
    ///     （先存成功、再删旧，避免存档损失）；其他战役的日期档因 UniqueGameId 不同永不被触及。
    ///   - 存档内容完全走原版管线：通过 MBSaveLoad.SaveAsCurrentGame 转发（其内部
    ///     OverwriteSaveAux → OverwriteSaveFile → Game.Current.Save 与原版一致），仅替换存档名。
    ///   - 开关关闭时放行原版命名（saveNNN / saveauto1-3 轮转）；旧文件与前缀不同的文件
    ///     永不被本补丁删除，新旧体系并行共存。
    ///   - 屏幕日志（左下角，青色）由 MCM 调试开关 DatedSaveNamingDebugLogEnabled 控制（默认关）：
    ///     输出新档名、保存结果、轮转淘汰的旧档。
    ///
    /// 由 HarmonyPatchRegistry 显式注册（不使用 [HarmonyPatch] 属性）。
    /// </summary>
    internal static class DatedSaveNamingPatch
    {
        /// <summary>快速存档新池前缀（区别于原版 saveNNN）。</summary>
        private const string QuickSaveNamePrefix = "save_qu_";

        /// <summary>自动存档新池前缀（区别于原版 saveautoN）。</summary>
        private const string AutoSaveNamePrefix = "save_au_";

        internal static bool Prefix_QuickSave(
            CampaignSaveMetaDataArgs campaignMetaData,
            Action<(SaveResult, string)> onSaveCompleted)
        {
            return TryDatedSave(campaignMetaData, onSaveCompleted, QuickSaveNamePrefix);
        }

        internal static bool Prefix_AutoSave(
            CampaignSaveMetaDataArgs campaignMetaData,
            Action<(SaveResult, string)> onSaveCompleted)
        {
            return TryDatedSave(campaignMetaData, onSaveCompleted, AutoSaveNamePrefix);
        }

        private static bool TryDatedSave(
            CampaignSaveMetaDataArgs campaignMetaData,
            Action<(SaveResult, string)> onSaveCompleted,
            string prefix)
        {
            // MCM 运行时开关 — 关闭时放行原版命名逻辑（Settings 为 null 时同样放行）
            if (Settings.Instance?.DatedSaveNamingEnabled != true)
            {
                return true;
            }

            string modeName = prefix == QuickSaveNamePrefix ? "快速存档" : "自动存档";
            string saveName = GetUniqueDatedSaveName(prefix);
            LogScreen($"[日期存档] {modeName} → {saveName}");
            MBSaveLoad.SaveAsCurrentGame(campaignMetaData, saveName, result =>
            {
                if (result.Item1 == SaveResult.Success)
                {
                    LogScreen($"[日期存档] 保存成功：{saveName}");
                    // 仅保存成功后修剪轮转池：先存新档、再淘汰最旧档，避免存档损失
                    int prunedCount = PruneDatedSavePool();
                    if (prunedCount > 0)
                    {
                        LogScreen($"[日期存档] 轮转：当前战役池已满，共淘汰 {prunedCount} 个最旧档");
                    }
                }
                else
                {
                    LogScreen($"[日期存档] 保存失败：{result.Item1}");
                }
                onSaveCompleted?.Invoke(result);
            });
            return false; // 已接管，跳过原方法
        }

        /// <summary>
        /// 生成唯一的新池存档名：前缀 + 当前日期时间（毫秒精度）。
        /// 极端情况下同毫秒重名时追加 "_1"、"_2"… 后缀，确保不会覆盖已有存档。
        /// </summary>
        private static string GetUniqueDatedSaveName(string prefix)
        {
            string baseName = prefix + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss.fff", CultureInfo.InvariantCulture);
            string name = baseName;
            int suffix = 1;
            while (MBSaveLoad.IsSaveGameFileExists(name))
            {
                name = baseName + "_" + suffix;
                suffix++;
            }
            return name;
        }

        /// <summary>
        /// 修剪轮转池（快速 + 自动共享）：GetSaveFiles 已按 CreationTime 降序返回。
        /// 仅统计并淘汰「当前战役（Campaign.Current.UniqueGameId）」的日期档，
        /// 其他战役的存档因 UniqueGameId 不同而被排除，不会互相覆盖。
        /// 无法归属（Campaign 为空 / UniqueGameId 缺失 / 元数据损坏）时不动任何文件，保守防误删。
        /// </summary>
        /// <returns>实际淘汰的文件数。</returns>
        private static int PruneDatedSavePool()
        {
            int poolSize = Settings.Instance?.DatedSavePoolSize ?? 10;
            if (poolSize < 1)
            {
                return 0;
            }

            string currentCampaignId = Campaign.Current?.UniqueGameId;
            if (string.IsNullOrEmpty(currentCampaignId))
            {
                return 0; // 无法归属到具体战役时不做任何删除，防止误删其他战役的存档
            }

            SaveGameFileInfo[] files = MBSaveLoad.GetSaveFiles(
                f => (f.Name.StartsWith(QuickSaveNamePrefix, StringComparison.Ordinal) ||
                      f.Name.StartsWith(AutoSaveNamePrefix, StringComparison.Ordinal)) &&
                     string.Equals(f.MetaData.GetUniqueGameId(), currentCampaignId, StringComparison.Ordinal));
            int prunedCount = 0;
            for (int i = poolSize; i < files.Length; i++)
            {
                LogScreen($"[日期存档] 淘汰旧档：{files[i].Name}");
                if (MBSaveLoad.DeleteSaveGame(files[i].Name))
                {
                    prunedCount++;
                }
            }
            return prunedCount;
        }

        /// <summary>屏幕左下角日志（由 MCM 调试开关控制，默认关闭）。</summary>
        private static void LogScreen(string message)
        {
            if (Settings.Instance?.DatedSaveNamingDebugLogEnabled != true)
            {
                return;
            }
            InformationManager.DisplayMessage(new InformationMessage(message, Color.FromUint(0x00FFFFu)));
        }
    }
}
