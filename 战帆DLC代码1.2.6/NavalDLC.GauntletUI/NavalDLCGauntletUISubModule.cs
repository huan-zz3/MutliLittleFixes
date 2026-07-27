using System;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.GauntletUI;
using TaleWorlds.TwoDimension;

namespace NavalDLC.GauntletUI
{
	// Token: 0x02000005 RID: 5
	public class NavalDLCGauntletUISubModule : MBSubModuleBase
	{
		// Token: 0x06000005 RID: 5 RVA: 0x00002068 File Offset: 0x00000268
		protected override void OnApplicationTick(float dt)
		{
			base.OnApplicationTick(dt);
			if (!this._initializedLoadingCategory)
			{
				LoadingWindow.InitializeWith<GauntletNavalLoadingWindowManager>();
				this._initializedLoadingCategory = true;
			}
			if (!this._loadBackgroundCategory && 5 == this._frameCounterToLoad)
			{
				this._fullBackgroundsCategory = UIResourceManager.LoadSpriteCategory("ui_naval_fullbackgrounds");
				this._loadBackgroundCategory = true;
				return;
			}
			if (!this._loadBackgroundCategory)
			{
				this._frameCounterToLoad++;
			}
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000020CF File Offset: 0x000002CF
		protected override void OnSubModuleLoad()
		{
			base.OnSubModuleLoad();
			GauntletGameVersionView.AddModuleVersionInfo("War Sails", NavalVersion.GetApplicationVersionBuildNumber());
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000020E6 File Offset: 0x000002E6
		protected override void OnSubModuleUnloaded()
		{
			base.OnSubModuleUnloaded();
			SpriteCategory fullBackgroundsCategory = this._fullBackgroundsCategory;
			if (fullBackgroundsCategory != null)
			{
				fullBackgroundsCategory.Unload();
			}
			GauntletGameVersionView.RemoveModuleVersionInfo("War Sails");
		}

		// Token: 0x04000001 RID: 1
		private const int NumberOfWaitFramesToLoad = 5;

		// Token: 0x04000002 RID: 2
		private bool _initializedLoadingCategory;

		// Token: 0x04000003 RID: 3
		private bool _loadBackgroundCategory;

		// Token: 0x04000004 RID: 4
		private int _frameCounterToLoad;

		// Token: 0x04000005 RID: 5
		private SpriteCategory _fullBackgroundsCategory;
	}
}
