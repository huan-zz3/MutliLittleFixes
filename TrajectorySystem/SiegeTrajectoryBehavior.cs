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
            _currentSiegeWeapon = null;
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
        }

        public override void OnPreDisplayMissionTick(float dt)
        {
            base.OnPreDisplayMissionTick(dt);

            MissionScreen missionScreen = ScreenManager.TopScreen as MissionScreen;
            if (missionScreen == null)
                return;

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

            // Draw trajectory after cameras are set up, so debug rendering works correctly
            Agent main = Agent.Main;
            if (main != null && main.IsActive() && _currentSiegeWeapon != null)
            {
                ProjectileTrajectorySystem.UpdateTrajectory(main, _currentSiegeWeapon);
            }
        }
    }
}
