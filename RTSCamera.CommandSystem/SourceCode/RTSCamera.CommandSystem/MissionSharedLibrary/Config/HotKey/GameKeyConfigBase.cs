using System;

namespace MissionSharedLibrary.Config.HotKey
{
	// Token: 0x02000040 RID: 64
	public abstract class GameKeyConfigBase<T> : MissionConfigBase<T>, IGameKeyConfig where T : GameKeyConfigBase<T>
	{
		// Token: 0x1700007C RID: 124
		// (get) Token: 0x06000241 RID: 577 RVA: 0x00008873 File Offset: 0x00006A73
		// (set) Token: 0x06000242 RID: 578 RVA: 0x0000887B File Offset: 0x00006A7B
		public SerializedGameKeyCategory Category { get; set; } = new SerializedGameKeyCategory();

		// Token: 0x06000243 RID: 579 RVA: 0x00008884 File Offset: 0x00006A84
		protected override void CopyFrom(T other)
		{
			this.Category = other.Category;
		}
	}
}
