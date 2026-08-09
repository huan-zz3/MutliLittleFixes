using System;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Storyline.MissionControllers
{
	// Token: 0x02000068 RID: 104
	public class BlockedEstuaryBattleEndLogic : NavalBattleEndLogic
	{
		// Token: 0x06000613 RID: 1555 RVA: 0x000234F8 File Offset: 0x000216F8
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._controller = base.Mission.GetMissionBehavior<BlockedEstuaryMissionController>();
		}

		// Token: 0x06000614 RID: 1556 RVA: 0x00023511 File Offset: 0x00021711
		public override void OnMissionTick(float dt)
		{
			if (this._controller.CanEndBattleNatively)
			{
				base.OnMissionTick(dt);
			}
		}

		// Token: 0x040002F1 RID: 753
		private BlockedEstuaryMissionController _controller;
	}
}
