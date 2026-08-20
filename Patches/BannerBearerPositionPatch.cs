using System.Collections.Generic;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace MutliLittleFixes.Patches
{
    /// <summary>
    ///     将旗手在编队中的默认位置从"最左列第二排（左前侧）"改为"最后一排中间"，
    ///     降低旗手死亡率，保护旗帜 buff 持续生效。
    ///
    ///     影响的阵型：Line（线列）、ShieldWall（盾阵）、Loose（散阵）、Scatter（散开）
    ///     不影响的阵型：Circle（圆阵）、Skein（楔形）、Square（方阵）— 已有安全位置
    ///     Column（纵队）无旗手机制，不受影响。
    /// </summary>
    internal static class BannerBearerPositionPatch
    {
        internal static void RepositionBannerBearerToLastRowCenter(
            ref List<FormationArrangementModel.ArrangementPosition> __result,
            Formation formation)
        {
            // 海战禁用（战帆 DLC 海战/沿海掠夺海战）— 统一原则：士兵 AI 行为调整在海战不干预
            if (NavalBattleDetector.IsNavalBattle())
                return;

            // MCM 运行时开关 — 关闭时不干预
            if (Settings.Instance?.BannerBearerPositionEnabled != true)
                return;

            if (__result == null || __result.Count == 0 || formation?.Arrangement == null)
                return;

            // 只处理 Line/ShieldWall/Loose/Scatter — 它们在 GetBannerBearerPositions 中
            // 走 LineFormation 基类的默认 fallback（BannerBearerLineFormationPositions），
            // 位置0是"左前列"。
            //
            // Circle/Skein/Square 有各自的专用位置数组且位置0已在安全区，不改动。
            if (formation.Arrangement is CircularFormation ||
                formation.Arrangement is SkeinFormation ||
                formation.Arrangement is SquareFormation)
                return;

            if (formation.Arrangement is LineFormation lineFormation)
            {
                lineFormation.GetFormationInfo(out int fileCount, out int rankCount);
                if (fileCount <= 0 || rankCount <= 0)
                    return;

                // 中间列（fileCount/2）：奇数居正中，偶数略偏右
                int centerFile = fileCount / 2;
                // 最后一排（rankCount - 1）
                int lastRank = rankCount - 1;

                __result[0] = new FormationArrangementModel.ArrangementPosition(centerFile, lastRank);
            }
        }
    }
}
