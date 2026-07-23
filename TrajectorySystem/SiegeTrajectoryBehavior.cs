using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace ExampleMod
{
    public class SiegeTrajectoryBehavior : MissionLogic
    {
        private RangedSiegeWeapon _currentSiegeWeapon;

        public override void OnAfterMissionCreated()
        {
            base.OnAfterMissionCreated();
            _currentSiegeWeapon = null;
        }

        protected override void OnEndMission()
        {
            base.OnEndMission();
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

        public override void OnMissionTick(float dt)
        {
            base.OnMissionTick(dt);

            Agent main = Agent.Main;
            if (main == null || !main.IsActive())
                return;

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
                _currentSiegeWeapon = weapon;
            }

            if (_currentSiegeWeapon != null)
            {
                ProjectileTrajectorySystem.UpdateTrajectory(main, _currentSiegeWeapon);
            }
        }
    }
}
