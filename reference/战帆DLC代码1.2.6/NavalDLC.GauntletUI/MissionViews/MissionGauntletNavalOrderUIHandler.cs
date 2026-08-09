using System;
using System.Collections.Generic;
using System.Linq;
using NavalDLC.Missions;
using NavalDLC.Missions.Objects;
using NavalDLC.Missions.Objects.UsableMachines;
using NavalDLC.View.MissionViews;
using NavalDLC.ViewModelCollection.Order;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.GauntletUI.Mission.Singleplayer;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order;

namespace NavalDLC.GauntletUI.MissionViews
{
	// Token: 0x0200001A RID: 26
	[OverrideView(typeof(NavalMissionOrderUIHandler))]
	public class MissionGauntletNavalOrderUIHandler : MissionGauntletSingleplayerOrderUIHandler
	{
		// Token: 0x060000A6 RID: 166 RVA: 0x00006F69 File Offset: 0x00005169
		public MissionGauntletNavalOrderUIHandler()
		{
			this._radialOrderMovieName = "NavalOrderRadial";
			this._barOrderMovieName = "NavalOrderBar";
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00006F88 File Offset: 0x00005188
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this._shipTargetHandler = base.Mission.GetMissionBehavior<NavalShipTargetSelectionHandler>();
			Mission mission = base.Mission;
			OrderController orderController;
			if (mission == null)
			{
				orderController = null;
			}
			else
			{
				Team playerTeam = mission.PlayerTeam;
				orderController = ((playerTeam != null) ? playerTeam.PlayerOrderController : null);
			}
			this._orderController = orderController;
			if (this._orderController != null)
			{
				this._orderController.OnSelectedFormationsChanged += this.OnSelectedFormationsChanged;
			}
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00006FEF File Offset: 0x000051EF
		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			if (this._orderController != null)
			{
				this._orderController.OnSelectedFormationsChanged -= this.OnSelectedFormationsChanged;
			}
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00007018 File Offset: 0x00005218
		protected override MissionOrderVM CreateDataSource(OrderController orderController)
		{
			NavalMissionOrderVM navalMissionOrderVM = new NavalMissionOrderVM(orderController, this.IsDeployment, false);
			navalMissionOrderVM.SetDeploymentParemeters(base.MissionScreen.CombatCamera, this.IsSiegeDeployment ? this._siegeDeploymentHandler.PlayerDeploymentPoints.ToList<DeploymentPoint>() : new List<DeploymentPoint>());
			MissionOrderCallbacks missionOrderCallbacks = default(MissionOrderCallbacks);
			missionOrderCallbacks.ToggleMissionInputs = new Action<bool>(base.ToggleScreenRotation);
			missionOrderCallbacks.RefreshVisuals = new MissionOrderCallbacks.OnRefreshVisualsDelegate(base.RefreshVisuals);
			missionOrderCallbacks.GetVisualOrderExecutionParameters = new MissionOrderCallbacks.GetOrderExecutionParametersDelegate(base.GetVisualOrderExecutionParameters);
			missionOrderCallbacks.SetSuspendTroopPlacer = new MissionOrderCallbacks.ToggleOrderPositionVisibilityDelegate(this.SetSuspendTroopPlacer);
			missionOrderCallbacks.OnActivateToggleOrder = new MissionOrderCallbacks.OnToggleActivateOrderStateDelegate(base.OnActivateToggleOrder);
			missionOrderCallbacks.OnDeactivateToggleOrder = new MissionOrderCallbacks.OnToggleActivateOrderStateDelegate(base.OnDeactivateToggleOrder);
			missionOrderCallbacks.OnTransferTroopsFinished = new MissionOrderCallbacks.OnTransferTroopsFinishedDelegate(this.OnTransferFinished);
			missionOrderCallbacks.OnBeforeOrder = new MissionOrderCallbacks.OnBeforeOrderDelegate(base.OnBeforeOrder);
			navalMissionOrderVM.SetCallbacks(missionOrderCallbacks);
			return navalMissionOrderVM;
		}

		// Token: 0x060000AA RID: 170 RVA: 0x0000710C File Offset: 0x0000530C
		protected override OrderItemVM GetChargeOrder()
		{
			string text = (NavalDLCHelpers.IsShipOrdersAvailable() ? "order_movement_advance" : "order_movement_charge");
			for (int i = 0; i < this._dataSource.OrderSets.Count; i++)
			{
				OrderSetVM orderSetVM = this._dataSource.OrderSets[i];
				for (int j = 0; j < orderSetVM.Orders.Count; j++)
				{
					OrderItemVM orderItemVM = orderSetVM.Orders[j];
					if (orderItemVM.Order.StringId == text)
					{
						return orderItemVM;
					}
				}
			}
			return null;
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00007196 File Offset: 0x00005396
		public void OnClassesSet(List<MissionOrderVM.ClassConfiguration> classData)
		{
			(this._dataSource as NavalMissionOrderVM).OnClassesSet(classData);
		}

		// Token: 0x060000AC RID: 172 RVA: 0x000071AC File Offset: 0x000053AC
		protected override void TickInput(float dt)
		{
			bool flag = true;
			if (Agent.Main != null)
			{
				AgentNavalComponent component = Agent.Main.GetComponent<AgentNavalComponent>();
				ShipControllerMachine shipControllerMachine;
				if (component == null)
				{
					shipControllerMachine = null;
				}
				else
				{
					MissionShip steppedShip = component.SteppedShip;
					shipControllerMachine = ((steppedShip != null) ? steppedShip.ShipControllerMachine : null);
				}
				ShipControllerMachine shipControllerMachine2 = shipControllerMachine;
				if (shipControllerMachine2 != null && shipControllerMachine2.PilotAgent == Agent.Main && shipControllerMachine2.CaptureTimer > 0f)
				{
					flag = false;
				}
			}
			if (flag)
			{
				base.TickInput(dt);
				return;
			}
			this._isReceivingInput = false;
			MissionOrderVM dataSource = this._dataSource;
			if (dataSource != null)
			{
				dataSource.UpdateCanUseShortcuts(false);
			}
			MissionOrderVM dataSource2 = this._dataSource;
			if (dataSource2 == null)
			{
				return;
			}
			dataSource2.TryCloseToggleOrder(false);
		}

		// Token: 0x060000AD RID: 173 RVA: 0x0000723C File Offset: 0x0000543C
		private void OnSelectedFormationsChanged()
		{
			OrderController orderController = this._orderController;
			MBReadOnlyList<Formation> mbreadOnlyList = ((orderController != null) ? orderController.SelectedFormations : null);
			if (mbreadOnlyList != null)
			{
				bool flag = mbreadOnlyList.Count == 1 && NavalDLCHelpers.IsPlayerCaptainOfFormationShip(mbreadOnlyList[0]);
				MissionFormationTargetSelectionHandler formationTargetHandler = this._formationTargetHandler;
				if (formationTargetHandler != null)
				{
					formationTargetHandler.SetIsFormationTargetingDisabled(flag);
				}
				NavalShipTargetSelectionHandler shipTargetHandler = this._shipTargetHandler;
				if (shipTargetHandler == null)
				{
					return;
				}
				shipTargetHandler.SetIsFormationTargetingDisabled(flag);
			}
		}

		// Token: 0x0400005B RID: 91
		protected NavalShipTargetSelectionHandler _shipTargetHandler;

		// Token: 0x0400005C RID: 92
		private OrderController _orderController;
	}
}
