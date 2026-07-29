using System;
using RTSCamera.CommandSystem.Logic;
using RTSCamera.CommandSystem.Patch;
using RTSCamera.CommandSystem.Utilities;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order.Visual;

namespace RTSCamera.CommandSystem.Orders.VisualOrders
{
	// Token: 0x0200007B RID: 123
	public class RTSCommandToggleVolleyVisualOrder : RTSCommandVisualOrder
	{
		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x06000466 RID: 1126 RVA: 0x0001A074 File Offset: 0x00018274
		public OrderState LastActiveState
		{
			get
			{
				return this._lastActiveState;
			}
		}

		// Token: 0x06000467 RID: 1127 RVA: 0x0001A07C File Offset: 0x0001827C
		public RTSCommandToggleVolleyVisualOrder(string stringId, TextObject positiveOrderName, TextObject negativeOrderName, VolleyMode volleyMode)
			: base(stringId)
		{
			this._positiveOrderName = positiveOrderName;
			this._negativeOrderName = negativeOrderName;
			this._volleyMode = volleyMode;
		}

		// Token: 0x06000468 RID: 1128 RVA: 0x0001A09C File Offset: 0x0001829C
		public override void ExecuteOrder(OrderController orderController, VisualOrderExecutionParameters executionParameters)
		{
			bool flag = base.OnBeforeExecuteOrder(orderController, executionParameters);
			MBReadOnlyList<Formation> selectedFormations = orderController.SelectedFormations;
			OrderInQueue orderInQueue = new OrderInQueue
			{
				SelectedFormations = selectedFormations
			};
			VolleyMode volleyMode = ((base.GetActiveState(orderController) == 3) ? VolleyMode.Disabled : this._volleyMode);
			orderInQueue.CustomOrderType = ((base.GetActiveState(orderController) == 3) ? CustomOrderType.DisableVolley : ((this._volleyMode == VolleyMode.Auto) ? CustomOrderType.AutoVolley : CustomOrderType.ManualVolley));
			Patch_OrderController.LivePreviewFormationChanges.SetFiringOrder(32, selectedFormations);
			Patch_OrderController.LivePreviewFormationChanges.SetVolleyMode(volleyMode, selectedFormations);
			orderInQueue.VirtualFormationChanges = Patch_OrderController.LivePreviewFormationChanges.CollectChanges(selectedFormations);
			if (flag)
			{
				CommandQueueLogic.AddOrderToQueue(orderInQueue);
				return;
			}
			foreach (Formation formation in selectedFormations)
			{
				formation.SetFiringOrder(FiringOrder.FiringOrderFireAtWill);
				CommandQueueLogic.SetFormationVolleyMode(formation, volleyMode);
			}
			Utility.CallAfterSetOrder(orderController, (volleyMode == VolleyMode.Manual) ? 31 : 32);
			CommandQueueLogic.OnCustomOrderIssued(orderInQueue, orderController);
			CommandQueueLogic.TryPendingOrder(orderInQueue.SelectedFormations, orderInQueue);
		}

		// Token: 0x06000469 RID: 1129 RVA: 0x0001A19C File Offset: 0x0001839C
		public override TextObject GetName(OrderController orderController)
		{
			OrderState activeState = base.GetActiveState(orderController);
			if (activeState - 2 <= 1)
			{
				return this._positiveOrderName;
			}
			return this._negativeOrderName;
		}

		// Token: 0x0600046A RID: 1130 RVA: 0x0001A1C4 File Offset: 0x000183C4
		public override bool IsTargeted()
		{
			return false;
		}

		// Token: 0x0600046B RID: 1131 RVA: 0x0001A1C7 File Offset: 0x000183C7
		protected override bool? OnGetFormationHasOrder(Formation formation)
		{
			return new bool?(Utility.DoesFormationHasVolleyOrder(formation, this._volleyMode));
		}

		// Token: 0x0600046C RID: 1132 RVA: 0x0001A1DC File Offset: 0x000183DC
		protected override string GetIconId()
		{
			string text = "order_toggle_fire";
			if (this._volleyMode != VolleyMode.Manual || this._lastActiveState != 3)
			{
				return text + "_active";
			}
			return text;
		}

		// Token: 0x040001C0 RID: 448
		private readonly TextObject _positiveOrderName;

		// Token: 0x040001C1 RID: 449
		private readonly TextObject _negativeOrderName;

		// Token: 0x040001C2 RID: 450
		private readonly VolleyMode _volleyMode;
	}
}
