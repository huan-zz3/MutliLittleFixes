using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using MissionSharedLibrary.Config;
using MissionSharedLibrary.Utilities;
using RTSCamera.CommandSystem.Config;
using RTSCamera.CommandSystem.Config.HotKey;
using RTSCamera.CommandSystem.Logic;
using RTSCamera.CommandSystem.Orders;
using RTSCamera.CommandSystem.Utilities;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.GauntletUI;
using TaleWorlds.MountAndBlade.View.MissionViews.Order;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order;
using TaleWorlds.ScreenSystem;

namespace RTSCamera.CommandSystem.Patch
{
	// Token: 0x0200005C RID: 92
	public class Patch_GauntletOrderUIHandler
	{
		// Token: 0x06000331 RID: 817 RVA: 0x0000E5CC File Offset: 0x0000C7CC
		public static bool Patch(Harmony harmony)
		{
			bool flag;
			try
			{
				if (Patch_GauntletOrderUIHandler._patched)
				{
					flag = false;
				}
				else
				{
					Patch_GauntletOrderUIHandler._patched = true;
					harmony.Patch(typeof(GauntletOrderUIHandler).GetMethod("TickInput", BindingFlags.Instance | BindingFlags.NonPublic), null, null, new HarmonyMethod(typeof(Patch_GauntletOrderUIHandler).GetMethod("Transpile_TickInput", BindingFlags.Static | BindingFlags.Public)), null);
					harmony.Patch(typeof(GauntletOrderUIHandler).GetMethod("OnMissionScreenTick", BindingFlags.Instance | BindingFlags.Public), null, new HarmonyMethod(typeof(Patch_GauntletOrderUIHandler).GetMethod("Postfix_OnMissionScreenTick", BindingFlags.Static | BindingFlags.Public), -1, new string[] { "RTSCameraPatch" }, null, null), null, null);
					flag = true;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				MissionSharedLibrary.Utilities.Utility.DisplayMessage(ex.ToString());
				MBDebug.Print(ex.ToString(), 0, 12, 17592186044416UL);
				flag = false;
			}
			return flag;
		}

		// Token: 0x06000332 RID: 818 RVA: 0x0000E6BC File Offset: 0x0000C8BC
		public static void OnBehaviorInitialize()
		{
			Patch_GauntletOrderUIHandler._callbackList = new List<Action>();
		}

		// Token: 0x06000333 RID: 819 RVA: 0x0000E6C8 File Offset: 0x0000C8C8
		public static void OnRemoveBehavior()
		{
			Patch_GauntletOrderUIHandler._callbackList = null;
		}

		// Token: 0x06000334 RID: 820 RVA: 0x0000E6D0 File Offset: 0x0000C8D0
		public static IEnumerable<CodeInstruction> Transpile_TickInput(IEnumerable<CodeInstruction> instructions)
		{
			List<CodeInstruction> list = new List<CodeInstruction>(instructions);
			Patch_GauntletOrderUIHandler.ApplyCommandQueueChange(list);
			return list.AsEnumerable<CodeInstruction>();
		}

		// Token: 0x06000335 RID: 821 RVA: 0x0000E6E4 File Offset: 0x0000C8E4
		private static void ApplyCommandQueueChange(List<CodeInstruction> codes)
		{
			bool flag = false;
			int num = -1;
			for (int i = 0; i < codes.Count; i++)
			{
				if (!flag && codes[i].opcode == OpCodes.Callvirt && (codes[i].operand as MethodInfo).Name == "get_ActiveTargetState")
				{
					flag = true;
					num = i;
					break;
				}
			}
			if (!flag)
			{
				throw new Exception("get_ActiveTargetState not found");
			}
			codes.InsertRange(num - 2, new List<CodeInstruction>
			{
				new CodeInstruction(OpCodes.Ldarg_0, null),
				new CodeInstruction(OpCodes.Call, typeof(Patch_GauntletOrderUIHandler).GetMethod("TryAddSelectedOrderToQueue", BindingFlags.Static | BindingFlags.NonPublic)),
				new CodeInstruction(OpCodes.Brtrue, codes[num + 1].operand)
			});
		}

		// Token: 0x06000336 RID: 822 RVA: 0x0000E7BC File Offset: 0x0000C9BC
		private static bool TryAddSelectedOrderToQueue(GauntletOrderUIHandler __instance)
		{
			if (__instance.Mission.IsNavalBattle)
			{
				return false;
			}
			MissionOrderVM missionOrderVM = Patch_GauntletOrderUIHandler._dataSource.GetValue(__instance) as MissionOrderVM;
			if (missionOrderVM.ActiveTargetState != 0 || (!__instance.Input.IsKeyReleased(224) && !__instance.Input.IsKeyReleased(255)))
			{
				return false;
			}
			if (missionOrderVM.SelectedOrderSet != null && Input.IsGamepadActive)
			{
				return false;
			}
			bool flag;
			OrderInQueue orderToAdd = Patch_GauntletOrderUIHandler.GetOrderToAdd(__instance, missionOrderVM, out flag);
			if (orderToAdd != null)
			{
				CommandQueueLogic.AddOrderToQueue(orderToAdd);
				return true;
			}
			return flag;
		}

		// Token: 0x06000337 RID: 823 RVA: 0x0000E840 File Offset: 0x0000CA40
		private static OrderInQueue GetOrderToAdd(GauntletOrderUIHandler __instance, MissionOrderVM dataSource, out bool skipNativeOrder)
		{
			MissionScreen missionScreen = __instance.MissionScreen;
			skipNativeOrder = false;
			if (dataSource == null)
			{
				return null;
			}
			bool flag = RTSCamera.CommandSystem.Utilities.Utility.ShouldQueueCommand();
			List<Formation> list = dataSource.OrderController.SelectedFormations.Where<Formation>((Formation f) => f.CountOfUnitsWithoutDetachedOnes > 0).ToList<Formation>();
			if (list.Count == 0)
			{
				return null;
			}
			if (!flag)
			{
				Patch_OrderController.LivePreviewFormationChanges.SetChanges(CommandQueueLogic.CurrentFormationChanges.CollectChanges(list));
			}
			else
			{
				Patch_OrderController.LivePreviewFormationChanges.SetChanges(CommandQueueLogic.LatestOrderInQueueChanges.CollectChanges(list));
			}
			OrderInQueue orderInQueue = new OrderInQueue
			{
				SelectedFormations = list
			};
			switch (__instance.CursorState)
			{
			case 0:
			{
				MBReadOnlyList<Formation> mbreadOnlyList = Patch_GauntletOrderUIHandler._focusedFormationsCache.GetValue(__instance) as MBReadOnlyList<Formation>;
				if (mbreadOnlyList != null && mbreadOnlyList.Count > 0)
				{
					bool flag2 = CommandSystemGameKeyCategory.GetKey(GameKeyEnum.KeepMovementOrder).IsKeyDownInOrder(__instance.Input);
					bool flag3 = MissionConfigBase<CommandSystemConfig>.Get().DisableNativeAttack && RTSCommandVisualOrder.OrderToSelectTarget == SelectTargetMode.None;
					OrderTroopPlacer orderTroopPlacer = Patch_GauntletOrderUIHandler._orderTroopPlacer.GetValue(__instance) as OrderTroopPlacer;
					if (orderTroopPlacer != null && (!flag3 || flag2))
					{
						orderTroopPlacer.SuspendTroopPlacer = true;
						FieldInfo targetFormationOrderGivenWithActionButton = Patch_GauntletOrderUIHandler._targetFormationOrderGivenWithActionButton;
						if (targetFormationOrderGivenWithActionButton != null)
						{
							targetFormationOrderGivenWithActionButton.SetValue(__instance, true);
						}
					}
					if (RTSCommandVisualOrder.OrderToSelectTarget == SelectTargetMode.Advance)
					{
						orderInQueue.OrderType = 12;
						orderInQueue.TargetFormation = mbreadOnlyList[0];
						Patch_OrderController.LivePreviewFormationChanges.SetMovementOrder(12, list, mbreadOnlyList[0], null, null);
						orderInQueue.VirtualFormationChanges = Patch_OrderController.LivePreviewFormationChanges.CollectChanges(list);
						RTSCommandVisualOrder.OrderToSelectTarget = SelectTargetMode.None;
						if (!flag)
						{
							skipNativeOrder = true;
							dataSource.OrderController.SetOrderWithFormation(12, mbreadOnlyList[0]);
						}
						else
						{
							RTSCamera.CommandSystem.Utilities.Utility.MissionOrderVM_OnOrderExecutedWithId("order_movement_advance");
						}
					}
					else if (RTSCommandVisualOrder.OrderToSelectTarget == SelectTargetMode.LookAtEnemy)
					{
						orderInQueue.OrderType = 14;
						orderInQueue.TargetFormation = mbreadOnlyList[0];
						orderInQueue.ShouldAdjustFormationSpeed = RTSCamera.CommandSystem.Utilities.Utility.ShouldLockFormation();
						Patch_OrderController.LivePreviewFormationChanges.SetFacingOrder(14, list, mbreadOnlyList[0]);
						if (!flag)
						{
							skipNativeOrder = true;
							Patch_OrderController.SetFacingEnemyTargetFormation(list, orderInQueue.TargetFormation);
							dataSource.OrderController.SetOrder(14);
						}
						else
						{
							RTSCamera.CommandSystem.Utilities.Utility.MissionOrderVM_OnOrderExecutedWithId("order_toggle_facing");
						}
						orderInQueue.VirtualFormationChanges = Patch_OrderController.LivePreviewFormationChanges.CollectChanges(list);
						RTSCommandVisualOrder.OrderToSelectTarget = SelectTargetMode.None;
					}
					else
					{
						if (flag2)
						{
							RTSCamera.CommandSystem.Utilities.Utility.FocusOnFormation(dataSource.OrderController, mbreadOnlyList[0]);
							skipNativeOrder = true;
							return null;
						}
						if (flag3)
						{
							skipNativeOrder = true;
							return null;
						}
						orderInQueue.OrderType = 4;
						orderInQueue.TargetFormation = mbreadOnlyList[0];
						Patch_OrderController.LivePreviewFormationChanges.SetMovementOrder(4, list, mbreadOnlyList[0], null, null);
						orderInQueue.VirtualFormationChanges = Patch_OrderController.LivePreviewFormationChanges.CollectChanges(list);
						if (flag)
						{
							RTSCamera.CommandSystem.Utilities.Utility.MissionOrderVM_OnOrderExecutedWithId("order_movement_charge");
						}
					}
				}
				else
				{
					IOrderable focusedOrderableObject = __instance.MissionScreen.OrderFlag.FocusedOrderableObject;
					if (focusedOrderableObject != null && list.Count > 0)
					{
						BattleSideEnum side = list[0].Team.Side;
						OrderType order = focusedOrderableObject.GetOrder(side);
						MissionObject missionObject = focusedOrderableObject as MissionObject;
						if (order <= 3)
						{
							if (order != 1)
							{
								if (order - 2 <= 1)
								{
									IPointDefendable pointDefendable = focusedOrderableObject as IPointDefendable;
									Vec3 globalPosition = pointDefendable.DefencePoints.Last<DefencePoint>().GameEntity.GlobalPosition;
									Vec3 globalPosition2 = pointDefendable.DefencePoints.First<DefencePoint>().GameEntity.GlobalPosition;
									if (!flag)
									{
										return null;
									}
									if (list.Count > 0)
									{
										orderInQueue.OrderType = ((order == 2) ? 2 : 3);
										WorldPosition worldPosition;
										worldPosition..ctor(__instance.Mission.Scene, UIntPtr.Zero, globalPosition, false);
										WorldPosition worldPosition2;
										worldPosition2..ctor(__instance.Mission.Scene, UIntPtr.Zero, globalPosition2, false);
										List<ValueTuple<Formation, int, float, WorldPosition, Vec2>> list2;
										bool flag4;
										OrderController.SimulateNewOrderWithPositionAndDirection(list, dataSource.OrderController.simulationFormations, worldPosition, worldPosition2, ref list2, ref flag4, order == 2);
										orderInQueue.IsLineShort = flag4;
										orderInQueue.ActualFormationChanges = list2;
										orderInQueue.PositionBegin = worldPosition;
										orderInQueue.PositionEnd = worldPosition2;
										Patch_OrderController.LivePreviewFormationChanges.SetMovementOrder((order == 2) ? 2 : 3, list, null, null, null);
										orderInQueue.VirtualFormationChanges = Patch_OrderController.LivePreviewFormationChanges.CollectChanges(list);
										RTSCamera.CommandSystem.Utilities.Utility.MissionOrderVM_OnOrderExecutedWithId("order_movement_move");
									}
								}
							}
							else
							{
								WorldPosition worldPosition3;
								worldPosition3..ctor(__instance.Mission.Scene, UIntPtr.Zero, missionObject.GameEntity.GlobalPosition, false);
								orderInQueue.OrderType = 1;
								orderInQueue.PositionBegin = worldPosition3;
								foreach (Formation formation in list)
								{
									Patch_OrderController.LivePreviewFormationChanges.UpdateFormationChange(formation, new WorldPosition?(worldPosition3), null, null, null);
								}
								Patch_OrderController.LivePreviewFormationChanges.SetMovementOrder(1, list, null, null, null);
								orderInQueue.VirtualFormationChanges = Patch_OrderController.LivePreviewFormationChanges.CollectChanges(list);
								if (flag)
								{
									RTSCamera.CommandSystem.Utilities.Utility.MissionOrderVM_OnOrderExecutedWithId("order_movement_move");
								}
							}
						}
						else if (order != 8)
						{
							switch (order)
							{
							case 39:
							{
								UsableMachine usableMachine = focusedOrderableObject as UsableMachine;
								IEnumerable<Formation> enumerable = list.Where<Formation>(new Func<Formation, bool>(usableMachine.IsUsedByFormation));
								orderInQueue.OrderType = 39;
								orderInQueue.TargetEntity = usableMachine;
								if (Extensions.IsEmpty<Formation>(enumerable))
								{
									if (usableMachine.HasWaitFrame)
									{
										orderInQueue.OrderType = 8;
										GameEntity waitEntity = usableMachine.WaitEntity;
										if (waitEntity != null)
										{
											MatrixFrame matrixFrame = waitEntity.GetGlobalFrame();
											Vec2 vec = matrixFrame.rotation.f.AsVec2.Normalized();
											Patch_OrderController.LivePreviewFormationChanges.SetFacingOrder(15, list, null);
											foreach (Formation formation2 in list)
											{
												Patch_OrderController.LivePreviewFormationChanges.UpdateFormationChange(formation2, null, new Vec2?(vec), null, null);
											}
										}
										Patch_OrderController.LivePreviewFormationChanges.SetMovementOrder(8, list, null, null, focusedOrderableObject);
									}
									if (!flag)
									{
										SiegeWeapon siegeWeapon = usableMachine as SiegeWeapon;
										if (siegeWeapon != null)
										{
											siegeWeapon.SetForcedUse(true);
										}
										RTSCamera.CommandSystem.Utilities.Utility.CallAfterSetOrder(dataSource.OrderController, 1);
									}
									else
									{
										RTSCamera.CommandSystem.Utilities.Utility.MissionOrderVM_OnOrderExecutedWithId("order_movement_stop");
									}
								}
								else
								{
									orderInQueue.CustomOrderType = CustomOrderType.StopUsing;
									orderInQueue.OrderType = 39;
									orderInQueue.IsStopUsing = true;
									if (!flag)
									{
										SiegeWeapon siegeWeapon2 = usableMachine as SiegeWeapon;
										if (siegeWeapon2 != null)
										{
											siegeWeapon2.SetForcedUse(false);
										}
										RTSCamera.CommandSystem.Utilities.Utility.CallAfterSetOrder(dataSource.OrderController, 6);
									}
									else
									{
										RTSCamera.CommandSystem.Utilities.Utility.MissionOrderVM_OnOrderExecutedWithId("order_movement_stop");
									}
								}
								orderInQueue.VirtualFormationChanges = Patch_OrderController.LivePreviewFormationChanges.CollectChanges(list);
								RTSCamera.CommandSystem.Utilities.Utility.DisplayExecuteOrderMessage(list, orderInQueue);
								break;
							}
							case 40:
								orderInQueue.OrderType = 40;
								orderInQueue.TargetEntity = focusedOrderableObject;
								Patch_OrderController.LivePreviewFormationChanges.SetMovementOrder(40, list, null, null, focusedOrderableObject);
								orderInQueue.VirtualFormationChanges = Patch_OrderController.LivePreviewFormationChanges.CollectChanges(list);
								if (!flag)
								{
									RTSCamera.CommandSystem.Utilities.Utility.CallAfterSetOrder(dataSource.OrderController, 4);
								}
								else
								{
									RTSCamera.CommandSystem.Utilities.Utility.MissionOrderVM_OnOrderExecutedWithId("order_movement_move");
								}
								RTSCamera.CommandSystem.Utilities.Utility.DisplayExecuteOrderMessage(list, orderInQueue);
								break;
							case 41:
								orderInQueue.OrderType = 41;
								orderInQueue.TargetEntity = focusedOrderableObject;
								Patch_OrderController.LivePreviewFormationChanges.SetMovementOrder(41, list, null, null, focusedOrderableObject);
								orderInQueue.VirtualFormationChanges = Patch_OrderController.LivePreviewFormationChanges.CollectChanges(list);
								RTSCamera.CommandSystem.Utilities.Utility.DisplayExecuteOrderMessage(list, orderInQueue);
								if (!flag)
								{
									RTSCamera.CommandSystem.Utilities.Utility.CallAfterSetOrder(dataSource.OrderController, 1);
								}
								else
								{
									RTSCamera.CommandSystem.Utilities.Utility.MissionOrderVM_OnOrderExecutedWithId("order_movement_move");
								}
								break;
							}
						}
						else
						{
							orderInQueue.OrderType = 8;
							orderInQueue.TargetEntity = focusedOrderableObject;
							UsableMachine usableMachine2 = focusedOrderableObject as UsableMachine;
							bool flag5 = usableMachine2 == null || Extensions.IsEmpty<Formation>(list.Where<Formation>(new Func<Formation, bool>(usableMachine2.IsUsedByFormation)));
							if (flag5)
							{
								if (usableMachine2 != null)
								{
									GameEntity waitEntity2 = usableMachine2.WaitEntity;
									if (waitEntity2 != null)
									{
										MatrixFrame matrixFrame = waitEntity2.GetGlobalFrame();
										Vec2 vec2 = matrixFrame.rotation.f.AsVec2.Normalized();
										Patch_OrderController.LivePreviewFormationChanges.SetFacingOrder(15, list, null);
										foreach (Formation formation3 in list)
										{
											Patch_OrderController.LivePreviewFormationChanges.UpdateFormationChange(formation3, null, new Vec2?(vec2), null, null);
										}
									}
								}
								Patch_OrderController.LivePreviewFormationChanges.SetMovementOrder(8, list, null, null, focusedOrderableObject);
								orderInQueue.VirtualFormationChanges = Patch_OrderController.LivePreviewFormationChanges.CollectChanges(list);
								if (!flag)
								{
									SiegeWeapon siegeWeapon3 = usableMachine2 as SiegeWeapon;
									if (siegeWeapon3 != null)
									{
										siegeWeapon3.SetForcedUse(true);
									}
									RTSCamera.CommandSystem.Utilities.Utility.CallAfterSetOrder(dataSource.OrderController, 1);
								}
								else
								{
									RTSCamera.CommandSystem.Utilities.Utility.MissionOrderVM_OnOrderExecutedWithId("order_movement_move");
								}
								RTSCamera.CommandSystem.Utilities.Utility.DisplayExecuteOrderMessage(list, orderInQueue);
							}
							else
							{
								orderInQueue.CustomOrderType = CustomOrderType.StopUsing;
								orderInQueue.OrderType = 39;
								orderInQueue.IsStopUsing = true;
								if (!flag)
								{
									skipNativeOrder = true;
									SiegeWeapon siegeWeapon4 = usableMachine2 as SiegeWeapon;
									if (siegeWeapon4 != null)
									{
										siegeWeapon4.SetForcedUse(false);
									}
									foreach (Formation formation4 in list)
									{
										formation4.SetControlledByAI(false, false);
										ModuleExtensions.StopUsingMachine(formation4, usableMachine2, true);
									}
									RTSCamera.CommandSystem.Utilities.Utility.CallAfterSetOrder(dataSource.OrderController, 6);
									CommandQueueLogic.OnCustomOrderIssued(orderInQueue, dataSource.OrderController);
								}
								else
								{
									RTSCamera.CommandSystem.Utilities.Utility.MissionOrderVM_OnOrderExecutedWithId("order_movement_stop");
								}
								RTSCamera.CommandSystem.Utilities.Utility.DisplayExecuteOrderMessage(list, orderInQueue);
							}
						}
					}
				}
				break;
			}
			case 1:
			{
				orderInQueue.OrderType = 15;
				orderInQueue.ShouldAdjustFormationSpeed = RTSCamera.CommandSystem.Utilities.Utility.ShouldLockFormation();
				RTSCommandVisualOrder.OrderToSelectTarget = SelectTargetMode.None;
				if (flag)
				{
					Patch_OrderController.FillOrderLookingAtPosition(orderInQueue, dataSource.OrderController, new WorldPosition(Mission.Current.Scene, UIntPtr.Zero, __instance.MissionScreen.GetOrderFlagPosition(), false));
					RTSCamera.CommandSystem.Utilities.Utility.MissionOrderVM_OnOrderExecutedWithId("order_toggle_facing");
				}
				else
				{
					skipNativeOrder = true;
					orderInQueue.SelectedFormations = orderInQueue.SelectedFormations.Where<Formation>((Formation f) => !RTSCamera.CommandSystem.Utilities.Utility.IsFormationOrderPositionMoving(f)).ToList<Formation>();
					dataSource.OrderController.SetOrderWithPosition(15, new WorldPosition(Mission.Current.Scene, UIntPtr.Zero, __instance.MissionScreen.GetOrderFlagPosition(), false));
					RTSCamera.CommandSystem.Utilities.Utility.MissionOrderVM_OnOrderExecutedWithId("order_toggle_facing");
					orderInQueue.VirtualFormationChanges = Patch_OrderController.LivePreviewFormationChanges.CollectChanges(list);
				}
				OrderSetVM selectedOrderSet = dataSource.SelectedOrderSet;
				if (selectedOrderSet != null)
				{
					selectedOrderSet.ExecuteDeSelect();
				}
				break;
			}
			case 2:
				return null;
			}
			if (!flag)
			{
				CommandQueueLogic.TryPendingOrder(orderInQueue.SelectedFormations, orderInQueue);
				return null;
			}
			return orderInQueue;
		}

		// Token: 0x06000338 RID: 824 RVA: 0x0000F2C4 File Offset: 0x0000D4C4
		public static void Postfix_OnMissionScreenTick(GauntletOrderUIHandler __instance, ref float ____latestDt, ref bool ____isReceivingInput, float dt, MissionOrderVM ____dataSource, GauntletLayer ____gauntletLayer, OrderTroopPlacer ____orderTroopPlacer, ref bool ____isTransferEnabled)
		{
			Patch_GauntletOrderUIHandler.UpdateMouseVisibility(__instance, ____dataSource, ____gauntletLayer, ref ____isTransferEnabled);
			Patch_GauntletOrderUIHandler.UpdateOrderTroopPlacerDrawingFacing(__instance, ____dataSource, ____gauntletLayer, ____orderTroopPlacer);
			Patch_GauntletOrderUIHandler.UpdateCallbackList();
		}

		// Token: 0x06000339 RID: 825 RVA: 0x0000F2E4 File Offset: 0x0000D4E4
		private static void UpdateMouseVisibility(GauntletOrderUIHandler __instance, MissionOrderVM ____dataSource, GauntletLayer ____gauntletLayer, ref bool ____isTransferEnabled)
		{
			if (__instance == null)
			{
				return;
			}
			bool flag = __instance.IsDeployment || ____dataSource.TroopController.IsTransferActive || (____dataSource.IsToggleOrderShown && (__instance.Input.IsAltDown() || __instance.MissionScreen.LastFollowedAgent == null));
			InputUsageMask inputUsageMask = ((__instance.IsDeployment || ____dataSource.TroopController.IsTransferActive) ? 7 : (MissionConfigBase<CommandSystemConfig>.Get().OrderUIClickable ? 7 : 0));
			if (flag != ____gauntletLayer.InputRestrictions.MouseVisibility || inputUsageMask != ____gauntletLayer.InputRestrictions.InputUsageMask)
			{
				____gauntletLayer.InputRestrictions.SetInputRestrictions(flag, inputUsageMask);
			}
			if (____dataSource.TroopController.IsTransferActive != ____isTransferEnabled)
			{
				____isTransferEnabled = ____dataSource.TroopController.IsTransferActive;
				if (!____isTransferEnabled)
				{
					____gauntletLayer.UIContext.ContextAlpha = (BannerlordConfig.HideBattleUI ? 0f : 1f);
					____gauntletLayer.IsFocusLayer = false;
					ScreenManager.TryLoseFocus(____gauntletLayer);
					return;
				}
				____gauntletLayer.UIContext.ContextAlpha = 1f;
				____gauntletLayer.IsFocusLayer = true;
				ScreenManager.TrySetFocus(____gauntletLayer);
			}
		}

		// Token: 0x0600033A RID: 826 RVA: 0x0000F3F4 File Offset: 0x0000D5F4
		private static void UpdateOrderTroopPlacerDrawingFacing(GauntletOrderUIHandler __instance, MissionOrderVM ____dataSource, GauntletLayer ____gauntletLayer, OrderTroopPlacer ____orderTroopPlacer)
		{
			if (__instance.IsValidForTick && ____dataSource != null && ____gauntletLayer.IsActive && ____dataSource.IsToggleOrderShown && MissionConfigBase<CommandSystemConfig>.Get().OrderUIClickable)
			{
				OrderSetVM selectedOrderSet = ____dataSource.SelectedOrderSet;
				____orderTroopPlacer.IsDrawingFacing = ((selectedOrderSet != null) ? selectedOrderSet.OrderIconId : null) == "order_type_facing" || RTSCommandVisualOrder.OrderToSelectTarget == SelectTargetMode.LookAtDirection;
			}
		}

		// Token: 0x0600033B RID: 827 RVA: 0x0000F458 File Offset: 0x0000D658
		private static void SetSiegeWeaponForceUseNextTick(SiegeWeapon siegeWeapon, bool forceUse)
		{
			Patch_GauntletOrderUIHandler._callbackList.Add(delegate
			{
				siegeWeapon.SetForcedUse(forceUse);
			});
		}

		// Token: 0x0600033C RID: 828 RVA: 0x0000F48F File Offset: 0x0000D68F
		private static void UpdateCallbackList()
		{
			if (Patch_GauntletOrderUIHandler._callbackList.Count == 0)
			{
				return;
			}
			Action action = Patch_GauntletOrderUIHandler._callbackList[Patch_GauntletOrderUIHandler._callbackList.Count - 1];
			Patch_GauntletOrderUIHandler._callbackList.RemoveAt(Patch_GauntletOrderUIHandler._callbackList.Count - 1);
			action();
		}

		// Token: 0x04000148 RID: 328
		private static FieldInfo _focusedFormationsCache = typeof(GauntletOrderUIHandler).GetField("_focusedFormationsCache", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x04000149 RID: 329
		private static FieldInfo _dataSource = typeof(GauntletOrderUIHandler).GetField("_dataSource", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x0400014A RID: 330
		private static FieldInfo _targetFormationOrderGivenWithActionButton = typeof(GauntletOrderUIHandler).GetField("_targetFormationOrderGivenWithActionButton", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x0400014B RID: 331
		private static FieldInfo _orderTroopPlacer = typeof(GauntletOrderUIHandler).GetField("_orderTroopPlacer", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x0400014C RID: 332
		private static bool _patched;

		// Token: 0x0400014D RID: 333
		private static List<Action> _callbackList = new List<Action>();
	}
}
