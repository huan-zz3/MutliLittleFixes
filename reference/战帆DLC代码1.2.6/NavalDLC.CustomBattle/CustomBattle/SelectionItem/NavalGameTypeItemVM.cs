using System;
using TaleWorlds.Core.ViewModelCollection.Selector;

namespace NavalDLC.CustomBattle.CustomBattle.SelectionItem
{
	// Token: 0x0200001E RID: 30
	public class NavalGameTypeItemVM : SelectorItemVM
	{
		// Token: 0x170000AA RID: 170
		// (get) Token: 0x060001DD RID: 477 RVA: 0x00008FD7 File Offset: 0x000071D7
		// (set) Token: 0x060001DE RID: 478 RVA: 0x00008FDF File Offset: 0x000071DF
		public string GameTypeStringId { get; private set; }

		// Token: 0x060001DF RID: 479 RVA: 0x00008FE8 File Offset: 0x000071E8
		public NavalGameTypeItemVM(string gameTypeName, string gameType)
			: base(gameTypeName)
		{
			this.GameTypeStringId = gameType;
		}
	}
}
