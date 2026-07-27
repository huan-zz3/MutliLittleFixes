using System;
using NavalDLC.Missions;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using NavalDLC.Storyline;
using SandBox.GauntletUI.Tutorial;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.GauntletUI.Tutorial
{
	// Token: 0x0200000F RID: 15
	[Tutorial("ShipCutLooseTutorial")]
	public class ShipCutLooseTutorial : TutorialItemBase
	{
		// Token: 0x0600002C RID: 44 RVA: 0x000027EA File Offset: 0x000009EA
		public ShipCutLooseTutorial()
		{
			base.Placement = 1;
			base.HighlightedVisualElementID = string.Empty;
			base.MouseRequired = false;
		}

		// Token: 0x0600002D RID: 45 RVA: 0x0000280C File Offset: 0x00000A0C
		public override bool IsConditionsMetForCompletion()
		{
			Mission mission = Mission.Current;
			PirateBattleMissionController pirateBattleMissionController = ((mission != null) ? mission.GetMissionBehavior<PirateBattleMissionController>() : null);
			Mission mission2 = Mission.Current;
			NavalShipsLogic navalShipsLogic = ((mission2 != null) ? mission2.GetMissionBehavior<NavalShipsLogic>() : null);
			if (pirateBattleMissionController != null)
			{
				if (this._lastControllerHashCode != pirateBattleMissionController.GetHashCode())
				{
					this._hasCutLoose = false;
					this._lastControllerHashCode = pirateBattleMissionController.GetHashCode();
				}
				if (navalShipsLogic != null)
				{
					MBList<MissionShip> mblist = new MBList<MissionShip>();
					navalShipsLogic.FillTeamShips(0, mblist);
					if (pirateBattleMissionController.HasSelectedShip && mblist.Count == 2)
					{
						MissionShip missionShip = mblist[0];
						MissionShip missionShip2 = mblist[1];
						if (missionShip.IsDisconnectionBlocked())
						{
							missionShip.ResetDisconnectionBlock();
						}
						if (missionShip2.IsDisconnectionBlocked())
						{
							missionShip2.ResetDisconnectionBlock();
						}
						Agent main = Agent.Main;
						if (((main != null) ? main.GetComponent<AgentNavalComponent>().FormationShip : null) != null && !missionShip.GetIsThereActiveBridgeTo(missionShip2) && pirateBattleMissionController.HasSelectedShip)
						{
							this._hasCutLoose = true;
						}
					}
				}
			}
			return this._hasCutLoose;
		}

		// Token: 0x0600002E RID: 46 RVA: 0x000028EC File Offset: 0x00000AEC
		public override bool IsConditionsMetForActivation()
		{
			if (Mission.Current == null || !Mission.Current.IsNavalBattle)
			{
				return false;
			}
			PirateBattleMissionController missionBehavior = Mission.Current.GetMissionBehavior<PirateBattleMissionController>();
			return missionBehavior != null && missionBehavior.IsFirstShipCleared && missionBehavior.HasSelectedShip && !this._hasCutLoose;
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00002937 File Offset: 0x00000B37
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 8;
		}

		// Token: 0x04000009 RID: 9
		private int _lastControllerHashCode;

		// Token: 0x0400000A RID: 10
		private bool _hasCutLoose;
	}
}
