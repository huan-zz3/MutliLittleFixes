using System;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.ScreenSystem;

namespace MissionSharedLibrary.View
{
	// Token: 0x02000015 RID: 21
	public abstract class MissionMenuViewBase : MissionView
	{
		// Token: 0x1700001D RID: 29
		// (get) Token: 0x060000B4 RID: 180 RVA: 0x00004802 File Offset: 0x00002A02
		// (set) Token: 0x060000B5 RID: 181 RVA: 0x0000480A File Offset: 0x00002A0A
		public bool IsActivated { get; set; }

		// Token: 0x060000B6 RID: 182 RVA: 0x00004813 File Offset: 0x00002A13
		protected MissionMenuViewBase(int viewOrderPriority, string movieName, bool pauseGameEngine = true, bool focus = true)
		{
			this.ViewOrderPriority = viewOrderPriority;
			this._movieName = movieName;
			this._pauseGameEngine = pauseGameEngine;
			this._focus = focus;
		}

		// Token: 0x060000B7 RID: 183
		protected abstract MissionMenuVMBase GetDataSource();

		// Token: 0x060000B8 RID: 184 RVA: 0x00004838 File Offset: 0x00002A38
		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			this.GauntletLayer = null;
			MissionMenuVMBase dataSource = this.DataSource;
			if (dataSource != null)
			{
				dataSource.OnFinalize();
			}
			this.DataSource = null;
			this._movie = null;
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x00004866 File Offset: 0x00002A66
		public void ToggleMenu()
		{
			if (this.IsActivated)
			{
				this.DeactivateMenu();
				return;
			}
			this.ActivateMenu();
		}

		// Token: 0x060000BA RID: 186 RVA: 0x00004880 File Offset: 0x00002A80
		public virtual void ActivateMenu()
		{
			if (this.IsActivated)
			{
				return;
			}
			this.IsActivated = true;
			this.DataSource = this.GetDataSource();
			if (this.DataSource == null)
			{
				return;
			}
			this.GauntletLayer = new GauntletLayer(this._movieName, this.ViewOrderPriority, false);
			this.GauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
			this.GauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			this._movie = this.GauntletLayer.LoadMovie(this._movieName, this.DataSource);
			UIResourceManager.LoadSpriteCategory("ui_saveload");
			base.MissionScreen.AddLayer(this.GauntletLayer);
			if (this._focus)
			{
				this.GauntletLayer.IsFocusLayer = true;
				ScreenManager.TrySetFocus(this.GauntletLayer);
			}
			this.DataSource.RefreshValues();
			this.PauseGame();
		}

		// Token: 0x060000BB RID: 187 RVA: 0x0000495F File Offset: 0x00002B5F
		public virtual void DeactivateMenu()
		{
			if (!this.IsActivated)
			{
				return;
			}
			MissionMenuVMBase dataSource = this.DataSource;
			if (dataSource == null)
			{
				return;
			}
			dataSource.CloseMenu();
		}

		// Token: 0x060000BC RID: 188 RVA: 0x0000497C File Offset: 0x00002B7C
		protected void OnCloseMenu()
		{
			this.IsActivated = false;
			this.GauntletLayer.InputRestrictions.ResetInputRestrictions();
			this.GauntletLayer.IsFocusLayer = false;
			ScreenManager.TryLoseFocus(this.GauntletLayer);
			base.MissionScreen.RemoveLayer(this.GauntletLayer);
			this.DataSource.OnFinalize();
			this.DataSource = null;
			this._movie = null;
			this.GauntletLayer = null;
			this.UnpauseGame();
		}

		// Token: 0x060000BD RID: 189 RVA: 0x000049F0 File Offset: 0x00002BF0
		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			if (this.IsActivated && (this.GauntletLayer.Input.IsKeyReleased(225) || this.GauntletLayer.Input.IsHotKeyReleased("Exit") || this.GauntletLayer.Input.IsHotKeyReleased("ToggleEscapeMenu")))
			{
				this.DeactivateMenu();
			}
		}

		// Token: 0x060000BE RID: 190 RVA: 0x00004A57 File Offset: 0x00002C57
		public override void OnRemoveBehavior()
		{
			base.OnRemoveBehavior();
			this.UnpauseGame();
		}

		// Token: 0x060000BF RID: 191 RVA: 0x00004A68 File Offset: 0x00002C68
		private void PauseGame()
		{
			if (this._pauseGameEngine && !MBCommon.IsPaused)
			{
				this._enginePausedBySelf = true;
				MBCommon.PauseGameEngine();
				Game.Current.GameStateManager.RegisterActiveStateDisableRequest(this);
			}
			if (!MissionState.Current.Paused)
			{
				this._missionPausedBySelf = true;
				MissionState.Current.Paused = true;
			}
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x00004AC0 File Offset: 0x00002CC0
		private void UnpauseGame()
		{
			if (this._pauseGameEngine && this._enginePausedBySelf)
			{
				this._enginePausedBySelf = false;
				MBCommon.UnPauseGameEngine();
				Game.Current.GameStateManager.UnregisterActiveStateDisableRequest(this);
			}
			if (this._missionPausedBySelf)
			{
				this._missionPausedBySelf = false;
				MissionState.Current.Paused = false;
			}
		}

		// Token: 0x0400003C RID: 60
		protected readonly string _movieName;

		// Token: 0x0400003D RID: 61
		private readonly bool _pauseGameEngine;

		// Token: 0x0400003E RID: 62
		protected MissionMenuVMBase DataSource;

		// Token: 0x0400003F RID: 63
		protected GauntletLayer GauntletLayer;

		// Token: 0x04000040 RID: 64
		protected GauntletMovieIdentifier _movie;

		// Token: 0x04000041 RID: 65
		protected bool _enginePausedBySelf;

		// Token: 0x04000042 RID: 66
		protected bool _missionPausedBySelf;

		// Token: 0x04000043 RID: 67
		protected bool _focus;
	}
}
