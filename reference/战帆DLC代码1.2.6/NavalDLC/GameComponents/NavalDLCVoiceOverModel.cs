using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.Localization;

namespace NavalDLC.GameComponents
{
	// Token: 0x0200013C RID: 316
	public class NavalDLCVoiceOverModel : VoiceOverModel
	{
		// Token: 0x0600153F RID: 5439 RVA: 0x00095868 File Offset: 0x00093A68
		public override string GetSoundPathForCharacter(CharacterObject character, VoiceObject voiceObject)
		{
			return base.BaseModel.GetSoundPathForCharacter(character, voiceObject);
		}

		// Token: 0x06001540 RID: 5440 RVA: 0x00095877 File Offset: 0x00093A77
		public override string GetAccentClass(CultureObject culture, bool isHighClass)
		{
			if (culture.StringId == "nord")
			{
				return "nord";
			}
			if (culture.StringId == "southern_pirates")
			{
				return "southern_pirates";
			}
			return base.BaseModel.GetAccentClass(culture, isHighClass);
		}

		// Token: 0x04000B0F RID: 2831
		private const string NordClass = "nord";

		// Token: 0x04000B10 RID: 2832
		private const string CultureSouthernPirates = "southern_pirates";

		// Token: 0x04000B11 RID: 2833
		private const string SouthernPiratesClass = "southern_pirates";
	}
}
