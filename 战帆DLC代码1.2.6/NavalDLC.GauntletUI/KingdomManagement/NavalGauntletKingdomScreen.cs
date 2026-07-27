using System;
using NavalDLC.ViewModelCollection.Kingdom;
using SandBox.GauntletUI;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement;
using TaleWorlds.MountAndBlade.View.Screens;

namespace NavalDLC.GauntletUI.KingdomManagement
{
	// Token: 0x02000023 RID: 35
	[GameStateScreen(typeof(KingdomState))]
	public class NavalGauntletKingdomScreen : GauntletKingdomScreen
	{
		// Token: 0x06000104 RID: 260 RVA: 0x0000A1CC File Offset: 0x000083CC
		public NavalGauntletKingdomScreen(KingdomState kingdomState)
			: base(kingdomState)
		{
		}

		// Token: 0x06000105 RID: 261 RVA: 0x0000A1D5 File Offset: 0x000083D5
		protected override KingdomManagementVM CreateDataSource()
		{
			return new NavalKingdomManagementVM(new Action(base.CloseKingdomScreen), new Action(base.OpenArmyManagement), new Action<Army>(base.ShowArmyOnMap));
		}
	}
}
