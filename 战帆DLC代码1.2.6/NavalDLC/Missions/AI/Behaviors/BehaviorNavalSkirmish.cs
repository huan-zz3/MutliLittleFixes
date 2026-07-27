using System;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.AI.Behaviors
{
	// Token: 0x020000F8 RID: 248
	public sealed class BehaviorNavalSkirmish : NavalBehaviorComponent
	{
		// Token: 0x06001289 RID: 4745 RVA: 0x000885CC File Offset: 0x000867CC
		public BehaviorNavalSkirmish(Formation formation)
			: base(formation)
		{
			base.BehaviorCoherence = 0.8f;
			this._navalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			this._formationMainShip = this._navalShipsLogic.GetShipAssignment(base.Formation.Team.TeamSide, base.Formation.FormationIndex).MissionShip;
		}

		// Token: 0x0600128A RID: 4746 RVA: 0x0008862C File Offset: 0x0008682C
		private void CalculateAndSetShipOrders()
		{
			if (base.Formation.CachedClosestEnemyFormation != null && this._formationMainShip.IsFormationAndShipAIControlled)
			{
				MissionShip missionShip = this._navalShipsLogic.GetShipAssignment(base.Formation.CachedClosestEnemyFormation.Team.Team.TeamSide, base.Formation.CachedClosestEnemyFormation.Formation.FormationIndex).MissionShip;
				this._formationMainShip.ShipOrder.SetShipSkirmishOrder(missionShip);
			}
		}

		// Token: 0x0600128B RID: 4747 RVA: 0x000886A4 File Offset: 0x000868A4
		public override void RefreshShipReferences()
		{
			this._formationMainShip = this._navalShipsLogic.GetShipAssignment(base.Formation.Team.TeamSide, base.Formation.FormationIndex).MissionShip;
		}

		// Token: 0x0600128C RID: 4748 RVA: 0x000886D8 File Offset: 0x000868D8
		public override void OnDeploymentFinished()
		{
			base.OnDeploymentFinished();
			this._navalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			this._formationMainShip = this._navalShipsLogic.GetShipAssignment(base.Formation.Team.TeamSide, base.Formation.FormationIndex).MissionShip;
		}

		// Token: 0x0600128D RID: 4749 RVA: 0x0008872C File Offset: 0x0008692C
		public override void ResetBehavior()
		{
			base.ResetBehavior();
			this._formationMainShip = this._navalShipsLogic.GetShipAssignment(base.Formation.Team.TeamSide, base.Formation.FormationIndex).MissionShip;
		}

		// Token: 0x0600128E RID: 4750 RVA: 0x00088768 File Offset: 0x00086968
		protected override void OnBehaviorActivatedAux()
		{
			this._navalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			this._formationMainShip = this._navalShipsLogic.GetShipAssignment(base.Formation.Team.TeamSide, base.Formation.FormationIndex).MissionShip;
			this._formationMainShip.ShipOrder.SetBoardingTargetShip(null);
			this._formationMainShip.ShipOrder.SetCutLoose(false);
			this._formationMainShip.ShipOrder.SetOrderOarsmenLevel(2);
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.SetFacingOrder(this.CurrentFacingOrder);
			base.Formation.SetArrangementOrder(ArrangementOrder.ArrangementOrderLine);
			base.Formation.SetFiringOrder(FiringOrder.FiringOrderFireAtWill);
			base.Formation.SetFormOrder(FormOrder.FormOrderWide, true);
		}

		// Token: 0x0600128F RID: 4751 RVA: 0x0008883C File Offset: 0x00086A3C
		public override void TickOccasionally()
		{
			if (this._navalShipsLogic == null)
			{
				this._navalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
				if (this._navalShipsLogic == null)
				{
					return;
				}
			}
			this.CalculateAndSetShipOrders();
		}

		// Token: 0x06001290 RID: 4752 RVA: 0x00088868 File Offset: 0x00086A68
		protected override float GetAiWeight()
		{
			if (this._formationMainShip.Formation != base.Formation)
			{
				this._navalShipsLogic.GetShip(base.Formation.Team.TeamSide, base.Formation.FormationIndex, out this._formationMainShip);
			}
			float num = 0f;
			if (base.Formation.CachedClosestEnemyFormation != null)
			{
				if (base.Formation.CachedClosestEnemyFormation.FormationMeleeFightingPower > 0f)
				{
					num = base.Formation.QuerySystem.FormationMeleeFightingPower / base.Formation.CachedClosestEnemyFormation.FormationMeleeFightingPower;
				}
				else
				{
					num = 5f;
				}
			}
			return ((this._formationMainShip == null || this._formationMainShip.GetIsConnected()) ? 0f : 1.5f) * MathF.Clamp(num, 0f, 5f) * base.Formation.QuerySystem.RangedUnitRatio;
		}

		// Token: 0x04000A77 RID: 2679
		private NavalShipsLogic _navalShipsLogic;

		// Token: 0x04000A78 RID: 2680
		private MissionShip _formationMainShip;
	}
}
