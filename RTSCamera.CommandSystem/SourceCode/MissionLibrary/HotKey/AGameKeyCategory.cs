using System;
using MissionLibrary.Repository;

namespace MissionLibrary.HotKey
{
	// Token: 0x0200001A RID: 26
	public abstract class AGameKeyCategory : AItem<AGameKeyCategory>
	{
		// Token: 0x0600005E RID: 94
		public abstract IGameKeySequence GetGameKeySequence(int i);

		// Token: 0x0600005F RID: 95
		public abstract void Save();

		// Token: 0x06000060 RID: 96
		public abstract void Load();

		// Token: 0x06000061 RID: 97
		public abstract AHotKeyConfigVM CreateViewModel(Action<IHotKeySetter> onKeyBindRequest);
	}
}
