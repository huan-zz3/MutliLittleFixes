using System;
using TaleWorlds.Core.ViewModelCollection.Selector;

namespace NavalDLC.CustomBattle.CustomBattle.SelectionItem
{
	// Token: 0x02000021 RID: 33
	public class NavalCustomBattleSeasonItemVM : SelectorItemVM
	{
		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x060001EF RID: 495 RVA: 0x00009143 File Offset: 0x00007343
		// (set) Token: 0x060001F0 RID: 496 RVA: 0x0000914B File Offset: 0x0000734B
		public string SeasonId { get; private set; }

		// Token: 0x060001F1 RID: 497 RVA: 0x00009154 File Offset: 0x00007354
		public NavalCustomBattleSeasonItemVM(string seasonName, string seasonId)
			: base(seasonName)
		{
			this.SeasonId = seasonId;
		}
	}
}
