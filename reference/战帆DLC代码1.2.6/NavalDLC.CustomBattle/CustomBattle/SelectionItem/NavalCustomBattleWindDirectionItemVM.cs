using System;
using NavalDLC.Missions.MissionLogics;
using TaleWorlds.Core.ViewModelCollection.Selector;

namespace NavalDLC.CustomBattle.CustomBattle.SelectionItem
{
	// Token: 0x02000025 RID: 37
	public class NavalCustomBattleWindDirectionItemVM : SelectorItemVM
	{
		// Token: 0x170000BD RID: 189
		// (get) Token: 0x06000215 RID: 533 RVA: 0x000097FB File Offset: 0x000079FB
		// (set) Token: 0x06000216 RID: 534 RVA: 0x00009803 File Offset: 0x00007A03
		public NavalCustomBattleWindConfig.Direction WindDirection { get; private set; }

		// Token: 0x06000217 RID: 535 RVA: 0x0000980C File Offset: 0x00007A0C
		public NavalCustomBattleWindDirectionItemVM(string windDirectionName, NavalCustomBattleWindConfig.Direction windDirection)
			: base(windDirectionName)
		{
			this.WindDirection = windDirection;
		}
	}
}
