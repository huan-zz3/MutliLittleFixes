using System;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem.Party;

namespace ExampleMod.Patches
{
    // ──────────────────────────────────────────────────────────────
    // 补丁: NavalDLCShipDeploymentModel.GetShipDeploymentLimit
    // ──────────────────────────────────────────────────────────────
    // 覆盖战帆DLC中每支队伍（玩家/友军/敌军）的船只部署上限。
    // 用 Postfix 将 DLC 默认 base 值（3）替换为 MCM 设置值（3~8）。
    // DLC 原有的 Perk 加成保持不变，总结果上限 8（DLC 硬编码上限）。
    //
    // 为什么不直接设置 Perk 值？
    //   DLC 的 base 是固定常数 3，Perk 加成在上层累加。
    //   我们替换 base 部分，Perk 加成自动继承，最终上限 8。
    // ──────────────────────────────────────────────────────────────
    internal static class NavalDeployLimitPatch
    {
        internal static MethodBase TargetMethod()
        {
            var type = AccessTools.TypeByName(
                "NavalDLC.GameComponents.NavalDLCShipDeploymentModel");
            if (type == null)
                return null;
            return AccessTools.Method(
                type,
                "GetShipDeploymentLimit",
                new[] { typeof(MobileParty) });
        }

        internal static void Postfix(ref int __result)
        {
            int userLimit = Settings.Instance?.NavalBattleShipLimit ?? 3;
            // 替换 base 值（原本是 3），保留 Perk 加成，上限 8
            __result = Math.Min(__result - 3 + userLimit, 8);
        }
    }
}
