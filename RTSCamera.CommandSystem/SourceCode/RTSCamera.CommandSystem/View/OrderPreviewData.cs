using System;
using System.Collections.Generic;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace RTSCamera.CommandSystem.View
{
	// Token: 0x0200004F RID: 79
	public class OrderPreviewData
	{
		// Token: 0x04000102 RID: 258
		public WorldPosition OrderPosition;

		// Token: 0x04000103 RID: 259
		public float? Width;

		// Token: 0x04000104 RID: 260
		public float? Depth;

		// Token: 0x04000105 RID: 261
		public float? RightSideOffset;

		// Token: 0x04000106 RID: 262
		public Vec2 Direction;

		// Token: 0x04000107 RID: 263
		public List<WorldPosition> AgentPositions = new List<WorldPosition>();

		// Token: 0x04000108 RID: 264
		public OrderTargetType OrderTargetType;
	}
}
