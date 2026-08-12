using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace MutliLittleFixes
{
    /// <summary>
    /// 协调管理玩家设定的投石机目标坐标。
    /// 存储全局目标位 + 每台武器的映射，供 AI Patch 查询。
    /// </summary>
    public static class CoordinateTargetManager
    {
        /// <summary>
        /// 目标点 Z 轴偏移量（米）。唯一的真值来源：
        /// AI 瞄准（CoordinateTargetAIPatch）与目标标记渲染（SiegeTrajectoryBehavior）
        /// 都基于抬高后的目标点工作，保证落点与标记一致。
        /// </summary>
        public const float TargetZOffset = 1.5f;

        /// <summary>武器 → 目标坐标映射</summary>
        private static readonly Dictionary<RangedSiegeWeapon, Vec3> _weaponTargets = new();

        /// <summary>当前是否有活跃的目标点</summary>
        public static bool IsActive { get; private set; }

        /// <summary>全局目标位置（用于渲染标记）</summary>
        public static Vec3? GlobalTargetPosition { get; private set; }

        /// <summary>
        /// 设定目标。会先清除旧目标。
        /// </summary>
        public static void SetTarget(List<RangedSiegeWeapon> weapons, Vec3 targetPos)
        {
            ClearAll();
            GlobalTargetPosition = targetPos;
            IsActive = true;
            foreach (var w in weapons)
            {
                _weaponTargets[w] = targetPos;
            }
        }

        /// <summary>
        /// 尝试获取指定武器的自定义目标。
        /// </summary>
        public static bool TryGetTarget(RangedSiegeWeapon weapon, out Vec3 target)
        {
            return _weaponTargets.TryGetValue(weapon, out target);
        }

        /// <summary>
        /// 当武器弹药耗尽、被摧毁或不再可用时从管理中移除。
        /// 全部武器移除后自动清除全局目标。
        /// </summary>
        public static void RemoveWeapon(RangedSiegeWeapon weapon)
        {
            _weaponTargets.Remove(weapon);
            if (_weaponTargets.Count == 0)
            {
                IsActive = false;
                GlobalTargetPosition = null;
            }
        }

        /// <summary>
        /// 清除所有目标，武器返回原版AI逻辑。
        /// </summary>
        public static void ClearAll()
        {
            _weaponTargets.Clear();
            IsActive = false;
            GlobalTargetPosition = null;
        }
    }
}
