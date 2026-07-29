using System;
using TaleWorlds.MountAndBlade;

namespace RTSCameraAgentComponent
{
	// Token: 0x02000002 RID: 2
	public class ComponentAdder : MissionLogic
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public override void OnAgentCreated(Agent agent)
		{
			base.OnAgentCreated(agent);
			agent.AddComponent(new RTSCameraComponent(agent));
		}
	}
}
