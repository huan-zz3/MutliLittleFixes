using System;

namespace MissionSharedLibrary.View.ViewModelCollection.Options.Selection
{
	// Token: 0x02000027 RID: 39
	public struct SelectionItem
	{
		// Token: 0x06000162 RID: 354 RVA: 0x00005EE0 File Offset: 0x000040E0
		public SelectionItem(bool isLocalizationId, string data, string variation = null)
		{
			this.IsLocalizationId = isLocalizationId;
			this.Data = data;
			this.Variation = variation;
		}

		// Token: 0x04000089 RID: 137
		public bool IsLocalizationId;

		// Token: 0x0400008A RID: 138
		public string Data;

		// Token: 0x0400008B RID: 139
		public string Variation;
	}
}
