using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace MutliLittleFixes
{
    /// <summary>
    /// 阵型蹲下逻辑：
    /// 在纯步兵/纯远程小队中（百分比大于95%），当Agents所属小队处于Hold状态，
    /// 且Agents自身不在移动中时：
    /// - 线阵/盾阵：步兵首排蹲下、远程前半排蹲下
    /// - 松散阵：步兵不蹲、远程全体蹲下
    ///
    /// 下马的骑兵视为步兵，下马的骑射手视为远程。
    /// 其它任何状态、任何阵型都不触发蹲下。
    /// </summary>
    public class AutoCrouchMissionLogic : MissionLogic
    {
        private const float CheckInterval = 1.0f;
        private const float PurityThreshold = 0.95f;
        private const float MovingSpeedThresholdSq = 0.01f;
        private float _checkTimer;

        public override void OnMissionTick(float dt)
        {
            if (Mission == null || Mission.Mode == MissionMode.Deployment)
                return;

            // 海战禁用（战帆 DLC 海战/沿海掠夺海战）— 士兵随船移动，静止判定与编队状态不适用
            if (NavalBattleDetector.IsNavalBattle(Mission))
                return;

            // MCM 实时开关 — 关闭时强制站起所有可能被本功能蹲下的士兵
            if (Settings.Instance?.AutoCrouchEnabled != true)
            {
                ForceAllFormationsToStand();
                return;
            }

            _checkTimer += dt;
            if (_checkTimer < CheckInterval)
                return;
            _checkTimer = 0f;

            foreach (Team team in Mission.Teams)
            {
                if (team != Mission.PlayerTeam && team != Mission.PlayerAllyTeam)
                    continue;

                foreach (Formation formation in team.FormationsIncludingSpecialAndEmpty)
                {
                    if (formation.CountOfUnits == 0)
                        continue;

                    // 快速阵型级过滤
                    if (!IsFormationEligibleByState(formation))
                    {
                        ForceFormationToStand(formation);
                        continue;
                    }

                    // 分析阵型实际兵种构成（考虑下马状态）
                    var (effectiveInfantry, effectiveRanged, totalEffective) = ClassifyFormation(formation);

                    if (totalEffective == 0)
                    {
                        ForceFormationToStand(formation);
                        continue;
                    }

                    float infantryRatio = (float)effectiveInfantry / totalEffective;
                    float rangedRatio = (float)effectiveRanged / totalEffective;

                    bool isLoose = formation.ArrangementOrder.OrderEnum == ArrangementOrder.ArrangementOrderEnum.Loose;

                    if (infantryRatio >= PurityThreshold)
                    {
                        if (isLoose)
                        {
                            // 松散阵下步兵不蹲
                            ForceFormationToStand(formation);
                        }
                        else
                        {
                            ApplyCrouchForInfantryFormation(formation);
                        }
                    }
                    else if (rangedRatio >= PurityThreshold)
                    {
                        if (isLoose)
                        {
                            // 松散阵下远程全体蹲下
                            ApplyCrouchForRangedLooseFormation(formation);
                        }
                        else
                        {
                            ApplyCrouchForRangedFormation(formation);
                        }
                    }
                    else
                    {
                        ForceFormationToStand(formation);
                    }
                }
            }
        }

        /// <summary>
        /// 阵型级快速过滤：检查 MovementState 和 ArrangementOrder。
        /// 允许线阵、盾阵、松散阵（松散阵下蹲逻辑由调用方进一步限制）。
        /// </summary>
        private static bool IsFormationEligibleByState(Formation formation)
        {
            // 必须处于 Hold 状态（对应 Advance/Move/FollowEntity/AttackEntity/FallBack 等指令）
            if (formation.GetMovementState() != MovementOrder.MovementStateEnum.Hold)
                return false;

            // 阵型排列必须是线阵、盾阵、松散阵三者之一
            var arrangement = formation.ArrangementOrder.OrderEnum;
            if (arrangement != ArrangementOrder.ArrangementOrderEnum.Line &&
                arrangement != ArrangementOrder.ArrangementOrderEnum.ShieldWall &&
                arrangement != ArrangementOrder.ArrangementOrderEnum.Loose)
                return false;

            return true;
        }

        /// <summary>
        /// 遍历阵型中所有 Agent，按当前骑乘状态和角色兵种分类：
        /// - 骑马 → mounted，不计入有效步兵/远程
        /// - 下马且角色为 Infantry/Cavalry → 有效步兵
        /// - 下马且角色为 Ranged/HorseArcher → 有效远程
        ///
        /// 使用 formation.ApplyActionOnEachUnit 直接遍历本小队成员，
        /// 替代原 GetAllFormationAgents 扫全校列表再筛选的方式，减少 60-80% 的 agent 遍历。
        /// </summary>
        private static (int effectiveInfantry, int effectiveRanged, int total) ClassifyFormation(Formation formation)
        {
            int effectiveInfantry = 0;
            int effectiveRanged = 0;
            int mounted = 0;

            formation.ApplyActionOnEachUnit(agent =>
            {
                if (agent.HasMount)
                {
                    mounted++;
                    return;
                }

                // 取角色原始兵种（含未下马时的分类）
                FormationClass charClass = agent.Character?.GetFormationClass() ?? FormationClass.Unset;
                FormationClass defaultClass = charClass.DefaultClass();

                if (defaultClass == FormationClass.Infantry || defaultClass == FormationClass.Cavalry)
                {
                    effectiveInfantry++;
                }
                else if (defaultClass == FormationClass.Ranged || defaultClass == FormationClass.HorseArcher)
                {
                    effectiveRanged++;
                }
                // 其他（如 General/Bodyguard/Unset）不计入
            });

            int total = effectiveInfantry + effectiveRanged + mounted;
            return (effectiveInfantry, effectiveRanged, total);
        }

        /// <summary>
        /// 步兵阵型蹲下：
        /// 线阵/盾阵：仅第一排（rank 0）
        /// </summary>
        private static void ApplyCrouchForInfantryFormation(Formation formation)
        {
            formation.ApplyActionOnEachUnit(agent =>
            {
                // 玩家操控的角色不干预（AI 控制时正常处理）
                if (IsPlayerControlledAgent(agent))
                    return;

                if (!IsCrouchEligibleAgent(agent))
                {
                    agent.SetCrouchMode(false);
                    return;
                }

                // 仅第一排（rank 0）蹲下
                int rank = ((IFormationUnit)agent).FormationRankIndex;
                agent.SetCrouchMode(rank == 0);
            });
        }

        /// <summary>
        /// 远程阵型蹲下：
        /// 线阵/盾阵：前半排蹲下。
        /// 使用 Arrangement.RankCount 直接获取有效排数，替代原两遍扫描找 maxRank 再应用的方案。
        /// </summary>
        private static void ApplyCrouchForRangedFormation(Formation formation)
        {
            int rankCount = formation.Arrangement.RankCount;
            if (rankCount <= 0)
            {
                ForceFormationToStand(formation);
                return;
            }

            int thresholdRank = (rankCount - 1) / 2;

            formation.ApplyActionOnEachUnit(agent =>
            {
                // 玩家操控的角色不干预（AI 控制时正常处理）
                if (IsPlayerControlledAgent(agent))
                    return;

                if (!IsCrouchEligibleAgent(agent))
                {
                    agent.SetCrouchMode(false);
                    return;
                }

                int rank = ((IFormationUnit)agent).FormationRankIndex;
                agent.SetCrouchMode(rank <= thresholdRank);
            });
        }

        /// <summary>
        /// 远程松散阵蹲下：所有不移动的 eligible agent 全体蹲下。
        /// 使用 formation.ApplyActionOnEachUnit 替代 GetAllFormationAgents，
        /// 直接遍历本小队成员，避免扫全校列表。
        /// </summary>
        private static void ApplyCrouchForRangedLooseFormation(Formation formation)
        {
            formation.ApplyActionOnEachUnit(agent =>
            {
                // 玩家操控的角色不干预（AI 控制时正常处理）
                if (IsPlayerControlledAgent(agent))
                    return;

                if (!IsCrouchEligibleAgent(agent))
                {
                    agent.SetCrouchMode(false);
                    return;
                }

                agent.SetCrouchMode(true);
            });
        }

        /// <summary>
        /// 判断单个 Agent 是否具备蹲下资格：
        /// 存活、AI控制、未交互游戏对象（排除操作攻城器械的）、未骑马、不在移动中。
        /// </summary>
        private static bool IsCrouchEligibleAgent(Agent agent)
        {
            if (!agent.IsActive())
                return false;
            if (!agent.IsAIControlled)
                return false;
            if (agent.InteractingWithAnyGameObject())
                return false;
            if (agent.HasMount)
                return false;

            // 判定是否在移动：速度接近零才视为静止
            if (agent.MovementVelocity.LengthSquared > MovingSpeedThresholdSq)
                return false;

            return true;
        }

        /// <summary>
        /// 判断 Agent 是否正被玩家操控（Controller == Player）。
        /// 玩家操控的角色本 Mod 完全不干预——既不强制蹲下，也不强制站起；
        /// 当玩家角色转为 AI 控制（如委任托管）时，按普通士兵逻辑正常处理。
        /// </summary>
        private static bool IsPlayerControlledAgent(Agent agent)
        {
            return agent.IsMine;
        }

        /// <summary>
        /// 强制阵型中所有 Agent 站起。
        /// </summary>
        private static void ForceFormationToStand(Formation formation)
        {
            formation.ApplyActionOnEachUnit(agent =>
            {
                // 玩家操控的角色不干预（玩家自身蹲姿不受本 Mod 影响）
                if (IsPlayerControlledAgent(agent))
                    return;

                if (agent.CrouchMode)
                    agent.SetCrouchMode(false);
            });
        }

        /// <summary>
        /// 遍历玩家/友军全部阵型强制站起（MCM 开关关闭时调用，
        /// 防止士兵保持本功能造成的蹲姿）。
        /// </summary>
        private void ForceAllFormationsToStand()
        {
            foreach (Team team in Mission.Teams)
            {
                if (team != Mission.PlayerTeam && team != Mission.PlayerAllyTeam)
                    continue;

                foreach (Formation formation in team.FormationsIncludingSpecialAndEmpty)
                {
                    if (formation.CountOfUnits == 0)
                        continue;
                    ForceFormationToStand(formation);
                }
            }
        }
    }
}
