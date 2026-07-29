using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MonoMod.Utils
{
	// Token: 0x0200088D RID: 2189
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class DynamicData : DynamicObject, IDisposable, IEnumerable<KeyValuePair<string, object>>, IEnumerable
	{
		// Token: 0x14000007 RID: 7
		// (add) Token: 0x06002CDD RID: 11485 RVA: 0x00096390 File Offset: 0x00094590
		// (remove) Token: 0x06002CDE RID: 11486 RVA: 0x000963C4 File Offset: 0x000945C4
		[Nullable(new byte[] { 2, 1, 1, 2 })]
		[field: Nullable(new byte[] { 2, 1, 1, 2 })]
		public static event Action<DynamicData, Type, object> OnInitialize;

		// Token: 0x17000830 RID: 2096
		// (get) Token: 0x06002CDF RID: 11487 RVA: 0x000963F7 File Offset: 0x000945F7
		[Nullable(new byte[] { 1, 1, 1, 2, 2 })]
		public Dictionary<string, Func<object, object>> Getters
		{
			[return: Nullable(new byte[] { 1, 1, 1, 2, 2 })]
			get
			{
				return this._Data.Getters;
			}
		}

		// Token: 0x17000831 RID: 2097
		// (get) Token: 0x06002CE0 RID: 11488 RVA: 0x00096404 File Offset: 0x00094604
		[Nullable(new byte[] { 1, 1, 1, 2, 2 })]
		public Dictionary<string, Action<object, object>> Setters
		{
			[return: Nullable(new byte[] { 1, 1, 1, 2, 2 })]
			get
			{
				return this._Data.Setters;
			}
		}

		// Token: 0x17000832 RID: 2098
		// (get) Token: 0x06002CE1 RID: 11489 RVA: 0x00096411 File Offset: 0x00094611
		[Nullable(new byte[] { 1, 1, 1, 2, 2, 2, 2 })]
		public Dictionary<string, Func<object, object[], object>> Methods
		{
			[return: Nullable(new byte[] { 1, 1, 1, 2, 2, 2, 2 })]
			get
			{
				return this._Data.Methods;
			}
		}

		// Token: 0x17000833 RID: 2099
		// (get) Token: 0x06002CE2 RID: 11490 RVA: 0x0009641E File Offset: 0x0009461E
		[Nullable(new byte[] { 1, 1, 2 })]
		public Dictionary<string, object> Data
		{
			[return: Nullable(new byte[] { 1, 1, 2 })]
			get
			{
				return this._Data.Data;
			}
		}

		// Token: 0x17000834 RID: 2100
		// (get) Token: 0x06002CE3 RID: 11491 RVA: 0x0009642B File Offset: 0x0009462B
		public bool IsAlive
		{
			get
			{
				return this.Weak == null || this.Weak.SafeGetIsAlive();
			}
		}

		// Token: 0x17000835 RID: 2101
		// (get) Token: 0x06002CE4 RID: 11492 RVA: 0x00096442 File Offset: 0x00094642
		[Nullable(2)]
		public object Target
		{
			[NullableContext(2)]
			get
			{
				WeakReference weak = this.Weak;
				if (weak == null)
				{
					return null;
				}
				return weak.SafeGetTarget();
			}
		}

		// Token: 0x17000836 RID: 2102
		// (get) Token: 0x06002CE5 RID: 11493 RVA: 0x00096455 File Offset: 0x00094655
		// (set) Token: 0x06002CE6 RID: 11494 RVA: 0x0009645D File Offset: 0x0009465D
		public Type TargetType { get; private set; }

		// Token: 0x06002CE7 RID: 11495 RVA: 0x00096466 File Offset: 0x00094666
		public DynamicData(Type type)
			: this(type, null, false)
		{
		}

		// Token: 0x06002CE8 RID: 11496 RVA: 0x00096471 File Offset: 0x00094671
		public DynamicData(object obj)
			: this(Helpers.ThrowIfNull<object>(obj, "obj").GetType(), obj, true)
		{
		}

		// Token: 0x06002CE9 RID: 11497 RVA: 0x0009648B File Offset: 0x0009468B
		public DynamicData(Type type, [Nullable(2)] object obj)
			: this(type, obj, true)
		{
		}

		// Token: 0x06002CEA RID: 11498 RVA: 0x00096498 File Offset: 0x00094698
		public DynamicData(Type type, [Nullable(2)] object obj, bool keepAlive)
		{
			this.TargetType = type;
			Dictionary<Type, DynamicData._Cache_> cacheMap = DynamicData._CacheMap;
			lock (cacheMap)
			{
				DynamicData._Cache_ cache_;
				if (!DynamicData._CacheMap.TryGetValue(type, out cache_))
				{
					cache_ = new DynamicData._Cache_(type);
					DynamicData._CacheMap.Add(type, cache_);
				}
				this._Cache = cache_;
			}
			if (obj != null)
			{
				ConditionalWeakTable<object, DynamicData._Data_> dataMap = DynamicData._DataMap;
				lock (dataMap)
				{
					DynamicData._Data_ data_;
					if (!DynamicData._DataMap.TryGetValue(obj, out data_))
					{
						data_ = new DynamicData._Data_(type);
						DynamicData._DataMap.Add(obj, data_);
					}
					this._Data = data_;
				}
				this.Weak = new WeakReference(obj);
				if (keepAlive)
				{
					this.KeepAlive = obj;
				}
			}
			else
			{
				Dictionary<Type, DynamicData._Data_> dataStaticMap = DynamicData._DataStaticMap;
				lock (dataStaticMap)
				{
					DynamicData._Data_ data_2;
					if (!DynamicData._DataStaticMap.TryGetValue(type, out data_2))
					{
						data_2 = new DynamicData._Data_(type);
						DynamicData._DataStaticMap.Add(type, data_2);
					}
					this._Data = data_2;
				}
			}
			Action<DynamicData, Type, object> onInitialize = DynamicData.OnInitialize;
			if (onInitialize == null)
			{
				return;
			}
			onInitialize(this, type, obj);
		}

		// Token: 0x06002CEB RID: 11499 RVA: 0x000965E0 File Offset: 0x000947E0
		public static DynamicData For(object obj)
		{
			ConditionalWeakTable<object, DynamicData> dynamicDataMap = DynamicData._DynamicDataMap;
			DynamicData dynamicData2;
			lock (dynamicDataMap)
			{
				DynamicData dynamicData;
				if (!DynamicData._DynamicDataMap.TryGetValue(obj, out dynamicData))
				{
					dynamicData = new DynamicData(obj);
					DynamicData._DynamicDataMap.Add(obj, dynamicData);
				}
				dynamicData2 = dynamicData;
			}
			return dynamicData2;
		}

		// Token: 0x06002CEC RID: 11500 RVA: 0x00096640 File Offset: 0x00094840
		[return: Nullable(new byte[] { 1, 1, 2 })]
		public static Func<object, T> New<T>(params object[] args)
		{
			T target = (T)((object)Activator.CreateInstance(typeof(T), BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, args, null));
			return (object other) => DynamicData.Set<T>(target, other);
		}

		// Token: 0x06002CED RID: 11501 RVA: 0x00096671 File Offset: 0x00094871
		[return: Nullable(new byte[] { 1, 1, 2 })]
		public static Func<object, object> New(Type type, params object[] args)
		{
			object target = Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, args, null);
			return (object other) => DynamicData.Set(target, other);
		}

		// Token: 0x06002CEE RID: 11502 RVA: 0x00096694 File Offset: 0x00094894
		[return: Dynamic(new bool[] { false, false, true })]
		public static Func<object, dynamic> NewWrap<[Nullable(2)] T>(params object[] args)
		{
			T target = (T)((object)Activator.CreateInstance(typeof(T), BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, args, null));
			return (object other) => DynamicData.Wrap(target, other);
		}

		// Token: 0x06002CEF RID: 11503 RVA: 0x000966C5 File Offset: 0x000948C5
		[return: Dynamic(new bool[] { false, false, true })]
		public static Func<object, dynamic> NewWrap(Type type, params object[] args)
		{
			object target = Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, args, null);
			return (object other) => DynamicData.Wrap(target, other);
		}

		// Token: 0x06002CF0 RID: 11504 RVA: 0x000966E8 File Offset: 0x000948E8
		[return: Dynamic]
		public static dynamic Wrap(object target, [Nullable(2)] object other = null)
		{
			DynamicData dynamicData = new DynamicData(target);
			dynamicData.CopyFrom(other);
			return dynamicData;
		}

		// Token: 0x06002CF1 RID: 11505 RVA: 0x000966F7 File Offset: 0x000948F7
		[return: Nullable(2)]
		public static T Set<T>(T target, [Nullable(2)] object other = null)
		{
			return (T)((object)DynamicData.Set(target, other));
		}

		// Token: 0x06002CF2 RID: 11506 RVA: 0x0009670C File Offset: 0x0009490C
		[NullableContext(2)]
		public static object Set([Nullable(1)] object target, object other = null)
		{
			object target2;
			using (DynamicData dynamicData = new DynamicData(target))
			{
				dynamicData.CopyFrom(other);
				target2 = dynamicData.Target;
			}
			return target2;
		}

		// Token: 0x06002CF3 RID: 11507 RVA: 0x0009674C File Offset: 0x0009494C
		public void RegisterProperty(string name, [Nullable(new byte[] { 1, 2, 2 })] Func<object, object> getter, [Nullable(new byte[] { 1, 2, 2 })] Action<object, object> setter)
		{
			this.Getters[name] = getter;
			this.Setters[name] = setter;
		}

		// Token: 0x06002CF4 RID: 11508 RVA: 0x00096768 File Offset: 0x00094968
		public void UnregisterProperty(string name)
		{
			this.Getters.Remove(name);
			this.Setters.Remove(name);
		}

		// Token: 0x06002CF5 RID: 11509 RVA: 0x00096784 File Offset: 0x00094984
		public void RegisterMethod(string name, [Nullable(new byte[] { 1, 2, 2, 2, 2 })] Func<object, object[], object> cb)
		{
			this.Methods[name] = cb;
		}

		// Token: 0x06002CF6 RID: 11510 RVA: 0x00096793 File Offset: 0x00094993
		public void UnregisterMethod(string name)
		{
			this.Methods.Remove(name);
		}

		// Token: 0x06002CF7 RID: 11511 RVA: 0x000967A4 File Offset: 0x000949A4
		[NullableContext(2)]
		public void CopyFrom(object other)
		{
			if (other == null)
			{
				return;
			}
			foreach (PropertyInfo propertyInfo in other.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
			{
				this.Set(propertyInfo.Name, propertyInfo.GetValue(other, null));
			}
		}

		// Token: 0x06002CF8 RID: 11512 RVA: 0x000967EC File Offset: 0x000949EC
		[return: Nullable(2)]
		public object Get(string name)
		{
			object obj;
			this.TryGet(name, out obj);
			return obj;
		}

		// Token: 0x06002CF9 RID: 11513 RVA: 0x00096804 File Offset: 0x00094A04
		public bool TryGet(string name, [Nullable(2)] out object value)
		{
			object target = this.Target;
			Func<object, object> func;
			if (this._Data.Getters.TryGetValue(name, out func))
			{
				value = func(target);
				return true;
			}
			if (this._Cache.Getters.TryGetValue(name, out func))
			{
				value = func(target);
				return true;
			}
			return this._Data.Data.TryGetValue(name, out value);
		}

		// Token: 0x06002CFA RID: 11514 RVA: 0x0009686F File Offset: 0x00094A6F
		[NullableContext(2)]
		public T Get<T>([Nullable(1)] string name)
		{
			return (T)((object)this.Get(name));
		}

		// Token: 0x06002CFB RID: 11515 RVA: 0x00096880 File Offset: 0x00094A80
		public bool TryGet<[Nullable(2)] T>(string name, [MaybeNullWhen(false)] out T value)
		{
			object obj;
			bool flag = this.TryGet(name, out obj);
			value = (T)((object)obj);
			return flag;
		}

		// Token: 0x06002CFC RID: 11516 RVA: 0x000968A4 File Offset: 0x00094AA4
		public void Set(string name, [Nullable(2)] object value)
		{
			object target = this.Target;
			Action<object, object> action;
			if (this._Data.Setters.TryGetValue(name, out action))
			{
				action(target, value);
				return;
			}
			if (this._Cache.Setters.TryGetValue(name, out action))
			{
				action(target, value);
				return;
			}
			this.Data[name] = value;
		}

		// Token: 0x06002CFD RID: 11517 RVA: 0x00096901 File Offset: 0x00094B01
		public void Add([Nullable(new byte[] { 0, 1, 1 })] KeyValuePair<string, object> kvp)
		{
			this.Set(kvp.Key, kvp.Value);
		}

		// Token: 0x06002CFE RID: 11518 RVA: 0x00096917 File Offset: 0x00094B17
		public void Add(string key, object value)
		{
			this.Set(key, value);
		}

		// Token: 0x06002CFF RID: 11519 RVA: 0x00096924 File Offset: 0x00094B24
		[return: Nullable(2)]
		public object Invoke(string name, params object[] args)
		{
			object obj;
			this.TryInvoke(name, args, out obj);
			return obj;
		}

		// Token: 0x06002D00 RID: 11520 RVA: 0x00096940 File Offset: 0x00094B40
		[NullableContext(2)]
		public bool TryInvoke([Nullable(1)] string name, object[] args, out object result)
		{
			Func<object, object[], object> func;
			if (this._Data.Methods.TryGetValue(name, out func))
			{
				result = func(this.Target, args);
				return true;
			}
			if (this._Cache.Methods.TryGetValue(name, out func))
			{
				result = func(this.Target, args);
				return true;
			}
			result = null;
			return false;
		}

		// Token: 0x06002D01 RID: 11521 RVA: 0x0009699D File Offset: 0x00094B9D
		[return: Nullable(2)]
		public T Invoke<[Nullable(2)] T>(string name, params object[] args)
		{
			return (T)((object)this.Invoke(name, args));
		}

		// Token: 0x06002D02 RID: 11522 RVA: 0x000969AC File Offset: 0x00094BAC
		public bool TryInvoke<[Nullable(2)] T>(string name, object[] args, [MaybeNullWhen(false)] out T result)
		{
			object obj;
			bool flag = this.TryInvoke(name, args, out obj);
			result = (T)((object)obj);
			return flag;
		}

		// Token: 0x06002D03 RID: 11523 RVA: 0x000969CF File Offset: 0x00094BCF
		private void Dispose(bool disposing)
		{
			this.KeepAlive = null;
		}

		// Token: 0x06002D04 RID: 11524 RVA: 0x000969D8 File Offset: 0x00094BD8
		~DynamicData()
		{
			this.Dispose(false);
		}

		// Token: 0x06002D05 RID: 11525 RVA: 0x00096A08 File Offset: 0x00094C08
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x06002D06 RID: 11526 RVA: 0x00096A18 File Offset: 0x00094C18
		public override IEnumerable<string> GetDynamicMemberNames()
		{
			return this._Data.Data.Keys.Union<string>(this._Data.Getters.Keys).Union<string>(this._Data.Setters.Keys).Union<string>(this._Data.Methods.Keys)
				.Union<string>(this._Cache.Getters.Keys)
				.Union<string>(this._Cache.Setters.Keys)
				.Union<string>(this._Cache.Methods.Keys);
		}

		// Token: 0x06002D07 RID: 11527 RVA: 0x00096AB4 File Offset: 0x00094CB4
		public override bool TryConvert(ConvertBinder binder, [Nullable(2)] out object result)
		{
			Helpers.ThrowIfArgumentNull<ConvertBinder>(binder, "binder");
			if (this.TargetType.IsCompatible(binder.Type) || this.TargetType.IsCompatible(binder.ReturnType) || binder.Type == typeof(object) || binder.ReturnType == typeof(object))
			{
				result = this.Target;
				return true;
			}
			if (typeof(DynamicData).IsCompatible(binder.Type) || typeof(DynamicData).IsCompatible(binder.ReturnType))
			{
				result = this;
				return true;
			}
			result = null;
			return false;
		}

		// Token: 0x06002D08 RID: 11528 RVA: 0x00096B61 File Offset: 0x00094D61
		public override bool TryGetMember(GetMemberBinder binder, [Nullable(2)] out object result)
		{
			Helpers.ThrowIfArgumentNull<GetMemberBinder>(binder, "binder");
			if (this.Methods.ContainsKey(binder.Name))
			{
				result = null;
				return false;
			}
			result = this.Get(binder.Name);
			return true;
		}

		// Token: 0x06002D09 RID: 11529 RVA: 0x00096B95 File Offset: 0x00094D95
		public override bool TrySetMember(SetMemberBinder binder, [Nullable(2)] object value)
		{
			Helpers.ThrowIfArgumentNull<SetMemberBinder>(binder, "binder");
			this.Set(binder.Name, value);
			return true;
		}

		// Token: 0x06002D0A RID: 11530 RVA: 0x00096BB0 File Offset: 0x00094DB0
		[NullableContext(2)]
		public override bool TryInvokeMember([Nullable(1)] InvokeMemberBinder binder, object[] args, out object result)
		{
			Helpers.ThrowIfArgumentNull<InvokeMemberBinder>(binder, "binder");
			return this.TryInvoke(binder.Name, args, out result);
		}

		// Token: 0x06002D0B RID: 11531 RVA: 0x00096BCB File Offset: 0x00094DCB
		[return: Nullable(new byte[] { 1, 0, 1, 2 })]
		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			DynamicData.<GetEnumerator>d__66 <GetEnumerator>d__ = new DynamicData.<GetEnumerator>d__66(0);
			<GetEnumerator>d__.<>4__this = this;
			return <GetEnumerator>d__;
		}

		// Token: 0x06002D0C RID: 11532 RVA: 0x00096BDA File Offset: 0x00094DDA
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x04003A8B RID: 14987
		[Nullable(new byte[] { 1, 2 })]
		private static readonly object[] _NoArgs = ArrayEx.Empty<object>();

		// Token: 0x04003A8D RID: 14989
		private static readonly Dictionary<Type, DynamicData._Cache_> _CacheMap = new Dictionary<Type, DynamicData._Cache_>();

		// Token: 0x04003A8E RID: 14990
		private static readonly Dictionary<Type, DynamicData._Data_> _DataStaticMap = new Dictionary<Type, DynamicData._Data_>();

		// Token: 0x04003A8F RID: 14991
		private static readonly ConditionalWeakTable<object, DynamicData._Data_> _DataMap = new ConditionalWeakTable<object, DynamicData._Data_>();

		// Token: 0x04003A90 RID: 14992
		private static readonly ConditionalWeakTable<object, DynamicData> _DynamicDataMap = new ConditionalWeakTable<object, DynamicData>();

		// Token: 0x04003A91 RID: 14993
		[Nullable(2)]
		private readonly WeakReference Weak;

		// Token: 0x04003A92 RID: 14994
		[Nullable(2)]
		private object KeepAlive;

		// Token: 0x04003A93 RID: 14995
		private readonly DynamicData._Cache_ _Cache;

		// Token: 0x04003A94 RID: 14996
		private readonly DynamicData._Data_ _Data;

		// Token: 0x0200088E RID: 2190
		[NullableContext(0)]
		private class _Cache_
		{
			// Token: 0x06002D0E RID: 11534 RVA: 0x00096C18 File Offset: 0x00094E18
			[NullableContext(2)]
			public _Cache_(Type targetType)
			{
				bool flag = true;
				while (targetType != null && targetType != targetType.BaseType)
				{
					foreach (FieldInfo fieldInfo in targetType.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
					{
						string name = fieldInfo.Name;
						if (!this.Getters.ContainsKey(name) && !this.Setters.ContainsKey(name))
						{
							try
							{
								FastReflectionHelper.FastInvoker fastInvoker2 = fieldInfo.GetFastInvoker();
								this.Getters[name] = (object obj) => fastInvoker2(obj, new object[0]);
								this.Setters[name] = delegate(object obj, object value)
								{
									fastInvoker2(obj, new object[] { value });
								};
							}
							catch
							{
								this.Getters[name] = new Func<object, object>(fieldInfo.GetValue);
								this.Setters[name] = new Action<object, object>(fieldInfo.SetValue);
							}
						}
					}
					PropertyInfo[] properties = targetType.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
					for (int i = 0; i < properties.Length; i++)
					{
						PropertyInfo propertyInfo = properties[i];
						string name2 = propertyInfo.Name;
						MethodInfo get = propertyInfo.GetGetMethod(true);
						if (get != null && !this.Getters.ContainsKey(name2))
						{
							try
							{
								FastReflectionHelper.FastInvoker fastInvoker3 = get.GetFastInvoker();
								this.Getters[name2] = (object obj) => fastInvoker3(obj, new object[0]);
							}
							catch
							{
								this.Getters[name2] = (object obj) => get.Invoke(obj, DynamicData._NoArgs);
							}
						}
						MethodInfo set = propertyInfo.GetSetMethod(true);
						if (set != null && !this.Setters.ContainsKey(name2))
						{
							try
							{
								FastReflectionHelper.FastInvoker fastInvoker = set.GetFastInvoker();
								this.Setters[name2] = delegate(object obj, object value)
								{
									fastInvoker(obj, new object[] { value });
								};
							}
							catch
							{
								this.Setters[name2] = delegate(object obj, object value)
								{
									set.Invoke(obj, new object[] { value });
								};
							}
						}
					}
					Dictionary<string, MethodInfo> dictionary = new Dictionary<string, MethodInfo>();
					foreach (MethodInfo methodInfo in targetType.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
					{
						string name3 = methodInfo.Name;
						if (flag || !this.Methods.ContainsKey(name3))
						{
							if (dictionary.ContainsKey(name3))
							{
								dictionary[name3] = null;
							}
							else
							{
								dictionary[name3] = methodInfo;
							}
						}
					}
					foreach (KeyValuePair<string, MethodInfo> keyValuePair in dictionary)
					{
						if (!(keyValuePair.Value == null) && !keyValuePair.Value.IsGenericMethod)
						{
							try
							{
								FastReflectionHelper.FastInvoker cb = keyValuePair.Value.GetFastInvoker();
								this.Methods[keyValuePair.Key] = (object target, object[] args) => cb(target, args);
							}
							catch
							{
								this.Methods[keyValuePair.Key] = new Func<object, object[], object>(keyValuePair.Value.Invoke);
							}
						}
					}
					flag = false;
					targetType = targetType.BaseType;
				}
			}

			// Token: 0x04003A96 RID: 14998
			[Nullable(new byte[] { 1, 1, 1, 2, 2 })]
			public readonly Dictionary<string, Func<object, object>> Getters = new Dictionary<string, Func<object, object>>();

			// Token: 0x04003A97 RID: 14999
			[Nullable(new byte[] { 1, 1, 1, 2, 2 })]
			public readonly Dictionary<string, Action<object, object>> Setters = new Dictionary<string, Action<object, object>>();

			// Token: 0x04003A98 RID: 15000
			[Nullable(new byte[] { 1, 1, 1, 2, 2, 2, 2 })]
			public readonly Dictionary<string, Func<object, object[], object>> Methods = new Dictionary<string, Func<object, object[], object>>();
		}

		// Token: 0x02000894 RID: 2196
		[NullableContext(0)]
		private class _Data_
		{
			// Token: 0x06002D1B RID: 11547 RVA: 0x00097069 File Offset: 0x00095269
			[NullableContext(1)]
			public _Data_(Type type)
			{
				type == null;
			}

			// Token: 0x04003A9F RID: 15007
			[Nullable(new byte[] { 1, 1, 1, 2, 2 })]
			public readonly Dictionary<string, Func<object, object>> Getters = new Dictionary<string, Func<object, object>>();

			// Token: 0x04003AA0 RID: 15008
			[Nullable(new byte[] { 1, 1, 1, 2, 2 })]
			public readonly Dictionary<string, Action<object, object>> Setters = new Dictionary<string, Action<object, object>>();

			// Token: 0x04003AA1 RID: 15009
			[Nullable(new byte[] { 1, 1, 1, 2, 2, 2, 2 })]
			public readonly Dictionary<string, Func<object, object[], object>> Methods = new Dictionary<string, Func<object, object[], object>>();

			// Token: 0x04003AA2 RID: 15010
			[Nullable(new byte[] { 1, 1, 2 })]
			public readonly Dictionary<string, object> Data = new Dictionary<string, object>();
		}
	}
}
