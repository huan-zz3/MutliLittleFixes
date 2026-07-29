using System;

namespace System.Diagnostics.CodeAnalysis
{
	// Token: 0x0200002F RID: 47
	[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
	[ExcludeFromCodeCoverage]
	[DebuggerNonUserCode]
	internal sealed class DoesNotReturnIfAttribute : Attribute
	{
		// Token: 0x1700003A RID: 58
		// (get) Token: 0x06000211 RID: 529 RVA: 0x0000C86F File Offset: 0x0000AA6F
		public bool ParameterValue { get; }

		// Token: 0x06000212 RID: 530 RVA: 0x0000C877 File Offset: 0x0000AA77
		public DoesNotReturnIfAttribute(bool parameterValue)
		{
			this.ParameterValue = parameterValue;
		}
	}
}
