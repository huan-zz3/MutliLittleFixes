using System;
using Helpers;
using NavalDLC.View.GameMenus;
using NavalDLC.ViewModelCollection;
using SandBox.View.Map;
using SandBox.View.Menu;
using SandBox.ViewModelCollection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View.Screens;

namespace NavalDLC.View.Map
{
	// Token: 0x02000032 RID: 50
	[GameStateScreen(typeof(MapState))]
	public class NavalMapScreen : MapScreen
	{
		// Token: 0x0600013B RID: 315 RVA: 0x0000922D File Offset: 0x0000742D
		public NavalMapScreen(MapState mapState)
			: base(mapState)
		{
		}

		// Token: 0x0600013C RID: 316 RVA: 0x00009238 File Offset: 0x00007438
		protected override bool TickNavigationInput(float dt)
		{
			if (base.TickNavigationInput(dt))
			{
				return true;
			}
			if (base.SceneLayer.Input.IsGameKeyPressed(45) && base.NavigationHandler.GetElement("manage_fleet").Permission.IsAuthorized)
			{
				this.OpenManageFleet();
				return true;
			}
			return false;
		}

		// Token: 0x0600013D RID: 317 RVA: 0x0000928C File Offset: 0x0000748C
		protected override SPScoreboardVM CreateSimulationScoreboardDatasource(BattleSimulation battleSimulation)
		{
			MapEvent mapEvent = battleSimulation.MapEvent;
			if ((mapEvent != null && mapEvent.IsNavalMapEvent) || MapEventHelper.IsNavalRaid(battleSimulation.MapEvent))
			{
				return NavalScoreboardVM.CreateSimulation(battleSimulation);
			}
			return base.CreateSimulationScoreboardDatasource(battleSimulation);
		}

		// Token: 0x0600013E RID: 318 RVA: 0x000092BD File Offset: 0x000074BD
		protected override MenuViewContext CreateMenuViewContext(MenuContext menuContext)
		{
			return new NavalMenuViewContext(this, menuContext);
		}

		// Token: 0x0600013F RID: 319 RVA: 0x000092C6 File Offset: 0x000074C6
		private void OpenManageFleet()
		{
			if (Hero.MainHero != null && !Hero.MainHero.IsPrisoner && !Hero.MainHero.IsDead)
			{
				PortStateHelper.OpenAsManageFleet(new MBReadOnlyList<Ship>());
			}
		}
	}
}
