using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Helpers;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.Core;

namespace ExampleMod.Patches
{
    // ──────────────────────────────────────────────────────────────
    // 补丁1: ShipHelper.GetOrderedNavalRaidShipsOfPlayerParty
    // ──────────────────────────────────────────────────────────────
    // 通过 Postfix 替换返回列表，应用 MCM 设置的船只上限。
    // 被 EncounterGameMenuBehavior / VillageHostileActionCampaignBehavior
    // 等调用，用于判断菜单按钮是否可用。
    // ──────────────────────────────────────────────────────────────
    [HarmonyPatch(typeof(ShipHelper), "GetOrderedNavalRaidShipsOfPlayerParty")]
    internal static class ShipHelperGetShipsPatch
    {
        internal static void Postfix(ref List<Ship> __result)
        {
            int limit = Settings.Instance?.NavalBattleShipLimit ?? 3;
            // 原方法内部已经 .Take(3) 了一次，我们用设置值再 Take 一遍
            __result = __result.Take(limit).ToList();
        }
    }

    // ──────────────────────────────────────────────────────────────
    // 补丁2: MenuHelper.StartSeaRaidMission
    // ──────────────────────────────────────────────────────────────
    // 用 Transpiler 替换方法中唯一的 .Take(3) 调用，
    // 将其常数参数 3 替换为 MCM 设置值。
    //
    // 为什么用 Transpiler？
    //   StartSeaRaidMission 是 private static 方法，无法用 Postfix
    //   修改其局部变量 selectedShips。Transpiler 在 IL 层面将
    //   ldc.i4.3 + call Enumerable.Take 替换为 ldc.i4.s limit。
    //
    // 安全分析：方法内唯一的"加载常数3 + 调用 Take"出现在
    // 第410行的 ship 选择上。其他 Take(count/maxSelectableTroopCount)
    // 用的都是变量而非常数3，不会被误匹配。
    // ──────────────────────────────────────────────────────────────
    [HarmonyPatch]
    internal static class MenuHelperStartSeaRaidPatch
    {
        [HarmonyTargetMethod]
        internal static MethodBase TargetMethod()
        {
            return AccessTools.Method(
                typeof(MenuHelper),
                "StartSeaRaidMission",
                new[] { typeof(MapEvent), typeof(BattleSideEnum), typeof(MenuCallbackArgs) });
        }

        internal static IEnumerable<CodeInstruction> Transpiler(
            IEnumerable<CodeInstruction> instructions)
        {
            int limit = Settings.Instance?.NavalBattleShipLimit ?? 3;

            var codes = instructions.ToList();
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Call
                    && codes[i].operand is MethodInfo methodInfo
                    && methodInfo.Name == nameof(Enumerable.Take)
                    && methodInfo.DeclaringType == typeof(Enumerable)
                    && i > 0
                    && codes[i - 1].opcode == OpCodes.Ldc_I4_3)
                {
                    // 将 ldc.i4.3 替换为用户设置的船只上限值
                    codes[i - 1] = new CodeInstruction(OpCodes.Ldc_I4_S, (byte)limit);
                }
            }

            return codes;
        }
    }
}
