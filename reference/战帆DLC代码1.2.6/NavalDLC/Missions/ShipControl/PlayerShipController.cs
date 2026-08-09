using System;
using NavalDLC.Missions.Objects;
using NavalDLC.Missions.ShipInput;

namespace NavalDLC.Missions.ShipControl
{
	// Token: 0x0200008E RID: 142
	public class PlayerShipController : ShipController
	{
		// Token: 0x06000A12 RID: 2578 RVA: 0x00046995 File Offset: 0x00044B95
		public PlayerShipController(MissionShip ownerShip)
			: base(ownerShip)
		{
			this._controllerType = ShipControllerType.Player;
		}

		// Token: 0x06000A13 RID: 2579 RVA: 0x000469A5 File Offset: 0x00044BA5
		public void SetInput(in ShipInputRecord inputRecord)
		{
			this._inputRecord = inputRecord;
		}

		// Token: 0x06000A14 RID: 2580 RVA: 0x000469B3 File Offset: 0x00044BB3
		public override ShipInputRecord Update(float dt)
		{
			return this._inputRecord;
		}

		// Token: 0x040005D0 RID: 1488
		private ShipInputRecord _inputRecord;
	}
}
