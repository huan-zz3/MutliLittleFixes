using System;
using System.Collections.Generic;
using System.Linq;
using NavalDLC.Missions.Objects;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.MissionLogics
{
	// Token: 0x020000CB RID: 203
	public class NavalAgentsLogic : MissionLogic
	{
		// Token: 0x14000008 RID: 8
		// (add) Token: 0x06000F0B RID: 3851 RVA: 0x00074EF4 File Offset: 0x000730F4
		// (remove) Token: 0x06000F0C RID: 3852 RVA: 0x00074F2C File Offset: 0x0007312C
		public event Action<IAgentOriginBase, MissionShip> TroopAddedToReserves;

		// Token: 0x14000009 RID: 9
		// (add) Token: 0x06000F0D RID: 3853 RVA: 0x00074F64 File Offset: 0x00073164
		// (remove) Token: 0x06000F0E RID: 3854 RVA: 0x00074F9C File Offset: 0x0007319C
		public event Action<IAgentOriginBase, MissionShip> TroopRemovedFromReserves;

		// Token: 0x1400000A RID: 10
		// (add) Token: 0x06000F0F RID: 3855 RVA: 0x00074FD4 File Offset: 0x000731D4
		// (remove) Token: 0x06000F10 RID: 3856 RVA: 0x0007500C File Offset: 0x0007320C
		public event Action<Agent, MissionShip> AgentAddedToShip;

		// Token: 0x1400000B RID: 11
		// (add) Token: 0x06000F11 RID: 3857 RVA: 0x00075044 File Offset: 0x00073244
		// (remove) Token: 0x06000F12 RID: 3858 RVA: 0x0007507C File Offset: 0x0007327C
		public event Action<Agent, MissionShip> AgentRemovedFromShip;

		// Token: 0x170002BE RID: 702
		// (get) Token: 0x06000F13 RID: 3859 RVA: 0x000750B1 File Offset: 0x000732B1
		// (set) Token: 0x06000F14 RID: 3860 RVA: 0x000750B9 File Offset: 0x000732B9
		public NavalShipsLogic NavalShipsLogic { get; private set; }

		// Token: 0x170002BF RID: 703
		// (get) Token: 0x06000F15 RID: 3861 RVA: 0x000750C2 File Offset: 0x000732C2
		public bool IsDeploymentMode
		{
			get
			{
				return this._isDeploymentMode;
			}
		}

		// Token: 0x170002C0 RID: 704
		// (get) Token: 0x06000F16 RID: 3862 RVA: 0x000750CA File Offset: 0x000732CA
		public bool IsDeploymentFinished
		{
			get
			{
				return base.Mission.IsDeploymentFinished;
			}
		}

		// Token: 0x170002C1 RID: 705
		// (get) Token: 0x06000F17 RID: 3863 RVA: 0x000750D7 File Offset: 0x000732D7
		public bool IsMissionEnding
		{
			get
			{
				return this.NavalShipsLogic.IsMissionEnding;
			}
		}

		// Token: 0x06000F18 RID: 3864 RVA: 0x000750E4 File Offset: 0x000732E4
		public NavalAgentsLogic()
		{
			this._teamAgentsData = new MBList<NavalTeamAgents>();
			this._ignoreTroopCapacities = new bool[3];
		}

		// Token: 0x06000F19 RID: 3865 RVA: 0x00075104 File Offset: 0x00073304
		public override void OnBehaviorInitialize()
		{
			this.NavalShipsLogic = base.Mission.GetMissionBehavior<NavalShipsLogic>();
			base.Mission.GetAgentTroopClass_Override += this.GetNavalMissionTroopClass;
			this.NavalShipsLogic.ShipSpawnedEvent += this.OnShipSpawned;
			this.NavalShipsLogic.ShipRemovedEvent += this.OnShipRemoved;
			this.NavalShipsLogic.ShipTransferredToFormationEvent += this.OnShipTransferredToFormation;
			this.NavalShipsLogic.ShipTransferredToTeamEvent += this.OnShipTransferredToTeam;
			this.NavalShipsLogic.ShipCapturedEvent += this.OnShipCaptured;
			this.NavalShipsLogic.ShipTeleportedEvent += this.OnShipTeleported;
			this.NavalShipsLogic.ShipPreparedForAbandonmentEvent += this.OnShipPreparedForAbandonment;
			this.NavalShipsLogic.MissionEndEvent += this.OnMissionEnd;
		}

		// Token: 0x06000F1A RID: 3866 RVA: 0x000751F1 File Offset: 0x000733F1
		public override void OnAgentCreated(Agent agent)
		{
			base.OnAgentCreated(agent);
			agent.AddComponent(new AgentNavalComponent(agent));
			if (agent.IsHuman)
			{
				agent.AddComponent(new AgentNavalAIComponent(agent));
			}
		}

		// Token: 0x06000F1B RID: 3867 RVA: 0x0007521C File Offset: 0x0007341C
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			NavalTeamAgents navalTeamAgents;
			if (affectedAgent.IsHuman && this.GetTeamAgents(affectedAgent.Team.TeamSide, out navalTeamAgents))
			{
				navalTeamAgents.OnAgentRemoved(affectedAgent);
			}
		}

		// Token: 0x06000F1C RID: 3868 RVA: 0x0007524D File Offset: 0x0007344D
		public override void EarlyStart()
		{
			this.UpdateTeamAgentsData();
		}

		// Token: 0x06000F1D RID: 3869 RVA: 0x00075258 File Offset: 0x00073458
		public void UpdateTeamAgentsData()
		{
			this._teamAgentsData.Clear();
			foreach (Team team in base.Mission.Teams)
			{
				this._teamAgentsData.Add(new NavalTeamAgents(this, team.Side, team.TeamSide));
			}
		}

		// Token: 0x06000F1E RID: 3870 RVA: 0x000752D4 File Offset: 0x000734D4
		public override void OnMissionTick(float dt)
		{
			foreach (NavalTeamAgents navalTeamAgents in this._teamAgentsData)
			{
				if (navalTeamAgents.SpawnReinforcementsOnTick)
				{
					navalTeamAgents.CheckSpawnReinforcements(null);
				}
			}
		}

		// Token: 0x06000F1F RID: 3871 RVA: 0x00075330 File Offset: 0x00073530
		public void SetSpawnReinforcementsOnTick(bool value, bool resetShips = true)
		{
			foreach (NavalTeamAgents navalTeamAgents in this._teamAgentsData)
			{
				navalTeamAgents.SetSpawnReinforcementsOnTick(value, resetShips);
			}
		}

		// Token: 0x06000F20 RID: 3872 RVA: 0x00075384 File Offset: 0x00073584
		private void SetSpawnReinforcementsForShip(MissionShip ship, bool value)
		{
			NavalTeamAgents navalTeamAgents;
			this.GetTeamAgents(ship.Team.TeamSide, out navalTeamAgents);
			navalTeamAgents.SetSpawnReinforcementsForShip(ship, value);
		}

		// Token: 0x06000F21 RID: 3873 RVA: 0x000753B0 File Offset: 0x000735B0
		public override void OnMissionStateFinalized()
		{
			base.Mission.GetAgentTroopClass_Override -= this.GetNavalMissionTroopClass;
			this.NavalShipsLogic.ShipSpawnedEvent -= this.OnShipSpawned;
			this.NavalShipsLogic.ShipRemovedEvent -= this.OnShipRemoved;
			this.NavalShipsLogic.ShipTransferredToFormationEvent -= this.OnShipTransferredToFormation;
			this.NavalShipsLogic.ShipTransferredToTeamEvent -= this.OnShipTransferredToTeam;
			this.NavalShipsLogic.ShipCapturedEvent -= this.OnShipCaptured;
			this.NavalShipsLogic.ShipTeleportedEvent -= this.OnShipTeleported;
			this.NavalShipsLogic.ShipPreparedForAbandonmentEvent -= this.OnShipPreparedForAbandonment;
			this.NavalShipsLogic.MissionEndEvent -= this.OnMissionEnd;
		}

		// Token: 0x06000F22 RID: 3874 RVA: 0x0007548C File Offset: 0x0007368C
		public void SetSpawnReinforcementsOnTick(TeamSideEnum teamSide, bool value, bool resetShips = true)
		{
			NavalTeamAgents navalTeamAgents;
			this.GetTeamAgents(teamSide, out navalTeamAgents);
			navalTeamAgents.SetSpawnReinforcementsOnTick(value, resetShips);
		}

		// Token: 0x06000F23 RID: 3875 RVA: 0x000754AC File Offset: 0x000736AC
		public bool GetSpawnReinforcementsOnTick(TeamSideEnum teamSide)
		{
			NavalTeamAgents navalTeamAgents;
			this.GetTeamAgents(teamSide, out navalTeamAgents);
			return navalTeamAgents.SpawnReinforcementsOnTick;
		}

		// Token: 0x06000F24 RID: 3876 RVA: 0x000754CC File Offset: 0x000736CC
		public void SetIgnoreTroopCapacities(bool value)
		{
			foreach (NavalTeamAgents navalTeamAgents in this._teamAgentsData)
			{
				this._ignoreTroopCapacities[navalTeamAgents.TeamSide] = value;
				navalTeamAgents.SetIgnoreTroopCapacities(value);
			}
		}

		// Token: 0x06000F25 RID: 3877 RVA: 0x00075530 File Offset: 0x00073730
		public void SetIgnoreTroopCapacities(MissionShip ship, bool value)
		{
			NavalTeamAgents navalTeamAgents;
			this.GetTeamAgents(ship.Team.TeamSide, out navalTeamAgents);
			navalTeamAgents.SetIgnoreTroopCapacities(ship, value);
		}

		// Token: 0x06000F26 RID: 3878 RVA: 0x0007555C File Offset: 0x0007375C
		public void SetRestrictRecentlySwappedAgentTransfers(TeamSideEnum teamSide, bool value)
		{
			NavalTeamAgents navalTeamAgents;
			this.GetTeamAgents(teamSide, out navalTeamAgents);
			navalTeamAgents.SetRestrictRecentlySwappedAgentTransfers(value);
		}

		// Token: 0x06000F27 RID: 3879 RVA: 0x0007557C File Offset: 0x0007377C
		public bool GetRestrictRecentlySwappedAgentTransfers(TeamSideEnum teamSide)
		{
			NavalTeamAgents navalTeamAgents;
			this.GetTeamAgents(teamSide, out navalTeamAgents);
			return navalTeamAgents.RestrictRecentlySwappedAgentTransfers;
		}

		// Token: 0x06000F28 RID: 3880 RVA: 0x0007559C File Offset: 0x0007379C
		public void ClearRecentlySwappedAgentsData(TeamSideEnum teamSide)
		{
			NavalTeamAgents navalTeamAgents;
			this.GetTeamAgents(teamSide, out navalTeamAgents);
			navalTeamAgents.ClearRecentlySwappedAgents();
		}

		// Token: 0x06000F29 RID: 3881 RVA: 0x000755BC File Offset: 0x000737BC
		public IAgentOriginBase FindTroopOrigin(TeamSideEnum teamSide, Predicate<IAgentOriginBase> predicate)
		{
			NavalTeamAgents navalTeamAgents;
			this.GetTeamAgents(teamSide, out navalTeamAgents);
			return navalTeamAgents.FindTroopOrigin(predicate);
		}

		// Token: 0x06000F2A RID: 3882 RVA: 0x000755DC File Offset: 0x000737DC
		public int FindTroopOrigins(TeamSideEnum teamSide, Predicate<IAgentOriginBase> predicate, ref MBList<IAgentOriginBase> foundOrigins)
		{
			NavalTeamAgents navalTeamAgents;
			this.GetTeamAgents(teamSide, out navalTeamAgents);
			return navalTeamAgents.FindTroopOrigins(predicate, ref foundOrigins);
		}

		// Token: 0x06000F2B RID: 3883 RVA: 0x000755FC File Offset: 0x000737FC
		public IReadOnlyCollection<IAgentOriginBase> GetTeamTroopOrigins(TeamSideEnum teamSide)
		{
			NavalTeamAgents navalTeamAgents;
			this.GetTeamAgents(teamSide, out navalTeamAgents);
			return navalTeamAgents.AllTroopOrigins;
		}

		// Token: 0x06000F2C RID: 3884 RVA: 0x0007561C File Offset: 0x0007381C
		public IReadOnlyCollection<IAgentOriginBase> GetTeamHeroOrigins(TeamSideEnum teamSide)
		{
			NavalTeamAgents navalTeamAgents;
			this.GetTeamAgents(teamSide, out navalTeamAgents);
			return navalTeamAgents.AllHeroOrigins;
		}

		// Token: 0x06000F2D RID: 3885 RVA: 0x0007563C File Offset: 0x0007383C
		public int GetNumberOfSpawnedAgents(BattleSideEnum side)
		{
			int num = 0;
			foreach (NavalTeamAgents navalTeamAgents in this._teamAgentsData)
			{
				if (navalTeamAgents.BattleSide == side)
				{
					num += navalTeamAgents.NumberOfSpawnedAgents;
				}
			}
			return num;
		}

		// Token: 0x06000F2E RID: 3886 RVA: 0x000756A0 File Offset: 0x000738A0
		public int GetNumberOfActiveAgents(BattleSideEnum side)
		{
			int num = 0;
			foreach (NavalTeamAgents navalTeamAgents in this._teamAgentsData)
			{
				if (navalTeamAgents.BattleSide == side)
				{
					num += navalTeamAgents.NumberOfActiveTroops;
				}
			}
			return num;
		}

		// Token: 0x06000F2F RID: 3887 RVA: 0x00075704 File Offset: 0x00073904
		internal int GetNumberOfReservedTroops(BattleSideEnum side, bool spawnableOnly = false)
		{
			int num = 0;
			foreach (NavalTeamAgents navalTeamAgents in this._teamAgentsData)
			{
				if (navalTeamAgents.BattleSide == side)
				{
					num += navalTeamAgents.GetNumberOfReservedTroops(spawnableOnly);
				}
			}
			return num;
		}

		// Token: 0x06000F30 RID: 3888 RVA: 0x00075768 File Offset: 0x00073968
		public MBReadOnlyList<Agent> GetActiveAgentsOfShip(MissionShip ship)
		{
			if (ship.Team != null)
			{
				NavalTeamAgents navalTeamAgents;
				this.GetTeamAgents(ship.Team.TeamSide, out navalTeamAgents);
				return navalTeamAgents.GetActiveAgentsOfShip(ship);
			}
			return null;
		}

		// Token: 0x06000F31 RID: 3889 RVA: 0x0007579C File Offset: 0x0007399C
		public int GetReservedTroopsCountOfShip(MissionShip ship)
		{
			if (ship.Team != null)
			{
				NavalTeamAgents navalTeamAgents;
				this.GetTeamAgents(ship.Team.TeamSide, out navalTeamAgents);
				return navalTeamAgents.GetReservedTroopsCountOfShip(ship);
			}
			return 0;
		}

		// Token: 0x06000F32 RID: 3890 RVA: 0x000757D0 File Offset: 0x000739D0
		internal int GetTotalTroopCountOfShip(MissionShip ship, bool spawnableReservesOnly = false)
		{
			if (ship.Team != null)
			{
				NavalTeamAgents navalTeamAgents;
				this.GetTeamAgents(ship.Team.TeamSide, out navalTeamAgents);
				return navalTeamAgents.GetTotalTroopsCountOfShip(ship, spawnableReservesOnly);
			}
			return 0;
		}

		// Token: 0x06000F33 RID: 3891 RVA: 0x00075803 File Offset: 0x00073A03
		public FormationClass GetNavalMissionTroopClass(BattleSideEnum battleSide, BasicCharacterObject agentCharacter)
		{
			return TroopClassExtensions.DismountedClass(agentCharacter.GetFormationClass());
		}

		// Token: 0x06000F34 RID: 3892 RVA: 0x00075810 File Offset: 0x00073A10
		public void FillReservedTroopsOfShip(MissionShip ship, MBList<IAgentOriginBase> reservedTroops)
		{
			NavalTeamAgents navalTeamAgents;
			this.GetTeamAgents(ship.Team.TeamSide, out navalTeamAgents);
			navalTeamAgents.FillReservedTroopsOfShip(ship, reservedTroops);
		}

		// Token: 0x06000F35 RID: 3893 RVA: 0x0007583C File Offset: 0x00073A3C
		public MBReadOnlyList<Agent> GetActiveHeroesOfShip(MissionShip ship)
		{
			if (ship.Team != null)
			{
				NavalTeamAgents navalTeamAgents;
				this.GetTeamAgents(ship.Team.TeamSide, out navalTeamAgents);
				return navalTeamAgents.GetActiveHeroesOfShip(ship);
			}
			return null;
		}

		// Token: 0x06000F36 RID: 3894 RVA: 0x00075870 File Offset: 0x00073A70
		public bool IsAgentOnAnyShip(Agent agent, out MissionShip onShip, TeamSideEnum teamSide = -1)
		{
			if (teamSide == -1)
			{
				using (List<NavalTeamAgents>.Enumerator enumerator = this._teamAgentsData.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.IsAgentOnAnyShip(agent, out onShip))
						{
							return true;
						}
					}
				}
				onShip = null;
				return false;
			}
			NavalTeamAgents navalTeamAgents;
			this.GetTeamAgents(teamSide, out navalTeamAgents);
			return navalTeamAgents.IsAgentOnAnyShip(agent, out onShip);
		}

		// Token: 0x06000F37 RID: 3895 RVA: 0x000758E8 File Offset: 0x00073AE8
		public int GetActiveHeroCountOfShip(MissionShip ship)
		{
			return this.GetActiveHeroesOfShip(ship).Count;
		}

		// Token: 0x06000F38 RID: 3896 RVA: 0x000758F8 File Offset: 0x00073AF8
		public bool IsTroopOriginInShipReserves(TeamSideEnum teamSide, IAgentOriginBase troopOrigin, out MissionShip onShip)
		{
			NavalTeamAgents navalTeamAgents;
			this.GetTeamAgents(teamSide, out navalTeamAgents);
			return navalTeamAgents.IsTroopInShipReserves(troopOrigin, out onShip);
		}

		// Token: 0x06000F39 RID: 3897 RVA: 0x00075918 File Offset: 0x00073B18
		public void AddAgentToShip(Agent agent, MissionShip targetShip)
		{
			TeamSideEnum teamSide = targetShip.Team.TeamSide;
			NavalTeamAgents navalTeamAgents;
			this.GetTeamAgents(teamSide, out navalTeamAgents);
			navalTeamAgents.AddAgentToShip(agent, targetShip);
		}

		// Token: 0x06000F3A RID: 3898 RVA: 0x00075944 File Offset: 0x00073B44
		public void RemoveAgentFromShip(Agent agent, MissionShip ship)
		{
			TeamSideEnum teamSide = ship.Team.TeamSide;
			NavalTeamAgents navalTeamAgents;
			this.GetTeamAgents(teamSide, out navalTeamAgents);
			navalTeamAgents.RemoveAgentFromShip(agent, ship);
		}

		// Token: 0x06000F3B RID: 3899 RVA: 0x00075970 File Offset: 0x00073B70
		public bool AddReservedTroopToShip(IAgentOriginBase troopOrigin, MissionShip ship)
		{
			TeamSideEnum teamSide = ship.Team.TeamSide;
			NavalTeamAgents navalTeamAgents;
			this.GetTeamAgents(teamSide, out navalTeamAgents);
			return navalTeamAgents.AddReservedTroopToShip(troopOrigin, ship);
		}

		// Token: 0x06000F3C RID: 3900 RVA: 0x0007599C File Offset: 0x00073B9C
		public bool IsAgentOnAnyShip(IAgentOriginBase agentOrigin, out Agent foundAgent, out MissionShip onShip, TeamSideEnum teamSide = -1)
		{
			if (teamSide == -1)
			{
				using (List<NavalTeamAgents>.Enumerator enumerator = this._teamAgentsData.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.IsAgentOnAnyShip(agentOrigin, out foundAgent, out onShip))
						{
							return true;
						}
					}
				}
				foundAgent = null;
				onShip = null;
				return false;
			}
			NavalTeamAgents navalTeamAgents;
			this.GetTeamAgents(teamSide, out navalTeamAgents);
			return navalTeamAgents.IsAgentOnAnyShip(agentOrigin, out foundAgent, out onShip);
		}

		// Token: 0x06000F3D RID: 3901 RVA: 0x00075A18 File Offset: 0x00073C18
		public int GetActiveAgentCountOfShip(MissionShip ship)
		{
			if (ship.Team != null)
			{
				NavalTeamAgents navalTeamAgents;
				this.GetTeamAgents(ship.Team.TeamSide, out navalTeamAgents);
				return navalTeamAgents.GetActiveTroopsCountOfShip(ship);
			}
			return 0;
		}

		// Token: 0x06000F3E RID: 3902 RVA: 0x00075A4C File Offset: 0x00073C4C
		public void RemoveAllReservedTroopsFromShip(MissionShip ship)
		{
			TeamSideEnum teamSide = ship.Team.TeamSide;
			NavalTeamAgents navalTeamAgents;
			this.GetTeamAgents(teamSide, out navalTeamAgents);
			navalTeamAgents.RemoveAllReservedTroopsFromShip(ship);
		}

		// Token: 0x06000F3F RID: 3903 RVA: 0x00075A78 File Offset: 0x00073C78
		public bool TransferAgentToShip(Agent agent, MissionShip ship)
		{
			TeamSideEnum teamSide = ship.Team.TeamSide;
			NavalTeamAgents navalTeamAgents;
			this.GetTeamAgents(teamSide, out navalTeamAgents);
			return navalTeamAgents.TransferAgentToShip(agent, ship, false);
		}

		// Token: 0x06000F40 RID: 3904 RVA: 0x00075AA4 File Offset: 0x00073CA4
		public int SpawnNextBatch(TeamSideEnum teamSide, bool isReinforcement = false, MBList<Agent> spawnedAgents = null)
		{
			NavalTeamAgents navalTeamAgents;
			this.GetTeamAgents(teamSide, out navalTeamAgents);
			return navalTeamAgents.SpawnNextBatch(isReinforcement, spawnedAgents);
		}

		// Token: 0x06000F41 RID: 3905 RVA: 0x00075AC4 File Offset: 0x00073CC4
		public int CheckSpawnReinforcements(TeamSideEnum teamSide, MBList<Agent> spawnedAgents = null)
		{
			NavalTeamAgents navalTeamAgents;
			this.GetTeamAgents(teamSide, out navalTeamAgents);
			return navalTeamAgents.CheckSpawnReinforcements(spawnedAgents);
		}

		// Token: 0x06000F42 RID: 3906 RVA: 0x00075AE4 File Offset: 0x00073CE4
		public void InitializeReinforcementTimers(TeamSideEnum teamSide, bool randomizeTimers = true, bool autoComputeDurations = true)
		{
			NavalTeamAgents navalTeamAgents;
			this.GetTeamAgents(teamSide, out navalTeamAgents);
			navalTeamAgents.InitializeReinforcementTimers(randomizeTimers, autoComputeDurations);
		}

		// Token: 0x06000F43 RID: 3907 RVA: 0x00075B04 File Offset: 0x00073D04
		internal void AssignCaptainToShip(Agent agent, MissionShip ship, MissionShip captainsCurrentShip = null)
		{
			TeamSideEnum teamSide = ship.Team.TeamSide;
			NavalTeamAgents navalTeamAgents;
			this.GetTeamAgents(teamSide, out navalTeamAgents);
			navalTeamAgents.AssignCaptainToShip(agent, ship, false, captainsCurrentShip);
		}

		// Token: 0x06000F44 RID: 3908 RVA: 0x00075B34 File Offset: 0x00073D34
		internal void AssignCaptainToShipForDeploymentMode(Agent agent, MissionShip targetShip, MissionShip captainsCurrentShip = null)
		{
			TeamSideEnum teamSide = targetShip.Team.TeamSide;
			MissionShip formationShip = agent.GetComponent<AgentNavalComponent>().FormationShip;
			NavalTeamAgents navalTeamAgents;
			this.GetTeamAgents(teamSide, out navalTeamAgents);
			navalTeamAgents.AssignCaptainToShip(agent, targetShip, true, captainsCurrentShip);
			navalTeamAgents.AssignAndTeleportCrewToShipMachines(targetShip);
			if (formationShip != targetShip)
			{
				navalTeamAgents.AssignAndTeleportCrewToShipMachines(formationShip);
			}
		}

		// Token: 0x06000F45 RID: 3909 RVA: 0x00075B80 File Offset: 0x00073D80
		internal void UnassignCaptainOfShip(MissionShip targetShip)
		{
			TeamSideEnum teamSide = targetShip.Team.TeamSide;
			NavalTeamAgents navalTeamAgents;
			this.GetTeamAgents(teamSide, out navalTeamAgents);
			navalTeamAgents.UnassignCaptainOfShip(targetShip);
		}

		// Token: 0x06000F46 RID: 3910 RVA: 0x00075BAC File Offset: 0x00073DAC
		internal void UnassignCaptainOfShipForDeploymentMode(MissionShip targetShip)
		{
			TeamSideEnum teamSide = targetShip.Team.TeamSide;
			NavalTeamAgents navalTeamAgents;
			this.GetTeamAgents(teamSide, out navalTeamAgents);
			navalTeamAgents.UnassignCaptainOfShip(targetShip);
			navalTeamAgents.AssignAndTeleportCrewToShipMachines(targetShip);
		}

		// Token: 0x06000F47 RID: 3911 RVA: 0x00075BE0 File Offset: 0x00073DE0
		public void SetDeploymentMode(bool value)
		{
			if (this._isDeploymentMode != value)
			{
				if (!value)
				{
					foreach (NavalTeamAgents navalTeamAgents in this._teamAgentsData)
					{
						navalTeamAgents.OnEndDeploymentMode();
					}
				}
				this._isDeploymentMode = value;
			}
		}

		// Token: 0x06000F48 RID: 3912 RVA: 0x00075C44 File Offset: 0x00073E44
		public void AddTroopOrigin(TeamSideEnum teamSide, IAgentOriginBase troopOrigin)
		{
			NavalTeamAgents navalTeamAgents;
			this.GetTeamAgents(teamSide, out navalTeamAgents);
			navalTeamAgents.AddTroopOrigin(troopOrigin);
		}

		// Token: 0x06000F49 RID: 3913 RVA: 0x00075C64 File Offset: 0x00073E64
		public void AddTroopOrigins(TeamSideEnum teamSide, MBList<IAgentOriginBase> troopOrigins)
		{
			NavalTeamAgents navalTeamAgents;
			this.GetTeamAgents(teamSide, out navalTeamAgents);
			foreach (IAgentOriginBase agentOriginBase in troopOrigins)
			{
				navalTeamAgents.AddTroopOrigin(agentOriginBase);
			}
		}

		// Token: 0x06000F4A RID: 3914 RVA: 0x00075CBC File Offset: 0x00073EBC
		public void UnassignTroops(TeamSideEnum teamSide)
		{
			NavalTeamAgents navalTeamAgents;
			this.GetTeamAgents(teamSide, out navalTeamAgents);
			navalTeamAgents.UnassignTroops();
		}

		// Token: 0x06000F4B RID: 3915 RVA: 0x00075CDC File Offset: 0x00073EDC
		public bool SpawnExistingHero(IAgentOriginBase heroOrigin, MissionShip ship, out Agent spawnedHero)
		{
			TeamSideEnum teamSide = ship.Team.TeamSide;
			NavalTeamAgents navalTeamAgents;
			this.GetTeamAgents(teamSide, out navalTeamAgents);
			return navalTeamAgents.SpawnExistingHero(heroOrigin, ship, out spawnedHero);
		}

		// Token: 0x06000F4C RID: 3916 RVA: 0x00075D08 File Offset: 0x00073F08
		public void AutoComputeDesiredTroopCountsPerShip(TeamSideEnum teamSide, bool loadBalanceShips = true)
		{
			NavalTeamAgents navalTeamAgents;
			this.GetTeamAgents(teamSide, out navalTeamAgents);
			int num = this.ComputeTeamTroopLimitAccordingToBattleSize(teamSide);
			navalTeamAgents.AutoComputeDesiredTroopCountsPerShip(loadBalanceShips, num);
		}

		// Token: 0x06000F4D RID: 3917 RVA: 0x00075D30 File Offset: 0x00073F30
		public void AssignTroops(TeamSideEnum teamSide, bool useDynamicTroopTraits = false)
		{
			NavalTeamAgents navalTeamAgents;
			this.GetTeamAgents(teamSide, out navalTeamAgents);
			navalTeamAgents.AssignTroops(useDynamicTroopTraits);
		}

		// Token: 0x06000F4E RID: 3918 RVA: 0x00075D4E File Offset: 0x00073F4E
		internal void InvokeAgentRemovedFromShip(Agent agent, MissionShip ship)
		{
			ship.InvalidateActiveFormationTroopOnShipCache();
			Action<Agent, MissionShip> agentRemovedFromShip = this.AgentRemovedFromShip;
			if (agentRemovedFromShip == null)
			{
				return;
			}
			agentRemovedFromShip(agent, ship);
		}

		// Token: 0x06000F4F RID: 3919 RVA: 0x00075D68 File Offset: 0x00073F68
		internal void InvokeAgentAddedToShip(Agent agent, MissionShip ship)
		{
			ship.InvalidateActiveFormationTroopOnShipCache();
			Action<Agent, MissionShip> agentAddedToShip = this.AgentAddedToShip;
			if (agentAddedToShip == null)
			{
				return;
			}
			agentAddedToShip(agent, ship);
		}

		// Token: 0x06000F50 RID: 3920 RVA: 0x00075D82 File Offset: 0x00073F82
		internal void InvokeTroopRemovedFromReserves(IAgentOriginBase troop, MissionShip ship)
		{
			Action<IAgentOriginBase, MissionShip> troopRemovedFromReserves = this.TroopRemovedFromReserves;
			if (troopRemovedFromReserves == null)
			{
				return;
			}
			troopRemovedFromReserves(troop, ship);
		}

		// Token: 0x06000F51 RID: 3921 RVA: 0x00075D96 File Offset: 0x00073F96
		internal void InvokeTroopAddedToReserves(IAgentOriginBase troop, MissionShip ship)
		{
			Action<IAgentOriginBase, MissionShip> troopAddedToReserves = this.TroopAddedToReserves;
			if (troopAddedToReserves == null)
			{
				return;
			}
			troopAddedToReserves(troop, ship);
		}

		// Token: 0x06000F52 RID: 3922 RVA: 0x00075DAC File Offset: 0x00073FAC
		public bool IsAgentUnassigned(Agent agent, TeamSideEnum teamSide = -1)
		{
			if (teamSide == -1)
			{
				using (List<NavalTeamAgents>.Enumerator enumerator = this._teamAgentsData.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.IsTroopUnassigned(agent.Origin))
						{
							return true;
						}
					}
				}
				return false;
			}
			NavalTeamAgents navalTeamAgents;
			this.GetTeamAgents(teamSide, out navalTeamAgents);
			return navalTeamAgents.IsTroopUnassigned(agent.Origin);
		}

		// Token: 0x06000F53 RID: 3923 RVA: 0x00075E28 File Offset: 0x00074028
		private int ComputeTeamTroopLimitAccordingToBattleSize(TeamSideEnum teamSide)
		{
			int realBattleSizeForNaval = BannerlordConfig.GetRealBattleSizeForNaval();
			int num = 0;
			int num2 = 0;
			foreach (NavalTeamAgents navalTeamAgents in this._teamAgentsData)
			{
				num += navalTeamAgents.AllTroopOrigins.Count;
				if (navalTeamAgents.TeamSide == teamSide)
				{
					num2 = navalTeamAgents.AllTroopOrigins.Count;
				}
			}
			float num3 = (float)num2 * (float)realBattleSizeForNaval / (float)num;
			int num4 = (int)num3;
			float num5 = (float)(num - num2) * (float)realBattleSizeForNaval / (float)num;
			int num6 = (int)num5;
			if (num4 + num6 < realBattleSizeForNaval)
			{
				float num7 = num3 - (float)num4;
				float num8 = num5 - (float)num6;
				if (num7 > num8)
				{
					num4++;
				}
			}
			return num4;
		}

		// Token: 0x06000F54 RID: 3924 RVA: 0x00075EE8 File Offset: 0x000740E8
		public void SetDesiredTroopCountOfShip(MissionShip ship, int desiredTroopCount)
		{
			NavalTeamAgents navalTeamAgents;
			this.GetTeamAgents(ship.Team.TeamSide, out navalTeamAgents);
			navalTeamAgents.SetDesiredTroopCountOfShip(ship, desiredTroopCount);
		}

		// Token: 0x06000F55 RID: 3925 RVA: 0x00075F14 File Offset: 0x00074114
		public void SetTroopClassFilter(MissionShip ship, TroopTraitsMask troopClassFilter)
		{
			NavalTeamAgents navalTeamAgents;
			this.GetTeamAgents(ship.Team.TeamSide, out navalTeamAgents);
			navalTeamAgents.SetTroopClassFilter(ship, troopClassFilter);
		}

		// Token: 0x06000F56 RID: 3926 RVA: 0x00075F40 File Offset: 0x00074140
		public void SetTroopTraitsFilter(MissionShip ship, TroopTraitsMask troopTraitsFilter)
		{
			NavalTeamAgents navalTeamAgents;
			this.GetTeamAgents(ship.Team.TeamSide, out navalTeamAgents);
			navalTeamAgents.SetTroopTraitsFilter(ship, troopTraitsFilter);
		}

		// Token: 0x06000F57 RID: 3927 RVA: 0x00075F6C File Offset: 0x0007416C
		internal void AssignAndTeleportCrewToShipMachines(MissionShip ship)
		{
			TeamSideEnum teamSide = ship.Team.TeamSide;
			NavalTeamAgents navalTeamAgents;
			this.GetTeamAgents(teamSide, out navalTeamAgents);
			navalTeamAgents.AssignAndTeleportCrewToShipMachines(ship);
		}

		// Token: 0x06000F58 RID: 3928 RVA: 0x00075F98 File Offset: 0x00074198
		internal void AssignAndTeleportCrewToShipMachines(TeamSideEnum teamSide)
		{
			NavalTeamAgents navalTeamAgents;
			this.GetTeamAgents(teamSide, out navalTeamAgents);
			navalTeamAgents.AssignAndTeleportCrewToShipMachines();
		}

		// Token: 0x06000F59 RID: 3929 RVA: 0x00075FB8 File Offset: 0x000741B8
		private bool GetTeamAgents(TeamSideEnum teamSide, out NavalTeamAgents teamAgents)
		{
			teamAgents = this._teamAgentsData.FirstOrDefault<NavalTeamAgents>((NavalTeamAgents mts) => mts.TeamSide == teamSide);
			return teamAgents != null;
		}

		// Token: 0x06000F5A RID: 3930 RVA: 0x00075FF0 File Offset: 0x000741F0
		internal void OnAgentSteppedShipChanged(Agent agent, MissionShip newShip)
		{
			agent.UpdateAgentStats();
		}

		// Token: 0x06000F5B RID: 3931 RVA: 0x00075FF8 File Offset: 0x000741F8
		private void OnShipSpawned(MissionShip ship)
		{
			NavalTeamAgents navalTeamAgents;
			if (this.GetTeamAgents(ship.Team.TeamSide, out navalTeamAgents))
			{
				navalTeamAgents.OnShipSpawned(ship, this._ignoreTroopCapacities[navalTeamAgents.TeamSide]);
			}
		}

		// Token: 0x06000F5C RID: 3932 RVA: 0x0007602E File Offset: 0x0007422E
		public static float ComputeReinforcementSpawnDuration(int reservedTroopCount)
		{
			return 0.5f + 2.5f / (float)(1 + reservedTroopCount / 50);
		}

		// Token: 0x06000F5D RID: 3933 RVA: 0x00076044 File Offset: 0x00074244
		private void OnShipRemoved(MissionShip ship)
		{
			NavalTeamAgents navalTeamAgents;
			if (ship.Team != null && this.GetTeamAgents(ship.Team.TeamSide, out navalTeamAgents))
			{
				navalTeamAgents.OnShipRemoved(ship);
			}
		}

		// Token: 0x06000F5E RID: 3934 RVA: 0x00076078 File Offset: 0x00074278
		private void OnShipTransferredToFormation(MissionShip ship, Formation oldFormation)
		{
			NavalTeamAgents navalTeamAgents;
			this.GetTeamAgents(ship.Team.TeamSide, out navalTeamAgents);
			navalTeamAgents.OnShipTransferredToFormation(ship, oldFormation);
		}

		// Token: 0x06000F5F RID: 3935 RVA: 0x000760A4 File Offset: 0x000742A4
		private void OnShipTransferredToTeam(MissionShip ship, Team oldTeam, Formation oldFormation)
		{
			if (oldTeam != null)
			{
				NavalTeamAgents navalTeamAgents;
				this.GetTeamAgents(oldTeam.TeamSide, out navalTeamAgents);
				navalTeamAgents.OnShipRemoved(ship);
			}
			NavalTeamAgents navalTeamAgents2;
			this.GetTeamAgents(ship.Team.TeamSide, out navalTeamAgents2);
			navalTeamAgents2.OnShipSpawned(ship, this._ignoreTroopCapacities[navalTeamAgents2.TeamSide]);
		}

		// Token: 0x06000F60 RID: 3936 RVA: 0x000760F4 File Offset: 0x000742F4
		private void OnShipCaptured(MissionShip ship, MissionShip ship2, Formation formation, Formation formation2)
		{
			if (formation != null)
			{
				NavalTeamAgents navalTeamAgents;
				this.GetTeamAgents(formation.Team.TeamSide, out navalTeamAgents);
				navalTeamAgents.OnShipCaptured(ship, ship2);
				formation.ApplyActionOnEachUnit(delegate(Agent agent)
				{
					agent.GetComponent<AgentNavalComponent>().OnShipCaptured();
				}, null);
			}
			NavalTeamAgents navalTeamAgents2;
			this.GetTeamAgents(formation2.Team.TeamSide, out navalTeamAgents2);
			navalTeamAgents2.OnShipCaptured(ship2, ship);
			formation2.ApplyActionOnEachUnit(delegate(Agent agent)
			{
				agent.GetComponent<AgentNavalComponent>().OnShipCaptured();
			}, null);
		}

		// Token: 0x06000F61 RID: 3937 RVA: 0x0007618C File Offset: 0x0007438C
		private void OnShipTeleported(MissionShip ship, MatrixFrame oldFrame, MatrixFrame targetFrame)
		{
			MBReadOnlyList<Agent> activeAgentsOfShip = this.GetActiveAgentsOfShip(ship);
			if (activeAgentsOfShip != null)
			{
				foreach (Agent agent in activeAgentsOfShip)
				{
					MatrixFrame matrixFrame = agent.GetWorldFrame().ToGroundMatrixFrame();
					MatrixFrame matrixFrame2 = oldFrame.TransformToLocal(ref matrixFrame);
					MatrixFrame matrixFrame3 = targetFrame.TransformToParent(ref matrixFrame2);
					NavalAgentsLogic.TeleportAgentToFrame(agent, in matrixFrame3);
				}
			}
		}

		// Token: 0x06000F62 RID: 3938 RVA: 0x0007620C File Offset: 0x0007440C
		private void OnShipPreparedForAbandonment(MissionShip ship)
		{
			this.SetSpawnReinforcementsForShip(ship, false);
		}

		// Token: 0x06000F63 RID: 3939 RVA: 0x00076218 File Offset: 0x00074418
		internal static float GetAgentPriority(Agent agent)
		{
			if (agent.IsMainAgent)
			{
				return 500f;
			}
			if (agent.IsPlayerTroop)
			{
				return 400f;
			}
			if (agent.Formation != null && agent == agent.Formation.Captain)
			{
				return 300f;
			}
			return (agent.IsHero ? 100f : 0f) + agent.Origin.Troop.GetPower();
		}

		// Token: 0x06000F64 RID: 3940 RVA: 0x00076282 File Offset: 0x00074482
		private void OnMissionEnd()
		{
			this.SetDeploymentMode(false);
		}

		// Token: 0x06000F65 RID: 3941 RVA: 0x0007628C File Offset: 0x0007448C
		internal static void TeleportAgentToFrame(Agent agent, in MatrixFrame teleportFrame)
		{
			Vec2 vec;
			Vec3 vec2;
			if (agent.Position.NearlyEquals(ref teleportFrame.origin, 0.001f))
			{
				vec = agent.GetMovementDirection();
				vec2 = teleportFrame.rotation.f;
				if (vec.NearlyEquals(vec2.AsVec2, 0.001f))
				{
					return;
				}
			}
			agent.TeleportToPosition(teleportFrame.origin);
			agent.LookDirection = teleportFrame.rotation.f;
			vec2 = teleportFrame.rotation.f;
			vec = vec2.AsVec2;
			vec = vec.Normalized();
			agent.SetMovementDirection(ref vec);
		}

		// Token: 0x06000F66 RID: 3942 RVA: 0x00076320 File Offset: 0x00074520
		internal static void TeleportAndAssignAgentToMachine(Agent agent, NavalShipAgents agentShip, UsableMachine shipMachine)
		{
			NavalAgentsLogic.TryStopMachineUseAndReattachAgent(agent);
			bool flag;
			NavalAgentsLogic.TryUseMachineAndDetachAgent(agent, agentShip, shipMachine, true, out flag);
		}

		// Token: 0x06000F67 RID: 3943 RVA: 0x0007633F File Offset: 0x0007453F
		internal static void TryStopMachineUseAndReattachAgent(Agent agent)
		{
			if (agent.IsDetachedFromFormation)
			{
				agent.TryAttachToFormation();
			}
			if (agent.InteractingWithAnyGameObject() && !(agent.CurrentlyUsedGameObject is SpawnedItemEntity))
			{
				agent.StopUsingGameObjectMT(true, 1);
			}
		}

		// Token: 0x06000F68 RID: 3944 RVA: 0x00076370 File Offset: 0x00074570
		internal static bool TryUseMachineAndDetachAgent(Agent agent, NavalShipAgents ownerShipAgents, UsableMachine machine, bool teleportAndUseInstantly, out bool isDetached)
		{
			isDetached = false;
			if (machine.PilotAgent == null && !machine.PilotStandingPoint.IsAIMovingTo(agent))
			{
				if (agent.IsAIControlled && agent.IsDetachableFromFormation && ownerShipAgents.Ship.Formation != null && ownerShipAgents.Ship.Formation == agent.Formation)
				{
					machine.AddAgentAtSlotIndex(agent, 0);
					isDetached = true;
				}
				agent.UseGameObject(machine.PilotStandingPoint, -1);
				if (teleportAndUseInstantly)
				{
					machine.OnPilotAssignedDuringSpawn();
				}
				return true;
			}
			return false;
		}

		// Token: 0x04000947 RID: 2375
		public const float MinReinforcementsDuration = 0.5f;

		// Token: 0x04000948 RID: 2376
		public const float MaxReinforcementsDuration = 3f;

		// Token: 0x0400094D RID: 2381
		private readonly bool[] _ignoreTroopCapacities;

		// Token: 0x0400094F RID: 2383
		private readonly MBList<NavalTeamAgents> _teamAgentsData;

		// Token: 0x04000950 RID: 2384
		private bool _isDeploymentMode;
	}
}
