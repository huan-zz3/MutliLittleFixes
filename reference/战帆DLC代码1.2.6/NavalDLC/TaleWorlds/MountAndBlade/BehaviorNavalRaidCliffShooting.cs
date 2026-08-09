using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200000B RID: 11
	public class BehaviorNavalRaidCliffShooting : BehaviorComponent
	{
		// Token: 0x06000057 RID: 87 RVA: 0x00003E20 File Offset: 0x00002020
		public BehaviorNavalRaidCliffShooting(Formation formation)
			: base(formation)
		{
			this.CalculateCurrentOrder();
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00003E3A File Offset: 0x0000203A
		public void SetTacticalDefendPosition(TacticalPosition tacticalPosition)
		{
			this._tacticalDefendPosition = tacticalPosition;
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00003E44 File Offset: 0x00002044
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

		// Token: 0x0600005A RID: 90 RVA: 0x00004068 File Offset: 0x00002268
		public override void TickOccasionally()
		{
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.SetFacingOrder(this.CurrentFacingOrder);
			if (base.Formation.CachedAveragePosition.DistanceSquared(base.CurrentOrder.GetPosition(base.Formation)) < 100f)
			{
				if (this._tacticalDefendPosition != null)
				{
					int countOfUnits = base.Formation.CountOfUnits;
					float num = base.Formation.Interval * (float)(countOfUnits - 1) + base.Formation.UnitDiameter * (float)countOfUnits;
					float num2 = MathF.Min(this._tacticalDefendPosition.Width, num / 3f);
					base.Formation.SetFormOrder(FormOrder.FormOrderCustom(num2), true);
					return;
				}
			}
			else
			{
				base.Formation.SetArrangementOrder(ArrangementOrder.ArrangementOrderScatter);
			}
		}

		// Token: 0x0600005B RID: 91 RVA: 0x0000413C File Offset: 0x0000233C
		protected override void OnBehaviorActivatedAux()
		{
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.SetFacingOrder(this.CurrentFacingOrder);
			base.Formation.SetArrangementOrder(ArrangementOrder.ArrangementOrderScatter);
			base.Formation.SetFiringOrder(FiringOrder.FiringOrderFireAtWill);
			base.Formation.SetFormOrder(FormOrder.FormOrderWide, true);
		}

		// Token: 0x0600005C RID: 92 RVA: 0x000041A2 File Offset: 0x000023A2
		public override void ResetBehavior()
		{
			base.ResetBehavior();
			this._defensePosition = WorldPosition.Invalid;
			this._tacticalDefendPosition = null;
		}

		// Token: 0x0600005D RID: 93 RVA: 0x000041BC File Offset: 0x000023BC
		protected override float GetAiWeight()
		{
			return 1f;
		}

		// Token: 0x04000038 RID: 56
		private WorldPosition _defensePosition = WorldPosition.Invalid;

		// Token: 0x04000039 RID: 57
		private TacticalPosition _tacticalDefendPosition;
	}
}
