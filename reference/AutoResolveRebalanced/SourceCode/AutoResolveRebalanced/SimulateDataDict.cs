using System;
using System.Collections.Concurrent;
using TaleWorlds.CampaignSystem.MapEvents;

namespace AutoResolveRebalanced
{
	// Token: 0x02000010 RID: 16
	public static class SimulateDataDict
	{
		// Token: 0x0600008D RID: 141 RVA: 0x00003519 File Offset: 0x00001719
		public static bool AddData(MapEventSide side, SimulateData sd)
		{
			return SimulateDataDict._dict.TryAdd(side, sd);
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00003528 File Offset: 0x00001728
		public static bool GetData(MapEventSide side, out SimulateData sd)
		{
			SimulateData simulateData;
			bool flag = SimulateDataDict._dict.TryGetValue(side, out simulateData);
			if (flag)
			{
				sd = simulateData;
				return flag;
			}
			sd = null;
			return flag;
		}

		// Token: 0x0600008F RID: 143 RVA: 0x0000354C File Offset: 0x0000174C
		public static bool RemoveData(MapEventSide side)
		{
			SimulateData simulateData;
			return SimulateDataDict._dict.TryRemove(side, out simulateData);
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00003566 File Offset: 0x00001766
		public static void ClearData()
		{
			SimulateDataDict._dict.Clear();
		}

		// Token: 0x04000034 RID: 52
		private static ConcurrentDictionary<MapEventSide, SimulateData> _dict = new ConcurrentDictionary<MapEventSide, SimulateData>();
	}
}
