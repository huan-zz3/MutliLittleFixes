using System;
using System.Reflection;
using HarmonyLib;
using MissionSharedLibrary.Utilities;
using RTSCamera.CommandSystem.Orders;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order.Visual;

namespace RTSCamera.CommandSystem.Patch
{
	// Token: 0x02000063 RID: 99
	public class Patch_OrderSetVM
	{
		// Token: 0x060003C3 RID: 963 RVA: 0x00015D94 File Offset: 0x00013F94
		public static bool Patch(Harmony harmony)
		{
			try
			{
				if (Patch_OrderSetVM._patched)
				{
					return false;
				}
				Patch_OrderSetVM._patched = true;
				harmony.Patch(typeof(OrderSetVM).GetMethod("RefreshOrders", BindingFlags.Instance | BindingFlags.Public), new HarmonyMethod(typeof(Patch_OrderSetVM).GetMethod("Prefix_RefreshOrders", BindingFlags.Static | BindingFlags.Public)), null, null, null);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				Utility.DisplayMessage(ex.ToString());
				MBDebug.Print(ex.ToString(), 0, 12, 17592186044416UL);
				return false;
			}
			return true;
		}

		// Token: 0x060003C4 RID: 964 RVA: 0x00015E30 File Offset: 0x00014030
		public static bool Prefix_RefreshOrders(OrderSetVM __instance, OrderController ____orderController)
		{
			if (Mission.Current.IsNavalBattle)
			{
				return true;
			}
			__instance.Orders.Clear();
			__instance.SoloOrder = null;
			if (__instance.OrderSet == null)
			{
				return false;
			}
			MBReadOnlyList<VisualOrder> orders = __instance.OrderSet.Orders;
			for (int i = 0; i < orders.Count; i++)
			{
				__instance.Orders.Add(new RTSCommandOrderItemVM(____orderController, orders[i]));
			}
			if (!__instance.OrderSet.IsSoloOrder)
			{
				return false;
			}
			__instance.SoloOrder = __instance.Orders[0];
			return false;
		}

		// Token: 0x04000169 RID: 361
		private static bool _patched;
	}
}
