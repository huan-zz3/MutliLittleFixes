using System;
using System.Collections.Generic;
using System.Linq;
using RTSCamera.CommandSystem.Logic;
using RTSCamera.CommandSystem.Patch;
using RTSCamera.CommandSystem.Utilities;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order.Visual;

namespace RTSCamera.CommandSystem.Orders.VisualOrders
{
	// Token: 0x02000075 RID: 117
	public class RTSCommandMoveVisualOrder : RTSCommandVisualOrder
	{
		// Token: 0x0600043E RID: 1086 RVA: 0x000195EF File Offset: 0x000177EF
		public static TextObject GetName()
		{
			return new TextObject("{=vbAZwibd}Move to Position", null);
		}

		// Token: 0x0600043F RID: 1087 RVA: 0x000195FC File Offset: 0x000177FC
		public RTSCommandMoveVisualOrder(string iconId)
			: base(iconId)
		{
		}

		// Token: 0x06000440 RID: 1088 RVA: 0x00019605 File Offset: 0x00017805
		public override TextObject GetName(OrderController orderController)
		{
			return RTSCommandMoveVisualOrder.GetName();
		}

		// Token: 0x06000441 RID: 1089 RVA: 0x0001960C File Offset: 0x0001780C
		public override void ExecuteOrder(OrderController orderController, VisualOrderExecutionParameters executionParameters)
		{
			if (!executionParameters.HasWorldPosition || RTSCommandVisualOrder.IsFromClicking)
			{
				return;
			}
			bool flag = base.OnBeforeExecuteOrder(orderController, executionParameters);
			List<Formation> list = orderController.SelectedFormations.Where<Formation>((Formation f) => f.CountOfUnitsWithoutDetachedOnes > 0).ToList<Formation>();
			OrderInQueue orderInQueue = new OrderInQueue
			{
				SelectedFormations = list
			};
			WorldPosition worldPosition = executionParameters.WorldPosition;
			if (Mission.Current.IsFormationUnitPositionAvailable(ref worldPosition, Mission.Current.PlayerTeam))
			{
				Patch_OrderController.LivePreviewFormationChanges.SetMovementOrder(2, list, null, null, null);
				orderInQueue.OrderType = 2;
				orderInQueue.PositionBegin = worldPosition;
				orderInQueue.PositionEnd = worldPosition;
				orderInQueue.ShouldAdjustFormationSpeed = Utility.ShouldLockFormation();
				if (!flag)
				{
					Patch_OrderController.TryFadeOutForMoveOrder(orderController, list, worldPosition);
					orderController.SetOrderWithTwoPositions(2, worldPosition, worldPosition);
					orderInQueue.IsLineShort = false;
					orderInQueue.VirtualFormationChanges = Patch_OrderController.LivePreviewFormationChanges.CollectChanges(list);
					CommandQueueLogic.TryPendingOrder(orderInQueue.SelectedFormations, orderInQueue);
					return;
				}
				List<ValueTuple<Formation, int, float, WorldPosition, Vec2>> list2;
				bool flag2;
				OrderController.SimulateNewOrderWithPositionAndDirection(list, orderController.simulationFormations, worldPosition, worldPosition, ref list2, ref flag2, true);
				orderInQueue.IsLineShort = flag2;
				orderInQueue.ActualFormationChanges = list2;
				orderInQueue.VirtualFormationChanges = Patch_OrderController.LivePreviewFormationChanges.CollectChanges(list);
				CommandQueueLogic.AddOrderToQueue(orderInQueue);
			}
		}

		// Token: 0x06000442 RID: 1090 RVA: 0x00019734 File Offset: 0x00017934
		protected override bool? OnGetFormationHasOrder(Formation formation)
		{
			OrderType activeMovementOrderOf = OrderController.GetActiveMovementOrderOf(formation);
			int num = ((activeMovementOrderOf - 1 <= 1 || activeMovementOrderOf == 3) ? 1 : 0);
			return new bool?(num != 0);
		}

		// Token: 0x06000443 RID: 1091 RVA: 0x00019760 File Offset: 0x00017960
		public override bool IsTargeted()
		{
			return true;
		}
	}
}
