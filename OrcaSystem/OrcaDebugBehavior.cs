#if false
// ══════════════════════════════════════════════════════════════════════════
// 【已禁用】commit 304cc5d 新增的 ORCA 骑兵避障功能，整文件注释（不参与编译）。
// 原始代码完整保留；如需恢复：删除本行与文件末尾的 #endif 即可。
// 恢复时还需同步：Settings.cs 中 ORCA 配置、SubModule.cs 中 OrcaDebugBehavior 注册。
// ══════════════════════════════════════════════════════════════════════════
using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.ScreenSystem;

namespace ExampleMod
{
    /// <summary>
    /// 单个骑兵参与 ORCA 求解的调试结果快照。
    /// Position = 当前世界位置（XZ），NewVelocity = ORCA 求解出的新速度（XZ），
    /// PreferredVelocity = 无障碍时的期望速度（XZ），Conflict = 冲突程度 0..1。
    /// HasNeighbor = 感知半径内是否有其他参与单位（= 求解时确实建了约束线，
    /// 无邻居的单位没有在避让任何人，UI 不绘制）。
    /// </summary>
    public struct OrcaDebugResult
    {
        public Agent Agent;
        public Vec2 Position;
        public Vec2 CurrentVelocity;
        public Vec2 PreferredVelocity;
        public Vec2 NewVelocity;
        public Vec2 Facing; // 骑兵朝向（马身方向，单位向量，绘制碰撞椭圆用）
        public float Conflict; // 0=无冲突, 1=完全被挡
        public bool HasNeighbor; // true=感知半径内有其他参与单位（有效避让）
    }

    /// <summary>
    /// ORCA 验证实现 —— 输入收集、求解与可视化绘制（MissionLogic）。
    ///
    /// OnMissionTick：
    ///   1. 收集 Agent.Main.Team 的存活骑兵（HasMount 且非 IsMount、IsActive、限制数量/距离）；
    ///   2. 计算每个骑兵的 PreferredVelocity（混合策略：移动中 = 当前速度方向×最大速度；
    ///      静止 = Formation.GetOrderPositionOfUnit 目标点方向×最大速度）；
    ///   3. 调用 OrcaSolver.ComputeNewVelocities 求解新速度。
    ///
    /// OnPreDisplayMissionTick（绘制）：
    ///   用 WorldBatchRenderer 绘制帧偏移终点彩点（绿/黄/红按冲突程度）+ 感知半径圈。
    ///   注意：必须用 ScreenManager.TopScreen as MissionScreen 获取 MissionScreen，
    ///   不能依赖 MissionView.MissionScreen（通过 AddMissionBehavior 注册的 MissionView
    ///   在 MissionScreen.RegisterView 遍历时尚未加入 MissionBehaviors，MissionScreen 为 null，
    ///   详见 MissionState.FinishMissionLoading 的时序：OnMissionAfterStarting 先于 AfterStart）。
    ///
    /// 输出只用于可视化验证，不接 native 输入。
    /// 受 MCM 实时开关 OrcaDebugEnabled 门控（关闭时 Results 清空、渲染器隐藏）。
    /// </summary>
    public class OrcaDebugBehavior : MissionLogic
    {
        // ============================================================
        // 结果缓存（View 读取）
        // ============================================================

        /// <summary>最近一次求解结果（仅玩家方骑兵，顺序与内部数组一致）</summary>
        public List<OrcaDebugResult> Results { get; } = new List<OrcaDebugResult>(64);

        /// <summary>上一次求解的帧时间步长（秒）</summary>
        public float LastTimeStep { get; private set; }

        /// <summary>当前参与求解的骑兵数量（0 = 未激活/开关关闭）</summary>
        public int ActiveAgentCount { get; private set; }

        // ============================================================
        // 内部状态
        // ============================================================

        private readonly OrcaSolver _solver = new OrcaSolver();
        private readonly List<Agent> _agents = new List<Agent>(64);
        private OrcaAgent[] _agentInputs = new OrcaAgent[64];
        private Vec2[] _newVelocities = new Vec2[64];

        // 帧间平滑（防抖动）
        private Vec2[] _smoothedVelocities = new Vec2[64];
        private Vec2[] _smoothedFacings = new Vec2[64];
        private const float VelocitySmoothing = 0.6f; // 新速度混合权重（越高越跟手）
        private const float FacingSmoothing = 0.35f;  // 朝向混合权重（越高越跟手）

        // 调试日志状态
        private bool _wasEnabled;
        private float _nextStatusLogTime;
        private float _statusLogCooldown;

        // 渲染器状态（绘制用，独立于求解）
        private WorldBatchRenderer? _renderer;
        private bool _rendererInitAttempted;

        // 绘制配置常量
        private const float DisplayTimeWindow = 0.5f;   // 帧偏移显示时间窗（秒）
        private const int CirclePointCount = 48;        // 每个圆环采样点数（感知圈/碰撞圈）
        private const uint Color_Green = 0xFF00FF00;    // 无冲突
        private const uint Color_Yellow = 0xFFFFFF00;   // 轻调
        private const uint Color_Red = 0xFFFF0000;      // 强制绕行
        private const uint Color_White = 0xFFFFFFFF;    // 当前速度参考点
        private const uint Color_SenseCircle = 0xFF00FFFF; // 感知圈（青）
        private const uint Color_CollisionCircle = 0xFF0000FF; // 碰撞半径圈（深蓝）
        private const float DotSize = 8f;
        private const float CircleDotSize = 3f;
        private const float VelocityDotSize = 4f;
        // 渲染器点池容量：按 MCM 上限（OrcaMaxAgents 500）× 每单位点数（2个圆环×48 + 速度点 + 彩点）预留
        private const int RendererCapacity = 500 * (2 + CirclePointCount * 2);

        // ============================================================
        // 过滤与配置（MCM 可调：OrcaMaxAgents / OrcaMaxRadius）
        // ============================================================

        /// <summary>参与求解的最大半径（米，超出不参与）——由 MCM OrcaMaxRadius 覆盖，此为兜底默认</summary>
        private const float DefaultMaxRadius = 60f;

        // ============================================================
        // MissionLogic
        // ============================================================

        protected override void OnEndMission()
        {
            base.OnEndMission();
            Results.Clear();
            _agents.Clear();
            ActiveAgentCount = 0;
            _renderer?.Dispose();
            _renderer = null;
            _rendererInitAttempted = false;
        }

        public override void OnMissionTick(float dt)
        {
            base.OnMissionTick(dt);

            LastTimeStep = dt;

            // MCM 实时开关 — 关闭时不干预
            if (Settings.Instance?.OrcaDebugEnabled != true)
            {
                if (_wasEnabled)
                {
                    _wasEnabled = false;
                    InformationManager.DisplayMessage(
                        new InformationMessage("[ORCA] 调试已关闭", Colors.Gray));
                }
                Results.Clear();
                _agents.Clear();
                ActiveAgentCount = 0;
                return;
            }

            if (!_wasEnabled)
            {
                _wasEnabled = true;
                _nextStatusLogTime = 0f;
                InformationManager.DisplayMessage(
                    new InformationMessage("[ORCA] 调试已启用", Colors.Cyan));
            }

            try
            {
                CollectCavalryAgents();
                int count = _agents.Count;
                if (count == 0)
                {
                    Results.Clear();
                    ActiveAgentCount = 0;
                    LogStatusIfDue(dt, "未找到玩家方骑乘单位（需骑马且在参与半径内）");
                    return;
                }

                EnsureCapacity(count);
                BuildAgentInputs(count, dt);
                _solver.ComputeNewVelocities(_agentInputs, count, dt, _newVelocities);
                SmoothVelocities(count);
                BuildResults(count);
                ActiveAgentCount = count;
                if (Settings.Instance?.OrcaApplyToNative == true)
                    ApplyToNative(count);
                LogStatusIfDue(dt, $"求解 {count} 骑乘单位");
            }
            catch (Exception ex)
            {
                // 调试功能：异常降级但必须显式报告，否则无法排查
                Results.Clear();
                ActiveAgentCount = 0;
                InformationManager.DisplayMessage(
                    new InformationMessage($"[ORCA] 求解异常: {ex.Message}", Colors.Red));
            }
        }

        /// <summary>节流：每 2 秒打印一次状态日志</summary>
        private void LogStatusIfDue(float dt, string message)
        {
            _statusLogCooldown -= dt;
            if (_statusLogCooldown > 0f)
                return;
            _statusLogCooldown = 2f;
            InformationManager.DisplayMessage(
                new InformationMessage($"[ORCA] {message}", Colors.White));

            // 同时输出队列第一个骑兵的 ORCA 输入/输出明细（诊断用）
            if (Results.Count > 0 && _agentInputs.Length > 0)
            {
                InformationManager.DisplayMessage(
                    new InformationMessage($"[ORCA]   {BuildFirstAgentDebugLine()}", Colors.White));
            }
        }

        /// <summary>
        /// 构建队列第一个骑兵的 ORCA 输入/输出诊断文本：
        /// 输入（Position/Velocity/Radius/MaxSpeed/PreferredVelocity）→ 输出（NewVelocity/Conflict）。
        /// </summary>
        private string BuildFirstAgentDebugLine()
        {
            OrcaAgent input = _agentInputs[0];
            OrcaDebugResult result = Results[0];
            return $"#0 输入[Pos({input.Position.x:F1},{input.Position.y:F1}) " +
                   $"Vel({input.Velocity.x:F2},{input.Velocity.y:F2}) " +
                   $"HL={input.HalfLength:F2} HW={input.HalfWidth:F2} " +
                   $"F({input.Facing.x:F2},{input.Facing.y:F2}) MaxSpd={input.MaxSpeed:F1} " +
                   $"Pref({input.PreferredVelocity.x:F1},{input.PreferredVelocity.y:F1})] " +
                   $"输出[New({result.NewVelocity.x:F2},{result.NewVelocity.y:F2}) " +
                   $"Conflict={result.Conflict:F2}]";
        }

        // ============================================================
        // 收集骑兵
        // ============================================================

        /// <summary>
        /// 收集玩家方（Agent.Main.Team）的骑兵/骑射手 agent。
        /// 过滤：非坐骑本体（HasMount 且 !IsMount，骑射手同样有坐骑因此包含）、IsActive、
        /// 距玩家 &lt; OrcaMaxRadius。玩家本人（Agent.Main）骑马时同样参与，不排除。
        /// 超过 OrcaMaxAgents 时按距玩家距离升序取前 N 个
        /// （先全量收集再排序，避免按 Mission.Agents 遍历顺序截断导致部分编队被整体丢弃）。
        /// </summary>
        private void CollectCavalryAgents()
        {
            _agents.Clear();

            Agent main = Agent.Main;
            if (main == null || !main.IsActive() || main.Team == null)
                return;

            float maxRadius = Settings.Instance?.OrcaMaxRadius ?? DefaultMaxRadius;
            int maxAgents = Settings.Instance?.OrcaMaxAgents ?? 80;
            Vec2 mainPos = main.Position.AsVec2;
            float maxRadiusSq = maxRadius * maxRadius;

            foreach (Agent agent in main.Mission.Agents)
            {
                if (agent == null || !agent.IsActive())
                    continue;
                if (agent.IsMount)          // 坐骑本体不算（用骑手）
                    continue;
                if (!agent.HasMount)        // 只收骑兵/骑射手（二者均有坐骑）
                    continue;
                if (agent.Team != main.Team) // 只收玩家方（敌方不算避让对象）
                    continue;
                if (agent.Position.AsVec2.DistanceSquared(mainPos) > maxRadiusSq)
                    continue;

                _agents.Add(agent);
            }

            // 始终按距玩家距离升序排序——一次排序同时服务：
            // ①数量上限截断（取前 maxAgents）②与 GetOrderPositionOfUnit 无关，仅稳定索引语义（0=最近）。
            _agents.Sort((a, b) =>
                a.Position.AsVec2.DistanceSquared(mainPos).CompareTo(
                    b.Position.AsVec2.DistanceSquared(mainPos)));

            if (_agents.Count > maxAgents)
                _agents.RemoveRange(maxAgents, _agents.Count - maxAgents);
        }

        // ============================================================
        // 构建求解输入
        // ============================================================

        private void EnsureCapacity(int count)
        {
            if (_agentInputs.Length < count)
            {
                _agentInputs = new OrcaAgent[count];
                _newVelocities = new Vec2[count];
                _smoothedVelocities = new Vec2[count];
                _smoothedFacings = new Vec2[count];
            }
        }

        private void BuildAgentInputs(int count, float dt)
        {
            float senseRadius = Settings.Instance?.OrcaSenseRadius ?? 3f;
            _solver.TimeHorizon = Settings.Instance?.OrcaTimeHorizon ?? 1.5f;
            _solver.NeighborRadius = senseRadius; // 感知半径=真实邻居截断（超出不建约束线）

            float halfLength = Settings.Instance?.OrcaHalfLength ?? 1.2f; // 马半长轴（沿朝向）
            float halfWidth = Settings.Instance?.OrcaHalfWidth ?? 0.45f;  // 马半短轴（垂直朝向）

            for (int i = 0; i < count; i++)
            {
                Agent agent = _agents[i];
                Vec2 pos = agent.Position.AsVec2;
                Vec2 vel = agent.Velocity.AsVec2;
                float maxSpeed = agent.GetMaximumForwardUnlimitedSpeed();

                Vec2 facing = ComputeFacing(agent, vel);
                facing = SmoothFacing(i, facing);

                // 椭圆碰撞体：半长轴沿朝向、半短轴垂直朝向（长宽不一适配马匹）
                Vec2 prefVel = ComputePreferredVelocity(agent, pos, vel, maxSpeed, dt);

                _agentInputs[i] = new OrcaAgent
                {
                    Position = pos,
                    Velocity = vel,
                    Facing = facing,
                    HalfLength = halfLength,
                    HalfWidth = halfWidth,
                    PreferredVelocity = prefVel,
                    MaxSpeed = maxSpeed,
                };
            }
        }

        /// <summary>
        /// 骑兵朝向（马身方向，单位向量）：
        /// 移动中（速度 &gt; 0.5 m/s）→ 速度方向（与 PreferredVelocity 混合策略一致）；
        /// 静止 → GetMovementDirection()（agent 当前朝向，native 维护）；
        /// 兜底 → Frame.rotation.f 前向转 XZ；再兜底 → Vec2.Zero（求解器退化为圆）。
        /// </summary>
        private Vec2 ComputeFacing(Agent agent, Vec2 vel)
        {
            if (vel.LengthSquared > 0.25f) // > 0.5 m/s
                return vel.Normalized();

            Vec2 dir = agent.GetMovementDirection();
            if (dir.LengthSquared > 0.0001f)
                return dir.Normalized();

            Vec3 f = agent.Frame.rotation.f;
            Vec2 frameDir = new Vec2(f.x, f.y);
            if (frameDir.LengthSquared > 0.0001f)
                return frameDir.Normalized();

            return Vec2.Zero;
        }

        /// <summary>
        /// 朝向帧间指数平滑（防静止/移动切换时跳变）。
        /// 若新旧朝向夹角 &gt; 90°（点积为负）则直接取新值快速转向，避免线性插值经过零向量。
        /// </summary>
        private Vec2 SmoothFacing(int index, Vec2 newFacing)
        {
            if (newFacing.LengthSquared <= 0.0001f)
                return _smoothedFacings[index]; // 无有效朝向：保持旧值

            Vec2 oldFacing = _smoothedFacings[index];
            if (oldFacing.LengthSquared <= 0.0001f)
            {
                _smoothedFacings[index] = newFacing;
                return newFacing;
            }

            if (Vec2.DotProduct(oldFacing, newFacing) < 0f)
            {
                _smoothedFacings[index] = newFacing; // 快速转向
                return newFacing;
            }

            Vec2 blended = oldFacing * (1f - FacingSmoothing) + newFacing * FacingSmoothing;
            if (blended.LengthSquared <= 0.0001f)
                return newFacing;
            blended.Normalize();
            _smoothedFacings[index] = blended;
            return blended;
        }

        /// <summary>
        /// PreferredVelocity 混合策略：
        /// 移动中（速度 &gt; 0.5 m/s）→ 当前速度方向 × 最大速度（保持惯性）；
        /// 静止（速度 ≤ 0.5 m/s）→ Formation.GetOrderPositionOfUnit 目标点方向 × 最大速度（走向落位点）；
        /// 目标点无效则原地（0 速度）。
        /// </summary>
        private Vec2 ComputePreferredVelocity(Agent agent, Vec2 pos, Vec2 vel, float maxSpeed, float dt)
        {
            if (maxSpeed <= 0f)
                return Vec2.Zero;

            if (vel.LengthSquared > 0.25f) // > 0.5 m/s
            {
                Vec2 dir = vel.Normalized();
                return dir * maxSpeed;
            }

            // 静止 → 朝阵型目标点
            Vec2 targetDir = GetFormationTargetDirection(agent, pos);
            if (targetDir.LengthSquared > 0.0001f)
                return targetDir * maxSpeed;

            return Vec2.Zero;
        }

        /// <summary>
        /// 取骑兵所在阵型的目标落位点方向（GetOrderPositionOfUnit）。
        /// 仅当目标有效（IsValid）时返回单位方向向量，否则 Vec2.Zero。
        /// </summary>
        private Vec2 GetFormationTargetDirection(Agent agent, Vec2 pos)
        {
            Formation formation = agent.Formation;
            if (formation == null)
                return Vec2.Zero;

            WorldPosition target = formation.GetOrderPositionOfUnit(agent);
            if (!target.IsValid)
                return Vec2.Zero;

            Vec2 delta = target.AsVec2 - pos;
            float lenSq = delta.LengthSquared;
            if (lenSq < 0.01f)
                return Vec2.Zero;

            return delta.Normalized();
        }

        // ============================================================
        // 平滑 & 结果
        // ============================================================

        /// <summary>
        /// 将 ORCA 建议速度翻译成 native 输入（仅玩家方骑兵）：
        /// 1. 限速通道：SetMaximumSpeedLimit(0..1, isMultiplier:true)，含坐骑同步。
        ///    冲突越高限速越低——ORCA 认为需要减速让开邻居时，native steering 也会慢下来。
        /// 2. 目标帧偏移通道：红点（强冲突）时把目标帧从阵型格点偏移到
        ///    Position + NewVelocity * offsetTime，TrySetFormationFrame 覆盖 native 本帧喂的帧。
        ///    注意：必须用 TrySetFormationFrame/SetFormationFrameEnabled（阵型 AI 兵的目标帧通道），
        ///    SetTargetPosition 对阵型兵会被 native formation frame 覆盖，无效。
        ///
        /// 调用时机：OnMissionTick 末尾——native 的 ParallelUpdateFormationMovement 在 Team.Tick
        /// （Mission.Tick 链更早处）已喂过本帧帧/限速，此处覆盖后 steering 下一帧才读取，安全。
        /// </summary>
        private void ApplyToNative(int count)
        {
            // MCM 可调参数
            float offsetTime = Settings.Instance?.OrcaApplyOffsetTime ?? 0.4f;
            float minSpeedMultiplier = Settings.Instance?.OrcaApplyMinSpeedMultiplier ?? 0.35f;
            float speedApplyThreshold = Settings.Instance?.OrcaApplySpeedThreshold ?? 0.35f;
            float frameApplyThreshold = Settings.Instance?.OrcaApplyFrameThreshold ?? 0.6f;

            for (int i = 0; i < count; i++)
            {
                Agent agent = _agents[i];
                if (agent == null || !agent.IsActive())
                    continue;

                OrcaDebugResult r = Results[i];
                if (!r.HasNeighbor)
                    continue; // 无邻居：未在避让任何人，不注入 native（与 UI 一致）

                float maxSpeed = MathF.Max(0.001f, _agentInputs[i].MaxSpeed);
                float newSpeed = r.NewVelocity.Length;
                float conflict = r.Conflict;

                // ---- 限速通道（不依赖编队，无 formation 的单位同样生效）----
                if (conflict >= speedApplyThreshold)
                {
                    // ORCA 认为安全的相对速度 / 全速 = 速度乘数；冲突越高越接近下限
                    float safeRatio = MathF.Clamp(newSpeed / maxSpeed, 0f, 1f);
                    float multiplier = MathF.Lerp(1f, safeRatio, MathF.Clamp((conflict - speedApplyThreshold) / (1f - speedApplyThreshold), 0f, 1f));
                    multiplier = MathF.Max(minSpeedMultiplier, multiplier);

                    agent.SetMaximumSpeedLimit(multiplier, isMultiplier: true);
                    if (agent.MountAgent != null)
                        agent.MountAgent.SetMaximumSpeedLimit(multiplier, isMultiplier: true);
                }
                else
                {
                    // 无冲突：恢复全速（native 本帧可能给了更慢的限速，如阵型追赶限制；这里只负责放开 ORCA 加的限速）
                    // 注意：直接放开会盖掉 native 自己的阵型限速（Hold/Charge 系数），所以用略保守的恢复值。
                    agent.SetMaximumSpeedLimit(1f, isMultiplier: true);
                    if (agent.MountAgent != null)
                        agent.MountAgent.SetMaximumSpeedLimit(1f, isMultiplier: true);
                }

                // ---- 目标帧偏移通道（仅强冲突，且需有编队目标帧）----
                if (conflict >= frameApplyThreshold && newSpeed > 0.5f && agent.Formation != null)
                {
                    Vec2 offsetTarget = r.Position + r.NewVelocity * offsetTime;
                    Vec3 targetVec = new Vec3(offsetTarget.x, offsetTarget.y, agent.Position.z);
                    WorldPosition wp = new WorldPosition(agent.Mission.Scene, targetVec);
                    Vec2 formationDirection = r.Agent.Formation.CurrentDirection;

                    agent.TrySetFormationFrame(in wp, in formationDirection);
                }
            }
        }

        /// <summary>
        /// 对 ORCA 输出做指数平滑，避免每帧跳变导致视觉抖动</summary>
        private void SmoothVelocities(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Vec2 v = _newVelocities[i];
                if (_smoothedVelocities[i].LengthSquared > 0.0001f)
                    v = _smoothedVelocities[i] * (1f - VelocitySmoothing) + v * VelocitySmoothing;
                _smoothedVelocities[i] = v;
                _newVelocities[i] = v;
            }
        }

        private void BuildResults(int count)
        {
            Results.Clear();

            float senseRadius = Settings.Instance?.OrcaSenseRadius ?? 3f;
            float senseRadiusSq = senseRadius * senseRadius;

            for (int i = 0; i < count; i++)
            {
                Agent agent = _agents[i];
                Vec2 pref = _agentInputs[i].PreferredVelocity;
                Vec2 curVel = _agentInputs[i].Velocity;
                Vec2 newVel = _newVelocities[i];
                float maxSpeed = MathF.Max(0.001f, _agentInputs[i].MaxSpeed);

                // 冲突程度 = 当前实际速度与 ORCA 建议速度的偏差 / 最大速度，钳制到 0..1。
                // 语义：绿=ORCA 建议保持当前行进；黄/红=ORCA 建议改变（减速/绕行/加速）。
                // 不比较 PreferredVelocity——那是"冲向理想格点的满速意图"，静止骑兵离格点
                // 哪怕 1m 也会产生恒 1.0 的假阳性（实测全红根因）。
                float deviation = (curVel - newVel).Length / maxSpeed;
                float conflict = MathF.Clamp(deviation, 0f, 1f);

                // 有效避让判定：感知半径内是否有其他参与单位（与求解器建线条件一致）。
                // 无邻居 = 该单位没有在避让任何人，不算"有效避让单位"。
                bool hasNeighbor = false;
                for (int j = 0; j < count; j++)
                {
                    if (j == i) continue;
                    if ((_agentInputs[i].Position - _agentInputs[j].Position).LengthSquared <= senseRadiusSq)
                    {
                        hasNeighbor = true;
                        break;
                    }
                }

                Results.Add(new OrcaDebugResult
                {
                    Agent = agent,
                    Position = _agentInputs[i].Position,
                    CurrentVelocity = curVel,
                    PreferredVelocity = pref,
                    NewVelocity = newVel,
                    Facing = _agentInputs[i].Facing,
                    Conflict = conflict,
                    HasNeighbor = hasNeighbor,
                });
            }
        }

        // ============================================================
        // 可视化绘制（OnPreDisplayMissionTick）
        // ============================================================

        public override void OnPreDisplayMissionTick(float dt)
        {
            base.OnPreDisplayMissionTick(dt);

            // MCM 实时开关 — 关闭时隐藏
            if (Settings.Instance?.OrcaDebugEnabled != true)
            {
                _renderer?.HideAll();
                return;
            }

            EnsureRenderer();
            if (_renderer == null)
                return;

            if (Results.Count == 0)
            {
                _renderer.HideAll();
                return;
            }

            float senseRadius = Settings.Instance?.OrcaSenseRadius ?? 3f;
            _renderer.Reset();

            foreach (OrcaDebugResult r in Results)
            {
                if (r.Agent == null || !r.Agent.IsActive())
                    continue;
                // 实时跟随：仅绘制"有效避让单位"——感知半径内有其他参与单位（正在避让）。
                // 单位失去邻居（脱离避让）的瞬间该帧即取消 UI 视图。
                if (!r.HasNeighbor)
                    continue;

                // 1a. 感知半径圈（青）——仅由「绘制感知半径圈」开关控制
                if (Settings.Instance?.OrcaShowSenseCircles != false)
                {
                    DrawCircle(r.Position, senseRadius, Color_SenseCircle, 0.6f);
                }

                // 1b. 碰撞椭圆轮廓（深蓝，以骑兵当前位置为圆心、朝向为长轴方向）
                //     ——由「ORCA避让调试视图」总开关控制（本绘制循环即在该开关门控内）
                float halfLength = Settings.Instance?.OrcaHalfLength ?? 1.2f;
                float halfWidth = Settings.Instance?.OrcaHalfWidth ?? 0.45f;
                DrawEllipse(r.Position, halfLength, halfWidth, r.Facing, Color_CollisionCircle, 0.3f);

                // 2. 当前速度方向参考点（白，小点，超前一小段）
                if (r.CurrentVelocity.LengthSquared > 0.0001f)
                {
                    Vec2 velDir = r.CurrentVelocity.Normalized();
                    _renderer.SetDot(
                        new Vec3(r.Position.x + velDir.x, r.Position.y + velDir.y, 0f),
                        Color_White, VelocityDotSize, 0.7f);
                }

                // 3. 终点彩点 = Position + NewVelocity × 显示时间窗
                Vec2 endPos = r.Position + r.NewVelocity * DisplayTimeWindow;
                uint color = ConflictToColor(r.Conflict);
                _renderer.SetDot(new Vec3(endPos.x, endPos.y, 0f), color, DotSize, 1f);
            }

            _renderer.EndFrame();
        }

        /// <summary>
        /// 懒创建渲染器。用 ScreenManager.TopScreen 获取 MissionScreen
        /// （不能依赖 MissionView.MissionScreen，见类注释的时序说明）。
        /// </summary>
        private void EnsureRenderer()
        {
            if (_renderer != null)
                return;

            MissionScreen? missionScreen = ScreenManager.TopScreen as MissionScreen;
            if (missionScreen == null)
            {
                if (!_rendererInitAttempted)
                {
                    _rendererInitAttempted = true;
                    InformationManager.DisplayMessage(
                        new InformationMessage("[ORCA] 等待 MissionScreen…", Colors.Gray));
                }
                return;
            }

            try
            {
                _renderer = new WorldBatchRenderer(missionScreen, RendererCapacity, layerOrder: 15);
                InformationManager.DisplayMessage(
                    new InformationMessage($"[ORCA] 渲染器已创建（容量 {RendererCapacity}）", Colors.Cyan));
            }
            catch (Exception ex)
            {
                InformationManager.DisplayMessage(
                    new InformationMessage($"[ORCA] 渲染器创建失败: {ex.Message}", Colors.Red));
                _rendererInitAttempted = true;
            }
        }

        /// <summary>冲突程度 → 颜色（绿/黄/红）</summary>
        private static uint ConflictToColor(float conflict)
        {
            if (conflict < 0.3f) return Color_Green;
            if (conflict < 0.7f) return Color_Yellow;
            return Color_Red;
        }

        /// <summary>在 XZ 平面画一个圆环采样点集（世界坐标 z=0）</summary>
        private void DrawCircle(Vec2 center, float radius, uint color, float alpha)
        {
            if (radius <= 0f) return;

            for (int i = 0; i < CirclePointCount; i++)
            {
                float angle = (float)i / CirclePointCount * 2f * MathF.PI;
                float x = center.x + MathF.Cos(angle) * radius;
                float y = center.y + MathF.Sin(angle) * radius;
                _renderer.SetDot(new Vec3(x, y, 0f), color, CircleDotSize, alpha);
            }
        }

        /// <summary>
        /// 在 XZ 平面画一个旋转椭圆轮廓采样点集（世界坐标 z=0）。
        /// 半长轴 halfLength 沿朝向 facing 方向、半短轴 halfWidth 垂直朝向；
        /// facing 为零向量时退化为圆（半长=半短=halfWidth）。
        /// 采样点数为感知圈的一半（椭圆只是提示性轮廓，无需过密）。
        /// </summary>
        private void DrawEllipse(Vec2 center, float halfLength, float halfWidth, Vec2 facing, uint color, float alpha)
        {
            if (halfWidth <= 0f) return;
            if (halfLength <= 0f) halfLength = halfWidth;

            float cosF = 1f, sinF = 0f;
            if (facing.LengthSquared > 0.0001f)
            {
                Vec2 f = facing.Normalized();
                cosF = f.x;
                sinF = f.y;
            }
            else
            {
                halfLength = halfWidth; // 无朝向：退化圆
            }

            int samplePoints = CirclePointCount / 2;
            for (int i = 0; i < samplePoints; i++)
            {
                float t = (float)i / samplePoints * 2f * MathF.PI;
                float localX = MathF.Cos(t) * halfLength;   // 沿朝向（长轴）
                float localY = MathF.Sin(t) * halfWidth;    // 垂直朝向（短轴）
                float x = center.x + localX * cosF - localY * sinF;
                float y = center.y + localX * sinF + localY * cosF;
                _renderer.SetDot(new Vec3(x, y, 0f), color, CircleDotSize, alpha);
            }
        }
    }
}
#endif
