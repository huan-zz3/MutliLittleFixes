using System;
using TaleWorlds.Core.ViewModelCollection.Selector;

namespace NavalDLC.CustomBattle.CustomBattle.SelectionItem
{
	// Token: 0x02000024 RID: 36
	public class NavalCustomBattleTimeOfDayItemVM : SelectorItemVM
	{
		// Token: 0x170000BC RID: 188
		// (get) Token: 0x06000212 RID: 530 RVA: 0x000097DA File Offset: 0x000079DA
		// (set) Token: 0x06000213 RID: 531 RVA: 0x000097E2 File Offset: 0x000079E2
		public int TimeOfDay { get; private set; }

		// Token: 0x06000214 RID: 532 RVA: 0x000097EB File Offset: 0x000079EB
		public NavalCustomBattleTimeOfDayItemVM(string timeOfDayName, int timeOfDay)
			: base(timeOfDayName)
		{
			this.TimeOfDay = timeOfDay;
		}
	}
}
