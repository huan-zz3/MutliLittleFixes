using System;
using NavalDLC.Storyline;
using SandBox.GauntletUI.Tutorial;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.GauntletUI.Tutorial
{
	// Token: 0x02000010 RID: 16
	[Tutorial("ShipCommandingShipsTutorial")]
	public class ShipCommandingShipsTutorial : TutorialItemBase
	{
		// Token: 0x06000030 RID: 48 RVA: 0x0000293A File Offset: 0x00000B3A
		public ShipCommandingShipsTutorial()
		{
			base.Placement = 1;
			base.HighlightedVisualElementID = string.Empty;
			base.MouseRequired = false;
		}

		// Token: 0x06000031 RID: 49 RVA: 0x0000295C File Offset: 0x00000B5C
		public override bool IsConditionsMetForCompletion()
		{
			Mission mission = Mission.Current;
			PirateBattleMissionController pirateBattleMissionController = ((mission != null) ? mission.GetMissionBehavior<PirateBattleMissionController>() : null);
			if (pirateBattleMissionController != null && pirateBattleMissionController.HasSelectedShip)
			{
				if (this._lastController != pirateBattleMissionController.GetHashCode())
				{
					this._hasOrderedCharge = false;
					this._registeredToOrderEvent = false;
					this._lastController = pirateBattleMissionController.GetHashCode();
				}
				if (pirateBattleMissionController.HasSelectedShip)
				{
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
			}
			return false;
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00002A28 File Offset: 0x00000C28
		private void OnPlayerOrdered(OrderType orderType, MBReadOnlyList<Formation> appliedFormations, OrderController orderController, object[] delegateParams)
		{
			this._hasOrderedCharge = this._hasOrderedCharge || orderType == 12;
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00002A40 File Offset: 0x00000C40
		public override bool IsConditionsMetForActivation()
		{
			if (Mission.Current == null || !Mission.Current.IsNavalBattle)
			{
				return false;
			}
			PirateBattleMissionController missionBehavior = Mission.Current.GetMissionBehavior<PirateBattleMissionController>();
			return missionBehavior != null && missionBehavior.IsFirstShipCleared && missionBehavior.HasSelectedShip;
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00002A80 File Offset: 0x00000C80
		public override TutorialContexts GetTutorialsRelevantContext()
		{
			return 8;
		}

		// Token: 0x0400000B RID: 11
		private int _lastController;

		// Token: 0x0400000C RID: 12
		private bool _registeredToOrderEvent;

		// Token: 0x0400000D RID: 13
		private bool _hasOrderedCharge;
	}
}
