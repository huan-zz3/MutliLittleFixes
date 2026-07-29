using System;
using System.Collections.Generic;
using System.Linq;
using RTSCamera.CommandSystem.Logic;
using RTSCamera.CommandSystem.Patch;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order.Visual;

namespace RTSCamera.CommandSystem.Orders.VisualOrders
{
	// Token: 0x02000078 RID: 120
	public class RTSCommandStopVisualOrder : RTSCommandVisualOrder
	{
		// Token: 0x0600044F RID: 1103 RVA: 0x00019ACA File Offset: 0x00017CCA
		public static TextObject GetName()
		{
			return new TextObject("{=QTr6UDAa}Stop", null);
		}

		// Token: 0x06000450 RID: 1104 RVA: 0x00019AD7 File Offset: 0x00017CD7
		public RTSCommandStopVisualOrder(string stringId)
			: base(stringId)
		{
		}

		// Token: 0x06000451 RID: 1105 RVA: 0x00019AE0 File Offset: 0x00017CE0
		public override TextObject GetName(OrderController orderController)
		{
			return RTSCommandStopVisualOrder.GetName();
		}

		// Token: 0x06000452 RID: 1106 RVA: 0x00019AE8 File Offset: 0x00017CE8
		public override void ExecuteOrder(OrderController orderController, VisualOrderExecutionParameters executionParameters)
		{
			bool flag = base.OnBeforeExecuteOrder(orderController, executionParameters);
			List<Formation> list = orderController.SelectedFormations.Where<Formation>((Formation f) => f.CountOfUnitsWithoutDetachedOnes > 0).ToList<Formation>();
			OrderInQueue orderInQueue = new OrderInQueue
			{
				SelectedFormations = list
			};
			orderInQueue.OrderType = 6;
			Patch_OrderController.LivePreviewFormationChanges.SetMovementOrder(6, list, null, null, null);
			orderInQueue.VirtualFormationChanges = Patch_OrderController.LivePreviewFormationChanges.CollectChanges(list);
			if (!flag)
			{
				CommandQueueLogic.TryPendingOrder(orderInQueue.SelectedFormations, orderInQueue);
				orderController.SetOrder(6);
				return;
			}
			CommandQueueLogic.AddOrderToQueue(orderInQueue);
		}

		// Token: 0x06000453 RID: 1107 RVA: 0x00019B7D File Offset: 0x00017D7D
		protected override bool? OnGetFormationHasOrder(Formation formation)
		{
			return new bool?(OrderController.GetActiveMovementOrderOf(formation) == 6);
		}

		// Token: 0x06000454 RID: 1108 RVA: 0x00019B8D File Offset: 0x00017D8D
		public override bool IsTargeted()
		{
			return true;
		}
	}
}
