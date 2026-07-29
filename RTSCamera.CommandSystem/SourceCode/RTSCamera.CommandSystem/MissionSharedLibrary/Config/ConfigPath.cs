using System;
using System.IO;

namespace MissionSharedLibrary.Config
{
	// Token: 0x0200003C RID: 60
	public static class ConfigPath
	{
		// Token: 0x17000075 RID: 117
		// (get) Token: 0x0600020F RID: 527 RVA: 0x00007BD0 File Offset: 0x00005DD0
		public static string ConfigDir { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), ConfigPath.ApplicationName, "Configs");

		// Token: 0x040000D7 RID: 215
		private static string ApplicationName = "Mount and Blade II Bannerlord";
	}
}
