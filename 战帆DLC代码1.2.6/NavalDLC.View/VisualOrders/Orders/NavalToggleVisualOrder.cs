using System;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order.Visual;

namespace NavalDLC.View.VisualOrders.Orders
{
	// Token: 0x02000014 RID: 20
	public class NavalToggleVisualOrder : VisualOrder
	{
		// Token: 0x0600007F RID: 127 RVA: 0x00005838 File Offset: 0x00003A38
		public NavalToggleVisualOrder(string stringId, OrderType positiveOrder, OrderType negativeOrder, TextObject positiveOrderName, TextObject negativeOrderName)
			: base(stringId)
		{
			this._positiveOrder = positiveOrder;
			this._negativeOrder = negativeOrder;
			this._positiveOrderName = positiveOrderName;
			this._negativeOrderName = negativeOrderName;
		}

		// Token: 0x06000080 RID: 128 RVA: 0x0000585F File Offset: 0x00003A5F
		public override void ExecuteOrder(OrderController orderController, VisualOrderExecutionParameters executionParameters)
		{
			if (base.GetActiveState(orderController) == 3)
			{
				orderController.SetOrder(this._negativeOrder);
				return;
			}
			orderController.SetOrder(this._positiveOrder);
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00005884 File Offset: 0x00003A84
		public override TextObject GetName(OrderController orderController)
		{
			OrderState activeState = base.GetActiveState(orderController);
			if (activeState == 3 || activeState == 2)
			{
				return this._positiveOrderName;
			}
			return this._negativeOrderName;
		}

		// Token: 0x06000082 RID: 130 RVA: 0x000058AE File Offset: 0x00003AAE
		public override bool IsTargeted()
		{
			return false;
		}

		// Token: 0x06000083 RID: 131 RVA: 0x000058B4 File Offset: 0x00003AB4
		protected override bool? OnGetFormationHasOrder(Formation formation)
		{
			NavalShipsLogic missionBehavior = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			if (missionBehavior == null)
			{
				return new bool?(false);
			}
			MissionShip missionShip;
			missionBehavior.GetShip(formation.Team.TeamSide, formation.FormationIndex, out missionShip);
			OrderType positiveOrder = this._positiveOrder;
			int num = positiveOrder;
			if (num == 14)
			{
				return new bool?(missionShip.ShipOrder.BoardAtWill);
			}
			if (num != 35)
			{
				return new bool?(VisualOrderHelper.DoesFormationHaveOrderType(formation, this._positiveOrder));
			}
			if (formation.GetReadonlyMovementOrderReference().OrderEnum != 2)
			{
				return new bool?(false);
			}
			return new bool?(true);
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00005944 File Offset: 0x00003B44
		protected override string GetIconId()
		{
			string iconId = base.GetIconId();
			if (this._lastActiveState == 3)
			{
				return iconId + "_active";
			}
			return iconId;
		}

		// Token: 0x04000025 RID: 37
		private OrderType _positiveOrder;

		// Token: 0x04000026 RID: 38
		private OrderType _negativeOrder;

		// Token: 0x04000027 RID: 39
		private TextObject _positiveOrderName;

		// Token: 0x04000028 RID: 40
		private TextObject _negativeOrderName;
	}
}
