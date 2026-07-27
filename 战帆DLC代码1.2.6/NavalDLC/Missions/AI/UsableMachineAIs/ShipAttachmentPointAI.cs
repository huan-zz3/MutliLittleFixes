using System;
using NavalDLC.Missions.Objects.UsableMachines;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.AI.UsableMachineAIs
{
	// Token: 0x020000E7 RID: 231
	public sealed class ShipAttachmentPointAI : UsableMachineAIBase
	{
		// Token: 0x17000316 RID: 790
		// (get) Token: 0x060011E8 RID: 4584 RVA: 0x00082998 File Offset: 0x00080B98
		public override bool HasActionCompleted
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000317 RID: 791
		// (get) Token: 0x060011E9 RID: 4585 RVA: 0x0008299B File Offset: 0x00080B9B
		protected override MovementOrder NextOrder
		{
			get
			{
				return MovementOrder.MovementOrderCharge;
			}
		}

		// Token: 0x060011EA RID: 4586 RVA: 0x000829A2 File Offset: 0x00080BA2
		public ShipAttachmentPointAI(ShipAttachmentPointMachine shipAttachmentPointMachine)
			: base(shipAttachmentPointMachine)
		{
		}
	}
}
