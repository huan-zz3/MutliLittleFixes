using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using MonoMod.Core.Platforms;
using MonoMod.Utils;

namespace HarmonyLib
{
	// Token: 0x020000A7 RID: 167
	public static class AccessTools
	{
		// Token: 0x06000351 RID: 849 RVA: 0x00011CF1 File Offset: 0x0000FEF1
		public static IEnumerable<Assembly> AllAssemblies()
		{
			return from a in AppDomain.CurrentDomain.GetAssemblies()
				where !a.FullName.StartsWith("Microsoft.VisualStudio")
				select a;
		}

		// Token: 0x06000352 RID: 850 RVA: 0x00011D24 File Offset: 0x0000FF24
		public static Type TypeByName(string name)
		{
			Type type = Type.GetType(name, false);
			if (type != null)
			{
				return type;
			}
			foreach (Assembly assembly in AccessTools.AllAssemblies())
			{
				Type type2 = assembly.GetType(name, false);
				if (type2 != null)
				{
					return type2;
				}
			}
			Type[] array = AccessTools.AllTypes().ToArray<Type>();
			Type type3 = array.FirstOrDefault<Type>((Type t) => t.FullName == name);
			if (type3 != null)
			{
				return type3;
			}
			Type type4 = array.FirstOrDefault<Type>((Type t) => t.Name == name);
			if (type4 != null)
			{
				return type4;
			}
			FileLog.Debug("AccessTools.TypeByName: Could not find type named " + name);
			return null;
		}

		// Token: 0x06000353 RID: 851 RVA: 0x00011E00 File Offset: 0x00010000
		public static Type TypeSearch(Regex search, bool invalidateCache = false)
		{
			if (AccessTools.allTypesCached == null || invalidateCache)
			{
				AccessTools.allTypesCached = AccessTools.AllTypes().ToArray<Type>();
			}
			Type type = AccessTools.allTypesCached.FirstOrDefault<Type>((Type t) => search.IsMatch(t.FullName));
			if (type != null)
			{
				return type;
			}
			Type type2 = AccessTools.allTypesCached.FirstOrDefault<Type>((Type t) => search.IsMatch(t.Name));
			if (type2 != null)
			{
				return type2;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(68, 1);
			defaultInterpolatedStringHandler.AppendLiteral("AccessTools.TypeSearch: Could not find type with regular expression ");
			defaultInterpolatedStringHandler.AppendFormatted<Regex>(search);
			FileLog.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			return null;
		}

		// Token: 0x06000354 RID: 852 RVA: 0x00011E9D File Offset: 0x0001009D
		public static void ClearTypeSearchCache()
		{
			AccessTools.allTypesCached = null;
		}

		// Token: 0x06000355 RID: 853 RVA: 0x00011EA8 File Offset: 0x000100A8
		public static Type[] GetTypesFromAssembly(Assembly assembly)
		{
			Type[] array;
			try
			{
				array = assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException ex)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(47, 2);
				defaultInterpolatedStringHandler.AppendLiteral("AccessTools.GetTypesFromAssembly: assembly ");
				defaultInterpolatedStringHandler.AppendFormatted<Assembly>(assembly);
				defaultInterpolatedStringHandler.AppendLiteral(" => ");
				defaultInterpolatedStringHandler.AppendFormatted<ReflectionTypeLoadException>(ex);
				FileLog.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
				array = ex.Types.Where<Type>((Type type) => type != null).ToArray<Type>();
			}
			return array;
		}

		// Token: 0x06000356 RID: 854 RVA: 0x00011F40 File Offset: 0x00010140
		public static IEnumerable<Type> AllTypes()
		{
			IEnumerable<Assembly> enumerable = AccessTools.AllAssemblies();
			Func<Assembly, IEnumerable<Type>> func;
			if ((func = AccessTools.<>O.<0>__GetTypesFromAssembly) == null)
			{
				func = (AccessTools.<>O.<0>__GetTypesFromAssembly = new Func<Assembly, IEnumerable<Type>>(AccessTools.GetTypesFromAssembly));
			}
			return enumerable.SelectMany<Assembly, Type>(func);
		}

		// Token: 0x06000357 RID: 855 RVA: 0x00011F67 File Offset: 0x00010167
		public static IEnumerable<Type> InnerTypes(Type type)
		{
			return type.GetNestedTypes(AccessTools.all);
		}

		// Token: 0x06000358 RID: 856 RVA: 0x00011F74 File Offset: 0x00010174
		public static T FindIncludingBaseTypes<T>(Type type, Func<Type, T> func) where T : class
		{
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
					goto Block_1;
				}
			}
			return t;
			Block_1:
			return default(T);
		}

		// Token: 0x06000359 RID: 857 RVA: 0x00011FA8 File Offset: 0x000101A8
		public static T FindIncludingInnerTypes<T>(Type type, Func<Type, T> func) where T : class
		{
			T t = func(type);
			if (t != null)
			{
				return t;
			}
			foreach (Type type2 in type.GetNestedTypes(AccessTools.all))
			{
				t = AccessTools.FindIncludingInnerTypes<T>(type2, func);
				if (t != null)
				{
					break;
				}
			}
			return t;
		}

		// Token: 0x0600035A RID: 858 RVA: 0x00011FF6 File Offset: 0x000101F6
		public static MethodInfo Identifiable(this MethodInfo method)
		{
			return (PlatformTriple.Current.GetIdentifiable(method) as MethodInfo) ?? method;
		}

		// Token: 0x0600035B RID: 859 RVA: 0x00012010 File Offset: 0x00010210
		public static FieldInfo DeclaredField(Type type, string name)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.DeclaredField: type is null");
				return null;
			}
			if (string.IsNullOrEmpty(name))
			{
				FileLog.Debug("AccessTools.DeclaredField: name is null/empty");
				return null;
			}
			FieldInfo field = type.GetField(name, AccessTools.allDeclared);
			if (field == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(67, 2);
				defaultInterpolatedStringHandler.AppendLiteral("AccessTools.DeclaredField: Could not find field for type ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(type);
				defaultInterpolatedStringHandler.AppendLiteral(" and name ");
				defaultInterpolatedStringHandler.AppendFormatted(name);
				FileLog.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			return field;
		}

		// Token: 0x0600035C RID: 860 RVA: 0x00012090 File Offset: 0x00010290
		public static FieldInfo DeclaredField(string typeColonName)
		{
			Tools.TypeAndName typeAndName = Tools.TypColonName(typeColonName);
			FieldInfo field = typeAndName.type.GetField(typeAndName.name, AccessTools.allDeclared);
			if (field == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(67, 2);
				defaultInterpolatedStringHandler.AppendLiteral("AccessTools.DeclaredField: Could not find field for type ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeAndName.type);
				defaultInterpolatedStringHandler.AppendLiteral(" and name ");
				defaultInterpolatedStringHandler.AppendFormatted(typeAndName.name);
				FileLog.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			return field;
		}

		// Token: 0x0600035D RID: 861 RVA: 0x00012108 File Offset: 0x00010308
		public static FieldInfo Field(Type type, string name)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.Field: type is null");
				return null;
			}
			if (string.IsNullOrEmpty(name))
			{
				FileLog.Debug("AccessTools.Field: name is null/empty");
				return null;
			}
			FieldInfo fieldInfo = AccessTools.FindIncludingBaseTypes<FieldInfo>(type, (Type t) => t.GetField(name, AccessTools.all));
			if (fieldInfo == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(59, 2);
				defaultInterpolatedStringHandler.AppendLiteral("AccessTools.Field: Could not find field for type ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(type);
				defaultInterpolatedStringHandler.AppendLiteral(" and name ");
				defaultInterpolatedStringHandler.AppendFormatted(name);
				FileLog.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			return fieldInfo;
		}

		// Token: 0x0600035E RID: 862 RVA: 0x000121A4 File Offset: 0x000103A4
		public static FieldInfo Field(string typeColonName)
		{
			Tools.TypeAndName info = Tools.TypColonName(typeColonName);
			FieldInfo fieldInfo = AccessTools.FindIncludingBaseTypes<FieldInfo>(info.type, (Type t) => t.GetField(info.name, AccessTools.all));
			if (fieldInfo == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(59, 2);
				defaultInterpolatedStringHandler.AppendLiteral("AccessTools.Field: Could not find field for type ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(info.type);
				defaultInterpolatedStringHandler.AppendLiteral(" and name ");
				defaultInterpolatedStringHandler.AppendFormatted(info.name);
				FileLog.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			return fieldInfo;
		}

		// Token: 0x0600035F RID: 863 RVA: 0x00012238 File Offset: 0x00010438
		public static FieldInfo DeclaredField(Type type, int idx)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.DeclaredField: type is null");
				return null;
			}
			FieldInfo fieldInfo = AccessTools.GetDeclaredFields(type).ElementAtOrDefault<FieldInfo>(idx);
			if (fieldInfo == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(66, 2);
				defaultInterpolatedStringHandler.AppendLiteral("AccessTools.DeclaredField: Could not find field for type ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(type);
				defaultInterpolatedStringHandler.AppendLiteral(" and idx ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(idx);
				FileLog.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			return fieldInfo;
		}

		// Token: 0x06000360 RID: 864 RVA: 0x000122A4 File Offset: 0x000104A4
		public static PropertyInfo DeclaredProperty(Type type, string name)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.DeclaredProperty: type is null");
				return null;
			}
			if (string.IsNullOrEmpty(name))
			{
				FileLog.Debug("AccessTools.DeclaredProperty: name is null/empty");
				return null;
			}
			PropertyInfo property = type.GetProperty(name, AccessTools.allDeclared);
			if (property == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(73, 2);
				defaultInterpolatedStringHandler.AppendLiteral("AccessTools.DeclaredProperty: Could not find property for type ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(type);
				defaultInterpolatedStringHandler.AppendLiteral(" and name ");
				defaultInterpolatedStringHandler.AppendFormatted(name);
				FileLog.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			return property;
		}

		// Token: 0x06000361 RID: 865 RVA: 0x00012324 File Offset: 0x00010524
		public static PropertyInfo DeclaredProperty(string typeColonName)
		{
			Tools.TypeAndName typeAndName = Tools.TypColonName(typeColonName);
			PropertyInfo property = typeAndName.type.GetProperty(typeAndName.name, AccessTools.allDeclared);
			if (property == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(73, 2);
				defaultInterpolatedStringHandler.AppendLiteral("AccessTools.DeclaredProperty: Could not find property for type ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeAndName.type);
				defaultInterpolatedStringHandler.AppendLiteral(" and name ");
				defaultInterpolatedStringHandler.AppendFormatted(typeAndName.name);
				FileLog.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			return property;
		}

		// Token: 0x06000362 RID: 866 RVA: 0x0001239C File Offset: 0x0001059C
		public static PropertyInfo DeclaredIndexer(Type type, Type[] parameters = null)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.DeclaredIndexer: type is null");
				return null;
			}
			PropertyInfo propertyInfo3;
			try
			{
				PropertyInfo propertyInfo;
				if (parameters != null)
				{
					propertyInfo = type.GetProperties(AccessTools.allDeclared).FirstOrDefault<PropertyInfo>((PropertyInfo property) => (from param in property.GetIndexParameters()
						select param.ParameterType).SequenceEqual<Type>(parameters));
				}
				else
				{
					propertyInfo = type.GetProperties(AccessTools.allDeclared).SingleOrDefault<PropertyInfo>((PropertyInfo property) => property.GetIndexParameters().Length != 0);
				}
				PropertyInfo propertyInfo2 = propertyInfo;
				if (propertyInfo2 == null)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(77, 2);
					defaultInterpolatedStringHandler.AppendLiteral("AccessTools.DeclaredIndexer: Could not find indexer for type ");
					defaultInterpolatedStringHandler.AppendFormatted<Type>(type);
					defaultInterpolatedStringHandler.AppendLiteral(" and parameters ");
					Type[] parameters2 = parameters;
					defaultInterpolatedStringHandler.AppendFormatted((parameters2 != null) ? parameters2.Description() : null);
					FileLog.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
				}
				propertyInfo3 = propertyInfo2;
			}
			catch (InvalidOperationException ex)
			{
				throw new AmbiguousMatchException("Multiple possible indexers were found.", ex);
			}
			return propertyInfo3;
		}

		// Token: 0x06000363 RID: 867 RVA: 0x00012494 File Offset: 0x00010694
		public static MethodInfo DeclaredPropertyGetter(Type type, string name)
		{
			PropertyInfo propertyInfo = AccessTools.DeclaredProperty(type, name);
			if (propertyInfo == null)
			{
				return null;
			}
			return propertyInfo.GetGetMethod(true);
		}

		// Token: 0x06000364 RID: 868 RVA: 0x000124A9 File Offset: 0x000106A9
		public static MethodInfo DeclaredPropertyGetter(string typeColonName)
		{
			PropertyInfo propertyInfo = AccessTools.DeclaredProperty(typeColonName);
			if (propertyInfo == null)
			{
				return null;
			}
			return propertyInfo.GetGetMethod(true);
		}

		// Token: 0x06000365 RID: 869 RVA: 0x000124BD File Offset: 0x000106BD
		public static MethodInfo DeclaredIndexerGetter(Type type, Type[] parameters = null)
		{
			PropertyInfo propertyInfo = AccessTools.DeclaredIndexer(type, parameters);
			if (propertyInfo == null)
			{
				return null;
			}
			return propertyInfo.GetGetMethod(true);
		}

		// Token: 0x06000366 RID: 870 RVA: 0x000124D2 File Offset: 0x000106D2
		public static MethodInfo DeclaredPropertySetter(Type type, string name)
		{
			PropertyInfo propertyInfo = AccessTools.DeclaredProperty(type, name);
			if (propertyInfo == null)
			{
				return null;
			}
			return propertyInfo.GetSetMethod(true);
		}

		// Token: 0x06000367 RID: 871 RVA: 0x000124E7 File Offset: 0x000106E7
		public static MethodInfo DeclaredPropertySetter(string typeColonName)
		{
			PropertyInfo propertyInfo = AccessTools.DeclaredProperty(typeColonName);
			if (propertyInfo == null)
			{
				return null;
			}
			return propertyInfo.GetSetMethod(true);
		}

		// Token: 0x06000368 RID: 872 RVA: 0x000124FB File Offset: 0x000106FB
		public static MethodInfo DeclaredIndexerSetter(Type type, Type[] parameters)
		{
			PropertyInfo propertyInfo = AccessTools.DeclaredIndexer(type, parameters);
			if (propertyInfo == null)
			{
				return null;
			}
			return propertyInfo.GetSetMethod(true);
		}

		// Token: 0x06000369 RID: 873 RVA: 0x00012510 File Offset: 0x00010710
		public static PropertyInfo Property(Type type, string name)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.Property: type is null");
				return null;
			}
			if (string.IsNullOrEmpty(name))
			{
				FileLog.Debug("AccessTools.Property: name is null/empty");
				return null;
			}
			PropertyInfo propertyInfo = AccessTools.FindIncludingBaseTypes<PropertyInfo>(type, (Type t) => t.GetProperty(name, AccessTools.all));
			if (propertyInfo == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(65, 2);
				defaultInterpolatedStringHandler.AppendLiteral("AccessTools.Property: Could not find property for type ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(type);
				defaultInterpolatedStringHandler.AppendLiteral(" and name ");
				defaultInterpolatedStringHandler.AppendFormatted(name);
				FileLog.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			return propertyInfo;
		}

		// Token: 0x0600036A RID: 874 RVA: 0x000125AC File Offset: 0x000107AC
		public static PropertyInfo Property(string typeColonName)
		{
			Tools.TypeAndName info = Tools.TypColonName(typeColonName);
			PropertyInfo propertyInfo = AccessTools.FindIncludingBaseTypes<PropertyInfo>(info.type, (Type t) => t.GetProperty(info.name, AccessTools.all));
			if (propertyInfo == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(65, 2);
				defaultInterpolatedStringHandler.AppendLiteral("AccessTools.Property: Could not find property for type ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(info.type);
				defaultInterpolatedStringHandler.AppendLiteral(" and name ");
				defaultInterpolatedStringHandler.AppendFormatted(info.name);
				FileLog.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			return propertyInfo;
		}

		// Token: 0x0600036B RID: 875 RVA: 0x00012640 File Offset: 0x00010840
		public static PropertyInfo Indexer(Type type, Type[] parameters = null)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.Indexer: type is null");
				return null;
			}
			Func<Type, PropertyInfo> func;
			if (parameters != null)
			{
				Func<PropertyInfo, bool> <>9__3;
				func = delegate(Type t)
				{
					IEnumerable<PropertyInfo> properties = t.GetProperties(AccessTools.all);
					Func<PropertyInfo, bool> func3;
					if ((func3 = <>9__3) == null)
					{
						func3 = (<>9__3 = (PropertyInfo property) => (from param in property.GetIndexParameters()
							select param.ParameterType).SequenceEqual<Type>(parameters));
					}
					return properties.FirstOrDefault<PropertyInfo>(func3);
				};
			}
			else
			{
				func = (Type t) => t.GetProperties(AccessTools.all).SingleOrDefault<PropertyInfo>((PropertyInfo property) => property.GetIndexParameters().Length != 0);
			}
			Func<Type, PropertyInfo> func2 = func;
			PropertyInfo propertyInfo2;
			try
			{
				PropertyInfo propertyInfo = AccessTools.FindIncludingBaseTypes<PropertyInfo>(type, func2);
				if (propertyInfo == null)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(69, 2);
					defaultInterpolatedStringHandler.AppendLiteral("AccessTools.Indexer: Could not find indexer for type ");
					defaultInterpolatedStringHandler.AppendFormatted<Type>(type);
					defaultInterpolatedStringHandler.AppendLiteral(" and parameters ");
					Type[] parameters2 = parameters;
					defaultInterpolatedStringHandler.AppendFormatted((parameters2 != null) ? parameters2.Description() : null);
					FileLog.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
				}
				propertyInfo2 = propertyInfo;
			}
			catch (InvalidOperationException ex)
			{
				throw new AmbiguousMatchException("Multiple possible indexers were found.", ex);
			}
			return propertyInfo2;
		}

		// Token: 0x0600036C RID: 876 RVA: 0x00012720 File Offset: 0x00010920
		public static MethodInfo PropertyGetter(Type type, string name)
		{
			PropertyInfo propertyInfo = AccessTools.Property(type, name);
			if (propertyInfo == null)
			{
				return null;
			}
			return propertyInfo.GetGetMethod(true);
		}

		// Token: 0x0600036D RID: 877 RVA: 0x00012735 File Offset: 0x00010935
		public static MethodInfo PropertyGetter(string typeColonName)
		{
			PropertyInfo propertyInfo = AccessTools.Property(typeColonName);
			if (propertyInfo == null)
			{
				return null;
			}
			return propertyInfo.GetGetMethod(true);
		}

		// Token: 0x0600036E RID: 878 RVA: 0x00012749 File Offset: 0x00010949
		public static MethodInfo IndexerGetter(Type type, Type[] parameters = null)
		{
			PropertyInfo propertyInfo = AccessTools.Indexer(type, parameters);
			if (propertyInfo == null)
			{
				return null;
			}
			return propertyInfo.GetGetMethod(true);
		}

		// Token: 0x0600036F RID: 879 RVA: 0x0001275E File Offset: 0x0001095E
		public static MethodInfo PropertySetter(Type type, string name)
		{
			PropertyInfo propertyInfo = AccessTools.Property(type, name);
			if (propertyInfo == null)
			{
				return null;
			}
			return propertyInfo.GetSetMethod(true);
		}

		// Token: 0x06000370 RID: 880 RVA: 0x00012773 File Offset: 0x00010973
		public static MethodInfo PropertySetter(string typeColonName)
		{
			PropertyInfo propertyInfo = AccessTools.Property(typeColonName);
			if (propertyInfo == null)
			{
				return null;
			}
			return propertyInfo.GetSetMethod(true);
		}

		// Token: 0x06000371 RID: 881 RVA: 0x00012787 File Offset: 0x00010987
		public static MethodInfo IndexerSetter(Type type, Type[] parameters = null)
		{
			PropertyInfo propertyInfo = AccessTools.Indexer(type, parameters);
			if (propertyInfo == null)
			{
				return null;
			}
			return propertyInfo.GetSetMethod(true);
		}

		// Token: 0x06000372 RID: 882 RVA: 0x0001279C File Offset: 0x0001099C
		public static EventInfo DeclaredEvent(Type type, string name)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.DeclaredEvent: type is null");
				return null;
			}
			if (string.IsNullOrEmpty(name))
			{
				FileLog.Debug("AccessTools.DeclaredEvent: name is null/empty");
				return null;
			}
			EventInfo @event = type.GetEvent(name, AccessTools.allDeclared);
			if (@event == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(67, 2);
				defaultInterpolatedStringHandler.AppendLiteral("AccessTools.DeclaredEvent: Could not find event for type ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(type);
				defaultInterpolatedStringHandler.AppendLiteral(" and name ");
				defaultInterpolatedStringHandler.AppendFormatted(name);
				FileLog.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			return @event;
		}

		// Token: 0x06000373 RID: 883 RVA: 0x0001281C File Offset: 0x00010A1C
		public static EventInfo DeclaredEvent(string typeColonName)
		{
			Tools.TypeAndName typeAndName = Tools.TypColonName(typeColonName);
			EventInfo @event = typeAndName.type.GetEvent(typeAndName.name, AccessTools.allDeclared);
			if (@event == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(67, 2);
				defaultInterpolatedStringHandler.AppendLiteral("AccessTools.DeclaredEvent: Could not find event for type ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeAndName.type);
				defaultInterpolatedStringHandler.AppendLiteral(" and name ");
				defaultInterpolatedStringHandler.AppendFormatted(typeAndName.name);
				FileLog.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			return @event;
		}

		// Token: 0x06000374 RID: 884 RVA: 0x00012894 File Offset: 0x00010A94
		public static EventInfo Event(Type type, string name)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.Event: type is null");
				return null;
			}
			if (string.IsNullOrEmpty(name))
			{
				FileLog.Debug("AccessTools.Event: name is null/empty");
				return null;
			}
			EventInfo eventInfo = AccessTools.FindIncludingBaseTypes<EventInfo>(type, (Type t) => t.GetEvent(name, AccessTools.all));
			if (eventInfo == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(59, 2);
				defaultInterpolatedStringHandler.AppendLiteral("AccessTools.Event: Could not find event for type ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(type);
				defaultInterpolatedStringHandler.AppendLiteral(" and name ");
				defaultInterpolatedStringHandler.AppendFormatted(name);
				FileLog.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			return eventInfo;
		}

		// Token: 0x06000375 RID: 885 RVA: 0x00012930 File Offset: 0x00010B30
		public static EventInfo Event(string typeColonName)
		{
			Tools.TypeAndName info = Tools.TypColonName(typeColonName);
			EventInfo eventInfo = AccessTools.FindIncludingBaseTypes<EventInfo>(info.type, (Type t) => t.GetEvent(info.name, AccessTools.all));
			if (eventInfo == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(59, 2);
				defaultInterpolatedStringHandler.AppendLiteral("AccessTools.Event: Could not find event for type ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(info.type);
				defaultInterpolatedStringHandler.AppendLiteral(" and name ");
				defaultInterpolatedStringHandler.AppendFormatted(info.name);
				FileLog.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			return eventInfo;
		}

		// Token: 0x06000376 RID: 886 RVA: 0x000129C2 File Offset: 0x00010BC2
		public static MethodInfo DeclaredEventAdder(Type type, string name)
		{
			EventInfo eventInfo = AccessTools.DeclaredEvent(type, name);
			if (eventInfo == null)
			{
				return null;
			}
			return eventInfo.GetAddMethod(true);
		}

		// Token: 0x06000377 RID: 887 RVA: 0x000129D7 File Offset: 0x00010BD7
		public static MethodInfo DeclaredEventAdder(string typeColonName)
		{
			EventInfo eventInfo = AccessTools.DeclaredEvent(typeColonName);
			if (eventInfo == null)
			{
				return null;
			}
			return eventInfo.GetAddMethod(true);
		}

		// Token: 0x06000378 RID: 888 RVA: 0x000129EB File Offset: 0x00010BEB
		public static MethodInfo EventAdder(Type type, string name)
		{
			EventInfo eventInfo = AccessTools.Event(type, name);
			if (eventInfo == null)
			{
				return null;
			}
			return eventInfo.GetAddMethod(true);
		}

		// Token: 0x06000379 RID: 889 RVA: 0x00012A00 File Offset: 0x00010C00
		public static MethodInfo EventAdder(string typeColonName)
		{
			EventInfo eventInfo = AccessTools.Event(typeColonName);
			if (eventInfo == null)
			{
				return null;
			}
			return eventInfo.GetAddMethod(true);
		}

		// Token: 0x0600037A RID: 890 RVA: 0x00012A14 File Offset: 0x00010C14
		public static MethodInfo DeclaredEventRemover(Type type, string name)
		{
			EventInfo eventInfo = AccessTools.DeclaredEvent(type, name);
			if (eventInfo == null)
			{
				return null;
			}
			return eventInfo.GetRemoveMethod(true);
		}

		// Token: 0x0600037B RID: 891 RVA: 0x00012A29 File Offset: 0x00010C29
		public static MethodInfo DeclaredEventRemover(string typeColonName)
		{
			EventInfo eventInfo = AccessTools.DeclaredEvent(typeColonName);
			if (eventInfo == null)
			{
				return null;
			}
			return eventInfo.GetRemoveMethod(true);
		}

		// Token: 0x0600037C RID: 892 RVA: 0x00012A3D File Offset: 0x00010C3D
		public static MethodInfo EventRemover(Type type, string name)
		{
			EventInfo eventInfo = AccessTools.Event(type, name);
			if (eventInfo == null)
			{
				return null;
			}
			return eventInfo.GetRemoveMethod(true);
		}

		// Token: 0x0600037D RID: 893 RVA: 0x00012A52 File Offset: 0x00010C52
		public static MethodInfo EventRemover(string typeColonName)
		{
			EventInfo eventInfo = AccessTools.Event(typeColonName);
			if (eventInfo == null)
			{
				return null;
			}
			return eventInfo.GetRemoveMethod(true);
		}

		// Token: 0x0600037E RID: 894 RVA: 0x00012A68 File Offset: 0x00010C68
		public static MethodInfo DeclaredMethod(Type type, string name, Type[] parameters = null, Type[] generics = null)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.DeclaredMethod: type is null");
				return null;
			}
			if (string.IsNullOrEmpty(name))
			{
				FileLog.Debug("AccessTools.DeclaredMethod: name is null/empty");
				return null;
			}
			ParameterModifier[] array = new ParameterModifier[0];
			MethodInfo methodInfo;
			if (parameters == null)
			{
				methodInfo = type.GetMethod(name, AccessTools.allDeclared);
			}
			else
			{
				methodInfo = type.GetMethod(name, AccessTools.allDeclared, null, parameters, array);
			}
			if (methodInfo == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(85, 3);
				defaultInterpolatedStringHandler.AppendLiteral("AccessTools.DeclaredMethod: Could not find method for type ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(type);
				defaultInterpolatedStringHandler.AppendLiteral(" and name ");
				defaultInterpolatedStringHandler.AppendFormatted(name);
				defaultInterpolatedStringHandler.AppendLiteral(" and parameters ");
				defaultInterpolatedStringHandler.AppendFormatted((parameters != null) ? parameters.Description() : null);
				FileLog.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
				return null;
			}
			if (generics != null)
			{
				methodInfo = methodInfo.MakeGenericMethod(generics);
			}
			return methodInfo;
		}

		// Token: 0x0600037F RID: 895 RVA: 0x00012B30 File Offset: 0x00010D30
		public static MethodInfo DeclaredMethod(string typeColonName, Type[] parameters = null, Type[] generics = null)
		{
			Tools.TypeAndName typeAndName = Tools.TypColonName(typeColonName);
			return AccessTools.DeclaredMethod(typeAndName.type, typeAndName.name, parameters, generics);
		}

		// Token: 0x06000380 RID: 896 RVA: 0x00012B58 File Offset: 0x00010D58
		public static MethodInfo Method(Type type, string name, Type[] parameters = null, Type[] generics = null)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.Method: type is null");
				return null;
			}
			if (string.IsNullOrEmpty(name))
			{
				FileLog.Debug("AccessTools.Method: name is null/empty");
				return null;
			}
			ParameterModifier[] modifiers = new ParameterModifier[0];
			MethodInfo methodInfo;
			if (parameters == null)
			{
				try
				{
					methodInfo = AccessTools.FindIncludingBaseTypes<MethodInfo>(type, (Type t) => t.GetMethod(name, AccessTools.all));
					goto IL_00D6;
				}
				catch (AmbiguousMatchException ex)
				{
					methodInfo = AccessTools.FindIncludingBaseTypes<MethodInfo>(type, (Type t) => t.GetMethod(name, AccessTools.all, null, Array.Empty<Type>(), modifiers));
					if (methodInfo == null)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(38, 2);
						defaultInterpolatedStringHandler.AppendLiteral("Ambiguous match in Harmony patch for ");
						defaultInterpolatedStringHandler.AppendFormatted<Type>(type);
						defaultInterpolatedStringHandler.AppendLiteral(":");
						defaultInterpolatedStringHandler.AppendFormatted(name);
						throw new AmbiguousMatchException(defaultInterpolatedStringHandler.ToStringAndClear(), ex);
					}
					goto IL_00D6;
				}
			}
			methodInfo = AccessTools.FindIncludingBaseTypes<MethodInfo>(type, (Type t) => t.GetMethod(name, AccessTools.all, null, parameters, modifiers));
			IL_00D6:
			if (methodInfo == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(77, 3);
				defaultInterpolatedStringHandler2.AppendLiteral("AccessTools.Method: Could not find method for type ");
				defaultInterpolatedStringHandler2.AppendFormatted<Type>(type);
				defaultInterpolatedStringHandler2.AppendLiteral(" and name ");
				defaultInterpolatedStringHandler2.AppendFormatted(name);
				defaultInterpolatedStringHandler2.AppendLiteral(" and parameters ");
				Type[] parameters2 = parameters;
				defaultInterpolatedStringHandler2.AppendFormatted((parameters2 != null) ? parameters2.Description() : null);
				FileLog.Debug(defaultInterpolatedStringHandler2.ToStringAndClear());
				return null;
			}
			if (generics != null)
			{
				methodInfo = methodInfo.MakeGenericMethod(generics);
			}
			return methodInfo;
		}

		// Token: 0x06000381 RID: 897 RVA: 0x00012CC4 File Offset: 0x00010EC4
		public static MethodInfo Method(string typeColonName, Type[] parameters = null, Type[] generics = null)
		{
			Tools.TypeAndName typeAndName = Tools.TypColonName(typeColonName);
			return AccessTools.Method(typeAndName.type, typeAndName.name, parameters, generics);
		}

		// Token: 0x06000382 RID: 898 RVA: 0x00012CEC File Offset: 0x00010EEC
		public static MethodInfo EnumeratorMoveNext(MethodBase method)
		{
			if (method == null)
			{
				FileLog.Debug("AccessTools.EnumeratorMoveNext: method is null");
				return null;
			}
			IEnumerable<KeyValuePair<OpCode, object>> enumerable = from pair in PatchProcessor.ReadMethodBody(method)
				where pair.Key == OpCodes.Newobj
				select pair;
			if (enumerable.Count<KeyValuePair<OpCode, object>>() != 1)
			{
				FileLog.Debug("AccessTools.EnumeratorMoveNext: " + method.FullDescription() + " contains no Newobj opcode");
				return null;
			}
			ConstructorInfo constructorInfo = enumerable.First<KeyValuePair<OpCode, object>>().Value as ConstructorInfo;
			if (constructorInfo == null)
			{
				FileLog.Debug("AccessTools.EnumeratorMoveNext: " + method.FullDescription() + " contains no constructor");
				return null;
			}
			Type declaringType = constructorInfo.DeclaringType;
			if (declaringType == null)
			{
				FileLog.Debug("AccessTools.EnumeratorMoveNext: " + method.FullDescription() + " refers to a global type");
				return null;
			}
			return AccessTools.Method(declaringType, "MoveNext", null, null);
		}

		// Token: 0x06000383 RID: 899 RVA: 0x00012DCC File Offset: 0x00010FCC
		public static MethodInfo AsyncMoveNext(MethodBase method)
		{
			if (method == null)
			{
				FileLog.Debug("AccessTools.AsyncMoveNext: method is null");
				return null;
			}
			AsyncStateMachineAttribute customAttribute = method.GetCustomAttribute<AsyncStateMachineAttribute>();
			if (customAttribute == null)
			{
				FileLog.Debug("AccessTools.AsyncMoveNext: Could not find AsyncStateMachine for " + method.FullDescription());
				return null;
			}
			Type stateMachineType = customAttribute.StateMachineType;
			MethodInfo methodInfo = AccessTools.DeclaredMethod(stateMachineType, "MoveNext", null, null);
			if (methodInfo == null)
			{
				FileLog.Debug("AccessTools.AsyncMoveNext: Could not find async method body for " + method.FullDescription());
				return null;
			}
			return methodInfo;
		}

		// Token: 0x06000384 RID: 900 RVA: 0x00012E39 File Offset: 0x00011039
		public static MethodInfo Finalizer(Type type)
		{
			return AccessTools.Method(type, "Finalize", null, null);
		}

		// Token: 0x06000385 RID: 901 RVA: 0x00012E48 File Offset: 0x00011048
		public static MethodInfo DeclaredFinalizer(Type type)
		{
			return AccessTools.DeclaredMethod(type, "Finalize", null, null);
		}

		// Token: 0x06000386 RID: 902 RVA: 0x00012E58 File Offset: 0x00011058
		public static List<string> GetMethodNames(Type type)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.GetMethodNames: type is null");
				return new List<string>();
			}
			return (from m in AccessTools.GetDeclaredMethods(type)
				select m.Name).ToList<string>();
		}

		// Token: 0x06000387 RID: 903 RVA: 0x00012EA7 File Offset: 0x000110A7
		public static List<string> GetMethodNames(object instance)
		{
			if (instance == null)
			{
				FileLog.Debug("AccessTools.GetMethodNames: instance is null");
				return new List<string>();
			}
			return AccessTools.GetMethodNames(instance.GetType());
		}

		// Token: 0x06000388 RID: 904 RVA: 0x00012EC8 File Offset: 0x000110C8
		public static List<string> GetFieldNames(Type type)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.GetFieldNames: type is null");
				return new List<string>();
			}
			return (from f in AccessTools.GetDeclaredFields(type)
				select f.Name).ToList<string>();
		}

		// Token: 0x06000389 RID: 905 RVA: 0x00012F17 File Offset: 0x00011117
		public static List<string> GetFieldNames(object instance)
		{
			if (instance == null)
			{
				FileLog.Debug("AccessTools.GetFieldNames: instance is null");
				return new List<string>();
			}
			return AccessTools.GetFieldNames(instance.GetType());
		}

		// Token: 0x0600038A RID: 906 RVA: 0x00012F38 File Offset: 0x00011138
		public static List<string> GetPropertyNames(Type type)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.GetPropertyNames: type is null");
				return new List<string>();
			}
			return (from f in AccessTools.GetDeclaredProperties(type)
				select f.Name).ToList<string>();
		}

		// Token: 0x0600038B RID: 907 RVA: 0x00012F87 File Offset: 0x00011187
		public static List<string> GetPropertyNames(object instance)
		{
			if (instance == null)
			{
				FileLog.Debug("AccessTools.GetPropertyNames: instance is null");
				return new List<string>();
			}
			return AccessTools.GetPropertyNames(instance.GetType());
		}

		// Token: 0x0600038C RID: 908 RVA: 0x00012FA8 File Offset: 0x000111A8
		public static Type GetUnderlyingType(this MemberInfo member)
		{
			MemberTypes memberType = member.MemberType;
			if (memberType <= MemberTypes.Field)
			{
				if (memberType == MemberTypes.Event)
				{
					return ((EventInfo)member).EventHandlerType;
				}
				if (memberType == MemberTypes.Field)
				{
					return ((FieldInfo)member).FieldType;
				}
			}
			else
			{
				if (memberType == MemberTypes.Method)
				{
					return ((MethodInfo)member).ReturnType;
				}
				if (memberType == MemberTypes.Property)
				{
					return ((PropertyInfo)member).PropertyType;
				}
			}
			throw new ArgumentException("Member must be of type EventInfo, FieldInfo, MethodInfo, or PropertyInfo");
		}

		// Token: 0x0600038D RID: 909 RVA: 0x0001301C File Offset: 0x0001121C
		public static MethodInfo GetMethodByModuleAndToken(string moduleGUID, int token)
		{
			Module module = (from a in AppDomain.CurrentDomain.GetAssemblies()
				where !a.FullName.StartsWith("Microsoft.VisualStudio")
				select a).SelectMany<Assembly, Module>((Assembly a) => a.GetLoadedModules()).First<Module>((Module m) => m.ModuleVersionId.ToString() == moduleGUID);
			if (!(module == null))
			{
				return (MethodInfo)module.ResolveMethod(token);
			}
			return null;
		}

		// Token: 0x0600038E RID: 910 RVA: 0x000130B1 File Offset: 0x000112B1
		public static bool IsDeclaredMember<T>(this T member) where T : MemberInfo
		{
			return member.DeclaringType == member.ReflectedType;
		}

		// Token: 0x0600038F RID: 911 RVA: 0x000130D0 File Offset: 0x000112D0
		public static T GetDeclaredMember<T>(this T member) where T : MemberInfo
		{
			if (member.DeclaringType == null || member.IsDeclaredMember<T>())
			{
				return member;
			}
			int metadataToken = member.MetadataToken;
			Type declaringType = member.DeclaringType;
			MemberInfo[] array = ((declaringType != null) ? declaringType.GetMembers(AccessTools.all) : null) ?? Array.Empty<MemberInfo>();
			foreach (MemberInfo memberInfo in array)
			{
				if (memberInfo.MetadataToken == metadataToken)
				{
					return (T)((object)memberInfo);
				}
			}
			return member;
		}

		// Token: 0x06000390 RID: 912 RVA: 0x00013150 File Offset: 0x00011350
		public static ConstructorInfo DeclaredConstructor(Type type, Type[] parameters = null, bool searchForStatic = false)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.DeclaredConstructor: type is null");
				return null;
			}
			if (parameters == null)
			{
				parameters = Array.Empty<Type>();
			}
			BindingFlags bindingFlags = (searchForStatic ? (AccessTools.allDeclared & ~BindingFlags.Instance) : (AccessTools.allDeclared & ~BindingFlags.Static));
			return type.GetConstructor(bindingFlags, null, parameters, Array.Empty<ParameterModifier>());
		}

		// Token: 0x06000391 RID: 913 RVA: 0x0001319C File Offset: 0x0001139C
		public static ConstructorInfo Constructor(Type type, Type[] parameters = null, bool searchForStatic = false)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.ConstructorInfo: type is null");
				return null;
			}
			if (parameters == null)
			{
				parameters = Array.Empty<Type>();
			}
			BindingFlags flags = (searchForStatic ? (AccessTools.all & ~BindingFlags.Instance) : (AccessTools.all & ~BindingFlags.Static));
			return AccessTools.FindIncludingBaseTypes<ConstructorInfo>(type, (Type t) => t.GetConstructor(flags, null, parameters, Array.Empty<ParameterModifier>()));
		}

		// Token: 0x06000392 RID: 914 RVA: 0x00013208 File Offset: 0x00011408
		public static List<ConstructorInfo> GetDeclaredConstructors(Type type, bool? searchForStatic = null)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.GetDeclaredConstructors: type is null");
				return new List<ConstructorInfo>();
			}
			BindingFlags bindingFlags = AccessTools.allDeclared;
			if (searchForStatic != null)
			{
				bindingFlags = (searchForStatic.Value ? (bindingFlags & ~BindingFlags.Instance) : (bindingFlags & ~BindingFlags.Static));
			}
			return (from method in type.GetConstructors(bindingFlags)
				where method.DeclaringType == type
				select method).ToList<ConstructorInfo>();
		}

		// Token: 0x06000393 RID: 915 RVA: 0x0001327F File Offset: 0x0001147F
		public static List<MethodInfo> GetDeclaredMethods(Type type)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.GetDeclaredMethods: type is null");
				return new List<MethodInfo>();
			}
			return type.GetMethods(AccessTools.allDeclared).ToList<MethodInfo>();
		}

		// Token: 0x06000394 RID: 916 RVA: 0x000132A4 File Offset: 0x000114A4
		public static List<PropertyInfo> GetDeclaredProperties(Type type)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.GetDeclaredProperties: type is null");
				return new List<PropertyInfo>();
			}
			return type.GetProperties(AccessTools.allDeclared).ToList<PropertyInfo>();
		}

		// Token: 0x06000395 RID: 917 RVA: 0x000132C9 File Offset: 0x000114C9
		public static List<FieldInfo> GetDeclaredFields(Type type)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.GetDeclaredFields: type is null");
				return new List<FieldInfo>();
			}
			return type.GetFields(AccessTools.allDeclared).ToList<FieldInfo>();
		}

		// Token: 0x06000396 RID: 918 RVA: 0x000132F0 File Offset: 0x000114F0
		public static Type GetReturnedType(MethodBase methodOrConstructor)
		{
			if (methodOrConstructor == null)
			{
				FileLog.Debug("AccessTools.GetReturnedType: methodOrConstructor is null");
				return null;
			}
			ConstructorInfo constructorInfo = methodOrConstructor as ConstructorInfo;
			if (constructorInfo != null)
			{
				return typeof(void);
			}
			return ((MethodInfo)methodOrConstructor).ReturnType;
		}

		// Token: 0x06000397 RID: 919 RVA: 0x0001332C File Offset: 0x0001152C
		public static Type Inner(Type type, string name)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.Inner: type is null");
				return null;
			}
			if (string.IsNullOrEmpty(name))
			{
				FileLog.Debug("AccessTools.Inner: name is null/empty");
				return null;
			}
			return AccessTools.FindIncludingBaseTypes<Type>(type, (Type t) => t.GetNestedType(name, AccessTools.all));
		}

		// Token: 0x06000398 RID: 920 RVA: 0x00013380 File Offset: 0x00011580
		public static Type FirstInner(Type type, Func<Type, bool> predicate)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.FirstInner: type is null");
				return null;
			}
			if (predicate == null)
			{
				FileLog.Debug("AccessTools.FirstInner: predicate is null");
				return null;
			}
			return type.GetNestedTypes(AccessTools.all).FirstOrDefault<Type>((Type subType) => predicate(subType));
		}

		// Token: 0x06000399 RID: 921 RVA: 0x000133DC File Offset: 0x000115DC
		public static MethodInfo FirstMethod(Type type, Func<MethodInfo, bool> predicate)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.FirstMethod: type is null");
				return null;
			}
			if (predicate == null)
			{
				FileLog.Debug("AccessTools.FirstMethod: predicate is null");
				return null;
			}
			return type.GetMethods(AccessTools.allDeclared).FirstOrDefault<MethodInfo>((MethodInfo method) => predicate(method));
		}

		// Token: 0x0600039A RID: 922 RVA: 0x00013438 File Offset: 0x00011638
		public static ConstructorInfo FirstConstructor(Type type, Func<ConstructorInfo, bool> predicate)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.FirstConstructor: type is null");
				return null;
			}
			if (predicate == null)
			{
				FileLog.Debug("AccessTools.FirstConstructor: predicate is null");
				return null;
			}
			return type.GetConstructors(AccessTools.allDeclared).FirstOrDefault<ConstructorInfo>((ConstructorInfo constructor) => predicate(constructor));
		}

		// Token: 0x0600039B RID: 923 RVA: 0x00013494 File Offset: 0x00011694
		public static PropertyInfo FirstProperty(Type type, Func<PropertyInfo, bool> predicate)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.FirstProperty: type is null");
				return null;
			}
			if (predicate == null)
			{
				FileLog.Debug("AccessTools.FirstProperty: predicate is null");
				return null;
			}
			return type.GetProperties(AccessTools.allDeclared).FirstOrDefault<PropertyInfo>((PropertyInfo property) => predicate(property));
		}

		// Token: 0x0600039C RID: 924 RVA: 0x000134ED File Offset: 0x000116ED
		public static Type[] GetTypes(object[] parameters)
		{
			if (parameters == null)
			{
				return Array.Empty<Type>();
			}
			return parameters.Select<object, Type>(delegate(object p)
			{
				if (p != null)
				{
					return p.GetType();
				}
				return typeof(object);
			}).ToArray<Type>();
		}

		// Token: 0x0600039D RID: 925 RVA: 0x00013524 File Offset: 0x00011724
		public static object[] ActualParameters(MethodBase method, object[] inputs)
		{
			List<Type> inputTypes = inputs.Select<object, Type>(delegate(object obj)
			{
				if (obj == null)
				{
					return null;
				}
				return obj.GetType();
			}).ToList<Type>();
			return (from p in method.GetParameters()
				select p.ParameterType).Select<Type, object>(delegate(Type pType)
			{
				int num = inputTypes.FindIndex((Type inType) => inType != null && pType.IsAssignableFrom(inType));
				if (num >= 0)
				{
					return inputs[num];
				}
				return AccessTools.GetDefaultValue(pType);
			}).ToArray<object>();
		}

		// Token: 0x0600039E RID: 926 RVA: 0x000135B4 File Offset: 0x000117B4
		public static AccessTools.FieldRef<T, F> FieldRefAccess<T, F>(string fieldName)
		{
			if (fieldName == null)
			{
				throw new ArgumentNullException("fieldName");
			}
			AccessTools.FieldRef<T, F> fieldRef;
			try
			{
				Type typeFromHandle = typeof(T);
				if (typeFromHandle.IsValueType)
				{
					throw new ArgumentException("T (FieldRefAccess instance type) must not be a value type");
				}
				fieldRef = Tools.FieldRefAccess<T, F>(Tools.GetInstanceField(typeFromHandle, fieldName), false);
			}
			catch (Exception ex)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(43, 3);
				defaultInterpolatedStringHandler.AppendLiteral("FieldRefAccess<");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeof(T));
				defaultInterpolatedStringHandler.AppendLiteral(", ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeof(F));
				defaultInterpolatedStringHandler.AppendLiteral("> for ");
				defaultInterpolatedStringHandler.AppendFormatted(fieldName);
				defaultInterpolatedStringHandler.AppendLiteral(" caused an exception");
				throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear(), ex);
			}
			return fieldRef;
		}

		// Token: 0x0600039F RID: 927 RVA: 0x00013684 File Offset: 0x00011884
		public static ref F FieldRefAccess<T, F>(T instance, string fieldName)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			if (fieldName == null)
			{
				throw new ArgumentNullException("fieldName");
			}
			ref F ptr;
			try
			{
				Type typeFromHandle = typeof(T);
				if (typeFromHandle.IsValueType)
				{
					throw new ArgumentException("T (FieldRefAccess instance type) must not be a value type");
				}
				ptr = Tools.FieldRefAccess<T, F>(Tools.GetInstanceField(typeFromHandle, fieldName), false)(instance);
			}
			catch (Exception ex)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(45, 4);
				defaultInterpolatedStringHandler.AppendLiteral("FieldRefAccess<");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeof(T));
				defaultInterpolatedStringHandler.AppendLiteral(", ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeof(F));
				defaultInterpolatedStringHandler.AppendLiteral("> for ");
				defaultInterpolatedStringHandler.AppendFormatted<T>(instance);
				defaultInterpolatedStringHandler.AppendLiteral(", ");
				defaultInterpolatedStringHandler.AppendFormatted(fieldName);
				defaultInterpolatedStringHandler.AppendLiteral(" caused an exception");
				throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear(), ex);
			}
			return ref ptr;
		}

		// Token: 0x060003A0 RID: 928 RVA: 0x00013784 File Offset: 0x00011984
		public static AccessTools.FieldRef<object, F> FieldRefAccess<F>(Type type, string fieldName)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (fieldName == null)
			{
				throw new ArgumentNullException("fieldName");
			}
			AccessTools.FieldRef<object, F> fieldRef;
			try
			{
				FieldInfo fieldInfo = AccessTools.Field(type, fieldName);
				if (fieldInfo == null)
				{
					throw new MissingFieldException(type.Name, fieldName);
				}
				if (!fieldInfo.IsStatic)
				{
					Type declaringType = fieldInfo.DeclaringType;
					if (declaringType != null && declaringType.IsValueType)
					{
						throw new ArgumentException("Either FieldDeclaringType must be a class or field must be static");
					}
				}
				fieldRef = Tools.FieldRefAccess<object, F>(fieldInfo, true);
			}
			catch (Exception ex)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(43, 3);
				defaultInterpolatedStringHandler.AppendLiteral("FieldRefAccess<");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeof(F));
				defaultInterpolatedStringHandler.AppendLiteral("> for ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(type);
				defaultInterpolatedStringHandler.AppendLiteral(", ");
				defaultInterpolatedStringHandler.AppendFormatted(fieldName);
				defaultInterpolatedStringHandler.AppendLiteral(" caused an exception");
				throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear(), ex);
			}
			return fieldRef;
		}

		// Token: 0x060003A1 RID: 929 RVA: 0x00013870 File Offset: 0x00011A70
		public static AccessTools.FieldRef<object, F> FieldRefAccess<F>(string typeColonName)
		{
			Tools.TypeAndName typeAndName = Tools.TypColonName(typeColonName);
			return AccessTools.FieldRefAccess<F>(typeAndName.type, typeAndName.name);
		}

		// Token: 0x060003A2 RID: 930 RVA: 0x00013898 File Offset: 0x00011A98
		public static AccessTools.FieldRef<T, F> FieldRefAccess<T, F>(FieldInfo fieldInfo)
		{
			if (fieldInfo == null)
			{
				throw new ArgumentNullException("fieldInfo");
			}
			AccessTools.FieldRef<T, F> fieldRef;
			try
			{
				Type typeFromHandle = typeof(T);
				if (typeFromHandle.IsValueType)
				{
					throw new ArgumentException("T (FieldRefAccess instance type) must not be a value type");
				}
				bool flag = false;
				if (!fieldInfo.IsStatic)
				{
					Type declaringType = fieldInfo.DeclaringType;
					if (declaringType != null)
					{
						if (declaringType.IsValueType)
						{
							throw new ArgumentException("Either FieldDeclaringType must be a class or field must be static");
						}
						flag = Tools.FieldRefNeedsClasscast(typeFromHandle, declaringType);
					}
				}
				fieldRef = Tools.FieldRefAccess<T, F>(fieldInfo, flag);
			}
			catch (Exception ex)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(43, 3);
				defaultInterpolatedStringHandler.AppendLiteral("FieldRefAccess<");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeof(T));
				defaultInterpolatedStringHandler.AppendLiteral(", ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeof(F));
				defaultInterpolatedStringHandler.AppendLiteral("> for ");
				defaultInterpolatedStringHandler.AppendFormatted<FieldInfo>(fieldInfo);
				defaultInterpolatedStringHandler.AppendLiteral(" caused an exception");
				throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear(), ex);
			}
			return fieldRef;
		}

		// Token: 0x060003A3 RID: 931 RVA: 0x00013994 File Offset: 0x00011B94
		public static ref F FieldRefAccess<T, F>(T instance, FieldInfo fieldInfo)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			if (fieldInfo == null)
			{
				throw new ArgumentNullException("fieldInfo");
			}
			ref F ptr;
			try
			{
				Type typeFromHandle = typeof(T);
				if (typeFromHandle.IsValueType)
				{
					throw new ArgumentException("T (FieldRefAccess instance type) must not be a value type");
				}
				if (fieldInfo.IsStatic)
				{
					throw new ArgumentException("Field must not be static");
				}
				bool flag = false;
				Type declaringType = fieldInfo.DeclaringType;
				if (declaringType != null)
				{
					if (declaringType.IsValueType)
					{
						throw new ArgumentException("FieldDeclaringType must be a class");
					}
					flag = Tools.FieldRefNeedsClasscast(typeFromHandle, declaringType);
				}
				ptr = Tools.FieldRefAccess<T, F>(fieldInfo, flag)(instance);
			}
			catch (Exception ex)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(45, 4);
				defaultInterpolatedStringHandler.AppendLiteral("FieldRefAccess<");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeof(T));
				defaultInterpolatedStringHandler.AppendLiteral(", ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeof(F));
				defaultInterpolatedStringHandler.AppendLiteral("> for ");
				defaultInterpolatedStringHandler.AppendFormatted<T>(instance);
				defaultInterpolatedStringHandler.AppendLiteral(", ");
				defaultInterpolatedStringHandler.AppendFormatted<FieldInfo>(fieldInfo);
				defaultInterpolatedStringHandler.AppendLiteral(" caused an exception");
				throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear(), ex);
			}
			return ref ptr;
		}

		// Token: 0x060003A4 RID: 932 RVA: 0x00013AC8 File Offset: 0x00011CC8
		public static AccessTools.StructFieldRef<T, F> StructFieldRefAccess<T, F>(string fieldName) where T : struct
		{
			if (fieldName == null)
			{
				throw new ArgumentNullException("fieldName");
			}
			AccessTools.StructFieldRef<T, F> structFieldRef;
			try
			{
				structFieldRef = Tools.StructFieldRefAccess<T, F>(Tools.GetInstanceField(typeof(T), fieldName));
			}
			catch (Exception ex)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(49, 3);
				defaultInterpolatedStringHandler.AppendLiteral("StructFieldRefAccess<");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeof(T));
				defaultInterpolatedStringHandler.AppendLiteral(", ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeof(F));
				defaultInterpolatedStringHandler.AppendLiteral("> for ");
				defaultInterpolatedStringHandler.AppendFormatted(fieldName);
				defaultInterpolatedStringHandler.AppendLiteral(" caused an exception");
				throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear(), ex);
			}
			return structFieldRef;
		}

		// Token: 0x060003A5 RID: 933 RVA: 0x00013B80 File Offset: 0x00011D80
		public static ref F StructFieldRefAccess<T, F>(ref T instance, string fieldName) where T : struct
		{
			if (fieldName == null)
			{
				throw new ArgumentNullException("fieldName");
			}
			ref F ptr;
			try
			{
				ptr = Tools.StructFieldRefAccess<T, F>(Tools.GetInstanceField(typeof(T), fieldName))(ref instance);
			}
			catch (Exception ex)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(51, 4);
				defaultInterpolatedStringHandler.AppendLiteral("StructFieldRefAccess<");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeof(T));
				defaultInterpolatedStringHandler.AppendLiteral(", ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeof(F));
				defaultInterpolatedStringHandler.AppendLiteral("> for ");
				defaultInterpolatedStringHandler.AppendFormatted<T>(instance);
				defaultInterpolatedStringHandler.AppendLiteral(", ");
				defaultInterpolatedStringHandler.AppendFormatted(fieldName);
				defaultInterpolatedStringHandler.AppendLiteral(" caused an exception");
				throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear(), ex);
			}
			return ref ptr;
		}

		// Token: 0x060003A6 RID: 934 RVA: 0x00013C5C File Offset: 0x00011E5C
		public static AccessTools.StructFieldRef<T, F> StructFieldRefAccess<T, F>(FieldInfo fieldInfo) where T : struct
		{
			if (fieldInfo == null)
			{
				throw new ArgumentNullException("fieldInfo");
			}
			AccessTools.StructFieldRef<T, F> structFieldRef;
			try
			{
				Tools.ValidateStructField<T, F>(fieldInfo);
				structFieldRef = Tools.StructFieldRefAccess<T, F>(fieldInfo);
			}
			catch (Exception ex)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(49, 3);
				defaultInterpolatedStringHandler.AppendLiteral("StructFieldRefAccess<");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeof(T));
				defaultInterpolatedStringHandler.AppendLiteral(", ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeof(F));
				defaultInterpolatedStringHandler.AppendLiteral("> for ");
				defaultInterpolatedStringHandler.AppendFormatted<FieldInfo>(fieldInfo);
				defaultInterpolatedStringHandler.AppendLiteral(" caused an exception");
				throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear(), ex);
			}
			return structFieldRef;
		}

		// Token: 0x060003A7 RID: 935 RVA: 0x00013D0C File Offset: 0x00011F0C
		public static ref F StructFieldRefAccess<T, F>(ref T instance, FieldInfo fieldInfo) where T : struct
		{
			if (fieldInfo == null)
			{
				throw new ArgumentNullException("fieldInfo");
			}
			ref F ptr;
			try
			{
				Tools.ValidateStructField<T, F>(fieldInfo);
				ptr = Tools.StructFieldRefAccess<T, F>(fieldInfo)(ref instance);
			}
			catch (Exception ex)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(51, 4);
				defaultInterpolatedStringHandler.AppendLiteral("StructFieldRefAccess<");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeof(T));
				defaultInterpolatedStringHandler.AppendLiteral(", ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeof(F));
				defaultInterpolatedStringHandler.AppendLiteral("> for ");
				defaultInterpolatedStringHandler.AppendFormatted<T>(instance);
				defaultInterpolatedStringHandler.AppendLiteral(", ");
				defaultInterpolatedStringHandler.AppendFormatted<FieldInfo>(fieldInfo);
				defaultInterpolatedStringHandler.AppendLiteral(" caused an exception");
				throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear(), ex);
			}
			return ref ptr;
		}

		// Token: 0x060003A8 RID: 936 RVA: 0x00013DE0 File Offset: 0x00011FE0
		public static ref F StaticFieldRefAccess<T, F>(string fieldName)
		{
			return AccessTools.StaticFieldRefAccess<F>(typeof(T), fieldName);
		}

		// Token: 0x060003A9 RID: 937 RVA: 0x00013DF4 File Offset: 0x00011FF4
		public static ref F StaticFieldRefAccess<F>(Type type, string fieldName)
		{
			ref F ptr;
			try
			{
				FieldInfo fieldInfo = AccessTools.Field(type, fieldName);
				if (fieldInfo == null)
				{
					throw new MissingFieldException(type.Name, fieldName);
				}
				ptr = Tools.StaticFieldRefAccess<F>(fieldInfo)();
			}
			catch (Exception ex)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(49, 3);
				defaultInterpolatedStringHandler.AppendLiteral("StaticFieldRefAccess<");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeof(F));
				defaultInterpolatedStringHandler.AppendLiteral("> for ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(type);
				defaultInterpolatedStringHandler.AppendLiteral(", ");
				defaultInterpolatedStringHandler.AppendFormatted(fieldName);
				defaultInterpolatedStringHandler.AppendLiteral(" caused an exception");
				throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear(), ex);
			}
			return ref ptr;
		}

		// Token: 0x060003AA RID: 938 RVA: 0x00013EA4 File Offset: 0x000120A4
		public static ref F StaticFieldRefAccess<F>(string typeColonName)
		{
			Tools.TypeAndName typeAndName = Tools.TypColonName(typeColonName);
			return AccessTools.StaticFieldRefAccess<F>(typeAndName.type, typeAndName.name);
		}

		// Token: 0x060003AB RID: 939 RVA: 0x00013ECC File Offset: 0x000120CC
		public static ref F StaticFieldRefAccess<T, F>(FieldInfo fieldInfo)
		{
			if (fieldInfo == null)
			{
				throw new ArgumentNullException("fieldInfo");
			}
			ref F ptr;
			try
			{
				ptr = Tools.StaticFieldRefAccess<F>(fieldInfo)();
			}
			catch (Exception ex)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(49, 3);
				defaultInterpolatedStringHandler.AppendLiteral("StaticFieldRefAccess<");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeof(T));
				defaultInterpolatedStringHandler.AppendLiteral(", ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeof(F));
				defaultInterpolatedStringHandler.AppendLiteral("> for ");
				defaultInterpolatedStringHandler.AppendFormatted<FieldInfo>(fieldInfo);
				defaultInterpolatedStringHandler.AppendLiteral(" caused an exception");
				throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear(), ex);
			}
			return ref ptr;
		}

		// Token: 0x060003AC RID: 940 RVA: 0x00013F7C File Offset: 0x0001217C
		public static AccessTools.FieldRef<F> StaticFieldRefAccess<F>(FieldInfo fieldInfo)
		{
			if (fieldInfo == null)
			{
				throw new ArgumentNullException("fieldInfo");
			}
			AccessTools.FieldRef<F> fieldRef;
			try
			{
				fieldRef = Tools.StaticFieldRefAccess<F>(fieldInfo);
			}
			catch (Exception ex)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(47, 2);
				defaultInterpolatedStringHandler.AppendLiteral("StaticFieldRefAccess<");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeof(F));
				defaultInterpolatedStringHandler.AppendLiteral("> for ");
				defaultInterpolatedStringHandler.AppendFormatted<FieldInfo>(fieldInfo);
				defaultInterpolatedStringHandler.AppendLiteral(" caused an exception");
				throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear(), ex);
			}
			return fieldRef;
		}

		// Token: 0x060003AD RID: 941 RVA: 0x00014008 File Offset: 0x00012208
		[Obsolete("This overload only exists for runtime backwards compatibility and will be removed in Harmony 3. Use MethodDelegate(MethodInfo, object, bool, Type[]) instead")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static DelegateType MethodDelegate<DelegateType>(MethodInfo method, object instance, bool virtualCall) where DelegateType : Delegate
		{
			return AccessTools.MethodDelegate<DelegateType>(method, instance, virtualCall, null);
		}

		// Token: 0x060003AE RID: 942 RVA: 0x00014014 File Offset: 0x00012214
		public static DelegateType MethodDelegate<DelegateType>(MethodInfo method, object instance = null, bool virtualCall = true, Type[] delegateArgs = null) where DelegateType : Delegate
		{
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			Type typeFromHandle = typeof(DelegateType);
			if (method.IsStatic)
			{
				return (DelegateType)((object)Delegate.CreateDelegate(typeFromHandle, method));
			}
			Type type = method.DeclaringType;
			if (type != null && type.IsInterface && !virtualCall)
			{
				throw new ArgumentException("Interface methods must be called virtually");
			}
			if (instance == null)
			{
				ParameterInfo[] parameters = typeFromHandle.GetMethod("Invoke").GetParameters();
				if (parameters.Length == 0)
				{
					Delegate.CreateDelegate(typeof(DelegateType), method);
					throw new ArgumentException("Invalid delegate type");
				}
				Type parameterType = parameters[0].ParameterType;
				if (type != null && type.IsInterface && parameterType.IsValueType)
				{
					InterfaceMapping interfaceMap = parameterType.GetInterfaceMap(type);
					method = interfaceMap.TargetMethods[Array.IndexOf<MethodInfo>(interfaceMap.InterfaceMethods, method)];
					type = parameterType;
				}
				if (type != null && virtualCall)
				{
					if (type.IsInterface)
					{
						return (DelegateType)((object)Delegate.CreateDelegate(typeFromHandle, method));
					}
					if (parameterType.IsInterface)
					{
						InterfaceMapping interfaceMap2 = type.GetInterfaceMap(parameterType);
						MethodInfo methodInfo = interfaceMap2.InterfaceMethods[Array.IndexOf<MethodInfo>(interfaceMap2.TargetMethods, method)];
						return (DelegateType)((object)Delegate.CreateDelegate(typeFromHandle, methodInfo));
					}
					if (!type.IsValueType)
					{
						return (DelegateType)((object)Delegate.CreateDelegate(typeFromHandle, method.GetBaseDefinition()));
					}
				}
				ParameterInfo[] parameters2 = method.GetParameters();
				int num = parameters2.Length;
				Type[] array = new Type[num + 1];
				array[0] = type;
				for (int i = 0; i < num; i++)
				{
					array[i + 1] = parameters2[i].ParameterType;
				}
				Type[] array2 = delegateArgs ?? typeFromHandle.GetGenericArguments();
				Type[] array3 = ((array2.Length < array.Length) ? array : array2);
				DynamicMethodDefinition dynamicMethodDefinition = new DynamicMethodDefinition("OpenInstanceDelegate_" + method.Name, method.ReturnType, array3);
				ILGenerator ilgenerator = dynamicMethodDefinition.GetILGenerator();
				if (type != null && type.IsValueType && array2.Length != 0 && !array2[0].IsByRef)
				{
					ilgenerator.Emit(OpCodes.Ldarga_S, 0);
				}
				else
				{
					ilgenerator.Emit(OpCodes.Ldarg_0);
				}
				for (int j = 1; j < array.Length; j++)
				{
					ilgenerator.Emit(OpCodes.Ldarg, j);
					if (array[j].IsValueType && j < array2.Length && !array2[j].IsValueType)
					{
						ilgenerator.Emit(OpCodes.Unbox_Any, array[j]);
					}
				}
				ilgenerator.Emit(OpCodes.Call, method);
				ilgenerator.Emit(OpCodes.Ret);
				return (DelegateType)((object)dynamicMethodDefinition.Generate().CreateDelegate(typeFromHandle));
			}
			else
			{
				if (virtualCall)
				{
					return (DelegateType)((object)Delegate.CreateDelegate(typeFromHandle, instance, method.GetBaseDefinition()));
				}
				if (type != null && !type.IsInstanceOfType(instance))
				{
					Delegate.CreateDelegate(typeof(DelegateType), instance, method);
					throw new ArgumentException("Invalid delegate type");
				}
				if (AccessTools.IsMonoRuntime)
				{
					DynamicMethodDefinition dynamicMethodDefinition2 = new DynamicMethodDefinition("LdftnDelegate_" + method.Name, typeFromHandle, new Type[] { typeof(object) });
					ILGenerator ilgenerator2 = dynamicMethodDefinition2.GetILGenerator();
					ilgenerator2.Emit(OpCodes.Ldarg_0);
					ilgenerator2.Emit(OpCodes.Ldftn, method);
					ilgenerator2.Emit(OpCodes.Newobj, typeFromHandle.GetConstructor(new Type[]
					{
						typeof(object),
						typeof(IntPtr)
					}));
					ilgenerator2.Emit(OpCodes.Ret);
					return (DelegateType)((object)dynamicMethodDefinition2.Generate().Invoke(null, new object[] { instance }));
				}
				return (DelegateType)((object)Activator.CreateInstance(typeFromHandle, new object[]
				{
					instance,
					method.MethodHandle.GetFunctionPointer()
				}));
			}
		}

		// Token: 0x060003AF RID: 943 RVA: 0x000143C4 File Offset: 0x000125C4
		[Obsolete("This overload only exists for runtime backwards compatibility and will be removed in Harmony 3. Use MethodDelegate(string, object, bool, Type[]) instead")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static DelegateType MethodDelegate<DelegateType>(string typeColonName, object instance, bool virtualCall) where DelegateType : Delegate
		{
			return AccessTools.MethodDelegate<DelegateType>(typeColonName, instance, virtualCall, null);
		}

		// Token: 0x060003B0 RID: 944 RVA: 0x000143CF File Offset: 0x000125CF
		public static DelegateType MethodDelegate<DelegateType>(string typeColonName, object instance = null, bool virtualCall = true, Type[] delegateArgs = null) where DelegateType : Delegate
		{
			return AccessTools.MethodDelegate<DelegateType>(AccessTools.DeclaredMethod(typeColonName, null, null), instance, virtualCall, delegateArgs);
		}

		// Token: 0x060003B1 RID: 945 RVA: 0x000143E4 File Offset: 0x000125E4
		public static DelegateType HarmonyDelegate<DelegateType>(object instance = null) where DelegateType : Delegate
		{
			HarmonyMethod mergedFromType = HarmonyMethodExtensions.GetMergedFromType(typeof(DelegateType));
			HarmonyMethod harmonyMethod = mergedFromType;
			MethodType methodType = harmonyMethod.methodType.GetValueOrDefault();
			if (harmonyMethod.methodType == null)
			{
				methodType = MethodType.Normal;
				harmonyMethod.methodType = new MethodType?(methodType);
			}
			MethodInfo methodInfo = mergedFromType.GetOriginalMethod() as MethodInfo;
			if (methodInfo == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(40, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Delegate ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(typeof(DelegateType));
				defaultInterpolatedStringHandler.AppendLiteral(" has no defined original method");
				throw new NullReferenceException(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			return AccessTools.MethodDelegate<DelegateType>(methodInfo, instance, !mergedFromType.nonVirtualDelegate, null);
		}

		// Token: 0x060003B2 RID: 946 RVA: 0x0001448C File Offset: 0x0001268C
		public static MethodBase GetOutsideCaller()
		{
			StackTrace stackTrace = new StackTrace(true);
			foreach (StackFrame stackFrame in stackTrace.GetFrames())
			{
				MethodBase method = stackFrame.GetMethod();
				Type declaringType = method.DeclaringType;
				if (((declaringType != null) ? declaringType.Namespace : null) != typeof(Harmony).Namespace)
				{
					return method;
				}
			}
			throw new Exception("Unexpected end of stack trace");
		}

		// Token: 0x060003B3 RID: 947 RVA: 0x000144F7 File Offset: 0x000126F7
		public static void RethrowException(Exception exception)
		{
			ExceptionDispatchInfo.Capture(exception).Throw();
			throw exception;
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060003B4 RID: 948 RVA: 0x00014505 File Offset: 0x00012705
		public static bool IsMonoRuntime { get; } = Type.GetType("Mono.Runtime") != null;

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060003B5 RID: 949 RVA: 0x0001450C File Offset: 0x0001270C
		public static bool IsNetFrameworkRuntime { get; }

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060003B6 RID: 950 RVA: 0x00014513 File Offset: 0x00012713
		public static bool IsNetCoreRuntime { get; }

		// Token: 0x060003B7 RID: 951 RVA: 0x0001451C File Offset: 0x0001271C
		public static void ThrowMissingMemberException(Type type, params string[] names)
		{
			string text = string.Join(",", AccessTools.GetFieldNames(type).ToArray());
			string text2 = string.Join(",", AccessTools.GetPropertyNames(type).ToArray());
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(44, 3);
			defaultInterpolatedStringHandler.AppendFormatted(string.Join(",", names));
			defaultInterpolatedStringHandler.AppendLiteral("; available fields: ");
			defaultInterpolatedStringHandler.AppendFormatted(text);
			defaultInterpolatedStringHandler.AppendLiteral("; available properties: ");
			defaultInterpolatedStringHandler.AppendFormatted(text2);
			throw new MissingMemberException(defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x060003B8 RID: 952 RVA: 0x000145A5 File Offset: 0x000127A5
		public static object GetDefaultValue(Type type)
		{
			if (type == null)
			{
				FileLog.Debug("AccessTools.GetDefaultValue: type is null");
				return null;
			}
			if (type == typeof(void))
			{
				return null;
			}
			if (type.IsValueType)
			{
				return Activator.CreateInstance(type);
			}
			return null;
		}

		// Token: 0x060003B9 RID: 953 RVA: 0x000145DC File Offset: 0x000127DC
		public static object CreateInstance(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			ConstructorInfo constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, CallingConventions.Any, Array.Empty<Type>(), null);
			if (constructor != null)
			{
				return constructor.Invoke(null);
			}
			return FormatterServices.GetUninitializedObject(type);
		}

		// Token: 0x060003BA RID: 954 RVA: 0x0001461C File Offset: 0x0001281C
		public static T CreateInstance<T>()
		{
			object obj = AccessTools.CreateInstance(typeof(T));
			if (obj is T)
			{
				return (T)((object)obj);
			}
			return default(T);
		}

		// Token: 0x060003BB RID: 955 RVA: 0x00014653 File Offset: 0x00012853
		public static T MakeDeepCopy<T>(object source) where T : class
		{
			return AccessTools.MakeDeepCopy(source, typeof(T), null, "") as T;
		}

		// Token: 0x060003BC RID: 956 RVA: 0x00014675 File Offset: 0x00012875
		public static void MakeDeepCopy<T>(object source, out T result, Func<string, Traverse, Traverse, object> processor = null, string pathRoot = "")
		{
			result = (T)((object)AccessTools.MakeDeepCopy(source, typeof(T), processor, pathRoot));
		}

		// Token: 0x060003BD RID: 957 RVA: 0x00014694 File Offset: 0x00012894
		public static object MakeDeepCopy(object source, Type resultType, Func<string, Traverse, Traverse, object> processor = null, string pathRoot = "")
		{
			if (source == null || resultType == null)
			{
				return null;
			}
			resultType = Nullable.GetUnderlyingType(resultType) ?? resultType;
			Type type = source.GetType();
			if (type.IsPrimitive)
			{
				return source;
			}
			if (type.IsEnum)
			{
				return Enum.ToObject(resultType, (int)source);
			}
			if (type.IsGenericType && resultType.IsGenericType)
			{
				AccessTools.addHandlerCacheLock.EnterUpgradeableReadLock();
				try
				{
					FastInvokeHandler handler;
					if (!AccessTools.addHandlerCache.TryGetValue(resultType, out handler))
					{
						MethodInfo methodInfo = AccessTools.FirstMethod(resultType, (MethodInfo m) => m.Name == "Add" && m.GetParameters().Length == 1);
						if (methodInfo != null)
						{
							handler = MethodInvoker.GetHandler(methodInfo, false);
						}
						AccessTools.addHandlerCacheLock.EnterWriteLock();
						try
						{
							AccessTools.addHandlerCache[resultType] = handler;
						}
						finally
						{
							AccessTools.addHandlerCacheLock.ExitWriteLock();
						}
					}
					if (handler != null)
					{
						object obj = Activator.CreateInstance(resultType);
						Type type2 = resultType.GetGenericArguments()[0];
						int num = 0;
						foreach (object obj2 in (source as IEnumerable))
						{
							string text = num++.ToString();
							string text2 = ((pathRoot.Length > 0) ? (pathRoot + "." + text) : text);
							object obj3 = AccessTools.MakeDeepCopy(obj2, type2, processor, text2);
							handler(obj, new object[] { obj3 });
						}
						return obj;
					}
				}
				finally
				{
					AccessTools.addHandlerCacheLock.ExitUpgradeableReadLock();
				}
			}
			if (type.IsArray && resultType.IsArray)
			{
				Type elementType = resultType.GetElementType();
				int length = ((Array)source).Length;
				object[] array = Activator.CreateInstance(resultType, new object[] { length }) as object[];
				object[] array2 = source as object[];
				for (int i = 0; i < length; i++)
				{
					string text3 = i.ToString();
					string text4 = ((pathRoot.Length > 0) ? (pathRoot + "." + text3) : text3);
					array[i] = AccessTools.MakeDeepCopy(array2[i], elementType, processor, text4);
				}
				return array;
			}
			string @namespace = type.Namespace;
			if (@namespace == "System" || (@namespace != null && @namespace.StartsWith("System.")))
			{
				return source;
			}
			object obj4 = AccessTools.CreateInstance((resultType == typeof(object)) ? type : resultType);
			Traverse.IterateFields(source, obj4, delegate(string name, Traverse src, Traverse dst)
			{
				string text5 = ((pathRoot.Length > 0) ? (pathRoot + "." + name) : name);
				object obj5 = ((processor != null) ? processor(text5, src, dst) : src.GetValue());
				if (dst.IsWriteable)
				{
					dst.SetValue(AccessTools.MakeDeepCopy(obj5, dst.GetValueType(), processor, text5));
				}
			});
			return obj4;
		}

		// Token: 0x060003BE RID: 958 RVA: 0x00014994 File Offset: 0x00012B94
		public static bool IsStruct(Type type)
		{
			return !(type == null) && (type.IsValueType && !AccessTools.IsValue(type)) && !AccessTools.IsVoid(type);
		}

		// Token: 0x060003BF RID: 959 RVA: 0x000149BC File Offset: 0x00012BBC
		public static bool IsClass(Type type)
		{
			return !(type == null) && !type.IsValueType;
		}

		// Token: 0x060003C0 RID: 960 RVA: 0x000149D2 File Offset: 0x00012BD2
		public static bool IsValue(Type type)
		{
			return !(type == null) && (type.IsPrimitive || type.IsEnum);
		}

		// Token: 0x060003C1 RID: 961 RVA: 0x000149F0 File Offset: 0x00012BF0
		public static bool IsInteger(Type type)
		{
			if (type == null)
			{
				return false;
			}
			TypeCode typeCode = Type.GetTypeCode(type);
			return typeCode - TypeCode.SByte <= 7;
		}

		// Token: 0x060003C2 RID: 962 RVA: 0x00014A1C File Offset: 0x00012C1C
		public static bool IsFloatingPoint(Type type)
		{
			if (type == null)
			{
				return false;
			}
			TypeCode typeCode = Type.GetTypeCode(type);
			return typeCode - TypeCode.Single <= 2;
		}

		// Token: 0x060003C3 RID: 963 RVA: 0x00014A49 File Offset: 0x00012C49
		public static bool IsNumber(Type type)
		{
			return AccessTools.IsInteger(type) || AccessTools.IsFloatingPoint(type);
		}

		// Token: 0x060003C4 RID: 964 RVA: 0x00014A5B File Offset: 0x00012C5B
		public static bool IsVoid(Type type)
		{
			return type == typeof(void);
		}

		// Token: 0x060003C5 RID: 965 RVA: 0x00014A6D File Offset: 0x00012C6D
		public static bool IsOfNullableType<T>(T instance)
		{
			return Nullable.GetUnderlyingType(typeof(T)) != null;
		}

		// Token: 0x060003C6 RID: 966 RVA: 0x00014A84 File Offset: 0x00012C84
		public static bool IsStatic(MemberInfo member)
		{
			if (member == null)
			{
				throw new ArgumentNullException("member");
			}
			MemberTypes memberType = member.MemberType;
			if (memberType <= MemberTypes.Method)
			{
				switch (memberType)
				{
				case MemberTypes.Constructor:
					break;
				case MemberTypes.Event:
					return AccessTools.IsStatic((EventInfo)member);
				case MemberTypes.Constructor | MemberTypes.Event:
					goto IL_0091;
				case MemberTypes.Field:
					return ((FieldInfo)member).IsStatic;
				default:
					if (memberType != MemberTypes.Method)
					{
						goto IL_0091;
					}
					break;
				}
				return ((MethodBase)member).IsStatic;
			}
			if (memberType == MemberTypes.Property)
			{
				return AccessTools.IsStatic((PropertyInfo)member);
			}
			if (memberType == MemberTypes.TypeInfo || memberType == MemberTypes.NestedType)
			{
				return AccessTools.IsStatic((Type)member);
			}
			IL_0091:
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(21, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Unknown member type: ");
			defaultInterpolatedStringHandler.AppendFormatted<MemberTypes>(member.MemberType);
			throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear());
		}

		// Token: 0x060003C7 RID: 967 RVA: 0x00014B53 File Offset: 0x00012D53
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static bool IsStatic(Type type)
		{
			return type != null && type.IsAbstract && type.IsSealed;
		}

		// Token: 0x060003C8 RID: 968 RVA: 0x00014B6A File Offset: 0x00012D6A
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static bool IsStatic(PropertyInfo propertyInfo)
		{
			if (propertyInfo == null)
			{
				throw new ArgumentNullException("propertyInfo");
			}
			return propertyInfo.GetAccessors(true)[0].IsStatic;
		}

		// Token: 0x060003C9 RID: 969 RVA: 0x00014B88 File Offset: 0x00012D88
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static bool IsStatic(EventInfo eventInfo)
		{
			if (eventInfo == null)
			{
				throw new ArgumentNullException("eventInfo");
			}
			return eventInfo.GetAddMethod(true).IsStatic;
		}

		// Token: 0x060003CA RID: 970 RVA: 0x00014BA4 File Offset: 0x00012DA4
		public static int CombinedHashCode(IEnumerable<object> objects)
		{
			int num = 352654597;
			int num2 = num;
			int num3 = 0;
			foreach (object obj in objects)
			{
				if (num3 % 2 == 0)
				{
					num = ((num << 5) + num + (num >> 27)) ^ obj.GetHashCode();
				}
				else
				{
					num2 = ((num2 << 5) + num2 + (num2 >> 27)) ^ obj.GetHashCode();
				}
				num3++;
			}
			return num + num2 * 1566083941;
		}

		// Token: 0x060003CB RID: 971 RVA: 0x00014C2C File Offset: 0x00012E2C
		// Note: this type is marked as 'beforefieldinit'.
		static AccessTools()
		{
			Type type = Type.GetType("System.Runtime.InteropServices.RuntimeInformation", false);
			AccessTools.IsNetFrameworkRuntime = ((type != null) ? type.GetProperty("FrameworkDescription").GetValue(null, null).ToString()
				.StartsWith(".NET Framework") : (!AccessTools.IsMonoRuntime));
			Type type2 = Type.GetType("System.Runtime.InteropServices.RuntimeInformation", false);
			AccessTools.IsNetCoreRuntime = type2 != null && type2.GetProperty("FrameworkDescription").GetValue(null, null).ToString()
				.StartsWith(".NET Core");
			AccessTools.addHandlerCache = new Dictionary<Type, FastInvokeHandler>();
			AccessTools.addHandlerCacheLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
		}

		// Token: 0x0400023C RID: 572
		private static Type[] allTypesCached = null;

		// Token: 0x0400023D RID: 573
		public static readonly BindingFlags all = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty;

		// Token: 0x0400023E RID: 574
		public static readonly BindingFlags allDeclared = AccessTools.all | BindingFlags.DeclaredOnly;

		// Token: 0x04000242 RID: 578
		private static readonly Dictionary<Type, FastInvokeHandler> addHandlerCache;

		// Token: 0x04000243 RID: 579
		private static readonly ReaderWriterLockSlim addHandlerCacheLock;

		// Token: 0x020000A8 RID: 168
		// (Invoke) Token: 0x060003CD RID: 973
		public delegate ref F FieldRef<in T, F>(T instance = default(T));

		// Token: 0x020000A9 RID: 169
		// (Invoke) Token: 0x060003D1 RID: 977
		public delegate ref F StructFieldRef<T, F>(ref T instance) where T : struct;

		// Token: 0x020000AA RID: 170
		// (Invoke) Token: 0x060003D5 RID: 981
		public delegate ref F FieldRef<F>();

		// Token: 0x020000AB RID: 171
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04000244 RID: 580
			public static Func<Assembly, IEnumerable<Type>> <0>__GetTypesFromAssembly;
		}
	}
}
