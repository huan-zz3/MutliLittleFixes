using TaleWorlds.CampaignSystem.Encounters;

namespace MutliLittleFixes.Patches
{
    /// <summary>
    /// 加入战斗自由撤退：
    /// 原版中玩家加入大地图上已有的战斗后，若玩家侧是防守方（如友军守城/被围攻），
    /// MapEventHelper.CanMainPartyLeaveBattleCommonCondition() 返回 false，
    /// encounter 菜单不显示「离开」选项，玩家只能打赢或战败被俘。
    ///
    /// 本补丁在 MCM 开关开启时，对「玩家加入的已有战斗」（IsJoinedBattle）强制放行，
    /// 使防守方也能随时带部队离开战场。
    /// 玩家自己发起的守城/攻城战斗（非加入）保持原版规则，不受影响。
    ///
    /// 带 MCM 开关，由 HarmonyPatchRegistry 显式注册，实时生效，无需重启。
    /// </summary>
    internal static class FreeBattleRetreatPatch
    {
        internal static void Postfix(ref bool __result)
        {
            // MCM 运行时开关 — 关闭时不干预
            if (Settings.Instance?.FreeBattleRetreatEnabled != true)
                return;

            // 只作用于玩家「加入」的已有战斗；玩家自己发起的战斗保持原版逻辑
            if (PlayerEncounter.Current?.IsJoinedBattle != true)
                return;

            __result = true;
        }
    }
}
