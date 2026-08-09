using System;
using NavalDLC.Missions.Objects;
using NavalDLC.View.MissionViews;
using NavalDLC.ViewModelCollection.OrderOfBattle;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace NavalDLC.GauntletUI.MissionViews
{
	// Token: 0x02000019 RID: 25
	[OverrideView(typeof(NavalOrderOfBattleView))]
	public class MissionGauntletNavalOrderOfBattleView : MissionView
	{
		// Token: 0x06000091 RID: 145 RVA: 0x000067D4 File Offset: 0x000049D4
		public MissionGauntletNavalOrderOfBattleView(Mission mission)
		{
			this._dataSource = new NavalOrderOfBattleVM(mission, new Action<NavalOrderOfBattleFormationItemVM>(this.OnFormationSelected), new Action(this.ClearFormationSelection), new Action(this.OnAutoDeploy), new Action(this.OnBeginMission));
			this._dataSource.SetDoneInputKey(HotKeyManager.GetCategory("OrderOfBattleHotKeyCategory").GetHotKey("Confirm"));
			this._dataSource.SetResetInputKey(HotKeyManager.GetCategory("OrderOfBattleHotKeyCategory").GetHotKey("AutoDeploy"));
			this.ViewOrderPriority = 13;
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00006869 File Offset: 0x00004A69
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this.InitializeView();
			this._orderUIHandler = base.Mission.GetMissionBehavior<MissionGauntletNavalOrderUIHandler>();
			this._deploymentController = base.Mission.GetMissionBehavior<DeploymentMissionController>();
		}

		// Token: 0x06000093 RID: 147 RVA: 0x0000689C File Offset: 0x00004A9C
		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			if (!this._isActive && this._deploymentController.TeamSetupOver && !base.Mission.IsDeploymentFinished)
			{
				this._cachedOrderTypeSetting = ManagedOptions.GetConfig(35);
				ManagedOptions.SetConfig(35, 1f);
				this._dataSource.Initialize();
				this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
				this._isActive = true;
			}
			if (this._isActive)
			{
				this.UpdateFormationPositions();
				this._wereHotkeysEnabledLastFrame = this._dataSource.AreHotkeysEnabled;
				this.HandleLayerFocus();
				this._dataSource.AreHotkeysEnabled = !base.MissionScreen.IsRadialMenuActive && !base.Mission.IsOrderMenuOpen && Input.IsGamepadActive && !this._gauntletLayer.IsFocusLayer;
				this.TickInput();
			}
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00006977 File Offset: 0x00004B77
		public override void OnDeploymentFinished()
		{
			base.OnDeploymentFinished();
			this.DestroyView();
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00006988 File Offset: 0x00004B88
		private void TickInput()
		{
			if (this._dataSource.IsAssignmentDirty)
			{
				return;
			}
			if (base.MissionScreen.SceneLayer.Input.IsKeyDown(225) || base.MissionScreen.SceneLayer.Input.IsKeyDown(254))
			{
				this._gauntletLayer.InputRestrictions.SetMouseVisibility(false);
				this._dataSource.AreCameraControlsEnabled = true;
			}
			else
			{
				this._gauntletLayer.InputRestrictions.SetMouseVisibility(true);
				this._dataSource.AreCameraControlsEnabled = false;
			}
			if (this._gauntletLayer.Input.IsHotKeyReleased("Exit") && (this._dataSource.HasSelectedHero || this._dataSource.HasSelectedShip) && this._dataSource.CanToggleHeroOrShipSelection)
			{
				UISoundsHelper.PlayUISound("event:/ui/oob/officer_pick");
				this._dataSource.ExecuteClearHeroAndShipSelection();
			}
			if (base.MissionScreen.SceneLayer.Input.IsHotKeyPressed("AutoDeploy"))
			{
				this._isResetPressed = this._dataSource.AreHotkeysEnabled && this._wereHotkeysEnabledLastFrame;
			}
			if (base.MissionScreen.SceneLayer.Input.IsHotKeyPressed("Confirm"))
			{
				this._isReadyPressed = this._dataSource.AreHotkeysEnabled && this._wereHotkeysEnabledLastFrame;
			}
			if (!this._dataSource.AreHotkeysEnabled)
			{
				this._isResetPressed = false;
				this._isReadyPressed = false;
			}
			if (base.MissionScreen.SceneLayer.Input.IsHotKeyReleased("AutoDeploy") && this._dataSource.AreHotkeysEnabled && this._isResetPressed)
			{
				UISoundsHelper.PlayUISound("event:/ui/default");
				this._dataSource.ExecuteAutoDeploy();
			}
			if (base.MissionScreen.SceneLayer.Input.IsHotKeyReleased("Confirm") && this._dataSource.AreHotkeysEnabled && this._dataSource.CanStartMission && this._isReadyPressed)
			{
				UISoundsHelper.PlayUISound("event:/ui/default");
				this._dataSource.ExecuteBeginMission();
			}
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00006B90 File Offset: 0x00004D90
		private void HandleLayerFocus()
		{
			bool flag = this._dataSource.HasSelectedHero || this._dataSource.HasSelectedShip;
			if (this._gauntletLayer.IsFocusLayer && !flag)
			{
				base.MissionScreen.SetDisplayDialog(false);
				this._gauntletLayer.IsFocusLayer = false;
				ScreenManager.TryLoseFocus(this._gauntletLayer);
				return;
			}
			if (!this._gauntletLayer.IsFocusLayer && flag)
			{
				base.MissionScreen.SetDisplayDialog(true);
				this._gauntletLayer.IsFocusLayer = true;
				ScreenManager.TrySetFocus(this._gauntletLayer);
			}
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00006C24 File Offset: 0x00004E24
		private void UpdateFormationPositions()
		{
			if (this._dataSource.IsAssignmentDirty)
			{
				return;
			}
			for (int i = 0; i < this._dataSource.AllFormations.Count; i++)
			{
				this.UpdateFormationPosition(this._dataSource.AllFormations[i]);
			}
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00006C74 File Offset: 0x00004E74
		private void UpdateFormationPosition(NavalOrderOfBattleFormationItemVM formation)
		{
			if (!formation.HasShip)
			{
				return;
			}
			MissionShip missionShip = formation.Ship.MissionShip;
			if (missionShip == null)
			{
				return;
			}
			Vec3 vec = missionShip.GlobalFrame.origin + Vec3.Up * 3f;
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			MBWindowManager.WorldToScreenInsideUsableArea(base.MissionScreen.CombatCamera, vec, ref num, ref num2, ref num3);
			formation.ScreenPosition = new Vec2(num, num2 - 50f);
			formation.WSign = MathF.Sign(num3);
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00006D08 File Offset: 0x00004F08
		public override bool OnEscape()
		{
			bool flag = false;
			if (this._isActive)
			{
				bool flag2 = false;
				if (this._orderUIHandler != null && this._orderUIHandler.IsOrderMenuActive)
				{
					flag2 = this._orderUIHandler.IsAnyOrderSetActive;
					flag = this._orderUIHandler.OnEscape();
				}
				if (!flag2)
				{
					flag = this._dataSource.OnEscape() || flag;
				}
			}
			return flag;
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00006D60 File Offset: 0x00004F60
		public override void OnMissionScreenFinalize()
		{
			this.DestroyView();
			base.OnMissionScreenFinalize();
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00006D6E File Offset: 0x00004F6E
		public override bool IsOpeningEscapeMenuOnFocusChangeAllowed()
		{
			return !this._isActive;
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00006D79 File Offset: 0x00004F79
		public override void OnPhotoModeActivated()
		{
			base.OnPhotoModeActivated();
			if (this._gauntletLayer != null)
			{
				this._gauntletLayer.UIContext.ContextAlpha = 0f;
			}
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00006D9E File Offset: 0x00004F9E
		public override void OnPhotoModeDeactivated()
		{
			base.OnPhotoModeDeactivated();
			if (this._gauntletLayer != null)
			{
				this._gauntletLayer.UIContext.ContextAlpha = 1f;
			}
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00006DC4 File Offset: 0x00004FC4
		private void InitializeView()
		{
			this._gauntletLayer = new GauntletLayer("NavalOrderOfBattle", this.ViewOrderPriority, false);
			this._gauntletLayer.LoadMovie("NavalOrderOfBattle", this._dataSource);
			this._orderOfBattleSpriteCategory = UIResourceManager.LoadSpriteCategory("ui_order_of_battle");
			base.MissionScreen.SceneLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("OrderOfBattleHotKeyCategory"));
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("OrderOfBattleHotKeyCategory"));
			base.MissionScreen.AddLayer(this._gauntletLayer);
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00006E5C File Offset: 0x0000505C
		private void DestroyView()
		{
			if (this._gauntletLayer == null && this._dataSource == null)
			{
				return;
			}
			if (this._isActive)
			{
				ManagedOptions.SetConfig(35, this._cachedOrderTypeSetting);
			}
			this._isActive = false;
			base.MissionScreen.SetDisplayDialog(false);
			this._dataSource.OnFinalize();
			this._dataSource = null;
			base.MissionScreen.RemoveLayer(this._gauntletLayer);
			this._gauntletLayer = null;
			this._orderOfBattleSpriteCategory.Unload();
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00006ED8 File Offset: 0x000050D8
		private void OnFormationSelected(NavalOrderOfBattleFormationItemVM selectedFormation)
		{
			this.SelectFormationAtIndex(selectedFormation.Formation.Index);
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00006EEB File Offset: 0x000050EB
		private void SelectFormationAtIndex(int index)
		{
			MissionGauntletNavalOrderUIHandler orderUIHandler = this._orderUIHandler;
			if (orderUIHandler == null)
			{
				return;
			}
			orderUIHandler.SelectFormationAtIndex(index);
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00006EFE File Offset: 0x000050FE
		private void DeselectFormationAtIndex(int index)
		{
			MissionGauntletNavalOrderUIHandler orderUIHandler = this._orderUIHandler;
			if (orderUIHandler == null)
			{
				return;
			}
			orderUIHandler.DeselectFormationAtIndex(index);
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00006F11 File Offset: 0x00005111
		private void ClearFormationSelection()
		{
			MissionGauntletNavalOrderUIHandler orderUIHandler = this._orderUIHandler;
			if (orderUIHandler == null)
			{
				return;
			}
			orderUIHandler.ClearFormationSelection();
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00006F23 File Offset: 0x00005123
		private void OnAutoDeploy()
		{
			this._orderUIHandler.OnAutoDeploy();
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00006F30 File Offset: 0x00005130
		private void OnBeginMission()
		{
			this._orderUIHandler.OnFiltersSet(this._dataSource.CurrentFilterConfiguration);
			this._orderUIHandler.OnClassesSet(this._dataSource.CurrentClassConfiguration);
			this._orderUIHandler.OnBeginMission();
		}

		// Token: 0x04000051 RID: 81
		private NavalOrderOfBattleVM _dataSource;

		// Token: 0x04000052 RID: 82
		private GauntletLayer _gauntletLayer;

		// Token: 0x04000053 RID: 83
		private SpriteCategory _orderOfBattleSpriteCategory;

		// Token: 0x04000054 RID: 84
		private MissionGauntletNavalOrderUIHandler _orderUIHandler;

		// Token: 0x04000055 RID: 85
		private DeploymentMissionController _deploymentController;

		// Token: 0x04000056 RID: 86
		private bool _isActive;

		// Token: 0x04000057 RID: 87
		private bool _wereHotkeysEnabledLastFrame;

		// Token: 0x04000058 RID: 88
		private bool _isResetPressed;

		// Token: 0x04000059 RID: 89
		private bool _isReadyPressed;

		// Token: 0x0400005A RID: 90
		private float _cachedOrderTypeSetting;
	}
}
