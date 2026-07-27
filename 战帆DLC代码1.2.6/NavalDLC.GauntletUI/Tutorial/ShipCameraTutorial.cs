using System;
using NavalDLC.Missions.Objects;
using NavalDLC.Storyline;
using NavalDLC.View.MissionViews;
using SandBox.GauntletUI.Tutorial;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.GauntletUI.Tutorial
{
	// Token: 0x0200000A RID: 10
	[Tutorial("ShipCameraTutorial")]
	public class ShipCameraTutorial : TutorialItemBase
	{
		// Token: 0x06000017 RID: 23 RVA: 0x0000236A File Offset: 0x0000056A
		public ShipCameraTutorial()
		{
			base.Placement = 1;
			base.HighlightedVisualElementID = "CameraToggle";
			base.MouseRequired = false;
		}

		// Token: 0x06000018 RID: 24 RVA: 0x0000238C File Offset: 0x0000058C
		public override bool IsConditionsMetForCompletion()
		{
			Mission mission = Mission.Current;
			MissionShipControlView missionShipControlView = ((mission != null) ? mission.GetMissionBehavior<MissionShipControlView>() : null);
			Mission mission2 = Mission.Current;
			NavalStorylineCaptivityMissionController navalStorylineCaptivityMissionController = ((mission2 != null) ? mission2.GetMissionBehavior<NavalStorylineCaptivityMissionController>() : null);
			return navalStorylineCaptivityMissionController != null && navalStorylineCaptivityMissionController.IsPlayerInShipControls() && missionShipControlView != null && missionShipControlView.ActiveCameraMode == MissionShipControlView.CameraModes.Back;
		}

		// Token: 0x06000019 RID: 25 RVA: 0x000023D8 File Offset: 0x000005D8
		public override bool IsConditionsMetForActivation()
		{
			if (Mission.Current == null || !Mission.Current.IsNavalBattle)
			{
				return false;
			}
			Mission mission = Mission.Current;
			NavalStorylineCaptivityMissionController navalStorylineCaptivityMissionController = ((mission != null) ? mission.GetMissionBehavior<NavalStorylineCaptivityMissionController>() : null);
			MissionShip missionShip = ((navalStorylineCaptivityMissionController != null) ? navalStorylineCaptivityMissionController.MissionShip : null);
			return missionShip != null && navalStorylineCaptivityMissionController.HasTalkedToGunnar && missionShip.ShipOrder.OarsmenLevel == 2;
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002436 File Offset: 0x00000636
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 8;
		}
	}
}
