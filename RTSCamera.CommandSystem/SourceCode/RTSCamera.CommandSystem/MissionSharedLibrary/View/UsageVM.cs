using System;
using MissionLibrary.Repository;
using MissionLibrary.Usage;
using TaleWorlds.Library;

namespace MissionSharedLibrary.View
{
	// Token: 0x02000013 RID: 19
	public class UsageVM : MissionMenuVMBase
	{
		// Token: 0x060000A9 RID: 169 RVA: 0x0000475D File Offset: 0x0000295D
		public UsageVM(ViewModel usageCollection, Action closeMenu)
			: base(closeMenu)
		{
			this.UsageCollection = usageCollection;
		}

		// Token: 0x060000AA RID: 170 RVA: 0x0000476D File Offset: 0x0000296D
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.UsageCollection.RefreshValues();
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00004780 File Offset: 0x00002980
		public override void OnFinalize()
		{
			base.OnFinalize();
			AUsageCategoryManager ausageCategoryManager = ARepository<AUsageCategoryManager, AUsageCategory>.Get();
			if (ausageCategoryManager == null)
			{
				return;
			}
			ausageCategoryManager.Clear();
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x060000AC RID: 172 RVA: 0x00004797 File Offset: 0x00002997
		public ViewModel UsageCollection { get; }
	}
}
