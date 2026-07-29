using System;
using System.Collections.Generic;
using System.Linq;
using RTSCamera.CommandSystem.Config.HotKey;
using RTSCamera.CommandSystem.Logic;
using RTSCamera.CommandSystem.Patch;
using RTSCamera.CommandSystem.Utilities;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order.Visual;

namespace RTSCamera.CommandSystem.Orders
{
	// Token: 0x0200006B RID: 107
	public abstract class RTSCommandVisualOrder : VisualOrder
	{
		// Token: 0x06000401 RID: 1025 RVA: 0x000181BA File Offset: 0x000163BA
		protected RTSCommandVisualOrder(string stringId)
			: base(stringId)
		{
		}

		// Token: 0x06000402 RID: 1026 RVA: 0x000181C4 File Offset: 0x000163C4
		protected bool OnBeforeExecuteOrder(OrderController orderController, VisualOrderExecutionParameters executionParameters)
		{
			List<Formation> list = orderController.SelectedFormations.Where<Formation>((Formation f) => f.CountOfUnitsWithoutDetachedOnes > 0).ToList<Formation>();
			this.QueueCommand = Utility.ShouldQueueCommand();
			if (!this.QueueCommand)
			{
				Patch_OrderController.LivePreviewFormationChanges.SetChanges(CommandQueueLogic.CurrentFormationChanges.CollectChanges(list));
			}
			else
			{
				Patch_OrderController.LivePreviewFormationChanges.SetChanges(CommandQueueLogic.LatestOrderInQueueChanges.CollectChanges(list));
			}
			this.IsSelectTargetForMouseClickingKeyDown = CommandSystemGameKeyCategory.GetKey(GameKeyEnum.SelectTargetForCommand).IsKeyDownInOrder(null);
			if (!this.IsSelectTargetForMouseClickingKeyDown)
			{
				RTSCommandVisualOrder.OrderToSelectTarget = SelectTargetMode.None;
			}
			return this.QueueCommand;
		}

		// Token: 0x040001A6 RID: 422
		protected bool QueueCommand;

		// Token: 0x040001A7 RID: 423
		protected bool IsSelectTargetForMouseClickingKeyDown;

		// Token: 0x040001A8 RID: 424
		public static bool IsFromClicking;

		// Token: 0x040001A9 RID: 425
		public static SelectTargetMode OrderToSelectTarget;
	}
}
