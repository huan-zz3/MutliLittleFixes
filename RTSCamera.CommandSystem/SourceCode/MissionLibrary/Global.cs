using System;
using System.Collections.Generic;
using MissionLibrary.Provider;

namespace MissionLibrary
{
	// Token: 0x02000002 RID: 2
	public static class Global
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		// (set) Token: 0x06000002 RID: 2 RVA: 0x00002057 File Offset: 0x00000257
		private static bool IsInitialized { get; set; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000003 RID: 3 RVA: 0x0000205F File Offset: 0x0000025F
		// (set) Token: 0x06000004 RID: 4 RVA: 0x00002066 File Offset: 0x00000266
		private static bool IsThirdInitialized { get; set; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000005 RID: 5 RVA: 0x0000206E File Offset: 0x0000026E
		// (set) Token: 0x06000006 RID: 6 RVA: 0x00002075 File Offset: 0x00000275
		private static ProviderManager ProviderManager { get; set; }

		// Token: 0x06000007 RID: 7 RVA: 0x0000207D File Offset: 0x0000027D
		public static void Initialize()
		{
			if (Global.IsInitialized)
			{
				return;
			}
			Global.IsInitialized = true;
			Global.ProviderManager = new ProviderManager();
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002097 File Offset: 0x00000297
		public static void ThirdInitialize()
		{
			if (Global.IsThirdInitialized)
			{
				return;
			}
			Global.IsThirdInitialized = true;
			Global.ProviderManager.InstantiateAll();
		}

		// Token: 0x06000009 RID: 9 RVA: 0x000020B1 File Offset: 0x000002B1
		public static void RegisterInstance<T>(IVersionProvider<T> newProvider, string key = "") where T : ATag<T>
		{
			Global.ProviderManager.RegisterInstance<T>(newProvider, key);
		}

		// Token: 0x0600000A RID: 10 RVA: 0x000020BF File Offset: 0x000002BF
		public static T GetInstance<T>(string key = "") where T : ATag<T>
		{
			return Global.ProviderManager.GetInstance<T>(key);
		}

		// Token: 0x0600000B RID: 11 RVA: 0x000020CC File Offset: 0x000002CC
		public static IEnumerable<T> GetInstances<T>() where T : ATag<T>
		{
			return Global.ProviderManager.GetInstances<T>();
		}

		// Token: 0x0600000C RID: 12 RVA: 0x000020D8 File Offset: 0x000002D8
		public static void Clear()
		{
			if (!Global.IsInitialized)
			{
				return;
			}
			Global.IsInitialized = false;
			Global.IsThirdInitialized = false;
			Global.ProviderManager = null;
		}
	}
}
