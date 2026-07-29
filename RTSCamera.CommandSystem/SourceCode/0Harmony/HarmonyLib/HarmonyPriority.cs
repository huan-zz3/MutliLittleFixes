using System;

namespace HarmonyLib
{
	// Token: 0x02000069 RID: 105
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method)]
	public class HarmonyPriority : HarmonyAttribute
	{
		// Token: 0x06000201 RID: 513 RVA: 0x0000D770 File Offset: 0x0000B970
		public HarmonyPriority(int priority)
		{
			this.info.priority = priority;
		}
	}
}
