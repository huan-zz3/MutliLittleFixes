using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace HarmonyLib.BUTR.Extensions
{
	// Token: 0x02000010 RID: 16
	[NullableContext(1)]
	[Nullable(0)]
	internal class Traverse2
	{
		// Token: 0x060000F7 RID: 247 RVA: 0x000090AC File Offset: 0x000072AC
		[MethodImpl(MethodImplOptions.Synchronized)]
		static Traverse2()
		{
			if (Traverse2.Cache == null)
			{
				Traverse2.Cache = AccessCacheHandle.Create();
			}
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x000090EA File Offset: 0x000072EA
		public static Traverse2 Create([Nullable(2)] Type type)
		{
			return new Traverse2(type);
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x000090F2 File Offset: 0x000072F2
		public static Traverse2 Create<[Nullable(2)] T>()
		{
			return Traverse2.Create(typeof(T));
		}

		// Token: 0x060000FA RID: 250 RVA: 0x00009103 File Offset: 0x00007303
		public static Traverse2 Create([Nullable(2)] object root)
		{
			return new Traverse2(root);
		}

		// Token: 0x060000FB RID: 251 RVA: 0x0000910B File Offset: 0x0000730B
		public static Traverse2 CreateWithType(string name)
		{
			return new Traverse2(AccessTools2.TypeByName(name, true));
		}

		// Token: 0x060000FC RID: 252 RVA: 0x00009119 File Offset: 0x00007319
		private Traverse2()
		{
		}

		// Token: 0x060000FD RID: 253 RVA: 0x00009123 File Offset: 0x00007323
		[NullableContext(2)]
		public Traverse2(Type type)
		{
			this._type = type;
		}

		// Token: 0x060000FE RID: 254 RVA: 0x00009134 File Offset: 0x00007334
		[NullableContext(2)]
		public Traverse2(object root)
		{
			this._root = root;
			this._type = ((root != null) ? root.GetType() : null);
		}

		// Token: 0x060000FF RID: 255 RVA: 0x00009157 File Offset: 0x00007357
		private Traverse2([Nullable(2)] object root, MemberInfo info, [Nullable(new byte[] { 2, 1 })] object[] index)
		{
			this._root = root;
			this._type = ((root != null) ? root.GetType() : null) ?? AccessTools.GetUnderlyingType(info);
			this._info = info;
			this._params = index;
		}

		// Token: 0x06000100 RID: 256 RVA: 0x00009192 File Offset: 0x00007392
		private Traverse2([Nullable(2)] object root, MethodInfo method, [Nullable(new byte[] { 2, 1 })] object[] parameter)
		{
			this._root = root;
			this._type = method.ReturnType;
			this._method = method;
			this._params = parameter;
		}

		// Token: 0x06000101 RID: 257 RVA: 0x000091C0 File Offset: 0x000073C0
		[NullableContext(2)]
		public object GetValue()
		{
			FieldInfo fieldInfo = this._info as FieldInfo;
			bool flag = fieldInfo != null;
			object obj;
			if (flag)
			{
				obj = fieldInfo.GetValue(this._root);
			}
			else
			{
				PropertyInfo propertyInfo = this._info as PropertyInfo;
				bool flag2 = propertyInfo != null;
				if (flag2)
				{
					obj = propertyInfo.GetValue(this._root, AccessTools.all, null, this._params, CultureInfo.CurrentCulture);
				}
				else
				{
					MethodBase method = this._method;
					bool flag3 = method != null;
					if (flag3)
					{
						obj = method.Invoke(this._root, this._params);
					}
					else
					{
						bool flag4 = this._root == null && this._type != null;
						if (flag4)
						{
							obj = this._type;
						}
						else
						{
							obj = this._root;
						}
					}
				}
			}
			return obj;
		}

		// Token: 0x06000102 RID: 258 RVA: 0x00009288 File Offset: 0x00007488
		[NullableContext(2)]
		public T GetValue<T>()
		{
			object value = this.GetValue();
			T t;
			bool flag;
			if (value is T)
			{
				t = (T)((object)value);
				flag = true;
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			T t2;
			if (flag2)
			{
				t2 = t;
			}
			else
			{
				t2 = default(T);
			}
			return t2;
		}

		// Token: 0x06000103 RID: 259 RVA: 0x000092C6 File Offset: 0x000074C6
		[return: Nullable(2)]
		public object GetValue(params object[] arguments)
		{
			MethodBase method = this._method;
			return (method != null) ? method.Invoke(this._root, arguments) : null;
		}

		// Token: 0x06000104 RID: 260 RVA: 0x000092E4 File Offset: 0x000074E4
		[NullableContext(2)]
		public T GetValue<T>([Nullable(1)] params object[] arguments)
		{
			MethodBase method = this._method;
			object obj = ((method != null) ? method.Invoke(this._root, arguments) : null);
			T t;
			bool flag;
			if (obj is T)
			{
				t = (T)((object)obj);
				flag = true;
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			T t2;
			if (flag2)
			{
				t2 = t;
			}
			else
			{
				t2 = default(T);
			}
			return t2;
		}

		// Token: 0x06000105 RID: 261 RVA: 0x00009338 File Offset: 0x00007538
		public Traverse2 SetValue(object value)
		{
			FieldInfo fieldInfo = this._info as FieldInfo;
			bool flag = fieldInfo != null && ((this._root == null && fieldInfo.IsStatic) || this._root != null);
			if (flag)
			{
				fieldInfo.SetValue(this._root, value, AccessTools.all, null, CultureInfo.CurrentCulture);
			}
			PropertyInfo propertyInfo = this._info as PropertyInfo;
			bool flag2 = propertyInfo != null && propertyInfo.SetMethod != null && ((this._root == null && propertyInfo.SetMethod.IsStatic) || this._root != null);
			if (flag2)
			{
				propertyInfo.SetValue(this._root, value, AccessTools.all, null, this._params, CultureInfo.CurrentCulture);
			}
			return this;
		}

		// Token: 0x06000106 RID: 262 RVA: 0x00009400 File Offset: 0x00007600
		[NullableContext(2)]
		public Type GetValueType()
		{
			FieldInfo fieldInfo = this._info as FieldInfo;
			bool flag = fieldInfo != null;
			Type type;
			if (flag)
			{
				type = fieldInfo.FieldType;
			}
			else
			{
				PropertyInfo propertyInfo = this._info as PropertyInfo;
				bool flag2 = propertyInfo != null;
				if (flag2)
				{
					type = propertyInfo.PropertyType;
				}
				else
				{
					type = null;
				}
			}
			return type;
		}

		// Token: 0x06000107 RID: 263 RVA: 0x00009450 File Offset: 0x00007650
		private Traverse2 Resolve()
		{
			bool flag = this._root == null;
			if (flag)
			{
				FieldInfo fieldInfo = this._info as FieldInfo;
				bool flag2 = fieldInfo != null && fieldInfo.IsStatic;
				if (flag2)
				{
					return new Traverse2(this.GetValue());
				}
				PropertyInfo propertyInfo = this._info as PropertyInfo;
				bool flag3 = propertyInfo != null && propertyInfo.GetGetMethod().IsStatic;
				if (flag3)
				{
					return new Traverse2(this.GetValue());
				}
				MethodBase method = this._method;
				bool flag4 = method != null && method.IsStatic;
				if (flag4)
				{
					return new Traverse2(this.GetValue());
				}
				bool flag5 = this._type != null;
				if (flag5)
				{
					return this;
				}
			}
			return new Traverse2(this.GetValue());
		}

		// Token: 0x06000108 RID: 264 RVA: 0x00009524 File Offset: 0x00007724
		public Traverse2 Type(string name)
		{
			bool flag = string.IsNullOrEmpty(name);
			Traverse2 traverse;
			if (flag)
			{
				traverse = new Traverse2();
			}
			else
			{
				bool flag2 = this._type == null;
				if (flag2)
				{
					traverse = new Traverse2();
				}
				else
				{
					Type type = AccessTools.Inner(this._type, name);
					bool flag3 = type == null;
					if (flag3)
					{
						traverse = new Traverse2();
					}
					else
					{
						traverse = new Traverse2(type);
					}
				}
			}
			return traverse;
		}

		// Token: 0x06000109 RID: 265 RVA: 0x00009584 File Offset: 0x00007784
		public Traverse2 Field(string name)
		{
			bool flag = string.IsNullOrEmpty(name);
			Traverse2 traverse;
			if (flag)
			{
				traverse = new Traverse2();
			}
			else
			{
				Traverse2 traverse2 = this.Resolve();
				bool flag2 = traverse2._type == null;
				if (flag2)
				{
					traverse = new Traverse2();
				}
				else
				{
					FieldInfo fieldInfo = ((Traverse2.Cache != null) ? Traverse2.Cache.GetValueOrDefault().GetFieldInfo(traverse2._type, name, AccessCacheHandle.MemberType.Any, false) : null);
					bool flag3 = fieldInfo == null;
					if (flag3)
					{
						traverse = new Traverse2();
					}
					else
					{
						bool flag4 = !fieldInfo.IsStatic && traverse2._root == null;
						if (flag4)
						{
							traverse = new Traverse2();
						}
						else
						{
							traverse = new Traverse2(traverse2._root, fieldInfo, null);
						}
					}
				}
			}
			return traverse;
		}

		// Token: 0x0600010A RID: 266 RVA: 0x00009633 File Offset: 0x00007833
		public Traverse2<T> Field<[Nullable(2)] T>(string name)
		{
			return new Traverse2<T>(this.Field(name));
		}

		// Token: 0x0600010B RID: 267 RVA: 0x00009644 File Offset: 0x00007844
		public List<string> Fields()
		{
			Traverse2 traverse = this.Resolve();
			return AccessTools.GetFieldNames(traverse._type);
		}

		// Token: 0x0600010C RID: 268 RVA: 0x00009668 File Offset: 0x00007868
		public Traverse2 Property(string name, [Nullable(new byte[] { 2, 1 })] object[] index = null)
		{
			bool flag = string.IsNullOrEmpty(name);
			Traverse2 traverse;
			if (flag)
			{
				traverse = new Traverse2();
			}
			else
			{
				Traverse2 traverse2 = this.Resolve();
				bool flag2 = traverse2._type == null;
				if (flag2)
				{
					traverse = new Traverse2();
				}
				else
				{
					PropertyInfo propertyInfo = ((Traverse2.Cache != null) ? Traverse2.Cache.GetValueOrDefault().GetPropertyInfo(traverse2._type, name, AccessCacheHandle.MemberType.Any, false) : null);
					bool flag3 = propertyInfo == null;
					if (flag3)
					{
						traverse = new Traverse2();
					}
					else
					{
						traverse = new Traverse2(traverse2._root, propertyInfo, index);
					}
				}
			}
			return traverse;
		}

		// Token: 0x0600010D RID: 269 RVA: 0x000096F2 File Offset: 0x000078F2
		public Traverse2<T> Property<[Nullable(2)] T>(string name, [Nullable(new byte[] { 2, 1 })] object[] index = null)
		{
			return new Traverse2<T>(this.Property(name, index));
		}

		// Token: 0x0600010E RID: 270 RVA: 0x00009704 File Offset: 0x00007904
		public List<string> Properties()
		{
			Traverse2 traverse = this.Resolve();
			return AccessTools.GetPropertyNames(traverse._type);
		}

		// Token: 0x0600010F RID: 271 RVA: 0x00009728 File Offset: 0x00007928
		public Traverse2 Method(string name, params object[] arguments)
		{
			bool flag = string.IsNullOrEmpty(name);
			Traverse2 traverse;
			if (flag)
			{
				traverse = new Traverse2();
			}
			else
			{
				Traverse2 traverse2 = this.Resolve();
				bool flag2 = traverse2._type == null;
				if (flag2)
				{
					traverse = new Traverse2();
				}
				else
				{
					Type[] types = AccessTools.GetTypes(arguments);
					MethodBase methodBase = ((Traverse2.Cache != null) ? Traverse2.Cache.GetValueOrDefault().GetMethodInfo(traverse2._type, name, types, AccessCacheHandle.MemberType.Any, false) : null);
					MethodInfo methodInfo = methodBase as MethodInfo;
					bool flag3 = methodInfo == null;
					if (flag3)
					{
						traverse = new Traverse2();
					}
					else
					{
						traverse = new Traverse2(traverse2._root, methodInfo, arguments);
					}
				}
			}
			return traverse;
		}

		// Token: 0x06000110 RID: 272 RVA: 0x000097CC File Offset: 0x000079CC
		public Traverse2 Method(string name, Type[] paramTypes, [Nullable(new byte[] { 2, 1 })] object[] arguments = null)
		{
			bool flag = string.IsNullOrEmpty(name);
			Traverse2 traverse;
			if (flag)
			{
				traverse = new Traverse2();
			}
			else
			{
				Traverse2 traverse2 = this.Resolve();
				bool flag2 = traverse2._type == null;
				if (flag2)
				{
					traverse = new Traverse2();
				}
				else
				{
					MethodBase methodBase = ((Traverse2.Cache != null) ? Traverse2.Cache.GetValueOrDefault().GetMethodInfo(traverse2._type, name, paramTypes, AccessCacheHandle.MemberType.Any, false) : null);
					MethodInfo methodInfo = methodBase as MethodInfo;
					bool flag3 = methodInfo == null;
					if (flag3)
					{
						traverse = new Traverse2();
					}
					else
					{
						traverse = new Traverse2(traverse2._root, methodInfo, arguments);
					}
				}
			}
			return traverse;
		}

		// Token: 0x06000111 RID: 273 RVA: 0x00009868 File Offset: 0x00007A68
		public List<string> Methods()
		{
			Traverse2 traverse = this.Resolve();
			return AccessTools.GetMethodNames(traverse._type);
		}

		// Token: 0x06000112 RID: 274 RVA: 0x0000988C File Offset: 0x00007A8C
		public bool FieldExists()
		{
			return this._info is FieldInfo;
		}

		// Token: 0x06000113 RID: 275 RVA: 0x0000989C File Offset: 0x00007A9C
		public bool PropertyExists()
		{
			return this._info is PropertyInfo;
		}

		// Token: 0x06000114 RID: 276 RVA: 0x000098AC File Offset: 0x00007AAC
		public bool MethodExists()
		{
			return this._method != null;
		}

		// Token: 0x06000115 RID: 277 RVA: 0x000098BA File Offset: 0x00007ABA
		public bool TypeExists()
		{
			return this._type != null;
		}

		// Token: 0x06000116 RID: 278 RVA: 0x000098C8 File Offset: 0x00007AC8
		public static void IterateFields(object source, Action<Traverse2> action)
		{
			bool flag = action == null;
			if (!flag)
			{
				Traverse2 sourceTrv = Traverse2.Create(source);
				AccessTools.GetFieldNames(source).ForEach(delegate(string f)
				{
					action(sourceTrv.Field(f));
				});
			}
		}

		// Token: 0x06000117 RID: 279 RVA: 0x00009918 File Offset: 0x00007B18
		public static void IterateFields(object source, object target, Action<Traverse2, Traverse2> action)
		{
			bool flag = action == null;
			if (!flag)
			{
				Traverse2 sourceTrv = Traverse2.Create(source);
				Traverse2 targetTrv = Traverse2.Create(target);
				AccessTools.GetFieldNames(source).ForEach(delegate(string f)
				{
					action(sourceTrv.Field(f), targetTrv.Field(f));
				});
			}
		}

		// Token: 0x06000118 RID: 280 RVA: 0x00009974 File Offset: 0x00007B74
		public static void IterateFields(object source, object target, Action<string, Traverse2, Traverse2> action)
		{
			bool flag = action == null;
			if (!flag)
			{
				Traverse2 sourceTrv = Traverse2.Create(source);
				Traverse2 targetTrv = Traverse2.Create(target);
				AccessTools.GetFieldNames(source).ForEach(delegate(string f)
				{
					action(f, sourceTrv.Field(f), targetTrv.Field(f));
				});
			}
		}

		// Token: 0x06000119 RID: 281 RVA: 0x000099D0 File Offset: 0x00007BD0
		public static void IterateProperties(object source, Action<Traverse2> action)
		{
			bool flag = action == null;
			if (!flag)
			{
				Traverse2 sourceTrv = Traverse2.Create(source);
				AccessTools.GetPropertyNames(source).ForEach(delegate(string f)
				{
					action(sourceTrv.Property(f, null));
				});
			}
		}

		// Token: 0x0600011A RID: 282 RVA: 0x00009A20 File Offset: 0x00007C20
		public static void IterateProperties(object source, object target, Action<Traverse2, Traverse2> action)
		{
			bool flag = action == null;
			if (!flag)
			{
				Traverse2 sourceTrv = Traverse2.Create(source);
				Traverse2 targetTrv = Traverse2.Create(target);
				AccessTools.GetPropertyNames(source).ForEach(delegate(string f)
				{
					action(sourceTrv.Property(f, null), targetTrv.Property(f, null));
				});
			}
		}

		// Token: 0x0600011B RID: 283 RVA: 0x00009A7C File Offset: 0x00007C7C
		public static void IterateProperties(object source, object target, Action<string, Traverse2, Traverse2> action)
		{
			bool flag = action == null;
			if (!flag)
			{
				Traverse2 sourceTrv = Traverse2.Create(source);
				Traverse2 targetTrv = Traverse2.Create(target);
				AccessTools.GetPropertyNames(source).ForEach(delegate(string f)
				{
					action(f, sourceTrv.Property(f, null), targetTrv.Property(f, null));
				});
			}
		}

		// Token: 0x0600011C RID: 284 RVA: 0x00009AD6 File Offset: 0x00007CD6
		[NullableContext(2)]
		public override string ToString()
		{
			MethodBase methodBase = this._method ?? this.GetValue();
			return (methodBase != null) ? methodBase.ToString() : null;
		}

		// Token: 0x04000057 RID: 87
		private static readonly AccessCacheHandle? Cache;

		// Token: 0x04000058 RID: 88
		[Nullable(2)]
		private readonly Type _type;

		// Token: 0x04000059 RID: 89
		[Nullable(2)]
		private readonly object _root;

		// Token: 0x0400005A RID: 90
		[Nullable(2)]
		private readonly MemberInfo _info;

		// Token: 0x0400005B RID: 91
		[Nullable(2)]
		private readonly MethodBase _method;

		// Token: 0x0400005C RID: 92
		[Nullable(new byte[] { 2, 1 })]
		private readonly object[] _params;

		// Token: 0x0400005D RID: 93
		public static Action<Traverse2, Traverse2> CopyFields = delegate(Traverse2 from, Traverse2 to)
		{
			bool flag = from == null || to == null;
			if (!flag)
			{
				to.SetValue(from.GetValue());
			}
		};
	}
}
