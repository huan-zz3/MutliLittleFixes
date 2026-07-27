using System;
using NavalDLC.Missions.Objects;
using NavalDLC.Storyline;
using SandBox.GauntletUI.Tutorial;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.GauntletUI.Tutorial
{
	// Token: 0x02000008 RID: 8
	[Tutorial("ShipSailTutorial")]
	public class ShipSailTutorial : TutorialItemBase
	{
		// Token: 0x0600000F RID: 15 RVA: 0x00002201 File Offset: 0x00000401
		public ShipSailTutorial()
		{
			base.Placement = 1;
			base.HighlightedVisualElementID = "SailToggle";
			base.MouseRequired = false;
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002224 File Offset: 0x00000424
		public override bool IsConditionsMetForCompletion()
		{
			Mission mission = Mission.Current;
			NavalStorylineCaptivityMissionController navalStorylineCaptivityMissionController = ((mission != null) ? mission.GetMissionBehavior<NavalStorylineCaptivityMissionController>() : null);
			MissionShip missionShip = ((navalStorylineCaptivityMissionController != null) ? navalStorylineCaptivityMissionController.MissionShip : null);
			return missionShip != null && missionShip.IsPlayerControlled && missionShip.SailTargetSetting > 0.5f;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x0000226C File Offset: 0x0000046C
		public override bool IsConditionsMetForActivation()
		{
			Mission mission = Mission.Current;
			NavalStorylineCaptivityMissionController navalStorylineCaptivityMissionController = ((mission != null) ? mission.GetMissionBehavior<NavalStorylineCaptivityMissionController>() : null);
			return navalStorylineCaptivityMissionController != null && navalStorylineCaptivityMissionController.IsFirstHighlightCleared();
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002296 File Offset: 0x00000496
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 8;
		}
	}
}
