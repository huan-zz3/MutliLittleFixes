using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using FormationFilter.Logics;
using FormationFilter.Models;
using FormationFilter.Utilities;
using HarmonyLib;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle;

namespace FormationFilter.Patch
{
	// Token: 0x02000013 RID: 19
	[NullableContext(1)]
	[Nullable(0)]
	public class Patch_OrderOfBattleVM
	{
		// Token: 0x060000C4 RID: 196 RVA: 0x00005D9C File Offset: 0x00003F9C
		public static bool Patch(Harmony harmony)
		{
			try
			{
				if (Patch_OrderOfBattleVM._patched)
				{
					return false;
				}
				Patch_OrderOfBattleVM._patched = true;
				harmony.Patch(typeof(OrderOfBattleVM).GetMethod("OnHeroesChanged", BindingFlags.Instance | BindingFlags.NonPublic), new HarmonyMethod(typeof(Patch_OrderOfBattleVM).GetMethod("Prefix_OnHeroesChanged", BindingFlags.Static | BindingFlags.Public)), null, null, null);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				InformationManager.DisplayMessage(new InformationMessage("[FormationFilter] Failed to patch OrderOfBattleVM: " + ex.Message));
				MBDebug.Print(ex.ToString(), 0, 12, 17592186044416UL);
				return false;
			}
			return true;
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x00005E4C File Offset: 0x0000404C
		public static void Prefix_OnHeroesChanged(OrderOfBattleVM __instance, List<OrderOfBattleFormationItemVM> ____allFormations)
		{
			if (Mission.Current == null)
			{
				return;
			}
			FormationFilterLogic missionBehavior = Mission.Current.GetMissionBehavior<FormationFilterLogic>();
			TeamFilter teamFilter = ((missionBehavior != null) ? missionBehavior.GetTeamFilter(Mission.Current.PlayerTeam) : null);
			if (teamFilter == null || !teamFilter.IsLoaded)
			{
				return;
			}
			List<Agent> list = ____allFormations.SelectMany<OrderOfBattleFormationItemVM, Agent>((OrderOfBattleFormationItemVM vm) => Utility.GetExcludedAgents(vm)).ToList<Agent>();
			teamFilter.UpdateTotalAndActualUnitCountOfFilters(Mission.Current.PlayerTeam, list);
		}

		// Token: 0x04000052 RID: 82
		private static bool _patched;
	}
}
