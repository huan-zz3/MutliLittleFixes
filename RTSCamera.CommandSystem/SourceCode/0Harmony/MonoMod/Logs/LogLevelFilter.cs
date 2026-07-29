using System;

namespace MonoMod.Logs
{
	// Token: 0x02000826 RID: 2086
	[Flags]
	internal enum LogLevelFilter
	{
		// Token: 0x04003A22 RID: 14882
		None = 0,
		// Token: 0x04003A23 RID: 14883
		Spam = 1,
		// Token: 0x04003A24 RID: 14884
		Trace = 2,
		// Token: 0x04003A25 RID: 14885
		Info = 4,
		// Token: 0x04003A26 RID: 14886
		Warning = 8,
		// Token: 0x04003A27 RID: 14887
		Error = 16,
		// Token: 0x04003A28 RID: 14888
		Assert = 32,
		// Token: 0x04003A29 RID: 14889
		DefaultFilter = -2
	}
}
