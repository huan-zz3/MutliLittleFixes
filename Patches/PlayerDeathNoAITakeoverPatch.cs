using TaleWorlds.MountAndBlade;

namespace MutliLittleFixes.Patches
{
    /// <summary>
    /// 玩家阵亡时不激活 AI 托管：
    /// 原版中玩家角色（MainAgent）阵亡/被移除时，Mission.OnAgentRemoved 会调用
    /// Team.DelegateCommandToAI()，把玩家队伍的所有阵型强制切换为 AI 指挥（AI 全权接管部队）。
    ///
    /// 实现：Prefix 拦截 Team.DelegateCommandToAI()。全源码仅 Mission.OnAgentRemoved
    /// （玩家阵亡路径）一处调用该方法，拦截入口即精确覆盖"玩家阵亡触发 AI 托管"场景。
    /// 开关开启且满足【玩家队伍 + 主控角色已阵亡（MainAgent 已置空）】时跳过原方法，
    /// 部队保持玩家阵亡瞬间的最后指令继续战斗；其余任何场景一律放行原方法，不影响原版行为。
    ///
    /// 注意：玩家阵亡后原版命令界面仍会被 MissionOrderVM 强制关闭（本补丁不干预），
    /// 因此阵亡后无法再手动下令，部队按最后指令行动。
    ///
    /// 不带 [HarmonyPatch] 属性，由 HarmonyPatchRegistry 显式注册。
    /// 运行时开关检查：关闭时 Prefix 直接 return true 放行，零开销。
    /// </summary>
    internal static class PlayerDeathNoAITakeoverPatch
    {
        internal static bool Prefix(Team __instance)
        {
            // MCM 运行时开关 — 关闭时放行原方法，恢复原版阵亡 AI 托管
            if (Settings.Instance?.PlayerDeathNoAITakeoverEnabled != true)
                return true;

            Mission mission = Mission.Current;
            // 仅拦截"玩家阵亡"场景：必须是玩家队伍，且玩家主控角色已被移除（已置空）。
            // 其他任何时机调用 DelegateCommandToAI 都放行。
            if (mission == null || __instance != mission.PlayerTeam || mission.MainAgent != null)
                return true;

            // 跳过原方法：不激活 AI 托管，部队保持玩家最后指令继续战斗
            return false;
        }
    }
}
