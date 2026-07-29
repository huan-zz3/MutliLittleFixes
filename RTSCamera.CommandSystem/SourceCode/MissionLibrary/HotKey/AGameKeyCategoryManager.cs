using System;
using MissionLibrary.Repository;

namespace MissionLibrary.HotKey
{
	// Token: 0x0200001B RID: 27
	public abstract class AGameKeyCategoryManager : ARepository<AGameKeyCategoryManager, AGameKeyCategory>
	{
		// Token: 0x06000063 RID: 99
		public abstract void Save();
	}
}
