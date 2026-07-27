using System;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.AI.Behaviors
{
	// Token: 0x020000FA RID: 250
	public abstract class NavalBehaviorComponent : BehaviorComponent
	{
		// Token: 0x060012A1 RID: 4769 RVA: 0x000892A4 File Offset: 0x000874A4
		public NavalBehaviorComponent(Formation formation)
			: base(formation)
		{
		}

		// Token: 0x060012A2 RID: 4770
		public abstract void RefreshShipReferences();
	}
}
