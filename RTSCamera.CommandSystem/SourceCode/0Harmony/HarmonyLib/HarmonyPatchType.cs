using System;

namespace HarmonyLib
{
	// Token: 0x02000060 RID: 96
	public enum HarmonyPatchType
	{
		// Token: 0x0400016C RID: 364
		All,
		// Token: 0x0400016D RID: 365
		Prefix,
		// Token: 0x0400016E RID: 366
		Postfix,
		// Token: 0x0400016F RID: 367
		Transpiler,
		// Token: 0x04000170 RID: 368
		Finalizer,
		// Token: 0x04000171 RID: 369
		ReversePatch,
		// Token: 0x04000172 RID: 370
		InnerPrefix,
		// Token: 0x04000173 RID: 371
		InnerPostfix
	}
}
