using System;
using SandBox.GauntletUI;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.TwoDimension;

namespace NavalDLC.GauntletUI.CharacterDeveloper
{
	// Token: 0x02000025 RID: 37
	[GameStateScreen(typeof(CharacterDeveloperState))]
	public class GauntletNavalCharacterDeveloperScreen : GauntletCharacterDeveloperScreen
	{
		// Token: 0x06000108 RID: 264 RVA: 0x0000A240 File Offset: 0x00008440
		public GauntletNavalCharacterDeveloperScreen(CharacterDeveloperState clanState)
			: base(clanState)
		{
			this._navalSpriteCategory = UIResourceManager.GetSpriteCategory("ui_naval_character_developer");
		}

		// Token: 0x06000109 RID: 265 RVA: 0x0000A259 File Offset: 0x00008459
		protected override void OnActivate()
		{
			base.OnActivate();
			Extensions.Load(this._navalSpriteCategory);
		}

		// Token: 0x0600010A RID: 266 RVA: 0x0000A26C File Offset: 0x0000846C
		protected override void OnDeactivate()
		{
			base.OnDeactivate();
			this._navalSpriteCategory.Unload();
		}

		// Token: 0x04000091 RID: 145
		private SpriteCategory _navalSpriteCategory;
	}
}
