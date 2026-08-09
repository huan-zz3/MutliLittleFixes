using System;

namespace NavalDLC.Missions.ShipActuators
{
	// Token: 0x02000096 RID: 150
	public struct ShipActuatorRecord
	{
		// Token: 0x06000AAF RID: 2735 RVA: 0x0004A85E File Offset: 0x00048A5E
		public ShipActuatorRecord(float rowerThrust, float rowerThrustDoubleTap, float rowerRotation, float rudderRotation, float squareSailSetting, float lateenSailSetting)
		{
			this.RowerThrust = rowerThrust;
			this.RowerThrustDoubleTap = rowerThrustDoubleTap;
			this.RowerRotation = rowerRotation;
			this.RudderRotation = rudderRotation;
			this.SquareSailSetting = squareSailSetting;
			this.LateenSailSetting = lateenSailSetting;
		}

		// Token: 0x04000632 RID: 1586
		public readonly float RowerThrust;

		// Token: 0x04000633 RID: 1587
		public readonly float RowerThrustDoubleTap;

		// Token: 0x04000634 RID: 1588
		public readonly float RowerRotation;

		// Token: 0x04000635 RID: 1589
		public readonly float RudderRotation;

		// Token: 0x04000636 RID: 1590
		public readonly float SquareSailSetting;

		// Token: 0x04000637 RID: 1591
		public readonly float LateenSailSetting;
	}
}
