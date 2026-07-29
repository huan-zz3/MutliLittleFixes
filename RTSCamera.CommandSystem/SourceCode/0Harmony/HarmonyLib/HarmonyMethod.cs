using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace HarmonyLib
{
	// Token: 0x02000084 RID: 132
	public class HarmonyMethod
	{
		// Token: 0x0600027B RID: 635 RVA: 0x0000ECBE File Offset: 0x0000CEBE
		public HarmonyMethod()
		{
		}

		// Token: 0x0600027C RID: 636 RVA: 0x0000ECD0 File Offset: 0x0000CED0
		private void ImportMethod(MethodInfo theMethod)
		{
			this.method = theMethod;
			if (this.method != null)
			{
				List<HarmonyMethod> fromMethod = HarmonyMethodExtensions.GetFromMethod(this.method);
				if (fromMethod != null)
				{
					HarmonyMethod.Merge(fromMethod).CopyTo(this);
				}
			}
		}

		// Token: 0x0600027D RID: 637 RVA: 0x0000ED07 File Offset: 0x0000CF07
		public HarmonyMethod(MethodInfo method)
		{
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			this.ImportMethod(method);
		}

		// Token: 0x0600027E RID: 638 RVA: 0x0000ED2B File Offset: 0x0000CF2B
		public HarmonyMethod(Delegate @delegate)
			: this(@delegate.Method)
		{
		}

		// Token: 0x0600027F RID: 639 RVA: 0x0000ED3C File Offset: 0x0000CF3C
		public HarmonyMethod(MethodInfo method, int priority = -1, string[] before = null, string[] after = null, bool? debug = null)
		{
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			this.ImportMethod(method);
			this.priority = priority;
			this.before = before;
			this.after = after;
			this.debug = debug;
		}

		// Token: 0x06000280 RID: 640 RVA: 0x0000ED89 File Offset: 0x0000CF89
		public HarmonyMethod(Delegate @delegate, int priority = -1, string[] before = null, string[] after = null, bool? debug = null)
			: this(@delegate.Method, priority, before, after, debug)
		{
		}

		// Token: 0x06000281 RID: 641 RVA: 0x0000EDA0 File Offset: 0x0000CFA0
		public HarmonyMethod(Type methodType, string methodName, Type[] argumentTypes = null)
		{
			MethodInfo methodInfo = AccessTools.Method(methodType, methodName, argumentTypes, null);
			if (methodInfo == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(58, 3);
				defaultInterpolatedStringHandler.AppendLiteral("Cannot not find method for type ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(methodType);
				defaultInterpolatedStringHandler.AppendLiteral(" and name ");
				defaultInterpolatedStringHandler.AppendFormatted(methodName);
				defaultInterpolatedStringHandler.AppendLiteral(" and parameters ");
				defaultInterpolatedStringHandler.AppendFormatted((argumentTypes != null) ? argumentTypes.Description() : null);
				throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			this.ImportMethod(methodInfo);
		}

		// Token: 0x06000282 RID: 642 RVA: 0x0000EE2C File Offset: 0x0000D02C
		public static List<string> HarmonyFields()
		{
			return (from s in AccessTools.GetFieldNames(typeof(HarmonyMethod))
				where s != "method"
				select s).ToList<string>();
		}

		// Token: 0x06000283 RID: 643 RVA: 0x0000EE68 File Offset: 0x0000D068
		public static HarmonyMethod Merge(List<HarmonyMethod> attributes)
		{
			HarmonyMethod harmonyMethod = new HarmonyMethod();
			if (attributes == null || attributes.Count == 0)
			{
				return harmonyMethod;
			}
			Traverse resultTrv = Traverse.Create(harmonyMethod);
			attributes.ForEach(delegate(HarmonyMethod attribute)
			{
				Traverse trv = Traverse.Create(attribute);
				HarmonyMethod.HarmonyFields().ForEach(delegate(string f)
				{
					object value = trv.Field(f).GetValue();
					if (value != null && (f != "priority" || (int)value != -1))
					{
						HarmonyMethodExtensions.SetValue(resultTrv, f, value);
					}
				});
			});
			return harmonyMethod;
		}

		// Token: 0x06000284 RID: 644 RVA: 0x0000EEB0 File Offset: 0x0000D0B0
		public override string ToString()
		{
			string result = "";
			Traverse trv = Traverse.Create(this);
			HarmonyMethod.HarmonyFields().ForEach(delegate(string f)
			{
				if (result.Length > 0)
				{
					result += ", ";
				}
				string result2 = result;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
				defaultInterpolatedStringHandler.AppendFormatted(f);
				defaultInterpolatedStringHandler.AppendLiteral("=");
				defaultInterpolatedStringHandler.AppendFormatted<object>(trv.Field(f).GetValue());
				result = result2 + defaultInterpolatedStringHandler.ToStringAndClear();
			});
			return "HarmonyMethod[" + result + "]";
		}

		// Token: 0x06000285 RID: 645 RVA: 0x0000EF08 File Offset: 0x0000D108
		internal string Description()
		{
			string text = ((this.declaringType != null) ? this.declaringType.FullName : "undefined");
			string text2 = this.methodName ?? "undefined";
			string text3 = ((this.methodType != null) ? this.methodType.Value.ToString() : "undefined");
			string text4 = ((this.argumentTypes != null) ? this.argumentTypes.Description() : "undefined");
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(35, 4);
			defaultInterpolatedStringHandler.AppendLiteral("(class=");
			defaultInterpolatedStringHandler.AppendFormatted(text);
			defaultInterpolatedStringHandler.AppendLiteral(", methodname=");
			defaultInterpolatedStringHandler.AppendFormatted(text2);
			defaultInterpolatedStringHandler.AppendLiteral(", type=");
			defaultInterpolatedStringHandler.AppendFormatted(text3);
			defaultInterpolatedStringHandler.AppendLiteral(", args=");
			defaultInterpolatedStringHandler.AppendFormatted(text4);
			defaultInterpolatedStringHandler.AppendLiteral(")");
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}

		// Token: 0x06000286 RID: 646 RVA: 0x0000EFF7 File Offset: 0x0000D1F7
		public static implicit operator HarmonyMethod(MethodInfo method)
		{
			return new HarmonyMethod(method);
		}

		// Token: 0x06000287 RID: 647 RVA: 0x0000EFFF File Offset: 0x0000D1FF
		public static implicit operator HarmonyMethod(Delegate @delegate)
		{
			return new HarmonyMethod(@delegate);
		}

		// Token: 0x040001A6 RID: 422
		public MethodInfo method;

		// Token: 0x040001A7 RID: 423
		public string category;

		// Token: 0x040001A8 RID: 424
		public Type declaringType;

		// Token: 0x040001A9 RID: 425
		public string methodName;

		// Token: 0x040001AA RID: 426
		public MethodType? methodType;

		// Token: 0x040001AB RID: 427
		public Type[] argumentTypes;

		// Token: 0x040001AC RID: 428
		public int priority = -1;

		// Token: 0x040001AD RID: 429
		public string[] before;

		// Token: 0x040001AE RID: 430
		public string[] after;

		// Token: 0x040001AF RID: 431
		public HarmonyReversePatchType? reversePatchType;

		// Token: 0x040001B0 RID: 432
		public bool? debug;

		// Token: 0x040001B1 RID: 433
		public bool nonVirtualDelegate;
	}
}
