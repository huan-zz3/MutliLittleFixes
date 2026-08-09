using System;

namespace NavalDLC.ViewModelCollection.Port.PortScreenHandlers
{
	// Token: 0x02000018 RID: 24
	public readonly struct PortChangeInfo
	{
		// Token: 0x060001CB RID: 459 RVA: 0x0000ACEE File Offset: 0x00008EEE
		public PortChangeInfo(float goldCost, string description)
		{
			this.GoldCost = goldCost;
			this.Description = description;
		}

		// Token: 0x040000B4 RID: 180
		public readonly float GoldCost;

		// Token: 0x040000B5 RID: 181
		public readonly string Description;
	}
}
