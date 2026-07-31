using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.ScreenSystem;

namespace ExampleMod
{
    public class SiegeTrajectoryBehavior : MissionLogic
    {
        private RangedSiegeWeapon _currentSiegeWeapon;

        // RTS camera state
        private bool _isRtsModeEnabled;
        private Camera _customCamera;
        private float _camYawOffset;
        private float _camPitchOffset;

        // ---- 落点标记渲染器 ----

        /// <summary>Lobber（投石机/抛石机）的圆环标记</summary>
        private WorldCircleRenderer _circleRenderer;

        /// <summary>Lobber 圆心粗点 / 弩炮落点标记</summary>
        private WorldPointRenderer _pointRenderer;

        /// <summary>上一次的命中结果，用于判断武器类型变化后重建渲染器</summary>
        private bool _lastWasLobber;

        // ============================================================
        // 渲染器颜色常量
        // ============================================================

        /// <summary>圆环颜色：橙黄 (ARGB)</summary>
        private const uint Color_Ring = 0xFFFFAA00;

        /// <summary>圆心粗点颜色：亮红 (ARGB)</summary>
        private const uint Color_CenterDot = 0xFFFF3333;

        /// <summary>弩炮落点颜色：亮红 (ARGB)</summary>
        private const uint Color_BallistaDot = 0xFFFF3333;

        // ============================================================
        // 坐标投掷目标系统
        // ============================================================

        /// <summary>玩家设定的坐标投掷目标标记 — 圆环</summary>
        private WorldCircleRenderer? _coordTargetCircle;

        /// <summary>玩家设定的坐标投掷目标标记 — 圆心点</summary>
        private WorldPointRenderer? _coordTargetPoint;

        /// <summary>目标标记颜色：圆环青色，中心点红色</summary>
        private const uint Color_CoordRing = 0xFF00E5FF;
        private const uint Color_CoordDot = 0xFFFF3333;

        // ============================================================

        public override void OnAfterMissionCreated()
        {
            base.OnAfterMissionCreated();
            _currentSiegeWeapon = null;
            _isRtsModeEnabled = false;
        }

        protected override void OnEndMission()
        {
            base.OnEndMission();
            DisableRtsMode();
            DisposeRenderers();
            DisposeCoordTargetRenderers();
            CoordinateTargetManager.ClearAll();
            _currentSiegeWeapon = null;
        }

        private void DisposeRenderers()
        {
            _circleRenderer?.Dispose();
            _circleRenderer = null;
            _pointRenderer?.Dispose();
            _pointRenderer = null;
        }

        private void DisposeCoordTargetRenderers()
        {
            _coordTargetCircle?.Dispose();
            _coordTargetCircle = null;
            _coordTargetPoint?.Dispose();
            _coordTargetPoint = null;
        }

        /// <summary>
        /// 确保渲染器已初始化（每次武器变化时重建）。
        /// </summary>
        private void EnsureRenderersInitialized(MissionScreen missionScreen, bool isLobber)
        {
            if (isLobber != _lastWasLobber)
            {
                DisposeRenderers();
            }

            if (_circleRenderer == null && isLobber)
            {
                _circleRenderer = new WorldCircleRenderer(missionScreen, layerOrder: 11);
                _circleRenderer.Radius = 3f;
                _circleRenderer.Color = Color_Ring;
                _circleRenderer.Alpha = 1f;
                _circleRenderer.DotSize = 4f;
                _circleRenderer.PointCount = 128;
                _circleRenderer.Rotation = Mat3.Identity;
            }

            if (_pointRenderer == null)
            {
                _pointRenderer = new WorldPointRenderer(missionScreen, layerOrder: 12);
                if (isLobber)
                {
                    // 圆心粗点：尺寸稍大，醒目
                    _pointRenderer.Color = Color_CenterDot;
                    _pointRenderer.Size = 12f;
                }
                else
                {
                    // 弩炮落点
                    _pointRenderer.Color = Color_BallistaDot;
                    _pointRenderer.Size = 10f;
                }
                _pointRenderer.Alpha = 1f;
            }

            _lastWasLobber = isLobber;
        }

        private static bool IsLobber(RangedSiegeWeapon w)
        {
            if (w == null)
                return false;

            string typeName = w.GetType().Name.ToLower();
            string entityName = w.GameEntity.Name.ToLower();
            return typeName.Contains("mangonel") || typeName.Contains("trebuchet") || typeName.Contains("onager")
                || entityName.Contains("mangonel") || entityName.Contains("trebuchet") || entityName.Contains("onager");
        }

        // ---- RTS Camera Methods ----

        private void ToggleRtsMode()
        {
            _isRtsModeEnabled = !_isRtsModeEnabled;
            if (_isRtsModeEnabled)
            {
                InformationManager.DisplayMessage(new InformationMessage("RTS视角: 开启", Colors.Magenta));
                _camYawOffset = 0f;
                _camPitchOffset = 0f;
            }
            else
            {
                InformationManager.DisplayMessage(new InformationMessage("RTS视角: 关闭", Colors.Gray));
                ResetCustomCamera();
            }
        }

        private void DisableRtsMode()
        {
            if (_isRtsModeEnabled)
            {
                _isRtsModeEnabled = false;
                ResetCustomCamera();
            }
        }

        private void ResetCustomCamera()
        {
            MissionScreen missionScreen = ScreenManager.TopScreen as MissionScreen;
            if (missionScreen != null && _customCamera != null && missionScreen.CustomCamera == _customCamera)
            {
                missionScreen.CustomCamera = null;
            }
            _customCamera = null;
            _camYawOffset = 0f;
            _camPitchOffset = 0f;
        }

        private MatrixFrame CalculateRtsCameraFrame(RangedSiegeWeapon w)
        {
            if (w == null || !w.GameEntity.IsValid)
                return MatrixFrame.Identity;

            MatrixFrame globalFrame = w.GameEntity.GetGlobalFrame();
            Vec3 origin = globalFrame.origin;
            Vec3 f = globalFrame.rotation.f;

            // Camera behind and above the weapon
            float yawAngle = f.AsVec2.RotationInRadians + MathF.PI + _camYawOffset;
            Mat3 rot = Mat3.Identity;
            rot.RotateAboutSide(1.5707964f); // look straight down
            rot.RotateAboutForward(yawAngle);
            rot.RotateAboutSide(_camPitchOffset);

            Vec3 camPos = origin - f * 18f;
            camPos.z += 32f;

            // Clamp to above ground
            float groundZ = Mission.Current.Scene.GetGroundHeightAtPosition(camPos, (BodyFlags)544321929);
            if (camPos.z < groundZ + 2f)
                camPos.z = groundZ + 2f;

            return new MatrixFrame(rot, camPos);
        }

        // ---- Mission Flow ----

        public override void OnMissionTick(float dt)
        {
            base.OnMissionTick(dt);

            Agent main = Agent.Main;
            if (main == null || !main.IsActive())
            {
                if (_isRtsModeEnabled)
                    DisableRtsMode();
                return;
            }

            // Walk up entity tree from whatever the player is using to find a siege weapon
            WeakGameEntity entity = main.CurrentlyUsedGameObject?.GameEntity ?? main.GetSteppedEntity();
            RangedSiegeWeapon weapon = null;
            while (entity.IsValid)
            {
                weapon = entity.GetFirstScriptOfType<RangedSiegeWeapon>();
                if (weapon != null)
                    break;
                if (!entity.Parent.IsValid)
                    break;
                entity = entity.Parent;
            }

            // Track weapon changes for state management
            if (_currentSiegeWeapon != weapon)
            {
                // Weapon changed — disable RTS mode
                if (_isRtsModeEnabled)
                    DisableRtsMode();
                _currentSiegeWeapon = weapon;
            }

            if (_currentSiegeWeapon != null)
            {
                // Toggle RTS mode on middle mouse button press (mangonel/trebuchet only)
                if (Input.IsKeyPressed(InputKey.MiddleMouseButton) && IsLobber(_currentSiegeWeapon))
                {
                    ToggleRtsMode();
                }
            }
            else
            {
                // Not using a siege weapon — ensure RTS is off
                if (_isRtsModeEnabled)
                    DisableRtsMode();
            }

            // ── 玩家坐标投掷指令：按 . 设定/取消目标 ────────────────
            // 受 MCM 开关 CoordinateTargetingEnabled 控制（实时生效）
            if (Input.IsKeyPressed(InputKey.Period)
                && IsCoordinateTargetingEnabled())
            {
                HandleCoordinateTargetInput();
            }
        }

        /// <summary>
        /// 坐标标定功能是否开启（MCM 实时开关，与 CoordinateTargetAIPatch 保持一致）。
        /// 语义：仅在设置显式为 true 时开启；设置未加载（Instance 为 null）时视为关闭，
        /// 与 CoordinateTargetAIPatch 的 "!= true 即放行原版 AI" 保持一致。
        /// </summary>
        private static bool IsCoordinateTargetingEnabled()
            => MCM.Abstractions.Base.Global.GlobalSettings<SiegeTrajectoryConfig>.Instance
                ?.CoordinateTargetingEnabled == true;

        /// <summary>
        /// 处理玩家按 . 键：取消已有目标 / 设定新的投掷目标。
        /// </summary>
        private void HandleCoordinateTargetInput()
        {
            // 如果已有活跃目标 → 取消
            if (CoordinateTargetManager.IsActive)
            {
                CoordinateTargetManager.ClearAll();
                InformationManager.DisplayMessage(
                    new InformationMessage("投石机目标已取消，返回原版AI", Colors.White));
                return;
            }

            // 玩家不在战场上 → 忽略
            Agent main = Agent.Main;
            if (main == null || !main.IsActive())
                return;

            // 玩家正在使用投石机 → 忽略（应该在地面引导）
            if (_currentSiegeWeapon != null)
            {
                InformationManager.DisplayMessage(
                    new InformationMessage("请离开投石机后在战场上引导目标", Colors.Red));
                return;
            }

            // 从屏幕中心发射射线，获取目标地面坐标
            MissionScreen missionScreen = ScreenManager.TopScreen as MissionScreen;
            if (missionScreen?.CombatCamera == null)
                return;

            Vec3 rayStart, rayEnd;
            missionScreen.ScreenPointToWorldRay(new Vec2(0.5f, 0.5f), out rayStart, out rayEnd);

            // 向外延伸射线以确保能撞到地面/建筑
            Vec3 farPoint = rayStart + (rayEnd - rayStart).NormalizedCopy() * 1000f;
            float hitDistance;
            Vec3 hitPos;
            WeakGameEntity hitEntity;
            bool hasHit = Mission.Current.Scene.RayCastForClosestEntityOrTerrain(
                rayStart, farPoint, out hitDistance, out hitPos, out hitEntity,
                0.01f, BodyFlags.CommonFocusRayCastExcludeFlags);

            if (!hasHit)
            {
                InformationManager.DisplayMessage(
                    new InformationMessage("未命中地面或建筑，请对准目标位置", Colors.Red));
                return;
            }

            // 查找可用的投石机
            var availableWeapons = FindAvailableSiegeWeapons(hitPos);
            if (availableWeapons.Count == 0)
            {
                InformationManager.DisplayMessage(
                    new InformationMessage("无可用投石机（无存活、有弹药、能射到该点的投石机）", Colors.Red));
                return;
            }

            // 设定目标
            CoordinateTargetManager.SetTarget(availableWeapons, hitPos);
            InformationManager.DisplayMessage(
                new InformationMessage(
                    $"已设定投石机目标！{availableWeapons.Count} 台投石机正在瞄准该点",
                    Colors.Cyan));
        }

        /// <summary>
        /// 查找所有可用投石机：存活 + 有弹药 + 玩家方 + 非玩家操控 + Lobber + 可射到目标点。
        /// </summary>
        private List<RangedSiegeWeapon> FindAvailableSiegeWeapons(Vec3 hitPos)
        {
            List<RangedSiegeWeapon> result = new List<RangedSiegeWeapon>();

            foreach (var mo in Mission.Current.ActiveMissionObjects)
            {
                if (mo is RangedSiegeWeapon weapon
                    && !weapon.IsDestroyed
                    && weapon.AmmoCount > 0
                    && weapon.Side == (Agent.Main?.Team?.Side ?? BattleSideEnum.Attacker)
                    && IsLobber(weapon)
                    && weapon.CanShootAtPoint(hitPos))
                {
                    // 排除玩家当前正在操控的这台
                    if (_currentSiegeWeapon != null && weapon == _currentSiegeWeapon)
                        continue;

                    result.Add(weapon);
                }
            }

            return result;
        }

        public override void OnPreDisplayMissionTick(float dt)
        {
            base.OnPreDisplayMissionTick(dt);

            MissionScreen missionScreen = ScreenManager.TopScreen as MissionScreen;
            if (missionScreen == null)
            {
                HideRenderers();
                return;
            }

            if (_isRtsModeEnabled && _currentSiegeWeapon != null)
            {
                if (_customCamera == null)
                {
                    _customCamera = Camera.CreateCamera();
                    _customCamera.SetFovVertical(1.3089969f, Screen.AspectRatio, 0.1f, 2000f);
                }

                // Accumulate mouse look
                _camYawOffset -= Input.GetMouseMoveX() * 0.003f;
                _camPitchOffset -= Input.GetMouseMoveY() * 0.003f;
                _camPitchOffset = MBMath.ClampFloat(_camPitchOffset, -1.5f, 1f);

                _customCamera.Frame = CalculateRtsCameraFrame(_currentSiegeWeapon);
                missionScreen.CustomCamera = _customCamera;
            }
            else
            {
                if (_customCamera != null && missionScreen.CustomCamera == _customCamera)
                {
                    missionScreen.CustomCamera = null;
                }
            }

            // ---- 轨迹模拟 + 落点标记渲染 ----
            Agent main = Agent.Main;
            if (main != null && main.IsActive() && _currentSiegeWeapon != null)
            {
                // 模拟轨迹，获取命中信息（不执行引擎 debug 渲染）
                var hit = ProjectileTrajectorySystem.UpdateTrajectory(main, _currentSiegeWeapon);
                bool isLobber = hit.IsLobber;

                if (hit.HasHit)
                {
                    // 确保渲染器已就绪
                    EnsureRenderersInitialized(missionScreen, isLobber);

                    if (isLobber)
                    {
                        // Lobber：圆环 + 圆心粗点
                        _circleRenderer.SetWorldPosition(hit.HitPosition);
                        _circleRenderer.Rotation = CreateRotationFromNormal(hit.SurfaceNormal);
                        _circleRenderer.Tick();

                        _pointRenderer.SetWorldPosition(hit.HitPosition);
                        _pointRenderer.Tick();
                    }
                    else
                    {
                        // 弩炮：单点
                        _circleRenderer?.Hide();

                        _pointRenderer.SetWorldPosition(hit.HitPosition);
                        _pointRenderer.Tick();
                    }
                }
                else
                {
                    // 未命中 → 隐藏标记
                    HideRenderers();
                }
            }
            else
            {
                // 没有使用的攻城器械 → 隐藏标记
                HideRenderers();
            }

            // ── 玩家坐标投掷目标标记渲染 ─────────────────────────────
            // 开关关闭时不渲染目标标记（已设定的目标由 AIPatch 门控自然失效）
            if (IsCoordinateTargetingEnabled()
                && CoordinateTargetManager.IsActive
                && CoordinateTargetManager.GlobalTargetPosition.HasValue)
            {
                Vec3 targetPos = CoordinateTargetManager.GlobalTargetPosition.Value;

                // 延迟初始化渲染器（首次使用时创建）
                if (_coordTargetCircle == null)
                {
                    _coordTargetCircle = new WorldCircleRenderer(missionScreen, layerOrder: 13);
                    _coordTargetCircle.Radius = 2.5f;
                    _coordTargetCircle.Color = Color_CoordRing;
                    _coordTargetCircle.Alpha = 1f;
                    _coordTargetCircle.DotSize = 5f;
                    _coordTargetCircle.PointCount = 64;
                    _coordTargetCircle.Rotation = Mat3.Identity;
                }
                if (_coordTargetPoint == null)
                {
                    _coordTargetPoint = new WorldPointRenderer(missionScreen, layerOrder: 14);
                    _coordTargetPoint.Color = Color_CoordDot;
                    _coordTargetPoint.Alpha = 1f;
                    _coordTargetPoint.Size = 16f;
                }

                // 获取地形法线让圆环贴合地面
                Vec3 normal = SampleGroundNormal(targetPos);
                _coordTargetCircle.Rotation = CreateRotationFromNormal(normal);

                _coordTargetCircle.SetWorldPosition(targetPos);
                _coordTargetCircle.Tick();

                _coordTargetPoint.SetWorldPosition(targetPos);
                _coordTargetPoint.Tick();
            }
            else
            {
                _coordTargetCircle?.Hide();
                _coordTargetPoint?.Hide();
            }
        }

        private void HideRenderers()
        {
            _circleRenderer?.Hide();
            _pointRenderer?.Hide();
        }

        /// <summary>
        /// 根据地形的法线向量构建旋转矩阵，使圆环贴合斜坡。
        /// 返回 Mat3(side, forward, up=normal) —— 圆环的局部 XY 平面被映射到地形切平面。
        /// 与 <see cref="ProjectileTrajectorySystem"/> 中的 CreateRotationFromUp 逻辑等价。
        /// </summary>
        private static Mat3 CreateRotationFromNormal(Vec3 normal)
        {
            Vec3 reference = MathF.Abs(normal.z) < 0.99f
                ? new Vec3(0f, 0f, 1f, -1f)
                : new Vec3(1f, 0f, 0f, -1f);
            Vec3 side = Vec3.CrossProduct(reference, normal);
            side.Normalize();
            Vec3 forward = Vec3.CrossProduct(normal, side);
            forward.Normalize();
            return new Mat3(side, forward, normal);
        }

        /// <summary>
        /// 在地面坐标周围采样高度推算法线（与 TrajectorySimulation 中的逻辑等价）。
        /// </summary>
        private static Vec3 SampleGroundNormal(Vec3 pos)
        {
            float h0 = 0f, h1 = 0f, h2 = 0f;
            Scene scene = Mission.Current.Scene;
            scene.GetHeightAtPoint(pos.AsVec2, 0, ref h0);
            scene.GetHeightAtPoint(pos.AsVec2 + new Vec2(0.5f, 0f), 0, ref h1);
            scene.GetHeightAtPoint(pos.AsVec2 + new Vec2(0f, 0.5f), 0, ref h2);

            if (MathF.Abs(h0 - pos.z) > 1.5f)
                return Vec3.Up;

            Vec3 normal = Vec3.CrossProduct(
                new Vec3(0.5f, 0f, h1 - h0, -1f),
                new Vec3(0f, 0.5f, h2 - h0, -1f)).NormalizedCopy();
            return normal.IsValid ? normal : Vec3.Up;
        }
    }
}
