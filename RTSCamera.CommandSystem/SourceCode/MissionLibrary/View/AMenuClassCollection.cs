using System;
using MissionLibrary.Repository;
using TaleWorlds.Library;

namespace MissionLibrary.View
{
	// Token: 0x02000004 RID: 4
	public abstract class AMenuClassCollection : ARepository<AMenuClassCollection, AOptionClass>
	{
		// Token: 0x0600000F RID: 15
		public abstract void OnOptionClassSelected(AOptionClass optionClass);

		// Token: 0x06000010 RID: 16
		public abstract void Clear();

		// Token: 0x06000011 RID: 17
		public abstract ViewModel GetViewModel();
	}
}
