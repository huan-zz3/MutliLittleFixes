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
	// Token: 0x02000072 RID: 114
	public class RTSCommandFollowMeVisualOrder : RTSCommandVisualOrder
	{
		// Token: 0x0600042A RID: 1066 RVA: 0x00019198 File Offset: 0x00017398
		public static TextObject GetName()
		{
			return new TextObject("{=5LpufKs7}Follow Me", null);
		}

		// Token: 0x0600042B RID: 1067 RVA: 0x000191A5 File Offset: 0x000173A5
		public RTSCommandFollowMeVisualOrder(string stringId)
			: base(stringId)
		{
		}

		// Token: 0x0600042C RID: 1068 RVA: 0x000191AE File Offset: 0x000173AE
		public override TextObject GetName(OrderController orderController)
		{
			return RTSCommandFollowMeVisualOrder.GetName();
		}

		// Token: 0x0600042D RID: 1069 RVA: 0x000191B8 File Offset: 0x000173B8
		public override void ExecuteOrder(OrderController orderController, VisualOrderExecutionParameters executionParameters)
		{
			bool flag = base.OnBeforeExecuteOrder(orderController, executionParameters);
			List<Formation> list = orderController.SelectedFormations.Where<Formation>((Formation f) => f.CountOfUnitsWithoutDetachedOnes > 0).ToList<Formation>();
			OrderInQueue orderInQueue = new OrderInQueue
			{
				SelectedFormations = list
			};
			orderInQueue.OrderType = 7;
			orderInQueue.TargetAgent = Agent.Main;
			Patch_OrderController.LivePreviewFormationChanges.SetMovementOrder(7, list, null, Agent.Main, null);
			orderInQueue.VirtualFormationChanges = Patch_OrderController.LivePreviewFormationChanges.CollectChanges(list);
			if (!flag)
			{
				CommandQueueLogic.TryPendingOrder(orderInQueue.SelectedFormations, orderInQueue);
				orderController.SetOrderWithAgent(7, executionParameters.Agent);
				return;
			}
			CommandQueueLogic.AddOrderToQueue(orderInQueue);
		}

		// Token: 0x0600042E RID: 1070 RVA: 0x00019262 File Offset: 0x00017462
		protected override bool? OnGetFormationHasOrder(Formation formation)
		{
			return new bool?(OrderController.GetActiveMovementOrderOf(formation) == 7);
		}

		// Token: 0x0600042F RID: 1071 RVA: 0x00019272 File Offset: 0x00017472
		public override bool IsTargeted()
		{
			return true;
		}
	}
}
