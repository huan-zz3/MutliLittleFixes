using System;
using System.Runtime.CompilerServices;

namespace System.Diagnostics.CodeAnalysis
{
	// Token: 0x02000033 RID: 51
	[NullableContext(1)]
	[Nullable(0)]
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
	[ExcludeFromCodeCoverage]
	[DebuggerNonUserCode]
	internal sealed class MemberNotNullWhenAttribute : Attribute
	{
		// Token: 0x1700003D RID: 61
		// (get) Token: 0x06000219 RID: 537 RVA: 0x0000C8D4 File Offset: 0x0000AAD4
		public bool ReturnValue { get; }

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x0600021A RID: 538 RVA: 0x0000C8DC File Offset: 0x0000AADC
		public string[] Members { get; }

		// Token: 0x0600021B RID: 539 RVA: 0x0000C8E4 File Offset: 0x0000AAE4
		public MemberNotNullWhenAttribute(bool returnValue, string member)
		{
			this.ReturnValue = returnValue;
			this.Members = new string[] { member };
		}

		// Token: 0x0600021C RID: 540 RVA: 0x0000C903 File Offset: 0x0000AB03
		public MemberNotNullWhenAttribute(bool returnValue, params string[] members)
		{
			this.ReturnValue = returnValue;
			this.Members = members;
		}
	}
}
