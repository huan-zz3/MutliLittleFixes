using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace HarmonyLib
{
	// Token: 0x020001CA RID: 458
	public class Traverse
	{
		// Token: 0x0600080D RID: 2061 RVA: 0x0001ACFC File Offset: 0x00018EFC
		[MethodImpl(MethodImplOptions.Synchronized)]
		static Traverse()
		{
			if (Traverse.Cache == null)
			{
				Traverse.Cache = new AccessCache();
			}
		}

		// Token: 0x0600080E RID: 2062 RVA: 0x0001AD24 File Offset: 0x00018F24
		public static Traverse Create(Type type)
		{
			return new Traverse(type);
		}

		// Token: 0x0600080F RID: 2063 RVA: 0x0001AD2C File Offset: 0x00018F2C
		public static Traverse Create<T>()
		{
			return Traverse.Create(typeof(T));
		}

		// Token: 0x06000810 RID: 2064 RVA: 0x0001AD3D File Offset: 0x00018F3D
		public static Traverse Create(object root)
		{
			return new Traverse(root);
		}

		// Token: 0x06000811 RID: 2065 RVA: 0x0001AD45 File Offset: 0x00018F45
		public static Traverse CreateWithType(string name)
		{
			return new Traverse(AccessTools.TypeByName(name));
		}

		// Token: 0x06000812 RID: 2066 RVA: 0x00002B15 File Offset: 0x00000D15
		private Traverse()
		{
		}

		// Token: 0x06000813 RID: 2067 RVA: 0x0001AD52 File Offset: 0x00018F52
		public Traverse(Type type)
		{
			this._type = type;
		}

		// Token: 0x06000814 RID: 2068 RVA: 0x0001AD61 File Offset: 0x00018F61
		public Traverse(object root)
		{
			this._root = root;
			this._type = ((root != null) ? root.GetType() : null);
		}

		// Token: 0x06000815 RID: 2069 RVA: 0x0001AD82 File Offset: 0x00018F82
		private Traverse(object root, MemberInfo info, object[] index)
		{
			this._root = root;
			this._type = ((root != null) ? root.GetType() : null) ?? info.GetUnderlyingType();
			this._info = info;
			this._params = index;
		}

		// Token: 0x06000816 RID: 2070 RVA: 0x0001ADBB File Offset: 0x00018FBB
		private Traverse(object root, MethodInfo method, object[] parameter)
		{
			this._root = root;
			this._type = method.ReturnType;
			this._method = method;
			this._params = parameter;
		}

		// Token: 0x06000817 RID: 2071 RVA: 0x0001ADE4 File Offset: 0x00018FE4
		public object GetValue()
		{
			if (this._info is FieldInfo)
			{
				return ((FieldInfo)this._info).GetValue(this._root);
			}
			if (this._info is PropertyInfo)
			{
				return ((PropertyInfo)this._info).GetValue(this._root, AccessTools.all, null, this._params, CultureInfo.CurrentCulture);
			}
			if (this._method != null)
			{
				return this._method.Invoke(this._root, this._params);
			}
			if (this._root == null && this._type != null)
			{
				return this._type;
			}
			return this._root;
		}

		// Token: 0x06000818 RID: 2072 RVA: 0x0001AE88 File Offset: 0x00019088
		public T GetValue<T>()
		{
			object value = this.GetValue();
			if (value == null)
			{
				return default(T);
			}
			return (T)((object)value);
		}

		// Token: 0x06000819 RID: 2073 RVA: 0x0001AEAF File Offset: 0x000190AF
		public object GetValue(params object[] arguments)
		{
			if (this._method == null)
			{
				throw new Exception("cannot get method value without method");
			}
			return this._method.Invoke(this._root, arguments);
		}

		// Token: 0x0600081A RID: 2074 RVA: 0x0001AED6 File Offset: 0x000190D6
		public T GetValue<T>(params object[] arguments)
		{
			if (this._method == null)
			{
				throw new Exception("cannot get method value without method");
			}
			return (T)((object)this._method.Invoke(this._root, arguments));
		}

		// Token: 0x0600081B RID: 2075 RVA: 0x0001AF04 File Offset: 0x00019104
		public Traverse SetValue(object value)
		{
			if (this._info is FieldInfo)
			{
				((FieldInfo)this._info).SetValue(this._root, value, AccessTools.all, null, CultureInfo.CurrentCulture);
			}
			if (this._info is PropertyInfo)
			{
				((PropertyInfo)this._info).SetValue(this._root, value, AccessTools.all, null, this._params, CultureInfo.CurrentCulture);
			}
			if (this._method != null)
			{
				throw new Exception("cannot set value of method " + this._method.FullDescription());
			}
			return this;
		}

		// Token: 0x0600081C RID: 2076 RVA: 0x0001AF99 File Offset: 0x00019199
		public Type GetValueType()
		{
			if (this._info is FieldInfo)
			{
				return ((FieldInfo)this._info).FieldType;
			}
			if (this._info is PropertyInfo)
			{
				return ((PropertyInfo)this._info).PropertyType;
			}
			return null;
		}

		// Token: 0x170001FA RID: 506
		// (get) Token: 0x0600081D RID: 2077 RVA: 0x0001AFD8 File Offset: 0x000191D8
		public bool IsField
		{
			get
			{
				return this._info is FieldInfo;
			}
		}

		// Token: 0x170001FB RID: 507
		// (get) Token: 0x0600081E RID: 2078 RVA: 0x0001AFE8 File Offset: 0x000191E8
		public bool IsProperty
		{
			get
			{
				return this._info is PropertyInfo;
			}
		}

		// Token: 0x170001FC RID: 508
		// (get) Token: 0x0600081F RID: 2079 RVA: 0x0001AFF8 File Offset: 0x000191F8
		public bool IsWriteable
		{
			get
			{
				FieldInfo fieldInfo = this._info as FieldInfo;
				if (fieldInfo != null)
				{
					bool flag = fieldInfo.IsLiteral && !fieldInfo.IsInitOnly && fieldInfo.IsStatic;
					bool flag2 = !fieldInfo.IsLiteral && fieldInfo.IsInitOnly && fieldInfo.IsStatic;
					return !flag && !flag2;
				}
				PropertyInfo propertyInfo = this._info as PropertyInfo;
				return propertyInfo != null && propertyInfo.CanWrite;
			}
		}

		// Token: 0x06000820 RID: 2080 RVA: 0x0001B06C File Offset: 0x0001926C
		private Traverse Resolve()
		{
			if (this._root == null)
			{
				FieldInfo fieldInfo = this._info as FieldInfo;
				if (fieldInfo != null && fieldInfo.IsStatic)
				{
					return new Traverse(this.GetValue());
				}
				PropertyInfo propertyInfo = this._info as PropertyInfo;
				if (propertyInfo != null && propertyInfo.GetGetMethod().IsStatic)
				{
					return new Traverse(this.GetValue());
				}
				if (this._method != null && this._method.IsStatic)
				{
					return new Traverse(this.GetValue());
				}
				if (this._type != null)
				{
					return this;
				}
			}
			return new Traverse(this.GetValue());
		}

		// Token: 0x06000821 RID: 2081 RVA: 0x0001B104 File Offset: 0x00019304
		public Traverse Type(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (this._type == null)
			{
				return new Traverse();
			}
			Type type = AccessTools.Inner(this._type, name);
			if (type == null)
			{
				return new Traverse();
			}
			return new Traverse(type);
		}

		// Token: 0x06000822 RID: 2082 RVA: 0x0001B14C File Offset: 0x0001934C
		public Traverse Field(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			Traverse traverse = this.Resolve();
			if (traverse._type == null)
			{
				return new Traverse();
			}
			FieldInfo fieldInfo = Traverse.Cache.GetFieldInfo(traverse._type, name, AccessCache.MemberType.Any, false);
			if (fieldInfo == null)
			{
				return new Traverse();
			}
			if (!fieldInfo.IsStatic && traverse._root == null)
			{
				return new Traverse();
			}
			return new Traverse(traverse._root, fieldInfo, null);
		}

		// Token: 0x06000823 RID: 2083 RVA: 0x0001B1BC File Offset: 0x000193BC
		public Traverse<T> Field<T>(string name)
		{
			return new Traverse<T>(this.Field(name));
		}

		// Token: 0x06000824 RID: 2084 RVA: 0x0001B1CC File Offset: 0x000193CC
		public List<string> Fields()
		{
			Traverse traverse = this.Resolve();
			return AccessTools.GetFieldNames(traverse._type);
		}

		// Token: 0x06000825 RID: 2085 RVA: 0x0001B1EC File Offset: 0x000193EC
		public Traverse Property(string name, object[] index = null)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			Traverse traverse = this.Resolve();
			if (traverse._type == null)
			{
				return new Traverse();
			}
			PropertyInfo propertyInfo = Traverse.Cache.GetPropertyInfo(traverse._type, name, AccessCache.MemberType.Any, false);
			if (propertyInfo == null)
			{
				return new Traverse();
			}
			return new Traverse(traverse._root, propertyInfo, index);
		}

		// Token: 0x06000826 RID: 2086 RVA: 0x0001B246 File Offset: 0x00019446
		public Traverse<T> Property<T>(string name, object[] index = null)
		{
			return new Traverse<T>(this.Property(name, index));
		}

		// Token: 0x06000827 RID: 2087 RVA: 0x0001B258 File Offset: 0x00019458
		public List<string> Properties()
		{
			Traverse traverse = this.Resolve();
			return AccessTools.GetPropertyNames(traverse._type);
		}

		// Token: 0x06000828 RID: 2088 RVA: 0x0001B278 File Offset: 0x00019478
		public Traverse Method(string name, params object[] arguments)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			Traverse traverse = this.Resolve();
			if (traverse._type == null)
			{
				return new Traverse();
			}
			Type[] types = AccessTools.GetTypes(arguments);
			MethodBase methodInfo = Traverse.Cache.GetMethodInfo(traverse._type, name, types, AccessCache.MemberType.Any, false);
			if (methodInfo == null)
			{
				return new Traverse();
			}
			return new Traverse(traverse._root, (MethodInfo)methodInfo, arguments);
		}

		// Token: 0x06000829 RID: 2089 RVA: 0x0001B2E0 File Offset: 0x000194E0
		public Traverse Method(string name, Type[] paramTypes, object[] arguments = null)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			Traverse traverse = this.Resolve();
			if (traverse._type == null)
			{
				return new Traverse();
			}
			MethodBase methodInfo = Traverse.Cache.GetMethodInfo(traverse._type, name, paramTypes, AccessCache.MemberType.Any, false);
			if (methodInfo == null)
			{
				return new Traverse();
			}
			return new Traverse(traverse._root, (MethodInfo)methodInfo, arguments);
		}

		// Token: 0x0600082A RID: 2090 RVA: 0x0001B340 File Offset: 0x00019540
		public List<string> Methods()
		{
			Traverse traverse = this.Resolve();
			return AccessTools.GetMethodNames(traverse._type);
		}

		// Token: 0x0600082B RID: 2091 RVA: 0x0001B35F File Offset: 0x0001955F
		public bool FieldExists()
		{
			return this._info != null && this._info is FieldInfo;
		}

		// Token: 0x0600082C RID: 2092 RVA: 0x0001B379 File Offset: 0x00019579
		public bool PropertyExists()
		{
			return this._info != null && this._info is PropertyInfo;
		}

		// Token: 0x0600082D RID: 2093 RVA: 0x0001B393 File Offset: 0x00019593
		public bool MethodExists()
		{
			return this._method != null;
		}

		// Token: 0x0600082E RID: 2094 RVA: 0x0001B3A1 File Offset: 0x000195A1
		public bool TypeExists()
		{
			return this._type != null;
		}

		// Token: 0x0600082F RID: 2095 RVA: 0x0001B3B0 File Offset: 0x000195B0
		public static void IterateFields(object source, Action<Traverse> action)
		{
			Traverse sourceTrv = Traverse.Create(source);
			AccessTools.GetFieldNames(source).ForEach(delegate(string f)
			{
				action(sourceTrv.Field(f));
			});
		}

		// Token: 0x06000830 RID: 2096 RVA: 0x0001B3F0 File Offset: 0x000195F0
		public static void IterateFields(object source, object target, Action<Traverse, Traverse> action)
		{
			Traverse sourceTrv = Traverse.Create(source);
			Traverse targetTrv = Traverse.Create(target);
			AccessTools.GetFieldNames(source).ForEach(delegate(string f)
			{
				action(sourceTrv.Field(f), targetTrv.Field(f));
			});
		}

		// Token: 0x06000831 RID: 2097 RVA: 0x0001B43C File Offset: 0x0001963C
		public static void IterateFields(object source, object target, Action<string, Traverse, Traverse> action)
		{
			Traverse sourceTrv = Traverse.Create(source);
			Traverse targetTrv = Traverse.Create(target);
			AccessTools.GetFieldNames(source).ForEach(delegate(string f)
			{
				action(f, sourceTrv.Field(f), targetTrv.Field(f));
			});
		}

		// Token: 0x06000832 RID: 2098 RVA: 0x0001B488 File Offset: 0x00019688
		public static void IterateProperties(object source, Action<Traverse> action)
		{
			Traverse sourceTrv = Traverse.Create(source);
			AccessTools.GetPropertyNames(source).ForEach(delegate(string f)
			{
				action(sourceTrv.Property(f, null));
			});
		}

		// Token: 0x06000833 RID: 2099 RVA: 0x0001B4C8 File Offset: 0x000196C8
		public static void IterateProperties(object source, object target, Action<Traverse, Traverse> action)
		{
			Traverse sourceTrv = Traverse.Create(source);
			Traverse targetTrv = Traverse.Create(target);
			AccessTools.GetPropertyNames(source).ForEach(delegate(string f)
			{
				action(sourceTrv.Property(f, null), targetTrv.Property(f, null));
			});
		}

		// Token: 0x06000834 RID: 2100 RVA: 0x0001B514 File Offset: 0x00019714
		public static void IterateProperties(object source, object target, Action<string, Traverse, Traverse> action)
		{
			Traverse sourceTrv = Traverse.Create(source);
			Traverse targetTrv = Traverse.Create(target);
			AccessTools.GetPropertyNames(source).ForEach(delegate(string f)
			{
				action(f, sourceTrv.Property(f, null), targetTrv.Property(f, null));
			});
		}

		// Token: 0x06000835 RID: 2101 RVA: 0x0001B560 File Offset: 0x00019760
		public override string ToString()
		{
			object obj = this._method ?? this.GetValue();
			if (obj == null)
			{
				return null;
			}
			return obj.ToString();
		}

		// Token: 0x040002C0 RID: 704
		private static readonly AccessCache Cache;

		// Token: 0x040002C1 RID: 705
		private readonly Type _type;

		// Token: 0x040002C2 RID: 706
		private readonly object _root;

		// Token: 0x040002C3 RID: 707
		private readonly MemberInfo _info;

		// Token: 0x040002C4 RID: 708
		private readonly MethodBase _method;

		// Token: 0x040002C5 RID: 709
		private readonly object[] _params;

		// Token: 0x040002C6 RID: 710
		public static Action<Traverse, Traverse> CopyFields = delegate(Traverse from, Traverse to)
		{
			if (to.IsWriteable)
			{
				to.SetValue(from.GetValue());
			}
		};
	}
}
