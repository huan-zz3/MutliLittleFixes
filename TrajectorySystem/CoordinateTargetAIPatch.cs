using MCM.Abstractions.Base.Global;
using TaleWorlds.MountAndBlade;

namespace MutliLittleFixes
{
    /// <summary>
    /// Harmony Patch：当玩家设定了自定义目标坐标时，覆盖 AI 瞄准逻辑。
    /// 让 AI 操控的投石机持续向玩家指定的位置射击。
    /// 由 HarmonyPatchRegistry 显式注册。
    /// </summary>
    public static class CoordinateTargetAIPatch
    {
        /// <summary>
        /// 拦截 RangedSiegeWeaponAi.UpdateAim，替换为自定义目标瞄准逻辑。
        /// 如果没有自定义目标，透传回原版 AI。
        /// </summary>
        public static bool Prefix_UpdateAim(RangedSiegeWeaponAi __instance, RangedSiegeWeapon rangedSiegeWeapon, float dt)
        {
            // MCM 运行时开关 — 关闭时走原版 AI
            if (GlobalSettings<SiegeTrajectoryConfig>.Instance?.CoordinateTargetingEnabled != true)
                return true;

            // 检查是否有自定义目标
            if (!CoordinateTargetManager.TryGetTarget(rangedSiegeWeapon, out var targetPos))
                return true; // 无自定义目标 → 走原版 AI

            // 武器被摧毁或弹药耗尽 → 从管理中移除，退回原版 AI
            if (rangedSiegeWeapon.IsDestroyed || rangedSiegeWeapon.AmmoCount <= 0)
            {
                CoordinateTargetManager.RemoveWeapon(rangedSiegeWeapon);
                return true;
            }

            // 只有在 Idle 状态才能瞄准和射击
            if (rangedSiegeWeapon.State != RangedSiegeWeapon.WeaponState.Idle)
                return false;

            // 瞄准玩家目标
            bool isAimed = rangedSiegeWeapon.AimAtTarget(targetPos);

            // 瞄准到位且有 AI 驾驶员 → 请求开火
            if (isAimed && rangedSiegeWeapon.PilotAgent != null)
            {
                rangedSiegeWeapon.AiRequestsShoot();
            }

            return false; // 跳过原版 UpdateAim
        }
    }
}
