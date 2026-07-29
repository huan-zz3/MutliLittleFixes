using System;

namespace MonoMod.Logs
{
	// Token: 0x02000824 RID: 2084
	internal interface IDebugFormattable
	{
		// Token: 0x0600282E RID: 10286
		bool TryFormatInto(Span<char> span, out int wrote);
	}
}
