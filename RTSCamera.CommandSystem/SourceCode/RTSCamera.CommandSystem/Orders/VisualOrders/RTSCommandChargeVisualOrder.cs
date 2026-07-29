using System;
using System.Collections.Generic;
using System.Linq;
using MissionSharedLibrary.Config;
using RTSCamera.CommandSystem.Config;
using RTSCamera.CommandSystem.Logic;
using RTSCamera.CommandSystem.Patch;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order.Visual;

namespace RTSCamera.CommandSystem.Orders.VisualOrders
{
	// Token: 0x02000070 RID: 112
	public class RTSCommandChargeVisualOrder : RTSCommandVisualOrder
	{
		// Token: 0x0600041E RID: 1054 RVA: 0x00018FB0 File Offset: 0x000171B0
		public static TextObject GetName()
		{
			return new TextObject("{=Dxmq32qW}Charge", null);
		}

		// Token: 0x0600041F RID: 1055 RVA: 0x00018FBD File Offset: 0x000171BD
		public RTSCommandChargeVisualOrder(string stringId)
			: base(stringId)
		{
		}

		// Token: 0x06000420 RID: 1056 RVA: 0x00018FC6 File Offset: 0x000171C6
		public override TextObject GetName(OrderController orderController)
		{
			return RTSCommandChargeVisualOrder.GetName();
		}

		// Token: 0x06000421 RID: 1057 RVA: 0x00018FD0 File Offset: 0x000171D0
		public override void ExecuteOrder(OrderController orderController, VisualOrderExecutionParameters executionParameters)
		{
			bool flag = base.OnBeforeExecuteOrder(orderController, executionParameters);
			List<Formation> list = orderController.SelectedFormations.Where<Formation>((Formation f) => f.CountOfUnitsWithoutDetachedOnes > 0).ToList<Formation>();
			OrderInQueue orderInQueue = new OrderInQueue
			{
				SelectedFormations = list
			};
			bool disableNativeAttack = MissionConfigBase<CommandSystemConfig>.Get().DisableNativeAttack;
			orderInQueue.OrderType = 4;
			orderInQueue.TargetFormation = (disableNativeAttack ? null : executionParameters.Formation);
			Patch_OrderController.LivePreviewFormationChanges.SetMovementOrder(4, list, orderInQueue.TargetFormation, null, null);
			orderInQueue.VirtualFormationChanges = Patch_OrderController.LivePreviewFormationChanges.CollectChanges(list);
			if (flag)
			{
				CommandQueueLogic.AddOrderToQueue(orderInQueue);
				return;
			}
			CommandQueueLogic.TryPendingOrder(orderInQueue.SelectedFormations, orderInQueue);
			if (executionParameters.HasFormation && !disableNativeAttack)
			{
				orderController.SetOrderWithFormation(4, executionParameters.Formation);
				return;
			}
			orderController.SetOrder(4);
		}

		// Token: 0x06000422 RID: 1058 RVA: 0x000190A0 File Offset: 0x000172A0
		protected override bool? OnGetFormationHasOrder(Formation formation)
		{
			OrderType activeMovementOrderOf = OrderController.GetActiveMovementOrderOf(formation);
			return new bool?(activeMovementOrderOf == 4 || activeMovementOrderOf == 5);
		}

		// Token: 0x06000423 RID: 1059 RVA: 0x000190C4 File Offset: 0x000172C4
		public override bool IsTargeted()
		{
			return true;
		}
	}
}
