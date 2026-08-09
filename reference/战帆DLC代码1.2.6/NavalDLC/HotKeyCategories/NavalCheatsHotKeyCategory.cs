using System;
using TaleWorlds.InputSystem;

namespace NavalDLC.HotKeyCategories
{
	// Token: 0x020000FE RID: 254
	public class NavalCheatsHotKeyCategory : GameKeyContext
	{
		// Token: 0x060012CA RID: 4810 RVA: 0x00089BA0 File Offset: 0x00087DA0
		public NavalCheatsHotKeyCategory()
			: base("NavalCheatsHotKeyCategory", 0, 1)
		{
			base.RegisterHotKey(new HotKey("DebugSailingMoveToLeft", "NavalCheatsHotKeyCategory", 30, 2, 0), true);
			base.RegisterHotKey(new HotKey("DebugSailingMoveToRight", "NavalCheatsHotKeyCategory", 32, 2, 0), true);
			base.RegisterHotKey(new HotKey("DebugRammingCollision", "NavalCheatsHotKeyCategory", 19, 3, 0), true);
			base.RegisterHotKey(new HotKey("DebugDealSiegeEngineDamage", "NavalCheatsHotKeyCategory", 48, 3, 0), true);
			base.RegisterHotKey(new HotKey("DebugSetWindDirection", "NavalCheatsHotKeyCategory", 17, 3, 0), true);
		}

		// Token: 0x04000A9A RID: 2714
		public const string CategoryId = "NavalCheatsHotKeyCategory";

		// Token: 0x04000A9B RID: 2715
		public const string DebugSailingMoveToRight = "DebugSailingMoveToRight";

		// Token: 0x04000A9C RID: 2716
		public const string DebugSailingMoveToLeft = "DebugSailingMoveToLeft";

		// Token: 0x04000A9D RID: 2717
		public const string DebugRammingCollision = "DebugRammingCollision";

		// Token: 0x04000A9E RID: 2718
		public const string DebugDealSiegeEngineDamage = "DebugDealSiegeEngineDamage";

		// Token: 0x04000A9F RID: 2719
		public const string DebugSetWindDirection = "DebugSetWindDirection";
	}
}
