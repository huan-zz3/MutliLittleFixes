using System;
using TaleWorlds.Library;

namespace NavalDLC.DWA
{
	// Token: 0x0200014D RID: 333
	public class DWAObstacleVertex : IDWAObstacleVertex
	{
		// Token: 0x170003AA RID: 938
		// (get) Token: 0x060015BB RID: 5563 RVA: 0x000986F2 File Offset: 0x000968F2
		int IDWAObstacleVertex.Id
		{
			get
			{
				return this.Id;
			}
		}

		// Token: 0x170003AB RID: 939
		// (get) Token: 0x060015BC RID: 5564 RVA: 0x000986FA File Offset: 0x000968FA
		Vec2 IDWAObstacleVertex.Point
		{
			get
			{
				return this.Point;
			}
		}

		// Token: 0x170003AC RID: 940
		// (get) Token: 0x060015BD RID: 5565 RVA: 0x00098702 File Offset: 0x00096902
		float IDWAObstacleVertex.PointZ
		{
			get
			{
				return this.PointZ;
			}
		}

		// Token: 0x170003AD RID: 941
		// (get) Token: 0x060015BE RID: 5566 RVA: 0x0009870A File Offset: 0x0009690A
		public int Id { get; }

		// Token: 0x170003AE RID: 942
		// (get) Token: 0x060015BF RID: 5567 RVA: 0x00098712 File Offset: 0x00096912
		// (set) Token: 0x060015C0 RID: 5568 RVA: 0x0009871A File Offset: 0x0009691A
		public Vec2 Point { get; internal set; }

		// Token: 0x170003AF RID: 943
		// (get) Token: 0x060015C1 RID: 5569 RVA: 0x00098724 File Offset: 0x00096924
		public Vec3 Point3D
		{
			get
			{
				return this.Point.ToVec3(this.PointZ);
			}
		}

		// Token: 0x170003B0 RID: 944
		// (get) Token: 0x060015C2 RID: 5570 RVA: 0x00098745 File Offset: 0x00096945
		// (set) Token: 0x060015C3 RID: 5571 RVA: 0x0009874D File Offset: 0x0009694D
		public float PointZ { get; internal set; }

		// Token: 0x170003B1 RID: 945
		// (get) Token: 0x060015C4 RID: 5572 RVA: 0x00098756 File Offset: 0x00096956
		// (set) Token: 0x060015C5 RID: 5573 RVA: 0x0009875E File Offset: 0x0009695E
		public Vec2 Direction
		{
			get
			{
				return this._direction;
			}
			internal set
			{
				this._direction = value;
			}
		}

		// Token: 0x170003B2 RID: 946
		// (get) Token: 0x060015C6 RID: 5574 RVA: 0x00098767 File Offset: 0x00096967
		// (set) Token: 0x060015C7 RID: 5575 RVA: 0x0009876F File Offset: 0x0009696F
		public DWAObstacleVertex Previous { get; internal set; }

		// Token: 0x170003B3 RID: 947
		// (get) Token: 0x060015C8 RID: 5576 RVA: 0x00098778 File Offset: 0x00096978
		// (set) Token: 0x060015C9 RID: 5577 RVA: 0x00098780 File Offset: 0x00096980
		public DWAObstacleVertex Next { get; internal set; }

		// Token: 0x170003B4 RID: 948
		// (get) Token: 0x060015CA RID: 5578 RVA: 0x00098789 File Offset: 0x00096989
		// (set) Token: 0x060015CB RID: 5579 RVA: 0x00098791 File Offset: 0x00096991
		public bool IsConvex { get; internal set; }

		// Token: 0x060015CC RID: 5580 RVA: 0x0009879C File Offset: 0x0009699C
		internal DWAObstacleVertex(int id)
		{
			this.Id = id;
			this.Point = Vec2.Invalid;
			this.PointZ = 0f;
			this.Direction = Vec2.Forward;
			this.Previous = null;
			this.Next = null;
			this.IsConvex = false;
		}

		// Token: 0x04000B39 RID: 2873
		private Vec2 _direction;
	}
}
