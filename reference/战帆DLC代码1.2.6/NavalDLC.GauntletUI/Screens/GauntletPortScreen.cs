using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using NavalDLC.Missions.NavalPhysics;
using NavalDLC.Missions.Objects;
using NavalDLC.View;
using NavalDLC.ViewModelCollection.Port;
using NavalDLC.ViewModelCollection.Port.PortScreenHandlers;
using SandBox.View;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Objects;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace NavalDLC.GauntletUI.Screens
{
	// Token: 0x02000011 RID: 17
	[GameStateScreen(typeof(PortState))]
	public class GauntletPortScreen : ScreenBase, IGameStateListener, IChangeableScreen
	{
		// Token: 0x06000035 RID: 53 RVA: 0x00002A84 File Offset: 0x00000C84
		public GauntletPortScreen(PortState portState)
		{
			this._portState = portState;
			this._initialCameraValues = new GauntletPortScreen.CameraParameters(2.2f, 1.45f, 40f, 0f);
			this._staticCameraValues = new GauntletPortScreen.StaticCameraParameters(0.2f, 0.1f, 0.015f, 1920f, 15f, 25f, 0.7853982f, 2.0943952f, 1.6580628f, 15f, 50f, 5f, 15f, 50f, 3000f, 0f, 6f);
			this._shipVisualInfos = new Dictionary<Ship, GauntletPortScreen.PortShipVisualInfo>();
			this._isInSettlementPort = Settlement.CurrentSettlement != null && Settlement.CurrentSettlement.HasPort && Settlement.CurrentSettlement.SiegeEvent == null;
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00002B67 File Offset: 0x00000D67
		protected override void OnInitialize()
		{
			base.OnInitialize();
			InformationManager.HideAllMessages();
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00002B74 File Offset: 0x00000D74
		protected override void OnFinalize()
		{
			base.OnFinalize();
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00002B7C File Offset: 0x00000D7C
		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			if (!this._sceneLayer.SceneView.ReadyToRender() || !this._sceneLayer.SceneView.CheckSceneReadyToRender())
			{
				return;
			}
			if (!this._isInitialized)
			{
				this._scene.WaitWaterRendererCPUSimulation();
				this.InitializeView();
				this._isInitialized = true;
				this._framesToWaitAfterInit = 10;
			}
			this._dataSource.OnTick(dt);
			this._scene.Tick(dt);
			if (this._framesToWaitAfterInit > 0)
			{
				this._framesToWaitAfterInit--;
				return;
			}
			if (LoadingWindow.IsLoadingWindowActive)
			{
				LoadingWindow.DisableGlobalLoadingWindow();
				return;
			}
			this.TickSceneInput(dt);
			this.TickDataSourceInput();
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00002C27 File Offset: 0x00000E27
		protected override void OnActivate()
		{
			base.OnActivate();
			if (this._gauntletLayer != null)
			{
				ScreenManager.SetSuspendLayer(this._gauntletLayer, false);
			}
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00002C43 File Offset: 0x00000E43
		protected override void OnDeactivate()
		{
			base.OnDeactivate();
			if (this._gauntletLayer != null)
			{
				ScreenManager.SetSuspendLayer(this._gauntletLayer, true);
			}
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00002C5F File Offset: 0x00000E5F
		void IGameStateListener.OnActivate()
		{
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00002C61 File Offset: 0x00000E61
		void IGameStateListener.OnDeactivate()
		{
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00002C64 File Offset: 0x00000E64
		private void InitializeView()
		{
			this._shipPiecesCategory = UIResourceManager.LoadSpriteCategory("ui_naval_ship_pieces");
			this._portCategory = UIResourceManager.LoadSpriteCategory("ui_port");
			this._clanCategory = UIResourceManager.LoadSpriteCategory("ui_clan");
			this._characterdeveloperCategory = UIResourceManager.LoadSpriteCategory("ui_characterdeveloper");
			Campaign campaign = Campaign.Current;
			this._viewDataTracker = ((campaign != null) ? campaign.GetCampaignBehavior<IViewDataTracker>() : null);
			PortScreenHandler portScreenHandler;
			switch (this._portState.PortScreenMode)
			{
			case 0:
				portScreenHandler = new PortScreenStoryModeHandler(this._portState.LeftOwner, this._portState.RightOwner);
				break;
			case 1:
				portScreenHandler = new PortScreenRestrictedModeHandler(this._portState.LeftOwner, this._portState.RightOwner);
				break;
			case 2:
				portScreenHandler = new PortScreenTradeModeHandler(this._portState.LeftOwner, this._portState.RightOwner);
				break;
			case 3:
				portScreenHandler = new PortScreenLootModeHandler(GameTexts.FindText("str_loot", null), this._portState.RightOwner, this._portState.LeftShips, this._portState.RightShips);
				break;
			case 4:
				portScreenHandler = new PortScreenManageFleetModeHandler(GameTexts.FindText("str_port_discard_ship", null), this._portState.RightOwner, this._portState.LeftShips, this._portState.RightShips);
				break;
			case 5:
				portScreenHandler = new PortScreenManageOtherFleetModeHandler(this._portState.LeftOwner);
				break;
			default:
				Debug.FailedAssert("Trying to initialize Port Screen with invalid PortScreenMode. Falling back to manage mode", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.GauntletUI\\Screens\\GauntletPortScreen.cs", "InitializeView", 212);
				portScreenHandler = new PortScreenManageFleetModeHandler(GameTexts.FindText("str_port_discard_ship", null), this._portState.RightOwner, this._portState.LeftShips, this._portState.RightShips);
				break;
			}
			this._dataSource = new PortVM(portScreenHandler, this._portState.PortScreenMode, new Action<Ship>(this.OnShipSelected), new Action(this.OnRostersRefreshed), new Action<ShipItemVM>(this.RefreshShipVisual), new Action(this.OnUpgradeSlotSelected));
			this.InitializeShipVisuals();
			this._dataSource.SelectFirstAvailableRosterAndShip();
			this._dataSource.IsNight = this._scene.TimeOfDay <= 4f || this._scene.TimeOfDay >= 20f;
			this._gauntletLayer = new GauntletLayer("PortScreen", 10, false);
			this._gauntletLayer.LoadMovie("PortScreen", this._dataSource);
			this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("PortHotKeyCategory"));
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory"));
			this._dataSource.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
			this._dataSource.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
			this._dataSource.SetResetInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Reset"));
			this._dataSource.SetSelectPreviousShipInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("SwitchToPreviousTab"));
			this._dataSource.SetSelectNextShipInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("SwitchToNextTab"));
			this._dataSource.SetSelectLeftRosterInputKey(HotKeyManager.GetCategory("PortHotKeyCategory").GetHotKey("SelectLeftRoster"));
			this._dataSource.SetSelectRightRosterInputKey(HotKeyManager.GetCategory("PortHotKeyCategory").GetHotKey("SelectRightRoster"));
			this._dataSource.AddGamepadCameraControlInputKey(HotKeyManager.GetCategory("PortHotKeyCategory").RegisteredGameAxisKeys.FirstOrDefault<GameAxisKey>((GameAxisKey x) => x.Id == "MovementAxisX"));
			this._dataSource.AddGamepadCameraControlInputKey(HotKeyManager.GetCategory("PortHotKeyCategory").RegisteredGameAxisKeys.FirstOrDefault<GameAxisKey>((GameAxisKey x) => x.Id == "CameraAxisX"));
			this._dataSource.AddGamepadCameraControlInputKey(HotKeyManager.GetCategory("PortHotKeyCategory").GetHotKey("ResetCamera"));
			this._dataSource.SetGamepadToggleCameraInputKey(HotKeyManager.GetCategory("PortHotKeyCategory").GetHotKey("ToggleCameraMovement"));
			this._dataSource.AddKeyboardMoveCameraInputKey(HotKeyManager.GetCategory("PortHotKeyCategory").RegisteredGameAxisKeys.FirstOrDefault<GameAxisKey>((GameAxisKey x) => x.Id == "MovementAxisY").PositiveKey);
			this._dataSource.AddKeyboardMoveCameraInputKey(HotKeyManager.GetCategory("PortHotKeyCategory").RegisteredGameAxisKeys.FirstOrDefault<GameAxisKey>((GameAxisKey x) => x.Id == "MovementAxisX").NegativeKey);
			this._dataSource.AddKeyboardMoveCameraInputKey(HotKeyManager.GetCategory("PortHotKeyCategory").RegisteredGameAxisKeys.FirstOrDefault<GameAxisKey>((GameAxisKey x) => x.Id == "MovementAxisY").NegativeKey);
			this._dataSource.AddKeyboardMoveCameraInputKey(HotKeyManager.GetCategory("PortHotKeyCategory").RegisteredGameAxisKeys.FirstOrDefault<GameAxisKey>((GameAxisKey x) => x.Id == "MovementAxisX").PositiveKey);
			this._dataSource.SetKeyboardRotateCameraInputKey(HotKeyManager.GetCategory("PortHotKeyCategory").GetHotKey("ToggleCameraMovement"));
			base.AddLayer(this._gauntletLayer);
			this.ResetCamera(true);
		}

		// Token: 0x0600003E RID: 62 RVA: 0x0000320D File Offset: 0x0000140D
		void IGameStateListener.OnInitialize()
		{
			LoadingWindow.EnableGlobalLoadingWindow();
			this._isInitialized = false;
		}

		// Token: 0x0600003F RID: 63 RVA: 0x0000321B File Offset: 0x0000141B
		protected override void OnReady()
		{
			base.OnReady();
			if (this._scene != null)
			{
				return;
			}
			this.CreateScene();
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00003238 File Offset: 0x00001438
		void IGameStateListener.OnFinalize()
		{
			if (this._isInitialized)
			{
				this._shipPiecesCategory.Unload();
				this._portCategory.Unload();
				this._clanCategory.Unload();
				this._characterdeveloperCategory.Unload();
				base.RemoveLayer(this._gauntletLayer);
				this._dataSource.OnFinalize();
				this._gauntletLayer = null;
				this._dataSource = null;
				if (this._underwaterSoundEvent != null)
				{
					this._underwaterSoundEvent.Release();
					this._underwaterSoundEvent = null;
					SoundManager.SetGlobalParameter("isUnderwater", 0f);
				}
			}
			if (this._sceneLayer != null && this._scene != null)
			{
				this.DestroyScene();
			}
		}

		// Token: 0x06000041 RID: 65 RVA: 0x000032E4 File Offset: 0x000014E4
		private void CreateScene()
		{
			this._scene = Scene.CreateNewScene(true, false, 0, "mono_renderscene");
			this._scene.SetUseAdvancedWaterRendering(true);
			SceneInitializationData sceneInitializationData = default(SceneInitializationData);
			sceneInitializationData.InitPhysicsWorld = true;
			sceneInitializationData.InitFloraNodes = true;
			SceneInitializationData sceneInitializationData2 = sceneInitializationData;
			this._scene.Read(this._isInSettlementPort ? "prototype_port_scene_wide" : "scn_port", ref sceneInitializationData2, "");
			CampaignVec2 campaignVec = (this._isInSettlementPort ? Settlement.CurrentSettlement.PortPosition : Campaign.Current.MainParty.Position);
			AtmosphereInfo atmosphereModel = Campaign.Current.Models.MapWeatherModel.GetAtmosphereModel(campaignVec);
			float num = MathF.Max(4f, atmosphereModel.NauticalInfo.WindVector.Length);
			float num2 = MathF.Max(2f, num / 4f);
			this._scene.EnableFixedTick();
			this._scene.SetClothSimulationState(true);
			this._scene.EnableInclusiveAsyncPhysx();
			this._scene.SetWaterStrength(num2);
			Scene scene = this._scene;
			Vec2 vec = num * (this._isInSettlementPort ? (-Vec2.Side) : Vec2.Forward);
			scene.SetGlobalWindVelocity(ref vec);
			this._scene.SetPhotoAtmosphereViaTod(atmosphereModel.TimeInfo.TimeOfDay, num > 20f);
			this._sceneLayer = new SceneLayer(true, true);
			this._sceneLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("PortHotKeyCategory"));
			this._sceneLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			this._sceneLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory"));
			this._sceneLayer.InputRestrictions.SetInputRestrictions(false, 7);
			this._sceneLayer.SceneView.SetScene(this._scene);
			this._sceneLayer.SceneView.SetSceneUsesShadows(true);
			this._sceneLayer.SceneView.SetAcceptGlobalDebugRenderObjects(true);
			this._sceneLayer.SceneView.SetRenderWithPostfx(true);
			this._sceneLayer.SceneView.SetResolutionScaling(true);
			this._shipSpawnPositionEntity = this._scene.FindEntityWithName("ship_spawn_point");
			GameEntity shipSpawnPositionEntity = this._shipSpawnPositionEntity;
			if (shipSpawnPositionEntity != null)
			{
				GameEntityPhysicsExtensions.SetPhysicsState(shipSpawnPositionEntity, false, true);
			}
			GameEntity shipSpawnPositionEntity2 = this._shipSpawnPositionEntity;
			Vec3 vec2;
			if (shipSpawnPositionEntity2 == null)
			{
				vec2 = Vec3.Forward;
			}
			else
			{
				vec = shipSpawnPositionEntity2.GetFrame().rotation.f.AsVec2;
				vec2 = vec.ToVec3(0f);
			}
			this._shipForwardDirection = vec2;
			this._shipSideDirection = Vec3.CrossProduct(Vec3.Up, this._shipForwardDirection);
			this.InitializeCamera();
			base.AddLayer(this._sceneLayer);
		}

		// Token: 0x06000042 RID: 66 RVA: 0x0000358C File Offset: 0x0000178C
		private void InitializeCamera()
		{
			GameEntity gameEntity = this._scene.FindEntityWithName("camera_position");
			GameEntityPhysicsExtensions.SetPhysicsState(gameEntity, false, true);
			this._sceneCamera = Camera.CreateCamera();
			this._sceneCamera.Frame = gameEntity.GetFrame();
			this._sceneCamera.SetFovHorizontal(1.5707964f, Screen.AspectRatio, 0.1f, 2000f);
			this.ResetCamera(true);
			this.UpdateCamera(1f);
			this._sceneLayer.SetCamera(this._sceneCamera);
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00003610 File Offset: 0x00001810
		private void DestroyScene()
		{
			base.RemoveLayer(this._sceneLayer);
			this._sceneLayer.ClearAll();
			this._scene.WaitWaterRendererCPUSimulation();
			this._scene.ClearAll();
			this._scene.ManualInvalidate();
			this._scene = null;
			this._shipSpawnPositionEntity = null;
			this._shipVisualInfos.Clear();
			this._sceneCamera = null;
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00003678 File Offset: 0x00001878
		private void InitializeShipVisuals()
		{
			Vec3 vec = this._shipSpawnPositionEntity.GetFrame().origin;
			int num = (this._isInSettlementPort ? 0 : (-this._dataSource.RightRoster.Ships.Count / 2));
			foreach (ShipItemVM shipItemVM in this._dataSource.RightRoster.Ships)
			{
				this.SpawnShipVisual(shipItemVM.Ship, vec + this.GetPositionOffsetForIndex(num, false), this.GetExtraRotationInRadiansForIndex(num, false));
				num++;
			}
			if (this._isInSettlementPort)
			{
				vec -= Vec3.Forward * 75f;
			}
			else
			{
				vec += Vec3.Forward * 100f;
			}
			num = (this._isInSettlementPort ? 0 : (-this._dataSource.LeftRoster.Ships.Count / 2));
			foreach (ShipItemVM shipItemVM2 in this._dataSource.LeftRoster.Ships)
			{
				this.SpawnShipVisual(shipItemVM2.Ship, vec + this.GetPositionOffsetForIndex(num, true), this.GetExtraRotationInRadiansForIndex(num, true));
				num++;
			}
		}

		// Token: 0x06000045 RID: 69 RVA: 0x000037E4 File Offset: 0x000019E4
		private void SpawnShipVisual(Ship ship, Vec3 position, float rotation)
		{
			List<ShipVisualSlotInfo> shipVisualSlotInfos = ship.GetShipVisualSlotInfos();
			GameEntity shipEntity = NavalDLCViewHelpers.ShipVisualHelper.GetShipEntity(ship, this._scene, shipVisualSlotInfos, true);
			MatrixFrame frame = this._shipSpawnPositionEntity.GetFrame();
			frame.origin = position;
			frame.origin.z = this._scene.GetWaterLevelAtPosition(frame.origin.AsVec2, true, false) - shipEntity.GetFirstScriptOfType<NavalPhysics>().StabilitySubmergedHeightOfShip;
			frame.rotation.RotateAboutUp(rotation);
			GameEntityPhysicsExtensions.SetPhysicsState(shipEntity, true, false);
			shipEntity.SetFrame(ref frame, true);
			shipEntity.GetFirstScriptOfType<NavalPhysics>().SetAnchor(true, true, 1f);
			this.RotateOars(shipEntity);
			this.RotateSails(shipEntity);
			ShipWaterEffects firstScriptOfTypeRecursive = shipEntity.GetFirstScriptOfTypeRecursive<ShipWaterEffects>();
			if (firstScriptOfTypeRecursive != null)
			{
				firstScriptOfTypeRecursive.EnableWakeAndParticles();
			}
			this._shipVisualInfos.Add(ship, new GauntletPortScreen.PortShipVisualInfo(shipEntity, frame.origin, frame.origin + this.GetVisualCenterOffsetForShip(shipEntity), false));
		}

		// Token: 0x06000046 RID: 70 RVA: 0x000038C8 File Offset: 0x00001AC8
		private void RotateOars(GameEntity visualShip)
		{
			foreach (GameEntity gameEntity in MBExtensions.CollectChildrenEntitiesWithTag(visualShip, "oar"))
			{
				MatrixFrame frame = gameEntity.GetFrame();
				frame.Rotate(-1.0471976f, ref Vec3.Side);
				gameEntity.SetFrame(ref frame, true);
			}
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00003938 File Offset: 0x00001B38
		private void RotateSails(GameEntity visualShip)
		{
			ShipVisual firstScriptOfType = visualShip.GetFirstScriptOfType<ShipVisual>();
			if (firstScriptOfType != null)
			{
				foreach (ScriptComponentBehavior scriptComponentBehavior in firstScriptOfType.SailVisuals)
				{
					SailVisual sailVisual = scriptComponentBehavior as SailVisual;
					if (sailVisual.Type == SailVisual.SailType.LateenSail)
					{
						MatrixFrame localFrame = sailVisual.SailYawRotationEntity.GetLocalFrame();
						localFrame.rotation = Mat3.Identity;
						localFrame.rotation.RotateAboutUp(0.87266463f);
						sailVisual.SailYawRotationEntity.SetLocalFrame(ref localFrame, false);
					}
				}
			}
		}

		// Token: 0x06000048 RID: 72 RVA: 0x000039D4 File Offset: 0x00001BD4
		private Vec3 GetPositionOffsetForIndex(int i, bool isOppositeSide)
		{
			Vec3 vec;
			Vec3 vec2;
			if (this._isInSettlementPort)
			{
				vec = Vec3.Forward * 45f * (float)(i % 4);
				vec2 = Vec3.Side * -60f * (float)(i / 4);
			}
			else
			{
				vec2 = Vec3.Side * -45f * (float)i;
				vec = Vec3.Forward * -20f * (float)MathF.Abs(i);
			}
			if (isOppositeSide)
			{
				vec *= -1f;
			}
			Vec3 vec3 = (MBRandom.RandomFloatWithSeed((uint)i, (uint)(i + (isOppositeSide ? 1 : 0))) - 0.5f) * 8f * Vec3.Side + (MBRandom.RandomFloatWithSeed((uint)i, (uint)(i + (isOppositeSide ? 3 : 2))) - 0.5f) * 8f * Vec3.Forward;
			return vec2 + vec + vec3;
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00003ABC File Offset: 0x00001CBC
		private float GetExtraRotationInRadiansForIndex(int i, bool isOppositeSide)
		{
			return (MBRandom.RandomFloatWithSeed((uint)i, (uint)(i + (isOppositeSide ? 1 : 0))) - 0.5f) * 20f * 0.017453292f;
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00003AE0 File Offset: 0x00001CE0
		private Vec3 GetVisualCenterOffsetForShip(GameEntity shipEntity)
		{
			GameEntity firstChildEntityWithTagRecursive = shipEntity.GetFirstChildEntityWithTagRecursive("body_mesh");
			MetaMesh metaMesh = ((firstChildEntityWithTagRecursive != null) ? firstChildEntityWithTagRecursive.GetMetaMesh(0) : null);
			if (metaMesh != null)
			{
				BoundingBox boundingBox = metaMesh.GetBoundingBox();
				return new Vec3(boundingBox.center.AsVec2, MathF.Lerp(boundingBox.center.Z, boundingBox.max.Z, 0.4f, 1E-05f), -1f);
			}
			return new Vec3(0f, 0f, 2.5f, -1f);
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00003B70 File Offset: 0x00001D70
		private void RecalculateShipVisibilities()
		{
			foreach (KeyValuePair<Ship, GauntletPortScreen.PortShipVisualInfo> keyValuePair in this._shipVisualInfos.ToList<KeyValuePair<Ship, GauntletPortScreen.PortShipVisualInfo>>())
			{
				Ship key = keyValuePair.Key;
				bool flag = this.ShouldShipBeHidden(key);
				if (keyValuePair.Value.IsHidden != flag)
				{
					this._shipVisualInfos[key] = new GauntletPortScreen.PortShipVisualInfo(keyValuePair.Value.VisualEntity, keyValuePair.Value.InitialPosition, keyValuePair.Value.VisualCenterPosition, flag);
				}
				keyValuePair.Value.VisualEntity.SetVisibilityExcludeParents(!flag);
			}
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00003C2C File Offset: 0x00001E2C
		private bool ShouldShipBeHidden(Ship ship)
		{
			return !this._dataSource.LeftRoster.Ships.Any<ShipItemVM>((ShipItemVM x) => x.Ship == ship) && !this._dataSource.RightRoster.Ships.Any<ShipItemVM>((ShipItemVM x) => x.Ship == ship);
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00003C90 File Offset: 0x00001E90
		private void RecalculateShipPositions()
		{
			Vec3 vec = this._shipSpawnPositionEntity.GetFrame().origin;
			int num = (this._isInSettlementPort ? 0 : (-this._dataSource.RightRoster.Ships.Count / 2));
			foreach (ShipItemVM shipItemVM in this._dataSource.RightRoster.Ships)
			{
				this.RecalculateShipPosition(shipItemVM.Ship, vec + this.GetPositionOffsetForIndex(num, false), this.GetExtraRotationInRadiansForIndex(num, false));
				num++;
			}
			if (this._isInSettlementPort)
			{
				vec -= Vec3.Forward * 75f;
			}
			else
			{
				vec += Vec3.Forward * 100f;
			}
			num = (this._isInSettlementPort ? 0 : (-this._dataSource.LeftRoster.Ships.Count / 2));
			foreach (ShipItemVM shipItemVM2 in this._dataSource.LeftRoster.Ships)
			{
				this.RecalculateShipPosition(shipItemVM2.Ship, vec + this.GetPositionOffsetForIndex(num, true), this.GetExtraRotationInRadiansForIndex(num, true));
				num++;
			}
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00003DFC File Offset: 0x00001FFC
		private void RecalculateShipPosition(Ship ship, Vec3 position, float rotation)
		{
			GauntletPortScreen.PortShipVisualInfo portShipVisualInfo = this._shipVisualInfos[ship];
			if (portShipVisualInfo.InitialPosition.AsVec2 != position.AsVec2)
			{
				GameEntity visualEntity = portShipVisualInfo.VisualEntity;
				MatrixFrame frame = this._shipSpawnPositionEntity.GetFrame();
				frame.origin = position;
				frame.origin.z = this._scene.GetWaterLevelAtPosition(frame.origin.AsVec2, true, false) - visualEntity.GetFirstScriptOfType<NavalPhysics>().StabilitySubmergedHeightOfShip;
				frame.rotation.RotateAboutUp(rotation);
				visualEntity.GetFirstScriptOfType<NavalPhysics>().SetAnchor(false, false, 1f);
				visualEntity.SetFrame(ref frame, true);
				visualEntity.GetFirstScriptOfType<NavalPhysics>().SetAnchor(true, true, 1f);
				this._shipVisualInfos[ship] = new GauntletPortScreen.PortShipVisualInfo(visualEntity, frame.origin, frame.origin + this.GetVisualCenterOffsetForShip(visualEntity), portShipVisualInfo.IsHidden);
				if (this._currentShipVisualInfo.VisualEntity == visualEntity)
				{
					this._currentShipVisualInfo = this._shipVisualInfos[ship];
				}
			}
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00003F10 File Offset: 0x00002110
		private void RefreshShipVisuals()
		{
			foreach (ShipItemVM shipItemVM in this._dataSource.AllShips)
			{
				this.RefreshShipVisual(shipItemVM);
			}
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00003F68 File Offset: 0x00002168
		private void RefreshShipVisual(ShipItemVM shipItem)
		{
			Ship ship = shipItem.Ship;
			List<ShipVisualSlotInfo> list = new List<ShipVisualSlotInfo>();
			foreach (ShipUpgradeSlotBaseVM shipUpgradeSlotBaseVM in shipItem.Upgrades.UpgradeSlots)
			{
				if (shipUpgradeSlotBaseVM is ShipUpgradeSlotVM)
				{
					List<ShipVisualSlotInfo> list2 = list;
					string shipSlotTag = shipUpgradeSlotBaseVM.ShipSlotTag;
					ShipUpgradePieceVM shipUpgradePieceVM = shipUpgradeSlotBaseVM.SelectedPiece as ShipUpgradePieceVM;
					list2.Add(new ShipVisualSlotInfo(shipSlotTag, ((shipUpgradePieceVM != null) ? shipUpgradePieceVM.Piece.SlotPrefabChildTagId : null) ?? string.Empty));
				}
				else if (shipUpgradeSlotBaseVM is ShipFigureheadSlotVM)
				{
					List<ShipVisualSlotInfo> list3 = list;
					string shipSlotTag2 = shipUpgradeSlotBaseVM.ShipSlotTag;
					ShipFigureheadVM shipFigureheadVM = shipUpgradeSlotBaseVM.SelectedPiece as ShipFigureheadVM;
					list3.Add(new ShipVisualSlotInfo(shipSlotTag2, ((shipFigureheadVM != null) ? shipFigureheadVM.Figurehead.StringId : null) ?? string.Empty));
				}
			}
			uint num;
			uint num2;
			Banner banner;
			if (this._dataSource.LeftRoster.Ships.Contains(shipItem))
			{
				ValueTuple<uint, uint> sailColors = ShipHelper.GetSailColors(this._dataSource.LeftRoster.Owner);
				num = sailColors.Item1;
				num2 = sailColors.Item2;
				banner = ShipHelper.GetShipBanner(this._dataSource.LeftRoster.Owner);
			}
			else
			{
				ValueTuple<uint, uint> sailColors2 = ShipHelper.GetSailColors(this._dataSource.RightRoster.Owner);
				num = sailColors2.Item1;
				num2 = sailColors2.Item2;
				banner = ShipHelper.GetShipBanner(this._dataSource.RightRoster.Owner);
			}
			NavalDLCViewHelpers.ShipVisualHelper.RefreshShipVisuals(this._shipVisualInfos[ship].VisualEntity, list, num, num2, banner, shipItem.CurrentHp / shipItem.MaxHp);
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00004104 File Offset: 0x00002304
		private void OnShipSelected(Ship shipItem)
		{
			if (shipItem != null)
			{
				if (this._shipVisualInfos.ContainsKey(shipItem))
				{
					this._currentShipVisualInfo = this._shipVisualInfos[shipItem];
					using (Dictionary<Ship, GauntletPortScreen.PortShipVisualInfo>.Enumerator enumerator = this._shipVisualInfos.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							KeyValuePair<Ship, GauntletPortScreen.PortShipVisualInfo> keyValuePair = enumerator.Current;
							if (keyValuePair.Value.VisualEntity != this._currentShipVisualInfo.VisualEntity)
							{
								keyValuePair.Value.VisualEntity.AddBodyFlags(65536, true);
							}
							else
							{
								keyValuePair.Value.VisualEntity.RemoveBodyFlags(65536, true);
							}
						}
						goto IL_00BF;
					}
				}
				Debug.FailedAssert("Selected ship item's visual has not been spawned!", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.GauntletUI\\Screens\\GauntletPortScreen.cs", "OnShipSelected", 665);
				IL_00BF:
				this._targetCameraValues.Deviation = this._initialCameraValues.Deviation;
			}
		}

		// Token: 0x06000052 RID: 82 RVA: 0x000041F8 File Offset: 0x000023F8
		private void OnRostersRefreshed()
		{
			if (this._dataSource == null)
			{
				return;
			}
			this.RecalculateShipVisibilities();
			this.RecalculateShipPositions();
			this.RefreshShipVisuals();
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00004218 File Offset: 0x00002418
		private void OnUpgradeSlotSelected()
		{
			if (!this._dataSource.IsAnyUpgradeSlotSelected)
			{
				this.FreeCameraFromUpgradeSlot();
				return;
			}
			string shipSlotTag = this._dataSource.SelectedUpgradeSlot.ShipSlotTag;
			string slotTypeId = this._dataSource.SelectedUpgradeSlot.SlotTypeId;
			if (this._currentSelectedSlotCameraEntity == null)
			{
				this._previousCameraValues = this._currentCameraValues;
			}
			this._currentSelectedSlotCameraEntity = this._currentShipVisualInfo.VisualEntity.GetFirstChildEntityWithTagRecursive(shipSlotTag + "_point");
			if (this._currentSelectedSlotCameraEntity == null)
			{
				Debug.FailedAssert("Slot camera point entity not found!", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.GauntletUI\\Screens\\GauntletPortScreen.cs", "OnUpgradeSlotSelected", 700);
				return;
			}
			this._targetCameraValues.Azimuth = this.GetCameraAzimuthForSlot();
			this._targetCameraValues.Inclination = this.GetCameraInclinationForSlotType(slotTypeId);
			this._targetCameraValues.Distance = this.GetCameraDistanceForSlotType(slotTypeId);
			this._targetCameraValues.Deviation = 0f;
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00004306 File Offset: 0x00002506
		private void FreeCameraFromUpgradeSlot()
		{
			if (this._currentSelectedSlotCameraEntity != null)
			{
				this._currentSelectedSlotCameraEntity = null;
				this._targetCameraValues = this._previousCameraValues;
			}
		}

		// Token: 0x06000055 RID: 85 RVA: 0x0000432C File Offset: 0x0000252C
		private float GetCameraAzimuthForSlot()
		{
			Vec3 vec = this.GetStableSlotPosition() - this._shipSideDirection;
			Vec3 vec2 = this._currentShipVisualInfo.VisualCenterPosition - this._shipForwardDirection * this._staticCameraValues.CameraDeviationLimit;
			Vec3 vec3 = this._currentShipVisualInfo.VisualCenterPosition + this._shipForwardDirection * this._staticCameraValues.CameraDeviationLimit;
			Vec3 closestPointOnLineSegmentToPoint = MBMath.GetClosestPointOnLineSegmentToPoint(ref vec2, ref vec3, ref vec);
			Vec3 vec4 = vec - closestPointOnLineSegmentToPoint;
			if (MBMath.ApproximatelyEqualsTo(MathF.Abs(Vec3.DotProduct(vec4.NormalizedCopy(), Vec3.Up)), 1f, 1E-05f))
			{
				return this._initialCameraValues.Azimuth;
			}
			return MathF.Atan2(vec4.y, vec4.x);
		}

		// Token: 0x06000056 RID: 86 RVA: 0x000043F4 File Offset: 0x000025F4
		private float GetCameraInclinationForSlotType(string slotType)
		{
			return 1.3962634f;
		}

		// Token: 0x06000057 RID: 87 RVA: 0x000043FB File Offset: 0x000025FB
		private float GetCameraDistanceForSlotType(string slotType)
		{
			if (slotType == "hull" || slotType == "sail")
			{
				return this._initialCameraValues.Distance;
			}
			return this._staticCameraValues.MinCameraDistance;
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00004430 File Offset: 0x00002630
		private void TickDataSourceInput()
		{
			if (this.IsHotKeyReleasedInAnyLayer("Confirm"))
			{
				if (!this._dataSource.IsConfirmDisabled)
				{
					UISoundsHelper.PlayUISound("event:/ui/port/confirm_ship");
					this._dataSource.ExecuteConfirm();
					return;
				}
			}
			else if (this.IsHotKeyReleasedInAnyLayer("Exit"))
			{
				if (this._dataSource.IsAnyUpgradeSlotSelected)
				{
					UISoundsHelper.PlayUISound("event:/ui/default");
					this._dataSource.SelectedUpgradeSlot.ExecuteDeselect();
					return;
				}
				UISoundsHelper.PlayUISound("event:/ui/default");
				this._dataSource.ExecuteCancel(true);
				return;
			}
			else
			{
				if (this.IsGameKeyPressedInAnyLayer(45))
				{
					UISoundsHelper.PlayUISound("event:/ui/default");
					this._dataSource.ExecuteCancel(true);
					return;
				}
				if (this.IsHotKeyReleasedInAnyLayer("Reset"))
				{
					UISoundsHelper.PlayUISound("event:/ui/default");
					this._dataSource.ExecuteReset();
					return;
				}
				if (this.IsHotKeyReleasedInAnyLayer("SwitchToPreviousTab"))
				{
					if (!this._isControllingCamera && this._dataSource.ExecuteSelectPreviousShip())
					{
						UISoundsHelper.PlayUISound("event:/ui/port/choose_ship");
						return;
					}
				}
				else if (this.IsHotKeyReleasedInAnyLayer("SwitchToNextTab"))
				{
					if (!this._isControllingCamera && this._dataSource.ExecuteSelectNextShip())
					{
						UISoundsHelper.PlayUISound("event:/ui/port/choose_ship");
						return;
					}
				}
				else if (this.IsHotKeyReleasedInAnyLayer("SelectLeftRoster"))
				{
					if (!this._isControllingCamera && !this._dataSource.LeftRoster.IsSelected && this._dataSource.LeftRoster.HasAnyShips)
					{
						UISoundsHelper.PlayUISound("event:/ui/default");
						this._dataSource.LeftRoster.ExecuteSelectRoster();
						return;
					}
				}
				else if (this.IsHotKeyReleasedInAnyLayer("SelectRightRoster") && !this._isControllingCamera && !this._dataSource.RightRoster.IsSelected && this._dataSource.RightRoster.HasAnyShips)
				{
					UISoundsHelper.PlayUISound("event:/ui/default");
					this._dataSource.RightRoster.ExecuteSelectRoster();
				}
			}
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00004613 File Offset: 0x00002813
		private bool IsHotKeyPressedInAnyLayer(string hotkey)
		{
			return this._gauntletLayer.Input.IsHotKeyPressed(hotkey) || this._sceneLayer.Input.IsHotKeyPressed(hotkey);
		}

		// Token: 0x0600005A RID: 90 RVA: 0x0000463B File Offset: 0x0000283B
		private bool IsHotKeyReleasedInAnyLayer(string hotkey)
		{
			return this._gauntletLayer.Input.IsHotKeyReleased(hotkey) || this._sceneLayer.Input.IsHotKeyReleased(hotkey);
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00004663 File Offset: 0x00002863
		private bool IsGameKeyPressedInAnyLayer(int gameKey)
		{
			return this._gauntletLayer.Input.IsGameKeyPressed(gameKey) || this._sceneLayer.Input.IsGameKeyPressed(gameKey);
		}

		// Token: 0x0600005C RID: 92 RVA: 0x0000468C File Offset: 0x0000288C
		private void TickSceneInput(float dt)
		{
			if (this._sceneLayer.IsHitThisFrame && ScreenManager.FocusedLayer == this._gauntletLayer)
			{
				this._gauntletLayer.IsFocusLayer = false;
				ScreenManager.TryLoseFocus(this._gauntletLayer);
				this._sceneLayer.IsFocusLayer = true;
				ScreenManager.TrySetFocus(this._sceneLayer);
			}
			else if (!this._sceneLayer.IsHitThisFrame && ScreenManager.FocusedLayer == this._sceneLayer)
			{
				this._sceneLayer.IsFocusLayer = false;
				ScreenManager.TryLoseFocus(this._sceneLayer);
				this._gauntletLayer.IsFocusLayer = true;
				ScreenManager.TrySetFocus(this._gauntletLayer);
			}
			bool flag = this._sceneLayer.IsHitThisFrame || this._gauntletLayer.IsHitThisFrame;
			if (Input.IsGamepadActive)
			{
				if (flag && this.IsHotKeyPressedInAnyLayer("ToggleCameraMovement"))
				{
					this._isControllingCamera = !this._isControllingCamera;
				}
			}
			else if (this._sceneLayer.Input.IsHotKeyPressed("ToggleCameraMovement"))
			{
				this._isControllingCamera = true;
			}
			else if (this._sceneLayer.Input.IsHotKeyReleased("ToggleCameraMovement"))
			{
				this._isControllingCamera = false;
			}
			this._dataSource.IsControllingCamera = this._isControllingCamera;
			this._dataSource.CanToggleCamera = flag;
			PortVM dataSource = this._dataSource;
			IViewDataTracker viewDataTracker = this._viewDataTracker;
			dataSource.IsMapBarExtended = viewDataTracker != null && viewDataTracker.GetMapBarExtendedState();
			this._dataSource.CanUseGamepadInputs = Input.IsGamepadActive;
			this._dataSource.CanUseKeyboardInputs = !Input.IsGamepadActive && this._sceneLayer.IsHitThisFrame;
			if (this._isControllingCamera)
			{
				MBWindowManager.DontChangeCursorPos();
				this._gauntletLayer.InputRestrictions.ResetInputRestrictions();
			}
			else
			{
				this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
			}
			if (this._sceneLayer.Input.IsHotKeyPressed("ResetCamera"))
			{
				this.ResetCamera(false);
			}
			Vec2 vec;
			vec..ctor(this._sceneLayer.Input.GetNormalizedMouseMoveX() * 1920f, this._sceneLayer.Input.GetNormalizedMouseMoveY() * 1080f);
			float num = 0f;
			if (Input.IsGamepadActive)
			{
				if (this._isControllingCamera)
				{
					float num2 = this._sceneLayer.Input.GetGameKeyAxis("MovementAxisY") * -1f;
					this.NormalizeControllerInputForDeadZone(ref num2, 0.1f);
					if (this._sceneLayer.Input.IsHotKeyDown("ControllerZoomOut"))
					{
						num2 += 1f;
					}
					if (this._sceneLayer.Input.IsHotKeyDown("ControllerZoomIn"))
					{
						num2 -= 1f;
					}
					num2 = MathF.Clamp(num2, -1f, 1f);
					num = num2 * this._staticCameraValues.ZoomSensitivity * this._staticCameraValues.SensitivityMappingMultiplier * dt;
				}
			}
			else
			{
				float num3 = this._sceneLayer.Input.GetDeltaMouseScroll() * -1f;
				float num4 = this._sceneLayer.Input.GetGameKeyAxis("MovementAxisY") * -1f;
				num = num3 * this._staticCameraValues.ZoomSensitivity + num4 * this._staticCameraValues.ZoomSensitivity * this._staticCameraValues.SensitivityMappingMultiplier * dt;
			}
			this._targetCameraValues.Distance = MathF.Clamp(this._targetCameraValues.Distance + num, this.GetTargetMinDistance(), this._staticCameraValues.MaxCameraDistance);
			float num6;
			if (Input.IsGamepadActive)
			{
				float num5 = (this._isControllingCamera ? (this._sceneLayer.Input.GetGameKeyAxis("CameraAxisX") * -1f) : 0f);
				this.NormalizeControllerInputForDeadZone(ref num5, 0.1f);
				num6 = num5 * this._staticCameraValues.HorizontalRotationSensitivity * this._sceneLayer.Input.GetMouseSensitivity() * this._staticCameraValues.SensitivityMappingMultiplier * dt;
			}
			else
			{
				num6 = (this._isControllingCamera ? (vec.x * -1f) : 0f) * this._staticCameraValues.HorizontalRotationSensitivity * this._sceneLayer.Input.GetMouseSensitivity();
			}
			this._targetCameraValues.Azimuth = MBMath.WrapAngle(this._targetCameraValues.Azimuth + num6 * 0.017453292f);
			float num8;
			if (Input.IsGamepadActive)
			{
				float num7 = (this._isControllingCamera ? this._sceneLayer.Input.GetGameKeyAxis("CameraAxisY") : 0f);
				this.NormalizeControllerInputForDeadZone(ref num7, 0.1f);
				num8 = num7 * this._staticCameraValues.VerticalRotationSensitivity * this._sceneLayer.Input.GetMouseSensitivity() * this._staticCameraValues.SensitivityMappingMultiplier * dt;
			}
			else
			{
				num8 = (this._isControllingCamera ? (vec.y * -1f) : 0f) * this._staticCameraValues.VerticalRotationSensitivity * this._sceneLayer.Input.GetMouseSensitivity();
			}
			if (NativeConfig.InvertMouse)
			{
				num8 *= -1f;
			}
			float num9 = (this._targetCameraValues.Distance - this.GetTargetMinDistance()) / (this._staticCameraValues.MaxCameraDistance - this.GetTargetMinDistance());
			float num10 = MathF.Lerp(this._staticCameraValues.MaxCameraInclinationAtMinDistance, this._staticCameraValues.MaxCameraInclinationAtMaxDistance, num9, 1E-05f);
			this._targetCameraValues.Inclination = MathF.Clamp(this._targetCameraValues.Inclination + num8 * 0.017453292f, this._staticCameraValues.MinCameraInclination, num10);
			float num11 = 0f;
			if (Input.IsGamepadActive)
			{
				if (this._isControllingCamera)
				{
					num11 = this._sceneLayer.Input.GetGameKeyAxis("MovementAxisX");
					this.NormalizeControllerInputForDeadZone(ref num11, 0.1f);
					if (this._sceneLayer.Input.IsHotKeyDown("ControllerDeviateRight"))
					{
						num11 += 1f;
					}
					if (this._sceneLayer.Input.IsHotKeyDown("ControllerDeviateLeft"))
					{
						num11 -= 1f;
					}
					num11 = MathF.Clamp(num11, -1f, 1f);
				}
			}
			else
			{
				num11 = this._sceneLayer.Input.GetGameKeyAxis("MovementAxisX");
			}
			float num12 = MathF.Lerp(this._staticCameraValues.DeviationSensitivityAtMinDistance, this._staticCameraValues.DeviationSensitivityAtMaxDistance, num9, 1E-05f);
			float num13 = MathF.Clamp(MathF.Pow(MathF.Cos(this._currentCameraValues.Azimuth - Vec3.AngleBetweenTwoVectors(Vec3.Forward, this._shipForwardDirection)), 3f) * 2f, -1f, 1f);
			float num14 = num11 * num12 * dt * num13;
			this._targetCameraValues.Deviation = MathF.Clamp(this._targetCameraValues.Deviation + num14, -this._staticCameraValues.CameraDeviationLimit, this._staticCameraValues.CameraDeviationLimit);
			if (num14 != 0f)
			{
				this.FreeCameraFromUpgradeSlot();
			}
			this.UpdateCamera(dt);
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00004D46 File Offset: 0x00002F46
		bool IChangeableScreen.AnyUnsavedChanges()
		{
			return this._isInitialized && this._dataSource.AreThereAnyChanges();
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00004D5D File Offset: 0x00002F5D
		bool IChangeableScreen.CanChangesBeApplied()
		{
			return !this._dataSource.IsConfirmDisabled;
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00004D6D File Offset: 0x00002F6D
		void IChangeableScreen.ApplyChanges()
		{
			this._dataSource.ExecuteConfirm();
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00004D7A File Offset: 0x00002F7A
		void IChangeableScreen.ResetChanges()
		{
			this._dataSource.ExecuteReset();
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00004D88 File Offset: 0x00002F88
		private void UpdateCamera(float dt)
		{
			float num = MathF.Min(1f, 10f * dt);
			float num2 = MathF.Min(1f, 5f * dt);
			float num3 = ((this._currentSelectedSlotCameraEntity != null) ? (6.2831855f * dt) : (100f * dt));
			this._currentCameraValues.Azimuth = this.LerpAngleWithMax(this._currentCameraValues.Azimuth, this._targetCameraValues.Azimuth, num, num3);
			this._currentCameraValues.Inclination = this.LerpAngleWithMax(this._currentCameraValues.Inclination, this._targetCameraValues.Inclination, num, num3);
			this._currentCameraValues.Deviation = MathF.Lerp(this._currentCameraValues.Deviation, this._targetCameraValues.Deviation, num, 1E-05f);
			this._currentCameraValues.Distance = MathF.Lerp(this._currentCameraValues.Distance, this._targetCameraValues.Distance, num, 1E-05f);
			float num4 = (this._currentCameraValues.Distance - this.GetTargetMinDistance()) / (this._staticCameraValues.MaxCameraDistance - this.GetTargetMinDistance());
			num4 = MathF.Clamp(num4, 0f, 1f);
			this._currentCameraTargetPosition = this.LerpVec3WithMax(this._currentCameraTargetPosition, this.GetCameraTargetPosition(), num2, 500f * dt);
			Vec3 vec = this._currentCameraTargetPosition;
			vec += this._currentCameraValues.Deviation * this._shipForwardDirection;
			float num5 = AnimationInterpolation.Ease(2, 0, num4);
			vec.z += MathF.Lerp(this._staticCameraValues.ExtraHeightAtMinDistance, this._staticCameraValues.ExtraHeightAtMaxDistance, num5, 1E-05f);
			this.HandleCameraCollision(vec);
			MatrixFrame identity = MatrixFrame.Identity;
			identity.origin = vec;
			identity.origin.x = identity.origin.x + this._currentCameraValues.Distance * MathF.Sin(this._currentCameraValues.Inclination) * MathF.Cos(this._currentCameraValues.Azimuth);
			identity.origin.y = identity.origin.y + this._currentCameraValues.Distance * MathF.Sin(this._currentCameraValues.Inclination) * MathF.Sin(this._currentCameraValues.Azimuth);
			identity.origin.z = identity.origin.z + this._currentCameraValues.Distance * MathF.Cos(this._currentCameraValues.Inclination);
			this._sceneCamera.LookAt(identity.origin, vec, Vec3.Up);
			this._sceneCamera.SetFovHorizontal(1.5707964f, Screen.AspectRatio, 0.1f, 2000f);
			this._scene.SetDepthOfFieldFocus(this._currentCameraValues.Distance);
			float num6 = AnimationInterpolation.Ease(1, 2, num4);
			float num7 = MathF.Lerp(this._staticCameraValues.FocusDistanceAtMinDistance, this._staticCameraValues.FocusDistanceAtMaxDistance, num6, 1E-05f);
			this._scene.SetDepthOfFieldParameters(num7, num7, true);
			this._sceneLayer.SetCamera(this._sceneCamera);
			SoundManager.SetListenerFrame(this._sceneCamera.Frame);
			this.HandleIsCameraUnderwater();
			this.HandleShipEntityVisibilities();
		}

		// Token: 0x06000062 RID: 98 RVA: 0x000050B0 File Offset: 0x000032B0
		private float LerpAngleWithMax(float current, float target, float amount, float maxAmount)
		{
			float num = MathF.AngleLerp(current, target, amount, 1E-05f);
			float num2 = (num - current) % 6.2831855f;
			float num3 = 2f * num2 % 6.2831855f - num2;
			if (MathF.Abs(num3) > maxAmount)
			{
				num = MathF.AngleClamp(current + (float)MathF.Sign(num3) * maxAmount);
			}
			return num;
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00005104 File Offset: 0x00003304
		private Vec3 LerpVec3WithMax(Vec3 current, Vec3 target, float amount, float maxAmount)
		{
			Vec3 vec = Vec3.Lerp(current, target, amount);
			if (vec.Distance(current) > maxAmount)
			{
				vec = current + (vec - current).NormalizedCopy() * maxAmount;
			}
			return vec;
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00005144 File Offset: 0x00003344
		private Vec3 GetCameraTargetPosition()
		{
			if (!(this._currentShipVisualInfo.VisualEntity != null))
			{
				return this._shipSpawnPositionEntity.GetFrame().origin + new Vec3(0f, 0f, 2.5f, -1f);
			}
			if (this._currentSelectedSlotCameraEntity != null)
			{
				return this.GetStableSlotPosition();
			}
			return this._currentShipVisualInfo.VisualCenterPosition;
		}

		// Token: 0x06000065 RID: 101 RVA: 0x000051B3 File Offset: 0x000033B3
		private Vec3 GetStableSlotPosition()
		{
			return this._currentSelectedSlotCameraEntity.GlobalPosition - this._currentShipVisualInfo.VisualEntity.GlobalPosition + this._currentShipVisualInfo.InitialPosition;
		}

		// Token: 0x06000066 RID: 102 RVA: 0x000051E5 File Offset: 0x000033E5
		private void NormalizeControllerInputForDeadZone(ref float inputValue, float controllerDeadZone)
		{
			if (MathF.Abs(inputValue) < controllerDeadZone)
			{
				inputValue = 0f;
				return;
			}
			inputValue = (inputValue - (float)MathF.Sign(inputValue) * controllerDeadZone) / (1f - controllerDeadZone);
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00005210 File Offset: 0x00003410
		private void HandleCameraCollision(Vec3 cameraTargetPos)
		{
			float num;
			if (this._scene.RayCastForClosestEntityOrTerrain(this._sceneCamera.Position, cameraTargetPos, ref num, 0.01f, 79617))
			{
				float num2 = this._currentCameraValues.Distance - num + 1f;
				if (this._currentCameraValues.Distance < num2)
				{
					this._currentCameraValues.Distance = num2;
					this._targetCameraValues.Distance = num2;
				}
			}
		}

		// Token: 0x06000068 RID: 104 RVA: 0x0000527C File Offset: 0x0000347C
		private void HandleIsCameraUnderwater()
		{
			Vec3 position = this._sceneCamera.Position;
			float waterLevelAtPosition = this._scene.GetWaterLevelAtPosition(position.AsVec2, true, false);
			if (position.Z < waterLevelAtPosition)
			{
				if (this._underwaterSoundEvent == null)
				{
					this._underwaterSoundEvent = SoundManager.CreateEvent("snapshot:/Underwater", this._scene);
					this._underwaterSoundEvent.Play();
					SoundManager.SetGlobalParameter("isUnderwater", 1f);
					return;
				}
			}
			else if (this._underwaterSoundEvent != null)
			{
				this._underwaterSoundEvent.Release();
				this._underwaterSoundEvent = null;
				SoundManager.SetGlobalParameter("isUnderwater", 0f);
			}
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00005317 File Offset: 0x00003517
		private void ResetCamera(bool isInstant)
		{
			if (isInstant)
			{
				this._currentCameraTargetPosition = this.GetCameraTargetPosition();
				this._currentCameraValues = this._initialCameraValues;
			}
			this._targetCameraValues = this._initialCameraValues;
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00005340 File Offset: 0x00003540
		private void HandleShipEntityVisibilities()
		{
			foreach (KeyValuePair<Ship, GauntletPortScreen.PortShipVisualInfo> keyValuePair in this._shipVisualInfos)
			{
				GameEntity visualEntity = keyValuePair.Value.VisualEntity;
				bool isHidden = keyValuePair.Value.IsHidden;
				if (visualEntity == this._currentShipVisualInfo.VisualEntity)
				{
					visualEntity.SetVisibilityExcludeParents(!isHidden);
				}
				else
				{
					float num = 6f;
					ValueTuple<Vec3, Vec3> valueTuple = visualEntity.ComputeGlobalPhysicsBoundingBoxMinMax();
					Vec3 item = valueTuple.Item1;
					Vec3 item2 = valueTuple.Item2;
					float num2 = MathF.Min(item.X, item2.X) - num;
					float num3 = MathF.Max(item.X, item2.X) + num;
					float num4 = MathF.Min(item.Y, item2.Y) - num;
					float num5 = MathF.Max(item.Y, item2.Y) + num;
					float num6 = MathF.Min(item.Z, item2.Z) - num;
					float num7 = MathF.Max(item.Z, item2.Z) + num;
					bool flag = this._sceneCamera.Position.X > num2 && this._sceneCamera.Position.X < num3 && this._sceneCamera.Position.Y > num4 && this._sceneCamera.Position.Y < num5 && this._sceneCamera.Position.Z > num6 && this._sceneCamera.Position.Z < num7;
					visualEntity.SetVisibilityExcludeParents(!isHidden && !flag);
				}
			}
		}

		// Token: 0x0600006B RID: 107 RVA: 0x0000552C File Offset: 0x0000372C
		private float GetTargetMinDistance()
		{
			if (!(this._currentSelectedSlotCameraEntity != null))
			{
				return this._staticCameraValues.MinCameraDistance;
			}
			return this._staticCameraValues.MinCameraDistanceWhileInspectingPiece;
		}

		// Token: 0x0400000E RID: 14
		private SceneLayer _sceneLayer;

		// Token: 0x0400000F RID: 15
		private Scene _scene;

		// Token: 0x04000010 RID: 16
		private readonly PortState _portState;

		// Token: 0x04000011 RID: 17
		private GauntletLayer _gauntletLayer;

		// Token: 0x04000012 RID: 18
		private PortVM _dataSource;

		// Token: 0x04000013 RID: 19
		private GameEntity _shipSpawnPositionEntity;

		// Token: 0x04000014 RID: 20
		private readonly Dictionary<Ship, GauntletPortScreen.PortShipVisualInfo> _shipVisualInfos;

		// Token: 0x04000015 RID: 21
		private GauntletPortScreen.PortShipVisualInfo _currentShipVisualInfo;

		// Token: 0x04000016 RID: 22
		private SpriteCategory _portCategory;

		// Token: 0x04000017 RID: 23
		private SpriteCategory _shipPiecesCategory;

		// Token: 0x04000018 RID: 24
		private SpriteCategory _clanCategory;

		// Token: 0x04000019 RID: 25
		private SpriteCategory _characterdeveloperCategory;

		// Token: 0x0400001A RID: 26
		private Camera _sceneCamera;

		// Token: 0x0400001B RID: 27
		private SoundEvent _underwaterSoundEvent;

		// Token: 0x0400001C RID: 28
		private IViewDataTracker _viewDataTracker;

		// Token: 0x0400001D RID: 29
		private readonly bool _isInSettlementPort;

		// Token: 0x0400001E RID: 30
		private bool _isInitialized;

		// Token: 0x0400001F RID: 31
		private bool _isControllingCamera;

		// Token: 0x04000020 RID: 32
		private int _framesToWaitAfterInit;

		// Token: 0x04000021 RID: 33
		private GauntletPortScreen.CameraParameters _targetCameraValues;

		// Token: 0x04000022 RID: 34
		private GauntletPortScreen.CameraParameters _currentCameraValues;

		// Token: 0x04000023 RID: 35
		private GauntletPortScreen.CameraParameters _previousCameraValues;

		// Token: 0x04000024 RID: 36
		private readonly GauntletPortScreen.CameraParameters _initialCameraValues;

		// Token: 0x04000025 RID: 37
		private readonly GauntletPortScreen.StaticCameraParameters _staticCameraValues;

		// Token: 0x04000026 RID: 38
		private Vec3 _currentCameraTargetPosition;

		// Token: 0x04000027 RID: 39
		private GameEntity _currentSelectedSlotCameraEntity;

		// Token: 0x04000028 RID: 40
		private Vec3 _shipForwardDirection = Vec3.Forward;

		// Token: 0x04000029 RID: 41
		private Vec3 _shipSideDirection = Vec3.Side;

		// Token: 0x02000027 RID: 39
		private struct CameraParameters
		{
			// Token: 0x0600010E RID: 270 RVA: 0x0000A29E File Offset: 0x0000849E
			public CameraParameters(float azimuth, float inclination, float distance, float deviation)
			{
				this.Azimuth = azimuth;
				this.Inclination = inclination;
				this.Distance = distance;
				this.Deviation = deviation;
			}

			// Token: 0x04000094 RID: 148
			public float Azimuth;

			// Token: 0x04000095 RID: 149
			public float Inclination;

			// Token: 0x04000096 RID: 150
			public float Distance;

			// Token: 0x04000097 RID: 151
			public float Deviation;
		}

		// Token: 0x02000028 RID: 40
		private struct StaticCameraParameters
		{
			// Token: 0x0600010F RID: 271 RVA: 0x0000A2C0 File Offset: 0x000084C0
			public StaticCameraParameters(float horizontalRotationSensitivity, float verticalRotationSensitivity, float zoomSensitivity, float sensitivityMappingMultiplier, float deviationSensitivityAtMinDistance, float deviationSensitivityAtMaxDistance, float minCameraInclination, float maxCameraInclinationAtMinDistance, float maxCameraInclinationAtMaxDistance, float minCameraDistance, float maxCameraDistance, float minCameraDistanceWhileInspectingPiece, float cameraDeviationLimit, float focusDistanceAtMinDistance, float focusDistanceAtMaxDistance, float extraHeightAtMinDistance, float extraHeightAtMaxDistance)
			{
				this.HorizontalRotationSensitivity = horizontalRotationSensitivity;
				this.VerticalRotationSensitivity = verticalRotationSensitivity;
				this.ZoomSensitivity = zoomSensitivity;
				this.SensitivityMappingMultiplier = sensitivityMappingMultiplier;
				this.DeviationSensitivityAtMinDistance = deviationSensitivityAtMinDistance;
				this.DeviationSensitivityAtMaxDistance = deviationSensitivityAtMaxDistance;
				this.MinCameraInclination = minCameraInclination;
				this.MaxCameraInclinationAtMinDistance = maxCameraInclinationAtMinDistance;
				this.MaxCameraInclinationAtMaxDistance = maxCameraInclinationAtMaxDistance;
				this.MinCameraDistance = minCameraDistance;
				this.MaxCameraDistance = maxCameraDistance;
				this.MinCameraDistanceWhileInspectingPiece = minCameraDistanceWhileInspectingPiece;
				this.CameraDeviationLimit = cameraDeviationLimit;
				this.FocusDistanceAtMinDistance = focusDistanceAtMinDistance;
				this.FocusDistanceAtMaxDistance = focusDistanceAtMaxDistance;
				this.ExtraHeightAtMinDistance = extraHeightAtMinDistance;
				this.ExtraHeightAtMaxDistance = extraHeightAtMaxDistance;
			}

			// Token: 0x04000098 RID: 152
			public float HorizontalRotationSensitivity;

			// Token: 0x04000099 RID: 153
			public float VerticalRotationSensitivity;

			// Token: 0x0400009A RID: 154
			public float ZoomSensitivity;

			// Token: 0x0400009B RID: 155
			public float SensitivityMappingMultiplier;

			// Token: 0x0400009C RID: 156
			public float DeviationSensitivityAtMinDistance;

			// Token: 0x0400009D RID: 157
			public float DeviationSensitivityAtMaxDistance;

			// Token: 0x0400009E RID: 158
			public float MinCameraInclination;

			// Token: 0x0400009F RID: 159
			public float MaxCameraInclinationAtMinDistance;

			// Token: 0x040000A0 RID: 160
			public float MaxCameraInclinationAtMaxDistance;

			// Token: 0x040000A1 RID: 161
			public float MinCameraDistance;

			// Token: 0x040000A2 RID: 162
			public float MaxCameraDistance;

			// Token: 0x040000A3 RID: 163
			public float MinCameraDistanceWhileInspectingPiece;

			// Token: 0x040000A4 RID: 164
			public float CameraDeviationLimit;

			// Token: 0x040000A5 RID: 165
			public float FocusDistanceAtMinDistance;

			// Token: 0x040000A6 RID: 166
			public float FocusDistanceAtMaxDistance;

			// Token: 0x040000A7 RID: 167
			public float ExtraHeightAtMinDistance;

			// Token: 0x040000A8 RID: 168
			public float ExtraHeightAtMaxDistance;
		}

		// Token: 0x02000029 RID: 41
		private struct PortShipVisualInfo
		{
			// Token: 0x06000110 RID: 272 RVA: 0x0000A352 File Offset: 0x00008552
			public PortShipVisualInfo(GameEntity visualEntity, Vec3 initialPosition, Vec3 visualCenterPosition, bool isHidden = false)
			{
				this.VisualEntity = visualEntity;
				this.InitialPosition = initialPosition;
				this.VisualCenterPosition = visualCenterPosition;
				this.IsHidden = isHidden;
			}

			// Token: 0x040000A9 RID: 169
			public GameEntity VisualEntity;

			// Token: 0x040000AA RID: 170
			public Vec3 InitialPosition;

			// Token: 0x040000AB RID: 171
			public Vec3 VisualCenterPosition;

			// Token: 0x040000AC RID: 172
			public bool IsHidden;
		}
	}
}
