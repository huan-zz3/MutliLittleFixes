using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using MonoMod.Utils;

namespace HarmonyLib
{
	// Token: 0x0200009B RID: 155
	public class PatchProcessor
	{
		// Token: 0x06000303 RID: 771 RVA: 0x00010D65 File Offset: 0x0000EF65
		public PatchProcessor(Harmony instance, MethodBase original)
		{
			this.instance = instance;
			this.original = original;
		}

		// Token: 0x06000304 RID: 772 RVA: 0x00010D7B File Offset: 0x0000EF7B
		public PatchProcessor AddPrefix(HarmonyMethod prefix)
		{
			this.prefix = prefix;
			return this;
		}

		// Token: 0x06000305 RID: 773 RVA: 0x00010D85 File Offset: 0x0000EF85
		public PatchProcessor AddPrefix(MethodInfo fixMethod)
		{
			this.prefix = new HarmonyMethod(fixMethod);
			return this;
		}

		// Token: 0x06000306 RID: 774 RVA: 0x00010D94 File Offset: 0x0000EF94
		public PatchProcessor AddPostfix(HarmonyMethod postfix)
		{
			this.postfix = postfix;
			return this;
		}

		// Token: 0x06000307 RID: 775 RVA: 0x00010D9E File Offset: 0x0000EF9E
		public PatchProcessor AddPostfix(MethodInfo fixMethod)
		{
			this.postfix = new HarmonyMethod(fixMethod);
			return this;
		}

		// Token: 0x06000308 RID: 776 RVA: 0x00010DAD File Offset: 0x0000EFAD
		public PatchProcessor AddTranspiler(HarmonyMethod transpiler)
		{
			this.transpiler = transpiler;
			return this;
		}

		// Token: 0x06000309 RID: 777 RVA: 0x00010DB7 File Offset: 0x0000EFB7
		public PatchProcessor AddTranspiler(MethodInfo fixMethod)
		{
			this.transpiler = new HarmonyMethod(fixMethod);
			return this;
		}

		// Token: 0x0600030A RID: 778 RVA: 0x00010DC6 File Offset: 0x0000EFC6
		public PatchProcessor AddFinalizer(HarmonyMethod finalizer)
		{
			this.finalizer = finalizer;
			return this;
		}

		// Token: 0x0600030B RID: 779 RVA: 0x00010DD0 File Offset: 0x0000EFD0
		public PatchProcessor AddFinalizer(MethodInfo fixMethod)
		{
			this.finalizer = new HarmonyMethod(fixMethod);
			return this;
		}

		// Token: 0x0600030C RID: 780 RVA: 0x00010DDF File Offset: 0x0000EFDF
		public PatchProcessor AddInnerPrefix(HarmonyMethod innerPrefix)
		{
			this.innerprefix = innerPrefix;
			return this;
		}

		// Token: 0x0600030D RID: 781 RVA: 0x00010DE9 File Offset: 0x0000EFE9
		public PatchProcessor AddInnerPrefix(MethodInfo fixMethod)
		{
			this.innerprefix = new HarmonyMethod(fixMethod);
			return this;
		}

		// Token: 0x0600030E RID: 782 RVA: 0x00010DF8 File Offset: 0x0000EFF8
		public PatchProcessor AddInnerPostfix(HarmonyMethod innerPostfix)
		{
			this.innerpostfix = innerPostfix;
			return this;
		}

		// Token: 0x0600030F RID: 783 RVA: 0x00010E02 File Offset: 0x0000F002
		public PatchProcessor AddInnerPostfix(MethodInfo fixMethod)
		{
			this.innerpostfix = new HarmonyMethod(fixMethod);
			return this;
		}

		// Token: 0x06000310 RID: 784 RVA: 0x00010E14 File Offset: 0x0000F014
		public static IEnumerable<MethodBase> GetAllPatchedMethods()
		{
			object obj = PatchProcessor.locker;
			IEnumerable<MethodBase> patchedMethods;
			lock (obj)
			{
				patchedMethods = HarmonySharedState.GetPatchedMethods();
			}
			return patchedMethods;
		}

		// Token: 0x06000311 RID: 785 RVA: 0x00010E54 File Offset: 0x0000F054
		public MethodInfo Patch()
		{
			if (this.original == null)
			{
				throw new NullReferenceException("Null method for " + this.instance.Id);
			}
			if (!this.original.IsDeclaredMember<MethodBase>())
			{
				MethodBase declaredMember = this.original.GetDeclaredMember<MethodBase>();
				throw new ArgumentException("You can only patch implemented methods/constructors. Patch the declared method " + declaredMember.FullDescription() + " instead.");
			}
			object obj = PatchProcessor.locker;
			MethodInfo methodInfo2;
			lock (obj)
			{
				PatchInfo patchInfo = HarmonySharedState.GetPatchInfo(this.original) ?? new PatchInfo();
				patchInfo.AddPrefixes(this.instance.Id, new HarmonyMethod[] { this.prefix });
				patchInfo.AddPostfixes(this.instance.Id, new HarmonyMethod[] { this.postfix });
				patchInfo.AddTranspilers(this.instance.Id, new HarmonyMethod[] { this.transpiler });
				patchInfo.AddFinalizers(this.instance.Id, new HarmonyMethod[] { this.finalizer });
				patchInfo.AddInnerPrefixes(this.instance.Id, new HarmonyMethod[] { this.innerprefix });
				patchInfo.AddInnerPostfixes(this.instance.Id, new HarmonyMethod[] { this.innerpostfix });
				MethodInfo methodInfo = PatchFunctions.UpdateWrapper(this.original, patchInfo);
				HarmonySharedState.UpdatePatchInfo(this.original, methodInfo, patchInfo);
				methodInfo2 = methodInfo;
			}
			return methodInfo2;
		}

		// Token: 0x06000312 RID: 786 RVA: 0x00010FDC File Offset: 0x0000F1DC
		public PatchProcessor Unpatch(HarmonyPatchType type, string harmonyID)
		{
			if (this.original == null)
			{
				throw new NullReferenceException("Null method for " + this.instance.Id);
			}
			object obj = PatchProcessor.locker;
			lock (obj)
			{
				PatchInfo patchInfo = HarmonySharedState.GetPatchInfo(this.original);
				if (patchInfo == null)
				{
					patchInfo = new PatchInfo();
				}
				if (type == HarmonyPatchType.All || type == HarmonyPatchType.Prefix)
				{
					patchInfo.RemovePrefix(harmonyID);
				}
				if (type == HarmonyPatchType.All || type == HarmonyPatchType.Postfix)
				{
					patchInfo.RemovePostfix(harmonyID);
				}
				if (type == HarmonyPatchType.All || type == HarmonyPatchType.Transpiler)
				{
					patchInfo.RemoveTranspiler(harmonyID);
				}
				if (type == HarmonyPatchType.All || type == HarmonyPatchType.Finalizer)
				{
					patchInfo.RemoveFinalizer(harmonyID);
				}
				if (type == HarmonyPatchType.All || type == HarmonyPatchType.InnerPrefix)
				{
					patchInfo.RemoveInnerPrefix(harmonyID);
				}
				if (type == HarmonyPatchType.All || type == HarmonyPatchType.InnerPostfix)
				{
					patchInfo.RemoveInnerPostfix(harmonyID);
				}
				MethodInfo methodInfo = PatchFunctions.UpdateWrapper(this.original, patchInfo);
				HarmonySharedState.UpdatePatchInfo(this.original, methodInfo, patchInfo);
			}
			return this;
		}

		// Token: 0x06000313 RID: 787 RVA: 0x000110C0 File Offset: 0x0000F2C0
		public PatchProcessor Unpatch(MethodInfo patch)
		{
			if (this.original == null)
			{
				throw new NullReferenceException("Null method for " + this.instance.Id);
			}
			object obj = PatchProcessor.locker;
			lock (obj)
			{
				PatchInfo patchInfo = HarmonySharedState.GetPatchInfo(this.original);
				if (patchInfo == null)
				{
					patchInfo = new PatchInfo();
				}
				patchInfo.RemovePatch(patch);
				MethodInfo methodInfo = PatchFunctions.UpdateWrapper(this.original, patchInfo);
				HarmonySharedState.UpdatePatchInfo(this.original, methodInfo, patchInfo);
			}
			return this;
		}

		// Token: 0x06000314 RID: 788 RVA: 0x00011158 File Offset: 0x0000F358
		public static Patches GetPatchInfo(MethodBase method)
		{
			object obj = PatchProcessor.locker;
			PatchInfo patchInfo;
			lock (obj)
			{
				patchInfo = HarmonySharedState.GetPatchInfo(method);
			}
			if (patchInfo == null)
			{
				return null;
			}
			return new Patches(patchInfo.prefixes, patchInfo.postfixes, patchInfo.transpilers, patchInfo.finalizers, patchInfo.innerprefixes, patchInfo.innerpostfixes);
		}

		// Token: 0x06000315 RID: 789 RVA: 0x000111C8 File Offset: 0x0000F3C8
		public static List<MethodInfo> GetSortedPatchMethods(MethodBase original, Patch[] patches)
		{
			return PatchFunctions.GetSortedPatchMethods(original, patches, false);
		}

		// Token: 0x06000316 RID: 790 RVA: 0x000111D4 File Offset: 0x0000F3D4
		public static Dictionary<string, Version> VersionInfo(out Version currentVersion)
		{
			currentVersion = typeof(Harmony).Assembly.GetName().Version;
			Dictionary<string, Assembly> assemblies = new Dictionary<string, Assembly>();
			Action<Patch> <>9__2;
			Action<Patch> <>9__3;
			Action<Patch> <>9__4;
			Action<Patch> <>9__5;
			Action<Patch> <>9__6;
			Action<Patch> <>9__7;
			PatchProcessor.GetAllPatchedMethods().Do<MethodBase>(delegate(MethodBase method)
			{
				object obj = PatchProcessor.locker;
				PatchInfo patchInfo;
				lock (obj)
				{
					patchInfo = HarmonySharedState.GetPatchInfo(method);
				}
				IEnumerable<Patch> prefixes = patchInfo.prefixes;
				Action<Patch> action;
				if ((action = <>9__2) == null)
				{
					action = (<>9__2 = delegate(Patch fix)
					{
						assemblies[fix.owner] = fix.PatchMethod.DeclaringType.Assembly;
					});
				}
				prefixes.Do<Patch>(action);
				IEnumerable<Patch> postfixes = patchInfo.postfixes;
				Action<Patch> action2;
				if ((action2 = <>9__3) == null)
				{
					action2 = (<>9__3 = delegate(Patch fix)
					{
						assemblies[fix.owner] = fix.PatchMethod.DeclaringType.Assembly;
					});
				}
				postfixes.Do<Patch>(action2);
				IEnumerable<Patch> transpilers = patchInfo.transpilers;
				Action<Patch> action3;
				if ((action3 = <>9__4) == null)
				{
					action3 = (<>9__4 = delegate(Patch fix)
					{
						assemblies[fix.owner] = fix.PatchMethod.DeclaringType.Assembly;
					});
				}
				transpilers.Do<Patch>(action3);
				IEnumerable<Patch> finalizers = patchInfo.finalizers;
				Action<Patch> action4;
				if ((action4 = <>9__5) == null)
				{
					action4 = (<>9__5 = delegate(Patch fix)
					{
						assemblies[fix.owner] = fix.PatchMethod.DeclaringType.Assembly;
					});
				}
				finalizers.Do<Patch>(action4);
				IEnumerable<Patch> innerprefixes = patchInfo.innerprefixes;
				Action<Patch> action5;
				if ((action5 = <>9__6) == null)
				{
					action5 = (<>9__6 = delegate(Patch fix)
					{
						assemblies[fix.owner] = fix.PatchMethod.DeclaringType.Assembly;
					});
				}
				innerprefixes.Do<Patch>(action5);
				IEnumerable<Patch> innerpostfixes = patchInfo.innerpostfixes;
				Action<Patch> action6;
				if ((action6 = <>9__7) == null)
				{
					action6 = (<>9__7 = delegate(Patch fix)
					{
						assemblies[fix.owner] = fix.PatchMethod.DeclaringType.Assembly;
					});
				}
				innerpostfixes.Do<Patch>(action6);
			});
			Dictionary<string, Version> result = new Dictionary<string, Version>();
			assemblies.Do<KeyValuePair<string, Assembly>>(delegate(KeyValuePair<string, Assembly> info)
			{
				AssemblyName assemblyName = info.Value.GetReferencedAssemblies().FirstOrDefault<AssemblyName>((AssemblyName a) => a.FullName.StartsWith("0Harmony, Version", StringComparison.Ordinal));
				if (assemblyName != null)
				{
					result[info.Key] = assemblyName.Version;
				}
			});
			return result;
		}

		// Token: 0x06000317 RID: 791 RVA: 0x0001124C File Offset: 0x0000F44C
		public static ILGenerator CreateILGenerator()
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 1);
			defaultInterpolatedStringHandler.AppendLiteral("ILGenerator_");
			defaultInterpolatedStringHandler.AppendFormatted<Guid>(Guid.NewGuid());
			DynamicMethodDefinition dynamicMethodDefinition = new DynamicMethodDefinition(defaultInterpolatedStringHandler.ToStringAndClear(), typeof(void), Array.Empty<Type>());
			return dynamicMethodDefinition.GetILGenerator();
		}

		// Token: 0x06000318 RID: 792 RVA: 0x000112A0 File Offset: 0x0000F4A0
		public static ILGenerator CreateILGenerator(MethodBase original)
		{
			MethodInfo methodInfo = original as MethodInfo;
			Type type = ((methodInfo != null) ? methodInfo.ReturnType : typeof(void));
			List<Type> list = (from pi in original.GetParameters()
				select pi.ParameterType).ToList<Type>();
			if (!original.IsStatic)
			{
				list.Insert(0, original.DeclaringType);
			}
			DynamicMethodDefinition dynamicMethodDefinition = new DynamicMethodDefinition("ILGenerator_" + original.Name, type, list.ToArray());
			return dynamicMethodDefinition.GetILGenerator();
		}

		// Token: 0x06000319 RID: 793 RVA: 0x00011332 File Offset: 0x0000F532
		public static List<CodeInstruction> GetOriginalInstructions(MethodBase original, ILGenerator generator = null)
		{
			return MethodCopier.GetInstructions(generator ?? PatchProcessor.CreateILGenerator(original), original, 0);
		}

		// Token: 0x0600031A RID: 794 RVA: 0x00011346 File Offset: 0x0000F546
		public static List<CodeInstruction> GetOriginalInstructions(MethodBase original, out ILGenerator generator)
		{
			generator = PatchProcessor.CreateILGenerator(original);
			return MethodCopier.GetInstructions(generator, original, 0);
		}

		// Token: 0x0600031B RID: 795 RVA: 0x00011359 File Offset: 0x0000F559
		public static List<CodeInstruction> GetCurrentInstructions(MethodBase original, int maxTranspilers = 2147483647, ILGenerator generator = null)
		{
			return MethodCopier.GetInstructions(generator ?? PatchProcessor.CreateILGenerator(original), original, maxTranspilers);
		}

		// Token: 0x0600031C RID: 796 RVA: 0x0001136D File Offset: 0x0000F56D
		public static List<CodeInstruction> GetCurrentInstructions(MethodBase original, out ILGenerator generator, int maxTranspilers = 2147483647)
		{
			generator = PatchProcessor.CreateILGenerator(original);
			return MethodCopier.GetInstructions(generator, original, maxTranspilers);
		}

		// Token: 0x0600031D RID: 797 RVA: 0x00011380 File Offset: 0x0000F580
		public static IEnumerable<KeyValuePair<OpCode, object>> ReadMethodBody(MethodBase method)
		{
			return from instr in MethodBodyReader.GetInstructions(PatchProcessor.CreateILGenerator(method), method)
				select new KeyValuePair<OpCode, object>(instr.opcode, instr.operand);
		}

		// Token: 0x0600031E RID: 798 RVA: 0x000113B2 File Offset: 0x0000F5B2
		public static IEnumerable<KeyValuePair<OpCode, object>> ReadMethodBody(MethodBase method, ILGenerator generator)
		{
			return from instr in MethodBodyReader.GetInstructions(generator, method)
				select new KeyValuePair<OpCode, object>(instr.opcode, instr.operand);
		}

		// Token: 0x04000203 RID: 515
		private readonly Harmony instance;

		// Token: 0x04000204 RID: 516
		private readonly MethodBase original;

		// Token: 0x04000205 RID: 517
		private HarmonyMethod prefix;

		// Token: 0x04000206 RID: 518
		private HarmonyMethod postfix;

		// Token: 0x04000207 RID: 519
		private HarmonyMethod transpiler;

		// Token: 0x04000208 RID: 520
		private HarmonyMethod finalizer;

		// Token: 0x04000209 RID: 521
		private HarmonyMethod innerprefix;

		// Token: 0x0400020A RID: 522
		private HarmonyMethod innerpostfix;

		// Token: 0x0400020B RID: 523
		internal static readonly object locker = new object();
	}
}
