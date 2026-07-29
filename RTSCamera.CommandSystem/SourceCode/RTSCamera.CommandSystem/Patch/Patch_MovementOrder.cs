using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MissionSharedLibrary.Config;
using MissionSharedLibrary.Utilities;
using RTSCamera.CommandSystem.Config;
using RTSCamera.CommandSystem.QuerySystem;
using RTSCamera.CommandSystem.Utilities;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace RTSCamera.CommandSystem.Patch
{
	// Token: 0x02000061 RID: 97
	public class Patch_MovementOrder
	{
		// Token: 0x06000352 RID: 850 RVA: 0x0000FEBC File Offset: 0x0000E0BC
		public static bool Patch()
		{
			bool flag;
			try
			{
				if (Patch_MovementOrder._patched)
				{
					flag = false;
				}
				else
				{
					Patch_MovementOrder._patched = true;
					Patch_MovementOrder.Harmony.Patch(typeof(MovementOrder).GetMethod("GetSubstituteOrder", BindingFlags.Instance | BindingFlags.Public), new HarmonyMethod(typeof(Patch_MovementOrder).GetMethod("Prefix_GetSubstituteOrder", BindingFlags.Static | BindingFlags.Public), 800, null, null, null), null, null, null);
					Patch_MovementOrder.Harmony.Patch(typeof(MovementOrder).GetMethod("GetPositionAux", BindingFlags.Instance | BindingFlags.NonPublic), new HarmonyMethod(typeof(Patch_MovementOrder).GetMethod("Prefix_GetPositionAux", BindingFlags.Static | BindingFlags.Public)), null, null, null);
					flag = true;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				MBDebug.Print(ex.ToString(), 0, 12, 17592186044416UL);
				MissionSharedLibrary.Utilities.Utility.DisplayMessage(ex.ToString());
				flag = false;
			}
			return flag;
		}

		// Token: 0x06000353 RID: 851 RVA: 0x0000FFAC File Offset: 0x0000E1AC
		public static bool Prefix_GetSubstituteOrder(MovementOrder __instance, ref MovementOrder __result, Formation formation)
		{
			if (Mission.Current.IsNavalBattle)
			{
				return true;
			}
			if (__instance.OrderType == 5 && formation.TargetFormation != null && MissionConfigBase<CommandSystemConfig>.Get().AttackSpecificFormation && !CommandSystemSubModule.IsRealisticBattleModuleInstalled && !formation.IsAIControlled && MissionConfigBase<CommandSystemConfig>.Get().BehaviorAfterCharge == BehaviorAfterCharge.Hold)
			{
				WorldPosition cachedMedianPosition = formation.CachedMedianPosition;
				cachedMedianPosition.SetVec2(formation.CurrentPosition);
				if (formation.Team == Mission.Current.PlayerTeam && formation.PlayerOwner == Agent.Main)
				{
					RTSCamera.CommandSystem.Utilities.Utility.DisplayFormationReadyMessage(formation);
				}
				__result = MovementOrder.MovementOrderMove(cachedMedianPosition);
				return false;
			}
			return true;
		}

		// Token: 0x06000354 RID: 852 RVA: 0x0001004C File Offset: 0x0000E24C
		public static bool Prefix_GetPositionAux(MovementOrder __instance, Formation f, WorldPosition.WorldPositionEnforcedCache worldPositionEnforcedCache, ref WorldPosition __result, ref WorldPosition ____engageTargetPositionCache, ref float ____engageTargetPositionOffset)
		{
			if (Mission.Current.IsNavalBattle)
			{
				return true;
			}
			FormationQuerySystem querySystem = f.QuerySystem;
			bool flag = querySystem.IsRangedFormation || querySystem.IsRangedCavalryFormation;
			if (__instance.OrderEnum != 10)
			{
				return true;
			}
			if (Mission.Current.Mode == 6)
			{
				return true;
			}
			Vec2 vec = f.Direction;
			Formation targetFormation = f.TargetFormation;
			FormationQuerySystem formationQuerySystem = ((targetFormation != null) ? targetFormation.QuerySystem : null) ?? f.CachedClosestEnemyFormation;
			CommandFormationQuerySystem queryForFormation = CommandQuerySystem.GetQueryForFormation(f);
			WorldPosition worldPosition;
			if (formationQuerySystem == null)
			{
				Agent closestEnemyAgent = querySystem.ClosestEnemyAgent;
				if (closestEnemyAgent == null)
				{
					__result = f.CreateNewOrderWorldPosition(worldPositionEnforcedCache);
					return false;
				}
				worldPosition = closestEnemyAgent.GetWorldPosition();
			}
			else
			{
				worldPosition = formationQuerySystem.Formation.CachedMedianPosition;
			}
			WorldPosition worldPosition2 = worldPosition;
			if (flag || (MissionConfigBase<CommandSystemConfig>.Get().FixAdvaneOrderForThrowing && querySystem.HasThrowingUnitRatio > MissionConfigBase<CommandSystemConfig>.Get().ThrowerRatioThreshold && queryForFormation.RatioOfRemainingAmmo > MissionConfigBase<CommandSystemConfig>.Get().RemainingAmmoRatioThreshold && f.FiringOrder.OrderType != 31 && (MissionConfigBase<CommandSystemConfig>.Get().ApplyAdvanceOrderFixForAI || !f.IsAIControlled)))
			{
				float num = f.CurrentPosition.DistanceSquared(worldPosition2.AsVec2);
				float num2 = MathF.Pow(MathF.Max(queryForFormation.RatioOfRemainingAmmo - MissionConfigBase<CommandSystemConfig>.Get().RemainingAmmoRatioThreshold, 0f), 0.2f);
				if (!MissionConfigBase<CommandSystemConfig>.Get().ShortenRangeBasedOnRemainingAmmo || flag)
				{
					num2 = 1f;
				}
				float averageMissileRangeAdjusted = queryForFormation.AverageMissileRangeAdjusted;
				float num3 = (flag ? 1f : MathF.Pow(MathF.Clamp(num / MathF.Max(averageMissileRangeAdjusted * averageMissileRangeAdjusted, 1f) * 1.5f, 0f, 1f), 0.1f));
				vec = Patch_MovementOrder.GetDirectionAux(__instance, f);
				worldPosition2.SetVec2(worldPosition2.AsVec2 - vec * averageMissileRangeAdjusted * num2 * num3);
			}
			else if (formationQuerySystem != null)
			{
				vec = (formationQuerySystem.Formation.CachedAveragePosition - f.CachedAveragePosition).Normalized();
				float num4 = 2f;
				if ((double)formationQuerySystem.FormationPower < (double)f.QuerySystem.FormationPower * 0.2)
				{
					num4 = 0.1f;
				}
				worldPosition2.SetVec2(worldPosition2.AsVec2 - vec * num4);
			}
			Vec2 direction = f.Direction;
			Vec2 vec2;
			vec2..ctor(-direction.y, direction.x);
			float width = f.Width;
			Vec2 vec3 = worldPosition2.AsVec2 + vec2 * width / 2f;
			Vec2 vec4 = worldPosition2.AsVec2 - vec2 * width / 2f;
			Vec2 vec5 = vec3 - direction * f.Depth;
			Vec2 vec6 = vec4 - direction * f.Depth;
			worldPosition2 = Patch_MovementOrder.AdjustOutOfBoundaryPositions(worldPosition2, vec3);
			worldPosition2 = Patch_MovementOrder.AdjustOutOfBoundaryPositions(worldPosition2, vec4);
			worldPosition2 = Patch_MovementOrder.AdjustOutOfBoundaryPositions(worldPosition2, vec5);
			worldPosition2 = Patch_MovementOrder.AdjustOutOfBoundaryPositions(worldPosition2, vec6);
			if (!____engageTargetPositionCache.IsValid)
			{
				____engageTargetPositionCache = worldPosition2;
			}
			float num5 = (float)((double)f.QuerySystem.MovementSpeedMaximum * (double)f.QuerySystem.MovementSpeedMaximum * 9.0) * f.Depth;
			bool flag2 = (double)(____engageTargetPositionCache.AsVec2 + vec * ____engageTargetPositionOffset).DistanceSquared(worldPosition2.AsVec2) > (double)f.CurrentPosition.DistanceSquared(____engageTargetPositionCache.AsVec2) * 0.10000000149011612;
			bool flag3 = (double)worldPosition2.AsVec2.DistanceSquared(f.CurrentPosition) <= (double)num5;
			if (flag2 || flag3)
			{
				____engageTargetPositionCache = worldPosition2;
				____engageTargetPositionOffset = 0f;
			}
			WorldPosition worldPosition3 = ____engageTargetPositionCache;
			bool flag4;
			if ((double)worldPosition3.AsVec2.DistanceSquared(f.CurrentPosition) > (double)num5)
			{
				LineFormation lineFormation = f.Arrangement as LineFormation;
				if (lineFormation != null)
				{
					flag4 = (double)lineFormation.GetUnavailableUnitPositions().Count<Vec2>() > (double)lineFormation.UnitCount * 0.03;
					goto IL_0427;
				}
			}
			flag4 = false;
			IL_0427:
			if (flag4 || worldPosition3.GetNavMesh() == UIntPtr.Zero)
			{
				WorldPosition worldPosition4 = worldPosition3;
				worldPosition4.SetVec2(worldPosition4.AsVec2 - vec * 10f);
				if (worldPosition4.GetNavMesh() == UIntPtr.Zero)
				{
					worldPosition4 = Mission.Current.GetStraightPathToTarget(worldPosition4.AsVec2, worldPosition, 1f, true);
				}
				float num6 = (worldPosition3.AsVec2 - worldPosition4.AsVec2).DotProduct(vec);
				worldPosition3 = worldPosition4;
				____engageTargetPositionOffset += num6;
			}
			____engageTargetPositionCache = worldPosition3;
			__result = worldPosition3;
			return false;
		}

		// Token: 0x06000355 RID: 853 RVA: 0x00010524 File Offset: 0x0000E724
		private static WorldPosition AdjustOutOfBoundaryPositions(WorldPosition orderPosition, Vec2 position)
		{
			if (!Mission.Current.IsPositionInsideBoundaries(position))
			{
				Vec2 vec = Mission.Current.GetClosestBoundaryPosition(position) - position;
				orderPosition.SetVec2(orderPosition.AsVec2 + vec);
			}
			return orderPosition;
		}

		// Token: 0x06000356 RID: 854 RVA: 0x00010568 File Offset: 0x0000E768
		public static Vec2 GetDirectionAux(MovementOrder __instance, Formation f)
		{
			MovementOrder.MovementOrderEnum orderEnum = __instance.OrderEnum;
			if (orderEnum - 10 > 1)
			{
				Debug.FailedAssert("false", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\Orders\\MovementOrder.cs", "GetDirectionAux", 1789);
				return Vec2.One;
			}
			Formation targetFormation = f.TargetFormation;
			FormationQuerySystem formationQuerySystem = ((targetFormation != null) ? targetFormation.QuerySystem : null) ?? f.CachedClosestEnemyFormation;
			if (formationQuerySystem == null)
			{
				return Vec2.One;
			}
			return (formationQuerySystem.Formation.CachedMedianPosition.AsVec2 - f.CachedAveragePosition).Normalized();
		}

		// Token: 0x04000155 RID: 341
		private static readonly Harmony Harmony = new Harmony("RTSCommandPatchMovementOrder");

		// Token: 0x04000156 RID: 342
		private static bool _patched;
	}
}
