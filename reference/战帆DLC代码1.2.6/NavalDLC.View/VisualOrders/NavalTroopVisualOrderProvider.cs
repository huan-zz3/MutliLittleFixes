using System;
using NavalDLC.View.VisualOrders.Orders;
using NavalDLC.View.VisualOrders.Orders.TroopOrders;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.VisualOrders.OrderSets;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order.Visual;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order.Visual.Default.Orders.MovementOrders;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order.Visual.Default.Orders.ToggleOrders;

namespace NavalDLC.View.VisualOrders
{
	// Token: 0x02000010 RID: 16
	public class NavalTroopVisualOrderProvider : VisualOrderProvider
	{
		// Token: 0x0600006F RID: 111 RVA: 0x00005424 File Offset: 0x00003624
		public override MBReadOnlyList<VisualOrderSet> GetOrders()
		{
			MBList<VisualOrderSet> mblist = new MBList<VisualOrderSet>();
			if (Input.IsGamepadActive)
			{
				GenericVisualOrderSet genericVisualOrderSet = new GenericVisualOrderSet("troop_visual_orders", new TextObject("{=bEmrKaHS}Orders", null), true, false);
				genericVisualOrderSet.AddOrder(new NavalTroopDefendShipOrder("naval_troop_defend_ship_order"));
				genericVisualOrderSet.AddOrder(new FollowMeVisualOrder("order_movement_follow"));
				genericVisualOrderSet.AddOrder(new NavalChargeVisualOrder("order_movement_charge"));
				genericVisualOrderSet.AddOrder(new GenericToggleVisualOrder("order_toggle_fire", 32, 31));
				if (!GameNetwork.IsMultiplayer)
				{
					genericVisualOrderSet.AddOrder(new GenericToggleVisualOrder("order_toggle_ai", 36, 37));
				}
				genericVisualOrderSet.AddOrder(new ReturnVisualOrder());
				mblist.Add(genericVisualOrderSet);
				mblist.Add(new SingleVisualOrderSet(new ReturnVisualOrder()));
			}
			else
			{
				mblist.Add(new SingleVisualOrderSet(new NavalTroopDefendShipOrder("naval_troop_defend_ship_order")));
				mblist.Add(new SingleVisualOrderSet(new FollowMeVisualOrder("order_movement_follow")));
				mblist.Add(new SingleVisualOrderSet(new NavalChargeVisualOrder("order_movement_charge")));
				mblist.Add(new SingleVisualOrderSet(new GenericToggleVisualOrder("order_toggle_fire", 32, 31)));
				if (!GameNetwork.IsMultiplayer)
				{
					mblist.Add(new SingleVisualOrderSet(new GenericToggleVisualOrder("order_toggle_ai", 36, 37)));
				}
			}
			return mblist;
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00005555 File Offset: 0x00003755
		public override bool IsAvailable()
		{
			if (NavalDLCHelpers.IsNavalRaidMissionOpen())
			{
				return false;
			}
			Mission mission = Mission.Current;
			return mission != null && mission.IsNavalBattle && !NavalDLCHelpers.IsShipOrdersAvailable();
		}
	}
}
