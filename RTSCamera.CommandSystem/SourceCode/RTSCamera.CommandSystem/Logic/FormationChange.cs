using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace RTSCamera.CommandSystem.Logic
{
	// Token: 0x02000083 RID: 131
	public struct FormationChange
	{
		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x060004EC RID: 1260 RVA: 0x0001CF54 File Offset: 0x0001B154
		public readonly Vec2? Position
		{
			get
			{
				if (this.WorldPosition == null)
				{
					return null;
				}
				return new Vec2?(this.WorldPosition.GetValueOrDefault().AsVec2);
			}
		}

		// Token: 0x040001F6 RID: 502
		public WorldPosition? WorldPosition;

		// Token: 0x040001F7 RID: 503
		public Vec2? Direciton;

		// Token: 0x040001F8 RID: 504
		public int? UnitSpacing;

		// Token: 0x040001F9 RID: 505
		public float? Width;

		// Token: 0x040001FA RID: 506
		public OrderType? MovementOrderType;

		// Token: 0x040001FB RID: 507
		public Formation TargetFormation;

		// Token: 0x040001FC RID: 508
		public Agent TargetAgent;

		// Token: 0x040001FD RID: 509
		public Formation FacingEnemyTargetFormation;

		// Token: 0x040001FE RID: 510
		public IOrderable TargetEntity;

		// Token: 0x040001FF RID: 511
		public OrderType? FacingOrderType;

		// Token: 0x04000200 RID: 512
		public OrderType? FiringOrderType;

		// Token: 0x04000201 RID: 513
		public OrderType? RidingOrderType;

		// Token: 0x04000202 RID: 514
		public OrderType? AIControlOrderType;

		// Token: 0x04000203 RID: 515
		public ArrangementOrder.ArrangementOrderEnum? ArrangementOrder;

		// Token: 0x04000204 RID: 516
		public VolleyMode? VolleyMode;

		// Token: 0x04000205 RID: 517
		public float? PreviewWidth;

		// Token: 0x04000206 RID: 518
		public float? PreviewDepth;
	}
}
