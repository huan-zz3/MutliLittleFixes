using System;
using TaleWorlds.Library;

namespace MissionLibrary.View
{
	// Token: 0x0200000A RID: 10
	public interface IViewModelProvider<out T, out U, in V> where T : ViewModel
	{
		// Token: 0x06000025 RID: 37
		T GetViewModel(Func<U, V> func);
	}
}
