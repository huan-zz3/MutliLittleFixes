using System;
using System.Collections.Generic;
using NavalDLC.Missions.Handlers;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using NavalDLC.Missions.ShipControl;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.Deployment
{
	// Token: 0x020000DF RID: 223
	public class NavalRaidDeploymentMissionController : DeploymentMissionController
	{
		// Token: 0x1400002A RID: 42
		// (add) Token: 0x06001184 RID: 4484 RVA: 0x00081044 File Offset: 0x0007F244
		// (remove) Token: 0x06001185 RID: 4485 RVA: 0x0008107C File Offset: 0x0007F27C
		public event Action PlayerShipsUpdated;

		// Token: 0x06001186 RID: 4486 RVA: 0x000810B1 File Offset: 0x0007F2B1
		public NavalRaidDeploymentMissionController(bool isPlayerAttacker)
			: base(isPlayerAttacker)
		{
		}

		// Token: 0x06001187 RID: 4487 RVA: 0x000810BC File Offset: 0x0007F2BC
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._navalRaidMissionLogic = base.Mission.GetMissionBehavior<NavalRaidMissionAgentSpawnLogic>();
			this._navalAgentsLogic = base.Mission.GetMissionBehavior<NavalAgentsLogic>();
			this._navalShipsLogic = base.Mission.GetMissionBehavior<NavalShipsLogic>();
			this._navalRaidMissionLogic.PlayerShipsUpdated += this.OnPlayerShipsUpdated;
			this._navalRaidDeploymentHandler = base.Mission.GetMissionBehavior<NavalRaidDeploymentHandler>();
		}

		// Token: 0x06001188 RID: 4488 RVA: 0x0008112A File Offset: 0x0007F32A
		public override void OnRemoveBehavior()
		{
			base.OnRemoveBehavior();
		}

		// Token: 0x06001189 RID: 4489 RVA: 0x00081134 File Offset: 0x0007F334
		protected override void OnAfterStart()
		{
			for (int i = 0; i < 2; i++)
			{
				this._navalRaidMissionLogic.SetSpawnTroops(i, false, false);
			}
			this._navalRaidMissionLogic.SetDefenderReinforcementSpawnEnabled(false, true);
		}

		// Token: 0x0600118A RID: 4490 RVA: 0x00081168 File Offset: 0x0007F368
		public override void OnMissionStateFinalized()
		{
			this._navalRaidMissionLogic.PlayerShipsUpdated -= this.OnPlayerShipsUpdated;
		}

		// Token: 0x0600118B RID: 4491 RVA: 0x00081184 File Offset: 0x0007F384
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
				this.UpdateShipsAttackerShips();
			}
			return true;
		}

		// Token: 0x0600118C RID: 4492 RVA: 0x00081224 File Offset: 0x0007F424
		public void UpdateShipsAttackerShips()
		{
			this._navalRaidMissionLogic.UpdateAttackerShips();
		}

		// Token: 0x0600118D RID: 4493 RVA: 0x00081231 File Offset: 0x0007F431
		public bool IsShipAssignedToFormation(Formation formation)
		{
			return this._navalShipsLogic.IsAShipAssignedToFormation(formation);
		}

		// Token: 0x0600118E RID: 4494 RVA: 0x00081240 File Offset: 0x0007F440
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

		// Token: 0x0600118F RID: 4495 RVA: 0x000812C4 File Offset: 0x0007F4C4
		public bool SetAttackerSideTroopClassFilter(TroopTraitsMask troopClassFilter, Formation targetFormation, bool updateShips)
		{
			MissionShip missionShip;
			this._navalShipsLogic.GetShip(targetFormation, out missionShip);
			this._navalAgentsLogic.SetTroopClassFilter(missionShip, troopClassFilter);
			if (updateShips)
			{
				this.UpdateShipsAttackerShips();
			}
			return updateShips;
		}

		// Token: 0x06001190 RID: 4496 RVA: 0x000812F8 File Offset: 0x0007F4F8
		public bool SetAttackerSideTroopTraitsFilter(TroopTraitsMask troopTraitsFilter, Formation targetFormation, bool updateShips)
		{
			MissionShip missionShip;
			this._navalShipsLogic.GetShip(targetFormation, out missionShip);
			this._navalAgentsLogic.SetTroopTraitsFilter(missionShip, troopTraitsFilter);
			if (updateShips)
			{
				this.UpdateShipsAttackerShips();
			}
			return updateShips;
		}

		// Token: 0x06001191 RID: 4497 RVA: 0x0008132B File Offset: 0x0007F52B
		public IReadOnlyCollection<IAgentOriginBase> GetAllPlayerTeamHeroes()
		{
			return this._navalAgentsLogic.GetTeamHeroOrigins(0);
		}

		// Token: 0x06001192 RID: 4498 RVA: 0x00081339 File Offset: 0x0007F539
		public MBReadOnlyList<IShipOrigin> GetAllPlayerShips()
		{
			return this._navalRaidMissionLogic.PlayerShips;
		}

		// Token: 0x06001193 RID: 4499 RVA: 0x00081346 File Offset: 0x0007F546
		public MBReadOnlyList<Formation> GetUsableFormations()
		{
			return base.Mission.PlayerTeam.FormationsIncludingEmpty;
		}

		// Token: 0x06001194 RID: 4500 RVA: 0x00081358 File Offset: 0x0007F558
		protected override void OnSetupTeamsOfSide(BattleSideEnum battleSide)
		{
			if (battleSide == 1)
			{
				this._navalRaidMissionLogic.DeployAttackerSideShips();
				this._navalRaidMissionLogic.DeployAttackerSideTroops();
			}
			else
			{
				this._navalRaidMissionLogic.DeployDefenderSideTroops();
			}
			this._navalRaidMissionLogic.OnSideDeploymentOver(battleSide);
			base.SetupAgentAIStatesForSide(battleSide);
		}

		// Token: 0x06001195 RID: 4501 RVA: 0x00081394 File Offset: 0x0007F594
		protected override void OnSetupTeamsFinished()
		{
			base.Mission.IsTeleportingAgents = true;
			this._navalShipsLogic.SetTeleportShips(true);
			Team defender = base.Mission.Teams.Defender;
			if (defender.GeneralAgent != null)
			{
				WorldPosition worldPosition;
				Vec2 vec;
				base.Mission.GetFormationSpawnFrame(defender, 8, false, ref worldPosition, ref vec, true);
				if (worldPosition.GetNavMesh() != UIntPtr.Zero && worldPosition.IsValid)
				{
					defender.GeneralAgent.TrySetFormationFrame(ref worldPosition, ref vec);
				}
			}
		}

		// Token: 0x06001196 RID: 4502 RVA: 0x00081414 File Offset: 0x0007F614
		protected override void SetupAIOfEnemySide(BattleSideEnum enemySide)
		{
			if (enemySide == 1)
			{
				Team attackerTeam = base.Mission.AttackerTeam;
				this.SetupAIOfEnemyTeam(attackerTeam);
				return;
			}
			Team defenderTeam = base.Mission.DefenderTeam;
			base.SetupAIOfEnemyTeam(defenderTeam);
		}

		// Token: 0x06001197 RID: 4503 RVA: 0x0008144C File Offset: 0x0007F64C
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

		// Token: 0x06001198 RID: 4504 RVA: 0x000814EC File Offset: 0x0007F6EC
		protected override void BeforeDeploymentFinished()
		{
			base.Mission.IsTeleportingAgents = false;
			this._navalShipsLogic.SetTeleportShips(false);
		}

		// Token: 0x06001199 RID: 4505 RVA: 0x00081506 File Offset: 0x0007F706
		protected override void AfterDeploymentFinished()
		{
			this._navalRaidMissionLogic.SetDefenderReinforcementSpawnEnabled(true, true);
			base.Mission.RemoveMissionBehavior(this._navalRaidDeploymentHandler);
		}

		// Token: 0x0600119A RID: 4506 RVA: 0x00081526 File Offset: 0x0007F726
		internal void OnPlayerShipsUpdated()
		{
			Action playerShipsUpdated = this.PlayerShipsUpdated;
			if (playerShipsUpdated == null)
			{
				return;
			}
			playerShipsUpdated();
		}

		// Token: 0x04000A0A RID: 2570
		private NavalShipsLogic _navalShipsLogic;

		// Token: 0x04000A0B RID: 2571
		private NavalAgentsLogic _navalAgentsLogic;

		// Token: 0x04000A0C RID: 2572
		private NavalRaidMissionAgentSpawnLogic _navalRaidMissionLogic;

		// Token: 0x04000A0D RID: 2573
		private NavalRaidDeploymentHandler _navalRaidDeploymentHandler;
	}
}
