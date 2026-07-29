using System;
using System.Runtime.CompilerServices;

namespace HarmonyLib.BUTR.Extensions
{
	// Token: 0x02000029 RID: 41
	[NullableContext(2)]
	[Nullable(0)]
	internal class Traverse2<T>
	{
		// Token: 0x17000039 RID: 57
		// (get) Token: 0x060001E4 RID: 484 RVA: 0x0000BF95 File Offset: 0x0000A195
		// (set) Token: 0x060001E5 RID: 485 RVA: 0x0000BFA2 File Offset: 0x0000A1A2
		public T Value
		{
			get
			{
				return this._traverse.GetValue<T>();
			}
			set
			{
				this._traverse.SetValue(value);
			}
		}

		// Token: 0x060001E6 RID: 486 RVA: 0x0000BFB6 File Offset: 0x0000A1B6
		private Traverse2()
		{
			this._traverse = new Traverse2(null);
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x0000BFCA File Offset: 0x0000A1CA
		[NullableContext(1)]
		public Traverse2(Traverse2 traverse)
		{
			this._traverse = traverse;
		}

		// Token: 0x040000A1 RID: 161
		[Nullable(1)]
		private readonly Traverse2 _traverse;
	}
}
