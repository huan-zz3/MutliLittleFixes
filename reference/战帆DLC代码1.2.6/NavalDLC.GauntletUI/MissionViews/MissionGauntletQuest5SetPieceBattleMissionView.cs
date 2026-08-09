using System;
using NavalDLC.Storyline.MissionControllers;
using NavalDLC.View.MissionViews;
using NavalDLC.View.MissionViews.Storyline;
using SandBox.View.Missions;
using SandBox.View.Missions.NameMarkers;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.GauntletUI;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.ScreenSystem;

namespace NavalDLC.GauntletUI.MissionViews
{
	// Token: 0x0200001D RID: 29
	[OverrideView(typeof(Quest5SetPieceBattleMissionView))]
	public class MissionGauntletQuest5SetPieceBattleMissionView : Quest5SetPieceBattleMissionView
	{
		// Token: 0x060000BD RID: 189 RVA: 0x0000795B File Offset: 0x00005B5B
		public MissionGauntletQuest5SetPieceBattleMissionView()
		{
			this._gauntletLayer = new MissionGauntletQuest5SetPieceBattleMissionView.Quest5CutsceneGauntletLayer(10, false);
		}

		// Token: 0x060000BE RID: 190 RVA: 0x00007971 File Offset: 0x00005B71
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			base.MissionScreen.AddLayer(this._gauntletLayer);
		}

		// Token: 0x060000BF RID: 191 RVA: 0x0000798A File Offset: 0x00005B8A
		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			base.MissionScreen.RemoveLayer(this._gauntletLayer);
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x000079A3 File Offset: 0x00005BA3
		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			this.HandleOrderUISuspendStateChange();
			this.HandleShipMarkersSuspendStateChange();
			this.HandleStealthBarSuspendStateChange();
			this.HandleNameMarkersSuspendStateChange();
			this.HandleAgentBannerSuspendStateChange();
			this.HandleShipHighlightSuspendStateChange();
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x000079D0 File Offset: 0x00005BD0
		public override void OnConversationEnd()
		{
			base.OnConversationEnd();
			this._disableOrderUI = false;
			this._disableShipMarkers = false;
			this._disableStealthBar = false;
			this._disableNameMarkers = false;
			this._disableAgentBanners = false;
			this._disableShipHighlights = false;
			this.HandleOrderUISuspendStateChange();
			this.HandleShipMarkersSuspendStateChange();
			this.HandleStealthBarSuspendStateChange();
			this.HandleNameMarkersSuspendStateChange();
			this.HandleAgentBannerSuspendStateChange();
			this.HandleShipHighlightSuspendStateChange();
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x00007A34 File Offset: 0x00005C34
		public override void PassMissionStateOnTick(Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState currentState)
		{
			base.PassMissionStateOnTick(currentState);
			if (!this._isShipCameraUpdatedAtTheStartOfApproachPhase && currentState == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1GoToEnemyShip)
			{
				this.SetActiveCameraModeForShip(MissionShipControlView.CameraModes.Back);
				this._isShipCameraUpdatedAtTheStartOfApproachPhase = true;
			}
			if (!this._isShipCameraUpdatedAtTheStartOfPhase3 && currentState == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase3InProgress)
			{
				this.SetActiveCameraModeForShip(MissionShipControlView.CameraModes.Back);
				this._isShipCameraUpdatedAtTheStartOfPhase3 = true;
			}
			switch (currentState)
			{
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.None:
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase2Part4:
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.End:
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Exit:
				break;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase1Part1:
				this._disableOrderUI = true;
				this._disableShipMarkers = true;
				this._disableStealthBar = false;
				this._disableNameMarkers = false;
				this._disableAgentBanners = true;
				this._disableShipHighlights = true;
				return;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase1Part2:
				this._disableOrderUI = true;
				this._disableShipMarkers = true;
				this._disableStealthBar = false;
				this._disableNameMarkers = false;
				this._disableAgentBanners = true;
				this._disableShipHighlights = true;
				return;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1GoToEnemyShip:
				this._disableOrderUI = true;
				this._disableShipMarkers = true;
				this._disableStealthBar = false;
				this._disableNameMarkers = false;
				this._disableAgentBanners = true;
				this._disableShipHighlights = true;
				return;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1SwimmingPhase:
				this._disableOrderUI = true;
				this._disableShipMarkers = true;
				this._disableStealthBar = false;
				this._disableNameMarkers = false;
				this._disableAgentBanners = true;
				this._disableShipHighlights = true;
				return;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializeStealthPhasePart1:
				this._disableOrderUI = true;
				this._disableShipMarkers = true;
				this._disableStealthBar = false;
				this._disableNameMarkers = false;
				this._disableAgentBanners = true;
				this._disableShipHighlights = true;
				return;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializeStealthPhasePart2:
				this._disableOrderUI = true;
				this._disableShipMarkers = true;
				this._disableStealthBar = false;
				this._disableNameMarkers = false;
				this._disableAgentBanners = true;
				this._disableShipHighlights = true;
				return;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1StealthPhase:
				this._disableOrderUI = true;
				this._disableShipMarkers = true;
				this._disableStealthBar = false;
				this._disableNameMarkers = false;
				this._disableAgentBanners = true;
				this._disableShipHighlights = true;
				return;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1GoToShipInteriorFadeOut:
				this._disableOrderUI = true;
				this._disableShipMarkers = true;
				this._disableStealthBar = true;
				this._disableNameMarkers = true;
				this._disableAgentBanners = true;
				this._disableShipHighlights = true;
				return;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1InitializeShipInteriorPhase:
				this._disableOrderUI = true;
				this._disableShipMarkers = true;
				this._disableStealthBar = true;
				this._disableNameMarkers = true;
				this._disableAgentBanners = true;
				this._disableShipHighlights = true;
				return;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1GoToShipInteriorFadeIn:
				this._disableOrderUI = true;
				this._disableShipMarkers = true;
				this._disableStealthBar = true;
				this._disableNameMarkers = true;
				this._disableAgentBanners = true;
				this._disableShipHighlights = true;
				return;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1ShipInteriorPhase:
				this._disableOrderUI = true;
				this._disableShipMarkers = true;
				this._disableStealthBar = true;
				this._disableNameMarkers = true;
				this._disableAgentBanners = true;
				this._disableShipHighlights = true;
				return;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1GoBackToShipFadeOut:
				this._disableOrderUI = true;
				this._disableShipMarkers = true;
				this._disableStealthBar = true;
				this._disableNameMarkers = true;
				this._disableAgentBanners = true;
				this._disableShipHighlights = true;
				return;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1InitializeGoBackToShip:
				this._disableOrderUI = true;
				this._disableShipMarkers = true;
				this._disableStealthBar = true;
				this._disableNameMarkers = true;
				this._disableAgentBanners = true;
				this._disableShipHighlights = true;
				return;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1GoBackToShipFadeIn:
				this._disableOrderUI = true;
				this._disableShipMarkers = true;
				this._disableStealthBar = true;
				this._disableNameMarkers = true;
				this._disableAgentBanners = true;
				this._disableShipHighlights = true;
				return;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1EscapePhase:
				this._disableOrderUI = true;
				this._disableShipMarkers = true;
				this._disableStealthBar = false;
				this._disableNameMarkers = false;
				this._disableAgentBanners = true;
				this._disableShipHighlights = true;
				return;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1ToPhase2FadeOut:
				this._disableOrderUI = true;
				this._disableShipMarkers = true;
				this._disableStealthBar = false;
				this._disableNameMarkers = false;
				this._disableAgentBanners = true;
				this._disableShipHighlights = true;
				return;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase2Part1:
				this._disableOrderUI = true;
				this._disableShipMarkers = true;
				this._disableStealthBar = true;
				this._disableNameMarkers = false;
				this._disableAgentBanners = false;
				this._disableShipHighlights = true;
				return;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase2Part2:
				this._disableOrderUI = true;
				this._disableShipMarkers = true;
				this._disableStealthBar = true;
				this._disableNameMarkers = false;
				this._disableAgentBanners = false;
				this._disableShipHighlights = true;
				return;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase2Part3:
				this._disableOrderUI = true;
				this._disableShipMarkers = true;
				this._disableStealthBar = true;
				this._disableNameMarkers = false;
				this._disableAgentBanners = false;
				this._disableShipHighlights = true;
				return;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1ToPhase2FadeIn:
				this._disableOrderUI = true;
				this._disableShipMarkers = true;
				this._disableStealthBar = true;
				this._disableNameMarkers = false;
				this._disableAgentBanners = false;
				this._disableShipHighlights = true;
				return;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase2InProgress:
				this._disableOrderUI = true;
				this._disableShipMarkers = false;
				this._disableStealthBar = true;
				this._disableNameMarkers = false;
				this._disableAgentBanners = false;
				this._disableShipHighlights = false;
				return;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase2ToPhase3FadeOut:
				this._disableOrderUI = true;
				this._disableShipMarkers = false;
				this._disableStealthBar = true;
				this._disableNameMarkers = false;
				this._disableAgentBanners = false;
				this._disableShipHighlights = false;
				return;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase3Part1:
				this._disableOrderUI = false;
				this._disableShipMarkers = false;
				this._disableStealthBar = true;
				this._disableNameMarkers = false;
				this._disableAgentBanners = false;
				this._disableShipHighlights = false;
				return;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase3Part2:
				this._disableOrderUI = false;
				this._disableShipMarkers = false;
				this._disableStealthBar = true;
				this._disableNameMarkers = false;
				this._disableAgentBanners = false;
				this._disableShipHighlights = false;
				return;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase3Part3:
				this._disableOrderUI = false;
				this._disableShipMarkers = false;
				this._disableStealthBar = true;
				this._disableNameMarkers = false;
				this._disableAgentBanners = false;
				this._disableShipHighlights = false;
				return;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase2ToPhase3FadeIn:
				this._disableOrderUI = false;
				this._disableShipMarkers = false;
				this._disableStealthBar = true;
				this._disableNameMarkers = false;
				this._disableAgentBanners = false;
				this._disableShipHighlights = false;
				return;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase3InProgress:
				this._disableOrderUI = false;
				this._disableShipMarkers = false;
				this._disableStealthBar = true;
				this._disableNameMarkers = false;
				this._disableAgentBanners = false;
				this._disableShipHighlights = false;
				return;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase3ToPhase4FadeOut:
				this._disableOrderUI = false;
				this._disableShipMarkers = false;
				this._disableStealthBar = true;
				this._disableNameMarkers = false;
				this._disableAgentBanners = false;
				this._disableShipHighlights = false;
				return;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase4Part1:
				this._disableOrderUI = false;
				this._disableShipMarkers = false;
				this._disableStealthBar = true;
				this._disableNameMarkers = false;
				this._disableAgentBanners = false;
				this._disableShipHighlights = false;
				return;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase4Part2:
				this._disableOrderUI = false;
				this._disableShipMarkers = false;
				this._disableStealthBar = true;
				this._disableNameMarkers = false;
				this._disableAgentBanners = false;
				this._disableShipHighlights = false;
				return;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase3ToPhase4FadeIn:
				this._disableOrderUI = false;
				this._disableShipMarkers = false;
				this._disableStealthBar = true;
				this._disableNameMarkers = false;
				this._disableAgentBanners = false;
				this._disableShipHighlights = false;
				return;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase4InProgress:
				this._disableOrderUI = false;
				this._disableShipMarkers = false;
				this._disableStealthBar = true;
				this._disableNameMarkers = false;
				this._disableAgentBanners = false;
				this._disableShipHighlights = false;
				return;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase4ToBossFightFadeOut:
				this._disableOrderUI = false;
				this._disableShipMarkers = true;
				this._disableStealthBar = true;
				this._disableNameMarkers = false;
				this._disableAgentBanners = false;
				this._disableShipHighlights = false;
				return;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializeBossFightPart1:
				this._disableOrderUI = true;
				this._disableShipMarkers = true;
				this._disableStealthBar = true;
				this._disableNameMarkers = false;
				this._disableAgentBanners = false;
				this._disableShipHighlights = false;
				return;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializeBossFightPart2:
				this._disableOrderUI = true;
				this._disableShipMarkers = true;
				this._disableStealthBar = true;
				this._disableNameMarkers = false;
				this._disableAgentBanners = false;
				this._disableShipHighlights = false;
				return;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase4ToBossFightFadeIn:
				this._disableOrderUI = true;
				this._disableShipMarkers = true;
				this._disableStealthBar = true;
				this._disableNameMarkers = false;
				this._disableAgentBanners = false;
				this._disableShipHighlights = false;
				return;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.StartBossFightConversation:
				this._disableOrderUI = true;
				this._disableShipMarkers = true;
				this._disableStealthBar = true;
				this._disableNameMarkers = false;
				this._disableAgentBanners = false;
				this._disableShipHighlights = false;
				return;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.BossFightConversationInProgress:
				this._disableOrderUI = true;
				this._disableShipMarkers = true;
				this._disableStealthBar = true;
				this._disableNameMarkers = false;
				this._disableAgentBanners = false;
				this._disableShipHighlights = false;
				return;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.BossFightInProgressAsDuel:
				this._disableOrderUI = true;
				this._disableShipMarkers = true;
				this._disableStealthBar = true;
				this._disableNameMarkers = false;
				this._disableAgentBanners = false;
				this._disableShipHighlights = false;
				return;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.BossFightInProgressAsAll:
				this._disableOrderUI = true;
				this._disableShipMarkers = true;
				this._disableStealthBar = true;
				this._disableNameMarkers = false;
				this._disableAgentBanners = false;
				this._disableShipHighlights = false;
				break;
			default:
				return;
			}
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x000081EC File Offset: 0x000063EC
		private void HandleOrderUISuspendStateChange()
		{
			if (this._disableOrderUI)
			{
				if (!this._isOrderUIDisabled)
				{
					GauntletOrderUIHandler missionBehavior = base.Mission.GetMissionBehavior<GauntletOrderUIHandler>();
					if (missionBehavior != null && missionBehavior.IsViewCreated)
					{
						this.SetMissionViewVisibility<GauntletOrderUIHandler>(false);
						this._isOrderUIDisabled = true;
						return;
					}
				}
			}
			else if (this._isOrderUIDisabled)
			{
				GauntletOrderUIHandler missionBehavior2 = base.Mission.GetMissionBehavior<GauntletOrderUIHandler>();
				if (missionBehavior2 != null && missionBehavior2.IsViewCreated)
				{
					this.SetMissionViewVisibility<GauntletOrderUIHandler>(true);
					this._isOrderUIDisabled = false;
				}
			}
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x0000825C File Offset: 0x0000645C
		private void HandleShipMarkersSuspendStateChange()
		{
			if (this._disableShipMarkers)
			{
				if (!this._isShipMarkersDisabled)
				{
					MissionGauntletNavalShipMarker missionBehavior = base.Mission.GetMissionBehavior<MissionGauntletNavalShipMarker>();
					if (missionBehavior != null && missionBehavior.IsViewCreated)
					{
						this.SetMissionViewVisibility<MissionGauntletNavalShipMarker>(false);
						this._isShipMarkersDisabled = true;
						return;
					}
				}
			}
			else if (this._isShipMarkersDisabled)
			{
				MissionGauntletNavalShipMarker missionBehavior2 = base.Mission.GetMissionBehavior<MissionGauntletNavalShipMarker>();
				if (missionBehavior2 != null && missionBehavior2.IsViewCreated)
				{
					this.SetMissionViewVisibility<MissionGauntletNavalShipMarker>(true);
					this._isShipMarkersDisabled = false;
				}
			}
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x000082CC File Offset: 0x000064CC
		private void HandleStealthBarSuspendStateChange()
		{
			if (this._disableStealthBar)
			{
				if (!this._isStealthBarDisabled)
				{
					MissionAgentAlarmStateView missionBehavior = base.Mission.GetMissionBehavior<MissionAgentAlarmStateView>();
					if (missionBehavior != null && missionBehavior.IsReady())
					{
						this.SetMissionViewVisibility<MissionAgentAlarmStateView>(false);
						this._isStealthBarDisabled = true;
						return;
					}
				}
			}
			else if (this._isStealthBarDisabled)
			{
				MissionAgentAlarmStateView missionBehavior2 = base.Mission.GetMissionBehavior<MissionAgentAlarmStateView>();
				if (missionBehavior2 != null && missionBehavior2.IsReady())
				{
					this.SetMissionViewVisibility<MissionAgentAlarmStateView>(true);
					this._isStealthBarDisabled = false;
				}
			}
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x0000833C File Offset: 0x0000653C
		private void HandleNameMarkersSuspendStateChange()
		{
			if (this._disableNameMarkers)
			{
				if (!this._isNameMarkersDisabled)
				{
					MissionNameMarkerUIHandler missionBehavior = base.Mission.GetMissionBehavior<MissionNameMarkerUIHandler>();
					if (missionBehavior != null && missionBehavior.IsReady())
					{
						this.SetMissionViewVisibility<MissionNameMarkerUIHandler>(false);
						this._isNameMarkersDisabled = true;
						return;
					}
				}
			}
			else if (this._isNameMarkersDisabled)
			{
				MissionNameMarkerUIHandler missionBehavior2 = base.Mission.GetMissionBehavior<MissionNameMarkerUIHandler>();
				if (missionBehavior2 != null && missionBehavior2.IsReady())
				{
					this.SetMissionViewVisibility<MissionNameMarkerUIHandler>(true);
					this._isNameMarkersDisabled = false;
				}
			}
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x000083AC File Offset: 0x000065AC
		private void HandleAgentBannerSuspendStateChange()
		{
			if (this._disableAgentBanners)
			{
				if (!this._isAgentBannersDisabled)
				{
					MissionAgentLabelView missionBehavior = base.Mission.GetMissionBehavior<MissionAgentLabelView>();
					if (missionBehavior != null && missionBehavior.IsReady())
					{
						this.SetMissionViewVisibility<MissionAgentLabelView>(false);
						this._isAgentBannersDisabled = true;
						return;
					}
				}
			}
			else if (this._isAgentBannersDisabled)
			{
				MissionAgentLabelView missionBehavior2 = base.Mission.GetMissionBehavior<MissionAgentLabelView>();
				if (missionBehavior2 != null && missionBehavior2.IsReady())
				{
					this.SetMissionViewVisibility<MissionAgentLabelView>(true);
					this._isAgentBannersDisabled = false;
				}
			}
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x0000841C File Offset: 0x0000661C
		private void HandleShipHighlightSuspendStateChange()
		{
			if (this._disableShipHighlights)
			{
				if (!this._isShipHighlightsDisabled)
				{
					MissionGauntletShipControlView missionBehavior = base.Mission.GetMissionBehavior<MissionGauntletShipControlView>();
					if (missionBehavior != null && missionBehavior.IsReady())
					{
						missionBehavior.SuspendFeature(MissionGauntletShipControlView.ShipControlFeatureFlags.ShipFocus);
						this._isShipHighlightsDisabled = true;
						return;
					}
				}
			}
			else if (this._isShipHighlightsDisabled)
			{
				MissionGauntletShipControlView missionBehavior2 = base.Mission.GetMissionBehavior<MissionGauntletShipControlView>();
				if (missionBehavior2 != null && missionBehavior2.IsReady())
				{
					missionBehavior2.ResumeFeature(MissionGauntletShipControlView.ShipControlFeatureFlags.ShipFocus);
					this._isShipHighlightsDisabled = false;
				}
			}
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x0000848C File Offset: 0x0000668C
		private void SetMissionViewVisibility<T>(bool isVisible) where T : MissionView
		{
			T missionBehavior = base.Mission.GetMissionBehavior<T>();
			if (missionBehavior == null)
			{
				Debug.FailedAssert("Trying to set visibility of mission view: " + typeof(T).Name + " but it does not exist in the mission!", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.GauntletUI\\MissionViews\\MissionGauntletQuest5SetPieceBattleMissionView.cs", "SetMissionViewVisibility", 695);
				return;
			}
			if (isVisible)
			{
				missionBehavior.ResumeView();
				return;
			}
			missionBehavior.SuspendView();
		}

		// Token: 0x060000CA RID: 202 RVA: 0x000084FC File Offset: 0x000066FC
		private void SetActiveCameraModeForShip(MissionShipControlView.CameraModes mode)
		{
			MissionGauntletShipControlView missionBehavior = base.Mission.GetMissionBehavior<MissionGauntletShipControlView>();
			if (missionBehavior != null && missionBehavior.IsReady())
			{
				missionBehavior.SetActiveCameraMode(mode);
			}
		}

		// Token: 0x060000CB RID: 203 RVA: 0x00008528 File Offset: 0x00006728
		protected override void SetPlayerMovementEnabled(bool isPlayerMovementEnabled)
		{
			base.SetPlayerMovementEnabled(isPlayerMovementEnabled);
			for (int i = 0; i < base.Mission.MissionBehaviors.Count; i++)
			{
				MissionBattleUIBaseView missionBattleUIBaseView;
				if ((missionBattleUIBaseView = base.Mission.MissionBehaviors[i] as MissionBattleUIBaseView) != null)
				{
					if (!isPlayerMovementEnabled)
					{
						missionBattleUIBaseView.SuspendView();
					}
					else
					{
						missionBattleUIBaseView.ResumeView();
					}
				}
			}
			if (isPlayerMovementEnabled)
			{
				this._gauntletLayer.IsFocusLayer = false;
				ScreenManager.TryLoseFocus(this._gauntletLayer);
				this._gauntletLayer.InputRestrictions.ResetInputRestrictions();
				this._disableOrderUI = false;
				this._disableShipMarkers = false;
				this._disableStealthBar = false;
				this._disableNameMarkers = false;
				this._disableAgentBanners = false;
				this._disableShipHighlights = false;
				this.HandleOrderUISuspendStateChange();
				this.HandleShipMarkersSuspendStateChange();
				this.HandleStealthBarSuspendStateChange();
				this.HandleNameMarkersSuspendStateChange();
				this.HandleAgentBannerSuspendStateChange();
				this.HandleShipHighlightSuspendStateChange();
				return;
			}
			this._gauntletLayer.IsFocusLayer = true;
			ScreenManager.TrySetFocus(this._gauntletLayer);
			this._gauntletLayer.InputRestrictions.SetInputRestrictions(false, 7);
		}

		// Token: 0x04000065 RID: 101
		private bool _disableOrderUI;

		// Token: 0x04000066 RID: 102
		private bool _isOrderUIDisabled;

		// Token: 0x04000067 RID: 103
		private bool _disableShipMarkers;

		// Token: 0x04000068 RID: 104
		private bool _isShipMarkersDisabled;

		// Token: 0x04000069 RID: 105
		private bool _disableStealthBar;

		// Token: 0x0400006A RID: 106
		private bool _isStealthBarDisabled;

		// Token: 0x0400006B RID: 107
		private bool _disableNameMarkers;

		// Token: 0x0400006C RID: 108
		private bool _isNameMarkersDisabled;

		// Token: 0x0400006D RID: 109
		private bool _disableAgentBanners;

		// Token: 0x0400006E RID: 110
		private bool _isAgentBannersDisabled;

		// Token: 0x0400006F RID: 111
		private bool _disableShipHighlights;

		// Token: 0x04000070 RID: 112
		private bool _isShipHighlightsDisabled;

		// Token: 0x04000071 RID: 113
		private bool _isShipCameraUpdatedAtTheStartOfApproachPhase;

		// Token: 0x04000072 RID: 114
		private bool _isShipCameraUpdatedAtTheStartOfPhase3;

		// Token: 0x04000073 RID: 115
		private MissionGauntletQuest5SetPieceBattleMissionView.Quest5CutsceneGauntletLayer _gauntletLayer;

		// Token: 0x02000034 RID: 52
		private class Quest5CutsceneGauntletLayer : GauntletLayer
		{
			// Token: 0x06000137 RID: 311 RVA: 0x0000AA1E File Offset: 0x00008C1E
			public Quest5CutsceneGauntletLayer(int localOrder, bool shouldClear = false)
				: base("Quest5CutsceneGauntletLayer", localOrder, shouldClear)
			{
			}

			// Token: 0x06000138 RID: 312 RVA: 0x0000AA2D File Offset: 0x00008C2D
			public override bool HitTest()
			{
				return true;
			}
		}
	}
}
