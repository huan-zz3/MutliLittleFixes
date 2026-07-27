using System;
using NavalDLC.Missions.Objects.UsableMachines;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.AI.UsableMachineAIs
{
	// Token: 0x020000EA RID: 234
	public sealed class ShipOarMachineAI : UsableMachineAIBase
	{
		// Token: 0x1700031C RID: 796
		// (get) Token: 0x060011F5 RID: 4597 RVA: 0x00082A1F File Offset: 0x00080C1F
		public override bool HasActionCompleted
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700031D RID: 797
		// (get) Token: 0x060011F6 RID: 4598 RVA: 0x00082A22 File Offset: 0x00080C22
		protected override MovementOrder NextOrder
		{
			get
			{
				return MovementOrder.MovementOrderCharge;
			}
		}

		// Token: 0x1700031E RID: 798
		// (get) Token: 0x060011F7 RID: 4599 RVA: 0x00082A29 File Offset: 0x00080C29
		private ShipOarMachine ShipOarMachine
		{
			get
			{
				return this.UsableMachine as ShipOarMachine;
			}
		}

		// Token: 0x060011F8 RID: 4600 RVA: 0x00082A36 File Offset: 0x00080C36
		public ShipOarMachineAI(ShipOarMachine shipOarMachine)
			: base(shipOarMachine)
		{
		}

		// Token: 0x060011F9 RID: 4601 RVA: 0x00082A3F File Offset: 0x00080C3F
		protected override void HandleAgentStopUsingStandingPoint(Agent agent, StandingPoint standingPoint)
		{
			if (agent == this.ShipOarMachine.PilotAgent)
			{
				this.ShipOarMachine.StartDelayedPilotRemoval(base.GetStopUsingStandingPointFlags(agent, standingPoint));
				return;
			}
			base.HandleAgentStopUsingStandingPoint(agent, standingPoint);
		}
	}
}
