using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using TaleWorlds.MountAndBlade.ViewModelCollection.Scoreboard;

namespace MutliLittleFixes.Patches
{
    /// <summary>
    /// 战场结算计分板排序逆转补丁。
    ///
    /// 原版行为：点击表头排序图标时，顺序为 Default(0) → Ascending(1) → Descending(2)。
    /// 本补丁将其反转为：Default(0) → Descending(1) → Ascending(2)。
    ///
    /// 实现方式：在每个 ExecuteSortBy* 方法中，将 SetSortMode(XxxState == 1) 替换为
    /// SetSortMode(XxxState == 2)，从而交换升序/降序的含义。
    ///
    /// 由 HarmonyPatchRegistry 显式注册（不使用 [HarmonyPatch] 属性）。
    /// MCM 开关 ScoreboardSortOrderEnabled 通过注入的运行时指令实时生效：
    /// 开启时比较常量用 2（逆转），关闭时用 1（原版）。
    ///
    /// 注意：开关分支必须用无标签的纯栈运算实现（(int)Enabled + 1），
    /// 不能使用 new Label() + brfalse/br 注入分支——所有 new Label() 的
    /// 内部编号都是 0，值相等，会与原方法已有的标签塌缩成同一个跳转目标，
    /// 导致栈不平衡的 InvalidProgramException（启动即崩溃）。
    /// </summary>
    internal static class ScoreboardSortOrderPatch
    {
        /// <summary>需要修补的所有 6 个排序方法（供注册器循环注册）。</summary>
        internal static readonly string[] TargetMethodNames =
        {
            "ExecuteSortByRemaining",
            "ExecuteSortByKill",
            "ExecuteSortByUpgrade",
            "ExecuteSortByDead",
            "ExecuteSortByWounded",
            "ExecuteSortByRouted",
        };

        /// <summary>MCM 实时开关：开启（默认）时排序循环反转。</summary>
        private static bool Enabled => Settings.Instance?.ScoreboardSortOrderEnabled != false;

        internal static IEnumerable<CodeInstruction> Transpiler(
            IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            MethodInfo enabledGetter = AccessTools.PropertyGetter(
                typeof(ScoreboardSortOrderPatch), nameof(Enabled));

            for (var i = 0; i < codes.Count - 1; i++)
            {
                // 匹配 ldc.i4.1 + ceq 模式 —— 这是比较操作数（XxxState == 1）。
                // (remainingState + 1) 中的 ldc.i4.1 后跟的是 add，不会匹配。
                if (codes[i].opcode == OpCodes.Ldc_I4_1
                    && codes[i + 1].opcode == OpCodes.Ceq)
                {
                    // 无分支注入，等效于：
                    //   stack: [state]
                    //   call Enabled → [state, bool]
                    //   conv.i4     → [state, 0|1]
                    //   ldc.i4.1    → [state, 0|1, 1]
                    //   add         → [state, 1|2]   （开启=2 逆转，关闭=1 原版）
                    //   ceq
                    // 不创建任何 Label，避免与原方法标签值相等导致跳转目标塌缩。
                    var injected = new List<CodeInstruction>
                    {
                        new CodeInstruction(OpCodes.Call, enabledGetter)
                            { labels = codes[i].labels }, // 转移原指令的标签（若有分支指向此处，落在 call 上等价于原 ldc.i4.1）
                        new CodeInstruction(OpCodes.Conv_I4),
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                        new CodeInstruction(OpCodes.Add),
                    };

                    codes.RemoveAt(i);          // 移除原 ldc.i4.1
                    codes.InsertRange(i, injected); // 注入序列替代（ceq 保持在注入序列之后）
                    i += injected.Count - 1;    // 跳过已注入区域，避免重复匹配
                }
            }

            return codes;
        }
    }
}
