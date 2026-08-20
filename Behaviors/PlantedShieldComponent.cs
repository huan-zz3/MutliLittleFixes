using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace MutliLittleFixes
{
    /// <summary>
    /// 插地盾血量组件（挂在插地盾实体上）。
    ///
    /// 原理：原版近战/弹矢命中实体时，伤害会经 Mission.OnEntityHit 路由到实体上的
    /// MissionObject 组件（Mission.MissileHitCallback 还要求命中实体链上必须存在
    /// MissionObject 脚本，否则弹矢伤害结算会被整体跳过——因此插地盾必须挂此组件
    /// 才能接收箭/弩/投掷伤害）。本组件重写 OnHit 接收伤害并扣除自身血量：
    /// - 伤害值为原版结算后的 InflictedDamage（近战挥砍与箭/弩/投掷均走同一管线）；
    /// - 血量归零时触发 OnDestroyed 回调（ShieldPlantingBehavior 注册，延迟到下一帧
    ///   移除实体，不在引擎伤害调用栈内修改场景）；
    /// - 血量来源：插盾瞬间士兵盾牌的当前剩余耐久（MissionWeapon.HitPoints）
    ///   乘以 MCM 百分比（ShieldPlantingShieldHpPercent，默认 50%），插盾时一次性确定。
    ///
    /// MCM 实时开关（ShieldPlantingShieldHpEnabled）：关闭时不扣血，插地盾恢复为
    /// 不可摧毁的纯障碍物；重新开启后按当前剩余血量继续扣减。
    /// </summary>
    public class PlantedShieldComponent : MissionObject
    {
        /// <summary>插盾的归属士兵（盾碎后用于反查并清理追踪状态）</summary>
        public Agent? OwnerAgent;

        /// <summary>插地盾最大血量（= 士兵拿起时盾牌最大耐久 × MCM 百分比）</summary>
        public float MaxHitPoints;

        /// <summary>插地盾当前血量</summary>
        public float HitPoints;

        /// <summary>插盾时应用的 MCM 百分比（默认 0.5 = 50%）。收盾时用于把插地盾剩余血量反算回士兵盾牌耐久（保存插盾时值，防止设置中途变化导致换算失真）</summary>
        public float PlantingPercent = 1.0f;

        /// <summary>血量归零回调（ShieldPlantingBehavior 注册，用于延迟移除实体）</summary>
        public Action<PlantedShieldComponent>? OnDestroyed;

        /// <summary>
        /// 原版伤害路由入口（Mission.OnEntityHit 对命中实体上的每个 MissionObject 调用，
        /// 返回 true 表示本组件已处理，不再向父链查找）。
        /// 返回 true 与 reportDamage=false：插地盾受击不产生战斗日志，与挂组件前行为一致。
        /// </summary>
        protected override bool OnHit(Agent attackerAgent, int damage, Vec3 impactPosition, Vec3 impactDirection,
            in MissionWeapon weapon, int affectorWeaponSlotOrMissileIndex,
            ScriptComponentBehavior attackerScriptComponentBehavior,
            out bool reportDamage, out float finalDamage, out float fireDamage, out float modifiedFireDamage)
        {
            reportDamage = false;
            finalDamage = damage;
            fireDamage = -1f;
            modifiedFireDamage = -1f;

            // MCM 实时开关 — 关闭时不干预（插地盾保持不可摧毁的纯障碍物）
            if (Settings.Instance?.ShieldPlantingShieldHpEnabled != true)
                return true;

            if (damage <= 0)
                return true;

            HitPoints -= damage;
            if (HitPoints <= 0f)
                OnDestroyed?.Invoke(this);

            return true;
        }
    }
}