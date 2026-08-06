using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;

namespace ExampleMod.Patches
{
    /// <summary>
    /// 自定义出场比例补丁（仅「单位生成优先级 = 高等级优先」时生效）。
    ///
    /// 目标方法：DefaultTroopSupplierProbabilityModel.EnqueueTroopSpawnProbabilitiesAccordingToUnitSpawnPrioritization
    /// （战役大地图战斗的出兵排序唯一入口；StoryMode 主线经 StoryModeTroopSupplierProbabilityModel
    ///  委托 BaseModel（即本类实例）调用同一方法，补丁一处即全覆盖；自定义战斗走 CustomBattleTroopSupplier，
    ///  不在此方法内，故不受影响）。
    ///
    /// 行为（由 MCM 开关实时控制，不重启生效）：
    ///   - 仅当 玩家部队 + 非攻城伏击 + 任务模式(includePlayer) + 游戏设置=HighLevel + 四比例总和>0 时接管；
    ///   - 普通兵按四类兵种（步兵/射手/骑兵/骑射手）的用户比例做加权轮转配额：
    ///       兵种内先按等级从高到低排列（同级保持名单顺序），再按「配额槽位」交错出场，
    ///       槽位 = 兵种内排名 k × 比例总和 ÷ 该兵种比例 → 比例大的兵种出场更频繁；
    ///       某兵种比例设为 0 则该兵种不登场（用于主动排除兵种）。
    ///   - 英雄 / 玩家角色 / 战前选兵(priorityTroops) 不受比例限制，仍按原版高等级优先逻辑（永远先于普通兵）；
    ///   - 其余情况（AI 部队 / 攻城伏击 / 其他模式 / 模拟结算）完全放行原版逻辑。
    ///
    /// 由 HarmonyPatchRegistry 显式注册（不使用 [HarmonyPatch] 属性）。
    /// </summary>
    internal static class UnitSpawnRatioPatch
    {
        /// <summary>
        /// 配额槽位权重单元：必须大于任何可能的兵种等级差（使槽位主导排序），
        /// 且与最大槽位数相乘不溢出 long。等级差远小于 10000，安全。
        /// </summary>
        private const int SlotUnit = 10000;

        internal static bool Prefix(
            MapEventParty battleParty,
            FlattenedTroopRoster priorityTroops,
            bool includePlayer,
            int sizeOfSide,
            bool forcePriorityTroops,
            List<(FlattenedTroopRosterElement, MapEventParty, float)> priorityList)
        {
            // MCM 运行时开关 — 关闭时不干预（Settings 为 null 时同样放行原版）
            Settings? settings = Settings.Instance;
            if (settings == null || !settings.UnitSpawnRatioEnabled)
            {
                return true;
            }

            // 仅玩家部队（与原版读取玩家设置的条件一致）；主菜单等 Campaign.Current 为空时 MainParty 为 null，
            // 此时任何 party != null → 放行原版，安全
            if (battleParty == null || battleParty.Party != PartyBase.MainParty)
            {
                return true;
            }

            // 攻城伏击战玩家也强制 HighLevel（原版行为），不接管
            if (PlayerEncounter.Battle?.IsSiegeAmbush ?? false)
            {
                return true;
            }

            // 模拟结算（includePlayer=false，所有兵同概率按 roster 原序）不干预
            if (!includePlayer)
            {
                return true;
            }

            // 仅在「高等级优先」下生效
            if (Game.Current.UnitSpawnPrioritization != UnitSpawnPrioritizations.HighLevel)
            {
                return true;
            }

            // 四项比例权重（相对值，总和无需 100；全为 0 时无意义，放行原版）
            int[] weights =
            {
                settings.InfantryRatio,
                settings.ArcherRatio,
                settings.CavalryRatio,
                settings.HorseArcherRatio,
            };
            int totalWeight = weights[0] + weights[1] + weights[2] + weights[3];
            if (totalWeight <= 0)
            {
                return true;
            }

            ApplyCustomOrdering(battleParty, priorityTroops, priorityList, weights, totalWeight);
            return false; // 跳过原方法
        }

        /// <summary>
        /// 自定义排序实现：复刻原方法的分组/概率结构（优先英雄 > 优先普通兵 > 英雄 > 普通兵），
        /// 仅替换普通兵的 key 计算为「兵种内等级降序 + 加权轮转配额槽位」。
        /// key 越大越先出（各组内升序排列后按位置分配递增概率）。
        /// </summary>
        private static void ApplyCustomOrdering(
            MapEventParty battleParty,
            FlattenedTroopRoster priorityTroops,
            List<(FlattenedTroopRosterElement, MapEventParty, float)> priorityList,
            int[] weights,
            int totalWeight)
        {
            var priorityHeroes = new List<KeyValuePair<long, FlattenedTroopRosterElement>>();
            var priorityRegulars = new List<KeyValuePair<long, FlattenedTroopRosterElement>>();
            var heroes = new List<KeyValuePair<long, FlattenedTroopRosterElement>>();
            var regularsByClass = new List<FlattenedTroopRosterElement>[4];
            for (int i = 0; i < 4; i++)
            {
                regularsByClass[i] = new List<FlattenedTroopRosterElement>();
            }

            foreach (FlattenedTroopRosterElement troop in battleParty.Troops)
            {
                // 过滤 死亡/溃逃/受伤（原版 CanTroopJoinBattle 在 includePlayer=true 时的等价逻辑）
                if (troop.IsWounded || troop.IsRouted || troop.IsKilled)
                {
                    continue;
                }

                CharacterObject character = troop.Troop;
                bool isHero = character.IsHero;
                // 英雄/优先组的内部顺序沿用 HighLevel（等级）；玩家角色永远最先出
                long key = character.Level;
                if (isHero && character.IsPlayerCharacter)
                {
                    key = int.MaxValue;
                }

                bool isPriority = false;
                if (priorityTroops != null)
                {
                    foreach (FlattenedTroopRosterElement priorityTroop in priorityTroops)
                    {
                        if (priorityTroop.Troop == character)
                        {
                            isPriority = true;
                            break;
                        }
                    }
                }

                if (isPriority)
                {
                    // isPriority=true 时 priorityTroops 必非 null（由上方循环验证），此处防御性判空
                    if (priorityTroops != null)
                    {
                        priorityTroops.Remove(priorityTroops.FindIndexOfCharacter(character));
                    }
                    if (isHero)
                    {
                        priorityHeroes.Add(new KeyValuePair<long, FlattenedTroopRosterElement>(key, troop));
                    }
                    else
                    {
                        priorityRegulars.Add(new KeyValuePair<long, FlattenedTroopRosterElement>(key, troop));
                    }
                }
                else if (isHero)
                {
                    heroes.Add(new KeyValuePair<long, FlattenedTroopRosterElement>(key, troop));
                }
                else
                {
                    int formationClass = MapToBasicFormationClass((int)character.GetFormationClass());
                    if (weights[formationClass] <= 0)
                    {
                        continue; // 比例设为 0 的兵种不登场
                    }
                    regularsByClass[formationClass].Add(troop);
                }
            }

            // 普通兵：兵种内等级降序（同级保持名单顺序）→ 加权轮转配额槽位
            // key = level - 槽位×SlotUnit：槽位小的先出（比例大的兵种槽位稀疏、出场更频繁），
            //       同槽位内等级高的先出（兵种内部按等级从高到低）。
            var regulars = new List<KeyValuePair<long, FlattenedTroopRosterElement>>();
            for (int fc = 0; fc < 4; fc++)
            {
                List<FlattenedTroopRosterElement> list = regularsByClass[fc];
                if (list.Count == 0)
                {
                    continue;
                }

                var indexed = new List<(FlattenedTroopRosterElement troop, int order)>(list.Count);
                for (int i = 0; i < list.Count; i++)
                {
                    indexed.Add((list[i], i));
                }
                indexed.Sort((a, b) =>
                {
                    int cmp = b.troop.Troop.Level.CompareTo(a.troop.Troop.Level); // 等级降序
                    return cmp != 0 ? cmp : a.order.CompareTo(b.order); // 同级保序（稳定）
                });

                for (int k = 0; k < indexed.Count; k++)
                {
                    int slot = k * totalWeight / weights[fc];
                    long key = indexed[k].troop.Troop.Level - (long)slot * SlotUnit;
                    regulars.Add(new KeyValuePair<long, FlattenedTroopRosterElement>(key, indexed[k].troop));
                }
            }

            // 与原版一致：各组内按 key 升序排列（key 越大概率越大 → 越先出）
            priorityHeroes = priorityHeroes.OrderBy(x => x.Key).ToList();
            priorityRegulars = priorityRegulars.OrderBy(x => x.Key).ToList();
            heroes = heroes.OrderBy(x => x.Key).ToList();
            regulars = regulars.OrderBy(x => x.Key).ToList();

            for (int i = 0; i < priorityHeroes.Count; i++)
            {
                priorityList.Add((priorityHeroes[i].Value, battleParty, 3f + (float)(i + 1) / priorityHeroes.Count));
            }
            for (int i = 0; i < priorityRegulars.Count; i++)
            {
                priorityList.Add((priorityRegulars[i].Value, battleParty, 2f + (float)(i + 1) / priorityRegulars.Count));
            }
            for (int i = 0; i < heroes.Count; i++)
            {
                priorityList.Add((heroes[i].Value, battleParty, 1f + (float)(i + 1) / heroes.Count));
            }
            for (int i = 0; i < regulars.Count; i++)
            {
                priorityList.Add((regulars[i].Value, battleParty, (float)(i + 1) / regulars.Count));
            }
        }

        /// <summary>
        /// 把任意 FormationClass 归并到四个基础兵种：0=步兵, 1=射手, 2=骑兵, 3=骑射手。
        /// 高级阵型类（4-7）归入其对应的基础类；其他/负数兜底为步兵。
        /// </summary>
        private static int MapToBasicFormationClass(int formationClass)
        {
            switch (formationClass)
            {
                case 1: return 1; // Ranged
                case 2: return 2; // Cavalry
                case 3: return 3; // HorseArcher
                case 4: return 1; // Skirmisher → 射手
                case 5: return 0; // HeavyInfantry → 步兵
                case 6: return 2; // HeavyCavalry → 骑兵
                case 7: return 3; // HeavyHorseArcher → 骑射手
                default: return 0; // Infantry / Unset / 其他 → 步兵
            }
        }
    }
}
