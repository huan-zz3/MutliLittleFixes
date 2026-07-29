using System;
using MissionLibrary.Repository;
using TaleWorlds.Library;

namespace MissionLibrary.Usage
{
	// Token: 0x0200000E RID: 14
	public abstract class AUsageCategoryManager : ARepository<AUsageCategoryManager, AUsageCategory>
	{
		// Token: 0x0600003D RID: 61
		public abstract void OnUsageCategorySelected(AUsageCategory usageCategory);

		// Token: 0x0600003E RID: 62
		public abstract ViewModel GetViewModel();

		// Token: 0x0600003F RID: 63
		public abstract void Clear();
	}
}
