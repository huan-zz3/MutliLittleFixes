using System;
using MissionLibrary.View;
using MissionSharedLibrary.View.ViewModelCollection;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace MissionSharedLibrary.View
{
	// Token: 0x02000014 RID: 20
	public class MenuManager : AMenuManager
	{
		// Token: 0x1700001C RID: 28
		// (get) Token: 0x060000AD RID: 173 RVA: 0x0000479F File Offset: 0x0000299F
		public override AMenuClassCollection MenuClassCollection { get; } = new MenuClassCollection();

		// Token: 0x060000AE RID: 174 RVA: 0x000047A7 File Offset: 0x000029A7
		public override MissionView CreateMenuView()
		{
			return null;
		}

		// Token: 0x060000AF RID: 175 RVA: 0x000047AA File Offset: 0x000029AA
		public override MissionView CreateGameKeyConfigView()
		{
			return null;
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x000047AD File Offset: 0x000029AD
		public override void RequestToOpenMenu()
		{
			OptionView missionBehavior = Mission.Current.GetMissionBehavior<OptionView>();
			if (missionBehavior == null)
			{
				return;
			}
			missionBehavior.ActivateMenu();
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x000047C3 File Offset: 0x000029C3
		public override void RequestToCloseMenu()
		{
			OptionView missionBehavior = Mission.Current.GetMissionBehavior<OptionView>();
			if (missionBehavior == null)
			{
				return;
			}
			missionBehavior.DeactivateMenu();
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x000047D9 File Offset: 0x000029D9
		public override void RequestToOpenUsageView()
		{
			UsageView missionBehavior = Mission.Current.GetMissionBehavior<UsageView>();
			if (missionBehavior == null)
			{
				return;
			}
			missionBehavior.ActivateMenu();
		}
	}
}
