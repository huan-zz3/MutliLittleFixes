using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace ExampleMod
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
        private const float CheckInterval = 0.5f;
        private const float PurityThreshold = 0.95f;
        private const float MovingSpeedThresholdSq = 0.01f;
        private float _checkTimer;

        public override void OnMissionTick(float dt)
        {
            if (Mission == null || Mission.Mode == MissionMode.Deployment)
                return;

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
        /// 获取阵型中所有 Agent（包括分配到 detachment 的兵）。
        /// ApplyActionOnEachUnit 在 detach 场景下不覆盖所有 agent，
        /// 用 Team.ActiveAgents 按 Formation 过滤捕获全部。
        /// </summary>
        private static List<Agent> GetAllFormationAgents(Formation formation)
        {
            var agents = new List<Agent>();
            foreach (var agent in formation.Team.ActiveAgents)
            {
                if (agent.Formation == formation && agent.IsActive() && agent.IsHuman)
                    agents.Add(agent);
            }
            return agents;
        }

        /// <summary>
        /// 遍历阵型中所有活跃 Agent，按当前骑乘状态和角色兵种分类：
        /// - 骑马 → mounted，不计入有效步兵/远程
        /// - 下马且角色为 Infantry/Cavalry → 有效步兵
        /// - 下马且角色为 Ranged/HorseArcher → 有效远程
        /// </summary>
        private static (int effectiveInfantry, int effectiveRanged, int total) ClassifyFormation(Formation formation)
        {
            int effectiveInfantry = 0;
            int effectiveRanged = 0;
            int mounted = 0;

            foreach (var agent in GetAllFormationAgents(formation))
            {
                if (agent.HasMount)
                {
                    mounted++;
                    continue;
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
            }

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
        /// 线阵/盾阵：前一半的排（两阶段：先找最大 rank，然后 rank &lt;= maxRank/2）
        /// </summary>
        private static void ApplyCrouchForRangedFormation(Formation formation)
        {
            // 基于排数的两阶段逻辑
            // 第一遍：找出 eligible agent 中的最大 rank
            int maxRank = -1;
            formation.ApplyActionOnEachUnit(agent =>
            {
                if (!IsCrouchEligibleAgent(agent))
                    return;

                int rank = ((IFormationUnit)agent).FormationRankIndex;
                if (rank > maxRank)
                    maxRank = rank;
            });

            if (maxRank < 0)
            {
                ForceFormationToStand(formation);
                return;
            }

            int thresholdRank = maxRank / 2;

            // 第二遍：应用蹲下
            formation.ApplyActionOnEachUnit(agent =>
            {
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
        /// 松散阵使用 LineFormation 行为（非 detached），行列索引有效，
        /// MovementVelocity 判定在 LineFormation 行为下正常工作。
        /// 使用 GetAllFormationAgents 确保覆盖率（与 ApplyActionOnEachUnit 等效但更全面）。
        /// </summary>
        private static void ApplyCrouchForRangedLooseFormation(Formation formation)
        {
            foreach (var agent in GetAllFormationAgents(formation))
            {
                if (!IsCrouchEligibleAgent(agent))
                {
                    agent.SetCrouchMode(false);
                    continue;
                }

                agent.SetCrouchMode(true);
            }
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
        /// 强制阵型中所有 Agent 站起。
        /// </summary>
        private static void ForceFormationToStand(Formation formation)
        {
            foreach (var agent in GetAllFormationAgents(formation))
            {
                if (agent.CrouchMode)
                    agent.SetCrouchMode(false);
            }
        }
    }
}
