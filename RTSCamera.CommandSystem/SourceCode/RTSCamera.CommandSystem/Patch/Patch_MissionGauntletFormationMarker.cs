using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using MissionSharedLibrary.Utilities;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.GauntletUI.Mission.Singleplayer;
using TaleWorlds.MountAndBlade.ViewModelCollection.HUD.FormationMarker;

namespace RTSCamera.CommandSystem.Patch
{
	// Token: 0x0200005E RID: 94
	public class Patch_MissionGauntletFormationMarker
	{
		// Token: 0x06000342 RID: 834 RVA: 0x0000F920 File Offset: 0x0000DB20
		public static bool Patch(Harmony harmony)
		{
			try
			{
				if (Patch_MissionGauntletFormationMarker._patched)
				{
					return false;
				}
				Patch_MissionGauntletFormationMarker._patched = true;
				harmony.Patch(typeof(MissionGauntletFormationMarker).GetMethod("RefreshTargetProperties", BindingFlags.Instance | BindingFlags.NonPublic), new HarmonyMethod(typeof(Patch_MissionGauntletFormationMarker).GetMethod("Prefix_RefreshTargetProperties", BindingFlags.Static | BindingFlags.Public)), null, null, null);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				Utility.DisplayMessage(ex.ToString());
				MBDebug.Print(ex.ToString(), 0, 12, 17592186044416UL);
				return false;
			}
			return true;
		}

		// Token: 0x06000343 RID: 835 RVA: 0x0000F9BC File Offset: 0x0000DBBC
		public unsafe static bool Prefix_RefreshTargetProperties(MissionGauntletFormationMarker __instance, MissionFormationMarkerVM ____dataSource, MBReadOnlyList<Formation> ____focusedFormationsCache)
		{
			if (!____dataSource.IsFormationTargetRelevant)
			{
				for (int i = 0; i < ____dataSource.Targets.Count; i++)
				{
					____dataSource.Targets[i].SetTargetedState(false, false);
				}
			}
			else
			{
				List<Formation> list = new List<Formation>();
				Agent main = Agent.Main;
				MBReadOnlyList<Formation> mbreadOnlyList;
				if (main == null)
				{
					mbreadOnlyList = null;
				}
				else
				{
					OrderController playerOrderController = main.Team.PlayerOrderController;
					mbreadOnlyList = ((playerOrderController != null) ? playerOrderController.SelectedFormations : null);
				}
				MBReadOnlyList<Formation> mbreadOnlyList2 = mbreadOnlyList;
				if (mbreadOnlyList2 != null)
				{
					for (int j = 0; j < mbreadOnlyList2.Count; j++)
					{
						if (mbreadOnlyList2[j].TargetFormation != null)
						{
							MovementOrder movementOrder = *mbreadOnlyList2[j].GetReadonlyMovementOrderReference();
							if (movementOrder.OrderType == 4 || movementOrder.OrderType == 12 || movementOrder.OrderType == 5)
							{
								list.Add(mbreadOnlyList2[j].TargetFormation);
							}
						}
					}
				}
				for (int k = 0; k < ____dataSource.Targets.Count; k++)
				{
					MissionFormationMarkerTargetVM missionFormationMarkerTargetVM = ____dataSource.Targets[k];
					if (missionFormationMarkerTargetVM.TeamType == 2)
					{
						bool flag = list.Contains(missionFormationMarkerTargetVM.Formation);
						bool flag2 = ____focusedFormationsCache != null && ____focusedFormationsCache.Contains(missionFormationMarkerTargetVM.Formation);
						missionFormationMarkerTargetVM.SetTargetedState(flag2, flag);
					}
				}
			}
			return false;
		}

		// Token: 0x0400014F RID: 335
		private static bool _patched;
	}
}
