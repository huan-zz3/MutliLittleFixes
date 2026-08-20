using TaleWorlds.MountAndBlade;

namespace MutliLittleFixes
{
    /// <summary>
    /// 海战模式检测（战帆 Warsail DLC 海战 / 沿海掠夺海战）。
    ///
    /// 士兵 AI 行为调整功能在海战中统一禁用（原则见 AGENTS.md §5 海战模式禁用规则）：
    /// 海战中士兵随船移动、编队绑定船只（每船一队，NavalTeamAgents 强管理）、甲板非地形，
    /// 自动蹲下/第9队移交/盾牌插地/站位重排等在海上要么失效、要么产生异常行为，
    /// 故所有士兵行为功能在海战任务中一律显式禁用、不干预。
    ///
    /// 检测基于原版 <see cref="Mission.IsNavalBattle"/> / <see cref="Mission.IsNavalRaidBattle"/>
    /// （即 MissionTeamAIType 为 NavalBattle / NavalRaid，含自定义海战与战役沿海掠夺海战），
    /// 不依赖 DLC 程序集，未安装战帆 DLC 时恒为 false，安全无副作用。
    /// </summary>
    internal static class NavalBattleDetector
    {
        /// <summary>
        /// 判断任务是否为海战（MissionLogic 内使用，传入 this.Mission）。
        /// </summary>
        public static bool IsNavalBattle(Mission? mission)
        {
            return mission != null && (mission.IsNavalBattle || mission.IsNavalRaidBattle);
        }

        /// <summary>
        /// 判断当前任务是否为海战（Harmony 补丁等无 Mission 实例的场景使用，取 Mission.Current）。
        /// 主菜单等无任务环境 Mission.Current 为 null，安全返回 false。
        /// </summary>
        public static bool IsNavalBattle()
        {
            Mission? current = Mission.Current;
            return current != null && (current.IsNavalBattle || current.IsNavalRaidBattle);
        }
    }
}