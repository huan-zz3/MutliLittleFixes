using System;
using System.Collections.Generic;
using NavalDLC.Missions.AI.TeamAI;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.AI.Behaviors
{
	// Token: 0x020000F7 RID: 247
	public sealed class BehaviorNavalRemoveConnection : NavalBehaviorComponent
	{
		// Token: 0x06001281 RID: 4737 RVA: 0x00088270 File Offset: 0x00086470
		public BehaviorNavalRemoveConnection(Formation formation)
			: base(formation)
		{
			base.BehaviorCoherence = 0.8f;
			this._navalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			this._navalShipsLogic.GetShip(base.Formation.Team.TeamSide, base.Formation.FormationIndex, out this._formationMainShip);
			this.CalculateCurrentOrder();
		}

		// Token: 0x06001282 RID: 4738 RVA: 0x000882D2 File Offset: 0x000864D2
		public override void RefreshShipReferences()
		{
			this._formationMainShip = this._navalShipsLogic.GetShipAssignment(base.Formation.Team.TeamSide, base.Formation.FormationIndex).MissionShip;
		}

		// Token: 0x06001283 RID: 4739 RVA: 0x00088305 File Offset: 0x00086505
		protected override void CalculateCurrentOrder()
		{
			base.CurrentOrder = ((this._formationMainShip != null) ? NavalOrderController.GetNavalDefensiveMovementOrder(this._formationMainShip) : MovementOrder.MovementOrderStop);
		}

		// Token: 0x06001284 RID: 4740 RVA: 0x00088327 File Offset: 0x00086527
		public override void OnDeploymentFinished()
		{
			base.OnDeploymentFinished();
			this._navalShipsLogic.GetShip(base.Formation.Team.TeamSide, base.Formation.FormationIndex, out this._formationMainShip);
		}

		// Token: 0x06001285 RID: 4741 RVA: 0x0008835C File Offset: 0x0008655C
		public override void ResetBehavior()
		{
			base.ResetBehavior();
			this._navalShipsLogic.GetShip(base.Formation.Team.TeamSide, base.Formation.FormationIndex, out this._formationMainShip);
		}

		// Token: 0x06001286 RID: 4742 RVA: 0x00088394 File Offset: 0x00086594
		protected override void OnBehaviorActivatedAux()
		{
			this._readyToSeparate = false;
			this._navalShipsLogic.GetShip(base.Formation.Team.TeamSide, base.Formation.FormationIndex, out this._formationMainShip);
			if (this._formationMainShip != null)
			{
				this._formationMainShip.ShipOrder.SetBoardingTargetShip(null);
				this._formationMainShip.ShipOrder.SetCutLoose(false);
				this._formationMainShip.ShipOrder.SetOrderOarsmenLevel(2);
				this._formationMainShip.ShipOrder.SetShipStopOrder();
			}
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.SetFacingOrder(this.CurrentFacingOrder);
			base.Formation.SetArrangementOrder(ArrangementOrder.ArrangementOrderLine);
			base.Formation.SetFiringOrder(FiringOrder.FiringOrderFireAtWill);
			base.Formation.SetFormOrder(FormOrder.FormOrderWide, true);
		}

		// Token: 0x06001287 RID: 4743 RVA: 0x0008847C File Offset: 0x0008667C
		public override void TickOccasionally()
		{
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			if (!this._readyToSeparate && this._formationMainShip != null)
			{
				int num = 0;
				using (List<IFormationUnit>.Enumerator enumerator = base.Formation.UnitsWithoutLooseDetachedOnes.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Agent agent;
						if ((agent = enumerator.Current as Agent) != null)
						{
							int currentNavigationFaceId = agent.GetCurrentNavigationFaceId();
							if (currentNavigationFaceId >= 0 && !this._formationMainShip.IsAgentOnShipNavmesh(currentNavigationFaceId))
							{
								num++;
							}
						}
					}
				}
				if ((float)num <= (float)base.Formation.CountOfUnitsWithoutLooseDetachedOnes * 0.2f)
				{
					this._readyToSeparate = true;
				}
			}
			if (this._readyToSeparate)
			{
				this._formationMainShip.ShipOrder.SetCutLoose(true);
			}
		}

		// Token: 0x06001288 RID: 4744 RVA: 0x00088554 File Offset: 0x00086754
		protected override float GetAiWeight()
		{
			if (this._formationMainShip.Formation != base.Formation)
			{
				this._navalShipsLogic.GetShip(base.Formation.Team.TeamSide, base.Formation.FormationIndex, out this._formationMainShip);
			}
			if (!this._formationMainShip.GetIsConnected() || this._formationMainShip.SearchShipConnection(null, true, true, true, true))
			{
				return 0f;
			}
			return 5000f;
		}

		// Token: 0x04000A74 RID: 2676
		private NavalShipsLogic _navalShipsLogic;

		// Token: 0x04000A75 RID: 2677
		private MissionShip _formationMainShip;

		// Token: 0x04000A76 RID: 2678
		private bool _readyToSeparate;
	}
}
