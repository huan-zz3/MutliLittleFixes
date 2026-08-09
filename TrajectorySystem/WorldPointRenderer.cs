using System;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.Screens;

namespace MutliLittleFixes
{
    /// <summary>
    /// 世界空间单点渲染器。在 3D 场景的指定坐标处绘制一个方形标记点，
    /// 使用 WorldToScreen + GauntletUI 方式。
    /// 
    /// 用法:
    ///   var p = new WorldPointRenderer(missionScreen);
    ///   p.SetWorldPosition(new Vec3(10, 5, 0));
    ///   // 每帧调用:
    ///   p.Tick();
    ///   // 清理:
    ///   p.Dispose();
    /// </summary>
    public class WorldPointRenderer : IDisposable
    {
        // ============================================================
        // 配置属性
        // ============================================================

        /// <summary>标记点在 3D 世界的坐标</summary>
        public Vec3 WorldPosition { get; set; }

        /// <summary>点的大小（像素，宽=高，默认 8）</summary>
        public float Size
        {
            get => _size;
            set
            {
                float clamped = MathF.Max(1f, value);
                if (MathF.Abs(_size - clamped) > 0.5f)
                {
                    _size = clamped;
                    _vm.Size = clamped;
                }
            }
        }

        /// <summary>颜色（ARGB 格式，默认红色）</summary>
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

        /// <summary>透明度（0~1f，0=完全透明, 1=完全不透明）</summary>
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
            get => _vm.IsVisible;
            set => _vm.IsVisible = value;
        }

        /// <summary>是否已释放</summary>
        public bool IsDisposed { get; private set; }

        // ============================================================
        // 内部状态
        // ============================================================

        private readonly MissionScreen _missionScreen;
        private readonly GauntletLayer _layer;
        private readonly PointVM _vm;
        private uint _color = 0xFFFF4444;
        private float _size = 8f;
        private float _alpha = 1f;

        // ============================================================
        // 构造 & 析构
        // ============================================================

        /// <param name="missionScreen">MissionScreen 实例</param>
        /// <param name="layerOrder">层序，默认 12（在圆圈之上）</param>
        public WorldPointRenderer(MissionScreen missionScreen, int layerOrder = 12)
        {
            _missionScreen = missionScreen ?? throw new ArgumentNullException(nameof(missionScreen));

            _vm = new PointVM();
            _layer = new GauntletLayer("WorldPointRenderer", layerOrder);
            _layer.LoadMovie("WorldPointRenderer", _vm);

            // 在 AddLayer 之前写入初始颜色，避免第一帧用默认值
            SyncColorToView();

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

        /// <summary>设置 3D 坐标</summary>
        public void SetWorldPosition(Vec3 position) => WorldPosition = position;

        /// <summary>设置颜色（各分量 0~255）</summary>
        public void SetColor(byte r, byte g, byte b, byte a = 255)
        {
            Color = ((uint)a << 24) | ((uint)r << 16) | ((uint)g << 8) | b;
        }

        /// <summary>显示</summary>
        public void Show() => _vm.IsVisible = true;

        /// <summary>隐藏</summary>
        public void Hide() => _vm.IsVisible = false;

        /// <summary>
        /// 每帧调用一次 —— 将 3D 坐标投影到屏幕并更新 Widget 位置。
        /// </summary>
        public void Tick()
        {
            if (IsDisposed) return;
            if (_missionScreen?.CombatCamera == null)
            {
                _vm.IsVisible = false;
                return;
            }

            float sx = -10000f, sy = -10000f, w = -1f;
            MBWindowManager.WorldToScreen(
                _missionScreen.CombatCamera, WorldPosition, ref sx, ref sy, ref w);

            bool visible = w > 0 && MathF.IsValidValue(sx) && MathF.IsValidValue(sy);
            _vm.ScreenX = sx;
            _vm.ScreenY = sy;
            _vm.IsVisible = visible;
        }

        // ============================================================
        // 内部
        // ============================================================

        private void SyncColorToView()
        {
            // 提取 ARGB 各分量
            uint a = (_color >> 24) & 0xFF;
            uint blendedA = (uint)(a * _alpha);
            uint r = (_color >> 16) & 0xFF;
            uint g = (_color >> 8) & 0xFF;
            uint b = _color & 0xFF;

            // GauntletUI 的 Color 属性使用 #RRGGBBAA 格式（非 #AARRGGBB）
            _vm.Color = $"#{r:X2}{g:X2}{b:X2}{blendedA:X2}";
        }

        // ============================================================
        // ViewModel
        // ============================================================

        private class PointVM : ViewModel
        {
            private float _screenX;
            private float _screenY;
            private bool _isVisible;
            private string _colorString = "#FF4444FF"; // RRGGBBAA 格式
            private float _size = 8f;

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
            public string Color
            {
                get => _colorString;
                set
                {
                    if (_colorString != value)
                    {
                        _colorString = value;
                        OnPropertyChanged(nameof(Color));
                    }
                }
            }

            [DataSourceProperty]
            public float Size
            {
                get => _size;
                set
                {
                    if (MathF.Abs(_size - value) > 0.5f)
                    {
                        _size = value;
                        OnPropertyChanged(nameof(Size));
                    }
                }
            }
        }
    }
}
