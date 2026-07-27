using System;
using TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapBar;

namespace NavalDLC.ViewModelCollection.Map.MapBar
{
	// Token: 0x0200002E RID: 46
	public class NavalMapBarVM : MapBarVM
	{
		// Token: 0x060003EC RID: 1004 RVA: 0x00012F9A File Offset: 0x0001119A
		protected override MapInfoVM CreateInfoVM()
		{
			return new NavalMapInfoVM();
		}
	}
}
