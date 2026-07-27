using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using NavalDLC.Missions.AI.TeamAI;
using NavalDLC.Missions.Objects;
using SandBox.Missions.MissionLogics;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.MissionLogics
{
	// Token: 0x020000C7 RID: 199
	public class DefaultNavalMissionAgentSpawnLogic : MissionLogic, IMissionAgentSpawnLogic, IMissionBehavior, INavalMissionAgentSpawnLogic
	{
		// Token: 0x14000007 RID: 7
		// (add) Token: 0x06000ECD RID: 3789 RVA: 0x000739CC File Offset: 0x00071BCC
		// (remove) Token: 0x06000ECE RID: 3790 RVA: 0x00073A04 File Offset: 0x00071C04
		public event Action PlayerShipsUpdated;

		// Token: 0x170002B6 RID: 694
		// (get) Token: 0x06000ECF RID: 3791 RVA: 0x00073A39 File Offset: 0x00071C39
		// (set) Token: 0x06000ED0 RID: 3792 RVA: 0x00073A41 File Offset: 0x00071C41
		public BattleSideEnum PlayerSide { get; private set; }

		// Token: 0x170002B7 RID: 695
		// (get) Token: 0x06000ED1 RID: 3793 RVA: 0x00073A4A File Offset: 0x00071C4A
		// (set) Token: 0x06000ED2 RID: 3794 RVA: 0x00073A52 File Offset: 0x00071C52
		public int DeployablePlayerShipCount { get; private set; }

		// Token: 0x170002B8 RID: 696
		// (get) Token: 0x06000ED3 RID: 3795 RVA: 0x00073A5B File Offset: 0x00071C5B
		public bool ReassignCaptainsOfRemovedShips
		{
			get
			{
				return this._setReassignCaptainsOfRemovedShips;
			}
		}

		// Token: 0x06000ED4 RID: 3796 RVA: 0x00073A64 File Offset: 0x00071C64
		public DefaultNavalMissionAgentSpawnLogic(IMissionTroopSupplier[] suppliers, BattleSideEnum playerSide, int deployablePlayerShipCount = 0, int[] maxDeployableTroopCountPerTeam = null)
		{
			this.PlayerSide = playerSide;
			this._missionTeamSides = new MBList<NavalTeamSideSpawnContext>();
			int num = 3;
			this.DeployablePlayerShipCount = deployablePlayerShipCount;
			this._maxDeployableTroopCountPerTeam = new int[num];
			if (maxDeployableTroopCountPerTeam == null)
			{
				for (int i = 0; i < num; i++)
				{
					this._maxDeployableTroopCountPerTeam[i] = int.MaxValue;
				}
			}
			else
			{
				for (int j = 0; j < num; j++)
				{
					int num2 = maxDeployableTroopCountPerTeam[j];
					this._maxDeployableTroopCountPerTeam[j] = num2;
				}
			}
			this._battleSideTroopSuppliers = suppliers;
			this._playerSide = playerSide;
		}

		// Token: 0x06000ED5 RID: 3797 RVA: 0x00073AEC File Offset: 0x00071CEC
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._agentsLogic = base.Mission.GetMissionBehavior<NavalAgentsLogic>();
			this._agentsLogic.SetDeploymentMode(true);
			this._shipsLogic = base.Mission.GetMissionBehavior<NavalShipsLogic>();
			this._shipsLogic.BeforeShipRemovedEvent += this.OnBeforeShipRemoved;
			MissionGameModels.Current.BattleInitializationModel.InitializeModel();
		}

		// Token: 0x06000ED6 RID: 3798 RVA: 0x00073B53 File Offset: 0x00071D53
		public override void OnMissionStateFinalized()
		{
			this._shipsLogic.BeforeShipRemovedEvent -= this.OnBeforeShipRemoved;
		}

		// Token: 0x06000ED7 RID: 3799 RVA: 0x00073B6C File Offset: 0x00071D6C
		public override void EarlyStart()
		{
			base.EarlyStart();
			this.InitializeMissionTeamSides();
		}

		// Token: 0x06000ED8 RID: 3800 RVA: 0x00073B7C File Offset: 0x00071D7C
		public override void OnDeploymentFinished()
		{
			foreach (NavalTeamSideSpawnContext navalTeamSideSpawnContext in this._missionTeamSides)
			{
				navalTeamSideSpawnContext.OnDeploymentFinished();
			}
			this._agentsLogic.SetIgnoreTroopCapacities(true);
			this._agentsLogic.SetDeploymentMode(false);
			BattleAgentLogic missionBehavior = base.Mission.GetMissionBehavior<BattleAgentLogic>();
			foreach (MissionShip missionShip in this._shipsLogic.AllShips)
			{
				foreach (Agent agent in this._agentsLogic.GetActiveAgentsOfShip(missionShip))
				{
					if (missionBehavior != null)
					{
						missionBehavior.OnAgentBuild(agent, null);
					}
				}
			}
		}

		// Token: 0x06000ED9 RID: 3801 RVA: 0x00073C80 File Offset: 0x00071E80
		public override void OnMissionTick(float dt)
		{
			if (!base.Mission.IsDeploymentFinished)
			{
				foreach (NavalTeamSideSpawnContext navalTeamSideSpawnContext in this._missionTeamSides)
				{
					navalTeamSideSpawnContext.OnDeploymentTick(dt);
				}
			}
		}

		// Token: 0x06000EDA RID: 3802 RVA: 0x00073CE0 File Offset: 0x00071EE0
		public void StartSpawner(BattleSideEnum side)
		{
			foreach (NavalTeamSideSpawnContext navalTeamSideSpawnContext in this._missionTeamSides)
			{
				if (navalTeamSideSpawnContext.BattleSide == side)
				{
					navalTeamSideSpawnContext.SetSpawnTroops(true, false);
				}
			}
		}

		// Token: 0x06000EDB RID: 3803 RVA: 0x00073D40 File Offset: 0x00071F40
		public void StopSpawner(BattleSideEnum side)
		{
			foreach (NavalTeamSideSpawnContext navalTeamSideSpawnContext in this._missionTeamSides)
			{
				if (navalTeamSideSpawnContext.BattleSide == side)
				{
					navalTeamSideSpawnContext.SetSpawnTroops(false, false);
				}
			}
		}

		// Token: 0x06000EDC RID: 3804 RVA: 0x00073DA0 File Offset: 0x00071FA0
		public bool IsSideSpawnEnabled(BattleSideEnum side)
		{
			bool flag = false;
			foreach (NavalTeamSideSpawnContext navalTeamSideSpawnContext in this._missionTeamSides)
			{
				if (navalTeamSideSpawnContext.BattleSide == side)
				{
					flag = flag || navalTeamSideSpawnContext.TroopSpawningActive;
				}
			}
			return flag;
		}

		// Token: 0x06000EDD RID: 3805 RVA: 0x00073E08 File Offset: 0x00072008
		public bool IsSideDepleted(BattleSideEnum side)
		{
			int num = 0;
			foreach (Team team in base.Mission.Teams)
			{
				if (team.Side == side)
				{
					num += team.ActiveAgents.Count;
				}
			}
			num += this._agentsLogic.GetNumberOfReservedTroops(side, true);
			return num == 0;
		}

		// Token: 0x06000EDE RID: 3806 RVA: 0x00073E88 File Offset: 0x00072088
		public float GetReinforcementInterval(BattleSideEnum side = -1)
		{
			return 0f;
		}

		// Token: 0x06000EDF RID: 3807 RVA: 0x00073E8F File Offset: 0x0007208F
		public int GetNumberOfPlayerControllableTroops()
		{
			return this._numTroopsControllableByPlayer;
		}

		// Token: 0x06000EE0 RID: 3808 RVA: 0x00073E98 File Offset: 0x00072098
		public IEnumerable<IAgentOriginBase> GetAllTroopsForSide(BattleSideEnum side)
		{
			return this._battleSideTroopSuppliers[side].GetAllTroops();
		}

		// Token: 0x06000EE1 RID: 3809 RVA: 0x00073EB4 File Offset: 0x000720B4
		public bool GetSpawnHorses(BattleSideEnum side)
		{
			return false;
		}

		// Token: 0x06000EE2 RID: 3810 RVA: 0x00073EB7 File Offset: 0x000720B7
		public void OnPlayerShipsUpdated()
		{
			Action playerShipsUpdated = this.PlayerShipsUpdated;
			if (playerShipsUpdated == null)
			{
				return;
			}
			playerShipsUpdated();
		}

		// Token: 0x06000EE3 RID: 3811 RVA: 0x00073EC9 File Offset: 0x000720C9
		public void SetReassignCaptainsOfRemovedShips(bool value)
		{
			this._setReassignCaptainsOfRemovedShips = value;
		}

		// Token: 0x06000EE4 RID: 3812 RVA: 0x00073ED4 File Offset: 0x000720D4
		internal void AllocateAndDeployInitialTroops(BattleSideEnum battleSide)
		{
			this.SetSpawnTroops(battleSide, true, false);
			foreach (NavalTeamSideSpawnContext navalTeamSideSpawnContext in this._missionTeamSides)
			{
				if (navalTeamSideSpawnContext.BattleSide == battleSide)
				{
					navalTeamSideSpawnContext.AllocateAndDeployInitialTroops(base.Mission);
				}
			}
		}

		// Token: 0x06000EE5 RID: 3813 RVA: 0x00073F40 File Offset: 0x00072140
		internal void UpdateShips(TeamSideEnum teamSide)
		{
			NavalTeamSideSpawnContext navalTeamSideSpawnContext;
			this.GetMissionTeamSide(teamSide, out navalTeamSideSpawnContext);
			navalTeamSideSpawnContext.UpdateShips();
		}

		// Token: 0x06000EE6 RID: 3814 RVA: 0x00073F60 File Offset: 0x00072160
		internal void SetSpawnTroops(BattleSideEnum battleSide, bool spawnTroops, bool enforceSpawning = false)
		{
			foreach (NavalTeamSideSpawnContext navalTeamSideSpawnContext in this._missionTeamSides)
			{
				if (navalTeamSideSpawnContext.BattleSide == battleSide)
				{
					navalTeamSideSpawnContext.SetSpawnTroops(spawnTroops, enforceSpawning);
				}
			}
		}

		// Token: 0x06000EE7 RID: 3815 RVA: 0x00073FC0 File Offset: 0x000721C0
		internal bool HasPendingCaptainAssignment(Formation formation)
		{
			NavalTeamSideSpawnContext navalTeamSideSpawnContext;
			this.GetMissionTeamSide(formation.Team.TeamSide, out navalTeamSideSpawnContext);
			return navalTeamSideSpawnContext.HasPendingCaptainAssignment(formation);
		}

		// Token: 0x06000EE8 RID: 3816 RVA: 0x00073FE8 File Offset: 0x000721E8
		internal void OnSideDeploymentOver(BattleSideEnum side)
		{
			IEnumerable<Team> teamsOfSide = Mission.GetTeamsOfSide(side);
			foreach (Team team in teamsOfSide)
			{
				base.Mission.OnTeamDeployed(team);
			}
			base.Mission.OnBattleSideDeployed(side);
			foreach (Team team2 in teamsOfSide)
			{
				foreach (Formation formation in team2.FormationsIncludingEmpty)
				{
					if (formation.CountOfUnits > 0)
					{
						formation.QuerySystem.EvaluateAllPreliminaryQueryData();
					}
				}
			}
			Team playerTeam = base.Mission.PlayerTeam;
			Formation formation2 = ((playerTeam != null) ? playerTeam.FormationsIncludingEmpty.FirstOrDefault<Formation>(new Func<Formation, bool>(NavalDLCHelpers.IsPlayerCaptainOfFormationShip)) : null);
			NavalOrderController navalOrderController;
			if (formation2 != null && (navalOrderController = base.Mission.PlayerTeam.PlayerOrderController as NavalOrderController) != null)
			{
				navalOrderController.SelectFormation(formation2);
				navalOrderController.SetOrder(34);
				navalOrderController.SetFormationUpdateEnabledAfterSetOrder(true);
				navalOrderController.ClearSelectedFormations();
			}
		}

		// Token: 0x06000EE9 RID: 3817 RVA: 0x0007412C File Offset: 0x0007232C
		private void InitializeMissionTeamSides()
		{
			MBList<ValueTuple<Team, MBList<IAgentOriginBase>>> mblist = new MBList<ValueTuple<Team, MBList<IAgentOriginBase>>>();
			foreach (Team team in base.Mission.Teams)
			{
				mblist.Add(new ValueTuple<Team, MBList<IAgentOriginBase>>(team, new MBList<IAgentOriginBase>()));
			}
			for (int i = 0; i < 2; i++)
			{
				IMissionTroopSupplier missionTroopSupplier = this._battleSideTroopSuppliers[i];
				BattleSideEnum battleSideEnum = i;
				bool flag = battleSideEnum == this._playerSide;
				if (flag)
				{
					this._numTroopsControllableByPlayer = missionTroopSupplier.GetNumberOfPlayerControllableTroops();
				}
				bool flag2 = true;
				while (missionTroopSupplier.AnyTroopRemainsToBeSupplied && (flag2 || DefaultNavalMissionAgentSpawnLogic.IsAnyTeamsUnfilled(battleSideEnum, mblist, this._maxDeployableTroopCountPerTeam)))
				{
					flag2 = false;
					IAgentOriginBase agentOriginBase = missionTroopSupplier.SupplyOneTroop();
					if (agentOriginBase != null)
					{
						Team troopTeam = Mission.GetAgentTeam(agentOriginBase, flag);
						MBList<IAgentOriginBase> item = mblist.FirstOrDefault<ValueTuple<Team, MBList<IAgentOriginBase>>>(([TupleElementNames(new string[] { "team", "troopOrigins" })] ValueTuple<Team, MBList<IAgentOriginBase>> tuple) => tuple.Item1 == troopTeam).Item2;
						if (item.Count < this._maxDeployableTroopCountPerTeam[troopTeam.TeamSide])
						{
							item.Add(agentOriginBase);
							flag2 = true;
						}
					}
				}
			}
			ValueTuple<Team, MBList<IAgentOriginBase>> valueTuple = mblist.FirstOrDefault<ValueTuple<Team, MBList<IAgentOriginBase>>>(([TupleElementNames(new string[] { "team", "troopOrigins" })] ValueTuple<Team, MBList<IAgentOriginBase>> tuple) => tuple.Item1.TeamSide == 0);
			this._numTroopsControllableByPlayer = MathF.Min(this._numTroopsControllableByPlayer, valueTuple.Item2.Count);
			foreach (ValueTuple<Team, MBList<IAgentOriginBase>> valueTuple2 in mblist)
			{
				BattleSideEnum side = valueTuple2.Item1.Side;
				TeamSideEnum teamSide = valueTuple2.Item1.TeamSide;
				MBList<IAgentOriginBase> item2 = valueTuple2.Item2;
				BattleSideEnum playerSide = this._playerSide;
				NavalTeamSideSpawnContext navalTeamSideSpawnContext = new NavalTeamSideSpawnContext(base.Mission, this, side, teamSide, item2);
				this._missionTeamSides.Add(navalTeamSideSpawnContext);
				item2.Clear();
			}
			mblist.Clear();
		}

		// Token: 0x06000EEA RID: 3818 RVA: 0x00074334 File Offset: 0x00072534
		private void OnBeforeShipRemoved(MissionShip ship)
		{
			NavalTeamSideSpawnContext navalTeamSideSpawnContext;
			if (ship.Team != null && this.GetMissionTeamSide(ship.Team.TeamSide, out navalTeamSideSpawnContext))
			{
				navalTeamSideSpawnContext.OnBeforeShipRemoved(ship);
			}
		}

		// Token: 0x06000EEB RID: 3819 RVA: 0x00074368 File Offset: 0x00072568
		private bool GetMissionTeamSide(TeamSideEnum teamSide, out NavalTeamSideSpawnContext missionTeamSide)
		{
			missionTeamSide = this._missionTeamSides.FirstOrDefault<NavalTeamSideSpawnContext>((NavalTeamSideSpawnContext mts) => mts.TeamSide == teamSide);
			return missionTeamSide != null;
		}

		// Token: 0x06000EEC RID: 3820 RVA: 0x000743A0 File Offset: 0x000725A0
		private static bool IsAnyTeamsUnfilled(BattleSideEnum battleSide, [TupleElementNames(new string[] { "team", "troopOrigins" })] MBList<ValueTuple<Team, MBList<IAgentOriginBase>>> troopOriginsPerTeam, int[] maxDeployableTroopCountPerTeam)
		{
			foreach (ValueTuple<Team, MBList<IAgentOriginBase>> valueTuple in troopOriginsPerTeam)
			{
				if (valueTuple.Item1.Side == battleSide)
				{
					MBList<IAgentOriginBase> item = valueTuple.Item2;
					if (((item != null) ? item.Count : 0) < maxDeployableTroopCountPerTeam[valueTuple.Item1.TeamSide])
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x04000935 RID: 2357
		private NavalAgentsLogic _agentsLogic;

		// Token: 0x04000936 RID: 2358
		private NavalShipsLogic _shipsLogic;

		// Token: 0x04000937 RID: 2359
		private readonly MBList<NavalTeamSideSpawnContext> _missionTeamSides;

		// Token: 0x04000938 RID: 2360
		private IMissionTroopSupplier[] _battleSideTroopSuppliers;

		// Token: 0x04000939 RID: 2361
		private readonly int[] _maxDeployableTroopCountPerTeam;

		// Token: 0x0400093A RID: 2362
		private BattleSideEnum _playerSide;

		// Token: 0x0400093B RID: 2363
		private int _numTroopsControllableByPlayer;

		// Token: 0x0400093C RID: 2364
		private bool _setReassignCaptainsOfRemovedShips = true;
	}
}
