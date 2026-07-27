using System;
using System.Numerics;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using NavalDLC.Missions.ShipActuators;
using NavalDLC.Missions.ShipInput;
using NavalDLC.View.MissionViews;
using NavalDLC.ViewModelCollection.Missions.ShipControl;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.GauntletUI.Mission;
using TaleWorlds.MountAndBlade.GauntletUI.Mission.Singleplayer;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.Screens;

namespace NavalDLC.GauntletUI.MissionViews
{
	// Token: 0x0200001E RID: 30
	[OverrideView(typeof(MissionShipControlView))]
	public class MissionGauntletShipControlView : MissionShipControlView
	{
		// Token: 0x17000003 RID: 3
		// (get) Token: 0x060000CC RID: 204 RVA: 0x00008624 File Offset: 0x00006824
		// (set) Token: 0x060000CD RID: 205 RVA: 0x0000862C File Offset: 0x0000682C
		public MissionGauntletShipControlView.ShipControlFeatureFlags SuspendedFeatures { get; private set; }

		// Token: 0x060000CE RID: 206 RVA: 0x00008638 File Offset: 0x00006838
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this._dataSource = new MissionShipControlVM();
			this._gauntletLayer = new GauntletLayer("MissionShipControl", this.ViewOrderPriority, false);
			this._gauntletLayer.LoadMovie("MissionShipControl", this._dataSource);
			this._orderUIHandler = base.Mission.GetMissionBehavior<MissionGauntletSingleplayerOrderUIHandler>();
			this._crosshairView = base.Mission.GetMissionBehavior<MissionGauntletCrosshair>();
			this._shipHighlightView = base.Mission.GetMissionBehavior<NavalMissionShipHighlightView>();
			this._agentStatusView = base.Mission.GetMissionBehavior<MissionGauntletNavalAgentStatus>();
			this._gauntletLayer.InputRestrictions.SetInputRestrictions(false, 0);
			if (!base.MissionScreen.SceneLayer.Input.IsCategoryRegistered(HotKeyManager.GetCategory("NavalShipControlsHotKeyCategory")))
			{
				base.MissionScreen.SceneLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("NavalShipControlsHotKeyCategory"));
			}
			base.MissionScreen.AddLayer(this._gauntletLayer);
			this.SetControlKeys();
		}

		// Token: 0x060000CF RID: 207 RVA: 0x00008731 File Offset: 0x00006931
		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			this._dataSource.OnFinalize();
			base.MissionScreen.RemoveLayer(this._gauntletLayer);
			this._dataSource = null;
			this._gauntletLayer = null;
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x00008763 File Offset: 0x00006963
		protected override void OnCreateView()
		{
			base.OnCreateView();
			this._isBattleUIVisible = true;
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x00008772 File Offset: 0x00006972
		protected override void OnDestroyView()
		{
			base.OnDestroyView();
			this._isBattleUIVisible = false;
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x00008781 File Offset: 0x00006981
		public void SuspendFeature(MissionGauntletShipControlView.ShipControlFeatureFlags feature)
		{
			this.SuspendedFeatures |= feature;
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x00008791 File Offset: 0x00006991
		public bool IsFeatureSuspended(MissionGauntletShipControlView.ShipControlFeatureFlags feature)
		{
			return (this.SuspendedFeatures & feature) > (MissionGauntletShipControlView.ShipControlFeatureFlags)0;
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x0000879E File Offset: 0x0000699E
		public void ResumeFeature(MissionGauntletShipControlView.ShipControlFeatureFlags feature)
		{
			this.SuspendedFeatures &= ~feature;
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x000087AF File Offset: 0x000069AF
		public override void OnPhotoModeActivated()
		{
			base.OnPhotoModeActivated();
			this._isPhotoModeActive = true;
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x000087BE File Offset: 0x000069BE
		public override void OnPhotoModeDeactivated()
		{
			base.OnPhotoModeDeactivated();
			this._isPhotoModeActive = false;
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x000087D0 File Offset: 0x000069D0
		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			this.UpdateVisibility();
			MissionShip playerControlledShip = this._playerControlledShip;
			NavalShipsLogic navalShipsLogic = this.NavalShipsLogic;
			this._playerControlledShip = ((navalShipsLogic != null) ? navalShipsLogic.PlayerControlledShip : null);
			MissionShip playerControlledShip2 = this._playerControlledShip;
			this._isAnyBridgeActive = playerControlledShip2 != null && playerControlledShip2.GetIsAnyBridgeActive();
			if (playerControlledShip != this._playerControlledShip)
			{
				if (this._playerControlledShip != null)
				{
					MissionGauntletCrosshair crosshairView = this._crosshairView;
					if (crosshairView != null)
					{
						crosshairView.SuspendView();
					}
					this._lastFirstPersonModeSelection = base.Mission.CameraIsFirstPerson;
					base.Mission.CameraIsFirstPerson = false;
				}
				else
				{
					MissionGauntletCrosshair crosshairView2 = this._crosshairView;
					if (crosshairView2 != null)
					{
						crosshairView2.ResumeView();
					}
					base.Mission.CameraIsFirstPerson = this._lastFirstPersonModeSelection;
					if (this.IsAimingWithRangedWeapon)
					{
						this.IsAimingWithRangedWeapon = false;
						if (playerControlledShip != null)
						{
							playerControlledShip.OnSetRangedWeaponControlMode(false);
						}
					}
				}
			}
			if (this._playerControlledShip != null && this.IsAimingWithRangedWeapon && !this.GetIsRangedWeaponAvailable())
			{
				this.IsAimingWithRangedWeapon = false;
				this._playerControlledShip.OnSetRangedWeaponControlMode(false);
			}
			this.UpdateShipValues();
			this.RefreshControlKeys();
			this.UpdateFocusedShip();
			this.TickInput();
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x000088E0 File Offset: 0x00006AE0
		private void UpdateHitPoints()
		{
			if (this._dataSource == null)
			{
				return;
			}
			if (this._playerControlledShip == null)
			{
				this._dataSource.ShipHitPoints.IsRelevant = false;
				this._dataSource.SailHitPoints.IsRelevant = false;
				this._dataSource.FireHitPoints.IsRelevant = false;
				return;
			}
			this._dataSource.ShipHitPoints.IsRelevant = true;
			this._dataSource.SailHitPoints.IsRelevant = true;
			this._dataSource.FireHitPoints.IsRelevant = true;
			this._dataSource.ShipHitPoints.ActiveHitPoints = MathF.Round(this._playerControlledShip.HitPoints);
			this._dataSource.ShipHitPoints.MaxHitPoints = MathF.Round(this._playerControlledShip.MaxHealth);
			this._dataSource.SailHitPoints.ActiveHitPoints = MathF.Round(this._playerControlledShip.SailHitPoints);
			this._dataSource.SailHitPoints.MaxHitPoints = MathF.Round(this._playerControlledShip.MaxSailHitPoints);
			this._dataSource.FireHitPoints.ActiveHitPoints = MathF.Round(this._playerControlledShip.FireHitPoints);
			this._dataSource.FireHitPoints.MaxHitPoints = MathF.Round(this._playerControlledShip.MaxFireHealth);
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x00008A28 File Offset: 0x00006C28
		private void TickInput()
		{
			MissionScreen missionScreen = base.MissionScreen;
			InputContext inputContext;
			if (missionScreen == null)
			{
				inputContext = null;
			}
			else
			{
				SceneLayer sceneLayer = missionScreen.SceneLayer;
				inputContext = ((sceneLayer != null) ? sceneLayer.Input : null);
			}
			InputContext inputContext2 = inputContext;
			if (inputContext2 == null || this._playerControlledShip == null || base.MissionScreen.IsPhotoModeEnabled || base.IsDisplayingADialog || base.MissionScreen.IsCheatGhostMode)
			{
				return;
			}
			if (inputContext2.IsGameKeyReleased(111))
			{
				if (this.GetCanToggleOarsmen())
				{
					int num = (this._playerControlledShip.ShipOrder.OarsmenLevel + 2) % 3;
					this._playerControlledShip.ShipOrder.SetOrderOarsmenLevel(num);
					TextObject textObject = null;
					if (num == 0)
					{
						textObject = new TextObject("{=RtRNkfMA}Stop using the oars!", null);
					}
					else if (num == 1)
					{
						textObject = new TextObject("{=a7CzRLXb}Use oars in half power!", null);
					}
					else if (num == 2)
					{
						textObject = new TextObject("{=RKthVuaC}Use oars in full power!", null);
					}
					if (textObject != null)
					{
						this.DisplayCommandForSelectedFormations(textObject);
					}
				}
				else if (this.GetCanCutLoose() && !this.GetIsCutLooseTemporarilyBlocked())
				{
					this._playerControlledShip.ShipOrder.SetCutLoose(true);
					this.DisplayCommandForSelectedFormations(new TextObject("{=siE18G0C}Cut loose!", null));
				}
			}
			if (inputContext2.IsGameKeyReleased(110) && this.GetCanToggleSail())
			{
				this.SailControl = (this.SailControl.IsMax() ? this.SailControl.Min(this._playerControlledShipHasHybridSails) : this.SailControl.Raise(this._playerControlledShipHasHybridSails));
				switch (this.SailControl)
				{
				case SailInput.Raised:
					this.DisplayCommandForSelectedFormations(new TextObject("{=kWfyfiVA}Furl sails!", null));
					break;
				case SailInput.SquareSailsRaised:
					this.DisplayCommandForSelectedFormations(new TextObject("{=kGtL9Kea}Furl square sails!", null));
					break;
				case SailInput.Full:
					this.DisplayCommandForSelectedFormations(new TextObject("{=75VaP7bL}Open sails!", null));
					break;
				}
			}
			if (inputContext2.IsGameKeyReleased(112) && this.GetCanChangeCamera())
			{
				base.ActiveCameraMode = (base.ActiveCameraMode + 1) % MissionShipControlView.CameraModes.NumPositions;
			}
			if (inputContext2.IsGameKeyReleased(113) && this.GetCanSelectShip())
			{
				Formation formation = this._focusedShip.Formation;
				int num2 = ((formation != null) ? formation.Index : (-1));
				if (num2 >= 0)
				{
					this._orderUIHandler.SelectFormationAtIndex(num2);
				}
			}
			if (inputContext2.IsGameKeyReleased(114) && this.GetCanAttemptBoarding())
			{
				if (this.GetIsCancelBoardingAvailable())
				{
					this._playerControlledShip.ShipOrder.SetBoardingTargetShip(null);
					this.DisplayCommandForSelectedFormations(new TextObject("{=U6Z4GFPW}Stop boarding!", null));
				}
				else if (!this.GetIsAttemptBoardingTemporarilyBlocked())
				{
					this._playerControlledShip.ShipOrder.SetBoardingTargetShip(this._focusedShip);
					this.DisplayCommandForSelectedFormations(new TextObject("{=HSALr4nl}Board {SHIP_NAME}!", null).SetTextVariable("SHIP_NAME", (this._focusedShip.Team == null || this._focusedShip.Team.TeamSide == 2) ? this._focusedShip.ShipOrigin.Hull.Name : this._focusedShip.ShipOrigin.Name));
				}
			}
			if (inputContext2.IsGameKeyReleased(115) && this.GetCanToggleRangedWeaponOrderMode())
			{
				this.IsAimingWithRangedWeapon = !this.IsAimingWithRangedWeapon;
				this._playerControlledShip.OnSetRangedWeaponControlMode(this.IsAimingWithRangedWeapon);
			}
			if (inputContext2.IsGameKeyReleased(9) && this.GetCanShootBallista())
			{
				this._playerControlledShip.ShootBallista();
			}
		}

		// Token: 0x060000DA RID: 218 RVA: 0x00008D4C File Offset: 0x00006F4C
		private void DisplayCommandForSelectedFormations(TextObject command)
		{
			TextObject textObject = new TextObject("{=ApD0xQXT}{STR1}: {STR2}", null);
			string text = "STR1";
			MissionShip playerControlledShip = this._playerControlledShip;
			TextObject textObject2;
			if (playerControlledShip == null)
			{
				textObject2 = null;
			}
			else
			{
				IShipOrigin shipOrigin = playerControlledShip.ShipOrigin;
				textObject2 = ((shipOrigin != null) ? shipOrigin.Name : null);
			}
			textObject.SetTextVariable(text, textObject2 ?? new TextObject("{=wXCM8BnW}Crew", null));
			textObject.SetTextVariable("STR2", command);
			InformationManager.DisplayMessage(new InformationMessage(textObject.ToString()));
		}

		// Token: 0x060000DB RID: 219 RVA: 0x00008DBC File Offset: 0x00006FBC
		private void UpdateFocusedShip()
		{
			if (base.Mission.Scene == null || this._playerControlledShip == null || base.MissionScreen.IsPhotoModeEnabled || base.IsDisplayingADialog || this.IsFeatureSuspended(MissionGauntletShipControlView.ShipControlFeatureFlags.ShipFocus))
			{
				MissionShipControlVM dataSource = this._dataSource;
				if (dataSource != null)
				{
					dataSource.SetTargetedShip(null, -5000f, -5000f, -1f);
				}
				this.SetFocusedShip(null);
				this._dataSource.SetBoardingTargetShip(null, -5000f, -5000f, -1f);
				return;
			}
			MatrixFrame lastFinalRenderCameraFrame = base.Mission.Scene.LastFinalRenderCameraFrame;
			Vec2 vec = Screen.RealScreenResolution * 0.5f;
			float maxValue = float.MaxValue;
			float num = Screen.RealScreenResolutionHeight / 4f;
			MissionShip missionShip = null;
			Vec3 globalPosition = this._playerControlledShip.GameEntity.GlobalPosition;
			Vec3 zero = Vec3.Zero;
			for (int i = 0; i < this.NavalShipsLogic.AllShips.Count; i++)
			{
				bool flag;
				this.CheckFocusableShip(this.NavalShipsLogic.AllShips[i], globalPosition, 100f, 350f, lastFinalRenderCameraFrame, vec, ref zero, ref maxValue, num, ref missionShip, out flag);
				if (flag)
				{
					break;
				}
			}
			this.SetFocusedShip(missionShip);
			if (this._dataSource != null)
			{
				this._dataSource.SetTargetedShip(missionShip, zero.x, zero.y - 70f, zero.z);
				this._dataSource.TargetedShipHasAction = !MBCommon.IsPaused && (this.GetCanAttemptBoarding() || this.GetCanSelectShip());
				this._dataSource.IsCancelBoardingOrderAvailable = this.GetIsCancelBoardingAvailable();
			}
		}

		// Token: 0x060000DC RID: 220 RVA: 0x00008F5C File Offset: 0x0000715C
		private void CheckFocusableShip(MissionShip focusableShip, Vec3 playerShipPosition, float enemyFocusDistance, float friendlyFocusDistance, MatrixFrame cameraFrame, Vec2 screenCenter, ref Vec3 hitScreenPosition, ref float closestDistance, float focusRadius, ref MissionShip closestShip, out bool directHitFound)
		{
			directHitFound = false;
			if (focusableShip.IsDisabled || focusableShip.IsSinking || focusableShip == this._playerControlledShip)
			{
				return;
			}
			Vec3 globalPosition = focusableShip.GameEntity.GlobalPosition;
			if (focusableShip.BattleSide == base.Mission.PlayerEnemyTeam.Side && globalPosition.DistanceSquared(playerShipPosition) > enemyFocusDistance * enemyFocusDistance)
			{
				return;
			}
			if (focusableShip.BattleSide == base.Mission.PlayerTeam.Side && globalPosition.DistanceSquared(playerShipPosition) > friendlyFocusDistance * friendlyFocusDistance)
			{
				return;
			}
			Vec3 shipFocusPosition = this.GetShipFocusPosition(focusableShip);
			float num = -5000f;
			float num2 = -5000f;
			float num3 = -5000f;
			MBWindowManager.WorldToScreenInsideUsableArea(base.MissionScreen.CombatCamera, shipFocusPosition, ref num, ref num2, ref num3);
			float num4 = 0f;
			if (focusableShip.GameEntity.RayHitEntity(cameraFrame.origin, -cameraFrame.rotation.u, friendlyFocusDistance, ref num4))
			{
				hitScreenPosition = new Vec3(num, num2, num3, -1f);
				closestShip = focusableShip;
				directHitFound = true;
				return;
			}
			Vec2 vec;
			vec..ctor(num, num2);
			float num5 = vec.Distance(screenCenter);
			if (num3 > 0f && num5 < closestDistance && screenCenter.DistanceSquared(vec) < focusRadius * focusRadius)
			{
				closestShip = focusableShip;
				closestDistance = num5;
				hitScreenPosition = new Vec3(num, num2, num3, -1f);
			}
		}

		// Token: 0x060000DD RID: 221 RVA: 0x000090BC File Offset: 0x000072BC
		private void SetFocusedShip(MissionShip ship)
		{
			this._focusedShip = ship;
			NavalMissionShipHighlightView shipHighlightView = this._shipHighlightView;
			if (shipHighlightView == null)
			{
				return;
			}
			shipHighlightView.OnShipFocused(ship);
		}

		// Token: 0x060000DE RID: 222 RVA: 0x000090D8 File Offset: 0x000072D8
		private Vec3 GetShipFocusPosition(MissionShip ship)
		{
			return ship.GameEntity.GlobalPosition + Vec3.Up * 3f;
		}

		// Token: 0x060000DF RID: 223 RVA: 0x00009108 File Offset: 0x00007308
		private void UpdateShipValues()
		{
			if (this._dataSource != null)
			{
				this._dataSource.IsControllingShip = this._playerControlledShip != null;
				this._dataSource.IsUsingBallistaRemotely = base.IsAimingWithRangedWeaponAndAllowed;
				this._dataSource.IsUsingBallistaDirectly = base.DirectlyControlledRangedSiegeWeapon != null;
				if (base.RangedSiegeWeapon != null || base.DirectlyControlledRangedSiegeWeapon != null)
				{
					MissionShipControlVM dataSource = this._dataSource;
					RangedSiegeWeapon rangedSiegeWeapon = base.RangedSiegeWeapon;
					dataSource.BallistaAmmoCount = ((rangedSiegeWeapon != null) ? rangedSiegeWeapon.AmmoCount : base.DirectlyControlledRangedSiegeWeapon.AmmoCount);
					this._dataSource.IsAmmoCountWarned = this._dataSource.BallistaAmmoCount <= 3;
				}
			}
			if (this._playerControlledShip == null || base.Mission.Scene == null || this._dataSource == null)
			{
				return;
			}
			bool flag = false;
			bool flag2 = false;
			bool flag3 = true;
			bool flag4 = true;
			foreach (MissionSail missionSail in this._playerControlledShip.Sails)
			{
				if (missionSail.SailObject.Type == 1)
				{
					flag = true;
					if (missionSail.TargetSailSetting <= 0f)
					{
						flag4 = false;
					}
				}
				else if (missionSail.SailObject.Type == null)
				{
					flag2 = true;
					if (missionSail.TargetSailSetting <= 0f)
					{
						flag3 = false;
					}
				}
			}
			this._playerControlledShipHasHybridSails = flag && flag2;
			if (this._playerControlledShipHasHybridSails)
			{
				if (flag4 && flag3)
				{
					this._dataSource.SetSailState(SailInput.Full);
				}
				else if (!flag4 && !flag3)
				{
					this._dataSource.SetSailState(SailInput.Raised);
				}
				else
				{
					this._dataSource.SetSailState(SailInput.SquareSailsRaised);
				}
			}
			else if (flag)
			{
				this._dataSource.SetSailState(flag4 ? SailInput.Full : SailInput.Raised);
			}
			else
			{
				this._dataSource.SetSailState(flag3 ? SailInput.Full : SailInput.Raised);
			}
			this._dataSource.SetOarsmanLevel(this._playerControlledShip.ShipOrder.OarsmenLevel);
			this._dataSource.SetSailType(flag, flag2);
			Vec2 vec = base.Mission.Scene.GetGlobalWindStrengthVector().Normalized();
			Vec2 vec2 = this._playerControlledShip.GlobalFrame.rotation.f.AsVec2.Normalized();
			this._dataSource.ProjectedWindDirection = MissionGauntletShipControlView.GetProjection(vec2, vec).Normalized();
			this.UpdateHitPoints();
			MissionShipControlVM dataSource2 = this._dataSource;
			MissionShip playerControlledShip = this._playerControlledShip;
			dataSource2.IsCutLooseOrderActive = playerControlledShip != null && playerControlledShip.ShipOrder.GetIsCuttingLoose() && this._isAnyBridgeActive;
			MissionShipControlVM dataSource3 = this._dataSource;
			MissionShip playerControlledShip2 = this._playerControlledShip;
			dataSource3.IsAttemptBoardingOrderActive = playerControlledShip2 != null && playerControlledShip2.ShipOrder.GetIsAttemptingBoarding();
			if (!this._dataSource.IsAttemptBoardingOrderActive)
			{
				this._dataSource.SetBoardingTargetShip(null, -5000f, -5000f, -1f);
				return;
			}
			MissionShip playerControlledShip3 = this._playerControlledShip;
			MissionShip missionShip = ((playerControlledShip3 != null) ? playerControlledShip3.ShipOrder.GetBoardingTargetShip() : null);
			if (missionShip != null)
			{
				Vec3 shipFocusPosition = this.GetShipFocusPosition(missionShip);
				float num = -5000f;
				float num2 = -5000f;
				float num3 = -5000f;
				MBWindowManager.WorldToScreenInsideUsableArea(base.MissionScreen.CombatCamera, shipFocusPosition, ref num, ref num2, ref num3);
				this._dataSource.SetBoardingTargetShip(missionShip, num, num2 - 70f, num3);
				return;
			}
			this._dataSource.SetBoardingTargetShip(null, -5000f, -5000f, -1f);
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x00009464 File Offset: 0x00007664
		private static Vec2 GetProjection(Vec2 from, Vec2 to)
		{
			Vec2 vec = from.Normalized();
			Vec2 vec2;
			vec2..ctor(-vec.y, vec.x);
			return new Vector2(Vec2.DotProduct(to, vec), Vec2.DotProduct(to, vec2));
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x000094A8 File Offset: 0x000076A8
		private void SetControlKeys()
		{
			GameKeyContext category = HotKeyManager.GetCategory("NavalShipControlsHotKeyCategory");
			GameKeyContext category2 = HotKeyManager.GetCategory("CombatHotKeyCategory");
			this._dataSource.SetChangeCameraKey(category.GetGameKey(112));
			this._dataSource.SetCutLooseKey(category.GetGameKey(111));
			this._dataSource.SetToggleOarsmenKey(category.GetGameKey(111));
			this._dataSource.SetToggleSailKey(category.GetGameKey(110));
			this._dataSource.SetToggleBallistaKey(category.GetGameKey(115));
			this._dataSource.SetAttemptBoardingKey(category.GetGameKey(114));
			this._dataSource.SetStopUsingShipKey(category2.GetGameKey(13));
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x00009550 File Offset: 0x00007750
		private void RefreshControlKeys()
		{
			if (this._playerControlledShip == null || base.MissionScreen.IsPhotoModeEnabled || base.IsDisplayingADialog)
			{
				if (this._dataSource != null)
				{
					this._dataSource.ChangeCameraKey.IsVisible = false;
					this._dataSource.CutLooseKey.IsVisible = false;
					this._dataSource.ToggleOarsmenKey.IsVisible = false;
					this._dataSource.ToggleSailKey.IsVisible = false;
					this._dataSource.ToggleBallistaKey.IsVisible = false;
					this._dataSource.AttemptBoardingKey.IsVisible = false;
					this._dataSource.StopUsingShipKey.IsVisible = false;
				}
				MissionGauntletNavalAgentStatus agentStatusView = this._agentStatusView;
				if (agentStatusView == null)
				{
					return;
				}
				agentStatusView.UpdateShipInteractionTexts(null, false, false, false, false, false);
				return;
			}
			else
			{
				if (this._dataSource != null)
				{
					this._dataSource.ChangeCameraKey.IsVisible = this.GetCanChangeCamera();
					this._dataSource.CutLooseKey.IsVisible = this.GetCanCutLoose();
					this._dataSource.CutLooseKey.IsDisabled = this.GetIsCutLooseTemporarilyBlocked();
					this._dataSource.ToggleOarsmenKey.IsVisible = this.GetCanToggleOarsmen();
					this._dataSource.ToggleSailKey.IsVisible = this.GetCanToggleSail();
					this._dataSource.ToggleBallistaKey.IsVisible = this.GetCanToggleRangedWeaponOrderMode();
					this._dataSource.AttemptBoardingKey.IsVisible = this.GetCanAttemptBoarding();
					this._dataSource.AttemptBoardingKey.IsDisabled = !this.GetIsCancelBoardingAvailable() && this.GetIsAttemptBoardingTemporarilyBlocked();
					this._dataSource.StopUsingShipKey.IsVisible = true;
				}
				MissionGauntletNavalAgentStatus agentStatusView2 = this._agentStatusView;
				if (agentStatusView2 == null)
				{
					return;
				}
				MissionShip focusedShip = this._focusedShip;
				IShipOrigin shipOrigin = ((focusedShip != null) ? focusedShip.ShipOrigin : null);
				MissionShip focusedShip2 = this._focusedShip;
				bool flag;
				if (focusedShip2 == null)
				{
					flag = false;
				}
				else
				{
					Team team = focusedShip2.Team;
					TeamSideEnum? teamSideEnum = ((team != null) ? new TeamSideEnum?(team.TeamSide) : null);
					TeamSideEnum teamSideEnum2 = 2;
					flag = (teamSideEnum.GetValueOrDefault() == teamSideEnum2) & (teamSideEnum != null);
				}
				agentStatusView2.UpdateShipInteractionTexts(shipOrigin, flag, this.GetCanSelectShip(), this.GetCanAttemptBoarding(), this.GetIsAttemptBoardingTemporarilyBlocked(), this.GetIsCancelBoardingAvailable());
				return;
			}
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x00009764 File Offset: 0x00007964
		private bool GetCanAttemptBoarding()
		{
			return !this.IsFeatureSuspended(MissionGauntletShipControlView.ShipControlFeatureFlags.AttemptBoarding) && (this._focusedShip != null && !this._focusedShip.IsConnectionPermanentlyBlocked() && this._focusedShip.ShipOrder.IsBoardingAvailable && !this._playerControlledShip.GetIsThereActiveBridgeTo(this._focusedShip) && (this.GetIsCancelBoardingAvailable() ? (this._focusedShip.GameEntity.GlobalPosition.Distance(this._playerControlledShip.GameEntity.GlobalPosition) <= 300f) : (this._focusedShip.GameEntity.GlobalPosition.Distance(this._playerControlledShip.GameEntity.GlobalPosition) <= 50f))) && !base.IsAimingWithRangedWeaponAndAllowed;
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x00009849 File Offset: 0x00007A49
		private bool GetIsAttemptBoardingTemporarilyBlocked()
		{
			MissionShip focusedShip = this._focusedShip;
			return (focusedShip != null && focusedShip.IsConnectionBlocked()) || this._playerControlledShip.ShipOrder.GetBoardingTargetShip() == this._focusedShip;
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x00009879 File Offset: 0x00007A79
		private bool GetIsCancelBoardingAvailable()
		{
			MissionShip playerControlledShip = this._playerControlledShip;
			return playerControlledShip != null && playerControlledShip.ShipOrder.GetIsAttemptingBoarding() && this._playerControlledShip.ShipOrder.GetBoardingTargetShip() == this._focusedShip;
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x000098AE File Offset: 0x00007AAE
		private bool GetCanChangeCamera()
		{
			return !this.IsFeatureSuspended(MissionGauntletShipControlView.ShipControlFeatureFlags.ChangeCamera) && !base.IsAimingWithRangedWeaponAndAllowed;
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x000098C8 File Offset: 0x00007AC8
		private bool GetCanCutLoose()
		{
			return !this.IsFeatureSuspended(MissionGauntletShipControlView.ShipControlFeatureFlags.CutLoose) && this._isAnyBridgeActive;
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x000098DC File Offset: 0x00007ADC
		private bool GetIsCutLooseTemporarilyBlocked()
		{
			return this._playerControlledShip.ShipOrder.GetIsCuttingLoose() || this._playerControlledShip.IsDisconnectionBlocked();
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x00009900 File Offset: 0x00007B00
		private bool GetCanSelectShip()
		{
			if (this.IsFeatureSuspended(MissionGauntletShipControlView.ShipControlFeatureFlags.ShipSelection))
			{
				return false;
			}
			if (this._orderUIHandler != null)
			{
				MissionShip focusedShip = this._focusedShip;
				if (((focusedShip != null) ? focusedShip.Formation : null) != null && this._focusedShip.Formation.CountOfUnits > 0 && this._focusedShip.Team.IsPlayerTeam && this._focusedShip.Formation.PlayerOwner == Agent.Main && this._focusedShip.GameEntity.GlobalPosition.Distance(this._playerControlledShip.GameEntity.GlobalPosition) <= 300f)
				{
					return !base.IsAimingWithRangedWeaponAndAllowed;
				}
			}
			return false;
		}

		// Token: 0x060000EA RID: 234 RVA: 0x000099B3 File Offset: 0x00007BB3
		private bool GetCanToggleOarsmen()
		{
			return !this.IsFeatureSuspended(MissionGauntletShipControlView.ShipControlFeatureFlags.ToggleOarsmen) && !this._isAnyBridgeActive && !this._playerControlledShip.ShipOrder.IsOarsmenLevelLocked();
		}

		// Token: 0x060000EB RID: 235 RVA: 0x000099DD File Offset: 0x00007BDD
		private bool GetCanToggleSail()
		{
			return !this.IsFeatureSuspended(MissionGauntletShipControlView.ShipControlFeatureFlags.ToggleSails) && !this._isAnyBridgeActive && this._playerControlledShip.ShipSailState == MissionShip.SailState.Intact;
		}

		// Token: 0x060000EC RID: 236 RVA: 0x00009A03 File Offset: 0x00007C03
		private bool GetCanToggleRangedWeaponOrderMode()
		{
			return this.GetIsRangedWeaponAvailable() && base.IsAimingWithRangedWeaponAllowed;
		}

		// Token: 0x060000ED RID: 237 RVA: 0x00009A18 File Offset: 0x00007C18
		private bool GetIsRangedWeaponAvailable()
		{
			return !this.IsFeatureSuspended(MissionGauntletShipControlView.ShipControlFeatureFlags.BallistaOrder) && (this._playerControlledShip.ShipSiegeWeapon != null && !this._playerControlledShip.ShipSiegeWeapon.IsDisabled && !this._playerControlledShip.ShipSiegeWeapon.IsDeactivated) && !this._playerControlledShip.ShipSiegeWeapon.IsDestroyed;
		}

		// Token: 0x060000EE RID: 238 RVA: 0x00009A77 File Offset: 0x00007C77
		private bool GetCanShootBallista()
		{
			return !this.IsFeatureSuspended(MissionGauntletShipControlView.ShipControlFeatureFlags.ShootBallista) && (base.IsAimingWithRangedWeaponAndAllowed && this._playerControlledShip.ShipSiegeWeapon != null) && this._playerControlledShip.ShipSiegeWeapon.UserCountNotInStruckAction > 0;
		}

		// Token: 0x060000EF RID: 239 RVA: 0x00009AB2 File Offset: 0x00007CB2
		private void UpdateVisibility()
		{
			if (this._gauntletLayer == null)
			{
				return;
			}
			this._gauntletLayer.UIContext.ContextAlpha = (float)((this._isBattleUIVisible && !this._isPhotoModeActive && !base.IsViewSuspended) ? 1 : 0);
		}

		// Token: 0x04000074 RID: 116
		private GauntletLayer _gauntletLayer;

		// Token: 0x04000075 RID: 117
		private MissionShipControlVM _dataSource;

		// Token: 0x04000076 RID: 118
		private MissionGauntletSingleplayerOrderUIHandler _orderUIHandler;

		// Token: 0x04000077 RID: 119
		private MissionGauntletCrosshair _crosshairView;

		// Token: 0x04000078 RID: 120
		private NavalMissionShipHighlightView _shipHighlightView;

		// Token: 0x04000079 RID: 121
		private MissionGauntletNavalAgentStatus _agentStatusView;

		// Token: 0x0400007A RID: 122
		private MissionShip _playerControlledShip;

		// Token: 0x0400007B RID: 123
		private MissionShip _focusedShip;

		// Token: 0x0400007C RID: 124
		private bool _playerControlledShipHasHybridSails;

		// Token: 0x0400007D RID: 125
		private bool _isAnyBridgeActive;

		// Token: 0x0400007E RID: 126
		private bool _isBattleUIVisible;

		// Token: 0x0400007F RID: 127
		private bool _isPhotoModeActive;

		// Token: 0x04000080 RID: 128
		private bool _lastFirstPersonModeSelection;

		// Token: 0x04000081 RID: 129
		private const float AttemptBoardingDistance = 50f;

		// Token: 0x04000082 RID: 130
		private const float SelectShipDistance = 300f;

		// Token: 0x02000035 RID: 53
		[Flags]
		public enum ShipControlFeatureFlags
		{
			// Token: 0x040000C8 RID: 200
			ShipFocus = 1,
			// Token: 0x040000C9 RID: 201
			ShipSelection = 2,
			// Token: 0x040000CA RID: 202
			AttemptBoarding = 4,
			// Token: 0x040000CB RID: 203
			ToggleOarsmen = 8,
			// Token: 0x040000CC RID: 204
			ToggleSails = 16,
			// Token: 0x040000CD RID: 205
			CutLoose = 32,
			// Token: 0x040000CE RID: 206
			BallistaOrder = 64,
			// Token: 0x040000CF RID: 207
			ShootBallista = 128,
			// Token: 0x040000D0 RID: 208
			ChangeCamera = 256
		}
	}
}
