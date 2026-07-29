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
	// Token: 0x0200006F RID: 111
	public class RTSCommandArrangementVisualOrder : RTSCommandVisualOrder
	{
		// Token: 0x06000416 RID: 1046 RVA: 0x00018CA0 File Offset: 0x00016EA0
		public static TextObject GetName(ArrangementOrder.ArrangementOrderEnum order)
		{
			switch (order)
			{
			case 0:
				return new TextObject("{=9TGLirQf}Circle", null);
			case 1:
				return new TextObject("{=WsmZzaOq}Column", null);
			case 2:
				return new TextObject("{=9aboazgu}Line", null);
			case 3:
				return new TextObject("{=iJXH3841}Loose", null);
			case 4:
				return new TextObject("{=eEf7hE4r}Scatter", null);
			case 5:
				return new TextObject("{=rTPnyeJ3}Shield Wall", null);
			case 6:
				return new TextObject("{=uCyQNvq1}Skein", null);
			case 7:
				return new TextObject("{=E3tCWX7w}Square", null);
			default:
				return TextObject.GetEmpty();
			}
		}

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x06000417 RID: 1047 RVA: 0x00018D3A File Offset: 0x00016F3A
		public ArrangementOrder.ArrangementOrderEnum ArrangementOrder { get; }

		// Token: 0x06000418 RID: 1048 RVA: 0x00018D42 File Offset: 0x00016F42
		public RTSCommandArrangementVisualOrder(ArrangementOrder.ArrangementOrderEnum arrangementOrder, string iconId)
			: base(iconId)
		{
			this.ArrangementOrder = arrangementOrder;
		}

		// Token: 0x06000419 RID: 1049 RVA: 0x00018D52 File Offset: 0x00016F52
		public override TextObject GetName(OrderController orderController)
		{
			return RTSCommandArrangementVisualOrder.GetName(this.ArrangementOrder);
		}

		// Token: 0x0600041A RID: 1050 RVA: 0x00018D60 File Offset: 0x00016F60
		public override void ExecuteOrder(OrderController orderController, VisualOrderExecutionParameters executionParameters)
		{
			bool flag = base.OnBeforeExecuteOrder(orderController, executionParameters);
			List<Formation> list = orderController.SelectedFormations.Where<Formation>((Formation f) => f.CountOfUnitsWithoutDetachedOnes > 0).ToList<Formation>();
			OrderInQueue orderInQueue = new OrderInQueue
			{
				SelectedFormations = list
			};
			orderInQueue.OrderType = Utility.ArrangementOrderEnumToOrderType(this.ArrangementOrder);
			bool flag2 = Utility.ShouldFadeOut();
			List<WorldPosition> list2;
			List<ValueTuple<Formation, int, float, WorldPosition, Vec2>> list3;
			Patch_OrderController.SimulateNewArrangementOrder(list, orderController.simulationFormations, this.ArrangementOrder, flag2, out list2, true, out list3);
			orderInQueue.VirtualFormationChanges = Patch_OrderController.LivePreviewFormationChanges.CollectChanges(list);
			if (flag2)
			{
				Patch_OrderTroopPlacer.AddOrderPositionEntities(list2, true, 0);
			}
			if (!flag)
			{
				RTSCommandArrangementVisualOrder.ExecuteArrangementOrder(orderController, orderInQueue);
				return;
			}
			CommandQueueLogic.AddOrderToQueue(orderInQueue);
		}

		// Token: 0x0600041B RID: 1051 RVA: 0x00018E0D File Offset: 0x0001700D
		protected override bool? OnGetFormationHasOrder(Formation formation)
		{
			return new bool?(OrderController.GetActiveArrangementOrderOf(formation) == Utility.ArrangementOrderEnumToOrderType(this.ArrangementOrder));
		}

		// Token: 0x0600041C RID: 1052 RVA: 0x00018E27 File Offset: 0x00017027
		public override bool IsTargeted()
		{
			return false;
		}

		// Token: 0x0600041D RID: 1053 RVA: 0x00018E2C File Offset: 0x0001702C
		private static void ExecuteArrangementOrder(OrderController orderController, OrderInQueue order)
		{
			Patch_OrderController.LivePreviewFormationChanges.SetChanges(order.VirtualFormationChanges);
			orderController.SetOrder(order.OrderType);
			foreach (KeyValuePair<Formation, FormationChange> keyValuePair in order.VirtualFormationChanges)
			{
				Formation key = keyValuePair.Key;
				FormationChange value = keyValuePair.Value;
				Formation formation = key;
				int? num = value.UnitSpacing;
				formation.SetPositioning(null, null, num);
				if (value.Width != null)
				{
					key.SetFormOrder(FormOrder.FormOrderCustom(value.Width.Value), true);
					Formation formation2 = key;
					num = value.UnitSpacing;
					formation2.SetPositioning(null, null, num);
				}
			}
			CommandQueueLogic.TryTeleportSelectedFormationInDeployment(orderController, order.SelectedFormations);
			CommandQueueLogic.CurrentFormationChanges.SetChanges(order.VirtualFormationChanges);
			foreach (KeyValuePair<Formation, FormationChange> keyValuePair2 in order.VirtualFormationChanges)
			{
				Formation key2 = keyValuePair2.Key;
				CommandQueueLogic.CurrentFormationChanges.UpdateFormationChange(key2, null, new Vec2?(key2.Direction), null, null);
			}
		}
	}
}
