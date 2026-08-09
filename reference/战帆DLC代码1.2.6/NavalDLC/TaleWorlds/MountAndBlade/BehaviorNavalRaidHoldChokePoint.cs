using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200000C RID: 12
	public class BehaviorNavalRaidHoldChokePoint : BehaviorComponent
	{
		// Token: 0x0600005E RID: 94 RVA: 0x000041C3 File Offset: 0x000023C3
		public BehaviorNavalRaidHoldChokePoint(Formation formation)
			: base(formation)
		{
			this.CalculateCurrentOrder();
		}

		// Token: 0x0600005F RID: 95 RVA: 0x000041DD File Offset: 0x000023DD
		public void SetTacticalDefendPosition(TacticalPosition tacticalPosition)
		{
			this._tacticalDefendPosition = tacticalPosition;
		}

		// Token: 0x06000060 RID: 96 RVA: 0x000041E8 File Offset: 0x000023E8
		protected override void CalculateCurrentOrder()
		{
			Vec2 vec;
			if (this._tacticalDefendPosition != null)
			{
				vec = ((!this._tacticalDefendPosition.IsInsurmountable) ? this._tacticalDefendPosition.Direction : (base.Formation.Team.QuerySystem.AverageEnemyPosition - this._tacticalDefendPosition.Position.AsVec2).Normalized());
			}
			else if (base.Formation.CachedClosestEnemyFormation == null)
			{
				vec = base.Formation.Direction;
			}
			else
			{
				vec = ((base.Formation.Direction.DotProduct((base.Formation.CachedClosestEnemyFormation.Formation.CachedMedianPosition.AsVec2 - base.Formation.CachedAveragePosition).Normalized()) < 0.5f) ? (base.Formation.CachedClosestEnemyFormation.Formation.CachedMedianPosition.AsVec2 - base.Formation.CachedAveragePosition) : base.Formation.Direction).Normalized();
			}
			if (this._tacticalDefendPosition != null)
			{
				if (!this._tacticalDefendPosition.IsInsurmountable)
				{
					base.CurrentOrder = MovementOrder.MovementOrderMove(this._tacticalDefendPosition.Position);
				}
				else
				{
					Vec2 vec2 = this._tacticalDefendPosition.Position.AsVec2 + this._tacticalDefendPosition.Width * 0.5f * vec;
					WorldPosition position = this._tacticalDefendPosition.Position;
					position.SetVec2(vec2);
					base.CurrentOrder = MovementOrder.MovementOrderMove(position);
				}
				this.CurrentFacingOrder = ((!this._tacticalDefendPosition.IsInsurmountable) ? FacingOrder.FacingOrderLookAtDirection(vec) : FacingOrder.FacingOrderLookAtEnemy);
				return;
			}
			if (this._defensePosition.IsValid)
			{
				base.CurrentOrder = MovementOrder.MovementOrderMove(this._defensePosition);
				this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(vec);
				return;
			}
			WorldPosition cachedMedianPosition = base.Formation.CachedMedianPosition;
			cachedMedianPosition.SetVec2(base.Formation.CachedAveragePosition);
			base.CurrentOrder = MovementOrder.MovementOrderMove(cachedMedianPosition);
			this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(vec);
		}

		// Token: 0x06000061 RID: 97 RVA: 0x0000440C File Offset: 0x0000260C
		public override void TickOccasionally()
		{
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.SetFacingOrder(this.CurrentFacingOrder);
			if (base.Formation.GetUnderAttackTypeOfUnits(5f) == 1)
			{
				base.Formation.SetArrangementOrder(ArrangementOrder.ArrangementOrderLine);
				return;
			}
			base.Formation.SetArrangementOrder(ArrangementOrder.ArrangementOrderShieldWall);
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00004478 File Offset: 0x00002678
		protected override void OnBehaviorActivatedAux()
		{
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.SetFacingOrder(this.CurrentFacingOrder);
			base.Formation.SetArrangementOrder(ArrangementOrder.ArrangementOrderShieldWall);
			base.Formation.SetFiringOrder(FiringOrder.FiringOrderFireAtWill);
			base.Formation.SetFormOrder(FormOrder.FormOrderWide, true);
		}

		// Token: 0x06000063 RID: 99 RVA: 0x000044DE File Offset: 0x000026DE
		public override void ResetBehavior()
		{
			base.ResetBehavior();
			this._defensePosition = WorldPosition.Invalid;
			this._tacticalDefendPosition = null;
		}

		// Token: 0x06000064 RID: 100 RVA: 0x000044F8 File Offset: 0x000026F8
		protected override float GetAiWeight()
		{
			return 1f;
		}

		// Token: 0x0400003A RID: 58
		private WorldPosition _defensePosition = WorldPosition.Invalid;

		// Token: 0x0400003B RID: 59
		private TacticalPosition _tacticalDefendPosition;
	}
}
