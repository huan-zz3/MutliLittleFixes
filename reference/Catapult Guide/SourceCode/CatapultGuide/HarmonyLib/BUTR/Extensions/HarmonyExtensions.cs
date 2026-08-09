using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace HarmonyLib.BUTR.Extensions
{
	// Token: 0x0200000D RID: 13
	[NullableContext(2)]
	[Nullable(0)]
	internal static class HarmonyExtensions
	{
		// Token: 0x060000BC RID: 188 RVA: 0x00008538 File Offset: 0x00006738
		public static bool TryPatch([Nullable(1)] this Harmony harmony, MethodBase original, MethodInfo prefix = null, MethodInfo postfix = null, MethodInfo transpiler = null, MethodInfo finalizer = null)
		{
			bool flag = original == null || (prefix == null && postfix == null && transpiler == null && finalizer == null);
			bool flag2;
			if (flag)
			{
				Trace.TraceError("HarmonyExtensions.TryPatch: 'original' or all methods are null");
				flag2 = false;
			}
			else
			{
				HarmonyMethod harmonyMethod = ((prefix == null) ? null : new HarmonyMethod(prefix));
				HarmonyMethod harmonyMethod2 = ((postfix == null) ? null : new HarmonyMethod(postfix));
				HarmonyMethod harmonyMethod3 = ((transpiler == null) ? null : new HarmonyMethod(transpiler));
				HarmonyMethod harmonyMethod4 = ((finalizer == null) ? null : new HarmonyMethod(finalizer));
				try
				{
					harmony.Patch(original, harmonyMethod, harmonyMethod2, harmonyMethod3, harmonyMethod4);
				}
				catch (Exception ex)
				{
					Trace.TraceError(string.Format("HarmonyExtensions.TryPatch: Exception occurred: {0}, original '{1}'", ex, original));
					return false;
				}
				flag2 = true;
			}
			return flag2;
		}

		// Token: 0x060000BD RID: 189 RVA: 0x000085F0 File Offset: 0x000067F0
		public static ReversePatcher TryCreateReversePatcher([Nullable(1)] this Harmony harmony, MethodBase original, MethodInfo standin)
		{
			bool flag = original == null || standin == null;
			ReversePatcher reversePatcher;
			if (flag)
			{
				Trace.TraceError("HarmonyExtensions.TryCreateReversePatcher: 'original' or 'standin' is null");
				reversePatcher = null;
			}
			else
			{
				try
				{
					reversePatcher = harmony.CreateReversePatcher(original, new HarmonyMethod(standin));
				}
				catch (Exception ex)
				{
					Trace.TraceError(string.Format("HarmonyExtensions.TryCreateReversePatcher: Exception occurred: {0}, original '{1}'", ex, original));
					reversePatcher = null;
				}
			}
			return reversePatcher;
		}

		// Token: 0x060000BE RID: 190 RVA: 0x00008658 File Offset: 0x00006858
		public static bool TryCreateReversePatcher([Nullable(1)] this Harmony harmony, MethodBase original, MethodInfo standin, out ReversePatcher result)
		{
			bool flag = original == null || standin == null;
			bool flag2;
			if (flag)
			{
				Trace.TraceError("HarmonyExtensions.TryCreateReversePatcher: 'original' or 'standin' is null");
				result = null;
				flag2 = false;
			}
			else
			{
				try
				{
					result = harmony.CreateReversePatcher(original, new HarmonyMethod(standin));
					flag2 = true;
				}
				catch (Exception ex)
				{
					Trace.TraceError(string.Format("HarmonyExtensions.TryCreateReversePatcher: Exception occurred: {0}, original '{1}'", ex, original));
					result = null;
					flag2 = false;
				}
			}
			return flag2;
		}
	}
}
