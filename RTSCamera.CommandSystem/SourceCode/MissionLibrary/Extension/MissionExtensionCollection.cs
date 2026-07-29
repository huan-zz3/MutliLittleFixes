using System;
using System.Collections.Generic;

namespace MissionLibrary.Extension
{
	// Token: 0x02000020 RID: 32
	public class MissionExtensionCollection
	{
		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000075 RID: 117 RVA: 0x0000262F File Offset: 0x0000082F
		public static List<IMissionExtension> Extensions { get; } = new List<IMissionExtension>();

		// Token: 0x06000076 RID: 118 RVA: 0x00002636 File Offset: 0x00000836
		public static void AddExtension(IMissionExtension extension)
		{
			MissionExtensionCollection.Extensions.Add(extension);
		}

		// Token: 0x06000077 RID: 119 RVA: 0x00002643 File Offset: 0x00000843
		public static void Clear()
		{
			MissionExtensionCollection.Extensions.Clear();
		}
	}
}
