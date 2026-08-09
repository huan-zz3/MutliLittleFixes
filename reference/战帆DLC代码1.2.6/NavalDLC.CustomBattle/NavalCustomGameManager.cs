using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.CustomBattle
{
	// Token: 0x02000008 RID: 8
	public class NavalCustomGameManager : MBGameManager
	{
		// Token: 0x0600004D RID: 77 RVA: 0x000036E0 File Offset: 0x000018E0
		protected override void DoLoadingForGameManager(GameManagerLoadingSteps gameManagerLoadingStep, out GameManagerLoadingSteps nextStep)
		{
			nextStep = -1;
			switch (gameManagerLoadingStep)
			{
			case 0:
				MBGameManager.LoadModuleData(false);
				MBGlobals.InitializeReferences();
				Game.CreateGame(new NavalCustomGame(), this).DoLoading();
				nextStep = 1;
				return;
			case 1:
			{
				bool flag = true;
				foreach (MBSubModuleBase mbsubModuleBase in Module.CurrentModule.CollectSubModules())
				{
					flag = flag && mbsubModuleBase.DoLoading(Game.Current);
				}
				nextStep = (flag ? 2 : 1);
				return;
			}
			case 2:
				MBGameManager.StartNewGame();
				nextStep = 3;
				return;
			case 3:
				nextStep = (Game.Current.DoLoading() ? 4 : 3);
				return;
			case 4:
				nextStep = 5;
				return;
			case 5:
				nextStep = -1;
				return;
			default:
				return;
			}
		}

		// Token: 0x0600004E RID: 78 RVA: 0x000037B4 File Offset: 0x000019B4
		public override void OnAfterCampaignStart(Game game)
		{
		}

		// Token: 0x0600004F RID: 79 RVA: 0x000037B6 File Offset: 0x000019B6
		public override void OnLoadFinished()
		{
			base.OnLoadFinished();
			Game.Current.GameStateManager.CleanAndPushState(Game.Current.GameStateManager.CreateState<NavalCustomBattleState>(), 0);
		}
	}
}
