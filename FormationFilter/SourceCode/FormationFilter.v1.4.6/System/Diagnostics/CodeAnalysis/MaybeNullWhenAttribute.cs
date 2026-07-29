using System;

namespace System.Diagnostics.CodeAnalysis
{
	// Token: 0x02000031 RID: 49
	[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
	[ExcludeFromCodeCoverage]
	[DebuggerNonUserCode]
	internal sealed class MaybeNullWhenAttribute : Attribute
	{
		// Token: 0x1700003B RID: 59
		// (get) Token: 0x06000214 RID: 532 RVA: 0x0000C88E File Offset: 0x0000AA8E
		public bool ReturnValue { get; }

		// Token: 0x06000215 RID: 533 RVA: 0x0000C896 File Offset: 0x0000AA96
		public MaybeNullWhenAttribute(bool returnValue)
		{
			this.ReturnValue = returnValue;
		}
	}
}
