using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace HarmonyLib
{
	// Token: 0x02000094 RID: 148
	public class Patches
	{
		// Token: 0x1700001D RID: 29
		// (get) Token: 0x060002D0 RID: 720 RVA: 0x00010630 File Offset: 0x0000E830
		public ReadOnlyCollection<string> Owners
		{
			get
			{
				HashSet<string> hashSet = new HashSet<string>();
				hashSet.UnionWith(this.Prefixes.Select<Patch, string>((Patch p) => p.owner));
				hashSet.UnionWith(this.Postfixes.Select<Patch, string>((Patch p) => p.owner));
				hashSet.UnionWith(this.Transpilers.Select<Patch, string>((Patch p) => p.owner));
				hashSet.UnionWith(this.Finalizers.Select<Patch, string>((Patch p) => p.owner));
				hashSet.UnionWith(this.InnerPrefixes.Select<Patch, string>((Patch p) => p.owner));
				hashSet.UnionWith(this.InnerPostfixes.Select<Patch, string>((Patch p) => p.owner));
				return hashSet.ToList<string>().AsReadOnly();
			}
		}

		// Token: 0x060002D1 RID: 721 RVA: 0x00010770 File Offset: 0x0000E970
		public Patches(Patch[] prefixes, Patch[] postfixes, Patch[] transpilers, Patch[] finalizers, Patch[] innerprefixes, Patch[] innerpostfixes)
		{
			if (prefixes == null)
			{
				prefixes = Array.Empty<Patch>();
			}
			if (postfixes == null)
			{
				postfixes = Array.Empty<Patch>();
			}
			if (transpilers == null)
			{
				transpilers = Array.Empty<Patch>();
			}
			if (finalizers == null)
			{
				finalizers = Array.Empty<Patch>();
			}
			if (innerprefixes == null)
			{
				innerprefixes = Array.Empty<Patch>();
			}
			if (innerpostfixes == null)
			{
				innerpostfixes = Array.Empty<Patch>();
			}
			this.Prefixes = prefixes.ToList<Patch>().AsReadOnly();
			this.Postfixes = postfixes.ToList<Patch>().AsReadOnly();
			this.Transpilers = transpilers.ToList<Patch>().AsReadOnly();
			this.Finalizers = finalizers.ToList<Patch>().AsReadOnly();
			this.InnerPrefixes = innerprefixes.ToList<Patch>().AsReadOnly();
			this.InnerPostfixes = innerpostfixes.ToList<Patch>().AsReadOnly();
		}

		// Token: 0x040001E3 RID: 483
		public readonly ReadOnlyCollection<Patch> Prefixes;

		// Token: 0x040001E4 RID: 484
		public readonly ReadOnlyCollection<Patch> Postfixes;

		// Token: 0x040001E5 RID: 485
		public readonly ReadOnlyCollection<Patch> Transpilers;

		// Token: 0x040001E6 RID: 486
		public readonly ReadOnlyCollection<Patch> Finalizers;

		// Token: 0x040001E7 RID: 487
		public readonly ReadOnlyCollection<Patch> InnerPrefixes;

		// Token: 0x040001E8 RID: 488
		public readonly ReadOnlyCollection<Patch> InnerPostfixes;
	}
}
