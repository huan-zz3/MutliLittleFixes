using System;
using System.Collections.Generic;
using TaleWorlds.MountAndBlade;

namespace MissionLibrary.Event
{
	// Token: 0x02000021 RID: 33
	public class MissionEvent
	{
		// Token: 0x14000002 RID: 2
		// (add) Token: 0x0600007A RID: 122 RVA: 0x00002664 File Offset: 0x00000864
		// (remove) Token: 0x0600007B RID: 123 RVA: 0x00002698 File Offset: 0x00000898
		public static event Action<Agent> MainAgentWillBeChangedToAnotherOne;

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x0600007C RID: 124 RVA: 0x000026CC File Offset: 0x000008CC
		// (remove) Token: 0x0600007D RID: 125 RVA: 0x00002700 File Offset: 0x00000900
		public static event Action<bool> ToggleFreeCamera;

		// Token: 0x14000004 RID: 4
		// (add) Token: 0x0600007E RID: 126 RVA: 0x00002734 File Offset: 0x00000934
		// (remove) Token: 0x0600007F RID: 127 RVA: 0x00002768 File Offset: 0x00000968
		public static event MissionEvent.SwitchTeamDelegate PreSwitchTeam;

		// Token: 0x14000005 RID: 5
		// (add) Token: 0x06000080 RID: 128 RVA: 0x0000279C File Offset: 0x0000099C
		// (remove) Token: 0x06000081 RID: 129 RVA: 0x000027D0 File Offset: 0x000009D0
		public static event MissionEvent.SwitchTeamDelegate PostSwitchTeam;

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x06000082 RID: 130 RVA: 0x00002804 File Offset: 0x00000A04
		// (remove) Token: 0x06000083 RID: 131 RVA: 0x00002838 File Offset: 0x00000A38
		public static event Action MissionMenuClosed;

		// Token: 0x06000084 RID: 132 RVA: 0x0000286C File Offset: 0x00000A6C
		public static void Register(string eventId, string receiverId, Action<object[]> callback)
		{
			if (MissionEvent._eventMapping == null)
			{
				MissionEvent._eventMapping = new Dictionary<string, Dictionary<string, Action<object[]>>>();
			}
			Dictionary<string, Dictionary<string, Action<object[]>>> eventMapping = MissionEvent._eventMapping;
			Dictionary<string, Action<object[]>> dictionary;
			if ((dictionary = eventMapping[eventId]) == null)
			{
				dictionary = (eventMapping[eventId] = new Dictionary<string, Action<object[]>>());
			}
			dictionary[receiverId] = callback;
		}

		// Token: 0x06000085 RID: 133 RVA: 0x000028B4 File Offset: 0x00000AB4
		public static void TriggerEvent(string eventId, object[] param)
		{
			Dictionary<string, Dictionary<string, Action<object[]>>> eventMapping = MissionEvent._eventMapping;
			if (((eventMapping != null) ? eventMapping[eventId] : null) == null)
			{
				return;
			}
			Dictionary<string, Dictionary<string, Action<object[]>>> eventMapping2 = MissionEvent._eventMapping;
			foreach (KeyValuePair<string, Action<object[]>> keyValuePair in ((eventMapping2 != null) ? eventMapping2[eventId] : null))
			{
				Action<object[]> value = keyValuePair.Value;
				if (value != null)
				{
					value(param);
				}
			}
		}

		// Token: 0x06000086 RID: 134 RVA: 0x00002934 File Offset: 0x00000B34
		public static void TriggerEvent(string eventId, string receiverId, object[] param)
		{
			Dictionary<string, Dictionary<string, Action<object[]>>> eventMapping = MissionEvent._eventMapping;
			if (eventMapping == null)
			{
				return;
			}
			Dictionary<string, Action<object[]>> dictionary = eventMapping[eventId];
			if (dictionary == null)
			{
				return;
			}
			Action<object[]> action = dictionary[receiverId];
			if (action == null)
			{
				return;
			}
			action(param);
		}

		// Token: 0x06000087 RID: 135 RVA: 0x0000295C File Offset: 0x00000B5C
		public static void Clear()
		{
			MissionEvent.MainAgentWillBeChangedToAnotherOne = null;
			MissionEvent._eventMapping = null;
			MissionEvent.ToggleFreeCamera = null;
			MissionEvent.PreSwitchTeam = null;
			MissionEvent.PostSwitchTeam = null;
			MissionEvent.MissionMenuClosed = null;
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00002982 File Offset: 0x00000B82
		public static void OnMainAgentWillBeChangedToAnotherOne(Agent newAgent)
		{
			Action<Agent> mainAgentWillBeChangedToAnotherOne = MissionEvent.MainAgentWillBeChangedToAnotherOne;
			if (mainAgentWillBeChangedToAnotherOne == null)
			{
				return;
			}
			mainAgentWillBeChangedToAnotherOne(newAgent);
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00002994 File Offset: 0x00000B94
		public static void OnToggleFreeCamera(bool freeCamera)
		{
			Action<bool> toggleFreeCamera = MissionEvent.ToggleFreeCamera;
			if (toggleFreeCamera == null)
			{
				return;
			}
			toggleFreeCamera(freeCamera);
		}

		// Token: 0x0600008A RID: 138 RVA: 0x000029A6 File Offset: 0x00000BA6
		public static void OnPreSwitchTeam()
		{
			MissionEvent.SwitchTeamDelegate preSwitchTeam = MissionEvent.PreSwitchTeam;
			if (preSwitchTeam == null)
			{
				return;
			}
			preSwitchTeam();
		}

		// Token: 0x0600008B RID: 139 RVA: 0x000029B7 File Offset: 0x00000BB7
		public static void OnPostSwitchTeam()
		{
			MissionEvent.SwitchTeamDelegate postSwitchTeam = MissionEvent.PostSwitchTeam;
			if (postSwitchTeam == null)
			{
				return;
			}
			postSwitchTeam();
		}

		// Token: 0x0600008C RID: 140 RVA: 0x000029C8 File Offset: 0x00000BC8
		public static void OnMissionMenuClosed()
		{
			Action missionMenuClosed = MissionEvent.MissionMenuClosed;
			if (missionMenuClosed == null)
			{
				return;
			}
			missionMenuClosed();
		}

		// Token: 0x0400000F RID: 15
		private static Dictionary<string, Dictionary<string, Action<object[]>>> _eventMapping = new Dictionary<string, Dictionary<string, Action<object[]>>>();

		// Token: 0x0200002A RID: 42
		// (Invoke) Token: 0x060000BA RID: 186
		public delegate void SwitchTeamDelegate();
	}
}
