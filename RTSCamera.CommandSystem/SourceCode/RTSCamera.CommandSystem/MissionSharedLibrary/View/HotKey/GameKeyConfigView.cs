using System;
using MissionLibrary.HotKey;
using MissionLibrary.Repository;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade.GauntletUI;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace MissionSharedLibrary.View.HotKey
{
	// Token: 0x0200001B RID: 27
	public class GameKeyConfigView : MissionView
	{
		// Token: 0x060000EF RID: 239 RVA: 0x00004FDB File Offset: 0x000031DB
		public GameKeyConfigView()
		{
			this.ViewOrderPriority = 50;
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x00004FEC File Offset: 0x000031EC
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this._keybindingPopup = new KeybindingPopup(new Action<Key>(this.SetHotKey), base.MissionScreen);
			this._optionsSpriteCategory = UIResourceManager.LoadSpriteCategory("ui_options");
			this._fullScreensSpriteCategory = UIResourceManager.LoadSpriteCategory("ui_fullscreens");
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x0000503C File Offset: 0x0000323C
		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			this._keybindingPopup.OnToggle(false);
			this._keybindingPopup = null;
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x00005058 File Offset: 0x00003258
		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			if (this._gauntletLayer == null)
			{
				return;
			}
			if (!this._keybindingPopup.IsActive && this._gauntletLayer.Input.IsHotKeyReleased("Exit"))
			{
				this._dataSource.ExecuteCancel();
			}
			this._keybindingPopup.Tick();
			if (this._enableKeyBindingPopupNextTick)
			{
				this._enableKeyBindingPopupNextTick = false;
				this._keybindingPopup.OnToggle(true);
			}
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x000050CC File Offset: 0x000032CC
		public void Activate()
		{
			string text = "MissionLibraryOptionsGameKeyScreen-2";
			this._dataSource = new GameKeyConfigVM(ARepository<AGameKeyCategoryManager, AGameKeyCategory>.Get(), new Action<IHotKeySetter>(this.OnKeyBindRequest), new Action(this.Deactivate));
			this._gauntletLayer = new GauntletLayer(text, this.ViewOrderPriority, false);
			this._gauntletLayer.LoadMovie(text, this._dataSource);
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
			this._gauntletLayer.IsFocusLayer = true;
			base.MissionScreen.AddLayer(this._gauntletLayer);
			ScreenManager.TrySetFocus(this._gauntletLayer);
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x00005184 File Offset: 0x00003384
		public void Deactivate()
		{
			if (this._gauntletLayer == null)
			{
				return;
			}
			this._gauntletLayer.InputRestrictions.ResetInputRestrictions();
			base.MissionScreen.RemoveLayer(this._gauntletLayer);
			this._gauntletLayer = null;
			this._dataSource.OnFinalize();
			this._dataSource = null;
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x000051D4 File Offset: 0x000033D4
		private void OnKeyBindRequest(IHotKeySetter requestedHotKeyToChange)
		{
			this._currentGameKey = requestedHotKeyToChange;
			this._enableKeyBindingPopupNextTick = true;
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x000051E4 File Offset: 0x000033E4
		private void SetHotKey(Key key)
		{
			if (this._gauntletLayer.Input.IsHotKeyReleased("Exit"))
			{
				this._currentGameKey = null;
				this._keybindingPopup.OnToggle(false);
				this._dataSource.Update();
				return;
			}
			IHotKeySetter currentGameKey = this._currentGameKey;
			if (currentGameKey != null)
			{
				currentGameKey.Set(key.InputKey);
			}
			this._currentGameKey = null;
			this._keybindingPopup.OnToggle(false);
		}

		// Token: 0x04000054 RID: 84
		private GauntletLayer _gauntletLayer;

		// Token: 0x04000055 RID: 85
		private GameKeyConfigVM _dataSource;

		// Token: 0x04000056 RID: 86
		private KeybindingPopup _keybindingPopup;

		// Token: 0x04000057 RID: 87
		private IHotKeySetter _currentGameKey;

		// Token: 0x04000058 RID: 88
		private bool _enableKeyBindingPopupNextTick;

		// Token: 0x04000059 RID: 89
		private SpriteCategory _optionsSpriteCategory;

		// Token: 0x0400005A RID: 90
		private SpriteCategory _fullScreensSpriteCategory;

		// Token: 0x0400005B RID: 91
		public const string KeyBindRequestEventId = "KeyBindRequest";

		// Token: 0x0400005C RID: 92
		public const string KeyBindRequestReceiverId = "GameKeyConfigView";
	}
}
