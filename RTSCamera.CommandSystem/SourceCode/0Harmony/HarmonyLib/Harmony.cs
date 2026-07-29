using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using MonoMod;

namespace HarmonyLib
{
	// Token: 0x0200007C RID: 124
	public class Harmony
	{
		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000237 RID: 567 RVA: 0x0000E249 File Offset: 0x0000C449
		// (set) Token: 0x06000238 RID: 568 RVA: 0x0000E251 File Offset: 0x0000C451
		public string Id { get; private set; }

		// Token: 0x06000239 RID: 569 RVA: 0x0000E25C File Offset: 0x0000C45C
		public Harmony(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				throw new ArgumentException("id cannot be null or empty");
			}
			try
			{
				string text = Environment.GetEnvironmentVariable("HARMONY_DEBUG");
				if (text != null && text.Length > 0)
				{
					text = text.Trim();
					Harmony.DEBUG = text == "1" || bool.Parse(text);
				}
			}
			catch
			{
			}
			if (Harmony.DEBUG)
			{
				Assembly assembly = typeof(Harmony).Assembly;
				Version version = assembly.GetName().Version;
				string text2 = assembly.Location;
				string text3 = Environment.Version.ToString();
				string text4 = Environment.OSVersion.Platform.ToString();
				if (string.IsNullOrEmpty(text2))
				{
					text2 = new Uri(assembly.CodeBase).LocalPath;
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(57, 5);
				defaultInterpolatedStringHandler.AppendLiteral("### Harmony id=");
				defaultInterpolatedStringHandler.AppendFormatted(id);
				defaultInterpolatedStringHandler.AppendLiteral(", version=");
				defaultInterpolatedStringHandler.AppendFormatted<Version>(version);
				defaultInterpolatedStringHandler.AppendLiteral(", location=");
				defaultInterpolatedStringHandler.AppendFormatted(text2);
				defaultInterpolatedStringHandler.AppendLiteral(", env/clr=");
				defaultInterpolatedStringHandler.AppendFormatted(text3);
				defaultInterpolatedStringHandler.AppendLiteral(", platform=");
				defaultInterpolatedStringHandler.AppendFormatted(text4);
				FileLog.Log(defaultInterpolatedStringHandler.ToStringAndClear());
				MethodBase outsideCaller = AccessTools.GetOutsideCaller();
				if (outsideCaller.DeclaringType != null)
				{
					Assembly assembly2 = outsideCaller.DeclaringType.Assembly;
					text2 = assembly2.Location;
					if (string.IsNullOrEmpty(text2))
					{
						text2 = new Uri(assembly2.CodeBase).LocalPath;
					}
					FileLog.Log("### Started from " + outsideCaller.FullDescription() + ", location " + text2);
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(7, 1);
					defaultInterpolatedStringHandler2.AppendLiteral("### At ");
					defaultInterpolatedStringHandler2.AppendFormatted<DateTime>(DateTime.Now, "yyyy-MM-dd hh.mm.ss");
					FileLog.Log(defaultInterpolatedStringHandler2.ToStringAndClear());
				}
			}
			this.Id = id;
		}

		// Token: 0x0600023A RID: 570 RVA: 0x0000E450 File Offset: 0x0000C650
		public void PatchAll()
		{
			MethodBase method = new StackTrace().GetFrame(1).GetMethod();
			Assembly assembly = method.ReflectedType.Assembly;
			this.PatchAll(assembly);
		}

		// Token: 0x0600023B RID: 571 RVA: 0x0000E481 File Offset: 0x0000C681
		public PatchProcessor CreateProcessor(MethodBase original)
		{
			return new PatchProcessor(this, original);
		}

		// Token: 0x0600023C RID: 572 RVA: 0x0000E48A File Offset: 0x0000C68A
		public PatchClassProcessor CreateClassProcessor(Type type)
		{
			return new PatchClassProcessor(this, type);
		}

		// Token: 0x0600023D RID: 573 RVA: 0x0000E493 File Offset: 0x0000C693
		public ReversePatcher CreateReversePatcher(MethodBase original, HarmonyMethod standin)
		{
			return new ReversePatcher(this, original, standin);
		}

		// Token: 0x0600023E RID: 574 RVA: 0x0000E49D File Offset: 0x0000C69D
		public void PatchAll(Assembly assembly)
		{
			AccessTools.GetTypesFromAssembly(assembly).DoIf<Type>((Type type) => type.HasHarmonyAttribute(), delegate(Type type)
			{
				this.CreateClassProcessor(type).Patch();
			});
		}

		// Token: 0x0600023F RID: 575 RVA: 0x0000E4D8 File Offset: 0x0000C6D8
		public void PatchAllUncategorized()
		{
			MethodBase method = new StackTrace().GetFrame(1).GetMethod();
			Assembly assembly = method.ReflectedType.Assembly;
			this.PatchAllUncategorized(assembly);
		}

		// Token: 0x06000240 RID: 576 RVA: 0x0000E50C File Offset: 0x0000C70C
		public void PatchAllUncategorized(Assembly assembly)
		{
			PatchClassProcessor[] array = (from type in AccessTools.GetTypesFromAssembly(assembly)
				where type.HasHarmonyAttribute()
				select type).Select<Type, PatchClassProcessor>(new Func<Type, PatchClassProcessor>(this.CreateClassProcessor)).ToArray<PatchClassProcessor>();
			array.DoIf<PatchClassProcessor>((PatchClassProcessor patchClass) => string.IsNullOrEmpty(patchClass.Category), delegate(PatchClassProcessor patchClass)
			{
				patchClass.Patch();
			});
		}

		// Token: 0x06000241 RID: 577 RVA: 0x0000E5A0 File Offset: 0x0000C7A0
		public void PatchCategory(string category)
		{
			MethodBase method = new StackTrace().GetFrame(1).GetMethod();
			Assembly assembly = method.ReflectedType.Assembly;
			this.PatchCategory(assembly, category);
		}

		// Token: 0x06000242 RID: 578 RVA: 0x0000E5D4 File Offset: 0x0000C7D4
		public void PatchCategory(Assembly assembly, string category)
		{
			ConditionalWeakTable<Assembly, Dictionary<string, List<Type>>> assemblyCachedCategories = Harmony.AssemblyCachedCategories;
			ConditionalWeakTable<Assembly, Dictionary<string, List<Type>>>.CreateValueCallback createValueCallback;
			if ((createValueCallback = Harmony.<>O.<0>__BuildCategoryCache) == null)
			{
				createValueCallback = (Harmony.<>O.<0>__BuildCategoryCache = new ConditionalWeakTable<Assembly, Dictionary<string, List<Type>>>.CreateValueCallback(Harmony.BuildCategoryCache));
			}
			Dictionary<string, List<Type>> value = assemblyCachedCategories.GetValue(assembly, createValueCallback);
			List<Type> list;
			if (value.TryGetValue(category, out list))
			{
				list.Do<Type>(delegate(Type type)
				{
					this.CreateClassProcessor(type).Patch();
				});
			}
		}

		// Token: 0x06000243 RID: 579 RVA: 0x0000E628 File Offset: 0x0000C828
		private static Dictionary<string, List<Type>> BuildCategoryCache(Assembly assembly)
		{
			Dictionary<string, List<Type>> dictionary = new Dictionary<string, List<Type>>();
			foreach (Type type in AccessTools.GetTypesFromAssembly(assembly))
			{
				List<HarmonyMethod> fromType = HarmonyMethodExtensions.GetFromType(type);
				if (fromType.Count != 0)
				{
					HarmonyMethod harmonyMethod = HarmonyMethod.Merge(fromType);
					string category = harmonyMethod.category;
					if (!string.IsNullOrEmpty(category))
					{
						List<Type> list;
						if (!dictionary.TryGetValue(category, out list) && list == null)
						{
							list = new List<Type>();
						}
						list.Add(type);
						dictionary[category] = list;
					}
				}
			}
			return dictionary;
		}

		// Token: 0x06000244 RID: 580 RVA: 0x0000E6AC File Offset: 0x0000C8AC
		public MethodInfo Patch(MethodBase original, HarmonyMethod prefix = null, HarmonyMethod postfix = null, HarmonyMethod transpiler = null, HarmonyMethod finalizer = null)
		{
			PatchProcessor patchProcessor = this.CreateProcessor(original);
			patchProcessor.AddPrefix(prefix);
			patchProcessor.AddPostfix(postfix);
			patchProcessor.AddTranspiler(transpiler);
			patchProcessor.AddFinalizer(finalizer);
			return patchProcessor.Patch();
		}

		// Token: 0x06000245 RID: 581 RVA: 0x0000E6E9 File Offset: 0x0000C8E9
		public static MethodInfo ReversePatch(MethodBase original, HarmonyMethod standin, MethodInfo transpiler = null)
		{
			return PatchFunctions.ReversePatch(standin, original, transpiler);
		}

		// Token: 0x06000246 RID: 582 RVA: 0x0000E6F4 File Offset: 0x0000C8F4
		public void UnpatchAll(string harmonyID = null)
		{
			Harmony.<>c__DisplayClass19_0 CS$<>8__locals1 = new Harmony.<>c__DisplayClass19_0();
			CS$<>8__locals1.harmonyID = harmonyID;
			CS$<>8__locals1.<>4__this = this;
			List<MethodBase> list = Harmony.GetAllPatchedMethods().ToList<MethodBase>();
			using (List<MethodBase>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MethodBase original = enumerator.Current;
					bool flag = original.HasMethodBody();
					Patches patchInfo2 = Harmony.GetPatchInfo(original);
					if (flag)
					{
						patchInfo2.Postfixes.DoIf<Patch>(new Func<Patch, bool>(CS$<>8__locals1.<UnpatchAll>g__IDCheck|0), delegate(Patch patchInfo)
						{
							CS$<>8__locals1.<>4__this.Unpatch(original, patchInfo.PatchMethod);
						});
						patchInfo2.Prefixes.DoIf<Patch>(new Func<Patch, bool>(CS$<>8__locals1.<UnpatchAll>g__IDCheck|0), delegate(Patch patchInfo)
						{
							CS$<>8__locals1.<>4__this.Unpatch(original, patchInfo.PatchMethod);
						});
						patchInfo2.InnerPostfixes.DoIf<Patch>(new Func<Patch, bool>(CS$<>8__locals1.<UnpatchAll>g__IDCheck|0), delegate(Patch patchInfo)
						{
							CS$<>8__locals1.<>4__this.Unpatch(original, patchInfo.PatchMethod);
						});
						patchInfo2.InnerPrefixes.DoIf<Patch>(new Func<Patch, bool>(CS$<>8__locals1.<UnpatchAll>g__IDCheck|0), delegate(Patch patchInfo)
						{
							CS$<>8__locals1.<>4__this.Unpatch(original, patchInfo.PatchMethod);
						});
					}
					patchInfo2.Transpilers.DoIf<Patch>(new Func<Patch, bool>(CS$<>8__locals1.<UnpatchAll>g__IDCheck|0), delegate(Patch patchInfo)
					{
						CS$<>8__locals1.<>4__this.Unpatch(original, patchInfo.PatchMethod);
					});
					if (flag)
					{
						patchInfo2.Finalizers.DoIf<Patch>(new Func<Patch, bool>(CS$<>8__locals1.<UnpatchAll>g__IDCheck|0), delegate(Patch patchInfo)
						{
							CS$<>8__locals1.<>4__this.Unpatch(original, patchInfo.PatchMethod);
						});
					}
				}
			}
		}

		// Token: 0x06000247 RID: 583 RVA: 0x0000E89C File Offset: 0x0000CA9C
		public void Unpatch(MethodBase original, HarmonyPatchType type, string harmonyID = "*")
		{
			PatchProcessor patchProcessor = this.CreateProcessor(original);
			patchProcessor.Unpatch(type, harmonyID);
		}

		// Token: 0x06000248 RID: 584 RVA: 0x0000E8BC File Offset: 0x0000CABC
		public void Unpatch(MethodBase original, MethodInfo patch)
		{
			PatchProcessor patchProcessor = this.CreateProcessor(original);
			patchProcessor.Unpatch(patch);
		}

		// Token: 0x06000249 RID: 585 RVA: 0x0000E8DC File Offset: 0x0000CADC
		public void UnpatchCategory(string category)
		{
			MethodBase method = new StackTrace().GetFrame(1).GetMethod();
			Assembly assembly = method.ReflectedType.Assembly;
			this.UnpatchCategory(assembly, category);
		}

		// Token: 0x0600024A RID: 586 RVA: 0x0000E910 File Offset: 0x0000CB10
		public void UnpatchCategory(Assembly assembly, string category)
		{
			ConditionalWeakTable<Assembly, Dictionary<string, List<Type>>> assemblyCachedCategories = Harmony.AssemblyCachedCategories;
			ConditionalWeakTable<Assembly, Dictionary<string, List<Type>>>.CreateValueCallback createValueCallback;
			if ((createValueCallback = Harmony.<>O.<0>__BuildCategoryCache) == null)
			{
				createValueCallback = (Harmony.<>O.<0>__BuildCategoryCache = new ConditionalWeakTable<Assembly, Dictionary<string, List<Type>>>.CreateValueCallback(Harmony.BuildCategoryCache));
			}
			Dictionary<string, List<Type>> value = assemblyCachedCategories.GetValue(assembly, createValueCallback);
			List<Type> list;
			if (value.TryGetValue(category, out list))
			{
				list.Do<Type>(delegate(Type type)
				{
					this.CreateClassProcessor(type).Unpatch();
				});
			}
		}

		// Token: 0x0600024B RID: 587 RVA: 0x0000E964 File Offset: 0x0000CB64
		public static bool HasAnyPatches(string harmonyID)
		{
			IEnumerable<MethodBase> allPatchedMethods = Harmony.GetAllPatchedMethods();
			Func<MethodBase, Patches> func;
			if ((func = Harmony.<>O.<1>__GetPatchInfo) == null)
			{
				func = (Harmony.<>O.<1>__GetPatchInfo = new Func<MethodBase, Patches>(Harmony.GetPatchInfo));
			}
			return allPatchedMethods.Select<MethodBase, Patches>(func).Any<Patches>((Patches info) => info.Owners.Contains(harmonyID));
		}

		// Token: 0x0600024C RID: 588 RVA: 0x0000E9B4 File Offset: 0x0000CBB4
		public static Patches GetPatchInfo(MethodBase method)
		{
			return PatchProcessor.GetPatchInfo(method);
		}

		// Token: 0x0600024D RID: 589 RVA: 0x0000E9BC File Offset: 0x0000CBBC
		public IEnumerable<MethodBase> GetPatchedMethods()
		{
			return from original in Harmony.GetAllPatchedMethods()
				where Harmony.GetPatchInfo(original).Owners.Contains(this.Id)
				select original;
		}

		// Token: 0x0600024E RID: 590 RVA: 0x0000E9D4 File Offset: 0x0000CBD4
		public static IEnumerable<MethodBase> GetAllPatchedMethods()
		{
			return PatchProcessor.GetAllPatchedMethods();
		}

		// Token: 0x0600024F RID: 591 RVA: 0x0000E9DB File Offset: 0x0000CBDB
		public static MethodBase GetOriginalMethod(MethodInfo replacement)
		{
			if (replacement == null)
			{
				throw new ArgumentNullException("replacement");
			}
			return HarmonySharedState.GetRealMethod(replacement, false);
		}

		// Token: 0x06000250 RID: 592 RVA: 0x0000E9F8 File Offset: 0x0000CBF8
		public static MethodBase GetMethodFromStackframe(StackFrame frame)
		{
			if (frame == null)
			{
				throw new ArgumentNullException("frame");
			}
			return HarmonySharedState.GetStackFrameMethod(frame, true);
		}

		// Token: 0x06000251 RID: 593 RVA: 0x0000EA0F File Offset: 0x0000CC0F
		public static MethodBase GetOriginalMethodFromStackframe(StackFrame frame)
		{
			if (frame == null)
			{
				throw new ArgumentNullException("frame");
			}
			return HarmonySharedState.GetStackFrameMethod(frame, false);
		}

		// Token: 0x06000252 RID: 594 RVA: 0x0000EA26 File Offset: 0x0000CC26
		public static Dictionary<string, Version> VersionInfo(out Version currentVersion)
		{
			return PatchProcessor.VersionInfo(out currentVersion);
		}

		// Token: 0x06000253 RID: 595 RVA: 0x0000EA2E File Offset: 0x0000CC2E
		public static void SetSwitch(string name, object value)
		{
			Switches.SetSwitchValue(name, value);
		}

		// Token: 0x06000254 RID: 596 RVA: 0x0000EA37 File Offset: 0x0000CC37
		public static void ClearSwitch(string name)
		{
			Switches.ClearSwitchValue(name);
		}

		// Token: 0x06000255 RID: 597 RVA: 0x0000EA3F File Offset: 0x0000CC3F
		public static bool TryGetSwitch(string name, out object value)
		{
			return Switches.TryGetSwitchValue(name, out value);
		}

		// Token: 0x06000256 RID: 598 RVA: 0x0000EA48 File Offset: 0x0000CC48
		public static bool TryIsSwitchEnabled(string name, out bool isEnabled)
		{
			return Switches.TryGetSwitchEnabled(name, out isEnabled);
		}

		// Token: 0x04000192 RID: 402
		public static bool DEBUG;

		// Token: 0x04000193 RID: 403
		private static readonly ConditionalWeakTable<Assembly, Dictionary<string, List<Type>>> AssemblyCachedCategories = new ConditionalWeakTable<Assembly, Dictionary<string, List<Type>>>();

		// Token: 0x0200007D RID: 125
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04000194 RID: 404
			public static ConditionalWeakTable<Assembly, Dictionary<string, List<Type>>>.CreateValueCallback <0>__BuildCategoryCache;

			// Token: 0x04000195 RID: 405
			public static Func<MethodBase, Patches> <1>__GetPatchInfo;
		}
	}
}
