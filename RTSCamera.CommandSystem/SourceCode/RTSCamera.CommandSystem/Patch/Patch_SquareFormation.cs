using System;
using System.Reflection;
using HarmonyLib;
using MissionSharedLibrary.Config;
using MissionSharedLibrary.Utilities;
using RTSCamera.CommandSystem.Config;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace RTSCamera.CommandSystem.Patch
{
	// Token: 0x02000067 RID: 103
	public class Patch_SquareFormation
	{
		// Token: 0x060003EF RID: 1007 RVA: 0x00017C54 File Offset: 0x00015E54
		public static bool Patch(Harmony harmony)
		{
			try
			{
				if (Patch_SquareFormation._patched)
				{
					return false;
				}
				Patch_SquareFormation._patched = true;
				harmony.Patch(typeof(SquareFormation).GetMethod("GetLocalDirectionOfUnit", BindingFlags.Instance | BindingFlags.NonPublic), new HarmonyMethod(typeof(Patch_SquareFormation).GetMethod("Prefix_GetLocalDirectionOfUnit", BindingFlags.Static | BindingFlags.Public)), null, null, null);
				MethodInfo methodInfo = AccessTools.Method(typeof(SquareFormation), "GetSideOfUnitPosition", new Type[]
				{
					typeof(int),
					typeof(int)
				}, null);
				Type returnType = methodInfo.ReturnType;
				Patch_SquareFormation._sideEnum = Nullable.GetUnderlyingType(returnType);
				Patch_SquareFormation._nullableCtor = returnType.GetConstructor(new Type[] { Patch_SquareFormation._sideEnum });
				harmony.Patch(methodInfo, new HarmonyMethod(typeof(Patch_SquareFormation).GetMethod("Prefix_GetSideOfUnitPosition", BindingFlags.Static | BindingFlags.Public)), null, null, null);
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

		// Token: 0x060003F0 RID: 1008 RVA: 0x00017D78 File Offset: 0x00015F78
		public static bool Prefix_GetLocalDirectionOfUnit(SquareFormation __instance, MBList2D<IFormationUnit> ____units2D, int fileIndex, int rankIndex, ref Vec2 __result)
		{
			if (!MissionConfigBase<CommandSystemConfig>.Get().SquareFormationCornerFix)
			{
				return true;
			}
			int num = Patch_SquareFormation.UnitCountOfOuterSide(____units2D);
			int num2 = Patch_SquareFormation.ShiftFileIndex(num, fileIndex);
			int num3 = (num2 - rankIndex) % (num - 1);
			int num4 = (num2 + rankIndex) % (num - 1);
			switch (Patch_SquareFormation.GetSideOfUnitPosition(num, num2))
			{
			case Patch_SquareFormation.Side.Front:
				if (num - 2 * rankIndex > 1 && (num3 == 0 || num4 == 0))
				{
					__result = (Vec2.Forward + -Vec2.Side).Normalized();
					return false;
				}
				__result = Vec2.Forward;
				return false;
			case Patch_SquareFormation.Side.Right:
				if (num3 == 0 || num4 == 0)
				{
					__result = (Vec2.Forward + Vec2.Side).Normalized();
					return false;
				}
				__result = Vec2.Side;
				return false;
			case Patch_SquareFormation.Side.Rear:
				if (num3 == 0 || num4 == 0)
				{
					__result = (-Vec2.Forward + Vec2.Side).Normalized();
					return false;
				}
				__result = -Vec2.Forward;
				return false;
			case Patch_SquareFormation.Side.Left:
				if (num3 == 0 || num4 == 0)
				{
					__result = (-Vec2.Forward + -Vec2.Side).Normalized();
					return false;
				}
				__result = -Vec2.Side;
				return false;
			default:
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\AI\\Formation\\SquareFormation.cs", "GetLocalDirectionOfUnit", 448);
				__result = Vec2.Forward;
				return false;
			}
		}

		// Token: 0x060003F1 RID: 1009 RVA: 0x00017EF8 File Offset: 0x000160F8
		private static int ShiftFileIndex(int unitCountOfOuterSide, int fileIndex)
		{
			int num = unitCountOfOuterSide + unitCountOfOuterSide / 2 - 2;
			int num2 = fileIndex - num;
			if (num2 < 0)
			{
				num2 += (unitCountOfOuterSide - 1) * 4;
			}
			return num2;
		}

		// Token: 0x060003F2 RID: 1010 RVA: 0x00017F1E File Offset: 0x0001611E
		private static int UnitCountOfOuterSide(MBList2D<IFormationUnit> ____units2D)
		{
			return MathF.Ceiling((float)____units2D.Count1 / 4f) + 1;
		}

		// Token: 0x060003F3 RID: 1011 RVA: 0x00017F34 File Offset: 0x00016134
		private static Patch_SquareFormation.Side GetSideOfUnitPosition(int unitCountOfOuterSide, int fileIndex)
		{
			return (Patch_SquareFormation.Side)(fileIndex / (unitCountOfOuterSide - 1));
		}

		// Token: 0x060003F4 RID: 1012 RVA: 0x00017F3C File Offset: 0x0001613C
		public static bool Prefix_GetSideOfUnitPosition(SquareFormation __instance, int fileIndex, int rankIndex, MBList2D<IFormationUnit> ____units2D, ref object __result)
		{
			int num = Patch_SquareFormation.UnitCountOfOuterSide(____units2D);
			Patch_SquareFormation.Side sideOfUnitPosition = Patch_SquareFormation.GetSideOfUnitPosition(num, fileIndex);
			if (rankIndex == 0)
			{
				__result = Patch_SquareFormation._nullableCtor.Invoke(new object[] { Enum.ToObject(Patch_SquareFormation._sideEnum, (int)sideOfUnitPosition) });
				return false;
			}
			int num2 = num - 2 * rankIndex;
			if (num2 == 1 && sideOfUnitPosition != Patch_SquareFormation.Side.Front)
			{
				__result = null;
				return false;
			}
			int num3 = fileIndex % (num - 1);
			if (num3 >= rankIndex && (num3 < num - rankIndex - 1 || (num3 == num - rankIndex - 1 && num2 == 1)))
			{
				__result = Patch_SquareFormation._nullableCtor.Invoke(new object[] { Enum.ToObject(Patch_SquareFormation._sideEnum, (int)sideOfUnitPosition) });
				return false;
			}
			__result = null;
			return false;
		}

		// Token: 0x0400019C RID: 412
		private static bool _patched;

		// Token: 0x0400019D RID: 413
		private static Type _sideEnum;

		// Token: 0x0400019E RID: 414
		private static ConstructorInfo _nullableCtor;

		// Token: 0x020000C0 RID: 192
		private enum Side
		{
			// Token: 0x04000329 RID: 809
			Front,
			// Token: 0x0400032A RID: 810
			Right,
			// Token: 0x0400032B RID: 811
			Rear,
			// Token: 0x0400032C RID: 812
			Left
		}
	}
}
