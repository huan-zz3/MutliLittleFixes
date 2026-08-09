using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FormationFilter.Models;
using TaleWorlds.Core;

namespace FormationFilter.CampaignBehaviors
{
	// Token: 0x02000023 RID: 35
	public class FormationFilterFormationClassSaveData
	{
		// Token: 0x04000094 RID: 148
		public FormationClass BasicFormationClass = 10;

		// Token: 0x04000095 RID: 149
		[Nullable(1)]
		public Dictionary<FilterTypeEnum, FilterValueEnum> FilterValueDictionary = new Dictionary<FilterTypeEnum, FilterValueEnum>();

		// Token: 0x04000096 RID: 150
		public float Weight;
	}
}
