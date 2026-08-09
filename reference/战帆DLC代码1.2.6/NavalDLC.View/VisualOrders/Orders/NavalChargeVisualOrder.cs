using System;
using NavalDLC.Missions;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order.Visual.Default.Orders.MovementOrders;

namespace NavalDLC.View.VisualOrders.Orders
{
	// Token: 0x02000011 RID: 17
	public class NavalChargeVisualOrder : ChargeVisualOrder
	{
		// Token: 0x06000072 RID: 114 RVA: 0x00005585 File Offset: 0x00003785
		public NavalChargeVisualOrder(string iconId)
			: base(iconId)
		{
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00005590 File Offset: 0x00003790
		protected override bool? OnGetFormationHasOrder(Formation formation)
		{
			bool? flag = base.OnGetFormationHasOrder(formation);
			bool flag2 = true;
			if ((flag.GetValueOrDefault() == flag2) & (flag != null))
			{
				return new bool?(true);
			}
			if (OrderController.GetActiveMovementOrderOf(formation) == 1)
			{
				Mission mission = Mission.Current;
				NavalShipsLogic navalShipsLogic = ((mission != null) ? mission.GetMissionBehavior<NavalShipsLogic>() : null);
				MissionShip missionShip;
				if (navalShipsLogic != null && navalShipsLogic.GetShip(formation, out missionShip))
				{
					ShipOrder shipOrder = missionShip.ShipOrder;
					return new bool?(shipOrder != null && shipOrder.GetIsChargeOrderOverridden());
				}
			}
			return new bool?(false);
		}
	}
}
