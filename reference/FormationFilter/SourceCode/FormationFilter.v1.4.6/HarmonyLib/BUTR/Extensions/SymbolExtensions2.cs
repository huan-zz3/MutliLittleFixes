using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace HarmonyLib.BUTR.Extensions
{
	// Token: 0x02000028 RID: 40
	[NullableContext(2)]
	[Nullable(0)]
	internal static class SymbolExtensions2
	{
		// Token: 0x060001B0 RID: 432 RVA: 0x0000B894 File Offset: 0x00009A94
		public static ConstructorInfo GetConstructorInfo<TResult>([Nullable(1)] Expression<Func<TResult>> expression)
		{
			if (expression != null)
			{
				return SymbolExtensions2.GetConstructorInfo(expression);
			}
			return null;
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x0000B8B0 File Offset: 0x00009AB0
		public static ConstructorInfo GetConstructorInfo<T1, TResult>([Nullable(1)] Expression<Func<T1, TResult>> expression)
		{
			if (expression != null)
			{
				return SymbolExtensions2.GetConstructorInfo(expression);
			}
			return null;
		}

		// Token: 0x060001B2 RID: 434 RVA: 0x0000B8CC File Offset: 0x00009ACC
		public static ConstructorInfo GetConstructorInfo<T1, T2, TResult>([Nullable(1)] Expression<Func<T1, T2, TResult>> expression)
		{
			if (expression != null)
			{
				return SymbolExtensions2.GetConstructorInfo(expression);
			}
			return null;
		}

		// Token: 0x060001B3 RID: 435 RVA: 0x0000B8E8 File Offset: 0x00009AE8
		public static ConstructorInfo GetConstructorInfo<T1, T2, T3, TResult>([Nullable(1)] Expression<Func<T1, T2, T3, TResult>> expression)
		{
			if (expression != null)
			{
				return SymbolExtensions2.GetConstructorInfo(expression);
			}
			return null;
		}

		// Token: 0x060001B4 RID: 436 RVA: 0x0000B904 File Offset: 0x00009B04
		public static ConstructorInfo GetConstructorInfo<T1, T2, T3, T4, TResult>([Nullable(1)] Expression<Func<T1, T2, T3, T4, TResult>> expression)
		{
			if (expression != null)
			{
				return SymbolExtensions2.GetConstructorInfo(expression);
			}
			return null;
		}

		// Token: 0x060001B5 RID: 437 RVA: 0x0000B920 File Offset: 0x00009B20
		public static ConstructorInfo GetConstructorInfo<T1, T2, T3, T4, T5, TResult>([Nullable(1)] Expression<Func<T1, T2, T3, T4, T5, TResult>> expression)
		{
			if (expression != null)
			{
				return SymbolExtensions2.GetConstructorInfo(expression);
			}
			return null;
		}

		// Token: 0x060001B6 RID: 438 RVA: 0x0000B93C File Offset: 0x00009B3C
		public static ConstructorInfo GetConstructorInfo<T1, T2, T3, T4, T5, T6, TResult>([Nullable(1)] Expression<Func<T1, T2, T3, T4, T5, T6, TResult>> expression)
		{
			if (expression != null)
			{
				return SymbolExtensions2.GetConstructorInfo(expression);
			}
			return null;
		}

		// Token: 0x060001B7 RID: 439 RVA: 0x0000B958 File Offset: 0x00009B58
		public static ConstructorInfo GetConstructorInfo<T1, T2, T3, T4, T5, T6, T7, TResult>([Nullable(1)] Expression<Func<T1, T2, T3, T4, T5, T6, T7, TResult>> expression)
		{
			if (expression != null)
			{
				return SymbolExtensions2.GetConstructorInfo(expression);
			}
			return null;
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x0000B974 File Offset: 0x00009B74
		public static ConstructorInfo GetConstructorInfo<T1, T2, T3, T4, T5, T6, T7, T8, TResult>([Nullable(1)] Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>> expression)
		{
			if (expression != null)
			{
				return SymbolExtensions2.GetConstructorInfo(expression);
			}
			return null;
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x0000B990 File Offset: 0x00009B90
		public static ConstructorInfo GetConstructorInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>([Nullable(1)] Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression)
		{
			if (expression != null)
			{
				return SymbolExtensions2.GetConstructorInfo(expression);
			}
			return null;
		}

		// Token: 0x060001BA RID: 442 RVA: 0x0000B9AC File Offset: 0x00009BAC
		[NullableContext(1)]
		[return: Nullable(2)]
		public static ConstructorInfo GetConstructorInfo(LambdaExpression expression)
		{
			NewExpression newExpression = ((expression != null) ? expression.Body : null) as NewExpression;
			if (newExpression != null && newExpression.Constructor != null)
			{
				return newExpression.Constructor;
			}
			return null;
		}

		// Token: 0x060001BB RID: 443 RVA: 0x0000B9E0 File Offset: 0x00009BE0
		public static FieldInfo GetFieldInfo<T>([Nullable(1)] Expression<Func<T>> expression)
		{
			if (expression != null)
			{
				return SymbolExtensions2.GetFieldInfo(expression);
			}
			return null;
		}

		// Token: 0x060001BC RID: 444 RVA: 0x0000B9FC File Offset: 0x00009BFC
		public static FieldInfo GetFieldInfo<T, TResult>([Nullable(1)] Expression<Func<T, TResult>> expression)
		{
			if (expression != null)
			{
				return SymbolExtensions2.GetFieldInfo(expression);
			}
			return null;
		}

		// Token: 0x060001BD RID: 445 RVA: 0x0000BA18 File Offset: 0x00009C18
		[NullableContext(1)]
		[return: Nullable(2)]
		public static FieldInfo GetFieldInfo(LambdaExpression expression)
		{
			MemberExpression memberExpression = ((expression != null) ? expression.Body : null) as MemberExpression;
			if (memberExpression != null)
			{
				FieldInfo fieldInfo = memberExpression.Member as FieldInfo;
				if (fieldInfo != null)
				{
					return fieldInfo;
				}
			}
			return null;
		}

		// Token: 0x060001BE RID: 446 RVA: 0x0000BA4C File Offset: 0x00009C4C
		[NullableContext(1)]
		[return: Nullable(new byte[] { 2, 1, 1 })]
		public static AccessTools.FieldRef<object, TField> GetFieldRefAccess<[Nullable(2)] TField>(Expression<Func<TField>> expression)
		{
			if (expression != null)
			{
				return SymbolExtensions2.GetFieldRefAccess<TField>(expression);
			}
			return null;
		}

		// Token: 0x060001BF RID: 447 RVA: 0x0000BA68 File Offset: 0x00009C68
		[NullableContext(1)]
		[return: Nullable(new byte[] { 2, 1, 1 })]
		public static AccessTools.FieldRef<object, TField> GetFieldRefAccess<[Nullable(2)] TField>(LambdaExpression expression)
		{
			MemberExpression memberExpression = ((expression != null) ? expression.Body : null) as MemberExpression;
			if (memberExpression != null)
			{
				FieldInfo fieldInfo = memberExpression.Member as FieldInfo;
				if (fieldInfo != null)
				{
					if (!(fieldInfo == null))
					{
						return AccessTools2.FieldRefAccess<object, TField>(fieldInfo, true);
					}
					return null;
				}
			}
			return null;
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x0000BAB0 File Offset: 0x00009CB0
		[NullableContext(1)]
		[return: Nullable(new byte[] { 2, 1, 1 })]
		public static AccessTools.FieldRef<TObject, TField> GetFieldRefAccess<TObject, [Nullable(2)] TField>(Expression<Func<TObject, TField>> expression) where TObject : class
		{
			if (expression != null)
			{
				return SymbolExtensions2.GetFieldRefAccess<TObject, TField>(expression);
			}
			return null;
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x0000BACC File Offset: 0x00009CCC
		[NullableContext(1)]
		[return: Nullable(new byte[] { 2, 1, 1 })]
		public static AccessTools.FieldRef<TObject, TField> GetFieldRefAccess<TObject, [Nullable(2)] TField>(LambdaExpression expression) where TObject : class
		{
			MemberExpression memberExpression = ((expression != null) ? expression.Body : null) as MemberExpression;
			if (memberExpression != null)
			{
				FieldInfo fieldInfo = memberExpression.Member as FieldInfo;
				if (fieldInfo != null)
				{
					if (!(fieldInfo == null))
					{
						return AccessTools2.FieldRefAccess<TObject, TField>(fieldInfo, true);
					}
					return null;
				}
			}
			return null;
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x0000BB14 File Offset: 0x00009D14
		[NullableContext(1)]
		[return: Nullable(2)]
		public static MethodInfo GetMethodInfo(Expression<Action> expression)
		{
			if (expression != null)
			{
				return SymbolExtensions2.GetMethodInfo(expression);
			}
			return null;
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x0000BB30 File Offset: 0x00009D30
		public static MethodInfo GetMethodInfo<T1>([Nullable(1)] Expression<Action<T1>> expression)
		{
			if (expression != null)
			{
				return SymbolExtensions2.GetMethodInfo(expression);
			}
			return null;
		}

		// Token: 0x060001C4 RID: 452 RVA: 0x0000BB4C File Offset: 0x00009D4C
		public static MethodInfo GetMethodInfo<T1, T2>([Nullable(1)] Expression<Action<T1, T2>> expression)
		{
			if (expression != null)
			{
				return SymbolExtensions2.GetMethodInfo(expression);
			}
			return null;
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x0000BB68 File Offset: 0x00009D68
		public static MethodInfo GetMethodInfo<T1, T2, T3>([Nullable(1)] Expression<Action<T1, T2, T3>> expression)
		{
			if (expression != null)
			{
				return SymbolExtensions2.GetMethodInfo(expression);
			}
			return null;
		}

		// Token: 0x060001C6 RID: 454 RVA: 0x0000BB84 File Offset: 0x00009D84
		public static MethodInfo GetMethodInfo<T1, T2, T3, T4>([Nullable(1)] Expression<Action<T1, T2, T3, T4>> expression)
		{
			if (expression != null)
			{
				return SymbolExtensions2.GetMethodInfo(expression);
			}
			return null;
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x0000BBA0 File Offset: 0x00009DA0
		public static MethodInfo GetMethodInfo<T1, T2, T3, T4, T5>([Nullable(1)] Expression<Action<T1, T2, T3, T4, T5>> expression)
		{
			if (expression != null)
			{
				return SymbolExtensions2.GetMethodInfo(expression);
			}
			return null;
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x0000BBBC File Offset: 0x00009DBC
		public static MethodInfo GetMethodInfo<T1, T2, T3, T4, T5, T6>([Nullable(1)] Expression<Action<T1, T2, T3, T4, T5, T6>> expression)
		{
			if (expression != null)
			{
				return SymbolExtensions2.GetMethodInfo(expression);
			}
			return null;
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x0000BBD8 File Offset: 0x00009DD8
		public static MethodInfo GetMethodInfo<T1, T2, T3, T4, T5, T6, T7>([Nullable(1)] Expression<Action<T1, T2, T3, T4, T5, T6, T7>> expression)
		{
			if (expression != null)
			{
				return SymbolExtensions2.GetMethodInfo(expression);
			}
			return null;
		}

		// Token: 0x060001CA RID: 458 RVA: 0x0000BBF4 File Offset: 0x00009DF4
		public static MethodInfo GetMethodInfo<T1, T2, T3, T4, T5, T6, T7, T8>([Nullable(1)] Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8>> expression)
		{
			if (expression != null)
			{
				return SymbolExtensions2.GetMethodInfo(expression);
			}
			return null;
		}

		// Token: 0x060001CB RID: 459 RVA: 0x0000BC10 File Offset: 0x00009E10
		public static MethodInfo GetMethodInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9>([Nullable(1)] Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>> expression)
		{
			if (expression != null)
			{
				return SymbolExtensions2.GetMethodInfo(expression);
			}
			return null;
		}

		// Token: 0x060001CC RID: 460 RVA: 0x0000BC2C File Offset: 0x00009E2C
		public static MethodInfo GetMethodInfo<TResult>([Nullable(1)] Expression<Func<TResult>> expression)
		{
			if (expression != null)
			{
				return SymbolExtensions2.GetMethodInfo(expression);
			}
			return null;
		}

		// Token: 0x060001CD RID: 461 RVA: 0x0000BC48 File Offset: 0x00009E48
		public static MethodInfo GetMethodInfo<T1, TResult>([Nullable(1)] Expression<Func<T1, TResult>> expression)
		{
			if (expression != null)
			{
				return SymbolExtensions2.GetMethodInfo(expression);
			}
			return null;
		}

		// Token: 0x060001CE RID: 462 RVA: 0x0000BC64 File Offset: 0x00009E64
		public static MethodInfo GetMethodInfo<T1, T2, TResult>([Nullable(1)] Expression<Func<T1, T2, TResult>> expression)
		{
			if (expression != null)
			{
				return SymbolExtensions2.GetMethodInfo(expression);
			}
			return null;
		}

		// Token: 0x060001CF RID: 463 RVA: 0x0000BC80 File Offset: 0x00009E80
		public static MethodInfo GetMethodInfo<T1, T2, T3, TResult>([Nullable(1)] Expression<Func<T1, T2, T3, TResult>> expression)
		{
			if (expression != null)
			{
				return SymbolExtensions2.GetMethodInfo(expression);
			}
			return null;
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x0000BC9C File Offset: 0x00009E9C
		public static MethodInfo GetMethodInfo<T1, T2, T3, T4, TResult>([Nullable(1)] Expression<Func<T1, T2, T3, T4, TResult>> expression)
		{
			if (expression != null)
			{
				return SymbolExtensions2.GetMethodInfo(expression);
			}
			return null;
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x0000BCB8 File Offset: 0x00009EB8
		public static MethodInfo GetMethodInfo<T1, T2, T3, T4, T5, TResult>([Nullable(1)] Expression<Func<T1, T2, T3, T4, T5, TResult>> expression)
		{
			if (expression != null)
			{
				return SymbolExtensions2.GetMethodInfo(expression);
			}
			return null;
		}

		// Token: 0x060001D2 RID: 466 RVA: 0x0000BCD4 File Offset: 0x00009ED4
		public static MethodInfo GetMethodInfo<T1, T2, T3, T4, T5, T6, TResult>([Nullable(1)] Expression<Func<T1, T2, T3, T4, T5, T6, TResult>> expression)
		{
			if (expression != null)
			{
				return SymbolExtensions2.GetMethodInfo(expression);
			}
			return null;
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x0000BCF0 File Offset: 0x00009EF0
		public static MethodInfo GetMethodInfo<T1, T2, T3, T4, T5, T6, T7, TResult>([Nullable(1)] Expression<Func<T1, T2, T3, T4, T5, T6, T7, TResult>> expression)
		{
			if (expression != null)
			{
				return SymbolExtensions2.GetMethodInfo(expression);
			}
			return null;
		}

		// Token: 0x060001D4 RID: 468 RVA: 0x0000BD0C File Offset: 0x00009F0C
		public static MethodInfo GetMethodInfo<T1, T2, T3, T4, T5, T6, T7, T8, TResult>([Nullable(1)] Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>> expression)
		{
			if (expression != null)
			{
				return SymbolExtensions2.GetMethodInfo(expression);
			}
			return null;
		}

		// Token: 0x060001D5 RID: 469 RVA: 0x0000BD28 File Offset: 0x00009F28
		public static MethodInfo GetMethodInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>([Nullable(1)] Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression)
		{
			if (expression != null)
			{
				return SymbolExtensions2.GetMethodInfo(expression);
			}
			return null;
		}

		// Token: 0x060001D6 RID: 470 RVA: 0x0000BD44 File Offset: 0x00009F44
		[NullableContext(1)]
		[return: Nullable(2)]
		public static MethodInfo GetMethodInfo(LambdaExpression expression)
		{
			MethodCallExpression methodCallExpression = ((expression != null) ? expression.Body : null) as MethodCallExpression;
			if (methodCallExpression != null)
			{
				MethodInfo method = methodCallExpression.Method;
				if (method != null)
				{
					return method;
				}
			}
			return null;
		}

		// Token: 0x060001D7 RID: 471 RVA: 0x0000BD74 File Offset: 0x00009F74
		public static PropertyInfo GetPropertyInfo<T>([Nullable(1)] Expression<Func<T>> expression)
		{
			if (expression != null)
			{
				return SymbolExtensions2.GetPropertyInfo(expression);
			}
			return null;
		}

		// Token: 0x060001D8 RID: 472 RVA: 0x0000BD90 File Offset: 0x00009F90
		public static PropertyInfo GetPropertyInfo<T, TResult>([Nullable(1)] Expression<Func<T, TResult>> expression)
		{
			if (expression != null)
			{
				return SymbolExtensions2.GetPropertyInfo(expression);
			}
			return null;
		}

		// Token: 0x060001D9 RID: 473 RVA: 0x0000BDAC File Offset: 0x00009FAC
		[NullableContext(1)]
		[return: Nullable(2)]
		public static PropertyInfo GetPropertyInfo(LambdaExpression expression)
		{
			MemberExpression memberExpression = ((expression != null) ? expression.Body : null) as MemberExpression;
			if (memberExpression != null)
			{
				PropertyInfo propertyInfo = memberExpression.Member as PropertyInfo;
				if (propertyInfo != null)
				{
					return propertyInfo;
				}
			}
			return null;
		}

		// Token: 0x060001DA RID: 474 RVA: 0x0000BDE0 File Offset: 0x00009FE0
		public static MethodInfo GetPropertyGetter<T>([Nullable(1)] Expression<Func<T>> expression)
		{
			if (expression != null)
			{
				return SymbolExtensions2.GetPropertyGetter(expression);
			}
			return null;
		}

		// Token: 0x060001DB RID: 475 RVA: 0x0000BDFC File Offset: 0x00009FFC
		public static MethodInfo GetPropertyGetter<T, TResult>([Nullable(1)] Expression<Func<T, TResult>> expression)
		{
			if (expression != null)
			{
				return SymbolExtensions2.GetPropertyGetter(expression);
			}
			return null;
		}

		// Token: 0x060001DC RID: 476 RVA: 0x0000BE18 File Offset: 0x0000A018
		[NullableContext(1)]
		[return: Nullable(2)]
		public static MethodInfo GetPropertyGetter(LambdaExpression expression)
		{
			MemberExpression memberExpression = ((expression != null) ? expression.Body : null) as MemberExpression;
			if (memberExpression != null)
			{
				PropertyInfo propertyInfo = memberExpression.Member as PropertyInfo;
				if (propertyInfo != null)
				{
					if (propertyInfo == null)
					{
						return null;
					}
					return propertyInfo.GetGetMethod(true);
				}
			}
			return null;
		}

		// Token: 0x060001DD RID: 477 RVA: 0x0000BE58 File Offset: 0x0000A058
		public static MethodInfo GetPropertySetter<T>([Nullable(1)] Expression<Func<T>> expression)
		{
			if (expression != null)
			{
				return SymbolExtensions2.GetPropertySetter(expression);
			}
			return null;
		}

		// Token: 0x060001DE RID: 478 RVA: 0x0000BE74 File Offset: 0x0000A074
		public static MethodInfo GetPropertySetter<T, TResult>([Nullable(1)] Expression<Func<T, TResult>> expression)
		{
			if (expression != null)
			{
				return SymbolExtensions2.GetPropertySetter(expression);
			}
			return null;
		}

		// Token: 0x060001DF RID: 479 RVA: 0x0000BE90 File Offset: 0x0000A090
		[NullableContext(1)]
		[return: Nullable(2)]
		public static MethodInfo GetPropertySetter(LambdaExpression expression)
		{
			MemberExpression memberExpression = ((expression != null) ? expression.Body : null) as MemberExpression;
			if (memberExpression != null)
			{
				PropertyInfo propertyInfo = memberExpression.Member as PropertyInfo;
				if (propertyInfo != null)
				{
					if (propertyInfo == null)
					{
						return null;
					}
					return propertyInfo.GetSetMethod(true);
				}
			}
			return null;
		}

		// Token: 0x060001E0 RID: 480 RVA: 0x0000BED0 File Offset: 0x0000A0D0
		[NullableContext(1)]
		[return: Nullable(new byte[] { 2, 1 })]
		public static AccessTools.FieldRef<TField> GetStaticFieldRefAccess<[Nullable(2)] TField>(Expression<Func<TField>> expression)
		{
			if (expression != null)
			{
				return SymbolExtensions2.GetStaticFieldRefAccess<TField>(expression);
			}
			return null;
		}

		// Token: 0x060001E1 RID: 481 RVA: 0x0000BEEC File Offset: 0x0000A0EC
		[NullableContext(1)]
		[return: Nullable(new byte[] { 2, 1 })]
		public static AccessTools.FieldRef<TField> GetStaticFieldRefAccess<[Nullable(2)] TField>(LambdaExpression expression)
		{
			MemberExpression memberExpression = ((expression != null) ? expression.Body : null) as MemberExpression;
			if (memberExpression != null)
			{
				FieldInfo fieldInfo = memberExpression.Member as FieldInfo;
				if (fieldInfo != null)
				{
					if (!(fieldInfo == null))
					{
						return AccessTools2.StaticFieldRefAccess<TField>(fieldInfo, true);
					}
					return null;
				}
			}
			return null;
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x0000BF34 File Offset: 0x0000A134
		[NullableContext(0)]
		[return: Nullable(new byte[] { 2, 0, 1 })]
		public static AccessTools.StructFieldRef<TObject, TField> GetStructFieldRefAccess<TObject, [Nullable(2)] TField>([Nullable(1)] Expression<Func<TField>> expression) where TObject : struct
		{
			if (expression != null)
			{
				return SymbolExtensions2.GetStructFieldRefAccess<TObject, TField>(expression);
			}
			return null;
		}

		// Token: 0x060001E3 RID: 483 RVA: 0x0000BF50 File Offset: 0x0000A150
		[NullableContext(0)]
		[return: Nullable(new byte[] { 2, 0, 1 })]
		public static AccessTools.StructFieldRef<TObject, TField> GetStructFieldRefAccess<TObject, [Nullable(2)] TField>([Nullable(1)] LambdaExpression expression) where TObject : struct
		{
			MemberExpression memberExpression = ((expression != null) ? expression.Body : null) as MemberExpression;
			if (memberExpression != null)
			{
				FieldInfo fieldInfo = memberExpression.Member as FieldInfo;
				if (fieldInfo != null)
				{
					if (!(fieldInfo == null))
					{
						return AccessTools2.StructFieldRefAccess<TObject, TField>(fieldInfo, true);
					}
					return null;
				}
			}
			return null;
		}
	}
}
