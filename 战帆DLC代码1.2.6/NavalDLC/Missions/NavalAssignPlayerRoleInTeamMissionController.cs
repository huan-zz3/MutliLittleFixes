using System;
using System.Collections.Generic;
using System.Linq;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions
{
	// Token: 0x02000082 RID: 130
	public class NavalAssignPlayerRoleInTeamMissionController : AssignPlayerRoleInTeamMissionController
	{
		// Token: 0x0600097E RID: 2430 RVA: 0x00044398 File Offset: 0x00042598
		public NavalAssignPlayerRoleInTeamMissionController(bool isPlayerGeneral, bool isPlayerSergeant, bool isPlayerInArmy, List<string> charactersInPlayerSideByPriority = null)
			: base(isPlayerGeneral, isPlayerSergeant, isPlayerInArmy, charactersInPlayerSideByPriority)
		{
		}

		// Token: 0x0600097F RID: 2431 RVA: 0x000443A5 File Offset: 0x000425A5
		public override void OnPlayerChoiceMade(int chosenIndex)
		{
			Debug.FailedAssert("Player cannot make a choice in naval battles as its decision is fixed by design", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\Missions\\NavalAssignPlayerRoleInTeamMissionController.cs", "OnPlayerChoiceMade", 24);
		}

		// Token: 0x06000980 RID: 2432 RVA: 0x000443C0 File Offset: 0x000425C0
		public override void OnPlayerTeamDeployed()
		{
			this._navalShipsLogic = base.Mission.GetMissionBehavior<NavalShipsLogic>();
			this._navalAgentsLogic = base.Mission.GetMissionBehavior<NavalAgentsLogic>();
			base.PlayerChosenIndex = 0;
			if (MissionGameModels.Current.BattleInitializationModel.CanPlayerSideDeployWithOrderOfBattle())
			{
				Team playerTeam = Mission.Current.PlayerTeam;
				this.FormationsLockedWithSergeants = new Dictionary<int, Agent>();
				this.FormationsWithLooselyChosenSergeants = new Dictionary<int, Agent>();
				if (playerTeam.IsPlayerGeneral)
				{
					this.CharacterNamesInPlayerSideByPriorityQueue = new Queue<string>();
					this.RemainingFormationsToAssignSergeantsTo = new List<Formation>();
					return;
				}
				this.CharacterNamesInPlayerSideByPriorityQueue = ((this.CharactersInPlayerSideByPriority != null) ? new Queue<string>(this.CharactersInPlayerSideByPriority) : new Queue<string>());
				this.RemainingFormationsToAssignSergeantsTo = LinQuick.WhereQ<Formation>(playerTeam.FormationsIncludingSpecialAndEmpty, (Formation f) => f.CountOfUnits > 0).ToList<Formation>();
				while (this.CharacterNamesInPlayerSideByPriorityQueue.Count > 0 && this.RemainingFormationsToAssignSergeantsTo.Count > 0)
				{
					string nextAgentNameToProcess = this.CharacterNamesInPlayerSideByPriorityQueue.Dequeue();
					Agent agent = playerTeam.ActiveAgents.FirstOrDefault<Agent>((Agent aa) => aa.Character.StringId.Equals(nextAgentNameToProcess));
					if (agent != null)
					{
						Formation formation = this.RemainingFormationsToAssignSergeantsTo[0];
						this.FormationsLockedWithSergeants.Add(formation.Index, agent);
						this.RemainingFormationsToAssignSergeantsTo.Remove(formation);
					}
				}
			}
		}

		// Token: 0x06000981 RID: 2433 RVA: 0x0004451C File Offset: 0x0004271C
		protected override void AssignSergeant(Formation formationToLead, Agent sergeant)
		{
			MissionShip missionShip;
			this._navalShipsLogic.GetShip(formationToLead, out missionShip);
			if (formationToLead.Captain != sergeant)
			{
				this._navalAgentsLogic.AssignCaptainToShipForDeploymentMode(sergeant, missionShip, null);
			}
			if (!sergeant.IsAIControlled || sergeant == Agent.Main)
			{
				formationToLead.PlayerOwner = sergeant;
			}
		}

		// Token: 0x04000590 RID: 1424
		private NavalShipsLogic _navalShipsLogic;

		// Token: 0x04000591 RID: 1425
		private NavalAgentsLogic _navalAgentsLogic;
	}
}
