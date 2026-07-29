using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace HarmonyLib
{
	// Token: 0x02000047 RID: 71
	internal static class PatchArgumentExtensions
	{
		// Token: 0x06000177 RID: 375 RVA: 0x0000BA39 File Offset: 0x00009C39
		private static IEnumerable<HarmonyArgument> AllHarmonyArguments(object[] attributes)
		{
			return attributes.Select<object, HarmonyArgument>(delegate(object attr)
			{
				if (attr.GetType().Name != "HarmonyArgument")
				{
					return null;
				}
				return AccessTools.MakeDeepCopy<HarmonyArgument>(attr);
			}).OfType<HarmonyArgument>();
		}

		// Token: 0x06000178 RID: 376 RVA: 0x0000BA68 File Offset: 0x00009C68
		internal static HarmonyArgument GetArgumentAttribute(this ParameterInfo parameter)
		{
			HarmonyArgument harmonyArgument;
			try
			{
				object[] customAttributes = parameter.GetCustomAttributes(true);
				harmonyArgument = PatchArgumentExtensions.AllHarmonyArguments(customAttributes).FirstOrDefault<HarmonyArgument>();
			}
			catch (NotSupportedException)
			{
				harmonyArgument = null;
			}
			return harmonyArgument;
		}

		// Token: 0x06000179 RID: 377 RVA: 0x0000BAA4 File Offset: 0x00009CA4
		internal static IEnumerable<HarmonyArgument> GetArgumentAttributes(this MethodInfo method)
		{
			IEnumerable<HarmonyArgument> enumerable;
			try
			{
				object[] customAttributes = method.GetCustomAttributes(true);
				enumerable = PatchArgumentExtensions.AllHarmonyArguments(customAttributes);
			}
			catch (NotSupportedException)
			{
				enumerable = Array.Empty<HarmonyArgument>();
			}
			return enumerable;
		}

		// Token: 0x0600017A RID: 378 RVA: 0x0000BADC File Offset: 0x00009CDC
		internal static IEnumerable<HarmonyArgument> GetArgumentAttributes(this Type type)
		{
			IEnumerable<HarmonyArgument> enumerable;
			try
			{
				object[] customAttributes = type.GetCustomAttributes(true);
				enumerable = PatchArgumentExtensions.AllHarmonyArguments(customAttributes);
			}
			catch (NotSupportedException)
			{
				enumerable = Array.Empty<HarmonyArgument>();
			}
			return enumerable;
		}

		// Token: 0x0600017B RID: 379 RVA: 0x0000BB14 File Offset: 0x00009D14
		internal static string GetRealName(this IEnumerable<HarmonyArgument> attributes, string name, string[] originalParameterNames)
		{
			HarmonyArgument harmonyArgument = attributes.FirstOrDefault<HarmonyArgument>((HarmonyArgument p) => p.OriginalName == name);
			if (harmonyArgument == null)
			{
				return null;
			}
			if (!string.IsNullOrEmpty(harmonyArgument.NewName))
			{
				return harmonyArgument.NewName;
			}
			if (originalParameterNames != null && harmonyArgument.Index >= 0 && harmonyArgument.Index < originalParameterNames.Length)
			{
				return originalParameterNames[harmonyArgument.Index];
			}
			return null;
		}

		// Token: 0x0600017C RID: 380 RVA: 0x0000BB7C File Offset: 0x00009D7C
		private static string GetRealParameterName(this MethodInfo method, string[] originalParameterNames, string name)
		{
			if (method == null || method is DynamicMethod)
			{
				return name;
			}
			string text = method.GetArgumentAttributes().GetRealName(name, originalParameterNames);
			if (text != null)
			{
				return text;
			}
			Type declaringType = method.DeclaringType;
			if (declaringType != null)
			{
				text = declaringType.GetArgumentAttributes().GetRealName(name, originalParameterNames);
				if (text != null)
				{
					return text;
				}
			}
			return name;
		}

		// Token: 0x0600017D RID: 381 RVA: 0x0000BBC8 File Offset: 0x00009DC8
		private static string GetRealParameterName(this ParameterInfo parameter, string[] originalParameterNames)
		{
			HarmonyArgument argumentAttribute = parameter.GetArgumentAttribute();
			if (argumentAttribute == null)
			{
				return null;
			}
			if (!string.IsNullOrEmpty(argumentAttribute.OriginalName))
			{
				return argumentAttribute.OriginalName;
			}
			if (argumentAttribute.Index >= 0 && argumentAttribute.Index < originalParameterNames.Length)
			{
				return originalParameterNames[argumentAttribute.Index];
			}
			return null;
		}

		// Token: 0x0600017E RID: 382 RVA: 0x0000BC14 File Offset: 0x00009E14
		internal static int GetArgumentIndex(this MethodInfo patch, string[] originalParameterNames, ParameterInfo patchParam)
		{
			if (patch is DynamicMethod)
			{
				return Array.IndexOf<string>(originalParameterNames, patchParam.Name);
			}
			string text = patchParam.GetRealParameterName(originalParameterNames);
			if (text != null)
			{
				return Array.IndexOf<string>(originalParameterNames, text);
			}
			text = patch.GetRealParameterName(originalParameterNames, patchParam.Name);
			if (text != null)
			{
				return Array.IndexOf<string>(originalParameterNames, text);
			}
			return -1;
		}
	}
}
