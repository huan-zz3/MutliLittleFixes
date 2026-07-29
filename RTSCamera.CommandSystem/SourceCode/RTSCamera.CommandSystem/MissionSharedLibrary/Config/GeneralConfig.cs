using System;
using System.IO;
using System.Xml.Serialization;
using MissionSharedLibrary.Utilities;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace MissionSharedLibrary.Config
{
	// Token: 0x0200003B RID: 59
	public class GeneralConfig : MissionConfigBase<GeneralConfig>
	{
		// Token: 0x17000073 RID: 115
		// (get) Token: 0x06000209 RID: 521 RVA: 0x00007AD0 File Offset: 0x00005CD0
		protected static Version BinaryVersion
		{
			get
			{
				return new Version(1, 1);
			}
		}

		// Token: 0x0600020A RID: 522 RVA: 0x00007AD9 File Offset: 0x00005CD9
		public static void OnMenuClosed()
		{
			MissionConfigBase<GeneralConfig>.Get().Serialize();
		}

		// Token: 0x0600020B RID: 523 RVA: 0x00007AE6 File Offset: 0x00005CE6
		protected override void CopyFrom(GeneralConfig other)
		{
			this.ConfigVersion = other.ConfigVersion;
			this.PreviouslySelectedOptionClassId = other.PreviouslySelectedOptionClassId;
			this.HasUsageShown = other.HasUsageShown;
		}

		// Token: 0x0600020C RID: 524 RVA: 0x00007B0C File Offset: 0x00005D0C
		protected override void UpgradeToCurrentVersion()
		{
			string configVersion = this.ConfigVersion;
			if (!(configVersion == "1.0"))
			{
				if (configVersion == "1.1")
				{
					goto IL_006D;
				}
				Utility.DisplayMessage(Module.CurrentModule.GlobalTextManager.FindText("str_mission_library_config_incompatible", null).ToString(), new Color(1f, 0f, 0f, 1f));
				base.ResetToDefault();
				this.Serialize();
			}
			this.HasUsageShown = false;
			IL_006D:
			this.ConfigVersion = GeneralConfig.BinaryVersion.ToString(2);
		}

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x0600020D RID: 525 RVA: 0x00007B97 File Offset: 0x00005D97
		[XmlIgnore]
		protected override string SaveName
		{
			get
			{
				return Path.Combine(ConfigPath.ConfigDir, "MissionLibrary", "GeneralConfig.xml");
			}
		}

		// Token: 0x040000D4 RID: 212
		public string ConfigVersion = GeneralConfig.BinaryVersion.ToString();

		// Token: 0x040000D5 RID: 213
		public string PreviouslySelectedOptionClassId = "RTSCamera";

		// Token: 0x040000D6 RID: 214
		public bool HasUsageShown;
	}
}
