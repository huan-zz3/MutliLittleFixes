using System;
using RTSCamera.CommandSystem.Logic;
using RTSCamera.CommandSystem.Orders.VisualOrders;
using SandBox.Missions.MissionLogics.Hideout;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.VisualOrders.OrderSets;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order.Visual;

namespace RTSCamera.CommandSystem.Orders
{
	// Token: 0x0200006C RID: 108
	public class RTSCommandVisualOrderProvider : VisualOrderProvider
	{
		// Token: 0x17000097 RID: 151
		// (get) Token: 0x06000403 RID: 1027 RVA: 0x00018266 File Offset: 0x00016466
		private bool IsHideOut
		{
			get
			{
				Mission mission = Mission.Current;
				return mission != null && mission.HasMissionBehavior<HideoutMissionController>();
			}
		}

		// Token: 0x06000404 RID: 1028 RVA: 0x00018278 File Offset: 0x00016478
		public override bool IsAvailable()
		{
			return Mission.Current != null && !Mission.Current.IsFriendlyMission && !Mission.Current.IsNavalBattle;
		}

		// Token: 0x06000405 RID: 1029 RVA: 0x0001829C File Offset: 0x0001649C
		public override MBReadOnlyList<VisualOrderSet> GetOrders()
		{
			if (BannerlordConfig.OrderLayoutType != 1 || this.IsNavalRaid)
			{
				return this.GetDefaultOrders();
			}
			return this.GetLegacyOrders();
		}

		// Token: 0x06000406 RID: 1030 RVA: 0x000182BC File Offset: 0x000164BC
		private MBReadOnlyList<VisualOrderSet> GetDefaultOrders()
		{
			MBList<VisualOrderSet> mblist = new MBList<VisualOrderSet>();
			RTSCommandGenericVisualOrderSet rtscommandGenericVisualOrderSet = new RTSCommandGenericVisualOrderSet("order_type_movement", new TextObject("{=KiJd6Xik}Movement", null), false, true, null);
			rtscommandGenericVisualOrderSet.AddOrder(new RTSCommandMoveVisualOrder("order_movement_move"));
			rtscommandGenericVisualOrderSet.AddOrder(new RTSCommandFollowMeVisualOrder("order_movement_follow"));
			rtscommandGenericVisualOrderSet.AddOrder(new RTSCommandChargeVisualOrder("order_movement_charge"));
			if (!this.IsHideOut)
			{
				rtscommandGenericVisualOrderSet.AddOrder(new RTSCommandAdvanceVisualOrder("order_movement_advance"));
			}
			rtscommandGenericVisualOrderSet.AddOrder(new RTSCommandFallbackVisualOrder("order_movement_fallback"));
			rtscommandGenericVisualOrderSet.AddOrder(new RTSCommandStopVisualOrder("order_movement_stop"));
			rtscommandGenericVisualOrderSet.AddOrder(new RTSCommandRetreatVisualOrder("order_movement_retreat"));
			rtscommandGenericVisualOrderSet.AddOrder(new ReturnVisualOrder());
			GenericVisualOrderSet genericVisualOrderSet = new GenericVisualOrderSet("order_type_form", new TextObject("{=iBk2wbn3}Form", null), true, true);
			RTSCommandArrangementVisualOrder rtscommandArrangementVisualOrder = new RTSCommandArrangementVisualOrder(2, "order_form_line");
			RTSCommandArrangementVisualOrder rtscommandArrangementVisualOrder2 = new RTSCommandArrangementVisualOrder(5, "order_form_close");
			genericVisualOrderSet.AddOrder(rtscommandArrangementVisualOrder);
			genericVisualOrderSet.AddOrder(rtscommandArrangementVisualOrder2);
			genericVisualOrderSet.AddOrder(new RTSCommandArrangementVisualOrder(3, "order_form_loose"));
			genericVisualOrderSet.AddOrder(new RTSCommandArrangementVisualOrder(0, "order_form_circular"));
			genericVisualOrderSet.AddOrder(new RTSCommandArrangementVisualOrder(7, "order_form_schiltron"));
			genericVisualOrderSet.AddOrder(new RTSCommandArrangementVisualOrder(6, "order_form_v"));
			genericVisualOrderSet.AddOrder(new RTSCommandArrangementVisualOrder(1, "order_form_column"));
			genericVisualOrderSet.AddOrder(new RTSCommandArrangementVisualOrder(4, "order_form_scatter"));
			genericVisualOrderSet.AddOrder(new ReturnVisualOrder());
			GenericVisualOrderSet genericVisualOrderSet2 = new GenericVisualOrderSet("order_type_toggle", new TextObject("{=0HTNYQz2}Toggle", null), false, false);
			RTSCommandToggleFacingVisualOrder rtscommandToggleFacingVisualOrder = new RTSCommandToggleFacingVisualOrder("order_toggle_facing");
			RTSCommandToggleVolleyVisualOrder rtscommandToggleVolleyVisualOrder = new RTSCommandToggleVolleyVisualOrder("order_auto_volley", GameTexts.FindText("str_rts_camera_command_system_auto_volley", null), GameTexts.FindText("str_rts_camera_command_system_auto_volley_off", null), VolleyMode.Auto);
			RTSCommandToggleVolleyVisualOrder rtscommandToggleVolleyVisualOrder2 = new RTSCommandToggleVolleyVisualOrder("order_manual_volley", GameTexts.FindText("str_rts_camera_command_system_manual_volley", null), GameTexts.FindText("str_rts_camera_command_system_manual_volley_off", null), VolleyMode.Manual);
			RTSCommandToggleFireVisualOrder rtscommandToggleFireVisualOrder = new RTSCommandToggleFireVisualOrder("order_toggle_fire", 32, 31, rtscommandToggleVolleyVisualOrder, rtscommandToggleVolleyVisualOrder2);
			RTSCommandGenericToggleVisualOrder rtscommandGenericToggleVisualOrder = new RTSCommandGenericToggleVisualOrder("order_toggle_mount", 34, 35);
			RTSCommandGenericToggleVisualOrder rtscommandGenericToggleVisualOrder2 = (GameNetwork.IsMultiplayer ? null : new RTSCommandGenericToggleVisualOrder("order_toggle_ai", 36, 37));
			TransferTroopsVisualOrder transferTroopsVisualOrder = (GameNetwork.IsMultiplayer ? null : new TransferTroopsVisualOrder());
			RTSCommandActivateFacingVisualOrder rtscommandActivateFacingVisualOrder = new RTSCommandActivateFacingVisualOrder(15, "order_toggle_facing");
			genericVisualOrderSet2.AddOrder(rtscommandToggleFacingVisualOrder);
			genericVisualOrderSet2.AddOrder(rtscommandToggleFireVisualOrder);
			if (!this.IsNavalRaid)
			{
				genericVisualOrderSet2.AddOrder(rtscommandGenericToggleVisualOrder);
			}
			if (rtscommandGenericToggleVisualOrder2 != null)
			{
				genericVisualOrderSet2.AddOrder(rtscommandGenericToggleVisualOrder2);
			}
			if (transferTroopsVisualOrder != null)
			{
				genericVisualOrderSet2.AddOrder(transferTroopsVisualOrder);
			}
			genericVisualOrderSet2.AddOrder(rtscommandToggleVolleyVisualOrder);
			genericVisualOrderSet2.AddOrder(rtscommandToggleVolleyVisualOrder2);
			genericVisualOrderSet2.AddOrder(new RTSCommandVolleyFireVisualOrder("order_volley_fire"));
			genericVisualOrderSet2.AddOrder(new ReturnVisualOrder());
			mblist.Add(rtscommandGenericVisualOrderSet);
			mblist.Add(genericVisualOrderSet);
			mblist.Add(genericVisualOrderSet2);
			if (!Input.IsGamepadActive)
			{
				mblist.Add(new SingleVisualOrderSet(rtscommandToggleFireVisualOrder));
				mblist.Add(new SingleVisualOrderSet(rtscommandGenericToggleVisualOrder));
				if (rtscommandGenericToggleVisualOrder2 != null)
				{
					mblist.Add(new SingleVisualOrderSet(rtscommandGenericToggleVisualOrder2));
				}
				mblist.Add(new SingleVisualOrderSet(rtscommandActivateFacingVisualOrder));
				mblist.Add(new SingleVisualOrderSet(rtscommandArrangementVisualOrder2));
				mblist.Add(new SingleVisualOrderSet(rtscommandArrangementVisualOrder));
			}
			mblist.Add(new SingleVisualOrderSet(new ReturnVisualOrder()));
			return mblist;
		}

		// Token: 0x06000407 RID: 1031 RVA: 0x000185D0 File Offset: 0x000167D0
		private MBList<VisualOrderSet> GetLegacyOrders()
		{
			MBList<VisualOrderSet> mblist = new MBList<VisualOrderSet>();
			RTSCommandGenericVisualOrderSet rtscommandGenericVisualOrderSet = new RTSCommandGenericVisualOrderSet("order_type_movement", new TextObject("{=KiJd6Xik}Movement", null), false, false, null);
			rtscommandGenericVisualOrderSet.AddOrder(new RTSCommandMoveVisualOrder("order_movement_move"));
			rtscommandGenericVisualOrderSet.AddOrder(new RTSCommandFollowMeVisualOrder("order_movement_follow"));
			rtscommandGenericVisualOrderSet.AddOrder(new RTSCommandChargeVisualOrder("order_movement_charge"));
			if (!this.IsHideOut)
			{
				rtscommandGenericVisualOrderSet.AddOrder(new RTSCommandAdvanceVisualOrder("order_movement_advance"));
			}
			rtscommandGenericVisualOrderSet.AddOrder(new RTSCommandFallbackVisualOrder("order_movement_fallback"));
			rtscommandGenericVisualOrderSet.AddOrder(new RTSCommandStopVisualOrder("order_movement_stop"));
			rtscommandGenericVisualOrderSet.AddOrder(new RTSCommandRetreatVisualOrder("order_movement_retreat"));
			rtscommandGenericVisualOrderSet.AddOrder(new ReturnVisualOrder());
			RTSCommandGenericVisualOrderSet rtscommandGenericVisualOrderSet2 = new RTSCommandGenericVisualOrderSet("order_type_facing", new TextObject("{=psynaDsM}Facing", null), false, false, null);
			RTSCommandSingleVisualOrder rtscommandSingleVisualOrder = new RTSCommandSingleVisualOrder("order_toggle_facing", new TextObject("{=MH9Pi3ao}Face Direction", null), 15, false, true);
			RTSCommandSingleVisualOrder rtscommandSingleVisualOrder2 = new RTSCommandSingleVisualOrder("order_toggle_facing_active", new TextObject("{=u8j8nN5U}Face Enemy", null), 14, true, false);
			rtscommandGenericVisualOrderSet2.AddOrder(rtscommandSingleVisualOrder);
			rtscommandGenericVisualOrderSet2.AddOrder(rtscommandSingleVisualOrder2);
			RTSCommandToggleVolleyVisualOrder rtscommandToggleVolleyVisualOrder = new RTSCommandToggleVolleyVisualOrder("order_auto_volley", GameTexts.FindText("str_rts_camera_command_system_auto_volley", null), GameTexts.FindText("str_rts_camera_command_system_auto_volley_off", null), VolleyMode.Auto);
			RTSCommandToggleVolleyVisualOrder rtscommandToggleVolleyVisualOrder2 = new RTSCommandToggleVolleyVisualOrder("order_manual_volley", GameTexts.FindText("str_rts_camera_command_system_manual_volley", null), GameTexts.FindText("str_rts_camera_command_system_manual_volley_off", null), VolleyMode.Manual);
			GenericVisualOrderSet genericVisualOrderSet = new GenericVisualOrderSet("order_type_form", new TextObject("{=iBk2wbn3}Form", null), true, true);
			RTSCommandArrangementVisualOrder rtscommandArrangementVisualOrder = new RTSCommandArrangementVisualOrder(2, "order_form_line");
			RTSCommandArrangementVisualOrder rtscommandArrangementVisualOrder2 = new RTSCommandArrangementVisualOrder(5, "order_form_close");
			genericVisualOrderSet.AddOrder(rtscommandArrangementVisualOrder);
			genericVisualOrderSet.AddOrder(rtscommandArrangementVisualOrder2);
			genericVisualOrderSet.AddOrder(new RTSCommandArrangementVisualOrder(3, "order_form_loose"));
			genericVisualOrderSet.AddOrder(new RTSCommandArrangementVisualOrder(0, "order_form_circular"));
			genericVisualOrderSet.AddOrder(new RTSCommandArrangementVisualOrder(7, "order_form_schiltron"));
			genericVisualOrderSet.AddOrder(new RTSCommandArrangementVisualOrder(6, "order_form_v"));
			genericVisualOrderSet.AddOrder(new RTSCommandArrangementVisualOrder(1, "order_form_column"));
			genericVisualOrderSet.AddOrder(new RTSCommandArrangementVisualOrder(4, "order_form_scatter"));
			genericVisualOrderSet.AddOrder(new ReturnVisualOrder());
			mblist.Add(rtscommandGenericVisualOrderSet);
			mblist.Add(rtscommandGenericVisualOrderSet2);
			mblist.Add(genericVisualOrderSet);
			RTSCommandToggleFireVisualOrder rtscommandToggleFireVisualOrder = new RTSCommandToggleFireVisualOrder("order_toggle_fire", 32, 31, rtscommandToggleVolleyVisualOrder, rtscommandToggleVolleyVisualOrder2);
			RTSCommandGenericToggleVisualOrder rtscommandGenericToggleVisualOrder = new RTSCommandGenericToggleVisualOrder("order_toggle_mount", 34, 35);
			RTSCommandGenericToggleVisualOrder rtscommandGenericToggleVisualOrder2 = (GameNetwork.IsMultiplayer ? null : new RTSCommandGenericToggleVisualOrder("order_toggle_ai", 36, 37));
			TransferTroopsVisualOrder transferTroopsVisualOrder = (GameNetwork.IsMultiplayer ? null : new TransferTroopsVisualOrder());
			if (!Input.IsGamepadActive)
			{
				mblist.Add(new SingleVisualOrderSet(rtscommandToggleFireVisualOrder));
				mblist.Add(new SingleVisualOrderSet(rtscommandGenericToggleVisualOrder));
				if (rtscommandGenericToggleVisualOrder2 != null)
				{
					mblist.Add(new SingleVisualOrderSet(rtscommandGenericToggleVisualOrder2));
				}
				if (transferTroopsVisualOrder != null)
				{
					mblist.Add(new SingleVisualOrderSet(transferTroopsVisualOrder));
				}
			}
			RTSCommandGenericVisualOrderSet rtscommandGenericVisualOrderSet3 = new RTSCommandGenericVisualOrderSet("order_type_volley", GameTexts.FindText("str_rts_camera_command_system_volley_order", null), true, true, rtscommandToggleVolleyVisualOrder);
			rtscommandGenericVisualOrderSet3.AddOrder(rtscommandToggleVolleyVisualOrder);
			rtscommandGenericVisualOrderSet3.AddOrder(rtscommandToggleVolleyVisualOrder2);
			rtscommandGenericVisualOrderSet3.AddOrder(new RTSCommandVolleyFireVisualOrder("order_volley_fire"));
			rtscommandGenericVisualOrderSet3.AddOrder(new ReturnVisualOrder());
			mblist.Add(rtscommandGenericVisualOrderSet3);
			if (!Input.IsGamepadActive)
			{
				mblist.Add(new SingleVisualOrderSet(new ReturnVisualOrder()));
			}
			return mblist;
		}

		// Token: 0x06000408 RID: 1032 RVA: 0x000188F1 File Offset: 0x00016AF1
		public RTSCommandVisualOrderProvider()
		{
			Mission mission = Mission.Current;
			this.IsNavalRaid = mission != null && mission.IsNavalRaidBattle;
			base..ctor();
		}

		// Token: 0x040001AA RID: 426
		private bool IsNavalRaid;
	}
}
