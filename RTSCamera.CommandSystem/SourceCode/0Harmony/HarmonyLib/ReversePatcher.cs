using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HarmonyLib
{
	// Token: 0x0200009F RID: 159
	public class ReversePatcher
	{
		// Token: 0x0600032F RID: 815 RVA: 0x000115D9 File Offset: 0x0000F7D9
		public ReversePatcher(Harmony instance, MethodBase original, HarmonyMethod standin)
		{
			this.instance = instance;
			this.original = original;
			this.standin = standin;
		}

		// Token: 0x06000330 RID: 816 RVA: 0x000115F8 File Offset: 0x0000F7F8
		public MethodInfo Patch(HarmonyReversePatchType type = HarmonyReversePatchType.Original)
		{
			if (this.original == null)
			{
				throw new NullReferenceException("Null method for " + this.instance.Id);
			}
			this.standin.reversePatchType = new HarmonyReversePatchType?(type);
			MethodInfo transpiler = ReversePatcher.GetTranspiler(this.standin.method);
			return PatchFunctions.ReversePatch(this.standin, this.original, transpiler);
		}

		// Token: 0x06000331 RID: 817 RVA: 0x0001165C File Offset: 0x0000F85C
		internal static MethodInfo GetTranspiler(MethodInfo method)
		{
			string methodName = method.Name;
			Type declaringType = method.DeclaringType;
			List<MethodInfo> declaredMethods = AccessTools.GetDeclaredMethods(declaringType);
			Type ici = typeof(IEnumerable<CodeInstruction>);
			return declaredMethods.FirstOrDefault<MethodInfo>((MethodInfo m) => !(m.ReturnType != ici) && m.Name.StartsWith("<" + methodName + ">"));
		}

		// Token: 0x04000222 RID: 546
		private readonly Harmony instance;

		// Token: 0x04000223 RID: 547
		private readonly MethodBase original;

		// Token: 0x04000224 RID: 548
		private readonly HarmonyMethod standin;
	}
}
