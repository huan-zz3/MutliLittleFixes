using System;
using TaleWorlds.Library;

namespace NavalDLC.Missions.NavalPhysics
{
	// Token: 0x020000C3 RID: 195
	public struct ShipForceRecord
	{
		// Token: 0x170002B3 RID: 691
		// (get) Token: 0x06000EB8 RID: 3768 RVA: 0x00072F3F File Offset: 0x0007113F
		public bool HasLeftOarForces
		{
			get
			{
				return this.LeftOarForces != null && this.LeftOarForces.Count > 0;
			}
		}

		// Token: 0x170002B4 RID: 692
		// (get) Token: 0x06000EB9 RID: 3769 RVA: 0x00072F59 File Offset: 0x00071159
		public bool HasRightOarForces
		{
			get
			{
				return this.RightOarForces != null && this.RightOarForces.Count > 0;
			}
		}

		// Token: 0x170002B5 RID: 693
		// (get) Token: 0x06000EBA RID: 3770 RVA: 0x00072F73 File Offset: 0x00071173
		public bool HasSailForces
		{
			get
			{
				return this.SailForces != null && this.SailForces.Count > 0;
			}
		}

		// Token: 0x06000EBB RID: 3771 RVA: 0x00072F8D File Offset: 0x0007118D
		public ShipForceRecord(MBReadOnlyList<ShipForce> leftOarForces, MBReadOnlyList<ShipForce> rightOarForces, in MBReadOnlyList<ShipForce> sailForces, in ShipForce rudderForce)
		{
			this.LeftOarForces = leftOarForces;
			this.RightOarForces = rightOarForces;
			this.SailForces = sailForces;
			this.RudderForce = rudderForce;
		}

		// Token: 0x06000EBC RID: 3772 RVA: 0x00072FB4 File Offset: 0x000711B4
		public static ShipForceRecord None()
		{
			MBReadOnlyList<ShipForce> mbreadOnlyList = null;
			MBReadOnlyList<ShipForce> mbreadOnlyList2 = null;
			MBReadOnlyList<ShipForce> mbreadOnlyList3 = null;
			ShipForce shipForce = ShipForce.None(ShipForce.SourceType.Rudder);
			return new ShipForceRecord(mbreadOnlyList, mbreadOnlyList2, in mbreadOnlyList3, in shipForce);
		}

		// Token: 0x0400092A RID: 2346
		public readonly MBReadOnlyList<ShipForce> LeftOarForces;

		// Token: 0x0400092B RID: 2347
		public readonly MBReadOnlyList<ShipForce> RightOarForces;

		// Token: 0x0400092C RID: 2348
		public readonly MBReadOnlyList<ShipForce> SailForces;

		// Token: 0x0400092D RID: 2349
		public readonly ShipForce RudderForce;
	}
}
