using System;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.Core;

namespace AutoResolveRebalanced
{
	// Token: 0x02000008 RID: 8
	internal static class MapEventAccessTools
	{
		// Token: 0x0600000F RID: 15 RVA: 0x00002A0F File Offset: 0x00000C0F
		internal static void InvokeSimulateBattleForRound(MapEvent mapEvent, BattleSideEnum side, float advantage)
		{
			MapEventAccessTools._SimulateBattleForRoundGetter.Invoke(mapEvent, new object[] { side, advantage });
		}

		// Token: 0x04000005 RID: 5
		private static readonly MethodInfo _SimulateBattleForRoundGetter = AccessTools.Method(typeof(MapEvent), "SimulateBattleForRound", null, null);
	}
}
