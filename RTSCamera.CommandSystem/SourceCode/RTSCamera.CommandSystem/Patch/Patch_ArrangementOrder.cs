using System;
using System.Reflection;
using HarmonyLib;
using MissionSharedLibrary.Config;
using MissionSharedLibrary.Utilities;
using RTSCamera.CommandSystem.Config;
using RTSCamera.CommandSystem.Utilities;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;

namespace RTSCamera.CommandSystem.Patch
{
	// Token: 0x02000058 RID: 88
	public class Patch_ArrangementOrder
	{
		// Token: 0x06000316 RID: 790 RVA: 0x0000D7A4 File Offset: 0x0000B9A4
		public static bool Patch(Harmony harmony)
		{
			try
			{
				if (Patch_ArrangementOrder._patched)
				{
					return false;
				}
				Patch_ArrangementOrder._patched = true;
				harmony.Patch(typeof(ArrangementOrder).GetMethod("GetArrangement", BindingFlags.Instance | BindingFlags.Public), new HarmonyMethod(typeof(Patch_ArrangementOrder).GetMethod("Prefix_GetArrangement", BindingFlags.Static | BindingFlags.Public)), null, null, null);
				harmony.Patch(typeof(ArrangementOrder).GetMethod("OnApply", BindingFlags.Instance | BindingFlags.Public), new HarmonyMethod(typeof(Patch_ArrangementOrder).GetMethod("Prefix_OnApply", BindingFlags.Static | BindingFlags.Public)), null, null, null);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				MissionSharedLibrary.Utilities.Utility.DisplayMessage(ex.ToString());
				MBDebug.Print(ex.ToString(), 0, 12, 17592186044416UL);
				return false;
			}
			return true;
		}

		// Token: 0x06000317 RID: 791 RVA: 0x0000D87C File Offset: 0x0000BA7C
		public static bool Prefix_GetArrangement(Formation formation, ArrangementOrder __instance, ref IFormationArrangement __result)
		{
			if (__instance.OrderEnum == 7 && MissionConfigBase<CommandSystemConfig>.Get().HollowSquare)
			{
				bool flag = RTSCamera.CommandSystem.Utilities.Utility.ShouldEnableHollowSquareFormationFor(formation);
				bool flag2 = formation.Team == null;
				bool isAIControlled = formation.IsAIControlled;
				if (flag || flag2)
				{
					__result = new SquareFormation(formation);
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000318 RID: 792 RVA: 0x0000D8C4 File Offset: 0x0000BAC4
		public unsafe static bool Prefix_OnApply(ArrangementOrder __instance, Formation formation)
		{
			int unitSpacing = formation.UnitSpacing;
			int unitSpacing2 = __instance.GetUnitSpacing();
			if (RTSCamera.CommandSystem.Utilities.Utility.ShouldEnablePlayerOrderControllerPatchForFormation(formation) && formation.Arrangement.GetType() != RTSCamera.CommandSystem.Utilities.Utility.GetTypeOfArrangement(__instance.OrderEnum, RTSCamera.CommandSystem.Utilities.Utility.ShouldEnableHollowSquareFormationFor(formation)))
			{
				Patch_ArrangementOrder._formOrder.SetValue(formation, FormOrder.FormOrderCustom(Patch_OrderController.GetFormationVirtualWidth(formation) ?? formation.Width));
				PropertyInfo unitSpacing3 = Patch_ArrangementOrder._unitSpacing;
				object formation2 = formation;
				int? formationVirtualUnitSpacing = Patch_OrderController.GetFormationVirtualUnitSpacing(formation);
				unitSpacing3.SetValue(formation2, formationVirtualUnitSpacing.GetValueOrDefault(unitSpacing2));
			}
			else
			{
				Formation formation3 = formation;
				int? formationVirtualUnitSpacing = new int?(unitSpacing2);
				formation3.SetPositioning(null, null, formationVirtualUnitSpacing);
			}
			__instance.Rearrange(formation);
			if (__instance.OrderEnum == 4)
			{
				__instance.TickOccasionally(formation);
				formation.ResetArrangementOrderTickTimer();
			}
			ArrangementOrder.ArrangementOrderEnum orderEnum = __instance.OrderEnum;
			formation.ApplyActionOnEachUnit(delegate(Agent agent)
			{
				if (agent.IsAIControlled)
				{
					Agent.UsageDirection shieldDirectionOfUnit = ArrangementOrder.GetShieldDirectionOfUnit(formation, agent, orderEnum);
					agent.EnforceShieldUsage(shieldDirectionOfUnit);
				}
				agent.UpdateAgentProperties();
				MovementOrder movementOrder = *formation.GetReadonlyMovementOrderReference();
				MovementOrder.MovementOrderEnum movementOrderEnum = movementOrder.OrderEnum;
				if (movementOrderEnum - 2 <= 1 && movementOrder.GetPosition(formation).IsValid)
				{
					movementOrderEnum = 7;
				}
				AgentComponentExtensions.RefreshBehaviorValues(agent, movementOrderEnum, orderEnum);
			}, null);
			return false;
		}

		// Token: 0x0400013F RID: 319
		private static bool _patched;

		// Token: 0x04000140 RID: 320
		private static PropertyInfo _formOrder = AccessTools.Property(typeof(Formation), "FormOrder");

		// Token: 0x04000141 RID: 321
		private static PropertyInfo _unitSpacing = AccessTools.Property(typeof(Formation), "UnitSpacing");
	}
}
