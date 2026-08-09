#if false
// ══════════════════════════════════════════════════════════════════════════
// 【已禁用】commit 304cc5d 新增的 ORCA 骑兵避障功能，整文件注释（不参与编译）。
// 原始代码完整保留；如需恢复：删除本行与文件末尾的 #endif 即可。
// 恢复时还需同步：Settings.cs 中 ORCA 配置、SubModule.cs 中 OrcaDebugBehavior 注册。
// ══════════════════════════════════════════════════════════════════════════
using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace MutliLittleFixes
{
    /// <summary>
    /// ORCA 算法的单个 Agent 输入快照。
    /// 纯数据：位置/速度/碰撞体（有向椭圆：朝向 + 半长轴 + 半短轴）/
    /// 期望速度/速度上限，全部在 XZ 平面（Vec2）。
    /// 椭圆建模：半长轴 HalfLength 沿朝向 Facing 方向，半短轴 HalfWidth 垂直朝向，
    /// 用支撑函数将圆-圆半径推广为朝向相关的等效组合半径（a==b 时退化为圆）。
    /// </summary>
    public struct OrcaAgent
    {
        public Vec2 Position;           // 当前位置
        public Vec2 Velocity;           // 当前速度
        public Vec2 Facing;             // 朝向（单位向量，沿马身方向；零向量=无朝向，退化为圆）
        public float HalfLength;        // 碰撞体半长轴（沿朝向，马≈1.2m）
        public float HalfWidth;         // 碰撞体半短轴（垂直朝向，马≈0.45m）
        public Vec2 PreferredVelocity;  // 期望速度（无冲突时想走的方向与速率）
        public float MaxSpeed;          // 速度上限
    }

    /// <summary>
    /// ORCA 半平面约束线。约束语义（RVO2 标准）：
    /// 可行速度 v 必须位于该线的「右侧」，即 det(Direction, Point - v) &gt;= 0。
    /// </summary>
    public struct OrcaLine
    {
        public Vec2 Direction;  // 线的方向（单位向量）
        public Vec2 Point;      // 线上一点
    }

    /// <summary>
    /// ORCA（Optimal Reciprocal Collision Avoidance）2D 求解器。
    /// 依据 RVO2-2D（Java 版）的算法逐行移植：
    ///   1. 对每个 Agent 相对每个邻居构造速度障碍 VO，reciprocal 化得到 ORCA 半平面；
    ///   2. linearProgram2：在 maxSpeed 圆内求离期望速度最近的可行速度；
    ///   3. 失败时 linearProgram3：沿冲突线投影求近似可行解。
    /// 零游戏依赖（仅 TaleWorlds.Library.Vec2），可独立测试。
    /// </summary>
    public sealed class OrcaSolver
    {
        private const float Epsilon = 1e-5f;

        /// <summary>时间窗：在此时间内保证不与邻居碰撞（秒）</summary>
        public float TimeHorizon { get; set; } = 1.5f;

        /// <summary>
        /// 邻居搜索半径（米）：仅与该半径内的其他 Agent 两两构造 ORCA 约束线
        /// （RVO2 的 neighborDist 语义）。超出此半径的 Agent 互不避让，显著降低 O(n²) 开销。
        /// 0 或负值 = 全部两两建线（不截断）。
        /// </summary>
        public float NeighborRadius { get; set; }

        /// <summary>并行约束线复用缓冲区（避免每帧 GC）</summary>
        private readonly List<OrcaLine> _orcaLines = new(64);

        /// <summary>
        /// 求解全部 Agent 的新速度。outVelocities 长度必须 &gt;= count，结果写入其中（复用缓冲）。
        /// 时间复杂度 O(n²)（两两构造 VO），n 建议 &lt;= 150。
        /// </summary>
        public void ComputeNewVelocities(OrcaAgent[] agents, int count, float timeStep, Vec2[] outVelocities)
        {
            if (outVelocities.Length < count)
                throw new ArgumentException("outVelocities 容量不足", nameof(outVelocities));

            for (int i = 0; i < count; i++)
            {
                _orcaLines.Clear();
                BuildOrcaLines(agents, count, i, timeStep, _orcaLines);

                Vec2 result;
                int lineFail = LinearProgram2(_orcaLines, agents[i].MaxSpeed, agents[i].PreferredVelocity, false, out result);
                if (lineFail < _orcaLines.Count)
                {
                    LinearProgram3(_orcaLines, 0, lineFail, agents[i].MaxSpeed, ref result);
                }
                outVelocities[i] = result;
            }
        }

        /// <summary>
        /// 为单个 Agent（索引 i）相对所有其他 Agent 构造 ORCA 半平面。
        /// 仅在 NeighborRadius 内的邻居才会建线（neighborDist 语义）。
        /// </summary>
        private void BuildOrcaLines(OrcaAgent[] agents, int count, int i, float timeStep, List<OrcaLine> lines)
        {
            ref readonly OrcaAgent a = ref agents[i];
            float invTimeHorizon = 1f / TimeHorizon;
            float neighborRadiusSq = NeighborRadius > 0f ? NeighborRadius * NeighborRadius : float.MaxValue;

            for (int j = 0; j < count; j++)
            {
                if (j == i) continue;
                ref readonly OrcaAgent b = ref agents[j];

                Vec2 relPos = b.Position - a.Position;
                float distSq = relPos.LengthSquared;
                if (distSq > neighborRadiusSq)
                    continue; // 超出邻居半径：不建约束线

                Vec2 relVel = a.Velocity - b.Velocity;

                // 朝向相关的等效组合半径（有向椭圆支撑函数）：
                //   椭圆在方向 d̂ 上的支撑投影 s(d̂) = sqrt(b² + (a²-b²)·cos²θ)，
                //   θ = d̂ 与该椭圆朝向的夹角（cosθ = dot(d̂, Facing)）。
                //   组合半径 R = sA(d̂) + sB(-d̂)；cos² 偶函数 → sB(-d̂)=sB(d̂)，直接用 d̂ 算两边。
                //   a==b 时退化为 R = 2r（纯圆，向后兼容）；Facing 为零向量时 cosθ=0 → s=b（退化为圆）。
                float combinedRadius;
                if (distSq > Epsilon)
                {
                    Vec2 dir = relPos / MathF.Sqrt(distSq);
                    float cosA = Vec2.DotProduct(dir, a.Facing);
                    float cosB = Vec2.DotProduct(dir, b.Facing);
                    float sA = MathF.Sqrt(a.HalfWidth * a.HalfWidth + (a.HalfLength * a.HalfLength - a.HalfWidth * a.HalfWidth) * cosA * cosA);
                    float sB = MathF.Sqrt(b.HalfWidth * b.HalfWidth + (b.HalfLength * b.HalfLength - b.HalfWidth * b.HalfWidth) * cosB * cosB);
                    combinedRadius = sA + sB;
                }
                else
                {
                    // 完全重合（无相对方向）：取最大投影兜底
                    combinedRadius = a.HalfLength + b.HalfLength;
                }
                float combinedRadiusSq = combinedRadius * combinedRadius;

                OrcaLine line = default;
                Vec2 u;

                if (distSq > combinedRadiusSq)
                {
                    // 未重叠：投影到 cutoff 圆或锥腿
                    Vec2 w = relVel - relPos * invTimeHorizon;
                    float wLengthSq = w.LengthSquared;
                    float dotProduct1 = Vec2.DotProduct(w, relPos);

                    if (dotProduct1 < 0f && dotProduct1 * dotProduct1 > combinedRadiusSq * wLengthSq)
                    {
                        // 投影到 cutoff 圆（相对速度方向已经指向远离，直线可避）
                        float wLength = MathF.Sqrt(wLengthSq);
                        Vec2 unitW = w / wLength;
                        line.Direction = new Vec2(unitW.y, -unitW.x);
                        u = unitW * (combinedRadius * invTimeHorizon - wLength);
                    }
                    else
                    {
                        // 投影到 VO 锥腿
                        float leg = MathF.Sqrt(distSq - combinedRadiusSq);
                        if (Det(relPos, w) > 0f)
                        {
                            // 左腿
                            line.Direction = new Vec2(
                                relPos.x * leg - relPos.y * combinedRadius,
                                relPos.x * combinedRadius + relPos.y * leg) / distSq;
                        }
                        else
                        {
                            // 右腿
                            line.Direction = new Vec2(
                                -relPos.x * leg - relPos.y * combinedRadius,
                                relPos.x * combinedRadius - relPos.y * leg) / distSq;
                        }
                        float dotProduct2 = Vec2.DotProduct(relVel, line.Direction);
                        u = line.Direction * dotProduct2 - relVel;
                    }
                }
                else
                {
                    // 已重叠：投影到 timeStep 的 cutoff 圆（强制推开）
                    float invTimeStep = 1f / timeStep;
                    Vec2 w = relVel - relPos * invTimeStep;
                    float wLength = w.Length;
                    if (wLength < Epsilon)
                    {
                        // 完全同速同点：用位置方向推开
                        Vec2 away = relPos.LengthSquared > Epsilon ? relPos.Normalized() : new Vec2(0f, 1f);
                        line.Direction = new Vec2(away.y, -away.x);
                        u = away * (combinedRadius * invTimeStep);
                    }
                    else
                    {
                        Vec2 unitW = w / wLength;
                        line.Direction = new Vec2(unitW.y, -unitW.x);
                        u = unitW * (combinedRadius * invTimeStep - wLength);
                    }
                }

                // reciprocal 化：半平面过 (a.Velocity + b.Velocity)/2 沿 u 方向平移一半
                line.Point = a.Velocity + u * 0.5f;
                lines.Add(line);
            }
        }

        /// <summary>
        /// 求速度圆（半径=radius）内满足第 lineNo 条线约束、且尽量靠近 optVelocity 的点。
        /// directionOpt=true 时只允许沿 optVelocity 方向（用于 LP3 的方向优化）。
        /// </summary>
        private static bool LinearProgram1(List<OrcaLine> lines, int lineNo, float radius, Vec2 optVelocity, bool directionOpt, out Vec2 result)
        {
            OrcaLine line = lines[lineNo];
            float dotProduct = Vec2.DotProduct(line.Point, line.Direction);
            float discriminant = dotProduct * dotProduct + radius * radius - line.Point.LengthSquared;

            if (discriminant < 0f)
            {
                // maxSpeed 圆完全落在该线外侧：无可行点
                result = Vec2.Zero;
                return false;
            }

            float sqrtDiscriminant = MathF.Sqrt(discriminant);
            float tLeft = -dotProduct - sqrtDiscriminant;
            float tRight = -dotProduct + sqrtDiscriminant;

            for (int i = 0; i < lineNo; i++)
            {
                float denominator = Det(lines[lineNo].Direction, lines[i].Direction);
                float numerator = Det(lines[i].Direction, lines[lineNo].Point - lines[i].Point);

                if (MathF.Abs(denominator) <= Epsilon)
                {
                    // 两线近平行：若线 i 完全在线 lineNo 左侧则不可行
                    if (numerator < 0f)
                    {
                        result = Vec2.Zero;
                        return false;
                    }
                    continue;
                }

                float t = numerator / denominator;
                if (denominator >= 0f)
                    tRight = MathF.Min(tRight, t);
                else
                    tLeft = MathF.Max(tLeft, t);

                if (tLeft > tRight)
                {
                    result = Vec2.Zero;
                    return false;
                }
            }

            if (directionOpt)
            {
                if (Vec2.DotProduct(optVelocity, line.Direction) > 0f)
                    result = line.Point + line.Direction * tRight;
                else
                    result = line.Point + line.Direction * tLeft;
            }
            else
            {
                float t = Vec2.DotProduct(line.Direction, optVelocity - line.Point);
                if (t < tLeft)
                    result = line.Point + line.Direction * tLeft;
                else if (t > tRight)
                    result = line.Point + line.Direction * tRight;
                else
                    result = line.Point + line.Direction * t;
            }
            return true;
        }

        /// <summary>
        /// 依次对每条线做 LP1。全部通过返回 lines.Count（成功）；
        /// 否则返回第一条失败的线索引。directionOpt=true 时在头部插入方向约束线。
        /// </summary>
        private static int LinearProgram2(List<OrcaLine> lines, float radius, Vec2 optVelocity, bool directionOpt, out Vec2 result)
        {
            if (directionOpt)
            {
                // 方向优化模式：额外插入一条过 optVelocity*radius、方向为 -optVelocity 的线
                OrcaLine dirLine;
                dirLine.Direction = -optVelocity;
                dirLine.Point = optVelocity * radius;
                lines.Insert(0, dirLine);
            }

            for (int i = 0; i < lines.Count; i++)
            {
                if (LinearProgram1(lines, i, radius, optVelocity, directionOpt, out result))
                    continue;

                if (directionOpt)
                {
                    // 移除方向线后重试；仍失败则返回总条数（整体失败）
                    lines.RemoveAt(0);
                    if (LinearProgram1(lines, i - 1, radius, optVelocity, true, out result))
                        return i - 1;
                    return lines.Count;
                }
                return i;
            }

            result = Vec2.Zero;
            return lines.Count;
        }

        /// <summary>
        /// LP2 失败后的兜底：从 beginLine 起逐条检查，对违反的线做方向优化投影。
        /// numObstLines=0（本求解器无静态障碍，全部是 ORCA 线）。
        /// </summary>
        private static void LinearProgram3(List<OrcaLine> lines, int numObstLines, int beginLine, float radius, ref Vec2 result)
        {
            float distance = 0f;

            for (int i = beginLine; i < lines.Count; i++)
            {
                float d = Det(lines[i].Direction, lines[i].Point - result);
                if (d > distance)
                {
                    // result 违反线 i：投影到该线
                    var projLines = new List<OrcaLine>(numObstLines + 1);
                    for (int k = 0; k < numObstLines; k++)
                        projLines.Add(lines[k]);
                    projLines.Add(lines[i]);

                    Vec2 projResult;
                    int failIndex = LinearProgram2(projLines, radius, lines[i].Point, true, out projResult);
                    if (failIndex >= projLines.Count)
                        projResult = lines[i].Point; // 最终兜底：取该线上一点

                    result = projResult;
                    distance = Det(lines[i].Direction, lines[i].Point - result);
                }
            }
        }

        /// <summary>2D 叉积（行列式）</summary>
        private static float Det(Vec2 a, Vec2 b) => a.x * b.y - a.y * b.x;
    }
}
#endif
