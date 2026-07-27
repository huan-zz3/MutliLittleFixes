using System;
using System.Linq;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using NavalDLC.Storyline;
using SandBox.GauntletUI.Tutorial;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.GauntletUI.Tutorial
{
	// Token: 0x02000009 RID: 9
	[Tutorial("ShipOarsmanTutorial")]
	public class ShipOarsmanTutorial : TutorialItemBase
	{
		// Token: 0x06000013 RID: 19 RVA: 0x00002299 File Offset: 0x00000499
		public ShipOarsmanTutorial()
		{
			base.Placement = 1;
			base.HighlightedVisualElementID = "OarsmenToggle";
			base.MouseRequired = false;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000022BC File Offset: 0x000004BC
		public override bool IsConditionsMetForCompletion()
		{
			Mission mission = Mission.Current;
			NavalStorylineCaptivityMissionController navalStorylineCaptivityMissionController = ((mission != null) ? mission.GetMissionBehavior<NavalStorylineCaptivityMissionController>() : null);
			MissionShip missionShip = ((navalStorylineCaptivityMissionController != null) ? navalStorylineCaptivityMissionController.MissionShip : null);
			return missionShip != null && missionShip.IsPlayerControlled && missionShip.ShipOrder.OarsmenLevel == 2;
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002304 File Offset: 0x00000504
		public override bool IsConditionsMetForActivation()
		{
			if (Mission.Current == null || !Mission.Current.IsNavalBattle)
			{
				return false;
			}
			Mission mission = Mission.Current;
			bool flag = ((mission != null) ? mission.GetMissionBehavior<NavalStorylineCaptivityMissionController>() : null) != null;
			Mission mission2 = Mission.Current;
			NavalShipsLogic navalShipsLogic = ((mission2 != null) ? mission2.GetMissionBehavior<NavalShipsLogic>() : null);
			MissionShip missionShip = ((navalShipsLogic != null) ? navalShipsLogic.AllShips.FirstOrDefault<MissionShip>() : null);
			return flag && missionShip != null && missionShip.IsPlayerControlled;
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00002367 File Offset: 0x00000567
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 8;
		}
	}
}
