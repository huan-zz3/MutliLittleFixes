using System;
using NavalDLC.Missions.Objects;
using NavalDLC.Storyline;
using SandBox.GauntletUI.Tutorial;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.GauntletUI.Tutorial
{
	// Token: 0x02000007 RID: 7
	[Tutorial("ShipControlTutorial")]
	public class ShipControlTutorial : TutorialItemBase
	{
		// Token: 0x0600000B RID: 11 RVA: 0x0000215D File Offset: 0x0000035D
		public ShipControlTutorial()
		{
			base.Placement = 0;
			base.HighlightedVisualElementID = string.Empty;
			base.MouseRequired = false;
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002180 File Offset: 0x00000380
		public override bool IsConditionsMetForCompletion()
		{
			Mission mission = Mission.Current;
			NavalStorylineCaptivityMissionController navalStorylineCaptivityMissionController = ((mission != null) ? mission.GetMissionBehavior<NavalStorylineCaptivityMissionController>() : null);
			if (navalStorylineCaptivityMissionController != null)
			{
				MissionShip missionShip = navalStorylineCaptivityMissionController.MissionShip;
				if (missionShip != null)
				{
					return missionShip.IsPlayerControlled;
				}
			}
			return false;
		}

		// Token: 0x0600000D RID: 13 RVA: 0x000021B4 File Offset: 0x000003B4
		public override bool IsConditionsMetForActivation()
		{
			if (Mission.Current == null || !Mission.Current.IsNavalBattle)
			{
				return false;
			}
			NavalStorylineCaptivityMissionController missionBehavior = Mission.Current.GetMissionBehavior<NavalStorylineCaptivityMissionController>();
			return missionBehavior != null && missionBehavior.HasTalkedToGunnar && Mission.Current.Mode != 1;
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000021FE File Offset: 0x000003FE
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 8;
		}
	}
}
