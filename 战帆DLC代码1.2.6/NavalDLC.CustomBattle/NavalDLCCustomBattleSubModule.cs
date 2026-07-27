using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.CustomBattle;

namespace NavalDLC.CustomBattle
{
	// Token: 0x02000009 RID: 9
	public class NavalDLCCustomBattleSubModule : MBSubModuleBase
	{
		// Token: 0x06000051 RID: 81 RVA: 0x000037E5 File Offset: 0x000019E5
		protected override void OnSubModuleLoad()
		{
			base.OnSubModuleLoad();
			CustomBattleFactory.RegisterProvider<NavalCustomBattleProvider>();
			TauntUsageManager.Initialize();
		}
	}
}
