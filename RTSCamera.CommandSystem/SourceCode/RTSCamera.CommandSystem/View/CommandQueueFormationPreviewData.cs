using System;
using System.Collections.Generic;
using TaleWorlds.MountAndBlade;

namespace RTSCamera.CommandSystem.View
{
	// Token: 0x02000052 RID: 82
	public class CommandQueueFormationPreviewData
	{
		// Token: 0x0400011C RID: 284
		public Formation Formation;

		// Token: 0x0400011D RID: 285
		public OrderPreviewData PendingOrder;

		// Token: 0x0400011E RID: 286
		public List<OrderPreviewData> OrderList = new List<OrderPreviewData>();

		// Token: 0x0400011F RID: 287
		public bool IsSelected;
	}
}
