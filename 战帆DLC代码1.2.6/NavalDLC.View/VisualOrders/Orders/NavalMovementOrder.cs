using System;
using NavalDLC.Missions;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order.Visual;

namespace NavalDLC.View.VisualOrders.Orders
{
	// Token: 0x02000012 RID: 18
	public class NavalMovementOrder : VisualOrder
	{
		// Token: 0x06000074 RID: 116 RVA: 0x00005609 File Offset: 0x00003809
		public NavalMovementOrder(string stringId, OrderType order, TextObject name, bool useWorldPosition = false, bool isTargeted = false)
			: base(stringId)
		{
			this._orderType = order;
			this._useWorldPosition = useWorldPosition;
			this._isTargeted = isTargeted;
			this._name = name;
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00005630 File Offset: 0x00003830
		public override void ExecuteOrder(OrderController orderController, VisualOrderExecutionParameters executionParameters)
		{
			if (this._useWorldPosition && executionParameters.HasWorldPosition)
			{
				orderController.SetOrderWithPosition(this._orderType, executionParameters.WorldPosition);
				return;
			}
			if (this._isTargeted && executionParameters.HasFormation)
			{
				orderController.SetOrderWithFormation(this._orderType, executionParameters.Formation);
				return;
			}
			orderController.SetOrder(this._orderType);
		}

		// Token: 0x06000076 RID: 118 RVA: 0x0000568F File Offset: 0x0000388F
		public override TextObject GetName(OrderController orderController)
		{
			return this._name;
		}

		// Token: 0x06000077 RID: 119 RVA: 0x00005697 File Offset: 0x00003897
		public override bool IsTargeted()
		{
			return this._isTargeted;
		}

		// Token: 0x06000078 RID: 120 RVA: 0x000056A0 File Offset: 0x000038A0
		protected override bool? OnGetFormationHasOrder(Formation formation)
		{
			NavalShipsLogic missionBehavior = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			if (missionBehavior == null)
			{
				return new bool?(false);
			}
			ShipOrder.ShipMovementOrderEnum movementOrderEnum = this.GetMovementOrderEnum();
			MissionShip missionShip;
			missionBehavior.GetShip(formation.Team.TeamSide, formation.FormationIndex, out missionShip);
			if (missionShip == null)
			{
				return new bool?(false);
			}
			if (missionShip.IsPlayerShip || missionShip.IsPlayerControlled)
			{
				return null;
			}
			return new bool?(missionShip.ShipOrder.MovementOrderEnum == movementOrderEnum);
		}

		// Token: 0x06000079 RID: 121 RVA: 0x0000571C File Offset: 0x0000391C
		private ShipOrder.ShipMovementOrderEnum GetMovementOrderEnum()
		{
			OrderType orderType = this._orderType;
			if (orderType != 1)
			{
				switch (orderType)
				{
				case 6:
					return ShipOrder.ShipMovementOrderEnum.Stop;
				case 7:
					return ShipOrder.ShipMovementOrderEnum.StaticOrderCount;
				case 9:
					return ShipOrder.ShipMovementOrderEnum.Retreat;
				case 12:
					return ShipOrder.ShipMovementOrderEnum.Engage;
				}
				Debug.FailedAssert("Failed to find corresponding ship order of: " + this._orderType, "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.View\\VisualOrders\\Orders\\NavalMovementOrder.cs", "GetMovementOrderEnum", 96);
				return ShipOrder.ShipMovementOrderEnum.Move;
			}
			return ShipOrder.ShipMovementOrderEnum.Move;
		}

		// Token: 0x04000020 RID: 32
		private OrderType _orderType;

		// Token: 0x04000021 RID: 33
		private bool _useWorldPosition;

		// Token: 0x04000022 RID: 34
		private bool _isTargeted;

		// Token: 0x04000023 RID: 35
		private TextObject _name;
	}
}
