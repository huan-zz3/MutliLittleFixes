using System;
using NavalDLC.View.VisualOrders.Orders;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.VisualOrders.OrderSets;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order.Visual;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order.Visual.Default.Orders.ToggleOrders;

namespace NavalDLC.View.VisualOrders
{
	// Token: 0x0200000F RID: 15
	public class NavalShipVisualOrderProvider : VisualOrderProvider
	{
		// Token: 0x0600006B RID: 107 RVA: 0x00005160 File Offset: 0x00003360
		public override MBReadOnlyList<VisualOrderSet> GetOrders()
		{
			MBList<VisualOrderSet> mblist = new MBList<VisualOrderSet>();
			if (Input.IsGamepadActive)
			{
				GenericVisualOrderSet genericVisualOrderSet = new GenericVisualOrderSet("order_type_movement", new TextObject("{=KiJd6Xik}Movement", null), true, true);
				genericVisualOrderSet.AddOrder(new NavalMovementOrder("order_movement_move", 1, new TextObject("{=F7JGCr9s}Move", null), true, false));
				genericVisualOrderSet.AddOrder(new NavalMovementOrder("order_movement_follow", 7, new TextObject("{=5LpufKs7}Follow Me", null), false, false));
				genericVisualOrderSet.AddOrder(new NavalSkirmishOrder("order_movement_skirmish"));
				genericVisualOrderSet.AddOrder(new NavalMovementOrder("order_movement_advance", 12, new TextObject("{=A38xbjqm}Engage", null), false, true));
				genericVisualOrderSet.AddOrder(new NavalMovementOrder("order_movement_stop", 6, new TextObject("{=QTr6UDAa}Stop", null), false, false));
				genericVisualOrderSet.AddOrder(new NavalMovementOrder("order_movement_retreat", 9, new TextObject("{=VbeHEAsa}Retreat", null), false, false));
				genericVisualOrderSet.AddOrder(new ReturnVisualOrder());
				GenericVisualOrderSet genericVisualOrderSet2 = new GenericVisualOrderSet("order_type_toggle", new TextObject("{=0HTNYQz2}Toggle", null), false, false);
				GenericToggleVisualOrder genericToggleVisualOrder = new GenericToggleVisualOrder("order_toggle_fire", 32, 31);
				GenericToggleVisualOrder genericToggleVisualOrder2 = (GameNetwork.IsMultiplayer ? null : new GenericToggleVisualOrder("order_toggle_ai", 36, 37));
				genericVisualOrderSet2.AddOrder(genericToggleVisualOrder);
				if (genericToggleVisualOrder2 != null)
				{
					genericVisualOrderSet2.AddOrder(genericToggleVisualOrder2);
				}
				genericVisualOrderSet2.AddOrder(new ReturnVisualOrder());
				mblist.Add(genericVisualOrderSet);
				mblist.Add(genericVisualOrderSet2);
				if (genericToggleVisualOrder2 != null)
				{
					mblist.Add(new SingleVisualOrderSet(genericToggleVisualOrder2));
				}
				mblist.Add(new SingleVisualOrderSet(new ReturnVisualOrder()));
			}
			else
			{
				mblist.Add(this.CreateSingleOrderSetFor(new NavalMovementOrder("order_movement_move", 1, new TextObject("{=F7JGCr9s}Move", null), true, false)));
				mblist.Add(this.CreateSingleOrderSetFor(new NavalMovementOrder("order_movement_follow", 7, new TextObject("{=5LpufKs7}Follow Me", null), false, false)));
				mblist.Add(this.CreateSingleOrderSetFor(new NavalSkirmishOrder("order_movement_skirmish")));
				mblist.Add(this.CreateSingleOrderSetFor(new NavalMovementOrder("order_movement_advance", 12, new TextObject("{=A38xbjqm}Engage", null), false, true)));
				mblist.Add(this.CreateSingleOrderSetFor(new NavalMovementOrder("order_movement_stop", 6, new TextObject("{=QTr6UDAa}Stop", null), false, false)));
				mblist.Add(this.CreateSingleOrderSetFor(new NavalMovementOrder("order_movement_retreat", 9, new TextObject("{=VbeHEAsa}Retreat", null), false, false)));
				mblist.Add(this.CreateSingleOrderSetFor(new GenericToggleVisualOrder("order_toggle_fire", 32, 31)));
				GenericToggleVisualOrder genericToggleVisualOrder3 = (GameNetwork.IsMultiplayer ? null : new GenericToggleVisualOrder("order_toggle_ai", 36, 37));
				if (genericToggleVisualOrder3 != null)
				{
					mblist.Add(this.CreateSingleOrderSetFor(genericToggleVisualOrder3));
				}
			}
			return mblist;
		}

		// Token: 0x0600006C RID: 108 RVA: 0x000053EE File Offset: 0x000035EE
		private SingleVisualOrderSet CreateSingleOrderSetFor(VisualOrder order)
		{
			return new SingleVisualOrderSet(order);
		}

		// Token: 0x0600006D RID: 109 RVA: 0x000053F6 File Offset: 0x000035F6
		public override bool IsAvailable()
		{
			if (NavalDLCHelpers.IsNavalRaidMissionOpen())
			{
				return false;
			}
			Mission mission = Mission.Current;
			return mission != null && mission.IsNavalBattle && NavalDLCHelpers.IsShipOrdersAvailable();
		}
	}
}
