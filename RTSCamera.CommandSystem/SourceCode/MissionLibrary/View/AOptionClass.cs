using System;
using MissionLibrary.Repository;
using TaleWorlds.Library;

namespace MissionLibrary.View
{
	// Token: 0x02000006 RID: 6
	public abstract class AOptionClass : AItem<AOptionClass>, IViewModelProvider<ViewModel>
	{
		// Token: 0x0600001E RID: 30
		public abstract ViewModel GetViewModel();

		// Token: 0x0600001F RID: 31
		public abstract void UpdateSelection(bool isSelected);
	}
}
