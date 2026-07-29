using System;
using System.Linq;
using System.Reflection;

namespace HarmonyLib
{
	// Token: 0x0200008E RID: 142
	[Serializable]
	public class InnerMethod
	{
		// Token: 0x060002A2 RID: 674 RVA: 0x0000F438 File Offset: 0x0000D638
		public InnerMethod(MethodInfo method, params int[] positions)
		{
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			if (positions.Any<int>((int p) => p == 0))
			{
				throw new ArgumentException("positions cannot contain zeros");
			}
			this.Method = method;
			this.positions = positions;
		}

		// Token: 0x060002A3 RID: 675 RVA: 0x0000F49F File Offset: 0x0000D69F
		internal InnerMethod(int methodToken, string moduleGUID, int[] positions)
		{
			this.methodToken = methodToken;
			this.moduleGUID = moduleGUID;
			this.positions = positions;
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x060002A4 RID: 676 RVA: 0x0000F4BC File Offset: 0x0000D6BC
		// (set) Token: 0x060002A5 RID: 677 RVA: 0x0000F4E4 File Offset: 0x0000D6E4
		public MethodInfo Method
		{
			get
			{
				if (this.method == null)
				{
					this.method = AccessTools.GetMethodByModuleAndToken(this.moduleGUID, this.methodToken);
				}
				return this.method;
			}
			set
			{
				this.method = value;
				this.methodToken = this.method.MetadataToken;
				this.moduleGUID = this.method.Module.ModuleVersionId.ToString();
			}
		}

		// Token: 0x040001C2 RID: 450
		[NonSerialized]
		private MethodInfo method;

		// Token: 0x040001C3 RID: 451
		private int methodToken;

		// Token: 0x040001C4 RID: 452
		private string moduleGUID;

		// Token: 0x040001C5 RID: 453
		public int[] positions;
	}
}
