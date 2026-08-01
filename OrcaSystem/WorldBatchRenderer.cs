using System;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.Screens;

namespace ExampleMod
{
    /// <summary>
    /// 世界空间批量点渲染器。单个 GauntletLayer + 固定容量点池，
    /// 每帧 Reset → SetDot → EndFrame，支持逐点颜色/大小/可见性。
    /// 与 WorldCircleRenderer 的区别：一个实例可同时绘制大量世界点
    /// （如 N 个骑兵的终点彩点 + 感知半径圈采样点），避免每点一个 layer。
    ///
    /// 用法:
    ///   var b = new WorldBatchRenderer(missionScreen, capacity: 512, layerOrder: 10);
    ///   // 每帧:
    ///   b.Reset();
    ///   b.SetDot(0, new Vec3(x, y, z), color, size);
    ///   b.SetDot(1, ...);
    ///   b.EndFrame(); // 投影到屏幕并刷新 UI
    ///   // 清理:
    ///   b.Dispose();
    /// </summary>
    public class WorldBatchRenderer : IDisposable
    {
        // ============================================================
        // 内部点池条目（世界坐标 + 视觉属性，EndFrame 时投影）
        // ============================================================

        private struct DotEntry
        {
            public Vec3 WorldPosition;
            public uint Color;   // ARGB
            public float Alpha;  // 0..1
            public float Size;   // 像素
            public bool Active;  // 本帧是否启用
        }

        // ============================================================
        // 配置
        // ============================================================

        /// <summary>容量（池内最大点数）</summary>
        public int Capacity => _entries.Length;

        /// <summary>渲染器是否已释放</summary>
        public bool IsDisposed { get; private set; }

        // ============================================================
        // 内部状态
        // ============================================================

        private readonly MissionScreen _missionScreen;
        private readonly GauntletLayer _layer;
        private readonly BatchVM _viewModel;
        private DotEntry[] _entries;
        private int _writeIndex;

        // ============================================================
        // 构造 & 析构
        // ============================================================

        /// <param name="missionScreen">MissionScreen 实例</param>
        /// <param name="capacity">最大点数（骑兵数 × (感知圈采样点数 + 1)）</param>
        /// <param name="layerOrder">层序，默认 10</param>
        public WorldBatchRenderer(MissionScreen missionScreen, int capacity, int layerOrder = 10)
        {
            if (capacity < 1) capacity = 1;

            _missionScreen = missionScreen ?? throw new ArgumentNullException(nameof(missionScreen));

            _entries = new DotEntry[capacity];
            _viewModel = new BatchVM(capacity);
            _layer = new GauntletLayer("WorldBatchRenderer", layerOrder);
            _layer.LoadMovie("WorldBatchRenderer", _viewModel);

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
        // 帧生命周期
        // ============================================================

        /// <summary>帧开始：清空本帧绘制内容</summary>
        public void Reset()
        {
            _writeIndex = 0;
        }

        /// <summary>
        /// 追加一个世界点。返回该点的池索引（-1 表示池满被丢弃）。
        /// </summary>
        public int SetDot(Vec3 worldPosition, uint color, float size, float alpha = 1f)
        {
            if (IsDisposed || _writeIndex >= _entries.Length)
                return -1;

            int index = _writeIndex++;
            _entries[index] = new DotEntry
            {
                WorldPosition = worldPosition,
                Color = color,
                Alpha = MathF.Clamp(alpha, 0f, 1f),
                Size = MathF.Max(1f, size),
                Active = true,
            };
            return index;
        }

        /// <summary>本帧剩余未用点数</summary>
        public int Remaining => _entries.Length - _writeIndex;

        /// <summary>
        /// 帧结束：将所有活跃点投影到屏幕并刷新 UI。
        /// 应在 MissionView.OnMissionScreenTick 或 OnPreDisplayMissionTick 中调用。
        /// </summary>
        public void EndFrame()
        {
            if (IsDisposed) return;

            Camera camera = _missionScreen?.CombatCamera;
            if (camera == null)
            {
                _viewModel.IsVisible = false;
                return;
            }

            bool anyVisible = false;
            int n = _entries.Length;

            for (int i = 0; i < n; i++)
            {
                ref DotEntry entry = ref _entries[i];
                bool visible = false;
                float sx = -10000f, sy = -10000f, w = -1f;

                if (entry.Active)
                {
                    MBWindowManager.WorldToScreen(camera, entry.WorldPosition, ref sx, ref sy, ref w);
                    visible = w > 0 && MathF.IsValidValue(sx) && MathF.IsValidValue(sy);
                    if (visible) anyVisible = true;
                }

                _viewModel.SetDot(i, sx, sy, visible, entry.Color, entry.Alpha, entry.Size);
            }

            _viewModel.IsVisible = anyVisible;
        }

        /// <summary>隐藏全部点（保留条目，帧内仍可 SetDot）</summary>
        public void HideAll()
        {
            _viewModel.IsVisible = false;
        }

        // ============================================================
        // 内部 ViewModel
        // ============================================================

        private class BatchDotVM : ViewModel
        {
            private float _screenX;
            private float _screenY;
            private bool _isVisible;
            private string _color = "#FF4444FF"; // RRGGBBAA
            private float _size = 5f;

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
                get => _color;
                set
                {
                    if (_color != value)
                    {
                        _color = value;
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

            public void Update(float x, float y, bool visible, string color, float size)
            {
                ScreenX = x;
                ScreenY = y;
                IsVisible = visible;
                Color = color;
                Size = size;
            }
        }

        private class BatchVM : ViewModel
        {
            private bool _isVisible;
            private readonly MBBindingList<BatchDotVM> _dots;

            public int DotCount => _dots.Count;

            public BatchVM(int capacity)
            {
                _dots = new MBBindingList<BatchDotVM>();
                for (int i = 0; i < capacity; i++)
                    _dots.Add(new BatchDotVM());
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
            public MBBindingList<BatchDotVM> Dots => _dots;

            public void SetDot(int index, float sx, float sy, bool visible, uint argb, float alpha, float size)
            {
                if (index < 0 || index >= _dots.Count) return;

                uint a = (argb >> 24) & 0xFF;
                uint r = (argb >> 16) & 0xFF;
                uint g = (argb >> 8) & 0xFF;
                uint b = argb & 0xFF;
                uint blendedA = (uint)(a * MathF.Clamp(alpha, 0f, 1f));
                string hex = $"#{r:X2}{g:X2}{b:X2}{blendedA:X2}";

                _dots[index].Update(sx, sy, visible, hex, size);
            }
        }
    }
}
