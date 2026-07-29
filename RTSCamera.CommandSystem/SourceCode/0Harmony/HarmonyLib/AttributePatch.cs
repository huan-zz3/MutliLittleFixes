using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace HarmonyLib
{
	// Token: 0x02000050 RID: 80
	internal class AttributePatch
	{
		// Token: 0x06000197 RID: 407 RVA: 0x0000C158 File Offset: 0x0000A358
		internal static AttributePatch Create(MethodInfo patch)
		{
			if (patch == null)
			{
				throw new NullReferenceException("Patch method cannot be null");
			}
			object[] customAttributes = patch.GetCustomAttributes(true);
			string name = patch.Name;
			HarmonyPatchType? patchType = AttributePatch.GetPatchType(name, customAttributes);
			if (patchType == null)
			{
				return null;
			}
			if (patchType.GetValueOrDefault() != HarmonyPatchType.ReversePatch && !patch.IsStatic)
			{
				throw new ArgumentException("Patch method " + patch.FullDescription() + " must be static");
			}
			IEnumerable<object> enumerable = customAttributes.Where<object>((object attr) => attr.GetType().BaseType.FullName == PatchTools.harmonyAttributeFullName).Select<object, object>(delegate(object attr)
			{
				FieldInfo fieldInfo = AccessTools.Field(attr.GetType(), "info");
				return fieldInfo.GetValue(attr);
			});
			Func<object, HarmonyMethod> func;
			if ((func = AttributePatch.<>O.<0>__MakeDeepCopy) == null)
			{
				func = (AttributePatch.<>O.<0>__MakeDeepCopy = new Func<object, HarmonyMethod>(AccessTools.MakeDeepCopy<HarmonyMethod>));
			}
			List<HarmonyMethod> list = enumerable.Select<object, HarmonyMethod>(func).ToList<HarmonyMethod>();
			HarmonyMethod harmonyMethod = HarmonyMethod.Merge(list);
			harmonyMethod.method = patch;
			return new AttributePatch
			{
				info = harmonyMethod,
				type = patchType
			};
		}

		// Token: 0x06000198 RID: 408 RVA: 0x0000C258 File Offset: 0x0000A458
		private static HarmonyPatchType? GetPatchType(string methodName, object[] allAttributes)
		{
			HashSet<string> hashSet = new HashSet<string>(from attr in allAttributes
				select attr.GetType().FullName into name
				where name.StartsWith("Harmony")
				select name);
			HarmonyPatchType? harmonyPatchType = null;
			foreach (HarmonyPatchType harmonyPatchType2 in AttributePatch.allPatchTypes)
			{
				string text = harmonyPatchType2.ToString();
				if (text == methodName || hashSet.Contains("HarmonyLib.Harmony" + text))
				{
					harmonyPatchType = new HarmonyPatchType?(harmonyPatchType2);
					break;
				}
			}
			return harmonyPatchType;
		}

		// Token: 0x04000119 RID: 281
		private static readonly HarmonyPatchType[] allPatchTypes = new HarmonyPatchType[]
		{
			HarmonyPatchType.Prefix,
			HarmonyPatchType.Postfix,
			HarmonyPatchType.Transpiler,
			HarmonyPatchType.Finalizer,
			HarmonyPatchType.ReversePatch,
			HarmonyPatchType.InnerPrefix,
			HarmonyPatchType.InnerPostfix
		};

		// Token: 0x0400011A RID: 282
		internal HarmonyMethod info;

		// Token: 0x0400011B RID: 283
		internal HarmonyPatchType? type;

		// Token: 0x02000051 RID: 81
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x0400011C RID: 284
			public static Func<object, HarmonyMethod> <0>__MakeDeepCopy;
		}
	}
}
