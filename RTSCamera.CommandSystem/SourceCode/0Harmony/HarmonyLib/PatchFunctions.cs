using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HarmonyLib
{
	// Token: 0x0200004A RID: 74
	internal static class PatchFunctions
	{
		// Token: 0x06000184 RID: 388 RVA: 0x0000BCA4 File Offset: 0x00009EA4
		internal static List<MethodInfo> GetSortedPatchMethods(MethodBase original, Patch[] patches, bool debug)
		{
			return (from p in new PatchSorter(patches, debug).Sort()
				select p.GetMethod(original)).ToList<MethodInfo>();
		}

		// Token: 0x06000185 RID: 389 RVA: 0x0000BCE0 File Offset: 0x00009EE0
		private static List<Infix> GetInfixes(Patch[] patches)
		{
			return patches.Select<Patch, Infix>((Patch p) => new Infix(p)).ToList<Infix>();
		}

		// Token: 0x06000186 RID: 390 RVA: 0x0000BD0C File Offset: 0x00009F0C
		internal static MethodInfo UpdateWrapper(MethodBase original, PatchInfo patchInfo)
		{
			bool flag = patchInfo.Debugging || Harmony.DEBUG;
			List<MethodInfo> sortedPatchMethods = PatchFunctions.GetSortedPatchMethods(original, patchInfo.prefixes, flag);
			List<MethodInfo> sortedPatchMethods2 = PatchFunctions.GetSortedPatchMethods(original, patchInfo.postfixes, flag);
			List<MethodInfo> sortedPatchMethods3 = PatchFunctions.GetSortedPatchMethods(original, patchInfo.transpilers, flag);
			List<MethodInfo> sortedPatchMethods4 = PatchFunctions.GetSortedPatchMethods(original, patchInfo.finalizers, flag);
			List<Infix> infixes = PatchFunctions.GetInfixes(patchInfo.innerprefixes);
			List<Infix> infixes2 = PatchFunctions.GetInfixes(patchInfo.innerpostfixes);
			MethodCreator methodCreator = new MethodCreator(new MethodCreatorConfig(original, null, sortedPatchMethods, sortedPatchMethods2, sortedPatchMethods3, sortedPatchMethods4, infixes, infixes2, flag));
			ValueTuple<MethodInfo, Dictionary<int, CodeInstruction>> valueTuple = methodCreator.CreateReplacement();
			MethodInfo item = valueTuple.Item1;
			Dictionary<int, CodeInstruction> item2 = valueTuple.Item2;
			if (item == null)
			{
				throw new MissingMethodException("Cannot create replacement for " + original.FullDescription());
			}
			try
			{
				PatchTools.DetourMethod(original, item);
			}
			catch (Exception ex)
			{
				throw HarmonyException.Create(ex, item2);
			}
			return item;
		}

		// Token: 0x06000187 RID: 391 RVA: 0x0000BDF0 File Offset: 0x00009FF0
		internal static MethodInfo ReversePatch(HarmonyMethod standin, MethodBase original, MethodInfo postTranspiler)
		{
			if (standin == null)
			{
				throw new ArgumentNullException("standin");
			}
			if (standin.method == null)
			{
				throw new ArgumentNullException("standin", "standin.method is NULL");
			}
			bool flag = standin.debug.GetValueOrDefault() || Harmony.DEBUG;
			List<MethodInfo> list = new List<MethodInfo>();
			if (standin.reversePatchType.GetValueOrDefault() == HarmonyReversePatchType.Snapshot)
			{
				Patches patchInfo = Harmony.GetPatchInfo(original);
				list.AddRange(PatchFunctions.GetSortedPatchMethods(original, patchInfo.Transpilers.ToArray<Patch>(), flag));
			}
			if (postTranspiler != null)
			{
				list.Add(postTranspiler);
			}
			List<MethodInfo> list2 = new List<MethodInfo>();
			List<Infix> list3 = new List<Infix>();
			MethodCreator methodCreator = new MethodCreator(new MethodCreatorConfig(standin.method, original, list2, list2, list, list2, list3, list3, flag));
			ValueTuple<MethodInfo, Dictionary<int, CodeInstruction>> valueTuple = methodCreator.CreateReplacement();
			MethodInfo item = valueTuple.Item1;
			Dictionary<int, CodeInstruction> item2 = valueTuple.Item2;
			if (item == null)
			{
				throw new MissingMethodException("Cannot create replacement for " + standin.method.FullDescription());
			}
			try
			{
				PatchTools.DetourMethod(standin.method, item);
			}
			catch (Exception ex)
			{
				throw HarmonyException.Create(ex, item2);
			}
			return item;
		}
	}
}
