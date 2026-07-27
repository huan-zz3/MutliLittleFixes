using System;
using System.Collections.Generic;
using NavalDLC.Missions.MissionLogics;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace NavalDLC.ComponentInterfaces
{
	// Token: 0x02000159 RID: 345
	public class NavalCustomBattleInitializationModel : BattleInitializationModel
	{
		// Token: 0x06001680 RID: 5760 RVA: 0x00099CE3 File Offset: 0x00097EE3
		public override List<FormationClass> GetAllAvailableTroopTypes()
		{
			return base.BaseModel.GetAllAvailableTroopTypes();
		}

		// Token: 0x06001681 RID: 5761 RVA: 0x00099CF0 File Offset: 0x00097EF0
		protected override bool CanPlayerSideDeployWithOrderOfBattleAux()
		{
			IMissionAgentSpawnLogic missionBehavior = Mission.Current.GetMissionBehavior<IMissionAgentSpawnLogic>();
			DefaultNavalMissionAgentSpawnLogic defaultNavalMissionAgentSpawnLogic;
			if ((defaultNavalMissionAgentSpawnLogic = missionBehavior as DefaultNavalMissionAgentSpawnLogic) != null)
			{
				return defaultNavalMissionAgentSpawnLogic.DeployablePlayerShipCount > 1;
			}
			NavalRaidMissionAgentSpawnLogic navalRaidMissionAgentSpawnLogic;
			if ((navalRaidMissionAgentSpawnLogic = missionBehavior as NavalRaidMissionAgentSpawnLogic) == null)
			{
				Debug.FailedAssert("Unable to retrieve mission agent spawn logic behavior for custom mission", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\ComponentInterfaces\\NavalCustomBattleInitializationModel.cs", "CanPlayerSideDeployWithOrderOfBattleAux", 42);
				return false;
			}
			if (navalRaidMissionAgentSpawnLogic.PlayerSide == 1)
			{
				return navalRaidMissionAgentSpawnLogic.DeployablePlayerShipCount > 1;
			}
			return navalRaidMissionAgentSpawnLogic.GetNumberOfPlayerControllableTroops() >= 20;
		}
	}
}
