using System;
using TaleWorlds.Library;

namespace NavalDLC.Missions.NavalPhysics
{
	// Token: 0x020000C2 RID: 194
	public struct ShipForce
	{
		// Token: 0x170002B2 RID: 690
		// (get) Token: 0x06000EB2 RID: 3762 RVA: 0x00072E80 File Offset: 0x00071080
		public bool IsApplicable
		{
			get
			{
				return this.Force.IsValid && this.Force.IsNonZero;
			}
		}

		// Token: 0x06000EB3 RID: 3763 RVA: 0x00072E9C File Offset: 0x0007109C
		public ShipForce(in Vec3 localPosition, in Vec3 force, ShipForce.SourceType source, float gamifiedForceMultiplier)
		{
			this.LocalPosition = localPosition;
			this.Force = new Vec3(force, 0f);
			this.Source = source;
			this.GamifiedForceMultiplier = gamifiedForceMultiplier;
		}

		// Token: 0x06000EB4 RID: 3764 RVA: 0x00072ECF File Offset: 0x000710CF
		public ShipForce(ShipForce.SourceType source)
		{
			this.LocalPosition = Vec3.Zero;
			this.Force = Vec3.Zero;
			this.Source = source;
			this.GamifiedForceMultiplier = 1f;
		}

		// Token: 0x06000EB5 RID: 3765 RVA: 0x00072EF9 File Offset: 0x000710F9
		public void ComputeRealisticAndGamifiedForceComponents(out Vec3 realisticForce, out Vec3 gamifiedForce)
		{
			realisticForce = this.Force / this.GamifiedForceMultiplier;
			gamifiedForce = realisticForce * (this.GamifiedForceMultiplier - 1f);
		}

		// Token: 0x06000EB6 RID: 3766 RVA: 0x00072F2F File Offset: 0x0007112F
		public static ShipForce None()
		{
			return new ShipForce(ShipForce.SourceType.None);
		}

		// Token: 0x06000EB7 RID: 3767 RVA: 0x00072F37 File Offset: 0x00071137
		public static ShipForce None(ShipForce.SourceType source)
		{
			return new ShipForce(source);
		}

		// Token: 0x04000926 RID: 2342
		public readonly Vec3 LocalPosition;

		// Token: 0x04000927 RID: 2343
		public Vec3 Force;

		// Token: 0x04000928 RID: 2344
		public readonly ShipForce.SourceType Source;

		// Token: 0x04000929 RID: 2345
		public readonly float GamifiedForceMultiplier;

		// Token: 0x02000246 RID: 582
		public enum SourceType
		{
			// Token: 0x0400103F RID: 4159
			None,
			// Token: 0x04001040 RID: 4160
			Sail,
			// Token: 0x04001041 RID: 4161
			Oar,
			// Token: 0x04001042 RID: 4162
			Rudder
		}
	}
}
