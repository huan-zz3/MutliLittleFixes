using System;
using NavalDLC.Missions;
using NavalDLC.Missions.Objects;
using NavalDLC.Storyline;
using SandBox.GauntletUI.Tutorial;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.GauntletUI.Tutorial
{
	// Token: 0x0200000D RID: 13
	[Tutorial("ShipBoardingAttemptBoardingTutorial")]
	public class ShipBoardingAttemptBoardingTutorial : TutorialItemBase
	{
		// Token: 0x06000023 RID: 35 RVA: 0x00002616 File Offset: 0x00000816
		public ShipBoardingAttemptBoardingTutorial()
		{
			base.Placement = 1;
			base.HighlightedVisualElementID = string.Empty;
			base.MouseRequired = false;
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00002638 File Offset: 0x00000838
		public override bool IsConditionsMetForCompletion()
		{
			Mission mission = Mission.Current;
			if (((mission != null) ? mission.GetMissionBehavior<PirateBattleMissionController>() : null) != null)
			{
				Agent main = Agent.Main;
				MissionShip missionShip = ((main != null) ? main.GetComponent<AgentNavalComponent>().FormationShip : null);
				return missionShip != null && missionShip.GetIsAnyBridgeActive();
			}
			return false;
		}

		// Token: 0x06000025 RID: 37 RVA: 0x0000267C File Offset: 0x0000087C
		public override bool IsConditionsMetForActivation()
		{
			if (Mission.Current == null || !Mission.Current.IsNavalBattle)
			{
				return false;
			}
			PirateBattleMissionController missionBehavior = Mission.Current.GetMissionBehavior<PirateBattleMissionController>();
			return missionBehavior != null && !missionBehavior.IsFirstShipCleared;
		}

		// Token: 0x06000026 RID: 38 RVA: 0x000026B7 File Offset: 0x000008B7
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 8;
		}
	}
}
