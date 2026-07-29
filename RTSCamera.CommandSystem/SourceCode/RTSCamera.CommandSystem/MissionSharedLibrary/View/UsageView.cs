using System;
using MissionLibrary.Repository;
using MissionLibrary.Usage;
using MissionSharedLibrary.HotKey;
using MissionSharedLibrary.View.ViewModelCollection.Usage;
using TaleWorlds.Core;

namespace MissionSharedLibrary.View
{
	// Token: 0x02000012 RID: 18
	public class UsageView : MissionMenuViewBase
	{
		// Token: 0x060000A5 RID: 165 RVA: 0x000046CB File Offset: 0x000028CB
		public UsageView(int viewOrderPriority, Version version)
			: base(viewOrderPriority, "MissionLibraryUsageView-" + ((version != null) ? version.ToString() : null), false, true)
		{
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x000046ED File Offset: 0x000028ED
		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			if (base.IsActivated && GeneralGameKeyCategory.GetKey(GeneralGameKey.OpenMenu).IsKeyPressed(null))
			{
				this.DeactivateMenu();
			}
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00004712 File Offset: 0x00002912
		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			AUsageCategoryManager ausageCategoryManager = ARepository<AUsageCategoryManager, AUsageCategory>.Get();
			if (ausageCategoryManager == null)
			{
				return;
			}
			ausageCategoryManager.Clear();
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00004729 File Offset: 0x00002929
		protected override MissionMenuVMBase GetDataSource()
		{
			return new UsageVM(new UsageCollectionViewModel(GameTexts.FindText("str_mission_library_usages", null), ARepository<AUsageCategoryManager, AUsageCategory>.Get(), new Action(base.OnCloseMenu)), new Action(base.OnCloseMenu));
		}
	}
}
