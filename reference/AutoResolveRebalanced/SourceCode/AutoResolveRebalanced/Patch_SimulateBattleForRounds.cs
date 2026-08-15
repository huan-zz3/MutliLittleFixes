using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.Core;

namespace AutoResolveRebalanced
{
	// Token: 0x02000004 RID: 4
	[HarmonyPatch(typeof(MapEvent), "SimulateBattleForRounds")]
	internal class Patch_SimulateBattleForRounds
	{
		// Token: 0x06000005 RID: 5 RVA: 0x00002160 File Offset: 0x00000360
		public static void Prefix(out ValueTuple<int, int> __state, int simulationRoundsDefender, int simulationRoundsAttacker)
		{
			__state.Item1 = simulationRoundsDefender;
			__state.Item2 = simulationRoundsAttacker;
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002170 File Offset: 0x00000370
		public static void Postfix(MapEvent __instance, int simulationRoundsDefender, int simulationRoundsAttacker, ValueTuple<int, int> __state)
		{
			try
			{
				if ((new Settings().aiEnabled || __instance.IsPlayerSimulation) && __instance.BattleState == null && __instance.AttackerSide.NumRemainingSimulationTroops > 0 && __instance.DefenderSide.NumRemainingSimulationTroops > 0)
				{
					int numRemainingSimulationTroops = __instance.DefenderSide.NumRemainingSimulationTroops;
					int numRemainingSimulationTroops2 = __instance.AttackerSide.NumRemainingSimulationTroops;
					int num = 0;
					if ((float)numRemainingSimulationTroops / (float)numRemainingSimulationTroops2 > 10f)
					{
						while (__instance.AttackerSide.NumRemainingSimulationTroops == numRemainingSimulationTroops2)
						{
							int num2 = __state.Item1 + 10;
							int num3 = __state.Item2;
							while (0 < num3 + num2 && __instance.BattleState == null)
							{
								float num4 = (float)num3 / (float)(num3 + num2);
								if (MBRandom.RandomFloat < num4)
								{
									num3--;
									MapEventAccessTools.InvokeSimulateBattleForRound(__instance, 1, 1f);
								}
								else
								{
									num2--;
									MapEventAccessTools.InvokeSimulateBattleForRound(__instance, 0, 1f);
								}
								num++;
								Debugger.Message(string.Concat(new string[]
								{
									"DefenderExtraRound",
									num.ToString(),
									" RND:",
									num2.ToString(),
									"vs",
									num3.ToString(),
									" TRP:",
									numRemainingSimulationTroops2.ToString(),
									"->",
									__instance.AttackerSide.NumRemainingSimulationTroops.ToString()
								}), Debugger.Type.Log, __instance, true);
							}
						}
					}
					if ((float)numRemainingSimulationTroops2 / (float)numRemainingSimulationTroops > 10f)
					{
						while (__instance.DefenderSide.NumRemainingSimulationTroops == numRemainingSimulationTroops)
						{
							int num2 = __state.Item1;
							int num3 = __state.Item2 + 10;
							while (0 < num3 + num2 && __instance.BattleState == null)
							{
								float num5 = (float)num3 / (float)(num3 + num2);
								if (MBRandom.RandomFloat < num5)
								{
									num3--;
									MapEventAccessTools.InvokeSimulateBattleForRound(__instance, 1, 1f);
								}
								else
								{
									num2--;
									MapEventAccessTools.InvokeSimulateBattleForRound(__instance, 0, 1f);
								}
								num++;
								Debugger.Message(string.Concat(new string[]
								{
									"AttackerExtraRound",
									num.ToString(),
									" RND:",
									num2.ToString(),
									"vs",
									num3.ToString(),
									" TRP:",
									numRemainingSimulationTroops.ToString(),
									"->",
									__instance.DefenderSide.NumRemainingSimulationTroops.ToString()
								}), Debugger.Type.Log, __instance, true);
							}
						}
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
