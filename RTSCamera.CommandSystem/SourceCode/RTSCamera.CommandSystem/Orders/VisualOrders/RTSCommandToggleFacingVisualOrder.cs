using System;
using System.Collections.Generic;
using System.Linq;
using MissionSharedLibrary.Config;
using RTSCamera.CommandSystem.Config;
using RTSCamera.CommandSystem.Logic;
using RTSCamera.CommandSystem.Patch;
using RTSCamera.CommandSystem.Utilities;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order.Visual;

namespace RTSCamera.CommandSystem.Orders.VisualOrders
{
	// Token: 0x02000079 RID: 121
	public class RTSCommandToggleFacingVisualOrder : RTSCommandVisualOrder
	{
		// Token: 0x06000455 RID: 1109 RVA: 0x00019B90 File Offset: 0x00017D90
		public static TextObject GetName(OrderType orderType)
		{
			if (orderType == 14)
			{
				return new TextObject("{=qWzBa3KT}Facing Enemy", null);
			}
			if (orderType != 15)
			{
				return new TextObject("", null);
			}
			return new TextObject("{=LWVwNcRA}Facing Direction", null);
		}

		// Token: 0x06000456 RID: 1110 RVA: 0x00019BC1 File Offset: 0x00017DC1
		public RTSCommandToggleFacingVisualOrder(string stringId)
			: base(stringId)
		{
		}

		// Token: 0x06000457 RID: 1111 RVA: 0x00019BCC File Offset: 0x00017DCC
		public override TextObject GetName(OrderController orderController)
		{
			OrderState activeState = base.GetActiveState(orderController);
			if (activeState - 2 <= 1)
			{
				return RTSCommandToggleFacingVisualOrder.GetName(14);
			}
			return RTSCommandToggleFacingVisualOrder.GetName(15);
		}

		// Token: 0x06000458 RID: 1112 RVA: 0x00019BF8 File Offset: 0x00017DF8
		public override void ExecuteOrder(OrderController orderController, VisualOrderExecutionParameters executionParameters)
		{
			bool flag = base.OnBeforeExecuteOrder(orderController, executionParameters);
			List<Formation> list = orderController.SelectedFormations.Where<Formation>((Formation f) => f.CountOfUnitsWithoutDetachedOnes > 0).ToList<Formation>();
			OrderInQueue orderInQueue = new OrderInQueue
			{
				SelectedFormations = list,
				ShouldAdjustFormationSpeed = Utility.ShouldLockFormation()
			};
			orderInQueue.OrderType = (RTSCommandToggleFacingVisualOrder.IsFacingEnemy(base.GetActiveState(orderController)) ? 15 : 14);
			if (orderInQueue.OrderType == 15)
			{
				if (RTSCommandVisualOrder.IsFromClicking && Patch_OrderTroopPlacer.IsFreeCamera && MissionConfigBase<CommandSystemConfig>.Get().OrderUIClickable)
				{
					RTSCommandVisualOrder.OrderToSelectTarget = SelectTargetMode.LookAtDirection;
					return;
				}
			}
			else if (this.IsSelectTargetForMouseClickingKeyDown && RTSCommandVisualOrder.IsFromClicking && Patch_OrderTroopPlacer.IsFreeCamera && MissionConfigBase<CommandSystemConfig>.Get().OrderUIClickable && MissionConfigBase<CommandSystemConfig>.Get().OrderUIClickableExtension)
			{
				RTSCommandVisualOrder.OrderToSelectTarget = SelectTargetMode.LookAtEnemy;
				return;
			}
			if (flag)
			{
				if (orderInQueue.OrderType == 15)
				{
					Patch_OrderController.FillOrderLookingAtPosition(orderInQueue, orderController, executionParameters.WorldPosition);
				}
				else
				{
					orderInQueue.TargetFormation = executionParameters.Formation;
					Patch_OrderController.LivePreviewFormationChanges.SetFacingOrder(14, list, orderInQueue.TargetFormation);
					orderInQueue.VirtualFormationChanges = Patch_OrderController.LivePreviewFormationChanges.CollectChanges(list);
				}
				CommandQueueLogic.AddOrderToQueue(orderInQueue);
				return;
			}
			if (orderInQueue.OrderType == 15)
			{
				Patch_OrderController.SetFacingEnemyTargetFormation(list, null);
				orderInQueue.SelectedFormations = orderInQueue.SelectedFormations.Where<Formation>((Formation f) => !Utility.IsFormationOrderPositionMoving(f)).ToList<Formation>();
				orderController.SetOrderWithPosition(15, executionParameters.WorldPosition);
			}
			else
			{
				orderInQueue.TargetFormation = executionParameters.Formation;
				orderInQueue.SelectedFormations = orderInQueue.SelectedFormations.Where<Formation>((Formation f) => !Utility.IsFormationOrderPositionMoving(f)).ToList<Formation>();
				Patch_OrderController.TryFadeOutForFacingToEnemyOrder(orderController, list, orderInQueue.TargetFormation);
				Patch_OrderController.SetFacingEnemyTargetFormation(list, orderInQueue.TargetFormation);
				orderController.SetOrder(14);
			}
			orderInQueue.VirtualFormationChanges = Patch_OrderController.LivePreviewFormationChanges.CollectChanges(list);
			CommandQueueLogic.TryPendingOrder(orderInQueue.SelectedFormations, orderInQueue);
		}

		// Token: 0x06000459 RID: 1113 RVA: 0x00019DF8 File Offset: 0x00017FF8
		public override bool IsTargeted()
		{
			return false;
		}

		// Token: 0x0600045A RID: 1114 RVA: 0x00019DFB File Offset: 0x00017FFB
		protected override bool? OnGetFormationHasOrder(Formation formation)
		{
			return new bool?(OrderController.GetActiveFacingOrderOf(formation) == 14);
		}

		// Token: 0x0600045B RID: 1115 RVA: 0x00019E0C File Offset: 0x0001800C
		protected override string GetIconId()
		{
			string iconId = base.GetIconId();
			if (this._lastActiveState != 3)
			{
				return iconId;
			}
			return iconId + "_active";
		}

		// Token: 0x0600045C RID: 1116 RVA: 0x00019E36 File Offset: 0x00018036
		private static bool IsFacingEnemy(OrderState activeState)
		{
			return activeState == 3;
		}
	}
}
