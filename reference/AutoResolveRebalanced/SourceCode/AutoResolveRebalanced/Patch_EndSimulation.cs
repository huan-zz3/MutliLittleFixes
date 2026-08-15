using System;
using System.Collections.Generic;
using HarmonyLib;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.Core;

namespace AutoResolveRebalanced
{
	// Token: 0x02000003 RID: 3
	[HarmonyPatch(typeof(MapEventSide), "EndSimulation")]
	internal class Patch_EndSimulation
	{
		// Token: 0x06000003 RID: 3 RVA: 0x000020C8 File Offset: 0x000002C8
		public static bool Prefix(MapEventSide __instance, ref List<UniqueTroopDescriptor> ____simulationTroopList)
		{
			Settings settings = new Settings();
			try
			{
				if (settings.aiEnabled || __instance.MapEvent.IsPlayerSimulation)
				{
					SimulateData simulateData;
					if (SimulateDataDict.GetData(__instance, out simulateData))
					{
						simulateData.StoreTroopNumber(____simulationTroopList.Count);
						simulateData.StoreHitPointAverage();
						simulateData.Clear(false);
					}
					else if (__instance.MapEvent.BattleState == null)
					{
						Debugger.Message("Data not found at EndSimulation", Debugger.Type.Warn, __instance.MapEvent, false);
					}
				}
			}
			catch (Exception ex)
			{
				Debugger.Message(ex.ToString(), Debugger.Type.Exception, null, false);
			}
			return true;
		}
	}
}
