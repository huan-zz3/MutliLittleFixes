using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BattlefieldUI.Views;
using HarmonyLib;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;

namespace BattlefieldUI.Patches
{
	// Token: 0x0200000C RID: 12
	[HarmonyPatch]
	internal static class BattlefieldMissionViewPatch
	{
		// Token: 0x060000AE RID: 174 RVA: 0x00003FA8 File Offset: 0x000021A8
		private static MethodBase TargetMethod()
		{
			return AccessTools.Method(typeof(ViewCreatorManager), "CreateDefaultMissionBehaviors", null, null);
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00003FC0 File Offset: 0x000021C0
		private static void Postfix(Mission mission, ref IEnumerable<MissionBehavior> __result)
		{
			List<MissionBehavior> list = ((__result == null) ? new List<MissionBehavior>() : __result.ToList<MissionBehavior>());
			if (list.Any<MissionBehavior>((MissionBehavior behavior) => behavior is BattlefieldHealthBarView))
			{
				__result = list;
				return;
			}
			list.Add(new BattlefieldHealthBarView());
			__result = list;
			Debug.Print("[BattlefieldUI] Mission view injected.", 0, 12, 17592186044416UL);
		}
	}
}
