using System;
using System.Runtime.CompilerServices;

namespace HarmonyLib.BUTR.Extensions
{
	// Token: 0x0200000F RID: 15
	[NullableContext(2)]
	[Nullable(0)]
	internal class Traverse2<T>
	{
		// Token: 0x1700000D RID: 13
		// (get) Token: 0x060000F3 RID: 243 RVA: 0x00009061 File Offset: 0x00007261
		// (set) Token: 0x060000F4 RID: 244 RVA: 0x0000906E File Offset: 0x0000726E
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

		// Token: 0x060000F5 RID: 245 RVA: 0x00009082 File Offset: 0x00007282
		private Traverse2()
		{
			this._traverse = new Traverse2(null);
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x00009098 File Offset: 0x00007298
		[NullableContext(1)]
		public Traverse2(Traverse2 traverse)
		{
			this._traverse = traverse;
		}

		// Token: 0x04000056 RID: 86
		[Nullable(1)]
		private readonly Traverse2 _traverse;
	}
}
