using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using MissionLibrary.Event;
using MissionSharedLibrary.Config;
using MissionSharedLibrary.Utilities;
using RTSCamera.CommandSystem.AgentComponents;
using RTSCamera.CommandSystem.Config;
using RTSCamera.CommandSystem.Patch;
using RTSCamera.CommandSystem.QuerySystem;
using RTSCamera.CommandSystem.Utilities;
using RTSCamera.CommandSystem.View;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews.Order;

namespace RTSCamera.CommandSystem.Logic
{
	// Token: 0x02000081 RID: 129
	public static class CommandQueueLogic
	{
		// Token: 0x060004A6 RID: 1190 RVA: 0x0001A928 File Offset: 0x00018B28
		public static void OnBehaviorInitialize()
		{
			CommandQueueLogic.OrderQueue = new List<OrderInQueue>();
			CommandQueueLogic.PendingOrders = new Dictionary<Formation, OrderInQueue>();
			CommandQueueLogic.ShouldSkipCurrentOrders = new Dictionary<Formation, bool>();
			CommandQueueLogic.CurrentFormationChanges = new FormationChanges();
			CommandQueueLogic.LatestOrderInQueueChanges = new FormationChanges();
			CommandQueueLogic.FormationVolleyMode = new Dictionary<Formation, VolleyMode>();
			MissionEvent.PreSwitchTeam += CommandQueueLogic.OnPreSwitchTeam;
			MissionEvent.PostSwitchTeam += CommandQueueLogic.OnPostSwitchTeam;
		}

		// Token: 0x060004A7 RID: 1191 RVA: 0x0001A994 File Offset: 0x00018B94
		public static void OnRemoveBehavior()
		{
			CommandQueueLogic.OrderQueue = null;
			CommandQueueLogic.PendingOrders = null;
			CommandQueueLogic.ShouldSkipCurrentOrders = null;
			CommandQueueLogic.CurrentFormationChanges = null;
			CommandQueueLogic.LatestOrderInQueueChanges = null;
			CommandQueueLogic.FormationVolleyMode = null;
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
				orderController2.OnOrderIssued -= new OnOrderIssuedDelegate(CommandQueueLogic.OnOrderIssued);
			}
			MissionEvent.PreSwitchTeam -= CommandQueueLogic.OnPreSwitchTeam;
			MissionEvent.PostSwitchTeam -= CommandQueueLogic.OnPostSwitchTeam;
		}

		// Token: 0x060004A8 RID: 1192 RVA: 0x0001AA1C File Offset: 0x00018C1C
		public static void AfterStart()
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
				orderController2.OnOrderIssued += new OnOrderIssuedDelegate(CommandQueueLogic.OnOrderIssued);
			}
		}

		// Token: 0x060004A9 RID: 1193 RVA: 0x0001AA5C File Offset: 0x00018C5C
		private static void OnPreSwitchTeam()
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
				orderController2.OnOrderIssued -= new OnOrderIssuedDelegate(CommandQueueLogic.OnOrderIssued);
			}
		}

		// Token: 0x060004AA RID: 1194 RVA: 0x0001AA9C File Offset: 0x00018C9C
		private static void OnPostSwitchTeam()
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
				orderController2.OnOrderIssued += new OnOrderIssuedDelegate(CommandQueueLogic.OnOrderIssued);
			}
		}

		// Token: 0x060004AB RID: 1195 RVA: 0x0001AADC File Offset: 0x00018CDC
		public static bool ShouldClearQueue(OrderType orderType)
		{
			switch (orderType)
			{
			case 0:
			case 1:
			case 2:
			case 3:
			case 4:
			case 5:
			case 6:
			case 7:
			case 8:
			case 9:
			case 10:
			case 11:
			case 12:
			case 13:
			case 14:
			case 15:
			case 16:
			case 17:
			case 18:
			case 19:
			case 20:
			case 21:
			case 22:
			case 23:
			case 24:
			case 25:
			case 26:
			case 27:
			case 28:
			case 29:
			case 30:
			case 31:
			case 32:
			case 33:
			case 34:
			case 35:
			case 36:
			case 40:
			case 41:
				return true;
			}
			return false;
		}

		// Token: 0x060004AC RID: 1196 RVA: 0x0001AB9C File Offset: 0x00018D9C
		public static bool ShouldClearPendingOrder(OrderType orderType)
		{
			switch (orderType)
			{
			case 1:
			case 2:
			case 3:
			case 4:
			case 5:
			case 6:
			case 7:
			case 8:
			case 9:
			case 10:
			case 11:
			case 12:
			case 13:
			case 14:
			case 15:
			case 36:
			case 40:
			case 41:
				return true;
			}
			return false;
		}

		// Token: 0x060004AD RID: 1197 RVA: 0x0001AC5C File Offset: 0x00018E5C
		public static bool ShouldCustomOrderClearQueue(OrderInQueue order)
		{
			switch (order.CustomOrderType)
			{
			case CustomOrderType.SetTargetFormation:
			case CustomOrderType.AutoVolley:
			case CustomOrderType.ManualVolley:
			case CustomOrderType.DisableVolley:
				return true;
			case CustomOrderType.VolleyFire:
				foreach (Formation formation in order.SelectedFormations)
				{
					if (formation.FiringOrder == FiringOrder.FiringOrderHoldYourFire || CommandQueueLogic.GetFormationVolleyMode(formation) != VolleyMode.Manual)
					{
						return true;
					}
				}
				return false;
			case CustomOrderType.StopUsing:
				return false;
			default:
				MissionSharedLibrary.Utilities.Utility.DisplayMessage("Error: unexpected order type");
				return false;
			}
		}

		// Token: 0x060004AE RID: 1198 RVA: 0x0001AD04 File Offset: 0x00018F04
		private static void OnOrderIssued(OrderType orderType, MBReadOnlyList<Formation> appliedFormations, OrderController orderController, params object[] delegateParams)
		{
			CommandQueueLogic.CurrentFormationChanges.SetChanges(Patch_OrderController.LivePreviewFormationChanges.CollectChanges(appliedFormations));
			if (CommandQueueLogic.ShouldClearQueue(orderType))
			{
				CommandQueueLogic.ClearOrderInQueue(appliedFormations);
			}
			using (List<Formation>.Enumerator enumerator = appliedFormations.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (CommandQueueLogic.GetNextOrderForFormation(enumerator.Current) == null)
					{
						CommandQueueLogic.LatestOrderInQueueChanges.SetChanges(CommandQueueLogic.CurrentFormationChanges.CollectChanges(appliedFormations));
					}
				}
			}
		}

		// Token: 0x060004AF RID: 1199 RVA: 0x0001AD8C File Offset: 0x00018F8C
		public static void OnCustomOrderIssued(OrderInQueue order, OrderController orderController)
		{
			CommandQueueLogic.CurrentFormationChanges.SetChanges(Patch_OrderController.LivePreviewFormationChanges.CollectChanges(order.SelectedFormations));
			if (CommandQueueLogic.ShouldCustomOrderClearQueue(order))
			{
				CommandQueueLogic.ClearOrderInQueue(order.SelectedFormations);
			}
			using (List<Formation>.Enumerator enumerator = order.SelectedFormations.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (CommandQueueLogic.GetNextOrderForFormation(enumerator.Current) == null)
					{
						CommandQueueLogic.LatestOrderInQueueChanges.SetChanges(CommandQueueLogic.CurrentFormationChanges.CollectChanges(order.SelectedFormations));
					}
				}
			}
		}

		// Token: 0x060004B0 RID: 1200 RVA: 0x0001AE28 File Offset: 0x00019028
		public static void AddOrderToQueue(OrderInQueue order)
		{
			if (order.CustomOrderType == CustomOrderType.Original && order.OrderType == null)
			{
				return;
			}
			order.RemainingFormations = order.SelectedFormations.ToList<Formation>();
			CommandQueueLogic.LatestOrderInQueueChanges.SetChanges(Patch_OrderController.LivePreviewFormationChanges.CollectChanges(order.SelectedFormations));
			CommandQueueLogic.OrderQueue.Add(order);
			RTSCamera.CommandSystem.Utilities.Utility.DisplayAddOrderToQueueMessage();
			CommandQueuePreview.IsPreviewOutdated = true;
		}

		// Token: 0x060004B1 RID: 1201 RVA: 0x0001AE88 File Offset: 0x00019088
		public static void ClearOrderInQueue(IEnumerable<Formation> formations)
		{
			Predicate<Formation> <>9__0;
			Predicate<Formation> <>9__1;
			foreach (OrderInQueue orderInQueue in CommandQueueLogic.OrderQueue.ToList<OrderInQueue>())
			{
				List<Formation> selectedFormations = orderInQueue.SelectedFormations;
				Predicate<Formation> predicate;
				if ((predicate = <>9__0) == null)
				{
					predicate = (<>9__0 = (Formation f) => formations.Contains(f));
				}
				selectedFormations.RemoveAll(predicate);
				List<Formation> remainingFormations = orderInQueue.RemainingFormations;
				Predicate<Formation> predicate2;
				if ((predicate2 = <>9__1) == null)
				{
					predicate2 = (<>9__1 = (Formation f) => formations.Contains(f));
				}
				remainingFormations.RemoveAll(predicate2);
				if (orderInQueue.SelectedFormations.Count == 0)
				{
					CommandQueueLogic.OrderQueue.Remove(orderInQueue);
				}
			}
			CommandQueuePreview.IsPreviewOutdated = true;
		}

		// Token: 0x060004B2 RID: 1202 RVA: 0x0001AF5C File Offset: 0x0001915C
		public unsafe static void UpdateFormation(Formation formation)
		{
			try
			{
				if (!Mission.Current.IsNavalBattle)
				{
					CommandQueueLogic.TickVolley(formation);
					if (CommandQueueLogic.TicksToSkip > 0)
					{
						CommandQueueLogic.TicksToSkip--;
					}
					else
					{
						Formation facingEnemyTargetFormation = Patch_OrderController.GetFacingEnemyTargetFormation(formation);
						if (facingEnemyTargetFormation != null && facingEnemyTargetFormation.CountOfUnits == 0)
						{
							Patch_OrderController.SetFacingEnemyTargetFormation(formation, null);
						}
						CommandQueueLogic.UpdatePendingOrder(formation);
						OrderInQueue orderInQueue = CommandQueueLogic.GetNextOrderForFormation(formation);
						MovementOrder movementOrder = *formation.GetReadonlyMovementOrderReference();
						bool flag = movementOrder.IsApplicable(formation);
						bool flag2 = CommandQueueLogic.IsPendingOrderCompleted(formation);
						bool flag3 = CommandQueueLogic.IsReadyToExecuteOrder(formation, orderInQueue);
						while (CommandQueueLogic.TicksToSkip <= 0 && orderInQueue != null && (!flag || flag2) && flag3)
						{
							CommandQueueLogic.ExecuteOrderForFormation(orderInQueue, formation);
							CommandQueueLogic.OnOrderExecutedForFormation(orderInQueue, formation);
							CommandQueueLogic.UpdatePendingOrder(formation);
							orderInQueue = CommandQueueLogic.GetNextOrderForFormation(formation);
							movementOrder = *formation.GetReadonlyMovementOrderReference();
							flag = movementOrder.IsApplicable(formation);
							flag2 = CommandQueueLogic.IsPendingOrderCompleted(formation);
							flag3 = CommandQueueLogic.IsReadyToExecuteOrder(formation, orderInQueue);
						}
					}
				}
			}
			catch (Exception ex)
			{
				MissionSharedLibrary.Utilities.Utility.DisplayMessageForced(ex.ToString());
			}
		}

		// Token: 0x060004B3 RID: 1203 RVA: 0x0001B060 File Offset: 0x00019260
		public unsafe static bool IsMovementOrderCompleted(Formation formation, OrderInQueue order)
		{
			if (formation.CountOfUnits == 0)
			{
				return true;
			}
			switch (formation.GetReadonlyMovementOrderReference().OrderEnum)
			{
			case 1:
			case 4:
			case 5:
			{
				if (order == null)
				{
					MovementOrder movementOrder = *formation.GetReadonlyMovementOrderReference();
					return !movementOrder.IsApplicable(formation);
				}
				UsableMachine usableMachine = order.TargetEntity as UsableMachine;
				if (usableMachine == null)
				{
					MovementOrder movementOrder = *formation.GetReadonlyMovementOrderReference();
					return !movementOrder.IsApplicable(formation);
				}
				return usableMachine.IsDestroyed;
			}
			case 2:
			case 3:
				return formation.TargetFormation == null || formation.TargetFormation.CountOfUnits == 0;
			case 11:
				return true;
			}
			if (formation.ArrangementOrder.OrderEnum == 1)
			{
				return RTSCamera.CommandSystem.Utilities.Utility.GetColumnFormationCurrentPosition(formation).Distance(formation.OrderGroundPosition) < 5f;
			}
			return !formation.OrderPositionIsValid || CommandQuerySystem.GetQueryForFormation(formation).HasCurrentMovementOrderCompleted;
		}

		// Token: 0x060004B4 RID: 1204 RVA: 0x0001B15C File Offset: 0x0001935C
		public static void UpdatePendingOrder(Formation formation)
		{
			OrderInQueue orderInQueue;
			if (CommandQueueLogic.PendingOrders.TryGetValue(formation, out orderInQueue))
			{
				orderInQueue.UpdateMovementSpeed();
				if (orderInQueue.ShouldAdjustFormationSpeed && !orderInQueue.IsAdjustingSpeedMessageShown && orderInQueue.RemainingFormations.Count == 0 && MissionConfigBase<CommandSystemConfig>.Get().FormationSpeedSyncMode != FormationSpeedSyncMode.Disabled && orderInQueue.FormationTargetDistances.Count > 1)
				{
					orderInQueue.IsAdjustingSpeedMessageShown = true;
					RTSCamera.CommandSystem.Utilities.Utility.DisplayAdjustFormationSpeedMessage(orderInQueue.FormationTargetDistances.Keys);
				}
				foreach (Formation formation2 in orderInQueue.SelectedFormations)
				{
					OrderInQueue orderInQueue2;
					if (formation2 != formation && CommandQueueLogic.PendingOrders.TryGetValue(formation2, out orderInQueue2))
					{
						if (orderInQueue2 != orderInQueue)
						{
							return;
						}
						if (!CommandQueueLogic.IsMovementOrderCompleted(formation2, orderInQueue))
						{
							return;
						}
					}
					if (formation2 == formation && !CommandQueueLogic.IsMovementOrderCompleted(formation, orderInQueue))
					{
						return;
					}
				}
				CommandQueueLogic.PendingOrders.Remove(formation);
			}
		}

		// Token: 0x060004B5 RID: 1205 RVA: 0x0001B250 File Offset: 0x00019450
		public static bool IsPendingOrderCompleted(Formation formation)
		{
			bool flag;
			if (CommandQueueLogic.ShouldSkipCurrentOrders.TryGetValue(formation, out flag) && flag)
			{
				CommandQueueLogic.ShouldSkipCurrentOrders[formation] = false;
				return true;
			}
			OrderInQueue orderInQueue;
			CommandQueueLogic.PendingOrders.TryGetValue(formation, out orderInQueue);
			return CommandQueueLogic.IsMovementOrderCompleted(formation, orderInQueue);
		}

		// Token: 0x060004B6 RID: 1206 RVA: 0x0001B294 File Offset: 0x00019494
		public static bool IsReadyToExecuteOrder(Formation formation, OrderInQueue order)
		{
			if (order == null)
			{
				return false;
			}
			foreach (Formation formation2 in order.RemainingFormations)
			{
				if (CommandQueueLogic.PendingOrders.ContainsKey(formation2))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060004B7 RID: 1207 RVA: 0x0001B2FC File Offset: 0x000194FC
		public unsafe static void ExecuteOrderForFormation(OrderInQueue order, Formation formation)
		{
			switch (order.CustomOrderType)
			{
			case CustomOrderType.Original:
				switch (order.OrderType)
				{
				case 1:
					formation.SetMovementOrder(MovementOrder.MovementOrderMove(order.PositionBegin));
					CommandQueueLogic.CurrentFormationChanges.SetChanges(order.VirtualFormationChanges.Where<KeyValuePair<Formation, FormationChange>>((KeyValuePair<Formation, FormationChange> pair) => pair.Key == formation));
					return;
				case 2:
				case 3:
				{
					ValueTuple<Formation, int, float, WorldPosition, Vec2> valueTuple = order.ActualFormationChanges.First<ValueTuple<Formation, int, float, WorldPosition, Vec2>>(([TupleElementNames(new string[] { "formation", "unitSpacingReduced", "customWidth", "position", "direction" })] ValueTuple<Formation, int, float, WorldPosition, Vec2> c) => c.Item1 == formation);
					float item = valueTuple.Item3;
					WorldPosition item2 = valueTuple.Item4;
					Vec2 item3 = valueTuple.Item5;
					FormationChange formationChange = order.VirtualFormationChanges[formation];
					int unitSpacing = formation.UnitSpacing;
					int? num = formationChange.UnitSpacing;
					if (!((unitSpacing == num.GetValueOrDefault()) & (num != null)))
					{
						Formation formation2 = formation;
						num = formationChange.UnitSpacing;
						formation2.SetPositioning(null, null, num);
					}
					if (order.IsLineShort)
					{
						if (formationChange.Width != null)
						{
							float width = formation.Width;
							float? width2 = formationChange.Width;
							if (!((width == width2.GetValueOrDefault()) & (width2 != null)) && formation.ArrangementOrder.OrderEnum != 1)
							{
								formation.SetFormOrder(FormOrder.FormOrderCustom(formationChange.Width.Value), true);
							}
						}
						OrderType activeFacingOrderOf = OrderController.GetActiveFacingOrderOf(formation);
						if (activeFacingOrderOf != 14)
						{
							if (activeFacingOrderOf == 15)
							{
								formation.SetMovementOrder(MovementOrder.MovementOrderMove(item2));
								formation.SetFacingOrder(FacingOrder.FacingOrderLookAtDirection(item3));
							}
						}
						else
						{
							formation.SetMovementOrder(MovementOrder.MovementOrderMove(item2));
						}
					}
					else
					{
						formation.SetMovementOrder(MovementOrder.MovementOrderMove(item2));
						formation.SetFacingOrder(FacingOrder.FacingOrderLookAtDirection(item3));
						formation.SetFormOrder(FormOrder.FormOrderCustom(item), true);
					}
					CommandQueueLogic.TryPendingOrder(new List<Formation> { formation }, order);
					CommandQueueLogic.CurrentFormationChanges.SetChanges(order.VirtualFormationChanges.Where<KeyValuePair<Formation, FormationChange>>((KeyValuePair<Formation, FormationChange> pair) => pair.Key == formation));
					return;
				}
				case 4:
					formation.SetMovementOrder(MovementOrder.MovementOrderCharge);
					if (order.TargetFormation != null)
					{
						formation.SetTargetFormation(order.TargetFormation);
						CommandQueueLogic.TryPendingOrder(new List<Formation> { formation }, order);
					}
					CommandQueueLogic.CurrentFormationChanges.SetChanges(order.VirtualFormationChanges.Where<KeyValuePair<Formation, FormationChange>>((KeyValuePair<Formation, FormationChange> pair) => pair.Key == formation));
					return;
				case 5:
					formation.SetMovementOrder(MovementOrder.MovementOrderChargeToTarget(formation));
					if (order.TargetFormation != null)
					{
						RTSCamera.CommandSystem.Utilities.Utility.DisplayFormationChargeMessage(formation);
						formation.SetTargetFormation(order.TargetFormation);
						CommandQueueLogic.TryPendingOrder(new List<Formation> { formation }, order);
					}
					CommandQueueLogic.CurrentFormationChanges.SetChanges(order.VirtualFormationChanges.Where<KeyValuePair<Formation, FormationChange>>((KeyValuePair<Formation, FormationChange> pair) => pair.Key == formation));
					return;
				case 6:
					formation.SetMovementOrder(MovementOrder.MovementOrderStop);
					CommandQueueLogic.CurrentFormationChanges.SetChanges(order.VirtualFormationChanges.Where<KeyValuePair<Formation, FormationChange>>((KeyValuePair<Formation, FormationChange> pair) => pair.Key == formation));
					return;
				case 7:
					formation.SetMovementOrder(MovementOrder.MovementOrderFollow(order.TargetAgent));
					CommandQueueLogic.TryPendingOrder(new List<Formation> { formation }, order);
					CommandQueueLogic.CurrentFormationChanges.SetChanges(order.VirtualFormationChanges.Where<KeyValuePair<Formation, FormationChange>>((KeyValuePair<Formation, FormationChange> pair) => pair.Key == formation));
					return;
				case 8:
				{
					UsableMachine usableMachine = order.TargetEntity as UsableMachine;
					if (usableMachine.IsDestroyed)
					{
						return;
					}
					if (order.IsStopUsing)
					{
						ModuleExtensions.StopUsingMachine(formation, usableMachine, true);
						SiegeWeapon siegeWeapon = usableMachine as SiegeWeapon;
						if (siegeWeapon != null)
						{
							siegeWeapon.SetForcedUse(false);
						}
						CommandQueueLogic.TryPendingOrder(new List<Formation> { formation }, order);
						CommandQueueLogic.CurrentFormationChanges.SetChanges(order.VirtualFormationChanges.Where<KeyValuePair<Formation, FormationChange>>((KeyValuePair<Formation, FormationChange> pair) => pair.Key == formation));
						return;
					}
					SiegeWeapon siegeWeapon2 = usableMachine as SiegeWeapon;
					if (siegeWeapon2 != null)
					{
						siegeWeapon2.SetForcedUse(true);
					}
					GameEntity waitEntity = usableMachine.WaitEntity;
					Vec2 vec = waitEntity.GetGlobalFrame().rotation.f.AsVec2.Normalized();
					formation.SetFacingOrder(FacingOrder.FacingOrderLookAtDirection(vec));
					formation.SetMovementOrder(MovementOrder.MovementOrderFollowEntity(waitEntity));
					CommandQueueLogic.TryPendingOrder(new List<Formation> { formation }, order);
					CommandQueueLogic.CurrentFormationChanges.SetChanges(order.VirtualFormationChanges.Where<KeyValuePair<Formation, FormationChange>>((KeyValuePair<Formation, FormationChange> pair) => pair.Key == formation));
					return;
				}
				case 9:
					formation.SetMovementOrder(MovementOrder.MovementOrderRetreat);
					CommandQueueLogic.CurrentFormationChanges.SetChanges(order.VirtualFormationChanges.Where<KeyValuePair<Formation, FormationChange>>((KeyValuePair<Formation, FormationChange> pair) => pair.Key == formation));
					return;
				case 12:
					formation.SetMovementOrder(MovementOrder.MovementOrderAdvance);
					if (order.TargetFormation != null)
					{
						formation.SetTargetFormation(order.TargetFormation);
					}
					CommandQueueLogic.TryPendingOrder(new List<Formation> { formation }, order);
					CommandQueueLogic.CurrentFormationChanges.SetChanges(order.VirtualFormationChanges.Where<KeyValuePair<Formation, FormationChange>>((KeyValuePair<Formation, FormationChange> pair) => pair.Key == formation));
					return;
				case 13:
					formation.SetMovementOrder(MovementOrder.MovementOrderFallBack);
					return;
				case 14:
					CommandQueueLogic.TryCancelStopOrder(formation);
					Patch_OrderController.SetFacingEnemyTargetFormation(formation, order.TargetFormation);
					formation.SetFacingOrder(FacingOrder.FacingOrderLookAtEnemy);
					CommandQueueLogic.TryPendingOrder(new List<Formation> { formation }, order);
					CommandQueueLogic.CurrentFormationChanges.SetChanges(order.VirtualFormationChanges.Where<KeyValuePair<Formation, FormationChange>>((KeyValuePair<Formation, FormationChange> pair) => pair.Key == formation));
					return;
				case 15:
				{
					OrderType? movementOrderType = order.VirtualFormationChanges[formation].MovementOrderType;
					OrderType orderType;
					if (movementOrderType == null)
					{
						MovementOrder movementOrder = *formation.GetReadonlyMovementOrderReference();
						orderType = movementOrder.OrderType;
					}
					else
					{
						orderType = movementOrderType.GetValueOrDefault();
					}
					bool flag = !RTSCamera.CommandSystem.Utilities.Utility.IsMovementOrderMoving(new OrderType?(orderType));
					CommandQueueLogic.FacingOrderLookAtDirection(order, formation);
					if (flag)
					{
						CommandQueueLogic.TryPendingOrder(new List<Formation> { formation }, order);
					}
					CommandQueueLogic.CurrentFormationChanges.SetChanges(order.VirtualFormationChanges.Where<KeyValuePair<Formation, FormationChange>>((KeyValuePair<Formation, FormationChange> pair) => pair.Key == formation));
					return;
				}
				case 16:
				case 17:
				case 18:
				case 19:
				case 20:
				case 21:
				case 22:
				case 23:
					CommandQueueLogic.ExecuteArrangementOrder(order, formation);
					return;
				case 31:
					formation.SetFiringOrder(FiringOrder.FiringOrderHoldYourFire);
					CommandQueueLogic.SetFormationVolleyMode(formation, VolleyMode.Disabled);
					CommandQueueLogic.TryPendingOrder(new List<Formation> { formation }, order);
					CommandQueueLogic.CurrentFormationChanges.SetChanges(order.VirtualFormationChanges.Where<KeyValuePair<Formation, FormationChange>>((KeyValuePair<Formation, FormationChange> pair) => pair.Key == formation));
					return;
				case 32:
					formation.SetFiringOrder(FiringOrder.FiringOrderFireAtWill);
					CommandQueueLogic.SetFormationVolleyMode(formation, VolleyMode.Disabled);
					CommandQueueLogic.TryPendingOrder(new List<Formation> { formation }, order);
					CommandQueueLogic.CurrentFormationChanges.SetChanges(order.VirtualFormationChanges.Where<KeyValuePair<Formation, FormationChange>>((KeyValuePair<Formation, FormationChange> pair) => pair.Key == formation));
					return;
				case 34:
					if (TroopClassExtensions.IsMounted(formation.PhysicalClass) || formation.HasAnyMountedUnit)
					{
						CommandQueueLogic.TryCancelStopOrder(formation);
					}
					formation.SetRidingOrder(RidingOrder.RidingOrderMount);
					CommandQueueLogic.TryPendingOrder(new List<Formation> { formation }, order);
					CommandQueueLogic.CurrentFormationChanges.SetChanges(order.VirtualFormationChanges.Where<KeyValuePair<Formation, FormationChange>>((KeyValuePair<Formation, FormationChange> pair) => pair.Key == formation));
					return;
				case 35:
					if (TroopClassExtensions.IsMounted(formation.PhysicalClass) || formation.HasAnyMountedUnit)
					{
						CommandQueueLogic.TryCancelStopOrder(formation);
					}
					formation.SetRidingOrder(RidingOrder.RidingOrderDismount);
					CommandQueueLogic.TryPendingOrder(new List<Formation> { formation }, order);
					CommandQueueLogic.CurrentFormationChanges.SetChanges(order.VirtualFormationChanges.Where<KeyValuePair<Formation, FormationChange>>((KeyValuePair<Formation, FormationChange> pair) => pair.Key == formation));
					return;
				case 36:
					formation.SetControlledByAI(true, false);
					Patch_OrderController.SetFacingEnemyTargetFormation(formation, null);
					CommandQueueLogic.CurrentFormationChanges.SetChanges(order.VirtualFormationChanges.Where<KeyValuePair<Formation, FormationChange>>((KeyValuePair<Formation, FormationChange> pair) => pair.Key == formation));
					CommandQueueLogic.ClearOrderInQueue(new List<Formation> { formation });
					return;
				case 37:
					formation.SetControlledByAI(false, false);
					Patch_OrderController.SetFacingEnemyTargetFormation(formation, null);
					CommandQueueLogic.TryPendingOrder(new List<Formation> { formation }, order);
					CommandQueueLogic.CurrentFormationChanges.SetChanges(order.VirtualFormationChanges.Where<KeyValuePair<Formation, FormationChange>>((KeyValuePair<Formation, FormationChange> pair) => pair.Key == formation));
					return;
				case 39:
				{
					UsableMachine usableMachine2 = order.TargetEntity as UsableMachine;
					if (!usableMachine2.IsDestroyed)
					{
						if (order.IsStopUsing)
						{
							ModuleExtensions.StopUsingMachine(formation, usableMachine2, true);
							SiegeWeapon siegeWeapon3 = usableMachine2 as SiegeWeapon;
							if (siegeWeapon3 != null)
							{
								siegeWeapon3.SetForcedUse(false);
							}
						}
						else
						{
							ModuleExtensions.StartUsingMachine(formation, usableMachine2, true);
							SiegeWeapon siegeWeapon4 = usableMachine2 as SiegeWeapon;
							if (siegeWeapon4 != null)
							{
								siegeWeapon4.SetForcedUse(true);
							}
						}
						CommandQueueLogic.TryPendingOrder(new List<Formation> { formation }, order);
						CommandQueueLogic.CurrentFormationChanges.SetChanges(order.VirtualFormationChanges.Where<KeyValuePair<Formation, FormationChange>>((KeyValuePair<Formation, FormationChange> pair) => pair.Key == formation));
						return;
					}
					return;
				}
				case 40:
				{
					MissionObject missionObject = order.TargetEntity as MissionObject;
					WeakGameEntity gameEntity = missionObject.GameEntity;
					formation.SetMovementOrder(MovementOrder.MovementOrderAttackEntity(GameEntity.CreateFromWeakEntity(gameEntity), !(missionObject is CastleGate)));
					CommandQueueLogic.TryPendingOrder(new List<Formation> { formation }, order);
					CommandQueueLogic.CurrentFormationChanges.SetChanges(order.VirtualFormationChanges.Where<KeyValuePair<Formation, FormationChange>>((KeyValuePair<Formation, FormationChange> pair) => pair.Key == formation));
					return;
				}
				case 41:
				{
					IPointDefendable pointDefendable = order.TargetEntity as IPointDefendable;
					formation.SetMovementOrder(MovementOrder.MovementOrderMove(pointDefendable.MiddleFrame.Origin));
					CommandQueueLogic.TryPendingOrder(new List<Formation> { formation }, order);
					CommandQueueLogic.CurrentFormationChanges.SetChanges(order.VirtualFormationChanges.Where<KeyValuePair<Formation, FormationChange>>((KeyValuePair<Formation, FormationChange> pair) => pair.Key == formation));
					return;
				}
				}
				MissionSharedLibrary.Utilities.Utility.DisplayMessage("Error: unexpected order type");
				return;
			case CustomOrderType.FollowMainAgent:
				break;
			case CustomOrderType.SetTargetFormation:
				formation.SetTargetFormation(order.TargetFormation);
				return;
			case CustomOrderType.AutoVolley:
				CommandQueueLogic.SetFormationVolleyMode(formation, VolleyMode.Auto);
				formation.SetFiringOrder(FiringOrder.FiringOrderFireAtWill);
				CommandQueueLogic.TryPendingOrder(new List<Formation> { formation }, order);
				CommandQueueLogic.CurrentFormationChanges.SetChanges(order.VirtualFormationChanges.Where<KeyValuePair<Formation, FormationChange>>((KeyValuePair<Formation, FormationChange> pair) => pair.Key == formation));
				return;
			case CustomOrderType.ManualVolley:
				CommandQueueLogic.SetFormationVolleyMode(formation, VolleyMode.Manual);
				formation.SetFiringOrder(FiringOrder.FiringOrderFireAtWill);
				CommandQueueLogic.TryPendingOrder(new List<Formation> { formation }, order);
				CommandQueueLogic.CurrentFormationChanges.SetChanges(order.VirtualFormationChanges.Where<KeyValuePair<Formation, FormationChange>>((KeyValuePair<Formation, FormationChange> pair) => pair.Key == formation));
				return;
			case CustomOrderType.DisableVolley:
				CommandQueueLogic.SetFormationVolleyMode(formation, VolleyMode.Disabled);
				formation.SetFiringOrder(FiringOrder.FiringOrderFireAtWill);
				CommandQueueLogic.TryPendingOrder(new List<Formation> { formation }, order);
				CommandQueueLogic.CurrentFormationChanges.SetChanges(order.VirtualFormationChanges.Where<KeyValuePair<Formation, FormationChange>>((KeyValuePair<Formation, FormationChange> pair) => pair.Key == formation));
				return;
			case CustomOrderType.VolleyFire:
				formation.SetFiringOrder(FiringOrder.FiringOrderFireAtWill);
				CommandQueueLogic.SetFormationVolleyMode(formation, VolleyMode.Manual);
				CommandQueueLogic.FormationVolleyFire(formation);
				CommandQueueLogic.TryPendingOrder(new List<Formation> { formation }, order);
				CommandQueueLogic.CurrentFormationChanges.SetChanges(order.VirtualFormationChanges.Where<KeyValuePair<Formation, FormationChange>>((KeyValuePair<Formation, FormationChange> pair) => pair.Key == formation));
				return;
			case CustomOrderType.StopUsing:
			{
				UsableMachine usableMachine3 = order.TargetEntity as UsableMachine;
				ModuleExtensions.StopUsingMachine(formation, usableMachine3, true);
				SiegeWeapon siegeWeapon5 = usableMachine3 as SiegeWeapon;
				if (siegeWeapon5 != null)
				{
					siegeWeapon5.SetForcedUse(false);
				}
				CommandQueueLogic.TryPendingOrder(new List<Formation> { formation }, order);
				CommandQueueLogic.CurrentFormationChanges.SetChanges(order.VirtualFormationChanges.Where<KeyValuePair<Formation, FormationChange>>((KeyValuePair<Formation, FormationChange> pair) => pair.Key == formation));
				break;
			}
			default:
				return;
			}
		}

		// Token: 0x060004B8 RID: 1208 RVA: 0x0001BFA8 File Offset: 0x0001A1A8
		private static void TryCancelStopOrder(Formation formation)
		{
			if (GameNetwork.IsClientOrReplay || formation.GetReadonlyMovementOrderReference().OrderEnum != 9)
			{
				return;
			}
			WorldPosition worldPosition = formation.CreateNewOrderWorldPosition(0);
			if (!worldPosition.IsValid)
			{
				return;
			}
			formation.SetMovementOrder(MovementOrder.MovementOrderMove(worldPosition));
		}

		// Token: 0x060004B9 RID: 1209 RVA: 0x0001BFEC File Offset: 0x0001A1EC
		private static void FacingOrderLookAtDirection(OrderInQueue order, Formation formation)
		{
			IEnumerable<ValueTuple<Formation, int, float, WorldPosition, Vec2>> actualFormationChanges = order.ActualFormationChanges;
			Patch_OrderController.SetFacingEnemyTargetFormation(formation, null);
			ValueTuple<Formation, int, float, WorldPosition, Vec2> valueTuple = actualFormationChanges.First<ValueTuple<Formation, int, float, WorldPosition, Vec2>>(([TupleElementNames(new string[] { "formation", "unitSpacingReduced", "customWidth", "position", "direction" })] ValueTuple<Formation, int, float, WorldPosition, Vec2> c) => c.Item1 == formation);
			WorldPosition item = valueTuple.Item4;
			Vec2 item2 = valueTuple.Item5;
			bool flag;
			if (order.ShouldLockFormationInFacingOrder.TryGetValue(formation, out flag) && flag)
			{
				formation.SetMovementOrder(MovementOrder.MovementOrderMove(item));
			}
			formation.SetFacingOrder(FacingOrder.FacingOrderLookAtDirection(item2));
		}

		// Token: 0x060004BA RID: 1210 RVA: 0x0001C070 File Offset: 0x0001A270
		private static OrderInQueue GetNextOrderForFormation(Formation formation)
		{
			return CommandQueueLogic.OrderQueue.FirstOrDefault<OrderInQueue>((OrderInQueue order) => order.RemainingFormations.Contains(formation));
		}

		// Token: 0x060004BB RID: 1211 RVA: 0x0001C0A0 File Offset: 0x0001A2A0
		private static void OnOrderExecutedForFormation(OrderInQueue order, Formation formation)
		{
			if (CommandQueueLogic.RelatedWithPlayerUI(formation))
			{
				RTSCamera.CommandSystem.Utilities.Utility.DisplayExecuteOrderMessageInQueue(new List<Formation> { formation }, order);
				CommandQueueLogic.TryTeleportSelectedFormationInDeployment(Mission.Current.PlayerTeam.PlayerOrderController, new List<Formation> { formation });
			}
			order.RemainingFormations.Remove(formation);
			if (order.RemainingFormations.Count == 0)
			{
				CommandQueueLogic.OrderQueue.Remove(order);
			}
			if (CommandQueueLogic.GetNextOrderForFormation(formation) == null)
			{
				CommandQueueLogic.LatestOrderInQueueChanges.SetChanges(CommandQueueLogic.CurrentFormationChanges.CollectChanges(new List<Formation> { formation }));
			}
			if (CommandQueueLogic.RelatedWithPlayerUI(formation))
			{
				CommandQueuePreview.IsPreviewOutdated = true;
				Mission mission = Mission.Current;
				if (mission != null)
				{
					CommandSystemLogic missionBehavior = mission.GetMissionBehavior<CommandSystemLogic>();
					if (missionBehavior != null)
					{
						missionBehavior.OnMovementOrderChanged(formation);
					}
				}
				RTSCamera.CommandSystem.Utilities.Utility.UpdateActiveOrders();
			}
		}

		// Token: 0x060004BC RID: 1212 RVA: 0x0001C164 File Offset: 0x0001A364
		public static bool RelatedWithPlayerUI(Formation formation)
		{
			Team team = formation.Team;
			return team != null && team.IsPlayerTeam;
		}

		// Token: 0x060004BD RID: 1213 RVA: 0x0001C178 File Offset: 0x0001A378
		public static bool CanBePended(OrderInQueue order)
		{
			if (order.CustomOrderType == CustomOrderType.Original)
			{
				OrderType orderType = order.OrderType;
				switch (orderType)
				{
				case 1:
				case 2:
				case 3:
				case 7:
				case 8:
				case 12:
				case 14:
				case 15:
					break;
				case 4:
				case 5:
					return order.TargetFormation != null;
				case 6:
				case 9:
				case 10:
				case 11:
				case 13:
					return false;
				default:
					if (orderType - 40 > 1)
					{
						return false;
					}
					break;
				}
				return true;
			}
			return false;
		}

		// Token: 0x060004BE RID: 1214 RVA: 0x0001C1F0 File Offset: 0x0001A3F0
		public static void TryPendingOrder(IEnumerable<Formation> formations, OrderInQueue order)
		{
			if (Mission.Current.IsNavalBattle)
			{
				return;
			}
			if (CommandQueueLogic.CanBePended(order))
			{
				CommandQueueLogic.CancelPendingOrder(formations);
				foreach (Formation formation in formations)
				{
					CommandQueueLogic.FormationPendingOrder(formation, order);
				}
				order.UpdateMovementSpeed();
				return;
			}
			if (CommandQueueLogic.ShouldClearPendingOrder(order.OrderType))
			{
				CommandQueueLogic.CancelPendingOrder(formations);
			}
		}

		// Token: 0x060004BF RID: 1215 RVA: 0x0001C26C File Offset: 0x0001A46C
		public static void CancelPendingOrder(IEnumerable<Formation> formations)
		{
			foreach (Formation formation in formations)
			{
				OrderInQueue orderInQueue;
				if (CommandQueueLogic.PendingOrders.TryGetValue(formation, out orderInQueue))
				{
					orderInQueue.SelectedFormations.Remove(formation);
				}
				CommandQueueLogic.PendingOrders.Remove(formation);
			}
		}

		// Token: 0x060004C0 RID: 1216 RVA: 0x0001C2D8 File Offset: 0x0001A4D8
		private static void FormationPendingOrder(Formation formation, OrderInQueue order)
		{
			CommandQueueLogic.PendingOrders[formation] = order;
			CommandQuerySystem.GetQueryForFormation(formation).OnOrderPended();
			CommandQueueLogic.TicksToSkip = 1;
		}

		// Token: 0x060004C1 RID: 1217 RVA: 0x0001C2F8 File Offset: 0x0001A4F8
		private static void ExecuteArrangementOrder(OrderInQueue order, Formation formation)
		{
			FormationChange formationChange = order.VirtualFormationChanges[formation];
			CommandQueueLogic.TryCancelStopOrder(formation);
			formation.SetArrangementOrder(RTSCamera.CommandSystem.Utilities.Utility.GetArrangementOrder(formationChange.ArrangementOrder.Value));
			Formation formation2 = formation;
			int? unitSpacing = formationChange.UnitSpacing;
			formation2.SetPositioning(null, null, unitSpacing);
			if (formationChange.Width != null)
			{
				formation.SetFormOrder(FormOrder.FormOrderCustom(formationChange.Width.Value), true);
			}
			CommandQueueLogic.CurrentFormationChanges.SetChanges(order.VirtualFormationChanges.Where<KeyValuePair<Formation, FormationChange>>((KeyValuePair<Formation, FormationChange> pair) => pair.Key == formation));
		}

		// Token: 0x060004C2 RID: 1218 RVA: 0x0001C3C0 File Offset: 0x0001A5C0
		public unsafe static void TryTeleportSelectedFormationInDeployment(OrderController orderController, IEnumerable<Formation> formations)
		{
			if (Mission.Current.Mode == 6 && orderController.FormationUpdateEnabledAfterSetOrder)
			{
				foreach (Formation formation in formations)
				{
					if (formation.CountOfUnits > 0 && (orderController == null || orderController.FormationUpdateEnabledAfterSetOrder))
					{
						bool flag = false;
						if (formation.IsPlayerTroopInFormation)
						{
							flag = formation.GetReadonlyMovementOrderReference().OrderEnum == 4;
						}
						MovementOrder movementOrder = *formation.GetReadonlyMovementOrderReference();
						Agent targetAgent = movementOrder._targetAgent;
						Formation formation2 = formation;
						Vec2? vec = new Vec2?(formation.FacingOrder.GetDirection(formation, (flag && targetAgent == Mission.Current.MainAgent) ? null : targetAgent));
						formation2.SetPositioning(null, vec, null);
						formation.ApplyActionOnEachUnit(delegate(Agent agent)
						{
							agent.ForceUpdateCachedAndFormationValues(false, false);
						}, flag ? Mission.Current.MainAgent : null);
						formation.SetHasPendingUnitPositions(false);
						Mission.Current.SetRandomDecideTimeOfAgentsWithIndices(formation.CollectUnitIndices(), null, null);
					}
				}
				OrderTroopPlacer missionBehavior = Mission.Current.GetMissionBehavior<OrderTroopPlacer>();
				if (missionBehavior == null)
				{
					return;
				}
				Action onUnitDeployed = missionBehavior.OnUnitDeployed;
				if (onUnitDeployed == null)
				{
					return;
				}
				onUnitDeployed();
			}
		}

		// Token: 0x060004C3 RID: 1219 RVA: 0x0001C53C File Offset: 0x0001A73C
		public static void OnFormationUnitsCleared(Formation formation)
		{
			if (formation.Team != null && formation.Team.IsPlayerTeam)
			{
				CommandQueueLogic.CurrentFormationChanges.SetChanges(new List<KeyValuePair<Formation, FormationChange>>
				{
					new KeyValuePair<Formation, FormationChange>(formation, default(FormationChange))
				});
				Patch_OrderController.LivePreviewFormationChanges.SetChanges(new List<KeyValuePair<Formation, FormationChange>>
				{
					new KeyValuePair<Formation, FormationChange>(formation, default(FormationChange))
				});
			}
		}

		// Token: 0x060004C4 RID: 1220 RVA: 0x0001C5A8 File Offset: 0x0001A7A8
		public static void SetFormationVolleyMode(Formation formation, VolleyMode volleyMode)
		{
			CommandQueueLogic.FormationVolleyMode[formation] = volleyMode;
			formation.ApplyActionOnEachUnit(delegate(Agent agent)
			{
				CommandSystemAgentComponent component = agent.GetComponent<CommandSystemAgentComponent>();
				if (component != null)
				{
					component.SetVolleyMode(volleyMode);
				}
			}, null);
		}

		// Token: 0x060004C5 RID: 1221 RVA: 0x0001C5E8 File Offset: 0x0001A7E8
		public static VolleyMode GetFormationVolleyMode(Formation formation)
		{
			VolleyMode volleyMode;
			if (!CommandQueueLogic.FormationVolleyMode.TryGetValue(formation, out volleyMode))
			{
				return VolleyMode.Disabled;
			}
			return volleyMode;
		}

		// Token: 0x060004C6 RID: 1222 RVA: 0x0001C607 File Offset: 0x0001A807
		public static void FormationVolleyFire(Formation formation)
		{
			formation.ApplyActionOnEachUnit(delegate(Agent agent)
			{
				CommandSystemAgentComponent component = agent.GetComponent<CommandSystemAgentComponent>();
				if (component != null)
				{
					component.ShootUnderVolley();
				}
			}, null);
		}

		// Token: 0x060004C7 RID: 1223 RVA: 0x0001C630 File Offset: 0x0001A830
		public static void AgentListVolleyFire(List<Agent> agentList)
		{
			foreach (Agent agent in agentList)
			{
				CommandSystemAgentComponent component = agent.GetComponent<CommandSystemAgentComponent>();
				if (component != null)
				{
					component.ShootUnderVolley();
				}
			}
		}

		// Token: 0x060004C8 RID: 1224 RVA: 0x0001C688 File Offset: 0x0001A888
		private static void TickVolley(Formation formation)
		{
			if (CommandQueueLogic.GetFormationVolleyMode(formation) != VolleyMode.Auto)
			{
				return;
			}
			VolleyByWeaponClassRecord globalRecord = new VolleyByWeaponClassRecord();
			bool volleyNonThrownWeaponByWeaponClass = MissionConfigBase<CommandSystemConfig>.Get().AutoVolleyByWeaponTypeForNonThrown;
			bool volleyThrownWeaponByWeaponClass = MissionConfigBase<CommandSystemConfig>.Get().AutoVolleyByWeaponTypeForThrown;
			Dictionary<WeaponClass, VolleyByWeaponClassRecord> weaponClassRecords = new Dictionary<WeaponClass, VolleyByWeaponClassRecord>();
			formation.ApplyActionOnEachAttachedUnit(delegate(Agent agent)
			{
				if (!agent.IsAIControlled)
				{
					return;
				}
				CommandSystemAgentComponent component = agent.GetComponent<CommandSystemAgentComponent>();
				if (component != null)
				{
					WeaponClass currentlyUsingWeaponClass = component.GetCurrentlyUsingWeaponClass();
					bool flag = (component.IsUsingThrownWeapon() ? volleyThrownWeaponByWeaponClass : volleyNonThrownWeaponByWeaponClass);
					if (currentlyUsingWeaponClass > 0 && flag)
					{
						if (!weaponClassRecords.ContainsKey(currentlyUsingWeaponClass))
						{
							weaponClassRecords[currentlyUsingWeaponClass] = new VolleyByWeaponClassRecord();
						}
						CommandQueueLogic.CollectAgentToRecord(weaponClassRecords[currentlyUsingWeaponClass], agent, component);
						return;
					}
					CommandQueueLogic.CollectAgentToRecord(globalRecord, agent, component);
				}
			});
			foreach (KeyValuePair<WeaponClass, VolleyByWeaponClassRecord> keyValuePair in weaponClassRecords)
			{
				CommandQueueLogic.VolleyForVolleyRecord(keyValuePair.Key, keyValuePair.Value);
			}
			CommandQueueLogic.VolleyForVolleyRecord(0, globalRecord);
		}

		// Token: 0x060004C9 RID: 1225 RVA: 0x0001C74C File Offset: 0x0001A94C
		private static void CollectAgentToRecord(VolleyByWeaponClassRecord record, Agent agent, CommandSystemAgentComponent component)
		{
			record.AgentList.Add(agent);
			if (!component.IsCandidateForNextFireAutoVolley())
			{
				return;
			}
			record.CandidateCount++;
			if (component.IsReadyForNextFire())
			{
				record.ReadyCount++;
			}
		}

		// Token: 0x060004CA RID: 1226 RVA: 0x0001C788 File Offset: 0x0001A988
		private static void VolleyForVolleyRecord(WeaponClass weaponClass, VolleyByWeaponClassRecord record)
		{
			int candidateCount = record.CandidateCount;
			int readyCount = record.ReadyCount;
			if (candidateCount == 0)
			{
				return;
			}
			if ((float)readyCount / (float)candidateCount >= MissionConfigBase<CommandSystemConfig>.Get().ReadyRatioInAutoVolley)
			{
				CommandQueueLogic.AgentListVolleyFire(record.AgentList);
			}
		}

		// Token: 0x040001EB RID: 491
		public static List<OrderInQueue> OrderQueue = new List<OrderInQueue>();

		// Token: 0x040001EC RID: 492
		public static Dictionary<Formation, OrderInQueue> PendingOrders = new Dictionary<Formation, OrderInQueue>();

		// Token: 0x040001ED RID: 493
		public static Dictionary<Formation, bool> ShouldSkipCurrentOrders = new Dictionary<Formation, bool>();

		// Token: 0x040001EE RID: 494
		public static FormationChanges CurrentFormationChanges = new FormationChanges();

		// Token: 0x040001EF RID: 495
		public static FormationChanges LatestOrderInQueueChanges = new FormationChanges();

		// Token: 0x040001F0 RID: 496
		private static int TicksToSkip = 0;

		// Token: 0x040001F1 RID: 497
		public static Dictionary<Formation, VolleyMode> FormationVolleyMode = new Dictionary<Formation, VolleyMode>();
	}
}
