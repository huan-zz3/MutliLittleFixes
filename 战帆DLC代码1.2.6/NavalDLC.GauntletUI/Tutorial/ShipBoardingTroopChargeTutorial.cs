using System;
using NavalDLC.Storyline;
using SandBox.GauntletUI.Tutorial;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.GauntletUI.Tutorial
{
	// Token: 0x0200000E RID: 14
	[Tutorial("ShipBoardingTroopChargeTutorial")]
	public class ShipBoardingTroopChargeTutorial : TutorialItemBase
	{
		// Token: 0x06000027 RID: 39 RVA: 0x000026BA File Offset: 0x000008BA
		public ShipBoardingTroopChargeTutorial()
		{
			base.Placement = 1;
			base.HighlightedVisualElementID = string.Empty;
			base.MouseRequired = false;
		}

		// Token: 0x06000028 RID: 40 RVA: 0x000026DC File Offset: 0x000008DC
		public override bool IsConditionsMetForCompletion()
		{
			Mission mission = Mission.Current;
			PirateBattleMissionController pirateBattleMissionController = ((mission != null) ? mission.GetMissionBehavior<PirateBattleMissionController>() : null);
			if (pirateBattleMissionController != null)
			{
				if (this._lastControllerHashCode != pirateBattleMissionController.GetHashCode())
				{
					this._hasOrderedCharge = false;
					this._registeredToOrderEvent = false;
					this._lastControllerHashCode = pirateBattleMissionController.GetHashCode();
				}
				if (!this._registeredToOrderEvent)
				{
					Mission mission2 = Mission.Current;
					bool flag;
					if (mission2 == null)
					{
						flag = null != null;
					}
					else
					{
						Team playerTeam = mission2.PlayerTeam;
						flag = ((playerTeam != null) ? playerTeam.PlayerOrderController : null) != null;
					}
					if (flag)
					{
						Mission mission3 = Mission.Current;
						if (mission3 != null && mission3.Mode == 2)
						{
							Mission.Current.PlayerTeam.PlayerOrderController.OnOrderIssued += new OnOrderIssuedDelegate(this.OnPlayerOrdered);
							this._registeredToOrderEvent = true;
						}
					}
				}
				return this._hasOrderedCharge;
			}
			return false;
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002795 File Offset: 0x00000995
		private void OnPlayerOrdered(OrderType orderType, MBReadOnlyList<Formation> appliedFormations, OrderController orderController, object[] delegateParams)
		{
			this._hasOrderedCharge = this._hasOrderedCharge || orderType == 4;
		}

		// Token: 0x0600002A RID: 42 RVA: 0x000027AC File Offset: 0x000009AC
		public override bool IsConditionsMetForActivation()
		{
			if (Mission.Current == null || !Mission.Current.IsNavalBattle)
			{
				return false;
			}
			PirateBattleMissionController missionBehavior = Mission.Current.GetMissionBehavior<PirateBattleMissionController>();
			return missionBehavior != null && !missionBehavior.IsFirstShipCleared;
		}

		// Token: 0x0600002B RID: 43 RVA: 0x000027E7 File Offset: 0x000009E7
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 8;
		}

		// Token: 0x04000006 RID: 6
		private int _lastControllerHashCode;

		// Token: 0x04000007 RID: 7
		private bool _registeredToOrderEvent;

		// Token: 0x04000008 RID: 8
		private bool _hasOrderedCharge;
	}
}
