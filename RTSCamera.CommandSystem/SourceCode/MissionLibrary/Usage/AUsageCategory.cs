using System;
using MissionLibrary.Repository;
using TaleWorlds.Library;

namespace MissionLibrary.Usage
{
	// Token: 0x0200000D RID: 13
	public abstract class AUsageCategory : AItem<AUsageCategory>
	{
		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600003A RID: 58
		public abstract ViewModel ViewModel { get; }

		// Token: 0x0600003B RID: 59
		public abstract void UpdateSelection(bool isSelected);
	}
}
