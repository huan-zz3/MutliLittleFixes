using System;
using NavalDLC.Storyline.MissionControllers;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.Objectives;

namespace NavalDLC.Storyline.Objectives.Quest3
{
	// Token: 0x0200005F RID: 95
	internal class SwimToShoreObjective : MissionObjective
	{
		// Token: 0x17000121 RID: 289
		// (get) Token: 0x060005D2 RID: 1490 RVA: 0x00022DC0 File Offset: 0x00020FC0
		public override string UniqueId
		{
			get
			{
				return "naval_storyline_quest_3_reach_horses_objective";
			}
		}

		// Token: 0x17000122 RID: 290
		// (get) Token: 0x060005D3 RID: 1491 RVA: 0x00022DC7 File Offset: 0x00020FC7
		public override TextObject Name
		{
			get
			{
				return new TextObject("{=h8HcPYjn}Swim to shore", null);
			}
		}

		// Token: 0x17000123 RID: 291
		// (get) Token: 0x060005D4 RID: 1492 RVA: 0x00022DD4 File Offset: 0x00020FD4
		public override TextObject Description
		{
			get
			{
				return new TextObject("{=dBQj9VSX}Swim to shore and reach your horses.", null);
			}
		}

		// Token: 0x060005D5 RID: 1493 RVA: 0x00022DE4 File Offset: 0x00020FE4
		internal SwimToShoreObjective(Mission mission, Agent gunnarAgent)
			: base(mission)
		{
			this._controller = base.Mission.GetMissionBehavior<BlockedEstuaryMissionController>();
			foreach (Agent agent in base.Mission.AllAgents)
			{
				if (agent.IsActive() && agent.IsMount)
				{
					base.AddTarget(new AgentObjectiveTarget(agent));
				}
			}
			if (gunnarAgent != null && gunnarAgent.IsActive())
			{
				base.AddTarget(new AgentObjectiveTarget(gunnarAgent));
			}
		}

		// Token: 0x060005D6 RID: 1494 RVA: 0x00022E80 File Offset: 0x00021080
		protected override bool IsActivationRequirementsMet()
		{
			return this._controller.CurrentPhase == BlockedEstuaryMissionController.BattlePhase.Phase2;
		}

		// Token: 0x060005D7 RID: 1495 RVA: 0x00022E90 File Offset: 0x00021090
		protected override bool IsCompletionRequirementsMet()
		{
			return this._controller.CurrentPhase != BlockedEstuaryMissionController.BattlePhase.Phase2;
		}

		// Token: 0x040002CE RID: 718
		private BlockedEstuaryMissionController _controller;
	}
}
