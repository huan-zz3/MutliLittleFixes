using System;

namespace HarmonyLib
{
	// Token: 0x0200006B RID: 107
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method)]
	public class HarmonyAfter : HarmonyAttribute
	{
		// Token: 0x06000203 RID: 515 RVA: 0x0000D798 File Offset: 0x0000B998
		public HarmonyAfter(params string[] after)
		{
			this.info.after = after;
		}
	}
}
