using System;
using MissionLibrary.Provider;
using MissionSharedLibrary.Provider;
using MissionSharedLibrary.Utilities;

namespace MissionSharedLibrary
{
	// Token: 0x02000007 RID: 7
	public class Initializer
	{
		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600002E RID: 46 RVA: 0x00002688 File Offset: 0x00000888
		// (set) Token: 0x0600002F RID: 47 RVA: 0x0000268F File Offset: 0x0000088F
		public static bool IsInitialized { get; private set; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000030 RID: 48 RVA: 0x00002697 File Offset: 0x00000897
		// (set) Token: 0x06000031 RID: 49 RVA: 0x0000269E File Offset: 0x0000089E
		public static bool IsInstancesRegisteredFromVersionManager { get; private set; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000032 RID: 50 RVA: 0x000026A6 File Offset: 0x000008A6
		// (set) Token: 0x06000033 RID: 51 RVA: 0x000026AD File Offset: 0x000008AD
		public static bool IsSecondInitialized { get; private set; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000034 RID: 52 RVA: 0x000026B5 File Offset: 0x000008B5
		// (set) Token: 0x06000035 RID: 53 RVA: 0x000026BC File Offset: 0x000008BC
		public static bool IsThirdInitialized { get; private set; }

		// Token: 0x06000036 RID: 54 RVA: 0x000026C4 File Offset: 0x000008C4
		public static bool Initialize(string moduleId)
		{
			if (Initializer.IsInitialized)
			{
				return false;
			}
			Initializer.IsInitialized = true;
			Utility.ModuleId = moduleId;
			Global2.Initialize();
			Initializer.RegisterVersionManager();
			return true;
		}

		// Token: 0x06000037 RID: 55 RVA: 0x000026E6 File Offset: 0x000008E6
		public static void OnApplicationTick(float dt)
		{
			if (!Initializer.IsInitialized || Initializer.IsSecondInitialized)
			{
				return;
			}
			Initializer.SecondInitialize();
		}

		// Token: 0x06000038 RID: 56 RVA: 0x000026FD File Offset: 0x000008FD
		private static bool SecondInitialize()
		{
			if (Initializer.IsSecondInitialized)
			{
				return false;
			}
			Initializer.IsSecondInitialized = true;
			Initializer.RegisterInstancesFromVersionManager();
			return true;
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00002714 File Offset: 0x00000914
		public static bool ThirdInitialize()
		{
			if (Initializer.IsThirdInitialized)
			{
				return false;
			}
			Initializer.IsThirdInitialized = true;
			Global2.ThirdInitialize();
			return true;
		}

		// Token: 0x0600003A RID: 58 RVA: 0x0000272B File Offset: 0x0000092B
		public static void Clear()
		{
			Global2.Clear();
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00002732 File Offset: 0x00000932
		private static void RegisterVersionManager()
		{
			MissionLibraryVersionManager.RegisterSelf();
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00002739 File Offset: 0x00000939
		private static void RegisterInstancesFromVersionManager()
		{
			MissionLibraryVersionManager.RegisterInstances();
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00002740 File Offset: 0x00000940
		public static void RegisterProvider<T>(Func<ATag<T>> creator, Version providerVersion, string key = "") where T : ATag<T>
		{
			Global2.RegisterInstance<T>(VersionProviderCreator.Create<T>(creator, providerVersion), key);
		}
	}
}
