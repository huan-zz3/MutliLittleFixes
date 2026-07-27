using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Storyline.MissionControllers
{
	// Token: 0x0200006D RID: 109
	internal class Quest5BattleObserverMissionLogic : BattleObserverMissionLogic
	{
		// Token: 0x0600069A RID: 1690 RVA: 0x00028040 File Offset: 0x00026240
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			if (affectedAgent.Character != NavalStorylineData.Gunnar.CharacterObject)
			{
				base.OnAgentRemoved(affectedAgent, affectorAgent, agentState, blow);
			}
		}

		// Token: 0x0600069B RID: 1691 RVA: 0x0002805F File Offset: 0x0002625F
		public override void OnAgentBuild(Agent agent, Banner banner)
		{
			if (agent.Character == NavalStorylineData.Gunnar.CharacterObject)
			{
				if (!this._isGunnarAddedBefore)
				{
					this._isGunnarAddedBefore = true;
					base.OnAgentBuild(agent, banner);
					return;
				}
			}
			else
			{
				base.OnAgentBuild(agent, banner);
			}
		}

		// Token: 0x04000361 RID: 865
		private bool _isGunnarAddedBefore;
	}
}
