using System;

namespace RTSCamera.CommandSystem.AgentComponents
{
	// Token: 0x02000097 RID: 151
	public struct Highlight
	{
		// Token: 0x0600056D RID: 1389 RVA: 0x000201A2 File Offset: 0x0001E3A2
		public Highlight(uint? color, bool alwaysVisible)
		{
			this.Color = color;
			this.AlwaysVisible = alwaysVisible;
		}

		// Token: 0x0400029F RID: 671
		public uint? Color;

		// Token: 0x040002A0 RID: 672
		public bool AlwaysVisible;
	}
}
