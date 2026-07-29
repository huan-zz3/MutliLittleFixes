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
	// Token: 0x02000073 RID: 115
	public class RTSCommandGenericToggleVisualOrder : RTSCommandVisualOrder
	{
		// Token: 0x06000430 RID: 1072 RVA: 0x00019278 File Offset: 0x00017478
		public static TextObject GetName(OrderType orderType)
		{
			if (orderType == 14)
			{
				return new TextObject("{=u8j8nN5U}Face Enemy", null);
			}
			if (orderType != 15)
			{
				switch (orderType)
				{
				case 31:
					return new TextObject("{=VyI0rimN}Holding Fire", null);
				case 32:
					return new TextObject("{=itoYrj8d}Firing at will", null);
				case 34:
					return new TextObject("{=ubTGIdcv}Mounted", null);
				case 35:
					return new TextObject("{=Ema5Vd6o}Dismounted", null);
				case 36:
					return new TextObject("{=zatDiaEI}Delegate Command On", null);
				case 37:
					return new TextObject("{=JceqNdWx}Delegate Command Off", null);
				}
				return TextObject.GetEmpty();
			}
			return new TextObject("{=1gC25EMb}Face this Direction", null);
		}

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x06000431 RID: 1073 RVA: 0x0001931B File Offset: 0x0001751B
		public OrderType PositiveOrder { get; }

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x06000432 RID: 1074 RVA: 0x00019323 File Offset: 0x00017523
		public OrderType NegativeOrder { get; }

		// Token: 0x06000433 RID: 1075 RVA: 0x0001932B File Offset: 0x0001752B
		public RTSCommandGenericToggleVisualOrder(string stringId, OrderType positiveOrder, OrderType negativeOrder)
			: base(stringId)
		{
			this.PositiveOrder = positiveOrder;
			this.NegativeOrder = negativeOrder;
			this._positiveOrderName = RTSCommandGenericToggleVisualOrder.GetName(positiveOrder);
			this._negativeOrderName = RTSCommandGenericToggleVisualOrder.GetName(negativeOrder);
		}

		// Token: 0x06000434 RID: 1076 RVA: 0x0001935C File Offset: 0x0001755C
		public override TextObject GetName(OrderController orderController)
		{
			OrderState activeState = base.GetActiveState(orderController);
			if (activeState - 2 <= 1)
			{
				return this._positiveOrderName;
			}
			return this._negativeOrderName;
		}

		// Token: 0x06000435 RID: 1077 RVA: 0x00019384 File Offset: 0x00017584
		public override bool IsTargeted()
		{
			return false;
		}

		// Token: 0x06000436 RID: 1078 RVA: 0x00019388 File Offset: 0x00017588
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
			orderInQueue.VirtualFormationChanges = Patch_OrderController.LivePreviewFormationChanges.CollectChanges(list);
			if (flag)
			{
				CommandQueueLogic.AddOrderToQueue(orderInQueue);
				return;
			}
			CommandQueueLogic.TryPendingOrder(orderInQueue.SelectedFormations, orderInQueue);
			orderController.SetOrder(orderInQueue.OrderType);
			if (orderInQueue.OrderType == 36)
			{
				foreach (Formation formation in orderController.SelectedFormations)
				{
					CommandQueueLogic.SetFormationVolleyMode(formation, VolleyMode.Disabled);
				}
			}
		}

		// Token: 0x06000437 RID: 1079 RVA: 0x0001948C File Offset: 0x0001768C
		protected override bool? OnGetFormationHasOrder(Formation formation)
		{
			return new bool?(Utility.DoesFormationHasOrderType(formation, this.PositiveOrder));
		}

		// Token: 0x06000438 RID: 1080 RVA: 0x000194A0 File Offset: 0x000176A0
		protected override string GetIconId()
		{
			string iconId = base.GetIconId();
			if (this._lastActiveState != 3)
			{
				return iconId;
			}
			return iconId + "_active";
		}

		// Token: 0x040001AD RID: 429
		private readonly TextObject _positiveOrderName;

		// Token: 0x040001AE RID: 430
		private readonly TextObject _negativeOrderName;
	}
}
