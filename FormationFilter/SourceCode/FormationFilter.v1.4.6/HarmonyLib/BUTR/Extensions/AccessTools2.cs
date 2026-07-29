using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace HarmonyLib.BUTR.Extensions
{
	// Token: 0x02000026 RID: 38
	[NullableContext(1)]
	[Nullable(0)]
	internal static class AccessTools2
	{
		// Token: 0x0600014F RID: 335 RVA: 0x00009758 File Offset: 0x00007958
		[return: Nullable(2)]
		public static ConstructorInfo DeclaredConstructor(Type type, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, bool searchForStatic = false, bool logErrorInTrace = true)
		{
			if (type == null)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.DeclaredConstructor: 'type' is null");
				}
				return null;
			}
			if (parameters == null)
			{
				parameters = Type.EmptyTypes;
			}
			BindingFlags bindingFlags = (searchForStatic ? (AccessTools.allDeclared & ~BindingFlags.Instance) : (AccessTools.allDeclared & ~BindingFlags.Static));
			return type.GetConstructor(bindingFlags, null, parameters, new ParameterModifier[0]);
		}

		// Token: 0x06000150 RID: 336 RVA: 0x000097A8 File Offset: 0x000079A8
		[return: Nullable(2)]
		public static ConstructorInfo Constructor(Type type, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, bool searchForStatic = false, bool logErrorInTrace = true)
		{
			if (type == null)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.ConstructorInfo: 'type' is null");
				}
				return null;
			}
			if (parameters == null)
			{
				parameters = Type.EmptyTypes;
			}
			BindingFlags flags = (searchForStatic ? (AccessTools.all & ~BindingFlags.Instance) : (AccessTools.all & ~BindingFlags.Static));
			return AccessTools2.FindIncludingBaseTypes<ConstructorInfo>(type, (Type t) => t.GetConstructor(flags, null, parameters, new ParameterModifier[0]));
		}

		// Token: 0x06000151 RID: 337 RVA: 0x00009814 File Offset: 0x00007A14
		[return: Nullable(2)]
		public static ConstructorInfo DeclaredConstructor(string typeString, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, bool searchForStatic = false, bool logErrorInTrace = true)
		{
			if (string.IsNullOrWhiteSpace(typeString))
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.Constructor: 'typeString' is null or whitespace/empty");
				}
				return null;
			}
			Type type = AccessTools2.TypeByName(typeString, logErrorInTrace);
			if (type == null)
			{
				return null;
			}
			return AccessTools2.DeclaredConstructor(type, parameters, searchForStatic, logErrorInTrace);
		}

		// Token: 0x06000152 RID: 338 RVA: 0x00009850 File Offset: 0x00007A50
		[return: Nullable(2)]
		public static ConstructorInfo Constructor(string typeString, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, bool searchForStatic = false, bool logErrorInTrace = true)
		{
			if (string.IsNullOrWhiteSpace(typeString))
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.Constructor: 'typeString' is null or whitespace/empty");
				}
				return null;
			}
			Type type = AccessTools2.TypeByName(typeString, logErrorInTrace);
			if (type == null)
			{
				return null;
			}
			return AccessTools2.Constructor(type, parameters, searchForStatic, logErrorInTrace);
		}

		// Token: 0x06000153 RID: 339 RVA: 0x0000988C File Offset: 0x00007A8C
		[return: Nullable(2)]
		public static TDelegate GetDeclaredConstructorDelegate<[Nullable(0)] TDelegate>(Type type, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			ConstructorInfo constructorInfo = AccessTools2.DeclaredConstructor(type, parameters, false, logErrorInTrace);
			if (constructorInfo == null)
			{
				return default(TDelegate);
			}
			return AccessTools2.GetDelegate<TDelegate>(constructorInfo, logErrorInTrace);
		}

		// Token: 0x06000154 RID: 340 RVA: 0x000098B8 File Offset: 0x00007AB8
		[return: Nullable(2)]
		public static TDelegate GetConstructorDelegate<[Nullable(0)] TDelegate>(Type type, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			ConstructorInfo constructorInfo = AccessTools2.Constructor(type, parameters, false, logErrorInTrace);
			if (constructorInfo == null)
			{
				return default(TDelegate);
			}
			return AccessTools2.GetDelegate<TDelegate>(constructorInfo, logErrorInTrace);
		}

		// Token: 0x06000155 RID: 341 RVA: 0x000098E4 File Offset: 0x00007AE4
		[return: Nullable(2)]
		public static TDelegate GetDeclaredConstructorDelegate<[Nullable(0)] TDelegate>(string typeString, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			ConstructorInfo constructorInfo = AccessTools2.DeclaredConstructor(typeString, parameters, false, logErrorInTrace);
			if (constructorInfo == null)
			{
				return default(TDelegate);
			}
			return AccessTools2.GetDelegate<TDelegate>(constructorInfo, logErrorInTrace);
		}

		// Token: 0x06000156 RID: 342 RVA: 0x00009910 File Offset: 0x00007B10
		[return: Nullable(2)]
		public static TDelegate GetConstructorDelegate<[Nullable(0)] TDelegate>(string typeString, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			ConstructorInfo constructorInfo = AccessTools2.Constructor(typeString, parameters, false, logErrorInTrace);
			if (constructorInfo == null)
			{
				return default(TDelegate);
			}
			return AccessTools2.GetDelegate<TDelegate>(constructorInfo, logErrorInTrace);
		}

		// Token: 0x06000157 RID: 343 RVA: 0x0000993C File Offset: 0x00007B3C
		[return: Nullable(2)]
		public static TDelegate GetPropertyGetterDelegate<[Nullable(0)] TDelegate>(PropertyInfo propertyInfo, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = ((propertyInfo != null) ? propertyInfo.GetGetMethod(true) : null);
			if (methodInfo == null)
			{
				return default(TDelegate);
			}
			return AccessTools2.GetDelegate<TDelegate>(methodInfo, logErrorInTrace);
		}

		// Token: 0x06000158 RID: 344 RVA: 0x0000996C File Offset: 0x00007B6C
		[return: Nullable(2)]
		public static TDelegate GetPropertySetterDelegate<[Nullable(0)] TDelegate>(PropertyInfo propertyInfo, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = ((propertyInfo != null) ? propertyInfo.GetSetMethod(true) : null);
			if (methodInfo == null)
			{
				return default(TDelegate);
			}
			return AccessTools2.GetDelegate<TDelegate>(methodInfo, logErrorInTrace);
		}

		// Token: 0x06000159 RID: 345 RVA: 0x0000999C File Offset: 0x00007B9C
		[return: Nullable(2)]
		public static TDelegate GetPropertyGetterDelegate<[Nullable(0)] TDelegate>([Nullable(2)] object instance, PropertyInfo propertyInfo, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = ((propertyInfo != null) ? propertyInfo.GetGetMethod(true) : null);
			if (methodInfo == null)
			{
				return default(TDelegate);
			}
			return AccessTools2.GetDelegate<TDelegate>(instance, methodInfo, logErrorInTrace);
		}

		// Token: 0x0600015A RID: 346 RVA: 0x000099CC File Offset: 0x00007BCC
		[return: Nullable(2)]
		public static TDelegate GetPropertySetterDelegate<[Nullable(0)] TDelegate>([Nullable(2)] object instance, PropertyInfo propertyInfo, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = ((propertyInfo != null) ? propertyInfo.GetSetMethod(true) : null);
			if (methodInfo == null)
			{
				return default(TDelegate);
			}
			return AccessTools2.GetDelegate<TDelegate>(instance, methodInfo, logErrorInTrace);
		}

		// Token: 0x0600015B RID: 347 RVA: 0x000099FC File Offset: 0x00007BFC
		[return: Nullable(2)]
		public static TDelegate GetDeclaredPropertyGetterDelegate<[Nullable(0)] TDelegate>(Type type, string name, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.DeclaredPropertyGetter(type, name, logErrorInTrace);
			if (methodInfo == null)
			{
				return default(TDelegate);
			}
			return AccessTools2.GetDelegate<TDelegate>(methodInfo, logErrorInTrace);
		}

		// Token: 0x0600015C RID: 348 RVA: 0x00009A28 File Offset: 0x00007C28
		[return: Nullable(2)]
		public static TDelegate GetDeclaredPropertySetterDelegate<[Nullable(0)] TDelegate>(Type type, string name, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.DeclaredPropertySetter(type, name, logErrorInTrace);
			if (methodInfo == null)
			{
				return default(TDelegate);
			}
			return AccessTools2.GetDelegate<TDelegate>(methodInfo, logErrorInTrace);
		}

		// Token: 0x0600015D RID: 349 RVA: 0x00009A54 File Offset: 0x00007C54
		[return: Nullable(2)]
		public static TDelegate GetPropertyGetterDelegate<[Nullable(0)] TDelegate>(Type type, string name, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.PropertyGetter(type, name, logErrorInTrace);
			if (methodInfo == null)
			{
				return default(TDelegate);
			}
			return AccessTools2.GetDelegate<TDelegate>(methodInfo, logErrorInTrace);
		}

		// Token: 0x0600015E RID: 350 RVA: 0x00009A80 File Offset: 0x00007C80
		[return: Nullable(2)]
		public static TDelegate GetPropertySetterDelegate<[Nullable(0)] TDelegate>(Type type, string name, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.PropertySetter(type, name, logErrorInTrace);
			if (methodInfo == null)
			{
				return default(TDelegate);
			}
			return AccessTools2.GetDelegate<TDelegate>(methodInfo, logErrorInTrace);
		}

		// Token: 0x0600015F RID: 351 RVA: 0x00009AAC File Offset: 0x00007CAC
		[return: Nullable(2)]
		public static TDelegate GetDeclaredPropertyGetterDelegate<[Nullable(0)] TDelegate>([Nullable(2)] object instance, Type type, string method, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.DeclaredPropertyGetter(type, method, logErrorInTrace);
			if (methodInfo == null)
			{
				return default(TDelegate);
			}
			return AccessTools2.GetDelegate<TDelegate>(instance, methodInfo, logErrorInTrace);
		}

		// Token: 0x06000160 RID: 352 RVA: 0x00009AD8 File Offset: 0x00007CD8
		[return: Nullable(2)]
		public static TDelegate GetDeclaredPropertySetterDelegate<[Nullable(0)] TDelegate>([Nullable(2)] object instance, Type type, string method, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.DeclaredPropertySetter(type, method, logErrorInTrace);
			if (methodInfo == null)
			{
				return default(TDelegate);
			}
			return AccessTools2.GetDelegate<TDelegate>(instance, methodInfo, logErrorInTrace);
		}

		// Token: 0x06000161 RID: 353 RVA: 0x00009B04 File Offset: 0x00007D04
		[return: Nullable(2)]
		public static TDelegate GetPropertyGetterDelegate<[Nullable(0)] TDelegate>([Nullable(2)] object instance, Type type, string method, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.PropertyGetter(type, method, logErrorInTrace);
			if (methodInfo == null)
			{
				return default(TDelegate);
			}
			return AccessTools2.GetDelegate<TDelegate>(instance, methodInfo, logErrorInTrace);
		}

		// Token: 0x06000162 RID: 354 RVA: 0x00009B30 File Offset: 0x00007D30
		[return: Nullable(2)]
		public static TDelegate GetPropertySetterDelegate<[Nullable(0)] TDelegate>([Nullable(2)] object instance, Type type, string method, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.PropertySetter(type, method, logErrorInTrace);
			if (methodInfo == null)
			{
				return default(TDelegate);
			}
			return AccessTools2.GetDelegate<TDelegate>(instance, methodInfo, logErrorInTrace);
		}

		// Token: 0x06000163 RID: 355 RVA: 0x00009B5C File Offset: 0x00007D5C
		[return: Nullable(2)]
		public static TDelegate GetDeclaredPropertyGetterDelegate<[Nullable(0)] TDelegate>(string typeColonPropertyName, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.DeclaredPropertyGetter(typeColonPropertyName, logErrorInTrace);
			if (methodInfo == null)
			{
				return default(TDelegate);
			}
			return AccessTools2.GetDelegate<TDelegate>(methodInfo, logErrorInTrace);
		}

		// Token: 0x06000164 RID: 356 RVA: 0x00009B88 File Offset: 0x00007D88
		[return: Nullable(2)]
		public static TDelegate GetDeclaredPropertySetterDelegate<[Nullable(0)] TDelegate>(string typeColonPropertyName, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.DeclaredPropertySetter(typeColonPropertyName, logErrorInTrace);
			if (methodInfo == null)
			{
				return default(TDelegate);
			}
			return AccessTools2.GetDelegate<TDelegate>(methodInfo, logErrorInTrace);
		}

		// Token: 0x06000165 RID: 357 RVA: 0x00009BB4 File Offset: 0x00007DB4
		[return: Nullable(2)]
		public static TDelegate GetPropertyGetterDelegate<[Nullable(0)] TDelegate>(string typeColonPropertyName, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.PropertyGetter(typeColonPropertyName, logErrorInTrace);
			if (methodInfo == null)
			{
				return default(TDelegate);
			}
			return AccessTools2.GetDelegate<TDelegate>(methodInfo, logErrorInTrace);
		}

		// Token: 0x06000166 RID: 358 RVA: 0x00009BE0 File Offset: 0x00007DE0
		[return: Nullable(2)]
		public static TDelegate GetPropertySetterDelegate<[Nullable(0)] TDelegate>(string typeColonPropertyName, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.PropertySetter(typeColonPropertyName, logErrorInTrace);
			if (methodInfo == null)
			{
				return default(TDelegate);
			}
			return AccessTools2.GetDelegate<TDelegate>(methodInfo, logErrorInTrace);
		}

		// Token: 0x06000167 RID: 359 RVA: 0x00009C0C File Offset: 0x00007E0C
		[return: Nullable(2)]
		public static TDelegate GetDeclaredPropertyGetterDelegate<[Nullable(0)] TDelegate>([Nullable(2)] object instance, string typeColonPropertyName, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.DeclaredPropertyGetter(typeColonPropertyName, logErrorInTrace);
			if (methodInfo == null)
			{
				return default(TDelegate);
			}
			return AccessTools2.GetDelegate<TDelegate>(instance, methodInfo, logErrorInTrace);
		}

		// Token: 0x06000168 RID: 360 RVA: 0x00009C38 File Offset: 0x00007E38
		[return: Nullable(2)]
		public static TDelegate GetDeclaredPropertySetterDelegate<[Nullable(0)] TDelegate>([Nullable(2)] object instance, string typeColonPropertyName, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.DeclaredPropertySetter(typeColonPropertyName, logErrorInTrace);
			if (methodInfo == null)
			{
				return default(TDelegate);
			}
			return AccessTools2.GetDelegate<TDelegate>(instance, methodInfo, logErrorInTrace);
		}

		// Token: 0x06000169 RID: 361 RVA: 0x00009C64 File Offset: 0x00007E64
		[return: Nullable(2)]
		public static TDelegate GetPropertyGetterDelegate<[Nullable(0)] TDelegate>([Nullable(2)] object instance, string typeColonPropertyName, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.PropertyGetter(typeColonPropertyName, logErrorInTrace);
			if (methodInfo == null)
			{
				return default(TDelegate);
			}
			return AccessTools2.GetDelegate<TDelegate>(instance, methodInfo, logErrorInTrace);
		}

		// Token: 0x0600016A RID: 362 RVA: 0x00009C90 File Offset: 0x00007E90
		[return: Nullable(2)]
		public static TDelegate GetPropertySetterDelegate<[Nullable(0)] TDelegate>([Nullable(2)] object instance, string typeColonPropertyName, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.PropertySetter(typeColonPropertyName, logErrorInTrace);
			if (methodInfo == null)
			{
				return default(TDelegate);
			}
			return AccessTools2.GetDelegate<TDelegate>(instance, methodInfo, logErrorInTrace);
		}

		// Token: 0x0600016B RID: 363 RVA: 0x00009CBC File Offset: 0x00007EBC
		[return: Nullable(2)]
		public static TDelegate GetDelegate<[Nullable(0)] TDelegate>(ConstructorInfo constructorInfo, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			TDelegate tdelegate;
			if (constructorInfo == null)
			{
				tdelegate = default(TDelegate);
				return tdelegate;
			}
			MethodInfo method = typeof(TDelegate).GetMethod("Invoke");
			if (method == null)
			{
				tdelegate = default(TDelegate);
				return tdelegate;
			}
			if (!method.ReturnType.IsAssignableFrom(constructorInfo.DeclaringType))
			{
				tdelegate = default(TDelegate);
				return tdelegate;
			}
			ParameterInfo[] parameters = method.GetParameters();
			ParameterInfo[] constructorParameters = constructorInfo.GetParameters();
			if (parameters.Length - constructorParameters.Length != 0 && !AccessTools2.ParametersAreEqual(parameters, constructorParameters))
			{
				tdelegate = default(TDelegate);
				return tdelegate;
			}
			ParameterExpression parameterExpression = Expression.Parameter(typeof(object), "instance");
			List<ParameterExpression> list = parameters.Select<ParameterInfo, ParameterExpression>((ParameterInfo pi, int i) => Expression.Parameter(pi.ParameterType, string.Format("p{0}", i))).ToList<ParameterExpression>();
			List<Expression> list2 = list.Select<ParameterExpression, Expression>(delegate(ParameterExpression pe, int i)
			{
				if (pe.IsByRef || pe.Type.Equals(constructorParameters[i].ParameterType))
				{
					return pe;
				}
				return Expression.Convert(pe, constructorParameters[i].ParameterType);
			}).ToList<Expression>();
			Expression expression = Expression.New(constructorInfo, list2);
			UnaryExpression unaryExpression = Expression.Convert(expression, method.ReturnType);
			try
			{
				tdelegate = Expression.Lambda<TDelegate>(unaryExpression, list).Compile();
			}
			catch (Exception ex)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError(string.Format("AccessTools2.GetDelegate<{0}>: Error while compiling lambds expression '{1}'", typeof(TDelegate).FullName, ex));
				}
				tdelegate = default(TDelegate);
			}
			return tdelegate;
		}

		// Token: 0x0600016C RID: 364 RVA: 0x00009E24 File Offset: 0x00008024
		[return: Nullable(2)]
		public static TDelegate GetDelegate<[Nullable(0)] TDelegate>([Nullable(2)] object instance, MethodInfo methodInfo, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			TDelegate tdelegate;
			if (methodInfo == null)
			{
				tdelegate = default(TDelegate);
				return tdelegate;
			}
			MethodInfo method = typeof(TDelegate).GetMethod("Invoke");
			if (method == null)
			{
				tdelegate = default(TDelegate);
				return tdelegate;
			}
			bool flag = method.ReturnType.IsEnum || methodInfo.ReturnType.IsEnum;
			bool flag2 = method.ReturnType.IsNumeric() || methodInfo.ReturnType.IsNumeric();
			if (!flag && !flag2 && !method.ReturnType.IsAssignableFrom(methodInfo.ReturnType))
			{
				tdelegate = default(TDelegate);
				return tdelegate;
			}
			ParameterInfo[] parameters = method.GetParameters();
			ParameterInfo[] methodParameters = methodInfo.GetParameters();
			bool flag3 = parameters.Length - methodParameters.Length == 0 && AccessTools2.ParametersAreEqual(parameters, methodParameters);
			bool flag4 = instance != null;
			bool flag5 = parameters.Length - methodParameters.Length == 1 && (parameters[0].ParameterType.IsAssignableFrom(methodInfo.DeclaringType) || methodInfo.DeclaringType.IsAssignableFrom(parameters[0].ParameterType));
			if (!flag4 && !flag5 && !methodInfo.IsStatic)
			{
				tdelegate = default(TDelegate);
				return tdelegate;
			}
			if (flag4 && methodInfo.IsStatic)
			{
				tdelegate = default(TDelegate);
				return tdelegate;
			}
			if (flag4 && !methodInfo.IsStatic && !methodInfo.DeclaringType.IsAssignableFrom(instance.GetType()))
			{
				tdelegate = default(TDelegate);
				return tdelegate;
			}
			if (flag3 && flag5)
			{
				tdelegate = default(TDelegate);
				return tdelegate;
			}
			if (flag4 && (flag5 || !flag3))
			{
				tdelegate = default(TDelegate);
				return tdelegate;
			}
			if (flag5 && (flag4 || flag3))
			{
				tdelegate = default(TDelegate);
				return tdelegate;
			}
			if (!flag5 && !flag4 && !flag3)
			{
				tdelegate = default(TDelegate);
				return tdelegate;
			}
			ParameterExpression parameterExpression = (flag5 ? Expression.Parameter(parameters[0].ParameterType, "instance") : null);
			List<ParameterExpression> list = parameters.Skip<ParameterInfo>((flag5 > false) ? 1 : 0).Select<ParameterInfo, ParameterExpression>((ParameterInfo pi, int i) => Expression.Parameter(pi.ParameterType, string.Format("p{0}", i))).ToList<ParameterExpression>();
			List<Expression> list2 = list.Select<ParameterExpression, Expression>(delegate(ParameterExpression pe, int i)
			{
				if (pe.IsByRef || pe.Type.Equals(methodParameters[i].ParameterType))
				{
					return pe;
				}
				return Expression.Convert(pe, methodParameters[i].ParameterType);
			}).ToList<Expression>();
			MethodCallExpression methodCallExpression = (flag4 ? (instance.GetType().Equals(methodInfo.DeclaringType) ? Expression.Call(Expression.Constant(instance), methodInfo, list2) : Expression.Call(Expression.Convert(Expression.Constant(instance), instance.GetType()), methodInfo, list2)) : (flag3 ? Expression.Call(methodInfo, list2) : (flag5 ? (parameterExpression.Type.Equals(methodInfo.DeclaringType) ? Expression.Call(parameterExpression, methodInfo, list2) : Expression.Call(Expression.Convert(parameterExpression, methodInfo.DeclaringType), methodInfo, list2)) : null)));
			if (methodCallExpression == null)
			{
				tdelegate = default(TDelegate);
				return tdelegate;
			}
			UnaryExpression unaryExpression = Expression.Convert(methodCallExpression, method.ReturnType);
			try
			{
				Expression expression = unaryExpression;
				IEnumerable<ParameterExpression> enumerable2;
				if (!flag5)
				{
					IEnumerable<ParameterExpression> enumerable = list;
					enumerable2 = enumerable;
				}
				else
				{
					enumerable2 = new List<ParameterExpression> { parameterExpression }.Concat<ParameterExpression>(list);
				}
				tdelegate = Expression.Lambda<TDelegate>(expression, enumerable2).Compile();
			}
			catch (Exception ex)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError(string.Format("AccessTools2.GetDelegate<{0}>: Error while compiling lambds expression '{1}'", typeof(TDelegate).FullName, ex));
				}
				tdelegate = default(TDelegate);
			}
			return tdelegate;
		}

		// Token: 0x0600016D RID: 365 RVA: 0x0000A184 File Offset: 0x00008384
		[return: Nullable(2)]
		public static TDelegate GetDelegate<[Nullable(0)] TDelegate>(MethodInfo methodInfo, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			return AccessTools2.GetDelegate<TDelegate>(null, methodInfo, logErrorInTrace);
		}

		// Token: 0x0600016E RID: 366 RVA: 0x0000A18E File Offset: 0x0000838E
		[return: Nullable(2)]
		public static TDelegate GetDelegateObjectInstance<[Nullable(0)] TDelegate>(MethodInfo methodInfo, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			return AccessTools2.GetDelegate<TDelegate>(methodInfo, logErrorInTrace);
		}

		// Token: 0x0600016F RID: 367 RVA: 0x0000A197 File Offset: 0x00008397
		public static bool IsNumeric(this Type myType)
		{
			return AccessTools2.NumericTypes.Contains(myType);
		}

		// Token: 0x06000170 RID: 368 RVA: 0x0000A1A4 File Offset: 0x000083A4
		private static bool ParametersAreEqual(ParameterInfo[] delegateParameters, ParameterInfo[] methodParameters)
		{
			if (delegateParameters.Length - methodParameters.Length == 0)
			{
				for (int i = 0; i < methodParameters.Length; i++)
				{
					if (delegateParameters[i].ParameterType.IsByRef != methodParameters[i].ParameterType.IsByRef)
					{
						return false;
					}
					bool flag = delegateParameters[i].ParameterType.IsEnum || methodParameters[i].ParameterType.IsEnum;
					bool flag2 = delegateParameters[i].ParameterType.IsNumeric() || methodParameters[i].ParameterType.IsNumeric();
					if (!flag && !flag2 && !delegateParameters[i].ParameterType.IsAssignableFrom(methodParameters[i].ParameterType))
					{
						return false;
					}
				}
				return true;
			}
			if (delegateParameters.Length - methodParameters.Length == 1)
			{
				for (int j = 0; j < methodParameters.Length; j++)
				{
					if (delegateParameters[j + 1].ParameterType.IsByRef != methodParameters[j].ParameterType.IsByRef)
					{
						return false;
					}
					bool flag3 = delegateParameters[j + 1].ParameterType.IsEnum || methodParameters[j].ParameterType.IsEnum;
					bool flag4 = delegateParameters[j + 1].ParameterType.IsNumeric() || methodParameters[j].ParameterType.IsNumeric();
					if (!flag3 && !flag4 && !delegateParameters[j + 1].ParameterType.IsAssignableFrom(methodParameters[j].ParameterType))
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		// Token: 0x06000171 RID: 369 RVA: 0x0000A2FD File Offset: 0x000084FD
		[return: Nullable(2)]
		public static TDelegate GetDelegate<[Nullable(0)] TDelegate, [Nullable(2)] TInstance>(TInstance instance, MethodInfo methodInfo, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			return AccessTools2.GetDelegate<TDelegate>(instance, methodInfo, logErrorInTrace);
		}

		// Token: 0x06000172 RID: 370 RVA: 0x0000A30C File Offset: 0x0000850C
		[return: Nullable(2)]
		public static TDelegate GetDeclaredDelegateObjectInstance<[Nullable(0)] TDelegate>(Type type, string method, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, [Nullable(new byte[] { 2, 1 })] Type[] generics = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.DeclaredMethod(type, method, parameters, generics, logErrorInTrace);
			if (methodInfo == null)
			{
				return default(TDelegate);
			}
			return AccessTools2.GetDelegateObjectInstance<TDelegate>(methodInfo, logErrorInTrace);
		}

		// Token: 0x06000173 RID: 371 RVA: 0x0000A33C File Offset: 0x0000853C
		[return: Nullable(2)]
		public static TDelegate GetDelegateObjectInstance<[Nullable(0)] TDelegate>(Type type, string method, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, [Nullable(new byte[] { 2, 1 })] Type[] generics = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.Method(type, method, parameters, generics, logErrorInTrace);
			if (methodInfo == null)
			{
				return default(TDelegate);
			}
			return AccessTools2.GetDelegateObjectInstance<TDelegate>(methodInfo, logErrorInTrace);
		}

		// Token: 0x06000174 RID: 372 RVA: 0x0000A36C File Offset: 0x0000856C
		[return: Nullable(2)]
		public static TDelegate GetDeclaredDelegateObjectInstance<[Nullable(0)] TDelegate>(string typeSemicolonMethod, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, [Nullable(new byte[] { 2, 1 })] Type[] generics = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.DeclaredMethod(typeSemicolonMethod, parameters, generics, logErrorInTrace);
			if (methodInfo == null)
			{
				return default(TDelegate);
			}
			return AccessTools2.GetDelegateObjectInstance<TDelegate>(methodInfo, logErrorInTrace);
		}

		// Token: 0x06000175 RID: 373 RVA: 0x0000A398 File Offset: 0x00008598
		[return: Nullable(2)]
		public static TDelegate GetDelegateObjectInstance<[Nullable(0)] TDelegate>(string typeSemicolonMethod, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, [Nullable(new byte[] { 2, 1 })] Type[] generics = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.Method(typeSemicolonMethod, parameters, generics, logErrorInTrace);
			if (methodInfo == null)
			{
				return default(TDelegate);
			}
			return AccessTools2.GetDelegateObjectInstance<TDelegate>(methodInfo, logErrorInTrace);
		}

		// Token: 0x06000176 RID: 374 RVA: 0x0000A3C4 File Offset: 0x000085C4
		[return: Nullable(2)]
		public static TDelegate GetDeclaredDelegate<[Nullable(0)] TDelegate>(Type type, string method, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, [Nullable(new byte[] { 2, 1 })] Type[] generics = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.DeclaredMethod(type, method, parameters, generics, logErrorInTrace);
			if (methodInfo == null)
			{
				return default(TDelegate);
			}
			return AccessTools2.GetDelegate<TDelegate>(methodInfo, logErrorInTrace);
		}

		// Token: 0x06000177 RID: 375 RVA: 0x0000A3F4 File Offset: 0x000085F4
		[return: Nullable(2)]
		public static TDelegate GetDelegate<[Nullable(0)] TDelegate>(Type type, string method, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, [Nullable(new byte[] { 2, 1 })] Type[] generics = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.Method(type, method, parameters, generics, logErrorInTrace);
			if (methodInfo == null)
			{
				return default(TDelegate);
			}
			return AccessTools2.GetDelegate<TDelegate>(methodInfo, logErrorInTrace);
		}

		// Token: 0x06000178 RID: 376 RVA: 0x0000A424 File Offset: 0x00008624
		[return: Nullable(2)]
		public static TDelegate GetDeclaredDelegate<[Nullable(0)] TDelegate>(string typeSemicolonMethod, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, [Nullable(new byte[] { 2, 1 })] Type[] generics = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.DeclaredMethod(typeSemicolonMethod, parameters, generics, logErrorInTrace);
			if (methodInfo == null)
			{
				return default(TDelegate);
			}
			return AccessTools2.GetDelegate<TDelegate>(methodInfo, logErrorInTrace);
		}

		// Token: 0x06000179 RID: 377 RVA: 0x0000A450 File Offset: 0x00008650
		[return: Nullable(2)]
		public static TDelegate GetDelegate<[Nullable(0)] TDelegate>(string typeSemicolonMethod, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, [Nullable(new byte[] { 2, 1 })] Type[] generics = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.Method(typeSemicolonMethod, parameters, generics, logErrorInTrace);
			if (methodInfo == null)
			{
				return default(TDelegate);
			}
			return AccessTools2.GetDelegate<TDelegate>(methodInfo, logErrorInTrace);
		}

		// Token: 0x0600017A RID: 378 RVA: 0x0000A47C File Offset: 0x0000867C
		[return: Nullable(2)]
		public static TDelegate GetDeclaredDelegate<[Nullable(0)] TDelegate, [Nullable(2)] TInstance>(TInstance instance, string method, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, [Nullable(new byte[] { 2, 1 })] Type[] generics = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			if (instance != null)
			{
				MethodInfo methodInfo = AccessTools2.DeclaredMethod(instance.GetType(), method, parameters, generics, logErrorInTrace);
				if (methodInfo != null)
				{
					return AccessTools2.GetDelegate<TDelegate, TInstance>(instance, methodInfo, logErrorInTrace);
				}
			}
			return default(TDelegate);
		}

		// Token: 0x0600017B RID: 379 RVA: 0x0000A4C0 File Offset: 0x000086C0
		[return: Nullable(2)]
		public static TDelegate GetDelegate<[Nullable(0)] TDelegate, [Nullable(2)] TInstance>(TInstance instance, string method, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, [Nullable(new byte[] { 2, 1 })] Type[] generics = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			if (instance != null)
			{
				MethodInfo methodInfo = AccessTools2.Method(instance.GetType(), method, parameters, generics, logErrorInTrace);
				if (methodInfo != null)
				{
					return AccessTools2.GetDelegate<TDelegate, TInstance>(instance, methodInfo, logErrorInTrace);
				}
			}
			return default(TDelegate);
		}

		// Token: 0x0600017C RID: 380 RVA: 0x0000A504 File Offset: 0x00008704
		[return: Nullable(2)]
		public static TDelegate GetDeclaredDelegate<[Nullable(0)] TDelegate>([Nullable(2)] object instance, Type type, string method, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, [Nullable(new byte[] { 2, 1 })] Type[] generics = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.DeclaredMethod(type, method, parameters, generics, logErrorInTrace);
			if (methodInfo == null)
			{
				return default(TDelegate);
			}
			return AccessTools2.GetDelegate<TDelegate>(instance, methodInfo, logErrorInTrace);
		}

		// Token: 0x0600017D RID: 381 RVA: 0x0000A534 File Offset: 0x00008734
		[return: Nullable(2)]
		public static TDelegate GetDelegate<[Nullable(0)] TDelegate>([Nullable(2)] object instance, Type type, string method, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, [Nullable(new byte[] { 2, 1 })] Type[] generics = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.Method(type, method, parameters, generics, logErrorInTrace);
			if (methodInfo == null)
			{
				return default(TDelegate);
			}
			return AccessTools2.GetDelegate<TDelegate>(instance, methodInfo, logErrorInTrace);
		}

		// Token: 0x0600017E RID: 382 RVA: 0x0000A564 File Offset: 0x00008764
		[return: Nullable(2)]
		public static TDelegate GetDeclaredDelegate<[Nullable(0)] TDelegate>([Nullable(2)] object instance, string typeSemicolonMethod, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, [Nullable(new byte[] { 2, 1 })] Type[] generics = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.DeclaredMethod(typeSemicolonMethod, parameters, generics, logErrorInTrace);
			if (methodInfo == null)
			{
				return default(TDelegate);
			}
			return AccessTools2.GetDelegate<TDelegate>(instance, methodInfo, logErrorInTrace);
		}

		// Token: 0x0600017F RID: 383 RVA: 0x0000A594 File Offset: 0x00008794
		[return: Nullable(2)]
		public static TDelegate GetDelegate<[Nullable(0)] TDelegate>([Nullable(2)] object instance, string typeSemicolonMethod, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, [Nullable(new byte[] { 2, 1 })] Type[] generics = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.Method(typeSemicolonMethod, parameters, generics, logErrorInTrace);
			if (methodInfo == null)
			{
				return default(TDelegate);
			}
			return AccessTools2.GetDelegate<TDelegate>(instance, methodInfo, logErrorInTrace);
		}

		// Token: 0x06000180 RID: 384 RVA: 0x0000A5C4 File Offset: 0x000087C4
		[return: Nullable(2)]
		public static FieldInfo DeclaredField(Type type, string name, bool logErrorInTrace = true)
		{
			if (type == null)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.DeclaredField: 'type' is null");
				}
				return null;
			}
			if (name == null)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError(string.Format("AccessTools2.DeclaredField: type '{0}', 'name' is null", type));
				}
				return null;
			}
			FieldInfo field = type.GetField(name, AccessTools.allDeclared);
			if (field == null)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError(string.Format("AccessTools2.DeclaredField: Could not find field for type '{0}' and name '{1}'", type, name));
				}
				return null;
			}
			return field;
		}

		// Token: 0x06000181 RID: 385 RVA: 0x0000A624 File Offset: 0x00008824
		[return: Nullable(2)]
		public static FieldInfo Field(Type type, string name, bool logErrorInTrace = true)
		{
			if (type == null)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.Field: 'type' is null");
				}
				return null;
			}
			if (name == null)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError(string.Format("AccessTools2.Field: type '{0}', 'name' is null", type));
				}
				return null;
			}
			FieldInfo fieldInfo = AccessTools2.FindIncludingBaseTypes<FieldInfo>(type, (Type t) => t.GetField(name, AccessTools.all));
			if (fieldInfo == null && logErrorInTrace)
			{
				Trace.TraceError(string.Format("AccessTools2.Field: Could not find field for type '{0}' and name '{1}'", type, name));
			}
			return fieldInfo;
		}

		// Token: 0x06000182 RID: 386 RVA: 0x0000A6A0 File Offset: 0x000088A0
		[return: Nullable(2)]
		public static FieldInfo DeclaredField(string typeColonFieldname, bool logErrorInTrace = true)
		{
			Type type;
			string text;
			if (!AccessTools2.TryGetComponents(typeColonFieldname, out type, out text, logErrorInTrace))
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.Field: Could not find type or field for '" + typeColonFieldname + "'");
				}
				return null;
			}
			return AccessTools2.DeclaredField(type, text, logErrorInTrace);
		}

		// Token: 0x06000183 RID: 387 RVA: 0x0000A6DC File Offset: 0x000088DC
		[return: Nullable(2)]
		public static FieldInfo Field(string typeColonFieldname, bool logErrorInTrace = true)
		{
			Type type;
			string text;
			if (!AccessTools2.TryGetComponents(typeColonFieldname, out type, out text, logErrorInTrace))
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.Field: Could not find type or field for '" + typeColonFieldname + "'");
				}
				return null;
			}
			return AccessTools2.Field(type, text, logErrorInTrace);
		}

		// Token: 0x06000184 RID: 388 RVA: 0x0000A718 File Offset: 0x00008918
		[return: Nullable(new byte[] { 2, 1, 1 })]
		public static AccessTools.FieldRef<object, F> FieldRefAccess<[Nullable(2)] F>(string typeColonFieldname, bool logErrorInTrace = true)
		{
			Type type;
			string text;
			if (!AccessTools2.TryGetComponents(typeColonFieldname, out type, out text, logErrorInTrace))
			{
				Trace.TraceError("AccessTools2.FieldRefAccess: Could not find type or field for '" + typeColonFieldname + "'");
				return null;
			}
			return AccessTools2.FieldRefAccess<F>(type, text, logErrorInTrace);
		}

		// Token: 0x06000185 RID: 389 RVA: 0x0000A754 File Offset: 0x00008954
		[return: Nullable(new byte[] { 2, 1, 1 })]
		public static AccessTools.FieldRef<T, F> FieldRefAccess<T, [Nullable(2)] F>(string fieldName, bool logErrorInTrace = true) where T : class
		{
			if (fieldName == null)
			{
				return null;
			}
			FieldInfo instanceField = AccessTools2.GetInstanceField(typeof(T), fieldName, logErrorInTrace);
			if (instanceField == null)
			{
				return null;
			}
			return AccessTools2.FieldRefAccessInternal<T, F>(instanceField, false, logErrorInTrace);
		}

		// Token: 0x06000186 RID: 390 RVA: 0x0000A788 File Offset: 0x00008988
		[return: Nullable(new byte[] { 2, 1, 1 })]
		public static AccessTools.FieldRef<object, F> FieldRefAccess<[Nullable(2)] F>(Type type, string fieldName, bool logErrorInTrace = true)
		{
			if (type == null)
			{
				return null;
			}
			if (fieldName == null)
			{
				return null;
			}
			FieldInfo fieldInfo = AccessTools2.Field(type, fieldName, logErrorInTrace);
			if (fieldInfo == null)
			{
				return null;
			}
			if (!fieldInfo.IsStatic)
			{
				Type declaringType = fieldInfo.DeclaringType;
				if (declaringType != null)
				{
					if (declaringType.IsValueType)
					{
						if (logErrorInTrace)
						{
							Trace.TraceError("AccessTools2.FieldRefAccess<object, " + typeof(F).FullName + ">: FieldDeclaringType must be a class");
						}
						return null;
					}
					return AccessTools2.FieldRefAccessInternal<object, F>(fieldInfo, true, logErrorInTrace);
				}
			}
			return null;
		}

		// Token: 0x06000187 RID: 391 RVA: 0x0000A7FC File Offset: 0x000089FC
		[return: Nullable(new byte[] { 2, 1, 1 })]
		public static AccessTools.FieldRef<T, F> FieldRefAccess<T, [Nullable(2)] F>(FieldInfo fieldInfo, bool logErrorInTrace = true) where T : class
		{
			if (fieldInfo == null)
			{
				return null;
			}
			if (!fieldInfo.IsStatic)
			{
				Type declaringType = fieldInfo.DeclaringType;
				if (declaringType != null)
				{
					if (declaringType.IsValueType)
					{
						if (logErrorInTrace)
						{
							Trace.TraceError(string.Concat(new string[]
							{
								"AccessTools2.FieldRefAccess<",
								typeof(T).FullName,
								", ",
								typeof(F).FullName,
								">: FieldDeclaringType must be a class"
							}));
						}
						return null;
					}
					bool? flag = AccessTools2.FieldRefNeedsClasscast(typeof(T), declaringType, logErrorInTrace);
					if (flag != null)
					{
						bool valueOrDefault = flag.GetValueOrDefault();
						return AccessTools2.FieldRefAccessInternal<T, F>(fieldInfo, valueOrDefault, logErrorInTrace);
					}
					return null;
				}
			}
			return null;
		}

		// Token: 0x06000188 RID: 392 RVA: 0x0000A8B0 File Offset: 0x00008AB0
		[return: Nullable(new byte[] { 2, 1, 1 })]
		private static AccessTools.FieldRef<T, F> FieldRefAccessInternal<T, [Nullable(2)] F>(FieldInfo fieldInfo, bool needCastclass, bool logErrorInTrace = true) where T : class
		{
			if (!AccessTools2.Helper.IsValid(logErrorInTrace))
			{
				return null;
			}
			if (fieldInfo.IsStatic)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError(string.Concat(new string[]
					{
						"AccessTools2.FieldRefAccessInternal<",
						typeof(T).FullName,
						", ",
						typeof(F).FullName,
						">: Field must not be static"
					}));
				}
				return null;
			}
			if (!AccessTools2.ValidateFieldType<F>(fieldInfo, logErrorInTrace))
			{
				return null;
			}
			Type typeFromHandle = typeof(T);
			Type declaringType = fieldInfo.DeclaringType;
			AccessTools2.DynamicMethodDefinitionHandle? dynamicMethodDefinitionHandle = AccessTools2.DynamicMethodDefinitionHandle.Create("__refget_" + typeFromHandle.Name + "_fi_" + fieldInfo.Name, typeof(F).MakeByRefType(), new Type[] { typeFromHandle });
			AccessTools2.ILGeneratorHandle? ilgeneratorHandle = ((dynamicMethodDefinitionHandle != null) ? dynamicMethodDefinitionHandle.GetValueOrDefault().GetILGenerator() : null);
			if (ilgeneratorHandle != null)
			{
				AccessTools2.ILGeneratorHandle valueOrDefault = ilgeneratorHandle.GetValueOrDefault();
				valueOrDefault.Emit(OpCodes.Ldarg_0);
				if (needCastclass)
				{
					valueOrDefault.Emit(OpCodes.Castclass, declaringType);
				}
				valueOrDefault.Emit(OpCodes.Ldflda, fieldInfo);
				valueOrDefault.Emit(OpCodes.Ret);
				object obj;
				if (dynamicMethodDefinitionHandle == null)
				{
					obj = null;
				}
				else
				{
					MethodInfo methodInfo = dynamicMethodDefinitionHandle.GetValueOrDefault().Generate();
					obj = ((methodInfo != null) ? methodInfo.CreateDelegate(typeof(AccessTools.FieldRef<T, F>)) : null);
				}
				return obj as AccessTools.FieldRef<T, F>;
			}
			return null;
		}

		// Token: 0x06000189 RID: 393 RVA: 0x0000AA20 File Offset: 0x00008C20
		private static bool? FieldRefNeedsClasscast(Type delegateInstanceType, Type declaringType, bool logErrorInTrace = true)
		{
			bool flag = false;
			if (delegateInstanceType != declaringType)
			{
				flag = delegateInstanceType.IsAssignableFrom(declaringType);
				if (!flag && !declaringType.IsAssignableFrom(delegateInstanceType))
				{
					if (logErrorInTrace)
					{
						Trace.TraceError(string.Format("AccessTools2.FieldRefNeedsClasscast: FieldDeclaringType must be assignable from or to T (FieldRefAccess instance type) - 'instanceOfT is FieldDeclaringType' must be possible, delegateInstanceType '{0}', declaringType '{1}'", delegateInstanceType, declaringType));
					}
					return null;
				}
			}
			return new bool?(flag);
		}

		// Token: 0x0600018A RID: 394 RVA: 0x0000AA70 File Offset: 0x00008C70
		[return: Nullable(new byte[] { 2, 1, 1 })]
		public static AccessTools.FieldRef<object, TField> FieldRefAccess<[Nullable(2)] TField>(FieldInfo fieldInfo)
		{
			if (fieldInfo != null)
			{
				return AccessTools.FieldRefAccess<object, TField>(fieldInfo);
			}
			return null;
		}

		// Token: 0x0600018B RID: 395 RVA: 0x0000AA80 File Offset: 0x00008C80
		[return: Nullable(2)]
		public static MethodInfo DeclaredMethod(Type type, string name, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, [Nullable(new byte[] { 2, 1 })] Type[] generics = null, bool logErrorInTrace = true)
		{
			if (type == null)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.DeclaredMethod: 'type' is null");
				}
				return null;
			}
			if (name == null)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError(string.Format("AccessTools2.DeclaredMethod: type '{0}', 'name' is null", type));
				}
				return null;
			}
			MethodInfo methodInfo;
			if (parameters == null)
			{
				try
				{
					methodInfo = type.GetMethod(name, AccessTools.allDeclared);
					goto IL_00AA;
				}
				catch (AmbiguousMatchException ex)
				{
					methodInfo = type.GetMethod(name, AccessTools.allDeclared, null, Type.EmptyTypes, new ParameterModifier[0]);
					if (methodInfo == null)
					{
						if (logErrorInTrace)
						{
							Trace.TraceError(string.Format("AccessTools2.DeclaredMethod: Ambiguous match for type '{0}' and name '{1}' and parameters '{2}', '{3}'", new object[]
							{
								type,
								name,
								(parameters != null) ? GeneralExtensions.Description(parameters) : null,
								ex
							}));
						}
						return null;
					}
					goto IL_00AA;
				}
			}
			methodInfo = type.GetMethod(name, AccessTools.allDeclared, null, parameters, new ParameterModifier[0]);
			IL_00AA:
			if (methodInfo == null)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError(string.Format("AccessTools2.DeclaredMethod: Could not find method for type '{0}' and name '{1}' and parameters '{2}'", type, name, (parameters != null) ? GeneralExtensions.Description(parameters) : null));
				}
				return null;
			}
			if (generics != null)
			{
				methodInfo = methodInfo.MakeGenericMethod(generics);
			}
			return methodInfo;
		}

		// Token: 0x0600018C RID: 396 RVA: 0x0000AB7C File Offset: 0x00008D7C
		[return: Nullable(2)]
		public static MethodInfo Method(Type type, string name, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, [Nullable(new byte[] { 2, 1 })] Type[] generics = null, bool logErrorInTrace = true)
		{
			if (type == null)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.Method: 'type' is null");
				}
				return null;
			}
			if (name == null)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError(string.Format("AccessTools2.Method: type '{0}', 'name' is null", type));
				}
				return null;
			}
			MethodInfo methodInfo;
			if (parameters == null)
			{
				try
				{
					methodInfo = AccessTools2.FindIncludingBaseTypes<MethodInfo>(type, (Type t) => t.GetMethod(name, AccessTools.all));
					goto IL_00D1;
				}
				catch (AmbiguousMatchException ex)
				{
					methodInfo = AccessTools2.FindIncludingBaseTypes<MethodInfo>(type, (Type t) => t.GetMethod(name, AccessTools.all, null, Type.EmptyTypes, new ParameterModifier[0]));
					if (methodInfo == null)
					{
						if (logErrorInTrace)
						{
							string text = "AccessTools2.Method: Ambiguous match for type '{0}' and name '{1}' and parameters '{2}', '{3}'";
							object[] array = new object[4];
							array[0] = type;
							array[1] = name;
							int num = 2;
							Type[] parameters2 = parameters;
							array[num] = ((parameters2 != null) ? GeneralExtensions.Description(parameters2) : null);
							array[3] = ex;
							Trace.TraceError(string.Format(text, array));
						}
						return null;
					}
					goto IL_00D1;
				}
			}
			methodInfo = AccessTools2.FindIncludingBaseTypes<MethodInfo>(type, (Type t) => t.GetMethod(name, AccessTools.all, null, parameters, new ParameterModifier[0]));
			IL_00D1:
			if (methodInfo == null)
			{
				if (logErrorInTrace)
				{
					string text2 = "AccessTools2.Method: Could not find method for type '{0}' and name '{1}' and parameters '{2}'";
					object name2 = name;
					Type[] parameters3 = parameters;
					Trace.TraceError(string.Format(text2, type, name2, (parameters3 != null) ? GeneralExtensions.Description(parameters3) : null));
				}
				return null;
			}
			if (generics != null)
			{
				methodInfo = methodInfo.MakeGenericMethod(generics);
			}
			return methodInfo;
		}

		// Token: 0x0600018D RID: 397 RVA: 0x0000ACAC File Offset: 0x00008EAC
		[return: Nullable(2)]
		public static MethodInfo DeclaredMethod(string typeColonMethodname, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, [Nullable(new byte[] { 2, 1 })] Type[] generics = null, bool logErrorInTrace = true)
		{
			Type type;
			string text;
			if (!AccessTools2.TryGetComponents(typeColonMethodname, out type, out text, logErrorInTrace))
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.Method: Could not find type or property for '" + typeColonMethodname + "'");
				}
				return null;
			}
			return AccessTools2.DeclaredMethod(type, text, parameters, generics, logErrorInTrace);
		}

		// Token: 0x0600018E RID: 398 RVA: 0x0000ACEC File Offset: 0x00008EEC
		[return: Nullable(2)]
		public static MethodInfo Method(string typeColonMethodname, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, [Nullable(new byte[] { 2, 1 })] Type[] generics = null, bool logErrorInTrace = true)
		{
			Type type;
			string text;
			if (!AccessTools2.TryGetComponents(typeColonMethodname, out type, out text, logErrorInTrace))
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.Method: Could not find type or property for '" + typeColonMethodname + "'");
				}
				return null;
			}
			return AccessTools2.Method(type, text, parameters, generics, logErrorInTrace);
		}

		// Token: 0x0600018F RID: 399 RVA: 0x0000AD2C File Offset: 0x00008F2C
		[return: Nullable(2)]
		public static PropertyInfo DeclaredProperty(Type type, string name, bool logErrorInTrace = true)
		{
			if (type == null)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.DeclaredProperty: 'type' is null");
				}
				return null;
			}
			if (name == null)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError(string.Format("AccessTools2.DeclaredProperty: type '{0}', 'name' is null", type));
				}
				return null;
			}
			PropertyInfo property = type.GetProperty(name, AccessTools.allDeclared);
			if (property == null && logErrorInTrace)
			{
				Trace.TraceError(string.Format("AccessTools2.DeclaredProperty: Could not find property for type '{0}' and name '{1}'", type, name));
			}
			return property;
		}

		// Token: 0x06000190 RID: 400 RVA: 0x0000AD8C File Offset: 0x00008F8C
		[return: Nullable(2)]
		public static PropertyInfo Property(Type type, string name, bool logErrorInTrace = true)
		{
			if (type == null)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.Property: 'type' is null");
				}
				return null;
			}
			if (name == null)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError(string.Format("AccessTools2.Property: type '{0}', 'name' is null", type));
				}
				return null;
			}
			PropertyInfo propertyInfo = AccessTools2.FindIncludingBaseTypes<PropertyInfo>(type, (Type t) => t.GetProperty(name, AccessTools.all));
			if (propertyInfo == null && logErrorInTrace)
			{
				Trace.TraceError(string.Format("AccessTools2.Property: Could not find property for type '{0}' and name '{1}'", type, name));
			}
			return propertyInfo;
		}

		// Token: 0x06000191 RID: 401 RVA: 0x0000AE07 File Offset: 0x00009007
		[return: Nullable(2)]
		public static MethodInfo DeclaredPropertyGetter(Type type, string name, bool logErrorInTrace = true)
		{
			PropertyInfo propertyInfo = AccessTools2.DeclaredProperty(type, name, logErrorInTrace);
			if (propertyInfo == null)
			{
				return null;
			}
			return propertyInfo.GetGetMethod(true);
		}

		// Token: 0x06000192 RID: 402 RVA: 0x0000AE1D File Offset: 0x0000901D
		[return: Nullable(2)]
		public static MethodInfo DeclaredPropertySetter(Type type, string name, bool logErrorInTrace = true)
		{
			PropertyInfo propertyInfo = AccessTools2.DeclaredProperty(type, name, logErrorInTrace);
			if (propertyInfo == null)
			{
				return null;
			}
			return propertyInfo.GetSetMethod(true);
		}

		// Token: 0x06000193 RID: 403 RVA: 0x0000AE33 File Offset: 0x00009033
		[return: Nullable(2)]
		public static MethodInfo PropertyGetter(Type type, string name, bool logErrorInTrace = true)
		{
			PropertyInfo propertyInfo = AccessTools2.Property(type, name, logErrorInTrace);
			if (propertyInfo == null)
			{
				return null;
			}
			return propertyInfo.GetGetMethod(true);
		}

		// Token: 0x06000194 RID: 404 RVA: 0x0000AE49 File Offset: 0x00009049
		[return: Nullable(2)]
		public static MethodInfo PropertySetter(Type type, string name, bool logErrorInTrace = true)
		{
			PropertyInfo propertyInfo = AccessTools2.Property(type, name, logErrorInTrace);
			if (propertyInfo == null)
			{
				return null;
			}
			return propertyInfo.GetSetMethod(true);
		}

		// Token: 0x06000195 RID: 405 RVA: 0x0000AE60 File Offset: 0x00009060
		[return: Nullable(2)]
		public static PropertyInfo DeclaredProperty(string typeColonPropertyName, bool logErrorInTrace = true)
		{
			Type type;
			string text;
			if (!AccessTools2.TryGetComponents(typeColonPropertyName, out type, out text, logErrorInTrace))
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.DeclaredProperty: Could not find type or property for '" + typeColonPropertyName + "'");
				}
				return null;
			}
			return AccessTools2.DeclaredProperty(type, text, logErrorInTrace);
		}

		// Token: 0x06000196 RID: 406 RVA: 0x0000AE9C File Offset: 0x0000909C
		[return: Nullable(2)]
		public static PropertyInfo Property(string typeColonPropertyName, bool logErrorInTrace = true)
		{
			Type type;
			string text;
			if (!AccessTools2.TryGetComponents(typeColonPropertyName, out type, out text, logErrorInTrace))
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.Property: Could not find type or property for '" + typeColonPropertyName + "'");
				}
				return null;
			}
			return AccessTools2.Property(type, text, logErrorInTrace);
		}

		// Token: 0x06000197 RID: 407 RVA: 0x0000AED8 File Offset: 0x000090D8
		[return: Nullable(2)]
		public static MethodInfo DeclaredPropertySetter(string typeColonPropertyName, bool logErrorInTrace = true)
		{
			PropertyInfo propertyInfo = AccessTools2.DeclaredProperty(typeColonPropertyName, logErrorInTrace);
			if (propertyInfo == null)
			{
				return null;
			}
			return propertyInfo.GetSetMethod(true);
		}

		// Token: 0x06000198 RID: 408 RVA: 0x0000AEED File Offset: 0x000090ED
		[return: Nullable(2)]
		public static MethodInfo DeclaredPropertyGetter(string typeColonPropertyName, bool logErrorInTrace = true)
		{
			PropertyInfo propertyInfo = AccessTools2.DeclaredProperty(typeColonPropertyName, logErrorInTrace);
			if (propertyInfo == null)
			{
				return null;
			}
			return propertyInfo.GetGetMethod(true);
		}

		// Token: 0x06000199 RID: 409 RVA: 0x0000AF02 File Offset: 0x00009102
		[return: Nullable(2)]
		public static MethodInfo PropertyGetter(string typeColonPropertyName, bool logErrorInTrace = true)
		{
			PropertyInfo propertyInfo = AccessTools2.Property(typeColonPropertyName, logErrorInTrace);
			if (propertyInfo == null)
			{
				return null;
			}
			return propertyInfo.GetGetMethod(true);
		}

		// Token: 0x0600019A RID: 410 RVA: 0x0000AF17 File Offset: 0x00009117
		[return: Nullable(2)]
		public static MethodInfo PropertySetter(string typeColonPropertyName, bool logErrorInTrace = true)
		{
			PropertyInfo propertyInfo = AccessTools2.Property(typeColonPropertyName, logErrorInTrace);
			if (propertyInfo == null)
			{
				return null;
			}
			return propertyInfo.GetSetMethod(true);
		}

		// Token: 0x0600019B RID: 411 RVA: 0x0000AF2C File Offset: 0x0000912C
		[return: Nullable(new byte[] { 2, 1 })]
		public static AccessTools.FieldRef<TField> StaticFieldRefAccess<[Nullable(2)] TField>(string typeColonFieldname, bool logErrorInTrace = true)
		{
			Type type;
			string text;
			if (!AccessTools2.TryGetComponents(typeColonFieldname, out type, out text, logErrorInTrace))
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.StaticFieldRefAccess: Could not find type or field for '" + typeColonFieldname + "'");
				}
				return null;
			}
			return AccessTools2.StaticFieldRefAccess<TField>(type, text, logErrorInTrace);
		}

		// Token: 0x0600019C RID: 412 RVA: 0x0000AF68 File Offset: 0x00009168
		[return: Nullable(new byte[] { 2, 1 })]
		public static AccessTools.FieldRef<F> StaticFieldRefAccess<[Nullable(2)] F>(FieldInfo fieldInfo, bool logErrorInTrace = true)
		{
			if (fieldInfo == null)
			{
				return null;
			}
			return AccessTools2.StaticFieldRefAccessInternal<F>(fieldInfo, logErrorInTrace);
		}

		// Token: 0x0600019D RID: 413 RVA: 0x0000AF78 File Offset: 0x00009178
		[return: Nullable(new byte[] { 2, 1 })]
		public static AccessTools.FieldRef<TField> StaticFieldRefAccess<[Nullable(2)] TField>(Type type, string fieldName, bool logErrorInTrace = true)
		{
			FieldInfo fieldInfo = AccessTools2.Field(type, fieldName, logErrorInTrace);
			if (fieldInfo == null)
			{
				return null;
			}
			return AccessTools2.StaticFieldRefAccess<TField>(fieldInfo, logErrorInTrace);
		}

		// Token: 0x0600019E RID: 414 RVA: 0x0000AF9C File Offset: 0x0000919C
		[return: Nullable(new byte[] { 2, 1 })]
		private static AccessTools.FieldRef<F> StaticFieldRefAccessInternal<[Nullable(2)] F>(FieldInfo fieldInfo, bool logErrorInTrace = true)
		{
			if (!AccessTools2.Helper.IsValid(logErrorInTrace))
			{
				return null;
			}
			if (!fieldInfo.IsStatic)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.StaticFieldRefAccessInternal<" + typeof(F).FullName + ">: Field must be static");
				}
				return null;
			}
			if (!AccessTools2.ValidateFieldType<F>(fieldInfo, logErrorInTrace))
			{
				return null;
			}
			string text = "__refget_";
			Type declaringType = fieldInfo.DeclaringType;
			AccessTools2.DynamicMethodDefinitionHandle? dynamicMethodDefinitionHandle = AccessTools2.DynamicMethodDefinitionHandle.Create(text + (((declaringType != null) ? declaringType.Name : null) ?? "null") + "_static_fi_" + fieldInfo.Name, typeof(F).MakeByRefType(), new Type[0]);
			AccessTools2.ILGeneratorHandle? ilgeneratorHandle = ((dynamicMethodDefinitionHandle != null) ? dynamicMethodDefinitionHandle.GetValueOrDefault().GetILGenerator() : null);
			if (ilgeneratorHandle != null)
			{
				AccessTools2.ILGeneratorHandle valueOrDefault = ilgeneratorHandle.GetValueOrDefault();
				valueOrDefault.Emit(OpCodes.Ldsflda, fieldInfo);
				valueOrDefault.Emit(OpCodes.Ret);
				object obj;
				if (dynamicMethodDefinitionHandle == null)
				{
					obj = null;
				}
				else
				{
					MethodInfo methodInfo = dynamicMethodDefinitionHandle.GetValueOrDefault().Generate();
					obj = ((methodInfo != null) ? methodInfo.CreateDelegate(typeof(AccessTools.FieldRef<F>)) : null);
				}
				return obj as AccessTools.FieldRef<F>;
			}
			return null;
		}

		// Token: 0x0600019F RID: 415 RVA: 0x0000B0C4 File Offset: 0x000092C4
		[NullableContext(0)]
		[return: Nullable(new byte[] { 2, 0, 1 })]
		public static AccessTools.StructFieldRef<T, F> StructFieldRefAccess<T, [Nullable(2)] F>([Nullable(1)] string fieldName, bool logErrorInTrace = true) where T : struct
		{
			if (string.IsNullOrEmpty(fieldName))
			{
				return null;
			}
			FieldInfo instanceField = AccessTools2.GetInstanceField(typeof(T), fieldName, logErrorInTrace);
			if (instanceField == null)
			{
				return null;
			}
			return AccessTools2.StructFieldRefAccessInternal<T, F>(instanceField, logErrorInTrace);
		}

		// Token: 0x060001A0 RID: 416 RVA: 0x0000B0F9 File Offset: 0x000092F9
		[NullableContext(2)]
		[return: Nullable(new byte[] { 2, 0, 1 })]
		public static AccessTools.StructFieldRef<T, F> StructFieldRefAccess<[Nullable(0)] T, F>(FieldInfo fieldInfo, bool logErrorInTrace = true) where T : struct
		{
			if (fieldInfo == null)
			{
				return null;
			}
			if (!AccessTools2.ValidateStructField<T, F>(fieldInfo, logErrorInTrace))
			{
				return null;
			}
			return AccessTools2.StructFieldRefAccessInternal<T, F>(fieldInfo, logErrorInTrace);
		}

		// Token: 0x060001A1 RID: 417 RVA: 0x0000B114 File Offset: 0x00009314
		[NullableContext(0)]
		[return: Nullable(new byte[] { 2, 0, 1 })]
		private static AccessTools.StructFieldRef<T, F> StructFieldRefAccessInternal<T, [Nullable(2)] F>([Nullable(1)] FieldInfo fieldInfo, bool logErrorInTrace = true) where T : struct
		{
			if (!AccessTools2.ValidateFieldType<F>(fieldInfo, logErrorInTrace))
			{
				return null;
			}
			AccessTools2.DynamicMethodDefinitionHandle? dynamicMethodDefinitionHandle = AccessTools2.DynamicMethodDefinitionHandle.Create("__refget_" + typeof(T).Name + "_struct_fi_" + fieldInfo.Name, typeof(F).MakeByRefType(), new Type[] { typeof(T).MakeByRefType() });
			AccessTools2.ILGeneratorHandle? ilgeneratorHandle = ((dynamicMethodDefinitionHandle != null) ? dynamicMethodDefinitionHandle.GetValueOrDefault().GetILGenerator() : null);
			if (ilgeneratorHandle != null)
			{
				AccessTools2.ILGeneratorHandle valueOrDefault = ilgeneratorHandle.GetValueOrDefault();
				valueOrDefault.Emit(OpCodes.Ldarg_0);
				valueOrDefault.Emit(OpCodes.Ldflda, fieldInfo);
				valueOrDefault.Emit(OpCodes.Ret);
				object obj;
				if (dynamicMethodDefinitionHandle == null)
				{
					obj = null;
				}
				else
				{
					MethodInfo methodInfo = dynamicMethodDefinitionHandle.GetValueOrDefault().Generate();
					obj = ((methodInfo != null) ? methodInfo.CreateDelegate(typeof(AccessTools.StructFieldRef<T, F>)) : null);
				}
				return obj as AccessTools.StructFieldRef<T, F>;
			}
			return null;
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x0000B213 File Offset: 0x00009413
		public static IEnumerable<Assembly> AllAssemblies()
		{
			return from a in AppDomain.CurrentDomain.GetAssemblies()
				where !a.FullName.StartsWith("Microsoft.VisualStudio")
				select a;
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x0000B243 File Offset: 0x00009443
		public static IEnumerable<Type> AllTypes()
		{
			return AccessTools2.AllAssemblies().SelectMany<Assembly, Type>((Assembly a) => AccessTools2.GetTypesFromAssembly(a, true));
		}

		// Token: 0x060001A4 RID: 420 RVA: 0x0000B270 File Offset: 0x00009470
		public static Type[] GetTypesFromAssembly(Assembly assembly, bool logErrorInTrace = true)
		{
			if (assembly == null)
			{
				return Type.EmptyTypes;
			}
			Type[] array;
			try
			{
				array = assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException ex)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError(string.Format("AccessTools2.GetTypesFromAssembly: assembly {0} => {1}", assembly, ex));
				}
				array = ex.Types.Where<Type>((Type type) => type != null).ToArray<Type>();
			}
			return array;
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x0000B2E8 File Offset: 0x000094E8
		public static Type[] GetTypesFromAssemblyIfValid(Assembly assembly, bool logErrorInTrace = true)
		{
			if (assembly == null)
			{
				return Type.EmptyTypes;
			}
			Type[] array;
			try
			{
				array = assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException ex)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError(string.Format("AccessTools2.GetTypesFromAssemblyIfValid: assembly {0} => {1}", assembly, ex));
				}
				array = Type.EmptyTypes;
			}
			return array;
		}

		// Token: 0x060001A6 RID: 422 RVA: 0x0000B338 File Offset: 0x00009538
		[return: Nullable(2)]
		public static Type TypeByName(string name, bool logErrorInTrace = true)
		{
			if (string.IsNullOrEmpty(name))
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.TypeByName: 'name' is null or empty");
				}
				return null;
			}
			Type type = Type.GetType(name, false);
			if (type == null)
			{
				type = AccessTools2.AllTypes().FirstOrDefault<Type>((Type t) => t.FullName == name);
			}
			if (type == null)
			{
				type = AccessTools2.AllTypes().FirstOrDefault<Type>((Type t) => t.Name == name);
			}
			if (type == null && logErrorInTrace)
			{
				Trace.TraceError("AccessTools2.TypeByName: Could not find type named '" + name + "'");
			}
			return type;
		}

		// Token: 0x060001A7 RID: 423 RVA: 0x0000B3D4 File Offset: 0x000095D4
		[return: Nullable(2)]
		public static T FindIncludingBaseTypes<T>(Type type, Func<Type, T> func) where T : class
		{
			if (type == null || func == null)
			{
				return default(T);
			}
			T t;
			for (;;)
			{
				t = func(type);
				if (t != null)
				{
					break;
				}
				type = type.BaseType;
				if (type == null)
				{
					goto Block_3;
				}
			}
			return t;
			Block_3:
			return default(T);
		}

		// Token: 0x060001A8 RID: 424 RVA: 0x0000B418 File Offset: 0x00009618
		[return: Nullable(2)]
		private static FieldInfo GetInstanceField(Type type, string fieldName, bool logErrorInTrace = true)
		{
			FieldInfo fieldInfo = AccessTools2.Field(type, fieldName, logErrorInTrace);
			if (fieldInfo == null)
			{
				return null;
			}
			if (fieldInfo.IsStatic)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError(string.Format("AccessTools2.GetInstanceField: Field must not be static, type '{0}', fieldName '{1}'", type, fieldName));
				}
				return null;
			}
			return fieldInfo;
		}

		// Token: 0x060001A9 RID: 425 RVA: 0x0000B454 File Offset: 0x00009654
		[NullableContext(2)]
		private static bool ValidateFieldType<F>(FieldInfo fieldInfo, bool logErrorInTrace = true)
		{
			if (fieldInfo == null)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.ValidateFieldType<" + typeof(F).FullName + ">: 'fieldInfo' is null");
				}
				return false;
			}
			Type typeFromHandle = typeof(F);
			Type fieldType = fieldInfo.FieldType;
			if (typeFromHandle == fieldType)
			{
				return true;
			}
			if (fieldType.IsEnum)
			{
				Type underlyingType = Enum.GetUnderlyingType(fieldType);
				if (typeFromHandle != underlyingType)
				{
					if (logErrorInTrace)
					{
						Trace.TraceError(string.Format("AccessTools2.ValidateFieldType<{0}>: FieldRefAccess return type must be the same as FieldType or FieldType's underlying integral type ({1}) for enum types, fieldInfo '{2}'", typeof(F).FullName, underlyingType, fieldInfo));
					}
					return false;
				}
			}
			else
			{
				if (fieldType.IsValueType)
				{
					if (logErrorInTrace)
					{
						Trace.TraceError(string.Format("AccessTools2.ValidateFieldType<{0}>: FieldRefAccess return type must be the same as FieldType for value types, fieldInfo '{1}'", typeof(F).FullName, fieldInfo));
					}
					return false;
				}
				if (!typeFromHandle.IsAssignableFrom(fieldType))
				{
					if (logErrorInTrace)
					{
						Trace.TraceError("AccessTools2.ValidateFieldType<" + typeof(F).FullName + ">: FieldRefAccess return type must be assignable from FieldType for reference types");
					}
					return false;
				}
			}
			return true;
		}

		// Token: 0x060001AA RID: 426 RVA: 0x0000B548 File Offset: 0x00009748
		[NullableContext(2)]
		private static bool ValidateStructField<[Nullable(0)] T, F>(FieldInfo fieldInfo, bool logErrorInTrace = true) where T : struct
		{
			if (fieldInfo == null)
			{
				return false;
			}
			if (fieldInfo.IsStatic)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError(string.Concat(new string[]
					{
						"AccessTools2.ValidateStructField<",
						typeof(T).FullName,
						", ",
						typeof(F).FullName,
						">: Field must not be static"
					}));
				}
				return false;
			}
			if (fieldInfo.DeclaringType != typeof(T))
			{
				if (logErrorInTrace)
				{
					Trace.TraceError(string.Concat(new string[]
					{
						"AccessTools2.ValidateStructField<",
						typeof(T).FullName,
						", ",
						typeof(F).FullName,
						">: FieldDeclaringType must be T (StructFieldRefAccess instance type)"
					}));
				}
				return false;
			}
			return true;
		}

		// Token: 0x060001AB RID: 427 RVA: 0x0000B61C File Offset: 0x0000981C
		[NullableContext(2)]
		private static bool TryGetComponents([Nullable(1)] string typeColonName, out Type type, out string name, bool logErrorInTrace = true)
		{
			if (string.IsNullOrWhiteSpace(typeColonName))
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.TryGetComponents: 'typeColonName' is null or whitespace/empty");
				}
				type = null;
				name = null;
				return false;
			}
			string[] array = typeColonName.Split(new char[] { ':' });
			if (array.Length != 2)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.TryGetComponents: typeColonName '" + typeColonName + "', name must be specified as 'Namespace.Type1.Type2:Name");
				}
				type = null;
				name = null;
				return false;
			}
			type = AccessTools2.TypeByName(array[0], logErrorInTrace);
			name = array[1];
			return type != null;
		}

		// Token: 0x040000A0 RID: 160
		private static readonly HashSet<Type> NumericTypes = new HashSet<Type>
		{
			typeof(long),
			typeof(ulong),
			typeof(int),
			typeof(uint),
			typeof(short),
			typeof(ushort),
			typeof(byte),
			typeof(sbyte)
		};

		// Token: 0x02000050 RID: 80
		[Nullable(0)]
		[ExcludeFromCodeCoverage]
		private readonly struct DynamicMethodDefinitionHandle
		{
			// Token: 0x06000294 RID: 660 RVA: 0x0000CE94 File Offset: 0x0000B094
			public static AccessTools2.DynamicMethodDefinitionHandle? Create(string name, Type returnType, Type[] parameterTypes)
			{
				if (AccessTools2.Helper.DynamicMethodDefinitionCtor != null)
				{
					return new AccessTools2.DynamicMethodDefinitionHandle?(new AccessTools2.DynamicMethodDefinitionHandle(AccessTools2.Helper.DynamicMethodDefinitionCtor(name, returnType, parameterTypes)));
				}
				return null;
			}

			// Token: 0x06000295 RID: 661 RVA: 0x0000CEC9 File Offset: 0x0000B0C9
			public DynamicMethodDefinitionHandle(object dynamicMethodDefinition)
			{
				this._dynamicMethodDefinition = dynamicMethodDefinition;
			}

			// Token: 0x06000296 RID: 662 RVA: 0x0000CED4 File Offset: 0x0000B0D4
			public AccessTools2.ILGeneratorHandle? GetILGenerator()
			{
				if (AccessTools2.Helper.GetILGenerator != null)
				{
					return new AccessTools2.ILGeneratorHandle?(new AccessTools2.ILGeneratorHandle(AccessTools2.Helper.GetILGenerator(this._dynamicMethodDefinition)));
				}
				return null;
			}

			// Token: 0x06000297 RID: 663 RVA: 0x0000CF0C File Offset: 0x0000B10C
			[NullableContext(2)]
			public MethodInfo Generate()
			{
				if (AccessTools2.Helper.Generate != null)
				{
					return AccessTools2.Helper.Generate(this._dynamicMethodDefinition);
				}
				return null;
			}

			// Token: 0x04000108 RID: 264
			private readonly object _dynamicMethodDefinition;
		}

		// Token: 0x02000051 RID: 81
		[Nullable(0)]
		[ExcludeFromCodeCoverage]
		private readonly struct ILGeneratorHandle
		{
			// Token: 0x06000298 RID: 664 RVA: 0x0000CF27 File Offset: 0x0000B127
			public ILGeneratorHandle(object ilGenerator)
			{
				this._ilGenerator = ilGenerator;
			}

			// Token: 0x06000299 RID: 665 RVA: 0x0000CF30 File Offset: 0x0000B130
			public void Emit(OpCode opcode)
			{
				AccessTools2.Helper.Emit1Delegate emit = AccessTools2.Helper.Emit1;
				if (emit == null)
				{
					return;
				}
				emit(this._ilGenerator, opcode);
			}

			// Token: 0x0600029A RID: 666 RVA: 0x0000CF48 File Offset: 0x0000B148
			public void Emit(OpCode opcode, FieldInfo field)
			{
				AccessTools2.Helper.Emit2Delegate emit = AccessTools2.Helper.Emit2;
				if (emit == null)
				{
					return;
				}
				emit(this._ilGenerator, opcode, field);
			}

			// Token: 0x0600029B RID: 667 RVA: 0x0000CF61 File Offset: 0x0000B161
			public void Emit(OpCode opcode, Type type)
			{
				AccessTools2.Helper.Emit3Delegate emit = AccessTools2.Helper.Emit3;
				if (emit == null)
				{
					return;
				}
				emit(this._ilGenerator, opcode, type);
			}

			// Token: 0x04000109 RID: 265
			private readonly object _ilGenerator;
		}

		// Token: 0x02000052 RID: 82
		[NullableContext(0)]
		[ExcludeFromCodeCoverage]
		private static class Helper
		{
			// Token: 0x0600029D RID: 669 RVA: 0x0000D078 File Offset: 0x0000B278
			public static bool IsValid(bool logErrorInTrace = true)
			{
				if (AccessTools2.Helper.DynamicMethodDefinitionCtor == null)
				{
					if (logErrorInTrace)
					{
						Trace.TraceError("AccessTools2.Helper.IsValid: DynamicMethodDefinitionCtor is null");
					}
					return false;
				}
				if (AccessTools2.Helper.GetILGenerator == null)
				{
					if (logErrorInTrace)
					{
						Trace.TraceError("AccessTools2.Helper.IsValid: GetILGenerator is null");
					}
					return false;
				}
				if (AccessTools2.Helper.Emit1 == null)
				{
					if (logErrorInTrace)
					{
						Trace.TraceError("AccessTools2.Helper.IsValid: Emit1 is null");
					}
					return false;
				}
				if (AccessTools2.Helper.Emit2 == null)
				{
					if (logErrorInTrace)
					{
						Trace.TraceError("AccessTools2.Helper.IsValid: Emit2 is null");
					}
					return false;
				}
				if (AccessTools2.Helper.Emit3 == null)
				{
					if (logErrorInTrace)
					{
						Trace.TraceError("AccessTools2.Helper.IsValid: Emit3 is null");
					}
					return false;
				}
				if (AccessTools2.Helper.Generate == null)
				{
					if (logErrorInTrace)
					{
						Trace.TraceError("AccessTools2.Helper.IsValid: Generate is null");
					}
					return false;
				}
				return true;
			}

			// Token: 0x0400010A RID: 266
			[Nullable(2)]
			public static readonly AccessTools2.Helper.DynamicMethodDefinitionCtorDelegate DynamicMethodDefinitionCtor = AccessTools2.GetDeclaredConstructorDelegate<AccessTools2.Helper.DynamicMethodDefinitionCtorDelegate>("MonoMod.Utils.DynamicMethodDefinition", new Type[]
			{
				typeof(string),
				typeof(Type),
				typeof(Type[])
			}, true);

			// Token: 0x0400010B RID: 267
			[Nullable(2)]
			public static readonly AccessTools2.Helper.GetILGeneratorDelegate GetILGenerator = AccessTools2.GetDelegateObjectInstance<AccessTools2.Helper.GetILGeneratorDelegate>("MonoMod.Utils.DynamicMethodDefinition:GetILGenerator", Type.EmptyTypes, null, true);

			// Token: 0x0400010C RID: 268
			[Nullable(2)]
			public static readonly AccessTools2.Helper.Emit1Delegate Emit1 = AccessTools2.GetDelegateObjectInstance<AccessTools2.Helper.Emit1Delegate>("System.Reflection.Emit.ILGenerator:Emit", new Type[] { typeof(OpCode) }, null, true);

			// Token: 0x0400010D RID: 269
			[Nullable(2)]
			public static readonly AccessTools2.Helper.Emit2Delegate Emit2 = AccessTools2.GetDelegateObjectInstance<AccessTools2.Helper.Emit2Delegate>("System.Reflection.Emit.ILGenerator:Emit", new Type[]
			{
				typeof(OpCode),
				typeof(FieldInfo)
			}, null, true);

			// Token: 0x0400010E RID: 270
			[Nullable(2)]
			public static readonly AccessTools2.Helper.Emit3Delegate Emit3 = AccessTools2.GetDelegateObjectInstance<AccessTools2.Helper.Emit3Delegate>("System.Reflection.Emit.ILGenerator:Emit", new Type[]
			{
				typeof(OpCode),
				typeof(Type)
			}, null, true);

			// Token: 0x0400010F RID: 271
			[Nullable(2)]
			public static readonly AccessTools2.Helper.GenerateDelegate Generate = AccessTools2.GetDelegateObjectInstance<AccessTools2.Helper.GenerateDelegate>("MonoMod.Utils.DynamicMethodDefinition:Generate", Type.EmptyTypes, null, true);

			// Token: 0x02000064 RID: 100
			// (Invoke) Token: 0x060002CA RID: 714
			public delegate object DynamicMethodDefinitionCtorDelegate(string name, Type returnType, Type[] parameterTypes);

			// Token: 0x02000065 RID: 101
			// (Invoke) Token: 0x060002CE RID: 718
			public delegate object GetILGeneratorDelegate(object instance);

			// Token: 0x02000066 RID: 102
			// (Invoke) Token: 0x060002D2 RID: 722
			public delegate void Emit1Delegate(object instance, OpCode opcode);

			// Token: 0x02000067 RID: 103
			// (Invoke) Token: 0x060002D6 RID: 726
			public delegate void Emit2Delegate(object instance, OpCode opcode, FieldInfo field);

			// Token: 0x02000068 RID: 104
			// (Invoke) Token: 0x060002DA RID: 730
			public delegate void Emit3Delegate(object instance, OpCode opcode, Type type);

			// Token: 0x02000069 RID: 105
			// (Invoke) Token: 0x060002DE RID: 734
			public delegate MethodInfo GenerateDelegate(object instance);
		}
	}
}
