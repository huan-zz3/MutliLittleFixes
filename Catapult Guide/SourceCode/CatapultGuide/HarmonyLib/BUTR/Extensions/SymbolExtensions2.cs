using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace HarmonyLib.BUTR.Extensions
{
	// Token: 0x0200000E RID: 14
	[NullableContext(2)]
	[Nullable(0)]
	internal static class SymbolExtensions2
	{
		// Token: 0x060000BF RID: 191 RVA: 0x000086C8 File Offset: 0x000068C8
		public static ConstructorInfo GetConstructorInfo<TResult>([Nullable(1)] Expression<Func<TResult>> expression)
		{
			bool flag = expression != null;
			ConstructorInfo constructorInfo;
			if (flag)
			{
				constructorInfo = SymbolExtensions2.GetConstructorInfo(expression);
			}
			else
			{
				constructorInfo = null;
			}
			return constructorInfo;
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x000086F0 File Offset: 0x000068F0
		public static ConstructorInfo GetConstructorInfo<T1, TResult>([Nullable(1)] Expression<Func<T1, TResult>> expression)
		{
			bool flag = expression != null;
			ConstructorInfo constructorInfo;
			if (flag)
			{
				constructorInfo = SymbolExtensions2.GetConstructorInfo(expression);
			}
			else
			{
				constructorInfo = null;
			}
			return constructorInfo;
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x00008718 File Offset: 0x00006918
		public static ConstructorInfo GetConstructorInfo<T1, T2, TResult>([Nullable(1)] Expression<Func<T1, T2, TResult>> expression)
		{
			bool flag = expression != null;
			ConstructorInfo constructorInfo;
			if (flag)
			{
				constructorInfo = SymbolExtensions2.GetConstructorInfo(expression);
			}
			else
			{
				constructorInfo = null;
			}
			return constructorInfo;
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x00008740 File Offset: 0x00006940
		public static ConstructorInfo GetConstructorInfo<T1, T2, T3, TResult>([Nullable(1)] Expression<Func<T1, T2, T3, TResult>> expression)
		{
			bool flag = expression != null;
			ConstructorInfo constructorInfo;
			if (flag)
			{
				constructorInfo = SymbolExtensions2.GetConstructorInfo(expression);
			}
			else
			{
				constructorInfo = null;
			}
			return constructorInfo;
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x00008768 File Offset: 0x00006968
		public static ConstructorInfo GetConstructorInfo<T1, T2, T3, T4, TResult>([Nullable(1)] Expression<Func<T1, T2, T3, T4, TResult>> expression)
		{
			bool flag = expression != null;
			ConstructorInfo constructorInfo;
			if (flag)
			{
				constructorInfo = SymbolExtensions2.GetConstructorInfo(expression);
			}
			else
			{
				constructorInfo = null;
			}
			return constructorInfo;
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x00008790 File Offset: 0x00006990
		public static ConstructorInfo GetConstructorInfo<T1, T2, T3, T4, T5, TResult>([Nullable(1)] Expression<Func<T1, T2, T3, T4, T5, TResult>> expression)
		{
			bool flag = expression != null;
			ConstructorInfo constructorInfo;
			if (flag)
			{
				constructorInfo = SymbolExtensions2.GetConstructorInfo(expression);
			}
			else
			{
				constructorInfo = null;
			}
			return constructorInfo;
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x000087B8 File Offset: 0x000069B8
		public static ConstructorInfo GetConstructorInfo<T1, T2, T3, T4, T5, T6, TResult>([Nullable(1)] Expression<Func<T1, T2, T3, T4, T5, T6, TResult>> expression)
		{
			bool flag = expression != null;
			ConstructorInfo constructorInfo;
			if (flag)
			{
				constructorInfo = SymbolExtensions2.GetConstructorInfo(expression);
			}
			else
			{
				constructorInfo = null;
			}
			return constructorInfo;
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x000087E0 File Offset: 0x000069E0
		public static ConstructorInfo GetConstructorInfo<T1, T2, T3, T4, T5, T6, T7, TResult>([Nullable(1)] Expression<Func<T1, T2, T3, T4, T5, T6, T7, TResult>> expression)
		{
			bool flag = expression != null;
			ConstructorInfo constructorInfo;
			if (flag)
			{
				constructorInfo = SymbolExtensions2.GetConstructorInfo(expression);
			}
			else
			{
				constructorInfo = null;
			}
			return constructorInfo;
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x00008808 File Offset: 0x00006A08
		public static ConstructorInfo GetConstructorInfo<T1, T2, T3, T4, T5, T6, T7, T8, TResult>([Nullable(1)] Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>> expression)
		{
			bool flag = expression != null;
			ConstructorInfo constructorInfo;
			if (flag)
			{
				constructorInfo = SymbolExtensions2.GetConstructorInfo(expression);
			}
			else
			{
				constructorInfo = null;
			}
			return constructorInfo;
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x00008830 File Offset: 0x00006A30
		public static ConstructorInfo GetConstructorInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>([Nullable(1)] Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression)
		{
			bool flag = expression != null;
			ConstructorInfo constructorInfo;
			if (flag)
			{
				constructorInfo = SymbolExtensions2.GetConstructorInfo(expression);
			}
			else
			{
				constructorInfo = null;
			}
			return constructorInfo;
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x00008858 File Offset: 0x00006A58
		[NullableContext(1)]
		[return: Nullable(2)]
		public static ConstructorInfo GetConstructorInfo(LambdaExpression expression)
		{
			NewExpression newExpression = ((expression != null) ? expression.Body : null) as NewExpression;
			bool flag = newExpression != null && newExpression.Constructor != null;
			ConstructorInfo constructorInfo;
			if (flag)
			{
				constructorInfo = newExpression.Constructor;
			}
			else
			{
				constructorInfo = null;
			}
			return constructorInfo;
		}

		// Token: 0x060000CA RID: 202 RVA: 0x0000889C File Offset: 0x00006A9C
		public static FieldInfo GetFieldInfo<T>([Nullable(1)] Expression<Func<T>> expression)
		{
			bool flag = expression != null;
			FieldInfo fieldInfo;
			if (flag)
			{
				fieldInfo = SymbolExtensions2.GetFieldInfo(expression);
			}
			else
			{
				fieldInfo = null;
			}
			return fieldInfo;
		}

		// Token: 0x060000CB RID: 203 RVA: 0x000088C4 File Offset: 0x00006AC4
		public static FieldInfo GetFieldInfo<T, TResult>([Nullable(1)] Expression<Func<T, TResult>> expression)
		{
			bool flag = expression != null;
			FieldInfo fieldInfo;
			if (flag)
			{
				fieldInfo = SymbolExtensions2.GetFieldInfo(expression);
			}
			else
			{
				fieldInfo = null;
			}
			return fieldInfo;
		}

		// Token: 0x060000CC RID: 204 RVA: 0x000088EC File Offset: 0x00006AEC
		[NullableContext(1)]
		[return: Nullable(2)]
		public static FieldInfo GetFieldInfo(LambdaExpression expression)
		{
			MemberExpression memberExpression = ((expression != null) ? expression.Body : null) as MemberExpression;
			FieldInfo fieldInfo;
			bool flag;
			if (memberExpression != null)
			{
				fieldInfo = memberExpression.Member as FieldInfo;
				flag = fieldInfo != null;
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			FieldInfo fieldInfo2;
			if (flag2)
			{
				fieldInfo2 = fieldInfo;
			}
			else
			{
				fieldInfo2 = null;
			}
			return fieldInfo2;
		}

		// Token: 0x060000CD RID: 205 RVA: 0x00008930 File Offset: 0x00006B30
		[NullableContext(1)]
		[return: Nullable(new byte[] { 2, 1, 1 })]
		public static AccessTools.FieldRef<object, TField> GetFieldRefAccess<[Nullable(2)] TField>(Expression<Func<TField>> expression)
		{
			bool flag = expression != null;
			AccessTools.FieldRef<object, TField> fieldRef;
			if (flag)
			{
				fieldRef = SymbolExtensions2.GetFieldRefAccess<TField>(expression);
			}
			else
			{
				fieldRef = null;
			}
			return fieldRef;
		}

		// Token: 0x060000CE RID: 206 RVA: 0x00008958 File Offset: 0x00006B58
		[NullableContext(1)]
		[return: Nullable(new byte[] { 2, 1, 1 })]
		public static AccessTools.FieldRef<object, TField> GetFieldRefAccess<[Nullable(2)] TField>(LambdaExpression expression)
		{
			MemberExpression memberExpression = ((expression != null) ? expression.Body : null) as MemberExpression;
			FieldInfo fieldInfo;
			bool flag;
			if (memberExpression != null)
			{
				fieldInfo = memberExpression.Member as FieldInfo;
				flag = fieldInfo != null;
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			AccessTools.FieldRef<object, TField> fieldRef;
			if (flag2)
			{
				fieldRef = ((fieldInfo == null) ? null : AccessTools2.FieldRefAccess<object, TField>(fieldInfo, true));
			}
			else
			{
				fieldRef = null;
			}
			return fieldRef;
		}

		// Token: 0x060000CF RID: 207 RVA: 0x000089B0 File Offset: 0x00006BB0
		[NullableContext(1)]
		[return: Nullable(new byte[] { 2, 1, 1 })]
		public static AccessTools.FieldRef<TObject, TField> GetFieldRefAccess<TObject, [Nullable(2)] TField>(Expression<Func<TObject, TField>> expression) where TObject : class
		{
			bool flag = expression != null;
			AccessTools.FieldRef<TObject, TField> fieldRef;
			if (flag)
			{
				fieldRef = SymbolExtensions2.GetFieldRefAccess<TObject, TField>(expression);
			}
			else
			{
				fieldRef = null;
			}
			return fieldRef;
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x000089D8 File Offset: 0x00006BD8
		[NullableContext(1)]
		[return: Nullable(new byte[] { 2, 1, 1 })]
		public static AccessTools.FieldRef<TObject, TField> GetFieldRefAccess<TObject, [Nullable(2)] TField>(LambdaExpression expression) where TObject : class
		{
			MemberExpression memberExpression = ((expression != null) ? expression.Body : null) as MemberExpression;
			FieldInfo fieldInfo;
			bool flag;
			if (memberExpression != null)
			{
				fieldInfo = memberExpression.Member as FieldInfo;
				flag = fieldInfo != null;
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			AccessTools.FieldRef<TObject, TField> fieldRef;
			if (flag2)
			{
				fieldRef = ((fieldInfo == null) ? null : AccessTools2.FieldRefAccess<TObject, TField>(fieldInfo, true));
			}
			else
			{
				fieldRef = null;
			}
			return fieldRef;
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x00008A30 File Offset: 0x00006C30
		[NullableContext(1)]
		[return: Nullable(2)]
		public static MethodInfo GetMethodInfo(Expression<Action> expression)
		{
			bool flag = expression != null;
			MethodInfo methodInfo;
			if (flag)
			{
				methodInfo = SymbolExtensions2.GetMethodInfo(expression);
			}
			else
			{
				methodInfo = null;
			}
			return methodInfo;
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x00008A58 File Offset: 0x00006C58
		public static MethodInfo GetMethodInfo<T1>([Nullable(1)] Expression<Action<T1>> expression)
		{
			bool flag = expression != null;
			MethodInfo methodInfo;
			if (flag)
			{
				methodInfo = SymbolExtensions2.GetMethodInfo(expression);
			}
			else
			{
				methodInfo = null;
			}
			return methodInfo;
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x00008A80 File Offset: 0x00006C80
		public static MethodInfo GetMethodInfo<T1, T2>([Nullable(1)] Expression<Action<T1, T2>> expression)
		{
			bool flag = expression != null;
			MethodInfo methodInfo;
			if (flag)
			{
				methodInfo = SymbolExtensions2.GetMethodInfo(expression);
			}
			else
			{
				methodInfo = null;
			}
			return methodInfo;
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x00008AA8 File Offset: 0x00006CA8
		public static MethodInfo GetMethodInfo<T1, T2, T3>([Nullable(1)] Expression<Action<T1, T2, T3>> expression)
		{
			bool flag = expression != null;
			MethodInfo methodInfo;
			if (flag)
			{
				methodInfo = SymbolExtensions2.GetMethodInfo(expression);
			}
			else
			{
				methodInfo = null;
			}
			return methodInfo;
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x00008AD0 File Offset: 0x00006CD0
		public static MethodInfo GetMethodInfo<T1, T2, T3, T4>([Nullable(1)] Expression<Action<T1, T2, T3, T4>> expression)
		{
			bool flag = expression != null;
			MethodInfo methodInfo;
			if (flag)
			{
				methodInfo = SymbolExtensions2.GetMethodInfo(expression);
			}
			else
			{
				methodInfo = null;
			}
			return methodInfo;
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x00008AF8 File Offset: 0x00006CF8
		public static MethodInfo GetMethodInfo<T1, T2, T3, T4, T5>([Nullable(1)] Expression<Action<T1, T2, T3, T4, T5>> expression)
		{
			bool flag = expression != null;
			MethodInfo methodInfo;
			if (flag)
			{
				methodInfo = SymbolExtensions2.GetMethodInfo(expression);
			}
			else
			{
				methodInfo = null;
			}
			return methodInfo;
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x00008B20 File Offset: 0x00006D20
		public static MethodInfo GetMethodInfo<T1, T2, T3, T4, T5, T6>([Nullable(1)] Expression<Action<T1, T2, T3, T4, T5, T6>> expression)
		{
			bool flag = expression != null;
			MethodInfo methodInfo;
			if (flag)
			{
				methodInfo = SymbolExtensions2.GetMethodInfo(expression);
			}
			else
			{
				methodInfo = null;
			}
			return methodInfo;
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x00008B48 File Offset: 0x00006D48
		public static MethodInfo GetMethodInfo<T1, T2, T3, T4, T5, T6, T7>([Nullable(1)] Expression<Action<T1, T2, T3, T4, T5, T6, T7>> expression)
		{
			bool flag = expression != null;
			MethodInfo methodInfo;
			if (flag)
			{
				methodInfo = SymbolExtensions2.GetMethodInfo(expression);
			}
			else
			{
				methodInfo = null;
			}
			return methodInfo;
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x00008B70 File Offset: 0x00006D70
		public static MethodInfo GetMethodInfo<T1, T2, T3, T4, T5, T6, T7, T8>([Nullable(1)] Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8>> expression)
		{
			bool flag = expression != null;
			MethodInfo methodInfo;
			if (flag)
			{
				methodInfo = SymbolExtensions2.GetMethodInfo(expression);
			}
			else
			{
				methodInfo = null;
			}
			return methodInfo;
		}

		// Token: 0x060000DA RID: 218 RVA: 0x00008B98 File Offset: 0x00006D98
		public static MethodInfo GetMethodInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9>([Nullable(1)] Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>> expression)
		{
			bool flag = expression != null;
			MethodInfo methodInfo;
			if (flag)
			{
				methodInfo = SymbolExtensions2.GetMethodInfo(expression);
			}
			else
			{
				methodInfo = null;
			}
			return methodInfo;
		}

		// Token: 0x060000DB RID: 219 RVA: 0x00008BC0 File Offset: 0x00006DC0
		public static MethodInfo GetMethodInfo<TResult>([Nullable(1)] Expression<Func<TResult>> expression)
		{
			bool flag = expression != null;
			MethodInfo methodInfo;
			if (flag)
			{
				methodInfo = SymbolExtensions2.GetMethodInfo(expression);
			}
			else
			{
				methodInfo = null;
			}
			return methodInfo;
		}

		// Token: 0x060000DC RID: 220 RVA: 0x00008BE8 File Offset: 0x00006DE8
		public static MethodInfo GetMethodInfo<T1, TResult>([Nullable(1)] Expression<Func<T1, TResult>> expression)
		{
			bool flag = expression != null;
			MethodInfo methodInfo;
			if (flag)
			{
				methodInfo = SymbolExtensions2.GetMethodInfo(expression);
			}
			else
			{
				methodInfo = null;
			}
			return methodInfo;
		}

		// Token: 0x060000DD RID: 221 RVA: 0x00008C10 File Offset: 0x00006E10
		public static MethodInfo GetMethodInfo<T1, T2, TResult>([Nullable(1)] Expression<Func<T1, T2, TResult>> expression)
		{
			bool flag = expression != null;
			MethodInfo methodInfo;
			if (flag)
			{
				methodInfo = SymbolExtensions2.GetMethodInfo(expression);
			}
			else
			{
				methodInfo = null;
			}
			return methodInfo;
		}

		// Token: 0x060000DE RID: 222 RVA: 0x00008C38 File Offset: 0x00006E38
		public static MethodInfo GetMethodInfo<T1, T2, T3, TResult>([Nullable(1)] Expression<Func<T1, T2, T3, TResult>> expression)
		{
			bool flag = expression != null;
			MethodInfo methodInfo;
			if (flag)
			{
				methodInfo = SymbolExtensions2.GetMethodInfo(expression);
			}
			else
			{
				methodInfo = null;
			}
			return methodInfo;
		}

		// Token: 0x060000DF RID: 223 RVA: 0x00008C60 File Offset: 0x00006E60
		public static MethodInfo GetMethodInfo<T1, T2, T3, T4, TResult>([Nullable(1)] Expression<Func<T1, T2, T3, T4, TResult>> expression)
		{
			bool flag = expression != null;
			MethodInfo methodInfo;
			if (flag)
			{
				methodInfo = SymbolExtensions2.GetMethodInfo(expression);
			}
			else
			{
				methodInfo = null;
			}
			return methodInfo;
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x00008C88 File Offset: 0x00006E88
		public static MethodInfo GetMethodInfo<T1, T2, T3, T4, T5, TResult>([Nullable(1)] Expression<Func<T1, T2, T3, T4, T5, TResult>> expression)
		{
			bool flag = expression != null;
			MethodInfo methodInfo;
			if (flag)
			{
				methodInfo = SymbolExtensions2.GetMethodInfo(expression);
			}
			else
			{
				methodInfo = null;
			}
			return methodInfo;
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x00008CB0 File Offset: 0x00006EB0
		public static MethodInfo GetMethodInfo<T1, T2, T3, T4, T5, T6, TResult>([Nullable(1)] Expression<Func<T1, T2, T3, T4, T5, T6, TResult>> expression)
		{
			bool flag = expression != null;
			MethodInfo methodInfo;
			if (flag)
			{
				methodInfo = SymbolExtensions2.GetMethodInfo(expression);
			}
			else
			{
				methodInfo = null;
			}
			return methodInfo;
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x00008CD8 File Offset: 0x00006ED8
		public static MethodInfo GetMethodInfo<T1, T2, T3, T4, T5, T6, T7, TResult>([Nullable(1)] Expression<Func<T1, T2, T3, T4, T5, T6, T7, TResult>> expression)
		{
			bool flag = expression != null;
			MethodInfo methodInfo;
			if (flag)
			{
				methodInfo = SymbolExtensions2.GetMethodInfo(expression);
			}
			else
			{
				methodInfo = null;
			}
			return methodInfo;
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x00008D00 File Offset: 0x00006F00
		public static MethodInfo GetMethodInfo<T1, T2, T3, T4, T5, T6, T7, T8, TResult>([Nullable(1)] Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>> expression)
		{
			bool flag = expression != null;
			MethodInfo methodInfo;
			if (flag)
			{
				methodInfo = SymbolExtensions2.GetMethodInfo(expression);
			}
			else
			{
				methodInfo = null;
			}
			return methodInfo;
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x00008D28 File Offset: 0x00006F28
		public static MethodInfo GetMethodInfo<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>([Nullable(1)] Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression)
		{
			bool flag = expression != null;
			MethodInfo methodInfo;
			if (flag)
			{
				methodInfo = SymbolExtensions2.GetMethodInfo(expression);
			}
			else
			{
				methodInfo = null;
			}
			return methodInfo;
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x00008D50 File Offset: 0x00006F50
		[NullableContext(1)]
		[return: Nullable(2)]
		public static MethodInfo GetMethodInfo(LambdaExpression expression)
		{
			MethodCallExpression methodCallExpression = ((expression != null) ? expression.Body : null) as MethodCallExpression;
			MethodInfo method;
			bool flag;
			if (methodCallExpression != null)
			{
				method = methodCallExpression.Method;
				flag = method != null;
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			MethodInfo methodInfo;
			if (flag2)
			{
				methodInfo = method;
			}
			else
			{
				methodInfo = null;
			}
			return methodInfo;
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x00008D90 File Offset: 0x00006F90
		public static PropertyInfo GetPropertyInfo<T>([Nullable(1)] Expression<Func<T>> expression)
		{
			bool flag = expression != null;
			PropertyInfo propertyInfo;
			if (flag)
			{
				propertyInfo = SymbolExtensions2.GetPropertyInfo(expression);
			}
			else
			{
				propertyInfo = null;
			}
			return propertyInfo;
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x00008DB8 File Offset: 0x00006FB8
		public static PropertyInfo GetPropertyInfo<T, TResult>([Nullable(1)] Expression<Func<T, TResult>> expression)
		{
			bool flag = expression != null;
			PropertyInfo propertyInfo;
			if (flag)
			{
				propertyInfo = SymbolExtensions2.GetPropertyInfo(expression);
			}
			else
			{
				propertyInfo = null;
			}
			return propertyInfo;
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x00008DE0 File Offset: 0x00006FE0
		[NullableContext(1)]
		[return: Nullable(2)]
		public static PropertyInfo GetPropertyInfo(LambdaExpression expression)
		{
			MemberExpression memberExpression = ((expression != null) ? expression.Body : null) as MemberExpression;
			PropertyInfo propertyInfo;
			bool flag;
			if (memberExpression != null)
			{
				propertyInfo = memberExpression.Member as PropertyInfo;
				flag = propertyInfo != null;
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			PropertyInfo propertyInfo2;
			if (flag2)
			{
				propertyInfo2 = propertyInfo;
			}
			else
			{
				propertyInfo2 = null;
			}
			return propertyInfo2;
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x00008E24 File Offset: 0x00007024
		public static MethodInfo GetPropertyGetter<T>([Nullable(1)] Expression<Func<T>> expression)
		{
			bool flag = expression != null;
			MethodInfo methodInfo;
			if (flag)
			{
				methodInfo = SymbolExtensions2.GetPropertyGetter(expression);
			}
			else
			{
				methodInfo = null;
			}
			return methodInfo;
		}

		// Token: 0x060000EA RID: 234 RVA: 0x00008E4C File Offset: 0x0000704C
		public static MethodInfo GetPropertyGetter<T, TResult>([Nullable(1)] Expression<Func<T, TResult>> expression)
		{
			bool flag = expression != null;
			MethodInfo methodInfo;
			if (flag)
			{
				methodInfo = SymbolExtensions2.GetPropertyGetter(expression);
			}
			else
			{
				methodInfo = null;
			}
			return methodInfo;
		}

		// Token: 0x060000EB RID: 235 RVA: 0x00008E74 File Offset: 0x00007074
		[NullableContext(1)]
		[return: Nullable(2)]
		public static MethodInfo GetPropertyGetter(LambdaExpression expression)
		{
			MemberExpression memberExpression = ((expression != null) ? expression.Body : null) as MemberExpression;
			PropertyInfo propertyInfo;
			bool flag;
			if (memberExpression != null)
			{
				propertyInfo = memberExpression.Member as PropertyInfo;
				flag = propertyInfo != null;
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			MethodInfo methodInfo;
			if (flag2)
			{
				methodInfo = ((propertyInfo != null) ? propertyInfo.GetGetMethod(true) : null);
			}
			else
			{
				methodInfo = null;
			}
			return methodInfo;
		}

		// Token: 0x060000EC RID: 236 RVA: 0x00008EC4 File Offset: 0x000070C4
		public static MethodInfo GetPropertySetter<T>([Nullable(1)] Expression<Func<T>> expression)
		{
			bool flag = expression != null;
			MethodInfo methodInfo;
			if (flag)
			{
				methodInfo = SymbolExtensions2.GetPropertySetter(expression);
			}
			else
			{
				methodInfo = null;
			}
			return methodInfo;
		}

		// Token: 0x060000ED RID: 237 RVA: 0x00008EEC File Offset: 0x000070EC
		public static MethodInfo GetPropertySetter<T, TResult>([Nullable(1)] Expression<Func<T, TResult>> expression)
		{
			bool flag = expression != null;
			MethodInfo methodInfo;
			if (flag)
			{
				methodInfo = SymbolExtensions2.GetPropertySetter(expression);
			}
			else
			{
				methodInfo = null;
			}
			return methodInfo;
		}

		// Token: 0x060000EE RID: 238 RVA: 0x00008F14 File Offset: 0x00007114
		[NullableContext(1)]
		[return: Nullable(2)]
		public static MethodInfo GetPropertySetter(LambdaExpression expression)
		{
			MemberExpression memberExpression = ((expression != null) ? expression.Body : null) as MemberExpression;
			PropertyInfo propertyInfo;
			bool flag;
			if (memberExpression != null)
			{
				propertyInfo = memberExpression.Member as PropertyInfo;
				flag = propertyInfo != null;
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			MethodInfo methodInfo;
			if (flag2)
			{
				methodInfo = ((propertyInfo != null) ? propertyInfo.GetSetMethod(true) : null);
			}
			else
			{
				methodInfo = null;
			}
			return methodInfo;
		}

		// Token: 0x060000EF RID: 239 RVA: 0x00008F64 File Offset: 0x00007164
		[NullableContext(1)]
		[return: Nullable(new byte[] { 2, 1 })]
		public static AccessTools.FieldRef<TField> GetStaticFieldRefAccess<[Nullable(2)] TField>(Expression<Func<TField>> expression)
		{
			bool flag = expression != null;
			AccessTools.FieldRef<TField> fieldRef;
			if (flag)
			{
				fieldRef = SymbolExtensions2.GetStaticFieldRefAccess<TField>(expression);
			}
			else
			{
				fieldRef = null;
			}
			return fieldRef;
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x00008F8C File Offset: 0x0000718C
		[NullableContext(1)]
		[return: Nullable(new byte[] { 2, 1 })]
		public static AccessTools.FieldRef<TField> GetStaticFieldRefAccess<[Nullable(2)] TField>(LambdaExpression expression)
		{
			MemberExpression memberExpression = ((expression != null) ? expression.Body : null) as MemberExpression;
			FieldInfo fieldInfo;
			bool flag;
			if (memberExpression != null)
			{
				fieldInfo = memberExpression.Member as FieldInfo;
				flag = fieldInfo != null;
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			AccessTools.FieldRef<TField> fieldRef;
			if (flag2)
			{
				fieldRef = ((fieldInfo == null) ? null : AccessTools2.StaticFieldRefAccess<TField>(fieldInfo, true));
			}
			else
			{
				fieldRef = null;
			}
			return fieldRef;
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x00008FE4 File Offset: 0x000071E4
		[NullableContext(0)]
		[return: Nullable(new byte[] { 2, 0, 1 })]
		public static AccessTools.StructFieldRef<TObject, TField> GetStructFieldRefAccess<TObject, [Nullable(2)] TField>([Nullable(1)] Expression<Func<TField>> expression) where TObject : struct
		{
			bool flag = expression != null;
			AccessTools.StructFieldRef<TObject, TField> structFieldRef;
			if (flag)
			{
				structFieldRef = SymbolExtensions2.GetStructFieldRefAccess<TObject, TField>(expression);
			}
			else
			{
				structFieldRef = null;
			}
			return structFieldRef;
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x0000900C File Offset: 0x0000720C
		[NullableContext(0)]
		[return: Nullable(new byte[] { 2, 0, 1 })]
		public static AccessTools.StructFieldRef<TObject, TField> GetStructFieldRefAccess<TObject, [Nullable(2)] TField>([Nullable(1)] LambdaExpression expression) where TObject : struct
		{
			MemberExpression memberExpression = ((expression != null) ? expression.Body : null) as MemberExpression;
			FieldInfo fieldInfo;
			bool flag;
			if (memberExpression != null)
			{
				fieldInfo = memberExpression.Member as FieldInfo;
				flag = fieldInfo != null;
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			AccessTools.StructFieldRef<TObject, TField> structFieldRef;
			if (flag2)
			{
				structFieldRef = ((fieldInfo == null) ? null : AccessTools2.StructFieldRefAccess<TObject, TField>(fieldInfo, true));
			}
			else
			{
				structFieldRef = null;
			}
			return structFieldRef;
		}
	}
}
