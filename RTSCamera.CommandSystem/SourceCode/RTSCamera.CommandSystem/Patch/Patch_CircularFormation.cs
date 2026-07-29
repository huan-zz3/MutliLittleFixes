using System;
using System.Reflection;
using HarmonyLib;
using MissionSharedLibrary.Config;
using MissionSharedLibrary.Utilities;
using RTSCamera.CommandSystem.Config;
using RTSCamera.CommandSystem.Utilities;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;

namespace RTSCamera.CommandSystem.Patch
{
	// Token: 0x02000059 RID: 89
	public class Patch_CircularFormation
	{
		// Token: 0x0600031B RID: 795 RVA: 0x0000DA54 File Offset: 0x0000BC54
		public static bool Patch(Harmony harmony)
		{
			try
			{
				if (Patch_CircularFormation._patched)
				{
					return false;
				}
				Patch_CircularFormation._patched = true;
				harmony.Patch(typeof(CircularFormation).GetMethod("GetCircumferenceAux", BindingFlags.Instance | BindingFlags.NonPublic), new HarmonyMethod(typeof(Patch_CircularFormation).GetMethod("Prefix_GetCircuferenceAux", BindingFlags.Static | BindingFlags.Public)), null, null, null);
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

		// Token: 0x0600031C RID: 796 RVA: 0x0000DAF0 File Offset: 0x0000BCF0
		public static bool Prefix_GetCircuferenceAux(int unitCount, int rankCount, float radialInterval, float distanceInterval, ref float __result)
		{
			if (MissionConfigBase<CommandSystemConfig>.Get().CircleFormationUnitSpacingPreference == CircleFormationUnitSpacingPreference.Loose)
			{
				return true;
			}
			__result = RTSCamera.CommandSystem.Utilities.Utility.GetCircumferenceAuxOfCircularFormation(unitCount, rankCount, radialInterval, distanceInterval);
			return false;
		}

		// Token: 0x04000142 RID: 322
		private static bool _patched;

		// Token: 0x04000143 RID: 323
		private static FieldInfo Owner = AccessTools.Field(typeof(LineFormation), "owner");
	}
}
