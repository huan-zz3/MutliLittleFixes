using System;

namespace HarmonyLib
{
	// Token: 0x0200006C RID: 108
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method)]
	public class HarmonyDebug : HarmonyAttribute
	{
		// Token: 0x06000204 RID: 516 RVA: 0x0000D7AC File Offset: 0x0000B9AC
		public HarmonyDebug()
		{
			this.info.debug = new bool?(true);
		}
	}
}
