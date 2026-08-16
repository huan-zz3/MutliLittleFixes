using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace MutliLittleFixes
{
    /// <summary>
    /// 远程盾兵站位（线阵/散阵）：
    /// 仅对【实际装备弓/弩的士兵占比 &gt; 95%】的 Line / Loose 阵型生效。阵型中携带盾牌且装备
    /// 远程武器（弓/弩，不含标枪）的士兵按以下优先级重排站位：
    ///   1. 第一排（所有列，排满不留空）
    ///   2. 最左侧列与最右侧列（排满不留空）
    ///   3. 最后两排（排满不留空）
    ///   4. 中间的其余排（自第二排起）依次排满（人数较多时剩余的持盾远程）
    /// 使持盾远程在前方与两翼及后方形成保护壳，掩护阵型中其余士兵。
    ///
    /// 触发时机：编队布局变化（玩家调整长宽/阵型命令）时立即重排，之后每 1.5 秒周期重算
    /// （应对伤亡、第9队移入等动态变化）。
    ///
    /// 配套补丁 FormationFrontRankShieldSortPatch 已对非步兵编队（步兵占比 ≤ 95%）
    /// 禁用原生"持盾冒泡前排"机制，两者不再冲突，布局可稳定收敛。
    /// 主控玩家本人不会被移动；MCM 开关实时控制（关闭后停止干预，恢复原生排列）。
    /// </summary>
    public class ShieldBearerFormationBehavior : MissionLogic
    {
        private const float REARRANGE_INTERVAL = 1.5f;
        private float _timer;

        // 编队上次布局快照（阵型命令 + 宽 + 深），变化时立即触发重排
        private readonly Dictionary<Formation, (ArrangementOrder.ArrangementOrderEnum Order, float Width, float Depth)> _lastLayouts =
            new Dictionary<Formation, (ArrangementOrder.ArrangementOrderEnum Order, float Width, float Depth)>();

        // 已插盾士兵判定（ShieldPlantingBehavior），用于重排时跳过已插盾的士兵
        private ShieldPlantingBehavior? _plantingBehavior;

        public override void OnMissionTick(float dt)
        {
            if (Mission == null || Mission.Mode == MissionMode.Deployment)
                return;

            // MCM 运行时开关 — 关闭时不干预
            if (Settings.Instance?.ShieldBearerFormationEnabled != true)
                return;

            _timer -= dt;
            if (HasLayoutChanged())
                _timer = 0f; // 编队布局变化（玩家调整长宽/阵型命令）→ 立即重排
            if (_timer > 0f)
                return;
            _timer = REARRANGE_INTERVAL;

            RearrangeFormations();
        }

        /// <summary>
        /// 检测本方阵型布局是否变化（阵型命令/宽/深），并更新缓存。
        /// </summary>
        private bool HasLayoutChanged()
        {
            bool changed = false;
            Team? team = Mission.PlayerTeam;
            if (team == null) return false;

            foreach (Formation formation in team.FormationsIncludingEmpty)
            {
                if (formation.CountOfUnits <= 0) continue;

                var order = formation.ArrangementOrder.OrderEnum;
                float width = formation.Arrangement.Width;
                float depth = formation.Arrangement.Depth;

                if (_lastLayouts.TryGetValue(formation, out var last))
                {
                    if (last.Order != order
                        || Math.Abs(last.Width - width) > 0.01f
                        || Math.Abs(last.Depth - depth) > 0.01f)
                        changed = true;
                }
                _lastLayouts[formation] = (order, width, depth);
            }
            return changed;
        }

        private void RearrangeFormations()
        {
            Team? team = Mission.PlayerTeam;
            if (team == null) return;

            foreach (Formation formation in team.FormationsIncludingEmpty)
            {
                if (formation.CountOfUnits <= 0) continue;

                // 仅处理线阵（Line）与散阵（Loose）；1.4.5 两者都是 LineFormation（IsLoose 区分）
                ArrangementOrder.ArrangementOrderEnum order = formation.ArrangementOrder.OrderEnum;
                if (order != ArrangementOrder.ArrangementOrderEnum.Line
                    && order != ArrangementOrder.ArrangementOrderEnum.Loose)
                    continue;

                // 仅处理远程编队（远程兵占比 > 95%，兵种判定与 RangedNoAmmoBehavior 一致）
                if (!IsRangedFormation(formation)) continue;

                RearrangeFormation(formation);
            }
        }

        private void RearrangeFormation(Formation formation)
        {
            if (formation.Arrangement is not LineFormation line) return;

            // 获取已插盾士兵判定（已插盾的士兵不再参与交换，避免与自动插盾/收盾互相反转导致不收敛）
            _plantingBehavior ??= Mission.GetMissionBehavior<ShieldPlantingBehavior>();

            // FileCount 在 LineFormation 是 protected，无法直接访问；
            // 从所有已定位单位的 FormationFileIndex/RankIndex 推算行列数（未定位单位索引为 -1，跳过）
            int fileCount = 0;
            int rankCount = 0;
            foreach (IFormationUnit u in line.GetAllUnits())
            {
                if (u.FormationFileIndex >= 0)
                    fileCount = Math.Max(fileCount, u.FormationFileIndex + 1);
                if (u.FormationRankIndex >= 0)
                    rankCount = Math.Max(rankCount, u.FormationRankIndex + 1);
            }
            if (fileCount < 2 || rankCount < 2) return;

            // 收敛交换：目标是把所有持盾远程放到优先级最高的格子里。
            // 优先级：第一排(0) → 左右列(1) → 最后两排(2) → 中间各排自第二排起依次(3,4,5…)。
            // 每轮找"占着最优先格子的非持盾单位"与"占着最劣格子的持盾单位"交换，
            // 单调减小持盾单位占位的优先级总和，必然收敛（maxIterations 防御极端抖动死循环）。
            // 已插盾的士兵被跳过：不参与交换，也不会被换走（它们已定位在插盾位置）。
            int maxIterations = fileCount * rankCount * 2;
            for (int iter = 0; iter < maxIterations; iter++)
            {
                // 占着最优先格子的非持盾单位
                Agent? bestNonShield = null;
                int bestNonShieldPri = int.MaxValue;
                // 占着最劣格子的持盾单位
                Agent? worstShield = null;
                int worstShieldPri = int.MinValue;

                for (int r = 0; r < rankCount; r++)
                {
                    for (int f = 0; f < fileCount; f++)
                    {
                        if (line.GetUnit(f, r) is not Agent a || a.IsMainAgent) continue;
                        // 已插盾的士兵跳过（不参与交换，也不作为交换目标）
                        if (_plantingBehavior != null && _plantingBehavior.IsDeployed(a)) continue;
                        int pri = GetSlotPriority(f, r, fileCount, rankCount);
                        if (IsShieldBearer(a))
                        {
                            if (pri > worstShieldPri)
                            {
                                worstShieldPri = pri;
                                worstShield = a;
                            }
                        }
                        else
                        {
                            if (pri < bestNonShieldPri)
                            {
                                bestNonShieldPri = pri;
                                bestNonShield = a;
                            }
                        }
                    }
                }

                // 收敛条件：不存在"非持盾单位占着比持盾单位更优先的格子"
                if (worstShield == null || bestNonShield == null || bestNonShieldPri >= worstShieldPri)
                    break;

                line.SwitchUnitLocations(bestNonShield, worstShield);
            }
        }

        /// <summary>
        /// 站位优先级（数值越小越优先）：
        /// 0 = 第一排（排满不留空）；1 = 左右列（排满不留空）；
        /// 2 = 最后两排（排满不留空）；3,4,5… = 中间各排自第二排起依次排满。
        /// </summary>
        private static int GetSlotPriority(int file, int rank, int fileCount, int rankCount)
        {
            if (rank == 0) return 0;                        // 第一排
            if (file == 0 || file == fileCount - 1) return 1;  // 左右列
            if (rank >= rankCount - 2) return 2;            // 最后两排
            return 2 + rank;                                // 中间各排：rank 1→3, rank 2→4…
        }

        /// <summary>
        /// 远程编队判定：编队内【实际装备弓/弩】的士兵（不含标枪）占比 &gt; 95%。
        /// 与逐士兵的 HasRangedWeapon 判定一致——纯近战编队（无任何远程武器）永不触发；
        /// 判定标准与 RangedNoAmmoBehavior 的弹药检测一致（仅弓/弩，不含投掷武器）。
        /// </summary>
        private static bool IsRangedFormation(Formation formation)
        {
            int ranged = 0;
            int total = 0;
            formation.ApplyActionOnEachUnit(a =>
            {
                total++;
                if (HasRangedWeapon(a)) ranged++;
            });
            return total > 0 && ranged * 20 > total * 19; // 装备弓/弩占比 > 95%
        }

        /// <summary>
        /// 持盾远程判定：携带盾牌 + 装备远程武器（弓/弩，不含标枪）。
        /// 与 ShieldPlantingBehavior 判定规则一致（此处不排除骑乘，Line/Loose 为步兵阵型，骑射手不会出现）。
        /// </summary>
        private static bool IsShieldBearer(Agent agent)
            => agent != null && agent.Character != null
               && GetShieldSlot(agent) != EquipmentIndex.None
               && HasRangedWeapon(agent);

        private static EquipmentIndex GetShieldSlot(Agent agent)
        {
            if (agent?.Character == null) return EquipmentIndex.None;
            for (EquipmentIndex i = EquipmentIndex.WeaponItemBeginSlot; i < EquipmentIndex.NumAllWeaponSlots; i++)
            {
                MissionWeapon el = agent.Equipment[i];
                if (!el.IsEmpty && el.Item?.ItemType == ItemObject.ItemTypeEnum.Shield)
                    return i;
            }
            return EquipmentIndex.None;
        }

        private static bool HasRangedWeapon(Agent agent)
        {
            if (agent?.Character == null) return false;
            for (EquipmentIndex i = EquipmentIndex.WeaponItemBeginSlot; i < EquipmentIndex.NumAllWeaponSlots; i++)
            {
                MissionWeapon el = agent.Equipment[i];
                if (el.IsEmpty || el.Item == null) continue;
                ItemObject.ItemTypeEnum type = el.Item.ItemType;
                if (type == ItemObject.ItemTypeEnum.Bow
                    || type == ItemObject.ItemTypeEnum.Crossbow)
                    return true;
            }
            return false;
        }
    }
}
