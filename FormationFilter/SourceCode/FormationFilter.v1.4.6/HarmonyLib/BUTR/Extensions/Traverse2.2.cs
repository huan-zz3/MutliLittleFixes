using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace HarmonyLib.BUTR.Extensions
{
	// Token: 0x0200002A RID: 42
	[NullableContext(1)]
	[Nullable(0)]
	internal class Traverse2
	{
		// Token: 0x060001E8 RID: 488 RVA: 0x0000BFDC File Offset: 0x0000A1DC
		[MethodImpl(MethodImplOptions.Synchronized)]
		static Traverse2()
		{
			if (Traverse2.Cache == null)
			{
				Traverse2.Cache = AccessCacheHandle.Create();
			}
		}

		// Token: 0x060001E9 RID: 489 RVA: 0x0000C017 File Offset: 0x0000A217
		public static Traverse2 Create([Nullable(2)] Type type)
		{
			return new Traverse2(type);
		}

		// Token: 0x060001EA RID: 490 RVA: 0x0000C01F File Offset: 0x0000A21F
		public static Traverse2 Create<[Nullable(2)] T>()
		{
			return Traverse2.Create(typeof(T));
		}

		// Token: 0x060001EB RID: 491 RVA: 0x0000C030 File Offset: 0x0000A230
		public static Traverse2 Create([Nullable(2)] object root)
		{
			return new Traverse2(root);
		}

		// Token: 0x060001EC RID: 492 RVA: 0x0000C038 File Offset: 0x0000A238
		public static Traverse2 CreateWithType(string name)
		{
			return new Traverse2(AccessTools2.TypeByName(name, true));
		}

		// Token: 0x060001ED RID: 493 RVA: 0x0000C046 File Offset: 0x0000A246
		private Traverse2()
		{
		}

		// Token: 0x060001EE RID: 494 RVA: 0x0000C04E File Offset: 0x0000A24E
		[NullableContext(2)]
		public Traverse2(Type type)
		{
			this._type = type;
		}

		// Token: 0x060001EF RID: 495 RVA: 0x0000C05D File Offset: 0x0000A25D
		[NullableContext(2)]
		public Traverse2(object root)
		{
			this._root = root;
			this._type = ((root != null) ? root.GetType() : null);
		}

		// Token: 0x060001F0 RID: 496 RVA: 0x0000C07E File Offset: 0x0000A27E
		private Traverse2([Nullable(2)] object root, MemberInfo info, [Nullable(new byte[] { 2, 1 })] object[] index)
		{
			this._root = root;
			this._type = ((root != null) ? root.GetType() : null) ?? AccessTools.GetUnderlyingType(info);
			this._info = info;
			this._params = index;
		}

		// Token: 0x060001F1 RID: 497 RVA: 0x0000C0B7 File Offset: 0x0000A2B7
		private Traverse2([Nullable(2)] object root, MethodInfo method, [Nullable(new byte[] { 2, 1 })] object[] parameter)
		{
			this._root = root;
			this._type = method.ReturnType;
			this._method = method;
			this._params = parameter;
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x0000C0E0 File Offset: 0x0000A2E0
		[NullableContext(2)]
		public object GetValue()
		{
			FieldInfo fieldInfo = this._info as FieldInfo;
			if (fieldInfo != null)
			{
				return fieldInfo.GetValue(this._root);
			}
			PropertyInfo propertyInfo = this._info as PropertyInfo;
			if (propertyInfo != null)
			{
				return propertyInfo.GetValue(this._root, AccessTools.all, null, this._params, CultureInfo.CurrentCulture);
			}
			MethodBase method = this._method;
			if (method != null)
			{
				return method.Invoke(this._root, this._params);
			}
			if (this._root == null && this._type != null)
			{
				return this._type;
			}
			return this._root;
		}

		// Token: 0x060001F3 RID: 499 RVA: 0x0000C170 File Offset: 0x0000A370
		[NullableContext(2)]
		public T GetValue<T>()
		{
			object value = this.GetValue();
			if (value is T)
			{
				return (T)((object)value);
			}
			return default(T);
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x0000C19E File Offset: 0x0000A39E
		[return: Nullable(2)]
		public object GetValue(params object[] arguments)
		{
			MethodBase method = this._method;
			if (method == null)
			{
				return null;
			}
			return method.Invoke(this._root, arguments);
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x0000C1B8 File Offset: 0x0000A3B8
		[NullableContext(2)]
		public T GetValue<T>([Nullable(1)] params object[] arguments)
		{
			MethodBase method = this._method;
			object obj = ((method != null) ? method.Invoke(this._root, arguments) : null);
			if (obj is T)
			{
				return (T)((object)obj);
			}
			return default(T);
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x0000C1FC File Offset: 0x0000A3FC
		public Traverse2 SetValue(object value)
		{
			FieldInfo fieldInfo = this._info as FieldInfo;
			if (fieldInfo != null && ((this._root == null && fieldInfo.IsStatic) || this._root != null))
			{
				fieldInfo.SetValue(this._root, value, AccessTools.all, null, CultureInfo.CurrentCulture);
			}
			PropertyInfo propertyInfo = this._info as PropertyInfo;
			if (propertyInfo != null && propertyInfo.SetMethod != null && ((this._root == null && propertyInfo.SetMethod.IsStatic) || this._root != null))
			{
				propertyInfo.SetValue(this._root, value, AccessTools.all, null, this._params, CultureInfo.CurrentCulture);
			}
			return this;
		}

		// Token: 0x060001F7 RID: 503 RVA: 0x0000C29C File Offset: 0x0000A49C
		[NullableContext(2)]
		public Type GetValueType()
		{
			FieldInfo fieldInfo = this._info as FieldInfo;
			if (fieldInfo != null)
			{
				return fieldInfo.FieldType;
			}
			PropertyInfo propertyInfo = this._info as PropertyInfo;
			if (propertyInfo != null)
			{
				return propertyInfo.PropertyType;
			}
			return null;
		}

		// Token: 0x060001F8 RID: 504 RVA: 0x0000C2D8 File Offset: 0x0000A4D8
		private Traverse2 Resolve()
		{
			if (this._root == null)
			{
				FieldInfo fieldInfo = this._info as FieldInfo;
				if (fieldInfo != null && fieldInfo.IsStatic)
				{
					return new Traverse2(this.GetValue());
				}
				PropertyInfo propertyInfo = this._info as PropertyInfo;
				if (propertyInfo != null && propertyInfo.GetGetMethod().IsStatic)
				{
					return new Traverse2(this.GetValue());
				}
				MethodBase method = this._method;
				if (method != null && method.IsStatic)
				{
					return new Traverse2(this.GetValue());
				}
				if (this._type != null)
				{
					return this;
				}
			}
			return new Traverse2(this.GetValue());
		}

		// Token: 0x060001F9 RID: 505 RVA: 0x0000C36C File Offset: 0x0000A56C
		public Traverse2 Type(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return new Traverse2();
			}
			if (this._type == null)
			{
				return new Traverse2();
			}
			Type type = AccessTools.Inner(this._type, name);
			if (type == null)
			{
				return new Traverse2();
			}
			return new Traverse2(type);
		}

		// Token: 0x060001FA RID: 506 RVA: 0x0000C3B4 File Offset: 0x0000A5B4
		public Traverse2 Field(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return new Traverse2();
			}
			Traverse2 traverse = this.Resolve();
			if (traverse._type == null)
			{
				return new Traverse2();
			}
			FieldInfo fieldInfo = ((Traverse2.Cache != null) ? Traverse2.Cache.GetValueOrDefault().GetFieldInfo(traverse._type, name, AccessCacheHandle.MemberType.Any, false) : null);
			if (fieldInfo == null)
			{
				return new Traverse2();
			}
			if (!fieldInfo.IsStatic && traverse._root == null)
			{
				return new Traverse2();
			}
			return new Traverse2(traverse._root, fieldInfo, null);
		}

		// Token: 0x060001FB RID: 507 RVA: 0x0000C438 File Offset: 0x0000A638
		public Traverse2<T> Field<[Nullable(2)] T>(string name)
		{
			return new Traverse2<T>(this.Field(name));
		}

		// Token: 0x060001FC RID: 508 RVA: 0x0000C448 File Offset: 0x0000A648
		public List<string> Fields()
		{
			Traverse2 traverse = this.Resolve();
			return AccessTools.GetFieldNames(traverse._type);
		}

		// Token: 0x060001FD RID: 509 RVA: 0x0000C468 File Offset: 0x0000A668
		public Traverse2 Property(string name, [Nullable(new byte[] { 2, 1 })] object[] index = null)
		{
			if (string.IsNullOrEmpty(name))
			{
				return new Traverse2();
			}
			Traverse2 traverse = this.Resolve();
			if (traverse._type == null)
			{
				return new Traverse2();
			}
			PropertyInfo propertyInfo = ((Traverse2.Cache != null) ? Traverse2.Cache.GetValueOrDefault().GetPropertyInfo(traverse._type, name, AccessCacheHandle.MemberType.Any, false) : null);
			if (propertyInfo == null)
			{
				return new Traverse2();
			}
			return new Traverse2(traverse._root, propertyInfo, index);
		}

		// Token: 0x060001FE RID: 510 RVA: 0x0000C4D6 File Offset: 0x0000A6D6
		public Traverse2<T> Property<[Nullable(2)] T>(string name, [Nullable(new byte[] { 2, 1 })] object[] index = null)
		{
			return new Traverse2<T>(this.Property(name, index));
		}

		// Token: 0x060001FF RID: 511 RVA: 0x0000C4E8 File Offset: 0x0000A6E8
		public List<string> Properties()
		{
			Traverse2 traverse = this.Resolve();
			return AccessTools.GetPropertyNames(traverse._type);
		}

		// Token: 0x06000200 RID: 512 RVA: 0x0000C508 File Offset: 0x0000A708
		public Traverse2 Method(string name, params object[] arguments)
		{
			if (string.IsNullOrEmpty(name))
			{
				return new Traverse2();
			}
			Traverse2 traverse = this.Resolve();
			if (traverse._type == null)
			{
				return new Traverse2();
			}
			Type[] types = AccessTools.GetTypes(arguments);
			MethodBase methodBase = ((Traverse2.Cache != null) ? Traverse2.Cache.GetValueOrDefault().GetMethodInfo(traverse._type, name, types, AccessCacheHandle.MemberType.Any, false) : null);
			MethodInfo methodInfo = methodBase as MethodInfo;
			if (methodInfo == null)
			{
				return new Traverse2();
			}
			return new Traverse2(traverse._root, methodInfo, arguments);
		}

		// Token: 0x06000201 RID: 513 RVA: 0x0000C588 File Offset: 0x0000A788
		public Traverse2 Method(string name, Type[] paramTypes, [Nullable(new byte[] { 2, 1 })] object[] arguments = null)
		{
			if (string.IsNullOrEmpty(name))
			{
				return new Traverse2();
			}
			Traverse2 traverse = this.Resolve();
			if (traverse._type == null)
			{
				return new Traverse2();
			}
			MethodBase methodBase = ((Traverse2.Cache != null) ? Traverse2.Cache.GetValueOrDefault().GetMethodInfo(traverse._type, name, paramTypes, AccessCacheHandle.MemberType.Any, false) : null);
			MethodInfo methodInfo = methodBase as MethodInfo;
			if (methodInfo == null)
			{
				return new Traverse2();
			}
			return new Traverse2(traverse._root, methodInfo, arguments);
		}

		// Token: 0x06000202 RID: 514 RVA: 0x0000C600 File Offset: 0x0000A800
		public List<string> Methods()
		{
			Traverse2 traverse = this.Resolve();
			return AccessTools.GetMethodNames(traverse._type);
		}

		// Token: 0x06000203 RID: 515 RVA: 0x0000C61F File Offset: 0x0000A81F
		public bool FieldExists()
		{
			return this._info is FieldInfo;
		}

		// Token: 0x06000204 RID: 516 RVA: 0x0000C62F File Offset: 0x0000A82F
		public bool PropertyExists()
		{
			return this._info is PropertyInfo;
		}

		// Token: 0x06000205 RID: 517 RVA: 0x0000C63F File Offset: 0x0000A83F
		public bool MethodExists()
		{
			return this._method != null;
		}

		// Token: 0x06000206 RID: 518 RVA: 0x0000C64D File Offset: 0x0000A84D
		public bool TypeExists()
		{
			return this._type != null;
		}

		// Token: 0x06000207 RID: 519 RVA: 0x0000C65C File Offset: 0x0000A85C
		public static void IterateFields(object source, Action<Traverse2> action)
		{
			if (action == null)
			{
				return;
			}
			Traverse2 sourceTrv = Traverse2.Create(source);
			AccessTools.GetFieldNames(source).ForEach(delegate(string f)
			{
				action(sourceTrv.Field(f));
			});
		}

		// Token: 0x06000208 RID: 520 RVA: 0x0000C6A4 File Offset: 0x0000A8A4
		public static void IterateFields(object source, object target, Action<Traverse2, Traverse2> action)
		{
			if (action == null)
			{
				return;
			}
			Traverse2 sourceTrv = Traverse2.Create(source);
			Traverse2 targetTrv = Traverse2.Create(target);
			AccessTools.GetFieldNames(source).ForEach(delegate(string f)
			{
				action(sourceTrv.Field(f), targetTrv.Field(f));
			});
		}

		// Token: 0x06000209 RID: 521 RVA: 0x0000C6F8 File Offset: 0x0000A8F8
		public static void IterateFields(object source, object target, Action<string, Traverse2, Traverse2> action)
		{
			if (action == null)
			{
				return;
			}
			Traverse2 sourceTrv = Traverse2.Create(source);
			Traverse2 targetTrv = Traverse2.Create(target);
			AccessTools.GetFieldNames(source).ForEach(delegate(string f)
			{
				action(f, sourceTrv.Field(f), targetTrv.Field(f));
			});
		}

		// Token: 0x0600020A RID: 522 RVA: 0x0000C74C File Offset: 0x0000A94C
		public static void IterateProperties(object source, Action<Traverse2> action)
		{
			if (action == null)
			{
				return;
			}
			Traverse2 sourceTrv = Traverse2.Create(source);
			AccessTools.GetPropertyNames(source).ForEach(delegate(string f)
			{
				action(sourceTrv.Property(f, null));
			});
		}

		// Token: 0x0600020B RID: 523 RVA: 0x0000C794 File Offset: 0x0000A994
		public static void IterateProperties(object source, object target, Action<Traverse2, Traverse2> action)
		{
			if (action == null)
			{
				return;
			}
			Traverse2 sourceTrv = Traverse2.Create(source);
			Traverse2 targetTrv = Traverse2.Create(target);
			AccessTools.GetPropertyNames(source).ForEach(delegate(string f)
			{
				action(sourceTrv.Property(f, null), targetTrv.Property(f, null));
			});
		}

		// Token: 0x0600020C RID: 524 RVA: 0x0000C7E8 File Offset: 0x0000A9E8
		public static void IterateProperties(object source, object target, Action<string, Traverse2, Traverse2> action)
		{
			if (action == null)
			{
				return;
			}
			Traverse2 sourceTrv = Traverse2.Create(source);
			Traverse2 targetTrv = Traverse2.Create(target);
			AccessTools.GetPropertyNames(source).ForEach(delegate(string f)
			{
				action(f, sourceTrv.Property(f, null), targetTrv.Property(f, null));
			});
		}

		// Token: 0x0600020D RID: 525 RVA: 0x0000C83A File Offset: 0x0000AA3A
		[NullableContext(2)]
		public override string ToString()
		{
			MethodBase methodBase = this._method ?? this.GetValue();
			if (methodBase == null)
			{
				return null;
			}
			return methodBase.ToString();
		}

		// Token: 0x040000A2 RID: 162
		private static readonly AccessCacheHandle? Cache;

		// Token: 0x040000A3 RID: 163
		[Nullable(2)]
		private readonly Type _type;

		// Token: 0x040000A4 RID: 164
		[Nullable(2)]
		private readonly object _root;

		// Token: 0x040000A5 RID: 165
		[Nullable(2)]
		private readonly MemberInfo _info;

		// Token: 0x040000A6 RID: 166
		[Nullable(2)]
		private readonly MethodBase _method;

		// Token: 0x040000A7 RID: 167
		[Nullable(new byte[] { 2, 1 })]
		private readonly object[] _params;

		// Token: 0x040000A8 RID: 168
		public static Action<Traverse2, Traverse2> CopyFields = delegate(Traverse2 from, Traverse2 to)
		{
			if (from == null || to == null)
			{
				return;
			}
			to.SetValue(from.GetValue());
		};
	}
}
