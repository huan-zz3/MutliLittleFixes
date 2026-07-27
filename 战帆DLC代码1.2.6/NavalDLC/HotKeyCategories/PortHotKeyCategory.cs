using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.HotKeyCategories
{
	// Token: 0x02000101 RID: 257
	public class PortHotKeyCategory : GameKeyContext
	{
		// Token: 0x060012CC RID: 4812 RVA: 0x00089DA0 File Offset: 0x00087FA0
		public PortHotKeyCategory()
			: base("PortHotKeyCategory", 0, 0)
		{
			this.RegisterHotKeys();
			this.RegisterGameAxisKeys();
		}

		// Token: 0x060012CD RID: 4813 RVA: 0x00089DBC File Offset: 0x00087FBC
		private void RegisterHotKeys()
		{
			base.RegisterHotKey(new HotKey("SelectLeftRoster", "PortHotKeyCategory", 254, 0, 0), true);
			base.RegisterHotKey(new HotKey("SelectRightRoster", "PortHotKeyCategory", 255, 0, 0), true);
			base.RegisterHotKey(new HotKey("ControllerDeviateLeft", "PortHotKeyCategory", 248, 0, 0), true);
			base.RegisterHotKey(new HotKey("ControllerDeviateRight", "PortHotKeyCategory", 249, 0, 0), true);
			base.RegisterHotKey(new HotKey("ControllerZoomIn", "PortHotKeyCategory", 255, 0, 0), true);
			base.RegisterHotKey(new HotKey("ControllerZoomOut", "PortHotKeyCategory", 254, 0, 0), true);
			List<Key> list = new List<Key>
			{
				new Key(225),
				new Key(252)
			};
			base.RegisterHotKey(new HotKey("ToggleCameraMovement", "PortHotKeyCategory", list, 0, 0), true);
			list = new List<Key>
			{
				new Key(19),
				new Key(253)
			};
			base.RegisterHotKey(new HotKey("ResetCamera", "PortHotKeyCategory", list, 0, 0), true);
		}

		// Token: 0x060012CE RID: 4814 RVA: 0x00089EF4 File Offset: 0x000880F4
		private void RegisterGameAxisKeys()
		{
			GameAxisKey gameAxisKey = GenericGameKeyContext.Current.RegisteredGameAxisKeys.First<GameAxisKey>((GameAxisKey g) => g.Id.Equals("CameraAxisX"));
			GameAxisKey gameAxisKey2 = GenericGameKeyContext.Current.RegisteredGameAxisKeys.First<GameAxisKey>((GameAxisKey g) => g.Id.Equals("CameraAxisY"));
			GameAxisKey gameAxisKey3 = GenericGameKeyContext.Current.RegisteredGameAxisKeys.First<GameAxisKey>((GameAxisKey g) => g.Id.Equals("MovementAxisX"));
			GameAxisKey gameAxisKey4 = GenericGameKeyContext.Current.RegisteredGameAxisKeys.First<GameAxisKey>((GameAxisKey g) => g.Id.Equals("MovementAxisY"));
			base.RegisterGameAxisKey(gameAxisKey, true);
			base.RegisterGameAxisKey(gameAxisKey2, true);
			base.RegisterGameAxisKey(gameAxisKey3, true);
			base.RegisterGameAxisKey(gameAxisKey4, true);
		}

		// Token: 0x04000AAE RID: 2734
		public const string CategoryId = "PortHotKeyCategory";

		// Token: 0x04000AAF RID: 2735
		public const string SelectLeftRoster = "SelectLeftRoster";

		// Token: 0x04000AB0 RID: 2736
		public const string SelectRightRoster = "SelectRightRoster";

		// Token: 0x04000AB1 RID: 2737
		public const string ToggleCameraMovement = "ToggleCameraMovement";

		// Token: 0x04000AB2 RID: 2738
		public const string ResetCamera = "ResetCamera";

		// Token: 0x04000AB3 RID: 2739
		public const string ControllerDeviateLeft = "ControllerDeviateLeft";

		// Token: 0x04000AB4 RID: 2740
		public const string ControllerDeviateRight = "ControllerDeviateRight";

		// Token: 0x04000AB5 RID: 2741
		public const string ControllerZoomIn = "ControllerZoomIn";

		// Token: 0x04000AB6 RID: 2742
		public const string ControllerZoomOut = "ControllerZoomOut";

		// Token: 0x04000AB7 RID: 2743
		public const string ControllerHorizontalRotationAxis = "CameraAxisX";

		// Token: 0x04000AB8 RID: 2744
		public const string ControllerVerticalRotationAxis = "CameraAxisY";

		// Token: 0x04000AB9 RID: 2745
		public const string CameraTargetDeviationAxis = "MovementAxisX";

		// Token: 0x04000ABA RID: 2746
		public const string ZoomAxis = "MovementAxisY";
	}
}
