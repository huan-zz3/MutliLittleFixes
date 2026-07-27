using System;
using NavalDLC.Missions.MissionLogics;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	// Token: 0x02000009 RID: 9
	public class NavalBattleAgentLogic : BattleAgentLogic
	{
		// Token: 0x06000051 RID: 81 RVA: 0x00003D2C File Offset: 0x00001F2C
		public override void OnBehaviorInitialize()
		{
			this._agentsLogic = base.Mission.GetMissionBehavior<NavalAgentsLogic>();
			base.OnBehaviorInitialize();
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00003D45 File Offset: 0x00001F45
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow killingBlow)
		{
			if (this._agentsLogic.IsDeploymentMode || this._agentsLogic.IsMissionEnding)
			{
				return;
			}
			base.OnAgentRemoved(affectedAgent, affectorAgent, agentState, killingBlow);
		}

		// Token: 0x04000037 RID: 55
		private NavalAgentsLogic _agentsLogic;
	}
}
