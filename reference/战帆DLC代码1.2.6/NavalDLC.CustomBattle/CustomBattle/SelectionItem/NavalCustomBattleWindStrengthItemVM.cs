using System;
using TaleWorlds.Core.ViewModelCollection.Selector;

namespace NavalDLC.CustomBattle.CustomBattle.SelectionItem
{
	// Token: 0x02000026 RID: 38
	public class NavalCustomBattleWindStrengthItemVM : SelectorItemVM
	{
		// Token: 0x170000BE RID: 190
		// (get) Token: 0x06000218 RID: 536 RVA: 0x0000981C File Offset: 0x00007A1C
		// (set) Token: 0x06000219 RID: 537 RVA: 0x00009824 File Offset: 0x00007A24
		public float WindStrength { get; private set; }

		// Token: 0x0600021A RID: 538 RVA: 0x0000982D File Offset: 0x00007A2D
		public NavalCustomBattleWindStrengthItemVM(string windStrengthName, float windStrength)
			: base(windStrengthName)
		{
			this.WindStrength = windStrength;
		}
	}
}
