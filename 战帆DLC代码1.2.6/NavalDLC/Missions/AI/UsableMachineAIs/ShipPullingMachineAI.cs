using System;
using NavalDLC.Missions.Objects.UsableMachines;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.AI.UsableMachineAIs
{
	// Token: 0x020000EB RID: 235
	public sealed class ShipPullingMachineAI : UsableMachineAIBase
	{
		// Token: 0x1700031F RID: 799
		// (get) Token: 0x060011FA RID: 4602 RVA: 0x00082A6B File Offset: 0x00080C6B
		public override bool HasActionCompleted
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000320 RID: 800
		// (get) Token: 0x060011FB RID: 4603 RVA: 0x00082A6E File Offset: 0x00080C6E
		protected override MovementOrder NextOrder
		{
			get
			{
				return MovementOrder.MovementOrderCharge;
			}
		}

		// Token: 0x17000321 RID: 801
		// (get) Token: 0x060011FC RID: 4604 RVA: 0x00082A75 File Offset: 0x00080C75
		private ShipPullingMachine ShipPullingMachine
		{
			get
			{
				return this.UsableMachine as ShipPullingMachine;
			}
		}

		// Token: 0x060011FD RID: 4605 RVA: 0x00082A82 File Offset: 0x00080C82
		public ShipPullingMachineAI(ShipPullingMachine shipPullingMachine)
			: base(shipPullingMachine)
		{
		}
	}
}
