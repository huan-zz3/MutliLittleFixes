using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.ScreenSystem;

namespace NavalDLC.CustomBattle.CustomBattle
{
	// Token: 0x02000014 RID: 20
	[GameStateScreen(typeof(NavalCustomBattleState))]
	public class NavalCustomBattleScreen : ScreenBase, IGameStateListener
	{
		// Token: 0x060000EC RID: 236 RVA: 0x00006017 File Offset: 0x00004217
		public NavalCustomBattleScreen(NavalCustomBattleState customBattleState)
		{
		}

		// Token: 0x060000ED RID: 237 RVA: 0x0000601F File Offset: 0x0000421F
		void IGameStateListener.OnActivate()
		{
		}

		// Token: 0x060000EE RID: 238 RVA: 0x00006021 File Offset: 0x00004221
		void IGameStateListener.OnDeactivate()
		{
		}

		// Token: 0x060000EF RID: 239 RVA: 0x00006023 File Offset: 0x00004223
		void IGameStateListener.OnInitialize()
		{
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x00006025 File Offset: 0x00004225
		void IGameStateListener.OnFinalize()
		{
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x00006028 File Offset: 0x00004228
		protected override void OnInitialize()
		{
			base.OnInitialize();
			this._dataSource = new NavalCustomBattleVM();
			this._dataSource.SetStartInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
			this._dataSource.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
			this._dataSource.SetResetInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Reset"));
			this._dataSource.SetRandomizeInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Randomize"));
			this._dataSource.SetCycleTierInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("SwitchToNextTab"));
			NavalCustomBattleTroopTypeSelectionPopUpVM troopTypeSelectionPopUp = this._dataSource.TroopTypeSelectionPopUp;
			if (troopTypeSelectionPopUp != null)
			{
				troopTypeSelectionPopUp.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
			}
			this._gauntletLayer = new GauntletLayer("NavalCustomBattle", 1, true);
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory"));
			this.LoadMovie();
			this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
			this._dataSource.SetActiveState(true);
			base.AddLayer(this._gauntletLayer);
			InformationManager.HideAllMessages();
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x00006188 File Offset: 0x00004388
		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			if (this._isFirstFrameCounter >= 0)
			{
				if (this._isFirstFrameCounter == 0)
				{
					LoadingWindow.DisableGlobalLoadingWindow();
				}
				this._isFirstFrameCounter--;
			}
			if (!this._gauntletLayer.IsFocusedOnInput())
			{
				NavalCustomBattleTroopTypeSelectionPopUpVM troopTypeSelectionPopUp = this._dataSource.TroopTypeSelectionPopUp;
				if (troopTypeSelectionPopUp != null && troopTypeSelectionPopUp.IsOpen)
				{
					if (this._gauntletLayer.Input.IsHotKeyReleased("Exit"))
					{
						UISoundsHelper.PlayUISound("event:/ui/default");
						this._dataSource.TroopTypeSelectionPopUp.ExecuteCancel();
						return;
					}
					if (this._gauntletLayer.Input.IsHotKeyReleased("Confirm"))
					{
						UISoundsHelper.PlayUISound("event:/ui/default");
						this._dataSource.TroopTypeSelectionPopUp.ExecuteDone();
						return;
					}
					if (this._gauntletLayer.Input.IsHotKeyReleased("Reset"))
					{
						UISoundsHelper.PlayUISound("event:/ui/default");
						this._dataSource.TroopTypeSelectionPopUp.ExecuteReset();
						return;
					}
				}
				else
				{
					NavalCustomBattleShipSelectionPopUpVM shipSelectionPopUp = this._dataSource.ShipSelectionPopUp;
					if (shipSelectionPopUp != null && shipSelectionPopUp.IsOpen)
					{
						if (this._gauntletLayer.Input.IsHotKeyReleased("Exit"))
						{
							UISoundsHelper.PlayUISound("event:/ui/default");
							this._dataSource.ShipSelectionPopUp.ExecuteClose();
							return;
						}
					}
					else
					{
						if (this._dataSource.FocusedShipItem != null && this._gauntletLayer.Input.IsHotKeyReleased("SwitchToNextTab"))
						{
							UISoundsHelper.PlayUISound("event:/ui/default");
							this._dataSource.FocusedShipItem.ExecuteCycleUpgradeTier();
						}
						if (this._gauntletLayer.Input.IsHotKeyReleased("Exit"))
						{
							UISoundsHelper.PlayUISound("event:/ui/default");
							this._dataSource.ExecuteBack();
							return;
						}
						if (this._gauntletLayer.Input.IsHotKeyReleased("Randomize"))
						{
							UISoundsHelper.PlayUISound("event:/ui/default");
							this._dataSource.ExecuteRandomize();
							return;
						}
						if (this._gauntletLayer.Input.IsHotKeyReleased("Confirm") && this._dataSource.CanConfirm)
						{
							UISoundsHelper.PlayUISound("event:/ui/default");
							this._dataSource.ExecuteStart();
						}
					}
				}
			}
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x000063A4 File Offset: 0x000045A4
		protected override void OnFinalize()
		{
			this.UnloadMovie();
			base.RemoveLayer(this._gauntletLayer);
			this._dataSource.OnFinalize();
			this._dataSource = null;
			this._gauntletLayer = null;
			base.OnFinalize();
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x000063D7 File Offset: 0x000045D7
		protected override void OnActivate()
		{
			this.LoadMovie();
			NavalCustomBattleVM dataSource = this._dataSource;
			if (dataSource != null)
			{
				dataSource.SetActiveState(true);
			}
			this._gauntletLayer.IsFocusLayer = true;
			ScreenManager.TrySetFocus(this._gauntletLayer);
			this._isFirstFrameCounter = 2;
			base.OnActivate();
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x00006415 File Offset: 0x00004615
		protected override void OnDeactivate()
		{
			base.OnDeactivate();
			this.UnloadMovie();
			NavalCustomBattleVM dataSource = this._dataSource;
			if (dataSource == null)
			{
				return;
			}
			dataSource.SetActiveState(false);
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x00006434 File Offset: 0x00004634
		public override void UpdateLayout()
		{
			base.UpdateLayout();
			if (!this._isMovieLoaded)
			{
				NavalCustomBattleVM dataSource = this._dataSource;
				if (dataSource == null)
				{
					return;
				}
				dataSource.RefreshValues();
			}
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x00006454 File Offset: 0x00004654
		private void LoadMovie()
		{
			if (!this._isMovieLoaded)
			{
				this._gauntletMovie = this._gauntletLayer.LoadMovie("NavalCustomBattleScreen", this._dataSource);
				this._isMovieLoaded = true;
			}
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x00006481 File Offset: 0x00004681
		private void UnloadMovie()
		{
			if (this._isMovieLoaded)
			{
				this._gauntletLayer.ReleaseMovie(this._gauntletMovie);
				this._gauntletMovie = null;
				this._isMovieLoaded = false;
				this._gauntletLayer.IsFocusLayer = false;
				ScreenManager.TryLoseFocus(this._gauntletLayer);
			}
		}

		// Token: 0x04000085 RID: 133
		private GauntletLayer _gauntletLayer;

		// Token: 0x04000086 RID: 134
		private GauntletMovieIdentifier _gauntletMovie;

		// Token: 0x04000087 RID: 135
		private NavalCustomBattleVM _dataSource;

		// Token: 0x04000088 RID: 136
		private bool _isMovieLoaded;

		// Token: 0x04000089 RID: 137
		private int _isFirstFrameCounter;
	}
}
