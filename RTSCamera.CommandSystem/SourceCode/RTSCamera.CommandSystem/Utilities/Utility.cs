using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MissionSharedLibrary.Config;
using MissionSharedLibrary.Utilities;
using RTSCamera.CommandSystem.Config;
using RTSCamera.CommandSystem.Config.HotKey;
using RTSCamera.CommandSystem.Logic;
using RTSCamera.CommandSystem.Orders.VisualOrders;
using RTSCamera.CommandSystem.Patch;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.GauntletUI;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order.Visual;

namespace RTSCamera.CommandSystem.Utilities
{
	// Token: 0x02000054 RID: 84
	public static class Utility
	{
		// Token: 0x060002B1 RID: 689 RVA: 0x0000B2B0 File Offset: 0x000094B0
		public static void PrintOrderHint()
		{
			Utility.DisplayMessage(Module.CurrentModule.GlobalTextManager.FindText("str_rts_camera_command_system_order_queue_usage", null).SetTextVariable("KeyName", CommandSystemGameKeyCategory.GetKey(GameKeyEnum.CommandQueue).ToSequenceString()).ToString());
		}

		// Token: 0x060002B2 RID: 690 RVA: 0x0000B2E8 File Offset: 0x000094E8
		public static void DisplayChargeToFormationMessage(MBReadOnlyList<Formation> selectedFormations, Formation targetFormation)
		{
			List<TextObject> list = new List<TextObject>();
			foreach (Formation formation in selectedFormations)
			{
				list.Add(GameTexts.FindText("str_formation_class_string", FormationClassExtensions.GetName(formation.PhysicalClass)));
			}
			if (!Extensions.IsEmpty<TextObject>(list))
			{
				TextObject textObject = new TextObject("{=ApD0xQXT}{STR1}: {STR2}", null);
				textObject.SetTextVariable("STR1", GameTexts.GameTextHelper.MergeTextObjectsWithComma(list, false));
				textObject.SetTextVariable("STR2", GameTexts.FindText("str_formation_ai_sergeant_instruction_behavior_text", "BehaviorTacticalCharge").SetTextVariable("AI_SIDE", GameTexts.FindText("str_formation_ai_side_strings", targetFormation.AI.Side.ToString())).SetTextVariable("CLASS", GameTexts.FindText("str_troop_group_name", targetFormation.PhysicalClass.ToString())));
				InformationManager.DisplayMessage(new InformationMessage(textObject.ToString()));
			}
		}

		// Token: 0x060002B3 RID: 691 RVA: 0x0000B3F4 File Offset: 0x000095F4
		public static void DisplayFormationReadyMessage(Formation formation)
		{
			TextObject textObject = GameTexts.FindText("str_formation_ai_behavior_text", "BehaviorStop");
			textObject.SetTextVariable("IS_PLURAL", 0);
			textObject.SetTextVariable("TROOP_NAMES_BEGIN", "");
			textObject.SetTextVariable("TROOP_NAMES_END", GameTexts.FindText("str_troop_group_name", formation.PhysicalClass.ToString()));
			Utility.DisplayMessage(textObject.ToString(), Utility.MessageColor);
		}

		// Token: 0x060002B4 RID: 692 RVA: 0x0000B464 File Offset: 0x00009664
		public static void DisplayFormationChargeMessage(Formation formation)
		{
			TextObject textObject = GameTexts.FindText("str_formation_ai_behavior_text", "BehaviorTacticalCharge");
			textObject.SetTextVariable("IS_PLURAL", 0);
			textObject.SetTextVariable("TROOP_NAMES_BEGIN", "");
			textObject.SetTextVariable("TROOP_NAMES_END", GameTexts.FindText("str_troop_group_name", formation.PhysicalClass.ToString()));
			Utility.DisplayMessage(textObject.ToString(), Utility.MessageColor);
		}

		// Token: 0x060002B5 RID: 693 RVA: 0x0000B4D4 File Offset: 0x000096D4
		public static void DisplayVolleyEnabledMessage(IEnumerable<Formation> selectedFormations, bool enabled)
		{
			List<TextObject> list = new List<TextObject>();
			foreach (Formation formation in selectedFormations)
			{
				list.Add(GameTexts.FindText("str_formation_class_string", FormationClassExtensions.GetName(formation.PhysicalClass)));
			}
			if (!Extensions.IsEmpty<TextObject>(list))
			{
				TextObject textObject = new TextObject("{=ApD0xQXT}{STR1}: {STR2}", null);
				textObject.SetTextVariable("STR1", GameTexts.GameTextHelper.MergeTextObjectsWithComma(list, false));
				textObject.SetTextVariable("STR2", enabled ? GameTexts.FindText("str_rts_camera_command_system_volley_enabled", null) : GameTexts.FindText("str_rts_camera_command_system_volley_disabled", null));
				InformationManager.DisplayMessage(new InformationMessage(textObject.ToString()));
			}
		}

		// Token: 0x060002B6 RID: 694 RVA: 0x0000B594 File Offset: 0x00009794
		public static void DisplayVolleyFireMessage(IEnumerable<Formation> selectedFormations)
		{
			List<TextObject> list = new List<TextObject>();
			foreach (Formation formation in selectedFormations)
			{
				list.Add(GameTexts.FindText("str_formation_class_string", FormationClassExtensions.GetName(formation.PhysicalClass)));
			}
			if (!Extensions.IsEmpty<TextObject>(list))
			{
				TextObject textObject = new TextObject("{=ApD0xQXT}{STR1}: {STR2}", null);
				textObject.SetTextVariable("STR1", GameTexts.GameTextHelper.MergeTextObjectsWithComma(list, false));
				textObject.SetTextVariable("STR2", GameTexts.FindText("str_rts_camera_command_system_volley_fire", null));
				InformationManager.DisplayMessage(new InformationMessage(textObject.ToString()));
			}
		}

		// Token: 0x060002B7 RID: 695 RVA: 0x0000B644 File Offset: 0x00009844
		public unsafe static bool ShouldChargeToFormation(Agent agent)
		{
			if (agent.Formation != null)
			{
				MovementOrder movementOrder = *agent.Formation.GetReadonlyMovementOrderReference();
				if (movementOrder.OrderType == 5 && MissionConfigBase<CommandSystemConfig>.Get().AttackSpecificFormation)
				{
					return QueryLibrary.IsCavalry(agent) || (QueryLibrary.IsRangedCavalry(agent) && agent.Formation.FiringOrder.OrderType == 31) || (!CommandSystemSubModule.IsRealisticBattleModuleInstalled && (QueryLibrary.IsInfantry(agent) || (QueryLibrary.IsRanged(agent) && agent.Formation.FiringOrder.OrderType == 31)));
				}
			}
			return false;
		}

		// Token: 0x060002B8 RID: 696 RVA: 0x0000B6E2 File Offset: 0x000098E2
		public static void CallAfterSetOrder(OrderController orderController, OrderType orderType)
		{
			MethodInfo afterSetOrder = Utility.AfterSetOrder;
			if (afterSetOrder == null)
			{
				return;
			}
			afterSetOrder.Invoke(orderController, new object[] { orderType });
		}

		// Token: 0x060002B9 RID: 697 RVA: 0x0000B704 File Offset: 0x00009904
		public static void DisplayFocusAttackMessage(IEnumerable<Formation> formations, Formation target)
		{
			List<TextObject> list = new List<TextObject>();
			foreach (Formation formation in formations)
			{
				list.Add(GameTexts.FindText("str_formation_class_string", FormationClassExtensions.GetName(formation.PhysicalClass)));
			}
			if (!Extensions.IsEmpty<TextObject>(list))
			{
				TextObject textObject = new TextObject("{=ApD0xQXT}{STR1}: {STR2}", null);
				textObject.SetTextVariable("STR1", GameTexts.GameTextHelper.MergeTextObjectsWithComma(list, false));
				TextObject textObject2 = GameTexts.FindText("str_rts_camera_command_system_defensive_attack", null);
				textObject2.SetTextVariable("TARGET_NAME", GameTexts.FindText("str_troop_group_name", target.PhysicalClass.ToString()));
				textObject.SetTextVariable("STR2", textObject2);
				Utility.DisplayMessage(textObject.ToString(), Utility.MessageColor);
			}
		}

		// Token: 0x060002BA RID: 698 RVA: 0x0000B7DC File Offset: 0x000099DC
		public static void DisplayAddOrderToQueueMessage()
		{
			Utility.DisplayLocalizedText("str_rts_camera_command_system_add_order_to_queue", null, Utility.MessageColor);
		}

		// Token: 0x060002BB RID: 699 RVA: 0x0000B7F0 File Offset: 0x000099F0
		public static void DisplayAdjustFormationSpeedMessage(IEnumerable<Formation> formations)
		{
			List<TextObject> list = new List<TextObject>();
			foreach (Formation formation in formations)
			{
				list.Add(GameTexts.FindText("str_troop_group_name", formation.PhysicalClass.ToString()));
			}
			if (!Extensions.IsEmpty<TextObject>(list))
			{
				TextObject textObject = new TextObject("{=ApD0xQXT}{STR1}: {STR2}", null);
				textObject.SetTextVariable("STR1", GameTexts.GameTextHelper.MergeTextObjectsWithComma(list, false));
				textObject.SetTextVariable("STR2", GameTexts.FindText("str_rts_camera_command_system_sync_locked_formation_speed_message", null));
				Utility.DisplayMessage(textObject.ToString(), Utility.MessageColor);
			}
		}

		// Token: 0x060002BC RID: 700 RVA: 0x0000B8A4 File Offset: 0x00009AA4
		public static void DisplayExecuteOrderMessageInQueue(IEnumerable<Formation> selectedFormations, OrderInQueue order)
		{
			if (!order.IsExecutingOrderMessageShown)
			{
				order.IsExecutingOrderMessageShown = true;
				Utility.DisplayLocalizedText("str_rts_camera_command_system_execute_order_in_queue", null, Utility.MessageColor);
			}
			List<TextObject> list = new List<TextObject>();
			foreach (Formation formation in selectedFormations)
			{
				list.Add(GameTexts.FindText("str_troop_group_name", formation.PhysicalClass.ToString()));
			}
			if (!Extensions.IsEmpty<TextObject>(list))
			{
				TextObject textObject = new TextObject("{=ApD0xQXT}{STR1}: {STR2}", null);
				textObject.SetTextVariable("STR1", GameTexts.GameTextHelper.MergeTextObjectsWithComma(list, false));
				textObject.SetTextVariable("STR2", Utility.GetOrderString(order));
				Utility.DisplayMessage(textObject.ToString(), Utility.MessageColor);
			}
		}

		// Token: 0x060002BD RID: 701 RVA: 0x0000B970 File Offset: 0x00009B70
		public static void DisplayExecuteOrderMessage(IEnumerable<Formation> selectedFormations, OrderInQueue order)
		{
			List<TextObject> list = new List<TextObject>();
			foreach (Formation formation in selectedFormations)
			{
				list.Add(GameTexts.FindText("str_troop_group_name", formation.PhysicalClass.ToString()));
			}
			if (!Extensions.IsEmpty<TextObject>(list))
			{
				TextObject textObject = new TextObject("{=ApD0xQXT}{STR1}: {STR2}", null);
				textObject.SetTextVariable("STR1", GameTexts.GameTextHelper.MergeTextObjectsWithComma(list, false));
				textObject.SetTextVariable("STR2", Utility.GetOrderString(order));
				Utility.DisplayMessage(textObject.ToString(), Utility.MessageColor);
			}
		}

		// Token: 0x060002BE RID: 702 RVA: 0x0000BA1C File Offset: 0x00009C1C
		private static TextObject GetOrderString(OrderInQueue order)
		{
			string text = "str_order_name";
			string text2 = null;
			switch (order.CustomOrderType)
			{
			case CustomOrderType.Original:
				switch (order.OrderType)
				{
				case 1:
				case 2:
				case 3:
					return RTSCommandMoveVisualOrder.GetName();
				case 4:
				case 5:
					return RTSCommandChargeVisualOrder.GetName();
				case 6:
					return RTSCommandStopVisualOrder.GetName();
				case 7:
					return RTSCommandFollowMeVisualOrder.GetName();
				case 8:
					text = (order.IsStopUsing ? "str_rts_camera_command_system_stop_use_entity" : "str_rts_camera_command_system_follow_entity");
					return Utility.AppendEntityName(GameTexts.FindText(text, text2), order.TargetEntity);
				case 9:
					return RTSCommandRetreatVisualOrder.GetName();
				case 12:
					return RTSCommandAdvanceVisualOrder.GetName();
				case 13:
					return RTSCommandFallbackVisualOrder.GetName();
				case 14:
					return RTSCommandToggleFacingVisualOrder.GetName(order.OrderType);
				case 15:
					return RTSCommandToggleFacingVisualOrder.GetName(order.OrderType);
				case 16:
				case 17:
				case 18:
				case 19:
				case 20:
				case 21:
				case 22:
				case 23:
					return RTSCommandArrangementVisualOrder.GetName(Utility.OrderTypeToArrangementOrderEnum(order.OrderType));
				case 31:
				case 32:
				case 34:
				case 35:
				case 36:
				case 37:
					return RTSCommandGenericToggleVisualOrder.GetName(order.OrderType);
				case 39:
					text = (order.IsStopUsing ? "str_rts_camera_command_system_stop_use_entity" : "str_rts_camera_command_system_use_entity");
					return Utility.AppendEntityName(GameTexts.FindText(text, text2), order.TargetEntity);
				case 40:
					text = "str_rts_camera_command_system_attack_entity";
					return Utility.AppendEntityName(GameTexts.FindText(text, text2), order.TargetEntity);
				case 41:
					text = "str_rts_camera_command_system_point_defense";
					return Utility.AppendEntityName(GameTexts.FindText(text, text2), order.TargetEntity);
				}
				Utility.DisplayMessage("Error: unexpected order type");
				break;
			case CustomOrderType.FollowMainAgent:
				return RTSCommandFollowMeVisualOrder.GetName();
			case CustomOrderType.SetTargetFormation:
			{
				TextObject textObject = GameTexts.FindText("str_rts_camera_command_system_defensive_attack", null);
				textObject.SetTextVariable("TARGET_NAME", GameTexts.FindText("str_troop_group_name", order.TargetFormation.PhysicalClass.ToString()));
				return textObject;
			}
			case CustomOrderType.ManualVolley:
				return GameTexts.FindText("str_rts_camera_command_system_volley_enabled", null);
			case CustomOrderType.DisableVolley:
				return GameTexts.FindText("str_rts_camera_command_system_volley_disabled", null);
			case CustomOrderType.VolleyFire:
				return GameTexts.FindText("str_rts_camera_command_system_volley_fire", null);
			case CustomOrderType.StopUsing:
				text = "str_rts_camera_command_system_stop_use_entity";
				return Utility.AppendEntityName(GameTexts.FindText(text, text2), order.TargetEntity);
			}
			return GameTexts.FindText(text, text2);
		}

		// Token: 0x060002BF RID: 703 RVA: 0x0000BC88 File Offset: 0x00009E88
		private static TextObject AppendEntityName(TextObject prefix, IOrderable orderable)
		{
			try
			{
				if (orderable != null)
				{
					UsableMachine usableMachine = orderable as UsableMachine;
					if (usableMachine == null)
					{
						return prefix;
					}
					string text = prefix.ToString();
					string text2 = " ";
					TextObject descriptionText = usableMachine.GetDescriptionText(usableMachine.GameEntity);
					prefix = new TextObject(text + text2 + ((descriptionText != null) ? descriptionText.ToString() : null), null);
				}
			}
			catch (Exception ex)
			{
				Utility.DisplayMessage(ex.ToString());
			}
			return prefix;
		}

		// Token: 0x060002C0 RID: 704 RVA: 0x0000BCF8 File Offset: 0x00009EF8
		public static void FocusOnFormation(OrderController playerController, Formation targetFormation)
		{
			Utility.GetMissionScreen();
			bool flag = Utility.ShouldQueueCommand();
			if (!flag)
			{
				Patch_OrderController.LivePreviewFormationChanges.SetChanges(CommandQueueLogic.CurrentFormationChanges.CollectChanges(playerController.SelectedFormations));
			}
			else
			{
				Patch_OrderController.LivePreviewFormationChanges.SetChanges(CommandQueueLogic.LatestOrderInQueueChanges.CollectChanges(playerController.SelectedFormations));
			}
			OrderInQueue orderInQueue = new OrderInQueue
			{
				CustomOrderType = CustomOrderType.SetTargetFormation,
				SelectedFormations = playerController.SelectedFormations,
				TargetFormation = targetFormation
			};
			orderInQueue.VirtualFormationChanges = Patch_OrderController.LivePreviewFormationChanges.CollectChanges(playerController.SelectedFormations);
			if (flag)
			{
				CommandQueueLogic.AddOrderToQueue(orderInQueue);
			}
			else
			{
				foreach (Formation formation in playerController.SelectedFormations)
				{
					formation.SetControlledByAI(false, false);
					formation.SetTargetFormation(targetFormation);
				}
				Mission mission = Mission.Current;
				if (mission != null)
				{
					CommandSystemLogic missionBehavior = mission.GetMissionBehavior<CommandSystemLogic>();
					if (missionBehavior != null)
					{
						missionBehavior.OnMovementOrderChanged(playerController.SelectedFormations);
					}
				}
				Utility.DisplayFocusAttackMessage(playerController.SelectedFormations, orderInQueue.TargetFormation);
				Utility.CallAfterSetOrder(playerController, 5);
				CommandQueueLogic.OnCustomOrderIssued(orderInQueue, playerController);
			}
			Utility.MissionOrderVM_OnOrderExecutedWithId("order_movement_charge");
		}

		// Token: 0x060002C1 RID: 705 RVA: 0x0000BE20 File Offset: 0x0000A020
		public static void ChargeToFormation(OrderController playerController, Formation targetFormation)
		{
			bool flag = Utility.ShouldQueueCommand();
			if (!flag)
			{
				Patch_OrderController.LivePreviewFormationChanges.SetChanges(CommandQueueLogic.CurrentFormationChanges.CollectChanges(playerController.SelectedFormations));
			}
			else
			{
				Patch_OrderController.LivePreviewFormationChanges.SetChanges(CommandQueueLogic.LatestOrderInQueueChanges.CollectChanges(playerController.SelectedFormations));
			}
			OrderInQueue orderInQueue = new OrderInQueue
			{
				OrderType = 5,
				SelectedFormations = playerController.SelectedFormations,
				TargetFormation = targetFormation
			};
			Patch_OrderController.LivePreviewFormationChanges.SetMovementOrder(5, playerController.SelectedFormations, targetFormation, null, null);
			orderInQueue.VirtualFormationChanges = Patch_OrderController.LivePreviewFormationChanges.CollectChanges(playerController.SelectedFormations);
			if (flag)
			{
				CommandQueueLogic.AddOrderToQueue(orderInQueue);
			}
			else
			{
				foreach (Formation formation in playerController.SelectedFormations)
				{
					Utility.CallAfterSetOrder(playerController, 5);
					formation.SetMovementOrder(MovementOrder.MovementOrderChargeToTarget(targetFormation));
					formation.SetTargetFormation(targetFormation);
				}
				CommandQueueLogic.TryPendingOrder(playerController.SelectedFormations, orderInQueue);
				playerController.SetOrderWithFormation(5, targetFormation);
			}
			Utility.MissionOrderVM_OnOrderExecutedWithId("order_movement_charge");
		}

		// Token: 0x060002C2 RID: 706 RVA: 0x0000BF34 File Offset: 0x0000A134
		public static bool ShouldLockFormation()
		{
			CommandSystemConfig commandSystemConfig = MissionConfigBase<CommandSystemConfig>.Get();
			MissionScreen missionScreen = Utility.GetMissionScreen();
			if (commandSystemConfig == null || missionScreen == null)
			{
				return false;
			}
			switch (commandSystemConfig.FormationLockCondition)
			{
			case FormationLockCondition.Never:
				return false;
			case FormationLockCondition.WhenPressed:
				return CommandSystemGameKeyCategory.GetKey(GameKeyEnum.FormationLockMovement).IsKeyDownInOrder(missionScreen.SceneLayer.Input);
			case FormationLockCondition.WhenNotPressed:
				return !CommandSystemGameKeyCategory.GetKey(GameKeyEnum.FormationLockMovement).IsKeyDownInOrder(missionScreen.SceneLayer.Input);
			default:
				return false;
			}
		}

		// Token: 0x060002C3 RID: 707 RVA: 0x0000BFA4 File Offset: 0x0000A1A4
		public static bool ShouldKeepRelativePositions()
		{
			return true;
		}

		// Token: 0x060002C4 RID: 708 RVA: 0x0000BFA8 File Offset: 0x0000A1A8
		public static bool IsFormationOrderPositionMoving(Formation formation)
		{
			FormationChange formationChange;
			return Patch_OrderController.LivePreviewFormationChanges.VirtualChanges.TryGetValue(formation, out formationChange) && Utility.IsMovementOrderMoving(formationChange.MovementOrderType);
		}

		// Token: 0x060002C5 RID: 709 RVA: 0x0000BFD8 File Offset: 0x0000A1D8
		public static bool IsMovementOrderMoving(OrderType? movementOrderType)
		{
			if (movementOrderType != null)
			{
				OrderType valueOrDefault = movementOrderType.GetValueOrDefault();
				switch (valueOrDefault)
				{
				case 4:
				case 5:
				case 7:
				case 8:
				case 12:
				case 13:
					break;
				case 6:
				case 9:
				case 10:
				case 11:
					return false;
				default:
					if (valueOrDefault != 40)
					{
						return false;
					}
					break;
				}
				return true;
			}
			return false;
		}

		// Token: 0x060002C6 RID: 710 RVA: 0x0000C030 File Offset: 0x0000A230
		public static WorldPosition? GetFormationMovingOrderPosition(Formation formation)
		{
			FormationChange formationChange;
			if (Patch_OrderController.LivePreviewFormationChanges.VirtualChanges.TryGetValue(formation, out formationChange))
			{
				OrderType? movementOrderType = formationChange.MovementOrderType;
				if (movementOrderType != null)
				{
					OrderType valueOrDefault = movementOrderType.GetValueOrDefault();
					switch (valueOrDefault)
					{
					case 4:
					case 5:
						Utility.GetMissionScreen();
						return new WorldPosition?(Utility.ShouldQueueCommand() ? Patch_OrderController.GetFormationVirtualPosition(formation) : formation.CachedMedianPosition);
					case 6:
					case 9:
					case 10:
					case 11:
						break;
					case 7:
						return new WorldPosition?(Patch_OrderController.GetFollowOrderPosition(formation, formationChange.TargetAgent));
					case 8:
					{
						UsableMachine usableMachine = formationChange.TargetEntity as UsableMachine;
						if (usableMachine == null)
						{
							return null;
						}
						GameEntity waitEntity = usableMachine.WaitEntity;
						if (waitEntity == null)
						{
							return null;
						}
						return new WorldPosition?(Patch_OrderController.GetFollowEntityOrderPosition(formation, waitEntity));
					}
					case 12:
						return new WorldPosition?(Patch_OrderController.GetAdvanceOrderPosition(formation, 0, formationChange.TargetFormation));
					case 13:
						return new WorldPosition?(Patch_OrderController.GetFallbackOrderPosition(formation, 0, formationChange.TargetFormation));
					default:
						if (valueOrDefault == 40)
						{
							MissionObject missionObject = formationChange.TargetEntity as MissionObject;
							if (missionObject == null)
							{
								return null;
							}
							GameEntity gameEntity = GameEntity.CreateFromWeakEntity(missionObject.GameEntity);
							if (gameEntity == null)
							{
								return null;
							}
							return new WorldPosition?(Patch_OrderController.GetAttackEntityWaitPosition(formation, gameEntity));
						}
						break;
					}
				}
			}
			return null;
		}

		// Token: 0x060002C7 RID: 711 RVA: 0x0000C1A0 File Offset: 0x0000A3A0
		public static Vec2 GetFormationMovingDirection(Formation formation)
		{
			FormationChange formationChange;
			if (Patch_OrderController.LivePreviewFormationChanges.VirtualChanges.TryGetValue(formation, out formationChange))
			{
				OrderType? movementOrderType = formationChange.MovementOrderType;
				if (movementOrderType != null)
				{
					OrderType valueOrDefault = movementOrderType.GetValueOrDefault();
					switch (valueOrDefault)
					{
					case 7:
					case 12:
					case 13:
						break;
					case 8:
					{
						GameEntity waitEntity = (formationChange.TargetEntity as UsableMachine).WaitEntity;
						return Patch_OrderController.GetFollowEntityDirection(formation, waitEntity);
					}
					case 9:
					case 10:
					case 11:
						goto IL_0076;
					default:
						if (valueOrDefault != 40)
						{
							goto IL_0076;
						}
						break;
					}
					return Vec2.Invalid;
				}
			}
			IL_0076:
			return Vec2.Invalid;
		}

		// Token: 0x060002C8 RID: 712 RVA: 0x0000C228 File Offset: 0x0000A428
		public static bool IsAnyFormationHavingMovingOrderPostion(IEnumerable<Formation> formations)
		{
			return Patch_OrderController.LivePreviewFormationChanges.CollectChanges(formations).Any<KeyValuePair<Formation, FormationChange>>((KeyValuePair<Formation, FormationChange> pair) => Utility.IsFormationOrderPositionMoving(pair.Key));
		}

		// Token: 0x060002C9 RID: 713 RVA: 0x0000C25E File Offset: 0x0000A45E
		public static bool ShouldLockFormationDuringLookAtDirection(Formation formation)
		{
			return !Utility.IsFormationOrderPositionMoving(formation) && Patch_OrderController.GetFormationVirtualFacingOrder(formation) == 15 && Utility.ShouldLockFormation();
		}

		// Token: 0x060002CA RID: 714 RVA: 0x0000C27C File Offset: 0x0000A47C
		public static bool ShouldKeepFormationWidth()
		{
			MissionScreen missionScreen = Utility.GetMissionScreen();
			return missionScreen != null && CommandSystemGameKeyCategory.GetKey(GameKeyEnum.KeepFormationWidth).IsKeyDownInOrder(missionScreen.SceneLayer.Input);
		}

		// Token: 0x060002CB RID: 715 RVA: 0x0000C2AA File Offset: 0x0000A4AA
		public static MovementOrder.MovementStateEnum MovementStateFromMovementOrderType(OrderType orderType)
		{
			switch (orderType)
			{
			case 4:
			case 5:
				return 0;
			case 6:
				return 3;
			case 9:
				return 2;
			}
			return 1;
		}

		// Token: 0x060002CC RID: 716 RVA: 0x0000C2D8 File Offset: 0x0000A4D8
		public static Type GetTypeOfArrangement(ArrangementOrder.ArrangementOrderEnum orderEnum, bool hollowSquareAllowed = false)
		{
			if (orderEnum <= 1)
			{
				if (orderEnum == null)
				{
					return typeof(CircularFormation);
				}
				if (orderEnum == 1)
				{
					return typeof(ColumnFormation);
				}
			}
			else
			{
				if (orderEnum == 6)
				{
					return typeof(SkeinFormation);
				}
				if (orderEnum == 7)
				{
					return (MissionConfigBase<CommandSystemConfig>.Get().HollowSquare && hollowSquareAllowed) ? typeof(SquareFormation) : typeof(RectilinearSchiltronFormation);
				}
			}
			return typeof(LineFormation);
		}

		// Token: 0x060002CD RID: 717 RVA: 0x0000C358 File Offset: 0x0000A558
		public static ArrangementOrder.ArrangementOrderEnum GetOrderEnumOfArrangement(IFormationArrangement arrangement)
		{
			Type type = arrangement.GetType();
			if (type == typeof(LineFormation))
			{
				return 2;
			}
			if (type == typeof(ColumnFormation))
			{
				return 1;
			}
			if (type == typeof(SkeinFormation))
			{
				return 6;
			}
			if (type == typeof(CircularFormation) || type == typeof(CircularSchiltronFormation))
			{
				return 0;
			}
			if (type == typeof(SquareFormation) || type == typeof(RectilinearSchiltronFormation))
			{
				return 7;
			}
			return 2;
		}

		// Token: 0x060002CE RID: 718 RVA: 0x0000C3F8 File Offset: 0x0000A5F8
		public static ArrangementOrder GetArrangementOrder(ArrangementOrder.ArrangementOrderEnum arrangementOrder)
		{
			switch (arrangementOrder)
			{
			case 0:
				return ArrangementOrder.ArrangementOrderCircle;
			case 1:
				return ArrangementOrder.ArrangementOrderColumn;
			case 2:
				return ArrangementOrder.ArrangementOrderLine;
			case 3:
				return ArrangementOrder.ArrangementOrderLoose;
			case 4:
				return ArrangementOrder.ArrangementOrderScatter;
			case 5:
				return ArrangementOrder.ArrangementOrderShieldWall;
			case 6:
				return ArrangementOrder.ArrangementOrderSkein;
			case 7:
				return ArrangementOrder.ArrangementOrderSquare;
			default:
				return ArrangementOrder.ArrangementOrderLine;
			}
		}

		// Token: 0x060002CF RID: 719 RVA: 0x0000C464 File Offset: 0x0000A664
		public static OrderType ArrangementOrderEnumToOrderType(ArrangementOrder.ArrangementOrderEnum arrangementOrder)
		{
			OrderType orderType;
			switch (arrangementOrder)
			{
			case 0:
				orderType = 19;
				break;
			case 1:
				orderType = 22;
				break;
			case 2:
				orderType = 16;
				break;
			case 3:
				orderType = 18;
				break;
			case 4:
				orderType = 23;
				break;
			case 5:
				orderType = 17;
				break;
			case 6:
				orderType = 21;
				break;
			case 7:
				orderType = 20;
				break;
			default:
				orderType = 0;
				break;
			}
			return orderType;
		}

		// Token: 0x060002D0 RID: 720 RVA: 0x0000C4C4 File Offset: 0x0000A6C4
		public static ArrangementOrder.ArrangementOrderEnum OrderTypeToArrangementOrderEnum(OrderType orderType)
		{
			ArrangementOrder.ArrangementOrderEnum arrangementOrderEnum;
			switch (orderType)
			{
			case 16:
				arrangementOrderEnum = 2;
				break;
			case 17:
				arrangementOrderEnum = 5;
				break;
			case 18:
				arrangementOrderEnum = 3;
				break;
			case 19:
				arrangementOrderEnum = 0;
				break;
			case 20:
				arrangementOrderEnum = 7;
				break;
			case 21:
				arrangementOrderEnum = 6;
				break;
			case 22:
				arrangementOrderEnum = 1;
				break;
			case 23:
				arrangementOrderEnum = 4;
				break;
			default:
				arrangementOrderEnum = 2;
				break;
			}
			return arrangementOrderEnum;
		}

		// Token: 0x060002D1 RID: 721 RVA: 0x0000C520 File Offset: 0x0000A720
		public static int GetUnitCountWithOverride(Formation formation)
		{
			if (formation.OverridenUnitCount != null)
			{
				return formation.OverridenUnitCount.Value;
			}
			return formation.Arrangement.UnitCount;
		}

		// Token: 0x060002D2 RID: 722 RVA: 0x0000C557 File Offset: 0x0000A757
		public static int GetMinimumFileCount(Formation formation)
		{
			return MathF.Max(1, (int)MathF.Sqrt((float)Utility.GetUnitCountWithOverride(formation)));
		}

		// Token: 0x060002D3 RID: 723 RVA: 0x0000C56C File Offset: 0x0000A76C
		public static float GetFormationInterval(Formation formation, int unitSpacing)
		{
			if (!formation.CalculateHasSignificantNumberOfMounted || formation.RidingOrder == RidingOrder.RidingOrderDismount)
			{
				return Formation.InfantryInterval(unitSpacing);
			}
			return Formation.CavalryInterval(unitSpacing);
		}

		// Token: 0x060002D4 RID: 724 RVA: 0x0000C595 File Offset: 0x0000A795
		public static float GetFormationDistance(Formation formation, int unitSpacing)
		{
			if (!formation.CalculateHasSignificantNumberOfMounted || formation.RidingOrder == RidingOrder.RidingOrderDismount)
			{
				return Formation.InfantryDistance(unitSpacing);
			}
			return Formation.CavalryDistance(unitSpacing);
		}

		// Token: 0x060002D5 RID: 725 RVA: 0x0000C5C0 File Offset: 0x0000A7C0
		public static float GetFormationMaximumWidthOfArrangementOrder(Formation formation, ArrangementOrder.ArrangementOrderEnum arrangementOrder)
		{
			int unitSpacingOf = ArrangementOrder.GetUnitSpacingOf(arrangementOrder);
			if (arrangementOrder == null)
			{
				return Utility.GetMaximumWidthOfCircularFormation(formation, unitSpacingOf);
			}
			if (arrangementOrder == 1)
			{
				return Utility.GetMaximumWidthOfColumnFormation(formation, unitSpacingOf);
			}
			if (arrangementOrder == 7)
			{
				return Utility.GetMaximumWidthOfSquareFormation(formation);
			}
			return Utility.GetMaximumWidthOfLineFormation(formation, unitSpacingOf);
		}

		// Token: 0x060002D6 RID: 726 RVA: 0x0000C5FD File Offset: 0x0000A7FD
		public static float GetFormationMinimumWidthOfArrangementOrder(Formation formation, ArrangementOrder.ArrangementOrderEnum arrangementOrder, int unitSpacing)
		{
			if (arrangementOrder == null)
			{
				return Utility.GetMinimumWidthOfCircularFormation(formation, unitSpacing);
			}
			if (arrangementOrder == 1)
			{
				return Utility.GetMinimumWidthOfColumnFormation(formation, unitSpacing);
			}
			if (arrangementOrder == 7)
			{
				return Utility.GetMinimumWidthOfSquareFormation(formation);
			}
			return Utility.GetMinimumWidthOfLineFormation(formation);
		}

		// Token: 0x060002D7 RID: 727 RVA: 0x0000C627 File Offset: 0x0000A827
		public static float GetMinimumWidthOfLineFormation(Formation formation)
		{
			return (float)(Utility.GetMinimumFileCount(formation) - 1) * (formation.MinimumInterval + formation.UnitDiameter) + formation.UnitDiameter;
		}

		// Token: 0x060002D8 RID: 728 RVA: 0x0000C648 File Offset: 0x0000A848
		public static float GetMaximumWidthOfLineFormation(Formation formation, int unitSpacing)
		{
			float num = formation.UnitDiameter;
			int unitCountWithOverride = Utility.GetUnitCountWithOverride(formation);
			if (unitCountWithOverride > 0)
			{
				num += (float)(unitCountWithOverride - 1) * (Utility.GetFormationInterval(formation, unitSpacing) + formation.UnitDiameter);
			}
			return num;
		}

		// Token: 0x060002D9 RID: 729 RVA: 0x0000C680 File Offset: 0x0000A880
		public static float GetMinimumWidthOfCircularFormation(Formation formation, int unitSpacing)
		{
			int unitCountWithOverride = Utility.GetUnitCountWithOverride(formation);
			int maximumRankCountOfCircularFormation = Utility.GetMaximumRankCountOfCircularFormation(formation, unitCountWithOverride, unitSpacing);
			float num = formation.MinimumInterval + formation.UnitDiameter;
			float num2 = formation.MinimumDistance + formation.UnitDiameter;
			return (float)((double)Utility.GetCircumferenceAuxOfCircularFormation(unitCountWithOverride, maximumRankCountOfCircularFormation, num, num2) / 3.1415927410125732);
		}

		// Token: 0x060002DA RID: 730 RVA: 0x0000C6CE File Offset: 0x0000A8CE
		public static float GetMaximumWidthOfCircularFormation(Formation formation, int unitSpacing)
		{
			return MathF.Max(0f, (float)((double)Utility.GetUnitCountWithOverride(formation) * (double)(Utility.GetFormationInterval(formation, unitSpacing) + formation.UnitDiameter) / 3.1415927410125732));
		}

		// Token: 0x060002DB RID: 731 RVA: 0x0000C6FC File Offset: 0x0000A8FC
		public static int GetMaximumRankCountOfCircularFormation(Formation formation, int unitCount, int unitSpacing)
		{
			int num = 0;
			int i = 0;
			float num2 = Utility.GetFormationInterval(formation, unitSpacing) + formation.UnitDiameter;
			float num3 = Utility.GetFormationDistance(formation, unitSpacing) + formation.UnitDiameter;
			while (i < unitCount)
			{
				int num4 = (int)(6.2831854820251465 * (double)((float)num * num3) / (double)num2);
				i += MathF.Max(1, num4);
				num++;
			}
			return MathF.Max(num, 1);
		}

		// Token: 0x060002DC RID: 732 RVA: 0x0000C75C File Offset: 0x0000A95C
		public static float GetCircumferenceAuxOfCircularFormation(int unitCount, int rankCount, float radialInterval, float distanceInterval)
		{
			float num = (float)(6.2831854820251465 * (double)distanceInterval);
			float num2 = MathF.Max(0f, (float)unitCount * radialInterval);
			int num3 = 0;
			float num4;
			int num5;
			do
			{
				num4 = num2 - (float)num3 * num;
				num4 -= num4 % radialInterval;
				num5 = Utility.GetUnitCountAuxOfCircularFormation(MathF.Max(0f, num2 - (float)(num3 + 1) * num), rankCount, radialInterval, distanceInterval);
				num3++;
			}
			while (num5 >= unitCount && num4 > 0f);
			if (MissionConfigBase<CommandSystemConfig>.Get().CircleFormationUnitSpacingPreference == CircleFormationUnitSpacingPreference.Loose)
			{
				return num4;
			}
			int num6 = 0;
			num2 = num4;
			do
			{
				num4 = num2 - (float)num6 * radialInterval;
				num5 = Utility.GetUnitCountAuxOfCircularFormation(MathF.Max(0f, num2 - (float)(num6 + 1) * radialInterval), rankCount, radialInterval, distanceInterval);
				num6++;
			}
			while (num5 >= unitCount && num4 > 0f);
			return num4;
		}

		// Token: 0x060002DD RID: 733 RVA: 0x0000C810 File Offset: 0x0000AA10
		private static int GetUnitCountAuxOfCircularFormation(float circumference, int rankCount, float radialInterval, float distanceInterval)
		{
			int num = 0;
			double num2 = 6.2831854820251465 * (double)distanceInterval;
			for (int i = 1; i <= rankCount; i++)
			{
				int num3 = (int)(MathF.Max(0.0, (double)circumference - (double)(rankCount - i) * num2) / (double)radialInterval);
				num += num3;
			}
			return MathF.Max(num, 1);
		}

		// Token: 0x060002DE RID: 734 RVA: 0x0000C860 File Offset: 0x0000AA60
		private static float GetDiameterOfCircularFormation(Formation formation, float circumference, int unitSpacing)
		{
			int unitCountWithOverride = Utility.GetUnitCountWithOverride(formation);
			int maximumRankCountOfCircularFormation = Utility.GetMaximumRankCountOfCircularFormation(formation, unitCountWithOverride, unitSpacing);
			float num = Utility.GetFormationInterval(formation, unitSpacing) + formation.UnitDiameter;
			float num2 = Utility.GetFormationDistance(formation, unitSpacing) + formation.UnitDiameter;
			float circumferenceAuxOfCircularFormation = Utility.GetCircumferenceAuxOfCircularFormation(unitCountWithOverride, maximumRankCountOfCircularFormation, num, num2);
			float num3 = MathF.Max(0f, (float)unitCountWithOverride * num);
			circumference = MBMath.ClampFloat(circumference, circumferenceAuxOfCircularFormation, num3);
			return Math.Max(circumference - Utility.GetFormationInterval(formation, unitSpacing), formation.UnitDiameter) / 3.1415927f;
		}

		// Token: 0x060002DF RID: 735 RVA: 0x0000C8DC File Offset: 0x0000AADC
		public static float GetMinimumWidthOfSquareFormation(Formation formation)
		{
			int num;
			return Utility.GetSideWidthFromUnitCountOfSquareFormation(Utility.GetUnitsPerSideFromRankCountOfSquareFormation(formation, Utility.GetMaximumRankCountOfSquareFormation(Utility.GetUnitCountWithOverride(formation), out num)), formation.MinimumInterval, formation.UnitDiameter);
		}

		// Token: 0x060002E0 RID: 736 RVA: 0x0000C910 File Offset: 0x0000AB10
		public static float GetMaximumWidthOfSquareFormation(Formation formation)
		{
			if (MissionConfigBase<CommandSystemConfig>.Get().HollowSquare && Utility.ShouldEnableHollowSquareFormationFor(formation))
			{
				return Utility.GetSideWidthFromUnitCountOfSquareFormation(Utility.GetUnitsPerSideFromRankCountOfSquareFormation(formation, 1), Utility.GetFormationInterval(formation, ArrangementOrder.GetUnitSpacingOf(7)), formation.UnitDiameter);
			}
			int num;
			return Utility.GetSideWidthFromUnitCountOfSquareFormation(Utility.GetUnitsPerSideFromRankCountOfSquareFormation(formation, Utility.GetMaximumRankCountOfSquareFormation(Utility.GetUnitCountWithOverride(formation), out num)), formation.MaximumInterval, formation.UnitDiameter);
		}

		// Token: 0x060002E1 RID: 737 RVA: 0x0000C974 File Offset: 0x0000AB74
		private static int GetUnitsPerSideFromRankCountOfSquareFormation(Formation formation, int rankCount)
		{
			int unitCountWithOverride = Utility.GetUnitCountWithOverride(formation);
			int num;
			rankCount = MathF.Min(Utility.GetMaximumRankCountOfSquareFormation(unitCountWithOverride, out num), rankCount);
			double num2 = (double)unitCountWithOverride / (4.0 * (double)rankCount) + (double)rankCount;
			int num3 = MathF.Ceiling((float)num2);
			int num4 = MathF.Floor(num2);
			if (num4 < num3 && (num4 * num4 == unitCountWithOverride || (rankCount == 1 && unitCountWithOverride > 10)))
			{
				num3 = num4;
			}
			if (num3 == 0)
			{
				num3 = 1;
			}
			return num3;
		}

		// Token: 0x060002E2 RID: 738 RVA: 0x0000C9D4 File Offset: 0x0000ABD4
		private static int GetMaximumRankCountOfSquareFormation(int unitCount, out int minimumFlankCount)
		{
			int num = (int)MathF.Sqrt((float)unitCount);
			if (num * num != unitCount)
			{
				num++;
			}
			minimumFlankCount = num;
			return MathF.Max(1, (num + 1) / 2);
		}

		// Token: 0x060002E3 RID: 739 RVA: 0x0000CA02 File Offset: 0x0000AC02
		private static float GetSideWidthFromUnitCountOfSquareFormation(int sideUnitCount, float interval, float unitDiameter)
		{
			if (sideUnitCount <= 0)
			{
				return 0f;
			}
			return (float)(sideUnitCount - 1) * (interval + unitDiameter) + unitDiameter;
		}

		// Token: 0x060002E4 RID: 740 RVA: 0x0000CA18 File Offset: 0x0000AC18
		public static float ConvertFromWidthToFlankWidthOfSquareFormation(Formation formation, int unitSpacing, float width)
		{
			return (width - formation.UnitDiameter) * 4f + Utility.GetFormationInterval(formation, unitSpacing);
		}

		// Token: 0x060002E5 RID: 741 RVA: 0x0000CA30 File Offset: 0x0000AC30
		public static float ConvertFromFlankWidthToWidthOfSquareFormation(Formation formation, int unitSpacing, float flankWidth)
		{
			return (flankWidth + Utility.GetFormationInterval(formation, unitSpacing)) / 4f + formation.UnitDiameter;
		}

		// Token: 0x060002E6 RID: 742 RVA: 0x0000CA48 File Offset: 0x0000AC48
		public static float ConvertFromWidthToFlankWidthOfCircularFormation(Formation formation, int unitSpacing, float width)
		{
			return width * 3.1415927f - Utility.GetFormationInterval(formation, unitSpacing);
		}

		// Token: 0x060002E7 RID: 743 RVA: 0x0000CA59 File Offset: 0x0000AC59
		public static float ConvertFromFlankWidthToWidthOfCircularFormation(Formation formation, int unitSpacing, float flankWidth)
		{
			return 2f * (float)(((double)flankWidth + (double)Utility.GetFormationInterval(formation, unitSpacing)) / 6.2831854820251465);
		}

		// Token: 0x060002E8 RID: 744 RVA: 0x0000CA77 File Offset: 0x0000AC77
		public static int GetFileCountFromWidth(Formation formation, float flankWidth, int unitSpacing)
		{
			return MathF.Max(Utility.GetUnlimitedFileCountFromWidth(formation, flankWidth, unitSpacing), (formation.Arrangement is ColumnFormation) ? 1 : ((int)Utility.MinimumFileCount.GetValue(formation.Arrangement)));
		}

		// Token: 0x060002E9 RID: 745 RVA: 0x0000CAAB File Offset: 0x0000ACAB
		public static int GetUnlimitedFileCountFromWidth(Formation formation, float flankWidth, int unitSpacing)
		{
			return MathF.Max(0, (int)(((double)flankWidth - (double)formation.UnitDiameter) / ((double)Utility.GetFormationInterval(formation, unitSpacing) + (double)formation.UnitDiameter) + 9.999999747378752E-06)) + 1;
		}

		// Token: 0x060002EA RID: 746 RVA: 0x0000CADB File Offset: 0x0000ACDB
		public static float GetFlankWidthFromFileCount(Formation formation, int fileCount, int unitSpacing)
		{
			return (float)MathF.Max(0, fileCount - 1) * (Utility.GetFormationInterval(formation, unitSpacing) + formation.UnitDiameter) + formation.UnitDiameter;
		}

		// Token: 0x060002EB RID: 747 RVA: 0x0000CAFD File Offset: 0x0000ACFD
		public static float GetMinimumWidthOfColumnFormation(Formation formation, int unitSpacing)
		{
			return (float)(MathF.Max(1, MathF.Ceiling(MathF.Sqrt((float)(formation.Arrangement.UnitCount / ColumnFormation.ArrangementAspectRatio)))) - 1) * (formation.UnitDiameter + Utility.GetFormationInterval(formation, unitSpacing)) + formation.UnitDiameter;
		}

		// Token: 0x060002EC RID: 748 RVA: 0x0000CB3A File Offset: 0x0000AD3A
		public static float GetMaximumWidthOfColumnFormation(Formation formation, int unitSpacing)
		{
			return (float)(formation.Arrangement.UnitCount - 1) * (formation.UnitDiameter + Utility.GetFormationInterval(formation, unitSpacing)) + formation.UnitDiameter;
		}

		// Token: 0x060002ED RID: 749 RVA: 0x0000CB60 File Offset: 0x0000AD60
		public static void UpdateActiveOrders()
		{
			GauntletOrderUIHandler missionBehavior = Mission.Current.GetMissionBehavior<GauntletOrderUIHandler>();
			if (missionBehavior == null)
			{
				return;
			}
			(typeof(GauntletOrderUIHandler).GetField("_dataSource", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(missionBehavior) as MissionOrderVM).SetActiveOrders();
		}

		// Token: 0x060002EE RID: 750 RVA: 0x0000CBA4 File Offset: 0x0000ADA4
		public static bool ShouldEnablePlayerOrderControllerPatchForFormation(IEnumerable<Formation> selectedFormations)
		{
			Formation formation = selectedFormations.FirstOrDefault<Formation>();
			Team team = ((formation != null) ? formation.Team : null);
			if (!selectedFormations.All<Formation>((Formation f) => !f.IsAIControlled) || team == null || team != Mission.Current.PlayerTeam)
			{
				return false;
			}
			if (team.IsPlayerGeneral)
			{
				return true;
			}
			if (team.IsPlayerSergeant)
			{
				return selectedFormations.All<Formation>((Formation f) => f.PlayerOwner == Agent.Main);
			}
			return false;
		}

		// Token: 0x060002EF RID: 751 RVA: 0x0000CC38 File Offset: 0x0000AE38
		public static bool ShouldEnablePlayerOrderControllerPatchForFormation(Formation formation)
		{
			Team team = formation.Team;
			return !formation.IsAIControlled && team != null && team == Mission.Current.PlayerTeam && (team.IsPlayerGeneral || (team.IsPlayerSergeant && formation.PlayerOwner == Agent.Main));
		}

		// Token: 0x060002F0 RID: 752 RVA: 0x0000CC88 File Offset: 0x0000AE88
		public static bool ShouldEnableHollowSquareFormationFor(Formation formation)
		{
			Team team = ((formation != null) ? formation.Team : null);
			return !formation.IsAIControlled && team != null && team == Mission.Current.PlayerTeam && (team.IsPlayerGeneral || (team.IsPlayerSergeant && formation.PlayerOwner == Agent.Main));
		}

		// Token: 0x060002F1 RID: 753 RVA: 0x0000CCE0 File Offset: 0x0000AEE0
		public static Vec3 GetColumnFormationCurrentPosition(Formation formation)
		{
			ColumnFormation columnFormation = formation.Arrangement as ColumnFormation;
			if (columnFormation != null)
			{
				Agent agent = (columnFormation.GetUnit(columnFormation.VanguardFileIndex, 0) ?? columnFormation.Vanguard) as Agent;
				if (agent != null)
				{
					return agent.Position;
				}
			}
			return Vec3.Invalid;
		}

		// Token: 0x060002F2 RID: 754 RVA: 0x0000CD2C File Offset: 0x0000AF2C
		public unsafe static bool DoesFormationHasOrderType(Formation formation, OrderType type)
		{
			MovementOrder movementOrder = *formation.GetReadonlyMovementOrderReference();
			switch (type)
			{
			case 14:
			case 15:
				return OrderController.GetActiveFacingOrderOf(formation) == type;
			case 16:
			case 17:
			case 18:
			case 19:
			case 20:
			case 21:
			case 22:
			case 23:
				return OrderController.GetActiveArrangementOrderOf(formation) == type;
			case 31:
			case 32:
				return OrderController.GetActiveFiringOrderOf(formation) == type;
			case 34:
			case 35:
				return OrderController.GetActiveRidingOrderOf(formation) == type;
			case 36:
			case 37:
				return OrderController.GetActiveAIControlOrderOf(formation) == type;
			}
			return movementOrder.OrderType == type || formation.ArrangementOrder.OrderType == type || formation.FacingOrder.OrderType == type || formation.FiringOrder.OrderType == type || formation.FormOrder.OrderType == type || formation.RidingOrder.OrderType == type;
		}

		// Token: 0x060002F3 RID: 755 RVA: 0x0000CE48 File Offset: 0x0000B048
		public static bool DoesFormationHasVolleyOrder(Formation formation, VolleyMode volleyMode)
		{
			FormationChange formationChange;
			if (!Utility.ShouldQueueCommand() || !CommandQueueLogic.LatestOrderInQueueChanges.VirtualChanges.TryGetValue(formation, out formationChange))
			{
				return CommandQueueLogic.GetFormationVolleyMode(formation) == volleyMode;
			}
			if (formationChange.VolleyMode != null)
			{
				VolleyMode? volleyMode2 = formationChange.VolleyMode;
				return (volleyMode2.GetValueOrDefault() == volleyMode) & (volleyMode2 != null);
			}
			return volleyMode == VolleyMode.Disabled;
		}

		// Token: 0x060002F4 RID: 756 RVA: 0x0000CEA8 File Offset: 0x0000B0A8
		public static bool ShouldQueueCommand()
		{
			Mission mission = Mission.Current;
			return (mission == null || !mission.IsNavalBattle) && CommandSystemGameKeyCategory.GetKey(GameKeyEnum.CommandQueue).IsKeyDownInOrder(null);
		}

		// Token: 0x060002F5 RID: 757 RVA: 0x0000CECC File Offset: 0x0000B0CC
		public static void GetMaxAndCurrentAmmoOfAgent(Agent agent, out int currentAmmo, out int maxAmmo)
		{
			currentAmmo = 0;
			maxAmmo = 0;
			for (EquipmentIndex equipmentIndex = 0; equipmentIndex < 4; equipmentIndex++)
			{
				if (!agent.Equipment[equipmentIndex].IsEmpty && agent.Equipment[equipmentIndex].CurrentUsageItem.IsRangedWeapon)
				{
					currentAmmo = agent.Equipment.GetAmmoAmount(equipmentIndex);
					maxAmmo = agent.Equipment.GetMaxAmmo(equipmentIndex);
				}
			}
		}

		// Token: 0x060002F6 RID: 758 RVA: 0x0000CF38 File Offset: 0x0000B138
		public static void ExecuteAutoVolley()
		{
			MissionOrderVM missionOrderVM = Utility.GetMissionOrderVM(Mission.Current);
			if (missionOrderVM == null)
			{
				return;
			}
			OrderItemVM orderItemVM = Utility.FindOrderWithId(missionOrderVM, "order_auto_volley");
			if (orderItemVM != null)
			{
				orderItemVM.ExecuteAction(new VisualOrderExecutionParameters(Agent.Main, null, null));
			}
		}

		// Token: 0x060002F7 RID: 759 RVA: 0x0000CF80 File Offset: 0x0000B180
		public static void ExecuteManualVolley()
		{
			MissionOrderVM missionOrderVM = Utility.GetMissionOrderVM(Mission.Current);
			if (missionOrderVM == null)
			{
				return;
			}
			OrderItemVM orderItemVM = Utility.FindOrderWithId(missionOrderVM, "order_manual_volley");
			if (orderItemVM != null)
			{
				orderItemVM.ExecuteAction(new VisualOrderExecutionParameters(Agent.Main, null, null));
			}
		}

		// Token: 0x060002F8 RID: 760 RVA: 0x0000CFC8 File Offset: 0x0000B1C8
		public static void ExecuteVolleyFire()
		{
			MissionOrderVM missionOrderVM = Utility.GetMissionOrderVM(Mission.Current);
			if (missionOrderVM == null)
			{
				return;
			}
			OrderItemVM orderItemVM = Utility.FindOrderWithId(missionOrderVM, "order_volley_fire");
			if (orderItemVM != null)
			{
				orderItemVM.ExecuteAction(new VisualOrderExecutionParameters(Agent.Main, null, null));
			}
		}

		// Token: 0x060002F9 RID: 761 RVA: 0x0000D010 File Offset: 0x0000B210
		public static Dictionary<Formation, Vec2> CollectFormationCurrentAndOrderPositions(IEnumerable<Formation> formations, out Vec2 weightedAverageOrderPosition, out Vec2 weightedAverageCurrentPosition)
		{
			Dictionary<Formation, Vec2> dictionary = new Dictionary<Formation, Vec2>();
			int num = 0;
			weightedAverageOrderPosition = Vec2.Zero;
			weightedAverageCurrentPosition = Vec2.Zero;
			float num2 = float.MaxValue;
			Vec2 zero = Vec2.Zero;
			foreach (Formation formation in formations)
			{
				Vec2 orderPosition = formation.OrderPosition;
				Vec2 currentPosition = formation.CurrentPosition;
				if (orderPosition.IsValid && currentPosition.IsValid)
				{
					if (num2 > formation.CachedMovementSpeed)
					{
						num2 = formation.CachedMovementSpeed;
					}
					int countOfUnitsWithoutDetachedOnes = formation.CountOfUnitsWithoutDetachedOnes;
					weightedAverageOrderPosition += orderPosition * (float)countOfUnitsWithoutDetachedOnes;
					weightedAverageCurrentPosition += currentPosition * (float)countOfUnitsWithoutDetachedOnes;
					num += countOfUnitsWithoutDetachedOnes;
				}
				dictionary.Add(formation, orderPosition);
			}
			if (num > 0)
			{
				weightedAverageOrderPosition = weightedAverageOrderPosition * 1f / (float)num;
				weightedAverageCurrentPosition = weightedAverageCurrentPosition * 1f / (float)num;
			}
			return dictionary;
		}

		// Token: 0x060002FA RID: 762 RVA: 0x0000D148 File Offset: 0x0000B348
		public static Vec2 GetExpectedGlobalPositionOfUnit(Vec2 expectedFormationPosition, Formation formation, Agent unit, bool blendWithOrderDirection)
		{
			if (unit.IsDetachedFromFormation)
			{
				return unit.Position.AsVec2;
			}
			Vec2? localPositionOfUnitOrDefaultWithAdjustment = formation.Arrangement.GetLocalPositionOfUnitOrDefaultWithAdjustment(unit, blendWithOrderDirection ? ((formation.QuerySystem.EstimatedInterval - formation.Interval) * 0.9f) : 0f);
			if (localPositionOfUnitOrDefaultWithAdjustment != null)
			{
				return (blendWithOrderDirection ? formation.CurrentDirection : formation.QuerySystem.EstimatedDirection).TransformToParentUnitF(localPositionOfUnitOrDefaultWithAdjustment.Value) + expectedFormationPosition;
			}
			return unit.Position.AsVec2;
		}

		// Token: 0x060002FB RID: 763 RVA: 0x0000D1E0 File Offset: 0x0000B3E0
		public static void MissionOrderVM_OnOrderExecutedWithId(string id)
		{
			MissionOrderVM missionOrderVM = Utility.GetMissionOrderVM(Mission.Current);
			OrderItemVM orderItemVM = Utility.FindOrderWithId(missionOrderVM, id);
			if (orderItemVM != null)
			{
				missionOrderVM.OnOrderExecuted(orderItemVM);
			}
		}

		// Token: 0x060002FC RID: 764 RVA: 0x0000D20A File Offset: 0x0000B40A
		public static bool ShouldFadeOut()
		{
			return MissionConfigBase<CommandSystemConfig>.Get().MovementTargetFadeOutDuration > 0.1f;
		}

		// Token: 0x060002FD RID: 765 RVA: 0x0000D21D File Offset: 0x0000B41D
		public static bool FormationArrangementContainsPlayerOnly(Formation formation)
		{
			return formation.CountOfUnitsWithoutLooseDetachedOnes == ((formation.IsPlayerTroopInFormation || formation.HasPlayerControlledTroop) ? 1 : 0);
		}

		// Token: 0x0400012C RID: 300
		public static Color MessageColor = new Color(0.2f, 0.9f, 0.7f, 1f);

		// Token: 0x0400012D RID: 301
		public static PropertyInfo MinimumFileCount = typeof(LineFormation).GetProperty("MinimumFileCount", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x0400012E RID: 302
		public static MethodInfo BeforeSetOrder = typeof(OrderController).GetMethod("BeforeSetOrder", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x0400012F RID: 303
		public static MethodInfo AfterSetOrder = typeof(OrderController).GetMethod("AfterSetOrder", BindingFlags.Instance | BindingFlags.NonPublic);
	}
}
