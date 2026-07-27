using System;
using System.Collections.Generic;
using NavalDLC.Missions.AI.Tactics;
using NavalDLC.Missions.AI.TeamAI;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.MissionLogics
{
	// Token: 0x020000CF RID: 207
	public class NavalMissionCombatantsLogic : MissionCombatantsLogic
	{
		// Token: 0x06000F79 RID: 3961 RVA: 0x000768F8 File Offset: 0x00074AF8
		public NavalMissionCombatantsLogic(IEnumerable<IBattleCombatant> battleCombatants, IBattleCombatant playerBattleCombatant, IBattleCombatant defenderLeaderBattleCombatant, IBattleCombatant attackerLeaderBattleCombatant, Mission.MissionTeamAITypeEnum teamAIType, bool isPlayerSergeant)
			: base(battleCombatants, playerBattleCombatant, defenderLeaderBattleCombatant, attackerLeaderBattleCombatant, teamAIType, isPlayerSergeant)
		{
		}

		// Token: 0x06000F7A RID: 3962 RVA: 0x0007690C File Offset: 0x00074B0C
		public override void EarlyStart()
		{
			Mission.Current.MissionTeamAIType = this.TeamAIType;
			foreach (Team team in Mission.Current.Teams)
			{
				if (this.TeamAIType == 4)
				{
					team.AddTeamAI(new TeamAINavalComponent(base.Mission, team, 5f, 1f), false);
				}
				else if (this.TeamAIType == 5)
				{
					if (team.IsAttacker)
					{
						team.AddTeamAI(new TeamAINavalRaidAttackerComponent(base.Mission, team, 5f, 1f), false);
					}
					else
					{
						team.AddTeamAI(new TeamAINavalRaidDefenderComponent(base.Mission, team, 5f, 1f), false);
					}
				}
			}
			if (Mission.Current.Teams.Count > 0)
			{
				foreach (Team team2 in Mission.Current.Teams)
				{
					if (team2.HasTeamAi)
					{
						if (this.TeamAIType == 4)
						{
							team2.AddTacticOption(new TacticNavalBalancedOffense(team2));
							if (team2.Side == null)
							{
								team2.AddTacticOption(new TacticNavalLineDefense(team2));
							}
						}
						else if (this.TeamAIType == 5)
						{
							team2.AddTacticOption(new TacticCharge(team2));
							if (team2.Side == null)
							{
								team2.AddTacticOption(new TacticNavalRaidDefense(team2));
							}
						}
					}
				}
				foreach (Team team3 in base.Mission.Teams)
				{
					team3.QuerySystem.Expire();
					team3.ResetTactic();
				}
			}
		}
	}
}
