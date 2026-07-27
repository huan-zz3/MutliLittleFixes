using System;
using System.Collections.Generic;
using System.Linq;
using NavalDLC.Missions.AI.TeamAI;
using NavalDLC.Missions.Deployment;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.Handlers
{
	// Token: 0x020000C5 RID: 197
	public class NavalDeploymentHandler : DeploymentHandler
	{
		// Token: 0x06000EBE RID: 3774 RVA: 0x0007305C File Offset: 0x0007125C
		public NavalDeploymentHandler(bool isPlayerAttacker)
			: base(isPlayerAttacker)
		{
		}

		// Token: 0x06000EBF RID: 3775 RVA: 0x00073065 File Offset: 0x00071265
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._navalShipsLogic = base.Mission.GetMissionBehavior<NavalShipsLogic>();
			base.Mission.GetDeploymentPlan<NavalMissionDeploymentPlanningLogic>(ref this._navalDeploymentPlan);
		}

		// Token: 0x06000EC0 RID: 3776 RVA: 0x00073090 File Offset: 0x00071290
		public override void AfterStart()
		{
			base.AfterStart();
		}

		// Token: 0x06000EC1 RID: 3777 RVA: 0x00073098 File Offset: 0x00071298
		public override void AutoDeployTeamUsingDeploymentPlan(Team team)
		{
			this._navalDeploymentPlan.RemakeDeploymentPlan(base.Mission.PlayerTeam);
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
					IFormationDeploymentPlan formationPlan = this._navalDeploymentPlan.GetFormationPlan(team, formationIndex, false);
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
					Debug.FailedAssert("Team order controller is not of type naval order controller", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\Missions\\MissionLogics\\NavalDeploymentHandler.cs", "AutoDeployTeamUsingDeploymentPlan", 148);
				}
				this._navalShipsLogic.SetTeleportShips(isTeleportingShips);
			}
			NavalDeploymentMissionController navalDeploymentMissionController;
			if (team.IsPlayerTeam && (navalDeploymentMissionController = this._deploymentMissionController as NavalDeploymentMissionController) != null)
			{
				navalDeploymentMissionController.OnPlayerShipsUpdated();
			}
		}

		// Token: 0x06000EC2 RID: 3778 RVA: 0x0007338C File Offset: 0x0007158C
		public override void ForceUpdateAllUnits()
		{
		}

		// Token: 0x0400092E RID: 2350
		private NavalMissionDeploymentPlanningLogic _navalDeploymentPlan;

		// Token: 0x0400092F RID: 2351
		private NavalShipsLogic _navalShipsLogic;
	}
}
