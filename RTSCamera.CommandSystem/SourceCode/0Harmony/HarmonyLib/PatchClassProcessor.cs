using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace HarmonyLib
{
	// Token: 0x02000091 RID: 145
	public class PatchClassProcessor
	{
		// Token: 0x1700001C RID: 28
		// (get) Token: 0x060002B2 RID: 690 RVA: 0x0000F7B0 File Offset: 0x0000D9B0
		// (set) Token: 0x060002B3 RID: 691 RVA: 0x0000F7B8 File Offset: 0x0000D9B8
		public string Category { get; set; }

		// Token: 0x060002B4 RID: 692 RVA: 0x0000F7C4 File Offset: 0x0000D9C4
		public PatchClassProcessor(Harmony instance, Type type)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			this.instance = instance;
			this.containerType = type;
			List<HarmonyMethod> fromType = HarmonyMethodExtensions.GetFromType(type);
			this.containerAttributes = HarmonyMethod.Merge(fromType);
			HarmonyMethod harmonyMethod = this.containerAttributes;
			MethodType methodType = harmonyMethod.methodType.GetValueOrDefault();
			if (harmonyMethod.methodType == null)
			{
				methodType = MethodType.Normal;
				harmonyMethod.methodType = new MethodType?(methodType);
			}
			this.Category = this.containerAttributes.category;
			this.auxilaryMethods = new Dictionary<Type, MethodInfo>();
			foreach (Type type2 in PatchClassProcessor.auxilaryTypes)
			{
				MethodInfo patchMethod = PatchTools.GetPatchMethod(this.containerType, type2.FullName);
				if (patchMethod != null)
				{
					this.auxilaryMethods[type2] = patchMethod;
				}
			}
			this.patchMethods = PatchTools.GetPatchMethods(this.containerType);
			foreach (AttributePatch attributePatch in this.patchMethods)
			{
				MethodInfo method = attributePatch.info.method;
				attributePatch.info = this.containerAttributes.Merge(attributePatch.info);
				attributePatch.info.method = method;
			}
		}

		// Token: 0x060002B5 RID: 693 RVA: 0x0000F948 File Offset: 0x0000DB48
		public List<MethodInfo> Patch()
		{
			Exception ex = null;
			if (!this.RunMethod<HarmonyPrepare, bool>(true, false, null, Array.Empty<object>()))
			{
				this.RunMethod<HarmonyCleanup>(ref ex, Array.Empty<object>());
				this.ReportException(ex, null);
				return new List<MethodInfo>();
			}
			List<MethodInfo> list = new List<MethodInfo>();
			MethodBase methodBase = null;
			try
			{
				List<MethodBase> bulkMethods = this.GetBulkMethods();
				if (bulkMethods.Count == 1)
				{
					methodBase = bulkMethods[0];
				}
				this.ReversePatch(ref methodBase);
				list = ((bulkMethods.Count > 0) ? this.BulkPatch(bulkMethods, ref methodBase, false) : this.PatchWithAttributes(ref methodBase, false));
			}
			catch (Exception ex2)
			{
				ex = ex2;
			}
			this.RunMethod<HarmonyCleanup>(ref ex, new object[] { ex });
			this.ReportException(ex, methodBase);
			return list;
		}

		// Token: 0x060002B6 RID: 694 RVA: 0x0000FA04 File Offset: 0x0000DC04
		public void Unpatch()
		{
			List<MethodBase> bulkMethods = this.GetBulkMethods();
			MethodBase methodBase = null;
			if (bulkMethods.Count > 0)
			{
				this.BulkPatch(bulkMethods, ref methodBase, true);
				return;
			}
			this.PatchWithAttributes(ref methodBase, true);
		}

		// Token: 0x060002B7 RID: 695 RVA: 0x0000FA3C File Offset: 0x0000DC3C
		private void ReversePatch(ref MethodBase lastOriginal)
		{
			for (int i = 0; i < this.patchMethods.Count; i++)
			{
				AttributePatch attributePatch = this.patchMethods[i];
				if (attributePatch.type.GetValueOrDefault() == HarmonyPatchType.ReversePatch)
				{
					MethodBase originalMethod = attributePatch.info.GetOriginalMethod();
					if (originalMethod != null)
					{
						lastOriginal = originalMethod;
					}
					ReversePatcher reversePatcher = this.instance.CreateReversePatcher(lastOriginal, attributePatch.info);
					object locker = PatchProcessor.locker;
					lock (locker)
					{
						reversePatcher.Patch(HarmonyReversePatchType.Original);
					}
				}
			}
		}

		// Token: 0x060002B8 RID: 696 RVA: 0x0000FADC File Offset: 0x0000DCDC
		private List<MethodInfo> BulkPatch(List<MethodBase> originals, ref MethodBase lastOriginal, bool unpatch)
		{
			PatchJobs<MethodInfo> patchJobs = new PatchJobs<MethodInfo>();
			for (int i = 0; i < originals.Count; i++)
			{
				lastOriginal = originals[i];
				PatchJobs<MethodInfo>.Job job = patchJobs.GetJob(lastOriginal);
				foreach (AttributePatch attributePatch in this.patchMethods)
				{
					string text = "You cannot combine TargetMethod, TargetMethods or [HarmonyPatchAll] with individual annotations";
					HarmonyMethod info = attributePatch.info;
					if (info.methodName != null)
					{
						throw new ArgumentException(text + " [" + info.methodName + "]");
					}
					if (info.methodType != null && info.methodType.Value != MethodType.Normal)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 2);
						defaultInterpolatedStringHandler.AppendFormatted(text);
						defaultInterpolatedStringHandler.AppendLiteral(" [");
						defaultInterpolatedStringHandler.AppendFormatted<MethodType?>(info.methodType);
						defaultInterpolatedStringHandler.AppendLiteral("]");
						throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear());
					}
					if (info.argumentTypes != null)
					{
						throw new ArgumentException(text + " [" + info.argumentTypes.Description() + "]");
					}
					job.AddPatch(attributePatch);
				}
			}
			foreach (PatchJobs<MethodInfo>.Job job2 in patchJobs.GetJobs())
			{
				lastOriginal = job2.original;
				if (unpatch)
				{
					this.ProcessUnpatchJob(job2);
				}
				else
				{
					this.ProcessPatchJob(job2);
				}
			}
			return patchJobs.GetReplacements();
		}

		// Token: 0x060002B9 RID: 697 RVA: 0x0000FC8C File Offset: 0x0000DE8C
		private List<MethodInfo> PatchWithAttributes(ref MethodBase lastOriginal, bool unpatch)
		{
			PatchJobs<MethodInfo> patchJobs = new PatchJobs<MethodInfo>();
			foreach (AttributePatch attributePatch in this.patchMethods)
			{
				lastOriginal = attributePatch.info.GetOriginalMethod();
				if (lastOriginal == null)
				{
					throw new ArgumentException("Undefined target method for patch method " + attributePatch.info.method.FullDescription());
				}
				PatchJobs<MethodInfo>.Job job = patchJobs.GetJob(lastOriginal);
				job.AddPatch(attributePatch);
			}
			foreach (PatchJobs<MethodInfo>.Job job2 in patchJobs.GetJobs())
			{
				lastOriginal = job2.original;
				if (unpatch)
				{
					this.ProcessUnpatchJob(job2);
				}
				else
				{
					this.ProcessPatchJob(job2);
				}
			}
			return patchJobs.GetReplacements();
		}

		// Token: 0x060002BA RID: 698 RVA: 0x0000FD80 File Offset: 0x0000DF80
		private void ProcessPatchJob(PatchJobs<MethodInfo>.Job job)
		{
			MethodInfo methodInfo = null;
			bool flag = this.RunMethod<HarmonyPrepare, bool>(true, false, null, new object[] { job.original });
			Exception ex = null;
			if (flag)
			{
				object locker = PatchProcessor.locker;
				lock (locker)
				{
					try
					{
						PatchInfo patchInfo = HarmonySharedState.GetPatchInfo(job.original) ?? new PatchInfo();
						patchInfo.AddPrefixes(this.instance.Id, job.prefixes.ToArray());
						patchInfo.AddPostfixes(this.instance.Id, job.postfixes.ToArray());
						patchInfo.AddTranspilers(this.instance.Id, job.transpilers.ToArray());
						patchInfo.AddFinalizers(this.instance.Id, job.finalizers.ToArray());
						patchInfo.AddInnerPrefixes(this.instance.Id, job.innerprefixes.ToArray());
						patchInfo.AddInnerPostfixes(this.instance.Id, job.innerpostfixes.ToArray());
						methodInfo = PatchFunctions.UpdateWrapper(job.original, patchInfo);
						HarmonySharedState.UpdatePatchInfo(job.original, methodInfo, patchInfo);
					}
					catch (Exception ex2)
					{
						ex = ex2;
					}
				}
			}
			this.RunMethod<HarmonyCleanup>(ref ex, new object[] { job.original, ex });
			this.ReportException(ex, job.original);
			job.replacement = methodInfo;
		}

		// Token: 0x060002BB RID: 699 RVA: 0x0000FF00 File Offset: 0x0000E100
		private void ProcessUnpatchJob(PatchJobs<MethodInfo>.Job job)
		{
			PatchInfo patchInfo = HarmonySharedState.GetPatchInfo(job.original) ?? new PatchInfo();
			bool flag = job.original.HasMethodBody();
			if (flag)
			{
				job.postfixes.Do<HarmonyMethod>(delegate(HarmonyMethod patch)
				{
					patchInfo.RemovePatch(patch.method);
				});
				job.prefixes.Do<HarmonyMethod>(delegate(HarmonyMethod patch)
				{
					patchInfo.RemovePatch(patch.method);
				});
			}
			job.transpilers.Do<HarmonyMethod>(delegate(HarmonyMethod patch)
			{
				patchInfo.RemovePatch(patch.method);
			});
			if (flag)
			{
				job.finalizers.Do<HarmonyMethod>(delegate(HarmonyMethod patch)
				{
					patchInfo.RemovePatch(patch.method);
				});
			}
			MethodInfo methodInfo = PatchFunctions.UpdateWrapper(job.original, patchInfo);
			HarmonySharedState.UpdatePatchInfo(job.original, methodInfo, patchInfo);
		}

		// Token: 0x060002BC RID: 700 RVA: 0x0000FFC0 File Offset: 0x0000E1C0
		private List<MethodBase> GetBulkMethods()
		{
			bool flag = this.containerType.GetCustomAttributes(true).Any<object>((object a) => a.GetType().FullName == PatchTools.harmonyPatchAllFullName);
			if (flag)
			{
				Type declaringType = this.containerAttributes.declaringType;
				if (declaringType == null)
				{
					throw new ArgumentException("Using " + PatchTools.harmonyPatchAllFullName + " requires an additional attribute for specifying the Class/Type");
				}
				List<MethodBase> list = new List<MethodBase>();
				list.AddRange(AccessTools.GetDeclaredConstructors(declaringType, null).Cast<MethodBase>());
				list.AddRange(AccessTools.GetDeclaredMethods(declaringType).Cast<MethodBase>());
				List<PropertyInfo> declaredProperties = AccessTools.GetDeclaredProperties(declaringType);
				list.AddRange((from prop in declaredProperties
					select prop.GetGetMethod(true) into method
					where method != null
					select method).Cast<MethodBase>());
				list.AddRange((from prop in declaredProperties
					select prop.GetSetMethod(true) into method
					where method != null
					select method).Cast<MethodBase>());
				return list;
			}
			else
			{
				List<MethodBase> list2 = new List<MethodBase>();
				IEnumerable<MethodBase> enumerable = this.RunMethod<HarmonyTargetMethods, IEnumerable<MethodBase>>(null, null, null, Array.Empty<object>());
				if (enumerable == null)
				{
					MethodBase methodBase = this.RunMethod<HarmonyTargetMethod, MethodBase>(null, null, delegate(MethodBase method)
					{
						if (method != null)
						{
							return null;
						}
						return "null";
					}, Array.Empty<object>());
					if (methodBase != null)
					{
						list2.Add(methodBase);
					}
					return list2;
				}
				string text = null;
				list2 = enumerable.ToList<MethodBase>();
				if (list2 == null)
				{
					text = "null";
				}
				else if (list2.Any<MethodBase>((MethodBase m) => m == null))
				{
					text = "some element was null";
				}
				if (text == null)
				{
					return list2;
				}
				MethodInfo methodInfo;
				if (this.auxilaryMethods.TryGetValue(typeof(HarmonyTargetMethods), out methodInfo))
				{
					throw new Exception("Method " + methodInfo.FullDescription() + " returned an unexpected result: " + text);
				}
				throw new Exception("Some method returned an unexpected result: " + text);
			}
		}

		// Token: 0x060002BD RID: 701 RVA: 0x00010204 File Offset: 0x0000E404
		private void ReportException(Exception exception, MethodBase original)
		{
			if (exception == null)
			{
				return;
			}
			if (this.containerAttributes.debug.GetValueOrDefault() || Harmony.DEBUG)
			{
				Version version;
				Harmony.VersionInfo(out version);
				FileLog.indentLevel = 0;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(37, 2);
				defaultInterpolatedStringHandler.AppendLiteral("### Exception from user \"");
				defaultInterpolatedStringHandler.AppendFormatted(this.instance.Id);
				defaultInterpolatedStringHandler.AppendLiteral("\", Harmony v");
				defaultInterpolatedStringHandler.AppendFormatted<Version>(version);
				FileLog.Log(defaultInterpolatedStringHandler.ToStringAndClear());
				FileLog.Log("### Original: " + (((original != null) ? original.FullDescription() : null) ?? "NULL"));
				FileLog.Log("### Patch class: " + this.containerType.FullDescription());
				Exception ex = exception;
				HarmonyException ex2 = ex as HarmonyException;
				if (ex2 != null)
				{
					ex = ex2.InnerException;
				}
				string text = ex.ToString();
				while (text.Contains("\n\n"))
				{
					text = text.Replace("\n\n", "\n");
				}
				text = text.Split(new char[] { '\n' }).Join<string>((string line) => "### " + line, "\n");
				FileLog.Log(text.Trim());
			}
			if (exception is HarmonyException)
			{
				throw exception;
			}
			throw new HarmonyException("Patching exception in method " + original.FullDescription(), exception);
		}

		// Token: 0x060002BE RID: 702 RVA: 0x00010368 File Offset: 0x0000E568
		private T RunMethod<S, T>(T defaultIfNotExisting, T defaultIfFailing, Func<T, string> failOnResult = null, params object[] parameters)
		{
			MethodInfo methodInfo;
			if (!this.auxilaryMethods.TryGetValue(typeof(S), out methodInfo))
			{
				return defaultIfNotExisting;
			}
			object[] array = (parameters ?? Array.Empty<object>()).Union<object>(new object[] { this.instance }).ToArray<object>();
			object[] array2 = AccessTools.ActualParameters(methodInfo, array);
			if (methodInfo.ReturnType != typeof(void) && !typeof(T).IsAssignableFrom(methodInfo.ReturnType))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(56, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Method ");
				defaultInterpolatedStringHandler.AppendFormatted(methodInfo.FullDescription());
				defaultInterpolatedStringHandler.AppendLiteral(" has wrong return type (should be assignable to ");
				defaultInterpolatedStringHandler.AppendFormatted(typeof(T).FullName);
				defaultInterpolatedStringHandler.AppendLiteral(")");
				throw new Exception(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			T t = defaultIfFailing;
			try
			{
				if (methodInfo.ReturnType == typeof(void))
				{
					methodInfo.Invoke(null, array2);
					t = defaultIfNotExisting;
				}
				else
				{
					t = (T)((object)methodInfo.Invoke(null, array2));
				}
				if (failOnResult != null)
				{
					string text = failOnResult(t);
					if (text != null)
					{
						throw new Exception("Method " + methodInfo.FullDescription() + " returned an unexpected result: " + text);
					}
				}
			}
			catch (Exception ex)
			{
				this.ReportException(ex, methodInfo);
			}
			return t;
		}

		// Token: 0x060002BF RID: 703 RVA: 0x000104D0 File Offset: 0x0000E6D0
		private void RunMethod<S>(ref Exception exception, params object[] parameters)
		{
			MethodInfo methodInfo;
			if (this.auxilaryMethods.TryGetValue(typeof(S), out methodInfo))
			{
				object[] array = (parameters ?? Array.Empty<object>()).Union<object>(new object[] { this.instance }).ToArray<object>();
				object[] array2 = AccessTools.ActualParameters(methodInfo, array);
				try
				{
					object obj = methodInfo.Invoke(null, array2);
					if (methodInfo.ReturnType == typeof(Exception))
					{
						exception = obj as Exception;
					}
				}
				catch (Exception ex)
				{
					this.ReportException(ex, methodInfo);
				}
			}
		}

		// Token: 0x040001D2 RID: 466
		private readonly Harmony instance;

		// Token: 0x040001D3 RID: 467
		private readonly Type containerType;

		// Token: 0x040001D4 RID: 468
		private readonly HarmonyMethod containerAttributes;

		// Token: 0x040001D5 RID: 469
		private readonly Dictionary<Type, MethodInfo> auxilaryMethods;

		// Token: 0x040001D6 RID: 470
		private readonly List<AttributePatch> patchMethods;

		// Token: 0x040001D7 RID: 471
		private static readonly List<Type> auxilaryTypes = new List<Type>(4)
		{
			typeof(HarmonyPrepare),
			typeof(HarmonyCleanup),
			typeof(HarmonyTargetMethod),
			typeof(HarmonyTargetMethods)
		};
	}
}
