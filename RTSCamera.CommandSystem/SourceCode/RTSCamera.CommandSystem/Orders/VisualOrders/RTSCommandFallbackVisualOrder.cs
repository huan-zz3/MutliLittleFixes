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
	// Token: 0x02000071 RID: 113
	public class RTSCommandFallbackVisualOrder : RTSCommandVisualOrder
	{
		// Token: 0x06000424 RID: 1060 RVA: 0x000190C7 File Offset: 0x000172C7
		public static TextObject GetName()
		{
			return new TextObject("{=WhUoF9Mw}Fallback", null);
		}

		// Token: 0x06000425 RID: 1061 RVA: 0x000190D4 File Offset: 0x000172D4
		public RTSCommandFallbackVisualOrder(string stringId)
			: base(stringId)
		{
		}

		// Token: 0x06000426 RID: 1062 RVA: 0x000190DD File Offset: 0x000172DD
		public override TextObject GetName(OrderController orderController)
		{
			return new TextObject("{=WhUoF9Mw}Fallback", null);
		}

		// Token: 0x06000427 RID: 1063 RVA: 0x000190EC File Offset: 0x000172EC
		public override void ExecuteOrder(OrderController orderController, VisualOrderExecutionParameters executionParameters)
		{
			bool flag = base.OnBeforeExecuteOrder(orderController, executionParameters);
			List<Formation> list = orderController.SelectedFormations.Where<Formation>((Formation f) => f.CountOfUnitsWithoutDetachedOnes > 0).ToList<Formation>();
			OrderInQueue orderInQueue = new OrderInQueue
			{
				SelectedFormations = list
			};
			orderInQueue.OrderType = 13;
			Patch_OrderController.LivePreviewFormationChanges.SetMovementOrder(13, list, null, null, null);
			orderInQueue.VirtualFormationChanges = Patch_OrderController.LivePreviewFormationChanges.CollectChanges(list);
			if (!flag)
			{
				CommandQueueLogic.TryPendingOrder(orderInQueue.SelectedFormations, orderInQueue);
				orderController.SetOrder(13);
				return;
			}
			CommandQueueLogic.AddOrderToQueue(orderInQueue);
		}

		// Token: 0x06000428 RID: 1064 RVA: 0x00019184 File Offset: 0x00017384
		protected override bool? OnGetFormationHasOrder(Formation formation)
		{
			return new bool?(OrderController.GetActiveMovementOrderOf(formation) == 13);
		}

		// Token: 0x06000429 RID: 1065 RVA: 0x00019195 File Offset: 0x00017395
		public override bool IsTargeted()
		{
			return true;
		}
	}
}
