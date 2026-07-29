using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TaleWorlds.Library;

namespace FormationFilter.Models
{
	// Token: 0x0200001D RID: 29
	[NullableContext(1)]
	[Nullable(0)]
	public class TroopFilterGroup
	{
		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000110 RID: 272 RVA: 0x00008BF4 File Offset: 0x00006DF4
		// (set) Token: 0x06000111 RID: 273 RVA: 0x00008BFC File Offset: 0x00006DFC
		public List<TroopFilter> Filters { get; private set; } = new List<TroopFilter>();

		// Token: 0x06000113 RID: 275 RVA: 0x00008C18 File Offset: 0x00006E18
		public void SetFilter(int indexInFilterGroup, FilterTypeEnum filterType, FilterValueEnum filterEnum)
		{
			if (indexInFilterGroup >= this.Filters.Count)
			{
				InformationManager.DisplayMessage(new InformationMessage("FormationFilter: indexInFilterGroup out of range when setting filter", new Color(1f, 0f, 0f, 1f)));
				return;
			}
			this.Filters[indexInFilterGroup].SetFilter(filterType, filterEnum);
		}
	}
}
