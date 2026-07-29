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
	// Token: 0x0200006E RID: 110
	public class RTSCommandAdvanceVisualOrder : RTSCommandVisualOrder
	{
		// Token: 0x06000410 RID: 1040 RVA: 0x00018B62 File Offset: 0x00016D62
		public static TextObject GetName()
		{
			return new TextObject("{=A38xbjqm}Engage", null);
		}

		// Token: 0x06000411 RID: 1041 RVA: 0x00018B6F File Offset: 0x00016D6F
		public RTSCommandAdvanceVisualOrder(string stringId)
			: base(stringId)
		{
		}

		// Token: 0x06000412 RID: 1042 RVA: 0x00018B78 File Offset: 0x00016D78
		public override TextObject GetName(OrderController orderController)
		{
			return RTSCommandAdvanceVisualOrder.GetName();
		}

		// Token: 0x06000413 RID: 1043 RVA: 0x00018B80 File Offset: 0x00016D80
		public override void ExecuteOrder(OrderController orderController, VisualOrderExecutionParameters executionParameters)
		{
			bool flag = base.OnBeforeExecuteOrder(orderController, executionParameters);
			if (this.IsSelectTargetForMouseClickingKeyDown && RTSCommandVisualOrder.IsFromClicking && Patch_OrderTroopPlacer.IsFreeCamera && MissionConfigBase<CommandSystemConfig>.Get().OrderUIClickable && MissionConfigBase<CommandSystemConfig>.Get().OrderUIClickableExtension)
			{
				RTSCommandVisualOrder.OrderToSelectTarget = SelectTargetMode.Advance;
				return;
			}
			List<Formation> list = orderController.SelectedFormations.Where<Formation>((Formation f) => f.CountOfUnitsWithoutDetachedOnes > 0).ToList<Formation>();
			OrderInQueue orderInQueue = new OrderInQueue
			{
				SelectedFormations = list
			};
			bool disableNativeAttack = MissionConfigBase<CommandSystemConfig>.Get().DisableNativeAttack;
			orderInQueue.OrderType = 12;
			orderInQueue.TargetFormation = (disableNativeAttack ? null : executionParameters.Formation);
			Patch_OrderController.LivePreviewFormationChanges.SetMovementOrder(12, list, orderInQueue.TargetFormation, null, null);
			orderInQueue.VirtualFormationChanges = Patch_OrderController.LivePreviewFormationChanges.CollectChanges(list);
			if (flag)
			{
				CommandQueueLogic.AddOrderToQueue(orderInQueue);
				return;
			}
			CommandQueueLogic.TryPendingOrder(orderInQueue.SelectedFormations, orderInQueue);
			if (executionParameters.HasFormation && !disableNativeAttack)
			{
				orderController.SetOrderWithFormation(12, executionParameters.Formation);
				return;
			}
			orderController.SetOrder(12);
		}

		// Token: 0x06000414 RID: 1044 RVA: 0x00018C8B File Offset: 0x00016E8B
		protected override bool? OnGetFormationHasOrder(Formation formation)
		{
			return new bool?(OrderController.GetActiveMovementOrderOf(formation) == 12);
		}

		// Token: 0x06000415 RID: 1045 RVA: 0x00018C9C File Offset: 0x00016E9C
		public override bool IsTargeted()
		{
			return true;
		}
	}
}
