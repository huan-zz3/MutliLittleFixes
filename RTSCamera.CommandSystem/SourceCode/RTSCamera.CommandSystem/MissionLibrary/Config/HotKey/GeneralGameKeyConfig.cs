using System;
using System.IO;
using MissionSharedLibrary.Config;
using MissionSharedLibrary.Config.HotKey;
using MissionSharedLibrary.Utilities;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace MissionLibrary.Config.HotKey
{
	// Token: 0x02000004 RID: 4
	public class GeneralGameKeyConfig : GameKeyConfigBase<GeneralGameKeyConfig>
	{
		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600001E RID: 30 RVA: 0x00002401 File Offset: 0x00000601
		protected override string SaveName { get; } = Path.Combine(ConfigPath.ConfigDir, "MissionLibrary", "GeneralGameKeyConfig.xml");

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600001F RID: 31 RVA: 0x00002409 File Offset: 0x00000609
		protected static Version BinaryVersion
		{
			get
			{
				return new Version(1, 1);
			}
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002412 File Offset: 0x00000612
		protected override void CopyFrom(GeneralGameKeyConfig other)
		{
			base.CopyFrom(other);
			this.ConfigVersion = other.ConfigVersion;
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00002428 File Offset: 0x00000628
		protected override void UpgradeToCurrentVersion()
		{
			if (!(this.ConfigVersion == "1.1"))
			{
				Utility.DisplayMessage(Module.CurrentModule.GlobalTextManager.FindText("str_mission_library_hotkey_config_incompatible", null).ToString(), new Color(1f, 0f, 0f, 1f));
				base.ResetToDefault();
				this.Serialize();
			}
			this.ConfigVersion = GeneralGameKeyConfig.BinaryVersion.ToString(2);
		}

		// Token: 0x0400000D RID: 13
		public string ConfigVersion = GeneralGameKeyConfig.BinaryVersion.ToString(2);
	}
}
