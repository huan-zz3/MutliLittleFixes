using System;
using MissionLibrary.Provider;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace MissionLibrary.View
{
	// Token: 0x02000005 RID: 5
	public abstract class AMenuManager : ATag<AMenuManager>
	{
		// Token: 0x06000013 RID: 19 RVA: 0x00002104 File Offset: 0x00000304
		public static AMenuManager Get()
		{
			return Global.GetInstance<AMenuManager>("");
		}

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x06000014 RID: 20 RVA: 0x00002110 File Offset: 0x00000310
		// (remove) Token: 0x06000015 RID: 21 RVA: 0x00002148 File Offset: 0x00000348
		public event Action OnMenuClosedEvent;

		// Token: 0x06000016 RID: 22 RVA: 0x0000217D File Offset: 0x0000037D
		public void OnMenuClosed()
		{
			Action onMenuClosedEvent = this.OnMenuClosedEvent;
			if (onMenuClosedEvent == null)
			{
				return;
			}
			onMenuClosedEvent();
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000017 RID: 23
		public abstract AMenuClassCollection MenuClassCollection { get; }

		// Token: 0x06000018 RID: 24
		public abstract MissionView CreateMenuView();

		// Token: 0x06000019 RID: 25
		public abstract MissionView CreateGameKeyConfigView();

		// Token: 0x0600001A RID: 26
		public abstract void RequestToOpenMenu();

		// Token: 0x0600001B RID: 27
		public abstract void RequestToCloseMenu();

		// Token: 0x0600001C RID: 28
		public abstract void RequestToOpenUsageView();
	}
}
