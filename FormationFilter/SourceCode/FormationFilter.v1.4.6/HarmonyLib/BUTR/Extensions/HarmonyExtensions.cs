using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace HarmonyLib.BUTR.Extensions
{
	// Token: 0x02000027 RID: 39
	[NullableContext(2)]
	[Nullable(0)]
	internal static class HarmonyExtensions
	{
		// Token: 0x060001AD RID: 429 RVA: 0x0000B73C File Offset: 0x0000993C
		public static bool TryPatch([Nullable(1)] this Harmony harmony, MethodBase original, MethodInfo prefix = null, MethodInfo postfix = null, MethodInfo transpiler = null, MethodInfo finalizer = null)
		{
			if (original == null || (prefix == null && postfix == null && transpiler == null && finalizer == null))
			{
				Trace.TraceError("HarmonyExtensions.TryPatch: 'original' or all methods are null");
				return false;
			}
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
			return true;
		}

		// Token: 0x060001AE RID: 430 RVA: 0x0000B7DC File Offset: 0x000099DC
		public static ReversePatcher TryCreateReversePatcher([Nullable(1)] this Harmony harmony, MethodBase original, MethodInfo standin)
		{
			if (original == null || standin == null)
			{
				Trace.TraceError("HarmonyExtensions.TryCreateReversePatcher: 'original' or 'standin' is null");
				return null;
			}
			ReversePatcher reversePatcher;
			try
			{
				reversePatcher = harmony.CreateReversePatcher(original, new HarmonyMethod(standin));
			}
			catch (Exception ex)
			{
				Trace.TraceError(string.Format("HarmonyExtensions.TryCreateReversePatcher: Exception occurred: {0}, original '{1}'", ex, original));
				reversePatcher = null;
			}
			return reversePatcher;
		}

		// Token: 0x060001AF RID: 431 RVA: 0x0000B834 File Offset: 0x00009A34
		public static bool TryCreateReversePatcher([Nullable(1)] this Harmony harmony, MethodBase original, MethodInfo standin, out ReversePatcher result)
		{
			if (original == null || standin == null)
			{
				Trace.TraceError("HarmonyExtensions.TryCreateReversePatcher: 'original' or 'standin' is null");
				result = null;
				return false;
			}
			bool flag;
			try
			{
				result = harmony.CreateReversePatcher(original, new HarmonyMethod(standin));
				flag = true;
			}
			catch (Exception ex)
			{
				Trace.TraceError(string.Format("HarmonyExtensions.TryCreateReversePatcher: Exception occurred: {0}, original '{1}'", ex, original));
				result = null;
				flag = false;
			}
			return flag;
		}
	}
}
