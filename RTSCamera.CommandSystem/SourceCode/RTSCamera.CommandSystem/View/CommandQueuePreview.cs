using System;
using System.Collections.Generic;
using System.Linq;
using MissionLibrary.Event;
using MissionLibrary.HotKey;
using MissionSharedLibrary.Config;
using MissionSharedLibrary.Utilities;
using RTSCamera.CommandSystem.Config;
using RTSCamera.CommandSystem.Config.HotKey;
using RTSCamera.CommandSystem.Logic;
using RTSCamera.CommandSystem.Patch;
using RTSCamera.CommandSystem.Utilities;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Order;

namespace RTSCamera.CommandSystem.View
{
	// Token: 0x02000053 RID: 83
	public class CommandQueuePreview : MissionView
	{
		// Token: 0x06000290 RID: 656 RVA: 0x00009884 File Offset: 0x00007A84
		public static void ClearArrows()
		{
			CommandQueuePreview missionBehavior = Mission.Current.GetMissionBehavior<CommandQueuePreview>();
			missionBehavior.HideArrowEntities();
			foreach (ArrowEntity arrowEntity in missionBehavior._arrowEntities)
			{
				arrowEntity.ArrowHead.Remove(0);
				arrowEntity.ArrowBody.Remove(0);
			}
			missionBehavior._arrowEntities.Clear();
		}

		// Token: 0x06000291 RID: 657 RVA: 0x00009904 File Offset: 0x00007B04
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this._orderTroopPlacer = base.Mission.GetMissionBehavior<OrderTroopPlacer>();
			this._agentPositionEntities = new List<GameEntity>();
			this._orderPositionFlagEntities = new List<GameEntity>();
			this._arrowEntities = new List<ArrowEntity>();
			this._formationShapeEntities = new List<FormationShapeEntity>();
			CommandQueuePreview.IsPreviewOutdated = true;
			this._commandQueuePreviewData = new Dictionary<Formation, CommandQueueFormationPreviewData>();
			MissionEvent.ToggleFreeCamera += this.OnToggleFreeCamera;
			FormationShapeEntity.Initialize();
		}

		// Token: 0x06000292 RID: 658 RVA: 0x0000997C File Offset: 0x00007B7C
		public override void AfterStart()
		{
			base.AfterStart();
			Team playerTeam = base.Mission.PlayerTeam;
			if (((playerTeam != null) ? playerTeam.PlayerOrderController : null) == null)
			{
				return;
			}
			base.Mission.PlayerTeam.PlayerOrderController.OnSelectedFormationsChanged += this.OnSelectedFormationsChanged;
		}

		// Token: 0x06000293 RID: 659 RVA: 0x000099CA File Offset: 0x00007BCA
		private void OnSelectedFormationsChanged()
		{
			CommandQueuePreview.IsPreviewOutdated = true;
		}

		// Token: 0x06000294 RID: 660 RVA: 0x000099D4 File Offset: 0x00007BD4
		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			this._orderTroopPlacer = null;
			this._agentPositionEntities = null;
			this._orderPositionFlagEntities = null;
			this._arrowEntities = null;
			this._commandQueuePreviewData = null;
			this._formationShapeEntities = null;
			this._commandQueuePreviewData = null;
			MissionEvent.ToggleFreeCamera -= this.OnToggleFreeCamera;
			FormationShapeEntity.Clear();
			Team playerTeam = base.Mission.PlayerTeam;
			if (((playerTeam != null) ? playerTeam.PlayerOrderController : null) == null)
			{
				return;
			}
			base.Mission.PlayerTeam.PlayerOrderController.OnSelectedFormationsChanged -= this.OnSelectedFormationsChanged;
		}

		// Token: 0x06000295 RID: 661 RVA: 0x00009A69 File Offset: 0x00007C69
		private void OnToggleFreeCamera(bool freeCamera)
		{
			this._isFreeCamera = freeCamera;
		}

		// Token: 0x06000296 RID: 662 RVA: 0x00009A74 File Offset: 0x00007C74
		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			if (base.Mission.IsNavalBattle)
			{
				return;
			}
			IGameKeySequence key = CommandSystemGameKeyCategory.GetKey(GameKeyEnum.CommandQueue);
			if (key.IsKeyPressedInOrder(null) || key.IsKeyReleasedInOrder(null))
			{
				RTSCamera.CommandSystem.Utilities.Utility.UpdateActiveOrders();
			}
			if (this._orderTroopPlacer == null)
			{
				return;
			}
			if (this._isPreviewShown)
			{
				this.UpdatePreview(dt);
			}
			if (this._orderTroopPlacer.SuspendTroopPlacer || (this._config.CommandQueueFlagShowMode == ShowMode.Never && this._config.CommandQueueArrowShowMode == ShowMode.Never && this._config.CommandQueueFormationShapeShowMode == ShowMode.Never))
			{
				if (this._isPreviewShown)
				{
					this._isPreviewShown = false;
					this.HidePreview();
					return;
				}
			}
			else if (!this._isPreviewShown)
			{
				this._isPreviewShown = true;
				CommandQueuePreview.IsPreviewOutdated = true;
			}
		}

		// Token: 0x06000297 RID: 663 RVA: 0x00009B2C File Offset: 0x00007D2C
		private void UpdatePreview(float dt)
		{
			Team playerTeam = base.Mission.PlayerTeam;
			if (((playerTeam != null) ? playerTeam.PlayerOrderController : null) == null)
			{
				return;
			}
			this.HidePreview();
			if (CommandQueuePreview.IsPreviewOutdated)
			{
				this._commandQueuePreviewData.Clear();
				foreach (Formation formation in base.Mission.PlayerTeam.FormationsIncludingEmpty)
				{
					if (formation.CountOfUnits != 0)
					{
						bool flag = base.Mission.PlayerTeam.PlayerOrderController.SelectedFormations.Contains(formation);
						CommandQueueFormationPreviewData commandQueueFormationPreviewData = this.CollectCommandQueuePreviewData(formation, flag);
						this._commandQueuePreviewData[formation] = commandQueueFormationPreviewData;
					}
				}
			}
			this.TickPreview(dt);
		}

		// Token: 0x06000298 RID: 664 RVA: 0x00009BF8 File Offset: 0x00007DF8
		private unsafe Vec3 GetInitialArrowStart(Formation formation)
		{
			MovementOrder movementOrder = *formation.GetReadonlyMovementOrderReference();
			switch (RTSCamera.CommandSystem.Utilities.Utility.MovementStateFromMovementOrderType(movementOrder.OrderType))
			{
			case 0:
			case 1:
			case 3:
				if (formation.ArrangementOrder.OrderEnum == 1)
				{
					return RTSCamera.CommandSystem.Utilities.Utility.GetColumnFormationCurrentPosition(formation);
				}
				return formation.CachedMedianPosition.GetGroundVec3();
			}
			return Vec3.Invalid;
		}

		// Token: 0x06000299 RID: 665 RVA: 0x00009C5F File Offset: 0x00007E5F
		private bool ShouldShowFormationShape()
		{
			return this._config.CommandQueueFormationShapeShowMode == ShowMode.Always || (this._isFreeCamera && this._config.CommandQueueFormationShapeShowMode == ShowMode.FreeCameraOnly);
		}

		// Token: 0x0600029A RID: 666 RVA: 0x00009C89 File Offset: 0x00007E89
		private bool ShouldShowOrderPositionFlag()
		{
			return this._config.CommandQueueFlagShowMode == ShowMode.Always || (this._isFreeCamera && this._config.CommandQueueFlagShowMode == ShowMode.FreeCameraOnly);
		}

		// Token: 0x0600029B RID: 667 RVA: 0x00009CB3 File Offset: 0x00007EB3
		private bool ShouldShowArrow()
		{
			return this._config.CommandQueueArrowShowMode == ShowMode.Always || (this._isFreeCamera && this._config.CommandQueueArrowShowMode == ShowMode.FreeCameraOnly);
		}

		// Token: 0x0600029C RID: 668 RVA: 0x00009CE0 File Offset: 0x00007EE0
		private void TickPreview(float dt)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			foreach (KeyValuePair<Formation, CommandQueueFormationPreviewData> keyValuePair in this._commandQueuePreviewData)
			{
				Formation key = keyValuePair.Key;
				CommandQueueFormationPreviewData value = keyValuePair.Value;
				Vec3 vec = this.GetInitialArrowStart(key);
				int num5 = 0;
				foreach (OrderPreviewData orderPreviewData in value.OrderList)
				{
					foreach (WorldPosition worldPosition in orderPreviewData.AgentPositions)
					{
						this.AddAgentFrameEntity(num, worldPosition.GetGroundVec3(), 0.7f);
						num++;
					}
					Vec3 groundVec = orderPreviewData.OrderPosition.GetGroundVec3();
					if (orderPreviewData.Width != null && orderPreviewData.Depth != null && this.ShouldShowFormationShape())
					{
						this.AddFormationShape(num4, groundVec, orderPreviewData.Direction, orderPreviewData.Width.Value, orderPreviewData.Depth.Value, orderPreviewData.RightSideOffset.GetValueOrDefault(), value.IsSelected);
						num4++;
					}
					if (this.ShouldShowOrderPositionFlag())
					{
						this.AddOrderPositionFlag(num2, groundVec, orderPreviewData.Direction, value.IsSelected ? (-1f) : 0.2f);
						num2++;
					}
					if (vec.IsValid && groundVec.IsValid && this.ShouldShowArrow())
					{
						Vec3 vec2 = groundVec - vec;
						float num6 = vec2.Normalize();
						if (num6 > 5f)
						{
							float num7 = MathF.Clamp(num6 * 0.1f, 1f, 10f);
							this.AddArrow(num3, vec + vec2 * num7, groundVec - vec2 * num7, value.IsSelected ? (-1f) : 0.3f, orderPreviewData.OrderTargetType);
							num3++;
						}
					}
					if (orderPreviewData.OrderTargetType == OrderTargetType.Move || orderPreviewData.OrderTargetType == OrderTargetType.Attack)
					{
						vec = groundVec;
					}
					num5++;
				}
			}
		}

		// Token: 0x0600029D RID: 669 RVA: 0x00009F74 File Offset: 0x00008174
		private CommandQueueFormationPreviewData CollectCommandQueuePreviewData(Formation formation, bool isSelected)
		{
			CommandQueueFormationPreviewData commandQueueFormationPreviewData = new CommandQueueFormationPreviewData();
			commandQueueFormationPreviewData.Formation = formation;
			commandQueueFormationPreviewData.IsSelected = isSelected;
			Patch_OrderController.ClearFormationLivePositionForPreview(formation);
			OrderPreviewData orderPreviewData = this.CollectFocusPreviewData(formation);
			if (orderPreviewData != null)
			{
				commandQueueFormationPreviewData.OrderList.Add(orderPreviewData);
			}
			OrderPreviewData orderPreviewData2 = this.CollectFacingPreviewData(formation);
			if (orderPreviewData2 != null)
			{
				commandQueueFormationPreviewData.OrderList.Add(orderPreviewData2);
			}
			OrderInQueue orderInQueue;
			if (CommandQueueLogic.PendingOrders.TryGetValue(formation, out orderInQueue))
			{
				Patch_OrderController.LivePreviewFormationChanges.SetChanges(CommandQueueLogic.CurrentFormationChanges.CollectChanges(new List<Formation> { formation }));
				OrderPreviewData orderPreviewData3 = this.CollectOrderPreviewData(orderInQueue, formation, false, true);
				if (orderPreviewData3 != null)
				{
					commandQueueFormationPreviewData.OrderList.Add(orderPreviewData3);
				}
			}
			Func<KeyValuePair<Formation, FormationChange>, bool> <>9__0;
			foreach (OrderInQueue orderInQueue2 in CommandQueueLogic.OrderQueue)
			{
				if (orderInQueue2.RemainingFormations.Contains(formation))
				{
					FormationChanges livePreviewFormationChanges = Patch_OrderController.LivePreviewFormationChanges;
					IEnumerable<KeyValuePair<Formation, FormationChange>> virtualFormationChanges = orderInQueue2.VirtualFormationChanges;
					Func<KeyValuePair<Formation, FormationChange>, bool> func;
					if ((func = <>9__0) == null)
					{
						func = (<>9__0 = (KeyValuePair<Formation, FormationChange> pair) => pair.Key == formation);
					}
					livePreviewFormationChanges.SetChanges(virtualFormationChanges.Where<KeyValuePair<Formation, FormationChange>>(func));
					OrderPreviewData orderPreviewData4 = this.CollectOrderPreviewData(orderInQueue2, formation, true, false);
					if (orderPreviewData4 != null)
					{
						commandQueueFormationPreviewData.OrderList.Add(orderPreviewData4);
					}
				}
			}
			return commandQueueFormationPreviewData;
		}

		// Token: 0x0600029E RID: 670 RVA: 0x0000A0F8 File Offset: 0x000082F8
		private bool ShouldIncludeFormationShape(OrderType orderType)
		{
			switch (orderType)
			{
			case 1:
			case 2:
			case 3:
			case 7:
			case 8:
			case 12:
			case 13:
			case 14:
			case 15:
			case 40:
			case 41:
				return true;
			case 4:
			case 5:
			case 6:
			case 9:
			case 16:
			case 17:
			case 18:
			case 19:
			case 20:
			case 21:
			case 22:
			case 23:
			case 31:
			case 32:
			case 34:
			case 35:
			case 36:
			case 37:
			case 39:
				return false;
			}
			MissionSharedLibrary.Utilities.Utility.DisplayMessage("Error: unexpected order type");
			return false;
		}

		// Token: 0x0600029F RID: 671 RVA: 0x0000A1C4 File Offset: 0x000083C4
		private unsafe static bool UpdateMovingOrderTarget(Formation formation, OrderType? movementOrder, WorldPosition? orderPosition, Formation targetFormation, Agent targetAgent, IOrderable targetEntity, bool isPendingOrder = false)
		{
			if (movementOrder != null)
			{
				switch (movementOrder.GetValueOrDefault())
				{
				case 1:
				case 2:
				case 3:
					return true;
				case 4:
				case 5:
				{
					if (isPendingOrder)
					{
						targetFormation = formation.TargetFormation;
					}
					if (targetFormation == null)
					{
						return false;
					}
					WorldPosition cachedMedianPosition = targetFormation.CachedMedianPosition;
					Patch_OrderController.LivePreviewFormationChanges.UpdateFormationChange(formation, new WorldPosition?(cachedMedianPosition), null, null, null);
					return true;
				}
				case 6:
				case 9:
				case 16:
				case 17:
				case 18:
				case 19:
				case 20:
				case 21:
				case 22:
				case 23:
				case 31:
				case 32:
				case 34:
				case 35:
				case 36:
				case 37:
					return false;
				case 7:
				{
					if (isPendingOrder)
					{
						MovementOrder movementOrder2 = *formation.GetReadonlyMovementOrderReference();
						targetAgent = movementOrder2._targetAgent;
					}
					if (targetAgent == null)
					{
						return false;
					}
					WorldPosition followOrderPosition = Patch_OrderController.GetFollowOrderPosition(formation, targetAgent);
					Vec2 formationVirtualDirectionWhenFollowingAgent = Patch_OrderController.GetFormationVirtualDirectionWhenFollowingAgent(formation, targetAgent);
					Patch_OrderController.LivePreviewFormationChanges.UpdateFormationChange(formation, new WorldPosition?(followOrderPosition), new Vec2?(formationVirtualDirectionWhenFollowingAgent), null, null);
					return true;
				}
				case 8:
				{
					if (targetEntity == null)
					{
						return false;
					}
					UsableMachine usableMachine = targetEntity as UsableMachine;
					if (usableMachine == null)
					{
						return false;
					}
					if (usableMachine.IsDestroyed)
					{
						return false;
					}
					GameEntity gameEntity = usableMachine.WaitEntity;
					if (isPendingOrder)
					{
						gameEntity = formation.GetReadonlyMovementOrderReference().TargetEntity;
					}
					if (gameEntity == null)
					{
						return false;
					}
					Vec2 followEntityDirection = Patch_OrderController.GetFollowEntityDirection(formation, gameEntity);
					WorldPosition followEntityOrderPosition = Patch_OrderController.GetFollowEntityOrderPosition(formation, gameEntity);
					Patch_OrderController.LivePreviewFormationChanges.UpdateFormationChange(formation, new WorldPosition?(followEntityOrderPosition), new Vec2?(followEntityDirection), null, null);
					return true;
				}
				case 12:
				{
					if (isPendingOrder)
					{
						targetFormation = formation.TargetFormation;
					}
					WorldPosition advanceOrderPosition = Patch_OrderController.GetAdvanceOrderPosition(formation, 0, targetFormation);
					Patch_OrderController.LivePreviewFormationChanges.UpdateFormationChange(formation, new WorldPosition?(advanceOrderPosition), null, null, null);
					return true;
				}
				case 13:
				{
					if (isPendingOrder)
					{
						targetFormation = formation.TargetFormation;
					}
					WorldPosition fallbackOrderPosition = Patch_OrderController.GetFallbackOrderPosition(formation, 0, targetFormation);
					Patch_OrderController.LivePreviewFormationChanges.UpdateFormationChange(formation, new WorldPosition?(fallbackOrderPosition), null, null, null);
					return true;
				}
				case 39:
				{
					if (targetEntity == null)
					{
						return false;
					}
					UsableMachine usableMachine2 = targetEntity as UsableMachine;
					if (usableMachine2 == null)
					{
						return false;
					}
					if (usableMachine2.IsDestroyed)
					{
						return false;
					}
					WorldPosition worldPosition;
					worldPosition..ctor(Mission.Current.Scene, UIntPtr.Zero, usableMachine2.GameEntity.GlobalPosition, false);
					worldPosition.SetVec2(worldPosition.AsVec2);
					Patch_OrderController.LivePreviewFormationChanges.UpdateFormationChange(formation, new WorldPosition?(worldPosition), null, null, null);
					return true;
				}
				case 40:
				{
					if (targetEntity == null)
					{
						return false;
					}
					MissionObject missionObject = targetEntity as MissionObject;
					if (missionObject == null)
					{
						return false;
					}
					GameEntity gameEntity2 = GameEntity.CreateFromWeakEntity(missionObject.GameEntity);
					if (isPendingOrder)
					{
						gameEntity2 = formation.GetReadonlyMovementOrderReference().TargetEntity;
					}
					if (gameEntity2 == null)
					{
						return false;
					}
					WorldPosition attackEntityWaitPosition = Patch_OrderController.GetAttackEntityWaitPosition(formation, gameEntity2);
					Patch_OrderController.LivePreviewFormationChanges.UpdateFormationChange(formation, new WorldPosition?(attackEntityWaitPosition), null, null, null);
					return true;
				}
				case 41:
				{
					if (targetEntity == null)
					{
						return false;
					}
					IPointDefendable pointDefendable = targetEntity as IPointDefendable;
					if (pointDefendable == null)
					{
						return false;
					}
					WorldPosition origin = pointDefendable.MiddleFrame.Origin;
					Patch_OrderController.LivePreviewFormationChanges.UpdateFormationChange(formation, new WorldPosition?(origin), null, null, null);
					return true;
				}
				}
				MissionSharedLibrary.Utilities.Utility.DisplayMessage("Error: unexpected order type");
				return false;
			}
			return false;
		}

		// Token: 0x060002A0 RID: 672 RVA: 0x0000A5C8 File Offset: 0x000087C8
		private unsafe OrderPreviewData CollectOrderPreviewData(OrderInQueue order, Formation formation, bool virtualFacingDirection = true, bool isPendingOrder = false)
		{
			OrderType formationVirtualFacingOrder = Patch_OrderController.GetFormationVirtualFacingOrder(formation);
			switch (order.CustomOrderType)
			{
			case CustomOrderType.Original:
			{
				OrderType orderType = order.OrderType;
				if (orderType == 14)
				{
					FormationChange formationChange;
					if (order.VirtualFormationChanges.TryGetValue(formation, out formationChange))
					{
						Vec2 vec;
						if (!virtualFacingDirection)
						{
							FacingOrder facingOrder = formation.FacingOrder;
							MovementOrder movementOrder = *formation.GetReadonlyMovementOrderReference();
							vec = facingOrder.GetDirection(formation, movementOrder._targetAgent);
						}
						else
						{
							vec = Patch_OrderController.GetVirtualDirectionOfFacingEnemyAccordingToPostitionAndDirection(formation, Patch_OrderController.GetFormationVirtualAveragePositionVec2(formation), Patch_OrderController.GetFormationVirtualDirection(formation));
						}
						Vec2 vec2 = vec;
						if (order.TargetFormation != null)
						{
							WorldPosition cachedMedianPosition = order.TargetFormation.CachedMedianPosition;
							Patch_OrderController.LivePreviewFormationChanges.UpdateFormationChange(formation, new WorldPosition?(cachedMedianPosition), null, null, null);
						}
						else
						{
							CommandQueuePreview.UpdateMovingOrderTarget(formation, formationChange.MovementOrderType, formationChange.WorldPosition, formationChange.TargetFormation, formationChange.TargetAgent, formationChange.TargetEntity, false);
						}
						Patch_OrderController.LivePreviewFormationChanges.UpdateFormationChange(formation, null, new Vec2?(vec2), null, null);
					}
					return this.CollectOrderPreviewData(formation, order.TargetFormation == null && this.ShouldIncludeFormationShape(order.OrderType), (order.TargetFormation != null) ? OrderTargetType.Facing : OrderTargetType.Move);
				}
				if (orderType != 15)
				{
					this.UpdateFacingOrderForOtherOrder(formationVirtualFacingOrder, formation, virtualFacingDirection);
					goto IL_025F;
				}
				FormationChange formationChange2;
				if (order.VirtualFormationChanges.TryGetValue(formation, out formationChange2))
				{
					CommandQueuePreview.UpdateMovingOrderTarget(formation, formationChange2.MovementOrderType, formationChange2.WorldPosition, formationChange2.TargetFormation, formationChange2.TargetAgent, formationChange2.TargetEntity, false);
					Patch_OrderController.LivePreviewFormationChanges.UpdateFormationChange(formation, null, formationChange2.Direciton, null, null);
				}
				return this.CollectOrderPreviewData(formation, this.ShouldIncludeFormationShape(order.OrderType), OrderTargetType.Move);
			}
			case CustomOrderType.SetTargetFormation:
			{
				if (order.TargetFormation == null)
				{
					return null;
				}
				WorldPosition cachedMedianPosition2 = order.TargetFormation.CachedMedianPosition;
				Patch_OrderController.LivePreviewFormationChanges.UpdateFormationChange(formation, new WorldPosition?(cachedMedianPosition2), null, null, null);
				this.UpdateFacingOrderForOtherOrder(formationVirtualFacingOrder, formation, virtualFacingDirection);
				return this.CollectOrderPreviewData(formation, false, OrderTargetType.Focus);
			}
			case CustomOrderType.AutoVolley:
			case CustomOrderType.ManualVolley:
			case CustomOrderType.DisableVolley:
			case CustomOrderType.VolleyFire:
				return null;
			}
			this.UpdateFacingOrderForOtherOrder(formationVirtualFacingOrder, formation, virtualFacingDirection);
			IL_025F:
			CommandQueuePreview.UpdateMovingOrderTarget(formation, new OrderType?(order.OrderType), new WorldPosition?(order.PositionBegin), order.TargetFormation, order.TargetAgent, order.TargetEntity, isPendingOrder);
			OrderTargetType orderTargetType = this.GetOrderTargetType(order);
			if (orderTargetType == OrderTargetType.Attack || orderTargetType == OrderTargetType.Move)
			{
				Patch_OrderController.SaveFormationLivePositionForPreview(formation, new WorldPosition?(Patch_OrderController.GetFormationVirtualMedianPosition(formation)));
			}
			return this.CollectOrderPreviewData(formation, this.ShouldIncludeFormationShape(order.OrderType), orderTargetType);
		}

		// Token: 0x060002A1 RID: 673 RVA: 0x0000A89C File Offset: 0x00008A9C
		private OrderTargetType GetOrderTargetType(OrderInQueue order)
		{
			OrderType orderType = order.OrderType;
			if (orderType != 8)
			{
				if (orderType != 39)
				{
					return this.GetOrderTargetType(order.OrderType);
				}
				if (!order.IsStopUsing)
				{
					return OrderTargetType.Use;
				}
				return OrderTargetType.StopUsing;
			}
			else
			{
				if (!order.IsStopUsing)
				{
					return OrderTargetType.Move;
				}
				return OrderTargetType.StopUsing;
			}
		}

		// Token: 0x060002A2 RID: 674 RVA: 0x0000A8E0 File Offset: 0x00008AE0
		private OrderTargetType GetOrderTargetType(OrderType orderType)
		{
			switch (orderType)
			{
			case 1:
			case 2:
			case 3:
			case 7:
				return OrderTargetType.Move;
			case 4:
			case 5:
			case 40:
				return OrderTargetType.Attack;
			case 6:
			case 9:
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
			case 31:
			case 32:
			case 34:
			case 35:
			case 36:
			case 37:
			case 41:
				return OrderTargetType.Move;
			case 8:
				return OrderTargetType.Move;
			case 39:
				return OrderTargetType.Use;
			}
			MissionSharedLibrary.Utilities.Utility.DisplayMessage("Error: unexpected order type");
			return OrderTargetType.Move;
		}

		// Token: 0x060002A3 RID: 675 RVA: 0x0000A9B0 File Offset: 0x00008BB0
		private unsafe void UpdateFacingOrderForOtherOrder(OrderType facingOrder, Formation formation, bool virtualFacingDirection)
		{
			if (facingOrder != 14)
			{
				return;
			}
			Vec2 vec = Patch_OrderController.GetFormationVirtualDirection(formation);
			if (RTSCamera.CommandSystem.Utilities.Utility.ShouldQueueCommand())
			{
				vec = Patch_OrderController.GetFormationVirtualDirectionIncludingFacingEnemyAccordingToPositionAndDirection(formation, Patch_OrderController.GetFormationVirtualPositionVec2(formation), Patch_OrderController.GetFormationVirtualDirection(formation));
			}
			Vec2 vec2;
			if (!virtualFacingDirection)
			{
				FacingOrder facingOrder2 = formation.FacingOrder;
				MovementOrder movementOrder = *formation.GetReadonlyMovementOrderReference();
				vec2 = facingOrder2.GetDirection(formation, movementOrder._targetAgent);
			}
			else
			{
				vec2 = Patch_OrderController.GetVirtualDirectionOfFacingEnemyAccordingToPostitionAndDirection(formation, Patch_OrderController.GetFormationVirtualAveragePositionVec2(formation), vec);
			}
			Vec2 vec3 = vec2;
			Patch_OrderController.LivePreviewFormationChanges.UpdateFormationChange(formation, null, new Vec2?(vec3), null, null);
		}

		// Token: 0x060002A4 RID: 676 RVA: 0x0000AA50 File Offset: 0x00008C50
		private void AddAgentFrameEntity(int index, Vec3 groundPosition, float alpha)
		{
			while (this._agentPositionEntities.Count <= index)
			{
				GameEntity gameEntity = GameEntity.CreateEmpty(base.Mission.Scene, true, true, true);
				gameEntity.EntityFlags |= 4194304;
				MetaMesh copy = MetaMesh.GetCopy("barrier_sphere", true, false);
				if (CommandQueuePreview._agentPositionMeshMaterial == null)
				{
					CommandQueuePreview._agentPositionMeshMaterial = Material.GetFromResource("vertex_color_blend_no_depth_mat").CreateCopy();
				}
				copy.SetMaterial(CommandQueuePreview._agentPositionMeshMaterial);
				copy.SetFactor1(Patch_OrderTroopPlacer.OrderPositionEntityColor);
				gameEntity.AddComponent(copy);
				gameEntity.SetVisibilityExcludeParents(false);
				this._agentPositionEntities.Add(gameEntity);
			}
			GameEntity gameEntity2 = this._agentPositionEntities[index];
			Mat3 identity = Mat3.Identity;
			Vec3 vec = groundPosition + Vec3.Up * 1f;
			MatrixFrame matrixFrame = new MatrixFrame(ref identity, ref vec);
			gameEntity2.SetFrame(ref matrixFrame, true);
			if ((double)alpha != -1.0)
			{
				gameEntity2.SetVisibilityExcludeParents(true);
				gameEntity2.SetAlpha(alpha);
				return;
			}
			GameEntityExtensions.FadeIn(gameEntity2, true);
		}

		// Token: 0x060002A5 RID: 677 RVA: 0x0000AB58 File Offset: 0x00008D58
		private void AddFormationShape(int index, Vec3 orderPosition, Vec2 direciton, float width, float depth, float rightSideOffset, bool isSelected)
		{
			while (this._formationShapeEntities.Count <= index)
			{
				FormationShapeEntity formationShapeEntity = new FormationShapeEntity();
				formationShapeEntity.CreateEntities();
				this._formationShapeEntities.Add(formationShapeEntity);
			}
			this._formationShapeEntities[index].Update(orderPosition, direciton, width, depth, rightSideOffset, isSelected);
		}

		// Token: 0x060002A6 RID: 678 RVA: 0x0000ABA8 File Offset: 0x00008DA8
		private void AddOrderPositionFlag(int index, Vec3 groundPosition, Vec2 direction, float alpha)
		{
			while (this._orderPositionFlagEntities.Count <= index)
			{
				GameEntity gameEntity = GameEntity.CreateEmpty(base.Mission.Scene, true, true, true);
				gameEntity.EntityFlags |= 4194304;
				MetaMesh copy = MetaMesh.GetCopy("order_flag_a", true, false);
				gameEntity.AddComponent(copy);
				gameEntity.SetVisibilityExcludeParents(false);
				this._orderPositionFlagEntities.Add(gameEntity);
			}
			GameEntity gameEntity2 = this._orderPositionFlagEntities[index];
			Vec3 vec = direction.ToVec3(0f);
			Mat3 mat = Mat3.CreateMat3WithForward(ref vec);
			MatrixFrame matrixFrame = new MatrixFrame(ref mat, ref groundPosition);
			vec = new Vec3(30f, 30f, 30f, -1f);
			matrixFrame.Scale(ref vec);
			gameEntity2.SetFrame(ref matrixFrame, true);
			if ((double)alpha != -1.0)
			{
				gameEntity2.SetVisibilityExcludeParents(true);
				gameEntity2.SetAlpha(alpha);
				return;
			}
			GameEntityExtensions.FadeIn(gameEntity2, true);
		}

		// Token: 0x060002A7 RID: 679 RVA: 0x0000AC94 File Offset: 0x00008E94
		private void AddArrow(int index, Vec3 arrowStart, Vec3 arrowEnd, float alpha, OrderTargetType orderTargetType)
		{
			while (this._arrowEntities.Count <= index)
			{
				ArrowEntity arrowEntity = new ArrowEntity
				{
					ArrowHead = GameEntity.CreateEmpty(base.Mission.Scene, true, true, true),
					ArrowBody = GameEntity.CreateEmpty(base.Mission.Scene, true, true, true)
				};
				MetaMesh copy = MetaMesh.GetCopy("rts_arrow_head", true, false);
				MetaMesh copy2 = MetaMesh.GetCopy("rts_arrow_body", true, false);
				copy2.GetMeshAtIndex(0).GetMaterial().CreateCopy();
				arrowEntity.ArrowHead.AddComponent(copy);
				arrowEntity.ArrowBody.AddComponent(copy2);
				arrowEntity.ArrowHead.EntityFlags |= 4194304;
				arrowEntity.ArrowHead.EntityVisibilityFlags = 4;
				arrowEntity.ArrowBody.EntityFlags |= 4194304;
				arrowEntity.ArrowBody.EntityVisibilityFlags = 4;
				arrowEntity.ArrowHead.SetVisibilityExcludeParents(false);
				arrowEntity.ArrowBody.SetVisibilityExcludeParents(false);
				this._arrowEntities.Add(arrowEntity);
			}
			ArrowEntity arrowEntity2 = this._arrowEntities[index];
			arrowEntity2.UpdateColor(orderTargetType);
			Vec3 vec = arrowEnd - arrowStart;
			float num = vec.Normalize();
			float num2 = 10f;
			float num3 = 2f;
			if (orderTargetType == OrderTargetType.Facing)
			{
				num2 = 7.5f;
				num3 = 2.01f;
				float num4 = MathF.Min(num * 0.2f, 0.2f);
				arrowStart += num4 * vec;
				num -= num4;
			}
			float num5 = 2.7f;
			Vec3 vec2 = -vec;
			Mat3 mat = Mat3.CreateMat3WithForward(ref vec2);
			Vec3 vec3 = arrowStart + (num - num5) * vec + Vec3.Up * num3;
			MatrixFrame matrixFrame = new MatrixFrame(ref mat, ref vec3);
			vec2 = -vec;
			mat = Mat3.CreateMat3WithForward(ref vec2);
			vec3 = arrowStart + Vec3.Up * num3;
			MatrixFrame matrixFrame2 = new MatrixFrame(ref mat, ref vec3);
			vec2 = new Vec3(num2, num2, 1f, -1f);
			matrixFrame.Scale(ref vec2);
			vec2 = new Vec3(num2, (num - num5) * 1.335942f, 1f, -1f);
			matrixFrame2.Scale(ref vec2);
			arrowEntity2.ArrowHead.SetFrame(ref matrixFrame, true);
			arrowEntity2.ArrowBody.SetFrame(ref matrixFrame2, true);
			arrowEntity2._isShown = true;
			arrowEntity2._alpha = alpha;
			if ((double)alpha != -1.0)
			{
				arrowEntity2.ArrowHead.SetVisibilityExcludeParents(true);
				arrowEntity2.ArrowBody.SetVisibilityExcludeParents(true);
				arrowEntity2.ArrowHead.SetAlpha(alpha);
				arrowEntity2.ArrowBody.SetAlpha(alpha);
				return;
			}
			GameEntityExtensions.FadeIn(arrowEntity2.ArrowHead, true);
			GameEntityExtensions.FadeIn(arrowEntity2.ArrowBody, true);
		}

		// Token: 0x060002A8 RID: 680 RVA: 0x0000AF53 File Offset: 0x00009153
		private void HidePreview()
		{
			this.HideAgentFrameEntities();
			this.HideOrderPositionFlagEntities();
			this.HideArrowEntities();
			this.HideFormationShapes();
		}

		// Token: 0x060002A9 RID: 681 RVA: 0x0000AF70 File Offset: 0x00009170
		private void HideAgentFrameEntities()
		{
			foreach (GameEntity gameEntity in this._agentPositionEntities)
			{
				GameEntityExtensions.HideIfNotFadingOut(gameEntity);
			}
		}

		// Token: 0x060002AA RID: 682 RVA: 0x0000AFC0 File Offset: 0x000091C0
		private void HideOrderPositionFlagEntities()
		{
			foreach (GameEntity gameEntity in this._orderPositionFlagEntities)
			{
				GameEntityExtensions.HideIfNotFadingOut(gameEntity);
			}
		}

		// Token: 0x060002AB RID: 683 RVA: 0x0000B010 File Offset: 0x00009210
		private void HideArrowEntities()
		{
			foreach (ArrowEntity arrowEntity in this._arrowEntities)
			{
				arrowEntity.Hide(this._isPreviewShown);
			}
		}

		// Token: 0x060002AC RID: 684 RVA: 0x0000B068 File Offset: 0x00009268
		private void HideFormationShapes()
		{
			foreach (FormationShapeEntity formationShapeEntity in this._formationShapeEntities)
			{
				formationShapeEntity.Hide(this._isPreviewShown);
			}
		}

		// Token: 0x060002AD RID: 685 RVA: 0x0000B0C0 File Offset: 0x000092C0
		private unsafe OrderPreviewData CollectFocusPreviewData(Formation formation)
		{
			if (formation.TargetFormation == null)
			{
				return null;
			}
			MovementOrder movementOrder = *formation.GetReadonlyMovementOrderReference();
			if (this.GetOrderTargetType(movementOrder.OrderType) == OrderTargetType.Attack)
			{
				return null;
			}
			WorldPosition cachedMedianPosition = formation.TargetFormation.CachedMedianPosition;
			Patch_OrderController.LivePreviewFormationChanges.UpdateFormationChange(formation, new WorldPosition?(cachedMedianPosition), null, null, null);
			return this.CollectOrderPreviewData(formation, false, OrderTargetType.Focus);
		}

		// Token: 0x060002AE RID: 686 RVA: 0x0000B138 File Offset: 0x00009338
		private OrderPreviewData CollectFacingPreviewData(Formation formation)
		{
			if (formation.FacingOrder.OrderType == 14)
			{
				Formation facingEnemyTargetFormation = Patch_OrderController.GetFacingEnemyTargetFormation(formation);
				if (facingEnemyTargetFormation != null)
				{
					WorldPosition cachedMedianPosition = facingEnemyTargetFormation.CachedMedianPosition;
					Patch_OrderController.LivePreviewFormationChanges.UpdateFormationChange(formation, new WorldPosition?(cachedMedianPosition), null, null, null);
					return this.CollectOrderPreviewData(formation, false, OrderTargetType.Facing);
				}
			}
			return null;
		}

		// Token: 0x060002AF RID: 687 RVA: 0x0000B1A4 File Offset: 0x000093A4
		private OrderPreviewData CollectOrderPreviewData(Formation formation, bool includeFormationShape, OrderTargetType orderTargetType = OrderTargetType.Move)
		{
			float? num = null;
			float? num2 = null;
			float? num3 = null;
			if (includeFormationShape)
			{
				float num4;
				float num5;
				float num6;
				Patch_OrderController.GetFormationVirtualShape(formation, out num4, out num5, out num6);
				num = new float?(num4);
				num2 = new float?(num5);
				num3 = new float?(num6);
			}
			if (this._showAgentFrames)
			{
				List<WorldPosition> list;
				Patch_OrderController.SimulateAgentFrames(new List<Formation> { formation }, base.Mission.PlayerTeam.PlayerOrderController.simulationFormations, out list);
				return new OrderPreviewData
				{
					AgentPositions = list,
					OrderPosition = Patch_OrderController.GetFormationVirtualPosition(formation),
					Direction = Patch_OrderController.GetFormationVirtualDirection(formation),
					Width = num,
					Depth = num2,
					RightSideOffset = num3,
					OrderTargetType = orderTargetType
				};
			}
			return new OrderPreviewData
			{
				OrderPosition = Patch_OrderController.GetFormationVirtualPosition(formation),
				Direction = Patch_OrderController.GetFormationVirtualDirection(formation),
				Width = num,
				Depth = num2,
				RightSideOffset = num3,
				OrderTargetType = orderTargetType
			};
		}

		// Token: 0x04000120 RID: 288
		private CommandSystemConfig _config = MissionConfigBase<CommandSystemConfig>.Get();

		// Token: 0x04000121 RID: 289
		private OrderTroopPlacer _orderTroopPlacer;

		// Token: 0x04000122 RID: 290
		private List<GameEntity> _agentPositionEntities;

		// Token: 0x04000123 RID: 291
		private static Material _agentPositionMeshMaterial;

		// Token: 0x04000124 RID: 292
		private List<GameEntity> _orderPositionFlagEntities;

		// Token: 0x04000125 RID: 293
		private List<ArrowEntity> _arrowEntities;

		// Token: 0x04000126 RID: 294
		private List<FormationShapeEntity> _formationShapeEntities;

		// Token: 0x04000127 RID: 295
		private bool _isPreviewShown;

		// Token: 0x04000128 RID: 296
		private bool _isFreeCamera;

		// Token: 0x04000129 RID: 297
		public static bool IsPreviewOutdated;

		// Token: 0x0400012A RID: 298
		private bool _showAgentFrames;

		// Token: 0x0400012B RID: 299
		private Dictionary<Formation, CommandQueueFormationPreviewData> _commandQueuePreviewData;
	}
}
