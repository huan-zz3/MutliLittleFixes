using System;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.AI.UsableMachineAIs
{
	// Token: 0x020000E8 RID: 232
	public sealed class ShipBallistaAI : BallistaAI
	{
		// Token: 0x17000318 RID: 792
		// (get) Token: 0x060011EB RID: 4587 RVA: 0x000829AB File Offset: 0x00080BAB
		// (set) Token: 0x060011EC RID: 4588 RVA: 0x000829B3 File Offset: 0x00080BB3
		public bool IsUnderDirectControl { get; private set; }

		// Token: 0x060011ED RID: 4589 RVA: 0x000829BC File Offset: 0x00080BBC
		public ShipBallistaAI(Ballista ballista)
			: base(ballista)
		{
			this.IsUnderDirectControl = false;
		}

		// Token: 0x060011EE RID: 4590 RVA: 0x000829D3 File Offset: 0x00080BD3
		protected override void UpdateAim(RangedSiegeWeapon rangedSiegeWeapon, float dt)
		{
			if (!this.IsUnderDirectControl && this._canAiUpdateAim)
			{
				base.UpdateAim(rangedSiegeWeapon, dt);
			}
		}

		// Token: 0x060011EF RID: 4591 RVA: 0x000829ED File Offset: 0x00080BED
		public void SetCanAiUpdateAim(bool canAiUpdateAim)
		{
			this._canAiUpdateAim = canAiUpdateAim;
		}

		// Token: 0x060011F0 RID: 4592 RVA: 0x000829F6 File Offset: 0x00080BF6
		public void SetIsUnderDirectControl(bool value)
		{
			this.IsUnderDirectControl = value;
		}

		// Token: 0x04000A20 RID: 2592
		private bool _canAiUpdateAim = true;
	}
}
