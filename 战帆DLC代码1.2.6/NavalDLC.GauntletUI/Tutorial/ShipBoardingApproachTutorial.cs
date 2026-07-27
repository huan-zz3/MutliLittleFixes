using System;
using System.Linq;
using NavalDLC.Missions;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using NavalDLC.Storyline;
using SandBox.GauntletUI.Tutorial;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.GauntletUI.Tutorial
{
	// Token: 0x0200000C RID: 12
	[Tutorial("ShipBoardingApproachTutorial")]
	public class ShipBoardingApproachTutorial : TutorialItemBase
	{
		// Token: 0x0600001F RID: 31 RVA: 0x00002507 File Offset: 0x00000707
		public ShipBoardingApproachTutorial()
		{
			base.Placement = 1;
			base.HighlightedVisualElementID = string.Empty;
			base.MouseRequired = false;
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002528 File Offset: 0x00000728
		public override bool IsConditionsMetForCompletion()
		{
			Mission mission = Mission.Current;
			if (((mission != null) ? mission.GetMissionBehavior<PirateBattleMissionController>() : null) != null)
			{
				Mission mission2 = Mission.Current;
				object obj = ((mission2 != null) ? mission2.GetMissionBehavior<NavalShipsLogic>() : null);
				Agent main = Agent.Main;
				MissionShip missionShip = ((main != null) ? main.GetComponent<AgentNavalComponent>().FormationShip : null);
				object obj2 = obj;
				MissionShip missionShip2;
				if (obj2 == null)
				{
					missionShip2 = null;
				}
				else
				{
					missionShip2 = obj2.AllShips.FirstOrDefault<MissionShip>((MissionShip x) => !x.IsPlayerShip);
				}
				MissionShip missionShip3 = missionShip2;
				if (missionShip3 != null && missionShip != null && missionShip.GameEntity.GlobalPosition.DistanceSquared(missionShip3.GameEntity.GlobalPosition) <= 2500f)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000021 RID: 33 RVA: 0x000025D8 File Offset: 0x000007D8
		public override bool IsConditionsMetForActivation()
		{
			if (Mission.Current == null || !Mission.Current.IsNavalBattle)
			{
				return false;
			}
			PirateBattleMissionController missionBehavior = Mission.Current.GetMissionBehavior<PirateBattleMissionController>();
			return missionBehavior != null && !missionBehavior.IsFirstShipCleared;
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00002613 File Offset: 0x00000813
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 8;
		}
	}
}
