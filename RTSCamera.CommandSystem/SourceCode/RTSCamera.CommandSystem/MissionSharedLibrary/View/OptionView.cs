using System;
using MissionLibrary.Event;
using MissionLibrary.View;
using MissionSharedLibrary.HotKey;

namespace MissionSharedLibrary.View
{
	// Token: 0x02000017 RID: 23
	public class OptionView : MissionMenuViewBase
	{
		// Token: 0x060000C3 RID: 195 RVA: 0x00004B3E File Offset: 0x00002D3E
		public OptionView(int viewOrderPriority, Version version)
			: base(viewOrderPriority, "MissionLibraryOptionView-" + ((version != null) ? version.ToString() : null), true, true)
		{
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x00004B60 File Offset: 0x00002D60
		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			if (base.IsActivated)
			{
				if (GeneralGameKeyCategory.GetKey(GeneralGameKey.OpenMenu).IsKeyPressed(null))
				{
					this.DeactivateMenu();
					return;
				}
			}
			else if (base.Mission.Mode != 1 && GeneralGameKeyCategory.GetKey(GeneralGameKey.OpenMenu).IsKeyPressed(null))
			{
				this.ActivateMenu();
			}
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x00004BB3 File Offset: 0x00002DB3
		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			MissionEvent.Clear();
			AMenuManager.Get().MenuClassCollection.Clear();
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x00004BCF File Offset: 0x00002DCF
		protected override MissionMenuVMBase GetDataSource()
		{
			return new OptionVM(AMenuManager.Get().MenuClassCollection, new Action(base.OnCloseMenu));
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x00004BEC File Offset: 0x00002DEC
		public override void DeactivateMenu()
		{
			if (!base.IsActivated)
			{
				return;
			}
			base.DeactivateMenu();
			MissionEvent.OnMissionMenuClosed();
		}
	}
}
