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
	// Token: 0x02000077 RID: 119
	public class RTSCommandSingleVisualOrder : RTSCommandVisualOrder
	{
		// Token: 0x0600044A RID: 1098 RVA: 0x0001982C File Offset: 0x00017A2C
		public RTSCommandSingleVisualOrder(string stringId, TextObject name, OrderType orderType, bool useFormationTarget, bool useWorldPositionTarget)
			: base(stringId)
		{
			this._name = name;
			this._orderType = orderType;
			this._useFormationTarget = useFormationTarget;
			this._useWorldPositionTarget = useWorldPositionTarget;
		}

		// Token: 0x0600044B RID: 1099 RVA: 0x00019854 File Offset: 0x00017A54
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
			if (this._useFormationTarget || this._orderType == 14)
			{
				orderInQueue.TargetFormation = executionParameters.Formation;
			}
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
				else if (orderInQueue.OrderType == 14)
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
			}
			else
			{
				orderInQueue.TargetFormation = executionParameters.Formation;
				orderInQueue.SelectedFormations = orderInQueue.SelectedFormations.Where<Formation>((Formation f) => !Utility.IsFormationOrderPositionMoving(f)).ToList<Formation>();
				Patch_OrderController.TryFadeOutForFacingToEnemyOrder(orderController, list, orderInQueue.TargetFormation);
				Patch_OrderController.SetFacingEnemyTargetFormation(list, orderInQueue.TargetFormation);
			}
			if (executionParameters.HasFormation && this._useFormationTarget)
			{
				orderController.SetOrderWithFormation(this._orderType, executionParameters.Formation);
			}
			else if (executionParameters.HasWorldPosition && this._useWorldPositionTarget)
			{
				orderController.SetOrderWithPosition(this._orderType, executionParameters.WorldPosition);
			}
			else
			{
				orderController.SetOrder(this._orderType);
			}
			orderInQueue.VirtualFormationChanges = Patch_OrderController.LivePreviewFormationChanges.CollectChanges(list);
			CommandQueueLogic.TryPendingOrder(orderInQueue.SelectedFormations, orderInQueue);
		}

		// Token: 0x0600044C RID: 1100 RVA: 0x00019AAC File Offset: 0x00017CAC
		public override TextObject GetName(OrderController orderController)
		{
			return this._name;
		}

		// Token: 0x0600044D RID: 1101 RVA: 0x00019AB4 File Offset: 0x00017CB4
		public override bool IsTargeted()
		{
			return false;
		}

		// Token: 0x0600044E RID: 1102 RVA: 0x00019AB7 File Offset: 0x00017CB7
		protected override bool? OnGetFormationHasOrder(Formation formation)
		{
			return new bool?(Utility.DoesFormationHasOrderType(formation, this._orderType));
		}

		// Token: 0x040001B6 RID: 438
		private TextObject _name;

		// Token: 0x040001B7 RID: 439
		private OrderType _orderType;

		// Token: 0x040001B8 RID: 440
		private bool _useFormationTarget;

		// Token: 0x040001B9 RID: 441
		private bool _useWorldPositionTarget;
	}
}
