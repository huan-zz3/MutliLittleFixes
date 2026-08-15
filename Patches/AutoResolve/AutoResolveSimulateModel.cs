using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace MutliLittleFixes.Patches
{
    /// <summary>
    /// 坐镇指挥模拟重平衡 —— 单次命中伤害辅助计算（纯武器伤害模型）。
    ///
    /// 原版伤害（DefaultCombatSimulationModel.SimulateHit）只由军事力比 × 优势 × 士气/perk 决定，
    /// 完全不读士兵武器。本类改为纯武器伤害模型：
    /// - 伤害来源按 4×4 优先级表选择（攻方兵种 × 守方兵种 → 武器类别），见 SelectWeapon；
    /// - 伤害数值 = 选中武器的面板伤害（近战 SwingDamage / 远程 ThrustDamage），不掺战力比/优势；
    /// - 伤害类型 = 选中武器的伤害类型（挥砍/穿刺/钝击），护甲减伤按该类型走原版 ComputeRawDamage 公式；
    /// - 步兵/骑手有概率（默认 5%）使用标枪远程攻击；只带标枪的兵 100% 使用；
    /// - 随机命中部位的护甲值（头 / 臂 / 腿 / 躯干，完全使用原版部位护甲）；
    /// - 盾牌格挡判定（持盾的步兵/骑手被攻击时有概率挡住，伤害为 0）。
    ///
    /// 对应旧版 AutoResolveRebalanced 的 SimulateModel。
    /// </summary>
    internal static class AutoResolveSimulateModel
    {
        /// <summary>武器选择结果：伤害基准 + 伤害类型 + 是否远程（用于命中判定）+ 武器名（供坐镇日志记录）。</summary>
        internal struct WeaponSelection
        {
            /// <summary>面板伤害基准（近战 SwingDamage / 远程 ThrustDamage）。</summary>
            public int Damage;
            /// <summary>伤害类型（护甲减伤按此类型计算）。</summary>
            public DamageTypes DamageType;
            /// <summary>是否远程攻击（弓/弩/标枪，需过命中判定）。</summary>
            public bool IsRanged;
            /// <summary>武器名（ItemObject.Name，空手时为 "Unarmed"），仅用于坐镇日志 CSV。</summary>
            public string WeaponName;
        }

        /// <summary>
        /// 按 4×4 优先级表选择本次命中的伤害来源武器。
        ///
        /// 表规则（攻方行 × 守方列 → 首选武器类别，找不到时按 fallback 链降级）：
        ///   ┌──────────┬──────────┬──────────┬──────────┬──────────┐
        ///   │ 攻方\守方 │ 步兵     │ 射手     │ 骑手     │ 骑射手   │
        ///   ├──────────┼──────────┼──────────┼──────────┼──────────┤
        ///   │ 步兵     │ 单手/双手│ 单手/双手│ 长杆     │ 长杆     │
        ///   │ 射手     │ 弓/弩    │ 弓/弩    │ 弓/弩    │ 弓/弩    │
        ///   │ 骑手     │ 单手/双手│ 单手/双手│ 长杆     │ 长杆     │
        ///   │ 骑射手   │ 弓/弩    │ 弓/弩    │ 弓/弩    │ 弓/弩    │
        ///   └──────────┴──────────┴──────────┴──────────┴──────────┘
        /// 标枪：非远程的步兵/骑手若携带标枪，先按「使用标枪概率」掷骰（默认 5%）；
        /// 只带标枪、没有其他武器的兵概率为 100%。射手/骑射手不判定标枪。
        /// 完全找不到武器时回退到空手：40 基准 + 钝击（与原版基础伤害一致）。
        /// </summary>
        public static WeaponSelection SelectWeapon(CharacterObject strikerTroop, CharacterObject struckTroop)
        {
            bool strikerIsRanged = strikerTroop.IsRanged;
            bool struckIsMounted = struckTroop.IsMounted;

            // 射手 / 骑射手：弓/弩（含投石），伤害 = ThrustDamage（远程），类型 = ThrustDamageType
            if (strikerIsRanged)
            {
                WeaponComponentData bow = FindWeaponByClass(strikerTroop, WeaponClass.Bow, WeaponClass.Crossbow, WeaponClass.Sling);
                if (bow != null)
                {
                    return new WeaponSelection
                    {
                        Damage = bow.ThrustDamage,
                        DamageType = bow.ThrustDamageType,
                        IsRanged = true,
                        WeaponName = GetWeaponName(strikerTroop, WeaponClass.Bow, WeaponClass.Crossbow, WeaponClass.Sling)
                    };
                }
                // 未带弓/弩（罕见）：回退到近战逻辑
            }

            // 步兵 / 骑手：先判定标枪
            bool hasJavelin = HasWeaponClass(strikerTroop, WeaponClass.Javelin, WeaponClass.ThrowingAxe, WeaponClass.ThrowingKnife);
            if (hasJavelin)
            {
                float javelinChance = Settings.Instance?.AutoResolveJavelinChance ?? 0.05f;
                bool hasMelee = HasMeleeWeapon(strikerTroop);
                if (!hasMelee)
                {
                    javelinChance = 1f; // 只带标枪 → 100% 使用
                }
                if (MBRandom.RandomFloat < javelinChance)
                {
                    WeaponComponentData javelin = FindWeaponByClass(strikerTroop, WeaponClass.Javelin, WeaponClass.ThrowingAxe, WeaponClass.ThrowingKnife);
                    if (javelin != null)
                    {
                        return new WeaponSelection
                        {
                            Damage = javelin.ThrustDamage,
                            DamageType = javelin.ThrustDamageType,
                            IsRanged = true,
                            WeaponName = GetWeaponName(strikerTroop, WeaponClass.Javelin, WeaponClass.ThrowingAxe, WeaponClass.ThrowingKnife)
                        };
                    }
                }
            }

            // 近战：按守方是否骑乘选类别
            WeaponComponentData melee = SelectMeleeWeapon(strikerTroop, struckIsMounted);
            if (melee != null)
            {
                return new WeaponSelection
                {
                    Damage = melee.SwingDamage,
                    DamageType = melee.SwingDamageType,
                    WeaponName = GetWeaponName(strikerTroop,
                        WeaponClass.OneHandedPolearm, WeaponClass.TwoHandedPolearm, WeaponClass.LowGripPolearm,
                        WeaponClass.Dagger, WeaponClass.OneHandedSword, WeaponClass.OneHandedAxe,
                        WeaponClass.Mace, WeaponClass.Pick,
                        WeaponClass.TwoHandedSword, WeaponClass.TwoHandedAxe, WeaponClass.TwoHandedMace)
                };
            }

            // 空手兜底：原版基础伤害 40 + 钝击
            return new WeaponSelection { Damage = 40, DamageType = DamageTypes.Blunt, WeaponName = "Unarmed" };
        }

        /// <summary>按类别查找武器的同时返回武器名（ItemObject.Name），仅用于坐镇日志；找不到返回空串。</summary>
        private static string GetWeaponName(CharacterObject troop, params WeaponClass[] weaponClasses)
        {
            Equipment equipment = troop.Equipment;
            for (int i = 0; i < 5; i++)
            {
                EquipmentElement element = equipment[(EquipmentIndex)i];
                if (element.IsEmpty || element.Item == null)
                {
                    continue;
                }
                WeaponComponentData primary = element.Item.PrimaryWeapon;
                if (primary == null)
                {
                    continue;
                }
                foreach (WeaponClass weaponClass in weaponClasses)
                {
                    if (primary.WeaponClass == weaponClass)
                    {
                        return element.Item.Name?.ToString() ?? weaponClass.ToString();
                    }
                }
            }
            return string.Empty;
        }

        /// <summary>按守方骑乘状态选近战武器：骑乘目标长杆优先（刺击），非骑乘目标单手/双手优先。</summary>
        private static WeaponComponentData SelectMeleeWeapon(CharacterObject strikerTroop, bool struckIsMounted)
        {
            if (struckIsMounted)
            {
                // 守方骑乘 → 长杆优先，fallback 单手/双手
                WeaponComponentData polearm = FindWeaponByClass(strikerTroop,
                    WeaponClass.OneHandedPolearm, WeaponClass.TwoHandedPolearm, WeaponClass.LowGripPolearm);
                if (polearm != null)
                {
                    return polearm;
                }
                return FindWeaponByClass(strikerTroop,
                    WeaponClass.Dagger, WeaponClass.OneHandedSword, WeaponClass.OneHandedAxe,
                    WeaponClass.Mace, WeaponClass.Pick,
                    WeaponClass.TwoHandedSword, WeaponClass.TwoHandedAxe, WeaponClass.TwoHandedMace);
            }
            // 守方非骑乘 → 单手/双手优先，fallback 长杆
            WeaponComponentData oneHanded = FindWeaponByClass(strikerTroop,
                WeaponClass.Dagger, WeaponClass.OneHandedSword, WeaponClass.OneHandedAxe,
                WeaponClass.Mace, WeaponClass.Pick,
                WeaponClass.TwoHandedSword, WeaponClass.TwoHandedAxe, WeaponClass.TwoHandedMace);
            if (oneHanded != null)
            {
                return oneHanded;
            }
            return FindWeaponByClass(strikerTroop,
                WeaponClass.OneHandedPolearm, WeaponClass.TwoHandedPolearm, WeaponClass.LowGripPolearm);
        }

        /// <summary>在士兵武器槽（0~4）中按类别查找第一把武器；找不到返回 null。</summary>
        private static WeaponComponentData FindWeaponByClass(CharacterObject troop, params WeaponClass[] weaponClasses)
        {
            Equipment equipment = troop.Equipment;
            for (int i = 0; i < 5; i++)
            {
                EquipmentElement element = equipment[(EquipmentIndex)i];
                if (element.IsEmpty || element.Item == null)
                {
                    continue;
                }
                WeaponComponentData primary = element.Item.PrimaryWeapon;
                if (primary == null)
                {
                    continue;
                }
                foreach (WeaponClass weaponClass in weaponClasses)
                {
                    if (primary.WeaponClass == weaponClass)
                    {
                        return primary;
                    }
                }
            }
            return null;
        }

        /// <summary>士兵是否携带近战武器（单手/双手/长杆）。</summary>
        private static bool HasMeleeWeapon(CharacterObject troop)
        {
            return FindWeaponByClass(troop,
                WeaponClass.Dagger, WeaponClass.OneHandedSword, WeaponClass.OneHandedAxe,
                WeaponClass.Mace, WeaponClass.Pick,
                WeaponClass.TwoHandedSword, WeaponClass.TwoHandedAxe, WeaponClass.TwoHandedMace,
                WeaponClass.OneHandedPolearm, WeaponClass.TwoHandedPolearm, WeaponClass.LowGripPolearm) != null;
        }

        /// <summary>士兵是否携带指定类别的武器。</summary>
        private static bool HasWeaponClass(CharacterObject troop, params WeaponClass[] weaponClasses)
        {
            return FindWeaponByClass(troop, weaponClasses) != null;
        }

        /// <summary>士兵是否携带盾牌。</summary>
        public static bool HasShield(CharacterObject troop)
        {
            return HasShield(troop.Equipment);
        }

        private static bool HasShield(Equipment equipment)
        {
            for (int i = 0; i < 5; i++)
            {
                EquipmentElement element = equipment[(EquipmentIndex)i];
                if (!element.IsEmpty
                    && element.Item != null
                    && element.Item.PrimaryWeapon != null
                    && element.Item.PrimaryWeapon.IsShield)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 复刻原版真实战斗护甲减伤公式（DefaultStrikeMagnitudeModel / SandboxStrikeMagnitudeModel.ComputeRawDamage）。
        ///
        /// 公式含义：
        ///   1. 打击力先经护甲有效性衰减：num2 = magnitude × 50/(50+护甲)；
        ///   2. 钝击因子（Blunt 0.6 / Cut 0.1 / Pierce 0.25）比例的伤害无视护甲直接穿透；
        ///   3. 剩余部分按伤害类型线性减甲（Cut 减 0.5×护甲 / Pierce 减 0.33×护甲 / Blunt 减 0.2×护甲），下限 0。
        /// absorbedDamageRatio 为格挡/吸收比，未格挡时为 1（坐镇模拟无格挡概念，恒传 1）。
        /// </summary>
        public static float ComputeRawDamage(DamageTypes damageType, float magnitude, float armorEffectiveness, float absorbedDamageRatio)
        {
            float bluntDamageFactorByDamageType = GetBluntDamageFactorByDamageType(damageType);
            float num = 50f / (50f + armorEffectiveness);
            float num2 = magnitude * num;
            float num3 = bluntDamageFactorByDamageType * num2;
            float num4;
            switch (damageType)
            {
                case DamageTypes.Cut:
                    num4 = MathF.Max(0f, num2 - armorEffectiveness * 0.5f);
                    break;
                case DamageTypes.Pierce:
                    num4 = MathF.Max(0f, num2 - armorEffectiveness * 0.33f);
                    break;
                case DamageTypes.Blunt:
                    num4 = MathF.Max(0f, num2 - armorEffectiveness * 0.2f);
                    break;
                default:
                    return 0f;
            }
            num3 += (1f - bluntDamageFactorByDamageType) * num4;
            return num3 * absorbedDamageRatio;
        }

        /// <summary>复刻原版钝击因子：Blunt 0.6、Cut 0.1、Pierce 0.25（该比例伤害无视护甲直接穿透）。</summary>
        public static float GetBluntDamageFactorByDamageType(DamageTypes damageType)
        {
            switch (damageType)
            {
                case DamageTypes.Blunt:
                    return 0.6f;
                case DamageTypes.Cut:
                    return 0.1f;
                case DamageTypes.Pierce:
                    return 0.25f;
                default:
                    return 0f;
            }
        }

        /// <summary>随机命中部位并返回护甲值（头 / 臂 / 腿 / 躯干，完全使用原版部位护甲）。</summary>
        public static float GetArmorInRandomPart(CharacterObject troop)
        {
            Equipment equipment = troop.Equipment;
            float armor;
            switch (MBRandom.RandomInt(1, 6)) // 1~5，5 个部位权重
            {
                case 1:
                    armor = equipment.GetHeadArmorSum();
                    break;
                case 2:
                    armor = equipment.GetArmArmorSum();
                    break;
                case 3:
                    armor = equipment.GetLegArmorSum();
                    break;
                default:
                    armor = equipment.GetHumanBodyArmorSum();
                    break;
            }
            return armor;
        }
    }
}
