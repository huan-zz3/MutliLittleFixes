using System;
using TaleWorlds.Library;

namespace MissionLibrary.HotKey
{
	// Token: 0x0200001C RID: 28
	public abstract class AHotKeyConfigVM : ViewModel
	{
		// Token: 0x06000065 RID: 101
		public abstract void Update();

		// Token: 0x06000066 RID: 102
		public abstract void OnReset();

		// Token: 0x06000067 RID: 103
		public abstract void OnDone();
	}
}
