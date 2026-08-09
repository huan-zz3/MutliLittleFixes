using System;
using NavalDLC.View.MissionViews;
using NavalDLC.View.MissionViews.Storyline;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace TaleWorlds.MountAndBlade.View
{
	// Token: 0x02000002 RID: 2
	public static class NavalViewCreator
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002048 File Offset: 0x00000248
		public static MissionView CreateNavalOrderUIHandler(Mission mission = null)
		{
			return ViewCreatorManager.CreateMissionView<NavalMissionOrderUIHandler>(false, mission, Array.Empty<object>());
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002056 File Offset: 0x00000256
		public static MissionView CreateNavalOrderOfBattleView(Mission mission = null)
		{
			return ViewCreatorManager.CreateMissionView<NavalOrderOfBattleView>(false, mission, new object[] { mission });
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002069 File Offset: 0x00000269
		public static MissionView CreateNavalShipMarkerUIHandler(Mission mission = null)
		{
			return ViewCreatorManager.CreateMissionView<NavalMissionShipMarkerUIHandler>(false, mission, Array.Empty<object>());
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002077 File Offset: 0x00000277
		public static MissionView CreateNavalShipTargetSelectionHandler(Mission mission = null)
		{
			return ViewCreatorManager.CreateMissionView<NavalShipTargetSelectionHandler>(false, mission, Array.Empty<object>());
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002085 File Offset: 0x00000285
		public static MissionView CreateMissionShipControlView(Mission mission = null)
		{
			return ViewCreatorManager.CreateMissionView<MissionShipControlView>(false, mission, Array.Empty<object>());
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002093 File Offset: 0x00000293
		public static MissionView CreateNavalMissionCaptureShipView(Mission mission = null)
		{
			return ViewCreatorManager.CreateMissionView<NavalMissionCaptureShipView>(false, mission, Array.Empty<object>());
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000020A1 File Offset: 0x000002A1
		public static MissionView CreateQuest5SetPieceBattleMissionView(Mission mission = null)
		{
			return ViewCreatorManager.CreateMissionView<Quest5SetPieceBattleMissionView>(false, mission, Array.Empty<object>());
		}

		// Token: 0x06000008 RID: 8 RVA: 0x000020AF File Offset: 0x000002AF
		public static MissionView CreateQuest5SetPieceBattleBossFightCameraView(Mission mission = null)
		{
			return ViewCreatorManager.CreateMissionView<Quest5SetPieceBattleBossFightCameraView>(false, mission, Array.Empty<object>());
		}

		// Token: 0x06000009 RID: 9 RVA: 0x000020BD File Offset: 0x000002BD
		public static MissionView CreateQuest5SetPieceBattleInteriorConversationCameraView(Mission mission = null)
		{
			return ViewCreatorManager.CreateMissionView<Quest5SetPieceBattleInteriorConversationCameraView>(false, mission, Array.Empty<object>());
		}

		// Token: 0x0600000A RID: 10 RVA: 0x000020CB File Offset: 0x000002CB
		public static MissionView CreateCaptivityMissionView(Mission mission = null)
		{
			return ViewCreatorManager.CreateMissionView<NavalCaptivityBattleMissionView>(false, mission, Array.Empty<object>());
		}

		// Token: 0x0600000B RID: 11 RVA: 0x000020D9 File Offset: 0x000002D9
		public static MissionView CreateFloatingFortressView(Mission mission = null)
		{
			return ViewCreatorManager.CreateMissionView<FloatingFortressView>(false, mission, Array.Empty<object>());
		}

		// Token: 0x0600000C RID: 12 RVA: 0x000020E7 File Offset: 0x000002E7
		public static MissionView CreatePirateBattleMissionView(Mission mission = null)
		{
			return ViewCreatorManager.CreateMissionView<NavalStorylinePirateBattleMissionView>(false, mission, Array.Empty<object>());
		}

		// Token: 0x0600000D RID: 13 RVA: 0x000020F5 File Offset: 0x000002F5
		public static MissionView CreateHelpingAnAllyMissionView(Mission mission = null)
		{
			return ViewCreatorManager.CreateMissionView<HelpingAnAllyMissionView>(false, mission, Array.Empty<object>());
		}
	}
}
