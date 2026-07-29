using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace HarmonyLib
{
	// Token: 0x02000096 RID: 150
	[Serializable]
	public class PatchInfo
	{
		// Token: 0x1700001E RID: 30
		// (get) Token: 0x060002DA RID: 730 RVA: 0x00010840 File Offset: 0x0000EA40
		public bool Debugging
		{
			get
			{
				if (!this.prefixes.Any<Patch>((Patch p) => p.debug))
				{
					if (!this.postfixes.Any<Patch>((Patch p) => p.debug))
					{
						if (!this.transpilers.Any<Patch>((Patch p) => p.debug))
						{
							if (!this.finalizers.Any<Patch>((Patch p) => p.debug))
							{
								if (!this.innerprefixes.Any<Patch>((Patch p) => p.debug))
								{
									return this.innerpostfixes.Any<Patch>((Patch p) => p.debug);
								}
							}
						}
					}
				}
				return true;
			}
		}

		// Token: 0x060002DB RID: 731 RVA: 0x0001095E File Offset: 0x0000EB5E
		internal void AddPrefixes(string owner, params HarmonyMethod[] methods)
		{
			this.prefixes = PatchInfo.Add(owner, methods, this.prefixes);
		}

		// Token: 0x060002DC RID: 732 RVA: 0x00010974 File Offset: 0x0000EB74
		[Obsolete("This method only exists for backwards compatibility since the class is public.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void AddPrefix(MethodInfo patch, string owner, int priority, string[] before, string[] after, bool debug)
		{
			this.AddPrefixes(owner, new HarmonyMethod[]
			{
				new HarmonyMethod(patch, priority, before, after, new bool?(debug))
			});
		}

		// Token: 0x060002DD RID: 733 RVA: 0x000109A3 File Offset: 0x0000EBA3
		public void RemovePrefix(string owner)
		{
			this.prefixes = PatchInfo.Remove(owner, this.prefixes);
		}

		// Token: 0x060002DE RID: 734 RVA: 0x000109B7 File Offset: 0x0000EBB7
		internal void AddPostfixes(string owner, params HarmonyMethod[] methods)
		{
			this.postfixes = PatchInfo.Add(owner, methods, this.postfixes);
		}

		// Token: 0x060002DF RID: 735 RVA: 0x000109CC File Offset: 0x0000EBCC
		[Obsolete("This method only exists for backwards compatibility since the class is public.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void AddPostfix(MethodInfo patch, string owner, int priority, string[] before, string[] after, bool debug)
		{
			this.AddPostfixes(owner, new HarmonyMethod[]
			{
				new HarmonyMethod(patch, priority, before, after, new bool?(debug))
			});
		}

		// Token: 0x060002E0 RID: 736 RVA: 0x000109FB File Offset: 0x0000EBFB
		public void RemovePostfix(string owner)
		{
			this.postfixes = PatchInfo.Remove(owner, this.postfixes);
		}

		// Token: 0x060002E1 RID: 737 RVA: 0x00010A0F File Offset: 0x0000EC0F
		internal void AddTranspilers(string owner, params HarmonyMethod[] methods)
		{
			this.transpilers = PatchInfo.Add(owner, methods, this.transpilers);
		}

		// Token: 0x060002E2 RID: 738 RVA: 0x00010A24 File Offset: 0x0000EC24
		[Obsolete("This method only exists for backwards compatibility since the class is public.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void AddTranspiler(MethodInfo patch, string owner, int priority, string[] before, string[] after, bool debug)
		{
			this.AddTranspilers(owner, new HarmonyMethod[]
			{
				new HarmonyMethod(patch, priority, before, after, new bool?(debug))
			});
		}

		// Token: 0x060002E3 RID: 739 RVA: 0x00010A53 File Offset: 0x0000EC53
		public void RemoveTranspiler(string owner)
		{
			this.transpilers = PatchInfo.Remove(owner, this.transpilers);
		}

		// Token: 0x060002E4 RID: 740 RVA: 0x00010A67 File Offset: 0x0000EC67
		internal void AddFinalizers(string owner, params HarmonyMethod[] methods)
		{
			this.finalizers = PatchInfo.Add(owner, methods, this.finalizers);
		}

		// Token: 0x060002E5 RID: 741 RVA: 0x00010A7C File Offset: 0x0000EC7C
		[Obsolete("This method only exists for backwards compatibility since the class is public.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void AddFinalizer(MethodInfo patch, string owner, int priority, string[] before, string[] after, bool debug)
		{
			this.AddFinalizers(owner, new HarmonyMethod[]
			{
				new HarmonyMethod(patch, priority, before, after, new bool?(debug))
			});
		}

		// Token: 0x060002E6 RID: 742 RVA: 0x00010AAB File Offset: 0x0000ECAB
		public void RemoveFinalizer(string owner)
		{
			this.finalizers = PatchInfo.Remove(owner, this.finalizers);
		}

		// Token: 0x060002E7 RID: 743 RVA: 0x00010ABF File Offset: 0x0000ECBF
		internal void AddInnerPrefixes(string owner, params HarmonyMethod[] methods)
		{
			this.innerprefixes = PatchInfo.Add(owner, methods, this.innerprefixes);
		}

		// Token: 0x060002E8 RID: 744 RVA: 0x00010AD4 File Offset: 0x0000ECD4
		public void RemoveInnerPrefix(string owner)
		{
			this.innerprefixes = PatchInfo.Remove(owner, this.innerprefixes);
		}

		// Token: 0x060002E9 RID: 745 RVA: 0x00010AE8 File Offset: 0x0000ECE8
		internal void AddInnerPostfixes(string owner, params HarmonyMethod[] methods)
		{
			this.innerpostfixes = PatchInfo.Add(owner, methods, this.innerpostfixes);
		}

		// Token: 0x060002EA RID: 746 RVA: 0x00010AFD File Offset: 0x0000ECFD
		public void RemoveInnerPostfix(string owner)
		{
			this.innerpostfixes = PatchInfo.Remove(owner, this.innerpostfixes);
		}

		// Token: 0x060002EB RID: 747 RVA: 0x00010B14 File Offset: 0x0000ED14
		public void RemovePatch(MethodInfo patch)
		{
			this.prefixes = this.prefixes.Where<Patch>((Patch p) => p.PatchMethod != patch).ToArray<Patch>();
			this.postfixes = this.postfixes.Where<Patch>((Patch p) => p.PatchMethod != patch).ToArray<Patch>();
			this.transpilers = this.transpilers.Where<Patch>((Patch p) => p.PatchMethod != patch).ToArray<Patch>();
			this.finalizers = this.finalizers.Where<Patch>((Patch p) => p.PatchMethod != patch).ToArray<Patch>();
			this.innerprefixes = this.innerprefixes.Where<Patch>((Patch p) => p.PatchMethod != patch).ToArray<Patch>();
			this.innerpostfixes = this.innerpostfixes.Where<Patch>((Patch p) => p.PatchMethod != patch).ToArray<Patch>();
		}

		// Token: 0x060002EC RID: 748 RVA: 0x00010BFC File Offset: 0x0000EDFC
		private static Patch[] Add(string owner, HarmonyMethod[] add, Patch[] current)
		{
			if (add.Length == 0)
			{
				return current;
			}
			int initialIndex = current.Length;
			List<Patch> list = new List<Patch>();
			list.AddRange(current);
			list.AddRange(add.Where<HarmonyMethod>((HarmonyMethod method) => method != null).Select<HarmonyMethod, Patch>((HarmonyMethod method, int i) => new Patch(method, i + initialIndex, owner)));
			return list.ToArray();
		}

		// Token: 0x060002ED RID: 749 RVA: 0x00010C74 File Offset: 0x0000EE74
		private static Patch[] Remove(string owner, Patch[] current)
		{
			if (!(owner == "*"))
			{
				return current.Where<Patch>((Patch patch) => patch.owner != owner).ToArray<Patch>();
			}
			return Array.Empty<Patch>();
		}

		// Token: 0x040001F0 RID: 496
		public Patch[] prefixes = Array.Empty<Patch>();

		// Token: 0x040001F1 RID: 497
		public Patch[] postfixes = Array.Empty<Patch>();

		// Token: 0x040001F2 RID: 498
		public Patch[] transpilers = Array.Empty<Patch>();

		// Token: 0x040001F3 RID: 499
		public Patch[] finalizers = Array.Empty<Patch>();

		// Token: 0x040001F4 RID: 500
		public Patch[] innerprefixes = Array.Empty<Patch>();

		// Token: 0x040001F5 RID: 501
		public Patch[] innerpostfixes = Array.Empty<Patch>();

		// Token: 0x040001F6 RID: 502
		public int VersionCount;
	}
}
