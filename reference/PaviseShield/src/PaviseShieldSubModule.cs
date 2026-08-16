using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace PaviseShield
{
    public class PaviseShieldSubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
        }

        public override void OnMissionBehaviorInitialize(Mission mission)
        {
            base.OnMissionBehaviorInitialize(mission);
            if (mission.CombatType == Mission.MissionCombatType.Combat)
            {
                mission.AddMissionBehavior(new PaviseShieldMissionBehavior());
            }
        }
    }
}
