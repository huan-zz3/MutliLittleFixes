using System;
using System.Linq;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.HotKeyCategories
{
	// Token: 0x02000100 RID: 256
	public class NavalShipControlsHotKeyCategory : GameKeyContext
	{
		// Token: 0x060012CB RID: 4811 RVA: 0x00089C3C File Offset: 0x00087E3C
		public NavalShipControlsHotKeyCategory()
			: base("NavalShipControlsHotKeyCategory", 116, 0)
		{
			GameAxisKey gameAxisKey = GenericGameKeyContext.Current.RegisteredGameAxisKeys.First<GameAxisKey>((GameAxisKey g) => g.Id.Equals("MovementAxisY"));
			GameAxisKey gameAxisKey2 = GenericGameKeyContext.Current.RegisteredGameAxisKeys.First<GameAxisKey>((GameAxisKey g) => g.Id.Equals("MovementAxisX"));
			base.RegisterGameAxisKey(gameAxisKey, true);
			base.RegisterGameAxisKey(gameAxisKey2, true);
			base.RegisterGameKey(new GameKey(110, "ToggleSail", "NavalShipControlsHotKeyCategory", 44, 240, GameKeyMainCategories.ShipControlsCategory), true);
			base.RegisterGameKey(new GameKey(111, "ToggleOarsmen", "NavalShipControlsHotKeyCategory", 45, 241, GameKeyMainCategories.ShipControlsCategory), true);
			base.RegisterGameKey(new GameKey(112, "ChangeShipCamera", "NavalShipControlsHotKeyCategory", 46, 243, GameKeyMainCategories.ShipControlsCategory), true);
			base.RegisterGameKey(new GameKey(113, "SelectShip", "NavalShipControlsHotKeyCategory", 18, 252, GameKeyMainCategories.ShipControlsCategory), true);
			base.RegisterGameKey(new GameKey(114, "AttemptBoarding", "NavalShipControlsHotKeyCategory", 19, 253, GameKeyMainCategories.ShipControlsCategory), true);
			base.RegisterGameKey(new GameKey(115, "ToggleRangedWeaponOrderMode", "NavalShipControlsHotKeyCategory", 225, 254, GameKeyMainCategories.ShipControlsCategory), true);
		}

		// Token: 0x04000AA5 RID: 2725
		public const string CategoryId = "NavalShipControlsHotKeyCategory";

		// Token: 0x04000AA6 RID: 2726
		public const string AccelerationAxis = "MovementAxisY";

		// Token: 0x04000AA7 RID: 2727
		public const string TurnAxis = "MovementAxisX";

		// Token: 0x04000AA8 RID: 2728
		public const int ToggleSail = 110;

		// Token: 0x04000AA9 RID: 2729
		public const int ToggleOarsmen = 111;

		// Token: 0x04000AAA RID: 2730
		public const int ChangeShipCamera = 112;

		// Token: 0x04000AAB RID: 2731
		public const int SelectShip = 113;

		// Token: 0x04000AAC RID: 2732
		public const int AttemptBoarding = 114;

		// Token: 0x04000AAD RID: 2733
		public const int ToggleRangedWeaponOrderMode = 115;
	}
}
