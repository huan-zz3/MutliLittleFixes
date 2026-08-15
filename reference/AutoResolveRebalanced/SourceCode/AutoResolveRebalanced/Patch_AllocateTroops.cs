using System;
using System.Collections.Generic;
using HarmonyLib;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.Core;

namespace AutoResolveRebalanced
{
	// Token: 0x02000002 RID: 2
	[HarmonyPatch(typeof(MapEventSide), "AllocateTroops")]
	internal class Patch_AllocateTroops
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002048 File Offset: 0x00000248
		public static void Postfix(ref List<UniqueTroopDescriptor> troopsList, MapEventSide __instance)
		{
			Settings settings = new Settings();
			try
			{
				if (settings.aiEnabled || __instance.MapEvent.IsPlayerSimulation)
				{
					SimulateData simulateData;
					if (SimulateDataDict.GetData(__instance, out simulateData))
					{
						simulateData.Clear(false);
						simulateData.UpdateDict(__instance, troopsList);
					}
					else
					{
						simulateData = new SimulateData(__instance, troopsList);
						SimulateDataDict.AddData(__instance, simulateData);
					}
				}
			}
			catch (Exception ex)
			{
				Debugger.Message(ex.ToString(), Debugger.Type.Exception, null, false);
			}
		}
	}
}
