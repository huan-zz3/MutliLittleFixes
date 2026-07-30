using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using TaleWorlds.MountAndBlade.ViewModelCollection.Scoreboard;

namespace ExampleMod.Patches
{
    /// <summary>
    /// 战场结算计分板排序逆转补丁。
    ///
    /// 原版行为：点击表头排序图标时，顺序为 Default(0) → Ascending(1) → Descending(2)。
    /// 本补丁将其反转为：Default(0) → Descending(1) → Ascending(2)。
    ///
    /// 实现方式：在每个 ExecuteSortBy* 方法中，将 SetSortMode(XxxState == 1) 替换为
    /// SetSortMode(XxxState == 2)，从而交换升序/降序的含义。
    /// </summary>
    [HarmonyPatch]
    internal static class ScoreboardSortOrderPatch
    {
        /// <summary>需要修补的所有 6 个排序方法。</summary>
        private static readonly string[] _targetMethods =
        {
            "ExecuteSortByRemaining",
            "ExecuteSortByKill",
            "ExecuteSortByUpgrade",
            "ExecuteSortByDead",
            "ExecuteSortByWounded",
            "ExecuteSortByRouted",
        };

        [HarmonyTargetMethods]
        private static IEnumerable<MethodBase> TargetMethods()
        {
            var type = typeof(SPScoreboardSortControllerVM);
            foreach (var name in _targetMethods)
            {
                var method = AccessTools.Method(type, name);
                if (method != null)
                    yield return method;
            }
        }

        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(
            IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();

            for (var i = 0; i < codes.Count - 1; i++)
            {
                // 匹配 ldc.i4.1 + ceq 模式 —— 这是比较操作数（XxxState == 1）。
                // (remainingState + 1) 中的 ldc.i4.1 后跟的是 add，不会匹配。
                if (codes[i].opcode == OpCodes.Ldc_I4_1
                    && codes[i + 1].opcode == OpCodes.Ceq)
                {
                    codes[i].opcode = OpCodes.Ldc_I4_2; // 1 → 2
                    codes[i].operand = null;
                }
            }

            return codes;
        }
    }
}
