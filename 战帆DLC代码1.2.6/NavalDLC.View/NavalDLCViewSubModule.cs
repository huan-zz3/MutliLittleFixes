using System;
using NavalDLC.HotKeyCategories;
using NavalDLC.View.Map;
using NavalDLC.View.Map.Managers;
using NavalDLC.View.Missions;
using NavalDLC.View.Overlay;
using NavalDLC.View.Permissions;
using NavalDLC.View.VisualOrders;
using SandBox;
using SandBox.View;
using SandBox.View.Map;
using SandBox.ViewModelCollection.Missions.NameMarker;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.Overlay;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order.Visual;
using TaleWorlds.ScreenSystem;

namespace NavalDLC.View
{
	// Token: 0x02000007 RID: 7
	public class NavalDLCViewSubModule : MBSubModuleBase
	{
		// Token: 0x06000033 RID: 51 RVA: 0x0000313C File Offset: 0x0000133C
		protected override void OnSubModuleLoad()
		{
			base.OnSubModuleLoad();
			this.RegisterHotKeyContexts();
			this.RegisterTooltipTypes();
			this._raidVisualOrderProvider = new NavalRaidVisualOrderProvider();
			this._shipVisualOrderProvider = new NavalShipVisualOrderProvider();
			this._troopVisualOrderProvider = new NavalTroopVisualOrderProvider();
			VisualOrderFactory.RegisterProvider(this._raidVisualOrderProvider);
			VisualOrderFactory.RegisterProvider(this._shipVisualOrderProvider);
			VisualOrderFactory.RegisterProvider(this._troopVisualOrderProvider);
			this._gameMenuOverlayProvider = new NavalGameMenuOverlayProvider();
			GameMenuOverlayFactory.RegisterProvider(this._gameMenuOverlayProvider);
			MissionNameMarkerFactory.DefaultContext.AddProvider<NavalMissionNameMarkerProvider>();
			ScreenManager.OnPushScreen += new ScreenManager.OnPushScreenEvent(this.OnScreenPushed);
		}

		// Token: 0x06000034 RID: 52 RVA: 0x000031CE File Offset: 0x000013CE
		public override void OnNewGameCreated(Game game, object initializerObject)
		{
			if (game.GameType is Campaign)
			{
				NavalDLCManager.Instance.NavalMapSceneWrapper = new NavalMapSceneWrapper();
			}
		}

		// Token: 0x06000035 RID: 53 RVA: 0x000031EC File Offset: 0x000013EC
		public override void OnAfterGameLoaded(Game game)
		{
			if (game.GameType is Campaign)
			{
				NavalDLCManager.Instance.NavalMapSceneWrapper = new NavalMapSceneWrapper();
			}
		}

		// Token: 0x06000036 RID: 54 RVA: 0x0000320A File Offset: 0x0000140A
		public override void OnGameInitializationFinished(Game game)
		{
			base.OnGameInitializationFinished(game);
			NavalPermissionsSystem.OnInitialize();
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00003218 File Offset: 0x00001418
		public override void OnGameEnd(Game game)
		{
			base.OnGameEnd(game);
			NavalPermissionsSystem.OnUnload();
			VisualShipFactory.DeregisterVisualShipCache();
		}

		// Token: 0x06000038 RID: 56 RVA: 0x0000322C File Offset: 0x0000142C
		protected override void OnSubModuleUnloaded()
		{
			base.OnSubModuleUnloaded();
			this.UnregisterTooltipTypes();
			VisualOrderFactory.UnregisterProvider(this._raidVisualOrderProvider);
			VisualOrderFactory.UnregisterProvider(this._shipVisualOrderProvider);
			VisualOrderFactory.UnregisterProvider(this._troopVisualOrderProvider);
			GameMenuOverlayFactory.UnregisterProvider(this._gameMenuOverlayProvider);
			ScreenManager.OnPushScreen -= new ScreenManager.OnPushScreenEvent(this.OnScreenPushed);
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00003282 File Offset: 0x00001482
		public override void OnSubModuleDeactivated()
		{
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00003284 File Offset: 0x00001484
		public override void OnSubModuleActivated()
		{
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00003288 File Offset: 0x00001488
		private void RegisterTooltipTypes()
		{
			InformationManager.RegisterTooltip<Ship, PropertyBasedTooltipVM>(new Action<PropertyBasedTooltipVM, object[]>(NavalTooltipRefresherCollection.RefreshShipTooltip), "PropertyBasedTooltip");
			InformationManager.RegisterTooltip<ShipHull, PropertyBasedTooltipVM>(new Action<PropertyBasedTooltipVM, object[]>(NavalTooltipRefresherCollection.RefreshShipHullTooltip), "PropertyBasedTooltip");
			InformationManager.RegisterTooltip<ShipUpgradePiece, PropertyBasedTooltipVM>(new Action<PropertyBasedTooltipVM, object[]>(NavalTooltipRefresherCollection.RefreshShipPieceTooltip), "PropertyBasedTooltip");
			InformationManager.RegisterTooltip<Figurehead, PropertyBasedTooltipVM>(new Action<PropertyBasedTooltipVM, object[]>(NavalTooltipRefresherCollection.RefreshFigureheadTooltip), "PropertyBasedTooltip");
			InformationManager.RegisterTooltip<AnchorPoint, PropertyBasedTooltipVM>(new Action<PropertyBasedTooltipVM, object[]>(NavalTooltipRefresherCollection.RefreshAnchorPointTooltip), "PropertyBasedTooltip");
			InformationManager.RegisterTooltip<Settlement, PropertyBasedTooltipVM>(new Action<PropertyBasedTooltipVM, object[]>(NavalTooltipRefresherCollection.RefreshSettlementTooltip), "PropertyBasedTooltip");
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00003319 File Offset: 0x00001519
		private void UnregisterTooltipTypes()
		{
			InformationManager.UnregisterTooltip<Ship>();
			InformationManager.UnregisterTooltip<ShipHull>();
			InformationManager.UnregisterTooltip<ShipUpgradePiece>();
			InformationManager.UnregisterTooltip<Figurehead>();
			InformationManager.UnregisterTooltip<AnchorPoint>();
			InformationManager.UnregisterTooltip<Settlement>();
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00003339 File Offset: 0x00001539
		private void RegisterHotKeyContexts()
		{
			HotKeyManager.RegisterContext(new NavalShipControlsHotKeyCategory(), false);
			HotKeyManager.RegisterContext(new PortHotKeyCategory(), false);
			HotKeyManager.RegisterContext(new NavalCheatsHotKeyCategory(), true);
		}

		// Token: 0x0600003E RID: 62 RVA: 0x0000335C File Offset: 0x0000155C
		private void OnScreenPushed(ScreenBase pushedScreen)
		{
			MapScreen mapScreen;
			if ((mapScreen = pushedScreen as MapScreen) != null)
			{
				mapScreen.AddMapView<NavalMapAnchorTrackerView>(Array.Empty<object>());
			}
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00003380 File Offset: 0x00001580
		public override void OnAfterGameInitializationFinished(Game game, object starterObject)
		{
			base.OnAfterGameInitializationFinished(game, starterObject);
			if (Campaign.Current != null && Campaign.Current.MapSceneWrapper != null)
			{
				VisualShipFactory.InitializeShipEntityCache(((MapScene)Campaign.Current.MapSceneWrapper).Scene);
				SandBoxViewSubModule.SandBoxViewVisualManager.AddEntityComponent<NavalMobilePartyVisualManager>();
				SandBoxViewSubModule.SandBoxViewVisualManager.AddEntityComponent<AnchorVisualManager>();
				SandBoxViewSubModule.SandBoxViewVisualManager.AddEntityComponent<StormVisualManager>();
			}
		}

		// Token: 0x04000014 RID: 20
		private NavalRaidVisualOrderProvider _raidVisualOrderProvider;

		// Token: 0x04000015 RID: 21
		private NavalShipVisualOrderProvider _shipVisualOrderProvider;

		// Token: 0x04000016 RID: 22
		private NavalTroopVisualOrderProvider _troopVisualOrderProvider;

		// Token: 0x04000017 RID: 23
		private NavalGameMenuOverlayProvider _gameMenuOverlayProvider;
	}
}
