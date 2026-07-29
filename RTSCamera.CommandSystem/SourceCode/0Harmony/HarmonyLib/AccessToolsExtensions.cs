using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace HarmonyLib
{
	// Token: 0x020000C3 RID: 195
	public static class AccessToolsExtensions
	{
		// Token: 0x0600041C RID: 1052 RVA: 0x000150F9 File Offset: 0x000132F9
		public static IEnumerable<Type> InnerTypes(this Type type)
		{
			return AccessTools.InnerTypes(type);
		}

		// Token: 0x0600041D RID: 1053 RVA: 0x00015101 File Offset: 0x00013301
		public static T FindIncludingBaseTypes<T>(this Type type, Func<Type, T> func) where T : class
		{
			return AccessTools.FindIncludingBaseTypes<T>(type, func);
		}

		// Token: 0x0600041E RID: 1054 RVA: 0x0001510A File Offset: 0x0001330A
		public static T FindIncludingInnerTypes<T>(this Type type, Func<Type, T> func) where T : class
		{
			return AccessTools.FindIncludingInnerTypes<T>(type, func);
		}

		// Token: 0x0600041F RID: 1055 RVA: 0x00015113 File Offset: 0x00013313
		public static FieldInfo DeclaredField(this Type type, string name)
		{
			return AccessTools.DeclaredField(type, name);
		}

		// Token: 0x06000420 RID: 1056 RVA: 0x0001511C File Offset: 0x0001331C
		public static FieldInfo Field(this Type type, string name)
		{
			return AccessTools.Field(type, name);
		}

		// Token: 0x06000421 RID: 1057 RVA: 0x00015125 File Offset: 0x00013325
		public static FieldInfo DeclaredField(this Type type, int idx)
		{
			return AccessTools.DeclaredField(type, idx);
		}

		// Token: 0x06000422 RID: 1058 RVA: 0x0001512E File Offset: 0x0001332E
		public static PropertyInfo DeclaredProperty(this Type type, string name)
		{
			return AccessTools.DeclaredProperty(type, name);
		}

		// Token: 0x06000423 RID: 1059 RVA: 0x00015137 File Offset: 0x00013337
		public static PropertyInfo DeclaredIndexer(this Type type, Type[] parameters = null)
		{
			return AccessTools.DeclaredIndexer(type, parameters);
		}

		// Token: 0x06000424 RID: 1060 RVA: 0x00015140 File Offset: 0x00013340
		public static MethodInfo DeclaredPropertyGetter(this Type type, string name)
		{
			return AccessTools.DeclaredPropertyGetter(type, name);
		}

		// Token: 0x06000425 RID: 1061 RVA: 0x00015149 File Offset: 0x00013349
		public static MethodInfo DeclaredIndexerGetter(this Type type, Type[] parameters = null)
		{
			return AccessTools.DeclaredIndexerGetter(type, parameters);
		}

		// Token: 0x06000426 RID: 1062 RVA: 0x00015152 File Offset: 0x00013352
		public static MethodInfo DeclaredPropertySetter(this Type type, string name)
		{
			return AccessTools.DeclaredPropertySetter(type, name);
		}

		// Token: 0x06000427 RID: 1063 RVA: 0x0001515B File Offset: 0x0001335B
		public static MethodInfo DeclaredIndexerSetter(this Type type, Type[] parameters)
		{
			return AccessTools.DeclaredIndexerSetter(type, parameters);
		}

		// Token: 0x06000428 RID: 1064 RVA: 0x00015164 File Offset: 0x00013364
		public static PropertyInfo Property(this Type type, string name)
		{
			return AccessTools.Property(type, name);
		}

		// Token: 0x06000429 RID: 1065 RVA: 0x0001516D File Offset: 0x0001336D
		public static PropertyInfo Indexer(this Type type, Type[] parameters = null)
		{
			return AccessTools.Indexer(type, parameters);
		}

		// Token: 0x0600042A RID: 1066 RVA: 0x00015176 File Offset: 0x00013376
		public static MethodInfo PropertyGetter(this Type type, string name)
		{
			return AccessTools.PropertyGetter(type, name);
		}

		// Token: 0x0600042B RID: 1067 RVA: 0x0001517F File Offset: 0x0001337F
		public static MethodInfo IndexerGetter(this Type type, Type[] parameters = null)
		{
			return AccessTools.IndexerGetter(type, parameters);
		}

		// Token: 0x0600042C RID: 1068 RVA: 0x00015188 File Offset: 0x00013388
		public static MethodInfo PropertySetter(this Type type, string name)
		{
			return AccessTools.PropertySetter(type, name);
		}

		// Token: 0x0600042D RID: 1069 RVA: 0x00015191 File Offset: 0x00013391
		public static MethodInfo IndexerSetter(this Type type, Type[] parameters = null)
		{
			return AccessTools.IndexerSetter(type, parameters);
		}

		// Token: 0x0600042E RID: 1070 RVA: 0x0001519A File Offset: 0x0001339A
		public static EventInfo DeclaredEvent(this Type type, string name)
		{
			return AccessTools.DeclaredEvent(type, name);
		}

		// Token: 0x0600042F RID: 1071 RVA: 0x000151A3 File Offset: 0x000133A3
		public static EventInfo Event(this Type type, string name)
		{
			return AccessTools.Event(type, name);
		}

		// Token: 0x06000430 RID: 1072 RVA: 0x000151AC File Offset: 0x000133AC
		public static MethodInfo DeclaredEventAdder(this Type type, string name)
		{
			return AccessTools.DeclaredEventAdder(type, name);
		}

		// Token: 0x06000431 RID: 1073 RVA: 0x000151B5 File Offset: 0x000133B5
		public static MethodInfo EventAdder(this Type type, string name)
		{
			return AccessTools.EventAdder(type, name);
		}

		// Token: 0x06000432 RID: 1074 RVA: 0x000151BE File Offset: 0x000133BE
		public static MethodInfo DeclaredEventRemover(this Type type, string name)
		{
			return AccessTools.DeclaredEventRemover(type, name);
		}

		// Token: 0x06000433 RID: 1075 RVA: 0x000151C7 File Offset: 0x000133C7
		public static MethodInfo EventRemover(this Type type, string name)
		{
			return AccessTools.EventRemover(type, name);
		}

		// Token: 0x06000434 RID: 1076 RVA: 0x000151D0 File Offset: 0x000133D0
		public static MethodInfo Finalizer(this Type type)
		{
			return AccessTools.Finalizer(type);
		}

		// Token: 0x06000435 RID: 1077 RVA: 0x000151D8 File Offset: 0x000133D8
		public static MethodInfo DeclaredFinalizer(this Type type)
		{
			return AccessTools.DeclaredFinalizer(type);
		}

		// Token: 0x06000436 RID: 1078 RVA: 0x000151E0 File Offset: 0x000133E0
		public static MethodInfo DeclaredMethod(this Type type, string name, Type[] parameters = null, Type[] generics = null)
		{
			return AccessTools.DeclaredMethod(type, name, parameters, generics);
		}

		// Token: 0x06000437 RID: 1079 RVA: 0x000151EB File Offset: 0x000133EB
		public static MethodInfo Method(this Type type, string name, Type[] parameters = null, Type[] generics = null)
		{
			return AccessTools.Method(type, name, parameters, generics);
		}

		// Token: 0x06000438 RID: 1080 RVA: 0x000151F6 File Offset: 0x000133F6
		public static List<string> GetMethodNames(this Type type)
		{
			return AccessTools.GetMethodNames(type);
		}

		// Token: 0x06000439 RID: 1081 RVA: 0x000151FE File Offset: 0x000133FE
		public static List<string> GetFieldNames(this Type type)
		{
			return AccessTools.GetFieldNames(type);
		}

		// Token: 0x0600043A RID: 1082 RVA: 0x00015206 File Offset: 0x00013406
		public static List<string> GetPropertyNames(this Type type)
		{
			return AccessTools.GetPropertyNames(type);
		}

		// Token: 0x0600043B RID: 1083 RVA: 0x0001520E File Offset: 0x0001340E
		public static ConstructorInfo DeclaredConstructor(this Type type, Type[] parameters = null, bool searchForStatic = false)
		{
			return AccessTools.DeclaredConstructor(type, parameters, searchForStatic);
		}

		// Token: 0x0600043C RID: 1084 RVA: 0x00015218 File Offset: 0x00013418
		public static ConstructorInfo Constructor(this Type type, Type[] parameters = null, bool searchForStatic = false)
		{
			return AccessTools.Constructor(type, parameters, searchForStatic);
		}

		// Token: 0x0600043D RID: 1085 RVA: 0x00015222 File Offset: 0x00013422
		public static List<ConstructorInfo> GetDeclaredConstructors(this Type type, bool? searchForStatic = null)
		{
			return AccessTools.GetDeclaredConstructors(type, searchForStatic);
		}

		// Token: 0x0600043E RID: 1086 RVA: 0x0001522B File Offset: 0x0001342B
		public static List<MethodInfo> GetDeclaredMethods(this Type type)
		{
			return AccessTools.GetDeclaredMethods(type);
		}

		// Token: 0x0600043F RID: 1087 RVA: 0x00015233 File Offset: 0x00013433
		public static List<PropertyInfo> GetDeclaredProperties(this Type type)
		{
			return AccessTools.GetDeclaredProperties(type);
		}

		// Token: 0x06000440 RID: 1088 RVA: 0x0001523B File Offset: 0x0001343B
		public static List<FieldInfo> GetDeclaredFields(this Type type)
		{
			return AccessTools.GetDeclaredFields(type);
		}

		// Token: 0x06000441 RID: 1089 RVA: 0x00015243 File Offset: 0x00013443
		public static Type Inner(this Type type, string name)
		{
			return AccessTools.Inner(type, name);
		}

		// Token: 0x06000442 RID: 1090 RVA: 0x0001524C File Offset: 0x0001344C
		public static Type FirstInner(this Type type, Func<Type, bool> predicate)
		{
			return AccessTools.FirstInner(type, predicate);
		}

		// Token: 0x06000443 RID: 1091 RVA: 0x00015255 File Offset: 0x00013455
		public static MethodInfo FirstMethod(this Type type, Func<MethodInfo, bool> predicate)
		{
			return AccessTools.FirstMethod(type, predicate);
		}

		// Token: 0x06000444 RID: 1092 RVA: 0x0001525E File Offset: 0x0001345E
		public static ConstructorInfo FirstConstructor(this Type type, Func<ConstructorInfo, bool> predicate)
		{
			return AccessTools.FirstConstructor(type, predicate);
		}

		// Token: 0x06000445 RID: 1093 RVA: 0x00015267 File Offset: 0x00013467
		public static PropertyInfo FirstProperty(this Type type, Func<PropertyInfo, bool> predicate)
		{
			return AccessTools.FirstProperty(type, predicate);
		}

		// Token: 0x06000446 RID: 1094 RVA: 0x00015270 File Offset: 0x00013470
		public static AccessTools.FieldRef<object, F> FieldRefAccess<F>(this Type type, string fieldName)
		{
			return AccessTools.FieldRefAccess<F>(type, fieldName);
		}

		// Token: 0x06000447 RID: 1095 RVA: 0x00015279 File Offset: 0x00013479
		public static ref F StaticFieldRefAccess<F>(this Type type, string fieldName)
		{
			return AccessTools.StaticFieldRefAccess<F>(type, fieldName);
		}

		// Token: 0x06000448 RID: 1096 RVA: 0x00015282 File Offset: 0x00013482
		public static void ThrowMissingMemberException(this Type type, params string[] names)
		{
			AccessTools.ThrowMissingMemberException(type, names);
		}

		// Token: 0x06000449 RID: 1097 RVA: 0x0001528B File Offset: 0x0001348B
		public static object GetDefaultValue(this Type type)
		{
			return AccessTools.GetDefaultValue(type);
		}

		// Token: 0x0600044A RID: 1098 RVA: 0x00015293 File Offset: 0x00013493
		public static object CreateInstance(this Type type)
		{
			return AccessTools.CreateInstance(type);
		}

		// Token: 0x0600044B RID: 1099 RVA: 0x0001529B File Offset: 0x0001349B
		public static bool IsStruct(this Type type)
		{
			return AccessTools.IsStruct(type);
		}

		// Token: 0x0600044C RID: 1100 RVA: 0x000152A3 File Offset: 0x000134A3
		public static bool IsClass(this Type type)
		{
			return AccessTools.IsClass(type);
		}

		// Token: 0x0600044D RID: 1101 RVA: 0x000152AB File Offset: 0x000134AB
		public static bool IsValue(this Type type)
		{
			return AccessTools.IsValue(type);
		}

		// Token: 0x0600044E RID: 1102 RVA: 0x000152B3 File Offset: 0x000134B3
		public static bool IsInteger(this Type type)
		{
			return AccessTools.IsInteger(type);
		}

		// Token: 0x0600044F RID: 1103 RVA: 0x000152BB File Offset: 0x000134BB
		public static bool IsFloatingPoint(this Type type)
		{
			return AccessTools.IsFloatingPoint(type);
		}

		// Token: 0x06000450 RID: 1104 RVA: 0x000152C3 File Offset: 0x000134C3
		public static bool IsNumber(this Type type)
		{
			return AccessTools.IsNumber(type);
		}

		// Token: 0x06000451 RID: 1105 RVA: 0x000152CB File Offset: 0x000134CB
		public static bool IsVoid(this Type type)
		{
			return AccessTools.IsVoid(type);
		}

		// Token: 0x06000452 RID: 1106 RVA: 0x000152D3 File Offset: 0x000134D3
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static bool IsStatic(this Type type)
		{
			return AccessTools.IsStatic(type);
		}
	}
}
