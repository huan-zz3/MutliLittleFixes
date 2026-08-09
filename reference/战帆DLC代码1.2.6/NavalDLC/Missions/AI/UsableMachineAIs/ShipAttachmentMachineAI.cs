using System;
using NavalDLC.Missions.Objects.UsableMachines;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.AI.UsableMachineAIs
{
	// Token: 0x020000E6 RID: 230
	public sealed class ShipAttachmentMachineAI : UsableMachineAIBase
	{
		// Token: 0x17000313 RID: 787
		// (get) Token: 0x060011E4 RID: 4580 RVA: 0x00082978 File Offset: 0x00080B78
		public override bool HasActionCompleted
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000314 RID: 788
		// (get) Token: 0x060011E5 RID: 4581 RVA: 0x0008297B File Offset: 0x00080B7B
		protected override MovementOrder NextOrder
		{
			get
			{
				return MovementOrder.MovementOrderCharge;
			}
		}

		// Token: 0x17000315 RID: 789
		// (get) Token: 0x060011E6 RID: 4582 RVA: 0x00082982 File Offset: 0x00080B82
		private ShipAttachmentMachine ShipAttachmentMachine
		{
			get
			{
				return this.UsableMachine as ShipAttachmentMachine;
			}
		}

		// Token: 0x060011E7 RID: 4583 RVA: 0x0008298F File Offset: 0x00080B8F
		public ShipAttachmentMachineAI(ShipAttachmentMachine shipAttachmentMachine)
			: base(shipAttachmentMachine)
		{
		}
	}
}
