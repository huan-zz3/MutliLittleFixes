using System;
using RTSCamera.CommandSystem.Logic;
using RTSCamera.CommandSystem.Patch;
using RTSCamera.CommandSystem.Utilities;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order.Visual;

namespace RTSCamera.CommandSystem.Orders.VisualOrders
{
	// Token: 0x0200007C RID: 124
	public class RTSCommandVolleyFireVisualOrder : RTSCommandVisualOrder
	{
		// Token: 0x0600046D RID: 1133 RVA: 0x0001A20E File Offset: 0x0001840E
		public RTSCommandVolleyFireVisualOrder(string stringId)
			: base(stringId)
		{
			this._volleyFireName = GameTexts.FindText("str_rts_camera_command_system_volley_fire", null);
		}

		// Token: 0x0600046E RID: 1134 RVA: 0x0001A228 File Offset: 0x00018428
		public override void ExecuteOrder(OrderController orderController, VisualOrderExecutionParameters executionParameters)
		{
			bool flag = base.OnBeforeExecuteOrder(orderController, executionParameters);
			MBReadOnlyList<Formation> selectedFormations = orderController.SelectedFormations;
			OrderInQueue orderInQueue = new OrderInQueue
			{
				SelectedFormations = selectedFormations
			};
			orderInQueue.CustomOrderType = CustomOrderType.VolleyFire;
			Patch_OrderController.LivePreviewFormationChanges.SetFiringOrder(32, selectedFormations);
			Patch_OrderController.LivePreviewFormationChanges.SetVolleyMode(VolleyMode.Manual, selectedFormations);
			orderInQueue.VirtualFormationChanges = Patch_OrderController.LivePreviewFormationChanges.CollectChanges(selectedFormations);
			if (flag)
			{
				CommandQueueLogic.AddOrderToQueue(orderInQueue);
				return;
			}
			foreach (Formation formation in selectedFormations)
			{
				formation.SetFiringOrder(FiringOrder.FiringOrderFireAtWill);
				CommandQueueLogic.SetFormationVolleyMode(formation, VolleyMode.Manual);
				CommandQueueLogic.FormationVolleyFire(formation);
			}
			Utility.CallAfterSetOrder(orderController, 32);
			CommandQueueLogic.OnCustomOrderIssued(orderInQueue, orderController);
			CommandQueueLogic.TryPendingOrder(orderInQueue.SelectedFormations, orderInQueue);
		}

		// Token: 0x0600046F RID: 1135 RVA: 0x0001A2F8 File Offset: 0x000184F8
		public override TextObject GetName(OrderController orderController)
		{
			return this._volleyFireName;
		}

		// Token: 0x06000470 RID: 1136 RVA: 0x0001A300 File Offset: 0x00018500
		public override bool IsTargeted()
		{
			return false;
		}

		// Token: 0x06000471 RID: 1137 RVA: 0x0001A303 File Offset: 0x00018503
		protected override bool? OnGetFormationHasOrder(Formation formation)
		{
			return new bool?(false);
		}

		// Token: 0x06000472 RID: 1138 RVA: 0x0001A30B File Offset: 0x0001850B
		protected override string GetIconId()
		{
			return "order_toggle_fire" + "_active";
		}

		// Token: 0x040001C3 RID: 451
		private readonly TextObject _volleyFireName;
	}
}
