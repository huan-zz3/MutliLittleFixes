using System;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.Objects.UsableMachines
{
	// Token: 0x020000BC RID: 188
	public class StandingPointWithSwimmingLimit : StandingPoint
	{
		// Token: 0x06000E41 RID: 3649 RVA: 0x0006F3C7 File Offset: 0x0006D5C7
		public override bool IsDisabledForAgent(Agent agent)
		{
			return !agent.IsInWater() || base.IsDisabledForAgent(agent);
		}
	}
}
