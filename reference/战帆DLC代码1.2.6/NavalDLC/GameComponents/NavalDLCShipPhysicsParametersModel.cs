using System;
using NavalDLC.ComponentInterfaces;

namespace NavalDLC.GameComponents
{
	// Token: 0x02000136 RID: 310
	public class NavalDLCShipPhysicsParametersModel : ShipPhysicsParametersModel
	{
		// Token: 0x0600150D RID: 5389 RVA: 0x00094A41 File Offset: 0x00092C41
		public override float GetWaterDensity()
		{
			return 1020f;
		}

		// Token: 0x0600150E RID: 5390 RVA: 0x00094A48 File Offset: 0x00092C48
		public override float GetAirDensity()
		{
			return 1.225f;
		}
	}
}
