using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC
{
	// Token: 0x02000023 RID: 35
	public class NavalMissionState : MissionState
	{
		// Token: 0x06000182 RID: 386 RVA: 0x00009DB4 File Offset: 0x00007FB4
		public static Mission OpenNew(string missionName, MissionInitializerRecord rec, InitializeMissionBehaviorsDelegate handler, bool addDefaultMissionBehaviors = true, bool needsMemoryCleanup = true)
		{
			Debug.Print(string.Concat(new string[] { "Opening new mission ", missionName, " ", rec.SceneLevels, ".\n" }), 0, 12, 17592186044416UL);
			if (!GameNetwork.IsClientOrReplay && !GameNetwork.IsServer)
			{
				MBCommon.CurrentGameType = (MissionState.IsRecordingActive() ? 5 : 0);
			}
			Game.Current.OnMissionIsStarting(missionName, rec);
			NavalMissionState navalMissionState = Game.Current.GameStateManager.CreateState<NavalMissionState>();
			Mission mission = navalMissionState.HandleOpenNew(missionName, rec, handler, addDefaultMissionBehaviors, needsMemoryCleanup);
			mission.SetCloseProximityWaveSoundsEnabled(true);
			mission.ForceDisableOcclusion(true);
			Game.Current.GameStateManager.PushState(navalMissionState, 0);
			return mission;
		}
	}
}
