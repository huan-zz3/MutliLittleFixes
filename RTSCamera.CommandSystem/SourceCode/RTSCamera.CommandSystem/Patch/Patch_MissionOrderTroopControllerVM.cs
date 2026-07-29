using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using MissionSharedLibrary.Utilities;
using RTSCamera.CommandSystem.Orders;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order;

namespace RTSCamera.CommandSystem.Patch
{
	// Token: 0x0200005F RID: 95
	public class Patch_MissionOrderTroopControllerVM
	{
		// Token: 0x06000345 RID: 837 RVA: 0x0000FAFC File Offset: 0x0000DCFC
		public static bool Patch(Harmony harmony)
		{
			bool flag;
			try
			{
				if (Patch_MissionOrderTroopControllerVM._patched)
				{
					flag = false;
				}
				else
				{
					Patch_MissionOrderTroopControllerVM._patched = true;
					harmony.Patch(typeof(MissionOrderTroopControllerVM).GetMethod("OrderController_OnTroopOrderIssued", BindingFlags.Instance | BindingFlags.NonPublic), new HarmonyMethod(typeof(Patch_MissionOrderTroopControllerVM).GetMethod("Prefix_OrderController_OnTroopOrderIssued", BindingFlags.Static | BindingFlags.Public)), null, null, null);
					flag = true;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				Utility.DisplayMessage(ex.ToString());
				MBDebug.Print(ex.ToString(), 0, 12, 17592186044416UL);
				flag = false;
			}
			return flag;
		}

		// Token: 0x06000346 RID: 838 RVA: 0x0000FB98 File Offset: 0x0000DD98
		public static void Prefix_OrderController_OnTroopOrderIssued(MissionOrderTroopControllerVM __instance, OrderType orderType, IEnumerable<Formation> appliedFormations, OrderController orderController)
		{
			Patch_MissionOrderTroopControllerVM.DisableSelectTargetMode();
		}

		// Token: 0x06000347 RID: 839 RVA: 0x0000FB9F File Offset: 0x0000DD9F
		private static void DisableSelectTargetMode()
		{
			RTSCommandVisualOrder.OrderToSelectTarget = SelectTargetMode.None;
		}

		// Token: 0x04000150 RID: 336
		private static FieldInfo ActiveOrders = typeof(OrderSubjectVM).GetField("ActiveOrders", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x04000151 RID: 337
		private static PropertyInfo _orderSubType = typeof(OrderItemVM).GetProperty("OrderSubType", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x04000152 RID: 338
		private static bool _patched;
	}
}
