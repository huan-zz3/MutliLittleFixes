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
	// Token: 0x0200000C RID: 12
	[NullableContext(1)]
	[Nullable(0)]
	internal static class AccessTools2
	{
		// Token: 0x0600005E RID: 94 RVA: 0x00005EE8 File Offset: 0x000040E8
		[return: Nullable(2)]
		public static ConstructorInfo DeclaredConstructor(Type type, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, bool searchForStatic = false, bool logErrorInTrace = true)
		{
			bool flag = type == null;
			ConstructorInfo constructorInfo;
			if (flag)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.DeclaredConstructor: 'type' is null");
				}
				constructorInfo = null;
			}
			else
			{
				bool flag2 = parameters == null;
				if (flag2)
				{
					parameters = Type.EmptyTypes;
				}
				BindingFlags bindingFlags = (searchForStatic ? (AccessTools.allDeclared & ~BindingFlags.Instance) : (AccessTools.allDeclared & ~BindingFlags.Static));
				constructorInfo = type.GetConstructor(bindingFlags, null, parameters, new ParameterModifier[0]);
			}
			return constructorInfo;
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00005F50 File Offset: 0x00004150
		[return: Nullable(2)]
		public static ConstructorInfo Constructor(Type type, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, bool searchForStatic = false, bool logErrorInTrace = true)
		{
			bool flag = type == null;
			ConstructorInfo constructorInfo;
			if (flag)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.ConstructorInfo: 'type' is null");
				}
				constructorInfo = null;
			}
			else
			{
				bool flag2 = parameters == null;
				if (flag2)
				{
					parameters = Type.EmptyTypes;
				}
				BindingFlags flags = (searchForStatic ? (AccessTools.all & ~BindingFlags.Instance) : (AccessTools.all & ~BindingFlags.Static));
				constructorInfo = AccessTools2.FindIncludingBaseTypes<ConstructorInfo>(type, (Type t) => t.GetConstructor(flags, null, parameters, new ParameterModifier[0]));
			}
			return constructorInfo;
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00005FD4 File Offset: 0x000041D4
		[return: Nullable(2)]
		public static ConstructorInfo DeclaredConstructor(string typeString, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, bool searchForStatic = false, bool logErrorInTrace = true)
		{
			bool flag = string.IsNullOrWhiteSpace(typeString);
			ConstructorInfo constructorInfo;
			if (flag)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.Constructor: 'typeString' is null or whitespace/empty");
				}
				constructorInfo = null;
			}
			else
			{
				Type type = AccessTools2.TypeByName(typeString, logErrorInTrace);
				bool flag2 = type == null;
				if (flag2)
				{
					constructorInfo = null;
				}
				else
				{
					constructorInfo = AccessTools2.DeclaredConstructor(type, parameters, searchForStatic, logErrorInTrace);
				}
			}
			return constructorInfo;
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00006024 File Offset: 0x00004224
		[return: Nullable(2)]
		public static ConstructorInfo Constructor(string typeString, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, bool searchForStatic = false, bool logErrorInTrace = true)
		{
			bool flag = string.IsNullOrWhiteSpace(typeString);
			ConstructorInfo constructorInfo;
			if (flag)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.Constructor: 'typeString' is null or whitespace/empty");
				}
				constructorInfo = null;
			}
			else
			{
				Type type = AccessTools2.TypeByName(typeString, logErrorInTrace);
				bool flag2 = type == null;
				if (flag2)
				{
					constructorInfo = null;
				}
				else
				{
					constructorInfo = AccessTools2.Constructor(type, parameters, searchForStatic, logErrorInTrace);
				}
			}
			return constructorInfo;
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00006074 File Offset: 0x00004274
		[return: Nullable(2)]
		public static TDelegate GetDeclaredConstructorDelegate<[Nullable(0)] TDelegate>(Type type, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			ConstructorInfo constructorInfo = AccessTools2.DeclaredConstructor(type, parameters, false, logErrorInTrace);
			return (constructorInfo != null) ? AccessTools2.GetDelegate<TDelegate>(constructorInfo, logErrorInTrace) : default(TDelegate);
		}

		// Token: 0x06000063 RID: 99 RVA: 0x000060A0 File Offset: 0x000042A0
		[return: Nullable(2)]
		public static TDelegate GetConstructorDelegate<[Nullable(0)] TDelegate>(Type type, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			ConstructorInfo constructorInfo = AccessTools2.Constructor(type, parameters, false, logErrorInTrace);
			return (constructorInfo != null) ? AccessTools2.GetDelegate<TDelegate>(constructorInfo, logErrorInTrace) : default(TDelegate);
		}

		// Token: 0x06000064 RID: 100 RVA: 0x000060CC File Offset: 0x000042CC
		[return: Nullable(2)]
		public static TDelegate GetDeclaredConstructorDelegate<[Nullable(0)] TDelegate>(string typeString, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			ConstructorInfo constructorInfo = AccessTools2.DeclaredConstructor(typeString, parameters, false, logErrorInTrace);
			return (constructorInfo != null) ? AccessTools2.GetDelegate<TDelegate>(constructorInfo, logErrorInTrace) : default(TDelegate);
		}

		// Token: 0x06000065 RID: 101 RVA: 0x000060F8 File Offset: 0x000042F8
		[return: Nullable(2)]
		public static TDelegate GetConstructorDelegate<[Nullable(0)] TDelegate>(string typeString, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			ConstructorInfo constructorInfo = AccessTools2.Constructor(typeString, parameters, false, logErrorInTrace);
			return (constructorInfo != null) ? AccessTools2.GetDelegate<TDelegate>(constructorInfo, logErrorInTrace) : default(TDelegate);
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00006124 File Offset: 0x00004324
		[return: Nullable(2)]
		public static TDelegate GetPropertyGetterDelegate<[Nullable(0)] TDelegate>(PropertyInfo propertyInfo, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = ((propertyInfo != null) ? propertyInfo.GetGetMethod(true) : null);
			return (methodInfo != null) ? AccessTools2.GetDelegate<TDelegate>(methodInfo, logErrorInTrace) : default(TDelegate);
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00006154 File Offset: 0x00004354
		[return: Nullable(2)]
		public static TDelegate GetPropertySetterDelegate<[Nullable(0)] TDelegate>(PropertyInfo propertyInfo, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = ((propertyInfo != null) ? propertyInfo.GetSetMethod(true) : null);
			return (methodInfo != null) ? AccessTools2.GetDelegate<TDelegate>(methodInfo, logErrorInTrace) : default(TDelegate);
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00006184 File Offset: 0x00004384
		[return: Nullable(2)]
		public static TDelegate GetPropertyGetterDelegate<[Nullable(0)] TDelegate>([Nullable(2)] object instance, PropertyInfo propertyInfo, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = ((propertyInfo != null) ? propertyInfo.GetGetMethod(true) : null);
			return (methodInfo != null) ? AccessTools2.GetDelegate<TDelegate>(instance, methodInfo, logErrorInTrace) : default(TDelegate);
		}

		// Token: 0x06000069 RID: 105 RVA: 0x000061B8 File Offset: 0x000043B8
		[return: Nullable(2)]
		public static TDelegate GetPropertySetterDelegate<[Nullable(0)] TDelegate>([Nullable(2)] object instance, PropertyInfo propertyInfo, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = ((propertyInfo != null) ? propertyInfo.GetSetMethod(true) : null);
			return (methodInfo != null) ? AccessTools2.GetDelegate<TDelegate>(instance, methodInfo, logErrorInTrace) : default(TDelegate);
		}

		// Token: 0x0600006A RID: 106 RVA: 0x000061EC File Offset: 0x000043EC
		[return: Nullable(2)]
		public static TDelegate GetDeclaredPropertyGetterDelegate<[Nullable(0)] TDelegate>(Type type, string name, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.DeclaredPropertyGetter(type, name, logErrorInTrace);
			return (methodInfo != null) ? AccessTools2.GetDelegate<TDelegate>(methodInfo, logErrorInTrace) : default(TDelegate);
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00006218 File Offset: 0x00004418
		[return: Nullable(2)]
		public static TDelegate GetDeclaredPropertySetterDelegate<[Nullable(0)] TDelegate>(Type type, string name, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.DeclaredPropertySetter(type, name, logErrorInTrace);
			return (methodInfo != null) ? AccessTools2.GetDelegate<TDelegate>(methodInfo, logErrorInTrace) : default(TDelegate);
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00006244 File Offset: 0x00004444
		[return: Nullable(2)]
		public static TDelegate GetPropertyGetterDelegate<[Nullable(0)] TDelegate>(Type type, string name, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.PropertyGetter(type, name, logErrorInTrace);
			return (methodInfo != null) ? AccessTools2.GetDelegate<TDelegate>(methodInfo, logErrorInTrace) : default(TDelegate);
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00006270 File Offset: 0x00004470
		[return: Nullable(2)]
		public static TDelegate GetPropertySetterDelegate<[Nullable(0)] TDelegate>(Type type, string name, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.PropertySetter(type, name, logErrorInTrace);
			return (methodInfo != null) ? AccessTools2.GetDelegate<TDelegate>(methodInfo, logErrorInTrace) : default(TDelegate);
		}

		// Token: 0x0600006E RID: 110 RVA: 0x0000629C File Offset: 0x0000449C
		[return: Nullable(2)]
		public static TDelegate GetDeclaredPropertyGetterDelegate<[Nullable(0)] TDelegate>([Nullable(2)] object instance, Type type, string method, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.DeclaredPropertyGetter(type, method, logErrorInTrace);
			return (methodInfo != null) ? AccessTools2.GetDelegate<TDelegate>(instance, methodInfo, logErrorInTrace) : default(TDelegate);
		}

		// Token: 0x0600006F RID: 111 RVA: 0x000062C8 File Offset: 0x000044C8
		[return: Nullable(2)]
		public static TDelegate GetDeclaredPropertySetterDelegate<[Nullable(0)] TDelegate>([Nullable(2)] object instance, Type type, string method, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.DeclaredPropertySetter(type, method, logErrorInTrace);
			return (methodInfo != null) ? AccessTools2.GetDelegate<TDelegate>(instance, methodInfo, logErrorInTrace) : default(TDelegate);
		}

		// Token: 0x06000070 RID: 112 RVA: 0x000062F4 File Offset: 0x000044F4
		[return: Nullable(2)]
		public static TDelegate GetPropertyGetterDelegate<[Nullable(0)] TDelegate>([Nullable(2)] object instance, Type type, string method, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.PropertyGetter(type, method, logErrorInTrace);
			return (methodInfo != null) ? AccessTools2.GetDelegate<TDelegate>(instance, methodInfo, logErrorInTrace) : default(TDelegate);
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00006320 File Offset: 0x00004520
		[return: Nullable(2)]
		public static TDelegate GetPropertySetterDelegate<[Nullable(0)] TDelegate>([Nullable(2)] object instance, Type type, string method, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.PropertySetter(type, method, logErrorInTrace);
			return (methodInfo != null) ? AccessTools2.GetDelegate<TDelegate>(instance, methodInfo, logErrorInTrace) : default(TDelegate);
		}

		// Token: 0x06000072 RID: 114 RVA: 0x0000634C File Offset: 0x0000454C
		[return: Nullable(2)]
		public static TDelegate GetDeclaredPropertyGetterDelegate<[Nullable(0)] TDelegate>(string typeColonPropertyName, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.DeclaredPropertyGetter(typeColonPropertyName, logErrorInTrace);
			return (methodInfo != null) ? AccessTools2.GetDelegate<TDelegate>(methodInfo, logErrorInTrace) : default(TDelegate);
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00006378 File Offset: 0x00004578
		[return: Nullable(2)]
		public static TDelegate GetDeclaredPropertySetterDelegate<[Nullable(0)] TDelegate>(string typeColonPropertyName, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.DeclaredPropertySetter(typeColonPropertyName, logErrorInTrace);
			return (methodInfo != null) ? AccessTools2.GetDelegate<TDelegate>(methodInfo, logErrorInTrace) : default(TDelegate);
		}

		// Token: 0x06000074 RID: 116 RVA: 0x000063A4 File Offset: 0x000045A4
		[return: Nullable(2)]
		public static TDelegate GetPropertyGetterDelegate<[Nullable(0)] TDelegate>(string typeColonPropertyName, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.PropertyGetter(typeColonPropertyName, logErrorInTrace);
			return (methodInfo != null) ? AccessTools2.GetDelegate<TDelegate>(methodInfo, logErrorInTrace) : default(TDelegate);
		}

		// Token: 0x06000075 RID: 117 RVA: 0x000063D0 File Offset: 0x000045D0
		[return: Nullable(2)]
		public static TDelegate GetPropertySetterDelegate<[Nullable(0)] TDelegate>(string typeColonPropertyName, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.PropertySetter(typeColonPropertyName, logErrorInTrace);
			return (methodInfo != null) ? AccessTools2.GetDelegate<TDelegate>(methodInfo, logErrorInTrace) : default(TDelegate);
		}

		// Token: 0x06000076 RID: 118 RVA: 0x000063FC File Offset: 0x000045FC
		[return: Nullable(2)]
		public static TDelegate GetDeclaredPropertyGetterDelegate<[Nullable(0)] TDelegate>([Nullable(2)] object instance, string typeColonPropertyName, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.DeclaredPropertyGetter(typeColonPropertyName, logErrorInTrace);
			return (methodInfo != null) ? AccessTools2.GetDelegate<TDelegate>(instance, methodInfo, logErrorInTrace) : default(TDelegate);
		}

		// Token: 0x06000077 RID: 119 RVA: 0x00006428 File Offset: 0x00004628
		[return: Nullable(2)]
		public static TDelegate GetDeclaredPropertySetterDelegate<[Nullable(0)] TDelegate>([Nullable(2)] object instance, string typeColonPropertyName, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.DeclaredPropertySetter(typeColonPropertyName, logErrorInTrace);
			return (methodInfo != null) ? AccessTools2.GetDelegate<TDelegate>(instance, methodInfo, logErrorInTrace) : default(TDelegate);
		}

		// Token: 0x06000078 RID: 120 RVA: 0x00006454 File Offset: 0x00004654
		[return: Nullable(2)]
		public static TDelegate GetPropertyGetterDelegate<[Nullable(0)] TDelegate>([Nullable(2)] object instance, string typeColonPropertyName, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.PropertyGetter(typeColonPropertyName, logErrorInTrace);
			return (methodInfo != null) ? AccessTools2.GetDelegate<TDelegate>(instance, methodInfo, logErrorInTrace) : default(TDelegate);
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00006480 File Offset: 0x00004680
		[return: Nullable(2)]
		public static TDelegate GetPropertySetterDelegate<[Nullable(0)] TDelegate>([Nullable(2)] object instance, string typeColonPropertyName, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.PropertySetter(typeColonPropertyName, logErrorInTrace);
			return (methodInfo != null) ? AccessTools2.GetDelegate<TDelegate>(instance, methodInfo, logErrorInTrace) : default(TDelegate);
		}

		// Token: 0x0600007A RID: 122 RVA: 0x000064AC File Offset: 0x000046AC
		[return: Nullable(2)]
		public static TDelegate GetDelegate<[Nullable(0)] TDelegate>(ConstructorInfo constructorInfo, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			bool flag = constructorInfo == null;
			TDelegate tdelegate;
			if (flag)
			{
				tdelegate = default(TDelegate);
			}
			else
			{
				MethodInfo method = typeof(TDelegate).GetMethod("Invoke");
				bool flag2 = method == null;
				if (flag2)
				{
					tdelegate = default(TDelegate);
				}
				else
				{
					bool flag3 = !method.ReturnType.IsAssignableFrom(constructorInfo.DeclaringType);
					if (flag3)
					{
						tdelegate = default(TDelegate);
					}
					else
					{
						ParameterInfo[] parameters = method.GetParameters();
						ParameterInfo[] constructorParameters = constructorInfo.GetParameters();
						bool flag4 = parameters.Length - constructorParameters.Length != 0 && !AccessTools2.ParametersAreEqual(parameters, constructorParameters);
						if (flag4)
						{
							tdelegate = default(TDelegate);
						}
						else
						{
							ParameterExpression parameterExpression = Expression.Parameter(typeof(object), "instance");
							List<ParameterExpression> list = parameters.Select<ParameterInfo, ParameterExpression>((ParameterInfo pi, int i) => Expression.Parameter(pi.ParameterType, string.Format("p{0}", i))).ToList<ParameterExpression>();
							List<Expression> list2 = list.Select<ParameterExpression, Expression>(delegate(ParameterExpression pe, int i)
							{
								bool flag5 = pe.IsByRef || pe.Type.Equals(constructorParameters[i].ParameterType);
								Expression expression2;
								if (flag5)
								{
									expression2 = pe;
								}
								else
								{
									expression2 = Expression.Convert(pe, constructorParameters[i].ParameterType);
								}
								return expression2;
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
						}
					}
				}
			}
			return tdelegate;
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00006658 File Offset: 0x00004858
		[return: Nullable(2)]
		public static TDelegate GetDelegate<[Nullable(0)] TDelegate>([Nullable(2)] object instance, MethodInfo methodInfo, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			bool flag = methodInfo == null;
			TDelegate tdelegate;
			if (flag)
			{
				tdelegate = default(TDelegate);
			}
			else
			{
				MethodInfo method = typeof(TDelegate).GetMethod("Invoke");
				bool flag2 = method == null;
				if (flag2)
				{
					tdelegate = default(TDelegate);
				}
				else
				{
					bool flag3 = method.ReturnType.IsEnum || methodInfo.ReturnType.IsEnum;
					bool flag4 = method.ReturnType.IsNumeric() || methodInfo.ReturnType.IsNumeric();
					bool flag5 = !flag3 && !flag4 && !method.ReturnType.IsAssignableFrom(methodInfo.ReturnType);
					if (flag5)
					{
						tdelegate = default(TDelegate);
					}
					else
					{
						ParameterInfo[] parameters = method.GetParameters();
						ParameterInfo[] methodParameters = methodInfo.GetParameters();
						bool flag6 = parameters.Length - methodParameters.Length == 0 && AccessTools2.ParametersAreEqual(parameters, methodParameters);
						bool flag7 = instance != null;
						bool flag8 = parameters.Length - methodParameters.Length == 1 && (parameters[0].ParameterType.IsAssignableFrom(methodInfo.DeclaringType) || methodInfo.DeclaringType.IsAssignableFrom(parameters[0].ParameterType));
						bool flag9 = !flag7 && !flag8 && !methodInfo.IsStatic;
						if (flag9)
						{
							tdelegate = default(TDelegate);
						}
						else
						{
							bool flag10 = flag7 && methodInfo.IsStatic;
							if (flag10)
							{
								tdelegate = default(TDelegate);
							}
							else
							{
								bool flag11 = flag7 && !methodInfo.IsStatic && !methodInfo.DeclaringType.IsAssignableFrom(instance.GetType());
								if (flag11)
								{
									tdelegate = default(TDelegate);
								}
								else
								{
									bool flag12 = flag6 && flag8;
									if (flag12)
									{
										tdelegate = default(TDelegate);
									}
									else
									{
										bool flag13 = flag7 && (flag8 || !flag6);
										if (flag13)
										{
											tdelegate = default(TDelegate);
										}
										else
										{
											bool flag14 = flag8 && (flag7 || flag6);
											if (flag14)
											{
												tdelegate = default(TDelegate);
											}
											else
											{
												bool flag15 = !flag8 && !flag7 && !flag6;
												if (flag15)
												{
													tdelegate = default(TDelegate);
												}
												else
												{
													ParameterExpression parameterExpression = (flag8 ? Expression.Parameter(parameters[0].ParameterType, "instance") : null);
													List<ParameterExpression> list = parameters.Skip<ParameterInfo>(flag8 ? 1 : 0).Select<ParameterInfo, ParameterExpression>((ParameterInfo pi, int i) => Expression.Parameter(pi.ParameterType, string.Format("p{0}", i))).ToList<ParameterExpression>();
													List<Expression> list2 = list.Select<ParameterExpression, Expression>(delegate(ParameterExpression pe, int i)
													{
														bool flag17 = pe.IsByRef || pe.Type.Equals(methodParameters[i].ParameterType);
														Expression expression2;
														if (flag17)
														{
															expression2 = pe;
														}
														else
														{
															expression2 = Expression.Convert(pe, methodParameters[i].ParameterType);
														}
														return expression2;
													}).ToList<Expression>();
													MethodCallExpression methodCallExpression = (flag7 ? (instance.GetType().Equals(methodInfo.DeclaringType) ? Expression.Call(Expression.Constant(instance), methodInfo, list2) : Expression.Call(Expression.Convert(Expression.Constant(instance), instance.GetType()), methodInfo, list2)) : (flag6 ? Expression.Call(methodInfo, list2) : (flag8 ? (parameterExpression.Type.Equals(methodInfo.DeclaringType) ? Expression.Call(parameterExpression, methodInfo, list2) : Expression.Call(Expression.Convert(parameterExpression, methodInfo.DeclaringType), methodInfo, list2)) : null)));
													bool flag16 = methodCallExpression == null;
													if (flag16)
													{
														tdelegate = default(TDelegate);
													}
													else
													{
														UnaryExpression unaryExpression = Expression.Convert(methodCallExpression, method.ReturnType);
														try
														{
															Expression expression = unaryExpression;
															IEnumerable<ParameterExpression> enumerable2;
															if (!flag8)
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
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return tdelegate;
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00006A60 File Offset: 0x00004C60
		[return: Nullable(2)]
		public static TDelegate GetDelegate<[Nullable(0)] TDelegate>(MethodInfo methodInfo, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			return AccessTools2.GetDelegate<TDelegate>(null, methodInfo, logErrorInTrace);
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00006A6A File Offset: 0x00004C6A
		[return: Nullable(2)]
		public static TDelegate GetDelegateObjectInstance<[Nullable(0)] TDelegate>(MethodInfo methodInfo, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			return AccessTools2.GetDelegate<TDelegate>(methodInfo, logErrorInTrace);
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00006A73 File Offset: 0x00004C73
		public static bool IsNumeric(this Type myType)
		{
			return AccessTools2.NumericTypes.Contains(myType);
		}

		// Token: 0x0600007F RID: 127 RVA: 0x00006A80 File Offset: 0x00004C80
		private static bool ParametersAreEqual(ParameterInfo[] delegateParameters, ParameterInfo[] methodParameters)
		{
			bool flag = delegateParameters.Length - methodParameters.Length == 0;
			bool flag6;
			if (flag)
			{
				for (int i = 0; i < methodParameters.Length; i++)
				{
					bool flag2 = delegateParameters[i].ParameterType.IsByRef != methodParameters[i].ParameterType.IsByRef;
					if (flag2)
					{
						return false;
					}
					bool flag3 = delegateParameters[i].ParameterType.IsEnum || methodParameters[i].ParameterType.IsEnum;
					bool flag4 = delegateParameters[i].ParameterType.IsNumeric() || methodParameters[i].ParameterType.IsNumeric();
					bool flag5 = !flag3 && !flag4 && !delegateParameters[i].ParameterType.IsAssignableFrom(methodParameters[i].ParameterType);
					if (flag5)
					{
						return false;
					}
				}
				flag6 = true;
			}
			else
			{
				bool flag7 = delegateParameters.Length - methodParameters.Length == 1;
				if (flag7)
				{
					for (int j = 0; j < methodParameters.Length; j++)
					{
						bool flag8 = delegateParameters[j + 1].ParameterType.IsByRef != methodParameters[j].ParameterType.IsByRef;
						if (flag8)
						{
							return false;
						}
						bool flag9 = delegateParameters[j + 1].ParameterType.IsEnum || methodParameters[j].ParameterType.IsEnum;
						bool flag10 = delegateParameters[j + 1].ParameterType.IsNumeric() || methodParameters[j].ParameterType.IsNumeric();
						bool flag11 = !flag9 && !flag10 && !delegateParameters[j + 1].ParameterType.IsAssignableFrom(methodParameters[j].ParameterType);
						if (flag11)
						{
							return false;
						}
					}
					flag6 = true;
				}
				else
				{
					flag6 = false;
				}
			}
			return flag6;
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00006C4E File Offset: 0x00004E4E
		[return: Nullable(2)]
		public static TDelegate GetDelegate<[Nullable(0)] TDelegate, [Nullable(2)] TInstance>(TInstance instance, MethodInfo methodInfo, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			return AccessTools2.GetDelegate<TDelegate>(instance, methodInfo, logErrorInTrace);
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00006C60 File Offset: 0x00004E60
		[return: Nullable(2)]
		public static TDelegate GetDeclaredDelegateObjectInstance<[Nullable(0)] TDelegate>(Type type, string method, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, [Nullable(new byte[] { 2, 1 })] Type[] generics = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.DeclaredMethod(type, method, parameters, generics, logErrorInTrace);
			return (methodInfo != null) ? AccessTools2.GetDelegateObjectInstance<TDelegate>(methodInfo, logErrorInTrace) : default(TDelegate);
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00006C90 File Offset: 0x00004E90
		[return: Nullable(2)]
		public static TDelegate GetDelegateObjectInstance<[Nullable(0)] TDelegate>(Type type, string method, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, [Nullable(new byte[] { 2, 1 })] Type[] generics = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.Method(type, method, parameters, generics, logErrorInTrace);
			return (methodInfo != null) ? AccessTools2.GetDelegateObjectInstance<TDelegate>(methodInfo, logErrorInTrace) : default(TDelegate);
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00006CC0 File Offset: 0x00004EC0
		[return: Nullable(2)]
		public static TDelegate GetDeclaredDelegateObjectInstance<[Nullable(0)] TDelegate>(string typeSemicolonMethod, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, [Nullable(new byte[] { 2, 1 })] Type[] generics = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.DeclaredMethod(typeSemicolonMethod, parameters, generics, logErrorInTrace);
			return (methodInfo != null) ? AccessTools2.GetDelegateObjectInstance<TDelegate>(methodInfo, logErrorInTrace) : default(TDelegate);
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00006CEC File Offset: 0x00004EEC
		[return: Nullable(2)]
		public static TDelegate GetDelegateObjectInstance<[Nullable(0)] TDelegate>(string typeSemicolonMethod, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, [Nullable(new byte[] { 2, 1 })] Type[] generics = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.Method(typeSemicolonMethod, parameters, generics, logErrorInTrace);
			return (methodInfo != null) ? AccessTools2.GetDelegateObjectInstance<TDelegate>(methodInfo, logErrorInTrace) : default(TDelegate);
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00006D18 File Offset: 0x00004F18
		[return: Nullable(2)]
		public static TDelegate GetDeclaredDelegate<[Nullable(0)] TDelegate>(Type type, string method, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, [Nullable(new byte[] { 2, 1 })] Type[] generics = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.DeclaredMethod(type, method, parameters, generics, logErrorInTrace);
			return (methodInfo != null) ? AccessTools2.GetDelegate<TDelegate>(methodInfo, logErrorInTrace) : default(TDelegate);
		}

		// Token: 0x06000086 RID: 134 RVA: 0x00006D48 File Offset: 0x00004F48
		[return: Nullable(2)]
		public static TDelegate GetDelegate<[Nullable(0)] TDelegate>(Type type, string method, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, [Nullable(new byte[] { 2, 1 })] Type[] generics = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.Method(type, method, parameters, generics, logErrorInTrace);
			return (methodInfo != null) ? AccessTools2.GetDelegate<TDelegate>(methodInfo, logErrorInTrace) : default(TDelegate);
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00006D78 File Offset: 0x00004F78
		[return: Nullable(2)]
		public static TDelegate GetDeclaredDelegate<[Nullable(0)] TDelegate>(string typeSemicolonMethod, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, [Nullable(new byte[] { 2, 1 })] Type[] generics = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.DeclaredMethod(typeSemicolonMethod, parameters, generics, logErrorInTrace);
			return (methodInfo != null) ? AccessTools2.GetDelegate<TDelegate>(methodInfo, logErrorInTrace) : default(TDelegate);
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00006DA4 File Offset: 0x00004FA4
		[return: Nullable(2)]
		public static TDelegate GetDelegate<[Nullable(0)] TDelegate>(string typeSemicolonMethod, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, [Nullable(new byte[] { 2, 1 })] Type[] generics = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.Method(typeSemicolonMethod, parameters, generics, logErrorInTrace);
			return (methodInfo != null) ? AccessTools2.GetDelegate<TDelegate>(methodInfo, logErrorInTrace) : default(TDelegate);
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00006DD0 File Offset: 0x00004FD0
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

		// Token: 0x0600008A RID: 138 RVA: 0x00006E14 File Offset: 0x00005014
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

		// Token: 0x0600008B RID: 139 RVA: 0x00006E58 File Offset: 0x00005058
		[return: Nullable(2)]
		public static TDelegate GetDeclaredDelegate<[Nullable(0)] TDelegate>([Nullable(2)] object instance, Type type, string method, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, [Nullable(new byte[] { 2, 1 })] Type[] generics = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.DeclaredMethod(type, method, parameters, generics, logErrorInTrace);
			return (methodInfo != null) ? AccessTools2.GetDelegate<TDelegate>(instance, methodInfo, logErrorInTrace) : default(TDelegate);
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00006E8C File Offset: 0x0000508C
		[return: Nullable(2)]
		public static TDelegate GetDelegate<[Nullable(0)] TDelegate>([Nullable(2)] object instance, Type type, string method, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, [Nullable(new byte[] { 2, 1 })] Type[] generics = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.Method(type, method, parameters, generics, logErrorInTrace);
			return (methodInfo != null) ? AccessTools2.GetDelegate<TDelegate>(instance, methodInfo, logErrorInTrace) : default(TDelegate);
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00006EC0 File Offset: 0x000050C0
		[return: Nullable(2)]
		public static TDelegate GetDeclaredDelegate<[Nullable(0)] TDelegate>([Nullable(2)] object instance, string typeSemicolonMethod, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, [Nullable(new byte[] { 2, 1 })] Type[] generics = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.DeclaredMethod(typeSemicolonMethod, parameters, generics, logErrorInTrace);
			return (methodInfo != null) ? AccessTools2.GetDelegate<TDelegate>(instance, methodInfo, logErrorInTrace) : default(TDelegate);
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00006EF0 File Offset: 0x000050F0
		[return: Nullable(2)]
		public static TDelegate GetDelegate<[Nullable(0)] TDelegate>([Nullable(2)] object instance, string typeSemicolonMethod, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, [Nullable(new byte[] { 2, 1 })] Type[] generics = null, bool logErrorInTrace = true) where TDelegate : Delegate
		{
			MethodInfo methodInfo = AccessTools2.Method(typeSemicolonMethod, parameters, generics, logErrorInTrace);
			return (methodInfo != null) ? AccessTools2.GetDelegate<TDelegate>(instance, methodInfo, logErrorInTrace) : default(TDelegate);
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00006F20 File Offset: 0x00005120
		[return: Nullable(2)]
		public static FieldInfo DeclaredField(Type type, string name, bool logErrorInTrace = true)
		{
			bool flag = type == null;
			FieldInfo fieldInfo;
			if (flag)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.DeclaredField: 'type' is null");
				}
				fieldInfo = null;
			}
			else
			{
				bool flag2 = name == null;
				if (flag2)
				{
					if (logErrorInTrace)
					{
						Trace.TraceError(string.Format("AccessTools2.DeclaredField: type '{0}', 'name' is null", type));
					}
					fieldInfo = null;
				}
				else
				{
					FieldInfo field = type.GetField(name, AccessTools.allDeclared);
					bool flag3 = field == null;
					if (flag3)
					{
						if (logErrorInTrace)
						{
							Trace.TraceError(string.Format("AccessTools2.DeclaredField: Could not find field for type '{0}' and name '{1}'", type, name));
						}
						fieldInfo = null;
					}
					else
					{
						fieldInfo = field;
					}
				}
			}
			return fieldInfo;
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00006FAC File Offset: 0x000051AC
		[return: Nullable(2)]
		public static FieldInfo Field(Type type, string name, bool logErrorInTrace = true)
		{
			bool flag = type == null;
			FieldInfo fieldInfo;
			if (flag)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.Field: 'type' is null");
				}
				fieldInfo = null;
			}
			else
			{
				bool flag2 = name == null;
				if (flag2)
				{
					if (logErrorInTrace)
					{
						Trace.TraceError(string.Format("AccessTools2.Field: type '{0}', 'name' is null", type));
					}
					fieldInfo = null;
				}
				else
				{
					FieldInfo fieldInfo2 = AccessTools2.FindIncludingBaseTypes<FieldInfo>(type, (Type t) => t.GetField(name, AccessTools.all));
					bool flag3 = fieldInfo2 == null && logErrorInTrace;
					if (flag3)
					{
						Trace.TraceError(string.Format("AccessTools2.Field: Could not find field for type '{0}' and name '{1}'", type, name));
					}
					fieldInfo = fieldInfo2;
				}
			}
			return fieldInfo;
		}

		// Token: 0x06000091 RID: 145 RVA: 0x00007050 File Offset: 0x00005250
		[return: Nullable(2)]
		public static FieldInfo DeclaredField(string typeColonFieldname, bool logErrorInTrace = true)
		{
			Type type;
			string text;
			bool flag = !AccessTools2.TryGetComponents(typeColonFieldname, out type, out text, logErrorInTrace);
			FieldInfo fieldInfo;
			if (flag)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.Field: Could not find type or field for '" + typeColonFieldname + "'");
				}
				fieldInfo = null;
			}
			else
			{
				fieldInfo = AccessTools2.DeclaredField(type, text, logErrorInTrace);
			}
			return fieldInfo;
		}

		// Token: 0x06000092 RID: 146 RVA: 0x000070A0 File Offset: 0x000052A0
		[return: Nullable(2)]
		public static FieldInfo Field(string typeColonFieldname, bool logErrorInTrace = true)
		{
			Type type;
			string text;
			bool flag = !AccessTools2.TryGetComponents(typeColonFieldname, out type, out text, logErrorInTrace);
			FieldInfo fieldInfo;
			if (flag)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.Field: Could not find type or field for '" + typeColonFieldname + "'");
				}
				fieldInfo = null;
			}
			else
			{
				fieldInfo = AccessTools2.Field(type, text, logErrorInTrace);
			}
			return fieldInfo;
		}

		// Token: 0x06000093 RID: 147 RVA: 0x000070F0 File Offset: 0x000052F0
		[return: Nullable(new byte[] { 2, 1, 1 })]
		public static AccessTools.FieldRef<object, F> FieldRefAccess<[Nullable(2)] F>(string typeColonFieldname, bool logErrorInTrace = true)
		{
			Type type;
			string text;
			bool flag = !AccessTools2.TryGetComponents(typeColonFieldname, out type, out text, logErrorInTrace);
			AccessTools.FieldRef<object, F> fieldRef;
			if (flag)
			{
				Trace.TraceError("AccessTools2.FieldRefAccess: Could not find type or field for '" + typeColonFieldname + "'");
				fieldRef = null;
			}
			else
			{
				fieldRef = AccessTools2.FieldRefAccess<F>(type, text, logErrorInTrace);
			}
			return fieldRef;
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00007138 File Offset: 0x00005338
		[return: Nullable(new byte[] { 2, 1, 1 })]
		public static AccessTools.FieldRef<T, F> FieldRefAccess<T, [Nullable(2)] F>(string fieldName, bool logErrorInTrace = true) where T : class
		{
			bool flag = fieldName == null;
			AccessTools.FieldRef<T, F> fieldRef;
			if (flag)
			{
				fieldRef = null;
			}
			else
			{
				FieldInfo instanceField = AccessTools2.GetInstanceField(typeof(T), fieldName, logErrorInTrace);
				bool flag2 = instanceField == null;
				if (flag2)
				{
					fieldRef = null;
				}
				else
				{
					fieldRef = AccessTools2.FieldRefAccessInternal<T, F>(instanceField, false, logErrorInTrace);
				}
			}
			return fieldRef;
		}

		// Token: 0x06000095 RID: 149 RVA: 0x0000717C File Offset: 0x0000537C
		[return: Nullable(new byte[] { 2, 1, 1 })]
		public static AccessTools.FieldRef<object, F> FieldRefAccess<[Nullable(2)] F>(Type type, string fieldName, bool logErrorInTrace = true)
		{
			bool flag = type == null;
			AccessTools.FieldRef<object, F> fieldRef;
			if (flag)
			{
				fieldRef = null;
			}
			else
			{
				bool flag2 = fieldName == null;
				if (flag2)
				{
					fieldRef = null;
				}
				else
				{
					FieldInfo fieldInfo = AccessTools2.Field(type, fieldName, logErrorInTrace);
					bool flag3 = fieldInfo == null;
					if (flag3)
					{
						fieldRef = null;
					}
					else
					{
						Type declaringType;
						bool flag4;
						if (!fieldInfo.IsStatic)
						{
							declaringType = fieldInfo.DeclaringType;
							flag4 = declaringType != null;
						}
						else
						{
							flag4 = false;
						}
						bool flag5 = flag4;
						if (flag5)
						{
							bool isValueType = declaringType.IsValueType;
							if (isValueType)
							{
								if (logErrorInTrace)
								{
									Trace.TraceError("AccessTools2.FieldRefAccess<object, " + typeof(F).FullName + ">: FieldDeclaringType must be a class");
								}
								fieldRef = null;
							}
							else
							{
								fieldRef = AccessTools2.FieldRefAccessInternal<object, F>(fieldInfo, true, logErrorInTrace);
							}
						}
						else
						{
							fieldRef = null;
						}
					}
				}
			}
			return fieldRef;
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00007228 File Offset: 0x00005428
		[return: Nullable(new byte[] { 2, 1, 1 })]
		public static AccessTools.FieldRef<T, F> FieldRefAccess<T, [Nullable(2)] F>(FieldInfo fieldInfo, bool logErrorInTrace = true) where T : class
		{
			bool flag = fieldInfo == null;
			AccessTools.FieldRef<T, F> fieldRef;
			if (flag)
			{
				fieldRef = null;
			}
			else
			{
				Type declaringType;
				bool flag2;
				if (!fieldInfo.IsStatic)
				{
					declaringType = fieldInfo.DeclaringType;
					flag2 = declaringType != null;
				}
				else
				{
					flag2 = false;
				}
				bool flag3 = flag2;
				if (flag3)
				{
					bool isValueType = declaringType.IsValueType;
					if (isValueType)
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
						fieldRef = null;
					}
					else
					{
						bool? flag4 = AccessTools2.FieldRefNeedsClasscast(typeof(T), declaringType, logErrorInTrace);
						bool valueOrDefault;
						int num;
						if (flag4 != null)
						{
							valueOrDefault = flag4.GetValueOrDefault();
							num = 1;
						}
						else
						{
							num = 0;
						}
						bool flag5 = num == 0;
						if (flag5)
						{
							fieldRef = null;
						}
						else
						{
							fieldRef = AccessTools2.FieldRefAccessInternal<T, F>(fieldInfo, valueOrDefault, logErrorInTrace);
						}
					}
				}
				else
				{
					fieldRef = null;
				}
			}
			return fieldRef;
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00007310 File Offset: 0x00005510
		[return: Nullable(new byte[] { 2, 1, 1 })]
		private static AccessTools.FieldRef<T, F> FieldRefAccessInternal<T, [Nullable(2)] F>(FieldInfo fieldInfo, bool needCastclass, bool logErrorInTrace = true) where T : class
		{
			bool flag = !AccessTools2.Helper.IsValid(logErrorInTrace);
			AccessTools.FieldRef<T, F> fieldRef;
			if (flag)
			{
				fieldRef = null;
			}
			else
			{
				bool isStatic = fieldInfo.IsStatic;
				if (isStatic)
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
					fieldRef = null;
				}
				else
				{
					bool flag2 = !AccessTools2.ValidateFieldType<F>(fieldInfo, logErrorInTrace);
					if (flag2)
					{
						fieldRef = null;
					}
					else
					{
						Type typeFromHandle = typeof(T);
						Type declaringType = fieldInfo.DeclaringType;
						AccessTools2.DynamicMethodDefinitionHandle? dynamicMethodDefinitionHandle = AccessTools2.DynamicMethodDefinitionHandle.Create("__refget_" + typeFromHandle.Name + "_fi_" + fieldInfo.Name, typeof(F).MakeByRefType(), new Type[] { typeFromHandle });
						AccessTools2.ILGeneratorHandle? ilgeneratorHandle = ((dynamicMethodDefinitionHandle != null) ? dynamicMethodDefinitionHandle.GetValueOrDefault().GetILGenerator() : null);
						AccessTools2.ILGeneratorHandle valueOrDefault;
						int num;
						if (ilgeneratorHandle != null)
						{
							valueOrDefault = ilgeneratorHandle.GetValueOrDefault();
							num = 1;
						}
						else
						{
							num = 0;
						}
						bool flag3 = num == 0;
						if (flag3)
						{
							fieldRef = null;
						}
						else
						{
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
							fieldRef = obj as AccessTools.FieldRef<T, F>;
						}
					}
				}
			}
			return fieldRef;
		}

		// Token: 0x06000098 RID: 152 RVA: 0x000074C8 File Offset: 0x000056C8
		private static bool? FieldRefNeedsClasscast(Type delegateInstanceType, Type declaringType, bool logErrorInTrace = true)
		{
			bool flag = false;
			bool flag2 = delegateInstanceType != declaringType;
			if (flag2)
			{
				flag = delegateInstanceType.IsAssignableFrom(declaringType);
				bool flag3 = !flag && !declaringType.IsAssignableFrom(delegateInstanceType);
				if (flag3)
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

		// Token: 0x06000099 RID: 153 RVA: 0x00007533 File Offset: 0x00005733
		[return: Nullable(new byte[] { 2, 1, 1 })]
		public static AccessTools.FieldRef<object, TField> FieldRefAccess<[Nullable(2)] TField>(FieldInfo fieldInfo)
		{
			return (fieldInfo == null) ? null : AccessTools.FieldRefAccess<object, TField>(fieldInfo);
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00007544 File Offset: 0x00005744
		[return: Nullable(2)]
		public static MethodInfo DeclaredMethod(Type type, string name, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, [Nullable(new byte[] { 2, 1 })] Type[] generics = null, bool logErrorInTrace = true)
		{
			bool flag = type == null;
			MethodInfo methodInfo;
			if (flag)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.DeclaredMethod: 'type' is null");
				}
				methodInfo = null;
			}
			else
			{
				bool flag2 = name == null;
				if (flag2)
				{
					if (logErrorInTrace)
					{
						Trace.TraceError(string.Format("AccessTools2.DeclaredMethod: type '{0}', 'name' is null", type));
					}
					methodInfo = null;
				}
				else
				{
					bool flag3 = parameters == null;
					MethodInfo methodInfo2;
					if (flag3)
					{
						try
						{
							methodInfo2 = type.GetMethod(name, AccessTools.allDeclared);
						}
						catch (AmbiguousMatchException ex)
						{
							methodInfo2 = type.GetMethod(name, AccessTools.allDeclared, null, Type.EmptyTypes, new ParameterModifier[0]);
							bool flag4 = methodInfo2 == null;
							if (flag4)
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
						}
					}
					else
					{
						methodInfo2 = type.GetMethod(name, AccessTools.allDeclared, null, parameters, new ParameterModifier[0]);
					}
					bool flag5 = methodInfo2 == null;
					if (flag5)
					{
						if (logErrorInTrace)
						{
							Trace.TraceError(string.Format("AccessTools2.DeclaredMethod: Could not find method for type '{0}' and name '{1}' and parameters '{2}'", type, name, (parameters != null) ? GeneralExtensions.Description(parameters) : null));
						}
						methodInfo = null;
					}
					else
					{
						bool flag6 = generics != null;
						if (flag6)
						{
							methodInfo2 = methodInfo2.MakeGenericMethod(generics);
						}
						methodInfo = methodInfo2;
					}
				}
			}
			return methodInfo;
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00007698 File Offset: 0x00005898
		[return: Nullable(2)]
		public static MethodInfo Method(Type type, string name, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, [Nullable(new byte[] { 2, 1 })] Type[] generics = null, bool logErrorInTrace = true)
		{
			bool flag = type == null;
			MethodInfo methodInfo;
			if (flag)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.Method: 'type' is null");
				}
				methodInfo = null;
			}
			else
			{
				bool flag2 = name == null;
				if (flag2)
				{
					if (logErrorInTrace)
					{
						Trace.TraceError(string.Format("AccessTools2.Method: type '{0}', 'name' is null", type));
					}
					methodInfo = null;
				}
				else
				{
					bool flag3 = parameters == null;
					MethodInfo methodInfo2;
					if (flag3)
					{
						try
						{
							methodInfo2 = AccessTools2.FindIncludingBaseTypes<MethodInfo>(type, (Type t) => t.GetMethod(name, AccessTools.all));
						}
						catch (AmbiguousMatchException ex)
						{
							methodInfo2 = AccessTools2.FindIncludingBaseTypes<MethodInfo>(type, (Type t) => t.GetMethod(name, AccessTools.all, null, Type.EmptyTypes, new ParameterModifier[0]));
							bool flag4 = methodInfo2 == null;
							if (flag4)
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
						}
					}
					else
					{
						methodInfo2 = AccessTools2.FindIncludingBaseTypes<MethodInfo>(type, (Type t) => t.GetMethod(name, AccessTools.all, null, parameters, new ParameterModifier[0]));
					}
					bool flag5 = methodInfo2 == null;
					if (flag5)
					{
						if (logErrorInTrace)
						{
							string text2 = "AccessTools2.Method: Could not find method for type '{0}' and name '{1}' and parameters '{2}'";
							object name2 = name;
							Type[] parameters3 = parameters;
							Trace.TraceError(string.Format(text2, type, name2, (parameters3 != null) ? GeneralExtensions.Description(parameters3) : null));
						}
						methodInfo = null;
					}
					else
					{
						bool flag6 = generics != null;
						if (flag6)
						{
							methodInfo2 = methodInfo2.MakeGenericMethod(generics);
						}
						methodInfo = methodInfo2;
					}
				}
			}
			return methodInfo;
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00007828 File Offset: 0x00005A28
		[return: Nullable(2)]
		public static MethodInfo DeclaredMethod(string typeColonMethodname, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, [Nullable(new byte[] { 2, 1 })] Type[] generics = null, bool logErrorInTrace = true)
		{
			Type type;
			string text;
			bool flag = !AccessTools2.TryGetComponents(typeColonMethodname, out type, out text, logErrorInTrace);
			MethodInfo methodInfo;
			if (flag)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.Method: Could not find type or property for '" + typeColonMethodname + "'");
				}
				methodInfo = null;
			}
			else
			{
				methodInfo = AccessTools2.DeclaredMethod(type, text, parameters, generics, logErrorInTrace);
			}
			return methodInfo;
		}

		// Token: 0x0600009D RID: 157 RVA: 0x0000787C File Offset: 0x00005A7C
		[return: Nullable(2)]
		public static MethodInfo Method(string typeColonMethodname, [Nullable(new byte[] { 2, 1 })] Type[] parameters = null, [Nullable(new byte[] { 2, 1 })] Type[] generics = null, bool logErrorInTrace = true)
		{
			Type type;
			string text;
			bool flag = !AccessTools2.TryGetComponents(typeColonMethodname, out type, out text, logErrorInTrace);
			MethodInfo methodInfo;
			if (flag)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.Method: Could not find type or property for '" + typeColonMethodname + "'");
				}
				methodInfo = null;
			}
			else
			{
				methodInfo = AccessTools2.Method(type, text, parameters, generics, logErrorInTrace);
			}
			return methodInfo;
		}

		// Token: 0x0600009E RID: 158 RVA: 0x000078D0 File Offset: 0x00005AD0
		[return: Nullable(2)]
		public static PropertyInfo DeclaredProperty(Type type, string name, bool logErrorInTrace = true)
		{
			bool flag = type == null;
			PropertyInfo propertyInfo;
			if (flag)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.DeclaredProperty: 'type' is null");
				}
				propertyInfo = null;
			}
			else
			{
				bool flag2 = name == null;
				if (flag2)
				{
					if (logErrorInTrace)
					{
						Trace.TraceError(string.Format("AccessTools2.DeclaredProperty: type '{0}', 'name' is null", type));
					}
					propertyInfo = null;
				}
				else
				{
					PropertyInfo property = type.GetProperty(name, AccessTools.allDeclared);
					bool flag3 = property == null && logErrorInTrace;
					if (flag3)
					{
						Trace.TraceError(string.Format("AccessTools2.DeclaredProperty: Could not find property for type '{0}' and name '{1}'", type, name));
					}
					propertyInfo = property;
				}
			}
			return propertyInfo;
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00007954 File Offset: 0x00005B54
		[return: Nullable(2)]
		public static PropertyInfo Property(Type type, string name, bool logErrorInTrace = true)
		{
			bool flag = type == null;
			PropertyInfo propertyInfo;
			if (flag)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.Property: 'type' is null");
				}
				propertyInfo = null;
			}
			else
			{
				bool flag2 = name == null;
				if (flag2)
				{
					if (logErrorInTrace)
					{
						Trace.TraceError(string.Format("AccessTools2.Property: type '{0}', 'name' is null", type));
					}
					propertyInfo = null;
				}
				else
				{
					PropertyInfo propertyInfo2 = AccessTools2.FindIncludingBaseTypes<PropertyInfo>(type, (Type t) => t.GetProperty(name, AccessTools.all));
					bool flag3 = propertyInfo2 == null && logErrorInTrace;
					if (flag3)
					{
						Trace.TraceError(string.Format("AccessTools2.Property: Could not find property for type '{0}' and name '{1}'", type, name));
					}
					propertyInfo = propertyInfo2;
				}
			}
			return propertyInfo;
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x000079F7 File Offset: 0x00005BF7
		[return: Nullable(2)]
		public static MethodInfo DeclaredPropertyGetter(Type type, string name, bool logErrorInTrace = true)
		{
			PropertyInfo propertyInfo = AccessTools2.DeclaredProperty(type, name, logErrorInTrace);
			return (propertyInfo != null) ? propertyInfo.GetGetMethod(true) : null;
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00007A0E File Offset: 0x00005C0E
		[return: Nullable(2)]
		public static MethodInfo DeclaredPropertySetter(Type type, string name, bool logErrorInTrace = true)
		{
			PropertyInfo propertyInfo = AccessTools2.DeclaredProperty(type, name, logErrorInTrace);
			return (propertyInfo != null) ? propertyInfo.GetSetMethod(true) : null;
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00007A25 File Offset: 0x00005C25
		[return: Nullable(2)]
		public static MethodInfo PropertyGetter(Type type, string name, bool logErrorInTrace = true)
		{
			PropertyInfo propertyInfo = AccessTools2.Property(type, name, logErrorInTrace);
			return (propertyInfo != null) ? propertyInfo.GetGetMethod(true) : null;
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00007A3C File Offset: 0x00005C3C
		[return: Nullable(2)]
		public static MethodInfo PropertySetter(Type type, string name, bool logErrorInTrace = true)
		{
			PropertyInfo propertyInfo = AccessTools2.Property(type, name, logErrorInTrace);
			return (propertyInfo != null) ? propertyInfo.GetSetMethod(true) : null;
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00007A54 File Offset: 0x00005C54
		[return: Nullable(2)]
		public static PropertyInfo DeclaredProperty(string typeColonPropertyName, bool logErrorInTrace = true)
		{
			Type type;
			string text;
			bool flag = !AccessTools2.TryGetComponents(typeColonPropertyName, out type, out text, logErrorInTrace);
			PropertyInfo propertyInfo;
			if (flag)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.DeclaredProperty: Could not find type or property for '" + typeColonPropertyName + "'");
				}
				propertyInfo = null;
			}
			else
			{
				propertyInfo = AccessTools2.DeclaredProperty(type, text, logErrorInTrace);
			}
			return propertyInfo;
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00007AA4 File Offset: 0x00005CA4
		[return: Nullable(2)]
		public static PropertyInfo Property(string typeColonPropertyName, bool logErrorInTrace = true)
		{
			Type type;
			string text;
			bool flag = !AccessTools2.TryGetComponents(typeColonPropertyName, out type, out text, logErrorInTrace);
			PropertyInfo propertyInfo;
			if (flag)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.Property: Could not find type or property for '" + typeColonPropertyName + "'");
				}
				propertyInfo = null;
			}
			else
			{
				propertyInfo = AccessTools2.Property(type, text, logErrorInTrace);
			}
			return propertyInfo;
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00007AF3 File Offset: 0x00005CF3
		[return: Nullable(2)]
		public static MethodInfo DeclaredPropertySetter(string typeColonPropertyName, bool logErrorInTrace = true)
		{
			PropertyInfo propertyInfo = AccessTools2.DeclaredProperty(typeColonPropertyName, logErrorInTrace);
			return (propertyInfo != null) ? propertyInfo.GetSetMethod(true) : null;
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00007B09 File Offset: 0x00005D09
		[return: Nullable(2)]
		public static MethodInfo DeclaredPropertyGetter(string typeColonPropertyName, bool logErrorInTrace = true)
		{
			PropertyInfo propertyInfo = AccessTools2.DeclaredProperty(typeColonPropertyName, logErrorInTrace);
			return (propertyInfo != null) ? propertyInfo.GetGetMethod(true) : null;
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00007B1F File Offset: 0x00005D1F
		[return: Nullable(2)]
		public static MethodInfo PropertyGetter(string typeColonPropertyName, bool logErrorInTrace = true)
		{
			PropertyInfo propertyInfo = AccessTools2.Property(typeColonPropertyName, logErrorInTrace);
			return (propertyInfo != null) ? propertyInfo.GetGetMethod(true) : null;
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00007B35 File Offset: 0x00005D35
		[return: Nullable(2)]
		public static MethodInfo PropertySetter(string typeColonPropertyName, bool logErrorInTrace = true)
		{
			PropertyInfo propertyInfo = AccessTools2.Property(typeColonPropertyName, logErrorInTrace);
			return (propertyInfo != null) ? propertyInfo.GetSetMethod(true) : null;
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00007B4C File Offset: 0x00005D4C
		[return: Nullable(new byte[] { 2, 1 })]
		public static AccessTools.FieldRef<TField> StaticFieldRefAccess<[Nullable(2)] TField>(string typeColonFieldname, bool logErrorInTrace = true)
		{
			Type type;
			string text;
			bool flag = !AccessTools2.TryGetComponents(typeColonFieldname, out type, out text, logErrorInTrace);
			AccessTools.FieldRef<TField> fieldRef;
			if (flag)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.StaticFieldRefAccess: Could not find type or field for '" + typeColonFieldname + "'");
				}
				fieldRef = null;
			}
			else
			{
				fieldRef = AccessTools2.StaticFieldRefAccess<TField>(type, text, logErrorInTrace);
			}
			return fieldRef;
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00007B9C File Offset: 0x00005D9C
		[return: Nullable(new byte[] { 2, 1 })]
		public static AccessTools.FieldRef<F> StaticFieldRefAccess<[Nullable(2)] F>(FieldInfo fieldInfo, bool logErrorInTrace = true)
		{
			bool flag = fieldInfo == null;
			AccessTools.FieldRef<F> fieldRef;
			if (flag)
			{
				fieldRef = null;
			}
			else
			{
				fieldRef = AccessTools2.StaticFieldRefAccessInternal<F>(fieldInfo, logErrorInTrace);
			}
			return fieldRef;
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00007BC4 File Offset: 0x00005DC4
		[return: Nullable(new byte[] { 2, 1 })]
		public static AccessTools.FieldRef<TField> StaticFieldRefAccess<[Nullable(2)] TField>(Type type, string fieldName, bool logErrorInTrace = true)
		{
			FieldInfo fieldInfo = AccessTools2.Field(type, fieldName, logErrorInTrace);
			bool flag = fieldInfo == null;
			AccessTools.FieldRef<TField> fieldRef;
			if (flag)
			{
				fieldRef = null;
			}
			else
			{
				fieldRef = AccessTools2.StaticFieldRefAccess<TField>(fieldInfo, logErrorInTrace);
			}
			return fieldRef;
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00007BF4 File Offset: 0x00005DF4
		[return: Nullable(new byte[] { 2, 1 })]
		private static AccessTools.FieldRef<F> StaticFieldRefAccessInternal<[Nullable(2)] F>(FieldInfo fieldInfo, bool logErrorInTrace = true)
		{
			bool flag = !AccessTools2.Helper.IsValid(logErrorInTrace);
			AccessTools.FieldRef<F> fieldRef;
			if (flag)
			{
				fieldRef = null;
			}
			else
			{
				bool flag2 = !fieldInfo.IsStatic;
				if (flag2)
				{
					if (logErrorInTrace)
					{
						Trace.TraceError("AccessTools2.StaticFieldRefAccessInternal<" + typeof(F).FullName + ">: Field must be static");
					}
					fieldRef = null;
				}
				else
				{
					bool flag3 = !AccessTools2.ValidateFieldType<F>(fieldInfo, logErrorInTrace);
					if (flag3)
					{
						fieldRef = null;
					}
					else
					{
						string text = "__refget_";
						Type declaringType = fieldInfo.DeclaringType;
						AccessTools2.DynamicMethodDefinitionHandle? dynamicMethodDefinitionHandle = AccessTools2.DynamicMethodDefinitionHandle.Create(text + (((declaringType != null) ? declaringType.Name : null) ?? "null") + "_static_fi_" + fieldInfo.Name, typeof(F).MakeByRefType(), new Type[0]);
						AccessTools2.ILGeneratorHandle? ilgeneratorHandle = ((dynamicMethodDefinitionHandle != null) ? dynamicMethodDefinitionHandle.GetValueOrDefault().GetILGenerator() : null);
						AccessTools2.ILGeneratorHandle valueOrDefault;
						int num;
						if (ilgeneratorHandle != null)
						{
							valueOrDefault = ilgeneratorHandle.GetValueOrDefault();
							num = 1;
						}
						else
						{
							num = 0;
						}
						bool flag4 = num == 0;
						if (flag4)
						{
							fieldRef = null;
						}
						else
						{
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
							fieldRef = obj as AccessTools.FieldRef<F>;
						}
					}
				}
			}
			return fieldRef;
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00007D5C File Offset: 0x00005F5C
		[NullableContext(0)]
		[return: Nullable(new byte[] { 2, 0, 1 })]
		public static AccessTools.StructFieldRef<T, F> StructFieldRefAccess<T, [Nullable(2)] F>([Nullable(1)] string fieldName, bool logErrorInTrace = true) where T : struct
		{
			bool flag = string.IsNullOrEmpty(fieldName);
			AccessTools.StructFieldRef<T, F> structFieldRef;
			if (flag)
			{
				structFieldRef = null;
			}
			else
			{
				FieldInfo instanceField = AccessTools2.GetInstanceField(typeof(T), fieldName, logErrorInTrace);
				bool flag2 = instanceField == null;
				if (flag2)
				{
					structFieldRef = null;
				}
				else
				{
					structFieldRef = AccessTools2.StructFieldRefAccessInternal<T, F>(instanceField, logErrorInTrace);
				}
			}
			return structFieldRef;
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00007DA4 File Offset: 0x00005FA4
		[NullableContext(2)]
		[return: Nullable(new byte[] { 2, 0, 1 })]
		public static AccessTools.StructFieldRef<T, F> StructFieldRefAccess<[Nullable(0)] T, F>(FieldInfo fieldInfo, bool logErrorInTrace = true) where T : struct
		{
			bool flag = fieldInfo == null;
			AccessTools.StructFieldRef<T, F> structFieldRef;
			if (flag)
			{
				structFieldRef = null;
			}
			else
			{
				bool flag2 = !AccessTools2.ValidateStructField<T, F>(fieldInfo, logErrorInTrace);
				if (flag2)
				{
					structFieldRef = null;
				}
				else
				{
					structFieldRef = AccessTools2.StructFieldRefAccessInternal<T, F>(fieldInfo, logErrorInTrace);
				}
			}
			return structFieldRef;
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x00007DDC File Offset: 0x00005FDC
		[NullableContext(0)]
		[return: Nullable(new byte[] { 2, 0, 1 })]
		private static AccessTools.StructFieldRef<T, F> StructFieldRefAccessInternal<T, [Nullable(2)] F>([Nullable(1)] FieldInfo fieldInfo, bool logErrorInTrace = true) where T : struct
		{
			bool flag = !AccessTools2.ValidateFieldType<F>(fieldInfo, logErrorInTrace);
			AccessTools.StructFieldRef<T, F> structFieldRef;
			if (flag)
			{
				structFieldRef = null;
			}
			else
			{
				AccessTools2.DynamicMethodDefinitionHandle? dynamicMethodDefinitionHandle = AccessTools2.DynamicMethodDefinitionHandle.Create("__refget_" + typeof(T).Name + "_struct_fi_" + fieldInfo.Name, typeof(F).MakeByRefType(), new Type[] { typeof(T).MakeByRefType() });
				AccessTools2.ILGeneratorHandle? ilgeneratorHandle = ((dynamicMethodDefinitionHandle != null) ? dynamicMethodDefinitionHandle.GetValueOrDefault().GetILGenerator() : null);
				AccessTools2.ILGeneratorHandle valueOrDefault;
				int num;
				if (ilgeneratorHandle != null)
				{
					valueOrDefault = ilgeneratorHandle.GetValueOrDefault();
					num = 1;
				}
				else
				{
					num = 0;
				}
				bool flag2 = num == 0;
				if (flag2)
				{
					structFieldRef = null;
				}
				else
				{
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
					structFieldRef = obj as AccessTools.StructFieldRef<T, F>;
				}
			}
			return structFieldRef;
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x00007EFC File Offset: 0x000060FC
		public static IEnumerable<Assembly> AllAssemblies()
		{
			return from a in AppDomain.CurrentDomain.GetAssemblies()
				where !a.FullName.StartsWith("Microsoft.VisualStudio")
				select a;
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x00007F2C File Offset: 0x0000612C
		public static IEnumerable<Type> AllTypes()
		{
			return AccessTools2.AllAssemblies().SelectMany<Assembly, Type>((Assembly a) => AccessTools2.GetTypesFromAssembly(a, true));
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x00007F58 File Offset: 0x00006158
		public static Type[] GetTypesFromAssembly(Assembly assembly, bool logErrorInTrace = true)
		{
			bool flag = assembly == null;
			Type[] array;
			if (flag)
			{
				array = Type.EmptyTypes;
			}
			else
			{
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
			}
			return array;
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00007FE0 File Offset: 0x000061E0
		public static Type[] GetTypesFromAssemblyIfValid(Assembly assembly, bool logErrorInTrace = true)
		{
			bool flag = assembly == null;
			Type[] array;
			if (flag)
			{
				array = Type.EmptyTypes;
			}
			else
			{
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
			}
			return array;
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x0000803C File Offset: 0x0000623C
		[return: Nullable(2)]
		public static Type TypeByName(string name, bool logErrorInTrace = true)
		{
			bool flag = string.IsNullOrEmpty(name);
			Type type;
			if (flag)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.TypeByName: 'name' is null or empty");
				}
				type = null;
			}
			else
			{
				Type type2 = Type.GetType(name, false);
				bool flag2 = type2 == null;
				if (flag2)
				{
					type2 = AccessTools2.AllTypes().FirstOrDefault<Type>((Type t) => t.FullName == name);
				}
				bool flag3 = type2 == null;
				if (flag3)
				{
					type2 = AccessTools2.AllTypes().FirstOrDefault<Type>((Type t) => t.Name == name);
				}
				bool flag4 = type2 == null && logErrorInTrace;
				if (flag4)
				{
					Trace.TraceError("AccessTools2.TypeByName: Could not find type named '" + name + "'");
				}
				type = type2;
			}
			return type;
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x000080FC File Offset: 0x000062FC
		[return: Nullable(2)]
		public static T FindIncludingBaseTypes<T>(Type type, Func<Type, T> func) where T : class
		{
			bool flag = type == null || func == null;
			T t;
			if (flag)
			{
				t = default(T);
			}
			else
			{
				T t2;
				for (;;)
				{
					t2 = func(type);
					bool flag2 = t2 != null;
					if (flag2)
					{
						break;
					}
					type = type.BaseType;
					bool flag3 = type == null;
					if (flag3)
					{
						goto Block_4;
					}
				}
				return t2;
				Block_4:
				t = default(T);
			}
			return t;
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x00008168 File Offset: 0x00006368
		[return: Nullable(2)]
		private static FieldInfo GetInstanceField(Type type, string fieldName, bool logErrorInTrace = true)
		{
			FieldInfo fieldInfo = AccessTools2.Field(type, fieldName, logErrorInTrace);
			bool flag = fieldInfo == null;
			FieldInfo fieldInfo2;
			if (flag)
			{
				fieldInfo2 = null;
			}
			else
			{
				bool isStatic = fieldInfo.IsStatic;
				if (isStatic)
				{
					if (logErrorInTrace)
					{
						Trace.TraceError(string.Format("AccessTools2.GetInstanceField: Field must not be static, type '{0}', fieldName '{1}'", type, fieldName));
					}
					fieldInfo2 = null;
				}
				else
				{
					fieldInfo2 = fieldInfo;
				}
			}
			return fieldInfo2;
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x000081B8 File Offset: 0x000063B8
		[NullableContext(2)]
		private static bool ValidateFieldType<F>(FieldInfo fieldInfo, bool logErrorInTrace = true)
		{
			bool flag = fieldInfo == null;
			bool flag2;
			if (flag)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.ValidateFieldType<" + typeof(F).FullName + ">: 'fieldInfo' is null");
				}
				flag2 = false;
			}
			else
			{
				Type typeFromHandle = typeof(F);
				Type fieldType = fieldInfo.FieldType;
				bool flag3 = typeFromHandle == fieldType;
				if (flag3)
				{
					flag2 = true;
				}
				else
				{
					bool isEnum = fieldType.IsEnum;
					if (isEnum)
					{
						Type underlyingType = Enum.GetUnderlyingType(fieldType);
						bool flag4 = typeFromHandle != underlyingType;
						if (flag4)
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
						bool isValueType = fieldType.IsValueType;
						if (isValueType)
						{
							if (logErrorInTrace)
							{
								Trace.TraceError(string.Format("AccessTools2.ValidateFieldType<{0}>: FieldRefAccess return type must be the same as FieldType for value types, fieldInfo '{1}'", typeof(F).FullName, fieldInfo));
							}
							return false;
						}
						bool flag5 = !typeFromHandle.IsAssignableFrom(fieldType);
						if (flag5)
						{
							if (logErrorInTrace)
							{
								Trace.TraceError("AccessTools2.ValidateFieldType<" + typeof(F).FullName + ">: FieldRefAccess return type must be assignable from FieldType for reference types");
							}
							return false;
						}
					}
					flag2 = true;
				}
			}
			return flag2;
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x00008304 File Offset: 0x00006504
		[NullableContext(2)]
		private static bool ValidateStructField<[Nullable(0)] T, F>(FieldInfo fieldInfo, bool logErrorInTrace = true) where T : struct
		{
			bool flag = fieldInfo == null;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool isStatic = fieldInfo.IsStatic;
				if (isStatic)
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
					flag2 = false;
				}
				else
				{
					bool flag3 = fieldInfo.DeclaringType != typeof(T);
					if (flag3)
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
						flag2 = false;
					}
					else
					{
						flag2 = true;
					}
				}
			}
			return flag2;
		}

		// Token: 0x060000BA RID: 186 RVA: 0x000083FC File Offset: 0x000065FC
		[NullableContext(2)]
		private static bool TryGetComponents([Nullable(1)] string typeColonName, out Type type, out string name, bool logErrorInTrace = true)
		{
			bool flag = string.IsNullOrWhiteSpace(typeColonName);
			bool flag2;
			if (flag)
			{
				if (logErrorInTrace)
				{
					Trace.TraceError("AccessTools2.TryGetComponents: 'typeColonName' is null or whitespace/empty");
				}
				type = null;
				name = null;
				flag2 = false;
			}
			else
			{
				string[] array = typeColonName.Split(new char[] { ':' });
				bool flag3 = array.Length != 2;
				if (flag3)
				{
					if (logErrorInTrace)
					{
						Trace.TraceError("AccessTools2.TryGetComponents: typeColonName '" + typeColonName + "', name must be specified as 'Namespace.Type1.Type2:Name");
					}
					type = null;
					name = null;
					flag2 = false;
				}
				else
				{
					type = AccessTools2.TypeByName(array[0], logErrorInTrace);
					name = array[1];
					flag2 = type != null;
				}
			}
			return flag2;
		}

		// Token: 0x04000055 RID: 85
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

		// Token: 0x0200001C RID: 28
		[Nullable(0)]
		[ExcludeFromCodeCoverage]
		private readonly struct DynamicMethodDefinitionHandle
		{
			// Token: 0x06000140 RID: 320 RVA: 0x00009C7C File Offset: 0x00007E7C
			public static AccessTools2.DynamicMethodDefinitionHandle? Create(string name, Type returnType, Type[] parameterTypes)
			{
				return (AccessTools2.Helper.DynamicMethodDefinitionCtor == null) ? null : new AccessTools2.DynamicMethodDefinitionHandle?(new AccessTools2.DynamicMethodDefinitionHandle(AccessTools2.Helper.DynamicMethodDefinitionCtor(name, returnType, parameterTypes)));
			}

			// Token: 0x06000141 RID: 321 RVA: 0x00009CB2 File Offset: 0x00007EB2
			public DynamicMethodDefinitionHandle(object dynamicMethodDefinition)
			{
				this._dynamicMethodDefinition = dynamicMethodDefinition;
			}

			// Token: 0x06000142 RID: 322 RVA: 0x00009CBC File Offset: 0x00007EBC
			public AccessTools2.ILGeneratorHandle? GetILGenerator()
			{
				return (AccessTools2.Helper.GetILGenerator == null) ? null : new AccessTools2.ILGeneratorHandle?(new AccessTools2.ILGeneratorHandle(AccessTools2.Helper.GetILGenerator(this._dynamicMethodDefinition)));
			}

			// Token: 0x06000143 RID: 323 RVA: 0x00009CF5 File Offset: 0x00007EF5
			[NullableContext(2)]
			public MethodInfo Generate()
			{
				return (AccessTools2.Helper.Generate == null) ? null : AccessTools2.Helper.Generate(this._dynamicMethodDefinition);
			}

			// Token: 0x04000070 RID: 112
			private readonly object _dynamicMethodDefinition;
		}

		// Token: 0x0200001D RID: 29
		[Nullable(0)]
		[ExcludeFromCodeCoverage]
		private readonly struct ILGeneratorHandle
		{
			// Token: 0x06000144 RID: 324 RVA: 0x00009D11 File Offset: 0x00007F11
			public ILGeneratorHandle(object ilGenerator)
			{
				this._ilGenerator = ilGenerator;
			}

			// Token: 0x06000145 RID: 325 RVA: 0x00009D1A File Offset: 0x00007F1A
			public void Emit(OpCode opcode)
			{
				AccessTools2.Helper.Emit1Delegate emit = AccessTools2.Helper.Emit1;
				if (emit != null)
				{
					emit(this._ilGenerator, opcode);
				}
			}

			// Token: 0x06000146 RID: 326 RVA: 0x00009D34 File Offset: 0x00007F34
			public void Emit(OpCode opcode, FieldInfo field)
			{
				AccessTools2.Helper.Emit2Delegate emit = AccessTools2.Helper.Emit2;
				if (emit != null)
				{
					emit(this._ilGenerator, opcode, field);
				}
			}

			// Token: 0x06000147 RID: 327 RVA: 0x00009D4F File Offset: 0x00007F4F
			public void Emit(OpCode opcode, Type type)
			{
				AccessTools2.Helper.Emit3Delegate emit = AccessTools2.Helper.Emit3;
				if (emit != null)
				{
					emit(this._ilGenerator, opcode, type);
				}
			}

			// Token: 0x04000071 RID: 113
			private readonly object _ilGenerator;
		}

		// Token: 0x0200001E RID: 30
		[NullableContext(0)]
		[ExcludeFromCodeCoverage]
		private static class Helper
		{
			// Token: 0x06000149 RID: 329 RVA: 0x00009E6C File Offset: 0x0000806C
			public static bool IsValid(bool logErrorInTrace = true)
			{
				bool flag = AccessTools2.Helper.DynamicMethodDefinitionCtor == null;
				bool flag2;
				if (flag)
				{
					if (logErrorInTrace)
					{
						Trace.TraceError("AccessTools2.Helper.IsValid: DynamicMethodDefinitionCtor is null");
					}
					flag2 = false;
				}
				else
				{
					bool flag3 = AccessTools2.Helper.GetILGenerator == null;
					if (flag3)
					{
						if (logErrorInTrace)
						{
							Trace.TraceError("AccessTools2.Helper.IsValid: GetILGenerator is null");
						}
						flag2 = false;
					}
					else
					{
						bool flag4 = AccessTools2.Helper.Emit1 == null;
						if (flag4)
						{
							if (logErrorInTrace)
							{
								Trace.TraceError("AccessTools2.Helper.IsValid: Emit1 is null");
							}
							flag2 = false;
						}
						else
						{
							bool flag5 = AccessTools2.Helper.Emit2 == null;
							if (flag5)
							{
								if (logErrorInTrace)
								{
									Trace.TraceError("AccessTools2.Helper.IsValid: Emit2 is null");
								}
								flag2 = false;
							}
							else
							{
								bool flag6 = AccessTools2.Helper.Emit3 == null;
								if (flag6)
								{
									if (logErrorInTrace)
									{
										Trace.TraceError("AccessTools2.Helper.IsValid: Emit3 is null");
									}
									flag2 = false;
								}
								else
								{
									bool flag7 = AccessTools2.Helper.Generate == null;
									if (flag7)
									{
										if (logErrorInTrace)
										{
											Trace.TraceError("AccessTools2.Helper.IsValid: Generate is null");
										}
										flag2 = false;
									}
									else
									{
										flag2 = true;
									}
								}
							}
						}
					}
				}
				return flag2;
			}

			// Token: 0x04000072 RID: 114
			[Nullable(2)]
			public static readonly AccessTools2.Helper.DynamicMethodDefinitionCtorDelegate DynamicMethodDefinitionCtor = AccessTools2.GetDeclaredConstructorDelegate<AccessTools2.Helper.DynamicMethodDefinitionCtorDelegate>("MonoMod.Utils.DynamicMethodDefinition", new Type[]
			{
				typeof(string),
				typeof(Type),
				typeof(Type[])
			}, true);

			// Token: 0x04000073 RID: 115
			[Nullable(2)]
			public static readonly AccessTools2.Helper.GetILGeneratorDelegate GetILGenerator = AccessTools2.GetDelegateObjectInstance<AccessTools2.Helper.GetILGeneratorDelegate>("MonoMod.Utils.DynamicMethodDefinition:GetILGenerator", Type.EmptyTypes, null, true);

			// Token: 0x04000074 RID: 116
			[Nullable(2)]
			public static readonly AccessTools2.Helper.Emit1Delegate Emit1 = AccessTools2.GetDelegateObjectInstance<AccessTools2.Helper.Emit1Delegate>("System.Reflection.Emit.ILGenerator:Emit", new Type[] { typeof(OpCode) }, null, true);

			// Token: 0x04000075 RID: 117
			[Nullable(2)]
			public static readonly AccessTools2.Helper.Emit2Delegate Emit2 = AccessTools2.GetDelegateObjectInstance<AccessTools2.Helper.Emit2Delegate>("System.Reflection.Emit.ILGenerator:Emit", new Type[]
			{
				typeof(OpCode),
				typeof(FieldInfo)
			}, null, true);

			// Token: 0x04000076 RID: 118
			[Nullable(2)]
			public static readonly AccessTools2.Helper.Emit3Delegate Emit3 = AccessTools2.GetDelegateObjectInstance<AccessTools2.Helper.Emit3Delegate>("System.Reflection.Emit.ILGenerator:Emit", new Type[]
			{
				typeof(OpCode),
				typeof(Type)
			}, null, true);

			// Token: 0x04000077 RID: 119
			[Nullable(2)]
			public static readonly AccessTools2.Helper.GenerateDelegate Generate = AccessTools2.GetDelegateObjectInstance<AccessTools2.Helper.GenerateDelegate>("MonoMod.Utils.DynamicMethodDefinition:Generate", Type.EmptyTypes, null, true);

			// Token: 0x02000030 RID: 48
			// (Invoke) Token: 0x06000176 RID: 374
			public delegate object DynamicMethodDefinitionCtorDelegate(string name, Type returnType, Type[] parameterTypes);

			// Token: 0x02000031 RID: 49
			// (Invoke) Token: 0x0600017A RID: 378
			public delegate object GetILGeneratorDelegate(object instance);

			// Token: 0x02000032 RID: 50
			// (Invoke) Token: 0x0600017E RID: 382
			public delegate void Emit1Delegate(object instance, OpCode opcode);

			// Token: 0x02000033 RID: 51
			// (Invoke) Token: 0x06000182 RID: 386
			public delegate void Emit2Delegate(object instance, OpCode opcode, FieldInfo field);

			// Token: 0x02000034 RID: 52
			// (Invoke) Token: 0x06000186 RID: 390
			public delegate void Emit3Delegate(object instance, OpCode opcode, Type type);

			// Token: 0x02000035 RID: 53
			// (Invoke) Token: 0x0600018A RID: 394
			public delegate MethodInfo GenerateDelegate(object instance);
		}
	}
}
