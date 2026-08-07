using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace ExampleMod.Patches
{
    /// <summary>
    /// 志愿者升级概率倍率补丁（Transpiler）。
    ///
    /// 原版行为：RecruitmentCampaignBehavior.UpdateVolunteersOfNotablesInSettlement 中，
    /// 已有志愿者的槽位每日按以下概率升级（该概率硬编码在行为方法内，不经任何模型）：
    ///   float num = MathF.Log(notable.Power / (float)characterObject.Tier, 2f) * 0.01f;
    ///   if (MBRandom.RandomFloat < num) → 随机替换为 UpgradeTargets[random]
    ///
    /// 本补丁将 IL 中的 0.01f 常量替换为 multiplier * 0.01f（倍率实时读取 MCM）：
    ///   ldc.r4 0.01  →  call GetUpgradeRateMultiplier; ldc.r4 0.01; mul
    /// 结果：log2(Power/Tier) * multiplier * 0.01，倍率 1.0 = 原版，开关关闭时 GetUpgradeRateMultiplier
    /// 返回 1.0f（等效原版）。
    ///
    /// 注入约束（与 ScoreboardSortOrderPatch 一致）：不创建任何 Label、不引入分支指令，
    /// 保持栈平衡（注入序列栈变化与原 ldc.r4 相同，均为 +1），原指令标签原样保留。
    ///
    /// 由 HarmonyPatchRegistry 显式注册（不使用 [HarmonyPatch] 属性）。
    /// </summary>
    internal static class VolunteerUpgradeRatePatch
    {
        /// <summary>
        /// Transpiler 注入的运行时调用目标：每次方法调用时执行，读取 MCM 当前值（实时生效）。
        /// </summary>
        internal static float GetUpgradeRateMultiplier()
        {
            return Settings.Instance?.VolunteerUpgradeRateEnabled == true
                ? Settings.Instance.VolunteerUpgradeRateMultiplier
                : 1f;
        }

        internal static IEnumerable<CodeInstruction> Transpiler(
            IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            MethodInfo multiplierGetter = AccessTools.Method(
                typeof(VolunteerUpgradeRatePatch), nameof(GetUpgradeRateMultiplier));

            for (var i = 0; i < codes.Count; i++)
            {
                // 匹配唯一常量 0.01f（该方法是升级概率硬编码点，方法内无其它 0.01f 字面量）。
                if (codes[i].opcode == OpCodes.Ldc_R4
                    && codes[i].operand is float value
                    && value == 0.01f)
                {
                    // 无分支注入，等效于（栈上原值: [log2(Power/Tier)]）：
                    //   [log2] call GetUpgradeRateMultiplier → [log2, m]
                    //   [log2, m] ldc.r4 0.01               → [log2, m, 0.01]
                    //   [log2, m, 0.01] mul                 → [log2, m*0.01]
                    // 原指令后的 mul（log2 * 结果）保持不变 → log2 * m * 0.01
                    // 栈平衡：注入序列净 +1（与原 ldc.r4 0.01 相同）。
                    var injected = new List<CodeInstruction>
                    {
                        new CodeInstruction(OpCodes.Call, multiplierGetter)
                            { labels = codes[i].labels }, // 转移原指令标签（若有分支指向此处，落在 call 上等价于原 ldc.r4）
                        new CodeInstruction(OpCodes.Ldc_R4, 0.01f),
                        new CodeInstruction(OpCodes.Mul),
                    };

                    codes.RemoveAt(i);
                    codes.InsertRange(i, injected);
                    break; // 唯一匹配点，替换后结束
                }
            }

            return codes;
        }
    }
}
