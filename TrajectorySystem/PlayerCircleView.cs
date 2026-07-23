using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.ScreenSystem;

namespace ExampleMod
{
    [DefaultView]
    public class PlayerCircleView : MissionView
    {
        private WorldCircleRenderer _circle;
        private WorldPointRenderer _point;
        private bool _initAttempted;

        public override void OnMissionScreenInitialize()
        {
            base.OnMissionScreenInitialize();

            if (GameNetwork.IsSessionActive)
                return;
            if (_initAttempted) return;
            _initAttempted = true;

            try
            {
                _circle = new WorldCircleRenderer(base.MissionScreen, layerOrder: 10);
                _circle.Radius = 1f;
                _circle.Color = 0xFFFF0000;     // 纯红 (ARGB: A=FF, R=FF, G=00, B=00)
                _circle.Alpha = 1f;              // 完全不透明
                _circle.DotSize = 6f;            // 加粗方便观察
                _circle.PointCount = 24;
                _circle.SetWorldPosition(Vec3.Zero);

                InformationManager.DisplayMessage(
                    new InformationMessage("[Test] 圆圈: 纯红", Colors.Green));
            }
            catch (System.Exception ex)
            {
                InformationManager.DisplayMessage(
                    new InformationMessage($"[Test] 圆圈失败: {ex.Message}", Colors.Red));
                _circle = null;
            }

            try
            {
                _point = new WorldPointRenderer(base.MissionScreen, layerOrder: 11);
                _point.Color = 0xFFFFFF00;       // 亮黄 (ARGB: A=FF, R=FF, G=FF, B=00)
                _point.Alpha = 1f;
                _point.Size = 14f;
                _point.SetWorldPosition(Vec3.Zero);

                InformationManager.DisplayMessage(
                    new InformationMessage("[Test] 点: 亮黄", Colors.Green));
            }
            catch (System.Exception ex)
            {
                InformationManager.DisplayMessage(
                    new InformationMessage($"[Test] 点失败: {ex.Message}", Colors.Red));
                _point = null;
            }
        }

        public override void OnMissionScreenTick(float dt)
        {
            base.OnMissionScreenTick(dt);

            Agent main = Agent.Main;
            if (main == null || !main.IsActive())
            {
                _circle?.Hide();
                _point?.Hide();
                return;
            }

            if (_circle != null)
            {
                _circle.SetWorldPosition(main.Position);
                _circle.Tick();
            }

            if (_point != null)
            {
                Vec3 lookDir = main.LookDirection;
                Vec3 forward2d = new Vec3(lookDir.X, lookDir.Y, 0f);
                if (forward2d.LengthSquared > 0.0001f)
                    forward2d.Normalize();
                else
                    forward2d = new Vec3(0f, 1f, 0f);

                Vec3 pointPos = main.Position + forward2d * 2f;
                _point.SetWorldPosition(pointPos);
                _point.Tick();
            }
        }

        public override void OnMissionScreenFinalize()
        {
            _point?.Dispose();
            _point = null;
            _circle?.Dispose();
            _circle = null;
            base.OnMissionScreenFinalize();
        }

        protected override void OnSuspendView()
        {
            base.OnSuspendView();
            _circle?.Hide();
            _point?.Hide();
        }

        protected override void OnResumeView() { }

        public override void OnClearScene()
        {
            base.OnClearScene();
            _circle?.Hide();
            _point?.Hide();
        }

        public override void OnPhotoModeActivated()
        {
            base.OnPhotoModeActivated();
            _circle?.Hide();
            _point?.Hide();
        }

        public override void OnPhotoModeDeactivated() { }
    }
}
