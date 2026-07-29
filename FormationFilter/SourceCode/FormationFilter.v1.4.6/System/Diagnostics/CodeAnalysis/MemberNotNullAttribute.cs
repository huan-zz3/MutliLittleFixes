using System;
using System.Runtime.CompilerServices;

namespace System.Diagnostics.CodeAnalysis
{
	// Token: 0x02000032 RID: 50
	[NullableContext(1)]
	[Nullable(0)]
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
	[ExcludeFromCodeCoverage]
	[DebuggerNonUserCode]
	internal sealed class MemberNotNullAttribute : Attribute
	{
		// Token: 0x1700003C RID: 60
		// (get) Token: 0x06000216 RID: 534 RVA: 0x0000C8A5 File Offset: 0x0000AAA5
		public string[] Members { get; }

		// Token: 0x06000217 RID: 535 RVA: 0x0000C8AD File Offset: 0x0000AAAD
		public MemberNotNullAttribute(string member)
		{
			this.Members = new string[] { member };
		}

		// Token: 0x06000218 RID: 536 RVA: 0x0000C8C5 File Offset: 0x0000AAC5
		public MemberNotNullAttribute(params string[] members)
		{
			this.Members = members;
		}
	}
}
