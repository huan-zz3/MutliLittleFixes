using System;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.Screens;

namespace ExampleMod
{
    /// <summary>
    /// 世界空间圆圈渲染器。在 3D 场景的指定坐标处绘制一个圆环，
    /// 使用 WorldToScreen + GauntletUI 方式（即 Battlefield UI 的技术路径）。
    /// 
    /// 用法:
    ///   var r = new WorldCircleRenderer(missionScreen);
    ///   r.SetWorldPosition(new Vec3(10, 5, 0));
    ///   // 每帧调用:
    ///   r.Tick();
    ///   // 清理:
    ///   r.Dispose();
    /// </summary>
    public class WorldCircleRenderer : IDisposable
    {
        // ============================================================
        // 配置属性
        // ============================================================

        /// <summary>圆心在 3D 世界的坐标</summary>
        public Vec3 WorldPosition { get; set; }

        /// <summary>
        /// 旋转矩阵。将圆环从局部 XY 平面变换到世界空间。<br/>
        /// 例如传入地形法线构建的旋转矩阵可使圆环贴合斜坡。<br/>
        /// 默认为 <see cref="Mat3.Identity"/>（圆环平行于 XZ 平面）。
        /// </summary>
        public Mat3 Rotation { get; set; } = Mat3.Identity;

        /// <summary>圆半径，游戏单位 ≈ 米（默认 1 米）</summary>
        public float Radius { get; set; } = 1f;

        /// <summary>圆环采样点数（越多越平滑，默认 24）</summary>
        public int PointCount { get; set; } = 24;

        /// <summary>
        /// 每个点的像素大小（宽=高，默认 5）<br/>
        /// 设为 3~4 得到细线效果，设为 6~8 得到粗线效果。
        /// </summary>
        public float DotSize { get; set; } = 5f;

        /// <summary>
        /// 圆圈颜色（ARGB 格式）。
        /// Alpha 通道在 [0x00, 0xFF] 范围内控制透明度——0x00 完全透明，0xFF 完全不透明。
        /// </summary>
        public uint Color
        {
            get => _color;
            set
            {
                if (_color != value)
                {
                    _color = value;
                    SyncColorToView();
                }
            }
        }

        /// <summary>
        /// 单独控制透明度（0~1f，0=完全透明, 1=完全不透明）。
        /// 此值会与 Color 中的 Alpha 通道混合。
        /// </summary>
        public float Alpha
        {
            get => _alpha;
            set
            {
                float clamped = MathF.Clamp(value, 0f, 1f);
                if (MathF.Abs(_alpha - clamped) > 0.01f)
                {
                    _alpha = clamped;
                    SyncColorToView();
                }
            }
        }

        /// <summary>整体可见性</summary>
        public bool Visible
        {
            get => _viewModel.IsVisible;
            set => _viewModel.IsVisible = value;
        }

        /// <summary>渲染器是否已释放</summary>
        public bool IsDisposed { get; private set; }

        // ============================================================
        // 内部状态
        // ============================================================

        private readonly MissionScreen _missionScreen;
        private readonly GauntletLayer _layer;
        private readonly CircleRendererVM _viewModel;
        private uint _color = 0xFFFF4444;
        private float _alpha = 1f;
        private float _currentDotSize = 5f;

        // ============================================================
        // 构造 & 析构
        // ============================================================

        /// <param name="missionScreen">MissionScreen 实例</param>
        /// <param name="layerOrder">层序，默认 11</param>
        public WorldCircleRenderer(MissionScreen missionScreen, int layerOrder = 11)
        {
            _missionScreen = missionScreen ?? throw new ArgumentNullException(nameof(missionScreen));

            _viewModel = new CircleRendererVM(24);
            _layer = new GauntletLayer("WorldCircleRenderer", layerOrder);
            _layer.LoadMovie("WorldCircleRenderer", _viewModel);

            // 在 AddLayer 之前写入初始颜色/大小，避免第一帧用默认值
            SyncColorToView();
            SyncDotSizeToView();

            _missionScreen.AddLayer(_layer);
        }

        public void Dispose()
        {
            if (IsDisposed) return;
            if (_layer != null && _missionScreen != null)
                try { _missionScreen.RemoveLayer(_layer); } catch { }
            IsDisposed = true;
            GC.SuppressFinalize(this);
        }

        // ============================================================
        // 公共方法
        // ============================================================

        /// <summary>设置圆心 3D 坐标（快捷属性设置）</summary>
        public void SetWorldPosition(Vec3 position) => WorldPosition = position;

        /// <summary>设置圆的半径</summary>
        public void SetRadius(float radius) => Radius = radius;

        /// <summary>设置颜色（各分量 0~255）</summary>
        public void SetColor(byte r, byte g, byte b, byte a = 255)
        {
            Color = ((uint)a << 24) | ((uint)r << 16) | ((uint)g << 8) | b;
        }

        /// <summary>设置每个点的像素大小</summary>
        public void SetDotSize(float pixels)
        {
            DotSize = MathF.Max(1f, pixels);
            SyncDotSizeToView();
        }

        /// <summary>显示圆圈</summary>
        public void Show() => _viewModel.IsVisible = true;

        /// <summary>隐藏圆圈</summary>
        public void Hide() => _viewModel.IsVisible = false;

        /// <summary>
        /// 每帧调用一次 —— 将 3D 世界坐标投影到屏幕并更新 UI。
        /// 应在 MissionView.OnMissionScreenTick 或 MissionLogic.OnMissionTick 中调用。
        /// </summary>
        public void Tick()
        {
            if (IsDisposed) return;
            if (_missionScreen?.CombatCamera == null)
            {
                _viewModel.IsVisible = false;
                return;
            }

            // 确保每次 Tick 前同步最新的视觉属性
            if (MathF.Abs(_currentDotSize - DotSize) > 0.01f || _currentDotSize != DotSize)
                SyncDotSizeToView();

            UpdateProjection();
        }

        // ============================================================
        // 视图同步
        // ============================================================

        /// <summary>将 Color + Alpha 混合后同步到所有点的绑定属性</summary>
        private void SyncColorToView()
        {
            // 提取 ARGB 各分量
            uint a = (_color >> 24) & 0xFF;
            uint blendedA = (uint)(a * _alpha);
            uint r = (_color >> 16) & 0xFF;
            uint g = (_color >> 8) & 0xFF;
            uint b = _color & 0xFF;

            // GauntletUI 的 Color 属性使用 #RRGGBBAA 格式（非 #AARRGGBB）
            string hex = $"#{r:X2}{g:X2}{b:X2}{blendedA:X2}";

            for (int i = 0; i < _viewModel.DotCount; i++)
                _viewModel.SetDotColor(i, hex);
        }

        /// <summary>将 DotSize 同步到所有点</summary>
        private void SyncDotSizeToView()
        {
            _currentDotSize = MathF.Max(1f, DotSize);
            for (int i = 0; i < _viewModel.DotCount; i++)
                _viewModel.SetDotSize(i, _currentDotSize);
        }

        // ============================================================
        // 核心投影逻辑
        // ============================================================

        private void UpdateProjection()
        {
            Camera camera = _missionScreen.CombatCamera;
            Vec3 center = WorldPosition;
            float r = Radius;
            int n = PointCount;
            bool anyVisible = false;

            int actualCount = MathF.Min(n, _viewModel.DotCount);
            int safeN = MathF.Max(3, actualCount);

            for (int i = 0; i < safeN; i++)
            {
                float angle = (float)i / safeN * 2f * MathF.PI;
                Vec3 offset = new Vec3(MathF.Cos(angle) * r, MathF.Sin(angle) * r, 0f);
                Vec3 worldPt = center + Rotation.TransformToParent(in offset);

                float sx = -10000f, sy = -10000f, w = -1f;
                MBWindowManager.WorldToScreen(camera, worldPt, ref sx, ref sy, ref w);

                bool visible = w > 0 && MathF.IsValidValue(sx) && MathF.IsValidValue(sy);
                if (visible) anyVisible = true;

                _viewModel.SetDot(i, sx, sy, visible);
            }

            for (int i = safeN; i < _viewModel.DotCount; i++)
                _viewModel.SetDot(i, 0f, 0f, false);

            _viewModel.IsVisible = anyVisible;
        }

        // ============================================================
        // 内部 ViewModel 类型
        // ============================================================

        /// <summary>圆周上单个点的 ViewModel</summary>
        private class CircleDotVM : ViewModel
        {
            private float _screenX;
            private float _screenY;
            private bool _isVisible;
            private string _dotColor = "#FF4444FF"; // RRGGBBAA 格式
            private float _dotSize = 5f;

            [DataSourceProperty]
            public float ScreenX
            {
                get => _screenX;
                set
                {
                    if (MathF.Abs(_screenX - value) > 0.5f)
                    {
                        _screenX = value;
                        OnPropertyChanged(nameof(ScreenX));
                    }
                }
            }

            [DataSourceProperty]
            public float ScreenY
            {
                get => _screenY;
                set
                {
                    if (MathF.Abs(_screenY - value) > 0.5f)
                    {
                        _screenY = value;
                        OnPropertyChanged(nameof(ScreenY));
                    }
                }
            }

            [DataSourceProperty]
            public bool IsVisible
            {
                get => _isVisible;
                set
                {
                    if (_isVisible != value)
                    {
                        _isVisible = value;
                        OnPropertyChanged(nameof(IsVisible));
                    }
                }
            }

            [DataSourceProperty]
            public string DotColor
            {
                get => _dotColor;
                set
                {
                    if (_dotColor != value)
                    {
                        _dotColor = value;
                        OnPropertyChanged(nameof(DotColor));
                    }
                }
            }

            [DataSourceProperty]
            public float DotSize
            {
                get => _dotSize;
                set
                {
                    if (MathF.Abs(_dotSize - value) > 0.5f)
                    {
                        _dotSize = value;
                        OnPropertyChanged(nameof(DotSize));
                    }
                }
            }

            public void Update(float x, float y, bool visible)
            {
                ScreenX = x;
                ScreenY = y;
                IsVisible = visible;
            }
        }

        /// <summary>圆圈整体的 ViewModel</summary>
        private class CircleRendererVM : ViewModel
        {
            private bool _isVisible;
            private readonly MBBindingList<CircleDotVM> _dots;

            public int DotCount => _dots.Count;

            public CircleRendererVM(int pointCount)
            {
                _dots = new MBBindingList<CircleDotVM>();
                for (int i = 0; i < pointCount; i++)
                    _dots.Add(new CircleDotVM());
            }

            [DataSourceProperty]
            public bool IsVisible
            {
                get => _isVisible;
                set
                {
                    if (_isVisible != value)
                    {
                        _isVisible = value;
                        OnPropertyChanged(nameof(IsVisible));
                    }
                }
            }

            [DataSourceProperty]
            public MBBindingList<CircleDotVM> Dots => _dots;

            public void SetDot(int index, float sx, float sy, bool visible)
            {
                if (index >= 0 && index < _dots.Count)
                    _dots[index].Update(sx, sy, visible);
            }

            public void SetDotColor(int index, string hex)
            {
                if (index >= 0 && index < _dots.Count)
                    _dots[index].DotColor = hex;
            }

            public void SetDotSize(int index, float size)
            {
                if (index >= 0 && index < _dots.Count)
                    _dots[index].DotSize = size;
            }
        }
    }
}
