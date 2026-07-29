using System;
using System.Reflection;
using HarmonyLib;
using MissionSharedLibrary.Config;
using MissionSharedLibrary.Utilities;
using RTSCamera.CommandSystem.Config;
using RTSCamera.CommandSystem.Logic;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace RTSCamera.CommandSystem.Patch
{
	// Token: 0x0200005D RID: 93
	public class Patch_HumanAIComponent
	{
		// Token: 0x0600033F RID: 831 RVA: 0x0000F55C File Offset: 0x0000D75C
		public static bool Patch(Harmony harmony)
		{
			try
			{
				if (Patch_HumanAIComponent._patched)
				{
					return false;
				}
				Patch_HumanAIComponent._patched = true;
				harmony.Patch(typeof(HumanAIComponent).GetMethod("GetDesiredSpeedInFormation", BindingFlags.Instance | BindingFlags.Public), new HarmonyMethod(typeof(Patch_HumanAIComponent).GetMethod("Prefix_GetDesiredSpeedInFormation", BindingFlags.Static | BindingFlags.Public)), null, null, null);
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

		// Token: 0x06000340 RID: 832 RVA: 0x0000F5F8 File Offset: 0x0000D7F8
		public static bool Prefix_GetDesiredSpeedInFormation(HumanAIComponent __instance, Agent ___Agent, ref float __result, bool isCharging)
		{
			if (Mission.Current.IsNavalBattle || isCharging || MissionConfigBase<CommandSystemConfig>.Get().FormationSpeedSyncMode == FormationSpeedSyncMode.Disabled)
			{
				return true;
			}
			if (___Agent.Formation == null || ___Agent.Team == null || !___Agent.Team.IsPlayerTeam)
			{
				return true;
			}
			if (___Agent.Formation.Arrangement is ColumnFormation || !__instance.ShouldCatchUpWithFormation || isCharging || Mission.Current.IsMissionEnding)
			{
				return true;
			}
			OrderInQueue orderInQueue;
			if (!CommandQueueLogic.PendingOrders.TryGetValue(___Agent.Formation, out orderInQueue))
			{
				return true;
			}
			if (!orderInQueue.ShouldAdjustFormationSpeed || orderInQueue.FormationExpectedPositions.Count <= 1 || !orderInQueue.FormationExpectedPositions.ContainsKey(___Agent.Formation))
			{
				return true;
			}
			if (isCharging || ___Agent.IsDetachedFromFormation)
			{
				return true;
			}
			Agent mountAgent = ___Agent.MountAgent;
			float num = ((mountAgent != null) ? mountAgent.GetMaximumForwardUnlimitedSpeed() : ___Agent.GetMaximumForwardUnlimitedSpeed());
			if (!isCharging)
			{
				Vec2 vec = ___Agent.Formation.GetCurrentGlobalPositionOfUnit(___Agent, true);
				Vec2 asVec = ___Agent.Position.AsVec2;
				float num2 = MathF.Max(0.1f, ___Agent.Formation.CachedMovementSpeed);
				float num3 = num2;
				float num4 = 1f;
				WorldPosition orderPositionOfUnit = ___Agent.Formation.GetOrderPositionOfUnit(___Agent);
				if (orderPositionOfUnit.IsValid)
				{
					float num5 = orderPositionOfUnit.AsVec2.Distance(asVec);
					float num6 = orderInQueue.FormationTargetDistances[___Agent.Formation];
					switch (MissionConfigBase<CommandSystemConfig>.Get().FormationSpeedSyncMode)
					{
					case FormationSpeedSyncMode.Linear:
						num3 = MathF.Clamp((num5 + num4) / orderInQueue.MaxDuration, 0.1f, num2);
						break;
					case FormationSpeedSyncMode.CatchUp:
					{
						float num7 = MathF.Clamp((num5 + num4) / orderInQueue.MaxDuration, 0.1f, num2);
						num3 = MathF.Clamp(MathF.Lerp(num7, num2, (num6 - orderInQueue.DistanceWithMaxDuration + num4) / (num2 * 2f), 1E-05f), num7, num2);
						break;
					}
					case FormationSpeedSyncMode.WaitForLastFormation:
					{
						Vec2 vec2 = orderInQueue.FormationExpectedPositions[___Agent.Formation];
						vec = vec - ___Agent.Formation.CurrentPosition + vec2;
						float num8 = MathF.Clamp((num5 + num4) / orderInQueue.MaxDuration, 0.1f, num2);
						num3 = MathF.Clamp(MathF.Lerp(num8, num2, (num6 - orderInQueue.DistanceWithMaxDuration + num4) / (num2 * 2f), 1E-05f), num8, num2);
						break;
					}
					}
				}
				Vec2 vec3 = vec - asVec;
				float num9 = MathF.Clamp(-___Agent.GetMovementDirection().DotProduct(vec3), 0f, 100f);
				float num10 = ((___Agent.MountAgent != null) ? 4f : 2f);
				float num11 = num3 / num;
				float num12 = num2 / num3;
				float num13 = MathF.Clamp((float)(0.7 + 0.4 * (((double)num - (double)num9 * (double)num10) / MathF.Max(1.0, (double)num + (double)num9 * (double)num10))), 0f, num12);
				__result = MathF.Clamp(num13 * num11, 0.1f, 1f);
				return false;
			}
			return true;
		}

		// Token: 0x0400014E RID: 334
		private static bool _patched;
	}
}
