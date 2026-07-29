using System;
using System.Collections.Generic;
using System.Linq;
using RTSCamera.CommandSystem.Logic;
using RTSCamera.CommandSystem.Patch;
using RTSCamera.CommandSystem.Utilities;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order.Visual;

namespace RTSCamera.CommandSystem.Orders.VisualOrders
{
	// Token: 0x0200007A RID: 122
	public class RTSCommandToggleFireVisualOrder : RTSCommandVisualOrder
	{
		// Token: 0x0600045D RID: 1117 RVA: 0x00019E3C File Offset: 0x0001803C
		public static TextObject GetName(OrderType orderType)
		{
			if (orderType == 31)
			{
				return new TextObject("{=VyI0rimN}Holding Fire", null);
			}
			if (orderType != 32)
			{
				return TextObject.GetEmpty();
			}
			return new TextObject("{=itoYrj8d}Firing at will", null);
		}

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x0600045E RID: 1118 RVA: 0x00019E67 File Offset: 0x00018067
		public OrderType PositiveOrder { get; }

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x0600045F RID: 1119 RVA: 0x00019E6F File Offset: 0x0001806F
		public OrderType NegativeOrder { get; }

		// Token: 0x06000460 RID: 1120 RVA: 0x00019E77 File Offset: 0x00018077
		public RTSCommandToggleFireVisualOrder(string stringId, OrderType positiveOrder, OrderType negativeOrder, RTSCommandToggleVolleyVisualOrder autoVolleyVisualOrder, RTSCommandToggleVolleyVisualOrder manualVolleyVisualOrder)
			: base(stringId)
		{
			this.PositiveOrder = positiveOrder;
			this.NegativeOrder = negativeOrder;
			this._positiveOrderName = RTSCommandToggleFireVisualOrder.GetName(positiveOrder);
			this._negativeOrderName = RTSCommandToggleFireVisualOrder.GetName(negativeOrder);
			this._autoVolleyVisualOrder = autoVolleyVisualOrder;
			this._manualVolleyVisualOrder = manualVolleyVisualOrder;
		}

		// Token: 0x06000461 RID: 1121 RVA: 0x00019EB8 File Offset: 0x000180B8
		public override TextObject GetName(OrderController orderController)
		{
			OrderState activeState = base.GetActiveState(orderController);
			if (activeState - 2 > 1)
			{
				return this._negativeOrderName;
			}
			OrderState lastActiveState = this._autoVolleyVisualOrder.LastActiveState;
			if (lastActiveState - 2 <= 1)
			{
				return this._autoVolleyVisualOrder.GetName(orderController);
			}
			OrderState lastActiveState2 = this._manualVolleyVisualOrder.LastActiveState;
			if (lastActiveState2 - 2 <= 1)
			{
				return this._manualVolleyVisualOrder.GetName(orderController);
			}
			return this._positiveOrderName;
		}

		// Token: 0x06000462 RID: 1122 RVA: 0x00019F1E File Offset: 0x0001811E
		public override bool IsTargeted()
		{
			return false;
		}

		// Token: 0x06000463 RID: 1123 RVA: 0x00019F24 File Offset: 0x00018124
		public override void ExecuteOrder(OrderController orderController, VisualOrderExecutionParameters executionParameters)
		{
			bool flag = base.OnBeforeExecuteOrder(orderController, executionParameters);
			List<Formation> list = orderController.SelectedFormations.Where<Formation>((Formation f) => f.CountOfUnitsWithoutDetachedOnes > 0).ToList<Formation>();
			OrderInQueue orderInQueue = new OrderInQueue
			{
				SelectedFormations = list
			};
			orderInQueue.OrderType = ((base.GetActiveState(orderController) == 3) ? this.NegativeOrder : this.PositiveOrder);
			Patch_OrderController.LivePreviewFormationChanges.SetToggleOrder(orderInQueue.OrderType, list);
			Patch_OrderController.LivePreviewFormationChanges.SetVolleyMode(VolleyMode.Disabled, list);
			orderInQueue.VirtualFormationChanges = Patch_OrderController.LivePreviewFormationChanges.CollectChanges(list);
			if (flag)
			{
				CommandQueueLogic.AddOrderToQueue(orderInQueue);
				return;
			}
			CommandQueueLogic.TryPendingOrder(orderInQueue.SelectedFormations, orderInQueue);
			orderController.SetOrder(orderInQueue.OrderType);
			foreach (Formation formation in orderController.SelectedFormations)
			{
				CommandQueueLogic.SetFormationVolleyMode(formation, VolleyMode.Disabled);
			}
		}

		// Token: 0x06000464 RID: 1124 RVA: 0x0001A028 File Offset: 0x00018228
		protected override bool? OnGetFormationHasOrder(Formation formation)
		{
			return new bool?(Utility.DoesFormationHasOrderType(formation, this.PositiveOrder));
		}

		// Token: 0x06000465 RID: 1125 RVA: 0x0001A03C File Offset: 0x0001823C
		protected override string GetIconId()
		{
			string iconId = base.GetIconId();
			if (this._lastActiveState != 3 || this._manualVolleyVisualOrder.LastActiveState == 3)
			{
				return iconId;
			}
			return iconId + "_active";
		}

		// Token: 0x040001BA RID: 442
		private readonly TextObject _positiveOrderName;

		// Token: 0x040001BB RID: 443
		private readonly TextObject _negativeOrderName;

		// Token: 0x040001BC RID: 444
		private readonly RTSCommandToggleVolleyVisualOrder _autoVolleyVisualOrder;

		// Token: 0x040001BD RID: 445
		private readonly RTSCommandToggleVolleyVisualOrder _manualVolleyVisualOrder;
	}
}
