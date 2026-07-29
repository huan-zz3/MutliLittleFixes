using System;

namespace System.Diagnostics.CodeAnalysis
{
	// Token: 0x02000036 RID: 54
	[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
	[ExcludeFromCodeCoverage]
	[DebuggerNonUserCode]
	internal sealed class NotNullWhenAttribute : Attribute
	{
		// Token: 0x17000040 RID: 64
		// (get) Token: 0x06000220 RID: 544 RVA: 0x0000C938 File Offset: 0x0000AB38
		public bool ReturnValue { get; }

		// Token: 0x06000221 RID: 545 RVA: 0x0000C940 File Offset: 0x0000AB40
		public NotNullWhenAttribute(bool returnValue)
		{
			this.ReturnValue = returnValue;
		}
	}
}
