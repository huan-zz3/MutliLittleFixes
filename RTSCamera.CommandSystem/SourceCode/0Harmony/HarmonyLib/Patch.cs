using System;
using System.Reflection;
using System.Reflection.Emit;

namespace HarmonyLib
{
	// Token: 0x02000090 RID: 144
	[Serializable]
	public class Patch : IComparable
	{
		// Token: 0x1700001B RID: 27
		// (get) Token: 0x060002A9 RID: 681 RVA: 0x0000F53F File Offset: 0x0000D73F
		// (set) Token: 0x060002AA RID: 682 RVA: 0x0000F568 File Offset: 0x0000D768
		public MethodInfo PatchMethod
		{
			get
			{
				if (this.patchMethod == null)
				{
					this.patchMethod = AccessTools.GetMethodByModuleAndToken(this.moduleGUID, this.methodToken);
				}
				return this.patchMethod;
			}
			set
			{
				this.patchMethod = value;
				this.methodToken = this.patchMethod.MetadataToken;
				this.moduleGUID = this.patchMethod.Module.ModuleVersionId.ToString();
			}
		}

		// Token: 0x060002AB RID: 683 RVA: 0x0000F5B4 File Offset: 0x0000D7B4
		public Patch(MethodInfo patch, int index, string owner, int priority, string[] before, string[] after, bool debug)
		{
			if (patch is DynamicMethod)
			{
				throw new Exception("Cannot directly reference dynamic method \"" + patch.FullDescription() + "\" in Harmony. Use a factory method instead that will return the dynamic method.");
			}
			this.index = index;
			this.owner = owner;
			this.priority = ((priority == -1) ? 400 : priority);
			this.before = before ?? Array.Empty<string>();
			this.after = after ?? Array.Empty<string>();
			this.debug = debug;
			this.PatchMethod = patch;
		}

		// Token: 0x060002AC RID: 684 RVA: 0x0000F63D File Offset: 0x0000D83D
		public Patch(HarmonyMethod method, int index, string owner)
			: this(method.method, index, owner, method.priority, method.before, method.after, method.debug.GetValueOrDefault())
		{
		}

		// Token: 0x060002AD RID: 685 RVA: 0x0000F66C File Offset: 0x0000D86C
		internal Patch(int index, string owner, int priority, string[] before, string[] after, bool debug, int methodToken, string moduleGUID)
		{
			this.index = index;
			this.owner = owner;
			this.priority = ((priority == -1) ? 400 : priority);
			this.before = before ?? Array.Empty<string>();
			this.after = after ?? Array.Empty<string>();
			this.debug = debug;
			this.methodToken = methodToken;
			this.moduleGUID = moduleGUID;
		}

		// Token: 0x060002AE RID: 686 RVA: 0x0000F6DC File Offset: 0x0000D8DC
		public MethodInfo GetMethod(MethodBase original)
		{
			MethodInfo methodInfo = this.PatchMethod;
			if (methodInfo.ReturnType != typeof(DynamicMethod) && methodInfo.ReturnType != typeof(MethodInfo))
			{
				return methodInfo;
			}
			if (!methodInfo.IsStatic)
			{
				return methodInfo;
			}
			ParameterInfo[] parameters = methodInfo.GetParameters();
			if (parameters.Length != 1)
			{
				return methodInfo;
			}
			if (parameters[0].ParameterType != typeof(MethodBase))
			{
				return methodInfo;
			}
			return methodInfo.Invoke(null, new object[] { original }) as MethodInfo;
		}

		// Token: 0x060002AF RID: 687 RVA: 0x0000F76A File Offset: 0x0000D96A
		public override bool Equals(object obj)
		{
			return obj != null && obj is Patch && this.PatchMethod == ((Patch)obj).PatchMethod;
		}

		// Token: 0x060002B0 RID: 688 RVA: 0x0000F78F File Offset: 0x0000D98F
		public int CompareTo(object obj)
		{
			return PatchInfoSerialization.PriorityComparer(obj, this.index, this.priority);
		}

		// Token: 0x060002B1 RID: 689 RVA: 0x0000F7A3 File Offset: 0x0000D9A3
		public override int GetHashCode()
		{
			return this.PatchMethod.GetHashCode();
		}

		// Token: 0x040001C8 RID: 456
		public readonly int index;

		// Token: 0x040001C9 RID: 457
		public readonly string owner;

		// Token: 0x040001CA RID: 458
		public readonly int priority;

		// Token: 0x040001CB RID: 459
		public readonly string[] before;

		// Token: 0x040001CC RID: 460
		public readonly string[] after;

		// Token: 0x040001CD RID: 461
		public readonly bool debug;

		// Token: 0x040001CE RID: 462
		[NonSerialized]
		private MethodInfo patchMethod;

		// Token: 0x040001CF RID: 463
		private int methodToken;

		// Token: 0x040001D0 RID: 464
		private string moduleGUID;

		// Token: 0x040001D1 RID: 465
		public readonly InnerMethod innerMethod;
	}
}
