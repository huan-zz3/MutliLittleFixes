using System;
using System.Collections.Generic;

namespace HarmonyLib
{
	// Token: 0x020001C2 RID: 450
	public static class CodeInstructionsExtensions
	{
		// Token: 0x060007DB RID: 2011 RVA: 0x00019D0F File Offset: 0x00017F0F
		public static bool Matches(this IEnumerable<CodeInstruction> instructions, CodeMatch[] matches)
		{
			return new CodeMatcher(instructions, null).MatchStartForward(matches).IsValid;
		}
	}
}
