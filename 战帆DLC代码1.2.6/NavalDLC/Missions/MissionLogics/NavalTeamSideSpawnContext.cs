using System;
using System.Linq;
using System.Runtime.CompilerServices;
using NavalDLC.Missions.Objects;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.MissionLogics
{
	// Token: 0x020000D6 RID: 214
	internal class NavalTeamSideSpawnContext
	{
		// Token: 0x170002F6 RID: 758
		// (get) Token: 0x060010EA RID: 4330 RVA: 0x0007E4A8 File Offset: 0x0007C6A8
		// (set) Token: 0x060010EB RID: 4331 RVA: 0x0007E4B0 File Offset: 0x0007C6B0
		public BattleSideEnum BattleSide { get; private set; }

		// Token: 0x170002F7 RID: 759
		// (get) Token: 0x060010EC RID: 4332 RVA: 0x0007E4B9 File Offset: 0x0007C6B9
		public TeamSideEnum TeamSide { get; }

		// Token: 0x170002F8 RID: 760
		// (get) Token: 0x060010ED RID: 4333 RVA: 0x0007E4C1 File Offset: 0x0007C6C1
		// (set) Token: 0x060010EE RID: 4334 RVA: 0x0007E4C9 File Offset: 0x0007C6C9
		public bool TroopSpawningActive
		{
			get
			{
				return this._troopSpawningActive;
			}
			private set
			{
				this._troopSpawningActive = value;
				if (this._agentsLogic.IsDeploymentFinished)
				{
					this._agentsLogic.SetSpawnReinforcementsOnTick(this._troopSpawningActive, true);
				}
			}
		}

		// Token: 0x170002F9 RID: 761
		// (get) Token: 0x060010EF RID: 4335 RVA: 0x0007E4F1 File Offset: 0x0007C6F1
		// (set) Token: 0x060010F0 RID: 4336 RVA: 0x0007E4F9 File Offset: 0x0007C6F9
		public bool IsInitialSpawnOver { get; private set; }

		// Token: 0x170002FA RID: 762
		// (get) Token: 0x060010F1 RID: 4337 RVA: 0x0007E502 File Offset: 0x0007C702
		// (set) Token: 0x060010F2 RID: 4338 RVA: 0x0007E50A File Offset: 0x0007C70A
		public int TotalSpawnNumber { get; private set; }

		// Token: 0x060010F3 RID: 4339 RVA: 0x0007E514 File Offset: 0x0007C714
		public NavalTeamSideSpawnContext(Mission mission, INavalMissionAgentSpawnLogic agentSpawnLogic, BattleSideEnum battleSide, TeamSideEnum teamSide, MBList<IAgentOriginBase> troopOrigins)
		{
			this.TotalSpawnNumber = troopOrigins.Count;
			this._mission = mission;
			this._agentSpawnLogic = agentSpawnLogic;
			this.BattleSide = battleSide;
			this.TeamSide = teamSide;
			this._agentsLogic = this._mission.GetMissionBehavior<NavalAgentsLogic>();
			this._shipsLogic = this._mission.GetMissionBehavior<NavalShipsLogic>();
			this._agentsLogic.SetSpawnReinforcementsOnTick(teamSide, this.TroopSpawningActive, true);
			this._agentsLogic.AddTroopOrigins(teamSide, troopOrigins);
			this._agentsLogic.SetRestrictRecentlySwappedAgentTransfers(teamSide, true);
			this._pendingCaptainAssignments = new MBQueue<ValueTuple<Formation, IAgentOriginBase>>();
		}

		// Token: 0x060010F4 RID: 4340 RVA: 0x0007E5AF File Offset: 0x0007C7AF
		public void OnDeploymentFinished()
		{
			this._agentsLogic.SetRestrictRecentlySwappedAgentTransfers(this.TeamSide, false);
			this._agentsLogic.SetSpawnReinforcementsOnTick(this.TroopSpawningActive, true);
		}

		// Token: 0x060010F5 RID: 4341 RVA: 0x0007E5D8 File Offset: 0x0007C7D8
		public void OnDeploymentTick(float dt)
		{
			this._agentsLogic.ClearRecentlySwappedAgentsData(this.TeamSide);
			if (this._updateShipsOnNextTick)
			{
				this._updateShipsOnNextTick = false;
				this._agentsLogic.AssignTroops(this.TeamSide, false);
				this._agentsLogic.InitializeReinforcementTimers(this.TeamSide, true, true);
				this.ReassignPendingCaptains();
				this.CheckSpawnNextBatch();
				this._agentsLogic.AssignAndTeleportCrewToShipMachines(this.TeamSide);
				if (this.TeamSide == null)
				{
					this._agentSpawnLogic.OnPlayerShipsUpdated();
				}
			}
		}

		// Token: 0x060010F6 RID: 4342 RVA: 0x0007E65C File Offset: 0x0007C85C
		public void AllocateAndDeployInitialTroops(Mission mission)
		{
			this._agentsLogic.AutoComputeDesiredTroopCountsPerShip(this.TeamSide, true);
			if (this.TeamSide == null)
			{
				this.AllocateAndDeployInitialTroopsOfPlayerTeam();
			}
			else
			{
				this.AllocateAndDeployInitialTroopsOfTeam();
			}
			this._agentsLogic.AssignAndTeleportCrewToShipMachines(this.TeamSide);
			this.IsInitialSpawnOver = true;
		}

		// Token: 0x060010F7 RID: 4343 RVA: 0x0007E6A9 File Offset: 0x0007C8A9
		public void UpdateShips()
		{
			this._agentsLogic.AutoComputeDesiredTroopCountsPerShip(this.TeamSide, true);
			this._agentsLogic.UnassignTroops(this.TeamSide);
			this._updateShipsOnNextTick = true;
		}

		// Token: 0x060010F8 RID: 4344 RVA: 0x0007E6D5 File Offset: 0x0007C8D5
		public void SetSpawnTroops(bool spawnTroops, bool enforceSpawn = false)
		{
			this.TroopSpawningActive = spawnTroops;
			if (enforceSpawn)
			{
				this.CheckSpawnNextBatch();
			}
		}

		// Token: 0x060010F9 RID: 4345 RVA: 0x0007E6E8 File Offset: 0x0007C8E8
		public bool HasPendingCaptainAssignment(Formation formation)
		{
			return this._pendingCaptainAssignments.Any<ValueTuple<Formation, IAgentOriginBase>>(([TupleElementNames(new string[] { "formation", "captainOrigin" })] ValueTuple<Formation, IAgentOriginBase> pca) => pca.Item1 == formation);
		}

		// Token: 0x060010FA RID: 4346 RVA: 0x0007E71C File Offset: 0x0007C91C
		private void ReassignPendingCaptains()
		{
			while (!Extensions.IsEmpty<ValueTuple<Formation, IAgentOriginBase>>(this._pendingCaptainAssignments))
			{
				ValueTuple<Formation, IAgentOriginBase> valueTuple = this._pendingCaptainAssignments.Dequeue();
				IAgentOriginBase item = valueTuple.Item2;
				Formation item2 = valueTuple.Item1;
				MissionShip missionShip;
				if (this._shipsLogic.GetShip(item2, out missionShip))
				{
					Agent agent;
					MissionShip missionShip2;
					if (!this._agentsLogic.IsAgentOnAnyShip(item, out agent, out missionShip2, this.TeamSide))
					{
						this._agentsLogic.SpawnExistingHero(item, missionShip, out agent);
						missionShip2 = missionShip;
					}
					this._agentsLogic.AssignCaptainToShipForDeploymentMode(agent, missionShip, missionShip2);
				}
			}
		}

		// Token: 0x060010FB RID: 4347 RVA: 0x0007E79C File Offset: 0x0007C99C
		public void OnBeforeShipRemoved(MissionShip ship)
		{
			if (!this._shipsLogic.IsMissionEnding && this._agentSpawnLogic.ReassignCaptainsOfRemovedShips && ship.Captain != null)
			{
				this._pendingCaptainAssignments.Enqueue(new ValueTuple<Formation, IAgentOriginBase>(ship.Formation, ship.Captain.Origin));
			}
		}

		// Token: 0x060010FC RID: 4348 RVA: 0x0007E7EC File Offset: 0x0007C9EC
		private void AllocateAndDeployInitialTroopsOfPlayerTeam()
		{
			IAgentOriginBase agentOriginBase = this._agentsLogic.FindTroopOrigin(this.TeamSide, (IAgentOriginBase origin) => origin.Troop.IsPlayerCharacter);
			MissionShip missionShip = this._shipsLogic.GetShipAssignment(0, 0).MissionShip;
			this._agentsLogic.AddReservedTroopToShip(agentOriginBase, missionShip);
			this._agentsLogic.AssignTroops(this.TeamSide, false);
			this._agentsLogic.InitializeReinforcementTimers(this.TeamSide, true, true);
			this.CheckSpawnNextBatch();
			Agent agent2 = this._agentsLogic.GetActiveHeroesOfShip(missionShip).FirstOrDefault<Agent>((Agent agent) => agent.IsPlayerTroop);
			if (missionShip.Captain != agent2)
			{
				this._agentsLogic.AssignCaptainToShipForDeploymentMode(agent2, missionShip, missionShip);
			}
		}

		// Token: 0x060010FD RID: 4349 RVA: 0x0007E8BF File Offset: 0x0007CABF
		private void AllocateAndDeployInitialTroopsOfTeam()
		{
			this._agentsLogic.AssignTroops(this.TeamSide, false);
			this._agentsLogic.InitializeReinforcementTimers(this.TeamSide, true, true);
			this.CheckSpawnNextBatch();
		}

		// Token: 0x060010FE RID: 4350 RVA: 0x0007E8F0 File Offset: 0x0007CAF0
		private int CheckSpawnNextBatch()
		{
			int num = 0;
			if (this.TroopSpawningActive)
			{
				num += this._agentsLogic.SpawnNextBatch(this.TeamSide, false, null);
			}
			return num;
		}

		// Token: 0x040009D7 RID: 2519
		private readonly INavalMissionAgentSpawnLogic _agentSpawnLogic;

		// Token: 0x040009D8 RID: 2520
		private readonly Mission _mission;

		// Token: 0x040009D9 RID: 2521
		private readonly NavalShipsLogic _shipsLogic;

		// Token: 0x040009DA RID: 2522
		private readonly NavalAgentsLogic _agentsLogic;

		// Token: 0x040009DB RID: 2523
		[TupleElementNames(new string[] { "formation", "captainOrigin" })]
		private readonly MBQueue<ValueTuple<Formation, IAgentOriginBase>> _pendingCaptainAssignments;

		// Token: 0x040009DC RID: 2524
		private bool _updateShipsOnNextTick;

		// Token: 0x040009DD RID: 2525
		private bool _troopSpawningActive;
	}
}
