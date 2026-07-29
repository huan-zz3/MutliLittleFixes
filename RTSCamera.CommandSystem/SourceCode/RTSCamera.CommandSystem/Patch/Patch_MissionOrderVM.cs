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
	// Token: 0x02000060 RID: 96
	public class Patch_MissionOrderVM
	{
		// Token: 0x0600034A RID: 842 RVA: 0x0000FBE8 File Offset: 0x0000DDE8
		public static bool Patch(Harmony harmony)
		{
			bool flag;
			try
			{
				if (Patch_MissionOrderVM._patched)
				{
					flag = false;
				}
				else
				{
					Patch_MissionOrderVM._patched = true;
					harmony.Patch(AccessTools.PropertyGetter(typeof(MissionOrderVM), "CursorState"), new HarmonyMethod(typeof(Patch_MissionOrderVM).GetMethod("Prefix_CursorState", BindingFlags.Static | BindingFlags.Public)), null, null, null);
					harmony.Patch(typeof(MissionOrderVM).GetMethod("OnEscape", BindingFlags.Instance | BindingFlags.Public), new HarmonyMethod(typeof(Patch_MissionOrderVM).GetMethod("Prefix_OnEscape", BindingFlags.Static | BindingFlags.Public), -1, null, new string[] { "RTSCameraPatch" }, null), null, null, null);
					harmony.Patch(typeof(MissionOrderVM).GetMethod("PopulateOrderSets", BindingFlags.Instance | BindingFlags.NonPublic), new HarmonyMethod(typeof(Patch_MissionOrderVM).GetMethod("Prefix_PopulateOrderSets", BindingFlags.Static | BindingFlags.Public)), null, null, null);
					harmony.Patch(typeof(MissionOrderVM).GetMethod("RegisterEvents", BindingFlags.Instance | BindingFlags.NonPublic), null, new HarmonyMethod(typeof(Patch_MissionOrderVM).GetMethod("Postfix_RegisterEvents", BindingFlags.Static | BindingFlags.Public)), null, null);
					harmony.Patch(typeof(MissionOrderVM).GetMethod("UnregisterEvents", BindingFlags.Instance | BindingFlags.NonPublic), null, new HarmonyMethod(typeof(Patch_MissionOrderVM).GetMethod("Postfix_UnregisterEvents", BindingFlags.Static | BindingFlags.Public)), null, null);
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

		// Token: 0x0600034B RID: 843 RVA: 0x0000FD94 File Offset: 0x0000DF94
		public static bool Prefix_CursorState(MissionOrderVM __instance, ref MissionOrderVM.CursorStates __result)
		{
			if (RTSCommandVisualOrder.OrderToSelectTarget == SelectTargetMode.LookAtDirection && Patch_OrderTroopPlacer.IsFreeCamera)
			{
				__result = 1;
				Patch_OrderTroopPlacer.SetIsDrawingFacing(true);
				return false;
			}
			return true;
		}

		// Token: 0x0600034C RID: 844 RVA: 0x0000FDB1 File Offset: 0x0000DFB1
		public static bool Prefix_OnEscape(MissionOrderVM __instance)
		{
			if (RTSCommandVisualOrder.OrderToSelectTarget != SelectTargetMode.None)
			{
				RTSCommandVisualOrder.OrderToSelectTarget = SelectTargetMode.None;
				return !Patch_OrderTroopPlacer.IsFreeCamera;
			}
			return true;
		}

		// Token: 0x0600034D RID: 845 RVA: 0x0000FDCC File Offset: 0x0000DFCC
		public static bool Prefix_PopulateOrderSets(MissionOrderVM __instance, bool ____isMultiplayer)
		{
			if (Mission.Current.IsNavalBattle)
			{
				return true;
			}
			__instance.OrderSets.ApplyActionOnAllItems(delegate(OrderSetVM o)
			{
				o.OnFinalize();
			});
			__instance.OrderSets.Clear();
			MBReadOnlyList<VisualOrderSet> orders = VisualOrderFactory.GetOrders();
			for (int i = 0; i < orders.Count; i++)
			{
				__instance.OrderSets.Add(new RTSCommandOrderSetVM(__instance.OrderController, orders[i]));
			}
			Patch_MissionOrderVM._updateOrderShortcuts.Invoke(__instance, Array.Empty<object>());
			if (!____isMultiplayer)
			{
				return false;
			}
			__instance.UpdateCanUseShortcuts(true);
			return false;
		}

		// Token: 0x0600034E RID: 846 RVA: 0x0000FE6E File Offset: 0x0000E06E
		public static void Postfix_RegisterEvents(MissionOrderVM __instance)
		{
			if (!Mission.Current.IsNavalBattle)
			{
				RTSCommandOrderItemVM.RegisterEvent(__instance);
			}
		}

		// Token: 0x0600034F RID: 847 RVA: 0x0000FE82 File Offset: 0x0000E082
		public static void Postfix_UnregisterEvents(MissionOrderVM __instance)
		{
			if (!Mission.Current.IsNavalBattle)
			{
				RTSCommandOrderItemVM.UnregisterEvent(__instance);
			}
		}

		// Token: 0x04000153 RID: 339
		private static bool _patched;

		// Token: 0x04000154 RID: 340
		private static MethodInfo _updateOrderShortcuts = AccessTools.Method(typeof(MissionOrderVM), "UpdateOrderShortcuts", null, null);
	}
}
