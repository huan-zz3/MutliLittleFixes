using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FormationFilter.Utilities;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace FormationFilter.Models
{
	// Token: 0x0200001E RID: 30
	[NullableContext(1)]
	[Nullable(0)]
	public class FormationFilters
	{
		// Token: 0x1700002E RID: 46
		// (get) Token: 0x06000114 RID: 276 RVA: 0x00008C6F File Offset: 0x00006E6F
		// (set) Token: 0x06000115 RID: 277 RVA: 0x00008C77 File Offset: 0x00006E77
		public List<TroopFilter> Filters { get; private set; } = new List<TroopFilter>();

		// Token: 0x06000116 RID: 278 RVA: 0x00008C80 File Offset: 0x00006E80
		public void SetFilter(int indexInFormation, FilterTypeEnum filterType, FilterValueEnum filterEnum)
		{
			if (indexInFormation >= this.Filters.Count)
			{
				InformationManager.DisplayMessage(new InformationMessage("FormationFilter: indexInFormation out of range when setting filter", new Color(1f, 0f, 0f, 1f)));
				return;
			}
			this.Filters[indexInFormation].SetFilter(filterType, filterEnum);
		}

		// Token: 0x06000117 RID: 279 RVA: 0x00008CD7 File Offset: 0x00006ED7
		public List<TroopFilter> GetTroopFilters()
		{
			return this.Filters;
		}

		// Token: 0x06000118 RID: 280 RVA: 0x00008CE0 File Offset: 0x00006EE0
		public void EnsureFilterCount(int count)
		{
			if (this.Filters.Count > count)
			{
				this.Filters.RemoveRange(count, this.Filters.Count - count);
			}
			if (this.Filters.Count < count)
			{
				for (int i = this.Filters.Count; i < count; i++)
				{
					this.Filters.Add(new TroopFilter());
				}
			}
		}

		// Token: 0x06000119 RID: 281 RVA: 0x00008D48 File Offset: 0x00006F48
		public TroopFilter ForceGetTroopFilterAtIndex(int index, FormationClass formationClass)
		{
			if (index >= this.Filters.Count)
			{
				for (int i = this.Filters.Count; i <= index; i++)
				{
					TroopFilter troopFilter = new TroopFilter();
					this.Filters.Add(troopFilter);
				}
			}
			this.Filters[index].SetBasicFilterForFormationClass(formationClass);
			return this.Filters[index];
		}

		// Token: 0x0600011A RID: 282 RVA: 0x00008DA9 File Offset: 0x00006FA9
		[NullableContext(2)]
		public TroopFilter TryGetTroopFilterAtIndex(int index)
		{
			if (index >= this.Filters.Count)
			{
				return null;
			}
			return this.Filters[index];
		}

		// Token: 0x0600011B RID: 283 RVA: 0x00008DC7 File Offset: 0x00006FC7
		public void ClearInvalidTroopFilter()
		{
			this.Filters.RemoveAll((TroopFilter filter) => filter.DetectBasicFomrationClass() == 10);
		}
	}
}
