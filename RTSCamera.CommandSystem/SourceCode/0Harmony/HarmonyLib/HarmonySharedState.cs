using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using MonoMod.Utils;

namespace HarmonyLib
{
	// Token: 0x0200001F RID: 31
	internal static class HarmonySharedState
	{
		// Token: 0x0600009C RID: 156 RVA: 0x00004A84 File Offset: 0x00002C84
		static HarmonySharedState()
		{
			Type orCreateSharedStateType = HarmonySharedState.GetOrCreateSharedStateType();
			if (AccessTools.IsMonoRuntime)
			{
				FieldInfo fieldInfo = AccessTools.Field(typeof(StackFrame), "methodAddress");
				if (fieldInfo != null)
				{
					HarmonySharedState.methodAddressRef = AccessTools.FieldRefAccess<StackFrame, long>(fieldInfo);
				}
			}
			FieldInfo field = orCreateSharedStateType.GetField("version");
			if ((int)field.GetValue(null) == 0)
			{
				field.SetValue(null, 102);
			}
			HarmonySharedState.actualVersion = (int)field.GetValue(null);
			FieldInfo field2 = orCreateSharedStateType.GetField("state");
			if (field2.GetValue(null) == null)
			{
				field2.SetValue(null, new Dictionary<MethodBase, byte[]>());
			}
			FieldInfo field3 = orCreateSharedStateType.GetField("originals");
			if (field3 != null && field3.GetValue(null) == null)
			{
				field3.SetValue(null, new Dictionary<MethodInfo, MethodBase>());
			}
			FieldInfo field4 = orCreateSharedStateType.GetField("originalsMono");
			if (field4 != null && field4.GetValue(null) == null)
			{
				field4.SetValue(null, new Dictionary<long, MethodBase[]>());
			}
			HarmonySharedState.state = (Dictionary<MethodBase, byte[]>)field2.GetValue(null);
			HarmonySharedState.originals = new Dictionary<MethodInfo, MethodBase>();
			if (field3 != null)
			{
				HarmonySharedState.originals = (Dictionary<MethodInfo, MethodBase>)field3.GetValue(null);
			}
			HarmonySharedState.originalsMono = new Dictionary<long, MethodBase[]>();
			if (field4 != null)
			{
				HarmonySharedState.originalsMono = (Dictionary<long, MethodBase[]>)field4.GetValue(null);
			}
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00004BD4 File Offset: 0x00002DD4
		private static Type GetOrCreateSharedStateType()
		{
			Type type = Type.GetType("HarmonySharedState", false);
			if (type != null)
			{
				return type;
			}
			Type type2;
			using (ModuleDefinition moduleDefinition = ModuleDefinition.CreateModule("HarmonySharedState", new ModuleParameters
			{
				Kind = ModuleKind.Dll,
				ReflectionImporterProvider = MMReflectionImporter.Provider
			}))
			{
				Mono.Cecil.TypeAttributes typeAttributes = Mono.Cecil.TypeAttributes.Public | Mono.Cecil.TypeAttributes.Abstract | Mono.Cecil.TypeAttributes.Sealed;
				TypeDefinition typeDefinition = new TypeDefinition("", "HarmonySharedState", typeAttributes)
				{
					BaseType = moduleDefinition.TypeSystem.Object
				};
				moduleDefinition.Types.Add(typeDefinition);
				typeDefinition.Fields.Add(new FieldDefinition("state", Mono.Cecil.FieldAttributes.FamANDAssem | Mono.Cecil.FieldAttributes.Family | Mono.Cecil.FieldAttributes.Static, moduleDefinition.ImportReference(typeof(Dictionary<MethodBase, byte[]>))));
				typeDefinition.Fields.Add(new FieldDefinition("originals", Mono.Cecil.FieldAttributes.FamANDAssem | Mono.Cecil.FieldAttributes.Family | Mono.Cecil.FieldAttributes.Static, moduleDefinition.ImportReference(typeof(Dictionary<MethodInfo, MethodBase>))));
				typeDefinition.Fields.Add(new FieldDefinition("originalsMono", Mono.Cecil.FieldAttributes.FamANDAssem | Mono.Cecil.FieldAttributes.Family | Mono.Cecil.FieldAttributes.Static, moduleDefinition.ImportReference(typeof(Dictionary<long, MethodBase[]>))));
				typeDefinition.Fields.Add(new FieldDefinition("version", Mono.Cecil.FieldAttributes.FamANDAssem | Mono.Cecil.FieldAttributes.Family | Mono.Cecil.FieldAttributes.Static, moduleDefinition.ImportReference(typeof(int))));
				type2 = ReflectionHelper.Load(moduleDefinition).GetType("HarmonySharedState");
			}
			return type2;
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00004D1C File Offset: 0x00002F1C
		internal static PatchInfo GetPatchInfo(MethodBase method)
		{
			Dictionary<MethodBase, byte[]> dictionary = HarmonySharedState.state;
			byte[] valueSafe;
			lock (dictionary)
			{
				valueSafe = HarmonySharedState.state.GetValueSafe(method);
			}
			if (valueSafe == null)
			{
				return null;
			}
			return PatchInfoSerialization.Deserialize(valueSafe);
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00004D6C File Offset: 0x00002F6C
		internal static IEnumerable<MethodBase> GetPatchedMethods()
		{
			Dictionary<MethodBase, byte[]> dictionary = HarmonySharedState.state;
			IEnumerable<MethodBase> enumerable;
			lock (dictionary)
			{
				enumerable = HarmonySharedState.state.Keys.ToArray<MethodBase>();
			}
			return enumerable;
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00004DB8 File Offset: 0x00002FB8
		internal static void UpdatePatchInfo(MethodBase original, MethodInfo replacement, PatchInfo patchInfo)
		{
			patchInfo.VersionCount++;
			byte[] array = patchInfo.Serialize();
			Dictionary<MethodBase, byte[]> dictionary = HarmonySharedState.state;
			lock (dictionary)
			{
				HarmonySharedState.state[original] = array;
			}
			Dictionary<MethodInfo, MethodBase> dictionary2 = HarmonySharedState.originals;
			lock (dictionary2)
			{
				HarmonySharedState.originals[replacement.Identifiable()] = original;
			}
			if (AccessTools.IsMonoRuntime)
			{
				long num = (long)replacement.MethodHandle.GetFunctionPointer();
				Dictionary<long, MethodBase[]> dictionary3 = HarmonySharedState.originalsMono;
				lock (dictionary3)
				{
					HarmonySharedState.originalsMono[num] = new MethodBase[] { original, replacement };
				}
			}
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00004EB4 File Offset: 0x000030B4
		internal static MethodBase GetRealMethod(MethodInfo method, bool useReplacement)
		{
			MethodInfo methodInfo = method.Identifiable();
			Dictionary<MethodInfo, MethodBase> dictionary = HarmonySharedState.originals;
			lock (dictionary)
			{
				MethodBase methodBase;
				if (HarmonySharedState.originals.TryGetValue(methodInfo, out methodBase))
				{
					return methodBase;
				}
			}
			if (AccessTools.IsMonoRuntime)
			{
				long num = (long)method.MethodHandle.GetFunctionPointer();
				Dictionary<long, MethodBase[]> dictionary2 = HarmonySharedState.originalsMono;
				lock (dictionary2)
				{
					MethodBase[] array;
					if (HarmonySharedState.originalsMono.TryGetValue(num, out array))
					{
						return useReplacement ? array[1] : array[0];
					}
				}
			}
			return method;
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00004F78 File Offset: 0x00003178
		internal unsafe static MethodBase GetStackFrameMethod(StackFrame frame, bool useReplacement)
		{
			MethodInfo methodInfo = frame.GetMethod() as MethodInfo;
			if (methodInfo != null)
			{
				return HarmonySharedState.GetRealMethod(methodInfo, useReplacement);
			}
			if (HarmonySharedState.methodAddressRef != null)
			{
				long num = *HarmonySharedState.methodAddressRef(frame);
				Dictionary<long, MethodBase[]> dictionary = HarmonySharedState.originalsMono;
				lock (dictionary)
				{
					MethodBase[] array;
					if (HarmonySharedState.originalsMono.TryGetValue(num, out array))
					{
						return useReplacement ? array[1] : array[0];
					}
				}
			}
			return null;
		}

		// Token: 0x0400004A RID: 74
		private const string name = "HarmonySharedState";

		// Token: 0x0400004B RID: 75
		internal const int internalVersion = 102;

		// Token: 0x0400004C RID: 76
		private static readonly Dictionary<MethodBase, byte[]> state;

		// Token: 0x0400004D RID: 77
		private static readonly Dictionary<MethodInfo, MethodBase> originals;

		// Token: 0x0400004E RID: 78
		private static readonly Dictionary<long, MethodBase[]> originalsMono;

		// Token: 0x0400004F RID: 79
		private static readonly AccessTools.FieldRef<StackFrame, long> methodAddressRef;

		// Token: 0x04000050 RID: 80
		internal static readonly int actualVersion;
	}
}
