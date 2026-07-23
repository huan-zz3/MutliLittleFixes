using System;
using System.Runtime.CompilerServices;
using TaleWorlds.MountAndBlade;

namespace ProjectileTrajectorySystem
{
	// Token: 0x0200000A RID: 10
	public class SubModule : MBSubModuleBase
	{
		// Token: 0x06000056 RID: 86 RVA: 0x00005DBD File Offset: 0x00003FBD
		[NullableContext(1)]
		public override void OnMissionBehaviorInitialize(Mission mission)
		{
			base.OnMissionBehaviorInitialize(mission);
			mission.AddMissionBehavior(new SkillSystemBehavior());
		}
	}
}
