using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace HarmonyLib
{
	// Token: 0x02000089 RID: 137
	public static class HarmonyMethodExtensions
	{
		// Token: 0x06000291 RID: 657 RVA: 0x0000F128 File Offset: 0x0000D328
		internal static void SetValue(Traverse trv, string name, object val)
		{
			if (val == null)
			{
				return;
			}
			Traverse traverse = trv.Field(name);
			if (name == "methodType" || name == "reversePatchType")
			{
				Type underlyingType = Nullable.GetUnderlyingType(traverse.GetValueType());
				val = Enum.ToObject(underlyingType, (int)val);
			}
			traverse.SetValue(val);
		}

		// Token: 0x06000292 RID: 658 RVA: 0x0000F180 File Offset: 0x0000D380
		public static void CopyTo(this HarmonyMethod from, HarmonyMethod to)
		{
			if (to == null)
			{
				return;
			}
			Traverse fromTrv = Traverse.Create(from);
			Traverse toTrv = Traverse.Create(to);
			HarmonyMethod.HarmonyFields().ForEach(delegate(string f)
			{
				object value = fromTrv.Field(f).GetValue();
				if (value != null)
				{
					HarmonyMethodExtensions.SetValue(toTrv, f, value);
				}
			});
		}

		// Token: 0x06000293 RID: 659 RVA: 0x0000F1C8 File Offset: 0x0000D3C8
		public static HarmonyMethod Clone(this HarmonyMethod original)
		{
			HarmonyMethod harmonyMethod = new HarmonyMethod();
			original.CopyTo(harmonyMethod);
			return harmonyMethod;
		}

		// Token: 0x06000294 RID: 660 RVA: 0x0000F1E4 File Offset: 0x0000D3E4
		public static HarmonyMethod Merge(this HarmonyMethod master, HarmonyMethod detail)
		{
			if (detail == null)
			{
				return master;
			}
			HarmonyMethod harmonyMethod = new HarmonyMethod();
			Traverse resultTrv = Traverse.Create(harmonyMethod);
			Traverse masterTrv = Traverse.Create(master);
			Traverse detailTrv = Traverse.Create(detail);
			HarmonyMethod.HarmonyFields().ForEach(delegate(string f)
			{
				object value = masterTrv.Field(f).GetValue();
				object value2 = detailTrv.Field(f).GetValue();
				if (f != "priority")
				{
					HarmonyMethodExtensions.SetValue(resultTrv, f, value2 ?? value);
					return;
				}
				int num = (int)value;
				int num2 = (int)value2;
				int num3 = Math.Max(num, num2);
				if (num == -1 && num2 != -1)
				{
					num3 = num2;
				}
				if (num != -1 && num2 == -1)
				{
					num3 = num;
				}
				HarmonyMethodExtensions.SetValue(resultTrv, f, num3);
			});
			return harmonyMethod;
		}

		// Token: 0x06000295 RID: 661 RVA: 0x0000F240 File Offset: 0x0000D440
		private static HarmonyMethod GetHarmonyMethodInfo(object attribute)
		{
			FieldInfo field = attribute.GetType().GetField("info", AccessTools.all);
			if (field == null)
			{
				return null;
			}
			if (field.FieldType.FullName != PatchTools.harmonyMethodFullName)
			{
				return null;
			}
			object value = field.GetValue(attribute);
			return AccessTools.MakeDeepCopy<HarmonyMethod>(value);
		}

		// Token: 0x06000296 RID: 662 RVA: 0x0000F290 File Offset: 0x0000D490
		public static List<HarmonyMethod> GetFromType(Type type)
		{
			IEnumerable<object> customAttributes = type.GetCustomAttributes(true);
			Func<object, HarmonyMethod> func;
			if ((func = HarmonyMethodExtensions.<>O.<0>__GetHarmonyMethodInfo) == null)
			{
				func = (HarmonyMethodExtensions.<>O.<0>__GetHarmonyMethodInfo = new Func<object, HarmonyMethod>(HarmonyMethodExtensions.GetHarmonyMethodInfo));
			}
			return (from info in customAttributes.Select<object, HarmonyMethod>(func)
				where info != null
				select info).ToList<HarmonyMethod>();
		}

		// Token: 0x06000297 RID: 663 RVA: 0x0000F2ED File Offset: 0x0000D4ED
		public static HarmonyMethod GetMergedFromType(Type type)
		{
			return HarmonyMethod.Merge(HarmonyMethodExtensions.GetFromType(type));
		}

		// Token: 0x06000298 RID: 664 RVA: 0x0000F2FC File Offset: 0x0000D4FC
		public static List<HarmonyMethod> GetFromMethod(MethodBase method)
		{
			IEnumerable<object> customAttributes = method.GetCustomAttributes(true);
			Func<object, HarmonyMethod> func;
			if ((func = HarmonyMethodExtensions.<>O.<0>__GetHarmonyMethodInfo) == null)
			{
				func = (HarmonyMethodExtensions.<>O.<0>__GetHarmonyMethodInfo = new Func<object, HarmonyMethod>(HarmonyMethodExtensions.GetHarmonyMethodInfo));
			}
			return (from info in customAttributes.Select<object, HarmonyMethod>(func)
				where info != null
				select info).ToList<HarmonyMethod>();
		}

		// Token: 0x06000299 RID: 665 RVA: 0x0000F359 File Offset: 0x0000D559
		public static HarmonyMethod GetMergedFromMethod(MethodBase method)
		{
			return HarmonyMethod.Merge(HarmonyMethodExtensions.GetFromMethod(method));
		}

		// Token: 0x0200008A RID: 138
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x040001B9 RID: 441
			public static Func<object, HarmonyMethod> <0>__GetHarmonyMethodInfo;
		}
	}
}
