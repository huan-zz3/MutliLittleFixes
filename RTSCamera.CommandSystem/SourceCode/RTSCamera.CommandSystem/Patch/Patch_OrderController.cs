using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using HarmonyLib;
using MissionSharedLibrary.Config;
using MissionSharedLibrary.Utilities;
using RTSCamera.CommandSystem.Config;
using RTSCamera.CommandSystem.Logic;
using RTSCamera.CommandSystem.QuerySystem;
using RTSCamera.CommandSystem.Utilities;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace RTSCamera.CommandSystem.Patch
{
	// Token: 0x02000062 RID: 98
	public class Patch_OrderController
	{
		// Token: 0x06000359 RID: 857 RVA: 0x00010608 File Offset: 0x0000E808
		public static bool Patch(Harmony harmony)
		{
			try
			{
				if (Patch_OrderController._patched)
				{
					return false;
				}
				Patch_OrderController._patched = true;
				harmony.Patch(typeof(OrderController).GetMethod("MoveToLineSegment", BindingFlags.Instance | BindingFlags.NonPublic), null, null, new HarmonyMethod(typeof(Patch_OrderController).GetMethod("Transpiler_MoveToLineSegment", BindingFlags.Static | BindingFlags.Public)), null);
				harmony.Patch(typeof(OrderController).GetMethod("SimulateNewOrderWithPositionAndDirectionAux", BindingFlags.Static | BindingFlags.NonPublic), new HarmonyMethod(typeof(Patch_OrderController).GetMethod("Prefix_SimulateNewOrderWithPositionAndDirectionAux", BindingFlags.Static | BindingFlags.Public)), null, null, null);
				harmony.Patch(typeof(OrderController).GetMethod("GetOrderLookAtDirection", BindingFlags.Static | BindingFlags.Public), new HarmonyMethod(typeof(Patch_OrderController).GetMethod("Prefix_GetOrderLookAtDirection", BindingFlags.Static | BindingFlags.Public)), null, null, null);
				harmony.Patch(typeof(OrderController).GetMethod("SimulateNewFacingOrder", BindingFlags.Static | BindingFlags.NonPublic), new HarmonyMethod(typeof(Patch_OrderController).GetMethod("Prefix_SimulateNewFacingOrder", BindingFlags.Static | BindingFlags.Public)), null, null, null);
				harmony.Patch(typeof(OrderController).GetMethod("SetOrderWithPosition", BindingFlags.Instance | BindingFlags.Public), null, null, new HarmonyMethod(typeof(Patch_OrderController).GetMethod("Transpile_SetOrderWithPosition", BindingFlags.Static | BindingFlags.Public)), null);
				harmony.Patch(typeof(OrderController).GetMethod("SetOrderWithPosition", BindingFlags.Instance | BindingFlags.Public), new HarmonyMethod(typeof(Patch_OrderController).GetMethod("Prefix_SetOrderWithPosition", BindingFlags.Static | BindingFlags.Public)), null, null, null);
				harmony.Patch(typeof(OrderController).GetMethod("GetActiveMovementOrderOf", BindingFlags.Static | BindingFlags.Public), new HarmonyMethod(typeof(Patch_OrderController).GetMethod("Prefix_GetActiveMovementOrderOf", BindingFlags.Static | BindingFlags.Public)), null, null, null);
				harmony.Patch(typeof(OrderController).GetMethod("GetActiveFacingOrderOf", BindingFlags.Static | BindingFlags.Public), new HarmonyMethod(typeof(Patch_OrderController).GetMethod("Prefix_GetActiveFacingOrderOf", BindingFlags.Static | BindingFlags.Public)), null, null, null);
				harmony.Patch(typeof(OrderController).GetMethod("GetActiveFiringOrderOf", BindingFlags.Static | BindingFlags.Public), new HarmonyMethod(typeof(Patch_OrderController).GetMethod("Prefix_GetActiveFiringOrderOf", BindingFlags.Static | BindingFlags.Public)), null, null, null);
				harmony.Patch(typeof(OrderController).GetMethod("GetActiveRidingOrderOf", BindingFlags.Static | BindingFlags.Public), new HarmonyMethod(typeof(Patch_OrderController).GetMethod("Prefix_GetActiveRidingOrderOf", BindingFlags.Static | BindingFlags.Public)), null, null, null);
				harmony.Patch(typeof(OrderController).GetMethod("GetActiveArrangementOrderOf", BindingFlags.Static | BindingFlags.Public), new HarmonyMethod(typeof(Patch_OrderController).GetMethod("Prefix_GetActiveArrangementOrderOf", BindingFlags.Static | BindingFlags.Public)), null, null, null);
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

		// Token: 0x0600035A RID: 858 RVA: 0x00010900 File Offset: 0x0000EB00
		public static void OnAfterMissionCreated()
		{
			Patch_OrderController._naturalUnitSpacings = new Dictionary<Formation, int>();
			Patch_OrderController._customUnitSpacings = new Dictionary<Formation, int>();
			Patch_OrderController._widthsBackup = new Dictionary<Formation, float>();
			Patch_OrderController.LivePreviewFormationChanges = new FormationChanges();
			Patch_OrderController._currentMovingTarget = new Dictionary<Formation, Patch_OrderController.MovingTarget>();
			Patch_OrderController.FacingEnemeyTarget = new Dictionary<Formation, Formation>();
		}

		// Token: 0x0600035B RID: 859 RVA: 0x0001093E File Offset: 0x0000EB3E
		public static void OnRemoveBehavior()
		{
			Patch_OrderController._naturalUnitSpacings = null;
			Patch_OrderController._customUnitSpacings = null;
			Patch_OrderController._widthsBackup = null;
			Patch_OrderController.LivePreviewFormationChanges = null;
			Patch_OrderController._currentMovingTarget = null;
			Patch_OrderController.FacingEnemeyTarget = null;
		}

		// Token: 0x0600035C RID: 860 RVA: 0x00010964 File Offset: 0x0000EB64
		public static void OnAddTeam(Team team)
		{
			if (team.FormationsIncludingSpecialAndEmpty == null)
			{
				return;
			}
			foreach (Formation formation in team.FormationsIncludingEmpty)
			{
				formation.OnAfterArrangementOrderApplied += Patch_OrderController.Formation_OnAfterArrangementOrderApplied;
			}
		}

		// Token: 0x0600035D RID: 861 RVA: 0x000109CC File Offset: 0x0000EBCC
		private static void Formation_OnAfterArrangementOrderApplied(Formation formation, ArrangementOrder.ArrangementOrderEnum arrangementOrder)
		{
			Patch_OrderController._naturalUnitSpacings[formation] = ArrangementOrder.GetUnitSpacingOf(arrangementOrder);
			Patch_OrderController._customUnitSpacings.Remove(formation);
			Patch_OrderController._widthsBackup.Remove(formation);
		}

		// Token: 0x0600035E RID: 862 RVA: 0x000109F7 File Offset: 0x0000EBF7
		public static IEnumerable<CodeInstruction> Transpiler_MoveToLineSegment(IEnumerable<CodeInstruction> instructions)
		{
			List<CodeInstruction> list = new List<CodeInstruction>(instructions);
			Patch_OrderController.FixNoLineShortFormationDirection(list);
			Patch_OrderController.FixLineShortFacingOrder(list);
			return list.AsEnumerable<CodeInstruction>();
		}

		// Token: 0x0600035F RID: 863 RVA: 0x00010A10 File Offset: 0x0000EC10
		private static void FixNoLineShortFormationDirection(List<CodeInstruction> codes)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			int num = -1;
			for (int i = 0; i < codes.Count; i++)
			{
				if (!flag)
				{
					if (codes[i].opcode == OpCodes.Call)
					{
						MethodInfo methodInfo = codes[i].operand as MethodInfo;
						if (methodInfo != null && methodInfo.Name == "GetActiveFacingOrderOf")
						{
							flag = true;
						}
					}
				}
				else if (!flag2)
				{
					if (codes[i].opcode == OpCodes.Ldc_I4_S && (sbyte)codes[i].operand == 15)
					{
						flag2 = true;
					}
				}
				else if (!flag3)
				{
					if (codes[i].opcode == OpCodes.Callvirt && (codes[i].operand as MethodInfo).Name == "SetMovementOrder")
					{
						flag3 = true;
						num = i;
					}
				}
				else if (!flag4 && codes[i].opcode == OpCodes.Callvirt && (codes[i].operand as MethodInfo).Name == "SetFacingOrder")
				{
					flag4 = true;
					break;
				}
			}
			if (flag3 && flag4)
			{
				codes[num + 2].opcode = OpCodes.Ldloc_S;
				codes[num + 2].operand = 13;
				codes[num + 3].opcode = OpCodes.Nop;
				return;
			}
			throw new Exception("SetMovementOrder or SetFacingOrder not found");
		}

		// Token: 0x06000360 RID: 864 RVA: 0x00010BB8 File Offset: 0x0000EDB8
		private static void FixLineShortFacingOrder(List<CodeInstruction> codes)
		{
			bool flag = false;
			bool flag2 = false;
			int num = -1;
			int num2 = -1;
			for (int i = 0; i < codes.Count; i++)
			{
				if (!flag)
				{
					if (codes[i].opcode == OpCodes.Call)
					{
						MethodInfo methodInfo = codes[i].operand as MethodInfo;
						if (methodInfo != null && methodInfo.Name == "GetActiveFacingOrderOf")
						{
							flag = true;
							num = i;
						}
					}
				}
				else if (!flag2 && codes[i].opcode == OpCodes.Callvirt && (codes[i].operand as MethodInfo).Name == "SetMovementOrder")
				{
					flag2 = true;
					num2 = i;
				}
			}
			if (flag && flag2)
			{
				codes[num - 1].opcode = OpCodes.Ldloc_S;
				codes[num - 1].operand = codes[num2 - 3].operand;
				return;
			}
			throw new Exception("GetActiveFacingOrderOf or SetMovementOrder not found");
		}

		// Token: 0x06000361 RID: 865 RVA: 0x00010CC0 File Offset: 0x0000EEC0
		public static bool Prefix_SimulateNewOrderWithPositionAndDirectionAux(IEnumerable<Formation> formations, Dictionary<Formation, Formation> simulationFormations, WorldPosition formationLineBegin, WorldPosition formationLineEnd, bool isSimulatingAgentFrames, ref List<WorldPosition> simulationAgentFrames, bool isSimulatingFormationChanges, ref List<ValueTuple<Formation, int, float, WorldPosition, Vec2>> simulationFormationChanges, ref bool isLineShort, bool isFormationLayoutVertical = true)
		{
			return Patch_OrderController.SimulateNewOrderWithPositionAndDirection(formations, simulationFormations, formationLineBegin, formationLineEnd, isSimulatingAgentFrames, out simulationAgentFrames, isSimulatingFormationChanges, out simulationFormationChanges, out isLineShort, isFormationLayoutVertical, true);
		}

		// Token: 0x06000362 RID: 866 RVA: 0x00010CE4 File Offset: 0x0000EEE4
		public static bool SimulateNewOrderWithPositionAndDirection(IEnumerable<Formation> formations, Dictionary<Formation, Formation> simulationFormations, WorldPosition formationLineBegin, WorldPosition formationLineEnd, bool isSimulatingAgentFrames, out List<WorldPosition> simulationAgentFrames, bool isSimulatingFormationChanges, out List<ValueTuple<Formation, int, float, WorldPosition, Vec2>> simulationFormationChanges, out bool isLineShort, bool isFormationLayoutVertical = true, bool isFromPatch = false)
		{
			simulationAgentFrames = null;
			simulationFormationChanges = null;
			isLineShort = false;
			try
			{
				if (isFromPatch && !RTSCamera.CommandSystem.Utilities.Utility.ShouldEnablePlayerOrderControllerPatchForFormation(formations))
				{
					return true;
				}
				List<Formation> list = formations.ToList<Formation>();
				simulationAgentFrames = ((!isSimulatingAgentFrames) ? null : new List<WorldPosition>());
				simulationFormationChanges = ((!isSimulatingFormationChanges) ? null : new List<ValueTuple<Formation, int, float, WorldPosition, Vec2>>());
				Vec2 vec = formationLineEnd.AsVec2 - formationLineBegin.AsVec2;
				float length = vec.Length;
				isLineShort = false;
				foreach (Formation formation in formations)
				{
					float? formationVirtualWidth = Patch_OrderController.GetFormationVirtualWidth(formation);
					if (formationVirtualWidth != null)
					{
						Patch_OrderController.SetActualWidth(formation, formationVirtualWidth.Value);
					}
					else
					{
						Patch_OrderController.SetFormationVirtualWidth(formation, Patch_OrderController.GetActualOrCurrentWidth(formation));
					}
					Patch_OrderController.TryIntializeFormationChanges(formation);
				}
				if (RTSCamera.CommandSystem.Utilities.Utility.ShouldKeepRelativePositions())
				{
					if (RTSCamera.CommandSystem.Utilities.Utility.ShouldKeepFormationWidth())
					{
						if ((double)length < (double)ManagedParameters.Instance.GetManagedParameter(3))
						{
							isLineShort = true;
						}
					}
					else
					{
						float num;
						float num2;
						Dictionary<Formation, bool> dictionary;
						List<Patch_OrderController.StackRecord> list2;
						List<KeyValuePair<Formation, Vec2>> list3;
						Vec2 vec2;
						Patch_OrderController.CollectStacksRecord(formations, out num, out num2, out dictionary, out list2, out list3, out vec, out vec2);
						if (length < num2 + (float)(formations.Count<Formation>() - dictionary.Count<KeyValuePair<Formation, bool>>((KeyValuePair<Formation, bool> pair) => pair.Value) - 1) * 1.5f)
						{
							isLineShort = true;
						}
					}
				}
				else
				{
					float num3;
					if (isFormationLayoutVertical)
					{
						num3 = formations.Sum<Formation>((Formation f) => Patch_OrderController.GetFormationVirtualMinimumWidth(f)) + (float)(formations.Count<Formation>() - 1) * 1.5f;
					}
					else
					{
						num3 = formations.Max<Formation>((Formation f) => Patch_OrderController.GetFormationVirtualMinimumWidth(f));
					}
					float num4 = num3;
					if ((double)length < (double)num4)
					{
						isLineShort = true;
					}
				}
				IEnumerable<Formation> enumerable = Enumerable.Empty<Formation>();
				if (isLineShort)
				{
					Dictionary<Formation, int> actualUnitSpacings = Patch_OrderController.GetActualUnitSpacings();
					if (actualUnitSpacings != null)
					{
						foreach (Formation formation2 in formations)
						{
							if (actualUnitSpacings.ContainsKey(formation2))
							{
								if (!Patch_OrderController._naturalUnitSpacings.ContainsKey(formation2))
								{
									Patch_OrderController._naturalUnitSpacings[formation2] = actualUnitSpacings[formation2];
								}
							}
							else
							{
								Patch_OrderController._naturalUnitSpacings[formation2] = ArrangementOrder.GetUnitSpacingOf(Patch_OrderController.GetFormationVirtualArrangementOrder(formation2));
							}
							int? formationVirtualUnitSpacing = Patch_OrderController.GetFormationVirtualUnitSpacing(formation2);
							if (formationVirtualUnitSpacing != null)
							{
								actualUnitSpacings[formation2] = formationVirtualUnitSpacing.Value;
							}
						}
					}
					if (RTSCamera.CommandSystem.Utilities.Utility.ShouldKeepRelativePositions() && formations.Any<Formation>())
					{
						WorldPosition worldPosition = formationLineBegin;
						Patch_OrderController.SimulateNewOrderWithKeepingRelativePositions(formations, simulationFormations, true, worldPosition, new WorldPosition?(formationLineBegin), new WorldPosition?(formationLineEnd), isSimulatingAgentFrames, simulationAgentFrames, isSimulatingFormationChanges, simulationFormationChanges, out enumerable);
						formations = enumerable;
					}
					if (formations.Any<Formation>())
					{
						float num5;
						if (isFormationLayoutVertical)
						{
							num5 = formations.Sum<Formation>((Formation f) => Patch_OrderController.GetActualOrCurrentWidth(f)) + (float)(formations.Count<Formation>() - 1) * 1.5f;
						}
						else
						{
							num5 = formations.Max<Formation>((Formation f) => Patch_OrderController.GetActualOrCurrentWidth(f));
						}
						float num6 = num5;
						Vec2 formationVirtualDirection = Patch_OrderController.GetFormationVirtualDirection(Extensions.MaxBy<Formation, int>(formations, (Formation f) => f.CountOfUnitsWithoutDetachedOnes));
						formationVirtualDirection.RotateCCW(-1.5707964f);
						formationVirtualDirection.Normalize();
						formationLineEnd = Mission.Current.GetStraightPathToTarget(formationLineBegin.AsVec2 + num6 / 2f * formationVirtualDirection, formationLineBegin, 1f, true);
						formationLineBegin = Mission.Current.GetStraightPathToTarget(formationLineBegin.AsVec2 - num6 / 2f * formationVirtualDirection, formationLineBegin, 1f, true);
					}
				}
				else
				{
					foreach (Formation formation3 in formations)
					{
						Patch_OrderController.SetActualUnitSpacing(formation3, Patch_OrderController.GetFormationVirtualNaturalUnitSpacing(formation3));
					}
					formationLineEnd = Mission.Current.GetStraightPathToTarget(formationLineEnd.AsVec2, formationLineBegin, 1f, true);
					if (RTSCamera.CommandSystem.Utilities.Utility.ShouldKeepRelativePositions())
					{
						WorldPosition worldPosition2 = formationLineBegin;
						worldPosition2.SetVec2((formationLineBegin.AsVec2 + formationLineEnd.AsVec2) / 2f);
						Patch_OrderController.SimulateNewOrderWithKeepingRelativePositions(formations, simulationFormations, false, worldPosition2, new WorldPosition?(formationLineBegin), new WorldPosition?(formationLineEnd), isSimulatingAgentFrames, simulationAgentFrames, isSimulatingFormationChanges, simulationFormationChanges, out enumerable);
						formations = enumerable;
					}
				}
				if (formations.Any<Formation>())
				{
					if (isFormationLayoutVertical)
					{
						Patch_OrderController.SimulateNewOrderWithVerticalLayout(formations, simulationFormations, isLineShort, formationLineBegin, formationLineEnd, isSimulatingAgentFrames, simulationAgentFrames, isSimulatingFormationChanges, simulationFormationChanges);
					}
					else
					{
						Patch_OrderController.SimulateNewOrderWithHorizontalLayout(formations, simulationFormations, isLineShort, formationLineBegin, formationLineEnd, isSimulatingAgentFrames, simulationAgentFrames, isSimulatingFormationChanges, simulationFormationChanges);
					}
				}
				foreach (Formation formation4 in list)
				{
					Patch_OrderController.RemoveActualWidth(formation4);
				}
				return false;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				MissionSharedLibrary.Utilities.Utility.DisplayMessage(ex.ToString());
			}
			return true;
		}

		// Token: 0x06000363 RID: 867 RVA: 0x00011238 File Offset: 0x0000F438
		private static void SimulateNewOrderWithKeepingRelativePositions(IEnumerable<Formation> formations, Dictionary<Formation, Formation> simulationFormations, bool isLineShort, WorldPosition clickedCenter, WorldPosition? formationLineBegin, WorldPosition? formationLineEnd, bool isSimulatingAgentFrames, List<WorldPosition> simulationAgentFrames, bool isSimulatingFormationChanges, List<ValueTuple<Formation, int, float, WorldPosition, Vec2>> simulationFormationChanges, out IEnumerable<Formation> remainingFormations)
		{
			if (isLineShort)
			{
				Patch_OrderController.SimulateNewOrderWithKeepingRelativePositionsLineShort(formations, simulationFormations, clickedCenter, formationLineBegin, formationLineEnd, isSimulatingAgentFrames, simulationAgentFrames, isSimulatingFormationChanges, simulationFormationChanges, out remainingFormations);
				return;
			}
			if (RTSCamera.CommandSystem.Utilities.Utility.ShouldKeepFormationWidth())
			{
				Patch_OrderController.SimulateNewOrderWithKeepingRelativePositionsNotLineShortKeepingFormationWidth(formations, simulationFormations, clickedCenter, formationLineBegin, formationLineEnd, isSimulatingAgentFrames, simulationAgentFrames, isSimulatingFormationChanges, simulationFormationChanges, out remainingFormations);
				return;
			}
			Patch_OrderController.SimulateNewOrderWithKeepingRelativePositionsNotLineShortNotKeepingFormationWidth(formations, simulationFormations, clickedCenter, formationLineBegin, formationLineEnd, isSimulatingAgentFrames, simulationAgentFrames, isSimulatingFormationChanges, simulationFormationChanges, out remainingFormations);
		}

		// Token: 0x06000364 RID: 868 RVA: 0x00011294 File Offset: 0x0000F494
		private static void SimulateNewOrderWithKeepingRelativePositionsLineShort(IEnumerable<Formation> formations, Dictionary<Formation, Formation> simulationFormations, WorldPosition clickedCenter, WorldPosition? formationLineBegin, WorldPosition? formationLineEnd, bool isSimulatingAgentFrames, List<WorldPosition> simulationAgentFrames, bool isSimulatingFormationChanges, List<ValueTuple<Formation, int, float, WorldPosition, Vec2>> simulationFormationChanges, out IEnumerable<Formation> remainingFormations)
		{
			simulationAgentFrames = ((!isSimulatingAgentFrames) ? null : simulationAgentFrames);
			simulationFormationChanges = ((!isSimulatingFormationChanges) ? null : simulationFormationChanges);
			Vec2 vec;
			Vec2 vec2;
			List<KeyValuePair<Formation, Vec2>> list = Patch_OrderController.CollectFormationVirtualOrderPositions(formations, out vec, false, out vec2).ToList<KeyValuePair<Formation, Vec2>>();
			List<Formation> list2 = new List<Formation>();
			remainingFormations = list2;
			float length = (formationLineEnd.Value.AsVec2 - formationLineBegin.Value.AsVec2).Length;
			Vec2 zero = Vec2.Zero;
			new Dictionary<Formation, bool>();
			Vec2 invalid = Vec2.Invalid;
			foreach (KeyValuePair<Formation, Vec2> keyValuePair in list)
			{
				Formation key = keyValuePair.Key;
				Vec2 value = keyValuePair.Value;
				if (!value.IsValid)
				{
					list2.Add(key);
				}
				else
				{
					int num = 0;
					int actualOrCurrentUnitSpacing = Patch_OrderController.GetActualOrCurrentUnitSpacing(key);
					float actualOrCurrentWidth = Patch_OrderController.GetActualOrCurrentWidth(key);
					Vec2 vec3 = value - vec + clickedCenter.AsVec2;
					WorldPosition worldPosition = clickedCenter;
					worldPosition.SetVec2(vec3);
					bool flag = Patch_OrderController.GetFormationVirtualArrangementOrder(key) == 1;
					Vec2 vec4;
					if (flag)
					{
						vec4 = Patch_OrderController.GetColumnFormationNewDirection(key, value, vec3);
					}
					else
					{
						if (Patch_OrderController.GetFormationVirtualFacingOrder(key) == 14)
						{
							Formation virtualFacingEnemyTargetFormation = Patch_OrderController.GetVirtualFacingEnemyTargetFormation(key);
							if (virtualFacingEnemyTargetFormation != null && virtualFacingEnemyTargetFormation.CountOfUnits == 0)
							{
								Patch_OrderController.LivePreviewFormationChanges.ClearFacingOrderTarget(key);
							}
						}
						Vec2 vec5 = key.Direction;
						if (RTSCamera.CommandSystem.Utilities.Utility.ShouldQueueCommand())
						{
							vec5 = Patch_OrderController.GetFormationVirtualDirectionIncludingFacingEnemyAccordingToPositionAndDirection(key, Patch_OrderController.GetFormationVirtualPositionVec2(key), Patch_OrderController.GetFormationVirtualDirection(key));
						}
						vec4 = Patch_OrderController.GetFormationVirtualDirectionIncludingFacingEnemyAccordingToPositionAndDirection(key, vec3, vec5);
					}
					WorldPosition worldPosition2;
					WorldPosition worldPosition3;
					Patch_OrderController.GetFormationLineBeginEnd(key, worldPosition, out worldPosition2, out worldPosition3);
					Vec2 vec6 = worldPosition3.AsVec2 - worldPosition2.AsVec2;
					float length2 = vec6.Length;
					vec6.Normalize();
					float num2 = MathF.Clamp(MBMath.ApproximatelyEqualsTo(length2, actualOrCurrentWidth, 0.1f) ? actualOrCurrentWidth : length2, Patch_OrderController.GetFormationVirtualMinimumWidth(key), Patch_OrderController.GetFormationVirtualMaximumWidth(key));
					if (isSimulatingFormationChanges)
					{
						Patch_OrderController.LivePreviewFormationChanges.UpdateFormationChange(key, new WorldPosition?(worldPosition), null, null, null);
						if (flag)
						{
							Patch_OrderController.LivePreviewFormationChanges.UpdateFormationChange(key, null, new Vec2?(vec4), null, null);
						}
					}
					if (!Mission.Current.IsPositionInsideBoundaries(worldPosition.AsVec2))
					{
						Vec2 closestBoundaryPosition = Mission.Current.GetClosestBoundaryPosition(worldPosition.AsVec2);
						worldPosition.SetVec2(closestBoundaryPosition);
					}
					Patch_OrderController.DecreaseUnitSpacingAndWidthIfNotAllUnitsFit(key, Patch_OrderController.GetSimulationFormation(key, simulationFormations), in worldPosition, in vec4, ref num2, ref num, actualOrCurrentUnitSpacing);
					float num3;
					Patch_OrderController.SimulateNewOrderWithFrameAndWidth(key, Patch_OrderController.GetSimulationFormation(key, simulationFormations), simulationAgentFrames, simulationFormationChanges, in worldPosition, in vec4, num2, num, true, out num3, actualOrCurrentUnitSpacing);
					if (isSimulatingFormationChanges)
					{
						Patch_OrderController.LivePreviewFormationChanges.SetPreviewShape(key, num2, num3);
					}
				}
			}
		}

		// Token: 0x06000365 RID: 869 RVA: 0x00011588 File Offset: 0x0000F788
		private static void SimulateNewOrderWithKeepingRelativePositionsNotLineShortKeepingFormationWidth(IEnumerable<Formation> formations, Dictionary<Formation, Formation> simulationFormations, WorldPosition clickedCenter, WorldPosition? formationLineBegin, WorldPosition? formationLineEnd, bool isSimulatingAgentFrames, List<WorldPosition> simulationAgentFrames, bool isSimulatingFormationChanges, List<ValueTuple<Formation, int, float, WorldPosition, Vec2>> simulationFormationChanges, out IEnumerable<Formation> remainingFormations)
		{
			simulationAgentFrames = ((!isSimulatingAgentFrames) ? null : simulationAgentFrames);
			simulationFormationChanges = ((!isSimulatingFormationChanges) ? null : simulationFormationChanges);
			Vec2 vec;
			Vec2 vec2;
			List<KeyValuePair<Formation, Vec2>> list = Patch_OrderController.CollectFormationVirtualOrderPositions(formations, out vec, true, out vec2).ToList<KeyValuePair<Formation, Vec2>>();
			List<Formation> list2 = new List<Formation>();
			remainingFormations = list2;
			Vec2 vec3 = formationLineEnd.Value.AsVec2 - formationLineBegin.Value.AsVec2;
			vec3.Normalize();
			Vec2 vec4 = new Vec2(-vec3.y, vec3.x).Normalized();
			foreach (KeyValuePair<Formation, Vec2> keyValuePair in list)
			{
				Formation key = keyValuePair.Key;
				Vec2 value = keyValuePair.Value;
				if (!value.IsValid)
				{
					list2.Add(key);
				}
				else
				{
					int num = 0;
					int actualOrCurrentUnitSpacing = Patch_OrderController.GetActualOrCurrentUnitSpacing(key);
					float actualOrCurrentWidth = Patch_OrderController.GetActualOrCurrentWidth(key);
					Vec2 vec5 = Patch_OrderController.rotateVector(value - vec, vec2, vec4) + clickedCenter.AsVec2;
					WorldPosition worldPosition = clickedCenter;
					worldPosition.SetVec2(vec5);
					float num2 = MathF.Min(actualOrCurrentWidth, Patch_OrderController.GetFormationVirtualMaximumWidth(key));
					Vec2 vec6 = Patch_OrderController.GetFormationVirtualDirection(key);
					if (Patch_OrderController.GetFormationVirtualArrangementOrder(key) == 1)
					{
						vec6 = Patch_OrderController.GetColumnFormationNewDirection(key, value, vec5);
					}
					else
					{
						vec6 = Patch_OrderController.rotateVector(vec6, vec2, vec4);
					}
					if (isSimulatingFormationChanges)
					{
						Patch_OrderController.LivePreviewFormationChanges.UpdateFormationChange(key, new WorldPosition?(worldPosition), new Vec2?(vec6), null, new float?(num2));
						Patch_OrderController.LivePreviewFormationChanges.SetFacingOrder(15, formations, null);
					}
					Patch_OrderController.DecreaseUnitSpacingAndWidthIfNotAllUnitsFit(key, Patch_OrderController.GetSimulationFormation(key, simulationFormations), in worldPosition, in vec6, ref num2, ref num, actualOrCurrentUnitSpacing);
					float num3;
					Patch_OrderController.SimulateNewOrderWithFrameAndWidth(key, Patch_OrderController.GetSimulationFormation(key, simulationFormations), simulationAgentFrames, simulationFormationChanges, in worldPosition, in vec6, num2, num, true, out num3, actualOrCurrentUnitSpacing);
					if (isSimulatingFormationChanges)
					{
						Patch_OrderController.LivePreviewFormationChanges.SetPreviewShape(key, num2, num3);
					}
				}
			}
		}

		// Token: 0x06000366 RID: 870 RVA: 0x0001178C File Offset: 0x0000F98C
		private static Vec2 GetLeftFlankPosition(Formation formation, Vec2 orderPosition, Vec2 dragVec)
		{
			float actualOrCurrentWidth = Patch_OrderController.GetActualOrCurrentWidth(formation);
			return orderPosition + -dragVec * actualOrCurrentWidth * 0.5f;
		}

		// Token: 0x06000367 RID: 871 RVA: 0x000117BC File Offset: 0x0000F9BC
		private static Vec2 GetRightFlankPosition(Formation formation, Vec2 orderPosition, Vec2 dragVec)
		{
			float actualOrCurrentWidth = Patch_OrderController.GetActualOrCurrentWidth(formation);
			return orderPosition + dragVec * actualOrCurrentWidth * 0.5f;
		}

		// Token: 0x06000368 RID: 872 RVA: 0x000117E8 File Offset: 0x0000F9E8
		private static void CollectStacksRecord(IEnumerable<Formation> formations, out float oldOverallWidth, out float minOverallWidth, out Dictionary<Formation, bool> shouldFormationBeStackedWithPreviousFormation, out List<Patch_OrderController.StackRecord> stacksRecord, out List<KeyValuePair<Formation, Vec2>> formationOrderPositionList, out Vec2 averageOrderPosition, out Vec2 weightedAverageDirection)
		{
			formationOrderPositionList = Patch_OrderController.CollectFormationVirtualOrderPositions(formations, out averageOrderPosition, true, out weightedAverageDirection).ToList<KeyValuePair<Formation, Vec2>>();
			oldOverallWidth = 0f;
			minOverallWidth = 0f;
			shouldFormationBeStackedWithPreviousFormation = new Dictionary<Formation, bool>();
			stacksRecord = new List<Patch_OrderController.StackRecord>();
			Vec2 oldDragVec = new Vec2(weightedAverageDirection.y, -weightedAverageDirection.x);
			oldDragVec.Normalize();
			formationOrderPositionList.Sort((KeyValuePair<Formation, Vec2> pair1, KeyValuePair<Formation, Vec2> pair2) => Patch_OrderController.GetLeftFlankPosition(pair1.Key, pair1.Value, oldDragVec).DotProduct(oldDragVec).CompareTo(Patch_OrderController.GetLeftFlankPosition(pair2.Key, pair2.Value, oldDragVec).DotProduct(oldDragVec)));
			Patch_OrderController.StackRecord stackRecord = new Patch_OrderController.StackRecord
			{
				Formations = new List<Formation> { formationOrderPositionList[0].Key },
				LeftMost = Patch_OrderController.GetLeftFlankPosition(formationOrderPositionList[0].Key, formationOrderPositionList[0].Value, oldDragVec).DotProduct(oldDragVec),
				RightMost = Patch_OrderController.GetRightFlankPosition(formationOrderPositionList[0].Key, formationOrderPositionList[0].Value, oldDragVec).DotProduct(oldDragVec),
				MinimumWidth = Patch_OrderController.GetFormationVirtualMinimumWidth(formationOrderPositionList[0].Key),
				MaximumWidth = Patch_OrderController.GetFormationVirtualMaximumWidth(formationOrderPositionList[0].Key),
				Width = Patch_OrderController.GetActualOrCurrentWidth(formationOrderPositionList[0].Key)
			};
			for (int i = 1; i < formationOrderPositionList.Count; i++)
			{
				Formation key = formationOrderPositionList[i].Key;
				Vec2 value = formationOrderPositionList[i].Value;
				float actualOrCurrentWidth = Patch_OrderController.GetActualOrCurrentWidth(key);
				if (Patch_OrderController.ShouldFormationBeStackedTogether(stackRecord, key, value, oldDragVec))
				{
					shouldFormationBeStackedWithPreviousFormation[key] = true;
					stackRecord.MinimumWidth = MathF.Max(stackRecord.MinimumWidth, Patch_OrderController.GetFormationVirtualMinimumWidth(key));
					stackRecord.MaximumWidth = MathF.Max(stackRecord.MaximumWidth, Patch_OrderController.GetFormationVirtualMaximumWidth(key));
					stackRecord.LeftMost = MathF.Min(stackRecord.LeftMost, Patch_OrderController.GetLeftFlankPosition(key, value, oldDragVec).DotProduct(oldDragVec));
					stackRecord.RightMost = MathF.Max(stackRecord.RightMost, Patch_OrderController.GetRightFlankPosition(key, value, oldDragVec).DotProduct(oldDragVec));
					stackRecord.Width = MathF.Max(stackRecord.Width, actualOrCurrentWidth);
					stackRecord.Formations.Add(key);
				}
				else
				{
					oldOverallWidth += stackRecord.Width;
					minOverallWidth += stackRecord.MinimumWidth;
					stacksRecord.Add(stackRecord);
					stackRecord = new Patch_OrderController.StackRecord
					{
						Formations = new List<Formation> { key },
						LeftMost = Patch_OrderController.GetLeftFlankPosition(key, value, oldDragVec).DotProduct(oldDragVec),
						RightMost = Patch_OrderController.GetRightFlankPosition(key, value, oldDragVec).DotProduct(oldDragVec),
						Width = actualOrCurrentWidth,
						MinimumWidth = Patch_OrderController.GetFormationVirtualMinimumWidth(key),
						MaximumWidth = Patch_OrderController.GetFormationVirtualMaximumWidth(key)
					};
				}
			}
			oldOverallWidth += stackRecord.Width;
			minOverallWidth += stackRecord.MinimumWidth;
			stacksRecord.Add(stackRecord);
		}

		// Token: 0x06000369 RID: 873 RVA: 0x00011B40 File Offset: 0x0000FD40
		private static List<float> GetExpectedWidths(List<Patch_OrderController.StackRecord> stacksRecord, float availableWidth, float oldOverallWidth)
		{
			List<float> list = stacksRecord.Select<Patch_OrderController.StackRecord, float>((Patch_OrderController.StackRecord r) => r.Width).ToList<float>();
			List<float> currentRatio = stacksRecord.Select<Patch_OrderController.StackRecord, float>((Patch_OrderController.StackRecord r) => r.Width / MathF.Max(r.MaximumWidth, 0.1f)).ToList<float>();
			List<int> list2 = stacksRecord.Select<Patch_OrderController.StackRecord, int>((Patch_OrderController.StackRecord r, int i) => i).ToList<int>();
			list2.Sort(delegate(int i1, int i2)
			{
				float num3 = currentRatio[i1] - currentRatio[i2];
				if (num3 <= 0f)
				{
					return (num3 < 0f) ? 1 : 0;
				}
				return -1;
			});
			for (int j = 0; j < list2.Count; j++)
			{
				int num = list2[j];
				float width = stacksRecord[num].Width;
				float num2 = MathF.Min(width * availableWidth / oldOverallWidth, stacksRecord[num].MaximumWidth);
				availableWidth -= num2;
				oldOverallWidth -= width;
				list[num] = num2;
			}
			return list;
		}

		// Token: 0x0600036A RID: 874 RVA: 0x00011C48 File Offset: 0x0000FE48
		private static void SimulateNewOrderWithKeepingRelativePositionsNotLineShortNotKeepingFormationWidth(IEnumerable<Formation> formations, Dictionary<Formation, Formation> simulationFormations, WorldPosition clickedCenter, WorldPosition? formationLineBegin, WorldPosition? formationLineEnd, bool isSimulatingAgentFrames, List<WorldPosition> simulationAgentFrames, bool isSimulatingFormationChanges, List<ValueTuple<Formation, int, float, WorldPosition, Vec2>> simulationFormationChanges, out IEnumerable<Formation> remainingFormations)
		{
			simulationAgentFrames = ((!isSimulatingAgentFrames) ? null : simulationAgentFrames);
			simulationFormationChanges = ((!isSimulatingFormationChanges) ? null : simulationFormationChanges);
			List<Formation> list = new List<Formation>();
			remainingFormations = list;
			Vec2 weightedAverageDirection;
			float num;
			float num2;
			Dictionary<Formation, bool> dictionary;
			List<Patch_OrderController.StackRecord> list2;
			List<KeyValuePair<Formation, Vec2>> list3;
			Vec2 vec;
			Patch_OrderController.CollectStacksRecord(formations, out num, out num2, out dictionary, out list2, out list3, out vec, out weightedAverageDirection);
			Dictionary<Formation, Vec2> formationOrderPostionDictionary = list3.ToDictionary<KeyValuePair<Formation, Vec2>, Formation, Vec2>((KeyValuePair<Formation, Vec2> pair) => pair.Key, (KeyValuePair<Formation, Vec2> pair) => pair.Value);
			Vec2 vec2 = formationLineEnd.Value.AsVec2 - formationLineBegin.Value.AsVec2;
			float length = vec2.Length;
			vec2.Normalize();
			float num3 = 1.5f;
			float num4 = MathF.Clamp(length - (float)(formations.Count<Formation>() - dictionary.Count - 1) * num3, 0f, list2.Sum<Patch_OrderController.StackRecord>((Patch_OrderController.StackRecord r) => r.MaximumWidth));
			bool flag = MBMath.ApproximatelyEqualsTo(num4, num, 0.1f);
			List<float> list4;
			if (!flag)
			{
				list4 = Patch_OrderController.GetExpectedWidths(list2, num4, num);
			}
			else
			{
				list4 = list2.Select<Patch_OrderController.StackRecord, float>((Patch_OrderController.StackRecord r) => r.Width).ToList<float>();
			}
			List<float> list5 = list4;
			Vec2 vec3 = new Vec2(-vec2.y, vec2.x).Normalized();
			float num5 = 0f;
			Vec2 invalid = Vec2.Invalid;
			Comparison<Formation> <>9__4;
			for (int i = 0; i < list2.Count; i++)
			{
				List<Formation> formations2 = list2[i].Formations;
				float num6 = list5[i];
				Comparison<Formation> comparison;
				if ((comparison = <>9__4) == null)
				{
					comparison = (<>9__4 = delegate(Formation f1, Formation f2)
					{
						Vec2 vec7 = formationOrderPostionDictionary[f1];
						Vec2 vec8 = formationOrderPostionDictionary[f2];
						return vec7.DotProduct(-weightedAverageDirection).CompareTo(vec8.DotProduct(-weightedAverageDirection));
					});
				}
				formations2.Sort(comparison);
				float? num7 = null;
				foreach (Formation formation in formations2)
				{
					Vec2 vec4 = formationOrderPostionDictionary[formation];
					if (!vec4.IsValid)
					{
						list.Add(formation);
					}
					else
					{
						int num8 = 0;
						int actualOrCurrentUnitSpacing = Patch_OrderController.GetActualOrCurrentUnitSpacing(formation);
						float actualOrCurrentWidth = Patch_OrderController.GetActualOrCurrentWidth(formation);
						float num9 = MathF.Clamp(flag ? actualOrCurrentWidth : (num6 / list2[i].Width * actualOrCurrentWidth), Patch_OrderController.GetFormationVirtualMinimumWidth(formation), Patch_OrderController.GetFormationVirtualMaximumWidth(formation));
						Vec2 vec5 = Patch_OrderController.rotateVector(vec4 - vec, weightedAverageDirection, vec3) + clickedCenter.AsVec2;
						if (num7 == null)
						{
							num7 = new float?(MathF.Clamp(vec5.DotProduct(-vec3) - formationLineBegin.Value.AsVec2.DotProduct(-vec3), -20f, 10f));
						}
						vec5 = formationLineBegin.Value.AsVec2 + num7.Value * -vec3;
						vec5 += vec2 * (num6 * 0.5f + num5);
						WorldPosition worldPosition = clickedCenter;
						worldPosition.SetVec2(vec5);
						Vec2 vec6 = vec3;
						if (Patch_OrderController.GetFormationVirtualArrangementOrder(formation) == 1)
						{
							vec6 = Patch_OrderController.GetColumnFormationNewDirection(formation, vec4, vec5);
						}
						if (isSimulatingFormationChanges)
						{
							Patch_OrderController.LivePreviewFormationChanges.UpdateFormationChange(formation, new WorldPosition?(worldPosition), new Vec2?(vec6), null, new float?(num9));
							Patch_OrderController.LivePreviewFormationChanges.SetFacingOrder(15, formations, null);
						}
						Patch_OrderController.DecreaseUnitSpacingAndWidthIfNotAllUnitsFit(formation, Patch_OrderController.GetSimulationFormation(formation, simulationFormations), in worldPosition, in vec6, ref num9, ref num8, actualOrCurrentUnitSpacing);
						float num10;
						Patch_OrderController.SimulateNewOrderWithFrameAndWidth(formation, Patch_OrderController.GetSimulationFormation(formation, simulationFormations), simulationAgentFrames, simulationFormationChanges, in worldPosition, in vec6, num9, num8, true, out num10, actualOrCurrentUnitSpacing);
						int num11 = MathF.Max(actualOrCurrentUnitSpacing - num8, 0);
						if (isSimulatingFormationChanges)
						{
							Patch_OrderController.LivePreviewFormationChanges.UpdateFormationChange(formation, null, null, new int?(num11), null);
							Patch_OrderController.LivePreviewFormationChanges.SetPreviewShape(formation, num9, num10);
						}
						num7 += num10 + Patch_OrderController.GetGapBetweenLinesOfFormation(formation, (float)num11);
					}
				}
				num5 += num6 + num3;
			}
		}

		// Token: 0x0600036B RID: 875 RVA: 0x000120E4 File Offset: 0x000102E4
		private static bool ShouldFormationBeStackedTogether(Patch_OrderController.StackRecord stackRecord, Formation formation, Vec2 orderPosition, Vec2 dragVec)
		{
			return MathF.Abs(stackRecord.Center - orderPosition.DotProduct(dragVec)) < MathF.Max(stackRecord.Width, Patch_OrderController.GetActualOrCurrentWidth(formation)) * 0.5f;
		}

		// Token: 0x0600036C RID: 876 RVA: 0x00012114 File Offset: 0x00010314
		private static Vec2 GetColumnFormationNewDirection(Formation formation, Vec2 oldOrderPositionVec2, Vec2 newOrderPositionVec2)
		{
			if (RTSCamera.CommandSystem.Utilities.Utility.ShouldQueueCommand())
			{
				return (newOrderPositionVec2 - oldOrderPositionVec2).Normalized();
			}
			IFormationArrangement arrangement = formation.Arrangement;
			return (newOrderPositionVec2 - formation.CurrentPosition).Normalized();
		}

		// Token: 0x0600036D RID: 877 RVA: 0x00012154 File Offset: 0x00010354
		private static void GetFormationLineBeginEnd(Formation formation, WorldPosition formationLineBegin, out WorldPosition begin, out WorldPosition end)
		{
			float actualOrCurrentWidth = Patch_OrderController.GetActualOrCurrentWidth(formation);
			Vec2 formationVirtualDirection = Patch_OrderController.GetFormationVirtualDirection(formation);
			formationVirtualDirection.RotateCCW(-1.5707964f);
			formationVirtualDirection.Normalize();
			end = Mission.Current.GetStraightPathToTarget(formationLineBegin.AsVec2 - actualOrCurrentWidth / 2f * formationVirtualDirection, formationLineBegin, 1f, true);
			begin = Mission.Current.GetStraightPathToTarget(formationLineBegin.AsVec2 + actualOrCurrentWidth / 2f * formationVirtualDirection, formationLineBegin, 1f, true);
		}

		// Token: 0x0600036E RID: 878 RVA: 0x000121E4 File Offset: 0x000103E4
		private static void SimulateNewOrderWithHorizontalLayout(IEnumerable<Formation> formations, Dictionary<Formation, Formation> simulationFormations, bool isLineShort, WorldPosition formationLineBegin, WorldPosition formationLineEnd, bool isSimulatingAgentFrames, List<WorldPosition> simulationAgentFrames, bool isSimulatingFormationChanges, List<ValueTuple<Formation, int, float, WorldPosition, Vec2>> simulationFormationChanges)
		{
			simulationAgentFrames = ((!isSimulatingAgentFrames) ? null : simulationAgentFrames);
			simulationFormationChanges = ((!isSimulatingFormationChanges) ? null : simulationFormationChanges);
			Vec2 vec = formationLineEnd.AsVec2 - formationLineBegin.AsVec2;
			float num = vec.Normalize();
			float num2 = formations.Max<Formation>((Formation f) => Patch_OrderController.GetFormationVirtualMinimumWidth(f));
			if (num < num2)
			{
				num = num2;
			}
			Vec2 vec2 = new Vec2(0f - vec.y, vec.x).Normalized();
			float num3 = 0f;
			formations = Patch_OrderController.SortFormationsForHorizontalLayout(formations);
			foreach (Formation formation in formations)
			{
				float num4 = num;
				num4 = MathF.Min(num4, Patch_OrderController.GetFormationVirtualMaximumWidth(formation));
				WorldPosition worldPosition = formationLineBegin;
				Vec2 vec3 = (formationLineEnd.AsVec2 + formationLineBegin.AsVec2) * 0.5f - vec2 * num3;
				worldPosition.SetVec2(vec3);
				if (isSimulatingFormationChanges)
				{
					Patch_OrderController.LivePreviewFormationChanges.UpdateFormationChange(formation, new WorldPosition?(worldPosition), new Vec2?(vec2), null, null);
				}
				if (isSimulatingFormationChanges && !isLineShort)
				{
					Patch_OrderController.LivePreviewFormationChanges.UpdateFormationChange(formation, null, null, null, new float?(num4));
					Patch_OrderController.LivePreviewFormationChanges.SetFacingOrder(15, formations, null);
				}
				int num5 = 0;
				int actualOrCurrentUnitSpacing = Patch_OrderController.GetActualOrCurrentUnitSpacing(formation);
				Patch_OrderController.DecreaseUnitSpacingAndWidthIfNotAllUnitsFit(formation, Patch_OrderController.GetSimulationFormation(formation, simulationFormations), in worldPosition, in vec2, ref num4, ref num5, actualOrCurrentUnitSpacing);
				float num6;
				Patch_OrderController.SimulateNewOrderWithFrameAndWidth(formation, Patch_OrderController.GetSimulationFormation(formation, simulationFormations), simulationAgentFrames, simulationFormationChanges, in worldPosition, in vec2, num4, num5, true, out num6, actualOrCurrentUnitSpacing);
				if (isSimulatingFormationChanges && !isLineShort)
				{
					int num7 = MathF.Max(Patch_OrderController.GetActualOrCurrentUnitSpacing(formation) - num5, 0);
					Patch_OrderController.LivePreviewFormationChanges.UpdateFormationChange(formation, null, null, new int?(num7), null);
					Patch_OrderController.LivePreviewFormationChanges.SetPreviewShape(formation, num4, num6);
				}
				num3 += num6 + Patch_OrderController.GetGapBetweenLinesOfFormation(formation, (float)(actualOrCurrentUnitSpacing - num5));
			}
		}

		// Token: 0x0600036F RID: 879 RVA: 0x00012444 File Offset: 0x00010644
		private static void SimulateNewOrderWithVerticalLayout(IEnumerable<Formation> formations, Dictionary<Formation, Formation> simulationFormations, bool isLineShort, WorldPosition formationLineBegin, WorldPosition formationLineEnd, bool isSimulatingAgentFrames, List<WorldPosition> simulationAgentFrames, bool isSimulatingFormationChanges, List<ValueTuple<Formation, int, float, WorldPosition, Vec2>> simulationFormationChanges)
		{
			simulationAgentFrames = ((!isSimulatingAgentFrames) ? null : simulationAgentFrames);
			simulationFormationChanges = ((!isSimulatingFormationChanges) ? null : simulationFormationChanges);
			Vec2 vec = formationLineEnd.AsVec2 - formationLineBegin.AsVec2;
			float length = vec.Length;
			vec.Normalize();
			float num = MathF.Max(0f, length - (float)(formations.Count<Formation>() - 1) * 1.5f);
			float num2 = formations.Sum<Formation>((Formation f) => Patch_OrderController.GetActualOrCurrentWidth(f));
			bool flag = MBMath.ApproximatelyEqualsTo(num, num2, 0.1f);
			float num3 = formations.Sum<Formation>((Formation f) => Patch_OrderController.GetFormationVirtualMinimumWidth(f));
			Vec2 vec2 = new Vec2(-vec.y, vec.x).Normalized();
			float num4 = 0f;
			foreach (Formation formation in formations)
			{
				float formationVirtualMinimumWidth = Patch_OrderController.GetFormationVirtualMinimumWidth(formation);
				float actualOrCurrentWidth = Patch_OrderController.GetActualOrCurrentWidth(formation);
				float num5 = MathF.Min((flag || RTSCamera.CommandSystem.Utilities.Utility.ShouldKeepFormationWidth()) ? actualOrCurrentWidth : MathF.Min(((double)num < (double)num2) ? actualOrCurrentWidth : float.MaxValue, num * (formationVirtualMinimumWidth / num3)), Patch_OrderController.GetFormationVirtualMaximumWidth(formation));
				WorldPosition worldPosition = formationLineBegin;
				Vec2 vec3 = worldPosition.AsVec2 + vec * (num5 * 0.5f + num4);
				worldPosition.SetVec2(vec3);
				if (isSimulatingFormationChanges)
				{
					Patch_OrderController.LivePreviewFormationChanges.UpdateFormationChange(formation, new WorldPosition?(worldPosition), new Vec2?(vec2), null, null);
				}
				if (isSimulatingFormationChanges && !isLineShort)
				{
					Patch_OrderController.LivePreviewFormationChanges.UpdateFormationChange(formation, null, null, null, new float?(num5));
					Patch_OrderController.LivePreviewFormationChanges.SetFacingOrder(15, formations, null);
				}
				int num6 = 0;
				int actualOrCurrentUnitSpacing = Patch_OrderController.GetActualOrCurrentUnitSpacing(formation);
				Patch_OrderController.DecreaseUnitSpacingAndWidthIfNotAllUnitsFit(formation, Patch_OrderController.GetSimulationFormation(formation, simulationFormations), in worldPosition, in vec2, ref num5, ref num6, actualOrCurrentUnitSpacing);
				float num7;
				Patch_OrderController.SimulateNewOrderWithFrameAndWidth(formation, Patch_OrderController.GetSimulationFormation(formation, simulationFormations), simulationAgentFrames, simulationFormationChanges, in worldPosition, in vec2, num5, num6, true, out num7, actualOrCurrentUnitSpacing);
				if (isSimulatingFormationChanges && !isLineShort)
				{
					int num8 = MathF.Max(Patch_OrderController.GetActualOrCurrentUnitSpacing(formation) - num6, 0);
					Patch_OrderController.LivePreviewFormationChanges.UpdateFormationChange(formation, null, null, new int?(num8), null);
					Patch_OrderController.LivePreviewFormationChanges.SetPreviewShape(formation, num5, num7);
				}
				num4 += num5 + 1.5f;
			}
		}

		// Token: 0x06000370 RID: 880 RVA: 0x0001270C File Offset: 0x0001090C
		private static IEnumerable<Formation> GetSortedFormations(IEnumerable<Formation> formations, bool isFormationLayoutVertical)
		{
			if (!isFormationLayoutVertical)
			{
				return OrderController.SortFormationsForHorizontalLayout(formations);
			}
			return formations;
		}

		// Token: 0x06000371 RID: 881 RVA: 0x00012719 File Offset: 0x00010919
		private static IEnumerable<Formation> SortFormationsForHorizontalLayout(IEnumerable<Formation> formations)
		{
			return formations.OrderBy<Formation, int>((Formation f) => Patch_OrderController.GetLineOrderByClass(f.FormationIndex));
		}

		// Token: 0x06000372 RID: 882 RVA: 0x00012740 File Offset: 0x00010940
		private static int GetLineOrderByClass(FormationClass formationClass)
		{
			FormationClass[] array = new FormationClass[8];
			RuntimeHelpers.InitializeArray(array, fieldof(<PrivateImplementationDetails>.946BEDC3EB2DB5954EE49083B2DB1447F78833A25D6D1D8E54059263034CD2B6).FieldHandle);
			return Array.IndexOf<FormationClass>(array, formationClass);
		}

		// Token: 0x06000373 RID: 883 RVA: 0x0001275C File Offset: 0x0001095C
		private static Dictionary<Formation, int> GetActualUnitSpacings()
		{
			Mission mission = Mission.Current;
			OrderController orderController;
			if (mission == null)
			{
				orderController = null;
			}
			else
			{
				Team playerTeam = mission.PlayerTeam;
				orderController = ((playerTeam != null) ? playerTeam.PlayerOrderController : null);
			}
			OrderController orderController2 = orderController;
			if (orderController2 != null)
			{
				return Patch_OrderController.actualUnitSpacingsField.GetValue(orderController2) as Dictionary<Formation, int>;
			}
			return null;
		}

		// Token: 0x06000374 RID: 884 RVA: 0x0001279C File Offset: 0x0001099C
		private static int GetActualOrCurrentUnitSpacing(Formation formation)
		{
			Dictionary<Formation, int> actualUnitSpacings = Patch_OrderController.GetActualUnitSpacings();
			if (actualUnitSpacings == null)
			{
				return formation.UnitSpacing;
			}
			if (actualUnitSpacings.ContainsKey(formation))
			{
				return actualUnitSpacings[formation];
			}
			return formation.UnitSpacing;
		}

		// Token: 0x06000375 RID: 885 RVA: 0x000127D0 File Offset: 0x000109D0
		private static void SetActualUnitSpacing(Formation formation, int unitSpacing)
		{
			Dictionary<Formation, int> actualUnitSpacings = Patch_OrderController.GetActualUnitSpacings();
			if (actualUnitSpacings == null)
			{
				return;
			}
			if (actualUnitSpacings.ContainsKey(formation))
			{
				actualUnitSpacings[formation] = unitSpacing;
				return;
			}
			actualUnitSpacings.Add(formation, unitSpacing);
		}

		// Token: 0x06000376 RID: 886 RVA: 0x00012804 File Offset: 0x00010A04
		private static void RemoveActualUnitSpacing(Formation formation)
		{
			Dictionary<Formation, int> actualUnitSpacings = Patch_OrderController.GetActualUnitSpacings();
			if (actualUnitSpacings == null)
			{
				return;
			}
			if (actualUnitSpacings.ContainsKey(formation))
			{
				actualUnitSpacings.Remove(formation);
			}
		}

		// Token: 0x06000377 RID: 887 RVA: 0x0001282C File Offset: 0x00010A2C
		private static Dictionary<Formation, float> GetActualWidths()
		{
			Mission mission = Mission.Current;
			OrderController orderController;
			if (mission == null)
			{
				orderController = null;
			}
			else
			{
				Team playerTeam = mission.PlayerTeam;
				orderController = ((playerTeam != null) ? playerTeam.PlayerOrderController : null);
			}
			OrderController orderController2 = orderController;
			if (orderController2 != null)
			{
				return Patch_OrderController.actualWidthsField.GetValue(orderController2) as Dictionary<Formation, float>;
			}
			return null;
		}

		// Token: 0x06000378 RID: 888 RVA: 0x0001286C File Offset: 0x00010A6C
		private static float GetActualOrCurrentWidth(Formation formation)
		{
			Dictionary<Formation, float> actualWidths = Patch_OrderController.GetActualWidths();
			if (actualWidths.ContainsKey(formation))
			{
				return actualWidths[formation];
			}
			return formation.Width;
		}

		// Token: 0x06000379 RID: 889 RVA: 0x00012898 File Offset: 0x00010A98
		private static void SetActualWidth(Formation formation, float width)
		{
			Dictionary<Formation, float> actualWidths = Patch_OrderController.GetActualWidths();
			if (actualWidths == null)
			{
				return;
			}
			if (actualWidths.ContainsKey(formation))
			{
				actualWidths[formation] = width;
				return;
			}
			actualWidths.Add(formation, width);
		}

		// Token: 0x0600037A RID: 890 RVA: 0x000128CC File Offset: 0x00010ACC
		private static void RemoveActualWidth(Formation formation)
		{
			Dictionary<Formation, float> actualWidths = Patch_OrderController.GetActualWidths();
			if (actualWidths == null)
			{
				return;
			}
			if (actualWidths.ContainsKey(formation))
			{
				actualWidths.Remove(formation);
			}
		}

		// Token: 0x0600037B RID: 891 RVA: 0x000128F4 File Offset: 0x00010AF4
		private static void DecreaseUnitSpacingAndWidthIfNotAllUnitsFit(Formation formation, Formation simulationFormation, in WorldPosition formationPosition, in Vec2 formationDirection, ref float formationWidth, ref int unitSpacingReduction, int actualUnitSpacing)
		{
			if (simulationFormation.UnitSpacing != actualUnitSpacing)
			{
				simulationFormation = new Formation(null, -1);
			}
			bool flag = formation.CalculateHasSignificantNumberOfMounted && Patch_OrderController.GetFormationVirtualRidingOrder(formation) != 35;
			Patch_OrderController._overridenHasAnyMountedUnit.SetValue(formation, flag);
			Patch_OrderController._overridenHasAnyMountedUnit.SetValue(simulationFormation, flag);
			int num = formation.CountOfUnitsWithoutDetachedOnes - 1;
			float num2 = formationWidth;
			if (num >= 0)
			{
				do
				{
					WorldPosition? worldPosition;
					Vec2? vec;
					Patch_OrderController.GetUnitPositionWithIndexAccordingToNewOrder(formation, simulationFormation, Patch_OrderController.GetFormationVirtualArrangementOrder(formation), null, num, in formationPosition, in formationDirection, formation.Arrangement, formationWidth, null, actualUnitSpacing - unitSpacingReduction, null, formation.Arrangement.UnitCount, formation.HasAnyMountedUnit, formation.Index, out worldPosition, out vec, out num2);
					if (worldPosition != null)
					{
						break;
					}
					unitSpacingReduction++;
				}
				while (actualUnitSpacing - unitSpacingReduction >= 0);
			}
			unitSpacingReduction = MathF.Min(unitSpacingReduction, actualUnitSpacing);
			formationWidth = num2;
			Patch_OrderController._overridenHasAnyMountedUnit.SetValue(formation, null);
			Patch_OrderController._overridenHasAnyMountedUnit.SetValue(simulationFormation, null);
		}

		// Token: 0x0600037C RID: 892 RVA: 0x00012A00 File Offset: 0x00010C00
		private static void SimulateNewOrderWithFrameAndWidth(Formation formation, Formation simulationFormation, List<WorldPosition> simulationAgentFrames, List<ValueTuple<Formation, int, float, WorldPosition, Vec2>> simulationFormationChanges, in WorldPosition formationPosition, in Vec2 formationDirection, float formationWidth, int unitSpacingReduction, bool simulateFormationDepth, out float simulatedFormationDepth, int actualUnitSpacing)
		{
			int num = 0;
			float num2 = (simulateFormationDepth ? 0f : float.NaN);
			bool flag = Mission.Current.Mode != 6 || Mission.Current.IsOrderPositionAvailable(ref formationPosition, formation.Team);
			bool flag2 = formation.CalculateHasSignificantNumberOfMounted && Patch_OrderController.GetFormationVirtualRidingOrder(formation) != 35;
			Patch_OrderController._overridenHasAnyMountedUnit.SetValue(formation, flag2);
			Patch_OrderController._overridenHasAnyMountedUnit.SetValue(simulationFormation, flag2);
			foreach (Agent agent in from u in formation.GetUnitsWithoutDetachedOnes()
				orderby MBCommon.Hash(u.Index, u)
				select u)
			{
				WorldPosition? worldPosition = null;
				Vec2? vec = null;
				if (flag)
				{
					float num3;
					Patch_OrderController.GetUnitPositionWithIndexAccordingToNewOrder(formation, simulationFormation, Patch_OrderController.GetFormationVirtualArrangementOrder(formation), null, num, in formationPosition, in formationDirection, formation.Arrangement, formationWidth, null, actualUnitSpacing - unitSpacingReduction, null, formation.Arrangement.UnitCount, formation.HasAnyMountedUnit, formation.Index, out worldPosition, out vec, out num3);
				}
				else
				{
					worldPosition = new WorldPosition?(agent.GetWorldPosition());
					vec = new Vec2?(agent.GetMovementDirection());
				}
				if (worldPosition != null)
				{
					if (simulationAgentFrames != null)
					{
						simulationAgentFrames.Add(worldPosition.Value);
					}
					if (simulateFormationDepth)
					{
						WorldPosition worldPosition2 = formationPosition;
						Vec2 asVec = worldPosition2.AsVec2;
						worldPosition2 = formationPosition;
						Vec2 asVec2 = worldPosition2.AsVec2;
						Vec2 vec2 = formationDirection;
						float num4 = Vec2.DistanceToLine(asVec, asVec2 + vec2.RightVec(), worldPosition.Value.AsVec2);
						if (num4 > num2)
						{
							num2 = num4;
						}
					}
				}
				num++;
			}
			Patch_OrderController._overridenHasAnyMountedUnit.SetValue(formation, null);
			Patch_OrderController._overridenHasAnyMountedUnit.SetValue(simulationFormation, null);
			if (flag)
			{
				if (simulationFormationChanges != null)
				{
					simulationFormationChanges.Add(ValueTuple.Create<Formation, int, float, WorldPosition, Vec2>(formation, unitSpacingReduction, formationWidth, formationPosition, formationDirection));
				}
			}
			else
			{
				WorldPosition worldPosition3 = formation.CreateNewOrderWorldPosition(0);
				if (simulationFormationChanges != null)
				{
					simulationFormationChanges.Add(ValueTuple.Create<Formation, int, float, WorldPosition, Vec2>(formation, unitSpacingReduction, formationWidth, worldPosition3, formation.Direction));
				}
			}
			simulatedFormationDepth = num2 + formation.UnitDiameter;
		}

		// Token: 0x0600037D RID: 893 RVA: 0x00012C60 File Offset: 0x00010E60
		private static Formation GetSimulationFormation(Formation formation, Dictionary<Formation, Formation> simulationFormations)
		{
			if (simulationFormations == null)
			{
				return null;
			}
			return simulationFormations[formation];
		}

		// Token: 0x0600037E RID: 894 RVA: 0x00012C70 File Offset: 0x00010E70
		private static float GetGapBetweenLinesOfFormation(Formation f, float unitSpacing)
		{
			float num = 1f;
			float num2 = 0.2f;
			if (f.CalculateHasSignificantNumberOfMounted && !(f.RidingOrder == RidingOrder.RidingOrderDismount))
			{
				num = 2f;
				num2 = 0.6f;
			}
			return num + unitSpacing * num2;
		}

		// Token: 0x0600037F RID: 895 RVA: 0x00012CB4 File Offset: 0x00010EB4
		public static bool Prefix_GetOrderLookAtDirection(IEnumerable<Formation> formations, Vec2 target, ref Vec2 __result)
		{
			if (!RTSCamera.CommandSystem.Utilities.Utility.ShouldEnablePlayerOrderControllerPatchForFormation(formations))
			{
				return true;
			}
			if (RTSCamera.CommandSystem.Utilities.Utility.IsAnyFormationHavingMovingOrderPostion(formations))
			{
				int num = 0;
				Vec2 vec = Vec2.Zero;
				foreach (Formation formation in formations)
				{
					if (RTSCamera.CommandSystem.Utilities.Utility.IsFormationOrderPositionMoving(formation))
					{
						WorldPosition? formationMovingOrderPosition = RTSCamera.CommandSystem.Utilities.Utility.GetFormationMovingOrderPosition(formation);
						if (formationMovingOrderPosition != null)
						{
							int countOfUnitsWithoutDetachedOnes = formation.CountOfUnitsWithoutDetachedOnes;
							vec += formationMovingOrderPosition.Value.AsVec2 * (float)countOfUnitsWithoutDetachedOnes;
							num += countOfUnitsWithoutDetachedOnes;
							continue;
						}
					}
					Vec2 formationVirtualPositionVec = Patch_OrderController.GetFormationVirtualPositionVec2(formation);
					if (formationVirtualPositionVec.IsValid)
					{
						int countOfUnitsWithoutDetachedOnes2 = formation.CountOfUnitsWithoutDetachedOnes;
						vec += formationVirtualPositionVec * (float)countOfUnitsWithoutDetachedOnes2;
						num += countOfUnitsWithoutDetachedOnes2;
					}
				}
				if (num > 0)
				{
					vec = vec * 1f / (float)num;
					__result = (target - vec).Normalized();
					return false;
				}
			}
			int num2 = 0;
			Vec2 vec2 = Vec2.Zero;
			foreach (Formation formation2 in formations)
			{
				Vec2 formationVirtualPositionVec2 = Patch_OrderController.GetFormationVirtualPositionVec2(formation2);
				if (formationVirtualPositionVec2.IsValid)
				{
					int countOfUnitsWithoutDetachedOnes3 = formation2.CountOfUnitsWithoutDetachedOnes;
					vec2 += formationVirtualPositionVec2 * (float)countOfUnitsWithoutDetachedOnes3;
					num2 += countOfUnitsWithoutDetachedOnes3;
				}
			}
			if (num2 > 0)
			{
				vec2 = vec2 * 1f / (float)num2;
				__result = (target - vec2).Normalized();
				return false;
			}
			return true;
		}

		// Token: 0x06000380 RID: 896 RVA: 0x00012E60 File Offset: 0x00011060
		public static bool Prefix_SimulateNewFacingOrder(IEnumerable<Formation> formations, Dictionary<Formation, Formation> simulationFormations, Vec2 direction, ref List<WorldPosition> simulationAgentFrames)
		{
			List<Formation> list = formations.Where<Formation>((Formation f) => f.CountOfUnitsWithoutDetachedOnes > 0).ToList<Formation>();
			if (!RTSCamera.CommandSystem.Utilities.Utility.ShouldEnablePlayerOrderControllerPatchForFormation(list))
			{
				return true;
			}
			List<Formation> list2 = new List<Formation>();
			List<Formation> list3 = new List<Formation>();
			foreach (Formation formation in list)
			{
				if (RTSCamera.CommandSystem.Utilities.Utility.ShouldLockFormationDuringLookAtDirection(formation))
				{
					list2.Add(formation);
				}
				else
				{
					list3.Add(formation);
				}
			}
			if (list3.Count > 0)
			{
				foreach (Formation formation2 in list3)
				{
					if (RTSCamera.CommandSystem.Utilities.Utility.IsFormationOrderPositionMoving(formation2))
					{
						WorldPosition? formationMovingOrderPosition = RTSCamera.CommandSystem.Utilities.Utility.GetFormationMovingOrderPosition(formation2);
						Vec2 formationMovingDirection = RTSCamera.CommandSystem.Utilities.Utility.GetFormationMovingDirection(formation2);
						Patch_OrderController.LivePreviewFormationChanges.UpdateFormationChange(formation2, formationMovingOrderPosition, formationMovingDirection.IsValid ? new Vec2?(formationMovingDirection) : null, null, null);
					}
					FormationChange formationChange;
					if (Patch_OrderController.LivePreviewFormationChanges.VirtualChanges.TryGetValue(formation2, out formationChange) && formationChange.MovementOrderType.GetValueOrDefault() == 12)
					{
						Formation targetFormation = formationChange.TargetFormation;
						WorldPosition advanceOrderPosition = Patch_OrderController.GetAdvanceOrderPosition(formation2, 0, targetFormation);
						Patch_OrderController.LivePreviewFormationChanges.UpdateFormationChange(formation2, new WorldPosition?(advanceOrderPosition), null, null, null);
					}
				}
				List<WorldPosition> list4;
				List<ValueTuple<Formation, int, float, WorldPosition, Vec2>> list5;
				Patch_OrderController.SimulateNewFacingOrderWithoutLockingFormations(list3, simulationFormations, direction, true, out list4, false, out list5);
				if (simulationAgentFrames == null)
				{
					simulationAgentFrames = list4;
				}
				else
				{
					simulationAgentFrames.AddRange(list4);
				}
			}
			if (list2.Count > 0)
			{
				List<ValueTuple<Formation, int, float, WorldPosition, Vec2>> list5;
				List<WorldPosition> list6;
				Patch_OrderController.SimulateNewFacingOrderWithLockingFormations(list2, simulationFormations, direction, true, out list6, false, out list5);
				if (simulationAgentFrames == null)
				{
					simulationAgentFrames = list6;
				}
				else
				{
					simulationAgentFrames.AddRange(list6);
				}
			}
			return false;
		}

		// Token: 0x06000381 RID: 897 RVA: 0x00013060 File Offset: 0x00011260
		public static bool Prefix_SetOrderWithPosition(OrderController __instance, OrderType orderType, WorldPosition orderPosition)
		{
			Mission mission = Mission.Current;
			OrderController orderController;
			if (mission == null)
			{
				orderController = null;
			}
			else
			{
				Team playerTeam = mission.PlayerTeam;
				orderController = ((playerTeam != null) ? playerTeam.PlayerOrderController : null);
			}
			if (__instance != orderController)
			{
				return true;
			}
			if (orderType == 15)
			{
				List<Formation> list = new List<Formation>();
				List<Formation> list2 = new List<Formation>();
				int num = 0;
				bool flag = RTSCamera.CommandSystem.Utilities.Utility.ShouldFadeOut();
				foreach (Formation formation in __instance.SelectedFormations)
				{
					if (RTSCamera.CommandSystem.Utilities.Utility.ShouldLockFormationDuringLookAtDirection(formation))
					{
						list.Add(formation);
					}
					else
					{
						list2.Add(formation);
					}
				}
				if (list.Count > 0)
				{
					List<WorldPosition> list3;
					List<ValueTuple<Formation, int, float, WorldPosition, Vec2>> list4;
					Patch_OrderController.SimulateNewFacingOrderWithLockingFormations(list, __instance.simulationFormations, OrderController.GetOrderLookAtDirection(__instance.SelectedFormations, orderPosition.AsVec2), flag, out list3, true, out list4);
					foreach (ValueTuple<Formation, int, float, WorldPosition, Vec2> valueTuple in list4)
					{
						Formation item = valueTuple.Item1;
						WorldPosition item2 = valueTuple.Item4;
						Vec2 item3 = valueTuple.Item5;
						item.SetMovementOrder(MovementOrder.MovementOrderMove(item2));
						item.SetFacingOrder(FacingOrder.FacingOrderLookAtDirection(item3));
					}
					if (flag)
					{
						Patch_OrderTroopPlacer.AddOrderPositionEntities(list3, flag, num);
						num += list3.Count;
					}
				}
				if (list2.Count > 0)
				{
					FacingOrder facingOrder = FacingOrder.FacingOrderLookAtDirection(OrderController.GetOrderLookAtDirection(__instance.SelectedFormations, orderPosition.AsVec2));
					List<WorldPosition> list5;
					List<ValueTuple<Formation, int, float, WorldPosition, Vec2>> list6;
					Patch_OrderController.SimulateNewFacingOrderWithoutLockingFormations(list2, __instance.simulationFormations, OrderController.GetOrderLookAtDirection(__instance.SelectedFormations, orderPosition.AsVec2), flag, out list5, true, out list6);
					foreach (Formation formation2 in list2)
					{
						formation2.SetFacingOrder(facingOrder);
					}
					if (flag)
					{
						Patch_OrderTroopPlacer.AddOrderPositionEntities(list5, flag, num);
						num += list5.Count;
					}
				}
			}
			return true;
		}

		// Token: 0x06000382 RID: 898 RVA: 0x00013254 File Offset: 0x00011454
		public static IEnumerable<CodeInstruction> Transpile_SetOrderWithPosition(IEnumerable<CodeInstruction> instructions)
		{
			List<CodeInstruction> list = new List<CodeInstruction>(instructions);
			Patch_OrderController.FixingFormationFacingOrder(list);
			return list.AsEnumerable<CodeInstruction>();
		}

		// Token: 0x06000383 RID: 899 RVA: 0x00013268 File Offset: 0x00011468
		private static void FixingFormationFacingOrder(List<CodeInstruction> codes)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			int num = -1;
			int num2 = -1;
			int num3 = -1;
			int num4 = -1;
			for (int i = 0; i < codes.Count; i++)
			{
				if (!flag)
				{
					if (codes[i].opcode == OpCodes.Call && (codes[i].operand as MethodInfo).Name == "FacingOrderLookAtDirection")
					{
						flag = true;
						num = i;
					}
				}
				else if (!flag3)
				{
					if (codes[i].opcode == OpCodes.Callvirt && (codes[i].operand as MethodInfo).Name == "SetFacingOrder")
					{
						flag3 = true;
						num3 = i;
					}
				}
				else if (!flag4 && codes[i].opcode == OpCodes.Endfinally)
				{
					flag4 = true;
					num4 = i;
					break;
				}
			}
			if (!flag)
			{
				throw new Exception("FacingOrderLookAtDirection not found");
			}
			for (int j = num; j >= 0; j--)
			{
				if (!flag2 && codes[j].opcode == OpCodes.Call && (codes[j].operand as MethodInfo).Name == "get_SelectedFormations")
				{
					flag2 = true;
					num2 = j;
				}
			}
			if (!flag3)
			{
				throw new Exception("set_FacingOrderIndex not found");
			}
			if (!flag4)
			{
				throw new Exception("EndFinally not found");
			}
			codes[num2 - 1].opcode = OpCodes.Br_S;
			codes[num2 - 1].operand = codes[num3 + 4].operand;
			codes.RemoveRange(num2, num4 - num2 + 1);
		}

		// Token: 0x06000384 RID: 900 RVA: 0x0001341C File Offset: 0x0001161C
		private static Dictionary<Formation, Vec2> CollectFormationVirtualOrderPositions(IEnumerable<Formation> formations, out Vec2 weightedAverageOrderPosition, bool collectDirection, out Vec2 weightedAverageDirection)
		{
			Dictionary<Formation, Vec2> dictionary = new Dictionary<Formation, Vec2>();
			int num = 0;
			weightedAverageOrderPosition = Vec2.Zero;
			weightedAverageDirection = Vec2.Zero;
			foreach (Formation formation in formations)
			{
				Vec2 formationVirtualPositionVec = Patch_OrderController.GetFormationVirtualPositionVec2(formation);
				if (formationVirtualPositionVec.IsValid)
				{
					weightedAverageOrderPosition += formationVirtualPositionVec * (float)formation.CountOfUnitsWithoutDetachedOnes;
					num += formation.CountOfUnitsWithoutDetachedOnes;
				}
				dictionary.Add(formation, formationVirtualPositionVec);
			}
			if (num > 0)
			{
				weightedAverageOrderPosition = weightedAverageOrderPosition * 1f / (float)num;
			}
			if (collectDirection)
			{
				foreach (KeyValuePair<Formation, Vec2> keyValuePair in dictionary)
				{
					Formation key = keyValuePair.Key;
					Vec2 value = keyValuePair.Value;
					if (value.IsValid)
					{
						weightedAverageDirection += Patch_OrderController.GetFormationVirtualDirection(key) * (1f / MathF.Max(5f, value.DistanceSquared(weightedAverageOrderPosition)));
					}
				}
				weightedAverageDirection.Normalize();
			}
			return dictionary;
		}

		// Token: 0x06000385 RID: 901 RVA: 0x0001357C File Offset: 0x0001177C
		public static void SimulateNewFacingOrderWithoutLockingFormations(IEnumerable<Formation> formations, Dictionary<Formation, Formation> simulationFormations, Vec2 direction, bool isSimulatingAgentFrames, out List<WorldPosition> simulationAgentFrames, bool isSimulatingFormationChanges, out List<ValueTuple<Formation, int, float, WorldPosition, Vec2>> simulationFormationChanges)
		{
			simulationAgentFrames = ((!isSimulatingAgentFrames) ? null : new List<WorldPosition>());
			simulationFormationChanges = ((!isSimulatingFormationChanges) ? null : new List<ValueTuple<Formation, int, float, WorldPosition, Vec2>>());
			Vec2 vec;
			Vec2 vec2;
			Patch_OrderController.CollectFormationVirtualOrderPositions(formations, out vec, true, out vec2);
			foreach (Formation formation in formations)
			{
				Vec2 formationVirtualPositionVec = Patch_OrderController.GetFormationVirtualPositionVec2(formation);
				Vec2 vec3 = direction;
				float num = Patch_OrderController.GetFormationVirtualWidth(formation) ?? Patch_OrderController.GetActualOrCurrentWidth(formation);
				WorldPosition worldPosition = formation.CreateNewOrderWorldPosition(0);
				worldPosition.SetVec2(formationVirtualPositionVec);
				if (isSimulatingFormationChanges)
				{
					Patch_OrderController.LivePreviewFormationChanges.UpdateFormationChange(formation, null, new Vec2?(vec3), null, null);
					Patch_OrderController.LivePreviewFormationChanges.SetFacingOrder(15, formation, null);
				}
				int num2 = 0;
				int num3 = Patch_OrderController.GetFormationVirtualUnitSpacing(formation) ?? Patch_OrderController.GetActualOrCurrentUnitSpacing(formation);
				Patch_OrderController.DecreaseUnitSpacingAndWidthIfNotAllUnitsFit(formation, Patch_OrderController.GetSimulationFormation(formation, simulationFormations), in worldPosition, in vec3, ref num, ref num2, num3);
				float num4;
				Patch_OrderController.SimulateNewOrderWithFrameAndWidth(formation, Patch_OrderController.GetSimulationFormation(formation, simulationFormations), simulationAgentFrames, simulationFormationChanges, in worldPosition, in vec3, num, num2, true, out num4, num3);
				if (isSimulatingFormationChanges)
				{
					Patch_OrderController.LivePreviewFormationChanges.SetPreviewShape(formation, num, num4);
				}
			}
		}

		// Token: 0x06000386 RID: 902 RVA: 0x000136E8 File Offset: 0x000118E8
		public static void SimulateNewFacingOrderWithLockingFormations(IEnumerable<Formation> formations, Dictionary<Formation, Formation> simulationFormations, Vec2 direction, bool isSimulatingAgentFrames, out List<WorldPosition> simulationAgentFrames, bool isSimulatingFormationChanges, out List<ValueTuple<Formation, int, float, WorldPosition, Vec2>> simulationFormationChanges)
		{
			simulationAgentFrames = ((!isSimulatingAgentFrames) ? null : new List<WorldPosition>());
			simulationFormationChanges = ((!isSimulatingFormationChanges) ? null : new List<ValueTuple<Formation, int, float, WorldPosition, Vec2>>());
			Vec2 vec;
			Vec2 vec2;
			foreach (KeyValuePair<Formation, Vec2> keyValuePair in Patch_OrderController.CollectFormationVirtualOrderPositions(formations, out vec, true, out vec2))
			{
				Formation key = keyValuePair.Key;
				Vec2 vec3 = Patch_OrderController.rotateVector(keyValuePair.Value - vec, vec2, direction) + vec;
				Vec2 vec4 = Patch_OrderController.rotateVector(Patch_OrderController.GetFormationVirtualDirection(key), vec2, direction);
				float num = Patch_OrderController.GetFormationVirtualWidth(key) ?? Patch_OrderController.GetActualOrCurrentWidth(key);
				WorldPosition worldPosition = key.CreateNewOrderWorldPosition(0);
				worldPosition.SetVec2(vec3);
				if (isSimulatingFormationChanges)
				{
					Patch_OrderController.LivePreviewFormationChanges.UpdateFormationChange(key, new WorldPosition?(worldPosition), new Vec2?(vec4), null, null);
					Patch_OrderController.LivePreviewFormationChanges.SetFacingOrder(15, key, null);
				}
				if (!Mission.Current.IsPositionInsideBoundaries(worldPosition.AsVec2))
				{
					Vec2 closestBoundaryPosition = Mission.Current.GetClosestBoundaryPosition(worldPosition.AsVec2);
					worldPosition.SetVec2(closestBoundaryPosition);
				}
				int num2 = 0;
				int num3 = Patch_OrderController.GetFormationVirtualUnitSpacing(key) ?? Patch_OrderController.GetActualOrCurrentUnitSpacing(key);
				Patch_OrderController.DecreaseUnitSpacingAndWidthIfNotAllUnitsFit(key, Patch_OrderController.GetSimulationFormation(key, simulationFormations), in worldPosition, in vec4, ref num, ref num2, num3);
				float num4;
				Patch_OrderController.SimulateNewOrderWithFrameAndWidth(key, Patch_OrderController.GetSimulationFormation(key, simulationFormations), simulationAgentFrames, simulationFormationChanges, in worldPosition, in vec4, num, num2, true, out num4, num3);
				if (isSimulatingFormationChanges)
				{
					Patch_OrderController.LivePreviewFormationChanges.SetPreviewShape(key, num, num4);
				}
			}
		}

		// Token: 0x06000387 RID: 903 RVA: 0x000138BC File Offset: 0x00011ABC
		public static void SimulateFacingToEnemyOrder(IEnumerable<Formation> formations, Dictionary<Formation, Formation> simulationFormations, Formation targetFormation, bool isSimulatingAgentFrames, out List<WorldPosition> simulationAgentFrames, bool isSimulatingFormationChanges, out List<ValueTuple<Formation, int, float, WorldPosition, Vec2>> simulationFormationChanges)
		{
			simulationAgentFrames = ((!isSimulatingAgentFrames) ? null : new List<WorldPosition>());
			simulationFormationChanges = ((!isSimulatingFormationChanges) ? null : new List<ValueTuple<Formation, int, float, WorldPosition, Vec2>>());
			Vec2 vec;
			Vec2 vec2;
			Patch_OrderController.CollectFormationVirtualOrderPositions(formations, out vec, true, out vec2);
			foreach (Formation formation in formations)
			{
				Vec2 formationVirtualPositionVec = Patch_OrderController.GetFormationVirtualPositionVec2(formation);
				float num = Patch_OrderController.GetFormationVirtualWidth(formation) ?? Patch_OrderController.GetActualOrCurrentWidth(formation);
				WorldPosition worldPosition = formation.CreateNewOrderWorldPosition(0);
				worldPosition.SetVec2(formationVirtualPositionVec);
				Vec2 formationVirtualDirectionIncludingFacingEnemyAccordingToPositionAndDirection = Patch_OrderController.GetFormationVirtualDirectionIncludingFacingEnemyAccordingToPositionAndDirection(formation, formationVirtualPositionVec, Patch_OrderController.GetFormationVirtualDirection(formation));
				if (isSimulatingFormationChanges)
				{
					Patch_OrderController.LivePreviewFormationChanges.SetFacingOrder(14, formation, targetFormation);
				}
				Vec2 virtualDirectionOfFacingEnemyAccordingToPostitionAndDirection = Patch_OrderController.GetVirtualDirectionOfFacingEnemyAccordingToPostitionAndDirection(formation, formationVirtualPositionVec, formationVirtualDirectionIncludingFacingEnemyAccordingToPositionAndDirection);
				int num2 = 0;
				int num3 = Patch_OrderController.GetFormationVirtualUnitSpacing(formation) ?? Patch_OrderController.GetActualOrCurrentUnitSpacing(formation);
				Patch_OrderController.DecreaseUnitSpacingAndWidthIfNotAllUnitsFit(formation, Patch_OrderController.GetSimulationFormation(formation, simulationFormations), in worldPosition, in virtualDirectionOfFacingEnemyAccordingToPostitionAndDirection, ref num, ref num2, num3);
				float num4;
				Patch_OrderController.SimulateNewOrderWithFrameAndWidth(formation, Patch_OrderController.GetSimulationFormation(formation, simulationFormations), simulationAgentFrames, simulationFormationChanges, in worldPosition, in virtualDirectionOfFacingEnemyAccordingToPostitionAndDirection, num, num2, true, out num4, num3);
				if (isSimulatingFormationChanges)
				{
					Patch_OrderController.LivePreviewFormationChanges.SetPreviewShape(formation, num, num4);
				}
			}
		}

		// Token: 0x06000388 RID: 904 RVA: 0x00013A08 File Offset: 0x00011C08
		public static void SimulateAgentFrames(IEnumerable<Formation> formations, Dictionary<Formation, Formation> simulationFormations, out List<WorldPosition> simulationAgentFrames)
		{
			simulationAgentFrames = new List<WorldPosition>();
			List<ValueTuple<Formation, int, float, WorldPosition, Vec2>> list = null;
			Vec2 vec;
			Vec2 vec2;
			Patch_OrderController.CollectFormationVirtualOrderPositions(formations, out vec, true, out vec2);
			foreach (Formation formation in formations)
			{
				WorldPosition formationVirtualPosition = Patch_OrderController.GetFormationVirtualPosition(formation);
				Vec2 formationVirtualDirection = Patch_OrderController.GetFormationVirtualDirection(formation);
				float num = Patch_OrderController.GetFormationVirtualWidth(formation) ?? Patch_OrderController.GetActualOrCurrentWidth(formation);
				int num2 = Patch_OrderController.GetFormationVirtualUnitSpacing(formation) ?? Patch_OrderController.GetActualOrCurrentUnitSpacing(formation);
				int num3 = 0;
				Patch_OrderController.DecreaseUnitSpacingAndWidthIfNotAllUnitsFit(formation, Patch_OrderController.GetSimulationFormation(formation, simulationFormations), in formationVirtualPosition, in formationVirtualDirection, ref num, ref num3, num2);
				float num4;
				Patch_OrderController.SimulateNewOrderWithFrameAndWidth(formation, Patch_OrderController.GetSimulationFormation(formation, simulationFormations), simulationAgentFrames, list, in formationVirtualPosition, in formationVirtualDirection, num, num3, false, out num4, num2);
			}
		}

		// Token: 0x06000389 RID: 905 RVA: 0x00013AFC File Offset: 0x00011CFC
		private static Vec2 rotateVector(Vec2 input, Vec2 from, Vec2 to)
		{
			float num = from.x * to.x + from.y * to.y;
			float num2 = from.x * to.y - from.y * to.x;
			return new Vec2(input.x * num - input.y * num2, input.x * num2 + input.y * num);
		}

		// Token: 0x0600038A RID: 906 RVA: 0x00013B68 File Offset: 0x00011D68
		public static Vec2 GetFormationVirtualPositionVec2(Formation formation)
		{
			if (Patch_OrderController.LivePreviewFormationChanges.VirtualChanges.ContainsKey(formation) && Patch_OrderController.LivePreviewFormationChanges.VirtualChanges[formation].Position != null)
			{
				return Patch_OrderController.LivePreviewFormationChanges.VirtualChanges[formation].Position.Value;
			}
			if (!formation.OrderPosition.IsValid)
			{
				return formation.CurrentPosition;
			}
			return formation.OrderPosition;
		}

		// Token: 0x0600038B RID: 907 RVA: 0x00013BE8 File Offset: 0x00011DE8
		public static WorldPosition GetFormationVirtualPosition(Formation formation)
		{
			if (Patch_OrderController.LivePreviewFormationChanges.VirtualChanges.ContainsKey(formation))
			{
				FormationChange formationChange = Patch_OrderController.LivePreviewFormationChanges.VirtualChanges[formation];
				if (formationChange.WorldPosition != null)
				{
					formationChange = Patch_OrderController.LivePreviewFormationChanges.VirtualChanges[formation];
					return formationChange.WorldPosition.Value;
				}
			}
			if (!formation.OrderPosition.IsValid)
			{
				return Patch_OrderController.GetFormationCurrentPositionAsWorldPosition(formation);
			}
			return formation.CreateNewOrderWorldPosition(0);
		}

		// Token: 0x0600038C RID: 908 RVA: 0x00013C64 File Offset: 0x00011E64
		private static WorldPosition GetFormationCurrentPositionAsWorldPosition(Formation formation)
		{
			WorldPosition cachedMedianPosition = formation.CachedMedianPosition;
			Vec2 currentPosition = formation.CurrentPosition;
			cachedMedianPosition.SetVec2(currentPosition);
			return cachedMedianPosition;
		}

		// Token: 0x0600038D RID: 909 RVA: 0x00013C88 File Offset: 0x00011E88
		public static Vec2 GetFormationVirtualDirection(Formation formation)
		{
			if (Patch_OrderController.LivePreviewFormationChanges.VirtualChanges.ContainsKey(formation))
			{
				FormationChange formationChange = Patch_OrderController.LivePreviewFormationChanges.VirtualChanges[formation];
				if (formationChange.Direciton != null)
				{
					formationChange = Patch_OrderController.LivePreviewFormationChanges.VirtualChanges[formation];
					return formationChange.Direciton.Value;
				}
			}
			return formation.Direction;
		}

		// Token: 0x0600038E RID: 910 RVA: 0x00013CEA File Offset: 0x00011EEA
		public static Vec2 GetFormationVirtualDirectionIncludingFacingEnemyAccordingToPositionAndDirection(Formation formation, Vec2 virtualFormationPositionVec2, Vec2 virtualFormationDirection)
		{
			if (Patch_OrderController.GetFormationVirtualFacingOrder(formation) == 14)
			{
				return Patch_OrderController.GetVirtualDirectionOfFacingEnemyAccordingToPostitionAndDirection(formation, virtualFormationPositionVec2, virtualFormationDirection);
			}
			return Patch_OrderController.GetFormationVirtualDirection(formation);
		}

		// Token: 0x0600038F RID: 911 RVA: 0x00013D08 File Offset: 0x00011F08
		public static Vec2 GetFormationVirtualDirectionWhenFollowingAgent(Formation formation, Agent targetAgent)
		{
			if (TroopClassExtensions.IsMounted(formation.PhysicalClass) && targetAgent != null)
			{
				Vec2 vec = Patch_OrderController.FollowingAgentDirection(targetAgent);
				if (vec.IsValid)
				{
					return vec;
				}
			}
			return Patch_OrderController.GetFormationVirtualDirection(formation);
		}

		// Token: 0x06000390 RID: 912 RVA: 0x00013D40 File Offset: 0x00011F40
		private static Vec2 FollowingAgentDirection(Agent targetAgent)
		{
			Vec3 velocity = targetAgent.Velocity;
			float maximumForwardUnlimitedSpeed = targetAgent.GetMaximumForwardUnlimitedSpeed();
			if ((double)velocity.LengthSquared > (double)maximumForwardUnlimitedSpeed * (double)maximumForwardUnlimitedSpeed * 0.09000000357627869)
			{
				return targetAgent.Velocity.AsVec2.Normalized();
			}
			return Vec2.Invalid;
		}

		// Token: 0x06000391 RID: 913 RVA: 0x00013D90 File Offset: 0x00011F90
		private static void TryIntializeFormationChanges(Formation formation)
		{
			FormationChange formationChange;
			bool flag = Patch_OrderController.LivePreviewFormationChanges.VirtualChanges.TryGetValue(formation, out formationChange);
			if (!flag || formationChange.Position == null)
			{
				Patch_OrderController.LivePreviewFormationChanges.UpdateFormationChange(formation, new WorldPosition?(formation.CreateNewOrderWorldPosition(0)), null, null, null);
			}
			if (!flag || formationChange.Direciton == null)
			{
				Patch_OrderController.LivePreviewFormationChanges.UpdateFormationChange(formation, null, new Vec2?(formation.Direction), null, null);
			}
		}

		// Token: 0x06000392 RID: 914 RVA: 0x00013E38 File Offset: 0x00012038
		public static int? GetFormationVirtualUnitSpacing(Formation formation)
		{
			if (!Patch_OrderController.LivePreviewFormationChanges.VirtualChanges.ContainsKey(formation) || Patch_OrderController.LivePreviewFormationChanges.VirtualChanges[formation].UnitSpacing == null)
			{
				return null;
			}
			return Patch_OrderController.LivePreviewFormationChanges.VirtualChanges[formation].UnitSpacing;
		}

		// Token: 0x06000393 RID: 915 RVA: 0x00013E95 File Offset: 0x00012095
		public static int GetFormationVirtualNaturalUnitSpacing(Formation formation)
		{
			return ArrangementOrder.GetUnitSpacingOf(Patch_OrderController.GetFormationVirtualArrangementOrder(formation));
		}

		// Token: 0x06000394 RID: 916 RVA: 0x00013EA4 File Offset: 0x000120A4
		public static float? GetFormationVirtualWidth(Formation formation)
		{
			if (!Patch_OrderController.LivePreviewFormationChanges.VirtualChanges.ContainsKey(formation) || Patch_OrderController.LivePreviewFormationChanges.VirtualChanges[formation].Width == null)
			{
				return null;
			}
			return Patch_OrderController.LivePreviewFormationChanges.VirtualChanges[formation].Width;
		}

		// Token: 0x06000395 RID: 917 RVA: 0x00013F04 File Offset: 0x00012104
		public static void SetFormationVirtualWidth(Formation formation, float width)
		{
			Patch_OrderController.LivePreviewFormationChanges.UpdateFormationChange(formation, null, null, null, new float?(width));
		}

		// Token: 0x06000396 RID: 918 RVA: 0x00013F40 File Offset: 0x00012140
		public static WorldPosition GetFormationVirtualMedianPosition(Formation formation)
		{
			WorldPosition formationVirtualPosition = Patch_OrderController.GetFormationVirtualPosition(formation);
			Vec2 formationVirtualDirection = Patch_OrderController.GetFormationVirtualDirection(formation);
			formationVirtualPosition.SetVec2(formationVirtualPosition.AsVec2 + formationVirtualDirection.TransformToParentUnitF(formation.OrderLocalAveragePosition));
			return formationVirtualPosition;
		}

		// Token: 0x06000397 RID: 919 RVA: 0x00013F7C File Offset: 0x0001217C
		public static Vec2 GetFormationVirtualAveragePositionVec2(Formation formation)
		{
			return Patch_OrderController.GetFormationVirtualPositionVec2(formation) + Patch_OrderController.GetFormationVirtualDirection(formation).TransformToParentUnitF(formation.OrderLocalAveragePosition);
		}

		// Token: 0x06000398 RID: 920 RVA: 0x00013FA8 File Offset: 0x000121A8
		public static void GetFormationVirtualShape(Formation formation, out float width, out float depth, out float rightSideOffset)
		{
			FormationChange formationChange;
			if (Patch_OrderController.LivePreviewFormationChanges.VirtualChanges.TryGetValue(formation, out formationChange) && formationChange.PreviewWidth != null && formationChange.PreviewDepth != null)
			{
				width = formationChange.PreviewWidth.Value;
				depth = formationChange.PreviewDepth.Value;
				rightSideOffset = Patch_OrderController.GetRightSideOffset(formation);
				return;
			}
			width = formation.Width;
			depth = formation.Depth;
			rightSideOffset = Patch_OrderController.GetRightSideOffset(formation);
		}

		// Token: 0x06000399 RID: 921 RVA: 0x00014024 File Offset: 0x00012224
		private static float GetRightSideOffset(Formation formation)
		{
			if (formation.Arrangement.RankCount <= 1)
			{
				return 0f;
			}
			ArrangementOrder.ArrangementOrderEnum formationVirtualArrangementOrder = Patch_OrderController.GetFormationVirtualArrangementOrder(formation);
			if (formationVirtualArrangementOrder == 2 || formationVirtualArrangementOrder == 3 || formationVirtualArrangementOrder == 5)
			{
				return (RTSCamera.CommandSystem.Utilities.Utility.GetFormationInterval(formation, Patch_OrderController.GetFormationVirtualUnitSpacing(formation) ?? formation.UnitSpacing) + formation.UnitDiameter) / 2f;
			}
			return 0f;
		}

		// Token: 0x0600039A RID: 922 RVA: 0x00014090 File Offset: 0x00012290
		public unsafe static OrderType GetFormationVirtualMovementorder(Formation formation)
		{
			if (Patch_OrderController.LivePreviewFormationChanges.VirtualChanges.ContainsKey(formation))
			{
				FormationChange formationChange = Patch_OrderController.LivePreviewFormationChanges.VirtualChanges[formation];
				if (formationChange.MovementOrderType != null)
				{
					formationChange = Patch_OrderController.LivePreviewFormationChanges.VirtualChanges[formation];
					return formationChange.MovementOrderType.Value;
				}
			}
			MovementOrder movementOrder = *formation.GetReadonlyMovementOrderReference();
			return movementOrder.OrderType;
		}

		// Token: 0x0600039B RID: 923 RVA: 0x00014100 File Offset: 0x00012300
		public static OrderType GetFormationVirtualFacingOrder(Formation formation)
		{
			if (Patch_OrderController.LivePreviewFormationChanges.VirtualChanges.ContainsKey(formation))
			{
				FormationChange formationChange = Patch_OrderController.LivePreviewFormationChanges.VirtualChanges[formation];
				if (formationChange.FacingOrderType != null)
				{
					formationChange = Patch_OrderController.LivePreviewFormationChanges.VirtualChanges[formation];
					return formationChange.FacingOrderType.Value;
				}
			}
			return formation.FacingOrder.OrderType;
		}

		// Token: 0x0600039C RID: 924 RVA: 0x0001416C File Offset: 0x0001236C
		public static ArrangementOrder.ArrangementOrderEnum GetFormationVirtualArrangementOrder(Formation formation)
		{
			if (Patch_OrderController.LivePreviewFormationChanges.VirtualChanges.ContainsKey(formation))
			{
				FormationChange formationChange = Patch_OrderController.LivePreviewFormationChanges.VirtualChanges[formation];
				if (formationChange.ArrangementOrder != null)
				{
					formationChange = Patch_OrderController.LivePreviewFormationChanges.VirtualChanges[formation];
					return formationChange.ArrangementOrder.Value;
				}
			}
			return formation.ArrangementOrder.OrderEnum;
		}

		// Token: 0x0600039D RID: 925 RVA: 0x000141D3 File Offset: 0x000123D3
		private static FormationQuerySystem GetTargetOrClosestEnemyFormationQuerySystem(Formation f, Formation targetFormation)
		{
			FormationQuerySystem formationQuerySystem;
			if ((formationQuerySystem = ((targetFormation != null) ? targetFormation.QuerySystem : null)) == null)
			{
				Formation closestEnemyFormation = CommandQuerySystem.GetQueryForFormation(f).ClosestEnemyFormation;
				if (closestEnemyFormation == null)
				{
					return null;
				}
				formationQuerySystem = closestEnemyFormation.QuerySystem;
			}
			return formationQuerySystem;
		}

		// Token: 0x0600039E RID: 926 RVA: 0x000141FC File Offset: 0x000123FC
		public static Formation GetFormationVirtualTargetFormation(Formation formation)
		{
			if (!Patch_OrderController.LivePreviewFormationChanges.VirtualChanges.ContainsKey(formation) || Patch_OrderController.LivePreviewFormationChanges.VirtualChanges[formation].TargetFormation == null)
			{
				return null;
			}
			return Patch_OrderController.LivePreviewFormationChanges.VirtualChanges[formation].TargetFormation;
		}

		// Token: 0x0600039F RID: 927 RVA: 0x0001424C File Offset: 0x0001244C
		public static Agent GetFormationVirtualTargetAgent(Formation formation)
		{
			if (!Patch_OrderController.LivePreviewFormationChanges.VirtualChanges.ContainsKey(formation) || Patch_OrderController.LivePreviewFormationChanges.VirtualChanges[formation].TargetAgent == null)
			{
				return null;
			}
			return Patch_OrderController.LivePreviewFormationChanges.VirtualChanges[formation].TargetAgent;
		}

		// Token: 0x060003A0 RID: 928 RVA: 0x0001429C File Offset: 0x0001249C
		public static OrderType GetFormationVirtualRidingOrder(Formation formation)
		{
			if (Patch_OrderController.LivePreviewFormationChanges.VirtualChanges.ContainsKey(formation))
			{
				FormationChange formationChange = Patch_OrderController.LivePreviewFormationChanges.VirtualChanges[formation];
				if (formationChange.RidingOrderType != null)
				{
					formationChange = Patch_OrderController.LivePreviewFormationChanges.VirtualChanges[formation];
					return formationChange.RidingOrderType.Value;
				}
			}
			return formation.RidingOrder.OrderType;
		}

		// Token: 0x060003A1 RID: 929 RVA: 0x00014306 File Offset: 0x00012506
		public static void SaveFormationLivePositionForPreview(Formation formation, WorldPosition? medianPosition)
		{
			Patch_OrderController._currentMovingTarget[formation] = new Patch_OrderController.MovingTarget
			{
				MedianPosition = medianPosition
			};
		}

		// Token: 0x060003A2 RID: 930 RVA: 0x0001431F File Offset: 0x0001251F
		public static void ClearFormationLivePositionForPreview(Formation formation)
		{
			if (Patch_OrderController._currentMovingTarget.ContainsKey(formation))
			{
				Patch_OrderController._currentMovingTarget.Remove(formation);
			}
		}

		// Token: 0x060003A3 RID: 931 RVA: 0x0001433C File Offset: 0x0001253C
		public static void GetFormationMovingTargetForPreview(Formation formation, out WorldPosition? medianPosition, WorldPosition? defaultPosition = null)
		{
			Patch_OrderController.MovingTarget movingTarget;
			if (Patch_OrderController._currentMovingTarget.TryGetValue(formation, out movingTarget))
			{
				WorldPosition? medianPosition2 = movingTarget.MedianPosition;
				medianPosition = ((medianPosition2 != null) ? medianPosition2 : defaultPosition);
				return;
			}
			medianPosition = defaultPosition;
		}

		// Token: 0x060003A4 RID: 932 RVA: 0x0001437C File Offset: 0x0001257C
		public static Vec2 GetAdvanceOrFallbackEnemyDirection(Formation f, Formation targetFormation)
		{
			FormationQuerySystem targetOrClosestEnemyFormationQuerySystem = Patch_OrderController.GetTargetOrClosestEnemyFormationQuerySystem(f, targetFormation);
			WorldPosition? worldPosition;
			Patch_OrderController.GetFormationMovingTargetForPreview(f, out worldPosition, null);
			if (targetOrClosestEnemyFormationQuerySystem != null)
			{
				return (targetOrClosestEnemyFormationQuerySystem.Formation.CachedMedianPosition.AsVec2 - ((worldPosition != null) ? worldPosition.GetValueOrDefault().AsVec2 : f.CachedAveragePosition)).Normalized();
			}
			return Vec2.Forward;
		}

		// Token: 0x060003A5 RID: 933 RVA: 0x000143EC File Offset: 0x000125EC
		public unsafe static WorldPosition GetAdvanceOrderPosition(Formation f, WorldPosition.WorldPositionEnforcedCache worldPositionEnforcedCache, Formation targetFormation)
		{
			if (Patch_OrderController._engageTargetPositionCache != null)
			{
				WorldPosition worldPosition = (WorldPosition)Patch_OrderController._engageTargetPositionCache.GetValue(*f.GetReadonlyMovementOrderReference());
				if (worldPosition.IsValid)
				{
					return worldPosition;
				}
			}
			FormationQuerySystem querySystem = f.QuerySystem;
			FormationQuerySystem targetOrClosestEnemyFormationQuerySystem = Patch_OrderController.GetTargetOrClosestEnemyFormationQuerySystem(f, targetFormation);
			WorldPosition worldPosition2;
			if (targetOrClosestEnemyFormationQuerySystem == null)
			{
				Agent closestEnemyAgent = CommandQuerySystem.GetQueryForFormation(f).ClosestEnemyAgent;
				if (closestEnemyAgent == null)
				{
					return f.CreateNewOrderWorldPosition(worldPositionEnforcedCache);
				}
				worldPosition2 = closestEnemyAgent.GetWorldPosition();
			}
			else
			{
				worldPosition2 = targetOrClosestEnemyFormationQuerySystem.Formation.CachedMedianPosition;
			}
			if (querySystem.IsRangedFormation || querySystem.IsRangedCavalryFormation || querySystem.HasThrowing)
			{
				Vec2 advanceOrFallbackEnemyDirection = Patch_OrderController.GetAdvanceOrFallbackEnemyDirection(f, targetFormation);
				worldPosition2.SetVec2(worldPosition2.AsVec2 - advanceOrFallbackEnemyDirection * querySystem.MissileRangeAdjusted);
			}
			else if (targetOrClosestEnemyFormationQuerySystem != null)
			{
				WorldPosition? worldPosition3;
				Patch_OrderController.GetFormationMovingTargetForPreview(f, out worldPosition3, null);
				Vec2 vec = (targetOrClosestEnemyFormationQuerySystem.Formation.CachedAveragePosition - ((worldPosition3 != null) ? worldPosition3.GetValueOrDefault().AsVec2 : f.CachedAveragePosition)).Normalized();
				float num = 2f;
				if ((double)targetOrClosestEnemyFormationQuerySystem.FormationPower < (double)f.QuerySystem.FormationPower * 0.20000000298023224)
				{
					num = 0.1f;
				}
				worldPosition2.SetVec2(worldPosition2.AsVec2 - vec * num);
			}
			return worldPosition2;
		}

		// Token: 0x060003A6 RID: 934 RVA: 0x00014558 File Offset: 0x00012758
		public static WorldPosition GetFallbackOrderPosition(Formation f, WorldPosition.WorldPositionEnforcedCache worldPositionEnforcedCache, Formation targetFormation)
		{
			Vec2 advanceOrFallbackEnemyDirection = Patch_OrderController.GetAdvanceOrFallbackEnemyDirection(f, targetFormation);
			WorldPosition? worldPosition;
			Patch_OrderController.GetFormationMovingTargetForPreview(f, out worldPosition, null);
			WorldPosition worldPosition2 = worldPosition ?? f.CachedMedianPosition;
			worldPosition2.SetVec2(((worldPosition != null) ? worldPosition.GetValueOrDefault().AsVec2 : f.CachedAveragePosition) - advanceOrFallbackEnemyDirection * 7f);
			return worldPosition2;
		}

		// Token: 0x060003A7 RID: 935 RVA: 0x000145D4 File Offset: 0x000127D4
		public static Vec2 GetVirtualDirectionOfFacingEnemyAccordingToPostitionAndDirection(Formation f, Vec2 virtualFormationPositionVec2, Vec2 virtualFormationDirection)
		{
			Agent formationVirtualTargetAgent = Patch_OrderController.GetFormationVirtualTargetAgent(f);
			if (TroopClassExtensions.IsMounted(f.PhysicalClass) && formationVirtualTargetAgent != null)
			{
				Vec2 vec = Patch_OrderController.FollowingAgentDirection(formationVirtualTargetAgent);
				if (vec.IsValid)
				{
					return vec;
				}
			}
			ArrangementOrder.ArrangementOrderEnum formationVirtualArrangementOrder = Patch_OrderController.GetFormationVirtualArrangementOrder(f);
			if (formationVirtualArrangementOrder == null || formationVirtualArrangementOrder == 7)
			{
				return f.Direction;
			}
			Formation virtualFacingEnemyTargetFormation = Patch_OrderController.GetVirtualFacingEnemyTargetFormation(f);
			if (virtualFacingEnemyTargetFormation != null)
			{
				return Patch_FacingOrder.GetVirtualDirectionFacingToEnemyFormation(f, virtualFacingEnemyTargetFormation);
			}
			Vec2 virtualWeightedAverageEnemyPosition = CommandQuerySystem.GetQueryForFormation(f).VirtualWeightedAverageEnemyPosition;
			return Patch_FacingOrder.GetDirectionFacingToEnemy(f, virtualFormationPositionVec2, virtualFormationDirection, virtualWeightedAverageEnemyPosition);
		}

		// Token: 0x060003A8 RID: 936 RVA: 0x00014648 File Offset: 0x00012848
		public static WorldPosition GetFollowOrderPosition(Formation f, Agent targetAgent)
		{
			float length = targetAgent.GetCurrentVelocity().Length;
			Vec2 vec = Vec2.Zero;
			WorldPosition worldPosition = targetAgent.GetWorldPosition();
			WorldPosition worldPosition2 = worldPosition;
			if (length < 0.01f || length < targetAgent.Monster.WalkingSpeedLimit * 0.7f)
			{
				if (targetAgent.MountAgent != null)
				{
					vec += f.Direction * -2f;
				}
				worldPosition2.SetVec2(worldPosition2.AsVec2 - f.GetMiddleFrontUnitPositionOffset() + vec);
				float num = (TroopClassExtensions.IsMounted(f.PhysicalClass) ? 4f : 2.5f);
				if (Mission.Current.IsTeleportingAgents || worldPosition2.AsVec2.DistanceSquared(worldPosition.AsVec2) > num * num)
				{
					worldPosition = worldPosition2;
				}
			}
			else
			{
				if (TroopClassExtensions.IsMounted(f.PhysicalClass))
				{
					vec += 2f * targetAgent.Velocity.AsVec2;
				}
				worldPosition2.SetVec2(worldPosition2.AsVec2 - f.GetMiddleFrontUnitPositionOffset() + vec);
				worldPosition = worldPosition2;
			}
			return worldPosition;
		}

		// Token: 0x060003A9 RID: 937 RVA: 0x0001476C File Offset: 0x0001296C
		public static Vec2 GetFollowEntityDirection(Formation f, GameEntity gameEntity)
		{
			return gameEntity.GetGlobalFrame().rotation.f.AsVec2.Normalized();
		}

		// Token: 0x060003AA RID: 938 RVA: 0x0001479C File Offset: 0x0001299C
		public static WorldPosition GetFollowEntityOrderPosition(Formation f, GameEntity targetEntity)
		{
			WorldPosition worldPosition;
			worldPosition..ctor(Mission.Current.Scene, UIntPtr.Zero, targetEntity.GlobalPosition, false);
			worldPosition.SetVec2(worldPosition.AsVec2);
			return worldPosition;
		}

		// Token: 0x060003AB RID: 939 RVA: 0x000147D8 File Offset: 0x000129D8
		public static WorldPosition GetAttackEntityWaitPosition(Formation formation, GameEntity targetEntity)
		{
			Scene scene = formation.Team.Mission.Scene;
			WorldPosition worldPosition;
			worldPosition..ctor(scene, UIntPtr.Zero, targetEntity.GlobalPosition, false);
			Vec2 vec = formation.CachedAveragePosition - worldPosition.AsVec2;
			MatrixFrame matrixFrame = targetEntity.GetGlobalFrame();
			Vec2 vec2 = matrixFrame.rotation.f.AsVec2.Normalized();
			Vec2 vec3 = (((double)vec.DotProduct(vec2) >= 0.0) ? vec2 : (-vec2));
			WorldPosition worldPosition2 = worldPosition;
			worldPosition2.SetVec2(worldPosition.AsVec2 + vec3 * 3f);
			if (scene.DoesPathExistBetweenPositions(worldPosition2, formation.CachedMedianPosition))
			{
				return worldPosition2;
			}
			WorldPosition worldPosition3 = worldPosition;
			worldPosition3.SetVec2(worldPosition.AsVec2 - vec3 * 3f);
			if (scene.DoesPathExistBetweenPositions(worldPosition3, formation.CachedMedianPosition))
			{
				return worldPosition3;
			}
			WorldPosition worldPosition4 = worldPosition;
			Vec2 asVec = worldPosition.AsVec2;
			matrixFrame = targetEntity.GetGlobalFrame();
			worldPosition4.SetVec2(asVec + matrixFrame.rotation.s.AsVec2.Normalized() * 3f);
			if (scene.DoesPathExistBetweenPositions(worldPosition4, formation.CachedMedianPosition))
			{
				return worldPosition4;
			}
			WorldPosition worldPosition5 = worldPosition;
			Vec2 asVec2 = worldPosition.AsVec2;
			matrixFrame = targetEntity.GetGlobalFrame();
			worldPosition5.SetVec2(asVec2 - matrixFrame.rotation.s.AsVec2.Normalized() * 3f);
			if (!scene.DoesPathExistBetweenPositions(worldPosition5, formation.CachedMedianPosition))
			{
				return worldPosition2;
			}
			return worldPosition5;
		}

		// Token: 0x060003AC RID: 940 RVA: 0x00014978 File Offset: 0x00012B78
		public static void FillOrderLookingAtPosition(OrderInQueue order, OrderController orderController, WorldPosition position)
		{
			List<Formation> list = orderController.SelectedFormations.Where<Formation>((Formation f) => f.CountOfUnitsWithoutDetachedOnes > 0).ToList<Formation>();
			List<Formation> list2 = new List<Formation>();
			List<Formation> list3 = new List<Formation>();
			foreach (Formation formation in list)
			{
				if (RTSCamera.CommandSystem.Utilities.Utility.ShouldLockFormationDuringLookAtDirection(formation))
				{
					list2.Add(formation);
				}
				else
				{
					list3.Add(formation);
				}
			}
			if (list2.Count > 0)
			{
				List<WorldPosition> list4;
				List<ValueTuple<Formation, int, float, WorldPosition, Vec2>> list5;
				Patch_OrderController.SimulateNewFacingOrderWithLockingFormations(list2, orderController.simulationFormations, OrderController.GetOrderLookAtDirection(list, position.AsVec2), false, out list4, true, out list5);
				order.ActualFormationChanges.AddRange(list5);
				foreach (KeyValuePair<Formation, FormationChange> keyValuePair in Patch_OrderController.LivePreviewFormationChanges.CollectChanges(list2))
				{
					order.ShouldLockFormationInFacingOrder[keyValuePair.Key] = true;
					order.VirtualFormationChanges[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			if (list3.Count > 0)
			{
				order.PositionBegin = position;
				List<WorldPosition> list4;
				List<ValueTuple<Formation, int, float, WorldPosition, Vec2>> list6;
				Patch_OrderController.SimulateNewFacingOrderWithoutLockingFormations(list3, orderController.simulationFormations, OrderController.GetOrderLookAtDirection(list, position.AsVec2), false, out list4, true, out list6);
				order.ActualFormationChanges.AddRange(list6);
				foreach (KeyValuePair<Formation, FormationChange> keyValuePair2 in Patch_OrderController.LivePreviewFormationChanges.CollectChanges(list3))
				{
					order.ShouldLockFormationInFacingOrder[keyValuePair2.Key] = false;
					order.VirtualFormationChanges[keyValuePair2.Key] = keyValuePair2.Value;
				}
			}
		}

		// Token: 0x060003AD RID: 941 RVA: 0x00014B6C File Offset: 0x00012D6C
		public static void FillOrderLookingAtEnemy(OrderInQueue order, OrderController orderController, Formation targetFormation)
		{
			order.TargetFormation = targetFormation;
			List<Formation> list = orderController.SelectedFormations.Where<Formation>((Formation f) => f.CountOfUnitsWithoutDetachedOnes > 0).ToList<Formation>();
			if (list.Count > 0)
			{
				List<WorldPosition> list2;
				List<ValueTuple<Formation, int, float, WorldPosition, Vec2>> list3;
				Patch_OrderController.SimulateFacingToEnemyOrder(list, orderController.simulationFormations, targetFormation, false, out list2, true, out list3);
				order.ActualFormationChanges.AddRange(list3);
				foreach (KeyValuePair<Formation, FormationChange> keyValuePair in Patch_OrderController.LivePreviewFormationChanges.CollectChanges(list))
				{
					order.ShouldLockFormationInFacingOrder[keyValuePair.Key] = false;
					order.VirtualFormationChanges[keyValuePair.Key] = keyValuePair.Value;
				}
			}
		}

		// Token: 0x060003AE RID: 942 RVA: 0x00014C4C File Offset: 0x00012E4C
		public static void SimulateNewArrangementOrder(IEnumerable<Formation> formations, Dictionary<Formation, Formation> simulationFormations, ArrangementOrder.ArrangementOrderEnum newArrangementOrder, bool isSimulatingAgentFrames, out List<WorldPosition> simulationAgentFrames, bool isSimulatingFormationChanges, out List<ValueTuple<Formation, int, float, WorldPosition, Vec2>> simulationFormationChanges)
		{
			simulationAgentFrames = ((!isSimulatingAgentFrames) ? null : new List<WorldPosition>());
			simulationFormationChanges = ((!isSimulatingFormationChanges) ? null : new List<ValueTuple<Formation, int, float, WorldPosition, Vec2>>());
			foreach (Formation formation in formations)
			{
				ArrangementOrder.ArrangementOrderEnum formationVirtualArrangementOrder = Patch_OrderController.GetFormationVirtualArrangementOrder(formation);
				Vec2 formationVirtualPositionVec = Patch_OrderController.GetFormationVirtualPositionVec2(formation);
				Vec2 formationVirtualDirectionIncludingFacingEnemyAccordingToPositionAndDirection = Patch_OrderController.GetFormationVirtualDirectionIncludingFacingEnemyAccordingToPositionAndDirection(formation, formationVirtualPositionVec, Patch_OrderController.GetFormationVirtualDirection(formation));
				int num = Patch_OrderController.GetFormationVirtualUnitSpacing(formation) ?? Patch_OrderController.GetActualOrCurrentUnitSpacing(formation);
				int unitSpacingOf = ArrangementOrder.GetUnitSpacingOf(newArrangementOrder);
				float num2 = Patch_OrderController.GetFormationVirtualWidth(formation) ?? formation.Width;
				WorldPosition worldPosition = formation.CreateNewOrderWorldPosition(0);
				worldPosition.SetVec2(formationVirtualPositionVec);
				float num3;
				int num4;
				Patch_OrderController.DecreaseUnitSpacingAndWidthWithNewArrangementOrderIfNotAllUnitsFit(formation, Patch_OrderController.GetSimulationFormation(formation, simulationFormations), formationVirtualArrangementOrder, newArrangementOrder, in worldPosition, in formationVirtualDirectionIncludingFacingEnemyAccordingToPositionAndDirection, in num2, out num3, in num, in unitSpacingOf, out num4);
				float num5;
				Patch_OrderController.SimulateNewArrangementOrderWithFrameAndWidth(formation, Patch_OrderController.GetSimulationFormation(formation, simulationFormations), formationVirtualArrangementOrder, newArrangementOrder, simulationAgentFrames, simulationFormationChanges, in worldPosition, in formationVirtualDirectionIncludingFacingEnemyAccordingToPositionAndDirection, num2, num3, in num, in unitSpacingOf, in num4, true, out num5);
				if (isSimulatingFormationChanges)
				{
					Patch_OrderController.LivePreviewFormationChanges.SetArrangementOrder(newArrangementOrder, new List<Formation> { formation });
					Patch_OrderController.LivePreviewFormationChanges.UpdateFormationChange(formation, null, null, new int?(unitSpacingOf - num4), new float?(num3));
					Patch_OrderController.LivePreviewFormationChanges.SetPreviewShape(formation, num3, num5);
				}
			}
		}

		// Token: 0x060003AF RID: 943 RVA: 0x00014DD4 File Offset: 0x00012FD4
		private static float GetNewWidthOfArrangementOrder(Formation formation, ArrangementOrder.ArrangementOrderEnum oldArrangementOrder, ArrangementOrder.ArrangementOrderEnum newArrangementOrder, float oldWidth, int oldUnitSpacing, int newUnitSpacing)
		{
			if (oldArrangementOrder == newArrangementOrder)
			{
				return oldWidth;
			}
			int num = Patch_OrderController.GetFormationVirtualUnitSpacing(formation) ?? formation.UnitSpacing;
			if (oldArrangementOrder != 1 && newArrangementOrder == 1)
			{
				return MathF.Clamp(oldWidth * 0.1f, RTSCamera.CommandSystem.Utilities.Utility.GetFormationMinimumWidthOfArrangementOrder(formation, newArrangementOrder, num), RTSCamera.CommandSystem.Utilities.Utility.GetFormationMaximumWidthOfArrangementOrder(formation, newArrangementOrder));
			}
			if (oldArrangementOrder == 1 && newArrangementOrder != 1)
			{
				return MathF.Clamp(oldWidth / 0.1f, RTSCamera.CommandSystem.Utilities.Utility.GetFormationMinimumWidthOfArrangementOrder(formation, newArrangementOrder, num), RTSCamera.CommandSystem.Utilities.Utility.GetFormationMaximumWidthOfArrangementOrder(formation, newArrangementOrder));
			}
			float num2 = oldWidth;
			if (oldArrangementOrder == null)
			{
				num2 = RTSCamera.CommandSystem.Utilities.Utility.ConvertFromWidthToFlankWidthOfCircularFormation(formation, oldUnitSpacing, oldWidth);
			}
			else if (oldArrangementOrder == 7)
			{
				num2 = RTSCamera.CommandSystem.Utilities.Utility.ConvertFromWidthToFlankWidthOfSquareFormation(formation, oldUnitSpacing, oldWidth);
			}
			int fileCountFromWidth = RTSCamera.CommandSystem.Utilities.Utility.GetFileCountFromWidth(formation, num2, oldUnitSpacing);
			float flankWidthFromFileCount = RTSCamera.CommandSystem.Utilities.Utility.GetFlankWidthFromFileCount(formation, fileCountFromWidth, newUnitSpacing);
			float num3 = flankWidthFromFileCount;
			if (newArrangementOrder == null)
			{
				num3 = RTSCamera.CommandSystem.Utilities.Utility.ConvertFromFlankWidthToWidthOfCircularFormation(formation, newUnitSpacing, flankWidthFromFileCount);
			}
			else if (newArrangementOrder == 7)
			{
				if (MissionConfigBase<CommandSystemConfig>.Get().HollowSquare)
				{
					num3 = RTSCamera.CommandSystem.Utilities.Utility.ConvertFromFlankWidthToWidthOfSquareFormation(formation, newUnitSpacing, flankWidthFromFileCount);
				}
				else
				{
					num3 = MathF.Min(RTSCamera.CommandSystem.Utilities.Utility.GetMinimumWidthOfSquareFormation(formation), flankWidthFromFileCount);
				}
			}
			return MathF.Clamp(num3, RTSCamera.CommandSystem.Utilities.Utility.GetFormationMinimumWidthOfArrangementOrder(formation, newArrangementOrder, newUnitSpacing), RTSCamera.CommandSystem.Utilities.Utility.GetFormationMaximumWidthOfArrangementOrder(formation, newArrangementOrder));
		}

		// Token: 0x060003B0 RID: 944 RVA: 0x00014EDC File Offset: 0x000130DC
		private static void DecreaseUnitSpacingAndWidthWithNewArrangementOrderIfNotAllUnitsFit(Formation formation, Formation simulationFormation, ArrangementOrder.ArrangementOrderEnum oldArrangementOrder, ArrangementOrder.ArrangementOrderEnum arrangementOrder, in WorldPosition formationPosition, in Vec2 formationDirection, in float formationWidth, out float newFormationWidth, in int unitSpacing, in int maxNewUnitSpacing, out int unitSpacingReduction)
		{
			if (simulationFormation.UnitSpacing != unitSpacing)
			{
				simulationFormation = new Formation(null, -1);
			}
			bool flag = formation.CalculateHasSignificantNumberOfMounted && Patch_OrderController.GetFormationVirtualRidingOrder(formation) != 35;
			Patch_OrderController._overridenHasAnyMountedUnit.SetValue(formation, flag);
			Patch_OrderController._overridenHasAnyMountedUnit.SetValue(simulationFormation, flag);
			int num = formation.CountOfUnitsWithoutDetachedOnes - 1;
			newFormationWidth = formationWidth;
			float num2 = formationWidth;
			if (num >= 0)
			{
				if (MissionConfigBase<CommandSystemConfig>.Get().CircleFormationUnitSpacingPreference == CircleFormationUnitSpacingPreference.Tight && arrangementOrder == null)
				{
					unitSpacingReduction = maxNewUnitSpacing;
					float num3 = formationWidth;
					do
					{
						newFormationWidth = Patch_OrderController.GetNewWidthOfArrangementOrder(formation, oldArrangementOrder, arrangementOrder, num2, unitSpacing, maxNewUnitSpacing - unitSpacingReduction);
						WorldPosition? worldPosition;
						Vec2? vec;
						Patch_OrderController.GetUnitPositionWithIndexAccordingToNewOrder(formation, simulationFormation, oldArrangementOrder, new ArrangementOrder.ArrangementOrderEnum?(arrangementOrder), num, in formationPosition, in formationDirection, formation.Arrangement, formationWidth, new float?(newFormationWidth), unitSpacing, new int?(maxNewUnitSpacing - unitSpacingReduction), formation.Arrangement.UnitCount, formation.HasAnyMountedUnit, formation.Index, out worldPosition, out vec, out num3);
						if (worldPosition != null)
						{
							break;
						}
						unitSpacingReduction--;
					}
					while (unitSpacingReduction >= 0);
					unitSpacingReduction = MathF.Max(unitSpacingReduction, 0);
					if (maxNewUnitSpacing - unitSpacingReduction > 0)
					{
						newFormationWidth = num3;
					}
				}
				else
				{
					unitSpacingReduction = 0;
					float num4 = formationWidth;
					do
					{
						newFormationWidth = Patch_OrderController.GetNewWidthOfArrangementOrder(formation, oldArrangementOrder, arrangementOrder, num2, unitSpacing, maxNewUnitSpacing - unitSpacingReduction);
						Vec2? vec;
						WorldPosition? worldPosition2;
						Patch_OrderController.GetUnitPositionWithIndexAccordingToNewOrder(formation, simulationFormation, oldArrangementOrder, new ArrangementOrder.ArrangementOrderEnum?(arrangementOrder), num, in formationPosition, in formationDirection, formation.Arrangement, formationWidth, new float?(newFormationWidth), unitSpacing, new int?(maxNewUnitSpacing - unitSpacingReduction), formation.Arrangement.UnitCount, formation.HasAnyMountedUnit, formation.Index, out worldPosition2, out vec, out num4);
						if (worldPosition2 != null)
						{
							break;
						}
						unitSpacingReduction++;
					}
					while (maxNewUnitSpacing - unitSpacingReduction >= 0);
					unitSpacingReduction = MathF.Min(unitSpacingReduction, maxNewUnitSpacing);
					newFormationWidth = num4;
				}
			}
			else
			{
				unitSpacingReduction = 0;
			}
			Patch_OrderController._overridenHasAnyMountedUnit.SetValue(formation, null);
			Patch_OrderController._overridenHasAnyMountedUnit.SetValue(simulationFormation, null);
		}

		// Token: 0x060003B1 RID: 945 RVA: 0x000150D0 File Offset: 0x000132D0
		private static void SimulateNewArrangementOrderWithFrameAndWidth(Formation formation, Formation simulationFormation, ArrangementOrder.ArrangementOrderEnum oldArrangementOrder, ArrangementOrder.ArrangementOrderEnum arrangementOrder, List<WorldPosition> simulationAgentFrames, List<ValueTuple<Formation, int, float, WorldPosition, Vec2>> simulationFormationChanges, in WorldPosition formationPosition, in Vec2 formationDirection, float formationWidth, float newFormationWidth, in int unitSpacing, in int maxNewUnitSpacing, in int unitSpacingReduction, bool simulateFormationDepth, out float simulatedFormationDepth)
		{
			int num = 0;
			float num2 = (simulateFormationDepth ? 0f : float.NaN);
			bool flag = Mission.Current.Mode != 6 || Mission.Current.IsOrderPositionAvailable(ref formationPosition, formation.Team);
			bool flag2 = formation.CalculateHasSignificantNumberOfMounted && Patch_OrderController.GetFormationVirtualRidingOrder(formation) != 35;
			Patch_OrderController._overridenHasAnyMountedUnit.SetValue(formation, flag2);
			Patch_OrderController._overridenHasAnyMountedUnit.SetValue(simulationFormation, flag2);
			foreach (Agent agent in from u in formation.GetUnitsWithoutDetachedOnes()
				orderby MBCommon.Hash(u.Index, u)
				select u)
			{
				WorldPosition? worldPosition = null;
				Vec2? vec = null;
				if (flag)
				{
					float num3;
					Patch_OrderController.GetUnitPositionWithIndexAccordingToNewOrder(formation, simulationFormation, oldArrangementOrder, new ArrangementOrder.ArrangementOrderEnum?(arrangementOrder), num, in formationPosition, in formationDirection, formation.Arrangement, formationWidth, new float?(newFormationWidth), unitSpacing, new int?(maxNewUnitSpacing - unitSpacingReduction), formation.Arrangement.UnitCount, formation.HasAnyMountedUnit, formation.Index, out worldPosition, out vec, out num3);
				}
				else
				{
					worldPosition = new WorldPosition?(agent.GetWorldPosition());
					vec = new Vec2?(agent.GetMovementDirection());
				}
				if (worldPosition != null)
				{
					if (simulationAgentFrames != null)
					{
						simulationAgentFrames.Add(worldPosition.Value);
					}
					if (simulateFormationDepth)
					{
						WorldPosition worldPosition2 = formationPosition;
						Vec2 asVec = worldPosition2.AsVec2;
						worldPosition2 = formationPosition;
						Vec2 asVec2 = worldPosition2.AsVec2;
						Vec2 vec2 = formationDirection;
						float num4 = Vec2.DistanceToLine(asVec, asVec2 + vec2.RightVec(), worldPosition.Value.AsVec2);
						if (num4 > num2)
						{
							num2 = num4;
						}
					}
				}
				num++;
			}
			Patch_OrderController._overridenHasAnyMountedUnit.SetValue(formation, null);
			Patch_OrderController._overridenHasAnyMountedUnit.SetValue(simulationFormation, null);
			if (flag)
			{
				if (simulationFormationChanges != null)
				{
					simulationFormationChanges.Add(ValueTuple.Create<Formation, int, float, WorldPosition, Vec2>(formation, unitSpacingReduction, formationWidth, formationPosition, formationDirection));
				}
			}
			else
			{
				WorldPosition worldPosition3 = formation.CreateNewOrderWorldPosition(0);
				if (simulationFormationChanges != null)
				{
					simulationFormationChanges.Add(ValueTuple.Create<Formation, int, float, WorldPosition, Vec2>(formation, unitSpacingReduction, formationWidth, worldPosition3, formation.Direction));
				}
			}
			simulatedFormationDepth = num2 + formation.UnitDiameter;
		}

		// Token: 0x060003B2 RID: 946 RVA: 0x0001532C File Offset: 0x0001352C
		private static void GetUnitPositionWithIndexAccordingToNewOrder(Formation formation, Formation simulationFormation, ArrangementOrder.ArrangementOrderEnum oldArrangementOrder, ArrangementOrder.ArrangementOrderEnum? newArrangementOrder, int unitIndex, in WorldPosition formationPosition, in Vec2 formationDirection, IFormationArrangement arrangement, float width, float? expectedNewWidth, int unitSpacing, int? expectedNewUnitSpacing, int unitCount, bool isMounted, int index, out WorldPosition? unitPosition, out Vec2? unitDirection, out float newWidth)
		{
			unitPosition = null;
			unitDirection = null;
			if (simulationFormation == null)
			{
				if (Patch_OrderController._simulationFormationTemp.GetValue(null) == null || (int)Patch_OrderController._simulationFormationUniqueIdentifier.GetValue(null) != index)
				{
					Patch_OrderController._simulationFormationTemp.SetValue(null, new Formation(null, -1));
				}
				simulationFormation = (Formation)Patch_OrderController._simulationFormationTemp.GetValue(null);
			}
			Type typeOfArrangement = RTSCamera.CommandSystem.Utilities.Utility.GetTypeOfArrangement(oldArrangementOrder, true);
			bool flag = simulationFormation.UnitSpacing == unitSpacing;
			bool flag2 = expectedNewUnitSpacing != null && simulationFormation.UnitSpacing == expectedNewUnitSpacing.Value;
			if ((flag || flag2) && simulationFormation.OrderPositionIsValid)
			{
				Vec3 orderGroundPosition = simulationFormation.OrderGroundPosition;
				WorldPosition worldPosition = formationPosition;
				Vec3 vec = worldPosition.GetGroundVec3();
				if (orderGroundPosition.NearlyEquals(ref vec, 0.1f))
				{
					Vec2 direction = simulationFormation.Direction;
					if (direction.NearlyEquals(formationDirection, 0.03f))
					{
						Type type = ((newArrangementOrder == null) ? null : RTSCamera.CommandSystem.Utilities.Utility.GetTypeOfArrangement(newArrangementOrder.Value, true));
						Type type2 = simulationFormation.Arrangement.GetType();
						Type typeOfArrangement2 = RTSCamera.CommandSystem.Utilities.Utility.GetTypeOfArrangement(simulationFormation.ArrangementOrder.OrderEnum, true);
						if (flag && direction.NearlyEquals(formationDirection, 0.2f) && type2 == typeOfArrangement)
						{
							if (newArrangementOrder == null)
							{
								goto IL_0456;
							}
							goto IL_0395;
						}
						else if (flag2 && direction.NearlyEquals(formationDirection, 0.1f) && newArrangementOrder != null && type2 == type && typeOfArrangement2 == type)
						{
							goto IL_0456;
						}
					}
				}
			}
			Patch_OrderController._overridenHasAnyMountedUnit.SetValue(simulationFormation, isMounted);
			Patch_OrderController.ResetForSimulation.Invoke(simulationFormation, new object[0]);
			Formation formation2 = simulationFormation;
			int? num = new int?(unitSpacing);
			formation2.SetPositioning(null, null, num);
			Patch_OrderController.OverridenUnitCount.SetValue(simulationFormation, unitCount);
			Formation formation3 = simulationFormation;
			WorldPosition? worldPosition2 = new WorldPosition?(formationPosition);
			Vec2? vec2 = new Vec2?(formationDirection);
			num = null;
			formation3.SetPositioning(worldPosition2, vec2, num);
			simulationFormation.Rearrange(arrangement.Clone(simulationFormation));
			Patch_OrderController.ArrangementOrderProperty.SetValue(simulationFormation, formation.ArrangementOrder);
			simulationFormation.Arrangement.DeepCopyFrom(arrangement);
			simulationFormation.Arrangement.Width = width;
			Patch_OrderController._simulationFormationUniqueIdentifier.SetValue(null, index);
			ColumnFormation columnFormation = arrangement as ColumnFormation;
			if (columnFormation != null && arrangement.RankCount > 1)
			{
				Vec3 vec = (arrangement.GetUnit(columnFormation.VanguardFileIndex, 0) as Agent).Position;
				Vec2 asVec = vec.AsVec2;
				vec = (arrangement.GetUnit(columnFormation.VanguardFileIndex, 1) as Agent).Position;
				Vec2 asVec2 = vec.AsVec2;
				Vec2 vec3 = (asVec - asVec2).Normalized();
				WorldPosition worldPosition = formationPosition;
				Vec2 vec4 = (worldPosition.AsVec2 - asVec).Normalized();
				if (arrangement.IsTurnBackwardsNecessary(asVec, new WorldPosition?(formationPosition), vec3, true, new Vec2?(vec4)))
				{
					(simulationFormation.Arrangement as ColumnFormation).UnitPositionsOnVanguardFileIndex.Reverse();
				}
			}
			if (arrangement.GetType() != typeOfArrangement)
			{
				Patch_OrderController.ArrangementOrderProperty.SetValue(simulationFormation, formation.ArrangementOrder);
				simulationFormation.SetArrangementOrder(RTSCamera.CommandSystem.Utilities.Utility.GetArrangementOrder(oldArrangementOrder));
				Formation formation4 = simulationFormation;
				num = new int?(unitSpacing);
				formation4.SetPositioning(null, null, num);
				simulationFormation.SetFormOrder(FormOrder.FormOrderCustom(width), true);
			}
			IL_0395:
			if (newArrangementOrder != null && expectedNewWidth != null && expectedNewUnitSpacing != null)
			{
				Patch_OrderController._unitSpacing.SetValue(simulationFormation, expectedNewUnitSpacing.Value);
				Patch_OrderController._formOrder.SetValue(simulationFormation, FormOrder.FormOrderCustom(expectedNewWidth.Value));
				Patch_OrderController.UpdateDesiredFileCountFromWidth(simulationFormation, newArrangementOrder.Value, expectedNewWidth.Value, expectedNewUnitSpacing.Value);
				simulationFormation.SetArrangementOrder(RTSCamera.CommandSystem.Utilities.Utility.GetArrangementOrder(newArrangementOrder.Value));
				Formation formation5 = simulationFormation;
				num = new int?(expectedNewUnitSpacing.Value);
				formation5.SetPositioning(null, null, num);
				simulationFormation.SetFormOrder(FormOrder.FormOrderCustom(expectedNewWidth.Value), true);
			}
			IL_0456:
			newWidth = simulationFormation.Width;
			if (newArrangementOrder != null && expectedNewWidth != null && expectedNewUnitSpacing != null)
			{
				if ((double)expectedNewWidth.Value < (double)newWidth)
				{
					num = expectedNewUnitSpacing;
					int num2 = 0;
					if ((num.GetValueOrDefault() > num2) & (num != null))
					{
						return;
					}
				}
			}
			else if ((double)width + 0.3630000054836273 < (double)newWidth && unitSpacing > 0)
			{
				return;
			}
			Vec2? vec5 = simulationFormation.Arrangement.GetLocalPositionOfUnitOrDefault(unitIndex);
			if (vec5 == null)
			{
				vec5 = simulationFormation.Arrangement.CreateNewPosition(unitIndex);
			}
			if (vec5 == null)
			{
				return;
			}
			Vec2 vec6 = simulationFormation.Direction.TransformToParentUnitF(vec5.Value);
			WorldPosition worldPosition3 = simulationFormation.CreateNewOrderWorldPosition(0);
			worldPosition3.SetVec2(worldPosition3.AsVec2 + vec6);
			unitPosition = new WorldPosition?(worldPosition3);
			unitDirection = new Vec2?(formationDirection);
		}

		// Token: 0x060003B3 RID: 947 RVA: 0x00015880 File Offset: 0x00013A80
		private static void UpdateDesiredFileCountFromWidth(Formation formation, ArrangementOrder.ArrangementOrderEnum newArrangementOrder, float width, int unitSpacing)
		{
			float num = width;
			if (newArrangementOrder == null)
			{
				num = RTSCamera.CommandSystem.Utilities.Utility.ConvertFromWidthToFlankWidthOfCircularFormation(formation, unitSpacing, width);
			}
			else if (newArrangementOrder == 7)
			{
				num = RTSCamera.CommandSystem.Utilities.Utility.ConvertFromWidthToFlankWidthOfSquareFormation(formation, unitSpacing, width);
			}
			int unlimitedFileCountFromWidth = RTSCamera.CommandSystem.Utilities.Utility.GetUnlimitedFileCountFromWidth(formation, num, unitSpacing);
			AccessTools.Field(typeof(Formation), "_desiredFileCount").SetValue(formation, unlimitedFileCountFromWidth);
		}

		// Token: 0x060003B4 RID: 948 RVA: 0x000158D4 File Offset: 0x00013AD4
		private static float GetFormationVirtualMinimumWidth(Formation formation)
		{
			ArrangementOrder.ArrangementOrderEnum formationVirtualArrangementOrder = Patch_OrderController.GetFormationVirtualArrangementOrder(formation);
			int num = Patch_OrderController.GetFormationVirtualUnitSpacing(formation) ?? formation.UnitSpacing;
			return RTSCamera.CommandSystem.Utilities.Utility.GetFormationMinimumWidthOfArrangementOrder(formation, formationVirtualArrangementOrder, num);
		}

		// Token: 0x060003B5 RID: 949 RVA: 0x00015910 File Offset: 0x00013B10
		private static float GetFormationVirtualMaximumWidth(Formation formation)
		{
			ArrangementOrder.ArrangementOrderEnum formationVirtualArrangementOrder = Patch_OrderController.GetFormationVirtualArrangementOrder(formation);
			return RTSCamera.CommandSystem.Utilities.Utility.GetFormationMaximumWidthOfArrangementOrder(formation, formationVirtualArrangementOrder);
		}

		// Token: 0x060003B6 RID: 950 RVA: 0x0001592C File Offset: 0x00013B2C
		public static bool Prefix_GetActiveMovementOrderOf(Formation formation, ref OrderType __result)
		{
			FormationChange formationChange;
			if (!RTSCamera.CommandSystem.Utilities.Utility.ShouldQueueCommand() || !CommandQueueLogic.LatestOrderInQueueChanges.VirtualChanges.TryGetValue(formation, out formationChange) || formationChange.MovementOrderType == null)
			{
				return true;
			}
			OrderType value = formationChange.MovementOrderType.Value;
			switch (RTSCamera.CommandSystem.Utilities.Utility.MovementStateFromMovementOrderType(value))
			{
			case 0:
				__result = 4;
				return false;
			case 1:
				if (value <= 7)
				{
					if (value == 5)
					{
						__result = 4;
						return false;
					}
					if (value == 7)
					{
						__result = 7;
						return false;
					}
				}
				else
				{
					if (value == 12)
					{
						__result = 12;
						return false;
					}
					if (value == 13)
					{
						__result = 13;
						return false;
					}
				}
				__result = 1;
				return false;
			case 2:
				__result = 9;
				return false;
			case 3:
				__result = 6;
				return false;
			default:
				__result = 1;
				return false;
			}
		}

		// Token: 0x060003B7 RID: 951 RVA: 0x000159E0 File Offset: 0x00013BE0
		public static bool Prefix_GetActiveFacingOrderOf(Formation formation, ref OrderType __result)
		{
			FormationChange formationChange;
			if (RTSCamera.CommandSystem.Utilities.Utility.ShouldQueueCommand() && CommandQueueLogic.LatestOrderInQueueChanges.VirtualChanges.TryGetValue(formation, out formationChange) && formationChange.FacingOrderType != null)
			{
				__result = formationChange.FacingOrderType.Value;
				return false;
			}
			return true;
		}

		// Token: 0x060003B8 RID: 952 RVA: 0x00015A28 File Offset: 0x00013C28
		public static bool Prefix_GetActiveFiringOrderOf(Formation formation, ref OrderType __result)
		{
			FormationChange formationChange;
			if (RTSCamera.CommandSystem.Utilities.Utility.ShouldQueueCommand() && CommandQueueLogic.LatestOrderInQueueChanges.VirtualChanges.TryGetValue(formation, out formationChange) && formationChange.FiringOrderType != null)
			{
				__result = formationChange.FiringOrderType.Value;
				return false;
			}
			return true;
		}

		// Token: 0x060003B9 RID: 953 RVA: 0x00015A70 File Offset: 0x00013C70
		public static bool Prefix_GetActiveRidingOrderOf(Formation formation, ref OrderType __result)
		{
			FormationChange formationChange;
			if (RTSCamera.CommandSystem.Utilities.Utility.ShouldQueueCommand() && CommandQueueLogic.LatestOrderInQueueChanges.VirtualChanges.TryGetValue(formation, out formationChange) && formationChange.RidingOrderType != null)
			{
				__result = formationChange.RidingOrderType.Value;
				return false;
			}
			return true;
		}

		// Token: 0x060003BA RID: 954 RVA: 0x00015AB8 File Offset: 0x00013CB8
		public static bool Prefix_GetActiveArrangementOrderOf(Formation formation, ref OrderType __result)
		{
			FormationChange formationChange;
			if (RTSCamera.CommandSystem.Utilities.Utility.ShouldQueueCommand() && CommandQueueLogic.LatestOrderInQueueChanges.VirtualChanges.TryGetValue(formation, out formationChange) && formationChange.ArrangementOrder != null)
			{
				__result = RTSCamera.CommandSystem.Utilities.Utility.ArrangementOrderEnumToOrderType(formationChange.ArrangementOrder.Value);
				return false;
			}
			return true;
		}

		// Token: 0x060003BB RID: 955 RVA: 0x00015B04 File Offset: 0x00013D04
		public static Formation GetFacingEnemyTargetFormation(Formation formation)
		{
			Formation formation2;
			if (Patch_OrderController.FacingEnemeyTarget.TryGetValue(formation, out formation2))
			{
				return formation2;
			}
			return null;
		}

		// Token: 0x060003BC RID: 956 RVA: 0x00015B23 File Offset: 0x00013D23
		public static Formation GetVirtualFacingEnemyTargetFormation(Formation formation)
		{
			if (!Patch_OrderController.LivePreviewFormationChanges.VirtualChanges.ContainsKey(formation))
			{
				return Patch_OrderController.GetFacingEnemyTargetFormation(formation);
			}
			return Patch_OrderController.LivePreviewFormationChanges.VirtualChanges[formation].FacingEnemyTargetFormation;
		}

		// Token: 0x060003BD RID: 957 RVA: 0x00015B54 File Offset: 0x00013D54
		public static void SetFacingEnemyTargetFormation(IEnumerable<Formation> formations, Formation targetFormation)
		{
			foreach (Formation formation in formations)
			{
				Patch_OrderController.SetFacingEnemyTargetFormation(formation, targetFormation);
			}
		}

		// Token: 0x060003BE RID: 958 RVA: 0x00015B9C File Offset: 0x00013D9C
		public static void SetFacingEnemyTargetFormation(Formation formation, Formation targetFormation)
		{
			if (targetFormation == null)
			{
				Patch_OrderController.FacingEnemeyTarget.Remove(formation);
				return;
			}
			Patch_OrderController.FacingEnemeyTarget[formation] = targetFormation;
		}

		// Token: 0x060003BF RID: 959 RVA: 0x00015BBC File Offset: 0x00013DBC
		public static void TryFadeOutForFacingToEnemyOrder(OrderController orderController, IEnumerable<Formation> selectedFormations, Formation targetFormation)
		{
			bool flag = RTSCamera.CommandSystem.Utilities.Utility.ShouldFadeOut();
			List<WorldPosition> list;
			List<ValueTuple<Formation, int, float, WorldPosition, Vec2>> list2;
			Patch_OrderController.SimulateFacingToEnemyOrder(selectedFormations, orderController.simulationFormations, targetFormation, flag, out list, true, out list2);
			if (flag)
			{
				Patch_OrderTroopPlacer.AddOrderPositionEntities(list, true, 0);
			}
		}

		// Token: 0x060003C0 RID: 960 RVA: 0x00015BF0 File Offset: 0x00013DF0
		public static void TryFadeOutForMoveOrder(OrderController orderController, List<Formation> selectedFormations, WorldPosition worldPosition)
		{
			if (RTSCamera.CommandSystem.Utilities.Utility.ShouldFadeOut())
			{
				List<WorldPosition> list;
				List<ValueTuple<Formation, int, float, WorldPosition, Vec2>> list2;
				bool flag;
				Patch_OrderController.SimulateNewOrderWithPositionAndDirection(selectedFormations, orderController.simulationFormations, worldPosition, worldPosition, true, out list, false, out list2, out flag, true, false);
				Patch_OrderTroopPlacer.AddOrderPositionEntities(list, true, 0);
			}
		}

		// Token: 0x04000157 RID: 343
		private static bool _patched;

		// Token: 0x04000158 RID: 344
		private static FieldInfo actualUnitSpacingsField = typeof(OrderController).GetField("actualUnitSpacings", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x04000159 RID: 345
		private static FieldInfo actualWidthsField = typeof(OrderController).GetField("actualWidths", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x0400015A RID: 346
		private static FieldInfo _overridenHasAnyMountedUnit = typeof(Formation).GetField("_overridenHasAnyMountedUnit", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x0400015B RID: 347
		private static MethodInfo ResetForSimulation = typeof(Formation).GetMethod("ResetForSimulation", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x0400015C RID: 348
		private static PropertyInfo OverridenUnitCount = typeof(Formation).GetProperty("OverridenUnitCount", BindingFlags.Instance | BindingFlags.Public);

		// Token: 0x0400015D RID: 349
		private static PropertyInfo ArrangementOrderProperty = typeof(Formation).GetProperty("ArrangementOrder", BindingFlags.Instance | BindingFlags.Public);

		// Token: 0x0400015E RID: 350
		private static FieldInfo _simulationFormationTemp = typeof(Formation).GetField("_simulationFormationTemp", BindingFlags.Static | BindingFlags.NonPublic);

		// Token: 0x0400015F RID: 351
		private static FieldInfo _simulationFormationUniqueIdentifier = typeof(Formation).GetField("_simulationFormationUniqueIdentifier", BindingFlags.Static | BindingFlags.NonPublic);

		// Token: 0x04000160 RID: 352
		private static Dictionary<Formation, int> _naturalUnitSpacings = new Dictionary<Formation, int>();

		// Token: 0x04000161 RID: 353
		private static Dictionary<Formation, int> _customUnitSpacings = new Dictionary<Formation, int>();

		// Token: 0x04000162 RID: 354
		private static Dictionary<Formation, float> _widthsBackup = new Dictionary<Formation, float>();

		// Token: 0x04000163 RID: 355
		public static FormationChanges LivePreviewFormationChanges = new FormationChanges();

		// Token: 0x04000164 RID: 356
		private static Dictionary<Formation, Patch_OrderController.MovingTarget> _currentMovingTarget;

		// Token: 0x04000165 RID: 357
		public static Dictionary<Formation, Formation> FacingEnemeyTarget = new Dictionary<Formation, Formation>();

		// Token: 0x04000166 RID: 358
		private static FieldInfo _engageTargetPositionCache = AccessTools.Field(typeof(MovementOrder), "_engageTargetPositionCache");

		// Token: 0x04000167 RID: 359
		private static PropertyInfo _unitSpacing = AccessTools.Property(typeof(Formation), "UnitSpacing");

		// Token: 0x04000168 RID: 360
		private static PropertyInfo _formOrder = AccessTools.Property(typeof(Formation), "FormOrder");

		// Token: 0x020000B9 RID: 185
		public class StackRecord
		{
			// Token: 0x170000D3 RID: 211
			// (get) Token: 0x06000603 RID: 1539 RVA: 0x000216C2 File Offset: 0x0001F8C2
			public float Center
			{
				get
				{
					return (this.LeftMost + this.RightMost) * 0.5f;
				}
			}

			// Token: 0x04000303 RID: 771
			public List<Formation> Formations = new List<Formation>();

			// Token: 0x04000304 RID: 772
			public float LeftMost;

			// Token: 0x04000305 RID: 773
			public float RightMost;

			// Token: 0x04000306 RID: 774
			public float Width;

			// Token: 0x04000307 RID: 775
			public float MinimumWidth;

			// Token: 0x04000308 RID: 776
			public float MaximumWidth;
		}

		// Token: 0x020000BA RID: 186
		public class MovingTarget
		{
			// Token: 0x04000309 RID: 777
			public WorldPosition? MedianPosition;
		}
	}
}
