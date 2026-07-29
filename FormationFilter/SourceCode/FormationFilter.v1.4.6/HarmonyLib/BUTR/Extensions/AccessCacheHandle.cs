using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace HarmonyLib.BUTR.Extensions
{
	// Token: 0x02000025 RID: 37
	[ExcludeFromCodeCoverage]
	internal readonly struct AccessCacheHandle
	{
		// Token: 0x0600014A RID: 330 RVA: 0x000096BC File Offset: 0x000078BC
		public static AccessCacheHandle? Create()
		{
			AccessCacheHandle.AccessCacheCtorDelegate accessCacheCtorMethod = AccessCacheHandle.AccessCacheCtorMethod;
			object obj = ((accessCacheCtorMethod != null) ? accessCacheCtorMethod() : null);
			if (obj == null)
			{
				return null;
			}
			return new AccessCacheHandle?(new AccessCacheHandle(obj));
		}

		// Token: 0x0600014B RID: 331 RVA: 0x000096F3 File Offset: 0x000078F3
		[NullableContext(1)]
		private AccessCacheHandle(object accessCache)
		{
			this._accessCache = accessCache;
		}

		// Token: 0x0600014C RID: 332 RVA: 0x000096FC File Offset: 0x000078FC
		[NullableContext(1)]
		[return: Nullable(2)]
		public FieldInfo GetFieldInfo(Type type, string name, AccessCacheHandle.MemberType memberType = AccessCacheHandle.MemberType.Any, bool declaredOnly = false)
		{
			AccessCacheHandle.GetFieldInfoDelegate getFieldInfoMethod = AccessCacheHandle.GetFieldInfoMethod;
			if (getFieldInfoMethod == null)
			{
				return null;
			}
			return getFieldInfoMethod(this._accessCache, type, name, memberType, declaredOnly);
		}

		// Token: 0x0600014D RID: 333 RVA: 0x00009719 File Offset: 0x00007919
		[NullableContext(1)]
		[return: Nullable(2)]
		public PropertyInfo GetPropertyInfo(Type type, string name, AccessCacheHandle.MemberType memberType = AccessCacheHandle.MemberType.Any, bool declaredOnly = false)
		{
			AccessCacheHandle.GetPropertyInfoDelegate getPropertyInfoMethod = AccessCacheHandle.GetPropertyInfoMethod;
			if (getPropertyInfoMethod == null)
			{
				return null;
			}
			return getPropertyInfoMethod(this._accessCache, type, name, memberType, declaredOnly);
		}

		// Token: 0x0600014E RID: 334 RVA: 0x00009736 File Offset: 0x00007936
		[NullableContext(1)]
		[return: Nullable(2)]
		public MethodBase GetMethodInfo(Type type, string name, Type[] arguments, AccessCacheHandle.MemberType memberType = AccessCacheHandle.MemberType.Any, bool declaredOnly = false)
		{
			AccessCacheHandle.GetMethodInfoDelegate getMethodInfoMethod = AccessCacheHandle.GetMethodInfoMethod;
			if (getMethodInfoMethod == null)
			{
				return null;
			}
			return getMethodInfoMethod(this._accessCache, type, name, arguments, memberType, declaredOnly);
		}

		// Token: 0x0400009A RID: 154
		[Nullable(1)]
		private static readonly Type Blank = typeof(Harmony);

		// Token: 0x0400009B RID: 155
		[Nullable(2)]
		private static readonly AccessCacheHandle.AccessCacheCtorDelegate AccessCacheCtorMethod = AccessTools2.GetDeclaredConstructorDelegate<AccessCacheHandle.AccessCacheCtorDelegate>("HarmonyLib.AccessCache", null, true);

		// Token: 0x0400009C RID: 156
		[Nullable(2)]
		private static readonly AccessCacheHandle.GetFieldInfoDelegate GetFieldInfoMethod = AccessTools2.GetDelegateObjectInstance<AccessCacheHandle.GetFieldInfoDelegate>("HarmonyLib.AccessCache:GetFieldInfo", null, null, true);

		// Token: 0x0400009D RID: 157
		[Nullable(2)]
		private static readonly AccessCacheHandle.GetPropertyInfoDelegate GetPropertyInfoMethod = AccessTools2.GetDelegateObjectInstance<AccessCacheHandle.GetPropertyInfoDelegate>("HarmonyLib.AccessCache:GetPropertyInfo", null, null, true);

		// Token: 0x0400009E RID: 158
		[Nullable(2)]
		private static readonly AccessCacheHandle.GetMethodInfoDelegate GetMethodInfoMethod = AccessTools2.GetDelegateObjectInstance<AccessCacheHandle.GetMethodInfoDelegate>("HarmonyLib.AccessCache:GetMethodInfo", null, null, true);

		// Token: 0x0400009F RID: 159
		[Nullable(1)]
		private readonly object _accessCache;

		// Token: 0x0200004B RID: 75
		internal enum MemberType
		{
			// Token: 0x04000105 RID: 261
			Any,
			// Token: 0x04000106 RID: 262
			Static,
			// Token: 0x04000107 RID: 263
			Instance
		}

		// Token: 0x0200004C RID: 76
		// (Invoke) Token: 0x06000285 RID: 645
		private delegate object AccessCacheCtorDelegate();

		// Token: 0x0200004D RID: 77
		// (Invoke) Token: 0x06000289 RID: 649
		private delegate FieldInfo GetFieldInfoDelegate(object instance, Type type, string name, AccessCacheHandle.MemberType memberType = AccessCacheHandle.MemberType.Any, bool declaredOnly = false);

		// Token: 0x0200004E RID: 78
		// (Invoke) Token: 0x0600028D RID: 653
		private delegate PropertyInfo GetPropertyInfoDelegate(object instance, Type type, string name, AccessCacheHandle.MemberType memberType = AccessCacheHandle.MemberType.Any, bool declaredOnly = false);

		// Token: 0x0200004F RID: 79
		// (Invoke) Token: 0x06000291 RID: 657
		private delegate MethodBase GetMethodInfoDelegate(object instance, Type type, string name, Type[] arguments, AccessCacheHandle.MemberType memberType = AccessCacheHandle.MemberType.Any, bool declaredOnly = false);
	}
}
