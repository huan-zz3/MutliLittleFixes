using System;

namespace System.Diagnostics.CodeAnalysis
{
	// Token: 0x0200002E RID: 46
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	[ExcludeFromCodeCoverage]
	[DebuggerNonUserCode]
	internal sealed class DoesNotReturnAttribute : Attribute
	{
	}
}
