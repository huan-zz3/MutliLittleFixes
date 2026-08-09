using System;
using NavalDLC.View.Map.Navigation;
using SandBox.GauntletUI.Map;
using SandBox.View.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.InputSystem;

namespace NavalDLC.GauntletUI.Map
{
	// Token: 0x02000022 RID: 34
	public class GauntletNavalMapBarGlobalLayer : GauntletMapBarGlobalLayer
	{
		// Token: 0x06000102 RID: 258 RVA: 0x0000A17E File Offset: 0x0000837E
		public GauntletNavalMapBarGlobalLayer(MapScreen mapScreen, INavigationHandler navigationHandler, float contextAlphaModifider)
			: base(mapScreen, navigationHandler, contextAlphaModifider)
		{
			this._manageFleetNavigationElement = (navigationHandler as NavalMapNavigationHandler).ManageFleetNavigationElement;
		}

		// Token: 0x06000103 RID: 259 RVA: 0x0000A19A File Offset: 0x0000839A
		protected override bool HandlePanelSwitchingInput(InputContext inputContext)
		{
			if (base.HandlePanelSwitchingInput(inputContext))
			{
				return true;
			}
			if (inputContext.IsGameKeyReleased(45) && !this._manageFleetNavigationElement.IsActive)
			{
				this._manageFleetNavigationElement.OpenView();
				return true;
			}
			return false;
		}

		// Token: 0x04000090 RID: 144
		private readonly ManageFleetNavigationElement _manageFleetNavigationElement;
	}
}
