using System;
using System.Collections.Generic;
using NavalDLC.View.GameMenus;
using NavalDLC.ViewModelCollection.GameMenus;
using SandBox.View.Map;
using SandBox.View.Menu;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.ScreenSystem;

namespace NavalDLC.GauntletUI.Menu
{
	// Token: 0x0200001F RID: 31
	[OverrideView(typeof(NavalMenuTroopSelectionView))]
	public class GauntletNavalMenuTroopSelectionView : MenuView
	{
		// Token: 0x060000F1 RID: 241 RVA: 0x00009AF2 File Offset: 0x00007CF2
		public GauntletNavalMenuTroopSelectionView(TroopRoster fullRoster, TroopRoster initialSelections, List<Ship> eligibleShips, Func<CharacterObject, bool> changeChangeStatusOfTroop, Action<TroopRoster> onDone, int maxSelectableTroopCount, int minSelectableTroopCount)
		{
			this._onDone = onDone;
			this._fullRoster = fullRoster;
			this._initialSelections = initialSelections;
			this._changeChangeStatusOfTroop = changeChangeStatusOfTroop;
			this._maxSelectableTroopCount = maxSelectableTroopCount;
			this._minSelectableTroopCount = minSelectableTroopCount;
			this._eligibleShips = eligibleShips;
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x00009B30 File Offset: 0x00007D30
		protected override void OnInitialize()
		{
			base.OnInitialize();
			this._dataSource = new NavalGameMenuTroopSelectionVM(this._fullRoster, this._initialSelections, this._eligibleShips, this._changeChangeStatusOfTroop, new Action<TroopRoster>(this.OnDone), this._maxSelectableTroopCount, this._minSelectableTroopCount)
			{
				IsEnabled = true
			};
			this._dataSource.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
			this._dataSource.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
			this._dataSource.SetResetInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Reset"));
			base.Layer = new GauntletLayer("NavalMapTroopSelection", 206, false);
			this._layerAsGauntletLayer = base.Layer as GauntletLayer;
			base.Layer.InputRestrictions.SetInputRestrictions(true, 7);
			base.Layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			base.Layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory"));
			this._movie = this._layerAsGauntletLayer.LoadMovie("NavalGameMenuTroopSelection", this._dataSource);
			base.Layer.IsFocusLayer = true;
			ScreenManager.TrySetFocus(this._layerAsGauntletLayer);
			base.MenuViewContext.AddLayer(base.Layer);
			MapScreen mapScreen;
			if ((mapScreen = ScreenManager.TopScreen as MapScreen) != null)
			{
				mapScreen.SetIsInHideoutTroopManage(true);
			}
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x00009CA8 File Offset: 0x00007EA8
		private void OnDone(TroopRoster obj)
		{
			MapScreen.Instance.SetIsInHideoutTroopManage(false);
			base.MenuViewContext.CloseTroopSelection();
			Action<TroopRoster> onDone = this._onDone;
			if (onDone == null)
			{
				return;
			}
			Common.DynamicInvokeWithLog(onDone, new object[] { obj });
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x00009CDC File Offset: 0x00007EDC
		protected override void OnFinalize()
		{
			base.Layer.IsFocusLayer = false;
			ScreenManager.TryLoseFocus(base.Layer);
			this._dataSource.OnFinalize();
			this._dataSource = null;
			this._layerAsGauntletLayer.ReleaseMovie(this._movie);
			base.MenuViewContext.RemoveLayer(base.Layer);
			this._movie = null;
			base.Layer = null;
			this._layerAsGauntletLayer = null;
			MapScreen.Instance.SetIsInHideoutTroopManage(false);
			base.OnFinalize();
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x00009D5C File Offset: 0x00007F5C
		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			if (this._dataSource != null)
			{
				this._dataSource.IsFiveStackModifierActive = base.Layer.Input.IsHotKeyDown("FiveStackModifier");
				this._dataSource.IsEntireStackModifierActive = base.Layer.Input.IsHotKeyDown("EntireStackModifier");
			}
			ScreenLayer layer = base.Layer;
			if (layer != null && layer.Input.IsHotKeyPressed("Exit"))
			{
				UISoundsHelper.PlayUISound("event:/ui/default");
				this._dataSource.ExecuteCancel();
			}
			else
			{
				ScreenLayer layer2 = base.Layer;
				if (layer2 != null && layer2.Input.IsHotKeyPressed("Confirm") && this._dataSource.IsDoneEnabled)
				{
					UISoundsHelper.PlayUISound("event:/ui/default");
					this._dataSource.ExecuteDone();
				}
				else
				{
					ScreenLayer layer3 = base.Layer;
					if (layer3 != null && layer3.Input.IsHotKeyPressed("Reset"))
					{
						UISoundsHelper.PlayUISound("event:/ui/default");
						this._dataSource.ExecuteReset();
					}
				}
			}
			NavalGameMenuTroopSelectionVM dataSource = this._dataSource;
			if (dataSource != null && !dataSource.IsEnabled)
			{
				base.MenuViewContext.CloseTroopSelection();
			}
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x00009E84 File Offset: 0x00008084
		protected override void OnMapConversationActivated()
		{
			base.OnMapConversationActivated();
			if (this._layerAsGauntletLayer != null)
			{
				ScreenManager.SetSuspendLayer(this._layerAsGauntletLayer, true);
			}
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x00009EA0 File Offset: 0x000080A0
		protected override void OnMapConversationDeactivated()
		{
			base.OnMapConversationDeactivated();
			if (this._layerAsGauntletLayer != null)
			{
				ScreenManager.SetSuspendLayer(this._layerAsGauntletLayer, false);
			}
		}

		// Token: 0x04000084 RID: 132
		private readonly Action<TroopRoster> _onDone;

		// Token: 0x04000085 RID: 133
		private readonly TroopRoster _fullRoster;

		// Token: 0x04000086 RID: 134
		private readonly TroopRoster _initialSelections;

		// Token: 0x04000087 RID: 135
		private readonly Func<CharacterObject, bool> _changeChangeStatusOfTroop;

		// Token: 0x04000088 RID: 136
		private readonly int _maxSelectableTroopCount;

		// Token: 0x04000089 RID: 137
		private readonly int _minSelectableTroopCount;

		// Token: 0x0400008A RID: 138
		private readonly List<Ship> _eligibleShips;

		// Token: 0x0400008B RID: 139
		private GauntletLayer _layerAsGauntletLayer;

		// Token: 0x0400008C RID: 140
		private NavalGameMenuTroopSelectionVM _dataSource;

		// Token: 0x0400008D RID: 141
		private GauntletMovieIdentifier _movie;
	}
}
