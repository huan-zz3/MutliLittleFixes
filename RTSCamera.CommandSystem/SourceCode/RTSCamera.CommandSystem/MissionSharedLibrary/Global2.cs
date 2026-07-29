using System;
using System.Collections.Generic;
using MissionLibrary;
using MissionLibrary.Provider;

namespace MissionSharedLibrary
{
	// Token: 0x02000005 RID: 5
	public static class Global2
	{
		// Token: 0x06000023 RID: 35 RVA: 0x000024D0 File Offset: 0x000006D0
		public static void Initialize()
		{
			Global.Initialize();
		}

		// Token: 0x06000024 RID: 36 RVA: 0x000024D7 File Offset: 0x000006D7
		public static void ThirdInitialize()
		{
			Global.ThirdInitialize();
		}

		// Token: 0x06000025 RID: 37 RVA: 0x000024DE File Offset: 0x000006DE
		public static void RegisterInstance<T>(IVersionProvider<T> newProvider, string key = "") where T : ATag<T>
		{
			Global.RegisterInstance<T>(newProvider, key);
		}

		// Token: 0x06000026 RID: 38 RVA: 0x000024E7 File Offset: 0x000006E7
		public static T GetInstance<T>(string key = "") where T : ATag<T>
		{
			return Global.GetInstance<T>(key);
		}

		// Token: 0x06000027 RID: 39 RVA: 0x000024EF File Offset: 0x000006EF
		public static IEnumerable<T> GetInstances<T>() where T : ATag<T>
		{
			return Global.GetInstances<T>();
		}

		// Token: 0x06000028 RID: 40 RVA: 0x000024F6 File Offset: 0x000006F6
		public static void Clear()
		{
			Global.Clear();
		}
	}
}
