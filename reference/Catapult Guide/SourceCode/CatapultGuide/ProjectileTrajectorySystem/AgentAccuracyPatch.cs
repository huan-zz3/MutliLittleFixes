using System;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TaleWorlds.MountAndBlade;

namespace ProjectileTrajectorySystem
{
	// Token: 0x02000006 RID: 6
	[HarmonyPatch(typeof(Agent), "UpdateDrivenProperties")]
	public static class AgentAccuracyPatch
	{
		// Token: 0x06000008 RID: 8 RVA: 0x000020F0 File Offset: 0x000002F0
		[NullableContext(1)]
		public static void Postfix(Agent __instance)
		{
			bool flag = __instance.IsMainAgent && __instance.AgentDrivenProperties != null;
			if (flag)
			{
				__instance.AgentDrivenProperties.WeaponInaccuracy = 0f;
				__instance.AgentDrivenProperties.WeaponBestAccuracyWaitTime = 0f;
			}
		}
	}
}
