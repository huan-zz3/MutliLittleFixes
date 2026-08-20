using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace MutliLittleFixes.Patches
{
    /// <summary>
    /// 骑马长杆/骑枪对未上马单位必定击倒补丁。
    ///
    /// 目标方法：MissionCombatMechanicsHelper.DecideAgentKnockedDownByBlow
    /// （所有击倒判定的唯一汇聚点——Sandbox/Custom/Multiplayer 的 AgentApplyDamageModel
    /// 实现以及 Mission.ChargeDamageCallback 均转发至此，patch 一处即全覆盖）。
    ///
    /// 行为（由 MCM 开关实时控制，敌我双方对称生效）：
    ///   - 架矛骑枪（攻击者骑马 + IsDoingPassiveAttack）命中未上马的人形单位 → 必定击倒
    ///   - 马上普通长杆刺击（攻击者骑马 + !IsDoingPassiveAttack）命中未上马的人形单位 → 必定击倒
    ///
    /// 保留原版 ShrugOff 门：格挡化解（ShrugOff）的命中不强制击倒。
    /// 下马的骑兵 HasMount == false，自动视作步兵，命中同样生效；上马士兵不受影响。
    ///
    /// 由 HarmonyPatchRegistry 显式注册（不使用 [HarmonyPatch] 属性）。
    /// </summary>
    internal static class MountedKnockDownPatch
    {
        /// <summary>MCM 实时开关：架矛骑枪必定击倒。</summary>
        private static bool IsCouchLanceEnabled =>
            Settings.Instance?.CouchLanceKnockDownEnabled != false;

        /// <summary>MCM 实时开关：马上长杆刺击必定击倒。</summary>
        private static bool IsMountedThrustEnabled =>
            Settings.Instance?.MountedPolearmThrustKnockDownEnabled != false;

        /// <summary>MCM 实时阈值：马上长杆刺击触发必定击倒所需的最小相对速度（默认 1.0，0 表示不限制）。</summary>
        private static float MinRelativeSpeed =>
            Settings.Instance?.MountedPolearmThrustMinRelativeSpeed ?? 1.0f;

        /// <summary>MCM 实时加成：马上长杆刺击触发必定击倒时，本次攻击的伤害加成比例（默认 0.3 = +30%，0 表示无加成）。</summary>
        private static float KnockDownDamageBonus =>
            Settings.Instance?.MountedPolearmThrustKnockDownDamageBonus ?? 0.3f;

        internal static void Postfix(
            Agent attackerAgent,
            Agent victimAgent,
            WeaponComponentData attackerWeapon,
            ref Blow blow,
            ref bool __result)
        {
            // 海战禁用（战帆 DLC 海战/沿海掠夺海战）— 统一原则：士兵 AI 行为调整在海战不干预
            if (NavalBattleDetector.IsNavalBattle())
            {
                return;
            }

            // MCM 运行时开关 — 两个功能均关闭时不干预
            if (!IsCouchLanceEnabled && !IsMountedThrustEnabled)
            {
                return;
            }

            // 原版已判定击倒 → 无需干预
            if (__result)
            {
                return;
            }

            // 保留原版 ShrugOff 门：格挡化解的命中不强制击倒
            if ((blow.BlowFlag & BlowFlags.ShrugOff) != 0)
            {
                return;
            }

            // 攻击者必须骑马（两个功能均显式/隐含要求）
            if (attackerAgent == null || !attackerAgent.HasMount)
            {
                return;
            }

            // 武器必须是长杆（MeleeWeapon | WideGrip，骑枪与普通长杆均满足）
            if (attackerWeapon == null || !attackerWeapon.IsPolearm)
            {
                return;
            }

            // 必须是刺击（架矛的 strikeType 也是 Thrust）
            if (blow.StrikeType != StrikeType.Thrust)
            {
                return;
            }

            // 目标必须是未上马的人形单位（下马骑兵视作步兵；上马士兵不受影响）
            if (victimAgent == null || !victimAgent.IsHuman || victimAgent.HasMount)
            {
                return;
            }

            // 分流：架矛（被动攻击）走开关 A；普通刺击走开关 B
            if (attackerAgent.IsDoingPassiveAttack)
            {
                if (!IsCouchLanceEnabled)
                {
                    return;
                }
            }
            else
            {
                if (!IsMountedThrustEnabled)
                {
                    return;
                }

                // 相对速度门槛：避免原地刺击也必定击倒（0 = 不限制）
                if (MinRelativeSpeed > 0f
                    && ComputeRelativeSpeedDiffOfAgents(attackerAgent, victimAgent) < MinRelativeSpeed)
                {
                    return;
                }

                // 本次攻击判定击倒 → 按 MCM 比例加成伤害（0 = 无加成）
                float bonus = KnockDownDamageBonus;
                if (bonus > 0f)
                {
                    blow.BaseMagnitude *= 1f + bonus;
                    blow.InflictedDamage = TaleWorlds.Library.MathF.Round(
                        blow.InflictedDamage * (1f + bonus));
                }
            }

            // 满足所有条件 → 必定击倒
            __result = true;
        }

        /// <summary>
        /// 计算攻击者与目标的相对移动速度（与原版 MissionCombatMechanicsHelper.ComputeRelativeSpeedDiffOfAgents
        /// 同源逻辑：骑乘时取坐骑前向速度向量，步行时取自身移动速度向量，二者之差的长度的绝对值）。
        /// </summary>
        private static float ComputeRelativeSpeedDiffOfAgents(Agent agentA, Agent agentB)
        {
            Vec2 speedA = Vec2.Zero;
            if (agentA.MountAgent != null)
            {
                speedA = agentA.MountAgent.MovementVelocity.y * agentA.MountAgent.GetMovementDirection();
            }
            else
            {
                speedA = agentA.MovementVelocity;
                speedA.RotateCCW(agentA.MovementDirectionAsAngle);
            }

            Vec2 speedB = Vec2.Zero;
            if (agentB.MountAgent != null)
            {
                speedB = agentB.MountAgent.MovementVelocity.y * agentB.MountAgent.GetMovementDirection();
            }
            else
            {
                speedB = agentB.MovementVelocity;
                speedB.RotateCCW(agentB.MovementDirectionAsAngle);
            }

            return (speedA - speedB).Length;
        }
    }
}
