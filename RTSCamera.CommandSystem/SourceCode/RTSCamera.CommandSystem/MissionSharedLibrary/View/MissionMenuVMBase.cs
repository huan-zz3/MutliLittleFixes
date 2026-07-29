using System;
using MissionLibrary.View;
using TaleWorlds.Library;

namespace MissionSharedLibrary.View
{
	// Token: 0x02000016 RID: 22
	public abstract class MissionMenuVMBase : ViewModel
	{
		// Token: 0x060000C1 RID: 193 RVA: 0x00004B13 File Offset: 0x00002D13
		public virtual void CloseMenu()
		{
			AMenuManager.Get().OnMenuClosed();
			Action closeMenu = this._closeMenu;
			if (closeMenu == null)
			{
				return;
			}
			closeMenu();
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x00004B2F File Offset: 0x00002D2F
		protected MissionMenuVMBase(Action closeMenu)
		{
			this._closeMenu = closeMenu;
		}

		// Token: 0x04000045 RID: 69
		private readonly Action _closeMenu;
	}
}
