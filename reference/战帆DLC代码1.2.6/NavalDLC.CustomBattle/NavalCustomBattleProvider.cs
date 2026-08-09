using System;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.CustomBattle;

namespace NavalDLC.CustomBattle
{
	// Token: 0x02000004 RID: 4
	public class NavalCustomBattleProvider : ICustomBattleProvider
	{
		// Token: 0x0600002C RID: 44 RVA: 0x00002F6D File Offset: 0x0000116D
		public void StartCustomBattle()
		{
			MBGameManager.StartNewGame(new NavalCustomGameManager());
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002F79 File Offset: 0x00001179
		public TextObject GetName()
		{
			return new TextObject("{=Q8gbZIiM}Naval Custom Battle", null);
		}
	}
}
