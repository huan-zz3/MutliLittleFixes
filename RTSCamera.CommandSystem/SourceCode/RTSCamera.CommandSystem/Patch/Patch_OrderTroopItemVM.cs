using System;
using System.Reflection;
using HarmonyLib;
using MissionSharedLibrary.Utilities;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order;

namespace RTSCamera.CommandSystem.Patch
{
	// Token: 0x02000064 RID: 100
	public class Patch_OrderTroopItemVM
	{
		// Token: 0x060003C6 RID: 966 RVA: 0x00015EC8 File Offset: 0x000140C8
		public static bool Patch(Harmony harmony)
		{
			try
			{
				if (Patch_OrderTroopItemVM._patched)
				{
					return false;
				}
				Patch_OrderTroopItemVM._patched = true;
				harmony.Patch(typeof(OrderTroopItemVM).GetMethod("ExecuteAction", BindingFlags.Instance | BindingFlags.Public), new HarmonyMethod(typeof(Patch_OrderTroopItemVM).GetMethod("Prefix_ExecuteAction", BindingFlags.Static | BindingFlags.Public)), null, null, null);
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

		// Token: 0x060003C7 RID: 967 RVA: 0x00015F64 File Offset: 0x00014164
		public static bool Prefix_ExecuteAction(OrderTroopItemVM __instance)
		{
			return __instance.SetSelected != null && __instance.IsSelectable;
		}

		// Token: 0x0400016A RID: 362
		private static bool _patched;
	}
}
