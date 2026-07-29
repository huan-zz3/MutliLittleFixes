using System;
using System.Reflection;
using HarmonyLib;
using MissionSharedLibrary.Utilities;
using RTSCamera.CommandSystem.QuerySystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace RTSCamera.CommandSystem.Patch
{
	// Token: 0x0200005A RID: 90
	public class Patch_FacingOrder
	{
		// Token: 0x0600031F RID: 799 RVA: 0x0000DB34 File Offset: 0x0000BD34
		public static bool Patch(Harmony harmony)
		{
			try
			{
				if (Patch_FacingOrder._patched)
				{
					return false;
				}
				Patch_FacingOrder._patched = true;
				harmony.Patch(typeof(FacingOrder).GetMethod("GetDirectionAux", BindingFlags.Instance | BindingFlags.NonPublic), new HarmonyMethod(typeof(Patch_FacingOrder).GetMethod("Prefix_GetDirectionAux", BindingFlags.Static | BindingFlags.Public)), null, null, null);
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

		// Token: 0x06000320 RID: 800 RVA: 0x0000DBD0 File Offset: 0x0000BDD0
		public static bool Prefix_GetDirectionAux(Formation f, Agent targetAgent, ref Vec2 __result, FacingOrder.FacingOrderEnum ___OrderEnum)
		{
			if (f.IsAIControlled)
			{
				return true;
			}
			Formation facingEnemyTargetFormation = Patch_OrderController.GetFacingEnemyTargetFormation(f);
			if (facingEnemyTargetFormation == null)
			{
				return true;
			}
			if (TroopClassExtensions.IsMounted(f.PhysicalClass) && targetAgent != null)
			{
				return true;
			}
			if (___OrderEnum == null)
			{
				return true;
			}
			if (f.Arrangement is CircularFormation || f.Arrangement is SquareFormation)
			{
				return true;
			}
			__result = Patch_FacingOrder.GetDirectionFacingToEnemyFormation(f, facingEnemyTargetFormation);
			return false;
		}

		// Token: 0x06000321 RID: 801 RVA: 0x0000DC34 File Offset: 0x0000BE34
		public static Vec2 GetDirectionFacingToEnemyFormation(Formation f, Formation target)
		{
			return Patch_FacingOrder.GetDirectionFacingToEnemyFormationAux(f, target, f.CurrentPosition, f.Direction, CommandQuerySystem.GetQueryForFormation(f).WeightedAverageFacingTargetEnemyPosition);
		}

		// Token: 0x06000322 RID: 802 RVA: 0x0000DC54 File Offset: 0x0000BE54
		public static Vec2 GetVirtualDirectionFacingToEnemyFormation(Formation f, Formation target)
		{
			return Patch_FacingOrder.GetDirectionFacingToEnemyFormationAux(f, target, Patch_OrderController.GetFormationVirtualPositionVec2(f), Patch_OrderController.GetFormationVirtualDirection(f), CommandQuerySystem.GetQueryForFormation(f).VirtualWeightedAverageFacingTargetEnemyPosition);
		}

		// Token: 0x06000323 RID: 803 RVA: 0x0000DC74 File Offset: 0x0000BE74
		private static Vec2 GetDirectionFacingToEnemyFormationAux(Formation f, Formation target, Vec2 currentPosition, Vec2 currentDirection, Vec2 averageEnemyPosition)
		{
			if (!averageEnemyPosition.IsValid)
			{
				return currentDirection;
			}
			Vec2 vec = (averageEnemyPosition - currentPosition).Normalized();
			float length = (averageEnemyPosition - currentPosition).Length;
			int countOfUnits = target.CountOfUnits;
			int countOfUnits2 = f.CountOfUnits;
			Vec2 vec2 = currentDirection;
			bool flag = (double)length >= (double)countOfUnits2 * 0.20000000298023224;
			if (countOfUnits == 0 || countOfUnits2 == 0)
			{
				flag = false;
			}
			float num = ((!flag) ? 1f : (MBMath.ClampFloat((float)countOfUnits2 * 1f / (float)countOfUnits, 0.33333334f, 3f) * MBMath.ClampFloat(length / (float)countOfUnits2, 0.33333334f, 3f)));
			if (flag && (double)MathF.Abs(vec.AngleBetween(vec2)) > 0.1745329350233078 * (double)num)
			{
				vec2 = vec;
			}
			return vec2;
		}

		// Token: 0x06000324 RID: 804 RVA: 0x0000DD44 File Offset: 0x0000BF44
		public static Vec2 GetDirectionFacingToEnemy(Formation f, Vec2 currentPosition, Vec2 currentDirection, Vec2 averageEnemyPosition)
		{
			if (!averageEnemyPosition.IsValid)
			{
				return currentDirection;
			}
			Vec2 vec = (averageEnemyPosition - currentPosition).Normalized();
			float length = (averageEnemyPosition - currentPosition).Length;
			int enemyUnitCount = f.QuerySystem.Team.EnemyUnitCount;
			int countOfUnits = f.CountOfUnits;
			Vec2 vec2 = currentDirection;
			bool flag = (double)length >= (double)countOfUnits * 0.20000000298023224;
			if (enemyUnitCount == 0 || countOfUnits == 0)
			{
				flag = false;
			}
			float num = ((!flag) ? 1f : (MBMath.ClampFloat((float)countOfUnits * 1f / (float)enemyUnitCount, 0.33333334f, 3f) * MBMath.ClampFloat(length / (float)countOfUnits, 0.33333334f, 3f)));
			if (flag && (double)MathF.Abs(vec.AngleBetween(vec2)) > 0.1745329350233078 * (double)num)
			{
				vec2 = vec;
			}
			return vec2;
		}

		// Token: 0x04000144 RID: 324
		private static bool _patched;
	}
}
