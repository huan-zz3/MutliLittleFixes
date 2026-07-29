using System;
using System.Reflection;
using HarmonyLib;
using MissionSharedLibrary.Config;
using MissionSharedLibrary.Utilities;
using RTSCamera.CommandSystem.Config;
using RTSCamera.CommandSystem.Logic;
using RTSCamera.CommandSystem.Utilities;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace RTSCamera.CommandSystem.Patch
{
	// Token: 0x0200005B RID: 91
	public class Patch_Formation
	{
		// Token: 0x06000326 RID: 806 RVA: 0x0000DE24 File Offset: 0x0000C024
		public static bool Patch(Harmony harmony)
		{
			try
			{
				if (Patch_Formation._patched)
				{
					return false;
				}
				Patch_Formation._patched = true;
				MethodInfo getMethod = typeof(IFormation).GetProperty("MinimumDistance", BindingFlags.Instance | BindingFlags.Public).GetMethod;
				InterfaceMapping interfaceMap = typeof(Formation).GetInterfaceMap(getMethod.DeclaringType);
				int num = Array.IndexOf<MethodInfo>(interfaceMap.InterfaceMethods, getMethod);
				MethodInfo methodInfo = interfaceMap.TargetMethods[num];
				MethodInfo getMethod2 = typeof(IFormation).GetProperty("MaximumDistance", BindingFlags.Instance | BindingFlags.Public).GetMethod;
				InterfaceMapping interfaceMap2 = typeof(Formation).GetInterfaceMap(getMethod2.DeclaringType);
				int num2 = Array.IndexOf<MethodInfo>(interfaceMap2.InterfaceMethods, getMethod2);
				MethodInfo methodInfo2 = interfaceMap2.TargetMethods[num2];
				MethodInfo getMethod3 = typeof(IFormation).GetProperty("MinimumInterval", BindingFlags.Instance | BindingFlags.Public).GetMethod;
				InterfaceMapping interfaceMap3 = typeof(Formation).GetInterfaceMap(getMethod3.DeclaringType);
				int num3 = Array.IndexOf<MethodInfo>(interfaceMap3.InterfaceMethods, getMethod3);
				MethodInfo methodInfo3 = interfaceMap3.TargetMethods[num3];
				MethodInfo getMethod4 = typeof(IFormation).GetProperty("MaximumInterval", BindingFlags.Instance | BindingFlags.Public).GetMethod;
				InterfaceMapping interfaceMap4 = typeof(Formation).GetInterfaceMap(getMethod4.DeclaringType);
				int num4 = Array.IndexOf<MethodInfo>(interfaceMap4.InterfaceMethods, getMethod4);
				MethodInfo methodInfo4 = interfaceMap4.TargetMethods[num4];
				harmony.Patch(methodInfo, new HarmonyMethod(typeof(Patch_Formation).GetMethod("Prefix_MinimumDistance", BindingFlags.Static | BindingFlags.Public)), null, null, null);
				harmony.Patch(methodInfo2, new HarmonyMethod(typeof(Patch_Formation).GetMethod("Prefix_MaximumDistance", BindingFlags.Static | BindingFlags.Public)), null, null, null);
				harmony.Patch(methodInfo3, new HarmonyMethod(typeof(Patch_Formation).GetMethod("Prefix_MinimumInterval", BindingFlags.Static | BindingFlags.Public)), null, null, null);
				harmony.Patch(methodInfo4, new HarmonyMethod(typeof(Patch_Formation).GetMethod("Prefix_MaximumInterval", BindingFlags.Static | BindingFlags.Public)), null, null, null);
				harmony.Patch(typeof(Formation).GetMethod("CalculateDesiredWidth", BindingFlags.Instance | BindingFlags.NonPublic), new HarmonyMethod(typeof(Patch_Formation).GetMethod("Prefix_CalculateDesiredWidth", BindingFlags.Static | BindingFlags.Public)), null, null, null);
				harmony.Patch(typeof(Formation).GetMethod("SetFormOrder", BindingFlags.Instance | BindingFlags.Public), new HarmonyMethod(typeof(Patch_Formation).GetMethod("Prefix_SetFormOrder", BindingFlags.Static | BindingFlags.Public)), null, null, null);
				harmony.Patch(typeof(Formation).GetMethod("Tick", BindingFlags.Instance | BindingFlags.Public), new HarmonyMethod(typeof(Patch_Formation).GetMethod("Prefix_Tick", BindingFlags.Static | BindingFlags.Public)), null, null, null);
				harmony.Patch(typeof(Formation).GetMethod("ReapplyFormOrder", BindingFlags.Instance | BindingFlags.NonPublic), new HarmonyMethod(typeof(Patch_Formation).GetMethod("Prefix_ReapplyFormOrder", BindingFlags.Static | BindingFlags.Public)), null, null, null);
				harmony.Patch(typeof(Formation).GetProperty("CalculateHasSignificantNumberOfMounted").GetGetMethod(), new HarmonyMethod(typeof(Patch_Formation).GetMethod("Prefix_CalculateHasSignificantNumberOfMounted", BindingFlags.Static | BindingFlags.Public)), null, null, null);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				MissionSharedLibrary.Utilities.Utility.DisplayMessage(ex.ToString());
				MBDebug.Print(ex.ToString(), 0, 12, 17592186044416UL);
				return false;
			}
			return true;
		}

		// Token: 0x06000327 RID: 807 RVA: 0x0000E180 File Offset: 0x0000C380
		public static bool Prefix_MinimumDistance(Formation __instance, ref float __result)
		{
			try
			{
				__result = Formation.GetDefaultMinimumUnitDistance(__instance.CalculateHasSignificantNumberOfMounted && !(__instance.RidingOrder == RidingOrder.RidingOrderDismount));
				return false;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				MissionSharedLibrary.Utilities.Utility.DisplayMessage(ex.ToString());
			}
			return true;
		}

		// Token: 0x06000328 RID: 808 RVA: 0x0000E1DC File Offset: 0x0000C3DC
		public static bool Prefix_MaximumDistance(Formation __instance, ref float __result)
		{
			try
			{
				__result = Formation.GetDefaultUnitDistance(__instance.CalculateHasSignificantNumberOfMounted && !(__instance.RidingOrder == RidingOrder.RidingOrderDismount), ArrangementOrder.GetUnitSpacingOf(__instance.ArrangementOrder.OrderEnum));
				return false;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				MissionSharedLibrary.Utilities.Utility.DisplayMessage(ex.ToString());
			}
			return true;
		}

		// Token: 0x06000329 RID: 809 RVA: 0x0000E248 File Offset: 0x0000C448
		public static bool Prefix_MinimumInterval(Formation __instance, ref float __result)
		{
			try
			{
				__result = Formation.GetDefaultMinimumUnitInterval(__instance.CalculateHasSignificantNumberOfMounted && !(__instance.RidingOrder == RidingOrder.RidingOrderDismount));
				return false;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				MissionSharedLibrary.Utilities.Utility.DisplayMessage(ex.ToString());
			}
			return true;
		}

		// Token: 0x0600032A RID: 810 RVA: 0x0000E2A4 File Offset: 0x0000C4A4
		public static bool Prefix_MaximumInterval(Formation __instance, ref float __result)
		{
			try
			{
				__result = Formation.GetDefaultUnitInterval(__instance.CalculateHasSignificantNumberOfMounted && !(__instance.RidingOrder == RidingOrder.RidingOrderDismount), ArrangementOrder.GetUnitSpacingOf(__instance.ArrangementOrder.OrderEnum));
				return false;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				MissionSharedLibrary.Utilities.Utility.DisplayMessage(ex.ToString());
			}
			return true;
		}

		// Token: 0x0600032B RID: 811 RVA: 0x0000E310 File Offset: 0x0000C510
		public static bool Prefix_Tick(Formation __instance, float dt)
		{
			if (__instance.Team == null)
			{
				return true;
			}
			CommandQueueLogic.UpdateFormation(__instance);
			return true;
		}

		// Token: 0x0600032C RID: 812 RVA: 0x0000E324 File Offset: 0x0000C524
		public static bool Prefix_ReapplyFormOrder(Formation __instance)
		{
			if (Patch_Formation.ReapplyFormOrderExecutiionCount < 3)
			{
				Patch_Formation.ReapplyFormOrderExecutiionCount++;
				bool flag;
				try
				{
					FormOrder formOrder = __instance.FormOrder;
					if (__instance.FormOrder.OrderEnum == 3 && __instance.ArrangementOrder.OrderEnum != null && __instance.ArrangementOrder.OrderEnum != 7)
					{
						formOrder.CustomFlankWidth = __instance.Arrangement.FlankWidth;
					}
					__instance.SetFormOrder(formOrder, false);
					flag = false;
				}
				finally
				{
					Patch_Formation.ReapplyFormOrderExecutiionCount--;
				}
				return flag;
			}
			string text = string.Format("RTS Command Warning: Detected that ReapplyFormOrder has been recursively call for 3 times. Skip execution to avoid issue. The current arrangement order is {0}, UnitSpacing = {1}, FlankWith = {2}, UnitCount = {3}", new object[]
			{
				__instance.ArrangementOrder.OrderType.ToString(),
				__instance.UnitSpacing,
				__instance.Arrangement.FlankWidth,
				__instance.Arrangement.UnitCount
			});
			MissionSharedLibrary.Utilities.Utility.DisplayMessage(text);
			Debug.Print(text, 0, 12, 17592186044416UL);
			return false;
		}

		// Token: 0x0600032D RID: 813 RVA: 0x0000E430 File Offset: 0x0000C630
		public static bool Prefix_CalculateDesiredWidth(Formation __instance, ref float __result, int ____desiredFileCount)
		{
			float num = RTSCamera.CommandSystem.Utilities.Utility.GetFlankWidthFromFileCount(__instance, ____desiredFileCount, __instance.UnitSpacing);
			if (__instance.ArrangementOrder.OrderEnum == null)
			{
				num = RTSCamera.CommandSystem.Utilities.Utility.ConvertFromFlankWidthToWidthOfCircularFormation(__instance, __instance.UnitSpacing, num);
			}
			else if (__instance.ArrangementOrder.OrderEnum == 7)
			{
				if (MissionConfigBase<CommandSystemConfig>.Get().HollowSquare)
				{
					num = RTSCamera.CommandSystem.Utilities.Utility.ConvertFromFlankWidthToWidthOfSquareFormation(__instance, __instance.UnitSpacing, num);
				}
				else
				{
					num = MathF.Min(RTSCamera.CommandSystem.Utilities.Utility.GetMinimumWidthOfSquareFormation(__instance), num);
				}
			}
			__result = MathF.Clamp(num, RTSCamera.CommandSystem.Utilities.Utility.GetFormationMinimumWidthOfArrangementOrder(__instance, __instance.ArrangementOrder.OrderEnum, __instance.UnitSpacing), RTSCamera.CommandSystem.Utilities.Utility.GetFormationMaximumWidthOfArrangementOrder(__instance, __instance.ArrangementOrder.OrderEnum));
			return false;
		}

		// Token: 0x0600032E RID: 814 RVA: 0x0000E4D0 File Offset: 0x0000C6D0
		public static bool Prefix_SetFormOrder(Formation __instance, FormOrder order, bool updateDesiredFileCount, ref int ____desiredFileCount)
		{
			if (order.OrderEnum == 3 && updateDesiredFileCount)
			{
				float num = order.CustomFlankWidth;
				if (__instance.ArrangementOrder.OrderEnum == null)
				{
					num = RTSCamera.CommandSystem.Utilities.Utility.ConvertFromWidthToFlankWidthOfCircularFormation(__instance, __instance.UnitSpacing, num);
				}
				else if (__instance.ArrangementOrder.OrderEnum == 7)
				{
					num = RTSCamera.CommandSystem.Utilities.Utility.ConvertFromWidthToFlankWidthOfSquareFormation(__instance, __instance.UnitSpacing, num);
				}
				____desiredFileCount = RTSCamera.CommandSystem.Utilities.Utility.GetFileCountFromWidth(__instance, num, __instance.UnitSpacing);
			}
			if (Patch_Formation._FormOrder == null)
			{
				Patch_Formation._FormOrder = AccessTools.Property(typeof(Formation), "FormOrder");
			}
			Patch_Formation._FormOrder.SetValue(__instance, order);
			__instance.FormOrder.OnApply(__instance);
			__instance.QuerySystem.Expire();
			return false;
		}

		// Token: 0x0600032F RID: 815 RVA: 0x0000E586 File Offset: 0x0000C786
		public static bool Prefix_CalculateHasSignificantNumberOfMounted(Formation __instance, bool? ____overridenHasAnyMountedUnit, ref bool __result)
		{
			if (____overridenHasAnyMountedUnit == null)
			{
				return (double)__instance.QuerySystem.CavalryUnitRatio + (double)__instance.QuerySystem.RangedCavalryUnitRatio >= (double)MissionConfigBase<CommandSystemConfig>.Get().MountedUnitsIntervalThreshold;
			}
			return ____overridenHasAnyMountedUnit.Value;
		}

		// Token: 0x04000145 RID: 325
		private static bool _patched;

		// Token: 0x04000146 RID: 326
		public static int ReapplyFormOrderExecutiionCount;

		// Token: 0x04000147 RID: 327
		private static PropertyInfo _FormOrder;
	}
}
