using System;
using TaleWorlds.Library;

namespace NavalDLC.DWA
{
	// Token: 0x02000148 RID: 328
	public struct DWAAgentState
	{
		// Token: 0x170003A6 RID: 934
		// (get) Token: 0x060015A6 RID: 5542 RVA: 0x00097750 File Offset: 0x00095950
		public Vec2 ShapeCenter
		{
			get
			{
				return this.Position + this.Direction * this.ShapeOffset.Y - this.Direction.LeftVec() * this.ShapeOffset.X * this.ShapeHalfSize.X;
			}
		}

		// Token: 0x170003A7 RID: 935
		// (get) Token: 0x060015A7 RID: 5543 RVA: 0x000977AE File Offset: 0x000959AE
		public float MaxExtent
		{
			get
			{
				return MathF.Max(this.ShapeHalfSize.x, this.ShapeHalfSize.y);
			}
		}

		// Token: 0x170003A8 RID: 936
		// (get) Token: 0x060015A8 RID: 5544 RVA: 0x000977CB File Offset: 0x000959CB
		public float MinExtent
		{
			get
			{
				return MathF.Min(this.ShapeHalfSize.x, this.ShapeHalfSize.y);
			}
		}

		// Token: 0x170003A9 RID: 937
		// (get) Token: 0x060015A9 RID: 5545 RVA: 0x000977E8 File Offset: 0x000959E8
		public Vec3 Position3D
		{
			get
			{
				return this.Position.ToVec3(this.PositionZ);
			}
		}

		// Token: 0x04000B1E RID: 2846
		public Vec2 Position;

		// Token: 0x04000B1F RID: 2847
		public float PositionZ;

		// Token: 0x04000B20 RID: 2848
		public Vec2 Direction;

		// Token: 0x04000B21 RID: 2849
		public Vec2 LinearVelocity;

		// Token: 0x04000B22 RID: 2850
		public float AngularVelocity;

		// Token: 0x04000B23 RID: 2851
		public float LinearAcceleration;

		// Token: 0x04000B24 RID: 2852
		public float AngularAcceleration;

		// Token: 0x04000B25 RID: 2853
		public Vec2 ShapeOffset;

		// Token: 0x04000B26 RID: 2854
		public Vec2 ShapeHalfSize;
	}
}
