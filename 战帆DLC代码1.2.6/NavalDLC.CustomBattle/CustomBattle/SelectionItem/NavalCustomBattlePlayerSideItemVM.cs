using System;
using TaleWorlds.Core.ViewModelCollection.Selector;

namespace NavalDLC.CustomBattle.CustomBattle.SelectionItem
{
	// Token: 0x02000020 RID: 32
	public class NavalCustomBattlePlayerSideItemVM : SelectorItemVM
	{
		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x060001EC RID: 492 RVA: 0x00009122 File Offset: 0x00007322
		// (set) Token: 0x060001ED RID: 493 RVA: 0x0000912A File Offset: 0x0000732A
		public NavalCustomBattlePlayerSide PlayerSide { get; private set; }

		// Token: 0x060001EE RID: 494 RVA: 0x00009133 File Offset: 0x00007333
		public NavalCustomBattlePlayerSideItemVM(string playerSideName, NavalCustomBattlePlayerSide playerSide)
			: base(playerSideName)
		{
			this.PlayerSide = playerSide;
		}
	}
}
