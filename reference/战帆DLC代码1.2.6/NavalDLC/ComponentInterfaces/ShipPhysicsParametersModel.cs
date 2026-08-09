using System;
using TaleWorlds.Core;

namespace NavalDLC.ComponentInterfaces
{
	// Token: 0x02000154 RID: 340
	public abstract class ShipPhysicsParametersModel : MBGameModel<ShipPhysicsParametersModel>
	{
		// Token: 0x06001647 RID: 5703
		public abstract float GetWaterDensity();

		// Token: 0x06001648 RID: 5704
		public abstract float GetAirDensity();
	}
}
