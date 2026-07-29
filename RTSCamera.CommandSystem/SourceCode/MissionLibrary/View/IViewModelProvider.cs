using System;
using TaleWorlds.Library;

namespace MissionLibrary.View
{
	// Token: 0x02000009 RID: 9
	public interface IViewModelProvider<out T> where T : ViewModel
	{
		// Token: 0x06000024 RID: 36
		T GetViewModel();
	}
}
