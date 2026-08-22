using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.Core;

namespace MutliLittleFixes.Patches
{
    /// <summary>
    /// 禁用投掷武器近战（AI 使用投掷武器近战模式禁用）—— 数据级全局改造，Harmony Postfix 补丁。
    ///
    /// 投掷武器（标枪/飞斧/飞刀）是 CraftedItem，由 spear/axe/knife 部件经锻造模板组装而成。
    /// 每个投掷锻造模板列出多个 WeaponDescription，成品武器上每个 description 变成一个
    /// usage（WeaponComponentData），其中那个近战 description 正是让 AI 在贴身时拿投掷武器
    /// 「戳人」而非拔剑的来源。
    ///
    /// 挂载点：`Game.LoadBasicFiles` 的 Postfix。原版加载时序（Campaign.InitializeDefaultCampaignObjects，
    /// 新开战役与读档都会走这条路径）：
    ///   1. LoadBasicFiles()   —— 依次 LoadXML Monsters…WeaponDescriptions…CraftingTemplates（模板在此加载）
    ///   2. LoadXML("Items")   —— 物品（CraftedItem）在此生成
    /// 因此在 LoadBasicFiles 返回后（Postfix 时机）修改模板，能保证后续 Items 加载时
    /// 生成的标枪/飞斧/飞刀只有投掷用法、没有近战用法。原版官方也在同一时机修改
    /// WeaponDescription.IsHiddenFromUI（Campaign.cs 1474-1483），佐证这是物品生成前改描述的正确时机。
    ///
    /// 通过反射移除三个投掷模板 WeaponDescriptions 数组中的近战 description，并**同步移除**
    /// _statDataValues 中同一索引的 stat 行，保证两数组索引对齐（CraftingTemplate.GetStatDatas
    /// 按 WeaponDescription.StringId 索引 _statDataValues[usageIndex]，只删 description 不删 stat
    /// 行会导致 GetStatDatas 错位读取）。
    ///
    /// 映射（模板 StringId → 需移除的近战 WeaponDescription StringId）：
    ///   "Javelin"       → "OneHandedPolearm_JavelinAlternative"
    ///   "ThrowingAxe"   → "OneHandedAxe"
    ///   "ThrowingKnife" → "Dagger"
    ///
    /// 注意：只移除三个投掷模板各自的近战 description，绝不全局移除 Dagger/OneHandedAxe
    /// 等与真实近战模板共享的 description。若某模板已不含目标 description（被其它 mod 改过），
    /// 保持原样不处理。修改是幂等的：LoadBasicFiles 每次进游戏都会重新加载模板（回到原版状态），
    /// Postfix 每次都会重新执行一次剔除。
    ///
    /// 每次进入游戏（新开/读档/自定义战斗）都会按 MCM 开关当前值生效一次；
    /// 战役中途修改不生效（物品已在进入时生成烘焙），故 RequireRestart = true。
    /// </summary>
    internal static class DisableThrownWeaponMeleePatch
    {
        /// <summary>投掷模板 StringId → 需移除的近战 WeaponDescription StringId。</summary>
        private static readonly Dictionary<string, string> MeleeDescriptionByTemplate =
            new Dictionary<string, string>
            {
                { "Javelin", "OneHandedPolearm_JavelinAlternative" },
                { "ThrowingAxe", "OneHandedAxe" },
                { "ThrowingKnife", "Dagger" },
            };

        /// <summary>WeaponDescriptions 属性（public getter + private setter，SetValue 反射可写私有 setter）。</summary>
        private static readonly PropertyInfo WeaponDescriptionsProp =
            typeof(CraftingTemplate).GetProperty("WeaponDescriptions");

        /// <summary>_statDataValues 私有字段（与 WeaponDescriptions 索引对齐的 stat 行数组）。</summary>
        private static readonly FieldInfo StatDataValuesField =
            typeof(CraftingTemplate).GetField("_statDataValues", BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// Game.LoadBasicFiles Postfix：基础数据（含三个投掷锻造模板）已加载、物品尚未生成。
        /// 遍历全部锻造模板，移除三个投掷模板的近战 description 与对应 stat 行。
        /// </summary>
        public static void Postfix()
        {
            try
            {
                // MCM 开关（RequireRestart = true）—— 关闭时不干预
                if (Settings.Instance?.DisableThrownWeaponMeleeEnabled != true)
                {
                    return;
                }

                if (WeaponDescriptionsProp == null || StatDataValuesField == null)
                {
                    return;
                }

                foreach (CraftingTemplate template in CraftingTemplate.All)
                {
                    if (template == null || !MeleeDescriptionByTemplate.TryGetValue(template.StringId, out string meleeId))
                    {
                        continue;
                    }

                    RemoveMeleeUsage(template, meleeId);
                }
            }
            catch (Exception ex)
            {
                // 数据改造：单点失败不应导致游戏崩溃，记录到 CrashLog 供排查（不吞异常日志，不 rethrow）
                CrashLog.Write("DisableThrownWeaponMelee", ex);
            }
        }

        /// <summary>
        /// 从单个投掷模板移除指定近战 description 及其 _statDataValues 行（保持两数组索引对齐）。
        /// 若模板不含目标 description，则保持原样不修改。
        /// </summary>
        private static void RemoveMeleeUsage(CraftingTemplate template, string meleeId)
        {
            WeaponDescription[] descriptions = (WeaponDescription[])WeaponDescriptionsProp.GetValue(template);
            if (descriptions == null || descriptions.Length == 0)
            {
                return;
            }

            float[][] statRows = (float[][])StatDataValuesField.GetValue(template);

            // 记录保留项（StringId != meleeId）的原始索引
            var keptIndices = new List<int>(descriptions.Length);
            bool removedAny = false;
            for (int i = 0; i < descriptions.Length; i++)
            {
                if (descriptions[i] != null && descriptions[i].StringId == meleeId)
                {
                    removedAny = true;
                }
                else
                {
                    keptIndices.Add(i);
                }
            }

            if (!removedAny)
            {
                return; // 该模板不含目标近战 description（如被其它 mod 修改过），保持不变
            }

            var newDescriptions = new WeaponDescription[keptIndices.Count];
            var newStatRows = new float[keptIndices.Count][];
            for (int k = 0; k < keptIndices.Count; k++)
            {
                int originalIndex = keptIndices[k];
                newDescriptions[k] = descriptions[originalIndex];
                // _statDataValues 与 WeaponDescriptions 在 Deserialize 时同步构建、索引严格对齐；
                // 若异常（被其它 mod 改动）则用空行占位，仍保证两数组索引对齐。
                newStatRows[k] = (statRows != null && originalIndex < statRows.Length)
                    ? statRows[originalIndex]
                    : Array.Empty<float>();
            }

            WeaponDescriptionsProp.SetValue(template, newDescriptions);
            StatDataValuesField.SetValue(template, newStatRows);
        }
    }
}
