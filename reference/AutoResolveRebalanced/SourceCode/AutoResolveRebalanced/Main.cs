using System;
using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace AutoResolveRebalanced
{
	// Token: 0x02000011 RID: 17
	public class Main : MBSubModuleBase
	{
		// Token: 0x06000092 RID: 146 RVA: 0x0000357E File Offset: 0x0000177E
		protected override void OnSubModuleLoad()
		{
			base.OnSubModuleLoad();
			new Harmony("CIMO.AutoResolveRebalanced").PatchAll();
			InformationManager.DisplayMessage(new InformationMessage("Auto Resolve Rebalanced installed."));
		}

		// Token: 0x06000093 RID: 147 RVA: 0x000035A4 File Offset: 0x000017A4
		protected override void OnBeforeInitialModuleScreenSetAsRoot()
		{
			base.OnBeforeInitialModuleScreenSetAsRoot();
		}

		// Token: 0x06000094 RID: 148 RVA: 0x000035AC File Offset: 0x000017AC
		protected override void OnGameStart(Game game, IGameStarter gameStarter)
		{
			base.OnGameStart(game, gameStarter);
			SimulateDataDict.ClearData();
		}

		// Token: 0x06000095 RID: 149 RVA: 0x000035BB File Offset: 0x000017BB
		public override void OnGameEnd(Game game)
		{
			base.OnGameEnd(game);
			SimulateDataDict.ClearData();
		}
	}
}
