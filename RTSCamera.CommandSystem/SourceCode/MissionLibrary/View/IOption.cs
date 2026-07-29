using System;
using TaleWorlds.Library;

namespace MissionLibrary.View
{
	// Token: 0x02000007 RID: 7
	public interface IOption : IViewModelProvider<ViewModel>
	{
		// Token: 0x06000021 RID: 33
		void Commit();

		// Token: 0x06000022 RID: 34
		void Cancel();
	}
}
