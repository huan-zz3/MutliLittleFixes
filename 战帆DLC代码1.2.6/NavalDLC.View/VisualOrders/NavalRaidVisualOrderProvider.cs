using System;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.VisualOrders.Orders.ToggleOrders;
using TaleWorlds.MountAndBlade.View.VisualOrders.OrderSets;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order.Visual;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order.Visual.Default.Orders.FormOrders;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order.Visual.Default.Orders.MovementOrders;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order.Visual.Default.Orders.ToggleOrders;

namespace NavalDLC.View.VisualOrders
{
	// Token: 0x0200000E RID: 14
	public class NavalRaidVisualOrderProvider : VisualOrderProvider
	{
		// Token: 0x06000068 RID: 104 RVA: 0x00004EB4 File Offset: 0x000030B4
		public override MBReadOnlyList<VisualOrderSet> GetOrders()
		{
			Mission mission = Mission.Current;
			bool flag;
			if (mission == null)
			{
				flag = false;
			}
			else
			{
				Team playerTeam = mission.PlayerTeam;
				BattleSideEnum? battleSideEnum = ((playerTeam != null) ? new BattleSideEnum?(playerTeam.Side) : null);
				BattleSideEnum battleSideEnum2 = 1;
				flag = (battleSideEnum.GetValueOrDefault() == battleSideEnum2) & (battleSideEnum != null);
			}
			bool flag2 = flag;
			MBList<VisualOrderSet> mblist = new MBList<VisualOrderSet>();
			GenericVisualOrderSet genericVisualOrderSet = new GenericVisualOrderSet("order_type_movement", new TextObject("{=KiJd6Xik}Movement", null), true, true);
			genericVisualOrderSet.AddOrder(new MoveVisualOrder("order_movement_move"));
			genericVisualOrderSet.AddOrder(new FollowMeVisualOrder("order_movement_follow"));
			genericVisualOrderSet.AddOrder(new ChargeVisualOrder("order_movement_charge"));
			genericVisualOrderSet.AddOrder(new AdvanceVisualOrder("order_movement_advance"));
			genericVisualOrderSet.AddOrder(new FallbackVisualOrder("order_movement_fallback"));
			genericVisualOrderSet.AddOrder(new StopVisualOrder("order_movement_stop"));
			if (!flag2)
			{
				genericVisualOrderSet.AddOrder(new RetreatVisualOrder("order_movement_retreat"));
			}
			genericVisualOrderSet.AddOrder(new ReturnVisualOrder());
			GenericVisualOrderSet genericVisualOrderSet2 = new GenericVisualOrderSet("order_type_form", new TextObject("{=iBk2wbn3}Form", null), true, true);
			ArrangementVisualOrder arrangementVisualOrder = new ArrangementVisualOrder(2, "order_form_line");
			ArrangementVisualOrder arrangementVisualOrder2 = new ArrangementVisualOrder(5, "order_form_close");
			genericVisualOrderSet2.AddOrder(arrangementVisualOrder);
			genericVisualOrderSet2.AddOrder(arrangementVisualOrder2);
			genericVisualOrderSet2.AddOrder(new ArrangementVisualOrder(3, "order_form_loose"));
			genericVisualOrderSet2.AddOrder(new ArrangementVisualOrder(0, "order_form_circular"));
			genericVisualOrderSet2.AddOrder(new ArrangementVisualOrder(7, "order_form_schiltron"));
			genericVisualOrderSet2.AddOrder(new ArrangementVisualOrder(6, "order_form_v"));
			genericVisualOrderSet2.AddOrder(new ArrangementVisualOrder(1, "order_form_column"));
			genericVisualOrderSet2.AddOrder(new ArrangementVisualOrder(4, "order_form_scatter"));
			genericVisualOrderSet2.AddOrder(new ReturnVisualOrder());
			GenericVisualOrderSet genericVisualOrderSet3 = new GenericVisualOrderSet("order_type_toggle", new TextObject("{=0HTNYQz2}Toggle", null), false, false);
			ToggleFacingVisualOrder toggleFacingVisualOrder = new ToggleFacingVisualOrder("order_toggle_facing");
			GenericToggleVisualOrder genericToggleVisualOrder = new GenericToggleVisualOrder("order_toggle_fire", 32, 31);
			GenericToggleVisualOrder genericToggleVisualOrder2 = (GameNetwork.IsMultiplayer ? null : new GenericToggleVisualOrder("order_toggle_ai", 36, 37));
			TransferTroopsVisualOrder transferTroopsVisualOrder = ((GameNetwork.IsMultiplayer || flag2) ? null : new TransferTroopsVisualOrder());
			genericVisualOrderSet3.AddOrder(toggleFacingVisualOrder);
			genericVisualOrderSet3.AddOrder(genericToggleVisualOrder);
			if (genericToggleVisualOrder2 != null)
			{
				genericVisualOrderSet3.AddOrder(genericToggleVisualOrder2);
			}
			if (transferTroopsVisualOrder != null)
			{
				genericVisualOrderSet3.AddOrder(transferTroopsVisualOrder);
			}
			genericVisualOrderSet3.AddOrder(new ReturnVisualOrder());
			mblist.Add(genericVisualOrderSet);
			mblist.Add(genericVisualOrderSet2);
			mblist.Add(genericVisualOrderSet3);
			if (!Input.IsGamepadActive)
			{
				mblist.Add(new SingleVisualOrderSet(genericToggleVisualOrder));
				if (genericToggleVisualOrder2 != null)
				{
					mblist.Add(new SingleVisualOrderSet(genericToggleVisualOrder2));
				}
				mblist.Add(new SingleVisualOrderSet(toggleFacingVisualOrder));
				mblist.Add(new SingleVisualOrderSet(arrangementVisualOrder2));
				mblist.Add(new SingleVisualOrderSet(arrangementVisualOrder));
			}
			return mblist;
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00005151 File Offset: 0x00003351
		public override bool IsAvailable()
		{
			return NavalDLCHelpers.IsNavalRaidMissionOpen();
		}
	}
}
