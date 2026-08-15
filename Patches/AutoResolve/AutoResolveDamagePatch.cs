using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.Core;

namespace MutliLittleFixes.Patches
{
    /// <summary>
    /// 坐镇指挥模拟重平衡 —— 单次命中伤害改为「纯武器伤害」模型。
    ///
    /// 原版伤害（DefaultCombatSimulationModel.SimulateHit）由军事力比 × 优势 × 士气/perk 决定，
    /// 完全不含武器数值。本补丁完全重写伤害：
    ///   1. 按 4×4 优先级表选武器（攻方兵种 × 守方兵种 → 武器类别）；
    ///   2. 伤害基准 = 选中武器的面板伤害（近战 SwingDamage / 远程 ThrustDamage），不含战力比/优势；
    ///   3. 伤害类型 = 选中武器的伤害类型（挥砍/穿刺/钝击），走原版 ComputeRawDamage 护甲减伤公式；
    ///   4. 远程（弓/弩/标枪）攻击需过命中判定，未命中伤害为 0；
    ///   5. 持盾的步兵/骑手被攻击时有概率格挡（盾牌格挡），伤害为 0。
    ///
    /// 适配说明：1.4.5 中伤害计算下沉到 CombatSimulationModel.SimulateHit（返回 ExplainedNumber），
    /// 故改为 Postfix 修改 __result（ExplainedNumber 为 struct，用 ref 参数改写；直接重建为纯数值，
    /// 坐镇模拟不展示明细）。
    ///
    /// 对应旧版 AutoResolveRebalanced 的 Patch_GetSimulatedDamage（行为已按武器伤害模型重写）。
    /// </summary>
    internal static class AutoResolveDamagePatch
    {
        internal static void Postfix(ref ExplainedNumber __result, CharacterObject strikerTroop, CharacterObject struckTroop, MapEvent battle)
        {
            // MCM 运行时开关 — 关闭时不干预
            if (Settings.Instance?.AutoResolveEnabled != true)
                return;

            try
            {
                if (Settings.Instance.AutoResolveAiEnabled || battle.IsPlayerSimulation)
                {
                    // 盾牌格挡：持盾的步兵/骑手被攻击时有概率挡住（射手/骑射手不判定）
                    if (!struckTroop.IsRanged
                        && AutoResolveSimulateModel.HasShield(struckTroop)
                        && MBRandom.RandomFloat < Settings.Instance.AutoResolveShieldBlockChance)
                    {
                        __result = new ExplainedNumber(0f);
                        return;
                    }

                    // 4×4 表选择伤害来源武器
                    AutoResolveSimulateModel.WeaponSelection selection = AutoResolveSimulateModel.SelectWeapon(strikerTroop, struckTroop);

                    // 远程（弓/弩/标枪）命中判定：未命中则本次伤害为 0
                    if (selection.IsRanged
                        && MBRandom.RandomFloat >= Settings.Instance.AutoResolveRangedHitChance)
                    {
                        __result = new ExplainedNumber(0f);
                        return;
                    }

                    // 护甲减伤（原版 ComputeRawDamage 公式）
                    float armor = 0f;
                    if (Settings.Instance.AutoResolveArmorEnabled)
                    {
                        armor = AutoResolveSimulateModel.GetArmorInRandomPart(struckTroop);
                    }
                    float newDamage = AutoResolveSimulateModel.ComputeRawDamage(selection.DamageType, selection.Damage, armor, 1f);
                    if (newDamage < 1f)
                    {
                        newDamage = 1f;
                    }
                    __result = new ExplainedNumber(newDamage);
                }
            }
            catch (Exception ex)
            {
                AutoResolveLog.PrintError("[坐镇重平衡] 伤害补丁异常: " + ex);
            }
        }
    }
}
