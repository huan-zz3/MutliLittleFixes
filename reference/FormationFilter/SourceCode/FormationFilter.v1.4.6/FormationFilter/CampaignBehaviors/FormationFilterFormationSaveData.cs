using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FormationFilter.CampaignBehaviors
{
	// Token: 0x02000024 RID: 36
	[NullableContext(1)]
	[Nullable(0)]
	public class FormationFilterFormationSaveData
	{
		// Token: 0x04000097 RID: 151
		public string Captain = "";

		// Token: 0x04000098 RID: 152
		public List<string> HeroTroops = new List<string>();

		// Token: 0x04000099 RID: 153
		public List<FormationFilterFormationClassSaveData> FormationClassFilters = new List<FormationFilterFormationClassSaveData>();
	}
}
