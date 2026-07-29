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
	// Token: 0x02000076 RID: 118
	public class RTSCommandRetreatVisualOrder : RTSCommandVisualOrder
	{
		// Token: 0x06000444 RID: 1092 RVA: 0x00019763 File Offset: 0x00017963
		public static TextObject GetName()
		{
			return new TextObject("{=VbeHEAsa}Retreat", null);
		}

		// Token: 0x06000445 RID: 1093 RVA: 0x00019770 File Offset: 0x00017970
		public RTSCommandRetreatVisualOrder(string stringId)
			: base(stringId)
		{
		}

		// Token: 0x06000446 RID: 1094 RVA: 0x00019779 File Offset: 0x00017979
		public override TextObject GetName(OrderController orderController)
		{
			return RTSCommandRetreatVisualOrder.GetName();
		}

		// Token: 0x06000447 RID: 1095 RVA: 0x00019780 File Offset: 0x00017980
		public override void ExecuteOrder(OrderController orderController, VisualOrderExecutionParameters executionParameters)
		{
			bool flag = base.OnBeforeExecuteOrder(orderController, executionParameters);
			List<Formation> list = orderController.SelectedFormations.Where<Formation>((Formation f) => f.CountOfUnitsWithoutDetachedOnes > 0).ToList<Formation>();
			OrderInQueue orderInQueue = new OrderInQueue
			{
				SelectedFormations = list
			};
			orderInQueue.OrderType = 9;
			Patch_OrderController.LivePreviewFormationChanges.SetMovementOrder(9, list, null, null, null);
			orderInQueue.VirtualFormationChanges = Patch_OrderController.LivePreviewFormationChanges.CollectChanges(list);
			if (!flag)
			{
				CommandQueueLogic.TryPendingOrder(orderInQueue.SelectedFormations, orderInQueue);
				orderController.SetOrder(9);
				return;
			}
			CommandQueueLogic.AddOrderToQueue(orderInQueue);
		}

		// Token: 0x06000448 RID: 1096 RVA: 0x00019818 File Offset: 0x00017A18
		protected override bool? OnGetFormationHasOrder(Formation formation)
		{
			return new bool?(OrderController.GetActiveMovementOrderOf(formation) == 9);
		}

		// Token: 0x06000449 RID: 1097 RVA: 0x00019829 File Offset: 0x00017A29
		public override bool IsTargeted()
		{
			return true;
		}
	}
}
