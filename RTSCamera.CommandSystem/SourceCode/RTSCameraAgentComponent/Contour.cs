using System;

namespace RTSCameraAgentComponent
{
	// Token: 0x02000004 RID: 4
	public struct Contour
	{
		// Token: 0x06000007 RID: 7 RVA: 0x00002098 File Offset: 0x00000298
		public Contour(uint? color, bool alwaysVisible)
		{
			this.Color = color;
			this.AlwaysVisible = alwaysVisible;
		}

		// Token: 0x04000001 RID: 1
		public uint? Color;

		// Token: 0x04000002 RID: 2
		public bool AlwaysVisible;
	}
}
