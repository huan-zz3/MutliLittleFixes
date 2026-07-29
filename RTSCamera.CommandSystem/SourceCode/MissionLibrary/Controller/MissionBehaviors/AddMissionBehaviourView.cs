using System;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace MissionLibrary.Controller.MissionBehaviors
{
	// Token: 0x02000026 RID: 38
	[DefaultView]
	internal class AddMissionBehaviourView : MissionView
	{
		// Token: 0x0600009B RID: 155 RVA: 0x00002A19 File Offset: 0x00000C19
		public override void OnCreated()
		{
			base.OnCreated();
			Global.GetInstance<AMissionStartingManager>("").OnCreated(this);
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00002A34 File Offset: 0x00000C34
		public override void OnPreMissionTick(float dt)
		{
			base.OnPreMissionTick(dt);
			Global.GetInstance<AMissionStartingManager>("").OnPreMissionTick(this, dt);
			AddMissionBehaviourView missionBehavior = base.Mission.GetMissionBehavior<AddMissionBehaviourView>();
			if (missionBehavior == this)
			{
				base.Mission.RemoveMissionBehavior(missionBehavior);
			}
		}
	}
}
