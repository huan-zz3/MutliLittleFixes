using System;
using NavalDLC.ViewModelCollection.ClanManagement;
using SandBox.GauntletUI;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement;
using TaleWorlds.MountAndBlade.View.Screens;

namespace NavalDLC.GauntletUI.Clan
{
	// Token: 0x02000024 RID: 36
	[GameStateScreen(typeof(ClanState))]
	public class NavalGauntletClanScreen : GauntletClanScreen
	{
		// Token: 0x06000106 RID: 262 RVA: 0x0000A200 File Offset: 0x00008400
		public NavalGauntletClanScreen(ClanState clanState)
			: base(clanState)
		{
		}

		// Token: 0x06000107 RID: 263 RVA: 0x0000A209 File Offset: 0x00008409
		protected override ClanManagementVM CreateDataSource()
		{
			return new NavalClanManagementVM(new Action(base.CloseClanScreen), new Action<Hero>(base.ShowHeroOnMap), new Action<Hero>(base.OpenPartyScreenForNewClanParty), new Action(base.OpenBannerEditorWithPlayerClan));
		}
	}
}
