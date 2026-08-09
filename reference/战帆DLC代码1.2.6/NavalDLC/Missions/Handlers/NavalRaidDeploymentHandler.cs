using System;
using System.Collections.Generic;
using System.Linq;
using NavalDLC.Missions.AI.TeamAI;
using NavalDLC.Missions.Deployment;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.Handlers
{
	// Token: 0x020000C6 RID: 198
	public class NavalRaidDeploymentHandler : DeploymentHandler
	{
		// Token: 0x06000EC3 RID: 3779 RVA: 0x0007338E File Offset: 0x0007158E
		public NavalRaidDeploymentHandler(bool isPlayerAttacker)
			: base(isPlayerAttacker)
		{
		}

		// Token: 0x06000EC4 RID: 3780 RVA: 0x00073397 File Offset: 0x00071597
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._navalShipsLogic = base.Mission.GetMissionBehavior<NavalShipsLogic>();
			base.Mission.GetDeploymentPlan<NavalRaidMissionDeploymentPlanningLogic>(ref this._navalRaidDeploymentPlan);
		}

		// Token: 0x06000EC5 RID: 3781 RVA: 0x000733C2 File Offset: 0x000715C2
		public override void OnRemoveBehavior()
		{
			base.OnRemoveBehavior();
			if (base.PlayerTeam != null)
			{
				base.PlayerTeam.OnOrderIssued -= new OnOrderIssuedDelegate(this.OrderController_OnOrderIssued);
			}
		}

		// Token: 0x06000EC6 RID: 3782 RVA: 0x000733E9 File Offset: 0x000715E9
		public override void AfterStart()
		{
			base.AfterStart();
			base.PlayerTeam.OnOrderIssued += new OnOrderIssuedDelegate(this.OrderController_OnOrderIssued);
		}

		// Token: 0x06000EC7 RID: 3783 RVA: 0x00073408 File Offset: 0x00071608
		public override void AutoDeployTeamUsingDeploymentPlan(Team team)
		{
			if (team.Side == 1)
			{
				this.AutoDeployAttackerTeam(team);
				return;
			}
			this.AutoDeployDefenderTeam(team);
		}

		// Token: 0x06000EC8 RID: 3784 RVA: 0x00073424 File Offset: 0x00071624
		private void AutoDeployAttackerTeam(Team team)
		{
			this._navalRaidDeploymentPlan.RemakeDeploymentPlan(base.Mission.PlayerTeam);
			List<Formation> list = team.FormationsIncludingEmpty.ToList<Formation>();
			if (list.Count > 0)
			{
				bool isTeleportingShips = this._navalShipsLogic.IsTeleportingShips;
				this._navalShipsLogic.SetTeleportShips(true);
				MBQueue<ValueTuple<MissionShip, Oriented2DArea>> mbqueue = new MBQueue<ValueTuple<MissionShip, Oriented2DArea>>();
				foreach (Formation formation in list)
				{
					FormationClass formationIndex = formation.FormationIndex;
					ShipAssignment shipAssignment = this._navalShipsLogic.GetShipAssignment(team.TeamSide, formationIndex);
					IFormationDeploymentPlan formationPlan = this._navalRaidDeploymentPlan.GetFormationPlan(team, formationIndex, false);
					MissionShip missionShip = shipAssignment.MissionShip;
					if (missionShip != null && formationPlan != null && formationPlan.HasFrame())
					{
						MatrixFrame frame = formationPlan.GetFrame();
						Vec2 asVec = frame.origin.AsVec2;
						Vec2 vec = frame.rotation.f.AsVec2;
						Vec2 vec2 = vec.Normalized();
						vec = missionShip.MissionShipObject.DeploymentArea;
						Oriented2DArea oriented2DArea = new Oriented2DArea(ref asVec, ref vec2, ref vec);
						mbqueue.Enqueue(new ValueTuple<MissionShip, Oriented2DArea>(missionShip, oriented2DArea));
					}
				}
				int num = 0;
				int num2 = mbqueue.Count * 5;
				while (!Extensions.IsEmpty<ValueTuple<MissionShip, Oriented2DArea>>(mbqueue))
				{
					if (num >= num2)
					{
						break;
					}
					ValueTuple<MissionShip, Oriented2DArea> valueTuple = mbqueue.Dequeue();
					MissionShip item = valueTuple.Item1;
					Oriented2DArea item2 = valueTuple.Item2;
					if (this._navalShipsLogic.IsAreaFreeOfShipCollision(in item2, 1f, item.Index))
					{
						ShipOrder shipOrder = item.ShipOrder;
						Vec2 globalCenter = item2.GlobalCenter;
						Vec2 vec = item2.GlobalForward;
						shipOrder.SetShipMovementOrder(globalCenter, in vec);
					}
					else
					{
						mbqueue.Enqueue(new ValueTuple<MissionShip, Oriented2DArea>(item, item2));
					}
					num++;
				}
				while (!Extensions.IsEmpty<ValueTuple<MissionShip, Oriented2DArea>>(mbqueue))
				{
					ValueTuple<MissionShip, Oriented2DArea> valueTuple2 = mbqueue.Dequeue();
					MissionShip item3 = valueTuple2.Item1;
					Oriented2DArea item4 = valueTuple2.Item2;
					ShipOrder shipOrder2 = item3.ShipOrder;
					Vec2 globalCenter2 = item4.GlobalCenter;
					Vec2 vec = item4.GlobalForward;
					shipOrder2.SetShipMovementOrder(globalCenter2, in vec);
				}
				NavalOrderController navalOrderController;
				if ((navalOrderController = (team.IsPlayerTeam ? team.PlayerOrderController : team.MasterOrderController) as NavalOrderController) != null)
				{
					navalOrderController.SelectAllFormations(false);
					navalOrderController.SetOrder(37);
					navalOrderController.SetFormationUpdateEnabledAfterSetOrder(false);
					navalOrderController.SetOrder(34);
					navalOrderController.SetOrder(32);
					navalOrderController.SetOrder(6);
					navalOrderController.SetFormationUpdateEnabledAfterSetOrder(true);
					navalOrderController.ClearSelectedFormations();
					Formation formation2 = team.FormationsIncludingEmpty.FirstOrDefault<Formation>((Formation x) => NavalDLCHelpers.IsPlayerCaptainOfFormationShip(x));
					if (formation2 != null)
					{
						navalOrderController.SelectFormation(formation2);
						navalOrderController.SetOrder(34);
						navalOrderController.SetFormationUpdateEnabledAfterSetOrder(true);
						navalOrderController.ClearSelectedFormations();
					}
				}
				else
				{
					Debug.FailedAssert("Team order controller is not of type naval order controller", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\Missions\\MissionLogics\\NavalRaidDeploymentHandler.cs", "AutoDeployAttackerTeam", 168);
				}
				this._navalShipsLogic.SetTeleportShips(isTeleportingShips);
			}
			NavalDeploymentMissionController navalDeploymentMissionController;
			if (team.IsPlayerTeam && (navalDeploymentMissionController = this._deploymentMissionController as NavalDeploymentMissionController) != null)
			{
				navalDeploymentMissionController.OnPlayerShipsUpdated();
			}
		}

		// Token: 0x06000EC9 RID: 3785 RVA: 0x00073718 File Offset: 0x00071918
		private void AutoDeployDefenderTeam(Team team)
		{
			List<Formation> list = team.FormationsIncludingEmpty.ToList<Formation>();
			if (list.Count > 0)
			{
				bool isTeleportingAgents = base.Mission.IsTeleportingAgents;
				base.Mission.IsTeleportingAgents = true;
				OrderController orderController = (team.IsPlayerTeam ? team.PlayerOrderController : team.MasterOrderController);
				orderController.SelectAllFormations(false);
				this.SetDefaultFormationOrders(orderController);
				orderController.ClearSelectedFormations();
				IMissionDeploymentPlan deploymentPlan = base.Mission.DeploymentPlan;
				if (deploymentPlan.IsPlanMade(team))
				{
					using (List<Formation>.Enumerator enumerator = list.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Formation formation = enumerator.Current;
							IFormationDeploymentPlan formationPlan = deploymentPlan.GetFormationPlan(team, formation.FormationIndex, false);
							WorldPosition worldPosition;
							Vec2 vec;
							base.Mission.GetFormationSpawnFrame(formation.Team, formation.FormationIndex, false, ref worldPosition, ref vec, true);
							if (formationPlan.HasDimensions)
							{
								formation.SetFormOrder(FormOrder.FormOrderCustom(formationPlan.PlannedWidth), true);
							}
							formation.SetMovementOrder(MovementOrder.MovementOrderMove(worldPosition));
							formation.SetFacingOrder(FacingOrder.FacingOrderLookAtDirection(vec));
							formation.SetPositioning(new WorldPosition?(worldPosition), new Vec2?(vec), new int?(formation.ArrangementOrder.GetUnitSpacing()));
							formation.ApplyActionOnEachUnit(delegate(Agent agent)
							{
								agent.ForceUpdateCachedAndFormationValues(true, false);
							}, null);
							formation.SetHasPendingUnitPositions(false);
							formation.SetMovementOrder(MovementOrder.MovementOrderStop);
						}
						goto IL_018C;
					}
				}
				Debug.FailedAssert("Failed to deploy team. Initial deployment plan is not made yet.", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\Missions\\MissionLogics\\NavalRaidDeploymentHandler.cs", "AutoDeployDefenderTeam", 228);
				IL_018C:
				foreach (Formation formation2 in list)
				{
					formation2.ApplyActionOnEachUnit(delegate(Agent agent)
					{
						agent.ForceUpdateCachedAndFormationValues(true, false);
					}, null);
					formation2.SetHasPendingUnitPositions(false);
				}
				base.Mission.IsTeleportingAgents = isTeleportingAgents;
			}
		}

		// Token: 0x06000ECA RID: 3786 RVA: 0x00073930 File Offset: 0x00071B30
		private void SetDefaultFormationOrders(OrderController orderController)
		{
			orderController.SetOrder(37);
			orderController.SetFormationUpdateEnabledAfterSetOrder(false);
			orderController.SetOrder(34);
			orderController.SetOrder(32);
			orderController.SetOrder(16);
			orderController.SetOrder(6);
			orderController.SetOrder((base.Mission.IsSiegeBattle || base.Mission.IsSallyOutBattle) ? 36 : 37);
			orderController.SetFormationUpdateEnabledAfterSetOrder(true);
		}

		// Token: 0x06000ECB RID: 3787 RVA: 0x00073998 File Offset: 0x00071B98
		public override void ForceUpdateAllUnits()
		{
			if (base.PlayerTeam.Side == null)
			{
				DeploymentHandler.OrderController_OnOrderIssued_Aux(1, base.PlayerTeam.FormationsIncludingSpecialAndEmpty, null, Array.Empty<object>());
			}
		}

		// Token: 0x06000ECC RID: 3788 RVA: 0x000739BE File Offset: 0x00071BBE
		private void OrderController_OnOrderIssued(OrderType orderType, MBReadOnlyList<Formation> appliedFormations, OrderController orderController, params object[] delegateParams)
		{
			DeploymentHandler.OrderController_OnOrderIssued_Aux(orderType, appliedFormations, orderController, delegateParams);
		}

		// Token: 0x04000930 RID: 2352
		private NavalRaidMissionDeploymentPlanningLogic _navalRaidDeploymentPlan;

		// Token: 0x04000931 RID: 2353
		private NavalShipsLogic _navalShipsLogic;
	}
}
