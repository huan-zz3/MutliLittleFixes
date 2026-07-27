using System;
using System.Collections.Generic;
using NavalDLC.Missions.Handlers;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using NavalDLC.Missions.ShipControl;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.Deployment
{
	// Token: 0x020000DB RID: 219
	public class NavalDeploymentMissionController : DeploymentMissionController
	{
		// Token: 0x14000029 RID: 41
		// (add) Token: 0x06001120 RID: 4384 RVA: 0x0007FE60 File Offset: 0x0007E060
		// (remove) Token: 0x06001121 RID: 4385 RVA: 0x0007FE98 File Offset: 0x0007E098
		public event Action PlayerShipsUpdated;

		// Token: 0x06001122 RID: 4386 RVA: 0x0007FECD File Offset: 0x0007E0CD
		public NavalDeploymentMissionController(bool isPlayerAttacker)
			: base(isPlayerAttacker)
		{
		}

		// Token: 0x06001123 RID: 4387 RVA: 0x0007FED8 File Offset: 0x0007E0D8
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._navalMissionLogic = base.Mission.GetMissionBehavior<DefaultNavalMissionLogic>();
			this._navalAgentsLogic = base.Mission.GetMissionBehavior<NavalAgentsLogic>();
			this._navalShipsLogic = base.Mission.GetMissionBehavior<NavalShipsLogic>();
			this._shipAgentSpawnLogic = base.Mission.GetMissionBehavior<DefaultNavalMissionAgentSpawnLogic>();
			this._shipAgentSpawnLogic.PlayerShipsUpdated += this.OnPlayerShipsUpdated;
			this._navalDeploymentHandler = base.Mission.GetMissionBehavior<NavalDeploymentHandler>();
		}

		// Token: 0x06001124 RID: 4388 RVA: 0x0007FF58 File Offset: 0x0007E158
		protected override void OnAfterStart()
		{
			for (int i = 0; i < 2; i++)
			{
				this._shipAgentSpawnLogic.SetSpawnTroops(i, false, false);
			}
		}

		// Token: 0x06001125 RID: 4389 RVA: 0x0007FF7F File Offset: 0x0007E17F
		public override void OnMissionStateFinalized()
		{
			this._shipAgentSpawnLogic.PlayerShipsUpdated -= this.OnPlayerShipsUpdated;
		}

		// Token: 0x06001126 RID: 4390 RVA: 0x0007FF98 File Offset: 0x0007E198
		public bool TryAssignShipToFormation(IShipOrigin shipOrigin, Formation formation, bool updateShips = true)
		{
			ShipAssignment shipAssignment = null;
			bool flag = shipOrigin != null && this._navalShipsLogic.FindAssignmentOfShipOrigin(shipOrigin, out shipAssignment);
			if (flag && shipAssignment.Formation == formation)
			{
				return false;
			}
			bool flag2 = this._navalShipsLogic.IsAShipAssignedToFormation(formation);
			if (shipOrigin == null && !flag2)
			{
				return false;
			}
			if (flag2)
			{
				this._navalShipsLogic.RemoveShip(formation);
			}
			if (shipOrigin != null)
			{
				if (flag)
				{
					this._navalShipsLogic.TransferShipToFormation(shipOrigin, shipAssignment.Formation, formation);
				}
				else
				{
					NavalShipsLogic navalShipsLogic = this._navalShipsLogic;
					MatrixFrame zero = MatrixFrame.Zero;
					navalShipsLogic.SpawnShip(shipOrigin, in zero, formation.Team, formation, true, 8, true).SetController(ShipControllerType.None, true);
				}
			}
			if (updateShips)
			{
				this.UpdateShips(formation.Team.TeamSide);
			}
			return true;
		}

		// Token: 0x06001127 RID: 4391 RVA: 0x00080043 File Offset: 0x0007E243
		public void UpdateShips(TeamSideEnum teamSide)
		{
			this._shipAgentSpawnLogic.UpdateShips(teamSide);
		}

		// Token: 0x06001128 RID: 4392 RVA: 0x00080051 File Offset: 0x0007E251
		public bool IsShipAssignedToFormation(Formation formation)
		{
			return this._navalShipsLogic.IsAShipAssignedToFormation(formation);
		}

		// Token: 0x06001129 RID: 4393 RVA: 0x00080060 File Offset: 0x0007E260
		public bool TryAssignCaptainToFormation(IAgentOriginBase captainOrigin, Formation formation)
		{
			MissionShip missionShip;
			this._navalShipsLogic.GetShip(formation, out missionShip);
			if (captainOrigin != null)
			{
				Agent agent;
				MissionShip missionShip2;
				bool flag = this._navalAgentsLogic.IsAgentOnAnyShip(captainOrigin, out agent, out missionShip2, formation.Team.TeamSide);
				if (flag && formation.Captain == agent)
				{
					return false;
				}
				if (!flag)
				{
					this._navalAgentsLogic.SpawnExistingHero(captainOrigin, missionShip, out agent);
				}
				this._navalAgentsLogic.AssignCaptainToShipForDeploymentMode(agent, missionShip, missionShip2);
				return true;
			}
			else
			{
				if (formation.Captain == null)
				{
					return false;
				}
				this._navalAgentsLogic.UnassignCaptainOfShipForDeploymentMode(missionShip);
				return true;
			}
		}

		// Token: 0x0600112A RID: 4394 RVA: 0x000800E4 File Offset: 0x0007E2E4
		public bool SetTroopClassFilter(TroopTraitsMask troopClassFilter, Formation targetFormation, bool updateShips)
		{
			MissionShip missionShip;
			this._navalShipsLogic.GetShip(targetFormation, out missionShip);
			this._navalAgentsLogic.SetTroopClassFilter(missionShip, troopClassFilter);
			if (updateShips)
			{
				this.UpdateShips(targetFormation.Team.TeamSide);
			}
			return updateShips;
		}

		// Token: 0x0600112B RID: 4395 RVA: 0x00080124 File Offset: 0x0007E324
		public bool SetTroopTraitsFilter(TroopTraitsMask troopTraitsFilter, Formation targetFormation, bool updateShips)
		{
			MissionShip missionShip;
			this._navalShipsLogic.GetShip(targetFormation, out missionShip);
			this._navalAgentsLogic.SetTroopTraitsFilter(missionShip, troopTraitsFilter);
			if (updateShips)
			{
				this.UpdateShips(targetFormation.Team.TeamSide);
			}
			return updateShips;
		}

		// Token: 0x0600112C RID: 4396 RVA: 0x00080162 File Offset: 0x0007E362
		public IReadOnlyCollection<IAgentOriginBase> GetAllPlayerTeamHeroes()
		{
			return this._navalAgentsLogic.GetTeamHeroOrigins(0);
		}

		// Token: 0x0600112D RID: 4397 RVA: 0x00080170 File Offset: 0x0007E370
		public MBReadOnlyList<IShipOrigin> GetAllPlayerShips()
		{
			return this._navalMissionLogic.PlayerShips;
		}

		// Token: 0x0600112E RID: 4398 RVA: 0x0008017D File Offset: 0x0007E37D
		public MBReadOnlyList<Formation> GetUsableFormations()
		{
			return base.Mission.PlayerTeam.FormationsIncludingEmpty;
		}

		// Token: 0x0600112F RID: 4399 RVA: 0x0008018F File Offset: 0x0007E38F
		protected override void OnSetupTeamsOfSide(BattleSideEnum battleSide)
		{
			this._navalMissionLogic.DeployBattleSide(battleSide);
			this._shipAgentSpawnLogic.AllocateAndDeployInitialTroops(battleSide);
			base.SetupAgentAIStatesForSide(battleSide);
			this._shipAgentSpawnLogic.OnSideDeploymentOver(battleSide);
		}

		// Token: 0x06001130 RID: 4400 RVA: 0x000801BC File Offset: 0x0007E3BC
		protected override void OnSetupTeamsFinished()
		{
			this._navalShipsLogic.SetTeleportShips(true);
		}

		// Token: 0x06001131 RID: 4401 RVA: 0x000801CC File Offset: 0x0007E3CC
		protected override void SetupAIOfEnemySide(BattleSideEnum enemySide)
		{
			Team team = ((enemySide == 1) ? base.Mission.AttackerTeam : base.Mission.DefenderTeam);
			this.SetupAIOfEnemyTeam(team);
			Team team2 = ((enemySide == 1) ? base.Mission.AttackerAllyTeam : base.Mission.DefenderAllyTeam);
			if (team2 != null)
			{
				this.SetupAIOfEnemyTeam(team2);
			}
		}

		// Token: 0x06001132 RID: 4402 RVA: 0x00080224 File Offset: 0x0007E424
		protected override void SetupAIOfEnemyTeam(Team team)
		{
			foreach (Formation formation in team.FormationsIncludingEmpty)
			{
				if (formation.CountOfUnits > 0)
				{
					formation.SetControlledByAI(true, false);
				}
			}
			team.QuerySystem.Expire();
			base.Mission.AllowAiTicking = true;
			base.Mission.ForceTickOccasionally = true;
			team.ResetTactic();
			base.Mission.AllowAiTicking = false;
			base.Mission.ForceTickOccasionally = false;
		}

		// Token: 0x06001133 RID: 4403 RVA: 0x000802C4 File Offset: 0x0007E4C4
		protected override void BeforeDeploymentFinished()
		{
			this._navalShipsLogic.SetTeleportShips(false);
		}

		// Token: 0x06001134 RID: 4404 RVA: 0x000802D2 File Offset: 0x0007E4D2
		protected override void AfterDeploymentFinished()
		{
			base.Mission.RemoveMissionBehavior(this._navalDeploymentHandler);
		}

		// Token: 0x06001135 RID: 4405 RVA: 0x000802E5 File Offset: 0x0007E4E5
		internal void OnPlayerShipsUpdated()
		{
			Action playerShipsUpdated = this.PlayerShipsUpdated;
			if (playerShipsUpdated == null)
			{
				return;
			}
			playerShipsUpdated();
		}

		// Token: 0x040009F1 RID: 2545
		private DefaultNavalMissionAgentSpawnLogic _shipAgentSpawnLogic;

		// Token: 0x040009F2 RID: 2546
		private NavalShipsLogic _navalShipsLogic;

		// Token: 0x040009F3 RID: 2547
		private NavalAgentsLogic _navalAgentsLogic;

		// Token: 0x040009F4 RID: 2548
		private DefaultNavalMissionLogic _navalMissionLogic;

		// Token: 0x040009F5 RID: 2549
		private NavalDeploymentHandler _navalDeploymentHandler;
	}
}
