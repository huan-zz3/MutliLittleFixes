using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HarmonyLib
{
	// Token: 0x020000A1 RID: 161
	public static class Transpilers
	{
		// Token: 0x06000334 RID: 820 RVA: 0x000116E2 File Offset: 0x0000F8E2
		public static IEnumerable<CodeInstruction> MethodReplacer(this IEnumerable<CodeInstruction> instructions, MethodBase from, MethodBase to)
		{
			Transpilers.<MethodReplacer>d__0 <MethodReplacer>d__ = new Transpilers.<MethodReplacer>d__0(-2);
			<MethodReplacer>d__.<>3__instructions = instructions;
			<MethodReplacer>d__.<>3__from = from;
			<MethodReplacer>d__.<>3__to = to;
			return <MethodReplacer>d__;
		}

		// Token: 0x06000335 RID: 821 RVA: 0x00011700 File Offset: 0x0000F900
		public static IEnumerable<CodeInstruction> Manipulator(this IEnumerable<CodeInstruction> instructions, Func<CodeInstruction, bool> predicate, Action<CodeInstruction> action)
		{
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			return instructions.Select<CodeInstruction, CodeInstruction>(delegate(CodeInstruction instruction)
			{
				if (predicate(instruction))
				{
					action(instruction);
				}
				return instruction;
			}).AsEnumerable<CodeInstruction>();
		}

		// Token: 0x06000336 RID: 822 RVA: 0x0001175E File Offset: 0x0000F95E
		public static IEnumerable<CodeInstruction> DebugLogger(this IEnumerable<CodeInstruction> instructions, string text)
		{
			Transpilers.<DebugLogger>d__2 <DebugLogger>d__ = new Transpilers.<DebugLogger>d__2(-2);
			<DebugLogger>d__.<>3__instructions = instructions;
			<DebugLogger>d__.<>3__text = text;
			return <DebugLogger>d__;
		}
	}
}
