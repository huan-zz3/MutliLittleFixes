using System;
using System.IO;
using MissionSharedLibrary.Config;
using MissionSharedLibrary.Config.HotKey;
using MissionSharedLibrary.Utilities;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace RTSCamera.CommandSystem.Config.HotKey
{
	// Token: 0x02000095 RID: 149
	public class CommandSystemGameKeyConfig : GameKeyConfigBase<CommandSystemGameKeyConfig>
	{
		// Token: 0x170000CF RID: 207
		// (get) Token: 0x0600055F RID: 1375 RVA: 0x0001FFED File Offset: 0x0001E1ED
		protected override string SaveName { get; } = Path.Combine(ConfigPath.ConfigDir, "RTSCamera", "CommandSystemGameKeyConfig.xml");

		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x06000560 RID: 1376 RVA: 0x0001FFF5 File Offset: 0x0001E1F5
		protected static Version BinaryVersion
		{
			get
			{
				return new Version(1, 1);
			}
		}

		// Token: 0x06000561 RID: 1377 RVA: 0x0001FFFE File Offset: 0x0001E1FE
		protected override void CopyFrom(CommandSystemGameKeyConfig other)
		{
			base.CopyFrom(other);
			this.ConfigVersion = other.ConfigVersion;
		}

		// Token: 0x06000562 RID: 1378 RVA: 0x00020014 File Offset: 0x0001E214
		protected override void UpgradeToCurrentVersion()
		{
			if (!(this.ConfigVersion == "1.1"))
			{
				Utility.DisplayMessage(Module.CurrentModule.GlobalTextManager.FindText("str_mission_library_hotkey_config_incompatible", null).ToString(), new Color(1f, 0f, 0f, 1f));
				base.ResetToDefault();
				this.Serialize();
			}
			this.ConfigVersion = CommandSystemGameKeyConfig.BinaryVersion.ToString(2);
		}

		// Token: 0x0400029C RID: 668
		public string ConfigVersion = CommandSystemGameKeyConfig.BinaryVersion.ToString(2);
	}
}
