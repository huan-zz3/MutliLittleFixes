using System;

namespace HarmonyLib
{
	// Token: 0x0200006A RID: 106
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method)]
	public class HarmonyBefore : HarmonyAttribute
	{
		// Token: 0x06000202 RID: 514 RVA: 0x0000D784 File Offset: 0x0000B984
		public HarmonyBefore(params string[] before)
		{
			this.info.before = before;
		}
	}
}
