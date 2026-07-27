using System;
using NavalDLC.Missions.Objects.UsableMachines;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.AI.UsableMachineAIs
{
	// Token: 0x020000E9 RID: 233
	public sealed class ShipControllerMachineAI : UsableMachineAIBase
	{
		// Token: 0x17000319 RID: 793
		// (get) Token: 0x060011F1 RID: 4593 RVA: 0x000829FF File Offset: 0x00080BFF
		public override bool HasActionCompleted
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700031A RID: 794
		// (get) Token: 0x060011F2 RID: 4594 RVA: 0x00082A02 File Offset: 0x00080C02
		protected override MovementOrder NextOrder
		{
			get
			{
				return MovementOrder.MovementOrderCharge;
			}
		}

		// Token: 0x1700031B RID: 795
		// (get) Token: 0x060011F3 RID: 4595 RVA: 0x00082A09 File Offset: 0x00080C09
		private ShipControllerMachine ShipControllerMachine
		{
			get
			{
				return this.UsableMachine as ShipControllerMachine;
			}
		}

		// Token: 0x060011F4 RID: 4596 RVA: 0x00082A16 File Offset: 0x00080C16
		public ShipControllerMachineAI(ShipControllerMachine shipControllerMachine)
			: base(shipControllerMachine)
		{
		}
	}
}
