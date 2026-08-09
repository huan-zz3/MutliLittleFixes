using System;
using System.Collections.Generic;
using NavalDLC.Missions.BattleScore;
using NavalDLC.View.MissionViews;
using NavalDLC.View.MissionViews.Order;
using NavalDLC.ViewModelCollection;
using SandBox.View;
using SandBox.View.Missions;
using SandBox.ViewModelCollection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.BattleScore;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer;
using TaleWorlds.MountAndBlade.View.MissionViews.Sound;
using TaleWorlds.MountAndBlade.ViewModelCollection.Scoreboard;

namespace NavalDLC.View
{
	// Token: 0x0200000C RID: 12
	[ViewCreatorModule]
	public class NavalViews
	{
		// Token: 0x06000054 RID: 84 RVA: 0x00003AB8 File Offset: 0x00001CB8
		[ViewMethod("NavalBattle")]
		public static MissionView[] OpenNavalBattleMission(Mission mission)
		{
			List<MissionView> list = new List<MissionView>();
			list.Add(ViewCreator.CreateMissionSingleplayerEscapeMenu(false));
			list.Add(ViewCreator.CreateMissionAgentLabelUIHandler(mission));
			list.Add(ViewCreator.CreateMissionBattleScoreUIHandler(mission, NavalScoreboardVM.CreateMission(mission)));
			list.Add(ViewCreator.CreateOptionsUIHandler());
			list.Add(ViewCreator.CreateMissionMainAgentEquipDropView(mission));
			MissionView missionView = NavalViewCreator.CreateNavalOrderUIHandler(mission);
			list.Add(missionView);
			list.Add(new MissionFormationTargetSelectionHandler());
			list.Add(new NavalOrderTroopPlacer(null));
			list.Add(ViewCreator.CreateMissionAgentStatusUIHandler(mission));
			list.Add(ViewCreator.CreateMissionMainAgentEquipmentController(mission));
			list.Add(ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission));
			list.Add(ViewCreator.CreateMissionAgentLockVisualizerView(mission));
			list.Add(new MusicNavalBattleMissionView());
			list.Add(new NavalAmbientShoutsView());
			list.Add(new NavalDeploymentMissionView());
			list.Add(ViewCreator.CreateMissionBoundaryCrossingView());
			list.Add(new MissionBoundaryWallView());
			list.Add(new NavalMissionDeploymentBoundaryMarker("buoy_small_a", "buoy_big_a", 20f));
			list.Add(ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler());
			list.Add(ViewCreator.CreateMissionSpectatorControlView(mission));
			list.Add(ViewCreator.CreatePhotoModeView());
			list.Add(new MissionItemContourControllerView());
			list.Add(new MissionAgentContourControllerView());
			list.Add(NavalViewCreator.CreateMissionShipControlView(mission));
			list.Add(new NavalMissionPrepareView());
			list.Add(SandBoxViewCreator.CreateMissionNameMarkerUIHandler(mission));
			list.Add(NavalViewCreator.CreateNavalShipMarkerUIHandler(mission));
			list.Add(NavalViewCreator.CreateNavalOrderOfBattleView(mission));
			list.Add(NavalViewCreator.CreateNavalShipTargetSelectionHandler(mission));
			list.Add(NavalViewCreator.CreateNavalMissionCaptureShipView(mission));
			list.Add(new NavalMissionShipHighlightView());
			list.Add(new MissionCampaignView());
			list.Add(new MissionPreloadView());
			list.Add(new NavalShipsPreloadView());
			return list.ToArray();
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00003C70 File Offset: 0x00001E70
		[ViewMethod("NavalRaid")]
		public static MissionView[] OpenNavalRaidMission(Mission mission)
		{
			List<MissionView> list = new List<MissionView>();
			list.Add(ViewCreator.CreateMissionSingleplayerEscapeMenu(false));
			list.Add(ViewCreator.CreateMissionAgentLabelUIHandler(mission));
			list.Add(ViewCreator.CreateMissionBattleScoreUIHandler(mission, NavalScoreboardVM.CreateMission(mission)));
			list.Add(ViewCreator.CreateOptionsUIHandler());
			list.Add(ViewCreator.CreateMissionMainAgentEquipDropView(mission));
			MissionView missionView = ViewCreator.CreateMissionOrderUIHandler(mission);
			list.Add(missionView);
			list.Add(new MissionFormationTargetSelectionHandler());
			list.Add(new NavalOrderTroopPlacer(null));
			list.Add(ViewCreator.CreateMissionAgentStatusUIHandler(mission));
			list.Add(ViewCreator.CreateMissionMainAgentEquipmentController(mission));
			list.Add(ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission));
			list.Add(ViewCreator.CreateMissionAgentLockVisualizerView(mission));
			list.Add(new MusicNavalBattleMissionView());
			list.Add(new NavalAmbientShoutsView());
			list.Add(new NavalDeploymentMissionView());
			list.Add(ViewCreator.CreateMissionBoundaryCrossingView());
			list.Add(new MissionBoundaryWallView());
			list.Add(new NavalMissionDeploymentBoundaryMarker("buoy_small_a", "buoy_big_a", 20f));
			list.Add(ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler());
			list.Add(ViewCreator.CreateMissionSpectatorControlView(mission));
			list.Add(ViewCreator.CreatePhotoModeView());
			list.Add(new MissionItemContourControllerView());
			list.Add(new MissionAgentContourControllerView());
			list.Add(new NavalMissionPrepareView());
			list.Add(SandBoxViewCreator.CreateMissionNameMarkerUIHandler(mission));
			list.Add(ViewCreator.CreateMissionFormationMarkerUIHandler(mission));
			list.Add(new MissionCampaignView());
			list.Add(new MissionPreloadView());
			list.Add(new NavalShipsPreloadView());
			return list.ToArray();
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00003DEC File Offset: 0x00001FEC
		[ViewMethod("NavalCustomBattle")]
		public static MissionView[] OpenNavalBattleForCustomMission(Mission mission)
		{
			List<MissionView> list = new List<MissionView>();
			list.Add(ViewCreator.CreateMissionSingleplayerEscapeMenu(false));
			list.Add(ViewCreator.CreateMissionAgentLabelUIHandler(mission));
			list.Add(ViewCreator.CreateMissionBattleScoreUIHandler(mission, NavalCustomBattleScoreboardVM.Create(mission, null)));
			list.Add(ViewCreator.CreateOptionsUIHandler());
			list.Add(ViewCreator.CreateMissionMainAgentEquipDropView(mission));
			MissionView missionView = NavalViewCreator.CreateNavalOrderUIHandler(mission);
			list.Add(missionView);
			list.Add(new MissionFormationTargetSelectionHandler());
			list.Add(new NavalOrderTroopPlacer(null));
			list.Add(ViewCreator.CreateMissionAgentStatusUIHandler(mission));
			list.Add(ViewCreator.CreateMissionMainAgentEquipmentController(mission));
			list.Add(ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission));
			list.Add(ViewCreator.CreateMissionAgentLockVisualizerView(mission));
			list.Add(new MusicNavalBattleMissionView());
			list.Add(new NavalAmbientShoutsView());
			list.Add(new NavalDeploymentMissionView());
			list.Add(ViewCreator.CreateMissionBoundaryCrossingView());
			list.Add(new MissionBoundaryWallView());
			list.Add(new NavalMissionDeploymentBoundaryMarker("buoy_small_a", "buoy_big_a", 20f));
			list.Add(ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler());
			list.Add(ViewCreator.CreateMissionSpectatorControlView(mission));
			list.Add(ViewCreator.CreatePhotoModeView());
			list.Add(new MissionItemContourControllerView());
			list.Add(new MissionAgentContourControllerView());
			list.Add(NavalViewCreator.CreateMissionShipControlView(mission));
			list.Add(new NavalMissionPrepareView());
			list.Add(SandBoxViewCreator.CreateMissionNameMarkerUIHandler(mission));
			list.Add(NavalViewCreator.CreateNavalShipMarkerUIHandler(mission));
			list.Add(NavalViewCreator.CreateNavalOrderOfBattleView(mission));
			list.Add(NavalViewCreator.CreateNavalShipTargetSelectionHandler(mission));
			list.Add(NavalViewCreator.CreateNavalMissionCaptureShipView(mission));
			list.Add(new NavalMissionShipHighlightView());
			list.Add(new MissionCampaignView());
			list.Add(new MissionCustomBattlePreloadView());
			list.Add(new NavalShipsPreloadView());
			return list.ToArray();
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00003FA4 File Offset: 0x000021A4
		[ViewMethod("NavalRaidCustomBattle")]
		public static MissionView[] OpenNavalRaidBattleForCustomMission(Mission mission)
		{
			List<MissionView> list = new List<MissionView>();
			list.Add(ViewCreator.CreateMissionSingleplayerEscapeMenu(false));
			list.Add(ViewCreator.CreateMissionAgentLabelUIHandler(mission));
			list.Add(ViewCreator.CreateMissionBattleScoreUIHandler(mission, new CustomBattleScoreboardVM(new CustomBattleScoreContext(mission))));
			list.Add(ViewCreator.CreateOptionsUIHandler());
			list.Add(ViewCreator.CreateMissionMainAgentEquipDropView(mission));
			MissionView missionView = ViewCreator.CreateMissionOrderUIHandler(mission);
			list.Add(missionView);
			list.Add(new MissionFormationTargetSelectionHandler());
			list.Add(new NavalOrderTroopPlacer(null));
			list.Add(ViewCreator.CreateMissionAgentStatusUIHandler(mission));
			list.Add(ViewCreator.CreateMissionMainAgentEquipmentController(mission));
			list.Add(ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission));
			list.Add(ViewCreator.CreateMissionAgentLockVisualizerView(mission));
			list.Add(new MusicNavalBattleMissionView());
			list.Add(new NavalAmbientShoutsView());
			list.Add(new NavalDeploymentMissionView());
			list.Add(ViewCreator.CreateMissionBoundaryCrossingView());
			list.Add(new MissionBoundaryWallView());
			list.Add(new NavalMissionDeploymentBoundaryMarker("buoy_small_a", "buoy_big_a", 20f));
			list.Add(ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler());
			list.Add(ViewCreator.CreateMissionSpectatorControlView(mission));
			list.Add(ViewCreator.CreatePhotoModeView());
			list.Add(new MissionItemContourControllerView());
			list.Add(new MissionAgentContourControllerView());
			list.Add(new NavalMissionPrepareView());
			list.Add(SandBoxViewCreator.CreateMissionNameMarkerUIHandler(mission));
			list.Add(ViewCreator.CreateMissionFormationMarkerUIHandler(mission));
			list.Add(new MissionCampaignView());
			list.Add(new MissionCustomBattlePreloadView());
			list.Add(new NavalShipsPreloadView());
			return list.ToArray();
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00004124 File Offset: 0x00002324
		[ViewMethod("NavalCaptivityBattle")]
		public static MissionView[] OpenNavalCaptivityBattleMission(Mission mission)
		{
			return new List<MissionView>
			{
				ViewCreator.CreateMissionSingleplayerEscapeMenu(false),
				ViewCreator.CreateMissionAgentLabelUIHandler(mission),
				ViewCreator.CreateOptionsUIHandler(),
				ViewCreator.CreateMissionMainAgentEquipDropView(mission),
				ViewCreator.CreateMissionAgentStatusUIHandler(mission),
				ViewCreator.CreateMissionMainAgentEquipmentController(mission),
				ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission),
				ViewCreator.CreateMissionAgentLockVisualizerView(mission),
				new MusicSilencedMissionView(),
				new MissionBoundaryWallView(),
				ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler(),
				ViewCreator.CreateMissionSpectatorControlView(mission),
				ViewCreator.CreatePhotoModeView(),
				NavalViewCreator.CreateMissionShipControlView(mission),
				new MissionItemContourControllerView(),
				new MissionAgentContourControllerView(),
				new MissionConversationCameraView(),
				SandBoxViewCreator.CreateMissionConversationView(mission),
				ViewCreator.CreateMissionLeaveView(),
				new NavalMissionPrepareView(),
				new MissionCampaignView(),
				ViewCreator.CreateMissionHintView(mission),
				ViewCreator.CreateMissionObjectiveView(mission),
				SandBoxViewCreator.CreateMissionNameMarkerUIHandler(mission),
				NavalViewCreator.CreateCaptivityMissionView(mission),
				NavalViewCreator.CreateNavalMissionCaptureShipView(mission)
			}.ToArray();
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00004268 File Offset: 0x00002468
		[ViewMethod("BlockedEstuary")]
		public static MissionView[] OpenNavalSetPieceBattleMission(Mission mission)
		{
			return new List<MissionView>
			{
				ViewCreator.CreateMissionSingleplayerEscapeMenu(false),
				ViewCreator.CreateMissionAgentLabelUIHandler(mission),
				ViewCreator.CreateMissionBattleScoreUIHandler(mission, NavalScoreboardVM.CreateMission(mission)),
				ViewCreator.CreateOptionsUIHandler(),
				ViewCreator.CreateMissionMainAgentEquipDropView(mission),
				ViewCreator.CreateMissionObjectiveView(mission),
				ViewCreator.CreateMissionAgentStatusUIHandler(mission),
				ViewCreator.CreateMissionMainAgentEquipmentController(mission),
				ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission),
				ViewCreator.CreateMissionAgentLockVisualizerView(mission),
				new MusicSilencedMissionView(),
				NavalViewCreator.CreateNavalShipMarkerUIHandler(mission),
				ViewCreator.CreateMissionBoundaryCrossingView(),
				new MissionBoundaryWallView(),
				ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler(),
				ViewCreator.CreateMissionSpectatorControlView(mission),
				ViewCreator.CreatePhotoModeView(),
				NavalViewCreator.CreateMissionShipControlView(mission),
				new MissionItemContourControllerView(),
				new MissionAgentContourControllerView(),
				new MissionConversationCameraView(),
				SandBoxViewCreator.CreateMissionConversationView(mission),
				ViewCreator.CreateMissionLeaveView(),
				SandBoxViewCreator.CreateMissionNameMarkerUIHandler(mission),
				new NavalMissionPrepareView(),
				new BlockedEstuaryView(),
				NavalViewCreator.CreateNavalMissionCaptureShipView(mission),
				new MissionCampaignView()
			}.ToArray();
		}

		// Token: 0x0600005A RID: 90 RVA: 0x000043C8 File Offset: 0x000025C8
		[ViewMethod("NavalStorylinePirateBattle")]
		public static MissionView[] OpenNavalStorylinePirateBattleMission(Mission mission)
		{
			List<MissionView> list = new List<MissionView>();
			list.Add(ViewCreator.CreateMissionSingleplayerEscapeMenu(false));
			list.Add(ViewCreator.CreateMissionAgentLabelUIHandler(mission));
			list.Add(ViewCreator.CreateMissionBattleScoreUIHandler(mission, NavalScoreboardVM.CreateCustom(new NavalStorylinePirateBattleScoreContext(mission), null)));
			list.Add(ViewCreator.CreateOptionsUIHandler());
			list.Add(ViewCreator.CreateMissionMainAgentEquipDropView(mission));
			MissionView missionView = NavalViewCreator.CreateNavalOrderUIHandler(mission);
			list.Add(missionView);
			list.Add(new MissionFormationTargetSelectionHandler());
			list.Add(new NavalOrderTroopPlacer(null));
			list.Add(ViewCreator.CreateMissionAgentStatusUIHandler(mission));
			list.Add(ViewCreator.CreateMissionMainAgentEquipmentController(mission));
			list.Add(ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission));
			list.Add(ViewCreator.CreateMissionAgentLockVisualizerView(mission));
			list.Add(new MusicSilencedMissionView());
			list.Add(ViewCreator.CreateMissionBoundaryCrossingView());
			list.Add(new MissionBoundaryWallView());
			list.Add(ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler());
			list.Add(ViewCreator.CreateMissionSpectatorControlView(mission));
			list.Add(ViewCreator.CreatePhotoModeView());
			list.Add(new MissionItemContourControllerView());
			list.Add(new MissionAgentContourControllerView());
			list.Add(NavalViewCreator.CreateMissionShipControlView(mission));
			list.Add(NavalViewCreator.CreateNavalShipMarkerUIHandler(mission));
			list.Add(NavalViewCreator.CreateNavalShipTargetSelectionHandler(mission));
			list.Add(new NavalMissionShipHighlightView());
			list.Add(NavalViewCreator.CreatePirateBattleMissionView(mission));
			list.Add(new MissionConversationCameraView());
			list.Add(SandBoxViewCreator.CreateMissionConversationView(mission));
			list.Add(ViewCreator.CreateMissionLeaveView());
			list.Add(new MissionCampaignView());
			list.Add(new NavalMissionPrepareView());
			list.Add(SandBoxViewCreator.CreateMissionNameMarkerUIHandler(mission));
			list.Add(ViewCreator.CreateMissionObjectiveView(mission));
			list.Add(NavalViewCreator.CreateNavalMissionCaptureShipView(mission));
			return list.ToArray();
		}

		// Token: 0x0600005B RID: 91 RVA: 0x0000456C File Offset: 0x0000276C
		[ViewMethod("HelpAnAllySetPieceBattle")]
		public static MissionView[] OpenHelpAnAllySetPieceBattle(Mission mission)
		{
			List<MissionView> list = new List<MissionView>();
			list.Add(NavalViewCreator.CreateHelpingAnAllyMissionView(null));
			list.Add(ViewCreator.CreateMissionSingleplayerEscapeMenu(false));
			list.Add(ViewCreator.CreateMissionAgentLabelUIHandler(mission));
			list.Add(ViewCreator.CreateMissionBattleScoreUIHandler(mission, NavalScoreboardVM.CreateMission(mission)));
			list.Add(ViewCreator.CreateOptionsUIHandler());
			list.Add(ViewCreator.CreateMissionMainAgentEquipDropView(mission));
			MissionView missionView = NavalViewCreator.CreateNavalOrderUIHandler(mission);
			list.Add(missionView);
			list.Add(new MissionFormationTargetSelectionHandler());
			list.Add(new NavalOrderTroopPlacer(null));
			list.Add(ViewCreator.CreateMissionAgentStatusUIHandler(mission));
			list.Add(ViewCreator.CreateMissionMainAgentEquipmentController(mission));
			list.Add(ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission));
			list.Add(ViewCreator.CreateMissionAgentLockVisualizerView(mission));
			list.Add(new MusicNavalBattleMissionView());
			list.Add(new NavalMissionPrepareView());
			list.Add(ViewCreator.CreateMissionBoundaryCrossingView());
			list.Add(new MissionBoundaryWallView());
			list.Add(ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler());
			list.Add(ViewCreator.CreateMissionSpectatorControlView(mission));
			list.Add(ViewCreator.CreatePhotoModeView());
			list.Add(new MissionItemContourControllerView());
			list.Add(new MissionAgentContourControllerView());
			list.Add(NavalViewCreator.CreateMissionShipControlView(mission));
			list.Add(NavalViewCreator.CreateNavalShipMarkerUIHandler(mission));
			list.Add(NavalViewCreator.CreateNavalShipTargetSelectionHandler(mission));
			list.Add(new NavalMissionShipHighlightView());
			list.Add(ViewCreator.CreateMissionObjectiveView(null));
			list.Add(SandBoxViewCreator.CreateMissionNameMarkerUIHandler(mission));
			list.Add(new MissionConversationCameraView());
			list.Add(SandBoxViewCreator.CreateMissionConversationView(mission));
			list.Add(ViewCreator.CreateMissionLeaveView());
			list.Add(NavalViewCreator.CreateNavalMissionCaptureShipView(mission));
			return list.ToArray();
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00004700 File Offset: 0x00002900
		[ViewMethod("NavalStorylineQuest5SetPieceBattleMission")]
		public static MissionView[] OpenNavalStorylineQuest5SetPieceBattleMission(Mission mission)
		{
			List<MissionView> list = new List<MissionView>();
			list.Add(ViewCreator.CreateMissionObjectiveView(null));
			list.Add(ViewCreator.CreateMissionSingleplayerEscapeMenu(false));
			list.Add(ViewCreator.CreateMissionAgentLabelUIHandler(mission));
			list.Add(ViewCreator.CreateMissionBattleScoreUIHandler(mission, NavalScoreboardVM.CreateMission(mission)));
			list.Add(ViewCreator.CreateOptionsUIHandler());
			list.Add(ViewCreator.CreateMissionMainAgentEquipDropView(mission));
			MissionView missionView = NavalViewCreator.CreateNavalOrderUIHandler(mission);
			list.Add(missionView);
			list.Add(new MissionFormationTargetSelectionHandler());
			list.Add(new NavalOrderTroopPlacer(null));
			list.Add(ViewCreator.CreateMissionAgentStatusUIHandler(mission));
			list.Add(ViewCreator.CreateMissionMainAgentEquipmentController(mission));
			list.Add(ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission));
			list.Add(ViewCreator.CreateMissionAgentLockVisualizerView(mission));
			list.Add(new MusicSilencedMissionView());
			list.Add(ViewCreator.CreateMissionBoundaryCrossingView());
			list.Add(new MissionBoundaryWallView());
			list.Add(ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler());
			list.Add(ViewCreator.CreateMissionSpectatorControlView(mission));
			list.Add(ViewCreator.CreatePhotoModeView());
			list.Add(new MissionItemContourControllerView());
			list.Add(new MissionAgentContourControllerView());
			list.Add(NavalViewCreator.CreateMissionShipControlView(mission));
			list.Add(new NavalMissionPrepareView());
			list.Add(NavalViewCreator.CreateNavalShipMarkerUIHandler(mission));
			list.Add(NavalViewCreator.CreateNavalShipTargetSelectionHandler(mission));
			list.Add(new NavalMissionShipHighlightView());
			list.Add(new MusicStealthMissionView());
			list.Add(new MissionCampaignView());
			list.Add(new MissionConversationCameraView());
			list.Add(SandBoxViewCreator.CreateMissionConversationView(mission));
			list.Add(ViewCreator.CreateMissionLeaveView());
			list.Add(NavalViewCreator.CreateQuest5SetPieceBattleMissionView(mission));
			list.Add(NavalViewCreator.CreateQuest5SetPieceBattleBossFightCameraView(mission));
			list.Add(NavalViewCreator.CreateQuest5SetPieceBattleInteriorConversationCameraView(mission));
			list.Add(SandBoxViewCreator.CreateMissionNameMarkerUIHandler(mission));
			list.Add(SandBoxViewCreator.CreateMissionAgentAlarmStateView(mission));
			list.Add(NavalViewCreator.CreateNavalMissionCaptureShipView(mission));
			return list.ToArray();
		}

		// Token: 0x0600005D RID: 93 RVA: 0x000048CC File Offset: 0x00002ACC
		[ViewMethod("NavalFinalConversationMission")]
		public static MissionView[] OpenNavalFinalConversationMission(Mission mission)
		{
			return new List<MissionView>
			{
				new MissionCampaignView(),
				new MissionConversationCameraView(),
				SandBoxViewCreator.CreateMissionConversationView(mission),
				ViewCreator.CreateMissionSingleplayerEscapeMenu(CampaignOptions.IsIronmanMode),
				ViewCreator.CreateOptionsUIHandler(),
				ViewCreator.CreateMissionMainAgentEquipDropView(mission),
				new MissionSingleplayerViewHandler(),
				ViewCreator.CreateMissionAgentStatusUIHandler(mission),
				ViewCreator.CreateMissionMainAgentEquipmentController(mission),
				ViewCreator.CreateMissionAgentLockVisualizerView(mission),
				new MusicSilencedMissionView(),
				SandBoxViewCreator.CreateMissionBarterView(),
				ViewCreator.CreateMissionLeaveView(),
				SandBoxViewCreator.CreateBoardGameView(),
				SandBoxViewCreator.CreateMissionNameMarkerUIHandler(mission),
				new MissionItemContourControllerView(),
				new MissionAgentContourControllerView(),
				new MissionCampaignBattleSpectatorView(),
				ViewCreator.CreatePhotoModeView(),
				ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler(),
				new NavalFinalConversationMissionView()
			}.ToArray();
		}

		// Token: 0x0600005E RID: 94 RVA: 0x000049D8 File Offset: 0x00002BD8
		[ViewMethod("NavalStorylineWoundedBeastBattle")]
		public static MissionView[] OpenNavalStorylineWoundedBeastBattleMission(Mission mission)
		{
			List<MissionView> list = new List<MissionView>();
			list.Add(ViewCreator.CreateMissionObjectiveView(null));
			list.Add(ViewCreator.CreateMissionSingleplayerEscapeMenu(false));
			list.Add(ViewCreator.CreateMissionAgentLabelUIHandler(mission));
			list.Add(ViewCreator.CreateMissionBattleScoreUIHandler(mission, NavalScoreboardVM.CreateMission(mission)));
			list.Add(ViewCreator.CreateOptionsUIHandler());
			list.Add(ViewCreator.CreateMissionMainAgentEquipDropView(mission));
			MissionView missionView = NavalViewCreator.CreateNavalOrderUIHandler(mission);
			list.Add(missionView);
			list.Add(new MissionFormationTargetSelectionHandler());
			list.Add(new NavalOrderTroopPlacer(null));
			list.Add(new MusicNavalBattleMissionView());
			list.Add(ViewCreator.CreateMissionAgentStatusUIHandler(mission));
			list.Add(ViewCreator.CreateMissionMainAgentEquipmentController(mission));
			list.Add(ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission));
			list.Add(ViewCreator.CreateMissionAgentLockVisualizerView(mission));
			list.Add(new MusicSilencedMissionView());
			list.Add(ViewCreator.CreateMissionBoundaryCrossingView());
			list.Add(new MissionBoundaryWallView());
			list.Add(ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler());
			list.Add(ViewCreator.CreateMissionSpectatorControlView(mission));
			list.Add(ViewCreator.CreatePhotoModeView());
			list.Add(new MissionItemContourControllerView());
			list.Add(new MissionAgentContourControllerView());
			list.Add(new NavalMissionShipHighlightView());
			list.Add(new NavalMissionPrepareView());
			list.Add(SandBoxViewCreator.CreateMissionNameMarkerUIHandler(mission));
			list.Add(NavalViewCreator.CreateMissionShipControlView(mission));
			list.Add(NavalViewCreator.CreateNavalShipMarkerUIHandler(mission));
			list.Add(ViewCreator.CreateMissionLeaveView());
			list.Add(new WoundedBeastView());
			list.Add(NavalViewCreator.CreateNavalMissionCaptureShipView(mission));
			return list.ToArray();
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00004B54 File Offset: 0x00002D54
		[ViewMethod("FloatingFortressSetPieceBattleMission")]
		public static MissionView[] OpenFloatingFortressSetPieceBattleMission(Mission mission)
		{
			List<MissionView> list = new List<MissionView>();
			list.Add(ViewCreator.CreateMissionSingleplayerEscapeMenu(false));
			list.Add(ViewCreator.CreateMissionAgentLabelUIHandler(mission));
			list.Add(ViewCreator.CreateMissionBattleScoreUIHandler(mission, NavalScoreboardVM.CreateMission(mission)));
			list.Add(ViewCreator.CreateOptionsUIHandler());
			list.Add(ViewCreator.CreateMissionMainAgentEquipDropView(mission));
			MissionView missionView = NavalViewCreator.CreateNavalOrderUIHandler(mission);
			list.Add(missionView);
			list.Add(new MissionFormationTargetSelectionHandler());
			list.Add(new NavalOrderTroopPlacer(null));
			list.Add(ViewCreator.CreateMissionAgentStatusUIHandler(mission));
			list.Add(ViewCreator.CreateMissionMainAgentEquipmentController(mission));
			list.Add(ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission));
			list.Add(ViewCreator.CreateMissionAgentLockVisualizerView(mission));
			list.Add(new MusicSilencedMissionView());
			list.Add(ViewCreator.CreateMissionBoundaryCrossingView());
			list.Add(new MissionBoundaryWallView());
			list.Add(ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler());
			list.Add(ViewCreator.CreateMissionSpectatorControlView(mission));
			list.Add(ViewCreator.CreatePhotoModeView());
			list.Add(new MissionItemContourControllerView());
			list.Add(new MissionAgentContourControllerView());
			list.Add(NavalViewCreator.CreateMissionShipControlView(mission));
			list.Add(NavalViewCreator.CreateNavalShipMarkerUIHandler(mission));
			list.Add(NavalViewCreator.CreateNavalShipTargetSelectionHandler(mission));
			list.Add(new NavalMissionShipHighlightView());
			list.Add(new MissionConversationCameraView());
			list.Add(SandBoxViewCreator.CreateMissionConversationView(mission));
			list.Add(ViewCreator.CreateMissionLeaveView());
			list.Add(NavalViewCreator.CreateFloatingFortressView(null));
			list.Add(ViewCreator.CreateMissionObjectiveView(null));
			list.Add(SandBoxViewCreator.CreateMissionNameMarkerUIHandler(mission));
			list.Add(NavalViewCreator.CreateNavalMissionCaptureShipView(mission));
			list.Add(new NavalMissionPrepareView());
			list.Add(new MissionCampaignView());
			return list.ToArray();
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00004CF4 File Offset: 0x00002EF4
		[ViewMethod("NavalStorylineAlleyFight")]
		public static MissionView[] OpenNavalStorylineAlleyFight(Mission mission)
		{
			return new List<MissionView>
			{
				ViewCreator.CreateMissionSingleplayerEscapeMenu(false),
				ViewCreator.CreateMissionAgentLabelUIHandler(mission),
				ViewCreator.CreateMissionBattleScoreUIHandler(mission, SPScoreboardVM.CreateCustom(new NavalAlleyFightBattleScoreContext(mission), null)),
				ViewCreator.CreateOptionsUIHandler(),
				ViewCreator.CreateMissionMainAgentEquipDropView(mission),
				new MissionFormationTargetSelectionHandler(),
				ViewCreator.CreateMissionAgentStatusUIHandler(mission),
				ViewCreator.CreateMissionMainAgentEquipmentController(mission),
				ViewCreator.CreateMissionAgentLockVisualizerView(mission),
				new MusicSilencedMissionView(),
				new NavalStorylineAlleyFightCinematicView(),
				ViewCreatorManager.CreateMissionView<MissionHintView>(false, mission, Array.Empty<object>()),
				ViewCreator.CreateMissionBoundaryCrossingView(),
				ViewCreator.CreatePhotoModeView(),
				new MissionBoundaryWallView(),
				ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler(),
				ViewCreator.CreateMissionSpectatorControlView(mission),
				new MissionItemContourControllerView(),
				new MissionAgentContourControllerView(),
				new MissionConversationCameraView(),
				SandBoxViewCreator.CreateMissionConversationView(mission),
				ViewCreator.CreateMissionLeaveView()
			}.ToArray();
		}
	}
}
