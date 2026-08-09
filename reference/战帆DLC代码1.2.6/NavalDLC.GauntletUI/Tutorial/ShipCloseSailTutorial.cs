using System;
using NavalDLC.Missions.Objects;
using NavalDLC.Storyline;
using SandBox.GauntletUI.Tutorial;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.GauntletUI.Tutorial
{
	// Token: 0x0200000B RID: 11
	[Tutorial("ShipCloseSailTutorial")]
	public class ShipCloseSailTutorial : TutorialItemBase
	{
		// Token: 0x0600001B RID: 27 RVA: 0x00002439 File Offset: 0x00000639
		public ShipCloseSailTutorial()
		{
			base.Placement = 1;
			base.HighlightedVisualElementID = "SailToggle";
			base.MouseRequired = false;
		}

		// Token: 0x0600001C RID: 28 RVA: 0x0000245C File Offset: 0x0000065C
		public override bool IsConditionsMetForCompletion()
		{
			Mission mission = Mission.Current;
			NavalStorylineCaptivityMissionController navalStorylineCaptivityMissionController = ((mission != null) ? mission.GetMissionBehavior<NavalStorylineCaptivityMissionController>() : null);
			MissionShip missionShip = ((navalStorylineCaptivityMissionController != null) ? navalStorylineCaptivityMissionController.MissionShip : null);
			return missionShip != null && (navalStorylineCaptivityMissionController.IsReadyToCloseSails() && missionShip.IsPlayerControlled && missionShip.SailTargetSetting < 0.5f) && missionShip.Physics.LinearVelocity.Length <= navalStorylineCaptivityMissionController.GetStoppedShipSpeedThreshold();
		}

		// Token: 0x0600001D RID: 29 RVA: 0x000024CC File Offset: 0x000006CC
		public override bool IsConditionsMetForActivation()
		{
			if (Mission.Current == null || !Mission.Current.IsNavalBattle)
			{
				return false;
			}
			NavalStorylineCaptivityMissionController missionBehavior = Mission.Current.GetMissionBehavior<NavalStorylineCaptivityMissionController>();
			return missionBehavior != null && missionBehavior.IsReadyToCloseSails();
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002504 File Offset: 0x00000704
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 8;
		}
	}
}
