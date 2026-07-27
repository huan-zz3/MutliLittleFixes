using System;
using NavalDLC.Missions.Objects;
using NavalDLC.Missions.ShipInput;

namespace NavalDLC.Missions.ShipControl
{
	// Token: 0x0200008F RID: 143
	public abstract class ShipController
	{
		// Token: 0x170001AC RID: 428
		// (get) Token: 0x06000A15 RID: 2581 RVA: 0x000469BB File Offset: 0x00044BBB
		public bool IsPlayerControlled
		{
			get
			{
				return this._controllerType == ShipControllerType.Player;
			}
		}

		// Token: 0x170001AD RID: 429
		// (get) Token: 0x06000A16 RID: 2582 RVA: 0x000469C6 File Offset: 0x00044BC6
		public bool IsAIControlled
		{
			get
			{
				return this._controllerType == ShipControllerType.AI;
			}
		}

		// Token: 0x170001AE RID: 430
		// (get) Token: 0x06000A17 RID: 2583 RVA: 0x000469D1 File Offset: 0x00044BD1
		public ShipControllerType ControllerType
		{
			get
			{
				return this._controllerType;
			}
		}

		// Token: 0x06000A18 RID: 2584 RVA: 0x000469D9 File Offset: 0x00044BD9
		public ShipController(MissionShip ownerShip)
		{
			this._ownerShip = ownerShip;
			this._controllerType = ShipControllerType.None;
		}

		// Token: 0x06000A19 RID: 2585
		public abstract ShipInputRecord Update(float dt);

		// Token: 0x06000A1A RID: 2586 RVA: 0x000469EF File Offset: 0x00044BEF
		public virtual void Deallocate()
		{
			this._ownerShip = null;
			this._controllerType = ShipControllerType.None;
		}

		// Token: 0x040005D1 RID: 1489
		protected MissionShip _ownerShip;

		// Token: 0x040005D2 RID: 1490
		protected ShipControllerType _controllerType;
	}
}
