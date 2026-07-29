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
	// Token: 0x0200006D RID: 109
	public class RTSCommandActivateFacingVisualOrder : RTSCommandVisualOrder
	{
		// Token: 0x06000409 RID: 1033 RVA: 0x00018910 File Offset: 0x00016B10
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

		// Token: 0x0600040A RID: 1034 RVA: 0x00018941 File Offset: 0x00016B41
		public RTSCommandActivateFacingVisualOrder(OrderType orderType, string stringId)
			: base(stringId)
		{
			this._orderType = orderType;
		}

		// Token: 0x0600040B RID: 1035 RVA: 0x00018951 File Offset: 0x00016B51
		public override TextObject GetName(OrderController orderController)
		{
			return RTSCommandActivateFacingVisualOrder.GetName(this._orderType);
		}

		// Token: 0x0600040C RID: 1036 RVA: 0x00018960 File Offset: 0x00016B60
		public override void ExecuteOrder(OrderController orderController, VisualOrderExecutionParameters executionParameters)
		{
			bool flag = base.OnBeforeExecuteOrder(orderController, executionParameters);
			List<Formation> list = orderController.SelectedFormations.Where<Formation>((Formation f) => f.CountOfUnitsWithoutDetachedOnes > 0).ToList<Formation>();
			OrderInQueue orderInQueue = new OrderInQueue
			{
				SelectedFormations = list,
				ShouldAdjustFormationSpeed = Utility.ShouldLockFormation()
			};
			orderInQueue.OrderType = this._orderType;
			if (orderInQueue.OrderType == 15)
			{
				if ((this.IsSelectTargetForMouseClickingKeyDown || RTSCommandVisualOrder.IsFromClicking) && Patch_OrderTroopPlacer.IsFreeCamera && MissionConfigBase<CommandSystemConfig>.Get().OrderUIClickable)
				{
					RTSCommandVisualOrder.OrderToSelectTarget = SelectTargetMode.LookAtDirection;
					return;
				}
			}
			else
			{
				if (orderInQueue.OrderType != 14)
				{
					return;
				}
				if (this.IsSelectTargetForMouseClickingKeyDown && RTSCommandVisualOrder.IsFromClicking && Patch_OrderTroopPlacer.IsFreeCamera && MissionConfigBase<CommandSystemConfig>.Get().OrderUIClickable && MissionConfigBase<CommandSystemConfig>.Get().OrderUIClickableExtension)
				{
					RTSCommandVisualOrder.OrderToSelectTarget = SelectTargetMode.LookAtEnemy;
					return;
				}
			}
			if (flag)
			{
				if (orderInQueue.OrderType == 15)
				{
					Patch_OrderController.FillOrderLookingAtPosition(orderInQueue, orderController, executionParameters.WorldPosition);
				}
				else
				{
					Patch_OrderController.FillOrderLookingAtEnemy(orderInQueue, orderController, executionParameters.Formation);
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

		// Token: 0x0600040D RID: 1037 RVA: 0x00018B42 File Offset: 0x00016D42
		public override bool IsTargeted()
		{
			return false;
		}

		// Token: 0x0600040E RID: 1038 RVA: 0x00018B45 File Offset: 0x00016D45
		protected override bool? OnGetFormationHasOrder(Formation formation)
		{
			return new bool?(OrderController.GetActiveFacingOrderOf(formation) == this._orderType);
		}

		// Token: 0x0600040F RID: 1039 RVA: 0x00018B5A File Offset: 0x00016D5A
		protected override string GetIconId()
		{
			return base.GetIconId();
		}

		// Token: 0x040001AB RID: 427
		private OrderType _orderType;
	}
}
