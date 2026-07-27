using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Selector;

namespace NavalDLC.CustomBattle.CustomBattle.SelectionItem
{
	// Token: 0x0200001C RID: 28
	public class NavalCustomBattleCharacterItemVM : SelectorItemVM
	{
		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x060001D1 RID: 465 RVA: 0x00008EDC File Offset: 0x000070DC
		// (set) Token: 0x060001D2 RID: 466 RVA: 0x00008EE4 File Offset: 0x000070E4
		public BasicCharacterObject Character { get; private set; }

		// Token: 0x060001D3 RID: 467 RVA: 0x00008EED File Offset: 0x000070ED
		public NavalCustomBattleCharacterItemVM(BasicCharacterObject character)
			: base(character.Name.ToString())
		{
			this.Character = character;
		}
	}
}
