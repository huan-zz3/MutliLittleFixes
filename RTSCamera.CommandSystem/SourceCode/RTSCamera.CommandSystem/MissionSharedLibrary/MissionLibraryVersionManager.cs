using System;
using MissionLibrary.Controller;
using MissionLibrary.Controller.Camera;
using MissionLibrary.HotKey;
using MissionLibrary.Provider;
using MissionLibrary.Usage;
using MissionLibrary.View;
using MissionSharedLibrary.Controller;
using MissionSharedLibrary.Controller.Camera;
using MissionSharedLibrary.HotKey;
using MissionSharedLibrary.Provider;
using MissionSharedLibrary.Usage;
using MissionSharedLibrary.View;

namespace MissionSharedLibrary
{
	// Token: 0x02000006 RID: 6
	public class MissionLibraryVersionManager : AResourceCreator
	{
		// Token: 0x06000029 RID: 41 RVA: 0x000024FD File Offset: 0x000006FD
		public static void RegisterInstances()
		{
			Global2.GetInstance<AResourceCreator>("MissionLibraryVersionManager");
		}

		// Token: 0x0600002A RID: 42 RVA: 0x0000250A File Offset: 0x0000070A
		public static void RegisterSelf()
		{
			MissionLibraryVersionManager.RegisterInstance<AResourceCreator>(() => new MissionLibraryVersionManager(), new Version(1, 9), "MissionLibraryVersionManager");
		}

		// Token: 0x0600002B RID: 43 RVA: 0x0000253D File Offset: 0x0000073D
		public MissionLibraryVersionManager()
		{
			this.RegisterProviders();
		}

		// Token: 0x0600002C RID: 44 RVA: 0x0000254C File Offset: 0x0000074C
		private void RegisterProviders()
		{
			MissionLibraryVersionManager.RegisterInstance<AGameKeyCategoryManager>(() => new GameKeyCategoryManager(), new Version(2, 0), "");
			MissionLibraryVersionManager.RegisterInstance<ACameraControllerManager>(() => new CameraControllerManager(), new Version(2, 0), "");
			MissionLibraryVersionManager.RegisterInstance<AMissionStartingManager>(() => new MissionStartingManager(), new Version(2, 0), "");
			MissionLibraryVersionManager.RegisterInstance<AMenuManager>(() => new MenuManager(), new Version(2, 1), "");
			MissionLibraryVersionManager.RegisterInstance<AUsageCategoryManager>(() => new UsageCategoryManager(), new Version(2, 0), "");
			MissionLibraryVersionManager.RegisterInstance<AResourceCreator>(() => new GeneralResourceCreator(), new Version(2, 1), "");
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002679 File Offset: 0x00000879
		private static void RegisterInstance<T>(Func<ATag<T>> creator, Version providerVersion, string key = "") where T : ATag<T>
		{
			Global2.RegisterInstance<T>(VersionProviderCreator.Create<T>(creator, providerVersion), key);
		}
	}
}
