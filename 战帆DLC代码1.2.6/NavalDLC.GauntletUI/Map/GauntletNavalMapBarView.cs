using System;
using NavalDLC.View.Map.Navigation;
using NavalDLC.ViewModelCollection.Map.MapBar;
using SandBox.GauntletUI.Map;
using SandBox.View.Map;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.ScreenSystem;

namespace NavalDLC.GauntletUI.Map
{
	// Token: 0x02000021 RID: 33
	[OverrideView(typeof(MapBarView))]
	public class GauntletNavalMapBarView : GauntletMapBarView
	{
		// Token: 0x06000100 RID: 256 RVA: 0x0000A13D File Offset: 0x0000833D
		protected override void CreateLayout()
		{
			this._mapBarGlobalLayer = new GauntletNavalMapBarGlobalLayer(base.MapScreen, new NavalMapNavigationHandler(), 8.5f);
			this._mapBarGlobalLayer.Initialize(new NavalMapBarVM());
			ScreenManager.AddGlobalLayer(this._mapBarGlobalLayer, true);
		}
	}
}
